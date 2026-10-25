using System;

/// <summary>
/// Module Auto Nhiệm Vụ Tân Thủ (Task 0 -> 10): Tự động hoàn thiện chuỗi nhiệm vụ từ Mộc Nhân đến Thần Mèo Karin & Tàu Pảy Pảy.
/// </summary>
public static class ModAutoNewAccount
{
	public static bool isEnabled = false;
	private static long lastActionTime = 0;
	private static long lastSupportCheckTime = 0;
	private static long lastStatUpTime = 0;
	private static long lastDuelAttackTime = 0;
	private static long lastBeanTime = 0;
	private static bool hasClaimedBaHatMitCharm = false;
	private static bool movedToTaoPaiPaiSpot = false;

	private static readonly int[] StarterSkillBookIds = new int[] { 227, 228, 229 };

	public static void Toggle()
	{
		isEnabled = !isEnabled;
		if (!isEnabled)
		{
			ModTanSatFilter.selectAllMobs = true;
			ModTanSatFilter.tickedMobTemplateIds.Clear();
			ModTanSatFilter.selectedMobTemplateId = -1;
		}
		ModConfig.SaveConfig();
		GameScr.info1?.addInfo("Auto NV Tân Thủ (Task 0-10): " + (isEnabled ? "BẬT" : "TẮT"), 0);
		SoundMn.gI().buttonClick();
	}

	private static int GetHomeMapId(int gender)
	{
		return 21 + gender;
	}

	private static int GetVillageMapId(int gender)
	{
		return gender * 7;
	}

	private static int GetCliffMapId(int gender)
	{
		return 42 + gender;
	}

	private static int GetFirstWildMapId(int gender)
	{
		return gender * 7 + 1;
	}

	private static int GetElderNpcTemplateId(int gender)
	{
		if (gender == 1) return 2;
		if (gender == 2) return 1;
		return 0;
	}

	private static int GetPowerTrainMapId(int gender)
	{
		if (gender == 1) return 9;
		if (gender == 2) return 17;
		return 3;
	}

	private static int GetTask4To6MapId(int gender, int subIndex)
	{
		if (subIndex == 0) return gender * 7 + 2;
		if (subIndex == 1) return (gender == 0) ? 9 : 2;
		return (gender == 2) ? 9 : 16;
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

	private static void TalkToNpc(Char me, int npcTemplateId, sbyte menuConfirmIndex = 0)
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
			Service.gI().confirmMenu((short)npcTemplateId, menuConfirmIndex);
			GameCanvas.menu.showMenu = false;
		}
		else
		{
			Service.gI().openMenu(npcTemplateId);
		}
	}

	private static void ResetMobFilterToAll()
	{
		if (!ModTanSatFilter.selectAllMobs)
		{
			ModTanSatFilter.selectAllMobs = true;
			ModTanSatFilter.tickedMobTemplateIds.Clear();
			ModTanSatFilter.selectedMobTemplateId = -1;
		}
	}

	private static void SelectMobsByMaxHpRange(long minMaxHp, long maxMaxHp)
	{
		if (GameScr.vMob == null || GameScr.vMob.size() == 0)
		{
			ResetMobFilterToAll();
			return;
		}
		ModTanSatFilter.tickedMobTemplateIds.Clear();
		for (int i = 0; i < GameScr.vMob.size(); i++)
		{
			Mob m = (Mob)GameScr.vMob.elementAt(i);
			if (m != null && m.maxHp >= minMaxHp && m.maxHp <= maxMaxHp && !ModTanSatFilter.tickedMobTemplateIds.Contains(m.templateId))
			{
				ModTanSatFilter.tickedMobTemplateIds.Add(m.templateId);
			}
		}
		if (ModTanSatFilter.tickedMobTemplateIds.Count > 0)
		{
			ModTanSatFilter.selectAllMobs = false;
			ModTanSatFilter.selectedMobTemplateId = -1;
		}
		else
		{
			ResetMobFilterToAll();
		}
	}

	private static int CountPeasInBag(Char me)
	{
		if (me == null || me.arrItemBag == null) return 0;
		int total = 0;
		for (int i = 0; i < me.arrItemBag.Length; i++)
		{
			Item it = me.arrItemBag[i];
			if (it != null && it.template != null && it.template.type == 6)
			{
				total += it.quantity;
			}
		}
		return total;
	}

	private static void RunNewcomerSupport(Char me, int taskId, int homeMap, long now)
	{
		// 1. Tự ăn đậu khi máu/KI thấp (trong Task 10 đánh Karin / Tàu Pảy Pảy giữ ngưỡng cao 65%-80%)
		int hpThresholdPct = (taskId == 10) ? 70 : 30;
		int mpThresholdPct = (taskId == 10) ? 35 : 20;
		bool needBean = (me.cHPFull > 0 && me.cHP * 100L <= me.cHPFull * hpThresholdPct) ||
		                (me.cMPFull > 0 && me.cMP * 100L <= me.cMPFull * mpThresholdPct) ||
		                me.cHP < 15 || me.cMP < 15;
		if (needBean && now - lastBeanTime >= 1500 && CountPeasInBag(me) > 0)
		{
			lastBeanTime = now;
			GameScr.gI().doUseHP();
		}

		// 2. Tự động cộng chỉ số cơ bản từ Task 4..10 để đủ sức đánh Karin & Tàu Pảy Pảy
		if (taskId >= 4 && taskId <= 10 && me.cTiemNang > 0 && now - lastStatUpTime >= 900)
		{
			lastStatUpTime = now;
			if ((me.cHPGoc < 400 || (me.cDamGoc >= 40 && me.cHPGoc < 500)) && me.cTiemNang > me.cHPGoc + 1000L)
			{
				Service.gI().upPotential(0, 1);
			}
			else if (me.cMPGoc < 300 && me.cDamGoc >= 25 && me.cTiemNang > me.cMPGoc + 1000L)
			{
				Service.gI().upPotential(1, 1);
			}
			else if (me.cDamGoc < 70 && me.cTiemNang > me.cDamGoc * 100L)
			{
				Service.gI().upPotential(2, 1);
			}
		}

		if (now - lastSupportCheckTime < 2200) return;
		lastSupportCheckTime = now;

		// 3. Lấy sách võ công tân thủ từ rương ra hành trang và tự học sách / cưỡi thú (396)
		if (me.arrItemBox != null)
		{
			for (int i = me.arrItemBox.Length - 1; i >= 0; i--)
			{
				Item boxIt = me.arrItemBox[i];
				if (boxIt == null || boxIt.template == null) continue;
				bool isStarterBook = (boxIt.template.type == 12);
				for (int b = 0; b < StarterSkillBookIds.Length; b++)
				{
					if (boxIt.template.id == StarterSkillBookIds[b]) isStarterBook = true;
				}
				if (isStarterBook)
				{
					Service.gI().getItem(0, (sbyte)i);
					break;
				}
			}
		}

		if (me.arrItemBag != null)
		{
			for (int i = me.arrItemBag.Length - 1; i >= 0; i--)
			{
				Item bagIt = me.arrItemBag[i];
				if (bagIt == null || bagIt.template == null) continue;
				if (bagIt.template.type == 12 || bagIt.template.id == 396)
				{
					Service.gI().useItem(0, 1, (sbyte)bagIt.indexUI, -1);
					break;
				}
			}
		}

		// 4. Khi đang ở nhà: tự thu hoạch đậu, nhặt đùi gà (74), nâng cấp Cây Đậu Thần cấp 1 -> 2 khi đủ 5000 vàng
		if (TileMap.mapID == homeMap)
		{
			ModGoBack.TryPickHomeChicken(me);
			MagicTree tree = (GameScr.gI() != null) ? GameScr.gI().magicTree : null;
			if (tree != null && !tree.isUpdate)
			{
				int targetBeans = (me.cgender == 1) ? 30 : 20;
				if (tree.currPeas > 0 && CountPeasInBag(me) < targetBeans && taskId >= 2)
				{
					Service.gI().openMenu(4);
					Service.gI().confirmMenu(4, 0);
				}
				else if (me.xu >= 5000 && tree.maxPeas == 5)
				{
					Service.gI().openMenu(4);
					Service.gI().confirmMenu(4, 1);
					Service.gI().confirmMenu(5, 0);
				}
			}
		}
	}

	private static Char FindHostileCharInMap(Char me)
	{
		if (GameScr.vCharInMap == null || me == null) return null;
		for (int i = 0; i < GameScr.vCharInMap.size(); i++)
		{
			Char c = (Char)GameScr.vCharInMap.elementAt(i);
			if (c != null && c.charID != me.charID && c.cHP > 0 && !c.isDie && c.statusMe != 14 && (c.cTypePk == 3 || c.cTypePk == 5))
			{
				return c;
			}
		}
		return null;
	}

	private static void AttackHostileQuestChar(Char me, Char target, long now)
	{
		if (me == null || target == null) return;
		me.charFocus = target;
		me.mobFocus = null;
		me.npcFocus = null;
		if (Res.distance(me.cx, me.cy, target.cx, target.cy) > 44)
		{
			ModTeleport.TeleportTo(target.cx, target.cy);
		}
		Skill sk = ModTanSatFilter.GetBestSkillToUse();
		if (sk == null) sk = me.myskill;
		if (sk == null || sk.template == null) return;
		int cd = (sk.coolDown > 250) ? sk.coolDown : 250;
		if (now - lastDuelAttackTime < cd) return;
		lastDuelAttackTime = now;
		me.myskill = sk;
		Service.gI().selectSkill(sk.template.id);
		MyVector vMob = new MyVector();
		MyVector vChar = new MyVector();
		vChar.addElement(target);
		Service.gI().sendPlayerAttack(vMob, vChar, -1);
		sk.lastTimeUseThisSkill = now;
	}

	private static void ClimbKarinTowerFrom47(Char me)
	{
		if (Res.abs(me.cx - 600) >= 20)
		{
			ModTeleport.TeleportTo(600, 336);
		}
		else
		{
			me.currentMovePoint = new MovePoint(600, 10);
		}
	}

	public static void Update()
	{
		if (!isEnabled || !ModMenu.IsInGame()) return;
		if (Char.isLoadingMap || Char.ischangingMap || ModNextMap.isNextMapActive || ModAutoCaptcha.IsCaptchaActive()) return;

		Char me = Char.myCharz();
		if (me == null || me.taskMaint == null) return;

		long now = mSystem.currentTimeMillis();
		if (now - lastActionTime < 550) return;
		lastActionTime = now;

		if (me.isDie || me.cHP <= 0)
		{
			ModTanSat.autoTanSat = false;
			Service.gI().returnTownFromDead();
			return;
		}

		// Bỏ qua hội thoại NPC tự động
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
			ResetMobFilterToAll();
			ModConfig.SaveConfig();
			GameScr.info1?.addInfo("Đã hoàn thành chuỗi Nhiệm Vụ Tân Thủ (Task 0 -> 10)!", 0);
			return;
		}

		RunNewcomerSupport(me, taskId, homeMap, now);

		// Nếu cạn sạch cả Đậu lẫn HP/KI (không phải ở nhà) thì về nhà lấy Đậu / Đùi gà trước khi đánh tiếp
		if (CountPeasInBag(me) == 0 && (me.cHP < 15 || me.cMP < 15) && TileMap.mapID != homeMap)
		{
			ModTanSat.autoTanSat = false;
			ModNextMap.StartNextMap(homeMap);
			return;
		}

		switch (taskId)
		{
			case 0:
			{
				if (TileMap.mapID >= 39 && TileMap.mapID <= 41 && TileMap.vGo != null && TileMap.vGo.size() > 0)
				{
					Waypoint wp = (Waypoint)TileMap.vGo.elementAt(0);
					if (wp != null)
					{
						ModTeleport.TeleportTo(wp.minX - 20, wp.minY);
						if (wp.isOffline) Service.gI().getMapOffline();
						else Service.gI().requestChangeMap();
					}
					return;
				}
				if (TileMap.mapID != homeMap)
				{
					ModNextMap.StartNextMap(homeMap);
					return;
				}
				if (idx == 3)
				{
					int chestX = (gender == 0) ? 85 : ((gender == 1) ? 638 : 94);
					if (Res.abs(me.cx - chestX) > 12 || Res.abs(me.cy - 336) > 12)
					{
						ModTeleport.TeleportTo(chestX, 336);
					}
					else
					{
						Npc chestNpc = FindNpcInMap(3);
						if (chestNpc != null) Service.gI().openMenu(3);
						Service.gI().getItem(0, 0);
					}
				}
				else if (idx == 4)
				{
					Service.gI().openMenu(4);
					Service.gI().confirmMenu(4, 0);
				}
				else
				{
					if (GameCanvas.menu != null && GameCanvas.menu.showMenu) GameCanvas.menu.showMenu = false;
					TalkToNpc(me, elderNpc);
				}
				return;
			}

			case 1:
			{
				if (idx == 0)
				{
					if (TileMap.mapID != villageMap)
					{
						ModTanSat.autoTanSat = false;
						ModNextMap.StartNextMap(villageMap);
						return;
					}
					ResetMobFilterToAll();
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

			case 2:
			{
				if (idx == 0)
				{
					if (TileMap.mapID != wildMap)
					{
						ModTanSat.autoTanSat = false;
						ModNextMap.StartNextMap(wildMap);
						return;
					}
					ResetMobFilterToAll();
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

			case 3:
			{
				ModTanSat.autoTanSat = false;
				if (idx == 0)
				{
					if (me.cTiemNang >= me.cDamGoc * 100L)
					{
						Service.gI().upPotential(2, 1);
					}
					else if (me.cTiemNang >= me.cHPGoc + 1000L)
					{
						Service.gI().upPotential(0, 1);
					}
					else
					{
						if (TileMap.mapID != wildMap)
						{
							ModNextMap.StartNextMap(wildMap);
							return;
						}
						ResetMobFilterToAll();
						ModTanSat.autoTanSat = true;
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
					Service.gI().pickItem(-1);
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

			case 4:
			case 5:
			case 6:
			{
				if (idx < 3)
				{
					int targetMap = GetTask4To6MapId(gender, idx);
					if (TileMap.mapID != targetMap)
					{
						ModTanSat.autoTanSat = false;
						ModNextMap.StartNextMap(targetMap);
						return;
					}
					SelectMobsByMaxHpRange(499, 500);
					ModAutoPick.autoPick = true;
					ModTanSat.autoTanSat = true;
				}
				else
				{
					ModTanSat.autoTanSat = false;
					ResetMobFilterToAll();
					if (TileMap.mapID != homeMap)
					{
						ModNextMap.StartNextMap(homeMap);
						return;
					}
					TalkToNpc(me, elderNpc);
				}
				return;
			}

			case 7:
			{
				if (!hasClaimedBaHatMitCharm)
				{
					if (TileMap.mapID != cliffMap)
					{
						ModTanSat.autoTanSat = false;
						ModNextMap.StartNextMap(cliffMap);
						return;
					}
					Npc baHatMit = FindNpcInMap(21);
					if (baHatMit != null)
					{
						TalkToNpc(me, 21, 0);
						if (GameCanvas.menu != null && !GameCanvas.menu.showMenu)
						{
							hasClaimedBaHatMitCharm = true;
						}
						return;
					}
					hasClaimedBaHatMitCharm = true;
				}

				if (idx == 0 || me.cPower < 40000L)
				{
					int trainMap = GetPowerTrainMapId(gender);
					if (TileMap.mapID != trainMap)
					{
						ModTanSat.autoTanSat = false;
						ModNextMap.StartNextMap(trainMap);
						return;
					}
					SelectMobsByMaxHpRange(1, 200);
					ModAutoPick.autoPick = true;
					ModTanSat.autoTanSat = true;
				}
				else if (idx == 1)
				{
					int targetMap = (gender == 0) ? 3 : ((gender == 1) ? 11 : 17);
					if (TileMap.mapID != targetMap)
					{
						ModTanSat.autoTanSat = false;
						ModNextMap.StartNextMap(targetMap);
						return;
					}
					SelectMobsByMaxHpRange(599, 600);
					ModAutoPick.autoPick = true;
					ModTanSat.autoTanSat = true;
				}
				else if (idx == 2)
				{
					ModTanSat.autoTanSat = false;
					ResetMobFilterToAll();
					if (TileMap.mapID != villageMap)
					{
						ModNextMap.StartNextMap(villageMap);
						return;
					}
					TalkToNpc(me, 7 + gender, 0);
				}
				else
				{
					ModTanSat.autoTanSat = false;
					ResetMobFilterToAll();
					if (TileMap.mapID != homeMap)
					{
						ModNextMap.StartNextMap(homeMap);
						return;
					}
					TalkToNpc(me, elderNpc);
				}
				return;
			}

			case 8:
			{
				if (idx == 0 || (idx == 1 && me.cPower < 140000L))
				{
					int trainMap = GetPowerTrainMapId(gender);
					if (TileMap.mapID != trainMap)
					{
						ModTanSat.autoTanSat = false;
						ModNextMap.StartNextMap(trainMap);
						return;
					}
					SelectMobsByMaxHpRange(1, 500);
					ModAutoPick.autoPick = true;
					ModTanSat.autoTanSat = true;
				}
				else if (idx == 1)
				{
					int targetMap = (gender == 0) ? 12 : ((gender == 1) ? 18 : 4);
					if (TileMap.mapID != targetMap)
					{
						ModTanSat.autoTanSat = false;
						ModNextMap.StartNextMap(targetMap);
						return;
					}
					SelectMobsByMaxHpRange(999, 1000);
					ModAutoPick.autoPick = true;
					ModTanSat.autoTanSat = true;
				}
				else if (idx == 2)
				{
					ModTanSat.autoTanSat = false;
					ResetMobFilterToAll();
					if (TileMap.mapID != homeMap)
					{
						ModNextMap.StartNextMap(homeMap);
						return;
					}
					TalkToNpc(me, elderNpc);
				}
				else
				{
					ModTanSat.autoTanSat = false;
					ResetMobFilterToAll();
					if (TileMap.mapID != 47)
					{
						ModNextMap.StartNextMap(47);
					}
				}
				return;
			}

			case 9:
			{
				ModTanSat.autoTanSat = false;
				ResetMobFilterToAll();
				if (idx <= 1)
				{
					if (TileMap.mapID != 47)
					{
						ModNextMap.StartNextMap(47);
						return;
					}
					TalkToNpc(me, 17, 0);
				}
				else if (idx == 2)
				{
					if (TileMap.mapID == 47)
					{
						ClimbKarinTowerFrom47(me);
					}
					else if (TileMap.mapID != 46)
					{
						ModNextMap.StartNextMap(47);
					}
				}
				else
				{
					if (TileMap.mapID == 46)
					{
						TalkToNpc(me, 18, 0);
					}
					else if (TileMap.mapID == 47)
					{
						ClimbKarinTowerFrom47(me);
					}
					else
					{
						ModNextMap.StartNextMap(47);
					}
				}
				return;
			}

			case 10:
			{
				ModTanSat.autoTanSat = false;
				ResetMobFilterToAll();
				if (idx == 0)
				{
					movedToTaoPaiPaiSpot = false;
					if (TileMap.mapID == 46)
					{
						Char karin = FindHostileCharInMap(me);
						if (karin != null)
						{
							AttackHostileQuestChar(me, karin, now);
						}
						else
						{
							Npc npcKarin = FindNpcInMap(18);
							if (npcKarin != null && Res.distance(me.cx, me.cy, npcKarin.cx, npcKarin.cy) > 40)
							{
								ModTeleport.TeleportTo(npcKarin.cx, npcKarin.cy - 3);
							}
							Service.gI().openMenu(18);
							Service.gI().confirmMenu(18, 3);
							Service.gI().confirmMenu(18, 0);
							if (GameCanvas.menu != null) GameCanvas.menu.showMenu = false;
						}
					}
					else if (TileMap.mapID == 47)
					{
						ClimbKarinTowerFrom47(me);
					}
					else
					{
						ModNextMap.StartNextMap(47);
					}
				}
				else if (idx == 1)
				{
					if (TileMap.mapID == 46)
					{
						me.currentMovePoint = new MovePoint(576, 552);
						return;
					}
					if (TileMap.mapID != 47)
					{
						ModNextMap.StartNextMap(47);
						return;
					}
					Char taoPaiPai = FindHostileCharInMap(me);
					if (taoPaiPai != null)
					{
						AttackHostileQuestChar(me, taoPaiPai, now);
					}
					else if (!movedToTaoPaiPaiSpot || Res.abs(me.cx - 371) > 24)
					{
						movedToTaoPaiPaiSpot = true;
						ModTeleport.TeleportTo(371, 336);
					}
				}
				else if (idx == 2)
				{
					if (TileMap.mapID != 47)
					{
						ModNextMap.StartNextMap(47);
						return;
					}
					TalkToNpc(me, 17, 0);
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
		}
	}
}
