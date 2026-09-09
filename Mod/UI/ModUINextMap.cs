using System;

public static class ModUINextMap
{
	public static void Paint(int uiX, int uiY, int uiW, int uiH, mGraphics g)
	{
		if (ModNextMap.isNextMapActive)
		{
			mFont.tahoma_7b_green2.drawString(g, "Đang đến: " + ModNextMap.GetMapName(ModNextMap.nextMapTargetId) + " (" + ModNextMap.nextMapTargetId + ")", uiX + 8, uiY + 12, mFont.LEFT);
			ModUI.PaintNativeButton(uiX + uiW - 68, uiY + 6, 60, 18, "HỦY ĐI", false, g);
		}
		else
		{
			mFont.tahoma_7b_dark.drawString(g, "Hiện tại: " + TileMap.mapName + " (" + TileMap.mapID + ") | Chọn map muốn đến:", uiX + 8, uiY + 12, mFont.LEFT);
		}

		// 3 Nút chọn hành tinh
		int pBtnW = (uiW - 20) / 3;
		ModUI.PaintNativeButton(uiX + 6, uiY + 28, pBtnW, 19, "Trái Đất (16)", ModNextMap.selectedPlanetTab == 0, g);
		ModUI.PaintNativeButton(uiX + 10 + pBtnW, uiY + 28, pBtnW, 19, "Namếc (14)", ModNextMap.selectedPlanetTab == 1, g);
		ModUI.PaintNativeButton(uiX + 14 + pBtnW * 2, uiY + 28, pBtnW, 19, "Xayda (14)", ModNextMap.selectedPlanetTab == 2, g);

		// Khung danh sách các map
		int listX = uiX + 6;
		int listY = uiY + 52;
		int listW = uiW - 12;
		int listH = uiH - 58;
		GameCanvas.paintz.paintFrameSimple(listX, listY, listW, listH, g);
		g.setColor(15196114);
		g.fillRect(listX + 2, listY + 2, listW - 4, listH - 4);

		int[] currentPlanetMaps = ModNextMap.planetMapIds[ModNextMap.selectedPlanetTab];
		int btnW = (listW - 14) / 2;
		int btnH = 17;

		for (int m = 0; m < currentPlanetMaps.Length; m++)
		{
			int mId = currentPlanetMaps[m];
			string mName = ModNextMap.GetMapName(mId);
			int col = m % 2;
			int row = m / 2;
			int btnX = (col == 0) ? (listX + 4) : (listX + 8 + btnW);
			int btnY = listY + 4 + row * 19;

			bool isCurrent = (mId == TileMap.mapID);
			bool isTarget = (mId == ModNextMap.nextMapTargetId && ModNextMap.isNextMapActive);

			if (isCurrent)
			{
				g.setColor(0xC8E6C9);
				g.fillRect(btnX, btnY, btnW, btnH);
				g.setColor(0x388E3C);
				g.drawRect(btnX, btnY, btnW, btnH);
				mFont.tahoma_7b_green2.drawString(g, mName + " (Hiện tại)", btnX + btnW / 2, btnY + 2, mFont.CENTER);
			}
			else if (isTarget)
			{
				g.setColor(0xFFE0B2);
				g.fillRect(btnX, btnY, btnW, btnH);
				g.setColor(0xF57C00);
				g.drawRect(btnX, btnY, btnW, btnH);
				(mFont.tahoma_7b_yellow ?? mFont.tahoma_7b_dark).drawString(g, mName + " (Đang đến)", btnX + btnW / 2, btnY + 2, mFont.CENTER);
			}
			else
			{
				g.setColor(15787715);
				g.fillRect(btnX, btnY, btnW, btnH);
				g.setColor(6702080);
				g.drawRect(btnX, btnY, btnW, btnH);
				mFont.tahoma_7b_dark.drawString(g, mName, btnX + btnW / 2, btnY + 2, mFont.CENTER);
			}
		}
	}

	public static bool HandleTap(int px, int py, int uiX, int uiY, int uiW, int uiH)
	{
		if (ModNextMap.isNextMapActive)
		{
			if (px >= uiX + uiW - 68 && px <= uiX + uiW - 8 && py >= uiY + 6 && py <= uiY + 24)
			{
				ModNextMap.StopNextMap();
				GameScr.info1.addInfo("Đã hủy Next Map!", 0);
				SoundMn.gI().buttonClick();
				return true;
			}
		}

		int pBtnW = (uiW - 20) / 3;
		if (px >= uiX + 6 && px <= uiX + 6 + pBtnW && py >= uiY + 28 && py <= uiY + 47)
		{
			ModNextMap.selectedPlanetTab = 0;
			SoundMn.gI().buttonClick();
			return true;
		}
		if (px >= uiX + 10 + pBtnW && px <= uiX + 10 + pBtnW * 2 && py >= uiY + 28 && py <= uiY + 47)
		{
			ModNextMap.selectedPlanetTab = 1;
			SoundMn.gI().buttonClick();
			return true;
		}
		if (px >= uiX + 14 + pBtnW * 2 && px <= uiX + 14 + pBtnW * 3 && py >= uiY + 28 && py <= uiY + 47)
		{
			ModNextMap.selectedPlanetTab = 2;
			SoundMn.gI().buttonClick();
			return true;
		}

		int listX = uiX + 6;
		int listY = uiY + 52;
		int listW = uiW - 12;
		int btnW = (listW - 14) / 2;
		int btnH = 17;

		int[] currentPlanetMaps = ModNextMap.planetMapIds[ModNextMap.selectedPlanetTab];
		for (int m = 0; m < currentPlanetMaps.Length; m++)
		{
			int col = m % 2;
			int row = m / 2;
			int btnX = (col == 0) ? (listX + 4) : (listX + 8 + btnW);
			int btnY = listY + 4 + row * 19;

			if (px >= btnX && px <= btnX + btnW && py >= btnY && py <= btnY + btnH)
			{
				int targetMapId = currentPlanetMaps[m];
				ModNextMap.StartNextMap(targetMapId);
				ModUI.uiCustomOpen = false;
				SoundMn.gI().buttonClick();
				return true;
			}
		}

		return false;
	}
}
