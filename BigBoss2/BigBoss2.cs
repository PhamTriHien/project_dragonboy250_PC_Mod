using System;

public partial class BigBoss2 : Mob, IMapObject
{
	public static Image shadowBig;

	public static EffectData data;

	public int xTo;

	public int yTo;

	public bool haftBody;

	public bool change;

	private Mob mob1;

	public new int xSd;

	public new int ySd;

	private bool isOutMap;

	private int wCount;

	public new bool isShadown = true;

	private int tick;

	private int frame;

	public new static Image imgHP = GameCanvas.loadImage("/mainImage/myTexture2dmobHP.png");

	private bool wy;

	private int wt;

	private int fy;

	private int ty;

	public new int typeSuperEff;

	private Char focus;

	private int timeDead;

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
			0, 0, 0, 0, 0, 0, 1, 1, 1, 1,
			1, 1
		};

	public new int[] move = new int[15]
		{
			1, 1, 1, 1, 2, 2, 2, 2, 3, 3,
			3, 3, 2, 2, 2
		};

	public new int[] moveFast = new int[7] { 1, 1, 2, 2, 3, 3, 2 };

	public new int[] attack1 = new int[12]
		{
			0, 0, 0, 7, 7, 7, 8, 8, 8, 9,
			9, 9
		};

	public new int[] attack2 = new int[12]
		{
			0, 0, 0, 10, 10, 10, 11, 11, 11, 12,
			12, 12
		};

	public int[] attack3 = new int[24]
		{
			0, 0, 1, 1, 4, 4, 6, 6, 8, 8,
			25, 25, 26, 26, 28, 28, 30, 30, 32, 32,
			2, 2, 1, 1
		};

	public int[] fly = new int[21]
		{
			4, 4, 4, 5, 5, 5, 6, 6, 6, 6,
			6, 6, 3, 3, 3, 2, 2, 2, 1, 1,
			1
		};

	public int[] hitground = new int[12]
		{
			6, 6, 6, 3, 3, 3, 2, 2, 2, 1,
			1, 1
		};

	private bool shock;

	private sbyte[] cou = new sbyte[2] { -1, 1 };

	public new Char injureBy;

	public new bool injureThenDie;

	public new Mob mobToAttack;

	public new int forceWait;

	public new bool blindEff;

	public new bool sleepEff;

	public BigBoss2(int id, short px, short py, int templateID, long hp, long maxHp, int s)
		{
			if (shadowBig == null)
			{
				shadowBig = GameCanvas.loadImage("/mainImage/shadowBig.png");
			}
			mobId = id;
			xTo = (x = px + 20);
			yTo = (y = py);
			yFirst = py;
			base.hp = hp;
			base.maxHp = maxHp;
			templateId = templateID;
			w_hp_bar = 100;
			h_hp_bar = 6;
			len = w_hp_bar;
			updateHp_bar();
			getDataB();
			status = 2;
		}

	public void getDataB()
		{
			data = null;
			data = new EffectData();
			string patch = "/x" + mGraphics.zoomLevel + "/effectdata/" + 109 + "/data";
			try
			{
				data.readData2(patch);
				data.img = GameCanvas.loadImage("/effectdata/" + 109 + "/img.png");
			}
			catch (Exception)
			{
				Service.gI().requestModTemplate(templateId);
			}
			w = data.width;
			h = data.height;
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

	private bool isSpecial()
		{
			if ((templateId >= 58 && templateId <= 65) || templateId == 67 || templateId == 68)
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

	public new void attackOtherMob(Mob mobToAttack)
		{
			this.mobToAttack = mobToAttack;
			isBusyAttackSomeOne = true;
			cFocus = null;
			p1 = 0;
			p2 = 0;
			status = 3;
			tick = 0;
			dir = ((mobToAttack.x > x) ? 1 : (-1));
			int num = mobToAttack.x;
			int num2 = mobToAttack.y;
			if (Res.abs(num - x) < w * 2 && Res.abs(num2 - y) < h * 2)
			{
				if (x < num)
				{
					x = num - w;
				}
				else
				{
					x = num + w;
				}
				p3 = 0;
			}
			else
			{
				p3 = 1;
			}
		}

	public new int getX()
		{
			return x;
		}

	public new int getY()
		{
			return y - 50;
		}

	public new int getH()
		{
			return 40;
		}

	public new int getW()
		{
			return 50;
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

	public new void removeHoldEff()
		{
			if (holdEffID != 0)
			{
				holdEffID = 0;
			}
		}

	public new void removeBlindEff()
		{
			blindEff = false;
		}

	public new void removeSleepEff()
		{
			sleepEff = false;
		}

}
