using System;
using System.Collections.Generic;
using Assets.src.g;
using UnityEngine;

public partial class Panel : IActionListener, IChatable
{
	public void updateKey()
		{
			if ((chatTField != null && chatTField.isShow) || !GameCanvas.panel.isDoneCombine || InfoDlg.isShow)
			{
				return;
			}
			if (tabIcon != null && tabIcon.isShow)
			{
				tabIcon.updateKey();
			}
			else
			{
				if (isClose || !isShow)
				{
					return;
				}
				if (cmdClose.isPointerPressInside())
				{
					cmdClose.performAction();
					return;
				}
				if (GameCanvas.keyPressed[13])
				{
					if (type != 4)
					{
						hide();
						return;
					}
					setTypeMain();
					cmx = (cmtoX = 0);
				}
				if (GameCanvas.keyPressed[12] || GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25])
				{
					if (left.idAction > 0)
					{
						perform(left.idAction, left.p);
					}
					else
					{
						waitToPerform = 2;
					}
				}
				if (Equals(GameCanvas.panel) && GameCanvas.panel2 == null && GameCanvas.isPointerJustRelease && !GameCanvas.isPointer(X, Y, W, H) && !pointerIsDowning)
				{
					hide();
					return;
				}
				if (!isClanOption)
				{
					updateKeyInTabBar();
				}
				switch (type)
				{
				case 23:
				case 24:
					updateKeyScrollView();
					break;
				case 21:
					if (currentTabIndex == 0)
					{
						updateKeyScrollView();
					}
					if (currentTabIndex == 1)
					{
						updateKeyPetStatus();
					}
					if (currentTabIndex == 2)
					{
						updateKeyScrollView();
					}
					break;
				case 0:
					if (currentTabIndex == 0)
					{
						updateKeyQuest();
						GameCanvas.clearKeyPressed();
						return;
					}
					if (currentTabIndex == 1)
					{
						updateKeyInventory();
					}
					if (currentTabIndex == 2)
					{
						updateKeySkill();
					}
					if (currentTabIndex == 3)
					{
						if (mainTabName.Length == 4)
						{
							updateKeyTool();
						}
						else
						{
							updateKeyClans();
						}
					}
					if (currentTabIndex == 4)
					{
						updateKeyTool();
					}
					break;
				case 2:
					updateKeyInventory();
					break;
				case 3:
					updateKeyScrollView();
					break;
				case 14:
					updateKeyScrollView();
					break;
				case 1:
				case 17:
				case 25:
					if (currentTabIndex < currentTabName.Length - ((GameCanvas.panel2 == null) ? 1 : 0) && type != 17)
					{
						updateKeyScrollView();
					}
					else if (typeShop == 0)
					{
						updateKeyInventory();
					}
					else
					{
						updateKeyScrollView();
					}
					break;
				case 4:
					updateKeyMap();
					GameCanvas.clearKeyPressed();
					return;
				case 7:
					updateKeyInventory();
					break;
				case 8:
					updateKeyScrollView();
					break;
				case 9:
					updateKeyScrollView();
					break;
				case 10:
					updateKeyScrollView();
					break;
				case 11:
				case 16:
					updateKeyScrollView();
					break;
				case 15:
					updateKeyScrollView();
					break;
				case 12:
					updateKeyCombine();
					break;
				case 13:
					updateKeyGiaoDich();
					break;
				case 18:
					updateKeyScrollView();
					break;
				case 19:
					updateKeyOption();
					break;
				case 20:
					updateKeyOption();
					break;
				case 22:
					updateKeyAuto();
					break;
				}
				GameCanvas.clearKeyHold();
				for (int i = 0; i < GameCanvas.keyPressed.Length; i++)
				{
					GameCanvas.keyPressed[i] = false;
				}
			}
		}

	private void updateKeyAuto()
		{
		}

	private void keyGiaodich()
		{
			updateKeyScrollView();
		}

	private void updateKeyGiaoDich()
		{
			if (currentTabIndex == 0)
			{
				if (Equals(GameCanvas.panel))
				{
					updateKeyInventory();
				}
				if (Equals(GameCanvas.panel2))
				{
					keyGiaodich();
				}
			}
			if (currentTabIndex == 1 || currentTabIndex == 2)
			{
				keyGiaodich();
			}
		}

	private void updateKeyTool()
		{
			updateKeyScrollView();
		}

	private void updateKeySkill()
		{
			updateKeyScrollView();
		}

	private void updateKeyQuest()
		{
			if (GameCanvas.keyHold[(!Main.isPC) ? 2 : 21])
			{
				cmyQuest -= 5;
			}
			if (GameCanvas.keyHold[(!Main.isPC) ? 8 : 22])
			{
				cmyQuest += 5;
			}
			if (cmyQuest < 0)
			{
				cmyQuest = 0;
			}
			int num = indexRowMax * 12 - (hScroll - 60);
			if (num < 0)
			{
				num = 0;
			}
			if (cmyQuest > num)
			{
				cmyQuest = num;
			}
			if (scroll != null)
			{
				if (!GameCanvas.isTouch)
				{
					scroll.cmy = cmyQuest;
				}
				scroll.updateKey();
			}
			int num2 = xScroll + wScroll / 2 - 35;
			int num3 = ((GameCanvas.h <= 300) ? 15 : 20);
			int num4 = yScroll + hScroll - num3 - 15;
			int px = GameCanvas.px;
			int py = GameCanvas.py;
			keyTouchMapButton = -1;
			if (isPaintMap && !GameScr.gI().isMapDocNhan() && px >= num2 && px <= num2 + 70 && py >= num4 && py <= num4 + 30 && (scroll == null || !scroll.pointerIsDowning))
			{
				keyTouchMapButton = 1;
				if (GameCanvas.isPointerJustRelease)
				{
					SoundMn.gI().buttonClick();
					waitToPerform = 2;
					GameCanvas.clearAllPointerEvent();
				}
			}
		}

	public void updateScroolMouse(int a)
		{
			bool flag = false;
			if (GameCanvas.pxMouse > wScroll)
			{
				return;
			}
			if (indexMouse == -1)
			{
				indexMouse = selected;
			}
			if (a > 0)
			{
				indexMouse -= a;
				flag = true;
			}
			else if (a < 0)
			{
				indexMouse += -a;
				flag = true;
			}
			if (indexMouse < 0)
			{
				indexMouse = 0;
			}
			if (flag)
			{
				cmtoY = indexMouse * 12;
				if (cmtoY > cmyLim)
				{
					cmtoY = cmyLim;
				}
				if (cmtoY < 0)
				{
					cmtoY = 0;
				}
			}
		}

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

	public void update()
		{
			if (chatTField != null && chatTField.isShow)
			{
				chatTField.update();
				return;
			}
			if (isKiguiXu)
			{
				delayKigui++;
				if (delayKigui == 10)
				{
					delayKigui = 0;
					isKiguiXu = false;
					chatTField.tfChat.setText(string.Empty);
					chatTField.strChat = mResources.kiguiXuchat + " ";
					chatTField.tfChat.name = mResources.input_money;
					chatTField.to = string.Empty;
					chatTField.isShow = true;
					chatTField.tfChat.setIputType(TField.INPUT_TYPE_NUMERIC);
					chatTField.tfChat.setMaxTextLenght(10);
					if (GameCanvas.isTouch)
					{
						chatTField.tfChat.doChangeToTextBox();
					}
					if (Main.isWindowsPhone)
					{
						chatTField.tfChat.strInfo = chatTField.strChat;
					}
					if (!Main.isPC)
					{
						chatTField.startChat2(this, string.Empty);
					}
				}
				return;
			}
			if (isKiguiLuong)
			{
				delayKigui++;
				if (delayKigui == 10)
				{
					delayKigui = 0;
					isKiguiLuong = false;
					chatTField.tfChat.setText(string.Empty);
					chatTField.strChat = mResources.kiguiLuongchat + "  ";
					chatTField.tfChat.name = mResources.input_money;
					chatTField.to = string.Empty;
					chatTField.isShow = true;
					chatTField.tfChat.setIputType(TField.INPUT_TYPE_NUMERIC);
					chatTField.tfChat.setMaxTextLenght(10);
					if (GameCanvas.isTouch)
					{
						chatTField.tfChat.doChangeToTextBox();
					}
					if (Main.isWindowsPhone)
					{
						chatTField.tfChat.strInfo = chatTField.strChat;
					}
					if (!Main.isPC)
					{
						chatTField.startChat2(this, string.Empty);
					}
				}
				return;
			}
			if (scroll != null)
			{
				scroll.updatecm();
			}
			if (tabIcon != null && tabIcon.isShow)
			{
				tabIcon.update();
				return;
			}
			moveCamera();
			if (isTabInven() && isnewInventory)
			{
				if (eBanner == null)
				{
					eBanner = new Effect(205, 0, 0, 3, 10, -1);
					eBanner.typeEff = 2;
				}
				if (eBanner != null)
				{
					eBanner.update();
				}
			}
			if (waitToPerform > 0)
			{
				waitToPerform--;
				if (waitToPerform == 0)
				{
					lastSelect[currentTabIndex] = selected;
					switch (type)
					{
					case 23:
						doFireGameInfo();
						break;
					case 21:
						doFirePetMain();
						break;
					case 0:
						doFireMain();
						break;
					case 2:
						doFireBox();
						break;
					case 3:
						doFireZone();
						break;
					case 1:
					case 17:
						doFireShop();
						break;
					case 25:
						doSpeacialSkill();
						break;
					case 4:
						doFireMap();
						break;
					case 14:
						doFireMapTrans();
						break;
					case 7:
						if (Equals(GameCanvas.panel2) && GameCanvas.panel.type == 2)
						{
							doFireBox();
							return;
						}
						doFireInventory();
						break;
					case 8:
						doFireLogMessage();
						break;
					case 9:
						doFireArchivement();
						break;
					case 10:
						doFirePlayerMenu();
						break;
					case 11:
						doFireFriend();
						break;
					case 16:
						doFireEnemy();
						break;
					case 15:
						doFireTop();
						break;
					case 12:
						doFireCombine();
						break;
					case 13:
						doFireGiaoDich();
						break;
					case 18:
						doFireChangeFlag();
						break;
					case 19:
						doFireOption();
						break;
					case 20:
						doFireAccount();
						break;
					case 22:
						doFireAuto();
						break;
					}
				}
			}
			for (int i = 0; i < ClanMessage.vMessage.size(); i++)
			{
				((ClanMessage)ClanMessage.vMessage.elementAt(i)).update();
			}
			updateCombineEff();
		}

	public void updateRequest(int recieve, int maxCap)
		{
			cp.says[cp.says.Length - 1] = mResources.received + " " + recieve + "/" + maxCap;
		}

	private void updateKeyOption()
		{
			updateKeyScrollView();
		}

	private void updateKeyInvenTab()
		{
			if (selected < 0)
			{
				return;
			}
			if (GameCanvas.keyPressed[(!Main.isPC) ? 4 : 23])
			{
				newSelected--;
				if (isnewInventory)
				{
					currentListLength = 5;
				}
				if (newSelected < 0)
				{
					newSelected = 0;
					if (GameCanvas.isFocusPanel2)
					{
						GameCanvas.isFocusPanel2 = false;
						GameCanvas.panel.selected = 0;
					}
				}
			}
			else
			{
				if (!GameCanvas.keyPressed[(!Main.isPC) ? 6 : 24])
				{
					return;
				}
				newSelected++;
				if (isnewInventory)
				{
					currentListLength = 5;
				}
				if (newSelected > size_tab - 1)
				{
					newSelected = size_tab - 1;
					if (GameCanvas.panel2 != null)
					{
						GameCanvas.isFocusPanel2 = true;
						GameCanvas.panel2.selected = 0;
					}
				}
			}
		}

}
