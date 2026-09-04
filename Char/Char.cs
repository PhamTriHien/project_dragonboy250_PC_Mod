using System;
using Assets.src.e;
using Assets.src.g;
using UnityEngine;

public partial class Char : IMapObject
{
	public string xuStr;

	public string luongStr;

	public string luongKhoaStr;

	public long lastUpdateTime;

	public bool meLive;

	public bool isMask;

	public bool isTeleport;

	public bool isUsePlane;

	public int shadowX;

	public int shadowY;

	public int shadowLife;

	public bool isNhapThe;

	public PetFollow petFollow;

	public int rank;

	public const sbyte A_STAND = 1;

	public const sbyte A_RUN = 2;

	public const sbyte A_JUMP = 3;

	public const sbyte A_FALL = 4;

	public const sbyte A_DEADFLY = 5;

	public const sbyte A_NOTHING = 6;

	public const sbyte A_ATTK = 7;

	public const sbyte A_INJURE = 8;

	public const sbyte A_AUTOJUMP = 9;

	public const sbyte A_FLY = 10;

	public const sbyte SKILL_STAND = 12;

	public const sbyte SKILL_FALL = 13;

	public const sbyte A_DEAD = 14;

	public const sbyte A_HIDE = 15;

	public const sbyte A_RESETPOINT = 16;

	public static ChatPopup chatPopup;

	public long cPower;

	public Info chatInfo;

	public sbyte petStatus;

	public int cx = 24;

	public int cy = 24;

	public int cvx;

	public int cvy;

	public int cp1;

	public int cp2;

	public int cp3;

	public int statusMe = 5;

	public int cdir = 1;

	public int charID;

	public int cgender;

	public int ctaskId;

	public int menuSelect;

	public int cBonusSpeed;

	public int cspeed = 4;

	public int cCriticalFull;

	public int cCritDameFull;

	public int clevel;

	public int xReload;

	public int yReload;

	public int cyStartFall;

	public int saveStatus;

	public int eff5BuffHp;

	public int eff5BuffMp;

	public int cdameDown;

	public int cStr;

	public long cMP;

	public long cHP;

	public long cHPNew;

	public long cHPShow;

	public long cHPFull;

	public long cMPFull;

	public long cDamFull;

	public long cDefull;

	public long cGiamST;

	public long cLevelPercent;

	public long cTiemNang;

	public long cNangdong;

	public long damHP;

	public long damMP;

	public bool isMob;

	public bool isCrit;

	public bool isDie;

	public int pointUydanh;

	public int pointNon;

	public int pointVukhi;

	public int pointAo;

	public int pointLien;

	public int pointGangtay;

	public int pointNhan;

	public int pointQuan;

	public int pointNgocboi;

	public int pointGiay;

	public int pointPhu;

	public int countFinishDay;

	public int countLoopBoos;

	public int limitTiemnangso;

	public int limitKynangso;

	public short[] potential = new short[4];

	public string cName = string.Empty;

	public int clanID;

	public sbyte ctypeClan;

	public Clan clan;

	public sbyte role;

	public int cw = 22;

	public int ch = 32;

	public int chw = 11;

	public int chh = 16;

	public Command cmdMenu;

	public bool canFly = true;

	public bool cmtoChar;

	public bool me;

	public bool cFinishedAttack;

	public bool cchistlast;

	public bool isAttack;

	public bool isAttFly;

	public int cwpt;

	public int cwplv;

	public int cf;

	public int tick;

	public static bool fallAttack;

	public bool isJump;

	public bool autoFall;

	public bool attack = true;

	public long xu;

	public int xuInBox;

	public int yen;

	public int gold_lock;

	public int luong;

	public int luongKhoa;

	public NClass nClass;

	public Command endMovePointCommand;

	public MyVector vSkill = new MyVector();

	public MyVector vSkillFight = new MyVector();

	public MyVector vEff = new MyVector();

	public Skill myskill;

	public Task taskMaint;

	public bool paintName = true;

	public Archivement[] arrArchive;

	public Item[] arrItemBag;

	public Item[] arrItemBox;

	public Item[] arrItemBody;

	public Skill[] arrPetSkill;

	public Item[][] arrItemShop;

	public string[][] infoSpeacialSkill;

	public short[][] imgSpeacialSkill;

	public short cResFire;

	public short cResIce;

	public short cResWind;

	public short cMiss;

	public short cExactly;

	public short cFatal;

	public sbyte cPk;

	public sbyte cTypePk;

	public short cReactDame;

	public short sysUp;

	public short sysDown;

	public int avatar;

	public int skillTemplateId;

	public Mob mobFocus;

	public Mob mobMe;

	public int tMobMeBorn;

	public Npc npcFocus;

	public Char charFocus;

	public ItemMap itemFocus;

	public MyVector focus = new MyVector();

	public Mob[] attMobs;

	public Char[] attChars;

	public short[] moveFast;

	public int testCharId = -9999;

	public int killCharId = -9999;

	public sbyte resultTest;

	public int countKill;

	public int countKillMax;

	public bool isInvisiblez;

	public bool isShadown = true;

	public const sbyte PK_NORMAL = 0;

	public const sbyte PK_PHE = 1;

	public const sbyte PK_BANG = 2;

	public const sbyte PK_THIDAU = 3;

	public const sbyte PK_LUYENTAP = 4;

	public const sbyte PK_TUDO = 5;

	public MyVector taskOrders = new MyVector();

	public int cStamina;

	public static short[] idHead;

	public static short[] idAvatar;

	public int exp;

	public string[] strLevel;

	public string currStrLevel;

	public static Image eyeTraiDat = GameCanvas.loadImage("/mainImage/myTexture2dmat-trai-dat.png");

	public static Image eyeNamek = GameCanvas.loadImage("/mainImage/myTexture2dmat-namek.png");

	public bool isFreez;

	public bool isCharge;

	public int seconds;

	public int freezSeconds;

	public long last;

	public long cur;

	public long lastFreez;

	public long currFreez;

	public bool isFlyUp;

	public static MyVector vItemTime = new MyVector();

	public static short ID_NEW_MOUNT = 30000;

	public short idMount;

	public bool isHaveMount;

	public bool isMountVip;

	public bool isEventMount;

	public bool isSpeacialMount;

	public static Image imgMount_TD = GameCanvas.loadImage("/mainImage/myTexture2dthucuoi10.png");

	public static Image imgMount_NM = GameCanvas.loadImage("/mainImage/myTexture2dthucuoi20.png");

	public static Image imgMount_NM_1 = GameCanvas.loadImage("/mainImage/myTexture2dthucuoi21.png");

	public static Image imgMount_XD = GameCanvas.loadImage("/mainImage/myTexture2dthucuoi30.png");

	public static Image imgMount_TD_VIP = GameCanvas.loadImage("/mainImage/myTexture2dthucuoi11.png");

	public static Image imgMount_NM_VIP = GameCanvas.loadImage("/mainImage/myTexture2dthucuoi22.png");

	public static Image imgMount_NM_1_VIP = GameCanvas.loadImage("/mainImage/myTexture2dthucuoi23.png");

	public static Image imgMount_XD_VIP = GameCanvas.loadImage("/mainImage/myTexture2dthucuoi31.png");

	public static Image imgEventMount = GameCanvas.loadImage("/mainImage/myTexture2drong.png");

	public static Image imgEventMountWing = GameCanvas.loadImage("/mainImage/myTexture2dcanhrong.png");

	public sbyte[] FrameMount = new sbyte[8] { 0, 0, 1, 1, 2, 2, 1, 1 };

	public int frameMount;

	public int frameNewMount;

	public int transMount;

	public int genderMount;

	public int idcharMount;

	public int xMount;

	public int yMount;

	public int dxMount;

	public int dyMount;

	public int xChar;

	public int xdis;

	public int speedMount;

	public bool isStartMount;

	public bool isMount;

	public bool isEndMount;

	public sbyte cFlag;

	public int flagImage;

	public short x_hint;

	public short y_hint;

	public short s_danhHieu1;

	public static int[][][] CharInfo = new int[33][][]
			{
				new int[4][]
				{
					new int[3] { 0, -13, 34 },
					new int[3] { 1, -8, 10 },
					new int[3] { 1, -9, 16 },
					new int[3] { 1, -9, 45 }
				},
				new int[4][]
				{
					new int[3] { 0, -13, 35 },
					new int[3] { 1, -8, 10 },
					new int[3] { 1, -9, 17 },
					new int[3] { 1, -9, 46 }
				},
				new int[4][]
				{
					new int[3] { 1, -10, 33 },
					new int[3] { 2, -10, 11 },
					new int[3] { 2, -8, 16 },
					new int[3] { 1, -12, 49 }
				},
				new int[4][]
				{
					new int[3] { 1, -10, 32 },
					new int[3] { 3, -12, 10 },
					new int[3] { 3, -11, 15 },
					new int[3] { 1, -13, 47 }
				},
				new int[4][]
				{
					new int[3] { 1, -10, 34 },
					new int[3] { 4, -8, 11 },
					new int[3] { 4, -7, 17 },
					new int[3] { 1, -12, 47 }
				},
				new int[4][]
				{
					new int[3] { 1, -10, 34 },
					new int[3] { 5, -12, 11 },
					new int[3] { 5, -9, 17 },
					new int[3] { 1, -13, 49 }
				},
				new int[4][]
				{
					new int[3] { 1, -10, 33 },
					new int[3] { 6, -10, 10 },
					new int[3] { 6, -8, 16 },
					new int[3] { 1, -12, 47 }
				},
				new int[4][]
				{
					new int[3] { 0, -9, 36 },
					new int[3] { 7, -5, 17 },
					new int[3] { 7, -11, 25 },
					new int[3] { 1, -8, 49 }
				},
				new int[4][]
				{
					new int[3] { 0, -7, 35 },
					new int[3] { 0, -18, 22 },
					new int[3] { 7, -10, 25 },
					new int[3] { 1, -7, 48 }
				},
				new int[4][]
				{
					new int[3] { 1, -11, 35 },
					new int[3] { 10, -3, 25 },
					new int[3] { 12, -10, 26 },
					new int[3]
				},
				new int[4][]
				{
					new int[3] { 1, -11, 37 },
					new int[3] { 11, -3, 25 },
					new int[3] { 12, -11, 27 },
					new int[3]
				},
				new int[4][]
				{
					new int[3] { 0, -14, 34 },
					new int[3] { 12, -8, 21 },
					new int[3] { 9, -7, 31 },
					new int[3]
				},
				new int[4][]
				{
					new int[3] { 0, -12, 35 },
					new int[3] { 8, -5, 14 },
					new int[3] { 8, -15, 29 },
					new int[3] { 1, -9, 49 }
				},
				new int[4][]
				{
					new int[3] { 1, -9, 34 },
					new int[3] { 9, -12, 9 },
					new int[3] { 10, -7, 19 },
					new int[3]
				},
				new int[4][]
				{
					new int[3] { 1, -13, 34 },
					new int[3] { 9, -12, 9 },
					new int[3] { 11, -10, 19 },
					new int[3]
				},
				new int[4][]
				{
					new int[3] { 1, -8, 32 },
					new int[3] { 9, -12, 9 },
					new int[3] { 2, -6, 15 },
					new int[3]
				},
				new int[4][]
				{
					new int[3] { 1, -8, 32 },
					new int[3] { 9, -12, 9 },
					new int[3] { 13, -12, 16 },
					new int[3]
				},
				new int[4][]
				{
					new int[3] { 0, -10, 31 },
					new int[3] { 9, -12, 9 },
					new int[3] { 7, -13, 20 },
					new int[3]
				},
				new int[4][]
				{
					new int[3] { 0, -11, 32 },
					new int[3] { 9, -12, 9 },
					new int[3] { 8, -15, 26 },
					new int[3]
				},
				new int[4][]
				{
					new int[3] { 0, -9, 33 },
					new int[3] { 9, -12, 9 },
					new int[3] { 14, -8, 18 },
					new int[3]
				},
				new int[4][]
				{
					new int[3] { 0, -11, 33 },
					new int[3] { 9, -12, 9 },
					new int[3] { 15, -6, 19 },
					new int[3]
				},
				new int[4][]
				{
					new int[3] { 0, -16, 31 },
					new int[3] { 9, -12, 9 },
					new int[3] { 9, -8, 28 },
					new int[3]
				},
				new int[4][]
				{
					new int[3] { 0, -14, 34 },
					new int[3] { 1, -8, 10 },
					new int[3] { 8, -16, 28 },
					new int[3]
				},
				new int[4][]
				{
					new int[3] { 0, -8, 36 },
					new int[3] { 7, -5, 17 },
					new int[3] { 0, -5, 25 },
					new int[3]
				},
				new int[4][]
				{
					new int[3] { 0, -9, 31 },
					new int[3] { 9, -12, 9 },
					new int[3] { 0, -6, 20 },
					new int[3]
				},
				new int[4][]
				{
					new int[3] { 2, -9, 36 },
					new int[3] { 13, -5, 17 },
					new int[3] { 16, -11, 25 },
					new int[3]
				},
				new int[4][]
				{
					new int[3] { 1, -9, 34 },
					new int[3] { 8, -5, 13 },
					new int[3] { 10, -7, 19 },
					new int[3]
				},
				new int[4][]
				{
					new int[3] { 1, -13, 34 },
					new int[3] { 8, -5, 13 },
					new int[3] { 11, -10, 19 },
					new int[3]
				},
				new int[4][]
				{
					new int[3] { 1, -8, 32 },
					new int[3] { 8, -5, 13 },
					new int[3] { 2, -6, 15 },
					new int[3]
				},
				new int[4][]
				{
					new int[3] { 1, -8, 32 },
					new int[3] { 8, -5, 13 },
					new int[3] { 13, -12, 16 },
					new int[3]
				},
				new int[4][]
				{
					new int[3] { 0, -9, 33 },
					new int[3] { 8, -5, 13 },
					new int[3] { 14, -8, 18 },
					new int[3]
				},
				new int[4][]
				{
					new int[3] { 0, -11, 33 },
					new int[3] { 8, -5, 13 },
					new int[3] { 15, -6, 19 },
					new int[3]
				},
				new int[4][]
				{
					new int[3] { 0, -16, 32 },
					new int[3] { 8, -5, 13 },
					new int[3] { 9, -8, 29 },
					new int[3]
				}
			};

	public static int[] CHAR_WEAPONX = new int[11]
			{
				-2, -6, 22, 21, 19, 22, 10, -2, -2, 5,
				19
			};

	public static int[] CHAR_WEAPONY = new int[11]
			{
				9, 22, 25, 17, 26, 37, 36, 49, 50, 52,
				36
			};

	private static Char myChar;

	private static Char myPet;

	public static int[] listAttack;

	public static int[][] listIonC;

	public int cvyJump;

	private int indexUseSkill = -1;

	public int cxSend;

	public int cySend;

	public int cdirSend = 1;

	public int cxFocus;

	public int cyFocus;

	public int cactFirst = 5;

	public MyVector vMovePoints = new MyVector();

	public static string[][] inforClass = new string[2][]
			{
				new string[4] { "1", "1", "chiêu 1", "0" },
				new string[4] { "2", "2", "chiêu 2", "5" }
			};

	public static int[][] inforSkill = new int[10][]
			{
				new int[12]
				{
					1, 0, 1, 1000, 40, 1, 0, 20, 0, 0,
					0, 0
				},
				new int[12]
				{
					2, 1, 10, 1000, 100, 1, 0, 40, 0, 0,
					0, 0
				},
				new int[12]
				{
					2, 2, 11, 800, 100, 1, 0, 45, 0, 0,
					0, 0
				},
				new int[12]
				{
					2, 3, 12, 600, 100, 1, 0, 50, 0, 0,
					0, 0
				},
				new int[12]
				{
					2, 4, 13, 500, 100, 1, 0, 55, 0, 0,
					0, 0
				},
				new int[12]
				{
					3, 1, 14, 500, 100, 1, 0, 60, 0, 0,
					0, 0
				},
				new int[12]
				{
					3, 2, 14, 500, 100, 1, 0, 60, 0, 0,
					0, 0
				},
				new int[12]
				{
					3, 3, 14, 500, 100, 1, 0, 60, 0, 0,
					0, 0
				},
				new int[12]
				{
					3, 4, 14, 500, 100, 1, 0, 60, 0, 0,
					0, 0
				},
				new int[12]
				{
					3, 5, 14, 500, 100, 1, 0, 60, 0, 0,
					0, 0
				}
			};

	public static bool flag;

	public static bool ischangingMap;

	public static bool isLockKey;

	public static bool isLoadingMap;

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

	public Char()
			{
				statusMe = 6;
			}

	public static void taskAction(bool isNextStep)
			{
				Task task = myCharz().taskMaint;
				if (task.index > task.contentInfo.Length - 1)
				{
					task.index = task.contentInfo.Length - 1;
				}
				string text = task.contentInfo[task.index];
				if (text != null && !text.Equals(string.Empty))
				{
					if (text.StartsWith("#"))
					{
						text = NinjaUtil.replace(text, "#", string.Empty);
						Npc npc = new Npc(5, 0, -100, -100, 5, GameScr.info1.charId[myCharz().cgender][2]);
						npc.cx = (npc.cy = -100);
						npc.avatar = GameScr.info1.charId[myCharz().cgender][2];
						npc.charID = 5;
						if (GameCanvas.currentScreen == GameScr.instance)
						{
							ChatPopup.addNextPopUpMultiLine(text, npc);
						}
					}
					else if (isNextStep)
					{
						GameScr.info1.addInfo(text, 0);
					}
				}
				GameScr.isHaveSelectSkill = true;
				Cout.println("TASKx " + myCharz().taskMaint.taskId);
				if (myCharz().taskMaint.taskId <= 2)
				{
					myCharz().canFly = false;
				}
				else
				{
					myCharz().canFly = true;
				}
				GameScr.gI().left = null;
				if (task.taskId == 0)
				{
					Hint.isViewMap = false;
					Hint.isViewPotential = false;
					GameScr.gI().right = null;
					GameScr.isHaveSelectSkill = false;
					GameScr.gI().left = null;
					if (task.index < 4)
					{
						MagicTree.isPaint = false;
						GameScr.isPaintRada = -1;
					}
					if (task.index == 4)
					{
						GameScr.isPaintRada = 1;
						MagicTree.isPaint = true;
					}
					if (task.index >= 5)
					{
						GameScr.gI().right = GameScr.gI().cmdFocus;
					}
				}
				if (task.taskId == 1)
				{
					GameScr.isHaveSelectSkill = true;
				}
				if (task.taskId >= 1)
				{
					GameScr.gI().right = GameScr.gI().cmdFocus;
					GameScr.gI().left = GameScr.gI().cmdMenu;
				}
				if (task.taskId >= 0)
				{
					Panel.isPaintMap = true;
				}
				else
				{
					Panel.isPaintMap = false;
				}
				if (task.taskId < 12)
				{
					GameCanvas.panel.mainTabName = mResources.mainTab1;
				}
				else
				{
					GameCanvas.panel.mainTabName = mResources.mainTab2;
				}
				GameCanvas.panel.tabName[0] = GameCanvas.panel.mainTabName;
				if (myChar.taskMaint.taskId > 10)
				{
					Rms.saveRMSString("fake", "aa");
				}
			}

	public int avatarz()
			{
				return getAvatar(head);
			}

	public void addInfo(string info)
			{
				if (chatInfo == null)
				{
					chatInfo = new Info();
				}
				Char cInfo = null;
				chatInfo.addInfo(info, 0, cInfo, isChatServer: false);
			}

	public static Char myCharz()
			{
				if (myChar == null)
				{
					myChar = new Char();
					myChar.me = true;
					myChar.cmtoChar = true;
				}
				return myChar;
			}

	public static Char myPetz()
			{
				if (myPet == null)
				{
					myPet = new Char();
					myPet.me = false;
				}
				return myPet;
			}

	public static void clearMyChar()
			{
				myChar = null;
			}


	public Waypoint isInEnterOfflinePoint()
			{
				Task task = myChar.taskMaint;
				if (task != null && task.taskId == 0 && task.index < 6)
				{
					return null;
				}
				int num = TileMap.vGo.size();
				for (sbyte b = 0; b < num; b++)
				{
					Waypoint waypoint = (Waypoint)TileMap.vGo.elementAt(b);
					if (PopUp.vPopups.size() >= num)
					{
						PopUp popUp = (PopUp)PopUp.vPopups.elementAt(b);
						if (!popUp.isPaint)
						{
							return null;
						}
					}
					if (cx >= waypoint.minX && cx <= waypoint.maxX && cy >= waypoint.minY && cy <= waypoint.maxY && waypoint.isEnter && waypoint.isOffline)
					{
						return waypoint;
					}
				}
				return null;
			}

	public Waypoint isInEnterOnlinePoint()
			{
				Task task = myChar.taskMaint;
				if (task != null && task.taskId == 0 && task.index < 6)
				{
					return null;
				}
				int num = TileMap.vGo.size();
				for (sbyte b = 0; b < num; b++)
				{
					Waypoint waypoint = (Waypoint)TileMap.vGo.elementAt(b);
					if (PopUp.vPopups.size() >= num)
					{
						PopUp popUp = (PopUp)PopUp.vPopups.elementAt(b);
						if (!popUp.isPaint)
						{
							return null;
						}
					}
					if (cx >= waypoint.minX && cx <= waypoint.maxX && cy >= waypoint.minY && cy <= waypoint.maxY && waypoint.isEnter && !waypoint.isOffline)
					{
						return waypoint;
					}
				}
				return null;
			}


	public void hide()
			{
				isHide = true;
				EffecMn.addEff(new Effect(107, cx, cy + 25, 3, 15, 1));
			}

	public void show()
			{
				isHide = false;
				EffecMn.addEff(new Effect(107, cx, cy + 25, 3, 10, 1));
			}

	public int returnAct(int xFirst, int yFirst, int xEnd, int yEnd)
			{
				int num = xEnd - xFirst;
				int num2 = yEnd - yFirst;
				if (num == 0 && num2 == 0)
				{
					return 1;
				}
				if (num2 == 0 && yFirst % 24 == 0 && TileMap.tileTypeAt(xFirst, yFirst, 2))
				{
					return 2;
				}
				if (num2 > 0 && (yFirst % 24 != 0 || !TileMap.tileTypeAt(xFirst, yFirst, 2)))
				{
					return 4;
				}
				cvy = -10;
				cp1 = 0;
				cdir = ((num > 0) ? 1 : (-1));
				if (num <= 5)
				{
					cvx = 0;
				}
				else if (num <= 10)
				{
					cvx = 3;
				}
				else
				{
					cvx = 5;
				}
				return 9;
			}

	public float getSoundVolumn()
			{
				if (me)
				{
					return 0.1f;
				}
				int num = Res.abs(myChar.cx - cx);
				if (num >= 0 && num <= 50)
				{
					return 0.1f;
				}
				return 0.05f;
			}

	private void stop()
			{
				statusMe = 6;
				cp3 = 0;
				cvx = 0;
				cvy = 0;
				cp1 = (cp2 = 0);
				if (me && (cx != cxSend || cy != cySend))
				{
					Service.gI().charMove();
				}
			}

	public static int abs(int i)
			{
				return (i <= 0) ? (-i) : i;
			}

	public SkillInfoPaint[] skillInfoPaint()
			{
				if (skillPaint == null)
				{
					return null;
				}
				if (skillPaintRandomPaint == null)
				{
					return null;
				}
				if (sType == 0)
				{
					return skillPaintRandomPaint.skillStand;
				}
				return skillPaintRandomPaint.skillfly;
			}

	public virtual void paint(mGraphics g)
			{
				if (isHide)
				{
					return;
				}
				if (isMafuba)
				{
					paintCharWithoutSkill(g);
				}
				else if (isMabuHold)
				{
					if (cmtoChar)
					{
						GameScr.cmtoX = cx - GameScr.gW2;
						GameScr.cmtoY = cy - GameScr.gH23;
						if (!GameCanvas.isTouchControl)
						{
							GameScr.cmtoX += GameScr.gW6 * cdir;
						}
					}
				}
				else
				{
					if (!isPaint() || (!me && GameScr.notPaint))
					{
						return;
					}
					if (petFollow != null)
					{
						petFollow.paint(g);
					}
					paintMount1(g);
					if ((TileMap.isInAirMap() && cy >= TileMap.pxh - 48) || isTeleport)
					{
						return;
					}
					if (holder && GameCanvas.gameTick % 2 == 0)
					{
						g.setColor(16185600);
						if (charHold != null)
						{
							g.drawLine(cx, cy - ch / 2, charHold.cx, charHold.cy - charHold.ch / 2);
						}
						if (mobHold != null)
						{
							g.drawLine(cx, cy - ch / 2, mobHold.x, mobHold.y - mobHold.h / 2);
						}
					}
					paintSuperEffBehind(g);
					paintAuraBehind(g);
					paintEffBehind(g);
					paintEff_Lvup_behind(g);
					paintEff_Pet(g);
					if (shadowLife > 0)
					{
						if (GameCanvas.gameTick % 2 == 0)
						{
							paintCharBody(g, shadowX, shadowY, cdir, 25, isPaintBag: true);
						}
						else if (shadowLife > 5)
						{
							paintCharBody(g, shadowX, shadowY, cdir, 7, isPaintBag: true);
						}
					}
					if (!isPaint() && skillPaint != null && (skillPaint.id < 70 || skillPaint.id > 76) && (skillPaint.id < 77 || skillPaint.id > 83))
					{
						if (skillPaint != null)
						{
							indexSkill = skillInfoPaint().Length;
							skillPaint = null;
						}
						effPaints = null;
						eff = null;
						effTask = null;
						indexEff = -1;
						indexEffTask = -1;
					}
					else if (statusMe != 15 && (moveFast == null || moveFast[0] <= 0))
					{
						paintCharName_HP_MP_Overhead(g);
						if (skillPaint == null || skillInfoPaint() == null || indexSkill >= skillInfoPaint().Length)
						{
							paintCharWithoutSkill(g);
						}
						if (arr != null)
						{
							arr.paint(g);
						}
						if (dart != null)
						{
							dart.paint(g);
						}
						paintEffect(g);
						if (mobMe != null)
						{
						}
						paintMount2(g);
						paintEff_Lvup_front(g);
						paintSuperEffFront(g);
						paintAuraFront(g);
						paintEffFront(g);
						paint_map_line(g);
					}
				}
			}


	public void liveFromDead()
			{
				cHP = cHPFull;
				cMP = cMPFull;
				statusMe = 1;
				cp1 = (cp2 = (cp3 = 0));
				ServerEffect.addServerEffect(109, this, 2);
				GameScr.gI().center = null;
				GameScr.isHaveSelectSkill = true;
			}


	public void stopMoving()
			{
			}

	public Effect getEffById(int id)
			{
				for (int i = 0; i < vEffChar.size(); i++)
				{
					Effect effect = (Effect)vEffChar.elementAt(i);
					if (effect.effId == id)
					{
						return effect;
					}
				}
				return null;
			}


	public void printlog()
			{
				string empty = string.Empty;
				string text = empty;
				empty = text + "isInjure " + isInjure + "\n";
				text = empty;
				empty = text + "isInjure " + isMonkey + "\n";
				text = empty;
				empty = text + "isInjure " + isAddChopMat + "\n";
				text = empty;
				empty = text + "isInjure " + isAttack + "\n";
				text = empty;
				empty = text + "isInjure " + isAttFly + "\n";
				text = empty;
				empty = text + "isInjure " + ischangingMap + "\n";
				text = empty;
				empty = text + "isInjure " + isCharge + "\n";
				text = empty;
				empty = text + "isInjure " + isCopy + "\n";
				text = empty;
				empty = text + "isInjure " + isCreateDark + "\n";
				text = empty;
				empty = text + "isInjure " + isCrit + "\n";
				text = empty;
				empty = text + "isInjure " + isDirtyPostion + "\n";
				text = empty;
				empty = text + "isInjure " + isEndMount + "\n";
				text = empty;
				empty = text + "isInjure " + isEventMount + "\n";
				text = empty;
				empty = text + "isInjure " + isMafuba + "\n";
				text = empty;
				empty = text + "isInjure " + isFusion + "\n";
				text = empty;
				empty = text + "isInjure " + isFeetEff + "\n";
				text = empty;
				empty = text + "isInjure " + isFlying + "\n";
				text = empty;
				empty = text + "isInjure " + isWaitMonkey + "\n";
				text = empty;
				empty = text + "isInjure " + isUseSkillSpec() + "\n";
				text = empty;
				empty = text + "isInjure " + isDie + "\n";
				text = empty;
				empty = text + "isInjure " + isDie + "\n";
				text = empty;
				empty = text + "isInjure " + isDie + "\n";
				text = empty;
				empty = text + "isInjure " + isDie + "\n";
				Res.outz(empty);
			}

}
