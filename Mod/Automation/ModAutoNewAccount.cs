using System;

/// <summary>
/// Module Auto Nhiệm Vụ Tân Thủ (Task 0 -> 10): Tự động làm chuỗi nhiệm vụ tân thủ từ Mộc Nhân đến Tàu Pảy Pảy theo chuẩn Mod Java.
/// </summary>
public static class ModAutoNewAccount
{
	public static bool isEnabled = false;
	private static long lastActionTime = 0;

	public static void Toggle()
	{
		isEnabled = !isEnabled;
		ModConfig.SaveConfig();
		GameScr.info1.addInfo("Auto NV Tân Thủ (Task 0-10): " + (isEnabled ? "BẬT" : "TẮT"), 0);
		SoundMn.gI().buttonClick();
	}

	private static int GetHomeMapId(int gender)
	{
		return 21 + gender;
	}

	private static int GetVillageMapId(int gender)
	{
		if (gender == 1) return 7;
		if (gender == 2) return 14;
		return 0;
	}

	private static int GetCliffMapId(int gender)
	{
		return 42 + gender;
	}

	private static int GetFirstWildMapId(int gender)
	{
		if (gender == 1) return 8;
		if (gender == 2) return 15;
		return 1;
	}

	private static int GetElderNpcTemplateId(int gender)
	{
		return gender;
	}

	private static Npc FindNpcInMap(int templateId)
	{
		if (GameScr.vNpc == null) return null;
		for (int i = 0; i < GameScr.vNpc.size(); i++)
		{
			Npc npc = (Npc)GameScr.vNpc.elementAt(i);
			if (npc != null && npc.template != null && npc.template.npcTemplateId == templateId)
			{
				return npc;
			}
		}
		return null;
	}

	private static void TalkToNpc(Char me, int npcTemplateId)
	{
		Npc npc = FindNpcInMap(npcTemplateId);
		if (npc != null)
		{
			if (Res.distance(me.cx, me.cy, npc.cx, npc.cy) > 36)
			{
				ModTeleport.TeleportTo(npc.cx, npc.cy - 3);
			}
			me.focusManualTo(npc);
		}
		if (GameCanvas.menu != null && GameCanvas.menu.showMenu)
		{
			Service.gI().confirmMenu((short)npcTemplateId, 0);
			GameCanvas.menu.showMenu = false;
		}
		else
		{
			Service.gI().openMenu(npcTemplateId);
		}
	}

	public static void Update()
	{
		if (!isEnabled || !ModMenu.IsInGame()) return;
		if (Char.isLoadingMap || Char.ischangingMap || ModNextMap.isNextMapActive) return;

		Char me = Char.myCharz();
		if (me == null || me.taskMaint == null) return;

		long now = mSystem.currentTimeMillis();
		if (now - lastActionTime < 650) return;
		lastActionTime = now;

		if (me.isDie || me.cHP <= 0)
		{
			Service.gI().returnTownFromDead();
			return;
		}

		// Tự động bỏ qua hội thoại NPC khi đang chạy nhiệm vụ
		if (ChatPopup.currChatPopup != null || ChatPopup.serverChatPopUp != null)
		{
			GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25] = true;
			return;
		}

		int taskId = me.taskMaint.taskId;
		int idx = me.taskMaint.index;
		int gender = me.cgender;
		int homeMap = GetHomeMapId(gender);
		int villageMap = GetVillageMapId(gender);
		int cliffMap = GetCliffMapId(gender);
		int wildMap = GetFirstWildMapId(gender);
		int elderNpc = GetElderNpcTemplateId(gender);

		if (taskId > 10)
		{
			isEnabled = false;
			ModTanSat.autoTanSat = false;
			ModConfig.SaveConfig();
			GameScr.info1.addInfo("Đã hoàn thành chuỗi Nhiệm Vụ Tân Thủ (Task 0 -> 10)!", 0);
			return;
		}

		// Task 0: Làm quen di chuyển & nhận nhiệm vụ Ông Gôhan / Moori / Paragus
		if (taskId == 0)
		{
			if (idx >= 0 && idx <= 1)
			{
				if (TileMap.mapID != homeMap)
				{
					ModNextMap.StartNextMap(homeMap);
					return;
				}
				TalkToNpc(me, elderNpc);
			}
			else if (idx >= 2 && idx <= 5)
			{
				if (TileMap.mapID == homeMap)
				{
					if (!ModAutoHeal.IsBeanFull() && GameScr.gI() != null && GameScr.gI().magicTree != null && GameScr.gI().magicTree.currPeas > 0)
					{
						Service.gI().openMenu(4);
						Service.gI().confirmMenu(4, 0);
						return;
					}
					if (GameScr.vItemMap != null && GameScr.vItemMap.size() > 0)
					{
						ItemMap itm = (ItemMap)GameScr.vItemMap.elementAt(0);
						if (itm != null)
						{
							Service.gI().pickItem(itm.itemMapID);
							return;
						}
					}
				}
				TalkToNpc(me, elderNpc);
			}
			return;
		}

		// Task 1: Đánh 5 Mộc Nhân tại Làng
		if (taskId == 1)
		{
			if (idx == 0)
			{
				if (TileMap.mapID != villageMap)
				{
					ModNextMap.StartNextMap(villageMap);
					return;
				}
				ModTanSat.autoTanSat = true;
			}
			else
			{
				ModTanSat.autoTanSat = false;
				if (TileMap.mapID != homeMap)
				{
					ModNextMap.StartNextMap(homeMap);
					return;
				}
				TalkToNpc(me, elderNpc);
			}
			return;
		}

		// Task 2: Đánh quái nhặt 10 Đùi Gà
		if (taskId == 2)
		{
			if (idx == 0)
			{
				if (TileMap.mapID != wildMap)
				{
					ModNextMap.StartNextMap(wildMap);
					return;
				}
				ModAutoPick.autoPick = true;
				ModTanSat.autoTanSat = true;
			}
			else
			{
				ModTanSat.autoTanSat = false;
				if (TileMap.mapID != homeMap)
				{
					ModNextMap.StartNextMap(homeMap);
					return;
				}
				TalkToNpc(me, elderNpc);
			}
			return;
		}

		// Task 3: Sao băng rơi - Bế đứa bé ở Vách Núi về Nhà
		if (taskId == 3)
		{
			if (idx == 0)
			{
				if (TileMap.mapID != cliffMap)
				{
					ModNextMap.StartNextMap(cliffMap);
					return;
				}
			}
			else if (idx == 1)
			{
				if (TileMap.mapID != cliffMap)
				{
					ModNextMap.StartNextMap(cliffMap);
					return;
				}
				if (GameScr.vItemMap != null && GameScr.vItemMap.size() > 0)
				{
					ItemMap baby = (ItemMap)GameScr.vItemMap.elementAt(0);
					if (baby != null)
					{
						ModTeleport.TeleportTo(baby.x, baby.y);
						Service.gI().pickItem(baby.itemMapID);
						return;
					}
				}
			}
			else
			{
				if (TileMap.mapID != homeMap)
				{
					ModNextMap.StartNextMap(homeMap);
					return;
				}
				TalkToNpc(me, elderNpc);
			}
			return;
		}

		// Task 4..10: Đánh quái theo hành tinh & trả nhiệm vụ Ông Gôhan / Moori / Paragus
		if (taskId >= 4 && taskId <= 10)
		{
			bool isCompletedStep = (me.taskMaint.subNames != null && idx >= me.taskMaint.subNames.Length - 1);
			if (isCompletedStep)
			{
				ModTanSat.autoTanSat = false;
				if (TileMap.mapID != homeMap)
				{
					ModNextMap.StartNextMap(homeMap);
					return;
				}
				TalkToNpc(me, elderNpc);
			}
			else
			{
				if (TileMap.mapID == homeMap || TileMap.mapID == villageMap)
				{
					ModNextMap.StartNextMap(wildMap);
					return;
				}
				ModAutoPick.autoPick = true;
				ModTanSat.autoTanSat = true;
			}
		}
	}
}
