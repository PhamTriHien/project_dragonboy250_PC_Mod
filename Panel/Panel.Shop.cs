using System;
using System.Collections.Generic;
using Assets.src.g;
using UnityEngine;

public partial class Panel : IActionListener, IChatable
{
	public void setTypeKiGuiOnly()
		{
			type = 17;
			setType(1);
			setTabKiGui();
			typeShop = 2;
			currentTabIndex = 0;
		}

	public void setTabKiGui()
		{
			ITEM_HEIGHT = 24;
			currentListLength = Char.myCharz().arrItemShop[4].Length;
			cmyLim = currentListLength * ITEM_HEIGHT - hScroll;
			if (cmyLim < 0)
			{
				cmyLim = 0;
			}
			cmy = (cmtoY = cmyLast[currentTabIndex]);
			if (cmy < 0)
			{
				cmy = (cmtoY = 0);
			}
			if (cmy > cmyLim)
			{
				cmy = (cmtoY = cmyLim);
			}
			selected = (GameCanvas.isTouch ? (-1) : 0);
		}

	public void setTypeBodyOnly()
		{
			type = 7;
			setType(1);
			setTabInventory(resetSelect: true);
			currentTabIndex = 0;
		}

	public void setTypeLockInventory()
		{
			type = 8;
			setType(0);
			setTabMessage();
			currentTabIndex = 0;
		}

	public void setTypeShop(int typeShop)
		{
			type = 1;
			setType(0);
			setTabShop();
			currentTabIndex = 0;
			this.typeShop = typeShop;
		}

	public void setTypeBox()
		{
			type = 2;
			if (GameCanvas.w > 2 * WIDTH_PANEL)
			{
				boxTabName = new string[1][] { mResources.chestt };
			}
			else
			{
				boxTabName = new string[2][]
				{
					mResources.chestt,
					mResources.inventory
				};
			}
			tabName[2] = boxTabName;
			setType(0);
			if (currentTabIndex == 0)
			{
				setTabBox();
			}
			if (currentTabIndex == 1)
			{
				setTabInventory(resetSelect: true);
			}
			if (GameCanvas.w > 2 * WIDTH_PANEL)
			{
				GameCanvas.panel2 = new Panel();
				GameCanvas.panel2.tabName[7] = new string[1][] { new string[1] { string.Empty } };
				GameCanvas.panel2.setTypeBodyOnly();
				GameCanvas.panel2.show();
			}
		}

	public void setTypeCombine()
		{
			type = 12;
			if (GameCanvas.w > 2 * WIDTH_PANEL)
			{
				boxCombine = new string[1][] { mResources.combine };
			}
			else
			{
				boxCombine = new string[2][]
				{
					mResources.combine,
					mResources.inventory
				};
			}
			tabName[type] = boxCombine;
			setType(0);
			if (currentTabIndex == 0)
			{
				setTabCombine();
			}
			if (currentTabIndex == 1)
			{
				setTabInventory(resetSelect: true);
			}
			if (GameCanvas.w > 2 * WIDTH_PANEL)
			{
				GameCanvas.panel2 = new Panel();
				GameCanvas.panel2.tabName[7] = new string[1][] { new string[1] { string.Empty } };
				GameCanvas.panel2.setTypeBodyOnly();
				GameCanvas.panel2.show();
			}
			combineSuccess = -1;
			isDoneCombine = true;
		}

	public void setTabCombine()
		{
			currentListLength = vItemCombine.size() + 1;
			ITEM_HEIGHT = 24;
			cmyLim = currentListLength * ITEM_HEIGHT - hScroll;
			if (cmyLim < 0)
			{
				cmyLim = 9;
			}
			cmy = (cmtoY = cmyLast[currentTabIndex]);
			if (cmy < 0)
			{
				cmy = (cmtoY = 0);
			}
			if (cmy > cmyLim)
			{
				cmy = (cmtoY = cmyLim);
			}
			selected = (GameCanvas.isTouch ? (-1) : 0);
		}

	private void updateKeyCombine()
		{
			if (currentTabIndex == 0)
			{
				updateKeyScrollView();
				keyTouchCombine = -1;
				if (selected == vItemCombine.size() && GameCanvas.isPointerClick)
				{
					GameCanvas.isPointerClick = false;
					keyTouchCombine = 1;
				}
			}
			if (currentTabIndex == 1)
			{
				updateKeyScrollView();
			}
		}

	public void setTabShop()
		{
			ITEM_HEIGHT = 24;
			if (currentTabIndex == currentTabName.Length - 1 && GameCanvas.panel2 == null && typeShop != 2)
			{
				currentListLength = checkCurrentListLength(Char.myCharz().arrItemBody.Length + Char.myCharz().arrItemBag.Length);
			}
			else
			{
				currentListLength = Char.myCharz().arrItemShop[currentTabIndex].Length;
			}
			cmyLim = currentListLength * ITEM_HEIGHT - hScroll;
			if (cmyLim < 0)
			{
				cmyLim = 0;
			}
			cmy = (cmtoY = cmyLast[currentTabIndex]);
			if (cmy < 0)
			{
				cmy = (cmtoY = 0);
			}
			if (cmy > cmyLim)
			{
				cmy = (cmtoY = cmyLim);
			}
			selected = (GameCanvas.isTouch ? (-1) : 0);
		}

	private void setTabBox()
		{
			currentListLength = checkCurrentListLength(Char.myCharz().arrItemBox.Length);
			ITEM_HEIGHT = 24;
			cmyLim = currentListLength * ITEM_HEIGHT - hScroll;
			if (cmyLim < 0)
			{
				cmyLim = 9;
			}
			cmy = (cmtoY = cmyLast[currentTabIndex]);
			if (cmy < 0)
			{
				cmy = (cmtoY = 0);
			}
			if (cmy > cmyLim)
			{
				cmy = (cmtoY = cmyLim);
			}
			selected = (GameCanvas.isTouch ? (-1) : 0);
		}

	private void setTabInventory(bool resetSelect)
		{
			if (isnewInventory)
			{
				int num = Char.myCharz().arrItemBody.Length + Char.myCharz().arrItemBag.Length;
				currentListLength = checkCurrentListLength(num);
				currentListLength = 3;
				newSelected = 0;
				size_tab = (sbyte)(num / 20 + ((num % 20 > 0) ? 1 : 0));
				return;
			}
			currentListLength = checkCurrentListLength(Char.myCharz().arrItemBody.Length + Char.myCharz().arrItemBag.Length);
			ITEM_HEIGHT = 24;
			cmyLim = currentListLength * ITEM_HEIGHT - hScroll;
			cmy = (cmtoY = cmyLast[currentTabIndex]);
			if (cmyLim < 0)
			{
				cmyLim = 0;
			}
			if (cmy < 0)
			{
				cmy = (cmtoY = 0);
			}
			if (cmy > cmyLim)
			{
				cmy = (cmtoY = 0);
			}
			if (resetSelect)
			{
				selected = (GameCanvas.isTouch ? (-1) : 0);
			}
		}

	public void cleanCombine()
		{
			for (int i = 0; i < vItemCombine.size(); i++)
			{
				((Item)vItemCombine.elementAt(i)).isSelect = false;
			}
			vItemCombine.removeAllElements();
		}

	public void setCombineEff(int type)
		{
			typeCombine = type;
			rS = 90;
			if (typeCombine == 0)
			{
				iDotS = 5;
				angleS = (angleO = 90);
				time = 2;
				for (int i = 0; i < vItemCombine.size(); i++)
				{
					Item item = (Item)vItemCombine.elementAt(i);
					if (item != null)
					{
						if (item.template.type == 14)
						{
							iconID2 = item.template.iconID;
						}
						else
						{
							iconID1 = item.template.iconID;
						}
					}
				}
			}
			else if (typeCombine == 1)
			{
				iDotS = 2;
				angleS = (angleO = 0);
				time = 1;
				for (int j = 0; j < vItemCombine.size(); j++)
				{
					Item item2 = (Item)vItemCombine.elementAt(j);
					if (item2 != null)
					{
						if (j == 0)
						{
							iconID1 = item2.template.iconID;
						}
						else
						{
							iconID2 = item2.template.iconID;
						}
					}
				}
			}
			else if (typeCombine == 2)
			{
				iDotS = 7;
				angleS = (angleO = 25);
				time = 1;
				for (int k = 0; k < vItemCombine.size(); k++)
				{
					Item item3 = (Item)vItemCombine.elementAt(k);
					if (item3 != null)
					{
						iconID1 = item3.template.iconID;
					}
				}
			}
			else if (typeCombine == 3)
			{
				xS = GameCanvas.hw;
				yS = GameCanvas.hh;
				iDotS = 1;
				angleS = (angleO = 1);
				time = 4;
				for (int l = 0; l < vItemCombine.size(); l++)
				{
					Item item4 = (Item)vItemCombine.elementAt(l);
					if (item4 != null)
					{
						iconID1 = item4.template.iconID;
					}
				}
			}
			else if (typeCombine == 4)
			{
				iDotS = vItemCombine.size();
				iconID = new short[iDotS];
				angleS = (angleO = 25);
				time = 1;
				for (int m = 0; m < vItemCombine.size(); m++)
				{
					Item item5 = (Item)vItemCombine.elementAt(m);
					if (item5 != null)
					{
						iconID[m] = item5.template.iconID;
					}
				}
			}
			speed = 1;
			isSpeedCombine = true;
			isDoneCombine = false;
			isCompleteEffCombine = false;
			iAngleS = 360 / iDotS;
			xArgS = new int[iDotS];
			yArgS = new int[iDotS];
			xDotS = new int[iDotS];
			yDotS = new int[iDotS];
			setDotStar();
			isPaintCombine = true;
			countUpdate = 10;
			countR = 30;
			countWait = 10;
			addTextCombineNPC(idNPC, mResources.combineSpell);
		}

	private void updateCombineEff()
		{
			countUpdate--;
			if (countUpdate < 0)
			{
				countUpdate = 0;
			}
			countR--;
			if (countR < 0)
			{
				countR = 0;
			}
			if (countUpdate != 0)
			{
				return;
			}
			if (!isCompleteEffCombine)
			{
				if (time > 0)
				{
					if (combineSuccess != -1)
					{
						if (typeCombine == 3)
						{
							if (GameCanvas.gameTick % 10 == 0)
							{
								Effect me = new Effect(21, xS - 10, yS + 25, 4, 1, 1);
								EffecMn.addEff(me);
								time--;
							}
						}
						else
						{
							if (GameCanvas.gameTick % 2 == 0)
							{
								if (isSpeedCombine)
								{
									if (speed < 40)
									{
										speed += 2;
									}
								}
								else if (speed > 10)
								{
									speed -= 2;
								}
							}
							if (countR == 0)
							{
								if (isSpeedCombine)
								{
									if (rS > 0)
									{
										rS -= 5;
									}
									else if (GameCanvas.gameTick % 10 == 0)
									{
										isSpeedCombine = false;
										time--;
										countR = 5;
										countWait = 10;
									}
								}
								else if (rS < 90)
								{
									rS += 5;
								}
								else if (GameCanvas.gameTick % 10 == 0)
								{
									isSpeedCombine = true;
									countR = 10;
								}
							}
							angleS = angleO;
							angleS -= speed;
							if (angleS >= 360)
							{
								angleS -= 360;
							}
							if (angleS < 0)
							{
								angleS = 360 + angleS;
							}
							angleO = angleS;
							setDotStar();
						}
					}
				}
				else if (GameCanvas.gameTick % 20 == 0)
				{
					isCompleteEffCombine = true;
				}
				if (GameCanvas.gameTick % 20 == 0)
				{
					if (typeCombine != 3)
					{
						EffectPanel.addServerEffect(132, xS, yS, 2);
					}
					EffectPanel.addServerEffect(114, xS, yS + 20, 2);
				}
			}
			else
			{
				if (!isCompleteEffCombine)
				{
					return;
				}
				if (combineSuccess == 1)
				{
					if (countWait == 10)
					{
						Effect me2 = new Effect(22, xS - 3, yS + 25, 4, 1, 1);
						EffecMn.addEff(me2);
					}
					countWait--;
					if (countWait < 0)
					{
						countWait = 0;
					}
					if (rS < 300)
					{
						rS = Res.abs(rS + 10);
						if (rS == 20)
						{
							addTextCombineNPC(idNPC, mResources.combineFail);
						}
					}
					else if (GameCanvas.gameTick % 20 == 0)
					{
						if (GameCanvas.w > 2 * WIDTH_PANEL)
						{
							GameCanvas.panel2 = new Panel();
							GameCanvas.panel2.tabName[7] = new string[1][] { new string[1] { string.Empty } };
							GameCanvas.panel2.setTypeBodyOnly();
							GameCanvas.panel2.show();
						}
						combineSuccess = -1;
						isDoneCombine = true;
						if (typeCombine == 4)
						{
							GameCanvas.panel.hideNow();
						}
					}
					setDotStar();
				}
				else
				{
					if (combineSuccess != 0)
					{
						return;
					}
					if (countWait == 10)
					{
						if (typeCombine == 2)
						{
							Effect me3 = new Effect(20, xS - 3, yS + 15, 4, 2, 1);
							EffecMn.addEff(me3);
						}
						else
						{
							Effect me4 = new Effect(21, xS - 10, yS + 25, 4, 1, 1);
							EffecMn.addEff(me4);
						}
						addTextCombineNPC(idNPC, mResources.combineSuccess);
						isPaintCombine = false;
					}
					if (isPaintCombine)
					{
						return;
					}
					countWait--;
					if (countWait < -50)
					{
						countWait = -50;
						if (typeCombine < 3 && GameCanvas.w > 2 * WIDTH_PANEL)
						{
							GameCanvas.panel2 = new Panel();
							GameCanvas.panel2.tabName[7] = new string[1][] { new string[1] { string.Empty } };
							GameCanvas.panel2.setTypeBodyOnly();
							GameCanvas.panel2.show();
						}
						combineSuccess = -1;
						isDoneCombine = true;
						if (typeCombine == 4)
						{
							GameCanvas.panel.hideNow();
						}
					}
				}
			}
		}

	public void addTextCombineNPC(int idNPC, string text)
		{
			if (typeCombine >= 3)
			{
				return;
			}
			for (int i = 0; i < GameScr.vNpc.size(); i++)
			{
				Npc npc = (Npc)GameScr.vNpc.elementAt(i);
				if (npc.template.npcTemplateId == idNPC)
				{
					npc.addInfo(text);
				}
			}
		}

	public bool isTypeShop()
		{
			if (type == 1)
			{
				return true;
			}
			return false;
		}

	private static int upgradeEffectX(int dk, int tick, int wItem, int hitem, int wSize)
		{
			int num = (tick + dk) % ((wItem << 1) + (hitem << 1));
			if (0 <= num && num < wItem)
			{
				return num % wItem;
			}
			if (wItem <= num && num < wItem + hitem)
			{
				return wItem - wSize;
			}
			if (wItem + hitem <= num && num < (wItem << 1) + hitem)
			{
				return wItem - (num - hitem) % wItem - wSize;
			}
			return 0;
		}

	private static int upgradeEffectY(int dk, int tick, int wItem, int hitem, int wSize)
		{
			int num = (tick + dk) % ((wItem << 1) + (hitem << 1));
			if (0 <= num && num < wItem)
			{
				return 0;
			}
			if (wItem <= num && num < wItem + hitem)
			{
				return num % wItem;
			}
			if (wItem + hitem <= num && num < (wItem << 1) + hitem)
			{
				return hitem - wSize;
			}
			return hitem - (num - (wItem << 1)) % hitem - wSize;
		}

	public static sbyte GetColor_Item_Upgrade(int lv)
		{
			if (lv < 0)
			{
				return 0;
			}
			switch (lv)
			{
			case 0:
			case 1:
			case 2:
			case 3:
			case 4:
			case 5:
			case 6:
			case 7:
			case 8:
				return 0;
			case 9:
				return 4;
			case 10:
				return 1;
			case 11:
				return 5;
			case 12:
				return 3;
			case 13:
				return 2;
			default:
				return 6;
			}
		}

	private void updateKeyInventory()
		{
			updateKeyScrollView();
			if (selected == 0)
			{
				updateKeyInvenTab();
			}
		}

}
