using System;

public static class ModUIAutoPick
{
	public static void Paint(int uiX, int uiY, int uiW, int uiH, mGraphics g)
	{
		mFont.tahoma_7_grey.drawString(g, "Tự nhặt đồ:", uiX + 6, uiY + 7, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 68, uiY + 4, 44, 16, ModAutoPick.autoPick ? "BẬT" : "TẮT", ModAutoPick.autoPick, g);

		int boxX = uiX + 5;
		int boxY = uiY + 23;
		int boxW = uiW - 10;
		int boxH = 162;
		GameCanvas.paintz.paintFrameSimple(boxX, boxY, boxW, boxH, g);
		g.setColor(15196114);
		g.fillRect(boxX + 2, boxY + 2, boxW - 4, boxH - 4);

		// 1. Nhặt tất cả
		ModUI.DrawCheckbox(boxX + 8, boxY + 6, ModAutoPick.pickAll, g);
		(ModAutoPick.pickAll ? mFont.tahoma_7_green2 : mFont.tahoma_7_grey).drawString(g, "Nhặt tất cả vật phẩm trên map", boxX + 30, boxY + 8, mFont.LEFT);

		// 2. Lọc theo ID
		ModUI.DrawCheckbox(boxX + 8, boxY + 32, ModAutoPick.filterById, g);
		string idText = string.IsNullOrEmpty(ModAutoPick.filterIdsRaw) ? "(Chưa đặt ID)" : ("[" + ModAutoPick.filterIdsRaw + "]");
		if (idText.Length > 30) idText = idText.Substring(0, 27) + "...]";
		(ModAutoPick.filterById ? mFont.tahoma_7_green2 : mFont.tahoma_7_grey).drawString(g, "Lọc ID: " + idText, boxX + 30, boxY + 34, mFont.LEFT);

		// 3. Lọc theo Tên
		ModUI.DrawCheckbox(boxX + 8, boxY + 58, ModAutoPick.filterByName, g);
		string nameText = string.IsNullOrEmpty(ModAutoPick.filterNameRaw) ? "(Chưa đặt từ khóa)" : ("[" + ModAutoPick.filterNameRaw + "]");
		if (nameText.Length > 30) nameText = nameText.Substring(0, 27) + "...]";
		(ModAutoPick.filterByName ? mFont.tahoma_7_green2 : mFont.tahoma_7_grey).drawString(g, "Lọc Tên: " + nameText, boxX + 30, boxY + 60, mFont.LEFT);

		// 4. Vàng / Thỏi vàng
		ModUI.DrawCheckbox(boxX + 8, boxY + 84, ModAutoPick.pickGold, g);
		(ModAutoPick.pickGold ? mFont.tahoma_7_green2 : mFont.tahoma_7_grey).drawString(g, "Ưu tiên nhặt Vàng / Thỏi Vàng", boxX + 30, boxY + 86, mFont.LEFT);

		// 5. Trang bị / Đồ sao
		ModUI.DrawCheckbox(boxX + 8, boxY + 110, ModAutoPick.pickEquip, g);
		(ModAutoPick.pickEquip ? mFont.tahoma_7_green2 : mFont.tahoma_7_grey).drawString(g, "Ưu tiên nhặt Trang Bị / Đồ sao", boxX + 30, boxY + 112, mFont.LEFT);

		// 6. Ngọc rồng & Sự kiện
		ModUI.DrawCheckbox(boxX + 8, boxY + 136, ModAutoPick.pickGem, g);
		(ModAutoPick.pickGem ? mFont.tahoma_7_green2 : mFont.tahoma_7_grey).drawString(g, "Ưu tiên nhặt Ngọc Rồng & Sự Kiện", boxX + 30, boxY + 138, mFont.LEFT);

		// 3 Nút Bấm Thao Tác Nhanh
		int actionBtnY = uiY + 190;
		int actionBtnW = (uiW - 20) / 3;
		ModUI.PaintNativeButton(uiX + 5, actionBtnY, actionBtnW, 18, "Nhập ID", false, g);
		ModUI.PaintNativeButton(uiX + 10 + actionBtnW, actionBtnY, actionBtnW, 18, "Nhập Tên", false, g);
		ModUI.PaintNativeButton(uiX + 15 + actionBtnW * 2, actionBtnY, actionBtnW, 18, "Xóa Lọc", false, g);
	}

	public static bool HandleTap(int px, int py, int uiX, int uiY, int uiW, int uiH)
	{
		// Nút BẬT/TẮT Tự nhặt
		if (px >= uiX + 68 && px <= uiX + 112 && py >= uiY + 3 && py <= uiY + 21)
		{
			ModAutoPick.autoPick = !ModAutoPick.autoPick;
			ModConfig.SaveConfig();
			SoundMn.gI().buttonClick();
			return true;
		}

		int boxX = uiX + 5;
		int boxY = uiY + 23;

		if (px >= boxX + 4 && px <= boxX + uiW - 14)
		{
			// 1. Nhặt tất cả
			if (py >= boxY + 4 && py <= boxY + 28)
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
			if (py >= boxY + 30 && py <= boxY + 54)
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
			if (py >= boxY + 56 && py <= boxY + 80)
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
			if (py >= boxY + 82 && py <= boxY + 106)
			{
				ModAutoPick.pickGold = !ModAutoPick.pickGold;
				ModConfig.SaveConfig();
				SoundMn.gI().buttonClick();
				return true;
			}
			// 5. Trang bị
			if (py >= boxY + 108 && py <= boxY + 132)
			{
				ModAutoPick.pickEquip = !ModAutoPick.pickEquip;
				ModConfig.SaveConfig();
				SoundMn.gI().buttonClick();
				return true;
			}
			// 6. Ngọc rồng
			if (py >= boxY + 134 && py <= boxY + 158)
			{
				ModAutoPick.pickGem = !ModAutoPick.pickGem;
				ModConfig.SaveConfig();
				SoundMn.gI().buttonClick();
				return true;
			}
		}

		int actionBtnY = uiY + 190;
		int actionBtnW = (uiW - 20) / 3;

		// Nút "Nhập ID"
		if (px >= uiX + 5 && px <= uiX + 5 + actionBtnW && py >= actionBtnY && py <= actionBtnY + 20)
		{
			ModAutoPick.ShowInputFilterId();
			return true;
		}

		// Nút "Nhập Tên"
		if (px >= uiX + 10 + actionBtnW && px <= uiX + 10 + actionBtnW * 2 && py >= actionBtnY && py <= actionBtnY + 20)
		{
			ModAutoPick.ShowInputFilterName();
			return true;
		}

		// Nút "Xóa Lọc"
		if (px >= uiX + 15 + actionBtnW * 2 && px <= uiX + 15 + actionBtnW * 3 && py >= actionBtnY && py <= actionBtnY + 20)
		{
			ModAutoPick.ClearFilters();
			ModConfig.SaveConfig();
			SoundMn.gI().buttonClick();
			return true;
		}

		return false;
	}
}
