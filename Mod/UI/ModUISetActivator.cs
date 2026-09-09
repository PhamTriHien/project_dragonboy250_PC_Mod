using System;

public static class ModUISetActivator
{
	public static void Paint(int uiX, int uiY, int uiW, int uiH, mGraphics g)
	{
		// Hàng 1: Auto Úp Set KH & Tự Bán Khi Full Túi
		mFont.tahoma_7b_white.drawString(g, "Auto Úp Set KH:", uiX + 18, uiY + 50, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 106, uiY + 46, 48, 18, ModSetActivator.isActive ? "BẬT" : "TẮT", ModSetActivator.isActive, g);

		mFont.tahoma_7b_white.drawString(g, "Bán Full Túi:", uiX + 165, uiY + 50, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 242, uiY + 46, 46, 18, ModSetActivator.autoSellJunk ? "BẬT" : "TẮT", ModSetActivator.autoSellJunk, g);

		// Khung thông tin bãi úp & bộ lọc
		int listY = uiY + 66;
		int listW = uiW - 36;
		int listH = 120;
		g.setColor(0x121212);
		g.fillRect(uiX + 18, listY, listW, listH);
		g.setColor(0x3c3c3c);
		g.drawRect(uiX + 18, listY, listW, listH);

		// Tiêu đề khung
		mFont.tahoma_7b_yellow.drawString(g, "BÃI ÚP & BỘ LỌC TRANG BỊ:", uiX + 26, listY + 5, mFont.LEFT);

		// Bãi úp đã lưu
		string spotText = (ModSetActivator.savedFarmMapId > 0)
			? (ModNextMap.GetMapName(ModSetActivator.savedFarmMapId) + " [K." + ModSetActivator.savedFarmZoneId + "] (" + ModSetActivator.savedFarmX + ", " + ModSetActivator.savedFarmY + ")")
			: "(Chưa lưu - Bấm 'Lưu Bãi' để lưu)";
		mFont.tahoma_7_white.drawString(g, "- Bãi úp: ", uiX + 26, listY + 19, mFont.LEFT);
		(ModSetActivator.savedFarmMapId > 0 ? mFont.tahoma_7b_green2 : mFont.tahoma_7_grey).drawString(g, spotText, uiX + 70, listY + 19, mFont.LEFT);

		// Lọc Đồ Sao & Hút Tức Thì & Hiện ID Item
		mFont.tahoma_7_white.drawString(g, "- Lọc sao: ", uiX + 26, listY + 34, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 78, listY + 31, 78, 16, ModSetActivator.GetMinStarText(), ModSetActivator.minStarToKeep > 0, g);

		mFont.tahoma_7_white.drawString(g, "Hút:", uiX + 162, listY + 34, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 188, listY + 31, 40, 16, ModDropRate.isInstantPick ? "BẬT" : "TẮT", ModDropRate.isInstantPick, g);

		ModUI.PaintNativeButton(uiX + 234, listY + 31, 46, 16, ModSetActivator.showItemId ? "ID: BẬT" : "ID: TẮT", ModSetActivator.showItemId, g);

		// Hàng cài đặt Bùa Bà Hạt Mít
		mFont.tahoma_7_white.drawString(g, "- Bùa Mít: ", uiX + 26, listY + 50, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 78, listY + 48, 72, 15, ModAutoBuyBua.buaTypeNames[ModAutoBuyBua.selectedBuaType], true, g);
		ModUI.PaintNativeButton(uiX + 154, listY + 48, 56, 15, ModAutoBuyBua.packageNames[ModAutoBuyBua.selectedPackage], false, g);
		ModUI.PaintNativeButton(uiX + 214, listY + 48, 102, 15, ModAutoBuyBua.isAutoRebuy ? "Auto: BẬT" : "Auto: TẮT", ModAutoBuyBua.isAutoRebuy, g);

		// Thống kê rơi đồ thực chiến (Real-Time Drop Tracker)
		string stat1 = "- Diệt: " + ModDropRate.totalMobsKilled + " (" + ModDropRate.GetMobsPerMinute() + "/p) | Rơi: " + ModDropRate.totalItemsDropped + " (" + ModDropRate.GetDropRatePercent() + "%)";
		mFont.tahoma_7b_yellow.drawString(g, stat1, uiX + 26, listY + 66, mFont.LEFT);

		string stat2 = "- Đồ KH: " + ModDropRate.totalSetKHCount + " món | Đồ Sao: " + ModDropRate.totalStarCount + " món";
		mFont.tahoma_7b_green2.drawString(g, stat2, uiX + 26, listY + 81, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 258, listY + 79, 58, 15, "Reset TK", false, g);

		// Trạng thái vận hành
		string stText = ModAutoBuyBua.isBusy ? ModAutoBuyBua.GetStatusText() : ModSetActivator.GetStatusText();
		mFont.tahoma_7_white.drawString(g, "- Trạng thái: ", uiX + 26, listY + 99, mFont.LEFT);
		mFont.tahoma_7b_white.drawString(g, stText, uiX + 88, listY + 99, mFont.LEFT);

		// Hàng 5 nút chức năng dưới cùng
		ModUI.PaintNativeButton(uiX + 18, uiY + 190, 56, 20, "Lưu Bãi", false, g);
		ModUI.PaintNativeButton(uiX + 78, uiY + 190, 58, 20, "Bán Urôn", false, g);
		ModUI.PaintNativeButton(uiX + 140, uiY + 190, 60, 20, "Mua Bùa", false, g);
		ModUI.PaintNativeButton(uiX + 204, uiY + 190, 56, 20, "Về Bãi", false, g);
		ModUI.PaintNativeButton(uiX + 264, uiY + 190, 54, 20, "Xóa Bãi", false, g);
	}

	public static bool HandleTap(int px, int py, int uiX, int uiY, int uiW, int uiH)
	{
		int listY = uiY + 66;

		// 1. Nút Bật/Tắt Auto Úp Set KH
		if (px >= uiX + 106 && px <= uiX + 154 && py >= uiY + 46 && py <= uiY + 64)
		{
			ModSetActivator.ToggleActive();
			return true;
		}

		// 2. Nút Bật/Tắt Bán Khi Full Túi
		if (px >= uiX + 242 && px <= uiX + 288 && py >= uiY + 46 && py <= uiY + 64)
		{
			ModSetActivator.autoSellJunk = !ModSetActivator.autoSellJunk;
			ModConfig.SaveConfig();
			GameScr.info1.addInfo("Tự bán khi full túi: " + (ModSetActivator.autoSellJunk ? "BẬT" : "TẮT"), 0);
			SoundMn.gI().buttonClick();
			return true;
		}

		// 3. Nút Đổi Bộ Lọc Đồ Sao
		if (px >= uiX + 78 && px <= uiX + 156 && py >= listY + 31 && py <= listY + 47)
		{
			ModSetActivator.CycleMinStar();
			return true;
		}

		// 3b. Nút Bật/Tắt Hút Đồ Tức Thì (Instant Pick)
		if (px >= uiX + 188 && px <= uiX + 228 && py >= listY + 31 && py <= listY + 47)
		{
			ModDropRate.isInstantPick = !ModDropRate.isInstantPick;
			ModConfig.SaveConfig();
			GameScr.info1.addInfo("Hút Đồ Tức Thì: " + (ModDropRate.isInstantPick ? "BẬT" : "TẮT"), 0);
			SoundMn.gI().buttonClick();
			return true;
		}

		// 4. Nút Bật/Tắt Hiện ID Item
		if (px >= uiX + 234 && px <= uiX + 280 && py >= listY + 31 && py <= listY + 47)
		{
			ModSetActivator.showItemId = !ModSetActivator.showItemId;
			ModConfig.SaveConfig();
			GameScr.info1.addInfo("Hiện Tên & ID Item: " + (ModSetActivator.showItemId ? "BẬT" : "TẮT"), 0);
			SoundMn.gI().buttonClick();
			return true;
		}

		// 4b. Nút Chọn Loại Bùa
		if (px >= uiX + 78 && px <= uiX + 150 && py >= listY + 48 && py <= listY + 63)
		{
			ModAutoBuyBua.CycleBuaType();
			return true;
		}

		// 4c. Nút Chọn Gói Thời Hạn Bùa
		if (px >= uiX + 154 && px <= uiX + 210 && py >= listY + 48 && py <= listY + 63)
		{
			ModAutoBuyBua.CyclePackage();
			return true;
		}

		// 4d. Nút Bật/Tắt Tự Động Mua Lại Bùa
		if (px >= uiX + 214 && px <= uiX + 316 && py >= listY + 48 && py <= listY + 63)
		{
			ModAutoBuyBua.ToggleAutoRebuy();
			return true;
		}

		// 4e. Nút Reset Thống Kê Rơi Đồ
		if (px >= uiX + 258 && px <= uiX + 316 && py >= listY + 79 && py <= listY + 94)
		{
			ModDropRate.ResetStats();
			return true;
		}

		// 5. Nút "Lưu Bãi"
		if (px >= uiX + 18 && px <= uiX + 74 && py >= uiY + 190 && py <= uiY + 210)
		{
			ModSetActivator.SaveCurrentFarmPosition();
			return true;
		}

		// 6. Nút "Bán Urôn"
		if (px >= uiX + 78 && px <= uiX + 136 && py >= uiY + 190 && py <= uiY + 210)
		{
			ModSetActivator.StartGoSellJunkNow();
			return true;
		}

		// 7. Nút "Mua Bùa"
		if (px >= uiX + 140 && px <= uiX + 200 && py >= uiY + 190 && py <= uiY + 210)
		{
			ModAutoBuyBua.StartBuyBuaNow();
			return true;
		}

		// 8. Nút "Về Bãi"
		if (px >= uiX + 204 && px <= uiX + 260 && py >= uiY + 190 && py <= uiY + 210)
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
			return true;
		}

		// 9. Nút "Xóa Bãi"
		if (px >= uiX + 264 && px <= uiX + 318 && py >= uiY + 190 && py <= uiY + 210)
		{
			ModSetActivator.ResetPosition();
			return true;
		}

		return false;
	}
}
