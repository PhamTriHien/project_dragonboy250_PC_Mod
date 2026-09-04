using System;
using Assets.src.e;
using Assets.src.f;
using Assets.src.g;
using UnityEngine;

public partial class Controller : IMessageHandler
{
	protected static Controller me;

	protected static Controller me2;

	public Message messWait;

	public static bool isLoadingData = false;

	public static bool isConnectOK;

	public static bool isConnectionFail;

	public static bool isDisconnected;

	public static bool isMain;

	private float demCount;

	private int move;

	private int total;

	public static bool isStopReadMessage;

	public static bool isGet_CLIENT_INFO = false;

	public static MyHashTable frameHT_NEWBOSS = new MyHashTable();

	public const sbyte PHUBAN_TYPE_CHIENTRUONGNAMEK = 0;

	public const sbyte PHUBAN_START = 0;

	public const sbyte PHUBAN_UPDATE_POINT = 1;

	public const sbyte PHUBAN_END = 2;

	public const sbyte PHUBAN_LIFE = 4;

	public const sbyte PHUBAN_INFO = 5;

	public static bool isEXTRA_LINK = false;

	public static Controller gI()
		{
			if (me == null)
			{
				me = new Controller();
			}
			return me;
		}

	public static Controller gI2()
		{
			if (me2 == null)
			{
				me2 = new Controller();
			}
			return me2;
		}

	public void onConnectOK(bool isMain1)
		{
			isMain = isMain1;
			mSystem.onConnectOK();
		}

	public void onConnectionFail(bool isMain1)
		{
			isMain = isMain1;
			mSystem.onConnectionFail();
		}

	public void onDisconnected(bool isMain1)
		{
			isMain = isMain1;
			mSystem.onDisconnected();
		}

	public void onMessage(Message msg)
		{
			GameCanvas.debugSession.removeAllElements();
			GameCanvas.debug("SA1", 2);
			try
			{
				if (msg.command != -74)
				{
					Res.outz("=========> [READ] cmd= " + msg.command);
				}
				Char @char = null;
				Mob mob = null;
				MyVector myVector = new MyVector();
				int num = 0;
				GameCanvas.timeLoading = 15;
				Controller2.readMessage(msg);
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
							return;
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
							return;
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
						return;
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
							return;
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
						return;
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
						return;
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
						return;
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
						return;
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
								return;
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
				case -45:
				{
					sbyte b48 = msg.reader().readByte();
					int num123 = msg.reader().readInt();
					short num124 = msg.reader().readShort();
					Res.outz(">.SKILL_NOT_FOCUS      skillNotFocusID: " + num124 + " skill type= " + b48 + "   player use= " + num123);
					if (b48 == 20)
					{
						sbyte b49 = msg.reader().readByte();
						sbyte dir = msg.reader().readByte();
						short timeGong = msg.reader().readShort();
						bool isFly = ((msg.reader().readByte() != 0) ? true : false);
						sbyte typePaint = msg.reader().readByte();
						sbyte typeItem = -1;
						try
						{
							typeItem = msg.reader().readByte();
						}
						catch (Exception)
						{
						}
						Res.outz(">.SKILL_NOT_FOCUS  skill typeFrame= " + b49);
						@char = ((Char.myCharz().charID != num123) ? GameScr.findCharInMap(num123) : Char.myCharz());
						@char.SetSkillPaint_NEW(num124, isFly, b49, typePaint, dir, timeGong, typeItem);
					}
					if (b48 == 21)
					{
						Point point = new Point();
						point.x = msg.reader().readShort();
						point.y = msg.reader().readShort();
						short timeDame = msg.reader().readShort();
						short rangeDame = msg.reader().readShort();
						sbyte typePaint2 = 0;
						sbyte typeItem2 = -1;
						Point[] array10 = null;
						@char = ((Char.myCharz().charID != num123) ? GameScr.findCharInMap(num123) : Char.myCharz());
						try
						{
							typePaint2 = msg.reader().readByte();
							sbyte b50 = msg.reader().readByte();
							if (b50 > 0)
							{
								array10 = new Point[b50];
								for (int num125 = 0; num125 < array10.Length; num125++)
								{
									array10[num125] = new Point();
									array10[num125].type = msg.reader().readByte();
									if (array10[num125].type == 0)
									{
										array10[num125].id = msg.reader().readByte();
									}
									else
									{
										array10[num125].id = msg.reader().readInt();
									}
								}
							}
						}
						catch (Exception)
						{
						}
						try
						{
							typeItem2 = msg.reader().readByte();
						}
						catch (Exception)
						{
						}
						Res.outz(">.SKILL_NOT_FOCUS  skill targetDame= " + point.x + ":" + point.y + "    c:" + @char.cx + ":" + @char.cy + "   cdir:" + @char.cdir);
						@char.SetSkillPaint_STT(1, num124, point, timeDame, rangeDame, typePaint2, array10, typeItem2);
					}
					if (b48 == 0)
					{
						Res.outz("id use= " + num123);
						if (Char.myCharz().charID != num123)
						{
							@char = GameScr.findCharInMap(num123);
							if ((TileMap.tileTypeAtPixel(@char.cx, @char.cy) & 2) == 2)
							{
								@char.setSkillPaint(GameScr.sks[num124], 0);
							}
							else
							{
								@char.setSkillPaint(GameScr.sks[num124], 1);
								@char.delayFall = 20;
							}
						}
						else
						{
							Char.myCharz().saveLoadPreviousSkill();
							Res.outz("LOAD LAST SKILL");
						}
						sbyte b51 = msg.reader().readByte();
						Res.outz("npc size= " + b51);
						for (int num126 = 0; num126 < b51; num126++)
						{
							sbyte b52 = msg.reader().readByte();
							sbyte b53 = msg.reader().readByte();
							Res.outz("index= " + b52);
							if (num124 >= 42 && num124 <= 48)
							{
								((Mob)GameScr.vMob.elementAt(b52)).isFreez = true;
								((Mob)GameScr.vMob.elementAt(b52)).seconds = b53;
								((Mob)GameScr.vMob.elementAt(b52)).last = (((Mob)GameScr.vMob.elementAt(b52)).cur = mSystem.currentTimeMillis());
							}
						}
						sbyte b54 = msg.reader().readByte();
						for (int num127 = 0; num127 < b54; num127++)
						{
							int num128 = msg.reader().readInt();
							sbyte b55 = msg.reader().readByte();
							Res.outz("player ID= " + num128 + " my ID= " + Char.myCharz().charID);
							if (num124 < 42 || num124 > 48)
							{
								continue;
							}
							if (num128 == Char.myCharz().charID)
							{
								if (!Char.myCharz().isFlyAndCharge && !Char.myCharz().isStandAndCharge)
								{
									GameScr.gI().isFreez = true;
									Char.myCharz().isFreez = true;
									Char.myCharz().freezSeconds = b55;
									Char.myCharz().lastFreez = (Char.myCharz().currFreez = mSystem.currentTimeMillis());
									Char.myCharz().isLockMove = true;
								}
							}
							else
							{
								@char = GameScr.findCharInMap(num128);
								if (@char != null && !@char.isFlyAndCharge && !@char.isStandAndCharge)
								{
									@char.isFreez = true;
									@char.seconds = b55;
									@char.freezSeconds = b55;
									@char.lastFreez = (GameScr.findCharInMap(num128).currFreez = mSystem.currentTimeMillis());
								}
							}
						}
					}
					if (b48 == 1 && num123 != Char.myCharz().charID)
					{
						try
						{
							GameScr.findCharInMap(num123).isCharge = true;
						}
						catch (Exception)
						{
						}
					}
					if (b48 == 3)
					{
						if (num123 == Char.myCharz().charID)
						{
							Char.myCharz().isCharge = false;
							SoundMn.gI().taitaoPause();
							Char.myCharz().saveLoadPreviousSkill();
						}
						else
						{
							GameScr.findCharInMap(num123).isCharge = false;
						}
					}
					if (b48 == 4)
					{
						if (num123 == Char.myCharz().charID)
						{
							Char.myCharz().seconds = msg.reader().readShort() - 1000;
							Char.myCharz().last = mSystem.currentTimeMillis();
							Res.outz("second= " + Char.myCharz().seconds + " last= " + Char.myCharz().last);
						}
						else if (GameScr.findCharInMap(num123) != null)
						{
							Char char9 = GameScr.findCharInMap(num123);
							switch (char9.cgender)
							{
							case 0:
								if (TileMap.mapID != 170)
								{
									@char.useChargeSkill(isGround: false);
									break;
								}
								if (num124 >= 77 && num124 <= 83)
								{
									@char.useChargeSkill(isGround: true);
								}
								if (num124 >= 70 && num124 <= 76)
								{
									@char.useChargeSkill(isGround: false);
								}
								break;
							case 1:
							{
								if (TileMap.mapID != 170)
								{
									@char.useChargeSkill(isGround: true);
									break;
								}
								bool isGround2 = true;
								if (num124 >= 70 && num124 <= 76)
								{
									isGround2 = false;
								}
								if (num124 >= 77 && num124 <= 83)
								{
									isGround2 = true;
								}
								@char.useChargeSkill(isGround2);
								break;
							}
							default:
								if (TileMap.mapID == 170)
								{
									bool isGround = true;
									if (num124 >= 70 && num124 <= 76)
									{
										isGround = false;
									}
									if (num124 >= 77 && num124 <= 83)
									{
										isGround = true;
									}
									@char.useChargeSkill(isGround);
								}
								break;
							}
							@char.skillTemplateId = num124;
							if (num124 >= 70 && num124 <= 76)
							{
								@char.isUseSkillAfterCharge = true;
							}
							@char.seconds = msg.reader().readShort();
							@char.last = mSystem.currentTimeMillis();
						}
					}
					if (b48 == 5)
					{
						if (num123 == Char.myCharz().charID)
						{
							Char.myCharz().stopUseChargeSkill();
						}
						else if (GameScr.findCharInMap(num123) != null)
						{
							GameScr.findCharInMap(num123).stopUseChargeSkill();
						}
					}
					if (b48 == 6)
					{
						if (num123 == Char.myCharz().charID)
						{
							Char.myCharz().setAutoSkillPaint(GameScr.sks[num124], 0);
						}
						else if (GameScr.findCharInMap(num123) != null)
						{
							GameScr.findCharInMap(num123).setAutoSkillPaint(GameScr.sks[num124], 0);
							SoundMn.gI().gong();
						}
					}
					if (b48 == 7)
					{
						if (num123 == Char.myCharz().charID)
						{
							Char.myCharz().seconds = msg.reader().readShort();
							Res.outz("second = " + Char.myCharz().seconds);
							Char.myCharz().last = mSystem.currentTimeMillis();
						}
						else if (GameScr.findCharInMap(num123) != null)
						{
							GameScr.findCharInMap(num123).useChargeSkill(isGround: true);
							GameScr.findCharInMap(num123).seconds = msg.reader().readShort();
							GameScr.findCharInMap(num123).last = mSystem.currentTimeMillis();
							SoundMn.gI().gong();
						}
					}
					if (b48 == 8 && num123 != Char.myCharz().charID && GameScr.findCharInMap(num123) != null)
					{
						GameScr.findCharInMap(num123).setAutoSkillPaint(GameScr.sks[num124], 0);
					}
					break;
				}
				case -44:
				{
					bool flag6 = false;
					if (GameCanvas.w > 2 * Panel.WIDTH_PANEL)
					{
						flag6 = true;
					}
					sbyte b30 = msg.reader().readByte();
					int num68 = msg.reader().readUnsignedByte();
					Char.myCharz().arrItemShop = new Item[num68][];
					GameCanvas.panel.shopTabName = new string[num68 + ((!flag6) ? 1 : 0)][];
					for (int num69 = 0; num69 < GameCanvas.panel.shopTabName.Length; num69++)
					{
						GameCanvas.panel.shopTabName[num69] = new string[2];
					}
					if (b30 == 2)
					{
						GameCanvas.panel.maxPageShop = new int[num68];
						GameCanvas.panel.currPageShop = new int[num68];
					}
					if (!flag6)
					{
						GameCanvas.panel.shopTabName[num68] = mResources.inventory;
					}
					for (int num70 = 0; num70 < num68; num70++)
					{
						string[] array5 = Res.split(msg.reader().readUTF(), "\n", 0);
						if (b30 == 2)
						{
							GameCanvas.panel.maxPageShop[num70] = msg.reader().readUnsignedByte();
						}
						if (array5.Length == 2)
						{
							GameCanvas.panel.shopTabName[num70] = array5;
						}
						if (array5.Length == 1)
						{
							GameCanvas.panel.shopTabName[num70][0] = array5[0];
							GameCanvas.panel.shopTabName[num70][1] = string.Empty;
						}
						int num71 = msg.reader().readUnsignedByte();
						Char.myCharz().arrItemShop[num70] = new Item[num71];
						Panel.strWantToBuy = mResources.say_wat_do_u_want_to_buy;
						if (b30 == 1)
						{
							Panel.strWantToBuy = mResources.say_wat_do_u_want_to_buy2;
						}
						for (int num72 = 0; num72 < num71; num72++)
						{
							short num73 = msg.reader().readShort();
							if (num73 == -1)
							{
								continue;
							}
							Char.myCharz().arrItemShop[num70][num72] = new Item();
							Char.myCharz().arrItemShop[num70][num72].template = ItemTemplates.get(num73);
							if (b30 == 8)
							{
								Char.myCharz().arrItemShop[num70][num72].buyCoin = msg.reader().readInt();
								Char.myCharz().arrItemShop[num70][num72].buyGold = msg.reader().readInt();
								Char.myCharz().arrItemShop[num70][num72].quantity = msg.reader().readInt();
							}
							else if (b30 == 4)
							{
								Char.myCharz().arrItemShop[num70][num72].reason = msg.reader().readUTF();
							}
							else if (b30 == 0)
							{
								Char.myCharz().arrItemShop[num70][num72].buyCoin = msg.reader().readInt();
								Char.myCharz().arrItemShop[num70][num72].buyGold = msg.reader().readInt();
							}
							else if (b30 == 1)
							{
								Char.myCharz().arrItemShop[num70][num72].powerRequire = msg.reader().readLong();
							}
							else if (b30 == 2)
							{
								Char.myCharz().arrItemShop[num70][num72].itemId = msg.reader().readShort();
								Char.myCharz().arrItemShop[num70][num72].buyCoin = msg.reader().readInt();
								Char.myCharz().arrItemShop[num70][num72].buyGold = msg.reader().readInt();
								Char.myCharz().arrItemShop[num70][num72].buyType = msg.reader().readByte();
								Char.myCharz().arrItemShop[num70][num72].quantity = msg.reader().readInt();
								Char.myCharz().arrItemShop[num70][num72].isMe = msg.reader().readByte();
							}
							else if (b30 == 3)
							{
								Char.myCharz().arrItemShop[num70][num72].isBuySpec = true;
								Char.myCharz().arrItemShop[num70][num72].iconSpec = msg.reader().readShort();
								Char.myCharz().arrItemShop[num70][num72].buySpec = msg.reader().readInt();
							}
							int num74 = msg.reader().readUnsignedByte();
							if (num74 != 0)
							{
								Char.myCharz().arrItemShop[num70][num72].itemOption = new ItemOption[num74];
								for (int num75 = 0; num75 < Char.myCharz().arrItemShop[num70][num72].itemOption.Length; num75++)
								{
									ItemOption itemOption3 = readItemOption(msg);
									if (itemOption3 != null)
									{
										Char.myCharz().arrItemShop[num70][num72].itemOption[num75] = itemOption3;
										Char.myCharz().arrItemShop[num70][num72].compare = GameCanvas.panel.getCompare(Char.myCharz().arrItemShop[num70][num72]);
									}
								}
							}
							sbyte b31 = msg.reader().readByte();
							Char.myCharz().arrItemShop[num70][num72].newItem = ((b31 != 0) ? true : false);
							sbyte b32 = msg.reader().readByte();
							if (b32 == 1)
							{
								int headTemp = msg.reader().readShort();
								int bodyTemp = msg.reader().readShort();
								int legTemp = msg.reader().readShort();
								int bagTemp = msg.reader().readShort();
								Char.myCharz().arrItemShop[num70][num72].setPartTemp(headTemp, bodyTemp, legTemp, bagTemp);
							}
							if (b30 == 2 && GameMidlet.intVERSION >= 237)
							{
								Char.myCharz().arrItemShop[num70][num72].nameNguoiKyGui = msg.reader().readUTF();
								Res.err("nguoi ki gui  " + Char.myCharz().arrItemShop[num70][num72].nameNguoiKyGui);
							}
						}
					}
					if (flag6)
					{
						if (b30 != 2)
						{
							GameCanvas.panel2 = new Panel();
							GameCanvas.panel2.tabName[7] = new string[1][] { new string[1] { string.Empty } };
							GameCanvas.panel2.setTypeBodyOnly();
							GameCanvas.panel2.show();
						}
						else
						{
							GameCanvas.panel2 = new Panel();
							GameCanvas.panel2.setTypeKiGuiOnly();
							GameCanvas.panel2.show();
						}
					}
					GameCanvas.panel.tabName[1] = GameCanvas.panel.shopTabName;
					if (b30 == 2)
					{
						string[][] array6 = GameCanvas.panel.tabName[1];
						if (flag6)
						{
							GameCanvas.panel.tabName[1] = new string[4][]
							{
								array6[0],
								array6[1],
								array6[2],
								array6[3]
							};
						}
						else
						{
							GameCanvas.panel.tabName[1] = new string[5][]
							{
								array6[0],
								array6[1],
								array6[2],
								array6[3],
								array6[4]
							};
						}
					}
					GameCanvas.panel.setTypeShop(b30);
					GameCanvas.panel.show();
					break;
				}
				case -41:
				{
					sbyte b24 = msg.reader().readByte();
					Char.myCharz().strLevel = new string[b24];
					for (int num53 = 0; num53 < b24; num53++)
					{
						string text4 = msg.reader().readUTF();
						Char.myCharz().strLevel[num53] = text4;
					}
					Res.outz("---   xong  level caption cmd : " + msg.command);
					break;
				}
				case -34:
				{
					sbyte b18 = msg.reader().readByte();
					Res.outz("act= " + b18);
					if (b18 == 0 && GameScr.gI().magicTree != null)
					{
						Res.outz("toi duoc day");
						MagicTree magicTree = GameScr.gI().magicTree;
						magicTree.id = msg.reader().readShort();
						magicTree.name = msg.reader().readUTF();
						magicTree.name = Res.changeString(magicTree.name);
						magicTree.x = msg.reader().readShort();
						magicTree.y = msg.reader().readShort();
						magicTree.level = msg.reader().readByte();
						magicTree.currPeas = msg.reader().readShort();
						magicTree.maxPeas = msg.reader().readShort();
						Res.outz("curr Peas= " + magicTree.currPeas);
						magicTree.strInfo = msg.reader().readUTF();
						magicTree.seconds = msg.reader().readInt();
						magicTree.timeToRecieve = magicTree.seconds;
						sbyte b19 = msg.reader().readByte();
						magicTree.peaPostionX = new int[b19];
						magicTree.peaPostionY = new int[b19];
						for (int num43 = 0; num43 < b19; num43++)
						{
							magicTree.peaPostionX[num43] = msg.reader().readByte();
							magicTree.peaPostionY[num43] = msg.reader().readByte();
						}
						magicTree.isUpdate = msg.reader().readBool();
						magicTree.last = (magicTree.cur = mSystem.currentTimeMillis());
						GameScr.gI().magicTree.isUpdateTree = true;
					}
					if (b18 == 1)
					{
						myVector = new MyVector();
						try
						{
							while (msg.reader().available() > 0)
							{
								string caption = msg.reader().readUTF();
								myVector.addElement(new Command(caption, GameCanvas.instance, 888392, null));
							}
						}
						catch (Exception ex6)
						{
							Cout.println("Loi MAGIC_TREE " + ex6.ToString());
						}
						GameCanvas.menu.startAt(myVector, 3);
					}
					if (b18 == 2)
					{
						GameScr.gI().magicTree.remainPeas = msg.reader().readShort();
						GameScr.gI().magicTree.seconds = msg.reader().readInt();
						GameScr.gI().magicTree.last = (GameScr.gI().magicTree.cur = mSystem.currentTimeMillis());
						GameScr.gI().magicTree.isUpdateTree = true;
						GameScr.gI().magicTree.isPeasEffect = true;
					}
					break;
				}
				case 11:
				{
					GameCanvas.debug("SA9", 2);
					int num14 = msg.reader().readShort();
					sbyte b10 = msg.reader().readByte();
					if (b10 != 0)
					{
						Mob.arrMobTemplate[num14].data.readDataNewBoss(NinjaUtil.readByteArray(msg), b10);
					}
					else
					{
						Mob.arrMobTemplate[num14].data.readData(NinjaUtil.readByteArray(msg));
					}
					for (int l = 0; l < GameScr.vMob.size(); l++)
					{
						mob = (Mob)GameScr.vMob.elementAt(l);
						if (mob.templateId == num14)
						{
							mob.w = Mob.arrMobTemplate[num14].data.width;
							mob.h = Mob.arrMobTemplate[num14].data.height;
						}
					}
					sbyte[] array2 = NinjaUtil.readByteArray(msg);
					Image img = Image.createImage(array2, 0, array2.Length);
					Mob.arrMobTemplate[num14].data.img = img;
					int num15 = msg.reader().readByte();
					Mob.arrMobTemplate[num14].data.typeData = num15;
					if (num15 == 1 || num15 == 2)
					{
						readFrameBoss(msg, num14);
					}
					break;
				}
				case -69:
					Char.myCharz().cMaxStamina = msg.reader().readShort();
					break;
				case -68:
					Char.myCharz().cStamina = msg.reader().readShort();
					break;
				case -67:
				{
					demCount += 1f;
					int num154 = msg.reader().readInt();
					Res.outz("RECIEVE  hinh small: " + num154);
					sbyte[] array17 = null;
					try
					{
						array17 = NinjaUtil.readByteArray(msg);
						Res.outz(">SIZE CHECK= " + array17.Length);
						if (num154 == 3896)
						{
						}
						SmallImage.imgNew[num154].img = createImage(array17);
					}
					catch (Exception)
					{
						array17 = null;
						SmallImage.imgNew[num154].img = Image.createRGBImage(new int[1], 1, 1, bl: true);
					}
					if (array17 != null && mGraphics.zoomLevel > 1)
					{
						Rms.saveRMS(mGraphics.zoomLevel + "Small" + num154, array17);
					}
					break;
				}
				case -66:
				{
					short id3 = msg.reader().readShort();
					sbyte[] data3 = NinjaUtil.readByteArray(msg);
					EffectData effDataById = Effect.getEffDataById(id3);
					sbyte b64 = msg.reader().readSByte();
					if (b64 == 0)
					{
						effDataById.readData(data3);
					}
					else
					{
						effDataById.readDataNewBoss(data3, b64);
					}
					sbyte[] array15 = NinjaUtil.readByteArray(msg);
					effDataById.img = Image.createImage(array15, 0, array15.Length);
					break;
				}
				case -32:
				{
					short num135 = msg.reader().readShort();
					int num136 = msg.reader().readInt();
					sbyte[] array11 = null;
					Image image = null;
					try
					{
						array11 = new sbyte[num136];
						for (int num137 = 0; num137 < num136; num137++)
						{
							array11[num137] = msg.reader().readByte();
						}
						image = Image.createImage(array11, 0, num136);
						BgItem.imgNew.put(num135 + string.Empty, image);
					}
					catch (Exception)
					{
						array11 = null;
						BgItem.imgNew.put(num135 + string.Empty, Image.createRGBImage(new int[1], 1, 1, bl: true));
					}
					if (array11 != null)
					{
						if (mGraphics.zoomLevel > 1)
						{
							Rms.saveRMS(mGraphics.zoomLevel + "bgItem" + num135, array11);
						}
						BgItemMn.blendcurrBg(num135, image);
					}
					break;
				}
				case 92:
				{
					if (GameCanvas.currentScreen == GameScr.instance)
					{
						GameCanvas.endDlg();
					}
					string text6 = msg.reader().readUTF();
					string str2 = msg.reader().readUTF();
					str2 = Res.changeString(str2);
					string empty = string.Empty;
					Char char8 = null;
					sbyte b46 = 0;
					if (!text6.Equals(string.Empty))
					{
						char8 = new Char();
						char8.charID = msg.reader().readInt();
						char8.head = msg.reader().readShort();
						char8.headICON = msg.reader().readShort();
						char8.body = msg.reader().readShort();
						char8.bag = msg.reader().readShort();
						char8.leg = msg.reader().readShort();
						b46 = msg.reader().readByte();
						char8.cName = text6;
					}
					empty += str2;
					InfoDlg.hide();
					if (text6.Equals(string.Empty))
					{
						GameScr.info1.addInfo(empty, 0);
						break;
					}
					GameScr.info2.addInfoWithChar(empty, char8, b46 == 0);
					if (GameCanvas.panel.isShow && GameCanvas.panel.type == 8)
					{
						GameCanvas.panel.initLogMessage();
					}
					break;
				}
				case -26:
					ServerListScreen.testConnect = 2;
					GameCanvas.debug("SA2", 2);
					GameCanvas.startOKDlg(msg.reader().readUTF());
					InfoDlg.hide();
					LoginScr.isContinueToLogin = false;
					Char.isLoadingMap = false;
					if (GameCanvas.currentScreen == GameCanvas.loginScr)
					{
						GameCanvas.serverScreen.switchToMe();
					}
					break;
				case -25:
				{
					GameCanvas.debug("SA3", 2);
					string serverMsg = msg.reader().readUTF();
					GameScr.info1.addInfo(serverMsg, 0);
					ModMenu.ProcessServerBossNotice(serverMsg);
					break;
				}
				case 94:
				{
					GameCanvas.debug("SA3", 2);
					string serverAlert = msg.reader().readUTF();
					GameScr.info1.addInfo(serverAlert, 0);
					ModMenu.ProcessServerBossNotice(serverAlert);
					break;
				}
				case 47:
					GameCanvas.debug("SA4", 2);
					GameScr.gI().resetButton();
					break;
				case 81:
				{
					GameCanvas.debug("SXX4", 2);
					Mob mob7 = (Mob)GameScr.vMob.elementAt(msg.reader().readUnsignedByte());
					mob7.isDisable = msg.reader().readBool();
					break;
				}
				case 82:
				{
					GameCanvas.debug("SXX5", 2);
					Mob mob7 = (Mob)GameScr.vMob.elementAt(msg.reader().readUnsignedByte());
					mob7.isDontMove = msg.reader().readBool();
					break;
				}
				case 85:
				{
					GameCanvas.debug("SXX5", 2);
					Mob mob7 = (Mob)GameScr.vMob.elementAt(msg.reader().readUnsignedByte());
					mob7.isFire = msg.reader().readBool();
					break;
				}
				case 86:
				{
					GameCanvas.debug("SXX5", 2);
					Mob mob7 = (Mob)GameScr.vMob.elementAt(msg.reader().readUnsignedByte());
					mob7.isIce = msg.reader().readBool();
					if (!mob7.isIce)
					{
						ServerEffect.addServerEffect(77, mob7.x, mob7.y - 9, 1);
					}
					break;
				}
				case 87:
				{
					GameCanvas.debug("SXX5", 2);
					Mob mob7 = (Mob)GameScr.vMob.elementAt(msg.reader().readUnsignedByte());
					mob7.isWind = msg.reader().readBool();
					break;
				}
				case 56:
				{
					GameCanvas.debug("SXX6", 2);
					@char = null;
					int num21 = msg.reader().readInt();
					if (num21 == Char.myCharz().charID)
					{
						bool flag4 = false;
						@char = Char.myCharz();
						@char.cHP = msg.reader().readLong();
						long num44 = msg.reader().readLong();
						Res.outz("dame hit = " + num44);
						if (num44 != 0)
						{
							@char.doInjure();
						}
						int num45 = 0;
						try
						{
							flag4 = msg.reader().readBoolean();
							sbyte b20 = msg.reader().readByte();
							if (b20 != -1)
							{
								Res.outz("hit eff= " + b20);
								EffecMn.addEff(new Effect(b20, @char.cx, @char.cy, 3, 1, -1));
							}
						}
						catch (Exception)
						{
						}
						num44 += num45;
						if (Char.myCharz().cTypePk != 4)
						{
							if (num44 == 0)
							{
								GameScr.startFlyText(mResources.miss, @char.cx, @char.cy - @char.ch, 0, -3, mFont.MISS_ME);
							}
							else
							{
								GameScr.startFlyText("-" + num44, @char.cx, @char.cy - @char.ch, 0, -3, flag4 ? mFont.FATAL : mFont.RED);
							}
						}
						break;
					}
					@char = GameScr.findCharInMap(num21);
					if (@char == null)
					{
						return;
					}
					@char.cHP = msg.reader().readLong();
					bool flag5 = false;
					long num46 = msg.reader().readLong();
					if (num46 != 0)
					{
						@char.doInjure();
					}
					int num47 = 0;
					try
					{
						flag5 = msg.reader().readBoolean();
						sbyte b21 = msg.reader().readByte();
						if (b21 != -1)
						{
							Res.outz("hit eff= " + b21);
							EffecMn.addEff(new Effect(b21, @char.cx, @char.cy, 3, 1, -1));
						}
					}
					catch (Exception)
					{
					}
					num46 += num47;
					if (@char.cTypePk != 4)
					{
						if (num46 == 0)
						{
							GameScr.startFlyText(mResources.miss, @char.cx, @char.cy - @char.ch, 0, -3, mFont.MISS);
						}
						else
						{
							GameScr.startFlyText("-" + num46, @char.cx, @char.cy - @char.ch, 0, -3, flag5 ? mFont.FATAL : mFont.ORANGE);
						}
					}
					break;
				}
				case 83:
				{
					GameCanvas.debug("SXX8", 2);
					int num21 = msg.reader().readInt();
					@char = ((num21 != Char.myCharz().charID) ? GameScr.findCharInMap(num21) : Char.myCharz());
					if (@char == null)
					{
						return;
					}
					Mob mobToAttack = (Mob)GameScr.vMob.elementAt(msg.reader().readUnsignedByte());
					if (@char.mobMe != null)
					{
						@char.mobMe.attackOtherMob(mobToAttack);
					}
					break;
				}
				case 84:
				{
					int num21 = msg.reader().readInt();
					if (num21 == Char.myCharz().charID)
					{
						@char = Char.myCharz();
					}
					else
					{
						@char = GameScr.findCharInMap(num21);
						if (@char == null)
						{
							return;
						}
					}
					@char.cHP = @char.cHPFull;
					@char.cMP = @char.cMPFull;
					@char.cx = msg.reader().readShort();
					@char.cy = msg.reader().readShort();
					@char.liveFromDead();
					break;
				}
				case 46:
					GameCanvas.debug("SA5", 2);
					Cout.LogWarning("Controler RESET_POINT  " + Char.ischangingMap);
					Char.isLockKey = false;
					Char.myCharz().setResetPoint(msg.reader().readShort(), msg.reader().readShort());
					break;
				case -29:
					messageNotLogin(msg);
					break;
				case -28:
					messageNotMap(msg);
					break;
				case -30:
					messageSubCommand(msg);
					break;
				case 62:
					GameCanvas.debug("SZ3", 2);
					@char = GameScr.findCharInMap(msg.reader().readInt());
					if (@char != null)
					{
						@char.killCharId = Char.myCharz().charID;
						Char.myCharz().npcFocus = null;
						Char.myCharz().mobFocus = null;
						Char.myCharz().itemFocus = null;
						Char.myCharz().charFocus = @char;
						Char.isManualFocus = true;
						GameScr.info1.addInfo(@char.cName + mResources.CUU_SAT, 0);
					}
					break;
				case 63:
					GameCanvas.debug("SZ4", 2);
					Char.myCharz().killCharId = msg.reader().readInt();
					Char.myCharz().npcFocus = null;
					Char.myCharz().mobFocus = null;
					Char.myCharz().itemFocus = null;
					Char.myCharz().charFocus = GameScr.findCharInMap(Char.myCharz().killCharId);
					Char.isManualFocus = true;
					break;
				case 64:
					GameCanvas.debug("SZ5", 2);
					@char = Char.myCharz();
					try
					{
						@char = GameScr.findCharInMap(msg.reader().readInt());
					}
					catch (Exception ex2)
					{
						Cout.println("Loi CLEAR_CUU_SAT " + ex2.ToString());
					}
					@char.killCharId = -9999;
					break;
				case 39:
					GameCanvas.debug("SA49", 2);
					GameScr.gI().typeTradeOrder = 2;
					if (GameScr.gI().typeTrade >= 2 && GameScr.gI().typeTradeOrder >= 2)
					{
						InfoDlg.showWait();
					}
					break;
				case 57:
				{
					GameCanvas.debug("SZ6", 2);
					MyVector myVector2 = new MyVector();
					myVector2.addElement(new Command(msg.reader().readUTF(), GameCanvas.instance, 88817, null));
					GameCanvas.menu.startAt(myVector2, 3);
					break;
				}
				case 58:
				{
					GameCanvas.debug("SZ7", 2);
					int num21 = msg.reader().readInt();
					Char char11 = ((num21 != Char.myCharz().charID) ? GameScr.findCharInMap(num21) : Char.myCharz());
					char11.moveFast = new short[3];
					char11.moveFast[0] = 0;
					short num167 = msg.reader().readShort();
					short num168 = msg.reader().readShort();
					char11.moveFast[1] = num167;
					char11.moveFast[2] = num168;
					try
					{
						num21 = msg.reader().readInt();
						Char char12 = ((num21 != Char.myCharz().charID) ? GameScr.findCharInMap(num21) : Char.myCharz());
						char12.cx = num167;
						char12.cy = num168;
					}
					catch (Exception ex26)
					{
						Cout.println("Loi MOVE_FAST " + ex26.ToString());
					}
					break;
				}
				case 88:
				{
					string info4 = msg.reader().readUTF();
					short num166 = msg.reader().readShort();
					GameCanvas.inputDlg.show(info4, new Command(mResources.ACCEPT, GameCanvas.instance, 88818, num166), TField.INPUT_TYPE_ANY);
					break;
				}
				case 27:
				{
					myVector = new MyVector();
					string text8 = msg.reader().readUTF();
					int num157 = msg.reader().readByte();
					for (int num158 = 0; num158 < num157; num158++)
					{
						string caption4 = msg.reader().readUTF();
						short num159 = msg.reader().readShort();
						myVector.addElement(new Command(caption4, GameCanvas.instance, 88819, num159));
					}
					GameCanvas.menu.startWithoutCloseButton(myVector, 3);
					break;
				}
				case 33:
				{
					GameCanvas.debug("SA51", 2);
					InfoDlg.hide();
					GameCanvas.clearKeyHold();
					GameCanvas.clearKeyPressed();
					myVector = new MyVector();
					try
					{
						while (true)
						{
							string caption3 = msg.reader().readUTF();
							myVector.addElement(new Command(caption3, GameCanvas.instance, 88822, null));
						}
					}
					catch (Exception ex23)
					{
						Cout.println("Loi OPEN_UI_MENU " + ex23.ToString());
					}
					if (Char.myCharz().npcFocus == null)
					{
						return;
					}
					for (int num153 = 0; num153 < Char.myCharz().npcFocus.template.menu.Length; num153++)
					{
						string[] array16 = Char.myCharz().npcFocus.template.menu[num153];
						myVector.addElement(new Command(array16[0], GameCanvas.instance, 88820, array16));
					}
					GameCanvas.menu.startAt(myVector, 3);
					break;
				}
				case 40:
				{
					GameCanvas.debug("SA52", 2);
					GameCanvas.taskTick = 150;
					short taskId = msg.reader().readShort();
					sbyte index2 = msg.reader().readByte();
					string str3 = msg.reader().readUTF();
					str3 = Res.changeString(str3);
					string str4 = msg.reader().readUTF();
					str4 = Res.changeString(str4);
					string[] array12 = new string[msg.reader().readByte()];
					string[] array13 = new string[array12.Length];
					GameScr.tasks = new int[array12.Length];
					GameScr.mapTasks = new int[array12.Length];
					short[] array14 = new short[array12.Length];
					short num141 = -1;
					for (int num142 = 0; num142 < array12.Length; num142++)
					{
						string str5 = msg.reader().readUTF();
						str5 = Res.changeString(str5);
						GameScr.tasks[num142] = msg.reader().readByte();
						GameScr.mapTasks[num142] = msg.reader().readShort();
						string str6 = msg.reader().readUTF();
						str6 = Res.changeString(str6);
						array14[num142] = -1;
						array12[num142] = str5;
						if (!str6.Equals(string.Empty))
						{
							array13[num142] = str6;
						}
					}
					try
					{
						num141 = msg.reader().readShort();
						Cout.println(" TASK_GET count:" + num141);
						for (int num143 = 0; num143 < array12.Length; num143++)
						{
							array14[num143] = msg.reader().readShort();
							Cout.println(num143 + " i TASK_GET   counts[i]:" + array14[num143]);
						}
					}
					catch (Exception ex20)
					{
						Cout.println("Loi TASK_GET " + ex20.ToString());
					}
					Char.myCharz().taskMaint = new Task(taskId, index2, str3, str4, array12, array14, num141, array13);
					if (Char.myCharz().npcFocus != null)
					{
						Npc.clearEffTask();
					}
					Char.taskAction(isNextStep: true);
					break;
				}
				case 41:
					GameCanvas.debug("SA53", 2);
					GameCanvas.taskTick = 100;
					Res.outz("TASK NEXT");
					Char.myCharz().taskMaint.index++;
					Char.myCharz().taskMaint.count = 0;
					Npc.clearEffTask();
					Char.taskAction(isNextStep: true);
					break;
				case 50:
				{
					sbyte b57 = msg.reader().readByte();
					Panel.vGameInfo.removeAllElements();
					for (int num138 = 0; num138 < b57; num138++)
					{
						GameInfo gameInfo = new GameInfo();
						gameInfo.id = msg.reader().readShort();
						gameInfo.main = msg.reader().readUTF();
						gameInfo.content = msg.reader().readUTF();
						Panel.vGameInfo.addElement(gameInfo);
						bool hasRead = Rms.loadRMSInt(gameInfo.id + string.Empty) != -1;
						gameInfo.hasRead = hasRead;
					}
					break;
				}
				case 43:
					GameCanvas.taskTick = 50;
					GameCanvas.debug("SA55", 2);
					Char.myCharz().taskMaint.count = msg.reader().readShort();
					if (Char.myCharz().npcFocus != null)
					{
						Npc.clearEffTask();
					}
					try
					{
						short x_hint = msg.reader().readShort();
						short y_hint = msg.reader().readShort();
						Char.myCharz().x_hint = x_hint;
						Char.myCharz().y_hint = y_hint;
					}
					catch (Exception)
					{
					}
					break;
				case 90:
					GameCanvas.debug("SA577", 2);
					requestItemPlayer(msg);
					break;
				case 29:
					GameCanvas.debug("SA58", 2);
					GameScr.gI().openUIZone(msg);
					break;
				case -21:
				{
					GameCanvas.debug("SA60", 2);
					short itemMapID = msg.reader().readShort();
					for (int num133 = 0; num133 < GameScr.vItemMap.size(); num133++)
					{
						if (((ItemMap)GameScr.vItemMap.elementAt(num133)).itemMapID == itemMapID)
						{
							GameScr.vItemMap.removeElementAt(num133);
							break;
						}
					}
					break;
				}
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
							return;
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
						return;
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
							return;
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
							return;
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
						return;
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
						return;
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
							return;
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
								return;
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
				}
				switch (msg.command)
				{
				case -2:
				{
					GameCanvas.debug("SA77", 22);
					int num190 = msg.reader().readInt();
					Char.myCharz().yen += num190;
					GameScr.startFlyText((num190 <= 0) ? (string.Empty + num190) : ("+" + num190), Char.myCharz().cx, Char.myCharz().cy - Char.myCharz().ch - 10, 0, -2, mFont.YELLOW);
					break;
				}
				case 95:
				{
					GameCanvas.debug("SA77", 22);
					int num178 = msg.reader().readInt();
					Char.myCharz().xu += num178;
					Char.myCharz().xuStr = Res.formatNumber(Char.myCharz().xu);
					GameScr.startFlyText((num178 <= 0) ? (string.Empty + num178) : ("+" + num178), Char.myCharz().cx, Char.myCharz().cy - Char.myCharz().ch - 10, 0, -2, mFont.YELLOW);
					break;
				}
				case 96:
					GameCanvas.debug("SA77a", 22);
					Char.myCharz().taskOrders.addElement(new TaskOrder(msg.reader().readByte(), msg.reader().readShort(), msg.reader().readShort(), msg.reader().readUTF(), msg.reader().readUTF(), msg.reader().readByte(), msg.reader().readByte()));
					break;
				case 97:
				{
					sbyte b75 = msg.reader().readByte();
					for (int num183 = 0; num183 < Char.myCharz().taskOrders.size(); num183++)
					{
						TaskOrder taskOrder = (TaskOrder)Char.myCharz().taskOrders.elementAt(num183);
						if (taskOrder.taskId == b75)
						{
							taskOrder.count = msg.reader().readShort();
							break;
						}
					}
					break;
				}
				case -1:
				{
					GameCanvas.debug("SA77", 222);
					int num189 = msg.reader().readInt();
					Char.myCharz().xu += num189;
					Char.myCharz().xuStr = Res.formatNumber(Char.myCharz().xu);
					Char.myCharz().yen -= num189;
					GameScr.startFlyText("+" + num189, Char.myCharz().cx, Char.myCharz().cy - Char.myCharz().ch - 10, 0, -2, mFont.YELLOW);
					break;
				}
				case -3:
				{
					GameCanvas.debug("SA78", 2);
					sbyte b71 = msg.reader().readByte();
					int num175 = msg.reader().readInt();
					if (b71 == 0)
					{
						Char.myCharz().cPower += num175;
					}
					if (b71 == 1)
					{
						Char.myCharz().cTiemNang += num175;
					}
					if (b71 == 2)
					{
						Char.myCharz().cPower += num175;
						Char.myCharz().cTiemNang += num175;
					}
					Char.myCharz().applyCharLevelPercent();
					if (Char.myCharz().cTypePk != 3)
					{
						GameScr.startFlyText(((num175 <= 0) ? string.Empty : "+") + num175, Char.myCharz().cx, Char.myCharz().cy - Char.myCharz().ch, 0, -4, mFont.GREEN);
						if (num175 > 0 && Char.myCharz().petFollow != null && Char.myCharz().petFollow.smallID == 5002)
						{
							ServerEffect.addServerEffect(55, Char.myCharz().petFollow.cmx, Char.myCharz().petFollow.cmy, 1);
							ServerEffect.addServerEffect(55, Char.myCharz().cx, Char.myCharz().cy, 1);
						}
					}
					break;
				}
				case -73:
				{
					sbyte b77 = msg.reader().readByte();
					for (int num188 = 0; num188 < GameScr.vNpc.size(); num188++)
					{
						Npc npc7 = (Npc)GameScr.vNpc.elementAt(num188);
						if (npc7.template.npcTemplateId == b77)
						{
							sbyte b78 = msg.reader().readByte();
							if (b78 == 0)
							{
								npc7.isHide = true;
							}
							else
							{
								npc7.isHide = false;
							}
							break;
						}
					}
					break;
				}
				case -5:
				{
					GameCanvas.debug("SA79", 2);
					int charID = msg.reader().readInt();
					int num180 = msg.reader().readInt();
					Char char16;
					if (num180 != -100)
					{
						char16 = new Char();
						char16.charID = charID;
						char16.clanID = num180;
					}
					else
					{
						char16 = new Mabu();
						char16.charID = charID;
						char16.clanID = num180;
					}
					if (char16.clanID == -2)
					{
						char16.isCopy = true;
					}
					if (readCharInfo(char16, msg))
					{
						sbyte b73 = msg.reader().readByte();
						if (char16.cy <= 10 && b73 != 0 && b73 != 2)
						{
							Res.outz("nhân vật bay trên trời xuống x= " + char16.cx + " y= " + char16.cy);
							Teleport teleport2 = new Teleport(char16.cx, char16.cy, char16.head, char16.cdir, 1, isMe: false, (b73 != 1) ? b73 : char16.cgender);
							teleport2.id = char16.charID;
							char16.isTeleport = true;
							Teleport.addTeleport(teleport2);
						}
						if (b73 == 2)
						{
							char16.show();
						}
						for (int num181 = 0; num181 < GameScr.vMob.size(); num181++)
						{
							Mob mob10 = (Mob)GameScr.vMob.elementAt(num181);
							if (mob10 != null && mob10.isMobMe && mob10.mobId == char16.charID)
							{
								Res.outz("co 1 con quai");
								char16.mobMe = mob10;
								char16.mobMe.x = char16.cx;
								char16.mobMe.y = char16.cy - 40;
								break;
							}
						}
						if (GameScr.findCharInMap(char16.charID) == null)
						{
							GameScr.vCharInMap.addElement(char16);
							if (char16 != null && char16.charID < 0 && (char16.cTypePk == 5 || char16.cTypePk == 3 || ModMenu.IsBossName(char16.cName)))
							{
								ModMenu.AddBossNotice(char16.cName, TileMap.mapName, DateTime.Now.ToString("HH:mm:ss"));
							}
						}
						char16.isMonkey = msg.reader().readByte();
						short num182 = msg.reader().readShort();
						Res.outz("mount id= " + num182 + "+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++");
						if (num182 != -1)
						{
							char16.isHaveMount = true;
							switch (num182)
							{
							case 346:
							case 347:
							case 348:
								char16.isMountVip = false;
								break;
							case 349:
							case 350:
							case 351:
								char16.isMountVip = true;
								break;
							case 396:
								char16.isEventMount = true;
								break;
							case 532:
								char16.isSpeacialMount = true;
								break;
							default:
								if (num182 >= Char.ID_NEW_MOUNT)
								{
									char16.idMount = num182;
								}
								break;
							}
						}
						else
						{
							char16.isHaveMount = false;
						}
					}
					sbyte b74 = msg.reader().readByte();
					Res.outz("addplayer:   " + b74);
					char16.cFlag = b74;
					char16.isNhapThe = msg.reader().readByte() == 1;
					try
					{
						char16.idAuraEff = msg.reader().readShort();
						char16.idEff_Set_Item = msg.reader().readSByte();
						char16.idHat = msg.reader().readShort();
						Effect.GetCharEff(char16);
					}
					catch (Exception ex38)
					{
						Res.outz("cmd: -5 err: " + ex38.StackTrace);
					}
					GameScr.gI().getFlagImage(char16.charID, char16.cFlag);
					break;
				}
				case -7:
				{
					GameCanvas.debug("SA80", 2);
					int num173 = msg.reader().readInt();
					for (int num176 = 0; num176 < GameScr.vCharInMap.size(); num176++)
					{
						Char char15 = null;
						try
						{
							char15 = (Char)GameScr.vCharInMap.elementAt(num176);
						}
						catch (Exception)
						{
							continue;
						}
						if (char15 == null || char15.charID != num173)
						{
							continue;
						}
						GameCanvas.debug("SA8x2y" + num176, 2);
						char15.moveTo(msg.reader().readShort(), msg.reader().readShort(), 0);
						char15.lastUpdateTime = mSystem.currentTimeMillis();
						break;
					}
					GameCanvas.debug("SA80x3", 2);
					break;
				}
				case -6:
				{
					GameCanvas.debug("SA81", 2);
					int num173 = msg.reader().readInt();
					for (int num174 = 0; num174 < GameScr.vCharInMap.size(); num174++)
					{
						Char char14 = (Char)GameScr.vCharInMap.elementAt(num174);
						if (char14 != null && char14.charID == num173)
						{
							if (!char14.isInvisiblez && !char14.isUsePlane)
							{
								ServerEffect.addServerEffect(60, char14.cx, char14.cy, 1);
							}
							if (!char14.isUsePlane)
							{
								GameScr.vCharInMap.removeElementAt(num174);
							}
							return;
						}
					}
					break;
				}
				case -13:
				{
					GameCanvas.debug("SA82", 2);
					int num184 = msg.reader().readUnsignedByte();
					if (num184 > GameScr.vMob.size() - 1 || num184 < 0)
					{
						return;
					}
					Mob mob9 = (Mob)GameScr.vMob.elementAt(num184);
					mob9.sys = msg.reader().readByte();
					mob9.levelBoss = msg.reader().readByte();
					if (mob9.levelBoss != 0)
					{
						mob9.typeSuperEff = Res.random(0, 3);
					}
					mob9.x = mob9.xFirst;
					mob9.y = mob9.yFirst;
					mob9.status = 5;
					mob9.injureThenDie = false;
					mob9.hp = msg.reader().readLong();
					mob9.maxHp = mob9.hp;
					mob9.updateHp_bar();
					ServerEffect.addServerEffect(60, mob9.x, mob9.y, 1);
					break;
				}
				case -75:
				{
					Mob mob9 = null;
					try
					{
						mob9 = (Mob)GameScr.vMob.elementAt(msg.reader().readUnsignedByte());
					}
					catch (Exception)
					{
					}
					if (mob9 != null)
					{
						mob9.levelBoss = msg.reader().readByte();
						if (mob9.levelBoss > 0)
						{
							mob9.typeSuperEff = Res.random(0, 3);
						}
					}
					break;
				}
				case -9:
				{
					GameCanvas.debug("SA83", 2);
					Mob mob9 = null;
					try
					{
						mob9 = (Mob)GameScr.vMob.elementAt(msg.reader().readUnsignedByte());
					}
					catch (Exception)
					{
					}
					GameCanvas.debug("SA83v1", 2);
					if (mob9 != null)
					{
						mob9.hp = msg.reader().readLong();
						mob9.updateHp_bar();
						long num177 = msg.reader().readLong();
						if (num177 == 1)
						{
							return;
						}
						if (num177 > 1)
						{
							mob9.setInjure();
						}
						bool flag11 = false;
						try
						{
							flag11 = msg.reader().readBoolean();
						}
						catch (Exception)
						{
						}
						sbyte b72 = msg.reader().readByte();
						if (b72 != -1)
						{
							EffecMn.addEff(new Effect(b72, mob9.x, mob9.getY(), 3, 1, -1));
						}
						GameCanvas.debug("SA83v2", 2);
						if (flag11)
						{
							GameScr.startFlyText("-" + num177, mob9.x, mob9.getY() - mob9.getH(), 0, -2, mFont.FATAL);
						}
						else if (num177 == 0)
						{
							mob9.x = mob9.xFirst;
							mob9.y = mob9.yFirst;
							GameScr.startFlyText(mResources.miss, mob9.x, mob9.getY() - mob9.getH(), 0, -2, mFont.MISS);
						}
						else if (num177 > 1)
						{
							GameScr.startFlyText("-" + num177, mob9.x, mob9.getY() - mob9.getH(), 0, -2, mFont.ORANGE);
						}
					}
					GameCanvas.debug("SA83v3", 2);
					break;
				}
				case 45:
				{
					GameCanvas.debug("SA84", 2);
					Mob mob9 = null;
					try
					{
						mob9 = (Mob)GameScr.vMob.elementAt(msg.reader().readUnsignedByte());
					}
					catch (Exception ex29)
					{
						Cout.println("Loi tai NPC_MISS  " + ex29.ToString());
					}
					if (mob9 != null)
					{
						mob9.hp = msg.reader().readLong();
						mob9.updateHp_bar();
						GameScr.startFlyText(mResources.miss, mob9.x, mob9.y - mob9.h, 0, -2, mFont.MISS);
					}
					break;
				}
				case -12:
				{
					Res.outz("SERVER SEND MOB DIE");
					GameCanvas.debug("SA85", 2);
					Mob mob9 = null;
					try
					{
						mob9 = (Mob)GameScr.vMob.elementAt(msg.reader().readUnsignedByte());
					}
					catch (Exception)
					{
						Cout.println("LOi tai NPC_DIE cmd " + msg.command);
					}
					if (mob9 == null || mob9.status == 0 || mob9.status == 0)
					{
						break;
					}
					mob9.startDie();
					try
					{
						long num185 = msg.reader().readLong();
						if (msg.reader().readBool())
						{
							GameScr.startFlyText("-" + num185, mob9.x, mob9.y - mob9.h, 0, -2, mFont.FATAL);
						}
						else
						{
							GameScr.startFlyText("-" + num185, mob9.x, mob9.y - mob9.h, 0, -2, mFont.ORANGE);
						}
						sbyte b76 = msg.reader().readByte();
						for (int num186 = 0; num186 < b76; num186++)
						{
							ItemMap itemMap6 = new ItemMap(msg.reader().readShort(), msg.reader().readShort(), mob9.x, mob9.y, msg.reader().readShort(), msg.reader().readShort());
							int num187 = (itemMap6.playerId = msg.reader().readInt());
							Res.outz("playerid= " + num187 + " my id= " + Char.myCharz().charID);
							GameScr.vItemMap.addElement(itemMap6);
							if (Res.abs(itemMap6.y - Char.myCharz().cy) < 24 && Res.abs(itemMap6.x - Char.myCharz().cx) < 24)
							{
								Char.myCharz().charFocus = null;
							}
						}
					}
					catch (Exception)
					{
					}
					break;
				}
				case 74:
				{
					GameCanvas.debug("SA85", 2);
					Mob mob9 = null;
					try
					{
						mob9 = (Mob)GameScr.vMob.elementAt(msg.reader().readUnsignedByte());
					}
					catch (Exception)
					{
						Cout.println("Loi tai NPC CHANGE " + msg.command);
					}
					if (mob9 != null && mob9.status != 0 && mob9.status != 0)
					{
						mob9.status = 0;
						ServerEffect.addServerEffect(60, mob9.x, mob9.y, 1);
						ItemMap itemMap5 = new ItemMap(msg.reader().readShort(), msg.reader().readShort(), mob9.x, mob9.y, msg.reader().readShort(), msg.reader().readShort());
						GameScr.vItemMap.addElement(itemMap5);
						if (Res.abs(itemMap5.y - Char.myCharz().cy) < 24 && Res.abs(itemMap5.x - Char.myCharz().cx) < 24)
						{
							Char.myCharz().charFocus = null;
						}
					}
					break;
				}
				case -11:
				{
					GameCanvas.debug("SA86", 2);
					Mob mob9 = null;
					try
					{
						int index4 = msg.reader().readUnsignedByte();
						mob9 = (Mob)GameScr.vMob.elementAt(index4);
					}
					catch (Exception ex27)
					{
						Res.outz("Loi tai NPC_ATTACK_ME " + msg.command + " err= " + ex27.StackTrace);
					}
					if (mob9 != null)
					{
						Char.myCharz().isDie = false;
						Char.isLockKey = false;
						long num170 = msg.reader().readLong();
						long num171;
						try
						{
							num171 = msg.reader().readLong();
						}
						catch (Exception)
						{
							num171 = 0L;
						}
						if (mob9.isBusyAttackSomeOne)
						{
							Char.myCharz().doInjure(num170, num171, isCrit: false, isMob: true);
							break;
						}
						mob9.dame = num170;
						mob9.dameMp = num171;
						mob9.setAttack(Char.myCharz());
					}
					break;
				}
				case -10:
				{
					GameCanvas.debug("SA87", 2);
					Mob mob9 = null;
					try
					{
						mob9 = (Mob)GameScr.vMob.elementAt(msg.reader().readUnsignedByte());
					}
					catch (Exception)
					{
					}
					GameCanvas.debug("SA87x1", 2);
					if (mob9 != null)
					{
						GameCanvas.debug("SA87x2", 2);
						@char = GameScr.findCharInMap(msg.reader().readInt());
						if (@char == null)
						{
							return;
						}
						GameCanvas.debug("SA87x3", 2);
						long num179 = msg.reader().readLong();
						mob9.dame = @char.cHP - num179;
						@char.cHPNew = num179;
						GameCanvas.debug("SA87x4", 2);
						try
						{
							@char.cMP = msg.reader().readLong();
						}
						catch (Exception)
						{
						}
						GameCanvas.debug("SA87x5", 2);
						if (mob9.isBusyAttackSomeOne)
						{
							@char.doInjure(mob9.dame, 0L, isCrit: false, isMob: true);
						}
						else
						{
							mob9.setAttack(@char);
						}
						GameCanvas.debug("SA87x6", 2);
					}
					break;
				}
				case -17:
					GameCanvas.debug("SA88", 2);
					Char.myCharz().meDead = true;
					Char.myCharz().cPk = msg.reader().readByte();
					Char.myCharz().startDie(msg.reader().readShort(), msg.reader().readShort());
					try
					{
						Char.myCharz().cPower = msg.reader().readLong();
						Char.myCharz().applyCharLevelPercent();
					}
					catch (Exception)
					{
						Cout.println("Loi tai ME_DIE " + msg.command);
					}
					Char.myCharz().countKill = 0;
					break;
				case 66:
					Res.outz("ME DIE XP DOWN NOT IMPLEMENT YET!!!!!!!!!!!!!!!!!!!!!!!!!!");
					break;
				case -8:
					GameCanvas.debug("SA89", 2);
					@char = GameScr.findCharInMap(msg.reader().readInt());
					if (@char == null)
					{
						return;
					}
					@char.cPk = msg.reader().readByte();
					@char.waitToDie(msg.reader().readShort(), msg.reader().readShort());
					break;
				case -16:
					GameCanvas.debug("SA90", 2);
					if (Char.myCharz().wdx != 0 || Char.myCharz().wdy != 0)
					{
						Char.myCharz().cx = Char.myCharz().wdx;
						Char.myCharz().cy = Char.myCharz().wdy;
						Char.myCharz().wdx = (Char.myCharz().wdy = 0);
					}
					Char.myCharz().liveFromDead();
					Char.myCharz().isLockMove = false;
					Char.myCharz().meDead = false;
					break;
				case 44:
				{
					GameCanvas.debug("SA91", 2);
					int num172 = msg.reader().readInt();
					string text9 = msg.reader().readUTF();
					Res.outz("user id= " + num172 + " text= " + text9);
					@char = ((Char.myCharz().charID != num172) ? GameScr.findCharInMap(num172) : Char.myCharz());
					if (@char == null)
					{
						return;
					}
					@char.addInfo(text9);
					break;
				}
				case 18:
				{
					sbyte b70 = msg.reader().readByte();
					for (int num169 = 0; num169 < b70; num169++)
					{
						int charId = msg.reader().readInt();
						int cx = msg.reader().readShort();
						int cy = msg.reader().readShort();
						long cHPShow = msg.reader().readLong();
						Char char13 = GameScr.findCharInMap(charId);
						if (char13 != null)
						{
							char13.cx = cx;
							char13.cy = cy;
							char13.cHP = (char13.cHPShow = cHPShow);
							char13.lastUpdateTime = mSystem.currentTimeMillis();
						}
					}
					break;
				}
				case 19:
					Char.myCharz().countKill = msg.reader().readUnsignedShort();
					Char.myCharz().countKillMax = msg.reader().readUnsignedShort();
					break;
				}
				GameCanvas.debug("SA92", 2);
			}
			catch (Exception ex41)
			{
				Res.err("[Controller] [error] " + ex41.StackTrace + " msg: " + ex41.Message + " cause " + ex41.Data);
			}
			finally
			{
				msg?.cleanup();
			}
		}

	private void readLogin(Message msg)
		{
			sbyte b = msg.reader().readByte();
			ChooseCharScr.playerData = new PlayerData[b];
			Res.outz("[LEN] sl nguoi choi " + b);
			for (int i = 0; i < b; i++)
			{
				int playerID = msg.reader().readInt();
				string name = msg.reader().readUTF();
				short head = msg.reader().readShort();
				short body = msg.reader().readShort();
				short leg = msg.reader().readShort();
				long ppoint = msg.reader().readLong();
				ChooseCharScr.playerData[i] = new PlayerData(playerID, name, head, body, leg, ppoint);
			}
			GameCanvas.chooseCharScr.switchToMe();
			GameCanvas.chooseCharScr.updateChooseCharacter((byte)b);
		}

}
