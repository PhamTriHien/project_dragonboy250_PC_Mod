using System;
using Assets.src.e;
using Assets.src.f;
using Assets.src.g;
using UnityEngine;

public partial class Controller : IMessageHandler
{
	public bool onMessage_Part3b(Message msg)
	{
		Char @char = null;
		Mob mob = null;
		MyVector myVector = new MyVector();
		int num = 0;
		switch (msg.command)
		{
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
			default:
				return false;
		}
		return true;
	}
}
