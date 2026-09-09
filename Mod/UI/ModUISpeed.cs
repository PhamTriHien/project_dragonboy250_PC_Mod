using System;

public static class ModUISpeed
{
	private static readonly float[] speeds = new float[] { 1.0f, 1.5f, 2.0f, 2.5f, 3.0f, 4.0f, 5.0f };

	public static void Paint(int uiX, int uiY, int uiW, int uiH, mGraphics g)
	{
		mFont.tahoma_7b_dark.drawString(g, "Tốc độ chạy:", uiX + 12, uiY + 16, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 90, uiY + 12, 52, 18, ModSpeed.speedHack ? "BẬT" : "TẮT", ModSpeed.speedHack, g);

		mFont.tahoma_7b_dark.drawString(g, "Chọn hệ số tốc độ di chuyển:", uiX + 12, uiY + 45, mFont.LEFT);

		int btnW = (uiW - 24) / speeds.Length;
		for (int s = 0; s < speeds.Length; s++)
		{
			int sx = uiX + 6 + s * (btnW + 2);
			bool isSel = Res.abs((int)(ModSpeed.speedMult * 10) - (int)(speeds[s] * 10)) < 2;
			ModUI.PaintNativeButton(sx, uiY + 68, btnW, 20, "x" + speeds[s].ToString("0.0"), isSel, g);
		}

		int boxX = uiX + 8;
		int boxY = uiY + 105;
		int boxW = uiW - 16;
		int boxH = 95;
		GameCanvas.paintz.paintFrameSimple(boxX, boxY, boxW, boxH, g);
		g.setColor(15196114);
		g.fillRect(boxX + 2, boxY + 2, boxW - 4, boxH - 4);

		mFont.tahoma_7b_dark.drawString(g, "Tốc độ hiện tại: x" + ModSpeed.speedMult.ToString("0.0") + (ModSpeed.speedHack ? " (ĐANG BẬT)" : " (ĐANG TẮT)"), boxX + boxW / 2, boxY + 25, mFont.CENTER);
		mFont.tahoma_7_grey.drawString(g, "* Tăng tốc di chuyển mượt mà, đồng bộ với tốc độ khung hình.", boxX + boxW / 2, boxY + 52, mFont.CENTER);
	}

	public static bool HandleTap(int px, int py, int uiX, int uiY, int uiW, int uiH)
	{
		if (px >= uiX + 90 && px <= uiX + 142 && py >= uiY + 10 && py <= uiY + 32)
		{
			ModSpeed.speedHack = !ModSpeed.speedHack;
			ModConfig.SaveConfig();
			SoundMn.gI().buttonClick();
			return true;
		}

		int btnW = (uiW - 24) / speeds.Length;
		for (int s = 0; s < speeds.Length; s++)
		{
			int sx = uiX + 6 + s * (btnW + 2);
			if (px >= sx && px <= sx + btnW && py >= uiY + 68 && py <= uiY + 90)
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
