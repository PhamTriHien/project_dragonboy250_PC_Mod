using System;
using System.Collections.Generic;

public static class ModSetActivator
{
	public enum SetActivatorState
	{
		Idle,
		Farming,
		BagFull_SavingLocation,
		MovingToUron,
		InteractingUron,
		SellingJunk,
		ReturningToFarm,
		SwitchingZone,
		TeleportingToSpot
	}

	public static bool isActive = false;
	public static bool autoSellJunk = true;
	public static int minStarToKeep = 1; // 0: Bán cả đồ sao, 1: Giữ >= 1 sao, 2: Giữ >= 2 sao, 3: Giữ >= 3 sao
	public static bool showItemId = true;

	public static SetActivatorState currentState = SetActivatorState.Idle;

	public static int savedFarmMapId = -1;
	public static int savedFarmZoneId = -1;
	public static int savedFarmX = -1;
	public static int savedFarmY = -1;

	public static bool isSellingJunk = false;
	private static int currentSellingBagIndex = -1;
	private static long lastSellActionTime = 0;
	private static int sellFailTimeoutCount = 0;

	private static long stateTimer = 0;
	private static long lastZoneAttemptTime = 0;
	private static int zoneAttemptCount = 0;

	public static bool isBusy
	{
		get
		{
			return isActive && currentState != SetActivatorState.Idle && currentState != SetActivatorState.Farming;
		}
	}

	public static void SaveCurrentFarmPosition()
	{
		Char me = Char.myCharz();
		if (me == null || !ModMenu.IsInGame())
		{
			return;
		}

		savedFarmMapId = TileMap.mapID;
		savedFarmZoneId = TileMap.zoneID;
		savedFarmX = me.cx;
		savedFarmY = me.cy;
		ModConfig.SaveConfig();

		GameScr.info1.addInfo("Úp Set KH: Đã lưu bãi " + ModNextMap.GetMapName(savedFarmMapId) + " [K." + savedFarmZoneId + "] (" + savedFarmX + ", " + savedFarmY + ")", 0);
		SoundMn.gI().buttonClick();
	}

	public static void ResetPosition()
	{
		savedFarmMapId = -1;
		savedFarmZoneId = -1;
		savedFarmX = -1;
		savedFarmY = -1;
		currentState = SetActivatorState.Idle;
		isSellingJunk = false;
		ModConfig.SaveConfig();

		GameScr.info1.addInfo("Úp Set KH: Đã xóa bãi úp lưu!", 0);
		SoundMn.gI().buttonClick();
	}

	public static void ToggleActive()
	{
		isActive = !isActive;
		if (isActive)
		{
			if (savedFarmMapId == -1)
			{
				SaveCurrentFarmPosition();
			}
			currentState = SetActivatorState.Farming;
			GameScr.info1.addInfo("Auto Úp Set Kích Hoạt: BẬT", 0);
		}
		else
		{
			currentState = SetActivatorState.Idle;
			isSellingJunk = false;
			if (ModNextMap.isNextMapActive)
			{
				ModNextMap.StopNextMap();
			}
			GameScr.info1.addInfo("Auto Úp Set Kích Hoạt: TẮT", 0);
		}
		ModConfig.SaveConfig();
		SoundMn.gI().buttonClick();
	}

	public static void CycleMinStar()
	{
		minStarToKeep++;
		if (minStarToKeep > 3)
		{
			minStarToKeep = 0;
		}
		ModConfig.SaveConfig();
		SoundMn.gI().buttonClick();
	}

	public static string GetMinStarText()
	{
		if (minStarToKeep == 0) return "Bán Hết Sao";
		return "Giữ >=" + minStarToKeep + " Sao";
	}

	public static int GetFreeBagSlots()
	{
		try
		{
			Char me = Char.myCharz();
			if (me == null || me.arrItemBag == null) return 0;
			int free = 0;
			for (int i = 0; i < me.arrItemBag.Length; i++)
			{
				if (me.arrItemBag[i] == null)
				{
					free++;
				}
			}
			return free;
		}
		catch
		{
			return 0;
		}
	}

	public static bool IsSetKichHoat(Item item)
	{
		if (item == null || item.itemOption == null) return false;
		for (int i = 0; i < item.itemOption.Length; i++)
		{
			ItemOption opt = item.itemOption[i];
			if (opt == null || opt.optionTemplate == null) continue;
			int optId = opt.optionTemplate.id;

			// Dải ID option của các bộ set kích hoạt chuẩn trong NRO
			if ((optId >= 127 && optId <= 144) || (optId >= 210 && optId <= 225))
			{
				return true;
			}

			// So khớp tên option theo từ khóa set kích hoạt
			string optName = opt.optionTemplate.name;
			if (!string.IsNullOrEmpty(optName))
			{
				string lower = optName.ToLower();
				if (lower.Contains("kích hoạt") || lower.Contains("kich hoat") ||
					lower.Contains("kirin") || lower.Contains("songoku") ||
					lower.Contains("thiên xin hăng") || lower.Contains("thien xin hang") ||
					lower.Contains("cadic") || lower.Contains("nappa") ||
					lower.Contains("kakarot") || lower.Contains("pikkoro") ||
					lower.Contains("ốc tiêu") || lower.Contains("oc tieu") ||
					lower.Contains("dende") || lower.Contains("zelot") ||
					lower.Contains("songohan") || lower.StartsWith("set ") || lower.Contains("set:"))
				{
					return true;
				}
			}
		}
		return false;
	}

	public static int GetItemStarCount(Item item)
	{
		if (item == null || item.itemOption == null) return 0;
		int stars = 0;
		for (int i = 0; i < item.itemOption.Length; i++)
		{
			ItemOption opt = item.itemOption[i];
			if (opt == null || opt.optionTemplate == null) continue;
			int optId = opt.optionTemplate.id;

			if (optId == 102 && opt.param > stars)
			{
				stars = opt.param;
			}
			if (optId == 107 && opt.param > stars)
			{
				stars = opt.param;
			}
			if ((optId == 34 || optId == 35 || optId == 36) && opt.param > stars)
			{
				stars = opt.param;
			}
		}
		return stars;
	}

	public static bool IsImportantItem(Item item)
	{
		if (item == null || item.template == null) return true;
		if (item.upgrade > 0) return true;
		if (item.isLock) return true;

		int type = item.template.type;
		// CHỈ cho phép bán nếu là trang bị thường (0: Áo, 1: Quần, 2: Găng, 3: Giày, 4: Rada)
		if (type < 0 || type > 4)
		{
			return true;
		}

		int id = item.template.id;
		if (id == 457) return true; // Thỏi vàng
		if (id >= 14 && id <= 20) return true; // Ngọc rồng
		if (id >= 13 && id <= 20) return true; // Đậu thần
		if (id >= 220 && id <= 226) return true; // Đá nâng cấp
		if (id >= 441 && id <= 447) return true; // Đá pha lê
		if (id == 193 || id == 194) return true; // Capsule
		if (id >= 380 && id <= 385) return true; // Potion buff
		if (id == 921 || id == 454 || id == 933) return true; // Bông tai Porata

		if (item.isHaveOption(30)) return true; // Không thể giao dịch
		if (item.isHaveOption(73) || item.isHaveOption(93)) return true; // Hạn sử dụng

		return false;
	}

	public static bool IsJunkItem(Item item)
	{
		if (item == null || item.template == null) return false;
		if (IsImportantItem(item)) return false;
		if (IsSetKichHoat(item)) return false;
		if (minStarToKeep > 0 && GetItemStarCount(item) >= minStarToKeep) return false;

		return true;
	}

	public static int GetTargetSpaceshipMapId()
	{
		Char me = Char.myCharz();
		if (me == null) return 24;

		// Map 24: Trạm tàu Trái Đất, 25: Trạm tàu Namếc, 26: Trạm tàu Xayda
		int currentMap = TileMap.mapID;
		if (currentMap >= 8 && currentMap <= 13) return 25;
		if (currentMap >= 15 && currentMap <= 20) return 26;
		if (currentMap >= 1 && currentMap <= 6) return 24;

		if (me.cgender == 1) return 25;
		if (me.cgender == 2) return 26;
		return 24;
	}

	public static Npc FindUronNpc()
	{
		if (GameScr.vNpc == null) return null;
		for (int i = 0; i < GameScr.vNpc.size(); i++)
		{
			Npc npc = (Npc)GameScr.vNpc.elementAt(i);
			if (npc == null || npc.template == null) continue;
			string name = npc.template.name.ToLower();
			if (name.Contains("uron") || name.Contains("urôn") || name.Contains("u-rôn") || name.Contains("oolong"))
			{
				return npc;
			}
		}

		for (int i = 0; i < GameScr.vNpc.size(); i++)
		{
			Npc npc = (Npc)GameScr.vNpc.elementAt(i);
			if (npc == null || npc.template == null) continue;
			if (npc.template.npcTemplateId == 19 || npc.template.npcTemplateId == 18)
			{
				return npc;
			}
		}
		return null;
	}

	public static void StartGoSellJunkNow()
	{
		Char me = Char.myCharz();
		if (me == null || !ModMenu.IsInGame()) return;

		savedFarmMapId = TileMap.mapID;
		savedFarmZoneId = TileMap.zoneID;
		savedFarmX = me.cx;
		savedFarmY = me.cy;
		ModConfig.SaveConfig();

		int targetMap = GetTargetSpaceshipMapId();
		currentState = SetActivatorState.MovingToUron;
		stateTimer = mSystem.currentTimeMillis();
		GameScr.info1.addInfo("Bắt đầu di chuyển đến Trạm Tàu Vũ Trụ gặp Urôn để bán đồ...", 0);
		ModNextMap.StartNextMap(targetMap);
	}

	public static void OnSaleRequestReceived(sbyte type, short id)
	{
		if (!isSellingJunk) return;

		try
		{
			// Tự động gửi packet xác nhận bán đồ
			Service.gI().saleItem(1, type, id);
			lastSellActionTime = mSystem.currentTimeMillis();
			sellFailTimeoutCount = 0;
			GameCanvas.endDlg();
		}
		catch
		{
		}
	}

	public static void Update()
	{
		if (!ModMenu.IsInGame() || ModAutoBuyBua.isBusy) return;
		Char me = Char.myCharz();
		if (me == null || me.isDie) return;

		long now = mSystem.currentTimeMillis();

		switch (currentState)
		{
			case SetActivatorState.Idle:
				if (isActive)
				{
					currentState = SetActivatorState.Farming;
				}
				break;

			case SetActivatorState.Farming:
				if (!isActive)
				{
					currentState = SetActivatorState.Idle;
					break;
				}

				// Kiểm tra hành trang nếu bật Auto Bán Rác
				if (autoSellJunk && GetFreeBagSlots() <= 1)
				{
					GameScr.info1.addInfo("Hành trang đã đầy! Tự động lưu bãi và đi bán đồ rác tại Urôn...", 0);
					StartGoSellJunkNow();
				}
				break;

			case SetActivatorState.MovingToUron:
				int targetStation = GetTargetSpaceshipMapId();
				if (TileMap.mapID == targetStation)
				{
					if (ModNextMap.isNextMapActive)
					{
						ModNextMap.StopNextMap();
					}
					currentState = SetActivatorState.InteractingUron;
					stateTimer = now;
				}
				else if (!ModNextMap.isNextMapActive)
				{
					// Nếu chưa đến nơi mà nextmap dừng, kích hoạt lại
					if (now - stateTimer > 3000)
					{
						stateTimer = now;
						ModNextMap.StartNextMap(targetStation);
					}
				}
				break;

			case SetActivatorState.InteractingUron:
				Npc uron = FindUronNpc();
				if (uron != null)
				{
					int dist = Res.distance(me.cx, me.cy, uron.cx, uron.cy);
					if (dist > 30)
					{
						me.cx = uron.cx;
						me.cy = uron.cy;
						Service.gI().charMoveTo(uron.cx, uron.cy);
					}
					me.focusManualTo(uron);

					if (now - stateTimer > 1000)
					{
						stateTimer = now;
						int npcTmplId = (uron.template != null) ? uron.template.npcTemplateId : uron.npcId;
						Service.gI().openMenu(npcTmplId);

						currentState = SetActivatorState.SellingJunk;
						isSellingJunk = true;
						lastSellActionTime = now;
						currentSellingBagIndex = -1;
						sellFailTimeoutCount = 0;
					}
				}
				else
				{
					// Không tìm thấy Urôn trong map, chờ hoặc thử tìm lại
					if (now - stateTimer > 5000)
					{
						GameScr.info1.addInfo("Không tìm thấy NPC Urôn tại map này! Quay lại bãi úp...", 0);
						currentState = SetActivatorState.ReturningToFarm;
						stateTimer = now;
						ModNextMap.StartNextMap(savedFarmMapId);
					}
				}
				break;

			case SetActivatorState.SellingJunk:
				if (now - lastSellActionTime < 300)
				{
					break;
				}

				// Tìm món đồ rác tiếp theo trong hành trang
				int junkIndex = -1;
				Item junkItem = null;
				if (me.arrItemBag != null)
				{
					for (int i = 0; i < me.arrItemBag.Length; i++)
					{
						Item item = me.arrItemBag[i];
						if (item != null && IsJunkItem(item))
						{
							junkIndex = i;
							junkItem = item;
							break;
						}
					}
				}

				if (junkIndex != -1 && junkItem != null)
				{
					currentSellingBagIndex = junkIndex;
					lastSellActionTime = now;
					Service.gI().saleItem(0, 1, (short)junkIndex);
					sellFailTimeoutCount++;

					if (sellFailTimeoutCount > 15)
					{
						// Nếu kẹt bán quá nhiều lần, bỏ qua kết thúc
						GameScr.info1.addInfo("Bán đồ hoàn tất hoặc kết thúc giao dịch.", 0);
						FinishSellingAndReturn();
					}
				}
				else
				{
					// Đã bán hết đồ rác!
					GameScr.info1.addInfo("Đã bán hết đồ rác! Chuẩn bị quay lại bãi úp...", 0);
					FinishSellingAndReturn();
				}
				break;

			case SetActivatorState.ReturningToFarm:
				if (savedFarmMapId == -1)
				{
					currentState = SetActivatorState.Idle;
					break;
				}

				if (TileMap.mapID == savedFarmMapId)
				{
					if (ModNextMap.isNextMapActive)
					{
						ModNextMap.StopNextMap();
					}

					if (TileMap.zoneID != savedFarmZoneId && savedFarmZoneId != -1)
					{
						currentState = SetActivatorState.SwitchingZone;
						stateTimer = now;
						lastZoneAttemptTime = now;
						zoneAttemptCount = 0;
						Service.gI().requestChangeZone(savedFarmZoneId, -1);
						GameScr.info1.addInfo("Đã về map bãi úp. Đang đổi sang Khu " + savedFarmZoneId + "...", 0);
					}
					else
					{
						currentState = SetActivatorState.TeleportingToSpot;
						stateTimer = now;
					}
				}
				else if (!ModNextMap.isNextMapActive)
				{
					if (now - stateTimer > 3000)
					{
						stateTimer = now;
						ModNextMap.StartNextMap(savedFarmMapId);
					}
				}
				break;

			case SetActivatorState.SwitchingZone:
				if (TileMap.zoneID == savedFarmZoneId)
				{
					currentState = SetActivatorState.TeleportingToSpot;
					stateTimer = now;
					break;
				}

				if (now - lastZoneAttemptTime > 2500)
				{
					lastZoneAttemptTime = now;
					zoneAttemptCount++;
					if (zoneAttemptCount <= 3)
					{
						Service.gI().requestChangeZone(savedFarmZoneId, -1);
					}
					else
					{
						GameScr.info1.addInfo("Khu " + savedFarmZoneId + " có thể đã đầy! Tiếp tục ở khu hiện tại.", 0);
						currentState = SetActivatorState.TeleportingToSpot;
						stateTimer = now;
					}
				}
				break;

			case SetActivatorState.TeleportingToSpot:
				if (savedFarmX != -1 && savedFarmY != -1)
				{
					ModTeleport.TeleportTo(savedFarmX, savedFarmY);
					GameScr.info1.addInfo("Đã về đúng vị trí bãi úp! Tiếp tục úp set kích hoạt.", 0);
					SoundMn.gI().buttonClick();
				}

				// Kích hoạt lại Tàn Sát để tự động đánh quái
				if (isActive)
				{
					ModTanSat.autoTanSat = true;
				}

				currentState = SetActivatorState.Farming;
				break;
		}
	}

	private static void FinishSellingAndReturn()
	{
		isSellingJunk = false;
		GameCanvas.endDlg();
		currentState = SetActivatorState.ReturningToFarm;
		stateTimer = mSystem.currentTimeMillis();
		ModNextMap.StartNextMap(savedFarmMapId);
	}

	public static string GetStatusText()
	{
		if (!isActive) return "Đang TẮT";
		switch (currentState)
		{
			case SetActivatorState.Farming:
				return "Đang Úp Quái (Trống: " + GetFreeBagSlots() + " ô)";
			case SetActivatorState.BagFull_SavingLocation:
				return "Full Hành Trang - Đang lưu vị trí";
			case SetActivatorState.MovingToUron:
				return "Đang đến Trạm Tàu gặp Urôn";
			case SetActivatorState.InteractingUron:
				return "Đang tương tác với Urôn";
			case SetActivatorState.SellingJunk:
				return "Đang bán đồ rác tại Urôn";
			case SetActivatorState.ReturningToFarm:
				return "Đang quay lại bản đồ bãi úp";
			case SetActivatorState.SwitchingZone:
				return "Đang đổi lại Khu " + savedFarmZoneId;
			case SetActivatorState.TeleportingToSpot:
				return "Đang teleport về đúng bãi úp";
			default:
				return "Đang chờ";
		}
	}
}
