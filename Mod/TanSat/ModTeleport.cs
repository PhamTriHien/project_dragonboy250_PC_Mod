using System;
using System.Collections.Generic;

/// <summary>
/// CLASS ModTeleport: Điều phối dịch chuyển Tàn Sát và Auto Nhặt sử dụng đồng nhất 100% cơ chế ModWaypoint (NextMap).
/// </summary>
public static class ModTeleport
{
	private struct TeleportEchoEntry
	{
		public int x;
		public int y;
		public long time;
	}

	public static long lastStepTime = 0;
	public static int lastStepDistance = 0;
	public static bool isWaitingSettle = false;
	public static bool isWaitingServerSync = false;
	public static bool isCurrentPosSynced = false;
	public static int pendingSyncX = 0;
	public static int pendingSyncY = 0;
	public static long lastSyncRequestTime = 0;
	private static readonly List<TeleportEchoEntry> pendingTeleportEchoes = new List<TeleportEchoEntry>(64);

	private const int ColdSolidMask = TileMap.T_CENTER | TileMap.T_LEFT | TileMap.T_RIGHT | TileMap.T_SOLIDGROUND | TileMap.T_BOTTOM | TileMap.T_DIE;
	public const int ColdStepIntervalMs = 82;
	private const int ColdMaxWalkStepX = 84;
	private static int nextColdStepDelayMs = ColdStepIntervalMs;
	private static readonly int[] ColdDirX = new int[4] { 0, -1, 1, 0 };
	private static readonly int[] ColdDirY = new int[4] { -1, 0, 0, 1 };
	public static int totalSessionResetPoints = 0;
	public static int coldPlanetResetPoints = 0;
	private static int coldTrackedTargetX = -1;
	private static int coldTrackedTargetY = -1;
	private static long coldTrackedStartTime = 0;
	private static int[] bfsQueue = new int[4096];
	private static int[] bfsParent = new int[4096];
	private static int[] bfsVisitStamp = new int[4096];
	private static int currentBfsStamp = 0;
	private static int[] bfsDist = new int[4096];
	private static int[] bfsPathBuffer = new int[512];
	private static int cachedCostMapId = -1;
	private static int cachedCostStartIdx = -1;
	private static bool cachedCostStrict = true;
	private static long cachedCostTime = 0;

	public static int GetColdStepDelayMs()
	{
		return nextColdStepDelayMs;
	}

	private static void RollNextColdStepDelay(bool arrived)
	{
		nextColdStepDelayMs = arrived ? Res.random(76, 86) : Res.random(78, 91);
	}

	public static bool IsColdPlanetMap()
	{
		return TileMap.mapID >= 105 && TileMap.mapID <= 110;
	}

	public static bool IsAntiScanModeActive()
	{
		return ModTanSat.antiScanMode;
	}

	public static bool IsColdPlanetAntiCheckActive()
	{
		return IsAntiScanModeActive();
	}

	public static bool IsColdStepTimedOut()
	{
		return IsColdPlanetAntiCheckActive() && coldTrackedStartTime > 0 && (mSystem.currentTimeMillis() - coldTrackedStartTime > 6500);
	}

	public static void ResetColdStepTracking()
	{
		coldTrackedTargetX = -1;
		coldTrackedTargetY = -1;
		coldTrackedStartTime = 0;
	}

	public static void NotifyServerResetPointReceived()
	{
		totalSessionResetPoints++;
		if (IsColdPlanetMap() || IsAntiScanModeActive())
		{
			coldPlanetResetPoints++;
		}
	}

	public static void ResetSessionColdCounters()
	{
		totalSessionResetPoints = 0;
		coldPlanetResetPoints = 0;
		cachedCostMapId = -1;
		cachedCostStartIdx = -1;
		cachedCostTime = 0;
		ModGoBack.ResetColdReturnSafety();
	}

	public static bool IsColdDirectAirReachable(int fromX, int fromY, int toX, int toY)
	{
		if (IsColdPositionBlocked(toX, toY))
		{
			return false;
		}
		int startY = TileMap.tileTypeAt(fromX, fromY, TileMap.T_TOP) ? System.Math.Max(24, fromY - 4) : fromY;
		return IsColdAirSegmentClear(fromX, startY, toX, toY, strictAir: false);
	}

	private static void ComputeProportionalColdStep(int fromX, int fromY, int toX, int toY, int maxAxis, int maxManhattan, out int outX, out int outY)
	{
		int remDx = toX - fromX;
		int remDy = toY - fromY;
		int absDx = Res.abs(remDx);
		int absDy = Res.abs(remDy);
		if (absDx <= maxAxis && absDy <= maxAxis && (absDx + absDy) <= maxManhattan)
		{
			outX = toX;
			outY = toY;
			return;
		}

		int maxCoord = System.Math.Max(1, System.Math.Max(absDx, absDy));
		int sumCoord = System.Math.Max(1, absDx + absDy);
		double scaleAxis = (double)maxAxis / maxCoord;
		double scaleManh = (double)maxManhattan / sumCoord;
		double scale = System.Math.Min(scaleAxis, scaleManh);

		int stepX = (int)System.Math.Round(remDx * scale);
		int stepY = (int)System.Math.Round(remDy * scale);
		stepX = ClampInt(stepX, -maxAxis, maxAxis);
		stepY = ClampInt(stepY, -maxAxis, maxAxis);
		while (Res.abs(stepX) + Res.abs(stepY) > maxManhattan)
		{
			if (Res.abs(stepX) >= Res.abs(stepY))
			{
				stepX += (stepX > 0) ? -1 : 1;
			}
			else
			{
				stepY += (stepY > 0) ? -1 : 1;
			}
		}
		if (stepX == 0 && remDx != 0 && Res.abs(stepY) < maxManhattan)
		{
			stepX = (remDx > 0) ? 1 : -1;
		}
		if (stepY == 0 && remDy != 0 && Res.abs(stepX) < maxManhattan)
		{
			stepY = (remDy > 0) ? 1 : -1;
		}
		outX = fromX + stepX;
		outY = fromY + stepY;
	}

	public static bool IsColdPositionBlocked(int px, int py, bool allowEnterWaypoint = false)
	{
		int margin = allowEnterWaypoint ? 12 : 24;
		if (px <= margin || (TileMap.pxw > 0 && px >= TileMap.pxw - margin) || py <= margin || (TileMap.pxh > 0 && py >= TileMap.pxh - margin))
		{
			return true;
		}
		if (IsPointUndergroundOrBlocked(px, py, allowEnterWaypoint))
		{
			return true;
		}
		return (TileMap.tileTypeAtPixel(px, py) & ColdSolidMask) != 0;
	}

	public static bool TryEscapeColdUnderground(Char me, long now)
	{
		if (me == null || !IsColdPlanetAntiCheckActive())
		{
			return false;
		}
		bool insideSolid = (TileMap.tileTypeAtPixel(me.cx, me.cy) & ColdSolidMask) != 0;
		bool undergroundTop = TileMap.tileTypeAt(me.cx, me.cy, 2) && (me.cy % 24 != 0 || (me.cy >= 24 && TileMap.tileTypeAt(me.cx, me.cy - 24, 2)));
		if (!insideSolid && !undergroundTop)
		{
			return false;
		}
		if (now - lastStepTime < nextColdStepDelayMs)
		{
			return true;
		}

		int alignedY = (me.cy / 24) * 24;
		for (int dyScan = 0; dyScan <= 120; dyScan += 24)
		{
			int candY = alignedY - dyScan;
			if (candY <= 24)
			{
				break;
			}
			if (TileMap.tileTypeAt(me.cx, candY, 2) && !IsPointUndergroundOrBlocked(me.cx, candY))
			{
				int stepY = me.cy + ClampInt(candY - me.cy, -84, 84);
				SendSingleCharMove(me.cx, stepY);
				lastStepTime = now;
				isCurrentPosSynced = true;
				isWaitingSettle = (stepY != candY);
				RollNextColdStepDelay(!isWaitingSettle);
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Ghi nhận tọa độ vừa gửi gói dịch chuyển để nhận diện phản hồi xác nhận RESET_POINT(x, y) từ máy chủ.
	/// </summary>
	public static void RegisterTeleportEcho(int x, int y)
	{
		long now = mSystem.currentTimeMillis();
		for (int i = pendingTeleportEchoes.Count - 1; i >= 0; i--)
		{
			if (now - pendingTeleportEchoes[i].time > 15000)
			{
				pendingTeleportEchoes.RemoveAt(i);
			}
		}
		if (pendingTeleportEchoes.Count >= 64)
		{
			pendingTeleportEchoes.RemoveAt(0);
		}
		pendingTeleportEchoes.Add(new TeleportEchoEntry { x = x, y = y, time = now });
		isWaitingServerSync = true;
		isCurrentPosSynced = false;
		pendingSyncX = x;
		pendingSyncY = y;
		lastSyncRequestTime = now;
	}

	/// <summary>
	/// Kiểm tra xem có đang chờ máy chủ phản hồi gói RESET_POINT xác nhận vị trí dịch chuyển trước đó hay không.
	/// Bắt buộc đợi đồng bộ xong (hoặc hết thời gian chờ tối đa 4500ms) mới dịch chuyển tiếp để tránh dồn hàng đợi gây lag,
	/// đồng thời giữ nguyên danh sách pendingTeleportEchoes để nhận diện các gói RESET_POINT trả về muộn.
	/// </summary>
	public static bool IsWaitingServerSync()
	{
		if (!isWaitingServerSync)
		{
			return false;
		}
		long now = mSystem.currentTimeMillis();
		int maxBlockMs = IsColdPlanetAntiCheckActive() ? 750 : 900;
		if (now - lastSyncRequestTime > maxBlockMs)
		{
			isWaitingServerSync = false;
			return false;
		}
		return true;
	}

	/// <summary>
	/// Kiểm tra xem có đang trong nhịp chờ máy chủ xác nhận tọa độ trước khi tung đòn đánh đầu tiên hay không (tối đa 380ms).
	/// </summary>
	public static bool IsWaitingServerSyncForAttack()
	{
		if (!isWaitingServerSync)
		{
			return false;
		}
		long now = mSystem.currentTimeMillis();
		int maxWait = IsColdPlanetAntiCheckActive() ? 320 : 380;
		if (now - lastSyncRequestTime > maxWait)
		{
			return false;
		}
		return true;
	}

	/// <summary>
	/// Kiểm tra xem tọa độ (x, y) hiện tại đã có trong hàng đợi chờ phản hồi RESET_POINT từ máy chủ hay chưa.
	/// </summary>
	public static bool HasPendingEchoNear(int x, int y)
	{
		long now = mSystem.currentTimeMillis();
		for (int i = pendingTeleportEchoes.Count - 1; i >= 0; i--)
		{
			TeleportEchoEntry entry = pendingTeleportEchoes[i];
			if (now - entry.time > 15000)
			{
				continue;
			}
			if (Res.abs(entry.x - x) <= 32 && (Res.abs(entry.y - y) <= 32 || (y > entry.y && TileMap.tileTypeAt(x, y, 2))))
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Kiểm tra và tiêu thụ gói phản hồi xác nhận tọa độ RESET_POINT(x, y) sinh ra bởi dịch chuyển trước đó.
	/// Không xóa các echo mới hơn đang chờ trên đường truyền để tránh nhận nhầm gói RESET_POINT kế tiếp là lệnh phạt.
	/// </summary>
	public static bool TryConsumeTeleportEcho(int x, int y, int currentCx, int currentCy, out bool isCurrentPos, out bool hasNewerPendingEcho)
	{
		long now = mSystem.currentTimeMillis();
		for (int i = pendingTeleportEchoes.Count - 1; i >= 0; i--)
		{
			if (now - pendingTeleportEchoes[i].time > 15000)
			{
				pendingTeleportEchoes.RemoveAt(i);
			}
		}

		isCurrentPos = (Res.abs(x - currentCx) <= 32 && Res.abs(y - currentCy) <= 32);

		for (int i = 0; i < pendingTeleportEchoes.Count; i++)
		{
			TeleportEchoEntry entry = pendingTeleportEchoes[i];
			if (Res.abs(entry.x - x) <= 32 && (Res.abs(entry.y - y) <= 32 || (y > entry.y && TileMap.tileTypeAt(x, y, 2))))
			{
				pendingTeleportEchoes.RemoveRange(0, i + 1);
				hasNewerPendingEcho = (pendingTeleportEchoes.Count > 0);
				if (isCurrentPos || !hasNewerPendingEcho || (Res.abs(x - pendingSyncX) <= 32 && Res.abs(y - pendingSyncY) <= 32))
				{
					isWaitingServerSync = false;
					isCurrentPosSynced = isCurrentPos;
					if (isCurrentPos)
					{
						pendingTeleportEchoes.Clear();
					}
				}
				return true;
			}
		}

		hasNewerPendingEcho = (pendingTeleportEchoes.Count > 0);
		if (isCurrentPos)
		{
			isWaitingServerSync = false;
			isCurrentPosSynced = true;
			pendingTeleportEchoes.Clear();
		}
		return false;
	}

	public static void ClearEchoes()
	{
		pendingTeleportEchoes.Clear();
		isWaitingSettle = false;
		isWaitingServerSync = false;
		isCurrentPosSynced = false;
		pendingSyncX = 0;
		pendingSyncY = 0;
		lastSyncRequestTime = 0;
		cachedCostMapId = -1;
		cachedCostStartIdx = -1;
		cachedCostTime = 0;
		ResetColdStepTracking();
	}

	private static int ClampInt(int value, int min, int max)
	{
		if (value < min)
		{
			return min;
		}
		if (value > max)
		{
			return max;
		}
		return value;
	}

	private static void GetColdCellPixel(int tx, int ty, out int px, out int py)
	{
		px = tx * 24 + 12;
		bool hasCeilingAbove = ty > 0 && (TileMap.tileTypeAt(tx, ty - 1) & TileMap.T_BOTTOM) != 0;
		py = hasCeilingAbove ? (ty * 24 + 20) : (ty * 24 + 12);
	}

	private static bool IsColdCellPassable(int tx, int ty, bool strictAir, int startIdx, int goalIdx)
	{
		if (tx <= 0 || tx >= TileMap.tmw - 1 || ty <= 0 || ty >= TileMap.tmh - 1)
		{
			return false;
		}
		int t = TileMap.tileTypeAt(tx, ty);
		if ((t & ColdSolidMask) != 0)
		{
			return false;
		}
		if (strictAir)
		{
			if ((t & TileMap.T_TOP) != 0)
			{
				return false;
			}
		}
		else if ((t & TileMap.T_TOP) != 0)
		{
			if (ty > 0 && (TileMap.tileTypeAt(tx, ty - 1) & (TileMap.T_TOP | ColdSolidMask)) != 0)
			{
				return false;
			}
		}

		int idx = ty * TileMap.tmw + tx;
		if (idx != startIdx && idx != goalIdx)
		{
			GetColdCellPixel(tx, ty, out int px, out int py);
			if (ModTanSatTargeting.IsInsideWaypoint(px, py))
			{
				return false;
			}
		}
		return true;
	}

	private static bool ResolveColdAirCell(int px, int py, bool strictAir, out int outTx, out int outTy)
	{
		int baseTx = ClampInt(px / 24, 1, TileMap.tmw - 2);
		int baseTy = ClampInt(py / 24, 1, TileMap.tmh - 2);
		int baseIdx = baseTy * TileMap.tmw + baseTx;
		if (IsColdCellPassable(baseTx, baseTy, strictAir, baseIdx, -1))
		{
			outTx = baseTx;
			outTy = baseTy;
			return true;
		}
		if (baseTy > 1)
		{
			int aboveIdx = (baseTy - 1) * TileMap.tmw + baseTx;
			if (IsColdCellPassable(baseTx, baseTy - 1, strictAir, aboveIdx, -1))
			{
				outTx = baseTx;
				outTy = baseTy - 1;
				return true;
			}
		}
		for (int r = 1; r <= 3; r++)
		{
			for (int dy = -r; dy <= r; dy++)
			{
				for (int dx = -r; dx <= r; dx++)
				{
					int ntx = baseTx + dx;
					int nty = baseTy + dy;
					if (ntx <= 0 || ntx >= TileMap.tmw - 1 || nty <= 0 || nty >= TileMap.tmh - 1)
					{
						continue;
					}
					int candIdx = nty * TileMap.tmw + ntx;
					if (IsColdCellPassable(ntx, nty, strictAir, candIdx, -1))
					{
						outTx = ntx;
						outTy = nty;
						return true;
					}
				}
			}
		}
		outTx = baseTx;
		outTy = System.Math.Max(1, baseTy - 1);
		return false;
	}

	private static bool RunColdBfs(int startTx, int startTy, int goalTx, int goalTy, bool strictAir, out int pathLen)
	{
		int w = TileMap.tmw;
		int h = TileMap.tmh;
		int total = w * h;
		if (bfsParent.Length < total)
		{
			bfsParent = new int[total];
			bfsVisitStamp = new int[total];
			bfsQueue = new int[total];
			currentBfsStamp = 0;
		}
		currentBfsStamp++;
		if (currentBfsStamp <= 0)
		{
			Array.Clear(bfsVisitStamp, 0, bfsVisitStamp.Length);
			currentBfsStamp = 1;
		}
		int stamp = currentBfsStamp;

		int startIdx = startTy * w + startTx;
		int goalIdx = goalTy * w + goalTx;
		if (startIdx == goalIdx)
		{
			bfsPathBuffer[0] = startIdx;
			pathLen = 1;
			return true;
		}

		int head = 0;
		int tail = 0;
		bfsQueue[tail++] = startIdx;
		bfsVisitStamp[startIdx] = stamp;
		bfsParent[startIdx] = startIdx;

		while (head < tail)
		{
			int cur = bfsQueue[head++];
			if (cur == goalIdx)
			{
				break;
			}
			int cx = cur % w;
			int cy = cur / w;
			for (int d = 0; d < 4; d++)
			{
				int nx = cx + ColdDirX[d];
				int ny = cy + ColdDirY[d];
				if (nx <= 0 || nx >= w - 1 || ny <= 0 || ny >= h - 1)
				{
					continue;
				}
				int nIdx = ny * w + nx;
				if (bfsVisitStamp[nIdx] == stamp)
				{
					continue;
				}
				if (!IsColdCellPassable(nx, ny, strictAir, startIdx, goalIdx))
				{
					continue;
				}
				bfsVisitStamp[nIdx] = stamp;
				bfsParent[nIdx] = cur;
				bfsQueue[tail++] = nIdx;
			}
		}

		if (bfsVisitStamp[goalIdx] != stamp)
		{
			pathLen = 0;
			return false;
		}

		int len = 0;
		int trace = goalIdx;
		while (trace != startIdx && len < total)
		{
			len++;
			trace = bfsParent[trace];
		}
		len++;

		if (bfsPathBuffer.Length < len)
		{
			bfsPathBuffer = new int[len + 64];
		}
		trace = goalIdx;
		for (int i = len - 1; i >= 0; i--)
		{
			bfsPathBuffer[i] = trace;
			trace = bfsParent[trace];
		}
		pathLen = len;
		return true;
	}

	public static int GetColdTravelTileCost(int fromX, int fromY, int toX, int toY)
	{
		if (TileMap.types == null || TileMap.tmw <= 2 || TileMap.tmh <= 2)
		{
			return Res.distance(fromX, fromY, toX, toY) / 24;
		}
		bool useStrict = true;
		if (!ResolveColdAirCell(fromX, fromY, strictAir: true, out int sTx, out int sTy))
		{
			useStrict = false;
			if (!ResolveColdAirCell(fromX, fromY, strictAir: false, out sTx, out sTy))
			{
				return Res.distance(fromX, fromY, toX, toY) / 24;
			}
		}

		int w = TileMap.tmw;
		int h = TileMap.tmh;
		int total = w * h;
		int startIdx = sTy * w + sTx;
		long now = mSystem.currentTimeMillis();

		if (cachedCostMapId != TileMap.mapID || cachedCostStartIdx != startIdx || cachedCostStrict != useStrict || now - cachedCostTime > 10000 || bfsDist.Length < total)
		{
			if (bfsDist.Length < total)
			{
				bfsDist = new int[total];
			}
			if (bfsQueue.Length < total)
			{
				bfsQueue = new int[total];
			}
			for (int i = 0; i < total; i++)
			{
				bfsDist[i] = -1;
			}

			int head = 0;
			int tail = 0;
			bfsQueue[tail++] = startIdx;
			bfsDist[startIdx] = 0;

			while (head < tail)
			{
				int cur = bfsQueue[head++];
				int curDist = bfsDist[cur];
				int cx = cur % w;
				int cy = cur / w;
				for (int d = 0; d < 4; d++)
				{
					int nx = cx + ColdDirX[d];
					int ny = cy + ColdDirY[d];
					if (nx <= 0 || nx >= w - 1 || ny <= 0 || ny >= h - 1)
					{
						continue;
					}
					int nIdx = ny * w + nx;
					if (bfsDist[nIdx] != -1)
					{
						continue;
					}
					if (!IsColdCellPassable(nx, ny, strictAir: useStrict, startIdx, -1))
					{
						continue;
					}
					bfsDist[nIdx] = curDist + 1;
					bfsQueue[tail++] = nIdx;
				}
			}

			cachedCostMapId = TileMap.mapID;
			cachedCostStartIdx = startIdx;
			cachedCostStrict = useStrict;
			cachedCostTime = now;
		}

		if (ResolveColdAirCell(toX, toY, strictAir: useStrict, out int gTx, out int gTy))
		{
			int gIdx = gTy * w + gTx;
			if (gIdx >= 0 && gIdx < total && bfsDist[gIdx] >= 0)
			{
				return bfsDist[gIdx];
			}
		}

		return 120 + (Res.distance(fromX, fromY, toX, toY) / 24);
	}

	private static bool FindColdAirPath(int fromX, int fromY, int toX, int toY, out int pathLen, out bool strictAirUsed)
	{
		pathLen = 0;
		strictAirUsed = true;
		if (TileMap.types == null || TileMap.tmw <= 2 || TileMap.tmh <= 2)
		{
			return false;
		}

		if (ResolveColdAirCell(fromX, fromY, strictAir: true, out int sTx, out int sTy) &&
		    ResolveColdAirCell(toX, toY, strictAir: true, out int gTx, out int gTy) &&
		    RunColdBfs(sTx, sTy, gTx, gTy, strictAir: true, out pathLen))
		{
			strictAirUsed = true;
			return true;
		}

		if (ResolveColdAirCell(fromX, fromY, strictAir: false, out sTx, out sTy) &&
		    ResolveColdAirCell(toX, toY, strictAir: false, out gTx, out gTy) &&
		    RunColdBfs(sTx, sTy, gTx, gTy, strictAir: false, out pathLen))
		{
			strictAirUsed = false;
			return true;
		}

		return false;
	}

	private static bool IsValidColdSurfaceAt(int px, int gy)
	{
		if (px < 24 || (TileMap.pxw > 0 && px > TileMap.pxw - 24))
		{
			return false;
		}
		if (gy < 24 || (TileMap.pxh > 0 && gy > TileMap.pxh - 24))
		{
			return false;
		}
		if (!TileMap.tileTypeAt(px, gy, TileMap.T_TOP))
		{
			return false;
		}
		if (gy % 24 != 0 || (gy >= 24 && TileMap.tileTypeAt(px, gy - 24, TileMap.T_TOP)))
		{
			return false;
		}
		int tWaist = TileMap.tileTypeAtPixel(px, System.Math.Max(0, gy - 12));
		int tHead = TileMap.tileTypeAtPixel(px, System.Math.Max(0, gy - 22));
		return (tWaist & ColdSolidMask) == 0 && (tHead & ColdSolidMask) == 0;
	}

	public static bool IsSameColdGroundBase(int fromX, int fromY, int toX, int toY)
	{
		return TryTraceSameColdGroundBase(fromX, fromY, toX, toY, ColdMaxWalkStepX, out _, out _);
	}

	public static bool IsColdSameFlatPlatform(int fromX, int fromY, int toX, int toY)
	{
		return IsSameColdGroundBase(fromX, fromY, toX, toY);
	}

	private static bool TryTraceSameColdGroundBase(int fromX, int fromY, int toX, int toY, int maxWalkStepX, out int nextWalkX, out int nextWalkY, bool allowEnterWaypoint = false)
	{
		nextWalkX = fromX;
		nextWalkY = fromY;

		int startGy;
		if (IsValidColdSurfaceAt(fromX, fromY))
		{
			startGy = fromY;
		}
		else
		{
			int gy = ModTanSatTargeting.FindGroundY(fromX, fromY, 12);
			if (gy == -1 || Res.abs(gy - fromY) > 12 || !IsValidColdSurfaceAt(fromX, gy))
			{
				return false;
			}
			startGy = gy;
		}

		int endGy;
		if (IsValidColdSurfaceAt(toX, toY))
		{
			endGy = toY;
		}
		else
		{
			int gy = ModTanSatTargeting.FindGroundY(toX, toY, 24);
			if (gy == -1 || Res.abs(gy - toY) > 24 || !IsValidColdSurfaceAt(toX, gy))
			{
				return false;
			}
			endGy = gy;
		}

		if (startGy == endGy && fromY == startGy && toY == endGy && IsFlatGroundContinuous(fromX, toX, startGy, allowEnterWaypoint))
		{
			int flatDx = Res.abs(toX - fromX);
			nextWalkX = (flatDx <= maxWalkStepX) ? toX : (fromX + ((toX > fromX) ? maxWalkStepX : (-maxWalkStepX)));
			nextWalkY = startGy;
			return true;
		}

		bool startInWp = allowEnterWaypoint || ModTanSatTargeting.IsInsideWaypoint(fromX, startGy);
		int totalDx = Res.abs(toX - fromX);
		int steps = System.Math.Max(1, (totalDx + 15) / 16);
		int curY = startGy;
		int prevSx = fromX;
		nextWalkX = fromX;
		nextWalkY = startGy;

		for (int s = 1; s <= steps; s++)
		{
			int sx = (s == steps) ? toX : (fromX + (toX - fromX) * s / steps);
			int matchedY = -1;

			if (IsValidColdSurfaceAt(sx, curY) && (startInWp || !ModTanSatTargeting.IsInsideWaypoint(sx, curY)))
			{
				matchedY = curY;
			}
			else if (curY >= 48 &&
			         IsValidColdSurfaceAt(sx, curY - 24) &&
			         (TileMap.tileTypeAtPixel(prevSx, System.Math.Max(0, curY - 46)) & ColdSolidMask) == 0 &&
			         (startInWp || !ModTanSatTargeting.IsInsideWaypoint(sx, curY - 24)))
			{
				matchedY = curY - 24;
			}
			else if (IsValidColdSurfaceAt(sx, curY + 24) &&
			         (TileMap.tileTypeAtPixel(sx, System.Math.Max(0, curY - 22)) & ColdSolidMask) == 0 &&
			         (startInWp || !ModTanSatTargeting.IsInsideWaypoint(sx, curY + 24)))
			{
				matchedY = curY + 24;
			}

			if (matchedY == -1)
			{
				return false;
			}

			curY = matchedY;
			prevSx = sx;
			if (Res.abs(sx - fromX) <= maxWalkStepX && Res.abs(curY - fromY) <= 84)
			{
				nextWalkX = sx;
				nextWalkY = curY;
			}
		}

		if (Res.abs(curY - endGy) > 24)
		{
			return false;
		}
		if (totalDx <= maxWalkStepX && Res.abs(endGy - fromY) <= 84)
		{
			nextWalkX = toX;
			nextWalkY = endGy;
		}
		return true;
	}

	private static bool IsFlatGroundContinuous(int fromX, int toX, int y, bool allowEnterWaypoint = false)
	{
		if (fromX == toX)
		{
			return TileMap.tileTypeAt(toX, y, TileMap.T_TOP) && !IsPointUndergroundOrBlocked(toX, y, allowEnterWaypoint);
		}
		bool startInWaypoint = allowEnterWaypoint || ModTanSatTargeting.IsInsideWaypoint(fromX, y);
		int dist = Res.abs(toX - fromX);
		int steps = System.Math.Max(1, (dist + 15) / 16);
		for (int s = 1; s <= steps; s++)
		{
			int sx = fromX + (toX - fromX) * s / steps;
			if (!TileMap.tileTypeAt(sx, y, TileMap.T_TOP))
			{
				return false;
			}
			if (y % 24 != 0 || (y >= 24 && TileMap.tileTypeAt(sx, y - 24, TileMap.T_TOP)))
			{
				return false;
			}
			int tWaist = TileMap.tileTypeAtPixel(sx, System.Math.Max(0, y - 12));
			int tHead = TileMap.tileTypeAtPixel(sx, System.Math.Max(0, y - 22));
			if ((tWaist & ColdSolidMask) != 0 || (tHead & ColdSolidMask) != 0)
			{
				return false;
			}
			if (!startInWaypoint && ModTanSatTargeting.IsInsideWaypoint(sx, y))
			{
				return false;
			}
		}
		return true;
	}

	private static bool IsColdAirSegmentClear(int fromX, int fromY, int toX, int toY, bool strictAir, bool allowEnterWaypoint = false)
	{
		bool startInWaypoint = allowEnterWaypoint || ModTanSatTargeting.IsInsideWaypoint(fromX, fromY);
		if (!startInWaypoint && ModTanSatTargeting.IsInsideWaypoint(toX, toY))
		{
			return false;
		}
		int dist = System.Math.Max(Res.abs(toX - fromX), Res.abs(toY - fromY));
		int steps = System.Math.Max(2, (dist + 7) / 8);
		for (int s = 1; s <= steps; s++)
		{
			int sx = fromX + (toX - fromX) * s / steps;
			int sy = fromY + (toY - fromY) * s / steps;
			int tFoot = TileMap.tileTypeAtPixel(sx, sy);
			int tWaist = TileMap.tileTypeAtPixel(sx, System.Math.Max(0, sy - 12));
			int tHead = TileMap.tileTypeAtPixel(sx, System.Math.Max(0, sy - 16));
			if ((tFoot & ColdSolidMask) != 0 || (tWaist & ColdSolidMask) != 0 || (tHead & TileMap.T_BOTTOM) != 0)
			{
				return false;
			}
			if (strictAir && (tFoot & TileMap.T_TOP) != 0)
			{
				return false;
			}
			if (!startInWaypoint && ModTanSatTargeting.IsInsideWaypoint(sx, sy))
			{
				return false;
			}
		}
		return true;
	}

	private static bool IsColdLandingSegmentClear(int fromX, int fromY, int toX, int toY, bool allowEnterWaypoint = false)
	{
		if (IsPointUndergroundOrBlocked(toX, toY, allowEnterWaypoint))
		{
			return false;
		}
		bool startInWaypoint = allowEnterWaypoint || ModTanSatTargeting.IsInsideWaypoint(fromX, fromY);
		int dist = System.Math.Max(Res.abs(toX - fromX), Res.abs(toY - fromY));
		int steps = System.Math.Max(2, (dist + 7) / 8);
		for (int s = 1; s < steps; s++)
		{
			int sx = fromX + (toX - fromX) * s / steps;
			int sy = fromY + (toY - fromY) * s / steps;
			int tFoot = TileMap.tileTypeAtPixel(sx, sy);
			int tWaist = TileMap.tileTypeAtPixel(sx, System.Math.Max(0, sy - 12));
			int tHead = TileMap.tileTypeAtPixel(sx, System.Math.Max(0, sy - 16));
			if ((tFoot & ColdSolidMask) != 0 || (tWaist & ColdSolidMask) != 0 || (tHead & TileMap.T_BOTTOM) != 0)
			{
				return false;
			}
			if ((tFoot & TileMap.T_TOP) != 0 && sy != toY)
			{
				return false;
			}
			if (!startInWaypoint && ModTanSatTargeting.IsInsideWaypoint(sx, sy))
			{
				return false;
			}
		}
		return true;
	}

	private static bool TryFindColdFlyingAnchor(int targetX, int targetY, int preferFromX, out int anchorX, out int anchorY)
	{
		anchorX = targetX;
		anchorY = -1;
		int preferredSign = (preferFromX <= targetX) ? -1 : 1;

		for (int offset = 0; offset <= 144; offset += 24)
		{
			int attempts = (offset == 0) ? 1 : 2;
			for (int a = 0; a < attempts; a++)
			{
				int dir = (a == 0) ? preferredSign : (-preferredSign);
				int candX = (offset == 0) ? targetX : (targetX + dir * offset);
				if (candX < 24 || (TileMap.pxw > 0 && candX > TileMap.pxw - 24))
				{
					continue;
				}
				for (int dy = 0; dy <= 480; dy += 12)
				{
					int sampleY = targetY + dy;
					if (TileMap.pxh > 0 && sampleY > TileMap.pxh - 12)
					{
						break;
					}
					if (!TileMap.tileTypeAt(candX, sampleY, TileMap.T_TOP))
					{
						continue;
					}
					int gy = TileMap.tileYofPixel(sampleY);
					int guard = 0;
					while (gy >= 24 && guard < 64 && TileMap.tileTypeAt(candX, gy - 24, TileMap.T_TOP))
					{
						gy -= 24;
						guard++;
					}
					if (gy >= targetY - 16 &&
					    IsValidColdSurfaceAt(candX, gy) &&
					    !ModTanSatTargeting.IsInsideWaypoint(candX, gy) &&
					    IsColdAirSegmentClear(candX, System.Math.Max(24, gy - 4), targetX, targetY, strictAir: false))
					{
						anchorX = candX;
						anchorY = gy;
						return true;
					}
					if (gy > sampleY)
					{
						dy = gy - targetY;
					}
				}
			}
		}

		int fallbackGy = ModTanSatTargeting.FindGroundY(targetX, targetY, 360);
		if (fallbackGy != -1 &&
		    fallbackGy >= targetY - 24 &&
		    !IsPointUndergroundOrBlocked(targetX, fallbackGy) &&
		    !ModTanSatTargeting.IsInsideWaypoint(targetX, fallbackGy))
		{
			anchorX = targetX;
			anchorY = fallbackGy;
			return true;
		}
		return false;
	}

	private static bool MoveColdAntiCheckTo(Char me, int targetX, int targetY, bool isTargetOnGround, bool allowLongTeleport, long now, bool allowEnterWaypoint = false)
	{
		if (Res.abs(targetX - coldTrackedTargetX) > 36 || Res.abs(targetY - coldTrackedTargetY) > 36 || coldTrackedStartTime <= 0)
		{
			coldTrackedTargetX = targetX;
			coldTrackedTargetY = targetY;
			coldTrackedStartTime = now;
		}

		int dx = Res.abs(me.cx - targetX);
		int dy = Res.abs(me.cy - targetY);
		bool mustLandExact = isTargetOnGround && !IsPointUndergroundOrBlocked(targetX, targetY, allowEnterWaypoint);
		int arriveTolY = mustLandExact ? 2 : 16;

		bool conservativeSpeed = (ModTanSat.lastResetPenaltyTime > 0 && now - ModTanSat.lastResetPenaltyTime < 3200) || (me.cspeed > 0 && me.cspeed < 4);
		int maxFlyAxisCeiling = conservativeSpeed ? 48 : 84;
		int maxFlyManhattanCeiling = conservativeSpeed ? 72 : 132;
		bool canFinishInOneAirHop = dx <= maxFlyAxisCeiling && dy <= maxFlyAxisCeiling && (dx + dy) <= maxFlyManhattanCeiling;
		int maxFlyAxis = conservativeSpeed ? 48 : (canFinishInOneAirHop ? 84 : Res.random(76, 85));
		int maxFlyManhattan = conservativeSpeed ? 72 : (canFinishInOneAirHop ? 132 : Res.random(120, 133));
		int maxClampStep = conservativeSpeed ? 48 : 84;
		int minWalkStride = (me.cspeed >= 6) ? 78 : 74;
		int maxWalk = conservativeSpeed
			? ((dx <= 52) ? 52 : Res.random(44, 51))
			: ((dx <= ColdMaxWalkStepX) ? ColdMaxWalkStepX : Res.random(minWalkStride, ColdMaxWalkStepX + 1));

		if (isTargetOnGround && TryTraceSameColdGroundBase(me.cx, me.cy, targetX, targetY, maxWalk, out int walkX, out int walkY, allowEnterWaypoint))
		{
			SendSingleCharMove(walkX, walkY);
			lastStepTime = now;
			isCurrentPosSynced = true;
			bool walkDone = Res.abs(walkX - targetX) <= 12 && Res.abs(walkY - targetY) <= arriveTolY;
			isWaitingSettle = !walkDone;
			RollNextColdStepDelay(walkDone);
			if (walkDone)
			{
				ResetColdStepTracking();
			}
			return walkDone;
		}

		bool onTopSurface = TileMap.tileTypeAt(me.cx, me.cy, TileMap.T_TOP);
		int startAirY = onTopSurface ? System.Math.Max(24, me.cy - 4) : me.cy;
		int landingStartY = (onTopSurface && targetY <= me.cy) ? startAirY : me.cy;
		bool canStepDirect = mustLandExact
			? IsColdLandingSegmentClear(me.cx, landingStartY, targetX, targetY, allowEnterWaypoint)
			: (!IsColdPositionBlocked(targetX, targetY, allowEnterWaypoint) && IsColdAirSegmentClear(me.cx, startAirY, targetX, targetY, strictAir: false, allowEnterWaypoint: allowEnterWaypoint));
		if (canStepDirect && canFinishInOneAirHop)
		{
			SendSingleCharMove(targetX, targetY);
			lastStepTime = now;
			isCurrentPosSynced = true;
			isWaitingSettle = false;
			RollNextColdStepDelay(arrived: true);
			ResetColdStepTracking();
			return true;
		}

		int aimX = targetX;
		int aimY = mustLandExact ? System.Math.Max(24, targetY - 12) : targetY;
		bool strictDirectAir = mustLandExact;
		if (!IsColdPositionBlocked(aimX, aimY, allowEnterWaypoint) && IsColdAirSegmentClear(me.cx, startAirY, aimX, aimY, strictAir: strictDirectAir, allowEnterWaypoint: allowEnterWaypoint))
		{
			if (mustLandExact && canFinishInOneAirHop)
			{
				SendSingleCharMove(targetX, targetY);
				lastStepTime = now;
				isCurrentPosSynced = true;
				isWaitingSettle = false;
				RollNextColdStepDelay(arrived: true);
				ResetColdStepTracking();
				return true;
			}
			ComputeProportionalColdStep(me.cx, me.cy, aimX, aimY, maxFlyAxis, maxFlyManhattan, out int vecX, out int vecY);
			if (onTopSurface && aimY < me.cy && vecY > me.cy - 4)
			{
				vecY = System.Math.Max(24, me.cy - 4);
			}
			if ((vecX != me.cx || vecY != me.cy) &&
			    !IsColdPositionBlocked(vecX, vecY, allowEnterWaypoint) &&
			    IsColdAirSegmentClear(me.cx, startAirY, vecX, vecY, strictAir: strictDirectAir, allowEnterWaypoint: allowEnterWaypoint))
			{
				SendSingleCharMove(vecX, vecY);
				lastStepTime = now;
				isCurrentPosSynced = true;
				bool vecDone = !mustLandExact && Res.abs(vecX - targetX) <= 12 && Res.abs(vecY - targetY) <= arriveTolY;
				isWaitingSettle = !vecDone;
				RollNextColdStepDelay(vecDone);
				if (vecDone)
				{
					ResetColdStepTracking();
				}
				return vecDone;
			}
		}

		bool hasPath = FindColdAirPath(me.cx, me.cy, targetX, targetY, out int pathLen, out bool strictAirUsed) && pathLen > 0;

		if (hasPath)
		{
			int w = TileMap.tmw;
			for (int i = pathLen - 1; i >= 0; i--)
			{
				int wayX, wayY;
				if (i == pathLen - 1 && !IsColdPositionBlocked(aimX, aimY, allowEnterWaypoint))
				{
					wayX = aimX;
					wayY = aimY;
				}
				else
				{
					int cellIdx = bfsPathBuffer[i];
					GetColdCellPixel(cellIdx % w, cellIdx / w, out wayX, out wayY);
				}

				if (i < pathLen - 1 &&
				    ((targetX < me.cx && wayX > me.cx) || (targetX > me.cx && wayX < me.cx)) &&
				    Res.abs(wayX - me.cx) <= 16 &&
				    Res.abs(wayY - me.cy) > 3 &&
				    IsColdAirSegmentClear(me.cx, startAirY, me.cx, wayY, strictAirUsed, allowEnterWaypoint))
				{
					wayX = me.cx;
				}

				int sdx = Res.abs(wayX - me.cx);
				int sdy = Res.abs(wayY - me.cy);
				if (sdx <= 3 && sdy <= 3)
				{
					if (i == pathLen - 1)
					{
						int landX = me.cx + ClampInt(targetX - me.cx, -maxClampStep, maxClampStep);
						int landY = me.cy + ClampInt(targetY - me.cy, -maxClampStep, maxClampStep);
						SendSingleCharMove(landX, landY);
						lastStepTime = now;
						isCurrentPosSynced = true;
						bool landed = Res.abs(landX - targetX) <= 12 && Res.abs(landY - targetY) <= arriveTolY;
						isWaitingSettle = !landed;
						RollNextColdStepDelay(landed);
						if (landed)
						{
							ResetColdStepTracking();
						}
						return landed;
					}
					continue;
				}
				if (!IsColdAirSegmentClear(me.cx, startAirY, wayX, wayY, strictAirUsed, allowEnterWaypoint))
				{
					continue;
				}
				if (i == pathLen - 1 && mustLandExact && canFinishInOneAirHop && (strictAirUsed || IsColdLandingSegmentClear(me.cx, landingStartY, targetX, targetY, allowEnterWaypoint)))
				{
					SendSingleCharMove(targetX, targetY);
					lastStepTime = now;
					isCurrentPosSynced = true;
					isWaitingSettle = false;
					RollNextColdStepDelay(arrived: true);
					ResetColdStepTracking();
					return true;
				}

				ComputeProportionalColdStep(me.cx, me.cy, wayX, wayY, maxFlyAxis, maxFlyManhattan, out int candX, out int candY);
				if (onTopSurface && wayY < me.cy && candY > me.cy - 4)
				{
					candY = System.Math.Max(24, me.cy - 4);
				}
				if ((candX == me.cx && candY == me.cy) ||
				    IsColdPositionBlocked(candX, candY, allowEnterWaypoint) ||
				    !IsColdAirSegmentClear(me.cx, startAirY, candX, candY, strictAirUsed, allowEnterWaypoint))
				{
					continue;
				}

				SendSingleCharMove(candX, candY);
				lastStepTime = now;
				isCurrentPosSynced = true;
				bool arrived = !mustLandExact && Res.abs(candX - targetX) <= 12 && Res.abs(candY - targetY) <= arriveTolY;
				isWaitingSettle = !arrived;
				RollNextColdStepDelay(arrived);
				if (arrived)
				{
					ResetColdStepTracking();
				}
				return arrived;
			}

			int fallbackIdx = bfsPathBuffer[System.Math.Min(1, pathLen - 1)];
			GetColdCellPixel(fallbackIdx % w, fallbackIdx / w, out int nextX, out int nextY);
			int stepX = me.cx + ClampInt(nextX - me.cx, -maxClampStep, maxClampStep);
			int stepY = me.cy + ClampInt(nextY - me.cy, -maxClampStep, maxClampStep);
			SendSingleCharMove(stepX, stepY);
			lastStepTime = now;
			isCurrentPosSynced = true;
			bool stepDone = Res.abs(stepX - targetX) <= 12 && Res.abs(stepY - targetY) <= arriveTolY;
			isWaitingSettle = !stepDone;
			RollNextColdStepDelay(stepDone);
			if (stepDone)
			{
				ResetColdStepTracking();
			}
			return stepDone;
		}

		int safeApexY = System.Math.Max(48, System.Math.Min(me.cy, targetY) - 48);
		int fbX = me.cx;
		int fbY = me.cy;
		if (dx > 48 && me.cy > safeApexY && (TileMap.tileTypeAtPixel(me.cx, System.Math.Max(24, me.cy - 48)) & ColdSolidMask) == 0)
		{
			fbY = System.Math.Max(safeApexY, me.cy - 48);
		}
		else if (dx > 12)
		{
			fbX = me.cx + ClampInt(targetX - me.cx, -48, 48);
		}
		else
		{
			fbY = me.cy + ClampInt(targetY - me.cy, -48, 48);
		}
		SendSingleCharMove(fbX, fbY);
		lastStepTime = now;
		isCurrentPosSynced = true;
		bool fbDone = Res.abs(fbX - targetX) <= 12 && Res.abs(fbY - targetY) <= arriveTolY;
		isWaitingSettle = !fbDone;
		RollNextColdStepDelay(fbDone);
		if (fbDone)
		{
			ResetColdStepTracking();
		}
		return fbDone;
	}

	private static bool IsPointUndergroundOrBlocked(int px, int py, bool allowEnterWaypoint = false)
	{
		if (allowEnterWaypoint)
		{
			if (px < 12 || (TileMap.pxw > 0 && px > TileMap.pxw - 12) || py < 12 || (TileMap.pxh > 0 && py > TileMap.pxh - 12))
			{
				return true;
			}
			int tWaist = TileMap.tileTypeAtPixel(px, System.Math.Max(0, py - 12));
			int tHead = TileMap.tileTypeAtPixel(px, System.Math.Max(0, py - 22));
			if ((tWaist & ColdSolidMask) != 0 || (tHead & ColdSolidMask) != 0)
			{
				return true;
			}
		}
		else if (ModTanSatTargeting.IsTileBlocked(px, py))
		{
			return true;
		}
		if (TileMap.tileTypeAt(px, py, 2) && py % 24 != 0)
		{
			return true;
		}
		if (py >= 24 && TileMap.tileTypeAt(px, py - 24, 2))
		{
			return true;
		}
		return false;
	}

	/// <summary>
	/// Gửi duy nhất 1 gói charMove hợp lệ từ vị trí hiện tại tới (toX, toY),
	/// tuyệt đối không gửi gói sai lệch (x, y+1) hay chuỗi gói 0ms để không kích hoạt bộ đếm RESET_POINT của máy chủ.
	/// </summary>
	public static void SendSingleCharMove(int toX, int toY)
	{
		Char me = Char.myCharz();
		if (me == null)
		{
			return;
		}
		int fromX = me.cx;
		int fromY = me.cy;
		bool wasPosSynced = isCurrentPosSynced;
		if (toX == fromX && toY == fromY)
		{
			me.cxSend = toX - 1;
			me.cySend = toY - 1;
		}
		else
		{
			me.cxSend = fromX;
			me.cySend = (toY != fromY || !wasPosSynced) ? ((toY != fromY) ? fromY : (toY - 1)) : fromY;
		}
		me.cx = toX;
		me.cy = toY;
		me.cvx = 0;
		me.cvy = 0;
		me.statusMe = 1;
		me.delayFall = 0;
		int stepDx = Res.abs(toX - fromX);
		int stepDy = Res.abs(toY - fromY);
		if (stepDx > 48 || stepDy > 24)
		{
			isCurrentPosSynced = false;
		}
		Service.gI().charMove();
		me.cxSend = toX;
		me.cySend = toY;
	}

	/// <summary>
	/// Đồng bộ bước di chuyển hợp lệ tới (toX, toY) trong 1 gói charMove duy nhất.
	/// </summary>
	public static void StepMoveFrom(int fromX, int fromY, int toX, int toY)
	{
		SendSingleCharMove(toX, toY);
	}

	/// <summary>
	/// Dịch chuyển nhân vật đến tọa độ đích và đăng ký echo đồng bộ máy chủ cho Tàn Sát / Auto Nhặt.
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
			if (!ItemTime.isExistItem(4387) && TileMap.tileTypeAt(x, y, 2))
			{
				RegisterTeleportEcho(x, y);
				me.cx = x;
				me.cy = y + 1;
				Service.gI().charMove();
				me.cx = x;
				me.cy = y;
				Service.gI().charMove();
			}
			else
			{
				isWaitingServerSync = false;
			}
		}
		catch
		{
		}
	}

	/// <summary>
	/// Điều phối dịch chuyển Tàn Sát và Auto Nhặt:
	/// - Khi nhích ngắn theo quái đi bộ ngay sát bên (dx <= 48, dy <= 24):
	///   Sử dụng SendSingleCharMove (1 gói charMove hợp lệ trong tốc độ đi bộ, không gửi y+1).
	/// - Khi dịch chuyển sang quái mới hoặc điểm mặt đất cách xa hơn 48px (TileMap.tileTypeAt(targetX, targetY, 2)):
	///   Sử dụng trực tiếp TeleportTo(targetX, targetY) 1 bước duy nhất, tránh đi bộ 2 bước nhanh bị máy chủ phạt RESET_POINT kéo ngược về vị trí cũ.
	/// - Khi mục tiêu là quái bay giữa không trung (!TileMap.tileTypeAt(targetX, targetY, 2)):
	///   Dịch chuyển tức thời tới mặt đất gần nhất bằng TeleportTo rồi bước lên không trung bằng SendSingleCharMove.
	/// </summary>
	public static bool MoveSteppedTo(int targetX, int targetY, bool isTargetFlying = false, bool allowColdLongTeleport = false, bool allowEnterWaypoint = false)
	{
		try
		{
			Char me = Char.myCharz();
			if (me == null)
			{
				isWaitingSettle = false;
				return false;
			}

			long now = mSystem.currentTimeMillis();
			int dx = Res.abs(me.cx - targetX);
			int dy = Res.abs(me.cy - targetY);

			bool coldAntiCheck = IsColdPlanetAntiCheckActive();
			bool isTargetOnGround = TileMap.tileTypeAt(targetX, targetY, 2) && !isTargetFlying;
			int arriveTolY = (coldAntiCheck && isTargetOnGround && !IsPointUndergroundOrBlocked(targetX, targetY, allowEnterWaypoint)) ? 2 : 16;

			if (dx <= 12 && dy <= arriveTolY)
			{
				isWaitingSettle = false;
				if (coldAntiCheck)
				{
					isCurrentPosSynced = true;
					ResetColdStepTracking();
				}
				return true;
			}

			if (IsWaitingServerSync())
			{
				return false;
			}

			int minStepInterval = coldAntiCheck ? nextColdStepDelayMs : 120;
			if (now - lastStepTime < minStepInterval)
			{
				if (!TileMap.tileTypeAt(me.cx, me.cy, 2))
				{
					me.statusMe = 1;
					me.delayFall = 0;
					me.cvx = 0;
					me.cvy = 0;
				}
				return false;
			}

			me.cdir = (targetX >= me.cx) ? 1 : (-1);
			if (coldAntiCheck)
			{
				return MoveColdAntiCheckTo(me, targetX, targetY, isTargetOnGround, allowColdLongTeleport, now, allowEnterWaypoint);
			}

			// 1. Nhích ngắn theo quái đi bộ ngay sát bên (dx <= 48, dy <= 24): Đi 1 gói đơn trong ngưỡng tốc độ hợp lệ, không kích hoạt RESET_POINT
			if (dx <= 48 && dy <= 24 && !IsPointUndergroundOrBlocked(targetX, targetY))
			{
				SendSingleCharMove(targetX, targetY);
				lastStepTime = now;
				isWaitingSettle = false;
				return true;
			}

			// 2. Dịch chuyển sang quái khác hoặc nhảy xa/chuyển tầng tới mặt đất (hoặc bay thẳng 1 bước khi đang có Tự Động Luyện Tập 4387)
			if (isTargetOnGround || (ItemTime.isExistItem(4387) && !IsColdPositionBlocked(targetX, targetY)))
			{
				TeleportTo(targetX, targetY);
				lastStepTime = now;
				isWaitingSettle = false;
				return true;
			}

			// 3. Mục tiêu là quái bay trên không trung khi không có TĐLT 4387:
			//    - Nếu nhân vật đang lơ lửng trên không và đường bay sang quái bay kế tiếp thông thoáng: bay ngang/chéo trực tiếp trên không, không tụt xuống mặt đất.
			//    - Chỉ khi đứng từ xa ở dưới đất mới đáp xuống nền base dưới chân quái bay trước rồi bay thẳng lên.
			bool onTopSurface = TileMap.tileTypeAt(me.cx, me.cy, 2);
			bool canFlyAirToAir = !onTopSurface &&
			                      dx <= 280 &&
			                      dy <= 192 &&
			                      !IsColdPositionBlocked(targetX, targetY) &&
			                      IsColdAirSegmentClear(me.cx, me.cy, targetX, targetY, strictAir: false);
			if (!canFlyAirToAir && (dx > 84 || dy > 84 || (dx + dy) > 108))
			{
				if (TryFindColdFlyingAnchor(targetX, targetY, me.cx, out int anchorX, out int anchorY))
				{
					bool alreadyClimbingAir = !onTopSurface &&
					                          Res.abs(me.cx - targetX) <= 120 &&
					                          me.cy <= anchorY + 8 &&
					                          Res.abs(me.cy - targetY) <= 380;
					if (!alreadyClimbingAir && (Res.abs(me.cx - anchorX) > 48 || Res.abs(me.cy - anchorY) > 24))
					{
						TeleportTo(anchorX, anchorY);
						lastStepTime = now;
						isWaitingSettle = true;
						return false;
					}
				}
			}

			ComputeProportionalColdStep(me.cx, me.cy, targetX, targetY, 84, 108, out int airX, out int airY);
			if (onTopSurface && targetY < me.cy && airY > me.cy - 4)
			{
				airY = System.Math.Max(24, me.cy - 4);
			}
			SendSingleCharMove(airX, airY);
			lastStepTime = now;

			if (Res.abs(airX - targetX) <= 12 && Res.abs(airY - targetY) <= 16)
			{
				isWaitingSettle = false;
				return true;
			}

			isWaitingSettle = true;
			return false;
		}
		catch
		{
			isWaitingSettle = false;
			return false;
		}
	}
}
