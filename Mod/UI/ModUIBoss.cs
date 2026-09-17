using System;

public static class ModUIBoss
{
	public static void Paint(int uiX, int uiY, int uiW, int uiH, mGraphics g)
	{
		mFont.tahoma_7_grey.drawString(g, "Báo Boss:", uiX + 6, uiY + 7, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 58, uiY + 4, 38, 16, ModBossNotice.isShowBossNotice ? "BẬT" : "TẮT", ModBossNotice.isShowBossNotice, g);
		ModUI.PaintNativeButton(uiX + 100, uiY + 4, 52, 16, "Xóa List", false, g);

		int hudBtnW = 38;
		int hudBtnX = uiX + uiW - hudBtnW - 6;
		mFont.tahoma_7_grey.drawString(g, "HUD Map:", hudBtnX - 52, uiY + 7, mFont.LEFT);
		ModUI.PaintNativeButton(hudBtnX, uiY + 4, hudBtnW, 16, ModMapEntityHUD.isShowMapEntityHUD ? "BẬT" : "TẮT", ModMapEntityHUD.isShowMapEntityHUD, g);

		// Hàng 2: Auto Né Broly, Khinh Công, Khoảng cách an toàn
		mFont.tahoma_7_grey.drawString(g, "Né Broly:", uiX + 6, uiY + 27, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 58, uiY + 24, 38, 16, ModKiteBroly.isAutoKite ? "BẬT" : "TẮT", ModKiteBroly.isAutoKite, g);

		int kcBtnW = 70;
		int kcBtnX = uiX + uiW - kcBtnW - 6;
		ModUI.PaintNativeButton(kcBtnX, uiY + 24, kcBtnW, 16, "KC: " + ModKiteBroly.safeDistance + "px", ModKiteBroly.safeDistance > 120, g);

		int kcBtn2W = 38;
		int kcBtn2X = kcBtnX - kcBtn2W - 6;
		mFont.tahoma_7_grey.drawString(g, "Khinh Công:", kcBtn2X - 60, uiY + 27, mFont.LEFT);
		ModUI.PaintNativeButton(kcBtn2X, uiY + 24, kcBtn2W, 16, ModKiteBroly.isKhinhCong ? "BẬT" : "TẮT", ModKiteBroly.isKhinhCong, g);

		// Dòng trạng thái Broly thời gian thực
		(ModKiteBroly.currentBrolyDist >= 0 ? mFont.tahoma_7_green2 : mFont.tahoma_7_grey).drawString(g, "Broly: " + ModKiteBroly.brolyStatusText, uiX + 6, uiY + 46, mFont.LEFT);

		int listX = uiX + 5;
		int listY = uiY + 60;
		int listW = uiW - 10;
		int listH = uiH - 66;
		GameCanvas.paintz.paintFrameSimple(listX, listY, listW, listH, g);
		g.setColor(15196114);
		g.fillRect(listX + 2, listY + 2, listW - 4, listH - 4);

		lock (ModBossNotice.listBossNotices)
		{
			if (ModBossNotice.listBossNotices.Count == 0)
			{
				mFont.tahoma_7_grey.drawString(g, "(Chưa nhận được thông báo boss nào từ server)", listX + listW / 2, listY + 60, mFont.CENTER);
			}
			else
			{
				int displayCount = (ModBossNotice.listBossNotices.Count < 8) ? ModBossNotice.listBossNotices.Count : 8;
				for (int i = 0; i < displayCount; i++)
				{
					ModBossNotice.BossNoticeEntry entry = ModBossNotice.listBossNotices[i];
					if (entry == null) continue;
					int rowY = listY + 3 + i * 17;

					g.setColor(entry.isDefeated ? 0x888888 : 0xcc2200);
					g.fillRect(listX + 5, rowY + 3, 3, 11);

					int curX = listX + 12;
					(entry.isDefeated ? mFont.tahoma_7_grey : mFont.tahoma_7_red).drawString(g, entry.bossName, curX, rowY + 1, mFont.LEFT);
					curX += (entry.isDefeated ? mFont.tahoma_7_grey : mFont.tahoma_7_red).getWidth(entry.bossName);

					mFont mapFont = entry.isDefeated ? mFont.tahoma_7_grey : (mFont.tahoma_7_blue ?? mFont.tahoma_7_white);
					if (mapFont != null)
					{
						mapFont.drawString(g, " - " + entry.mapName + " - ", curX, rowY + 1, mFont.LEFT);
						curX += mapFont.getWidth(" - " + entry.mapName + " - ");
					}

					string timeAgo = ModBossNotice.GetTimeAgoString(entry.timestamp);
					(entry.isDefeated ? mFont.tahoma_7_grey : mFont.tahoma_7_green2).drawString(g, timeAgo, curX, rowY + 1, mFont.LEFT);

					// Nút "Đến" nhanh cho Boss đang còn sống
					if (!entry.isDefeated)
					{
						ModUI.PaintNativeButton(listX + listW - 36, rowY + 1, 30, 15, "Đến", false, g);
					}
				}
			}
		}
	}

	public static bool HandleTap(int px, int py, int uiX, int uiY, int uiW, int uiH)
	{
		// 1. Nút Bật/Tắt HUD Báo Boss
		if (px >= uiX + 58 && px <= uiX + 96 && py >= uiY + 3 && py <= uiY + 21)
		{
			ModBossNotice.isShowBossNotice = !ModBossNotice.isShowBossNotice;
			ModConfig.SaveConfig();
			SoundMn.gI().buttonClick();
			return true;
		}

		// 2. Nút Xóa List
		if (px >= uiX + 100 && px <= uiX + 152 && py >= uiY + 3 && py <= uiY + 21)
		{
			lock (ModBossNotice.listBossNotices)
			{
				ModBossNotice.listBossNotices.Clear();
			}
			SoundMn.gI().buttonClick();
			return true;
		}

		// 3. Nút Bật/Tắt HUD Map
		int hudBtnW = 38;
		int hudBtnX = uiX + uiW - hudBtnW - 6;
		if (px >= hudBtnX && px <= hudBtnX + hudBtnW && py >= uiY + 3 && py <= uiY + 21)
		{
			ModMapEntityHUD.isShowMapEntityHUD = !ModMapEntityHUD.isShowMapEntityHUD;
			ModConfig.SaveConfig();
			SoundMn.gI().buttonClick();
			return true;
		}

		// 4. Nút Bật/Tắt Né Broly
		if (px >= uiX + 58 && px <= uiX + 96 && py >= uiY + 23 && py <= uiY + 41)
		{
			ModKiteBroly.ToggleAutoKite();
			return true;
		}

		// 5. Nút Bật/Tắt Khinh Công
		int kcBtnW = 70;
		int kcBtnX = uiX + uiW - kcBtnW - 6;
		int kcBtn2W = 38;
		int kcBtn2X = kcBtnX - kcBtn2W - 6;
		if (px >= kcBtn2X && px <= kcBtn2X + kcBtn2W && py >= uiY + 23 && py <= uiY + 41)
		{
			ModKiteBroly.ToggleKhinhCong();
			return true;
		}

		// 6. Nút Đổi Khoảng Cách An Toàn
		if (px >= kcBtnX && px <= kcBtnX + kcBtnW && py >= uiY + 23 && py <= uiY + 41)
		{
			ModKiteBroly.CycleSafeDistance();
			return true;
		}

		int listX = uiX + 5;
		int listY = uiY + 60;
		int listW = uiW - 10;
		lock (ModBossNotice.listBossNotices)
		{
			int displayCount = (ModBossNotice.listBossNotices.Count < 8) ? ModBossNotice.listBossNotices.Count : 8;
			for (int i = 0; i < displayCount; i++)
			{
				ModBossNotice.BossNoticeEntry entry = ModBossNotice.listBossNotices[i];
				if (entry == null || entry.isDefeated) continue;

				int rowY = listY + 3 + i * 17;
				if (px >= listX + listW - 38 && px <= listX + listW - 4 && py >= rowY && py <= rowY + 16)
				{
					int targetMapId = ModNextMap.FindMapIdByName(entry.mapName);
					if (targetMapId >= 0)
					{
						ModUI.uiCustomOpen = false;
						ModNextMap.StartNextMap(targetMapId);
						GameScr.info1.addInfo("Di chuyển đến " + entry.mapName + " săn " + entry.bossName, 0);
						SoundMn.gI().buttonClick();
						return true;
					}
					else
					{
						GameScr.info1.addInfo("Chưa xác định được map: " + entry.mapName, 0);
					}
					return true;
				}
			}
		}

		return false;
	}
}
