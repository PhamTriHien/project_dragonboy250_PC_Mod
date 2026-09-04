using System;
using Assets.src.e;
using Assets.src.g;
using UnityEngine;
public partial class GameCanvas : IActionListener
{
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

}
