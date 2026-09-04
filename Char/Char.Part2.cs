using System;
using Assets.src.e;
using Assets.src.g;
using UnityEngine;
public partial class Char : IMapObject
{
	public virtual void paint(mGraphics g)
			{
				if (isHide)
				{
					return;
				}
				if (isMafuba)
				{
					paintCharWithoutSkill(g);
				}
				else if (isMabuHold)
				{
					if (cmtoChar)
					{
						GameScr.cmtoX = cx - GameScr.gW2;
						GameScr.cmtoY = cy - GameScr.gH23;
						if (!GameCanvas.isTouchControl)
						{
							GameScr.cmtoX += GameScr.gW6 * cdir;
						}
					}
				}
				else
				{
					if (!isPaint() || (!me && GameScr.notPaint))
					{
						return;
					}
					if (petFollow != null)
					{
						petFollow.paint(g);
					}
					paintMount1(g);
					if ((TileMap.isInAirMap() && cy >= TileMap.pxh - 48) || isTeleport)
					{
						return;
					}
					if (holder && GameCanvas.gameTick % 2 == 0)
					{
						g.setColor(16185600);
						if (charHold != null)
						{
							g.drawLine(cx, cy - ch / 2, charHold.cx, charHold.cy - charHold.ch / 2);
						}
						if (mobHold != null)
						{
							g.drawLine(cx, cy - ch / 2, mobHold.x, mobHold.y - mobHold.h / 2);
						}
					}
					paintSuperEffBehind(g);
					paintAuraBehind(g);
					paintEffBehind(g);
					paintEff_Lvup_behind(g);
					paintEff_Pet(g);
					if (shadowLife > 0)
					{
						if (GameCanvas.gameTick % 2 == 0)
						{
							paintCharBody(g, shadowX, shadowY, cdir, 25, isPaintBag: true);
						}
						else if (shadowLife > 5)
						{
							paintCharBody(g, shadowX, shadowY, cdir, 7, isPaintBag: true);
						}
					}
					if (!isPaint() && skillPaint != null && (skillPaint.id < 70 || skillPaint.id > 76) && (skillPaint.id < 77 || skillPaint.id > 83))
					{
						if (skillPaint != null)
						{
							indexSkill = skillInfoPaint().Length;
							skillPaint = null;
						}
						effPaints = null;
						eff = null;
						effTask = null;
						indexEff = -1;
						indexEffTask = -1;
					}
					else if (statusMe != 15 && (moveFast == null || moveFast[0] <= 0))
					{
						paintCharName_HP_MP_Overhead(g);
						if (skillPaint == null || skillInfoPaint() == null || indexSkill >= skillInfoPaint().Length)
						{
							paintCharWithoutSkill(g);
						}
						if (arr != null)
						{
							arr.paint(g);
						}
						if (dart != null)
						{
							dart.paint(g);
						}
						paintEffect(g);
						if (mobMe != null)
						{
						}
						paintMount2(g);
						paintEff_Lvup_front(g);
						paintSuperEffFront(g);
						paintAuraFront(g);
						paintEffFront(g);
						paint_map_line(g);
					}
				}
			}
	public void liveFromDead()
			{
				cHP = cHPFull;
				cMP = cMPFull;
				statusMe = 1;
				cp1 = (cp2 = (cp3 = 0));
				ServerEffect.addServerEffect(109, this, 2);
				GameScr.gI().center = null;
				GameScr.isHaveSelectSkill = true;
			}
	public void stopMoving()
			{
			}
	public Effect getEffById(int id)
			{
				for (int i = 0; i < vEffChar.size(); i++)
				{
					Effect effect = (Effect)vEffChar.elementAt(i);
					if (effect.effId == id)
					{
						return effect;
					}
				}
				return null;
			}
	public void printlog()
			{
				string empty = string.Empty;
				string text = empty;
				empty = text + "isInjure " + isInjure + "\n";
				text = empty;
				empty = text + "isInjure " + isMonkey + "\n";
				text = empty;
				empty = text + "isInjure " + isAddChopMat + "\n";
				text = empty;
				empty = text + "isInjure " + isAttack + "\n";
				text = empty;
				empty = text + "isInjure " + isAttFly + "\n";
				text = empty;
				empty = text + "isInjure " + ischangingMap + "\n";
				text = empty;
				empty = text + "isInjure " + isCharge + "\n";
				text = empty;
				empty = text + "isInjure " + isCopy + "\n";
				text = empty;
				empty = text + "isInjure " + isCreateDark + "\n";
				text = empty;
				empty = text + "isInjure " + isCrit + "\n";
				text = empty;
				empty = text + "isInjure " + isDirtyPostion + "\n";
				text = empty;
				empty = text + "isInjure " + isEndMount + "\n";
				text = empty;
				empty = text + "isInjure " + isEventMount + "\n";
				text = empty;
				empty = text + "isInjure " + isMafuba + "\n";
				text = empty;
				empty = text + "isInjure " + isFusion + "\n";
				text = empty;
				empty = text + "isInjure " + isFeetEff + "\n";
				text = empty;
				empty = text + "isInjure " + isFlying + "\n";
				text = empty;
				empty = text + "isInjure " + isWaitMonkey + "\n";
				text = empty;
				empty = text + "isInjure " + isUseSkillSpec() + "\n";
				text = empty;
				empty = text + "isInjure " + isDie + "\n";
				text = empty;
				empty = text + "isInjure " + isDie + "\n";
				text = empty;
				empty = text + "isInjure " + isDie + "\n";
				text = empty;
				empty = text + "isInjure " + isDie + "\n";
				Res.outz(empty);
			}

}
