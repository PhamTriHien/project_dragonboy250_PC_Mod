using System;
using Assets.src.e;
using Assets.src.g;
using UnityEngine;

public partial class Char : IMapObject
{
	public int getdxSkill()
		{
			if (myskill != null)
			{
				return myskill.dx;
			}
			return 0;
		}

	public int getdySkill()
		{
			if (myskill != null)
			{
				return myskill.dy;
			}
			return 0;
		}

	public Skill getSkill(SkillTemplate skillTemplate)
		{
			for (int i = 0; i < vSkill.size(); i++)
			{
				if (((Skill)vSkill.elementAt(i)).template.id == skillTemplate.id)
				{
					return (Skill)vSkill.elementAt(i);
				}
			}
			return null;
		}

	public bool isPunchKickSkill()
		{
			if (skillPaint == null)
			{
				return false;
			}
			if (skillPaint.id >= 0 && skillPaint.id <= 6)
			{
				return true;
			}
			if (skillPaint.id >= 14 && skillPaint.id <= 20)
			{
				return true;
			}
			if (skillPaint.id >= 28 && skillPaint.id <= 34)
			{
				return true;
			}
			if (skillPaint.id >= 63 && skillPaint.id <= 69)
			{
				return true;
			}
			return false;
		}

	public void updateChargeSkill()
		{
		}

	public void saveLoadPreviousSkill()
		{
		}

	public void updateSkillFall()
		{
		}

	public void updateSkillStand()
		{
			ty = 0;
			cp1++;
			if (cdir == 1)
			{
				if ((TileMap.tileTypeAtPixel(cx + chw, cy - chh) & 4) == 4)
				{
					cvx = 0;
				}
			}
			else if ((TileMap.tileTypeAtPixel(cx - chw, cy - chh) & 8) == 8)
			{
				cvx = 0;
			}
			if (cy > ch && TileMap.tileTypeAt(cx, cy - ch + 24, 8192))
			{
				if (!TileMap.tileTypeAt(cx, cy, 2))
				{
					statusMe = 4;
					cp1 = 0;
					cp2 = 0;
					cvy = 1;
				}
				else
				{
					cy = TileMap.tileYofPixel(cy);
				}
			}
			cx += cvx;
			cy += cvy;
			if (cy < 0)
			{
				cy = (cvy = 0);
			}
			if (cvy == 0)
			{
				if ((TileMap.tileTypeAtPixel(cx, cy) & 2) != 2)
				{
					statusMe = 4;
					cvx = (cspeed >> 1) * cdir;
					cp1 = (cp2 = 0);
				}
			}
			else if (cvy < 0)
			{
				cvy++;
				if (cvy == 0)
				{
					cvy = 1;
				}
			}
			else
			{
				if (cvy < 20 && cp1 % 5 == 0)
				{
					cvy++;
				}
				if (cvy > 3)
				{
					cvy = 3;
				}
				if ((TileMap.tileTypeAtPixel(cx, cy + 3) & 2) == 2 && cy <= TileMap.tileXofPixel(cy + 3))
				{
					cvx = (cvy = 0);
					cy = TileMap.tileXofPixel(cy + 3);
				}
			}
			if (cvx > 0)
			{
				cvx--;
			}
			else if (cvx < 0)
			{
				cvx++;
			}
		}

	public bool isSelectingSkillUseAlone()
		{
			return myskill != null && myskill.template.isUseAlone();
		}

	public bool isUseSkillSpec()
		{
			return myskill != null && myskill.template.isSkillSpec();
		}

	public bool isSelectingSkillBuffToPlayer()
		{
			return myskill != null && myskill.template.isBuffToPlayer();
		}

	public bool isUseChargeSkill()
		{
			return !isUseSkillAfterCharge && myskill != null && (myskill.template.id == 10 || myskill.template.id == 11);
		}

	public void useSkillNotFocus()
		{
			GameScr.gI().auto = 0;
			myCharz().setSkillPaint(GameScr.sks[myCharz().myskill.skillId], (!TileMap.tileTypeAt(myCharz().cx, myCharz().cy, 2)) ? 1 : 0);
		}

	public void sendUseChargeSkill()
		{
			if (me && (isFreez || isUsePlane))
			{
				GameScr.gI().auto = 0;
				return;
			}
			long num = mSystem.currentTimeMillis();
			if (me && num - myskill.lastTimeUseThisSkill < myskill.coolDown)
			{
				myskill.paintCanNotUseSkill = true;
				return;
			}
			if (myskill.template.id == 10)
			{
				useChargeSkill(isGround: false);
			}
			if (myskill.template.id == 11)
			{
				useChargeSkill(isGround: true);
			}
		}

	public void stopUseChargeSkill()
		{
			isFlyAndCharge = false;
			isStandAndCharge = false;
			isUseSkillAfterCharge = false;
			isCreateDark = false;
			if (me && statusMe != 14 && statusMe != 5)
			{
				isLockMove = false;
			}
			GameScr.gI().auto = 0;
		}

	public void useChargeSkill(bool isGround)
		{
			if (isCreateDark)
			{
				return;
			}
			GameScr.gI().auto = 0;
			if (isGround)
			{
				if (isStandAndCharge)
				{
					return;
				}
				chargeCount = 0;
				seconds = 50000;
				posDisY = 0;
				last = mSystem.currentTimeMillis();
				if (me)
				{
					isLockMove = true;
					if (cgender == 1)
					{
						Service.gI().skill_not_focus(4);
					}
					if (TileMap.mapID == 170 && cgender != 1)
					{
						Service.gI().skill_not_focus(4);
					}
				}
				if (cgender == 1)
				{
					SoundMn.gI().gongName();
				}
				if (TileMap.mapID == 170 && cgender != 1)
				{
					SoundMn.gI().gongName();
				}
				isStandAndCharge = true;
			}
			else if (!isFlyAndCharge)
			{
				if (me)
				{
					GameScr.gI().auto = 0;
					isLockMove = true;
					Service.gI().skill_not_focus(4);
				}
				isUseSkillAfterCharge = false;
				chargeCount = 0;
				isFlyAndCharge = true;
				posDisY = 0;
				seconds = 50000;
				isFlying = TileMap.tileTypeAt(cx, cy, 2);
			}
		}

	public void setAttack()
		{
			if (me)
			{
				SkillPaint skillPaint = skillPaintRandomPaint;
				if (dart != null)
				{
					skillPaint = dart.skillPaint;
				}
				if (skillPaint == null)
				{
					return;
				}
				MyVector myVector = new MyVector();
				MyVector myVector2 = new MyVector();
				if (charFocus != null)
				{
					myVector2.addElement(charFocus);
				}
				else if (mobFocus != null)
				{
					myVector.addElement(mobFocus);
				}
				effPaints = new EffectPaint[myVector.size() + myVector2.size()];
				for (int i = 0; i < myVector.size(); i++)
				{
					effPaints[i] = new EffectPaint();
					effPaints[i].effCharPaint = GameScr.efs[skillPaint.effectHappenOnMob - 1];
					if (!isSelectingSkillUseAlone())
					{
						effPaints[i].eMob = (Mob)myVector.elementAt(i);
					}
				}
				for (int j = 0; j < myVector2.size(); j++)
				{
					effPaints[j + myVector.size()] = new EffectPaint();
					effPaints[j + myVector.size()].effCharPaint = GameScr.efs[skillPaint.effectHappenOnMob - 1];
					effPaints[j + myVector.size()].eChar = (Char)myVector2.elementAt(j);
				}
				int type = 0;
				if (mobFocus != null)
				{
					type = 1;
				}
				else if (charFocus != null)
				{
					type = 2;
				}
				if (myVector.size() == 0 && myVector2.size() == 0)
				{
					stopUseChargeSkill();
				}
				if (me && !isSelectingSkillUseAlone() && !hasSendAttack)
				{
					Service.gI().sendPlayerAttack(myVector, myVector2, type);
					hasSendAttack = true;
				}
				return;
			}
			SkillPaint skillPaint2 = skillPaintRandomPaint;
			if (dart != null)
			{
				skillPaint2 = dart.skillPaint;
			}
			if (skillPaint2 == null)
			{
				return;
			}
			if (attMobs != null)
			{
				effPaints = new EffectPaint[attMobs.Length];
				for (int k = 0; k < attMobs.Length; k++)
				{
					effPaints[k] = new EffectPaint();
					effPaints[k].effCharPaint = GameScr.efs[skillPaint2.effectHappenOnMob - 1];
					effPaints[k].eMob = attMobs[k];
				}
				attMobs = null;
			}
			else if (attChars != null)
			{
				effPaints = new EffectPaint[attChars.Length];
				for (int l = 0; l < attChars.Length; l++)
				{
					effPaints[l] = new EffectPaint();
					effPaints[l].effCharPaint = GameScr.efs[skillPaint2.effectHappenOnMob - 1];
					effPaints[l].eChar = attChars[l];
				}
				attChars = null;
			}
		}

	public static void getcharInjure(int cID, int dx, int dy, long HP)
		{
			Char @char = (Char)GameScr.vCharInMap.elementAt(cID);
			if (@char.vMovePoints.size() != 0)
			{
				MovePoint movePoint = (MovePoint)@char.vMovePoints.lastElement();
				int xEnd = movePoint.xEnd + dx;
				int yEnd = movePoint.yEnd + dy;
				Char char2 = (Char)GameScr.vCharInMap.elementAt(cID);
				char2.cHP -= HP;
				if (char2.cHP < 0)
				{
					char2.cHP = 0L;
				}
				char2.cHPShow = ((Char)GameScr.vCharInMap.elementAt(cID)).cHP - HP;
				char2.statusMe = 6;
				char2.cp3 = 0;
				char2.vMovePoints.addElement(new MovePoint(xEnd, yEnd, 8, char2.cdir));
			}
		}

	public void doInjure(long HPShow, long MPShow, bool isCrit, bool isMob)
		{
			this.isCrit = isCrit;
			this.isMob = isMob;
			Res.outz("CHP= " + cHP + " dame -= " + HPShow + " HP FULL= " + cHPFull);
			cHP -= HPShow;
			cMP -= MPShow;
			GameScr.gI().isInjureHp = true;
			GameScr.gI().twHp = 0L;
			GameScr.gI().isInjureMp = true;
			GameScr.gI().twMp = 0L;
			if (cHP < 0)
			{
				cHP = 0L;
			}
			if (cMP < 0)
			{
				cMP = 0L;
			}
			if (isMob || (!isMob && cTypePk != 4 && damMP != -100))
			{
				if (HPShow <= 0)
				{
					if (me)
					{
						GameScr.startFlyText(mResources.miss, cx, cy - ch, 0, -2, mFont.MISS_ME);
					}
					else
					{
						GameScr.startFlyText(mResources.miss, cx, cy - ch, 0, -2, mFont.MISS);
					}
				}
				else
				{
					GameScr.startFlyText("-" + HPShow, cx, cy - ch, 0, -2, isCrit ? mFont.FATAL : mFont.RED);
				}
			}
			if (HPShow > 0)
			{
				isInjure = 6;
			}
			ServerEffect.addServerEffect(80, this, 1);
			if (isDie)
			{
				isDie = false;
				isLockKey = false;
				startDie((short)xSd, (short)ySd);
			}
		}

	public void doInjure()
		{
			GameScr.gI().isInjureHp = true;
			GameScr.gI().twHp = 0L;
			GameScr.gI().isInjureMp = true;
			GameScr.gI().twMp = 0L;
			isInjure = 6;
			ServerEffect.addServerEffect(8, this, 1);
			isInjureHp = true;
			twHp = 0;
		}

	public void startDie(short toX, short toY)
		{
			isMonkey = 0;
			isWaitMonkey = false;
			if (me && isDie)
			{
				return;
			}
			if (me)
			{
				isLockMove = true;
				for (int i = 0; i < GameScr.vCharInMap.size(); i++)
				{
					Char @char = (Char)GameScr.vCharInMap.elementAt(i);
					@char.killCharId = -9999;
				}
				if (GameCanvas.panel != null && GameCanvas.panel.cp != null)
				{
					GameCanvas.panel.cp = null;
				}
				if (GameCanvas.panel2 != null && GameCanvas.panel2.cp != null)
				{
					GameCanvas.panel2.cp = null;
				}
			}
			statusMe = 5;
			cp2 = toX;
			cp3 = toY;
			cp1 = 0;
			cHP = 0L;
			testCharId = -9999;
			killCharId = -9999;
			if (me && myskill != null && myskill.template.id != 14)
			{
				stopUseChargeSkill();
			}
			cTypePk = 0;
		}

	public void waitToDie(short toX, short toY)
		{
			wdx = toX;
			wdy = toY;
		}

	public bool isMeCanAttackOtherPlayer(Char cAtt)
		{
			if (cAtt == null || myCharz().myskill == null || myCharz().myskill.template.type == 2 || (myCharz().myskill.template.type == 4 && cAtt.statusMe != 14 && cAtt.statusMe != 5))
			{
				return false;
			}
			return ((cAtt.cTypePk == 3 && myCharz().cTypePk == 3) || myCharz().cTypePk == 5 || cAtt.cTypePk == 5 || (myCharz().cTypePk == 1 && cAtt.cTypePk == 1) || (myCharz().cTypePk == 4 && cAtt.cTypePk == 4) || (myCharz().testCharId >= 0 && myCharz().testCharId == cAtt.charID) || (myCharz().killCharId >= 0 && myCharz().killCharId == cAtt.charID && !isLang()) || (cAtt.killCharId >= 0 && cAtt.killCharId == myCharz().charID && !isLang()) || (myCharz().cFlag == 8 && cAtt.cFlag != 0) || (myCharz().cFlag != 0 && cAtt.cFlag == 8) || (myCharz().cFlag != cAtt.cFlag && myCharz().cFlag != 0 && cAtt.cFlag != 0)) && cAtt.statusMe != 14 && cAtt.statusMe != 5;
		}

	public void cancelAttack()
		{
		}

	public bool focusToAttack()
		{
			return mobFocus != null || (charFocus != null && isMeCanAttackOtherPlayer(charFocus));
		}

	public void removeBlindEff()
		{
			blindEff = false;
		}

	public void fusionComplete()
		{
			isFusion = false;
			isLockKey = false;
			tFusion = 0;
		}

	public void setFusion(sbyte fusion)
		{
			tFusion = 0;
			if (fusion == 4 || fusion == 5)
			{
				if (me)
				{
					Service.gI().funsion(fusion);
				}
				EffecMn.addEff(new Effect(34, cx, cy + 12, 2, 1, -1));
			}
			if (fusion == 6)
			{
				EffecMn.addEff(new Effect(38, cx, cy + 12, 2, 1, -1));
			}
			if (me)
			{
				GameCanvas.panel.hideNow();
				isLockKey = true;
			}
			isFusion = true;
			if (fusion == 1)
			{
				isNhapThe = false;
			}
			else
			{
				isNhapThe = true;
			}
		}

	public void removeSleepEff()
		{
			sleepEff = false;
		}

	public void sendNewAttack(short idTemplateSkill)
		{
			short x = -1;
			short y = -1;
			if (mobFocus != null)
			{
				x = (short)mobFocus.x;
				y = (short)mobFocus.y;
			}
			if (charFocus != null && !charFocus.isPet && !charFocus.isMiniPet)
			{
				x = (short)charFocus.cx;
				y = (short)charFocus.cy;
			}
			Service.gI().new_skill_not_focus((sbyte)idTemplateSkill, (sbyte)cdir, x, y);
		}

}
