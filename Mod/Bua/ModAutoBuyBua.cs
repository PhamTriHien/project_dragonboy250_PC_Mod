using System;

public static class ModAutoBuyBua
{
	public enum AutoBuyBuaState
	{
		Idle,
		GoingToVachNui,
		ApproachingBaHatMit,
		WaitingMenu,
		WaitingShop,
		ReturningToFarm
	}

	// Cấu hình
	public static bool isAutoRebuy = false; // Tự động mua lại khi hết hạn
	public static int selectedBuaType = 0;   // 0: Thu Hút, 1: Trí Tuệ, 2: Oai Hùng, 3: Mạnh Mẽ, 4: Da Trâu, 5: Dẻo Dai
	public static int selectedPackage = 0;   // 0: 1 Giờ, 1: 8 Giờ, 2: 1 Tháng

	public static string[] buaTypeNames = new string[]
	{
		"Thu Hút",
		"Trí Tuệ",
		"Oai Hùng",
		"Mạnh Mẽ",
		"Da Trâu",
		"Dẻo Dai"
	};

	public static string[] buaKeywords = new string[]
	{
		"thu hut",
		"tri tue",
		"oai hung",
		"manh me",
		"da trau",
		"deo dai"
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

	public static int GetHomeVachNuiMapId()
	{
		Char me = Char.myCharz();
		if (me == null) return 42;
		if (me.cgender == 1) return 43; // Vách Moori (Namếc)
		if (me.cgender == 2) return 44; // Vách Kakarot (Xayda)
		return 42;                      // Vách Aru (Trái Đất)
	}

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

	public static void StopAutoBuyBua()
	{
		currentState = AutoBuyBuaState.Idle;
		if (GameCanvas.panel.isShow)
		{
			GameCanvas.panel.hideNow();
		}
		GameCanvas.menu.showMenu = false;
	}

	public static void ToggleAutoRebuy()
	{
		isAutoRebuy = !isAutoRebuy;
		ModConfig.SaveConfig();
		GameScr.info1.addInfo("Tự động mua lại bùa: " + (isAutoRebuy ? "BẬT" : "TẮT"), 0);
		SoundMn.gI().buttonClick();
	}

	public static void CycleBuaType()
	{
		selectedBuaType = (selectedBuaType + 1) % buaTypeNames.Length;
		ModConfig.SaveConfig();
		GameScr.info1.addInfo("Chọn mua: " + buaTypeNames[selectedBuaType], 0);
		SoundMn.gI().buttonClick();
	}

	public static void CyclePackage()
	{
		selectedPackage = (selectedPackage + 1) % packageNames.Length;
		ModConfig.SaveConfig();
		GameScr.info1.addInfo("Gói bùa: " + packageNames[selectedPackage], 0);
		SoundMn.gI().buttonClick();
	}

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
				return "Đang quay lại bãi farm...";
			default:
				return isAutoRebuy ? "Sẵn sàng (Tự mua khi hết)" : "Sẵn sàng";
		}
	}

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
				// Nếu đã từng mua thành công và đã hết thời hạn gói (hoặc không còn bùa)
				long durationMs = (selectedPackage == 0) ? 3600000L : ((selectedPackage == 1) ? 28800000L : 2592000000L);
				if (lastBuySuccessTime > 0 && now - lastBuySuccessTime > durationMs)
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
				else if (!ModNextMap.isNextMapActive)
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
						me.cx = baHatMit.cx;
						me.cy = baHatMit.cy;
						Service.gI().charMoveTo(baHatMit.cx, baHatMit.cy);
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

					// Fallback nếu không thấy đúng tên gói: tìm mục có chữ "bua"
					if (targetIndex == -1)
					{
						for (int j = 0; j < GameCanvas.menu.menuItems.size(); j++)
						{
							Command cmd2 = (Command)GameCanvas.menu.menuItems.elementAt(j);
							if (cmd2 != null && !string.IsNullOrEmpty(cmd2.caption))
							{
								string cap2 = ModNextMapData.RemoveAccents(cmd2.caption).ToLower();
								if (cap2.Contains("bua"))
								{
									targetIndex = j;
									break;
								}
							}
						}
					}

					if (targetIndex == -1) targetIndex = 0;

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
				if (savedFarmMapId > 0)
				{
					if (TileMap.mapID == savedFarmMapId && !Char.isLoadingMap && !Char.ischangingMap)
					{
						if (savedFarmZoneId >= 0 && TileMap.zoneID != savedFarmZoneId)
						{
							Service.gI().requestChangeZone(savedFarmZoneId, -1);
						}
						if (savedFarmX > 0 && savedFarmY > 0)
						{
							ModTeleport.TeleportTo(savedFarmX, savedFarmY);
						}
						currentState = AutoBuyBuaState.Idle;
						GameScr.info1.addInfo("Đã quay về bãi farm an toàn!", 0);
					}
					else if (!ModNextMap.isNextMapActive)
					{
						ModNextMap.StartNextMap(savedFarmMapId);
					}
				}
				else
				{
					currentState = AutoBuyBuaState.Idle;
				}
				break;
		}
	}

	private static Item FindTargetBuaInShop()
	{
		Char me = Char.myCharz();
		if (me == null || me.arrItemShop == null) return null;

		string kw = buaKeywords[selectedBuaType];

		for (int tab = 0; tab < me.arrItemShop.Length; tab++)
		{
			if (me.arrItemShop[tab] == null) continue;
			for (int i = 0; i < me.arrItemShop[tab].Length; i++)
			{
				Item it = me.arrItemShop[tab][i];
				if (it != null && it.template != null && !string.IsNullOrEmpty(it.template.name))
				{
					string name = ModNextMapData.RemoveAccents(it.template.name).ToLower();
					if (name.Contains(kw))
					{
						return it;
					}
				}
			}
		}
		return null;
	}

	private static void ReturnToFarmSpot()
	{
		if (GameCanvas.panel.isShow)
		{
			GameCanvas.panel.hideNow();
		}
		GameCanvas.menu.showMenu = false;

		if (savedFarmMapId > 0)
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
