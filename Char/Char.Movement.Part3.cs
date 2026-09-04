using System;
using Assets.src.e;
using Assets.src.g;
using UnityEngine;
public partial class Char : IMapObject
{
	public void updateCharFly()
		{
			int num = ((isMonkey != 1 || me) ? 1 : 2);
			setMountIsStart();
			if (statusMe != 16 && (TileMap.tileTypeAt(cx, cy - ch + 24, 8192) || cy < 0))
			{
				if (cy - ch < 0)
				{
					cy = ch;
				}
				cf = 7;
				statusMe = 4;
				cvx = 0;
				cp2 = 0;
				currentMovePoint = null;
				return;
			}
			int num2 = cy;
			if (isHead_Fly(head))
			{
				if (GameCanvas.gameTick % 3 == 0)
				{
					cp1++;
				}
				if (cp1 > 4)
				{
					cp1 = 0;
				}
				cf = cp1 + 2;
			}
			else
			{
				cp1++;
				if (cp1 >= 9)
				{
					cp1 = 0;
					if (!me)
					{
						cvx = (cvy = 0);
					}
					cBonusSpeed = 0;
				}
				cf = 8;
				if (Res.abs(cvx) <= 4 && me)
				{
					if (currentMovePoint != null)
					{
						int num3 = abs(cx - currentMovePoint.xEnd);
						int num4 = abs(cy - currentMovePoint.yEnd);
						if (num3 > num4 * 10)
						{
							cf = 8;
						}
						else if (num3 > num4 && num3 > 48 && num4 > 32)
						{
							cf = 8;
						}
						else
						{
							cf = 7;
						}
					}
					else
					{
						if (cvy < 0)
						{
							cvy = 0;
						}
						if (cvy > 16)
						{
							cvy = 16;
						}
						cf = 7;
					}
				}
				if (!me)
				{
					if (abs(cvx) < 2)
					{
						cvx = (cdir << 1) * num;
					}
					if (cvy != 0)
					{
						cf = 7;
					}
					if (abs(cvx) <= 2)
					{
						cp2++;
						if (cp2 > 32)
						{
							statusMe = 4;
							cvx = 0;
							cvy = 0;
						}
					}
				}
			}
			if (cdir == 1)
			{
				if (TileMap.tileTypeAt(cx + chw, cy - 1, 4))
				{
					cvx = 0;
					cx = TileMap.tileXofPixel(cx + chw) - chw;
					if (cvy == 0)
					{
						currentMovePoint = null;
					}
				}
			}
			else if (TileMap.tileTypeAt(cx - chw - 1, cy - 1, 8))
			{
				cvx = 0;
				cx = TileMap.tileXofPixel(cx - chw - 1) + TileMap.size + chw;
				if (cvy == 0)
				{
					currentMovePoint = null;
				}
			}
			cx += cvx * num;
			cy += cvy * num;
			if (!isMount && num2 - cy == 0)
			{
				ty++;
				wt++;
				fy += ((!wy) ? 1 : (-1));
				if (wt == 10)
				{
					wt = 0;
					wy = !wy;
				}
				if (ty > 20)
				{
					delayFall = 10;
					if (GameCanvas.gameTick % 3 == 0)
					{
						ServerEffect.addServerEffect(111, cx + ((cdir != 1) ? 27 : (-17)), cy + fy + 13, 1, (cdir != 1) ? 2 : 0);
					}
				}
			}
			if (!me)
			{
				return;
			}
			if (cvx > 0)
			{
				cvx--;
			}
			else if (cvx < 0)
			{
				cvx++;
			}
			else if (cvy == 0)
			{
				statusMe = 4;
				checkDelayFallIfTooHigh();
				if (currentMovePoint == null && (cx != cxSend || cy != cySend))
				{
					Service.gI().charMove();
				}
			}
			if ((TileMap.tileTypeAtPixel(cx, cy + 20) & 2) == 2 || (TileMap.tileTypeAtPixel(cx, cy + 40) & 2) == 2)
			{
				if (cvy == 0)
				{
					delayFall = 0;
				}
				cyStartFall = 0;
				cvx = (cvy = 0);
				cp1 = (cp2 = 0);
				statusMe = 4;
				addDustEff(3);
			}
			if (me && currentMovePoint == null && (abs(cx - cxSend) > 200 || abs(cy - cySend) > 50))
			{
				Service.gI().charMove();
			}
		}
	private void checkDelayFallIfTooHigh()
		{
			bool flag = true;
			for (int i = 0; i < 150; i += 24)
			{
				if ((TileMap.tileTypeAtPixel(cx, cy + i) & 2) == 2 || cy + i > TileMap.tmh * TileMap.size - 24)
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				delayFall = 40;
			}
		}
	public void moveTo(int toX, int toY, int type)
		{
			if (type == 1 || Res.abs(toX - cx) > 100 || Res.abs(toY - cy) > 300)
			{
				createShadow(cx, cy, 10);
				cx = toX;
				cy = toY;
				vMovePoints.removeAllElements();
				statusMe = 6;
				cp3 = 0;
				currentMovePoint = null;
				cf = 25;
				return;
			}
			int dir = 0;
			int act = 0;
			int num = toX - cx;
			int num2 = toY - cy;
			if (num == 0 && num2 == 0)
			{
				act = 1;
				cp3 = 0;
			}
			else if (num2 == 0)
			{
				act = 2;
				if (num > 0)
				{
					dir = 1;
				}
				if (num < 0)
				{
					dir = -1;
				}
			}
			else if (num2 != 0)
			{
				if (num2 < 0)
				{
					act = 3;
				}
				if (num2 > 0)
				{
					act = 4;
				}
				if (num < 0)
				{
					dir = -1;
				}
				if (num > 0)
				{
					dir = 1;
				}
			}
			vMovePoints.addElement(new MovePoint(toX, toY, act, dir));
			if (statusMe != 6)
			{
				statusBeforeNothing = statusMe;
			}
			statusMe = 6;
			cp3 = 0;
		}
	public void removeHoleEff()
		{
			if (holder)
			{
				holder = false;
				charHold = null;
				mobHold = null;
			}
			else
			{
				holdEffID = 0;
				charHold = null;
				mobHold = null;
			}
		}
	public void removeProtectEff()
		{
			protectEff = false;
			eProtect = null;
		}
	public void removeEffect()
		{
			if (holdEffID != 0)
			{
				holdEffID = 0;
			}
			if (holder)
			{
				holder = false;
			}
			if (protectEff)
			{
				protectEff = false;
			}
			eProtect = null;
			charHold = null;
			mobHold = null;
			blindEff = false;
			sleepEff = false;
		}
	public void setPos(short xPos, short yPos, sbyte typePos)
		{
			isSetPos = true;
			this.xPos = xPos;
			this.yPos = yPos;
			this.typePos = typePos;
			tpos = 0;
			if (me)
			{
				if (GameCanvas.panel != null)
				{
					GameCanvas.panel.hide();
				}
				if (GameCanvas.panel2 != null)
				{
					GameCanvas.panel2.hide();
				}
			}
		}
	public void removeHuytSao()
		{
			huytSao = false;
		}
	public void removeEffChar(int type, int id)
		{
			if (type == -1)
			{
				vEffChar.removeAllElements();
			}
			else if (getEffById(id) != null)
			{
				vEffChar.removeElement(getEffById(id));
			}
		}

}
