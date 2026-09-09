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
		// Kiem tra va cham o muc ngang hong (py - 12)
		int tWaist = TileMap.tileTypeAtPixel(px, py - 12);
		if ((tWaist & (1 | 4 | 8 | 4096 | 8192 | 16384)) != 0)
		{
			return true;
		}
		// Kiem tra va cham o muc dau (py - 22) de tranh bi ket vao tran/vach da
		int tHead = TileMap.tileTypeAtPixel(px, py - 22);
		if ((tHead & (1 | 4 | 8 | 4096 | 8192 | 16384)) != 0)
		{
			return true;
		}
		return false;
	}

	public static void GetSafeAttackPosition(Mob target, bool isRanged, out int outX, out int outY)
	{
		if (target == null)
		{
			outX = 0;
			outY = 0;
			return;
		}

		// 1. Luon su dung toa do thuc te thoi gian thuc cua quai
		int mobX = target.x;
		int mobY = target.y;

		// Xac dinh quai bay hay quai di bo tren dat theo dac ta chuan cua game (type == 4 hoac 5 la quai bay)
		bool isFlying = (Mob.arrMobTemplate != null && target.templateId >= 0 && target.templateId < Mob.arrMobTemplate.Length && Mob.arrMobTemplate[target.templateId] != null && (Mob.arrMobTemplate[target.templateId].type == 4 || Mob.arrMobTemplate[target.templateId].type == 5));

		// 2. Khoang cach tiep can toi uu: 24px cho can chien (> 20px tranh repel, < 40px trong hitbox), 45px cho chuong xa
		int offset = isRanged ? 45 : 24;
		Char me = Char.myCharz();
		int preferredDir = (me != null && me.cx > mobX) ? 1 : -1;

		int[] dirMultipliers = { preferredDir, -preferredDir };
		int[] distOffsets = isRanged ? new int[] { 45, 35, 25 } : new int[] { 24, 20, 16, 12 };

		int bestX = mobX + preferredDir * offset;
		int bestY = mobY;
		bool found = false;

		foreach (int dist in distOffsets)
		{
			foreach (int dir in dirMultipliers)
			{
				int candX = mobX + dir * dist;
				int candY = mobY;

				if (!isFlying)
				{
					// Quai tren dat: Quet tim be mat dat chuan ((tileTypeAtPixel & 2) == 2) uu tien ngang tam quai
					int baseTileY = TileMap.tileYofPixel(mobY);
					int foundGroundY = -1;

					// Quet tim be mat dat uu tien tu vi tri gan mobY nhat ra xa dan
					int[] yDeltas = { 0, 24, -24, 48, -48, 72 };
					for (int yd = 0; yd < yDeltas.Length; yd++)
					{
						int testY = baseTileY + yDeltas[yd];
						if ((TileMap.tileTypeAtPixel(candX, testY) & 2) == 2)
						{
							foundGroundY = testY;
							break;
						}
					}

					if (foundGroundY != -1)
					{
						candY = foundGroundY;
					}
					else
					{
						candY = baseTileY;
					}
				}

				if (!IsTileBlocked(candX, candY))
				{
					bestX = candX;
					bestY = candY;
					found = true;
					break;
				}
			}
			if (found) break;
		}

		outX = bestX;
		outY = bestY;

		// 4. Rang buoc toa do khong vuot qua mep ban do
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
