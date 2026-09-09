using System;

public static class ModUIAutoPick
{
	public static void Paint(int uiX, int uiY, int uiW, int uiH, mGraphics g)
	{
		mFont.tahoma_7b_white.drawString(g, "Tự nhặt đồ:", uiX + 20, uiY + 54, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 90, uiY + 50, 52, 18, ModAutoPick.autoPick ? "BẬT" : "TẮT", ModAutoPick.autoPick, g);

		int boxX = uiX + 16;
		int boxY = uiY + 72;
		int boxH = 120;
		int boxW = uiW - 32;
		GameCanvas.paintz.paintFrameSimple(boxX, boxY, boxW, boxH, g);
		g.setColor(0x181818);
		g.fillRect(boxX + 2, boxY + 2, boxW - 4, boxH - 4);

		// 1. Nhặt tất cả
		ModUI.DrawCheckbox(boxX + 10, boxY + 5, ModAutoPick.pickAll, g);
		(ModAutoPick.pickAll ? mFont.tahoma_7b_green2 : mFont.tahoma_7_white).drawString(g, "Nhặt tất cả vật phẩm trên map", boxX + 28, boxY + 6, mFont.LEFT);

		// 2. Lọc theo ID
		ModUI.DrawCheckbox(boxX + 10, boxY + 24, ModAutoPick.filterById, g);
		string idText = string.IsNullOrEmpty(ModAutoPick.filterIdsRaw) ? "(Chưa đặt ID)" : ("[" + ModAutoPick.filterIdsRaw + "]");
		if (idText.Length > 28) idText = idText.Substring(0, 25) + "...]";
		(ModAutoPick.filterById ? mFont.tahoma_7b_green2 : mFont.tahoma_7_white).drawString(g, "Lọc ID: " + idText, boxX + 28, boxY + 25, mFont.LEFT);

		// 3. Lọc theo Tên
		ModUI.DrawCheckbox(boxX + 10, boxY + 43, ModAutoPick.filterByName, g);
		string nameText = string.IsNullOrEmpty(ModAutoPick.filterNameRaw) ? "(Chưa đặt từ khóa)" : ("[" + ModAutoPick.filterNameRaw + "]");
		if (nameText.Length > 28) nameText = nameText.Substring(0, 25) + "...]";
		(ModAutoPick.filterByName ? mFont.tahoma_7b_green2 : mFont.tahoma_7_white).drawString(g, "Lọc Tên: " + nameText, boxX + 28, boxY + 44, mFont.LEFT);

		// 4. Vàng / Thỏi vàng
		ModUI.DrawCheckbox(boxX + 10, boxY + 62, ModAutoPick.pickGold, g);
		(ModAutoPick.pickGold ? mFont.tahoma_7b_green2 : mFont.tahoma_7_white).drawString(g, "Ưu tiên nhặt Vàng / Thỏi Vàng", boxX + 28, boxY + 63, mFont.LEFT);

		// 5. Trang bị / Đồ sao
		ModUI.DrawCheckbox(boxX + 10, boxY + 81, ModAutoPick.pickEquip, g);
		(ModAutoPick.pickEquip ? mFont.tahoma_7b_green2 : mFont.tahoma_7_white).drawString(g, "Ưu tiên nhặt Trang Bị / Đồ sao", boxX + 28, boxY + 82, mFont.LEFT);

		// 6. Ngọc rồng & Sự kiện
		ModUI.DrawCheckbox(boxX + 10, boxY + 100, ModAutoPick.pickGem, g);
		(ModAutoPick.pickGem ? mFont.tahoma_7b_green2 : mFont.tahoma_7_white).drawString(g, "Ưu tiên nhặt Ngọc Rồng & Sự Kiện", boxX + 28, boxY + 101, mFont.LEFT);

		// 3 Nút Bấm Thao Tác Nhanh (Native Buttons) - Đặt ở hàng riêng biệt, không đè nút ĐÓNG
		int actionBtnY = uiY + 198;
		ModUI.PaintNativeButton(uiX + 20, actionBtnY, 90, 19, "Nhập ID", false, g);
		ModUI.PaintNativeButton(uiX + 125, actionBtnY, 90, 19, "Nhập Tên", false, g);
		ModUI.PaintNativeButton(uiX + 230, actionBtnY, 90, 19, "Xóa Lọc", false, g);
	}

	public static bool HandleTap(int px, int py, int uiX, int uiY, int uiW, int uiH)
	{
		// Nút BẬT/TẮT Tự nhặt
		if (px >= uiX + 90 && px <= uiX + 142 && py >= uiY + 48 && py <= uiY + 70)
		{
			ModAutoPick.autoPick = !ModAutoPick.autoPick;
			ModConfig.SaveConfig();
			SoundMn.gI().buttonClick();
			return true;
		}

		int boxX = uiX + 16;
		int boxY = uiY + 72;

		if (px >= boxX + 6 && px <= boxX + 295)
		{
			// 1. Nhặt tất cả
			if (py >= boxY + 2 && py <= boxY + 21)
			{
				ModAutoPick.pickAll = !ModAutoPick.pickAll;
				if (ModAutoPick.pickAll)
				{
					ModAutoPick.filterById = false;
					ModAutoPick.filterByName = false;
				}
				ModConfig.SaveConfig();
				SoundMn.gI().buttonClick();
				return true;
			}
			// 2. Lọc ID
			if (py >= boxY + 22 && py <= boxY + 40)
			{
				ModAutoPick.filterById = !ModAutoPick.filterById;
				if (ModAutoPick.filterById)
				{
					ModAutoPick.pickAll = false;
					if (ModAutoPick.filterIds.Count == 0)
					{
						ModAutoPick.ShowInputFilterId();
						return true;
					}
				}
				ModConfig.SaveConfig();
				SoundMn.gI().buttonClick();
				return true;
			}
			// 3. Lọc Tên
			if (py >= boxY + 41 && py <= boxY + 59)
			{
				ModAutoPick.filterByName = !ModAutoPick.filterByName;
				if (ModAutoPick.filterByName)
				{
					ModAutoPick.pickAll = false;
					if (ModAutoPick.filterNameKeywords.Count == 0)
					{
						ModAutoPick.ShowInputFilterName();
						return true;
					}
				}
				ModConfig.SaveConfig();
				SoundMn.gI().buttonClick();
				return true;
			}
			// 4. Vàng
			if (py >= boxY + 60 && py <= boxY + 78)
			{
				ModAutoPick.pickGold = !ModAutoPick.pickGold;
				ModConfig.SaveConfig();
				SoundMn.gI().buttonClick();
				return true;
			}
			// 5. Trang bị
			if (py >= boxY + 79 && py <= boxY + 97)
			{
				ModAutoPick.pickEquip = !ModAutoPick.pickEquip;
				ModConfig.SaveConfig();
				SoundMn.gI().buttonClick();
				return true;
			}
			// 6. Ngọc rồng
			if (py >= boxY + 98 && py <= boxY + 118)
			{
				ModAutoPick.pickGem = !ModAutoPick.pickGem;
				ModConfig.SaveConfig();
				SoundMn.gI().buttonClick();
				return true;
			}
		}

		// Nút "Nhập ID"
		if (px >= uiX + 20 && px <= uiX + 110 && py >= uiY + 196 && py <= uiY + 220)
		{
			ModAutoPick.ShowInputFilterId();
			return true;
		}

		// Nút "Nhập Tên"
		if (px >= uiX + 125 && px <= uiX + 215 && py >= uiY + 196 && py <= uiY + 220)
		{
			ModAutoPick.ShowInputFilterName();
			return true;
		}

		// Nút "Xóa Lọc"
		if (px >= uiX + 230 && px <= uiX + 320 && py >= uiY + 196 && py <= uiY + 220)
		{
			ModAutoPick.ClearFilters();
			ModConfig.SaveConfig();
			SoundMn.gI().buttonClick();
			return true;
		}

		return false;
	}
}
