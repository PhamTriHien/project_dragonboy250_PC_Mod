using System;
using Assets.src.e;
using Assets.src.g;
using UnityEngine;

namespace Assets.src.f
{
public partial class Controller2
{
	public static bool readMessage_Part1(sbyte cmd, Message msg)
	{
		switch (cmd)
		{
				case sbyte.MinValue:
					readInfoEffChar(msg);
					break;
				case sbyte.MaxValue:
					readInfoRada(msg);
					break;
				case 114:
					try
					{
						string text3 = msg.reader().readUTF();
						mSystem.curINAPP = msg.reader().readByte();
						mSystem.maxINAPP = msg.reader().readByte();
						break;
					}
					catch (Exception)
					{
						break;
					}
				case 113:
				{
					int loop = 0;
					int layer = 0;
					int id = 0;
					short x = 0;
					short y = 0;
					short loopCount = -1;
					try
					{
						loop = msg.reader().readByte();
						layer = msg.reader().readByte();
						id = msg.reader().readShort();
						x = msg.reader().readShort();
						y = msg.reader().readShort();
						loopCount = msg.reader().readShort();
					}
					catch (Exception)
					{
					}
					EffecMn.addEff(new Effect(id, x, y, layer, loop, loopCount));
					break;
				}
				case 48:
				{
					sbyte b10 = msg.reader().readByte();
					ServerListScreen.SetIpSelect(b10, issave: false);
					GameCanvas.instance.doResetToLoginScr(GameCanvas.serverScreen);
					Session_ME.gI().close();
					GameCanvas.endDlg();
					ServerListScreen.waitToLogin = true;
					break;
				}
				case 31:
				{
					int num17 = msg.reader().readInt();
					sbyte b17 = msg.reader().readByte();
					if (b17 == 1)
					{
						short smallID = msg.reader().readShort();
						sbyte b18 = -1;
						int[] array = null;
						short wimg = 0;
						short himg = 0;
						try
						{
							b18 = msg.reader().readByte();
							if (b18 > 0)
							{
								sbyte b19 = msg.reader().readByte();
								array = new int[b19];
								for (int num18 = 0; num18 < b19; num18++)
								{
									array[num18] = msg.reader().readByte();
								}
								wimg = msg.reader().readShort();
								himg = msg.reader().readShort();
							}
						}
						catch (Exception)
						{
						}
						if (num17 == Char.myCharz().charID)
						{
							Char.myCharz().petFollow = new PetFollow();
							Char.myCharz().petFollow.smallID = smallID;
							if (b18 > 0)
							{
								Char.myCharz().petFollow.SetImg(b18, array, wimg, himg);
							}
							break;
						}
						Char char3 = GameScr.findCharInMap(num17);
						char3.petFollow = new PetFollow();
						char3.petFollow.smallID = smallID;
						if (b18 > 0)
						{
							char3.petFollow.SetImg(b18, array, wimg, himg);
						}
					}
					else if (num17 == Char.myCharz().charID)
					{
						Char.myCharz().petFollow.remove();
						Char.myCharz().petFollow = null;
					}
					else
					{
						Char char4 = GameScr.findCharInMap(num17);
						char4.petFollow.remove();
						char4.petFollow = null;
					}
					break;
				}
				case -89:
					GameCanvas.open3Hour = msg.reader().readByte() == 1;
					break;
				case 42:
				{
					GameCanvas.endDlg();
					LoginScr.isContinueToLogin = false;
					Char.isLoadingMap = false;
					sbyte haveName = msg.reader().readByte();
					if (GameCanvas.registerScr == null)
					{
						GameCanvas.registerScr = new RegisterScreen(haveName);
					}
					GameCanvas.registerScr.switchToMe();
					break;
				}
				case 52:
				{
					sbyte b23 = msg.reader().readByte();
					if (b23 == 1)
					{
						int num25 = msg.reader().readInt();
						if (num25 == Char.myCharz().charID)
						{
							Char.myCharz().setMabuHold(m: true);
							Char.myCharz().cx = msg.reader().readShort();
							Char.myCharz().cy = msg.reader().readShort();
						}
						else
						{
							Char char5 = GameScr.findCharInMap(num25);
							if (char5 != null)
							{
								char5.setMabuHold(m: true);
								char5.cx = msg.reader().readShort();
								char5.cy = msg.reader().readShort();
							}
						}
					}
					if (b23 == 0)
					{
						int num26 = msg.reader().readInt();
						if (num26 == Char.myCharz().charID)
						{
							Char.myCharz().setMabuHold(m: false);
						}
						else
						{
							GameScr.findCharInMap(num26)?.setMabuHold(m: false);
						}
					}
					if (b23 == 2)
					{
						int charId2 = msg.reader().readInt();
						int id3 = msg.reader().readInt();
						Mabu mabu2 = (Mabu)GameScr.findCharInMap(charId2);
						mabu2.eat(id3);
					}
					if (b23 == 3)
					{
						GameScr.mabuPercent = msg.reader().readByte();
					}
					break;
				}
				case 51:
				{
					int charId = msg.reader().readInt();
					Mabu mabu = (Mabu)GameScr.findCharInMap(charId);
					sbyte id2 = msg.reader().readByte();
					short x2 = msg.reader().readShort();
					short y2 = msg.reader().readShort();
					sbyte b20 = msg.reader().readByte();
					Char[] array2 = new Char[b20];
					long[] array3 = new long[b20];
					for (int num19 = 0; num19 < b20; num19++)
					{
						int num20 = msg.reader().readInt();
						Res.outz("char ID=" + num20);
						array2[num19] = null;
						if (num20 != Char.myCharz().charID)
						{
							array2[num19] = GameScr.findCharInMap(num20);
						}
						else
						{
							array2[num19] = Char.myCharz();
						}
						array3[num19] = msg.reader().readLong();
					}
					mabu.setSkill(id2, x2, y2, array2, array3);
					break;
				}
				case -127:
					readLuckyRound(msg);
					break;
				case -126:
				{
					sbyte b29 = msg.reader().readByte();
					Res.outz("type quay= " + b29);
					if (b29 == 1)
					{
						sbyte b30 = msg.reader().readByte();
						string num40 = msg.reader().readUTF();
						string finish = msg.reader().readUTF();
						GameScr.gI().showWinNumber(num40, finish);
					}
					if (b29 == 0)
					{
						GameScr.gI().showYourNumber(msg.reader().readUTF());
					}
					break;
				}
				case -122:
				{
					short id4 = msg.reader().readShort();
					Npc npc = GameScr.findNPCInMap(id4);
					sbyte b28 = msg.reader().readByte();
					npc.duahau = new int[b28];
					Res.outz("N DUA HAU= " + b28);
					for (int num39 = 0; num39 < b28; num39++)
					{
						npc.duahau[num39] = msg.reader().readShort();
					}
					npc.setStatus(msg.reader().readByte(), msg.reader().readInt());
					break;
				}
				case 102:
				{
					sbyte b24 = msg.reader().readByte();
					if (b24 == 0 || b24 == 1 || b24 == 2 || b24 == 6)
					{
						BigBoss2 bigBoss2 = Mob.getBigBoss2();
						if (bigBoss2 == null)
						{
							break;
						}
						if (b24 == 6)
						{
							bigBoss2.x = (bigBoss2.y = (bigBoss2.xTo = (bigBoss2.yTo = (bigBoss2.xFirst = (bigBoss2.yFirst = -1000)))));
							break;
						}
						sbyte b25 = msg.reader().readByte();
						Char[] array7 = new Char[b25];
						long[] array8 = new long[b25];
						for (int num32 = 0; num32 < b25; num32++)
						{
							int num33 = msg.reader().readInt();
							array7[num32] = null;
							if (num33 != Char.myCharz().charID)
							{
								array7[num32] = GameScr.findCharInMap(num33);
							}
							else
							{
								array7[num32] = Char.myCharz();
							}
							array8[num32] = msg.reader().readLong();
						}
						bigBoss2.setAttack(array7, array8, b24);
					}
					if (b24 == 3 || b24 == 4 || b24 == 5 || b24 == 7)
					{
						BachTuoc bachTuoc = Mob.getBachTuoc();
						if (bachTuoc == null)
						{
							break;
						}
						if (b24 == 7)
						{
							bachTuoc.x = (bachTuoc.y = (bachTuoc.xTo = (bachTuoc.yTo = (bachTuoc.xFirst = (bachTuoc.yFirst = -1000)))));
							break;
						}
						if (b24 == 3 || b24 == 4)
						{
							sbyte b26 = msg.reader().readByte();
							Char[] array9 = new Char[b26];
							long[] array10 = new long[b26];
							for (int num34 = 0; num34 < b26; num34++)
							{
								int num35 = msg.reader().readInt();
								array9[num34] = null;
								if (num35 != Char.myCharz().charID)
								{
									array9[num34] = GameScr.findCharInMap(num35);
								}
								else
								{
									array9[num34] = Char.myCharz();
								}
								array10[num34] = msg.reader().readLong();
							}
							bachTuoc.setAttack(array9, array10, b24);
						}
						if (b24 == 5)
						{
							short xMoveTo = msg.reader().readShort();
							bachTuoc.move(xMoveTo);
						}
					}
					if (b24 > 9 && b24 < 30)
					{
						readActionBoss(msg, b24);
					}
					break;
				}
				case 101:
				{
					Res.outz("big boss--------------------------------------------------");
					BigBoss bigBoss = Mob.getBigBoss();
					if (bigBoss == null)
					{
						break;
					}
					sbyte b21 = msg.reader().readByte();
					if (b21 == 0 || b21 == 1 || b21 == 2 || b21 == 4 || b21 == 3)
					{
						if (b21 == 3)
						{
							bigBoss.xTo = (bigBoss.xFirst = msg.reader().readShort());
							bigBoss.yTo = (bigBoss.yFirst = msg.reader().readShort());
							bigBoss.setFly();
						}
						else
						{
							sbyte b22 = msg.reader().readByte();
							Res.outz("CHUONG nChar= " + b22);
							Char[] array4 = new Char[b22];
							long[] array5 = new long[b22];
							for (int num21 = 0; num21 < b22; num21++)
							{
								int num22 = msg.reader().readInt();
								Res.outz("char ID=" + num22);
								array4[num21] = null;
								if (num22 != Char.myCharz().charID)
								{
									array4[num21] = GameScr.findCharInMap(num22);
								}
								else
								{
									array4[num21] = Char.myCharz();
								}
								array5[num21] = msg.reader().readLong();
							}
							bigBoss.setAttack(array4, array5, b21);
						}
					}
					if (b21 == 5)
					{
						bigBoss.haftBody = true;
						bigBoss.status = 2;
					}
					if (b21 == 6)
					{
						bigBoss.getDataB2();
						bigBoss.x = msg.reader().readShort();
						bigBoss.y = msg.reader().readShort();
					}
					if (b21 == 7)
					{
						bigBoss.setAttack(null, null, b21);
					}
					if (b21 == 8)
					{
						bigBoss.xTo = (bigBoss.xFirst = msg.reader().readShort());
						bigBoss.yTo = (bigBoss.yFirst = msg.reader().readShort());
						bigBoss.status = 2;
					}
					if (b21 == 9)
					{
						bigBoss.x = (bigBoss.y = (bigBoss.xTo = (bigBoss.yTo = (bigBoss.xFirst = (bigBoss.yFirst = -1000)))));
					}
					break;
				}
				case -120:
				{
					long num24 = mSystem.currentTimeMillis();
					Service.logController = num24 - Service.curCheckController;
					Service.gI().sendCheckController();
					break;
				}
				case -121:
				{
					long num27 = mSystem.currentTimeMillis();
					Service.logMap = num27 - Service.curCheckMap;
					Service.gI().sendCheckMap();
					break;
				}
				case 100:
				{
					sbyte b31 = msg.reader().readByte();
					sbyte b32 = msg.reader().readByte();
					Item item2 = null;
					if (b31 == 0)
					{
						item2 = Char.myCharz().arrItemBody[b32];
					}
					if (b31 == 1)
					{
						item2 = Char.myCharz().arrItemBag[b32];
					}
					short num41 = msg.reader().readShort();
					if (num41 == -1)
					{
						break;
					}
					item2.template = ItemTemplates.get(num41);
					item2.quantity = msg.reader().readInt();
					item2.info = msg.reader().readUTF();
					item2.content = msg.reader().readUTF();
					sbyte b33 = msg.reader().readByte();
					if (b33 != 0)
					{
						item2.itemOption = new ItemOption[b33];
						for (int num42 = 0; num42 < item2.itemOption.Length; num42++)
						{
							ItemOption itemOption3 = Controller.gI().readItemOption(msg);
							if (itemOption3 != null)
							{
								item2.itemOption[num42] = itemOption3;
							}
						}
					}
					if (item2.quantity <= 0)
					{
						item2 = null;
					}
					break;
				}
				case -123:
				{
					int charId3 = msg.reader().readInt();
					if (GameScr.findCharInMap(charId3) != null)
					{
						GameScr.findCharInMap(charId3).perCentMp = msg.reader().readByte();
					}
					break;
				}
				case -119:
					Char.myCharz().rank = msg.reader().readInt();
					break;
			default:
				return false;
		}
		return true;
	}

}
}
