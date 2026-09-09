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
		new CommandInfo("bg", "Mở Quản Lý Hình Nền Phong Cảnh (Git)", "Chat", 0xab47bc),

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
	private static int lastDragY = 0;

	public static void OnMouseScroll(float wheel)
	{
		int listH = 186;
		int itemH = 24;
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
		int listX = uiX + 6;
		int listY = uiY + 24;
		int listW = uiW - 12;
		int listH = uiH - 30;

		int itemH = 24;
		int contentH = commands.Length * itemH + 6;
		int maxScroll = (contentH > listH - 6) ? (contentH - (listH - 6)) : 0;

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
		int listX = uiX + 6;
		int listY = uiY + 24;
		int listW = uiW - 12;
		int listH = uiH - 30;

		// Tiêu đề danh sách
		mFont.tahoma_7b_dark.drawString(g, "LỆNH CHAT & PHÍM TẮT (" + commands.Length + " mục - Chạm kéo trượt):", listX, uiY + 8, mFont.LEFT);

		// Khung chứa danh sách
		GameCanvas.paintz.paintFrameSimple(listX, listY, listW, listH, g);
		g.setColor(15196114);
		g.fillRect(listX + 2, listY + 2, listW - 4, listH - 4);

		int itemH = 24;
		int contentH = commands.Length * itemH + 6;
		int maxScroll = (contentH > listH - 6) ? (contentH - (listH - 6)) : 0;
		if (scrollY > maxScroll) scrollY = maxScroll;
		if (scrollY < 0) scrollY = 0;

		g.setClip(listX + 2, listY + 2, listW - 4, listH - 4);

		for (int i = 0; i < commands.Length; i++)
		{
			CommandInfo info = commands[i];
			int rowY = listY + 4 + i * itemH - scrollY;

			if (rowY + itemH < listY || rowY > listY + listH)
			{
				continue;
			}

			// Nền xen kẽ
			if (i % 2 == 1)
			{
				g.setColor(15724527);
				g.fillRect(listX + 3, rowY, listW - 6, itemH - 1);
			}

			// Tag loại lệnh
			g.setColor(info.tagColor);
			g.fillRect(listX + 6, rowY + 3, 3, 14);

			// Tên lệnh / Phím tắt
			mFont.tahoma_7b_green2.drawString(g, info.cmd, listX + 14, rowY + 3, mFont.LEFT);

			// Nhãn loại
			int tagW = mFont.tahoma_7_grey.getWidth(info.tag);
			mFont.tahoma_7_grey.drawString(g, "[" + info.tag + "]", listX + 85, rowY + 3, mFont.LEFT);

			// Mô tả chức năng
			mFont.tahoma_7_grey.drawString(g, info.desc, listX + 130, rowY + 3, mFont.LEFT);

			// Phím thử ngay
			if (info.tag.Equals("Chat") || info.tag.Equals("Hệ thống"))
			{
				ModUI.PaintNativeButton(listX + listW - 54, rowY + 2, 48, 17, "Thử", false, g);
			}
		}

		g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
	}

	public static bool HandleTap(int px, int py, int uiX, int uiY, int uiW, int uiH)
	{
		if (hasDragged)
		{
			hasDragged = false;
			return false;
		}

		int listX = uiX + 6;
		int listY = uiY + 24;
		int listW = uiW - 12;
		int listH = uiH - 30;

		if (px < listX || px > listX + listW || py < listY || py > listY + listH)
		{
			return false;
		}

		int itemH = 24;

		for (int i = 0; i < commands.Length; i++)
		{
			int rowY = listY + 4 + i * itemH - scrollY;
			if (rowY + itemH < listY || rowY > listY + listH)
			{
				continue;
			}

			int btnX = listX + listW - 54;
			if (px >= btnX && px <= btnX + 48 && py >= rowY + 2 && py <= rowY + 19)
			{
				ExecuteCommand(commands[i].cmd);
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
				GameScr.info1.addInfo("Hiện ID & Tên Item: " + (ModSetActivator.showItemId ? "BẬT" : "TẮT"), 0);
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
				GameScr.info1.addInfo("Hút Đồ Tức Thì: " + (ModDropRate.isInstantPick ? "BẬT" : "TẮT"), 0);
			}
			else if (cmd.Equals("tkrd"))
			{
				GameScr.info1.addInfo("TK Rơi Đồ: " + ModDropRate.totalItemsDropped + "/" + ModDropRate.totalMobsKilled + " (" + ModDropRate.GetDropRatePercent() + "%), " + ModDropRate.GetMobsPerMinute() + " quái/ph", 0);
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
			else if (cmd.Equals("bg"))
			{
				ModUI.selectedTab = 4;
				ModUIBackground.isOpen = true;
				ModUI.uiCustomOpen = true;
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
