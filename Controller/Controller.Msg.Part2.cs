using System;
using Assets.src.e;
using Assets.src.f;
using Assets.src.g;
using UnityEngine;

public partial class Controller : IMessageHandler
{
	public bool onMessage_Part2(Message msg)
	{
		Char @char = null;
		Mob mob = null;
		MyVector myVector = new MyVector();
		int num = 0;
		switch (msg.command)
		{
				case -112:
				{
					sbyte b42 = msg.reader().readByte();
					if (b42 == 0)
					{
						sbyte mobIndex = msg.reader().readByte();
						GameScr.findMobInMap(mobIndex).clearBody();
					}
					if (b42 == 1)
					{
						sbyte mobIndex2 = msg.reader().readByte();
						GameScr.findMobInMap(mobIndex2).setBody(msg.reader().readShort());
					}
					break;
				}
				case -84:
				{
					int index3 = msg.reader().readUnsignedByte();
					Mob mob8 = null;
					try
					{
						mob8 = (Mob)GameScr.vMob.elementAt(index3);
					}
					catch (Exception)
					{
					}
					if (mob8 != null)
					{
						mob8.maxHp = msg.reader().readLong();
					}
					break;
				}
				case -83:
				{
					sbyte b38 = msg.reader().readByte();
					if (b38 == 0)
					{
						int num92 = msg.reader().readShort();
						int bgRID = msg.reader().readShort();
						int num93 = msg.reader().readUnsignedByte();
						int num94 = msg.reader().readInt();
						string text5 = msg.reader().readUTF();
						int num95 = msg.reader().readShort();
						int num96 = msg.reader().readShort();
						sbyte b39 = msg.reader().readByte();
						if (b39 == 1)
						{
							GameScr.gI().isRongNamek = true;
						}
						else
						{
							GameScr.gI().isRongNamek = false;
						}
						GameScr.gI().xR = num95;
						GameScr.gI().yR = num96;
						Res.outz("xR= " + num95 + " yR= " + num96 + " +++++++++++++++++++++++++++++++++++++++");
						if (Char.myCharz().charID == num94)
						{
							GameCanvas.panel.hideNow();
							GameScr.gI().activeRongThanEff(isMe: true);
						}
						else if (TileMap.mapID == num92 && TileMap.zoneID == num93)
						{
							GameScr.gI().activeRongThanEff(isMe: false);
						}
						else if (mGraphics.zoomLevel > 1)
						{
							GameScr.gI().doiMauTroi();
						}
						GameScr.gI().mapRID = num92;
						GameScr.gI().bgRID = bgRID;
						GameScr.gI().zoneRID = num93;
					}
					if (b38 == 1)
					{
						Res.outz("map RID = " + GameScr.gI().mapRID + " zone RID= " + GameScr.gI().zoneRID);
						Res.outz("map ID = " + TileMap.mapID + " zone ID= " + TileMap.zoneID);
						if (TileMap.mapID == GameScr.gI().mapRID && TileMap.zoneID == GameScr.gI().zoneRID)
						{
							GameScr.gI().hideRongThanEff();
						}
						else
						{
							GameScr.gI().isRongThanXuatHien = false;
							if (GameScr.gI().isRongNamek)
							{
								GameScr.gI().isRongNamek = false;
							}
						}
					}
					if (b38 != 2)
					{
					}
					break;
				}
				case -82:
				{
					sbyte b11 = msg.reader().readByte();
					TileMap.tileIndex = new int[b11][][];
					TileMap.tileType = new int[b11][];
					Res.outz(">>>>>>Cmd.TILE_SET:nTile: " + b11);
					for (int n = 0; n < b11; n++)
					{
						Res.outz(n + ">>>>>>Cmd.TILE_SET: forr");
						sbyte b12 = msg.reader().readByte();
						Res.outz(n + ">>>>>>Cmd.TILE_SET:nTypeSize: " + b12);
						TileMap.tileType[n] = new int[b12];
						TileMap.tileIndex[n] = new int[b12][];
						for (int num19 = 0; num19 < b12; num19++)
						{
							TileMap.tileType[n][num19] = msg.reader().readInt();
							sbyte b13 = msg.reader().readByte();
							TileMap.tileIndex[n][num19] = new int[b13];
							for (int num20 = 0; num20 < b13; num20++)
							{
								TileMap.tileIndex[n][num19][num20] = msg.reader().readByte();
							}
						}
					}
					break;
				}
				case -81:
				{
					sbyte b67 = msg.reader().readByte();
					if (b67 == 0)
					{
						string src = msg.reader().readUTF();
						string src2 = msg.reader().readUTF();
						GameCanvas.panel.setTypeCombine();
						GameCanvas.panel.combineInfo = mFont.tahoma_7b_blue.splitFontArray(src, Panel.WIDTH_PANEL);
						GameCanvas.panel.combineTopInfo = mFont.tahoma_7.splitFontArray(src2, Panel.WIDTH_PANEL);
						GameCanvas.panel.show();
					}
					if (b67 == 1)
					{
						GameCanvas.panel.vItemCombine.removeAllElements();
						sbyte b68 = msg.reader().readByte();
						for (int num160 = 0; num160 < b68; num160++)
						{
							sbyte b69 = msg.reader().readByte();
							for (int num161 = 0; num161 < Char.myCharz().arrItemBag.Length; num161++)
							{
								Item item4 = Char.myCharz().arrItemBag[num161];
								if (item4 != null && item4.indexUI == b69)
								{
									item4.isSelect = true;
									GameCanvas.panel.vItemCombine.addElement(item4);
								}
							}
						}
						if (GameCanvas.panel.isShow)
						{
							GameCanvas.panel.setTabCombine();
						}
					}
					if (b67 == 2)
					{
						GameCanvas.panel.combineSuccess = 0;
						GameCanvas.panel.setCombineEff(0);
					}
					if (b67 == 3)
					{
						GameCanvas.panel.combineSuccess = 1;
						GameCanvas.panel.setCombineEff(0);
					}
					if (b67 == 4)
					{
						short iconID = msg.reader().readShort();
						GameCanvas.panel.iconID3 = iconID;
						GameCanvas.panel.combineSuccess = 0;
						GameCanvas.panel.setCombineEff(1);
					}
					if (b67 == 5)
					{
						short iconID2 = msg.reader().readShort();
						GameCanvas.panel.iconID3 = iconID2;
						GameCanvas.panel.combineSuccess = 0;
						GameCanvas.panel.setCombineEff(2);
					}
					if (b67 == 6)
					{
						short iconID3 = msg.reader().readShort();
						short iconID4 = msg.reader().readShort();
						GameCanvas.panel.combineSuccess = 0;
						GameCanvas.panel.setCombineEff(3);
						GameCanvas.panel.iconID1 = iconID3;
						GameCanvas.panel.iconID3 = iconID4;
					}
					if (b67 == 7)
					{
						short iconID5 = msg.reader().readShort();
						GameCanvas.panel.iconID3 = iconID5;
						GameCanvas.panel.combineSuccess = 0;
						GameCanvas.panel.setCombineEff(4);
					}
					if (b67 == 8)
					{
						GameCanvas.panel.iconID3 = -1;
						GameCanvas.panel.combineSuccess = 1;
						GameCanvas.panel.setCombineEff(4);
					}
					short num162 = 21;
					int num163 = 0;
					int num164 = 0;
					try
					{
						num162 = msg.reader().readShort();
						num163 = msg.reader().readShort();
						num164 = msg.reader().readShort();
						GameCanvas.panel.xS = num163 - GameScr.cmx;
						GameCanvas.panel.yS = num164 - GameScr.cmy;
					}
					catch (Exception)
					{
					}
					for (int num165 = 0; num165 < GameScr.vNpc.size(); num165++)
					{
						Npc npc6 = (Npc)GameScr.vNpc.elementAt(num165);
						if (npc6.template.npcTemplateId == num162)
						{
							GameCanvas.panel.xS = npc6.cx - GameScr.cmx;
							GameCanvas.panel.yS = npc6.cy - GameScr.cmy;
							GameCanvas.panel.idNPC = num162;
							break;
						}
					}
					break;
				}
				case -80:
				{
					sbyte b40 = msg.reader().readByte();
					InfoDlg.hide();
					if (b40 == 0)
					{
						GameCanvas.panel.vFriend.removeAllElements();
						int num97 = msg.reader().readUnsignedByte();
						for (int num98 = 0; num98 < num97; num98++)
						{
							Char char7 = new Char();
							char7.charID = msg.reader().readInt();
							char7.head = msg.reader().readShort();
							char7.headICON = msg.reader().readShort();
							char7.body = msg.reader().readShort();
							char7.leg = msg.reader().readShort();
							char7.bag = msg.reader().readShort();
							char7.cName = msg.reader().readUTF();
							bool isOnline = msg.reader().readBoolean();
							InfoItem infoItem = new InfoItem(mResources.power + ": " + msg.reader().readUTF());
							infoItem.charInfo = char7;
							infoItem.isOnline = isOnline;
							GameCanvas.panel.vFriend.addElement(infoItem);
						}
						GameCanvas.panel.setTypeFriend();
						GameCanvas.panel.show();
					}
					if (b40 == 3)
					{
						MyVector vFriend = GameCanvas.panel.vFriend;
						int num99 = msg.reader().readInt();
						Res.outz("online offline id=" + num99);
						for (int num100 = 0; num100 < vFriend.size(); num100++)
						{
							InfoItem infoItem2 = (InfoItem)vFriend.elementAt(num100);
							if (infoItem2.charInfo != null && infoItem2.charInfo.charID == num99)
							{
								Res.outz("online= " + infoItem2.isOnline);
								infoItem2.isOnline = msg.reader().readBoolean();
								break;
							}
						}
					}
					if (b40 != 2)
					{
						break;
					}
					MyVector vFriend2 = GameCanvas.panel.vFriend;
					int num101 = msg.reader().readInt();
					for (int num102 = 0; num102 < vFriend2.size(); num102++)
					{
						InfoItem infoItem3 = (InfoItem)vFriend2.elementAt(num102);
						if (infoItem3.charInfo != null && infoItem3.charInfo.charID == num101)
						{
							vFriend2.removeElement(infoItem3);
							break;
						}
					}
					if (GameCanvas.panel.isShow)
					{
						GameCanvas.panel.setTabFriend();
					}
					break;
				}
				case -99:
				{
					InfoDlg.hide();
					sbyte b63 = msg.reader().readByte();
					if (b63 == 0)
					{
						GameCanvas.panel.vEnemy.removeAllElements();
						int num151 = msg.reader().readUnsignedByte();
						for (int num152 = 0; num152 < num151; num152++)
						{
							Char char10 = new Char();
							char10.charID = msg.reader().readInt();
							char10.head = msg.reader().readShort();
							char10.headICON = msg.reader().readShort();
							char10.body = msg.reader().readShort();
							char10.leg = msg.reader().readShort();
							char10.bag = msg.reader().readShort();
							char10.cName = msg.reader().readUTF();
							InfoItem infoItem4 = new InfoItem(msg.reader().readUTF());
							bool flag10 = msg.reader().readBoolean();
							infoItem4.charInfo = char10;
							infoItem4.isOnline = flag10;
							Res.outz("isonline = " + flag10);
							GameCanvas.panel.vEnemy.addElement(infoItem4);
						}
						GameCanvas.panel.setTypeEnemy();
						GameCanvas.panel.show();
					}
					break;
				}
				case -79:
				{
					InfoDlg.hide();
					int num62 = msg.reader().readInt();
					Char charMenu = GameCanvas.panel.charMenu;
					if (charMenu == null)
					{
						return true;
					}
					charMenu.cPower = msg.reader().readLong();
					charMenu.currStrLevel = msg.reader().readUTF();
					break;
				}
				case -93:
				{
					short num103 = msg.reader().readShort();
					BgItem.newSmallVersion = new sbyte[num103];
					for (int num104 = 0; num104 < num103; num104++)
					{
						BgItem.newSmallVersion[num104] = msg.reader().readByte();
					}
					break;
				}
				case -77:
				{
					short num121 = msg.reader().readShort();
					SmallImage.newSmallVersion = new sbyte[num121];
					SmallImage.maxSmall = num121;
					SmallImage.imgNew = new Small[num121];
					for (int num122 = 0; num122 < num121; num122++)
					{
						SmallImage.newSmallVersion[num122] = msg.reader().readByte();
					}
					break;
				}
				case -76:
				{
					sbyte b65 = msg.reader().readByte();
					if (b65 == 0)
					{
						sbyte b66 = msg.reader().readByte();
						if (b66 <= 0)
						{
							return true;
						}
						Char.myCharz().arrArchive = new Archivement[b66];
						for (int num155 = 0; num155 < b66; num155++)
						{
							Char.myCharz().arrArchive[num155] = new Archivement();
							Char.myCharz().arrArchive[num155].info1 = num155 + 1 + ". " + msg.reader().readUTF();
							Char.myCharz().arrArchive[num155].info2 = msg.reader().readUTF();
							Char.myCharz().arrArchive[num155].money = msg.reader().readShort();
							Char.myCharz().arrArchive[num155].isFinish = msg.reader().readBoolean();
							Char.myCharz().arrArchive[num155].isRecieve = msg.reader().readBoolean();
						}
						GameCanvas.panel.setTypeArchivement();
						GameCanvas.panel.show();
					}
					else if (b65 == 1)
					{
						int num156 = msg.reader().readUnsignedByte();
						if (Char.myCharz().arrArchive[num156] != null)
						{
							Char.myCharz().arrArchive[num156].isRecieve = true;
						}
					}
					break;
				}
				case -74:
				{
					if (ServerListScreen.stopDownload)
					{
						return true;
					}
					if (!GameCanvas.isGetResourceFromServer())
					{
						Service.gI().getResource(3, null);
						SmallImage.loadBigRMS();
						SplashScr.imgLogo = null;
						if (Rms.loadRMSString(Rms.RMS_acc) != null || Rms.loadRMSString(Rms.RMS_userAo + ServerListScreen.ipSelect) != null)
						{
							LoginScr.isContinueToLogin = true;
						}
						GameCanvas.loginScr = new LoginScr();
						GameCanvas.loginScr.switchToMe();
						return true;
					}
					bool flag3 = true;
					Res.outz("1>>GET_IMAGE_SOURCE = " + msg.reader().available());
					sbyte b14 = msg.reader().readByte();
					Res.outz("2>GET_IMAGE_SOURCE = " + b14);
					if (b14 == 0)
					{
						int num22 = msg.reader().readInt();
						Res.outz("3>GET_IMAGE_SOURCE serverVersion = " + num22);
						string text2 = Rms.loadRMSString(Rms.RMS_ResVersion);
						int num23 = ((text2 == null || !(text2 != string.Empty)) ? (-1) : int.Parse(text2));
						Res.outz("4>>>GET_IMAGE_SOURCE: version>> " + text2 + " <> " + num23 + "!=" + num22);
						if (num23 == -1 || num23 != num22)
						{
							GameCanvas.serverScreen.show2();
						}
						else
						{
							SmallImage.loadBigRMS();
							SplashScr.imgLogo = null;
							ServerListScreen.loadScreen = true;
							Res.outz(">>>vo ne: " + GameCanvas.currentScreen);
							if (GameCanvas.currentScreen != GameCanvas.loginScr)
							{
								if (GameCanvas.serverScreen == null)
								{
									GameCanvas.serverScreen = new ServerListScreen();
								}
								GameCanvas.serverScreen.switchToMe();
							}
							else
							{
								if (GameCanvas.loginScr == null)
								{
									GameCanvas.loginScr = new LoginScr();
								}
								GameCanvas.loginScr.doLogin();
							}
						}
					}
					if (b14 == 1)
					{
						ServerListScreen.strWait = mResources.downloading_data;
						short nBig = msg.reader().readShort();
						ServerListScreen.nBig = nBig;
						Service.gI().getResource(2, null);
					}
					if (b14 == 2)
					{
						try
						{
							isLoadingData = true;
							GameCanvas.endDlg();
							ServerListScreen.demPercent++;
							ServerListScreen.percent = ServerListScreen.demPercent * 100 / ServerListScreen.nBig;
							string text3 = msg.reader().readUTF();
							Res.outz(">>>vo serverPath: " + text3);
							string[] array4 = Res.split(text3, "/", 0);
							string filename = "x" + mGraphics.zoomLevel + array4[array4.Length - 1];
							int num24 = msg.reader().readInt();
							sbyte[] data = new sbyte[num24];
							msg.reader().read(ref data, 0, num24);
							Rms.saveRMS(filename, data);
						}
						catch (Exception)
						{
							GameCanvas.startOK(mResources.pls_restart_game_error, 8885, null);
						}
					}
					if (b14 == 3 && flag3)
					{
						isLoadingData = false;
						int num25 = msg.reader().readInt();
						Res.outz(">>>GET_IMAGE_SOURCE: lastVersion>> " + num25);
						Rms.saveRMSString(Rms.RMS_ResVersion, num25 + string.Empty);
						Service.gI().getResource(3, null);
						GameCanvas.endDlg();
						SplashScr.imgLogo = null;
						SmallImage.loadBigRMS();
						mSystem.gcc();
						ServerListScreen.bigOk = true;
						ServerListScreen.loadScreen = true;
						GameScr.gI().loadGameScr();
						GameScr.isLoadAllData = false;
						Service.gI().updateData();
						if (GameCanvas.currentScreen != GameCanvas.loginScr)
						{
							GameCanvas.serverScreen.switchToMe();
						}
					}
					break;
				}
				case -43:
				{
					sbyte itemAction = msg.reader().readByte();
					sbyte where = msg.reader().readByte();
					sbyte index = msg.reader().readByte();
					string info = msg.reader().readUTF();
					GameCanvas.panel.itemRequest(itemAction, info, where, index);
					break;
				}
				case -59:
				{
					sbyte typePK = msg.reader().readByte();
					GameScr.gI().player_vs_player(msg.reader().readInt(), msg.reader().readInt(), msg.reader().readUTF(), typePK);
					break;
				}
				case -62:
				{
					int num149 = msg.reader().readUnsignedByte();
					sbyte b62 = msg.reader().readByte();
					if (b62 <= 0)
					{
						break;
					}
					ClanImage clanImage3 = ClanImage.getClanImage((short)num149);
					if (clanImage3 == null)
					{
						break;
					}
					clanImage3.idImage = new short[b62];
					for (int num150 = 0; num150 < b62; num150++)
					{
						clanImage3.idImage[num150] = msg.reader().readShort();
						if (clanImage3.idImage[num150] > 0)
						{
							SmallImage.vKeys.addElement(clanImage3.idImage[num150] + string.Empty);
						}
					}
					break;
				}
				case -65:
				{
					InfoDlg.hide();
					int num67 = msg.reader().readInt();
					sbyte b29 = msg.reader().readByte();
					if (b29 == 0)
					{
						break;
					}
					if (Char.myCharz().charID == num67)
					{
						isStopReadMessage = true;
						GameScr.lockTick = 500;
						GameScr.gI().center = null;
						if (b29 == 0 || b29 == 1 || b29 == 3)
						{
							Teleport p = new Teleport(Char.myCharz().cx, Char.myCharz().cy, Char.myCharz().head, Char.myCharz().cdir, 0, isMe: true, (b29 != 1) ? b29 : Char.myCharz().cgender);
							Teleport.addTeleport(p);
						}
						if (b29 == 2)
						{
							GameScr.lockTick = 50;
							Char.myCharz().hide();
						}
					}
					else
					{
						Char char5 = GameScr.findCharInMap(num67);
						if ((b29 == 0 || b29 == 1 || b29 == 3) && char5 != null)
						{
							char5.isUsePlane = true;
							Teleport teleport = new Teleport(char5.cx, char5.cy, char5.head, char5.cdir, 0, isMe: false, (b29 != 1) ? b29 : char5.cgender);
							teleport.id = num67;
							Teleport.addTeleport(teleport);
						}
						if (b29 == 2)
						{
							char5.hide();
						}
					}
					break;
				}
				case -64:
				{
					int num49 = msg.reader().readInt();
					int num50 = msg.reader().readShort();
					@char = null;
					@char = ((num49 != Char.myCharz().charID) ? GameScr.findCharInMap(num49) : Char.myCharz());
					if (@char == null)
					{
						return true;
					}
					@char.bag = num50;
					Effect.GetCharEff(@char);
					Res.outz("cmd:-64 UPDATE BAG PLAER = " + ((@char != null) ? @char.cName : string.Empty) + num49 + " BAG ID= " + num50);
					if (num50 == 30 && @char.me)
					{
						GameScr.isPickNgocRong = true;
					}
					break;
				}
				case -63:
				{
					Res.outz("GET BAG");
					int num51 = msg.reader().readShort();
					sbyte b23 = msg.reader().readByte();
					ClanImage clanImage = new ClanImage();
					clanImage.ID = num51;
					if (b23 > 0)
					{
						clanImage.idImage = new short[b23];
						for (int num52 = 0; num52 < b23; num52++)
						{
							clanImage.idImage[num52] = msg.reader().readShort();
							Res.outz("ID=  " + num51 + " frame= " + clanImage.idImage[num52]);
						}
						ClanImage.idImages.put(num51 + string.Empty, clanImage);
					}
					break;
				}
				case -57:
				{
					string strInvite = msg.reader().readUTF();
					int clanID = msg.reader().readInt();
					int code = msg.reader().readInt();
					GameScr.gI().clanInvite(strInvite, clanID, code);
					break;
				}
			default:
				return false;
		}
		return true;
	}

}
