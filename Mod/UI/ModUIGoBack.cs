using System;

/// <summary>
/// CLASS ModUIGoBack: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public static class ModUIGoBack
{
	/// <summary>
	/// Thực hiện vẽ toàn bộ đối tượng và các lớp hiển thị lên canvas mGraphics.
	/// </summary>
	public static void Paint(int uiX, int uiY, int uiW, int uiH, mGraphics g)
	{
		int r1W = (uiW - 16) / 3;
		ModUI.PaintNativeButton(uiX + 4, uiY + 4, r1W, 17, "GoBack: " + (ModGoBack.isGoBackActive ? "BẬT" : "TẮT"), ModGoBack.isGoBackActive, g);
		ModUI.PaintNativeButton(uiX + 8 + r1W, uiY + 4, r1W, 17, "Lưu Chết: " + (ModGoBack.isAutoRecordOnDeath ? "BẬT" : "TẮT"), ModGoBack.isAutoRecordOnDeath, g);
		ModUI.PaintNativeButton(uiX + 12 + r1W * 2, uiY + 4, r1W, 17, "Về KI<" + ModGoBack.lowMpPercent + "%: " + (ModGoBack.gobackOnLowMp ? "BẬT" : "TẮT"), ModGoBack.gobackOnLowMp, g);

		int r2W = (uiW - 12) / 2;
		ModUI.PaintNativeButton(uiX + 4, uiY + 23, r2W, 16, "Mức KI Về: < " + ModGoBack.lowMpPercent + "%", ModGoBack.gobackOnLowMp, g);
		ModUI.PaintNativeButton(uiX + 8 + r2W, uiY + 23, r2W, 16, "Ăn Đùi Gà Nhà: " + (ModGoBack.eatChickenAtHome ? "BẬT" : "TẮT"), ModGoBack.eatChickenAtHome, g);

		int footerH = 18;
		int footerY = uiY + uiH - footerH - 4;

		// Khung hiển thị thông tin toạ độ đã lưu co dãn động
		int listY = uiY + 42;
		int listW = uiW - 8;
		int listH = (footerY - 4) - listY;
		if (listH < 40) listH = 40;

		GameCanvas.paintz.paintFrameSimple(uiX + 4, listY, listW, listH, g);
		g.setColor(15196114);
		g.fillRect(uiX + 6, listY + 2, listW - 4, listH - 4);

		// Dòng tiêu đề box
		mFont.tahoma_7_green2.drawString(g, "VỊ TRÍ GOBACK ĐÃ LƯU:", uiX + 10, listY + 4, mFont.LEFT);

		// Bản đồ
		string mapText = (ModGoBack.savedMapId >= 0)
			? (ModNextMap.GetMapName(ModGoBack.savedMapId) + " (" + ModGoBack.savedMapId + ")")
			: "(Chưa lưu - sẽ tự định khi chết)";
		mFont.tahoma_7_grey.drawString(g, "- Bản đồ: ", uiX + 10, listY + 20, mFont.LEFT);
		(ModGoBack.savedMapId >= 0 ? mFont.tahoma_7_green2 : mFont.tahoma_7_grey).drawString(g, mapText, uiX + 56, listY + 20, mFont.LEFT);

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

		// Hàng 4 nút chức năng ở đáy
		int btnW = (uiW - 20) / 4;
		ModUI.PaintNativeButton(uiX + 4, footerY, btnW, footerH, "Lưu Vị Trí", false, g);
		ModUI.PaintNativeButton(uiX + 8 + btnW, footerY, btnW, footerH, "Nhập TĐ", false, g);
		ModUI.PaintNativeButton(uiX + 12 + btnW * 2, footerY, btnW, footerH, "Xóa Vị Trí", false, g);
		ModUI.PaintNativeButton(uiX + 16 + btnW * 3, footerY, btnW, footerH, "Về Ngay", false, g);
	}

	/// <summary>
	/// Xử lý nghiệp vụ logic chuyên trách cho HandleTap.
	/// </summary>
	public static bool HandleTap(int px, int py, int uiX, int uiY, int uiW, int uiH)
	{
		int r1W = (uiW - 16) / 3;
		if (py >= uiY + 2 && py <= uiY + 22)
		{
			if (px >= uiX + 4 && px <= uiX + 4 + r1W)
			{
				ModGoBack.ToggleGoBack();
				return true;
			}
			if (px >= uiX + 8 + r1W && px <= uiX + 8 + r1W * 2)
			{
				ModGoBack.isAutoRecordOnDeath = !ModGoBack.isAutoRecordOnDeath;
				ModConfig.SaveConfig();
				GameScr.info1.addInfo("Tự định khi chết: " + (ModGoBack.isAutoRecordOnDeath ? "BẬT" : "TẮT"), 0);
				SoundMn.gI().buttonClick();
				return true;
			}
			if (px >= uiX + 12 + r1W * 2 && px <= uiX + 12 + r1W * 3)
			{
				ModGoBack.gobackOnLowMp = !ModGoBack.gobackOnLowMp;
				ModConfig.SaveConfig();
				GameScr.info1.addInfo("Về nhà hồi KI khi KI < " + ModGoBack.lowMpPercent + "%: " + (ModGoBack.gobackOnLowMp ? "BẬT" : "TẮT"), 0);
				SoundMn.gI().buttonClick();
				return true;
			}
		}

		int r2W = (uiW - 12) / 2;
		if (py >= uiY + 22 && py <= uiY + 40)
		{
			if (px >= uiX + 4 && px <= uiX + 4 + r2W)
			{
				ModGoBack.CycleLowMpPercent();
				return true;
			}
			if (px >= uiX + 8 + r2W && px <= uiX + 8 + r2W * 2)
			{
				ModGoBack.eatChickenAtHome = !ModGoBack.eatChickenAtHome;
				ModConfig.SaveConfig();
				GameScr.info1.addInfo("Tự ăn Đùi Gà ở nhà: " + (ModGoBack.eatChickenAtHome ? "BẬT" : "TẮT"), 0);
				SoundMn.gI().buttonClick();
				return true;
			}
		}

		int footerH = 18;
		int footerY = uiY + uiH - footerH - 4;
		int btnW = (uiW - 20) / 4;

		if (py >= footerY - 1 && py <= footerY + footerH + 1)
		{
			if (px >= uiX + 4 && px <= uiX + 4 + btnW)
			{
				ModGoBack.SaveCurrentPosition();
				return true;
			}
			if (px >= uiX + 8 + btnW && px <= uiX + 8 + btnW * 2)
			{
				ModUI.uiCustomOpen = false;
				ModGoBack.ShowInputCustomGoBack();
				SoundMn.gI().buttonClick();
				return true;
			}
			if (px >= uiX + 12 + btnW * 2 && px <= uiX + 12 + btnW * 3)
			{
				ModGoBack.ResetPosition();
				return true;
			}
			if (px >= uiX + 16 + btnW * 3 && px <= uiX + 16 + btnW * 4)
			{
				ModGoBack.StartGoBackNow();
				return true;
			}
		}

		return false;
	}
}
