using System;

public static class ModWaypoint
{
	public static int GetGroundY(int x, int minY, int maxY)
	{
		try
		{
			if (TileMap.isInAirMap() || TileMap.mapID == 47 || TileMap.mapID == 45 || TileMap.mapID == 46 || TileMap.mapID == 48)
			{
				return (minY + maxY) / 2;
			}

			// Quét từ maxY lên minY tìm vị trí đất va chạm
			for (int y = maxY; y >= minY; y -= 4)
			{
				if (TileMap.tileTypeAt(x, y, 2))
				{
					return y;
				}
			}
			// Quét toàn bộ chiều cao map nếu waypoint bao trọn trục Y
			for (int y = TileMap.pxh - 12; y >= 24; y -= 12)
			{
				if (TileMap.tileTypeAt(x, y, 2))
				{
					return y;
				}
			}
			return (maxY > 24) ? (maxY - 12) : ((minY + maxY) / 2);
		}
		catch
		{
			return (minY + maxY) / 2;
		}
	}

	public static bool StepToWaypoint(Waypoint wp)
	{
		try
		{
			Char me = Char.myCharz();
			if (me == null || wp == null)
			{
				return false;
			}

			// 1. Tính toạ độ X an toàn tuyệt đối nằm gọn trong vùng Waypoint
			int targetX = (wp.minX + wp.maxX) / 2;
			if (wp.minX <= 24)
			{
				targetX = wp.minX + 15;
			}
			else if (wp.maxX >= TileMap.pxw - 24)
			{
				targetX = wp.maxX - 15;
			}

			// 2. Tính toạ độ Y an toàn (chân tiếp xúc mặt đất hoặc trung tâm cổng)
			int targetY = me.cy;
			if (wp.maxY - wp.minY <= 60)
			{
				targetY = (wp.minY + wp.maxY) / 2;
			}
			else
			{
				int groundY = GetGroundY(targetX, wp.minY, wp.maxY);
				if (groundY >= wp.minY && groundY <= wp.maxY)
				{
					targetY = groundY;
				}
				else if (me.cy >= wp.minY && me.cy <= wp.maxY)
				{
					targetY = me.cy;
				}
				else
				{
					targetY = (wp.minY + wp.maxY) / 2;
				}
			}

			// Ràng buộc Y không bị lọt ra ngoài hitbox của cổng
			if (targetY < wp.minY + 2) targetY = wp.minY + 2;
			if (targetY > wp.maxY - 2) targetY = wp.maxY - 2;

			// 3. Đặt nhân vật trực tiếp vào tâm cổng và đồng bộ với Server
			me.vMovePoints.removeAllElements();
			me.currentMovePoint = null;
			me.cx = targetX;
			me.cy = targetY;
			me.cvx = 0;
			me.cvy = 0;
			me.statusMe = 1;
			me.delayFall = 0;

			int dx = targetX - me.cx;
			me.cdir = (targetX > TileMap.pxw / 2) ? 1 : -1;

			// Gửi gói tin cập nhật vị trí nguyên tử lên server
			Service.gI().charMoveTo(targetX, targetY);

			// 4. Gửi gói tin yêu cầu chuyển map thực
			if (wp.isOffline || TileMap.isTrainingMap())
			{
				Service.gI().getMapOffline();
			}
			else
			{
				Service.gI().requestChangeMap();
			}

			Char.isLockKey = true;
			Char.ischangingMap = true;
			GameCanvas.clearKeyHold();
			GameCanvas.clearKeyPressed();
			InfoDlg.showWait();

			return true;
		}
		catch
		{
			return false;
		}
	}

	public static void UseSpaceShip(Npc shipNpc, int targetPlanetMapId = -1)
	{
		try
		{
			Char me = Char.myCharz();
			if (me == null || shipNpc == null)
			{
				return;
			}

			me.vMovePoints.removeAllElements();
			me.currentMovePoint = null;
			me.cx = shipNpc.cx;
			me.cy = shipNpc.cy;
			me.cvx = 0;
			me.cvy = 0;
			me.statusMe = 1;

			Service.gI().charMoveTo(shipNpc.cx, shipNpc.cy);
			Service.gI().openMenu(shipNpc.npcId);

			int menuIndex = 0;
			if (TileMap.mapID == 24) // Trái Đất -> Namếc (25) = 0, Xayda (26) = 1
			{
				menuIndex = (targetPlanetMapId == 26) ? 1 : 0;
			}
			else if (TileMap.mapID == 25) // Namếc -> Trái Đất (24) = 0, Xayda (26) = 1
			{
				menuIndex = (targetPlanetMapId == 26) ? 1 : 0;
			}
			else if (TileMap.mapID == 26) // Xayda -> Trái Đất (24) = 0, Namếc (25) = 1
			{
				menuIndex = (targetPlanetMapId == 25) ? 1 : 0;
			}

			Service.gI().confirmMenu((short)shipNpc.npcId, (sbyte)menuIndex);
			Char.isLockKey = true;
			Char.ischangingMap = true;
			GameCanvas.clearKeyHold();
			GameCanvas.clearKeyPressed();
			InfoDlg.showWait();
		}
		catch
		{
		}
	}
}
