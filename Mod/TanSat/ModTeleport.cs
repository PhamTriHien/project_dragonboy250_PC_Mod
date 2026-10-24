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
		if (now - lastSyncRequestTime > 650)
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
	/// Dịch chuyển nhân vật đến tọa độ đích sử dụng trực tiếp ModWaypoint.TeleportTo của NextMap.
	/// </summary>
	public static void TeleportTo(int targetX, int targetY)
	{
		ModWaypoint.TeleportTo(targetX, targetY);
	}

	/// <summary>
	/// Điều phối dịch chuyển Tàn Sát và Auto Nhặt theo đúng cơ chế NextMap (ModWaypoint.TeleportTo):
	/// - Khi nhích ngắn theo quái đi bộ ngay sát bên (dx <= 48, dy <= 24):
	///   Sử dụng SendSingleCharMove (1 gói charMove hợp lệ trong tốc độ đi bộ, không gửi y+1).
	/// - Khi dịch chuyển sang quái mới hoặc điểm mặt đất cách xa hơn 48px (TileMap.tileTypeAt(targetX, targetY, 2)):
	///   Sử dụng trực tiếp ModWaypoint.TeleportTo(targetX, targetY) 1 bước duy nhất như NextMap, tránh đi bộ 2 bước nhanh bị máy chủ phạt RESET_POINT kéo ngược về vị trí cũ.
	/// - Khi mục tiêu là quái bay giữa không trung (!TileMap.tileTypeAt(targetX, targetY, 2)):
	///   Dịch chuyển tức thời tới mặt đất gần nhất bằng ModWaypoint.TeleportTo rồi bước lên không trung bằng SendSingleCharMove.
	/// </summary>
	public static bool MoveSteppedTo(int targetX, int targetY, bool isTargetFlying = false)
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

			if (dx <= 12 && dy <= 16)
			{
				isWaitingSettle = false;
				return true;
			}

			if (IsWaitingServerSync())
			{
				return false;
			}

			if (now - lastStepTime < 120)
			{
				return false;
			}

			me.cdir = (targetX >= me.cx) ? 1 : (-1);
			bool isTargetOnGround = TileMap.tileTypeAt(targetX, targetY, 2) && !isTargetFlying;

			// 1. Nhích ngắn theo quái đi bộ ngay sát bên (dx <= 48, dy <= 24): Đi 1 gói đơn trong ngưỡng tốc độ hợp lệ, không kích hoạt RESET_POINT
			if (dx <= 48 && dy <= 24 && !IsPointUndergroundOrBlocked(targetX, targetY))
			{
				SendSingleCharMove(targetX, targetY);
				lastStepTime = now;
				isWaitingSettle = false;
				return true;
			}

			// 2. Dịch chuyển sang quái khác hoặc nhảy xa/chuyển tầng tới mặt đất: Dịch chuyển tức thời 1 bước bằng đúng ModWaypoint.TeleportTo của NextMap!
			if (isTargetOnGround)
			{
				ModWaypoint.TeleportTo(targetX, targetY);
				lastStepTime = now;
				isWaitingSettle = false;
				return true;
			}

			// 3. Mục tiêu là quái bay trên không trung: Nếu còn ở xa, dịch chuyển tức thời bằng ModWaypoint.TeleportTo tới mặt đất gần quái bay trước, sau đó bay lên
			if (dx > 96 || dy > 84)
			{
				int groundY = ModTanSatTargeting.FindGroundY(targetX, targetY, 240);
				if (groundY != -1 && (Res.abs(me.cx - targetX) > 48 || Res.abs(me.cy - groundY) > 24))
				{
					ModWaypoint.TeleportTo(targetX, groundY);
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
