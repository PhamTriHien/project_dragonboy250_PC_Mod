using System;
using System.Collections.Generic;
using UnityEngine;

public static class ModUITanSat
{
	public static int scrollSkillY = 0;
	public static int scrollMobY = 0;

	private static bool isDragging = false;
	private static bool hasDragged = false;
	private static int startDragY = 0;
	private static int lastDragY = 0;

	public static void OnMouseScroll(float wheel)
	{
		int step = 22;
		if (ModUI.tanSatTab == 0)
		{
			List<int> mobIds = ModUI.GetUniqueMobTemplateIds();
			int totalRows = (mobIds.Count + 1 + 1) / 2;
			int contentH = totalRows * 19 + 4;
			int maxScroll = (contentH > 126) ? (contentH - 126) : 0;
			if (maxScroll <= 0) return;

			if (wheel > 0) { scrollMobY -= step; if (scrollMobY < 0) scrollMobY = 0; }
			else if (wheel < 0) { scrollMobY += step; if (scrollMobY > maxScroll) scrollMobY = maxScroll; }
		}
		else
		{
			List<Skill> skills = ModUI.GetPlayerAttackSkills();
			int totalRows = (skills.Count + 1 + 1) / 2;
			int contentH = totalRows * 19 + 4;
			int maxScroll = (contentH > 126) ? (contentH - 126) : 0;
			if (maxScroll <= 0) return;

			if (wheel > 0) { scrollSkillY -= step; if (scrollSkillY < 0) scrollSkillY = 0; }
			else if (wheel < 0) { scrollSkillY += step; if (scrollSkillY > maxScroll) scrollSkillY = maxScroll; }
		}
	}

	public static void UpdateDragScroll(int px, int py, int uiX, int uiY, int uiW, int uiH)
	{
		int listX = uiX + 5;
		int listY = uiY + 59;
		int listW = uiW - 10;
		int listH = 132;

		int maxScroll = 0;
		if (ModUI.tanSatTab == 0)
		{
			List<int> mobIds = ModUI.GetUniqueMobTemplateIds();
			int totalRows = (mobIds.Count + 1 + 1) / 2;
			int contentH = totalRows * 19 + 4;
			maxScroll = (contentH > listH - 4) ? (contentH - (listH - 4)) : 0;
		}
		else
		{
			List<Skill> skills = ModUI.GetPlayerAttackSkills();
			int totalRows = (skills.Count + 1 + 1) / 2;
			int contentH = totalRows * 19 + 4;
			maxScroll = (contentH > listH - 4) ? (contentH - (listH - 4)) : 0;
		}

		bool isDown = Input.GetMouseButton(0) || GameCanvas.isPointerDown;
		if (isDown)
		{
			if (!isDragging)
			{
				if (px >= listX && px <= listX + listW && py >= listY && py <= listY + listH)
				{
					isDragging = true;
					hasDragged = false;
					startDragY = py;
					lastDragY = py;
				}
			}
			else
			{
				int moveY = py - lastDragY;
				lastDragY = py;
				if (Res.abs(py - startDragY) > 5)
				{
					hasDragged = true;
				}
				if (hasDragged && maxScroll > 0)
				{
					if (ModUI.tanSatTab == 0)
					{
						scrollMobY -= moveY;
						if (scrollMobY < 0) scrollMobY = 0;
						if (scrollMobY > maxScroll) scrollMobY = maxScroll;
					}
					else
					{
						scrollSkillY -= moveY;
						if (scrollSkillY < 0) scrollSkillY = 0;
						if (scrollSkillY > maxScroll) scrollSkillY = maxScroll;
					}
				}
			}
		}
		else
		{
			isDragging = false;
		}
	}

	public static void Paint(int uiX, int uiY, int uiW, int uiH, mGraphics g)
	{
		mFont.tahoma_7_grey.drawString(g, "Trạng thái:", uiX + 6, uiY + 6, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 58, uiY + 3, 44, 16, ModTanSat.autoTanSat ? "BẬT" : "TẮT", ModTanSat.autoTanSat, g);

		int tpBtnW = 74;
		int tpBtnX = uiX + uiW - tpBtnW - 6;
		mFont.tahoma_7_grey.drawString(g, "Tiếp cận:", tpBtnX - 48, uiY + 6, mFont.LEFT);
		ModUI.PaintNativeButton(tpBtnX, uiY + 3, tpBtnW, 16, ModTanSat.useTeleport ? "Dịch chuyển" : "Chạy bộ", ModTanSat.useTeleport, g);

		int tabBtnW = (uiW - 14) / 2;
		ModUI.PaintNativeButton(uiX + 5, uiY + 22, tabBtnW, 17, "1. Chọn Quái", ModUI.tanSatTab == 0, g);
		ModUI.PaintNativeButton(uiX + 9 + tabBtnW, uiY + 22, tabBtnW, 17, "2. Chọn Kỹ Năng", ModUI.tanSatTab == 1, g);

		int listX = uiX + 5;
		int listY = uiY + 59;
		int listW = uiW - 10;
		int listH = 132;
		int colW = (listW - 10) / 2;
		int allBtnW = 76;
		int allBtnX = uiX + uiW - allBtnW - 6;

		if (ModUI.tanSatTab == 0)
		{
			mFont.tahoma_7_grey.drawString(g, "Quái map (Tick để đánh):", uiX + 6, uiY + 44, mFont.LEFT);
			ModUI.PaintNativeButton(allBtnX, uiY + 41, allBtnW, 16, ModTanSat.selectAllMobs ? "Bỏ chọn hết" : "Chọn tất cả", ModTanSat.selectAllMobs, g);

			GameCanvas.paintz.paintFrameSimple(listX, listY, listW, listH, g);
			g.setColor(15196114);
			g.fillRect(listX + 2, listY + 2, listW - 4, listH - 4);

			List<int> mobIds = ModUI.GetUniqueMobTemplateIds();
			int totalRows = (mobIds.Count + 1 + 1) / 2;
			int contentH = totalRows * 19 + 4;
			int maxScroll = (contentH > listH - 4) ? (contentH - (listH - 4)) : 0;
			if (scrollMobY > maxScroll) scrollMobY = maxScroll;
			if (scrollMobY < 0) scrollMobY = 0;

			if (mobIds.Count == 0)
			{
				mFont.tahoma_7_grey.drawString(g, "(Chưa thấy quái nào trong map)", listX + listW / 2, listY + 58, mFont.CENTER);
			}
			else
			{
				g.setClip(listX + 2, listY + 2, listW - 4, listH - 4);
				for (int idx = 0; idx <= mobIds.Count; idx++)
				{
					int col = idx % 2;
					int row = idx / 2;
					int itemX = listX + 4 + col * (colW + 4);
					int itemY = listY + 3 + row * 19 - scrollMobY;

					if (itemY + 19 < listY || itemY > listY + listH) continue;

					if (idx == 0)
					{
						ModUI.DrawCheckbox(itemX, itemY + 1, ModTanSat.selectAllMobs, g);
						(ModTanSat.selectAllMobs ? mFont.tahoma_7_green2 : mFont.tahoma_7_grey).drawString(g, "Tất cả quái", itemX + 22, itemY + 2, mFont.LEFT);
					}
					else
					{
						int tId = mobIds[idx - 1];
						MobTemplate t = Mob.arrMobTemplate[tId];
						string mobName = (t != null) ? t.name : ("Quái #" + tId);
						bool isTicked = ModTanSat.IsMobTicked(tId);
						ModUI.DrawCheckbox(itemX, itemY + 1, isTicked, g);
						(isTicked ? mFont.tahoma_7_green2 : mFont.tahoma_7_grey).drawString(g, mobName, itemX + 22, itemY + 2, mFont.LEFT);
					}
				}
				g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
			}
		}
		else
		{
			mFont.tahoma_7_grey.drawString(g, "Kỹ năng nhân vật (Tick để dùng):", uiX + 6, uiY + 44, mFont.LEFT);
			ModUI.PaintNativeButton(allBtnX, uiY + 41, allBtnW, 16, ModTanSat.selectAllSkills ? "Tự động chiêu" : "Tất cả chiêu", ModTanSat.selectAllSkills, g);

			GameCanvas.paintz.paintFrameSimple(listX, listY, listW, listH, g);
			g.setColor(15196114);
			g.fillRect(listX + 2, listY + 2, listW - 4, listH - 4);

			List<Skill> skills = ModUI.GetPlayerAttackSkills();
			int totalRows = (skills.Count + 1 + 1) / 2;
			int contentH = totalRows * 19 + 4;
			int maxScroll = (contentH > listH - 4) ? (contentH - (listH - 4)) : 0;
			if (scrollSkillY > maxScroll) scrollSkillY = maxScroll;
			if (scrollSkillY < 0) scrollSkillY = 0;

			if (skills.Count == 0)
			{
				mFont.tahoma_7_grey.drawString(g, "(Nhân vật chưa học kỹ năng nào)", listX + listW / 2, listY + 58, mFont.CENTER);
			}
			else
			{
				g.setClip(listX + 2, listY + 2, listW - 4, listH - 4);
				for (int idx = 0; idx <= skills.Count; idx++)
				{
					int col = idx % 2;
					int row = idx / 2;
					int itemX = listX + 4 + col * (colW + 4);
					int itemY = listY + 3 + row * 19 - scrollSkillY;

					if (itemY + 19 < listY || itemY > listY + listH) continue;

					if (idx == 0)
					{
						ModUI.DrawCheckbox(itemX, itemY + 1, ModTanSat.selectAllSkills, g);
						(ModTanSat.selectAllSkills ? mFont.tahoma_7_green2 : mFont.tahoma_7_grey).drawString(g, "Tất cả kỹ năng", itemX + 22, itemY + 2, mFont.LEFT);
					}
					else
					{
						Skill sk = skills[idx - 1];
						int slot = ModTanSatFilter.GetSkillHotbarSlot(sk.template.id);
						string slotPrefix = (slot >= 0) ? ("[Ô " + ((slot == 9) ? 0 : (slot + 1)) + "] ") : "";
						string skName = slotPrefix + ((sk != null && sk.template != null) ? sk.template.name : ("Skill #" + idx));
						bool isTicked = ModTanSat.IsSkillTicked(sk.template.id);
						ModUI.DrawCheckbox(itemX, itemY + 1, isTicked, g);
						(isTicked ? mFont.tahoma_7_green2 : mFont.tahoma_7_grey).drawString(g, skName, itemX + 22, itemY + 2, mFont.LEFT);
					}
				}
				g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
			}
		}

		// Thiết lập Time Attack
		mFont.tahoma_7_grey.drawString(g, "Time attack:", uiX + 6, uiY + 198, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 66, uiY + 194, 18, 16, "-", false, g);
		ModUI.PaintNativeButton(uiX + 86, uiY + 194, 46, 16, ModTanSat.timeAttack + "ms", true, g);
		ModUI.PaintNativeButton(uiX + 134, uiY + 194, 18, 16, "+", false, g);

		// Các preset nhanh
		ModUI.PaintNativeButton(uiX + 156, uiY + 194, 28, 16, "100", ModTanSat.timeAttack == 100, g);
		ModUI.PaintNativeButton(uiX + 186, uiY + 194, 28, 16, "200", ModTanSat.timeAttack == 200, g);
		ModUI.PaintNativeButton(uiX + 216, uiY + 194, 28, 16, "300", ModTanSat.timeAttack == 300, g);
		ModUI.PaintNativeButton(uiX + 246, uiY + 194, 28, 16, "500", ModTanSat.timeAttack == 500, g);
	}

	public static bool HandleTap(int px, int py, int uiX, int uiY, int uiW, int uiH)
	{
		if (hasDragged)
		{
			hasDragged = false;
			return true;
		}

		// Bật/Tắt Tàn sát
		if (px >= uiX + 58 && px <= uiX + 102 && py >= uiY + 2 && py <= uiY + 20)
		{
			ModTanSat.autoTanSat = !ModTanSat.autoTanSat;
			ModConfig.SaveConfig();
			SoundMn.gI().buttonClick();
			return true;
		}

		// Nút Tiếp cận
		int tpBtnW = 74;
		int tpBtnX = uiX + uiW - tpBtnW - 6;
		if (px >= tpBtnX && px <= tpBtnX + tpBtnW && py >= uiY + 2 && py <= uiY + 20)
		{
			ModTanSat.useTeleport = !ModTanSat.useTeleport;
			ModConfig.SaveConfig();
			SoundMn.gI().buttonClick();
			return true;
		}

		// Sub tabs (1. Chọn Quái / 2. Chọn Kỹ Năng)
		int tabBtnW = (uiW - 14) / 2;
		if (py >= uiY + 21 && py <= uiY + 40)
		{
			if (px >= uiX + 5 && px <= uiX + 5 + tabBtnW)
			{
				ModUI.tanSatTab = 0;
				SoundMn.gI().buttonClick();
				return true;
			}
			if (px >= uiX + 9 + tabBtnW && px <= uiX + 9 + tabBtnW * 2)
			{
				ModUI.tanSatTab = 1;
				SoundMn.gI().buttonClick();
				return true;
			}
		}

		// Nút Chọn tất cả / Bỏ chọn hết
		int allBtnW = 76;
		int allBtnX = uiX + uiW - allBtnW - 6;
		if (px >= allBtnX && px <= allBtnX + allBtnW && py >= uiY + 40 && py <= uiY + 58)
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

		int listX = uiX + 5;
		int listY = uiY + 59;
		int listW = uiW - 10;
		int listH = 132;
		int colW = (listW - 10) / 2;

		// Checklist quái / skill
		if (px >= listX && px <= listX + listW && py >= listY && py <= listY + listH)
		{
			if (ModUI.tanSatTab == 0)
			{
				List<int> mobIds = ModUI.GetUniqueMobTemplateIds();
				for (int idx = 0; idx <= mobIds.Count; idx++)
				{
					int col = idx % 2;
					int row = idx / 2;
					int itemX = listX + 4 + col * (colW + 4);
					int itemY = listY + 3 + row * 19 - scrollMobY;

					if (itemY + 19 < listY || itemY > listY + listH) continue;

					if (px >= itemX && px <= itemX + colW && py >= itemY && py <= itemY + 19)
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
					int itemX = listX + 4 + col * (colW + 4);
					int itemY = listY + 3 + row * 19 - scrollSkillY;

					if (itemY + 19 < listY || itemY > listY + listH) continue;

					if (px >= itemX && px <= itemX + colW && py >= itemY && py <= itemY + 19)
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

		// Điều chỉnh Time Attack (Y: uiY + 192 đến uiY + 212)
		if (py >= uiY + 192 && py <= uiY + 212)
		{
			// Nút [-]
			if (px >= uiX + 66 && px <= uiX + 84)
			{
				ModTanSat.timeAttack -= 50;
				if (ModTanSat.timeAttack < 50) ModTanSat.timeAttack = 50;
				ModConfig.SaveConfig();
				SoundMn.gI().buttonClick();
				return true;
			}
			// Nút current [ xxx ms ]
			if (px >= uiX + 86 && px <= uiX + 132)
			{
				int[] cycle = new int[] { 100, 200, 300, 400, 500, 700, 1000 };
				int nextVal = 300;
				for (int c = 0; c < cycle.Length; c++)
				{
					if (cycle[c] > ModTanSat.timeAttack)
					{
						nextVal = cycle[c];
						break;
					}
					if (c == cycle.Length - 1)
					{
						nextVal = cycle[0];
					}
				}
				ModTanSat.timeAttack = nextVal;
				ModConfig.SaveConfig();
				SoundMn.gI().buttonClick();
				return true;
			}
			// Nút [+]
			if (px >= uiX + 134 && px <= uiX + 152)
			{
				ModTanSat.timeAttack += 50;
				if (ModTanSat.timeAttack > 3000) ModTanSat.timeAttack = 3000;
				ModConfig.SaveConfig();
				SoundMn.gI().buttonClick();
				return true;
			}
			// Preset 100
			if (px >= uiX + 156 && px <= uiX + 184)
			{
				ModTanSat.timeAttack = 100;
				ModConfig.SaveConfig();
				SoundMn.gI().buttonClick();
				return true;
			}
			// Preset 200
			if (px >= uiX + 186 && px <= uiX + 214)
			{
				ModTanSat.timeAttack = 200;
				ModConfig.SaveConfig();
				SoundMn.gI().buttonClick();
				return true;
			}
			// Preset 300
			if (px >= uiX + 216 && px <= uiX + 244)
			{
				ModTanSat.timeAttack = 300;
				ModConfig.SaveConfig();
				SoundMn.gI().buttonClick();
				return true;
			}
			// Preset 500
			if (px >= uiX + 246 && px <= uiX + 274)
			{
				ModTanSat.timeAttack = 500;
				ModConfig.SaveConfig();
				SoundMn.gI().buttonClick();
				return true;
			}
		}

		return false;
	}
}
