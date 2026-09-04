using System;

namespace Assets.src.g;

public partial class BigBoss
{
	public override void setBody(short id)
		{
			changBody = true;
			smallBody = id;
		}

	private void updateShadown()
		{
			int num = TileMap.size;
			xSd = x;
			wCount = 0;
			if (ySd <= 0 || TileMap.tileTypeAt(xSd, ySd, 2))
			{
				return;
			}
			if (TileMap.tileTypeAt(xSd / num, ySd / num) == 0)
			{
				isOutMap = true;
			}
			else if (TileMap.tileTypeAt(xSd / num, ySd / num) != 0 && !TileMap.tileTypeAt(xSd, ySd, 2))
			{
				xSd = x;
				ySd = y;
				isOutMap = false;
			}
			while (isOutMap && wCount < 10)
			{
				wCount++;
				ySd += 24;
				if (TileMap.tileTypeAt(xSd, ySd, 2))
				{
					if (ySd % 24 != 0)
					{
						ySd -= ySd % 24;
					}
					break;
				}
			}
		}

	public new void updateSuperEff()
		{
		}

	public override void update()
		{
			if (!isUpdate())
			{
				return;
			}
			updateShadown();
			switch (status)
			{
			case 2:
				updateMobStandWait();
				break;
			case 4:
				timeStatus = 0;
				updateMobFly();
				break;
			case 3:
				updateMobAttack();
				break;
			case 5:
				timeStatus = 0;
				updateMobWalk();
				break;
			case 6:
				timeStatus = 0;
				p1++;
				y += p1;
				if (y >= yFirst)
				{
					y = yFirst;
					p1 = 0;
					status = 5;
				}
				break;
			case 7:
				updateInjure();
				break;
			case 0:
			case 1:
				updateDead();
				break;
			}
		}

	private void updateDead()
		{
			checkFrameTick((!haftBody) ? stand : stand_1);
			if (GameCanvas.gameTick % 5 == 0)
			{
				ServerEffect.addServerEffect(167, Res.random(x - getW() / 2, x + getW() / 2), Res.random(getY() + getH() / 2, getY() + getH()), 1);
			}
			if (x != xFirst || y != yFirst)
			{
				x += (xFirst - x) / 4;
				y += (yFirst - y) / 4;
			}
		}

	private void updateMobFly()
		{
			if (flyUp)
			{
				dy++;
				y -= dy;
				checkFrameTick(fly);
				if (y <= -500)
				{
					flyUp = false;
					flyDown = true;
					dy = 0;
				}
			}
			if (flyDown)
			{
				x = xTo;
				dy += 2;
				y += dy;
				checkFrameTick(hitground);
				if (y > yFirst)
				{
					y = yFirst;
					flyDown = false;
					dy = 0;
					status = 2;
					GameScr.shock_scr = 10;
					shock = true;
				}
			}
		}

	public new void setInjure()
		{
		}

	public new void setAttack(Char cFocus)
		{
			isBusyAttackSomeOne = true;
			mobToAttack = null;
			base.cFocus = cFocus;
			p1 = 0;
			p2 = 0;
			status = 3;
			tick = 0;
			dir = ((cFocus.cx > x) ? 1 : (-1));
			int cx = cFocus.cx;
			int cy = cFocus.cy;
			if (Res.abs(cx - x) < w * 2 && Res.abs(cy - y) < h * 2)
			{
				if (x < cx)
				{
					x = cx - w;
				}
				else
				{
					x = cx + w;
				}
				p3 = 0;
			}
			else
			{
				p3 = 1;
			}
		}

	private void updateInjure()
		{
		}

	private void updateMobStandWait()
		{
			checkFrameTick((!haftBody) ? stand : stand_1);
			if (x != xFirst || y != yFirst)
			{
				x += (xFirst - x) / 4;
				y += (yFirst - y) / 4;
			}
		}

	public void setFly()
		{
			status = 4;
			flyUp = true;
		}

	public void setAttack(Char[] cAttack, long[] dame, sbyte type)
		{
			charAttack = cAttack;
			dameHP = dame;
			this.type = type;
			tick = 0;
			if (type < 3)
			{
				status = 3;
			}
			if (type == 3)
			{
				flyUp = true;
				status = 4;
			}
			if (type == 4)
			{
				for (int i = 0; i < charAttack.Length; i++)
				{
					charAttack[i].doInjure(dameHP[i], 0L, isCrit: false, isMob: false);
				}
			}
			if (type == 7)
			{
				status = 3;
			}
		}

	public new void updateMobAttack()
		{
			if (type == 7)
			{
				if (tick > 8)
				{
					tick = 8;
				}
				checkFrameTick(attack1);
				if (GameCanvas.gameTick % 4 == 0)
				{
					ServerEffect.addServerEffect(70, x + ((dir != 1) ? (-15) : 15), y - 40, 1);
				}
			}
			if (type == 0)
			{
				if (tick == attack1.Length - 1)
				{
					status = 2;
				}
				dir = ((x < charAttack[0].cx) ? 1 : (-1));
				checkFrameTick(attack1);
				if (tick == 8)
				{
					for (int i = 0; i < charAttack.Length; i++)
					{
						MonsterDart.addMonsterDart(x + ((dir != 1) ? (-45) : 45), y - 30, isBoss: true, dameHP[i], 0L, charAttack[i], 24);
					}
				}
			}
			if (type == 1)
			{
				if (tick == ((!haftBody) ? (attack2.Length - 1) : (attack2_1.Length - 1)))
				{
					status = 2;
				}
				dir = ((x < charAttack[0].cx) ? 1 : (-1));
				checkFrameTick((!haftBody) ? attack2 : attack2_1);
				x += (charAttack[0].cx - x) / 4;
				y += (charAttack[0].cy - y) / 4;
				if (tick == 18)
				{
					for (int j = 0; j < charAttack.Length; j++)
					{
						charAttack[j].doInjure(dameHP[j], 0L, isCrit: false, isMob: false);
						ServerEffect.addServerEffect(102, charAttack[j].cx, charAttack[j].cy, 1);
					}
				}
			}
			if (type == 8)
			{
			}
			if (type != 2)
			{
				return;
			}
			if (tick == ((!haftBody) ? (attack3.Length - 1) : (attack3_1.Length - 1)))
			{
				status = 2;
			}
			dir = ((x < charAttack[0].cx) ? 1 : (-1));
			checkFrameTick((!haftBody) ? attack3 : attack3_1);
			if (tick == 13)
			{
				GameScr.shock_scr = 10;
				shock = true;
				for (int k = 0; k < charAttack.Length; k++)
				{
					charAttack[k].doInjure(dameHP[k], 0L, isCrit: false, isMob: false);
				}
			}
		}

	public new void updateMobWalk()
		{
		}

	public new bool isUpdate()
		{
			if (status == 0)
			{
				return false;
			}
			return true;
		}

	public new void attackOtherMob(Mob mobToAttack)
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

	public new void removeHoldEff()
		{
			if (holdEffID != 0)
			{
				holdEffID = 0;
			}
		}

	public new void removeBlindEff()
		{
			blindEff = false;
		}

	public new void removeSleepEff()
		{
			sleepEff = false;
		}

}
