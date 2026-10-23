using System;

/// <summary>
/// CLASS ModUIAutoHeal: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public static class ModUIAutoHeal
{
	private static readonly int[] pcts = new int[] { 20, 30, 50, 70 };

	/// <summary>
	/// Thực hiện vẽ toàn bộ đối tượng và các lớp hiển thị lên canvas mGraphics.
	/// </summary>
	public static void Paint(int uiX, int uiY, int uiW, int uiH, mGraphics g)
	{
		int halfW = (uiW - 14) / 2;
		int item1X = uiX + 4;
		int item2X = uiX + 8 + halfW;

		// Hàng 1: Tự dùng đậu & Khóa HP/MP
		mFont.tahoma_7_grey.drawString(g, "Tự dùng đậu:", item1X + 2, uiY + 6, mFont.LEFT);
		ModUI.PaintNativeButton(item1X + halfW - 38, uiY + 3, 38, 16, ModAutoHeal.autoPean ? "BẬT" : "TẮT", ModAutoHeal.autoPean, g);

		mFont.tahoma_7_grey.drawString(g, "Khóa HP/MP:", item2X + 2, uiY + 6, mFont.LEFT);
		ModUI.PaintNativeButton(item2X + halfW - 38, uiY + 3, 38, 16, ModAutoHeal.lockHPMP ? "BẬT" : "TẮT", ModAutoHeal.lockHPMP, g);

		// Hàng 2: Ngưỡng HP & Ngưỡng KI riêng biệt
		int pStartX = uiX + 46;
		int pBtnW = (uiW - 50 - 12) / pcts.Length;
		mFont.tahoma_7_grey.drawString(g, "HP dưới:", uiX + 5, uiY + 24, mFont.LEFT);
		for (int p = 0; p < pcts.Length; p++)
		{
			int pbx = pStartX + p * (pBtnW + 3);
			bool isSel = (ModAutoHeal.autoPeanHpPercent == pcts[p]);
			ModUI.PaintNativeButton(pbx, uiY + 21, pBtnW, 16, "< " + pcts[p] + "%", isSel, g);
		}

		mFont.tahoma_7_grey.drawString(g, "KI dưới:", uiX + 5, uiY + 42, mFont.LEFT);
		for (int p = 0; p < pcts.Length; p++)
		{
			int pbx = pStartX + p * (pBtnW + 3);
			bool isSel = (ModAutoHeal.autoPeanMpPercent == pcts[p]);
			ModUI.PaintNativeButton(pbx, uiY + 39, pBtnW, 16, "< " + pcts[p] + "%", isSel, g);
		}

		// Hàng 3: Tự thu đậu & Cho đậu bang hội
		mFont.tahoma_7_grey.drawString(g, "Tự thu đậu:", item1X + 2, uiY + 60, mFont.LEFT);
		ModUI.PaintNativeButton(item1X + halfW - 38, uiY + 57, 38, 16, ModAutoHeal.autoHarvestPea ? "BẬT" : "TẮT", ModAutoHeal.autoHarvestPea, g);

		mFont.tahoma_7_grey.drawString(g, "Cho bang:", item2X + 2, uiY + 60, mFont.LEFT);
		ModUI.PaintNativeButton(item2X + halfW - 38, uiY + 57, 38, 16, ModAutoHeal.autoDonateClan ? "BẬT" : "TẮT", ModAutoHeal.autoDonateClan, g);

		// Hàng 4: Tự xin đậu bang & Cho đệ xin
		mFont.tahoma_7_grey.drawString(g, "Xin đậu bang:", item1X + 2, uiY + 78, mFont.LEFT);
		ModUI.PaintNativeButton(item1X + halfW - 38, uiY + 75, 38, 16, ModAutoHeal.autoRequestPea ? "BẬT" : "TẮT", ModAutoHeal.autoRequestPea, g);

		mFont.tahoma_7_grey.drawString(g, "Cho đệ xin:", item2X + 2, uiY + 78, mFont.LEFT);
		ModUI.PaintNativeButton(item2X + halfW - 38, uiY + 75, 38, 16, ModAutoFeedPet(), ModAutoHeal.autoFeedPetOnAsk, g);

		// Hàng 5: Tự hồi sinh, Ngưỡng Đệ & Lọc Cho Bang
		mFont.tahoma_7_grey.drawString(g, "Tự hồi sinh:", item1X + 2, uiY + 96, mFont.LEFT);
		ModUI.PaintNativeButton(item1X + halfW - 38, uiY + 93, 38, 16, ModAutoRevive.isEnabled ? "BẬT" : "TẮT", ModAutoRevive.isEnabled, g);

		ModUI.PaintNativeButton(item2X, uiY + 93, (halfW - 4) / 2, 16, "Đệ <" + ModAutoHeal.petEatBeanHpPercent + "%", ModAutoHeal.autoFeedPetOnAsk, g);
		ModUI.PaintNativeButton(item2X + (halfW - 4) / 2 + 4, uiY + 93, (halfW - 4) / 2, 16, string.IsNullOrEmpty(ModAutoHeal.donateTargetNamesRaw) ? "Lọc Bang" : "Đang Lọc", !string.IsNullOrEmpty(ModAutoHeal.donateTargetNamesRaw), g);

		int actBtnW = (uiW - 16) / 3;
		ModUI.PaintNativeButton(uiX + 4, uiY + 112, actBtnW, 16, "Thu Đậu", false, g);
		ModUI.PaintNativeButton(uiX + 8 + actBtnW, uiY + 112, actBtnW, 16, "Xin Đậu", false, g);
		ModUI.PaintNativeButton(uiX + 12 + actBtnW * 2, uiY + 112, actBtnW, 16, "Cho Đậu", false, g);

		// Ghi chú nhỏ ở đáy co dãn linh hoạt
		int boxX = uiX + 4;
		int boxY = uiY + 131;
		int boxW = uiW - 8;
		int boxH = uiH - 135;
		if (boxH < 24) boxH = 24;
		GameCanvas.paintz.paintFrameSimple(boxX, boxY, boxW, boxH, g);
		g.setColor(15196114);
		g.fillRect(boxX + 2, boxY + 2, boxW - 4, boxH - 4);
		int beanCount = ModAutoHeal.GetBeanCount();
		mFont.tahoma_7_grey.drawString(g, "* Đậu trong túi: " + beanCount + " hạt. Tự ăn đậu, xin/cho bang & đệ.", boxX + boxW / 2, boxY + (boxH - 10) / 2, mFont.CENTER);
	}

	private static string ModAutoFeedPet() => ModAutoHeal.autoFeedPetOnAsk ? "BẬT" : "TẮT";

	/// <summary>
	/// Xử lý nghiệp vụ logic chuyên trách cho HandleTap.
	/// </summary>
	public static bool HandleTap(int px, int py, int uiX, int uiY, int uiW, int uiH)
	{
		int halfW = (uiW - 14) / 2;
		int item1X = uiX + 4;
		int item2X = uiX + 8 + halfW;

		// 1. Tự dùng đậu
		if (px >= item1X + halfW - 40 && px <= item1X + halfW + 2 && py >= uiY + 2 && py <= uiY + 20)
		{
			ModAutoHeal.autoPean = !ModAutoHeal.autoPean;
			ModConfig.SaveConfig();
			SoundMn.gI().buttonClick();
			return true;
		}

		// 2. Khóa HP/MP
		if (px >= item2X + halfW - 40 && px <= item2X + halfW + 2 && py >= uiY + 2 && py <= uiY + 20)
		{
			ModAutoHeal.lockHPMP = !ModAutoHeal.lockHPMP;
			ModConfig.SaveConfig();
			SoundMn.gI().buttonClick();
			return true;
		}

		// 3a. Ngưỡng ăn đậu HP & KI
		int pStartX = uiX + 46;
		int pBtnW = (uiW - 50 - 12) / pcts.Length;
		for (int p = 0; p < pcts.Length; p++)
		{
			int pbx = pStartX + p * (pBtnW + 3);
			if (px >= pbx && px <= pbx + pBtnW && py >= uiY + 20 && py <= uiY + 38)
			{
				ModAutoHeal.autoPeanHpPercent = pcts[p];
				ModConfig.SaveConfig();
				SoundMn.gI().buttonClick();
				return true;
			}
			if (px >= pbx && px <= pbx + pBtnW && py >= uiY + 38 && py <= uiY + 56)
			{
				ModAutoHeal.autoPeanMpPercent = pcts[p];
				ModConfig.SaveConfig();
				SoundMn.gI().buttonClick();
				return true;
			}
		}

		// 4. Tự thu đậu
		if (px >= item1X + halfW - 40 && px <= item1X + halfW + 2 && py >= uiY + 56 && py <= uiY + 74)
		{
			ModAutoHeal.autoHarvestPea = !ModAutoHeal.autoHarvestPea;
			ModConfig.SaveConfig();
			GameScr.info1.addInfo("Tự thu đậu: " + (ModAutoHeal.autoHarvestPea ? "BẬT" : "TẮT"), 0);
			SoundMn.gI().buttonClick();
			return true;
		}

		// 5. Cho đậu bang
		if (px >= item2X + halfW - 40 && px <= item2X + halfW + 2 && py >= uiY + 56 && py <= uiY + 74)
		{
			ModAutoHeal.autoDonateClan = !ModAutoHeal.autoDonateClan;
			ModConfig.SaveConfig();
			GameScr.info1.addInfo("Cho đậu bang: " + (ModAutoHeal.autoDonateClan ? "BẬT" : "TẮT"), 0);
			SoundMn.gI().buttonClick();
			return true;
		}

		// 6a. Tự xin đậu bang
		if (px >= item1X + halfW - 40 && px <= item1X + halfW + 2 && py >= uiY + 74 && py <= uiY + 92)
		{
			ModAutoHeal.autoRequestPea = !ModAutoHeal.autoRequestPea;
			ModConfig.SaveConfig();
			GameScr.info1.addInfo("Tự xin đậu bang: " + (ModAutoHeal.autoRequestPea ? "BẬT" : "TẮT"), 0);
			SoundMn.gI().buttonClick();
			return true;
		}

		// 6b. Cho đệ khi xin
		if (px >= item2X + halfW - 40 && px <= item2X + halfW + 2 && py >= uiY + 74 && py <= uiY + 92)
		{
			ModAutoHeal.autoFeedPetOnAsk = !ModAutoHeal.autoFeedPetOnAsk;
			ModConfig.SaveConfig();
			GameScr.info1.addInfo("Ăn đậu cho đệ tử: " + (ModAutoHeal.autoFeedPetOnAsk ? "BẬT" : "TẮT"), 0);
			SoundMn.gI().buttonClick();
			return true;
		}

		// 7. Tự hồi sinh, Ngưỡng Đệ & Lọc Cho Bang
		if (py >= uiY + 92 && py <= uiY + 110)
		{
			if (px >= item1X + halfW - 40 && px <= item1X + halfW + 2)
			{
				ModAutoRevive.Toggle();
				SoundMn.gI().buttonClick();
				return true;
			}
			int subHalf = (halfW - 4) / 2;
			if (px >= item2X && px <= item2X + subHalf)
			{
				ModAutoHeal.CyclePetEatBeanPercent();
				GameScr.info1.addInfo("Ăn đậu khi HP/KI đệ < " + ModAutoHeal.petEatBeanHpPercent + "%", 0);
				return true;
			}
			if (px >= item2X + subHalf + 4 && px <= item2X + halfW)
			{
				ModUI.uiCustomOpen = false;
				ModAutoHeal.ShowInputDonateTarget();
				SoundMn.gI().buttonClick();
				return true;
			}
		}

		// 8. 3 Nút Thao tác nhanh: Thu Đậu / Xin Đậu / Cho Đậu
		int actBtnW = (uiW - 16) / 3;
		if (py >= uiY + 111 && py <= uiY + 129)
		{
			if (px >= uiX + 4 && px <= uiX + 4 + actBtnW)
			{
				ModAutoHeal.HarvestMagicTreeNow();
				return true;
			}
			if (px >= uiX + 8 + actBtnW && px <= uiX + 8 + actBtnW * 2)
			{
				ModAutoHeal.RequestClanPeaNow();
				return true;
			}
			if (px >= uiX + 12 + actBtnW * 2 && px <= uiX + 12 + actBtnW * 3)
			{
				ModAutoHeal.DonateClanNow();
				return true;
			}
		}

		return false;
	}
}
