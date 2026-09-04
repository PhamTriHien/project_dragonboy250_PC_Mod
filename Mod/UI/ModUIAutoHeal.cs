using System;

public static class ModUIAutoHeal
{
	private static readonly int[] pcts = new int[] { 20, 30, 50, 70 };

	public static void Paint(int uiX, int uiY, int uiW, int uiH, mGraphics g)
	{
		mFont.tahoma_7b_white.drawString(g, "Tự dùng đậu:", uiX + 20, uiY + 56, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 110, uiY + 50, 85, ModAutoHeal.autoPean ? "BẬT" : "TẮT", ModAutoHeal.autoPean, g);

		mFont.tahoma_7b_yellow.drawString(g, "Ngưỡng tự ăn đậu khi HP/KI dưới:", uiX + 20, uiY + 82, mFont.LEFT);
		for (int p = 0; p < pcts.Length; p++)
		{
			int px = uiX + 16 + p * 77;
			bool isSel = (ModAutoHeal.autoPeanHpPercent == pcts[p]);
			ModUI.PaintNativeButton(px, uiY + 102, 72, "< " + pcts[p] + "%", isSel, g);
		}

		mFont.tahoma_7b_white.drawString(g, "Khóa HP/MP:", uiX + 20, uiY + 138, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 110, uiY + 132, 85, ModAutoHeal.lockHPMP ? "BẬT" : "TẮT", ModAutoHeal.lockHPMP, g);

		mFont.tahoma_7_grey.drawString(g, "* Tự động ăn đậu thần hồi phục đầy cây khi farm hoặc treo map.", uiX + uiW / 2, uiY + 175, mFont.CENTER);
	}

	public static bool HandleTap(int px, int py, int uiX, int uiY, int uiW, int uiH)
	{
		if (px >= uiX + 110 && px <= uiX + 195 && py >= uiY + 50 && py <= uiY + 72)
		{
			ModAutoHeal.autoPean = !ModAutoHeal.autoPean;
			ModConfig.SaveConfig();
			SoundMn.gI().buttonClick();
			return true;
		}

		for (int p = 0; p < pcts.Length; p++)
		{
			int pbx = uiX + 16 + p * 77;
			if (px >= pbx && px <= pbx + 72 && py >= uiY + 102 && py <= uiY + 124)
			{
				ModAutoHeal.autoPeanHpPercent = pcts[p];
				ModConfig.SaveConfig();
				SoundMn.gI().buttonClick();
				return true;
			}
		}

		if (px >= uiX + 110 && px <= uiX + 195 && py >= uiY + 132 && py <= uiY + 154)
		{
			ModAutoHeal.lockHPMP = !ModAutoHeal.lockHPMP;
			ModConfig.SaveConfig();
			SoundMn.gI().buttonClick();
			return true;
		}

		return false;
	}
}
