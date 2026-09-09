using System;

public static class ModUIAutoPick
{
	public static void Paint(int uiX, int uiY, int uiW, int uiH, mGraphics g)
	{
		mFont.tahoma_7b_dark.drawString(g, "Tự nhặt đồ:", uiX + 8, uiY + 12, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 78, uiY + 8, 52, 18, ModAutoPick.autoPick ? "BẬT" : "TẮT", ModAutoPick.autoPick, g);

		int boxX = uiX + 6;
		int boxY = uiY + 32;
		int boxW = uiW - 12;
		int boxH = 152;
		GameCanvas.paintz.paintFrameSimple(boxX, boxY, boxW, boxH, g);
		g.setColor(15196114);
		g.fillRect(boxX + 2, boxY + 2, boxW - 4, boxH - 4);

		// 1. Nhặt tất cả
		ModUI.DrawCheckbox(boxX + 10, boxY + 6, ModAutoPick.pickAll, g);
		(ModAutoPick.pickAll ? mFont.tahoma_7b_green2 : mFont.tahoma_7b_dark).drawString(g, "Nhặt tất cả vật phẩm trên map", boxX + 32, boxY + 9, mFont.LEFT);

		// 2. Lọc theo ID
		ModUI.DrawCheckbox(boxX + 10, boxY + 30, ModAutoPick.filterById, g);
		string idText = string.IsNullOrEmpty(ModAutoPick.filterIdsRaw) ? "(Chưa đặt ID)" : ("[" + ModAutoPick.filterIdsRaw + "]");
		if (idText.Length > 28) idText = idText.Substring(0, 25) + "...]";
		(ModAutoPick.filterById ? mFont.tahoma_7b_green2 : mFont.tahoma_7b_dark).drawString(g, "Lọc ID: " + idText, boxX + 32, boxY + 33, mFont.LEFT);

		// 3. Lọc theo Tên
		ModUI.DrawCheckbox(boxX + 10, boxY + 54, ModAutoPick.filterByName, g);
		string nameText = string.IsNullOrEmpty(ModAutoPick.filterNameRaw) ? "(Chưa đặt từ khóa)" : ("[" + ModAutoPick.filterNameRaw + "]");
		if (nameText.Length > 28) nameText = nameText.Substring(0, 25) + "...]";
		(ModAutoPick.filterByName ? mFont.tahoma_7b_green2 : mFont.tahoma_7b_dark).drawString(g, "Lọc Tên: " + nameText, boxX + 32, boxY + 57, mFont.LEFT);

		// 4. Vàng / Thỏi vàng
		ModUI.DrawCheckbox(boxX + 10, boxY + 78, ModAutoPick.pickGold, g);
		(ModAutoPick.pickGold ? mFont.tahoma_7b_green2 : mFont.tahoma_7b_dark).drawString(g, "Ưu tiên nhặt Vàng / Thỏi Vàng", boxX + 32, boxY + 81, mFont.LEFT);

		// 5. Trang bị / Đồ sao
		ModUI.DrawCheckbox(boxX + 10, boxY + 102, ModAutoPick.pickEquip, g);
		(ModAutoPick.pickEquip ? mFont.tahoma_7b_green2 : mFont.tahoma_7b_dark).drawString(g, "Ưu tiên nhặt Trang Bị / Đồ sao", boxX + 32, boxY + 105, mFont.LEFT);

		// 6. Ngọc rồng & Sự kiện
		ModUI.DrawCheckbox(boxX + 10, boxY + 126, ModAutoPick.pickGem, g);
		(ModAutoPick.pickGem ? mFont.tahoma_7b_green2 : mFont.tahoma_7b_dark).drawString(g, "Ưu tiên nhặt Ngọc Rồng & Sự Kiện", boxX + 32, boxY + 129, mFont.LEFT);

		// 3 Nút Bấm Thao Tác Nhanh (Native Buttons)
		int actionBtnY = uiY + 194;
		int actionBtnW = (uiW - 24) / 3;
		ModUI.PaintNativeButton(uiX + 6, actionBtnY, actionBtnW, 20, "Nhập ID", false, g);
		ModUI.PaintNativeButton(uiX + 12 + actionBtnW, actionBtnY, actionBtnW, 20, "Nhập Tên", false, g);
		ModUI.PaintNativeButton(uiX + 18 + actionBtnW * 2, actionBtnY, actionBtnW, 20, "Xóa Lọc", false, g);
	}

	public static bool HandleTap(int px, int py, int uiX, int uiY, int uiW, int uiH)
	{
		// Nút BẬT/TẮT Tự nhặt
		if (px >= uiX + 78 && px <= uiX + 130 && py >= uiY + 6 && py <= uiY + 28)
		{
			ModAutoPick.autoPick = !ModAutoPick.autoPick;
			ModConfig.SaveConfig();
			SoundMn.gI().buttonClick();
			return true;
		}

		int boxX = uiX + 6;
		int boxY = uiY + 32;

		if (px >= boxX + 6 && px <= boxX + uiW - 16)
		{
			// 1. Nhặt tất cả
			if (py >= boxY + 4 && py <= boxY + 26)
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
			if (py >= boxY + 28 && py <= boxY + 50)
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
			if (py >= boxY + 52 && py <= boxY + 74)
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
			if (py >= boxY + 76 && py <= boxY + 98)
			{
				ModAutoPick.pickGold = !ModAutoPick.pickGold;
				ModConfig.SaveConfig();
				SoundMn.gI().buttonClick();
				return true;
			}
			// 5. Trang bị
			if (py >= boxY + 100 && py <= boxY + 122)
			{
				ModAutoPick.pickEquip = !ModAutoPick.pickEquip;
				ModConfig.SaveConfig();
				SoundMn.gI().buttonClick();
				return true;
			}
			// 6. Ngọc rồng
			if (py >= boxY + 124 && py <= boxY + 146)
			{
				ModAutoPick.pickGem = !ModAutoPick.pickGem;
				ModConfig.SaveConfig();
				SoundMn.gI().buttonClick();
				return true;
			}
		}

		int actionBtnY = uiY + 194;
		int actionBtnW = (uiW - 24) / 3;

		// Nút "Nhập ID"
		if (px >= uiX + 6 && px <= uiX + 6 + actionBtnW && py >= actionBtnY && py <= actionBtnY + 22)
		{
			ModAutoPick.ShowInputFilterId();
			return true;
		}

		// Nút "Nhập Tên"
		if (px >= uiX + 12 + actionBtnW && px <= uiX + 12 + actionBtnW * 2 && py >= actionBtnY && py <= actionBtnY + 22)
		{
			ModAutoPick.ShowInputFilterName();
			return true;
		}

		// Nút "Xóa Lọc"
		if (px >= uiX + 18 + actionBtnW * 2 && px <= uiX + 18 + actionBtnW * 3 && py >= actionBtnY && py <= actionBtnY + 22)
		{
			ModAutoPick.ClearFilters();
			ModConfig.SaveConfig();
			SoundMn.gI().buttonClick();
			return true;
		}

		return false;
	}
}
