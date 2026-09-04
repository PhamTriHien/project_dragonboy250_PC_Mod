using System;
using System.Collections.Generic;
using Assets.src.g;
using UnityEngine;

public partial class Panel : IActionListener, IChatable
{
	private void paintGiaoDich(mGraphics g, bool isMe)
			{
				g.setColor(16711680);
				g.setClip(xScroll, yScroll, wScroll, hScroll);
				g.translate(0, -cmy);
				MyVector myVector = ((!isMe) ? vFriendGD : vMyGD);
				for (int i = 0; i < currentListLength; i++)
				{
					int num = xScroll + 36;
					int num2 = yScroll + i * ITEM_HEIGHT;
					int num3 = wScroll - 36;
					int num4 = ITEM_HEIGHT - 1;
					int num5 = xScroll;
					int num6 = yScroll + i * ITEM_HEIGHT;
					int num7 = 34;
					int num8 = ITEM_HEIGHT - 1;
					if (num2 - cmy > yScroll + hScroll || num2 - cmy < yScroll - ITEM_HEIGHT)
					{
						continue;
					}
					if (i == currentListLength - 1)
					{
						if (!isMe)
						{
							continue;
						}
						g.setColor(15196114);
						g.fillRect(num5, num2, wScroll, num4);
						if (!isLock)
						{
							if (!isFriendLock)
							{
								mFont.tahoma_7_grey.drawString(g, mResources.opponent + mResources.not_lock_trade, xScroll + wScroll / 2, num2 + num4 / 2 - 4, mFont.CENTER);
							}
							else
							{
								mFont.tahoma_7_grey.drawString(g, mResources.opponent + mResources.locked_trade, xScroll + wScroll / 2, num2 + num4 / 2 - 4, mFont.CENTER);
							}
						}
						else if (isFriendLock)
						{
							g.setColor(15196114);
							g.fillRect(num5, num2, wScroll, num4);
							g.drawImage((i != selected) ? GameScr.imgLbtn2 : GameScr.imgLbtnFocus2, xScroll + wScroll - 5, num2 + 2, StaticObj.TOP_RIGHT);
							((i != selected) ? mFont.tahoma_7b_dark : mFont.tahoma_7b_green2).drawString(g, mResources.done, xScroll + wScroll - 22, num2 + 7, 2);
							mFont.tahoma_7_grey.drawString(g, mResources.opponent + mResources.locked_trade, xScroll + 5, num2 + num4 / 2 - 4, mFont.LEFT);
						}
						else
						{
							mFont.tahoma_7_grey.drawString(g, mResources.opponent + mResources.not_lock_trade, xScroll + wScroll / 2, num2 + num4 / 2 - 4, mFont.CENTER);
						}
						continue;
					}
					if (i == currentListLength - 2)
					{
						if (isMe)
						{
							g.setColor(15196114);
							g.fillRect(num5, num2, wScroll, num4);
							if (!isAccept)
							{
								if (!isLock)
								{
									g.drawImage((i != selected) ? GameScr.imgLbtn2 : GameScr.imgLbtnFocus2, xScroll + wScroll - 5, num2 + 2, StaticObj.TOP_RIGHT);
									((i != selected) ? mFont.tahoma_7b_dark : mFont.tahoma_7b_green2).drawString(g, mResources.mlock, xScroll + wScroll - 22, num2 + 7, 2);
									mFont.tahoma_7_grey.drawString(g, mResources.you + mResources.not_lock_trade, xScroll + 5, num2 + num4 / 2 - 4, mFont.LEFT);
								}
								else
								{
									g.drawImage((i != selected) ? GameScr.imgLbtn2 : GameScr.imgLbtnFocus2, xScroll + wScroll - 5, num2 + 2, StaticObj.TOP_RIGHT);
									((i != selected) ? mFont.tahoma_7b_dark : mFont.tahoma_7b_green2).drawString(g, mResources.CANCEL, xScroll + wScroll - 22, num2 + 7, 2);
									mFont.tahoma_7_grey.drawString(g, mResources.you + mResources.locked_trade, xScroll + 5, num2 + num4 / 2 - 4, mFont.LEFT);
								}
							}
						}
						else if (!isFriendLock)
						{
							mFont.tahoma_7b_dark.drawString(g, mResources.not_lock_trade_upper, xScroll + wScroll / 2, num2 + num4 / 2 - 4, mFont.CENTER);
						}
						else
						{
							mFont.tahoma_7b_dark.drawString(g, mResources.locked_trade_upper, xScroll + wScroll / 2, num2 + num4 / 2 - 4, mFont.CENTER);
						}
						continue;
					}
					if (i == currentListLength - 3)
					{
						if (isLock)
						{
							g.setColor(13748667);
						}
						else
						{
							g.setColor((i != selected) ? 15196114 : 16383818);
						}
						g.fillRect(num, num2, num3, num4);
						if (isLock)
						{
							g.setColor(13748667);
						}
						else
						{
							g.setColor((i != selected) ? 9993045 : 7300181);
						}
						g.fillRect(num5, num6, num7, num8);
						g.drawImage(imgXu, num5 + num7 / 2, num6 + num8 / 2, 3);
						mFont.tahoma_7_green2.drawString(g, NinjaUtil.getMoneys((!isMe) ? friendMoneyGD : moneyGD) + " " + mResources.XU, num + 5, num2 + 11, 0);
						mFont.tahoma_7_green.drawString(g, mResources.money_trade, num + 5, num2, 0);
						continue;
					}
					if (myVector.size() == 0)
					{
						return;
					}
					if (isLock)
					{
						g.setColor(13748667);
					}
					else
					{
						g.setColor((i != selected) ? 15196114 : 16383818);
					}
					g.fillRect(num, num2, num3, num4);
					if (isLock)
					{
						g.setColor(13748667);
					}
					else
					{
						g.setColor((i != selected) ? 9993045 : 9541120);
					}
					Item item = (Item)myVector.elementAt(i);
					if (item != null)
					{
						for (int j = 0; j < item.itemOption.Length; j++)
						{
							if (item.itemOption[j].optionTemplate.id != 72 || item.itemOption[j].param <= 0)
							{
								continue;
							}
							sbyte color_Item_Upgrade = GetColor_Item_Upgrade(item.itemOption[j].param);
							int color_ItemBg = GetColor_ItemBg(color_Item_Upgrade);
							if (color_ItemBg != -1)
							{
								if (isLock)
								{
									g.setColor(13748667);
								}
								else
								{
									g.setColor((i != selected) ? GetColor_ItemBg(color_Item_Upgrade) : GetColor_ItemBg(color_Item_Upgrade));
								}
							}
						}
					}
					g.fillRect(num5, num6, num7, num8);
					if (item == null)
					{
						continue;
					}
					string text = string.Empty;
					mFont mFont2 = mFont.tahoma_7_green2;
					if (item.itemOption != null)
					{
						for (int k = 0; k < item.itemOption.Length; k++)
						{
							if (item.itemOption[k].optionTemplate.id == 72)
							{
								text = " [+" + item.itemOption[k].param + "]";
							}
							if (item.itemOption[k].optionTemplate.id == 41)
							{
								if (item.itemOption[k].param == 1)
								{
									mFont2 = GetFont(0);
								}
								else if (item.itemOption[k].param == 2)
								{
									mFont2 = GetFont(2);
								}
								else if (item.itemOption[k].param == 3)
								{
									mFont2 = GetFont(8);
								}
								else if (item.itemOption[k].param == 4)
								{
									mFont2 = GetFont(7);
								}
							}
						}
					}
					mFont2.drawString(g, item.template.name + text, num + 5, num2 + 1, 0);
					string text2 = string.Empty;
					if (item.itemOption != null)
					{
						if (item.itemOption.Length > 0 && item.itemOption[0] != null)
						{
							text2 += item.itemOption[0].getOptionString();
						}
						mFont mFont3 = mFont.tahoma_7_blue;
						if (item.compare < 0 && item.template.type != 5)
						{
							mFont3 = mFont.tahoma_7_red;
						}
						if (item.itemOption.Length > 1)
						{
							for (int l = 1; l < item.itemOption.Length; l++)
							{
								if (item.itemOption[l] != null && item.itemOption[l].optionTemplate.id != 102 && item.itemOption[l].optionTemplate.id != 107)
								{
									text2 = text2 + "," + item.itemOption[l].getOptionString();
								}
							}
						}
						mFont3.drawString(g, text2, num + 5, num2 + 11, mFont.LEFT);
					}
					SmallImage.drawSmallImage(g, item.template.iconID, num5 + num7 / 2, num6 + num8 / 2, 0, 3);
					if (item.itemOption != null)
					{
						for (int m = 0; m < item.itemOption.Length; m++)
						{
							paintOptItem(g, item.itemOption[m].optionTemplate.id, item.itemOption[m].param, num5, num6, num7, num8);
						}
						for (int n = 0; n < item.itemOption.Length; n++)
						{
							paintOptSlotItem(g, item.itemOption[n].optionTemplate.id, item.itemOption[n].param, num5, num6, num7, num8);
						}
					}
					if (item.quantity > 1)
					{
						mFont.tahoma_7_yellow.drawString(g, string.Empty + item.quantity, num5 + num7, num6 + num8 - mFont.tahoma_7_yellow.getHeight(), 1);
					}
				}
				paintScrollArrow(g);
			}

	public void paintTop(mGraphics g)
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
					int h = ITEM_HEIGHT - 1;
					int num6 = xScroll + num5;
					int num7 = yScroll + i * ITEM_HEIGHT;
					int num8 = wScroll - num5;
					int num9 = ITEM_HEIGHT - 1;
					g.setColor((i != selected) ? 15196114 : 16383818);
					g.fillRect(num6, num7, num8, num9);
					g.setColor((i != selected) ? 9993045 : 9541120);
					g.fillRect(num3, num4, num5, h);
					TopInfo topInfo = (TopInfo)vTop.elementAt(i);
					if (topInfo.headICON != -1)
					{
						SmallImage.drawSmallImage(g, topInfo.headICON, num3, num4, 0, 0);
					}
					else
					{
						Part part = GameScr.parts[topInfo.headID];
						SmallImage.drawSmallImage(g, part.pi[Char.CharInfo[0][0][0]].id, num3 + part.pi[Char.CharInfo[0][0][0]].dx, num4 + num9 - 1, 0, mGraphics.BOTTOM | mGraphics.LEFT);
					}
					g.setClip(xScroll, yScroll + cmy, wScroll, hScroll);
					if (topInfo.pId != Char.myCharz().charID)
					{
						mFont.tahoma_7b_green.drawString(g, topInfo.name, num6 + 5, num7, 0);
					}
					else
					{
						mFont.tahoma_7b_red.drawString(g, topInfo.name, num6 + 5, num7, 0);
					}
					mFont.tahoma_7_blue.drawString(g, topInfo.info, num6 + num8 - 5, num7 + 11, 1);
					mFont.tahoma_7_green2.drawString(g, mResources.rank + ": " + topInfo.rank + string.Empty, num6 + 5, num7 + 11, 0);
				}
				paintScrollArrow(g);
			}

	public void paint(mGraphics g)
			{
				g.translate(-g.getTranslateX(), -g.getTranslateY() + mGraphics.addYWhenOpenKeyBoard);
				g.translate(-cmx, 0);
				g.translate(X, Y);
				if (GameCanvas.panel.combineSuccess != -1)
				{
					if (Equals(GameCanvas.panel))
					{
						paintCombineEff(g);
					}
					return;
				}
				GameCanvas.paintz.paintFrameSimple(X, Y, W, H, g);
				try
				{
					paintTopInfo(g);
				}
				catch (Exception)
				{
				}
				paintBottomMoneyInfo(g);
				paintTab(g);
				switch (type)
				{
				case 9:
					paintArchivement(g);
					break;
				case 21:
					if (currentTabIndex == 0)
					{
						paintPetInventory(g);
					}
					if (currentTabIndex == 1)
					{
						paintPetStatus(g);
					}
					if (currentTabIndex == 2)
					{
						paintInventory(g);
					}
					break;
				case 24:
					paintGameSubInfo(g);
					break;
				case 23:
					paintGameInfo(g);
					break;
				case 0:
					if (currentTabIndex == 0)
					{
						paintTask(g);
					}
					if (currentTabIndex == 1)
					{
						paintInventory(g);
					}
					if (currentTabIndex == 2)
					{
						paintSkill(g);
					}
					if (currentTabIndex == 3)
					{
						if (mainTabName.Length == 4)
						{
							paintTools(g);
						}
						else
						{
							paintClans(g);
						}
					}
					if (currentTabIndex == 4)
					{
						paintTools(g);
					}
					break;
				case 2:
					if (currentTabIndex == 0)
					{
						paintBox(g);
					}
					if (currentTabIndex == 1)
					{
						paintInventory(g);
					}
					break;
				case 3:
					paintZone(g);
					break;
				case 1:
					paintShop(g);
					break;
				case 25:
					paintSpeacialSkill(g);
					break;
				case 4:
					paintMap(g);
					break;
				case 7:
					paintInventory(g);
					break;
				case 17:
					paintShop(g);
					break;
				case 8:
					paintLogChat(g);
					break;
				case 10:
					paintPlayerMenu(g);
					break;
				case 11:
					paintFriend(g);
					break;
				case 16:
					paintEnemy(g);
					break;
				case 15:
					paintTop(g);
					break;
				case 12:
					if (currentTabIndex == 0)
					{
						paintCombine(g);
					}
					if (currentTabIndex == 1)
					{
						paintInventory(g);
					}
					break;
				case 13:
					if (currentTabIndex == 0)
					{
						if (Equals(GameCanvas.panel))
						{
							paintInventory(g);
						}
						else
						{
							paintGiaoDich(g, isMe: false);
						}
					}
					if (currentTabIndex == 1)
					{
						paintGiaoDich(g, isMe: true);
					}
					if (currentTabIndex == 2)
					{
						paintGiaoDich(g, isMe: false);
					}
					break;
				case 14:
					paintMapTrans(g);
					break;
				case 18:
					paintFlagChange(g);
					break;
				case 19:
					paintOption(g);
					break;
				case 20:
					paintAccount(g);
					break;
				case 22:
					paintAuto(g);
					break;
				}
				GameScr.resetTranslate(g);
				paintDetail(g);
				if (cmx == cmtoX && !GameCanvas.menu.showMenu)
				{
					cmdClose.paint(g);
				}
				if (tabIcon != null && tabIcon.isShow)
				{
					tabIcon.paint(g);
				}
				g.translate(-g.getTranslateX(), -g.getTranslateY());
				g.translate(X, Y);
				g.translate(-cmx, 0);
			}

	private void paintAuto(mGraphics g)
			{
			}

	private void paintScrollArrow(mGraphics g)
			{
				g.translate(-g.getTranslateX(), -g.getTranslateY());
				if ((cmy > 24 && currentListLength > 0) || (Equals(GameCanvas.panel) && typeShop == 2 && maxPageShop[currentTabIndex] > 1))
				{
					g.drawRegion(Mob.imgHP, 0, 0, 9, 6, 1, xScroll + wScroll - 12, yScroll + 3, 0);
				}
				if ((cmy < cmyLim && currentListLength > 0) || (Equals(GameCanvas.panel) && typeShop == 2 && maxPageShop[currentTabIndex] > 1))
				{
					g.drawRegion(Mob.imgHP, 0, 0, 9, 6, 0, xScroll + wScroll - 12, yScroll + hScroll - 8, 0);
				}
			}

	private void paintTools(mGraphics g)
			{
				g.setClip(xScroll, yScroll, wScroll, hScroll);
				g.translate(0, -cmy);
				for (int i = 0; i < strTool.Length; i++)
				{
					int num = xScroll;
					int num2 = yScroll + i * ITEM_HEIGHT;
					int num3 = wScroll - 1;
					int h = ITEM_HEIGHT - 1;
					if (num2 - cmy > yScroll + hScroll || num2 - cmy < yScroll - ITEM_HEIGHT)
					{
						continue;
					}
					g.setColor((i != selected) ? 15196114 : 16383818);
					g.fillRect(num, num2, num3, h);
					mFont.tahoma_7b_dark.drawString(g, strTool[i], xScroll + wScroll / 2, num2 + 6, mFont.CENTER);
					if (!strTool[i].Equals(mResources.gameInfo))
					{
						continue;
					}
					for (int j = 0; j < vGameInfo.size(); j++)
					{
						GameInfo gameInfo = (GameInfo)vGameInfo.elementAt(j);
						if (!gameInfo.hasRead)
						{
							if (GameCanvas.gameTick % 20 > 10)
							{
								g.drawImage(imgNew, num + 10, num2 + 10, 3);
							}
							break;
						}
					}
				}
				paintScrollArrow(g);
			}

	private void paintGameSubInfo(mGraphics g)
			{
				g.setClip(xScroll, yScroll, wScroll, hScroll);
				g.translate(0, -cmy);
				for (int i = 0; i < contenInfo.Length; i++)
				{
					int num = xScroll;
					int num2 = yScroll + i * 15;
					int num3 = wScroll - 1;
					int num4 = ITEM_HEIGHT - 1;
					if (num2 - cmy <= yScroll + hScroll && num2 - cmy >= yScroll - ITEM_HEIGHT)
					{
						mFont.tahoma_7b_dark.drawString(g, contenInfo[i], xScroll + 5, num2 + 6, mFont.LEFT);
					}
				}
				paintScrollArrow(g);
			}

	private void paintGameInfo(mGraphics g)
			{
				g.setClip(xScroll, yScroll, wScroll, hScroll);
				g.translate(0, -cmy);
				for (int i = 0; i < vGameInfo.size(); i++)
				{
					GameInfo gameInfo = (GameInfo)vGameInfo.elementAt(i);
					int num = xScroll;
					int num2 = yScroll + i * ITEM_HEIGHT;
					int num3 = wScroll - 1;
					int h = ITEM_HEIGHT - 1;
					if (num2 - cmy <= yScroll + hScroll && num2 - cmy >= yScroll - ITEM_HEIGHT)
					{
						g.setColor((i != selected) ? 15196114 : 16383818);
						g.fillRect(num, num2, num3, h);
						mFont.tahoma_7b_dark.drawString(g, gameInfo.main, xScroll + wScroll / 2, num2 + 6, mFont.CENTER);
						if (!gameInfo.hasRead && GameCanvas.gameTick % 20 > 10)
						{
							g.drawImage(imgNew, num + 10, num2 + 10, 3);
						}
					}
				}
				paintScrollArrow(g);
			}

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

	private void paintFriend(mGraphics g)
			{
				g.setClip(xScroll, yScroll, wScroll, hScroll);
				g.translate(0, -cmy);
				g.setColor(0);
				if (currentListLength == 0)
				{
					mFont.tahoma_7_green2.drawString(g, mResources.no_friend, xScroll + wScroll / 2, yScroll + hScroll / 2 - mFont.tahoma_7.getHeight() / 2, 2);
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
					InfoItem infoItem = (InfoItem)vFriend.elementAt(i);
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

	public void paintPlayerMenu(mGraphics g)
			{
				g.setClip(xScroll, yScroll, wScroll, hScroll);
				g.translate(0, -cmy);
				for (int i = 0; i < vPlayerMenu.size(); i++)
				{
					int x = xScroll;
					int num = yScroll + i * ITEM_HEIGHT;
					int num2 = wScroll - 1;
					int h = ITEM_HEIGHT - 1;
					if (num - cmy <= yScroll + hScroll && num - cmy >= yScroll - ITEM_HEIGHT)
					{
						Command command = (Command)vPlayerMenu.elementAt(i);
						g.setColor((i != selected) ? 15196114 : 16383818);
						g.fillRect(x, num, num2, h);
						if (command.caption2.Equals(string.Empty))
						{
							mFont.tahoma_7b_dark.drawString(g, command.caption, xScroll + wScroll / 2, num + 6, mFont.CENTER);
							continue;
						}
						mFont.tahoma_7b_dark.drawString(g, command.caption, xScroll + wScroll / 2, num + 1, mFont.CENTER);
						mFont.tahoma_7b_dark.drawString(g, command.caption2, xScroll + wScroll / 2, num + 11, mFont.CENTER);
					}
				}
				paintScrollArrow(g);
			}

	private void paintArchivement(mGraphics g)
			{
				g.setClip(xScroll, yScroll, wScroll, hScroll);
				g.translate(0, -cmy);
				g.setColor(0);
				if (currentListLength == 0)
				{
					mFont.tahoma_7_green2.drawString(g, mResources.no_mission, xScroll + wScroll / 2, yScroll + hScroll / 2 - mFont.tahoma_7.getHeight() / 2, 2);
				}
				else
				{
					if (Char.myCharz().arrArchive == null || Char.myCharz().arrArchive.Length != currentListLength)
					{
						return;
					}
					for (int i = 0; i < currentListLength; i++)
					{
						int num = xScroll;
						int num2 = yScroll + i * ITEM_HEIGHT;
						int num3 = wScroll;
						int num4 = ITEM_HEIGHT - 1;
						Archivement archivement = Char.myCharz().arrArchive[i];
						g.setColor((i != selected || ((archivement.isRecieve || archivement.isFinish) && (!archivement.isRecieve || !archivement.isFinish))) ? 15196114 : 16383818);
						g.fillRect(num, num2, num3, num4);
						if (archivement == null)
						{
							continue;
						}
						if (!archivement.isFinish)
						{
							mFont.tahoma_7.drawString(g, archivement.info1, num + 5, num2, 0);
							mFont.tahoma_7_green.drawString(g, archivement.money + " " + mResources.RUBY, num + num3 - 5, num2, mFont.RIGHT);
							mFont.tahoma_7_red.drawString(g, archivement.info2, num + 5, num2 + 11, 0);
						}
						else if (archivement.isFinish && !archivement.isRecieve)
						{
							mFont.tahoma_7.drawString(g, archivement.info1, num + 5, num2, 0);
							mFont.tahoma_7_blue.drawString(g, mResources.reward_mission + archivement.money + " " + mResources.RUBY, num + 5, num2 + 11, 0);
							if (i == selected)
							{
								mFont.tahoma_7b_green2.drawString(g, mResources.receive_upper, num + num3 - 20, num2 + 6, mFont.CENTER);
								mFont.tahoma_7b_dark.drawString(g, mResources.receive_upper, num + num3 - 20, num2 + 6, mFont.CENTER);
							}
							else
							{
								g.drawImage(GameScr.imgLbtn2, num + num3 - 20, num2 + num4 / 2, StaticObj.VCENTER_HCENTER);
								mFont.tahoma_7b_dark.drawString(g, mResources.receive_upper, num + num3 - 20, num2 + 6, mFont.CENTER);
							}
						}
						else if (archivement.isFinish && archivement.isRecieve)
						{
							mFont.tahoma_7_green.drawString(g, archivement.info1, num + 5, num2, 0);
							mFont.tahoma_7_green.drawString(g, archivement.info2, num + 5, num2 + 11, 0);
						}
					}
					paintScrollArrow(g);
				}
			}

	private void paintTab(mGraphics g)
			{
				if (type == 23 || type == 24)
				{
					g.setColor(13524492);
					g.fillRect(X + 1, 78, W - 2, 1);
					mFont.tahoma_7b_dark.drawString(g, mResources.gameInfo, xScroll + wScroll / 2, 59, mFont.CENTER);
					return;
				}
				if (type == 20)
				{
					g.setColor(13524492);
					g.fillRect(X + 1, 78, W - 2, 1);
					mFont.tahoma_7b_dark.drawString(g, mResources.account, xScroll + wScroll / 2, 59, mFont.CENTER);
					return;
				}
				if (type == 22)
				{
					g.setColor(13524492);
					g.fillRect(X + 1, 78, W - 2, 1);
					mFont.tahoma_7b_dark.drawString(g, mResources.autoFunction, xScroll + wScroll / 2, 59, mFont.CENTER);
					return;
				}
				if (type == 19)
				{
					g.setColor(13524492);
					g.fillRect(X + 1, 78, W - 2, 1);
					mFont.tahoma_7b_dark.drawString(g, mResources.option, xScroll + wScroll / 2, 59, mFont.CENTER);
					return;
				}
				if (type == 18)
				{
					g.setColor(13524492);
					g.fillRect(X + 1, 78, W - 2, 1);
					mFont.tahoma_7b_dark.drawString(g, mResources.change_flag, xScroll + wScroll / 2, 59, mFont.CENTER);
					return;
				}
				if (type == 13 && Equals(GameCanvas.panel2))
				{
					g.setColor(13524492);
					g.fillRect(X + 1, 78, W - 2, 1);
					mFont.tahoma_7b_dark.drawString(g, mResources.item_receive2, xScroll + wScroll / 2, 59, mFont.CENTER);
					return;
				}
				if (type == 12 && GameCanvas.panel2 != null)
				{
					g.setColor(13524492);
					g.fillRect(X + 1, 78, W - 2, 1);
					mFont.tahoma_7b_dark.drawString(g, mResources.UPGRADE, xScroll + wScroll / 2, 59, mFont.CENTER);
					return;
				}
				if (type == 11)
				{
					g.setColor(13524492);
					g.fillRect(X + 1, 78, W - 2, 1);
					mFont.tahoma_7b_dark.drawString(g, mResources.friend, xScroll + wScroll / 2, 59, mFont.CENTER);
					return;
				}
				if (type == 16)
				{
					g.setColor(13524492);
					g.fillRect(X + 1, 78, W - 2, 1);
					mFont.tahoma_7b_dark.drawString(g, mResources.enemy, xScroll + wScroll / 2, 59, mFont.CENTER);
					return;
				}
				if (type == 15)
				{
					g.setColor(13524492);
					g.fillRect(X + 1, 78, W - 2, 1);
					mFont.tahoma_7b_dark.drawString(g, topName, xScroll + wScroll / 2, 59, mFont.CENTER);
					return;
				}
				if (type == 2 && GameCanvas.panel2 != null)
				{
					g.setColor(13524492);
					g.fillRect(X + 1, 78, W - 2, 1);
					mFont.tahoma_7b_dark.drawString(g, mResources.chest, xScroll + wScroll / 2, 59, mFont.CENTER);
					return;
				}
				if (type == 9)
				{
					g.setColor(13524492);
					g.fillRect(X + 1, 78, W - 2, 1);
					mFont.tahoma_7b_dark.drawString(g, mResources.achievement_mission, xScroll + wScroll / 2, 59, mFont.CENTER);
					return;
				}
				if (type == 3)
				{
					g.setColor(13524492);
					g.fillRect(X + 1, 78, W - 2, 1);
					mFont.tahoma_7b_dark.drawString(g, mResources.select_zone, startTabPos + TAB_W / 2, 59, mFont.CENTER);
					return;
				}
				if (type == 14)
				{
					g.setColor(13524492);
					g.fillRect(X + 1, 78, W - 2, 1);
					mFont.tahoma_7b_dark.drawString(g, mResources.select_map, startTabPos + TAB_W / 2, 59, mFont.CENTER);
					return;
				}
				if (type == 4)
				{
					mFont.tahoma_7b_dark.drawString(g, mResources.map, startTabPos + TAB_W / 2, 59, mFont.CENTER);
					g.setColor(13524492);
					g.fillRect(X + 1, 78, W - 2, 1);
					return;
				}
				if (type == 7)
				{
					mFont.tahoma_7b_dark.drawString(g, mResources.trangbi, startTabPos + TAB_W / 2, 59, mFont.CENTER);
					g.setColor(13524492);
					g.fillRect(X + 1, 78, W - 2, 1);
					return;
				}
				if (type == 17)
				{
					mFont.tahoma_7b_dark.drawString(g, mResources.kigui, startTabPos + TAB_W / 2, 59, mFont.CENTER);
					g.setColor(13524492);
					g.fillRect(X + 1, 78, W - 2, 1);
					return;
				}
				if (type == 8)
				{
					mFont.tahoma_7b_dark.drawString(g, mResources.msg, startTabPos + TAB_W / 2, 59, mFont.CENTER);
					g.setColor(13524492);
					g.fillRect(X + 1, 78, W - 2, 1);
					return;
				}
				if (type == 10)
				{
					mFont.tahoma_7b_dark.drawString(g, mResources.wat_do_u_want, startTabPos + TAB_W / 2, 59, mFont.CENTER);
					g.setColor(13524492);
					g.fillRect(X + 1, 78, W - 2, 1);
					return;
				}
				if (currentTabIndex == 3 && mainTabName.Length != 4)
				{
					g.translate(-cmx, 0);
				}
				for (int i = 0; i < currentTabName.Length; i++)
				{
					g.setColor((i != currentTabIndex) ? 16773296 : 6805896);
					PopUp.paintPopUp(g, startTabPos + i * TAB_W, 52, TAB_W - 1, 25, (i == currentTabIndex) ? 1 : 0, isButton: true);
					if (i == keyTouchTab)
					{
						g.drawImage(ItemMap.imageFlare, startTabPos + i * TAB_W + TAB_W / 2, 62, 3);
					}
					mFont mFont2 = ((i != currentTabIndex) ? mFont.tahoma_7_grey : mFont.tahoma_7_green2);
					if (!currentTabName[i][1].Equals(string.Empty))
					{
						mFont2.drawString(g, currentTabName[i][0], startTabPos + i * TAB_W + TAB_W / 2, 53, mFont.CENTER);
						mFont2.drawString(g, currentTabName[i][1], startTabPos + i * TAB_W + TAB_W / 2, 64, mFont.CENTER);
					}
					else
					{
						mFont2.drawString(g, currentTabName[i][0], startTabPos + i * TAB_W + TAB_W / 2, 59, mFont.CENTER);
					}
					if (type == 0 && currentTabName.Length == 5 && GameScr.isNewClanMessage && GameCanvas.gameTick % 4 == 0)
					{
						g.drawImage(ItemMap.imageFlare, startTabPos + 3 * TAB_W + TAB_W / 2, 77, mGraphics.BOTTOM | mGraphics.HCENTER);
					}
				}
				g.setColor(13524492);
				g.fillRect(1, 78, W - 2, 1);
			}

	private void paintBottomMoneyInfo(mGraphics g)
			{
				if (type != 13 || (currentTabIndex != 2 && !Equals(GameCanvas.panel2)))
				{
					g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
					g.setColor(11837316);
					g.fillRect(X + 1, H - 15, W - 2, 14);
					g.setColor(13524492);
					g.fillRect(X + 1, H - 15, W - 2, 1);
					g.drawImage(imgXu, X + 11, H - 7, 3);
					g.drawImage(imgLuong, X + 75, H - 8, 3);
					mFont.tahoma_7_yellow.drawString(g, Char.myCharz().xuStr + string.Empty, X + 24, H - 13, mFont.LEFT, mFont.tahoma_7_grey);
					mFont.tahoma_7_yellow.drawString(g, Char.myCharz().luongStr + string.Empty, X + 85, H - 13, mFont.LEFT, mFont.tahoma_7_grey);
					g.drawImage(imgLuongKhoa, X + 130, H - 8, 3);
					mFont.tahoma_7_yellow.drawString(g, Char.myCharz().luongKhoaStr + string.Empty, X + 140, H - 13, mFont.LEFT, mFont.tahoma_7_grey);
				}
			}

	private void paintToolInfo(mGraphics g)
			{
				mFont.tahoma_7b_white.drawString(g, mResources.dragon_ball + " " + GameMidlet.VERSION, 60, 4, mFont.LEFT, mFont.tahoma_7b_dark);
				mFont.tahoma_7_yellow.drawString(g, mResources.character + ": " + Char.myCharz().cName, 60, 16, mFont.LEFT, mFont.tahoma_7_grey);
				string text = ((!GameCanvas.loginScr.tfUser.getText().Equals(string.Empty)) ? GameCanvas.loginScr.tfUser.getText() : mResources.not_register_yet);
				string svName = (ServerListScreen.nameServer != null && ServerListScreen.ipSelect >= 0 && ServerListScreen.ipSelect < ServerListScreen.nameServer.Length) ? ServerListScreen.nameServer[ServerListScreen.ipSelect] : string.Empty;
				mFont.tahoma_7_yellow.drawString(g, mResources.account_server + " " + svName + ": " + text, 60, 27, mFont.LEFT, mFont.tahoma_7_grey);
			}

	private void paintGiaoDichInfo(mGraphics g)
			{
				mFont.tahoma_7_yellow.drawString(g, mResources.select_item, 60, 4, mFont.LEFT, mFont.tahoma_7_grey);
				mFont.tahoma_7_yellow.drawString(g, mResources.lock_trade, 60, 16, mFont.LEFT, mFont.tahoma_7_grey);
				mFont.tahoma_7_yellow.drawString(g, mResources.wait_opp_lock_trade, 60, 27, mFont.LEFT, mFont.tahoma_7_grey);
				mFont.tahoma_7_yellow.drawString(g, mResources.press_done, 60, 38, mFont.LEFT, mFont.tahoma_7_grey);
			}

	private void paintMyInfo(mGraphics g)
			{
				paintCharInfo(g, Char.myCharz());
			}

	private void paintCharInfo(mGraphics g, Char c)
			{
				mFont.tahoma_7b_white.drawString(g, ((GameScr.isNewMember == 1) ? "       " : string.Empty) + c.cName, X + 60, 4, mFont.LEFT, mFont.tahoma_7b_dark);
				if (GameScr.isNewMember == 1)
				{
					SmallImage.drawSmallImage(g, 5427, X + 55, 4, 0, 0);
				}
				if (c.cMaxStamina > 0)
				{
					mFont.tahoma_7_yellow.drawString(g, mResources.vitality, X + 60, 16, mFont.LEFT, mFont.tahoma_7_grey);
					g.drawImage(GameScr.imgMPLost, X + 95, 19, 0);
					int num = c.cStamina * mGraphics.getImageWidth(GameScr.imgMP) / c.cMaxStamina;
					g.setClip(95, X + 19, num, 20);
					g.drawImage(GameScr.imgMP, X + 95, 19, 0);
				}
				g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
				if (c.cPower > 0)
				{
					mFont.tahoma_7_yellow.drawString(g, (!c.me) ? c.currStrLevel : c.getStrLevel(), X + 60, 27, mFont.LEFT, mFont.tahoma_7_grey);
				}
				mFont.tahoma_7_yellow.drawString(g, mResources.power + ": " + NinjaUtil.getMoneys(c.cPower), X + 60, 38, mFont.LEFT, mFont.tahoma_7_grey);
			}

	private void paintCharInfo(mGraphics g, Char c, int x, int y)
			{
				mFont.tahoma_7b_white.drawString(g, ((GameScr.isNewMember == 1) ? "       " : string.Empty) + c.cName, x + 60, y + 4, mFont.LEFT, mFont.tahoma_7b_dark);
				if (GameScr.isNewMember == 1)
				{
					SmallImage.drawSmallImage(g, 5427, x + 55, y + 4, 0, 0);
				}
				if (c.cMaxStamina > 0)
				{
					mFont.tahoma_7_yellow.drawString(g, mResources.vitality, x + 60, y + 16, mFont.LEFT, mFont.tahoma_7_grey);
					g.drawImage(GameScr.imgMPLost, x + 95, y + 19, 0);
					int num = c.cStamina * mGraphics.getImageWidth(GameScr.imgMP) / c.cMaxStamina;
					g.drawImage(GameScr.imgMP, x + 95, y + 19, 0);
				}
				if (c.cPower > 0)
				{
					mFont.tahoma_7_yellow.drawString(g, (!c.me) ? c.currStrLevel : c.getStrLevel(), x + 60, y + 27, mFont.LEFT, mFont.tahoma_7_grey);
				}
				mFont.tahoma_7_yellow.drawString(g, mResources.power + ": " + NinjaUtil.getMoneys(c.cPower), x + 60, y + 38, mFont.LEFT, mFont.tahoma_7_grey);
			}

	private void paintSkillInfo(mGraphics g)
			{
				mFont.tahoma_7_white.drawString(g, "Top " + Char.myCharz().rank, X + 45 + (W - 50) / 2, 2, mFont.CENTER);
				mFont.tahoma_7_yellow.drawString(g, mResources.potential_point, X + 45 + (W - 50) / 2, 14, mFont.CENTER);
				mFont.tahoma_7_white.drawString(g, string.Empty + NinjaUtil.getMoneys(Char.myCharz().cTiemNang), X + ((GameCanvas.gameTick % 20 > 10) ? (GameCanvas.gameTick % 4 / 2) : 0) + 45 + (W - 50) / 2, 26, mFont.CENTER);
				mFont.tahoma_7_yellow.drawString(g, mResources.active_point + ": " + NinjaUtil.getMoneys(Char.myCharz().cNangdong), X + ((GameCanvas.gameTick % 20 > 10) ? (GameCanvas.gameTick % 4 / 2) : 0) + 45 + (W - 50) / 2, 38, mFont.CENTER);
			}

	private void paintTopInfo(mGraphics g)
			{
				g.setClip(X + 1, Y, W - 2, yScroll - 2);
				g.setColor(9993045);
				g.fillRect(X, Y, W - 2, 50);
				switch (type)
				{
				case 13:
					if (currentTabIndex == 0 || currentTabIndex == 1)
					{
						if (Equals(GameCanvas.panel))
						{
							SmallImage.drawSmallImage(g, Char.myCharz().avatarz(), X + 25, 50, 0, 33);
							paintGiaoDichInfo(g);
						}
						if (Equals(GameCanvas.panel2) && charMenu != null)
						{
							SmallImage.drawSmallImage(g, charMenu.avatarz(), X + 25, 50, 0, 33);
							paintCharInfo(g, charMenu);
						}
					}
					if (currentTabIndex == 2 && charMenu != null)
					{
						SmallImage.drawSmallImage(g, charMenu.avatarz(), X + 25, 50, 0, 33);
						paintCharInfo(g, charMenu);
					}
					break;
				case 12:
					if (currentTabIndex == 0)
					{
						int id = 1410;
						for (int i = 0; i < GameScr.vNpc.size(); i++)
						{
							Npc npc = (Npc)GameScr.vNpc.elementAt(i);
							if (npc.template.npcTemplateId == idNPC)
							{
								id = npc.avatar;
							}
						}
						SmallImage.drawSmallImage(g, id, X + 25, 50, 0, 33);
						paintCombineInfo(g);
					}
					if (currentTabIndex == 1)
					{
						SmallImage.drawSmallImage(g, Char.myCharz().avatarz(), X + 25, 50, 0, 33);
						paintMyInfo(g);
					}
					break;
				case 11:
				case 16:
				case 23:
				case 24:
					SmallImage.drawSmallImage(g, Char.myCharz().avatarz(), X + 25, 50, 0, 33);
					paintMyInfo(g);
					break;
				case 15:
					SmallImage.drawSmallImage(g, Char.myCharz().avatarz(), X + 25, 50, 0, 33);
					paintMyInfo(g);
					break;
				case 9:
					SmallImage.drawSmallImage(g, Char.myCharz().avatarz(), X + 25, 50, 0, 33);
					paintMyInfo(g);
					break;
				case 21:
					if (currentTabIndex == 0)
					{
						Debug.LogWarning(">>>head:" + Char.myPetz().avatarz());
						SmallImage.drawSmallImage(g, Char.myPetz().avatarz(), X + 25, 50, 0, 33);
						paintPetInfo(g);
					}
					if (currentTabIndex == 1)
					{
						SmallImage.drawSmallImage(g, Char.myPetz().avatarz(), X + 25, 50, 0, 33);
						paintPetStatusInfo(g);
					}
					if (currentTabIndex == 2)
					{
						SmallImage.drawSmallImage(g, Char.myCharz().avatarz(), X + 25, 50, 0, 33);
						paintItemBodyBagInfo(g);
					}
					break;
				case 0:
					if (currentTabIndex == 0)
					{
						SmallImage.drawSmallImage(g, Char.myCharz().avatarz(), X + 25, 50, 0, 33);
						paintMyInfo(g);
					}
					if (currentTabIndex == 1)
					{
						SmallImage.drawSmallImage(g, Char.myCharz().avatarz(), X + 25, 50, 0, 33);
						if (isnewInventory)
						{
							paintCharInfo(g, Char.myCharz());
						}
						else
						{
							paintItemBodyBagInfo(g);
						}
					}
					if (currentTabIndex == 2)
					{
						SmallImage.drawSmallImage(g, Char.myCharz().avatarz(), X + 25, 50, 0, 33);
						paintSkillInfo(g);
					}
					if (currentTabIndex == 3)
					{
						if (mainTabName.Length == 5)
						{
							paintClanInfo(g);
						}
						else
						{
							SmallImage.drawSmallImage(g, Char.myCharz().avatarz(), X + 25, 50, 0, 33);
							paintToolInfo(g);
						}
					}
					if (currentTabIndex == 4)
					{
						SmallImage.drawSmallImage(g, Char.myCharz().avatarz(), X + 25, 50, 0, 33);
						paintToolInfo(g);
					}
					break;
				case 25:
					SmallImage.drawSmallImage(g, Char.myCharz().avatarz(), X + 25, 50, 0, 33);
					paintMyInfo(g);
					break;
				case 2:
					if (currentTabIndex == 0)
					{
						SmallImage.drawSmallImage(g, 526, X + 25, 50, 0, 33);
						paintItemBoxInfo(g);
					}
					if (currentTabIndex == 1)
					{
						SmallImage.drawSmallImage(g, Char.myCharz().avatarz(), X + 25, 50, 0, 33);
						paintItemBodyBagInfo(g);
					}
					break;
				case 3:
					SmallImage.drawSmallImage(g, 561, X + 25, 50, 0, 33);
					paintZoneInfo(g);
					break;
				case 1:
					if (currentTabIndex == currentTabName.Length - 1 && GameCanvas.panel2 == null)
					{
						SmallImage.drawSmallImage(g, Char.myCharz().avatarz(), X + 25, 50, 0, 33);
					}
					else if (Char.myCharz().npcFocus != null)
					{
						SmallImage.drawSmallImage(g, Char.myCharz().npcFocus.avatar, X + 25, 50, 0, 33);
					}
					paintShopInfo(g);
					break;
				case 4:
					SmallImage.drawSmallImage(g, Char.myCharz().avatarz(), X + 25, 50, 0, 33);
					paintMapInfo(g);
					break;
				case 7:
				case 17:
					SmallImage.drawSmallImage(g, Char.myCharz().avatarz(), X + 25, 50, 0, 33);
					paintMyInfo(g);
					break;
				case 8:
					SmallImage.drawSmallImage(g, Char.myCharz().avatarz(), X + 25, 50, 0, 33);
					paintMyInfo(g);
					break;
				case 10:
					if (charMenu != null)
					{
						SmallImage.drawSmallImage(g, charMenu.avatarz(), X + 25, 50, 0, 33);
						paintCharInfo(g, charMenu);
					}
					break;
				case 14:
					SmallImage.drawSmallImage(g, Char.myCharz().avatarz(), X + 25, 50, 0, 33);
					paintMapInfo(g);
					break;
				case 18:
					SmallImage.drawSmallImage(g, Char.myCharz().avatarz(), X + 25, 50, 0, 33);
					paintMyInfo(g);
					break;
				case 19:
					SmallImage.drawSmallImage(g, Char.myCharz().avatarz(), X + 25, 50, 0, 33);
					paintToolInfo(g);
					break;
				case 20:
					SmallImage.drawSmallImage(g, Char.myCharz().avatarz(), X + 25, 50, 0, 33);
					paintToolInfo(g);
					break;
				case 22:
					SmallImage.drawSmallImage(g, Char.myCharz().avatarz(), X + 25, 50, 0, 33);
					paintToolInfo(g);
					break;
				case 5:
				case 6:
					break;
				}
			}

	private void paintChatManager(mGraphics g)
			{
			}

	private void paintChatPlayer(mGraphics g)
			{
			}

	private void paintInfomation(mGraphics g)
			{
			}

	public void paintTask(mGraphics g)
			{
				int num = ((GameCanvas.h <= 300) ? 15 : 20);
				if (isPaintMap && !GameScr.gI().isMapDocNhan() && !GameScr.gI().isMapFize())
				{
					g.drawImage((keyTouchMapButton != 1) ? GameScr.imgLbtn : GameScr.imgLbtnFocus, xScroll + wScroll / 2, yScroll + hScroll - num, 3);
					mFont.tahoma_7b_dark.drawString(g, mResources.map, xScroll + wScroll / 2, yScroll + hScroll - (num + 5), mFont.CENTER);
				}
				xstart = xScroll + 5;
				ystart = yScroll + 14;
				yPaint = ystart;
				g.setClip(xScroll, yScroll, wScroll, hScroll - 35);
				if (scroll != null)
				{
					if (scroll.cmy > 0)
					{
						g.drawRegion(Mob.imgHP, 0, 0, 9, 6, 1, xScroll + wScroll - 12, yScroll + 3, 0);
					}
					if (scroll.cmy < scroll.cmyLim)
					{
						g.drawRegion(Mob.imgHP, 0, 0, 9, 6, 0, xScroll + wScroll - 12, yScroll + hScroll - 45, 0);
					}
					g.translate(0, -scroll.cmy);
				}
				indexRowMax = 0;
				if (indexMenu == 0)
				{
					bool flag = false;
					if (Char.myCharz().taskMaint != null)
					{
						for (int i = 0; i < Char.myCharz().taskMaint.names.Length; i++)
						{
							mFont.tahoma_7_grey.drawString(g, Char.myCharz().taskMaint.names[i], xScroll + wScroll / 2, yPaint - 5 + i * 12, mFont.CENTER);
							indexRowMax++;
						}
						yPaint += (Char.myCharz().taskMaint.names.Length - 1) * 12;
						int num2 = 0;
						string empty = string.Empty;
						for (int j = 0; j < Char.myCharz().taskMaint.subNames.Length; j++)
						{
							if (Char.myCharz().taskMaint.subNames[j] != null)
							{
								num2 = j;
								empty = "- " + Char.myCharz().taskMaint.subNames[j];
								if (Char.myCharz().taskMaint.counts[j] != -1)
								{
									if (Char.myCharz().taskMaint.index == j)
									{
										if (Char.myCharz().taskMaint.counts[j] != 1)
										{
											string text = empty;
											empty = text + " (" + Char.myCharz().taskMaint.count + "/" + Char.myCharz().taskMaint.counts[j] + ")";
										}
										if (Char.myCharz().taskMaint.count == Char.myCharz().taskMaint.counts[j])
										{
											mFont.tahoma_7.drawString(g, empty, xstart + 5, yPaint += 12, 0);
										}
										else
										{
											mFont tahoma_7_grey = mFont.tahoma_7_grey;
											if (!flag)
											{
												flag = true;
												tahoma_7_grey = mFont.tahoma_7_blue;
												tahoma_7_grey.drawString(g, empty, xstart + 5 + ((tahoma_7_grey == mFont.tahoma_7_blue && GameCanvas.gameTick % 20 > 10) ? (GameCanvas.gameTick % 4 / 2) : 0), yPaint += 12, 0);
											}
											else
											{
												tahoma_7_grey.drawString(g, "- ...", xstart + 5 + ((tahoma_7_grey == mFont.tahoma_7_blue && GameCanvas.gameTick % 20 > 10) ? (GameCanvas.gameTick % 4 / 2) : 0), yPaint += 12, 0);
											}
										}
									}
									else if (Char.myCharz().taskMaint.index > j)
									{
										if (Char.myCharz().taskMaint.counts[j] != 1)
										{
											string text = empty;
											empty = text + " (" + Char.myCharz().taskMaint.counts[j] + "/" + Char.myCharz().taskMaint.counts[j] + ")";
										}
										mFont.tahoma_7_white.drawString(g, empty, xstart + 5, yPaint += 12, 0);
									}
									else
									{
										if (Char.myCharz().taskMaint.counts[j] != 1)
										{
											empty = empty + " 0/" + Char.myCharz().taskMaint.counts[j];
										}
										mFont tahoma_7_grey2 = mFont.tahoma_7_grey;
										if (!flag)
										{
											flag = true;
											tahoma_7_grey2 = mFont.tahoma_7_blue;
											tahoma_7_grey2.drawString(g, empty, xstart + 5 + ((tahoma_7_grey2 == mFont.tahoma_7_blue && GameCanvas.gameTick % 20 > 10) ? (GameCanvas.gameTick % 4 / 2) : 0), yPaint += 12, 0);
										}
										else
										{
											tahoma_7_grey2.drawString(g, "- ...", xstart + 5 + ((tahoma_7_grey2 == mFont.tahoma_7_blue && GameCanvas.gameTick % 20 > 10) ? (GameCanvas.gameTick % 4 / 2) : 0), yPaint += 12, 0);
										}
									}
								}
								else if (Char.myCharz().taskMaint.index > j)
								{
									mFont.tahoma_7_white.drawString(g, empty, xstart + 5, yPaint += 12, 0);
								}
								else
								{
									mFont tahoma_7_grey3 = mFont.tahoma_7_grey;
									if (!flag)
									{
										flag = true;
										tahoma_7_grey3 = mFont.tahoma_7_blue;
										tahoma_7_grey3.drawString(g, empty, xstart + 5 + ((tahoma_7_grey3 == mFont.tahoma_7_blue && GameCanvas.gameTick % 20 > 10) ? (GameCanvas.gameTick % 4 / 2) : 0), yPaint += 12, 0);
									}
									else
									{
										tahoma_7_grey3.drawString(g, "- ...", xstart + 5 + ((tahoma_7_grey3 == mFont.tahoma_7_blue && GameCanvas.gameTick % 20 > 10) ? (GameCanvas.gameTick % 4 / 2) : 0), yPaint += 12, 0);
									}
								}
								indexRowMax++;
							}
							else if (Char.myCharz().taskMaint.index <= j)
							{
								empty = "- " + Char.myCharz().taskMaint.subNames[num2];
								mFont mFont2 = mFont.tahoma_7_grey;
								if (!flag)
								{
									flag = true;
									mFont2 = mFont.tahoma_7_blue;
								}
								mFont2.drawString(g, empty, xstart + 5 + ((mFont2 == mFont.tahoma_7_blue && GameCanvas.gameTick % 20 > 10) ? (GameCanvas.gameTick % 4 / 2) : 0), yPaint += 12, 0);
							}
						}
						yPaint += 5;
						for (int k = 0; k < Char.myCharz().taskMaint.details.Length; k++)
						{
							mFont.tahoma_7_green2.drawString(g, Char.myCharz().taskMaint.details[k], xstart + 5, yPaint += 12, 0);
							indexRowMax++;
						}
					}
					else
					{
						int taskMapId = GameScr.getTaskMapId();
						sbyte taskNpcId = GameScr.getTaskNpcId();
						string empty2 = string.Empty;
						if (taskMapId == -3 || taskNpcId == -3)
						{
							empty2 = mResources.DES_TASK[3];
						}
						else if (Char.myCharz().taskMaint == null && Char.myCharz().ctaskId == 9 && Char.myCharz().nClass.classId == 0)
						{
							empty2 = mResources.TASK_INPUT_CLASS;
						}
						else
						{
							if (taskNpcId < 0 || taskMapId < 0)
							{
								return;
							}
							empty2 = mResources.DES_TASK[0] + Npc.arrNpcTemplate[taskNpcId].name + mResources.DES_TASK[1] + TileMap.mapNames[taskMapId] + mResources.DES_TASK[2];
						}
						string[] array = mFont.tahoma_7_white.splitFontArray(empty2, 150);
						for (int l = 0; l < array.Length; l++)
						{
							if (l == 0)
							{
								mFont.tahoma_7_white.drawString(g, array[l], xstart + 5, yPaint = ystart, 0);
							}
							else
							{
								mFont.tahoma_7_white.drawString(g, array[l], xstart + 5, yPaint += 12, 0);
							}
						}
					}
				}
				else if (indexMenu == 1)
				{
					yPaint = ystart - 12;
					for (int m = 0; m < Char.myCharz().taskOrders.size(); m++)
					{
						TaskOrder taskOrder = (TaskOrder)Char.myCharz().taskOrders.elementAt(m);
						mFont.tahoma_7_white.drawString(g, taskOrder.name, xstart + 5, yPaint += 12, 0);
						if (taskOrder.count == taskOrder.maxCount)
						{
							mFont.tahoma_7_white.drawString(g, ((taskOrder.taskId != 0) ? mResources.KILLBOSS : mResources.KILL) + " " + Mob.arrMobTemplate[taskOrder.killId].name + " (" + taskOrder.count + "/" + taskOrder.maxCount + ")", xstart + 5, yPaint += 12, 0);
						}
						else
						{
							mFont.tahoma_7_blue.drawString(g, ((taskOrder.taskId != 0) ? mResources.KILLBOSS : mResources.KILL) + " " + Mob.arrMobTemplate[taskOrder.killId].name + " (" + taskOrder.count + "/" + taskOrder.maxCount + ")", xstart + 5, yPaint += 12, 0);
						}
						indexRowMax += 3;
						inforW = popupW - 25;
						paintMultiLine(g, mFont.tahoma_7_grey, taskOrder.description, xstart + 5, yPaint += 12, 0);
						yPaint += 12;
					}
				}
				if (scroll == null)
				{
					scroll = new Scroll();
					scroll.setStyle(indexRowMax, 12, xScroll, yScroll, wScroll, hScroll - num - 40, styleUPDOWN: true, 1);
				}
			}

	public void paintMultiLine(mGraphics g, mFont f, string[] arr, string str, int x, int y, int align)
			{
				for (int i = 0; i < arr.Length; i++)
				{
					string text = arr[i];
					if (text.StartsWith("c"))
					{
						if (text.StartsWith("c0"))
						{
							text = text.Substring(2);
							f = mFont.tahoma_7b_dark;
						}
						else if (text.StartsWith("c1"))
						{
							text = text.Substring(2);
							f = mFont.tahoma_7b_yellow;
						}
						else if (text.StartsWith("c2"))
						{
							text = text.Substring(2);
							f = mFont.tahoma_7b_green;
						}
					}
					if (i == 0)
					{
						f.drawString(g, text, x, y, align);
						continue;
					}
					if (i < indexRow + 30 && i > indexRow - 30)
					{
						f.drawString(g, text, x, y += 12, align);
					}
					else
					{
						y += 12;
					}
					yPaint += 12;
					indexRowMax++;
				}
			}

	public void paintMultiLine(mGraphics g, mFont f, string str, int x, int y, int align)
			{
				int num = ((!GameCanvas.isTouch || GameCanvas.w < 320) ? 10 : 20);
				string[] array = f.splitFontArray(str, inforW - num);
				for (int i = 0; i < array.Length; i++)
				{
					if (i == 0)
					{
						f.drawString(g, array[i], x, y, align);
						continue;
					}
					if (i < indexRow + 15 && i > indexRow - 15)
					{
						f.drawString(g, array[i], x, y += 12, align);
					}
					else
					{
						y += 12;
					}
					yPaint += 12;
					indexRowMax++;
				}
			}

	private void paintAccount(mGraphics g)
			{
				g.setClip(xScroll, yScroll, wScroll, hScroll);
				g.translate(0, -cmy);
				for (int i = 0; i < strAccount.Length; i++)
				{
					int x = xScroll;
					int num = yScroll + i * ITEM_HEIGHT;
					int num2 = wScroll - 1;
					int h = ITEM_HEIGHT - 1;
					if (num - cmy <= yScroll + hScroll && num - cmy >= yScroll - ITEM_HEIGHT)
					{
						g.setColor((i != selected) ? 15196114 : 16383818);
						g.fillRect(x, num, num2, h);
						mFont.tahoma_7b_dark.drawString(g, strAccount[i], xScroll + wScroll / 2, num + 6, mFont.CENTER);
					}
				}
				paintScrollArrow(g);
			}

}
