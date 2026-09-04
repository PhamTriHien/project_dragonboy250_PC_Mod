using System;
using Assets.src.g;
public partial class GameScr : mScreen, IChatable
{
	private void checkClick()
			{
				if (isCharging())
				{
					return;
				}
				if (ModArrowButton.CheckClick())
				{
					return;
				}
				if (ModMenu.uiCustomOpen)
				{
					return;
				}
				if (ModBossNotice.CheckHUDClick(GameCanvas.px, GameCanvas.py))
				{
					return;
				}
				if (ModNextMap.CheckHUDMapTagClick(GameCanvas.px, GameCanvas.py))
				{
					return;
				}
				if (cmdMenu != null)
				{
					int menuW = (cmdMenu.w > 0) ? cmdMenu.w : 64;
					int menuH = (cmdMenu.h > 0) ? cmdMenu.h : 34;
					bool isClickMenuBtn = GameCanvas.isPointerHoldIn(cmdMenu.x, cmdMenu.y, menuW, menuH);
					if (!isClickMenuBtn && GameCanvas.isTouch && GameCanvas.isPointerHoldIn(0, 0, 60, 50))
					{
						isClickMenuBtn = true;
					}
					if (!isClickMenuBtn && !GameCanvas.isTouch && GameCanvas.isPointerHoldIn(0, GameCanvas.h - 35, 65, 35))
					{
						isClickMenuBtn = true;
					}
					if (isClickMenuBtn)
					{
						if (GameCanvas.isPointerClick || GameCanvas.isPointerJustRelease)
						{
							GameCanvas.clearAllPointerEvent();
							Char.myCharz().currentMovePoint = null;
							Char.myCharz().vMovePoints.removeAllElements();
							clickMoving = false;
							cmdMenu.performAction();
							return;
						}
						return;
					}
				}
				if (popUpYesNo != null && popUpYesNo.cmdYes != null && popUpYesNo.cmdYes.isPointerPressInside())
				{
					popUpYesNo.cmdYes.performAction();
				}
				else
				{
					if (checkClickToCapcha())
					{
						return;
					}
					if (GameCanvas.isPointerJustRelease || GameCanvas.isPointerClick)
					{
						int worldX = GameCanvas.px + cmx;
						int worldY = GameCanvas.py + cmy;

						// 1. Kiểm tra click vào Waypoint (cổng chuyển map trên mặt đất)
						Waypoint wp = findClickToWaypoint(worldX, worldY);
						if (wp != null)
						{
							GameCanvas.clearAllPointerEvent();
							ModWaypoint.StepToWaypoint(wp);
							GameScr.info1.addInfo("Di chuyển qua cổng " + (wp.name ?? "kế tiếp"), 0);
							return;
						}

						// 2. Kiểm tra click vào PopUp trên bản đồ
						if (checkClickToPopup(worldX, worldY) || checkClipTopChatPopUp(worldX, worldY))
						{
							GameCanvas.clearAllPointerEvent();
							return;
						}

						// 3. Kiểm tra click vào thực thể trong game (Mob, NPC, Item, Nhân vật)
						IMapObject mapObject = findClickToItem(worldX, worldY);
						if (mapObject != null)
						{
							Char me = Char.myCharz();
							if (me != null)
							{
								me.cancelAttack();
								if (me.mobFocus == mapObject || me.itemFocus == mapObject || me.npcFocus == mapObject || me.charFocus == mapObject)
								{
									doDoubleClickToObj(mapObject);
								}
								else
								{
									me.focusManualTo(mapObject);
									mapObject.stopMoving();
								}
							}
							GameCanvas.clearAllPointerEvent();
							return;
						}

						// 4. Click chuột ra ngoài khoảng trống -> Không di chuyển nhân vật
						GameCanvas.clearAllPointerEvent();
					}
				}
			}
	private Waypoint findClickToWaypoint(int px, int py)
			{
				if (TileMap.vGo == null || TileMap.vGo.size() == 0)
				{
					return null;
				}
				for (int i = 0; i < TileMap.vGo.size(); i++)
				{
					Waypoint waypoint = (Waypoint)TileMap.vGo.elementAt(i);
					if (waypoint != null)
					{
						int diffW = waypoint.maxX - waypoint.minX;
						int diffH = waypoint.maxY - waypoint.minY;
						int w = (diffW > 40) ? diffW : 40;
						int h = (diffH > 40) ? diffH : 40;
						if (inRectangle(px, py, waypoint.minX - 25, waypoint.minY - 25, w + 50, h + 50))
						{
							return waypoint;
						}
						if (waypoint.popup != null && inRectangle(px, py, waypoint.popup.cx - 15, waypoint.popup.cy - 15, waypoint.popup.cw + 30, waypoint.popup.ch + 30))
						{
							return waypoint;
						}
					}
				}
				return null;
			}
	private IMapObject findClickToItem(int px, int py)
			{
				IMapObject mapObject = null;
				int num = 0;
				int num2 = 30;
				MyVector[] array = new MyVector[4] { vMob, vNpc, vItemMap, vCharInMap };
				for (int i = 0; i < array.Length; i++)
				{
					for (int j = 0; j < array[i].size(); j++)
					{
						IMapObject mapObject2 = (IMapObject)array[i].elementAt(j);
						if (mapObject2.isInvisible())
						{
							continue;
						}
						if (mapObject2 is Mob)
						{
							Mob mob = (Mob)mapObject2;
							if (mob.isMobMe && mob.Equals(Char.myCharz().mobMe))
							{
								continue;
							}
						}
						int x = mapObject2.getX();
						int y = mapObject2.getY();
						int w = mapObject2.getW();
						int h = mapObject2.getH();
						if (!inRectangle(px, py, x - w / 2 - num2, y - h - num2, w + num2 * 2, h + num2 * 2))
						{
							continue;
						}
						if (mapObject == null)
						{
							mapObject = mapObject2;
							num = Res.abs(px - x) + Res.abs(py - y);
							if (i == 1)
							{
								return mapObject;
							}
						}
						else
						{
							int num3 = Res.abs(px - x) + Res.abs(py - y);
							if (num3 < num)
							{
								mapObject = mapObject2;
								num = num3;
							}
						}
					}
				}
				return mapObject;
			}
	private Mob findClickToMOB(int px, int py)
			{
				int num = 30;
				Mob mob = null;
				int num2 = 0;
				for (int i = 0; i < vMob.size(); i++)
				{
					Mob mob2 = (Mob)vMob.elementAt(i);
					if (mob2.isInvisible())
					{
						continue;
					}
					if (mob2 != null)
					{
						Mob mob3 = mob2;
						if (mob3.isMobMe && mob3.Equals(Char.myCharz().mobMe))
						{
							continue;
						}
					}
					int x = mob2.getX();
					int y = mob2.getY();
					int w = mob2.getW();
					int h = mob2.getH();
					if (!inRectangle(px, py, x - w / 2 - num, y - h - num, w + num * 2, h + num * 2))
					{
						continue;
					}
					if (mob == null)
					{
						mob = mob2;
						num2 = Res.abs(px - x) + Res.abs(py - y);
						continue;
					}
					int num3 = Res.abs(px - x) + Res.abs(py - y);
					if (num3 < num2)
					{
						mob = mob2;
						num2 = num3;
					}
				}
				return mob;
			}
	private bool checkSingleClickEarly()
			{
				int num = GameCanvas.px + cmx;
				int num2 = GameCanvas.py + cmy;
				Char.myCharz().cancelAttack();
				IMapObject mapObject = findClickToItem(num, num2);
				if (mapObject != null)
				{
					if (Char.myCharz().isAttacPlayerStatus() && Char.myCharz().charFocus != null && !mapObject.Equals(Char.myCharz().charFocus) && !mapObject.Equals(Char.myCharz().charFocus.mobMe) && mapObject is Char)
					{
						Char @char = (Char)mapObject;
						if (@char.cTypePk != 5 && !@char.isAttacPlayerStatus())
						{
							return false;
						}
					}
					if (Char.myCharz().mobFocus == mapObject || Char.myCharz().itemFocus == mapObject || Char.myCharz().npcFocus == mapObject)
					{
						doDoubleClickToObj(mapObject);
						return true;
					}
					if (Char.myCharz().skillPaint != null || Char.myCharz().arr != null || Char.myCharz().dart != null || Char.myCharz().skillInfoPaint() != null)
					{
						return false;
					}
					Char.myCharz().focusManualTo(mapObject);
					mapObject.stopMoving();
					return true;
				}
				return false;
			}
	private void checkDoubleClick()
			{
				int num = GameCanvas.px + lastClickCMX;
				int num2 = GameCanvas.py + lastClickCMY;
				if (isLockKey)
				{
					return;
				}
				IMapObject mapObject = findClickToItem(num, num2);
				if (mapObject != null)
				{
					if (checkClickToBotton(mapObject) || (!mapObject.Equals(Char.myCharz().npcFocus) && mobCapcha != null))
					{
						return;
					}
					doDoubleClickToObj(mapObject);
				}
			}
	private bool checkClickToBotton(IMapObject Object)
			{
				if (Object == null)
				{
					return false;
				}
				int y = Object.getY();
				int num = Char.myCharz().cy;
				if (y < num)
				{
					while (y < num)
					{
						num -= 5;
						if (TileMap.tileTypeAt(Char.myCharz().cx, num, 8192))
						{
							auto = 0;
							Char.myCharz().cancelAttack();
							Char.myCharz().currentMovePoint = null;
							return true;
						}
					}
				}
				return false;
			}
	private void doDoubleClickToObj(IMapObject obj)
			{
				if (obj == null)
				{
					return;
				}
				if (obj is Npc)
				{
					Npc npc = (Npc)obj;
					Char.myCharz().focusManualTo(npc);
					Char.myCharz().cancelAttack();
					Char.myCharz().currentMovePoint = null;
					auto = 0;
					Service.gI().openMenu(npc.npcId);
					return;
				}
				if (obj is ItemMap)
				{
					ItemMap item = (ItemMap)obj;
					Char.myCharz().focusManualTo(item);
					Char.myCharz().cancelAttack();
					Service.gI().pickItem(item.itemMapID);
					return;
				}
				if ((obj.Equals(Char.myCharz().npcFocus) || mobCapcha == null) && !checkClickToBotton(obj))
				{
					checkEffToObj(obj, isnew: false);
					Char me = Char.myCharz();
					if (me != null)
					{
						me.currentMovePoint = null;
						me.cvx = (me.cvy = 0);
						me.cdir = (obj.getX() >= me.cx) ? 1 : -1;
						if (obj is Mob)
						{
							me.mobFocus = (Mob)obj;
							me.charFocus = null;
						}
					}
					obj.stopMoving();
					auto = 10;
					doFire(isFireByShortCut: false, skipWaypoint: true);
				}
			}

}
