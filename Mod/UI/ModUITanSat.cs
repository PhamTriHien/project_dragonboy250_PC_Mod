using System;
using System.Collections.Generic;

public static class ModUITanSat
{
	public static void Paint(int uiX, int uiY, int uiW, int uiH, mGraphics g)
	{
		mFont.tahoma_7b_white.drawString(g, "Trạng thái:", uiX + 16, uiY + 55, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 80, uiY + 51, 52, 18, ModTanSat.autoTanSat ? "BẬT" : "TẮT", ModTanSat.autoTanSat, g);

		mFont.tahoma_7b_white.drawString(g, "Tiếp cận:", uiX + 150, uiY + 55, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 205, uiY + 51, 80, 18, ModTanSat.useTeleport ? "Dịch chuyển" : "Chạy bộ", ModTanSat.useTeleport, g);

		ModUI.PaintNativeButton(uiX + 16, uiY + 74, 145, 19, "1. Chọn Quái", ModUI.tanSatTab == 0, g);
		ModUI.PaintNativeButton(uiX + 178, uiY + 74, 145, 19, "2. Chọn Kỹ Năng", ModUI.tanSatTab == 1, g);

		if (ModUI.tanSatTab == 0)
		{
			mFont.tahoma_7b_yellow.drawString(g, "Quái trong map (Tick để đánh):", uiX + 16, uiY + 102, mFont.LEFT);

			g.setColor(0x333333);
			g.fillRect(uiX + uiW - 105, uiY + 98, 90, 18);
			g.setColor(0x888888);
			g.drawRect(uiX + uiW - 105, uiY + 98, 90, 18);
			mFont.tahoma_7_yellow.drawString(g, ModTanSat.selectAllMobs ? "Bỏ chọn hết" : "Chọn tất cả", uiX + uiW - 60, uiY + 100, mFont.CENTER);

			int listX = uiX + 14;
			int listY = uiY + 118;
			int listW = uiW - 28;
			int listH = 96;
			GameCanvas.paintz.paintFrameSimple(listX, listY, listW, listH, g);
			g.setColor(0x181818);
			g.fillRect(listX + 2, listY + 2, listW - 4, listH - 4);

			List<int> mobIds = ModUI.GetUniqueMobTemplateIds();
			if (mobIds.Count == 0)
			{
				mFont.tahoma_7_grey.drawString(g, "(Chưa thấy quái nào trong map)", listX + listW / 2, listY + 40, mFont.CENTER);
			}
			else
			{
				for (int idx = 0; idx <= mobIds.Count; idx++)
				{
					int col = idx % 2;
					int row = idx / 2;
					int itemX = (col == 0) ? (uiX + 22) : (uiX + 172);
					int itemY = uiY + 122 + row * 22;

					if (idx == 0)
					{
						ModUI.DrawCheckbox(itemX, itemY + 2, ModTanSat.selectAllMobs, g);
						mFont.tahoma_7b_yellow.drawString(g, "Tất cả quái", itemX + 20, itemY + 3, mFont.LEFT);
					}
					else
					{
						int mobTpl = mobIds[idx - 1];
						string mobName = (Mob.arrMobTemplate != null && mobTpl >= 0 && mobTpl < Mob.arrMobTemplate.Length && Mob.arrMobTemplate[mobTpl] != null) ? Mob.arrMobTemplate[mobTpl].name : ("Quái #" + mobTpl);
						bool isTicked = ModTanSat.IsMobTicked(mobTpl);
						ModUI.DrawCheckbox(itemX, itemY + 2, isTicked, g);
						if (isTicked)
						{
							mFont.tahoma_7b_green2.drawString(g, mobName, itemX + 20, itemY + 3, mFont.LEFT);
						}
						else
						{
							mFont.tahoma_7_white.drawString(g, mobName, itemX + 20, itemY + 3, mFont.LEFT);
						}
					}
				}
			}
		}
		else
		{
			mFont.tahoma_7b_yellow.drawString(g, "Kỹ năng tấn công (Tick để dùng):", uiX + 16, uiY + 102, mFont.LEFT);

			g.setColor(0x333333);
			g.fillRect(uiX + uiW - 105, uiY + 98, 90, 18);
			g.setColor(0x888888);
			g.drawRect(uiX + uiW - 105, uiY + 98, 90, 18);
			mFont.tahoma_7_yellow.drawString(g, ModTanSat.selectAllSkills ? "Bỏ chọn hết" : "Chọn tất cả", uiX + uiW - 60, uiY + 100, mFont.CENTER);

			int listX = uiX + 14;
			int listY = uiY + 118;
			int listW = uiW - 28;
			int listH = 96;
			GameCanvas.paintz.paintFrameSimple(listX, listY, listW, listH, g);
			g.setColor(0x181818);
			g.fillRect(listX + 2, listY + 2, listW - 4, listH - 4);

			List<Skill> skills = ModUI.GetPlayerAttackSkills();
			if (skills.Count == 0)
			{
				mFont.tahoma_7_grey.drawString(g, "(Nhân vật chưa có chiêu tấn công)", listX + listW / 2, listY + 40, mFont.CENTER);
			}
			else
			{
				for (int idx = 0; idx <= skills.Count; idx++)
				{
					int col = idx % 2;
					int row = idx / 2;
					int itemX = (col == 0) ? (uiX + 22) : (uiX + 172);
					int itemY = uiY + 122 + row * 22;

					if (idx == 0)
					{
						ModUI.DrawCheckbox(itemX, itemY + 2, ModTanSat.selectAllSkills, g);
						mFont.tahoma_7b_yellow.drawString(g, "Tất cả kỹ năng", itemX + 20, itemY + 3, mFont.LEFT);
					}
					else
					{
						Skill sk = skills[idx - 1];
						string skName = (sk != null && sk.template != null) ? sk.template.name : ("Skill #" + idx);
						bool isTicked = ModTanSat.IsSkillTicked(sk.template.id);
						ModUI.DrawCheckbox(itemX, itemY + 2, isTicked, g);
						if (isTicked)
						{
							mFont.tahoma_7b_green2.drawString(g, skName, itemX + 20, itemY + 3, mFont.LEFT);
						}
						else
						{
							mFont.tahoma_7_white.drawString(g, skName, itemX + 20, itemY + 3, mFont.LEFT);
						}
					}
				}
			}
		}
	}

	public static bool HandleTap(int px, int py, int uiX, int uiY, int uiW, int uiH)
	{
		// Bật/Tắt Tàn sát
		if (px >= uiX + 80 && px <= uiX + 132 && py >= uiY + 50 && py <= uiY + 70)
		{
			ModTanSat.autoTanSat = !ModTanSat.autoTanSat;
			ModConfig.SaveConfig();
			SoundMn.gI().buttonClick();
			return true;
		}

		// Bật/Tắt Dịch chuyển / Chạy bộ
		if (px >= uiX + 205 && px <= uiX + 285 && py >= uiY + 50 && py <= uiY + 70)
		{
			ModTanSat.useTeleport = !ModTanSat.useTeleport;
			ModConfig.SaveConfig();
			SoundMn.gI().buttonClick();
			return true;
		}

		// Sub tabs (1. Chọn Quái / 2. Chọn Kỹ Năng)
		if (py >= uiY + 73 && py <= uiY + 95)
		{
			if (px >= uiX + 16 && px <= uiX + 161)
			{
				ModUI.tanSatTab = 0;
				SoundMn.gI().buttonClick();
				return true;
			}
			if (px >= uiX + 178 && px <= uiX + 323)
			{
				ModUI.tanSatTab = 1;
				SoundMn.gI().buttonClick();
				return true;
			}
		}

		// Nút Chọn tất cả / Bỏ chọn hết
		if (px >= uiX + uiW - 110 && px <= uiX + uiW - 14 && py >= uiY + 96 && py <= uiY + 118)
		{
			if (ModUI.tanSatTab == 0)
			{
				ModTanSat.ToggleSelectAllMobs();
			}
			else
			{
				ModTanSat.ToggleSelectAllSkills();
			}
			ModConfig.SaveConfig();
			SoundMn.gI().buttonClick();
			return true;
		}

		// Checklist quái / skill
		if (px >= uiX + 14 && px <= uiX + uiW - 14 && py >= uiY + 118 && py <= uiY + 214)
		{
			if (ModUI.tanSatTab == 0)
			{
				List<int> mobIds = ModUI.GetUniqueMobTemplateIds();
				for (int idx = 0; idx <= mobIds.Count; idx++)
				{
					int col = idx % 2;
					int row = idx / 2;
					int itemX = (col == 0) ? (uiX + 22) : (uiX + 172);
					int itemY = uiY + 122 + row * 22;

					if (px >= itemX && px <= itemX + 140 && py >= itemY && py <= itemY + 20)
					{
						if (idx == 0)
						{
							ModTanSat.ToggleSelectAllMobs();
						}
						else
						{
							ModTanSat.ToggleMobTicked(mobIds[idx - 1]);
						}
						SoundMn.gI().buttonClick();
						return true;
					}
				}
			}
			else
			{
				List<Skill> skills = ModUI.GetPlayerAttackSkills();
				for (int idx = 0; idx <= skills.Count; idx++)
				{
					int col = idx % 2;
					int row = idx / 2;
					int itemX = (col == 0) ? (uiX + 22) : (uiX + 172);
					int itemY = uiY + 122 + row * 22;

					if (px >= itemX && px <= itemX + 140 && py >= itemY && py <= itemY + 20)
					{
						if (idx == 0)
						{
							ModTanSat.ToggleSelectAllSkills();
						}
						else
						{
							ModTanSat.ToggleSkillTicked(skills[idx - 1].template.id);
						}
						SoundMn.gI().buttonClick();
						return true;
					}
				}
			}
		}

		return false;
	}
}