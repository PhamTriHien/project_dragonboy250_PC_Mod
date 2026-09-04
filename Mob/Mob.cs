using System;
using Assets.src.g;
public partial class Mob
{
	public const sbyte TYPE_DUNG = 0;

	public const sbyte TYPE_DI = 1;

	public const sbyte TYPE_NHAY = 2;

	public const sbyte TYPE_LET = 3;

	public const sbyte TYPE_BAY = 4;

	public const sbyte TYPE_BAY_DAU = 5;

	public static MobTemplate[] arrMobTemplate;

	public const sbyte MA_INHELL = 0;

	public const sbyte MA_DEADFLY = 1;

	public const sbyte MA_STANDWAIT = 2;

	public const sbyte MA_ATTACK = 3;

	public const sbyte MA_STANDFLY = 4;

	public const sbyte MA_WALK = 5;

	public const sbyte MA_FALL = 6;

	public const sbyte MA_INJURE = 7;

	public bool changBody;

	public short smallBody;

	public bool isHintFocus;

	public string flystring;

	public int flyx;

	public int flyy;

	public int flyIndex;

	public bool isFreez;

	public int seconds;

	public long last;

	public long cur;

	public int holdEffID;

	public long hp;

	public long maxHp;

	public long hpInjure;

	public int x;

	public int y;

	public int dir = 1;

	public int dirV = 1;

	public int status;

	public int p1;

	public int p2;

	public int p3;

	public int xFirst;

	public int yFirst;

	public int vy;

	public int exp;

	public int w;

	public int h;

	public int charIndex;

	public int timeStatus;

	public int mobId;

	public bool isx;

	public bool isy;

	public bool isDisable;

	public bool isDontMove;

	public bool isFire;

	public bool isIce;

	public bool isWind;

	public bool isDie;

	public MyVector vMobMove = new MyVector();

	public bool isGo;

	public string mobName;

	public int templateId;

	public short pointx;

	public short pointy;

	public Char cFocus;

	public long dame;

	public long dameMp;

	public int sys;

	public sbyte levelBoss;

	public sbyte level;

	public bool isBoss;

	public bool isMobMe;

	public static MyVector lastMob = new MyVector();

	public static MyVector newMob = new MyVector();

	public bool isMafuba;

	public int xMFB;

	public int yMFB;

	public int xSd;

	public int ySd;

	private bool isOutMap;

	private int wCount;

	public bool isShadown = true;

	private int tick;

	private int frame;

	public static Image imgHP = GameCanvas.loadImage("/mainImage/myTexture2dmobHP.png");

	private bool wy;

	private int wt;

	private int fy;

	private int ty;

	public int typeSuperEff;

	public bool isBusyAttackSomeOne = true;

	public int[] stand = new int[12]
			{
				0, 0, 0, 0, 0, 0, 0, 0, 1, 1,
				1, 1
			};

	public int[] move = new int[15]
			{
				1, 1, 1, 1, 2, 2, 2, 2, 3, 3,
				3, 3, 2, 2, 2
			};

	public int[] moveFast = new int[7] { 1, 1, 2, 2, 3, 3, 2 };

	public int[] attack1 = new int[3] { 4, 5, 6 };

	public int[] attack2 = new int[3] { 7, 8, 9 };

	public int[] hurt = new int[1];

	private int color = 8421504;

	public int len = 24;

	public int w_hp_bar = 24;

	public int per = 100;

	public int per_tem = 100;

	public byte h_hp_bar = 4;

	public Image imgHPtem;

	private int offset;

	public bool isHide;

	private sbyte[] cou = new sbyte[2] { -1, 1 };

	public Char injureBy;

	public bool injureThenDie;

	public Mob mobToAttack;

	public int forceWait;

	public bool blindEff;

	public bool sleepEff;

	private int[][] frameArr = new int[6][]
			{
				new int[8] { 0, 0, 0, 0, 1, 1, 1, 1 },
				new int[8] { 0, 0, 0, 0, 1, 1, 1, 1 },
				new int[8] { 0, 0, 0, 0, 1, 1, 1, 1 },
				new int[8] { 0, 0, 0, 0, 1, 1, 1, 1 },
				new int[8] { 0, 0, 0, 0, 1, 1, 1, 1 },
				new int[8] { 0, 0, 0, 0, 1, 1, 1, 1 }
			};

	private bool isGetFr = true;
























}
