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
	private static readonly List<TeleportEchoEntry> pendingTeleportEchoes = new List<TeleportEchoEntry>(64);

	/// <summary>
	/// Ghi nhận tọa độ vừa gửi gói dịch chuyển để nhận diện phản hồi xác nhận RESET_POINT(x, y) từ máy chủ.
	/// </summary>
	public static void RegisterTeleportEcho(int x, int y)
	{
		long now = mSystem.currentTimeMillis();
		for (int i = pendingTeleportEchoes.Count - 1; i >= 0; i--)
		{
			if (now - pendingTeleportEchoes[i].time > 60000)
			{
				pendingTeleportEchoes.RemoveAt(i);
			}
		}
		if (pendingTeleportEchoes.Count >= 64)
		{
			pendingTeleportEchoes.RemoveAt(0);
		}
		pendingTeleportEchoes.Add(new TeleportEchoEntry { x = x, y = y, time = now });
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
			if (now - entry.time > 60000)
			{
				continue;
			}
			if (Res.abs(entry.x - x) <= 24 && (Res.abs(entry.y - y) <= 24 || (y > entry.y && TileMap.tileTypeAt(x, y, 2))))
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Kiểm tra và tiêu thụ gói phản hồi xác nhận tọa độ RESET_POINT(x, y) sinh ra bởi dịch chuyển trước đó.
	/// </summary>
	public static bool TryConsumeTeleportEcho(int x, int y, int currentCx, int currentCy, out bool isCurrentPos)
	{
		long now = mSystem.currentTimeMillis();
		for (int i = pendingTeleportEchoes.Count - 1; i >= 0; i--)
		{
			if (now - pendingTeleportEchoes[i].time > 60000)
			{
				pendingTeleportEchoes.RemoveAt(i);
			}
		}

		for (int i = 0; i < pendingTeleportEchoes.Count; i++)
		{
			TeleportEchoEntry entry = pendingTeleportEchoes[i];
			if (Res.abs(entry.x - x) <= 24 && (Res.abs(entry.y - y) <= 24 || (y > entry.y && TileMap.tileTypeAt(x, y, 2))))
			{
				pendingTeleportEchoes.RemoveAt(i);
				isCurrentPos = (Res.abs(x - currentCx) <= 24 && Res.abs(y - currentCy) <= 24);
				return true;
			}
		}

		isCurrentPos = (Res.abs(x - currentCx) <= 24 && Res.abs(y - currentCy) <= 24);
		return false;
	}

	public static void ClearEchoes()
	{
		pendingTeleportEchoes.Clear();
		isWaitingSettle = false;
	}

	private static bool IsPointUndergroundOrBlocked(int px, int py, int refTopY)
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
		int startY = TileMap.tileYofPixel(System.Math.Max(24, refTopY));
		int endY = TileMap.tileYofPixel(py);
		for (int y = startY; y < endY; y += 24)
		{
			int t = TileMap.tileTypeAtPixel(px, y);
			if ((t & (1 | 4 | 8 | 4096 | 8192 | 16384)) != 0)
			{
				return true;
			}
			if ((t & 2) != 0 && !TileMap.tileTypeAt(px, py, 2))
			{
				return true;
			}
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
	/// Điều phối di chuyển Tàn Sát và Auto Nhặt theo từng bước đơn (1 gói charMove / nhịp >= 120ms, tối đa 96px ngang, 84px dọc)
	/// kết hợp quỹ đạo vòng qua vách đá (Canopy Path) và không bao giờ gửi (x, y+1), triệt tiêu 100% hàng đợi RESET_POINT từ máy chủ.
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

			if (now - lastStepTime < 120)
			{
				return false;
			}

			me.cdir = (targetX >= me.cx) ? 1 : (-1);

			if (dx <= 96 && dy <= 48)
			{
				SendSingleCharMove(targetX, targetY);
				lastStepTime = now;
				isWaitingSettle = false;
				return true;
			}

			int candX = (dx <= 96) ? targetX : (me.cx + ((targetX > me.cx) ? 96 : (-96)));
			int nextX = candX;
			int nextY = targetY;

			if (targetY < me.cy)
			{
				// Di chuyển lên nền cao / quái bay: Ưu tiên nâng độ cao trước nếu tiến ngang sẽ đâm vào vách núi
				int candY = (me.cy - targetY <= 84) ? targetY : (me.cy - 84);
				if (candY > targetY && IsPointUndergroundOrBlocked(candX, candY, targetY))
				{
					nextX = me.cx;
				}
				nextY = candY;
			}
			else if (targetY > me.cy)
			{
				// Di chuyển từ nền cao xuống nền thấp: Tiến ngang trên không vượt qua mép vách đá trước khi hạ độ cao
				int candY = (targetY - me.cy <= 84) ? targetY : (me.cy + 84);
				if (dx > 64 || IsPointUndergroundOrBlocked(candX, candY, me.cy))
				{
					nextY = me.cy;
					int highGroundY = ModTanSatTargeting.FindGroundY(nextX, me.cy, 48);
					if (highGroundY != -1 && highGroundY < nextY)
					{
						nextY = highGroundY;
					}
				}
				else
				{
					nextY = candY;
				}
			}
			else
			{
				nextY = targetY;
				if (TileMap.tileTypeAt(targetX, targetY, 2))
				{
					int groundY = ModTanSatTargeting.FindGroundY(nextX, nextY, 48);
					if (groundY != -1 && groundY < nextY)
					{
						nextY = groundY;
					}
				}
			}

			SendSingleCharMove(nextX, nextY);
			lastStepTime = now;

			if (Res.abs(nextX - targetX) <= 12 && Res.abs(nextY - targetY) <= 16)
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
