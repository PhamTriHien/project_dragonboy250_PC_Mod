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
			return (minY + maxY) / 2;
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
				targetX = wp.minX + 12;
			}
			else if (wp.maxX >= TileMap.pxw - 24)
			{
				targetX = wp.maxX - 12;
			}

			// 2. Tính toạ độ Y an toàn (giữ nguyên độ cao bay/đứng nếu đã nằm trong cổng, hoặc lấy trung tâm/mặt đất)
			int targetY = me.cy;
			if (TileMap.isInAirMap() || TileMap.mapID == 45 || TileMap.mapID == 46 || TileMap.mapID == 47 || TileMap.mapID == 48)
			{
				targetY = (wp.minY + wp.maxY) / 2;
			}
			else if (wp.isEnter || wp.isOffline)
			{
				int groundY = GetGroundY(targetX, wp.minY, wp.maxY);
				targetY = (groundY >= wp.minY && groundY <= wp.maxY) ? groundY : ((wp.minY + wp.maxY) / 2);
			}
			else
			{
				if (me.cy >= wp.minY && me.cy <= wp.maxY)
				{
					targetY = me.cy;
				}
				else
				{
					int groundY = GetGroundY(targetX, wp.minY, wp.maxY);
					targetY = (groundY >= wp.minY && groundY <= wp.maxY) ? groundY : ((wp.minY + wp.maxY) / 2);
				}
			}

			// Ràng buộc Y không bị lọt ra ngoài hitbox của cổng
			if (targetY < wp.minY + 2) targetY = wp.minY + 2;
			if (targetY > wp.maxY - 2) targetY = wp.maxY - 2;

			// Tính khoảng cách từ vị trí hiện tại tới điểm đích Waypoint
			int dist = Res.distance(me.cx, me.cy, targetX, targetY);

			// GIAI ĐOẠN 1: Nếu nhân vật còn ở xa (> 30px), đồng bộ vị trí tới Waypoint trước
			if (dist > 30)
			{
				me.vMovePoints.removeAllElements();
				me.currentMovePoint = null;
				me.cx = targetX;
				me.cy = targetY;
				me.cvx = 0;
				me.cvy = 0;
				me.statusMe = 1;
				me.delayFall = 0;
				me.cdir = (targetX > TileMap.pxw / 2) ? 1 : -1;

				// Gửi gói tin cập nhật toạ độ nguyên tử lên server
				Service.gI().charMoveTo(targetX, targetY);
				return false; // Nhường tick cho server cập nhật vị trí trước khi gửi requestChangeMap
			}

			// GIAI ĐOẠN 2: Nhân vật đã đứng gọn trong Waypoint (dist <= 30px) -> Kích hoạt qua Map
			me.cx = targetX;
			me.cy = targetY;
			me.cvx = 0;
			me.cvy = 0;
			me.statusMe = 1;
			me.delayFall = 0;
			me.cdir = (targetX > TileMap.pxw / 2) ? 1 : -1;

			// Gửi gói tin di chuyển xác thực
			Service.gI().charMove();

			if (wp.isOffline || TileMap.isTrainingMap())
			{
				Service.gI().getMapOffline();
			}
			else if (wp.isEnter)
			{
				if (wp.popup != null && wp.popup.command != null)
				{
					wp.popup.command.performAction();
				}
				else
				{
					Service.gI().requestChangeMap();
				}
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

	public static bool isWaitingShipMenu;
	public static int pendingTargetPlanetMapId = -1;
	public static long lastShipOpenMenuTime;

	public static void UseSpaceShip(Npc shipNpc, int targetPlanetMapId = -1)
	{
		try
		{
			Char me = Char.myCharz();
			if (me == null || shipNpc == null)
			{
				return;
			}

			int dist = Res.distance(me.cx, me.cy, shipNpc.cx, shipNpc.cy);
			if (dist > 30)
			{
				me.vMovePoints.removeAllElements();
				me.currentMovePoint = null;
				me.cx = shipNpc.cx;
				me.cy = shipNpc.cy;
				me.cvx = 0;
				me.cvy = 0;
				me.statusMe = 1;

				Service.gI().charMoveTo(shipNpc.cx, shipNpc.cy);
				return;
			}

			me.focusManualTo(shipNpc);
			int npcTemplateId = (shipNpc.template != null) ? shipNpc.template.npcTemplateId : shipNpc.npcId;

			long now = mSystem.currentTimeMillis();
			if (isWaitingShipMenu && now - lastShipOpenMenuTime < 2500)
			{
				return;
			}

			isWaitingShipMenu = true;
			pendingTargetPlanetMapId = targetPlanetMapId;
			lastShipOpenMenuTime = now;

			// Mở menu NPC Tàu vũ trụ, chờ server gửi danh sách menu
			Service.gI().openMenu(npcTemplateId);
		}
		catch
		{
		}
	}

	public static void OnReceiveShipMenu(string[] menuItems, Npc npc)
	{
		try
		{
			if (!isWaitingShipMenu || menuItems == null || menuItems.Length == 0)
			{
				return;
			}

			isWaitingShipMenu = false;
			int targetPlanet = pendingTargetPlanetMapId;
			int selectedIndex = -1;

			// Tìm option dựa trên tên hành tinh trong menu thực tế trả về từ server
			for (int i = 0; i < menuItems.Length; i++)
			{
				string text = menuItems[i].ToLower();
				if (targetPlanet == 25 && (text.Contains("nam") || text.Contains("namec")))
				{
					selectedIndex = i;
					break;
				}
				if (targetPlanet == 26 && (text.Contains("xay") || text.Contains("say") || text.Contains("sai")))
				{
					selectedIndex = i;
					break;
				}
				if (targetPlanet == 24 && (text.Contains("trái") || text.Contains("trai") || text.Contains("earth") || text.Contains("đất")))
				{
					selectedIndex = i;
					break;
				}
			}

			// Fallback theo chỉ mục mặc định nếu không tìm thấy theo keyword
			if (selectedIndex == -1)
			{
				if (TileMap.mapID == 24)
				{
					selectedIndex = (targetPlanet == 26) ? 1 : 0;
				}
				else if (TileMap.mapID == 25)
				{
					selectedIndex = (targetPlanet == 26) ? 1 : 0;
				}
				else if (TileMap.mapID == 26)
				{
					selectedIndex = (targetPlanet == 25) ? 1 : 0;
				}
				else
				{
					selectedIndex = 0;
				}
			}

			if (selectedIndex >= menuItems.Length)
			{
				selectedIndex = 0;
			}

			int npcTemplateId = (npc != null && npc.template != null) ? npc.template.npcTemplateId : ((npc != null) ? npc.npcId : 10);
			Service.gI().confirmMenu((short)npcTemplateId, (sbyte)selectedIndex);

			GameCanvas.menu.showMenu = false;
			Char.isLockKey = true;
			Char.ischangingMap = true;
			GameCanvas.clearKeyHold();
			GameCanvas.clearKeyPressed();
			InfoDlg.showWait();
		}
		catch
		{
			isWaitingShipMenu = false;
		}
	}
}
