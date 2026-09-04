using System;
using Assets.src.g;
public partial class GameScr : mScreen, IChatable
{
	public void createMenu(string[] menu, Npc npc)
		{
			MyVector myVector = new MyVector();
			for (int i = 0; i < menu.Length; i++)
			{
				myVector.addElement(new Command(menu[i], 11057, npc));
			}
			GameCanvas.menu.startAt(myVector, 2);
		}
	public void doMenuInforMe()
		{
			scrMain.clear();
			scrInfo.clear();
			isViewNext = false;
			cmdBag = new Command(mResources.MENUME[0], 1100011);
			cmdSkill = new Command(mResources.MENUME[1], 1100012);
			cmdTiemnang = new Command(mResources.MENUME[2], 1100013);
			cmdInfo = new Command(mResources.MENUME[3], 1100014);
			cmdtrangbi = new Command(mResources.MENUME[4], 1100015);
			MyVector myVector = new MyVector();
			myVector.addElement(cmdBag);
			myVector.addElement(cmdSkill);
			myVector.addElement(cmdTiemnang);
			myVector.addElement(cmdInfo);
			myVector.addElement(cmdtrangbi);
			GameCanvas.menu.startAt(myVector, 3);
		}
	public void doMenusynthesis()
		{
			MyVector myVector = new MyVector();
			myVector.addElement(new Command(mResources.SYNTHESIS[0], 110002));
			myVector.addElement(new Command(mResources.SYNTHESIS[1], 1100032));
			myVector.addElement(new Command(mResources.SYNTHESIS[2], 1100033));
			GameCanvas.menu.startAt(myVector, 3);
		}
	public void playerMenu(Char c)
		{
			auto = 0;
			GameCanvas.clearKeyHold();
			if (Char.myCharz().charFocus.charID < 0 || Char.myCharz().charID < 0)
			{
				return;
			}
			MyVector vPlayerMenu = GameCanvas.panel.vPlayerMenu;
			if (vPlayerMenu.size() > 0)
			{
				return;
			}
			if (Char.myCharz().taskMaint != null && Char.myCharz().taskMaint.taskId > 1)
			{
				vPlayerMenu.addElement(new Command(mResources.make_friend, 11112, Char.myCharz().charFocus));
				vPlayerMenu.addElement(new Command(mResources.trade, 11113, Char.myCharz().charFocus));
			}
			if (Char.myCharz().clan != null && Char.myCharz().role < 2 && Char.myCharz().charFocus.clanID == -1)
			{
				vPlayerMenu.addElement(new Command(mResources.CHAR_ORDER[4], 110391));
			}
			if (Char.myCharz().charFocus.statusMe != 14 && Char.myCharz().charFocus.statusMe != 5)
			{
				if (Char.myCharz().taskMaint != null && Char.myCharz().taskMaint.taskId >= 14)
				{
					vPlayerMenu.addElement(new Command(mResources.CHAR_ORDER[0], 2003));
				}
			}
			else if (Char.myCharz().myskill.template.type != 4)
			{
			}
			if (Char.myCharz().clan != null && Char.myCharz().clan.ID == Char.myCharz().charFocus.clanID && Char.myCharz().charFocus.statusMe != 14 && Char.myCharz().taskMaint != null && Char.myCharz().taskMaint.taskId >= 14)
			{
				vPlayerMenu.addElement(new Command(mResources.CHAR_ORDER[1], 2004));
			}
			int num = Char.myCharz().nClass.skillTemplates.Length;
			for (int i = 0; i < num; i++)
			{
				SkillTemplate skillTemplate = Char.myCharz().nClass.skillTemplates[i];
				Skill skill = Char.myCharz().getSkill(skillTemplate);
				if (skill != null && skillTemplate.isBuffToPlayer() && skill.point >= 1)
				{
					vPlayerMenu.addElement(new Command(skillTemplate.name, 12004, skill));
				}
			}
		}
	private bool checkClipTopChatPopUp(int xClick, int yClick)
		{
			if (Equals(info2) && gI().popUpYesNo != null)
			{
				return false;
			}
			if (info2.info.info != null && info2.info.info.charInfo != null)
			{
				int num = 0;
				int num2 = 0;
				num = Res.abs(info2.cmx) + info2.info.X - 40;
				num2 = Res.abs(info2.cmy) + info2.info.Y;
				if (inRectangle(xClick - cmx, yClick - cmy, num, num2, 200, info2.info.H))
				{
					info2.doClick(10);
					return true;
				}
			}
			return false;
		}
	private bool checkClickToPopup(int xClick, int yClick)
		{
			for (int i = 0; i < PopUp.vPopups.size(); i++)
			{
				PopUp popUp = (PopUp)PopUp.vPopups.elementAt(i);
				if (inRectangle(xClick, yClick, popUp.cx, popUp.cy, popUp.cw, popUp.ch))
				{
					if (popUp.cy <= 24 && TileMap.isInAirMap() && Char.myCharz().cTypePk != 0)
					{
						return false;
					}
					if (popUp.isPaint)
					{
						popUp.doClick(10);
						return true;
					}
				}
			}
			return false;
		}
	public void checkMouseChat()
		{
			if (GameCanvas.isMouseFocus(xC, yC, 34, 34))
			{
				if (!TileMap.isOfflineMap())
				{
					mScreen.keyMouse = 15;
				}
			}
			else if (GameCanvas.isMouseFocus(xHP, yHP, 40, 40))
			{
				if (Char.myCharz().statusMe != 14)
				{
					mScreen.keyMouse = 10;
				}
			}
			else if (GameCanvas.isMouseFocus(xF, yF, 40, 40))
			{
				if (Char.myCharz().statusMe != 14)
				{
					mScreen.keyMouse = 5;
				}
			}
			else if (cmdMenu != null && GameCanvas.isMouseFocus(cmdMenu.x, cmdMenu.y, cmdMenu.w / 2, cmdMenu.h))
			{
				mScreen.keyMouse = 1;
			}
			else
			{
				mScreen.keyMouse = -1;
			}
		}
	public void initCreateCommand()
		{
		}
	public void updateXoSo()
		{
			if (tShow == 0)
			{
				return;
			}
			currXS = mSystem.currentTimeMillis();
			if (currXS - lastXS > 1000)
			{
				lastXS = mSystem.currentTimeMillis();
				secondXS++;
			}
			if (secondXS > 20)
			{
				for (int i = 0; i < winnumber.Length; i++)
				{
					randomNumber[i] = winnumber[i];
				}
				tShow--;
				if (tShow == 0)
				{
					yourNumber = string.Empty;
					info1.addInfo(strFinish, 0);
					secondXS = 0;
				}
				return;
			}
			if (moveIndex > winnumber.Length - 1)
			{
				tShow--;
				if (tShow == 0)
				{
					yourNumber = string.Empty;
					info1.addInfo(strFinish, 0);
				}
				return;
			}
			if (moveIndex < randomNumber.Length)
			{
				if (tMove[moveIndex] == 15)
				{
					if (randomNumber[moveIndex] == winnumber[moveIndex] - 1)
					{
						delayMove[moveIndex] = 10;
					}
					if (randomNumber[moveIndex] == winnumber[moveIndex])
					{
						tMove[moveIndex] = -1;
						moveIndex++;
					}
				}
				else if (GameCanvas.gameTick % 5 == 0)
				{
					tMove[moveIndex]++;
				}
			}
			for (int j = 0; j < winnumber.Length; j++)
			{
				if (tMove[j] == -1)
				{
					continue;
				}
				moveCount[j]++;
				if (moveCount[j] > tMove[j] + delayMove[j])
				{
					moveCount[j] = 0;
					randomNumber[j]++;
					if (randomNumber[j] >= 10)
					{
						randomNumber[j] = 0;
					}
				}
			}
		}
	public void updateKeyChatPopUp()
		{
		}
	public bool isRongThanMenu()
		{
			if (isMeCallRongThan)
			{
				return true;
			}
			return false;
		}
	public bool isPaintPopup()
		{
			if (isPaintItemInfo || isPaintInfoMe || isPaintStore || isPaintWeapon || isPaintNonNam || isPaintNonNu || isPaintAoNam || isPaintAoNu || isPaintGangTayNam || isPaintGangTayNu || isPaintQuanNam || isPaintQuanNu || isPaintGiayNam || isPaintGiayNu || isPaintLien || isPaintNhan || isPaintNgocBoi || isPaintPhu || isPaintStack || isPaintStackLock || isPaintGrocery || isPaintGroceryLock || isPaintUpGrade || isPaintConvert || isPaintSplit || isPaintUpPearl || isPaintBox || isPaintTrade || isPaintAlert || isPaintZone || isPaintTeam || isPaintClan || isPaintFindTeam || isPaintTask || isPaintFriend || isPaintEnemies || isPaintCharInMap || isPaintMessage)
			{
				return true;
			}
			return false;
		}
	public static void setPopupSize(int w, int h)
		{
			if (GameCanvas.w == 128 || GameCanvas.h <= 208)
			{
				w = 126;
				h = 160;
			}
			indexTitle = 0;
			popupW = w;
			popupH = h;
			popupX = gW2 - w / 2;
			popupY = gH2 - h / 2;
			if (GameCanvas.isTouch && !isPaintZone && !isPaintTeam && !isPaintClan && !isPaintCharInMap && !isPaintFindTeam && !isPaintFriend && !isPaintEnemies && !isPaintTask && !isPaintMessage)
			{
				if (GameCanvas.h <= 240)
				{
					popupY -= 10;
				}
				if (GameCanvas.isTouch && !GameCanvas.isTouchControlSmallScreen && GameCanvas.currentScreen is GameScr)
				{
					popupW = 310;
					popupX = gW / 2 - popupW / 2;
					if (isPaintInfoMe && indexMenu > 0)
					{
						popupW = w;
						popupX = gW2 - w / 2;
					}
				}
			}
			if (popupY < -10)
			{
				popupY = -10;
			}
			if (GameCanvas.h > 208 && popupY < 0)
			{
				popupY = 0;
			}
			if (GameCanvas.h == 208 && popupY < 10)
			{
				popupY = 10;
			}
		}
	public void onChatFromMe(string text, string to)
		{
			Res.outz("CHAT");
			if (!isPaintMessage || GameCanvas.isTouch)
			{
				ChatTextField.gI().isShow = false;
			}
			if (to.Equals(mResources.chat_player))
			{
				if (info2.playerID != Char.myCharz().charID)
				{
					Service.gI().chatPlayer(text, info2.playerID);
				}
			}
			else if (!text.Equals(string.Empty))
			{
				Service.gI().chat(text);
			}
		}
	public void onCancelChat()
		{
			if (isPaintMessage)
			{
				isPaintMessage = false;
				ChatTextField.gI().center = null;
			}
		}
	public void actMenu()
		{
			GameCanvas.panel.setTypeMain();
			GameCanvas.panel.show();
		}
	public void startYesNoPopUp(string info, Command cmdYes, Command cmdNo)
		{
			popUpYesNo = new PopUpYesNo();
			popUpYesNo.setPopUp(info, cmdYes, cmdNo);
		}
	public void player_vs_player(int playerId, int xu, string info, sbyte typePK)
		{
			Char @char = findCharInMap(playerId);
			if (@char != null)
			{
				if (typePK == 3)
				{
					startYesNoPopUp(info, new Command(mResources.OK, 2000, @char), new Command(mResources.CLOSE, 2009, @char));
				}
				if (typePK == 4)
				{
					startYesNoPopUp(info, new Command(mResources.OK, 2005, @char), new Command(mResources.CLOSE, 2009, @char));
				}
			}
		}

}
