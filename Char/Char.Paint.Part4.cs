using System;
using Assets.src.e;
using Assets.src.g;
using UnityEngine;
public partial class Char : IMapObject
{
	public void SetSkillPaint_NEW(short idskillPaint, bool isFly, sbyte typeFrame, sbyte typePaint, sbyte dir, short timeGong, sbyte typeItem)
			{
				isPaintNewSkill = true;
				timeReset_newSkill = GameCanvas.timeNow + 10000;
				this.idskillPaint = idskillPaint;
				this.isFly = isFly;
				this.typeFrame = typeFrame;
				this.typePaint = typePaint;
				this.typeItem = typeItem;
				cdir = dir;
				count_NEW = 0;
				stt = 0;
				long lastTimeUseThisSkill = mSystem.currentTimeMillis();
				if (me)
				{
					saveLoadPreviousSkill();
					myskill.lastTimeUseThisSkill = lastTimeUseThisSkill;
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
				switch (idskillPaint)
				{
				case 24:
					GameScr.addEffectEnd_Target(18, 0, typePaint, clone(), null, 3, timeGong, 0);
					GameScr.addEffectEnd_Target(21, 0, typePaint, clone(), null, 1, timeGong, 0);
					break;
				case 25:
					GameScr.addEffectEnd_Target(19, 0, typePaint, clone(), null, 3, timeGong, 0);
					GameScr.addEffectEnd_Target(22, 0, typePaint, clone(), null, 1, timeGong, 0);
					break;
				case 26:
					GameScr.addEffectEnd_Target(20, 0, typePaint, clone(), null, 3, timeGong, 0);
					GameScr.addEffectEnd_Target(23, 0, typePaint, clone(), null, 1, timeGong, 0);
					break;
				}
				if (this.typeFrame == 1)
				{
					if (!this.isFly)
					{
						fr_start = new byte[7] { 20, 20, 20, 20, 20, 20, 19 };
						fr_atk = new byte[1] { 20 };
						fr_end = new byte[1];
					}
					else
					{
						fr_start = new byte[7] { 31, 31, 31, 31, 31, 31, 30 };
						fr_atk = new byte[1] { 31 };
						fr_end = new byte[1] { 12 };
					}
				}
				if (this.typeFrame == 2)
				{
					if (!this.isFly)
					{
						fr_start = new byte[1] { 20 };
						fr_atk = new byte[6] { 13, 13, 13, 14, 14, 14 };
						fr_end = new byte[1];
					}
					else
					{
						fr_start = new byte[1] { 31 };
						fr_atk = new byte[6] { 26, 26, 26, 27, 27, 27 };
						fr_end = new byte[1] { 12 };
					}
				}
				if (this.typeFrame == 4)
				{
					if (!this.isFly)
					{
						fr_start = new byte[6] { 17, 17, 17, 18, 18, 18 };
						fr_atk = new byte[1] { 18 };
						fr_end = new byte[1];
					}
					else
					{
						fr_start = new byte[7] { 7, 7, 7, 12, 12, 12, 12 };
						fr_atk = new byte[1] { 12 };
						fr_end = new byte[1] { 12 };
					}
				}
				if (this.typeFrame == 3)
				{
					if (!this.isFly)
					{
						fr_start = new byte[9] { 24, 24, 24, 17, 17, 17, 18, 18, 18 };
						fr_atk = new byte[1] { 20 };
						fr_end = new byte[1];
					}
					else
					{
						fr_start = new byte[10] { 23, 23, 23, 7, 7, 7, 12, 12, 12, 12 };
						fr_atk = new byte[1] { 31 };
						fr_end = new byte[1] { 12 };
					}
				}
			}
	public void SetSkillPaint_STT(int stt, short idskillPaint, Point targetDame, short timeDame, short rangeDame, sbyte typePaint, Point[] listObj, sbyte typeItem)
			{
				this.stt = stt;
				this.idskillPaint = idskillPaint;
				count_NEW = 0;
				this.targetDame = targetDame;
				this.typePaint = typePaint;
				this.timeDame = mSystem.currentTimeMillis() + timeDame;
				this.rangeDame = rangeDame;
				this.typeItem = typeItem;
				if (this.stt == 1)
				{
					if (this.idskillPaint == 24)
					{
						GameScr.addEffectEnd_Target(18, 1, typePaint, this, null, 3, timeDame, 0);
						GameScr.addEffectEnd_Target(24, 0, typePaint, this, this.targetDame, 1, timeDame, rangeDame);
					}
					if (this.idskillPaint == 25)
					{
						GameScr.addEffectEnd_Target(19, 0, typePaint, this, null, 3, timeDame, 0);
						GameScr.addEffectEnd_Target(25, 0, typePaint, this, this.targetDame, 1, timeDame, rangeDame);
					}
					if (this.idskillPaint == 26)
					{
						GameScr.addEffectEnd_Target(20, 0, typePaint, this, null, 3, timeDame, 0);
						GameScr.addEffectEnd(26, typeItem, typePaint, targetDame.x, targetDame.y, 1, 0, timeDame, listObj);
					}
				}
			}
	public void UpdSkillPaint_NEW()
			{
				if (stt == 0)
				{
					if (isFly && count_NEW < 20)
					{
						cvy = -3;
						cy += cvy;
					}
					if (fr_start.Length == 1)
					{
						cf = fr_start[0];
					}
					else if (count_NEW > fr_start.Length - 1)
					{
						cf = fr_start[fr_start.Length - 1];
					}
					else
					{
						cf = fr_start[count_NEW];
					}
				}
				else if (stt == 1)
				{
					cf = fr_atk[count_NEW % fr_atk.Length];
					if (mSystem.currentTimeMillis() - timeDame > 0)
					{
						SetSkillPaint_STT(2, 0, null, 0, 0, 0, null, 0);
					}
					if (count_NEW % 5 == 0)
					{
						GameScr.shock_scr = 5;
					}
					if (typeFrame == 1 && count_NEW < 10 && !TileMap.tileTypeAt(cx - (chw + 1) * cdir, cy, (cdir != 1) ? 4 : 8))
					{
						cx -= cdir;
					}
					if (typeFrame != 2)
					{
					}
				}
				else if (stt == 2)
				{
					if (fr_end.Length == 1)
					{
						cf = fr_end[0];
					}
					else if (count_NEW > fr_end.Length - 1)
					{
						cf = fr_end[fr_end.Length - 1];
					}
					else
					{
						cf = fr_end[count_NEW];
					}
					if (isFly)
					{
						cvx = (cvy = 0);
						statusMe = 4;
					}
					isPaintNewSkill = false;
				}
				count_NEW++;
			}

}
