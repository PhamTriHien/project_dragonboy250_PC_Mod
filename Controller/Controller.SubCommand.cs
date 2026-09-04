using System;
using Assets.src.e;
using Assets.src.f;
using Assets.src.g;
using UnityEngine;

public partial class Controller : IMessageHandler
{
	public void messageSubCommand(Message msg)
		{
			try
			{
				GameCanvas.debug("SA12", 2);
				sbyte b = msg.reader().readByte();
				Res.outz("---messageSubCommand : " + b);
				switch (b)
				{
				case 63:
				{
					sbyte b5 = msg.reader().readByte();
					if (b5 > 0)
					{
						GameCanvas.panel.vPlayerMenu_id.removeAllElements();
						InfoDlg.showWait();
						MyVector vPlayerMenu = GameCanvas.panel.vPlayerMenu;
						for (int j = 0; j < b5; j++)
						{
							string caption = msg.reader().readUTF();
							string caption2 = msg.reader().readUTF();
							short num5 = msg.reader().readShort();
							GameCanvas.panel.vPlayerMenu_id.addElement(num5 + string.Empty);
							Char.myCharz().charFocus.menuSelect = num5;
							Command command = new Command(caption, 11115, Char.myCharz().charFocus);
							command.caption2 = caption2;
							vPlayerMenu.addElement(command);
						}
						InfoDlg.hide();
						GameCanvas.panel.setTabPlayerMenu();
					}
					break;
				}
				case 1:
					GameCanvas.debug("SA13", 2);
					Char.myCharz().nClass = GameScr.nClasss[msg.reader().readByte()];
					Char.myCharz().cTiemNang = msg.reader().readLong();
					Char.myCharz().vSkill.removeAllElements();
					Char.myCharz().vSkillFight.removeAllElements();
					Char.myCharz().myskill = null;
					break;
				case 2:
				{
					GameCanvas.debug("SA14", 2);
					if (Char.myCharz().statusMe != 14 && Char.myCharz().statusMe != 5)
					{
						Char.myCharz().cHP = Char.myCharz().cHPFull;
						Char.myCharz().cMP = Char.myCharz().cMPFull;
						Cout.LogError2(" ME_LOAD_SKILL");
					}
					Char.myCharz().vSkill.removeAllElements();
					Char.myCharz().vSkillFight.removeAllElements();
					sbyte b2 = msg.reader().readByte();
					for (sbyte b3 = 0; b3 < b2; b3++)
					{
						short skillId = msg.reader().readShort();
						Skill skill2 = Skills.get(skillId);
						useSkill(skill2);
					}
					GameScr.gI().sortSkill();
					if (GameScr.isPaintInfoMe)
					{
						GameScr.indexRow = -1;
						GameScr.gI().left = (GameScr.gI().center = null);
					}
					break;
				}
				case 19:
					GameCanvas.debug("SA17", 2);
					Char.myCharz().boxSort();
					break;
				case 21:
				{
					GameCanvas.debug("SA19", 2);
					int num3 = msg.reader().readInt();
					Char.myCharz().xuInBox -= num3;
					Char.myCharz().xu += num3;
					Char.myCharz().xuStr = mSystem.numberTostring(Char.myCharz().xu);
					break;
				}
				case 0:
				{
					GameCanvas.debug("SA21", 2);
					RadarScr.list = new MyVector();
					Teleport.vTeleport.removeAllElements();
					GameScr.vCharInMap.removeAllElements();
					GameScr.vItemMap.removeAllElements();
					Char.vItemTime.removeAllElements();
					GameScr.loadImg();
					GameScr.currentCharViewInfo = Char.myCharz();
					Char.myCharz().charID = msg.reader().readInt();
					Char.myCharz().ctaskId = msg.reader().readByte();
					Char.myCharz().cgender = msg.reader().readByte();
					Char.myCharz().head = msg.reader().readShort();
					Char.myCharz().cName = msg.reader().readUTF();
					Char.myCharz().cPk = msg.reader().readByte();
					Char.myCharz().cTypePk = msg.reader().readByte();
					Char.myCharz().cPower = msg.reader().readLong();
					Char.myCharz().applyCharLevelPercent();
					Char.myCharz().eff5BuffHp = msg.reader().readShort();
					Char.myCharz().eff5BuffMp = msg.reader().readShort();
					Char.myCharz().nClass = GameScr.nClasss[msg.reader().readByte()];
					Char.myCharz().vSkill.removeAllElements();
					Char.myCharz().vSkillFight.removeAllElements();
					GameScr.gI().dHP = Char.myCharz().cHP;
					GameScr.gI().dMP = Char.myCharz().cMP;
					sbyte b2 = msg.reader().readByte();
					for (sbyte b6 = 0; b6 < b2; b6++)
					{
						Skill skill3 = Skills.get(msg.reader().readShort());
						useSkill(skill3);
					}
					GameScr.gI().sortSkill();
					GameScr.gI().loadSkillShortcut();
					Char.myCharz().xu = msg.reader().readLong();
					Char.myCharz().luongKhoa = msg.reader().readInt();
					Char.myCharz().luong = msg.reader().readInt();
					Char.myCharz().xuStr = Res.formatNumber(Char.myCharz().xu);
					Char.myCharz().luongStr = mSystem.numberTostring(Char.myCharz().luong);
					Char.myCharz().luongKhoaStr = mSystem.numberTostring(Char.myCharz().luongKhoa);
					Char.myCharz().arrItemBody = new Item[msg.reader().readByte()];
					try
					{
						Char.myCharz().setDefaultPart();
						for (int k = 0; k < Char.myCharz().arrItemBody.Length; k++)
						{
							short num6 = msg.reader().readShort();
							if (num6 == -1)
							{
								continue;
							}
							ItemTemplate itemTemplate = ItemTemplates.get(num6);
							int num7 = itemTemplate.type;
							Char.myCharz().arrItemBody[k] = new Item();
							Char.myCharz().arrItemBody[k].template = itemTemplate;
							Char.myCharz().arrItemBody[k].quantity = msg.reader().readInt();
							Char.myCharz().arrItemBody[k].info = Res.changeString(msg.reader().readUTF());
							Char.myCharz().arrItemBody[k].content = Res.changeString(msg.reader().readUTF());
							int num8 = msg.reader().readUnsignedByte();
							if (num8 != 0)
							{
								Char.myCharz().arrItemBody[k].itemOption = new ItemOption[num8];
								for (int l = 0; l < Char.myCharz().arrItemBody[k].itemOption.Length; l++)
								{
									ItemOption itemOption = readItemOption(msg);
									if (itemOption != null)
									{
										Char.myCharz().arrItemBody[k].itemOption[l] = itemOption;
									}
								}
							}
							switch (num7)
							{
							case 0:
								Res.outz("toi day =======================================" + Char.myCharz().body);
								Char.myCharz().body = Char.myCharz().arrItemBody[k].template.part;
								break;
							case 1:
								Char.myCharz().leg = Char.myCharz().arrItemBody[k].template.part;
								Res.outz("toi day =======================================" + Char.myCharz().leg);
								break;
							}
						}
					}
					catch (Exception)
					{
					}
					Char.myCharz().arrItemBag = new Item[msg.reader().readByte()];
					GameScr.hpPotion = 0;
					GameScr.isudungCapsun4 = false;
					GameScr.isudungCapsun3 = false;
					for (int m = 0; m < Char.myCharz().arrItemBag.Length; m++)
					{
						short num9 = msg.reader().readShort();
						if (num9 == -1)
						{
							continue;
						}
						Char.myCharz().arrItemBag[m] = new Item();
						Char.myCharz().arrItemBag[m].template = ItemTemplates.get(num9);
						Char.myCharz().arrItemBag[m].quantity = msg.reader().readInt();
						Char.myCharz().arrItemBag[m].info = Res.changeString(msg.reader().readUTF());
						Char.myCharz().arrItemBag[m].content = Res.changeString(msg.reader().readUTF());
						Char.myCharz().arrItemBag[m].indexUI = m;
						sbyte b7 = msg.reader().readByte();
						if (b7 != 0)
						{
							Char.myCharz().arrItemBag[m].itemOption = new ItemOption[b7];
							for (int n = 0; n < Char.myCharz().arrItemBag[m].itemOption.Length; n++)
							{
								ItemOption itemOption2 = readItemOption(msg);
								if (itemOption2 != null)
								{
									Char.myCharz().arrItemBag[m].itemOption[n] = itemOption2;
									Char.myCharz().arrItemBag[m].getCompare();
								}
							}
						}
						if (Char.myCharz().arrItemBag[m].template.type == 6)
						{
							GameScr.hpPotion += Char.myCharz().arrItemBag[m].quantity;
						}
						switch (num9)
						{
						case 194:
							GameScr.isudungCapsun4 = Char.myCharz().arrItemBag[m].quantity > 0;
							break;
						case 193:
							if (!GameScr.isudungCapsun4)
							{
								GameScr.isudungCapsun3 = Char.myCharz().arrItemBag[m].quantity > 0;
							}
							break;
						}
					}
					Char.myCharz().arrItemBox = new Item[msg.reader().readByte()];
					GameCanvas.panel.hasUse = 0;
					for (int num10 = 0; num10 < Char.myCharz().arrItemBox.Length; num10++)
					{
						short num11 = msg.reader().readShort();
						if (num11 == -1)
						{
							continue;
						}
						Char.myCharz().arrItemBox[num10] = new Item();
						Char.myCharz().arrItemBox[num10].template = ItemTemplates.get(num11);
						Char.myCharz().arrItemBox[num10].quantity = msg.reader().readInt();
						Char.myCharz().arrItemBox[num10].info = Res.changeString(msg.reader().readUTF());
						Char.myCharz().arrItemBox[num10].content = Res.changeString(msg.reader().readUTF());
						Char.myCharz().arrItemBox[num10].itemOption = new ItemOption[msg.reader().readByte()];
						for (int num12 = 0; num12 < Char.myCharz().arrItemBox[num10].itemOption.Length; num12++)
						{
							ItemOption itemOption3 = readItemOption(msg);
							if (itemOption3 != null)
							{
								Char.myCharz().arrItemBox[num10].itemOption[num12] = itemOption3;
								Char.myCharz().arrItemBox[num10].getCompare();
							}
						}
						GameCanvas.panel.hasUse++;
					}
					Char.myCharz().statusMe = 4;
					int num13 = Rms.loadRMSInt(Char.myCharz().cName + "vci");
					if (num13 < 1)
					{
						GameScr.isViewClanInvite = false;
					}
					else
					{
						GameScr.isViewClanInvite = true;
					}
					short num14 = msg.reader().readShort();
					Char.idHead = new short[num14];
					Char.idAvatar = new short[num14];
					for (int num15 = 0; num15 < num14; num15++)
					{
						Char.idHead[num15] = msg.reader().readShort();
						Char.idAvatar[num15] = msg.reader().readShort();
					}
					for (int num16 = 0; num16 < GameScr.info1.charId.Length; num16++)
					{
						GameScr.info1.charId[num16] = new int[3];
					}
					GameScr.info1.charId[Char.myCharz().cgender][0] = msg.reader().readShort();
					GameScr.info1.charId[Char.myCharz().cgender][1] = msg.reader().readShort();
					GameScr.info1.charId[Char.myCharz().cgender][2] = msg.reader().readShort();
					Char.myCharz().isNhapThe = msg.reader().readByte() == 1;
					Res.outz("NHAP THE= " + Char.myCharz().isNhapThe);
					GameScr.deltaTime = mSystem.currentTimeMillis() - (long)msg.reader().readInt() * 1000L;
					GameScr.isNewMember = msg.reader().readByte();
					Service.gI().updateCaption((sbyte)Char.myCharz().cgender);
					Service.gI().androidPack();
					try
					{
						Char.myCharz().idAuraEff = msg.reader().readShort();
						Char.myCharz().idEff_Set_Item = msg.reader().readSByte();
						Char.myCharz().idHat = msg.reader().readShort();
						break;
					}
					catch (Exception)
					{
						break;
					}
				}
				case 4:
					GameCanvas.debug("SA23", 2);
					Char.myCharz().xu = msg.reader().readLong();
					Char.myCharz().luong = msg.reader().readInt();
					Char.myCharz().cHP = msg.reader().readLong();
					Char.myCharz().cMP = msg.reader().readLong();
					Char.myCharz().luongKhoa = msg.reader().readInt();
					Char.myCharz().xuStr = Res.formatNumber2(Char.myCharz().xu);
					Char.myCharz().luongStr = mSystem.numberTostring(Char.myCharz().luong);
					Char.myCharz().luongKhoaStr = mSystem.numberTostring(Char.myCharz().luongKhoa);
					break;
				case 5:
				{
					GameCanvas.debug("SA24", 2);
					long cHP = Char.myCharz().cHP;
					Char.myCharz().cHP = msg.reader().readLong();
					if (Char.myCharz().cHP > cHP && Char.myCharz().cTypePk != 4)
					{
						GameScr.startFlyText("+" + (Char.myCharz().cHP - cHP) + " " + mResources.HP, Char.myCharz().cx, Char.myCharz().cy - Char.myCharz().ch - 20, 0, -1, mFont.HP);
						SoundMn.gI().HP_MPup();
						if (Char.myCharz().petFollow != null && Char.myCharz().petFollow.smallID == 5003)
						{
							MonsterDart.addMonsterDart(Char.myCharz().petFollow.cmx + ((Char.myCharz().petFollow.dir != 1) ? (-10) : 10), Char.myCharz().petFollow.cmy + 10, isBoss: true, -1L, -1L, Char.myCharz(), 29);
						}
					}
					if (Char.myCharz().cHP < cHP)
					{
						GameScr.startFlyText("-" + (cHP - Char.myCharz().cHP) + " " + mResources.HP, Char.myCharz().cx, Char.myCharz().cy - Char.myCharz().ch - 20, 0, -1, mFont.HP);
					}
					GameScr.gI().dHP = Char.myCharz().cHP;
					if (GameScr.isPaintInfoMe)
					{
					}
					break;
				}
				case 6:
				{
					GameCanvas.debug("SA25", 2);
					if (Char.myCharz().statusMe == 14 || Char.myCharz().statusMe == 5)
					{
						break;
					}
					long cMP = Char.myCharz().cMP;
					Char.myCharz().cMP = msg.reader().readLong();
					if (Char.myCharz().cMP > cMP)
					{
						GameScr.startFlyText("+" + (Char.myCharz().cMP - cMP) + " " + mResources.KI, Char.myCharz().cx, Char.myCharz().cy - Char.myCharz().ch - 23, 0, -2, mFont.MP);
						SoundMn.gI().HP_MPup();
						if (Char.myCharz().petFollow != null && Char.myCharz().petFollow.smallID == 5001)
						{
							MonsterDart.addMonsterDart(Char.myCharz().petFollow.cmx + ((Char.myCharz().petFollow.dir != 1) ? (-10) : 10), Char.myCharz().petFollow.cmy + 10, isBoss: true, -1L, -1L, Char.myCharz(), 29);
						}
					}
					if (Char.myCharz().cMP < cMP)
					{
						GameScr.startFlyText("-" + (cMP - Char.myCharz().cMP) + " " + mResources.KI, Char.myCharz().cx, Char.myCharz().cy - Char.myCharz().ch - 23, 0, -2, mFont.MP);
					}
					Res.outz("curr MP= " + Char.myCharz().cMP);
					GameScr.gI().dMP = Char.myCharz().cMP;
					if (GameScr.isPaintInfoMe)
					{
					}
					break;
				}
				case 7:
				{
					Char @char = GameScr.findCharInMap(msg.reader().readInt());
					if (@char != null)
					{
						@char.clanID = msg.reader().readInt();
						if (@char.clanID == -2)
						{
							@char.isCopy = true;
						}
						readCharInfo(@char, msg);
						try
						{
							@char.idAuraEff = msg.reader().readShort();
							@char.idEff_Set_Item = msg.reader().readSByte();
							@char.idHat = msg.reader().readShort();
							Effect.GetCharEff(@char);
							break;
						}
						catch (Exception)
						{
							break;
						}
					}
					break;
				}
				case 8:
				{
					GameCanvas.debug("SA26", 2);
					Char @char = GameScr.findCharInMap(msg.reader().readInt());
					if (@char != null)
					{
						@char.cspeed = msg.reader().readByte();
					}
					break;
				}
				case 9:
				{
					GameCanvas.debug("SA27", 2);
					Char @char = GameScr.findCharInMap(msg.reader().readInt());
					if (@char != null)
					{
						@char.cHP = msg.reader().readLong();
						@char.cHPFull = msg.reader().readLong();
					}
					break;
				}
				case 10:
				{
					GameCanvas.debug("SA28", 2);
					Char @char = GameScr.findCharInMap(msg.reader().readInt());
					if (@char != null)
					{
						@char.cHP = msg.reader().readLong();
						@char.cHPFull = msg.reader().readLong();
						@char.eff5BuffHp = msg.reader().readShort();
						@char.eff5BuffMp = msg.reader().readShort();
						@char.wp = msg.reader().readShort();
						if (@char.wp == -1)
						{
							@char.setDefaultWeapon();
						}
					}
					break;
				}
				case 11:
				{
					GameCanvas.debug("SA29", 2);
					Char @char = GameScr.findCharInMap(msg.reader().readInt());
					if (@char != null)
					{
						@char.cHP = msg.reader().readLong();
						@char.cHPFull = msg.reader().readLong();
						@char.eff5BuffHp = msg.reader().readShort();
						@char.eff5BuffMp = msg.reader().readShort();
						@char.body = msg.reader().readShort();
						if (@char.body == -1)
						{
							@char.setDefaultBody();
						}
					}
					break;
				}
				case 12:
				{
					GameCanvas.debug("SA30", 2);
					Char @char = GameScr.findCharInMap(msg.reader().readInt());
					if (@char != null)
					{
						@char.cHP = msg.reader().readLong();
						@char.cHPFull = msg.reader().readLong();
						@char.eff5BuffHp = msg.reader().readShort();
						@char.eff5BuffMp = msg.reader().readShort();
						@char.leg = msg.reader().readShort();
						if (@char.leg == -1)
						{
							@char.setDefaultLeg();
						}
					}
					break;
				}
				case 13:
				{
					GameCanvas.debug("SA31", 2);
					int num2 = msg.reader().readInt();
					Char @char = ((num2 != Char.myCharz().charID) ? GameScr.findCharInMap(num2) : Char.myCharz());
					if (@char != null)
					{
						@char.cHP = msg.reader().readLong();
						@char.cHPFull = msg.reader().readLong();
						@char.eff5BuffHp = msg.reader().readShort();
						@char.eff5BuffMp = msg.reader().readShort();
					}
					break;
				}
				case 14:
				{
					GameCanvas.debug("SA32", 2);
					Char @char = GameScr.findCharInMap(msg.reader().readInt());
					if (@char == null)
					{
						break;
					}
					@char.cHP = msg.reader().readLong();
					sbyte b4 = msg.reader().readByte();
					Res.outz("player load hp type= " + b4);
					if (b4 == 1)
					{
						ServerEffect.addServerEffect(11, @char, 5);
						ServerEffect.addServerEffect(104, @char, 4);
					}
					if (b4 == 2)
					{
						@char.doInjure();
					}
					try
					{
						@char.cHPFull = msg.reader().readLong();
						break;
					}
					catch (Exception)
					{
						break;
					}
				}
				case 15:
				{
					GameCanvas.debug("SA33", 2);
					Char @char = GameScr.findCharInMap(msg.reader().readInt());
					if (@char != null)
					{
						@char.cHP = msg.reader().readLong();
						@char.cHPFull = msg.reader().readLong();
						@char.cx = msg.reader().readShort();
						@char.cy = msg.reader().readShort();
						@char.statusMe = 1;
						@char.cp3 = 3;
						ServerEffect.addServerEffect(109, @char, 2);
					}
					break;
				}
				case 35:
				{
					GameCanvas.debug("SY3", 2);
					int num4 = msg.reader().readInt();
					Res.outz("CID = " + num4);
					if (TileMap.mapID == 130)
					{
						GameScr.gI().starVS();
					}
					if (num4 == Char.myCharz().charID)
					{
						Char.myCharz().cTypePk = msg.reader().readByte();
						if (GameScr.gI().isVS() && Char.myCharz().cTypePk != 0)
						{
							GameScr.gI().starVS();
						}
						Res.outz("type pk= " + Char.myCharz().cTypePk);
						Char.myCharz().npcFocus = null;
						if (!GameScr.gI().isMeCanAttackMob(Char.myCharz().mobFocus))
						{
							Char.myCharz().mobFocus = null;
						}
						Char.myCharz().itemFocus = null;
					}
					else
					{
						Char @char = GameScr.findCharInMap(num4);
						if (@char != null)
						{
							Res.outz("type pk= " + @char.cTypePk);
							@char.cTypePk = msg.reader().readByte();
							if (@char.isAttacPlayerStatus())
							{
								Char.myCharz().charFocus = @char;
							}
						}
					}
					for (int i = 0; i < GameScr.vCharInMap.size(); i++)
					{
						Char char2 = GameScr.findCharInMap(i);
						if (char2 != null && char2.cTypePk != 0 && char2.cTypePk == Char.myCharz().cTypePk)
						{
							if (!Char.myCharz().mobFocus.isMobMe)
							{
								Char.myCharz().mobFocus = null;
							}
							Char.myCharz().npcFocus = null;
							Char.myCharz().itemFocus = null;
							break;
						}
					}
					Res.outz("update type pk= ");
					break;
				}
				case 61:
				{
					string text = msg.reader().readUTF();
					sbyte[] data = new sbyte[msg.reader().readInt()];
					msg.reader().read(ref data);
					if (data.Length == 0)
					{
						data = null;
					}
					if (text.Equals("KSkill"))
					{
						GameScr.gI().onKSkill(data);
					}
					else if (text.Equals("OSkill"))
					{
						GameScr.gI().onOSkill(data);
					}
					else if (text.Equals("CSkill"))
					{
						GameScr.gI().onCSkill(data);
					}
					break;
				}
				case 23:
				{
					short num = msg.reader().readShort();
					Skill skill = Skills.get(num);
					useSkill(skill);
					if (num != 0 && num != 14 && num != 28)
					{
						GameScr.info1.addInfo(mResources.LEARN_SKILL + " " + skill.template.name, 0);
					}
					break;
				}
				case 62:
					Res.outz("ME UPDATE SKILL");
					read_UpdateSkill(msg);
					break;
				}
			}
			catch (Exception ex5)
			{
				Cout.println("Loi tai Sub : " + ex5.ToString());
			}
			finally
			{
				msg?.cleanup();
			}
		}

	public void read_cmdExtra(Message msg)
		{
			try
			{
				sbyte b = msg.reader().readByte();
				mSystem.println(">>---read_cmdExtra-sub:" + b);
				if (b == 0)
				{
					short idHat = msg.reader().readShort();
					Char.myCharz().idHat = idHat;
					SoundMn.gI().getStrOption();
				}
				else if (b == 2)
				{
					int num = msg.reader().readInt();
					sbyte b2 = msg.reader().readByte();
					short num2 = msg.reader().readShort();
					string v = num2 + "," + b2;
					MainImage imagePath = ImgByName.getImagePath("banner_" + num2, ImgByName.hashImagePath);
					GameCanvas.danhHieu.put(num + string.Empty, v);
				}
				else if (b == 3)
				{
					short num3 = msg.reader().readShort();
					SmallImage.createImage(num3);
					BackgroudEffect.id_water1 = num3;
				}
				else if (b == 4)
				{
					string o = msg.reader().readUTF();
					GameCanvas.messageServer.addElement(o);
				}
				else if (b == 5)
				{
					string text = "------------------|ChienTruong|Log: ";
					text = "\n|ChienTruong|Log: ";
					sbyte b3 = msg.reader().readByte();
					if (b3 == 0)
					{
						GameScr.nCT_team = msg.reader().readUTF();
						GameScr.nCT_TeamA = (GameScr.nCT_TeamB = msg.reader().readByte());
						GameScr.nCT_nBoyBaller = GameScr.nCT_TeamA * 2;
						GameScr.isPaint_CT = false;
						string text2 = text;
						text = text2 + "\tsub    0|  nCT_team= " + GameScr.nCT_team + "|nCT_TeamA =" + GameScr.nCT_TeamA + "  isPaint_CT=false \n";
					}
					else if (b3 == 1)
					{
						int num4 = msg.reader().readInt();
						sbyte b4 = (GameScr.nCT_floor = msg.reader().readByte());
						GameScr.nCT_timeBallte = num4 * 1000 + mSystem.currentTimeMillis();
						GameScr.isPaint_CT = true;
						string text2 = text;
						text = text2 + "\tsub    1 floor= " + b4 + "|timeBallte= " + num4 + "isPaint_CT=true \n";
					}
					else if (b3 == 2)
					{
						GameScr.nCT_TeamA = msg.reader().readByte();
						GameScr.nCT_TeamB = msg.reader().readByte();
						GameScr.res_CT.removeAllElements();
						sbyte b5 = msg.reader().readByte();
						for (int i = 0; i < b5; i++)
						{
							string empty = string.Empty;
							empty = empty + msg.reader().readByte() + "|";
							empty = empty + msg.reader().readUTF() + "|";
							empty = empty + msg.reader().readShort() + "|";
							empty += msg.reader().readInt();
							GameScr.res_CT.addElement(empty);
						}
						string text2 = text;
						text = text2 + "\tsub   2|  A= " + GameScr.nCT_TeamA + "|B =" + GameScr.nCT_TeamB + "  isPaint_CT=true \n";
					}
					else if (b3 == 3)
					{
						Service.gI().sendCT_ready(b, b3);
						GameScr.nCT_floor = 0;
						GameScr.nCT_timeBallte = 0L;
						GameScr.isPaint_CT = false;
						text += "\tsub    3|  isPaint_CT=false \n";
					}
					else if (b3 == 4)
					{
						GameScr.nUSER_CT = msg.reader().readByte();
						GameScr.nUSER_MAX_CT = msg.reader().readByte();
					}
					text += "END LOG CT.";
					Res.err(text);
				}
				else
				{
					readExtra(b, msg);
				}
			}
			catch (Exception)
			{
			}
		}

	public void readExtra(sbyte sub, Message msg)
		{
			try
			{
				if (sub != sbyte.MaxValue)
				{
					return;
				}
				GameCanvas.endDlg();
				try
				{
					string text = (ServerListScreen.linkDefault = msg.reader().readUTF());
					mSystem.AddIpTest();
					ServerListScreen.getServerList(ServerListScreen.linkDefault);
					Res.outz(">>>>read.isEXTRA_LINK " + text);
					sbyte b = msg.reader().readByte();
					if (b > 0)
					{
						ServerListScreen.typeClass = new sbyte[b];
						ServerListScreen.listChar = new Char[b];
						for (int i = 0; i < b; i++)
						{
							ServerListScreen.typeClass[i] = msg.reader().readByte();
							Res.outz(ServerListScreen.nameServer[i] + ">>>>read.isEXTRA_LINK  typeClass: " + ServerListScreen.typeClass[i]);
							if (ServerListScreen.typeClass[i] > -1)
							{
								ServerListScreen.isHaveChar = true;
								ServerListScreen.listChar[i] = new Char();
								ServerListScreen.listChar[i].cgender = ServerListScreen.typeClass[i];
								ServerListScreen.listChar[i].head = msg.reader().readShort();
								ServerListScreen.listChar[i].body = msg.reader().readShort();
								ServerListScreen.listChar[i].leg = msg.reader().readShort();
								ServerListScreen.listChar[i].bag = msg.reader().readShort();
								ServerListScreen.listChar[i].cName = msg.reader().readUTF();
							}
						}
					}
				}
				catch (Exception)
				{
				}
				isEXTRA_LINK = true;
				ServerListScreen.saveRMS_ExtraLink();
				ServerListScreen.isWait = false;
				Char.isLoadingMap = false;
				LoginScr.isContinueToLogin = false;
				ServerListScreen.waitToLogin = false;
				bool flag = false;
				bool flag2 = false;
				try
				{
					if (!Rms.loadRMSString(Rms.RMS_acc).Equals(string.Empty))
					{
						flag = true;
					}
					if (!Rms.loadRMSString(Rms.RMS_userAo + ServerListScreen.ipSelect).Equals(string.Empty))
					{
						flag2 = true;
					}
				}
				catch (Exception)
				{
				}
				if (!ServerListScreen.isHaveChar && !flag && !flag2)
				{
					GameCanvas.serverScreen.Login_New();
					return;
				}
				int svSel = Rms.loadRMSInt(ServerListScreen.RMS_svselect);
				if (svSel == -1)
				{
					ServerScr.isShowSv_HaveChar = false;
					if (GameCanvas.serverScr == null)
					{
						GameCanvas.serverScr = new ServerScr();
					}
					GameCanvas.serverScr.switchToMe();
					return;
				}
				ServerListScreen.SetIpSelect(svSel, issave: false);
				if (ServerListScreen.listChar != null && ServerListScreen.ipSelect >= 0 && ServerListScreen.ipSelect < ServerListScreen.listChar.Length && ServerListScreen.listChar[ServerListScreen.ipSelect] != null)
				{
					if (GameCanvas._SelectCharScr == null)
					{
						GameCanvas._SelectCharScr = new SelectCharScr();
					}
					GameCanvas._SelectCharScr.SetInfoChar(ServerListScreen.listChar[ServerListScreen.ipSelect]);
				}
				else
				{
					if (GameCanvas.serverScreen == null)
					{
						GameCanvas.serverScreen = new ServerListScreen();
					}
					GameCanvas.serverScreen.Login_New();
				}
			}
			catch (Exception)
			{
				Res.outz(">>>>read.isEXTRA_LINK  errr:");
				if (GameCanvas.serverScr == null)
				{
					GameCanvas.serverScr = new ServerScr();
				}
				GameCanvas.serverScr.switchToMe();
			}
		}

	public void read_cmdExtraBig(Message msg)
		{
			try
			{
				sbyte b = msg.reader().readByte();
				mSystem.println(">>---read_cmdExtraBig-sub:" + b);
				if (b == 0)
				{
					loadItemNew(msg.reader(), 1, isSave: true);
				}
			}
			catch (Exception)
			{
			}
		}

}
