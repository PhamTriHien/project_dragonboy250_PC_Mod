using System;

/// <summary>
/// Module Auto Cộng Tiềm Năng: Tự động cộng chỉ số sức mạnh, HP, KI, giáp, chí mạng theo thiết lập và mốc giới hạn dừng.
/// </summary>
public static class ModAutoPoint
{
	public static bool isEnabled;
	public static int targetType = 0; // 0: HP, 1: KI, 2: SĐ, 3: Giáp, 4: Chí mạng
	public static int pointsPerTick = 1;
	public static int intervalMs = 1000;
	public static long maxTargetStat = 0; // 0 = Không giới hạn
	private static long nextAt;
	private static bool reopenModUiAfterInput;

	/// <summary>
	/// Cấu hình tham số và thiết lập hoạt động cho module.
	/// </summary>
	public static void Configure(int type, int amount, int interval)
	{
		targetType = type < 0 ? 0 : (type > 4 ? 4 : type);
		pointsPerTick = (amount >= 100) ? 100 : ((amount >= 10) ? 10 : 1);
		intervalMs = interval < 500 ? 500 : (interval > 60000 ? 60000 : interval);
	}

	/// <summary>
	/// Bật hoặc tắt trạng thái kích hoạt của Toggle.
	/// </summary>
	public static void Toggle()
	{
		isEnabled = !isEnabled;
		nextAt = 0;
		GameScr.info1.addInfo("Auto cộng điểm: " + (isEnabled ? "BẬT" : "TẮT"), 0);
		ModConfig.SaveConfig();
	}

	/// <summary>
	/// Mở hộp thoại nhập mốc chỉ số giới hạn tự dừng cộng điểm (0 = Không giới hạn).
	/// </summary>
	public static void ShowInputMaxTargetStat()
	{
		if (GameCanvas.inputDlg == null) return;
		reopenModUiAfterInput = ModUI.uiCustomOpen;
		ModUI.uiCustomOpen = false;
		ModMenu.modMenuOpen = false;
		GameCanvas.inputDlg.show("Nhập mốc chỉ số gốc tự dừng (0 = Không giới hạn):", new Command("OK", new MaxStatActionListener(), 1, null), TField.INPUT_TYPE_NUMERIC);
		if (GameCanvas.inputDlg.tfInput != null)
		{
			GameCanvas.inputDlg.tfInput.setMaxTextLenght(15);
			GameCanvas.inputDlg.tfInput.setText(maxTargetStat > 0 ? maxTargetStat.ToString() : "0");
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
					if (long.TryParse(text, out long val) && val >= 0)
					{
						maxTargetStat = val;
					}
					else
					{
						maxTargetStat = 0;
					}
					ModConfig.SaveConfig();
					GameCanvas.endDlg();
					GameScr.info1?.addInfo("Mốc dừng cộng điểm: " + (maxTargetStat > 0 ? Res.formatNumber2(maxTargetStat) : "Không giới hạn"), 0);
					SoundMn.gI().buttonClick();
				}
				else
				{
					GameCanvas.endDlg();
				}
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
			case 0: // HP Gốc
			{
				long baseHp = me.cHPGoc + 1000L;
				if (num == 1) return baseHp;
				if (num == 10) return 10L * (2L * baseHp + 180L) / 2L;
				if (num == 100) return 100L * (2L * baseHp + 1980L) / 2L;
				return long.MaxValue;
			}
			case 1: // KI Gốc
			{
				long baseMp = me.cMPGoc + 1000L;
				if (num == 1) return baseMp;
				if (num == 10) return 10L * (2L * baseMp + 180L) / 2L;
				if (num == 100) return 100L * (2L * baseMp + 1980L) / 2L;
				return long.MaxValue;
			}
			case 2: // Sức Đánh Gốc
			{
				long expForOne = me.expForOneAdd > 0 ? me.expForOneAdd : 100L;
				long baseDam = me.cDamGoc;
				if (num == 1) return baseDam * expForOne;
				if (num == 10) return 10L * (2L * baseDam + 9L) / 2L * expForOne;
				if (num == 100) return 100L * (2L * baseDam + 99L) / 2L * expForOne;
				return long.MaxValue;
			}
			case 3: // Giáp Gốc
			{
				long baseDef = me.cDefGoc + 5L;
				if (num == 1) return (2L * baseDef) / 2L * 100000L;
				if (num == 10) return 10L * (2L * baseDef + 9L) / 2L * 100000L;
				if (num == 100) return 100L * (2L * baseDef + 99L) / 2L * 100000L;
				return long.MaxValue;
			}
			case 4: // Chí Mạng Gốc
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

	/// <summary>
	/// Cập nhật trạng thái logic của đối tượng theo từng chu kỳ khung hình.
	/// </summary>
	public static void Update()
	{
		if (!isEnabled || !ModMenu.IsInGame() || Char.myCharz() == null || GameScr.gI() == null || GameCanvas.gameTick % 2 != 0) return;
		if (Char.isLoadingMap || Char.ischangingMap) return;
		Char me = Char.myCharz();
		if (me.isDie || me.cHP <= 0 || me.statusMe == 14 || me.statusMe == 5 || me.cTiemNang <= 0) return;

		if (maxTargetStat > 0 && GetCurrentBaseStat(me, targetType) >= maxTargetStat)
		{
			isEnabled = false;
			ModConfig.SaveConfig();
			GameScr.info1.addInfo("Đã đạt mốc chỉ số gốc (" + Res.formatNumber2(maxTargetStat) + ")! Dừng Auto Cộng Điểm.", 0);
			return;
		}

		long now = mSystem.currentTimeMillis();
		if (now < nextAt) return;

		int[] tiers = (targetType == 4)
			? new int[] { 1 }
			: ((pointsPerTick >= 100) ? new int[] { 100, 10, 1 } : ((pointsPerTick >= 10) ? new int[] { 10, 1 } : new int[] { 1 }));

		int chosenAmount = 0;
		for (int i = 0; i < tiers.Length; i++)
		{
			long cost = GetRequiredPotential(me, targetType, tiers[i]);
			if (me.cTiemNang >= cost)
			{
				chosenAmount = tiers[i];
				break;
			}
		}
		if (chosenAmount <= 0) return;
		Service.gI().upPotential(targetType, chosenAmount);
		nextAt = now + intervalMs;
	}
}
