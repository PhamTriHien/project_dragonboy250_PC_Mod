using System;
using Assets.src.e;
using Assets.src.g;
using UnityEngine;

namespace Assets.src.f
{
public partial class Controller2
{
	public static bool readMessage_Part2(sbyte cmd, Message msg)
	{
		switch (cmd)
		{
				case -117:
					GameScr.gI().tMabuEff = 0;
					GameScr.gI().percentMabu = msg.reader().readByte();
					if (GameScr.gI().percentMabu == 100)
					{
						GameScr.gI().mabuEff = true;
					}
					if (GameScr.gI().percentMabu == 101)
					{
						Npc.mabuEff = true;
					}
					break;
				case -116:
					GameScr.canAutoPlay = msg.reader().readByte() == 1;
					break;
				case -115:
					Char.myCharz().setPowerInfo(msg.reader().readUTF(), msg.reader().readShort(), msg.reader().readShort(), msg.reader().readShort());
					break;
				case -113:
				{
					sbyte[] array6 = new sbyte[10];
					for (int num29 = 0; num29 < 10; num29++)
					{
						array6[num29] = msg.reader().readByte();
						Res.outz("vlue i= " + array6[num29]);
					}
					GameScr.gI().onKSkill(array6);
					GameScr.gI().onOSkill(array6);
					GameScr.gI().onCSkill(array6);
					break;
				}
				case -111:
				{
					short num10 = msg.reader().readShort();
					ImageSource.vSource = new MyVector();
					for (int l = 0; l < num10; l++)
					{
						string iD = msg.reader().readUTF();
						sbyte version = msg.reader().readByte();
						ImageSource.vSource.addElement(new ImageSource(iD, version));
					}
					ImageSource.checkRMS();
					ImageSource.saveRMS();
					break;
				}
				case 125:
				{
					sbyte fusion = msg.reader().readByte();
					int num11 = msg.reader().readInt();
					if (num11 == Char.myCharz().charID)
					{
						Char.myCharz().setFusion(fusion);
					}
					else if (GameScr.findCharInMap(num11) != null)
					{
						GameScr.findCharInMap(num11).setFusion(fusion);
					}
					break;
				}
				case 124:
				{
					short num23 = msg.reader().readShort();
					string text4 = msg.reader().readUTF();
					Res.outz("noi chuyen = " + text4 + "npc ID= " + num23);
					GameScr.findNPCInMap(num23)?.addInfo(text4);
					break;
				}
				case 123:
				{
					Res.outz("SET POSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSss");
					int num3 = msg.reader().readInt();
					short xPos = msg.reader().readShort();
					short yPos = msg.reader().readShort();
					sbyte b4 = msg.reader().readByte();
					Char @char = null;
					if (num3 == Char.myCharz().charID)
					{
						@char = Char.myCharz();
					}
					else if (GameScr.findCharInMap(num3) != null)
					{
						@char = GameScr.findCharInMap(num3);
					}
					if (@char != null)
					{
						ServerEffect.addServerEffect((b4 != 0) ? 173 : 60, @char, 1);
						@char.setPos(xPos, yPos, b4);
					}
					break;
				}
				case 122:
				{
					short num28 = msg.reader().readShort();
					Res.outz("second login = " + num28);
					LoginScr.timeLogin = num28;
					LoginScr.currTimeLogin = (LoginScr.lastTimeLogin = mSystem.currentTimeMillis());
					GameCanvas.endDlg();
					break;
				}
				case 121:
					mSystem.publicID = msg.reader().readUTF();
					mSystem.strAdmob = msg.reader().readUTF();
					Res.outz("SHOW AD public ID= " + mSystem.publicID);
					mSystem.createAdmob();
					break;
				case -124:
				{
					sbyte b7 = msg.reader().readByte();
					sbyte b8 = msg.reader().readByte();
					if (b8 == 0)
					{
						if (b7 == 2)
						{
							int num4 = msg.reader().readInt();
							if (num4 == Char.myCharz().charID)
							{
								Char.myCharz().removeEffect();
							}
							else if (GameScr.findCharInMap(num4) != null)
							{
								GameScr.findCharInMap(num4).removeEffect();
							}
						}
						int num5 = msg.reader().readUnsignedByte();
						int num6 = msg.reader().readInt();
						if (num5 == 32)
						{
							if (b7 == 1)
							{
								int num7 = msg.reader().readInt();
								if (num6 == Char.myCharz().charID)
								{
									Char.myCharz().holdEffID = num5;
									GameScr.findCharInMap(num7).setHoldChar(Char.myCharz());
								}
								else if (GameScr.findCharInMap(num6) != null && num7 != Char.myCharz().charID)
								{
									GameScr.findCharInMap(num6).holdEffID = num5;
									GameScr.findCharInMap(num7).setHoldChar(GameScr.findCharInMap(num6));
								}
								else if (GameScr.findCharInMap(num6) != null && num7 == Char.myCharz().charID)
								{
									GameScr.findCharInMap(num6).holdEffID = num5;
									Char.myCharz().setHoldChar(GameScr.findCharInMap(num6));
								}
							}
							else if (num6 == Char.myCharz().charID)
							{
								Char.myCharz().removeHoleEff();
							}
							else if (GameScr.findCharInMap(num6) != null)
							{
								GameScr.findCharInMap(num6).removeHoleEff();
							}
						}
						if (num5 == 33)
						{
							if (b7 == 1)
							{
								if (num6 == Char.myCharz().charID)
								{
									Char.myCharz().protectEff = true;
								}
								else if (GameScr.findCharInMap(num6) != null)
								{
									GameScr.findCharInMap(num6).protectEff = true;
								}
							}
							else if (num6 == Char.myCharz().charID)
							{
								Char.myCharz().removeProtectEff();
							}
							else if (GameScr.findCharInMap(num6) != null)
							{
								GameScr.findCharInMap(num6).removeProtectEff();
							}
						}
						if (num5 == 39)
						{
							if (b7 == 1)
							{
								if (num6 == Char.myCharz().charID)
								{
									Char.myCharz().huytSao = true;
								}
								else if (GameScr.findCharInMap(num6) != null)
								{
									GameScr.findCharInMap(num6).huytSao = true;
								}
							}
							else if (num6 == Char.myCharz().charID)
							{
								Char.myCharz().removeHuytSao();
							}
							else if (GameScr.findCharInMap(num6) != null)
							{
								GameScr.findCharInMap(num6).removeHuytSao();
							}
						}
						if (num5 == 40)
						{
							if (b7 == 1)
							{
								if (num6 == Char.myCharz().charID)
								{
									Char.myCharz().blindEff = true;
								}
								else if (GameScr.findCharInMap(num6) != null)
								{
									GameScr.findCharInMap(num6).blindEff = true;
								}
							}
							else if (num6 == Char.myCharz().charID)
							{
								Char.myCharz().removeBlindEff();
							}
							else if (GameScr.findCharInMap(num6) != null)
							{
								GameScr.findCharInMap(num6).removeBlindEff();
							}
						}
						if (num5 == 41)
						{
							if (b7 == 1)
							{
								if (num6 == Char.myCharz().charID)
								{
									Char.myCharz().sleepEff = true;
								}
								else if (GameScr.findCharInMap(num6) != null)
								{
									GameScr.findCharInMap(num6).sleepEff = true;
								}
							}
							else if (num6 == Char.myCharz().charID)
							{
								Char.myCharz().removeSleepEff();
							}
							else if (GameScr.findCharInMap(num6) != null)
							{
								GameScr.findCharInMap(num6).removeSleepEff();
							}
						}
						if (num5 == 42)
						{
							if (b7 == 1)
							{
								if (num6 == Char.myCharz().charID)
								{
									Char.myCharz().stone = true;
								}
							}
							else if (num6 == Char.myCharz().charID)
							{
								Char.myCharz().stone = false;
							}
						}
					}
					if (b8 != 1)
					{
						break;
					}
					int num8 = msg.reader().readUnsignedByte();
					sbyte b9 = msg.reader().readByte();
					Res.outz("modbHoldID= " + b9 + " skillID= " + num8 + "eff ID= " + b7);
					if (num8 == 32)
					{
						if (b7 == 1)
						{
							int num9 = msg.reader().readInt();
							if (num9 == Char.myCharz().charID)
							{
								GameScr.findMobInMap(b9).holdEffID = num8;
								Char.myCharz().setHoldMob(GameScr.findMobInMap(b9));
							}
							else if (GameScr.findCharInMap(num9) != null)
							{
								GameScr.findMobInMap(b9).holdEffID = num8;
								GameScr.findCharInMap(num9).setHoldMob(GameScr.findMobInMap(b9));
							}
						}
						else
						{
							GameScr.findMobInMap(b9).removeHoldEff();
						}
					}
					if (num8 == 40)
					{
						if (b7 == 1)
						{
							GameScr.findMobInMap(b9).blindEff = true;
						}
						else
						{
							GameScr.findMobInMap(b9).removeBlindEff();
						}
					}
					if (num8 == 41)
					{
						if (b7 == 1)
						{
							GameScr.findMobInMap(b9).sleepEff = true;
						}
						else
						{
							GameScr.findMobInMap(b9).removeSleepEff();
						}
					}
					break;
				}
				case -125:
				{
					ChatTextField.gI().isShow = false;
					string text = msg.reader().readUTF();
					Res.outz("titile= " + text);
					sbyte b5 = msg.reader().readByte();
					ClientInput.gI().setInput(b5, text);
					for (int k = 0; k < b5; k++)
					{
						ClientInput.gI().tf[k].name = msg.reader().readUTF();
						sbyte b6 = msg.reader().readByte();
						if (b6 == 0)
						{
							ClientInput.gI().tf[k].setIputType(TField.INPUT_TYPE_NUMERIC);
						}
						if (b6 == 1)
						{
							ClientInput.gI().tf[k].setIputType(TField.INPUT_TYPE_ANY);
						}
						if (b6 == 2)
						{
							ClientInput.gI().tf[k].setIputType(TField.INPUT_TYPE_PASSWORD);
						}
					}
					break;
				}
				case -110:
				{
					sbyte b27 = msg.reader().readByte();
					if (b27 == 1)
					{
						int num36 = msg.reader().readInt();
						sbyte[] array11 = Rms.loadRMS(num36 + string.Empty);
						if (array11 == null)
						{
							Service.gI().sendServerData(1, -1, null);
						}
						else
						{
							Service.gI().sendServerData(1, num36, array11);
						}
					}
					if (b27 == 0)
					{
						int num37 = msg.reader().readInt();
						short num38 = msg.reader().readShort();
						sbyte[] data = new sbyte[num38];
						msg.reader().read(ref data, 0, num38);
						Rms.saveRMS(num37 + string.Empty, data);
					}
					break;
				}
				case 93:
				{
					string str = msg.reader().readUTF();
					str = Res.changeString(str);
					GameScr.gI().chatVip(str);
					break;
				}
				case -106:
				{
					short num30 = msg.reader().readShort();
					int num31 = msg.reader().readShort();
					if (ItemTime.isExistItem(num30))
					{
						ItemTime.getItemById(num30).initTime(num31);
						break;
					}
					ItemTime o = new ItemTime(num30, num31);
					Char.vItemTime.addElement(o);
					break;
				}
				case -105:
					TransportScr.gI().time = 0;
					TransportScr.gI().maxTime = msg.reader().readShort();
					TransportScr.gI().last = (TransportScr.gI().curr = mSystem.currentTimeMillis());
					TransportScr.gI().type = msg.reader().readByte();
					TransportScr.gI().switchToMe();
					break;
				case -103:
				{
					sbyte b12 = msg.reader().readByte();
					if (b12 == 0)
					{
						GameCanvas.panel.vFlag.removeAllElements();
						sbyte b13 = msg.reader().readByte();
						for (int m = 0; m < b13; m++)
						{
							Item item = new Item();
							short num12 = msg.reader().readShort();
							if (num12 != -1)
							{
								item.template = ItemTemplates.get(num12);
								sbyte b14 = msg.reader().readByte();
								if (b14 != -1)
								{
									item.itemOption = new ItemOption[b14];
									for (int n = 0; n < item.itemOption.Length; n++)
									{
										ItemOption itemOption2 = Controller.gI().readItemOption(msg);
										if (itemOption2 != null)
										{
											item.itemOption[n] = itemOption2;
										}
									}
								}
							}
							GameCanvas.panel.vFlag.addElement(item);
						}
						GameCanvas.panel.setTypeFlag();
						GameCanvas.panel.show();
					}
					else if (b12 == 1)
					{
						int num13 = msg.reader().readInt();
						sbyte b15 = msg.reader().readByte();
						Res.outz("---------------actionFlag1:  " + num13 + " : " + b15);
						if (num13 == Char.myCharz().charID)
						{
							Char.myCharz().cFlag = b15;
						}
						else if (GameScr.findCharInMap(num13) != null)
						{
							GameScr.findCharInMap(num13).cFlag = b15;
						}
						GameScr.gI().getFlagImage(num13, b15);
					}
					else
					{
						if (b12 != 2)
						{
							break;
						}
						sbyte b16 = msg.reader().readByte();
						int num14 = msg.reader().readShort();
						PKFlag pKFlag = new PKFlag();
						pKFlag.cflag = b16;
						pKFlag.IDimageFlag = num14;
						GameScr.vFlag.addElement(pKFlag);
						for (int num15 = 0; num15 < GameScr.vFlag.size(); num15++)
						{
							PKFlag pKFlag2 = (PKFlag)GameScr.vFlag.elementAt(num15);
							Res.outz("i: " + num15 + "  cflag: " + pKFlag2.cflag + "   IDimageFlag: " + pKFlag2.IDimageFlag);
						}
						for (int num16 = 0; num16 < GameScr.vCharInMap.size(); num16++)
						{
							Char char2 = (Char)GameScr.vCharInMap.elementAt(num16);
							if (char2 != null && char2.cFlag == b16)
							{
								char2.flagImage = num14;
							}
						}
						if (Char.myCharz().cFlag == b16)
						{
							Char.myCharz().flagImage = num14;
						}
					}
					break;
				}
				case -102:
				{
					sbyte b11 = msg.reader().readByte();
					if (b11 != 0 && b11 == 1)
					{
						GameCanvas.loginScr.isLogin2 = false;
						Service.gI().login(Rms.loadRMSString(Rms.RMS_acc), Rms.loadRMSString(Rms.RMS_pass), GameMidlet.VERSION, 0);
						LoginScr.isLoggingIn = true;
					}
					break;
				}
				case -101:
				{
					GameCanvas.loginScr.isLogin2 = true;
					GameCanvas.connect();
					string text2 = msg.reader().readUTF();
					Rms.saveRMSString(Rms.RMS_userAo + ServerListScreen.ipSelect, text2);
					Service.gI().setClientType();
					Service.gI().login(text2, string.Empty, GameMidlet.VERSION, 1);
					break;
				}
				case -100:
				{
					InfoDlg.hide();
					bool flag = false;
					if (GameCanvas.w > 2 * Panel.WIDTH_PANEL)
					{
						flag = true;
					}
					sbyte b = msg.reader().readByte();
					if (b < 0)
					{
						break;
					}
					Res.outz("t Indxe= " + b);
					GameCanvas.panel.maxPageShop[b] = msg.reader().readByte();
					GameCanvas.panel.currPageShop[b] = msg.reader().readByte();
					Res.outz("max page= " + GameCanvas.panel.maxPageShop[b] + " curr page= " + GameCanvas.panel.currPageShop[b]);
					int num = msg.reader().readUnsignedByte();
					Char.myCharz().arrItemShop[b] = new Item[num];
					for (int i = 0; i < num; i++)
					{
						short num2 = msg.reader().readShort();
						if (num2 == -1)
						{
							continue;
						}
						Res.outz("template id= " + num2);
						Char.myCharz().arrItemShop[b][i] = new Item();
						Char.myCharz().arrItemShop[b][i].template = ItemTemplates.get(num2);
						Char.myCharz().arrItemShop[b][i].itemId = msg.reader().readShort();
						Char.myCharz().arrItemShop[b][i].buyCoin = msg.reader().readInt();
						Char.myCharz().arrItemShop[b][i].buyGold = msg.reader().readInt();
						Char.myCharz().arrItemShop[b][i].buyType = msg.reader().readByte();
						Char.myCharz().arrItemShop[b][i].quantity = msg.reader().readInt();
						Char.myCharz().arrItemShop[b][i].isMe = msg.reader().readByte();
						Panel.strWantToBuy = mResources.say_wat_do_u_want_to_buy;
						sbyte b2 = msg.reader().readByte();
						if (b2 != -1)
						{
							Char.myCharz().arrItemShop[b][i].itemOption = new ItemOption[b2];
							for (int j = 0; j < Char.myCharz().arrItemShop[b][i].itemOption.Length; j++)
							{
								ItemOption itemOption = Controller.gI().readItemOption(msg);
								if (itemOption != null)
								{
									Char.myCharz().arrItemShop[b][i].itemOption[j] = itemOption;
									Char.myCharz().arrItemShop[b][i].compare = GameCanvas.panel.getCompare(Char.myCharz().arrItemShop[b][i]);
								}
							}
						}
						sbyte b3 = msg.reader().readByte();
						if (b3 == 1)
						{
							int headTemp = msg.reader().readShort();
							int bodyTemp = msg.reader().readShort();
							int legTemp = msg.reader().readShort();
							int bagTemp = msg.reader().readShort();
							Char.myCharz().arrItemShop[b][i].setPartTemp(headTemp, bodyTemp, legTemp, bagTemp);
						}
						if (GameMidlet.intVERSION >= 237)
						{
							Char.myCharz().arrItemShop[b][i].nameNguoiKyGui = msg.reader().readUTF();
							Res.err("nguoi ki gui  " + Char.myCharz().arrItemShop[b][i].nameNguoiKyGui);
						}
					}
					if (flag)
					{
						GameCanvas.panel2.setTabKiGui();
					}
					GameCanvas.panel.setTabShop();
					GameCanvas.panel.cmy = (GameCanvas.panel.cmtoY = 0);
					break;
				}
			default:
				return false;
		}
		return true;
	}

}
}
