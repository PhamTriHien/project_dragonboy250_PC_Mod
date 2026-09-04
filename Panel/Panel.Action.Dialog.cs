using System;
using System.Collections.Generic;
using Assets.src.g;
using UnityEngine;

public partial class Panel : IActionListener, IChatable
{
	public void setTabChatManager()
			{
				currentListLength = chats.Count;
				ITEM_HEIGHT = 24;
				selected = (GameCanvas.isTouch ? (-1) : 0);
				cmyLim = currentListLength * ITEM_HEIGHT - hScroll;
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
					cmy = (cmtoY = cmyLim);
				}
			}

	public void setTabChatPlayer()
			{
			}

	public void setTypeChatPlayer()
			{
			}

	public void addChatMessage(InfoItem info)
			{
				logChat.insertElementAt(info, 0);
				if (logChat.size() > 20)
				{
					logChat.removeElementAt(logChat.size() - 1);
				}
			}

	public void chatTFUpdateKey()
			{
				if (chatTField != null && chatTField.isShow)
				{
					if (chatTField.left != null && (GameCanvas.keyPressed[12] || mScreen.getCmdPointerLast(chatTField.left)) && chatTField.left != null)
					{
						chatTField.left.performAction();
					}
					if (chatTField.right != null && (GameCanvas.keyPressed[13] || mScreen.getCmdPointerLast(chatTField.right)) && chatTField.right != null)
					{
						chatTField.right.performAction();
					}
					if (chatTField.center != null && (GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25] || mScreen.getCmdPointerLast(chatTField.center)) && chatTField.center != null)
					{
						chatTField.center.performAction();
					}
					if (chatTField.isShow && GameCanvas.keyAsciiPress != 0)
					{
						chatTField.keyPressed(GameCanvas.keyAsciiPress);
						GameCanvas.keyAsciiPress = 0;
					}
					GameCanvas.clearKeyHold();
					GameCanvas.clearKeyPressed();
				}
			}

	public void onChatFromMe(string text, string to)
			{
				if (chatTField.tfChat.getText() == null || chatTField.tfChat.getText().Equals(string.Empty) || text.Equals(string.Empty) || text == null)
				{
					chatTField.isShow = false;
					return;
				}
				if (chatTField.strChat.Equals(mResources.input_clan_name))
				{
					InfoDlg.showWait();
					chatTField.isShow = false;
					Service.gI().searchClan(text);
					return;
				}
				if (chatTField.strChat.Equals(mResources.chat_clan))
				{
					InfoDlg.showWait();
					chatTField.isShow = false;
					Service.gI().clanMessage(0, text, -1);
					return;
				}
				if (chatTField.strChat.Equals(mResources.input_clan_name_to_create))
				{
					if (chatTField.tfChat.getText() == string.Empty)
					{
						GameScr.info1.addInfo(mResources.clan_name_blank, 0);
						return;
					}
					if (tabIcon == null)
					{
						tabIcon = new TabClanIcon();
					}
					tabIcon.text = chatTField.tfChat.getText();
					tabIcon.show(isGetName: false);
					chatTField.isShow = false;
					return;
				}
				if (chatTField.strChat.Equals(mResources.input_clan_slogan))
				{
					if (chatTField.tfChat.getText() == string.Empty)
					{
						GameScr.info1.addInfo(mResources.clan_slogan_blank, 0);
						return;
					}
					Service.gI().getClan(4, Char.myCharz().clan.imgID, chatTField.tfChat.getText());
					chatTField.isShow = false;
					return;
				}
				if (chatTField.strChat.Equals(mResources.input_Inventory_Pass))
				{
					try
					{
						int lockInventory = int.Parse(chatTField.tfChat.getText());
						chatTField.isShow = false;
						chatTField.tfChat.setIputType(TField.INPUT_TYPE_ANY);
						hide();
						if (chatTField.tfChat.getText().Length != 6 || chatTField.tfChat.getText().Equals(string.Empty))
						{
							GameCanvas.startOKDlg(mResources.input_Inventory_Pass_wrong);
						}
						else
						{
							Service.gI().setLockInventory(lockInventory);
							chatTField.isShow = false;
							chatTField.tfChat.setIputType(TField.INPUT_TYPE_ANY);
							hide();
						}
						return;
					}
					catch (Exception)
					{
						GameCanvas.startOKDlg(mResources.ALERT_PRIVATE_PASS_2);
						return;
					}
				}
				if (chatTField.strChat.Equals(mResources.world_channel_5_luong))
				{
					if (!chatTField.tfChat.getText().Equals(string.Empty))
					{
						Service.gI().chatGlobal(chatTField.tfChat.getText());
						chatTField.isShow = false;
						hide();
					}
				}
				else if (chatTField.strChat.Equals(mResources.chat_player))
				{
					chatTField.isShow = false;
					InfoItem infoItem = null;
					if (type == 8)
					{
						infoItem = (InfoItem)logChat.elementAt(currInfoItem);
					}
					else if (type == 11)
					{
						infoItem = (InfoItem)vFriend.elementAt(currInfoItem);
					}
					if (infoItem.charInfo.charID != Char.myCharz().charID)
					{
						Service.gI().chatPlayer(text, infoItem.charInfo.charID);
					}
				}
				else if (chatTField.strChat.Equals(mResources.input_quantity_to_trade))
				{
					int num = 0;
					try
					{
						num = int.Parse(chatTField.tfChat.getText());
					}
					catch (Exception)
					{
						GameCanvas.startOKDlg(mResources.input_quantity_wrong);
						chatTField.isShow = false;
						chatTField.tfChat.setIputType(TField.INPUT_TYPE_ANY);
						return;
					}
					if (num <= 0 || num > currItem.quantity)
					{
						GameCanvas.startOKDlg(mResources.input_quantity_wrong);
						chatTField.isShow = false;
						chatTField.tfChat.setIputType(TField.INPUT_TYPE_ANY);
						return;
					}
					currItem.isSelect = true;
					Item item = new Item();
					item.template = currItem.template;
					item.quantity = num;
					item.indexUI = currItem.indexUI;
					item.itemOption = currItem.itemOption;
					GameCanvas.panel.vMyGD.addElement(item);
					Service.gI().giaodich(2, -1, (sbyte)item.indexUI, item.quantity);
					chatTField.isShow = false;
					chatTField.tfChat.setIputType(TField.INPUT_TYPE_ANY);
				}
				else if (chatTField.strChat == mResources.input_money_to_trade)
				{
					int num2 = 0;
					try
					{
						num2 = int.Parse(chatTField.tfChat.getText());
					}
					catch (Exception)
					{
						GameCanvas.startOKDlg(mResources.input_money_wrong);
						chatTField.isShow = false;
						chatTField.tfChat.setIputType(TField.INPUT_TYPE_ANY);
						return;
					}
					if (num2 > Char.myCharz().xu)
					{
						GameCanvas.startOKDlg(mResources.not_enough_money);
						chatTField.isShow = false;
						chatTField.tfChat.setIputType(TField.INPUT_TYPE_ANY);
					}
					else
					{
						moneyGD = num2;
						Service.gI().giaodich(2, -1, -1, num2);
						chatTField.isShow = false;
						chatTField.tfChat.setIputType(TField.INPUT_TYPE_ANY);
					}
				}
				else if (chatTField.strChat.Equals(mResources.kiguiXuchat))
				{
					try
					{
						Service.gI().kigui(0, currItem.itemId, 0, int.Parse(chatTField.tfChat.getText()), 1);
					}
					catch (Exception)
					{
						GameCanvas.startOKDlg(mResources.input_money_wrong);
					}
					chatTField.isShow = false;
				}
				else if (chatTField.strChat.Equals(mResources.kiguiXuchat + " "))
				{
					try
					{
						Service.gI().kigui(0, currItem.itemId, 0, int.Parse(chatTField.tfChat.getText()), currItem.quantilyToBuy);
					}
					catch (Exception)
					{
						GameCanvas.startOKDlg(mResources.input_money_wrong);
					}
					chatTField.isShow = false;
				}
				else if (chatTField.strChat.Equals(mResources.kiguiLuongchat))
				{
					doNotiRuby(0);
					chatTField.isShow = false;
				}
				else if (chatTField.strChat.Equals(mResources.kiguiLuongchat + "  "))
				{
					doNotiRuby(1);
					chatTField.isShow = false;
				}
				else if (chatTField.strChat.Equals(mResources.input_quantity + " "))
				{
					currItem.quantilyToBuy = int.Parse(chatTField.tfChat.getText());
					if (currItem.quantilyToBuy > currItem.quantity)
					{
						GameCanvas.startOKDlg(mResources.input_quantity_wrong);
						return;
					}
					isKiguiXu = true;
					chatTField.isShow = false;
				}
				else if (chatTField.strChat.Equals(mResources.input_quantity + "  "))
				{
					currItem.quantilyToBuy = int.Parse(chatTField.tfChat.getText());
					if (currItem.quantilyToBuy > currItem.quantity)
					{
						GameCanvas.startOKDlg(mResources.input_quantity_wrong);
						return;
					}
					isKiguiLuong = true;
					chatTField.isShow = false;
				}
			}

	public void onCancelChat()
			{
				chatTField.tfChat.setIputType(TField.INPUT_TYPE_ANY);
			}

}
