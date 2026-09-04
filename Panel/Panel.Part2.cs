using System;
using System.Collections.Generic;
using Assets.src.g;
using UnityEngine;
public partial class Panel : IActionListener, IChatable
{
	public void hide()
		{
			if (timeShow > 0)
			{
				isClose = false;
				return;
			}
			cp = null;
			if (isTypeShop() || TileMap.mapID == 45)
			{
				Char.myCharz().resetPartTemp();
			}
			if (chatTField != null && type == 13 && chatTField.isShow)
			{
				chatTField = null;
			}
			if (type == 13 && !isAccept)
			{
				Service.gI().giaodich(3, -1, -1, -1);
			}
			if (type == 15)
			{
				Service.gI().sendThachDau(-1);
			}
			SoundMn.gI().buttonClose();
			GameScr.isPaint = true;
			TileMap.lastPlanetId = -1;
			if (imgMap != null)
			{
				imgMap.texture = null;
				imgMap = null;
			}
			mSystem.gcc();
			isClanOption = false;
			if (type != 4)
			{
				if (type == 24)
				{
					setTypeGameInfo();
				}
				else if (type == 23)
				{
					setTypeMain();
				}
				else if (type == 3 || type == 14)
				{
					if (isChangeZone)
					{
						isClose = true;
					}
					else
					{
						setTypeMain();
						cmx = (cmtoX = 0);
					}
				}
				else if (type == 18 || type == 19 || type == 20 || type == 21)
				{
					setTypeMain();
					cmx = (cmtoX = 0);
				}
				else if (type == 8 || type == 11 || type == 16)
				{
					setTypeAccount();
					cmx = (cmtoX = 0);
				}
				else
				{
					isClose = true;
				}
			}
			else
			{
				setTypeMain();
				cmx = (cmtoX = 0);
			}
			Hint.clickNpc();
			GameCanvas.panel2 = null;
			GameCanvas.clearAllPointerEvent();
			GameCanvas.clearKeyPressed();
			GameCanvas.isFocusPanel2 = false;
			pointerDownTime = (pointerDownFirstX = 0);
			pointerIsDowning = false;
			if ((Char.myCharz().cHP <= 0 || Char.myCharz().statusMe == 14 || Char.myCharz().statusMe == 5) && Char.myCharz().meDead)
			{
				Command center = new Command(mResources.DIES[0], 11038, GameScr.gI());
				GameScr.gI().center = center;
				Char.myCharz().cHP = 0L;
			}
		}
	private void doSpeacialSkill()
		{
		}
	private void doRada()
		{
			hide();
			if (RadarScr.list == null || RadarScr.list.size() == 0)
			{
				Service.gI().SendRada(0, -1);
				RadarScr.gI().switchToMe();
			}
			else
			{
				RadarScr.gI().switchToMe();
			}
		}
	public void putMoney()
		{
			if (chatTField == null)
			{
				chatTField = new ChatTextField();
				chatTField.tfChat.y = GameCanvas.h - 35 - ChatTextField.gI().tfChat.height;
				chatTField.initChatTextField();
				chatTField.parentScreen = GameCanvas.panel;
			}
			chatTField.strChat = mResources.input_money_to_trade;
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
	public void putQuantily()
		{
			if (chatTField == null)
			{
				chatTField = new ChatTextField();
				chatTField.tfChat.y = GameCanvas.h - 35 - ChatTextField.gI().tfChat.height;
				chatTField.initChatTextField();
				chatTField.parentScreen = GameCanvas.panel;
			}
			chatTField.strChat = mResources.input_quantity_to_trade;
			chatTField.tfChat.name = mResources.input_quantity;
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
	public void chagenSlogan()
		{
			chatTField.strChat = mResources.input_clan_slogan;
			chatTField.tfChat.name = mResources.input_clan_slogan;
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
	public void changeIcon()
		{
			if (tabIcon == null)
			{
				tabIcon = new TabClanIcon();
			}
			tabIcon.text = chatTField.tfChat.getText();
			tabIcon.show(isGetName: false);
			chatTField.isShow = false;
		}
	private void addFriend(InfoItem info)
		{
			string text = "|0|1|" + info.charInfo.cName;
			text += "\n";
			text = ((!info.isOnline) ? (text + "|3|1|" + mResources.is_offline) : (text + "|4|1|" + mResources.is_online));
			text += "\n--";
			string text2 = text;
			text = text2 + "\n|5|" + mResources.power + ": " + info.s;
			cp = new ChatPopup();
			popUpDetailInit(cp, text);
			charInfo = info.charInfo;
			currItem = null;
		}
	private void addLogMessage(InfoItem info)
		{
			string text = "|0|1|" + info.charInfo.cName;
			text += "\n";
			text += "\n--";
			text = text + "\n|5|" + Res.split(info.s, "|", 0)[2];
			cp = new ChatPopup();
			popUpDetailInit(cp, text);
			charInfo = info.charInfo;
			currItem = null;
		}
	private void addSkillDetail2(int type)
		{
			string empty = string.Empty;
			int num = 0;
			if (selected == 0)
			{
				num = Char.myCharz().cHPGoc + 1000;
			}
			if (selected == 1)
			{
				num = Char.myCharz().cMPGoc + 1000;
			}
			if (selected == 2)
			{
				num = Char.myCharz().cDamGoc * Char.myCharz().expForOneAdd;
			}
			if (selected == 3)
			{
				num = 500000 + Char.myCharz().cDefGoc * 100000;
			}
			string text = empty;
			empty = text + "|5|2|" + mResources.USE + " " + num + " " + mResources.potential;
			if (type == 0)
			{
				empty = empty + "\n|5|2|" + mResources.to_gain_20hp;
			}
			if (type == 1)
			{
				empty = empty + "\n|5|2|" + mResources.to_gain_20mp;
			}
			if (type == 2)
			{
				empty = empty + "\n|5|2|" + mResources.to_gain_1pow;
			}
			if (type == 3)
			{
				empty = empty + "\n|5|2|" + mResources.to_gain_1pow;
			}
			currItem = null;
			partID = null;
			charInfo = null;
			idIcon = -1;
			cp = new ChatPopup();
			popUpDetailInit(cp, empty);
		}
	public void itemRequest(sbyte itemAction, string info, sbyte where, sbyte index)
		{
			GameCanvas.endDlg();
			ItemObject itemObject = new ItemObject();
			itemObject.type = itemAction;
			itemObject.id = index;
			itemObject.where = where;
			GameCanvas.startYesNoDlg(info, new Command(mResources.YES, this, 2004, itemObject), new Command(mResources.NO, this, 4005, null));
		}
	public void saleRequest(sbyte type, string info, short id)
		{
			ItemObject itemObject = new ItemObject();
			itemObject.type = type;
			itemObject.id = id;
			GameCanvas.startYesNoDlg(info, new Command(mResources.YES, this, 3003, itemObject), new Command(mResources.NO, this, 4005, null));
		}
	private void setDotStar()
		{
			for (int i = 0; i < yArgS.Length; i++)
			{
				if (angleS >= 360)
				{
					angleS -= 360;
				}
				if (angleS < 0)
				{
					angleS = 360 + angleS;
				}
				yArgS[i] = Res.abs(rS * Res.sin(angleS) / 1024);
				xArgS[i] = Res.abs(rS * Res.cos(angleS) / 1024);
				if (angleS < 90)
				{
					xDotS[i] = xS + xArgS[i];
					yDotS[i] = yS - yArgS[i];
				}
				else if (angleS >= 90 && angleS < 180)
				{
					xDotS[i] = xS - xArgS[i];
					yDotS[i] = yS - yArgS[i];
				}
				else if (angleS >= 180 && angleS < 270)
				{
					xDotS[i] = xS - xArgS[i];
					yDotS[i] = yS + yArgS[i];
				}
				else
				{
					xDotS[i] = xS + xArgS[i];
					yDotS[i] = yS + yArgS[i];
				}
				angleS -= iAngleS;
			}
		}
	private void doNotiRuby(int type)
		{
			try
			{
				currItem.buyRuby = int.Parse(chatTField.tfChat.getText());
			}
			catch (Exception)
			{
				GameCanvas.startOKDlg(mResources.input_money_wrong);
				chatTField.isShow = false;
				return;
			}
			Command cmdYes = new Command(mResources.YES, this, (type != 0) ? 11001 : 11000, null);
			Command cmdNo = new Command(mResources.NO, this, 11002, null);
			GameCanvas.startYesNoDlg(mResources.notiRuby, cmdYes, cmdNo);
		}
	private bool isTabInven()
		{
			if ((type == 0 && currentTabIndex == 1) || (type == 7 && currentTabIndex == 0))
			{
				return true;
			}
			return false;
		}

}
