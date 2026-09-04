using System;
using System.Collections.Generic;

public static class ModNextMapData
{
	// Danh sách ID bản đồ theo 3 hành tinh: Trái Đất, Namếc, Xayda
	public static readonly int[][] planetMapIds = new int[3][]
	{
		new int[] { 21, 0, 1, 2, 24, 3, 4, 5, 6, 27, 28, 29, 30, 42, 47, 46, 45, 48 },
		new int[] { 22, 7, 8, 9, 25, 11, 12, 13, 10, 31, 32, 33, 34, 43 },
		new int[] { 23, 14, 15, 16, 26, 17, 18, 20, 19, 35, 36, 37, 38, 44 }
	};

	// Đồ thị kết nối bản đồ
	public static readonly Dictionary<int, List<int>> mapWaypoints = new Dictionary<int, List<int>>
	{
		{ 0, new List<int> { 21, 1, 24, 42 } },
		{ 21, new List<int> { 0 } },
		{ 1, new List<int> { 0, 2, 27 } },
		{ 2, new List<int> { 1, 3 } },
		{ 3, new List<int> { 2, 4 } },
		{ 4, new List<int> { 3, 5 } },
		{ 5, new List<int> { 4, 6 } },
		{ 6, new List<int> { 5 } },
		{ 27, new List<int> { 1, 28 } },
		{ 28, new List<int> { 27, 29 } },
		{ 29, new List<int> { 28, 30 } },
		{ 30, new List<int> { 29 } },
		{ 42, new List<int> { 0, 47 } },
		{ 47, new List<int> { 42, 46 } },
		{ 46, new List<int> { 47, 45 } },
		{ 45, new List<int> { 46, 48 } },
		{ 48, new List<int> { 45 } },
		{ 24, new List<int> { 0, 25, 26 } },

		{ 7, new List<int> { 22, 8, 25, 43 } },
		{ 22, new List<int> { 7 } },
		{ 8, new List<int> { 7, 9, 31 } },
		{ 9, new List<int> { 8, 11, 10 } },
		{ 10, new List<int> { 9 } },
		{ 11, new List<int> { 9, 12 } },
		{ 12, new List<int> { 11, 13 } },
		{ 13, new List<int> { 12 } },
		{ 31, new List<int> { 8, 32 } },
		{ 32, new List<int> { 31, 33 } },
		{ 33, new List<int> { 32, 34 } },
		{ 34, new List<int> { 33 } },
		{ 43, new List<int> { 7 } },
		{ 25, new List<int> { 7, 24, 26 } },

		{ 14, new List<int> { 23, 15, 26, 44 } },
		{ 23, new List<int> { 14 } },
		{ 15, new List<int> { 14, 16, 35 } },
		{ 16, new List<int> { 15, 17 } },
		{ 17, new List<int> { 16, 18 } },
		{ 18, new List<int> { 17, 20 } },
		{ 20, new List<int> { 18, 19 } },
		{ 19, new List<int> { 20 } },
		{ 35, new List<int> { 15, 36 } },
		{ 36, new List<int> { 35, 37 } },
		{ 37, new List<int> { 36, 38 } },
		{ 38, new List<int> { 37 } },
		{ 44, new List<int> { 14 } },
		{ 26, new List<int> { 14, 24, 25 } }
	};

	public static string GetMapName(int id)
	{
		switch (id)
		{
			case 0: return "Làng Aru";
			case 1: return "Đồi hoa cúc";
			case 2: return "Thung lũng tre";
			case 3: return "Rừng nấm";
			case 4: return "Rừng xương";
			case 5: return "Đảo Kamê";
			case 6: return "Đông Nam Kamê";
			case 7: return "Làng Mori";
			case 8: return "Đồi nấm";
			case 9: return "Thung lũng Maima";
			case 10: return "Thung lũng đá";
			case 11: return "Vực cấm";
			case 12: return "Núi Appule";
			case 13: return "Căn cứ Fide";
			case 14: return "Làng Kakarot";
			case 15: return "Đồi hoang";
			case 16: return "Làng Plant";
			case 17: return "Rừng nguyên sinh";
			case 18: return "Rừng cọ";
			case 19: return "Thành phố Vegeta";
			case 20: return "Vách núi đen";
			case 21: return "Nhà Gôhan";
			case 22: return "Nhà Moori";
			case 23: return "Nhà Broly";
			case 24: return "Trạm tàu T.Đất";
			case 25: return "Trạm tàu Namếc";
			case 26: return "Trạm tàu Xayda";
			case 27: return "Rừng bamboo";
			case 28: return "Rừng dương xỉ";
			case 29: return "Nam Kamê";
			case 30: return "Đảo Kamê 2";
			case 31: return "Núi hoa vàng";
			case 32: return "Núi hoa tím";
			case 33: return "Nam Guru";
			case 34: return "Đền Guru";
			case 35: return "Rừng đá";
			case 36: return "Thung lũng đen";
			case 37: return "Bờ vực đen";
			case 38: return "Căn cứ Fide 2";
			case 42: return "Vách Aru";
			case 43: return "Vách Moori";
			case 44: return "Vách Kakarot";
			case 45: return "Thần điện";
			case 46: return "Tháp Karin";
			case 47: return "Rừng Karin";
			case 48: return "Thánh địa Kaio";
			default: return "Map " + id;
		}
	}

	public static string CleanName(string s)
	{
		if (string.IsNullOrEmpty(s)) return string.Empty;
		s = s.ToLower().Trim();
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

	public static bool MatchMapName(string wpName, string mapName)
	{
		if (string.IsNullOrEmpty(wpName) || string.IsNullOrEmpty(mapName)) return false;
		string w = CleanName(wpName);
		string m = CleanName(mapName);
		return w.Contains(m) || m.Contains(w);
	}

	public static int FindMapIdByName(string name)
	{
		if (string.IsNullOrEmpty(name)) return -1;
		string cleanTarget = CleanName(name);

		// 1. So khớp chính xác theo danh sách ID chuẩn
		for (int id = 0; id <= 48; id++)
		{
			string mName = GetMapName(id);
			if (CleanName(mName).Equals(cleanTarget))
			{
				return id;
			}
		}

		// 2. So khớp với TileMap.mapNames
		if (TileMap.mapNames != null)
		{
			for (int i = 0; i < TileMap.mapNames.Length; i++)
			{
				if (TileMap.mapNames[i] != null && CleanName(TileMap.mapNames[i]).Equals(cleanTarget))
				{
					return i;
				}
			}
		}

		// 3. So khớp gần đúng chứa từ khóa
		for (int id = 0; id <= 48; id++)
		{
			string mName = GetMapName(id);
			if (MatchMapName(mName, name))
			{
				return id;
			}
		}

		if (TileMap.mapNames != null)
		{
			for (int i = 0; i < TileMap.mapNames.Length; i++)
			{
				if (TileMap.mapNames[i] != null && MatchMapName(TileMap.mapNames[i], name))
				{
					return i;
				}
			}
		}

		return -1;
	}
}
