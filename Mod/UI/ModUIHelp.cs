using System;
using UnityEngine;

public static class ModUIHelp
{
	public struct CommandInfo
	{
		public string cmd;
		public string desc;
		public string tag;
		public int tagColor;

		public CommandInfo(string cmd, string desc, string tag, int tagColor)
		{
			this.cmd = cmd;
			this.desc = desc;
			this.tag = tag;
			this.tagColor = tagColor;
		}
	}

	public static int scrollY = 0;

	public static readonly CommandInfo[] commands = new CommandInfo[]
	{
		// Nhóm Úp Set & Nhặt Đồ
		new CommandInfo("upset", "Bật / Tắt Auto Úp Set Kích Hoạt", "Chat", 0x00e676),
		new CommandInfo("banrac", "Tự về Urôn bán rác rồi quay lại farm", "Chat", 0x00e676),
		new CommandInfo("iditem", "Bật / Tắt hiện Tên & ID trên ô item", "Chat", 0x00e676),
		new CommandInfo("nhat_id", "Cài đặt lọc ID nhặt (nhập ID)", "Chat", 0x00e676),
		new CommandInfo("nhat_ten", "Cài đặt lọc Tên nhặt (nhập từ khóa)", "Chat", 0x00e676),
		new CommandInfo("xnhat", "Xóa toàn bộ lọc ID và Tên nhặt", "Chat", 0x00e676),
		new CommandInfo("roido", "Bật / Tắt Hút Đồ Tức Thì (Instant Pick)", "Chat", 0x00e676),
		new CommandInfo("tkrd", "Xem thống kê quái diệt & tỉ lệ rơi đồ", "Chat", 0x00e676),

		// Nhóm Boss & Broly
		new CommandInfo("kbroly", "Bật / Tắt Auto Né Broly (Kite áp sát)", "Chat", 0xff9800),
		new CommandInfo("kc", "Bật / Tắt Khinh công Broly (bay lơ lửng)", "Chat", 0xff9800),

		// Nhóm Bùa & Tiện Ích
		new CommandInfo("muabua", "Tự đến Bà Hạt Mít mua bùa đã chọn", "Chat", 0x29b6f6),
		new CommandInfo("autobua", "Bật / Tắt tự động gia hạn bùa khi hết", "Chat", 0x29b6f6),
		new CommandInfo("gb", "Bật / Tắt Auto GoBack về chỗ cũ khi chết", "Chat", 0x29b6f6),
		new CommandInfo("td", "Thu hoạch Đậu Thần trên cây ngay", "Chat", 0x29b6f6),
		new CommandInfo("cd", "Tự động xin đậu / cho đậu trong bang", "Chat", 0x29b6f6),
		new CommandInfo("cde", "Bật / Tắt tự cho đệ tử ăn đậu khi kêu", "Chat", 0x29b6f6),

		// Nhóm Ngôn Ngữ & Dữ Liệu
		new CommandInfo("dich", "Đổi ngôn ngữ Việt Hoá / Gốc Server", "Chat", 0xab47bc),

		// Nhóm Phím Tắt PC
		new CommandInfo("Phím ~ / F2", "Bật / Tắt Giao diện Menu Mod", "Phím", 0xffca28),
		new CommandInfo("F11", "Bật / Tắt Toàn Màn Hình (Fullscreen)", "Phím", 0xffca28),
		new CommandInfo("Phím Home", "Giải kẹt nhân vật khẩn cấp (Unstuck)", "Phím", 0xffca28),
		new CommandInfo("Click Logo", "Bấm Logo TriHienKun để mở Menu Mod", "Chuột", 0x26a69a),
		new CommandInfo("update", "Kiểm tra & Tải cập nhật mới (GitHub)", "Hệ thống", 0x00e676),
	};

	private static bool isDragging = false;
	private static bool hasDragged = false;
	private static int startDragY = 0;
	private static int startScrollY = 0;

	public static void OnMouseScroll(float wheel)
	{
		int listH = 178;
		int itemH = 22;
		int contentH = commands.Length * itemH + 6;
		int maxScroll = (contentH > listH - 6) ? (contentH - (listH - 6)) : 0;
		if (maxScroll <= 0) return;

		int step = 28;
		if (wheel > 0)
		{
			scrollY -= step;
			if (scrollY < 0) scrollY = 0;
		}
		else if (wheel < 0)
		{
			scrollY += step;
			if (scrollY > maxScroll) scrollY = maxScroll;
		}
	}

	public static void UpdateDragScroll(int px, int py, int uiX, int uiY, int uiW, int uiH)
	{
		int listX = uiX + 8;
		int listY = uiY + 24;
		int listW = uiW - 16;
		int listH = 178;

		int itemH = 22;
		int contentH = commands.Length * itemH + 6;
		int maxScroll = (contentH > listH - 6) ? (contentH - (listH - 6)) : 0;

		if (GameCanvas.isPointerJustDown)
		{
			if (px >= listX && px <= listX + listW && py >= listY && py <= listY + listH)
			{
				isDragging = true;
				hasDragged = false;
				startDragY = py;
				startScrollY = scrollY;
			}
		}
		else if (GameCanvas.isPointerDown && isDragging)
		{
			int deltaY = py - startDragY;
			if (Res.abs(deltaY) > 4)
			{
				hasDragged = true;
			}
			if (hasDragged && maxScroll > 0)
			{
				scrollY = startScrollY - deltaY;
				if (scrollY < 0) scrollY = 0;
				if (scrollY > maxScroll) scrollY = maxScroll;
			}
		}
		else if (GameCanvas.isPointerJustRelease)
		{
			isDragging = false;
		}
	}

	public static void Paint(int uiX, int uiY, int uiW, int uiH, mGraphics g)
	{
		int listX = uiX + 8;
		int listY = uiY + 24;
		int listW = uiW - 16;
		int listH = 178;

		// Tiêu đề danh sách & Nút cuộn Lên / Xuống (Vector Triangle sắc nét)
		mFont.tahoma_7b_dark.drawString(g, "LỆNH CHAT & PHÍM TẮT (" + commands.Length + " mục):", listX, uiY + 8, mFont.LEFT);
		ModUI.PaintArrowButton(uiX + uiW - 52, uiY + 5, 20, 16, true, false, g);
		ModUI.PaintArrowButton(uiX + uiW - 28, uiY + 5, 20, 16, false, false, g);

		// Khung chứa danh sách
		GameCanvas.paintz.paintFrameSimple(listX, listY, listW, listH, g);
		g.setColor(15196114);
		g.fillRect(listX + 2, listY + 2, listW - 4, listH - 4);

		int itemH = 22;
		int contentH = commands.Length * itemH + 6;
		int maxScroll = (contentH > listH - 6) ? (contentH - (listH - 6)) : 0;
		if (scrollY > maxScroll) scrollY = maxScroll;
		if (scrollY < 0) scrollY = 0;

		g.setClip(listX + 2, listY + 2, listW - 4, listH - 4);

		for (int i = 0; i < commands.Length; i++)
		{
			int rowY = listY + 4 + i * itemH - scrollY;
			if (rowY + itemH < listY || rowY > listY + listH)
			{
				continue;
			}

			// Màu nền xen kẽ (tông ngà và be sáng NRO)
			g.setColor((i % 2 == 0) ? 15196114 : 15787715);
			g.fillRect(listX + 4, rowY, listW - 8, itemH - 2);

			CommandInfo ci = commands[i];

			// Tag loại lệnh: [Chat], [Phím], [Chuột]
			g.setColor(6702080);
			g.drawRect(listX + 6, rowY + 3, 34, 13);
			mFont.tahoma_7b_dark.drawString(g, ci.tag, listX + 23, rowY + 4, mFont.CENTER);

			// Tên lệnh (màu đậm nổi bật)
			mFont.tahoma_7b_dark.drawString(g, ci.cmd, listX + 46, rowY + 4, mFont.LEFT);

			// Dấu gạch nối và Mô tả chức năng
			int descX = listX + 46 + mFont.tahoma_7b_dark.getWidth(ci.cmd) + 6;
			if (descX < listX + 115) descX = listX + 115;
			mFont.tahoma_7_grey.drawString(g, "- " + ci.desc, descX, rowY + 4, mFont.LEFT);
		}

		g.setClip(0, 0, GameCanvas.w, GameCanvas.h);

		// Thanh cuộn thanh mảnh (Scrollbar NRO)
		if (maxScroll > 0)
		{
			int barH = listH * (listH - 4) / contentH;
			if (barH < 14) barH = 14;
			int barY = listY + 2 + (listH - 4 - barH) * scrollY / maxScroll;
			g.setColor(3847752);
			g.fillRect(listX + listW - 4, barY, 2, barH);
		}

		// Dòng hướng dẫn nhỏ căn giữa ở đáy
		mFont.tahoma_7_grey.drawString(g, "* Bấm vào từng dòng lệnh để kích hoạt nhanh", uiX + uiW / 2, uiY + 208, mFont.CENTER);
	}

	public static bool HandleTap(int px, int py, int uiX, int uiY, int uiW, int uiH)
	{
		int listX = uiX + 8;
		int listY = uiY + 24;
		int listW = uiW - 16;
		int listH = 178;

		int itemH = 22;
		int contentH = commands.Length * itemH + 6;
		int maxScroll = (contentH > listH - 6) ? (contentH - (listH - 6)) : 0;

		// Nếu vừa thực hiện thao tác kéo thả cuộn thì không kích hoạt lệnh nhầm
		if (hasDragged)
		{
			hasDragged = false;
			return true;
		}

		// 1. Nút cuộn lên (Vector Up Triangle)
		if (px >= uiX + uiW - 52 && px <= uiX + uiW - 32 && py >= uiY + 4 && py <= uiY + 22)
		{
			scrollY -= 44;
			if (scrollY < 0) scrollY = 0;
			SoundMn.gI().buttonClick();
			return true;
		}

		// 2. Nút cuộn xuống (Vector Down Triangle)
		if (px >= uiX + uiW - 28 && px <= uiX + uiW - 8 && py >= uiY + 4 && py <= uiY + 22)
		{
			scrollY += 44;
			if (scrollY > maxScroll) scrollY = maxScroll;
			SoundMn.gI().buttonClick();
			return true;
		}

		// 3. Bấm trực tiếp vào dòng lệnh để kích hoạt tức thì (Action Launcher)
		if (px >= listX + 4 && px <= listX + listW - 4 && py >= listY + 2 && py <= listY + listH - 2)
		{
			int clickedIdx = (py - (listY + 4) + scrollY) / itemH;
			if (clickedIdx >= 0 && clickedIdx < commands.Length)
			{
				ExecuteCommand(commands[clickedIdx].cmd);
				SoundMn.gI().buttonClick();
				return true;
			}
		}

		return false;
	}

	private static void ExecuteCommand(string cmd)
	{
		try
		{
			if (cmd.Equals("upset"))
			{
				ModSetActivator.ToggleActive();
			}
			else if (cmd.Equals("banrac"))
			{
				ModSetActivator.StartGoSellJunkNow();
			}
			else if (cmd.Equals("iditem"))
			{
				ModSetActivator.showItemId = !ModSetActivator.showItemId;
				ModConfig.SaveConfig();
				GameScr.info1.addInfo("Hiện Tên & ID Item: " + (ModSetActivator.showItemId ? "BẬT" : "TẮT"), 0);
			}
			else if (cmd.Equals("nhat_id"))
			{
				ModAutoPick.ShowInputFilterId();
			}
			else if (cmd.Equals("nhat_ten"))
			{
				ModAutoPick.ShowInputFilterName();
			}
			else if (cmd.Equals("xnhat"))
			{
				ModAutoPick.ClearFilters();
				ModConfig.SaveConfig();
			}
			else if (cmd.Equals("roido"))
			{
				ModDropRate.isInstantPick = !ModDropRate.isInstantPick;
				ModConfig.SaveConfig();
				GameScr.info1.addInfo("Trick Rơi Đồ (Hút Tức Thì): " + (ModDropRate.isInstantPick ? "BẬT" : "TẮT"), 0);
			}
			else if (cmd.Equals("tkrd"))
			{
				GameScr.info1.addInfo("TK Rơi Đồ: " + ModDropRate.totalItemsDropped + "/" + ModDropRate.totalMobsKilled + " (" + ModDropRate.GetDropRatePercent() + "%), KH: " + ModDropRate.totalSetKHCount + ", Sao: " + ModDropRate.totalStarCount + ", " + ModDropRate.GetMobsPerMinute() + " quái/ph", 0);
			}
			else if (cmd.Equals("kbroly"))
			{
				ModKiteBroly.ToggleAutoKite();
			}
			else if (cmd.Equals("kc"))
			{
				ModKiteBroly.ToggleKhinhCong();
			}
			else if (cmd.Equals("muabua"))
			{
				ModAutoBuyBua.StartBuyBuaNow();
			}
			else if (cmd.Equals("autobua"))
			{
				ModAutoBuyBua.ToggleAutoRebuy();
			}
			else if (cmd.StartsWith("gb"))
			{
				ModGoBack.ToggleGoBack();
			}
			else if (cmd.Equals("td"))
			{
				ModAutoHeal.HarvestMagicTreeNow();
			}
			else if (cmd.Equals("cd"))
			{
				ModAutoHeal.DonateClanNow();
			}
			else if (cmd.Equals("cde"))
			{
				ModAutoHeal.autoFeedPetOnAsk = !ModAutoHeal.autoFeedPetOnAsk;
				ModConfig.SaveConfig();
				GameScr.info1.addInfo("Ăn đậu cho đệ tử: " + (ModAutoHeal.autoFeedPetOnAsk ? "BẬT" : "TẮT"), 0);
			}
			else if (cmd.Equals("dich"))
			{
				ModConfig.isTranslate = !ModConfig.isTranslate;
				ModConfig.SaveConfig();
				ModTranslate.ApplyAllTranslations();
				GameScr.info1.addInfo("Dịch Việt Hoá: " + (ModConfig.isTranslate ? "BẬT" : "TẮT (Gốc Server)"), 0);
			}
			else if (cmd.Contains("F11"))
			{
				ModGraphics.ToggleFullscreen();
			}
			else if (cmd.Contains("Home"))
			{
				ModHotkey.EmergencyUnstuck();
			}
			else if (cmd.Equals("update"))
			{
				ModAutoUpdate.CheckManual();
			}
		}
		catch
		{
		}
	}
}
