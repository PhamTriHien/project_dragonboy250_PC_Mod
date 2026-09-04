using System;
using System.Collections.Generic;
using Assets.src.g;
using UnityEngine;
public partial class Panel : IActionListener, IChatable
{
	private void doFireTool()
			{
				if (selected < 0)
				{
					return;
				}
				if (SoundMn.IsDelAcc && selected == strTool.Length - 1)
				{
					Service.gI().sendDelAcc();
					return;
				}
				if (!Char.myCharz().havePet)
				{
					switch (selected)
					{
					case 0:
						hide();
						doRada();
						break;
					case 1:
						Service.gI().openMenu(54);
						break;
					case 2:
						setTypeGameInfo();
						break;
					case 3:
						Service.gI().getFlag(0, -1);
						InfoDlg.showWait();
						break;
					case 4:
						if (Char.myCharz().statusMe == 14)
						{
							GameCanvas.startOKDlg(mResources.can_not_do_when_die);
						}
						else
						{
							Service.gI().openUIZone();
						}
						break;
					case 5:
						GameCanvas.endDlg();
						if (Char.myCharz().checkLuong() < 5)
						{
							GameCanvas.startOKDlg(mResources.not_enough_luong_world_channel);
							break;
						}
						if (chatTField == null)
						{
							chatTField = new ChatTextField();
							chatTField.tfChat.y = GameCanvas.h - 35 - ChatTextField.gI().tfChat.height;
							chatTField.initChatTextField();
							chatTField.parentScreen = GameCanvas.panel;
						}
						chatTField.strChat = mResources.world_channel_5_luong;
						chatTField.tfChat.name = mResources.CHAT;
						chatTField.to = string.Empty;
						chatTField.isShow = true;
						chatTField.tfChat.isFocus = true;
						chatTField.tfChat.setIputType(TField.INPUT_TYPE_ANY);
						if (Main.isWindowsPhone)
						{
							chatTField.tfChat.strInfo = chatTField.strChat;
						}
						if (!Main.isPC)
						{
							chatTField.startChat2(this, string.Empty);
						}
						else if (GameCanvas.isTouch)
						{
							chatTField.tfChat.doChangeToTextBox();
						}
						break;
					case 6:
						setTypeAccount();
						break;
					case 7:
						setTypeOption();
						break;
					case 8:
						GameCanvas.loginScr.backToRegister();
						break;
					case 9:
						if (GameCanvas.loginScr.isLogin2)
						{
							SoundMn.gI().backToRegister();
						}
						break;
					}
					return;
				}
				switch (selected)
				{
				case 0:
					hide();
					doRada();
					break;
				case 1:
					Service.gI().openMenu(54);
					break;
				case 2:
					setTypeGameInfo();
					break;
				case 3:
					doFirePet();
					break;
				case 4:
					Service.gI().getFlag(0, -1);
					InfoDlg.showWait();
					break;
				case 5:
					if (Char.myCharz().statusMe == 14)
					{
						GameCanvas.startOKDlg(mResources.can_not_do_when_die);
					}
					else
					{
						Service.gI().openUIZone();
					}
					break;
				case 6:
					GameCanvas.endDlg();
					if (Char.myCharz().checkLuong() < 5)
					{
						GameCanvas.startOKDlg(mResources.not_enough_luong_world_channel);
						break;
					}
					if (chatTField == null)
					{
						chatTField = new ChatTextField();
						chatTField.tfChat.y = GameCanvas.h - 35 - ChatTextField.gI().tfChat.height;
						chatTField.initChatTextField();
						chatTField.parentScreen = GameCanvas.panel;
					}
					chatTField.strChat = mResources.world_channel_5_luong;
					chatTField.tfChat.name = mResources.CHAT;
					chatTField.to = string.Empty;
					chatTField.isShow = true;
					chatTField.tfChat.isFocus = true;
					chatTField.tfChat.setIputType(TField.INPUT_TYPE_ANY);
					if (Main.isWindowsPhone)
					{
						chatTField.tfChat.strInfo = chatTField.strChat;
					}
					if (!Main.isPC)
					{
						chatTField.startChat2(this, string.Empty);
					}
					else if (GameCanvas.isTouch)
					{
						chatTField.tfChat.doChangeToTextBox();
					}
					break;
				case 7:
					setTypeAccount();
					break;
				case 8:
					setTypeOption();
					break;
				case 9:
					GameCanvas.loginScr.backToRegister();
					break;
				case 10:
					if (GameCanvas.loginScr.isLogin2)
					{
						SoundMn.gI().backToRegister();
					}
					break;
				}
			}
	private void doFireEnemy()
			{
				if (selected >= 0 && vEnemy.size() != 0)
				{
					MyVector myVector = new MyVector();
					currInfoItem = selected;
					myVector.addElement(new Command(mResources.REVENGE, this, 10000, (InfoItem)vEnemy.elementAt(currInfoItem)));
					myVector.addElement(new Command(mResources.DELETE, this, 10001, (InfoItem)vEnemy.elementAt(currInfoItem)));
					GameCanvas.menu.startAt(myVector, X, (selected + 1) * ITEM_HEIGHT - cmy + yScroll);
					addFriend((InfoItem)vEnemy.elementAt(selected));
				}
			}
	private void doFireFriend()
			{
				if (selected >= 0 && vFriend.size() != 0)
				{
					MyVector myVector = new MyVector();
					currInfoItem = selected;
					myVector.addElement(new Command(mResources.CHAT, this, 8001, (InfoItem)vFriend.elementAt(currInfoItem)));
					myVector.addElement(new Command(mResources.DELETE, this, 8002, (InfoItem)vFriend.elementAt(currInfoItem)));
					myVector.addElement(new Command(mResources.den, this, 8004, (InfoItem)vFriend.elementAt(currInfoItem)));
					GameCanvas.menu.startAt(myVector, X, (selected + 1) * ITEM_HEIGHT - cmy + yScroll);
					addFriend((InfoItem)vFriend.elementAt(selected));
				}
			}
	private void doFireChangeFlag()
			{
				if (selected >= 0)
				{
					MyVector myVector = new MyVector();
					currInfoItem = selected;
					myVector.addElement(new Command(mResources.change_flag, this, 10030, null));
					myVector.addElement(new Command(mResources.BACK, this, 10031, null));
					GameCanvas.menu.startAt(myVector, X, (selected + 1) * ITEM_HEIGHT - cmy + yScroll);
				}
			}
	private void doFireLogMessage()
			{
				if (selected == 0)
				{
					isViewChatServer = !isViewChatServer;
					Rms.saveRMSInt("viewchat", isViewChatServer ? 1 : 0);
					if (GameCanvas.isTouch)
					{
						selected = -1;
					}
				}
				else if (selected >= 0 && logChat.size() != 0)
				{
					MyVector myVector = new MyVector();
					currInfoItem = selected - 1;
					myVector.addElement(new Command(mResources.CHAT, this, 8001, (InfoItem)logChat.elementAt(currInfoItem)));
					myVector.addElement(new Command(mResources.make_friend, this, 8003, (InfoItem)logChat.elementAt(currInfoItem)));
					GameCanvas.menu.startAt(myVector, X, (selected + 1) * ITEM_HEIGHT - cmy + yScroll);
					addLogMessage((InfoItem)logChat.elementAt(selected - 1));
				}
			}
	private void doFireMain()
			{
				try
				{
					if (currentTabIndex == 0)
					{
						setTypeMap();
					}
					if (currentTabIndex == 1)
					{
						doFireInventory();
					}
					if (currentTabIndex == 2)
					{
						doFireSkill();
					}
					if (currentTabIndex == 3)
					{
						if (mainTabName.Length == 4)
						{
							doFireTool();
						}
						else
						{
							doFireClanOption();
						}
					}
					if (currentTabIndex == 4)
					{
						doFireTool();
					}
				}
				catch (Exception ex)
				{
					Res.outz("Throw ex " + ex.StackTrace);
				}
			}

}
