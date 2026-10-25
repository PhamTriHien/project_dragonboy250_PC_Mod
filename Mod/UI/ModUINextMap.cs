using System;
using UnityEngine;

/// <summary>
/// CLASS ModUINextMap: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public static class ModUINextMap
{
	public static int scrollMapY = 0;
	public static int selectedMapId = -1;
	private static bool isDragging = false;
	private static bool hasDragged = false;
	private static int startDragX = 0;
	private static int startDragY = 0;
	private static int lastDragY = 0;

	/// <summary>
	/// Bộ xử lý sự kiện tiếp nhận và phản hồi OnMouseScroll.
	/// </summary>
	public static void OnMouseScroll(float wheel)
	{
		if (ModNextMap.selectedPlanetTab < 0 || ModNextMap.selectedPlanetTab >= ModNextMap.planetMapIds.Length)
		{
			ModNextMap.selectedPlanetTab = 0;
		}
		int[] currentPlanetMaps = ModNextMap.planetMapIds[ModNextMap.selectedPlanetTab];
		int totalRows = (currentPlanetMaps.Length + 1) / 2;
		int contentH = totalRows * 18 + 4;
		int maxScroll = (contentH > 125) ? (contentH - 125) : 0;
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

	/// <summary>
	/// Cập nhật trạng thái logic của UpdateDragScroll theo chu kỳ khung hình.
	/// </summary>
	public static void UpdateDragScroll(int px, int py, int uiX, int uiY, int uiW, int uiH)
	{
		int listX = uiX + 4;
		int listY = uiY + 73;
		int listW = uiW - 8;
		int listH = uiH - 77;
		if (listH < 40) listH = 40;

		if (ModNextMap.selectedPlanetTab < 0 || ModNextMap.selectedPlanetTab >= ModNextMap.planetMapIds.Length)
		{
			ModNextMap.selectedPlanetTab = 0;
		}
		int[] currentPlanetMaps = ModNextMap.planetMapIds[ModNextMap.selectedPlanetTab];
		int totalRows = (currentPlanetMaps.Length + 1) / 2;
		int contentH = totalRows * 18 + 4;
		int maxScroll = (contentH > listH - 4) ? (contentH - (listH - 4)) : 0;

		bool isDown = GameCanvas.isPointerDown;
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

	/// <summary>
	/// Thực hiện vẽ toàn bộ đối tượng và các lớp hiển thị lên canvas mGraphics.
	/// </summary>
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

		if (ModNextMap.selectedPlanetTab < 0 || ModNextMap.selectedPlanetTab >= ModNextMap.planetMapIds.Length)
		{
			ModNextMap.selectedPlanetTab = 0;
		}

		// 7 Nút chọn hành tinh / khu vực (Hàng 1: 3 cột; Hàng 2: 4 cột)
		int row1BtnW = (uiW - 16) / 3;
		int row2Count = System.Math.Max(1, ModNextMap.planetMapIds.Length - 3);
		int row2BtnW = (uiW - 8 - (row2Count - 1) * 4) / row2Count;
		for (int t = 0; t < ModNextMap.planetMapIds.Length; t++)
		{
			int bx;
			int by;
			int bw;
			if (t < 3)
			{
				bw = row1BtnW;
				bx = uiX + 4 + t * (row1BtnW + 4);
				by = uiY + 22;
			}
			else
			{
				int col2 = t - 3;
				bw = row2BtnW;
				bx = uiX + 4 + col2 * (row2BtnW + 4);
				by = uiY + 38;
			}
			string tabLabel = ModNextMapData.planetTabNames[t] + " (" + ModNextMap.planetMapIds[t].Length + ")";
			ModUI.PaintNativeButton(bx, by, bw, 15, tabLabel, ModNextMap.selectedPlanetTab == t, g);
		}

		// Hàng tùy chọn Xmap nhanh: CS VIP, CS Thường, Tua Tàu
		int optBtnW = (uiW - 16) / 3;
		ModUI.PaintNativeButton(uiX + 4, uiY + 55, optBtnW, 15, "CS VIP: " + (ModNextMap.useCapsuleVip ? "BẬT" : "TẮT"), ModNextMap.useCapsuleVip, g);
		ModUI.PaintNativeButton(uiX + 8 + optBtnW, uiY + 55, optBtnW, 15, "CS Thường: " + (ModNextMap.useNormalCapsule ? "BẬT" : "TẮT"), ModNextMap.useNormalCapsule, g);
		ModUI.PaintNativeButton(uiX + 12 + optBtnW * 2, uiY + 55, optBtnW, 15, "Tua Tàu: " + (ModNextMap.speedBoostTransport ? "BẬT" : "TẮT"), ModNextMap.speedBoostTransport, g);

		// Khung danh sách các map co dãn động
		int listX = uiX + 4;
		int listY = uiY + 73;
		int listW = uiW - 8;
		int listH = uiH - 77;
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
				ModUI.PaintNativeButton(btnX, btnY, btnW, btnH, mName + " (Hiện tại)", true, g);
			}
			else if (isTarget)
			{
				ModUI.PaintNativeButton(btnX, btnY, btnW, btnH, mName + " (Đang đến)", true, g);
			}
			else if (isSelected)
			{
				ModUI.PaintNativeButton(btnX, btnY, btnW, btnH, mName + " [Chọn]", true, g);
			}
			else
			{
				ModUI.PaintNativeButton(btnX, btnY, btnW, btnH, mName, false, g);
			}
		}
		g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
	}

	/// <summary>
	/// Xử lý nghiệp vụ logic chuyên trách cho HandleTap.
	/// </summary>
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

		int row1BtnW = (uiW - 16) / 3;
		int row2Count = System.Math.Max(1, ModNextMap.planetMapIds.Length - 3);
		int row2BtnW = (uiW - 8 - (row2Count - 1) * 4) / row2Count;
		for (int t = 0; t < ModNextMap.planetMapIds.Length; t++)
		{
			int bw = (t < 3) ? row1BtnW : row2BtnW;
			int bx = (t < 3) ? (uiX + 4 + t * (row1BtnW + 4)) : (uiX + 4 + (t - 3) * (row2BtnW + 4));
			int by = (t < 3) ? (uiY + 22) : (uiY + 38);
			if (px >= bx && px <= bx + bw && py >= by && py <= by + 15)
			{
				ModNextMap.selectedPlanetTab = t;
				scrollMapY = 0;
				SoundMn.gI().buttonClick();
				return true;
			}
		}

		int optBtnW = (uiW - 16) / 3;
		if (py >= uiY + 54 && py <= uiY + 71)
		{
			if (px >= uiX + 4 && px <= uiX + 4 + optBtnW)
			{
				ModNextMap.useCapsuleVip = !ModNextMap.useCapsuleVip;
				ModConfig.SaveConfig();
				SoundMn.gI().buttonClick();
				return true;
			}
			if (px >= uiX + 8 + optBtnW && px <= uiX + 8 + optBtnW * 2)
			{
				ModNextMap.useNormalCapsule = !ModNextMap.useNormalCapsule;
				ModConfig.SaveConfig();
				SoundMn.gI().buttonClick();
				return true;
			}
			if (px >= uiX + 12 + optBtnW * 2 && px <= uiX + 12 + optBtnW * 3)
			{
				ModNextMap.speedBoostTransport = !ModNextMap.speedBoostTransport;
				ModConfig.SaveConfig();
				SoundMn.gI().buttonClick();
				return true;
			}
		}

		int listX = uiX + 4;
		int listY = uiY + 73;
		int listW = uiW - 8;
		int listH = uiH - 77;
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
