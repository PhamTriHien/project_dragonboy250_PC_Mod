using System;

public static class ModUINextMap
{
	public static void Paint(int uiX, int uiY, int uiW, int uiH, mGraphics g)
	{
		if (ModNextMap.isNextMapActive)
		{
			mFont.tahoma_7b_green2.drawString(g, "Đang đến: " + ModNextMap.GetMapName(ModNextMap.nextMapTargetId) + " (" + ModNextMap.nextMapTargetId + ")", uiX + 18, uiY + 54, mFont.LEFT);
			ModUI.PaintNativeButton(uiX + uiW - 85, uiY + 48, 70, "HỦY ĐI", false, g);
		}
		else
		{
			mFont.tahoma_7_white.drawString(g, "Hiện tại: " + TileMap.mapName + " (" + TileMap.mapID + ") | Chọn map muốn đến:", uiX + 18, uiY + 54, mFont.LEFT);
		}

		// 3 Nút chọn hành tinh
		int pBtnW = 96;
		ModUI.PaintNativeButton(uiX + 18, uiY + 70, pBtnW, "Trái Đất (16)", ModNextMap.selectedPlanetTab == 0, g);
		ModUI.PaintNativeButton(uiX + 120, uiY + 70, pBtnW, "Namếc (14)", ModNextMap.selectedPlanetTab == 1, g);
		ModUI.PaintNativeButton(uiX + 222, uiY + 70, pBtnW, "Xayda (14)", ModNextMap.selectedPlanetTab == 2, g);

		// Khung danh sách các map
		int listY = uiY + 95;
		int listW = uiW - 36;
		int listH = 120;
		g.setColor(0x121212);
		g.fillRect(uiX + 18, listY, listW, listH);
		g.setColor(0x3c3c3c);
		g.drawRect(uiX + 18, listY, listW, listH);

		int[] currentPlanetMaps = ModNextMap.planetMapIds[ModNextMap.selectedPlanetTab];
		for (int m = 0; m < currentPlanetMaps.Length; m++)
		{
			int mId = currentPlanetMaps[m];
			string mName = ModNextMap.GetMapName(mId);
			int col = m % 2;
			int row = m / 2;
			int btnX = (col == 0) ? (uiX + 22) : (uiX + 172);
			int btnY = listY + 3 + row * 14;
			int btnW = 144;
			int btnH = 13;

			bool isCurrent = (mId == TileMap.mapID);
			bool isTarget = (mId == ModNextMap.nextMapTargetId && ModNextMap.isNextMapActive);

			if (isCurrent)
			{
				g.setColor(0x1b4d24);
				g.fillRect(btnX, btnY, btnW, btnH);
				g.setColor(0x4CAF50);
				g.drawRect(btnX, btnY, btnW, btnH);
				mFont.tahoma_7b_green2.drawString(g, mName + " (Hiện tại)", btnX + btnW / 2, btnY + 1, mFont.CENTER);
			}
			else if (isTarget)
			{
				g.setColor(0x5d3800);
				g.fillRect(btnX, btnY, btnW, btnH);
				g.setColor(0xff9800);
				g.drawRect(btnX, btnY, btnW, btnH);
				mFont.tahoma_7b_yellow.drawString(g, mName + " (Đang đến)", btnX + btnW / 2, btnY + 1, mFont.CENTER);
			}
			else
			{
				g.setColor(0x222222);
				g.fillRect(btnX, btnY, btnW, btnH);
				g.setColor(0x444444);
				g.drawRect(btnX, btnY, btnW, btnH);
				mFont.tahoma_7_white.drawString(g, mName, btnX + btnW / 2, btnY + 1, mFont.CENTER);
			}
		}
	}

	public static bool HandleTap(int px, int py, int uiX, int uiY, int uiW, int uiH)
	{
		if (ModNextMap.isNextMapActive)
		{
			if (px >= uiX + uiW - 85 && px <= uiX + uiW - 15 && py >= uiY + 48 && py <= uiY + 70)
			{
				ModNextMap.StopNextMap();
				GameScr.info1.addInfo("Đã hủy Next Map!", 0);
				SoundMn.gI().buttonClick();
				return true;
			}
		}

		int pBtnW = 96;
		if (px >= uiX + 18 && px <= uiX + 18 + pBtnW && py >= uiY + 70 && py <= uiY + 92)
		{
			ModNextMap.selectedPlanetTab = 0;
			SoundMn.gI().buttonClick();
			return true;
		}
		if (px >= uiX + 120 && px <= uiX + 120 + pBtnW && py >= uiY + 70 && py <= uiY + 92)
		{
			ModNextMap.selectedPlanetTab = 1;
			SoundMn.gI().buttonClick();
			return true;
		}
		if (px >= uiX + 222 && px <= uiX + 222 + pBtnW && py >= uiY + 70 && py <= uiY + 92)
		{
			ModNextMap.selectedPlanetTab = 2;
			SoundMn.gI().buttonClick();
			return true;
		}

		int listY = uiY + 95;
		int[] currentPlanetMaps = ModNextMap.planetMapIds[ModNextMap.selectedPlanetTab];
		for (int m = 0; m < currentPlanetMaps.Length; m++)
		{
			int col = m % 2;
			int row = m / 2;
			int btnX = (col == 0) ? (uiX + 22) : (uiX + 172);
			int btnY = listY + 3 + row * 14;
			int btnW = 144;
			int btnH = 13;

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
