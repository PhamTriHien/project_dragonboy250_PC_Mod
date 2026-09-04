using System;

public partial class NewBoss
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

	public void updateShadown()
		{
			int num = 0;
			xSd = x;
			if (TileMap.tileTypeAt(x, y, 2))
			{
				ySd = y;
				return;
			}
			ySd = y;
			while (num < 30)
			{
				num++;
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
			if (frameArr == null && Mob.arrMobTemplate[templateId].data != null)
			{
				GetFrame();
			}
			if (frameArr == null || !isUpdate())
			{
				return;
			}
			updateShadown();
			switch (status)
			{
			case 2:
				updateMobStandWait();
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
				base.update();
				break;
			case 0:
			case 1:
				updateDead();
				break;
			case 4:
				updateMobFly();
				break;
			}
		}

	private void updateDead()
		{
			tick++;
			if (tick > frameArr[13].Length - 1)
			{
				tick = frameArr[13].Length - 1;
			}
			frame = frameArr[13][tick];
			if (x != xTo || y != yTo)
			{
				x += (xTo - x) / 4;
				y += (yTo - y) / 4;
			}
		}

	private void updateMobFly()
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
			checkFrameTick(frameArr[0]);
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

	public void setAttack(Char[] cAttack, long[] dame, sbyte type, sbyte dir)
		{
			charAttack = cAttack;
			dameHP = dame;
			this.type = type;
			base.dir = dir;
			status = 3;
			if (x != xTo || y != yTo)
			{
				x += (xTo - x) / 4;
				y += (yTo - y) / 4;
			}
		}

	public new void updateMobAttack()
		{
			if (tick == frameArr[type + 1].Length - 1)
			{
				status = 2;
			}
			checkFrameTick(frameArr[type + 1]);
			if (tick == frameArr[15][type - 1])
			{
				for (int i = 0; i < charAttack.Length; i++)
				{
					charAttack[i].doInjure(dameHP[i], 0L, isCrit: false, isMob: false);
					ServerEffect.addServerEffect(frameArr[16][type - 1], charAttack[i].cx, charAttack[i].cy, 1);
				}
			}
		}

	public new void updateMobWalk()
		{
			checkFrameTick(frameArr[1]);
			sbyte speed = Mob.arrMobTemplate[templateId].speed;
			int num = speed;
			if (Res.abs(x - xTo) < speed)
			{
				num = Res.abs(x - xTo);
			}
			x += ((x >= xTo) ? (-num) : num);
			y = yTo;
			if (x < xTo)
			{
				dir = 1;
			}
			else if (x > xTo)
			{
				dir = -1;
			}
			if (Res.abs(x - xTo) <= 1)
			{
				x = xTo;
				status = 2;
			}
		}

	public new bool isUpdate()
		{
			if (status == 0)
			{
				return false;
			}
			return true;
		}

	public new void startDie()
		{
			hp = 0L;
			injureThenDie = true;
			hp = 0L;
			status = 1;
			p1 = -3;
			p2 = -dir;
			p3 = 0;
		}

	public void setDie()
		{
			status = 0;
		}

}
