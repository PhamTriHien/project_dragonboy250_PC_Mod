using System;

public static class ModUIBoss
{
	public static void Paint(int uiX, int uiY, int uiW, int uiH, mGraphics g)
	{
		mFont.tahoma_7b_dark.drawString(g, "Báo Boss:", uiX + 8, uiY + 12, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 68, uiY + 8, 42, 18, ModBossNotice.isShowBossNotice ? "BẬT" : "TẮT", ModBossNotice.isShowBossNotice, g);
		ModUI.PaintNativeButton(uiX + 114, uiY + 8, 56, 18, "Xóa List", false, g);

		mFont.tahoma_7b_dark.drawString(g, "HUD Map:", uiX + 180, uiY + 12, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 236, uiY + 8, 42, 18, ModMapEntityHUD.isShowMapEntityHUD ? "BẬT" : "TẮT", ModMapEntityHUD.isShowMapEntityHUD, g);

		// Hàng 2: Auto Né Broly, Khinh Công, Khoảng cách an toàn
		mFont.tahoma_7b_dark.drawString(g, "Né Broly:", uiX + 8, uiY + 34, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 68, uiY + 30, 42, 18, ModKiteBroly.isAutoKite ? "BẬT" : "TẮT", ModKiteBroly.isAutoKite, g);

		mFont.tahoma_7b_dark.drawString(g, "Khinh Công:", uiX + 118, uiY + 34, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 182, uiY + 30, 42, 18, ModKiteBroly.isKhinhCong ? "BẬT" : "TẮT", ModKiteBroly.isKhinhCong, g);

		ModUI.PaintNativeButton(uiX + 230, uiY + 30, 80, 18, "KC: " + ModKiteBroly.safeDistance + "px", ModKiteBroly.safeDistance > 120, g);

		// Dòng trạng thái Broly thời gian thực
		(ModKiteBroly.currentBrolyDist >= 0 ? mFont.tahoma_7b_green2 : mFont.tahoma_7_grey).drawString(g, "Broly: " + ModKiteBroly.brolyStatusText, uiX + 8, uiY + 54, mFont.LEFT);

		int listX = uiX + 6;
		int listY = uiY + 68;
		int listW = uiW - 12;
		int listH = uiH - 74;
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
				int displayCount = (ModBossNotice.listBossNotices.Count < 7) ? ModBossNotice.listBossNotices.Count : 7;
				for (int i = 0; i < displayCount; i++)
				{
					ModBossNotice.BossNoticeEntry entry = ModBossNotice.listBossNotices[i];
					if (entry == null) continue;
					int rowY = listY + 4 + i * 20;

					g.setColor(entry.isDefeated ? 0x888888 : 0xcc2200);
					g.fillRect(listX + 6, rowY + 3, 4, 12);

					int curX = listX + 14;
					if (mFont.tahoma_7b_dark != null)
					{
						(entry.isDefeated ? mFont.tahoma_7_grey : mFont.tahoma_7b_dark).drawString(g, entry.bossName, curX, rowY + 1, mFont.LEFT);
						curX += (entry.isDefeated ? mFont.tahoma_7_grey : mFont.tahoma_7b_dark).getWidth(entry.bossName);
					}

					mFont mapFont = entry.isDefeated ? mFont.tahoma_7_grey : (mFont.tahoma_7_blue ?? mFont.tahoma_7b_blue ?? mFont.tahoma_7_grey);
					if (mapFont != null)
					{
						mapFont.drawString(g, " - " + entry.mapName + " - ", curX, rowY + 1, mFont.LEFT);
						curX += mapFont.getWidth(" - " + entry.mapName + " - ");
					}

					if (mFont.tahoma_7b_green2 != null)
					{
						string timeAgo = ModBossNotice.GetTimeAgoString(entry.timestamp);
						(entry.isDefeated ? mFont.tahoma_7_grey : mFont.tahoma_7b_green2).drawString(g, timeAgo, curX, rowY + 1, mFont.LEFT);
					}

					// Nút "Đến" nhanh cho Boss đang còn sống
					if (!entry.isDefeated)
					{
						ModUI.PaintNativeButton(listX + listW - 38, rowY + 1, 32, 16, "Đến", false, g);
					}
				}
			}
		}
	}

	public static bool HandleTap(int px, int py, int uiX, int uiY, int uiW, int uiH)
	{
		// 1. Nút Bật/Tắt HUD Báo Boss
		if (px >= uiX + 68 && px <= uiX + 110 && py >= uiY + 6 && py <= uiY + 28)
		{
			ModBossNotice.isShowBossNotice = !ModBossNotice.isShowBossNotice;
			ModConfig.SaveConfig();
			SoundMn.gI().buttonClick();
			return true;
		}

		// 2. Nút Xóa List
		if (px >= uiX + 114 && px <= uiX + 170 && py >= uiY + 6 && py <= uiY + 28)
		{
			lock (ModBossNotice.listBossNotices)
			{
				ModBossNotice.listBossNotices.Clear();
			}
			SoundMn.gI().buttonClick();
			return true;
		}

		// 3. Nút Bật/Tắt HUD Map
		if (px >= uiX + 236 && px <= uiX + 278 && py >= uiY + 6 && py <= uiY + 28)
		{
			ModMapEntityHUD.isShowMapEntityHUD = !ModMapEntityHUD.isShowMapEntityHUD;
			ModConfig.SaveConfig();
			SoundMn.gI().buttonClick();
			return true;
		}

		// 4. Nút Bật/Tắt Né Broly
		if (px >= uiX + 68 && px <= uiX + 110 && py >= uiY + 28 && py <= uiY + 50)
		{
			ModKiteBroly.ToggleAutoKite();
			return true;
		}

		// 5. Nút Bật/Tắt Khinh Công
		if (px >= uiX + 182 && px <= uiX + 224 && py >= uiY + 28 && py <= uiY + 50)
		{
			ModKiteBroly.ToggleKhinhCong();
			return true;
		}

		// 6. Nút Đổi Khoảng Cách An Toàn
		if (px >= uiX + 230 && px <= uiX + 310 && py >= uiY + 28 && py <= uiY + 50)
		{
			ModKiteBroly.CycleSafeDistance();
			return true;
		}

		int listX = uiX + 6;
		int listY = uiY + 68;
		int listW = uiW - 12;
		lock (ModBossNotice.listBossNotices)
		{
			int displayCount = (ModBossNotice.listBossNotices.Count < 7) ? ModBossNotice.listBossNotices.Count : 7;
			for (int i = 0; i < displayCount; i++)
			{
				ModBossNotice.BossNoticeEntry entry = ModBossNotice.listBossNotices[i];
				if (entry == null || entry.isDefeated) continue;

				int rowY = listY + 4 + i * 20;
				// Bấm vào nút "Đến" của Boss
				if (px >= listX + listW - 40 && px <= listX + listW - 4 && py >= rowY && py <= rowY + 18)
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
