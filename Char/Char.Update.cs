using System;
using Assets.src.e;
using Assets.src.g;
using UnityEngine;

public partial class Char : IMapObject
{
	public void soundUpdate()
		{
			if (me && statusMe == 10 && cf == 8 && ty > 20 && GameCanvas.gameTick % 20 == 0)
			{
				SoundMn.gI().charFly();
			}
			if (skillPaint != null && skillInfoPaint() != null && indexSkill < skillInfoPaint().Length && isPunchKickSkill() && (me || (!me && cx >= GameScr.cmx && cx <= GameScr.cmx + GameCanvas.w)) && GameCanvas.gameTick % 5 == 0)
			{
				if (cf == 9 || cf == 10 || cf == 11)
				{
					SoundMn.gI().charPunch(isKick: true, (!me) ? 0.05f : 0.1f);
				}
				else
				{
					SoundMn.gI().charPunch(isKick: false, (!me) ? 0.05f : 0.1f);
				}
			}
		}

	private void updateEffect()
		{
			if (effPaints != null)
			{
				for (int i = 0; i < effPaints.Length; i++)
				{
					if (effPaints[i] == null)
					{
						continue;
					}
					if (effPaints[i].eMob != null)
					{
						if (!effPaints[i].isFly)
						{
							effPaints[i].eMob.setInjure();
							effPaints[i].eMob.injureBy = this;
							if (me)
							{
								effPaints[i].eMob.hpInjure = myCharz().cDamFull / 2 - myCharz().cDamFull * NinjaUtil.randomNumber(11) / 100;
							}
							int num = effPaints[i].eMob.h >> 1;
							if (effPaints[i].eMob.isBigBoss())
							{
								num = effPaints[i].eMob.getY() + 20;
							}
							GameScr.startSplash(effPaints[i].eMob.x, effPaints[i].eMob.y - num, cdir);
							effPaints[i].isFly = true;
						}
					}
					else if (effPaints[i].eChar != null && !effPaints[i].isFly)
					{
						if (effPaints[i].eChar.charID >= 0)
						{
							effPaints[i].eChar.doInjure();
						}
						GameScr.startSplash(effPaints[i].eChar.cx, effPaints[i].eChar.cy - (effPaints[i].eChar.ch >> 1), cdir);
						effPaints[i].isFly = true;
					}
					effPaints[i].index++;
					if (effPaints[i].index >= effPaints[i].effCharPaint.arrEfInfo.Length)
					{
						effPaints[i] = null;
					}
				}
			}
			if (indexEff >= 0 && eff != null && GameCanvas.gameTick % 2 == 0)
			{
				indexEff++;
				if (indexEff >= eff.arrEfInfo.Length)
				{
					indexEff = -1;
					eff = null;
				}
			}
			if (indexEffTask >= 0 && effTask != null && GameCanvas.gameTick % 2 == 0)
			{
				indexEffTask++;
				if (indexEffTask >= effTask.arrEfInfo.Length)
				{
					indexEffTask = -1;
					effTask = null;
				}
			}
		}

	private void updateMobMe()
		{
			if (tMobMeBorn != 0)
			{
				tMobMeBorn--;
			}
			if (tMobMeBorn == 0)
			{
				mobMe.xFirst = ((cdir != 1) ? (cx + 30) : (cx - 30));
				mobMe.yFirst = cy - 60;
				int num = mobMe.xFirst - mobMe.x;
				int num2 = mobMe.yFirst - mobMe.y;
				mobMe.x += num / 4;
				mobMe.y += num2 / 4;
				mobMe.dir = cdir;
			}
		}

	private void updateResetPoint()
		{
			InfoDlg.hide();
			GameCanvas.clearAllPointerEvent();
			currentMovePoint = null;
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
				statusMe = 1;
				cp3 = 0;
				ischangingMap = false;
				Service.gI().charMove();
			}
			cf = 23;
		}

	public void updateCharStand()
		{
			isSoundJump = false;
			isAttack = false;
			isAttFly = false;
			cvx = 0;
			cvy = 0;
			cp1++;
			if (cp1 > 30)
			{
				cp1 = 0;
			}
			if (cp1 % 15 < 5)
			{
				cf = 0;
			}
			else
			{
				cf = 1;
			}
			updateCharInBridge();
			if (!me)
			{
				cp3++;
				if (cp3 > 50)
				{
					cp3 = 0;
					currentMovePoint = null;
				}
			}
			updateSuperEff();
			if (!me || GameScr.vCharInMap.size() == 0 || TileMap.mapID != 50)
			{
				return;
			}
			Char @char = (Char)GameScr.vCharInMap.elementAt(0);
			if (!@char.changePos)
			{
				if (@char.statusMe != 2)
				{
					@char.moveTo(cx - 45, cy, 0);
				}
				@char.lastUpdateTime = mSystem.currentTimeMillis();
				if (Res.abs(cx - 45 - @char.cx) <= 10)
				{
					@char.changePos = true;
				}
			}
			else
			{
				if (@char.statusMe != 2)
				{
					@char.moveTo(cx + 45, cy, 0);
				}
				@char.lastUpdateTime = mSystem.currentTimeMillis();
				if (Res.abs(cx + 45 - @char.cx) <= 10)
				{
					@char.changePos = false;
				}
			}
			if (GameCanvas.gameTick % 100 == 0)
			{
				@char.addInfo("Cắc cùm cum");
			}
		}

	public void updateSuperEff()
		{
			if (isCopy || isFusion || isSetPos || isPet || isMiniPet || isMonkey == 1 || (me && !isPaintAura2 && idAuraEff > -1) || (!me && idAuraEff > -1))
			{
				return;
			}
			ty++;
			if (clevel < 9 || clevel >= 14)
			{
				return;
			}
			if ((ty == 40 || ty == 50) && !GameCanvas.lowGraphic)
			{
				GameCanvas.gI().startDust(-1, cx + 8, cy);
				GameCanvas.gI().startDust(1, cx - 8, cy);
				addDustEff(1);
			}
			if (ty <= 50)
			{
				return;
			}
			switch (cgender)
			{
			case 0:
				if (GameCanvas.gameTick % 25 == 0)
				{
					ServerEffect.addServerEffect(114, this, 1);
				}
				if (clevel >= 13 && GameCanvas.gameTick % 4 == 0)
				{
					ServerEffect.addServerEffect(132, this, 1);
				}
				break;
			case 1:
				if (GameCanvas.gameTick % 4 == 0)
				{
					ServerEffect.addServerEffect(132, this, 1);
				}
				if (clevel >= 13 && GameCanvas.gameTick % 12 == 0)
				{
					ServerEffect.addServerEffect(114, this, 1);
				}
				if (clevel >= 13 && GameCanvas.gameTick % 25 == 0)
				{
					ServerEffect.addServerEffect(131, this, 1);
				}
				break;
			case 2:
				if (GameCanvas.gameTick % 4 == 0)
				{
					ServerEffect.addServerEffect(131, this, 1);
				}
				if (clevel >= 13 && GameCanvas.gameTick % 25 == 0)
				{
					ServerEffect.addServerEffect(114, this, 1);
				}
				break;
			}
		}

	public void updateCharRun()
		{
			int num = ((isMonkey != 1 || me) ? 1 : 1);
			if (cx >= GameScr.cmx && cx <= GameScr.cmx + GameCanvas.w)
			{
				if (isMonkey == 0)
				{
					SoundMn.gI().charRun(getSoundVolumn());
				}
				else
				{
					SoundMn.gI().monkeyRun(getSoundVolumn());
				}
			}
			ty = 0;
			isFreez = false;
			if (isCharge)
			{
				isCharge = false;
				SoundMn.gI().taitaoPause();
				Service.gI().skill_not_focus(3);
			}
			int num2 = 0;
			if (!me && currentMovePoint != null)
			{
				num2 = abs(cx - currentMovePoint.xEnd);
			}
			cp1++;
			if (cp1 >= 10)
			{
				cp1 = 0;
				cBonusSpeed = 0;
			}
			cf = (cp1 >> 1) + 2;
			if ((TileMap.tileTypeAtPixel(cx, cy - 1) & 0x40) == 64)
			{
				cx += cvx * num >> 1;
			}
			else
			{
				cx += cvx * num;
			}
			if (cdir == 1)
			{
				if (TileMap.tileTypeAt(cx + chw, cy - chh, 4))
				{
					if (me)
					{
						cvx = 0;
						cx = TileMap.tileXofPixel(cx + chw) - chw;
					}
					else
					{
						stop();
					}
				}
			}
			else if (TileMap.tileTypeAt(cx - chw - 1, cy - chh, 8))
			{
				if (me)
				{
					cvx = 0;
					cx = TileMap.tileXofPixel(cx - chw - 1) + TileMap.size + chw;
				}
				else
				{
					stop();
				}
			}
			if (me)
			{
				if (cvx > 0)
				{
					cvx--;
				}
				else if (cvx < 0)
				{
					cvx++;
				}
				else
				{
					if (cx - cxSend != 0 && me)
					{
						Service.gI().charMove();
					}
					statusMe = 1;
					cBonusSpeed = 0;
				}
			}
			if ((TileMap.tileTypeAtPixel(cx, cy) & 2) != 2)
			{
				if (me)
				{
					if (cx - cxSend != 0 || cy - cySend != 0)
					{
						Service.gI().charMove();
					}
					cf = 7;
					statusMe = 4;
					delayFall = 0;
					cvx = 3 * cdir;
					cp2 = 0;
				}
				else
				{
					stop();
				}
			}
			if (!me && currentMovePoint != null)
			{
				int num3 = abs(cx - currentMovePoint.xEnd);
				if (num3 > num2)
				{
					stop();
				}
			}
			GameCanvas.gI().startDust(cdir, cx - (cdir << 3), cy);
			updateCharInBridge();
			addDustEff(2);
		}

	public void updateMount()
		{
			frameMount++;
			if (frameMount > FrameMount.Length - 1)
			{
				frameMount = 0;
			}
			frameNewMount++;
			if (frameNewMount > 1000)
			{
				frameNewMount = 0;
			}
			if (isStartMount && !isMount)
			{
				yMount = cy;
				if (transMount == 0)
				{
					if (xMount - cx >= speedMount)
					{
						xMount -= speedMount;
						return;
					}
					xMount = cx;
					isMount = true;
					isEndMount = false;
				}
				else if (transMount == 2)
				{
					if (cx - xMount >= speedMount)
					{
						xMount += speedMount;
						return;
					}
					xMount = cx;
					isMount = true;
					isEndMount = false;
				}
			}
			else if (isMount)
			{
				if (statusMe == 14 || ySd - cy < 24)
				{
					setMountIsEnd();
				}
				if (cp1 % 15 < 5)
				{
					cf = 0;
				}
				else
				{
					cf = 1;
				}
				transMount = cdir;
				updateSuperEff();
				if (transMount < 0)
				{
					transMount = 0;
					dxMount = -19;
				}
				else if (transMount == 1)
				{
					transMount = 2;
					dxMount = -31;
					if (isEventMount)
					{
						dxMount = -38;
					}
				}
				if (skillInfoPaint() != null)
				{
					dyMount = -15;
				}
				else
				{
					dyMount = -17;
				}
				yMount = cy;
				xMount = cx;
			}
			else if (isEndMount)
			{
				if (transMount == 0)
				{
					if (xMount > GameScr.cmx - 100)
					{
						xMount -= 20;
						return;
					}
					isStartMount = false;
					isMount = false;
					isEndMount = false;
				}
				else if (transMount == 2)
				{
					if (xMount < GameScr.cmx + GameCanvas.w + 50)
					{
						xMount += 20;
						return;
					}
					isStartMount = false;
					isMount = false;
					isEndMount = false;
				}
			}
			else if (!isStartMount || !isMount || !isEndMount)
			{
				xMount = GameScr.cmx - 100;
				yMount = GameScr.cmy - 100;
			}
		}

	public void checkFrameTick(int[] array)
		{
			t++;
			if (t > array.Length - 1)
			{
				t = 0;
			}
			fM = array[t];
		}

	public void setMabuHold(bool m)
		{
			isMabuHold = m;
		}

	public void setHoldChar(Char r)
		{
			if (cx < r.cx)
			{
				cdir = 1;
			}
			else
			{
				cdir = -1;
			}
			charHold = r;
			holder = true;
		}

	public void setHoldMob(Mob r)
		{
			if (cx < r.x)
			{
				cdir = 1;
			}
			else
			{
				cdir = -1;
			}
			mobHold = r;
			holder = true;
		}

	public void updateCharInBridge()
		{
			if (!GameCanvas.lowGraphic)
			{
				if (TileMap.tileTypeAt(cx, cy + 1, 1024))
				{
					TileMap.setTileTypeAtPixel(cx, cy + 1, 512);
					TileMap.setTileTypeAtPixel(cx, cy - 2, 512);
				}
				if (TileMap.tileTypeAt(cx - TileMap.size, cy + 1, 512))
				{
					TileMap.killTileTypeAt(cx - TileMap.size, cy + 1, 512);
					TileMap.killTileTypeAt(cx - TileMap.size, cy - 2, 512);
				}
				if (TileMap.tileTypeAt(cx + TileMap.size, cy + 1, 512))
				{
					TileMap.killTileTypeAt(cx + TileMap.size, cy + 1, 512);
					TileMap.killTileTypeAt(cx + TileMap.size, cy - 2, 512);
				}
			}
		}

	public void addDustEff(int type)
		{
			if (GameCanvas.lowGraphic)
			{
				return;
			}
			switch (type)
			{
			case 1:
				if (clevel >= 9)
				{
					Effect effect3 = new Effect(19, cx - 5, cy + 20, 2, 1, -1);
					EffecMn.addEff(effect3);
				}
				break;
			case 2:
				if ((!me || isMonkey != 1) && isNhapThe && GameCanvas.gameTick % 5 == 0)
				{
					Effect effect2 = new Effect(22, cx - 5, cy + 35, 2, 1, -1);
					EffecMn.addEff(effect2);
				}
				break;
			case 3:
				if (clevel >= 9 && ySd - cy <= 5)
				{
					Effect effect = new Effect(19, cx - 5, ySd + 20, 2, 1, -1);
					EffecMn.addEff(effect);
				}
				break;
			}
		}

	public void addEffChar(Effect e)
		{
			removeEffChar(0, e.effId);
			vEffChar.addElement(e);
		}

	public void updEffChar()
		{
			for (int i = 0; i < vEffChar.size(); i++)
			{
				((Effect)vEffChar.elementAt(i)).update();
			}
		}

	public void updateEye()
		{
			if (head != 934)
			{
				return;
			}
			if (GameCanvas.timeNow - timeAddChopmat > 0)
			{
				fChopmat++;
				if (fChopmat > frEye.Length - 1)
				{
					fChopmat = 0;
					timeAddChopmat = GameCanvas.timeNow + Res.random(2000, 3500);
					frEye = frChopCham;
					if (Res.random(2) == 0)
					{
						frEye = frChopNhanh;
					}
				}
			}
			else
			{
				fChopmat = 0;
			}
		}

}
