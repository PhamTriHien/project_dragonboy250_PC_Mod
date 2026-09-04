using System;
using Assets.src.e;
using Assets.src.g;
using UnityEngine;
public partial class Char : IMapObject
{
	public void updateCharFall()
		{
			if (holder)
			{
				return;
			}
			ty = 0;
			if (cy + 4 >= TileMap.pxh)
			{
				statusMe = 1;
				if (me)
				{
					SoundMn.gI().charFall();
				}
				cvx = (cvy = 0);
				cp3 = 0;
				return;
			}
			if (cy % 24 == 0 && (TileMap.tileTypeAtPixel(cx, cy) & 2) == 2)
			{
				delayFall = 0;
				if (me)
				{
					if (cy - cySend > 0)
					{
						Service.gI().charMove();
					}
					else if (cx - cxSend != 0 || cy - cySend < 0)
					{
						Service.gI().charMove();
					}
					cvx = (cvy = 0);
					cp1 = (cp2 = 0);
					statusMe = 1;
					cp3 = 0;
					return;
				}
				stop();
				cf = 0;
				GameCanvas.gI().startDust(-1, cx - -8, cy);
				GameCanvas.gI().startDust(1, cx - 8, cy);
				addDustEff(1);
			}
			if (delayFall > 0)
			{
				delayFall--;
				if (delayFall % 10 > 5)
				{
					cy++;
				}
				else
				{
					cy--;
				}
				return;
			}
			if (cvy < -4)
			{
				cf = 7;
			}
			else
			{
				cf = 12;
			}
			cx += cvx;
			if (!me && currentMovePoint != null)
			{
				int num = currentMovePoint.xEnd - cx;
				if (num > 0)
				{
					if (cvx > num)
					{
						cvx = num;
					}
					if (cvx < 0)
					{
						cvx = num;
					}
				}
				else if (num < 0)
				{
					if (cvx < num)
					{
						cvx = num;
					}
					if (cvx > 0)
					{
						cvx = num;
					}
				}
				else
				{
					cvx = num;
				}
			}
			cvy++;
			if (cvy > 8)
			{
				cvy = 8;
			}
			if (skillPaintRandomPaint == null)
			{
				cy += cvy;
			}
			if (cdir == 1)
			{
				if ((TileMap.tileTypeAtPixel(cx + chw, cy - 1) & 4) == 4 && cx <= TileMap.tileXofPixel(cx + chw) + 12)
				{
					cx = TileMap.tileXofPixel(cx + chw) - chw;
					cvx = 0;
				}
			}
			else if ((TileMap.tileTypeAtPixel(cx - chw, cy - 1) & 8) == 8 && cx >= TileMap.tileXofPixel(cx - chw) + 12)
			{
				cx = TileMap.tileXofPixel(cx + 24 - chw) + chw;
				cvx = 0;
			}
			if (cvy > 3 && (cyStartFall == 0 || cyStartFall <= TileMap.tileYofPixel(cy + 3)) && (TileMap.tileTypeAtPixel(cx, cy + 3) & 2) == 2)
			{
				if (me)
				{
					cyStartFall = 0;
					cvx = (cvy = 0);
					cp1 = (cp2 = 0);
					cy = TileMap.tileXofPixel(cy + 3);
					statusMe = 1;
					if (me)
					{
						SoundMn.gI().charFall();
					}
					cp3 = 0;
					GameCanvas.gI().startDust(-1, cx - -8, cy);
					GameCanvas.gI().startDust(1, cx - 8, cy);
					addDustEff(1);
					if (cy - cySend > 0)
					{
						if (me)
						{
							Service.gI().charMove();
						}
					}
					else if ((cx - cxSend != 0 || cy - cySend < 0) && me)
					{
						Service.gI().charMove();
					}
				}
				else
				{
					stop();
					cy = TileMap.tileXofPixel(cy + 3);
					cf = 0;
					GameCanvas.gI().startDust(-1, cx - -8, cy);
					GameCanvas.gI().startDust(1, cx - 8, cy);
					addDustEff(1);
					currentMovePoint = null;
				}
				return;
			}
			cf = 12;
			if (me)
			{
				if (!isAttack)
				{
				}
				return;
			}
			if ((TileMap.tileTypeAtPixel(cx, cy + 1) & 2) == 2)
			{
				cf = 0;
			}
			if (currentMovePoint != null && cy > currentMovePoint.yEnd)
			{
				stop();
				cy = TileMap.tileXofPixel(cy + 3);
				currentMovePoint = null;
			}
		}

}
