using System;
using System.Collections;

public static partial class ModTranslate
{
	// Lưu trữ dữ liệu thô nhận từ server để hỗ trợ khôi phục khi tắt dịch
	public static string[] rawMapNames;
	public static string[] rawNpcNames;
	public static string[] rawMobNames;

	public static string GetMapName(int id, string defaultName)
	{
		if (!ModConfig.isTranslate)
		{
			return defaultName;
		}
		if (id >= 0 && id < MapNames.Length && !string.IsNullOrEmpty(MapNames[id]))
		{
			return MapNames[id];
		}
		return defaultName;
	}

	public static string GetNpcName(int id, string defaultName)
	{
		if (!ModConfig.isTranslate)
		{
			return defaultName;
		}
		if (id >= 0 && id < NpcNames.Length && !string.IsNullOrEmpty(NpcNames[id]))
		{
			return NpcNames[id];
		}
		return defaultName;
	}

	public static string GetMobName(int id, string defaultName)
	{
		if (!ModConfig.isTranslate)
		{
			return defaultName;
		}
		if (id >= 0 && id < MobNames.Length && !string.IsNullOrEmpty(MobNames[id]))
		{
			return MobNames[id];
		}
		return defaultName;
	}

	public static string GetOptionTemplate(int id, string defaultName)
	{
		if (!ModConfig.isTranslate)
		{
			return defaultName;
		}
		if (id >= 0 && id < OptionTemplates.Length && !string.IsNullOrEmpty(OptionTemplates[id]))
		{
			return OptionTemplates[id];
		}
		return TranslateString(defaultName);
	}

	public static string GetItemName(int templateId, string rawName, string rawDescription)
	{
		if (!ModConfig.isTranslate || string.IsNullOrEmpty(rawName))
		{
			return rawName;
		}

		if (SpecificItemNames.TryGetValue(templateId, out string specificName))
		{
			return specificName;
		}

		return TranslateItemNamePattern(rawName, rawDescription);
	}

	private static string TranslateItemNamePattern(string name, string desc)
	{
		if (string.IsNullOrEmpty(name))
		{
			return name;
		}

		// Rada
		if (name.StartsWith("Radar level ", StringComparison.OrdinalIgnoreCase))
		{
			return "Rada cấp " + name.Substring(12).Trim();
		}

		// Đậu thần
		if (name.StartsWith("Senzu Beans level ", StringComparison.OrdinalIgnoreCase) ||
		    name.StartsWith("Senzu beans level ", StringComparison.OrdinalIgnoreCase))
		{
			return "Đậu thần cấp " + name.Substring(18).Trim();
		}

		// Ngọc Rồng
		if (name.StartsWith("Dragon Ball bintang ", StringComparison.OrdinalIgnoreCase))
		{
			return "Ngọc Rồng " + name.Substring(20).Trim() + " sao";
		}
		if (name.StartsWith("Dragon Ball Namek Bintang ", StringComparison.OrdinalIgnoreCase))
		{
			return "Ngọc Rồng Namếc " + name.Substring(26).Trim() + " sao";
		}
		if (name.StartsWith("Shadow Dragon Bintang ", StringComparison.OrdinalIgnoreCase))
		{
			return "Ngọc Rồng Sao Đen " + name.Substring(22).Trim() + " sao";
		}
		if (name.StartsWith("Labu bintang ", StringComparison.OrdinalIgnoreCase))
		{
			return "Bí ngô " + name.Substring(13).Trim() + " sao";
		}
		if (name.StartsWith("Bintang hitam ", StringComparison.OrdinalIgnoreCase))
		{
			return "Ngọc Rồng Đen " + name.Substring(14).Trim() + " sao";
		}
		if (name.StartsWith("Bola naga es ", StringComparison.OrdinalIgnoreCase) && name.IndexOf("bintang", StringComparison.OrdinalIgnoreCase) >= 0)
		{
			string star = name.Replace("Bola naga es ", "").Replace("bintang", "").Trim();
			return "Ngọc Rồng Băng " + star + " sao";
		}

		// Sách kỹ năng
		if (name.StartsWith("Buku ", StringComparison.OrdinalIgnoreCase))
		{
			string sub = name.Substring(5).Trim();
			sub = sub.Replace("lv ", "cấp ").Replace("lv", "cấp ");
			sub = sub.Replace("Solar Flare", "Thái Dương Hạ San");
			sub = sub.Replace("Rescure", "Trị Thương");
			sub = sub.Replace("Energy", "Khiên Năng Lượng");
			return "Sách " + sub;
		}

		// Cải trang (nếu tên gốc là Menyamar hoặc Avatar mà mô tả có Menyamar menjadi ...)
		if ((name.Equals("Menyamar", StringComparison.OrdinalIgnoreCase) || name.Equals("Avatar", StringComparison.OrdinalIgnoreCase)) &&
		    !string.IsNullOrEmpty(desc) && desc.StartsWith("Menyamar menjadi ", StringComparison.OrdinalIgnoreCase))
		{
			string charName = desc.Substring(17).Trim();
			int parenIdx = charName.IndexOf('(');
			if (parenIdx > 0)
			{
				charName = charName.Substring(0, parenIdx).Trim();
			}
			return "Cải trang " + charName;
		}

		// Áp dụng từ điển chuỗi cho tên trang bị / vật phẩm khác
		return TranslateString(name);
	}

	public static string GetItemDescription(int templateId, string rawDescription)
	{
		if (!ModConfig.isTranslate || string.IsNullOrEmpty(rawDescription))
		{
			return rawDescription;
		}

		if (ItemDescriptionMap.TryGetValue(rawDescription, out string mappedDesc))
		{
			return mappedDesc;
		}

		return TranslateItemDescriptionPattern(rawDescription);
	}

	private static string TranslateItemDescriptionPattern(string desc)
	{
		if (string.IsNullOrEmpty(desc))
		{
			return desc;
		}

		if (desc.StartsWith("Menyamar menjadi ", StringComparison.OrdinalIgnoreCase))
		{
			return "Cải trang thành " + desc.Substring(17).Trim();
		}
		if (desc.StartsWith("Belajar skill ", StringComparison.OrdinalIgnoreCase))
		{
			return "Học kỹ năng " + desc.Substring(14).Trim();
		}
		if (desc.StartsWith("Learn ", StringComparison.OrdinalIgnoreCase) && desc.EndsWith(" skill", StringComparison.OrdinalIgnoreCase))
		{
			return "Học kỹ năng " + desc.Substring(6, desc.Length - 12).Trim();
		}
		if (desc.StartsWith("Upgrade ", StringComparison.OrdinalIgnoreCase) && desc.IndexOf(" ke level", StringComparison.OrdinalIgnoreCase) >= 0)
		{
			string text = desc.Substring(8).Trim();
			int idx = text.IndexOf(" ke level", StringComparison.OrdinalIgnoreCase);
			string skill = text.Substring(0, idx).Trim();
			string lv = text.Substring(idx + 9).Trim();
			return "Nâng cấp " + skill + " lên cấp " + lv;
		}
		if (desc.StartsWith("Upgrade ", StringComparison.OrdinalIgnoreCase) && desc.IndexOf(" to level", StringComparison.OrdinalIgnoreCase) >= 0)
		{
			string text = desc.Substring(8).Trim();
			int idx = text.IndexOf(" to level", StringComparison.OrdinalIgnoreCase);
			string skill = text.Substring(0, idx).Trim();
			string lv = text.Substring(idx + 9).Trim();
			return "Nâng cấp " + skill + " lên cấp " + lv;
		}

		return TranslateString(desc);
	}

	public static void ApplyAllTranslations()
	{
		try
		{
			// 1. Cập nhật MapNames
			if (TileMap.mapNames != null && rawMapNames != null)
			{
				for (int i = 0; i < TileMap.mapNames.Length && i < rawMapNames.Length; i++)
				{
					TileMap.mapNames[i] = GetMapName(i, rawMapNames[i]);
				}
			}

			// 2. Cập nhật NpcTemplate names
			if (Npc.arrNpcTemplate != null && rawNpcNames != null)
			{
				for (int i = 0; i < Npc.arrNpcTemplate.Length && i < rawNpcNames.Length; i++)
				{
					if (Npc.arrNpcTemplate[i] != null)
					{
						Npc.arrNpcTemplate[i].name = GetNpcName(i, rawNpcNames[i]);
					}
				}
			}

			// 3. Cập nhật MobTemplate names
			if (Mob.arrMobTemplate != null && rawMobNames != null)
			{
				for (int i = 0; i < Mob.arrMobTemplate.Length && i < rawMobNames.Length; i++)
				{
					if (Mob.arrMobTemplate[i] != null)
					{
						Mob.arrMobTemplate[i].name = GetMobName(i, rawMobNames[i]);
					}
				}
			}

			// 4. Cập nhật ItemTemplates
			if (ItemTemplates.itemTemplates != null)
			{
				IDictionaryEnumerator enumerator = ItemTemplates.itemTemplates.h.GetEnumerator();
				while (enumerator.MoveNext())
				{
					if (enumerator.Value is ItemTemplate it)
					{
						if (it.rawName != null)
						{
							it.name = GetItemName(it.id, it.rawName, it.rawDescription);
						}
						if (it.rawDescription != null)
						{
							it.description = GetItemDescription(it.id, it.rawDescription);
						}
					}
				}
			}
		}
		catch (Exception)
		{
		}
	}

	public static string TranslateString(string str)
	{
		if (!ModConfig.isTranslate || string.IsNullOrEmpty(str))
		{
			return str;
		}
		try
		{
			for (int i = 0; i < WordTranslations.Length; i++)
			{
				string pattern = WordTranslations[i][0];
				string replacement = WordTranslations[i][1];
				if (str.IndexOf(pattern, StringComparison.OrdinalIgnoreCase) >= 0)
				{
					str = ReplaceWordIgnoreCase(str, pattern, replacement);
				}
			}
		}
		catch (Exception)
		{
		}
		return str;
	}

	public static string ReplaceWordIgnoreCase(string input, string pattern, string replacement)
	{
		if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(pattern))
		{
			return input;
		}
		int pos = 0;
		while (true)
		{
			int idx = input.IndexOf(pattern, pos, StringComparison.OrdinalIgnoreCase);
			if (idx < 0)
			{
				break;
			}
			if (IsWordBoundary(input, idx, pattern.Length))
			{
				input = input.Substring(0, idx) + replacement + input.Substring(idx + pattern.Length);
				pos = idx + replacement.Length;
			}
			else
			{
				pos = idx + pattern.Length;
			}
		}
		return input;
	}

	public static bool IsWordBoundary(string text, int index, int length)
	{
		if (index > 0 && char.IsLetterOrDigit(text[index - 1]))
		{
			return false;
		}
		int end = index + length;
		if (end < text.Length && char.IsLetterOrDigit(text[end]))
		{
			return false;
		}
		return true;
	}
}
