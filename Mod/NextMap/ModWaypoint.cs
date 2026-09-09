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

			// 1. Quét tìm mặt đất trong phạm vi waypoint [minY, maxY]
			for (int y = maxY; y >= minY; y -= 4)
			{
				if (TileMap.tileTypeAt(x, y, 2))
				{
					int gy = TileMap.tileYofPixel(y);
					if (gy >= minY && gy <= maxY)
					{
						return gy;
					}
					return (y <= maxY) ? y : maxY;
				}
			}

			// 2. Nếu tại x chưa thấy đất, thử quét các điểm x lân cận trong cùng waypoint
			int[] xOffsets = new int[] { 8, -8, 16, -16 };
			for (int i = 0; i < xOffsets.Length; i++)
			{
				int testX = x + xOffsets[i];
				for (int y = maxY; y >= minY; y -= 4)
				{
					if (TileMap.tileTypeAt(testX, y, 2))
					{
						int gy = TileMap.tileYofPixel(y);
						if (gy >= minY && gy <= maxY)
						{
							return gy;
						}
						return (y <= maxY) ? y : maxY;
					}
				}
			}

			// 3. Quét toàn bộ chiều cao map từ dưới lên để tìm mặt đất chuẩn của cột X
			for (int y = TileMap.pxh - 12; y >= 24; y -= 12)
			{
				if (TileMap.tileTypeAt(x, y, 2))
				{
					int gy = TileMap.tileYofPixel(y);
					if (gy >= minY && gy <= maxY)
					{
						return gy;
					}
					if (gy >= maxY && gy <= maxY + 24)
					{
						return maxY;
					}
					break;
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

			// Luôn xóa cờ entranceWaypoint để không bị vô hiệu hóa cổng đích
			Char.entranceWaypoint = null;

			// 1. Tính toạ độ X an toàn tuyệt đối nằm gọn trong vùng Waypoint
			int targetX = (wp.minX + wp.maxX) / 2;
			if (wp.minX <= 24)
			{
				if (wp.maxX >= 48)
				{
					if (targetX < 24) targetX = 24;
					if (targetX > wp.maxX - 8) targetX = wp.maxX - 8;
				}
				else
				{
					if (targetX < wp.minX + 4) targetX = wp.minX + 4;
					if (targetX > wp.maxX - 4) targetX = wp.maxX - 4;
				}
			}
			else if (wp.maxX >= TileMap.pxw - 24)
			{
				if (wp.minX <= TileMap.pxw - 48)
				{
					if (targetX > TileMap.pxw - 24) targetX = TileMap.pxw - 24;
					if (targetX < wp.minX + 8) targetX = wp.minX + 8;
				}
				else
				{
					if (targetX < wp.minX + 4) targetX = wp.minX + 4;
					if (targetX > wp.maxX - 4) targetX = wp.maxX - 4;
				}
			}
			else
			{
				if (targetX < wp.minX + 4) targetX = wp.minX + 4;
				if (targetX > wp.maxX - 4) targetX = wp.maxX - 4;
			}

			// 2. Tính toạ độ Y an toàn (chạm sàn đất T_TOP hoặc trung tâm cổng)
			int targetY;
			if (TileMap.isInAirMap() || TileMap.mapID == 45 || TileMap.mapID == 46 || TileMap.mapID == 47 || TileMap.mapID == 48)
			{
				targetY = (wp.minY + wp.maxY) / 2;
			}
			else
			{
				int groundY = GetGroundY(targetX, wp.minY, wp.maxY);
				targetY = (groundY >= wp.minY && groundY <= wp.maxY) ? groundY : ((wp.minY + wp.maxY) / 2);
			}

			// Ràng buộc Y nằm gọn trong hitbox của cổng, TUYỆT ĐỐI KHÔNG trừ 2px làm rơi vào không trung
			if (targetY < wp.minY) targetY = wp.minY;
			if (targetY > wp.maxY) targetY = wp.maxY;

			// Xóa sạch trạng thái di chuyển cũ
			me.vMovePoints.removeAllElements();
			me.currentMovePoint = null;
			me.endMovePointCommand = null;

			// Đặt nhân vật trực tiếp vào tâm cổng và đồng bộ với Server
			me.cx = targetX;
			me.cy = targetY;
			me.cvx = 0;
			me.cvy = 0;
			me.statusMe = 1; // Đứng yên vững chắc trên mặt đất
			me.delayFall = 0;
			me.cdir = (targetX > TileMap.pxw / 2) ? 1 : -1;

			// Gửi gói tin cập nhật toạ độ nguyên tử lên server
			Service.gI().charMoveTo(targetX, targetY);

			// Gửi gói tin yêu cầu chuyển map thực
			if (wp.isOffline || TileMap.isTrainingMap())
			{
				Service.gI().getMapOffline();
			}
			else
			{
				// Cả cổng thông thường và cổng isEnter đều gửi requestChangeMap
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
