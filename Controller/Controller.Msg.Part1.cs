using System;
using Assets.src.e;
using Assets.src.f;
using Assets.src.g;
using UnityEngine;

public partial class Controller : IMessageHandler
{
	public bool onMessage_Part1(Message msg)
	{
		Char @char = null;
		Mob mob = null;
		MyVector myVector = new MyVector();
		int num = 0;
		switch (msg.command)
		{
				case 12:
					read_cmdExtraBig(msg);
					LoginScr.isUpdateItem = false;
					GameScr.gI().readOk();
					GameCanvas.endDlg();
					break;
				case 0:
					readLogin(msg);
					break;
				case 24:
					read_cmdExtra(msg);
					break;
				case 20:
					phuban_Info(msg);
					break;
				case 66:
					readGetImgByName(msg);
					break;
				case 65:
				{
					sbyte b6 = msg.reader().readSByte();
					string text = msg.reader().readUTF();
					short num9 = msg.reader().readShort();
					if (ItemTime.isExistMessage(b6))
					{
						if (num9 != 0)
						{
							ItemTime.getMessageById(b6).initTimeText(b6, text, num9);
						}
						else
						{
							GameScr.textTime.removeElement(ItemTime.getMessageById(b6));
						}
					}
					else
					{
						ItemTime itemTime = new ItemTime();
						itemTime.initTimeText(b6, text, num9);
						GameScr.textTime.addElement(itemTime);
					}
					break;
				}
				case 112:
				{
					sbyte b43 = msg.reader().readByte();
					Res.outz("spec type= " + b43);
					if (b43 == 0)
					{
						Panel.spearcialImage = msg.reader().readShort();
						Panel.specialInfo = msg.reader().readUTF();
					}
					else
					{
						if (b43 != 1)
						{
							break;
						}
						sbyte b44 = msg.reader().readByte();
						Char.myCharz().infoSpeacialSkill = new string[b44][];
						Char.myCharz().imgSpeacialSkill = new short[b44][];
						GameCanvas.panel.speacialTabName = new string[b44][];
						for (int num110 = 0; num110 < b44; num110++)
						{
							GameCanvas.panel.speacialTabName[num110] = new string[2];
							string[] array9 = Res.split(msg.reader().readUTF(), "\n", 0);
							if (array9.Length == 2)
							{
								GameCanvas.panel.speacialTabName[num110] = array9;
							}
							if (array9.Length == 1)
							{
								GameCanvas.panel.speacialTabName[num110][0] = array9[0];
								GameCanvas.panel.speacialTabName[num110][1] = string.Empty;
							}
							int num111 = msg.reader().readByte();
							Char.myCharz().infoSpeacialSkill[num110] = new string[num111];
							Char.myCharz().imgSpeacialSkill[num110] = new short[num111];
							for (int num112 = 0; num112 < num111; num112++)
							{
								Char.myCharz().imgSpeacialSkill[num110][num112] = msg.reader().readShort();
								Char.myCharz().infoSpeacialSkill[num110][num112] = msg.reader().readUTF();
							}
						}
						GameCanvas.panel.tabName[25] = GameCanvas.panel.speacialTabName;
						GameCanvas.panel.setTypeSpeacialSkill();
						GameCanvas.panel.show();
					}
					break;
				}
				case -98:
				{
					sbyte b41 = msg.reader().readByte();
					GameCanvas.menu.showMenu = false;
					if (b41 == 0)
					{
						GameCanvas.startYesNoDlg(msg.reader().readUTF(), new Command(mResources.YES, GameCanvas.instance, 888397, msg.reader().readUTF()), new Command(mResources.NO, GameCanvas.instance, 888396, null));
					}
					break;
				}
				case -97:
					Char.myCharz().cNangdong = msg.reader().readInt();
					break;
				case -96:
				{
					sbyte typeTop = msg.reader().readByte();
					GameCanvas.panel.vTop.removeAllElements();
					string topName = msg.reader().readUTF();
					sbyte b56 = msg.reader().readByte();
					for (int num134 = 0; num134 < b56; num134++)
					{
						int rank = msg.reader().readInt();
						int pId = msg.reader().readInt();
						short headID = msg.reader().readShort();
						short headICON = msg.reader().readShort();
						short body = msg.reader().readShort();
						short leg = msg.reader().readShort();
						string name = msg.reader().readUTF();
						string info3 = msg.reader().readUTF();
						TopInfo topInfo = new TopInfo();
						topInfo.rank = rank;
						topInfo.headID = headID;
						topInfo.headICON = headICON;
						topInfo.body = body;
						topInfo.leg = leg;
						topInfo.name = name;
						topInfo.info = info3;
						topInfo.info2 = msg.reader().readUTF();
						topInfo.pId = pId;
						GameCanvas.panel.vTop.addElement(topInfo);
					}
					GameCanvas.panel.topName = topName;
					GameCanvas.panel.setTypeTop(typeTop);
					GameCanvas.panel.show();
					break;
				}
				case -94:
					while (msg.reader().available() > 0)
					{
						short num17 = msg.reader().readShort();
						int num18 = msg.reader().readInt();
						for (int m = 0; m < Char.myCharz().vSkill.size(); m++)
						{
							Skill skill = (Skill)Char.myCharz().vSkill.elementAt(m);
							if (skill != null && skill.skillId == num17)
							{
								if (num18 < skill.coolDown)
								{
									skill.lastTimeUseThisSkill = mSystem.currentTimeMillis() - (skill.coolDown - num18);
								}
								Res.outz("1 chieu id= " + skill.template.id + " cooldown= " + num18 + "curr cool down= " + skill.coolDown);
							}
						}
					}
					break;
				case -95:
				{
					sbyte b16 = msg.reader().readByte();
					Res.outz("MOB_ME_UPDATE type= " + b16);
					if (b16 == 0)
					{
						int num27 = msg.reader().readInt();
						short templateId = msg.reader().readShort();
						long num28 = msg.reader().readLong();
						SoundMn.gI().explode_1();
						if (num27 == Char.myCharz().charID)
						{
							Char.myCharz().mobMe = new Mob(num27, isDisable: false, isDontMove: false, isFire: false, isIce: false, isWind: false, templateId, 1, num28, 0, num28, (short)(Char.myCharz().cx + ((Char.myCharz().cdir != 1) ? (-40) : 40)), (short)Char.myCharz().cy, 4, 0);
							Char.myCharz().mobMe.isMobMe = true;
							EffecMn.addEff(new Effect(18, Char.myCharz().mobMe.x, Char.myCharz().mobMe.y, 2, 10, -1));
							Char.myCharz().tMobMeBorn = 30;
							GameScr.vMob.addElement(Char.myCharz().mobMe);
						}
						else
						{
							@char = GameScr.findCharInMap(num27);
							if (@char != null)
							{
								Mob mob3 = new Mob(num27, isDisable: false, isDontMove: false, isFire: false, isIce: false, isWind: false, templateId, 1, num28, 0, num28, (short)@char.cx, (short)@char.cy, 4, 0);
								mob3.isMobMe = true;
								@char.mobMe = mob3;
								GameScr.vMob.addElement(@char.mobMe);
							}
							else
							{
								Mob mob4 = GameScr.findMobInMap(num27);
								if (mob4 == null)
								{
									mob4 = new Mob(num27, isDisable: false, isDontMove: false, isFire: false, isIce: false, isWind: false, templateId, 1, num28, 0, num28, -100, -100, 4, 0);
									mob4.isMobMe = true;
									GameScr.vMob.addElement(mob4);
								}
							}
						}
					}
					if (b16 == 1)
					{
						int num29 = msg.reader().readInt();
						int mobId = msg.reader().readByte();
						Res.outz("mod attack id= " + num29);
						if (num29 == Char.myCharz().charID)
						{
							if (GameScr.findMobInMap(mobId) != null)
							{
								Char.myCharz().mobMe.attackOtherMob(GameScr.findMobInMap(mobId));
							}
						}
						else
						{
							@char = GameScr.findCharInMap(num29);
							if (@char != null && GameScr.findMobInMap(mobId) != null)
							{
								@char.mobMe.attackOtherMob(GameScr.findMobInMap(mobId));
							}
						}
					}
					if (b16 == 2)
					{
						int num30 = msg.reader().readInt();
						int num31 = msg.reader().readInt();
						long num32 = msg.reader().readLong();
						long cHPNew = msg.reader().readLong();
						if (num30 == Char.myCharz().charID)
						{
							Res.outz("mob dame= " + num32);
							@char = GameScr.findCharInMap(num31);
							if (@char != null)
							{
								@char.cHPNew = cHPNew;
								if (Char.myCharz().mobMe.isBusyAttackSomeOne)
								{
									@char.doInjure(num32, 0L, isCrit: false, isMob: true);
								}
								else
								{
									Char.myCharz().mobMe.dame = num32;
									Char.myCharz().mobMe.setAttack(@char);
								}
							}
						}
						else
						{
							mob = GameScr.findMobInMap(num30);
							if (mob != null)
							{
								if (num31 == Char.myCharz().charID)
								{
									Char.myCharz().cHPNew = cHPNew;
									if (mob.isBusyAttackSomeOne)
									{
										Char.myCharz().doInjure(num32, 0L, isCrit: false, isMob: true);
									}
									else
									{
										mob.dame = num32;
										mob.setAttack(Char.myCharz());
									}
								}
								else
								{
									@char = GameScr.findCharInMap(num31);
									if (@char != null)
									{
										@char.cHPNew = cHPNew;
										if (mob.isBusyAttackSomeOne)
										{
											@char.doInjure(num32, 0L, isCrit: false, isMob: true);
										}
										else
										{
											mob.dame = num32;
											mob.setAttack(@char);
										}
									}
								}
							}
						}
					}
					if (b16 == 3)
					{
						int num33 = msg.reader().readInt();
						int mobId2 = msg.reader().readInt();
						long hp = msg.reader().readLong();
						long num34 = msg.reader().readLong();
						@char = null;
						@char = ((Char.myCharz().charID != num33) ? GameScr.findCharInMap(num33) : Char.myCharz());
						if (@char != null)
						{
							mob = GameScr.findMobInMap(mobId2);
							if (@char.mobMe != null)
							{
								@char.mobMe.attackOtherMob(mob);
							}
							if (mob != null)
							{
								mob.hp = hp;
								mob.updateHp_bar();
								if (num34 == 0)
								{
									mob.x = mob.xFirst;
									mob.y = mob.yFirst;
									GameScr.startFlyText(mResources.miss, mob.x, mob.y - mob.h, 0, -2, mFont.MISS);
								}
								else
								{
									GameScr.startFlyText("-" + num34, mob.x, mob.y - mob.h, 0, -2, mFont.ORANGE);
								}
							}
						}
					}
					if (b16 == 4)
					{
					}
					if (b16 == 5)
					{
						int num35 = msg.reader().readInt();
						sbyte b17 = msg.reader().readByte();
						int num36 = msg.reader().readInt();
						long num37 = msg.reader().readLong();
						long hp2 = msg.reader().readLong();
						Res.outz("MOB_ME_UPDATE type= 5   playerAttack:" + num35 + "  skillID:" + b17 + "  mobAttacked:" + num36);
						@char = null;
						@char = ((num35 != Char.myCharz().charID) ? GameScr.findCharInMap(num35) : Char.myCharz());
						if (@char == null)
						{
							Res.outz("MOB_ME_UPDATE char = null == null");
							return true;
						}
						Res.outz(@char.cName + "   MOB_ME_UPDATE Attack Mob With Skill ID===" + b17);
						if ((TileMap.tileTypeAtPixel(@char.cx, @char.cy) & 2) == 2)
						{
							@char.setSkillPaint(GameScr.sks[b17], 0);
						}
						else
						{
							@char.setSkillPaint(GameScr.sks[b17], 1);
						}
						Mob mob5 = GameScr.findMobInMap(num36);
						if (mob5 == null)
						{
							Res.err(@char.cName + "   MOB_ME_UPDATE mob  nullllllllll");
						}
						if (@char.cx <= mob5.x)
						{
							@char.cdir = 1;
						}
						else
						{
							@char.cdir = -1;
						}
						@char.mobFocus = mob5;
						mob5.hp = hp2;
						mob5.updateHp_bar();
						GameCanvas.debug("SA83v2", 2);
						if (num37 == 0)
						{
							mob5.x = mob5.xFirst;
							mob5.y = mob5.yFirst;
							GameScr.startFlyText(mResources.miss, mob5.x, mob5.y - mob5.h, 0, -2, mFont.MISS);
						}
						else
						{
							GameScr.startFlyText("-" + num37, mob5.x, mob5.y - mob5.h, 0, -2, mFont.ORANGE);
						}
					}
					if (b16 == 6)
					{
						int num38 = msg.reader().readInt();
						if (num38 == Char.myCharz().charID)
						{
							Char.myCharz().mobMe.startDie();
						}
						else
						{
							GameScr.findCharInMap(num38)?.mobMe.startDie();
						}
					}
					if (b16 != 7)
					{
						break;
					}
					int num39 = msg.reader().readInt();
					if (num39 == Char.myCharz().charID)
					{
						Char.myCharz().mobMe = null;
						for (int num40 = 0; num40 < GameScr.vMob.size(); num40++)
						{
							if (((Mob)GameScr.vMob.elementAt(num40)).mobId == num39)
							{
								GameScr.vMob.removeElementAt(num40);
							}
						}
						break;
					}
					@char = GameScr.findCharInMap(num39);
					for (int num41 = 0; num41 < GameScr.vMob.size(); num41++)
					{
						if (((Mob)GameScr.vMob.elementAt(num41)).mobId == num39)
						{
							GameScr.vMob.removeElementAt(num41);
						}
					}
					if (@char != null)
					{
						@char.mobMe = null;
					}
					break;
				}
				case -92:
					mSystem.clientType = msg.reader().readByte();
					if (Rms.loadRMSString(Rms.RMS_ResVersion) != null)
					{
						Rms.clearAll();
					}
					Rms.saveRMSInt(Rms.RMS_clienttype, mSystem.clientType);
					Rms.saveRMSInt(Rms.RMS_lastZoomlevel, mGraphics.zoomLevel);
					if (Rms.loadRMSString(Rms.RMS_ResVersion) == null)
					{
						GameCanvas.startOK(mResources.plsRestartGame, 8885, null);
					}
					break;
				case -91:
				{
					sbyte b37 = msg.reader().readByte();
					GameCanvas.panel.mapNames = new string[b37];
					GameCanvas.panel.planetNames = new string[b37];
					for (int num91 = 0; num91 < b37; num91++)
					{
						GameCanvas.panel.mapNames[num91] = msg.reader().readUTF();
						GameCanvas.panel.planetNames[num91] = msg.reader().readUTF();
					}
					GameCanvas.panel.setTypeMapTrans();
					GameCanvas.panel.show();
					break;
				}
				case -90:
				{
					sbyte b47 = msg.reader().readByte();
					int num116 = msg.reader().readInt();
					Res.outz("===> UPDATE_BODY:    type = " + b47);
					@char = ((Char.myCharz().charID != num116) ? GameScr.findCharInMap(num116) : Char.myCharz());
					if (b47 != -1)
					{
						short num117 = msg.reader().readShort();
						short num118 = msg.reader().readShort();
						short num119 = msg.reader().readShort();
						sbyte isMonkey = msg.reader().readByte();
						if (@char != null)
						{
							if (@char.charID == num116)
							{
								@char.isMask = true;
								@char.isMonkey = isMonkey;
								if (@char.isMonkey != 0)
								{
									@char.isWaitMonkey = false;
									@char.isLockMove = false;
								}
							}
							else if (@char != null)
							{
								@char.isMask = true;
								@char.isMonkey = isMonkey;
							}
							if (num117 != -1)
							{
								@char.head = num117;
							}
							if (num118 != -1)
							{
								@char.body = num118;
							}
							if (num119 != -1)
							{
								@char.leg = num119;
							}
						}
					}
					if (b47 == -1 && @char != null)
					{
						@char.isMask = false;
						@char.isMonkey = 0;
					}
					if (@char == null)
					{
						break;
					}
					Effect.GetCharEff(@char);
					if (@char.bag == 30 && @char.me)
					{
						GameScr.isPickNgocRong = true;
					}
					if (!@char.me)
					{
						break;
					}
					GameScr.isudungCapsun4 = false;
					GameScr.isudungCapsun3 = false;
					for (int num120 = 0; num120 < Char.myCharz().arrItemBag.Length; num120++)
					{
						Item item3 = Char.myCharz().arrItemBag[num120];
						if (item3 == null)
						{
							continue;
						}
						if (item3.template.id == 194)
						{
							GameScr.isudungCapsun4 = item3.quantity > 0;
							if (GameScr.isudungCapsun4)
							{
								break;
							}
						}
						else if (item3.template.id == 193)
						{
							GameScr.isudungCapsun3 = item3.quantity > 0;
						}
					}
					break;
				}
				case -88:
					GameCanvas.endDlg();
					GameCanvas.serverScreen.switchToMe();
					break;
				case -87:
				{
					Res.outz("GET UPDATE_DATA " + msg.reader().available() + " bytes");
					msg.reader().mark(500000);
					createData(msg.reader(), isSaveRMS: true);
					msg.reader().reset();
					sbyte[] data4 = new sbyte[msg.reader().available()];
					msg.reader().readFully(ref data4);
					sbyte[] data5 = new sbyte[1] { GameScr.vcData };
					Rms.saveRMS("NRdataVersion", data5);
					LoginScr.isUpdateData = false;
					GameScr.gI().readOk();
					break;
				}
				case -86:
				{
					sbyte b34 = msg.reader().readByte();
					Res.outz("server gui ve giao dich action = " + b34);
					if (b34 == 0)
					{
						int playerID = msg.reader().readInt();
						GameScr.gI().giaodich(playerID);
					}
					if (b34 == 1)
					{
						int num86 = msg.reader().readInt();
						Char char6 = GameScr.findCharInMap(num86);
						if (char6 == null)
						{
							return true;
						}
						GameCanvas.panel.setTypeGiaoDich(char6);
						GameCanvas.panel.show();
						Service.gI().getPlayerMenu(num86);
					}
					if (b34 == 2)
					{
						sbyte b35 = msg.reader().readByte();
						for (int num87 = 0; num87 < GameCanvas.panel.vMyGD.size(); num87++)
						{
							Item item = (Item)GameCanvas.panel.vMyGD.elementAt(num87);
							if (item.indexUI == b35)
							{
								GameCanvas.panel.vMyGD.removeElement(item);
								break;
							}
						}
					}
					if (b34 == 5)
					{
					}
					if (b34 == 6)
					{
						GameCanvas.panel.isFriendLock = true;
						if (GameCanvas.panel2 != null)
						{
							GameCanvas.panel2.isFriendLock = true;
						}
						GameCanvas.panel.vFriendGD.removeAllElements();
						if (GameCanvas.panel2 != null)
						{
							GameCanvas.panel2.vFriendGD.removeAllElements();
						}
						int friendMoneyGD = msg.reader().readInt();
						sbyte b36 = msg.reader().readByte();
						Res.outz("item size = " + b36);
						for (int num88 = 0; num88 < b36; num88++)
						{
							Item item2 = new Item();
							item2.template = ItemTemplates.get(msg.reader().readShort());
							item2.quantity = msg.reader().readInt();
							int num89 = msg.reader().readUnsignedByte();
							if (num89 != 0)
							{
								item2.itemOption = new ItemOption[num89];
								for (int num90 = 0; num90 < item2.itemOption.Length; num90++)
								{
									ItemOption itemOption5 = readItemOption(msg);
									if (itemOption5 != null)
									{
										item2.itemOption[num90] = itemOption5;
										item2.compare = GameCanvas.panel.getCompare(item2);
									}
								}
							}
							if (GameCanvas.panel2 != null)
							{
								GameCanvas.panel2.vFriendGD.addElement(item2);
							}
							else
							{
								GameCanvas.panel.vFriendGD.addElement(item2);
							}
						}
						if (GameCanvas.panel2 != null)
						{
							GameCanvas.panel2.setTabGiaoDich(isMe: false);
							GameCanvas.panel2.friendMoneyGD = friendMoneyGD;
						}
						else
						{
							GameCanvas.panel.friendMoneyGD = friendMoneyGD;
							if (GameCanvas.panel.currentTabIndex == 2)
							{
								GameCanvas.panel.setTabGiaoDich(isMe: false);
							}
						}
					}
					if (b34 == 7)
					{
						InfoDlg.hide();
						if (GameCanvas.panel.isShow)
						{
							GameCanvas.panel.hide();
						}
					}
					break;
				}
				case -85:
				{
					Res.outz("CAP CHAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
					sbyte b26 = msg.reader().readByte();
					if (b26 == 0)
					{
						int num63 = msg.reader().readUnsignedShort();
						Res.outz("lent =" + num63);
						sbyte[] data2 = new sbyte[num63];
						msg.reader().read(ref data2, 0, num63);
						GameScr.imgCapcha = Image.createImage(data2, 0, num63);
						GameScr.gI().keyInput = "-----";
						GameScr.gI().strCapcha = msg.reader().readUTF();
						GameScr.gI().keyCapcha = new int[GameScr.gI().strCapcha.Length];
						GameScr.gI().mobCapcha = new Mob();
						GameScr.gI().right = null;
					}
					if (b26 == 1)
					{
						MobCapcha.isAttack = true;
					}
					if (b26 == 2)
					{
						MobCapcha.explode = true;
						GameScr.gI().right = GameScr.gI().cmdFocus;
					}
					break;
				}
			default:
				return false;
		}
		return true;
	}

}
