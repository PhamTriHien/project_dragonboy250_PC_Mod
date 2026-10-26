using System;

/// <summary>
/// CLASS ModTanSatTargeting: Định nghĩa và điều phối logic chọn vị trí đứng đánh quái an toàn, chống kẹt địa hình và cổng chuyển map.
/// </summary>
public static class ModTanSatTargeting
{
	/// <summary>
	/// Kiểm tra tọa độ (px, py) có nằm trong phạm vi cổng chuyển map (Waypoint) hoặc đáy Air Map hay không.
	/// </summary>
	public static bool IsInsideWaypoint(int px, int py)
	{
		try
		{
			if (TileMap.isInAirMap() && TileMap.pxh > 0 && py >= TileMap.pxh - 52)
			{
				return true;
			}
			if (TileMap.vGo == null)
			{
				return false;
			}
			int count = TileMap.vGo.size();
			for (int i = 0; i < count; i++)
			{
				Waypoint wp = (Waypoint)TileMap.vGo.elementAt(i);
				if (wp == null)
				{
					continue;
				}
				int marginX = wp.isEnter ? 8 : 18;
				int marginY = 16;
				if (px >= wp.minX - marginX && px <= wp.maxX + marginX &&
				    py >= wp.minY - marginY && py <= wp.maxY + marginY)
				{
					return true;
				}
			}
		}
		catch
		{
		}
		return false;
	}

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái va chạm địa hình của IsTileBlocked.
	/// </summary>
	public static bool IsTileBlocked(int px, int py)
	{
		if (px < 24 || (TileMap.pxw > 0 && px > TileMap.pxw - 24))
		{
			return true;
		}
		if (py < 24 || (TileMap.pxh > 0 && py > TileMap.pxh - 24))
		{
			return true;
		}
		if (IsInsideWaypoint(px, py))
		{
			return true;
		}
		// Kiểm tra va chạm ở mức ngang hông (py - 12)
		int tWaist = TileMap.tileTypeAtPixel(px, py - 12);
		if ((tWaist & (1 | 4 | 8 | 4096 | 8192 | 16384)) != 0)
		{
			return true;
		}
		// Kiểm tra va chạm ở mức đầu (py - 22) để tránh bị kẹt vào trần/vách đá
		int tHead = TileMap.tileTypeAtPixel(px, py - 22);
		if ((tHead & (1 | 4 | 8 | 4096 | 8192 | 16384)) != 0)
		{
			return true;
		}
		return false;
	}

	/// <summary>
	/// Leo lên bề mặt đất trên cùng nếu tại cột X có nhiều lớp gạch mặt đất (type = 2) xếp chồng lên nhau,
	/// tránh việc đặt nhân vật chìm xuống lớp gạch ngầm bên dưới (ví dụ y = 360 nằm ngay dưới mặt đất y = 336) gây RESET_POINT.
	/// </summary>
	private static int ClimbToTopSurface(int px, int gy)
	{
		int guard = 0;
		while (gy >= 24 && guard < 64 && TileMap.tileTypeAt(px, gy - 24, 2))
		{
			gy -= 24;
			guard++;
		}
		return gy;
	}

	/// <summary>
	/// Tìm tọa độ mặt đất hợp lệ gần refY nhất tại cột X (tương tự ModWaypoint.GetGroundY của NextMap).
	/// Trả về -1 nếu không tìm thấy mặt đất hợp lệ trong phạm vi maxDeltaY.
	/// </summary>
	public static int FindGroundY(int px, int refY, int maxDeltaY = 96)
	{
		try
		{
			if (px < 24 || (TileMap.pxw > 0 && px > TileMap.pxw - 24))
			{
				return -1;
			}

			int baseY = TileMap.tileYofPixel(refY);
			if (TileMap.tileTypeAt(px, refY, 2))
			{
				int topY = ClimbToTopSurface(px, baseY);
				if (!IsTileBlocked(px, topY))
				{
					return topY;
				}
			}

			for (int delta = 12; delta <= maxDeltaY; delta += 12)
			{
				int upY = refY - delta;
				if (upY >= 24)
				{
					if (TileMap.tileTypeAt(px, upY, 2))
					{
						int gy = ClimbToTopSurface(px, TileMap.tileYofPixel(upY));
						if (!IsTileBlocked(px, gy))
						{
							return gy;
						}
					}
				}

				int downY = refY + delta;
				if (TileMap.pxh <= 0 || downY <= TileMap.pxh - 12)
				{
					if (TileMap.tileTypeAt(px, downY, 2))
					{
						int gy = ClimbToTopSurface(px, TileMap.tileYofPixel(downY));
						if (!IsTileBlocked(px, gy))
						{
							return gy;
						}
					}
				}
			}
		}
		catch
		{
		}
		return -1;
	}

	/// <summary>
	/// Tính toán tọa độ đứng đánh quái an toàn, ưu tiên cùng mặt đất với quái, không lọt vào tường hoặc Waypoint.
	/// </summary>
	public static void GetSafeAttackPosition(Mob target, bool isRanged, out int outX, out int outY)
	{
		if (target == null)
		{
			outX = 0;
			outY = 0;
			return;
		}

		int mobX = target.x;
		int mobY = target.y;

		bool isFlying = (Mob.arrMobTemplate != null &&
		                 target.templateId >= 0 &&
		                 target.templateId < Mob.arrMobTemplate.Length &&
		                 Mob.arrMobTemplate[target.templateId] != null &&
		                 (Mob.arrMobTemplate[target.templateId].type == 4 || Mob.arrMobTemplate[target.templateId].type == 5));

		Char me = Char.myCharz();
		int preferredDir;
		if (!isRanged)
		{
			if (target.xFirst > mobX + 4)
			{
				preferredDir = 1;
			}
			else if (target.xFirst < mobX - 4)
			{
				preferredDir = -1;
			}
			else
			{
				preferredDir = (target.dir != 0) ? ((target.dir > 0) ? 1 : -1) : ((me != null && me.cx > mobX) ? 1 : -1);
			}
		}
		else
		{
			preferredDir = (me != null && me.cx > mobX) ? 1 : -1;
		}

		int[] dirMultipliers = { preferredDir, -preferredDir };
		int[] distOffsets = isRanged ? new int[] { 36, 24, 16, 0 } : new int[] { 6, 0, 12, 18 };

		int bestX = mobX;
		int bestY = mobY;
		bool found = false;

		foreach (int dist in distOffsets)
		{
			foreach (int dir in dirMultipliers)
			{
				int candX = mobX + dir * dist;
				if (candX < 24 || (TileMap.pxw > 0 && candX > TileMap.pxw - 24))
				{
					continue;
				}

				if (!isFlying)
				{
					int groundY = FindGroundY(candX, mobY, 24);
					if (groundY != -1 && Res.abs(groundY - mobY) <= 24)
					{
						bestX = candX;
						bestY = groundY;
						found = true;
						break;
					}
				}
				else
				{
					int candY = mobY;
					int groundNear = FindGroundY(candX, mobY, 24);
					if (groundNear != -1 && Res.abs(groundNear - mobY) <= 24)
					{
						candY = groundNear;
					}
					if (!IsTileBlocked(candX, candY))
					{
						bestX = candX;
						bestY = candY;
						found = true;
						break;
					}
				}

				if (dist == 0)
				{
					break;
				}
			}
			if (found)
			{
				break;
			}
		}

		if (!found)
		{
			int fallbackGroundY = FindGroundY(mobX, mobY, 24);
			if (fallbackGroundY != -1 && Res.abs(fallbackGroundY - mobY) <= 24)
			{
				bestX = mobX;
				bestY = fallbackGroundY;
			}
			else if (IsInsideWaypoint(bestX, bestY) && TileMap.pxw > 160)
			{
				if (bestX < 80) bestX = 80;
				else if (bestX > TileMap.pxw - 80) bestX = TileMap.pxw - 80;
				int inwardGroundY = FindGroundY(bestX, mobY, 120);
				if (inwardGroundY != -1)
				{
					bestY = inwardGroundY;
				}
			}
		}

		outX = bestX;
		outY = bestY;

		if (TileMap.pxw > 0)
		{
			if (outX < 24) outX = 24;
			if (outX > TileMap.pxw - 24) outX = TileMap.pxw - 24;
		}
		if (TileMap.pxh > 0)
		{
			if (outY < 24) outY = 24;
			if (outY > TileMap.pxh - 24) outY = TileMap.pxh - 24;
		}
	}
}
