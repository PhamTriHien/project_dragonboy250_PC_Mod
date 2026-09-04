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

}
