using System;

public static class ModUISetActivator
{
	public static void Paint(int uiX, int uiY, int uiW, int uiH, mGraphics g)
	{
		int halfW = (uiW - 14) / 2;
		int item1X = uiX + 4;
		int item2X = uiX + 8 + halfW;

		// Hàng 1: Auto Úp Set KH & Tự Bán Khi Full Túi
		mFont.tahoma_7_grey.drawString(g, "Auto Úp Set KH:", item1X + 2, uiY + 7, mFont.LEFT);
		ModUI.PaintNativeButton(item1X + halfW - 38, uiY + 4, 38, 17, ModSetActivator.isActive ? "BẬT" : "TẮT", ModSetActivator.isActive, g);

		mFont.tahoma_7_grey.drawString(g, "Bán Full:", item2X + 2, uiY + 7, mFont.LEFT);
		ModUI.PaintNativeButton(item2X + halfW - 38, uiY + 4, 38, 17, ModSetActivator.autoSellJunk ? "BẬT" : "TẮT", ModSetActivator.autoSellJunk, g);

		int footerH = 18;
		int footerY = uiY + uiH - footerH - 4;

		// Khung thông tin bãi úp & bộ lọc co dãn động
		int listY = uiY + 24;
		int listW = uiW - 8;
		int listH = (footerY - 4) - listY;
		if (listH < 40) listH = 40;

		GameCanvas.paintz.paintFrameSimple(uiX + 4, listY, listW, listH, g);
		g.setColor(15196114);
		g.fillRect(uiX + 6, listY + 2, listW - 4, listH - 4);

		// Tiêu đề khung
		mFont.tahoma_7_green2.drawString(g, "BÃI ÚP & BỘ LỌC TRANG BỊ:", uiX + 10, listY + 4, mFont.LEFT);

		// Bãi úp đã lưu
		string spotText = (ModSetActivator.savedFarmMapId > 0)
			? (ModNextMap.GetMapName(ModSetActivator.savedFarmMapId) + " [K." + ModSetActivator.savedFarmZoneId + "] (" + ModSetActivator.savedFarmX + ", " + ModSetActivator.savedFarmY + ")")
			: "(Chưa lưu - Bấm 'Lưu Bãi' để lưu)";
		mFont.tahoma_7_grey.drawString(g, "- Bãi úp: ", uiX + 10, listY + 18, mFont.LEFT);
		(ModSetActivator.savedFarmMapId > 0 ? mFont.tahoma_7_green2 : mFont.tahoma_7_grey).drawString(g, spotText, uiX + 56, listY + 18, mFont.LEFT);

		// 3 nút lọc: Lọc Đồ Sao | Hút Tức Thì | Hiện ID Item
		int w3 = (listW - 16) / 3;
		ModUI.PaintNativeButton(uiX + 8, listY + 34, w3, 16, ModSetActivator.GetMinStarText(), ModSetActivator.minStarToKeep > 0, g);
		ModUI.PaintNativeButton(uiX + 11 + w3, listY + 34, w3, 16, ModDropRate.isInstantPick ? "Hút: BẬT" : "Hút: TẮT", ModDropRate.isInstantPick, g);
		ModUI.PaintNativeButton(uiX + 14 + w3 * 2, listY + 34, w3, 16, ModSetActivator.showItemId ? "ID: BẬT" : "ID: TẮT", ModSetActivator.showItemId, g);

		// Hàng cài đặt Bùa Bà Hạt Mít
		ModUI.PaintNativeButton(uiX + 8, listY + 52, w3, 16, ModAutoBuyBua.buaTypeNames[ModAutoBuyBua.selectedBuaType], true, g);
		ModUI.PaintNativeButton(uiX + 11 + w3, listY + 52, w3, 16, ModAutoBuyBua.packageNames[ModAutoBuyBua.selectedPackage], false, g);
		ModUI.PaintNativeButton(uiX + 14 + w3 * 2, listY + 52, w3, 16, ModAutoBuyBua.isAutoRebuy ? "Bùa: Auto" : "Bùa: Tắt", ModAutoBuyBua.isAutoRebuy, g);

		// Thống kê rơi đồ thực chiến
		string stat1 = "- Diệt: " + ModDropRate.totalMobsKilled + " (" + ModDropRate.GetMobsPerMinute() + "/p) | Rơi: " + ModDropRate.totalItemsDropped;
		mFont.tahoma_7_grey.drawString(g, stat1, uiX + 10, listY + 70, mFont.LEFT);

		string stat2 = "- Đồ KH: " + ModDropRate.totalSetKHCount + " | Sao: " + ModDropRate.totalStarCount;
		mFont.tahoma_7_green2.drawString(g, stat2, uiX + 10, listY + 86, mFont.LEFT);

		ModUI.PaintNativeButton(uiX + listW - 48, listY + 84, 44, 15, "Reset TK", false, g);

		// Trạng thái vận hành
		string stText = ModAutoBuyBua.isBusy ? ModAutoBuyBua.GetStatusText() : ModSetActivator.GetStatusText();
		mFont.tahoma_7_grey.drawString(g, "- Trạng thái: ", uiX + 10, listY + 102, mFont.LEFT);
		mFont.tahoma_7_green2.drawString(g, stText, uiX + 68, listY + 102, mFont.LEFT);

		// Hàng 5 nút chức năng dưới cùng ở Footer
		int fBtnW = (uiW - 24) / 5;
		string[] bottomLabels = new string[] { "Lưu Bãi", "Bán Rác", "Mua Bùa", "Về Bãi", "Xóa Bãi" };
		for (int b = 0; b < 5; b++)
		{
			int bx = uiX + 4 + b * (fBtnW + 4);
			ModUI.PaintNativeButton(bx, footerY, fBtnW, footerH, bottomLabels[b], false, g);
		}
	}

	public static bool HandleTap(int px, int py, int uiX, int uiY, int uiW, int uiH)
	{
		int halfW = (uiW - 14) / 2;
		int item1X = uiX + 4;
		int item2X = uiX + 8 + halfW;
		int footerH = 18;
		int footerY = uiY + uiH - footerH - 4;
		int listY = uiY + 24;
		int listW = uiW - 8;

		// 1. Nút Bật/Tắt Auto Úp Set KH
		if (px >= item1X + halfW - 40 && px <= item1X + halfW + 2 && py >= uiY + 2 && py <= uiY + 22)
		{
			ModSetActivator.ToggleActive();
			return true;
		}

		// 2. Nút Bật/Tắt Bán Khi Full Túi
		if (px >= item2X + halfW - 40 && px <= item2X + halfW + 2 && py >= uiY + 2 && py <= uiY + 22)
		{
			ModSetActivator.autoSellJunk = !ModSetActivator.autoSellJunk;
			ModConfig.SaveConfig();
			GameScr.info1.addInfo("Tự bán khi full túi: " + (ModSetActivator.autoSellJunk ? "BẬT" : "TẮT"), 0);
			SoundMn.gI().buttonClick();
			return true;
		}

		// 3. 3 nút lọc hàng 1
		int w3 = (listW - 16) / 3;
		if (px >= uiX + 8 && px <= uiX + 8 + w3 && py >= listY + 33 && py <= listY + 51)
		{
			ModSetActivator.CycleMinStar();
			ModConfig.SaveConfig();
			SoundMn.gI().buttonClick();
			return true;
		}

		if (px >= uiX + 11 + w3 && px <= uiX + 11 + w3 * 2 && py >= listY + 33 && py <= listY + 51)
		{
			ModDropRate.isInstantPick = !ModDropRate.isInstantPick;
			ModConfig.SaveConfig();
			GameScr.info1.addInfo("Hút đồ tức thì: " + (ModDropRate.isInstantPick ? "BẬT" : "TẮT"), 0);
			SoundMn.gI().buttonClick();
			return true;
		}

		if (px >= uiX + 14 + w3 * 2 && px <= uiX + 14 + w3 * 3 && py >= listY + 33 && py <= listY + 51)
		{
			ModSetActivator.showItemId = !ModSetActivator.showItemId;
			ModConfig.SaveConfig();
			GameScr.info1.addInfo("Hiện ID trang bị: " + (ModSetActivator.showItemId ? "BẬT" : "TẮT"), 0);
			SoundMn.gI().buttonClick();
			return true;
		}

		// 4. 3 nút Bùa Bà Hạt Mít
		if (px >= uiX + 8 && px <= uiX + 8 + w3 && py >= listY + 51 && py <= listY + 69)
		{
			ModAutoBuyBua.CycleBuaType();
			ModConfig.SaveConfig();
			SoundMn.gI().buttonClick();
			return true;
		}

		if (px >= uiX + 11 + w3 && px <= uiX + 11 + w3 * 2 && py >= listY + 51 && py <= listY + 69)
		{
			ModAutoBuyBua.CyclePackage();
			ModConfig.SaveConfig();
			SoundMn.gI().buttonClick();
			return true;
		}

		if (px >= uiX + 14 + w3 * 2 && px <= uiX + 14 + w3 * 3 && py >= listY + 51 && py <= listY + 69)
		{
			ModAutoBuyBua.isAutoRebuy = !ModAutoBuyBua.isAutoRebuy;
			ModConfig.SaveConfig();
			GameScr.info1.addInfo("Auto gia hạn bùa: " + (ModAutoBuyBua.isAutoRebuy ? "BẬT" : "TẮT"), 0);
			SoundMn.gI().buttonClick();
			return true;
		}

		// 5. Nút Reset TK
		if (px >= uiX + listW - 50 && px <= uiX + listW && py >= listY + 83 && py <= listY + 100)
		{
			ModDropRate.ResetStats();
			GameScr.info1.addInfo("Đã reset thống kê rơi đồ!", 0);
			SoundMn.gI().buttonClick();
			return true;
		}

		// 6. Hàng 5 nút chức năng dưới cùng ở Footer
		int fBtnW = (uiW - 24) / 5;
		if (py >= footerY - 1 && py <= footerY + footerH + 1)
		{
			for (int b = 0; b < 5; b++)
			{
				int bx = uiX + 4 + b * (fBtnW + 4);
				if (px >= bx && px <= bx + fBtnW)
				{
					switch (b)
					{
						case 0:
							ModSetActivator.SaveCurrentFarmPosition();
							break;
						case 1:
							ModSetActivator.StartGoSellJunkNow();
							break;
						case 2:
							ModAutoBuyBua.StartBuyBuaNow();
							break;
						case 3:
							if (ModSetActivator.savedFarmMapId > 0)
							{
								ModSetActivator.currentState = ModSetActivator.SetActivatorState.ReturningToFarm;
								ModNextMap.StartNextMap(ModSetActivator.savedFarmMapId);
								ModUI.uiCustomOpen = false;
							}
							else
							{
								GameScr.info1.addInfo("Chưa lưu bãi úp!", 0);
							}
							break;
						case 4:
							ModSetActivator.ResetPosition();
							break;
					}
					return true;
				}
			}
		}

		return false;
	}
}
