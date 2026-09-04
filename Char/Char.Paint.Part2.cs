using System;
using Assets.src.e;
using Assets.src.g;
using UnityEngine;
public partial class Char : IMapObject
{
	public void setSkillPaint(SkillPaint skillPaint, int sType)
			{
				bool alreadySent = hasSendAttack;
				if (!alreadySent)
				{
					hasSendAttack = false;
				}
				if (stone || (me && myskill.template.id == 9 && cHP <= cHPFull / 10))
				{
					return;
				}
				if (me)
				{
					if (mobFocus == null && charFocus == null)
					{
						stopUseChargeSkill();
					}
					if (mobFocus != null && (mobFocus.status == 1 || mobFocus.status == 0))
					{
						stopUseChargeSkill();
					}
					if (charFocus != null && (charFocus.statusMe == 14 || charFocus.statusMe == 5))
					{
						stopUseChargeSkill();
					}
					if ((myskill.template.id == 23 && ((charFocus != null && charFocus.holdEffID != 0) || (mobFocus != null && mobFocus.holdEffID != 0) || holdEffID != 0)) || sleepEff || blindEff)
					{
						return;
					}
				}
				Res.outz("skill id= " + skillPaint.id);
				if ((me && dart != null) || TileMap.isOfflineMap())
				{
					return;
				}
				long num = mSystem.currentTimeMillis();
				if (me)
				{
					if (isSelectingSkillBuffToPlayer() && charFocus == null)
					{
						return;
					}
					if (!alreadySent && num - myskill.lastTimeUseThisSkill < myskill.coolDown)
					{
						myskill.paintCanNotUseSkill = true;
						return;
					}
					if (!alreadySent)
					{
						myskill.lastTimeUseThisSkill = num;
					}
					if (myskill.template.manaUseType == 2)
					{
						cMP = 1L;
					}
					else if (myskill.template.manaUseType != 1)
					{
						cMP -= myskill.manaUse;
					}
					else
					{
						cMP -= myskill.manaUse * cMPFull / 100;
					}
					myCharz().cStamina--;
					GameScr.gI().isInjureMp = true;
					GameScr.gI().twMp = 0L;
					if (cMP < 0)
					{
						cMP = 0L;
					}
				}
				if (me)
				{
					if (myskill.template.id == 10)
					{
						Service.gI().skill_not_focus(4);
					}
					if (myskill.template.id == 11)
					{
						Service.gI().skill_not_focus(4);
					}
					if (myskill.template.id == 7)
					{
						SoundMn.gI().hoisinh();
					}
					if (myskill.template.id == 6)
					{
						Service.gI().skill_not_focus(0);
						GameScr.gI().isUseFreez = true;
						SoundMn.gI().thaiduonghasan();
					}
					if (myskill.template.id == 8)
					{
						if (!isCharge)
						{
							SoundMn.gI().taitaoPause();
							Service.gI().skill_not_focus(1);
							isCharge = true;
							last = (cur = mSystem.currentTimeMillis());
						}
						else
						{
							Service.gI().skill_not_focus(3);
							isCharge = false;
							SoundMn.gI().taitaoPause();
						}
					}
					if (myskill.template.id == 13)
					{
						if (isMonkey != 0)
						{
							GameScr.gI().auto = 0;
						}
						else if (!isCreateDark)
						{
							SoundMn.gI().gong();
							Service.gI().skill_not_focus(6);
							chargeCount = 0;
							isWaitMonkey = true;
						}
						return;
					}
					if (myskill.template.id == 14)
					{
						SoundMn.gI().gong();
						Service.gI().skill_not_focus(7);
						useChargeSkill(isGround: true);
					}
					if (myskill.template.id == 21)
					{
						Service.gI().skill_not_focus(10);
						return;
					}
					if (myskill.template.id == 12)
					{
						Service.gI().skill_not_focus(8);
					}
					if (myskill.template.id == 19)
					{
						Service.gI().skill_not_focus(9);
						return;
					}
				}
				if (isMonkey == 1 && skillPaint.id >= 35 && skillPaint.id <= 41)
				{
					skillPaint = GameScr.sks[106];
				}
				if (skillPaint.id >= 128 && skillPaint.id <= 134)
				{
					skillPaint = GameScr.sks[skillPaint.id - 65];
					if (charFocus != null)
					{
						cx = charFocus.cx;
						cy = charFocus.cy;
						currentMovePoint = null;
					}
					if (mobFocus != null)
					{
						cx = mobFocus.x;
						cy = mobFocus.y;
						currentMovePoint = null;
					}
					ServerEffect.addServerEffect(60, cx, cy, 1);
					telePortSkill = true;
				}
				if (skillPaint.id >= 107 && skillPaint.id <= 113)
				{
					skillPaint = GameScr.sks[skillPaint.id - 44];
					EffecMn.addEff(new Effect(23, cx, cy + ch / 2, 3, 2, 1));
				}
				setAutoSkillPaint(skillPaint, sType);
			}
	public void setAutoSkillPaint(SkillPaint skillPaint, int sType)
			{
				this.skillPaint = skillPaint;
				Res.outz("set auto skill " + ((skillPaint == null) ? "null" : "ko null"));
				if (skillPaint.id >= 0 && skillPaint.id <= 6)
				{
					int num = Res.random(0, skillPaint.id + 4) - 1;
					if (num < 0)
					{
						num = 0;
					}
					if (num > 6)
					{
						num = 6;
					}
					skillPaintRandomPaint = GameScr.sks[num];
				}
				else if (skillPaint.id >= 14 && skillPaint.id <= 20)
				{
					int num2 = Res.random(0, skillPaint.id - 14 + 4) - 1;
					if (num2 < 0)
					{
						num2 = 0;
					}
					if (num2 > 6)
					{
						num2 = 6;
					}
					skillPaintRandomPaint = GameScr.sks[num2 + 14];
				}
				else if (skillPaint.id >= 28 && skillPaint.id <= 34)
				{
					int num3 = Res.random(0, ((isMonkey != 1) ? skillPaint.id : 105) - ((isMonkey != 1) ? 28 : 105) + 4) - 1;
					if (num3 < 0)
					{
						num3 = 0;
					}
					if (num3 > 6)
					{
						num3 = 6;
					}
					if (isMonkey == 1)
					{
						num3 = 0;
					}
					skillPaintRandomPaint = GameScr.sks[num3 + ((isMonkey != 1) ? 28 : 105)];
				}
				else if (skillPaint.id >= 63 && skillPaint.id <= 69)
				{
					int num4 = Res.random(0, skillPaint.id - 63 + 4) - 1;
					if (num4 < 0)
					{
						num4 = 0;
					}
					if (num4 > 6)
					{
						num4 = 6;
					}
					skillPaintRandomPaint = GameScr.sks[num4 + 63];
				}
				else if (skillPaint.id >= 107 && skillPaint.id <= 109)
				{
					int num5 = Res.random(0, skillPaint.id - 107 + 4) - 1;
					if (num5 < 0)
					{
						num5 = 0;
					}
					if (num5 > 6)
					{
						num5 = 6;
					}
					skillPaintRandomPaint = GameScr.sks[num5 + 107];
				}
				else
				{
					skillPaintRandomPaint = skillPaint;
				}
				this.sType = sType;
				indexSkill = 0;
				i0 = (i1 = (i2 = (dx0 = (dx1 = (dx2 = (dy0 = (dy1 = (dy2 = 0))))))));
				eff0 = null;
				eff1 = null;
				eff2 = null;
				cvy = 0;
			}
	public bool isPaint()
			{
				if (cy < GameScr.cmy)
				{
					return false;
				}
				if (cy > GameScr.cmy + GameScr.gH + 30)
				{
					return false;
				}
				if (isOutX())
				{
					return false;
				}
				if (isSetPos)
				{
					return false;
				}
				if (isFusion)
				{
					return false;
				}
				return true;
			}
	private void paint_map_line(mGraphics g)
			{
				if (isPaintNewSkill || x_hint == 0 || y_hint == 0 || statusMe == 14)
				{
					return;
				}
				int arg = 0;
				int x = cx - 30;
				int y = cy - 15;
				int num = -30;
				int num2 = 5;
				if (Res.abs(cy - y_hint) > 150)
				{
					if (cy > y_hint)
					{
						arg = 7;
						x = cx;
						y = cy - 15 - 60;
					}
					else
					{
						arg = 5;
						x = cx;
						y = cy - 15 + 60;
					}
				}
				else if (cx > x_hint)
				{
					arg = 2;
				}
				else if (cx <= x_hint)
				{
					x = cx + 30;
				}
				if (GameCanvas.gameTick % 10 >= 5)
				{
					if (Res.abs(cx - x_hint) > 100)
					{
						g.drawRegion(GameScr.arrow, 0, 0, 13, 16, arg, x, y, StaticObj.VCENTER_HCENTER);
					}
					else if (Res.abs(cx - x_hint) < 50)
					{
						g.drawImage(Panel.imgBantay, x_hint + num, y_hint - 60 + num2, 0);
					}
				}
			}
	private void paintArrowAttack(mGraphics g)
			{
			}

}
