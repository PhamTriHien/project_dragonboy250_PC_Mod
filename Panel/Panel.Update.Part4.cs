using System;
using System.Collections.Generic;
using Assets.src.g;
using UnityEngine;
public partial class Panel : IActionListener, IChatable
{
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
