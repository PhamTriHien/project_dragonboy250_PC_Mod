using System;

/// <summary>
/// Module Auto Cộng Tiềm Năng: Hỗ trợ đồng thời 5 mốc chỉ số gốc (HP, KI, Sức Đánh, Giáp, Chí Mạng) hoặc cộng đơn từng chỉ số.
/// </summary>
public static class ModAutoPoint
{
	public static bool isEnabled;
	public static int targetType = 0; // 0: HP, 1: KI, 2: SĐ, 3: Giáp, 4: Chí mạng
	public static int pointsPerTick = 1;
	public static int intervalMs = 1000;
	public static long maxTargetStat = 0; // Tương thích ngược cho chế độ 1 chỉ số
	public static readonly long[] maxTargetStats = new long[5]; // [0]=HP, [1]=KI, [2]=SĐ, [3]=Giáp, [4]=CM

	private static readonly int[] MultiStatPriority = new int[] { 2, 0, 1, 3, 4 };
	private static readonly string[] StatShortNames = new string[] { "HP Gốc", "KI Gốc", "Sức Đánh", "Giáp", "Chí Mạng" };

	private static long nextAt;
	private static int editingStatIndex = -1;
	private static bool reopenModUiAfterInput;

	public static void Configure(int type, int amount, int interval)
	{
		targetType = type < 0 ? 0 : (type > 4 ? 4 : type);
		pointsPerTick = (amount >= 100) ? 100 : ((amount >= 10) ? 10 : 1);
		intervalMs = interval < 500 ? 500 : (interval > 60000 ? 60000 : interval);
	}

	public static bool HasAnyMultiStatTarget()
	{
		for (int i = 0; i < maxTargetStats.Length; i++)
		{
			if (maxTargetStats[i] > 0) return true;
		}
		return false;
	}

	public static string ExportMultiStatTargets()
	{
		string[] parts = new string[5];
		for (int i = 0; i < 5; i++) parts[i] = maxTargetStats[i].ToString();
		return string.Join(",", parts);
	}

	public static void ImportMultiStatTargets(string raw)
	{
		for (int i = 0; i < 5; i++) maxTargetStats[i] = 0;
		if (string.IsNullOrEmpty(raw)) return;
		string[] parts = raw.Split(',');
		for (int i = 0; i < parts.Length && i < 5; i++)
		{
			if (long.TryParse(parts[i].Trim(), out long val) && val > 0)
			{
				maxTargetStats[i] = val;
			}
		}
	}

	public static void Toggle()
	{
		isEnabled = !isEnabled;
		nextAt = 0;
		GameScr.info1?.addInfo("Auto cộng điểm: " + (isEnabled ? "BẬT" : "TẮT"), 0);
		ModConfig.SaveConfig();
	}

	public static void ShowInputMaxTargetStat()
	{
		ShowInputStatTarget(-1);
	}

	public static void ShowInputStatTarget(int statIdx)
	{
		if (GameCanvas.inputDlg == null) return;
		editingStatIndex = (statIdx >= 0 && statIdx < 5) ? statIdx : -1;
		reopenModUiAfterInput = ModUI.uiCustomOpen;
		ModUI.uiCustomOpen = false;
		ModMenu.modMenuOpen = false;

		string title = (editingStatIndex >= 0)
			? ("Mốc " + StatShortNames[editingStatIndex] + " cần đạt (0 = Bỏ qua):")
			: "Nhập mốc chỉ số gốc tự dừng (0 = Không giới hạn):";
		long curVal = (editingStatIndex >= 0) ? maxTargetStats[editingStatIndex] : maxTargetStat;

		GameCanvas.inputDlg.show(title, new Command("OK", new MaxStatActionListener(), 1, null), TField.INPUT_TYPE_NUMERIC);
		if (GameCanvas.inputDlg.tfInput != null)
		{
			GameCanvas.inputDlg.tfInput.setMaxTextLenght(15);
			GameCanvas.inputDlg.tfInput.setText(curVal > 0 ? curVal.ToString() : "0");
			if (!Main.isPC) GameCanvas.inputDlg.tfInput.setFocusWithKb(isFocus: true);
		}
	}

	private class MaxStatActionListener : IActionListener
	{
		public void perform(int idAction, object p)
		{
			try
			{
				if (idAction == 1 && GameCanvas.inputDlg != null && GameCanvas.inputDlg.tfInput != null)
				{
					string text = (GameCanvas.inputDlg.tfInput.getText() ?? string.Empty).Trim();
					long val = (long.TryParse(text, out long parsed) && parsed > 0) ? parsed : 0;
					if (editingStatIndex >= 0 && editingStatIndex < 5)
					{
						maxTargetStats[editingStatIndex] = val;
						GameScr.info1?.addInfo("Mốc " + StatShortNames[editingStatIndex] + ": " + (val > 0 ? Res.formatNumber2(val) : "Tắt"), 0);
					}
					else
					{
						maxTargetStat = val;
						GameScr.info1?.addInfo("Mốc dừng cộng điểm: " + (val > 0 ? Res.formatNumber2(val) : "Không giới hạn"), 0);
					}
					ModConfig.SaveConfig();
					SoundMn.gI().buttonClick();
				}
				GameCanvas.endDlg();
				if (reopenModUiAfterInput)
				{
					reopenModUiAfterInput = false;
					ModUI.uiCustomOpen = true;
				}
			}
			catch
			{
				GameCanvas.endDlg();
			}
		}
	}

	public static long GetCurrentBaseStat(Char me, int type)
	{
		if (me == null) return 0;
		switch (type)
		{
			case 0: return me.cHPGoc;
			case 1: return me.cMPGoc;
			case 2: return me.cDamGoc;
			case 3: return me.cDefGoc;
			case 4: return me.cCriticalGoc;
			default: return 0;
		}
	}

	private static long GetRequiredPotential(Char me, int type, int num)
	{
		if (me == null || num <= 0) return long.MaxValue;
		switch (type)
		{
			case 0:
			{
				long baseHp = me.cHPGoc + 1000L;
				if (num == 1) return baseHp;
				if (num == 10) return 10L * (2L * baseHp + 180L) / 2L;
				if (num == 100) return 100L * (2L * baseHp + 1980L) / 2L;
				return long.MaxValue;
			}
			case 1:
			{
				long baseMp = me.cMPGoc + 1000L;
				if (num == 1) return baseMp;
				if (num == 10) return 10L * (2L * baseMp + 180L) / 2L;
				if (num == 100) return 100L * (2L * baseMp + 1980L) / 2L;
				return long.MaxValue;
			}
			case 2:
			{
				long expForOne = me.expForOneAdd > 0 ? me.expForOneAdd : 100L;
				long baseDam = me.cDamGoc;
				if (num == 1) return baseDam * expForOne;
				if (num == 10) return 10L * (2L * baseDam + 9L) / 2L * expForOne;
				if (num == 100) return 100L * (2L * baseDam + 99L) / 2L * expForOne;
				return long.MaxValue;
			}
			case 3:
			{
				long baseDef = me.cDefGoc + 5L;
				if (num == 1) return (2L * baseDef) / 2L * 100000L;
				if (num == 10) return 10L * (2L * baseDef + 9L) / 2L * 100000L;
				if (num == 100) return 100L * (2L * baseDef + 99L) / 2L * 100000L;
				return long.MaxValue;
			}
			case 4:
			{
				if (num != 1) return long.MaxValue;
				int idx = me.cCriticalGoc;
				if (Panel.t_tiemnang == null || idx < 0 || idx >= Panel.t_tiemnang.Length) return long.MaxValue;
				return Panel.t_tiemnang[idx];
			}
			default:
				return long.MaxValue;
		}
	}

	private static int SelectBestStepForStat(Char me, int type, long targetCap)
	{
		long cur = GetCurrentBaseStat(me, type);
		if (targetCap > 0 && cur >= targetCap) return 0;

		int[] tiers = (type == 4)
			? new int[] { 1 }
			: ((pointsPerTick >= 100) ? new int[] { 100, 10, 1 } : ((pointsPerTick >= 10) ? new int[] { 10, 1 } : new int[] { 1 }));

		for (int i = 0; i < tiers.Length; i++)
		{
			int step = tiers[i];
			if (targetCap > 0 && step > 1)
			{
				long statGain = (type == 0 || type == 1) ? (step * 20L) : step;
				if (cur + statGain > targetCap) continue;
			}
			long cost = GetRequiredPotential(me, type, step);
			if (me.cTiemNang >= cost)
			{
				return step;
			}
		}
		return 0;
	}

	public static void Update()
	{
		if (!isEnabled || !ModMenu.IsInGame() || Char.myCharz() == null || GameScr.gI() == null || GameCanvas.gameTick % 2 != 0) return;
		if (Char.isLoadingMap || Char.ischangingMap) return;
		Char me = Char.myCharz();
		if (me.isDie || me.cHP <= 0 || me.statusMe == 14 || me.statusMe == 5 || me.cTiemNang <= 0) return;

		long now = mSystem.currentTimeMillis();
		if (now < nextAt) return;

		// 1. Chế độ đa chỉ số 5 mục đồng thời (ưu tiên SĐ -> HP -> KI -> Giáp -> Chí Mạng)
		if (HasAnyMultiStatTarget())
		{
			bool anyBelowCap = false;
			for (int p = 0; p < MultiStatPriority.Length; p++)
			{
				int st = MultiStatPriority[p];
				long cap = maxTargetStats[st];
				if (cap <= 0) continue;
				if (GetCurrentBaseStat(me, st) >= cap) continue;

				anyBelowCap = true;
				int step = SelectBestStepForStat(me, st, cap);
				if (step > 0)
				{
					Service.gI().upPotential(st, step);
					nextAt = now + intervalMs;
					return;
				}
			}

			if (!anyBelowCap)
			{
				isEnabled = false;
				ModConfig.SaveConfig();
				GameScr.info1?.addInfo("Đã hoàn thành toàn bộ các mốc chỉ số tiềm năng!", 0);
			}
			return;
		}

		// 2. Chế độ cộng đơn 1 chỉ số
		if (maxTargetStat > 0 && GetCurrentBaseStat(me, targetType) >= maxTargetStat)
		{
			isEnabled = false;
			ModConfig.SaveConfig();
			GameScr.info1?.addInfo("Đã đạt mốc chỉ số gốc (" + Res.formatNumber2(maxTargetStat) + ")! Dừng Auto Cộng Điểm.", 0);
			return;
		}

		int chosenAmount = SelectBestStepForStat(me, targetType, maxTargetStat);
		if (chosenAmount <= 0) return;
		Service.gI().upPotential(targetType, chosenAmount);
		nextAt = now + intervalMs;
	}
}
