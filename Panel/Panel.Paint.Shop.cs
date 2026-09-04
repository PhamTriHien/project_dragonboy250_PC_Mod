using System;
using System.Collections.Generic;
using Assets.src.g;
using UnityEngine;

public partial class Panel : IActionListener, IChatable
{
	private void paintShop(mGraphics g)
			{
				try
				{
					if (type == 1 && currentTabIndex == currentTabName.Length - 1 && GameCanvas.panel2 == null && typeShop != 2)
					{
						paintInventory(g);
						return;
					}
					g.setColor(16711680);
					g.setClip(xScroll, yScroll, wScroll, hScroll);
					if (typeShop == 2 && Equals(GameCanvas.panel))
					{
						if (currentTabIndex <= 3 && GameCanvas.isTouch)
						{
							if (cmy < -50)
							{
								GameCanvas.paintShukiren(xScroll + wScroll / 2, yScroll + 30, g);
							}
							else if (cmy < 0)
							{
								mFont.tahoma_7_grey.drawString(g, mResources.getDown, xScroll + wScroll / 2, yScroll + 15, 2);
							}
							else if (cmyLim >= 0)
							{
								if (cmy > cmyLim + 50)
								{
									GameCanvas.paintShukiren(xScroll + wScroll / 2, yScroll + hScroll - 30, g);
								}
								else if (cmy > cmyLim)
								{
									mFont.tahoma_7_grey.drawString(g, mResources.getUp, xScroll + wScroll / 2, yScroll + hScroll - 25, 2);
								}
							}
						}
						if (Char.myCharz().arrItemShop[currentTabIndex].Length == 0 && type != 17)
						{
							mFont.tahoma_7_grey.drawString(g, mResources.notYetSell, xScroll + wScroll / 2, yScroll + hScroll / 2 - 10, 2);
							return;
						}
					}
					g.translate(0, -cmy);
					Item[] array = Char.myCharz().arrItemShop[currentTabIndex];
					if (typeShop == 2 && (currentTabIndex == 4 || type == 17))
					{
						array = Char.myCharz().arrItemShop[4];
						if (array.Length == 0)
						{
							mFont.tahoma_7_grey.drawString(g, mResources.notYetSell, xScroll + wScroll / 2, yScroll + hScroll / 2 - 10, 2);
							return;
						}
					}
					int num = array.Length;
					for (int i = 0; i < num; i++)
					{
						int num2 = xScroll + 26;
						int num3 = yScroll + i * ITEM_HEIGHT;
						int num4 = wScroll - 26;
						int h = ITEM_HEIGHT - 1;
						int num5 = xScroll;
						int num6 = yScroll + i * ITEM_HEIGHT;
						int num7 = 24;
						int num8 = ITEM_HEIGHT - 1;
						if (num3 - cmy > yScroll + hScroll || num3 - cmy < yScroll - ITEM_HEIGHT)
						{
							continue;
						}
						g.setColor((i != selected) ? 15196114 : 16383818);
						g.fillRect(num2, num3, num4, h);
						g.setColor((i != selected) ? 9993045 : 9541120);
						g.fillRect(num5, num6, num7, num8);
						Item item = array[i];
						if (item != null)
						{
							string text = string.Empty;
							mFont mFont2 = mFont.tahoma_7_green2;
							if (item.isMe != 0 && typeShop == 2 && currentTabIndex <= 3 && !Equals(GameCanvas.panel2) && item.template.name.Length < 20)
							{
								mFont2 = mFont.tahoma_7b_green;
							}
							if (item.itemOption != null)
							{
								for (int j = 0; j < item.itemOption.Length; j++)
								{
									if (item.itemOption[j].optionTemplate.id == 72)
									{
										text = " [+" + item.itemOption[j].param + "]";
									}
									if (item.itemOption[j].optionTemplate.id == 41)
									{
										if (item.itemOption[j].param == 1)
										{
											mFont2 = GetFont(0);
										}
										else if (item.itemOption[j].param == 2)
										{
											mFont2 = GetFont(2);
										}
										else if (item.itemOption[j].param == 3)
										{
											mFont2 = GetFont(8);
										}
										else if (item.itemOption[j].param == 4)
										{
											mFont2 = GetFont(7);
										}
									}
								}
							}
							mFont2.drawString(g, item.template.name + text, num2 + 5, num3 + 1, 0);
							string text2 = string.Empty;
							if (item.itemOption != null && item.itemOption.Length >= 1)
							{
								if (item.itemOption[0] != null && item.itemOption[0].optionTemplate.id != 102 && item.itemOption[0].optionTemplate.id != 107)
								{
									text2 += item.itemOption[0].getOptionString();
								}
								mFont mFont3 = mFont.tahoma_7_blue;
								if (item.compare < 0 && item.template.type != 5)
								{
									mFont3 = mFont.tahoma_7_red;
								}
								if (typeShop == 2 && item.itemOption.Length > 1 && item.buyType != -1)
								{
									text2 += string.Empty;
								}
								if (typeShop != 2 || (typeShop == 2 && item.buyType <= 1))
								{
									mFont3.drawString(g, text2, num2 + 5, num3 + 11, 0);
								}
							}
							if (item.buySpec > 0)
							{
								SmallImage.drawSmallImage(g, item.iconSpec, num2 + num4 - 7, num3 + 9, 0, 3);
								mFont.tahoma_7b_blue.drawString(g, Res.formatNumber(item.buySpec), num2 + num4 - 15, num3 + 1, mFont.RIGHT);
							}
							if (item.buyCoin != 0 || item.buyGold != 0)
							{
								if (typeShop != 2 && item.powerRequire == 0)
								{
									if (item.buyCoin > 0 && item.buyGold > 0)
									{
										if (item.buyCoin > 0)
										{
											g.drawImage(imgXu, num2 + num4 - 7, num3 + 7, 3);
											mFont.tahoma_7b_yellow.drawString(g, Res.formatNumber(item.buyCoin), num2 + num4 - 15, num3 + 1, mFont.RIGHT);
										}
										if (item.buyGold > 0)
										{
											g.drawImage(imgLuong, num2 + num4 - 7, num3 + 7 + 11, 3);
											mFont.tahoma_7b_green.drawString(g, Res.formatNumber(item.buyGold), num2 + num4 - 15, num3 + 12, mFont.RIGHT);
										}
									}
									else
									{
										if (item.buyCoin > 0)
										{
											g.drawImage(imgXu, num2 + num4 - 7, num3 + 7, 3);
											mFont.tahoma_7b_yellow.drawString(g, Res.formatNumber(item.buyCoin), num2 + num4 - 15, num3 + 1, mFont.RIGHT);
										}
										if (item.buyGold > 0)
										{
											g.drawImage(imgLuong, num2 + num4 - 7, num3 + 7, 3);
											mFont.tahoma_7b_green.drawString(g, Res.formatNumber(item.buyGold), num2 + num4 - 15, num3 + 1, mFont.RIGHT);
										}
									}
								}
								if (typeShop == 2 && currentTabIndex <= 3 && !Equals(GameCanvas.panel2))
								{
									if (item.buyCoin > 0 && item.buyGold > 0)
									{
										if (item.buyCoin > 0)
										{
											g.drawImage(imgXu, num2 + num4 - 7, num3 + 7, 3);
											mFont2 = ((Char.myCharz().xu >= item.buyCoin) ? mFont.tahoma_7b_yellow : mFont.tahoma_7b_red);
											mFont2.drawString(g, Res.formatNumber2(item.buyCoin), num2 + num4 - 15, num3 + 1, mFont.RIGHT);
										}
										if (item.buyGold > 0)
										{
											g.drawImage(imgLuong, num2 + num4 - 7, num3 + 7 + 11, 3);
											mFont2 = ((Char.myCharz().luong >= item.buyGold) ? mFont.tahoma_7b_green : mFont.tahoma_7b_red);
											mFont2.drawString(g, Res.formatNumber2(item.buyGold), num2 + num4 - 15, num3 + 12, mFont.RIGHT);
										}
									}
									else
									{
										if (item.buyCoin > 0)
										{
											g.drawImage(imgXu, num2 + num4 - 7, num3 + 7, 3);
											mFont2 = ((Char.myCharz().xu >= item.buyCoin) ? mFont.tahoma_7b_yellow : mFont.tahoma_7b_red);
											mFont2.drawString(g, Res.formatNumber2(item.buyCoin), num2 + num4 - 15, num3 + 1, mFont.RIGHT);
										}
										if (item.buyGold > 0)
										{
											g.drawImage(imgLuong, num2 + num4 - 7, num3 + 7, 3);
											mFont2 = ((Char.myCharz().luong >= item.buyGold) ? mFont.tahoma_7b_green : mFont.tahoma_7b_red);
											mFont2.drawString(g, Res.formatNumber2(item.buyGold), num2 + num4 - 15, num3 + 1, mFont.RIGHT);
										}
										try
										{
											mFont2 = mFont.tahoma_7b_green;
											if (!Char.myCharz().cName.Equals(item.nameNguoiKyGui))
											{
												mFont2 = mFont.tahoma_7b_green;
											}
											mFont2.drawString(g, item.nameNguoiKyGui, num2 + num4, num3 + 1 + mFont.tahoma_7b_red.getHeight(), mFont.RIGHT);
										}
										catch (Exception)
										{
										}
									}
								}
							}
							SmallImage.drawSmallImage(g, item.template.iconID, num5 + num7 / 2, num6 + num8 / 2, 0, 3);
							if (item.quantity > 1)
							{
								mFont.tahoma_7_yellow.drawString(g, string.Empty + item.quantity, num5 + num7, num6 + num8 - mFont.tahoma_7_yellow.getHeight(), 1);
							}
							if (item.newItem && GameCanvas.gameTick % 10 > 5)
							{
								g.drawImage(imgNew, num5 + num7 / 2, num3 + 19, 3);
							}
						}
						if (typeShop != 2 || (!Equals(GameCanvas.panel2) && currentTabIndex != 4) || item.buyType == 0)
						{
							continue;
						}
						if (item.buyType == 1)
						{
							mFont.tahoma_7_green.drawString(g, mResources.dangban, num2 + num4 - 5, num3 + 1, mFont.RIGHT);
							if (item.buyCoin != -1)
							{
								g.drawImage(imgXu, num2 + num4 - 7, num3 + 19, 3);
								mFont.tahoma_7b_yellow.drawString(g, Res.formatNumber2(item.buyCoin), num2 + num4 - 15, num3 + 13, mFont.RIGHT);
							}
							else if (item.buyGold != -1)
							{
								g.drawImage(imgLuongKhoa, num2 + num4 - 7, num3 + 17, 3);
								mFont.tahoma_7b_red.drawString(g, Res.formatNumber2(item.buyGold), num2 + num4 - 15, num3 + 11, mFont.RIGHT);
							}
						}
						else if (item.buyType == 2)
						{
							mFont.tahoma_7b_blue.drawString(g, mResources.daban, num2 + num4 - 5, num3 + 1, mFont.RIGHT);
							if (item.buyCoin != -1)
							{
								g.drawImage(imgXu, num2 + num4 - 7, num3 + 17, 3);
								mFont.tahoma_7b_yellow.drawString(g, Res.formatNumber2(item.buyCoin), num2 + num4 - 15, num3 + 11, mFont.RIGHT);
							}
							else if (item.buyGold != -1)
							{
								g.drawImage(imgLuongKhoa, num2 + num4 - 7, num3 + 17, 3);
								mFont.tahoma_7b_red.drawString(g, Res.formatNumber2(item.buyGold), num2 + num4 - 15, num3 + 11, mFont.RIGHT);
							}
						}
					}
					paintScrollArrow(g);
				}
				catch (Exception)
				{
				}
			}

	private void paintShopInfo(mGraphics g)
			{
				if (currentTabIndex == currentTabName.Length - 1 && GameCanvas.panel2 == null)
				{
					paintMyInfo(g);
				}
				else if (selected < 0)
				{
					if (typeShop != 2)
					{
						mFont.tahoma_7_white.drawString(g, mResources.say_hello, X + 60, 14, 0);
						mFont.tahoma_7_white.drawString(g, strWantToBuy, X + 60, 26, 0);
						return;
					}
					mFont.tahoma_7_white.drawString(g, mResources.say_hello, X + 60, 5, 0);
					mFont.tahoma_7_white.drawString(g, strWantToBuy, X + 60, 17, 0);
					mFont.tahoma_7_white.drawString(g, mResources.page + " " + (currPageShop[currentTabIndex] + 1) + "/" + maxPageShop[currentTabIndex], X + 60, 29, 0);
				}
				else
				{
					if (currentTabIndex < 0 || currentTabIndex > Char.myCharz().arrItemShop.Length - 1 || selected < 0 || selected > Char.myCharz().arrItemShop[currentTabIndex].Length - 1)
					{
						return;
					}
					Item item = Char.myCharz().arrItemShop[currentTabIndex][selected];
					if (item != null)
					{
						if (Equals(GameCanvas.panel) && currentTabIndex <= 3 && typeShop == 2)
						{
							mFont.tahoma_7b_white.drawString(g, mResources.page + " " + (currPageShop[currentTabIndex] + 1) + "/" + maxPageShop[currentTabIndex], X + 55, 4, 0);
						}
						mFont.tahoma_7b_white.drawString(g, item.template.name, X + 55, 24, 0);
						string st = mResources.pow_request + " " + Res.formatNumber(item.template.strRequire);
						if (item.template.strRequire > Char.myCharz().cPower)
						{
							mFont.tahoma_7_yellow.drawString(g, st, X + 55, 35, 0);
						}
						else
						{
							mFont.tahoma_7_green.drawString(g, st, X + 55, 35, 0);
						}
					}
				}
			}

}
