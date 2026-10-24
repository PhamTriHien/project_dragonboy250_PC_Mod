using System;

/// <summary>
/// CLASS ModUIAutomation: Định nghĩa và điều phối logic trong hệ thống game.
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
		return (subTab == 0) ? 256 : 224;
	}

	/// <summary>
	/// Bộ xử lý sự kiện tiếp nhận và phản hồi OnMouseScroll.
	/// </summary>
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

	/// <summary>
	/// Cập nhật trạng thái logic của UpdateDragScroll theo chu kỳ khung hình.
	/// </summary>
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

	/// <summary>
	/// Thực hiện vẽ toàn bộ đối tượng và các lớp hiển thị lên canvas mGraphics.
	/// </summary>
	public static void Paint(int uiX, int uiY, int uiW, int uiH, mGraphics g)
	{
		// Hàng 0: 2 Sub-Tabs (1. Nhân Vật & Item / 2. Đệ Tử & Tiện Ích)
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

	/// <summary>
	/// Thực hiện vẽ và hiển thị thành phần PaintSubTabPlayer lên canvas đồ họa.
	/// </summary>
	private static void PaintSubTabPlayer(int x, int y, int w, int h, mGraphics g)
	{
		int curY = y;

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
			bool isSel = (ModAutoPoint.targetType == i);
			ModUI.PaintNativeButton(bx, curY, btnW5, 16, statNames[i], isSel, g);
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
		curY += 24;

		// 2. AUTO DÙNG ITEM TRONG TÚI
		mFont.tahoma_7_green2.drawString(g, "AUTO DÙNG ITEM (TÚI):", x + 2, curY + 2, mFont.LEFT);
		ModUI.PaintNativeButton(x + w - 114, curY, 68, 16, ModAutoItem.autoOpenCskb ? "CSKB: BẬT" : "CSKB: TẮT", ModAutoItem.autoOpenCskb, g);
		ModUI.PaintNativeButton(x + w - 42, curY, 40, 16, ModAutoItem.isEnabled ? "BẬT" : "TẮT", ModAutoItem.isEnabled, g);
		curY += 18;

		// Lưới 12 nút chọn nhanh item có thời gian sử dụng (Cuồng nộ, Bổ huyết, Bổ khí, Giáp xên, Ẩn danh, Đồ ăn, Hải sản, Máy dò...)
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
		curY += 24;

		// 3. AUTO HỢP THỂ (FUSION)
		mFont.tahoma_7_green2.drawString(g, "AUTO HỢP THỂ (FUSION):", x + 2, curY + 2, mFont.LEFT);
		ModUI.PaintNativeButton(x + w - 42, curY, 40, 16, ModAutoFusion.isEnabled ? "BẬT" : "TẮT", ModAutoFusion.isEnabled, g);
		curY += 19;

		int fuseW = (w - 3) / 2;
		ModUI.PaintNativeButton(x, curY, fuseW, 16, "Bông Tai Porata", ModAutoFusion.fusionType == 4, g);
		ModUI.PaintNativeButton(x + fuseW + 3, curY, fuseW, 16, "Kaio-ken / Thường", ModAutoFusion.fusionType == 6, g);
		curY += 21;

		// 4. AUTO NHIỆM VỤ TÂN THỦ (TASK 0 -> 10)
		mFont.tahoma_7_green2.drawString(g, "AUTO NV TÂN THỦ (0->10):", x + 2, curY + 2, mFont.LEFT);
		ModUI.PaintNativeButton(x + w - 42, curY, 40, 16, ModAutoNewAccount.isEnabled ? "BẬT" : "TẮT", ModAutoNewAccount.isEnabled, g);
	}

	/// <summary>
	/// Thực hiện vẽ và hiển thị thành phần PaintSubTabUtilities lên canvas đồ họa.
	/// </summary>
	private static void PaintSubTabUtilities(int x, int y, int w, int h, mGraphics g)
	{
		int curY = y;

		// 1. AUTO ĐỆ TỬ (PET)
		mFont.tahoma_7_green2.drawString(g, "AUTO LỆNH ĐỆ TỬ:", x + 2, curY + 2, mFont.LEFT);
		ModUI.PaintNativeButton(x + w - 122, curY, 76, 16, ModAutoPet.GetPetPowerLimitLabel(), ModAutoPet.exitOnPetPowerLimit > 0, g);
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

		// Hàng tính năng nâng cao của Đệ Tử: HUD Đệ, Né Boss, Cờ Đen, Trị Thương
		ModUI.PaintNativeButton(x, curY, petBtnW, 16, "HUD Đệ", ModAutoPet.showPetHUD, g);
		ModUI.PaintNativeButton(x + (petBtnW + 3), curY, petBtnW, 16, "Né Boss", ModAutoPet.petAvoidBoss, g);
		ModUI.PaintNativeButton(x + (petBtnW + 3) * 2, curY, petBtnW, 16, "Cờ Đen", ModAutoPet.petAutoFlag, g);
		ModUI.PaintNativeButton(x + (petBtnW + 3) * 3, curY, petBtnW, 16, "Trị Thương", ModAutoPet.autoNamekHeal, g);
		curY += 21;

		// 2. AUTO CHAT
		mFont.tahoma_7_green2.drawString(g, "AUTO CHAT ĐỊNH KỲ:", x + 2, curY + 2, mFont.LEFT);
		ModUI.PaintNativeButton(x + w - 42, curY, 40, 16, ModAutoChat.isEnabled ? "BẬT" : "TẮT", ModAutoChat.isEnabled, g);
		curY += 18;

		string msgPreview = string.IsNullOrEmpty(ModAutoChat.message) ? "(Chưa đặt nội dung)" : ("\"" + ModAutoChat.message + "\"");
		if (msgPreview.Length > 24) msgPreview = msgPreview.Substring(0, 21) + "...\"";
		mFont.tahoma_7_grey.drawString(g, msgPreview, x + 2, curY + 3, mFont.LEFT);
		ModUI.PaintNativeButton(x + w - 74, curY, 72, 16, "Đổi Lời Chat", false, g);
		curY += 18;

		mFont.tahoma_7_grey.drawString(g, "Chu kỳ chat:", x + 2, curY + 3, mFont.LEFT);
		int[] chatIntervals = new int[] { 10000, 30000, 60000, 120000 };
		string[] chatIntvNames = new string[] { "10s", "30s", "1m", "2m" };
		int intvW = (w - 76) / 4;
		for (int i = 0; i < 4; i++)
		{
			int cx = x + 70 + i * (intvW + 2);
			bool isSelInt = (ModAutoChat.intervalMs == chatIntervals[i]);
			ModUI.PaintNativeButton(cx, curY, intvW, 16, chatIntvNames[i], isSelInt, g);
		}
		curY += 21;

		// 3. SẮP XẾP RƯƠNG
		mFont.tahoma_7_green2.drawString(g, "SẮP XẾP RƯƠNG ĐỒ:", x + 2, curY + 2, mFont.LEFT);
		ModUI.PaintNativeButton(x + w - 42, curY, 40, 16, ModAutoBoxSort.isEnabled ? "BẬT" : "TẮT", ModAutoBoxSort.isEnabled, g);
		ModUI.PaintNativeButton(x + w - 128, curY, 82, 16, "Sắp Xếp Ngay", false, g);
		curY += 21;

		// 4. LẬP LỊCH AUTO
		mFont.tahoma_7_green2.drawString(g, "LẬP LỊCH KHUNG GIỜ:", x + 2, curY + 2, mFont.LEFT);
		ModUI.PaintNativeButton(x + w - 114, curY, 68, 16, "Hẹn Map/Khu", !string.IsNullOrEmpty(ModSchedule.trainSlotsRaw[0]), g);
		ModUI.PaintNativeButton(x + w - 42, curY, 40, 16, ModSchedule.isEnabled ? "BẬT" : "TẮT", ModSchedule.isEnabled, g);
		curY += 18;

		int schedW = (w - 6) / 3;
		bool isAllDay = (ModSchedule.startMinute == ModSchedule.endMinute);
		bool isDay = (ModSchedule.startMinute == 8 * 60 && ModSchedule.endMinute == 18 * 60);
		bool isNight = (ModSchedule.startMinute == 22 * 60 && ModSchedule.endMinute == 6 * 60);
		ModUI.PaintNativeButton(x, curY, schedW, 16, "Cả Ngày", isAllDay, g);
		ModUI.PaintNativeButton(x + schedW + 3, curY, schedW, 16, "08h - 18h", isDay, g);
		ModUI.PaintNativeButton(x + (schedW + 3) * 2, curY, schedW, 16, "22h - 06h", isNight, g);
		curY += 21;

		// 5. TỰ KẾT NỐI LẠI (AUTO RECONNECT) & GIỮ KHU
		mFont.tahoma_7_green2.drawString(g, "RECONNECT & GIỮ KHU:", x + 2, curY + 2, mFont.LEFT);
		ModUI.PaintNativeButton(x + w - 114, curY, 68, 16, ModAutoLogin.keepSavedZone ? ("Giữ K." + ModAutoLogin.savedKeepZoneId) : "Giữ Khu: Tắt", ModAutoLogin.keepSavedZone, g);
		ModUI.PaintNativeButton(x + w - 42, curY, 40, 16, ModAutoLogin.isEnabled ? "BẬT" : "TẮT", ModAutoLogin.isEnabled, g);
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

		int subTabW = (uiW - 12) / 2;
		int row0Y = uiY + 4;
		int row0H = 18;

		// Chuyển Sub-Tab
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

	/// <summary>
	/// Xử lý nghiệp vụ logic chuyên trách cho HandleTapPlayer.
	/// </summary>
	private static bool HandleTapPlayer(int px, int py, int x, int y, int w)
	{
		int curY = y;

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

		// Chọn chỉ số (HP, KI, Sức Đánh, Giáp, Chí Mạng)
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
		curY += 24;

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

		// Lưới 12 nút chọn nhanh item có thời gian sử dụng
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

		// Điều kiện dùng item
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

		// Ngưỡng HP/MP hoặc Chu kỳ
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

		// Nút Xóa ID & Nhập ID Item
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
		curY += 24;

		// 3. Auto Hợp Thể: Bật / Tắt
		if (px >= x + w - 44 && px <= x + w && py >= curY - 2 && py <= curY + 18)
		{
			ModAutoFusion.Toggle();
			SoundMn.gI().buttonClick();
			return true;
		}
		curY += 19;

		// Loại hợp thể (Porata / Kaio-ken)
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

		return false;
	}

	/// <summary>
	/// Xử lý nghiệp vụ logic chuyên trách cho HandleTapUtilities.
	/// </summary>
	private static bool HandleTapUtilities(int px, int py, int x, int y, int w)
	{
		int curY = y;

		// 1. Auto Pet: Mốc SM Đệ & Bật / Tắt
		if (px >= x + w - 124 && px <= x + w - 45 && py >= curY - 2 && py <= curY + 18)
		{
			ModAutoPet.CyclePetPowerLimit();
			GameScr.info1?.addInfo("Đệ Tử - " + ModAutoPet.GetPetPowerLimitLabel(), 0);
			return true;
		}
		if (px >= x + w - 44 && px <= x + w && py >= curY - 2 && py <= curY + 18)
		{
			ModAutoPet.Toggle();
			SoundMn.gI().buttonClick();
			return true;
		}
		curY += 18;

		// Lệnh Pet (Đi theo, Bảo vệ, Tấn công, Về nhà)
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

		// Hàng tính năng nâng cao của Đệ Tử: HUD Đệ, Né Boss, Cờ Đen, Trị Thương
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
		curY += 21;

		// 2. Auto Chat: Bật / Tắt
		if (px >= x + w - 44 && px <= x + w && py >= curY - 2 && py <= curY + 18)
		{
			ModAutoChat.Toggle();
			SoundMn.gI().buttonClick();
			return true;
		}
		curY += 18;

		// Nút Đổi Lời Chat
		if (px >= x + w - 76 && px <= x + w && py >= curY - 2 && py <= curY + 18)
		{
			ModAutoChat.ShowInputChatMessage();
			return true;
		}
		curY += 18;

		// Chu kỳ chat
		int[] chatIntervals = new int[] { 10000, 30000, 60000, 120000 };
		int intvW = (w - 76) / 4;
		for (int i = 0; i < 4; i++)
		{
			int cx = x + 70 + i * (intvW + 2);
			if (px >= cx && px <= cx + intvW && py >= curY - 2 && py <= curY + 18)
			{
				ModAutoChat.Configure(ModAutoChat.message, chatIntervals[i]);
				SoundMn.gI().buttonClick();
				return true;
			}
		}
		curY += 21;

		// 3. Sắp Xếp Rương: Bật / Tắt & Sắp Xếp Ngay
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

		// 4. Lập Lịch Auto: Hẹn Map/Khu, Bật / Tắt & Presets
		if (px >= x + w - 116 && px <= x + w - 45 && py >= curY - 2 && py <= curY + 18)
		{
			ModUI.uiCustomOpen = false;
			ModSchedule.ShowInputTrainSlot(0);
			SoundMn.gI().buttonClick();
			return true;
		}
		if (px >= x + w - 44 && px <= x + w && py >= curY - 2 && py <= curY + 18)
		{
			ModSchedule.Toggle();
			SoundMn.gI().buttonClick();
			return true;
		}
		curY += 18;

		int schedW = (w - 6) / 3;
		if (px >= x && px <= x + schedW && py >= curY - 2 && py <= curY + 18)
		{
			ModSchedule.ApplyPreset(0, 0);
			GameScr.info1.addInfo("Lịch Auto: Cả ngày", 0);
			SoundMn.gI().buttonClick();
			return true;
		}
		if (px >= x + schedW + 3 && px <= x + (schedW + 3) * 2 && py >= curY - 2 && py <= curY + 18)
		{
			ModSchedule.ApplyPreset(8 * 60, 18 * 60);
			GameScr.info1.addInfo("Lịch Auto: 08:00 - 18:00", 0);
			SoundMn.gI().buttonClick();
			return true;
		}
		if (px >= x + (schedW + 3) * 2 && px <= x + w && py >= curY - 2 && py <= curY + 18)
		{
			ModSchedule.ApplyPreset(22 * 60, 6 * 60);
			GameScr.info1.addInfo("Lịch Auto: 22:00 - 06:00", 0);
			SoundMn.gI().buttonClick();
			return true;
		}
		curY += 21;

		// 5. Tự Reconnect & Giữ Khu: Bật / Tắt
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
