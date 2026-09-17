using System;

public static class ModUINextMap
{
	public static void Paint(int uiX, int uiY, int uiW, int uiH, mGraphics g)
	{
		if (ModNextMap.isNextMapActive)
		{
			mFont.tahoma_7_green2.drawString(g, "Đang đến: " + ModNextMap.GetMapName(ModNextMap.nextMapTargetId) + " (" + ModNextMap.nextMapTargetId + ")", uiX + 6, uiY + 7, mFont.LEFT);
			ModUI.PaintNativeButton(uiX + uiW - 62, uiY + 4, 56, 16, "HỦY ĐI", false, g);
		}
		else
		{
			mFont.tahoma_7_grey.drawString(g, "Hiện tại: " + TileMap.mapName + " (" + TileMap.mapID + ") | Chọn map đến:", uiX + 6, uiY + 7, mFont.LEFT);
		}

		// 3 Nút chọn hành tinh
		int pBtnW = (uiW - 16) / 3;
		ModUI.PaintNativeButton(uiX + 5, uiY + 23, pBtnW, 17, "Trái Đất (16)", ModNextMap.selectedPlanetTab == 0, g);
		ModUI.PaintNativeButton(uiX + 8 + pBtnW, uiY + 23, pBtnW, 17, "Namếc (14)", ModNextMap.selectedPlanetTab == 1, g);
		ModUI.PaintNativeButton(uiX + 11 + pBtnW * 2, uiY + 23, pBtnW, 17, "Xayda (14)", ModNextMap.selectedPlanetTab == 2, g);

		// Khung danh sách các map
		int listX = uiX + 5;
		int listY = uiY + 44;
		int listW = uiW - 10;
		int listH = uiH - 48;
		GameCanvas.paintz.paintFrameSimple(listX, listY, listW, listH, g);
		g.setColor(15196114);
		g.fillRect(listX + 2, listY + 2, listW - 4, listH - 4);

		int[] currentPlanetMaps = ModNextMap.planetMapIds[ModNextMap.selectedPlanetTab];
		int btnW = (listW - 10) / 2;
		int btnH = 16;

		for (int m = 0; m < currentPlanetMaps.Length; m++)
		{
			int mId = currentPlanetMaps[m];
			string mName = ModNextMap.GetMapName(mId);
			int col = m % 2;
			int row = m / 2;
			int btnX = (col == 0) ? (listX + 3) : (listX + 7 + btnW);
			int btnY = listY + 3 + row * 18;

			bool isCurrent = (mId == TileMap.mapID);
			bool isTarget = (mId == ModNextMap.nextMapTargetId && ModNextMap.isNextMapActive);

			if (isCurrent)
			{
				g.setColor(0xC8E6C9);
				g.fillRect(btnX, btnY, btnW, btnH);
				g.setColor(0x388E3C);
				g.drawRect(btnX, btnY, btnW, btnH);
				mFont.tahoma_7_green2.drawString(g, mName + " (Hiện tại)", btnX + btnW / 2, btnY + 2, mFont.CENTER);
			}
			else if (isTarget)
			{
				g.setColor(0xFFE0B2);
				g.fillRect(btnX, btnY, btnW, btnH);
				g.setColor(0xF57C00);
				g.drawRect(btnX, btnY, btnW, btnH);
				(mFont.tahoma_7_yellow ?? mFont.tahoma_7_white).drawString(g, mName + " (Đang đến)", btnX + btnW / 2, btnY + 2, mFont.CENTER);
			}
			else
			{
				g.setColor(15787715);
				g.fillRect(btnX, btnY, btnW, btnH);
				g.setColor(6702080);
				g.drawRect(btnX, btnY, btnW, btnH);
				mFont.tahoma_7_grey.drawString(g, mName, btnX + btnW / 2, btnY + 2, mFont.CENTER);
			}
		}
	}

	public static bool HandleTap(int px, int py, int uiX, int uiY, int uiW, int uiH)
	{
		if (ModNextMap.isNextMapActive)
		{
			if (px >= uiX + uiW - 62 && px <= uiX + uiW - 6 && py >= uiY + 3 && py <= uiY + 21)
			{
				ModNextMap.StopNextMap();
				GameScr.info1.addInfo("Đã hủy Next Map!", 0);
				SoundMn.gI().buttonClick();
				return true;
			}
		}

		int pBtnW = (uiW - 16) / 3;
		if (px >= uiX + 5 && px <= uiX + 5 + pBtnW && py >= uiY + 22 && py <= uiY + 41)
		{
			ModNextMap.selectedPlanetTab = 0;
			SoundMn.gI().buttonClick();
			return true;
		}
		if (px >= uiX + 8 + pBtnW && px <= uiX + 8 + pBtnW * 2 && py >= uiY + 22 && py <= uiY + 41)
		{
			ModNextMap.selectedPlanetTab = 1;
			SoundMn.gI().buttonClick();
			return true;
		}
		if (px >= uiX + 11 + pBtnW * 2 && px <= uiX + 11 + pBtnW * 3 && py >= uiY + 22 && py <= uiY + 41)
		{
			ModNextMap.selectedPlanetTab = 2;
			SoundMn.gI().buttonClick();
			return true;
		}

		int listX = uiX + 5;
		int listY = uiY + 44;
		int listW = uiW - 10;
		int btnW = (listW - 10) / 2;
		int btnH = 16;

		int[] currentPlanetMaps = ModNextMap.planetMapIds[ModNextMap.selectedPlanetTab];
		for (int m = 0; m < currentPlanetMaps.Length; m++)
		{
			int col = m % 2;
			int row = m / 2;
			int btnX = (col == 0) ? (listX + 3) : (listX + 7 + btnW);
			int btnY = listY + 3 + row * 18;

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
