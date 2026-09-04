using System;
using System.Collections.Generic;
using Assets.src.g;
using UnityEngine;
public partial class Panel : IActionListener, IChatable
{
	private void paintSkill(mGraphics g)
			{
				g.setColor(16711680);
				g.setClip(xScroll, yScroll, wScroll, hScroll);
				g.translate(0, -cmy);
				int num = Char.myCharz().nClass.skillTemplates.Length;
				for (int i = 0; i < num + 6; i++)
				{
					int num2 = xScroll + 30;
					int num3 = yScroll + i * ITEM_HEIGHT;
					int num4 = wScroll - 30;
					int h = ITEM_HEIGHT - 1;
					int num5 = xScroll;
					int num6 = yScroll + i * ITEM_HEIGHT;
					int num7 = 34;
					int num8 = ITEM_HEIGHT - 1;
					if (num3 - cmy > yScroll + hScroll || num3 - cmy < yScroll - ITEM_HEIGHT)
					{
						continue;
					}
					g.setColor((i != selected) ? 15196114 : 16383818);
					if (i == 5)
					{
						g.setColor((i != selected) ? 16765060 : 16776068);
					}
					g.fillRect(num2, num3, num4, h);
					g.drawImage(GameScr.imgSkill, num5, num6, 0);
					if (i == 0)
					{
						SmallImage.drawSmallImage(g, 567, num5 + 4, num6 + 4, 0, 0);
						string st = mResources.HP + " " + mResources.root + ": " + NinjaUtil.getMoneys(Char.myCharz().cHPGoc);
						mFont.tahoma_7b_blue.drawString(g, st, num2 + 5, num3 + 3, 0);
						mFont.tahoma_7_green2.drawString(g, NinjaUtil.getMoneys(Char.myCharz().cHPGoc + 1000) + " " + mResources.potential + ": " + mResources.increase + " " + Char.myCharz().hpFrom1000TiemNang, num2 + 5, num3 + 15, 0);
					}
					if (i == 1)
					{
						SmallImage.drawSmallImage(g, 569, num5 + 4, num6 + 4, 0, 0);
						string st2 = mResources.KI + " " + mResources.root + ": " + NinjaUtil.getMoneys(Char.myCharz().cMPGoc);
						mFont.tahoma_7b_blue.drawString(g, st2, num2 + 5, num3 + 3, 0);
						mFont.tahoma_7_green2.drawString(g, NinjaUtil.getMoneys(Char.myCharz().cMPGoc + 1000) + " " + mResources.potential + ": " + mResources.increase + " " + Char.myCharz().mpFrom1000TiemNang, num2 + 5, num3 + 15, 0);
					}
					if (i == 2)
					{
						SmallImage.drawSmallImage(g, 568, num5 + 4, num6 + 4, 0, 0);
						string st3 = mResources.hit_point + " " + mResources.root + ": " + NinjaUtil.getMoneys(Char.myCharz().cDamGoc);
						mFont.tahoma_7b_blue.drawString(g, st3, num2 + 5, num3 + 3, 0);
						mFont.tahoma_7_green2.drawString(g, NinjaUtil.getMoneys(Char.myCharz().cDamGoc * 100) + " " + mResources.potential + ": " + mResources.increase + " " + Char.myCharz().damFrom1000TiemNang, num2 + 5, num3 + 15, 0);
					}
					if (i == 3)
					{
						SmallImage.drawSmallImage(g, 721, num5 + 4, num6 + 4, 0, 0);
						string st4 = mResources.armor + " " + mResources.root + ": " + NinjaUtil.getMoneys(Char.myCharz().cDefGoc);
						mFont.tahoma_7b_blue.drawString(g, st4, num2 + 5, num3 + 3, 0);
						mFont.tahoma_7_green2.drawString(g, NinjaUtil.getMoneys(500000 + Char.myCharz().cDefGoc * 100000) + " " + mResources.potential + ": " + mResources.increase + " " + Char.myCharz().defFrom1000TiemNang, num2 + 5, num3 + 15, 0);
					}
					if (i == 4)
					{
						SmallImage.drawSmallImage(g, 719, num5 + 4, num6 + 4, 0, 0);
						string st5 = mResources.critical + " " + mResources.root + ": " + Char.myCharz().cCriticalGoc + "%";
						long num9 = 50000000L;
						int num10 = Char.myCharz().cCriticalGoc;
						if (num10 > t_tiemnang.Length - 1)
						{
							num10 = t_tiemnang.Length - 1;
						}
						num9 = t_tiemnang[num10];
						mFont.tahoma_7b_blue.drawString(g, st5, num2 + 5, num3 + 3, 0);
						long number = num9;
						mFont.tahoma_7_green2.drawString(g, Res.formatNumber2(number) + " " + mResources.potential + ": " + mResources.increase + " " + Char.myCharz().criticalFrom1000Tiemnang, num2 + 5, num3 + 15, 0);
					}
					if (i == 5)
					{
						if (specialInfo != null)
						{
							SmallImage.drawSmallImage(g, spearcialImage, num5 + 4, num6 + 4, 0, 0);
							string[] array = mFont.tahoma_7.splitFontArray(specialInfo, 120);
							for (int j = 0; j < array.Length; j++)
							{
								mFont.tahoma_7_green2.drawString(g, array[j], num2 + 5, num3 + 3 + j * 12, 0);
							}
						}
						else
						{
							mFont.tahoma_7_green2.drawString(g, string.Empty, num2 + 5, num3 + 9, 0);
						}
					}
					if (i < 6)
					{
						continue;
					}
					int num11 = i - 6;
					SkillTemplate skillTemplate = Char.myCharz().nClass.skillTemplates[num11];
					SmallImage.drawSmallImage(g, skillTemplate.iconId, num5 + 4, num6 + 4, 0, 0);
					Skill skill = Char.myCharz().getSkill(skillTemplate);
					if (skill != null)
					{
						mFont.tahoma_7b_blue.drawString(g, skillTemplate.name, num2 + 5, num3 + 3, 0);
						mFont.tahoma_7_blue.drawString(g, mResources.level + ": " + skill.point, num2 + num4 - 5, num3 + 3, mFont.RIGHT);
						if (skill.point == skillTemplate.maxPoint)
						{
							mFont.tahoma_7_green2.drawString(g, mResources.max_level_reach, num2 + 5, num3 + 15, 0);
						}
						else if (skill.template.isSkillSpec())
						{
							string text = mResources.proficiency + ": ";
							int x = mFont.tahoma_7_green2.getWidthExactOf(text) + num2 + 5;
							int num12 = num3 + 15;
							mFont.tahoma_7_green2.drawString(g, text, num2 + 5, num12, 0);
							mFont.tahoma_7_green2.drawString(g, "(" + skill.strCurExp() + ")", num2 + num4 - 5, num12, mFont.RIGHT);
							num12 += 4;
							g.setColor(7169134);
							g.fillRect(x, num12, 50, 5);
							int num13 = skill.curExp * 50 / 1000;
							g.setColor(11992374);
							g.fillRect(x, num12, num13, 5);
							if (skill.curExp < 1000)
							{
							}
						}
						else
						{
							Skill skill2 = skillTemplate.skills[skill.point];
							mFont.tahoma_7_green2.drawString(g, mResources.level + " " + (skill.point + 1) + " " + mResources.need + " " + Res.formatNumber2(skill2.powRequire) + " " + mResources.potential, num2 + 5, num3 + 15, 0);
						}
					}
					else
					{
						Skill skill3 = skillTemplate.skills[0];
						string st6 = mResources.need_upper + " " + Res.formatNumber2(skill3.powRequire) + " " + mResources.potential_to_learn;
						if (skill3.template.id == 24 || skill3.template.id == 25 || skill3.template.id == 26)
						{
							st6 = mResources.need_upper + " " + Res.formatNumber2(skill3.powRequire) + " " + mResources.potential_to_learn_tuyetKi;
						}
						mFont.tahoma_7b_green.drawString(g, skillTemplate.name, num2 + 5, num3 + 3, 0);
						mFont.tahoma_7_green2.drawString(g, st6, num2 + 5, num3 + 15, 0);
					}
				}
				paintScrollArrow(g);
			}
	private void paintSpeacialSkill(mGraphics g)
			{
				g.setClip(xScroll, yScroll, wScroll, hScroll);
				g.translate(0, -cmy);
				g.setColor(0);
				if (currentListLength == 0)
				{
					return;
				}
				int num = (cmy + hScroll) / 24 + 1;
				if (num < hScroll / 24 + 1)
				{
					num = hScroll / 24 + 1;
				}
				if (num > currentListLength)
				{
					num = currentListLength;
				}
				int num2 = cmy / 24;
				if (num2 >= num)
				{
					num2 = num - 1;
				}
				if (num2 < 0)
				{
					num2 = 0;
				}
				for (int i = num2; i < num; i++)
				{
					int num3 = xScroll;
					int num4 = yScroll + i * ITEM_HEIGHT;
					int num5 = 24;
					int num6 = ITEM_HEIGHT - 1;
					int num7 = xScroll + num5;
					int num8 = yScroll + i * ITEM_HEIGHT;
					int num9 = wScroll - num5;
					int h = ITEM_HEIGHT - 1;
					g.setColor((i != selected) ? 15196114 : 16383818);
					g.fillRect(num7, num8, num9, h);
					g.setColor((i != selected) ? 9993045 : 9541120);
					g.fillRect(num3, num4, num5, num6);
					SmallImage.drawSmallImage(g, Char.myCharz().imgSpeacialSkill[currentTabIndex][i], num3 + num5 / 2, num4 + num6 / 2, 0, 3);
					string[] array = mFont.tahoma_7_grey.splitFontArray(Char.myCharz().infoSpeacialSkill[currentTabIndex][i], 140);
					for (int j = 0; j < array.Length; j++)
					{
						mFont.tahoma_7_grey.drawString(g, array[j], num7 + 5, num8 + 1 + j * 11, 0);
					}
				}
				paintScrollArrow(g);
			}
	private void paintLogChat(mGraphics g)
			{
				g.setClip(xScroll, yScroll, wScroll, hScroll);
				g.translate(0, -cmy);
				g.setColor(0);
				if (logChat.size() == 0)
				{
					mFont.tahoma_7_green2.drawString(g, mResources.no_msg, xScroll + wScroll / 2, yScroll + hScroll / 2 - mFont.tahoma_7.getHeight() / 2 + 24, 2);
				}
				for (int i = 0; i < currentListLength; i++)
				{
					int num = xScroll;
					int num2 = yScroll + i * ITEM_HEIGHT;
					int num3 = 24;
					int h = ITEM_HEIGHT - 1;
					int num4 = xScroll + num3;
					int num5 = yScroll + i * ITEM_HEIGHT;
					int num6 = wScroll - num3;
					int num7 = ITEM_HEIGHT - 1;
					if (i == 0)
					{
						g.setColor(15196114);
						g.fillRect(num, num5, wScroll, num7);
						g.drawImage((i != selected) ? GameScr.imgLbtn2 : GameScr.imgLbtnFocus2, xScroll + wScroll - 5, num5 + 2, StaticObj.TOP_RIGHT);
						((i != selected) ? mFont.tahoma_7b_dark : mFont.tahoma_7b_green2).drawString(g, (!isViewChatServer) ? mResources.on : mResources.off, xScroll + wScroll - 22, num5 + 7, 2);
						mFont.tahoma_7_grey.drawString(g, (!isViewChatServer) ? mResources.onPlease : mResources.offPlease, xScroll + 5, num5 + num7 / 2 - 4, mFont.LEFT);
						continue;
					}
					g.setColor((i != selected) ? 15196114 : 16383818);
					g.fillRect(num4, num5, num6, num7);
					g.setColor((i != selected) ? 9993045 : 9541120);
					g.fillRect(num, num2, num3, h);
					InfoItem infoItem = (InfoItem)logChat.elementAt(i - 1);
					if (infoItem.charInfo.headICON != -1)
					{
						SmallImage.drawSmallImage(g, infoItem.charInfo.headICON, num, num2, 0, 0);
					}
					else
					{
						Part part = GameScr.parts[infoItem.charInfo.head];
						SmallImage.drawSmallImage(g, part.pi[Char.CharInfo[0][0][0]].id, num + part.pi[Char.CharInfo[0][0][0]].dx, num2 + part.pi[Char.CharInfo[0][0][0]].dy, 0, 0);
					}
					g.setClip(xScroll, yScroll + cmy, wScroll, hScroll);
					mFont tahoma_7b_dark = mFont.tahoma_7b_dark;
					tahoma_7b_dark = mFont.tahoma_7b_green2;
					tahoma_7b_dark.drawString(g, infoItem.charInfo.cName, num4 + 5, num5, 0);
					if (!infoItem.isChatServer)
					{
						mFont.tahoma_7_blue.drawString(g, Res.split(infoItem.s, "|", 0)[2], num4 + 5, num5 + 11, 0);
					}
					else
					{
						mFont.tahoma_7_red.drawString(g, Res.split(infoItem.s, "|", 0)[2], num4 + 5, num5 + 11, 0);
					}
				}
				paintScrollArrow(g);
			}
	private void paintFlagChange(mGraphics g)
			{
				g.setClip(xScroll, yScroll, wScroll, hScroll);
				g.translate(0, -cmy);
				g.setColor(0);
				for (int i = 0; i < currentListLength; i++)
				{
					int num = xScroll + 26;
					int num2 = yScroll + i * ITEM_HEIGHT;
					int num3 = wScroll - 26;
					int h = ITEM_HEIGHT - 1;
					int num4 = xScroll;
					int num5 = yScroll + i * ITEM_HEIGHT;
					int num6 = 24;
					int num7 = ITEM_HEIGHT - 1;
					if (num2 - cmy > yScroll + hScroll || num2 - cmy < yScroll - ITEM_HEIGHT)
					{
						continue;
					}
					g.setColor((i != selected) ? 15196114 : 16383818);
					g.fillRect(num, num2, num3, h);
					g.setColor((i != selected) ? 9993045 : 9541120);
					g.fillRect(num4, num5, num6, num7);
					Item item = (Item)vFlag.elementAt(i);
					if (item == null)
					{
						continue;
					}
					mFont.tahoma_7_green2.drawString(g, item.template.name, num + 5, num2 + 1, 0);
					string text = string.Empty;
					if (item.itemOption != null && item.itemOption.Length >= 1)
					{
						if (item.itemOption[0] != null && item.itemOption[0].optionTemplate.id != 102 && item.itemOption[0].optionTemplate.id != 107)
						{
							text += item.itemOption[0].getOptionString();
						}
						mFont tahoma_7_blue = mFont.tahoma_7_blue;
						tahoma_7_blue.drawString(g, text, num + 5, num2 + 11, 0);
						SmallImage.drawSmallImage(g, item.template.iconID, num4 + num6 / 2, num5 + num7 / 2, 0, 3);
					}
				}
				paintScrollArrow(g);
			}
	private void paintEnemy(mGraphics g)
			{
				g.setClip(xScroll, yScroll, wScroll, hScroll);
				g.translate(0, -cmy);
				g.setColor(0);
				if (currentListLength == 0)
				{
					mFont.tahoma_7_green2.drawString(g, mResources.no_enemy, xScroll + wScroll / 2, yScroll + hScroll / 2 - mFont.tahoma_7.getHeight() / 2, 2);
					return;
				}
				for (int i = 0; i < currentListLength; i++)
				{
					int num = xScroll;
					int num2 = yScroll + i * ITEM_HEIGHT;
					int num3 = 24;
					int h = ITEM_HEIGHT - 1;
					int num4 = xScroll + num3;
					int num5 = yScroll + i * ITEM_HEIGHT;
					int num6 = wScroll - num3;
					int h2 = ITEM_HEIGHT - 1;
					g.setColor((i != selected) ? 15196114 : 16383818);
					g.fillRect(num4, num5, num6, h2);
					g.setColor((i != selected) ? 9993045 : 9541120);
					g.fillRect(num, num2, num3, h);
					InfoItem infoItem = (InfoItem)vEnemy.elementAt(i);
					if (infoItem.charInfo.headICON != -1)
					{
						SmallImage.drawSmallImage(g, infoItem.charInfo.headICON, num, num2, 0, 0);
					}
					else
					{
						Part part = GameScr.parts[infoItem.charInfo.head];
						SmallImage.drawSmallImage(g, part.pi[Char.CharInfo[0][0][0]].id, num + part.pi[Char.CharInfo[0][0][0]].dx, num2 + 3 + part.pi[Char.CharInfo[0][0][0]].dy, 0, 0);
					}
					g.setClip(xScroll, yScroll + cmy, wScroll, hScroll);
					if (infoItem.isOnline)
					{
						mFont.tahoma_7b_green.drawString(g, infoItem.charInfo.cName, num4 + 5, num5, 0);
						mFont.tahoma_7_blue.drawString(g, infoItem.s, num4 + 5, num5 + 11, 0);
					}
					else
					{
						mFont.tahoma_7_grey.drawString(g, infoItem.charInfo.cName, num4 + 5, num5, 0);
						mFont.tahoma_7_grey.drawString(g, infoItem.s, num4 + 5, num5 + 11, 0);
					}
				}
				paintScrollArrow(g);
			}

}
