using System;

public static class ModUISpeed
{
	private static readonly float[] speeds = new float[] { 1.0f, 1.5f, 2.0f, 2.5f, 3.0f, 4.0f, 5.0f };

	public static void Paint(int uiX, int uiY, int uiW, int uiH, mGraphics g)
	{
		mFont.tahoma_7_grey.drawString(g, "Tốc độ chạy:", uiX + 5, uiY + 7, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 68, uiY + 4, 38, 17, ModSpeed.speedHack ? "BẬT" : "TẮT", ModSpeed.speedHack, g);

		mFont.tahoma_7_grey.drawString(g, "Chọn hệ số tốc độ di chuyển:", uiX + 5, uiY + 25, mFont.LEFT);

		int btnW = (uiW - 16) / speeds.Length;
		for (int s = 0; s < speeds.Length; s++)
		{
			int sx = uiX + 4 + s * (btnW + 2);
			bool isSel = Res.abs((int)(ModSpeed.speedMult * 10) - (int)(speeds[s] * 10)) < 2;
			ModUI.PaintNativeButton(sx, uiY + 39, btnW, 17, "x" + speeds[s].ToString("0.0"), isSel, g);
		}

		int boxX = uiX + 4;
		int boxY = uiY + 59;
		int boxW = uiW - 8;
		int boxH = uiH - 63;
		if (boxH < 40) boxH = 40;
		GameCanvas.paintz.paintFrameSimple(boxX, boxY, boxW, boxH, g);
		g.setColor(15196114);
		g.fillRect(boxX + 2, boxY + 2, boxW - 4, boxH - 4);

		mFont.tahoma_7_green2.drawString(g, "Tốc độ hiện tại: x" + ModSpeed.speedMult.ToString("0.0") + (ModSpeed.speedHack ? " (ĐANG BẬT)" : " (ĐANG TẮT)"), boxX + boxW / 2, boxY + 16, mFont.CENTER);
		mFont.tahoma_7_grey.drawString(g, "* Tăng tốc di chuyển mượt mà, đồng bộ với tốc độ khung hình.", boxX + boxW / 2, boxY + 34, mFont.CENTER);
	}

	public static bool HandleTap(int px, int py, int uiX, int uiY, int uiW, int uiH)
	{
		if (px >= uiX + 68 && px <= uiX + 108 && py >= uiY + 2 && py <= uiY + 22)
		{
			ModSpeed.speedHack = !ModSpeed.speedHack;
			ModConfig.SaveConfig();
			SoundMn.gI().buttonClick();
			return true;
		}

		int btnW = (uiW - 16) / speeds.Length;
		for (int s = 0; s < speeds.Length; s++)
		{
			int sx = uiX + 4 + s * (btnW + 2);
			if (px >= sx && px <= sx + btnW && py >= uiY + 38 && py <= uiY + 57)
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
