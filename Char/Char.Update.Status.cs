using System;
using Assets.src.e;
using Assets.src.g;
using UnityEngine;

public partial class Char : IMapObject
{
	private void updateCharStatus()
	{
						switch (statusMe)
						{
						case 1:
							updateCharStand();
							break;
						case 2:
							updateCharRun();
							break;
						case 3:
							updateCharJump();
							break;
						case 4:
							updateCharFall();
							break;
						case 5:
							updateCharDeadFly();
							break;
						case 16:
							updateResetPoint();
							break;
						case 9:
							updateCharAutoJump();
							break;
						case 10:
							updateCharFly();
							break;
						case 12:
							updateSkillStand();
							break;
						case 13:
							updateSkillFall();
							break;
						case 14:
							cp1++;
							if (cp1 > 30)
							{
								cp1 = 0;
							}
							if (cp1 % 15 < 5)
							{
								cf = 0;
							}
							else
							{
								cf = 1;
							}
							break;
						case 6:
							if (isInjure <= 0)
							{
								cf = 0;
							}
							else if (statusBeforeNothing == 10)
							{
								cx += cvx;
							}
							else if (cf <= 1)
							{
								cp1++;
								if (cp1 > 6)
								{
									cf = 0;
								}
								else
								{
									cf = 1;
								}
								if (cp1 > 10)
								{
									cp1 = 0;
								}
							}
							if (cf != 7 && cf != 12 && (TileMap.tileTypeAtPixel(cx, cy + 1) & 2) != 2)
							{
								cvx = 0;
								cvy = 0;
								statusMe = 4;
								cf = 7;
							}
							if (me)
							{
								break;
							}
							cp3++;
							if (cp3 > 10)
							{
								if ((TileMap.tileTypeAtPixel(cx, cy + 1) & 2) != 2)
								{
									cy += 5;
								}
								else
								{
									cf = 0;
								}
							}
							if (cp3 > 50)
							{
								cp3 = 0;
								currentMovePoint = null;
							}
							break;
						}
	}

}
