using System;

public partial class TileMap
{
	public static void paintTile(mGraphics g, int frame, int indexX, int indexY)
		{
			if (imgTile != null)
			{
				if (imgTile.Length == 1)
				{
					g.drawRegion(imgTile[0], 0, frame * size, size, size, 0, indexX * size, indexY * size, 0);
				}
				else
				{
					g.drawImage(imgTile[frame], indexX * size, indexY * size, 0);
				}
			}
		}

	public static void paintTile(mGraphics g, int frame, int x, int y, int w, int h)
		{
			if (imgTile != null)
			{
				if (imgTile.Length == 1)
				{
					g.drawRegion(imgTile[0], 0, frame * w, w, w, 0, x, y, 0);
				}
				else
				{
					g.drawImage(imgTile[frame], x, y, 0);
				}
			}
		}

	public static void paintTilemapLOW(mGraphics g)
		{
			for (int i = GameScr.gssx; i < GameScr.gssxe; i++)
			{
				for (int j = GameScr.gssy; j < GameScr.gssye; j++)
				{
					int num = maps[j * tmw + i] - 1;
					if (num != -1)
					{
						paintTile(g, num, i, j);
					}
					if ((tileTypeAt(i, j) & 0x20) == 32)
					{
						g.drawRegion(imgWaterfall, 0, 24 * (GameCanvas.gameTick % 4), 24, 24, 0, i * size, j * size, 0);
					}
					else if ((tileTypeAt(i, j) & 0x40) == 64)
					{
						if ((tileTypeAt(i, j - 1) & 0x20) == 32)
						{
							g.drawRegion(imgWaterfall, 0, 24 * (GameCanvas.gameTick % 4), 24, 24, 0, i * size, j * size, 0);
						}
						else if ((tileTypeAt(i, j - 1) & 0x1000) == 4096)
						{
							paintTile(g, 21, i, j);
						}
						Image image = null;
						image = ((tileID == 5) ? imgWaterlowN : ((tileID != 8) ? imgWaterflow : imgWaterlowN2));
						g.drawRegion(image, 0, (GameCanvas.gameTick % 8 >> 2) * 24, 24, 24, 0, i * size, j * size, 0);
					}
					if ((tileTypeAt(i, j) & 0x800) == 2048)
					{
						if ((tileTypeAt(i, j - 1) & 0x20) == 32)
						{
							g.drawRegion(imgWaterfall, 0, 24 * (GameCanvas.gameTick % 4), 24, 24, 0, i * size, j * size, 0);
						}
						else if ((tileTypeAt(i, j - 1) & 0x1000) == 4096)
						{
							paintTile(g, 21, i, j);
						}
						paintTile(g, maps[j * tmw + i] - 1, i, j);
					}
				}
			}
		}

	public static void paintTilemapSuperLow(mGraphics g)
		{
			if (Char.isLoadingMap || maps == null)
			{
				return;
			}
			try
			{
				for (int j = GameScr.gssx; j < GameScr.gssxe; j++)
				{
					for (int k = GameScr.gssy; k < GameScr.gssye; k++)
					{
						if (j <= 0 || j >= tmw - 1 || k < 0 || k >= tmh)
						{
							continue;
						}
						int idx = k * tmw + j;
						if (idx >= 0 && idx < maps.Length)
						{
							int num = maps[idx] - 1;
							if (num != -1)
							{
								paintTile(g, num, j, k);
							}
						}
					}
				}
			}
			catch
			{
			}
		}

	public static void paintTilemap(mGraphics g)
		{
			if (Char.isLoadingMap || maps == null)
			{
				return;
			}
			if (ModMenu.graphicsQuality == 3)
			{
				paintTilemapSuperLow(g);
				return;
			}
			GameScr.gI().paintBgItem(g, 1);
			for (int i = 0; i < GameScr.vItemMap.size(); i++)
			{
				((ItemMap)GameScr.vItemMap.elementAt(i)).paintAuraItemEff(g);
			}
			for (int j = GameScr.gssx; j < GameScr.gssxe; j++)
			{
				for (int k = GameScr.gssy; k < GameScr.gssye; k++)
				{
					if (j == 0 || j == tmw - 1)
					{
						continue;
					}
					int num = maps[k * tmw + j] - 1;
					if ((tileTypeAt(j, k) & 0x100) == 256)
					{
						continue;
					}
					if ((tileTypeAt(j, k) & 0x20) == 32)
					{
						g.drawRegion(imgWaterfall, 0, 24 * (GameCanvas.gameTick % 8 >> 1), 24, 24, 0, j * size, k * size, 0);
					}
					else if ((tileTypeAt(j, k) & 0x80) == 128)
					{
						g.drawRegion(imgTopWaterfall, 0, 24 * (GameCanvas.gameTick % 8 >> 1), 24, 24, 0, j * size, k * size, 0);
					}
					else
					{
						if (tileID == 13 && num != -1)
						{
							continue;
						}
						if (tileID == 2 && (tileTypeAt(j, k) & 0x200) == 512 && num != -1)
						{
							paintTile(g, num, j * size, k * size, 24, 1);
							paintTile(g, num, j * size, k * size + 1, 24, 24);
						}
						if (tileID == 3)
						{
						}
						if ((tileTypeAt(j, k) & 0x10) == 16)
						{
							bx = j * size - GameScr.cmx;
							dbx = bx - GameScr.gW2;
							dfx = (size - 2) * dbx / size;
							fx = dfx + GameScr.gW2;
							paintTile(g, num, fx + GameScr.cmx, k * size, 24, 24);
						}
						else if ((tileTypeAt(j, k) & 0x200) == 512)
						{
							if (num != -1)
							{
								paintTile(g, num, j * size, k * size, 24, 1);
								paintTile(g, num, j * size, k * size + 1, 24, 24);
							}
						}
						else if (num != -1)
						{
							paintTile(g, num, j, k);
						}
					}
				}
			}
			if (GameScr.cmx < 24)
			{
				for (int l = GameScr.gssy; l < GameScr.gssye; l++)
				{
					int num2 = maps[l * tmw + 1] - 1;
					if (num2 != -1)
					{
						paintTile(g, num2, 0, l);
					}
				}
			}
			if (GameScr.cmx <= GameScr.cmxLim)
			{
				return;
			}
			int num3 = tmw - 2;
			for (int m = GameScr.gssy; m < GameScr.gssye; m++)
			{
				int num4 = maps[m * tmw + num3] - 1;
				if (num4 != -1)
				{
					paintTile(g, num4, num3 + 1, m);
				}
			}
		}

	public static void paintOutTilemap(mGraphics g)
		{
			if (GameCanvas.lowGraphic || ModMenu.graphicsQuality == 3)
			{
				return;
			}
			int num = 0;
			for (int i = GameScr.gssx; i < GameScr.gssxe; i++)
			{
				for (int j = GameScr.gssy; j < GameScr.gssye; j++)
				{
					num++;
					if ((tileTypeAt(i, j) & 0x40) != 64)
					{
						continue;
					}
					Image image = null;
					image = ((tileID == 5) ? imgWaterlowN : ((tileID != 8) ? imgWaterflow : imgWaterlowN2));
					if (!isWaterEff())
					{
						g.drawRegion(image, 0, 0, 24, 24, 0, i * size, j * size - 1, 0);
						g.drawRegion(image, 0, 0, 24, 24, 0, i * size, j * size - 3, 0);
					}
					g.drawRegion(image, 0, (GameCanvas.gameTick % 8 >> 2) * 24, 24, 24, 0, i * size, j * size - 12, 0);
					if (yWater == 0 && isWaterEff())
					{
						yWater = j * size - 12;
						int color = 16777215;
						if (GameCanvas.typeBg == 2)
						{
							color = 10871287;
						}
						else if (GameCanvas.typeBg == 4)
						{
							color = 8111470;
						}
						else if (GameCanvas.typeBg == 7)
						{
							color = 5693125;
						}
						else if (GameCanvas.typeBg == 19)
						{
							color = 16711680;
						}
						BackgroudEffect.addWater(color, yWater + 15);
					}
				}
			}
			BackgroudEffect.paintWaterAll(g);
		}

}
