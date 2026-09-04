using System;

public static class ModTanSatTargeting
{
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
		int t = TileMap.tileTypeAtPixel(px, py - 12);
		return (t & (1 | 4 | 8 | 4096 | 8192 | 16384)) != 0;
	}

	public static void GetSafeAttackPosition(Mob target, bool isRanged, out int outX, out int outY)
	{
		if (target == null)
		{
			outX = 0;
			outY = 0;
			return;
		}

		// 1. Luôn sử dụng toạ độ thực tế thời gian thực của quái (thay vì toạ độ hồi sinh cũ)
		int mobX = target.x;
		int mobY = target.y;

		// 2. Khoảng cách tiếp cận tối ưu: 18px cho cận chiến, 45px cho chưởng xa
		int offset = isRanged ? 45 : 18;
		Char me = Char.myCharz();

		int preferredDir = (me != null && me.cx > mobX) ? 1 : -1;

		int x1 = mobX + preferredDir * offset;
		int y1 = mobY;

		int x2 = mobX - preferredDir * offset;
		int y2 = mobY;

		bool b1 = IsTileBlocked(x1, y1);
		bool b2 = IsTileBlocked(x2, y2);

		if (!b1)
		{
			outX = x1;
			outY = y1;
		}
		else if (!b2)
		{
			outX = x2;
			outY = y2;
		}
		else
		{
			outX = mobX;
			outY = mobY;
		}

		// 3. Ràng buộc toạ độ không vượt quá mép bản đồ
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
