using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// CLASS ModUITanSat: Giao diện điều khiển Tàn Sát Pro với thanh chỉ định kỹ năng trực tiếp và bộ lọc mục tiêu.
/// </summary>
public static class ModUITanSat
{
	public static int scrollSkillY = 0;
	public static int scrollMobY = 0;

	private static bool isDragging = false;
	private static bool hasDragged = false;
	private static int startDragY = 0;
	private static int lastDragY = 0;

	private static string GetCompactSkillButtonLabel(Skill s, int index)
	{
		if (s == null || s.template == null) return "Ô" + (index + 1);
		int slot = ModTanSatFilter.GetSkillHotbarSlot(s.template.id);
		string prefix = (slot >= 0) ? (((slot == 9) ? 0 : (slot + 1)) + ":") : "";
		string raw = s.template.name ?? ("ID" + s.template.id);
		if (raw.StartsWith("Chiêu ")) raw = raw.Substring(6).Trim();
		if (raw.StartsWith("Đấm ")) raw = raw.Substring(4).Trim();
		if (raw.Length > 6) raw = raw.Substring(0, 6);
		return prefix + raw;
	}

	private static void ComputeLayout(
		int uiX,
		int uiY,
		int uiW,
		int uiH,
		out int row0Y,
		out int modeRowY,
		out int skillRowY,
		out int row1Y,
		out int row2Y,
		out int footerY,
		out int listX,
		out int listY,
		out int listW,
		out int listH)
	{
		row0Y = uiY + 3;
		modeRowY = row0Y + 18;
		skillRowY = modeRowY + 18;
		row1Y = skillRowY + 18;
		row2Y = row1Y + 19;
		footerY = uiY + uiH - 20;
		listX = uiX + 4;
		listY = row2Y + 18;
		listW = uiW - 8;
		listH = (footerY - 3) - listY;
		if (listH < 36)
		{
			listH = 36;
		}
	}

	private static int GetCurrentInnerListHeight()
	{
		int safeTop = SafeArea.Top;
		int safeBottom = SafeArea.Bottom;
		int maxH = GameCanvas.h - 12 - safeTop - safeBottom;
		int outerH = (maxH > 260) ? 260 : maxH;
		if (outerH < 200)
		{
			outerH = GameCanvas.h - safeTop - safeBottom;
		}
		int detailH = outerH - 30;
		ComputeLayout(0, 0, 353, detailH, out _, out _, out _, out _, out _, out _, out _, out _, out _, out int listH);
		return System.Math.Max(32, listH - 4);
	}

	/// <summary>
	/// Bộ xử lý sự kiện tiếp nhận và phản hồi OnMouseScroll.
	/// </summary>
	public static void OnMouseScroll(float wheel)
	{
		int step = 22;
		int innerH = GetCurrentInnerListHeight();
		if (ModUI.tanSatTab == 0)
		{
			List<int> mobIds = ModUI.GetUniqueMobTemplateIds();
			int totalRows = (mobIds.Count + 8 + 1) / 2;
			int contentH = totalRows * 18 + 4;
			int maxScroll = (contentH > innerH) ? (contentH - innerH) : 0;
			if (maxScroll <= 0) return;

			if (wheel > 0) { scrollMobY -= step; if (scrollMobY < 0) scrollMobY = 0; }
			else if (wheel < 0) { scrollMobY += step; if (scrollMobY > maxScroll) scrollMobY = maxScroll; }
		}
		else
		{
			List<Skill> skills = ModUI.GetPlayerAttackSkills();
			int totalRows = (skills.Count + 4 + 1) / 2;
			int contentH = totalRows * 18 + 4;
			int maxScroll = (contentH > innerH) ? (contentH - innerH) : 0;
			if (maxScroll <= 0) return;

			if (wheel > 0) { scrollSkillY -= step; if (scrollSkillY < 0) scrollSkillY = 0; }
			else if (wheel < 0) { scrollSkillY += step; if (scrollSkillY > maxScroll) scrollSkillY = maxScroll; }
		}
	}

	/// <summary>
	/// Cập nhật trạng thái logic của UpdateDragScroll theo chu kỳ khung hình.
	/// </summary>
	public static void UpdateDragScroll(int px, int py, int uiX, int uiY, int uiW, int uiH)
	{
		ComputeLayout(uiX, uiY, uiW, uiH, out _, out _, out _, out _, out _, out _, out int listX, out int listY, out int listW, out int listH);

		int maxScroll = 0;
		if (ModUI.tanSatTab == 0)
		{
			List<int> mobIds = ModUI.GetUniqueMobTemplateIds();
			int totalRows = (mobIds.Count + 8 + 1) / 2;
			int contentH = totalRows * 18 + 4;
			maxScroll = (contentH > listH - 4) ? (contentH - (listH - 4)) : 0;
		}
		else
		{
			List<Skill> skills = ModUI.GetPlayerAttackSkills();
			int totalRows = (skills.Count + 4 + 1) / 2;
			int contentH = totalRows * 18 + 4;
			maxScroll = (contentH > listH - 4) ? (contentH - (listH - 4)) : 0;
		}

		bool isDown = GameCanvas.isPointerDown;
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

	/// <summary>
	/// Thực hiện vẽ toàn bộ đối tượng và các lớp hiển thị lên canvas mGraphics.
	/// </summary>
	public static void Paint(int uiX, int uiY, int uiW, int uiH, mGraphics g)
	{
		ModTanSatFilter.EnsureDesignatedSkill();

		ComputeLayout(
			uiX,
			uiY,
			uiW,
			uiH,
			out int row0Y,
			out int modeRowY,
			out int skillRowY,
			out int row1Y,
			out int row2Y,
			out int footerY,
			out int listX,
			out int listY,
			out int listW,
			out int listH);

		// Hàng 0: Tàn sát, Tự nhặt & Tiếp cận
		int row0H = 16;
		mFont.tahoma_7_grey.drawString(g, "Tàn sát:", uiX + 5, row0Y + 2, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 46, row0Y, 36, row0H, ModTanSat.autoTanSat ? "BẬT" : "TẮT", ModTanSat.autoTanSat, g);

		mFont.tahoma_7_grey.drawString(g, "Tự nhặt:", uiX + 87, row0Y + 2, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 128, row0Y, 36, row0H, ModAutoPick.autoPick ? "BẬT" : "TẮT", ModAutoPick.autoPick, g);

		int tpBtnW = 72;
		int tpBtnX = uiX + uiW - tpBtnW - 4;
		string tpLabel = !ModTanSat.useTeleport
			? "Chạy bộ"
			: (ModTanSat.antiScanMode ? "Lướt BFS" : "Dịch chuyển");
		mFont.tahoma_7_grey.drawString(g, "Tiếp cận:", tpBtnX - 44, row0Y + 2, mFont.LEFT);
		ModUI.PaintNativeButton(tpBtnX, row0Y, tpBtnW, row0H, tpLabel, ModTanSat.useTeleport, g);

		// Hàng Chế độ úp: Chuyển đổi giữa Map Thường và Chống Quét (dành cho các map dễ bị quét đưa về nhà)
		int modeRowH = 16;
		mFont.tahoma_7_grey.drawString(g, "Chế độ úp:", uiX + 5, modeRowY + 2, mFont.LEFT);
		int modeStartX = uiX + 62;
		int modeAreaW = uiW - 66;
		int normalBtnW = (modeAreaW - 4) * 42 / 100;
		int antiBtnX = modeStartX + normalBtnW + 4;
		int antiBtnW = (modeAreaW - 4) - normalBtnW;
		ModUI.PaintNativeButton(modeStartX, modeRowY, normalBtnW, modeRowH, "Map Thường", !ModTanSat.antiScanMode, g);
		ModUI.PaintNativeButton(antiBtnX, modeRowY, antiBtnW, modeRowH, "Chống Quét Về Nhà", ModTanSat.antiScanMode, g);

		// Hàng Chiêu TS (Luôn hiển thị): Thanh chỉ định nhanh Skill sử dụng khi Tàn Sát
		int skillRowH = 16;
		mFont.tahoma_7_grey.drawString(g, "Chiêu TS:", uiX + 5, skillRowY + 2, mFont.LEFT);

		List<Skill> quickSkills = ModUI.GetPlayerAttackSkills();
		int quickStartX = uiX + 52;
		int quickAreaW = uiW - 56;
		if (quickSkills.Count == 0)
		{
			mFont.tahoma_7_grey.drawString(g, "(Chưa có kỹ năng)", quickStartX + 4, skillRowY + 2, mFont.LEFT);
		}
		else
		{
			int maxShow = (quickSkills.Count > 5) ? 5 : quickSkills.Count;
			int gap = 2;
			int btnW = (quickAreaW - gap * (maxShow - 1)) / maxShow;
			if (btnW < 34) btnW = 34;
			for (int i = 0; i < maxShow; i++)
			{
				Skill qSk = quickSkills[i];
				int qx = quickStartX + i * (btnW + gap);
				bool isActive = (qSk != null && qSk.template != null) &&
				                (ModTanSatFilter.selectAllSkills
				                 ? ModTanSatFilter.IsAutoAttackSkill(qSk)
				                 : ModTanSatFilter.IsSkillTicked(qSk.template.id));
				string qLabel = GetCompactSkillButtonLabel(qSk, i);
				ModUI.PaintNativeButton(qx, skillRowY, btnW, skillRowH, qLabel, isActive, g);
			}
		}

		// Hàng Sub-Tabs (1. Chọn Quái / 2. Chọn Kỹ Năng)
		int row1H = 17;
		int subTabW = (uiW - 12) / 2;
		ModUI.PaintNativeButton(uiX + 4, row1Y, subTabW, row1H, "1. Chọn Quái", ModUI.tanSatTab == 0, g);
		ModUI.PaintNativeButton(uiX + 8 + subTabW, row1Y, subTabW, row1H, "2. Chọn Kỹ Năng", ModUI.tanSatTab == 1, g);

		// Hàng Tiêu đề danh sách & Chế độ
		int row2H = 16;
		int allBtnW = 74;
		int allBtnX = uiX + uiW - allBtnW - 4;

		// Hàng Đáy: Thiết lập Time Attack
		int footerH = 17;
		int colW = (listW - 10) / 2;

		if (ModUI.tanSatTab == 0)
		{
			mFont.tahoma_7_grey.drawString(g, "Quái map (Tick để đánh):", uiX + 5, row2Y + 2, mFont.LEFT);
			ModUI.PaintNativeButton(allBtnX, row2Y, allBtnW, row2H, ModTanSat.selectAllMobs ? "Bỏ chọn hết" : "Chọn tất cả", ModTanSat.selectAllMobs, g);

			GameCanvas.paintz.paintFrameSimple(listX, listY, listW, listH, g);
			g.setColor(15196114);
			g.fillRect(listX + 2, listY + 2, listW - 4, listH - 4);

			List<int> mobIds = ModUI.GetUniqueMobTemplateIds();
			int totalItems = mobIds.Count + 8;
			int totalRows = (totalItems + 1) / 2;
			int contentH = totalRows * 18 + 4;
			int maxScroll = (contentH > listH - 4) ? (contentH - (listH - 4)) : 0;
			if (scrollMobY > maxScroll) scrollMobY = maxScroll;
			if (scrollMobY < 0) scrollMobY = 0;

			g.setClip(listX + 2, listY + 2, listW - 4, listH - 4);
			for (int idx = 0; idx < totalItems; idx++)
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
				else if (idx == 1)
				{
					ModUI.DrawCheckbox(itemX, itemY + 1, ModTanSat.avoidSuperMob, g);
					(ModTanSat.avoidSuperMob ? mFont.tahoma_7_green2 : mFont.tahoma_7_grey).drawString(g, "Né Siêu Quái", itemX + 22, itemY + 2, mFont.LEFT);
				}
				else if (idx == 2)
				{
					ModUI.DrawCheckbox(itemX, itemY + 1, ModTanSat.autoCheckStamina, g);
					(ModTanSat.autoCheckStamina ? mFont.tahoma_7_green2 : mFont.tahoma_7_grey).drawString(g, "Tự ăn Nho (TL)", itemX + 22, itemY + 2, mFont.LEFT);
				}
				else if (idx == 3)
				{
					ModUI.DrawCheckbox(itemX, itemY + 1, ModTanSat.autoChangeZone, g);
					(ModTanSat.autoChangeZone ? mFont.tahoma_7_green2 : mFont.tahoma_7_grey).drawString(g, "Tự đổi khu KS", itemX + 22, itemY + 2, mFont.LEFT);
				}
				else if (idx == 4)
				{
					ModUI.DrawCheckbox(itemX, itemY + 1, ModTanSat.checkLagMob, g);
					(ModTanSat.checkLagMob ? mFont.tahoma_7_green2 : mFont.tahoma_7_grey).drawString(g, "Check Lag Quái", itemX + 22, itemY + 2, mFont.LEFT);
				}
				else if (idx == 5)
				{
					ModUI.DrawCheckbox(itemX, itemY + 1, ModTanSat.autoResetZone, g);
					string rzLabel = "Reset Khu (" + ModTanSat.resetZoneMinutes + "p)";
					(ModTanSat.autoResetZone ? mFont.tahoma_7_green2 : mFont.tahoma_7_grey).drawString(g, rzLabel, itemX + 22, itemY + 2, mFont.LEFT);
				}
				else if (idx == 6)
				{
					ModUI.DrawCheckbox(itemX, itemY + 1, ModTanSat.showGoldTrain, g);
					(ModTanSat.showGoldTrain ? mFont.tahoma_7_green2 : mFont.tahoma_7_grey).drawString(g, "HUD Vàng/Giờ", itemX + 22, itemY + 2, mFont.LEFT);
				}
				else if (idx == 7)
				{
					ModUI.DrawCheckbox(itemX, itemY + 1, ModTanSat.trainPets, g);
					(ModTanSat.trainPets ? mFont.tahoma_7_green2 : mFont.tahoma_7_grey).drawString(g, "Chế độ Úp Đệ", itemX + 22, itemY + 2, mFont.LEFT);
				}
				else
				{
					int tId = mobIds[idx - 8];
					MobTemplate t = Mob.arrMobTemplate[tId];
					string mobName = (t != null) ? t.name : ("Quái #" + tId);
					bool isTicked = ModTanSat.IsMobTicked(tId);
					ModUI.DrawCheckbox(itemX, itemY + 1, isTicked, g);
					(isTicked ? mFont.tahoma_7_green2 : mFont.tahoma_7_grey).drawString(g, mobName, itemX + 22, itemY + 2, mFont.LEFT);
				}
			}
			g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
		}
		else
		{
			mFont.tahoma_7_grey.drawString(g, "Chế độ chiêu:", uiX + 5, row2Y + 2, mFont.LEFT);

			int modeAllW = 54;
			int modeAllX = uiX + uiW - modeAllW - 4;
			int modeComboW = 62;
			int modeComboX = modeAllX - modeComboW - 3;
			int modeSingleW = 70;
			int modeSingleX = modeComboX - modeSingleW - 3;

			int curMode = ModTanSatFilter.selectAllSkills ? 2 : ModTanSatFilter.skillSelectionMode;
			ModUI.PaintNativeButton(modeSingleX, row2Y, modeSingleW, row2H, "Chỉ 1 chiêu", curMode == 0, g);
			ModUI.PaintNativeButton(modeComboX, row2Y, modeComboW, row2H, "Kết hợp", curMode == 1, g);
			ModUI.PaintNativeButton(modeAllX, row2Y, modeAllW, row2H, "Tất cả", curMode == 2, g);

			GameCanvas.paintz.paintFrameSimple(listX, listY, listW, listH, g);
			g.setColor(15196114);
			g.fillRect(listX + 2, listY + 2, listW - 4, listH - 4);

			List<Skill> skills = ModUI.GetPlayerAttackSkills();
			int totalSkillItems = skills.Count + 4;
			int totalRows = (totalSkillItems + 1) / 2;
			int contentH = totalRows * 18 + 4;
			int maxScroll = (contentH > listH - 4) ? (contentH - (listH - 4)) : 0;
			if (scrollSkillY > maxScroll) scrollSkillY = maxScroll;
			if (scrollSkillY < 0) scrollSkillY = 0;

			g.setClip(listX + 2, listY + 2, listW - 4, listH - 4);
			for (int idx = 0; idx < totalSkillItems; idx++)
			{
				int col = idx % 2;
				int row = idx / 2;
				int itemX = listX + 4 + col * (colW + 4);
				int itemY = listY + 3 + row * 18 - scrollSkillY;

				if (itemY + 18 < listY || itemY > listY + listH) continue;

				if (idx == 0)
				{
					ModUI.DrawCheckbox(itemX, itemY + 1, ModTanSat.selectAllSkills, g);
					(ModTanSat.selectAllSkills ? mFont.tahoma_7_green2 : mFont.tahoma_7_grey).drawString(g, "Tất cả chiêu đánh", itemX + 22, itemY + 2, mFont.LEFT);
				}
				else if (idx == 1)
				{
					ModUI.DrawCheckbox(itemX, itemY + 1, ModTanSat.autoSupportSkills, g);
					(ModTanSat.autoSupportSkills ? mFont.tahoma_7_green2 : mFont.tahoma_7_grey).drawString(g, "Auto Buff Hỗ Trợ", itemX + 22, itemY + 2, mFont.LEFT);
				}
				else if (idx == 2)
				{
					ModUI.DrawCheckbox(itemX, itemY + 1, ModTanSat.freezeSkillCooldown, g);
					(ModTanSat.freezeSkillCooldown ? mFont.tahoma_7_green2 : mFont.tahoma_7_grey).drawString(g, "Đóng băng Hồi Chiêu", itemX + 22, itemY + 2, mFont.LEFT);
				}
				else if (idx == 3)
				{
					ModUI.DrawCheckbox(itemX, itemY + 1, ModTanSat.autoChangeFocus, g);
					(ModTanSat.autoChangeFocus ? mFont.tahoma_7_green2 : mFont.tahoma_7_grey).drawString(g, "Đổi Mục Tiêu", itemX + 22, itemY + 2, mFont.LEFT);
				}
				else
				{
					Skill sk = skills[idx - 4];
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

		// Vẽ Hàng 5: Time Attack
		mFont.tahoma_7_grey.drawString(g, "Time attack:", uiX + 5, footerY + 2, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 68, footerY, 16, footerH, "-", false, g);
		ModUI.PaintNativeButton(uiX + 86, footerY, 48, footerH, ModTanSat.timeAttack + "ms", true, g);
		ModUI.PaintNativeButton(uiX + 136, footerY, 16, footerH, "+", false, g);

		// Các preset nhanh
		int presetStartX = uiX + 155;
		int presetW = (uiW - 159 - 9) / 4;
		if (presetW < 24) presetW = 24;
		ModUI.PaintNativeButton(presetStartX, footerY, presetW, footerH, "100", ModTanSat.timeAttack == 100, g);
		ModUI.PaintNativeButton(presetStartX + presetW + 3, footerY, presetW, footerH, "200", ModTanSat.timeAttack == 200, g);
		ModUI.PaintNativeButton(presetStartX + (presetW + 3) * 2, footerY, presetW, footerH, "300", ModTanSat.timeAttack == 300, g);
		ModUI.PaintNativeButton(presetStartX + (presetW + 3) * 3, footerY, presetW, footerH, "500", ModTanSat.timeAttack == 500, g);
	}

	/// <summary>
	/// Xử lý nghiệp vụ logic chuyên trách cho HandleTap.
	/// </summary>
	public static bool HandleTap(int px, int py, int uiX, int uiY, int uiW, int uiH)
	{
		if (hasDragged)
		{
			hasDragged = false;
			return true;
		}

		ComputeLayout(
			uiX,
			uiY,
			uiW,
			uiH,
			out int row0Y,
			out int modeRowY,
			out int skillRowY,
			out int row1Y,
			out int row2Y,
			out int footerY,
			out int listX,
			out int listY,
			out int listW,
			out int listH);

		int row0H = 16;
		int modeRowH = 16;
		int skillRowH = 16;
		int row1H = 17;
		int row2H = 16;
		int footerH = 17;

		// 1. Bật/Tắt Tàn sát
		if (px >= uiX + 46 && px <= uiX + 82 && py >= row0Y - 1 && py <= row0Y + row0H + 1)
		{
			ModTanSat.autoTanSat = !ModTanSat.autoTanSat;
			ModConfig.SaveConfig();
			SoundMn.gI().buttonClick();
			return true;
		}

		// 1b. Bật/Tắt Tự nhặt item
		if (px >= uiX + 128 && px <= uiX + 164 && py >= row0Y - 1 && py <= row0Y + row0H + 1)
		{
			ModAutoPick.autoPick = !ModAutoPick.autoPick;
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

		// 2a. Hàng Chế độ úp: Map Thường vs Chống Quét Về Nhà
		if (py >= modeRowY - 1 && py <= modeRowY + modeRowH + 1)
		{
			int modeStartX = uiX + 62;
			int modeAreaW = uiW - 66;
			int normalBtnW = (modeAreaW - 4) * 42 / 100;
			int antiBtnX = modeStartX + normalBtnW + 4;
			int antiBtnW = (modeAreaW - 4) - normalBtnW;

			if (px >= modeStartX && px <= modeStartX + normalBtnW)
			{
				ModTanSat.SetAntiScanMode(false);
				ModConfig.SaveConfig();
				GameScr.info1?.addInfo("Chế độ Tàn Sát: Map Thường (Dịch chuyển nhanh)", 0);
				SoundMn.gI().buttonClick();
				return true;
			}
			if (px >= antiBtnX && px <= antiBtnX + antiBtnW)
			{
				ModTanSat.SetAntiScanMode(true);
				ModConfig.SaveConfig();
				GameScr.info1?.addInfo("Chế độ Tàn Sát: Chống Quét Về Nhà (Úp map dễ bị quét)", 0);
				SoundMn.gI().buttonClick();
				return true;
			}
		}

		// 2b. Thanh chỉ định nhanh Skill Tàn Sát (Hàng 1)
		if (py >= skillRowY - 1 && py <= skillRowY + skillRowH + 1)
		{
			List<Skill> quickSkills = ModUI.GetPlayerAttackSkills();
			if (quickSkills.Count > 0)
			{
				int quickStartX = uiX + 52;
				int quickAreaW = uiW - 56;
				int maxShow = (quickSkills.Count > 5) ? 5 : quickSkills.Count;
				int gap = 2;
				int btnW = (quickAreaW - gap * (maxShow - 1)) / maxShow;
				if (btnW < 34) btnW = 34;

				for (int i = 0; i < maxShow; i++)
				{
					int qx = quickStartX + i * (btnW + gap);
					if (px >= qx && px <= qx + btnW)
					{
						Skill qSk = quickSkills[i];
						if (qSk != null && qSk.template != null)
						{
							if (ModTanSatFilter.skillSelectionMode == 1 && !ModTanSatFilter.selectAllSkills)
							{
								ModTanSatFilter.ToggleSkillTicked(qSk.template.id);
							}
							else
							{
								ModTanSatFilter.DesignateSingleSkill(qSk.template.id);
								GameScr.info1?.addInfo("Chỉ định chiêu Tàn Sát: " + qSk.template.name, 0);
							}
							SoundMn.gI().buttonClick();
							return true;
						}
					}
				}
			}
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

		// 4. Nút Chọn tất cả / Chế độ kỹ năng
		if (ModUI.tanSatTab == 0)
		{
			int allBtnW = 74;
			int allBtnX = uiX + uiW - allBtnW - 4;
			if (px >= allBtnX && px <= allBtnX + allBtnW && py >= row2Y - 1 && py <= row2Y + row2H + 1)
			{
				ModTanSat.ToggleSelectAllMobs();
				ModConfig.SaveConfig();
				SoundMn.gI().buttonClick();
				return true;
			}
		}
		else if (py >= row2Y - 1 && py <= row2Y + row2H + 1)
		{
			int modeAllW = 54;
			int modeAllX = uiX + uiW - modeAllW - 4;
			int modeComboW = 62;
			int modeComboX = modeAllX - modeComboW - 3;
			int modeSingleW = 70;
			int modeSingleX = modeComboX - modeSingleW - 3;

			if (px >= modeSingleX && px <= modeSingleX + modeSingleW)
			{
				ModTanSatFilter.SetSkillSelectionMode(0);
				GameScr.info1?.addInfo("Chế độ Tàn Sát: Chỉ định 1 chiêu cố định", 0);
				SoundMn.gI().buttonClick();
				return true;
			}
			if (px >= modeComboX && px <= modeComboX + modeComboW)
			{
				ModTanSatFilter.SetSkillSelectionMode(1);
				GameScr.info1?.addInfo("Chế độ Tàn Sát: Kết hợp các chiêu đã tick", 0);
				SoundMn.gI().buttonClick();
				return true;
			}
			if (px >= modeAllX && px <= modeAllX + modeAllW)
			{
				ModTanSatFilter.SetSkillSelectionMode(2);
				GameScr.info1?.addInfo("Chế độ Tàn Sát: Tất cả chiêu đánh cơ bản", 0);
				SoundMn.gI().buttonClick();
				return true;
			}
		}

		int colW = (listW - 10) / 2;

		// 5. Checklist quái / skill trong khung cuộn
		if (px >= listX && px <= listX + listW && py >= listY && py <= listY + listH)
		{
			if (ModUI.tanSatTab == 0)
			{
				List<int> mobIds = ModUI.GetUniqueMobTemplateIds();
				int totalItems = mobIds.Count + 8;
				for (int idx = 0; idx < totalItems; idx++)
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
						else if (idx == 1)
						{
							ModTanSat.avoidSuperMob = !ModTanSat.avoidSuperMob;
							ModConfig.SaveConfig();
						}
						else if (idx == 2)
						{
							ModTanSat.autoCheckStamina = !ModTanSat.autoCheckStamina;
							ModConfig.SaveConfig();
						}
						else if (idx == 3)
						{
							ModTanSat.autoChangeZone = !ModTanSat.autoChangeZone;
							ModConfig.SaveConfig();
						}
						else if (idx == 4)
						{
							ModTanSat.checkLagMob = !ModTanSat.checkLagMob;
							ModConfig.SaveConfig();
							GameScr.info1?.addInfo("Check Lag Quái (15s >=90% HP): " + (ModTanSat.checkLagMob ? "BẬT" : "TẮT"), 0);
						}
						else if (idx == 5)
						{
							if (!ModTanSat.autoResetZone)
							{
								ModTanSat.autoResetZone = true;
								ModTanSat.resetZoneMinutes = 15;
							}
							else if (ModTanSat.resetZoneMinutes == 15) ModTanSat.resetZoneMinutes = 30;
							else if (ModTanSat.resetZoneMinutes == 30) ModTanSat.resetZoneMinutes = 60;
							else ModTanSat.autoResetZone = false;
							ModConfig.SaveConfig();
							GameScr.info1?.addInfo("Tự Reset Khu: " + (ModTanSat.autoResetZone ? (ModTanSat.resetZoneMinutes + " phút") : "TẮT"), 0);
						}
						else if (idx == 6)
						{
							ModTanSat.showGoldTrain = !ModTanSat.showGoldTrain;
							if (ModTanSat.showGoldTrain) ModTanSat.ResetGoldTrain();
							ModConfig.SaveConfig();
							GameScr.info1?.addInfo("HUD Vàng/Giờ: " + (ModTanSat.showGoldTrain ? "BẬT" : "TẮT"), 0);
						}
						else if (idx == 7)
						{
							ModTanSat.trainPets = !ModTanSat.trainPets;
							ModConfig.SaveConfig();
							GameScr.info1?.addInfo("Chế độ Úp Đệ Tử: " + (ModTanSat.trainPets ? "BẬT" : "TẮT"), 0);
						}
						else
						{
							ModTanSat.ToggleMobTicked(mobIds[idx - 8]);
						}
						SoundMn.gI().buttonClick();
						return true;
					}
				}
			}
			else
			{
				List<Skill> skills = ModUI.GetPlayerAttackSkills();
				int totalSkillItems = skills.Count + 4;
				for (int idx = 0; idx < totalSkillItems; idx++)
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
						else if (idx == 1)
						{
							ModTanSat.autoSupportSkills = !ModTanSat.autoSupportSkills;
							ModConfig.SaveConfig();
						}
						else if (idx == 2)
						{
							ModTanSat.freezeSkillCooldown = !ModTanSat.freezeSkillCooldown;
							ModConfig.SaveConfig();
						}
						else if (idx == 3)
						{
							ModTanSat.autoChangeFocus = !ModTanSat.autoChangeFocus;
							ModConfig.SaveConfig();
							GameScr.info1?.addInfo("Đổi Mục Tiêu Liên Tục: " + (ModTanSat.autoChangeFocus ? "BẬT" : "TẮT"), 0);
						}
						else
						{
							Skill chosen = skills[idx - 4];
							ModTanSat.ToggleSkillTicked(chosen.template.id);
							if (ModTanSatFilter.skillSelectionMode == 0 && chosen.template != null)
							{
								GameScr.info1?.addInfo("Chỉ định chiêu Tàn Sát: " + chosen.template.name, 0);
							}
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
			if (px >= uiX + 68 && px <= uiX + 84)
			{
				ModTanSat.timeAttack -= 50;
				if (ModTanSat.timeAttack < 50) ModTanSat.timeAttack = 50;
				ModConfig.SaveConfig();
				SoundMn.gI().buttonClick();
				return true;
			}
			// Nút current [ xxx ms ]
			if (px >= uiX + 86 && px <= uiX + 134)
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
			if (px >= uiX + 136 && px <= uiX + 152)
			{
				ModTanSat.timeAttack += 50;
				if (ModTanSat.timeAttack > 3000) ModTanSat.timeAttack = 3000;
				ModConfig.SaveConfig();
				SoundMn.gI().buttonClick();
				return true;
			}

			int presetStartX = uiX + 155;
			int presetW = (uiW - 159 - 9) / 4;
			if (presetW < 24) presetW = 24;
			int[] presets = new int[] { 100, 200, 300, 500 };
			for (int p = 0; p < presets.Length; p++)
			{
				int pbx = presetStartX + p * (presetW + 3);
				if (px >= pbx && px <= pbx + presetW)
				{
					ModTanSat.timeAttack = presets[p];
					ModConfig.SaveConfig();
					SoundMn.gI().buttonClick();
					return true;
				}
			}
		}

		return false;
	}
}
