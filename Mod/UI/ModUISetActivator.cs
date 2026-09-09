using System;

public static class ModUISetActivator
{
	public static void Paint(int uiX, int uiY, int uiW, int uiH, mGraphics g)
	{
		// Hàng 1: Auto Úp Set KH & Tự Bán Khi Full Túi
		mFont.tahoma_7b_dark.drawString(g, "Auto Úp Set KH:", uiX + 8, uiY + 12, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 96, uiY + 8, 46, 18, ModSetActivator.isActive ? "BẬT" : "TẮT", ModSetActivator.isActive, g);

		int rSellW = 46;
		int rSellX = uiX + uiW - rSellW - 6;
		mFont.tahoma_7b_dark.drawString(g, "Bán Full:", rSellX - 54, uiY + 12, mFont.LEFT);
		ModUI.PaintNativeButton(rSellX, uiY + 8, rSellW, 18, ModSetActivator.autoSellJunk ? "BẬT" : "TẮT", ModSetActivator.autoSellJunk, g);

		// Khung thông tin bãi úp & bộ lọc
		int listY = uiY + 32;
		int listW = uiW - 16;
		int listH = 152;
		GameCanvas.paintz.paintFrameSimple(uiX + 8, listY, listW, listH, g);
		g.setColor(15196114);
		g.fillRect(uiX + 10, listY + 2, listW - 4, listH - 4);

		// Tiêu đề khung
		mFont.tahoma_7b_dark.drawString(g, "BÃI ÚP & BỘ LỌC TRANG BỊ:", uiX + 16, listY + 5, mFont.LEFT);

		// Bãi úp đã lưu
		string spotText = (ModSetActivator.savedFarmMapId > 0)
			? (ModNextMap.GetMapName(ModSetActivator.savedFarmMapId) + " [K." + ModSetActivator.savedFarmZoneId + "] (" + ModSetActivator.savedFarmX + ", " + ModSetActivator.savedFarmY + ")")
			: "(Chưa lưu - Bấm 'Lưu Bãi' để lưu)";
		mFont.tahoma_7b_dark.drawString(g, "- Bãi úp: ", uiX + 16, listY + 20, mFont.LEFT);
		(ModSetActivator.savedFarmMapId > 0 ? mFont.tahoma_7b_green2 : mFont.tahoma_7_grey).drawString(g, spotText, uiX + 66, listY + 20, mFont.LEFT);

		// Lọc Đồ Sao & Hút Tức Thì & Hiện ID Item
		mFont.tahoma_7b_dark.drawString(g, "- Lọc sao: ", uiX + 16, listY + 39, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 68, listY + 36, 70, 17, ModSetActivator.GetMinStarText(), ModSetActivator.minStarToKeep > 0, g);

		mFont.tahoma_7b_dark.drawString(g, "Hút:", uiX + 144, listY + 39, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 168, listY + 36, 42, 17, ModDropRate.isInstantPick ? "BẬT" : "TẮT", ModDropRate.isInstantPick, g);

		int rIdW = 66;
		int rIdX = uiX + uiW - rIdW - 14;
		ModUI.PaintNativeButton(rIdX, listY + 36, rIdW, 17, ModSetActivator.showItemId ? "ID: BẬT" : "ID: TẮT", ModSetActivator.showItemId, g);

		// Hàng cài đặt Bùa Bà Hạt Mít
		mFont.tahoma_7b_dark.drawString(g, "- Bùa Mít: ", uiX + 16, listY + 59, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 68, listY + 56, 68, 17, ModAutoBuyBua.buaTypeNames[ModAutoBuyBua.selectedBuaType], true, g);
		ModUI.PaintNativeButton(uiX + 140, listY + 56, 52, 17, ModAutoBuyBua.packageNames[ModAutoBuyBua.selectedPackage], false, g);

		int rBuaAutoW = 76;
		int rBuaAutoX = uiX + uiW - rBuaAutoW - 14;
		ModUI.PaintNativeButton(rBuaAutoX, listY + 56, rBuaAutoW, 17, ModAutoBuyBua.isAutoRebuy ? "Auto: BẬT" : "Auto: TẮT", ModAutoBuyBua.isAutoRebuy, g);

		// Thống kê rơi đồ thực chiến (Real-Time Drop Tracker)
		string stat1 = "- Diệt: " + ModDropRate.totalMobsKilled + " (" + ModDropRate.GetMobsPerMinute() + "/p) | Rơi: " + ModDropRate.totalItemsDropped + " (" + ModDropRate.GetDropRatePercent() + "%)";
		mFont.tahoma_7b_dark.drawString(g, stat1, uiX + 16, listY + 80, mFont.LEFT);

		string stat2 = "- Đồ KH: " + ModDropRate.totalSetKHCount + " món | Đồ Sao: " + ModDropRate.totalStarCount + " món";
		mFont.tahoma_7b_green2.drawString(g, stat2, uiX + 16, listY + 100, mFont.LEFT);

		int rTkW = 58;
		int rTkX = uiX + uiW - rTkW - 14;
		ModUI.PaintNativeButton(rTkX, listY + 97, rTkW, 17, "Reset TK", false, g);

		// Trạng thái vận hành
		string stText = ModAutoBuyBua.isBusy ? ModAutoBuyBua.GetStatusText() : ModSetActivator.GetStatusText();
		mFont.tahoma_7b_dark.drawString(g, "- Trạng thái: ", uiX + 16, listY + 124, mFont.LEFT);
		mFont.tahoma_7b_green2.drawString(g, stText, uiX + 80, listY + 124, mFont.LEFT);

		// Hàng 5 nút chức năng dưới cùng
		int btnY = uiY + 190;
		int fBtnW = (uiW - 32) / 5;
		string[] bottomLabels = new string[] { "Lưu Bãi", "Bán Urôn", "Mua Bùa", "Về Bãi", "Xóa Bãi" };
		for (int b = 0; b < 5; b++)
		{
			int bx = uiX + 8 + b * (fBtnW + 3);
			ModUI.PaintNativeButton(bx, btnY, fBtnW, 22, bottomLabels[b], false, g);
		}
	}

	public static bool HandleTap(int px, int py, int uiX, int uiY, int uiW, int uiH)
	{
		int listY = uiY + 32;
		int btnY = uiY + 190;

		// 1. Nút Bật/Tắt Auto Úp Set KH
		if (px >= uiX + 96 && px <= uiX + 142 && py >= uiY + 8 && py <= uiY + 26)
		{
			ModSetActivator.ToggleActive();
			return true;
		}

		// 2. Nút Bật/Tắt Bán Khi Full Túi
		int rSellW = 46;
		int rSellX = uiX + uiW - rSellW - 6;
		if (px >= rSellX && px <= rSellX + rSellW && py >= uiY + 8 && py <= uiY + 26)
		{
			ModSetActivator.autoSellJunk = !ModSetActivator.autoSellJunk;
			ModConfig.SaveConfig();
			GameScr.info1.addInfo("Tự bán khi full túi: " + (ModSetActivator.autoSellJunk ? "BẬT" : "TẮT"), 0);
			SoundMn.gI().buttonClick();
			return true;
		}

		// 3. Nút Đổi Bộ Lọc Đồ Sao
		if (px >= uiX + 68 && px <= uiX + 138 && py >= listY + 36 && py <= listY + 53)
		{
			ModSetActivator.CycleMinStar();
			return true;
		}

		// 3b. Nút Bật/Tắt Hút Đồ Tức Thì (Instant Pick)
		if (px >= uiX + 168 && px <= uiX + 210 && py >= listY + 36 && py <= listY + 53)
		{
			ModDropRate.isInstantPick = !ModDropRate.isInstantPick;
			ModConfig.SaveConfig();
			GameScr.info1.addInfo("Hút Đồ Tức Thì: " + (ModDropRate.isInstantPick ? "BẬT" : "TẮT"), 0);
			SoundMn.gI().buttonClick();
			return true;
		}

		// 4. Nút Bật/Tắt Hiện ID Item
		int rIdW = 66;
		int rIdX = uiX + uiW - rIdW - 14;
		if (px >= rIdX && px <= rIdX + rIdW && py >= listY + 36 && py <= listY + 53)
		{
			ModSetActivator.showItemId = !ModSetActivator.showItemId;
			ModConfig.SaveConfig();
			GameScr.info1.addInfo("Hiện Tên & ID Item: " + (ModSetActivator.showItemId ? "BẬT" : "TẮT"), 0);
			SoundMn.gI().buttonClick();
			return true;
		}

		// 4b. Nút Chọn Loại Bùa
		if (px >= uiX + 68 && px <= uiX + 136 && py >= listY + 56 && py <= listY + 73)
		{
			ModAutoBuyBua.CycleBuaType();
			return true;
		}

		// 4c. Nút Chọn Gói Thời Hạn Bùa
		if (px >= uiX + 140 && px <= uiX + 192 && py >= listY + 56 && py <= listY + 73)
		{
			ModAutoBuyBua.CyclePackage();
			return true;
		}

		// 4d. Nút Bật/Tắt Tự Động Mua Lại Bùa
		int rBuaAutoW = 76;
		int rBuaAutoX = uiX + uiW - rBuaAutoW - 14;
		if (px >= rBuaAutoX && px <= rBuaAutoX + rBuaAutoW && py >= listY + 56 && py <= listY + 73)
		{
			ModAutoBuyBua.ToggleAutoRebuy();
			return true;
		}

		// 4e. Nút Reset Thống Kê Rơi Đồ
		int rTkW = 58;
		int rTkX = uiX + uiW - rTkW - 14;
		if (px >= rTkX && px <= rTkX + rTkW && py >= listY + 97 && py <= listY + 114)
		{
			ModDropRate.ResetStats();
			return true;
		}

		// 5 nút chức năng dưới cùng
		int fBtnW = (uiW - 32) / 5;
		if (py >= btnY && py <= btnY + 22)
		{
			for (int b = 0; b < 5; b++)
			{
				int bx = uiX + 8 + b * (fBtnW + 3);
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
