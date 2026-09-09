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
	private static int startScrollY = 0;

	public static void OnMouseScroll(float wheel)
	{
		int step = 24;
		if (ModUI.tanSatTab == 0)
		{
			List<int> mobIds = ModUI.GetUniqueMobTemplateIds();
			int totalRows = (mobIds.Count + 1 + 1) / 2;
			int contentH = totalRows * 20 + 6;
			int maxScroll = (contentH > 118) ? (contentH - 118) : 0;
			if (maxScroll <= 0) return;

			if (wheel > 0) { scrollMobY -= step; if (scrollMobY < 0) scrollMobY = 0; }
			else if (wheel < 0) { scrollMobY += step; if (scrollMobY > maxScroll) scrollMobY = maxScroll; }
		}
		else
		{
			List<Skill> skills = ModUI.GetPlayerAttackSkills();
			int totalRows = (skills.Count + 1 + 1) / 2;
			int contentH = totalRows * 20 + 6;
			int maxScroll = (contentH > 118) ? (contentH - 118) : 0;
			if (maxScroll <= 0) return;

			if (wheel > 0) { scrollSkillY -= step; if (scrollSkillY < 0) scrollSkillY = 0; }
			else if (wheel < 0) { scrollSkillY += step; if (scrollSkillY > maxScroll) scrollSkillY = maxScroll; }
		}
	}

	public static void UpdateDragScroll(int px, int py, int uiX, int uiY, int uiW, int uiH)
	{
		int listX = uiX + 6;
		int listY = uiY + 70;
		int listW = uiW - 12;
		int listH = 124;

		int maxScroll = 0;
		if (ModUI.tanSatTab == 0)
		{
			List<int> mobIds = ModUI.GetUniqueMobTemplateIds();
			int totalRows = (mobIds.Count + 1 + 1) / 2;
			int contentH = totalRows * 20 + 6;
			maxScroll = (contentH > listH - 6) ? (contentH - (listH - 6)) : 0;
		}
		else
		{
			List<Skill> skills = ModUI.GetPlayerAttackSkills();
			int totalRows = (skills.Count + 1 + 1) / 2;
			int contentH = totalRows * 20 + 6;
			maxScroll = (contentH > listH - 6) ? (contentH - (listH - 6)) : 0;
		}

		bool isDown = Input.GetMouseButton(0) || GameCanvas.isPointerDown;
		bool isJustDown = Input.GetMouseButtonDown(0) || GameCanvas.isPointerJustDown;
		bool isJustRelease = Input.GetMouseButtonUp(0) || GameCanvas.isPointerJustRelease;

		if (isJustDown)
		{
			if (px >= listX && px <= listX + listW && py >= listY && py <= listY + listH)
			{
				isDragging = true;
				hasDragged = false;
				startDragY = py;
				startScrollY = (ModUI.tanSatTab == 0) ? scrollMobY : scrollSkillY;
			}
		}
		else if (isDown && isDragging)
		{
			int deltaY = py - startDragY;
			if (Res.abs(deltaY) > 4)
			{
				hasDragged = true;
			}
			if (hasDragged && maxScroll > 0)
			{
				int newScroll = startScrollY - deltaY;
				if (newScroll < 0) newScroll = 0;
				if (newScroll > maxScroll) newScroll = maxScroll;
				if (ModUI.tanSatTab == 0) scrollMobY = newScroll;
				else scrollSkillY = newScroll;
			}
		}
		else if (isJustRelease)
		{
			isDragging = false;
		}
	}

	public static void Paint(int uiX, int uiY, int uiW, int uiH, mGraphics g)
	{
		mFont.tahoma_7b_dark.drawString(g, "Trạng thái:", uiX + 8, uiY + 10, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 68, uiY + 6, 52, 18, ModTanSat.autoTanSat ? "BẬT" : "TẮT", ModTanSat.autoTanSat, g);

		int tpBtnW = 80;
		int tpBtnX = uiX + uiW - tpBtnW - 6;
		mFont.tahoma_7b_dark.drawString(g, "Tiếp cận:", tpBtnX - 52, uiY + 10, mFont.LEFT);
		ModUI.PaintNativeButton(tpBtnX, uiY + 6, tpBtnW, 18, ModTanSat.useTeleport ? "Dịch chuyển" : "Chạy bộ", ModTanSat.useTeleport, g);

		int tabBtnW = (uiW - 16) / 2;
		ModUI.PaintNativeButton(uiX + 6, uiY + 28, tabBtnW, 19, "1. Chọn Quái", ModUI.tanSatTab == 0, g);
		ModUI.PaintNativeButton(uiX + 10 + tabBtnW, uiY + 28, tabBtnW, 19, "2. Chọn Kỹ Năng", ModUI.tanSatTab == 1, g);

		int listX = uiX + 6;
		int listY = uiY + 70;
		int listW = uiW - 12;
		int listH = 124;
		int colW = (listW - 12) / 2;
		int allBtnW = 82;
		int allBtnX = uiX + uiW - allBtnW - 6;

		if (ModUI.tanSatTab == 0)
		{
			mFont.tahoma_7b_dark.drawString(g, "Quái map (Tick để đánh):", uiX + 6, uiY + 54, mFont.LEFT);

			ModUI.PaintNativeButton(allBtnX, uiY + 49, allBtnW, 18, ModTanSat.selectAllMobs ? "Bỏ chọn hết" : "Chọn tất cả", ModTanSat.selectAllMobs, g);

			GameCanvas.paintz.paintFrameSimple(listX, listY, listW, listH, g);
			g.setColor(15196114);
			g.fillRect(listX + 2, listY + 2, listW - 4, listH - 4);

			List<int> mobIds = ModUI.GetUniqueMobTemplateIds();
			int totalRows = (mobIds.Count + 1 + 1) / 2;
			int contentH = totalRows * 20 + 6;
			int maxScroll = (contentH > listH - 6) ? (contentH - (listH - 6)) : 0;
			if (scrollMobY > maxScroll) scrollMobY = maxScroll;
			if (scrollMobY < 0) scrollMobY = 0;

			if (mobIds.Count == 0)
			{
				mFont.tahoma_7_grey.drawString(g, "(Chưa thấy quái nào trong map)", listX + listW / 2, listY + 55, mFont.CENTER);
			}
			else
			{
				g.setClip(listX + 2, listY + 2, listW - 4, listH - 4);
				for (int idx = 0; idx <= mobIds.Count; idx++)
				{
					int col = idx % 2;
					int row = idx / 2;
					int itemX = listX + 4 + col * (colW + 4);
					int itemY = listY + 4 + row * 20 - scrollMobY;

					if (itemY + 20 < listY || itemY > listY + listH) continue;

					if (idx == 0)
					{
						ModUI.DrawCheckbox(itemX, itemY + 2, ModTanSat.selectAllMobs, g);
						mFont.tahoma_7b_dark.drawString(g, "Tất cả quái", itemX + 22, itemY + 4, mFont.LEFT);
					}
					else
					{
						int mobTpl = mobIds[idx - 1];
						string mobName = (Mob.arrMobTemplate != null && mobTpl >= 0 && mobTpl < Mob.arrMobTemplate.Length && Mob.arrMobTemplate[mobTpl] != null) ? Mob.arrMobTemplate[mobTpl].name : ("Quái #" + mobTpl);
						bool isTicked = ModTanSat.IsMobTicked(mobTpl);
						ModUI.DrawCheckbox(itemX, itemY + 2, isTicked, g);
						if (isTicked)
						{
							mFont.tahoma_7b_green2.drawString(g, mobName, itemX + 22, itemY + 4, mFont.LEFT);
						}
						else
						{
							mFont.tahoma_7b_dark.drawString(g, mobName, itemX + 22, itemY + 4, mFont.LEFT);
						}
					}
				}
				g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
			}
		}
		else
		{
			mFont.tahoma_7b_dark.drawString(g, "Kỹ năng nhân vật (Tick để dùng):", uiX + 6, uiY + 54, mFont.LEFT);

			ModUI.PaintNativeButton(allBtnX, uiY + 49, allBtnW, 18, ModTanSat.selectAllSkills ? "Tự động chiêu" : "Tất cả chiêu", ModTanSat.selectAllSkills, g);

			GameCanvas.paintz.paintFrameSimple(listX, listY, listW, listH, g);
			g.setColor(15196114);
			g.fillRect(listX + 2, listY + 2, listW - 4, listH - 4);

			List<Skill> skills = ModUI.GetPlayerAttackSkills();
			int totalRows = (skills.Count + 1 + 1) / 2;
			int contentH = totalRows * 20 + 6;
			int maxScroll = (contentH > listH - 6) ? (contentH - (listH - 6)) : 0;
			if (scrollSkillY > maxScroll) scrollSkillY = maxScroll;
			if (scrollSkillY < 0) scrollSkillY = 0;

			if (skills.Count == 0)
			{
				mFont.tahoma_7_grey.drawString(g, "(Nhân vật chưa học kỹ năng nào)", listX + listW / 2, listY + 55, mFont.CENTER);
			}
			else
			{
				g.setClip(listX + 2, listY + 2, listW - 4, listH - 4);
				for (int idx = 0; idx <= skills.Count; idx++)
				{
					int col = idx % 2;
					int row = idx / 2;
					int itemX = listX + 4 + col * (colW + 4);
					int itemY = listY + 4 + row * 20 - scrollSkillY;

					if (itemY + 20 < listY || itemY > listY + listH) continue;

					if (idx == 0)
					{
						ModUI.DrawCheckbox(itemX, itemY + 2, ModTanSat.selectAllSkills, g);
						mFont.tahoma_7b_dark.drawString(g, "Tất cả kỹ năng", itemX + 22, itemY + 4, mFont.LEFT);
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
							mFont.tahoma_7b_green2.drawString(g, skName, itemX + 22, itemY + 4, mFont.LEFT);
						}
						else
						{
							mFont.tahoma_7b_dark.drawString(g, skName, itemX + 22, itemY + 4, mFont.LEFT);
						}
					}
				}
				g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
			}
		}

		// Thiet lap Time Attack (Delay giua cac don danh)
		mFont.tahoma_7b_dark.drawString(g, "Time attack:", uiX + 8, uiY + 203, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 78, uiY + 199, 20, 18, "-", false, g);
		ModUI.PaintNativeButton(uiX + 100, uiY + 199, 50, 18, ModTanSat.timeAttack + "ms", true, g);
		ModUI.PaintNativeButton(uiX + 152, uiY + 199, 20, 18, "+", false, g);

		// Cac preset nhanh
		ModUI.PaintNativeButton(uiX + 178, uiY + 199, 32, 18, "100", ModTanSat.timeAttack == 100, g);
		ModUI.PaintNativeButton(uiX + 212, uiY + 199, 32, 18, "200", ModTanSat.timeAttack == 200, g);
		ModUI.PaintNativeButton(uiX + 246, uiY + 199, 32, 18, "300", ModTanSat.timeAttack == 300, g);
		ModUI.PaintNativeButton(uiX + 280, uiY + 199, 32, 18, "500", ModTanSat.timeAttack == 500, g);
	}

	public static bool HandleTap(int px, int py, int uiX, int uiY, int uiW, int uiH)
	{
		if (hasDragged)
		{
			hasDragged = false;
			return true;
		}

		// Bat/Tat Tan sat
		if (px >= uiX + 68 && px <= uiX + 120 && py >= uiY + 5 && py <= uiY + 25)
		{
			ModTanSat.autoTanSat = !ModTanSat.autoTanSat;
			ModConfig.SaveConfig();
			SoundMn.gI().buttonClick();
			return true;
		}

		// Bat/Tat Dich chuyen / Chay bo
		int tpBtnW = 80;
		int tpBtnX = uiX + uiW - tpBtnW - 6;
		if (px >= tpBtnX && px <= tpBtnX + tpBtnW && py >= uiY + 5 && py <= uiY + 25)
		{
			ModTanSat.useTeleport = !ModTanSat.useTeleport;
			ModConfig.SaveConfig();
			SoundMn.gI().buttonClick();
			return true;
		}

		// Sub tabs (1. Chon Quai / 2. Chon Ky Nang)
		int tabBtnW = (uiW - 16) / 2;
		if (py >= uiY + 27 && py <= uiY + 48)
		{
			if (px >= uiX + 6 && px <= uiX + 6 + tabBtnW)
			{
				ModUI.tanSatTab = 0;
				SoundMn.gI().buttonClick();
				return true;
			}
			if (px >= uiX + 10 + tabBtnW && px <= uiX + 10 + tabBtnW * 2)
			{
				ModUI.tanSatTab = 1;
				SoundMn.gI().buttonClick();
				return true;
			}
		}

		// Nut Chon tat ca / Bo chon het
		int allBtnW = 82;
		int allBtnX = uiX + uiW - allBtnW - 6;
		if (px >= allBtnX && px <= allBtnX + allBtnW && py >= uiY + 47 && py <= uiY + 68)
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

		int listX = uiX + 6;
		int listY = uiY + 70;
		int listW = uiW - 12;
		int listH = 124;
		int colW = (listW - 12) / 2;

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
					int itemX = listX + 4 + col * (colW + 4);
					int itemY = listY + 4 + row * 20 - scrollMobY;

					if (itemY + 20 < listY || itemY > listY + listH) continue;

					if (px >= itemX && px <= itemX + colW && py >= itemY && py <= itemY + 20)
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
					int itemY = listY + 4 + row * 20 - scrollSkillY;

					if (itemY + 20 < listY || itemY > listY + listH) continue;

					if (px >= itemX && px <= itemX + colW && py >= itemY && py <= itemY + 20)
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

		// Dieu chinh Time Attack (Y: uiY + 197 den uiY + 219)
		if (py >= uiY + 197 && py <= uiY + 219)
		{
			// Nut [-]
			if (px >= uiX + 78 && px <= uiX + 98)
			{
				ModTanSat.timeAttack -= 50;
				if (ModTanSat.timeAttack < 50) ModTanSat.timeAttack = 50;
				ModConfig.SaveConfig();
				SoundMn.gI().buttonClick();
				return true;
			}
			// Nut current [ xxx ms ]
			if (px >= uiX + 100 && px <= uiX + 150)
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
			// Nut [+]
			if (px >= uiX + 152 && px <= uiX + 172)
			{
				ModTanSat.timeAttack += 50;
				if (ModTanSat.timeAttack > 3000) ModTanSat.timeAttack = 3000;
				ModConfig.SaveConfig();
				SoundMn.gI().buttonClick();
				return true;
			}
			// Preset 100
			if (px >= uiX + 178 && px <= uiX + 210)
			{
				ModTanSat.timeAttack = 100;
				ModConfig.SaveConfig();
				SoundMn.gI().buttonClick();
				return true;
			}
			// Preset 200
			if (px >= uiX + 212 && px <= uiX + 244)
			{
				ModTanSat.timeAttack = 200;
				ModConfig.SaveConfig();
				SoundMn.gI().buttonClick();
				return true;
			}
			// Preset 300
			if (px >= uiX + 246 && px <= uiX + 278)
			{
				ModTanSat.timeAttack = 300;
				ModConfig.SaveConfig();
				SoundMn.gI().buttonClick();
				return true;
			}
			// Preset 500
			if (px >= uiX + 280 && px <= uiX + 312)
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
