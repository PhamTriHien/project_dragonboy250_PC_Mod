using System;

public static class ModUISpeed
{
	private static readonly float[] speeds = new float[] { 1.0f, 1.5f, 2.0f, 2.5f, 3.0f, 4.0f, 5.0f };

	public static void Paint(int uiX, int uiY, int uiW, int uiH, mGraphics g)
	{
		mFont.tahoma_7b_white.drawString(g, "Tốc độ chạy:", uiX + 20, uiY + 58, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 110, uiY + 52, 85, ModSpeed.speedHack ? "BẬT" : "TẮT", ModSpeed.speedHack, g);

		mFont.tahoma_7b_yellow.drawString(g, "Chọn hệ số tốc độ di chuyển:", uiX + 20, uiY + 86, mFont.LEFT);

		for (int s = 0; s < speeds.Length; s++)
		{
			int sx = uiX + 16 + s * 44;
			bool isSel = Res.abs((int)(ModSpeed.speedMult * 10) - (int)(speeds[s] * 10)) < 2;
			ModUI.PaintNativeButton(sx, uiY + 108, 41, "x" + speeds[s].ToString("0.0"), isSel, g);
		}

		mFont.tahoma_7_yellow.drawString(g, "Tốc độ hiện tại: x" + ModSpeed.speedMult.ToString("0.0") + (ModSpeed.speedHack ? " (ĐANG BẬT)" : " (ĐANG TẮT)"), uiX + uiW / 2, uiY + 155, mFont.CENTER);
		mFont.tahoma_7_grey.drawString(g, "* Tăng tốc di chuyển mượt mà, đồng bộ với tốc độ khung hình.", uiX + uiW / 2, uiY + 180, mFont.CENTER);
	}

	public static bool HandleTap(int px, int py, int uiX, int uiY, int uiW, int uiH)
	{
		if (px >= uiX + 110 && px <= uiX + 195 && py >= uiY + 52 && py <= uiY + 74)
		{
			ModSpeed.speedHack = !ModSpeed.speedHack;
			ModConfig.SaveConfig();
			SoundMn.gI().buttonClick();
			return true;
		}

		for (int s = 0; s < speeds.Length; s++)
		{
			int sx = uiX + 16 + s * 44;
			if (px >= sx && px <= sx + 41 && py >= uiY + 108 && py <= uiY + 130)
			{
				ModSpeed.speedMult = speeds[s];
				ModConfig.SaveConfig();
				SoundMn.gI().buttonClick();
				return true;
			}
		}
		return false;
	}
}
