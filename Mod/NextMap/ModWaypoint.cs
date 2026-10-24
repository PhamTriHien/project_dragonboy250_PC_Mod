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
	private static bool isWpPosConfirmed;
	private static bool isWaitingTeleportConfirm;
	private static int pendingConfirmX;
	private static int pendingConfirmY;
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
		isWpPosConfirmed = false;
		isWaitingTeleportConfirm = false;
		pendingConfirmX = 0;
		pendingConfirmY = 0;
		lastTeleportTime = 0;
		edgeWalkAttempts = 0;
		isAirBottomActive = false;
		isWaitingShipMenu = false;
		pendingTargetPlanetMapId = -1;
	}

	/// <summary>
	/// Xử lý gói RESET_POINT (cmd 46) từ Server khi đang chạy Next Map:
	/// Phân biệt giữa xác nhận dịch chuyển hợp lệ (y + 1 ground snap) và kéo giật lùi (pullback).
	/// </summary>
	public static void OnServerResetPoint(int x, int y)
	{
		bool isConfirmedTarget = isWaitingTeleportConfirm &&
			Res.abs(x - pendingConfirmX) <= 32 &&
			Res.abs(y - pendingConfirmY) <= 32;

		if (isConfirmedTarget)
		{
			isWaitingTeleportConfirm = false;
			if (hasTeleportedToWp)
			{
				isWpPosConfirmed = true;
			}
			ModNextMap.OnServerConfirmPoint();
			ModTdltLogger.Log("NEXT_MAP_CONFIRM", $"Server confirmed pos=({x},{y})");
			return;
		}

		Char.ischangingMap = false;
		Char.isLockKey = false;
		isWaitingTeleportConfirm = false;
		hasTeleportedToWp = false;
		isWpPosConfirmed = false;
		ModNextMap.OnServerResetPoint();
		ModTdltLogger.Log("NEXT_MAP_RESET_POINT", $"Server reset pos=({x},{y}), penalty={ModNextMap.resetPenaltyCount}");
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

			int checkX = System.Math.Max(35, System.Math.Min(TileMap.pxw - 35, x));
			int safeMinY = System.Math.Max(12, minY);
			int safeMaxY = System.Math.Min(TileMap.pxh - 12, maxY);
			if (safeMaxY < safeMinY)
			{
				return maxY;
			}

			// Quét từ đáy lên và leo qua các ô đất đặc liên tiếp để lấy đúng bề mặt đất trên cùng
			int searchStart = System.Math.Min(TileMap.pxh - 12, safeMaxY + 24);
			for (int y = searchStart; y >= safeMinY; y -= 12)
			{
				if (TileMap.tileTypeAt(checkX, y, 2))
				{
					int gy = TileMap.tileYofPixel(y);
					int guard = 0;
					while (gy - 24 >= safeMinY && guard < 6 && TileMap.tileTypeAt(checkX, gy - 24, 2))
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
	/// Dịch chuyển nhân vật đến tọa độ mặt đất hợp lệ và gửi gói tin (x,y) -> (x,y+1) -> (x,y) để Server xác nhận vị trí.
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

			pendingConfirmX = x;
			pendingConfirmY = y;
			lastTeleportTime = mSystem.currentTimeMillis();

			me.cx = x;
			me.cy = y;
			me.cxSend = x - 1;
			me.cySend = y - 1;
			Service.gI().charMove();
			if (!ItemTime.isExistItem(4387))
			{
				isWaitingTeleportConfirm = TileMap.tileTypeAt(x, y, 2);
				me.cx = x;
				me.cy = y + 1;
				Service.gI().charMove();
				me.cx = x;
				me.cy = y;
				Service.gI().charMove();
			}
			else
			{
				isWaitingTeleportConfirm = false;
			}
		}
		catch
		{
		}
	}

	/// <summary>
	/// Di chuyển bước ngắn thông thường vào lòng cổng Waypoint mà không gửi gói y+1.
	/// </summary>
	public static void SendNormalMove(int x, int y)
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
			me.statusMe = 2;
			me.delayFall = 0;
			me.cyStartFall = 0;
			me.isFlyUp = false;
			me.cx = x;
			me.cy = y;
			Service.gI().charMove();
			me.statusMe = 1;
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
		int maxStepX = (ModNextMap.resetPenaltyCount >= 3) ? 64 : ((ModNextMap.resetPenaltyCount == 2) ? 108 : ((ModNextMap.resetPenaltyCount == 1) ? 160 : 180));

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

		if (me.cx != targetX || me.cy != targetY || me.cxSend != targetX || me.cySend != targetY || dx > 0 || dy > 0)
		{
			me.cdir = (targetX >= me.cx) ? 1 : (-1);
			TeleportTo(targetX, targetY);
		}
		return true;
	}

	/// <summary>
	/// Tính toán tọa độ X nằm gọn bên trong vùng [wp.minX, wp.maxX] để kích hoạt chuyển map (chuẩn Xmap gốc: 15 hoặc pxw - 15).
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
			if (15 >= minX && 15 <= maxX)
			{
				return 15;
			}
			return System.Math.Max(6, center);
		}

		if (minX >= TileMap.pxw - 60)
		{
			int rightTarget = TileMap.pxw - 15;
			if (rightTarget >= minX && rightTarget <= maxX)
			{
				return rightTarget;
			}
			return System.Math.Min(TileMap.pxw - 6, center);
		}

		return center;
	}

	/// <summary>
	/// Tính toán tọa độ tiếp cận (Phase 1) tại cột đất hợp lệ sát mép cổng (x = 35 hoặc pxw - 35) để Server xác nhận RESET_POINT trước khi bước vào cổng.
	/// </summary>
	public static int GetWaypointApproachX(Waypoint wp, int wpTargetX, int wpTargetY)
	{
		if (wp == null)
		{
			return wpTargetX;
		}

		bool isLeftEdge = wp.maxX <= 60;
		bool isRightEdge = !isLeftEdge && wp.minX >= TileMap.pxw - 60;
		if (!isLeftEdge && !isRightEdge)
		{
			return wpTargetX;
		}

		bool isAirOrTower = TileMap.isInAirMap() || TileMap.mapID == 45 || TileMap.mapID == 46 || TileMap.mapID == 47 || TileMap.mapID == 48;
		if (isLeftEdge)
		{
			int baseApproachX = System.Math.Max(35, wpTargetX + 20);
			if (isAirOrTower || TileMap.tileTypeAt(baseApproachX, wpTargetY, 2))
			{
				return baseApproachX;
			}
			for (int testX = baseApproachX + 12; testX <= 96; testX += 12)
			{
				if (TileMap.tileTypeAt(testX, wpTargetY, 2))
				{
					return testX;
				}
			}
			return baseApproachX;
		}
		else
		{
			int baseApproachX = System.Math.Min(TileMap.pxw - 35, wpTargetX - 20);
			if (isAirOrTower || TileMap.tileTypeAt(baseApproachX, wpTargetY, 2))
			{
				return baseApproachX;
			}
			for (int testX = baseApproachX - 12; testX >= TileMap.pxw - 96; testX -= 12)
			{
				if (TileMap.tileTypeAt(testX, wpTargetY, 2))
				{
					return testX;
				}
			}
			return baseApproachX;
		}
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
	/// Thực thi dịch chuyển 2 pha chuẩn Xmap gốc:
	/// Pha 1: Dịch chuyển tới cột đất sát cổng (approachX = 35 hoặc pxw - 35) và đợi Server xác nhận vị trí.
	/// Pha 2: Bước thường (SendNormalMove <= 24px, không gửi y+1) vào tâm cổng (wpTargetX = 15 hoặc pxw - 15) và gửi requestChangeMap.
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
				isWpPosConfirmed = false;
				isWaitingTeleportConfirm = false;
				lastTeleportTime = 0;
				edgeWalkAttempts = 0;
			}

			if (isWaitingTeleportConfirm)
			{
				if (now - lastTeleportTime < 550)
				{
					return false;
				}
				isWaitingTeleportConfirm = false;
				if (hasTeleportedToWp)
				{
					isWpPosConfirmed = true;
				}
			}

			bool isLeftEdge = wp.maxX <= 60;
			bool isRightEdge = !isLeftEdge && wp.minX >= TileMap.pxw - 60;

			int wpTargetX = GetWaypointTargetX(wp);
			int wpTargetY = GetWaypointTargetY(wp, wpTargetX);
			int approachX = GetWaypointApproachX(wp, wpTargetX, wpTargetY);
			if (approachX != wpTargetX)
			{
				wpTargetY = GetWaypointTargetY(wp, approachX);
			}

			me.cdir = isLeftEdge ? (-1) : (isRightEdge ? 1 : ((me.cx <= wpTargetX) ? 1 : (-1)));

			// Pha 1: Dịch chuyển tới tọa độ tiếp cận sát cổng (cột đất hợp lệ) và đợi Server xác nhận
			if (!hasTeleportedToWp)
			{
				if (MoveSteppedTo(approachX, wpTargetY))
				{
					hasTeleportedToWp = true;
					if (!isWaitingTeleportConfirm)
					{
						isWpPosConfirmed = true;
					}
				}
				wpInitTime = now;
				ModTdltLogger.Log("NEXT_MAP_STEP", $"Phase 1 teleport to ({me.cx},{me.cy}) -> approach=({approachX},{wpTargetY}) for '{wp.name}' target=({wpTargetX},{wpTargetY}), map={TileMap.mapID}");
				return false;
			}

			if (!isWpPosConfirmed)
			{
				if (now - lastTeleportTime >= 550)
				{
					isWpPosConfirmed = true;
				}
				else
				{
					return false;
				}
			}

			// Kiểm tra nếu nhân vật bị lệch xa khỏi vùng tiếp cận thì quay lại Pha 1
			if (Res.abs(me.cx - wpTargetX) > 84 || Res.abs(me.cy - wpTargetY) > 48)
			{
				hasTeleportedToWp = false;
				isWpPosConfirmed = false;
				return false;
			}

			// Pha 2: Bước thường (<= 24px, không gửi y+1) từ approachX vào đúng wpTargetX rồi yêu cầu chuyển map
			int dxToTarget = wpTargetX - me.cx;
			if (Res.abs(dxToTarget) > 24)
			{
				int stepX = me.cx + ((dxToTarget > 0) ? 24 : (-24));
				SendNormalMove(stepX, wpTargetY);
				lastTeleportTime = now;
				return false;
			}

			if (me.cx != wpTargetX || me.cy != wpTargetY || me.cxSend != wpTargetX || me.cySend != wpTargetY)
			{
				SendNormalMove(wpTargetX, wpTargetY);
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
			ModTdltLogger.Log("NEXT_MAP_REQ", $"Request change map from map={TileMap.mapID} via '{wp.name}' at pos=({me.cx},{me.cy})");
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
				isWpPosConfirmed = false;
				isWaitingTeleportConfirm = false;
				lastTeleportTime = 0;
			}

			if (isWaitingTeleportConfirm)
			{
				if (now - lastTeleportTime < 550)
				{
					return false;
				}
				isWaitingTeleportConfirm = false;
				if (hasTeleportedToWp)
				{
					isWpPosConfirmed = true;
				}
			}

			int targetX = System.Math.Max(48, System.Math.Min(TileMap.pxw - 48, me.cx));
			int targetY = TileMap.pxh - 24;

			if (!hasTeleportedToWp)
			{
				if (MoveSteppedTo(targetX, targetY))
				{
					hasTeleportedToWp = true;
					if (!isWaitingTeleportConfirm)
					{
						isWpPosConfirmed = true;
					}
				}
				return false;
			}

			if (!isWpPosConfirmed)
			{
				if (now - lastTeleportTime >= 550)
				{
					isWpPosConfirmed = true;
				}
				else
				{
					return false;
				}
			}

			SendNormalMove(targetX, TileMap.pxh - 12);
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

			long now = mSystem.currentTimeMillis();
			if (isWaitingTeleportConfirm)
			{
				if (now - lastTeleportTime < 550)
				{
					return false;
				}
				isWaitingTeleportConfirm = false;
			}

			if (Res.abs(me.cx - shipNpc.cx) > 24 || Res.abs(me.cy - shipNpc.cy) > 24)
			{
				MoveSteppedTo(shipNpc.cx, shipNpc.cy);
				return false;
			}

			me.focusManualTo(shipNpc);
			int npcTemplateId = (shipNpc.template != null) ? shipNpc.template.npcTemplateId : shipNpc.npcId;

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
