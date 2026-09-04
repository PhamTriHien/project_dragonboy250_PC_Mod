using System;
using Assets.src.g;
public partial class GameScr : mScreen, IChatable
{
	public static void setSkillBarPosition()
		{
			Skill[] array = ((!GameCanvas.isTouch) ? keySkill : onScreenSkill);
			if (array == null || array.Length == 0)
			{
				array = new Skill[10];
			}
			xS = new int[array.Length];
			yS = new int[array.Length];

			wSkill = 30;
			xSkill = 10;
			ySkill = GameCanvas.h - wSkill - 6;

			for (int i = 0; i < xS.Length; i++)
			{
				xS[i] = i * wSkill;
				yS[i] = ySkill;
			}

			xHP = xSkill + array.Length * wSkill + 6;
			yHP = ySkill;

			if (!GameCanvas.isTouch)
			{
				return;
			}

			if (gamePad.isSmallGamePad && isAnalog == 1)
			{
				xHP = xSkill + array.Length * wSkill + 6;
				yHP = ySkill;
			}
			else
			{
				xHP = GameCanvas.w - 45;
				yHP = GameCanvas.h - 45;
			}
			setTouchBtn();
		}
	private bool checkSkillValid()
		{
			if (Char.myCharz().myskill != null && ((Char.myCharz().myskill.template.manaUseType != 1 && Char.myCharz().cMP < Char.myCharz().myskill.manaUse) || (Char.myCharz().myskill.template.manaUseType == 1 && Char.myCharz().cMP < Char.myCharz().cMPFull * Char.myCharz().myskill.manaUse / 100)))
			{
				info1.addInfo(mResources.NOT_ENOUGH_MP, 0);
				auto = 0;
				return false;
			}
			if (Char.myCharz().myskill == null || (Char.myCharz().myskill.template.maxPoint > 0 && Char.myCharz().myskill.point == 0))
			{
				GameCanvas.startOKDlg(mResources.SKILL_FAIL);
				return false;
			}
			return true;
		}
	private bool checkSkillValid2()
		{
			if (Char.myCharz().myskill != null && ((Char.myCharz().myskill.template.manaUseType != 1 && Char.myCharz().cMP < Char.myCharz().myskill.manaUse) || (Char.myCharz().myskill.template.manaUseType == 1 && Char.myCharz().cMP < Char.myCharz().cMPFull * Char.myCharz().myskill.manaUse / 100)))
			{
				return false;
			}
			if (Char.myCharz().myskill == null || (Char.myCharz().myskill.template.maxPoint > 0 && Char.myCharz().myskill.point == 0))
			{
				return false;
			}
			return true;
		}
	public void activeRongThanEff(bool isMe)
		{
			activeRongThan = true;
			isUseFreez = true;
			isMeCallRongThan = true;
			if (isMe)
			{
				Effect me = new Effect(20, Char.myCharz().cx, Char.myCharz().cy - 77, 2, 8, 1);
				EffecMn.addEff(me);
			}
		}
	public void hideRongThanEff()
		{
			activeRongThan = false;
			isUseFreez = true;
			isMeCallRongThan = false;
		}
	public void doFire(bool isFireByShortCut, bool skipWaypoint)
		{
			tam++;
			Waypoint waypoint = Char.myCharz().isInEnterOfflinePoint();
			Waypoint waypoint2 = Char.myCharz().isInEnterOnlinePoint();
			if (!skipWaypoint && waypoint != null && (Char.myCharz().mobFocus == null || (Char.myCharz().mobFocus != null && Char.myCharz().mobFocus.templateId == 0)))
			{
				waypoint.popup.command.performAction();
			}
			else if (!skipWaypoint && waypoint2 != null && (Char.myCharz().mobFocus == null || (Char.myCharz().mobFocus != null && Char.myCharz().mobFocus.templateId == 0)))
			{
				waypoint2.popup.command.performAction();
			}
			else
			{
				if ((TileMap.mapID == 51 && Char.myCharz().npcFocus != null) || Char.myCharz().statusMe == 14)
				{
					return;
				}
				Char.myCharz().cvx = (Char.myCharz().cvy = 0);
				if (Char.myCharz().isSelectingSkillUseAlone() && Char.myCharz().focusToAttack())
				{
					if (checkSkillValid())
					{
						Char.myCharz().currentFireByShortcut = isFireByShortCut;
						Char.myCharz().useSkillNotFocus();
					}
				}
				else if (isAttack())
				{
					if (Char.myCharz().isUseChargeSkill() && Char.myCharz().focusToAttack())
					{
						if (checkSkillValid())
						{
							Char.myCharz().currentFireByShortcut = isFireByShortCut;
							Char.myCharz().sendUseChargeSkill();
						}
						else
						{
							Char.myCharz().stopUseChargeSkill();
						}
					}
					else
					{
						bool flag = TileMap.tileTypeAt(Char.myCharz().cx, Char.myCharz().cy, 2);
						Char.myCharz().setSkillPaint(sks[Char.myCharz().myskill.skillId], (!flag) ? 1 : 0);
						if (flag)
						{
							Char.myCharz().delayFall = 20;
						}
						Char.myCharz().currentFireByShortcut = isFireByShortCut;
					}
				}
				if (Char.myCharz().isSelectingSkillBuffToPlayer())
				{
					auto = 0;
				}
			}
		}
	public void doSelectSkill(Skill skill, bool isShortcut)
		{
			if (Char.myCharz().isCreateDark || isCharging() || Char.myCharz().taskMaint.taskId <= 1)
			{
				return;
			}
			Char.myCharz().myskill = skill;
			if (lastSkill != skill && lastSkill != null)
			{
				Service.gI().selectSkill(skill.template.id);
				saveRMSCurrentSkill(skill.template.id);
				resetButton();
				lastSkill = skill;
				selectedIndexSkill = -1;
				gI().auto = 0;
				return;
			}
			if (Char.myCharz().isUseSkillSpec())
			{
				Res.outz(">>>use skill spec: " + skill.template.id);
				Char.myCharz().sendNewAttack(skill.template.id);
				saveRMSCurrentSkill(skill.template.id);
				resetButton();
				lastSkill = skill;
				selectedIndexSkill = -1;
				gI().auto = 0;
				return;
			}
			if (Char.myCharz().isSelectingSkillUseAlone())
			{
				Res.outz("use skill not focus");
				doUseSkillNotFocus(skill);
				lastSkill = skill;
				return;
			}
			selectedIndexSkill = -1;
			if (skill == null)
			{
				return;
			}
			Res.outz("only select skill");
			if (lastSkill != skill)
			{
				Service.gI().selectSkill(skill.template.id);
				saveRMSCurrentSkill(skill.template.id);
				resetButton();
			}
			if (Char.myCharz().charFocus != null || !Char.myCharz().isSelectingSkillBuffToPlayer())
			{
				if (Char.myCharz().focusToAttack())
				{
					doFire(isShortcut, skipWaypoint: true);
					doSeleckSkillFlag = true;
				}
				lastSkill = skill;
			}
		}
	public void doUseSkill(Skill skill, bool isShortcut)
		{
			if ((TileMap.mapID == 112 || TileMap.mapID == 113) && Char.myCharz().cTypePk == 0)
			{
				return;
			}
			if (Char.myCharz().isSelectingSkillUseAlone())
			{
				Res.outz("HERE");
				doUseSkillNotFocus(skill);
				return;
			}
			selectedIndexSkill = -1;
			if (skill != null)
			{
				Service.gI().selectSkill(skill.template.id);
				saveRMSCurrentSkill(skill.template.id);
				resetButton();
				Char.myCharz().myskill = skill;
				doFire(isShortcut, skipWaypoint: true);
			}
		}
	public void doUseSkillNotFocus(Skill skill)
		{
			if (((TileMap.mapID != 112 && TileMap.mapID != 113) || Char.myCharz().cTypePk != 0) && checkSkillValid())
			{
				selectedIndexSkill = -1;
				if (skill != null)
				{
					Service.gI().selectSkill(skill.template.id);
					saveRMSCurrentSkill(skill.template.id);
					resetButton();
					Char.myCharz().myskill = skill;
					Char.myCharz().useSkillNotFocus();
					Char.myCharz().currentFireByShortcut = true;
					auto = 0;
				}
			}
		}
	public void sortSkill()
		{
			for (int i = 0; i < Char.myCharz().vSkillFight.size() - 1; i++)
			{
				Skill skill = (Skill)Char.myCharz().vSkillFight.elementAt(i);
				for (int j = i + 1; j < Char.myCharz().vSkillFight.size(); j++)
				{
					Skill skill2 = (Skill)Char.myCharz().vSkillFight.elementAt(j);
					if (skill2.template.id < skill.template.id)
					{
						Skill skill3 = skill2;
						skill2 = skill;
						skill = skill3;
						Char.myCharz().vSkillFight.setElementAt(skill, i);
						Char.myCharz().vSkillFight.setElementAt(skill2, j);
					}
				}
			}
		}
	public void setCharJumpAtt()
		{
			Char.myCharz().cvy = -10;
			Char.myCharz().statusMe = 3;
			Char.myCharz().cp1 = 0;
		}
	public void setCharJump(int cvx)
		{
			if (Char.myCharz().cx - Char.myCharz().cxSend != 0 || Char.myCharz().cy - Char.myCharz().cySend != 0)
			{
				Service.gI().charMove();
			}
			Char.myCharz().cvy = -10;
			Char.myCharz().cvx = cvx;
			Char.myCharz().statusMe = 3;
			Char.myCharz().cp1 = 0;
		}
	private void checkEffToObj(IMapObject obj, bool isnew)
		{
			if (obj == null || tDoubleDelay > 0)
			{
				return;
			}
			tDoubleDelay = 10;
			int x = obj.getX();
			int num = 1;
			int num2 = Res.abs(Char.myCharz().cx - x);
			num = ((num2 <= 80) ? 1 : ((num2 > 80 && num2 <= 200) ? 2 : ((num2 <= 200 || num2 > 400) ? 4 : 3)));
			if (!isnew)
			{
				if (obj.Equals(Char.myCharz().mobFocus) || (obj.Equals(Char.myCharz().charFocus) && Char.myCharz().isMeCanAttackOtherPlayer(Char.myCharz().charFocus)))
				{
					ServerEffect.addServerEffect(135, obj.getX(), obj.getY(), num);
				}
				else if (obj.Equals(Char.myCharz().npcFocus) || obj.Equals(Char.myCharz().itemFocus) || obj.Equals(Char.myCharz().charFocus))
				{
					ServerEffect.addServerEffect(136, obj.getX(), obj.getY(), num);
				}
			}
			else
			{
				ServerEffect.addServerEffect(136, obj.getX(), obj.getY(), num);
			}
		}
	public static void startFlyText(string flyString, int x, int y, int dx, int dy, int color)
		{
			int num = -1;
			for (int i = 0; i < 5; i++)
			{
				if (flyTextState[i] == -1)
				{
					num = i;
					break;
				}
			}
			if (num == -1)
			{
				return;
			}
			flyTextColor[num] = color;
			flyTextString[num] = flyString;
			flyTextX[num] = x;
			flyTextY[num] = y;
			flyTextDx[num] = dx;
			flyTextDy[num] = ((dy >= 0) ? 5 : (-5));
			flyTextState[num] = 0;
			flyTime[num] = 0;
			flyTextYTo[num] = 10;
			for (int j = 0; j < 5; j++)
			{
				if (flyTextState[j] != -1 && num != j && flyTextDy[num] < 0 && Res.abs(flyTextX[num] - flyTextX[j]) <= 20 && flyTextYTo[num] == flyTextYTo[j])
				{
					flyTextYTo[num] += 10;
				}
			}
		}

}
