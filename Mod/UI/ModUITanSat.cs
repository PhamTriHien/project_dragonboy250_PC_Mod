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
		int row2Y = uiY + 45;
		int row2H = 17;
		int listX = uiX + 4;
		int listY = row2Y + row2H + 3;
		int listW = uiW - 8;
		int footerH = 18;
		int footerY = uiY + uiH - footerH - 4;
		int listH = (footerY - 4) - listY;
		if (listH < 40) listH = 40;

		int maxScroll = 0;
		if (ModUI.tanSatTab == 0)
		{
			List<int> mobIds = ModUI.GetUniqueMobTemplateIds();
			int totalRows = (mobIds.Count + 1 + 1) / 2;
			int contentH = totalRows * 18 + 4;
			maxScroll = (contentH > listH - 4) ? (contentH - (listH - 4)) : 0;
		}
		else
		{
			List<Skill> skills = ModUI.GetPlayerAttackSkills();
			int totalRows = (skills.Count + 1 + 1) / 2;
			int contentH = totalRows * 18 + 4;
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
		// Hàng 0: Trạng thái & Tiếp cận
		int row0Y = uiY + 4;
		int row0H = 17;
		mFont.tahoma_7_grey.drawString(g, "Trạng thái:", uiX + 5, row0Y + 3, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 58, row0Y, 42, row0H, ModTanSat.autoTanSat ? "BẬT" : "TẮT", ModTanSat.autoTanSat, g);

		int tpBtnW = 72;
		int tpBtnX = uiX + uiW - tpBtnW - 4;
		mFont.tahoma_7_grey.drawString(g, "Tiếp cận:", tpBtnX - 46, row0Y + 3, mFont.LEFT);
		ModUI.PaintNativeButton(tpBtnX, row0Y, tpBtnW, row0H, ModTanSat.useTeleport ? "Dịch chuyển" : "Chạy bộ", ModTanSat.useTeleport, g);

		// Hàng 1: 2 Sub-Tabs (1. Chọn Quái / 2. Chọn Kỹ Năng)
		int row1Y = row0Y + row0H + 3;
		int row1H = 18;
		int subTabW = (uiW - 12) / 2;
		ModUI.PaintNativeButton(uiX + 4, row1Y, subTabW, row1H, "1. Chọn Quái", ModUI.tanSatTab == 0, g);
		ModUI.PaintNativeButton(uiX + 8 + subTabW, row1Y, subTabW, row1H, "2. Chọn Kỹ Năng", ModUI.tanSatTab == 1, g);

		// Hàng 2: Tiêu đề danh sách & Nút Chọn tất cả / Bỏ chọn hết
		int row2Y = row1Y + row1H + 3;
		int row2H = 17;
		int allBtnW = 74;
		int allBtnX = uiX + uiW - allBtnW - 4;

		// Hàng 4 (Đáy): Thiết lập Time Attack
		int footerH = 18;
		int footerY = uiY + uiH - footerH - 4;

		// Hàng 3 (Giữa): Khung danh sách Quái / Skill co dãn động 100%
		int listX = uiX + 4;
		int listY = row2Y + row2H + 3;
		int listW = uiW - 8;
		int listH = (footerY - 4) - listY;
		if (listH < 40) listH = 40;
		int colW = (listW - 10) / 2;

		if (ModUI.tanSatTab == 0)
		{
			mFont.tahoma_7_grey.drawString(g, "Quái map (Tick để đánh):", uiX + 5, row2Y + 3, mFont.LEFT);
			ModUI.PaintNativeButton(allBtnX, row2Y, allBtnW, row2H, ModTanSat.selectAllMobs ? "Bỏ chọn hết" : "Chọn tất cả", ModTanSat.selectAllMobs, g);

			GameCanvas.paintz.paintFrameSimple(listX, listY, listW, listH, g);
			g.setColor(15196114);
			g.fillRect(listX + 2, listY + 2, listW - 4, listH - 4);

			List<int> mobIds = ModUI.GetUniqueMobTemplateIds();
			int totalRows = (mobIds.Count + 1 + 1) / 2;
			int contentH = totalRows * 18 + 4;
			int maxScroll = (contentH > listH - 4) ? (contentH - (listH - 4)) : 0;
			if (scrollMobY > maxScroll) scrollMobY = maxScroll;
			if (scrollMobY < 0) scrollMobY = 0;

			if (mobIds.Count == 0)
			{
				mFont.tahoma_7_grey.drawString(g, "(Chưa thấy quái nào trong map)", listX + listW / 2, listY + listH / 2 - 4, mFont.CENTER);
			}
			else
			{
				g.setClip(listX + 2, listY + 2, listW - 4, listH - 4);
				for (int idx = 0; idx <= mobIds.Count; idx++)
				{
					int col = idx % 2;
					int row = idx / 2;
					int itemX = listX + 4 + col * (colW + 4);
					int itemY = listY + 3 + row * 18 - scrollMobY;

					if (itemY + 18 < listY || itemY > listY + listH) continue;

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
			mFont.tahoma_7_grey.drawString(g, "Kỹ năng nhân vật (Tick để dùng):", uiX + 5, row2Y + 3, mFont.LEFT);
			ModUI.PaintNativeButton(allBtnX, row2Y, allBtnW, row2H, ModTanSat.selectAllSkills ? "Tự động chiêu" : "Tất cả chiêu", ModTanSat.selectAllSkills, g);

			GameCanvas.paintz.paintFrameSimple(listX, listY, listW, listH, g);
			g.setColor(15196114);
			g.fillRect(listX + 2, listY + 2, listW - 4, listH - 4);

			List<Skill> skills = ModUI.GetPlayerAttackSkills();
			int totalRows = (skills.Count + 1 + 1) / 2;
			int contentH = totalRows * 18 + 4;
			int maxScroll = (contentH > listH - 4) ? (contentH - (listH - 4)) : 0;
			if (scrollSkillY > maxScroll) scrollSkillY = maxScroll;
			if (scrollSkillY < 0) scrollSkillY = 0;

			if (skills.Count == 0)
			{
				mFont.tahoma_7_grey.drawString(g, "(Nhân vật chưa học kỹ năng nào)", listX + listW / 2, listY + listH / 2 - 4, mFont.CENTER);
			}
			else
			{
				g.setClip(listX + 2, listY + 2, listW - 4, listH - 4);
				for (int idx = 0; idx <= skills.Count; idx++)
				{
					int col = idx % 2;
					int row = idx / 2;
					int itemX = listX + 4 + col * (colW + 4);
					int itemY = listY + 3 + row * 18 - scrollSkillY;

					if (itemY + 18 < listY || itemY > listY + listH) continue;

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

		// Vẽ Hàng 4: Time Attack
		mFont.tahoma_7_grey.drawString(g, "Time attack:", uiX + 5, footerY + 3, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 65, footerY, 16, footerH, "-", false, g);
		ModUI.PaintNativeButton(uiX + 83, footerY, 46, footerH, ModTanSat.timeAttack + "ms", true, g);
		ModUI.PaintNativeButton(uiX + 131, footerY, 16, footerH, "+", false, g);

		// Các preset nhanh
		ModUI.PaintNativeButton(uiX + 151, footerY, 26, footerH, "100", ModTanSat.timeAttack == 100, g);
		ModUI.PaintNativeButton(uiX + 180, footerY, 26, footerH, "200", ModTanSat.timeAttack == 200, g);
		ModUI.PaintNativeButton(uiX + 209, footerY, 26, footerH, "300", ModTanSat.timeAttack == 300, g);
		ModUI.PaintNativeButton(uiX + 238, footerY, 26, footerH, "500", ModTanSat.timeAttack == 500, g);
	}

	public static bool HandleTap(int px, int py, int uiX, int uiY, int uiW, int uiH)
	{
		if (hasDragged)
		{
			hasDragged = false;
			return true;
		}

		int row0Y = uiY + 4;
		int row0H = 17;
		int row1Y = row0Y + row0H + 3;
		int row1H = 18;
		int row2Y = row1Y + row1H + 3;
		int row2H = 17;
		int footerH = 18;
		int footerY = uiY + uiH - footerH - 4;

		// 1. Bật/Tắt Tàn sát
		if (px >= uiX + 58 && px <= uiX + 100 && py >= row0Y - 1 && py <= row0Y + row0H + 1)
		{
			ModTanSat.autoTanSat = !ModTanSat.autoTanSat;
			ModConfig.SaveConfig();
			SoundMn.gI().buttonClick();
			return true;
		}

		// 2. Nút Tiếp cận
		int tpBtnW = 72;
		int tpBtnX = uiX + uiW - tpBtnW - 4;
		if (px >= tpBtnX && px <= tpBtnX + tpBtnW && py >= row0Y - 1 && py <= row0Y + row0H + 1)
		{
			ModTanSat.useTeleport = !ModTanSat.useTeleport;
			ModConfig.SaveConfig();
			SoundMn.gI().buttonClick();
			return true;
		}

		// 3. Sub tabs (1. Chọn Quái / 2. Chọn Kỹ Năng)
		int subTabW = (uiW - 12) / 2;
		if (py >= row1Y - 1 && py <= row1Y + row1H + 1)
		{
			if (px >= uiX + 4 && px <= uiX + 4 + subTabW)
			{
				ModUI.tanSatTab = 0;
				SoundMn.gI().buttonClick();
				return true;
			}
			if (px >= uiX + 8 + subTabW && px <= uiX + 8 + subTabW * 2)
			{
				ModUI.tanSatTab = 1;
				SoundMn.gI().buttonClick();
				return true;
			}
		}

		// 4. Nút Chọn tất cả / Bỏ chọn hết
		int allBtnW = 74;
		int allBtnX = uiX + uiW - allBtnW - 4;
		if (px >= allBtnX && px <= allBtnX + allBtnW && py >= row2Y - 1 && py <= row2Y + row2H + 1)
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

		int listX = uiX + 4;
		int listY = row2Y + row2H + 3;
		int listW = uiW - 8;
		int listH = (footerY - 4) - listY;
		if (listH < 40) listH = 40;
		int colW = (listW - 10) / 2;

		// 5. Checklist quái / skill trong khung cuộn
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
					int itemY = listY + 3 + row * 18 - scrollMobY;

					if (itemY + 18 < listY || itemY > listY + listH) continue;

					if (px >= itemX && px <= itemX + colW && py >= itemY && py <= itemY + 18)
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
					int itemY = listY + 3 + row * 18 - scrollSkillY;

					if (itemY + 18 < listY || itemY > listY + listH) continue;

					if (px >= itemX && px <= itemX + colW && py >= itemY && py <= itemY + 18)
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

		// 6. Điều chỉnh Time Attack ở Footer
		if (py >= footerY - 1 && py <= footerY + footerH + 1)
		{
			// Nút [-]
			if (px >= uiX + 65 && px <= uiX + 81)
			{
				ModTanSat.timeAttack -= 50;
				if (ModTanSat.timeAttack < 50) ModTanSat.timeAttack = 50;
				ModConfig.SaveConfig();
				SoundMn.gI().buttonClick();
				return true;
			}
			// Nút current [ xxx ms ]
			if (px >= uiX + 83 && px <= uiX + 129)
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
			if (px >= uiX + 131 && px <= uiX + 147)
			{
				ModTanSat.timeAttack += 50;
				if (ModTanSat.timeAttack > 3000) ModTanSat.timeAttack = 3000;
				ModConfig.SaveConfig();
				SoundMn.gI().buttonClick();
				return true;
			}
			// Preset 100
			if (px >= uiX + 151 && px <= uiX + 177)
			{
				ModTanSat.timeAttack = 100;
				ModConfig.SaveConfig();
				SoundMn.gI().buttonClick();
				return true;
			}
			// Preset 200
			if (px >= uiX + 180 && px <= uiX + 206)
			{
				ModTanSat.timeAttack = 200;
				ModConfig.SaveConfig();
				SoundMn.gI().buttonClick();
				return true;
			}
			// Preset 300
			if (px >= uiX + 209 && px <= uiX + 235)
			{
				ModTanSat.timeAttack = 300;
				ModConfig.SaveConfig();
				SoundMn.gI().buttonClick();
				return true;
			}
			// Preset 500
			if (px >= uiX + 238 && px <= uiX + 264)
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
