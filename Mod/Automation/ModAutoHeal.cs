using System;

public static class ModAutoHeal
{
	public static bool autoPean = true;
	public static int autoPeanHpPercent = 30;
	public static bool lockHPMP = false;
	private static long lastPeanTime = 0;

	public static void DoRealAutoHeal()
	{
		try
		{
			Char me = Char.myCharz();
			if (me == null || me.cHP <= 0 || me.statusMe == 14 || me.statusMe == 5)
			{
				return;
			}

			if (lockHPMP)
			{
				me.cHP = me.cHPFull;
				me.cMP = me.cMPFull;
			}

			if (autoPean)
			{
				long now = mSystem.currentTimeMillis();
				if (now - lastPeanTime < 1500)
				{
					return;
				}

				long curHpPercent = (me.cHPFull > 0) ? (me.cHP * 100L / me.cHPFull) : 100;
				long curMpPercent = (me.cMPFull > 0) ? (me.cMP * 100L / me.cMPFull) : 100;

				if (curHpPercent < autoPeanHpPercent || curMpPercent < autoPeanHpPercent)
				{
					lastPeanTime = now;
					// Ưu tiên đậu thần trong túi đồ (Item type = 6: Đậu thần)
					if (me.arrItemBag != null)
					{
						for (int i = 0; i < me.arrItemBag.Length; i++)
						{
							Item it = me.arrItemBag[i];
							if (it != null && it.template != null && it.template.type == 6)
							{
								Service.gI().useItem(0, 1, (sbyte)i, it.template.id);
								return;
							}
						}
					}
					// Nếu không có đậu trong túi, dùng hàm doUseHP mặc định của game
					GameScr.gI().doUseHP();
				}
			}
		}
		catch
		{
		}
	}
}
