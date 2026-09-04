using System;
using System.Collections.Generic;
using Assets.src.g;
using UnityEngine;

public partial class Panel : IActionListener, IChatable
{
	private void doFireCombine()
			{
				if (currentTabIndex == 0)
				{
					if (selected == -1 || vItemCombine.size() == 0)
					{
						return;
					}
					if (selected == vItemCombine.size())
					{
						keyTouchCombine = -1;
						selected = (GameCanvas.isTouch ? (-1) : 0);
						InfoDlg.showWait();
						Service.gI().combine(1, vItemCombine);
						return;
					}
					if (selected > vItemCombine.size() - 1)
					{
						return;
					}
					currItem = (Item)GameCanvas.panel.vItemCombine.elementAt(selected);
					MyVector myVector = new MyVector();
					myVector.addElement(new Command(mResources.GETOUT, this, 6001, currItem));
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
					doFireInventory();
				}
			}

	private void doFireShop()
			{
				currItem = null;
				if (selected < 0)
				{
					return;
				}
				MyVector myVector = new MyVector();
				if (currentTabIndex < currentTabName.Length - ((GameCanvas.panel2 == null) ? 1 : 0) && type != 17)
				{
					currItem = Char.myCharz().arrItemShop[currentTabIndex][selected];
					if (currItem != null)
					{
						if (currItem.isBuySpec)
						{
							if (currItem.buySpec > 0)
							{
								myVector.addElement(new Command(mResources.buy_with + "\n" + Res.formatNumber2(currItem.buySpec), this, 3005, currItem));
							}
						}
						else if (typeShop == 4)
						{
							myVector.addElement(new Command(mResources.receive_upper, this, 30001, currItem));
							myVector.addElement(new Command(mResources.DELETE, this, 30002, currItem));
							myVector.addElement(new Command(mResources.receive_all, this, 30003, currItem));
						}
						else if (currItem.buyCoin == 0 && currItem.buyGold == 0)
						{
							if (currItem.powerRequire != 0)
							{
								myVector.addElement(new Command(mResources.learn_with + "\n" + Res.formatNumber(currItem.powerRequire) + " \n" + mResources.potential, this, 3004, currItem));
							}
							else
							{
								myVector.addElement(new Command(mResources.receive_upper + "\n" + mResources.free, this, 3000, currItem));
							}
						}
						else if (typeShop == 8)
						{
							if (currItem.buyCoin > 0)
							{
								myVector.addElement(new Command(mResources.buy_with + "\n" + Res.formatNumber2(currItem.buyCoin) + "\n" + mResources.XU, this, 30001, currItem));
							}
							if (currItem.buyGold > 0)
							{
								myVector.addElement(new Command(mResources.buy_with + "\n" + Res.formatNumber2(currItem.buyGold) + "\n" + mResources.LUONG, this, 30002, currItem));
							}
						}
						else if (typeShop != 2)
						{
							if (currItem.buyCoin > 0)
							{
								myVector.addElement(new Command(mResources.buy_with + "\n" + Res.formatNumber2(currItem.buyCoin) + "\n" + mResources.XU, this, 3000, currItem));
							}
							if (currItem.buyGold > 0)
							{
								myVector.addElement(new Command(mResources.buy_with + "\n" + Res.formatNumber2(currItem.buyGold) + "\n" + mResources.LUONG, this, 3001, currItem));
							}
						}
						else
						{
							if (currItem.buyCoin != -1)
							{
								myVector.addElement(new Command(mResources.buy_with + "\n" + Res.formatNumber2(currItem.buyCoin) + "\n" + mResources.XU, this, 10016, currItem));
							}
							if (currItem.buyGold != -1)
							{
								myVector.addElement(new Command(mResources.buy_with + "\n" + Res.formatNumber2(currItem.buyGold) + "\n" + mResources.LUONG, this, 10017, currItem));
							}
						}
					}
				}
				else if (typeShop == 0)
				{
					if (selected == 0)
					{
						setNewSelected(Char.myCharz().arrItemBody.Length + Char.myCharz().arrItemBag.Length, resetSelect: false, isTabBox: false);
					}
					else
					{
						currItem = null;
						if (!GetInventorySelect_isbody(selected, newSelected, Char.myCharz().arrItemBody))
						{
							Item item = Char.myCharz().arrItemBag[GetInventorySelect_bag(selected, newSelected, Char.myCharz().arrItemBody)];
							if (item != null)
							{
								currItem = item;
							}
						}
						else
						{
							Item item2 = Char.myCharz().arrItemBody[GetInventorySelect_body(selected, newSelected)];
							if (item2 != null)
							{
								currItem = item2;
							}
						}
						if (currItem != null)
						{
							myVector.addElement(new Command(mResources.SALE, this, 3002, currItem));
						}
					}
				}
				else
				{
					if (type == 17)
					{
						currItem = Char.myCharz().arrItemShop[4][selected];
					}
					else
					{
						currItem = Char.myCharz().arrItemShop[currentTabIndex][selected];
					}
					if (currItem.buyType == 0)
					{
						if (currItem.isHaveOption(87))
						{
							myVector.addElement(new Command(mResources.kiguiLuong, this, 10013, currItem));
						}
						else
						{
							myVector.addElement(new Command(mResources.kiguiXu, this, 10012, currItem));
						}
					}
					else if (currItem.buyType == 1)
					{
						myVector.addElement(new Command(mResources.huykigui, this, 10014, currItem));
						myVector.addElement(new Command(mResources.upTop, this, 10018, currItem));
					}
					else if (currItem.buyType == 2)
					{
						myVector.addElement(new Command(mResources.nhantien, this, 10015, currItem));
					}
				}
				if (currItem != null)
				{
					Char.myCharz().setPartTemp(currItem.headTemp, currItem.bodyTemp, currItem.legTemp, currItem.bagTemp);
					GameCanvas.menu.startAt(myVector, X, (selected + 1) * ITEM_HEIGHT - cmy + yScroll);
					addItemDetail(currItem);
				}
				else
				{
					cp = null;
				}
			}

}
