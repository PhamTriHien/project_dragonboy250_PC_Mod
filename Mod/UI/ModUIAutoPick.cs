using System;

public static class ModUIAutoPick
{
	public static void Paint(int uiX, int uiY, int uiW, int uiH, mGraphics g)
	{
		mFont.tahoma_7b_white.drawString(g, "Tự nhặt đồ:", uiX + 20, uiY + 58, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 100, uiY + 52, 85, ModAutoPick.autoPick ? "BẬT" : "TẮT", ModAutoPick.autoPick, g);

		int boxX = uiX + 16;
		int boxY = uiY + 80;
		int boxW = uiW - 32;
		int boxH = 105;
		GameCanvas.paintz.paintFrameSimple(boxX, boxY, boxW, boxH, g);
		g.setColor(0x181818);
		g.fillRect(boxX + 2, boxY + 2, boxW - 4, boxH - 4);

		ModUI.DrawCheckbox(boxX + 12, boxY + 10, ModAutoPick.pickAll, g);
		(ModAutoPick.pickAll ? mFont.tahoma_7b_green2 : mFont.tahoma_7_white).drawString(g, "Nhặt tất cả vật phẩm trên map", boxX + 32, boxY + 11, mFont.LEFT);

		ModUI.DrawCheckbox(boxX + 12, boxY + 33, ModAutoPick.pickGold, g);
		(ModAutoPick.pickGold ? mFont.tahoma_7b_green2 : mFont.tahoma_7_white).drawString(g, "Ưu tiên nhặt Vàng / Thỏi Vàng", boxX + 32, boxY + 34, mFont.LEFT);

		ModUI.DrawCheckbox(boxX + 12, boxY + 56, ModAutoPick.pickEquip, g);
		(ModAutoPick.pickEquip ? mFont.tahoma_7b_green2 : mFont.tahoma_7_white).drawString(g, "Ưu tiên nhặt Trang Bị / Đồ sao", boxX + 32, boxY + 57, mFont.LEFT);

		ModUI.DrawCheckbox(boxX + 12, boxY + 79, ModAutoPick.pickGem, g);
		(ModAutoPick.pickGem ? mFont.tahoma_7b_green2 : mFont.tahoma_7_white).drawString(g, "Ưu tiên nhặt Ngọc Rồng & Sự Kiện", boxX + 32, boxY + 80, mFont.LEFT);

		mFont.tahoma_7_grey.drawString(g, "* Tự động nhặt đồ rơi trong phạm vi toàn map cực nhanh.", uiX + uiW / 2, uiY + 198, mFont.CENTER);
	}

	public static bool HandleTap(int px, int py, int uiX, int uiY, int uiW, int uiH)
	{
		if (px >= uiX + 100 && px <= uiX + 185 && py >= uiY + 52 && py <= uiY + 74)
		{
			ModAutoPick.autoPick = !ModAutoPick.autoPick;
			ModConfig.SaveConfig();
			SoundMn.gI().buttonClick();
			return true;
		}

		int boxX = uiX + 16;
		int boxY = uiY + 80;

		if (px >= boxX + 10 && px <= boxX + 280)
		{
			if (py >= boxY + 8 && py <= boxY + 28)
			{
				ModAutoPick.pickAll = !ModAutoPick.pickAll;
				ModConfig.SaveConfig();
				SoundMn.gI().buttonClick();
				return true;
			}
			if (py >= boxY + 31 && py <= boxY + 51)
			{
				ModAutoPick.pickGold = !ModAutoPick.pickGold;
				ModConfig.SaveConfig();
				SoundMn.gI().buttonClick();
				return true;
			}
			if (py >= boxY + 54 && py <= boxY + 74)
			{
				ModAutoPick.pickEquip = !ModAutoPick.pickEquip;
				ModConfig.SaveConfig();
				SoundMn.gI().buttonClick();
				return true;
			}
			if (py >= boxY + 77 && py <= boxY + 97)
			{
				ModAutoPick.pickGem = !ModAutoPick.pickGem;
				ModConfig.SaveConfig();
				SoundMn.gI().buttonClick();
				return true;
			}
		}
		return false;
	}
}
