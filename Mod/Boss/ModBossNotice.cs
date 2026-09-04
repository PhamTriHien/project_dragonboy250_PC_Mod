using System;
using System.Collections.Generic;

public static class ModBossNotice
{
	public class BossNoticeEntry
	{
		public string bossName;
		public string mapName;
		public string timeStr;
		public long timestamp;
		public bool isDefeated;
	}

	public static bool isShowBossNotice = true;
	public static readonly List<BossNoticeEntry> listBossNotices = new List<BossNoticeEntry>();
	public const int MAX_BOSS_NOTICES = 6;

	public static readonly string[] KNOWN_BOSSES = new string[]
	{
		"Kuku", "Mập Đầu Đinh", "Rambo", "Tiểu Đội Sát Thủ", "Số 4", "Số 3", "Số 2", "Số 1", "Tiểu Đội Trưởng",
		"Fide Đại Ca", "Fide", "Xên Bọ Hung", "Xên Hoàn Thiện", "Siêu Bọ Hung", "Xên",
		"Android 19", "Android 20", "Android 13", "Android 14", "Android 15", "Android 16", "Android 17", "Android 18",
		"Poc", "Pic", "King Kong", "Broly", "Super Broly",
		"Black Goku", "Zamasu", "Cooler", "Chilled", "Bojack", "Hatchiyack", "Cumber", "Moro", "Granola", "Gas",
		"Birus", "Whis", "Tập Trận", "Dơi Thủ Lĩnh", "Thần Rồng", "Bong Bóng",
		"Mabuu", "Bui Bui", "Yacon", "Dabura", "Dr Lychee", "Cyborg 8",
		"Ninja Áo Tím", "Trung Úy Trắng", "Trung Úy Xanh Lơ", "Đại Úy Sắt", "Trung Úy Thép", "Robot Vệ Sĩ",
		"Chichi", "Bulma", "Videl", "Goku", "Vegeta", "Cadich", "Ma Trơi", "Thỏ Đại Ca", "Pilaf", "Mai", "Shu"
	};

	public static bool IsBossName(string name)
	{
		if (string.IsNullOrEmpty(name)) return false;
		for (int i = 0; i < KNOWN_BOSSES.Length; i++)
		{
			if (name.IndexOf(KNOWN_BOSSES[i], StringComparison.OrdinalIgnoreCase) >= 0)
			{
				return true;
			}
		}
		return false;
	}

	public static void AddBossNotice(string bossName, string mapName, string timeStr, bool isDefeated = false)
	{
		lock (listBossNotices)
		{
			long now = mSystem.currentTimeMillis();

			// Chống trùng lặp thông báo trong 5 giây
			if (listBossNotices.Count > 0)
			{
				BossNoticeEntry latest = listBossNotices[0];
				if (latest != null && latest.bossName.Equals(bossName, StringComparison.OrdinalIgnoreCase) && (now - latest.timestamp < 5000))
				{
					return;
				}
			}

			BossNoticeEntry entry = new BossNoticeEntry
			{
				bossName = bossName,
				mapName = mapName,
				timeStr = timeStr,
				timestamp = now,
				isDefeated = isDefeated
			};

			listBossNotices.Insert(0, entry);
			while (listBossNotices.Count > MAX_BOSS_NOTICES)
			{
				listBossNotices.RemoveAt(listBossNotices.Count - 1);
			}
		}
	}

	public static void ProcessServerBossNotice(string raw)
	{
		try
		{
			if (string.IsNullOrEmpty(raw))
			{
				return;
			}
			string text = raw.Trim();

			// Loại bỏ các tiền tố thông báo máy chủ
			string[] cleanPrefixes = new string[] { "[Thông Báo]", "[Thế Giới]", "[Server]", "[Boss]", "Thông Báo:", "Thế Giới:", "Server:" };
			for (int p = 0; p < cleanPrefixes.Length; p++)
			{
				if (text.StartsWith(cleanPrefixes[p], StringComparison.OrdinalIgnoreCase))
				{
					text = text.Substring(cleanPrefixes[p].Length).Trim();
				}
			}

			string lower = text.ToLower();

			bool isDefeat = lower.Contains("bị tiêu diệt") || lower.Contains("tiêu diệt") || lower.Contains("hạ gục") || lower.Contains("đã chết") || lower.Contains("bi tieu diet");
			bool isAppear = lower.Contains("xuất hiện") || lower.Contains("vừa xuất hiện") || lower.Contains("đã xuất hiện") || lower.Contains("đang ở") || lower.Contains("tại") || lower.Contains("xuat hien") || lower.Contains("vua xuat hien") || lower.Contains("khu vực") || lower.Contains("khu vuc");

			if (!isDefeat && !isAppear && !lower.Contains("boss"))
			{
				return;
			}

			// 1. Tìm tên Boss
			string foundBoss = null;
			for (int i = 0; i < KNOWN_BOSSES.Length; i++)
			{
				if (text.IndexOf(KNOWN_BOSSES[i], StringComparison.OrdinalIgnoreCase) >= 0)
				{
					foundBoss = KNOWN_BOSSES[i];
					break;
				}
			}

			// 2. Nếu chưa tìm thấy nhưng có từ khóa "BOSS / Boss"
			if (string.IsNullOrEmpty(foundBoss))
			{
				int bossIdx = text.IndexOf("BOSS ", StringComparison.OrdinalIgnoreCase);
				if (bossIdx < 0) bossIdx = text.IndexOf("Boss ", StringComparison.OrdinalIgnoreCase);

				if (bossIdx >= 0)
				{
					string afterBoss = text.Substring(bossIdx + 5).Trim();
					string[] stopWords = new string[] { " vừa ", " đã ", " xuất hiện ", " đang ", " tại ", " ở ", " bị ", " tieu diet ", " hạ gục " };
					int stopIdx = -1;
					for (int sw = 0; sw < stopWords.Length; sw++)
					{
						int idx = afterBoss.ToLower().IndexOf(stopWords[sw]);
						if (idx > 0 && (stopIdx == -1 || idx < stopIdx))
						{
							stopIdx = idx;
						}
					}
					if (stopIdx > 0)
					{
						foundBoss = afterBoss.Substring(0, stopIdx).Trim();
					}
					else if (afterBoss.Length > 0 && afterBoss.Length < 25)
					{
						foundBoss = afterBoss;
					}
				}
			}

			if (string.IsNullOrEmpty(foundBoss))
			{
				return;
			}

			// 3. Phân tích địa điểm / bản đồ / trạng thái
			string mapName = isDefeat ? "Đã bị hạ gục!" : "Không rõ map";
			if (!isDefeat)
			{
				string[] splitKeywords = new string[] { " tại ", " ở ", " khu vực ", " toạ độ ", " map ", " tai ", " o " };
				for (int j = 0; j < splitKeywords.Length; j++)
				{
					int idx = lower.IndexOf(splitKeywords[j]);
					if (idx >= 0)
					{
						string sub = text.Substring(idx + splitKeywords[j].Length).Trim();
						int dotIdx = sub.IndexOfAny(new char[] { '.', ',', '!', ';', '\n' });
						if (dotIdx > 0)
						{
							sub = sub.Substring(0, dotIdx).Trim();
						}
						if (!string.IsNullOrEmpty(sub))
						{
							mapName = sub;
							break;
						}
					}
				}
			}

			DateTime now = DateTime.Now;
			string timeStr = string.Format("{0:D2}:{1:D2}:{2:D2}", now.Hour, now.Minute, now.Second);

			AddBossNotice(foundBoss, mapName, timeStr, isDefeat);
		}
		catch
		{
		}
	}

	public static void PaintBossNotice(mGraphics g)
	{
		if (!isShowBossNotice)
		{
			return;
		}

		// Tự động ẩn HUD Boss khi đang mở Hành trang, Menu, Hộp thoại hoặc Bảng Mod
		if ((GameCanvas.panel != null && GameCanvas.panel.isShow) ||
		    (GameCanvas.panel2 != null && GameCanvas.panel2.isShow) ||
		    (GameCanvas.menu != null && GameCanvas.menu.showMenu) ||
		    GameCanvas.currentDialog != null ||
		    ModUI.uiCustomOpen)
		{
			return;
		}

		lock (listBossNotices)
		{
			if (listBossNotices.Count == 0)
			{
				return;
			}

			try
			{
				int startY = 32;
				int lineH = 14;
				int padding = 5;

				int maxTextW = 150;
				for (int i = 0; i < listBossNotices.Count; i++)
				{
					BossNoticeEntry e = listBossNotices[i];
					if (e != null)
					{
						string full = "[" + e.timeStr + "] " + e.bossName + " - " + e.mapName;
						int w = mFont.tahoma_7b_white.getWidth(full);
						if (w > maxTextW)
						{
							maxTextW = w;
						}
					}
				}

				int hudW = maxTextW + padding * 2 + 6;
				int hudH = listBossNotices.Count * lineH + padding * 2 + 13;
				int hudX = GameCanvas.w - hudW - 4;

				// Nền mờ chuẩn game NRO
				g.setColor(0x000000, 0.75f);
				g.fillRect(hudX, startY, hudW, hudH);

				g.setColor(0xff9800);
				g.drawRect(hudX, startY, hudW, hudH);

				mFont.tahoma_7b_yellow.drawString(g, "THÔNG BÁO BOSS", hudX + hudW / 2, startY + 2, mFont.CENTER);

				int drawY = startY + 16;
				for (int j = 0; j < listBossNotices.Count; j++)
				{
					BossNoticeEntry entry = listBossNotices[j];
					if (entry != null)
					{
						string timePart = "[" + entry.timeStr + "] ";
						string bossPart = entry.bossName;
						string mapPart = " - " + entry.mapName;

						mFont.tahoma_7_grey.drawString(g, timePart, hudX + padding, drawY, mFont.LEFT);
						int timeW = mFont.tahoma_7_grey.getWidth(timePart);

						(entry.isDefeated ? mFont.tahoma_7_grey : mFont.tahoma_7b_red).drawString(g, bossPart, hudX + padding + timeW, drawY, mFont.LEFT);
						int bossW = (entry.isDefeated ? mFont.tahoma_7_grey : mFont.tahoma_7b_red).getWidth(bossPart);

						(entry.isDefeated ? mFont.tahoma_7_green2 : mFont.tahoma_7_white).drawString(g, mapPart, hudX + padding + timeW + bossW, drawY, mFont.LEFT);

						drawY += lineH;
					}
				}
			}
			catch
			{
			}
		}
	}
}
