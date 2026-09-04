using System;
using Assets.src.g;
public partial class GameScr : mScreen, IChatable
{
	private void paintXoSo(mGraphics g)
			{
				if (tShow != 0)
				{
					string text = string.Empty;
					for (int i = 0; i < winnumber.Length; i++)
					{
						text = text + randomNumber[i] + " ";
					}
					PopUp.paintPopUp(g, 20, 45, 95, 35, 16777215, isButton: false);
					mFont.tahoma_7b_dark.drawString(g, mResources.kquaVongQuay, 68, 50, 2);
					mFont.tahoma_7b_dark.drawString(g, text + string.Empty, 68, 65, 2);
				}
			}
	private void paintWaypointArrow(mGraphics g)
			{
				int num = 10;
				Task taskMaint = Char.myCharz().taskMaint;
				if (taskMaint != null && taskMaint.taskId == 0 && ((taskMaint.index != 1 && taskMaint.index < 6) || taskMaint.index == 0))
				{
					return;
				}
				for (int i = 0; i < TileMap.vGo.size(); i++)
				{
					Waypoint waypoint = (Waypoint)TileMap.vGo.elementAt(i);
					if (waypoint.minY == 0 || waypoint.maxY >= TileMap.pxh - 24)
					{
						if (waypoint.maxY <= TileMap.pxh / 2)
						{
							int x = waypoint.minX + (waypoint.maxX - waypoint.minX) / 2;
							int y = waypoint.minY + (waypoint.maxY - waypoint.minY) / 2 + runArrow;
							if (GameCanvas.isTouch)
							{
								y = waypoint.maxY + (waypoint.maxY - waypoint.minY) + runArrow + num;
							}
							g.drawRegion(arrow, 0, 0, 13, 16, 6, x, y, StaticObj.VCENTER_HCENTER);
						}
						else if (waypoint.minY >= TileMap.pxh / 2)
						{
							g.drawRegion(arrow, 0, 0, 13, 16, 4, waypoint.minX + (waypoint.maxX - waypoint.minX) / 2, waypoint.minY - 12 - runArrow, StaticObj.VCENTER_HCENTER);
						}
					}
					else if (waypoint.minX >= 0 && waypoint.minX < 24)
					{
						if (!GameCanvas.isTouch)
						{
							g.drawRegion(arrow, 0, 0, 13, 16, 2, waypoint.maxX + 12 + runArrow, waypoint.maxY - 12, StaticObj.VCENTER_HCENTER);
						}
						else
						{
							g.drawRegion(arrow, 0, 0, 13, 16, 2, waypoint.maxX + 12 + runArrow, waypoint.maxY - 32, StaticObj.VCENTER_HCENTER);
						}
					}
					else if (waypoint.minX <= TileMap.tmw * 24 && waypoint.minX >= TileMap.tmw * 24 - 48)
					{
						if (!GameCanvas.isTouch)
						{
							g.drawRegion(arrow, 0, 0, 13, 16, 0, waypoint.minX - 12 - runArrow, waypoint.maxY - 12, StaticObj.VCENTER_HCENTER);
						}
						else
						{
							g.drawRegion(arrow, 0, 0, 13, 16, 0, waypoint.minX - 12 - runArrow, waypoint.maxY - 32, StaticObj.VCENTER_HCENTER);
						}
					}
					else
					{
						g.drawRegion(arrow, 0, 0, 13, 16, 4, waypoint.minX + (waypoint.maxX - waypoint.minX) / 2, waypoint.maxY - 48 - runArrow, StaticObj.VCENTER_HCENTER);
					}
				}
			}
	private void paintArrowPointToNPC(mGraphics g)
			{
				try
				{
					if (ChatPopup.currChatPopup != null)
					{
						return;
					}
					int num = getTaskNpcId();
					if (num == -1)
					{
						return;
					}
					Npc npc = null;
					for (int i = 0; i < vNpc.size(); i++)
					{
						Npc npc2 = (Npc)vNpc.elementAt(i);
						if (npc2.template.npcTemplateId == num)
						{
							if (npc == null)
							{
								npc = npc2;
							}
							else if (Res.abs(npc2.cx - Char.myCharz().cx) < Res.abs(npc.cx - Char.myCharz().cx))
							{
								npc = npc2;
							}
						}
					}
					if (npc == null || npc.statusMe == 15 || (npc.cx > cmx && npc.cx < cmx + gW && npc.cy > cmy && npc.cy < cmy + gH) || GameCanvas.gameTick % 10 < 5)
					{
						return;
					}
					int num2 = npc.cx - Char.myCharz().cx;
					int num3 = npc.cy - Char.myCharz().cy;
					int x = 0;
					int y = 0;
					int arg = 0;
					if (num2 > 0 && num3 >= 0)
					{
						if (Res.abs(num2) >= Res.abs(num3))
						{
							x = gW - 10;
							y = gH / 2 + 30;
							if (GameCanvas.isTouch)
							{
								y = gH / 2 + 10;
							}
							arg = 0;
						}
						else
						{
							x = gW / 2;
							y = gH - 10;
							arg = 5;
						}
					}
					else if (num2 >= 0 && num3 < 0)
					{
						if (Res.abs(num2) >= Res.abs(num3))
						{
							x = gW - 10;
							y = gH / 2 + 30;
							if (GameCanvas.isTouch)
							{
								y = gH / 2 + 10;
							}
							arg = 0;
						}
						else
						{
							x = gW / 2;
							y = 10;
							arg = 6;
						}
					}
					if (num2 < 0 && num3 >= 0)
					{
						if (Res.abs(num2) >= Res.abs(num3))
						{
							x = 10;
							y = gH / 2 + 30;
							if (GameCanvas.isTouch)
							{
								y = gH / 2 + 10;
							}
							arg = 3;
						}
						else
						{
							x = gW / 2;
							y = gH - 10;
							arg = 5;
						}
					}
					else if (num2 <= 0 && num3 < 0)
					{
						if (Res.abs(num2) >= Res.abs(num3))
						{
							x = 10;
							y = gH / 2 + 30;
							if (GameCanvas.isTouch)
							{
								y = gH / 2 + 10;
							}
							arg = 3;
						}
						else
						{
							x = gW / 2;
							y = 10;
							arg = 6;
						}
					}
					resetTranslate(g);
					g.drawRegion(arrow, 0, 0, 13, 16, arg, x, y, StaticObj.VCENTER_HCENTER);
				}
				catch (Exception ex)
				{
					Cout.LogError("Loi ham arrow to npc: " + ex.ToString());
				}
			}

}
