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

			bool needHeal = false;
			if (lockHPMP)
			{
				if (me.cHP < me.cHPFull || me.cMP < me.cMPFull)
				{
					needHeal = true;
				}
			}
			else if (autoPean)
			{
				long curHpPercent = (me.cHPFull > 0) ? (me.cHP * 100L / me.cHPFull) : 100;
				long curMpPercent = (me.cMPFull > 0) ? (me.cMP * 100L / me.cMPFull) : 100;

				if (curHpPercent < autoPeanHpPercent || curMpPercent < autoPeanHpPercent)
				{
					needHeal = true;
				}
			}

			if (needHeal)
			{
				long now = mSystem.currentTimeMillis();
				if (now - lastPeanTime < 1500)
				{
					return;
				}

				lastPeanTime = now;
				if (!me.doUsePotion())
				{
					GameScr.gI().doUseHP();
				}
			}
		}
		catch
		{
		}
	}
}
