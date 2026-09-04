using System;
using System.Collections.Generic;
using Assets.src.g;
using UnityEngine;

public partial class Panel : IActionListener, IChatable
{
	private void paintBox(mGraphics g)
			{
				g.setColor(16711680);
				g.setClip(xScroll, yScroll, wScroll, hScroll);
				g.translate(0, -cmy);
				try
				{
					Item[] arrItemBox = Char.myCharz().arrItemBox;
					currentListLength = checkCurrentListLength(arrItemBox.Length);
					int num = arrItemBox.Length / 20 + ((arrItemBox.Length % 20 > 0) ? 1 : 0);
					TAB_W_NEW = wScroll / num;
					for (int i = 0; i < currentListLength; i++)
					{
						int num2 = xScroll + 36;
						int num3 = yScroll + i * ITEM_HEIGHT;
						int num4 = wScroll - 36;
						int h = ITEM_HEIGHT - 1;
						int num5 = xScroll;
						int num6 = yScroll + i * ITEM_HEIGHT;
						int num7 = 34;
						int num8 = ITEM_HEIGHT - 1;
						if (num3 - cmy > yScroll + hScroll || num3 - cmy < yScroll - ITEM_HEIGHT)
						{
							continue;
						}
						if (i == 0)
						{
							for (int j = 0; j < num; j++)
							{
								int num9 = ((j == newSelected && selected == 0) ? ((GameCanvas.gameTick % 10 < 7) ? (-1) : 0) : 0);
								g.setColor((j != newSelected) ? 15723751 : 16383818);
								g.fillRect(xScroll + j * TAB_W_NEW, num3 + 9 + num9, TAB_W_NEW - 1, 14);
								mFont.tahoma_7_grey.drawString(g, string.Empty + j, xScroll + j * TAB_W_NEW + TAB_W_NEW / 2, yScroll + 11 + num9, mFont.CENTER);
							}
							continue;
						}
						g.setColor((i != selected) ? 15196114 : 16383818);
						g.fillRect(num2, num3, num4, h);
						g.setColor((i != selected) ? 9993045 : 9541120);
						int inventorySelect_body = GetInventorySelect_body(i, newSelected);
						Item item = arrItemBox[inventorySelect_body];
						if (item != null)
						{
							for (int k = 0; k < item.itemOption.Length; k++)
							{
								if (item.itemOption[k].optionTemplate.id == 72 && item.itemOption[k].param > 0)
								{
									sbyte color_Item_Upgrade = GetColor_Item_Upgrade(item.itemOption[k].param);
									int color_ItemBg = GetColor_ItemBg(color_Item_Upgrade);
									if (color_ItemBg != -1)
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
							for (int l = 0; l < item.itemOption.Length; l++)
							{
								if (item.itemOption[l].optionTemplate.id == 72)
								{
									text = " [+" + item.itemOption[l].getOptionString() + "]";
								}
								if (item.itemOption[l].optionTemplate.id == 41)
								{
									if (item.itemOption[l].param == 1)
									{
										mFont2 = GetFont(0);
									}
									else if (item.itemOption[l].param == 2)
									{
										mFont2 = GetFont(2);
									}
									else if (item.itemOption[l].param == 3)
									{
										mFont2 = GetFont(8);
									}
									else if (item.itemOption[l].param == 4)
									{
										mFont2 = GetFont(7);
									}
								}
							}
						}
						mFont2.drawString(g, item.template.name + text, num2 + 5, num3 + 1, 0);
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
								for (int m = 1; m < item.itemOption.Length; m++)
								{
									if (item.itemOption[m] != null && item.itemOption[m].optionTemplate.id != 102 && item.itemOption[m].optionTemplate.id != 107)
									{
										text2 = text2 + "," + item.itemOption[m].getOptionString();
									}
								}
							}
							mFont3.drawString(g, text2, num2 + 5, num3 + 11, mFont.LEFT);
						}
						SmallImage.drawSmallImage(g, item.template.iconID, num5 + num7 / 2, num6 + num8 / 2, 0, 3);
						if (item.itemOption != null)
						{
							for (int n = 0; n < item.itemOption.Length; n++)
							{
								paintOptItem(g, item.itemOption[n].optionTemplate.id, item.itemOption[n].param, num5, num6, num7, num8);
							}
							for (int num10 = 0; num10 < item.itemOption.Length; num10++)
							{
								paintOptSlotItem(g, item.itemOption[num10].optionTemplate.id, item.itemOption[num10].param, num5, num6, num7, num8);
							}
						}
						if (item.quantity > 1)
						{
							mFont.tahoma_7_yellow.drawString(g, string.Empty + item.quantity, num5 + num7, num6 + num8 - mFont.tahoma_7_yellow.getHeight(), 1);
						}
					}
				}
				catch (Exception)
				{
				}
				paintScrollArrow(g);
			}

	private void paintCombine(mGraphics g)
			{
				g.setColor(16711680);
				g.setClip(xScroll, yScroll, wScroll, hScroll);
				g.translate(0, -cmy);
				if (vItemCombine.size() == 0)
				{
					if (combineInfo != null)
					{
						for (int i = 0; i < combineInfo.Length; i++)
						{
							mFont.tahoma_7b_dark.drawString(g, combineInfo[i], xScroll + wScroll / 2, yScroll + hScroll / 2 - combineInfo.Length * 14 / 2 + i * 14 + 5, 2);
						}
					}
					return;
				}
				for (int j = 0; j < vItemCombine.size() + 1; j++)
				{
					int num = xScroll + 36;
					int num2 = yScroll + j * ITEM_HEIGHT;
					int num3 = wScroll - 36;
					int num4 = ITEM_HEIGHT - 1;
					int num5 = xScroll;
					int num6 = yScroll + j * ITEM_HEIGHT;
					int num7 = 34;
					int num8 = ITEM_HEIGHT - 1;
					if (num2 - cmy > yScroll + hScroll || num2 - cmy < yScroll - ITEM_HEIGHT)
					{
						continue;
					}
					if (j == vItemCombine.size())
					{
						if (vItemCombine.size() > 0)
						{
							if (!GameCanvas.isTouch && j == selected)
							{
								g.setColor(16383818);
								g.fillRect(num5, num2, wScroll, num4 + 2);
							}
							if ((j == selected && keyTouchCombine == 1) || (!GameCanvas.isTouch && j == selected))
							{
								g.drawImage(GameScr.imgLbtnFocus, xScroll + wScroll / 2, num2 + num4 / 2 + 1, StaticObj.VCENTER_HCENTER);
								mFont.tahoma_7b_green2.drawString(g, mResources.UPGRADE, xScroll + wScroll / 2, num2 + num4 / 2 - 4, mFont.CENTER);
							}
							else
							{
								g.drawImage(GameScr.imgLbtn, xScroll + wScroll / 2, num2 + num4 / 2 + 1, StaticObj.VCENTER_HCENTER);
								mFont.tahoma_7b_dark.drawString(g, mResources.UPGRADE, xScroll + wScroll / 2, num2 + num4 / 2 - 4, mFont.CENTER);
							}
						}
						continue;
					}
					g.setColor((j != selected) ? 15196114 : 16383818);
					g.fillRect(num, num2, num3, num4);
					g.setColor((j != selected) ? 9993045 : 9541120);
					Item item = (Item)vItemCombine.elementAt(j);
					if (item != null)
					{
						for (int k = 0; k < item.itemOption.Length; k++)
						{
							if (item.itemOption[k].optionTemplate.id == 72 && item.itemOption[k].param > 0)
							{
								sbyte color_Item_Upgrade = GetColor_Item_Upgrade(item.itemOption[k].param);
								int color_ItemBg = GetColor_ItemBg(color_Item_Upgrade);
								if (color_ItemBg != -1)
								{
									g.setColor((j != selected) ? GetColor_ItemBg(color_Item_Upgrade) : GetColor_ItemBg(color_Item_Upgrade));
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
						for (int l = 0; l < item.itemOption.Length; l++)
						{
							if (item.itemOption[l].optionTemplate.id == 72)
							{
								text = " [+" + item.itemOption[l].param + "]";
							}
							if (item.itemOption[l].optionTemplate.id == 41)
							{
								if (item.itemOption[l].param == 1)
								{
									mFont2 = GetFont(0);
								}
								else if (item.itemOption[l].param == 2)
								{
									mFont2 = GetFont(2);
								}
								else if (item.itemOption[l].param == 3)
								{
									mFont2 = GetFont(8);
								}
								else if (item.itemOption[l].param == 4)
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
						if (item.itemOption.Length > 0 && item.itemOption[0] != null && item.itemOption[0].optionTemplate.id != 102 && item.itemOption[0].optionTemplate.id != 107)
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
							for (int m = 1; m < item.itemOption.Length; m++)
							{
								if (item.itemOption[m] != null && item.itemOption[m].optionTemplate.id != 102 && item.itemOption[m].optionTemplate.id != 107)
								{
									text2 = text2 + "," + item.itemOption[m].getOptionString();
								}
							}
						}
						mFont3.drawString(g, text2, num + 5, num2 + 11, mFont.LEFT);
					}
					SmallImage.drawSmallImage(g, item.template.iconID, num5 + num7 / 2, num6 + num8 / 2, 0, 3);
					if (item.itemOption != null)
					{
						for (int n = 0; n < item.itemOption.Length; n++)
						{
							paintOptItem(g, item.itemOption[n].optionTemplate.id, item.itemOption[n].param, num5, num6, num7, num8);
						}
						for (int num9 = 0; num9 < item.itemOption.Length; num9++)
						{
							paintOptSlotItem(g, item.itemOption[num9].optionTemplate.id, item.itemOption[num9].param, num5, num6, num7, num8);
						}
					}
					if (item.quantity > 1)
					{
						mFont.tahoma_7_yellow.drawString(g, string.Empty + item.quantity, num5 + num7, num6 + num8 - mFont.tahoma_7_yellow.getHeight(), 1);
					}
				}
				paintScrollArrow(g);
			}

	private void paintItemBoxInfo(mGraphics g)
			{
				string st = mResources.used + ": " + hasUse + "/" + Char.myCharz().arrItemBox.Length + " " + mResources.place;
				mFont.tahoma_7b_white.drawString(g, mResources.chest, 60, 4, 0);
				mFont.tahoma_7_yellow.drawString(g, st, 60, 16, 0);
			}

	private void paintCombineInfo(mGraphics g)
			{
				if (combineTopInfo != null)
				{
					for (int i = 0; i < combineTopInfo.Length; i++)
					{
						mFont.tahoma_7_white.drawString(g, combineTopInfo[i], X + 45 + (W - 50) / 2, 5 + i * 14, mFont.CENTER);
					}
				}
			}

	public void paintCombineEff(mGraphics g)
			{
				GameScr.gI().paintBlackSky(g);
				paintCombineNPC(g);
				if (GameCanvas.gameTick % 4 == 0)
				{
					g.drawImage(ItemMap.imageFlare, xS, yS + 15, mGraphics.BOTTOM | mGraphics.HCENTER);
				}
				if (typeCombine == 0)
				{
					for (int i = 0; i < yArgS.Length; i++)
					{
						SmallImage.drawSmallImage(g, iconID1, xS, yS, 0, mGraphics.VCENTER | mGraphics.HCENTER);
						if (isPaintCombine)
						{
							SmallImage.drawSmallImage(g, iconID2, xDotS[i], yDotS[i], 0, mGraphics.VCENTER | mGraphics.HCENTER);
						}
					}
				}
				else if (typeCombine == 1)
				{
					if (!isPaintCombine)
					{
						SmallImage.drawSmallImage(g, iconID3, xS, yS, 0, mGraphics.VCENTER | mGraphics.HCENTER);
						return;
					}
					for (int j = 0; j < yArgS.Length; j++)
					{
						SmallImage.drawSmallImage(g, iconID1, xDotS[0], yDotS[0], 0, mGraphics.VCENTER | mGraphics.HCENTER);
						SmallImage.drawSmallImage(g, iconID2, xDotS[1], yDotS[1], 0, mGraphics.VCENTER | mGraphics.HCENTER);
					}
				}
				else if (typeCombine == 2)
				{
					if (!isPaintCombine)
					{
						SmallImage.drawSmallImage(g, iconID3, xS, yS, 0, mGraphics.VCENTER | mGraphics.HCENTER);
						return;
					}
					for (int k = 0; k < yArgS.Length; k++)
					{
						SmallImage.drawSmallImage(g, iconID1, xDotS[k], yDotS[k], 0, mGraphics.VCENTER | mGraphics.HCENTER);
					}
				}
				else if (typeCombine == 3)
				{
					if (!isPaintCombine)
					{
						SmallImage.drawSmallImage(g, iconID3, xS, yS, 0, mGraphics.VCENTER | mGraphics.HCENTER);
					}
					else
					{
						SmallImage.drawSmallImage(g, iconID1, xS, yS, 0, mGraphics.VCENTER | mGraphics.HCENTER);
					}
				}
				else
				{
					if (typeCombine != 4)
					{
						return;
					}
					if (!isPaintCombine)
					{
						if (iconID3 != -1)
						{
							SmallImage.drawSmallImage(g, iconID3, xS, yS, 0, mGraphics.VCENTER | mGraphics.HCENTER);
						}
					}
					else
					{
						for (int l = 0; l < iconID.Length; l++)
						{
							SmallImage.drawSmallImage(g, iconID[l], xDotS[l], yDotS[l], 0, mGraphics.VCENTER | mGraphics.HCENTER);
						}
					}
				}
			}

	public void paintCombineNPC(mGraphics g)
			{
				g.translate(-GameScr.cmx, -GameScr.cmy);
				if (typeCombine < 3)
				{
					for (int i = 0; i < GameScr.vNpc.size(); i++)
					{
						Npc npc = (Npc)GameScr.vNpc.elementAt(i);
						if (npc.template.npcTemplateId == idNPC)
						{
							npc.paint(g);
							if (npc.chatInfo != null)
							{
								npc.chatInfo.paint(g, npc.cx, npc.cy - npc.ch - GameCanvas.transY, npc.cdir);
							}
						}
					}
				}
				GameCanvas.resetTrans(g);
				if (GameCanvas.gameTick % 4 == 0)
				{
					g.drawImage(ItemMap.imageFlare, xS - 5, yS + 15, mGraphics.BOTTOM | mGraphics.HCENTER);
					g.drawImage(ItemMap.imageFlare, xS + 5, yS + 15, mGraphics.BOTTOM | mGraphics.HCENTER);
					g.drawImage(ItemMap.imageFlare, xS, yS + 15, mGraphics.BOTTOM | mGraphics.HCENTER);
				}
				for (int j = 0; j < Effect2.vEffect3.size(); j++)
				{
					Effect2 effect = (Effect2)Effect2.vEffect3.elementAt(j);
					effect.paint(g);
				}
			}

	public static void paintUpgradeEffect(int x, int y, int wItem, int hItem, int nline, int cl, mGraphics g)
			{
				try
				{
					int num = (wItem << 1) + (hItem << 1);
					int num2 = num / nline;
					nsize = sizeUpgradeEff.Length;
					if (nline > 4)
					{
						nsize = 2;
					}
					for (int i = 0; i < nline; i++)
					{
						for (int j = 0; j < nsize; j++)
						{
							int wSize = ((sizeUpgradeEff[j] <= 1) ? 1 : ((sizeUpgradeEff[j] >> 1) + 1));
							int x2 = x + upgradeEffectX(num2 * i, GameCanvas.gameTick - j * 4, wItem, hItem, wSize);
							int y2 = y + upgradeEffectY(num2 * i, GameCanvas.gameTick - j * 4, wItem, hItem, wSize);
							g.setColor(colorUpgradeEffect[cl][j]);
							g.fillRect(x2, y2, sizeUpgradeEff[j], sizeUpgradeEff[j]);
						}
					}
				}
				catch (Exception)
				{
				}
			}

}
