using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Module Thông Báo Boss: Giám sát toàn server thông báo boss xuất hiện kèm nút bay đến tức thì, tự dò khu và tự đánh Boss.
/// </summary>
public static class ModBossNotice
{
	/// <summary>
	/// CLASS BossNoticeEntry: Định nghĩa và điều phối logic trong hệ thống game.
	/// </summary>
	public class BossNoticeEntry
	{
		public string bossName;
		public string mapName;
		public string timeStr;
		public long timestamp;
		public bool isDefeated;
	}

	public static bool isShowBossNotice = true;
	public static bool isAutoScanBossZone = false;
	public static bool isAutoAttackBoss = false;
	public static bool isAutoFocusBoss = false;
	public static bool isKsSuperBroly1Hp = false;
	public static string filterBossNamesRaw = "";
	public static int hudStartY = 38;
	public static readonly List<BossNoticeEntry> listBossNotices = new List<BossNoticeEntry>();
	public const int MAX_BOSS_NOTICES = 6;

	private static long lastScanZoneTime = 0;
	private static int currentScanZoneIdx = 0;
	private static long lastAutoAttackBossTime = 0;

	public static bool MatchesBossFilter(string bossName)
	{
		if (string.IsNullOrEmpty(filterBossNamesRaw)) return true;
		if (string.IsNullOrEmpty(bossName)) return false;
		string[] tokens = filterBossNamesRaw.Split(new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
		if (tokens.Length == 0) return true;
		for (int i = 0; i < tokens.Length; i++)
		{
			string t = tokens[i].Trim();
			if (t.Length > 0 && bossName.IndexOf(t, StringComparison.OrdinalIgnoreCase) >= 0)
			{
				return true;
			}
		}
		return false;
	}

	private class BossFilterChatListener : IChatable
	{
		public void onChatFromMe(string text, string to)
		{
			filterBossNamesRaw = (text ?? string.Empty).Trim();
			ModConfig.SaveConfig();
			ChatTextField.gI().isShow = false;
			ChatTextField.gI().parentScreen = GameScr.gI();
			GameScr.info1.addInfo(string.IsNullOrEmpty(filterBossNamesRaw) ? "Đã tắt lọc tên Boss (dò tất cả)" : ("Lọc Boss: " + filterBossNamesRaw), 0);
		}
		public void onCancelChat()
		{
			ChatTextField.gI().isShow = false;
			ChatTextField.gI().parentScreen = GameScr.gI();
		}
	}

	private static readonly BossFilterChatListener filterChatListener = new BossFilterChatListener();

	public static void ShowInputFilterBoss()
	{
		ChatTextField.gI().strChat = "Nhập tên Boss lọc (cách nhau dấu ;)";
		ChatTextField.gI().tfChat.name = "Tên Boss (để trống = tất cả)";
		ChatTextField.gI().tfChat.setIputType(TField.INPUT_TYPE_ANY);
		ChatTextField.gI().startChat2(filterChatListener, string.Empty);
		ChatTextField.gI().tfChat.setText(filterBossNamesRaw ?? string.Empty);
	}

	/// <summary>
	/// Bật hoặc tắt tự động dò khu tìm Boss trong map hiện tại.
	/// </summary>
	public static void ToggleAutoScanBossZone()
	{
		isAutoScanBossZone = !isAutoScanBossZone;
		lastScanZoneTime = 0;
		if (isAutoScanBossZone && ModMenu.IsInGame())
		{
			currentScanZoneIdx = TileMap.zoneID;
			Service.gI().openUIZone();
		}
		ModConfig.SaveConfig();
		GameScr.info1.addInfo("Tự dò khu Boss: " + (isAutoScanBossZone ? "BẬT" : "TẮT"), 0);
		SoundMn.gI().buttonClick();
	}

	/// <summary>
	/// Bật hoặc tắt tự động khóa mục tiêu và tấn công Boss trong map.
	/// </summary>
	public static void ToggleAutoAttackBoss()
	{
		isAutoAttackBoss = !isAutoAttackBoss;
		ModConfig.SaveConfig();
		GameScr.info1.addInfo("Tự săn Boss: " + (isAutoAttackBoss ? "BẬT" : "TẮT"), 0);
		SoundMn.gI().buttonClick();
	}

	/// <summary>
	/// Tìm kiếm thực thể Boss đang còn sống trong bản đồ hiện tại.
	/// </summary>
	public static Char FindLiveBossInMap()
	{
		if (GameScr.vCharInMap == null)
		{
			return null;
		}
		for (int i = 0; i < GameScr.vCharInMap.size(); i++)
		{
			Char c = (Char)GameScr.vCharInMap.elementAt(i);
			if (c != null && c.charID < 0 && c.cHP > 0 && !c.isDie && c.statusMe != 14 && c.statusMe != 5)
			{
				if ((c.cTypePk == 5 || IsBossName(c.cName)) && MatchesBossFilter(c.cName))
				{
					return c;
				}
			}
		}
		return null;
	}

	/// <summary>
	/// Cập nhật tiến trình tự dò khu tìm Boss và tự động tấn công Boss.
	/// </summary>
	public static void UpdateAutoBoss()
	{
		if ((!isAutoScanBossZone && !isAutoAttackBoss && !isAutoFocusBoss && !isKsSuperBroly1Hp) || !ModMenu.IsInGame())
		{
			return;
		}
		if (Char.isLoadingMap || Char.ischangingMap || ModNextMap.isNextMapActive || ModGoBack.isReturning || ModSetActivator.isBusy || ModAutoBuyBua.isBusy)
		{
			return;
		}
		Char me = Char.myCharz();
		if (me == null || me.isDie || me.cHP <= 0 || me.statusMe == 14 || me.statusMe == 5)
		{
			return;
		}

		long now = mSystem.currentTimeMillis();
		Char liveBoss = FindLiveBossInMap();

		if (liveBoss != null)
		{
			if (isAutoScanBossZone)
			{
				isAutoScanBossZone = false;
				ModConfig.SaveConfig();
				GameScr.info1.addInfo("Đã tìm thấy Boss [" + liveBoss.cName + "] tại Khu " + TileMap.zoneID + "!", 0);
				SoundMn.gI().buttonClick();
			}

			if (isAutoFocusBoss || isAutoAttackBoss || isKsSuperBroly1Hp)
			{
				me.charFocus = liveBoss;
				me.mobFocus = null;
				me.npcFocus = null;
				me.itemFocus = null;
			}

			bool shouldAttack = isAutoAttackBoss;
			if (isKsSuperBroly1Hp && liveBoss.cName != null && liveBoss.cName.IndexOf("Broly", StringComparison.OrdinalIgnoreCase) >= 0)
			{
				shouldAttack = (liveBoss.cHP <= 500 || (liveBoss.cHPFull > 0 && liveBoss.cHP * 100L / liveBoss.cHPFull <= 1));
			}

			if (shouldAttack)
			{
				int dist = Res.distance(me.cx, me.cy, liveBoss.cx, liveBoss.cy);
				if (dist > 48 && now - lastAutoAttackBossTime >= 350)
				{
					ModTeleport.TeleportTo(liveBoss.cx, liveBoss.cy);
				}

				long attackInterval = isKsSuperBroly1Hp ? 60 : 250;
				if (now - lastAutoAttackBossTime >= attackInterval)
				{
					Skill sk = me.myskill;
					if (sk != null && !sk.paintCanNotUseSkill && (isKsSuperBroly1Hp || now - sk.lastTimeUseThisSkill >= sk.coolDown))
					{
						lastAutoAttackBossTime = now;
						sk.lastTimeUseThisSkill = now;
						MyVector vMob = new MyVector();
						MyVector vChar = new MyVector();
						vChar.addElement(liveBoss);
						Service.gI().sendPlayerAttack(vMob, vChar, 2);
					}
				}
			}
			return;
		}

		if (isAutoScanBossZone && now - lastScanZoneTime >= 1600)
		{
			lastScanZoneTime = now;
			GameScr gs = GameScr.gI();
			int maxZ = (gs != null && gs.zones != null && gs.zones.Length > 0) ? gs.zones.Length : 15;
			if (maxZ <= 1)
			{
				return;
			}

			int nextZ = (TileMap.zoneID + 1) % maxZ;
			if (gs != null && gs.zones != null && gs.maxPlayer != null)
			{
				for (int attempt = 0; attempt < maxZ; attempt++)
				{
					int candidate = (TileMap.zoneID + 1 + attempt) % maxZ;
					if (candidate != TileMap.zoneID && candidate < gs.zones.Length && candidate < gs.maxPlayer.Length)
					{
						if (gs.zones[candidate] < gs.maxPlayer[candidate])
						{
							nextZ = candidate;
							break;
						}
					}
				}
			}

			currentScanZoneIdx = nextZ;
			Service.gI().requestChangeZone(currentScanZoneIdx, -1);
		}
	}

	/// <summary>
	/// Tính toán tọa độ Y ngay sát bên dưới nút Chat góc phải trên màn hình.
	/// </summary>
	public static int GetHudStartY()
	{
		int yC = (GameCanvas.w <= 240) ? 5 : 2;
		Image chatImg = (!Main.isPC) ? GameScr.imgChat : GameScr.imgChatPC;
		int chatH = (chatImg != null) ? mGraphics.getImageHeight(chatImg) : 34;
		if (chatH <= 0)
		{
			chatH = 34;
		}
		return yC + 17 + chatH / 2 + 2 + mGraphics.addYWhenOpenKeyBoard;
	}

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

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của IsBossName.
	/// </summary>
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

	/// <summary>
	/// Thực thi phương thức nghiệp vụ AddBossNotice.
	/// </summary>
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

	/// <summary>
	/// Thực thi phương thức nghiệp vụ LogBossDebug.
	/// </summary>
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

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của IsBossCandidate.
	/// </summary>
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

	/// <summary>
	/// Xử lý nghiệp vụ logic chuyên trách cho ProcessServerBossNotice.
	/// </summary>
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

	/// <summary>
	/// Truy xuất và trả về dữ liệu của GetTimeAgoString.
	/// </summary>
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

	/// <summary>
	/// Thực hiện vẽ và hiển thị thành phần PaintBossNotice lên canvas đồ họa.
	/// </summary>
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

				hudStartY = GetHudStartY();
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

						// Thụt lùi sát mép phải màn hình (cách mép 2px) có tính toán Safe Area
						int lineX = GameCanvas.w - rowW - 2 - SafeArea.Right;
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

	/// <summary>
	/// Truy xuất và trả về dữ liệu của GetBottomY.
	/// </summary>
	public static int GetBottomY()
	{
		int baseY = GetHudStartY();
		if (!isShowBossNotice)
		{
			return baseY;
		}
		lock (listBossNotices)
		{
			if (listBossNotices.Count == 0)
			{
				return baseY;
			}
			int count = (listBossNotices.Count < MAX_BOSS_NOTICES) ? listBossNotices.Count : MAX_BOSS_NOTICES;
			return baseY + count * 11 + 4;
		}
	}
}
