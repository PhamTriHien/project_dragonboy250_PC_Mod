using System;

/// <summary>
/// Module Auto Mua Bùa: Tự động mua và gia hạn các loại bùa hỗ trợ tại NPC Bà Hạt Mít.
/// </summary>
public static class ModAutoBuyBua
{
	/// <summary>
	/// ENUM AutoBuyBuaState: Định nghĩa và điều phối logic trong hệ thống game.
	/// </summary>
	public enum AutoBuyBuaState
	{
		Idle,
		GoingToVachNui,
		ApproachingBaHatMit,
		WaitingMenu,
		WaitingShop,
		ReturningToFarm,
		SwitchingZone
	}

	// Cấu hình
	public static bool isAutoRebuy = false; // Tự động mua lại khi hết hạn
	public static int selectedBuaType = 0;   // 0..9: 10 loại bùa Bà Hạt Mít
	public static int selectedPackage = 0;   // 0: 1 Giờ, 1: 8 Giờ, 2: 1 Tháng
	public static bool showCharmHud = false;

	public static string[] buaTypeNames = new string[]
	{
		"Thu Hút",
		"Trí Tuệ",
		"Oai Hùng",
		"Mạnh Mẽ",
		"Da Trâu",
		"Dẻo Dai",
		"Bất Tử",
		"Đệ Tử",
		"Trí Tuệ x3",
		"Trí Tuệ x4"
	};

	public static string[] buaKeywords = new string[]
	{
		"thu hut",
		"tri tue",
		"oai hung",
		"manh me",
		"da trau",
		"deo dai",
		"bat tu",
		"de tu",
		"tri tue x3",
		"tri tue x4"
	};

	public static int[] buaTemplateIds = new int[]
	{
		219,
		213,
		216,
		214,
		215,
		218,
		217,
		522,
		671,
		672
	};

	public static string[] packageNames = new string[]
	{
		"1 Giờ",
		"8 Giờ",
		"1 Tháng"
	};

	// Trạng thái vận hành
	public static AutoBuyBuaState currentState = AutoBuyBuaState.Idle;
	public static bool isBusy => currentState != AutoBuyBuaState.Idle;

	public static int savedFarmMapId = -1;
	public static int savedFarmZoneId = -1;
	public static int savedFarmX = -1;
	public static int savedFarmY = -1;

	private static long stateTimer = 0;
	private static long lastAutoCheckTime = 0;
	private static long lastBuySuccessTime = 0;
	private static int retryOpenMenuCount = 0;
	private static int retryShopCount = 0;
	private static long lastZoneAttemptTime = 0;
	private static int zoneAttemptCount = 0;

	/// <summary>
	/// Truy xuất và trả về dữ liệu của GetHomeVachNuiMapId.
	/// </summary>
	public static int GetHomeVachNuiMapId()
	{
		Char me = Char.myCharz();
		if (me == null) return 42;
		if (me.cgender == 1) return 43; // Vách Moori (Namếc)
		if (me.cgender == 2) return 44; // Vách Kakarot (Xayda)
		return 42;                      // Vách Aru (Trái Đất)
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ FindBaHatMit.
	/// </summary>
	public static Npc FindBaHatMit()
	{
		if (GameScr.vNpc == null) return null;
		for (int i = 0; i < GameScr.vNpc.size(); i++)
		{
			Npc npc = (Npc)GameScr.vNpc.elementAt(i);
			if (npc != null && npc.template != null)
			{
				string name = ModNextMapData.RemoveAccents(npc.template.name).ToLower();
				if (name.Contains("hat mit") || name.Contains("ba hat mit"))
				{
					return npc;
				}
			}
		}
		// Fallback nếu map 42, 43, 44 chỉ có 1 NPC
		if (GameScr.vNpc.size() > 0)
		{
			return (Npc)GameScr.vNpc.elementAt(0);
		}
		return null;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ StartBuyBuaNow.
	/// </summary>
	public static void StartBuyBuaNow()
	{
		Char me = Char.myCharz();
		if (me == null || me.cHP <= 0) return;

		// Lưu lại vị trí bãi farm hiện tại
		savedFarmMapId = TileMap.mapID;
		savedFarmZoneId = TileMap.zoneID;
		savedFarmX = me.cx;
		savedFarmY = me.cy;

		int targetMap = GetHomeVachNuiMapId();
		currentState = AutoBuyBuaState.GoingToVachNui;
		stateTimer = mSystem.currentTimeMillis();
		retryOpenMenuCount = 0;
		retryShopCount = 0;

		GameScr.info1.addInfo("Bắt đầu đi mua " + buaTypeNames[selectedBuaType] + " tại Vách Núi...", 0);
		SoundMn.gI().buttonClick();

		if (TileMap.mapID != targetMap)
		{
			ModNextMap.StartNextMap(targetMap);
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ StopAutoBuyBua.
	/// </summary>
	public static void StopAutoBuyBua()
	{
		currentState = AutoBuyBuaState.Idle;
		if (GameCanvas.panel.isShow)
		{
			GameCanvas.panel.hideNow();
		}
		GameCanvas.menu.showMenu = false;
	}

	/// <summary>
	/// Bật hoặc tắt trạng thái kích hoạt của ToggleAutoRebuy.
	/// </summary>
	public static void ToggleAutoRebuy()
	{
		isAutoRebuy = !isAutoRebuy;
		ModConfig.SaveConfig();
		GameScr.info1.addInfo("Tự động mua lại bùa: " + (isAutoRebuy ? "BẬT" : "TẮT"), 0);
		SoundMn.gI().buttonClick();
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ CycleBuaType.
	/// </summary>
	public static void CycleBuaType()
	{
		selectedBuaType = (selectedBuaType + 1) % buaTypeNames.Length;
		ModConfig.SaveConfig();
		GameScr.info1.addInfo("Chọn mua: " + buaTypeNames[selectedBuaType], 0);
		SoundMn.gI().buttonClick();
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ CyclePackage.
	/// </summary>
	public static void CyclePackage()
	{
		selectedPackage = (selectedPackage + 1) % packageNames.Length;
		ModConfig.SaveConfig();
		GameScr.info1.addInfo("Gói bùa: " + packageNames[selectedPackage], 0);
		SoundMn.gI().buttonClick();
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của GetStatusText.
	/// </summary>
	public static string GetStatusText()
	{
		switch (currentState)
		{
			case AutoBuyBuaState.GoingToVachNui:
				return "Đang đến Vách núi " + ModNextMap.GetMapName(GetHomeVachNuiMapId());
			case AutoBuyBuaState.ApproachingBaHatMit:
				return "Đang tiếp cận Bà Hạt Mít...";
			case AutoBuyBuaState.WaitingMenu:
				return "Đang mở menu bùa...";
			case AutoBuyBuaState.WaitingShop:
				return "Đang mua bùa trong cửa hàng...";
			case AutoBuyBuaState.ReturningToFarm:
			case AutoBuyBuaState.SwitchingZone:
				return "Đang quay lại bãi farm...";
			default:
				return isAutoRebuy ? "Sẵn sàng (Tự mua khi hết)" : "Sẵn sàng";
		}
	}

	/// <summary>
	/// Cập nhật trạng thái logic của đối tượng theo từng chu kỳ khung hình.
	/// </summary>
	public static void Update()
	{
		if (!ModMenu.IsInGame()) return;

		Char me = Char.myCharz();
		if (me == null || me.cHP <= 0) return;

		long now = mSystem.currentTimeMillis();

		// Kiểm tra tự động mua lại khi hết hạn bùa
		if (isAutoRebuy && currentState == AutoBuyBuaState.Idle)
		{
			if (now - lastAutoCheckTime > 30000) // Kiểm tra mỗi 30 giây
			{
				lastAutoCheckTime = now;
				long durationMs = (selectedPackage == 0) ? 3600000L : ((selectedPackage == 1) ? 28800000L : 2592000000L);
				if (lastBuySuccessTime == 0 || now - lastBuySuccessTime > durationMs)
				{
					StartBuyBuaNow();
					return;
				}
			}
		}

		if (currentState == AutoBuyBuaState.Idle) return;

		// Watchdog chống kẹt tổng thể (quá 60 giây tự động hủy về bãi)
		if (now - stateTimer > 60000)
		{
			GameScr.info1.addInfo("Quá thời gian mua bùa! Quay lại bãi farm...", 0);
			ReturnToFarmSpot();
			return;
		}

		int vachMapId = GetHomeVachNuiMapId();

		switch (currentState)
		{
			case AutoBuyBuaState.GoingToVachNui:
				if (TileMap.mapID == vachMapId && !Char.isLoadingMap && !Char.ischangingMap)
				{
					currentState = AutoBuyBuaState.ApproachingBaHatMit;
					stateTimer = now;
				}
				else if (!ModNextMap.isNextMapActive && !Char.isLoadingMap && !Char.ischangingMap)
				{
					ModNextMap.StartNextMap(vachMapId);
				}
				break;

			case AutoBuyBuaState.ApproachingBaHatMit:
				if (Char.isLoadingMap || Char.ischangingMap) break;

				Npc baHatMit = FindBaHatMit();
				if (baHatMit != null)
				{
					int dist = Res.distance(me.cx, me.cy, baHatMit.cx, baHatMit.cy);
					if (dist > 30)
					{
						ModTeleport.TeleportTo(baHatMit.cx, baHatMit.cy);
					}
					me.focusManualTo(baHatMit);

					if (now - stateTimer > 800)
					{
						stateTimer = now;
						int tmplId = (baHatMit.template != null) ? baHatMit.template.npcTemplateId : baHatMit.npcId;
						Service.gI().openMenu(tmplId);
						currentState = AutoBuyBuaState.WaitingMenu;
						retryOpenMenuCount = 0;
					}
				}
				else if (now - stateTimer > 5000)
				{
					GameScr.info1.addInfo("Không tìm thấy Bà Hạt Mít! Quay lại bãi farm...", 0);
					ReturnToFarmSpot();
				}
				break;

			case AutoBuyBuaState.WaitingMenu:
				if (GameCanvas.menu.showMenu && GameCanvas.menu.menuItems != null && GameCanvas.menu.menuItems.size() > 0)
				{
					Npc npc = FindBaHatMit();
					int tmplId = (npc != null && npc.template != null) ? npc.template.npcTemplateId : 21;

					// Tìm menu tương ứng với gói thời hạn đã chọn
					int targetIndex = -1;
					string searchPkg = (selectedPackage == 0) ? "1 gio" : ((selectedPackage == 1) ? "8 gio" : "1 thang");

					for (int i = 0; i < GameCanvas.menu.menuItems.size(); i++)
					{
						Command cmd = (Command)GameCanvas.menu.menuItems.elementAt(i);
						if (cmd != null && !string.IsNullOrEmpty(cmd.caption))
						{
							string cap = ModNextMapData.RemoveAccents(cmd.caption).ToLower();
							if (cap.Contains(searchPkg))
							{
								targetIndex = i;
								break;
							}
						}
					}

					// Nếu chưa ở menu chọn gói mà đang ở menu cấp 1 ("Cửa hàng Bùa"), bấm vào "Cửa hàng Bùa" và tiếp tục chờ menu cấp 2
					if (targetIndex == -1)
					{
						int shopMenuIdx = -1;
						for (int j = 0; j < GameCanvas.menu.menuItems.size(); j++)
						{
							Command cmd2 = (Command)GameCanvas.menu.menuItems.elementAt(j);
							if (cmd2 != null && !string.IsNullOrEmpty(cmd2.caption))
							{
								string cap2 = ModNextMapData.RemoveAccents(cmd2.caption).ToLower();
								if (cap2.Contains("cua hang bua") || cap2.Contains("bua"))
								{
									shopMenuIdx = j;
									break;
								}
							}
						}
						if (shopMenuIdx != -1 && retryOpenMenuCount == 0)
						{
							retryOpenMenuCount = 1;
							Service.gI().confirmMenu((short)tmplId, (sbyte)shopMenuIdx);
							GameCanvas.menu.showMenu = false;
							stateTimer = now;
							break;
						}
						targetIndex = (shopMenuIdx != -1) ? shopMenuIdx : 0;
					}

					Service.gI().confirmMenu((short)tmplId, (sbyte)targetIndex);
					GameCanvas.menu.showMenu = false;
					currentState = AutoBuyBuaState.WaitingShop;
					stateTimer = now;
					retryShopCount = 0;
				}
				else if (now - stateTimer > 2500)
				{
					retryOpenMenuCount++;
					if (retryOpenMenuCount >= 3)
					{
						GameScr.info1.addInfo("Không mở được menu Bà Hạt Mít! Quay lại bãi...", 0);
						ReturnToFarmSpot();
					}
					else
					{
						Npc npc2 = FindBaHatMit();
						if (npc2 != null)
						{
							int tmplId2 = (npc2.template != null) ? npc2.template.npcTemplateId : npc2.npcId;
							Service.gI().openMenu(tmplId2);
						}
						stateTimer = now;
					}
				}
				break;

			case AutoBuyBuaState.WaitingShop:
				if (now - stateTimer < 600) break;

				Item buaItem = FindTargetBuaInShop();
				if (buaItem != null && buaItem.template != null)
				{
					// Gửi lệnh mua bùa bằng ngọc
					Service.gI().buyItem(1, buaItem.template.id, 0);
					lastBuySuccessTime = now;
					GameScr.info1.addInfo("ĐÃ MUA: " + buaItem.template.name + " (" + packageNames[selectedPackage] + ")!", 0);

					if (GameCanvas.panel.isShow)
					{
						GameCanvas.panel.hideNow();
					}

					ReturnToFarmSpot();
				}
				else if (now - stateTimer > 4000)
				{
					retryShopCount++;
					if (retryShopCount >= 2)
					{
						GameScr.info1.addInfo("Không tìm thấy " + buaTypeNames[selectedBuaType] + " trong cửa hàng! Quay lại bãi...", 0);
						ReturnToFarmSpot();
					}
					else
					{
						stateTimer = now;
					}
				}
				break;

			case AutoBuyBuaState.ReturningToFarm:
				if (savedFarmMapId >= 0)
				{
					if (TileMap.mapID == savedFarmMapId && !Char.isLoadingMap && !Char.ischangingMap)
					{
						if (ModNextMap.isNextMapActive)
						{
							ModNextMap.StopNextMap();
						}
						if (savedFarmZoneId >= 0 && TileMap.zoneID != savedFarmZoneId)
						{
							currentState = AutoBuyBuaState.SwitchingZone;
							stateTimer = now;
							lastZoneAttemptTime = now;
							zoneAttemptCount = 1;
							Service.gI().requestChangeZone(savedFarmZoneId, -1);
						}
						else
						{
							if (savedFarmX > 0 && savedFarmY > 0)
							{
								ModTeleport.TeleportTo(savedFarmX, savedFarmY);
							}
							currentState = AutoBuyBuaState.Idle;
							GameScr.info1.addInfo("Đã quay về bãi farm an toàn!", 0);
						}
					}
					else if (!ModNextMap.isNextMapActive && !Char.isLoadingMap && !Char.ischangingMap)
					{
						ModNextMap.StartNextMap(savedFarmMapId);
					}
				}
				else
				{
					currentState = AutoBuyBuaState.Idle;
				}
				break;

			case AutoBuyBuaState.SwitchingZone:
				if (Char.isLoadingMap || Char.ischangingMap) break;

				if (savedFarmZoneId < 0 || TileMap.zoneID == savedFarmZoneId || zoneAttemptCount >= 3)
				{
					if (savedFarmX > 0 && savedFarmY > 0)
					{
						ModTeleport.TeleportTo(savedFarmX, savedFarmY);
					}
					currentState = AutoBuyBuaState.Idle;
					GameScr.info1.addInfo("Đã quay về bãi farm an toàn!", 0);
					break;
				}

				if (now - lastZoneAttemptTime > 2500)
				{
					lastZoneAttemptTime = now;
					zoneAttemptCount++;
					Service.gI().requestChangeZone(savedFarmZoneId, -1);
				}
				break;
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ FindTargetBuaInShop.
	/// </summary>
	private static Item FindTargetBuaInShop()
	{
		Char me = Char.myCharz();
		if (me == null || me.arrItemShop == null) return null;

		int targetId = (selectedBuaType >= 0 && selectedBuaType < buaTemplateIds.Length) ? buaTemplateIds[selectedBuaType] : -1;
		string kw = (selectedBuaType >= 0 && selectedBuaType < buaKeywords.Length) ? buaKeywords[selectedBuaType] : string.Empty;

		for (int tab = 0; tab < me.arrItemShop.Length; tab++)
		{
			if (me.arrItemShop[tab] == null) continue;
			for (int i = 0; i < me.arrItemShop[tab].Length; i++)
			{
				Item it = me.arrItemShop[tab][i];
				if (it != null && it.template != null && it.template.id == targetId)
				{
					return it;
				}
			}
		}

		for (int tab = 0; tab < me.arrItemShop.Length; tab++)
		{
			if (me.arrItemShop[tab] == null) continue;
			for (int i = 0; i < me.arrItemShop[tab].Length; i++)
			{
				Item it = me.arrItemShop[tab][i];
				if (it != null && it.template != null && !string.IsNullOrEmpty(it.template.name))
				{
					string name = ModNextMapData.RemoveAccents(it.template.name).ToLower();
					if (!string.IsNullOrEmpty(kw) && name.Contains(kw))
					{
						return it;
					}
				}
			}
		}
		return null;
	}

	/// <summary>
	/// Vẽ HUD thời gian bùa đang kích hoạt lên màn hình.
	/// </summary>
	public static void PaintCharmHUD(mGraphics g)
	{
		if (!showCharmHud || !ModMenu.IsInGame() || GameCanvas.panel.isShow || ModUI.uiCustomOpen) return;
		try
		{
			if (lastBuySuccessTime <= 0) return;
			long durationMs = (selectedPackage == 0) ? 3600000L : ((selectedPackage == 1) ? 28800000L : 2592000000L);
			long remainMs = durationMs - (mSystem.currentTimeMillis() - lastBuySuccessTime);
			if (remainMs <= 0) return;
			long mins = remainMs / 60000L;
			string text = "Bùa " + buaTypeNames[selectedBuaType] + ": còn " + mins + "p";
			mFont.tahoma_7_yellow.drawString(g, text, 6, 138, mFont.LEFT, mFont.tahoma_7_grey);
		}
		catch
		{
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ ReturnToFarmSpot.
	/// </summary>
	private static void ReturnToFarmSpot()
	{
		if (GameCanvas.panel.isShow)
		{
			GameCanvas.panel.hideNow();
		}
		GameCanvas.menu.showMenu = false;

		if (savedFarmMapId >= 0)
		{
			currentState = AutoBuyBuaState.ReturningToFarm;
			stateTimer = mSystem.currentTimeMillis();
			ModNextMap.StartNextMap(savedFarmMapId);
		}
		else
		{
			currentState = AutoBuyBuaState.Idle;
		}
	}
}
