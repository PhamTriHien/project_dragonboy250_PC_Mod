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
	public const int ColdStepIntervalMs = 92;
	private const int ColdMaxWalkStepX = 76;
	private const int ColdTeleportMaxTokens = 1;
	private const int ColdTeleportRefillMs = 11000;
	private const int ColdTeleportBurstGapMs = 2500;
	private const int ColdMaxPlanetResetPoints = 95;
	private const int ColdMaxSessionResetPoints = 120;
	private static readonly int[] ColdDirX = new int[4] { 0, -1, 1, 0 };
	private static readonly int[] ColdDirY = new int[4] { -1, 0, 0, 1 };
	public static int totalSessionResetPoints = 0;
	public static int coldPlanetResetPoints = 0;
	private static int coldTrackedTargetX = -1;
	private static int coldTrackedTargetY = -1;
	private static long coldTrackedStartTime = 0;
	private static int coldTeleportTokens = ColdTeleportMaxTokens;
	private static long lastColdTokenRefillTime = 0;
	private static long lastColdLongTeleportTime = 0;
	private static int coldSessionLongTeleportCount = 0;
	private static int[] bfsQueue = new int[4096];
	private static int[] bfsParent = new int[4096];
	private static int[] bfsDist = new int[4096];
	private static int[] bfsPathBuffer = new int[512];
	private static int cachedCostMapId = -1;
	private static int cachedCostStartIdx = -1;
	private static bool cachedCostStrict = true;
	private static long cachedCostTime = 0;

	public static bool IsColdPlanetMap()
	{
		return TileMap.mapID >= 105 && TileMap.mapID <= 110;
	}

	public static bool IsColdPlanetAntiCheckActive()
	{
		return IsColdPlanetMap() && !ItemTime.isExistItem(4387);
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
		if (IsColdPlanetMap())
		{
			coldPlanetResetPoints++;
		}
	}

	public static void ResetSessionColdCounters()
	{
		totalSessionResetPoints = 0;
		coldPlanetResetPoints = 0;
		coldTeleportTokens = ColdTeleportMaxTokens;
		lastColdTokenRefillTime = 0;
		lastColdLongTeleportTime = 0;
		coldSessionLongTeleportCount = 0;
		cachedCostMapId = -1;
		cachedCostStartIdx = -1;
		cachedCostTime = 0;
	}

	public static bool IsColdPositionBlocked(int px, int py)
	{
		if (px <= 24 || px >= TileMap.pxw - 24 || py <= 24 || py >= TileMap.pxh - 24)
		{
			return true;
		}
		if (IsPointUndergroundOrBlocked(px, py))
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
		if (now - lastStepTime < ColdStepIntervalMs)
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
				int stepY = me.cy + ClampInt(candY - me.cy, -72, 72);
				SendSingleCharMove(me.cx, stepY);
				lastStepTime = now;
				isCurrentPosSynced = true;
				isWaitingSettle = (stepY != candY);
				return true;
			}
		}
		return false;
	}

	private static void RefreshColdTeleportTokens(long now)
	{
		if (coldTeleportTokens >= ColdTeleportMaxTokens)
		{
			lastColdTokenRefillTime = now;
			return;
		}
		if (lastColdTokenRefillTime <= 0)
		{
			lastColdTokenRefillTime = now;
			return;
		}
		long elapsed = now - lastColdTokenRefillTime;
		if (elapsed >= ColdTeleportRefillMs)
		{
			int gained = (int)(elapsed / ColdTeleportRefillMs);
			coldTeleportTokens = System.Math.Min(ColdTeleportMaxTokens, coldTeleportTokens + gained);
			if (coldTeleportTokens >= ColdTeleportMaxTokens)
			{
				lastColdTokenRefillTime = now;
			}
			else
			{
				lastColdTokenRefillTime += (long)gained * ColdTeleportRefillMs;
			}
		}
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
		if (now - lastSyncRequestTime > 4500)
		{
			isWaitingServerSync = false;
			return false;
		}
		return true;
	}

	/// <summary>
	/// Kiểm tra xem có đang trong nhịp chờ máy chủ xác nhận tọa độ trước khi tung đòn đánh đầu tiên hay không (tối đa 650ms).
	/// </summary>
	public static bool IsWaitingServerSyncForAttack()
	{
		if (!isWaitingServerSync)
		{
			return false;
		}
		long now = mSystem.currentTimeMillis();
		int maxWait = IsColdPlanetAntiCheckActive() ? 420 : 650;
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
			bfsQueue = new int[total];
		}
		for (int i = 0; i < total; i++)
		{
			bfsParent[i] = -1;
		}

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
				if (bfsParent[nIdx] != -1)
				{
					continue;
				}
				if (!IsColdCellPassable(nx, ny, strictAir, startIdx, goalIdx))
				{
					continue;
				}
				bfsParent[nIdx] = cur;
				bfsQueue[tail++] = nIdx;
			}
		}

		if (bfsParent[goalIdx] == -1)
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

		if (cachedCostMapId != TileMap.mapID || cachedCostStartIdx != startIdx || cachedCostStrict != useStrict || now - cachedCostTime > 90 || bfsDist.Length < total)
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

	private static bool IsFlatGroundContinuous(int fromX, int toX, int y)
	{
		if (fromX == toX)
		{
			return TileMap.tileTypeAt(toX, y, TileMap.T_TOP) && !IsPointUndergroundOrBlocked(toX, y);
		}
		bool startInWaypoint = ModTanSatTargeting.IsInsideWaypoint(fromX, y);
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

	private static bool IsColdAirSegmentClear(int fromX, int fromY, int toX, int toY, bool strictAir)
	{
		bool startInWaypoint = ModTanSatTargeting.IsInsideWaypoint(fromX, fromY);
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

	private static bool IsColdLandingSegmentClear(int fromX, int fromY, int toX, int toY)
	{
		if (IsPointUndergroundOrBlocked(toX, toY))
		{
			return false;
		}
		bool startInWaypoint = ModTanSatTargeting.IsInsideWaypoint(fromX, fromY);
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

	private static bool MoveColdAntiCheckTo(Char me, int targetX, int targetY, bool isTargetOnGround, bool allowLongTeleport, long now)
	{
		if (Res.abs(targetX - coldTrackedTargetX) > 36 || Res.abs(targetY - coldTrackedTargetY) > 36 || coldTrackedStartTime <= 0)
		{
			coldTrackedTargetX = targetX;
			coldTrackedTargetY = targetY;
			coldTrackedStartTime = now;
		}

		int dx = Res.abs(me.cx - targetX);
		int dy = Res.abs(me.cy - targetY);
		bool mustLandExact = isTargetOnGround && !IsPointUndergroundOrBlocked(targetX, targetY);
		int arriveTolY = mustLandExact ? 2 : 16;

		bool conservativeSpeed = (ModTanSat.lastResetPenaltyTime > 0 && now - ModTanSat.lastResetPenaltyTime < 6000) || (me.cspeed > 0 && me.cspeed < 4);
		int maxFlyAxis = conservativeSpeed ? 48 : 76;
		int maxFlyManhattan = conservativeSpeed ? 72 : 104;
		int maxClampStep = conservativeSpeed ? 48 : 72;

		if (dx <= maxFlyAxis && dy <= maxFlyAxis && (dx + dy) <= maxFlyManhattan && IsColdLandingSegmentClear(me.cx, me.cy, targetX, targetY))
		{
			SendSingleCharMove(targetX, targetY);
			lastStepTime = now;
			isCurrentPosSynced = true;
			isWaitingSettle = false;
			ResetColdStepTracking();
			return true;
		}

		if (TileMap.tileTypeAt(me.cx, me.cy, TileMap.T_TOP) && me.cy == targetY && dx <= 640 && IsFlatGroundContinuous(me.cx, targetX, targetY))
		{
			int maxWalk = conservativeSpeed ? 48 : ColdMaxWalkStepX;
			int walkX = (dx <= maxWalk) ? targetX : (me.cx + ((targetX > me.cx) ? maxWalk : (-maxWalk)));
			SendSingleCharMove(walkX, targetY);
			lastStepTime = now;
			isCurrentPosSynced = true;
			bool walkDone = Res.abs(walkX - targetX) <= 12;
			isWaitingSettle = !walkDone;
			if (walkDone)
			{
				ResetColdStepTracking();
			}
			return walkDone;
		}

		bool hasPath = FindColdAirPath(me.cx, me.cy, targetX, targetY, out int pathLen, out bool strictAirUsed) && pathLen > 0;
		RefreshColdTeleportTokens(now);
		bool hasTeleportBudget = allowLongTeleport &&
		                         coldTeleportTokens > 0 &&
		                         (now - lastColdLongTeleportTime >= ColdTeleportBurstGapMs) &&
		                         coldPlanetResetPoints < ColdMaxPlanetResetPoints &&
		                         totalSessionResetPoints < ColdMaxSessionResetPoints &&
		                         coldSessionLongTeleportCount < ColdMaxPlanetResetPoints;
		bool isLongJump = (hasPath && pathLen >= 13) || (!hasPath && (dx > 120 || dy > 96)) || (dx + dy) >= 400;
		if (hasTeleportBudget && isLongJump)
		{
			if (mustLandExact && !ModTanSatTargeting.IsInsideWaypoint(targetX, targetY))
			{
				if (coldTeleportTokens == ColdTeleportMaxTokens)
				{
					lastColdTokenRefillTime = now;
				}
				coldTeleportTokens--;
				lastColdLongTeleportTime = now;
				coldSessionLongTeleportCount++;
				ResetColdStepTracking();
				TeleportTo(targetX, targetY);
				lastStepTime = now;
				isWaitingSettle = false;
				return true;
			}
			if (!mustLandExact && dx > 144)
			{
				int groundAnchorY = ModTanSatTargeting.FindGroundY(targetX, targetY, 240);
				if (groundAnchorY != -1 &&
				    !IsPointUndergroundOrBlocked(targetX, groundAnchorY) &&
				    !ModTanSatTargeting.IsInsideWaypoint(targetX, groundAnchorY) &&
				    (dx > 144 || Res.abs(me.cy - groundAnchorY) > 144))
				{
					if (coldTeleportTokens == ColdTeleportMaxTokens)
					{
						lastColdTokenRefillTime = now;
					}
					coldTeleportTokens--;
					lastColdLongTeleportTime = now;
					coldSessionLongTeleportCount++;
					TeleportTo(targetX, groundAnchorY);
					lastStepTime = now;
					isWaitingSettle = true;
					return false;
				}
			}
		}

		if (hasPath)
		{
			int w = TileMap.tmw;
			for (int i = pathLen - 1; i >= 0; i--)
			{
				int cellIdx = bfsPathBuffer[i];
				GetColdCellPixel(cellIdx % w, cellIdx / w, out int candX, out int candY);
				int sdx = Res.abs(candX - me.cx);
				int sdy = Res.abs(candY - me.cy);
				if (sdx > maxFlyAxis || sdy > maxFlyAxis || (sdx + sdy) > maxFlyManhattan)
				{
					continue;
				}
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
						if (landed)
						{
							ResetColdStepTracking();
						}
						return landed;
					}
					continue;
				}
				if (!IsColdAirSegmentClear(me.cx, me.cy, candX, candY, strictAirUsed))
				{
					continue;
				}

				SendSingleCharMove(candX, candY);
				lastStepTime = now;
				isCurrentPosSynced = true;
				bool arrived = Res.abs(candX - targetX) <= 12 && Res.abs(candY - targetY) <= arriveTolY;
				isWaitingSettle = !arrived;
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
		if (fbDone)
		{
			ResetColdStepTracking();
		}
		return fbDone;
	}

	private static bool IsPointUndergroundOrBlocked(int px, int py)
	{
		if (ModTanSatTargeting.IsTileBlocked(px, py))
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
		me.cxSend = (toX != fromX) ? fromX : (toX - 1);
		me.cySend = (toY != fromY) ? fromY : (toY - 1);
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
			if (!ItemTime.isExistItem(4387))
			{
				RegisterTeleportEcho(x, y);
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
	/// Điều phối dịch chuyển Tàn Sát và Auto Nhặt:
	/// - Khi nhích ngắn theo quái đi bộ ngay sát bên (dx <= 48, dy <= 24):
	///   Sử dụng SendSingleCharMove (1 gói charMove hợp lệ trong tốc độ đi bộ, không gửi y+1).
	/// - Khi dịch chuyển sang quái mới hoặc điểm mặt đất cách xa hơn 48px (TileMap.tileTypeAt(targetX, targetY, 2)):
	///   Sử dụng trực tiếp TeleportTo(targetX, targetY) 1 bước duy nhất, tránh đi bộ 2 bước nhanh bị máy chủ phạt RESET_POINT kéo ngược về vị trí cũ.
	/// - Khi mục tiêu là quái bay giữa không trung (!TileMap.tileTypeAt(targetX, targetY, 2)):
	///   Dịch chuyển tức thời tới mặt đất gần nhất bằng TeleportTo rồi bước lên không trung bằng SendSingleCharMove.
	/// </summary>
	public static bool MoveSteppedTo(int targetX, int targetY, bool isTargetFlying = false, bool allowColdLongTeleport = false)
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
			int arriveTolY = (coldAntiCheck && isTargetOnGround && !IsPointUndergroundOrBlocked(targetX, targetY)) ? 2 : 16;

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

			int minStepInterval = coldAntiCheck ? ColdStepIntervalMs : 120;
			if (now - lastStepTime < minStepInterval)
			{
				if (coldAntiCheck && !TileMap.tileTypeAt(me.cx, me.cy, 2))
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
				return MoveColdAntiCheckTo(me, targetX, targetY, isTargetOnGround, allowColdLongTeleport, now);
			}

			// 1. Nhích ngắn theo quái đi bộ ngay sát bên (dx <= 48, dy <= 24): Đi 1 gói đơn trong ngưỡng tốc độ hợp lệ, không kích hoạt RESET_POINT
			if (dx <= 48 && dy <= 24 && !IsPointUndergroundOrBlocked(targetX, targetY))
			{
				SendSingleCharMove(targetX, targetY);
				lastStepTime = now;
				isWaitingSettle = false;
				return true;
			}

			// 2. Dịch chuyển sang quái khác hoặc nhảy xa/chuyển tầng tới mặt đất: Dịch chuyển tức thời 1 bước
			if (isTargetOnGround)
			{
				TeleportTo(targetX, targetY);
				lastStepTime = now;
				isWaitingSettle = false;
				return true;
			}

			// 3. Mục tiêu là quái bay trên không trung: Nếu còn ở xa, dịch chuyển tức thời tới mặt đất gần quái bay trước, sau đó bay lên
			if (dx > 96 || dy > 84)
			{
				int groundY = ModTanSatTargeting.FindGroundY(targetX, targetY, 240);
				if (groundY != -1 && (Res.abs(me.cx - targetX) > 48 || Res.abs(me.cy - groundY) > 24))
				{
					TeleportTo(targetX, groundY);
					lastStepTime = now;
					isWaitingSettle = true;
					return false;
				}
			}

			int airX = (dx <= 96) ? targetX : (me.cx + ((targetX > me.cx) ? 96 : (-96)));
			int airY = (dy <= 84) ? targetY : (me.cy + ((targetY > me.cy) ? 84 : (-84)));
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
