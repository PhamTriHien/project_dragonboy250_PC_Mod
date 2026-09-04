using System;
using Assets.src.e;
using Assets.src.f;
using Assets.src.g;
using UnityEngine;

public partial class Controller : IMessageHandler
{
	private void createSkill(myReader d)
		{
			GameScr.vcSkill = d.readByte();
			GameScr.gI().sOptionTemplates = new SkillOptionTemplate[d.readByte()];
			for (int i = 0; i < GameScr.gI().sOptionTemplates.Length; i++)
			{
				GameScr.gI().sOptionTemplates[i] = new SkillOptionTemplate();
				GameScr.gI().sOptionTemplates[i].id = i;
				GameScr.gI().sOptionTemplates[i].name = d.readUTF();
			}
			GameScr.nClasss = new NClass[d.readByte()];
			for (int j = 0; j < GameScr.nClasss.Length; j++)
			{
				GameScr.nClasss[j] = new NClass();
				GameScr.nClasss[j].classId = j;
				GameScr.nClasss[j].name = d.readUTF();
				GameScr.nClasss[j].skillTemplates = new SkillTemplate[d.readByte()];
				for (int k = 0; k < GameScr.nClasss[j].skillTemplates.Length; k++)
				{
					GameScr.nClasss[j].skillTemplates[k] = new SkillTemplate();
					GameScr.nClasss[j].skillTemplates[k].id = d.readByte();
					GameScr.nClasss[j].skillTemplates[k].name = d.readUTF();
					GameScr.nClasss[j].skillTemplates[k].maxPoint = d.readByte();
					GameScr.nClasss[j].skillTemplates[k].manaUseType = d.readByte();
					GameScr.nClasss[j].skillTemplates[k].type = d.readByte();
					GameScr.nClasss[j].skillTemplates[k].iconId = d.readShort();
					GameScr.nClasss[j].skillTemplates[k].damInfo = d.readUTF();
					int lineWidth = 130;
					if (GameCanvas.w == 128 || GameCanvas.h <= 208)
					{
						lineWidth = 100;
					}
					GameScr.nClasss[j].skillTemplates[k].description = mFont.tahoma_7_green2.splitFontArray(d.readUTF(), lineWidth);
					GameScr.nClasss[j].skillTemplates[k].skills = new Skill[d.readByte()];
					for (int l = 0; l < GameScr.nClasss[j].skillTemplates[k].skills.Length; l++)
					{
						GameScr.nClasss[j].skillTemplates[k].skills[l] = new Skill();
						GameScr.nClasss[j].skillTemplates[k].skills[l].skillId = d.readShort();
						GameScr.nClasss[j].skillTemplates[k].skills[l].template = GameScr.nClasss[j].skillTemplates[k];
						GameScr.nClasss[j].skillTemplates[k].skills[l].point = d.readByte();
						GameScr.nClasss[j].skillTemplates[k].skills[l].powRequire = d.readLong();
						GameScr.nClasss[j].skillTemplates[k].skills[l].manaUse = d.readShort();
						GameScr.nClasss[j].skillTemplates[k].skills[l].coolDown = d.readInt();
						GameScr.nClasss[j].skillTemplates[k].skills[l].dx = d.readShort();
						GameScr.nClasss[j].skillTemplates[k].skills[l].dy = d.readShort();
						GameScr.nClasss[j].skillTemplates[k].skills[l].maxFight = d.readByte();
						GameScr.nClasss[j].skillTemplates[k].skills[l].damage = d.readShort();
						GameScr.nClasss[j].skillTemplates[k].skills[l].price = d.readShort();
						GameScr.nClasss[j].skillTemplates[k].skills[l].moreInfo = d.readUTF();
						Skills.add(GameScr.nClasss[j].skillTemplates[k].skills[l]);
					}
				}
			}
		}

	private void createData(myReader d, bool isSaveRMS)
		{
			GameScr.vcData = d.readByte();
			if (isSaveRMS)
			{
				Rms.saveRMS("NR_dart", NinjaUtil.readByteArray(d));
				Rms.saveRMS("NR_arrow", NinjaUtil.readByteArray(d));
				Rms.saveRMS("NR_effect", NinjaUtil.readByteArray(d));
				Rms.saveRMS("NR_image", NinjaUtil.readByteArray(d));
				Rms.saveRMS("NR_part", NinjaUtil.readByteArray(d));
				Rms.saveRMS("NR_skill", NinjaUtil.readByteArray(d));
				Rms.DeleteStorage("NRdata");
			}
		}

	private Image createImage(sbyte[] arr)
		{
			try
			{
				return Image.createImage(arr, 0, arr.Length);
			}
			catch (Exception)
			{
			}
			return null;
		}

	public int[] arrayByte2Int(sbyte[] b)
		{
			int[] array = new int[b.Length];
			for (int i = 0; i < b.Length; i++)
			{
				int num = b[i];
				if (num < 0)
				{
					num += 256;
				}
				array[i] = num;
			}
			return array;
		}

	private void useSkill(Skill skill)
		{
			if (Char.myCharz().myskill == null)
			{
				Char.myCharz().myskill = skill;
			}
			else if (skill.template.Equals(Char.myCharz().myskill.template))
			{
				Char.myCharz().myskill = skill;
			}
			Char.myCharz().vSkill.addElement(skill);
			if ((skill.template.type == 1 || skill.template.type == 4 || skill.template.type == 2 || skill.template.type == 3) && (skill.template.maxPoint == 0 || (skill.template.maxPoint > 0 && skill.point > 0)))
			{
				if (skill.template.id == Char.myCharz().skillTemplateId)
				{
					Service.gI().selectSkill(Char.myCharz().skillTemplateId);
				}
				Char.myCharz().vSkillFight.addElement(skill);
			}
		}

	private void createItemNew(myReader d)
		{
			try
			{
				loadItemNew(d, -1, isSave: true);
			}
			catch (Exception)
			{
			}
		}

	private void loadItemNew(myReader d, sbyte type, bool isSave)
		{
			try
			{
				d.mark(1000000);
				GameScr.vcItem = d.readByte();
				type = d.readByte();
				Res.err(GameScr.vcItem + ":<<GameScr.vcItem >>>>>>loadItemNew: " + type + "  isSave:" + isSave);
				if (type == 0)
				{
					GameScr.gI().iOptionTemplates = new ItemOptionTemplate[d.readShort()];
					for (int i = 0; i < GameScr.gI().iOptionTemplates.Length; i++)
					{
						GameScr.gI().iOptionTemplates[i] = new ItemOptionTemplate();
						GameScr.gI().iOptionTemplates[i].id = i;
						GameScr.gI().iOptionTemplates[i].name = Res.GetVietnameseOptionTemplate(i, d.readUTF());
						GameScr.gI().iOptionTemplates[i].type = d.readByte();
					}
					try
					{
						short num = d.readShort();
						for (int j = 0; j < num; j++)
						{
							short num2 = d.readShort();
							GameScr.gI().iOptionTemplates[num2].color = d.readUnsignedByte();
						}
					}
					catch (Exception)
					{
					}
					if (isSave)
					{
						d.reset();
						sbyte[] data = new sbyte[d.available()];
						d.readFully(ref data);
						Rms.saveRMS("NRitem0", data);
					}
				}
				else if (type == 1)
				{
					ItemTemplates.itemTemplates.clear();
					int num3 = d.readShort();
					for (int k = 0; k < num3; k++)
					{
						ItemTemplate it = new ItemTemplate((short)k, d.readByte(), d.readByte(), d.readUTF(), d.readUTF(), d.readByte(), d.readInt(), d.readShort(), d.readShort(), d.readBoolean());
						ItemTemplates.add(it);
					}
					if (isSave)
					{
						d.reset();
						sbyte[] data2 = new sbyte[d.available()];
						d.readFully(ref data2);
						Rms.saveRMS("NRitem1", data2);
						sbyte[] data3 = new sbyte[1] { GameScr.vcItem };
						Rms.saveRMS("NRitemVersion", data3);
					}
					LoginScr.isUpdateItem = false;
					GameScr.gI().readOk();
				}
				else
				{
					if (type == 2)
					{
						return;
					}
					if (type == 100)
					{
						Char.Arr_Head_2Fr = readArrHead(d);
						if (isSave)
						{
							d.reset();
							sbyte[] data4 = new sbyte[d.available()];
							d.readFully(ref data4);
							Rms.saveRMS("NRitem100", data4);
						}
					}
					else
					{
						if (type != 101)
						{
							return;
						}
						try
						{
							int num4 = d.readShort();
							Char.Arr_Head_FlyMove = new short[num4];
							for (int l = 0; l < num4; l++)
							{
								short num5 = d.readShort();
								Char.Arr_Head_FlyMove[l] = num5;
							}
							if (isSave)
							{
								d.reset();
								sbyte[] data5 = new sbyte[d.available()];
								d.readFully(ref data5);
								Rms.saveRMS("NRitem101", data5);
							}
							return;
						}
						catch (Exception)
						{
							Char.Arr_Head_FlyMove = new short[0];
							return;
						}
					}
				}
			}
			catch (Exception ex3)
			{
				ex3.ToString();
			}
		}

	private int[][] readArrHead(myReader d)
		{
			int[][] array = new int[1][] { new int[2] { 542, 543 } };
			try
			{
				int num = d.readShort();
				array = new int[num][];
				for (int i = 0; i < array.Length; i++)
				{
					int num2 = d.readByte();
					array[i] = new int[num2];
					for (int j = 0; j < num2; j++)
					{
						array[i][j] = d.readShort();
					}
				}
			}
			catch (Exception)
			{
			}
			return array;
		}

	public void read_UpdateSkill(Message msg)
		{
			try
			{
				short num = msg.reader().readShort();
				sbyte b = -1;
				try
				{
					b = msg.reader().readSByte();
				}
				catch (Exception)
				{
				}
				if (b == 0)
				{
					short curExp = msg.reader().readShort();
					for (int i = 0; i < Char.myCharz().vSkill.size(); i++)
					{
						Skill skill = (Skill)Char.myCharz().vSkill.elementAt(i);
						if (skill.skillId == num)
						{
							skill.curExp = curExp;
							break;
						}
					}
				}
				else if (b == 1)
				{
					sbyte b2 = msg.reader().readByte();
					for (int j = 0; j < Char.myCharz().vSkill.size(); j++)
					{
						Skill skill2 = (Skill)Char.myCharz().vSkill.elementAt(j);
						if (skill2.skillId == num)
						{
							for (int k = 0; k < 20; k++)
							{
								string nameImg = "Skills_" + skill2.template.id + "_" + b2 + "_" + k;
								MainImage imagePath = ImgByName.getImagePath(nameImg, ImgByName.hashImagePath);
							}
							break;
						}
					}
				}
				else
				{
					if (b != -1)
					{
						return;
					}
					Skill skill3 = Skills.get(num);
					for (int l = 0; l < Char.myCharz().vSkill.size(); l++)
					{
						Skill skill4 = (Skill)Char.myCharz().vSkill.elementAt(l);
						if (skill4.template.id == skill3.template.id)
						{
							Char.myCharz().vSkill.setElementAt(skill3, l);
							break;
						}
					}
					for (int m = 0; m < Char.myCharz().vSkillFight.size(); m++)
					{
						Skill skill5 = (Skill)Char.myCharz().vSkillFight.elementAt(m);
						if (skill5.template.id == skill3.template.id)
						{
							Char.myCharz().vSkillFight.setElementAt(skill3, m);
							break;
						}
					}
					for (int n = 0; n < GameScr.onScreenSkill.Length; n++)
					{
						if (GameScr.onScreenSkill[n] != null && GameScr.onScreenSkill[n].template.id == skill3.template.id)
						{
							GameScr.onScreenSkill[n] = skill3;
							break;
						}
					}
					for (int num2 = 0; num2 < GameScr.keySkill.Length; num2++)
					{
						if (GameScr.keySkill[num2] != null && GameScr.keySkill[num2].template.id == skill3.template.id)
						{
							GameScr.keySkill[num2] = skill3;
							break;
						}
					}
					if (Char.myCharz().myskill.template.id == skill3.template.id)
					{
						Char.myCharz().myskill = skill3;
					}
					GameScr.info1.addInfo(mResources.hasJustUpgrade1 + skill3.template.name + mResources.hasJustUpgrade2 + skill3.point, 0);
				}
			}
			catch (Exception)
			{
			}
		}

	public ItemOption readItemOption(Message msg)
		{
			ItemOption result = null;
			try
			{
				int num = msg.reader().readShort();
				int param = msg.reader().readInt();
				if (num != -1)
				{
					result = new ItemOption(num, param);
				}
			}
			catch (Exception)
			{
				Res.err(">>>>read.ItemOption  errr:");
			}
			return result;
		}

}
