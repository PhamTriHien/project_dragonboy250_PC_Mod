using System;

/// <summary>
/// CLASS ModUISetActivator: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public static class ModUISetActivator
{
	/// <summary>
	/// Thực hiện vẽ toàn bộ đối tượng và các lớp hiển thị lên canvas mGraphics.
	/// </summary>
	public static void Paint(int uiX, int uiY, int uiW, int uiH, mGraphics g)
	{
		// Hàng 1: Auto Úp Set KH, Tự Bán Rác & Tự Cất Rương
		int r1W = (uiW - 16) / 3;
		ModUI.PaintNativeButton(uiX + 4, uiY + 4, r1W, 17, "Úp SKH: " + (ModSetActivator.isActive ? "BẬT" : "TẮT"), ModSetActivator.isActive, g);
		ModUI.PaintNativeButton(uiX + 8 + r1W, uiY + 4, r1W, 17, "Bán Rác: " + (ModSetActivator.autoSellJunk ? "BẬT" : "TẮT"), ModSetActivator.autoSellJunk, g);
		ModUI.PaintNativeButton(uiX + 12 + r1W * 2, uiY + 4, r1W, 17, "Cất Rương: " + (ModSetActivator.autoStoreToBox ? "BẬT" : "TẮT"), ModSetActivator.autoStoreToBox, g);

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
		string spotText = (ModSetActivator.savedFarmMapId >= 0)
			? (ModNextMap.GetMapName(ModSetActivator.savedFarmMapId) + " [K." + ModSetActivator.savedFarmZoneId + "] (" + ModSetActivator.savedFarmX + ", " + ModSetActivator.savedFarmY + ")")
			: "(Chưa lưu - Bấm 'Lưu Bãi' để lưu)";
		mFont.tahoma_7_grey.drawString(g, "- Bãi úp: ", uiX + 10, listY + 18, mFont.LEFT);
		(ModSetActivator.savedFarmMapId >= 0 ? mFont.tahoma_7_green2 : mFont.tahoma_7_grey).drawString(g, spotText, uiX + 56, listY + 18, mFont.LEFT);

		// 4 nút lọc: Lọc Đồ Sao | Hút Tức Thì | Hồi Sinh Đánh | Khóa Kết Liễu
		int w4 = (listW - 20) / 4;
		ModUI.PaintNativeButton(uiX + 8, listY + 34, w4, 16, ModSetActivator.GetMinStarText(), ModSetActivator.minStarToKeep > 0, g);
		ModUI.PaintNativeButton(uiX + 11 + w4, listY + 34, w4, 16, ModDropRate.isInstantPick ? "Hút Đồ" : "Hút: Tắt", ModDropRate.isInstantPick, g);
		ModUI.PaintNativeButton(uiX + 14 + w4 * 2, listY + 34, w4, 16, ModDropRate.isInstantRespawnAttack ? "HS Đánh" : "HS: Tắt", ModDropRate.isInstantRespawnAttack, g);
		ModUI.PaintNativeButton(uiX + 17 + w4 * 3, listY + 34, w4, 16, ModDropRate.isLastHitLock ? "Khóa K.Liễu" : "K.Liễu: Tắt", ModDropRate.isLastHitLock, g);

		// Hàng cài đặt Bùa Bà Hạt Mít & Hiện ID Trang Bị
		ModUI.PaintNativeButton(uiX + 8, listY + 52, w4, 16, ModAutoBuyBua.buaTypeNames[ModAutoBuyBua.selectedBuaType], true, g);
		ModUI.PaintNativeButton(uiX + 11 + w4, listY + 52, w4, 16, ModAutoBuyBua.packageNames[ModAutoBuyBua.selectedPackage], false, g);
		ModUI.PaintNativeButton(uiX + 14 + w4 * 2, listY + 52, w4, 16, ModAutoBuyBua.isAutoRebuy ? "Bùa: Auto" : "Bùa: Tắt", ModAutoBuyBua.isAutoRebuy, g);
		ModUI.PaintNativeButton(uiX + 17 + w4 * 3, listY + 52, w4, 16, ModSetActivator.showItemId ? "Hiện ID" : "ID: Tắt", ModSetActivator.showItemId, g);

		// Hàng cài đặt mở rộng: HUD SKH | Auto TĐLT | Giữ Đồ Thần | Ô Trống
		ModUI.PaintNativeButton(uiX + 8, listY + 70, w4, 16, ModSetActivator.showSkhHud ? "HUD SKH" : "SKH: Tắt", ModSetActivator.showSkhHud, g);
		ModUI.PaintNativeButton(uiX + 11 + w4, listY + 70, w4, 16, ModSetActivator.autoUseTdlt ? "Auto TĐLT" : "TĐLT: Tắt", ModSetActivator.autoUseTdlt, g);
		ModUI.PaintNativeButton(uiX + 14 + w4 * 2, listY + 70, w4, 16, !ModSetActivator.sellGodItems ? "Giữ Thần" : "Bán Thần", !ModSetActivator.sellGodItems, g);
		ModUI.PaintNativeButton(uiX + 17 + w4 * 3, listY + 70, w4, 16, "Trống <=" + ModSetActivator.minEmptyBagSlots, true, g);

		// Thống kê rơi đồ thực chiến
		string stat1 = "- Diệt: " + ModDropRate.totalMobsKilled + " (" + ModDropRate.GetMobsPerMinute() + "/p) | Rơi: " + ModDropRate.totalItemsDropped + " | KH: " + ModDropRate.totalSetKHCount + " | Sao: " + ModDropRate.totalStarCount;
		mFont.tahoma_7_green2.drawString(g, stat1, uiX + 10, listY + 89, mFont.LEFT);

		ModUI.PaintNativeButton(uiX + listW - 48, listY + 87, 44, 15, "Reset TK", false, g);

		// Trạng thái vận hành
		string stText = ModAutoBuyBua.isBusy ? ModAutoBuyBua.GetStatusText() : ModSetActivator.GetStatusText();
		mFont.tahoma_7_grey.drawString(g, "- Trạng thái: ", uiX + 10, listY + 103, mFont.LEFT);
		mFont.tahoma_7_green2.drawString(g, stText, uiX + 68, listY + 103, mFont.LEFT);

		// Hàng 5 nút chức năng dưới cùng ở Footer
		int fBtnW = (uiW - 24) / 5;
		string[] bottomLabels = new string[] { "Lưu Bãi", "Bán Rác", "Cất Rương", "Mua Bùa", "Về Bãi" };
		for (int b = 0; b < 5; b++)
		{
			int bx = uiX + 4 + b * (fBtnW + 4);
			ModUI.PaintNativeButton(bx, footerY, fBtnW, footerH, bottomLabels[b], false, g);
		}
	}

	/// <summary>
	/// Xử lý nghiệp vụ logic chuyên trách cho HandleTap.
	/// </summary>
	public static bool HandleTap(int px, int py, int uiX, int uiY, int uiW, int uiH)
	{
		int r1W = (uiW - 16) / 3;
		int footerH = 18;
		int footerY = uiY + uiH - footerH - 4;
		int listY = uiY + 24;
		int listW = uiW - 8;

		// 1. Hàng 1: Úp SKH | Bán Rác | Cất Rương
		if (py >= uiY + 2 && py <= uiY + 22)
		{
			if (px >= uiX + 4 && px <= uiX + 4 + r1W)
			{
				ModSetActivator.ToggleActive();
				return true;
			}
			if (px >= uiX + 8 + r1W && px <= uiX + 8 + r1W * 2)
			{
				ModSetActivator.autoSellJunk = !ModSetActivator.autoSellJunk;
				ModConfig.SaveConfig();
				GameScr.info1.addInfo("Tự bán khi full túi: " + (ModSetActivator.autoSellJunk ? "BẬT" : "TẮT"), 0);
				SoundMn.gI().buttonClick();
				return true;
			}
			if (px >= uiX + 12 + r1W * 2 && px <= uiX + 12 + r1W * 3)
			{
				ModSetActivator.autoStoreToBox = !ModSetActivator.autoStoreToBox;
				ModConfig.SaveConfig();
				GameScr.info1.addInfo("Tự cất đồ SKH/Sao vào rương: " + (ModSetActivator.autoStoreToBox ? "BẬT" : "TẮT"), 0);
				SoundMn.gI().buttonClick();
				return true;
			}
		}

		// 3. 4 nút lọc hàng 1
		int w4 = (listW - 20) / 4;
		if (px >= uiX + 8 && px <= uiX + 8 + w4 && py >= listY + 33 && py <= listY + 51)
		{
			ModSetActivator.CycleMinStar();
			ModConfig.SaveConfig();
			SoundMn.gI().buttonClick();
			return true;
		}

		if (px >= uiX + 11 + w4 && px <= uiX + 11 + w4 * 2 && py >= listY + 33 && py <= listY + 51)
		{
			ModDropRate.isInstantPick = !ModDropRate.isInstantPick;
			ModConfig.SaveConfig();
			GameScr.info1.addInfo("Hút đồ tức thì: " + (ModDropRate.isInstantPick ? "BẬT" : "TẮT"), 0);
			SoundMn.gI().buttonClick();
			return true;
		}

		if (px >= uiX + 14 + w4 * 2 && px <= uiX + 14 + w4 * 3 && py >= listY + 33 && py <= listY + 51)
		{
			ModDropRate.isInstantRespawnAttack = !ModDropRate.isInstantRespawnAttack;
			ModConfig.SaveConfig();
			GameScr.info1.addInfo("Hồi sinh đánh ngay: " + (ModDropRate.isInstantRespawnAttack ? "BẬT" : "TẮT"), 0);
			SoundMn.gI().buttonClick();
			return true;
		}

		if (px >= uiX + 17 + w4 * 3 && px <= uiX + 17 + w4 * 4 && py >= listY + 33 && py <= listY + 51)
		{
			ModDropRate.isLastHitLock = !ModDropRate.isLastHitLock;
			ModConfig.SaveConfig();
			GameScr.info1.addInfo("Khóa đòn kết liễu: " + (ModDropRate.isLastHitLock ? "BẬT" : "TẮT"), 0);
			SoundMn.gI().buttonClick();
			return true;
		}

		// 4. Hàng Bùa & Hiện ID Trang Bị
		if (px >= uiX + 8 && px <= uiX + 8 + w4 && py >= listY + 51 && py <= listY + 69)
		{
			ModAutoBuyBua.CycleBuaType();
			ModConfig.SaveConfig();
			SoundMn.gI().buttonClick();
			return true;
		}

		if (px >= uiX + 11 + w4 && px <= uiX + 11 + w4 * 2 && py >= listY + 51 && py <= listY + 69)
		{
			ModAutoBuyBua.CyclePackage();
			ModConfig.SaveConfig();
			SoundMn.gI().buttonClick();
			return true;
		}

		if (px >= uiX + 14 + w4 * 2 && px <= uiX + 14 + w4 * 3 && py >= listY + 51 && py <= listY + 69)
		{
			ModAutoBuyBua.isAutoRebuy = !ModAutoBuyBua.isAutoRebuy;
			ModConfig.SaveConfig();
			GameScr.info1.addInfo("Auto gia hạn bùa: " + (ModAutoBuyBua.isAutoRebuy ? "BẬT" : "TẮT"), 0);
			SoundMn.gI().buttonClick();
			return true;
		}

		if (px >= uiX + 17 + w4 * 3 && px <= uiX + 17 + w4 * 4 && py >= listY + 51 && py <= listY + 69)
		{
			ModSetActivator.showItemId = !ModSetActivator.showItemId;
			ModConfig.SaveConfig();
			GameScr.info1.addInfo("Hiện ID trang bị: " + (ModSetActivator.showItemId ? "BẬT" : "TẮT"), 0);
			SoundMn.gI().buttonClick();
			return true;
		}

		// 4b. Hàng mở rộng: HUD SKH | Auto TĐLT | Giữ Đồ Thần | Ô Trống
		if (py >= listY + 69 && py <= listY + 87)
		{
			if (px >= uiX + 8 && px <= uiX + 8 + w4)
			{
				ModSetActivator.showSkhHud = !ModSetActivator.showSkhHud;
				ModConfig.SaveConfig();
				GameScr.info1.addInfo("HUD Thống kê SKH: " + (ModSetActivator.showSkhHud ? "BẬT" : "TẮT"), 0);
				SoundMn.gI().buttonClick();
				return true;
			}
			if (px >= uiX + 11 + w4 && px <= uiX + 11 + w4 * 2)
			{
				ModSetActivator.autoUseTdlt = !ModSetActivator.autoUseTdlt;
				ModConfig.SaveConfig();
				GameScr.info1.addInfo("Auto Tự động luyện tập (521): " + (ModSetActivator.autoUseTdlt ? "BẬT" : "TẮT"), 0);
				SoundMn.gI().buttonClick();
				return true;
			}
			if (px >= uiX + 14 + w4 * 2 && px <= uiX + 14 + w4 * 3)
			{
				ModSetActivator.sellGodItems = !ModSetActivator.sellGodItems;
				ModConfig.SaveConfig();
				GameScr.info1.addInfo("Giữ Đồ Thần Linh khi bán rác: " + (!ModSetActivator.sellGodItems ? "BẬT" : "TẮT"), 0);
				SoundMn.gI().buttonClick();
				return true;
			}
			if (px >= uiX + 17 + w4 * 3 && px <= uiX + 17 + w4 * 4)
			{
				ModSetActivator.CycleMinEmptyBagSlots();
				GameScr.info1.addInfo("Bán rác khi ô trống <= " + ModSetActivator.minEmptyBagSlots, 0);
				return true;
			}
		}

		// 5. Nút Reset TK
		if (px >= uiX + listW - 50 && px <= uiX + listW && py >= listY + 86 && py <= listY + 103)
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
							ModSetActivator.StartGoStoreBoxNow();
							break;
						case 3:
							ModAutoBuyBua.StartBuyBuaNow();
							break;
						case 4:
							if (ModSetActivator.savedFarmMapId >= 0)
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
					}
					return true;
				}
			}
		}

		return false;
	}
}
