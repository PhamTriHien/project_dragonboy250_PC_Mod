using System;

public static class ModAutoPick
{
	public static bool autoPick = false;
	public static bool pickAll = true;
	public static bool pickGold = true;
	public static bool pickEquip = true;
	public static bool pickGem = true;
	private static long lastPickTime = 0;

	public static void RunRealAutoPick()
	{
		try
		{
			if (!autoPick || ModNextMap.isNextMapActive)
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

				// Lọc loại vật phẩm theo cấu hình
				bool shouldPick = pickAll;
				if (!shouldPick && it.template != null)
				{
					int tId = it.template.id;
					// Vàng (id 190, 76...)
					if (pickGold && (tId == 190 || tId == 76 || (it.template.name != null && it.template.name.ToLower().Contains("vàng"))))
					{
						shouldPick = true;
					}
					// Trang bị (type 0..5: áo, quần, găng, giày, rada)
					if (pickEquip && it.template.type >= 0 && it.template.type <= 5)
					{
						shouldPick = true;
					}
					// Ngọc rồng & ngọc (type 12 hoặc id ngọc)
					if (pickGem && (it.template.type == 12 || (tId >= 14 && tId <= 20) || (it.template.name != null && it.template.name.ToLower().Contains("ngọc"))))
					{
						shouldPick = true;
					}
				}

				if (shouldPick)
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
