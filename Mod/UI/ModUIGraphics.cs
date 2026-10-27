using System;
using UnityEngine;

public static class ModUIGraphics
{
	public const int ContentHeight = 446;
	private static readonly int[] fpsOptions = new int[8] { 30, 60, 90, 120, 144, 165, 185, 240 };

	public static void Paint(int uiX, int uiY, int uiW, int uiH, mGraphics g)
	{
		mFont.tahoma_7_grey.drawString(g, "Toàn màn hình:", uiX + 6, uiY + 7, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 88, uiY + 4, 44, 16, ModGraphics.isFullscreen ? "BẬT" : "TẮT", ModGraphics.isFullscreen, g);
		mFont.tahoma_7_grey.drawString(g, "(F11 / Alt+Enter)", uiX + 138, uiY + 7, mFont.LEFT);

		mFont.tahoma_7_grey.drawString(g, "Độ phân giải cửa sổ:", uiX + 6, uiY + 28, mFont.LEFT);
		int rBtnW = (uiW - 18) / 4;
		for (int r = 0; r < 4; r++)
		{
			int rx = uiX + 5 + r * (rBtnW + 3);
			bool isSelRes = (!ModGraphics.isFullscreen && ModGraphics.resolutionIndex == r);
			ModUI.PaintNativeButton(rx, uiY + 43, rBtnW, 16, ModGraphics.resolutionNames[r], isSelRes, g);
		}

		mFont.tahoma_7_grey.drawString(g, "Chất lượng đồ họa (GPU Khử răng cưa):", uiX + 6, uiY + 66, mFont.LEFT);
		int qCount = ModGraphics.graphicsNames.Length;
		int qBtnW = (uiW - 10 - (qCount - 1) * 3) / qCount;
		for (int q = 0; q < qCount; q++)
		{
			int qx = uiX + 5 + q * (qBtnW + 3);
			ModUI.PaintNativeButton(qx, uiY + 81, qBtnW, 16, ModGraphics.graphicsNames[q], ModGraphics.graphicsQuality == q, g);
		}

		int descY = uiY + 101;
		if (ModGraphics.graphicsQuality == 0)
		{
			mFont.tahoma_7_grey.drawString(g, "* Ultra: GPU khử răng cưa 4x, texture sắc nét.", uiX + 6, descY, mFont.LEFT);
		}
		else if (ModGraphics.graphicsQuality == 1)
		{
			mFont.tahoma_7_grey.drawString(g, "* Medium: Tối ưu mượt, xóa hiệu ứng chiêu & thời tiết.", uiX + 6, descY, mFont.LEFT);
		}
		else if (ModGraphics.graphicsQuality == 2)
		{
			mFont.tahoma_7_grey.drawString(g, "* Low: Xóa background, phông nền sáng siêu nhẹ.", uiX + 6, descY, mFont.LEFT);
		}
		else if (ModGraphics.graphicsQuality == 3)
		{
			mFont.tahoma_7_green2.drawString(g, "* Super Low: Xóa cây cỏ trang trí, chỉ giữ base map.", uiX + 6, descY, mFont.LEFT);
		}
		else if (ModGraphics.graphicsQuality == 4)
		{
			mFont.tahoma_7_green2.drawString(g, "* Hình Học: Khung lưới địa hình & hitbox quái siêu nhẹ.", uiX + 6, descY, mFont.LEFT);
		}

		mFont.tahoma_7_grey.drawString(g, "Chế Độ Đồ Hoạ (F10):", uiX + 6, uiY + 116, mFont.LEFT);
		int mCount = ModGraphics.renderModeNames.Length;
		int mBtnW = (uiW - 10 - (mCount - 1) * 4) / mCount;
		for (int m = 0; m < mCount; m++)
		{
			int mx = uiX + 5 + m * (mBtnW + 4);
			ModUI.PaintNativeButton(mx, uiY + 131, mBtnW, 16, ModGraphics.renderModeNames[m], ModGraphics.renderMode == m, g);
		}

		int bdDescY = uiY + 151;
		if (ModGraphics.renderMode == 0)
		{
			mFont.tahoma_7_green2.drawString(g, "* Đồ Hoạ Java: Tài nguyên x1 & font bitmap từ src Java gốc.", uiX + 6, bdDescY, mFont.LEFT);
		}
		else if (ModGraphics.renderMode == 2)
		{
			mFont.tahoma_7_green2.drawString(g, "* Đồ Hoạ iOS: Tài nguyên Retina x4/x3 & BMFont gốc từ IPA iOS.", uiX + 6, bdDescY, mFont.LEFT);
		}
		else
		{
			mFont.tahoma_7_green2.drawString(g, "* Đồ Hoạ HD: Tài nguyên x2 sắc nét, lọc mượt & font hệ thống.", uiX + 6, bdDescY, mFont.LEFT);
		}

		mFont.tahoma_7_grey.drawString(g, "Auto FPS:", uiX + 6, uiY + 168, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 62, uiY + 165, 44, 16, ModFps.isAutoFps ? "BẬT" : "TẮT", ModFps.isAutoFps, g);
		mFont.tahoma_7_grey.drawString(g, "(Khớp tần số quét)", uiX + 112, uiY + 168, mFont.LEFT);

		for (int f = 0; f < fpsOptions.Length; f++)
		{
			int col = f % 4;
			int row = f / 4;
			int fx = uiX + 5 + col * (rBtnW + 3);
			int fy = uiY + 187 + row * 20;
			bool isSel = (!ModFps.isAutoFps && ModFps.targetFps == fpsOptions[f]);
			ModUI.PaintNativeButton(fx, fy, rBtnW, 16, fpsOptions[f] + " FPS", isSel, g);
		}

		mFont.tahoma_7_green2.drawString(g, "FPS: " + Main.realFPS + " | Màn: " + ModFps.GetDeviceMaxRefreshRate() + "Hz | Cửa sổ: " + Screen.width + "x" + Screen.height, uiX + uiW / 2, uiY + 233, mFont.CENTER);

		int curAnimIdx = (ModGraphics.animSpeedIndex >= 0 && ModGraphics.animSpeedIndex < ModGraphics.animSpeedNames.Length) ? ModGraphics.animSpeedIndex : 0;
		mFont.tahoma_7_grey.drawString(g, "Tốc độ hoạt ảnh game:", uiX + 6, uiY + 250, mFont.LEFT);
		mFont.tahoma_7_green2.drawString(g, ModGraphics.animSpeedNames[curAnimIdx] + " (" + (int)(50f * ModGraphics.GetAnimSpeed() + 0.5f) + "Hz)", uiX + 122, uiY + 250, mFont.LEFT);
		for (int a = 0; a < ModGraphics.animSpeedNames.Length; a++)
		{
			int col = a % 4;
			int row = a / 4;
			int ax = uiX + 5 + col * (rBtnW + 3);
			int ay = uiY + 265 + row * 20;
			ModUI.PaintNativeButton(ax, ay, rBtnW, 16, ModGraphics.animSpeedNames[a], curAnimIdx == a, g);
		}

		mFont.tahoma_7_grey.drawString(g, "Việt Hoá:", uiX + 6, uiY + 310, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 54, uiY + 307, 38, 16, ModConfig.isTranslate ? "BẬT" : "TẮT", ModConfig.isTranslate, g);

		mFont.tahoma_7_grey.drawString(g, "Logo:", uiX + 100, uiY + 310, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 132, uiY + 307, 38, 16, ModLogo.isShowLogoInGame ? "BẬT" : "TẮT", ModLogo.isShowLogoInGame, g);

		mFont.tahoma_7_grey.drawString(g, "Analog:", uiX + 178, uiY + 310, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 220, uiY + 307, 38, 16, (GameScr.isAnalog == 1) ? "BẬT" : "TẮT", GameScr.isAnalog == 1, g);

		int optBtnW = (uiW - 15) / 2;
		ModUI.PaintNativeButton(uiX + 5, uiY + 329, optBtnW, 16, "Xóa Người: " + (ModGraphics.hideOtherPlayers ? "BẬT" : "TẮT"), ModGraphics.hideOtherPlayers, g);
		ModUI.PaintNativeButton(uiX + 10 + optBtnW, uiY + 329, optBtnW, 16, "Treo Giảm CPU: " + (ModGraphics.isAfkLowCpu ? "BẬT" : "TẮT"), ModGraphics.isAfkLowCpu, g);

		ModUI.PaintNativeButton(uiX + 5, uiY + 349, optBtnW, 16, "Nút Nhanh Màn Hình: " + (ModArrowButton.showExtraQuickButtons ? "BẬT" : "TẮT"), ModArrowButton.showExtraQuickButtons, g);
		ModUI.PaintNativeButton(uiX + 10 + optBtnW, uiY + 349, optBtnW, 16, "Phím Tắt Nhanh PC: " + (ModHotkey.enableQuickPcHotkeys ? "BẬT" : "TẮT"), ModHotkey.enableQuickPcHotkeys, g);

		string bgBtnText = "HÌNH NỀN PHONG CẢNH (GIT)" + (ModBackground.isCustomBGActive ? " [ĐANG BẬT]" : "");
		ModUI.PaintNativeButton(uiX + 6, uiY + 373, uiW - 12, 18, bgBtnText, ModBackground.isCustomBGActive, g);

		ModUI.PaintNativeButton(uiX + 6, uiY + 395, optBtnW, 18, "HUD Sư Phụ: " + (ModMapEntityHUD.isShowMasterHUD ? "BẬT" : "TẮT"), ModMapEntityHUD.isShowMasterHUD, g);
		ModUI.PaintNativeButton(uiX + 10 + optBtnW, uiY + 395, optBtnW, 18, "Màn Đen (F9): " + (ModGraphics.isAfkBlackScreen ? "BẬT" : "TẮT"), ModGraphics.isAfkBlackScreen, g);
	}

	public static bool HandleTap(int px, int py, int uiX, int uiY, int uiW, int uiH)
	{
		if (px >= uiX + 88 && px <= uiX + 132 && py >= uiY + 3 && py <= uiY + 21)
		{
			ModGraphics.ToggleFullscreen();
			SoundMn.gI().buttonClick();
			return true;
		}

		int rBtnW = (uiW - 18) / 4;
		for (int r = 0; r < 4; r++)
		{
			int rx = uiX + 5 + r * (rBtnW + 3);
			if (px >= rx && px <= rx + rBtnW && py >= uiY + 42 && py <= uiY + 60)
			{
				ModGraphics.ApplyResolution(r);
				SoundMn.gI().buttonClick();
				return true;
			}
		}

		int qCount = ModGraphics.graphicsNames.Length;
		int qBtnW = (uiW - 10 - (qCount - 1) * 3) / qCount;
		for (int q = 0; q < qCount; q++)
		{
			int qx = uiX + 5 + q * (qBtnW + 3);
			if (px >= qx && px <= qx + qBtnW && py >= uiY + 80 && py <= uiY + 98)
			{
				ModGraphics.SetQuality(q);
				SoundMn.gI().buttonClick();
				return true;
			}
		}

		int mCount = ModGraphics.renderModeNames.Length;
		int mBtnW = (uiW - 10 - (mCount - 1) * 4) / mCount;
		for (int m = 0; m < mCount; m++)
		{
			int mx = uiX + 5 + m * (mBtnW + 4);
			if (px >= mx && px <= mx + mBtnW && py >= uiY + 130 && py <= uiY + 148)
			{
				ModGraphics.SetRenderMode(m);
				GameScr.info1.addInfo("Chế độ đồ hoạ: " + ModGraphics.renderModeNames[m], 0);
				SoundMn.gI().buttonClick();
				return true;
			}
		}

		if (px >= uiX + 62 && px <= uiX + 106 && py >= uiY + 164 && py <= uiY + 182)
		{
			if (ModFps.isAutoFps) ModFps.SetFPS(144);
			else ModFps.SetAutoFPS();
			SoundMn.gI().buttonClick();
			return true;
		}

		for (int f = 0; f < fpsOptions.Length; f++)
		{
			int col = f % 4;
			int row = f / 4;
			int fx = uiX + 5 + col * (rBtnW + 3);
			int fy = uiY + 187 + row * 20;
			if (px >= fx && px <= fx + rBtnW && py >= fy && py <= fy + 18)
			{
				ModFps.SetFPS(fpsOptions[f]);
				SoundMn.gI().buttonClick();
				return true;
			}
		}

		for (int a = 0; a < ModGraphics.animSpeedNames.Length; a++)
		{
			int col = a % 4;
			int row = a / 4;
			int ax = uiX + 5 + col * (rBtnW + 3);
			int ay = uiY + 265 + row * 20;
			if (px >= ax && px <= ax + rBtnW && py >= ay && py <= ay + 18)
			{
				ModGraphics.SetAnimSpeed(a);
				if (GameScr.info1 != null) GameScr.info1.addInfo("Tốc độ hoạt ảnh game: " + ModGraphics.animSpeedNames[a], 0);
				SoundMn.gI().buttonClick();
				return true;
			}
		}

		if (px >= uiX + 54 && px <= uiX + 92 && py >= uiY + 306 && py <= uiY + 324)
		{
			ModConfig.isTranslate = !ModConfig.isTranslate;
			ModConfig.SaveConfig();
			ModTranslate.ApplyAllTranslations();
			GameScr.info1.addInfo("Dịch Việt Hoá: " + (ModConfig.isTranslate ? "BẬT" : "TẮT (Gốc Server)"), 0);
			SoundMn.gI().buttonClick();
			return true;
		}

		if (px >= uiX + 132 && px <= uiX + 170 && py >= uiY + 306 && py <= uiY + 324)
		{
			ModLogo.isShowLogoInGame = !ModLogo.isShowLogoInGame;
			ModConfig.SaveConfig();
			SoundMn.gI().buttonClick();
			return true;
		}

		if (px >= uiX + 220 && px <= uiX + 258 && py >= uiY + 306 && py <= uiY + 324)
		{
			GameScr.isAnalog = (GameScr.isAnalog == 1) ? 0 : 1;
			GameScr.setSkillBarPosition();
			Rms.saveRMSInt("analog", GameScr.isAnalog);
			ModConfig.SaveConfig();
			GameScr.info1.addInfo("Bàn phím ảo & Analog: " + ((GameScr.isAnalog == 1) ? "BẬT" : "TẮT"), 0);
			SoundMn.gI().buttonClick();
			return true;
		}

		int optBtnW = (uiW - 15) / 2;
		if (py >= uiY + 328 && py <= uiY + 346)
		{
			if (px >= uiX + 5 && px <= uiX + 5 + optBtnW)
			{
				ModGraphics.ToggleHideOtherPlayers();
				return true;
			}
			if (px >= uiX + 10 + optBtnW && px <= uiX + 10 + optBtnW * 2)
			{
				ModGraphics.ToggleAfkLowCpu();
				SoundMn.gI().buttonClick();
				return true;
			}
		}

		if (py >= uiY + 348 && py <= uiY + 366)
		{
			if (px >= uiX + 5 && px <= uiX + 5 + optBtnW)
			{
				ModArrowButton.showExtraQuickButtons = !ModArrowButton.showExtraQuickButtons;
				ModConfig.SaveConfig();
				GameScr.info1.addInfo("Nút tác vụ nhanh trên màn hình: " + (ModArrowButton.showExtraQuickButtons ? "BẬT" : "TẮT"), 0);
				SoundMn.gI().buttonClick();
				return true;
			}
			if (px >= uiX + 10 + optBtnW && px <= uiX + 10 + optBtnW * 2)
			{
				ModHotkey.enableQuickPcHotkeys = !ModHotkey.enableQuickPcHotkeys;
				ModConfig.SaveConfig();
				GameScr.info1.addInfo("Phím tắt nhanh PC (E,C,F,G,I,J,K,L,M,N,T,X,Z): " + (ModHotkey.enableQuickPcHotkeys ? "BẬT" : "TẮT"), 0);
				SoundMn.gI().buttonClick();
				return true;
			}
		}

		if (px >= uiX + 6 && px <= uiX + uiW - 6 && py >= uiY + 372 && py <= uiY + 393)
		{
			ModUIBackground.isOpen = true;
			ModUI.detailScrollY = 0;
			SoundMn.gI().buttonClick();
			return true;
		}
		if (py >= uiY + 394 && py <= uiY + 414)
		{
			if (px >= uiX + 6 && px <= uiX + 6 + optBtnW)
			{
				ModMapEntityHUD.ToggleMasterHUD();
				return true;
			}
			if (px >= uiX + 10 + optBtnW && px <= uiX + 10 + optBtnW * 2)
			{
				ModGraphics.ToggleAfkBlackScreen();
				if (ModGraphics.isAfkBlackScreen)
				{
					ModMenu.CloseMenu();
				}
				return true;
			}
		}

		return false;
	}
}
