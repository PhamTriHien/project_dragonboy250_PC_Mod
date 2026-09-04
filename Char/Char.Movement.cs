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
