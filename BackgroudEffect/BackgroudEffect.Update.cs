using System;
using Assets.src.e;

public partial class BackgroudEffect
{
	public static void initCloud()
		{
			if (mSystem.clientType == 1)
			{
				imgCloud1 = null;
				imgFog = null;
				return;
			}
			if (GameCanvas.lowGraphic)
			{
				imgCloud1 = null;
				imgFog = null;
				return;
			}
			if (nCloud > 0)
			{
				if (imgCloud1 == null)
				{
					imgCloud1 = GameCanvas.loadImage("/bg/fog1.png");
					cloudw = imgCloud1.getWidth();
				}
			}
			else
			{
				imgCloud1 = null;
			}
			if (!isFog)
			{
				imgFog = null;
				return;
			}
			if (imgFog == null)
			{
				imgFog = GameCanvas.loadImage("/bg/fog0.png");
			}
			fogw = 287;
		}

	public static void updateCloud2()
		{
			if (mSystem.clientType == 1 || GameCanvas.lowGraphic || ModMenu.graphicsQuality >= 1 || nCloud <= 0)
			{
				return;
			}
			int num = ((GameCanvas.currentScreen != GameScr.gI()) ? (GameScr.cmx + GameCanvas.w) : TileMap.pxw);
			for (int i = 0; i < nCloud; i++)
			{
				int num2 = i + 1;
				GameCanvas.cloudX[i] -= num2;
				if (GameCanvas.cloudX[i] < -cloudw)
				{
					GameCanvas.cloudX[i] = num + 100;
				}
			}
		}

	public static void updateFog()
		{
			if (mSystem.clientType != 1 && !GameCanvas.lowGraphic && isFog)
			{
				xfog--;
				if (xfog < -fogw)
				{
					xfog = 0;
				}
			}
		}

	public void update()
		{
			try
			{
				switch (typeEff)
				{
				case 10:
				{
					for (int m = 0; m < this.num; m++)
					{
						x[m] -= vx[m];
						if (x[m] < -vx[m] + GameScr.cmx)
						{
							x[m] = GameCanvas.w + vx[m] + GameScr.cmx;
						}
					}
					break;
				}
				case 9:
				{
					for (int i = 0; i < this.num; i++)
					{
						x[i] -= vx[i];
						if (x[i] < -vx[i])
						{
							wP[i] = Res.abs(Res.random(1, 3));
							vx[i] = wP[i];
							x[i] = GameCanvas.w + vx[i];
						}
					}
					break;
				}
				case 3:
					break;
				case 0:
				case 12:
				{
					for (int l = 0; l < sum; l++)
					{
						if (l % 3 != 0 && typeEff != 12 && TileMap.tileTypeAt(x[l], y[l] - GameCanvas.transY, 2))
						{
							activeEff[l] = true;
						}
						if (l % 3 == 0 && y[l] > GameCanvas.h + GameScr.cmy)
						{
							x[l] = Res.random(-10, GameCanvas.w + 300) + GameScr.cmx;
							y[l] = Res.random(-100, 0) + GameScr.cmy;
						}
						if (!activeEff[l])
						{
							y[l] += vy[l];
							x[l] += vx[l];
						}
						if (!activeEff[l])
						{
							continue;
						}
						t[l]++;
						if (t[l] > 2)
						{
							frame[l]++;
							t[l] = 0;
							if (frame[l] > 1)
							{
								frame[l] = 0;
								activeEff[l] = false;
								x[l] = Res.random(-10, GameCanvas.w + 300) + GameScr.cmx;
								y[l] = Res.random(-100, 0) + GameScr.cmy;
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
				{
					for (int j = 0; j < sum; j++)
					{
						if (j % 3 != 0 && TileMap.tileTypeAt(x[j], y[j] + ((TileMap.tileID == 15) ? 10 : 0), 2))
						{
							activeEff[j] = true;
						}
						if (j % 3 == 0 && y[j] > TileMap.pxh)
						{
							x[j] = Res.random(-10, TileMap.pxw + 50);
							y[j] = Res.random(-50, 0);
						}
						if (!activeEff[j])
						{
							for (int k = 0; k < Teleport.vTeleport.size(); k++)
							{
								Teleport teleport = (Teleport)Teleport.vTeleport.elementAt(k);
								if (teleport != null && teleport.paintFire && x[j] < teleport.x + 80 && x[j] > teleport.x - 80 && y[j] < teleport.y + 80 && y[j] > teleport.y - 80)
								{
									x[j] += ((x[j] >= teleport.x) ? 10 : (-10));
								}
							}
							y[j] += vy[j];
							x[j] += vx[j];
							t[j]++;
							int num = ((typeEff != 11) ? 4 : 3);
							num = ((typeEff != 15) ? 4 : 4);
							if (t[j] > ((typeEff == 2) ? 4 : 2))
							{
								if (typeEff != 11 && typeEff != 15)
								{
									frame[j]++;
								}
								t[j] = 0;
								if (frame[j] > num - 1)
								{
									frame[j] = 0;
								}
							}
						}
						else
						{
							t[j]++;
							if (t[j] == 100)
							{
								t[j] = 0;
								x[j] = Res.random(-10, TileMap.pxw + 50);
								y[j] = Res.random(-50, 0);
								activeEff[j] = false;
							}
						}
					}
					break;
				}
				case 4:
				{
					for (int n = 0; n < sum; n++)
					{
						t[n]++;
						if (t[n] > 10)
						{
							tick[n]++;
							t[n] = 0;
							if (tick[n] > 5)
							{
								tick[n] = 0;
							}
							frame[n] = dem[tick[n]];
						}
					}
					break;
				}
				case 8:
					tFire++;
					if (tFire == 3)
					{
						tFire = 0;
						frameFire++;
						if (frameFire > 1)
						{
							frameFire = 0;
						}
					}
					if (GameCanvas.gameTick % tStart == 0)
					{
						isFly = true;
					}
					if (!isFly)
					{
						break;
					}
					if (way == 1)
					{
						xShip += speed;
						if (xShip > TileMap.pxw + 50)
						{
							reloadShip();
						}
					}
					else if (way == 2)
					{
						xShip -= speed;
						if (xShip < -50)
						{
							reloadShip();
						}
					}
					else if (way == 3)
					{
						yShip += speed;
						if (yShip > TileMap.pxh + 50)
						{
							reloadShip();
						}
					}
					else if (way == 4)
					{
						yShip -= speed;
						if (yShip < -50)
						{
							reloadShip();
						}
					}
					break;
				case 13:
					updateCloud2();
					break;
				case 14:
					updateFog();
					break;
				}
			}
			catch (Exception)
			{
			}
		}

	public static void updateEff()
		{
			if (ModMenu.graphicsQuality >= 1) return;
			for (int i = 0; i < vBgEffect.size(); i++)
			{
				((BackgroudEffect)vBgEffect.elementAt(i)).update();
			}
		}

}
