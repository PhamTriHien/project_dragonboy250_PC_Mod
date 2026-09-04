using System;
using Assets.src.e;

public partial class BackgroudEffect
{
	public static void paintCloud2(mGraphics g)
		{
			if (mSystem.clientType == 1 || GameCanvas.lowGraphic || ModMenu.graphicsQuality >= 1 || nCloud == 0 || imgCloud1 == null)
			{
				return;
			}
			for (int i = 0; i < nCloud; i++)
			{
				int num = i;
				if (num > 3)
				{
					num = 3;
				}
				if (num == 0)
				{
					num = 1;
				}
				g.drawImage(imgCloud1, GameCanvas.cloudX[i], GameCanvas.cloudY[i], 3);
			}
		}

	public static void paintFog(mGraphics g)
		{
			if (mSystem.clientType == 1 || GameCanvas.lowGraphic || ModMenu.graphicsQuality >= 1 || !isFog || imgFog == null)
			{
				return;
			}
			for (int i = xfog; i < TileMap.pxw; i += fogw)
			{
				if (i >= GameScr.cmx - fogw)
				{
					g.drawImageFog(imgFog, i, yfog, 0);
				}
			}
		}

	public void paintWater(mGraphics g)
		{
			if (typeEff != 10)
			{
				return;
			}
			g.setColor(colorWater);
			for (int i = 0; i < num; i++)
			{
				g.drawImage((i >= num / 2) ? water1 : water2, x[i], y[i] + yWater, 0);
			}
			if (id_water1 != 0 && water3 == null)
			{
				water3 = SmallImage.imgNew[id_water1].img;
			}
			if (water3 != null)
			{
				for (int j = 0; j < num / 2; j++)
				{
					g.drawImage(water3, x[j], y[j] + yWater, 0);
				}
			}
		}

	public void paintFar(mGraphics g)
		{
			g.translate(-g.getTranslateX(), -g.getTranslateY());
			if (typeEff == 4)
			{
				for (int i = 0; i < sum; i++)
				{
					g.drawRegion(imgSao, 0, 16 * frame[i], 16, 16, 0, x[i], y[i], 0);
				}
			}
			if (typeEff == 9)
			{
				g.setColor(16777215);
				for (int j = 0; j < num; j++)
				{
					g.drawImage((wP[j] != 1) ? imgChamTron2 : imgChamTron1, x[j], y[j], 3);
				}
			}
		}

	public void paintFront(mGraphics g)
		{
			try
			{
				switch (typeEff)
				{
				case 3:
					break;
				case 0:
				case 12:
				{
					int cmx = GameScr.cmx;
					int cmy = GameScr.cmy;
					for (int i = 0; i < sum; i++)
					{
						if (type[i] == 2 && x[i] >= GameScr.cmx && x[i] <= GameCanvas.w + GameScr.cmx && y[i] >= GameScr.cmy && y[i] <= GameCanvas.h + GameScr.cmy)
						{
							if (activeEff[i])
							{
								g.drawRegion(imgHatMua, 0, 10 * frame[i], 13, 10, 0, x[i], y[i] - 10, 0);
							}
							else
							{
								g.drawImage(imgMua1, x[i], y[i], 0);
							}
						}
					}
					break;
				}
				case 1:
				case 2:
				case 5:
				case 6:
				case 7:
				case 11:
				case 15:
					if (typeEff == 15)
					{
						if (SmallImage.imgNew[11120] != null && SmallImage.imgNew[11120].img != null)
						{
							imgLacay = SmallImage.imgNew[11120].img;
						}
						if (imgLacay == null)
						{
							break;
						}
					}
					paintLacay1(g, imgLacay);
					break;
				case 13:
					if (!isPaintFar)
					{
						paintCloud2(g);
					}
					break;
				case 4:
				case 8:
				case 9:
				case 10:
				case 14:
					break;
				}
			}
			catch (Exception)
			{
			}
		}

	public void paintLacay1(mGraphics g, Image img)
		{
			int num = ((typeEff != 11) ? 4 : 3);
			num = ((typeEff != 15) ? 4 : 4);
			if (typeEff == 11)
			{
				PIXEL = 5;
			}
			for (int i = 0; i < sum; i++)
			{
				if (i % 3 == 0 && x[i] >= GameScr.cmx && x[i] <= GameCanvas.w + GameScr.cmx && y[i] >= GameScr.cmy && y[i] <= GameCanvas.h + GameScr.cmy && img != null)
				{
					g.drawRegion(img, 0, PIXEL * frame[i], img.getWidth(), PIXEL, 0, x[i], y[i], 0);
				}
			}
		}

	public void paintLacay2(mGraphics g, Image img)
		{
			int num = ((typeEff != 11) ? 4 : 3);
			num = ((typeEff != 15) ? 4 : 4);
			if (typeEff == 11)
			{
				PIXEL = 5;
			}
			for (int i = 0; i < sum; i++)
			{
				if (i % 3 != 0 && x[i] >= GameScr.cmx && x[i] <= GameCanvas.w + GameScr.cmx && y[i] >= GameScr.cmy && y[i] <= GameCanvas.h + GameScr.cmy && img != null)
				{
					g.drawRegion(img, 0, PIXEL * frame[i], img.getWidth(), PIXEL, 0, x[i], y[i], 0);
				}
			}
		}

	public void paintBehindTile(mGraphics g)
		{
			switch (typeEff)
			{
			case 8:
				g.drawRegion(imgShip, 0, 0, imgShip.getWidth(), imgShip.getHeight(), trans, xShip, yShip, 3);
				if (way == 1 || way == 2)
				{
					int num = ((trans != 0) ? 25 : (-25));
					g.drawRegion(imgFire1, 0, frameFire * 8, 20, 8, trans, xShip + num, yShip + 5, 3);
				}
				else
				{
					int num2 = ((trans != 0) ? (-11) : 11);
					g.drawRegion(imgFire2, 0, frameFire * 18, 8, 18, trans, xShip + num2, yShip + 22, 3);
				}
				break;
			case 13:
				if (isPaintFar)
				{
					paintCloud2(g);
				}
				break;
			}
		}

	public void paintBack(mGraphics g)
		{
			switch (typeEff)
			{
			case 3:
				break;
			case 0:
			{
				int cmx = GameScr.cmx;
				int cmy = GameScr.cmy;
				g.setColor(10742731);
				for (int i = 0; i < sum; i++)
				{
					if (type[i] != 2 && x[i] >= GameScr.cmx && x[i] <= GameCanvas.w + GameScr.cmx && y[i] >= GameScr.cmy && y[i] <= GameCanvas.h + GameScr.cmy)
					{
						g.drawImage(imgMua2, x[i], y[i], 0);
					}
				}
				break;
			}
			case 1:
			case 2:
			case 5:
			case 6:
			case 7:
			case 11:
			case 15:
				if (typeEff == 15)
				{
					if (SmallImage.imgNew[11120] != null && SmallImage.imgNew[11120].img != null)
					{
						imgLacay = SmallImage.imgNew[11120].img;
					}
					if (imgLacay == null)
					{
						break;
					}
				}
				paintLacay2(g, imgLacay);
				break;
			case 4:
			case 8:
			case 9:
			case 10:
			case 12:
			case 13:
			case 14:
				break;
			}
		}

	public static void paintWaterAll(mGraphics g)
		{
			if (ModMenu.graphicsQuality >= 1) return;
			for (int i = 0; i < vBgEffect.size(); i++)
			{
				((BackgroudEffect)vBgEffect.elementAt(i)).paintWater(g);
			}
		}

	public static void paintBehindTileAll(mGraphics g)
		{
			if (ModMenu.graphicsQuality >= 1) return;
			for (int i = 0; i < vBgEffect.size(); i++)
			{
				((BackgroudEffect)vBgEffect.elementAt(i)).paintBehindTile(g);
			}
		}

	public static void paintFrontAll(mGraphics g)
		{
			if (ModMenu.graphicsQuality >= 1) return;
			for (int i = 0; i < vBgEffect.size(); i++)
			{
				((BackgroudEffect)vBgEffect.elementAt(i)).paintFront(g);
			}
		}

	public static void paintFarAll(mGraphics g)
		{
			if (ModMenu.graphicsQuality >= 1) return;
			for (int i = 0; i < vBgEffect.size(); i++)
			{
				((BackgroudEffect)vBgEffect.elementAt(i)).paintFar(g);
			}
		}

	public static void paintBackAll(mGraphics g)
		{
			if (ModMenu.graphicsQuality >= 1) return;
			for (int i = 0; i < vBgEffect.size(); i++)
			{
				((BackgroudEffect)vBgEffect.elementAt(i)).paintBack(g);
			}
		}

}
