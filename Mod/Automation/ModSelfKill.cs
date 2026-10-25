using System;

/// <summary>
/// Tự Sát Nhanh (Kill bản thân): Tự động bật cờ đen và tấn công bản thân cho đến khi về trạng thái hồi sinh/về nhà.
/// </summary>
public static class ModSelfKill
{
	public static bool isEnabled = false;
	private static long s_LastFlagTime = 0;
	private static long s_LastAttackTime = 0;

	public static void Toggle()
	{
		isEnabled = !isEnabled;
		GameScr.info1.addInfo("Tự sát nhanh: " + (isEnabled ? "BẬT" : "TẮT"), 0);
		SoundMn.gI().buttonClick();
	}

	public static void Update()
	{
		if (!isEnabled) return;
		if (!ModMenu.IsInGame())
		{
			isEnabled = false;
			return;
		}

		Char me = Char.myCharz();
		if (me == null || me.isDie || me.cHP <= 0 || me.statusMe == 14)
		{
			isEnabled = false;
			GameScr.info1.addInfo("Tự sát nhanh: Đã hoàn tất!", 0);
			return;
		}

		long now = mSystem.currentTimeMillis();
		if (me.cFlag == 0)
		{
			if (now - s_LastFlagTime >= 1100)
			{
				s_LastFlagTime = now;
				Service.gI().getFlag(1, 8);
			}
			return;
		}

		if (now - s_LastAttackTime >= 500)
		{
			s_LastAttackTime = now;
			MyVector vMob = new MyVector();
			MyVector vChar = new MyVector();
			vChar.addElement(me);
			Service.gI().sendPlayerAttack(vMob, vChar, -1);
		}
	}
}
