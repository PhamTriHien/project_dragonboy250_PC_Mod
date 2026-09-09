using System;
using Assets.src.g;

public partial class GameScr : mScreen, IChatable
{
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
