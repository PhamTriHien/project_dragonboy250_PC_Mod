using System;
using System.Collections.Generic;
using Assets.src.g;
using UnityEngine;
public partial class Panel : IActionListener, IChatable
{
	private void updateKeyInTabBar()
		{
			if ((scroll != null && scroll.pointerIsDowning) || pointerIsDowning)
			{
				return;
			}
			int num = currentTabIndex;
			if (isTabInven() && isnewInventory)
			{
				if (selected == -1)
				{
					if (GameCanvas.keyPressed[6])
					{
						currentTabIndex++;
						if (currentTabIndex >= currentTabName.Length)
						{
							if (GameCanvas.panel2 != null)
							{
								currentTabIndex = currentTabName.Length - 1;
								GameCanvas.isFocusPanel2 = true;
							}
							else
							{
								currentTabIndex = 0;
							}
						}
						selected = lastSelect[currentTabIndex];
						lastTabIndex[type] = currentTabIndex;
					}
					if (GameCanvas.keyPressed[4])
					{
						currentTabIndex--;
						if (currentTabIndex < 0)
						{
							currentTabIndex = currentTabName.Length - 1;
						}
						if (GameCanvas.isFocusPanel2)
						{
							GameCanvas.isFocusPanel2 = false;
						}
						selected = lastSelect[currentTabIndex];
						lastTabIndex[type] = currentTabIndex;
					}
				}
				else if (selected > 0)
				{
					if (GameCanvas.keyPressed[8])
					{
						if (newSelected == 0)
						{
							sellectInventory++;
						}
						else
						{
							sellectInventory += 5;
						}
					}
					else if (GameCanvas.keyPressed[2])
					{
						if (newSelected == 0)
						{
							sellectInventory--;
						}
						else
						{
							sellectInventory -= 5;
						}
					}
					else if (GameCanvas.keyPressed[4])
					{
						if (newSelected == 0)
						{
							sellectInventory -= 5;
						}
						else
						{
							sellectInventory--;
						}
					}
					else if (GameCanvas.keyPressed[6])
					{
						if (newSelected == 0)
						{
							sellectInventory += 5;
						}
						else
						{
							sellectInventory++;
						}
					}
				}
				if (sellectInventory < 0)
				{
				}
				if (sellectInventory == nTableItem)
				{
					sellectInventory = 0;
				}
			}
			else if (!IsTabOption())
			{
				if (GameCanvas.keyPressed[(!Main.isPC) ? 6 : 24])
				{
					if (isTabInven())
					{
						if (selected >= 0)
						{
							updateKeyInvenTab();
						}
						else
						{
							currentTabIndex++;
							if (currentTabIndex >= currentTabName.Length)
							{
								if (GameCanvas.panel2 != null)
								{
									currentTabIndex = currentTabName.Length - 1;
									GameCanvas.isFocusPanel2 = true;
								}
								else
								{
									currentTabIndex = 0;
								}
							}
							selected = lastSelect[currentTabIndex];
							lastTabIndex[type] = currentTabIndex;
						}
					}
					else
					{
						currentTabIndex++;
						if (currentTabIndex >= currentTabName.Length)
						{
							if (GameCanvas.panel2 != null)
							{
								currentTabIndex = currentTabName.Length - 1;
								GameCanvas.isFocusPanel2 = true;
							}
							else
							{
								currentTabIndex = 0;
							}
						}
						selected = lastSelect[currentTabIndex];
						lastTabIndex[type] = currentTabIndex;
					}
				}
				if (GameCanvas.keyPressed[(!Main.isPC) ? 4 : 23])
				{
					currentTabIndex--;
					if (currentTabIndex < 0)
					{
						currentTabIndex = currentTabName.Length - 1;
					}
					if (GameCanvas.isFocusPanel2)
					{
						GameCanvas.isFocusPanel2 = false;
					}
					selected = lastSelect[currentTabIndex];
					lastTabIndex[type] = currentTabIndex;
				}
			}
			keyTouchTab = -1;
			for (int i = 0; i < currentTabName.Length; i++)
			{
				if (!GameCanvas.isPointer(startTabPos + i * TAB_W, 52, TAB_W - 1, 25))
				{
					continue;
				}
				keyTouchTab = i;
				if (GameCanvas.isPointerJustRelease)
				{
					currentTabIndex = i;
					lastTabIndex[type] = i;
					GameCanvas.isPointerJustRelease = false;
					selected = lastSelect[currentTabIndex];
					if (num == currentTabIndex && cmRun == 0)
					{
						cmtoY = 0;
						selected = (GameCanvas.isTouch ? (-1) : 0);
					}
					break;
				}
			}
			if (num == currentTabIndex)
			{
				return;
			}
			size_tab = 0;
			SoundMn.gI().panelClick();
			switch (type)
			{
			case 21:
				if (currentTabIndex == 0)
				{
					setTabPetInventory();
				}
				if (currentTabIndex == 1)
				{
					setTabPetStatus();
				}
				if (currentTabIndex == 2)
				{
					setTabInventory(resetSelect: true);
				}
				break;
			case 0:
				if (currentTabIndex == 0)
				{
					setTabTask();
				}
				if (currentTabIndex == 1)
				{
					setTabInventory(resetSelect: true);
				}
				if (currentTabIndex == 2)
				{
					setTabSkill();
				}
				if (currentTabIndex == 3)
				{
					if (mainTabName.Length > 4)
					{
						setTabClans();
					}
					else
					{
						setTabTool();
					}
				}
				if (currentTabIndex == 4)
				{
					setTabTool();
				}
				break;
			case 2:
				if (currentTabIndex == 0)
				{
					setTabBox();
				}
				if (currentTabIndex == 1)
				{
					setTabInventory(resetSelect: true);
				}
				break;
			case 3:
				setTabZone();
				break;
			case 1:
				setTabShop();
				break;
			case 25:
				setTabSpeacialSkill();
				break;
			case 12:
				if (currentTabIndex == 0)
				{
					setTabCombine();
				}
				if (currentTabIndex == 1)
				{
					setTabInventory(resetSelect: true);
				}
				break;
			case 13:
				if (currentTabIndex == 0)
				{
					if (Equals(GameCanvas.panel))
					{
						setTabInventory(resetSelect: true);
					}
					else if (Equals(GameCanvas.panel2))
					{
						setTabGiaoDich(isMe: false);
					}
				}
				if (currentTabIndex == 1)
				{
					setTabGiaoDich(isMe: true);
				}
				if (currentTabIndex == 2)
				{
					setTabGiaoDich(isMe: false);
				}
				break;
			}
			selected = lastSelect[currentTabIndex];
		}

}
