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
	public static bool useCapsuleVip = true;
	public static bool useNormalCapsule = false;
	public static bool speedBoostTransport = true;
	public static int lastMapId = -1;
	public static Waypoint currentTargetWaypoint = null;
	public static int resetPenaltyCount = 0;
	private static long lastResetPenaltyTime = 0;
	private static int nextMapCooldown = 0;
	private static int nextMapFailCount = 0;
	private static long lastChangeAttemptTime = 0;
	private static long mapEnterWaitUntil = 0;
	private static long lastStepUpdateTime = 0;
	private static bool s_WaitingCapsuleMenu = false;
	private static long s_LastCapsuleUseTime = 0;

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

		// Khôi phục các cạnh động của NPC Calích (27/28/29 -> 102) cho mỗi lượt tìm đường mới
		int[] calichMaps = new int[] { 27, 28, 29 };
		bool restoredCalich = false;
		for (int i = 0; i < calichMaps.Length; i++)
		{
			if (ModNextMapData.mapWaypoints.TryGetValue(calichMaps[i], out List<int> cList) && !cList.Contains(102))
			{
				cList.Add(102);
				restoredCalich = true;
			}
		}
		if (restoredCalich)
		{
			ModNextMapPathFinder.ClearCache();
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
		s_WaitingCapsuleMenu = false;
		s_LastCapsuleUseTime = 0;
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
		s_WaitingCapsuleMenu = false;
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

		// Ưu tiên hướng cổng theo chuỗi Xmap gốc (-1: Trái, +1: Phải), nếu không có mới so sánh ID
		int chainSide = ModNextMapData.GetChainWaypointSide(currMapId, nextMapId);
		int preferSide = (chainSide != 0) ? chainSide : ((nextMapId > currMapId) ? 1 : -1);

		// Chỉ fallback theo hướng X đối với các cổng không xác định được ID đích (tránh bước nhầm vào cổng đã biết rõ dẫn sang map khác)
		Waypoint bestUnresolvedWp = null;
		if (preferSide > 0)
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

	private static Npc FindTransitionNpc(int currMapId, int nextMapId)
	{
		if (GameScr.vNpc == null)
		{
			return null;
		}

		for (int i = 0; i < GameScr.vNpc.size(); i++)
		{
			Npc npc = (Npc)GameScr.vNpc.elementAt(i);
			if (npc == null || npc.template == null || npc.statusMe == 15)
			{
				continue;
			}

			int tplId = npc.template.npcTemplateId;
			if ((currMapId == 24 || currMapId == 25 || currMapId == 26) &&
			    (nextMapId == 24 || nextMapId == 25 || nextMapId == 26 || nextMapId == 84))
			{
				if (tplId == 10 || tplId == 11 || tplId == 12) return npc;
			}
			else if ((currMapId == 19 && (nextMapId == 68 || nextMapId == 109)) ||
			         ((currMapId == 68 || currMapId == 109) && nextMapId == 19))
			{
				if (tplId == 12) return npc;
			}
			else if ((currMapId == 80 && nextMapId == 131) || (currMapId == 131 && nextMapId == 80))
			{
				if (tplId == 60) return npc;
			}
			else if (((currMapId == 27 || currMapId == 28 || currMapId == 29) && nextMapId == 102) ||
			         (currMapId == 102 && (nextMapId == 24 || nextMapId == 27 || nextMapId == 28 || nextMapId == 29)))
			{
				if (tplId == 38) return npc;
			}
			else if (currMapId == 48 && nextMapId == 50)
			{
				if (tplId == 20) return npc;
			}
			else if (currMapId == 50 && nextMapId == 48)
			{
				if (tplId == 44 || tplId == 20) return npc;
			}
			else if ((currMapId == 24 || currMapId == 25 || currMapId == 26) && nextMapId >= 85 && nextMapId <= 91)
			{
				if (tplId == 29) return npc;
			}
			else if (currMapId >= 85 && currMapId <= 91 && (nextMapId == 24 || nextMapId == 25 || nextMapId == 26))
			{
				if (tplId >= 30 && tplId <= 36) return npc;
			}
		}

		return null;
	}

	private static int FindBagItemIndex(Char me, int templateId, int minQuantity = 1)
	{
		if (me == null || me.arrItemBag == null)
		{
			return -1;
		}
		for (int i = 0; i < me.arrItemBag.Length; i++)
		{
			Item it = me.arrItemBag[i];
			if (it != null && it.template != null && it.template.id == templateId && it.quantity >= minQuantity)
			{
				return i;
			}
		}
		return -1;
	}

	private static string ExtractCapsuleMapLabel(string rawEntry)
	{
		if (string.IsNullOrEmpty(rawEntry))
		{
			return string.Empty;
		}
		int colonIdx = rawEntry.IndexOf(':');
		if (colonIdx >= 0 && colonIdx + 1 < rawEntry.Length)
		{
			return rawEntry.Substring(colonIdx + 1).Trim();
		}
		return rawEntry.Trim();
	}

	public static bool OnCapsuleMapListReceived(string[] mapNames)
	{
		if (!isNextMapActive || !s_WaitingCapsuleMenu)
		{
			return false;
		}
		s_WaitingCapsuleMenu = false;
		if (mapNames == null || mapNames.Length == 0 || nextMapTargetId < 0)
		{
			return true;
		}

		List<int> path = FindPath(TileMap.mapID, nextMapTargetId);
		int bestMenuIndex = -1;

		if (path != null && path.Count > 0)
		{
			for (int p = path.Count - 1; p >= 1 && bestMenuIndex < 0; p--)
			{
				int candidateMapId = path[p];
				for (int k = 0; k < mapNames.Length; k++)
				{
					string label = ExtractCapsuleMapLabel(mapNames[k]);
					if (FindMapIdByName(label) == candidateMapId || ModNextMapData.MatchMapId(label, candidateMapId))
					{
						bestMenuIndex = k;
						break;
					}
				}
			}
		}

		if (bestMenuIndex < 0)
		{
			for (int k = 0; k < mapNames.Length; k++)
			{
				string label = ExtractCapsuleMapLabel(mapNames[k]);
				if (FindMapIdByName(label) == nextMapTargetId || ModNextMapData.MatchMapId(label, nextMapTargetId))
				{
					bestMenuIndex = k;
					break;
				}
			}
		}

		if (bestMenuIndex >= 0)
		{
			long now = mSystem.currentTimeMillis();
			Char.ischangingMap = true;
			lastChangeAttemptTime = now;
			mapEnterWaitUntil = now + 650;
			Service.gI().requestMapSelect(bestMenuIndex);
		}
		return true;
	}

	public static void LoadMapDirection(int dir)
	{
		try
		{
			if (!ModMenu.IsInGame() || Char.isLoadingMap || Char.ischangingMap)
			{
				return;
			}
			Char me = Char.myCharz();
			if (me == null || me.isDie || me.cHP <= 0)
			{
				return;
			}
			if (isNextMapActive)
			{
				StopNextMap();
			}

			int currMap = TileMap.mapID;
			if (dir == 2 && currMap >= 85 && currMap <= 91 && GameScr.vNpc != null)
			{
				for (int i = 0; i < GameScr.vNpc.size(); i++)
				{
					Npc npc = (Npc)GameScr.vNpc.elementAt(i);
					if (npc != null && npc.template != null && npc.template.npcTemplateId >= 30 && npc.template.npcTemplateId <= 36)
					{
						ModTeleport.TeleportTo(npc.cx, TileMap.tileYofPixel(npc.cy - 3));
						Service.gI().openMenu(npc.template.npcTemplateId);
						return;
					}
				}
			}

			Waypoint targetWp = null;
			if (TileMap.vGo != null && TileMap.vGo.size() > 0)
			{
				for (int i = 0; i < TileMap.vGo.size(); i++)
				{
					Waypoint wp = (Waypoint)TileMap.vGo.elementAt(i);
					if (wp == null) continue;
					if (dir == 0 && wp.maxX <= 60)
					{
						targetWp = wp;
						break;
					}
					if (dir == 1 && wp.minX >= TileMap.pxw - 60)
					{
						targetWp = wp;
						break;
					}
					if (dir == 2 && wp.minX > 60 && wp.maxX < TileMap.pxw - 60)
					{
						targetWp = wp;
						break;
					}
				}

				if (targetWp == null)
				{
					if (dir == 0)
					{
						int minX = int.MaxValue;
						for (int i = 0; i < TileMap.vGo.size(); i++)
						{
							Waypoint wp = (Waypoint)TileMap.vGo.elementAt(i);
							if (wp != null && wp.minX < TileMap.pxw / 3 && wp.minX < minX)
							{
								minX = wp.minX;
								targetWp = wp;
							}
						}
					}
					else if (dir == 1)
					{
						int maxX = int.MinValue;
						for (int i = 0; i < TileMap.vGo.size(); i++)
						{
							Waypoint wp = (Waypoint)TileMap.vGo.elementAt(i);
							if (wp != null && wp.maxX > (TileMap.pxw * 2) / 3 && wp.maxX > maxX)
							{
								maxX = wp.maxX;
								targetWp = wp;
							}
						}
					}
				}
			}

			if (targetWp != null)
			{
				int tx = ModWaypoint.GetWaypointTargetX(targetWp);
				int ty = ModWaypoint.GetWaypointTargetY(targetWp, tx);
				ModTeleport.TeleportTo(tx, ty);
				Char.ischangingMap = true;
				if (targetWp.isOffline || TileMap.isOfflineMap() || TileMap.isTrainingMap())
				{
					Service.gI().getMapOffline();
				}
				else
				{
					Service.gI().requestChangeMap();
				}
				return;
			}

			if (dir == 2 && (currMap == 45 || currMap == 46 || currMap == 48))
			{
				int tx = System.Math.Max(48, System.Math.Min(TileMap.pxw - 48, me.cx));
				ModTeleport.TeleportTo(tx, TileMap.pxh - 12);
				Char.ischangingMap = true;
				Service.gI().requestChangeMap();
				return;
			}

			GameScr.info1.addInfo("Không có cổng qua map ở hướng này!", 0);
		}
		catch
		{
		}
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
		if (me == null || Char.isLoadingMap || Controller.isStopReadMessage || Controller.gI().messWait != null)
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
			else
			{
				long watchdogTimeoutMs = (Controller.lastServerPacketTime >= lastChangeAttemptTime) ? 2500 : 12000;
				if (now - lastChangeAttemptTime > watchdogTimeoutMs)
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
					me.cxSend = -1;
					ModWaypoint.ResetWaypointState();
				}
			}
			return;
		}

		if (nextMapCooldown > 0)
		{
			nextMapCooldown--;
			return;
		}

		int stepIntervalMs = ModTeleport.IsColdPlanetAntiCheckActive()
			? ModTeleport.ColdStepIntervalMs
			: ((useSafeMovement || resetPenaltyCount > 0) ? 260 : 180);
		if (now < mapEnterWaitUntil || now - lastStepUpdateTime < stepIntervalMs)
		{
			return;
		}
		lastStepUpdateTime = now;

		ModNextMapData.SyncLiveWaypointsForCurrentMap();
		if (nextMapTargetId == 160 && TileMap.mapID != 160 && now - s_LastCapsuleUseTime > 3000)
		{
			int ringIdx = FindBagItemIndex(me, 992, 1);
			if (ringIdx >= 0)
			{
				s_LastCapsuleUseTime = now;
				Service.gI().useItem(0, 1, (sbyte)ringIdx, -1);
				mapEnterWaitUntil = now + 800;
				return;
			}
		}

		if (s_WaitingCapsuleMenu)
		{
			if (now - s_LastCapsuleUseTime < 1800)
			{
				return;
			}
			s_WaitingCapsuleMenu = false;
		}

		List<int> path = FindPath(TileMap.mapID, nextMapTargetId);
		if (path == null || path.Count == 0)
		{
			GameScr.info1.addInfo("Không tìm thấy đường đi đến " + GetMapName(nextMapTargetId), 0);
			StopNextMap();
			return;
		}

		if (path.Count >= 4 && (useCapsuleVip || useNormalCapsule) && now - s_LastCapsuleUseTime > 6000 &&
		    !TileMap.isInAirMap() && !(TileMap.mapID >= 85 && TileMap.mapID <= 91) && !ModTeleport.IsColdPlanetAntiCheckActive())
		{
			int capIdx = useCapsuleVip ? FindBagItemIndex(me, 194, 1) : -1;
			if (capIdx < 0 && useNormalCapsule)
			{
				capIdx = FindBagItemIndex(me, 193, 1);
			}
			if (capIdx >= 0)
			{
				s_WaitingCapsuleMenu = true;
				s_LastCapsuleUseTime = now;
				Service.gI().useItem(0, 1, (sbyte)capIdx, -1);
				return;
			}
		}

		int nextMapId = path[0];

		// Xử lý các chặng chuyển map thông qua NPC (Trạm tàu vũ trụ 10/11/12, Cui 12 đi Nappa/Cold, Tapion 60 đi Yardrat, Calích 38 đi Tương lai, Thần Vũ Trụ 20)
		if (ModNextMapData.IsNpcTransitionEdge(TileMap.mapID, nextMapId))
		{
			Npc transitionNpc = FindTransitionNpc(TileMap.mapID, nextMapId);
			if (transitionNpc != null)
			{
				currentTargetWaypoint = null;
				nextMapFailCount = 0;
				bool confirmedNpc = ModWaypoint.UseSpaceShip(transitionNpc, nextMapId);
				if (confirmedNpc)
				{
					lastChangeAttemptTime = now;
					mapEnterWaitUntil = now + 600;
				}
				return;
			}
			if ((TileMap.mapID == 27 || TileMap.mapID == 28 || TileMap.mapID == 29) && nextMapId == 102)
			{
				// NPC Calích không xuất hiện ở map hiện tại -> Loại bỏ cạnh tạm thời để đi sang map kế tiếp trong cụm 27-28-29 tìm Calích
				if (ModNextMapData.mapWaypoints.TryGetValue(TileMap.mapID, out List<int> currList) && currList.Remove(102))
				{
					ModNextMapPathFinder.ClearCache();
					return;
				}
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
			if (nextMapFailCount >= 2 &&
			    TileMap.vGo != null && TileMap.vGo.size() > 0 &&
			    ModNextMapData.mapWaypoints.TryGetValue(TileMap.mapID, out List<int> currNeighbors) &&
			    currNeighbors.Count > 1 &&
			    currNeighbors.Remove(nextMapId))
			{
				ModNextMapPathFinder.ClearCache();
				nextMapFailCount = 0;
				return;
			}
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

			mFont mapTagFont = isNextMapActive
				? (mFont.tahoma_7b_white ?? mFont.tahoma_7_white ?? mFont.tahoma_7b_yellow)
				: (mFont.tahoma_7b_yellow ?? mFont.tahoma_7_yellow ?? mFont.tahoma_7_white);
			if (mapTagFont != null)
			{
				mapTagFont.drawString(g, mapTag, drawX, drawY, mFont.LEFT);
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
