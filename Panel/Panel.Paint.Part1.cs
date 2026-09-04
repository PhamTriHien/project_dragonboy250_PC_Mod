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

}
