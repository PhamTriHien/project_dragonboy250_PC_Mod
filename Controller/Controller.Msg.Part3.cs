using System;
using Assets.src.e;
using Assets.src.f;
using Assets.src.g;
using UnityEngine;

public partial class Controller : IMessageHandler
{
	public bool onMessage_Part3(Message msg)
	{
		Char @char = null;
		Mob mob = null;
		MyVector myVector = new MyVector();
		int num = 0;
		switch (msg.command)
		{
				case -51:
					InfoDlg.hide();
					readClanMsg(msg, 0);
					if (GameCanvas.panel.isMessage && GameCanvas.panel.type == 5)
					{
						GameCanvas.panel.initTabClans();
					}
					break;
				case -53:
				{
					InfoDlg.hide();
					bool flag7 = false;
					int num105 = msg.reader().readInt();
					Res.outz("clanId= " + num105);
					if (num105 == -1)
					{
						flag7 = true;
						Char.myCharz().clan = null;
						ClanMessage.vMessage.removeAllElements();
						if (GameCanvas.panel.member != null)
						{
							GameCanvas.panel.member.removeAllElements();
						}
						if (GameCanvas.panel.myMember != null)
						{
							GameCanvas.panel.myMember.removeAllElements();
						}
						if (GameCanvas.currentScreen == GameScr.gI())
						{
							GameCanvas.panel.setTabClans();
						}
						return true;
					}
					GameCanvas.panel.tabIcon = null;
					if (Char.myCharz().clan == null)
					{
						Char.myCharz().clan = new Clan();
					}
					Char.myCharz().clan.ID = num105;
					Char.myCharz().clan.name = msg.reader().readUTF();
					Char.myCharz().clan.slogan = msg.reader().readUTF();
					Char.myCharz().clan.imgID = msg.reader().readShort();
					Char.myCharz().clan.powerPoint = msg.reader().readUTF();
					Char.myCharz().clan.leaderName = msg.reader().readUTF();
					Char.myCharz().clan.currMember = msg.reader().readUnsignedByte();
					Char.myCharz().clan.maxMember = msg.reader().readUnsignedByte();
					Char.myCharz().role = msg.reader().readByte();
					Char.myCharz().clan.clanPoint = msg.reader().readInt();
					Char.myCharz().clan.level = msg.reader().readByte();
					GameCanvas.panel.myMember = new MyVector();
					for (int num106 = 0; num106 < Char.myCharz().clan.currMember; num106++)
					{
						Member member5 = new Member();
						member5.ID = msg.reader().readInt();
						member5.head = msg.reader().readShort();
						member5.headICON = msg.reader().readShort();
						member5.leg = msg.reader().readShort();
						member5.body = msg.reader().readShort();
						member5.name = msg.reader().readUTF();
						member5.role = msg.reader().readByte();
						member5.powerPoint = msg.reader().readUTF();
						member5.donate = msg.reader().readInt();
						member5.receive_donate = msg.reader().readInt();
						member5.clanPoint = msg.reader().readInt();
						member5.curClanPoint = msg.reader().readInt();
						member5.joinTime = NinjaUtil.getDate(msg.reader().readInt());
						GameCanvas.panel.myMember.addElement(member5);
					}
					int num107 = msg.reader().readUnsignedByte();
					for (int num108 = 0; num108 < num107; num108++)
					{
						readClanMsg(msg, -1);
					}
					if (GameCanvas.panel.isSearchClan || GameCanvas.panel.isViewMember || GameCanvas.panel.isMessage)
					{
						GameCanvas.panel.setTabClans();
					}
					if (flag7)
					{
						GameCanvas.panel.setTabClans();
					}
					Res.outz("=>>>>>>>>>>>>>>>>>>>>>> -537 MY CLAN INFO");
					break;
				}
				case -52:
				{
					sbyte b22 = msg.reader().readByte();
					if (b22 == 0)
					{
						Member member2 = new Member();
						member2.ID = msg.reader().readInt();
						member2.head = msg.reader().readShort();
						member2.headICON = msg.reader().readShort();
						member2.leg = msg.reader().readShort();
						member2.body = msg.reader().readShort();
						member2.name = msg.reader().readUTF();
						member2.role = msg.reader().readByte();
						member2.powerPoint = msg.reader().readUTF();
						member2.donate = msg.reader().readInt();
						member2.receive_donate = msg.reader().readInt();
						member2.clanPoint = msg.reader().readInt();
						member2.joinTime = NinjaUtil.getDate(msg.reader().readInt());
						if (GameCanvas.panel.myMember == null)
						{
							GameCanvas.panel.myMember = new MyVector();
						}
						GameCanvas.panel.myMember.addElement(member2);
						GameCanvas.panel.initTabClans();
					}
					if (b22 == 1)
					{
						GameCanvas.panel.myMember.removeElementAt(msg.reader().readByte());
						GameCanvas.panel.currentListLength--;
						GameCanvas.panel.initTabClans();
					}
					if (b22 == 2)
					{
						Member member3 = new Member();
						member3.ID = msg.reader().readInt();
						member3.head = msg.reader().readShort();
						member3.headICON = msg.reader().readShort();
						member3.leg = msg.reader().readShort();
						member3.body = msg.reader().readShort();
						member3.name = msg.reader().readUTF();
						member3.role = msg.reader().readByte();
						member3.powerPoint = msg.reader().readUTF();
						member3.donate = msg.reader().readInt();
						member3.receive_donate = msg.reader().readInt();
						member3.clanPoint = msg.reader().readInt();
						member3.joinTime = NinjaUtil.getDate(msg.reader().readInt());
						for (int num48 = 0; num48 < GameCanvas.panel.myMember.size(); num48++)
						{
							Member member4 = (Member)GameCanvas.panel.myMember.elementAt(num48);
							if (member4.ID == member3.ID)
							{
								if (Char.myCharz().charID == member3.ID)
								{
									Char.myCharz().role = member3.role;
								}
								Member o = member3;
								GameCanvas.panel.myMember.removeElement(member4);
								GameCanvas.panel.myMember.insertElementAt(o, num48);
								return true;
							}
						}
					}
					Res.outz("=>>>>>>>>>>>>>>>>>>>>>> -52  MY CLAN UPDSTE");
					break;
				}
				case -50:
				{
					InfoDlg.hide();
					GameCanvas.panel.member = new MyVector();
					sbyte b15 = msg.reader().readByte();
					for (int num26 = 0; num26 < b15; num26++)
					{
						Member member = new Member();
						member.ID = msg.reader().readInt();
						member.head = msg.reader().readShort();
						member.headICON = msg.reader().readShort();
						member.leg = msg.reader().readShort();
						member.body = msg.reader().readShort();
						member.name = msg.reader().readUTF();
						member.role = msg.reader().readByte();
						member.powerPoint = msg.reader().readUTF();
						member.donate = msg.reader().readInt();
						member.receive_donate = msg.reader().readInt();
						member.clanPoint = msg.reader().readInt();
						member.joinTime = NinjaUtil.getDate(msg.reader().readInt());
						GameCanvas.panel.member.addElement(member);
					}
					GameCanvas.panel.isViewMember = true;
					GameCanvas.panel.isSearchClan = false;
					GameCanvas.panel.isMessage = false;
					GameCanvas.panel.currentListLength = GameCanvas.panel.member.size() + 2;
					GameCanvas.panel.initTabClans();
					break;
				}
				case -47:
				{
					InfoDlg.hide();
					sbyte b7 = msg.reader().readByte();
					Res.outz("clan = " + b7);
					if (b7 == 0)
					{
						GameCanvas.panel.clanReport = mResources.cannot_find_clan;
						GameCanvas.panel.clans = null;
					}
					else
					{
						GameCanvas.panel.clans = new Clan[b7];
						Res.outz("clan search lent= " + GameCanvas.panel.clans.Length);
						for (int i = 0; i < GameCanvas.panel.clans.Length; i++)
						{
							GameCanvas.panel.clans[i] = new Clan();
							GameCanvas.panel.clans[i].ID = msg.reader().readInt();
							GameCanvas.panel.clans[i].name = msg.reader().readUTF();
							GameCanvas.panel.clans[i].slogan = msg.reader().readUTF();
							GameCanvas.panel.clans[i].imgID = msg.reader().readShort();
							GameCanvas.panel.clans[i].powerPoint = msg.reader().readUTF();
							GameCanvas.panel.clans[i].leaderName = msg.reader().readUTF();
							GameCanvas.panel.clans[i].currMember = msg.reader().readUnsignedByte();
							GameCanvas.panel.clans[i].maxMember = msg.reader().readUnsignedByte();
							GameCanvas.panel.clans[i].date = msg.reader().readInt();
						}
					}
					GameCanvas.panel.isSearchClan = true;
					GameCanvas.panel.isViewMember = false;
					GameCanvas.panel.isMessage = false;
					if (GameCanvas.panel.isSearchClan)
					{
						GameCanvas.panel.initTabClans();
					}
					break;
				}
				case -46:
				{
					InfoDlg.hide();
					sbyte b58 = msg.reader().readByte();
					if (b58 == 1 || b58 == 3)
					{
						GameCanvas.endDlg();
						ClanImage.vClanImage.removeAllElements();
						int num139 = msg.reader().readShort();
						for (int num140 = 0; num140 < num139; num140++)
						{
							ClanImage clanImage2 = new ClanImage();
							clanImage2.ID = msg.reader().readShort();
							clanImage2.name = msg.reader().readUTF();
							clanImage2.xu = msg.reader().readInt();
							clanImage2.luong = msg.reader().readInt();
							if (!ClanImage.isExistClanImage(clanImage2.ID))
							{
								ClanImage.addClanImage(clanImage2);
								continue;
							}
							ClanImage.getClanImage((short)clanImage2.ID).name = clanImage2.name;
							ClanImage.getClanImage((short)clanImage2.ID).xu = clanImage2.xu;
							ClanImage.getClanImage((short)clanImage2.ID).luong = clanImage2.luong;
						}
						if (Char.myCharz().clan != null)
						{
							GameCanvas.panel.changeIcon();
						}
					}
					if (b58 == 4)
					{
						Char.myCharz().clan.imgID = msg.reader().readShort();
						Char.myCharz().clan.slogan = msg.reader().readUTF();
					}
					break;
				}
				case -61:
				{
					int num132 = msg.reader().readInt();
					if (num132 != Char.myCharz().charID)
					{
						if (GameScr.findCharInMap(num132) != null)
						{
							GameScr.findCharInMap(num132).clanID = msg.reader().readInt();
							if (GameScr.findCharInMap(num132).clanID == -2)
							{
								GameScr.findCharInMap(num132).isCopy = true;
							}
						}
					}
					else if (Char.myCharz().clan != null)
					{
						Char.myCharz().clan.ID = msg.reader().readInt();
					}
					break;
				}
				case -42:
					Char.myCharz().cHPGoc = msg.readInt3Byte();
					Char.myCharz().cMPGoc = msg.readInt3Byte();
					Char.myCharz().cDamGoc = msg.reader().readInt();
					Char.myCharz().cHPFull = msg.reader().readLong();
					Char.myCharz().cMPFull = msg.reader().readLong();
					Char.myCharz().cHP = msg.reader().readLong();
					Char.myCharz().cMP = msg.reader().readLong();
					Char.myCharz().cspeed = msg.reader().readByte();
					Char.myCharz().hpFrom1000TiemNang = msg.reader().readByte();
					Char.myCharz().mpFrom1000TiemNang = msg.reader().readByte();
					Char.myCharz().damFrom1000TiemNang = msg.reader().readByte();
					Char.myCharz().cDamFull = msg.reader().readLong();
					Char.myCharz().cDefull = msg.reader().readLong();
					Char.myCharz().cCriticalFull = msg.reader().readByte();
					Char.myCharz().cTiemNang = msg.reader().readLong();
					Char.myCharz().expForOneAdd = msg.reader().readShort();
					Char.myCharz().cDefGoc = msg.reader().readInt();
					Char.myCharz().cCriticalGoc = msg.reader().readByte();
					Char.myCharz().cGiamST = msg.reader().readByte();
					Char.myCharz().cCritDameFull = msg.reader().readShort();
					InfoDlg.hide();
					break;
				case 1:
				{
					bool flag9 = msg.reader().readBool();
					Res.outz("isRes= " + flag9);
					if (!flag9)
					{
						GameCanvas.startOKDlg(msg.reader().readUTF());
						break;
					}
					GameCanvas.loginScr.isLogin2 = false;
					Rms.saveRMSString(Rms.RMS_userAo + ServerListScreen.ipSelect, string.Empty);
					GameCanvas.endDlg();
					GameCanvas.loginScr.doLogin();
					break;
				}
				case 2:
					Char.isLoadingMap = false;
					LoginScr.isLoggingIn = false;
					if (!GameScr.isLoadAllData)
					{
						GameScr.gI().initSelectChar();
					}
					BgItem.clearHashTable();
					GameCanvas.endDlg();
					CreateCharScr.isCreateChar = true;
					CreateCharScr.gI().switchToMe();
					break;
				case -107:
				{
					sbyte b25 = msg.reader().readByte();
					if (b25 == 0)
					{
						Char.myCharz().havePet = false;
					}
					if (b25 == 1)
					{
						Char.myCharz().havePet = true;
					}
					if (b25 != 2)
					{
						break;
					}
					InfoDlg.hide();
					Char.myPetz().head = msg.reader().readShort();
					Debug.LogWarning(">>>cmd head:" + Char.myPetz().avatarz());
					Res.outz("tra ve head= " + Char.myCharz().head);
					Char.myPetz().setDefaultPart();
					int num54 = msg.reader().readUnsignedByte();
					Res.outz("num body = " + num54);
					Char.myPetz().arrItemBody = new Item[num54];
					for (int num55 = 0; num55 < num54; num55++)
					{
						short num56 = msg.reader().readShort();
						Res.outz("template id= " + num56);
						if (num56 == -1)
						{
							continue;
						}
						Res.outz("1");
						Char.myPetz().arrItemBody[num55] = new Item();
						Char.myPetz().arrItemBody[num55].template = ItemTemplates.get(num56);
						int num57 = Char.myPetz().arrItemBody[num55].template.type;
						Char.myPetz().arrItemBody[num55].quantity = msg.reader().readInt();
						Res.outz("3");
						Char.myPetz().arrItemBody[num55].info = msg.reader().readUTF();
						Char.myPetz().arrItemBody[num55].content = msg.reader().readUTF();
						int num58 = msg.reader().readUnsignedByte();
						Res.outz("option size= " + num58);
						if (num58 != 0)
						{
							Char.myPetz().arrItemBody[num55].itemOption = new ItemOption[num58];
							for (int num59 = 0; num59 < Char.myPetz().arrItemBody[num55].itemOption.Length; num59++)
							{
								ItemOption itemOption2 = readItemOption(msg);
								if (itemOption2 != null)
								{
									Char.myPetz().arrItemBody[num55].itemOption[num59] = itemOption2;
								}
							}
						}
						switch (num57)
						{
						case 0:
							Char.myPetz().body = Char.myPetz().arrItemBody[num55].template.part;
							break;
						case 1:
							Char.myPetz().leg = Char.myPetz().arrItemBody[num55].template.part;
							break;
						}
					}
					Char.myPetz().cHP = msg.reader().readLong();
					Char.myPetz().cHPFull = msg.reader().readLong();
					Char.myPetz().cMP = msg.reader().readLong();
					Char.myPetz().cMPFull = msg.reader().readLong();
					Char.myPetz().cDamFull = msg.reader().readLong();
					Char.myPetz().cName = msg.reader().readUTF();
					Char.myPetz().currStrLevel = msg.reader().readUTF();
					Char.myPetz().cPower = msg.reader().readLong();
					Char.myPetz().cTiemNang = msg.reader().readLong();
					Char.myPetz().petStatus = msg.reader().readByte();
					Char.myPetz().cStamina = msg.reader().readShort();
					Char.myPetz().cMaxStamina = msg.reader().readShort();
					Char.myPetz().cCriticalFull = msg.reader().readByte();
					Char.myPetz().cDefull = msg.reader().readLong();
					Char.myPetz().arrPetSkill = new Skill[msg.reader().readByte()];
					Res.outz("SKILLENT = " + Char.myPetz().arrPetSkill);
					for (int num60 = 0; num60 < Char.myPetz().arrPetSkill.Length; num60++)
					{
						short num61 = msg.reader().readShort();
						if (num61 != -1)
						{
							Char.myPetz().arrPetSkill[num60] = Skills.get(num61);
							continue;
						}
						Char.myPetz().arrPetSkill[num60] = new Skill();
						Char.myPetz().arrPetSkill[num60].template = null;
						Char.myPetz().arrPetSkill[num60].moreInfo = msg.reader().readUTF();
					}
					Char.myPetz().cGiamST = msg.reader().readByte();
					Char.myPetz().cCritDameFull = msg.reader().readShort();
					if (GameCanvas.w > 2 * Panel.WIDTH_PANEL)
					{
						GameCanvas.panel2 = new Panel();
						GameCanvas.panel2.tabName[7] = new string[1][] { new string[1] { string.Empty } };
						GameCanvas.panel2.setTypeBodyOnly();
						GameCanvas.panel2.show();
						GameCanvas.panel.setTypePetMain();
						GameCanvas.panel.show();
					}
					else
					{
						GameCanvas.panel.tabName[21] = mResources.petMainTab;
						GameCanvas.panel.setTypePetMain();
						GameCanvas.panel.show();
					}
					break;
				}
				case -37:
				{
					sbyte b33 = msg.reader().readByte();
					Res.outz("cAction= " + b33);
					if (b33 != 0)
					{
						break;
					}
					Char.myCharz().head = msg.reader().readShort();
					Char.myCharz().setDefaultPart();
					int num80 = msg.reader().readUnsignedByte();
					Res.outz("num body = " + num80);
					Char.myCharz().arrItemBody = new Item[num80];
					for (int num81 = 0; num81 < num80; num81++)
					{
						short num82 = msg.reader().readShort();
						if (num82 == -1)
						{
							continue;
						}
						Char.myCharz().arrItemBody[num81] = new Item();
						Char.myCharz().arrItemBody[num81].template = ItemTemplates.get(num82);
						int num83 = Char.myCharz().arrItemBody[num81].template.type;
						Char.myCharz().arrItemBody[num81].quantity = msg.reader().readInt();
						Char.myCharz().arrItemBody[num81].info = msg.reader().readUTF();
						Char.myCharz().arrItemBody[num81].content = msg.reader().readUTF();
						int num84 = msg.reader().readUnsignedByte();
						if (num84 != 0)
						{
							Char.myCharz().arrItemBody[num81].itemOption = new ItemOption[num84];
							for (int num85 = 0; num85 < Char.myCharz().arrItemBody[num81].itemOption.Length; num85++)
							{
								ItemOption itemOption4 = readItemOption(msg);
								if (itemOption4 != null)
								{
									Char.myCharz().arrItemBody[num81].itemOption[num85] = itemOption4;
								}
							}
						}
						switch (num83)
						{
						case 0:
							Char.myCharz().body = Char.myCharz().arrItemBody[num81].template.part;
							break;
						case 1:
							Char.myCharz().leg = Char.myCharz().arrItemBody[num81].template.part;
							break;
						}
					}
					break;
				}
				case -36:
				{
					sbyte b8 = msg.reader().readByte();
					Res.outz("cAction= " + b8);
					GameScr.isudungCapsun4 = false;
					GameScr.isudungCapsun3 = false;
					if (b8 == 0)
					{
						int num10 = msg.reader().readUnsignedByte();
						Char.myCharz().arrItemBag = new Item[num10];
						GameScr.hpPotion = 0;
						Res.outz("numC=" + num10);
						for (int j = 0; j < num10; j++)
						{
							short num11 = msg.reader().readShort();
							if (num11 == -1)
							{
								continue;
							}
							Char.myCharz().arrItemBag[j] = new Item();
							Char.myCharz().arrItemBag[j].template = ItemTemplates.get(num11);
							Char.myCharz().arrItemBag[j].quantity = msg.reader().readInt();
							Char.myCharz().arrItemBag[j].info = msg.reader().readUTF();
							Char.myCharz().arrItemBag[j].content = msg.reader().readUTF();
							Char.myCharz().arrItemBag[j].indexUI = j;
							int num12 = msg.reader().readUnsignedByte();
							if (num12 != 0)
							{
								Char.myCharz().arrItemBag[j].itemOption = new ItemOption[num12];
								for (int k = 0; k < Char.myCharz().arrItemBag[j].itemOption.Length; k++)
								{
									ItemOption itemOption = readItemOption(msg);
									if (itemOption != null)
									{
										Char.myCharz().arrItemBag[j].itemOption[k] = itemOption;
									}
								}
								Char.myCharz().arrItemBag[j].compare = GameCanvas.panel.getCompare(Char.myCharz().arrItemBag[j]);
							}
							if (Char.myCharz().arrItemBag[j].template.type == 11)
							{
							}
							if (Char.myCharz().arrItemBag[j].template.type == 6)
							{
								GameScr.hpPotion += Char.myCharz().arrItemBag[j].quantity;
							}
							if (Char.myCharz().arrItemBag[j].template.id == 194)
							{
								GameScr.isudungCapsun4 = Char.myCharz().arrItemBag[j].quantity > 0;
							}
							else if (Char.myCharz().arrItemBag[j].template.id == 193 && !GameScr.isudungCapsun4)
							{
								GameScr.isudungCapsun3 = Char.myCharz().arrItemBag[j].quantity > 0;
							}
						}
					}
					if (b8 == 2)
					{
						sbyte b9 = msg.reader().readByte();
						int num13 = msg.reader().readInt();
						int quantity = Char.myCharz().arrItemBag[b9].quantity;
						int id = Char.myCharz().arrItemBag[b9].template.id;
						Char.myCharz().arrItemBag[b9].quantity = num13;
						if (Char.myCharz().arrItemBag[b9].quantity < quantity && Char.myCharz().arrItemBag[b9].template.type == 6)
						{
							GameScr.hpPotion -= quantity - Char.myCharz().arrItemBag[b9].quantity;
						}
						if (Char.myCharz().arrItemBag[b9].quantity == 0)
						{
							Char.myCharz().arrItemBag[b9] = null;
						}
						switch (id)
						{
						case 194:
							GameScr.isudungCapsun4 = num13 > 0;
							break;
						case 193:
							GameScr.isudungCapsun3 = num13 > 0;
							break;
						}
					}
					break;
				}
				case -35:
				{
					sbyte b59 = msg.reader().readByte();
					Res.outz("cAction= " + b59);
					if (b59 == 0)
					{
						int num144 = msg.reader().readUnsignedByte();
						Char.myCharz().arrItemBox = new Item[num144];
						GameCanvas.panel.hasUse = 0;
						for (int num145 = 0; num145 < num144; num145++)
						{
							short num146 = msg.reader().readShort();
							if (num146 == -1)
							{
								continue;
							}
							Char.myCharz().arrItemBox[num145] = new Item();
							Char.myCharz().arrItemBox[num145].template = ItemTemplates.get(num146);
							Char.myCharz().arrItemBox[num145].quantity = msg.reader().readInt();
							Char.myCharz().arrItemBox[num145].info = msg.reader().readUTF();
							Char.myCharz().arrItemBox[num145].content = msg.reader().readUTF();
							int num147 = msg.reader().readUnsignedByte();
							if (num147 != 0)
							{
								Char.myCharz().arrItemBox[num145].itemOption = new ItemOption[num147];
								for (int num148 = 0; num148 < Char.myCharz().arrItemBox[num145].itemOption.Length; num148++)
								{
									ItemOption itemOption6 = readItemOption(msg);
									if (itemOption6 != null)
									{
										Char.myCharz().arrItemBox[num145].itemOption[num148] = itemOption6;
									}
								}
							}
							GameCanvas.panel.hasUse++;
						}
					}
					if (b59 == 1)
					{
						bool isBoxClan = false;
						try
						{
							sbyte b60 = msg.reader().readByte();
							if (b60 == 1)
							{
								isBoxClan = true;
							}
						}
						catch (Exception)
						{
						}
						GameCanvas.panel.setTypeBox();
						GameCanvas.panel.isBoxClan = isBoxClan;
						GameCanvas.panel.show();
					}
					if (b59 == 2)
					{
						sbyte b61 = msg.reader().readByte();
						int quantity2 = msg.reader().readInt();
						Char.myCharz().arrItemBox[b61].quantity = quantity2;
						if (Char.myCharz().arrItemBox[b61].quantity == 0)
						{
							Char.myCharz().arrItemBox[b61] = null;
						}
					}
					break;
				}
			default:
				return false;
		}
		return true;
	}
}
