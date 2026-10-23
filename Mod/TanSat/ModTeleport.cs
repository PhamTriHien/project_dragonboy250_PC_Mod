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

	/// <summary>
	/// Gửi chuỗi gói charMove hợp lệ (|dx| <= 80, |dy| <= 44 mỗi bước) từ (fromX, fromY) tới (toX, toY)
	/// mà không gửi gói sai lệch (x, y+1), giúp máy chủ chấp nhận tọa độ tức thì và không sinh RESET_POINT trễ.
	/// </summary>
	public static void StepMoveFrom(int fromX, int fromY, int toX, int toY)
	{
		Char me = Char.myCharz();
		if (me == null)
		{
			return;
		}
		int totalDx = Res.abs(toX - fromX);
		int totalDy = Res.abs(toY - fromY);
		int stepsX = (totalDx + 79) / 80;
		int stepsY = (totalDy + 43) / 44;
		int steps = System.Math.Max(1, System.Math.Max(stepsX, stepsY));
		if (steps > 6)
		{
			steps = 6;
		}

		int prevX = fromX;
		int prevY = fromY;
		for (int i = 1; i <= steps; i++)
		{
			int curX = (i == steps) ? toX : (fromX + (toX - fromX) * i / steps);
			int curY = (i == steps) ? toY : (fromY + (toY - fromY) * i / steps);
			me.cxSend = (curX != prevX) ? prevX : (curX - 1);
			me.cySend = (curY != prevY) ? prevY : (curY - 1);
			me.cx = curX;
			me.cy = curY;
			Service.gI().charMove();
			prevX = curX;
			prevY = curY;
		}
		me.cx = toX;
		me.cy = toY;
		me.cxSend = toX;
		me.cySend = toY;
		me.cvx = 0;
		me.cvy = 0;
		me.statusMe = 1;
		me.delayFall = 0;
	}

	/// <summary>
	/// Dịch chuyển nhân vật đến tọa độ đích sử dụng trực tiếp ModWaypoint.TeleportTo của NextMap.
	/// </summary>
	public static void TeleportTo(int targetX, int targetY)
	{
		ModWaypoint.TeleportTo(targetX, targetY);
	}

	/// <summary>
	/// Điều phối dịch chuyển thông minh theo địa hình và khoảng cách:
	/// 1. Bước ngắn (dx <= 96 && dy <= 48): Gửi gói charMove hợp lệ trực tiếp, không gửi (x, y+1), triệt tiêu hoàn toàn hàng đợi RESET_POINT trễ.
	/// 2. Mục tiêu trên không (!tileTypeAt(target, 2) hoặc quái bay): Di chuyển theo nhịp charMove trên không (tối đa 140px ngang, 132px dọc mỗi nhịp) không gửi (x, y+1) để không bị máy chủ kéo rơi xuống đất (y=336).
	/// 3. Dịch chuyển xa trên mặt đất (dx > 96 || dy > 48): Sử dụng ModWaypoint.TeleportTo nhảy thẳng tới đích trong 1 bước.
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

			if (isWaitingSettle)
			{
				if (now - lastStepTime < 140)
				{
					return false;
				}
				isWaitingSettle = false;
			}

			if (dx <= 12 && dy <= 16)
			{
				return true;
			}

			if (now - lastStepTime < 140)
			{
				return false;
			}

			me.cdir = (targetX >= me.cx) ? 1 : (-1);

			if (dx <= 96 && dy <= 48)
			{
				StepMoveFrom(me.cx, me.cy, targetX, targetY);
				lastStepTime = now;
				isWaitingSettle = false;
				return true;
			}

			bool isTargetOnGround = TileMap.tileTypeAt(targetX, targetY, 2);
			if (!isTargetOnGround || isTargetFlying)
			{
				int nextX = (dx <= 140) ? targetX : (me.cx + ((targetX > me.cx) ? 140 : (-140)));
				int nextY = (dy <= 132) ? targetY : (me.cy + ((targetY > me.cy) ? 132 : (-132)));
				StepMoveFrom(me.cx, me.cy, nextX, nextY);
				RegisterTeleportEcho(nextX, nextY);
				lastStepTime = now;
				if (nextX == targetX && nextY == targetY)
				{
					isWaitingSettle = false;
					return true;
				}
				isWaitingSettle = true;
				return false;
			}

			ModWaypoint.TeleportTo(targetX, targetY);
			lastStepTime = now;
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
