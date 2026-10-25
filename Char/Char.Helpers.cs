using System;
using Assets.src.e;
using Assets.src.g;
using UnityEngine;

/// <summary>
/// Thành phần phân mảnh (partial) của lớp Char: Đảm nhiệm phân hệ chuyên trách Helpers ().
/// </summary>
public partial class Char : IMapObject
{
	/// <summary>
	/// Truy xuất và trả về dữ liệu của getAvatar.
	/// </summary>
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

	/// <summary>
	/// Truy xuất và trả về dữ liệu của getSys.
	/// </summary>
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

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của isInWaypoint.
	/// </summary>
	public bool isInWaypoint()
			{
				if (isLoadingMap || ModNextMap.isNextMapActive)
				{
					return false;
				}
				if ((ModTanSat.autoTanSat || ModAutoPick.isBusy) && !ModGoBack.isReturning && !ModSetActivator.isBusy && !ModAutoBuyBua.isBusy && !ModTanSat.IsManualMoving())
				{
					return false;
				}
				if (TileMap.isInAirMap() && cy >= TileMap.pxh - 48)
				{
					if (ModNextMap.isNextMapActive && ModNextMap.currentTargetWaypoint != null && ModNextMap.currentTargetWaypoint.minY <= 150)
					{
						return false;
					}
					return true;
				}
				if (isTeleport || isUsePlane)
				{
					return false;
				}
				if (TileMap.vGo == null)
				{
					return false;
				}
				int num = TileMap.vGo.size();
				for (sbyte b = 0; b < num; b++)
				{
					Waypoint waypoint = (Waypoint)TileMap.vGo.elementAt(b);
					if (waypoint == null)
					{
						continue;
					}
					if (ModNextMap.isNextMapActive && ModNextMap.currentTargetWaypoint != null && waypoint != ModNextMap.currentTargetWaypoint)
					{
						continue;
					}
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
						if (entranceWaypoint != null && waypoint == entranceWaypoint)
						{
							return false;
						}
						return true;
					}
				}
				if (entranceWaypoint != null)
				{
					if (cx < entranceWaypoint.minX || cx > entranceWaypoint.maxX || cy < entranceWaypoint.minY || cy > entranceWaypoint.maxY)
					{
						entranceWaypoint = null;
					}
				}
				return false;
			}

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của checkHideCharName.
	/// </summary>
	private void checkHideCharName()
			{
				paintName = true;
			}

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setResetPoint.
	/// </summary>
	public void setResetPoint(int x, int y)
			{
				ModTeleport.NotifyServerResetPointReceived();
				ModTdltLogger.Log("RESET_POINT_RAW", $"Packet 46 received -> resetPos=({x},{y}), curPos=({cx},{cy}), Map='{TileMap.mapName}' (ID:{TileMap.mapID}), HP={cHP}/{cHPFull}");
				InfoDlg.hide();
				currentMovePoint = null;
				vMovePoints.removeAllElements();
				myCharz().cxSend = x;
				myCharz().cySend = y;
				if (ModNextMap.isNextMapActive)
				{
					int prevCx = cx;
					int prevCy = cy;
					cx = x;
					cy = y;
					myCharz().cxSend = x;
					myCharz().cySend = y;
					cvx = 0;
					cvy = 0;
					statusMe = 1;
					delayFall = 0;
					cp1 = (cp2 = (cp3 = 0));
					ModWaypoint.OnServerResetPoint(x, y, prevCx, prevCy);
					return;
				}
				if (ModTanSat.autoTanSat || ModAutoPick.isBusy || ModGoBack.isReturning || cy - y == 0)
				{
					bool isEcho = ModTeleport.TryConsumeTeleportEcho(x, y, cx, cy, out bool isCurrentPos, out bool hasNewerPendingEcho);
					bool isNearCurrent = (Res.abs(x - cx) <= 32 && Res.abs(y - cy) <= 32);
					cvx = 0;
					cvy = 0;
					statusMe = 1;
					delayFall = 0;
					cp1 = (cp2 = (cp3 = 0));
					ischangingMap = false;
					isLockKey = false;
					ModTeleport.isWaitingSettle = false;

					if (isEcho || isNearCurrent)
					{
						if (isCurrentPos || isNearCurrent)
						{
							myCharz().cxSend = cx;
							myCharz().cySend = cy;
							ModTeleport.isWaitingServerSync = false;
							ModTeleport.isCurrentPosSynced = true;
							if (ModTanSat.autoTanSat)
							{
								ModTanSat.OnTeleportSyncConfirmed();
							}
							if (ModTanSat.autoTanSat || ModGoBack.isReturning)
							{
								ModTdltLogger.Log("RESET_POINT_CONFIRM", $"Server confirmed teleport pos=({x},{y})");
							}
						}
						else if (hasNewerPendingEcho)
						{
							myCharz().cxSend = cx;
							myCharz().cySend = cy;
						}
						else
						{
							cx = x;
							cy = y;
							myCharz().cxSend = x;
							myCharz().cySend = y;
							ModTeleport.isWaitingServerSync = false;
							ModTeleport.isCurrentPosSynced = true;
							if (ModTanSat.autoTanSat)
							{
								ModTanSat.lastTeleportTargetMobId = -1;
								ModTanSat.lastTeleportTime = 0;
								ModTanSat.waitUntilTime = mSystem.currentTimeMillis() + 180;
								ModTanSat.OnTeleportSyncConfirmed();
							}
							if (ModTanSat.autoTanSat || ModGoBack.isReturning)
							{
								ModTdltLogger.Log("RESET_POINT_STALE", $"Synced delayed server pos=({x},{y}) to re-teleport cleanly");
							}
						}
						return;
					}

					cx = x;
					cy = y;
					myCharz().cxSend = x;
					myCharz().cySend = y;
					ModTeleport.isWaitingServerSync = false;
					ModTeleport.isCurrentPosSynced = true;
					if (ModTanSat.autoTanSat)
					{
						ModTanSat.lastTeleportTargetMobId = -1;
						ModTanSat.lastTeleportTime = 0;
						ModTanSat.OnServerResetPoint();
					}
					return;
				}
				statusMe = 16;
				cp2 = x;
				cp3 = y;
				cp1 = 0;
			}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của getVx.
	/// </summary>
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

	/// <summary>
	/// Truy xuất và trả về dữ liệu của getVy.
	/// </summary>
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

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setMount.
	/// </summary>
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

	/// <summary>
	/// Truy xuất và trả về dữ liệu của getMountData.
	/// </summary>
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

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setMountIsStart.
	/// </summary>
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

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setMountIsEnd.
	/// </summary>
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

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của checkHaveMount.
	/// </summary>
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

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setDefaultPart.
	/// </summary>
	public void setDefaultPart()
			{
				setDefaultWeapon();
				setDefaultBody();
				setDefaultLeg();
			}

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setDefaultWeapon.
	/// </summary>
	public void setDefaultWeapon()
			{
				if (cgender == 0)
				{
					wp = 0;
				}
			}

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của isOutX.
	/// </summary>
	public bool isOutX()
			{
				if (cx < GameScr.cmx - 60)
				{
					return true;
				}
				if (cx > GameScr.cmx + GameScr.gW + 60)
				{
					return true;
				}
				return false;
			}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của getClassColor.
	/// </summary>
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

	/// <summary>
	/// Truy xuất và trả về dữ liệu của getIndexChar.
	/// </summary>
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

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của isMagicTree.
	/// </summary>
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

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của isCharInScreen.
	/// </summary>
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

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của isAttacPlayerStatus.
	/// </summary>
	public bool isAttacPlayerStatus()
			{
				return cTypePk == 4 || cTypePk == 3;
			}

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setInsc.
	/// </summary>
	public static bool setInsc(int cmX, int cmWx, int x, int cmy, int cmyH, int y)
			{
				if (x > cmWx || x < cmX || y > cmyH || y < cmy)
				{
					return false;
				}
				return true;
			}

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của isLang.
	/// </summary>
	public bool isLang()
			{
				if (TileMap.mapID == 1 || TileMap.mapID == 27 || TileMap.mapID == 72 || TileMap.mapID == 10 || TileMap.mapID == 17 || TileMap.mapID == 22 || TileMap.mapID == 32 || TileMap.mapID == 38 || TileMap.mapID == 43 || TileMap.mapID == 48)
				{
					return true;
				}
				return false;
			}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của getX.
	/// </summary>
	public int getX()
			{
				return cx;
			}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của getY.
	/// </summary>
	public int getY()
			{
				return cy;
			}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của getH.
	/// </summary>
	public int getH()
			{
				return 32;
			}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của getW.
	/// </summary>
	public int getW()
			{
				return 24;
			}

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của isInvisible.
	/// </summary>
	public bool isInvisible()
			{
				return false;
			}

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của isGetFlagImage.
	/// </summary>
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

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setPartOld.
	/// </summary>
	public void setPartOld()
			{
				headTemp = head;
				bodyTemp = body;
				legTemp = leg;
				bagTemp = bag;
			}

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setPartTemp.
	/// </summary>
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

	/// <summary>
	/// Dọn dẹp và đặt lại trạng thái ban đầu cho resetPartTemp.
	/// </summary>
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

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của checkLuong.
	/// </summary>
	public int checkLuong()
			{
				return luong + luongKhoa;
			}

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của isFrNgang.
	/// </summary>
	public bool isFrNgang(int fr)
			{
				if (fr == 2 || fr == 3 || fr == 4 || fr == 5 || fr == 6 || fr == 9 || fr == 10 || fr == 13 || fr == 14 || fr == 15 || fr == 16 || fr == 26 || fr == 27 || fr == 28 || fr == 29)
				{
					return true;
				}
				return false;
			}

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setDanhHieu.
	/// </summary>
	public void setDanhHieu(int smallDanhHieu, int frame)
			{
				if (mainImg == null)
				{
					mainImg = ImgByName.getImagePath("banner_" + smallDanhHieu, ImgByName.hashImagePath);
				}
				if (mainImg.img != null)
				{
					int nFr = (mainImg.nFrame > 0) ? mainImg.nFrame : ((frame > 0) ? frame : 1);
					int num = mainImg.img.getHeight() / nFr;
					if (num < 1)
					{
						num = 1;
					}
					fraDanhHieu = new FrameImage(mainImg.img, mainImg.img.getWidth(), num);
				}
			}

}
