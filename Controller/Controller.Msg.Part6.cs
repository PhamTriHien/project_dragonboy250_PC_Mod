using System;
using Assets.src.e;
using Assets.src.f;
using Assets.src.g;
using UnityEngine;

public partial class Controller : IMessageHandler
{
	public bool onMessage_Part6(Message msg)
	{
		Char @char = null;
		Mob mob = null;
		MyVector myVector = new MyVector();
		int num = 0;
		switch (msg.command)
		{
				case -20:
				{
					GameCanvas.debug("SA61", 2);
					Char.myCharz().itemFocus = null;
					short itemMapID = msg.reader().readShort();
					for (int num131 = 0; num131 < GameScr.vItemMap.size(); num131++)
					{
						ItemMap itemMap4 = (ItemMap)GameScr.vItemMap.elementAt(num131);
						if (itemMap4.itemMapID != itemMapID)
						{
							continue;
						}
						itemMap4.setPoint(Char.myCharz().cx, Char.myCharz().cy - 10);
						string text7 = msg.reader().readUTF();
						num = 0;
						try
						{
							num = msg.reader().readShort();
							if (itemMap4.template.type == 9)
							{
								num = msg.reader().readShort();
								Char.myCharz().xu += num;
								Char.myCharz().xuStr = Res.formatNumber(Char.myCharz().xu);
							}
							else if (itemMap4.template.type == 10)
							{
								num = msg.reader().readShort();
								Char.myCharz().luong += num;
								Char.myCharz().luongStr = mSystem.numberTostring(Char.myCharz().luong);
							}
							else if (itemMap4.template.type == 34)
							{
								num = msg.reader().readShort();
								Char.myCharz().luongKhoa += num;
								Char.myCharz().luongKhoaStr = mSystem.numberTostring(Char.myCharz().luongKhoa);
							}
						}
						catch (Exception)
						{
						}
						if (text7.Equals(string.Empty))
						{
							if (itemMap4.template.type == 9)
							{
								GameScr.startFlyText(((num >= 0) ? "+" : string.Empty) + num, Char.myCharz().cx, Char.myCharz().cy - Char.myCharz().ch, 0, -2, mFont.YELLOW);
								SoundMn.gI().getItem();
							}
							else if (itemMap4.template.type == 10)
							{
								GameScr.startFlyText(((num >= 0) ? "+" : string.Empty) + num, Char.myCharz().cx, Char.myCharz().cy - Char.myCharz().ch, 0, -2, mFont.GREEN);
								SoundMn.gI().getItem();
							}
							else if (itemMap4.template.type == 34)
							{
								GameScr.startFlyText(((num >= 0) ? "+" : string.Empty) + num, Char.myCharz().cx, Char.myCharz().cy - Char.myCharz().ch, 0, -2, mFont.RED);
								SoundMn.gI().getItem();
							}
							else
							{
								GameScr.info1.addInfo(mResources.you_receive + " " + ((num <= 0) ? string.Empty : (num + " ")) + itemMap4.template.name, 0);
								SoundMn.gI().getItem();
							}
							if (num > 0 && Char.myCharz().petFollow != null && Char.myCharz().petFollow.smallID == 4683)
							{
								ServerEffect.addServerEffect(55, Char.myCharz().petFollow.cmx, Char.myCharz().petFollow.cmy, 1);
								ServerEffect.addServerEffect(55, Char.myCharz().cx, Char.myCharz().cy, 1);
							}
						}
						else if (text7.Length == 1)
						{
							Cout.LogError3("strInf.Length =1:  " + text7);
						}
						else
						{
							GameScr.info1.addInfo(text7, 0);
						}
						break;
					}
					break;
				}
				case -19:
				{
					GameCanvas.debug("SA62", 2);
					short itemMapID = msg.reader().readShort();
					@char = GameScr.findCharInMap(msg.reader().readInt());
					for (int num130 = 0; num130 < GameScr.vItemMap.size(); num130++)
					{
						ItemMap itemMap3 = (ItemMap)GameScr.vItemMap.elementAt(num130);
						if (itemMap3.itemMapID != itemMapID)
						{
							continue;
						}
						if (@char == null)
						{
							return true;
						}
						itemMap3.setPoint(@char.cx, @char.cy - 10);
						if (itemMap3.x < @char.cx)
						{
							@char.cdir = -1;
						}
						else if (itemMap3.x > @char.cx)
						{
							@char.cdir = 1;
						}
						break;
					}
					break;
				}
				case -18:
				{
					GameCanvas.debug("SA63", 2);
					int num129 = msg.reader().readByte();
					GameScr.vItemMap.addElement(new ItemMap(msg.reader().readShort(), Char.myCharz().arrItemBag[num129].template.id, Char.myCharz().cx, Char.myCharz().cy, msg.reader().readShort(), msg.reader().readShort()));
					Char.myCharz().arrItemBag[num129] = null;
					break;
				}
				case 68:
				{
					Res.outz("ADD ITEM TO MAP --------------------------------------");
					GameCanvas.debug("SA6333", 2);
					short itemMapID = msg.reader().readShort();
					short itemTemplateID = msg.reader().readShort();
					int x = msg.reader().readShort();
					int y = msg.reader().readShort();
					int num114 = msg.reader().readInt();
					short r = 0;
					if (num114 == -2)
					{
						r = msg.reader().readShort();
					}
					ItemMap itemMap = new ItemMap(num114, itemMapID, itemTemplateID, x, y, r);
					bool flag8 = false;
					for (int num115 = 0; num115 < GameScr.vItemMap.size(); num115++)
					{
						ItemMap itemMap2 = (ItemMap)GameScr.vItemMap.elementAt(num115);
						if (itemMap2.itemMapID == itemMap.itemMapID)
						{
							flag8 = true;
							break;
						}
					}
					if (!flag8)
					{
						GameScr.vItemMap.addElement(itemMap);
					}
					break;
				}
				case 69:
					SoundMn.IsDelAcc = ((msg.reader().readByte() != 0) ? true : false);
					break;
				case -14:
					GameCanvas.debug("SA64", 2);
					@char = GameScr.findCharInMap(msg.reader().readInt());
					if (@char == null)
					{
						return true;
					}
					GameScr.vItemMap.addElement(new ItemMap(msg.reader().readShort(), msg.reader().readShort(), @char.cx, @char.cy, msg.reader().readShort(), msg.reader().readShort()));
					break;
				case -22:
					GameCanvas.debug("SA65", 2);
					Char.isLockKey = true;
					Char.ischangingMap = true;
					GameScr.gI().timeStartMap = 0;
					GameScr.gI().timeLengthMap = 0;
					Char.myCharz().mobFocus = null;
					Char.myCharz().npcFocus = null;
					Char.myCharz().charFocus = null;
					Char.myCharz().itemFocus = null;
					Char.myCharz().focus.removeAllElements();
					Char.myCharz().testCharId = -9999;
					Char.myCharz().killCharId = -9999;
					GameCanvas.resetBg();
					GameScr.gI().resetButton();
					GameScr.gI().center = null;
					if (Effect.vEffData.size() > 15)
					{
						for (int num113 = 0; num113 < 5; num113++)
						{
							Effect.vEffData.removeElementAt(0);
						}
					}
					break;
				case -70:
				{
					Res.outz("BIG MESSAGE .......................................");
					GameCanvas.endDlg();
					int avatar2 = msg.reader().readShort();
					string chat3 = msg.reader().readUTF();
					Npc npc5 = new Npc(-1, 0, 0, 0, 0, 0);
					npc5.avatar = avatar2;
					ChatPopup.addBigMessage(chat3, 100000, npc5);
					sbyte b45 = msg.reader().readByte();
					if (b45 == 0)
					{
						ChatPopup.serverChatPopUp.cmdMsg1 = new Command(mResources.CLOSE, ChatPopup.serverChatPopUp, 1001, null);
						ChatPopup.serverChatPopUp.cmdMsg1.x = GameCanvas.w / 2 - 35;
						ChatPopup.serverChatPopUp.cmdMsg1.y = GameCanvas.h - 35;
					}
					if (b45 == 1)
					{
						string p2 = msg.reader().readUTF();
						string caption2 = msg.reader().readUTF();
						ChatPopup.serverChatPopUp.cmdMsg1 = new Command(caption2, ChatPopup.serverChatPopUp, 1000, p2);
						ChatPopup.serverChatPopUp.cmdMsg1.x = GameCanvas.w / 2 - 75;
						ChatPopup.serverChatPopUp.cmdMsg1.y = GameCanvas.h - 35;
						ChatPopup.serverChatPopUp.cmdMsg2 = new Command(mResources.CLOSE, ChatPopup.serverChatPopUp, 1001, null);
						ChatPopup.serverChatPopUp.cmdMsg2.x = GameCanvas.w / 2 + 11;
						ChatPopup.serverChatPopUp.cmdMsg2.y = GameCanvas.h - 35;
					}
					break;
				}
				case 38:
				{
					GameCanvas.debug("SA67", 2);
					InfoDlg.hide();
					int num76 = msg.reader().readShort();
					Res.outz("OPEN_UI_SAY ID= " + num76);
					string str = msg.reader().readUTF();
					str = Res.changeString(str);
					for (int num109 = 0; num109 < GameScr.vNpc.size(); num109++)
					{
						Npc npc3 = (Npc)GameScr.vNpc.elementAt(num109);
						Res.outz("npc id= " + npc3.template.npcTemplateId);
						if (npc3.template.npcTemplateId == num76)
						{
							ChatPopup.addChatPopupMultiLine(str, 100000, npc3);
							GameCanvas.panel.hideNow();
							return true;
						}
					}
					Npc npc4 = new Npc(num76, 0, 0, 0, num76, GameScr.info1.charId[Char.myCharz().cgender][2]);
					if (npc4.template.npcTemplateId == 5)
					{
						npc4.charID = 5;
					}
					try
					{
						npc4.avatar = msg.reader().readShort();
					}
					catch (Exception)
					{
					}
					ChatPopup.addChatPopupMultiLine(str, 100000, npc4);
					GameCanvas.panel.hideNow();
					break;
				}
				case 32:
				{
					GameCanvas.debug("SA68", 2);
					int num76 = msg.reader().readShort();
					for (int num77 = 0; num77 < GameScr.vNpc.size(); num77++)
					{
						Npc npc = (Npc)GameScr.vNpc.elementAt(num77);
						if (npc.template.npcTemplateId == num76 && npc.Equals(Char.myCharz().npcFocus))
						{
							string chat = msg.reader().readUTF();
							string[] array7 = new string[msg.reader().readByte()];
							for (int num78 = 0; num78 < array7.Length; num78++)
							{
								array7[num78] = msg.reader().readUTF();
							}
							GameScr.gI().createMenu(array7, npc);
							ChatPopup.addChatPopup(chat, 100000, npc);
							return true;
						}
					}
					Npc npc2 = new Npc(num76, 0, -100, 100, num76, GameScr.info1.charId[Char.myCharz().cgender][2]);
					Res.outz((Char.myCharz().npcFocus == null) ? "null" : "!null");
					string chat2 = msg.reader().readUTF();
					string[] array8 = new string[msg.reader().readByte()];
					for (int num79 = 0; num79 < array8.Length; num79++)
					{
						array8[num79] = msg.reader().readUTF();
					}
					try
					{
						short avatar = msg.reader().readShort();
						npc2.avatar = avatar;
					}
					catch (Exception)
					{
					}
					Res.outz((Char.myCharz().npcFocus == null) ? "null" : "!null");
					GameScr.gI().createMenu(array8, npc2);
					ChatPopup.addChatPopup(chat2, 100000, npc2);
					break;
				}
				case 7:
				{
					sbyte type = msg.reader().readByte();
					short id2 = msg.reader().readShort();
					string info2 = msg.reader().readUTF();
					GameCanvas.panel.saleRequest(type, info2, id2);
					break;
				}
				case 6:
					GameCanvas.debug("SA70", 2);
					Char.myCharz().xu = msg.reader().readLong();
					Char.myCharz().luong = msg.reader().readInt();
					Char.myCharz().luongKhoa = msg.reader().readInt();
					Char.myCharz().xuStr = Res.formatNumber(Char.myCharz().xu);
					Char.myCharz().luongStr = mSystem.numberTostring(Char.myCharz().luong);
					Char.myCharz().luongKhoaStr = mSystem.numberTostring(Char.myCharz().luongKhoa);
					GameCanvas.endDlg();
					break;
				case -24:
					Res.outz("***************MAP_INFO**************");
					GameScr.isPickNgocRong = false;
					Char.isLoadingMap = true;
					Cout.println("GET MAP INFO");
					GameScr.gI().magicTree = null;
					GameCanvas.isLoading = true;
					GameCanvas.debug("SA75", 2);
					GameScr.resetAllvector();
					GameCanvas.endDlg();
					TileMap.vGo.removeAllElements();
					PopUp.vPopups.removeAllElements();
					mSystem.gcc();
					TileMap.mapID = msg.reader().readUnsignedByte();
					TileMap.planetID = msg.reader().readByte();
					TileMap.tileID = msg.reader().readByte();
					TileMap.bgID = msg.reader().readByte();
					GameScr.isPaint_CT = TileMap.mapID != 170;
					Cout.println("load planet from server: " + TileMap.planetID + "bgType= " + TileMap.bgType + ".............................");
					TileMap.typeMap = msg.reader().readByte();
					TileMap.mapName = msg.reader().readUTF();
					TileMap.zoneID = msg.reader().readByte();
					GameCanvas.debug("SA75x1", 2);
					try
					{
						TileMap.loadMapFromResource(TileMap.mapID);
					}
					catch (Exception)
					{
						Service.gI().requestMaptemplate(TileMap.mapID);
						messWait = msg;
						break;
					}
					loadInfoMap(msg);
					try
					{
						sbyte b28 = msg.reader().readByte();
						TileMap.isMapDouble = ((b28 != 0) ? true : false);
					}
					catch (Exception)
					{
					}
					GameScr.cmx = GameScr.cmtoX;
					GameScr.cmy = GameScr.cmtoY;
					GameCanvas.isRequestMapID = 2;
					GameCanvas.waitingTimeChangeMap = mSystem.currentTimeMillis() + 1000;
					break;
				case -31:
				{
					TileMap.vItemBg.removeAllElements();
					short num64 = msg.reader().readShort();
					Res.err("[ITEM_BACKGROUND] nItem= " + num64);
					for (int num65 = 0; num65 < num64; num65++)
					{
						BgItem bgItem = new BgItem();
						bgItem.id = num65;
						bgItem.idImage = msg.reader().readShort();
						bgItem.layer = msg.reader().readByte();
						bgItem.dx = msg.reader().readShort();
						bgItem.dy = msg.reader().readShort();
						sbyte b27 = msg.reader().readByte();
						bgItem.tileX = new int[b27];
						bgItem.tileY = new int[b27];
						for (int num66 = 0; num66 < b27; num66++)
						{
							bgItem.tileX[num65] = msg.reader().readByte();
							bgItem.tileY[num65] = msg.reader().readByte();
						}
						TileMap.vItemBg.addElement(bgItem);
					}
					break;
				}
				case -4:
				{
					GameCanvas.debug("SA76", 2);
					@char = GameScr.findCharInMap(msg.reader().readInt());
					if (@char == null)
					{
						return true;
					}
					GameCanvas.debug("SA76v1", 2);
					if ((TileMap.tileTypeAtPixel(@char.cx, @char.cy) & 2) == 2)
					{
						@char.setSkillPaint(GameScr.sks[msg.reader().readUnsignedByte()], 0);
					}
					else
					{
						@char.setSkillPaint(GameScr.sks[msg.reader().readUnsignedByte()], 1);
					}
					GameCanvas.debug("SA76v2", 2);
					@char.attMobs = new Mob[msg.reader().readByte()];
					for (int num42 = 0; num42 < @char.attMobs.Length; num42++)
					{
						Mob mob6 = (Mob)GameScr.vMob.elementAt(msg.reader().readByte());
						@char.attMobs[num42] = mob6;
						if (num42 == 0)
						{
							if (@char.cx <= mob6.x)
							{
								@char.cdir = 1;
							}
							else
							{
								@char.cdir = -1;
							}
						}
					}
					GameCanvas.debug("SA76v3", 2);
					@char.charFocus = null;
					@char.mobFocus = @char.attMobs[0];
					Char[] array = new Char[10];
					num = 0;
					try
					{
						for (num = 0; num < array.Length; num++)
						{
							int num21 = msg.reader().readInt();
							Char char4 = (array[num] = ((num21 != Char.myCharz().charID) ? GameScr.findCharInMap(num21) : Char.myCharz()));
							if (num == 0)
							{
								if (@char.cx <= char4.cx)
								{
									@char.cdir = 1;
								}
								else
								{
									@char.cdir = -1;
								}
							}
						}
					}
					catch (Exception ex5)
					{
						Cout.println("Loi PLAYER_ATTACK_N_P " + ex5.ToString());
					}
					GameCanvas.debug("SA76v4", 2);
					if (num > 0)
					{
						@char.attChars = new Char[num];
						for (num = 0; num < @char.attChars.Length; num++)
						{
							@char.attChars[num] = array[num];
						}
						@char.charFocus = @char.attChars[0];
						@char.mobFocus = null;
					}
					GameCanvas.debug("SA76v5", 2);
					break;
				}
				case 54:
				{
					@char = GameScr.findCharInMap(msg.reader().readInt());
					if (@char == null)
					{
						return true;
					}
					int num16 = msg.reader().readUnsignedByte();
					if ((TileMap.tileTypeAtPixel(@char.cx, @char.cy) & 2) == 2)
					{
						@char.setSkillPaint(GameScr.sks[num16], 0);
					}
					else
					{
						@char.setSkillPaint(GameScr.sks[num16], 1);
					}
					Mob[] array3 = new Mob[10];
					num = 0;
					try
					{
						for (num = 0; num < array3.Length; num++)
						{
							Mob mob2 = (array3[num] = (Mob)GameScr.vMob.elementAt(msg.reader().readByte()));
							if (num == 0)
							{
								if (@char.cx <= mob2.x)
								{
									@char.cdir = 1;
								}
								else
								{
									@char.cdir = -1;
								}
							}
						}
					}
					catch (Exception)
					{
					}
					if (num > 0)
					{
						@char.attMobs = new Mob[num];
						for (num = 0; num < @char.attMobs.Length; num++)
						{
							@char.attMobs[num] = array3[num];
						}
						@char.charFocus = null;
						@char.mobFocus = @char.attMobs[0];
					}
					break;
				}
				case -60:
				{
					GameCanvas.debug("SA7666", 2);
					int num2 = msg.reader().readInt();
					int num3 = -1;
					if (num2 != Char.myCharz().charID)
					{
						Char char2 = GameScr.findCharInMap(num2);
						if (char2 == null)
						{
							return true;
						}
						if (char2.currentMovePoint != null)
						{
							char2.createShadow(char2.cx, char2.cy, 10);
							char2.cx = char2.currentMovePoint.xEnd;
							char2.cy = char2.currentMovePoint.yEnd;
						}
						int num4 = msg.reader().readUnsignedByte();
						if ((TileMap.tileTypeAtPixel(char2.cx, char2.cy) & 2) == 2)
						{
							char2.setSkillPaint(GameScr.sks[num4], 0);
						}
						else
						{
							char2.setSkillPaint(GameScr.sks[num4], 1);
						}
						sbyte b = msg.reader().readByte();
						Char[] array = new Char[b];
						for (num = 0; num < array.Length; num++)
						{
							num3 = msg.reader().readInt();
							Char char3;
							if (num3 == Char.myCharz().charID)
							{
								char3 = Char.myCharz();
								if (!GameScr.isChangeZone && GameScr.isAutoPlay && GameScr.canAutoPlay)
								{
									Service.gI().requestChangeZone(-1, -1);
									GameScr.isChangeZone = true;
								}
							}
							else
							{
								char3 = GameScr.findCharInMap(num3);
							}
							array[num] = char3;
							if (num == 0)
							{
								if (char2.cx <= char3.cx)
								{
									char2.cdir = 1;
								}
								else
								{
									char2.cdir = -1;
								}
							}
						}
						if (num > 0)
						{
							char2.attChars = new Char[num];
							for (num = 0; num < char2.attChars.Length; num++)
							{
								char2.attChars[num] = array[num];
							}
							char2.mobFocus = null;
							char2.charFocus = char2.attChars[0];
						}
					}
					else
					{
						sbyte b2 = msg.reader().readByte();
						sbyte b3 = msg.reader().readByte();
						num3 = msg.reader().readInt();
					}
					try
					{
						sbyte b4 = msg.reader().readByte();
						Res.outz("isRead continue = " + b4);
						if (b4 != 1)
						{
							break;
						}
						sbyte b5 = msg.reader().readByte();
						Res.outz("type skill = " + b5);
						if (num3 == Char.myCharz().charID)
						{
							bool flag = false;
							@char = Char.myCharz();
							long num5 = msg.reader().readLong();
							Res.outz("dame hit = " + num5);
							@char.isDie = msg.reader().readBoolean();
							if (@char.isDie)
							{
								Char.isLockKey = true;
							}
							Res.outz("isDie=" + @char.isDie + "---------------------------------------");
							int num6 = 0;
							flag = (@char.isCrit = msg.reader().readBoolean());
							@char.isMob = false;
							num5 = (@char.damHP = num5 + num6);
							if (b5 == 0)
							{
								@char.doInjure(num5, 0L, flag, isMob: false);
							}
						}
						else
						{
							@char = GameScr.findCharInMap(num3);
							if (@char == null)
							{
								return true;
							}
							bool flag2 = false;
							long num7 = msg.reader().readLong();
							Res.outz("dame hit= " + num7);
							@char.isDie = msg.reader().readBoolean();
							Res.outz("isDie=" + @char.isDie + "---------------------------------------");
							int num8 = 0;
							flag2 = (@char.isCrit = msg.reader().readBoolean());
							@char.isMob = false;
							num7 = (@char.damHP = num7 + num8);
							if (b5 == 0)
							{
								@char.doInjure(num7, 0L, flag2, isMob: false);
							}
						}
					}
					catch (Exception)
					{
					}
					break;
				}
			default:
				return false;
		}
		return true;
	}

}
