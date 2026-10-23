using System;

/// <summary>
/// Module Tọa Độ Cổng Chuyển Map: Quản lý dịch chuyển đến các Waypoint và tương tác trạm tàu vũ trụ.
/// </summary>
public static class ModWaypoint
{
	public static int edgeWalkAttempts = 0;
	public static bool isWaitingShipMenu;
	public static int pendingTargetPlanetMapId = -1;
	public static long lastShipOpenMenuTime;
	private static Waypoint activeWp;
	private static long wpInitTime;
	private static bool hasTeleportedToWp;
	private static long lastTeleportTime;
	private static bool isAirBottomActive;

	/// <summary>
	/// Đặt lại trạng thái theo dõi Waypoint khi đổi map hoặc khởi động lại lộ trình.
	/// </summary>
	public static void ResetWaypointState()
	{
		activeWp = null;
		wpInitTime = 0;
		hasTeleportedToWp = false;
		lastTeleportTime = 0;
		edgeWalkAttempts = 0;
		isAirBottomActive = false;
		isWaitingShipMenu = false;
		pendingTargetPlanetMapId = -1;
	}

	/// <summary>
	/// Tìm tọa độ mặt đất hợp lệ nằm gọn trong khoảng [minY, maxY] của cổng dịch chuyển.
	/// </summary>
	public static int GetGroundY(int x, int minY, int maxY)
	{
		try
		{
			if (TileMap.isInAirMap() || TileMap.mapID == 47 || TileMap.mapID == 45 || TileMap.mapID == 46 || TileMap.mapID == 48)
			{
				return (minY + maxY) / 2;
			}

			int safeMinY = System.Math.Max(12, minY);
			int safeMaxY = System.Math.Min(TileMap.pxh - 12, maxY);
			if (safeMaxY < safeMinY)
			{
				return maxY;
			}

			// Quét từ đáy cổng lên đỉnh cổng để luôn lấy đúng mặt đất tầng dưới (tránh dính vách đá/mái nhà phía trên)
			int searchStart = System.Math.Min(TileMap.pxh - 12, safeMaxY + 24);
			for (int y = searchStart; y >= safeMinY; y -= 12)
			{
				if (TileMap.tileTypeAt(x, y, 2))
				{
					int gy = TileMap.tileYofPixel(y);
					int guard = 0;
					while (gy >= 24 && guard < 6 && TileMap.tileTypeAt(x, gy - 24, 2))
					{
						gy -= 24;
						guard++;
					}
					if (gy >= minY && gy <= maxY)
					{
						return gy;
					}
					if (gy > maxY && gy - maxY <= 24)
					{
						return maxY;
					}
				}
			}

			return maxY;
		}
		catch
		{
			return maxY;
		}
	}

	/// <summary>
	/// Dịch chuyển nhân vật đến tọa độ đích và đồng bộ gói tin charMove lên máy chủ.
	/// </summary>
	public static void TeleportTo(int x, int y)
	{
		try
		{
			Char me = Char.myCharz();
			if (me == null)
			{
				return;
			}
			me.vMovePoints.removeAllElements();
			me.currentMovePoint = null;
			me.endMovePointCommand = null;
			me.cvx = 0;
			me.cvy = 0;
			me.statusMe = 1;
			me.delayFall = 0;
			me.cyStartFall = 0;
			me.isFlyUp = false;

			me.cx = x;
			me.cy = y;
			me.cxSend = x - 1;
			me.cySend = y - 1;
			Service.gI().charMove();
			if (!ItemTime.isExistItem(4387))
			{
				ModTeleport.RegisterTeleportEcho(x, y);
				me.cx = x;
				me.cy = y + 1;
				Service.gI().charMove();
				me.cx = x;
				me.cy = y;
				Service.gI().charMove();
			}
		}
		catch
		{
		}
	}

	/// <summary>
	/// Dịch chuyển theo từng nhịp mặt đất an toàn đến tọa độ đích khi bật chế độ an toàn hoặc sau khi bị server reset point.
	/// </summary>
	public static bool MoveSteppedTo(int targetX, int targetY)
	{
		Char me = Char.myCharz();
		if (me == null)
		{
			return false;
		}

		int dx = Res.abs(me.cx - targetX);
		int dy = Res.abs(me.cy - targetY);
		int maxStepX = 160;

		if ((ModNextMap.useSafeMovement || ModNextMap.resetPenaltyCount > 0) && dx > maxStepX)
		{
			int nextX = me.cx + ((targetX > me.cx) ? maxStepX : (-maxStepX));
			int nextY;
			if (TileMap.isInAirMap() || TileMap.mapID == 45 || TileMap.mapID == 46 || TileMap.mapID == 47 || TileMap.mapID == 48)
			{
				nextY = targetY;
			}
			else
			{
				int minSearchY = System.Math.Max(24, System.Math.Min(me.cy, targetY) - 96);
				int maxSearchY = System.Math.Min(TileMap.pxh - 12, System.Math.Max(me.cy, targetY) + 96);
				nextY = GetGroundY(nextX, minSearchY, maxSearchY);
			}
			me.cdir = (targetX >= me.cx) ? 1 : (-1);
			TeleportTo(nextX, nextY);
			return false;
		}

		if (dx > 20 || dy > 20)
		{
			me.cdir = (targetX >= me.cx) ? 1 : (-1);
			TeleportTo(targetX, targetY);
			return false;
		}

		if (me.cx != targetX || me.cy != targetY || me.cxSend != targetX || me.cySend != targetY)
		{
			TeleportTo(targetX, targetY);
		}
		return true;
	}

	/// <summary>
	/// Tính toán tọa độ X nằm gọn bên trong vùng [wp.minX, wp.maxX] và biên bản đồ an toàn.
	/// </summary>
	public static int GetWaypointTargetX(Waypoint wp)
	{
		if (wp == null)
		{
			return 24;
		}

		int minX = wp.minX;
		int maxX = wp.maxX;
		int center = (minX + maxX) / 2;

		if (maxX <= 60)
		{
			int target = System.Math.Max(12, minX + 12);
			if (target > maxX - 2)
			{
				target = center;
			}
			return System.Math.Max(6, target);
		}

		if (minX >= TileMap.pxw - 60)
		{
			int target = System.Math.Min(TileMap.pxw - 12, maxX - 12);
			if (target < minX + 2)
			{
				target = center;
			}
			return System.Math.Min(TileMap.pxw - 6, target);
		}

		return center;
	}

	/// <summary>
	/// Tính toán tọa độ Y nằm gọn bên trong vùng [wp.minY, wp.maxY] của cổng dịch chuyển.
	/// </summary>
	public static int GetWaypointTargetY(Waypoint wp, int targetX)
	{
		if (wp == null)
		{
			return 24;
		}

		bool isAirOrTower = TileMap.isInAirMap() || TileMap.mapID == 45 || TileMap.mapID == 46 || TileMap.mapID == 47 || TileMap.mapID == 48;
		if (isAirOrTower)
		{
			return (wp.maxY > wp.minY + 24) ? ((wp.minY + wp.maxY) / 2) : wp.maxY;
		}

		return GetGroundY(targetX, wp.minY, wp.maxY);
	}

	/// <summary>
	/// Thực thi dịch chuyển vào đúng tâm cổng Waypoint và gửi yêu cầu chuyển map lên Server.
	/// </summary>
	public static bool StepToWaypoint(Waypoint wp)
	{
		try
		{
			Char me = Char.myCharz();
			if (me == null || wp == null)
			{
				return false;
			}

			if (GameCanvas.menu != null && GameCanvas.menu.showMenu)
			{
				GameCanvas.menu.showMenu = false;
			}

			if (Char.entranceWaypoint == wp)
			{
				Char.entranceWaypoint = null;
			}

			long now = mSystem.currentTimeMillis();
			if (activeWp != wp || isAirBottomActive)
			{
				activeWp = wp;
				isAirBottomActive = false;
				wpInitTime = now;
				hasTeleportedToWp = false;
				lastTeleportTime = 0;
				edgeWalkAttempts = 0;
			}

			if (hasTeleportedToWp && now - lastTeleportTime < 140)
			{
				return false;
			}

			bool isLeftEdge = wp.maxX <= 60;
			bool isRightEdge = !isLeftEdge && wp.minX >= TileMap.pxw - 60;

			int wpTargetX = GetWaypointTargetX(wp);
			int wpTargetY = GetWaypointTargetY(wp, wpTargetX);

			int dx = Res.abs(me.cx - wpTargetX);
			int dy = Res.abs(me.cy - wpTargetY);
			me.cdir = isLeftEdge ? (-1) : (isRightEdge ? 1 : ((me.cx <= wpTargetX) ? 1 : (-1)));

			if (!hasTeleportedToWp || dx > 24 || dy > 24)
			{
				if (ModNextMap.useSafeMovement || ModNextMap.resetPenaltyCount > 0)
				{
					if (MoveSteppedTo(wpTargetX, wpTargetY))
					{
						hasTeleportedToWp = true;
					}
				}
				else
				{
					TeleportTo(wpTargetX, wpTargetY);
					hasTeleportedToWp = true;
				}
				lastTeleportTime = now;
				wpInitTime = now;
				return false;
			}

			// Đảm bảo nhân vật đứng chính xác trong lòng cổng trước khi gửi gói chuyển map
			if (me.cx != wpTargetX || me.cy != wpTargetY || me.cxSend != wpTargetX || me.cySend != wpTargetY)
			{
				TeleportTo(wpTargetX, wpTargetY);
			}

			if (wp.isOffline || TileMap.isOfflineMap() || TileMap.isTrainingMap())
			{
				Char.ischangingMap = true;
				Service.gI().getMapOffline();
			}
			else
			{
				Char.ischangingMap = true;
				Service.gI().requestChangeMap();
			}
			ResetWaypointState();
			return true;
		}
		catch
		{
			return false;
		}
	}

	/// <summary>
	/// Chuyển map xuống dưới trên các bản đồ trên không (Thần điện 45 -> Tháp Karin 46 -> Rừng Karin 47, Hành tinh Kaio 48 -> Thần điện 45)
	/// nơi Waypoint.cs đã lọc bỏ cổng dưới đáy (minY > 150) khỏi TileMap.vGo.
	/// </summary>
	public static bool StepToAirMapBottom()
	{
		try
		{
			Char me = Char.myCharz();
			if (me == null)
			{
				return false;
			}

			if (GameCanvas.menu != null && GameCanvas.menu.showMenu)
			{
				GameCanvas.menu.showMenu = false;
			}

			long now = mSystem.currentTimeMillis();
			if (!isAirBottomActive)
			{
				activeWp = null;
				isAirBottomActive = true;
				wpInitTime = now;
				hasTeleportedToWp = false;
				lastTeleportTime = 0;
			}

			if (hasTeleportedToWp && now - lastTeleportTime < 140)
			{
				return false;
			}

			int targetX = System.Math.Max(48, System.Math.Min(TileMap.pxw - 48, me.cx));
			int targetY = TileMap.pxh - 24;

			int dx = Res.abs(me.cx - targetX);
			int dy = Res.abs(me.cy - targetY);

			if (!hasTeleportedToWp || dx > 24 || dy > 24)
			{
				if (ModNextMap.useSafeMovement || ModNextMap.resetPenaltyCount > 0)
				{
					if (MoveSteppedTo(targetX, targetY))
					{
						hasTeleportedToWp = true;
					}
				}
				else
				{
					TeleportTo(targetX, targetY);
					hasTeleportedToWp = true;
				}
				lastTeleportTime = now;
				return false;
			}

			if (me.cx != targetX || me.cy != targetY || me.cxSend != targetX || me.cySend != targetY)
			{
				TeleportTo(targetX, targetY);
			}

			Char.ischangingMap = true;
			Service.gI().requestChangeMap();
			ResetWaypointState();
			return true;
		}
		catch
		{
			return false;
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ UseSpaceShip.
	/// </summary>
	public static bool UseSpaceShip(Npc shipNpc, int targetPlanetMapId = -1)
	{
		try
		{
			Char me = Char.myCharz();
			if (me == null || shipNpc == null)
			{
				return false;
			}

			if (!MoveSteppedTo(shipNpc.cx, shipNpc.cy))
			{
				return false;
			}

			me.focusManualTo(shipNpc);
			int npcTemplateId = (shipNpc.template != null) ? shipNpc.template.npcTemplateId : shipNpc.npcId;

			long now = mSystem.currentTimeMillis();
			if (isWaitingShipMenu && now - lastShipOpenMenuTime < 2500)
			{
				return true;
			}

			isWaitingShipMenu = true;
			pendingTargetPlanetMapId = targetPlanetMapId;
			lastShipOpenMenuTime = now;

			Service.gI().openMenu(npcTemplateId);
			return true;
		}
		catch
		{
			return false;
		}
	}

	/// <summary>
	/// Bộ xử lý sự kiện tiếp nhận và phản hồi OnReceiveShipMenu.
	/// </summary>
	public static void OnReceiveShipMenu(string[] menuItems, Npc npc)
	{
		try
		{
			if (!isWaitingShipMenu || menuItems == null || menuItems.Length == 0)
			{
				return;
			}

			if (mSystem.currentTimeMillis() - lastShipOpenMenuTime > 5000)
			{
				isWaitingShipMenu = false;
				pendingTargetPlanetMapId = -1;
				return;
			}

			isWaitingShipMenu = false;
			int targetPlanet = pendingTargetPlanetMapId;
			int selectedIndex = -1;

			for (int i = 0; i < menuItems.Length; i++)
			{
				string text = menuItems[i].ToLower();
				if (targetPlanet == 25 && (text.Contains("nam") || text.Contains("namec") || text.Contains("namek")))
				{
					selectedIndex = i;
					break;
				}
				if (targetPlanet == 26 && (text.Contains("xay") || text.Contains("say") || text.Contains("sai")))
				{
					selectedIndex = i;
					break;
				}
				if (targetPlanet == 24 && (text.Contains("trái") || text.Contains("trai") || text.Contains("earth") || text.Contains("đất") || text.Contains("bumi")))
				{
					selectedIndex = i;
					break;
				}
			}

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
		}
		catch
		{
			isWaitingShipMenu = false;
		}
	}
}
