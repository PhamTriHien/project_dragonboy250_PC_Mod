using System;

public static class ModUIAutoHeal
{
	private static readonly int[] pcts = new int[] { 20, 30, 50, 70 };

	public static void Paint(int uiX, int uiY, int uiW, int uiH, mGraphics g)
	{
		mFont.tahoma_7b_white.drawString(g, "Tự dùng đậu:", uiX + 20, uiY + 54, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 95, uiY + 50, 52, 18, ModAutoHeal.autoPean ? "BẬT" : "TẮT", ModAutoHeal.autoPean, g);

		mFont.tahoma_7b_yellow.drawString(g, "Ngưỡng tự ăn đậu khi HP/KI dưới:", uiX + 20, uiY + 80, mFont.LEFT);
		for (int p = 0; p < pcts.Length; p++)
		{
			int px = uiX + 16 + p * 77;
			bool isSel = (ModAutoHeal.autoPeanHpPercent == pcts[p]);
			ModUI.PaintNativeButton(px, uiY + 98, 60, 18, "< " + pcts[p] + "%", isSel, g);
		}

		mFont.tahoma_7b_white.drawString(g, "Khóa HP/MP:", uiX + 20, uiY + 134, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 95, uiY + 130, 52, 18, ModAutoHeal.lockHPMP ? "BẬT" : "TẮT", ModAutoHeal.lockHPMP, g);

		mFont.tahoma_7_grey.drawString(g, "* Tự động ăn đậu thần hồi phục đầy cây khi farm hoặc treo map.", uiX + uiW / 2, uiY + 175, mFont.CENTER);
	}

	public static bool HandleTap(int px, int py, int uiX, int uiY, int uiW, int uiH)
	{
		if (px >= uiX + 95 && px <= uiX + 147 && py >= uiY + 50 && py <= uiY + 68)
		{
			ModAutoHeal.autoPean = !ModAutoHeal.autoPean;
			ModConfig.SaveConfig();
			SoundMn.gI().buttonClick();
			return true;
		}

		for (int p = 0; p < pcts.Length; p++)
		{
			int pbx = uiX + 16 + p * 77;
			if (px >= pbx && px <= pbx + 60 && py >= uiY + 98 && py <= uiY + 116)
			{
				ModAutoHeal.autoPeanHpPercent = pcts[p];
				ModConfig.SaveConfig();
				SoundMn.gI().buttonClick();
				return true;
			}
		}

		if (px >= uiX + 95 && px <= uiX + 147 && py >= uiY + 130 && py <= uiY + 148)
		{
			ModAutoHeal.lockHPMP = !ModAutoHeal.lockHPMP;
			ModConfig.SaveConfig();
			SoundMn.gI().buttonClick();
			return true;
		}

		return false;
	}
}
