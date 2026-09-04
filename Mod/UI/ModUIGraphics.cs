using System;

public static class ModUIGraphics
{
	private static readonly int[] fpsOptions = new int[8] { 30, 60, 90, 120, 144, 165, 185, 240 };

	public static void Paint(int uiX, int uiY, int uiW, int uiH, mGraphics g)
	{
		mFont.tahoma_7b_white.drawString(g, "Chế độ đồ họa:", uiX + 18, uiY + 49, mFont.LEFT);

		// 4 Nút chọn chất lượng đồ họa
		int qBtnW = 66;
		for (int q = 0; q < 4; q++)
		{
			int qx = uiX + 16 + q * 77;
			ModUI.PaintNativeButton(qx, uiY + 63, qBtnW, 18, ModGraphics.graphicsNames[q], ModGraphics.graphicsQuality == q, g);
		}

		// Mô tả chi tiết mức đồ họa đang chọn
		if (ModGraphics.graphicsQuality == 0)
		{
			mFont.tahoma_7_white.drawString(g, "* Ultra: Mặc định gốc, đầy đủ background, cây cỏ & hiệu ứng.", uiX + 18, uiY + 89, mFont.LEFT);
		}
		else if (ModGraphics.graphicsQuality == 1)
		{
			mFont.tahoma_7_yellow.drawString(g, "* Medium: Xóa tất cả hiệu ứng động (skill, thời tiết, pháo hoa...).", uiX + 18, uiY + 89, mFont.LEFT);
		}
		else if (ModGraphics.graphicsQuality == 2)
		{
			mFont.tahoma_7_yellow.drawString(g, "* Low: Xóa background, phông nền trắng xanh nhạt siêu mượt.", uiX + 18, uiY + 89, mFont.LEFT);
		}
		else if (ModGraphics.graphicsQuality == 3)
		{
			mFont.tahoma_7b_green2.drawString(g, "* Super Low: Xóa cây cỏ trang trí, chỉ chừa base map & NPC.", uiX + 18, uiY + 89, mFont.LEFT);
		}

		// Phần FPS
		mFont.tahoma_7b_white.drawString(g, "Auto FPS:", uiX + 18, uiY + 109, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 80, uiY + 105, 52, 18, ModFps.isAutoFps ? "BẬT" : "TẮT", ModFps.isAutoFps, g);
		mFont.tahoma_7_yellow.drawString(g, "(Khuyến nghị theo màn hình)", uiX + 142, uiY + 109, mFont.LEFT);

		mFont.tahoma_7b_yellow.drawString(g, "Mốc FPS cố định:", uiX + 18, uiY + 130, mFont.LEFT);
		for (int f = 0; f < fpsOptions.Length; f++)
		{
			int col = f % 4;
			int row = f / 4;
			int fx = uiX + 16 + col * 77;
			int fy = uiY + 145 + row * 22;
			bool isSel = (!ModFps.isAutoFps && ModFps.targetFps == fpsOptions[f]);
			ModUI.PaintNativeButton(fx, fy, 62, 18, fpsOptions[f] + " FPS", isSel, g);
		}

		mFont.tahoma_7_green2.drawString(g, "FPS: " + Main.realFPS + " | Tần số màn: " + ModFps.GetDeviceMaxRefreshRate() + "Hz", uiX + uiW / 2, uiY + 198, mFont.CENTER);
	}

	public static bool HandleTap(int px, int py, int uiX, int uiY, int uiW, int uiH)
	{
		for (int q = 0; q < 4; q++)
		{
			int qx = uiX + 16 + q * 77;
			if (px >= qx && px <= qx + 66 && py >= uiY + 63 && py <= uiY + 81)
			{
				ModGraphics.SetQuality(q);
				SoundMn.gI().buttonClick();
				return true;
			}
		}

		if (px >= uiX + 80 && px <= uiX + 132 && py >= uiY + 105 && py <= uiY + 123)
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

		for (int f = 0; f < fpsOptions.Length; f++)
		{
			int col = f % 4;
			int row = f / 4;
			int fx = uiX + 16 + col * 77;
			int fy = uiY + 145 + row * 22;
			if (px >= fx && px <= fx + 62 && py >= fy && py <= fy + 18)
			{
				ModFps.SetFPS(fpsOptions[f]);
				SoundMn.gI().buttonClick();
				return true;
			}
		}

		return false;
	}
}
