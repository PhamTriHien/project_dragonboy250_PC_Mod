using System;
using UnityEngine;

public static class ModUIGraphics
{
	private static readonly int[] fpsOptions = new int[8] { 30, 60, 90, 120, 144, 165, 185, 240 };

	public static void Paint(int uiX, int uiY, int uiW, int uiH, mGraphics g)
	{
		// 1. Toàn Màn Hình
		mFont.tahoma_7b_dark.drawString(g, "Toàn màn hình:", uiX + 8, uiY + 10, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 96, uiY + 6, 48, 17, ModGraphics.isFullscreen ? "BẬT" : "TẮT", ModGraphics.isFullscreen, g);
		mFont.tahoma_7_grey.drawString(g, "(F11 / Alt+Enter)", uiX + 150, uiY + 10, mFont.LEFT);

		// 2. Độ Phân Giải Cửa Sổ
		int rBtnW = (uiW - 20) / 4;
		for (int r = 0; r < 4; r++)
		{
			int rx = uiX + 6 + r * (rBtnW + 3);
			bool isSelRes = (!ModGraphics.isFullscreen && ModGraphics.resolutionIndex == r);
			ModUI.PaintNativeButton(rx, uiY + 28, rBtnW, 17, ModGraphics.resolutionNames[r], isSelRes, g);
		}

		// 3. Chế độ đồ họa & Khử răng cưa GPU
		mFont.tahoma_7b_dark.drawString(g, "Chất lượng đồ họa (GPU Khử răng cưa):", uiX + 8, uiY + 50, mFont.LEFT);
		for (int q = 0; q < 4; q++)
		{
			int qx = uiX + 6 + q * (rBtnW + 3);
			ModUI.PaintNativeButton(qx, uiY + 66, rBtnW, 17, ModGraphics.graphicsNames[q], ModGraphics.graphicsQuality == q, g);
		}

		// Mô tả chi tiết mức đồ họa
		if (ModGraphics.graphicsQuality == 0)
		{
			mFont.tahoma_7_grey.drawString(g, "* Ultra: GPU khử răng cưa 4x, texture sắc nét.", uiX + 8, uiY + 87, mFont.LEFT);
		}
		else if (ModGraphics.graphicsQuality == 1)
		{
			mFont.tahoma_7_grey.drawString(g, "* Medium: Tối ưu mượt, xóa hiệu ứng chiêu & thời tiết.", uiX + 8, uiY + 87, mFont.LEFT);
		}
		else if (ModGraphics.graphicsQuality == 2)
		{
			mFont.tahoma_7_grey.drawString(g, "* Low: Xóa background, phông nền sáng siêu nhẹ.", uiX + 8, uiY + 87, mFont.LEFT);
		}
		else if (ModGraphics.graphicsQuality == 3)
		{
			mFont.tahoma_7b_green2.drawString(g, "* Super Low: Xóa cây cỏ trang trí, chỉ giữ base map.", uiX + 8, uiY + 87, mFont.LEFT);
		}

		// 4. Phần FPS
		mFont.tahoma_7b_dark.drawString(g, "Auto FPS:", uiX + 8, uiY + 106, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 68, uiY + 102, 48, 17, ModFps.isAutoFps ? "BẬT" : "TẮT", ModFps.isAutoFps, g);
		mFont.tahoma_7_grey.drawString(g, "(Khớp tần số quét màn hình)", uiX + 122, uiY + 106, mFont.LEFT);

		// Mốc FPS cố định (2 hàng x 4 nút)
		for (int f = 0; f < fpsOptions.Length; f++)
		{
			int col = f % 4;
			int row = f / 4;
			int fx = uiX + 6 + col * (rBtnW + 3);
			int fy = uiY + 124 + row * 20;
			bool isSel = (!ModFps.isAutoFps && ModFps.targetFps == fpsOptions[f]);
			ModUI.PaintNativeButton(fx, fy, rBtnW, 17, fpsOptions[f] + " FPS", isSel, g);
		}

		mFont.tahoma_7b_green2.drawString(g, "FPS: " + Main.realFPS + " | Màn: " + ModFps.GetDeviceMaxRefreshRate() + "Hz | Cửa sổ: " + Screen.width + "x" + Screen.height, uiX + uiW / 2, uiY + 168, mFont.CENTER);

		// 5. Việt Hoá Server Data, Logo & Bàn phím ảo Analog
		mFont.tahoma_7b_dark.drawString(g, "Việt Hoá:", uiX + 8, uiY + 192, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 58, uiY + 188, 42, 18, ModConfig.isTranslate ? "BẬT" : "TẮT", ModConfig.isTranslate, g);

		mFont.tahoma_7b_dark.drawString(g, "Logo:", uiX + 110, uiY + 192, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 144, uiY + 188, 42, 18, ModLogo.isShowLogoInGame ? "BẬT" : "TẮT", ModLogo.isShowLogoInGame, g);

		mFont.tahoma_7b_dark.drawString(g, "Analog:", uiX + 196, uiY + 192, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 242, uiY + 188, 42, 18, (GameScr.isAnalog == 1) ? "BẬT" : "TẮT", GameScr.isAnalog == 1, g);

		// 6. Phong Cảnh Nền Game (Git)
		string bgBtnText = "🌌 HÌNH NỀN PHONG CẢNH (GIT)" + (ModBackground.isCustomBGActive ? " [ĐANG BẬT]" : "");
		ModUI.PaintNativeButton(uiX + 8, uiY + 208, uiW - 16, 18, bgBtnText, ModBackground.isCustomBGActive, g);
	}

	public static bool HandleTap(int px, int py, int uiX, int uiY, int uiW, int uiH)
	{
		// 1. Bật / Tắt Toàn Màn Hình
		if (px >= uiX + 96 && px <= uiX + 144 && py >= uiY + 6 && py <= uiY + 24)
		{
			ModGraphics.ToggleFullscreen();
			SoundMn.gI().buttonClick();
			return true;
		}

		// 2. Chọn Độ Phân Giải
		int rBtnW = (uiW - 20) / 4;
		for (int r = 0; r < 4; r++)
		{
			int rx = uiX + 6 + r * (rBtnW + 3);
			if (px >= rx && px <= rx + rBtnW && py >= uiY + 28 && py <= uiY + 46)
			{
				ModGraphics.ApplyResolution(r);
				SoundMn.gI().buttonClick();
				return true;
			}
		}

		// 3. Chọn Chất Lượng Đồ Họa
		for (int q = 0; q < 4; q++)
		{
			int qx = uiX + 6 + q * (rBtnW + 3);
			if (px >= qx && px <= qx + rBtnW && py >= uiY + 66 && py <= uiY + 84)
			{
				ModGraphics.SetQuality(q);
				SoundMn.gI().buttonClick();
				return true;
			}
		}

		// 4. Auto FPS
		if (px >= uiX + 68 && px <= uiX + 116 && py >= uiY + 102 && py <= uiY + 120)
		{
			if (ModFps.isAutoFps)
			{
				ModFps.SetFPS(144);
			}
			else
			{
				ModFps.SetAutoFPS();
			}
			SoundMn.gI().buttonClick();
			return true;
		}

		// 5. Chọn Mốc FPS
		for (int f = 0; f < fpsOptions.Length; f++)
		{
			int col = f % 4;
			int row = f / 4;
			int fx = uiX + 6 + col * (rBtnW + 3);
			int fy = uiY + 124 + row * 20;
			if (px >= fx && px <= fx + rBtnW && py >= fy && py <= fy + 18)
			{
				ModFps.SetFPS(fpsOptions[f]);
				SoundMn.gI().buttonClick();
				return true;
			}
		}

		// 6. Bật / Tắt Việt Hoá Server Data
		if (px >= uiX + 58 && px <= uiX + 100 && py >= uiY + 188 && py <= uiY + 208)
		{
			ModConfig.isTranslate = !ModConfig.isTranslate;
			ModConfig.SaveConfig();
			ModTranslate.ApplyAllTranslations();
			GameScr.info1.addInfo("Dịch Việt Hoá: " + (ModConfig.isTranslate ? "BẬT" : "TẮT (Gốc Server)"), 0);
			SoundMn.gI().buttonClick();
			return true;
		}

		// 7. Bật / Tắt Logo TriHienKun trong game
		if (px >= uiX + 144 && px <= uiX + 186 && py >= uiY + 188 && py <= uiY + 208)
		{
			ModLogo.isShowLogoInGame = !ModLogo.isShowLogoInGame;
			ModConfig.SaveConfig();
			SoundMn.gI().buttonClick();
			return true;
		}

		// 8. Bật / Tắt Bàn Phím Ảo & Analog
		if (px >= uiX + 242 && px <= uiX + 284 && py >= uiY + 188 && py <= uiY + 208)
		{
			GameScr.isAnalog = (GameScr.isAnalog == 1) ? 0 : 1;
			GameScr.setSkillBarPosition();
			Rms.saveRMSInt("analog", GameScr.isAnalog);
			ModConfig.SaveConfig();
			GameScr.info1.addInfo("Bàn phím ảo & Analog: " + ((GameScr.isAnalog == 1) ? "BẬT" : "TẮT"), 0);
			SoundMn.gI().buttonClick();
			return true;
		}

		// 9. Mở Giao diện Hình Nền Phong Cảnh
		if (px >= uiX + 8 && px <= uiX + uiW - 8 && py >= uiY + 208 && py <= uiY + 228)
		{
			ModUIBackground.isOpen = true;
			SoundMn.gI().buttonClick();
			return true;
		}

		return false;
	}
}
