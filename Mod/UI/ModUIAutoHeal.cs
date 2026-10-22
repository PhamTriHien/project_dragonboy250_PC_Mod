using System;

public static class ModUIAutoHeal
{
	private static readonly int[] pcts = new int[] { 20, 30, 50, 70 };

	public static void Paint(int uiX, int uiY, int uiW, int uiH, mGraphics g)
	{
		int halfW = (uiW - 14) / 2;
		int item1X = uiX + 4;
		int item2X = uiX + 8 + halfW;

		// Hàng 1: Tự dùng đậu & Khóa HP/MP
		mFont.tahoma_7_grey.drawString(g, "Tự dùng đậu:", item1X + 2, uiY + 7, mFont.LEFT);
		ModUI.PaintNativeButton(item1X + halfW - 38, uiY + 4, 38, 17, ModAutoHeal.autoPean ? "BẬT" : "TẮT", ModAutoHeal.autoPean, g);

		mFont.tahoma_7_grey.drawString(g, "Khóa HP/MP:", item2X + 2, uiY + 7, mFont.LEFT);
		ModUI.PaintNativeButton(item2X + halfW - 38, uiY + 4, 38, 17, ModAutoHeal.lockHPMP ? "BẬT" : "TẮT", ModAutoHeal.lockHPMP, g);

		// Hàng 2: Ngưỡng tự ăn đậu
		mFont.tahoma_7_grey.drawString(g, "Ngưỡng ăn đậu khi HP/KI dưới:", uiX + 6, uiY + 24, mFont.LEFT);
		int pBtnW = (uiW - 20) / pcts.Length;
		for (int p = 0; p < pcts.Length; p++)
		{
			int pbx = uiX + 4 + p * (pBtnW + 4);
			bool isSel = (ModAutoHeal.autoPeanHpPercent == pcts[p]);
			ModUI.PaintNativeButton(pbx, uiY + 39, pBtnW, 17, "< " + pcts[p] + "%", isSel, g);
		}

		// Hàng 3: Tự thu đậu & Cho đậu bang hội
		mFont.tahoma_7_grey.drawString(g, "Tự thu đậu:", item1X + 2, uiY + 62, mFont.LEFT);
		ModUI.PaintNativeButton(item1X + halfW - 38, uiY + 59, 38, 17, ModAutoHeal.autoHarvestPea ? "BẬT" : "TẮT", ModAutoHeal.autoHarvestPea, g);

		mFont.tahoma_7_grey.drawString(g, "Cho bang:", item2X + 2, uiY + 62, mFont.LEFT);
		ModUI.PaintNativeButton(item2X + halfW - 38, uiY + 59, 38, 17, ModAutoHeal.autoDonateClan ? "BẬT" : "TẮT", ModAutoHeal.autoDonateClan, g);

		// Hàng 4: Ăn đậu khi đệ tử xin & Số lượng đậu hiện có
		mFont.tahoma_7_grey.drawString(g, "Cho đệ xin:", item1X + 2, uiY + 81, mFont.LEFT);
		ModUI.PaintNativeButton(item1X + halfW - 38, uiY + 78, 38, 17, ModAutoFeedPet(), ModAutoHeal.autoFeedPetOnAsk, g);

		int beanCount = ModAutoHeal.GetBeanCount();
		mFont.tahoma_7_grey.drawString(g, "Đậu túi: ", item2X + 2, uiY + 81, mFont.LEFT);
		(beanCount > 0 ? mFont.tahoma_7_green2 : mFont.tahoma_7_red).drawString(g, beanCount + " hạt", item2X + 46, uiY + 81, mFont.LEFT);

		// Hàng 5: Nút thao tác nhanh
		int actBtnW = (uiW - 12) / 2;
		ModUI.PaintNativeButton(uiX + 4, uiY + 98, actBtnW, 17, "Thu Đậu Ngay", false, g);
		ModUI.PaintNativeButton(uiX + 8 + actBtnW, uiY + 98, actBtnW, 17, "Cho Đậu Bang", false, g);

		// Ghi chú nhỏ ở đáy co dãn linh hoạt
		int boxX = uiX + 4;
		int boxY = uiY + 119;
		int boxW = uiW - 8;
		int boxH = uiH - 123;
		if (boxH < 36) boxH = 36;
		GameCanvas.paintz.paintFrameSimple(boxX, boxY, boxW, boxH, g);
		g.setColor(15196114);
		g.fillRect(boxX + 2, boxY + 2, boxW - 4, boxH - 4);
		mFont.tahoma_7_grey.drawString(g, "* Tự động thu đậu trên cây thần & cứu sinh", boxX + boxW / 2, boxY + 6, mFont.CENTER);
		mFont.tahoma_7_grey.drawString(g, "đệ tử, thành viên bang hội tức thời.", boxX + boxW / 2, boxY + 20, mFont.CENTER);
	}

	private static string ModAutoFeedPet() => ModAutoHeal.autoFeedPetOnAsk ? "BẬT" : "TẮT";

	public static bool HandleTap(int px, int py, int uiX, int uiY, int uiW, int uiH)
	{
		int halfW = (uiW - 14) / 2;
		int item1X = uiX + 4;
		int item2X = uiX + 8 + halfW;

		// 1. Tự dùng đậu
		if (px >= item1X + halfW - 40 && px <= item1X + halfW + 2 && py >= uiY + 2 && py <= uiY + 22)
		{
			ModAutoHeal.autoPean = !ModAutoHeal.autoPean;
			ModConfig.SaveConfig();
			SoundMn.gI().buttonClick();
			return true;
		}

		// 2. Khóa HP/MP
		if (px >= item2X + halfW - 40 && px <= item2X + halfW + 2 && py >= uiY + 2 && py <= uiY + 22)
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
			int pbx = uiX + 4 + p * (pBtnW + 4);
			if (px >= pbx && px <= pbx + pBtnW && py >= uiY + 38 && py <= uiY + 57)
			{
				ModAutoHeal.autoPeanHpPercent = pcts[p];
				ModConfig.SaveConfig();
				SoundMn.gI().buttonClick();
				return true;
			}
		}

		// 4. Tự thu đậu
		if (px >= item1X + halfW - 40 && px <= item1X + halfW + 2 && py >= uiY + 57 && py <= uiY + 77)
		{
			ModAutoHeal.autoHarvestPea = !ModAutoHeal.autoHarvestPea;
			ModConfig.SaveConfig();
			GameScr.info1.addInfo("Tự thu đậu: " + (ModAutoHeal.autoHarvestPea ? "BẬT" : "TẮT"), 0);
			SoundMn.gI().buttonClick();
			return true;
		}

		// 5. Cho đậu bang
		if (px >= item2X + halfW - 40 && px <= item2X + halfW + 2 && py >= uiY + 57 && py <= uiY + 77)
		{
			ModAutoHeal.autoDonateClan = !ModAutoHeal.autoDonateClan;
			ModConfig.SaveConfig();
			GameScr.info1.addInfo("Cho đậu bang: " + (ModAutoHeal.autoDonateClan ? "BẬT" : "TẮT"), 0);
			SoundMn.gI().buttonClick();
			return true;
		}

		// 6. Cho đệ khi xin
		if (px >= item1X + halfW - 40 && px <= item1X + halfW + 2 && py >= uiY + 76 && py <= uiY + 96)
		{
			ModAutoHeal.autoFeedPetOnAsk = !ModAutoHeal.autoFeedPetOnAsk;
			ModConfig.SaveConfig();
			GameScr.info1.addInfo("Ăn đậu cho đệ tử: " + (ModAutoHeal.autoFeedPetOnAsk ? "BẬT" : "TẮT"), 0);
			SoundMn.gI().buttonClick();
			return true;
		}

		// 7. Nút Thu Đậu Ngay
		int actBtnW = (uiW - 12) / 2;
		if (px >= uiX + 4 && px <= uiX + 4 + actBtnW && py >= uiY + 97 && py <= uiY + 117)
		{
			ModAutoHeal.HarvestMagicTreeNow();
			return true;
		}

		// 8. Nút Cho Đậu Bang Ngay
		if (px >= uiX + 8 + actBtnW && px <= uiX + 8 + actBtnW * 2 && py >= uiY + 97 && py <= uiY + 117)
		{
			ModAutoHeal.DonateClanNow();
			return true;
		}

		return false;
	}
}
