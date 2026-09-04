using System;
using Assets.src.g;
public partial class GameScr : mScreen, IChatable
{
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

}
