using System;
using Assets.src.e;
using Assets.src.f;
using Assets.src.g;
using UnityEngine;

public partial class Controller : IMessageHandler
{
	public bool onMessage_Part4(Message msg)
	{
		Char @char = null;
		Mob mob = null;
		MyVector myVector = new MyVector();
		int num = 0;
		switch (msg.command)
		{
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
						return true;
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
						return true;
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
							return true;
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
			default:
				return false;
		}
		return true;
	}

}
