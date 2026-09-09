using System;
using Assets.src.e;
using Assets.src.g;
using UnityEngine;

public partial class Char : IMapObject
{
	private bool updateMyChar()
	{
							if (charFocus != null && !GameScr.vCharInMap.contains(charFocus))
							{
								charFocus = null;
							}
							if (cx < 10)
							{
								cvx = 0;
								cx = 10;
							}
							else if (cx > TileMap.pxw - 10)
							{
								cx = TileMap.pxw - 10;
								cvx = 0;
							}
							if (!ischangingMap && !isLoadingMap && isInWaypoint())
							{
								Service.gI().charMove();
								if (TileMap.isTrainingMap())
								{
									Service.gI().getMapOffline();
									ischangingMap = true;
								}
								else
								{
									Service.gI().requestChangeMap();
								}
								isLockKey = true;
								ischangingMap = true;
								GameCanvas.clearKeyHold();
								GameCanvas.clearKeyPressed();
								InfoDlg.showWait();
								return true;
							}
							if (currentMovePoint == null && statusMe != 4 && Res.abs(cx - cxSend) + Res.abs(cy - cySend) >= 200 && cy - cySend <= 0 && me)
							{
								Service.gI().charMove();
							}
							if (isLockMove)
							{
								currentMovePoint = null;
							}
							if (currentMovePoint != null)
							{
								if (abs(cx - currentMovePoint.xEnd) <= 16 && abs(cy - currentMovePoint.yEnd) <= 16)
								{
									cx = (currentMovePoint.xEnd + cx) / 2;
									cy = currentMovePoint.yEnd;
									currentMovePoint = null;
									GameScr.instance.clickMoving = false;
									checkPerformEndMovePointAction();
									cvx = (cvy = 0);
									if ((TileMap.tileTypeAtPixel(cx, cy) & 2) == 2)
									{
										statusMe = 1;
									}
									else
									{
										setCharFallFromJump();
									}
									Service.gI().charMove();
								}
								else
								{
									cdir = ((currentMovePoint.xEnd > cx) ? 1 : (-1));
									if (TileMap.tileTypeAt(cx, cy, 2))
									{
										statusMe = 2;
										if (currentMovePoint != null)
										{
											cvx = cspeed * cdir;
											cvy = 0;
										}
										if (abs(cx - currentMovePoint.xEnd) <= 10)
										{
											if (currentMovePoint.yEnd > cy)
											{
												bool flag4 = false;
												sbyte b = 1;
												b = (sbyte)((cdir == 1) ? 1 : (-1));
												for (int j = 0; j < 2; j++)
												{
													if (TileMap.tileTypeAt(currentMovePoint.xEnd + chw * b, cy + chh * j, 2))
													{
														flag4 = true;
														break;
													}
												}
												if (flag4)
												{
													currentMovePoint = null;
													GameScr.instance.clickMoving = false;
													statusMe = 1;
													cvx = (cvy = 0);
													checkPerformEndMovePointAction();
												}
												else
												{
													SoundMn.gI().charJump();
													cx = currentMovePoint.xEnd;
													statusMe = 10;
													cvy = -5;
													cvx = 0;
													Res.outz("Jum lun");
												}
											}
											else
											{
												SoundMn.gI().charJump();
												cx = currentMovePoint.xEnd;
												statusMe = 10;
												cvy = -5;
												cvx = 0;
											}
										}
										if (cdir == 1)
										{
											if (TileMap.tileTypeAt(cx + chw, cy - chh, 4))
											{
												cvx = cspeed * cdir;
												statusMe = 10;
												cvy = -5;
											}
										}
										else if (TileMap.tileTypeAt(cx - chw - 1, cy - chh, 8))
										{
											cvx = cspeed * cdir;
											statusMe = 10;
											cvy = -5;
										}
									}
									else
									{
										if (currentMovePoint.yEnd < cy + 10)
										{
											statusMe = 10;
											cvy = -5;
											if (abs(cy - currentMovePoint.yEnd) <= 10)
											{
												cy = currentMovePoint.yEnd;
												cvy = 0;
											}
											if (abs(cx - currentMovePoint.xEnd) <= 10)
											{
												cvx = 0;
											}
											else
											{
												cvx = cspeed * cdir;
											}
										}
										else if (TileMap.tileTypeAt(cx, cy, 2))
										{
											currentMovePoint = null;
											GameScr.instance.clickMoving = false;
											statusMe = 1;
											cvx = (cvy = 0);
											checkPerformEndMovePointAction();
										}
										else
										{
											if (statusMe == 10 || statusMe == 2)
											{
												cvy = 0;
											}
											statusMe = 4;
										}
										if (currentMovePoint.yEnd > cy)
										{
											if (cdir == 1)
											{
												if (TileMap.tileTypeAt(cx + chw, cy - chh, 4))
												{
													cvx = (cvy = 0);
													statusMe = 4;
													currentMovePoint = null;
													GameScr.instance.clickMoving = false;
													checkPerformEndMovePointAction();
												}
											}
											else if (TileMap.tileTypeAt(cx - chw - 1, cy - chh, 8))
											{
												cvx = (cvy = 0);
												statusMe = 4;
												currentMovePoint = null;
												GameScr.instance.clickMoving = false;
												checkPerformEndMovePointAction();
											}
										}
									}
								}
							}
							searchFocus();
		return false;
	}
}
