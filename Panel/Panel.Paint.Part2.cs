using System;
using System.Collections.Generic;
using Assets.src.g;
using UnityEngine;
public partial class Panel : IActionListener, IChatable
{
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

}
