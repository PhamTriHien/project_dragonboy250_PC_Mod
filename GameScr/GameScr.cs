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

	public static int wSkill;

	public static long deltaTime;

	public bool isPointerDowning;

	public bool isChangingCameraMode;

	private int ptLastDownX;

	private int ptLastDownY;

	private int ptFirstDownX;

	private int ptFirstDownY;

	private int ptDownTime;

	private bool disableSingleClick;

	public long lastSingleClick;

	public bool clickMoving;

	public bool clickOnTileTop;

	public bool clickMovingRed;

	private int clickToX;

	private int clickToY;

	private int lastClickCMX;

	private int lastClickCMY;

	private int clickMovingP1;

	private int clickMovingTimeOut;

	private long lastMove;

	public static bool isNewClanMessage;

	private long lastFire;

	private long lastUsePotion;

	public int auto;

	public int dem;

	private string strTam = string.Empty;

	private int a;

	public bool isFreez;

	public bool isUseFreez;

	public static Image imgTrans;

	public bool isRongThanXuatHien;

	public bool isRongNamek;

	public bool isSuperPower;

	public int tPower;

	public int xPower;

	public int yPower;

	public int dxPower;

	public bool activeRongThan;

	public bool isMeCallRongThan;

	public int mautroi;

	public int mapRID;

	public int zoneRID;

	public int bgRID = -1;

	public static int tam = 0;

	public static bool isAutoPlay;

	public static bool canAutoPlay;

	public static bool isChangeZone;

	private int timeSkill;

	private int nSkill;

	private int selectedIndexSkill = -1;

	public static Skill lastSkill;

	private bool doSeleckSkillFlag;

	public string strCapcha;

	private long longPress;

	private int move;

	public bool flareFindFocus;

	private int flareTime;

	public int keyTouchSkill = -1;

	private long lastSendUpdatePostion;

	public static long lastTick;

	public static long currTick;

	private int timeAuto;

	public static long lastXS;

	public static long currXS;

	public static int secondXS;

	public int runArrow;

	public static int isPaintRada;

	public static Image imgNut;

	public static Image imgNutF;

	public int[] keyCapcha;

	public static Image imgCapcha;

	public string keyInput;

	public static int disXC;

	public static bool isPaint = true;

	public static int shock_scr;

	private static int[] shock_x = new int[4] { 1, -1, 1, -1 };

	private static int[] shock_y = new int[4] { 1, -1, -1, 1 };

	private int tDoubleDelay;

	public static Image arrow;

	private static int yTouchBar;

	private static int xC;

	private static int yC;

	private static int xL;

	private static int yL;

	public int xR;

	public int yR;

	private static int xU;

	private static int yU;

	private static int xF;

	private static int yF;

	public static int xHP;

	public static int yHP;

	private static int xTG;

	private static int yTG;

	public static int[] xS;

	public static int[] yS;

	public static int xSkill;

	public static int ySkill;

	public static int padSkill;

	public long dMP;

	public long twMp;

	public bool isInjureMp;

	public long dHP;

	public long twHp;

	public bool isInjureHp;

	private long curr;

	private long last;

	private int secondVS;

	private int[] idVS = new int[2] { -1, -1 };

	public static string[] flyTextString;

	public static int[] flyTextX;

	public static int[] flyTextY;

	public static int[] flyTextYTo;

	public static int[] flyTextDx;

	public static int[] flyTextDy;

	public static int[] flyTextState;

	public static int[] flyTextColor;

	public static int[] flyTime;

	public static int[] splashX;

	public static int[] splashY;

	public static int[] splashState;

	public static int[] splashF;

	public static int[] splashDir;

	public static Image[] imgSplash;

	public static int cmdBarX;

	public static int cmdBarY;

	public static int cmdBarW;

	public static int cmdBarLeftW;

	public static int cmdBarRightW;

	public static int cmdBarCenterW;

	public static int hpBarX;

	public static int hpBarY;

	public static int spBarW;

	public static int mpBarW;

	public static int expBarW;

	public static int lvPosX;

	public static int moneyPosX;

	public static int hpBarH;

	public static int girlHPBarY;

	public static long hpBarW;

	public static Image[] imgCmdBar;

	private int imgScrW;

	public static int popupY;

	public static int popupX;

	public static int isborderIndex;

	public static int isselectedRow;

	private static Image imgNolearn;

	public int cmxp;

	public int cmvxp;

	public int cmdxp;

	public int cmxLimp;

	public int cmyLimp;

	public int cmyp;

	public int cmvyp;

	public int cmdyp;

	private int indexTiemNang;

	private string alertURL;

	private string fnick;

	public static int xstart;

	public static int ystart;

	public static int popupW = 140;

	public static int popupH = 160;

	public static int cmySK;

	public static int cmtoYSK;

	public static int cmdySK;

	public static int cmvySK;

	public static int cmyLimSK;

	public static int columns = 6;

	public static int rows;

	private int totalRowInfo;

	private int ypaintKill;

	private int ylimUp;

	private int ylimDow;

	private int yPaint;

	public static int indexEff = 0;

	public static EffectCharPaint effUpok;

	public static int inforX;

	public static int inforY;

	public static int inforW;

	public static int inforH;

	public Command cmdDead;

	public static bool notPaint = false;

	public static bool isPing = false;

	public static int INFO = 0;

	public static int STORE = 1;

	public static int ZONE = 2;

	public static int UPGRADE = 3;

	private int Hitem = 30;

	private int maxSizeRow = 5;

	private int isTranKyNang;

	private bool isTran;

	private int cmY_Old;

	private int cmX_Old;

	public PopUpYesNo popUpYesNo;

	public static MyVector vChatVip = new MyVector();

	public static int vBig;

	public bool isFireWorks;

	public int[] winnumber;

	public int[] randomNumber;

	public int[] tMove;

	public int[] moveCount;

	public int[] delayMove;

	public int moveIndex;

	private bool isWin;

	private string strFinish;

	private int tShow;

	private int xChatVip;

	private int currChatWidth;

	private bool startChat;

	public sbyte percentMabu;

	public bool mabuEff;

	public int tMabuEff;

	public static bool isPaintChatVip;

	public static sbyte mabuPercent;

	public static sbyte isNewMember;

	private string yourNumber = string.Empty;

	private string[] strPaint;

	public static Image imgHP_NEW;

	public static InfoPhuBan phuban_Info;

	public static FrameImage fra_PVE_Bar_0;

	public static FrameImage fra_PVE_Bar_1;

	public static Image imgVS;

	public static Image imgBall;

	public static Image imgKhung;

	public int countFrameSkill;

	public static Image imgBgIOS;

	public static int nCT_TeamB = 50;

	public static int nCT_TeamA = 50;

	public static long nCT_timeBallte;

	public static string nCT_team;

	public static int nCT_nBoyBaller = 100;

	public static bool isPaint_CT;

	public static sbyte nCT_floor;

	public static bool is_Paint_boardCT_Expand;

	private static int xRect;

	private static int yRect;

	private static int wRect;

	private static int hRect;

	public static MyVector res_CT = new MyVector();

	public static int nTop = 1;

	public static bool isPickNgocRong = false;

	public static int nUSER_CT;

	public static int nUSER_MAX_CT;

	public static bool isudungCapsun;

	public static bool isudungCapsun4;

	public static bool isudungCapsun3;




































































}
