using System;

public static class ModUIAutoHeal
{
	private static readonly int[] pcts = new int[] { 20, 30, 50, 70 };

	public static void Paint(int uiX, int uiY, int uiW, int uiH, mGraphics g)
	{
		// Hàng 1: Tự dùng đậu & Khóa HP/MP
		mFont.tahoma_7b_dark.drawString(g, "Tự dùng đậu:", uiX + 8, uiY + 12, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 86, uiY + 8, 52, 18, ModAutoHeal.autoPean ? "BẬT" : "TẮT", ModAutoHeal.autoPean, g);

		mFont.tahoma_7b_dark.drawString(g, "Khóa HP/MP:", uiX + 160, uiY + 12, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 246, uiY + 8, 52, 18, ModAutoHeal.lockHPMP ? "BẬT" : "TẮT", ModAutoHeal.lockHPMP, g);

		// Hàng 2: Ngưỡng tự ăn đậu
		mFont.tahoma_7b_dark.drawString(g, "Ngưỡng ăn đậu khi HP/KI dưới:", uiX + 8, uiY + 34, mFont.LEFT);
		int pBtnW = (uiW - 24) / pcts.Length;
		for (int p = 0; p < pcts.Length; p++)
		{
			int pbx = uiX + 6 + p * (pBtnW + 4);
			bool isSel = (ModAutoHeal.autoPeanHpPercent == pcts[p]);
			ModUI.PaintNativeButton(pbx, uiY + 50, pBtnW, 19, "< " + pcts[p] + "%", isSel, g);
		}

		// Hàng 3: Tự thu đậu & Cho đậu bang hội
		mFont.tahoma_7b_dark.drawString(g, "Tự thu đậu:", uiX + 8, uiY + 80, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 86, uiY + 76, 52, 18, ModAutoHeal.autoHarvestPea ? "BẬT" : "TẮT", ModAutoHeal.autoHarvestPea, g);

		mFont.tahoma_7b_dark.drawString(g, "Cho đậu bang:", uiX + 160, uiY + 80, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 246, uiY + 76, 52, 18, ModAutoHeal.autoDonateClan ? "BẬT" : "TẮT", ModAutoHeal.autoDonateClan, g);

		// Hàng 4: Ăn đậu khi đệ tử xin & Số lượng đậu hiện có
		mFont.tahoma_7b_dark.drawString(g, "Cho đệ khi xin:", uiX + 8, uiY + 108, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 96, uiY + 104, 52, 18, ModAutoHeal.autoFeedPetOnAsk ? "BẬT" : "TẮT", ModAutoHeal.autoFeedPetOnAsk, g);

		int beanCount = ModAutoHeal.GetBeanCount();
		mFont.tahoma_7_grey.drawString(g, "Đậu túi: ", uiX + 170, uiY + 108, mFont.LEFT);
		(beanCount > 0 ? mFont.tahoma_7b_green2 : mFont.tahoma_7b_red).drawString(g, beanCount + " hạt", uiX + 220, uiY + 108, mFont.LEFT);

		// Hàng 5: Nút thao tác nhanh
		int actBtnW = (uiW - 16) / 2;
		ModUI.PaintNativeButton(uiX + 6, uiY + 134, actBtnW, 22, "Thu Đậu Ngay", false, g);
		ModUI.PaintNativeButton(uiX + 10 + actBtnW, uiY + 134, actBtnW, 22, "Cho Đậu Bang Ngay", false, g);

		// Ghi chú nhỏ ở đáy
		int boxX = uiX + 6;
		int boxY = uiY + 166;
		int boxW = uiW - 12;
		int boxH = 44;
		GameCanvas.paintz.paintFrameSimple(boxX, boxY, boxW, boxH, g);
		g.setColor(15196114);
		g.fillRect(boxX + 2, boxY + 2, boxW - 4, boxH - 4);
		mFont.tahoma_7_grey.drawString(g, "* Tự động thu hoạch đậu trên cây thần và cứu sinh", boxX + boxW / 2, boxY + 8, mFont.CENTER);
		mFont.tahoma_7_grey.drawString(g, "lực đệ tử, thành viên bang hội tức thời.", boxX + boxW / 2, boxY + 24, mFont.CENTER);
	}

	public static bool HandleTap(int px, int py, int uiX, int uiY, int uiW, int uiH)
	{
		// 1. Tự dùng đậu
		if (px >= uiX + 86 && px <= uiX + 138 && py >= uiY + 6 && py <= uiY + 28)
		{
			ModAutoHeal.autoPean = !ModAutoHeal.autoPean;
			ModConfig.SaveConfig();
			SoundMn.gI().buttonClick();
			return true;
		}

		// 2. Khóa HP/MP
		if (px >= uiX + 246 && px <= uiX + 298 && py >= uiY + 6 && py <= uiY + 28)
		{
			ModAutoHeal.lockHPMP = !ModAutoHeal.lockHPMP;
			ModConfig.SaveConfig();
			SoundMn.gI().buttonClick();
			return true;
		}

		// 3. Ngưỡng ăn đậu
		int pBtnW = (uiW - 24) / pcts.Length;
		for (int p = 0; p < pcts.Length; p++)
		{
			int pbx = uiX + 6 + p * (pBtnW + 4);
			if (px >= pbx && px <= pbx + pBtnW && py >= uiY + 50 && py <= uiY + 70)
			{
				ModAutoHeal.autoPeanHpPercent = pcts[p];
				ModConfig.SaveConfig();
				SoundMn.gI().buttonClick();
				return true;
			}
		}

		// 4. Tự thu đậu
		if (px >= uiX + 86 && px <= uiX + 138 && py >= uiY + 74 && py <= uiY + 96)
		{
			ModAutoHeal.autoHarvestPea = !ModAutoHeal.autoHarvestPea;
			ModConfig.SaveConfig();
			GameScr.info1.addInfo("Tự thu đậu: " + (ModAutoHeal.autoHarvestPea ? "BẬT" : "TẮT"), 0);
			SoundMn.gI().buttonClick();
			return true;
		}

		// 5. Cho đậu bang
		if (px >= uiX + 246 && px <= uiX + 298 && py >= uiY + 74 && py <= uiY + 96)
		{
			ModAutoHeal.autoDonateClan = !ModAutoHeal.autoDonateClan;
			ModConfig.SaveConfig();
			GameScr.info1.addInfo("Cho đậu bang: " + (ModAutoHeal.autoDonateClan ? "BẬT" : "TẮT"), 0);
			SoundMn.gI().buttonClick();
			return true;
		}

		// 6. Cho đệ khi xin
		if (px >= uiX + 96 && px <= uiX + 148 && py >= uiY + 102 && py <= uiY + 124)
		{
			ModAutoHeal.autoFeedPetOnAsk = !ModAutoHeal.autoFeedPetOnAsk;
			ModConfig.SaveConfig();
			GameScr.info1.addInfo("Ăn đậu cho đệ tử: " + (ModAutoHeal.autoFeedPetOnAsk ? "BẬT" : "TẮT"), 0);
			SoundMn.gI().buttonClick();
			return true;
		}

		// 7. Nút Thu Đậu Ngay
		int actBtnW = (uiW - 16) / 2;
		if (px >= uiX + 6 && px <= uiX + 6 + actBtnW && py >= uiY + 134 && py <= uiY + 158)
		{
			ModAutoHeal.HarvestMagicTreeNow();
			return true;
		}

		// 8. Nút Cho Đậu Bang Ngay
		if (px >= uiX + 10 + actBtnW && px <= uiX + 10 + actBtnW * 2 && py >= uiY + 134 && py <= uiY + 158)
		{
			ModAutoHeal.DonateClanNow();
			return true;
		}

		return false;
	}
}
