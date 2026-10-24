using System;
using System.Collections.Generic;

/// <summary>
/// Module Dịch Chuyển Liên Map (Xmap): Tìm đường và tự động chuyển map qua nhiều chặng trung gian.
/// </summary>
public static class ModNextMap
{
	public static int selectedPlanetTab = 0;
	public static bool isNextMapActive = false;
	public static int nextMapTargetId = -1;
	public static bool useSafeMovement = false;
	public static int lastMapId = -1;
	public static Waypoint currentTargetWaypoint = null;
	public static int resetPenaltyCount = 0;
	private static long lastResetPenaltyTime = 0;
	private static int nextMapCooldown = 0;
	private static int nextMapFailCount = 0;
	private static long lastChangeAttemptTime = 0;
	private static long mapEnterWaitUntil = 0;
	private static long lastStepUpdateTime = 0;

	/// <summary>
	/// Phản hồi khi server gửi gói RESET_POINT (cmd 46) kéo lùi nhân vật để tạm hoãn và giảm bước nhảy chống giật lặp lại.
	/// </summary>
	public static void OnServerResetPoint()
	{
		long now = mSystem.currentTimeMillis();
		if (now - lastResetPenaltyTime > 250)
		{
			resetPenaltyCount++;
			lastResetPenaltyTime = now;
		}
		lastChangeAttemptTime = 0;
		nextMapCooldown = 6;
		mapEnterWaitUntil = now + 280;
		ModWaypoint.ResetWaypointState();
	}

	/// <summary>
	/// Phản hồi khi server gửi gói RESET_POINT xác nhận vị trí dịch chuyển hợp lệ để chuyển ngay sang nhịp tiếp theo.
	/// </summary>
	public static void OnServerConfirmPoint()
	{
		nextMapCooldown = 0;
		lastStepUpdateTime = 0;
	}

	// Backward-compatible accessors to data
	public static int[][] planetMapIds
	{
		get { return ModNextMapData.planetMapIds; }
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của GetMapName.
	/// </summary>
	public static string GetMapName(int id)
	{
		return ModNextMapData.GetMapName(id);
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ MatchMapName.
	/// </summary>
	public static bool MatchMapName(string wpName, string mapName)
	{
		return ModNextMapData.MatchMapName(wpName, mapName);
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ FindMapIdByName.
	/// </summary>
	public static int FindMapIdByName(string name)
	{
		return ModNextMapData.FindMapIdByName(name);
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ FindPath.
	/// </summary>
	public static List<int> FindPath(int startMapId, int targetMapId)
	{
		return ModNextMapPathFinder.FindPath(startMapId, targetMapId);
	}

	/// <summary>
	/// Kiểm tra xem chặng chuyển map hiện tại có phải là đi xuống đáy của Air Map (45 -> 46, 46 -> 47, 48 -> 45) hay không.
	/// </summary>
	public static bool IsAirMapDownwardTransition(int currMapId, int nextMapId)
	{
		return (currMapId == 46 && nextMapId == 47) ||
		       (currMapId == 45 && nextMapId == 46) ||
		       (currMapId == 48 && nextMapId == 45);
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ StartNextMap.
	/// </summary>
	public static void StartNextMap(int targetId)
	{
		if (TileMap.mapID == targetId)
		{
			GameScr.info1.addInfo("Bạn đang ở " + GetMapName(targetId) + " rồi!", 0);
			return;
		}

		ModNextMapData.SyncLiveWaypointsForCurrentMap();

		nextMapTargetId = targetId;
		isNextMapActive = true;
		nextMapCooldown = 0;
		nextMapFailCount = 0;
		resetPenaltyCount = 0;
		currentTargetWaypoint = null;
		lastMapId = TileMap.mapID;
		lastChangeAttemptTime = 0;
		mapEnterWaitUntil = 0;
		lastStepUpdateTime = 0;
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
		ModWaypoint.ResetWaypointState();

		ModTdltLogger.Log("NEXT_MAP_START", $"Start NextMap from map={TileMap.mapID} ({GetMapName(TileMap.mapID)}) -> target={targetId} ({GetMapName(targetId)})");
		GameScr.info1.addInfo("Bắt đầu Next Map đến: " + GetMapName(targetId), 0);
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ StopNextMap.
	/// </summary>
	public static void StopNextMap()
	{
		isNextMapActive = false;
		nextMapTargetId = -1;
		nextMapCooldown = 0;
		nextMapFailCount = 0;
		resetPenaltyCount = 0;
		currentTargetWaypoint = null;
		lastChangeAttemptTime = 0;
		mapEnterWaitUntil = 0;
		lastStepUpdateTime = 0;
		Char.entranceWaypoint = null;
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
		ModWaypoint.ResetWaypointState();
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ FindWaypointToMap.
	/// </summary>
	public static Waypoint FindWaypointToMap(int nextMapId)
	{
		if (TileMap.vGo == null || TileMap.vGo.size() == 0)
		{
			return null;
		}

		int currMapId = TileMap.mapID;
		int planetId = TileMap.planetID;

		// 1. Giải mã ID đích chính xác từ rawName, name và popup.says của từng Waypoint
		for (int j = 0; j < TileMap.vGo.size(); j++)
		{
			Waypoint wp = (Waypoint)TileMap.vGo.elementAt(j);
			if (wp != null)
			{
				int resolvedId = ModNextMapData.ResolveWaypointTargetMapId(wp, currMapId, planetId);
				if (resolvedId == nextMapId)
				{
					return wp;
				}
			}
		}

		// 2. So khớp đa ngôn ngữ trực tiếp trên rawName, name và popup.says
		for (int j = 0; j < TileMap.vGo.size(); j++)
		{
			Waypoint wp = (Waypoint)TileMap.vGo.elementAt(j);
			if (wp != null)
			{
				if (!string.IsNullOrEmpty(wp.rawName) && ModNextMapData.MatchMapId(wp.rawName, nextMapId))
				{
					return wp;
				}

				string fullName = wp.name ?? string.Empty;
				if (wp.popup != null && wp.popup.says != null)
				{
					for (int s = 0; s < wp.popup.says.Length; s++)
					{
						fullName += " " + wp.popup.says[s];
					}
				}
				if (ModNextMapData.MatchMapId(fullName, nextMapId))
				{
					return wp;
				}
			}
		}

		// 3. Xử lý các map đặc biệt: Nhà đẻ (21, 22, 23)
		if (nextMapId == 21 || nextMapId == 22 || nextMapId == 23)
		{
			for (int j = 0; j < TileMap.vGo.size(); j++)
			{
				Waypoint wp = (Waypoint)TileMap.vGo.elementAt(j);
				if (wp != null && (wp.isOffline || (wp.isEnter && wp.minX >= 60 && wp.maxX <= TileMap.pxw - 60)))
				{
					return wp;
				}
			}
			return null;
		}
		if (currMapId == 21 || currMapId == 22 || currMapId == 23)
		{
			return (Waypoint)TileMap.vGo.elementAt(0);
		}

		// 4. Xử lý Vách núi (42, 43, 44, 39, 40, 41) từ Làng (0, 7, 14) - luôn là cổng rìa trái
		if ((currMapId == 0 || currMapId == 7 || currMapId == 14) &&
		    (nextMapId == 42 || nextMapId == 43 || nextMapId == 44 || nextMapId == 39 || nextMapId == 40 || nextMapId == 41))
		{
			for (int j = 0; j < TileMap.vGo.size(); j++)
			{
				Waypoint wp = (Waypoint)TileMap.vGo.elementAt(j);
				if (wp != null && !wp.isOffline && !wp.isEnter && wp.maxX < 100)
				{
					return wp;
				}
			}
		}

		// 5. Xử lý Vách Aru, Rừng Karin, Tháp Karin, Thần điện & Hành tinh Kaio (42, 47, 46, 45, 48)
		if (currMapId == 42)
		{
			if (nextMapId == 47)
			{
				Waypoint leftWp = null;
				int minLeftX = int.MaxValue;
				for (int k = 0; k < TileMap.vGo.size(); k++)
				{
					Waypoint w = (Waypoint)TileMap.vGo.elementAt(k);
					if (w != null && w.minX < minLeftX)
					{
						minLeftX = w.minX;
						leftWp = w;
					}
				}
				if (leftWp != null) return leftWp;
			}
			else if (nextMapId == 0)
			{
				Waypoint rightWp = null;
				int maxRightX = int.MinValue;
				for (int k = 0; k < TileMap.vGo.size(); k++)
				{
					Waypoint w = (Waypoint)TileMap.vGo.elementAt(k);
					if (w != null && w.maxX > maxRightX)
					{
						maxRightX = w.maxX;
						rightWp = w;
					}
				}
				if (rightWp != null) return rightWp;
			}
		}
		else if (currMapId == 47)
		{
			if (nextMapId == 46)
			{
				Waypoint topWp = null;
				int minTopY = int.MaxValue;
				for (int k = 0; k < TileMap.vGo.size(); k++)
				{
					Waypoint w = (Waypoint)TileMap.vGo.elementAt(k);
					if (w != null && w.minY < minTopY)
					{
						minTopY = w.minY;
						topWp = w;
					}
				}
				if (topWp != null) return topWp;
			}
			else if (nextMapId == 42)
			{
				Waypoint leftWp = null;
				int minLeftX = int.MaxValue;
				for (int k = 0; k < TileMap.vGo.size(); k++)
				{
					Waypoint w = (Waypoint)TileMap.vGo.elementAt(k);
					if (w != null && w.minX < minLeftX)
					{
						minLeftX = w.minX;
						leftWp = w;
					}
				}
				if (leftWp != null) return leftWp;
			}
			else if (nextMapId == 2)
			{
				Waypoint rightWp = null;
				int maxRightX = int.MinValue;
				for (int k = 0; k < TileMap.vGo.size(); k++)
				{
					Waypoint w = (Waypoint)TileMap.vGo.elementAt(k);
					if (w != null && w.maxX > maxRightX)
					{
						maxRightX = w.maxX;
						rightWp = w;
					}
				}
				if (rightWp != null) return rightWp;
			}
		}
		else if (currMapId == 46)
		{
			if (nextMapId == 45)
			{
				Waypoint topWp = null;
				int minTopY = int.MaxValue;
				for (int k = 0; k < TileMap.vGo.size(); k++)
				{
					Waypoint w = (Waypoint)TileMap.vGo.elementAt(k);
					if (w != null && w.minY <= 150 && w.minY < minTopY)
					{
						minTopY = w.minY;
						topWp = w;
					}
				}
				if (topWp != null) return topWp;
			}
			else if (nextMapId == 47)
			{
				for (int k = 0; k < TileMap.vGo.size(); k++)
				{
					Waypoint w = (Waypoint)TileMap.vGo.elementAt(k);
					if (w != null && w.minY > 150)
					{
						return w;
					}
				}
				return null;
			}
		}
		else if (currMapId == 45)
		{
			if (nextMapId == 48)
			{
				Waypoint topWp = null;
				int minTopY = int.MaxValue;
				for (int k = 0; k < TileMap.vGo.size(); k++)
				{
					Waypoint w = (Waypoint)TileMap.vGo.elementAt(k);
					if (w != null && w.minY <= 150 && w.minY < minTopY)
					{
						minTopY = w.minY;
						topWp = w;
					}
				}
				if (topWp != null) return topWp;
			}
			else if (nextMapId == 46)
			{
				for (int k = 0; k < TileMap.vGo.size(); k++)
				{
					Waypoint w = (Waypoint)TileMap.vGo.elementAt(k);
					if (w != null && w.minY > 150)
					{
						return w;
					}
				}
				return null;
			}
		}
		else if (currMapId == 48 && nextMapId == 45)
		{
			for (int k = 0; k < TileMap.vGo.size(); k++)
			{
				Waypoint w = (Waypoint)TileMap.vGo.elementAt(k);
				if (w != null && w.minY > 150)
				{
					return w;
				}
			}
			return null;
		}

		// 6. Nếu không tìm thấy cổng khớp tên, đồng bộ lại đồ thị thực tế để FindPath tự định tuyến lại ở nhịp kế tiếp
		ModNextMapData.SyncLiveWaypointsForCurrentMap();

		// Chỉ fallback theo hướng X đối với các cổng không xác định được ID đích (tránh bước nhầm vào cổng đã biết rõ dẫn sang map khác)
		Waypoint bestUnresolvedWp = null;
		if (nextMapId > currMapId)
		{
			int maxRight = int.MinValue;
			for (int k = 0; k < TileMap.vGo.size(); k++)
			{
				Waypoint wp2 = (Waypoint)TileMap.vGo.elementAt(k);
				if (wp2 != null && ModNextMapData.ResolveWaypointTargetMapId(wp2, currMapId, planetId) == -1 && wp2.maxX > maxRight)
				{
					maxRight = wp2.maxX;
					bestUnresolvedWp = wp2;
				}
			}
		}
		else
		{
			int minLeft = int.MaxValue;
			for (int k = 0; k < TileMap.vGo.size(); k++)
			{
				Waypoint wp2 = (Waypoint)TileMap.vGo.elementAt(k);
				if (wp2 != null && ModNextMapData.ResolveWaypointTargetMapId(wp2, currMapId, planetId) == -1 && wp2.minX < minLeft)
				{
					minLeft = wp2.minX;
					bestUnresolvedWp = wp2;
				}
			}
		}

		return bestUnresolvedWp;
	}

	/// <summary>
	/// Cập nhật trạng thái logic của UpdateNextMap theo chu kỳ khung hình.
	/// </summary>
	public static void UpdateNextMap()
	{
		if (!isNextMapActive || nextMapTargetId == -1)
		{
			return;
		}

		Char me = Char.myCharz();
		if (me == null || Char.isLoadingMap || Controller.isStopReadMessage)
		{
			return;
		}

		long now = mSystem.currentTimeMillis();

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
			nextMapCooldown = 6;
			lastChangeAttemptTime = 0;
			nextMapFailCount = 0;
			resetPenaltyCount = 0;
			currentTargetWaypoint = null;
			mapEnterWaitUntil = now + 300;
			lastStepUpdateTime = 0;
			ModWaypoint.ResetWaypointState();
			ModNextMapData.SyncLiveWaypointsForCurrentMap();
			ModTdltLogger.Log("NEXT_MAP_ENTER", $"Entered map={TileMap.mapID} ({GetMapName(TileMap.mapID)}), target={nextMapTargetId} ({GetMapName(nextMapTargetId)})");
		}

		// Tự động đóng popup thông báo sự kiện khi đang chạy Next Map
		if (GameCanvas.currentDialog == GameCanvas.msgdlg && GameCanvas.msgdlg != null && !GameCanvas.msgdlg.isWait)
		{
			GameCanvas.endDlg();
		}

		// Kiểm tra đến map đích
		if (TileMap.mapID == nextMapTargetId)
		{
			GameScr.info1.addInfo("ĐÃ ĐẾN: " + GetMapName(nextMapTargetId) + "!", 0);
			SoundMn.gI().buttonClose();
			StopNextMap();
			return;
		}

		// Đợi tàu vũ trụ hạ cánh xong mới di chuyển, tránh bị Teleport.update kéo ngược về tàu
		if (me.isTeleport || me.statusMe == 16)
		{
			mapEnterWaitUntil = now + 500;
			return;
		}

		// Watchdog chống kẹt trạng thái ischangingMap khi server rớt gói tin hoặc phản hồi chậm
		if (Char.ischangingMap)
		{
			if (lastChangeAttemptTime == 0)
			{
				lastChangeAttemptTime = now;
			}
			else if (now - lastChangeAttemptTime > 1200)
			{
				Char.ischangingMap = false;
				Char.isLockKey = false;
				me.isLockAttack = false;
				me.isLockMove = false;
				me.isTeleport = false;
				Controller.isStopReadMessage = false;
				GameScr.lockTick = 0;
				if (Teleport.vTeleport != null)
				{
					Teleport.vTeleport.removeAllElements();
				}
				InfoDlg.hide();
				GameCanvas.endDlg();
				lastChangeAttemptTime = 0;
				nextMapCooldown = 3;
				mapEnterWaitUntil = now + 180;
				Char.entranceWaypoint = null;
				ModWaypoint.ResetWaypointState();
			}
			return;
		}

		if (nextMapCooldown > 0)
		{
			nextMapCooldown--;
			return;
		}

		int stepIntervalMs = (useSafeMovement || resetPenaltyCount > 0) ? 260 : 180;
		if (now < mapEnterWaitUntil || now - lastStepUpdateTime < stepIntervalMs)
		{
			return;
		}
		lastStepUpdateTime = now;

		ModNextMapData.SyncLiveWaypointsForCurrentMap();
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
			currentTargetWaypoint = null;
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
				bool confirmedShip = ModWaypoint.UseSpaceShip(shipNpc, nextMapId);
				if (confirmedShip)
				{
					lastChangeAttemptTime = now;
					mapEnterWaitUntil = now + 600;
				}
				return;
			}
		}

		// Tìm Waypoint dẫn sang map tiếp theo
		Waypoint targetWp = FindWaypointToMap(nextMapId);
		currentTargetWaypoint = targetWp;

		if (targetWp != null)
		{
			nextMapFailCount = 0;
			bool sentRequest = ModWaypoint.StepToWaypoint(targetWp);
			if (sentRequest)
			{
				lastChangeAttemptTime = now;
				mapEnterWaitUntil = now + 400;
			}
		}
		else if (IsAirMapDownwardTransition(TileMap.mapID, nextMapId))
		{
			nextMapFailCount = 0;
			bool sentRequest = ModWaypoint.StepToAirMapBottom();
			if (sentRequest)
			{
				lastChangeAttemptTime = now;
				mapEnterWaitUntil = now + 400;
			}
		}
		else
		{
			nextMapFailCount++;
			if (nextMapFailCount > 25)
			{
				GameScr.info1.addInfo("Không tìm thấy cổng sang " + GetMapName(nextMapId), 0);
				StopNextMap();
			}
			else
			{
				mapEnterWaitUntil = now + 250;
			}
		}
	}

	/// <summary>
	/// Thực hiện vẽ và hiển thị thành phần PaintHUDMapTag lên canvas đồ họa.
	/// </summary>
	public static void PaintHUDMapTag(mGraphics g)
	{
		try
		{
			if (g == null || !ModMenu.IsInGame())
			{
				return;
			}

			// Tự động ẩn khi đang mở Panel, Menu, Dialog hoặc Mod UI
			if ((GameCanvas.panel != null && GameCanvas.panel.isShow) ||
			    (GameCanvas.panel2 != null && GameCanvas.panel2.isShow) ||
			    (GameCanvas.menu != null && GameCanvas.menu.showMenu) ||
			    GameCanvas.currentDialog != null ||
			    ModUI.uiCustomOpen)
			{
				return;
			}

			g.translate(-g.getTranslateX(), -g.getTranslateY());
			g.setClip(0, 0, GameCanvas.w, GameCanvas.h);

			int drawX = 160;
			int drawY = 16;

			string mapTag = (isNextMapActive && nextMapTargetId >= 0)
				? ("-> " + GetMapName(nextMapTargetId) + " [K." + TileMap.zoneID + "]")
				: (TileMap.mapName + " [K." + TileMap.zoneID + "]");

			if (mFont.tahoma_7_grey != null)
			{
				mFont.tahoma_7_grey.drawString(g, mapTag, drawX + 1, drawY + 1, mFont.LEFT);
			}

			if (isNextMapActive)
			{
				if (mFont.tahoma_7b_green2 != null)
				{
					mFont.tahoma_7b_green2.drawString(g, mapTag, drawX, drawY, mFont.LEFT);
				}
			}
			else
			{
				if (mFont.tahoma_7b_yellow != null)
				{
					mFont.tahoma_7b_yellow.drawString(g, mapTag, drawX, drawY, mFont.LEFT);
				}
				else if (mFont.tahoma_7_white != null)
				{
					mFont.tahoma_7_white.drawString(g, mapTag, drawX, drawY, mFont.LEFT);
				}
			}

			if (isNextMapActive || ModBossNotice.isAutoScanBossZone || ModGoBack.isReturning || ModSetActivator.isBusy || ModAutoBuyBua.isBusy)
			{
				int stopW = 86;
				int stopH = 16;
				int stopX = (GameCanvas.w - stopW) / 2;
				int stopY = GameCanvas.h - 68;
				ModUI.PaintNativeButton(stopX, stopY, stopW, stopH, "Dừng Di Chuyển", true, g);
			}
		}
		catch
		{
		}
	}

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của CheckHUDMapTagClick.
	/// </summary>
	public static bool CheckHUDMapTagClick(int px, int py)
	{
		try
		{
			if (!ModMenu.IsInGame())
			{
				return false;
			}

			if ((GameCanvas.panel != null && GameCanvas.panel.isShow) ||
			    (GameCanvas.panel2 != null && GameCanvas.panel2.isShow) ||
			    (GameCanvas.menu != null && GameCanvas.menu.showMenu) ||
			    GameCanvas.currentDialog != null ||
			    ModUI.uiCustomOpen)
			{
				return false;
			}

			if (isNextMapActive || ModBossNotice.isAutoScanBossZone || ModGoBack.isReturning || ModSetActivator.isBusy || ModAutoBuyBua.isBusy)
			{
				int stopW = 86;
				int stopH = 16;
				int stopX = (GameCanvas.w - stopW) / 2;
				int stopY = GameCanvas.h - 68;
				if (Res.inRect(stopX, stopY, stopW, stopH, px, py))
				{
					if (GameCanvas.isPointerClick || GameCanvas.isPointerJustRelease)
					{
						if (isNextMapActive) StopNextMap();
						if (ModBossNotice.isAutoScanBossZone) ModBossNotice.isAutoScanBossZone = false;
						if (ModGoBack.isReturning) ModGoBack.isReturning = false;
						if (ModSetActivator.isBusy) ModSetActivator.currentState = ModSetActivator.SetActivatorState.Farming;
						if (ModAutoBuyBua.isBusy) ModAutoBuyBua.StopAutoBuyBua();
						GameScr.info1.addInfo("Đã dừng di chuyển tự động!", 0);
						SoundMn.gI().buttonClick();
						GameCanvas.clearAllPointerEvent();
						return true;
					}
					return true;
				}
			}

			int drawX = 160;
			int drawY = 16;

			string mapTag = (isNextMapActive && nextMapTargetId >= 0)
				? ("-> " + GetMapName(nextMapTargetId) + " [K." + TileMap.zoneID + "]")
				: (TileMap.mapName + " [K." + TileMap.zoneID + "]");

			int tagW = (mFont.tahoma_7b_yellow != null) ? mFont.tahoma_7b_yellow.getWidth(mapTag) : 90;

			if (Res.inRect(drawX - 2, drawY - 2, tagW + 6, 13, px, py))
			{
				if (GameCanvas.isPointerClick || GameCanvas.isPointerJustRelease)
				{
					ModUI.selectedTab = 7;
					ModUI.uiCustomOpen = true;
					SoundMn.gI().buttonClick();
					GameCanvas.clearAllPointerEvent();
					return true;
				}
				return true;
			}
		}
		catch
		{
		}

		return false;
	}
}
