using System;
using Assets.src.g;

public partial class Mob : IMapObject
{
	public bool isBigBoss()
		{
			return this is BachTuoc || this is BigBoss2 || this is BigBoss || this is NewBoss;
		}

	public void setInjure()
		{
			if (hp > 0 && status != 3 && status != 7)
			{
				timeStatus = 4;
				status = 7;
			}
		}

	public void startDie()
		{
			hp = 0L;
			injureThenDie = true;
			hp = 0L;
			status = 1;
			Res.outz("MOB DIEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEe");
			p1 = -3;
			p2 = -dir;
			p3 = 0;
		}

}
