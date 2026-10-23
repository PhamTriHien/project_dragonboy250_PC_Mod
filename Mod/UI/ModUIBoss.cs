using System;

/// <summary>
/// Giao diện quản lý Boss Mod: Hiển thị danh sách Boss xuất hiện trong map, thông báo Boss toàn server, tự dò khu và nút bay đến.
/// </summary>
public static class ModUIBoss
{
	/// <summary>
	/// Thực hiện vẽ toàn bộ đối tượng và các lớp hiển thị lên canvas mGraphics.
	/// </summary>
	public static void Paint(int uiX, int uiY, int uiW, int uiH, mGraphics g)
	{
		int halfW = (uiW - 14) / 2;
		int item1X = uiX + 4;
		int item2X = uiX + 8 + halfW;

		// Hàng 1: Báo Boss, Xóa List & HUD Map
		mFont.tahoma_7_grey.drawString(g, "Báo Boss:", item1X + 2, uiY + 7, mFont.LEFT);
		ModUI.PaintNativeButton(item1X + 56, uiY + 4, 36, 17, ModBossNotice.isShowBossNotice ? "BẬT" : "TẮT", ModBossNotice.isShowBossNotice, g);
		ModUI.PaintNativeButton(item1X + 95, uiY + 4, 36, 17, "Xóa", false, g);

		mFont.tahoma_7_grey.drawString(g, "HUD Map:", item2X + 2, uiY + 7, mFont.LEFT);
		ModUI.PaintNativeButton(item2X + halfW - 38, uiY + 4, 36, 17, ModMapEntityHUD.isShowMapEntityHUD ? "BẬT" : "TẮT", ModMapEntityHUD.isShowMapEntityHUD, g);

		// Hàng 2: Auto Né Broly, Khinh Công, Khoảng cách an toàn (3 nút đồng đều)
		int r2W = (uiW - 16) / 3;
		ModUI.PaintNativeButton(uiX + 4, uiY + 23, r2W, 16, "Né Broly: " + (ModKiteBroly.isAutoKite ? "BẬT" : "TẮT"), ModKiteBroly.isAutoKite, g);
		ModUI.PaintNativeButton(uiX + 8 + r2W, uiY + 23, r2W, 16, "Khinh công: " + (ModKiteBroly.isKhinhCong ? "BẬT" : "TẮT"), ModKiteBroly.isKhinhCong, g);
		ModUI.PaintNativeButton(uiX + 12 + r2W * 2, uiY + 23, r2W, 16, "KC: " + ModKiteBroly.safeDistance + "p", ModKiteBroly.safeDistance > 120, g);

		// Hàng 3: Tự Dò Khu Boss, Tự Săn Boss, Tự Đánh Broly (3 nút đồng đều)
		ModUI.PaintNativeButton(uiX + 4, uiY + 40, r2W, 16, "Dò Khu: " + (ModBossNotice.isAutoScanBossZone ? "BẬT" : "TẮT"), ModBossNotice.isAutoScanBossZone, g);
		ModUI.PaintNativeButton(uiX + 8 + r2W, uiY + 40, r2W, 16, "Săn Boss: " + (ModBossNotice.isAutoAttackBoss ? "BẬT" : "TẮT"), ModBossNotice.isAutoAttackBoss, g);
		ModUI.PaintNativeButton(uiX + 12 + r2W * 2, uiY + 40, r2W, 16, "Đánh Broly: " + (ModKiteBroly.autoAttackBroly ? "BẬT" : "TẮT"), ModKiteBroly.autoAttackBroly, g);

		// Hàng 4: Lọc Boss, Gim Mục Tiêu, KS Broly 1HP
		ModUI.PaintNativeButton(uiX + 4, uiY + 57, r2W, 16, string.IsNullOrEmpty(ModBossNotice.filterBossNamesRaw) ? "Lọc Boss: Tất cả" : "Lọc: Đang bật", !string.IsNullOrEmpty(ModBossNotice.filterBossNamesRaw), g);
		ModUI.PaintNativeButton(uiX + 8 + r2W, uiY + 57, r2W, 16, "Gim M.Tiêu: " + (ModMapEntityHUD.isLockTarget ? "BẬT" : "TẮT"), ModMapEntityHUD.isLockTarget, g);
		ModUI.PaintNativeButton(uiX + 12 + r2W * 2, uiY + 57, r2W, 16, "KS Broly 1HP: " + (ModBossNotice.isKsSuperBroly1Hp ? "BẬT" : "TẮT"), ModBossNotice.isKsSuperBroly1Hp, g);

		int listX = uiX + 4;
		int listY = uiY + 75;
		int listW = uiW - 8;
		int listH = uiH - 79;
		if (listH < 40) listH = 40;
		GameCanvas.paintz.paintFrameSimple(listX, listY, listW, listH, g);
		g.setColor(15196114);
		g.fillRect(listX + 2, listY + 2, listW - 4, listH - 4);

		g.setClip(listX + 2, listY + 2, listW - 4, listH - 4);
		lock (ModBossNotice.listBossNotices)
		{
			if (ModBossNotice.listBossNotices.Count == 0)
			{
				mFont.tahoma_7_grey.drawString(g, "(Chưa nhận được thông báo boss nào từ server)", listX + listW / 2, listY + listH / 2 - 8, mFont.CENTER);
			}
			else
			{
				int displayCount = (ModBossNotice.listBossNotices.Count < 8) ? ModBossNotice.listBossNotices.Count : 8;
				for (int i = 0; i < displayCount; i++)
				{
					ModBossNotice.BossNoticeEntry entry = ModBossNotice.listBossNotices[i];
					if (entry == null) continue;
					int rowY = listY + 3 + i * 16;
					if (rowY + 14 > listY + listH - 12) break;

					int textClipW = entry.isDefeated ? (listW - 8) : (listW - 38);
					g.setClip(listX + 2, rowY, textClipW, 16);

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

					g.setClip(listX + 2, listY + 2, listW - 4, listH - 4);
					// Nút "Đến" nhanh cho Boss đang còn sống
					if (!entry.isDefeated)
					{
						ModUI.PaintNativeButton(listX + listW - 32, rowY, 28, 15, "Đến", false, g);
					}
				}
			}
			(ModKiteBroly.currentBrolyDist >= 0 ? mFont.tahoma_7_green2 : mFont.tahoma_7_grey).drawString(g, "Broly: " + ModKiteBroly.brolyStatusText, listX + 6, listY + listH - 13, mFont.LEFT);
		}
		g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
	}

	/// <summary>
	/// Xử lý nghiệp vụ logic chuyên trách cho HandleTap.
	/// </summary>
	public static bool HandleTap(int px, int py, int uiX, int uiY, int uiW, int uiH)
	{
		int halfW = (uiW - 14) / 2;
		int item1X = uiX + 4;
		int item2X = uiX + 8 + halfW;

		// 1. Nút Bật/Tắt HUD Báo Boss
		if (px >= item1X + 56 && px <= item1X + 92 && py >= uiY + 3 && py <= uiY + 21)
		{
			ModBossNotice.isShowBossNotice = !ModBossNotice.isShowBossNotice;
			ModConfig.SaveConfig();
			SoundMn.gI().buttonClick();
			return true;
		}

		// 2. Nút Xóa List
		if (px >= item1X + 95 && px <= item1X + 131 && py >= uiY + 3 && py <= uiY + 21)
		{
			lock (ModBossNotice.listBossNotices)
			{
				ModBossNotice.listBossNotices.Clear();
			}
			SoundMn.gI().buttonClick();
			return true;
		}

		// 3. Nút Bật/Tắt HUD Map
		if (px >= item2X + halfW - 38 && px <= item2X + halfW && py >= uiY + 3 && py <= uiY + 21)
		{
			ModMapEntityHUD.isShowMapEntityHUD = !ModMapEntityHUD.isShowMapEntityHUD;
			ModConfig.SaveConfig();
			SoundMn.gI().buttonClick();
			return true;
		}

		// 4. Hàng 2: Né Broly, Khinh công, Khoảng cách
		int r2W = (uiW - 16) / 3;
		if (py >= uiY + 22 && py <= uiY + 39)
		{
			if (px >= uiX + 4 && px <= uiX + 4 + r2W)
			{
				ModKiteBroly.ToggleAutoKite();
				return true;
			}

			if (px >= uiX + 8 + r2W && px <= uiX + 8 + r2W * 2)
			{
				ModKiteBroly.ToggleKhinhCong();
				return true;
			}

			if (px >= uiX + 12 + r2W * 2 && px <= uiX + 12 + r2W * 3)
			{
				ModKiteBroly.CycleSafeDistance();
				ModConfig.SaveConfig();
				SoundMn.gI().buttonClick();
				return true;
			}
		}

		// 5. Hàng 3: Dò Khu Boss, Săn Boss, Đánh Broly
		if (py >= uiY + 39 && py <= uiY + 57)
		{
			if (px >= uiX + 4 && px <= uiX + 4 + r2W)
			{
				ModBossNotice.ToggleAutoScanBossZone();
				return true;
			}

			if (px >= uiX + 8 + r2W && px <= uiX + 8 + r2W * 2)
			{
				ModBossNotice.ToggleAutoAttackBoss();
				return true;
			}

			if (px >= uiX + 12 + r2W * 2 && px <= uiX + 12 + r2W * 3)
			{
				ModKiteBroly.autoAttackBroly = !ModKiteBroly.autoAttackBroly;
				ModConfig.SaveConfig();
				GameScr.info1.addInfo("Tự đánh Broly: " + (ModKiteBroly.autoAttackBroly ? "BẬT" : "TẮT"), 0);
				SoundMn.gI().buttonClick();
				return true;
			}
		}

		// 5b. Hàng 4: Lọc Boss, Gim Mục Tiêu, KS Broly 1HP
		if (py >= uiY + 56 && py <= uiY + 74)
		{
			if (px >= uiX + 4 && px <= uiX + 4 + r2W)
			{
				ModUI.uiCustomOpen = false;
				ModBossNotice.ShowInputFilterBoss();
				SoundMn.gI().buttonClick();
				return true;
			}
			if (px >= uiX + 8 + r2W && px <= uiX + 8 + r2W * 2)
			{
				ModMapEntityHUD.ToggleLockTarget();
				return true;
			}
			if (px >= uiX + 12 + r2W * 2 && px <= uiX + 12 + r2W * 3)
			{
				ModBossNotice.isKsSuperBroly1Hp = !ModBossNotice.isKsSuperBroly1Hp;
				ModConfig.SaveConfig();
				GameScr.info1.addInfo("KS Super Broly 1HP: " + (ModBossNotice.isKsSuperBroly1Hp ? "BẬT" : "TẮT"), 0);
				SoundMn.gI().buttonClick();
				return true;
			}
		}

		// 6. Click vào nút "Đến" ở từng dòng Boss
		int listX = uiX + 4;
		int listY = uiY + 75;
		int listW = uiW - 8;
		int listH = uiH - 79;

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
