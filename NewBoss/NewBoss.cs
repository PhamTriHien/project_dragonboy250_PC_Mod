using System;

public partial class NewBoss : Mob, IMapObject
{
	public static Image shadowBig = mSystem.loadImage("/mainImage/shadowBig.png");

	public int xTo;

	public int yTo;

	public bool haftBody;

	public bool change;

	public new int xSd;

	public new int ySd;

	private int wCount;

	public new bool isShadown = true;

	private int tick;

	private int frame;

	public new static Image imgHP = mSystem.loadImage("/mainImage/myTexture2dmobHP.png");

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

	private int ff;

	private int offset;

	private int xTempRight = -1;

	private int xTempLeft = -1;

	private int yTemp = -1;

	private sbyte[] cou = new sbyte[2] { -1, 1 };

	public new Char injureBy;

	public new bool injureThenDie;

	public new Mob mobToAttack;

	public new int forceWait;

	public new bool blindEff;

	public new bool sleepEff;

	private int[][] frameArr = new int[17][]
		{
			new int[8] { 0, 0, 0, 0, 1, 1, 1, 1 },
			new int[8] { 0, 0, 0, 0, 1, 1, 1, 1 },
			new int[8] { 0, 0, 0, 0, 1, 1, 1, 1 },
			new int[8] { 0, 0, 0, 0, 1, 1, 1, 1 },
			new int[8] { 0, 0, 0, 0, 1, 1, 1, 1 },
			new int[8] { 0, 0, 0, 0, 1, 1, 1, 1 },
			new int[8] { 0, 0, 0, 0, 1, 1, 1, 1 },
			new int[8] { 0, 0, 0, 0, 1, 1, 1, 1 },
			new int[8] { 0, 0, 0, 0, 1, 1, 1, 1 },
			new int[8] { 0, 0, 0, 0, 1, 1, 1, 1 },
			new int[8] { 0, 0, 0, 0, 1, 1, 1, 1 },
			new int[8] { 0, 0, 0, 0, 1, 1, 1, 1 },
			new int[8] { 0, 0, 0, 0, 1, 1, 1, 1 },
			new int[8] { 0, 0, 0, 0, 1, 1, 1, 1 },
			new int[8] { 0, 0, 0, 0, 1, 1, 1, 1 },
			new int[8] { 0, 0, 0, 0, 1, 1, 1, 1 },
			new int[8] { 0, 0, 0, 0, 1, 1, 1, 1 }
		};

	public new const sbyte stand = 0;

	public const sbyte moveFra = 1;

	public new const sbyte attack1 = 2;

	public new const sbyte attack2 = 3;

	public const sbyte attack3 = 4;

	public const sbyte attack4 = 5;

	public const sbyte attack5 = 6;

	public const sbyte attack6 = 7;

	public const sbyte attack7 = 8;

	public const sbyte attack8 = 9;

	public const sbyte attack9 = 10;

	public const sbyte attack10 = 11;

	public new const sbyte hurt = 12;

	public const sbyte die = 13;

	public const sbyte fly = 14;

	public const sbyte adddame = 15;

	public const sbyte typeEff = 16;

	public NewBoss(int id, short px, short py, int templateID, long hp, long maxHp, int s)
		{
			mobId = id;
			x = (xFirst = px + 20);
			y = (yFirst = py);
			xTo = x;
			yTo = y;
			base.maxHp = maxHp;
			base.hp = hp;
			templateId = templateID;
			h_hp_bar = 6;
			w_hp_bar = 100;
			len = w_hp_bar;
			updateHp_bar();
			if (Mob.arrMobTemplate[templateId].data == null)
			{
				Service.gI().requestModTemplate(templateId);
			}
			status = 2;
			frameArr = null;
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

	public new int getHPColor()
		{
			return 16711680;
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
			return y;
		}

	public new int getH()
		{
			return h;
		}

	public new int getW()
		{
			return w;
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

	public new void move(short xMoveTo, short yMoveTo)
		{
			if (yMoveTo != -1)
			{
				if (Res.distance(x, y, xTo, yTo) > 100)
				{
					x = xMoveTo;
					y = yMoveTo;
					status = 2;
				}
				else
				{
					xTo = xMoveTo;
					yTo = yMoveTo;
					status = 5;
				}
			}
			else
			{
				xTo = xMoveTo;
				status = 5;
			}
		}

	public new void GetFrame()
		{
			try
			{
				frameArr = (int[][])Controller.frameHT_NEWBOSS.get(templateId + string.Empty);
				w = Mob.arrMobTemplate[templateId].data.width;
				h = Mob.arrMobTemplate[templateId].data.height;
			}
			catch (Exception)
			{
			}
		}

}
