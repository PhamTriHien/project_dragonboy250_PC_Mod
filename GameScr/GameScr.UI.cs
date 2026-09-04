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

	public void giaodich(int playerID)
		{
			Char @char = findCharInMap(playerID);
			if (@char != null)
			{
				startYesNoPopUp(@char.cName + mResources.want_to_trade, new Command(mResources.YES, 11114, @char), new Command(mResources.NO, 2009, @char));
			}
		}

	public void actionPerform(int idAction, object p)
		{
			Cout.println("PERFORM WITH ID = " + idAction);
			switch (idAction)
			{
			case 888351:
				Service.gI().petStatus(5);
				GameCanvas.endDlg();
				break;
			case 11112:
			{
				Char @char = (Char)p;
				Service.gI().friend(1, @char.charID);
				break;
			}
			case 11113:
			{
				Char char2 = (Char)p;
				if (char2 != null)
				{
					Service.gI().giaodich(0, char2.charID, -1, -1);
				}
				break;
			}
			case 11114:
			{
				popUpYesNo = null;
				GameCanvas.endDlg();
				Char char3 = (Char)p;
				if (char3 != null)
				{
					Service.gI().giaodich(1, char3.charID, -1, -1);
				}
				break;
			}
			case 11111:
				if (Char.myCharz().charFocus != null)
				{
					InfoDlg.showWait();
					if (GameCanvas.panel.vPlayerMenu.size() <= 0)
					{
						playerMenu(Char.myCharz().charFocus);
					}
					GameCanvas.panel.setTypePlayerMenu(Char.myCharz().charFocus);
					GameCanvas.panel.show();
					Service.gI().getPlayerMenu(Char.myCharz().charFocus.charID);
					Service.gI().messagePlayerMenu(Char.myCharz().charFocus.charID);
				}
				break;
			case 11115:
				if (Char.myCharz().charFocus != null)
				{
					InfoDlg.showWait();
					Service.gI().playerMenuAction(Char.myCharz().charFocus.charID, (short)Char.myCharz().charFocus.menuSelect);
				}
				break;
			case 2000:
				popUpYesNo = null;
				GameCanvas.endDlg();
				if ((Char)p == null)
				{
					Service.gI().player_vs_player(1, 3, -1);
					break;
				}
				Service.gI().player_vs_player(1, 3, ((Char)p).charID);
				Service.gI().charMove();
				break;
			case 2001:
				GameCanvas.endDlg();
				break;
			case 2003:
				GameCanvas.endDlg();
				InfoDlg.showWait();
				Service.gI().player_vs_player(0, 3, Char.myCharz().charFocus.charID);
				break;
			case 2004:
				GameCanvas.endDlg();
				Service.gI().player_vs_player(0, 4, Char.myCharz().charFocus.charID);
				break;
			case 2005:
				GameCanvas.endDlg();
				popUpYesNo = null;
				if ((Char)p == null)
				{
					Service.gI().player_vs_player(1, 4, -1);
				}
				else
				{
					Service.gI().player_vs_player(1, 4, ((Char)p).charID);
				}
				break;
			case 2009:
				popUpYesNo = null;
				break;
			case 2006:
				GameCanvas.endDlg();
				Service.gI().player_vs_player(2, 4, Char.myCharz().charFocus.charID);
				break;
			case 2007:
				GameCanvas.endDlg();
				GameMidlet.instance.exit();
				break;
			case 11038:
				actDead();
				break;
			case 110382:
				Service.gI().returnTownFromDead();
				break;
			case 110383:
				Service.gI().wakeUpFromDead();
				break;
			case 1:
				GameCanvas.endDlg();
				break;
			case 2:
				GameCanvas.menu.showMenu = false;
				break;
			case 8002:
				doFire(isFireByShortCut: false, skipWaypoint: true);
				GameCanvas.clearKeyHold();
				GameCanvas.clearKeyPressed();
				break;
			case 11057:
			{
				Effect2.vEffect2Outside.removeAllElements();
				Effect2.vEffect2.removeAllElements();
				Npc npc = (Npc)p;
				if (npc.idItem == 0)
				{
					Service.gI().confirmMenu((short)npc.template.npcTemplateId, (sbyte)GameCanvas.menu.menuSelectedItem);
				}
				else if (GameCanvas.menu.menuSelectedItem == 0)
				{
					Service.gI().pickItem(npc.idItem);
				}
				break;
			}
			case 11000:
				actMenu();
				break;
			case 11001:
				Char.myCharz().findNextFocusByKey();
				break;
			case 11002:
				GameCanvas.panel.hide();
				break;
			case 11120:
			{
				object[] array2 = (object[])p;
				Skill skill4 = (Skill)array2[0];
				int num2 = int.Parse((string)array2[1]);
				for (int j = 0; j < onScreenSkill.Length; j++)
				{
					if (onScreenSkill[j] == skill4)
					{
						onScreenSkill[j] = null;
					}
				}
				onScreenSkill[num2] = skill4;
				saveonScreenSkillToRMS();
				break;
			}
			case 11121:
			{
				object[] array = (object[])p;
				Skill skill3 = (Skill)array[0];
				int num = int.Parse((string)array[1]);
				for (int i = 0; i < keySkill.Length; i++)
				{
					if (keySkill[i] == skill3)
					{
						keySkill[i] = null;
					}
				}
				keySkill[num] = skill3;
				saveKeySkillToRMS();
				break;
			}
			case 110001:
				GameCanvas.panel.setTypeMain();
				GameCanvas.panel.show();
				break;
			case 110004:
				GameCanvas.menu.showMenu = false;
				break;
			case 11067:
				if (TileMap.zoneID != indexSelect)
				{
					Service.gI().requestChangeZone(indexSelect, indexItemUse);
					InfoDlg.showWait();
				}
				else
				{
					info1.addInfo(mResources.ZONE_HERE, 0);
				}
				break;
			case 11059:
			{
				Skill skill2 = onScreenSkill[selectedIndexSkill];
				doUseSkill(skill2, isShortcut: false);
				center = null;
				break;
			}
			case 12000:
				Service.gI().getClan(1, -1, null);
				break;
			case 12001:
				GameCanvas.endDlg();
				break;
			case 12002:
			{
				GameCanvas.endDlg();
				ClanObject clanObject = (ClanObject)p;
				Service.gI().clanInvite(1, -1, clanObject.clanID, clanObject.code);
				popUpYesNo = null;
				break;
			}
			case 12003:
			{
				ClanObject clanObject = (ClanObject)p;
				GameCanvas.endDlg();
				Service.gI().clanInvite(2, -1, clanObject.clanID, clanObject.code);
				popUpYesNo = null;
				break;
			}
			case 12004:
			{
				Skill skill = (Skill)p;
				doUseSkill(skill, isShortcut: true);
				Char.myCharz().saveLoadPreviousSkill();
				break;
			}
			case 110391:
				Service.gI().clanInvite(0, Char.myCharz().charFocus.charID, -1, -1);
				break;
			case 12005:
				if (GameCanvas.serverScr == null)
				{
					GameCanvas.serverScr = new ServerScr();
				}
				GameCanvas.serverScr.switchToMe();
				GameCanvas.endDlg();
				break;
			case 12006:
				GameMidlet.instance.exit();
				break;
			}
		}

	public void chatVip(string chatVip)
		{
			if (!startChat)
			{
				currChatWidth = mFont.tahoma_7b_yellowSmall.getWidth(chatVip);
				xChatVip = GameCanvas.w;
				startChat = true;
			}
			if (chatVip.StartsWith("!"))
			{
				chatVip = chatVip.Substring(1, chatVip.Length);
				isFireWorks = true;
			}
			vChatVip.addElement(chatVip);
		}

	public void clearChatVip()
		{
			vChatVip.removeAllElements();
			xChatVip = GameCanvas.w;
			startChat = false;
		}

	public void updateChatVip()
		{
			if (!startChat)
			{
				return;
			}
			xChatVip -= 2;
			if (xChatVip < -currChatWidth)
			{
				xChatVip = GameCanvas.w;
				vChatVip.removeElementAt(0);
				if (vChatVip.size() == 0)
				{
					isFireWorks = false;
					startChat = false;
				}
				else
				{
					currChatWidth = mFont.tahoma_7b_white.getWidth((string)vChatVip.elementAt(0));
				}
			}
		}

	public static void StartServerPopUp(string strMsg)
		{
			GameCanvas.endDlg();
			int avatar = 1139;
			Npc npc = new Npc(-1, 0, 0, 0, 0, 0);
			npc.avatar = avatar;
			ChatPopup.addBigMessage(strMsg, 100000, npc);
			ChatPopup.serverChatPopUp.cmdMsg1 = new Command(mResources.CLOSE, ChatPopup.serverChatPopUp, 1001, null);
			ChatPopup.serverChatPopUp.cmdMsg1.x = GameCanvas.w / 2 - 35;
			ChatPopup.serverChatPopUp.cmdMsg1.y = GameCanvas.h - 35;
		}

	public void paintPhuBanBar(mGraphics g, int x, int y, int w)
		{
			if (phuban_Info == null || isPaintOther || isPaintRada != 1 || GameCanvas.panel.isShow || !ispaintPhubangBar())
			{
				return;
			}
			if (w < fra_PVE_Bar_1.frameWidth + fra_PVE_Bar_0.frameWidth * 4)
			{
				w = fra_PVE_Bar_1.frameWidth + fra_PVE_Bar_0.frameWidth * 4;
			}
			if (x > GameCanvas.w - w / 2)
			{
				x = GameCanvas.w - w / 2;
			}
			if (x < mGraphics.getImageWidth(imgKhung) + w / 2 + 10)
			{
				x = mGraphics.getImageWidth(imgKhung) + w / 2 + 10;
			}
			int frameHeight = fra_PVE_Bar_0.frameHeight;
			int num = y + frameHeight + mGraphics.getImageHeight(imgBall) / 2 + 2;
			int frameWidth = fra_PVE_Bar_1.frameWidth;
			int num2 = w / 2 - frameWidth / 2;
			int num3 = x - w / 2;
			int num4 = x + frameWidth / 2;
			int y2 = y + 3;
			int num5 = num2 - fra_PVE_Bar_0.frameWidth;
			int num6 = num5 / fra_PVE_Bar_0.frameWidth;
			if (num5 % fra_PVE_Bar_0.frameWidth > 0)
			{
				num6++;
			}
			for (int i = 0; i < num6; i++)
			{
				if (i < num6 - 1)
				{
					fra_PVE_Bar_0.drawFrame(1, num3 + fra_PVE_Bar_0.frameWidth + i * fra_PVE_Bar_0.frameWidth, y2, 0, 0, g);
				}
				else
				{
					fra_PVE_Bar_0.drawFrame(1, num3 + num5, y2, 0, 0, g);
				}
				if (i < num6 - 1)
				{
					fra_PVE_Bar_0.drawFrame(1, num4 + i * fra_PVE_Bar_0.frameWidth, y2, 0, 0, g);
				}
				else
				{
					fra_PVE_Bar_0.drawFrame(1, num4 + num5 - fra_PVE_Bar_0.frameWidth, y2, 0, 0, g);
				}
			}
			fra_PVE_Bar_0.drawFrame(0, num3, y2, 2, 0, g);
			fra_PVE_Bar_0.drawFrame(0, num4 + num5, y2, 0, 0, g);
			if (phuban_Info.pointTeam1 > 0)
			{
				int idx = 2;
				int idx2 = 3;
				if (phuban_Info.color_1 == 4)
				{
					idx = 4;
					idx2 = 5;
				}
				int num7 = phuban_Info.pointTeam1 * num2 / phuban_Info.maxPoint;
				if (num7 < 0)
				{
					num7 = 0;
				}
				if (num7 > num2)
				{
					num7 = num2;
				}
				g.setClip(num3 + num2 - num7, y2, num7, frameHeight);
				for (int j = 0; j < num6; j++)
				{
					if (j < num6 - 1)
					{
						fra_PVE_Bar_0.drawFrame(idx2, num3 + fra_PVE_Bar_0.frameWidth + j * fra_PVE_Bar_0.frameWidth, y2, 0, 0, g);
					}
					else
					{
						fra_PVE_Bar_0.drawFrame(idx2, num3 + num5, y2, 0, 0, g);
					}
				}
				fra_PVE_Bar_0.drawFrame(idx, num3, y2, 2, 0, g);
				GameCanvas.resetTrans(g);
			}
			if (phuban_Info.pointTeam2 > 0)
			{
				int idx3 = 2;
				int idx4 = 3;
				if (phuban_Info.color_2 == 4)
				{
					idx3 = 4;
					idx4 = 5;
				}
				int num8 = phuban_Info.pointTeam2 * num2 / phuban_Info.maxPoint;
				if (num8 < 0)
				{
					num8 = 0;
				}
				if (num8 > num2)
				{
					num8 = num2;
				}
				g.setClip(num4, y2, num8, frameHeight);
				for (int k = 0; k < num6; k++)
				{
					if (k < num6 - 1)
					{
						fra_PVE_Bar_0.drawFrame(idx4, num4 + k * fra_PVE_Bar_0.frameWidth, y2, 0, 0, g);
					}
					else
					{
						fra_PVE_Bar_0.drawFrame(idx4, num4 + num5 - fra_PVE_Bar_0.frameWidth, y2, 0, 0, g);
					}
				}
				fra_PVE_Bar_0.drawFrame(idx3, num4 + num5, y2, 0, 0, g);
				GameCanvas.resetTrans(g);
			}
			fra_PVE_Bar_1.drawFrame(0, x - frameWidth / 2, y, 0, 0, g);
			string timeCountDown = mSystem.getTimeCountDown(phuban_Info.timeStart, phuban_Info.timeSecond, isOnlySecond: true, isShortText: false);
			mFont.tahoma_7b_yellow.drawString(g, timeCountDown, x + 1, y + fra_PVE_Bar_1.frameHeight / 2 - mFont.tahoma_7b_green2.getHeight() / 2, 2);
			Panel.setTextColor(phuban_Info.color_1, 1).drawString(g, phuban_Info.nameTeam1, x - 5, num + 5, 1);
			Panel.setTextColor(phuban_Info.color_2, 1).drawString(g, phuban_Info.nameTeam2, x + 5, num + 5, 0);
			if (phuban_Info.type_PB != 0)
			{
				int y3 = y + frameHeight / 2 - 2;
				mFont.bigNumber_While.drawString(g, string.Empty + phuban_Info.pointTeam1, num3 + num2 / 2, y3, 2);
				mFont.bigNumber_While.drawString(g, string.Empty + phuban_Info.pointTeam2, num4 + num2 / 2, y3, 2);
			}
			g.drawImage(imgVS, x, y + fra_PVE_Bar_1.frameHeight + 2, 3);
			if (phuban_Info.type_PB == 0)
			{
				paintChienTruong_Life(g, phuban_Info.maxLife, phuban_Info.color_1, phuban_Info.lifeTeam1, x - 13, phuban_Info.color_2, phuban_Info.lifeTeam2, x + 13, num);
			}
		}

	public static void paintChienTruong_Life(mGraphics g, int maxLife, int cl1, int lifeTeam1, int x1, int cl2, int lifeTeam2, int x2, int y)
		{
			if (imgBall == null)
			{
				return;
			}
			int num = mGraphics.getImageHeight(imgBall) / 2;
			for (int i = 0; i < maxLife; i++)
			{
				int num2 = 0;
				if (i < lifeTeam1)
				{
					num2 = 1;
				}
				g.drawRegion(imgBall, 0, num2 * num, mGraphics.getImageWidth(imgBall), num, 0, x1 - i * (num + 1), y, mGraphics.VCENTER | mGraphics.HCENTER);
			}
			for (int j = 0; j < maxLife; j++)
			{
				int num3 = 0;
				if (j < lifeTeam2)
				{
					num3 = 1;
				}
				g.drawRegion(imgBall, 0, num3 * num, mGraphics.getImageWidth(imgBall), num, 0, x2 + j * (num + 1), y, mGraphics.VCENTER | mGraphics.HCENTER);
			}
		}

	private void paint_board_CT(mGraphics g, int x, int y)
		{
			if (!is_Paint_boardCT_Expand)
			{
				string s = "#01 nnnnnnnnnnnn";
				int width = mFont.tahoma_7.getWidth(s);
				int num = GameCanvas.w - width - 20;
				for (int i = 0; i < nTop; i++)
				{
					mFont mFont2 = mFont.tahoma_7_white;
					switch (i)
					{
					case 0:
						mFont2 = mFont.tahoma_7_red;
						break;
					case 1:
						mFont2 = mFont.tahoma_7_yellow;
						break;
					case 2:
						mFont2 = mFont.tahoma_7_blue;
						break;
					}
					if (i == nTop - 1)
					{
						mFont2 = mFont.tahoma_7_green;
					}
					string[] array = Res.split((string)res_CT.elementAt(i), "|", 0);
					int[] array2 = new int[2] { 0, 18 };
					for (int j = 0; j < 2; j++)
					{
						mFont2.drawString(g, array[j], num + array2[j], y + i * mFont.tahoma_7.getHeight(), 0, mFont.tahoma_7);
					}
				}
				GameCanvas.resetTrans(g);
				xRect = num;
				yRect = y;
				wRect = width + 10;
				hRect = mFont.tahoma_7b_dark.getHeight() * 6;
			}
			else
			{
				string s2 = "#01 namec1000000 0001   00000";
				int[] array3 = new int[4] { 0, 18, 80, 101 };
				int width2 = mFont.tahoma_7.getWidth(s2);
				int num2 = GameCanvas.w - width2 - 20;
				int num3 = y;
				for (int k = 0; k < nTop; k++)
				{
					string[] array4 = Res.split((string)res_CT.elementAt(k), "|", 0);
					mFont mFont3 = mFont.tahoma_7_white;
					switch (k)
					{
					case 0:
						mFont3 = mFont.tahoma_7_red;
						break;
					case 1:
						mFont3 = mFont.tahoma_7_yellow;
						break;
					case 2:
						mFont3 = mFont.tahoma_7_blue;
						break;
					}
					if (k == nTop - 1)
					{
						mFont3 = mFont.tahoma_7_green;
					}
					num3 = k * mFont.tahoma_7_white.getHeight() + y;
					for (int l = 0; l < array3.Length; l++)
					{
						mFont3.drawString(g, array4[l], num2 + array3[l], num3, 0, mFont.tahoma_7);
					}
				}
				xRect = num2;
				yRect = y;
				wRect = width2 + 10;
				hRect = mFont.tahoma_7b_dark.getHeight() * 6;
			}
			GameCanvas.resetTrans(g);
		}

	private void paintHPCT(mGraphics g, int x, int y, Char c)
		{
			g.drawImage(imgKhung, x, y, 0);
			int x2 = x + 3;
			int num = y + 19;
			int num2 = 0;
			int num3 = 0;
			int width = imgHP_NEW.getWidth();
			int num4 = imgHP_NEW.getHeight() / 2;
			num2 = (int)(c.cHP * width / c.cHPFull);
			if (num2 <= 0)
			{
				num2 = 1;
			}
			else if (num2 > width)
			{
				num2 = width;
			}
			g.drawRegion(imgHP_NEW, 0, num4, 80, num4, 0, x2, num, 0);
			num3 = (int)(c.cMP * width / c.cMPFull);
			if (num3 <= 0)
			{
				num3 = 1;
			}
			else if (num3 > width)
			{
				num3 = width;
			}
			g.drawRegion(imgHP_NEW, 0, 0, 80, num4, 0, x2, num + 6, 0);
		}

}
