using System;
using UnityEngine;

public static class ModUINextMap
{
	public static int scrollMapY = 0;
	public static int selectedMapId = -1;
	private static bool isDragging = false;
	private static bool hasDragged = false;
	private static int startDragX = 0;
	private static int startDragY = 0;
	private static int lastDragY = 0;

	public static void OnMouseScroll(float wheel)
	{
		int[] currentPlanetMaps = ModNextMap.planetMapIds[ModNextMap.selectedPlanetTab];
		int totalRows = (currentPlanetMaps.Length + 1) / 2;
		int contentH = totalRows * 18 + 4;
		int maxScroll = (contentH > 140) ? (contentH - 140) : 0;
		if (maxScroll <= 0) return;

		int step = 20;
		if (wheel > 0)
		{
			scrollMapY -= step;
			if (scrollMapY < 0) scrollMapY = 0;
		}
		else if (wheel < 0)
		{
			scrollMapY += step;
			if (scrollMapY > maxScroll) scrollMapY = maxScroll;
		}
	}

	public static void UpdateDragScroll(int px, int py, int uiX, int uiY, int uiW, int uiH)
	{
		int listX = uiX + 4;
		int listY = uiY + 43;
		int listW = uiW - 8;
		int listH = uiH - 47;
		if (listH < 40) listH = 40;

		int[] currentPlanetMaps = ModNextMap.planetMapIds[ModNextMap.selectedPlanetTab];
		int totalRows = (currentPlanetMaps.Length + 1) / 2;
		int contentH = totalRows * 18 + 4;
		int maxScroll = (contentH > listH - 4) ? (contentH - (listH - 4)) : 0;

		bool isDown = Input.GetMouseButton(0) || GameCanvas.isPointerDown;
		if (isDown)
		{
			if (!isDragging)
			{
				if (px >= listX && px <= listX + listW && py >= listY && py <= listY + listH)
				{
					isDragging = true;
					hasDragged = false;
					startDragX = px;
					startDragY = py;
					lastDragY = py;
				}
			}
			else
			{
				int moveY = py - lastDragY;
				lastDragY = py;
				if (Res.abs(py - startDragY) > 8 || Res.abs(px - startDragX) > 8)
				{
					hasDragged = true;
				}
				if (hasDragged && maxScroll > 0)
				{
					scrollMapY -= moveY;
					if (scrollMapY < 0) scrollMapY = 0;
					if (scrollMapY > maxScroll) scrollMapY = maxScroll;
				}
			}
		}
		else
		{
			isDragging = false;
		}
	}

	public static void Paint(int uiX, int uiY, int uiW, int uiH, mGraphics g)
	{
		if (ModNextMap.isNextMapActive)
		{
			mFont.tahoma_7_green2.drawString(g, "Đang đến: " + ModNextMap.GetMapName(ModNextMap.nextMapTargetId) + " (" + ModNextMap.nextMapTargetId + ")", uiX + 5, uiY + 7, mFont.LEFT);
			ModUI.PaintNativeButton(uiX + uiW - 56, uiY + 4, 52, 17, "HỦY", false, g);
		}
		else
		{
			string selText = (selectedMapId >= 0 && selectedMapId != TileMap.mapID) ? (" | Đến: " + ModNextMap.GetMapName(selectedMapId)) : " | Chọn map:";
			mFont.tahoma_7_grey.drawString(g, "Map: " + TileMap.mapName + selText, uiX + 5, uiY + 7, mFont.LEFT);
			if (selectedMapId >= 0 && selectedMapId != TileMap.mapID)
			{
				ModUI.PaintNativeButton(uiX + uiW - 56, uiY + 4, 52, 17, "ĐI ĐẾN", true, g);
			}
		}

		// 3 Nút chọn hành tinh
		int pBtnW = (uiW - 16) / 3;
		ModUI.PaintNativeButton(uiX + 4, uiY + 23, pBtnW, 17, "Trái Đất (" + ModNextMap.planetMapIds[0].Length + ")", ModNextMap.selectedPlanetTab == 0, g);
		ModUI.PaintNativeButton(uiX + 8 + pBtnW, uiY + 23, pBtnW, 17, "Namếc (" + ModNextMap.planetMapIds[1].Length + ")", ModNextMap.selectedPlanetTab == 1, g);
		ModUI.PaintNativeButton(uiX + 12 + pBtnW * 2, uiY + 23, pBtnW, 17, "Xayda (" + ModNextMap.planetMapIds[2].Length + ")", ModNextMap.selectedPlanetTab == 2, g);

		// Khung danh sách các map co dãn động
		int listX = uiX + 4;
		int listY = uiY + 43;
		int listW = uiW - 8;
		int listH = uiH - 47;
		if (listH < 40) listH = 40;

		GameCanvas.paintz.paintFrameSimple(listX, listY, listW, listH, g);
		g.setColor(15196114);
		g.fillRect(listX + 2, listY + 2, listW - 4, listH - 4);

		int[] currentPlanetMaps = ModNextMap.planetMapIds[ModNextMap.selectedPlanetTab];
		int totalRows = (currentPlanetMaps.Length + 1) / 2;
		int contentH = totalRows * 18 + 4;
		int maxScroll = (contentH > listH - 4) ? (contentH - (listH - 4)) : 0;
		if (scrollMapY > maxScroll) scrollMapY = maxScroll;
		if (scrollMapY < 0) scrollMapY = 0;

		int btnW = (listW - 10) / 2;
		int btnH = 16;

		g.setClip(listX + 2, listY + 2, listW - 4, listH - 4);
		for (int m = 0; m < currentPlanetMaps.Length; m++)
		{
			int mId = currentPlanetMaps[m];
			string mName = ModNextMap.GetMapName(mId);
			int col = m % 2;
			int row = m / 2;
			int btnX = (col == 0) ? (listX + 3) : (listX + 7 + btnW);
			int btnY = listY + 3 + row * 18 - scrollMapY;

			if (btnY + btnH < listY || btnY > listY + listH) continue;

			bool isCurrent = (mId == TileMap.mapID);
			bool isTarget = (mId == ModNextMap.nextMapTargetId && ModNextMap.isNextMapActive);
			bool isSelected = (mId == selectedMapId);

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
			else if (isSelected)
			{
				g.setColor(0xFFF9C4);
				g.fillRect(btnX, btnY, btnW, btnH);
				g.setColor(0xFBC02D);
				g.drawRect(btnX, btnY, btnW, btnH);
				(mFont.tahoma_7b_dark ?? mFont.tahoma_7_green2).drawString(g, mName + " [Chọn]", btnX + btnW / 2, btnY + 2, mFont.CENTER);
			}
			else
			{
				ModUI.PaintNativeButton(btnX, btnY, btnW, btnH, mName, false, g);
			}
		}
		g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
	}

	public static bool HandleTap(int px, int py, int uiX, int uiY, int uiW, int uiH)
	{
		if (hasDragged)
		{
			hasDragged = false;
			return true;
		}

		// Nút HỦY hoặc ĐI ĐẾN ở góc phải
		if (ModNextMap.isNextMapActive)
		{
			if (px >= uiX + uiW - 58 && px <= uiX + uiW - 2 && py >= uiY + 2 && py <= uiY + 22)
			{
				ModNextMap.StopNextMap();
				GameScr.info1.addInfo("Đã dừng Next Map!", 0);
				SoundMn.gI().buttonClick();
				return true;
			}
		}
		else if (selectedMapId >= 0 && selectedMapId != TileMap.mapID)
		{
			if (px >= uiX + uiW - 58 && px <= uiX + uiW - 2 && py >= uiY + 2 && py <= uiY + 22)
			{
				ModNextMap.StartNextMap(selectedMapId);
				ModUI.uiCustomOpen = false;
				SoundMn.gI().buttonClick();
				return true;
			}
		}

		int pBtnW = (uiW - 16) / 3;
		if (py >= uiY + 21 && py <= uiY + 41)
		{
			if (px >= uiX + 4 && px <= uiX + 4 + pBtnW)
			{
				ModNextMap.selectedPlanetTab = 0;
				scrollMapY = 0;
				SoundMn.gI().buttonClick();
				return true;
			}
			if (px >= uiX + 8 + pBtnW && px <= uiX + 8 + pBtnW * 2)
			{
				ModNextMap.selectedPlanetTab = 1;
				scrollMapY = 0;
				SoundMn.gI().buttonClick();
				return true;
			}
			if (px >= uiX + 12 + pBtnW * 2 && px <= uiX + 12 + pBtnW * 3)
			{
				ModNextMap.selectedPlanetTab = 2;
				scrollMapY = 0;
				SoundMn.gI().buttonClick();
				return true;
			}
		}

		int listX = uiX + 4;
		int listY = uiY + 43;
		int listW = uiW - 8;
		int listH = uiH - 47;
		if (listH < 40) listH = 40;
		int btnW = (listW - 10) / 2;
		int btnH = 16;

		if (px >= listX && px <= listX + listW && py >= listY && py <= listY + listH)
		{
			int[] currentPlanetMaps = ModNextMap.planetMapIds[ModNextMap.selectedPlanetTab];
			for (int m = 0; m < currentPlanetMaps.Length; m++)
			{
				int col = m % 2;
				int row = m / 2;
				int btnX = (col == 0) ? (listX + 3) : (listX + 7 + btnW);
				int btnY = listY + 3 + row * 18 - scrollMapY;

				if (px >= btnX && px <= btnX + btnW && py >= btnY && py <= btnY + btnH)
				{
					int targetMapId = currentPlanetMaps[m];
					if (targetMapId == TileMap.mapID)
					{
						GameScr.info1.addInfo("Bạn đang ở " + ModNextMap.GetMapName(targetMapId) + " rồi!", 0);
						selectedMapId = -1;
					}
					else if (selectedMapId == targetMapId)
					{
						// Chạm lần 2 vào map đã chọn -> Bắt đầu đi luôn!
						ModNextMap.StartNextMap(targetMapId);
						ModUI.uiCustomOpen = false;
					}
					else
					{
						// Chạm lần 1 -> Chỉ chọn (Highlight), KHÔNG TỰ Ý ĐI VÀ KHÔNG ĐÓNG MENU!
						selectedMapId = targetMapId;
						GameScr.info1.addInfo("Đã chọn: " + ModNextMap.GetMapName(targetMapId) + " (Bấm 'ĐI ĐẾN' để bắt đầu)", 0);
					}
					SoundMn.gI().buttonClick();
					return true;
				}
			}
		}

		return false;
	}
}
