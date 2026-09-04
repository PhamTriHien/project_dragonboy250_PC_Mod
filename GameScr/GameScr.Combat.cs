using System;
using Assets.src.g;

public partial class GameScr : mScreen, IChatable
{
	public void loadSkillShortcut()
		{
		}

	public void onOSkill(sbyte[] oSkillID)
		{
			Cout.println("GET onScreenSkill!");
			onScreenSkill = new Skill[10];
			if (oSkillID == null)
			{
				loadDefaultonScreenSkill();
				return;
			}
			for (int i = 0; i < oSkillID.Length; i++)
			{
				for (int j = 0; j < Char.myCharz().vSkillFight.size(); j++)
				{
					Skill skill = (Skill)Char.myCharz().vSkillFight.elementAt(j);
					if (skill.template.id == oSkillID[i])
					{
						onScreenSkill[i] = skill;
						break;
					}
				}
			}
		}

	public void onKSkill(sbyte[] kSkillID)
		{
			Cout.println("GET KEYSKILL!");
			keySkill = new Skill[10];
			if (kSkillID == null)
			{
				loadDefaultKeySkill();
				return;
			}
			for (int i = 0; i < kSkillID.Length; i++)
			{
				for (int j = 0; j < Char.myCharz().vSkillFight.size(); j++)
				{
					Skill skill = (Skill)Char.myCharz().vSkillFight.elementAt(j);
					if (skill.template.id == kSkillID[i])
					{
						keySkill[i] = skill;
						break;
					}
				}
			}
		}

	public void onCSkill(sbyte[] cSkillID)
		{
			Cout.println("GET CURRENTSKILL!");
			if (cSkillID == null || cSkillID.Length == 0)
			{
				if (Char.myCharz().vSkillFight.size() > 0)
				{
					Char.myCharz().myskill = (Skill)Char.myCharz().vSkillFight.elementAt(0);
				}
			}
			else
			{
				for (int i = 0; i < Char.myCharz().vSkillFight.size(); i++)
				{
					Skill skill = (Skill)Char.myCharz().vSkillFight.elementAt(i);
					if (skill.template.id == cSkillID[0])
					{
						Char.myCharz().myskill = skill;
						break;
					}
				}
			}
			if (Char.myCharz().myskill != null)
			{
				Service.gI().selectSkill(Char.myCharz().myskill.template.id);
				saveRMSCurrentSkill(Char.myCharz().myskill.template.id);
			}
		}

	private void loadDefaultonScreenSkill()
		{
			Cout.println("LOAD DEFAULT ONmScreen SKILL");
			for (int i = 0; i < onScreenSkill.Length && i < Char.myCharz().vSkillFight.size(); i++)
			{
				Skill skill = (Skill)Char.myCharz().vSkillFight.elementAt(i);
				onScreenSkill[i] = skill;
			}
			saveonScreenSkillToRMS();
		}

	private void loadDefaultKeySkill()
		{
			Cout.println("LOAD DEFAULT KEY SKILL");
			for (int i = 0; i < keySkill.Length && i < Char.myCharz().vSkillFight.size(); i++)
			{
				Skill skill = (Skill)Char.myCharz().vSkillFight.elementAt(i);
				keySkill[i] = skill;
			}
			saveKeySkillToRMS();
		}

	public void doSetOnScreenSkill(SkillTemplate skillTemplate)
		{
			Skill skill = Char.myCharz().getSkill(skillTemplate);
			MyVector myVector = new MyVector();
			for (int i = 0; i < 10; i++)
			{
				Command command = new Command(p: new object[2]
				{
					skill,
					i + string.Empty
				}, caption: mResources.into_place + (i + 1), action: 11120);
				Skill skill2 = onScreenSkill[i];
				if (skill2 != null)
				{
					command.isDisplay = true;
				}
				myVector.addElement(command);
			}
			GameCanvas.menu.startAt(myVector, 0);
		}

	public void doSetKeySkill(SkillTemplate skillTemplate)
		{
			Cout.println("DO SET KEY SKILL");
			Skill skill = Char.myCharz().getSkill(skillTemplate);
			string[] array = ((!TField.isQwerty) ? mResources.key_skill : mResources.key_skill_qwerty);
			MyVector myVector = new MyVector();
			for (int i = 0; i < 10; i++)
			{
				myVector.addElement(new Command(p: new object[2]
				{
					skill,
					i + string.Empty
				}, caption: array[i], action: 11121));
			}
			GameCanvas.menu.startAt(myVector, 0);
		}

	public void saveonScreenSkillToRMS()
		{
			sbyte[] array = new sbyte[onScreenSkill.Length];
			for (int i = 0; i < onScreenSkill.Length; i++)
			{
				if (onScreenSkill[i] == null)
				{
					array[i] = -1;
				}
				else
				{
					array[i] = onScreenSkill[i].template.id;
				}
			}
			Service.gI().changeOnKeyScr(array);
		}

	public void saveKeySkillToRMS()
		{
			sbyte[] array = new sbyte[keySkill.Length];
			for (int i = 0; i < keySkill.Length; i++)
			{
				if (keySkill[i] == null)
				{
					array[i] = -1;
				}
				else
				{
					array[i] = keySkill[i].template.id;
				}
			}
			Service.gI().changeOnKeyScr(array);
		}

	public void saveRMSCurrentSkill(sbyte id)
		{
		}

	public void addSkillShortcut(Skill skill)
		{
			Cout.println("ADD SKILL SHORTCUT TO SKILL " + skill.template.id);
			for (int i = 0; i < onScreenSkill.Length; i++)
			{
				if (onScreenSkill[i] == null)
				{
					onScreenSkill[i] = skill;
					break;
				}
			}
			for (int j = 0; j < keySkill.Length; j++)
			{
				if (keySkill[j] == null)
				{
					keySkill[j] = skill;
					break;
				}
			}
			if (Char.myCharz().myskill == null)
			{
				Char.myCharz().myskill = skill;
			}
			saveKeySkillToRMS();
			saveonScreenSkillToRMS();
		}

	public void readDart()
		{
			DataInputStream dataInputStream = null;
			try
			{
				dataInputStream = new DataInputStream(Rms.loadRMS("NR_dart"));
				int num = dataInputStream.readShort();
				darts = new DartInfo[num];
				for (int i = 0; i < num; i++)
				{
					darts[i] = new DartInfo();
					darts[i].id = dataInputStream.readShort();
					darts[i].nUpdate = dataInputStream.readShort();
					darts[i].va = dataInputStream.readShort() * 256;
					darts[i].xdPercent = dataInputStream.readShort();
					int num2 = dataInputStream.readShort();
					darts[i].tail = new short[num2];
					for (int j = 0; j < num2; j++)
					{
						darts[i].tail[j] = dataInputStream.readShort();
					}
					num2 = dataInputStream.readShort();
					darts[i].tailBorder = new short[num2];
					for (int k = 0; k < num2; k++)
					{
						darts[i].tailBorder[k] = dataInputStream.readShort();
					}
					num2 = dataInputStream.readShort();
					darts[i].xd1 = new short[num2];
					for (int l = 0; l < num2; l++)
					{
						darts[i].xd1[l] = dataInputStream.readShort();
					}
					num2 = dataInputStream.readShort();
					darts[i].xd2 = new short[num2];
					for (int m = 0; m < num2; m++)
					{
						darts[i].xd2[m] = dataInputStream.readShort();
					}
					num2 = dataInputStream.readShort();
					darts[i].head = new short[num2][];
					for (int n = 0; n < num2; n++)
					{
						short num3 = dataInputStream.readShort();
						darts[i].head[n] = new short[num3];
						for (int num4 = 0; num4 < num3; num4++)
						{
							darts[i].head[n][num4] = dataInputStream.readShort();
						}
					}
					num2 = dataInputStream.readShort();
					darts[i].headBorder = new short[num2][];
					for (int num5 = 0; num5 < num2; num5++)
					{
						short num6 = dataInputStream.readShort();
						darts[i].headBorder[num5] = new short[num6];
						for (int num7 = 0; num7 < num6; num7++)
						{
							darts[i].headBorder[num5][num7] = dataInputStream.readShort();
						}
					}
				}
			}
			catch (Exception ex)
			{
				Cout.LogError("Loi ham ReadDart: " + ex.ToString());
			}
			finally
			{
				try
				{
					dataInputStream.close();
				}
				catch (Exception ex2)
				{
					Cout.LogError("Loi ham reaaDart: " + ex2.ToString());
				}
			}
		}

	public void readSkill()
		{
			DataInputStream dataInputStream = null;
			try
			{
				dataInputStream = new DataInputStream(Rms.loadRMS("NR_skill"));
				int num = dataInputStream.readShort();
				int num2 = Skills.skills.size();
				sks = new SkillPaint[num2];
				for (int i = 0; i < num; i++)
				{
					short num3 = dataInputStream.readShort();
					if (num3 == 1111)
					{
						num3 = (short)(num - 1);
					}
					sks[num3] = new SkillPaint();
					sks[num3].id = num3;
					sks[num3].effectHappenOnMob = dataInputStream.readShort();
					if (sks[num3].effectHappenOnMob <= 0)
					{
						sks[num3].effectHappenOnMob = 80;
					}
					sks[num3].numEff = dataInputStream.readByte();
					sks[num3].skillStand = new SkillInfoPaint[dataInputStream.readByte()];
					for (int j = 0; j < sks[num3].skillStand.Length; j++)
					{
						sks[num3].skillStand[j] = new SkillInfoPaint();
						sks[num3].skillStand[j].status = dataInputStream.readByte();
						sks[num3].skillStand[j].effS0Id = dataInputStream.readShort();
						sks[num3].skillStand[j].e0dx = dataInputStream.readShort();
						sks[num3].skillStand[j].e0dy = dataInputStream.readShort();
						sks[num3].skillStand[j].effS1Id = dataInputStream.readShort();
						sks[num3].skillStand[j].e1dx = dataInputStream.readShort();
						sks[num3].skillStand[j].e1dy = dataInputStream.readShort();
						sks[num3].skillStand[j].effS2Id = dataInputStream.readShort();
						sks[num3].skillStand[j].e2dx = dataInputStream.readShort();
						sks[num3].skillStand[j].e2dy = dataInputStream.readShort();
						sks[num3].skillStand[j].arrowId = dataInputStream.readShort();
						sks[num3].skillStand[j].adx = dataInputStream.readShort();
						sks[num3].skillStand[j].ady = dataInputStream.readShort();
					}
					sks[num3].skillfly = new SkillInfoPaint[dataInputStream.readByte()];
					for (int k = 0; k < sks[num3].skillfly.Length; k++)
					{
						sks[num3].skillfly[k] = new SkillInfoPaint();
						sks[num3].skillfly[k].status = dataInputStream.readByte();
						sks[num3].skillfly[k].effS0Id = dataInputStream.readShort();
						sks[num3].skillfly[k].e0dx = dataInputStream.readShort();
						sks[num3].skillfly[k].e0dy = dataInputStream.readShort();
						sks[num3].skillfly[k].effS1Id = dataInputStream.readShort();
						sks[num3].skillfly[k].e1dx = dataInputStream.readShort();
						sks[num3].skillfly[k].e1dy = dataInputStream.readShort();
						sks[num3].skillfly[k].effS2Id = dataInputStream.readShort();
						sks[num3].skillfly[k].e2dx = dataInputStream.readShort();
						sks[num3].skillfly[k].e2dy = dataInputStream.readShort();
						sks[num3].skillfly[k].arrowId = dataInputStream.readShort();
						sks[num3].skillfly[k].adx = dataInputStream.readShort();
						sks[num3].skillfly[k].ady = dataInputStream.readShort();
					}
				}
			}
			catch (Exception ex)
			{
				Cout.LogError("Loi ham readSkill: " + ex.ToString());
			}
			finally
			{
				try
				{
					dataInputStream.close();
				}
				catch (Exception ex2)
				{
					Cout.LogError("Loi ham readskill: " + ex2.ToString());
				}
			}
		}

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

	public static void updateFlyText()
		{
			for (int i = 0; i < 5; i++)
			{
				if (flyTextState[i] == -1)
				{
					continue;
				}
				if (flyTextState[i] > flyTextYTo[i])
				{
					flyTime[i]++;
					if (flyTime[i] == 25)
					{
						flyTime[i] = 0;
						flyTextState[i] = -1;
						flyTextYTo[i] = 0;
						flyTextDx[i] = 0;
						flyTextX[i] = 0;
					}
				}
				else
				{
					flyTextState[i] += Res.abs(flyTextDy[i]);
					flyTextX[i] += flyTextDx[i];
					flyTextY[i] += flyTextDy[i];
				}
			}
		}

	public static void loadSplash()
		{
			if (imgSplash == null)
			{
				imgSplash = new Image[3];
				for (int i = 0; i < 3; i++)
				{
					imgSplash[i] = GameCanvas.loadImage("/e/sp" + i + ".png");
				}
			}
			splashX = new int[2];
			splashY = new int[2];
			splashState = new int[2];
			splashF = new int[2];
			splashDir = new int[2];
			splashState[0] = (splashState[1] = -1);
		}

	public static bool startSplash(int x, int y, int dir)
		{
			int num = ((splashState[0] != -1) ? 1 : 0);
			if (splashState[num] != -1)
			{
				return false;
			}
			splashState[num] = 0;
			splashDir[num] = dir;
			splashX[num] = x;
			splashY[num] = y;
			return true;
		}

	public static void updateSplash()
		{
			for (int i = 0; i < 2; i++)
			{
				if (splashState[i] != -1)
				{
					splashState[i]++;
					splashX[i] += splashDir[i] << 2;
					splashY[i]--;
					if (splashState[i] >= 6)
					{
						splashState[i] = -1;
					}
					else
					{
						splashF[i] = (splashState[i] >> 1) % 3;
					}
				}
			}
		}

	public static void addEffectEnd(int type, int subtype, int typePaint, int x, int y, int levelPaint, int dir, short timeRemove, Point[] listObj)
		{
			Effect_End eff = new Effect_End(type, subtype, typePaint, x, y, levelPaint, dir, timeRemove, listObj);
			addEffect2Vector(eff);
		}

	public static void addEffectEnd_Target(int type, int subtype, int typePaint, Char charUse, Point target, int levelPaint, short timeRemove, short range)
		{
			Effect_End eff = new Effect_End(type, subtype, typePaint, charUse.clone(), target, levelPaint, timeRemove, range);
			addEffect2Vector(eff);
		}

	public static void addEffect2Vector(Effect_End eff)
		{
			if (eff.levelPaint == 0)
			{
				EffectManager.addHiEffect(eff);
			}
			else if (eff.levelPaint == 1)
			{
				EffectManager.addMidEffects(eff);
			}
			else if (eff.levelPaint == 2)
			{
				EffectManager.addMid_2Effects(eff);
			}
			else
			{
				EffectManager.addLowEffect(eff);
			}
		}

}
