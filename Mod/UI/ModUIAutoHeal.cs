using System;

public static class ModUIAutoHeal
{
	private static readonly int[] pcts = new int[] { 20, 30, 50, 70 };

	public static void Paint(int uiX, int uiY, int uiW, int uiH, mGraphics g)
	{
		// Hàng 1: Tự dùng đậu & Khóa HP/MP
		mFont.tahoma_7b_white.drawString(g, "Tự dùng đậu:", uiX + 18, uiY + 52, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 96, uiY + 48, 52, 18, ModAutoHeal.autoPean ? "BẬT" : "TẮT", ModAutoHeal.autoPean, g);

		mFont.tahoma_7b_white.drawString(g, "Khóa HP/MP:", uiX + 175, uiY + 52, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 258, uiY + 48, 52, 18, ModAutoHeal.lockHPMP ? "BẬT" : "TẮT", ModAutoHeal.lockHPMP, g);

		// Hàng 2: Ngưỡng tự ăn đậu
		mFont.tahoma_7b_yellow.drawString(g, "Ngưỡng ăn đậu khi HP/KI dưới:", uiX + 18, uiY + 72, mFont.LEFT);
		for (int p = 0; p < pcts.Length; p++)
		{
			int px = uiX + 16 + p * 77;
			bool isSel = (ModAutoHeal.autoPeanHpPercent == pcts[p]);
			ModUI.PaintNativeButton(px, uiY + 88, 60, 18, "< " + pcts[p] + "%", isSel, g);
		}

		// Hàng 3: Tự thu đậu & Cho đậu bang hội
		mFont.tahoma_7b_white.drawString(g, "Tự thu đậu:", uiX + 18, uiY + 116, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 96, uiY + 112, 52, 18, ModAutoHeal.autoHarvestPea ? "BẬT" : "TẮT", ModAutoHeal.autoHarvestPea, g);

		mFont.tahoma_7b_white.drawString(g, "Cho đậu bang:", uiX + 175, uiY + 116, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 258, uiY + 112, 52, 18, ModAutoHeal.autoDonateClan ? "BẬT" : "TẮT", ModAutoHeal.autoDonateClan, g);

		// Hàng 4: Ăn đậu khi đệ tử xin & Số lượng đậu hiện có
		mFont.tahoma_7b_white.drawString(g, "Cho đệ khi xin:", uiX + 18, uiY + 140, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 110, uiY + 136, 52, 18, ModAutoHeal.autoFeedPetOnAsk ? "BẬT" : "TẮT", ModAutoHeal.autoFeedPetOnAsk, g);

		int beanCount = ModAutoHeal.GetBeanCount();
		mFont.tahoma_7_white.drawString(g, "Đậu túi: ", uiX + 180, uiY + 140, mFont.LEFT);
		(beanCount > 0 ? mFont.tahoma_7b_green2 : mFont.tahoma_7b_red).drawString(g, beanCount + " hạt", uiX + 230, uiY + 140, mFont.LEFT);

		// Hàng 5: Nút thao tác nhanh
		ModUI.PaintNativeButton(uiX + 18, uiY + 165, 140, 20, "Thu Đậu Ngay", false, g);
		ModUI.PaintNativeButton(uiX + 170, uiY + 165, 145, 20, "Cho Đậu Bang Ngay", false, g);

		// Ghi chú nhỏ ở đáy
		mFont.tahoma_7_grey.drawString(g, "* Tự động thu hoạch, tặng đậu bang và cứu sinh lực đệ tử tức thời.", uiX + uiW / 2, uiY + 195, mFont.CENTER);
	}

	public static bool HandleTap(int px, int py, int uiX, int uiY, int uiW, int uiH)
	{
		// 1. Tự dùng đậu
		if (px >= uiX + 96 && px <= uiX + 148 && py >= uiY + 48 && py <= uiY + 66)
		{
			ModAutoHeal.autoPean = !ModAutoHeal.autoPean;
			ModConfig.SaveConfig();
			SoundMn.gI().buttonClick();
			return true;
		}

		// 2. Khóa HP/MP
		if (px >= uiX + 258 && px <= uiX + 310 && py >= uiY + 48 && py <= uiY + 66)
		{
			ModAutoHeal.lockHPMP = !ModAutoHeal.lockHPMP;
			ModConfig.SaveConfig();
			SoundMn.gI().buttonClick();
			return true;
		}

		// 3. Ngưỡng ăn đậu
		for (int p = 0; p < pcts.Length; p++)
		{
			int pbx = uiX + 16 + p * 77;
			if (px >= pbx && px <= pbx + 60 && py >= uiY + 88 && py <= uiY + 106)
			{
				ModAutoHeal.autoPeanHpPercent = pcts[p];
				ModConfig.SaveConfig();
				SoundMn.gI().buttonClick();
				return true;
			}
		}

		// 4. Tự thu đậu
		if (px >= uiX + 96 && px <= uiX + 148 && py >= uiY + 112 && py <= uiY + 130)
		{
			ModAutoHeal.autoHarvestPea = !ModAutoHeal.autoHarvestPea;
			ModConfig.SaveConfig();
			GameScr.info1.addInfo("Tự thu đậu: " + (ModAutoHeal.autoHarvestPea ? "BẬT" : "TẮT"), 0);
			SoundMn.gI().buttonClick();
			return true;
		}

		// 5. Cho đậu bang
		if (px >= uiX + 258 && px <= uiX + 310 && py >= uiY + 112 && py <= uiY + 130)
		{
			ModAutoHeal.autoDonateClan = !ModAutoHeal.autoDonateClan;
			ModConfig.SaveConfig();
			GameScr.info1.addInfo("Cho đậu bang: " + (ModAutoHeal.autoDonateClan ? "BẬT" : "TẮT"), 0);
			SoundMn.gI().buttonClick();
			return true;
		}

		// 6. Cho đệ khi xin
		if (px >= uiX + 110 && px <= uiX + 162 && py >= uiY + 136 && py <= uiY + 154)
		{
			ModAutoHeal.autoFeedPetOnAsk = !ModAutoHeal.autoFeedPetOnAsk;
			ModConfig.SaveConfig();
			GameScr.info1.addInfo("Ăn đậu cho đệ tử: " + (ModAutoHeal.autoFeedPetOnAsk ? "BẬT" : "TẮT"), 0);
			SoundMn.gI().buttonClick();
			return true;
		}

		// 7. Nút Thu Đậu Ngay
		if (px >= uiX + 18 && px <= uiX + 158 && py >= uiY + 165 && py <= uiY + 185)
		{
			ModAutoHeal.HarvestMagicTreeNow();
			return true;
		}

		// 8. Nút Cho Đậu Bang Ngay
		if (px >= uiX + 170 && px <= uiX + 315 && py >= uiY + 165 && py <= uiY + 185)
		{
			ModAutoHeal.DonateClanNow();
			return true;
		}

		return false;
	}
}
