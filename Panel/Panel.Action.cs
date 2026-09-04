using System;
using System.Collections.Generic;
using Assets.src.g;
using UnityEngine;

public partial class Panel : IActionListener, IChatable
{
	private void checkOptionSelect()
			{
				try
				{
					if (type != 0 || currentTabIndex != 3 || mainTabName.Length != 5 || selected == -1)
					{
						return;
					}
					int num = 0;
					if (selected == 0)
					{
						num = xScroll + wScroll / 2 - clansOption.Length * TAB_W / 2;
						cSelected = (GameCanvas.px - num) / TAB_W;
					}
					else
					{
						currMess = getCurrMessage();
						if (currMess != null && currMess.option != null)
						{
							num = xScroll + wScroll - 2 - currMess.option.Length * 40;
							cSelected = (GameCanvas.px - num) / 40;
						}
					}
					if (GameCanvas.px < num)
					{
						cSelected = -1;
					}
				}
				catch (Exception ex)
				{
					Res.outz("Throw err " + ex.StackTrace);
				}
			}

	private void doFireGameInfo()
			{
				if (selected != -1)
				{
					infoSelect = selected;
					((GameInfo)vGameInfo.elementAt(infoSelect)).hasRead = true;
					Rms.saveRMSInt(((GameInfo)vGameInfo.elementAt(infoSelect)).id + string.Empty, 1);
					setTypeGameSubInfo();
				}
			}

	private void doFireAuto()
			{
			}

	private void doFireTop()
			{
				if (selected >= -1)
				{
					if (isThachDau)
					{
						Service.gI().sendTop(topName, (sbyte)selected);
						return;
					}
					MyVector myVector = new MyVector(string.Empty);
					myVector.addElement(new Command(mResources.CHAR_ORDER[0], this, 9999, (TopInfo)vTop.elementAt(selected)));
					GameCanvas.menu.startAt(myVector, X, (selected + 1) * ITEM_HEIGHT - cmy + yScroll);
					addThachDauDetail((TopInfo)vTop.elementAt(selected));
				}
			}

	private void doFireGiaoDich()
			{
				if (currentTabIndex == 0 && Equals(GameCanvas.panel))
				{
					doFireInventory();
					return;
				}
				if ((currentTabIndex == 0 && Equals(GameCanvas.panel2)) || currentTabIndex == 2)
				{
					if (Equals(GameCanvas.panel2))
					{
						currItem = (Item)GameCanvas.panel2.vFriendGD.elementAt(selected);
					}
					else
					{
						currItem = (Item)GameCanvas.panel.vFriendGD.elementAt(selected);
					}
					Res.outz2("toi day select= " + selected);
					MyVector myVector = new MyVector();
					myVector.addElement(new Command(mResources.CLOSE, this, 8000, currItem));
					if (currItem != null)
					{
						GameCanvas.menu.startAt(myVector, X, (selected + 1) * ITEM_HEIGHT - cmy + yScroll);
						addItemDetail(currItem);
					}
					else
					{
						cp = null;
					}
				}
				if (currentTabIndex == 1)
				{
					if (selected == currentListLength - 3)
					{
						if (isLock)
						{
							return;
						}
						putMoney();
					}
					else if (selected == currentListLength - 2)
					{
						if (!isAccept)
						{
							isLock = !isLock;
							if (isLock)
							{
								Service.gI().giaodich(5, -1, -1, -1);
							}
							else
							{
								hide();
								InfoDlg.showWait();
								Service.gI().giaodich(3, -1, -1, -1);
							}
						}
						else
						{
							isAccept = false;
						}
					}
					else if (selected == currentListLength - 1)
					{
						if (isLock && !isAccept && isFriendLock)
						{
							GameCanvas.startYesNoDlg(mResources.do_u_sure_to_trade, new Command(mResources.YES, this, 7002, null), new Command(mResources.NO, this, 4005, null));
						}
					}
					else
					{
						if (isLock)
						{
							return;
						}
						currItem = (Item)GameCanvas.panel.vMyGD.elementAt(selected);
						MyVector myVector2 = new MyVector();
						myVector2.addElement(new Command(mResources.CLOSE, this, 8000, currItem));
						if (currItem != null)
						{
							GameCanvas.menu.startAt(myVector2, X, (selected + 1) * ITEM_HEIGHT - cmy + yScroll);
							addItemDetail(currItem);
						}
						else
						{
							cp = null;
						}
					}
				}
				if (GameCanvas.isTouch)
				{
					selected = -1;
				}
			}

	private void doFirePlayerMenu()
			{
				if (selected != -1)
				{
					isSelectPlayerMenu = true;
					hide();
				}
			}

	private void doFireArchivement()
			{
				if (selected >= 0 && Char.myCharz().arrArchive[selected].isFinish && !Char.myCharz().arrArchive[selected].isRecieve)
				{
					if (!GameCanvas.isTouch)
					{
						Service.gI().getArchivemnt(selected);
					}
					else if (GameCanvas.px > xScroll + wScroll - 40)
					{
						Service.gI().getArchivemnt(selected);
					}
				}
			}

	private void doFireInventory()
			{
				Res.outz("fire inventory");
				if (Char.myCharz().statusMe == 14)
				{
					GameCanvas.startOKDlg(mResources.can_not_do_when_die);
				}
				else
				{
					if (selected == -1)
					{
						return;
					}
					if (selected == 0)
					{
						setNewSelected(Char.myCharz().arrItemBody.Length + Char.myCharz().arrItemBag.Length, resetSelect: false, isTabBox: false);
						return;
					}
					currItem = null;
					MyVector myVector = new MyVector();
					if (isnewInventory)
					{
						currItem = itemInvenNew;
						if (newSelected == 0)
						{
							myVector.addElement(new Command(mResources.GETOUT, this, 2002, currItem));
						}
						else if (GameCanvas.panel.type == 12)
						{
							myVector.addElement(new Command(mResources.use_for_combine, this, 6000, currItem));
						}
						else if (GameCanvas.panel.type == 13)
						{
							myVector.addElement(new Command(mResources.use_for_trade, this, 7000, currItem));
						}
						else if (currItem.isTypeBody())
						{
							myVector.addElement(new Command(mResources.USE, this, 2000, currItem));
							if (Char.myCharz().havePet)
							{
								myVector.addElement(new Command(mResources.MOVEFORPET, this, 2005, currItem));
							}
						}
						else
						{
							myVector.addElement(new Command(mResources.USE, this, 2001, currItem));
						}
					}
					else if (!GetInventorySelect_isbody(selected, newSelected, Char.myCharz().arrItemBody))
					{
						Item item = Char.myCharz().arrItemBag[GetInventorySelect_bag(selected, newSelected, Char.myCharz().arrItemBody)];
						if (item != null)
						{
							currItem = item;
							if (GameCanvas.panel.type == 12)
							{
								myVector.addElement(new Command(mResources.use_for_combine, this, 6000, currItem));
							}
							else if (GameCanvas.panel.type == 13)
							{
								myVector.addElement(new Command(mResources.use_for_trade, this, 7000, currItem));
							}
							else if (item.isTypeBody())
							{
								myVector.addElement(new Command(mResources.USE, this, 2000, currItem));
								if (Char.myCharz().havePet)
								{
									myVector.addElement(new Command(mResources.MOVEFORPET, this, 2005, currItem));
								}
							}
							else
							{
								myVector.addElement(new Command(mResources.USE, this, 2001, currItem));
							}
						}
					}
					else
					{
						Item item2 = Char.myCharz().arrItemBody[GetInventorySelect_body(selected, newSelected)];
						if (item2 != null)
						{
							currItem = item2;
							myVector.addElement(new Command(mResources.GETOUT, this, 2002, currItem));
						}
					}
					if (currItem != null)
					{
						Char.myCharz().setPartTemp(currItem.headTemp, currItem.bodyTemp, currItem.legTemp, currItem.bagTemp);
						if (GameCanvas.panel.type != 12 && GameCanvas.panel.type != 13)
						{
							if (position == 0)
							{
								myVector.addElement(new Command(mResources.MOVEOUT, this, 2003, currItem));
							}
							if (position == 1)
							{
								myVector.addElement(new Command(mResources.SALE, this, 3002, currItem));
							}
						}
						GameCanvas.menu.startAt(myVector, X, (selected + 1) * ITEM_HEIGHT - cmy + yScroll);
						addItemDetail(currItem);
					}
					else
					{
						cp = null;
					}
				}
			}

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

	private void doFireSkill()
			{
				if (selected < 0)
				{
					return;
				}
				if (Char.myCharz().statusMe == 14)
				{
					GameCanvas.startOKDlg(mResources.can_not_do_when_die);
					return;
				}
				if (selected == 0 || selected == 1 || selected == 2 || selected == 3 || selected == 4 || selected == 5)
				{
					long cTiemNang = Char.myCharz().cTiemNang;
					int cHPGoc = Char.myCharz().cHPGoc;
					int cMPGoc = Char.myCharz().cMPGoc;
					int cDamGoc = Char.myCharz().cDamGoc;
					int cDefGoc = Char.myCharz().cDefGoc;
					int cCriticalGoc = Char.myCharz().cCriticalGoc;
					int num = 0;
					int num2 = 1000;
					if (selected == 0)
					{
						if (cTiemNang < Char.myCharz().cHPGoc + num2)
						{
							GameCanvas.startOKDlg(mResources.not_enough_potential_point1 + Char.myCharz().cTiemNang + mResources.not_enough_potential_point2 + (Char.myCharz().cHPGoc + num2), isError: false);
							return;
						}
						if (cTiemNang > cHPGoc && cTiemNang < 10 * (2 * (cHPGoc + num2) + 180) / 2)
						{
							GameCanvas.startYesNoDlg(mResources.use_potential_point_for1 + (cHPGoc + num2) + mResources.use_potential_point_for2 + Char.myCharz().hpFrom1000TiemNang + mResources.for_HP, new Command(mResources.increase_upper, this, 9000, null), new Command(mResources.CANCEL, this, 4007, null));
							return;
						}
						if (cTiemNang >= 10 * (2 * (cHPGoc + num2) + 180) / 2 && cTiemNang < 100 * (2 * (cHPGoc + num2) + 1980) / 2)
						{
							MyVector myVector = new MyVector(string.Empty);
							myVector.addElement(new Command(mResources.increase_upper + "\n" + Char.myCharz().hpFrom1000TiemNang + mResources.HP + "\n-" + Res.formatNumber2(cHPGoc + num2), this, 9000, null));
							myVector.addElement(new Command(mResources.increase_upper + "\n" + 10 * Char.myCharz().hpFrom1000TiemNang + mResources.HP + "\n-" + Res.formatNumber2(10 * (2 * (cHPGoc + num2) + 180) / 2), this, 9006, null));
							GameCanvas.menu.startAt(myVector, X, (selected + 1) * ITEM_HEIGHT - cmy + yScroll);
							addSkillDetail2(selected);
						}
						if (cTiemNang >= 100 * (2 * (cHPGoc + num2) + 1980) / 2)
						{
							MyVector myVector2 = new MyVector(string.Empty);
							myVector2.addElement(new Command(mResources.increase_upper + "\n" + Char.myCharz().hpFrom1000TiemNang + mResources.HP + "\n-" + Res.formatNumber2(cHPGoc + num2), this, 9000, null));
							myVector2.addElement(new Command(mResources.increase_upper + "\n" + 10 * Char.myCharz().hpFrom1000TiemNang + mResources.HP + "\n-" + Res.formatNumber2(10 * (2 * (cHPGoc + num2) + 180) / 2), this, 9006, null));
							myVector2.addElement(new Command(mResources.increase_upper + "\n" + 100 * Char.myCharz().hpFrom1000TiemNang + mResources.HP + "\n-" + Res.formatNumber2(100 * (2 * (cHPGoc + num2) + 1980) / 2), this, 9007, null));
							GameCanvas.menu.startAt(myVector2, X, (selected + 1) * ITEM_HEIGHT - cmy + yScroll);
							addSkillDetail2(selected);
						}
					}
					if (selected == 1)
					{
						if (Char.myCharz().cTiemNang < Char.myCharz().cMPGoc + num2)
						{
							GameCanvas.startOKDlg(mResources.not_enough_potential_point1 + Char.myCharz().cTiemNang + mResources.not_enough_potential_point2 + (Char.myCharz().cMPGoc + num2));
							return;
						}
						if (cTiemNang > cMPGoc && cTiemNang < 10 * (2 * (cMPGoc + num2) + 180) / 2)
						{
							GameCanvas.startYesNoDlg(mResources.use_potential_point_for1 + (cMPGoc + num2) + mResources.use_potential_point_for2 + Char.myCharz().mpFrom1000TiemNang + mResources.for_KI, new Command(mResources.increase_upper, this, 9000, null), new Command(mResources.CANCEL, this, 4007, null));
							return;
						}
						if (cTiemNang >= 10 * (2 * (cMPGoc + num2) + 180) / 2 && cTiemNang < 100 * (2 * (cMPGoc + num2) + 1980) / 2)
						{
							MyVector myVector3 = new MyVector(string.Empty);
							myVector3.addElement(new Command(mResources.increase_upper + "\n" + Char.myCharz().mpFrom1000TiemNang + mResources.KI + "\n-" + Res.formatNumber2(cHPGoc + num2), this, 9000, null));
							myVector3.addElement(new Command(mResources.increase_upper + "\n" + 10 * Char.myCharz().mpFrom1000TiemNang + mResources.KI + "\n-" + Res.formatNumber2(10 * (2 * (cHPGoc + num2) + 180) / 2), this, 9006, null));
							GameCanvas.menu.startAt(myVector3, X, (selected + 1) * ITEM_HEIGHT - cmy + yScroll);
							addSkillDetail2(selected);
						}
						if (cTiemNang >= 100 * (2 * (cMPGoc + num2) + 1980) / 2)
						{
							MyVector myVector4 = new MyVector(string.Empty);
							myVector4.addElement(new Command(mResources.increase_upper + "\n" + Char.myCharz().mpFrom1000TiemNang + mResources.KI + "\n-" + Res.formatNumber2(cMPGoc + num2), this, 9000, null));
							myVector4.addElement(new Command(mResources.increase_upper + "\n" + 10 * Char.myCharz().mpFrom1000TiemNang + mResources.KI + "\n-" + Res.formatNumber2(10 * (2 * (cMPGoc + num2) + 180) / 2), this, 9006, null));
							myVector4.addElement(new Command(mResources.increase_upper + "\n" + 100 * Char.myCharz().mpFrom1000TiemNang + mResources.KI + "\n-" + Res.formatNumber2(100 * (2 * (cMPGoc + num2) + 1980) / 2), this, 9007, null));
							GameCanvas.menu.startAt(myVector4, X, (selected + 1) * ITEM_HEIGHT - cmy + yScroll);
							addSkillDetail2(selected);
						}
					}
					if (selected == 2)
					{
						if (Char.myCharz().cTiemNang < Char.myCharz().cDamGoc * Char.myCharz().expForOneAdd)
						{
							GameCanvas.startOKDlg(mResources.not_enough_potential_point1 + Char.myCharz().cTiemNang + mResources.not_enough_potential_point2 + cDamGoc * 100);
							return;
						}
						if (cTiemNang > cDamGoc && cTiemNang < 10 * (2 * cDamGoc + 9) / 2 * Char.myCharz().expForOneAdd)
						{
							GameCanvas.startYesNoDlg(mResources.use_potential_point_for1 + cDamGoc * 100 + mResources.use_potential_point_for2 + Char.myCharz().damFrom1000TiemNang + mResources.for_hit_point, new Command(mResources.increase_upper, this, 9000, null), new Command(mResources.CANCEL, this, 4007, null));
							return;
						}
						if (cTiemNang >= 10 * (2 * cDamGoc + 9) / 2 * Char.myCharz().expForOneAdd && cTiemNang < 100 * (2 * cDamGoc + 99) / 2 * Char.myCharz().expForOneAdd)
						{
							MyVector myVector5 = new MyVector(string.Empty);
							myVector5.addElement(new Command(mResources.increase_upper + "\n" + Char.myCharz().damFrom1000TiemNang + "\n" + mResources.hit_point + "\n-" + Res.formatNumber2(cDamGoc * 100), this, 9000, null));
							myVector5.addElement(new Command(mResources.increase_upper + "\n" + 10 * Char.myCharz().damFrom1000TiemNang + "\n" + mResources.hit_point + "\n-" + Res.formatNumber2(10 * (2 * cDamGoc + 9) / 2 * Char.myCharz().expForOneAdd), this, 9006, null));
							GameCanvas.menu.startAt(myVector5, X, (selected + 1) * ITEM_HEIGHT - cmy + yScroll);
							addSkillDetail2(selected);
						}
						if (cTiemNang >= 100 * (2 * cDamGoc + 99) / 2 * Char.myCharz().expForOneAdd)
						{
							MyVector myVector6 = new MyVector(string.Empty);
							myVector6.addElement(new Command(mResources.increase_upper + "\n" + Char.myCharz().damFrom1000TiemNang + "\n" + mResources.hit_point + "\n-" + Res.formatNumber2(cDamGoc * 100), this, 9000, null));
							myVector6.addElement(new Command(mResources.increase_upper + "\n" + 10 * Char.myCharz().damFrom1000TiemNang + "\n" + mResources.hit_point + "\n-" + Res.formatNumber2(10 * (2 * cDamGoc + 9) / 2 * Char.myCharz().expForOneAdd), this, 9006, null));
							myVector6.addElement(new Command(mResources.increase_upper + "\n" + 100 * Char.myCharz().damFrom1000TiemNang + "\n" + mResources.hit_point + "\n-" + Res.formatNumber2(100 * (2 * cDamGoc + 99) / 2 * Char.myCharz().expForOneAdd), this, 9007, null));
							GameCanvas.menu.startAt(myVector6, X, (selected + 1) * ITEM_HEIGHT - cmy + yScroll);
							addSkillDetail2(selected);
						}
					}
					if (selected == 3)
					{
						if (Char.myCharz().cTiemNang < 50000 + Char.myCharz().cDefGoc * 1000)
						{
							GameCanvas.startOKDlg(mResources.not_enough_potential_point1 + NinjaUtil.getMoneys(Char.myCharz().cTiemNang) + mResources.not_enough_potential_point2 + NinjaUtil.getMoneys(50000 + Char.myCharz().cDefGoc * 1000));
							return;
						}
						long number = (long)(2 * (cDefGoc + 5)) / 2L * 100000;
						long number2 = 10L * (long)(2 * (cDefGoc + 5) + 9) / 2 * 100000;
						long number3 = 100L * (long)(2 * (cDefGoc + 5) + 99) / 2 * 100000;
						mResources.use_potential_point_for1 = mResources.increase_upper;
						MyVector myVector7 = new MyVector(string.Empty);
						myVector7.addElement(new Command(mResources.use_potential_point_for1 + "\n1 " + mResources.armor + "\n" + Res.formatNumber2(number), this, 9000, null));
						myVector7.addElement(new Command(mResources.use_potential_point_for1 + "\n10 " + mResources.armor + "\n" + Res.formatNumber2(number2), this, 9006, null));
						myVector7.addElement(new Command(mResources.use_potential_point_for1 + "\n100 " + mResources.armor + "\n" + Res.formatNumber2(number3), this, 9007, null));
						GameCanvas.menu.startAt(myVector7, X, (selected + 1) * ITEM_HEIGHT - cmy + yScroll);
						addSkillDetail2(selected);
					}
					else if (selected == 4)
					{
						long num3 = 50000000L;
						int num4 = Char.myCharz().cCriticalGoc;
						if (num4 > t_tiemnang.Length - 1)
						{
							num4 = t_tiemnang.Length - 1;
						}
						num3 = t_tiemnang[num4];
						if (Char.myCharz().cTiemNang < num3)
						{
							GameCanvas.startOKDlg(mResources.not_enough_potential_point1 + Res.formatNumber2(Char.myCharz().cTiemNang) + mResources.not_enough_potential_point2 + Res.formatNumber2(num3));
							return;
						}
						GameCanvas.startYesNoDlg(mResources.use_potential_point_for1 + Res.formatNumber(num3) + mResources.use_potential_point_for2 + Char.myCharz().criticalFrom1000Tiemnang + mResources.for_crit, new Command(mResources.increase_upper, this, 9000, null), new Command(mResources.CANCEL, this, 4007, null));
					}
					else if (selected == 5)
					{
						Service.gI().speacialSkill(0);
					}
					return;
				}
				int num5 = selected - 6;
				SkillTemplate skillTemplate = Char.myCharz().nClass.skillTemplates[num5];
				Skill skill = Char.myCharz().getSkill(skillTemplate);
				Skill skill2 = null;
				MyVector myVector8 = new MyVector(string.Empty);
				if (skill != null)
				{
					if (skill.point == skillTemplate.maxPoint)
					{
						myVector8.addElement(new Command(mResources.make_shortcut, this, 9003, skill.template));
						myVector8.addElement(new Command(mResources.CLOSE, 2));
					}
					else
					{
						skill2 = skillTemplate.skills[skill.point];
						myVector8.addElement(new Command(mResources.UPGRADE, this, 9002, skill2));
						myVector8.addElement(new Command(mResources.make_shortcut, this, 9003, skill.template));
					}
				}
				else
				{
					skill2 = skillTemplate.skills[0];
					myVector8.addElement(new Command(mResources.learn, this, 9004, skill2));
				}
				GameCanvas.menu.startAt(myVector8, X, (selected + 1) * ITEM_HEIGHT - cmy + yScroll);
				addSkillDetail(skillTemplate, skill, skill2);
			}

	private void doFireBox()
			{
				if (selected < 0)
				{
					return;
				}
				currItem = null;
				MyVector myVector = new MyVector();
				if (currentTabIndex == 0 && !Equals(GameCanvas.panel2))
				{
					if (selected == 0)
					{
						setNewSelected(Char.myCharz().arrItemBox.Length, resetSelect: false, isTabBox: true);
					}
					else
					{
						sbyte b = (sbyte)GetInventorySelect_body(selected, newSelected);
						Item item = Char.myCharz().arrItemBox[b];
						if (item != null)
						{
							if (isBoxClan)
							{
								myVector.addElement(new Command(mResources.GETOUT, this, 1000, item));
								myVector.addElement(new Command(mResources.USE, this, 2010, item));
							}
							else if (item.isTypeBody())
							{
								myVector.addElement(new Command(mResources.GETOUT, this, 1000, item));
							}
							else
							{
								myVector.addElement(new Command(mResources.GETOUT, this, 1000, item));
							}
							currItem = item;
						}
					}
				}
				if (currentTabIndex == 1 || Equals(GameCanvas.panel2))
				{
					if (selected == 0)
					{
						setNewSelected(Char.myCharz().arrItemBody.Length + Char.myCharz().arrItemBag.Length, resetSelect: true, isTabBox: false);
					}
					else
					{
						Item[] arrItemBody = Char.myCharz().arrItemBody;
						if (!GetInventorySelect_isbody(selected, newSelected, arrItemBody))
						{
							sbyte b2 = (sbyte)GetInventorySelect_bag(selected, newSelected, arrItemBody);
							Item item2 = Char.myCharz().arrItemBag[b2];
							if (item2 != null)
							{
								myVector.addElement(new Command(mResources.move_to_chest, this, 1001, item2));
								if (item2.isTypeBody())
								{
									myVector.addElement(new Command(mResources.USE, this, 2000, item2));
								}
								else
								{
									myVector.addElement(new Command(mResources.USE, this, 2001, item2));
								}
								currItem = item2;
							}
						}
						else
						{
							Item item3 = Char.myCharz().arrItemBody[GetInventorySelect_body(selected, newSelected)];
							if (item3 != null)
							{
								myVector.addElement(new Command(mResources.move_to_chest2, this, 1002, item3));
								currItem = item3;
							}
						}
					}
				}
				if (currItem != null)
				{
					Char.myCharz().setPartTemp(currItem.headTemp, currItem.bodyTemp, currItem.legTemp, currItem.bagTemp);
					if (isBoxClan)
					{
						myVector.addElement(new Command(mResources.MOVEOUT, this, 2011, currItem));
					}
					GameCanvas.menu.startAt(myVector, X, (selected + 1) * ITEM_HEIGHT - cmy + yScroll);
					addItemDetail(currItem);
				}
				else
				{
					cp = null;
				}
				cmyLim = currentListLength * ITEM_HEIGHT - hScroll;
			}

	public void perform(int idAction, object p)
			{
				if (idAction == 9999)
				{
					TopInfo topInfo = (TopInfo)p;
					Service.gI().sendThachDau(topInfo.pId);
				}
				if (idAction == 170391)
				{
					Rms.clearAll();
					if (mGraphics.zoomLevel > 1)
					{
						Rms.saveRMSInt("levelScreenKN", 1);
					}
					else
					{
						Rms.saveRMSInt("levelScreenKN", 0);
					}
					GameMidlet.instance.exit();
				}
				if (idAction == 6001)
				{
					Item item = (Item)p;
					item.isSelect = false;
					GameCanvas.panel.vItemCombine.removeElement(item);
					if (GameCanvas.panel.currentTabIndex == 0)
					{
						GameCanvas.panel.setTabCombine();
					}
				}
				if (idAction == 6000)
				{
					Item item2 = (Item)p;
					for (int i = 0; i < GameCanvas.panel.vItemCombine.size(); i++)
					{
						Item item3 = (Item)GameCanvas.panel.vItemCombine.elementAt(i);
						if (item3.template.id == item2.template.id)
						{
							GameCanvas.startOKDlg(mResources.already_has_item);
							return;
						}
					}
					item2.isSelect = true;
					GameCanvas.panel.vItemCombine.addElement(item2);
					if (GameCanvas.panel.currentTabIndex == 0)
					{
						GameCanvas.panel.setTabCombine();
					}
				}
				if (idAction == 7000)
				{
					if (isLock)
					{
						GameCanvas.startOKDlg(mResources.unlock_item_to_trade);
						return;
					}
					Item item4 = (Item)p;
					for (int j = 0; j < GameCanvas.panel.vMyGD.size(); j++)
					{
						Item item5 = (Item)GameCanvas.panel.vMyGD.elementAt(j);
						if (item5.indexUI == item4.indexUI)
						{
							GameCanvas.startOKDlg(mResources.already_has_item);
							return;
						}
					}
					if (item4.quantity > 1)
					{
						putQuantily();
						return;
					}
					item4.isSelect = true;
					Item item6 = new Item();
					item6.template = item4.template;
					item6.itemOption = item4.itemOption;
					item6.indexUI = item4.indexUI;
					GameCanvas.panel.vMyGD.addElement(item6);
					Service.gI().giaodich(2, -1, (sbyte)item6.indexUI, item6.quantity);
				}
				if (idAction == 7001)
				{
					Item item7 = (Item)p;
					item7.isSelect = false;
					GameCanvas.panel.vMyGD.removeElement(item7);
					if (GameCanvas.panel.currentTabIndex == 1)
					{
						GameCanvas.panel.setTabGiaoDich(isMe: true);
					}
					Service.gI().giaodich(4, -1, (sbyte)item7.indexUI, -1);
				}
				if (idAction == 7002)
				{
					isAccept = true;
					GameCanvas.endDlg();
					Service.gI().giaodich(7, -1, -1, -1);
					hide();
				}
				if (idAction == 8003)
				{
					InfoItem infoItem = (InfoItem)p;
					Service.gI().friend(1, infoItem.charInfo.charID);
					if (type != 8)
					{
					}
				}
				if (idAction == 8002)
				{
					InfoItem infoItem2 = (InfoItem)p;
					Service.gI().friend(2, infoItem2.charInfo.charID);
				}
				if (idAction == 8004)
				{
					InfoItem infoItem3 = (InfoItem)p;
					Service.gI().gotoPlayer(infoItem3.charInfo.charID);
				}
				if (idAction == 8001)
				{
					Res.outz("chat player");
					InfoItem infoItem4 = (InfoItem)p;
					if (chatTField == null)
					{
						chatTField = new ChatTextField();
						chatTField.tfChat.y = GameCanvas.h - 35 - ChatTextField.gI().tfChat.height;
						chatTField.initChatTextField();
						chatTField.parentScreen = GameCanvas.panel;
					}
					chatTField.strChat = mResources.chat_player;
					chatTField.tfChat.name = mResources.chat_with + " " + infoItem4.charInfo.cName;
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
				}
				if (idAction == 1000)
				{
					Service.gI().getItem(BOX_BAG, (sbyte)GetInventorySelect_body(selected, newSelected));
				}
				if (idAction == 1001)
				{
					sbyte id = (sbyte)GetInventorySelect_bag(selected, newSelected, Char.myCharz().arrItemBody);
					Service.gI().getItem(BAG_BOX, id);
				}
				if (idAction == 1003)
				{
					hide();
				}
				if (idAction == 1002)
				{
					Service.gI().getItem(BODY_BOX, (sbyte)GetInventorySelect_body(selected, newSelected));
				}
				if (idAction == 2011)
				{
					Service.gI().useItem(1, 2, (sbyte)GetInventorySelect_body(selected, newSelected), -1);
				}
				if (idAction == 2010)
				{
					Service.gI().useItem(0, 2, (sbyte)GetInventorySelect_body(selected, newSelected), -1);
					Item item8 = (Item)p;
					if (item8 != null && (item8.template.id == 193 || item8.template.id == 194))
					{
						GameCanvas.panel.hide();
					}
				}
				if (idAction == 2000)
				{
					Item[] arrItemBody = Char.myCharz().arrItemBody;
					sbyte id2 = (sbyte)GetInventorySelect_bag(selected, newSelected, arrItemBody);
					if (isnewInventory)
					{
						id2 = (sbyte)currItem.indexUI;
					}
					Service.gI().getItem(BAG_BODY, id2);
				}
				if (idAction == 2001)
				{
					Res.outz("use item");
					Item item9 = (Item)p;
					bool inventorySelect_isbody = GetInventorySelect_isbody(selected, newSelected, Char.myCharz().arrItemBody);
					sbyte b = 0;
					b = (inventorySelect_isbody ? ((sbyte)GetInventorySelect_body(selected, newSelected)) : ((sbyte)GetInventorySelect_bag(selected, newSelected, Char.myCharz().arrItemBody)));
					if (isnewInventory)
					{
						b = (sbyte)currItem.indexUI;
						sbyte where = 0;
						if (newSelected != 0)
						{
							where = 1;
						}
						Service.gI().useItem(0, where, b, -1);
					}
					else
					{
						Service.gI().useItem(0, (!inventorySelect_isbody) ? ((sbyte)1) : ((sbyte)0), b, -1);
					}
					if (item9.template.id == 193 || item9.template.id == 194)
					{
						GameCanvas.panel.hide();
					}
				}
				if (idAction == 2002)
				{
					if (isnewInventory)
					{
						Service.gI().getItem(BODY_BAG, (sbyte)sellectInventory);
					}
					else
					{
						Service.gI().getItem(BODY_BAG, (sbyte)GetInventorySelect_body(selected, newSelected));
					}
				}
				if (idAction == 2003)
				{
					Res.outz("remove item");
					bool inventorySelect_isbody2 = GetInventorySelect_isbody(selected, newSelected, Char.myCharz().arrItemBody);
					sbyte b2 = 0;
					b2 = (inventorySelect_isbody2 ? ((sbyte)GetInventorySelect_body(selected, newSelected)) : ((sbyte)GetInventorySelect_bag(selected, newSelected, Char.myCharz().arrItemBody)));
					Service.gI().useItem(1, (!inventorySelect_isbody2) ? ((sbyte)1) : ((sbyte)0), b2, -1);
				}
				if (idAction == 2004)
				{
					GameCanvas.endDlg();
					ItemObject itemObject = (ItemObject)p;
					sbyte where2 = (sbyte)itemObject.where;
					sbyte index = (sbyte)itemObject.id;
					Service.gI().useItem((sbyte)((itemObject.type != 0) ? 2 : 3), where2, index, -1);
				}
				if (idAction == 2005)
				{
					sbyte id3 = (sbyte)GetInventorySelect_bag(selected, newSelected, Char.myCharz().arrItemBody);
					Service.gI().getItem(BAG_PET, id3);
				}
				if (idAction == 2006)
				{
					Item[] arrItemBody2 = Char.myPetz().arrItemBody;
					sbyte id4 = (sbyte)selected;
					Service.gI().getItem(PET_BAG, id4);
				}
				if (idAction == 30001)
				{
					Res.outz("nhan do");
					Service.gI().buyItem(0, selected, 0);
				}
				if (idAction == 30002)
				{
					Res.outz("xoa do");
					Service.gI().buyItem(1, selected, 0);
				}
				if (idAction == 30003)
				{
					Res.outz("nhan tat");
					Service.gI().buyItem(2, selected, 0);
				}
				if (idAction == 3000)
				{
					Res.outz("mua do");
					Item item10 = (Item)p;
					Service.gI().buyItem(0, item10.template.id, 0);
				}
				if (idAction == 3001)
				{
					Item item11 = (Item)p;
					GameCanvas.msgdlg.pleasewait();
					Service.gI().buyItem(1, item11.template.id, 0);
				}
				if (idAction == 3002)
				{
					GameCanvas.endDlg();
					bool inventorySelect_isbody3 = GetInventorySelect_isbody(selected, newSelected, Char.myCharz().arrItemBody);
					sbyte b3 = 0;
					b3 = (inventorySelect_isbody3 ? ((sbyte)GetInventorySelect_body(selected, newSelected)) : ((sbyte)GetInventorySelect_bag(selected, newSelected, Char.myCharz().arrItemBody)));
					Service.gI().saleItem(0, (!inventorySelect_isbody3) ? ((sbyte)1) : ((sbyte)0), b3);
				}
				if (idAction == 3003)
				{
					GameCanvas.endDlg();
					ItemObject itemObject2 = (ItemObject)p;
					Service.gI().saleItem(1, (sbyte)itemObject2.type, (short)itemObject2.id);
				}
				if (idAction == 3004)
				{
					Item item12 = (Item)p;
					Service.gI().buyItem(3, item12.template.id, 0);
				}
				if (idAction == 3005)
				{
					Res.outz("mua do");
					Item item13 = (Item)p;
					Service.gI().buyItem(3, item13.template.id, 0);
				}
				if (idAction == 4000)
				{
					Clan clan = (Clan)p;
					if (clan != null)
					{
						GameCanvas.endDlg();
						Service.gI().clanMessage(2, null, clan.ID);
					}
				}
				if (idAction == 4001)
				{
					Clan clan2 = (Clan)p;
					if (clan2 != null)
					{
						InfoDlg.showWait();
						clanReport = mResources.PLEASEWAIT;
						Service.gI().clanMember(clan2.ID);
					}
				}
				if (idAction == 4005)
				{
					GameCanvas.endDlg();
				}
				if (idAction == 4007)
				{
					GameCanvas.endDlg();
				}
				if (idAction == 4006)
				{
					ClanMessage clanMessage = (ClanMessage)p;
					Service.gI().clanDonate(clanMessage.id);
				}
				if (idAction == 5001)
				{
					Member member = (Member)p;
					Service.gI().clanRemote(member.ID, 0);
				}
				if (idAction == 5002)
				{
					Member member2 = (Member)p;
					Service.gI().clanRemote(member2.ID, 1);
				}
				if (idAction == 5003)
				{
					Member member3 = (Member)p;
					Service.gI().clanRemote(member3.ID, 2);
				}
				if (idAction == 5004)
				{
					Member member4 = (Member)p;
					Service.gI().clanRemote(member4.ID, -1);
				}
				if (idAction == 9000)
				{
					Service.gI().upPotential(selected, 1);
					GameCanvas.endDlg();
					InfoDlg.showWait();
				}
				if (idAction == 9006)
				{
					Service.gI().upPotential(selected, 10);
					GameCanvas.endDlg();
					InfoDlg.showWait();
				}
				if (idAction == 9007)
				{
					Service.gI().upPotential(selected, 100);
					GameCanvas.endDlg();
					InfoDlg.showWait();
				}
				if (idAction == 9002)
				{
					Skill skill = (Skill)p;
					if (skill.template.isSkillSpec())
					{
						GameCanvas.startOKDlg(mResources.updSkill);
					}
					else
					{
						GameCanvas.startOKDlg(mResources.can_buy_from_Uron1 + skill.powRequire + mResources.can_buy_from_Uron2 + skill.moreInfo + mResources.can_buy_from_Uron3);
					}
				}
				if (idAction == 9003)
				{
					if (GameCanvas.isTouch && !Main.isPC)
					{
						GameScr.gI().doSetOnScreenSkill((SkillTemplate)p);
					}
					else
					{
						GameScr.gI().doSetKeySkill((SkillTemplate)p);
					}
				}
				if (idAction == 9004)
				{
					Skill skill2 = (Skill)p;
					if (skill2.template.isSkillSpec())
					{
						GameCanvas.startOKDlg(mResources.learnSkill);
					}
					else
					{
						GameCanvas.startOKDlg(mResources.can_buy_from_Uron1 + skill2.powRequire + mResources.can_buy_from_Uron2 + skill2.moreInfo + mResources.can_buy_from_Uron3);
					}
				}
				if (idAction == 10000)
				{
					InfoItem infoItem5 = (InfoItem)p;
					Service.gI().enemy(1, infoItem5.charInfo.charID);
					GameCanvas.panel.hideNow();
				}
				if (idAction == 10001)
				{
					InfoItem infoItem6 = (InfoItem)p;
					Service.gI().enemy(2, infoItem6.charInfo.charID);
					InfoDlg.showWait();
				}
				if (idAction == 10021)
				{
				}
				if (idAction == 10012)
				{
					if (chatTField == null)
					{
						chatTField = new ChatTextField();
						chatTField.tfChat.y = GameCanvas.h - 35 - ChatTextField.gI().tfChat.height;
						chatTField.initChatTextField();
						chatTField.parentScreen = ((GameCanvas.panel2 != null) ? GameCanvas.panel2 : GameCanvas.panel);
					}
					chatTField.tfChat.setIputType(TField.INPUT_TYPE_NUMERIC);
					chatTField.tfChat.setText(string.Empty);
					if (currItem.quantity == 1)
					{
						chatTField.strChat = mResources.kiguiXuchat;
						chatTField.tfChat.name = mResources.input_money;
					}
					else
					{
						chatTField.strChat = mResources.input_quantity + " ";
						chatTField.tfChat.name = mResources.input_quantity;
					}
					chatTField.tfChat.setMaxTextLenght(10);
					chatTField.to = string.Empty;
					chatTField.isShow = true;
					chatTField.tfChat.setIputType(TField.INPUT_TYPE_NUMERIC);
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
				if (idAction == 10013)
				{
					if (chatTField == null)
					{
						chatTField = new ChatTextField();
						chatTField.tfChat.y = GameCanvas.h - 35 - ChatTextField.gI().tfChat.height;
						chatTField.initChatTextField();
						chatTField.parentScreen = ((GameCanvas.panel2 != null) ? GameCanvas.panel2 : GameCanvas.panel);
					}
					chatTField.tfChat.setIputType(TField.INPUT_TYPE_NUMERIC);
					chatTField.tfChat.setText(string.Empty);
					if (currItem.quantity == 1)
					{
						chatTField.strChat = mResources.kiguiLuongchat;
						chatTField.tfChat.name = mResources.input_money;
					}
					else
					{
						chatTField.strChat = mResources.input_quantity + "  ";
						chatTField.tfChat.name = mResources.input_quantity;
					}
					chatTField.to = string.Empty;
					chatTField.isShow = true;
					chatTField.tfChat.setIputType(TField.INPUT_TYPE_NUMERIC);
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
				if (idAction == 10014)
				{
					Item item14 = (Item)p;
					Service.gI().kigui(1, item14.itemId, -1, -1, -1);
					InfoDlg.showWait();
				}
				if (idAction == 10015)
				{
					Item item15 = (Item)p;
					Service.gI().kigui(2, item15.itemId, -1, -1, -1);
					InfoDlg.showWait();
				}
				if (idAction == 10016)
				{
					Item item16 = (Item)p;
					Service.gI().kigui(3, item16.itemId, 0, item16.buyCoin, -1);
					InfoDlg.showWait();
				}
				if (idAction == 10017)
				{
					Item item17 = (Item)p;
					Service.gI().kigui(3, item17.itemId, 1, item17.buyGold, -1);
					InfoDlg.showWait();
				}
				if (idAction == 10018)
				{
					Item item18 = (Item)p;
					Service.gI().kigui(5, item18.itemId, -1, -1, -1);
					InfoDlg.showWait();
				}
				if (idAction == 10019)
				{
					Session_ME.gI().close();
					Rms.saveRMSString(Rms.RMS_acc, string.Empty);
					Rms.saveRMSString(Rms.RMS_pass, string.Empty);
					GameCanvas.loginScr.tfPass.setText(string.Empty);
					GameCanvas.loginScr.tfUser.setText(string.Empty);
					GameCanvas.loginScr.isLogin2 = false;
					GameCanvas.serverScreen.switchToMe();
					GameCanvas.endDlg();
					hide();
				}
				if (idAction == 10020)
				{
					GameCanvas.endDlg();
				}
				if (idAction == 10030)
				{
					Service.gI().getFlag(1, (sbyte)selected);
					GameCanvas.panel.hideNow();
				}
				if (idAction == 10031)
				{
					Session_ME.gI().close();
				}
				if (idAction == 11000)
				{
					Service.gI().kigui(0, currItem.itemId, 1, currItem.buyRuby, 1);
					GameCanvas.endDlg();
				}
				if (idAction == 11001)
				{
					Service.gI().kigui(0, currItem.itemId, 1, currItem.buyRuby, currItem.quantilyToBuy);
					GameCanvas.endDlg();
				}
				if (idAction == 11002)
				{
					chatTField.isShow = false;
					GameCanvas.endDlg();
				}
			}

	private void doFireOption()
			{
				if (selected < 0)
				{
					return;
				}
				switch (selected)
				{
				case 0:
					SoundMn.gI().AuraToolOption();
					break;
				case 1:
					SoundMn.gI().AuraToolOption2();
					break;
				case 2:
					SoundMn.gI().soundToolOption();
					break;
				case 3:
					if (Main.isPC)
					{
						GameCanvas.startYesNoDlg(mResources.changeSizeScreen, new Command(mResources.YES, this, 170391, null), new Command(mResources.NO, this, 4005, null));
					}
					else
					{
						SoundMn.gI().CaseSizeScr();
					}
					break;
				case 4:
					if (Main.isPC)
					{
						GameCanvas.startYesNoDlg(mResources.changeSizeScreen, new Command(mResources.YES, this, 170391, null), new Command(mResources.NO, this, 4005, null));
					}
					else
					{
						SoundMn.gI().CaseAnalog();
					}
					break;
				case 5:
					SoundMn.gI().CaseAnalog();
					break;
				}
			}

	private void doFireAccount()
			{
				if (selected < 0)
				{
					return;
				}
				switch (selected)
				{
				case 0:
					GameCanvas.endDlg();
					if (chatTField == null)
					{
						chatTField = new ChatTextField();
						chatTField.tfChat.y = GameCanvas.h - 35 - ChatTextField.gI().tfChat.height;
						chatTField.initChatTextField();
						chatTField.parentScreen = GameCanvas.panel;
					}
					chatTField.tfChat.setText(string.Empty);
					chatTField.strChat = mResources.input_Inventory_Pass;
					chatTField.tfChat.name = mResources.input_Inventory_Pass;
					chatTField.to = string.Empty;
					chatTField.isShow = true;
					chatTField.tfChat.isFocus = true;
					chatTField.tfChat.setIputType(TField.INPUT_TYPE_NUMERIC);
					if (GameCanvas.isTouch)
					{
						chatTField.tfChat.doChangeToTextBox();
					}
					if (!Main.isPC)
					{
						chatTField.startChat2(this, string.Empty);
					}
					if (Main.isWindowsPhone)
					{
						chatTField.tfChat.strInfo = chatTField.strChat;
					}
					break;
				case 1:
					Service.gI().friend(0, -1);
					InfoDlg.showWait();
					break;
				case 2:
					Service.gI().enemy(0, -1);
					InfoDlg.showWait();
					break;
				case 3:
					setTypeMessage();
					if (chatTField == null)
					{
						chatTField = new ChatTextField();
						chatTField.tfChat.y = GameCanvas.h - 35 - ChatTextField.gI().tfChat.height;
						chatTField.initChatTextField();
						chatTField.parentScreen = GameCanvas.panel;
					}
					break;
				case 4:
					if (mResources.language == 2)
					{
						string url = "http://dragonball.indonaga.com/coda/?username=" + GameCanvas.loginScr.tfUser.getText();
						hideNow();
						try
						{
							GameMidlet.instance.platformRequest(url);
							break;
						}
						catch (Exception ex)
						{
							ex.StackTrace.ToString();
							break;
						}
					}
					hideNow();
					if (Char.myCharz().taskMaint.taskId <= 10)
					{
						GameCanvas.startOKDlg(mResources.finishBomong);
					}
					else
					{
						MoneyCharge.gI().switchToMe();
					}
					break;
				case 5:
					setTypeAuto();
					break;
				}
			}

	private bool GetInventorySelect_isbody(int select, int subSelect, Item[] arrItem)
			{
				int num = select - 1 + subSelect * 20;
				return subSelect == 0 && num < arrItem.Length;
			}

	private int GetInventorySelect_body(int select, int subSelect)
			{
				return select - 1 + subSelect * 20;
			}

	private int GetInventorySelect_bag(int select, int subSelect, Item[] arrItem)
			{
				int num = select - 1 + subSelect * 20;
				return num - arrItem.Length;
			}

	private void setNewSelected(int arrLength, bool resetSelect, bool isTabBox)
			{
				int num = arrLength / 20 + ((arrLength % 20 > 0) ? 1 : 0);
				int num2 = xScroll;
				newSelected = (GameCanvas.px - num2) / TAB_W_NEW;
				if (newSelected > num - 1)
				{
					newSelected = num - 1;
				}
				if (GameCanvas.px < num2)
				{
					newSelected = 0;
				}
				if (!isTabBox)
				{
					setTabInventory(resetSelect);
				}
				else
				{
					setTabBox();
				}
			}

}
