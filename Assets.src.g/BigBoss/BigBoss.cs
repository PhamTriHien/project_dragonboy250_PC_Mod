using System;

namespace Assets.src.g;

public partial class BigBoss : Mob, IMapObject
{
	public static Image shadowBig = GameCanvas.loadImage("/mainImage/shadowBig.png");

	public static EffectData data;

	public int xTo;

	public int yTo;

	public bool haftBody;

	public bool change;

	public new int xSd;

	public new int ySd;

	private bool isOutMap;

	private int wCount;

	public new bool isShadown = true;

	private int tick;

	private int frame;

	private bool wy;

	private int wt;

	private int fy;

	private int ty;

	public new int typeSuperEff;

	private Char focus;

	private bool flyUp;

	private bool flyDown;

	private int dy;

	public bool changePos;

	private int tShock;

	public new bool isBusyAttackSomeOne = true;

	private int tA;

	private Char[] charAttack;

	private long[] dameHP;

	private sbyte type;

	public new int[] stand = new int[12]
		{
			0, 0, 0, 0, 0, 0, 0, 0, 1, 1,
			1, 1
		};

	public int[] stand_1 = new int[17]
		{
			37, 37, 37, 38, 38, 38, 39, 39, 40, 40,
			40, 39, 39, 39, 38, 38, 38
		};

	public new int[] move = new int[15]
		{
			1, 1, 1, 1, 2, 2, 2, 2, 3, 3,
			3, 3, 2, 2, 2
		};

	public new int[] moveFast = new int[7] { 1, 1, 2, 2, 3, 3, 2 };

	public new int[] attack1 = new int[12]
		{
			0, 0, 34, 34, 35, 35, 36, 36, 2, 2,
			1, 1
		};

	public new int[] attack2 = new int[23]
		{
			0, 0, 0, 4, 4, 6, 6, 9, 9, 10,
			10, 13, 13, 15, 15, 17, 17, 19, 19, 21,
			21, 23, 23
		};

	public int[] attack3 = new int[24]
		{
			0, 0, 1, 1, 4, 4, 6, 6, 8, 8,
			25, 25, 26, 26, 28, 28, 30, 30, 32, 32,
			2, 2, 1, 1
		};

	public int[] attack2_1 = new int[20]
		{
			37, 37, 5, 5, 7, 7, 11, 11, 14, 14,
			16, 16, 18, 18, 20, 20, 22, 22, 24, 24
		};

	public int[] attack3_1 = new int[21]
		{
			37, 37, 37, 38, 38, 5, 5, 7, 7, 11,
			11, 27, 27, 29, 29, 31, 31, 33, 33, 38,
			38
		};

	public int[] fly = new int[8] { 8, 8, 9, 9, 10, 10, 12, 12 };

	public int[] hitground = new int[24]
		{
			0, 0, 1, 1, 4, 4, 6, 6, 8, 8,
			25, 25, 26, 26, 28, 28, 30, 30, 32, 32,
			2, 2, 1, 1
		};

	private bool shock;

	private sbyte[] cou = new sbyte[2] { -1, 1 };

	public new Char injureBy;

	public new bool injureThenDie;

	public new Mob mobToAttack;

	public new int forceWait;

	public new bool blindEff;

	public new bool sleepEff;

	public BigBoss(int id, short px, short py, int templateID, long hp, long maxhp, int s)
		{
			xFirst = (x = px + 20);
			yFirst = (y = py);
			mobId = id;
			base.hp = hp;
			maxHp = maxhp;
			templateId = templateID;
			w_hp_bar = 100;
			h_hp_bar = 6;
			len = w_hp_bar;
			updateHp_bar();
			if (s == 0)
			{
				getDataB();
			}
			if (s == 1)
			{
				getDataB2();
			}
			if (s == 2)
			{
				getDataB2();
				haftBody = true;
			}
			status = 2;
		}

	public void getDataB2()
		{
			data = null;
			data = new EffectData();
			string patch = "/x" + mGraphics.zoomLevel + "/effectdata/" + 100 + "/data";
			try
			{
				data.readData2(patch);
				data.img = GameCanvas.loadImage("/effectdata/" + 100 + "/img.png");
			}
			catch (Exception)
			{
				Service.gI().requestModTemplate(templateId);
			}
			status = 2;
			w = data.width;
			h = data.height;
		}

	public void getDataB()
		{
			data = null;
			data = new EffectData();
			string patch = "/x" + mGraphics.zoomLevel + "/effectdata/" + 101 + "/data";
			try
			{
				data.readData2(patch);
				data.img = GameCanvas.loadImage("/effectdata/" + 101 + "/img.png");
				Res.outz("read xong data");
			}
			catch (Exception)
			{
				Service.gI().requestModTemplate(templateId);
			}
			w = data.width;
			h = data.height;
		}

	public override void clearBody()
		{
			changBody = false;
		}

	public new static bool isExistNewMob(string id)
		{
			for (int i = 0; i < Mob.newMob.size(); i++)
			{
				string text = (string)Mob.newMob.elementAt(i);
				if (text.Equals(id))
				{
					return true;
				}
			}
			return false;
		}

	public new void checkFrameTick(int[] array)
		{
			tick++;
			if (tick > array.Length - 1)
			{
				tick = 0;
			}
			frame = array[tick];
		}

	private bool isSpecial()
		{
			if ((templateId >= 58 && templateId <= 65) || templateId == 67 || templateId == 68)
			{
				return true;
			}
			return false;
		}

	public new bool checkIsBoss()
		{
			if (isBoss || levelBoss > 0)
			{
				return true;
			}
			return false;
		}

	public new int getHPColor()
		{
			return 16711680;
		}

	public new void startDie()
		{
			hp = 0L;
			injureThenDie = true;
			hp = 0L;
			status = 1;
			p1 = -3;
			p2 = -dir;
			p3 = 0;
		}

	public new int getX()
		{
			return x;
		}

	public new int getY()
		{
			return (!haftBody) ? (y - 60) : (y - 20);
		}

	public new int getH()
		{
			return 40;
		}

	public new int getW()
		{
			return 60;
		}

	public new void stopMoving()
		{
			if (status == 5)
			{
				status = 2;
				p1 = (p2 = (p3 = 0));
				forceWait = 50;
			}
		}

	public new bool isInvisible()
		{
			return status == 0 || status == 1;
		}

}
