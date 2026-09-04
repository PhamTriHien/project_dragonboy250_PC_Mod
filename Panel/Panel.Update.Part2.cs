using System;
using System.Collections.Generic;
using Assets.src.g;
using UnityEngine;
public partial class Panel : IActionListener, IChatable
{
	private void updateKeyScrollView()
		{
			if (currentListLength <= 0)
			{
				return;
			}
			bool flag = false;
			if (GameCanvas.keyPressed[(!Main.isPC) ? 2 : 21])
			{
				flag = true;
				if (isTabInven() && isnewInventory)
				{
					if (selected > 0 && sellectInventory == 0)
					{
						selected--;
					}
				}
				else
				{
					selected--;
					if (type == 24)
					{
						selected -= 2;
						if (selected < 0)
						{
							selected = 0;
						}
					}
					else if (selected < 0)
					{
						if (Equals(GameCanvas.panel) && typeShop == 2 && currentTabIndex <= 3 && maxPageShop[currentTabIndex] > 1)
						{
							InfoDlg.showWait();
							if (currPageShop[currentTabIndex] <= 0)
							{
								Service.gI().kigui(4, -1, (sbyte)currentTabIndex, maxPageShop[currentTabIndex] - 1, -1);
							}
							else
							{
								Service.gI().kigui(4, -1, (sbyte)currentTabIndex, currPageShop[currentTabIndex] - 1, -1);
							}
							return;
						}
						selected = currentListLength - 1;
						if (isClanOption)
						{
							selected = -1;
						}
						if (size_tab > 0)
						{
							selected = -1;
						}
					}
					lastSelect[currentTabIndex] = selected;
					cSelected = 0;
					getCurrClanOtion();
				}
			}
			else if (GameCanvas.keyPressed[(!Main.isPC) ? 8 : 22])
			{
				flag = true;
				if (isTabInven() && isnewInventory)
				{
					if (selected < 1 && sellectInventory == 0)
					{
						selected++;
					}
				}
				else
				{
					selected++;
					if (type == 24)
					{
						selected += 2;
						if (selected > currentListLength - 1)
						{
							selected = currentListLength - 1;
						}
					}
					else if (selected > currentListLength - 1)
					{
						if (Equals(GameCanvas.panel) && typeShop == 2 && currentTabIndex <= 3 && maxPageShop[currentTabIndex] > 1)
						{
							InfoDlg.showWait();
							if (currPageShop[currentTabIndex] >= maxPageShop[currentTabIndex] - 1)
							{
								Service.gI().kigui(4, -1, (sbyte)currentTabIndex, 0, -1);
							}
							else
							{
								Service.gI().kigui(4, -1, (sbyte)currentTabIndex, currPageShop[currentTabIndex] + 1, -1);
							}
							return;
						}
						selected = 0;
					}
					lastSelect[currentTabIndex] = selected;
					cSelected = 0;
					getCurrClanOtion();
				}
			}
			if (isnewInventory && GameCanvas.keyPressed[5] && itemInvenNew != null)
			{
				pointerDownTime = 0;
				waitToPerform = 2;
			}
			if (flag)
			{
				cmtoY = selected * ITEM_HEIGHT - hScroll / 2;
				if (cmtoY > cmyLim)
				{
					cmtoY = cmyLim;
				}
				if (cmtoY < 0)
				{
					cmtoY = 0;
				}
				cmy = cmtoY;
			}
			if (GameCanvas.isPointerDown)
			{
				justRelease = false;
				if (!pointerIsDowning && GameCanvas.isPointer(xScroll, yScroll, wScroll, hScroll))
				{
					for (int i = 0; i < pointerDownLastX.Length; i++)
					{
						pointerDownLastX[0] = GameCanvas.py;
					}
					pointerDownFirstX = GameCanvas.py;
					pointerIsDowning = true;
					isDownWhenRunning = cmRun != 0;
					cmRun = 0;
				}
				else if (pointerIsDowning)
				{
					pointerDownTime++;
					if (pointerDownTime > 5 && pointerDownFirstX == GameCanvas.py && !isDownWhenRunning)
					{
						pointerDownFirstX = -1000;
						selected = (cmtoY + GameCanvas.py - yScroll) / ITEM_HEIGHT;
						if (selected >= currentListLength)
						{
							selected = -1;
						}
						checkOptionSelect();
					}
					else
					{
						indexMouse = -1;
					}
					int num = GameCanvas.py - pointerDownLastX[0];
					if (num != 0 && selected != -1)
					{
						selected = -1;
						cSelected = -1;
					}
					for (int num2 = pointerDownLastX.Length - 1; num2 > 0; num2--)
					{
						pointerDownLastX[num2] = pointerDownLastX[num2 - 1];
					}
					pointerDownLastX[0] = GameCanvas.py;
					cmtoY -= num;
					if (cmtoY < 0)
					{
						cmtoY = 0;
					}
					if (cmtoY > cmyLim)
					{
						cmtoY = cmyLim;
					}
					if (cmy < 0 || cmy > cmyLim)
					{
						num /= 2;
					}
					cmy -= num;
					if (cmy < -(GameCanvas.h / 3))
					{
						wantUpdateList = true;
					}
					else
					{
						wantUpdateList = false;
					}
					if (isnewInventory)
					{
						int num3 = GameCanvas.px - xScroll;
						int num4 = GameCanvas.py - yScroll;
						sellectInventory = num4 / 34 * 5 + num3 / 34;
					}
				}
			}
			if (!GameCanvas.isPointerJustRelease || !pointerIsDowning)
			{
				return;
			}
			justRelease = true;
			int i2 = GameCanvas.py - pointerDownLastX[0];
			GameCanvas.isPointerJustRelease = false;
			if (Res.abs(i2) < 20 && Res.abs(GameCanvas.py - pointerDownFirstX) < 20 && !isDownWhenRunning)
			{
				cmRun = 0;
				cmtoY = cmy;
				pointerDownFirstX = -1000;
				selected = (cmtoY + GameCanvas.py - yScroll) / ITEM_HEIGHT;
				if (selected >= currentListLength)
				{
					selected = -1;
				}
				checkOptionSelect();
				pointerDownTime = 0;
				waitToPerform = 10;
				if (isnewInventory)
				{
					waitToPerform = -1;
				}
				SoundMn.gI().panelClick();
			}
			else if (selected != -1 && pointerDownTime > 5)
			{
				pointerDownTime = 0;
				waitToPerform = 1;
			}
			else if (selected == -1 && !isDownWhenRunning)
			{
				if (cmy < 0)
				{
					cmtoY = 0;
				}
				else if (cmy > cmyLim)
				{
					cmtoY = cmyLim;
				}
				else
				{
					int num5 = GameCanvas.py - pointerDownLastX[0] + (pointerDownLastX[0] - pointerDownLastX[1]) + (pointerDownLastX[1] - pointerDownLastX[2]);
					num5 = ((num5 > 10) ? 10 : ((num5 < -10) ? (-10) : 0));
					cmRun = -num5 * 100;
				}
			}
			int num6 = 0;
			if ((isTabInven() || type == 13) && GameCanvas.py < yScroll + 21)
			{
				selected = 0;
				updateKeyInvenTab();
			}
			pointerIsDowning = false;
			pointerDownTime = 0;
			GameCanvas.isPointerJustRelease = false;
		}

}
