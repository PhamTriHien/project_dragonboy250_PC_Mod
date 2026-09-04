using System;
using Assets.src.e;
using Assets.src.g;
using UnityEngine;

public partial class GameCanvas : IActionListener
{
	public static void paintCloud(mGraphics g)
		{
		}

	public static void updateBG()
		{
		}

	public static void paintBackgroundtLayer(mGraphics g, int layer, int deltaY, int color1, int color2)
		{
			try
			{
				int num = layer - 1;
				if (num == imgBG.Length - 1 && (GameScr.gI().isRongThanXuatHien || GameScr.gI().isFireWorks))
				{
					g.setColor(GameScr.gI().mautroi);
					g.fillRect(0, 0, w, h);
					if (typeBg == 2 || typeBg == 4 || typeBg == 7)
					{
						drawSun1(g);
						drawSun2(g);
					}
					if (GameScr.gI().isFireWorks && !lowGraphic)
					{
						FireWorkEff.paint(g);
					}
				}
				else
				{
					if (imgBG == null || imgBG[num] == null)
					{
						return;
					}
					if (moveX[num] != 0)
					{
						moveX[num] += moveXSpeed[num];
					}
					int cmy = GameScr.cmy;
					if (cmy > h)
					{
						cmy = h;
					}
					if (layerSpeed[num] != 0)
					{
						for (int i = -((GameScr.cmx + moveX[num] >> layerSpeed[num]) % bgW[num]); i < GameScr.gW; i += bgW[num])
						{
							g.drawImage(imgBG[num], i, yb[num] - ((deltaY > 0) ? (cmy >> deltaY) : 0), 0);
						}
					}
					else
					{
						for (int j = 0; j < GameScr.gW; j += bgW[num])
						{
							g.drawImage(imgBG[num], j, yb[num] - ((deltaY > 0) ? (cmy >> deltaY) : 0), 0);
						}
					}
					if (color1 != -1)
					{
						if (num == nBg - 1)
						{
							fillRect(g, color1, 0, -(cmy >> deltaY), GameScr.gW, yb[num], deltaY);
						}
						else
						{
							fillRect(g, color1, 0, yb[num - 1] + bgH[num - 1], GameScr.gW, yb[num] - (yb[num - 1] + bgH[num - 1]), deltaY);
						}
					}
					if (color2 != -1)
					{
						if (num == 0)
						{
							fillRect(g, color2, 0, yb[num] + bgH[num], GameScr.gW, GameScr.gH - (yb[num] + bgH[num]), deltaY);
						}
						else
						{
							fillRect(g, color2, 0, yb[num] + bgH[num], GameScr.gW, yb[num - 1] - (yb[num] + bgH[num]) + 80, deltaY);
						}
					}
					if (currentScreen == GameScr.instance)
					{
						if (layer == 1 && typeBg == 11)
						{
							g.drawImage(imgSun2, -(GameScr.cmx >> layerSpeed[0]) + 400, yb[0] + 30 - (cmy >> 2), StaticObj.BOTTOM_HCENTER);
						}
						if (layer == 1 && typeBg == 13)
						{
							g.drawImage(imgBG[1], -(GameScr.cmx >> layerSpeed[0]) + TileMap.tmw * 24 / 4, yb[0] - (cmy >> 3) + 30, 0);
							g.drawRegion(imgBG[1], 0, 0, bgW[1], bgH[1], 2, -(GameScr.cmx >> layerSpeed[0]) + TileMap.tmw * 24 / 4 + bgW[1], yb[0] - (cmy >> 3) + 30, 0);
						}
						if (layer == 3 && TileMap.mapID == 1)
						{
							for (int k = 0; k < TileMap.pxh / mGraphics.getImageHeight(imgCaycot); k++)
							{
								g.drawImage(imgCaycot, -(GameScr.cmx >> layerSpeed[2]) + 300, k * mGraphics.getImageHeight(imgCaycot) - (cmy >> 3), 0);
							}
						}
					}
					int x = -(GameScr.cmx + moveX[num] >> layerSpeed[num]);
					EffecMn.paintBackGroundUnderLayer(g, x, yb[num] + bgH[num] - (cmy >> deltaY), num);
				}
			}
			catch (Exception ex)
			{
				Cout.LogError("Loi ham paint bground: " + ex.ToString());
			}
		}

	public static void drawSun1(mGraphics g)
		{
			if (imgSun != null)
			{
				g.drawImage(imgSun, sunX, sunY, 0);
			}
			if (!isBoltEff)
			{
				return;
			}
			if (gameTick % 200 == 0)
			{
				boltActive = true;
			}
			if (boltActive)
			{
				tBolt++;
				if (tBolt == 10)
				{
					tBolt = 0;
					boltActive = false;
				}
				if (tBolt % 2 == 0)
				{
					g.setColor(16777215);
					g.fillRect(0, 0, w, h);
				}
			}
		}

	public static void drawSun2(mGraphics g)
		{
			if (imgSun2 != null)
			{
				g.drawImage(imgSun2, sunX2, sunY2, 0);
			}
		}

	public static void paint_ios_bg(mGraphics g)
		{
			if (mSystem.clientType != 5)
			{
				return;
			}
			if (imgBgIOS != null)
			{
				g.setColor(0);
				g.fillRect(0, 0, w, h);
				for (int i = 0; i < 3; i++)
				{
					g.drawImage(imgBgIOS, imgBgIOS.getWidth() * i, h / 2, mGraphics.VCENTER | mGraphics.HCENTER);
				}
			}
			else
			{
				int num = ((TileMap.bgID % 2 != 0) ? 1 : 2);
				imgBgIOS = mSystem.loadImage("/bg/bg_ios_" + num + ".png");
			}
		}

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

	public static void loadBG(int typeBG)
		{
			try
			{
				isLoadBGok = true;
				if (typeBg == 12)
				{
					BackgroudEffect.yfog = TileMap.pxh - 100;
				}
				else
				{
					BackgroudEffect.yfog = TileMap.pxh - 160;
				}
				BackgroudEffect.clearImage();
				randomRaintEff(typeBG);
				if ((TileMap.lastBgID == typeBG && TileMap.lastType == TileMap.bgType) || typeBG == -1)
				{
					return;
				}
				transY = 12;
				TileMap.lastBgID = (sbyte)typeBG;
				TileMap.lastType = (sbyte)TileMap.bgType;
				layerSpeed = new int[5] { 1, 2, 3, 7, 8 };
				moveX = new int[5];
				moveXSpeed = new int[5];
				typeBg = typeBG;
				isBoltEff = false;
				GameScr.firstY = GameScr.cmy;
				imgBG = null;
				imgCloud = null;
				imgSun = null;
				imgCaycot = null;
				GameScr.firstY = -1;
				switch (typeBg)
				{
				case 0:
					imgCaycot = loadImageRMS("/bg/caycot.png");
					layerSpeed = new int[4] { 1, 3, 5, 7 };
					nBg = 4;
					if (TileMap.bgType == 2)
					{
						transY = 8;
					}
					break;
				case 1:
					transY = 7;
					nBg = 4;
					break;
				case 2:
					moveX = new int[5] { 0, 0, 1, 0, 0 };
					moveXSpeed = new int[5] { 0, 0, 2, 0, 0 };
					nBg = 5;
					break;
				case 3:
					nBg = 3;
					break;
				case 4:
					BackgroudEffect.addEffect(3);
					moveX = new int[5] { 0, 1, 0, 0, 0 };
					moveXSpeed = new int[5] { 0, 1, 0, 0, 0 };
					nBg = 4;
					break;
				case 5:
					nBg = 4;
					break;
				case 6:
					moveX = new int[5] { 1, 0, 0, 0, 0 };
					moveXSpeed = new int[5] { 2, 0, 0, 0, 0 };
					nBg = 5;
					break;
				case 7:
					nBg = 4;
					break;
				case 8:
					transY = 8;
					nBg = 4;
					break;
				case 9:
					BackgroudEffect.addEffect(9);
					nBg = 4;
					break;
				case 10:
					nBg = 2;
					break;
				case 11:
					transY = 7;
					layerSpeed[2] = 0;
					nBg = 3;
					break;
				case 12:
					moveX = new int[5] { 1, 1, 0, 0, 0 };
					moveXSpeed = new int[5] { 2, 1, 0, 0, 0 };
					nBg = 3;
					break;
				case 13:
					nBg = 2;
					break;
				case 15:
					Res.outz("HELL");
					nBg = 2;
					break;
				case 16:
					layerSpeed = new int[4] { 1, 3, 5, 7 };
					nBg = 4;
					break;
				case 19:
					moveX = new int[5] { 0, 2, 1, 0, 0 };
					moveXSpeed = new int[5] { 0, 2, 1, 0, 0 };
					nBg = 5;
					break;
				default:
					layerSpeed = new int[4] { 1, 3, 5, 7 };
					nBg = 4;
					break;
				}
				if (typeBG <= 16)
				{
					skyColor = StaticObj.SKYCOLOR[typeBg];
				}
				else
				{
					try
					{
						string path = "/bg/b" + typeBg + 3 + ".png";
						if (TileMap.bgType != 0)
						{
							path = "/bg/b" + typeBg + 3 + "-" + TileMap.bgType + ".png";
						}
						int[] data = new int[1];
						Image image = loadImageRMS(path);
						image.getRGB(ref data, 0, 1, mGraphics.getRealImageWidth(image) / 2, 0, 1, 1);
						skyColor = data[0];
					}
					catch (Exception)
					{
						skyColor = StaticObj.SKYCOLOR[StaticObj.SKYCOLOR.Length - 1];
					}
				}
				colorTop = new int[StaticObj.SKYCOLOR.Length];
				colorBotton = new int[StaticObj.SKYCOLOR.Length];
				for (int i = 0; i < StaticObj.SKYCOLOR.Length; i++)
				{
					colorTop[i] = StaticObj.SKYCOLOR[i];
					colorBotton[i] = StaticObj.SKYCOLOR[i];
				}
				if (lowGraphic)
				{
					tam = loadImageRMS("/bg/b63.png");
					return;
				}
				imgBG = new Image[nBg];
				bgW = new int[nBg];
				bgH = new int[nBg];
				colorBotton = new int[nBg];
				colorTop = new int[nBg];
				if (TileMap.bgType == 100)
				{
					imgBG[0] = loadImageRMS("/bg/b100.png");
					imgBG[1] = loadImageRMS("/bg/b100.png");
					imgBG[2] = loadImageRMS("/bg/b82-1.png");
					imgBG[3] = loadImageRMS("/bg/b93.png");
					for (int j = 0; j < nBg; j++)
					{
						if (imgBG[j] != null)
						{
							int[] data2 = new int[1];
							imgBG[j].getRGB(ref data2, 0, 1, mGraphics.getRealImageWidth(imgBG[j]) / 2, 0, 1, 1);
							colorTop[j] = data2[0];
							data2 = new int[1];
							imgBG[j].getRGB(ref data2, 0, 1, mGraphics.getRealImageWidth(imgBG[j]) / 2, mGraphics.getRealImageHeight(imgBG[j]) - 1, 1, 1);
							colorBotton[j] = data2[0];
							bgW[j] = mGraphics.getImageWidth(imgBG[j]);
							bgH[j] = mGraphics.getImageHeight(imgBG[j]);
						}
						else if (nBg > 1)
						{
							imgBG[j] = loadImageRMS("/bg/b" + typeBg + "0.png");
							bgW[j] = mGraphics.getImageWidth(imgBG[j]);
							bgH[j] = mGraphics.getImageHeight(imgBG[j]);
						}
					}
				}
				else
				{
					for (int k = 0; k < nBg; k++)
					{
						string path2 = "/bg/b" + typeBg + k + ".png";
						if (TileMap.bgType != 0)
						{
							path2 = "/bg/b" + typeBg + k + "-" + TileMap.bgType + ".png";
						}
						imgBG[k] = loadImageRMS(path2);
						if (imgBG[k] != null)
						{
							int[] data3 = new int[1];
							imgBG[k].getRGB(ref data3, 0, 1, mGraphics.getRealImageWidth(imgBG[k]) / 2, 0, 1, 1);
							colorTop[k] = data3[0];
							data3 = new int[1];
							imgBG[k].getRGB(ref data3, 0, 1, mGraphics.getRealImageWidth(imgBG[k]) / 2, mGraphics.getRealImageHeight(imgBG[k]) - 1, 1, 1);
							colorBotton[k] = data3[0];
							bgW[k] = mGraphics.getImageWidth(imgBG[k]);
							bgH[k] = mGraphics.getImageHeight(imgBG[k]);
						}
						else if (nBg > 1)
						{
							imgBG[k] = loadImageRMS("/bg/b" + typeBg + "0.png");
							bgW[k] = mGraphics.getImageWidth(imgBG[k]);
							bgH[k] = mGraphics.getImageHeight(imgBG[k]);
						}
					}
				}
				getYBackground(typeBg);
				cloudX = new int[5]
				{
					GameScr.gW / 2 - 40,
					GameScr.gW / 2 + 40,
					GameScr.gW / 2 - 100,
					GameScr.gW / 2 - 80,
					GameScr.gW / 2 - 120
				};
				cloudY = new int[5] { 130, 100, 150, 140, 80 };
				imgSunSpec = null;
				if (typeBg != 0)
				{
					if (typeBg == 2)
					{
						imgSun = loadImageRMS("/bg/sun0.png");
						sunX = GameScr.gW / 2 + 50;
						sunY = yb[4] - 40;
						TileMap.imgWaterflow = loadImageRMS("/tWater/wts");
					}
					else if (typeBg == 19)
					{
						TileMap.imgWaterflow = loadImageRMS("/tWater/water_flow_32");
					}
					else if (typeBg == 4)
					{
						imgSun = loadImageRMS("/bg/sun2.png");
						sunX = GameScr.gW / 2 + 30;
						sunY = yb[3];
					}
					else if (typeBg == 7)
					{
						imgSun = loadImageRMS("/bg/sun3" + ((TileMap.bgType != 0) ? ("-" + TileMap.bgType) : string.Empty) + ".png");
						imgSun2 = loadImageRMS("/bg/sun4" + ((TileMap.bgType != 0) ? ("-" + TileMap.bgType) : string.Empty) + ".png");
						sunX = GameScr.gW - GameScr.gW / 3;
						sunY = yb[3] - 80;
						sunX2 = sunX - 100;
						sunY2 = yb[3] - 30;
					}
					else if (typeBg == 6)
					{
						imgSun = loadImageRMS("/bg/sun5" + ((TileMap.bgType != 0) ? ("-" + TileMap.bgType) : string.Empty) + ".png");
						imgSun2 = loadImageRMS("/bg/sun6" + ((TileMap.bgType != 0) ? ("-" + TileMap.bgType) : string.Empty) + ".png");
						sunX = GameScr.gW - GameScr.gW / 3;
						sunY = yb[4];
						sunX2 = sunX - 100;
						sunY2 = yb[4] + 20;
					}
					else if (typeBG == 5)
					{
						imgSun = loadImageRMS("/bg/sun8" + ((TileMap.bgType != 0) ? ("-" + TileMap.bgType) : string.Empty) + ".png");
						imgSun2 = loadImageRMS("/bg/sun7" + ((TileMap.bgType != 0) ? ("-" + TileMap.bgType) : string.Empty) + ".png");
						sunX = GameScr.gW / 2 - 50;
						sunY = yb[3] + 20;
						sunX2 = GameScr.gW / 2 + 20;
						sunY2 = yb[3] - 30;
					}
					else if (typeBg == 8 && TileMap.mapID < 90)
					{
						imgSun = loadImageRMS("/bg/sun9" + ((TileMap.bgType != 0) ? ("-" + TileMap.bgType) : string.Empty) + ".png");
						imgSun2 = loadImageRMS("/bg/sun10" + ((TileMap.bgType != 0) ? ("-" + TileMap.bgType) : string.Empty) + ".png");
						sunX = GameScr.gW / 2 - 30;
						sunY = yb[3] + 60;
						sunX2 = GameScr.gW / 2 + 20;
						sunY2 = yb[3] + 10;
					}
					else
					{
						switch (typeBG)
						{
						case 9:
							imgSun = loadImageRMS("/bg/sun11" + ((TileMap.bgType != 0) ? ("-" + TileMap.bgType) : string.Empty) + ".png");
							imgSun2 = loadImageRMS("/bg/sun12" + ((TileMap.bgType != 0) ? ("-" + TileMap.bgType) : string.Empty) + ".png");
							sunX = GameScr.gW - GameScr.gW / 3;
							sunY = yb[4] + 20;
							sunX2 = sunX - 80;
							sunY2 = yb[4] + 40;
							break;
						case 10:
							imgSun = loadImageRMS("/bg/sun13" + ((TileMap.bgType != 0) ? ("-" + TileMap.bgType) : string.Empty) + ".png");
							imgSun2 = loadImageRMS("/bg/sun14" + ((TileMap.bgType != 0) ? ("-" + TileMap.bgType) : string.Empty) + ".png");
							sunX = GameScr.gW - GameScr.gW / 3;
							sunY = yb[1] - 30;
							sunX2 = sunX - 80;
							sunY2 = yb[1];
							break;
						case 11:
							imgSun = loadImageRMS("/bg/sun15" + ((TileMap.bgType != 0) ? ("-" + TileMap.bgType) : string.Empty) + ".png");
							imgSun2 = loadImageRMS("/bg/b113" + ((TileMap.bgType != 0) ? ("-" + TileMap.bgType) : string.Empty) + ".png");
							sunX = GameScr.gW / 2 - 30;
							sunY = yb[2] - 30;
							break;
						case 12:
							cloudY = new int[5] { 200, 170, 220, 150, 250 };
							break;
						case 16:
						{
							cloudX = new int[7] { 90, 170, 250, 320, 400, 450, 500 };
							cloudY = new int[7]
							{
								yb[2] + 5,
								yb[2] - 20,
								yb[2] - 50,
								yb[2] - 30,
								yb[2] - 50,
								yb[2],
								yb[2] - 40
							};
							imgSunSpec = new Image[7];
							for (int l = 0; l < imgSunSpec.Length; l++)
							{
								int num = 161;
								if (l == 0 || l == 2 || l == 3 || l == 2 || l == 6)
								{
									num = 160;
								}
								imgSunSpec[l] = loadImageRMS("/bg/sun" + num + ".png");
							}
							break;
						}
						case 19:
							moveX = new int[5] { 0, 2, 1, 0, 0 };
							moveXSpeed = new int[5] { 0, 2, 1, 0, 0 };
							nBg = 5;
							break;
						default:
							imgCloud = null;
							imgSun = null;
							imgSun2 = null;
							imgSun = loadImageRMS("/bg/sun" + typeBG + ((TileMap.bgType != 0) ? ("-" + TileMap.bgType) : string.Empty) + ".png");
							if (loadImageRMS("/tWater/water_flow_" + typeBG) != null)
							{
								TileMap.imgWaterflow = loadImageRMS("/tWater/water_flow_" + typeBG);
							}
							sunX = GameScr.gW - GameScr.gW / 3;
							sunY = yb[2] - 30;
							break;
						}
					}
				}
				paintBG = false;
				if (!paintBG)
				{
					paintBG = true;
				}
			}
			catch (Exception)
			{
				isLoadBGok = false;
			}
		}

	public void paintChangeMap(mGraphics g)
		{
			string empty = string.Empty;
			resetTrans(g);
			g.setColor(0);
			g.fillRect(0, 0, w, h);
			g.drawImage(LoginScr.imgTitle, w / 2, h / 2 - 24, StaticObj.BOTTOM_HCENTER);
			paintShukiren(hw, h / 2 + 24, g);
			mFont.tahoma_7b_white.drawString(g, mResources.PLEASEWAIT + ((LoginScr.timeLogin <= 0) ? empty : (" " + LoginScr.timeLogin + "s")), w / 2, h / 2, 2);
		}

	public void paint(mGraphics gx)
		{
			try
			{
				debugPaint.removeAllElements();
				debug("PA", 1);
				if (currentScreen != null)
				{
					currentScreen.paint(g);
				}
				debug("PB", 1);
				g.translate(-g.getTranslateX(), -g.getTranslateY());
				g.setClip(0, 0, w, h);
				if (panel.isShow)
				{
					panel.paint(g);
					if (panel2 != null && panel2.isShow)
					{
						panel2.paint(g);
					}
					if (panel.chatTField != null && panel.chatTField.isShow)
					{
						panel.chatTField.paint(g);
					}
					if (panel2 != null && panel2.chatTField != null && panel2.chatTField.isShow)
					{
						panel2.chatTField.paint(g);
					}
				}
				Res.paintOnScreenDebug(g);
				InfoDlg.paint(g);
				if (currentDialog != null)
				{
					debug("PC", 1);
					currentDialog.paint(g);
				}
				else if (menu.showMenu)
				{
					debug("PD", 1);
					resetTrans(g);
					menu.paintMenu(g);
				}
				GameScr.info1.paint(g);
				GameScr.info2.paint(g);
				if (GameScr.gI().popUpYesNo != null)
				{
					GameScr.gI().popUpYesNo.paint(g);
				}
				if (ChatPopup.currChatPopup != null)
				{
					ChatPopup.currChatPopup.paint(g);
				}
				Hint.paint(g);
				if (ChatPopup.serverChatPopUp != null)
				{
					ChatPopup.serverChatPopUp.paint(g);
				}
				for (int i = 0; i < Effect2.vEffect2.size(); i++)
				{
					Effect2 effect = (Effect2)Effect2.vEffect2.elementAt(i);
					if (effect is ChatPopup && !effect.Equals(ChatPopup.currChatPopup) && !effect.Equals(ChatPopup.serverChatPopUp))
					{
						effect.paint(g);
					}
				}
				if (currentDialog != null)
				{
					currentDialog.paint(g);
				}
				if (isWait())
				{
					paintChangeMap(g);
					if (timeLoading > 0 && LoginScr.timeLogin <= 0 && mSystem.currentTimeMillis() - TIMEOUT >= 1000)
					{
						timeLoading--;
						if (timeLoading == 0)
						{
							timeLoading = 15;
						}
						TIMEOUT = mSystem.currentTimeMillis();
					}
				}
				debug("PE", 1);
				resetTrans(g);
				EffecMn.paintLayer4(g);
				if (open3Hour && !isLoading)
				{
					if (currentScreen == loginScr || currentScreen == serverScreen || currentScreen == serverScr)
					{
						g.drawImage(img18, 5, 5, 0);
					}
					if (currentScreen == CreateCharScr.instance)
					{
						g.drawImage(img18, hw, 5, 0);
					}
				}
				resetTrans(g);
				int num = h / 4;
				if (currentScreen != null && currentScreen is GameScr && thongBaoTest != null)
				{
					g.setClip(60, num, w - 120, mFont.tahoma_7_white.getHeight() + 2);
					mFont.tahoma_7_grey.drawString(g, thongBaoTest, xThongBaoTranslate, num + 1, 0);
					mFont.tahoma_7_yellow.drawString(g, thongBaoTest, xThongBaoTranslate, num, 0);
					g.setClip(0, 0, w, h);
				}
				resetTrans(g);
				ModMenu.Paint(g);
			}
			catch (Exception)
			{
			}
		}

	public void paintDust(mGraphics g)
		{
			if (lowGraphic || ModMenu.graphicsQuality >= 1)
			{
				return;
			}
			for (int i = 0; i < 2; i++)
			{
				if (dustState[i] != -1 && isPaint(dustX[i], dustY[i]))
				{
					g.drawImage(imgDust[i][dustState[i]], dustX[i], dustY[i], 3);
				}
			}
		}

	public static void paintShukiren(int x, int y, mGraphics g)
		{
			g.drawRegion(imgShuriken, 0, Main.f * 16, 16, 16, 0, x, y, mGraphics.HCENTER | mGraphics.VCENTER);
		}

}
