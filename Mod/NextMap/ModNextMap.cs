using System;
using System.Collections.Generic;

public static class ModNextMap
{
	public static int selectedPlanetTab = 0;
	public static bool isNextMapActive = false;
	public static int nextMapTargetId = -1;
	public static int lastMapId = -1;
	private static int nextMapCooldown = 0;
	private static int nextMapFailCount = 0;
	private static long lastChangeAttemptTime = 0;

	// Backward-compatible accessors to data
	public static int[][] planetMapIds
	{
		get { return ModNextMapData.planetMapIds; }
	}

	public static string GetMapName(int id)
	{
		return ModNextMapData.GetMapName(id);
	}

	public static bool MatchMapName(string wpName, string mapName)
	{
		return ModNextMapData.MatchMapName(wpName, mapName);
	}

	public static List<int> FindPath(int startMapId, int targetMapId)
	{
		return ModNextMapPathFinder.FindPath(startMapId, targetMapId);
	}

	public static void StartNextMap(int targetId)
	{
		if (TileMap.mapID == targetId)
		{
			GameScr.info1.addInfo("Bạn đang ở " + GetMapName(targetId) + " rồi!", 0);
			return;
		}

		nextMapTargetId = targetId;
		isNextMapActive = true;
		nextMapCooldown = 0;
		nextMapFailCount = 0;
		lastMapId = TileMap.mapID;
		lastChangeAttemptTime = 0;
		Char.ischangingMap = false;
		Char.isLockKey = false;
		InfoDlg.hide();
		GameCanvas.endDlg();

		GameScr.info1.addInfo("Bắt đầu Next Map đến: " + GetMapName(targetId), 0);
	}

	public static void StopNextMap()
	{
		isNextMapActive = false;
		nextMapTargetId = -1;
		nextMapCooldown = 0;
		nextMapFailCount = 0;
		lastChangeAttemptTime = 0;
		Char.ischangingMap = false;
		Char.isLockKey = false;
		InfoDlg.hide();
		GameCanvas.endDlg();
		Char me = Char.myCharz();
		if (me != null)
		{
			me.isLockAttack = false;
			me.isLockMove = false;
		}
	}

	public static Waypoint FindWaypointToMap(int nextMapId)
	{
		if (TileMap.vGo == null || TileMap.vGo.size() == 0)
		{
			return null;
		}

		string targetMapName = GetMapName(nextMapId);

		// 1. So khớp tên cổng chính xác (bao gồm cả popup says)
		for (int j = 0; j < TileMap.vGo.size(); j++)
		{
			Waypoint wp = (Waypoint)TileMap.vGo.elementAt(j);
			if (wp != null)
			{
				string fullName = wp.name ?? string.Empty;
				if (wp.popup != null && wp.popup.says != null)
				{
					for (int s = 0; s < wp.popup.says.Length; s++)
					{
						fullName += " " + wp.popup.says[s];
					}
				}
				if (MatchMapName(fullName, targetMapName))
				{
					return wp;
				}
			}
		}

		// 2. Xử lý các map đặc biệt: Nhà đẻ (21, 22, 23)
		if (nextMapId == 21 || nextMapId == 22 || nextMapId == 23)
		{
			for (int j = 0; j < TileMap.vGo.size(); j++)
			{
				Waypoint wp = (Waypoint)TileMap.vGo.elementAt(j);
				if (wp != null && (wp.isOffline || wp.isEnter))
				{
					return wp;
				}
			}
		}
		if (TileMap.mapID == 21 || TileMap.mapID == 22 || TileMap.mapID == 23)
		{
			return (Waypoint)TileMap.vGo.elementAt(0);
		}

		// 3. Xử lý Vách núi (42, 43, 44)
		if (nextMapId == 42 || nextMapId == 43 || nextMapId == 44)
		{
			for (int j = 0; j < TileMap.vGo.size(); j++)
			{
				Waypoint wp = (Waypoint)TileMap.vGo.elementAt(j);
				if (wp != null && (wp.minX < 200 || (wp.name != null && (wp.name.ToLower().Contains("vách") || wp.name.ToLower().Contains("núi")))))
				{
					return wp;
				}
			}
		}

		// 4. Tuyến đường thẳng nối tiếp: chọn cổng theo hướng tọa độ X
		if (TileMap.vGo.size() == 1)
		{
			return (Waypoint)TileMap.vGo.elementAt(0);
		}

		Waypoint bestWp = null;
		if (nextMapId > TileMap.mapID)
		{
			int maxRight = int.MinValue;
			for (int k = 0; k < TileMap.vGo.size(); k++)
			{
				Waypoint wp2 = (Waypoint)TileMap.vGo.elementAt(k);
				if (wp2 != null && wp2.maxX > maxRight)
				{
					maxRight = wp2.maxX;
					bestWp = wp2;
				}
			}
		}
		else
		{
			int minLeft = int.MaxValue;
			for (int k = 0; k < TileMap.vGo.size(); k++)
			{
				Waypoint wp2 = (Waypoint)TileMap.vGo.elementAt(k);
				if (wp2 != null && wp2.minX < minLeft)
				{
					minLeft = wp2.minX;
					bestWp = wp2;
				}
			}
		}

		return bestWp ?? ((Waypoint)TileMap.vGo.elementAt(0));
	}

	public static void UpdateNextMap()
	{
		if (!isNextMapActive || nextMapTargetId == -1)
		{
			return;
		}

		Char me = Char.myCharz();
		if (me == null)
		{
			return;
		}

		if (Char.isLoadingMap)
		{
			return;
		}

		// Nhận diện khi đã sang map mới thành công
		if (TileMap.mapID != lastMapId)
		{
			lastMapId = TileMap.mapID;
			Char.ischangingMap = false;
			Char.isLockKey = false;
			me.isLockAttack = false;
			me.isLockMove = false;
			InfoDlg.hide();
			GameCanvas.endDlg();
			nextMapCooldown = 15;
			lastChangeAttemptTime = 0;
		}

		// Kiểm tra đến map đích
		if (TileMap.mapID == nextMapTargetId)
		{
			GameScr.info1.addInfo("ĐÃ ĐẾN: " + GetMapName(nextMapTargetId) + "!", 0);
			SoundMn.gI().buttonClose();
			StopNextMap();
			return;
		}

		// Watchdog chống kẹt trạng thái ischangingMap khi server rớt gói tin hoặc phản hồi chậm
		if (Char.ischangingMap)
		{
			if (lastChangeAttemptTime > 0 && mSystem.currentTimeMillis() - lastChangeAttemptTime > 1800)
			{
				Char.ischangingMap = false;
				Char.isLockKey = false;
				me.isLockAttack = false;
				me.isLockMove = false;
				InfoDlg.hide();
				GameCanvas.endDlg();
				lastChangeAttemptTime = 0;
				nextMapCooldown = 10;
				nextMapFailCount++;
			}
			return;
		}

		if (nextMapCooldown > 0)
		{
			nextMapCooldown--;
			return;
		}

		List<int> path = FindPath(TileMap.mapID, nextMapTargetId);
		if (path == null || path.Count == 0)
		{
			GameScr.info1.addInfo("Không tìm thấy đường đi đến " + GetMapName(nextMapTargetId), 0);
			StopNextMap();
			return;
		}

		int nextMapId = path[0];

		// Trường hợp chuyển trạm tàu vũ trụ (map 24, 25, 26)
		if ((TileMap.mapID == 24 || TileMap.mapID == 25 || TileMap.mapID == 26) &&
			(nextMapId == 24 || nextMapId == 25 || nextMapId == 26))
		{
			Npc shipNpc = null;
			for (int i = 0; i < GameScr.vNpc.size(); i++)
			{
				Npc npc = (Npc)GameScr.vNpc.elementAt(i);
				if (npc != null && (npc.template.npcTemplateId == 10 || npc.template.npcTemplateId == 11 || npc.template.npcTemplateId == 12))
				{
					shipNpc = npc;
					break;
				}
			}
			if (shipNpc != null)
			{
				nextMapFailCount = 0;
				ModWaypoint.UseSpaceShip(shipNpc, nextMapId);
				lastChangeAttemptTime = mSystem.currentTimeMillis();
				nextMapCooldown = 50;
				return;
			}
		}

		// Tìm Waypoint dẫn sang map tiếp theo
		Waypoint targetWp = FindWaypointToMap(nextMapId);

		if (targetWp != null)
		{
			nextMapFailCount = 0;
			bool sentRequest = ModWaypoint.StepToWaypoint(targetWp);
			if (sentRequest)
			{
				lastChangeAttemptTime = mSystem.currentTimeMillis();
				nextMapCooldown = 30;
			}
		}
		else
		{
			nextMapFailCount++;
			if (nextMapFailCount > 8)
			{
				GameScr.info1.addInfo("Không tìm thấy cổng sang " + GetMapName(nextMapId), 0);
				StopNextMap();
			}
			else
			{
				nextMapCooldown = 15;
			}
		}
	}
}
