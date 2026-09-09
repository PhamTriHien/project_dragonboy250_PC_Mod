using System;
using UnityEngine;

public static class ModUIGraphics
{
	private static readonly int[] fpsOptions = new int[8] { 30, 60, 90, 120, 144, 165, 185, 240 };

	public static void Paint(int uiX, int uiY, int uiW, int uiH, mGraphics g)
	{
		// 1. Toàn Màn Hình
		mFont.tahoma_7b_white.drawString(g, "Toàn màn hình:", uiX + 18, uiY + 50, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 105, uiY + 47, 48, 16, ModGraphics.isFullscreen ? "BẬT" : "TẮT", ModGraphics.isFullscreen, g);
		mFont.tahoma_7_yellow.drawString(g, "(Phím tắt F11 / Alt+Enter)", uiX + 160, uiY + 50, mFont.LEFT);

		// 2. Độ Phân Giải Cửa Sổ
		for (int r = 0; r < 4; r++)
		{
			int rx = uiX + 16 + r * 77;
			int rw = (r == 3) ? 77 : 73;
			bool isSelRes = (!ModGraphics.isFullscreen && ModGraphics.resolutionIndex == r);
			ModUI.PaintNativeButton(rx, uiY + 67, rw, 16, ModGraphics.resolutionNames[r], isSelRes, g);
		}

		// 3. Chế độ đồ họa & Khử răng cưa GPU
		mFont.tahoma_7b_white.drawString(g, "Chất lượng đồ họa (GPU Khử răng cưa):", uiX + 18, uiY + 87, mFont.LEFT);
		for (int q = 0; q < 4; q++)
		{
			int qx = uiX + 16 + q * 77;
			int qw = (q == 3) ? 77 : 73;
			ModUI.PaintNativeButton(qx, uiY + 101, qw, 16, ModGraphics.graphicsNames[q], ModGraphics.graphicsQuality == q, g);
		}

		// Mô tả chi tiết mức đồ họa
		if (ModGraphics.graphicsQuality == 0)
		{
			mFont.tahoma_7_white.drawString(g, "* Ultra: GPU khử răng cưa 4x, lọc dị hướng, max texture nét căng.", uiX + 18, uiY + 120, mFont.LEFT);
		}
		else if (ModGraphics.graphicsQuality == 1)
		{
			mFont.tahoma_7_yellow.drawString(g, "* Medium: Tối ưu mượt, xóa hiệu ứng chiêu thức & thời tiết.", uiX + 18, uiY + 120, mFont.LEFT);
		}
		else if (ModGraphics.graphicsQuality == 2)
		{
			mFont.tahoma_7_yellow.drawString(g, "* Low: Xóa background, phông nền trắng xanh nhạt siêu nhẹ.", uiX + 18, uiY + 120, mFont.LEFT);
		}
		else if (ModGraphics.graphicsQuality == 3)
		{
			mFont.tahoma_7b_green2.drawString(g, "* Super Low: Xóa cây cỏ trang trí, chỉ chừa base map & NPC.", uiX + 18, uiY + 120, mFont.LEFT);
		}

		// 4. Phần FPS
		mFont.tahoma_7b_white.drawString(g, "Auto FPS:", uiX + 18, uiY + 135, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 75, uiY + 132, 48, 16, ModFps.isAutoFps ? "BẬT" : "TẮT", ModFps.isAutoFps, g);
		mFont.tahoma_7_yellow.drawString(g, "(Khớp tần số quét màn hình)", uiX + 130, uiY + 135, mFont.LEFT);

		// Mốc FPS cố định (2 hàng x 4 nút)
		for (int f = 0; f < fpsOptions.Length; f++)
		{
			int col = f % 4;
			int row = f / 4;
			int fx = uiX + 16 + col * 77;
			int fy = uiY + 152 + row * 18;
			int fw = (col == 3) ? 77 : 73;
			bool isSel = (!ModFps.isAutoFps && ModFps.targetFps == fpsOptions[f]);
			ModUI.PaintNativeButton(fx, fy, fw, 15, fpsOptions[f] + " FPS", isSel, g);
		}

		mFont.tahoma_7_green2.drawString(g, "FPS: " + Main.realFPS + " | Màn: " + ModFps.GetDeviceMaxRefreshRate() + "Hz | Cửa sổ: " + Screen.width + "x" + Screen.height, uiX + uiW / 2, uiY + 191, mFont.CENTER);

		// 5. Việt Hoá Server Data, Logo & Bàn phím ảo Analog
		mFont.tahoma_7b_white.drawString(g, "Việt Hoá:", uiX + 18, uiY + 207, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 68, uiY + 204, 38, 16, ModConfig.isTranslate ? "BẬT" : "TẮT", ModConfig.isTranslate, g);

		mFont.tahoma_7b_white.drawString(g, "Logo:", uiX + 114, uiY + 207, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 146, uiY + 204, 38, 16, ModLogo.isShowLogoInGame ? "BẬT" : "TẮT", ModLogo.isShowLogoInGame, g);

		mFont.tahoma_7b_white.drawString(g, "Analog:", uiX + 192, uiY + 207, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 238, uiY + 204, 38, 16, (GameScr.isAnalog == 1) ? "BẬT" : "TẮT", GameScr.isAnalog == 1, g);
	}

	public static bool HandleTap(int px, int py, int uiX, int uiY, int uiW, int uiH)
	{
		// 1. Bật / Tắt Toàn Màn Hình
		if (px >= uiX + 105 && px <= uiX + 153 && py >= uiY + 47 && py <= uiY + 63)
		{
			ModGraphics.ToggleFullscreen();
			SoundMn.gI().buttonClick();
			return true;
		}

		// 2. Chọn Độ Phân Giải
		for (int r = 0; r < 4; r++)
		{
			int rx = uiX + 16 + r * 77;
			int rw = (r == 3) ? 77 : 73;
			if (px >= rx && px <= rx + rw && py >= uiY + 67 && py <= uiY + 83)
			{
				ModGraphics.ApplyResolution(r);
				SoundMn.gI().buttonClick();
				return true;
			}
		}

		// 3. Chọn Chất Lượng Đồ Họa
		for (int q = 0; q < 4; q++)
		{
			int qx = uiX + 16 + q * 77;
			int qw = (q == 3) ? 77 : 73;
			if (px >= qx && px <= qx + qw && py >= uiY + 101 && py <= uiY + 117)
			{
				ModGraphics.SetQuality(q);
				SoundMn.gI().buttonClick();
				return true;
			}
		}

		// 4. Auto FPS
		if (px >= uiX + 75 && px <= uiX + 123 && py >= uiY + 132 && py <= uiY + 148)
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
			int fx = uiX + 16 + col * 77;
			int fy = uiY + 152 + row * 18;
			int fw = (col == 3) ? 77 : 73;
			if (px >= fx && px <= fx + fw && py >= fy && py <= fy + 15)
			{
				ModFps.SetFPS(fpsOptions[f]);
				SoundMn.gI().buttonClick();
				return true;
			}
		}

		// 6. Bật / Tắt Việt Hoá Server Data
		if (px >= uiX + 68 && px <= uiX + 106 && py >= uiY + 204 && py <= uiY + 220)
		{
			ModConfig.isTranslate = !ModConfig.isTranslate;
			ModConfig.SaveConfig();
			ModTranslate.ApplyAllTranslations();
			GameScr.info1.addInfo("Dịch Việt Hoá: " + (ModConfig.isTranslate ? "BẬT" : "TẮT (Gốc Server)"), 0);
			SoundMn.gI().buttonClick();
			return true;
		}

		// 7. Bật / Tắt Logo TriHienKun trong game
		if (px >= uiX + 146 && px <= uiX + 184 && py >= uiY + 204 && py <= uiY + 220)
		{
			ModLogo.isShowLogoInGame = !ModLogo.isShowLogoInGame;
			ModConfig.SaveConfig();
			SoundMn.gI().buttonClick();
			return true;
		}

		// 8. Bật / Tắt Bàn Phím Ảo & Analog
		if (px >= uiX + 238 && px <= uiX + 276 && py >= uiY + 204 && py <= uiY + 220)
		{
			GameScr.isAnalog = (GameScr.isAnalog == 1) ? 0 : 1;
			Rms.saveRMSInt("analog", GameScr.isAnalog);
			ModConfig.SaveConfig();
			GameScr.info1.addInfo("Bàn phím ảo & Analog: " + ((GameScr.isAnalog == 1) ? "BẬT" : "TẮT"), 0);
			SoundMn.gI().buttonClick();
			return true;
		}

		return false;
	}
}
