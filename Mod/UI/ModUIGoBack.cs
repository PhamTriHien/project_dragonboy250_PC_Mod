using System;

public static class ModUIGoBack
{
	public static void Paint(int uiX, int uiY, int uiW, int uiH, mGraphics g)
	{
		int halfW = (uiW - 14) / 2;
		int item1X = uiX + 4;
		int item2X = uiX + 8 + halfW;

		// Hàng 1: Công tắc Bật/Tắt GoBack & Tự định toạ độ khi chết
		mFont.tahoma_7_grey.drawString(g, "GoBack Map:", item1X + 2, uiY + 7, mFont.LEFT);
		ModUI.PaintNativeButton(item1X + halfW - 38, uiY + 4, 38, 17, ModGoBack.isGoBackActive ? "BẬT" : "TẮT", ModGoBack.isGoBackActive, g);

		mFont.tahoma_7_grey.drawString(g, "Tự lưu khi chết:", item2X + 2, uiY + 7, mFont.LEFT);
		ModUI.PaintNativeButton(item2X + halfW - 38, uiY + 4, 38, 17, ModGoBack.isAutoRecordOnDeath ? "BẬT" : "TẮT", ModGoBack.isAutoRecordOnDeath, g);

		int footerH = 18;
		int footerY = uiY + uiH - footerH - 4;

		// Khung hiển thị thông tin toạ độ đã lưu co dãn động
		int listY = uiY + 24;
		int listW = uiW - 8;
		int listH = (footerY - 4) - listY;
		if (listH < 40) listH = 40;

		GameCanvas.paintz.paintFrameSimple(uiX + 4, listY, listW, listH, g);
		g.setColor(15196114);
		g.fillRect(uiX + 6, listY + 2, listW - 4, listH - 4);

		// Dòng tiêu đề box
		mFont.tahoma_7_green2.drawString(g, "VỊ TRÍ GOBACK ĐÃ LƯU:", uiX + 10, listY + 4, mFont.LEFT);

		// Bản đồ
		string mapText = (ModGoBack.savedMapId > 0)
			? (ModNextMap.GetMapName(ModGoBack.savedMapId) + " (" + ModGoBack.savedMapId + ")")
			: "(Chưa lưu - sẽ tự định khi chết)";
		mFont.tahoma_7_grey.drawString(g, "- Bản đồ: ", uiX + 10, listY + 20, mFont.LEFT);
		(ModGoBack.savedMapId > 0 ? mFont.tahoma_7_green2 : mFont.tahoma_7_grey).drawString(g, mapText, uiX + 56, listY + 20, mFont.LEFT);

		// Khu vực
		string zoneText = (ModGoBack.savedZoneId >= 0)
			? ("Khu " + ModGoBack.savedZoneId)
			: "(Chưa lưu)";
		mFont.tahoma_7_grey.drawString(g, "- Khu vực: ", uiX + 10, listY + 38, mFont.LEFT);
		(ModGoBack.savedZoneId >= 0 ? mFont.tahoma_7_green2 : mFont.tahoma_7_grey).drawString(g, zoneText, uiX + 60, listY + 38, mFont.LEFT);

		// Tọa độ
		string coordText = (ModGoBack.savedX > 0 && ModGoBack.savedY > 0)
			? ("X: " + ModGoBack.savedX + " | Y: " + ModGoBack.savedY)
			: "(Chưa lưu)";
		mFont.tahoma_7_grey.drawString(g, "- Tọa độ: ", uiX + 10, listY + 56, mFont.LEFT);
		(ModGoBack.savedX > 0 ? mFont.tahoma_7_green2 : mFont.tahoma_7_grey).drawString(g, coordText, uiX + 56, listY + 56, mFont.LEFT);

		// Trạng thái vận hành
		mFont.tahoma_7_grey.drawString(g, "- Trạng thái: ", uiX + 10, listY + 74, mFont.LEFT);
		mFont.tahoma_7_green2.drawString(g, ModGoBack.GetStatusText(), uiX + 70, listY + 74, mFont.LEFT);

		// Vị trí hiện tại của nhân vật
		Char me = Char.myCharz();
		int curX = (me != null) ? me.cx : 0;
		int curY = (me != null) ? me.cy : 0;
		string curPosText = "Hiện tại: " + TileMap.mapName + " [K." + TileMap.zoneID + "] (" + curX + ", " + curY + ")";
		mFont.tahoma_7_grey.drawString(g, curPosText, uiX + 10, listY + 94, mFont.LEFT);

		// Hàng 3 nút chức năng ở đáy
		int b1W = (uiW - 16) / 3;
		int b2W = (uiW - 16) / 3;
		int b3W = uiW - 8 - b1W - b2W - 6;
		int b1X = uiX + 4;
		int b2X = b1X + b1W + 3;
		int b3X = b2X + b2W + 3;
		ModUI.PaintNativeButton(b1X, footerY, b1W, footerH, "Lưu Vị Trí", false, g);
		ModUI.PaintNativeButton(b2X, footerY, b2W, footerH, "Xóa Vị Trí", false, g);
		ModUI.PaintNativeButton(b3X, footerY, b3W, footerH, "Về Ngay", false, g);
	}

	public static bool HandleTap(int px, int py, int uiX, int uiY, int uiW, int uiH)
	{
		int halfW = (uiW - 14) / 2;
		int item1X = uiX + 4;
		int item2X = uiX + 8 + halfW;

		// 1. Nút Bật/Tắt GoBack Map
		if (px >= item1X + halfW - 40 && px <= item1X + halfW + 2 && py >= uiY + 2 && py <= uiY + 22)
		{
			ModGoBack.ToggleGoBack();
			return true;
		}

		// 2. Nút Bật/Tắt Tự định khi chết
		if (px >= item2X + halfW - 40 && px <= item2X + halfW + 2 && py >= uiY + 2 && py <= uiY + 22)
		{
			ModGoBack.isAutoRecordOnDeath = !ModGoBack.isAutoRecordOnDeath;
			ModConfig.SaveConfig();
			GameScr.info1.addInfo("Tự định khi chết: " + (ModGoBack.isAutoRecordOnDeath ? "BẬT" : "TẮT"), 0);
			SoundMn.gI().buttonClick();
			return true;
		}

		int footerH = 18;
		int footerY = uiY + uiH - footerH - 4;
		int b1W = (uiW - 16) / 3;
		int b2W = (uiW - 16) / 3;
		int b3W = uiW - 8 - b1W - b2W - 6;
		int b1X = uiX + 4;
		int b2X = b1X + b1W + 3;
		int b3X = b2X + b2W + 3;

		if (py >= footerY - 1 && py <= footerY + footerH + 1)
		{
			// 3. Nút "Lưu Vị Trí"
			if (px >= b1X && px <= b1X + b1W)
			{
				ModGoBack.SaveCurrentPosition();
				return true;
			}

			// 4. Nút "Xóa Vị Trí"
			if (px >= b2X && px <= b2X + b2W)
			{
				ModGoBack.ResetPosition();
				return true;
			}

			// 5. Nút "Về Chỗ Này Ngay"
			if (px >= b3X && px <= b3X + b3W)
			{
				ModGoBack.StartGoBackNow();
				return true;
			}
		}

		return false;
	}
}
