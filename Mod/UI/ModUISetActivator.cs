using System;

/// <summary>
/// Giao diện điều khiển phân hệ Úp Set Kích Hoạt (SKH), Lọc/Bán đồ rác, Cất rương, Auto TĐLT Santa & Auto Mua 10 Bùa.
/// </summary>
public static class ModUISetActivator
{
	public static int scrollY = 0;
	private static bool s_IsDragging = false;
	private static bool s_HasDragged = false;
	private static int s_StartDragY = 0;
	private static int s_LastDragY = 0;
	private static int s_LastMaxScroll = 0;

	public static void OnMouseScroll(float wheel)
	{
		if (s_LastMaxScroll <= 0) return;
		int step = 20;
		if (wheel > 0) scrollY -= step;
		else if (wheel < 0) scrollY += step;
		if (scrollY < 0) scrollY = 0;
		if (scrollY > s_LastMaxScroll) scrollY = s_LastMaxScroll;
	}

	public static void UpdateDragScroll(int px, int py, int uiX, int uiY, int uiW, int uiH)
	{
		int footerH = 18;
		int footerY = uiY + uiH - footerH - 4;
		int listY = uiY + 24;
		int listW = uiW - 8;
		int listH = (footerY - 4) - listY;
		if (listH < 40) listH = 40;

		if (GameCanvas.isPointerDown)
		{
			if (!s_IsDragging)
			{
				if (px >= uiX + 4 && px <= uiX + 4 + listW && py >= listY && py <= listY + listH)
				{
					s_IsDragging = true;
					s_HasDragged = false;
					s_StartDragY = py;
					s_LastDragY = py;
				}
			}
			else
			{
				int dy = py - s_LastDragY;
				s_LastDragY = py;
				if (Res.abs(py - s_StartDragY) > 6)
				{
					s_HasDragged = true;
				}
				if (s_HasDragged && s_LastMaxScroll > 0)
				{
					scrollY -= dy;
					if (scrollY < 0) scrollY = 0;
					if (scrollY > s_LastMaxScroll) scrollY = s_LastMaxScroll;
				}
			}
		}
		else
		{
			s_IsDragging = false;
		}
	}

	public static void Paint(int uiX, int uiY, int uiW, int uiH, mGraphics g)
	{
		int r1W = (uiW - 16) / 3;
		ModUI.PaintNativeButton(uiX + 4, uiY + 4, r1W, 17, "Úp SKH: " + (ModSetActivator.isActive ? "BẬT" : "TẮT"), ModSetActivator.isActive, g);
		ModUI.PaintNativeButton(uiX + 8 + r1W, uiY + 4, r1W, 17, "Bán Rác: " + (ModSetActivator.autoSellJunk ? "BẬT" : "TẮT"), ModSetActivator.autoSellJunk, g);
		ModUI.PaintNativeButton(uiX + 12 + r1W * 2, uiY + 4, r1W, 17, "Cất Rương: " + (ModSetActivator.autoStoreToBox ? "BẬT" : "TẮT"), ModSetActivator.autoStoreToBox, g);

		int footerH = 18;
		int footerY = uiY + uiH - footerH - 4;

		int listY = uiY + 24;
		int listW = uiW - 8;
		int listH = (footerY - 4) - listY;
		if (listH < 40) listH = 40;

		int innerContentH = 162;
		s_LastMaxScroll = (innerContentH > listH - 4) ? (innerContentH - (listH - 4)) : 0;
		if (scrollY > s_LastMaxScroll) scrollY = s_LastMaxScroll;
		if (scrollY < 0) scrollY = 0;

		GameCanvas.paintz.paintFrameSimple(uiX + 4, listY, listW, listH, g);
		g.setColor(15196114);
		g.fillRect(uiX + 6, listY + 2, listW - 4, listH - 4);

		g.setClip(uiX + 6, listY + 2, listW - 4, listH - 4);
		int cy = listY - scrollY;

		mFont.tahoma_7_green2.drawString(g, "BÃI ÚP, 15 BỘ SKH, TĐLT SANTA & 10 BÙA:", uiX + 10, cy + 4, mFont.LEFT);

		string spotText = (ModSetActivator.savedFarmMapId >= 0)
			? (ModNextMap.GetMapName(ModSetActivator.savedFarmMapId) + " [K." + ModSetActivator.savedFarmZoneId + "] (" + ModSetActivator.savedFarmX + ", " + ModSetActivator.savedFarmY + ")")
			: "(Chưa lưu - Bấm 'Lưu Bãi' để lưu)";
		mFont.tahoma_7_grey.drawString(g, "- Bãi úp: ", uiX + 10, cy + 18, mFont.LEFT);
		(ModSetActivator.savedFarmMapId >= 0 ? mFont.tahoma_7_green2 : mFont.tahoma_7_grey).drawString(g, spotText, uiX + 56, cy + 18, mFont.LEFT);

		int w4 = (listW - 20) / 4;
		// Hàng nút 1: Sao | Hút Đồ | HS Đánh | Khóa K.Liễu
		ModUI.PaintNativeButton(uiX + 8, cy + 33, w4, 16, ModSetActivator.GetMinStarText(), ModSetActivator.minStarToKeep > 0, g);
		ModUI.PaintNativeButton(uiX + 11 + w4, cy + 33, w4, 16, ModDropRate.isInstantPick ? "Hút Đồ" : "Hút: Tắt", ModDropRate.isInstantPick, g);
		ModUI.PaintNativeButton(uiX + 14 + w4 * 2, cy + 33, w4, 16, ModDropRate.isInstantRespawnAttack ? "HS Đánh" : "HS: Tắt", ModDropRate.isInstantRespawnAttack, g);
		ModUI.PaintNativeButton(uiX + 17 + w4 * 3, cy + 33, w4, 16, ModDropRate.isLastHitLock ? "Khóa K.Liễu" : "K.Liễu: Tắt", ModDropRate.isLastHitLock, g);

		// Hàng nút 2: Chọn 10 Bùa | Gói Bùa | Auto Bùa | HUD Bùa
		ModUI.PaintNativeButton(uiX + 8, cy + 51, w4, 16, ModAutoBuyBua.GetSelectedButtonLabel(), ModAutoBuyBua.GetSelectedCount() > 0, g);
		ModUI.PaintNativeButton(uiX + 11 + w4, cy + 51, w4, 16, ModAutoBuyBua.packageNames[ModAutoBuyBua.selectedPackage], false, g);
		ModUI.PaintNativeButton(uiX + 14 + w4 * 2, cy + 51, w4, 16, ModAutoBuyBua.isAutoRebuy ? "Bùa: Auto" : "Bùa: Tắt", ModAutoBuyBua.isAutoRebuy, g);
		ModUI.PaintNativeButton(uiX + 17 + w4 * 3, cy + 51, w4, 16, ModAutoBuyBua.showCharmHud ? "HUD Bùa" : "HUD Bùa:Tắt", ModAutoBuyBua.showCharmHud, g);

		// Hàng nút 3: HUD 15 SKH | Auto TĐLT | Mua TĐLT Santa | Hiện Chỉ Số
		ModUI.PaintNativeButton(uiX + 8, cy + 69, w4, 16, ModSetActivator.showSkhHud ? "HUD 15 SKH" : "SKH: Tắt", ModSetActivator.showSkhHud, g);
		ModUI.PaintNativeButton(uiX + 11 + w4, cy + 69, w4, 16, ModSetActivator.autoUseTdlt ? "Dùng TĐLT" : "TĐLT: Tắt", ModSetActivator.autoUseTdlt, g);
		ModUI.PaintNativeButton(uiX + 14 + w4 * 2, cy + 69, w4, 16, ModSetActivator.autoBuyTdlt ? "Mua TĐLT:Bật" : "Mua TĐLT:Tắt", ModSetActivator.autoBuyTdlt, g);
		ModUI.PaintNativeButton(uiX + 17 + w4 * 3, cy + 69, w4, 16, ModSetActivator.showItemId ? "Hiện %Đồ" : "%Đồ: Tắt", ModSetActivator.showItemId, g);

		// Hàng nút 4: Đồ Thần | Đồ SKH | Cấp Bán | Ô Trống
		ModUI.PaintNativeButton(uiX + 8, cy + 87, w4, 16, !ModSetActivator.sellGodItems ? "Giữ Thần" : "Bán Thần", !ModSetActivator.sellGodItems, g);
		ModUI.PaintNativeButton(uiX + 11 + w4, cy + 87, w4, 16, !ModSetActivator.sellSkhItems ? "Giữ SKH" : "Bán SKH", !ModSetActivator.sellSkhItems, g);
		ModUI.PaintNativeButton(uiX + 14 + w4 * 2, cy + 87, w4, 16, "Bán Cấp<=" + ModSetActivator.sellMaxLevel, true, g);
		ModUI.PaintNativeButton(uiX + 17 + w4 * 3, cy + 87, w4, 16, "Trống <=" + ModSetActivator.minEmptyBagSlots, true, g);

		// Hàng nút 5: Nhập Giữ ID | Nhập Bán ID | Mua TĐLT Ngay | Reset TK
		ModUI.PaintNativeButton(uiX + 8, cy + 105, w4, 16, string.IsNullOrEmpty(ModSetActivator.keepItemIdsRaw) ? "Giữ ID" : "Giữ ID*", !string.IsNullOrEmpty(ModSetActivator.keepItemIdsRaw), g);
		ModUI.PaintNativeButton(uiX + 11 + w4, cy + 105, w4, 16, string.IsNullOrEmpty(ModSetActivator.sellItemIdsRaw) ? "Bán ID" : "Bán ID*", !string.IsNullOrEmpty(ModSetActivator.sellItemIdsRaw), g);
		ModUI.PaintNativeButton(uiX + 14 + w4 * 2, cy + 105, w4, 16, "Mua TĐLT", false, g);
		ModUI.PaintNativeButton(uiX + 17 + w4 * 3, cy + 105, w4, 16, "Reset TK", false, g);

		string stat1 = "- Diệt: " + ModDropRate.totalMobsKilled + " (" + ModDropRate.GetMobsPerMinute() + "/p) | Rơi: " + ModDropRate.totalItemsDropped + " | KH: " + ModDropRate.totalSetKHCount + " | Sao: " + ModDropRate.totalStarCount;
		mFont.tahoma_7_green2.drawString(g, stat1, uiX + 10, cy + 125, mFont.LEFT);

		string stText = ModAutoBuyBua.isBusy ? ModAutoBuyBua.GetStatusText() : ModSetActivator.GetStatusText();
		mFont.tahoma_7_grey.drawString(g, "- Trạng thái: ", uiX + 10, cy + 140, mFont.LEFT);
		mFont.tahoma_7_green2.drawString(g, stText, uiX + 68, cy + 140, mFont.LEFT);

		g.setClip(uiX, uiY, uiW, uiH);

		int fBtnW = (uiW - 24) / 5;
		string[] bottomLabels = new string[] { "Lưu Bãi", "Bán Rác", "Cất Rương", "Mua Bùa", "Về Bãi" };
		for (int b = 0; b < 5; b++)
		{
			int bx = uiX + 4 + b * (fBtnW + 4);
			ModUI.PaintNativeButton(bx, footerY, fBtnW, footerH, bottomLabels[b], false, g);
		}
	}

	public static bool HandleTap(int px, int py, int uiX, int uiY, int uiW, int uiH)
	{
		if (s_HasDragged)
		{
			s_HasDragged = false;
			return true;
		}

		int r1W = (uiW - 16) / 3;
		int footerH = 18;
		int footerY = uiY + uiH - footerH - 4;
		int listY = uiY + 24;
		int listW = uiW - 8;
		int listH = (footerY - 4) - listY;
		if (listH < 40) listH = 40;

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

		if (py >= listY && py <= listY + listH)
		{
			int cy = listY - scrollY;
			int w4 = (listW - 20) / 4;

			int col = -1;
			if (px >= uiX + 8 && px <= uiX + 8 + w4) col = 0;
			else if (px >= uiX + 11 + w4 && px <= uiX + 11 + w4 * 2) col = 1;
			else if (px >= uiX + 14 + w4 * 2 && px <= uiX + 14 + w4 * 3) col = 2;
			else if (px >= uiX + 17 + w4 * 3 && px <= uiX + 17 + w4 * 4) col = 3;

			if (col >= 0)
			{
				if (py >= cy + 32 && py <= cy + 50)
				{
					switch (col)
					{
						case 0:
							ModSetActivator.CycleMinStar();
							break;
						case 1:
							ModDropRate.isInstantPick = !ModDropRate.isInstantPick;
							ModConfig.SaveConfig();
							GameScr.info1.addInfo("Hút đồ tức thì: " + (ModDropRate.isInstantPick ? "BẬT" : "TẮT"), 0);
							SoundMn.gI().buttonClick();
							break;
						case 2:
							ModDropRate.isInstantRespawnAttack = !ModDropRate.isInstantRespawnAttack;
							ModConfig.SaveConfig();
							GameScr.info1.addInfo("Hồi sinh đánh ngay: " + (ModDropRate.isInstantRespawnAttack ? "BẬT" : "TẮT"), 0);
							SoundMn.gI().buttonClick();
							break;
						case 3:
							ModDropRate.isLastHitLock = !ModDropRate.isLastHitLock;
							ModConfig.SaveConfig();
							GameScr.info1.addInfo("Khóa đòn kết liễu: " + (ModDropRate.isLastHitLock ? "BẬT" : "TẮT"), 0);
							SoundMn.gI().buttonClick();
							break;
					}
					return true;
				}

				if (py >= cy + 50 && py <= cy + 68)
				{
					switch (col)
					{
						case 0:
							ModUI.uiCustomOpen = false;
							ModAutoBuyBua.ShowCharmChecklistMenu();
							SoundMn.gI().buttonClick();
							break;
						case 1:
							ModAutoBuyBua.CyclePackage();
							break;
						case 2:
							ModAutoBuyBua.ToggleAutoRebuy();
							break;
						case 3:
							ModAutoBuyBua.showCharmHud = !ModAutoBuyBua.showCharmHud;
							ModConfig.SaveConfig();
							GameScr.info1.addInfo("HUD Bùa Bà Hạt Mít: " + (ModAutoBuyBua.showCharmHud ? "BẬT" : "TẮT"), 0);
							SoundMn.gI().buttonClick();
							break;
					}
					return true;
				}

				if (py >= cy + 68 && py <= cy + 86)
				{
					switch (col)
					{
						case 0:
							ModSetActivator.showSkhHud = !ModSetActivator.showSkhHud;
							ModConfig.SaveConfig();
							GameScr.info1.addInfo("HUD Thống kê 15 bộ SKH: " + (ModSetActivator.showSkhHud ? "BẬT" : "TẮT"), 0);
							SoundMn.gI().buttonClick();
							break;
						case 1:
							ModSetActivator.autoUseTdlt = !ModSetActivator.autoUseTdlt;
							ModConfig.SaveConfig();
							GameScr.info1.addInfo("Auto dùng TĐLT (521): " + (ModSetActivator.autoUseTdlt ? "BẬT" : "TẮT"), 0);
							SoundMn.gI().buttonClick();
							break;
						case 2:
							ModSetActivator.autoBuyTdlt = !ModSetActivator.autoBuyTdlt;
							ModConfig.SaveConfig();
							GameScr.info1.addInfo("Tự mua/gia hạn TĐLT tại Santa (<10p): " + (ModSetActivator.autoBuyTdlt ? "BẬT" : "TẮT"), 0);
							SoundMn.gI().buttonClick();
							break;
						case 3:
							ModSetActivator.showItemId = !ModSetActivator.showItemId;
							ModConfig.SaveConfig();
							GameScr.info1.addInfo("Hiện chỉ số rút gọn trên ô đồ: " + (ModSetActivator.showItemId ? "BẬT" : "TẮT"), 0);
							SoundMn.gI().buttonClick();
							break;
					}
					return true;
				}

				if (py >= cy + 86 && py <= cy + 104)
				{
					switch (col)
					{
						case 0:
							ModSetActivator.sellGodItems = !ModSetActivator.sellGodItems;
							ModConfig.SaveConfig();
							GameScr.info1.addInfo("Giữ Đồ Thần Linh khi bán rác: " + (!ModSetActivator.sellGodItems ? "BẬT" : "TẮT"), 0);
							SoundMn.gI().buttonClick();
							break;
						case 1:
							ModSetActivator.sellSkhItems = !ModSetActivator.sellSkhItems;
							ModConfig.SaveConfig();
							GameScr.info1.addInfo("Giữ Đồ SKH khi bán rác: " + (!ModSetActivator.sellSkhItems ? "BẬT" : "TẮT"), 0);
							SoundMn.gI().buttonClick();
							break;
						case 2:
							SoundMn.gI().buttonClick();
							ModSetActivator.ShowInputSellMaxLevel();
							break;
						case 3:
							SoundMn.gI().buttonClick();
							ModSetActivator.ShowInputMinEmptyBagSlots();
							break;
					}
					return true;
				}

				if (py >= cy + 104 && py <= cy + 122)
				{
					switch (col)
					{
						case 0:
							ModUI.uiCustomOpen = false;
							ModSetActivator.ShowInputKeepIds();
							SoundMn.gI().buttonClick();
							break;
						case 1:
							ModUI.uiCustomOpen = false;
							ModSetActivator.ShowInputSellIds();
							SoundMn.gI().buttonClick();
							break;
						case 2:
							ModUI.uiCustomOpen = false;
							ModSetActivator.StartBuyTdltNow();
							SoundMn.gI().buttonClick();
							break;
						case 3:
							ModDropRate.ResetStats();
							GameScr.info1.addInfo("Đã reset thống kê rơi đồ!", 0);
							SoundMn.gI().buttonClick();
							break;
					}
					return true;
				}
			}
		}

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
