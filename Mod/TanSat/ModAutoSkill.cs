using System;
using System.Text;

/// <summary>
/// Quản lý Tự Đánh Tại Chỗ (PK / Quái không dịch chuyển) và Tự Dùng 10 Ô Kỹ Năng độc lập theo chu kỳ riêng ngoài Tàn Sát.
/// </summary>
public static class ModAutoSkill
{
	public static bool isAutoAttackInPlace = false;
	public static readonly bool[] slotAutoUse = new bool[10];
	public static readonly int[] slotDelayMs = new int[10] { 500, 500, 500, 500, 500, 500, 500, 500, 500, 500 };

	private static readonly long[] lastSlotAutoUseTime = new long[10];
	private static readonly long[] lastInPlaceSlotAttackTime = new long[10];
	private static long lastGlobalSlotCastTime = 0;
	private static long lastGlobalFocusSwitchTime = 0;
	private static int editingSlotIndex = -1;
	private static bool reopenUiAfterMenu = false;

	private sealed class AutoSkillMenuListener : IActionListener
	{
		public void perform(int idAction, object p)
		{
			try
			{
				if (idAction >= 100 && idAction < 110)
				{
					ShowSingleSlotSubMenu(idAction - 100);
					return;
				}
				switch (idAction)
				{
					case 1:
					{
						int slot = (p is int idx) ? idx : editingSlotIndex;
						if (slot >= 0 && slot < 10)
						{
							slotAutoUse[slot] = !slotAutoUse[slot];
							ModConfig.SaveConfig();
							GameScr.info1?.addInfo(GetSlotDisplayName(slot) + " tự dùng: " + (slotAutoUse[slot] ? ("BẬT (" + slotDelayMs[slot] + "ms)") : "TẮT"), 0);
							ShowSingleSlotSubMenu(slot);
						}
						break;
					}
					case 2:
					{
						int slot = (p is int idx) ? idx : editingSlotIndex;
						if (slot >= 0 && slot < 10)
						{
							PromptSlotDelay(slot);
						}
						break;
					}
					case 3:
					{
						int slot = (p is int idx) ? idx : editingSlotIndex;
						Skill sk = GetHotbarSkillAt(slot);
						if (sk != null && sk.template != null)
						{
							ModTanSat.ToggleSkillFrozen(sk.template.id);
						}
						ShowSingleSlotSubMenu(slot);
						break;
					}
					case 4:
						ShowSlotListMenu(preserveReopenState: true);
						break;
					case 5:
						if (reopenUiAfterMenu)
						{
							reopenUiAfterMenu = false;
							ModUI.uiCustomOpen = true;
						}
						break;
					case 20:
						if (GameCanvas.inputDlg != null && GameCanvas.inputDlg.tfInput != null && editingSlotIndex >= 0 && editingSlotIndex < 10)
						{
							string raw = (GameCanvas.inputDlg.tfInput.getText() ?? string.Empty).Trim();
							if (int.TryParse(raw, out int ms))
							{
								if (ms < 100) ms = 100;
								if (ms > 10000) ms = 10000;
								slotDelayMs[editingSlotIndex] = ms;
								slotAutoUse[editingSlotIndex] = true;
								ModConfig.SaveConfig();
								GameScr.info1?.addInfo("Đã đặt " + GetSlotDisplayName(editingSlotIndex) + ": " + ms + "ms", 0);
							}
						}
						GameCanvas.endDlg();
						SoundMn.gI().buttonClick();
						ShowSingleSlotSubMenu(editingSlotIndex);
						break;
				}
			}
			catch
			{
				GameCanvas.endDlg();
			}
		}
	}

	private static readonly AutoSkillMenuListener menuListener = new AutoSkillMenuListener();

	public static void ToggleAutoAttackInPlace()
	{
		isAutoAttackInPlace = !isAutoAttackInPlace;
		if (isAutoAttackInPlace && ModTanSat.autoTanSat)
		{
			ModTanSat.autoTanSat = false;
		}
		GameScr.info1?.addInfo("Tự đánh tại chỗ (PK/Quái): " + (isAutoAttackInPlace ? "BẬT" : "TẮT"), 0);
		SoundMn.gI().buttonClick();
	}

	public static int CountEnabledAutoSlots()
	{
		int count = 0;
		for (int i = 0; i < 10; i++)
		{
			if (slotAutoUse[i]) count++;
		}
		return count;
	}

	private static Skill GetHotbarSkillAt(int slot)
	{
		Skill[] arr = ModTanSatFilter.GetHotbarSkills();
		if (arr != null && slot >= 0 && slot < arr.Length)
		{
			return arr[slot];
		}
		return null;
	}

	private static string GetSlotDisplayName(int slot)
	{
		Skill s = GetHotbarSkillAt(slot);
		string keyLabel = (slot == 9) ? "Ô 0" : ("Ô " + (slot + 1));
		if (s != null && s.template != null && !string.IsNullOrEmpty(s.template.name))
		{
			return keyLabel + " (" + s.template.name + ")";
		}
		return keyLabel + " (Trống)";
	}

	public static void ShowSlotListMenu(bool preserveReopenState = false)
	{
		if (GameCanvas.menu == null) return;
		if (!preserveReopenState)
		{
			reopenUiAfterMenu = ModUI.uiCustomOpen;
		}
		ModUI.uiCustomOpen = false;
		ModMenu.modMenuOpen = false;

		MyVector items = new MyVector();
		Skill[] arr = ModTanSatFilter.GetHotbarSkills();
		int maxSlots = (arr != null && arr.Length > 0 && arr.Length < 10) ? arr.Length : 10;
		for (int i = 0; i < maxSlots; i++)
		{
			string status = slotAutoUse[i] ? (" [ON " + slotDelayMs[i] + "ms]") : "";
			items.addElement(new Command(GetSlotDisplayName(i) + status, menuListener, 100 + i, i));
		}
		items.addElement(new Command("Đóng", menuListener, 5, null));
		GameCanvas.menu.startAt(items, 3);
	}

	private static void ShowSingleSlotSubMenu(int slot)
	{
		if (GameCanvas.menu == null || slot < 0 || slot >= 10) return;
		editingSlotIndex = slot;
		Skill sk = GetHotbarSkillAt(slot);
		bool isFrozen = ModTanSat.freezeSkillCooldown || (sk != null && sk.template != null && ModTanSat.frozenSkillIds.Contains(sk.template.id));

		MyVector items = new MyVector();
		items.addElement(new Command("Tự dùng: " + (slotAutoUse[slot] ? ("BẬT (" + slotDelayMs[slot] + "ms)") : "TẮT"), menuListener, 1, slot));
		items.addElement(new Command("Đặt độ trễ (100 - 10000ms)", menuListener, 2, slot));
		if (sk != null && sk.template != null)
		{
			items.addElement(new Command("Đóng băng hồi chiêu: " + (isFrozen ? "BẬT" : "TẮT"), menuListener, 3, slot));
		}
		items.addElement(new Command("Quay lại 10 Ô", menuListener, 4, null));
		GameCanvas.menu.startAt(items, 3);
	}

	private static void PromptSlotDelay(int slot)
	{
		if (GameCanvas.inputDlg == null) return;
		editingSlotIndex = slot;
		GameCanvas.inputDlg.show("Độ trễ tự dùng " + GetSlotDisplayName(slot) + " (100-10000ms):", new Command("OK", menuListener, 20, null), TField.INPUT_TYPE_NUMERIC);
		if (GameCanvas.inputDlg.tfInput != null)
		{
			GameCanvas.inputDlg.tfInput.setMaxTextLenght(6);
			GameCanvas.inputDlg.tfInput.setText(slotDelayMs[slot].ToString());
			if (!Main.isPC) GameCanvas.inputDlg.tfInput.setFocusWithKb(isFocus: true);
		}
	}

	public static string ExportConfig()
	{
		StringBuilder sb = new StringBuilder();
		for (int i = 0; i < 10; i++)
		{
			if (i > 0) sb.Append(',');
			sb.Append(i).Append(':').Append(slotAutoUse[i] ? '1' : '0').Append(':').Append(slotDelayMs[i]);
		}
		return sb.ToString();
	}

	public static void ImportConfig(string raw)
	{
		for (int i = 0; i < 10; i++)
		{
			slotAutoUse[i] = false;
			slotDelayMs[i] = 500;
		}
		if (string.IsNullOrEmpty(raw)) return;
		string[] parts = raw.Split(',');
		for (int i = 0; i < parts.Length; i++)
		{
			string[] kv = parts[i].Split(':');
			if (kv.Length == 3 && int.TryParse(kv[0].Trim(), out int idx) && idx >= 0 && idx < 10)
			{
				slotAutoUse[idx] = (kv[1].Trim() == "1" || kv[1].Trim().Equals("True", StringComparison.OrdinalIgnoreCase));
				if (int.TryParse(kv[2].Trim(), out int delay))
				{
					if (delay < 100) delay = 100;
					if (delay > 10000) delay = 10000;
					slotDelayMs[idx] = delay;
				}
			}
		}
	}

	public static void Update()
	{
		if (!ModMenu.IsInGame()) return;
		Char me = Char.myCharz();
		if (me == null || me.isDie || me.cHP <= 0 || me.statusMe == 14 || me.statusMe == 5) return;
		if (Char.isLoadingMap || Char.ischangingMap || ModAutoCaptcha.IsCaptchaActive()) return;

		long now = mSystem.currentTimeMillis();

		if (!ModTanSat.autoTanSat && ModTanSat.autoChangeFocus && now - lastGlobalFocusSwitchTime >= 5000)
		{
			lastGlobalFocusSwitchTime = now;
			me.findNextFocusByKey();
		}

		UpdateAutoSlots(me, now);

		if (isAutoAttackInPlace && !ModTanSat.autoTanSat)
		{
			UpdateInPlaceAttack(me, now);
		}
	}

	private static void UpdateAutoSlots(Char me, long now)
	{
		if (TileMap.mapID == 21 || TileMap.mapID == 22 || TileMap.mapID == 23) return;
		if (me.isCharge || me.isFreez || ModNextMap.isNextMapActive || ModGoBack.isReturning) return;
		if (now - lastGlobalSlotCastTime < 150) return;

		Skill[] hotbar = ModTanSatFilter.GetHotbarSkills();
		if (hotbar == null) return;

		int limit = hotbar.Length < 10 ? hotbar.Length : 10;
		for (int i = 0; i < limit; i++)
		{
			if (!slotAutoUse[i]) continue;
			Skill sk = hotbar[i];
			if (sk == null || sk.template == null || sk.paintCanNotUseSkill) continue;
			if (!ModTanSatFilter.HasEnoughMp(me, sk)) continue;

			int configuredDelay = slotDelayMs[i] > 0 ? slotDelayMs[i] : 500;
			int reqDelay = sk.coolDown > configuredDelay ? sk.coolDown : configuredDelay;
			if (now - lastSlotAutoUseTime[i] < reqDelay || now - sk.lastTimeUseThisSkill < sk.coolDown)
			{
				continue;
			}

			lastSlotAutoUseTime[i] = now;
			lastGlobalSlotCastTime = now;

			int tplId = sk.template.id;
			if (tplId == 7 && me.cgender == 1)
			{
				Skill prev = me.myskill;
				Service.gI().selectSkill(7);
				MyVector vChar = new MyVector();
				vChar.addElement((me.charFocus != null && me.charFocus.cHP > 0) ? me.charFocus : me);
				Service.gI().sendPlayerAttack(new MyVector(), vChar, 2);
				sk.lastTimeUseThisSkill = now;
				if (prev != null && prev.template != null && prev.template.id != 7)
				{
					Service.gI().selectSkill(prev.template.id);
				}
				return;
			}

			me.myskill = sk;
			Service.gI().selectSkill(tplId);
			if (GameScr.gI() != null)
			{
				GameScr.gI().doSelectSkill(sk, isShortcut: true);
			}
			sk.lastTimeUseThisSkill = now;
			return;
		}
	}

	private static bool CanAttackCharTarget(Char me, Char target, Skill sk)
	{
		if (target == null || target.cHP <= 0 || target.isDie || target.statusMe == 14 || target.statusMe == 5)
		{
			return false;
		}
		if (sk == null || sk.template == null || sk.template.type == 2)
		{
			return false;
		}
		if (me.isMeCanAttackOtherPlayer(target))
		{
			return true;
		}
		if (TileMap.mapID == 113)
		{
			return target.cTypePk == 5 || target.cTypePk == 3;
		}
		if (me.cTypePk == 5 || target.cTypePk == 5)
		{
			return true;
		}
		if ((me.cFlag == 8 && target.cFlag != 0) ||
		    (me.cFlag != 0 && target.cFlag == 8) ||
		    (me.cFlag != 0 && target.cFlag != 0 && me.cFlag != target.cFlag))
		{
			return true;
		}
		return (me.cTypePk == 3 && target.cTypePk == 3) ||
		       (me.cTypePk == 1 && target.cTypePk == 1) ||
		       (me.cTypePk == 4 && target.cTypePk == 4);
	}

	private static void UpdateInPlaceAttack(Char me, long now)
	{
		if (me.isCharge || me.isFreez) return;
		Skill sk = me.myskill;
		if (sk == null || sk.template == null || sk.template.type == 3) return;
		int tplId = sk.template.id;
		if (tplId == 10 || tplId == 11) return;
		if (sk.paintCanNotUseSkill && !ModTanSat.freezeSkillCooldown && !ModTanSat.frozenSkillIds.Contains(tplId)) return;
		if (!ModTanSatFilter.HasEnoughMp(me, sk)) return;

		int slotIdx = ModTanSatFilter.GetSkillHotbarSlot(tplId);
		if (slotIdx < 0 || slotIdx >= 10) slotIdx = 0;

		bool isSpecialDelaySkill = (tplId == 7 || tplId == 18 || tplId == 20 || tplId == 22 || tplId == 23);
		int baseCd = (ModTanSat.freezeSkillCooldown || ModTanSat.frozenSkillIds.Contains(tplId)) ? 0 : sk.coolDown;
		long reqCd = baseCd + (isSpecialDelaySkill ? 500L : 100L);
		if (now - lastInPlaceSlotAttackTime[slotIdx] < reqCd || now - sk.lastTimeUseThisSkill < baseCd)
		{
			return;
		}

		int maxReachX = (int)(sk.dx * 1.7);
		int maxReachY = (int)(sk.dy * 1.7);
		if (maxReachX < 48) maxReachX = 48;
		if (maxReachY < 48) maxReachY = 48;

		Char cTarget = me.charFocus;
		if (cTarget != null && CanAttackCharTarget(me, cTarget, sk))
		{
			if (Res.abs(cTarget.cx - me.cx) <= maxReachX && Res.abs(cTarget.cy - me.cy) <= maxReachY)
			{
				lastInPlaceSlotAttackTime[slotIdx] = now;
				sk.lastTimeUseThisSkill = now;
				me.cdir = (cTarget.cx >= me.cx) ? 1 : -1;
				Service.gI().selectSkill(tplId);
				MyVector vChar = new MyVector();
				vChar.addElement(cTarget);
				Service.gI().sendPlayerAttack(new MyVector(), vChar, 2);
				return;
			}
		}

		Mob mTarget = me.mobFocus;
		if (mTarget != null && !mTarget.isMobMe && mTarget.hp > 0 && mTarget.status != 0 && mTarget.status != 1 &&
		    GameScr.gI() != null && GameScr.gI().isMeCanAttackMob(mTarget))
		{
			if (Res.abs(mTarget.x - me.cx) <= maxReachX && Res.abs(mTarget.y - me.cy) <= maxReachY)
			{
				lastInPlaceSlotAttackTime[slotIdx] = now;
				sk.lastTimeUseThisSkill = now;
				me.cdir = (mTarget.x >= me.cx) ? 1 : -1;
				Service.gI().selectSkill(tplId);
				MyVector vMob = new MyVector();
				vMob.addElement(mTarget);
				Service.gI().sendPlayerAttack(vMob, new MyVector(), 1);
			}
		}
	}
}
