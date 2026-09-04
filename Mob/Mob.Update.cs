using System;
using Assets.src.g;

public partial class Mob : IMapObject
{
	public void updateSuperEff()
		{
			if (typeSuperEff == 0 && GameCanvas.gameTick % 25 == 0)
			{
				ServerEffect.addServerEffect(114, this, 1);
			}
			if (typeSuperEff == 1 && GameCanvas.gameTick % 4 == 0)
			{
				ServerEffect.addServerEffect(132, this, 1);
			}
			if (typeSuperEff == 2 && GameCanvas.gameTick % 7 == 0)
			{
				ServerEffect.addServerEffect(131, this, 1);
			}
		}

	public void setAttack(Char cFocus)
		{
			isBusyAttackSomeOne = true;
			mobToAttack = null;
			this.cFocus = cFocus;
			p1 = 0;
			p2 = 0;
			status = 3;
			tick = 0;
			dir = ((cFocus.cx > x) ? 1 : (-1));
			int cx = cFocus.cx;
			int cy = cFocus.cy;
			if (Res.abs(cx - x) < w * 2 && Res.abs(cy - y) < h * 2)
			{
				p3 = 0;
			}
			else
			{
				p3 = 1;
			}
		}

	public void updateMobAttack()
		{
			int[] array = ((p3 != 0) ? attack2 : attack1);
			if (tick < array.Length)
			{
				checkFrameTick(array);
				if (x >= GameScr.cmx && x <= GameScr.cmx + GameCanvas.w && p3 == 0 && GameCanvas.gameTick % 2 == 0)
				{
					SoundMn.gI().charPunch(isKick: false, 0.05f);
				}
			}
			if (p1 == 0)
			{
				int num = 0;
				int num2 = 0;
				num = ((cFocus == null) ? mobToAttack.x : cFocus.cx);
				num2 = ((cFocus == null) ? mobToAttack.y : cFocus.cy);
				if (!isNewMod())
				{
					if (x > xFirst + arrMobTemplate[templateId].rangeMove)
					{
						p1 = 1;
					}
					if (x < xFirst - arrMobTemplate[templateId].rangeMove)
					{
						p1 = 1;
					}
				}
				if ((arrMobTemplate[templateId].type == 4 || arrMobTemplate[templateId].type == 5) && !isDontMove)
				{
					y += (num2 - y) / 20;
				}
				p2++;
				if (p2 > array.Length - 1 || p1 == 1)
				{
					p1 = 1;
					if (p3 == 0)
					{
						if (cFocus != null)
						{
							cFocus.doInjure(dame, dameMp, isCrit: false, isMob: true);
						}
						else
						{
							mobToAttack.setInjure();
						}
						isBusyAttackSomeOne = false;
					}
					else
					{
						if (cFocus != null)
						{
							MonsterDart.addMonsterDart(x + dir * w, y, checkIsBoss(), dame, dameMp, cFocus, getTemplate().dartType);
						}
						else
						{
							Char @char = new Char();
							@char.cx = mobToAttack.x;
							@char.cy = mobToAttack.y;
							@char.charID = -100;
							MonsterDart.addMonsterDart(x + dir * w, y, checkIsBoss(), dame, dameMp, @char, getTemplate().dartType);
						}
						isBusyAttackSomeOne = false;
					}
				}
				dir = ((x < num) ? 1 : (-1));
			}
			else if (p1 == 1)
			{
				if (arrMobTemplate[templateId].type == 0 || isDontMove || isIce || !isWind)
				{
				}
				if (tick == array.Length)
				{
					status = 2;
					p1 = 0;
					p2 = 0;
					tick = 0;
				}
			}
			if (tick == 5 && cFocus != null && cFocus.charID == Char.myCharz().charID)
			{
				if (templateId == 88 && p3 != 0)
				{
					GameScr.shock_scr = 2;
				}
				if (templateId == 89)
				{
					GameScr.shock_scr = 2;
				}
			}
		}

	public void updateMobWalk()
		{
			int num = 0;
			try
			{
				if (injureThenDie)
				{
					status = 1;
					p2 = injureBy.cdir << 3;
					p1 = -5;
					p3 = 0;
				}
				num = 1;
				if (isIce)
				{
					return;
				}
				if (isDontMove || isWind)
				{
					checkFrameTick(stand);
					return;
				}
				switch (arrMobTemplate[templateId].type)
				{
				case 0:
					if (isNewModStand())
					{
						frame = stand[GameCanvas.gameTick % stand.Length];
					}
					else
					{
						frame = 0;
					}
					num = 2;
					break;
				case 1:
				case 2:
				case 3:
				{
					num = 3;
					sbyte b = arrMobTemplate[templateId].speed;
					if (b == 1)
					{
						if (GameCanvas.gameTick % 2 == 1)
						{
							break;
						}
					}
					else if (b > 2)
					{
						b += (sbyte)(mobId % 2);
					}
					else if (GameCanvas.gameTick % 2 == 1)
					{
						b--;
					}
					x += b * dir;
					if (x > xFirst + arrMobTemplate[templateId].rangeMove)
					{
						dir = -1;
					}
					else if (x < xFirst - arrMobTemplate[templateId].rangeMove)
					{
						dir = 1;
					}
					if (Res.abs(x - Char.myCharz().cx) < 40 && Res.abs(x - xFirst) < arrMobTemplate[templateId].rangeMove)
					{
						dir = ((x <= Char.myCharz().cx) ? 1 : (-1));
						if (Res.abs(x - Char.myCharz().cx) < 20)
						{
							x -= dir * 10;
						}
						status = 2;
						forceWait = 20;
					}
					checkFrameTick((w <= 30) ? moveFast : move);
					break;
				}
				case 4:
				{
					num = 4;
					sbyte speed2 = arrMobTemplate[templateId].speed;
					speed2 += (sbyte)(mobId % 2);
					x += speed2 * dir;
					if (GameCanvas.gameTick % 10 > 2)
					{
						y += speed2 * dirV;
					}
					speed2 += (sbyte)((GameCanvas.gameTick + mobId) % 2);
					if (x > xFirst + arrMobTemplate[templateId].rangeMove)
					{
						dir = -1;
						status = 2;
						forceWait = GameCanvas.gameTick % 20 + 20;
						p1 = 0;
					}
					else if (x < xFirst - arrMobTemplate[templateId].rangeMove)
					{
						dir = 1;
						status = 2;
						forceWait = GameCanvas.gameTick % 20 + 20;
						p1 = 0;
					}
					if (y > yFirst + 24)
					{
						dirV = -1;
					}
					else if (y < yFirst - (20 + GameCanvas.gameTick % 10))
					{
						dirV = 1;
					}
					checkFrameTick(move);
					break;
				}
				case 5:
				{
					num = 5;
					sbyte speed = arrMobTemplate[templateId].speed;
					speed += (sbyte)(mobId % 2);
					x += speed * dir;
					speed += (sbyte)((GameCanvas.gameTick + mobId) % 2);
					if (GameCanvas.gameTick % 10 > 2)
					{
						y += speed * dirV;
					}
					if (x > xFirst + arrMobTemplate[templateId].rangeMove)
					{
						dir = -1;
						status = 2;
						forceWait = GameCanvas.gameTick % 20 + 20;
						p1 = 0;
					}
					else if (x < xFirst - arrMobTemplate[templateId].rangeMove)
					{
						dir = 1;
						status = 2;
						forceWait = GameCanvas.gameTick % 20 + 20;
						p1 = 0;
					}
					if (y > yFirst + 24)
					{
						dirV = -1;
					}
					else if (y < yFirst - (20 + GameCanvas.gameTick % 10))
					{
						dirV = 1;
					}
					if (TileMap.tileTypeAt(x, y, 2))
					{
						if (GameCanvas.gameTick % 10 > 5)
						{
							y = TileMap.tileYofPixel(y);
							status = 4;
							p1 = 0;
							dirV = -1;
						}
						else
						{
							dirV = -1;
						}
					}
					break;
				}
				}
			}
			catch (Exception)
			{
				Cout.println("lineee: " + num);
			}
		}

	public bool isUpdate()
		{
			if (arrMobTemplate[templateId] == null)
			{
				return false;
			}
			if (arrMobTemplate[templateId].data == null)
			{
				return false;
			}
			if (status == 0)
			{
				return false;
			}
			return true;
		}

	public void attackOtherMob(Mob mobToAttack)
		{
			this.mobToAttack = mobToAttack;
			isBusyAttackSomeOne = true;
			cFocus = null;
			p1 = 0;
			p2 = 0;
			status = 3;
			tick = 0;
			dir = ((mobToAttack.x > x) ? 1 : (-1));
			int num = mobToAttack.x;
			int num2 = mobToAttack.y;
			if (Res.abs(num - x) < w * 2 && Res.abs(num2 - y) < h * 2)
			{
				if (x < num)
				{
					x = num - w;
				}
				else
				{
					x = num + w;
				}
				p3 = 0;
			}
			else
			{
				p3 = 1;
			}
		}

	public void removeHoldEff()
		{
			if (holdEffID != 0)
			{
				holdEffID = 0;
			}
		}

	public void removeBlindEff()
		{
			blindEff = false;
		}

	public void removeSleepEff()
		{
			sleepEff = false;
		}

}
