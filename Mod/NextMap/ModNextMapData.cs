using System;
using System.Collections.Generic;

/// <summary>
/// CLASS ModNextMapData: Định nghĩa và điều phối dữ liệu bản đồ, đồ thị kết nối và nhận diện cổng dịch chuyển đa ngôn ngữ.
/// </summary>
public static class ModNextMapData
{
	// Danh sách ID bản đồ theo 3 hành tinh: Trái Đất, Namếc, Xayda
	public static readonly int[][] planetMapIds = new int[3][]
	{
		new int[] { 21, 0, 1, 2, 24, 3, 4, 5, 6, 27, 28, 29, 30, 42, 47, 46, 45, 48 },
		new int[] { 22, 7, 8, 9, 25, 11, 12, 13, 10, 31, 32, 33, 34, 43 },
		new int[] { 23, 14, 15, 16, 26, 17, 18, 20, 19, 35, 36, 37, 38, 44 }
	};

	// Đồ thị kết nối bản đồ chuẩn xác theo World Map gốc và dữ liệu NRmap thực tế
	public static readonly Dictionary<int, List<int>> mapWaypoints = new Dictionary<int, List<int>>
	{
		// Hành tinh Trái Đất
		{ 0, new List<int> { 21, 1, 24, 42 } },
		{ 21, new List<int> { 0 } },
		{ 1, new List<int> { 0, 2 } },
		{ 2, new List<int> { 1, 3, 24, 47 } },
		{ 3, new List<int> { 2, 4, 27 } },
		{ 4, new List<int> { 3, 5 } },
		{ 5, new List<int> { 4, 6, 29 } },
		{ 6, new List<int> { 5 } },
		{ 27, new List<int> { 3, 28 } },
		{ 28, new List<int> { 27, 29 } },
		{ 29, new List<int> { 28, 5, 30 } },
		{ 30, new List<int> { 29 } },
		{ 39, new List<int> { 0, 42 } },
		{ 42, new List<int> { 0, 47 } },
		{ 47, new List<int> { 42, 2, 46, 111 } },
		{ 111, new List<int> { 47 } },
		{ 46, new List<int> { 47, 45 } },
		{ 45, new List<int> { 46, 48 } },
		{ 48, new List<int> { 45, 50 } },
		{ 50, new List<int> { 48, 49 } },
		{ 49, new List<int> { 50 } },
		{ 24, new List<int> { 0, 2, 25, 26 } },

		// Hành tinh Namếc
		{ 7, new List<int> { 22, 8, 25, 43 } },
		{ 22, new List<int> { 7 } },
		{ 8, new List<int> { 7, 9 } },
		{ 9, new List<int> { 8, 11, 25 } },
		{ 11, new List<int> { 9, 12, 31 } },
		{ 12, new List<int> { 11, 13 } },
		{ 13, new List<int> { 12, 10, 33 } },
		{ 10, new List<int> { 13 } },
		{ 31, new List<int> { 11, 32 } },
		{ 32, new List<int> { 31, 33 } },
		{ 33, new List<int> { 32, 13, 34 } },
		{ 34, new List<int> { 33 } },
		{ 40, new List<int> { 7, 43 } },
		{ 43, new List<int> { 7 } },
		{ 25, new List<int> { 7, 9, 24, 26 } },

		// Hành tinh Xayda
		{ 14, new List<int> { 23, 15, 44 } },
		{ 23, new List<int> { 14 } },
		{ 15, new List<int> { 14, 16 } },
		{ 16, new List<int> { 15, 17, 26 } },
		{ 17, new List<int> { 16, 18, 35 } },
		{ 18, new List<int> { 17, 20 } },
		{ 20, new List<int> { 18, 19, 37 } },
		{ 19, new List<int> { 20 } },
		{ 35, new List<int> { 17, 36 } },
		{ 36, new List<int> { 35, 37 } },
		{ 37, new List<int> { 36, 20, 38 } },
		{ 38, new List<int> { 37 } },
		{ 41, new List<int> { 14, 44 } },
		{ 44, new List<int> { 14 } },
		{ 26, new List<int> { 16, 14, 24, 25 } },

		// Khu vực Fide / Nappa (63..83)
		{ 68, new List<int> { 69, 67 } },
		{ 69, new List<int> { 68, 70 } },
		{ 70, new List<int> { 69, 71 } },
		{ 71, new List<int> { 70, 72 } },
		{ 72, new List<int> { 71, 64 } },
		{ 64, new List<int> { 72, 65 } },
		{ 65, new List<int> { 64, 63 } },
		{ 63, new List<int> { 65, 66 } },
		{ 66, new List<int> { 63, 67 } },
		{ 67, new List<int> { 66, 68, 73 } },
		{ 73, new List<int> { 67, 74 } },
		{ 74, new List<int> { 73, 75 } },
		{ 75, new List<int> { 74, 76 } },
		{ 76, new List<int> { 75, 77 } },
		{ 77, new List<int> { 76, 81 } },
		{ 81, new List<int> { 77, 82 } },
		{ 82, new List<int> { 81, 83 } },
		{ 83, new List<int> { 82, 79 } },
		{ 79, new List<int> { 83, 80 } },
		{ 80, new List<int> { 79 } },

		// Khu vực Tương lai / Android (92..103)
		{ 92, new List<int> { 93, 94, 102 } },
		{ 93, new List<int> { 92, 94 } },
		{ 94, new List<int> { 92, 93, 96 } },
		{ 96, new List<int> { 94, 97 } },
		{ 97, new List<int> { 96, 98, 99 } },
		{ 98, new List<int> { 97, 99 } },
		{ 99, new List<int> { 97, 98, 100 } },
		{ 100, new List<int> { 99, 103 } },
		{ 102, new List<int> { 92, 93 } },
		{ 103, new List<int> { 100 } },

		// Khu vực Hành tinh Cold (105..110)
		{ 105, new List<int> { 106, 109 } },
		{ 106, new List<int> { 105, 107, 109 } },
		{ 107, new List<int> { 106, 108, 110 } },
		{ 108, new List<int> { 107, 109, 110 } },
		{ 109, new List<int> { 105, 106, 108, 110 } },
		{ 110, new List<int> { 107, 108, 109 } }
	};

	/// <summary>
	/// Truy xuất và trả về tên hiển thị tiếng Việt chuẩn của bản đồ theo ID.
	/// </summary>
	public static string GetMapName(int id)
	{
		switch (id)
		{
			case 24: return "Trạm tàu T.Đất";
			case 25: return "Trạm tàu Namếc";
			case 26: return "Trạm tàu Xayda";
			case 42: return "Vách núi Aru";
			case 43: return "Vách núi Moori";
			case 44: return "Vách núi Kakarot";
		}

		if (ModTranslate.MapNames != null && id >= 0 && id < ModTranslate.MapNames.Length && !string.IsNullOrEmpty(ModTranslate.MapNames[id]))
		{
			return ModTranslate.MapNames[id];
		}

		if (TileMap.mapNames != null && id >= 0 && id < TileMap.mapNames.Length && !string.IsNullOrEmpty(TileMap.mapNames[id]))
		{
			return TileMap.mapNames[id];
		}

		return "Map " + id;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ RemoveAccents.
	/// </summary>
	public static string RemoveAccents(string text)
	{
		if (string.IsNullOrEmpty(text)) return string.Empty;
		string[] arr1 = new string[] {
			"á","à","ả","ã","ạ","â","ấ","ầ","ẩ","ẫ","ậ","ă","ắ","ằ","ẳ","ẵ","ặ",
			"đ",
			"é","è","ẻ","ẽ","ẹ","ê","ế","ề","ể","ễ","ệ",
			"í","ì","ỉ","ĩ","ị",
			"ó","ò","ỏ","õ","ọ","ô","ố","ồ","ổ","ỗ","ộ","ơ","ớ","ờ","ở","ỡ","ợ",
			"ú","ù","ủ","ũ","ụ","ư","ứ","ừ","ử","ữ","ự",
			"ý","ỳ","ỷ","ỹ","ỵ"
		};
		string[] arr2 = new string[] {
			"a","a","a","a","a","a","a","a","a","a","a","a","a","a","a","a","a",
			"d",
			"e","e","e","e","e","e","e","e","e","e","e",
			"i","i","i","i","i",
			"o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o","o",
			"u","u","u","u","u","u","u","u","u","u","u",
			"y","y","y","y","y"
		};
		text = text.ToLower().Trim();
		for (int i = 0; i < arr1.Length; i++)
		{
			text = text.Replace(arr1[i], arr2[i]);
		}
		return text;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ CleanName.
	/// </summary>
	public static string CleanName(string s)
	{
		if (string.IsNullOrEmpty(s)) return string.Empty;
		s = RemoveAccents(s);
		string res = string.Empty;
		for (int i = 0; i < s.Length; i++)
		{
			char c = s[i];
			if (char.IsLetterOrDigit(c))
			{
				res += c;
			}
		}
		return res;
	}

	// Bảng tên bản đồ đa ngôn ngữ xác thực từ NRmap của Server Teamobi (English / IndoNaga / Vietnamese)
	private static readonly Dictionary<int, string[]> s_MapMultilingualAliases = new Dictionary<int, string[]>
	{
		{ 0, new string[] { "Lang Aru", "Aru village", "Desa Aru" } },
		{ 1, new string[] { "Doi hoa cuc", "Chrysanthemum hill", "Bukit krisan" } },
		{ 2, new string[] { "Thung lung tre", "Bamboo valley", "Lembah bambu" } },
		{ 3, new string[] { "Rung nam", "Mushroom forest", "Hutan jamur" } },
		{ 4, new string[] { "Rung xuong", "Bone forest", "Hutan tulang" } },
		{ 5, new string[] { "Dao Kame", "Kame island", "Pulau Kame" } },
		{ 6, new string[] { "Dong Karin", "East Karin", "Dong Nam Kame", "Timur Karin", "Tenggara Kame" } },
		{ 7, new string[] { "Lang Mori", "Lang Moori", "Mori village", "Moori village", "Desa Mori", "Desa Moori" } },
		{ 8, new string[] { "Doi nam tim", "Purple mushroom hill", "Bukit jamur ungu" } },
		{ 9, new string[] { "Thi tran Moori", "Thi tran Mori", "Moori town", "Mori town", "Kota Moori" } },
		{ 10, new string[] { "Thung lung Namec", "Thung lung Namek", "Namek valley", "Namec valley", "Lembah Namek" } },
		{ 11, new string[] { "Thung lung Maima", "Maima valley", "Lembah Maima" } },
		{ 12, new string[] { "Vuc Maima", "Maima deep", "Jurang Maima" } },
		{ 13, new string[] { "Dao Guru", "Guru island", "Su phu island", "Pulau Guru" } },
		{ 14, new string[] { "Lang Kakarot", "Kakalot village", "Kakarot village", "Desa Kakarot", "Desa Kakalot" } },
		{ 15, new string[] { "Doi hoang", "Abadon hill", "Bukit Abadon", "Bukit liar" } },
		{ 16, new string[] { "Lang Plant", "Plant village", "Desa Plant" } },
		{ 17, new string[] { "Rung nguyen sinh", "Primary forest", "Primeval forest", "Hutan primer", "Hutan perawan" } },
		{ 18, new string[] { "Rung thong Xayda", "Rung thong", "Saiyan pine forest", "Hutan pinus Saiyan" } },
		{ 19, new string[] { "Thanh pho Vegeta", "Vegeta city", "Kota Vegeta" } },
		{ 20, new string[] { "Vach nui den", "Black Cliff", "Tebing hitam" } },
		{ 21, new string[] { "Nha Gohan", "Gohan house", "Rumah Gohan" } },
		{ 22, new string[] { "Nha Moori", "Nha Mori", "Moori house", "Mori house", "Rumah Moori" } },
		{ 23, new string[] { "Nha Broly", "Broly house", "Rumah Broly" } },
		{ 24, new string[] { "Tram tau vu tru", "Tram tau T.Dat", "Spaceship station", "Stasiun pesawat" } },
		{ 25, new string[] { "Tram tau vu tru", "Tram tau Namec", "Spaceship station", "Stasiun pesawat" } },
		{ 26, new string[] { "Tram tau vu tru", "Tram tau Xayda", "Spaceship station", "Stasiun pesawat" } },
		{ 27, new string[] { "Rung truc", "Rung bamboo", "Bamboo forest", "Hutan bambu" } },
		{ 28, new string[] { "Rung duong xi", "Fern forest", "Hutan pakis" } },
		{ 29, new string[] { "Nam Kame", "South Kame", "Selatan Kame" } },
		{ 30, new string[] { "Dao Bulong", "Bulong island", "Pulau Bulong", "Dao Kame 2" } },
		{ 31, new string[] { "Nui hoa vang", "Yellow flowers mountain", "Gunung bunga kuning" } },
		{ 32, new string[] { "Nui hoa tim", "Purple flowers mountain", "Gunung bunga ungu" } },
		{ 33, new string[] { "Nam Guru", "South Guru", "South Su phu", "Selatan Guru" } },
		{ 34, new string[] { "Dong Nam Guru", "East South Guru", "East South Su phu", "Tenggara Guru", "Den Guru" } },
		{ 35, new string[] { "Rung co", "Palm forest", "Hutan palem" } },
		{ 36, new string[] { "Rung da", "Stone forest", "Da forest", "Hutan batu" } },
		{ 37, new string[] { "Thung lung den", "Black valley", "Lembah hitam" } },
		{ 38, new string[] { "Bo vuc den", "Black deep", "Jurang hitam" } },
		{ 39, new string[] { "Vach nui Aru", "Vach Aru", "Aru cliff", "Tebing Aru" } },
		{ 40, new string[] { "Vach nui Moori", "Vach Moori", "Moori cliff", "Tebing Moori" } },
		{ 41, new string[] { "Vuc Plant", "Plant deep", "Jurang Plant" } },
		{ 42, new string[] { "Vach nui Aru", "Vach Aru", "Aru cliff", "Tebing Aru" } },
		{ 43, new string[] { "Vach nui Moori", "Vach Moori", "Vach Mori", "Moori cliff", "Tebing Moori" } },
		{ 44, new string[] { "Vach nui Kakarot", "Vach Kakarot", "Kakarot cliff", "Kakalot cliff", "Tebing Kakarot" } },
		{ 45, new string[] { "Than dien", "Sama palace", "Kami palace", "Kuil Dewa" } },
		{ 46, new string[] { "Thap Karin", "Karin tower", "Korin tower", "Menara Karin" } },
		{ 47, new string[] { "Rung Karin", "Karin forest", "Hutan Karin" } },
		{ 48, new string[] { "Hanh tinh Kaio", "Kaio planet", "Planet Kaio", "Thanh dia Kaio" } },
		{ 49, new string[] { "Phong tap thoi gian", "Time room" } },
		{ 50, new string[] { "Thanh dia Kaio", "Kaio sanctuary" } },
		{ 111, new string[] { "Dong Nam Karin", "East South Karin" } }
	};

	/// <summary>
	/// Đồng bộ trực tiếp các cổng dịch chuyển thực tế (TileMap.vGo) của bản đồ hiện tại vào đồ thị mapWaypoints.
	/// </summary>
	public static void SyncLiveWaypointsForCurrentMap()
	{
		try
		{
			int currMapId = TileMap.mapID;
			if (currMapId < 0 || TileMap.vGo == null || TileMap.vGo.size() == 0)
			{
				return;
			}

			int planetId = TileMap.planetID;
			List<int> resolvedTargets = new List<int>();
			int totalWps = TileMap.vGo.size();
			int resolvedCount = 0;

			for (int i = 0; i < totalWps; i++)
			{
				Waypoint wp = (Waypoint)TileMap.vGo.elementAt(i);
				if (wp == null)
				{
					continue;
				}

				int targetId = ResolveWaypointTargetMapId(wp, currMapId, planetId);
				if (targetId >= 0 && targetId != currMapId)
				{
					resolvedCount++;
					if (!resolvedTargets.Contains(targetId))
					{
						resolvedTargets.Add(targetId);
					}
				}
			}

			if (resolvedTargets.Count == 0)
			{
				return;
			}

			bool graphChanged = false;
			if (!mapWaypoints.TryGetValue(currMapId, out List<int> currNeighbors))
			{
				currNeighbors = new List<int>();
				mapWaypoints[currMapId] = currNeighbors;
				graphChanged = true;
			}

			// Nếu mọi cổng trên map thường đều đã xác định rõ đích đến, loại bỏ các cạnh tĩnh bị lệch
			bool isSpaceshipStation = (currMapId == 24 || currMapId == 25 || currMapId == 26);
			bool isAirMap = TileMap.isInAirMap() || currMapId == 45 || currMapId == 46 || currMapId == 48;
			if (!isSpaceshipStation && !isAirMap && resolvedCount == totalWps)
			{
				for (int i = currNeighbors.Count - 1; i >= 0; i--)
				{
					int oldNeighbor = currNeighbors[i];
					if (!resolvedTargets.Contains(oldNeighbor))
					{
						currNeighbors.RemoveAt(i);
						graphChanged = true;
					}
				}
			}

			for (int i = 0; i < resolvedTargets.Count; i++)
			{
				int targetId = resolvedTargets[i];
				if (!currNeighbors.Contains(targetId))
				{
					currNeighbors.Add(targetId);
					graphChanged = true;
				}

				if (!mapWaypoints.TryGetValue(targetId, out List<int> revNeighbors))
				{
					revNeighbors = new List<int>();
					mapWaypoints[targetId] = revNeighbors;
				}
				if (!revNeighbors.Contains(currMapId))
				{
					revNeighbors.Add(currMapId);
					graphChanged = true;
				}
			}

			if (graphChanged)
			{
				ModNextMapPathFinder.ClearCache();
			}
		}
		catch
		{
		}
	}

	/// <summary>
	/// Giải mã chính xác ID bản đồ đích từ tên gốc (rawName) và tên hiển thị (name) của một Waypoint.
	/// </summary>
	public static int ResolveWaypointTargetMapId(Waypoint wp, int currentMapId, int planetId)
	{
		if (wp == null)
		{
			return -1;
		}

		int idFromRaw = ResolveMapIdFromText(wp.rawName, currentMapId, planetId);
		if (idFromRaw >= 0)
		{
			return idFromRaw;
		}

		int idFromName = ResolveMapIdFromText(wp.name, currentMapId, planetId);
		if (idFromName >= 0)
		{
			return idFromName;
		}

		if (wp.popup != null && wp.popup.says != null && wp.popup.says.Length > 0)
		{
			string joined = string.Join(" ", wp.popup.says);
			int idFromPopup = ResolveMapIdFromText(joined, currentMapId, planetId);
			if (idFromPopup >= 0)
			{
				return idFromPopup;
			}
		}

		return -1;
	}

	private static int ResolveMapIdFromText(string text, int currentMapId, int planetId)
	{
		if (string.IsNullOrEmpty(text))
		{
			return -1;
		}

		string clean = CleanName(text);
		if (string.IsNullOrEmpty(clean))
		{
			return -1;
		}

		// Trạm tàu vũ trụ (24, 25, 26) dùng chung tên "Spaceship station" / "Trạm tàu vũ trụ" nên phân biệt theo hành tinh đang đứng
		if (clean == "spaceshipstation" || clean == "tramtauvutru" || clean == "stasiunpesawat")
		{
			if (planetId == 1 || currentMapId == 7 || currentMapId == 8 || currentMapId == 9) return 25;
			if (planetId == 2 || currentMapId == 14 || currentMapId == 15 || currentMapId == 16) return 26;
			return 24;
		}

		// Vách núi Aru / Moori có cặp map 42/39 và 43/40: từ làng ưu tiên map 42/43
		if (clean == "arucliff" || clean == "vachnuiaru" || clean == "vacharu")
		{
			return 42;
		}
		if (clean == "mooricliff" || clean == "vachnuimoori" || clean == "vachmoori")
		{
			return 43;
		}
		if (clean == "kakarotcliff" || clean == "kakalotcliff" || clean == "vachnuikakarot" || clean == "vachkakarot")
		{
			return 44;
		}

		// 1. So khớp chính xác với tên gốc từ Server (ModTranslate.rawMapNames)
		if (ModTranslate.rawMapNames != null)
		{
			for (int i = 0; i < ModTranslate.rawMapNames.Length; i++)
			{
				if (i == currentMapId || i == 39 || i == 40) continue;
				string raw = ModTranslate.rawMapNames[i];
				if (!string.IsNullOrEmpty(raw) && CleanName(raw).Equals(clean, StringComparison.OrdinalIgnoreCase))
				{
					return i;
				}
			}
		}

		// 2. So khớp chính xác với bảng tên chuẩn ModTranslate.MapNames
		if (ModTranslate.MapNames != null)
		{
			for (int i = 0; i < ModTranslate.MapNames.Length; i++)
			{
				if (i == currentMapId || i == 39 || i == 40) continue;
				string vn = ModTranslate.MapNames[i];
				if (!string.IsNullOrEmpty(vn) && CleanName(vn).Equals(clean, StringComparison.OrdinalIgnoreCase))
				{
					return i;
				}
			}
		}

		// 3. So khớp chính xác với s_MapMultilingualAliases
		foreach (KeyValuePair<int, string[]> kv in s_MapMultilingualAliases)
		{
			int mapId = kv.Key;
			if (mapId == currentMapId || mapId == 39 || mapId == 40) continue;
			string[] aliases = kv.Value;
			for (int j = 0; j < aliases.Length; j++)
			{
				if (CleanName(aliases[j]).Equals(clean, StringComparison.OrdinalIgnoreCase))
				{
					return mapId;
				}
			}
		}

		// 4. Ưu tiên kiểm tra các láng giềng đã biết của currentMapId
		if (mapWaypoints.TryGetValue(currentMapId, out List<int> neighbors))
		{
			for (int i = 0; i < neighbors.Count; i++)
			{
				int cand = neighbors[i];
				if (MatchMapId(text, cand))
				{
					return cand;
				}
			}
		}

		// 5. Quét toàn bộ danh sách map của hành tinh hiện tại trước, sau đó toàn bộ map
		if (planetId >= 0 && planetId < planetMapIds.Length)
		{
			int[] pMaps = planetMapIds[planetId];
			for (int i = 0; i < pMaps.Length; i++)
			{
				int cand = pMaps[i];
				if (cand != currentMapId && MatchMapId(text, cand))
				{
					return cand;
				}
			}
		}

		int maxMap = (ModTranslate.MapNames != null) ? ModTranslate.MapNames.Length : 187;
		for (int i = 0; i < maxMap; i++)
		{
			if (i == currentMapId || i == 39 || i == 40) continue;
			if (MatchMapId(text, i))
			{
				return i;
			}
		}

		return -1;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ MatchMapId.
	/// </summary>
	public static bool MatchMapId(string wpName, int targetMapId)
	{
		if (string.IsNullOrEmpty(wpName) || targetMapId < 0) return false;

		string mName = GetMapName(targetMapId);
		if (MatchMapName(wpName, mName)) return true;

		if (ModTranslate.MapNames != null && targetMapId < ModTranslate.MapNames.Length && !string.IsNullOrEmpty(ModTranslate.MapNames[targetMapId]))
		{
			if (MatchMapName(wpName, ModTranslate.MapNames[targetMapId])) return true;
		}

		if (ModTranslate.rawMapNames != null && targetMapId < ModTranslate.rawMapNames.Length && !string.IsNullOrEmpty(ModTranslate.rawMapNames[targetMapId]))
		{
			if (MatchMapName(wpName, ModTranslate.rawMapNames[targetMapId])) return true;
		}

		if (TileMap.mapNames != null && targetMapId < TileMap.mapNames.Length && !string.IsNullOrEmpty(TileMap.mapNames[targetMapId]))
		{
			if (MatchMapName(wpName, TileMap.mapNames[targetMapId])) return true;
		}

		if (s_MapMultilingualAliases.TryGetValue(targetMapId, out string[] aliases))
		{
			for (int i = 0; i < aliases.Length; i++)
			{
				if (MatchMapName(wpName, aliases[i]))
				{
					return true;
				}
			}
		}

		return false;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ MatchMapName.
	/// </summary>
	public static bool MatchMapName(string wpName, string mapName)
	{
		if (string.IsNullOrEmpty(wpName) || string.IsNullOrEmpty(mapName)) return false;
		string w = CleanName(wpName);
		string m = CleanName(mapName);
		if (string.IsNullOrEmpty(w) || string.IsNullOrEmpty(m)) return false;

		if (w.Equals(m, StringComparison.OrdinalIgnoreCase)) return true;

		// Tránh khớp nhầm giữa bản đồ gốc và bản đồ hậu tố số
		bool wpHas2 = w.Contains("2");
		bool mapHas2 = m.Contains("2");
		if (wpHas2 != mapHas2) return false;

		string normW = RemoveAccents(wpName).ToLower();
		string normM = RemoveAccents(mapName).ToLower();

		// Tránh khớp nhầm giữa "Đông Nam" (East South / Tenggara) và "Nam" (South / Selatan) hoặc "Đông" (East / Timur)
		bool wEastSouth = normW.Contains("dong nam") || normW.Contains("east south") || normW.Contains("tenggara");
		bool mEastSouth = normM.Contains("dong nam") || normM.Contains("east south") || normM.Contains("tenggara");
		if (wEastSouth != mEastSouth)
		{
			return false;
		}

		// Tránh khớp nhầm giữa "Rừng Karin" (Karin forest) và "Đông Karin" (East Karin) hoặc "Tháp Karin" (Karin tower)
		if ((normW.Contains("karin") || normM.Contains("karin")) && !wEastSouth && !mEastSouth)
		{
			bool wEast = normW.Contains("dong ") || normW.Contains("east ");
			bool mEast = normM.Contains("dong ") || normM.Contains("east ");
			if (wEast != mEast) return false;

			bool wTower = normW.Contains("thap") || normW.Contains("tower") || normW.Contains("menara");
			bool mTower = normM.Contains("thap") || normM.Contains("tower") || normM.Contains("menara");
			if (wTower != mTower) return false;

			bool wForest = normW.Contains("rung") || normW.Contains("forest") || normW.Contains("hutan");
			bool mForest = normM.Contains("rung") || normM.Contains("forest") || normM.Contains("hutan");
			if (wForest != mForest) return false;
		}

		// Tránh khớp nhầm giữa Làng Mori (Mori village), Thị trấn Moori (Moori town), Nhà Moori (Moori house), Vách núi Moori (Moori cliff)
		if (normW.Contains("mori") || normW.Contains("moori") || normM.Contains("mori") || normM.Contains("moori"))
		{
			bool wTown = normW.Contains("thi tran") || normW.Contains("town") || normW.Contains("kota");
			bool mTown = normM.Contains("thi tran") || normM.Contains("town") || normM.Contains("kota");
			if (wTown != mTown) return false;

			bool wHouse = normW.Contains("nha ") || normW.Contains("house") || normW.Contains("rumah");
			bool mHouse = normM.Contains("nha ") || normM.Contains("house") || normM.Contains("rumah");
			if (wHouse != mHouse) return false;

			bool wCliff = normW.Contains("vach") || normW.Contains("cliff") || normW.Contains("tebing");
			bool mCliff = normM.Contains("vach") || normM.Contains("cliff") || normM.Contains("tebing");
			if (wCliff != mCliff) return false;
		}

		if (w.Contains(m))
		{
			return true;
		}

		char[] sep = new char[] { ' ', '-', '_', '(', ')', ',', '.', '/', '\\', ':' };
		string[] wWords = normW.Split(sep, StringSplitOptions.RemoveEmptyEntries);
		string[] mWords = normM.Split(sep, StringSplitOptions.RemoveEmptyEntries);
		if (mWords.Length == 0 || wWords.Length == 0) return false;

		int requiredCount = 0;
		int matchedCount = 0;
		for (int i = 0; i < mWords.Length; i++)
		{
			string kw = mWords[i];
			if (kw == "t" || (kw == "dat" && normM.Contains("tram tau")))
			{
				continue;
			}
			requiredCount++;
			bool tokenFound = false;
			for (int j = 0; j < wWords.Length; j++)
			{
				if (wWords[j].Equals(kw, StringComparison.OrdinalIgnoreCase))
				{
					tokenFound = true;
					break;
				}
			}
			if (tokenFound)
			{
				matchedCount++;
			}
		}

		return requiredCount > 0 && matchedCount == requiredCount;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ FindMapIdByName.
	/// </summary>
	public static int FindMapIdByName(string name)
	{
		if (string.IsNullOrEmpty(name)) return -1;
		int resolved = ResolveMapIdFromText(name, TileMap.mapID, TileMap.planetID);
		if (resolved >= 0)
		{
			return resolved;
		}

		string cleanTarget = CleanName(name);
		if (TileMap.mapNames != null)
		{
			for (int i = 0; i < TileMap.mapNames.Length; i++)
			{
				if (TileMap.mapNames[i] != null && CleanName(TileMap.mapNames[i]).Equals(cleanTarget, StringComparison.OrdinalIgnoreCase))
				{
					return i;
				}
			}
		}

		return -1;
	}
}
