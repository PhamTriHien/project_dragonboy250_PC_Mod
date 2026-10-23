using System;
using System.Collections.Generic;

/// <summary>
/// Module Úp Set Kích Hoạt (SKH): Quản lý nhận diện 13 bộ set kích hoạt, tự động lọc, bán đồ rác và cất trang bị SKH/Sao vào rương nhà.
/// </summary>
public static class ModSetActivator
{
	/// <summary>
	/// ENUM SetActivatorState: Định nghĩa và điều phối logic trong hệ thống game.
	/// </summary>
	public enum SetActivatorState
	{
		Idle,
		Farming,
		BagFull_SavingLocation,
		MovingToUron,
		InteractingUron,
		SellingJunk,
		MovingToHomeBox,
		InteractingBox,
		StoringToBox,
		ReturningToFarm,
		SwitchingZone,
		TeleportingToSpot
	}

	public static bool isActive = false;
	public static bool autoSellJunk = true;
	public static bool autoStoreToBox = true;
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
	private static int storeAttemptCount = 0;
	private static bool isBoxFullWarned = false;

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

	/// <summary>
	/// Thực thi phương thức nghiệp vụ SaveCurrentFarmPosition.
	/// </summary>
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

	/// <summary>
	/// Dọn dẹp và đặt lại trạng thái ban đầu cho ResetPosition.
	/// </summary>
	public static void ResetPosition()
	{
		savedFarmMapId = -1;
		savedFarmZoneId = -1;
		savedFarmX = -1;
		savedFarmY = -1;
		currentState = SetActivatorState.Idle;
		isSellingJunk = false;
		isBoxFullWarned = false;
		ModConfig.SaveConfig();

		GameScr.info1.addInfo("Úp Set KH: Đã xóa bãi úp lưu!", 0);
		SoundMn.gI().buttonClick();
	}

	/// <summary>
	/// Bật hoặc tắt trạng thái kích hoạt của ToggleActive.
	/// </summary>
	public static void ToggleActive()
	{
		isActive = !isActive;
		if (isActive)
		{
			if (savedFarmMapId == -1)
			{
				SaveCurrentFarmPosition();
			}
			isBoxFullWarned = false;
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

	/// <summary>
	/// Thực thi phương thức nghiệp vụ CycleMinStar.
	/// </summary>
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

	/// <summary>
	/// Truy xuất và trả về dữ liệu của GetMinStarText.
	/// </summary>
	public static string GetMinStarText()
	{
		if (minStarToKeep == 0) return "Bán Hết Sao";
		return "Giữ >=" + minStarToKeep + " Sao";
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của GetFreeBagSlots.
	/// </summary>
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

	/// <summary>
	/// Truy xuất số ô trống trong rương đồ tại nhà.
	/// </summary>
	public static int GetFreeBoxSlots()
	{
		try
		{
			Char me = Char.myCharz();
			if (me == null || me.arrItemBox == null) return -1;
			int free = 0;
			for (int i = 0; i < me.arrItemBox.Length; i++)
			{
				if (me.arrItemBox[i] == null)
				{
					free++;
				}
			}
			return free;
		}
		catch
		{
			return -1;
		}
	}

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của IsSetKichHoat.
	/// </summary>
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

	/// <summary>
	/// Truy xuất và trả về dữ liệu của GetItemStarCount.
	/// </summary>
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

	/// <summary>
	/// Kiểm tra xem trang bị có phải là đồ SKH hoặc đồ Sao cần cất vào rương hay không.
	/// </summary>
	public static bool IsValuableStoreItem(Item item)
	{
		if (item == null || item.template == null) return false;
		int type = item.template.type;
		if (type < 0 || type > 4) return false;
		if (IsSetKichHoat(item)) return true;
		if (minStarToKeep > 0 && GetItemStarCount(item) >= minStarToKeep) return true;
		return false;
	}

	/// <summary>
	/// Kiểm tra trong hành trang có trang bị SKH/Sao cần cất vào rương hay không.
	/// </summary>
	public static bool HasValuableStoreItemInBag(Char me)
	{
		if (me == null || me.arrItemBag == null) return false;
		for (int i = 0; i < me.arrItemBag.Length; i++)
		{
			if (me.arrItemBag[i] != null && IsValuableStoreItem(me.arrItemBag[i]))
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của IsImportantItem.
	/// </summary>
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

	private static long lastFullNonJunkWarnTime = 0;

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của IsJunkItem.
	/// </summary>
	public static bool IsJunkItem(Item item)
	{
		if (item == null || item.template == null) return false;
		if (IsImportantItem(item)) return false;
		if (IsSetKichHoat(item)) return false;
		if (minStarToKeep > 0 && GetItemStarCount(item) >= minStarToKeep) return false;

		return true;
	}

	/// <summary>
	/// Kiểm tra xem trong hành trang có ít nhất một vật phẩm rác có thể bán được hay không.
	/// </summary>
	public static bool HasSellableJunkInBag(Char me)
	{
		if (me == null || me.arrItemBag == null) return false;
		for (int i = 0; i < me.arrItemBag.Length; i++)
		{
			if (me.arrItemBag[i] != null && IsJunkItem(me.arrItemBag[i]))
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của GetTargetSpaceshipMapId.
	/// </summary>
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

	/// <summary>
	/// Truy xuất ID map Nhà theo hành tinh nhân vật (21: Trái Đất, 22: Namếc, 23: Xayda).
	/// </summary>
	public static int GetHomeMapId()
	{
		Char me = Char.myCharz();
		if (me == null) return 21;
		return 21 + me.cgender;
	}

	/// <summary>
	/// Tìm NPC Rương Đồ (templateId = 3) tại map Nhà.
	/// </summary>
	public static Npc FindHomeBoxNpc()
	{
		if (GameScr.vNpc == null) return null;
		for (int i = 0; i < GameScr.vNpc.size(); i++)
		{
			Npc npc = (Npc)GameScr.vNpc.elementAt(i);
			if (npc == null || npc.template == null) continue;
			if (npc.template.npcTemplateId == 3)
			{
				return npc;
			}
			string name = (npc.template.name ?? string.Empty).ToLower();
			if (name.Contains("rương") || name.Contains("ruong"))
			{
				return npc;
			}
		}
		return null;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ FindUronNpc.
	/// </summary>
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

	/// <summary>
	/// Thực thi phương thức nghiệp vụ StartGoSellJunkNow.
	/// </summary>
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

	/// <summary>
	/// Bắt đầu về Nhà để cất trang bị SKH / Đồ Sao vào Rương Đồ.
	/// </summary>
	public static void StartGoStoreBoxNow()
	{
		Char me = Char.myCharz();
		if (me == null || !ModMenu.IsInGame()) return;

		if (savedFarmMapId == -1)
		{
			savedFarmMapId = TileMap.mapID;
			savedFarmZoneId = TileMap.zoneID;
			savedFarmX = me.cx;
			savedFarmY = me.cy;
			ModConfig.SaveConfig();
		}

		int homeMap = GetHomeMapId();
		currentState = SetActivatorState.MovingToHomeBox;
		stateTimer = mSystem.currentTimeMillis();
		GameScr.info1.addInfo("Bắt đầu về Nhà cất đồ SKH/Đồ Sao vào Rương...", 0);
		ModNextMap.StartNextMap(homeMap);
	}

	/// <summary>
	/// Bộ xử lý sự kiện tiếp nhận và phản hồi OnSaleRequestReceived.
	/// </summary>
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

	/// <summary>
	/// Cập nhật trạng thái logic của đối tượng theo từng chu kỳ khung hình.
	/// </summary>
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

				ModDropRate.ScanBagForSpecialItems(me);

				// Kiểm tra hành trang khi đầy (<= 1 ô trống)
				if (GetFreeBagSlots() <= 1)
				{
					if (autoSellJunk && HasSellableJunkInBag(me))
					{
						GameScr.info1.addInfo("Hành trang đã đầy! Tự động lưu bãi và đi bán đồ rác tại Urôn...", 0);
						StartGoSellJunkNow();
					}
					else if (autoStoreToBox && !isBoxFullWarned && HasValuableStoreItemInBag(me))
					{
						savedFarmMapId = TileMap.mapID;
						savedFarmZoneId = TileMap.zoneID;
						savedFarmX = me.cx;
						savedFarmY = me.cy;
						ModConfig.SaveConfig();
						StartGoStoreBoxNow();
					}
					else if (now - lastFullNonJunkWarnTime > 30000)
					{
						lastFullNonJunkWarnTime = now;
						GameScr.info1.addInfo("Hành trang đã đầy vật phẩm giá trị (không còn đồ rác để bán)!", 0);
					}
				}
				break;

			case SetActivatorState.MovingToUron:
				int targetStation = GetTargetSpaceshipMapId();
				if (TileMap.mapID == targetStation && !Char.isLoadingMap && !Char.ischangingMap)
				{
					if (ModNextMap.isNextMapActive)
					{
						ModNextMap.StopNextMap();
					}
					currentState = SetActivatorState.InteractingUron;
					stateTimer = now;
				}
				else if (!ModNextMap.isNextMapActive && !Char.isLoadingMap && !Char.ischangingMap)
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
				if (Char.isLoadingMap || Char.ischangingMap) break;

				Npc uron = FindUronNpc();
				if (uron != null)
				{
					int dist = Res.distance(me.cx, me.cy, uron.cx, uron.cy);
					if (dist > 30)
					{
						ModTeleport.TeleportTo(uron.cx, uron.cy);
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
					GameScr.info1.addInfo("Đã bán hết đồ rác!", 0);
					FinishSellingAndReturn();
				}
				break;

			case SetActivatorState.MovingToHomeBox:
				int homeMap = GetHomeMapId();
				if (TileMap.mapID == homeMap && !Char.isLoadingMap && !Char.ischangingMap)
				{
					if (ModNextMap.isNextMapActive)
					{
						ModNextMap.StopNextMap();
					}
					currentState = SetActivatorState.InteractingBox;
					stateTimer = now;
				}
				else if (!ModNextMap.isNextMapActive && !Char.isLoadingMap && !Char.ischangingMap)
				{
					if (now - stateTimer > 3000)
					{
						stateTimer = now;
						ModNextMap.StartNextMap(homeMap);
					}
				}
				break;

			case SetActivatorState.InteractingBox:
				if (Char.isLoadingMap || Char.ischangingMap) break;

				Npc boxNpc = FindHomeBoxNpc();
				if (boxNpc != null)
				{
					int distBox = Res.distance(me.cx, me.cy, boxNpc.cx, boxNpc.cy);
					if (distBox > 30)
					{
						ModTeleport.TeleportTo(boxNpc.cx, boxNpc.cy);
					}
					me.focusManualTo(boxNpc);

					if (now - stateTimer > 900)
					{
						stateTimer = now;
						int boxTmplId = (boxNpc.template != null) ? boxNpc.template.npcTemplateId : 3;
						Service.gI().openMenu(boxTmplId);

						currentState = SetActivatorState.StoringToBox;
						lastSellActionTime = now;
						storeAttemptCount = 0;
					}
				}
				else
				{
					if (now - stateTimer > 5000)
					{
						GameScr.info1.addInfo("Không tìm thấy Rương Đồ tại nhà! Quay lại bãi úp...", 0);
						ReturnToSavedFarm();
					}
				}
				break;

			case SetActivatorState.StoringToBox:
				if (now - lastSellActionTime < 350)
				{
					break;
				}

				int freeBox = GetFreeBoxSlots();
				if (freeBox == 0)
				{
					isBoxFullWarned = true;
					GameScr.info1.addInfo("Rương đồ ở nhà đã đầy! Quay lại bãi úp...", 0);
					ReturnToSavedFarm();
					break;
				}

				int storeIdx = -1;
				if (me.arrItemBag != null)
				{
					for (int i = 0; i < me.arrItemBag.Length; i++)
					{
						Item item = me.arrItemBag[i];
						if (item != null && IsValuableStoreItem(item))
						{
							storeIdx = i;
							break;
						}
					}
				}

				if (storeIdx != -1 && storeAttemptCount < 25)
				{
					lastSellActionTime = now;
					storeAttemptCount++;
					Service.gI().getItem(1, (sbyte)storeIdx);
				}
				else
				{
					GameScr.info1.addInfo("Đã cất đồ SKH/Đồ Sao vào rương! Quay lại bãi úp...", 0);
					ReturnToSavedFarm();
				}
				break;

			case SetActivatorState.ReturningToFarm:
				if (savedFarmMapId == -1)
				{
					currentState = SetActivatorState.Idle;
					break;
				}

				if (TileMap.mapID == savedFarmMapId && !Char.isLoadingMap && !Char.ischangingMap)
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
				else if (!ModNextMap.isNextMapActive && !Char.isLoadingMap && !Char.ischangingMap)
				{
					if (now - stateTimer > 3000)
					{
						stateTimer = now;
						ModNextMap.StartNextMap(savedFarmMapId);
					}
				}
				break;

			case SetActivatorState.SwitchingZone:
				if (Char.isLoadingMap || Char.ischangingMap) break;

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
				if (Char.isLoadingMap || Char.ischangingMap) break;

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

	/// <summary>
	/// Thực thi phương thức nghiệp vụ FinishSellingAndReturn.
	/// </summary>
	private static void FinishSellingAndReturn()
	{
		isSellingJunk = false;
		GameCanvas.endDlg();

		Char me = Char.myCharz();
		if (autoStoreToBox && !isBoxFullWarned && me != null && HasValuableStoreItemInBag(me))
		{
			StartGoStoreBoxNow();
			return;
		}

		ReturnToSavedFarm();
	}

	private static void ReturnToSavedFarm()
	{
		isSellingJunk = false;
		GameCanvas.endDlg();
		if (GameCanvas.panel != null && GameCanvas.panel.isShow)
		{
			GameCanvas.panel.hide();
		}
		currentState = SetActivatorState.ReturningToFarm;
		stateTimer = mSystem.currentTimeMillis();
		if (savedFarmMapId >= 0)
		{
			ModNextMap.StartNextMap(savedFarmMapId);
		}
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của GetStatusText.
	/// </summary>
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
			case SetActivatorState.MovingToHomeBox:
				return "Đang về Nhà mở Rương Đồ";
			case SetActivatorState.InteractingBox:
				return "Đang mở Rương Đồ tại nhà";
			case SetActivatorState.StoringToBox:
				return "Đang cất đồ SKH/Sao vào Rương";
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
