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
				if (onMessage_Part1(msg)) return;
				if (onMessage_Part2(msg)) return;
				if (onMessage_Part3(msg)) return;
				if (onMessage_Part4(msg)) return;
				if (onMessage_Part5(msg)) return;
				if (onMessage_Part6(msg)) return;
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
