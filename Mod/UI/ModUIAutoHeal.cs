using System;

public static class ModUIAutoHeal
{
	private static readonly int[] pcts = new int[] { 20, 30, 50, 70 };

	public static void Paint(int uiX, int uiY, int uiW, int uiH, mGraphics g)
	{
		// Hàng 1: Tự dùng đậu & Khóa HP/MP
		mFont.tahoma_7_grey.drawString(g, "Tự dùng đậu:", uiX + 6, uiY + 7, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 74, uiY + 4, 42, 16, ModAutoHeal.autoPean ? "BẬT" : "TẮT", ModAutoHeal.autoPean, g);

		int rBtn1W = 42;
		int rBtn1X = uiX + uiW - rBtn1W - 6;
		mFont.tahoma_7_grey.drawString(g, "Khóa HP/MP:", rBtn1X - 68, uiY + 7, mFont.LEFT);
		ModUI.PaintNativeButton(rBtn1X, uiY + 4, rBtn1W, 16, ModAutoHeal.lockHPMP ? "BẬT" : "TẮT", ModAutoHeal.lockHPMP, g);

		// Hàng 2: Ngưỡng tự ăn đậu
		mFont.tahoma_7_grey.drawString(g, "Ngưỡng ăn đậu khi HP/KI dưới:", uiX + 6, uiY + 27, mFont.LEFT);
		int pBtnW = (uiW - 20) / pcts.Length;
		for (int p = 0; p < pcts.Length; p++)
		{
			int pbx = uiX + 5 + p * (pBtnW + 3);
			bool isSel = (ModAutoHeal.autoPeanHpPercent == pcts[p]);
			ModUI.PaintNativeButton(pbx, uiY + 43, pBtnW, 17, "< " + pcts[p] + "%", isSel, g);
		}

		// Hàng 3: Tự thu đậu & Cho đậu bang hội
		mFont.tahoma_7_grey.drawString(g, "Tự thu đậu:", uiX + 6, uiY + 70, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 74, uiY + 67, 42, 16, ModAutoHeal.autoHarvestPea ? "BẬT" : "TẮT", ModAutoHeal.autoHarvestPea, g);

		int rBtn3W = 42;
		int rBtn3X = uiX + uiW - rBtn3W - 6;
		mFont.tahoma_7_grey.drawString(g, "Cho đậu bang:", rBtn3X - 74, uiY + 70, mFont.LEFT);
		ModUI.PaintNativeButton(rBtn3X, uiY + 67, rBtn3W, 16, ModAutoHeal.autoDonateClan ? "BẬT" : "TẮT", ModAutoHeal.autoDonateClan, g);

		// Hàng 4: Ăn đậu khi đệ tử xin & Số lượng đậu hiện có
		mFont.tahoma_7_grey.drawString(g, "Cho đệ khi xin:", uiX + 6, uiY + 95, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 80, uiY + 92, 42, 16, ModAutoFeedPet(), ModAutoHeal.autoFeedPetOnAsk, g);

		int beanCount = ModAutoHeal.GetBeanCount();
		int rBeanX = uiX + uiW - 84;
		mFont.tahoma_7_grey.drawString(g, "Đậu túi: ", rBeanX, uiY + 95, mFont.LEFT);
		(beanCount > 0 ? mFont.tahoma_7_green2 : mFont.tahoma_7_red).drawString(g, beanCount + " hạt", rBeanX + 42, uiY + 95, mFont.LEFT);

		// Hàng 5: Nút thao tác nhanh
		int actBtnW = (uiW - 14) / 2;
		ModUI.PaintNativeButton(uiX + 5, uiY + 118, actBtnW, 18, "Thu Đậu Ngay", false, g);
		ModUI.PaintNativeButton(uiX + 9 + actBtnW, uiY + 118, actBtnW, 18, "Cho Đậu Bang Ngay", false, g);

		// Ghi chú nhỏ ở đáy
		int boxX = uiX + 5;
		int boxY = uiY + 144;
		int boxW = uiW - 10;
		int boxH = 64;
		GameCanvas.paintz.paintFrameSimple(boxX, boxY, boxW, boxH, g);
		g.setColor(15196114);
		g.fillRect(boxX + 2, boxY + 2, boxW - 4, boxH - 4);
		mFont.tahoma_7_grey.drawString(g, "* Tự động thu hoạch đậu trên cây thần và cứu sinh", boxX + boxW / 2, boxY + 14, mFont.CENTER);
		mFont.tahoma_7_grey.drawString(g, "lực đệ tử, thành viên bang hội tức thời.", boxX + boxW / 2, boxY + 34, mFont.CENTER);
	}

	private static string ModAutoFeedPet() => ModAutoHeal.autoFeedPetOnAsk ? "BẬT" : "TẮT";

	public static bool HandleTap(int px, int py, int uiX, int uiY, int uiW, int uiH)
	{
		// 1. Tự dùng đậu
		if (px >= uiX + 74 && px <= uiX + 116 && py >= uiY + 3 && py <= uiY + 21)
		{
			ModAutoHeal.autoPean = !ModAutoHeal.autoPean;
			ModConfig.SaveConfig();
			SoundMn.gI().buttonClick();
			return true;
		}

		// 2. Khóa HP/MP
		int rBtn1W = 42;
		int rBtn1X = uiX + uiW - rBtn1W - 6;
		if (px >= rBtn1X && px <= rBtn1X + rBtn1W && py >= uiY + 3 && py <= uiY + 21)
		{
			ModAutoHeal.lockHPMP = !ModAutoHeal.lockHPMP;
			ModConfig.SaveConfig();
			SoundMn.gI().buttonClick();
			return true;
		}

		// 3. Ngưỡng ăn đậu
		int pBtnW = (uiW - 20) / pcts.Length;
		for (int p = 0; p < pcts.Length; p++)
		{
			int pbx = uiX + 5 + p * (pBtnW + 3);
			if (px >= pbx && px <= pbx + pBtnW && py >= uiY + 42 && py <= uiY + 61)
			{
				ModAutoHeal.autoPeanHpPercent = pcts[p];
				ModConfig.SaveConfig();
				SoundMn.gI().buttonClick();
				return true;
			}
		}

		// 4. Tự thu đậu
		if (px >= uiX + 74 && px <= uiX + 116 && py >= uiY + 66 && py <= uiY + 84)
		{
			ModAutoHeal.autoHarvestPea = !ModAutoHeal.autoHarvestPea;
			ModConfig.SaveConfig();
			GameScr.info1.addInfo("Tự thu đậu: " + (ModAutoHeal.autoHarvestPea ? "BẬT" : "TẮT"), 0);
			SoundMn.gI().buttonClick();
			return true;
		}

		// 5. Cho đậu bang
		int rBtn3W = 42;
		int rBtn3X = uiX + uiW - rBtn3W - 6;
		if (px >= rBtn3X && px <= rBtn3X + rBtn3W && py >= uiY + 66 && py <= uiY + 84)
		{
			ModAutoHeal.autoDonateClan = !ModAutoHeal.autoDonateClan;
			ModConfig.SaveConfig();
			GameScr.info1.addInfo("Cho đậu bang: " + (ModAutoHeal.autoDonateClan ? "BẬT" : "TẮT"), 0);
			SoundMn.gI().buttonClick();
			return true;
		}

		// 6. Cho đệ khi xin
		if (px >= uiX + 80 && px <= uiX + 122 && py >= uiY + 91 && py <= uiY + 109)
		{
			ModAutoHeal.autoFeedPetOnAsk = !ModAutoHeal.autoFeedPetOnAsk;
			ModConfig.SaveConfig();
			GameScr.info1.addInfo("Ăn đậu cho đệ tử: " + (ModAutoHeal.autoFeedPetOnAsk ? "BẬT" : "TẮT"), 0);
			SoundMn.gI().buttonClick();
			return true;
		}

		// 7. Nút Thu Đậu Ngay
		int actBtnW = (uiW - 14) / 2;
		if (px >= uiX + 5 && px <= uiX + 5 + actBtnW && py >= uiY + 117 && py <= uiY + 137)
		{
			ModAutoHeal.HarvestMagicTreeNow();
			return true;
		}

		// 8. Nút Cho Đậu Bang Ngay
		if (px >= uiX + 9 + actBtnW && px <= uiX + 9 + actBtnW * 2 && py >= uiY + 117 && py <= uiY + 137)
		{
			ModAutoHeal.DonateClanNow();
			return true;
		}

		return false;
	}
}
