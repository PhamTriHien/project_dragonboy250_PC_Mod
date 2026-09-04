using System;

public partial class BigBoss2
{
	public override void setBody(short id)
		{
			changBody = true;
			smallBody = id;
		}

	public override void clearBody()
		{
			changBody = false;
		}

	public new void checkFrameTick(int[] array)
		{
			tick++;
			if (tick > array.Length - 1)
			{
				tick = 0;
			}
			frame = array[tick];
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
			checkFrameTick(stand);
			if (GameCanvas.gameTick % 5 == 0)
			{
				ServerEffect.addServerEffect(167, Res.random(x - getW() / 2, x + getW() / 2), Res.random(getY() + getH() / 2, getY() + getH()), 1);
			}
			if (x != xTo || y != yTo)
			{
				x += (xTo - x) / 4;
				y += (yTo - y) / 4;
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
			checkFrameTick(stand);
			if (x != xTo || y != yTo)
			{
				x += (xTo - x) / 4;
				y += (yTo - y) / 4;
			}
		}

	public void setFly()
		{
			status = 4;
			flyUp = true;
		}

	public void setAttack(Char[] cAttack, long[] dame, sbyte type)
		{
			status = 3;
			charAttack = cAttack;
			dameHP = dame;
			this.type = type;
			tick = 0;
		}

	public new void updateMobAttack()
		{
			if (type == 0)
			{
				if (tick == attack1.Length - 1)
				{
					status = 2;
				}
				dir = ((x < charAttack[0].cx) ? 1 : (-1));
				checkFrameTick(attack1);
				x += (charAttack[0].cx - x) / 4;
				y += (charAttack[0].cy - y) / 4;
				xTo = x;
				if (tick == 8)
				{
					for (int i = 0; i < charAttack.Length; i++)
					{
						charAttack[i].doInjure(dameHP[i], 0L, isCrit: false, isMob: false);
						ServerEffect.addServerEffect(102, charAttack[i].cx, charAttack[i].cy, 1);
					}
				}
			}
			if (type == 1)
			{
				if (tick == attack2.Length - 1)
				{
					status = 2;
				}
				dir = ((x < charAttack[0].cx) ? 1 : (-1));
				checkFrameTick(attack2);
				if (tick == 8)
				{
					for (int j = 0; j < charAttack.Length; j++)
					{
						MonsterDart.addMonsterDart(x + ((dir != 1) ? (-45) : 45), y - 25, isBoss: true, dameHP[j], 0L, charAttack[j], 24);
					}
				}
			}
			if (type != 2)
			{
				return;
			}
			if (tick == fly.Length - 1)
			{
				status = 2;
			}
			dir = ((x < charAttack[0].cx) ? 1 : (-1));
			checkFrameTick(fly);
			x += (charAttack[0].cx - x) / 4;
			xTo = x;
			yTo = y;
			if (tick == 12)
			{
				for (int k = 0; k < charAttack.Length; k++)
				{
					charAttack[k].doInjure(dameHP[k], 0L, isCrit: false, isMob: false);
					ServerEffect.addServerEffect(102, charAttack[k].cx, charAttack[k].cy, 1);
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

	public new bool checkIsBoss()
		{
			if (isBoss || levelBoss > 0)
			{
				return true;
			}
			return false;
		}

}
