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

}
