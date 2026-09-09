using System;
using System.Collections.Generic;
using UnityEngine;

public static class ModUITanSat
{
	public static int scrollSkillY = 0;
	public static int scrollMobY = 0;

	public static void Paint(int uiX, int uiY, int uiW, int uiH, mGraphics g)
	{
		mFont.tahoma_7b_white.drawString(g, "Trạng thái:", uiX + 16, uiY + 55, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 80, uiY + 51, 52, 18, ModTanSat.autoTanSat ? "BẬT" : "TẮT", ModTanSat.autoTanSat, g);

		mFont.tahoma_7b_white.drawString(g, "Tiếp cận:", uiX + 150, uiY + 55, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 205, uiY + 51, 80, 18, ModTanSat.useTeleport ? "Dịch chuyển" : "Chạy bộ", ModTanSat.useTeleport, g);

		ModUI.PaintNativeButton(uiX + 16, uiY + 74, 145, 19, "1. Chọn Quái", ModUI.tanSatTab == 0, g);
		ModUI.PaintNativeButton(uiX + 178, uiY + 74, 145, 19, "2. Chọn Kỹ Năng", ModUI.tanSatTab == 1, g);

		int listX = uiX + 14;
		int listY = uiY + 118;
		int listW = uiW - 28;
		int listH = 100;

		if (ModUI.tanSatTab == 0)
		{
			mFont.tahoma_7b_yellow.drawString(g, "Quái trong map (Tick để đánh):", uiX + 16, uiY + 102, mFont.LEFT);

			g.setColor(0x333333);
			g.fillRect(uiX + uiW - 146, uiY + 98, 88, 18);
			g.setColor(0x888888);
			g.drawRect(uiX + uiW - 146, uiY + 98, 88, 18);
			mFont.tahoma_7_yellow.drawString(g, ModTanSat.selectAllMobs ? "Bỏ chọn hết" : "Chọn tất cả", uiX + uiW - 102, uiY + 100, mFont.CENTER);

			ModUI.PaintNativeButton(uiX + uiW - 54, uiY + 98, 22, 18, "▲", false, g);
			ModUI.PaintNativeButton(uiX + uiW - 28, uiY + 98, 22, 18, "▼", false, g);

			GameCanvas.paintz.paintFrameSimple(listX, listY, listW, listH, g);
			g.setColor(0x181818);
			g.fillRect(listX + 2, listY + 2, listW - 4, listH - 4);

			List<int> mobIds = ModUI.GetUniqueMobTemplateIds();
			int totalRows = (mobIds.Count + 1 + 1) / 2;
			int contentH = totalRows * 20 + 6;
			int maxScroll = (contentH > listH - 6) ? (contentH - (listH - 6)) : 0;
			if (scrollMobY > maxScroll) scrollMobY = maxScroll;
			if (scrollMobY < 0) scrollMobY = 0;

			if (mobIds.Count == 0)
			{
				mFont.tahoma_7_grey.drawString(g, "(Chưa thấy quái nào trong map)", listX + listW / 2, listY + 45, mFont.CENTER);
			}
			else
			{
				g.setClip(listX + 2, listY + 2, listW - 4, listH - 4);
				for (int idx = 0; idx <= mobIds.Count; idx++)
				{
					int col = idx % 2;
					int row = idx / 2;
					int itemX = (col == 0) ? (uiX + 22) : (uiX + 172);
					int itemY = listY + 4 + row * 20 - scrollMobY;

					if (itemY + 20 < listY || itemY > listY + listH) continue;

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
				g.setClip(0, 0, GameCanvas.w, GameCanvas.h);

				if (maxScroll > 0)
				{
					int barH = listH * (listH - 4) / contentH;
					if (barH < 12) barH = 12;
					int barY = listY + 2 + (listH - 4 - barH) * scrollMobY / maxScroll;
					g.setColor(0x00e676);
					g.fillRect(listX + listW - 4, barY, 2, barH);
				}
			}
		}
		else
		{
			mFont.tahoma_7b_yellow.drawString(g, "Kỹ năng nhân vật (Tick để dùng):", uiX + 16, uiY + 102, mFont.LEFT);

			g.setColor(0x333333);
			g.fillRect(uiX + uiW - 146, uiY + 98, 88, 18);
			g.setColor(0x888888);
			g.drawRect(uiX + uiW - 146, uiY + 98, 88, 18);
			mFont.tahoma_7_yellow.drawString(g, ModTanSat.selectAllSkills ? "Bỏ chọn hết" : "Chọn tất cả", uiX + uiW - 102, uiY + 100, mFont.CENTER);

			ModUI.PaintNativeButton(uiX + uiW - 54, uiY + 98, 22, 18, "▲", false, g);
			ModUI.PaintNativeButton(uiX + uiW - 28, uiY + 98, 22, 18, "▼", false, g);

			GameCanvas.paintz.paintFrameSimple(listX, listY, listW, listH, g);
			g.setColor(0x181818);
			g.fillRect(listX + 2, listY + 2, listW - 4, listH - 4);

			List<Skill> skills = ModUI.GetPlayerAttackSkills();
			int totalRows = (skills.Count + 1 + 1) / 2;
			int contentH = totalRows * 20 + 6;
			int maxScroll = (contentH > listH - 6) ? (contentH - (listH - 6)) : 0;
			if (scrollSkillY > maxScroll) scrollSkillY = maxScroll;
			if (scrollSkillY < 0) scrollSkillY = 0;

			if (skills.Count == 0)
			{
				mFont.tahoma_7_grey.drawString(g, "(Nhân vật chưa học kỹ năng nào)", listX + listW / 2, listY + 45, mFont.CENTER);
			}
			else
			{
				g.setClip(listX + 2, listY + 2, listW - 4, listH - 4);
				for (int idx = 0; idx <= skills.Count; idx++)
				{
					int col = idx % 2;
					int row = idx / 2;
					int itemX = (col == 0) ? (uiX + 22) : (uiX + 172);
					int itemY = listY + 4 + row * 20 - scrollSkillY;

					if (itemY + 20 < listY || itemY > listY + listH) continue;

					if (idx == 0)
					{
						ModUI.DrawCheckbox(itemX, itemY + 2, ModTanSat.selectAllSkills, g);
						mFont.tahoma_7b_yellow.drawString(g, "Tất cả kỹ năng", itemX + 20, itemY + 3, mFont.LEFT);
					}
					else
					{
						Skill sk = skills[idx - 1];
						int slot = ModTanSatFilter.GetSkillHotbarSlot(sk.template.id);
						string slotPrefix = (slot >= 0) ? ("[Ô " + ((slot == 9) ? 0 : (slot + 1)) + "] ") : "";
						string skName = slotPrefix + ((sk != null && sk.template != null) ? sk.template.name : ("Skill #" + idx));
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
				g.setClip(0, 0, GameCanvas.w, GameCanvas.h);

				if (maxScroll > 0)
				{
					int barH = listH * (listH - 4) / contentH;
					if (barH < 12) barH = 12;
					int barY = listY + 2 + (listH - 4 - barH) * scrollSkillY / maxScroll;
					g.setColor(0x00e676);
					g.fillRect(listX + listW - 4, barY, 2, barH);
				}
			}
		}
	}

	public static bool HandleTap(int px, int py, int uiX, int uiY, int uiW, int uiH)
	{
		// Bat/Tat Tan sat
		if (px >= uiX + 80 && px <= uiX + 132 && py >= uiY + 50 && py <= uiY + 70)
		{
			ModTanSat.autoTanSat = !ModTanSat.autoTanSat;
			ModConfig.SaveConfig();
			SoundMn.gI().buttonClick();
			return true;
		}

		// Bat/Tat Dich chuyen / Chay bo
		if (px >= uiX + 205 && px <= uiX + 285 && py >= uiY + 50 && py <= uiY + 70)
		{
			ModTanSat.useTeleport = !ModTanSat.useTeleport;
			ModConfig.SaveConfig();
			SoundMn.gI().buttonClick();
			return true;
		}

		// Sub tabs (1. Chon Quai / 2. Chon Ky Nang)
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

		// Nut Chon tat ca / Bo chon het
		if (px >= uiX + uiW - 146 && px <= uiX + uiW - 58 && py >= uiY + 96 && py <= uiY + 118)
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

		// Nut Cuon len (▲)
		if (px >= uiX + uiW - 54 && px <= uiX + uiW - 32 && py >= uiY + 96 && py <= uiY + 118)
		{
			if (ModUI.tanSatTab == 0)
			{
				scrollMobY -= 40;
				if (scrollMobY < 0) scrollMobY = 0;
			}
			else
			{
				scrollSkillY -= 40;
				if (scrollSkillY < 0) scrollSkillY = 0;
			}
			SoundMn.gI().buttonClick();
			return true;
		}

		// Nut Cuon xuong (▼)
		if (px >= uiX + uiW - 28 && px <= uiX + uiW - 6 && py >= uiY + 96 && py <= uiY + 118)
		{
			if (ModUI.tanSatTab == 0)
			{
				scrollMobY += 40;
			}
			else
			{
				scrollSkillY += 40;
			}
			SoundMn.gI().buttonClick();
			return true;
		}

		int listX = uiX + 14;
		int listY = uiY + 118;
		int listW = uiW - 28;
		int listH = 100;

		// Con lan chuot (Mouse Scroll Wheel)
		if (px >= listX && px <= listX + listW && py >= listY && py <= listY + listH)
		{
			float wheel = Input.GetAxis("Mouse ScrollWheel");
			if (wheel > 0)
			{
				if (ModUI.tanSatTab == 0) { scrollMobY -= 20; if (scrollMobY < 0) scrollMobY = 0; }
				else { scrollSkillY -= 20; if (scrollSkillY < 0) scrollSkillY = 0; }
				return true;
			}
			else if (wheel < 0)
			{
				if (ModUI.tanSatTab == 0) { scrollMobY += 20; }
				else { scrollSkillY += 20; }
				return true;
			}
		}

		// Checklist quai / skill
		if (px >= listX && px <= listX + listW && py >= listY && py <= listY + listH)
		{
			if (ModUI.tanSatTab == 0)
			{
				List<int> mobIds = ModUI.GetUniqueMobTemplateIds();
				for (int idx = 0; idx <= mobIds.Count; idx++)
				{
					int col = idx % 2;
					int row = idx / 2;
					int itemX = (col == 0) ? (uiX + 22) : (uiX + 172);
					int itemY = listY + 4 + row * 20 - scrollMobY;

					if (itemY + 20 < listY || itemY > listY + listH) continue;

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
					int itemY = listY + 4 + row * 20 - scrollSkillY;

					if (itemY + 20 < listY || itemY > listY + listH) continue;

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
