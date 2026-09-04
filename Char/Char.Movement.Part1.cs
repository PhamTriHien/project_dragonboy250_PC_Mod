using System;
using Assets.src.e;
using Assets.src.g;
using UnityEngine;
public partial class Char : IMapObject
{
	private void checkPerformEndMovePointAction()
		{
			if (endMovePointCommand != null)
			{
				Command command = endMovePointCommand;
				endMovePointCommand = null;
				command.performAction();
			}
		}
	private void updateCharDeadFly()
		{
			isFreez = false;
			if (isCharge)
			{
				isCharge = false;
				SoundMn.gI().taitaoPause();
				Service.gI().skill_not_focus(3);
			}
			cp1++;
			cx += (cp2 - cx) / 4;
			if (cp1 > 7)
			{
				cy += (cp3 - cy) / 4;
			}
			else
			{
				cy += cp1 - 10;
			}
			if (Res.abs(cp2 - cx) < 4 && Res.abs(cp3 - cy) < 10)
			{
				cx = cp2;
				cy = cp3;
				statusMe = 14;
				if (me)
				{
					GameScr.gI().resetButton();
					Service.gI().charMove();
				}
			}
			cf = 23;
		}
	public void updateCharAutoJump()
		{
			isFreez = false;
			if (isCharge)
			{
				isCharge = false;
				SoundMn.gI().taitaoPause();
				Service.gI().skill_not_focus(3);
			}
			cx += cvx * cdir;
			cy += cvyJump;
			cvyJump++;
			if (cp1 == 0)
			{
				cf = 7;
			}
			else
			{
				cf = 23;
			}
			if (cvyJump == -3)
			{
				cf = 8;
			}
			else if (cvyJump == -2)
			{
				cf = 9;
			}
			else if (cvyJump == -1)
			{
				cf = 10;
			}
			else if (cvyJump == 0)
			{
				cf = 11;
			}
			if (cvyJump == 0)
			{
				statusMe = 6;
				cp3 = 0;
				((MovePoint)vMovePoints.firstElement()).status = 4;
				isJump = true;
				cp1 = 0;
				cvy = 1;
			}
		}
	public void setAutoJump()
		{
			int num = ((MovePoint)vMovePoints.firstElement()).xEnd - cx;
			cvyJump = -10;
			cp1 = 0;
			cdir = ((num > 0) ? 1 : (-1));
			if (num <= 6)
			{
				cvx = 0;
			}
			else if (num <= 20)
			{
				cvx = 3;
			}
			else
			{
				cvx = 5;
			}
		}
	public void updateCharJump()
		{
			setMountIsStart();
			ty = 0;
			isFreez = false;
			if (isCharge)
			{
				isCharge = false;
				SoundMn.gI().taitaoPause();
				Service.gI().skill_not_focus(3);
			}
			addDustEff(3);
			cx += cvx;
			cy += cvy;
			if (cy < 0)
			{
				cy = 0;
				cvy = -1;
			}
			cvy++;
			if (cvy > 0)
			{
				cvy = 0;
			}
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
			if (cvy == 0)
			{
				if (!isAttFly)
				{
					if (me)
					{
						setCharFallFromJump();
					}
					else
					{
						stop();
					}
				}
				else
				{
					setCharFallFromJump();
				}
			}
			if (me && !ischangingMap && isInWaypoint())
			{
				Service.gI().charMove();
				if (TileMap.isTrainingMap())
				{
					ischangingMap = true;
					Service.gI().getMapOffline();
				}
				else
				{
					Service.gI().requestChangeMap();
				}
				isLockKey = true;
				ischangingMap = true;
				GameCanvas.clearKeyHold();
				GameCanvas.clearKeyPressed();
				InfoDlg.showWait();
				return;
			}
			if (statusMe != 16 && (TileMap.tileTypeAt(cx, cy - ch + 24, 8192) || cy < 0))
			{
				statusMe = 4;
				cp1 = 0;
				cp2 = 0;
				cvy = 1;
				delayFall = 0;
				if (cy < 0)
				{
					cy = 0;
				}
				cy = TileMap.tileYofPixel(cy + 25);
				GameCanvas.clearKeyHold();
			}
			if (cp3 < 0)
			{
				cp3++;
			}
			cf = 7;
			if (!me && currentMovePoint != null && cy < currentMovePoint.yEnd)
			{
				stop();
			}
		}
	public bool checkInRangeJump(int x1, int xw1, int xmob, int y1, int yh1, int ymob)
		{
			if (xmob > xw1 || xmob < x1 || ymob > y1 || ymob < yh1)
			{
				return false;
			}
			return true;
		}
	public void setCharFallFromJump()
		{
			cyStartFall = cy;
			cp1 = 0;
			cp2 = 0;
			statusMe = 10;
			cvx = cdir << 2;
			cvy = 0;
			cy = TileMap.tileYofPixel(cy) + 12;
			if (me && currentMovePoint == null && (cx - cxSend != 0 || cy - cySend != 0) && (Res.abs(myCharz().cx - myCharz().cxSend) > 200 || Res.abs(myCharz().cy - myCharz().cySend) > 50))
			{
				Service.gI().charMove();
			}
		}

}
