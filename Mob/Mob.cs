using System;
using Assets.src.g;

public partial class Mob
{
	public const sbyte TYPE_DUNG = 0;

	public const sbyte TYPE_DI = 1;

	public const sbyte TYPE_NHAY = 2;

	public const sbyte TYPE_LET = 3;

	public const sbyte TYPE_BAY = 4;

	public const sbyte TYPE_BAY_DAU = 5;

	public static MobTemplate[] arrMobTemplate;

	public const sbyte MA_INHELL = 0;

	public const sbyte MA_DEADFLY = 1;

	public const sbyte MA_STANDWAIT = 2;

	public const sbyte MA_ATTACK = 3;

	public const sbyte MA_STANDFLY = 4;

	public const sbyte MA_WALK = 5;

	public const sbyte MA_FALL = 6;

	public const sbyte MA_INJURE = 7;

	public bool changBody;

	public short smallBody;

	public bool isHintFocus;

	public string flystring;

	public int flyx;

	public int flyy;

	public int flyIndex;

	public bool isFreez;

	public int seconds;

	public long last;

	public long cur;

	public int holdEffID;

	public long hp;

	public long maxHp;

	public long hpInjure;

	public int x;

	public int y;

	public int dir = 1;

	public int dirV = 1;

	public int status;

	public int p1;

	public int p2;

	public int p3;

	public int xFirst;

	public int yFirst;

	public int vy;

	public int exp;

	public int w;

	public int h;

	public int charIndex;

	public int timeStatus;

	public int mobId;

	public bool isx;

	public bool isy;

	public bool isDisable;

	public bool isDontMove;

	public bool isFire;

	public bool isIce;

	public bool isWind;

	public bool isDie;

	public MyVector vMobMove = new MyVector();

	public bool isGo;

	public string mobName;

	public int templateId;

	public short pointx;

	public short pointy;

	public Char cFocus;

	public long dame;

	public long dameMp;

	public int sys;

	public sbyte levelBoss;

	public sbyte level;

	public bool isBoss;

	public bool isMobMe;

	public static MyVector lastMob = new MyVector();

	public static MyVector newMob = new MyVector();

	public bool isMafuba;

	public int xMFB;

	public int yMFB;

	public int xSd;

	public int ySd;

	private bool isOutMap;

	private int wCount;

	public bool isShadown = true;

	private int tick;

	private int frame;

	public static Image imgHP = GameCanvas.loadImage("/mainImage/myTexture2dmobHP.png");

	private bool wy;

	private int wt;

	private int fy;

	private int ty;

	public int typeSuperEff;

	public bool isBusyAttackSomeOne = true;

	public int[] stand = new int[12]
			{
				0, 0, 0, 0, 0, 0, 0, 0, 1, 1,
				1, 1
			};

	public int[] move = new int[15]
			{
				1, 1, 1, 1, 2, 2, 2, 2, 3, 3,
				3, 3, 2, 2, 2
			};

	public int[] moveFast = new int[7] { 1, 1, 2, 2, 3, 3, 2 };

	public int[] attack1 = new int[3] { 4, 5, 6 };

	public int[] attack2 = new int[3] { 7, 8, 9 };

	public int[] hurt = new int[1];

	private int color = 8421504;

	public int len = 24;

	public int w_hp_bar = 24;

	public int per = 100;

	public int per_tem = 100;

	public byte h_hp_bar = 4;

	public Image imgHPtem;

	private int offset;

	public bool isHide;

	private sbyte[] cou = new sbyte[2] { -1, 1 };

	public Char injureBy;

	public bool injureThenDie;

	public Mob mobToAttack;

	public int forceWait;

	public bool blindEff;

	public bool sleepEff;

	private int[][] frameArr = new int[6][]
			{
				new int[8] { 0, 0, 0, 0, 1, 1, 1, 1 },
				new int[8] { 0, 0, 0, 0, 1, 1, 1, 1 },
				new int[8] { 0, 0, 0, 0, 1, 1, 1, 1 },
				new int[8] { 0, 0, 0, 0, 1, 1, 1, 1 },
				new int[8] { 0, 0, 0, 0, 1, 1, 1, 1 },
				new int[8] { 0, 0, 0, 0, 1, 1, 1, 1 }
			};

	private bool isGetFr = true;

	public Mob()
			{
			}

	public Mob(int mobId, bool isDisable, bool isDontMove, bool isFire, bool isIce, bool isWind, int templateId, int sys, long hp, sbyte level, long maxp, short pointx, short pointy, sbyte status, sbyte levelBoss)
			{
				this.isDisable = isDisable;
				this.isDontMove = isDontMove;
				this.isFire = isFire;
				this.isIce = isIce;
				this.isWind = isWind;
				this.sys = sys;
				this.mobId = mobId;
				this.templateId = templateId;
				this.hp = hp;
				this.level = level;
				xFirst = (x = (this.pointx = pointx));
				yFirst = (y = (this.pointy = pointy));
				this.status = status;
				if (templateId != 70)
				{
					checkData();
					getData();
				}
				if (!isExistNewMob(templateId + string.Empty))
				{
					newMob.addElement(templateId + string.Empty);
				}
				maxHp = maxp;
				this.levelBoss = levelBoss;
				updateHp_bar();
				isDie = false;
				xSd = pointx;
				ySd = pointy;
				if (isNewModStand())
				{
					stand = new int[17]
					{
						0, 0, 0, 0, 0, 1, 1, 1, 1, 1,
						2, 2, 2, 2, 2, 2, 2
					};
					move = new int[17]
					{
						0, 0, 0, 0, 0, 1, 1, 1, 1, 1,
						2, 2, 2, 2, 2, 2, 2
					};
					moveFast = new int[17]
					{
						0, 0, 0, 0, 0, 1, 1, 1, 1, 1,
						2, 2, 2, 2, 2, 2, 2
					};
					attack1 = new int[12]
					{
						3, 3, 3, 3, 4, 4, 4, 4, 5, 5,
						5, 5
					};
					attack2 = new int[12]
					{
						3, 3, 3, 3, 4, 4, 4, 4, 5, 5,
						5, 5
					};
				}
				else if (isNewMod())
				{
					stand = new int[12]
					{
						0, 0, 0, 0, 0, 0, 0, 0, 1, 1,
						1, 1
					};
					move = new int[16]
					{
						1, 1, 1, 1, 2, 2, 2, 2, 1, 1,
						1, 1, 3, 3, 3, 3
					};
					moveFast = new int[8] { 1, 1, 2, 2, 1, 1, 3, 3 };
					attack1 = new int[11]
					{
						4, 4, 4, 5, 5, 5, 6, 6, 6, 6,
						6
					};
					attack2 = new int[11]
					{
						7, 7, 7, 8, 8, 8, 9, 9, 9, 9,
						9
					};
				}
				else if (isSpecial())
				{
					stand = new int[12]
					{
						0, 0, 0, 0, 0, 0, 0, 0, 1, 1,
						1, 1
					};
					move = new int[16]
					{
						2, 2, 3, 3, 2, 2, 4, 4, 2, 2,
						3, 3, 2, 2, 4, 4
					};
					moveFast = new int[8] { 2, 2, 3, 3, 2, 2, 4, 4 };
					attack1 = new int[8] { 5, 6, 7, 8, 9, 10, 11, 12 };
					attack2 = new int[4] { 5, 12, 13, 14 };
				}
				else
				{
					stand = new int[12]
					{
						0, 0, 0, 0, 0, 0, 0, 0, 1, 1,
						1, 1
					};
					move = new int[15]
					{
						1, 1, 1, 1, 2, 2, 2, 2, 3, 3,
						3, 3, 2, 2, 2
					};
					moveFast = new int[7] { 1, 1, 2, 2, 3, 3, 2 };
					attack1 = new int[3] { 4, 5, 6 };
					attack2 = new int[3] { 7, 8, 9 };
				}
			}

	public void getData()
			{
				if (arrMobTemplate[templateId].data == null)
				{
					arrMobTemplate[templateId].data = new EffectData();
					string text = "/Mob/" + templateId;
					DataInputStream dataInputStream = MyStream.readFile(text);
					if (dataInputStream != null)
					{
						arrMobTemplate[templateId].data.readData(text + "/data");
						arrMobTemplate[templateId].data.img = GameCanvas.loadImage(text + "/img.png");
					}
					else
					{
						Service.gI().requestModTemplate(templateId);
					}
					if (lastMob.size() > 15)
					{
						arrMobTemplate[int.Parse((string)lastMob.elementAt(0))].data = null;
						lastMob.removeElementAt(0);
					}
					lastMob.addElement(templateId + string.Empty);
				}
				else
				{
					w = arrMobTemplate[templateId].data.width;
					h = arrMobTemplate[templateId].data.height;
				}
			}

	public virtual void setBody(short id)
			{
				changBody = true;
				smallBody = id;
			}

	public virtual void clearBody()
			{
				changBody = false;
			}

	public static bool isExistNewMob(string id)
			{
				for (int i = 0; i < newMob.size(); i++)
				{
					string text = (string)newMob.elementAt(i);
					if (text.Equals(id))
					{
						return true;
					}
				}
				return false;
			}

	public void checkData()
			{
				int num = 0;
				for (int i = 0; i < arrMobTemplate.Length; i++)
				{
					if (arrMobTemplate[i].data != null)
					{
						num++;
					}
				}
				if (num < 10)
				{
					return;
				}
				for (int j = 0; j < arrMobTemplate.Length; j++)
				{
					if (arrMobTemplate[j].data != null && num > 5)
					{
						arrMobTemplate[j].data = null;
					}
				}
			}

	public void checkFrameTick(int[] array)
			{
				if (tick > array.Length - 1)
				{
					tick = 0;
				}
				frame = array[tick];
				tick++;
			}

	private void updateShadown()
			{
				int num = TileMap.size;
				xSd = x;
				wCount = 0;
				if (ySd <= 0 || TileMap.tileTypeAt(xSd, ySd, 2))
				{
					return;
				}
				if (TileMap.tileTypeAt(xSd / num, ySd / num) == 0)
				{
					isOutMap = true;
				}
				else if (TileMap.tileTypeAt(xSd / num, ySd / num) != 0 && !TileMap.tileTypeAt(xSd, ySd, 2))
				{
					xSd = x;
					ySd = y;
					isOutMap = false;
				}
				while (isOutMap && wCount < 10)
				{
					wCount++;
					ySd += 24;
					if (TileMap.tileTypeAt(xSd, ySd, 2))
					{
						if (ySd % 24 != 0)
						{
							ySd -= ySd % 24;
						}
						break;
					}
				}
			}

	private void paintShadow(mGraphics g)
			{
				int num = TileMap.size;
				if (TileMap.tileTypeAt(xSd + num / 2, ySd + 1, 4))
				{
					g.setClip(xSd / num * num, (ySd - 30) / num * num, num, 100);
				}
				else if (TileMap.tileTypeAt((xSd - num / 2) / num, (ySd + 1) / num) == 0)
				{
					g.setClip(xSd / num * num, (ySd - 30) / num * num, 100, 100);
				}
				else if (TileMap.tileTypeAt((xSd + num / 2) / num, (ySd + 1) / num) == 0)
				{
					g.setClip(xSd / num * num, (ySd - 30) / num * num, num, 100);
				}
				else if (TileMap.tileTypeAt(xSd - num / 2, ySd + 1, 8))
				{
					g.setClip(xSd / 24 * num, (ySd - 30) / num * num, num, 100);
				}
				g.drawImage(TileMap.bong, xSd, ySd, 3);
				g.setClip(GameScr.cmx, GameScr.cmy - GameCanvas.transY, GameScr.gW, GameScr.gH + 2 * GameCanvas.transY);
			}

	public virtual void update()
			{
				if (isMafuba)
				{
					return;
				}
				GetFrame();
				if (blindEff && GameCanvas.gameTick % 5 == 0)
				{
					ServerEffect.addServerEffect(113, x, y, 1);
				}
				if (sleepEff && GameCanvas.gameTick % 10 == 0)
				{
					EffecMn.addEff(new Effect(41, x, y, 3, 1, 1));
				}
				if (!GameCanvas.lowGraphic && status != 1 && status != 0 && !GameCanvas.lowGraphic && GameCanvas.gameTick % (15 + mobId * 2) == 0)
				{
					for (int i = 0; i < GameScr.vCharInMap.size(); i++)
					{
						Char @char = (Char)GameScr.vCharInMap.elementAt(i);
						if (@char != null && @char.isFlyAndCharge && @char.cf == 32)
						{
							Char char2 = new Char();
							char2.cx = @char.cx;
							char2.cy = @char.cy - @char.ch;
							if (@char.cgender == 0)
							{
								MonsterDart.addMonsterDart(x + dir * w, y, checkIsBoss(), -100L, -100L, char2, 25);
							}
						}
					}
					if (Char.myCharz().isFlyAndCharge && Char.myCharz().cf == 32)
					{
						Char char3 = new Char();
						char3.cx = Char.myCharz().cx;
						char3.cy = Char.myCharz().cy - Char.myCharz().ch;
						if (Char.myCharz().cgender == 0)
						{
							MonsterDart.addMonsterDart(x + dir * w, y, checkIsBoss(), -100L, -100L, char3, 25);
						}
					}
				}
				if (holdEffID != 0 && GameCanvas.gameTick % 5 == 0)
				{
					EffecMn.addEff(new Effect(holdEffID, x, y + 24, 3, 5, 1));
				}
				if (isFreez)
				{
					if (GameCanvas.gameTick % 5 == 0)
					{
						ServerEffect.addServerEffect(113, x, y, 1);
					}
					long num = mSystem.currentTimeMillis();
					if (num - last >= 1000)
					{
						seconds--;
						last = num;
						if (seconds < 0)
						{
							isFreez = false;
							seconds = 0;
						}
					}
					if (isTypeNewMod())
					{
						frame = hurt[GameCanvas.gameTick % hurt.Length];
					}
					else if (isNewModStand())
					{
						frame = attack1[GameCanvas.gameTick % attack1.Length];
					}
					else if (isNewMod())
					{
						if (GameCanvas.gameTick % 20 > 5)
						{
							frame = 11;
						}
						else
						{
							frame = 10;
						}
					}
					else if (isSpecial())
					{
						if (GameCanvas.gameTick % 20 > 5)
						{
							frame = 1;
						}
						else
						{
							frame = 15;
						}
					}
					else if (GameCanvas.gameTick % 20 > 5)
					{
						frame = 11;
					}
					else
					{
						frame = 10;
					}
				}
				if (!isUpdate())
				{
					return;
				}
				if (isShadown)
				{
					updateShadown();
				}
				if (vMobMove == null && arrMobTemplate[templateId].rangeMove != 0)
				{
					return;
				}
				if (status != 3 && isBusyAttackSomeOne)
				{
					if (cFocus != null)
					{
						cFocus.doInjure(dame, dameMp, isCrit: false, isMob: true);
					}
					else if (mobToAttack != null)
					{
						mobToAttack.setInjure();
					}
					isBusyAttackSomeOne = false;
				}
				if (levelBoss > 0)
				{
					updateSuperEff();
				}
				if (status != 0 && status != 1 && !isMobMe)
				{
					if (hp <= 0)
					{
						status = 0;
					}
					else if (arrMobTemplate != null && templateId < arrMobTemplate.Length && arrMobTemplate[templateId] != null)
					{
						int maxRange = arrMobTemplate[templateId].rangeMove;
						if (maxRange > 0 && Res.abs(x - xFirst) > maxRange + 15)
						{
							x = xFirst;
						}
						if (arrMobTemplate[templateId].type != 4 && arrMobTemplate[templateId].type != 5)
						{
							if (yFirst > 0 && Res.abs(y - yFirst) > 25)
							{
								y = yFirst;
							}
						}
					}
				}
				switch (status)
				{
				case 1:
					isDisable = false;
					isDontMove = false;
					isFire = false;
					isIce = false;
					isWind = false;
					y += p1;
					if (GameCanvas.gameTick % 2 == 0)
					{
						if (p2 > 1)
						{
							p2--;
						}
						else if (p2 < -1)
						{
							p2++;
						}
					}
					x += p2;
					if (isTypeNewMod())
					{
						frame = hurt[GameCanvas.gameTick % hurt.Length];
					}
					else if (isNewModStand())
					{
						frame = attack1[GameCanvas.gameTick % attack1.Length];
					}
					else if (isNewMod())
					{
						frame = 11;
					}
					else if (isSpecial())
					{
						frame = 15;
					}
					else
					{
						frame = 11;
					}
					if (isDie)
					{
						isDie = false;
						if (isMobMe)
						{
							for (int j = 0; j < GameScr.vMob.size(); j++)
							{
								if (((Mob)GameScr.vMob.elementAt(j)).mobId == mobId)
								{
									GameScr.vMob.removeElementAt(j);
								}
							}
						}
						p1 = 0;
						p2 = 0;
						x = (y = 0);
						hp = getTemplate().hp;
						status = 0;
						timeStatus = 0;
						break;
					}
					if ((TileMap.tileTypeAtPixel(x, y) & 2) == 2)
					{
						p1 = ((p1 <= 4) ? (-p1) : (-4));
						if (p3 == 0)
						{
							p3 = 16;
						}
					}
					else
					{
						p1++;
					}
					if (p3 > 0)
					{
						p3--;
						if (p3 == 0)
						{
							isDie = true;
						}
					}
					break;
				case 2:
					if (holdEffID == 0 && !isFreez && !blindEff && !sleepEff)
					{
						timeStatus = 0;
						updateMobStandWait();
					}
					break;
				case 4:
					if (holdEffID == 0 && !blindEff && !sleepEff && !isFreez)
					{
						timeStatus = 0;
						p1++;
						if (p1 > 40 + mobId % 5)
						{
							y -= 2;
							status = 5;
							p1 = 0;
						}
					}
					break;
				case 3:
					if (holdEffID == 0 && !blindEff && !sleepEff && !isFreez)
					{
						updateMobAttack();
					}
					break;
				case 5:
					if (holdEffID != 0 || blindEff || sleepEff)
					{
						break;
					}
					if (isFreez)
					{
						if (arrMobTemplate[templateId].type == 4)
						{
							ty++;
							wt++;
							fy += ((!wy) ? 1 : (-1));
							if (wt == 10)
							{
								wt = 0;
								wy = !wy;
							}
						}
					}
					else
					{
						timeStatus = 0;
						updateMobWalk();
					}
					break;
				case 6:
					timeStatus = 0;
					p1++;
					y += p1;
					if (y >= yFirst)
					{
						y = yFirst;
						p1 = 0;
						status = 5;
					}
					break;
				case 7:
					updateInjure();
					break;
				}
			}

	public MobTemplate getTemplate()
			{
				return arrMobTemplate[templateId];
			}

	public bool checkIsBoss()
			{
				if (isBoss || levelBoss > 0)
				{
					return true;
				}
				return false;
			}

	public virtual void paint(mGraphics g)
			{
				if (isHide)
				{
					return;
				}
				if (isMafuba)
				{
					if (!changBody)
					{
						arrMobTemplate[templateId].data.paintFrame(g, frame, xMFB, yMFB, (dir != 1) ? 1 : 0, 2);
					}
					else
					{
						SmallImage.drawSmallImage(g, smallBody, xMFB, yMFB, (dir != 1) ? 2 : 0, mGraphics.BOTTOM | mGraphics.HCENTER);
					}
					return;
				}
				if (isShadown && status != 0)
				{
					paintShadow(g);
				}
				if (!isPaint() || (status == 1 && p3 > 0 && GameCanvas.gameTick % 3 == 0))
				{
					return;
				}
				g.translate(0, GameCanvas.transY);
				if (!changBody)
				{
					arrMobTemplate[templateId].data.paintFrame(g, frame, x, y + fy, (dir != 1) ? 1 : 0, 2);
				}
				else
				{
					SmallImage.drawSmallImage(g, smallBody, x, y + fy - 9, (dir != 1) ? 2 : 0, mGraphics.BOTTOM | mGraphics.HCENTER);
				}
				g.translate(0, -GameCanvas.transY);
				if (Char.myCharz().mobFocus == null || !Char.myCharz().mobFocus.Equals(this) || status == 1 || hp <= 0 || imgHPtem == null)
				{
					return;
				}
				int imageWidth = mGraphics.getImageWidth(imgHPtem);
				int imageHeight = mGraphics.getImageHeight(imgHPtem);
				int num = imageWidth * per / 100;
				int num2 = num;
				if (per_tem >= per)
				{
					num2 = imageWidth * (per_tem -= ((GameCanvas.gameTick % 6 <= 3) ? offset : offset++)) / 100;
					if (per_tem <= 0)
					{
						per_tem = 0;
					}
					if (per_tem < per)
					{
						per_tem = per;
					}
					if (offset >= 3)
					{
						offset = 3;
					}
				}
				g.drawImage(GameScr.imgHP_tm_xam, x - (imageWidth >> 1), y - h - 5, mGraphics.TOP | mGraphics.LEFT);
				g.setColor(16777215);
				g.fillRect(x - (imageWidth >> 1), y - h - 5, num2, 2);
				g.drawRegion(imgHPtem, 0, 0, num, imageHeight, 0, x - (imageWidth >> 1), y - h - 5, mGraphics.TOP | mGraphics.LEFT);
			}

	public int getX()
			{
				return x;
			}

	public int getY()
			{
				return y;
			}

	public int getH()
			{
				return h;
			}

	public int getW()
			{
				return w;
			}

	public void stopMoving()
			{
				if (status == 5)
				{
					status = 2;
					p1 = (p2 = (p3 = 0));
					forceWait = 50;
				}
			}

	public bool isInvisible()
			{
				return status == 0 || status == 1;
			}

	public void GetFrame()
			{
				if (isGetFr && isTypeNewMod() && arrMobTemplate[templateId].data != null)
				{
					frameArr = (int[][])Controller.frameHT_NEWBOSS.get(templateId + string.Empty);
					stand = frameArr[0];
					move = frameArr[1];
					moveFast = frameArr[2];
					attack1 = frameArr[3];
					attack2 = frameArr[4];
					hurt = frameArr[5];
					isGetFr = false;
				}
			}

	private bool isTypeNewMod()
			{
				if (arrMobTemplate[templateId].data != null && arrMobTemplate[templateId].data.typeData == 2)
				{
					return true;
				}
				return false;
			}

}
