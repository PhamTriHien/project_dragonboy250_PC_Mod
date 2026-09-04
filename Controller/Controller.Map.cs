using System;
using Assets.src.e;
using Assets.src.f;
using Assets.src.g;
using UnityEngine;

public partial class Controller : IMessageHandler
{
	private void createMap(myReader d)
		{
			GameScr.vcMap = d.readByte();
			TileMap.mapNames = new string[d.readShort()];
			for (int i = 0; i < TileMap.mapNames.Length; i++)
			{
				TileMap.mapNames[i] = d.readUTF();
			}
			Npc.arrNpcTemplate = new NpcTemplate[d.readByte()];
			for (sbyte b = 0; b < Npc.arrNpcTemplate.Length; b++)
			{
				Npc.arrNpcTemplate[b] = new NpcTemplate();
				Npc.arrNpcTemplate[b].npcTemplateId = b;
				Npc.arrNpcTemplate[b].name = d.readUTF();
				Npc.arrNpcTemplate[b].headId = d.readShort();
				Npc.arrNpcTemplate[b].bodyId = d.readShort();
				Npc.arrNpcTemplate[b].legId = d.readShort();
				Npc.arrNpcTemplate[b].menu = new string[d.readByte()][];
				for (int j = 0; j < Npc.arrNpcTemplate[b].menu.Length; j++)
				{
					Npc.arrNpcTemplate[b].menu[j] = new string[d.readByte()];
					for (int k = 0; k < Npc.arrNpcTemplate[b].menu[j].Length; k++)
					{
						Npc.arrNpcTemplate[b].menu[j][k] = d.readUTF();
					}
				}
			}
			Mob.arrMobTemplate = new MobTemplate[d.readShort()];
			for (int l = 0; l < Mob.arrMobTemplate.Length; l++)
			{
				Mob.arrMobTemplate[l] = new MobTemplate();
				Mob.arrMobTemplate[l].mobTemplateId = l;
				Mob.arrMobTemplate[l].type = d.readByte();
				Mob.arrMobTemplate[l].name = d.readUTF();
				Mob.arrMobTemplate[l].hp = d.readLong();
				Mob.arrMobTemplate[l].rangeMove = d.readByte();
				Mob.arrMobTemplate[l].speed = d.readByte();
				Mob.arrMobTemplate[l].dartType = d.readByte();
			}
		}

	public void loadCurrMap(sbyte teleport3)
		{
			Res.outz("[CONTROLER] start load map " + teleport3);
			GameScr.gI().auto = 0;
			GameScr.isChangeZone = false;
			CreateCharScr.instance = null;
			GameScr.info1.isUpdate = false;
			GameScr.info2.isUpdate = false;
			GameScr.lockTick = 0;
			GameCanvas.panel.isShow = false;
			SoundMn.gI().stopAll();
			if (!GameScr.isLoadAllData && !CreateCharScr.isCreateChar)
			{
				GameScr.gI().initSelectChar();
			}
			GameScr.loadCamera(fullmScreen: false, (teleport3 != 1) ? (-1) : Char.myCharz().cx, (teleport3 == 0) ? (-1) : 0);
			TileMap.loadMainTile();
			TileMap.loadMap(TileMap.tileID);
			Res.outz("LOAD GAMESCR 2");
			Char.myCharz().cvx = 0;
			Char.myCharz().statusMe = 4;
			Char.myCharz().currentMovePoint = null;
			Char.myCharz().mobFocus = null;
			Char.myCharz().charFocus = null;
			Char.myCharz().npcFocus = null;
			Char.myCharz().itemFocus = null;
			Char.myCharz().skillPaint = null;
			Char.myCharz().setMabuHold(m: false);
			Char.myCharz().skillPaintRandomPaint = null;
			GameCanvas.clearAllPointerEvent();
			if (Char.myCharz().cy >= TileMap.pxh - 100)
			{
				Char.myCharz().isFlyUp = true;
				Char.myCharz().cx += Res.abs(Res.random(0, 80));
				Service.gI().charMove();
			}
			GameScr.gI().loadGameScr();
			GameCanvas.loadBG(TileMap.bgID);
			Char.isLockKey = false;
			Res.outz("cy= " + Char.myCharz().cy + "---------------------------------------------");
			for (int i = 0; i < Char.myCharz().vEff.size(); i++)
			{
				EffectChar effectChar = (EffectChar)Char.myCharz().vEff.elementAt(i);
				if (effectChar.template.type == 10)
				{
					Char.isLockKey = true;
					break;
				}
			}
			GameCanvas.clearKeyHold();
			GameCanvas.clearKeyPressed();
			GameScr.gI().dHP = Char.myCharz().cHP;
			GameScr.gI().dMP = Char.myCharz().cMP;
			Char.ischangingMap = false;
			GameScr.gI().switchToMe();
			if (Char.myCharz().cy <= 10 && teleport3 != 0 && teleport3 != 2)
			{
				Teleport p = new Teleport(Char.myCharz().cx, Char.myCharz().cy, Char.myCharz().head, Char.myCharz().cdir, 1, isMe: true, (teleport3 != 1) ? teleport3 : Char.myCharz().cgender);
				Teleport.addTeleport(p);
				Char.myCharz().isTeleport = true;
			}
			if (teleport3 == 2)
			{
				Char.myCharz().show();
			}
			if (GameScr.gI().isRongThanXuatHien)
			{
				if (TileMap.mapID == GameScr.gI().mapRID && TileMap.zoneID == GameScr.gI().zoneRID)
				{
					GameScr.gI().callRongThan(GameScr.gI().xR, GameScr.gI().yR);
				}
				if (mGraphics.zoomLevel > 1)
				{
					GameScr.gI().doiMauTroi();
				}
			}
			InfoDlg.hide();
			InfoDlg.show(TileMap.mapName, mResources.zone + " " + TileMap.zoneID, 30);
			GameCanvas.endDlg();
			GameCanvas.isLoading = false;
			Hint.clickMob();
			Hint.clickNpc();
			GameCanvas.debug("SA75x9", 2);
			GameCanvas.isRequestMapID = 2;
			GameCanvas.waitingTimeChangeMap = mSystem.currentTimeMillis() + 1000;
			Res.outz("[CONTROLLER] loadMap DONE!!!!!!!!!");
		}

	public void loadInfoMap(Message msg)
		{
			try
			{
				if (mGraphics.zoomLevel == 1)
				{
					SmallImage.clearHastable();
				}
				Char.myCharz().cx = (Char.myCharz().cxSend = (Char.myCharz().cxFocus = msg.reader().readShort()));
				Char.myCharz().cy = (Char.myCharz().cySend = (Char.myCharz().cyFocus = msg.reader().readShort()));
				Char.myCharz().xSd = Char.myCharz().cx;
				Char.myCharz().ySd = Char.myCharz().cy;
				Res.outz("head= " + Char.myCharz().head + " body= " + Char.myCharz().body + " left= " + Char.myCharz().leg + " x= " + Char.myCharz().cx + " y= " + Char.myCharz().cy + " chung toc= " + Char.myCharz().cgender);
				if (Char.myCharz().cx >= 0 && Char.myCharz().cx <= 100)
				{
					Char.myCharz().cdir = 1;
				}
				else if (Char.myCharz().cx >= TileMap.tmw - 100 && Char.myCharz().cx <= TileMap.tmw)
				{
					Char.myCharz().cdir = -1;
				}
				GameCanvas.debug("SA75x4", 2);
				int num = msg.reader().readByte();
				Res.outz("vGo size= " + num);
				if (!GameScr.info1.isDone)
				{
					GameScr.info1.cmx = Char.myCharz().cx - GameScr.cmx;
					GameScr.info1.cmy = Char.myCharz().cy - GameScr.cmy;
				}
				for (int i = 0; i < num; i++)
				{
					Waypoint waypoint = new Waypoint(msg.reader().readShort(), msg.reader().readShort(), msg.reader().readShort(), msg.reader().readShort(), msg.reader().readBoolean(), msg.reader().readBoolean(), msg.reader().readUTF());
					if ((TileMap.mapID != 21 && TileMap.mapID != 22 && TileMap.mapID != 23) || waypoint.minX < 0 || waypoint.minX <= 24)
					{
					}
				}
				Resources.UnloadUnusedAssets();
				GC.Collect();
				GameCanvas.debug("SA75x5", 2);
				num = msg.reader().readByte();
				Mob.newMob.removeAllElements();
				for (sbyte b = 0; b < num; b++)
				{
					Mob mob = new Mob(b, msg.reader().readBoolean(), msg.reader().readBoolean(), msg.reader().readBoolean(), msg.reader().readBoolean(), msg.reader().readBoolean(), msg.reader().readShort(), msg.reader().readByte(), msg.reader().readLong(), msg.reader().readByte(), msg.reader().readLong(), msg.reader().readShort(), msg.reader().readShort(), msg.reader().readByte(), msg.reader().readByte());
					mob.xSd = mob.x;
					mob.ySd = mob.y;
					mob.isBoss = msg.reader().readBoolean();
					if (Mob.arrMobTemplate[mob.templateId].type != 0)
					{
						if (b % 3 == 0)
						{
							mob.dir = -1;
						}
						else
						{
							mob.dir = 1;
						}
						mob.x += 10 - b % 20;
					}
					mob.isMobMe = false;
					BigBoss bigBoss = null;
					BachTuoc bachTuoc = null;
					BigBoss2 bigBoss2 = null;
					NewBoss newBoss = null;
					if (mob.templateId == 70)
					{
						bigBoss = new BigBoss(b, (short)mob.x, (short)mob.y, 70, mob.hp, mob.maxHp, mob.sys);
					}
					if (mob.templateId == 71)
					{
						bachTuoc = new BachTuoc(b, (short)mob.x, (short)mob.y, 71, mob.hp, mob.maxHp, mob.sys);
					}
					if (mob.templateId == 72)
					{
						bigBoss2 = new BigBoss2(b, (short)mob.x, (short)mob.y, 72, mob.hp, mob.maxHp, 3);
					}
					if (mob.isBoss)
					{
						newBoss = new NewBoss(b, (short)mob.x, (short)mob.y, mob.templateId, mob.hp, mob.maxHp, mob.sys);
						string bName = (Mob.arrMobTemplate != null && mob.templateId >= 0 && mob.templateId < Mob.arrMobTemplate.Length && Mob.arrMobTemplate[mob.templateId] != null) ? Mob.arrMobTemplate[mob.templateId].name : "Boss";
						ModMenu.AddBossNotice(bName, TileMap.mapName, DateTime.Now.ToString("HH:mm:ss"));
					}
					if (newBoss != null)
					{
						GameScr.vMob.addElement(newBoss);
					}
					else if (bigBoss != null)
					{
						GameScr.vMob.addElement(bigBoss);
						ModMenu.AddBossNotice("BigBoss", TileMap.mapName, DateTime.Now.ToString("HH:mm:ss"));
					}
					else if (bachTuoc != null)
					{
						GameScr.vMob.addElement(bachTuoc);
						ModMenu.AddBossNotice("B\u1EA1ch Tu\u1ED9c", TileMap.mapName, DateTime.Now.ToString("HH:mm:ss"));
					}
					else if (bigBoss2 != null)
					{
						GameScr.vMob.addElement(bigBoss2);
						ModMenu.AddBossNotice("BigBoss 2", TileMap.mapName, DateTime.Now.ToString("HH:mm:ss"));
					}
					else
					{
						GameScr.vMob.addElement(mob);
					}
				}
				if (Char.myCharz().mobMe != null && GameScr.findMobInMap(Char.myCharz().mobMe.mobId) == null)
				{
					Char.myCharz().mobMe.getData();
					Char.myCharz().mobMe.x = Char.myCharz().cx;
					Char.myCharz().mobMe.y = Char.myCharz().cy - 40;
					GameScr.vMob.addElement(Char.myCharz().mobMe);
				}
				num = msg.reader().readByte();
				for (byte b2 = 0; b2 < num; b2++)
				{
				}
				GameCanvas.debug("SA75x6", 2);
				num = msg.reader().readByte();
				Res.outz("NPC size= " + num);
				for (int j = 0; j < num; j++)
				{
					sbyte b3 = msg.reader().readByte();
					short cx = msg.reader().readShort();
					short num2 = msg.reader().readShort();
					sbyte b4 = msg.reader().readByte();
					short num3 = msg.reader().readShort();
					if (b4 != 6 && ((Char.myCharz().taskMaint.taskId >= 7 && (Char.myCharz().taskMaint.taskId != 7 || Char.myCharz().taskMaint.index > 1)) || (b4 != 7 && b4 != 8 && b4 != 9)) && (Char.myCharz().taskMaint.taskId >= 6 || b4 != 16))
					{
						if (b4 == 4)
						{
							GameScr.gI().magicTree = new MagicTree(j, b3, cx, num2, b4, num3);
							Service.gI().magicTree(2);
							GameScr.vNpc.addElement(GameScr.gI().magicTree);
						}
						else
						{
							Npc o = new Npc(j, b3, cx, num2 + 3, b4, num3);
							GameScr.vNpc.addElement(o);
						}
					}
				}
				GameCanvas.debug("SA75x7", 2);
				num = msg.reader().readByte();
				string empty = string.Empty;
				Res.outz("item size = " + num);
				empty = empty + "item: " + num;
				for (int k = 0; k < num; k++)
				{
					short itemMapID = msg.reader().readShort();
					short num4 = msg.reader().readShort();
					int x = msg.reader().readShort();
					int y = msg.reader().readShort();
					int num5 = msg.reader().readInt();
					short r = 0;
					if (num5 == -2)
					{
						r = msg.reader().readShort();
					}
					ItemMap itemMap = new ItemMap(num5, itemMapID, num4, x, y, r);
					bool flag = false;
					for (int l = 0; l < GameScr.vItemMap.size(); l++)
					{
						ItemMap itemMap2 = (ItemMap)GameScr.vItemMap.elementAt(l);
						if (itemMap2.itemMapID == itemMap.itemMapID)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						GameScr.vItemMap.addElement(itemMap);
					}
					empty = empty + num4 + ",";
				}
				Res.err("sl item on map " + empty + "\n");
				TileMap.vCurrItem.removeAllElements();
				if (mGraphics.zoomLevel == 1)
				{
					BgItem.clearHashTable();
				}
				BgItem.vKeysNew.removeAllElements();
				if (!GameCanvas.lowGraphic || (GameCanvas.lowGraphic && TileMap.isVoDaiMap()) || TileMap.mapID == 45 || TileMap.mapID == 46 || TileMap.mapID == 47 || TileMap.mapID == 48 || TileMap.mapID == 120 || TileMap.mapID == 128 || TileMap.mapID == 170 || TileMap.mapID == 49)
				{
					short num6 = msg.reader().readShort();
					empty = "item high graphic: ";
					for (int m = 0; m < num6; m++)
					{
						short num7 = msg.reader().readShort();
						short num8 = msg.reader().readShort();
						short num9 = msg.reader().readShort();
						if (TileMap.getBIById(num7) != null)
						{
							BgItem bIById = TileMap.getBIById(num7);
							BgItem bgItem = new BgItem();
							bgItem.id = num7;
							bgItem.idImage = bIById.idImage;
							bgItem.dx = bIById.dx;
							bgItem.dy = bIById.dy;
							bgItem.x = num8 * TileMap.size;
							bgItem.y = num9 * TileMap.size;
							bgItem.layer = bIById.layer;
							if (TileMap.isExistMoreOne(bgItem.id))
							{
								bgItem.trans = ((m % 2 != 0) ? 2 : 0);
								if (TileMap.mapID == 45)
								{
									bgItem.trans = 0;
								}
							}
							Image image = null;
							if (!BgItem.imgNew.containsKey(bgItem.idImage + string.Empty))
							{
								if (mGraphics.zoomLevel == 1)
								{
									image = GameCanvas.loadImage("/mapBackGround/" + bgItem.idImage + ".png");
									if (image == null)
									{
										image = Image.createRGBImage(new int[1], 1, 1, bl: true);
										Service.gI().getBgTemplate(bgItem.idImage);
									}
									BgItem.imgNew.put(bgItem.idImage + string.Empty, image);
								}
								else
								{
									bool flag2 = false;
									sbyte[] array = Rms.loadRMS(mGraphics.zoomLevel + "bgItem" + bgItem.idImage);
									if (array != null)
									{
										if (BgItem.newSmallVersion != null)
										{
											Res.outz("Small  last= " + array.Length % 127 + "new Version= " + BgItem.newSmallVersion[bgItem.idImage]);
											if (array.Length % 127 != BgItem.newSmallVersion[bgItem.idImage])
											{
												flag2 = true;
											}
										}
										if (!flag2)
										{
											image = Image.createImage(array, 0, array.Length);
											if (image != null)
											{
												BgItem.imgNew.put(bgItem.idImage + string.Empty, image);
											}
											else
											{
												flag2 = true;
											}
										}
									}
									else
									{
										flag2 = true;
									}
									if (flag2)
									{
										image = GameCanvas.loadImage("/mapBackGround/" + bgItem.idImage + ".png");
										if (image == null)
										{
											image = Image.createRGBImage(new int[1], 1, 1, bl: true);
											Service.gI().getBgTemplate(bgItem.idImage);
										}
										BgItem.imgNew.put(bgItem.idImage + string.Empty, image);
									}
								}
								BgItem.vKeysLast.addElement(bgItem.idImage + string.Empty);
							}
							if (!BgItem.isExistKeyNews(bgItem.idImage + string.Empty))
							{
								BgItem.vKeysNew.addElement(bgItem.idImage + string.Empty);
							}
							bgItem.changeColor();
							TileMap.vCurrItem.addElement(bgItem);
						}
						empty = empty + num7 + ",";
					}
					Res.err("item High Graphics: " + empty);
					for (int n = 0; n < BgItem.vKeysLast.size(); n++)
					{
						string text = (string)BgItem.vKeysLast.elementAt(n);
						if (!BgItem.isExistKeyNews(text))
						{
							BgItem.imgNew.remove(text);
							if (BgItem.imgNew.containsKey(text + "blend" + 1))
							{
								BgItem.imgNew.remove(text + "blend" + 1);
							}
							if (BgItem.imgNew.containsKey(text + "blend" + 3))
							{
								BgItem.imgNew.remove(text + "blend" + 3);
							}
							BgItem.vKeysLast.removeElementAt(n);
							n--;
						}
					}
					BackgroudEffect.isFog = false;
					BackgroudEffect.nCloud = 0;
					EffecMn.vEff.removeAllElements();
					BackgroudEffect.vBgEffect.removeAllElements();
					Effect.newEff.removeAllElements();
					short num10 = msg.reader().readShort();
					for (int num11 = 0; num11 < num10; num11++)
					{
						string key = msg.reader().readUTF();
						string value = msg.reader().readUTF();
						keyValueAction(key, value);
					}
				}
				else
				{
					short num12 = msg.reader().readShort();
					for (int num13 = 0; num13 < num12; num13++)
					{
						short num14 = msg.reader().readShort();
						short num15 = msg.reader().readShort();
						short num16 = msg.reader().readShort();
					}
					short num17 = msg.reader().readShort();
					for (int num18 = 0; num18 < num17; num18++)
					{
						string text2 = msg.reader().readUTF();
						string text3 = msg.reader().readUTF();
					}
				}
				TileMap.bgType = msg.reader().readByte();
				sbyte teleport = msg.reader().readByte();
				loadCurrMap(teleport);
				GameCanvas.debug("SA75x8", 2);
			}
			catch (Exception)
			{
				Res.err(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> Loadmap khong thanh cong");
				GameCanvas.instance.doResetToLoginScr(GameCanvas.serverScreen);
				ServerListScreen.waitToLogin = true;
				GameCanvas.endDlg();
			}
			GameCanvas.isLoading = false;
			Res.err(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> Loadmap thanh cong");
		}

	public void messageNotMap(Message msg)
		{
			GameCanvas.debug("SA6", 2);
			try
			{
				sbyte b = msg.reader().readByte();
				Res.outz("---messageNotMap : " + b);
				switch (b)
				{
				case 16:
					MoneyCharge.gI().switchToMe();
					break;
				case 17:
					GameCanvas.debug("SYB123", 2);
					Char.myCharz().clearTask();
					break;
				case 18:
				{
					GameCanvas.isLoading = false;
					GameCanvas.endDlg();
					int num2 = msg.reader().readInt();
					GameCanvas.inputDlg.show(mResources.changeNameChar, new Command(mResources.OK, GameCanvas.instance, 88829, num2), TField.INPUT_TYPE_ANY);
					break;
				}
				case 20:
					Char.myCharz().cPk = msg.reader().readByte();
					GameScr.info1.addInfo(mResources.PK_NOW + " " + Char.myCharz().cPk, 0);
					break;
				case 35:
					GameCanvas.endDlg();
					GameScr.gI().resetButton();
					GameScr.info1.addInfo(msg.reader().readUTF(), 0);
					break;
				case 36:
					GameScr.typeActive = msg.reader().readByte();
					Res.outz("load Me Active: " + GameScr.typeActive);
					break;
				case 4:
				{
					GameCanvas.debug("SA8", 2);
					GameCanvas.loginScr.savePass();
					GameScr.isAutoPlay = false;
					GameScr.canAutoPlay = false;
					LoginScr.isUpdateAll = true;
					LoginScr.isUpdateData = true;
					LoginScr.isUpdateMap = true;
					LoginScr.isUpdateSkill = true;
					LoginScr.isUpdateItem = true;
					GameScr.vsData = msg.reader().readByte();
					GameScr.vsMap = msg.reader().readByte();
					GameScr.vsSkill = msg.reader().readByte();
					GameScr.vsItem = msg.reader().readByte();
					sbyte b3 = msg.reader().readByte();
					if (GameCanvas.loginScr.isLogin2)
					{
						Rms.saveRMSString(Rms.RMS_acc, string.Empty);
						Rms.saveRMSString(Rms.RMS_pass, string.Empty);
					}
					else
					{
						Rms.saveRMSString(Rms.RMS_userAo + ServerListScreen.ipSelect, string.Empty);
					}
					if (GameScr.vsData != GameScr.vcData)
					{
						GameScr.isLoadAllData = false;
						Service.gI().updateData();
					}
					else
					{
						try
						{
							LoginScr.isUpdateData = false;
						}
						catch (Exception)
						{
							GameScr.vcData = -1;
							Service.gI().updateData();
						}
					}
					if (GameScr.vsMap != GameScr.vcMap)
					{
						GameScr.isLoadAllData = false;
						Service.gI().updateMap();
					}
					else
					{
						try
						{
							if (!GameScr.isLoadAllData)
							{
								DataInputStream dataInputStream = new DataInputStream(Rms.loadRMS("NRmap"));
								createMap(dataInputStream.r);
							}
							LoginScr.isUpdateMap = false;
						}
						catch (Exception)
						{
							GameScr.vcMap = -1;
							Service.gI().updateMap();
						}
					}
					if (GameScr.vsSkill != GameScr.vcSkill)
					{
						GameScr.isLoadAllData = false;
						Service.gI().updateSkill();
					}
					else
					{
						try
						{
							if (!GameScr.isLoadAllData)
							{
								DataInputStream dataInputStream2 = new DataInputStream(Rms.loadRMS("NRskill"));
								createSkill(dataInputStream2.r);
							}
							LoginScr.isUpdateSkill = false;
						}
						catch (Exception)
						{
							GameScr.vcSkill = -1;
							Service.gI().updateSkill();
						}
					}
					if (GameScr.vsItem != GameScr.vcItem)
					{
						GameScr.isLoadAllData = false;
						Service.gI().updateItem();
					}
					else
					{
						try
						{
							DataInputStream dataInputStream3 = new DataInputStream(Rms.loadRMS("NRitem0"));
							loadItemNew(dataInputStream3.r, 0, isSave: false);
							DataInputStream dataInputStream4 = new DataInputStream(Rms.loadRMS("NRitem1"));
							loadItemNew(dataInputStream4.r, 1, isSave: false);
							DataInputStream dataInputStream5 = new DataInputStream(Rms.loadRMS("NRitem100"));
							loadItemNew(dataInputStream5.r, 100, isSave: false);
							LoginScr.isUpdateItem = false;
						}
						catch (Exception)
						{
							GameScr.vcItem = -1;
							Service.gI().updateItem();
						}
						try
						{
							DataInputStream dataInputStream6 = new DataInputStream(Rms.loadRMS("NRitem101"));
							loadItemNew(dataInputStream6.r, 101, isSave: false);
						}
						catch (Exception)
						{
						}
					}
					if (!GameScr.isLoadAllData)
					{
						GameScr.gI().readOk();
					}
					else
					{
						Service.gI().clientOk();
					}
					sbyte b4 = msg.reader().readByte();
					Res.outz("CAPTION LENT= " + b4);
					GameScr.exps = new long[b4];
					for (int j = 0; j < GameScr.exps.Length; j++)
					{
						GameScr.exps[j] = msg.reader().readLong();
					}
					break;
				}
				case 6:
				{
					Res.outz("GET UPDATE_MAP " + msg.reader().available() + " bytes");
					msg.reader().mark(500000);
					createMap(msg.reader());
					msg.reader().reset();
					sbyte[] data3 = new sbyte[msg.reader().available()];
					msg.reader().readFully(ref data3);
					Rms.saveRMS("NRmap", data3);
					sbyte[] data4 = new sbyte[1] { GameScr.vcMap };
					Rms.saveRMS("NRmapVersion", data4);
					LoginScr.isUpdateMap = false;
					GameScr.gI().readOk();
					break;
				}
				case 7:
				{
					Res.outz("GET UPDATE_SKILL " + msg.reader().available() + " bytes");
					msg.reader().mark(500000);
					createSkill(msg.reader());
					msg.reader().reset();
					sbyte[] data = new sbyte[msg.reader().available()];
					msg.reader().readFully(ref data);
					Rms.saveRMS("NRskill", data);
					sbyte[] data2 = new sbyte[1] { GameScr.vcSkill };
					Rms.saveRMS("NRskillVersion", data2);
					LoginScr.isUpdateSkill = false;
					GameScr.gI().readOk();
					break;
				}
				case 8:
					Res.outz("GET UPDATE_ITEM " + msg.reader().available() + " bytes");
					createItemNew(msg.reader());
					break;
				case 10:
					try
					{
						Char.isLoadingMap = true;
						Res.outz("REQUEST MAP TEMPLATE");
						GameCanvas.isLoading = true;
						TileMap.maps = null;
						TileMap.types = null;
						mSystem.gcc();
						GameCanvas.debug("SA99", 2);
						TileMap.tmw = msg.reader().readByte();
						TileMap.tmh = msg.reader().readByte();
						TileMap.maps = new int[TileMap.tmw * TileMap.tmh];
						Res.err("   M apsize= " + TileMap.tmw * TileMap.tmh);
						for (int i = 0; i < TileMap.maps.Length; i++)
						{
							int num = msg.reader().readByte();
							if (num < 0)
							{
								num += 256;
							}
							TileMap.maps[i] = (ushort)num;
						}
						TileMap.types = new int[TileMap.maps.Length];
						msg = messWait;
						loadInfoMap(msg);
						try
						{
							sbyte b2 = msg.reader().readByte();
							TileMap.isMapDouble = ((b2 != 0) ? true : false);
						}
						catch (Exception ex)
						{
							Res.err(" 1 LOI TAI CASE REQUEST_MAPTEMPLATE " + ex.ToString());
						}
					}
					catch (Exception ex2)
					{
						Res.err("2 LOI TAI CASE REQUEST_MAPTEMPLATE " + ex2.ToString());
					}
					msg.cleanup();
					messWait.cleanup();
					msg = (messWait = null);
					GameScr.gI().switchToMe();
					break;
				case 9:
					GameCanvas.debug("SA11", 2);
					break;
				}
			}
			catch (Exception ex8)
			{
				Cout.LogError("LOI TAI messageNotMap=== " + msg.command + "  >>" + ex8.StackTrace);
			}
			finally
			{
				msg?.cleanup();
			}
		}

}
