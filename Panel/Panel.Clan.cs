using System;
using System.Collections.Generic;
using Assets.src.g;
using UnityEngine;

public partial class Panel : IActionListener, IChatable
{
	public int getXMap()
		{
			for (int i = 0; i < mapId[TileMap.planetID].Length; i++)
			{
				if (TileMap.mapID == mapId[TileMap.planetID][i])
				{
					return mapX[TileMap.planetID][i];
				}
			}
			return -1;
		}

	public int getYMap()
		{
			for (int i = 0; i < mapId[TileMap.planetID].Length; i++)
			{
				if (TileMap.mapID == mapId[TileMap.planetID][i])
				{
					return mapY[TileMap.planetID][i];
				}
			}
			return -1;
		}

	public int getXMapTask()
		{
			if (Char.myCharz().taskMaint == null)
			{
				return -1;
			}
			for (int i = 0; i < mapId[TileMap.planetID].Length; i++)
			{
				if (GameScr.mapTasks[Char.myCharz().taskMaint.index] == mapId[TileMap.planetID][i])
				{
					return mapX[TileMap.planetID][i];
				}
			}
			return -1;
		}

	public int getYMapTask()
		{
			if (Char.myCharz().taskMaint == null)
			{
				return -1;
			}
			for (int i = 0; i < mapId[TileMap.planetID].Length; i++)
			{
				if (GameScr.mapTasks[Char.myCharz().taskMaint.index] == mapId[TileMap.planetID][i])
				{
					return mapY[TileMap.planetID][i];
				}
			}
			return -1;
		}

	public void setTypeMapTrans()
		{
			type = 14;
			setType(0);
			setTabMapTrans();
			cmx = (cmtoX = 0);
		}

	public void setTypeMap()
		{
			if (!GameScr.gI().isMapFize() && isPaintMap)
			{
				if (Hint.isOnTask(2, 0))
				{
					Hint.isViewMap = true;
					GameScr.info1.addInfo(mResources.go_to_quest, 0);
				}
				if (Hint.isOnTask(3, 0))
				{
					Hint.isViewPotential = true;
				}
				type = 4;
				currentTabName = tabName[type];
				startTabPos = xScroll + wScroll / 2 - currentTabName.Length * TAB_W / 2;
				cmx = (cmtoX = 0);
				setTabMap();
			}
		}

	public void setTypeTop(sbyte t)
		{
			type = 15;
			setType(0);
			ITEM_HEIGHT = 24;
			selected = (GameCanvas.isTouch ? (-1) : 0);
			setTabTop();
			isThachDau = ((t != 0) ? true : false);
		}

	public void setTypePetMain()
		{
			type = 21;
			if (GameCanvas.panel2 != null)
			{
				boxPet = mResources.petMainTab2;
			}
			else
			{
				boxPet = mResources.petMainTab;
			}
			tabName[21] = boxPet;
			if (Char.myCharz().cgender == 1)
			{
				strStatus = new string[6]
				{
					mResources.follow,
					mResources.defend,
					mResources.attack,
					mResources.gohome,
					mResources.fusion,
					mResources.fusionForever
				};
			}
			else
			{
				strStatus = new string[5]
				{
					mResources.follow,
					mResources.defend,
					mResources.attack,
					mResources.gohome,
					mResources.fusion
				};
			}
			setType(2);
			if (currentTabIndex == 0)
			{
				setTabPetInventory();
			}
			if (currentTabIndex == 1)
			{
				setTabPetStatus();
			}
			if (currentTabIndex == 2)
			{
				setTabInventory(resetSelect: true);
			}
		}

	public void setTypeZone()
		{
			type = 3;
			setType(0);
			setTabZone();
			cmx = (cmtoX = 0);
		}

	public void addClanMemberDetail(Member m)
		{
			string text = "|0|1|" + m.name;
			string text2 = "\n|2|1|";
			if (m.role == 0)
			{
				text2 = "\n|7|1|";
			}
			if (m.role == 1)
			{
				text2 = "\n|1|1|";
			}
			if (m.role == 2)
			{
				text2 = "\n|0|1|";
			}
			text = text + text2 + Member.getRole(m.role);
			string text3 = text;
			text = text3 + "\n|2|1|" + mResources.power + ": " + m.powerPoint;
			text += "\n--";
			text3 = text;
			text = text3 + "\n|5|" + mResources.clan_capsuledonate + ": " + m.clanPoint;
			text3 = text;
			text = text3 + "\n|5|" + mResources.clan_capsuleself + ": " + m.curClanPoint;
			text3 = text;
			text = text3 + "\n|4|" + mResources.give_pea + ": " + m.donate + mResources.time;
			text3 = text;
			text = text3 + "\n|4|" + mResources.receive_pea + ": " + m.receive_donate + mResources.time;
			text3 = text;
			text = text3 + "\n|6|" + mResources.join_date + ": " + m.joinTime;
			cp = new ChatPopup();
			popUpDetailInit(cp, text);
			partID = new int[3] { m.head, m.leg, m.body };
			currItem = null;
			charInfo = null;
		}

	public void addClanDetail(Clan cl)
		{
			try
			{
				string text = "|0|" + cl.name;
				string[] array = mFont.tahoma_7_green.splitFontArray(cl.slogan, wScroll - 60);
				for (int i = 0; i < array.Length; i++)
				{
					text = text + "\n|2|" + array[i];
				}
				text += "\n--";
				string text2 = text;
				text = text2 + "\n|7|" + mResources.clan_leader + ": " + cl.leaderName;
				text2 = text;
				text = text2 + "\n|1|" + mResources.power_point + ": " + cl.powerPoint;
				text2 = text;
				text = text2 + "\n|4|" + mResources.member + ": " + cl.currMember + "/" + cl.maxMember;
				text2 = text;
				text = text2 + "\n|4|" + mResources.level + ": " + cl.level;
				text2 = text;
				text = text2 + "\n|4|" + mResources.clan_birthday + ": " + NinjaUtil.getDate(cl.date);
				cp = new ChatPopup();
				popUpDetailInit(cp, text);
				idIcon = ClanImage.getClanImage((short)cl.imgID).idImage[0];
				currItem = null;
			}
			catch (Exception ex)
			{
				Res.outz("Throw  exception " + ex.StackTrace);
			}
		}

	private void updateKeyPetStatus()
		{
			updateKeyScrollView();
		}

	private void updateKeyPetSkill()
		{
		}

	private void updateKeyClanIcon()
		{
			updateKeyScrollView();
		}

	private void updateKeyMap()
		{
			if (GameCanvas.keyHold[(!Main.isPC) ? 2 : 21])
			{
				yMove -= 5;
				cmyMap = yMove - (yScroll + hScroll / 2);
				if (yMove < yScroll)
				{
					yMove = yScroll;
				}
			}
			if (GameCanvas.keyHold[(!Main.isPC) ? 8 : 22])
			{
				yMove += 5;
				cmyMap = yMove - (yScroll + hScroll / 2);
				if (yMove > yScroll + 200)
				{
					yMove = yScroll + 200;
				}
			}
			if (GameCanvas.keyHold[(!Main.isPC) ? 4 : 23])
			{
				xMove -= 5;
				cmxMap = xMove - wScroll / 2;
				if (xMove < 16)
				{
					xMove = 16;
				}
			}
			if (GameCanvas.keyHold[(!Main.isPC) ? 6 : 24])
			{
				xMove += 5;
				cmxMap = xMove - wScroll / 2;
				if (xMove > 250)
				{
					xMove = 250;
				}
			}
			if (GameCanvas.isPointerDown)
			{
				pointerIsDowning = true;
				if (!trans)
				{
					pa1 = cmxMap;
					pa2 = cmyMap;
					trans = true;
				}
				cmxMap = pa1 + (GameCanvas.pxLast - GameCanvas.px);
				cmyMap = pa2 + (GameCanvas.pyLast - GameCanvas.py);
			}
			if (GameCanvas.isPointerJustRelease)
			{
				trans = false;
				GameCanvas.pxLast = GameCanvas.px;
				GameCanvas.pyLast = GameCanvas.py;
				pX = GameCanvas.pxLast + cmxMap;
				pY = GameCanvas.pyLast + cmyMap;

				if (TileMap.planetID >= 0 && TileMap.planetID < mapX.Length)
				{
					for (int k = 0; k < mapX[TileMap.planetID].Length; k++)
					{
						int num4 = mapX[TileMap.planetID][k] + xScroll;
						int num5 = mapY[TileMap.planetID][k] + yScroll;
						if (Res.inRect(num4 - 20, num5 - 20, 40, 40, pX, pY))
						{
							int targetMapId = mapId[TileMap.planetID][k];
							if (targetMapId == TileMap.mapID)
							{
								GameScr.info1.addInfo("Bạn đang ở map này!", 0);
							}
							else
							{
								hide();
								ModNextMap.StartNextMap(targetMapId);
							}
							GameCanvas.clearAllPointerEvent();
							break;
						}
					}
				}
			}
			if (GameCanvas.isPointerClick)
			{
				pointerIsDowning = false;
			}
			if (cmxMap < 0)
			{
				cmxMap = 0;
			}
			if (cmxMap > cmxMapLim)
			{
				cmxMap = cmxMapLim;
			}
			if (cmyMap < 0)
			{
				cmyMap = 0;
			}
			if (cmyMap > cmyMapLim)
			{
				cmyMap = cmyMapLim;
			}
		}

	private void getCurrClanOtion()
		{
			isClanOption = false;
			if (type != 0 || mainTabName.Length != 5 || currentTabIndex != 3)
			{
				return;
			}
			isClanOption = false;
			if (selected == 0)
			{
				currClanOption = new int[clansOption.Length];
				for (int i = 0; i < currClanOption.Length; i++)
				{
					currClanOption[i] = i;
				}
				if (!isViewMember)
				{
					isClanOption = true;
				}
			}
			else if (selected != 1 && !isSearchClan && selected > 0)
			{
				currClanOption = new int[1];
				for (int j = 0; j < currClanOption.Length; j++)
				{
					currClanOption[j] = j;
				}
				isClanOption = true;
			}
		}

	private void updateKeyClansOption()
		{
			if (currClanOption == null)
			{
				return;
			}
			if (GameCanvas.keyPressed[(!Main.isPC) ? 4 : 23])
			{
				currMess = getCurrMessage();
				cSelected--;
				if (selected == 0 && cSelected < 0)
				{
					cSelected = currClanOption.Length - 1;
				}
				if (selected > 1 && isMessage && currMess.option != null && cSelected < 0)
				{
					cSelected = currMess.option.Length - 1;
				}
			}
			else if (GameCanvas.keyPressed[(!Main.isPC) ? 6 : 24])
			{
				currMess = getCurrMessage();
				cSelected++;
				if (selected == 0 && cSelected > currClanOption.Length - 1)
				{
					cSelected = 0;
				}
				if (selected > 1 && isMessage && currMess.option != null && cSelected > currMess.option.Length - 1)
				{
					cSelected = 0;
				}
			}
		}

	private void updateKeyClans()
		{
			updateKeyScrollView();
			updateKeyClansOption();
		}

	private void setTabPetStatus()
		{
			currentListLength = strStatus.Length;
			ITEM_HEIGHT = 24;
			selected = (GameCanvas.isTouch ? (-1) : 0);
			cmyLim = currentListLength * ITEM_HEIGHT - hScroll;
			if (cmyLim < 0)
			{
				cmyLim = 0;
			}
			cmy = (cmtoY = cmyLast[currentTabIndex]);
			if (cmy < 0)
			{
				cmy = (cmtoY = 0);
			}
			if (cmy > cmyLim)
			{
				cmy = (cmtoY = cmyLim);
			}
		}

	private void setTabPetSkill()
		{
		}

	public void initTabClans()
		{
			if (isSearchClan)
			{
				currentListLength = ((clans != null) ? (clans.Length + 2) : 2);
				clanInfo = mResources.clan_list;
			}
			else if (isViewMember)
			{
				clanReport = string.Empty;
				currentListLength = ((member != null) ? member.size() : myMember.size()) + 2;
				clanInfo = mResources.member + " " + ((currClan == null) ? Char.myCharz().clan.name : currClan.name);
			}
			else if (isMessage)
			{
				currentListLength = ClanMessage.vMessage.size() + 2;
				clanInfo = mResources.msg;
				clanReport = string.Empty;
			}
			if (Char.myCharz().clan == null)
			{
				clansOption = new string[2][]
				{
					mResources.findClan,
					mResources.createClan
				};
			}
			else if (!isViewMember)
			{
				if (myMember.size() > 1)
				{
					clansOption = new string[3][]
					{
						mResources.chatClan,
						mResources.request_pea2,
						mResources.memberr
					};
				}
				else
				{
					clansOption = new string[1][] { mResources.memberr };
				}
			}
			else if (Char.myCharz().role > 0)
			{
				clansOption = new string[2][]
				{
					mResources.msgg,
					mResources.leaveClan
				};
			}
			else if (myMember.size() > 1)
			{
				clansOption = new string[4][]
				{
					mResources.msgg,
					mResources.leaveClan,
					mResources.khau_hieuu,
					mResources.bieu_tuongg
				};
			}
			else
			{
				clansOption = new string[3][]
				{
					mResources.msgg,
					mResources.khau_hieuu,
					mResources.bieu_tuongg
				};
			}
			cmyLim = currentListLength * ITEM_HEIGHT - hScroll;
			if (cmyLim < 0)
			{
				cmyLim = 0;
			}
			cmy = (cmtoY = cmyLast[currentTabIndex]);
			if (cmy < 0)
			{
				cmy = (cmtoY = 0);
			}
			if (cmy > cmyLim)
			{
				cmy = (cmtoY = cmyLim);
			}
		}

	public void setTabClans()
		{
			GameScr.isNewClanMessage = false;
			ITEM_HEIGHT = 24;
			if (lastSelect != null && lastSelect[3] == 0)
			{
				lastSelect[3] = -1;
			}
			currentListLength = 2;
			if (Char.myCharz().clan != null)
			{
				isMessage = true;
				isViewMember = false;
				isSearchClan = false;
			}
			else
			{
				isMessage = false;
				isViewMember = false;
				isSearchClan = true;
			}
			if (Char.myCharz().clan != null)
			{
				currentListLength = ClanMessage.vMessage.size() + 2;
			}
			initTabClans();
			cSelected = -1;
			if (chatTField == null)
			{
				chatTField = new ChatTextField();
				chatTField.tfChat.y = GameCanvas.h - 35 - ChatTextField.gI().tfChat.height;
				chatTField.initChatTextField();
				chatTField.parentScreen = GameCanvas.panel;
			}
			if (Char.myCharz().clan == null)
			{
				clanReport = mResources.findingClan;
				Service.gI().searchClan(string.Empty);
			}
			selected = lastSelect[currentTabIndex];
			if (GameCanvas.isTouch)
			{
				selected = -1;
			}
		}

	private void setTabMapTrans()
		{
			ITEM_HEIGHT = 24;
			currentListLength = mapNames.Length;
			cmyLim = currentListLength * ITEM_HEIGHT - hScroll;
			cmy = (cmtoY = 0);
			selected = (GameCanvas.isTouch ? (-1) : 0);
		}

	private void setTabZone()
		{
			ITEM_HEIGHT = 24;
			currentListLength = GameScr.gI().zones.Length;
			cmyLim = currentListLength * ITEM_HEIGHT - hScroll;
			cmy = (cmtoY = 0);
			selected = (GameCanvas.isTouch ? (-1) : 0);
		}

	private void setTabPetInventory()
		{
			ITEM_HEIGHT = 30;
			Item[] arrItemBody = Char.myPetz().arrItemBody;
			Skill[] arrPetSkill = Char.myPetz().arrPetSkill;
			currentListLength = arrItemBody.Length + arrPetSkill.Length;
			cmyLim = currentListLength * ITEM_HEIGHT - hScroll;
			cmy = (cmtoY = cmyLast[currentTabIndex]);
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
				cmy = (cmtoY = 0);
			}
			selected = (GameCanvas.isTouch ? (-1) : 0);
		}

	private void setTabMap()
		{
			if (!isPaintMap)
			{
				return;
			}
			if (TileMap.lastPlanetId != TileMap.planetID)
			{
				Res.outz("LOAD TAM HINH");
				imgMap = GameCanvas.loadImageRMS("/img/map" + TileMap.planetID + ".png");
				TileMap.lastPlanetId = TileMap.planetID;
			}
			cmxMap = getXMap() - wScroll / 2;
			cmyMap = getYMap() + yScroll - (yScroll + hScroll / 2);
			pa1 = cmxMap;
			pa2 = cmyMap;
			cmxMapLim = 250 - wScroll;
			cmyMapLim = 220 - hScroll;
			if (cmxMapLim < 0)
			{
				cmxMapLim = 0;
			}
			if (cmyMapLim < 0)
			{
				cmyMapLim = 0;
			}
			for (int i = 0; i < mapId[TileMap.planetID].Length; i++)
			{
				if (TileMap.mapID == mapId[TileMap.planetID][i])
				{
					xMove = mapX[TileMap.planetID][i] + xScroll;
					yMove = mapY[TileMap.planetID][i] + yScroll + 5;
					break;
				}
			}
			xMap = getXMap() + xScroll;
			yMap = getYMap() + yScroll;
			xMapTask = getXMapTask() + xScroll;
			yMapTask = getYMapTask() + yScroll;
			Resources.UnloadUnusedAssets();
			GC.Collect();
		}

	private void searchClan()
		{
			chatTField.strChat = mResources.input_clan_name;
			chatTField.tfChat.name = mResources.clan_name;
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

	public void creatClan()
		{
			chatTField.strChat = mResources.input_clan_name_to_create;
			chatTField.tfChat.name = mResources.input_clan_name;
			chatTField.to = string.Empty;
			chatTField.isShow = true;
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

}
