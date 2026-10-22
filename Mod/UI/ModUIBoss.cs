using System;

public static class ModUIBoss
{
	public static void Paint(int uiX, int uiY, int uiW, int uiH, mGraphics g)
	{
		int halfW = (uiW - 14) / 2;
		int item1X = uiX + 4;
		int item2X = uiX + 8 + halfW;

		// Hàng 1: Báo Boss, Xóa List & HUD Map
		mFont.tahoma_7_grey.drawString(g, "Báo Boss:", item1X + 2, uiY + 7, mFont.LEFT);
		ModUI.PaintNativeButton(item1X + 50, uiY + 4, 34, 17, ModBossNotice.isShowBossNotice ? "BẬT" : "TẮT", ModBossNotice.isShowBossNotice, g);
		ModUI.PaintNativeButton(item1X + 88, uiY + 4, 38, 17, "Xóa", false, g);

		mFont.tahoma_7_grey.drawString(g, "HUD Map:", item2X + 2, uiY + 7, mFont.LEFT);
		ModUI.PaintNativeButton(item2X + halfW - 38, uiY + 4, 36, 17, ModMapEntityHUD.isShowMapEntityHUD ? "BẬT" : "TẮT", ModMapEntityHUD.isShowMapEntityHUD, g);

		// Hàng 2: Auto Né Broly, Khinh Công, Khoảng cách an toàn
		int r2W = (uiW - 16) / 3;
		mFont.tahoma_7_grey.drawString(g, "Né Broly:", uiX + 4, uiY + 26, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 4 + r2W - 36, uiY + 23, 34, 17, ModKiteBroly.isAutoKite ? "BẬT" : "TẮT", ModKiteBroly.isAutoKite, g);

		mFont.tahoma_7_grey.drawString(g, "Khinh công:", uiX + 8 + r2W, uiY + 26, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 8 + r2W * 2 - 36, uiY + 23, 34, 17, ModKiteBroly.isKhinhCong ? "BẬT" : "TẮT", ModKiteBroly.isKhinhCong, g);

		ModUI.PaintNativeButton(uiX + 12 + r2W * 2, uiY + 23, r2W - 4, 17, "KC:" + ModKiteBroly.safeDistance + "p", ModKiteBroly.safeDistance > 120, g);

		// Dòng trạng thái Broly thời gian thực
		(ModKiteBroly.currentBrolyDist >= 0 ? mFont.tahoma_7_green2 : mFont.tahoma_7_grey).drawString(g, "Broly: " + ModKiteBroly.brolyStatusText, uiX + 6, uiY + 44, mFont.LEFT);

		int listX = uiX + 4;
		int listY = uiY + 57;
		int listW = uiW - 8;
		int listH = uiH - 61;
		if (listH < 40) listH = 40;
		GameCanvas.paintz.paintFrameSimple(listX, listY, listW, listH, g);
		g.setColor(15196114);
		g.fillRect(listX + 2, listY + 2, listW - 4, listH - 4);

		lock (ModBossNotice.listBossNotices)
		{
			if (ModBossNotice.listBossNotices.Count == 0)
			{
				mFont.tahoma_7_grey.drawString(g, "(Chưa nhận được thông báo boss nào từ server)", listX + listW / 2, listY + listH / 2 - 4, mFont.CENTER);
			}
			else
			{
				int displayCount = (ModBossNotice.listBossNotices.Count < 8) ? ModBossNotice.listBossNotices.Count : 8;
				for (int i = 0; i < displayCount; i++)
				{
					ModBossNotice.BossNoticeEntry entry = ModBossNotice.listBossNotices[i];
					if (entry == null) continue;
					int rowY = listY + 3 + i * 16;
					if (rowY + 14 > listY + listH) break;

					g.setColor(entry.isDefeated ? 0x888888 : 0xcc2200);
					g.fillRect(listX + 4, rowY + 3, 3, 10);

					int curX = listX + 10;
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
						ModUI.PaintNativeButton(listX + listW - 32, rowY, 28, 15, "Đến", false, g);
					}
				}
			}
		}
	}

	public static bool HandleTap(int px, int py, int uiX, int uiY, int uiW, int uiH)
	{
		int halfW = (uiW - 14) / 2;
		int item1X = uiX + 5;
		int item2X = uiX + 9 + halfW;

		// 1. Nút Bật/Tắt HUD Báo Boss
		if (px >= item1X + 50 && px <= item1X + 84 && py >= uiY + 2 && py <= uiY + 20)
		{
			ModBossNotice.isShowBossNotice = !ModBossNotice.isShowBossNotice;
			ModConfig.SaveConfig();
			SoundMn.gI().buttonClick();
			return true;
		}

		// 2. Nút Xóa List
		if (px >= item1X + 88 && px <= item1X + 126 && py >= uiY + 2 && py <= uiY + 20)
		{
			lock (ModBossNotice.listBossNotices)
			{
				ModBossNotice.listBossNotices.Clear();
			}
			SoundMn.gI().buttonClick();
			return true;
		}

		// 3. Nút Bật/Tắt HUD Map
		if (px >= item2X + halfW - 36 && px <= item2X + halfW + 2 && py >= uiY + 2 && py <= uiY + 20)
		{
			ModMapEntityHUD.isShowMapEntityHUD = !ModMapEntityHUD.isShowMapEntityHUD;
			ModConfig.SaveConfig();
			SoundMn.gI().buttonClick();
			return true;
		}

		// 4. Hàng 2
		int r2W = (uiW - 16) / 3;
		if (px >= uiX + 5 + r2W - 36 && px <= uiX + 5 + r2W && py >= uiY + 20 && py <= uiY + 38)
		{
			ModKiteBroly.isAutoKite = !ModKiteBroly.isAutoKite;
			ModConfig.SaveConfig();
			GameScr.info1.addInfo("Né Broly: " + (ModKiteBroly.isAutoKite ? "BẬT" : "TẮT"), 0);
			SoundMn.gI().buttonClick();
			return true;
		}

		if (px >= uiX + 8 + r2W * 2 - 36 && px <= uiX + 8 + r2W * 2 && py >= uiY + 20 && py <= uiY + 38)
		{
			ModKiteBroly.isKhinhCong = !ModKiteBroly.isKhinhCong;
			ModConfig.SaveConfig();
			GameScr.info1.addInfo("Khinh công Broly: " + (ModKiteBroly.isKhinhCong ? "BẬT" : "TẮT"), 0);
			SoundMn.gI().buttonClick();
			return true;
		}

		if (px >= uiX + 11 + r2W * 2 && px <= uiX + 11 + r2W * 3 && py >= uiY + 20 && py <= uiY + 38)
		{
			ModKiteBroly.CycleSafeDistance();
			ModConfig.SaveConfig();
			SoundMn.gI().buttonClick();
			return true;
		}

		// 5. Click vào nút "Đến" ở từng dòng Boss
		int listX = uiX + 5;
		int listY = uiY + 54;
		int listW = uiW - 10;
		int listH = uiH - 58;

		if (px >= listX + listW - 34 && px <= listX + listW && py >= listY && py <= listY + listH)
		{
			lock (ModBossNotice.listBossNotices)
			{
				int displayCount = (ModBossNotice.listBossNotices.Count < 8) ? ModBossNotice.listBossNotices.Count : 8;
				for (int i = 0; i < displayCount; i++)
				{
					int rowY = listY + 3 + i * 16;
					if (py >= rowY && py <= rowY + 16)
					{
						if (i < ModBossNotice.listBossNotices.Count)
						{
							ModBossNotice.BossNoticeEntry entry = ModBossNotice.listBossNotices[i];
							if (entry != null && !entry.isDefeated)
							{
								int targetMapId = ModNextMapData.FindMapIdByName(entry.mapName);
								if (targetMapId >= 0)
								{
									ModNextMap.StartNextMap(targetMapId);
									ModUI.uiCustomOpen = false;
									GameScr.info1.addInfo("Đang di chuyển đến: " + entry.mapName, 0);
									SoundMn.gI().buttonClick();
									return true;
								}
							}
						}
						break;
					}
				}
			}
		}

		return false;
	}
}
