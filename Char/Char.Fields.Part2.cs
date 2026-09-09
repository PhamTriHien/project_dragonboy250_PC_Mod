using System;
using Assets.src.e;
using Assets.src.g;
using UnityEngine;

public partial class Char : IMapObject
{

	public static bool flag;

	public static bool ischangingMap;

	public static bool isLockKey;

	public static bool isLoadingMap;

	public static long lastMapChangeTime;

	public static Waypoint entranceWaypoint;

	public bool isLockMove;

	public bool isLockAttack;

	public string strInfo;

	public short powerPoint;

	public short maxPowerPoint;

	public short secondPower;

	public long lastS;

	public long currS;

	public const int C_XAYDA_2 = 2;

	public const int C_NAMEC_1 = 1;

	public const int C_TRAIDAT_0 = 0;

	public bool havePet = true;

	public MovePoint currentMovePoint;

	public int bom;

	public int delayFall;

	private bool isSoundJump;

	public int lastFrame;

	private Effect eProtect;

	private Effect eDanhHieu;

	private int twHp;

	public bool isInjureHp;

	public bool changePos;

	public bool isHide;

	private int count;

	private bool wy;

	public int wt;

	public int fy;

	public int ty;

	private int t;

	private int fM;

	public int[] move = new int[15]
			{
				1, 1, 1, 1, 2, 2, 2, 2, 3, 3,
				3, 3, 2, 2, 2
			};

	private string strMount = "mount_";

	public int headICON = -1;

	public int head;

	public int leg;

	public int body;

	public int bag;

	public int wp;

	public int indexEff = -1;

	public int indexEffTask = -1;

	public EffectCharPaint eff;

	public EffectCharPaint effTask;

	public int indexSkill;

	public int i0;

	public int i1;

	public int i2;

	public int dx0;

	public int dx1;

	public int dx2;

	public int dy0;

	public int dy1;

	public int dy2;

	public EffectCharPaint eff0;

	public EffectCharPaint eff1;

	public EffectCharPaint eff2;

	public Arrow arr;

	public PlayerDart dart;

	public bool isCreateDark;

	public SkillPaint skillPaint;

	public SkillPaint skillPaintRandomPaint;

	public EffectPaint[] effPaints;

	public int sType;

	public sbyte isInjure;

	public bool isUseSkillAfterCharge;

	public bool isFlyAndCharge;

	public bool isStandAndCharge;

	private bool isFlying;

	public int posDisY;

	private int chargeCount;

	public bool hasSendAttack;

	public bool isMabuHold;

	private long timeBlue;

	private int tBlue;

	private bool IsAddDust1;

	private bool IsAddDust2;

	public int len = 24;

	public int w_hp_bar = 24;

	private int per = 100;

	private int per_tem = 100;

	private Image imgHPtem;

	private bool isPet;

	private bool isMiniPet;

	private int iiii;

	private int danhHieuFramme;

	public int xSd;

	public int ySd;

	private bool isOutMap;

	private int fBag;

	private Part ph;

	private Part pl;

	private Part pb;

	public int cH_new = 32;

	private int statusBeforeNothing;

	private int timeFocusToMob;

	public static bool isManualFocus = false;

	private Char charHold;

	private Mob mobHold;

	private int nInjure;

	public short wdx;

	public short wdy;

	public bool isDirtyPostion;

	public Skill lastNormalSkill;

	public bool currentFireByShortcut;

	public int cDamGoc;

	public int cHPGoc;

	public int cMPGoc;

	public int cDefGoc;

	public int cCriticalGoc;

	public sbyte hpFrom1000TiemNang;

	public sbyte mpFrom1000TiemNang;

	public sbyte damFrom1000TiemNang;

	public sbyte defFrom1000TiemNang = 1;

	public sbyte criticalFrom1000Tiemnang = 1;

	public short cMaxStamina;

	public short expForOneAdd;

	public sbyte isMonkey;

	public bool isCopy;

	public bool isWaitMonkey;

	private bool isFeetEff;

	public bool meDead;

	public int holdEffID;

	public bool holder;

	public bool protectEff;

	public bool danhHieuEff = true;

	private bool isSetPos;

	private int tpos;

	private short xPos;

	private short yPos;

	private sbyte typePos;

	private bool isMyFusion;

	public bool isFusion;

	public int tFusion;

	public bool huytSao;

	public bool blindEff;

	public bool telePortSkill;

	public bool sleepEff;

	public bool stone;

	public int perCentMp = 100;

	public long dHP;

	public int headTemp = -1;

	public int bodyTemp = -1;

	public int legTemp = -1;

	public int bagTemp = -1;

	public int wpTemp = -1;

	public MyVector vEffChar = new MyVector("vEff");

	public static FrameImage fraRedEye;

	private int fChopmat;

	private bool isAddChopMat;

	private long timeAddChopmat;

	private int[] frChopNhanh = new int[34]
			{
				-1, -1, -1, -1, 0, 0, 1, 1, 0, 0,
				1, 1, 0, 0, 1, 1, 0, 0, 1, 1,
				0, 0, 1, 1, 0, 0, 1, 1, 0, 0,
				-1, -1, -1, -1
			};

	private int[] frChopCham = new int[23]
			{
				-1, -1, -1, -1, 0, 0, 1, 1, 1, 0,
				0, 1, 1, 1, 0, 0, 1, 1, 1, -1,
				-1, -1, -1
			};

	private int[] frEye = new int[30]
			{
				-1, -1, 0, 0, 1, 1, 0, 0, 1, 1,
				0, 0, 1, 1, 0, 0, 1, 1, 0, 0,
				1, 1, 0, 0, 1, 1, 0, 0, -1, -1
			};

	public static int[][] Arr_Head_2Fr = new int[1][] { new int[2] { 542, 543 } };

	private int fHead;

	private string strEffAura = "aura_";

	public short idAuraEff = -1;

	public static bool isPaintAura = true;

	public static bool isPaintAura2 = true;

	private FrameImage fraEff;

	private FrameImage fraEffSub;

	private string strEff_Set_Item = "set_eff_";

	public short idEff_Set_Item = -1;

	private FrameImage fraHat_behind;

	private FrameImage fraHat_font;

	private FrameImage fraHat_behind_2;

	private FrameImage fraHat_font_2;

	private string strHat_behind = "hat_sau_";

	private string strHat_font = "hat_truoc_";

	private string strNgang = "ngang_";

	public short idHat = -1;

	public static int[][] hatInfo = new int[32][]
			{
				new int[2] { 5, -7 },
				new int[2] { 5, -7 },
				new int[2] { 5, -8 },
				new int[2] { 5, -7 },
				new int[2] { 5, -6 },
				new int[2] { 5, -8 },
				new int[2] { 5, -7 },
				new int[2] { 9, 0 },
				new int[2] { 11, 1 },
				new int[2] { 4, 0 },
				new int[2] { 4, -1 },
				new int[2] { 4, 8 },
				new int[2] { 6, 5 },
				new int[2] { 6, -6 },
				new int[2] { 2, -5 },
				new int[2] { 7, -8 },
				new int[2] { 7, -6 },
				new int[2] { 8, 0 },
				new int[2] { 7, 5 },
				new int[2] { 9, -7 },
				new int[2] { 7, -3 },
				new int[2] { 2, 8 },
				new int[2] { 4, 5 },
				new int[2] { 10, -5 },
				new int[2] { 9, -5 },
				new int[2] { 9, -5 },
				new int[2] { 6, -6 },
				new int[2] { 2, -5 },
				new int[2] { 7, -8 },
				new int[2] { 7, -6 },
				new int[2] { 9, -7 },
				new int[2] { 7, -3 }
			};

	public static short[] Arr_Head_FlyMove = new short[0];

	public const byte TYPE_SKILL_KAMEX10 = 1;

	public const byte TYPE_SKILL_FINAL = 2;

	public const byte TYPE_SKILL_MAFUBA = 3;

	public const byte TYPE_SKILL_GENKI = 4;

	public bool isPaintNewSkill;

	private bool isFly;

	private long timeReset_newSkill;

	private sbyte typeFrame;

	private short idskillPaint;

	private byte[] fr_start;

	private byte[] fr_atk;

	private byte[] fr_end;

	private int count_NEW;

	private int stt;

	private short rangeDame;

	private sbyte typePaint;

	private sbyte typeItem;

	private Point targetDame;

	private long timeDame;

	public bool isMafuba;

	private short countMafuba;

	public int xMFB;

	public int yMFB;

	public int timeGongSkill;

	private FrameImage fraDanhHieu;

	private MainImage mainImg;




























}
