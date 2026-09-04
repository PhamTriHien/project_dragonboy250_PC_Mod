using System;
using Assets.src.e;
using Assets.src.g;
using UnityEngine;
public partial class GameCanvas : IActionListener
{
	public static void paintBGGameScr(mGraphics g)
		{
			if (!isLoadBGok)
			{
				g.setColor(0);
				g.fillRect(0, 0, w, h);
			}
			if (Char.isLoadingMap)
			{
				return;
			}
			int gW = GameScr.gW;
			int gH = GameScr.gH;
			g.translate(-g.getTranslateX(), -g.getTranslateY());
			if (ModMenu.graphicsQuality >= 2)
			{
				if (GameScr.gI() != null && GameScr.gI().isRongThanXuatHien)
				{
					g.setColor(0);
					g.fillRect(0, 0, w, h);
					return;
				}
				g.setColor(0xD4EDFF);
				g.fillRect(0, 0, w, h);
				return;
			}
			g.setColor(0);
			g.fillRect(0, 0, w, h);
			try
			{
				if (paintBG)
				{
					if (currentScreen == GameScr.gI())
					{
						if (TileMap.mapID != 172 && (TileMap.mapID == 137 || TileMap.mapID == 115 || TileMap.mapID == 117 || TileMap.mapID == 118 || TileMap.mapID == 120 || TileMap.isMapDouble))
						{
							g.setColor(0);
							g.fillRect(0, 0, w, h);
							return;
						}
						if (TileMap.mapID == 138)
						{
							g.setColor(6776679);
							g.fillRect(0, 0, w, h);
							return;
						}
					}
					if (typeBg == 0)
					{
						paintBackgroundtLayer(g, 4, 6, colorTop[3], colorBotton[3]);
						paintBackgroundtLayer(g, 3, 4, -1, colorBotton[2]);
						paintBackgroundtLayer(g, 2, 3, -1, colorBotton[1]);
						paintBackgroundtLayer(g, 1, 2, -1, colorBotton[0]);
					}
					else if (typeBg == 1)
					{
						paintBackgroundtLayer(g, 4, 6, -1, -1);
						paintBackgroundtLayer(g, 3, 3, -1, -1);
						fillRect(g, colorTop[2], 0, -(GameScr.cmy >> 5), gW, yb[2], 5);
						fillRect(g, colorBotton[2], 0, yb[2] + bgH[2] - (GameScr.cmy >> 3), gW, 70, 3);
						paintBackgroundtLayer(g, 2, 2, -1, -1);
						paintBackgroundtLayer(g, 1, 1, -1, colorBotton[0]);
					}
					else if (typeBg == 2)
					{
						paintBackgroundtLayer(g, 5, 10, colorTop[4], colorBotton[4]);
						paintBackgroundtLayer(g, 4, 8, -1, colorTop[2]);
						paintBackgroundtLayer(g, 3, 5, -1, colorBotton[2]);
						paintBackgroundtLayer(g, 2, 2, -1, colorBotton[1]);
						paintBackgroundtLayer(g, 1, 1, -1, colorBotton[0]);
						paintCloud(g);
					}
					else if (typeBg == 3)
					{
						int num = GameScr.cmy - (325 - GameScr.gH23);
						g.translate(0, -num);
						fillRect(g, (!GameScr.gI().isRongThanXuatHien && !GameScr.gI().isFireWorks) ? colorTop[2] : GameScr.gI().mautroi, 0, num - (GameScr.cmy >> 3), gW, yb[2] - num + (GameScr.cmy >> 3) + 100, 2);
						paintBackgroundtLayer(g, 3, 2, -1, colorBotton[2]);
						paintBackgroundtLayer(g, 2, 0, -1, -1);
						paintBackgroundtLayer(g, 1, 0, -1, colorBotton[0]);
						g.translate(0, -g.getTranslateY());
					}
					else if (typeBg == 4)
					{
						paintBackgroundtLayer(g, 4, 7, colorTop[3], -1);
						paintBackgroundtLayer(g, 3, 3, -1, (!isHDVersion()) ? colorTop[1] : colorBotton[2]);
						paintBackgroundtLayer(g, 2, 2, colorTop[1], colorBotton[1]);
						paintBackgroundtLayer(g, 1, 1, -1, colorBotton[0]);
					}
					else if (typeBg == 5)
					{
						paintBackgroundtLayer(g, 4, 15, colorTop[3], -1);
						drawSun1(g);
						g.translate(100, 10);
						drawSun1(g);
						g.translate(-100, -10);
						drawSun2(g);
						paintBackgroundtLayer(g, 3, 10, -1, -1);
						paintBackgroundtLayer(g, 2, 6, -1, -1);
						paintBackgroundtLayer(g, 1, 4, -1, -1);
						g.translate(0, 27);
						paintBackgroundtLayer(g, 1, 2, -1, -1);
						g.translate(0, 20);
						paintBackgroundtLayer(g, 1, 2, -1, colorBotton[0]);
						g.translate(-g.getTranslateX(), -g.getTranslateY());
					}
					else if (typeBg == 6)
					{
						paintBackgroundtLayer(g, 5, 10, colorTop[4], colorBotton[4]);
						drawSun1(g);
						drawSun2(g);
						g.translate(60, 40);
						drawSun2(g);
						g.translate(-60, -40);
						paintBackgroundtLayer(g, 4, 7, -1, colorBotton[3]);
						BackgroudEffect.paintFarAll(g);
						paintBackgroundtLayer(g, 3, 4, -1, -1);
						paintBackgroundtLayer(g, 2, 3, -1, colorBotton[1]);
						paintBackgroundtLayer(g, 1, 2, -1, colorBotton[0]);
					}
					else if (typeBg == 7)
					{
						paintBackgroundtLayer(g, 4, 6, colorTop[3], colorBotton[3]);
						paintBackgroundtLayer(g, 3, 5, -1, -1);
						paintBackgroundtLayer(g, 2, 4, -1, -1);
						paintBackgroundtLayer(g, 1, 3, -1, colorBotton[0]);
					}
					else if (typeBg == 8)
					{
						paintBackgroundtLayer(g, 4, 8, colorTop[3], colorBotton[3]);
						drawSun1(g);
						drawSun2(g);
						paintBackgroundtLayer(g, 3, 4, -1, colorBotton[2]);
						paintBackgroundtLayer(g, 2, 2, -1, colorBotton[1]);
						if (((TileMap.mapID < 92 || TileMap.mapID > 96) && TileMap.mapID != 51 && TileMap.mapID != 52) || currentScreen == loginScr)
						{
							paintBackgroundtLayer(g, 1, 1, -1, colorBotton[0]);
						}
					}
					else if (typeBg == 9)
					{
						paintBackgroundtLayer(g, 4, 8, colorTop[3], colorBotton[3]);
						drawSun1(g);
						drawSun2(g);
						g.translate(-80, 20);
						drawSun2(g);
						g.translate(80, -20);
						BackgroudEffect.paintFarAll(g);
						paintBackgroundtLayer(g, 3, 5, -1, -1);
						paintBackgroundtLayer(g, 2, 3, -1, -1);
						paintBackgroundtLayer(g, 1, 2, -1, colorBotton[0]);
					}
					else if (typeBg == 10)
					{
						int num2 = GameScr.cmy - (380 - GameScr.gH23);
						g.translate(0, -num2);
						fillRect(g, (!GameScr.gI().isRongThanXuatHien) ? colorTop[1] : GameScr.gI().mautroi, 0, num2 - (GameScr.cmy >> 2), gW, yb[1] - num2 + (GameScr.cmy >> 2) + 100, 2);
						paintBackgroundtLayer(g, 2, 2, -1, colorBotton[1]);
						drawSun1(g);
						drawSun2(g);
						paintBackgroundtLayer(g, 1, 0, -1, -1);
						g.translate(0, -g.getTranslateY());
					}
					else if (typeBg == 11)
					{
						paintBackgroundtLayer(g, 3, 6, colorTop[2], colorBotton[2]);
						drawSun1(g);
						paintBackgroundtLayer(g, 2, 3, -1, colorBotton[1]);
						paintBackgroundtLayer(g, 1, 2, -1, colorBotton[0]);
					}
					else if (typeBg == 12)
					{
						g.setColor(9161471);
						g.fillRect(0, 0, w, h);
						paintBackgroundtLayer(g, 3, 4, -1, 14417919);
						paintBackgroundtLayer(g, 2, 3, -1, 14417919);
						paintBackgroundtLayer(g, 1, 2, -1, 14417919);
						paintCloud(g);
					}
					else if (typeBg == 13)
					{
						g.setColor(15268088);
						g.fillRect(0, 0, w, h);
						paintBackgroundtLayer(g, 1, 5, -1, 15268088);
					}
					else if (typeBg == 15)
					{
						g.setColor(2631752);
						g.fillRect(0, 0, w, h);
						paintBackgroundtLayer(g, 2, 3, -1, colorBotton[1]);
						paintBackgroundtLayer(g, 1, 2, -1, colorBotton[0]);
					}
					else if (typeBg == 16)
					{
						paintBackgroundtLayer(g, 4, 6, colorTop[3], colorBotton[3]);
						for (int i = 0; i < imgSunSpec.Length; i++)
						{
							g.drawImage(imgSunSpec[i], cloudX[i], cloudY[i], 33);
						}
						paintBackgroundtLayer(g, 3, 4, -1, colorBotton[2]);
						paintBackgroundtLayer(g, 2, 3, -1, colorBotton[1]);
						paintBackgroundtLayer(g, 1, 2, -1, colorBotton[0]);
					}
					else if (typeBg == 19)
					{
						paintBackgroundtLayer(g, 5, 10, colorTop[4], colorBotton[4]);
						paintBackgroundtLayer(g, 4, 8, -1, colorTop[2]);
						paintBackgroundtLayer(g, 3, 5, -1, colorBotton[2]);
						paintBackgroundtLayer(g, 2, 2, -1, colorBotton[1]);
						paintBackgroundtLayer(g, 1, 1, -1, colorBotton[0]);
						paintCloud(g);
					}
					else
					{
						fillRect(g, colorBotton[3], 0, yb[3] + bgH[3], GameScr.gW, yb[2] + bgH[2], 6);
						paintBackgroundtLayer(g, 4, 6, colorTop[3], colorBotton[3]);
						drawSun1(g);
						paintBackgroundtLayer(g, 3, 4, -1, colorBotton[2]);
						paintBackgroundtLayer(g, 2, 3, -1, colorBotton[1]);
						paintBackgroundtLayer(g, 1, 2, -1, colorBotton[0]);
					}
					return;
				}
				g.setColor(2315859);
				g.fillRect(0, 0, w, h);
				if (tam != null)
				{
					for (int j = -((GameScr.cmx >> 2) % mGraphics.getImageWidth(tam)); j < GameScr.gW; j += mGraphics.getImageWidth(tam))
					{
						g.drawImage(tam, j, (GameScr.cmy >> 3) + h / 2 - 50, 0);
					}
				}
				g.setColor(5084791);
				g.fillRect(0, (GameScr.cmy >> 3) + h / 2 - 50 + mGraphics.getImageHeight(tam), gW, h);
			}
			catch (Exception)
			{
				g.setColor(0);
				g.fillRect(0, 0, w, h);
			}
		}
	public static void resetBg()
		{
		}

}
