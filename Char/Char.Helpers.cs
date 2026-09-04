using System;
using Assets.src.e;
using Assets.src.g;
using UnityEngine;

public partial class Char : IMapObject
{
	public int getAvatar(int headId)
			{
				for (int i = 0; i < idHead.Length; i++)
				{
					if (headId == idHead[i])
					{
						return idAvatar[i];
					}
				}
				return -1;
			}

	public int getSys()
			{
				if (nClass.classId == 1 || nClass.classId == 2)
				{
					return 1;
				}
				if (nClass.classId == 3 || nClass.classId == 4)
				{
					return 2;
				}
				if (nClass.classId == 5 || nClass.classId == 6)
				{
					return 3;
				}
				return 0;
			}

	public bool isInWaypoint()
			{
				if (TileMap.isInAirMap() && cy >= TileMap.pxh - 48)
				{
					return true;
				}
				if (isTeleport || isUsePlane)
				{
					return false;
				}
				int num = TileMap.vGo.size();
				for (sbyte b = 0; b < num; b++)
				{
					Waypoint waypoint = (Waypoint)TileMap.vGo.elementAt(b);
					if ((TileMap.mapID == 47 || TileMap.isInAirMap()) && cy <= waypoint.minY + waypoint.maxY && cx > waypoint.minX && cx < waypoint.maxX)
					{
						if (TileMap.isInAirMap() && cTypePk != 0)
						{
							return false;
						}
						return true;
					}
					if (cx >= waypoint.minX && cx <= waypoint.maxX && cy >= waypoint.minY && cy <= waypoint.maxY && !waypoint.isEnter)
					{
						return true;
					}
				}
				return false;
			}

	private void checkHideCharName()
			{
				if (GameCanvas.gameTick % 20 != 0 || charID < 0)
				{
					return;
				}
				paintName = true;
				for (int i = 0; i < GameScr.vCharInMap.size(); i++)
				{
					Char @char = null;
					try
					{
						@char = (Char)GameScr.vCharInMap.elementAt(i);
					}
					catch (Exception)
					{
					}
					if (@char != null && !@char.Equals(this) && ((@char.cy == cy && Res.abs(@char.cx - cx) < 35) || (cy - @char.cy < 32 && cy - @char.cy > 0 && Res.abs(@char.cx - cx) < 24)))
					{
						paintName = false;
					}
				}
				for (int j = 0; j < GameScr.vNpc.size(); j++)
				{
					Npc npc = null;
					try
					{
						npc = (Npc)GameScr.vNpc.elementAt(j);
					}
					catch (Exception)
					{
					}
					if (npc != null && npc.cy == cy && Res.abs(npc.cx - cx) < 24)
					{
						paintName = false;
					}
				}
			}

	public void setResetPoint(int x, int y)
			{
				InfoDlg.hide();
				currentMovePoint = null;
				int num = cx - x;
				if (cy - y == 0)
				{
					cx = x;
					ischangingMap = false;
					isLockKey = false;
					return;
				}
				statusMe = 16;
				cp2 = x;
				cp3 = y;
				cp1 = 0;
				myCharz().cxSend = x;
				myCharz().cySend = y;
			}

	public int getVx(int size, int dx, int dy)
			{
				if (dy > 0 && !TileMap.tileTypeAt(cx, cy, 2))
				{
					if (dx - dy <= 10)
					{
						return 5;
					}
					if (dx - dy <= 30)
					{
						return 6;
					}
					if (dx - dy <= 50)
					{
						return 7;
					}
					if (dx - dy <= 70)
					{
						return 8;
					}
				}
				if (dx <= 30)
				{
					return 4;
				}
				if (dx <= 160)
				{
					return 5;
				}
				if (dx <= 270)
				{
					return 6;
				}
				if (dx <= 320)
				{
					return 7;
				}
				return 8;
			}

	public int getVy(int size, int dx, int dy)
			{
				if (dy <= 10)
				{
					return 5;
				}
				if (dy <= 20)
				{
					return 6;
				}
				if (dy <= 30)
				{
					return 7;
				}
				if (dy <= 40)
				{
					return 8;
				}
				if (dy <= 50)
				{
					return 9;
				}
				return 10;
			}

	public void setMount(int cid, int ctrans, int cgender)
			{
				idcharMount = cid;
				transMount = ctrans;
				genderMount = cgender;
				speedMount = 30;
				if (transMount < 0)
				{
					transMount = 0;
					xMount = GameScr.cmx + GameCanvas.w + 50;
					dxMount = -19;
				}
				else if (transMount == 1)
				{
					transMount = 2;
					xMount = GameScr.cmx - 100;
					dxMount = -33;
				}
				dyMount = -17;
				yMount = cy;
				frameMount = 0;
				frameNewMount = 0;
				isMount = false;
				isEndMount = false;
			}

	public void getMountData()
			{
				if (Mob.arrMobTemplate[50].data == null)
				{
					Mob.arrMobTemplate[50].data = new EffectData();
					string text = "/Mob/" + 50;
					DataInputStream dataInputStream = null;
					dataInputStream = MyStream.readFile(text);
					if (dataInputStream != null)
					{
						Mob.arrMobTemplate[50].data.readData(text + "/data");
						Mob.arrMobTemplate[50].data.img = GameCanvas.loadImage(text + "/img.png");
					}
					else
					{
						Service.gI().requestModTemplate(50);
					}
					Mob.lastMob.addElement(50 + string.Empty);
				}
			}

	public void setMountIsStart()
			{
				if (me)
				{
					isHaveMount = checkHaveMount();
					if (TileMap.isVoDaiMap())
					{
						isHaveMount = false;
					}
				}
				if (isHaveMount)
				{
					if (ySd - cy <= 20)
					{
						xChar = cx;
					}
					if (xdis < 100)
					{
						xdis = Res.abs(xChar - cx);
					}
					if (xdis >= 70 && ySd - cy > 30 && !isStartMount && !isEndMount)
					{
						setMount(charID, cdir, cgender);
						isStartMount = true;
					}
				}
			}

	public void setMountIsEnd()
			{
				if (ySd - cy < 24 && !isEndMount)
				{
					isStartMount = false;
					isMount = false;
					isEndMount = true;
					xdis = 0;
				}
			}

	public bool checkHaveMount()
			{
				bool result = false;
				short num = -1;
				Item[] array = arrItemBody;
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i] != null && (array[i].template.type == 24 || array[i].template.type == 23))
					{
						num = ((array[i].template.part < 0) ? array[i].template.id : ((short)(ID_NEW_MOUNT + array[i].template.part)));
						result = true;
						break;
					}
				}
				isMountVip = false;
				isSpeacialMount = false;
				isEventMount = false;
				idMount = -1;
				switch (num)
				{
				case 349:
				case 350:
				case 351:
					isMountVip = true;
					break;
				case 396:
					isEventMount = true;
					break;
				case 532:
					isSpeacialMount = true;
					break;
				default:
					if (num >= ID_NEW_MOUNT)
					{
						idMount = num;
					}
					break;
				}
				return result;
			}

	public void setDefaultPart()
			{
				setDefaultWeapon();
				setDefaultBody();
				setDefaultLeg();
			}

	public void setDefaultWeapon()
			{
				if (cgender == 0)
				{
					wp = 0;
				}
			}

	public bool isOutX()
			{
				if (cx < GameScr.cmx)
				{
					return true;
				}
				if (cx > GameScr.cmx + GameScr.gW)
				{
					return true;
				}
				return false;
			}

	public int getClassColor()
			{
				int result = 9145227;
				if (nClass.classId == 1 || nClass.classId == 2)
				{
					result = 16711680;
				}
				else if (nClass.classId == 3 || nClass.classId == 4)
				{
					result = 33023;
				}
				else if (nClass.classId == 5 || nClass.classId == 6)
				{
					result = 7443811;
				}
				return result;
			}

	public static int getIndexChar(int ID)
			{
				for (int i = 0; i < GameScr.vCharInMap.size(); i++)
				{
					Char @char = (Char)GameScr.vCharInMap.elementAt(i);
					if (@char.charID == ID)
					{
						return i;
					}
				}
				return -1;
			}

	public bool isMagicTree()
			{
				if (GameScr.gI().magicTree != null)
				{
					int x = GameScr.gI().magicTree.x;
					int y = GameScr.gI().magicTree.y;
					if (cx > x - 30 && cx < x + 30 && cy > y - 30 && cy < y + 30)
					{
						return true;
					}
					return false;
				}
				return false;
			}

	public static bool isCharInScreen(Char c)
			{
				int cmx = GameScr.cmx;
				int num = GameScr.cmx + GameCanvas.w;
				int num2 = GameScr.cmy + 10;
				int num3 = GameScr.cmy + GameScr.gH;
				if (c.statusMe != 15 && !c.isInvisiblez && cmx <= c.cx && c.cx <= num && num2 <= c.cy && c.cy <= num3)
				{
					return true;
				}
				return false;
			}

	public bool isAttacPlayerStatus()
			{
				return cTypePk == 4 || cTypePk == 3;
			}

	public static bool setInsc(int cmX, int cmWx, int x, int cmy, int cmyH, int y)
			{
				if (x > cmWx || x < cmX || y > cmyH || y < cmy)
				{
					return false;
				}
				return true;
			}

	public bool isLang()
			{
				if (TileMap.mapID == 1 || TileMap.mapID == 27 || TileMap.mapID == 72 || TileMap.mapID == 10 || TileMap.mapID == 17 || TileMap.mapID == 22 || TileMap.mapID == 32 || TileMap.mapID == 38 || TileMap.mapID == 43 || TileMap.mapID == 48)
				{
					return true;
				}
				return false;
			}

	public int getX()
			{
				return cx;
			}

	public int getY()
			{
				return cy;
			}

	public int getH()
			{
				return 32;
			}

	public int getW()
			{
				return 24;
			}

	public bool isInvisible()
			{
				return false;
			}

	public bool isGetFlagImage(sbyte getFlag)
			{
				bool result = true;
				for (int i = 0; i < GameScr.vFlag.size(); i++)
				{
					PKFlag pKFlag = (PKFlag)GameScr.vFlag.elementAt(i);
					if (pKFlag != null)
					{
						if (pKFlag.cflag == getFlag)
						{
							return true;
						}
						result = false;
					}
				}
				return result;
			}

	public void setPartOld()
			{
				headTemp = head;
				bodyTemp = body;
				legTemp = leg;
				bagTemp = bag;
			}

	public void setPartTemp(int head, int body, int leg, int bag)
			{
				if (head != -1)
				{
					this.head = head;
				}
				if (body != -1)
				{
					this.body = body;
				}
				if (leg != -1)
				{
					this.leg = leg;
				}
				if (bag != -1)
				{
					this.bag = bag;
				}
			}

	public void resetPartTemp()
			{
				if (headTemp != -1)
				{
					head = headTemp;
					headTemp = -1;
				}
				if (bodyTemp != -1)
				{
					body = bodyTemp;
					bodyTemp = -1;
				}
				if (legTemp != -1)
				{
					leg = legTemp;
					legTemp = -1;
				}
				if (bagTemp != -1)
				{
					bag = bagTemp;
					bagTemp = -1;
				}
			}

	public int checkLuong()
			{
				return luong + luongKhoa;
			}

	public bool isFrNgang(int fr)
			{
				if (fr == 2 || fr == 3 || fr == 4 || fr == 5 || fr == 6 || fr == 9 || fr == 10 || fr == 13 || fr == 14 || fr == 15 || fr == 16 || fr == 26 || fr == 27 || fr == 28 || fr == 29)
				{
					return true;
				}
				return false;
			}

	public void setDanhHieu(int smallDanhHieu, int frame)
			{
				smallDanhHieu = 0;
				frame = 1;
				if (mainImg == null)
				{
					mainImg = ImgByName.getImagePath("banner_" + 0, ImgByName.hashImagePath);
				}
				if (mainImg.img != null)
				{
					int num = mainImg.img.getHeight() / mainImg.nFrame;
					if (num < 1)
					{
						num = 1;
					}
					fraDanhHieu = new FrameImage(mainImg.img, mainImg.img.getWidth(), num);
				}
				Res.err("===== tim thay DanhHieu ve danh hieu ra");
			}

}
