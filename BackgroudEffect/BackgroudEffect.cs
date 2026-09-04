using System;
using Assets.src.e;

public partial class BackgroudEffect
{
	public static MyVector vBgEffect = new MyVector();

	private int[] x;

	private int[] y;

	private int[] vx;

	private int[] vy;

	public static int[] wP;

	private int num;

	private int xShip;

	private int yShip;

	private int way;

	private int trans;

	private int frameFire;

	private int tFire;

	private int tStart;

	private int speed;

	private bool isFly;

	public static Image imgSnow;

	public static Image imgHatMua;

	public static Image imgMua1;

	public static Image imgMua2;

	public static Image imgSao;

	private static Image imgLacay;

	private static Image imgShip;

	private static Image imgFire1;

	private static Image imgFire2;

	private int[] type;

	private int sum;

	public int typeEff;

	public int xx;

	public int waterY;

	private bool[] isRainEffect;

	private int[] frame;

	private int[] t;

	private bool[] activeEff;

	private int yWater;

	private int colorWater;

	public const int TYPE_MUA = 0;

	public const int TYPE_LATRAIDAT_1 = 1;

	public const int TYPE_LATRAIDAT_2 = 2;

	public const int TYPE_SAMSET = 3;

	public const int TYPE_SAO = 4;

	public const int TYPE_LANAMEK_1 = 5;

	public const int TYPE_LASAYAI_1 = 6;

	public const int TYPE_LANAMEK_2 = 7;

	public const int TYPE_SHIP_TRAIDAT = 8;

	public const int TYPE_HANHTINH = 9;

	public const int TYPE_WATER = 10;

	public const int TYPE_SNOW = 11;

	public const int TYPE_MUA_FRONT = 12;

	public const int TYPE_CLOUD = 13;

	public const int TYPE_FOG = 14;

	public const int TYPE_LUNAR_YEAR = 15;

	public static int PIXEL = 16;

	public static Image water1 = GameCanvas.loadImage("/mainImage/myTexture2dwater1.png");

	public static Image water2 = GameCanvas.loadImage("/mainImage/myTexture2dwater2.png");

	public static Image imgChamTron1;

	public static Image imgChamTron2;

	public static short id_water1;

	public static short id_water2;

	public static Image water3 = null;

	public static bool isFog;

	public static bool isPaintFar;

	public static int nCloud;

	public static Image imgCloud1;

	public static Image imgFog;

	public static int cloudw;

	public static int xfog;

	public static int yfog;

	public static int fogw;

	private int[] dem = new int[6] { 0, 1, 2, 1, 0, 0 };

	private int[] tick;

	public BackgroudEffect(int typeS)
		{
			isFog = true;
			initCloud();
			typeEff = typeS;
			switch (typeEff)
			{
			case 10:
			{
				this.num = 30;
				x = new int[this.num];
				y = new int[this.num];
				wP = new int[this.num];
				vx = new int[this.num];
				int num = 0;
				for (int l = 0; l < this.num; l++)
				{
					x[l] = Res.abs(Res.random(0, GameCanvas.w)) + GameScr.cmx;
					num++;
					if (num > this.num / 2)
					{
						y[l] = Res.abs(Res.random(20, 60));
						wP[l] = 10;
					}
					else
					{
						y[l] = Res.abs(Res.random(0, 20));
						wP[l] = 7;
					}
					vx[l] = wP[l] / 2 - 2;
				}
				break;
			}
			case 9:
			{
				if (imgChamTron1 == null)
				{
					imgChamTron1 = GameCanvas.loadImageRMS("/bg/cham-tron1.png");
				}
				if (imgChamTron2 == null)
				{
					imgChamTron2 = GameCanvas.loadImageRMS("/bg/cham-tron2.png");
				}
				this.num = 20;
				x = new int[this.num];
				y = new int[this.num];
				wP = new int[this.num];
				vx = new int[this.num];
				for (int i = 0; i < this.num; i++)
				{
					x[i] = Res.abs(Res.random(0, GameCanvas.w));
					y[i] = Res.abs(Res.random(10, 80));
					wP[i] = Res.abs(Res.random(1, 3));
					vx[i] = wP[i];
				}
				break;
			}
			case 0:
			case 12:
			{
				if (imgHatMua == null)
				{
					imgHatMua = GameCanvas.loadImageRMS("/bg/mua.png");
				}
				if (imgMua1 == null)
				{
					imgMua1 = GameCanvas.loadImageRMS("/bg/mua1.png");
				}
				if (imgMua2 == null)
				{
					imgMua2 = GameCanvas.loadImageRMS("/bg/mua2.png");
				}
				sum = Res.random(GameCanvas.w / 3, GameCanvas.w / 2);
				x = new int[sum];
				y = new int[sum];
				vx = new int[sum];
				vy = new int[sum];
				type = new int[sum];
				t = new int[sum];
				frame = new int[sum];
				isRainEffect = new bool[sum];
				activeEff = new bool[sum];
				for (int k = 0; k < sum; k++)
				{
					y[k] = Res.random(-10, GameCanvas.h + 100) + GameScr.cmy;
					x[k] = Res.random(-10, GameCanvas.w + 300) + GameScr.cmx;
					t[k] = Res.random(0, 1);
					vx[k] = -12;
					vy[k] = 12;
					type[k] = Res.random(1, 3);
					isRainEffect[k] = false;
					if (type[k] == 2 && k % 2 == 0)
					{
						isRainEffect[k] = true;
					}
					activeEff[k] = false;
					frame[k] = Res.random(1, 2);
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
				if (typeEff == 1)
				{
					imgLacay = GameCanvas.loadImageRMS("/bg/lacay.png");
					PIXEL = 10;
				}
				if (typeEff == 2)
				{
					imgLacay = GameCanvas.loadImageRMS("/bg/lacay2.png");
					PIXEL = 18;
				}
				if (typeEff == 5)
				{
					imgLacay = GameCanvas.loadImageRMS("/bg/lacay3.png");
					PIXEL = 14;
				}
				if (typeEff == 6)
				{
					imgLacay = GameCanvas.loadImageRMS("/bg/lacay4.png");
					PIXEL = 14;
				}
				if (typeEff == 7)
				{
					imgLacay = GameCanvas.loadImageRMS("/bg/lacay5.png");
					PIXEL = 12;
				}
				if (typeEff == 11)
				{
					imgLacay = GameCanvas.loadImageRMS("/bg/tuyet.png");
				}
				if (typeEff == 15)
				{
					Small small = SmallImage.imgNew[11120];
					if (small == null)
					{
						SmallImage.createImage(11120);
					}
					PIXEL = 16;
				}
				sum = Res.random(15, 25);
				if (typeEff == 11)
				{
					sum = 100;
				}
				x = new int[sum];
				y = new int[sum];
				vx = new int[sum];
				vy = new int[sum];
				t = new int[sum];
				frame = new int[sum];
				activeEff = new bool[sum];
				for (int j = 0; j < sum; j++)
				{
					x[j] = Res.random(-10, TileMap.pxw + 10);
					y[j] = Res.random(0, TileMap.pxh);
					frame[j] = Res.random(0, 1);
					t[j] = Res.random(0, 1);
					vx[j] = Res.random(-3, 3);
					vy[j] = Res.random(1, 4);
					if (typeEff == 11)
					{
						frame[j] = Res.random(0, 2);
						vx[j] = Res.abs(Res.random(1, 3));
						vy[j] = Res.abs(Res.random(1, 3));
					}
					if (typeEff == 15)
					{
						frame[j] = Res.random(0, 2);
						vx[j] = Res.abs(Res.random(1, 3));
						vy[j] = Res.abs(Res.random(1, 3));
					}
				}
				break;
			}
			case 4:
			{
				sum = Res.random(5, 10);
				if (imgSao == null)
				{
					imgSao = GameCanvas.loadImageRMS("/bg/sao.png");
				}
				x = new int[sum];
				y = new int[sum];
				frame = new int[sum];
				t = new int[sum];
				tick = new int[sum];
				for (int m = 0; m < sum; m++)
				{
					x[m] = Res.random(0, GameCanvas.w);
					y[m] = Res.random(0, 50);
					if (m % 2 == 0)
					{
						tick[m] = 0;
					}
					else if (m % 3 == 0)
					{
						tick[m] = 1;
					}
					else if (m % 4 == 0)
					{
						tick[m] = 2;
					}
					else
					{
						tick[m] = 3;
					}
					t[m] = Res.random(0, 10);
				}
				break;
			}
			case 3:
				GameCanvas.isBoltEff = true;
				break;
			case 8:
				tStart = Res.random(100, 300);
				if (imgShip == null)
				{
					imgShip = GameCanvas.loadImageRMS("/bg/ship.png");
				}
				if (imgFire1 == null)
				{
					imgFire1 = GameCanvas.loadImageRMS("/bg/fire1.png");
				}
				if (imgFire2 == null)
				{
					imgFire2 = GameCanvas.loadImageRMS("/bg/fire2.png");
				}
				isFly = false;
				reloadShip();
				break;
			case 13:
				if (Res.abs(Res.random(0, 2)) == 0)
				{
					if (Res.abs(Res.random(0, 2)) == 0)
					{
						isPaintFar = true;
					}
					else
					{
						isPaintFar = false;
					}
					nCloud = Res.abs(Res.random(2, 5));
					initCloud();
				}
				break;
			case 14:
				if (Res.abs(Res.random(0, 2)) == 0)
				{
					isFog = true;
					initCloud();
				}
				break;
			}
		}

	public static void clearImage()
		{
			TileMap.yWater = 0;
		}

	public static bool isHaveRain()
		{
			for (int i = 0; i < vBgEffect.size(); i++)
			{
				BackgroudEffect backgroudEffect = (BackgroudEffect)vBgEffect.elementAt(i);
				if (backgroudEffect.typeEff == 0 || backgroudEffect.typeEff == 12)
				{
					return true;
				}
			}
			return false;
		}

	private void reloadShip()
		{
			int cmx = GameScr.cmx;
			int cmy = GameScr.cmy;
			way = Res.random(1, 3);
			isFly = false;
			speed = Res.random(3, 5);
			if (way == 1)
			{
				xShip = -50;
				yShip = Res.random(cmy, GameCanvas.h - 100 + cmy);
				trans = 0;
			}
			else if (way == 2)
			{
				xShip = TileMap.pxw + 50;
				yShip = Res.random(cmy, GameCanvas.h - 100 + cmy);
				trans = 2;
			}
			else if (way == 3)
			{
				xShip = Res.random(50 + cmx, GameCanvas.w - 50 + cmx);
				yShip = -50;
				int num = Res.random(0, 2);
				trans = ((num != 0) ? 2 : 0);
			}
			else if (way == 4)
			{
				xShip = Res.random(50 + cmx, GameCanvas.w - 50 + cmx);
				yShip = TileMap.pxh + 50;
				int num2 = Res.random(0, 2);
				trans = ((num2 != 0) ? 2 : 0);
			}
		}

	public static void addEffect(int id)
		{
			if (!GameCanvas.lowGraphic)
			{
				BackgroudEffect o = new BackgroudEffect(id);
				vBgEffect.addElement(o);
			}
		}

	public static void addWater(int color, int yWater)
		{
			BackgroudEffect backgroudEffect = new BackgroudEffect(10);
			backgroudEffect.colorWater = color;
			backgroudEffect.yWater = yWater;
			vBgEffect.addElement(backgroudEffect);
		}

}
