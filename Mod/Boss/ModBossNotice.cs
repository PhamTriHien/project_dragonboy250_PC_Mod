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
	}

	public static bool isShowBossNotice = true;
	public static readonly List<BossNoticeEntry> listBossNotices = new List<BossNoticeEntry>();
	public const int MAX_BOSS_NOTICES = 6;

	public static readonly string[] KNOWN_BOSSES = new string[]
	{
		"Kuku", "Mập Đầu Đinh", "Rambo", "Tiểu Đội Sát Thủ", "Số 4", "Số 3", "Số 2", "Số 1", "Tiểu Đội Trưởng",
		"Fide Đại Ca", "Fide", "Xên Bọ Hung", "Xên Hoàn Thiện", "Xên", "Android 19", "Android 20",
		"Android 13", "Android 14", "Android 15", "Poc", "Pic", "King Kong", "Broly", "Super Broly",
		"Black Goku", "Zamasu", "Cooler", "Chilled", "Bojack", "Hatchiyack", "Cumber", "Moro",
		"Birus", "Whis", "Tập Trận", "Dơi Thủ Lĩnh", "Thần Rồng", "Bong Bóng"
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

	public static void AddBossNotice(string bossName, string mapName, string timeStr)
	{
		lock (listBossNotices)
		{
			BossNoticeEntry entry = new BossNoticeEntry
			{
				bossName = bossName,
				mapName = mapName,
				timeStr = timeStr,
				timestamp = mSystem.currentTimeMillis()
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
			string lower = text.ToLower();

			bool isBossAppear = lower.Contains("xuất hiện") || lower.Contains("vừa xuất hiện") || lower.Contains("đã xuất hiện") || lower.Contains("đang ở") || lower.Contains("tại");
			if (!isBossAppear)
			{
				return;
			}

			string foundBoss = null;
			for (int i = 0; i < KNOWN_BOSSES.Length; i++)
			{
				if (text.IndexOf(KNOWN_BOSSES[i], StringComparison.OrdinalIgnoreCase) >= 0)
				{
					foundBoss = KNOWN_BOSSES[i];
					break;
				}
			}

			if (string.IsNullOrEmpty(foundBoss))
			{
				return;
			}

			string mapName = "Không rõ map";
			string[] splitKeywords = new string[] { " tại ", " ở ", " khu vực ", " toạ độ " };
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

			DateTime now = DateTime.Now;
			string timeStr = string.Format("{0:D2}:{1:D2}:{2:D2}", now.Hour, now.Minute, now.Second);

			AddBossNotice(foundBoss, mapName, timeStr);
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

		lock (listBossNotices)
		{
			if (listBossNotices.Count == 0)
			{
				return;
			}

			try
			{
				int startY = 32;
				int lineH = 15;
				int padding = 6;

				int maxTextW = 160;
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

				int hudW = maxTextW + padding * 2;
				int hudH = listBossNotices.Count * lineH + padding * 2 + 14;
				int hudX = GameCanvas.w - hudW - 4;

				g.setColor(0x000000, 0.7f);
				g.fillRect(hudX, startY, hudW, hudH);

				g.setColor(0xff9800);
				g.drawRect(hudX, startY, hudW, hudH);

				mFont.tahoma_7b_yellow.drawString(g, "THÔNG BÁO BOSS", hudX + hudW / 2, startY + 3, mFont.CENTER);

				int drawY = startY + 18;
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

						mFont.tahoma_7b_red.drawString(g, bossPart, hudX + padding + timeW, drawY, mFont.LEFT);
						int bossW = mFont.tahoma_7b_red.getWidth(bossPart);

						mFont.tahoma_7_white.drawString(g, mapPart, hudX + padding + timeW + bossW, drawY, mFont.LEFT);

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
