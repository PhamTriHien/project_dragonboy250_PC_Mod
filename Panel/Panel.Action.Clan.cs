using System;
using System.Collections.Generic;
using Assets.src.g;
using UnityEngine;

public partial class Panel : IActionListener, IChatable
{
	private void doFirePetMain()
			{
				if (currentTabIndex == 0)
				{
					if (selected == -1 || selected > Char.myPetz().arrItemBody.Length - 1)
					{
						return;
					}
					MyVector myVector = new MyVector(string.Empty);
					Item item = Char.myPetz().arrItemBody[selected];
					currItem = item;
					if (currItem != null)
					{
						myVector.addElement(new Command(mResources.MOVEOUT, this, 2006, currItem));
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
					doFirePetStatus();
				}
				if (currentTabIndex == 2)
				{
					doFireInventory();
				}
			}

	private void doFirePetStatus()
			{
				if (selected == -1)
				{
					return;
				}
				if (selected == 5)
				{
					GameCanvas.startYesNoDlg(mResources.sure_fusion, new Command(mResources.YES, 888351), new Command(mResources.NO, 2001));
					return;
				}
				Service.gI().petStatus((sbyte)selected);
				if (selected < 4)
				{
					Char.myPetz().petStatus = (sbyte)selected;
				}
			}

	private void doFireMapTrans()
			{
				doFireZone();
			}

	private void doFirePet()
			{
				InfoDlg.showWait();
				Service.gI().petInfo();
				timeShow = 20;
			}

	private void chatClan()
			{
				chatTField.strChat = mResources.chat_clan;
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
			}

	private void doFireClanOption()
			{
				try
				{
					partID = null;
					charInfo = null;
					Res.outz("cSelect= " + cSelected);
					if (selected < 0)
					{
						cSelected = -1;
						return;
					}
					if (Char.myCharz().clan == null)
					{
						if (selected == 0)
						{
							if (cSelected == 0)
							{
								searchClan();
							}
							else if (cSelected == 1)
							{
								InfoDlg.showWait();
								creatClan();
								Service.gI().getClan(1, -1, null);
							}
						}
						else if (selected != -1)
						{
							if (selected == 1)
							{
								if (isSearchClan)
								{
									Service.gI().searchClan(string.Empty);
								}
								else if (isViewMember && currClan != null)
								{
									GameCanvas.startYesNoDlg(mResources.do_u_want_join_clan + currClan.name, new Command(mResources.YES, this, 4000, currClan), new Command(mResources.NO, this, 4005, currClan));
								}
							}
							else if (isSearchClan)
							{
								currClan = getCurrClan();
								if (currClan != null)
								{
									MyVector myVector = new MyVector();
									myVector.addElement(new Command(mResources.request_join_clan, this, 4000, currClan));
									myVector.addElement(new Command(mResources.view_clan_member, this, 4001, currClan));
									GameCanvas.menu.startAt(myVector, X, (selected + 1) * ITEM_HEIGHT - cmy + yScroll);
									addClanDetail(getCurrClan());
								}
							}
							else if (isViewMember)
							{
								currMem = getCurrMember();
								if (currMem != null)
								{
									MyVector myVector2 = new MyVector();
									myVector2.addElement(new Command(mResources.CLOSE, this, 8000, currClan));
									GameCanvas.menu.startAt(myVector2, X, (selected + 1) * ITEM_HEIGHT - cmy + yScroll);
									GameCanvas.menu.startAt(myVector2, 0, (selected + 1) * ITEM_HEIGHT - cmy + yScroll);
									addClanMemberDetail(currMem);
								}
							}
						}
					}
					else if (selected == 0)
					{
						if (isMessage)
						{
							if (cSelected == 0)
							{
								if (myMember.size() > 1)
								{
									chatClan();
								}
								else
								{
									member = null;
									isSearchClan = false;
									isViewMember = true;
									isMessage = false;
									currentListLength = myMember.size() + 2;
									initTabClans();
								}
							}
							if (cSelected == 1)
							{
								Service.gI().clanMessage(1, null, -1);
							}
							if (cSelected == 2)
							{
								member = null;
								isSearchClan = false;
								isViewMember = true;
								isMessage = false;
								currentListLength = myMember.size() + 2;
								initTabClans();
								getCurrClanOtion();
							}
						}
						else if (isViewMember)
						{
							if (cSelected == 0)
							{
								isSearchClan = false;
								isViewMember = false;
								isMessage = true;
								currentListLength = ClanMessage.vMessage.size() + 2;
								initTabClans();
							}
							if (cSelected == 1)
							{
								if (myMember.size() > 1)
								{
									Service.gI().leaveClan();
								}
								else
								{
									chagenSlogan();
								}
							}
							if (cSelected == 2)
							{
								if (myMember.size() > 1)
								{
									chagenSlogan();
								}
								else
								{
									Service.gI().getClan(3, -1, null);
								}
							}
							if (cSelected == 3)
							{
								Service.gI().getClan(3, -1, null);
							}
						}
					}
					else if (selected == 1)
					{
						if (isSearchClan)
						{
							Service.gI().searchClan(string.Empty);
						}
					}
					else if (isSearchClan)
					{
						currClan = getCurrClan();
						if (currClan != null)
						{
							MyVector myVector3 = new MyVector();
							myVector3.addElement(new Command(mResources.view_clan_member, this, 4001, currClan));
							GameCanvas.menu.startAt(myVector3, X, (selected + 1) * ITEM_HEIGHT - cmy + yScroll);
							addClanDetail(getCurrClan());
						}
					}
					else if (isViewMember)
					{
						Res.outz("TOI DAY 1");
						currMem = getCurrMember();
						if (currMem != null)
						{
							MyVector myVector4 = new MyVector();
							Res.outz("TOI DAY 2");
							if (member != null)
							{
								myVector4.addElement(new Command(mResources.CLOSE, this, 8000, null));
								Res.outz("TOI DAY 3");
							}
							else if (myMember != null)
							{
								Res.outz("TOI DAY 4");
								Res.outz("my role= " + Char.myCharz().role);
								if (Char.myCharz().charID == currMem.ID || Char.myCharz().role == 2)
								{
									myVector4.addElement(new Command(mResources.CLOSE, this, 8000, currMem));
								}
								if (Char.myCharz().role < 2 && Char.myCharz().charID != currMem.ID)
								{
									Res.outz("TOI DAY");
									if (currMem.role == 0 || currMem.role == 1)
									{
										myVector4.addElement(new Command(mResources.CLOSE, this, 8000, currMem));
									}
									if (currMem.role == 2)
									{
										myVector4.addElement(new Command(mResources.create_clan_co_leader, this, 5002, currMem));
									}
									if (Char.myCharz().role == 0)
									{
										myVector4.addElement(new Command(mResources.create_clan_leader, this, 5001, currMem));
										if (currMem.role == 1)
										{
											myVector4.addElement(new Command(mResources.disable_clan_mastership, this, 5003, currMem));
										}
									}
								}
								if (Char.myCharz().role < currMem.role)
								{
									myVector4.addElement(new Command(mResources.kick_clan_mem, this, 5004, currMem));
								}
							}
							GameCanvas.menu.startAt(myVector4, X, (selected + 1) * ITEM_HEIGHT - cmy + yScroll);
							addClanMemberDetail(currMem);
						}
					}
					else if (isMessage)
					{
						currMess = getCurrMessage();
						if (currMess != null)
						{
							if (currMess.type == 0)
							{
								MyVector myVector5 = new MyVector();
								myVector5.addElement(new Command(mResources.CLOSE, this, 8000, currMess));
								GameCanvas.menu.startAt(myVector5, X, (selected + 1) * ITEM_HEIGHT - cmy + yScroll);
								addMessageDetail(currMess);
							}
							else if (currMess.type == 1)
							{
								if (currMess.playerId != Char.myCharz().charID && cSelected != -1)
								{
									Service.gI().clanDonate(currMess.id);
								}
							}
							else if (currMess.type == 2 && currMess.option != null)
							{
								if (cSelected == 0)
								{
									Service.gI().joinClan(currMess.id, 1);
								}
								else if (cSelected == 1)
								{
									Service.gI().joinClan(currMess.id, 0);
								}
							}
						}
					}
					if (GameCanvas.isTouch)
					{
						cSelected = -1;
						selected = -1;
					}
				}
				catch (Exception)
				{
					throw;
				}
			}

	private void doFireClanIcon()
			{
			}

	private void doFireMap()
			{
				if (imgMap != null)
				{
					imgMap.texture = null;
					imgMap = null;
				}
				TileMap.lastPlanetId = -1;
				mSystem.gcc();
				SmallImage.loadBigRMS();
				setTypeMain();
				cmx = (cmtoX = 0);
			}

	private void doFireZone()
			{
				if (selected != -1)
				{
					Res.outz("FIRE ZONE");
					isChangeZone = true;
					GameCanvas.panel.hide();
				}
			}

}
