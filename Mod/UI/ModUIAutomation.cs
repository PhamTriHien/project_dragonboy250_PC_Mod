using System;

/// <summary>
/// Giao diện cấu hình Tự Động Hóa (Nhân Vật, Set Đồ, Tiềm Năng, Item, Hợp Thể, Tân Thủ, Đệ Tử, Captcha, Lập Lịch & Reconnect).
/// </summary>
public static class ModUIAutomation
{
	public static int subTab = 0;
	public static int scrollY = 0;

	private static bool isDragging = false;
	private static bool hasDragged = false;
	private static int startDragY = 0;
	private static int lastDragY = 0;

	private static int GetContentHeight()
	{
		return (subTab == 0) ? 340 : 294;
	}

	public static void OnMouseScroll(float wheel)
	{
		int step = 22;
		if (wheel > 0)
		{
			scrollY -= step;
			if (scrollY < 0) scrollY = 0;
		}
		else if (wheel < 0)
		{
			scrollY += step;
		}
	}

	public static void UpdateDragScroll(int px, int py, int uiX, int uiY, int uiW, int uiH)
	{
		int row0Y = uiY + 4;
		int row0H = 18;
		int boxX = uiX + 4;
		int boxY = row0Y + row0H + 4;
		int boxW = uiW - 8;
		int boxH = uiH - (boxY - uiY) - 4;
		if (boxH < 40) boxH = 40;

		int contentH = GetContentHeight();
		int viewH = boxH - 8;
		int maxScroll = (contentH > viewH) ? (contentH - viewH) : 0;

		if (GameCanvas.isPointerDown)
		{
			if (!isDragging)
			{
				if (px >= boxX && px <= boxX + boxW && py >= boxY && py <= boxY + boxH)
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
					scrollY -= moveY;
					if (scrollY < 0) scrollY = 0;
					if (scrollY > maxScroll) scrollY = maxScroll;
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
		int subTabW = (uiW - 12) / 2;
		int row0Y = uiY + 4;
		int row0H = 18;
		ModUI.PaintNativeButton(uiX + 4, row0Y, subTabW, row0H, "1. Nhân Vật & Item", subTab == 0, g);
		ModUI.PaintNativeButton(uiX + 8 + subTabW, row0Y, subTabW, row0H, "2. Đệ Tử & Tiện Ích", subTab == 1, g);

		int boxX = uiX + 4;
		int boxY = row0Y + row0H + 4;
		int boxW = uiW - 8;
		int boxH = uiH - (boxY - uiY) - 4;
		if (boxH < 40) boxH = 40;

		int contentH = GetContentHeight();
		int viewH = boxH - 8;
		int maxScroll = (contentH > viewH) ? (contentH - viewH) : 0;
		if (scrollY > maxScroll) scrollY = maxScroll;
		if (scrollY < 0) scrollY = 0;

		GameCanvas.paintz.paintFrameSimple(boxX, boxY, boxW, boxH, g);
		g.setColor(15196114);
		g.fillRect(boxX + 2, boxY + 2, boxW - 4, boxH - 4);

		g.setClip(boxX + 2, boxY + 2, boxW - 4, boxH - 4);
		if (subTab == 0)
		{
			PaintSubTabPlayer(boxX + 4, boxY + 4 - scrollY, boxW - 8, boxH - 8, g);
		}
		else
		{
			PaintSubTabUtilities(boxX + 4, boxY + 4 - scrollY, boxW - 8, boxH - 8, g);
		}
		g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
	}

	private static void PaintSubTabPlayer(int x, int y, int w, int h, mGraphics g)
	{
		int curY = y;

		// 0. SET ĐỒ NHANH (SET 1 & SET 2)
		mFont.tahoma_7_green2.drawString(g, "SET ĐỒ NHANH (HÀNH TRANG):", x + 2, curY + 2, mFont.LEFT);
		curY += 18;

		int setBtnW = (w - 6) / 3;
		int c1 = ModInventoryExt.GetSetCount(1);
		int c2 = ModInventoryExt.GetSetCount(2);
		ModUI.PaintNativeButton(x, curY, setBtnW, 16, "Mặc Set 1 (" + c1 + ")", c1 > 0, g);
		ModUI.PaintNativeButton(x + setBtnW + 3, curY, setBtnW, 16, "Mặc Set 2 (" + c2 + ")", c2 > 0, g);
		ModUI.PaintNativeButton(x + (setBtnW + 3) * 2, curY, setBtnW, 16, "Xóa Set 1/2", false, g);
		curY += 21;

		// 1. AUTO CỘNG ĐIỂM TIỀM NĂNG
		mFont.tahoma_7_green2.drawString(g, "CỘNG ĐIỂM TIỀM NĂNG:", x + 2, curY + 2, mFont.LEFT);
		string capLabel = "Mốc: " + (ModAutoPoint.maxTargetStat > 0 ? Res.formatNumber2(ModAutoPoint.maxTargetStat) : "Tắt");
		ModUI.PaintNativeButton(x + w - 118, curY, 72, 16, capLabel, ModAutoPoint.maxTargetStat > 0, g);
		ModUI.PaintNativeButton(x + w - 42, curY, 40, 16, ModAutoPoint.isEnabled ? "BẬT" : "TẮT", ModAutoPoint.isEnabled, g);
		curY += 19;

		int btnW5 = (w - 12) / 5;
		string[] statNames = new string[] { "HP Gốc", "KI Gốc", "Sức Đánh", "Giáp", "Chí Mạng" };
		for (int i = 0; i < 5; i++)
		{
			int bx = x + i * (btnW5 + 3);
			bool isSel = !ModAutoPoint.HasAnyMultiStatTarget() && (ModAutoPoint.targetType == i);
			ModUI.PaintNativeButton(bx, curY, btnW5, 16, statNames[i], isSel, g);
		}
		curY += 19;

		string[] multiShort = new string[] { "M.HP", "M.KI", "M.SĐ", "M.Giáp", "M.CM" };
		for (int i = 0; i < 5; i++)
		{
			int bx = x + i * (btnW5 + 3);
			long targetVal = ModAutoPoint.maxTargetStats[i];
			string lbl = targetVal > 0 ? (multiShort[i] + ":" + Res.formatNumber2(targetVal)) : multiShort[i];
			ModUI.PaintNativeButton(bx, curY, btnW5, 16, lbl, targetVal > 0, g);
		}
		curY += 19;

		mFont.tahoma_7_grey.drawString(g, "Mức cộng/lần: +" + ModAutoPoint.pointsPerTick, x + 2, curY + 3, mFont.LEFT);
		int[] ptPresets = new int[] { 1, 10, 100 };
		int ptBtnW = 34;
		for (int p = 0; p < ptPresets.Length; p++)
		{
			int pbx = x + w - (ptPresets.Length - p) * (ptBtnW + 3);
			bool isSelPt = (ModAutoPoint.pointsPerTick == ptPresets[p]);
			ModUI.PaintNativeButton(pbx, curY, ptBtnW, 16, "+" + ptPresets[p], isSelPt, g);
		}
		curY += 23;

		// 2. AUTO DÙNG ITEM TRONG TÚI
		mFont.tahoma_7_green2.drawString(g, "AUTO DÙNG ITEM (TÚI):", x + 2, curY + 2, mFont.LEFT);
		ModUI.PaintNativeButton(x + w - 114, curY, 68, 16, ModAutoItem.autoOpenCskb ? "CSKB: BẬT" : "CSKB: TẮT", ModAutoItem.autoOpenCskb, g);
		ModUI.PaintNativeButton(x + w - 42, curY, 40, 16, ModAutoItem.isEnabled ? "BẬT" : "TẮT", ModAutoItem.isEnabled, g);
		curY += 18;

		int presetBtnW = (w - 9) / 4;
		for (int row = 0; row < 3; row++)
		{
			for (int col = 0; col < 4; col++)
			{
				int pIdx = row * 4 + col;
				int pbx = x + col * (presetBtnW + 3);
				ModUI.PaintNativeButton(pbx, curY, presetBtnW, 16, ModAutoItem.PresetNames[pIdx], ModAutoItem.IsPresetActive(pIdx), g);
			}
			curY += 18;
		}
		curY += 1;

		int condBtnW = (w - 6) / 3;
		string[] condNames = new string[] { "Khi hết / Chu kỳ", "HP < X%", "MP < X%" };
		for (int c = 0; c < 3; c++)
		{
			int cx = x + c * (condBtnW + 3);
			bool isSelCond = (ModAutoItem.conditionType == c);
			ModUI.PaintNativeButton(cx, curY, condBtnW, 16, condNames[c], isSelCond, g);
		}
		curY += 19;

		if (ModAutoItem.conditionType > 0)
		{
			mFont.tahoma_7_grey.drawString(g, "Ngưỡng:", x + 2, curY + 3, mFont.LEFT);
			int[] thresholds = new int[] { 20, 30, 50, 70 };
			int subW4 = (w - 48 - 9) / 4;
			for (int t = 0; t < 4; t++)
			{
				int tx = x + 48 + t * (subW4 + 3);
				bool isSelTh = (ModAutoItem.thresholdPercent == thresholds[t]);
				ModUI.PaintNativeButton(tx, curY, subW4, 16, "< " + thresholds[t] + "%", isSelTh, g);
			}
		}
		else
		{
			mFont.tahoma_7_grey.drawString(g, "Chu kỳ:", x + 2, curY + 3, mFont.LEFT);
			int[] itemIntervals = new int[] { 1000, 3000, 5000, 10000 };
			string[] intvNames = new string[] { "1s", "3s", "5s", "10s" };
			int subW4 = (w - 48 - 9) / 4;
			for (int t = 0; t < 4; t++)
			{
				int tx = x + 48 + t * (subW4 + 3);
				bool isSelInt = (ModAutoItem.intervalMs == itemIntervals[t]);
				ModUI.PaintNativeButton(tx, curY, subW4, 16, intvNames[t], isSelInt, g);
			}
		}
		curY += 20;

		string idList = string.IsNullOrEmpty(ModAutoItem.itemIdsRaw) ? "(Chưa chọn)" : ("[" + ModAutoItem.itemIdsRaw + "]");
		if (idList.Length > 21) idList = idList.Substring(0, 18) + "...]";
		mFont.tahoma_7_grey.drawString(g, "ID: " + idList, x + 2, curY + 3, mFont.LEFT);
		ModUI.PaintNativeButton(x + w - 103, curY, 44, 16, "Xóa ID", false, g);
		ModUI.PaintNativeButton(x + w - 56, curY, 54, 16, "Nhập ID", false, g);
		curY += 23;

		// 3. AUTO HỢP THỂ (FUSION)
		mFont.tahoma_7_green2.drawString(g, "AUTO HỢP THỂ:", x + 2, curY + 2, mFont.LEFT);
		ModUI.PaintNativeButton(x + w - 116, curY, 70, 16, ModAutoFusion.spamPotaraMode ? "Spam 11s: ON" : "Spam 11s", ModAutoFusion.spamPotaraMode, g);
		ModUI.PaintNativeButton(x + w - 42, curY, 40, 16, ModAutoFusion.isEnabled ? "BẬT" : "TẮT", ModAutoFusion.isEnabled, g);
		curY += 19;

		int fuseW = (w - 3) / 2;
		ModUI.PaintNativeButton(x, curY, fuseW, 16, "Bông Tai Porata", ModAutoFusion.fusionType == 4, g);
		ModUI.PaintNativeButton(x + fuseW + 3, curY, fuseW, 16, "Kaio-ken / Thường", ModAutoFusion.fusionType == 6, g);
		curY += 21;

		// 4. AUTO NHIỆM VỤ TÂN THỦ (TASK 0 -> 10)
		mFont.tahoma_7_green2.drawString(g, "AUTO NV TÂN THỦ (0->10):", x + 2, curY + 2, mFont.LEFT);
		ModUI.PaintNativeButton(x + w - 42, curY, 40, 16, ModAutoNewAccount.isEnabled ? "BẬT" : "TẮT", ModAutoNewAccount.isEnabled, g);
		curY += 20;

		// 5. TỰ SÁT NHANH
		mFont.tahoma_7_green2.drawString(g, "TỰ SÁT NHANH (CỜ ĐEN):", x + 2, curY + 2, mFont.LEFT);
		ModUI.PaintNativeButton(x + w - 42, curY, 40, 16, ModSelfKill.isEnabled ? "BẬT" : "TẮT", ModSelfKill.isEnabled, g);
	}

	private static void PaintSubTabUtilities(int x, int y, int w, int h, mGraphics g)
	{
		int curY = y;

		// 1. AUTO ĐỆ TỬ (PET)
		mFont.tahoma_7_green2.drawString(g, "AUTO ĐỆ TỬ:", x + 2, curY + 2, mFont.LEFT);
		ModUI.PaintNativeButton(x + w - 166, curY, 68, 16, ModAutoPet.GetPetPowerLimitLabel(), ModAutoPet.exitOnPetPowerLimit > 0, g);
		ModUI.PaintNativeButton(x + w - 95, curY, 50, 16, "Nhập SM", false, g);
		ModUI.PaintNativeButton(x + w - 42, curY, 40, 16, ModAutoPet.isEnabled ? "BẬT" : "TẮT", ModAutoPet.isEnabled, g);
		curY += 18;

		int petBtnW = (w - 9) / 4;
		string[] petCmds = new string[] { "Đi Theo", "Bảo Vệ", "Tấn Công", "Về Nhà" };
		for (int i = 0; i < 4; i++)
		{
			int bx = x + i * (petBtnW + 3);
			bool isSelPet = (ModAutoPet.status == i);
			ModUI.PaintNativeButton(bx, curY, petBtnW, 16, petCmds[i], isSelPet, g);
		}
		curY += 18;

		ModUI.PaintNativeButton(x, curY, petBtnW, 16, "HUD Đệ", ModAutoPet.showPetHUD, g);
		ModUI.PaintNativeButton(x + (petBtnW + 3), curY, petBtnW, 16, "Né Boss", ModAutoPet.petAvoidBoss, g);
		ModUI.PaintNativeButton(x + (petBtnW + 3) * 2, curY, petBtnW, 16, "Cờ Đen", ModAutoPet.petAutoFlag, g);
		ModUI.PaintNativeButton(x + (petBtnW + 3) * 3, curY, petBtnW, 16, "Trị Thương", ModAutoPet.autoNamekHeal, g);
		curY += 18;

		int petW3 = (w - 6) / 3;
		ModUI.PaintNativeButton(x, curY, petW3, 16, "Sủa Đánh Mồi", ModAutoPet.petAutoAttackOnBark, g);
		ModUI.PaintNativeButton(x + petW3 + 3, curY, petW3, 16, "Auto KOK", ModAutoPet.petAutoKok, g);
		ModUI.PaintNativeButton(x + (petW3 + 3) * 2, curY, petW3, 16, "Đạt SM Ngắt", ModAutoPet.disconnectOnPetPowerLimit, g);
		curY += 21;

		// 2. AUTO GIẢI CAPTCHA
		mFont.tahoma_7_green2.drawString(g, "AUTO GIẢI CAPTCHA:", x + 2, curY + 2, mFont.LEFT);
		ModUI.PaintNativeButton(x + w - 118, curY, 72, 16, ModAutoCaptcha.disconnectOnFail ? "Sai>5: Ngắt" : "Sai>5: Tắt", ModAutoCaptcha.disconnectOnFail, g);
		ModUI.PaintNativeButton(x + w - 42, curY, 40, 16, ModAutoCaptcha.isEnabled ? "BẬT" : "TẮT", ModAutoCaptcha.isEnabled, g);
		curY += 18;

		string apiPreview = string.IsNullOrEmpty(ModAutoCaptcha.captchaApiUrl) ? "(Chưa nhập Link API)" : ModAutoCaptcha.captchaApiUrl;
		if (apiPreview.Length > 24) apiPreview = apiPreview.Substring(0, 21) + "...";
		mFont.tahoma_7_grey.drawString(g, apiPreview, x + 2, curY + 3, mFont.LEFT);
		ModUI.PaintNativeButton(x + w - 68, curY, 66, 16, "Nhập API", !string.IsNullOrEmpty(ModAutoCaptcha.captchaApiUrl), g);
		curY += 21;

		// 3. AUTO CHAT
		mFont.tahoma_7_green2.drawString(g, "AUTO CHAT ĐỊNH KỲ:", x + 2, curY + 2, mFont.LEFT);
		ModUI.PaintNativeButton(x + w - 108, curY, 62, 16, "Nhập Giây", false, g);
		ModUI.PaintNativeButton(x + w - 42, curY, 40, 16, ModAutoChat.isEnabled ? "BẬT" : "TẮT", ModAutoChat.isEnabled, g);
		curY += 18;

		string msgPreview = string.IsNullOrEmpty(ModAutoChat.message) ? "(Chưa đặt nội dung)" : ("\"" + ModAutoChat.message + "\"");
		if (msgPreview.Length > 24) msgPreview = msgPreview.Substring(0, 21) + "...\"";
		mFont.tahoma_7_grey.drawString(g, msgPreview, x + 2, curY + 3, mFont.LEFT);
		ModUI.PaintNativeButton(x + w - 74, curY, 72, 16, "Đổi Lời Chat", false, g);
		curY += 18;

		mFont.tahoma_7_grey.drawString(g, "Chu kỳ (" + (ModAutoChat.intervalMs / 1000) + "s):", x + 2, curY + 3, mFont.LEFT);
		int[] chatIntervals = new int[] { 10000, 30000, 60000, 120000 };
		string[] chatIntvNames = new string[] { "10s", "30s", "1m", "2m" };
		int intvW = (w - 84) / 4;
		for (int i = 0; i < 4; i++)
		{
			int cx = x + 78 + i * (intvW + 2);
			bool isSelInt = (ModAutoChat.intervalMs == chatIntervals[i]);
			ModUI.PaintNativeButton(cx, curY, intvW, 16, chatIntvNames[i], isSelInt, g);
		}
		curY += 21;

		// 4. SẮP XẾP RƯƠNG
		mFont.tahoma_7_green2.drawString(g, "SẮP XẾP RƯƠNG ĐỒ:", x + 2, curY + 2, mFont.LEFT);
		ModUI.PaintNativeButton(x + w - 42, curY, 40, 16, ModAutoBoxSort.isEnabled ? "BẬT" : "TẮT", ModAutoBoxSort.isEnabled, g);
		ModUI.PaintNativeButton(x + w - 128, curY, 82, 16, "Sắp Xếp Ngay", false, g);
		curY += 21;

		// 5. LẬP LỊCH ON/OFF & 6 SLOT TRAIN
		mFont.tahoma_7_green2.drawString(g, "LẬP LỊCH ON/OFF:", x + 2, curY + 2, mFont.LEFT);
		ModUI.PaintNativeButton(x + w - 126, curY, 80, 16, ModSchedule.disconnectOnOffWindow ? "Ngoài Giờ: Ngắt" : "Ngoài Giờ: Treo", ModSchedule.disconnectOnOffWindow, g);
		ModUI.PaintNativeButton(x + w - 42, curY, 40, 16, ModSchedule.isEnabled ? "BẬT" : "TẮT", ModSchedule.isEnabled, g);
		curY += 18;

		int schedW4 = (w - 9) / 4;
		bool isAllDay = (ModSchedule.startMinute == ModSchedule.endMinute);
		bool isDay = (ModSchedule.startMinute == 8 * 60 && ModSchedule.endMinute == 18 * 60);
		bool isNight = (ModSchedule.startMinute == 22 * 60 && ModSchedule.endMinute == 6 * 60);
		ModUI.PaintNativeButton(x, curY, schedW4, 16, "Cả Ngày", isAllDay, g);
		ModUI.PaintNativeButton(x + (schedW4 + 3), curY, schedW4, 16, "08h-18h", isDay, g);
		ModUI.PaintNativeButton(x + (schedW4 + 3) * 2, curY, schedW4, 16, "22h-06h", isNight, g);
		ModUI.PaintNativeButton(x + (schedW4 + 3) * 3, curY, schedW4, 16, "Nhập Giờ", !isAllDay && !isDay && !isNight, g);
		curY += 18;

		int slotW3 = (w - 6) / 3;
		for (int row = 0; row < 2; row++)
		{
			for (int col = 0; col < 3; col++)
			{
				int sIdx = row * 3 + col;
				int sx = x + col * (slotW3 + 3);
				bool hasSlot = !string.IsNullOrEmpty(ModSchedule.trainSlotsRaw[sIdx]);
				ModUI.PaintNativeButton(sx, curY, slotW3, 16, "Slot " + (sIdx + 1) + (hasSlot ? " *" : ""), hasSlot, g);
			}
			curY += (row == 0) ? 18 : 21;
		}

		// 6. TỰ KẾT NỐI LẠI (AUTO RECONNECT) & GIỮ KHU
		mFont.tahoma_7_green2.drawString(g, "RECONNECT & GIỮ KHU:", x + 2, curY + 2, mFont.LEFT);
		ModUI.PaintNativeButton(x + w - 114, curY, 68, 16, ModAutoLogin.keepSavedZone ? ("Giữ K." + ModAutoLogin.savedKeepZoneId) : "Giữ Khu: Tắt", ModAutoLogin.keepSavedZone, g);
		ModUI.PaintNativeButton(x + w - 42, curY, 40, 16, ModAutoLogin.isEnabled ? "BẬT" : "TẮT", ModAutoLogin.isEnabled, g);
	}

	public static bool HandleTap(int px, int py, int uiX, int uiY, int uiW, int uiH)
	{
		if (hasDragged)
		{
			hasDragged = false;
			return true;
		}

		int subTabW = (uiW - 12) / 2;
		int row0Y = uiY + 4;
		int row0H = 18;

		if (py >= row0Y && py <= row0Y + row0H)
		{
			if (px >= uiX + 4 && px <= uiX + 4 + subTabW)
			{
				subTab = 0;
				scrollY = 0;
				SoundMn.gI().buttonClick();
				return true;
			}
			if (px >= uiX + 8 + subTabW && px <= uiX + 8 + subTabW * 2)
			{
				subTab = 1;
				scrollY = 0;
				SoundMn.gI().buttonClick();
				return true;
			}
		}

		int boxX = uiX + 4;
		int boxY = row0Y + row0H + 4;
		int boxW = uiW - 8;
		int boxH = uiH - (boxY - uiY) - 4;
		if (boxH < 40) boxH = 40;
		if (px < boxX || px > boxX + boxW || py < boxY || py > boxY + boxH)
		{
			return false;
		}

		int x = boxX + 4;
		int y = boxY + 4 - scrollY;
		int w = boxW - 8;

		if (subTab == 0)
		{
			return HandleTapPlayer(px, py, x, y, w);
		}
		else
		{
			return HandleTapUtilities(px, py, x, y, w);
		}
	}

	private static bool HandleTapPlayer(int px, int py, int x, int y, int w)
	{
		int curY = y;

		// 0. Set Đồ Nhanh (Set 1 / Set 2 / Xóa)
		curY += 18;
		int setBtnW = (w - 6) / 3;
		if (py >= curY - 2 && py <= curY + 18)
		{
			if (px >= x && px <= x + setBtnW)
			{
				ModInventoryExt.EquipSet(1);
				return true;
			}
			if (px >= x + setBtnW + 3 && px <= x + setBtnW * 2 + 3)
			{
				ModInventoryExt.EquipSet(2);
				return true;
			}
			if (px >= x + (setBtnW + 3) * 2 && px <= x + w)
			{
				ModInventoryExt.ClearSets();
				return true;
			}
		}
		curY += 21;

		// 1. Auto Tiềm Năng: Nút Mốc giới hạn & Bật / Tắt
		if (px >= x + w - 120 && px <= x + w - 45 && py >= curY - 2 && py <= curY + 18)
		{
			ModAutoPoint.ShowInputMaxTargetStat();
			SoundMn.gI().buttonClick();
			return true;
		}
		if (px >= x + w - 44 && px <= x + w && py >= curY - 2 && py <= curY + 18)
		{
			ModAutoPoint.Toggle();
			SoundMn.gI().buttonClick();
			return true;
		}
		curY += 19;

		// Chọn chỉ số đơn (HP, KI, Sức Đánh, Giáp, Chí Mạng)
		int btnW5 = (w - 12) / 5;
		for (int i = 0; i < 5; i++)
		{
			int bx = x + i * (btnW5 + 3);
			if (px >= bx && px <= bx + btnW5 && py >= curY - 2 && py <= curY + 18)
			{
				ModAutoPoint.Configure(i, ModAutoPoint.pointsPerTick, ModAutoPoint.intervalMs);
				ModConfig.SaveConfig();
				SoundMn.gI().buttonClick();
				return true;
			}
		}
		curY += 19;

		// Mốc 5 chỉ số đồng thời (M.HP, M.KI, M.SĐ, M.Giáp, M.CM)
		for (int i = 0; i < 5; i++)
		{
			int bx = x + i * (btnW5 + 3);
			if (px >= bx && px <= bx + btnW5 && py >= curY - 2 && py <= curY + 18)
			{
				ModAutoPoint.ShowInputStatTarget(i);
				SoundMn.gI().buttonClick();
				return true;
			}
		}
		curY += 19;

		// Chọn điểm mỗi lần (1, 10, 100)
		int[] ptPresets = new int[] { 1, 10, 100 };
		int ptBtnW = 34;
		for (int p = 0; p < ptPresets.Length; p++)
		{
			int pbx = x + w - (ptPresets.Length - p) * (ptBtnW + 3);
			if (px >= pbx && px <= pbx + ptBtnW && py >= curY - 2 && py <= curY + 18)
			{
				ModAutoPoint.Configure(ModAutoPoint.targetType, ptPresets[p], ModAutoPoint.intervalMs);
				ModConfig.SaveConfig();
				SoundMn.gI().buttonClick();
				return true;
			}
		}
		curY += 23;

		// 2. Auto Dùng Item: CSKB & Bật / Tắt
		if (px >= x + w - 116 && px <= x + w - 45 && py >= curY - 2 && py <= curY + 18)
		{
			ModAutoItem.autoOpenCskb = !ModAutoItem.autoOpenCskb;
			ModConfig.SaveConfig();
			GameScr.info1?.addInfo("Auto Mở CSKB (380): " + (ModAutoItem.autoOpenCskb ? "BẬT" : "TẮT"), 0);
			SoundMn.gI().buttonClick();
			return true;
		}
		if (px >= x + w - 44 && px <= x + w && py >= curY - 2 && py <= curY + 18)
		{
			ModAutoItem.Toggle();
			SoundMn.gI().buttonClick();
			return true;
		}
		curY += 18;

		int presetBtnW = (w - 9) / 4;
		for (int row = 0; row < 3; row++)
		{
			for (int col = 0; col < 4; col++)
			{
				int pIdx = row * 4 + col;
				int pbx = x + col * (presetBtnW + 3);
				if (px >= pbx && px <= pbx + presetBtnW && py >= curY - 2 && py <= curY + 17)
				{
					ModAutoItem.TogglePreset(pIdx);
					SoundMn.gI().buttonClick();
					return true;
				}
			}
			curY += 18;
		}
		curY += 1;

		int condBtnW = (w - 6) / 3;
		for (int c = 0; c < 3; c++)
		{
			int cx = x + c * (condBtnW + 3);
			if (px >= cx && px <= cx + condBtnW && py >= curY - 2 && py <= curY + 18)
			{
				ModAutoItem.conditionType = c;
				ModConfig.SaveConfig();
				SoundMn.gI().buttonClick();
				return true;
			}
		}
		curY += 19;

		int subW4 = (w - 48 - 9) / 4;
		if (ModAutoItem.conditionType > 0)
		{
			int[] thresholds = new int[] { 20, 30, 50, 70 };
			for (int t = 0; t < 4; t++)
			{
				int tx = x + 48 + t * (subW4 + 3);
				if (px >= tx && px <= tx + subW4 && py >= curY - 2 && py <= curY + 18)
				{
					ModAutoItem.thresholdPercent = thresholds[t];
					ModConfig.SaveConfig();
					SoundMn.gI().buttonClick();
					return true;
				}
			}
		}
		else
		{
			int[] itemIntervals = new int[] { 1000, 3000, 5000, 10000 };
			for (int t = 0; t < 4; t++)
			{
				int tx = x + 48 + t * (subW4 + 3);
				if (px >= tx && px <= tx + subW4 && py >= curY - 2 && py <= curY + 18)
				{
					ModAutoItem.intervalMs = itemIntervals[t];
					ModConfig.SaveConfig();
					SoundMn.gI().buttonClick();
					return true;
				}
			}
		}
		curY += 20;

		if (px >= x + w - 105 && px <= x + w - 58 && py >= curY - 2 && py <= curY + 18)
		{
			ModAutoItem.ClearAllIds();
			SoundMn.gI().buttonClick();
			return true;
		}
		if (px >= x + w - 57 && px <= x + w && py >= curY - 2 && py <= curY + 18)
		{
			ModAutoItem.ShowInputItemIds();
			return true;
		}
		curY += 23;

		// 3. Auto Hợp Thể: Spam 11s & Bật / Tắt
		if (px >= x + w - 118 && px <= x + w - 45 && py >= curY - 2 && py <= curY + 18)
		{
			ModAutoFusion.ToggleSpamPotara();
			SoundMn.gI().buttonClick();
			return true;
		}
		if (px >= x + w - 44 && px <= x + w && py >= curY - 2 && py <= curY + 18)
		{
			ModAutoFusion.Toggle();
			SoundMn.gI().buttonClick();
			return true;
		}
		curY += 19;

		int fuseW = (w - 3) / 2;
		if (px >= x && px <= x + fuseW && py >= curY - 2 && py <= curY + 18)
		{
			ModAutoFusion.Configure(4, ModAutoFusion.intervalMs);
			SoundMn.gI().buttonClick();
			return true;
		}
		if (px >= x + fuseW + 3 && px <= x + w && py >= curY - 2 && py <= curY + 18)
		{
			ModAutoFusion.Configure(6, ModAutoFusion.intervalMs);
			SoundMn.gI().buttonClick();
			return true;
		}
		curY += 21;

		// 4. Auto NV Tân Thủ (Task 0 -> 10)
		if (px >= x + w - 44 && px <= x + w && py >= curY - 2 && py <= curY + 18)
		{
			ModAutoNewAccount.Toggle();
			return true;
		}
		curY += 20;

		// 5. Tự Sát Nhanh
		if (px >= x + w - 44 && px <= x + w && py >= curY - 2 && py <= curY + 18)
		{
			ModSelfKill.Toggle();
			return true;
		}

		return false;
	}

	private static bool HandleTapUtilities(int px, int py, int x, int y, int w)
	{
		int curY = y;

		// 1. Auto Pet: Mốc SM Đệ, Nhập SM & Bật / Tắt
		if (py >= curY - 2 && py <= curY + 18)
		{
			if (px >= x + w - 168 && px <= x + w - 97)
			{
				ModAutoPet.CyclePetPowerLimit();
				GameScr.info1?.addInfo("Đệ Tử - " + ModAutoPet.GetPetPowerLimitLabel(), 0);
				return true;
			}
			if (px >= x + w - 96 && px <= x + w - 45)
			{
				ModAutoPet.ShowInputPetPowerLimit();
				SoundMn.gI().buttonClick();
				return true;
			}
			if (px >= x + w - 44 && px <= x + w)
			{
				ModAutoPet.Toggle();
				SoundMn.gI().buttonClick();
				return true;
			}
		}
		curY += 18;

		int petBtnW = (w - 9) / 4;
		for (int i = 0; i < 4; i++)
		{
			int bx = x + i * (petBtnW + 3);
			if (px >= bx && px <= bx + petBtnW && py >= curY - 2 && py <= curY + 18)
			{
				ModAutoPet.Configure(i, ModAutoPet.intervalMs);
				SoundMn.gI().buttonClick();
				return true;
			}
		}
		curY += 18;

		if (py >= curY - 2 && py <= curY + 18)
		{
			if (px >= x && px <= x + petBtnW)
			{
				ModAutoPet.showPetHUD = !ModAutoPet.showPetHUD;
				ModConfig.SaveConfig();
				GameScr.info1.addInfo("HUD Đệ Tử: " + (ModAutoPet.showPetHUD ? "BẬT" : "TẮT"), 0);
				SoundMn.gI().buttonClick();
				return true;
			}
			if (px >= x + (petBtnW + 3) && px <= x + (petBtnW + 3) + petBtnW)
			{
				ModAutoPet.petAvoidBoss = !ModAutoPet.petAvoidBoss;
				ModConfig.SaveConfig();
				GameScr.info1.addInfo("Đệ Tử Né Boss: " + (ModAutoPet.petAvoidBoss ? "BẬT" : "TẮT"), 0);
				SoundMn.gI().buttonClick();
				return true;
			}
			if (px >= x + (petBtnW + 3) * 2 && px <= x + (petBtnW + 3) * 2 + petBtnW)
			{
				ModAutoPet.petAutoFlag = !ModAutoPet.petAutoFlag;
				ModConfig.SaveConfig();
				GameScr.info1.addInfo("Tự Bật Cờ Đen: " + (ModAutoPet.petAutoFlag ? "BẬT" : "TẮT"), 0);
				SoundMn.gI().buttonClick();
				return true;
			}
			if (px >= x + (petBtnW + 3) * 3 && px <= x + w)
			{
				ModAutoPet.autoNamekHeal = !ModAutoPet.autoNamekHeal;
				ModConfig.SaveConfig();
				GameScr.info1.addInfo("Auto Trị Thương Đệ: " + (ModAutoPet.autoNamekHeal ? "BẬT" : "TẮT"), 0);
				SoundMn.gI().buttonClick();
				return true;
			}
		}
		curY += 18;

		int petW3 = (w - 6) / 3;
		if (py >= curY - 2 && py <= curY + 18)
		{
			if (px >= x && px <= x + petW3)
			{
				ModAutoPet.petAutoAttackOnBark = !ModAutoPet.petAutoAttackOnBark;
				ModConfig.SaveConfig();
				GameScr.info1.addInfo("Đệ sủa 'không đánh' -> SP đánh mồi: " + (ModAutoPet.petAutoAttackOnBark ? "BẬT" : "TẮT"), 0);
				SoundMn.gI().buttonClick();
				return true;
			}
			if (px >= x + petW3 + 3 && px <= x + petW3 * 2 + 3)
			{
				ModAutoPet.petAutoKok = !ModAutoPet.petAutoKok;
				ModConfig.SaveConfig();
				GameScr.info1.addInfo("Đệ Tử Auto Kaio-ken: " + (ModAutoPet.petAutoKok ? "BẬT" : "TẮT"), 0);
				SoundMn.gI().buttonClick();
				return true;
			}
			if (px >= x + (petW3 + 3) * 2 && px <= x + w)
			{
				ModAutoPet.disconnectOnPetPowerLimit = !ModAutoPet.disconnectOnPetPowerLimit;
				ModConfig.SaveConfig();
				GameScr.info1.addInfo("Đạt mốc SM Đệ ngắt kết nối: " + (ModAutoPet.disconnectOnPetPowerLimit ? "BẬT" : "TẮT (Đổi trạng thái)"), 0);
				SoundMn.gI().buttonClick();
				return true;
			}
		}
		curY += 21;

		// 2. Auto Giải Captcha
		if (py >= curY - 2 && py <= curY + 18)
		{
			if (px >= x + w - 120 && px <= x + w - 45)
			{
				ModAutoCaptcha.disconnectOnFail = !ModAutoCaptcha.disconnectOnFail;
				ModConfig.SaveConfig();
				GameScr.info1.addInfo("Captcha sai quá " + ModAutoCaptcha.maxFailCount + " lần: " + (ModAutoCaptcha.disconnectOnFail ? "Ngắt kết nối" : "Chỉ tạm tắt"), 0);
				SoundMn.gI().buttonClick();
				return true;
			}
			if (px >= x + w - 44 && px <= x + w)
			{
				ModAutoCaptcha.isEnabled = !ModAutoCaptcha.isEnabled;
				ModConfig.SaveConfig();
				GameScr.info1.addInfo("Auto Giải Captcha: " + (ModAutoCaptcha.isEnabled ? "BẬT" : "TẮT"), 0);
				SoundMn.gI().buttonClick();
				return true;
			}
		}
		curY += 18;

		if (px >= x + w - 70 && px <= x + w && py >= curY - 2 && py <= curY + 18)
		{
			ModAutoCaptcha.PromptApiUrl();
			SoundMn.gI().buttonClick();
			return true;
		}
		curY += 21;

		// 3. Auto Chat: Nhập Giây & Bật / Tắt
		if (py >= curY - 2 && py <= curY + 18)
		{
			if (px >= x + w - 110 && px <= x + w - 45)
			{
				ModAutoChat.ShowInputIntervalSeconds();
				SoundMn.gI().buttonClick();
				return true;
			}
			if (px >= x + w - 44 && px <= x + w)
			{
				ModAutoChat.Toggle();
				SoundMn.gI().buttonClick();
				return true;
			}
		}
		curY += 18;

		if (px >= x + w - 76 && px <= x + w && py >= curY - 2 && py <= curY + 18)
		{
			ModAutoChat.ShowInputChatMessage();
			return true;
		}
		curY += 18;

		int[] chatIntervals = new int[] { 10000, 30000, 60000, 120000 };
		int intvW = (w - 84) / 4;
		for (int i = 0; i < 4; i++)
		{
			int cx = x + 78 + i * (intvW + 2);
			if (px >= cx && px <= cx + intvW && py >= curY - 2 && py <= curY + 18)
			{
				ModAutoChat.Configure(ModAutoChat.message, chatIntervals[i]);
				SoundMn.gI().buttonClick();
				return true;
			}
		}
		curY += 21;

		// 4. Sắp Xếp Rương
		if (px >= x + w - 44 && px <= x + w && py >= curY - 2 && py <= curY + 18)
		{
			ModAutoBoxSort.Toggle();
			SoundMn.gI().buttonClick();
			return true;
		}
		if (px >= x + w - 130 && px <= x + w - 48 && py >= curY - 2 && py <= curY + 18)
		{
			ModAutoBoxSort.SortNow();
			GameScr.info1?.addInfo("Đã gửi lệnh sắp xếp rương.", 0);
			SoundMn.gI().buttonClick();
			return true;
		}
		curY += 21;

		// 5. Lập Lịch On/Off & 6 Slot Train
		if (py >= curY - 2 && py <= curY + 18)
		{
			if (px >= x + w - 128 && px <= x + w - 45)
			{
				ModSchedule.disconnectOnOffWindow = !ModSchedule.disconnectOnOffWindow;
				ModConfig.SaveConfig();
				GameScr.info1.addInfo("Lịch On/Off ngoài giờ: " + (ModSchedule.disconnectOnOffWindow ? "Tự ngắt kết nối" : "Chỉ dừng Tàn Sát"), 0);
				SoundMn.gI().buttonClick();
				return true;
			}
			if (px >= x + w - 44 && px <= x + w)
			{
				ModSchedule.Toggle();
				SoundMn.gI().buttonClick();
				return true;
			}
		}
		curY += 18;

		int schedW4 = (w - 9) / 4;
		if (py >= curY - 2 && py <= curY + 18)
		{
			if (px >= x && px <= x + schedW4)
			{
				ModSchedule.ApplyPreset(0, 0);
				GameScr.info1.addInfo("Lịch Auto: Cả ngày", 0);
				SoundMn.gI().buttonClick();
				return true;
			}
			if (px >= x + (schedW4 + 3) && px <= x + (schedW4 + 3) + schedW4)
			{
				ModSchedule.ApplyPreset(8 * 60, 18 * 60);
				GameScr.info1.addInfo("Lịch Auto: 08:00 - 18:00", 0);
				SoundMn.gI().buttonClick();
				return true;
			}
			if (px >= x + (schedW4 + 3) * 2 && px <= x + (schedW4 + 3) * 2 + schedW4)
			{
				ModSchedule.ApplyPreset(22 * 60, 6 * 60);
				GameScr.info1.addInfo("Lịch Auto: 22:00 - 06:00", 0);
				SoundMn.gI().buttonClick();
				return true;
			}
			if (px >= x + (schedW4 + 3) * 3 && px <= x + w)
			{
				ModSchedule.ShowInputGlobalWindow();
				SoundMn.gI().buttonClick();
				return true;
			}
		}
		curY += 18;

		int slotW3 = (w - 6) / 3;
		for (int row = 0; row < 2; row++)
		{
			if (py >= curY - 2 && py <= curY + 18)
			{
				for (int col = 0; col < 3; col++)
				{
					int sIdx = row * 3 + col;
					int sx = x + col * (slotW3 + 3);
					if (px >= sx && px <= sx + slotW3)
					{
						ModSchedule.ShowInputTrainSlot(sIdx);
						SoundMn.gI().buttonClick();
						return true;
					}
				}
			}
			curY += (row == 0) ? 18 : 21;
		}

		// 6. Tự Reconnect & Giữ Khu
		if (px >= x + w - 116 && px <= x + w - 45 && py >= curY - 2 && py <= curY + 18)
		{
			ModAutoLogin.ToggleKeepZone();
			return true;
		}
		if (px >= x + w - 44 && px <= x + w && py >= curY - 2 && py <= curY + 18)
		{
			ModAutoLogin.isEnabled = !ModAutoLogin.isEnabled;
			ModConfig.SaveConfig();
			GameScr.info1?.addInfo("Tự Reconnect: " + (ModAutoLogin.isEnabled ? "BẬT" : "TẮT"), 0);
			SoundMn.gI().buttonClick();
			return true;
		}

		return false;
	}
}
