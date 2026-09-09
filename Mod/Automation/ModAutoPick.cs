using System;
using System.Collections.Generic;

public class AutoPickActionListener : IActionListener
{
	public const int ACTION_SET_FILTER_IDS = 99101;
	public const int ACTION_SET_FILTER_NAMES = 99102;

	public void perform(int idAction, object p)
	{
		try
		{
			if (GameCanvas.inputDlg == null || GameCanvas.inputDlg.tfInput == null)
			{
				GameCanvas.endDlg();
				return;
			}
			string text = GameCanvas.inputDlg.tfInput.getText();
			GameCanvas.endDlg();

			if (idAction == ACTION_SET_FILTER_IDS)
			{
				ModAutoPick.SetFilterIds(text);
				ModConfig.SaveConfig();
				SoundMn.gI().buttonClick();
			}
			else if (idAction == ACTION_SET_FILTER_NAMES)
			{
				ModAutoPick.SetFilterNames(text);
				ModConfig.SaveConfig();
				SoundMn.gI().buttonClick();
			}
		}
		catch
		{
			GameCanvas.endDlg();
		}
	}
}

public static class ModAutoPick
{
	public static bool autoPick = false;
	public static bool pickAll = true;
	public static bool pickGold = true;
	public static bool pickEquip = true;
	public static bool pickGem = true;

	// Lọc theo ID Item
	public static bool filterById = false;
	public static string filterIdsRaw = "";
	public static HashSet<int> filterIds = new HashSet<int>();

	// Lọc theo Tên Item
	public static bool filterByName = false;
	public static string filterNameRaw = "";
	public static List<string> filterNameKeywords = new List<string>();

	private static long lastPickTime = 0;
	private static readonly AutoPickActionListener listener = new AutoPickActionListener();

	public static void LoadFilterIds(string raw)
	{
		filterIdsRaw = raw != null ? raw.Trim() : "";
		filterIds.Clear();
		if (!string.IsNullOrEmpty(filterIdsRaw))
		{
			string[] parts = filterIdsRaw.Split(new char[] { ',', ' ', ';', '|' }, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < parts.Length; i++)
			{
				if (int.TryParse(parts[i].Trim(), out int id))
				{
					filterIds.Add(id);
				}
			}
		}
	}

	public static void SetFilterIds(string raw)
	{
		LoadFilterIds(raw);
		if (filterIds.Count > 0)
		{
			filterById = true;
			pickAll = false;
			GameScr.info1.addInfo("Đã đặt lọc ID nhặt (" + filterIds.Count + " ID): [" + filterIdsRaw + "]", 0);
		}
		else
		{
			filterById = false;
			GameScr.info1.addInfo("Đã xóa bộ lọc ID nhặt", 0);
		}
	}

	public static void LoadFilterNames(string raw)
	{
		filterNameRaw = raw != null ? raw.Trim() : "";
		filterNameKeywords.Clear();
		if (!string.IsNullOrEmpty(filterNameRaw))
		{
			string[] parts = filterNameRaw.Split(new char[] { ',', ';', '|' }, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < parts.Length; i++)
			{
				string kw = parts[i].Trim().ToLower();
				if (!string.IsNullOrEmpty(kw))
				{
					filterNameKeywords.Add(kw);
				}
			}
		}
	}

	public static void SetFilterNames(string raw)
	{
		LoadFilterNames(raw);
		if (filterNameKeywords.Count > 0)
		{
			filterByName = true;
			pickAll = false;
			GameScr.info1.addInfo("Đã đặt lọc Tên nhặt (" + filterNameKeywords.Count + " từ khóa): [" + filterNameRaw + "]", 0);
		}
		else
		{
			filterByName = false;
			GameScr.info1.addInfo("Đã xóa bộ lọc Tên nhặt", 0);
		}
	}

	public static void ClearFilters()
	{
		filterIdsRaw = "";
		filterIds.Clear();
		filterById = false;

		filterNameRaw = "";
		filterNameKeywords.Clear();
		filterByName = false;

		GameScr.info1.addInfo("Đã xóa toàn bộ lọc ID và Tên nhặt", 0);
	}

	public static void ShowInputFilterId()
	{
		if (GameCanvas.inputDlg == null) return;
		GameCanvas.inputDlg.show("Nhập danh sách ID cần nhặt (cách nhau dấu phẩy hoặc khoảng trắng):", new Command("OK", listener, AutoPickActionListener.ACTION_SET_FILTER_IDS, null), TField.INPUT_TYPE_ANY);
		if (GameCanvas.inputDlg.tfInput != null)
		{
			GameCanvas.inputDlg.tfInput.setMaxTextLenght(150);
			GameCanvas.inputDlg.tfInput.setText(filterIdsRaw);
		}
	}

	public static void ShowInputFilterName()
	{
		if (GameCanvas.inputDlg == null) return;
		GameCanvas.inputDlg.show("Nhập từ khóa tên item cần nhặt (cách nhau dấu phẩy):", new Command("OK", listener, AutoPickActionListener.ACTION_SET_FILTER_NAMES, null), TField.INPUT_TYPE_ANY);
		if (GameCanvas.inputDlg.tfInput != null)
		{
			GameCanvas.inputDlg.tfInput.setMaxTextLenght(150);
			GameCanvas.inputDlg.tfInput.setText(filterNameRaw);
		}
	}

	public static bool ShouldPickItem(ItemMap it)
	{
		if (it == null || it.template == null) return false;
		if (pickAll) return true;

		int tId = it.template.id;
		string name = it.template.name;

		// 1. Lọc theo danh sách ID
		if (filterById && filterIds.Contains(tId))
		{
			return true;
		}

		// 2. Lọc theo từ khóa Tên
		if (filterByName && !string.IsNullOrEmpty(name))
		{
			string lowerName = name.ToLower();
			for (int i = 0; i < filterNameKeywords.Count; i++)
			{
				if (lowerName.Contains(filterNameKeywords[i]))
				{
					return true;
				}
			}
		}

		// 3. Vàng (id 190, 76...)
		if (pickGold && (tId == 190 || tId == 76 || (name != null && name.ToLower().Contains("vàng"))))
		{
			return true;
		}

		// 4. Trang bị (type 0..5: áo, quần, găng, giày, rada)
		if (pickEquip && it.template.type >= 0 && it.template.type <= 5)
		{
			return true;
		}

		// 5. Ngọc rồng & ngọc (type 12 hoặc id 14..20)
		if (pickGem && (it.template.type == 12 || (tId >= 14 && tId <= 20) || (name != null && name.ToLower().Contains("ngọc"))))
		{
			return true;
		}

		return false;
	}

	public static void RunRealAutoPick()
	{
		try
		{
			if (!autoPick || ModNextMap.isNextMapActive || ModGoBack.isReturning || ModSetActivator.isBusy)
			{
				return;
			}
			long now = mSystem.currentTimeMillis();
			if (now - lastPickTime < 250)
			{
				return;
			}
			Char me = Char.myCharz();
			if (me == null || me.cHP <= 0 || me.statusMe == 14 || me.statusMe == 5)
			{
				return;
			}

			MyVector items = GameScr.vItemMap;
			if (items == null || items.size() == 0)
			{
				return;
			}

			ItemMap closest = null;
			int minDistance = int.MaxValue;

			for (int i = 0; i < items.size(); i++)
			{
				ItemMap it = (ItemMap)items.elementAt(i);
				if (it == null)
				{
					continue;
				}

				if (ShouldPickItem(it))
				{
					int dist = Res.distance(me.cx, me.cy, it.x, it.y);
					if (dist < minDistance)
					{
						minDistance = dist;
						closest = it;
					}
				}
			}

			if (closest != null)
			{
				lastPickTime = now;
				if (minDistance <= 30)
				{
					// Đã ở cự ly gần: Nhặt trực tiếp
					me.itemFocus = closest;
					Service.gI().pickItem(closest.itemMapID);
				}
				else
				{
					// Áp sát vật phẩm và nhặt
					ModTeleport.TeleportTo(closest.x, closest.y);
					me.itemFocus = closest;
					Service.gI().pickItem(closest.itemMapID);
				}
			}
		}
		catch
		{
		}
	}
}
