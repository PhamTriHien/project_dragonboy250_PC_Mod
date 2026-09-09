using System;
using Assets.src.g;
public partial class GameScr : mScreen, IChatable
{
	public bool isWaitingDoubleClick;

	public long timeStartDblClick;

	public long timeEndDblClick;

	public static bool isPaintOther = false;

	public static MyVector textTime = new MyVector(string.Empty);

	public static bool isLoadAllData = false;

	public static GameScr instance;

	public static int gW;

	public static int gH;

	public static int gW2;

	public static int gssw;

	public static int gssh;

	public static int gH34;

	public static int gW3;

	public static int gH3;

	public static int gH23;

	public static int gW23;

	public static int gH2;

	public static int csPadMaxH;

	public static int cmdBarH;

	public static int gW34;

	public static int gW6;

	public static int gH6;

	public static int cmx;

	public static int cmy;

	public static int cmdx;

	public static int cmdy;

	public static int cmvx;

	public static int cmvy;

	public static int cmtoX;

	public static int cmtoY;

	public static int cmxLim;

	public static int cmyLim;

	public static int gssx;

	public static int gssy;

	public static int gssxe;

	public static int gssye;

	public Command cmdback;

	public Command cmdBag;

	public Command cmdSkill;

	public Command cmdTiemnang;

	public Command cmdtrangbi;

	public Command cmdInfo;

	public Command cmdFocus;

	public Command cmdFire;

	public static int d;

	public static int hpPotion;

	public static SkillPaint[] sks;

	public static Arrowpaint[] arrs;

	public static DartInfo[] darts;

	public static Part[] parts;

	public static EffectCharPaint[] efs;

	public static int lockTick;

	private int moveUp;

	private int moveDow;

	private int idTypeTask;

	private bool isstarOpen;

	private bool isChangeSkill;

	public static MyVector vClan = new MyVector();

	public static MyVector vPtMap = new MyVector();

	public static MyVector vFriend = new MyVector();

	public static MyVector vEnemies = new MyVector();

	public static MyVector vCharInMap = new MyVector();

	public static MyVector vItemMap = new MyVector();

	public static MyVector vMobAttack = new MyVector();

	public static MyVector vSet = new MyVector();

	public static MyVector vMob = new MyVector();

	public static MyVector vNpc = new MyVector();

	public static MyVector vFlag = new MyVector();

	public static NClass[] nClasss;

	public static int indexSize = 28;

	public static int indexTitle = 0;

	public static int indexSelect = 0;

	public static int indexRow = -1;

	public static int indexRowMax;

	public static int indexMenu = 0;

	public Item itemFocus;

	public ItemOptionTemplate[] iOptionTemplates;

	public SkillOptionTemplate[] sOptionTemplates;

	private static Scroll scrInfo = new Scroll();

	public static Scroll scrMain = new Scroll();

	public static MyVector vItemUpGrade = new MyVector();

	public static bool isTypeXu;

	public static bool isViewNext;

	public static bool isViewClanMemOnline = false;

	public static bool isViewClanInvite = true;

	public static bool isChop;

	public static string titleInputText = string.Empty;

	public static int tickMove;

	public static bool isPaintAlert = false;

	public static bool isPaintTask = false;

	public static bool isPaintTeam = false;

	public static bool isPaintFindTeam = false;

	public static bool isPaintFriend = false;

	public static bool isPaintEnemies = false;

	public static bool isPaintItemInfo = false;

	public static bool isHaveSelectSkill = false;

	public static bool isPaintSkill = false;

	public static bool isPaintInfoMe = false;

	public static bool isPaintStore = false;

	public static bool isPaintNonNam = false;

	public static bool isPaintNonNu = false;

	public static bool isPaintAoNam = false;

	public static bool isPaintAoNu = false;

	public static bool isPaintGangTayNam = false;

	public static bool isPaintGangTayNu = false;

	public static bool isPaintQuanNam = false;

	public static bool isPaintQuanNu = false;

	public static bool isPaintGiayNam = false;

	public static bool isPaintGiayNu = false;

	public static bool isPaintLien = false;

	public static bool isPaintNhan = false;

	public static bool isPaintNgocBoi = false;

	public static bool isPaintPhu = false;

	public static bool isPaintWeapon = false;

	public static bool isPaintStack = false;

	public static bool isPaintStackLock = false;

	public static bool isPaintGrocery = false;

	public static bool isPaintGroceryLock = false;

	public static bool isPaintUpGrade = false;

	public static bool isPaintConvert = false;

	public static bool isPaintUpGradeGold = false;

	public static bool isPaintUpPearl = false;

	public static bool isPaintBox = false;

	public static bool isPaintSplit = false;

	public static bool isPaintCharInMap = false;

	public static bool isPaintTrade = false;

	public static bool isPaintZone = false;

	public static bool isPaintMessage = false;

	public static bool isPaintClan = false;

	public static bool isRequestMember = false;

	public static Char currentCharViewInfo;

	public static long[] exps;

	public static int[] crystals;

	public static int[] upClothe;

	public static int[] upAdorn;

	public static int[] upWeapon;

	public static int[] coinUpCrystals;

	public static int[] coinUpClothes;

	public static int[] coinUpAdorns;

	public static int[] coinUpWeapons;

	public static int[] maxPercents;

	public static int[] goldUps;

	public int tMenuDelay;

	public int zoneCol = 6;

	public int[] zones;

	public int[] pts;

	public int[] numPlayer;

	public int[] maxPlayer;

	public int[] rank1;

	public int[] rank2;

	public string[] rankName1;

	public string[] rankName2;

	public int typeTrade;

	public int typeTradeOrder;

	public int coinTrade;

	public int coinTradeOrder;

	public int timeTrade;

	public int indexItemUse = -1;

	public int cLastFocusID = -1;

	public int cPreFocusID = -1;

	public bool isLockKey;

	public static int[] tasks;

	public static int[] mapTasks;

	public static Image imgRoomStat;

	public static Image frBarPow0;

	public static Image frBarPow1;

	public static Image frBarPow2;

	public static Image frBarPow20;

	public static Image frBarPow21;

	public static Image frBarPow22;

	public MyVector texts;

	public string textsTitle;

	public static sbyte vcData;

	public static sbyte vcMap;

	public static sbyte vcSkill;

	public static sbyte vcItem;

	public static sbyte vsData;

	public static sbyte vsMap;

	public static sbyte vsSkill;

	public static sbyte vsItem;

	public static sbyte vcTask;

	public static Image imgArrow;

	public static Image imgArrow2;

	public static Image imgChat;

	public static Image imgChat2;

	public static Image imgMenu;

	public static Image imgFocus;

	public static Image imgFocus2;

	public static Image imgSkill;

	public static Image imgSkill2;

	public static Image imgHP1;

	public static Image imgHP2;

	public static Image imgHP3;

	public static Image imgHP4;

	public static Image imgFire0;

	public static Image imgFire1;

	public static Image imgNR1;

	public static Image imgNR2;

	public static Image imgNR3;

	public static Image imgNR4;

	public static Image imgLbtn;

	public static Image imgLbtnFocus;

	public static Image imgLbtn2;

	public static Image imgLbtnFocus2;

	public static Image imgAnalog1;

	public static Image imgAnalog2;

	public string tradeName = string.Empty;

	public string tradeItemName = string.Empty;

	public int timeLengthMap;

	public int timeStartMap;

	public static sbyte typeViewInfo = 0;

	public static sbyte typeActive = 0;

	public static InfoMe info1 = new InfoMe();

	public static InfoMe info2 = new InfoMe();

	public static Image imgPanel;

	public static Image imgPanel2;

	public static Image imgHP;

	public static Image imgMP;

	public static Image imgSP;

	public static Image imgHPLost;

	public static Image imgMPLost;

	public static Image imgHP_tm_do;

	public static Image imgHP_tm_vang;

	public static Image imgHP_tm_xam;

	public static Image imgHP_tm_xanh;

	public Mob mobCapcha;

	public MagicTree magicTree;

	private short l;

	public static int countEff;

	public static GamePad gamePad = new GamePad();

	public static Image imgChatPC;

	public static Image imgChatsPC2;

	public static int isAnalog = 0;

	public static Image img_ct_bar_0 = mSystem.loadImage("/mainImage/i_pve_bar_0.png");

	public static Image img_ct_bar_1 = mSystem.loadImage("/mainImage/i_pve_bar_1.png");

	public static bool isUseTouch;

	public Command cmdDoiCo;

	public Command cmdLogOut;

	public Command cmdChatTheGioi;

	public Command cmdshowInfo;

	private static Command[] cmdTestLogin = null;

	public const int numSkill = 10;

	public const int numSkill_2 = 5;

	public static Skill[] keySkill = new Skill[10];

	public static Skill[] onScreenSkill = new Skill[10];

	public Command cmdMenu;

	public static int firstY;

}
