using System;
using UnityEngine;

public static class ModUIGraphics
{
	private static readonly int[] fpsOptions = new int[8] { 30, 60, 90, 120, 144, 165, 185, 240 };

	public static void Paint(int uiX, int uiY, int uiW, int uiH, mGraphics g)
	{
		// 1. Toàn Màn Hình
		mFont.tahoma_7_grey.drawString(g, "Toàn màn hình:", uiX + 6, uiY + 7, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 88, uiY + 4, 44, 16, ModGraphics.isFullscreen ? "BẬT" : "TẮT", ModGraphics.isFullscreen, g);
		mFont.tahoma_7_grey.drawString(g, "(F11 / Alt+Enter)", uiX + 138, uiY + 7, mFont.LEFT);

		// 2. Độ Phân Giải Cửa Sổ
		mFont.tahoma_7_grey.drawString(g, "Độ phân giải cửa sổ:", uiX + 6, uiY + 28, mFont.LEFT);
		int rBtnW = (uiW - 18) / 4;
		for (int r = 0; r < 4; r++)
		{
			int rx = uiX + 5 + r * (rBtnW + 3);
			bool isSelRes = (!ModGraphics.isFullscreen && ModGraphics.resolutionIndex == r);
			ModUI.PaintNativeButton(rx, uiY + 43, rBtnW, 16, ModGraphics.resolutionNames[r], isSelRes, g);
		}

		// 3. Chế độ đồ họa & Khử răng cưa GPU
		mFont.tahoma_7_grey.drawString(g, "Chất lượng đồ họa (GPU Khử răng cưa):", uiX + 6, uiY + 66, mFont.LEFT);
		for (int q = 0; q < 4; q++)
		{
			int qx = uiX + 5 + q * (rBtnW + 3);
			ModUI.PaintNativeButton(qx, uiY + 81, rBtnW, 16, ModGraphics.graphicsNames[q], ModGraphics.graphicsQuality == q, g);
		}

		// Mô tả chi tiết mức đồ họa
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

		// 4. Độ Sâu Màu Render (Bit-Depth: 16-bit, 32-bit, 64-bit, 128-bit)
		mFont.tahoma_7_grey.drawString(g, "Độ sâu màu Render (Bit-Depth):", uiX + 6, uiY + 116, mFont.LEFT);
		for (int b = 0; b < 4; b++)
		{
			int bx = uiX + 5 + b * (rBtnW + 3);
			ModUI.PaintNativeButton(bx, uiY + 131, rBtnW, 16, ModGraphics.bitDepthNames[b], ModGraphics.renderBitDepth == b, g);
		}

		// Mô tả chi tiết Bit-Depth
		int bdDescY = uiY + 151;
		if (ModGraphics.renderBitDepth == 0)
		{
			mFont.tahoma_7_grey.drawString(g, "* 16-bit: Siêu nhẹ, tiết kiệm RAM/GPU, retro RGB565.", uiX + 6, bdDescY, mFont.LEFT);
		}
		else if (ModGraphics.renderBitDepth == 1)
		{
			mFont.tahoma_7_green2.drawString(g, "* 32-bit: Chuẩn sắc nét HD (RGBA8888) - Mặc định.", uiX + 6, bdDescY, mFont.LEFT);
		}
		else if (ModGraphics.renderBitDepth == 2)
		{
			mFont.tahoma_7_grey.drawString(g, "* 64-bit: Dải màu rộng HDR (RGBA16F), chống banding.", uiX + 6, bdDescY, mFont.LEFT);
		}
		else if (ModGraphics.renderBitDepth == 3)
		{
			mFont.tahoma_7_grey.drawString(g, "* 128-bit: Độ chính xác cực đại (RGBA32F Ultra HDR).", uiX + 6, bdDescY, mFont.LEFT);
		}

		// 5. Phần FPS
		mFont.tahoma_7_grey.drawString(g, "Auto FPS:", uiX + 6, uiY + 168, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 62, uiY + 165, 44, 16, ModFps.isAutoFps ? "BẬT" : "TẮT", ModFps.isAutoFps, g);
		mFont.tahoma_7_grey.drawString(g, "(Khớp tần số quét)", uiX + 112, uiY + 168, mFont.LEFT);

		// Mốc FPS cố định (2 hàng x 4 nút)
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

		// 6. Việt Hoá Server Data, Logo & Bàn phím ảo Analog
		mFont.tahoma_7_grey.drawString(g, "Việt Hoá:", uiX + 6, uiY + 252, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 54, uiY + 249, 38, 16, ModConfig.isTranslate ? "BẬT" : "TẮT", ModConfig.isTranslate, g);

		mFont.tahoma_7_grey.drawString(g, "Logo:", uiX + 100, uiY + 252, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 132, uiY + 249, 38, 16, ModLogo.isShowLogoInGame ? "BẬT" : "TẮT", ModLogo.isShowLogoInGame, g);

		mFont.tahoma_7_grey.drawString(g, "Analog:", uiX + 178, uiY + 252, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 220, uiY + 249, 38, 16, (GameScr.isAnalog == 1) ? "BẬT" : "TẮT", GameScr.isAnalog == 1, g);

		// 7. Phong Cảnh Nền Game (Git)
		string bgBtnText = "🌌 HÌNH NỀN PHONG CẢNH (GIT)" + (ModBackground.isCustomBGActive ? " [ĐANG BẬT]" : "");
		ModUI.PaintNativeButton(uiX + 6, uiY + 273, uiW - 12, 18, bgBtnText, ModBackground.isCustomBGActive, g);
	}

	public static bool HandleTap(int px, int py, int uiX, int uiY, int uiW, int uiH)
	{
		// 1. Bật / Tắt Toàn Màn Hình
		if (px >= uiX + 88 && px <= uiX + 132 && py >= uiY + 3 && py <= uiY + 21)
		{
			ModGraphics.ToggleFullscreen();
			SoundMn.gI().buttonClick();
			return true;
		}

		// 2. Chọn Độ Phân Giải
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

		// 3. Chọn Chất Lượng Đồ Họa
		for (int q = 0; q < 4; q++)
		{
			int qx = uiX + 5 + q * (rBtnW + 3);
			if (px >= qx && px <= qx + rBtnW && py >= uiY + 80 && py <= uiY + 98)
			{
				ModGraphics.SetQuality(q);
				SoundMn.gI().buttonClick();
				return true;
			}
		}

		// 4. Chọn Độ Sâu Màu Render (Bit-Depth)
		for (int b = 0; b < 4; b++)
		{
			int bx = uiX + 5 + b * (rBtnW + 3);
			if (px >= bx && px <= bx + rBtnW && py >= uiY + 130 && py <= uiY + 148)
			{
				ModGraphics.SetBitDepth(b);
				GameScr.info1.addInfo("Độ sâu màu: " + ModGraphics.bitDepthNames[b], 0);
				SoundMn.gI().buttonClick();
				return true;
			}
		}

		// 5. Auto FPS
		if (px >= uiX + 62 && px <= uiX + 106 && py >= uiY + 164 && py <= uiY + 182)
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

		// 6. Chọn Mốc FPS
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

		// 7. Bật / Tắt Việt Hoá Server Data
		if (px >= uiX + 54 && px <= uiX + 92 && py >= uiY + 248 && py <= uiY + 266)
		{
			ModConfig.isTranslate = !ModConfig.isTranslate;
			ModConfig.SaveConfig();
			ModTranslate.ApplyAllTranslations();
			GameScr.info1.addInfo("Dịch Việt Hoá: " + (ModConfig.isTranslate ? "BẬT" : "TẮT (Gốc Server)"), 0);
			SoundMn.gI().buttonClick();
			return true;
		}

		// 8. Bật / Tắt Logo TriHienKun trong game
		if (px >= uiX + 132 && px <= uiX + 170 && py >= uiY + 248 && py <= uiY + 266)
		{
			ModLogo.isShowLogoInGame = !ModLogo.isShowLogoInGame;
			ModConfig.SaveConfig();
			SoundMn.gI().buttonClick();
			return true;
		}

		// 9. Bật / Tắt Bàn Phím Ảo & Analog
		if (px >= uiX + 220 && px <= uiX + 258 && py >= uiY + 248 && py <= uiY + 266)
		{
			GameScr.isAnalog = (GameScr.isAnalog == 1) ? 0 : 1;
			GameScr.setSkillBarPosition();
			Rms.saveRMSInt("analog", GameScr.isAnalog);
			ModConfig.SaveConfig();
			GameScr.info1.addInfo("Bàn phím ảo & Analog: " + ((GameScr.isAnalog == 1) ? "BẬT" : "TẮT"), 0);
			SoundMn.gI().buttonClick();
			return true;
		}

		// 10. Mở Giao diện Hình Nền Phong Cảnh
		if (px >= uiX + 6 && px <= uiX + uiW - 6 && py >= uiY + 272 && py <= uiY + 293)
		{
			ModUIBackground.isOpen = true;
			ModUI.detailScrollY = 0;
			SoundMn.gI().buttonClick();
			return true;
		}

		return false;
	}
}
