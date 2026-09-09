using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

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
	public static int hudStartY = 70;
	public static readonly List<BossNoticeEntry> listBossNotices = new List<BossNoticeEntry>();
	public const int MAX_BOSS_NOTICES = 6;

	public static readonly string[] KNOWN_BOSSES = new string[]
	{
		"Kuku", "Mập Đầu Đinh", "Rambo", "Tiểu Đội Sát Thủ", "Số 4", "Số 3", "Số 2", "Số 1", "Tiểu Đội Trưởng",
		"Fide Đại Ca", "Fide", "Frieza", "Super Frieza", "Xên Bọ Hung", "Xên Hoàn Thiện", "Siêu Bọ Hung", "Xên",
		"Android 19", "Android 20", "Android 13", "Android 14", "Android 15", "Android 16", "Android 17", "Android 18",
		"Poc", "Pic", "King Kong", "Broly", "Super Broly",
		"Black Goku", "Zamasu", "Cooler", "Chilled", "Bojack", "Hatchiyack", "Cumber", "Moro", "Granola", "Gas",
		"Cell", "Super Cell", "Perfect Cell", "Majin Buu", "Kid Buu", "Evil Buu",
		"Birus", "Whis", "Tập Trận", "Dơi Thủ Lĩnh", "Thần Rồng", "Bong Bóng",
		"Mabuu", "Bui Bui", "Yacon", "Dabura", "Dr Lychee", "Cyborg 8",
		"Ninja Áo Tím", "Trung Úy Trắng", "Trung Úy Xanh Lơ", "Đại Úy Sắt", "Trung Úy Thép", "Robot Vệ Sĩ",
		"Chichi", "Bulma", "Videl", "Goku", "Vegeta", "Cadich", "Ma Trơi", "Thỏ Đại Ca", "Pilaf", "Mai", "Shu",
		"Số 4 Thần Nami", "Số 3 Thần Lam", "Số 2 Tầm Trụ", "Super Baby", "Baby", "Janemba", "Hildegarn", "Cui", "Dodoria", "Zarbon"
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

			// Kiểm tra trùng lặp và cập nhật trạng thái tức thời 0ms
			for (int d = 0; d < listBossNotices.Count; d++)
			{
				BossNoticeEntry old = listBossNotices[d];
				if (old != null && old.bossName.Equals(bossName, StringComparison.OrdinalIgnoreCase))
				{
					// Nếu boss chuyển trạng thái (xuất hiện -> bị tiêu diệt): Cập nhật tức thời không delay
					if (old.isDefeated != isDefeated)
					{
						old.isDefeated = isDefeated;
						old.timestamp = now;
						old.timeStr = timeStr;
						if (!string.IsNullOrEmpty(mapName) && !mapName.Equals("Không rõ map"))
						{
							old.mapName = mapName;
						}
						return;
					}

					// Bỏ qua gói tin trùng lặp cùng trạng thái gửi dồn từ nhiều kênh trong 3 giây
					if (now - old.timestamp < 3000)
					{
						return;
					}
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

	public static void LogBossDebug(string verdict, string raw)
	{
		try
		{
			string path;
			try
			{
				path = Path.Combine(Application.dataPath, "../boss_debug.log");
			}
			catch
			{
				path = "boss_debug.log";
			}
			try
			{
				if (File.Exists(path) && new FileInfo(path).Length > 30720)
				{
					File.Delete(path);
				}
			}
			catch
			{
			}
			File.AppendAllText(path, DateTime.Now.ToString("HH:mm:ss") + " [" + verdict + "] " + raw + "\r\n");
		}
		catch
		{
		}
	}

	private static bool IsBossCandidate(string lower)
	{
		try
		{
			return lower.Contains("boss") || lower.Contains("bos ") || lower.Contains("muncul")
				|| lower.Contains("mati") || lower.Contains("kalah") || lower.Contains("bunuh")
				|| lower.Contains("appear") || lower.Contains("spawn") || lower.Contains("defeat")
				|| lower.Contains("slain") || lower.Contains("kill") || lower.Contains("xuat")
				|| lower.Contains("hien") || lower.Contains("tieu") || lower.Contains("diet")
				|| lower.Contains("tiêu") || lower.Contains("diệt") || lower.Contains("xuất")
				|| lower.Contains("hiện") || lower.Contains("hạ") || lower.Contains("gục");
		}
		catch
		{
			return false;
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

			// 1. Vòng lặp bóc sạch tiền tố: dấu "!", mã màu |0|-|9| hoặc |...|, và các tag vuông [...]
			string[] cleanPrefixes = new string[] 
			{ 
				"[Thông Báo]", "[Thế Giới]", "[Server]", "[Boss]", "Thông Báo:", "Thế Giới:", "Server:", 
				"[Pemberitahuan]", "[Pengumuman]", "[Dunia]", "[Announcement]", "[World]", 
				"Pemberitahuan:", "Pengumuman:", "Announcement:", "Notice:", "[Notice]", "[SYSTEM]", "[Hệ Thống]" 
			};
			bool cleaned = true;
			while (cleaned)
			{
				cleaned = false;
				text = text.Trim();
				if (text.StartsWith("!"))
				{
					text = text.Substring(1).Trim();
					cleaned = true;
				}
				if (text.StartsWith("|"))
				{
					int pipeIdx = text.IndexOf('|', 1);
					if (pipeIdx > 0 && pipeIdx <= 6)
					{
						text = text.Substring(pipeIdx + 1).Trim();
						cleaned = true;
					}
				}
				for (int p = 0; p < cleanPrefixes.Length; p++)
				{
					if (text.StartsWith(cleanPrefixes[p], StringComparison.OrdinalIgnoreCase))
					{
						text = text.Substring(cleanPrefixes[p].Length).Trim();
						cleaned = true;
					}
				}
				if (text.StartsWith("["))
				{
					int closeIdx = text.IndexOf(']');
					if (closeIdx > 0 && closeIdx <= 24)
					{
						text = text.Substring(closeIdx + 1).Trim();
						cleaned = true;
					}
				}
			}

			string lower = text.ToLower();

			bool isDefeat = lower.Contains("bị tiêu diệt") || lower.Contains("tiêu diệt") || lower.Contains("hạ gục") || lower.Contains("đã chết") || lower.Contains("bi tieu diet")
				|| lower.Contains("dikalahkan") || lower.Contains("telah mati") || lower.Contains("dibunuh")
				|| lower.Contains("defeated") || lower.Contains("slain") || lower.Contains("killed");
			bool isAppear = lower.Contains("xuất hiện") || lower.Contains("vừa xuất hiện") || lower.Contains("đã xuất hiện") || lower.Contains("đang ở") || lower.Contains("tại") || lower.Contains("xuat hien") || lower.Contains("vua xuat hien") || lower.Contains("khu vực") || lower.Contains("khu vuc")
				|| lower.Contains("muncul") || lower.Contains("berada di")
				|| lower.Contains("appeared") || lower.Contains("spawned");

			if (!isDefeat && !isAppear && !lower.Contains("boss") && !lower.Contains("bos "))
			{
				if (IsBossCandidate(lower))
				{
					LogBossDebug("DROP-GATE", raw);
				}
				return;
			}

			string foundBoss = null;
			string mapName = isDefeat ? "Đã bị hạ gục!" : "Không rõ map";

			// 2. Trích xuất Boss Name và Map Name theo phương pháp chia 2 vế (chuẩn MOD DVK mở rộng)
			if (isDefeat)
			{
				string[] defeatSplits = new string[]
				{
					" vừa bị tiêu diệt bởi ", " đã bị tiêu diệt bởi ", " bị tiêu diệt bởi ", " tiêu diệt bởi ",
					" vừa bị tiêu diệt", " đã bị tiêu diệt", " bị tiêu diệt", " tiêu diệt",
					" vừa bị hạ gục bởi ", " đã bị hạ gục bởi ", " bị hạ gục bởi ", " hạ gục bởi ",
					" vừa bị hạ gục", " đã bị hạ gục", " bị hạ gục", " hạ gục",
					" đã chết", " da chet", " bi tieu diet",
					" telah dikalahkan oleh ", " dikalahkan oleh ", " telah dikalahkan", " dikalahkan",
					" telah mati", " dibunuh bởi ", " dibunuh oleh ", " dibunuh",
					" was defeated by ", " defeated by ", " was defeated", " defeated",
					" was slain by ", " slain by ", " was slain", " slain", " killed by ", " killed"
				};

				for (int ds = 0; ds < defeatSplits.Length; ds++)
				{
					int dIdx = lower.IndexOf(defeatSplits[ds]);
					if (dIdx > 0)
					{
						foundBoss = text.Substring(0, dIdx).Trim();
						break;
					}
				}
			}
			else
			{
				string[] appearSplits = new string[]
				{
					" vừa xuất hiện tại ", " đã xuất hiện tại ", " xuất hiện tại ",
					" vừa xuất hiện ở ", " đã xuất hiện ở ", " xuất hiện ở ",
					" vừa xuất hiện khu vực ", " xuất hiện khu vực ",
					" vừa xuất hiện toạ độ ", " xuất hiện toạ độ ",
					" vừa xuất hiện map ", " xuất hiện map ",
					" vừa xuất hiện ", " đã xuất hiện ", " xuất hiện ",
					" vua xuat hien tai ", " da xuat hien tai ", " xuat hien tai ",
					" vua xuat hien o ", " da xuat hien o ", " xuat hien o ",
					" vua xuat hien ", " da xuat hien ", " xuat hien ",
					" đã đến ", " vừa đến ", " đang ở ", " dang o ",
					" telah muncul di ", " baru saja muncul di ", " sudah muncul di ", " muncul di ",
					" telah muncul ", " muncul ", " berada di ",
					" has appeared at ", " have appeared at ", " appeared at ", " appear at ",
					" has spawned at ", " have spawned at ", " spawned at ", " spawn at ",
					" has appeared in ", " appeared in ", " appeared ", " spawned "
				};

				for (int sp = 0; sp < appearSplits.Length; sp++)
				{
					int sIdx = lower.IndexOf(appearSplits[sp]);
					if (sIdx > 0)
					{
						foundBoss = text.Substring(0, sIdx).Trim();
						string right = text.Substring(sIdx + appearSplits[sp].Length).Trim();
						if (!string.IsNullOrEmpty(right))
						{
							mapName = right;
						}
						break;
					}
				}
			}

			// 3. Tinh lọc tên Boss nếu có từ khóa BOSS / BOS
			if (!string.IsNullOrEmpty(foundBoss))
			{
				if (foundBoss.StartsWith("BOSS ", StringComparison.OrdinalIgnoreCase))
				{
					foundBoss = foundBoss.Substring(5).Trim();
				}
				else if (foundBoss.StartsWith("BOS ", StringComparison.OrdinalIgnoreCase))
				{
					foundBoss = foundBoss.Substring(4).Trim();
				}
				while (foundBoss.StartsWith("["))
				{
					int ci = foundBoss.IndexOf(']');
					if (ci <= 0 || ci > 24) break;
					foundBoss = foundBoss.Substring(ci + 1).Trim();
				}
			}

			// 4. Fallback tra cứu qua KNOWN_BOSSES nếu chưa tìm thấy tên
			if (string.IsNullOrEmpty(foundBoss))
			{
				for (int i = 0; i < KNOWN_BOSSES.Length; i++)
				{
					if (text.IndexOf(KNOWN_BOSSES[i], StringComparison.OrdinalIgnoreCase) >= 0)
					{
						foundBoss = KNOWN_BOSSES[i];
						break;
					}
				}
			}

			if (string.IsNullOrEmpty(foundBoss))
			{
				if (IsBossCandidate(lower))
				{
					LogBossDebug("DROP-NONAME", raw);
				}
				return;
			}

			// 5. Làm sạch tên Map và tách bỏ hậu tố khu vực (để ModNextMap tìm đúng map)
			if (!isDefeat)
			{
				if (string.IsNullOrEmpty(mapName) || mapName.Equals("Không rõ map"))
				{
					string[] splitKeywords = new string[] { " tại ", " ở ", " khu vực ", " toạ độ ", " map ", " tai ", " o ", " di ", " ke ", " pada ", " at ", " on ", " in " };
					for (int j = 0; j < splitKeywords.Length; j++)
					{
						int ridx = lower.IndexOf(splitKeywords[j]);
						if (ridx >= 0)
						{
							string sub = text.Substring(ridx + splitKeywords[j].Length).Trim();
							if (!string.IsNullOrEmpty(sub))
							{
								mapName = sub;
								break;
							}
						}
					}
				}

				if (!string.IsNullOrEmpty(mapName) && !mapName.Equals("Không rõ map"))
				{
					int dotIdx = mapName.IndexOfAny(new char[] { '.', ',', '!', ';', '\n', '\r' });
					if (dotIdx > 0)
					{
						mapName = mapName.Substring(0, dotIdx).Trim();
					}

					string[] zoneKeywords = new string[] { " khu vực ", " khu ", " kv ", " toạ độ ", " tọa độ ", " zone ", " ch " };
					for (int z = 0; z < zoneKeywords.Length; z++)
					{
						int zIdx = mapName.ToLower().IndexOf(zoneKeywords[z]);
						if (zIdx > 0)
						{
							mapName = mapName.Substring(0, zIdx).Trim();
							break;
						}
					}
				}
			}

			DateTime now = DateTime.Now;
			string timeStr = string.Format("{0:D2}:{1:D2}:{2:D2}", now.Hour, now.Minute, now.Second);

			AddBossNotice(foundBoss, mapName, timeStr, isDefeat);
			LogBossDebug("ADDED boss=" + foundBoss + " map=" + mapName, raw);
		}
		catch
		{
		}
	}

	public static string GetTimeAgoString(long timestamp)
	{
		if (timestamp <= 0)
		{
			return "0s";
		}
		long now = mSystem.currentTimeMillis();
		long diffSec = (now - timestamp) / 1000L;
		if (diffSec < 0)
		{
			diffSec = 0;
		}

		if (diffSec < 60)
		{
			return diffSec + "s";
		}
		if (diffSec < 3600)
		{
			long m = diffSec / 60L;
			long s = diffSec % 60L;
			if (s > 0)
			{
				return m + "p" + s + "s";
			}
			return m + "p";
		}
		if (diffSec < 86400)
		{
			long h = diffSec / 3600L;
			long remM = (diffSec % 3600L) / 60L;
			if (remM > 0)
			{
				return h + "h" + remM + "p";
			}
			return h + "h";
		}
		long d = diffSec / 86400L;
		long remH = (diffSec % 86400L) / 3600L;
		if (remH > 0)
		{
			return d + "d" + remH + "h";
		}
		return d + "d";
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
				g.translate(-g.getTranslateX(), -g.getTranslateY());
				g.setClip(0, 0, GameCanvas.w, GameCanvas.h);

				int startY = hudStartY;
				int lineH = 11;
				int drawY = startY;

				for (int j = 0; j < listBossNotices.Count; j++)
				{
					BossNoticeEntry entry = listBossNotices[j];
					if (entry != null)
					{
						string bossPart = entry.bossName ?? string.Empty;
						string mapPart = " - " + (entry.mapName ?? string.Empty) + " - ";
						string timePart = GetTimeAgoString(entry.timestamp);

						mFont bossF = entry.isDefeated ? mFont.tahoma_7_grey : mFont.tahoma_7_red;
						mFont mapF = entry.isDefeated ? mFont.tahoma_7_grey : (mFont.tahoma_7_blue ?? mFont.tahoma_7b_blue ?? mFont.tahoma_7_white);
						mFont timeF = entry.isDefeated ? mFont.tahoma_7_grey : mFont.tahoma_7_green2;
						if (bossF == null || mapF == null || timeF == null)
						{
							break;
						}

						int bossW = bossF.getWidth(bossPart);
						int mapW = mapF.getWidth(mapPart);
						int timeW = timeF.getWidth(timePart);
						int rowW = bossW + mapW + timeW;

						// Thụt lùi sát mép phải màn hình (cách mép 2px)
						int lineX = GameCanvas.w - rowW - 2;
						if (lineX < 2)
						{
							lineX = 2;
						}

						bossF.drawString(g, bossPart, lineX, drawY, mFont.LEFT);
						mapF.drawString(g, mapPart, lineX + bossW, drawY, mFont.LEFT);
						timeF.drawString(g, timePart, lineX + bossW + mapW, drawY, mFont.LEFT);

						drawY += lineH;
					}
				}
			}
			catch
			{
			}
		}
	}

	public static bool CheckHUDClick(int px, int py)
	{
		// Thông báo boss trên HUD chỉ để hiển thị thông tin, không nhận tương tác click
		return false;
	}
}
