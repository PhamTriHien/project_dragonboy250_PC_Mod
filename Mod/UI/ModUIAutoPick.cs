using System;

public static class ModUIAutoPick
{
	public static void Paint(int uiX, int uiY, int uiW, int uiH, mGraphics g)
	{
		mFont.tahoma_7_grey.drawString(g, "Tự nhặt đồ:", uiX + 5, uiY + 7, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 66, uiY + 4, 42, 17, ModAutoPick.autoPick ? "BẬT" : "TẮT", ModAutoPick.autoPick, g);

		int footerH = 18;
		int footerY = uiY + uiH - footerH - 4;

		int boxX = uiX + 4;
		int boxY = uiY + 24;
		int boxW = uiW - 8;
		int boxH = (footerY - 4) - boxY;
		if (boxH < 40) boxH = 40;

		GameCanvas.paintz.paintFrameSimple(boxX, boxY, boxW, boxH, g);
		g.setColor(15196114);
		g.fillRect(boxX + 2, boxY + 2, boxW - 4, boxH - 4);

		int stepY = (boxH >= 118) ? 19 : System.Math.Max(18, (boxH - 8) / 6);

		// 1. Nhặt tất cả
		ModUI.DrawCheckbox(boxX + 6, boxY + 4, ModAutoPick.pickAll, g);
		(ModAutoPick.pickAll ? mFont.tahoma_7_green2 : mFont.tahoma_7_grey).drawString(g, "Nhặt tất cả vật phẩm trên map", boxX + 26, boxY + 6, mFont.LEFT);

		// 2. Lọc theo ID
		ModUI.DrawCheckbox(boxX + 6, boxY + 4 + stepY, ModAutoPick.filterById, g);
		string idText = string.IsNullOrEmpty(ModAutoPick.filterIdsRaw) ? "(Chưa đặt ID)" : ("[" + ModAutoPick.filterIdsRaw + "]");
		if (idText.Length > 28) idText = idText.Substring(0, 25) + "...]";
		(ModAutoPick.filterById ? mFont.tahoma_7_green2 : mFont.tahoma_7_grey).drawString(g, "Lọc ID: " + idText, boxX + 26, boxY + 6 + stepY, mFont.LEFT);

		// 3. Lọc theo Tên
		ModUI.DrawCheckbox(boxX + 6, boxY + 4 + stepY * 2, ModAutoPick.filterByName, g);
		string nameText = string.IsNullOrEmpty(ModAutoPick.filterNameRaw) ? "(Chưa đặt từ khóa)" : ("[" + ModAutoPick.filterNameRaw + "]");
		if (nameText.Length > 28) nameText = nameText.Substring(0, 25) + "...]";
		(ModAutoPick.filterByName ? mFont.tahoma_7_green2 : mFont.tahoma_7_grey).drawString(g, "Lọc Tên: " + nameText, boxX + 26, boxY + 6 + stepY * 2, mFont.LEFT);

		// 4. Vàng / Thỏi vàng
		ModUI.DrawCheckbox(boxX + 6, boxY + 4 + stepY * 3, ModAutoPick.pickGold, g);
		(ModAutoPick.pickGold ? mFont.tahoma_7_green2 : mFont.tahoma_7_grey).drawString(g, "Ưu tiên nhặt Vàng / Thỏi Vàng", boxX + 26, boxY + 6 + stepY * 3, mFont.LEFT);

		// 5. Trang bị / Đồ sao
		ModUI.DrawCheckbox(boxX + 6, boxY + 4 + stepY * 4, ModAutoPick.pickEquip, g);
		(ModAutoPick.pickEquip ? mFont.tahoma_7_green2 : mFont.tahoma_7_grey).drawString(g, "Ưu tiên nhặt Trang Bị / Đồ sao", boxX + 26, boxY + 6 + stepY * 4, mFont.LEFT);

		// 6. Ngọc rồng & Sự kiện
		ModUI.DrawCheckbox(boxX + 6, boxY + 4 + stepY * 5, ModAutoPick.pickGem, g);
		(ModAutoPick.pickGem ? mFont.tahoma_7_green2 : mFont.tahoma_7_grey).drawString(g, "Ưu tiên nhặt Ngọc Rồng & Sự Kiện", boxX + 26, boxY + 6 + stepY * 5, mFont.LEFT);

		// 3 Nút Bấm Thao Tác Nhanh ở Footer
		int actionBtnW = (uiW - 16) / 3;
		ModUI.PaintNativeButton(uiX + 4, footerY, actionBtnW, footerH, "Nhập ID", false, g);
		ModUI.PaintNativeButton(uiX + 7 + actionBtnW, footerY, actionBtnW, footerH, "Nhập Tên", false, g);
		ModUI.PaintNativeButton(uiX + 10 + actionBtnW * 2, footerY, actionBtnW, footerH, "Xóa Lọc", false, g);
	}

	public static bool HandleTap(int px, int py, int uiX, int uiY, int uiW, int uiH)
	{
		// Nút BẬT/TẮT Tự nhặt
		if (px >= uiX + 66 && px <= uiX + 108 && py >= uiY + 3 && py <= uiY + 22)
		{
			ModAutoPick.autoPick = !ModAutoPick.autoPick;
			ModConfig.SaveConfig();
			SoundMn.gI().buttonClick();
			return true;
		}

		int footerH = 18;
		int footerY = uiY + uiH - footerH - 4;

		int boxX = uiX + 4;
		int boxY = uiY + 24;
		int boxW = uiW - 8;
		int boxH = (footerY - 4) - boxY;
		int stepY = (boxH >= 118) ? 19 : System.Math.Max(18, (boxH - 8) / 6);

		if (px >= boxX + 4 && px <= boxX + boxW - 4)
		{
			// 1. Nhặt tất cả
			if (py >= boxY + 2 && py <= boxY + stepY)
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
			if (py >= boxY + stepY && py <= boxY + stepY * 2)
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
			if (py >= boxY + stepY * 2 && py <= boxY + stepY * 3)
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
			if (py >= boxY + stepY * 3 && py <= boxY + stepY * 4)
			{
				ModAutoPick.pickGold = !ModAutoPick.pickGold;
				ModConfig.SaveConfig();
				SoundMn.gI().buttonClick();
				return true;
			}
			// 5. Trang bị
			if (py >= boxY + stepY * 4 && py <= boxY + stepY * 5)
			{
				ModAutoPick.pickEquip = !ModAutoPick.pickEquip;
				ModConfig.SaveConfig();
				SoundMn.gI().buttonClick();
				return true;
			}
			// 6. Ngọc rồng
			if (py >= boxY + stepY * 5 && py <= boxY + boxH)
			{
				ModAutoPick.pickGem = !ModAutoPick.pickGem;
				ModConfig.SaveConfig();
				SoundMn.gI().buttonClick();
				return true;
			}
		}

		// 3 Nút Bấm Thao Tác Nhanh ở Footer
		int actionBtnW = (uiW - 16) / 3;
		if (py >= footerY - 1 && py <= footerY + footerH + 1)
		{
			// Nút "Nhập ID"
			if (px >= uiX + 4 && px <= uiX + 4 + actionBtnW)
			{
				ModAutoPick.ShowInputFilterId();
				return true;
			}

			// Nút "Nhập Tên"
			if (px >= uiX + 7 + actionBtnW && px <= uiX + 7 + actionBtnW * 2)
			{
				ModAutoPick.ShowInputFilterName();
				return true;
			}

			// Nút "Xóa Lọc"
			if (px >= uiX + 10 + actionBtnW * 2 && px <= uiX + 10 + actionBtnW * 3)
			{
				ModAutoPick.ClearFilters();
				ModConfig.SaveConfig();
				SoundMn.gI().buttonClick();
				return true;
			}
		}

		return false;
	}
}
