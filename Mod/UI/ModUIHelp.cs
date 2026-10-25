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
		new CommandInfo("upset", "Bật / Tắt Auto Úp Set Kích Hoạt", "Chat", 0x00e676),
		new CommandInfo("banrac", "Tự về shop bán rác rồi quay lại farm", "Chat", 0x00e676),
		new CommandInfo("iditem", "Bật / Tắt hiện Tên & ID trên ô item", "Chat", 0x00e676),
		new CommandInfo("nhat_id", "Cài đặt lọc ID nhặt (nhập ID)", "Chat", 0x00e676),
		new CommandInfo("nhat_ten", "Cài đặt lọc Tên nhặt (nhập từ khóa)", "Chat", 0x00e676),
		new CommandInfo("xnhat", "Xóa toàn bộ lọc ID và Tên nhặt", "Chat", 0x00e676),
		new CommandInfo("roido", "Bật / Tắt Hút Đồ Tức Thì (Instant Pick)", "Chat", 0x00e676),
		new CommandInfo("tkrd", "Xem thống kê quái diệt & tỉ lệ rơi đồ", "Chat", 0x00e676),
		new CommandInfo("kbroly", "Bật / Tắt Auto Né Broly (Kite áp sát)", "Chat", 0xff9800),
		new CommandInfo("kc", "Bật / Tắt Khinh công Broly (bay lơ lửng)", "Chat", 0xff9800),
		new CommandInfo("muabua", "Tự đến Bà Hạt Mít mua bùa đã chọn", "Chat", 0x29b6f6),
		new CommandInfo("autobua", "Bật / Tắt tự động gia hạn bùa khi hết", "Chat", 0x29b6f6),
		new CommandInfo("gb", "Bật / Tắt Auto GoBack về chỗ cũ khi chết", "Chat", 0x29b6f6),
		new CommandInfo("nextmap=...", "safe:ID (an toàn) hoặc fast:ID (nhanh)", "Chat", 0x29b6f6),
		new CommandInfo("stopmap", "Dừng Next Map đang chạy", "Chat", 0x29b6f6),
		new CommandInfo("td", "Thu hoạch Đậu Thần trên cây ngay", "Chat", 0x29b6f6),
		new CommandInfo("cd", "Tự động xin đậu / cho đậu trong bang", "Chat", 0x29b6f6),
		new CommandInfo("cde", "Bật / Tắt tự cho đệ tử ăn đậu khi kêu", "Chat", 0x29b6f6),
		new CommandInfo("dich", "Đổi ngôn ngữ Việt Hoá / Gốc Server", "Chat", 0xab47bc),
		new CommandInfo("bg", "Mở Quản Lý Hình Nền Phong Cảnh (Git)", "Chat", 0xab47bc),
		new CommandInfo("autochat", "Bật/tắt Auto Chat; autochat=Nội dung|ms", "Chat", 0x00e676),
		new CommandInfo("boxsort", "Sắp xếp rương ngay", "Chat", 0x00e676),
		new CommandInfo("autoboxsort", "Bật/tắt tự động sắp xếp rương", "Chat", 0x00e676),
		new CommandInfo("autopoint", "Bật/tắt Auto cộng điểm tiềm năng", "Chat", 0x00e676),
		new CommandInfo("autofreeze", "Bật/tắt đóng băng hồi chiêu; autofreeze=ID theo chiêu", "Chat", 0x00e676),
		new CommandInfo("autoitem=...", "Cấu hình: autoitem=ID1,ID2|chu_kỳ_ms", "Chat", 0x00e676),
		new CommandInfo("schedule=...", "Hẹn giờ: schedule=08:00-18:00", "Chat", 0x00e676),
		new CommandInfo("autorevive", "Bật/tắt tự hồi sinh bằng ngọc", "Chat", 0xff9800),
		new CommandInfo("autopet=...", "Cấu hình: autopet=mã_trạng_thái|ms", "Chat", 0xff9800),
		new CommandInfo("autofusion", "Cấu hình: autofusion=type|chu_kỳ_ms", "Chat", 0xff9800),
		new CommandInfo("set1 / set2 / xoaset", "Mặc nhanh Set Đồ 1 / Set Đồ 2 / Xóa Set", "Chat", 0x00e676),
		new CommandInfo("xoanguoi / ttsp", "Xóa người chơi khác / Bật HUD Sư Phụ", "Chat", 0x29b6f6),
		new CommandInfo("tusat / captcha", "Tự sát nhanh / Bật tắt Auto Giải Captcha", "Chat", 0xff9800),
		new CommandInfo("Phím ~ / F2", "Bật / Tắt Giao diện Menu Mod", "Phím", 0xffca28),
		new CommandInfo("F11", "Bật / Tắt Toàn Màn Hình (Fullscreen)", "Phím", 0xffca28),
		new CommandInfo("Phím Home", "Giải kẹt nhân vật khẩn cấp (Unstuck)", "Phím", 0xffca28),
		new CommandInfo("Phím E / N / Z", "Tự đánh / Auto Nhặt / Bật tắt Cờ đen", "Phím", 0xffca28),
		new CommandInfo("Phím C / F / T", "Dùng Capsule / Bông tai / Tự Động Luyện Tập", "Phím", 0xffca28),
		new CommandInfo("Phím J / K / L", "Qua nhanh cổng Map Trái / Giữa / Phải", "Phím", 0xffca28),
		new CommandInfo("Phím B / G / I / M / X", "Bạn bè / Giao dịch / Khóa mục tiêu / Khu / Menu Mod", "Phím", 0xffca28),
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
		int itemH = 20;
		int contentH = commands.Length * itemH + 4;
		int maxScroll = (contentH > listH - 4) ? (contentH - (listH - 4)) : 0;
		if (maxScroll <= 0) return;

		int step = 24;
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
		int listX = uiX + 5;
		int listY = uiY + 20;
		int listW = uiW - 10;
		int listH = uiH - 24;
		int itemH = 20;
		int contentH = commands.Length * itemH + 4;
		int maxScroll = (contentH > listH - 4) ? (contentH - (listH - 4)) : 0;

		if (Input.GetMouseButton(0) || GameCanvas.isPointerDown)
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
		mFont.tahoma_7_grey.drawString(g, "Gõ lệnh vào khung Chat (Enter) hoặc bấm nút [Thử] trực tiếp:", uiX + 6, uiY + 5, mFont.LEFT);

		int listX = uiX + 5;
		int listY = uiY + 20;
		int listW = uiW - 10;
		int listH = uiH - 24;

		GameCanvas.paintz.paintFrameSimple(listX, listY, listW, listH, g);
		g.setColor(15196114);
		g.fillRect(listX + 2, listY + 2, listW - 4, listH - 4);

		int itemH = 20;
		int contentH = commands.Length * itemH + 4;
		int maxScroll = (contentH > listH - 4) ? (contentH - (listH - 4)) : 0;
		if (scrollY > maxScroll) scrollY = maxScroll;
		if (scrollY < 0) scrollY = 0;

		g.setClip(listX + 2, listY + 2, listW - 4, listH - 4);

		for (int i = 0; i < commands.Length; i++)
		{
			CommandInfo info = commands[i];
			int rowY = listY + 3 + i * itemH - scrollY;
			if (rowY + itemH < listY || rowY > listY + listH) continue;

			if (i % 2 == 1)
			{
				g.setColor(15724527);
				g.fillRect(listX + 3, rowY, listW - 6, itemH - 1);
			}

			g.setColor(info.tagColor);
			g.fillRect(listX + 5, rowY + 3, 3, 12);

			mFont.tahoma_7_green2.drawString(g, info.cmd, listX + 12, rowY + 2, mFont.LEFT);
			mFont.tahoma_7_grey.drawString(g, "[" + info.tag + "]", listX + 78, rowY + 2, mFont.LEFT);
			mFont.tahoma_7_grey.drawString(g, info.desc, listX + 122, rowY + 2, mFont.LEFT);

			if (info.tag.Equals("Chat") || info.tag.Equals("Hệ thống"))
			{
				ModUI.PaintNativeButton(listX + listW - 46, rowY + 2, 40, 15, "Thử", false, g);
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

		int listX = uiX + 5;
		int listY = uiY + 20;
		int listW = uiW - 10;
		int listH = uiH - 24;
		if (px < listX || px > listX + listW || py < listY || py > listY + listH) return false;

		int itemH = 20;
		for (int i = 0; i < commands.Length; i++)
		{
			int rowY = listY + 3 + i * itemH - scrollY;
			if (rowY + itemH < listY || rowY > listY + listH) continue;

			int btnX = listX + listW - 46;
			if (px >= btnX && px <= btnX + 42 && py >= rowY + 1 && py <= rowY + 18)
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
			if (cmd.Equals("upset")) ModSetActivator.ToggleActive();
			else if (cmd.Equals("banrac")) ModSetActivator.StartGoSellJunkNow();
			else if (cmd.Equals("iditem"))
			{
				ModSetActivator.showItemId = !ModSetActivator.showItemId;
				ModConfig.SaveConfig();
				GameScr.info1.addInfo("Hiện ID & Tên Item: " + (ModSetActivator.showItemId ? "BẬT" : "TẮT"), 0);
			}
			else if (cmd.StartsWith("nhat_id")) ModAutoPick.ShowInputFilterId();
			else if (cmd.StartsWith("nhat_ten")) ModAutoPick.ShowInputFilterName();
			else if (cmd.Equals("xnhat")) { ModAutoPick.ClearFilters(); ModConfig.SaveConfig(); }
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
			else if (cmd.Equals("kbroly")) ModKiteBroly.ToggleAutoKite();
			else if (cmd.Equals("kc")) ModKiteBroly.ToggleKhinhCong();
			else if (cmd.Equals("muabua")) ModAutoBuyBua.StartBuyBuaNow();
			else if (cmd.Equals("autobua")) ModAutoBuyBua.ToggleAutoRebuy();
			else if (cmd.StartsWith("gb")) ModGoBack.ToggleGoBack();
			else if (cmd.StartsWith("nextmap=")) { ModUI.selectedTab = 7; ModUI.uiCustomOpen = true; }
			else if (cmd.Equals("stopmap")) ModNextMap.StopNextMap();
			else if (cmd.Equals("td")) ModAutoHeal.HarvestMagicTreeNow();
			else if (cmd.Equals("cd")) ModAutoHeal.DonateClanNow();
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
			else if (cmd.Equals("bg")) { ModUI.selectedTab = 5; ModUIBackground.isOpen = true; ModUI.uiCustomOpen = true; }
			else if (cmd.StartsWith("autochat")) ModAutoChat.Toggle();
			else if (cmd.Equals("boxsort")) { ModAutoBoxSort.SortNow(); GameScr.info1.addInfo("Đã gửi lệnh sắp xếp rương.", 0); }
			else if (cmd.Equals("autoboxsort")) ModAutoBoxSort.Toggle();
			else if (cmd.StartsWith("autopoint")) ModAutoPoint.Toggle();
			else if (cmd.Equals("autofreeze"))
			{
				ModTanSat.freezeSkillCooldown = !ModTanSat.freezeSkillCooldown;
				ModConfig.SaveConfig();
				GameScr.info1.addInfo("Đóng băng hồi chiêu tất cả chiêu: " + (ModTanSat.freezeSkillCooldown ? "BẬT" : "TẮT"), 0);
			}
			else if (cmd.StartsWith("autoitem"))
			{
				if (ModAutoItem.itemIds.Count == 0) ModAutoItem.ShowInputItemIds();
				else ModAutoItem.Toggle();
			}
			else if (cmd.StartsWith("schedule")) ModSchedule.Toggle();
			else if (cmd.Equals("autorevive")) ModAutoRevive.Toggle();
			else if (cmd.StartsWith("autopet")) ModAutoPet.Toggle();
			else if (cmd.StartsWith("autofusion")) ModAutoFusion.Toggle();
			else if (cmd.StartsWith("set1")) ModInventoryExt.EquipSet(1);
			else if (cmd.StartsWith("xoanguoi")) ModGraphics.ToggleHideOtherPlayers();
			else if (cmd.StartsWith("tusat")) ModSelfKill.Toggle();
			else if (cmd.Contains("F11")) ModGraphics.ToggleFullscreen();
			else if (cmd.Contains("Home")) ModHotkey.EmergencyUnstuck();
			else if (cmd.Equals("update")) ModAutoUpdate.CheckManual();
		}
		catch
		{
		}
	}
}
