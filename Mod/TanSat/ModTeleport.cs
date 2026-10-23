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
	private static readonly List<TeleportEchoEntry> pendingTeleportEchoes = new List<TeleportEchoEntry>(16);

	/// <summary>
	/// Ghi nhận tọa độ vừa gửi gói (x, y+1) trong ModWaypoint.TeleportTo để nhận diện phản hồi xác nhận RESET_POINT(x, y) từ máy chủ.
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
		if (pendingTeleportEchoes.Count >= 16)
		{
			pendingTeleportEchoes.RemoveAt(0);
		}
		pendingTeleportEchoes.Add(new TeleportEchoEntry { x = x, y = y, time = now });
	}

	/// <summary>
	/// Kiểm tra và tiêu thụ gói phản hồi xác nhận tọa độ RESET_POINT(x, y) sinh ra bởi gói (x, y+1) của ModWaypoint.TeleportTo.
	/// </summary>
	public static bool TryConsumeTeleportEcho(int x, int y, int currentCx, int currentCy, out bool isCurrentPos)
	{
		long now = mSystem.currentTimeMillis();
		for (int i = pendingTeleportEchoes.Count - 1; i >= 0; i--)
		{
			if (now - pendingTeleportEchoes[i].time > 15000)
			{
				pendingTeleportEchoes.RemoveAt(i);
			}
		}

		for (int i = 0; i < pendingTeleportEchoes.Count; i++)
		{
			TeleportEchoEntry entry = pendingTeleportEchoes[i];
			if (Res.abs(entry.x - x) <= 24 && Res.abs(entry.y - y) <= 24)
			{
				pendingTeleportEchoes.RemoveRange(0, i + 1);
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
	/// Dịch chuyển nhân vật đến tọa độ đích sử dụng trực tiếp ModWaypoint.TeleportTo của NextMap.
	/// </summary>
	public static void TeleportTo(int targetX, int targetY)
	{
		ModWaypoint.TeleportTo(targetX, targetY);
	}

	/// <summary>
	/// Dịch chuyển tức thời 1 bước thẳng tới tọa độ đích giống hệt ModWaypoint.EnterWaypoint của NextMap:
	/// - Khi dịch chuyển xa hoặc khác tầng (dx > 64 || dy > 0): Gửi 3 gói đồng bộ tọa độ qua ModWaypoint.TeleportTo(targetX, targetY) nhảy thẳng tới mục tiêu trong 1 bước.
	/// - Khi chỉnh bước ngắn trên cùng mặt đất (dx <= 64 && dy == 0): Gửi 1 gói charMove chuẩn để không phát sinh gói RESET_POINT trễ.
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

			if (dx > 28 || dy > 24)
			{
				if (now - lastStepTime < 140)
				{
					return false;
				}
				me.cdir = (targetX >= me.cx) ? 1 : (-1);
				ModWaypoint.TeleportTo(targetX, targetY);
				lastStepTime = now;
				isWaitingSettle = true;
				return false;
			}

			return true;
		}
		catch
		{
			isWaitingSettle = false;
			return false;
		}
	}
}
