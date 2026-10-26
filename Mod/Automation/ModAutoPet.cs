using System;

/// <summary>
/// Module Auto Đệ Tử: Quản lý trạng thái đệ tử, đánh mồi khi đệ sủa, Auto KOK, HUD chỉ số đệ, né Boss đổi khu, cờ đen thông minh, mốc SM ngắt kết nối và trị thương Namếc.
/// </summary>
public static class ModAutoPet
{
	public static bool isEnabled;
	public static int status = 2;
	public static int intervalMs = 5000;
	public static bool showPetHUD = false;
	public static bool petAvoidBoss = false;
	public static bool petAutoFlag = false;
	public static bool autoNamekHeal = false;
	public static bool petAutoAttackOnBark = false;
	public static bool petAutoKok = false;
	public static bool disconnectOnPetPowerLimit = false;
	public static long exitOnPetPowerLimit = 0;

	private static long nextAt;
	private static long lastSilentPetInfoTime;
	private static long silentPetInfoExpireAt;
	private static long lastFlagTime;
	private static long lastHealTime;
	private static long lastBarkAttackTime;
	private static long lastKokTime;
	private static long lastBossZoneSwitchTime;
	private static bool isEvadingBoss;
	private static bool reopenUiAfterPowerInput;

	private static readonly string[] petStatusNames = new string[] { "Đi Theo", "Bảo Vệ", "Tấn Công", "Về Nhà" };

	public static void CyclePetPowerLimit()
	{
		if (exitOnPetPowerLimit <= 0) exitOnPetPowerLimit = 149_900_000L;
		else if (exitOnPetPowerLimit == 149_900_000L) exitOnPetPowerLimit = 1_499_000_000L;
		else if (exitOnPetPowerLimit == 1_499_000_000L) exitOnPetPowerLimit = 17_990_000_000L;
		else exitOnPetPowerLimit = 0;
		ModConfig.SaveConfig();
		SoundMn.gI().buttonClick();
	}

	public static void ShowInputPetPowerLimit()
	{
		if (GameCanvas.inputDlg == null) return;
		reopenUiAfterPowerInput = ModUI.uiCustomOpen;
		ModUI.uiCustomOpen = false;
		ModMenu.modMenuOpen = false;
		GameCanvas.inputDlg.show("Nhập mốc Sức Mạnh Đệ Tử tự dừng (0 = Tắt):", new Command("OK", new PetPowerLimitListener(), 1, null), TField.INPUT_TYPE_NUMERIC);
		if (GameCanvas.inputDlg.tfInput != null)
		{
			GameCanvas.inputDlg.tfInput.setMaxTextLenght(15);
			GameCanvas.inputDlg.tfInput.setText(exitOnPetPowerLimit > 0 ? exitOnPetPowerLimit.ToString() : "0");
			if (!Main.isPC) GameCanvas.inputDlg.tfInput.setFocusWithKb(isFocus: true);
		}
	}

	private class PetPowerLimitListener : IActionListener
	{
		public void perform(int idAction, object p)
		{
			try
			{
				if (idAction == 1 && GameCanvas.inputDlg != null && GameCanvas.inputDlg.tfInput != null)
				{
					string raw = (GameCanvas.inputDlg.tfInput.getText() ?? string.Empty).Trim();
					if (long.TryParse(raw, out long val) && val > 0)
					{
						exitOnPetPowerLimit = val;
					}
					else
					{
						exitOnPetPowerLimit = 0;
					}
					ModConfig.SaveConfig();
					GameScr.info1?.addInfo("Đệ Tử - " + GetPetPowerLimitLabel(), 0);
					SoundMn.gI().buttonClick();
				}
				GameCanvas.endDlg();
				if (reopenUiAfterPowerInput)
				{
					reopenUiAfterPowerInput = false;
					ModUI.uiCustomOpen = true;
				}
			}
			catch
			{
				GameCanvas.endDlg();
			}
		}
	}

	public static string GetPetPowerLimitLabel()
	{
		if (exitOnPetPowerLimit <= 0) return "Mốc SM: Tắt";
		if (exitOnPetPowerLimit == 149_900_000L) return "Mốc: 149.9Tr";
		if (exitOnPetPowerLimit == 1_499_000_000L) return "Mốc: 1.49T";
		if (exitOnPetPowerLimit == 17_990_000_000L) return "Mốc: 17.99T";
		return "Mốc: " + Res.formatNumber2(exitOnPetPowerLimit);
	}

	public static bool ConsumeSilentPetInfoRequest()
	{
		long now = mSystem.currentTimeMillis();
		if (silentPetInfoExpireAt > now)
		{
			silentPetInfoExpireAt = 0;
			return true;
		}
		return false;
	}

	public static void Configure(int value, int interval)
	{
		status = value < 0 ? 0 : (value > 3 ? 3 : value);
		intervalMs = interval < 1000 ? 1000 : (interval > 3600000 ? 3600000 : interval);
		isEnabled = true;
		nextAt = 0;
		ModConfig.SaveConfig();
	}

	public static void Toggle()
	{
		if (status < 0 || status > 3) status = 2;
		isEnabled = !isEnabled;
		nextAt = 0;
		ModConfig.SaveConfig();
		GameScr.info1?.addInfo("Auto Đệ Tử: " + (isEnabled ? "BẬT" : "TẮT"), 0);
	}

	public static Char FindMyPetInMap(Char me)
	{
		if (me == null || !me.havePet || GameScr.vCharInMap == null)
		{
			return null;
		}
		Char myPet = Char.myPetz();
		for (int i = 0; i < GameScr.vCharInMap.size(); i++)
		{
			Char c = (Char)GameScr.vCharInMap.elementAt(i);
			if (c == null) continue;
			if (c.charID == -me.charID)
			{
				return c;
			}
			if (myPet != null && !string.IsNullOrEmpty(myPet.cName) && !string.IsNullOrEmpty(c.cName) && c.cName.EndsWith(myPet.cName))
			{
				return c;
			}
		}
		return null;
	}

	/// <summary>
	/// Bắt sự kiện Đệ Tử nói "Sao sư phụ không đánh đi?" để Sư Phụ lập tức đánh mồi 1 đòn giúp đệ tiếp tục tấn công.
	/// </summary>
	public static void OnCharChatMap(Char speaker, string chat)
	{
		if (!petAutoAttackOnBark || speaker == null || string.IsNullOrEmpty(chat) || !ModMenu.IsInGame()) return;
		Char me = Char.myCharz();
		if (me == null || !me.havePet || me.isDie || me.cHP <= 0) return;

		Char myPet = Char.myPetz();
		bool isMyPet = (speaker.charID == -me.charID) ||
		               (myPet != null && !string.IsNullOrEmpty(myPet.cName) && !string.IsNullOrEmpty(speaker.cName) && speaker.cName.EndsWith(myPet.cName));
		if (!isMyPet) return;

		string lower = chat.ToLowerInvariant();
		if (!lower.Contains("không đánh") && !lower.Contains("khong danh")) return;

		long now = mSystem.currentTimeMillis();
		if (now - lastBarkAttackTime < 600) return;
		lastBarkAttackTime = now;

		TriggerMasterBaitAttack(me);
	}

	private static void TriggerMasterBaitAttack(Char me)
	{
		try
		{
			if (GameScr.vMob == null || GameScr.vMob.size() == 0) return;
			Skill attackSk = null;
			Skill[] hotbar = ModTanSatFilter.GetHotbarSkills();
			if (hotbar != null)
			{
				for (int i = 0; i < hotbar.Length; i++)
				{
					Skill s = hotbar[i];
					if (s != null && s.template != null && s.template.type == 1)
					{
						attackSk = s;
						break;
					}
				}
			}
			if (attackSk == null && me.myskill != null && ModTanSatFilter.IsAttackSkill(me.myskill))
			{
				attackSk = me.myskill;
			}
			if (attackSk == null || attackSk.template == null) return;

			Mob nearestMob = null;
			int bestDist = int.MaxValue;
			for (int i = 0; i < GameScr.vMob.size(); i++)
			{
				Mob m = (Mob)GameScr.vMob.elementAt(i);
				if (m == null || m.hp <= 0 || m.status <= 1 || m.isMobMe) continue;
				int dist = Res.abs(me.cx - m.x) + Res.abs(me.cy - m.y);
				if (dist < bestDist)
				{
					bestDist = dist;
					nearestMob = m;
				}
			}
			if (nearestMob == null) return;

			Service.gI().selectSkill(attackSk.template.id);
			me.mobFocus = nearestMob;
			me.myskill = attackSk;
			MyVector vMob = new MyVector();
			vMob.addElement(nearestMob);
			Service.gI().sendPlayerAttack(vMob, new MyVector(), 1);
		}
		catch
		{
		}
	}

	private static bool HasOtherFlaggedPlayerInMap(Char me)
	{
		if (GameScr.vCharInMap == null || me == null) return false;
		for (int i = 0; i < GameScr.vCharInMap.size(); i++)
		{
			Char c = (Char)GameScr.vCharInMap.elementAt(i);
			if (c == null || c.charID == me.charID || c.charID == -me.charID) continue;
			if (c.charID > 0 && c.cFlag != 0 && c.cHP > 0 && !c.isDie)
			{
				return true;
			}
		}
		return false;
	}

	private static int PickNextSafeZone()
	{
		int[] zones = (GameScr.gI() != null) ? GameScr.gI().zones : null;
		int[] maxPlayers = (GameScr.gI() != null) ? GameScr.gI().maxPlayer : null;
		if (zones == null || zones.Length <= 1) return -1;
		int curZ = TileMap.zoneID;
		for (int offset = 1; offset < zones.Length; offset++)
		{
			int cand = (curZ + offset) % zones.Length;
			int cap = (maxPlayers != null && cand < maxPlayers.Length && maxPlayers[cand] > 0) ? maxPlayers[cand] : 15;
			if (zones[cand] < cap - 1)
			{
				return cand;
			}
		}
		return (curZ + 1) % zones.Length;
	}

	private static Skill FindNamekHealSkill(Char me)
	{
		if (me == null) return null;
		if (GameScr.keySkill != null)
		{
			for (int i = 0; i < GameScr.keySkill.Length; i++)
			{
				Skill s = GameScr.keySkill[i];
				if (s != null && s.template != null && s.template.id == 7) return s;
			}
		}
		if (GameScr.onScreenSkill != null)
		{
			for (int i = 0; i < GameScr.onScreenSkill.Length; i++)
			{
				Skill s = GameScr.onScreenSkill[i];
				if (s != null && s.template != null && s.template.id == 7) return s;
			}
		}
		if (me.vSkill != null)
		{
			for (int i = 0; i < me.vSkill.size(); i++)
			{
				Skill s = (Skill)me.vSkill.elementAt(i);
				if (s != null && s.template != null && s.template.id == 7) return s;
			}
		}
		return null;
	}

	public static void Update()
	{
		if (!ModMenu.IsInGame()) return;
		Char me = Char.myCharz();
		if (me == null || !me.havePet || Char.isLoadingMap || Char.ischangingMap) return;

		long now = mSystem.currentTimeMillis();

		Char petInMap = FindMyPetInMap(me);
		if (petInMap != null)
		{
			Char myPet = Char.myPetz();
			if (myPet != null)
			{
				myPet.cHP = petInMap.cHP;
				if (petInMap.cHPFull > 0)
				{
					myPet.cHPFull = petInMap.cHPFull;
				}
			}
		}

		if ((showPetHUD || exitOnPetPowerLimit > 0) && (GameCanvas.panel == null || !GameCanvas.panel.isShow) && GameCanvas.currentDialog == null && !ModUI.uiCustomOpen)
		{
			if (now - lastSilentPetInfoTime >= 12000)
			{
				lastSilentPetInfoTime = now;
				silentPetInfoExpireAt = now + 2500;
				Service.gI().petInfo();
			}
		}

		// 0. Kiểm tra mốc sức mạnh đệ tử để tự động thu đệ về nhà & dừng Tàn Sát (và tùy chọn ngắt kết nối hẳn)
		if (exitOnPetPowerLimit > 0)
		{
			Char myPetCheck = Char.myPetz();
			if (myPetCheck != null && myPetCheck.cPower >= exitOnPetPowerLimit)
			{
				Service.gI().petStatus(3);
				myPetCheck.petStatus = 3;
				status = 3;
				if (ModTanSat.autoTanSat)
				{
					ModTanSat.autoTanSat = false;
				}
				long reached = myPetCheck.cPower;
				exitOnPetPowerLimit = 0;
				ModConfig.SaveConfig();
				GameScr.info1?.addInfo("Đệ tử đã đạt mốc SM (" + Res.formatNumber2(reached) + ")! Đã cho đệ về nhà!", 0);
				SoundMn.gI().buttonClick();
				if (disconnectOnPetPowerLimit)
				{
					ModAutoLogin.isEnabled = false;
					ModAutoLogin.isReconnecting = false;
					Session_ME.gI().close();
				}
				return;
			}
		}

		// 0b. Auto KOK (nhấp nhích nhẹ Sư Phụ định kỳ để giữ đồng bộ vị trí giúp Đệ Tử xuất chiêu liên tục)
		if (petAutoKok && !me.isDie && me.cHP > 0 && me.statusMe == 1 && me.currentMovePoint == null && !ModNextMap.isNextMapActive && now - lastKokTime >= 900)
		{
			lastKokTime = now;
			int origY = me.cy;
			me.cy = origY - 1;
			Service.gI().charMove();
			me.cy = origY;
			Service.gI().charMove();
		}

		// 1. Tự động cho Đệ về nhà & đổi khu an toàn khi phát hiện Boss trong khu
		if (petAvoidBoss)
		{
			Char liveBoss = ModBossNotice.FindLiveBossInMap();
			if (liveBoss != null)
			{
				if (!isEvadingBoss || now >= nextAt)
				{
					isEvadingBoss = true;
					Service.gI().petStatus(3);
					nextAt = now + 2500;
					if (Char.myPetz() != null) Char.myPetz().petStatus = 3;
				}
				if (petAutoFlag && me.cFlag != 0 && now - lastFlagTime >= 2500)
				{
					lastFlagTime = now;
					Service.gI().getFlag(1, 0);
				}
				if (!TileMap.isOfflineMap() && now - lastBossZoneSwitchTime >= 3000)
				{
					lastBossZoneSwitchTime = now;
					int safeZone = PickNextSafeZone();
					if (safeZone >= 0 && safeZone != TileMap.zoneID)
					{
						Service.gI().requestChangeZone(safeZone, -1);
						GameScr.info1?.addInfo("Né Boss [" + liveBoss.cName + "] -> Đổi sang Khu " + safeZone, 0);
					}
					else if (GameScr.gI() != null)
					{
						Service.gI().openUIZone();
					}
				}
				return;
			}
			else if (isEvadingBoss)
			{
				isEvadingBoss = false;
				nextAt = 0;
			}
		}

		// 2. Cờ Đen thông minh: Hạ cờ ngay nếu có người chơi khác mang cờ trong khu để chống bị PK lén, bật lại cờ đen (8) khi an toàn
		if (petAutoFlag && !me.isDie && me.cHP > 0)
		{
			bool dangerFlaggedPlayer = HasOtherFlaggedPlayerInMap(me);
			if (dangerFlaggedPlayer)
			{
				if (me.cFlag != 0 && now - lastFlagTime >= 2500)
				{
					lastFlagTime = now;
					Service.gI().getFlag(1, 0);
				}
			}
			else if (me.cFlag != 8 && now - lastFlagTime >= 5000)
			{
				lastFlagTime = now;
				Service.gI().getFlag(1, 8);
			}
		}

		// 3. Tự động dùng Trị Thương (Namếc - Skill ID 7) cứu/bơm máu cho Đệ Tử
		if (autoNamekHeal && me.cgender == 1 && !me.isDie && me.cHP > 0 && petInMap != null && now - lastHealTime >= 1200)
		{
			bool needHeal = petInMap.isDie || petInMap.cHP <= 0 || (petInMap.cHPFull > 0 && petInMap.cHP * 100L / petInMap.cHPFull <= 35);
			if (needHeal)
			{
				Skill healSkill = FindNamekHealSkill(me);
				if (healSkill != null && !healSkill.paintCanNotUseSkill && now - healSkill.lastTimeUseThisSkill >= healSkill.coolDown)
				{
					lastHealTime = now;
					healSkill.lastTimeUseThisSkill = now;
					Skill prevSkill = me.myskill;
					Service.gI().selectSkill(7);
					MyVector vMob = new MyVector();
					MyVector vChar = new MyVector();
					vChar.addElement(petInMap);
					Service.gI().sendPlayerAttack(vMob, vChar, 2);
					if (prevSkill != null && prevSkill.template != null && prevSkill.template.id != 7)
					{
						Service.gI().selectSkill(prevSkill.template.id);
					}
				}
			}
		}

		// 4. Duy trì trạng thái lệnh Đệ Tử
		if (!isEnabled) return;
		if (status < 0 || status > 3) status = 2;
		if (now < nextAt) return;
		Service.gI().petStatus((sbyte)status);
		if (Char.myPetz() != null) Char.myPetz().petStatus = (sbyte)status;
		nextAt = now + intervalMs;
	}

	public static void PaintPetHUD(mGraphics g)
	{
		if (!showPetHUD || !ModMenu.IsInGame()) return;
		if ((GameCanvas.panel != null && GameCanvas.panel.isShow) ||
		    (GameCanvas.panel2 != null && GameCanvas.panel2.isShow) ||
		    (GameCanvas.menu != null && GameCanvas.menu.showMenu) ||
		    GameCanvas.currentDialog != null ||
		    ModUI.uiCustomOpen)
		{
			return;
		}

		Char me = Char.myCharz();
		if (me == null || !me.havePet) return;
		Char pet = Char.myPetz();
		if (pet == null) return;

		try
		{
			g.translate(-g.getTranslateX(), -g.getTranslateY());
			g.setClip(0, 0, GameCanvas.w, GameCanvas.h);

			int x = 6 + SafeArea.Left;
			int y = 76;

			string petName = string.IsNullOrEmpty(pet.cName) ? "Đệ Tử" : pet.cName;
			int stIdx = (pet.petStatus >= 0 && pet.petStatus < petStatusNames.Length) ? pet.petStatus : status;
			string stLabel = (stIdx >= 0 && stIdx < petStatusNames.Length) ? petStatusNames[stIdx] : "Tấn Công";
			if (isEvadingBoss) stLabel = "Né Boss";

			string line1 = "[Đệ] " + petName + " (" + stLabel + ")";
			string line2 = "HP: " + Res.formatNumber2(pet.cHP) + "/" + Res.formatNumber2(pet.cHPFull) + " | TL: " + pet.cStamina;
			string line3 = "SM: " + Res.formatNumber2(pet.cPower) + " | SĐ: " + Res.formatNumber2(pet.cDamFull);

			(mFont.tahoma_7b_yellow ?? mFont.tahoma_7_yellow).drawString(g, line1, x, y, mFont.LEFT);
			(mFont.tahoma_7_yellow ?? mFont.tahoma_7_white).drawString(g, line2, x, y + 11, mFont.LEFT);
			mFont.tahoma_7_white.drawString(g, line3, x, y + 22, mFont.LEFT);
		}
		catch
		{
		}
	}
}
