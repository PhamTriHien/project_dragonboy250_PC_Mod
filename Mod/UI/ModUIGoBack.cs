using System;

public static class ModUIGoBack
{
	public static void Paint(int uiX, int uiY, int uiW, int uiH, mGraphics g)
	{
		// Hàng 1: Công tắc Bật/Tắt GoBack & Tự định toạ độ khi chết
		mFont.tahoma_7b_white.drawString(g, "GoBack Map:", uiX + 18, uiY + 52, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 96, uiY + 48, 54, 18, ModGoBack.isGoBackActive ? "BẬT" : "TẮT", ModGoBack.isGoBackActive, g);

		mFont.tahoma_7b_white.drawString(g, "Tự định khi chết:", uiX + 165, uiY + 52, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 268, uiY + 48, 54, 18, ModGoBack.isAutoRecordOnDeath ? "BẬT" : "TẮT", ModGoBack.isAutoRecordOnDeath, g);

		// Khung hiển thị thông tin toạ độ đã lưu
		int listY = uiY + 72;
		int listW = uiW - 36;
		int listH = 110;
		g.setColor(0x121212);
		g.fillRect(uiX + 18, listY, listW, listH);
		g.setColor(0x3c3c3c);
		g.drawRect(uiX + 18, listY, listW, listH);

		// Dòng tiêu đề box
		mFont.tahoma_7b_yellow.drawString(g, "VỊ TRÍ GOBACK ĐÃ LƯU:", uiX + 26, listY + 8, mFont.LEFT);

		// Bản đồ
		string mapText = (ModGoBack.savedMapId > 0)
			? (ModNextMap.GetMapName(ModGoBack.savedMapId) + " (ID: " + ModGoBack.savedMapId + ")")
			: "(Chưa lưu - sẽ tự định khi chết)";
		mFont.tahoma_7_white.drawString(g, "- Bản đồ: ", uiX + 26, listY + 24, mFont.LEFT);
		(ModGoBack.savedMapId > 0 ? mFont.tahoma_7b_green2 : mFont.tahoma_7_grey).drawString(g, mapText, uiX + 74, listY + 24, mFont.LEFT);

		// Khu vực
		string zoneText = (ModGoBack.savedZoneId >= 0)
			? ("Khu " + ModGoBack.savedZoneId)
			: "(Chưa lưu)";
		mFont.tahoma_7_white.drawString(g, "- Khu vực: ", uiX + 26, listY + 39, mFont.LEFT);
		(ModGoBack.savedZoneId >= 0 ? mFont.tahoma_7b_green2 : mFont.tahoma_7_grey).drawString(g, zoneText, uiX + 78, listY + 39, mFont.LEFT);

		// Tọa độ
		string coordText = (ModGoBack.savedX > 0 && ModGoBack.savedY > 0)
			? ("X: " + ModGoBack.savedX + " | Y: " + ModGoBack.savedY)
			: "(Chưa lưu)";
		mFont.tahoma_7_white.drawString(g, "- Tọa độ: ", uiX + 26, listY + 54, mFont.LEFT);
		(ModGoBack.savedX > 0 ? mFont.tahoma_7b_green2 : mFont.tahoma_7_grey).drawString(g, coordText, uiX + 74, listY + 54, mFont.LEFT);

		// Trạng thái vận hành
		mFont.tahoma_7_yellow.drawString(g, "- Trạng thái: ", uiX + 26, listY + 70, mFont.LEFT);
		mFont.tahoma_7b_white.drawString(g, ModGoBack.GetStatusText(), uiX + 90, listY + 70, mFont.LEFT);

		// Vị trí hiện tại của nhân vật
		Char me = Char.myCharz();
		int curX = (me != null) ? me.cx : 0;
		int curY = (me != null) ? me.cy : 0;
		string curPosText = "Hiện tại: " + TileMap.mapName + " [K." + TileMap.zoneID + "] (" + curX + ", " + curY + ")";
		mFont.tahoma_7_grey.drawString(g, curPosText, uiX + 26, listY + 90, mFont.LEFT);

		// Hàng 3 nút chức năng
		ModUI.PaintNativeButton(uiX + 18, uiY + 190, 96, 20, "Lưu Vị Trí Này", false, g);
		ModUI.PaintNativeButton(uiX + 120, uiY + 190, 78, 20, "Xóa Vị Trí", false, g);
		ModUI.PaintNativeButton(uiX + 204, uiY + 190, 118, 20, "Về Chỗ Này Ngay", false, g);
	}

	public static bool HandleTap(int px, int py, int uiX, int uiY, int uiW, int uiH)
	{
		// 1. Nút Bật/Tắt GoBack Map
		if (px >= uiX + 96 && px <= uiX + 150 && py >= uiY + 48 && py <= uiY + 66)
		{
			ModGoBack.ToggleGoBack();
			return true;
		}

		// 2. Nút Bật/Tắt Tự định khi chết
		if (px >= uiX + 268 && px <= uiX + 322 && py >= uiY + 48 && py <= uiY + 66)
		{
			ModGoBack.isAutoRecordOnDeath = !ModGoBack.isAutoRecordOnDeath;
			ModConfig.SaveConfig();
			GameScr.info1.addInfo("Tự định khi chết: " + (ModGoBack.isAutoRecordOnDeath ? "BẬT" : "TẮT"), 0);
			SoundMn.gI().buttonClick();
			return true;
		}

		// 3. Nút "Lưu Vị Trí Này"
		if (px >= uiX + 18 && px <= uiX + 114 && py >= uiY + 190 && py <= uiY + 210)
		{
			ModGoBack.SaveCurrentPosition();
			return true;
		}

		// 4. Nút "Xóa Vị Trí"
		if (px >= uiX + 120 && px <= uiX + 198 && py >= uiY + 190 && py <= uiY + 210)
		{
			ModGoBack.ResetPosition();
			return true;
		}

		// 5. Nút "Về Chỗ Này Ngay"
		if (px >= uiX + 204 && px <= uiX + 322 && py >= uiY + 190 && py <= uiY + 210)
		{
			ModGoBack.StartGoBackNow();
			return true;
		}

		return false;
	}
}
