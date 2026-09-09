using System;
using Assets.src.e;
using Assets.src.g;
using UnityEngine;

public partial class Char : IMapObject
{
	private void updateOtherChar()
	{
							checkHideCharName();
							if (statusMe == 1 || statusMe == 6)
							{
								bool flag5 = false;
								if (currentMovePoint != null)
								{
									if (abs(currentMovePoint.xEnd - cx) < 17 && abs(currentMovePoint.yEnd - cy) < 25)
									{
										cx = currentMovePoint.xEnd;
										cy = currentMovePoint.yEnd;
										currentMovePoint = null;
										if ((TileMap.tileTypeAtPixel(cx, cy) & 2) == 2)
										{
											statusMe = 1;
											cp3 = 0;
											GameCanvas.gI().startDust(-1, cx - -8, cy);
											GameCanvas.gI().startDust(1, cx - 8, cy);
										}
										else
										{
											statusMe = 4;
											cvy = 0;
											cp1 = 0;
										}
										flag5 = true;
									}
									else if ((statusBeforeNothing == 10 || cf == 8) && vMovePoints.size() > 0)
									{
										flag5 = true;
									}
									else if (cy == currentMovePoint.yEnd)
									{
										if (cx != currentMovePoint.xEnd)
										{
											cx = (cx + currentMovePoint.xEnd) / 2;
											cf = GameCanvas.gameTick % 5 + 2;
										}
									}
									else if (cy < currentMovePoint.yEnd)
									{
										cf = 12;
										cx = (cx + currentMovePoint.xEnd) / 2;
										if (cvy < 0)
										{
											cvy = 0;
										}
										cy += cvy;
										if ((TileMap.tileTypeAtPixel(cx, cy) & 2) == 2)
										{
											GameCanvas.gI().startDust(-1, cx - -8, cy);
											GameCanvas.gI().startDust(1, cx - 8, cy);
										}
										cvy++;
										if (cvy > 16)
										{
											cy = (cy + currentMovePoint.yEnd) / 2;
										}
									}
									else
									{
										cf = 7;
										cx = (cx + currentMovePoint.xEnd) / 2;
										cy = (cy + currentMovePoint.yEnd) / 2;
									}
								}
								else
								{
									flag5 = true;
								}
								if (flag5 && vMovePoints.size() > 0)
								{
									currentMovePoint = (MovePoint)vMovePoints.firstElement();
									vMovePoints.removeElementAt(0);
									if (currentMovePoint.status == 2)
									{
										if ((TileMap.tileTypeAtPixel(cx, cy + 12) & 2) != 2)
										{
											statusMe = 10;
											cp1 = 0;
											cp2 = 0;
											cvx = -(cx - currentMovePoint.xEnd) / 10;
											cvy = -(cy - currentMovePoint.yEnd) / 10;
											if (cx - currentMovePoint.xEnd > 0)
											{
												cdir = -1;
											}
											else if (cx - currentMovePoint.xEnd < 0)
											{
												cdir = 1;
											}
										}
										else
										{
											statusMe = 2;
											if (cx - currentMovePoint.xEnd > 0)
											{
												cdir = -1;
											}
											else if (cx - currentMovePoint.xEnd < 0)
											{
												cdir = 1;
											}
											cvx = cspeed * cdir;
											cvy = 0;
										}
									}
									else if (currentMovePoint.status == 3)
									{
										if ((TileMap.tileTypeAtPixel(cx, cy + 23) & 2) != 2)
										{
											statusMe = 10;
											cp1 = 0;
											cp2 = 0;
											cvx = -(cx - currentMovePoint.xEnd) / 10;
											cvy = -(cy - currentMovePoint.yEnd) / 10;
											if (cx - currentMovePoint.xEnd > 0)
											{
												cdir = -1;
											}
											else if (cx - currentMovePoint.xEnd < 0)
											{
												cdir = 1;
											}
										}
										else
										{
											statusMe = 3;
											GameCanvas.gI().startDust(-1, cx - -8, cy);
											GameCanvas.gI().startDust(1, cx - 8, cy);
											if (cx - currentMovePoint.xEnd > 0)
											{
												cdir = -1;
											}
											else if (cx - currentMovePoint.xEnd < 0)
											{
												cdir = 1;
											}
											cvx = abs(cx - currentMovePoint.xEnd) / 10 * cdir;
											cvy = -10;
										}
									}
									else if (currentMovePoint.status == 4)
									{
										statusMe = 4;
										if (cx - currentMovePoint.xEnd > 0)
										{
											cdir = -1;
										}
										else if (cx - currentMovePoint.xEnd < 0)
										{
											cdir = 1;
										}
										cvx = abs(cx - currentMovePoint.xEnd) / 9 * cdir;
										cvy = 0;
									}
									else
									{
										cx = currentMovePoint.xEnd;
										cy = currentMovePoint.yEnd;
										currentMovePoint = null;
									}
								}
								else if (flag5 && me && (cx != cxSend || cy != cySend))
								{
									Service.gI().charMove();
								}
							}
	}
}
