using System;
using System.Collections.Generic;
using Assets.src.g;
using UnityEngine;
public partial class Panel : IActionListener, IChatable
{
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

}
