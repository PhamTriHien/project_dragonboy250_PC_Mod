using System;
using Assets.src.g;
using UnityEngine;

public partial class Panel : IActionListener
{
	public void addItemDetail(Item item)
		{
			try
			{
				cp = new ChatPopup();
				string empty = string.Empty;
				string text = string.Empty;
				if (item.template.gender != Char.myCharz().cgender)
				{
					if (item.template.gender == 0)
					{
						text = text + "\n|7|1|" + mResources.from_earth;
					}
					else if (item.template.gender == 1)
					{
						text = text + "\n|7|1|" + mResources.from_namec;
					}
					else if (item.template.gender == 2)
					{
						text = text + "\n|7|1|" + mResources.from_sayda;
					}
				}
				string text2 = string.Empty;
				if (item.itemOption != null)
				{
					for (int i = 0; i < item.itemOption.Length; i++)
					{
						if (item.itemOption[i].optionTemplate.id == 72)
						{
							text2 = " [+" + item.itemOption[i].param + "]";
						}
					}
				}
				bool flag = false;
				if (item.itemOption != null)
				{
					for (int j = 0; j < item.itemOption.Length; j++)
					{
						if (item.itemOption[j].optionTemplate.id == 41)
						{
							flag = true;
							if (item.itemOption[j].param == 1)
							{
								text = text + "|0|1|" + item.template.name + text2;
							}
							if (item.itemOption[j].param == 2)
							{
								text = text + "|2|1|" + item.template.name + text2;
							}
							if (item.itemOption[j].param == 3)
							{
								text = text + "|8|1|" + item.template.name + text2;
							}
							if (item.itemOption[j].param == 4)
							{
								text = text + "|7|1|" + item.template.name + text2;
							}
						}
					}
				}
				if (!flag)
				{
					text = text + "|0|1|" + item.template.name + text2;
				}
				if (item.itemOption != null)
				{
					for (int k = 0; k < item.itemOption.Length; k++)
					{
						if (item.itemOption[k].optionTemplate.name.StartsWith("$") ? true : false)
						{
							empty = item.itemOption[k].getOptiongColor();
							if (item.itemOption[k].param == 1)
							{
								text = text + "\n|1|1|" + empty;
							}
							if (item.itemOption[k].param == 0)
							{
								text = text + "\n|0|1|" + empty;
							}
						}
						else
						{
							empty = item.itemOption[k].getOptionString();
							if (!empty.Equals(string.Empty))
							{
								if (item.itemOption[k].optionTemplate.id == 72)
								{
									continue;
								}
								if (item.itemOption[k].optionTemplate.id == 102)
								{
									cp.starSlot = (sbyte)item.itemOption[k].param;
								}
								else if (item.itemOption[k].optionTemplate.id == 107)
								{
									cp.maxStarSlot = (sbyte)item.itemOption[k].param;
								}
								else if (item.itemOption[k].optionTemplate.color > 0)
								{
									string text3 = text;
									text = text3 + "\n|" + item.itemOption[k].optionTemplate.color + "|1|" + empty;
								}
								else
								{
									text = text + "\n|1|1|" + empty;
								}
							}
						}
						if (item.itemOption[k].optionTemplate.id != 228)
						{
							continue;
						}
						Res.outz("========>>> " + item.itemOption[k].optionTemplate.name + "_" + item.itemOption[k].param);
						if (item.itemOption[k].param > 7)
						{
							for (int l = 0; l < item.itemOption[k].param - 7; l++)
							{
								cp.starCuongHoa[l + 7] = true;
							}
						}
					}
				}
				if (currItem.template.strRequire > 1)
				{
					string text4 = mResources.pow_request + ": " + currItem.template.strRequire;
					if (currItem.template.strRequire > Char.myCharz().cPower)
					{
						text = text + "\n|3|1|" + text4;
						string text3 = text;
						text = text3 + "\n|3|1|" + mResources.your_pow + ": " + Char.myCharz().cPower;
					}
					else
					{
						text = text + "\n|6|1|" + text4;
					}
				}
				else
				{
					text += "\n|6|1|";
				}
				currItem.compare = getCompare(currItem);
				text += "\n--";
				text = text + "\n|6|" + item.template.description;
				if (!item.reason.Equals(string.Empty))
				{
					if (!item.template.description.Equals(string.Empty))
					{
						text += "\n--";
					}
					text = text + "\n|2|" + item.reason;
				}
				if (cp.maxStarSlot > 0)
				{
					text += "\n\n";
				}
				popUpDetailInit(cp, text);
				idIcon = item.template.iconID;
				partID = null;
				charInfo = null;
			}
			catch (Exception ex)
			{
				Res.outz("ex " + ex.StackTrace);
			}
		}

	public void popUpDetailInit(ChatPopup cp, string chat)
		{
			cp.isClip = false;
			cp.sayWidth = 180;
			cp.cx = 3 + X - ((X != 0) ? (Res.abs(cp.sayWidth - W) + 8) : 0);
			cp.says = mFont.tahoma_7_red.splitFontArray(chat, cp.sayWidth - 10);
			cp.delay = 10000000;
			cp.c = null;
			cp.sayRun = 7;
			cp.ch = 15 - cp.sayRun + cp.says.Length * 12 + 10;
			if (cp.ch > GameCanvas.h - 80)
			{
				cp.ch = GameCanvas.h - 80;
				cp.lim = cp.says.Length * 12 - cp.ch + 17;
				if (cp.lim < 0)
				{
					cp.lim = 0;
				}
				ChatPopup.cmyText = 0;
				cp.isClip = true;
			}
			cp.cy = GameCanvas.menu.menuY - cp.ch;
			while (cp.cy < 10)
			{
				cp.cy++;
				GameCanvas.menu.menuY++;
			}
			cp.mH = 0;
			cp.strY = 10;
		}

	public void popUpDetailInitArray(ChatPopup cp, string[] chat)
		{
			cp.sayWidth = 160;
			cp.cx = 3 + X;
			cp.says = chat;
			cp.delay = 10000000;
			cp.c = null;
			cp.sayRun = 7;
			cp.ch = 15 - cp.sayRun + cp.says.Length * 12 + 10;
			cp.cy = GameCanvas.menu.menuY - cp.ch;
			cp.mH = 0;
			cp.strY = 10;
		}

	public void addMessageDetail(ClanMessage cm)
		{
			cp = new ChatPopup();
			string text = "|0|" + cm.playerName;
			text = text + "\n|1|" + Member.getRole(cm.role);
			for (int i = 0; i < myMember.size(); i++)
			{
				Member member = (Member)myMember.elementAt(i);
				if (cm.playerId == member.ID)
				{
					string text2 = text;
					text = text2 + "\n|5|" + mResources.clan_capsuledonate + ": " + member.clanPoint;
					text2 = text;
					text = text2 + "\n|5|" + mResources.clan_capsuleself + ": " + member.curClanPoint;
					text2 = text;
					text = text2 + "\n|4|" + mResources.give_pea + ": " + member.donate + mResources.time;
					text2 = text;
					text = text2 + "\n|4|" + mResources.receive_pea + ": " + member.receive_donate + mResources.time;
					partID = new int[3] { member.head, member.leg, member.body };
					break;
				}
			}
			text += "\n--";
			for (int j = 0; j < cm.chat.Length; j++)
			{
				text = text + "\n" + cm.chat[j];
			}
			if (cm.type == 1)
			{
				string text2 = text;
				text = text2 + "\n|6|" + mResources.received + " " + cm.recieve + "/" + cm.maxCap;
			}
			popUpDetailInit(cp, text);
			charInfo = null;
		}

	public void addThachDauDetail(TopInfo t)
		{
			string text = "|0|1|" + t.name;
			text = text + "\n|1|Top " + t.rank;
			text = text + "\n|1|" + t.info;
			text = text + "\n|2|" + t.info2;
			cp = new ChatPopup();
			popUpDetailInit(cp, text);
			partID = new int[3] { t.headID, t.leg, t.body };
			currItem = null;
			charInfo = null;
		}

	public void addSkillDetail(SkillTemplate tp, Skill skill, Skill nextSkill)
		{
			string text = "|0|" + tp.name;
			for (int i = 0; i < tp.description.Length; i++)
			{
				text = text + "\n|4|" + tp.description[i];
			}
			text += "\n--";
			if (skill != null)
			{
				string text2 = text;
				text = text2 + "\n|2|" + mResources.cap_do + ": " + skill.point;
				text = text + "\n|5|" + NinjaUtil.replace(tp.damInfo, "#", skill.damage + string.Empty);
				text2 = text;
				text = text2 + "\n|5|" + mResources.KI_consume + skill.manaUse + ((tp.manaUseType != 1) ? string.Empty : "%");
				text2 = text;
				text = text2 + "\n|5|" + mResources.cooldown + ": " + skill.strTimeReplay() + "s";
				text += "\n--";
				if (skill.point == tp.maxPoint)
				{
					text = text + "\n|0|" + mResources.max_level_reach;
				}
				else
				{
					if (!skill.template.isSkillSpec())
					{
						text2 = text;
						text = text2 + "\n|1|" + mResources.next_level_require + Res.formatNumber(nextSkill.powRequire) + " " + mResources.potential;
					}
					text = text + "\n|4|" + NinjaUtil.replace(tp.damInfo, "#", nextSkill.damage + string.Empty);
				}
			}
			else
			{
				text = text + "\n|2|" + mResources.not_learn;
				string text2 = text;
				text = text2 + "\n|1|" + mResources.learn_require + Res.formatNumber(nextSkill.powRequire) + " " + mResources.potential;
				text = text + "\n|4|" + NinjaUtil.replace(tp.damInfo, "#", nextSkill.damage + string.Empty);
				text2 = text;
				text = text2 + "\n|4|" + mResources.KI_consume + nextSkill.manaUse + ((tp.manaUseType != 1) ? string.Empty : "%");
				text2 = text;
				text = text2 + "\n|4|" + mResources.cooldown + ": " + nextSkill.strTimeReplay() + "s";
			}
			currItem = null;
			partID = null;
			charInfo = null;
			cp = new ChatPopup();
			popUpDetailInit(cp, text);
			idIcon = 0;
		}


	public static int GetColor_ItemBg(int id)
		{
			return id switch
			{
				4 => 1269146, 
				1 => 2786816, 
				5 => 13279744, 
				3 => 12537346, 
				2 => 7078041, 
				6 => 11599872, 
				_ => -1, 
			};
		}

	public static mFont GetFont(int color)
		{
			mFont result = mFont.tahoma_7;
			switch (color)
			{
			case -1:
				result = mFont.tahoma_7;
				break;
			case 0:
				result = mFont.tahoma_7b_dark;
				break;
			case 1:
				result = mFont.tahoma_7b_green;
				break;
			case 2:
				result = mFont.tahoma_7b_blue;
				break;
			case 3:
				result = mFont.tahoma_7_red;
				break;
			case 4:
				result = mFont.tahoma_7_green;
				break;
			case 5:
				result = mFont.tahoma_7_blue;
				break;
			case 7:
				result = mFont.tahoma_7b_red;
				break;
			case 8:
				result = mFont.tahoma_7b_yellow;
				break;
			}
			return result;
		}

	public static mFont setTextColor(int id, int type)
		{
			if (type == 0)
			{
				return id switch
				{
					0 => mFont.bigNumber_While, 
					1 => mFont.bigNumber_green, 
					3 => mFont.bigNumber_orange, 
					4 => mFont.bigNumber_blue, 
					5 => mFont.bigNumber_yellow, 
					6 => mFont.bigNumber_red, 
					_ => mFont.bigNumber_While, 
				};
			}
			return id switch
			{
				0 => mFont.tahoma_7b_white, 
				1 => mFont.tahoma_7b_green, 
				3 => mFont.tahoma_7b_yellowSmall2, 
				4 => mFont.tahoma_7b_blue, 
				5 => mFont.tahoma_7b_yellow, 
				6 => mFont.tahoma_7b_red, 
				7 => mFont.tahoma_7b_dark, 
				_ => mFont.tahoma_7b_white, 
			};
		}


}
