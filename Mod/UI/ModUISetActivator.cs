using System;

public static class ModUISetActivator
{
	public static void Paint(int uiX, int uiY, int uiW, int uiH, mGraphics g)
	{
		// Hàng 1: Auto Úp Set KH & Tự Bán Khi Full Túi
		mFont.tahoma_7_grey.drawString(g, "Auto Úp Set KH:", uiX + 6, uiY + 7, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 90, uiY + 4, 42, 16, ModSetActivator.isActive ? "BẬT" : "TẮT", ModSetActivator.isActive, g);

		int rSellW = 42;
		int rSellX = uiX + uiW - rSellW - 6;
		mFont.tahoma_7_grey.drawString(g, "Bán Full:", rSellX - 50, uiY + 7, mFont.LEFT);
		ModUI.PaintNativeButton(rSellX, uiY + 4, rSellW, 16, ModSetActivator.autoSellJunk ? "BẬT" : "TẮT", ModSetActivator.autoSellJunk, g);

		// Khung thông tin bãi úp & bộ lọc
		int listY = uiY + 24;
		int listW = uiW - 10;
		int listH = 162;
		GameCanvas.paintz.paintFrameSimple(uiX + 5, listY, listW, listH, g);
		g.setColor(15196114);
		g.fillRect(uiX + 7, listY + 2, listW - 4, listH - 4);

		// Tiêu đề khung
		mFont.tahoma_7_green2.drawString(g, "BÃI ÚP & BỘ LỌC TRANG BỊ:", uiX + 12, listY + 5, mFont.LEFT);

		// Bãi úp đã lưu
		string spotText = (ModSetActivator.savedFarmMapId > 0)
			? (ModNextMap.GetMapName(ModSetActivator.savedFarmMapId) + " [K." + ModSetActivator.savedFarmZoneId + "] (" + ModSetActivator.savedFarmX + ", " + ModSetActivator.savedFarmY + ")")
			: "(Chưa lưu - Bấm 'Lưu Bãi' để lưu)";
		mFont.tahoma_7_grey.drawString(g, "- Bãi úp: ", uiX + 12, listY + 21, mFont.LEFT);
		(ModSetActivator.savedFarmMapId > 0 ? mFont.tahoma_7_green2 : mFont.tahoma_7_grey).drawString(g, spotText, uiX + 58, listY + 21, mFont.LEFT);

		// Lọc Đồ Sao & Hút Tức Thì & Hiện ID Item
		mFont.tahoma_7_grey.drawString(g, "- Lọc sao: ", uiX + 12, listY + 40, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 62, listY + 37, 64, 16, ModSetActivator.GetMinStarText(), ModSetActivator.minStarToKeep > 0, g);

		mFont.tahoma_7_grey.drawString(g, "Hút:", uiX + 132, listY + 40, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 154, listY + 37, 38, 16, ModDropRate.isInstantPick ? "BẬT" : "TẮT", ModDropRate.isInstantPick, g);

		int rIdW = 60;
		int rIdX = uiX + uiW - rIdW - 10;
		ModUI.PaintNativeButton(rIdX, listY + 37, rIdW, 16, ModSetActivator.showItemId ? "ID: BẬT" : "ID: TẮT", ModSetActivator.showItemId, g);

		// Hàng cài đặt Bùa Bà Hạt Mít
		mFont.tahoma_7_grey.drawString(g, "- Bùa Mít: ", uiX + 12, listY + 60, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 62, listY + 57, 64, 16, ModAutoBuyBua.buaTypeNames[ModAutoBuyBua.selectedBuaType], true, g);
		ModUI.PaintNativeButton(uiX + 130, listY + 57, 48, 16, ModAutoBuyBua.packageNames[ModAutoBuyBua.selectedPackage], false, g);

		int rBuaAutoW = 70;
		int rBuaAutoX = uiX + uiW - rBuaAutoW - 10;
		ModUI.PaintNativeButton(rBuaAutoX, listY + 57, rBuaAutoW, 16, ModAutoBuyBua.isAutoRebuy ? "Auto: BẬT" : "Auto: TẮT", ModAutoBuyBua.isAutoRebuy, g);

		// Thống kê rơi đồ thực chiến
		string stat1 = "- Diệt: " + ModDropRate.totalMobsKilled + " (" + ModDropRate.GetMobsPerMinute() + "/p) | Rơi: " + ModDropRate.totalItemsDropped + " (" + ModDropRate.GetDropRatePercent() + "%)";
		mFont.tahoma_7_grey.drawString(g, stat1, uiX + 12, listY + 81, mFont.LEFT);

		string stat2 = "- Đồ KH: " + ModDropRate.totalSetKHCount + " món | Đồ Sao: " + ModDropRate.totalStarCount + " món";
		mFont.tahoma_7_green2.drawString(g, stat2, uiX + 12, listY + 101, mFont.LEFT);

		int rTkW = 52;
		int rTkX = uiX + uiW - rTkW - 10;
		ModUI.PaintNativeButton(rTkX, listY + 98, rTkW, 16, "Reset TK", false, g);

		// Trạng thái vận hành
		string stText = ModAutoBuyBua.isBusy ? ModAutoBuyBua.GetStatusText() : ModSetActivator.GetStatusText();
		mFont.tahoma_7_grey.drawString(g, "- Trạng thái: ", uiX + 12, listY + 126, mFont.LEFT);
		mFont.tahoma_7_green2.drawString(g, stText, uiX + 74, listY + 126, mFont.LEFT);

		// Hàng 5 nút chức năng dưới cùng
		int btnY = uiY + 190;
		int fBtnW = (uiW - 28) / 5;
		string[] bottomLabels = new string[] { "Lưu Bãi", "Bán Urôn", "Mua Bùa", "Về Bãi", "Xóa Bãi" };
		for (int b = 0; b < 5; b++)
		{
			int bx = uiX + 5 + b * (fBtnW + 3);
			ModUI.PaintNativeButton(bx, btnY, fBtnW, 18, bottomLabels[b], false, g);
		}
	}

	public static bool HandleTap(int px, int py, int uiX, int uiY, int uiW, int uiH)
	{
		int listY = uiY + 24;
		int btnY = uiY + 190;

		// 1. Nút Bật/Tắt Auto Úp Set KH
		if (px >= uiX + 90 && px <= uiX + 132 && py >= uiY + 3 && py <= uiY + 21)
		{
			ModSetActivator.ToggleActive();
			return true;
		}

		// 2. Nút Bật/Tắt Bán Khi Full Túi
		int rSellW = 42;
		int rSellX = uiX + uiW - rSellW - 6;
		if (px >= rSellX && px <= rSellX + rSellW && py >= uiY + 3 && py <= uiY + 21)
		{
			ModSetActivator.autoSellJunk = !ModSetActivator.autoSellJunk;
			ModConfig.SaveConfig();
			GameScr.info1.addInfo("Tự bán khi full túi: " + (ModSetActivator.autoSellJunk ? "BẬT" : "TẮT"), 0);
			SoundMn.gI().buttonClick();
			return true;
		}

		// 3. Nút Đổi Bộ Lọc Đồ Sao
		if (px >= uiX + 62 && px <= uiX + 126 && py >= listY + 36 && py <= listY + 54)
		{
			ModSetActivator.CycleMinStar();
			return true;
		}

		// 3b. Nút Bật/Tắt Hút Đồ Tức Thì (Instant Pick)
		if (px >= uiX + 154 && px <= uiX + 192 && py >= listY + 36 && py <= listY + 54)
		{
			ModDropRate.isInstantPick = !ModDropRate.isInstantPick;
			ModConfig.SaveConfig();
			GameScr.info1.addInfo("Hút Đồ Tức Thì: " + (ModDropRate.isInstantPick ? "BẬT" : "TẮT"), 0);
			SoundMn.gI().buttonClick();
			return true;
		}

		// 4. Nút Bật/Tắt Hiện ID Item
		int rIdW = 60;
		int rIdX = uiX + uiW - rIdW - 10;
		if (px >= rIdX && px <= rIdX + rIdW && py >= listY + 36 && py <= listY + 54)
		{
			ModSetActivator.showItemId = !ModSetActivator.showItemId;
			ModConfig.SaveConfig();
			GameScr.info1.addInfo("Hiện Tên & ID Item: " + (ModSetActivator.showItemId ? "BẬT" : "TẮT"), 0);
			SoundMn.gI().buttonClick();
			return true;
		}

		// 4b. Nút Chọn Loại Bùa
		if (px >= uiX + 62 && px <= uiX + 126 && py >= listY + 56 && py <= listY + 74)
		{
			ModAutoBuyBua.CycleBuaType();
			return true;
		}

		// 4c. Nút Chọn Gói Thời Hạn Bùa
		if (px >= uiX + 130 && px <= uiX + 178 && py >= listY + 56 && py <= listY + 74)
		{
			ModAutoBuyBua.CyclePackage();
			return true;
		}

		// 4d. Nút Bật/Tắt Tự Động Mua Lại Bùa
		int rBuaAutoW = 70;
		int rBuaAutoX = uiX + uiW - rBuaAutoW - 10;
		if (px >= rBuaAutoX && px <= rBuaAutoX + rBuaAutoW && py >= listY + 56 && py <= listY + 74)
		{
			ModAutoBuyBua.ToggleAutoRebuy();
			return true;
		}

		// 4e. Nút Reset Thống Kê Rơi Đồ
		int rTkW = 52;
		int rTkX = uiX + uiW - rTkW - 10;
		if (px >= rTkX && px <= rTkX + rTkW && py >= listY + 97 && py <= listY + 115)
		{
			ModDropRate.ResetStats();
			return true;
		}

		// 5 nút chức năng dưới cùng
		int fBtnW = (uiW - 28) / 5;
		if (py >= btnY && py <= btnY + 20)
		{
			for (int b = 0; b < 5; b++)
			{
				int bx = uiX + 5 + b * (fBtnW + 3);
				if (px >= bx && px <= bx + fBtnW)
				{
					if (b == 0) ModSetActivator.SaveCurrentFarmPosition();
					else if (b == 1) ModSetActivator.StartGoSellJunkNow();
					else if (b == 2) ModAutoBuyBua.StartBuyBuaNow();
					else if (b == 3)
					{
						if (ModSetActivator.savedFarmMapId > 0)
						{
							ModSetActivator.currentState = ModSetActivator.SetActivatorState.ReturningToFarm;
							ModNextMap.StartNextMap(ModSetActivator.savedFarmMapId);
							GameScr.info1.addInfo("Đang di chuyển về bãi úp...", 0);
							SoundMn.gI().buttonClick();
						}
						else
						{
							GameScr.info1.addInfo("Chưa lưu vị trí bãi úp!", 0);
						}
					}
					else if (b == 4) ModSetActivator.ResetPosition();
					return true;
				}
			}
		}

		return false;
	}
}
