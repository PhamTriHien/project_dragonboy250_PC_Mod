using System;
using Assets.src.g;

public partial class Mob
{
	public static BigBoss getBigBoss()
			{
				for (int i = 0; i < GameScr.vMob.size(); i++)
				{
					Mob mob = (Mob)GameScr.vMob.elementAt(i);
					if (mob is BigBoss)
					{
						return (BigBoss)mob;
					}
				}
				return null;
			}

	public static BigBoss2 getBigBoss2()
			{
				for (int i = 0; i < GameScr.vMob.size(); i++)
				{
					Mob mob = (Mob)GameScr.vMob.elementAt(i);
					if (mob is BigBoss2)
					{
						return (BigBoss2)mob;
					}
				}
				return null;
			}

	public static BachTuoc getBachTuoc()
			{
				for (int i = 0; i < GameScr.vMob.size(); i++)
				{
					Mob mob = (Mob)GameScr.vMob.elementAt(i);
					if (mob is BachTuoc)
					{
						return (BachTuoc)mob;
					}
				}
				return null;
			}

	public static NewBoss getNewBoss(sbyte idBoss)
			{
				Mob mob = (Mob)GameScr.vMob.elementAt(idBoss);
				if (mob is NewBoss)
				{
					return (NewBoss)mob;
				}
				return null;
			}

	public static void removeBigBoss()
			{
				for (int i = 0; i < GameScr.vMob.size(); i++)
				{
					Mob mob = (Mob)GameScr.vMob.elementAt(i);
					if (mob is BigBoss)
					{
						GameScr.vMob.removeElement(mob);
						break;
					}
				}
			}

	private bool isSpecial()
			{
				if ((templateId >= 58 && templateId <= 65) || templateId == 67 || templateId == 68)
				{
					return true;
				}
				return false;
			}

	private bool isNewModStand()
			{
				return templateId == 76;
			}

	private bool isNewMod()
			{
				if (templateId >= 73 && !isNewModStand())
				{
					return true;
				}
				return false;
			}

	private void updateInjure()
			{
				if (!isBusyAttackSomeOne && GameCanvas.gameTick % 4 == 0)
				{
					if (isTypeNewMod())
					{
						frame = hurt[GameCanvas.gameTick % hurt.Length];
					}
					else if (isNewModStand())
					{
						frame = attack1[GameCanvas.gameTick % attack1.Length];
					}
					else if (isNewMod())
					{
						if (frame != 10)
						{
							frame = 10;
						}
						else
						{
							frame = 11;
						}
					}
					else if (isSpecial())
					{
						if (frame != 1)
						{
							frame = 1;
						}
						else
						{
							frame = 15;
						}
					}
					else if (frame != 10)
					{
						frame = 10;
					}
					else
					{
						frame = 11;
					}
				}
				timeStatus--;
				if (timeStatus <= 0 && (isTypeNewMod() || isNewModStand() || (isNewMod() && frame == 11) || (isSpecial() && frame == 15) || (templateId < 58 && frame == 11)))
				{
					if ((injureBy != null && injureThenDie) || hp == 0)
					{
						status = 1;
						p2 = injureBy.cdir << 1;
						p1 = -3;
						p3 = 0;
					}
					else
					{
						status = 5;
						if (injureBy != null)
						{
							dir = -injureBy.cdir;
							if (Res.abs(x - injureBy.cx) < 24)
							{
								status = 2;
							}
						}
						p1 = (p2 = (p3 = 0));
						timeStatus = 0;
					}
					injureBy = null;
				}
				else if (arrMobTemplate[templateId].type != 0 && injureBy != null)
				{
					int num = -injureBy.cdir << 1;
					if (x > xFirst - arrMobTemplate[templateId].rangeMove && x < xFirst + arrMobTemplate[templateId].rangeMove)
					{
						x -= num;
					}
				}
			}

	private void updateMobStandWait()
			{
				checkFrameTick(stand);
				switch (arrMobTemplate[templateId].type)
				{
				case 0:
				case 1:
				case 2:
				case 3:
					p1++;
					if (p1 > 10 + mobId % 10 && (cFocus == null || Res.abs(cFocus.cx - x) > 80) && (mobToAttack == null || Res.abs(mobToAttack.x - x) > 80))
					{
						status = 5;
					}
					break;
				case 4:
				case 5:
					p1++;
					if (p1 > mobId % 3 && (cFocus == null || Res.abs(cFocus.cx - x) > 80) && (mobToAttack == null || Res.abs(mobToAttack.x - x) > 80))
					{
						status = 5;
					}
					break;
				}
				if (cFocus != null && GameCanvas.gameTick % (10 + p1 % 20) == 0)
				{
					if (cFocus.cx > x)
					{
						dir = 1;
					}
					else
					{
						dir = -1;
					}
				}
				else if (mobToAttack != null && GameCanvas.gameTick % (10 + p1 % 20) == 0)
				{
					if (mobToAttack.x > x)
					{
						dir = 1;
					}
					else
					{
						dir = -1;
					}
				}
				if (forceWait > 0)
				{
					forceWait--;
					status = 2;
				}
			}

}
