using System;

public static class ModUIBoss
{
	public static void Paint(int uiX, int uiY, int uiW, int uiH, mGraphics g)
	{
		mFont.tahoma_7b_white.drawString(g, "Báo Boss:", uiX + 18, uiY + 52, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 75, uiY + 48, 42, 18, ModBossNotice.isShowBossNotice ? "BẬT" : "TẮT", ModBossNotice.isShowBossNotice, g);
		ModUI.PaintNativeButton(uiX + 121, uiY + 48, 62, 18, "Xóa List", false, g);

		mFont.tahoma_7b_white.drawString(g, "HUD Map:", uiX + 190, uiY + 52, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 242, uiY + 48, 42, 18, ModMapEntityHUD.isShowMapEntityHUD ? "BẬT" : "TẮT", ModMapEntityHUD.isShowMapEntityHUD, g);

		// Hàng 2: Auto Né Broly, Khinh Công, Khoảng cách an toàn
		mFont.tahoma_7b_white.drawString(g, "Né Broly:", uiX + 18, uiY + 72, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 75, uiY + 68, 42, 18, ModKiteBroly.isAutoKite ? "BẬT" : "TẮT", ModKiteBroly.isAutoKite, g);

		mFont.tahoma_7b_white.drawString(g, "Khinh Công:", uiX + 125, uiY + 72, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 190, uiY + 68, 42, 18, ModKiteBroly.isKhinhCong ? "BẬT" : "TẮT", ModKiteBroly.isKhinhCong, g);

		ModUI.PaintNativeButton(uiX + 242, uiY + 68, 80, 18, "KC: " + ModKiteBroly.safeDistance + "px", ModKiteBroly.safeDistance > 120, g);

		// Dòng trạng thái Broly thời gian thực
		(ModKiteBroly.currentBrolyDist >= 0 ? mFont.tahoma_7b_yellow : mFont.tahoma_7_grey).drawString(g, "Broly: " + ModKiteBroly.brolyStatusText, uiX + 20, uiY + 90, mFont.LEFT);

		int listY = uiY + 104;
		int listW = uiW - 36;
		int listH = 108;
		g.setColor(0x111111);
		g.fillRect(uiX + 18, listY, listW, listH);
		g.setColor(0x444444);
		g.drawRect(uiX + 18, listY, listW, listH);

		lock (ModBossNotice.listBossNotices)
		{
			if (ModBossNotice.listBossNotices.Count == 0)
			{
				mFont.tahoma_7_grey.drawString(g, "(Chưa nhận được thông báo boss nào từ server)", uiX + uiW / 2, listY + 46, mFont.CENTER);
			}
			else
			{
				int displayCount = (ModBossNotice.listBossNotices.Count < 5) ? ModBossNotice.listBossNotices.Count : 5;
				for (int i = 0; i < displayCount; i++)
				{
					ModBossNotice.BossNoticeEntry entry = ModBossNotice.listBossNotices[i];
					if (entry == null) continue;
					int rowY = listY + 4 + i * 19;

					g.setColor(entry.isDefeated ? 0x888888 : 0xff3300);
					g.fillRect(uiX + 22, rowY + 3, 3, 11);

					int curX = uiX + 30;
					if (mFont.tahoma_7_yellow != null)
					{
						(entry.isDefeated ? mFont.tahoma_7_grey : mFont.tahoma_7_yellow).drawString(g, entry.bossName, curX, rowY + 1, mFont.LEFT);
						curX += (entry.isDefeated ? mFont.tahoma_7_grey : mFont.tahoma_7_yellow).getWidth(entry.bossName);
					}

					mFont mapFont = entry.isDefeated ? mFont.tahoma_7_grey : (mFont.tahoma_7_blue ?? mFont.tahoma_7b_blue ?? mFont.tahoma_7_white);
					if (mapFont != null)
					{
						mapFont.drawString(g, " - " + entry.mapName + " - ", curX, rowY + 1, mFont.LEFT);
						curX += mapFont.getWidth(" - " + entry.mapName + " - ");
					}

					if (mFont.tahoma_7_green2 != null)
					{
						string timeAgo = ModBossNotice.GetTimeAgoString(entry.timestamp);
						(entry.isDefeated ? mFont.tahoma_7_grey : mFont.tahoma_7_green2).drawString(g, timeAgo, curX, rowY + 1, mFont.LEFT);
					}

					// Nút "Đến" nhanh cho Boss đang còn sống
					if (!entry.isDefeated)
					{
						ModUI.PaintNativeButton(uiX + listW - 32, rowY + 1, 28, 15, "Đến", false, g);
					}
				}
			}
		}
	}

	public static bool HandleTap(int px, int py, int uiX, int uiY, int uiW, int uiH)
	{
		// 1. Nút Bật/Tắt HUD Báo Boss
		if (px >= uiX + 75 && px <= uiX + 117 && py >= uiY + 48 && py <= uiY + 66)
		{
			ModBossNotice.isShowBossNotice = !ModBossNotice.isShowBossNotice;
			ModConfig.SaveConfig();
			SoundMn.gI().buttonClick();
			return true;
		}

		// 2. Nút Xóa List
		if (px >= uiX + 121 && px <= uiX + 183 && py >= uiY + 48 && py <= uiY + 66)
		{
			lock (ModBossNotice.listBossNotices)
			{
				ModBossNotice.listBossNotices.Clear();
			}
			SoundMn.gI().buttonClick();
			return true;
		}

		// 3. Nút Bật/Tắt HUD Map
		if (px >= uiX + 242 && px <= uiX + 284 && py >= uiY + 48 && py <= uiY + 66)
		{
			ModMapEntityHUD.isShowMapEntityHUD = !ModMapEntityHUD.isShowMapEntityHUD;
			ModConfig.SaveConfig();
			SoundMn.gI().buttonClick();
			return true;
		}

		// 4. Nút Bật/Tắt Né Broly
		if (px >= uiX + 75 && px <= uiX + 117 && py >= uiY + 68 && py <= uiY + 86)
		{
			ModKiteBroly.ToggleAutoKite();
			return true;
		}

		// 5. Nút Bật/Tắt Khinh Công
		if (px >= uiX + 190 && px <= uiX + 232 && py >= uiY + 68 && py <= uiY + 86)
		{
			ModKiteBroly.ToggleKhinhCong();
			return true;
		}

		// 6. Nút Đổi Khoảng Cách An Toàn
		if (px >= uiX + 242 && px <= uiX + 322 && py >= uiY + 68 && py <= uiY + 86)
		{
			ModKiteBroly.CycleSafeDistance();
			return true;
		}

		int listY = uiY + 104;
		int listW = uiW - 36;
		lock (ModBossNotice.listBossNotices)
		{
			int displayCount = (ModBossNotice.listBossNotices.Count < ModBossNotice.MAX_BOSS_NOTICES) ? ModBossNotice.listBossNotices.Count : ModBossNotice.MAX_BOSS_NOTICES;
			for (int i = 0; i < displayCount; i++)
			{
				ModBossNotice.BossNoticeEntry entry = ModBossNotice.listBossNotices[i];
				if (entry == null) continue;
				int rowY = listY + 4 + i * 19;

				// Chỉ nhấn vào nút "Đến" cụ thể, thông báo chữ chỉ dùng để hiển thị không nhận click
				if (!entry.isDefeated && px >= uiX + listW - 35 && px <= uiX + listW - 2 && py >= rowY && py <= rowY + 18)
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
