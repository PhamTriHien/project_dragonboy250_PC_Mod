using System;

/// <summary>
/// CLASS ModUIAutoPick: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public static class ModUIAutoPick
{
	/// <summary>
	/// Thực hiện vẽ toàn bộ đối tượng và các lớp hiển thị lên canvas mGraphics.
	/// </summary>
	public static void Paint(int uiX, int uiY, int uiW, int uiH, mGraphics g)
	{
		mFont.tahoma_7_grey.drawString(g, "Tự nhặt đồ:", uiX + 5, uiY + 7, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 66, uiY + 4, 42, 17, ModAutoPick.autoPick ? "BẬT" : "TẮT", ModAutoPick.autoPick, g);

		int distBtnW = 72;
		int distBtnX = uiX + uiW - distBtnW - 4;
		mFont.tahoma_7_grey.drawString(g, "Phạm vi:", distBtnX - 44, uiY + 7, mFont.LEFT);
		ModUI.PaintNativeButton(distBtnX, uiY + 4, distBtnW, 17, ModAutoPick.GetPickDistanceLabel(), ModAutoPick.maxPickDistance > 0, g);

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

		int stepY = (boxH >= 130) ? 18 : System.Math.Max(16, (boxH - 6) / 7);

		// 1. Nhặt tất cả
		ModUI.DrawCheckbox(boxX + 6, boxY + 3, ModAutoPick.pickAll, g);
		(ModAutoPick.pickAll ? mFont.tahoma_7_green2 : mFont.tahoma_7_grey).drawString(g, "Nhặt tất cả vật phẩm trên map", boxX + 26, boxY + 5, mFont.LEFT);

		// 2. Lọc theo ID
		ModUI.DrawCheckbox(boxX + 6, boxY + 3 + stepY, ModAutoPick.filterById, g);
		string idText = string.IsNullOrEmpty(ModAutoPick.filterIdsRaw) ? "(Chưa đặt ID)" : ("[" + ModAutoPick.filterIdsRaw + "]");
		if (idText.Length > 28) idText = idText.Substring(0, 25) + "...]";
		(ModAutoPick.filterById ? mFont.tahoma_7_green2 : mFont.tahoma_7_grey).drawString(g, "Lọc ID: " + idText, boxX + 26, boxY + 5 + stepY, mFont.LEFT);

		// 3. Lọc theo Tên
		ModUI.DrawCheckbox(boxX + 6, boxY + 3 + stepY * 2, ModAutoPick.filterByName, g);
		string nameText = string.IsNullOrEmpty(ModAutoPick.filterNameRaw) ? "(Chưa đặt từ khóa)" : ("[" + ModAutoPick.filterNameRaw + "]");
		if (nameText.Length > 28) nameText = nameText.Substring(0, 25) + "...]";
		(ModAutoPick.filterByName ? mFont.tahoma_7_green2 : mFont.tahoma_7_grey).drawString(g, "Lọc Tên: " + nameText, boxX + 26, boxY + 5 + stepY * 2, mFont.LEFT);

		// 4. Vàng / Thỏi vàng
		ModUI.DrawCheckbox(boxX + 6, boxY + 3 + stepY * 3, ModAutoPick.pickGold, g);
		(ModAutoPick.pickGold ? mFont.tahoma_7_green2 : mFont.tahoma_7_grey).drawString(g, "Ưu tiên nhặt Vàng / Thỏi Vàng", boxX + 26, boxY + 5 + stepY * 3, mFont.LEFT);

		// 5. Trang bị / Đồ sao
		ModUI.DrawCheckbox(boxX + 6, boxY + 3 + stepY * 4, ModAutoPick.pickEquip, g);
		(ModAutoPick.pickEquip ? mFont.tahoma_7_green2 : mFont.tahoma_7_grey).drawString(g, "Ưu tiên nhặt Trang Bị / Đồ sao", boxX + 26, boxY + 5 + stepY * 4, mFont.LEFT);

		// 6. Ngọc rồng & Sự kiện
		ModUI.DrawCheckbox(boxX + 6, boxY + 3 + stepY * 5, ModAutoPick.pickGem, g);
		(ModAutoPick.pickGem ? mFont.tahoma_7_green2 : mFont.tahoma_7_grey).drawString(g, "Ưu tiên nhặt Ngọc Rồng & Sự Kiện", boxX + 26, boxY + 5 + stepY * 5, mFont.LEFT);

		// 7. Tự vứt đồ theo ID
		ModUI.DrawCheckbox(boxX + 6, boxY + 3 + stepY * 6, ModAutoPick.autoDropItem, g);
		string dropText = string.IsNullOrEmpty(ModAutoPick.dropIdsRaw) ? "(Chưa đặt ID vứt)" : ("[" + ModAutoPick.dropIdsRaw + "]");
		if (dropText.Length > 26) dropText = dropText.Substring(0, 23) + "...]";
		(ModAutoPick.autoDropItem ? mFont.tahoma_7_green2 : mFont.tahoma_7_grey).drawString(g, "Tự vứt ID: " + dropText, boxX + 26, boxY + 5 + stepY * 6, mFont.LEFT);

		// 4 Nút Bấm Thao Tác Nhanh ở Footer
		int actionBtnW = (uiW - 20) / 4;
		ModUI.PaintNativeButton(uiX + 4, footerY, actionBtnW, footerH, "ID Nhặt", false, g);
		ModUI.PaintNativeButton(uiX + 8 + actionBtnW, footerY, actionBtnW, footerH, "Tên Nhặt", false, g);
		ModUI.PaintNativeButton(uiX + 12 + actionBtnW * 2, footerY, actionBtnW, footerH, "ID Vứt", false, g);
		ModUI.PaintNativeButton(uiX + 16 + actionBtnW * 3, footerY, actionBtnW, footerH, "Xóa Lọc", false, g);
	}

	/// <summary>
	/// Xử lý nghiệp vụ logic chuyên trách cho HandleTap.
	/// </summary>
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

		// Nút Phạm vi nhặt
		int distBtnW = 72;
		int distBtnX = uiX + uiW - distBtnW - 4;
		if (px >= distBtnX && px <= distBtnX + distBtnW && py >= uiY + 3 && py <= uiY + 22)
		{
			ModAutoPick.CycleMaxPickDistance();
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
		int stepY = (boxH >= 130) ? 18 : System.Math.Max(16, (boxH - 6) / 7);

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
			if (py >= boxY + stepY * 5 && py <= boxY + stepY * 6)
			{
				ModAutoPick.pickGem = !ModAutoPick.pickGem;
				ModConfig.SaveConfig();
				SoundMn.gI().buttonClick();
				return true;
			}
			// 7. Tự vứt đồ theo ID
			if (py >= boxY + stepY * 6 && py <= boxY + boxH)
			{
				ModAutoPick.autoDropItem = !ModAutoPick.autoDropItem;
				if (ModAutoPick.autoDropItem && ModAutoPick.dropIds.Count == 0)
				{
					ModAutoPick.ShowInputDropId();
					return true;
				}
				ModConfig.SaveConfig();
				SoundMn.gI().buttonClick();
				return true;
			}
		}

		// 4 Nút Bấm Thao Tác Nhanh ở Footer
		int actionBtnW = (uiW - 20) / 4;
		if (py >= footerY - 1 && py <= footerY + footerH + 1)
		{
			if (px >= uiX + 4 && px <= uiX + 4 + actionBtnW)
			{
				ModAutoPick.ShowInputFilterId();
				return true;
			}
			if (px >= uiX + 8 + actionBtnW && px <= uiX + 8 + actionBtnW * 2)
			{
				ModAutoPick.ShowInputFilterName();
				return true;
			}
			if (px >= uiX + 12 + actionBtnW * 2 && px <= uiX + 12 + actionBtnW * 3)
			{
				ModAutoPick.ShowInputDropId();
				return true;
			}
			if (px >= uiX + 16 + actionBtnW * 3 && px <= uiX + 16 + actionBtnW * 4)
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
