using System;

public static class ModUIGoBack
{
	public static void Paint(int uiX, int uiY, int uiW, int uiH, mGraphics g)
	{
		// Hàng 1: Công tắc Bật/Tắt GoBack & Tự định toạ độ khi chết
		mFont.tahoma_7b_dark.drawString(g, "GoBack Map:", uiX + 8, uiY + 12, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 84, uiY + 8, 48, 18, ModGoBack.isGoBackActive ? "BẬT" : "TẮT", ModGoBack.isGoBackActive, g);

		int rBtnW = 48;
		int rBtnX = uiX + uiW - rBtnW - 6;
		mFont.tahoma_7b_dark.drawString(g, "Tự định khi chết:", rBtnX - 98, uiY + 12, mFont.LEFT);
		ModUI.PaintNativeButton(rBtnX, uiY + 8, rBtnW, 18, ModGoBack.isAutoRecordOnDeath ? "BẬT" : "TẮT", ModGoBack.isAutoRecordOnDeath, g);

		// Khung hiển thị thông tin toạ độ đã lưu
		int listY = uiY + 32;
		int listW = uiW - 16;
		int listH = 150;
		GameCanvas.paintz.paintFrameSimple(uiX + 8, listY, listW, listH, g);
		g.setColor(15196114);
		g.fillRect(uiX + 10, listY + 2, listW - 4, listH - 4);

		// Dòng tiêu đề box
		mFont.tahoma_7b_dark.drawString(g, "VỊ TRÍ GOBACK ĐÃ LƯU:", uiX + 16, listY + 8, mFont.LEFT);

		// Bản đồ
		string mapText = (ModGoBack.savedMapId > 0)
			? (ModNextMap.GetMapName(ModGoBack.savedMapId) + " (ID: " + ModGoBack.savedMapId + ")")
			: "(Chưa lưu - sẽ tự định khi chết)";
		mFont.tahoma_7b_dark.drawString(g, "- Bản đồ: ", uiX + 16, listY + 28, mFont.LEFT);
		(ModGoBack.savedMapId > 0 ? mFont.tahoma_7b_green2 : mFont.tahoma_7_grey).drawString(g, mapText, uiX + 68, listY + 28, mFont.LEFT);

		// Khu vực
		string zoneText = (ModGoBack.savedZoneId >= 0)
			? ("Khu " + ModGoBack.savedZoneId)
			: "(Chưa lưu)";
		mFont.tahoma_7b_dark.drawString(g, "- Khu vực: ", uiX + 16, listY + 48, mFont.LEFT);
		(ModGoBack.savedZoneId >= 0 ? mFont.tahoma_7b_green2 : mFont.tahoma_7_grey).drawString(g, zoneText, uiX + 72, listY + 48, mFont.LEFT);

		// Tọa độ
		string coordText = (ModGoBack.savedX > 0 && ModGoBack.savedY > 0)
			? ("X: " + ModGoBack.savedX + " | Y: " + ModGoBack.savedY)
			: "(Chưa lưu)";
		mFont.tahoma_7b_dark.drawString(g, "- Tọa độ: ", uiX + 16, listY + 68, mFont.LEFT);
		(ModGoBack.savedX > 0 ? mFont.tahoma_7b_green2 : mFont.tahoma_7_grey).drawString(g, coordText, uiX + 68, listY + 68, mFont.LEFT);

		// Trạng thái vận hành
		mFont.tahoma_7b_dark.drawString(g, "- Trạng thái: ", uiX + 16, listY + 88, mFont.LEFT);
		mFont.tahoma_7b_green2.drawString(g, ModGoBack.GetStatusText(), uiX + 86, listY + 88, mFont.LEFT);

		// Vị trí hiện tại của nhân vật
		Char me = Char.myCharz();
		int curX = (me != null) ? me.cx : 0;
		int curY = (me != null) ? me.cy : 0;
		string curPosText = "Hiện tại: " + TileMap.mapName + " [K." + TileMap.zoneID + "] (" + curX + ", " + curY + ")";
		mFont.tahoma_7_grey.drawString(g, curPosText, uiX + 16, listY + 110, mFont.LEFT);

		// Ghi chú
		mFont.tahoma_7_grey.drawString(g, "* Khi chết sẽ tự động bay/chạy quay về đúng toạ độ đã lưu", uiX + 16, listY + 130, mFont.LEFT);

		// Hàng 3 nút chức năng ở đáy
		int btnY = uiY + 190;
		int b1W = (uiW - 24) * 30 / 100;
		int b2W = (uiW - 24) * 30 / 100;
		int b3W = uiW - 24 - b1W - b2W;
		int b1X = uiX + 8;
		int b2X = b1X + b1W + 4;
		int b3X = b2X + b2W + 4;
		ModUI.PaintNativeButton(b1X, btnY, b1W, 22, "Lưu Vị Trí", false, g);
		ModUI.PaintNativeButton(b2X, btnY, b2W, 22, "Xóa Vị Trí", false, g);
		ModUI.PaintNativeButton(b3X, btnY, b3W, 22, "Về Ngay", false, g);
	}

	public static bool HandleTap(int px, int py, int uiX, int uiY, int uiW, int uiH)
	{
		int btnY = uiY + 190;
		int rBtnW = 48;
		int rBtnX = uiX + uiW - rBtnW - 6;

		// 1. Nút Bật/Tắt GoBack Map
		if (px >= uiX + 84 && px <= uiX + 132 && py >= uiY + 8 && py <= uiY + 26)
		{
			ModGoBack.ToggleGoBack();
			return true;
		}

		// 2. Nút Bật/Tắt Tự định khi chết
		if (px >= rBtnX && px <= rBtnX + rBtnW && py >= uiY + 8 && py <= uiY + 26)
		{
			ModGoBack.isAutoRecordOnDeath = !ModGoBack.isAutoRecordOnDeath;
			ModConfig.SaveConfig();
			GameScr.info1.addInfo("Tự định khi chết: " + (ModGoBack.isAutoRecordOnDeath ? "BẬT" : "TẮT"), 0);
			SoundMn.gI().buttonClick();
			return true;
		}

		int b1W = (uiW - 24) * 30 / 100;
		int b2W = (uiW - 24) * 30 / 100;
		int b3W = uiW - 24 - b1W - b2W;
		int b1X = uiX + 8;
		int b2X = b1X + b1W + 4;
		int b3X = b2X + b2W + 4;

		// 3. Nút "Lưu Vị Trí"
		if (px >= b1X && px <= b1X + b1W && py >= btnY && py <= btnY + 22)
		{
			ModGoBack.SaveCurrentPosition();
			return true;
		}

		// 4. Nút "Xóa Vị Trí"
		if (px >= b2X && px <= b2X + b2W && py >= btnY && py <= btnY + 22)
		{
			ModGoBack.ResetPosition();
			return true;
		}

		// 5. Nút "Về Chỗ Này Ngay"
		if (px >= b3X && px <= b3X + b3W && py >= btnY && py <= btnY + 22)
		{
			ModGoBack.StartGoBackNow();
			return true;
		}

		return false;
	}
}
