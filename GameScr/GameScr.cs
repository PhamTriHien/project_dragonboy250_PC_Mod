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

	private Skill lastSkill;

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

	public GameScr()
		{
			if (GameCanvas.w == 128 || GameCanvas.h <= 208)
			{
				indexSize = 20;
			}
			cmdback = new Command(string.Empty, 11021);
			cmdMenu = new Command("menu", 11000);
			cmdFocus = new Command(string.Empty, 11001);
			cmdMenu.img = imgMenu;
			int mImgW = (imgMenu != null) ? mGraphics.getImageWidth(imgMenu) : 0;
			int mImgH = (imgMenu != null) ? mGraphics.getImageHeight(imgMenu) : 0;
			cmdMenu.w = (mImgW > 0) ? (mImgW + 20) : 60;
			cmdMenu.h = (mImgH > 0) ? (mImgH + 12) : 32;
			cmdMenu.isPlaySoundButton = false;
			cmdFocus.img = imgFocus;
			if (GameCanvas.isTouch)
			{
				cmdMenu.x = 0;
				cmdMenu.y = 50;
				cmdFocus = null;
			}
			else
			{
				cmdMenu.x = 0;
				cmdMenu.y = gH - 30;
				cmdFocus.x = gW - 32;
				cmdFocus.y = gH - 32;
			}
			left = cmdMenu;
			right = cmdFocus;
			isPaintRada = 1;
			if (GameCanvas.isTouch)
			{
				isHaveSelectSkill = true;
			}
			cmdDoiCo = new Command("Đổi cờ", GameCanvas.gI(), 100001, null);
			cmdLogOut = new Command("Logout", GameCanvas.gI(), 100002, null);
			cmdChatTheGioi = new Command("chat world", GameCanvas.gI(), 100003, null);
			cmdshowInfo = new Command("InfoLog", GameCanvas.gI(), 100004, null);
			cmdDoiCo.setType();
			cmdLogOut.setType();
			cmdChatTheGioi.setType();
			cmdshowInfo.setType();
			cmdChatTheGioi.x = GameCanvas.w - cmdChatTheGioi.w;
			cmdshowInfo.x = GameCanvas.w - cmdshowInfo.w;
			cmdLogOut.x = GameCanvas.w - cmdLogOut.w;
			cmdDoiCo.x = GameCanvas.w - cmdDoiCo.w;
			cmdChatTheGioi.y = cmdChatTheGioi.h + mFont.tahoma_7_white.getHeight();
			cmdshowInfo.y = cmdChatTheGioi.h * 2 + mFont.tahoma_7_white.getHeight();
			cmdLogOut.y = cmdChatTheGioi.h * 3 + mFont.tahoma_7_white.getHeight();
			cmdDoiCo.y = cmdChatTheGioi.h * 4 + mFont.tahoma_7_white.getHeight();
		}

	public static void loadBg()
		{
			fra_PVE_Bar_0 = new FrameImage(mSystem.loadImage("/mainImage/i_pve_bar_0.png"), 6, 15);
			fra_PVE_Bar_1 = new FrameImage(mSystem.loadImage("/mainImage/i_pve_bar_1.png"), 38, 21);
			imgVS = mSystem.loadImage("/mainImage/i_vs.png");
			imgBall = mSystem.loadImage("/mainImage/i_charlife.png");
			imgHP_NEW = mSystem.loadImage("/mainImage/i_hp.png");
			imgKhung = mSystem.loadImage("/mainImage/i_khung.png");
			imgLbtn = GameCanvas.loadImage("/mainImage/myTexture2dbtnl.png");
			imgLbtnFocus = GameCanvas.loadImage("/mainImage/myTexture2dbtnlf.png");
			imgLbtn2 = GameCanvas.loadImage("/mainImage/myTexture2dbtnl2.png");
			imgLbtnFocus2 = GameCanvas.loadImage("/mainImage/myTexture2dbtnlf2.png");
			imgPanel = GameCanvas.loadImage("/mainImage/myTexture2dpanel.png");
			imgPanel2 = GameCanvas.loadImage("/mainImage/panel2.png");
			imgHP = GameCanvas.loadImage("/mainImage/myTexture2dHP.png");
			imgSP = GameCanvas.loadImage("/mainImage/SP.png");
			imgHPLost = GameCanvas.loadImage("/mainImage/myTexture2dhpLost.png");
			imgMPLost = GameCanvas.loadImage("/mainImage/myTexture2dmpLost.png");
			imgMP = GameCanvas.loadImage("/mainImage/myTexture2dMP.png");
			imgSkill = GameCanvas.loadImage("/mainImage/myTexture2dskill.png");
			imgSkill2 = GameCanvas.loadImage("/mainImage/myTexture2dskill2.png");
			imgMenu = GameCanvas.loadImage("/mainImage/myTexture2dmenu.png");
			if (imgMenu != null && instance != null && instance.cmdMenu != null)
			{
				instance.cmdMenu.img = imgMenu;
				instance.cmdMenu.w = mGraphics.getImageWidth(imgMenu) + 20;
				instance.cmdMenu.h = mGraphics.getImageHeight(imgMenu) + 12;
			}
			imgFocus = GameCanvas.loadImage("/mainImage/myTexture2dfocus.png");
			imgHP_tm_do = GameCanvas.loadImage("/mainImage/tm-do.png");
			imgHP_tm_vang = GameCanvas.loadImage("/mainImage/tm-vang.png");
			imgHP_tm_xam = GameCanvas.loadImage("/mainImage/tm-xam.png");
			imgHP_tm_xanh = GameCanvas.loadImage("/mainImage/tm-xanh.png");
			imgChatPC = GameCanvas.loadImage("/pc/chat.png");
			imgChatsPC2 = GameCanvas.loadImage("/pc/chat2.png");
			imgArrow = GameCanvas.loadImage("/mainImage/myTexture2darrow.png");
			imgArrow2 = GameCanvas.loadImage("/mainImage/myTexture2darrow2.png");
			if (GameCanvas.isTouch)
			{
				imgChat = GameCanvas.loadImage("/mainImage/myTexture2dchat.png");
				imgChat2 = GameCanvas.loadImage("/mainImage/myTexture2dchat2.png");
				imgFocus2 = GameCanvas.loadImage("/mainImage/myTexture2dfocus2.png");
				imgHP1 = GameCanvas.loadImage("/mainImage/myTexture2dPea0.png");
				imgHP2 = GameCanvas.loadImage("/mainImage/myTexture2dPea1.png");
				imgAnalog1 = GameCanvas.loadImage("/mainImage/myTexture2danalog1.png");
				imgAnalog2 = GameCanvas.loadImage("/mainImage/myTexture2danalog2.png");
				imgHP3 = GameCanvas.loadImage("/mainImage/myTexture2dPea2.png");
				imgHP4 = GameCanvas.loadImage("/mainImage/myTexture2dPea3.png");
				imgFire0 = GameCanvas.loadImage("/mainImage/myTexture2dfirebtn0.png");
				imgFire1 = GameCanvas.loadImage("/mainImage/myTexture2dfirebtn1.png");
			}
			imgNR1 = GameCanvas.loadImage("/mainImage/myTexture2dPea_0.png");
			imgNR2 = GameCanvas.loadImage("/mainImage/myTexture2dPea_1.png");
			imgNR3 = GameCanvas.loadImage("/mainImage/myTexture2dPea_2.png");
			imgNR4 = GameCanvas.loadImage("/mainImage/myTexture2dPea_3.png");
			flyTextX = new int[5];
			flyTextY = new int[5];
			flyTextDx = new int[5];
			flyTextDy = new int[5];
			flyTextState = new int[5];
			flyTextString = new string[5];
			flyTextYTo = new int[5];
			flyTime = new int[5];
			flyTextColor = new int[8];
			for (int i = 0; i < 5; i++)
			{
				flyTextState[i] = -1;
			}
			sbyte[] array = Rms.loadRMS("NRdataVersion");
			sbyte[] array2 = Rms.loadRMS("NRmapVersion");
			sbyte[] array3 = Rms.loadRMS("NRskillVersion");
			sbyte[] array4 = Rms.loadRMS("NRitemVersion");
			if (array != null)
			{
				vcData = array[0];
			}
			if (array2 != null)
			{
				vcMap = array2[0];
			}
			if (array3 != null)
			{
				vcSkill = array3[0];
			}
			if (array4 != null)
			{
				vcItem = array4[0];
			}
			imgNut = GameCanvas.loadImage("/mainImage/myTexture2dnut.png");
			imgNutF = GameCanvas.loadImage("/mainImage/myTexture2dnutF.png");
			MobCapcha.init();
			isAnalog = ((Rms.loadRMSInt("analog") == 1) ? 1 : 0);
			gamePad = new GamePad();
			arrow = GameCanvas.loadImage("/mainImage/myTexture2darrow3.png");
			imgTrans = GameCanvas.loadImage("/bg/trans.png");
			imgRoomStat = GameCanvas.loadImage("/mainImage/myTexture2dstat.png");
			frBarPow0 = GameCanvas.loadImage("/mainImage/myTexture2dlineColor20.png");
			frBarPow1 = GameCanvas.loadImage("/mainImage/myTexture2dlineColor21.png");
			frBarPow2 = GameCanvas.loadImage("/mainImage/myTexture2dlineColor22.png");
			frBarPow20 = GameCanvas.loadImage("/mainImage/myTexture2dlineColor00.png");
			frBarPow21 = GameCanvas.loadImage("/mainImage/myTexture2dlineColor01.png");
			frBarPow22 = GameCanvas.loadImage("/mainImage/myTexture2dlineColor02.png");
		}

	public void initSelectChar()
		{
			readPart();
			SmallImage.init();
		}

	public void initTraining()
		{
			if (CreateCharScr.isCreateChar)
			{
				CreateCharScr.isCreateChar = false;
				right = null;
			}
		}

	public bool isMapDocNhan()
		{
			if (TileMap.mapID >= 53 && TileMap.mapID <= 62)
			{
				return true;
			}
			return false;
		}

	public bool isMapFize()
		{
			if (TileMap.mapID >= 63)
			{
				return true;
			}
			return false;
		}

	public override void switchToMe()
		{
			vChatVip.removeAllElements();
			ServerListScreen.isWait = false;
			if (BackgroudEffect.isHaveRain())
			{
				SoundMn.gI().rain();
			}
			LoginScr.isContinueToLogin = false;
			Char.isLoadingMap = false;
			if (!isPaintOther)
			{
				Service.gI().finishLoadMap();
			}
			if (TileMap.isTrainingMap())
			{
				initTraining();
			}
			info1.isUpdate = true;
			info2.isUpdate = true;
			resetButton();
			isLoadAllData = true;
			isPaintOther = false;
			base.switchToMe();
		}

	public static int getMaxExp(int level)
		{
			int num = 0;
			for (int i = 0; i <= level; i++)
			{
				num += (int)exps[i];
			}
			return num;
		}

	public static void resetAllvector()
		{
			vCharInMap.removeAllElements();
			Teleport.vTeleport.removeAllElements();
			vItemMap.removeAllElements();
			Effect2.vEffect2.removeAllElements();
			Effect2.vAnimateEffect.removeAllElements();
			Effect2.vEffect2Outside.removeAllElements();
			Effect2.vEffectFeet.removeAllElements();
			Effect2.vEffect3.removeAllElements();
			vMobAttack.removeAllElements();
			vMob.removeAllElements();
			vNpc.removeAllElements();
			Char.myCharz().vMovePoints.removeAllElements();
		}

	public bool isBagFull()
		{
			for (int num = Char.myCharz().arrItemBag.Length - 1; num >= 0; num--)
			{
				if (Char.myCharz().arrItemBag[num] == null)
				{
					return false;
				}
			}
			return true;
		}

	public void createConfirm(string[] menu, Npc npc)
		{
			resetButton();
			isLockKey = true;
			left = new Command(menu[0], 130011, npc);
			right = new Command(menu[1], 130012, npc);
		}

	public void readPart()
		{
			DataInputStream dataInputStream = null;
			try
			{
				dataInputStream = new DataInputStream(Rms.loadRMS("NR_part"));
				int num = dataInputStream.readShort();
				parts = new Part[num];
				for (int i = 0; i < num; i++)
				{
					int type = dataInputStream.readByte();
					parts[i] = new Part(type);
					for (int j = 0; j < parts[i].pi.Length; j++)
					{
						parts[i].pi[j] = new PartImage();
						parts[i].pi[j].id = dataInputStream.readShort();
						parts[i].pi[j].dx = dataInputStream.readByte();
						parts[i].pi[j].dy = dataInputStream.readByte();
					}
				}
			}
			catch (Exception ex)
			{
				Cout.LogError("LOI TAI readPart " + ex.ToString());
			}
			finally
			{
				try
				{
					dataInputStream.close();
				}
				catch (Exception ex2)
				{
					Res.outz2("LOI TAI readPart 2" + ex2.StackTrace);
				}
			}
		}

	public void readEfect()
		{
			DataInputStream dataInputStream = null;
			try
			{
				dataInputStream = new DataInputStream(Rms.loadRMS("NR_effect"));
				int num = dataInputStream.readShort();
				efs = new EffectCharPaint[num];
				for (int i = 0; i < num; i++)
				{
					efs[i] = new EffectCharPaint();
					efs[i].idEf = dataInputStream.readShort();
					efs[i].arrEfInfo = new EffectInfoPaint[dataInputStream.readByte()];
					for (int j = 0; j < efs[i].arrEfInfo.Length; j++)
					{
						efs[i].arrEfInfo[j] = new EffectInfoPaint();
						efs[i].arrEfInfo[j].idImg = dataInputStream.readShort();
						efs[i].arrEfInfo[j].dx = dataInputStream.readByte();
						efs[i].arrEfInfo[j].dy = dataInputStream.readByte();
					}
				}
			}
			catch (Exception)
			{
			}
			finally
			{
				try
				{
					dataInputStream.close();
				}
				catch (Exception ex2)
				{
					Cout.LogError("Loi ham Eff: " + ex2.ToString());
				}
			}
		}

	public void readArrow()
		{
			DataInputStream dataInputStream = null;
			try
			{
				dataInputStream = new DataInputStream(Rms.loadRMS("NR_arrow"));
				int num = dataInputStream.readShort();
				arrs = new Arrowpaint[num];
				for (int i = 0; i < num; i++)
				{
					arrs[i] = new Arrowpaint();
					arrs[i].id = dataInputStream.readShort();
					arrs[i].imgId[0] = dataInputStream.readShort();
					arrs[i].imgId[1] = dataInputStream.readShort();
					arrs[i].imgId[2] = dataInputStream.readShort();
				}
			}
			catch (Exception)
			{
			}
			finally
			{
				try
				{
					dataInputStream.close();
				}
				catch (Exception ex2)
				{
					Cout.LogError("Loi ham readArrow: " + ex2.ToString());
				}
			}
		}

	public void readOk()
		{
			try
			{
				Res.outz("<readOk><vsData<" + vsData + "==" + vcData);
				Res.outz("<readOk><vsMap<" + vsMap + "==" + vcMap);
				Res.outz("<readOk><vsSkill<" + vsSkill + "==" + vcSkill);
				Res.outz("<readOk><vsItem<" + vsItem + "==" + vcItem);
				if (vsData == vcData && vsMap == vcMap && vsSkill == vcSkill && vsItem == vcItem)
				{
					Res.outz(vsData + "," + vsMap + "," + vsSkill + "," + vsItem);
					gI().readDart();
					gI().readEfect();
					gI().readArrow();
					gI().readSkill();
					Service.gI().clientOk();
				}
			}
			catch (Exception ex)
			{
				Cout.LogError("Loi ham readOk: " + ex.ToString());
			}
		}

	public static GameScr gI()
		{
			if (instance == null)
			{
				instance = new GameScr();
			}
			return instance;
		}

	public static void clearGameScr()
		{
			instance = null;
		}

	public void loadGameScr()
		{
			loadSplash();
			Res.init();
			loadInforBar();
		}

	public bool testAct()
		{
			for (sbyte b = 2; b < 9; b += 2)
			{
				if (GameCanvas.keyHold[b])
				{
					return false;
				}
			}
			return true;
		}

	public void clanInvite(string strInvite, int clanID, int code)
		{
			ClanObject clanObject = new ClanObject();
			clanObject.code = code;
			clanObject.clanID = clanID;
			startYesNoPopUp(strInvite, new Command(mResources.YES, 12002, clanObject), new Command(mResources.NO, 12003, clanObject));
		}

	public bool isAttack()
		{
			if (checkClickToBotton(Char.myCharz().charFocus))
			{
				return false;
			}
			if (checkClickToBotton(Char.myCharz().mobFocus))
			{
				return false;
			}
			if (checkClickToBotton(Char.myCharz().npcFocus))
			{
				return false;
			}
			if (ChatTextField.gI().isShow)
			{
				return false;
			}
			if (InfoDlg.isLock || Char.myCharz().isLockAttack || Char.isLockKey)
			{
				return false;
			}
			if (Char.myCharz().myskill != null && Char.myCharz().myskill.template.id == 6 && Char.myCharz().itemFocus != null)
			{
				pickItem();
				return false;
			}
			if (Char.myCharz().myskill != null && Char.myCharz().myskill.template.type == 2 && Char.myCharz().npcFocus == null && Char.myCharz().myskill.template.id != 6)
			{
				if (!checkSkillValid())
				{
					return false;
				}
				return true;
			}
			if (Char.myCharz().skillPaint != null || (Char.myCharz().mobFocus == null && Char.myCharz().npcFocus == null && Char.myCharz().charFocus == null && Char.myCharz().itemFocus == null))
			{
				return false;
			}
			if (Char.myCharz().mobFocus != null)
			{
				if (Char.myCharz().mobFocus.isBigBoss() && Char.myCharz().mobFocus.status == 4)
				{
					Char.myCharz().mobFocus = null;
					Char.myCharz().currentMovePoint = null;
				}
				isAutoPlay = true;
				if (!isMeCanAttackMob(Char.myCharz().mobFocus))
				{
					Res.outz("can not attack");
					return false;
				}
				if (mobCapcha != null)
				{
					return false;
				}
				if (Char.myCharz().myskill == null)
				{
					return false;
				}
				if (Char.myCharz().isSelectingSkillUseAlone())
				{
					return false;
				}
				int num = -1;
				int num2 = Res.abs(Char.myCharz().cx - cmx) * mGraphics.zoomLevel;
				if (Char.myCharz().charFocus != null)
				{
					num = Res.abs(Char.myCharz().cx - Char.myCharz().charFocus.cx) * mGraphics.zoomLevel;
				}
				else if (Char.myCharz().mobFocus != null)
				{
					num = Res.abs(Char.myCharz().cx - Char.myCharz().mobFocus.x) * mGraphics.zoomLevel;
				}
				if ((Char.myCharz().mobFocus.status == 1 || Char.myCharz().mobFocus.status == 0 || Char.myCharz().myskill.template.type == 4 || num == -1 || num > num2) && Char.myCharz().myskill.template.type == 4)
				{
					if (Char.myCharz().mobFocus.x < Char.myCharz().cx)
					{
						Char.myCharz().cdir = -1;
					}
					else
					{
						Char.myCharz().cdir = 1;
					}
					doSelectSkill(Char.myCharz().myskill, isShortcut: true);
				}
				if (!checkSkillValid())
				{
					return false;
				}
				if (Char.myCharz().cx < Char.myCharz().mobFocus.getX())
				{
					Char.myCharz().cdir = 1;
				}
				else
				{
					Char.myCharz().cdir = -1;
				}
				int num3 = Math.abs(Char.myCharz().cx - Char.myCharz().mobFocus.getX());
				int num4 = Math.abs(Char.myCharz().cy - Char.myCharz().mobFocus.getY());
				Char.myCharz().cvx = 0;
				if (num3 <= Char.myCharz().myskill.dx && num4 <= Char.myCharz().myskill.dy)
				{
					if (Char.myCharz().myskill.template.id == 20)
					{
						return true;
					}
					if (num4 > num3 && Res.abs(Char.myCharz().cy - Char.myCharz().mobFocus.getY()) > 30 && Char.myCharz().mobFocus.getTemplate().type == 4)
					{
						Char.myCharz().currentMovePoint = new MovePoint(Char.myCharz().cx + Char.myCharz().cdir, Char.myCharz().mobFocus.getY());
						Char.myCharz().endMovePointCommand = new Command(null, null, 8002, null);
						GameCanvas.clearKeyHold();
						GameCanvas.clearKeyPressed();
						return false;
					}
					int num5 = 20;
					bool flag = false;
					if (Char.myCharz().mobFocus is BigBoss || Char.myCharz().mobFocus is BigBoss2)
					{
						flag = true;
					}
					if (Char.myCharz().myskill.dx > 100)
					{
						num5 = 60;
						if (num3 < 20)
						{
							Char.myCharz().createShadow(Char.myCharz().cx, Char.myCharz().cy, 10);
						}
					}
					bool flag2 = false;
					if ((TileMap.tileTypeAtPixel(Char.myCharz().cx, Char.myCharz().cy + 3) & 2) == 2)
					{
						int num6 = ((Char.myCharz().cx > Char.myCharz().mobFocus.getX()) ? 1 : (-1));
						if ((TileMap.tileTypeAtPixel(Char.myCharz().mobFocus.getX() + num5 * num6, Char.myCharz().cy + 3) & 2) != 2)
						{
							flag2 = true;
						}
					}
					if (num3 <= num5 && !flag2)
					{
						if (Char.myCharz().cx > Char.myCharz().mobFocus.getX())
						{
							int num7 = Char.myCharz().mobFocus.getX() + num5 + (flag ? 30 : 0);
							int i = Char.myCharz().mobFocus.getX();
							bool flag3 = false;
							for (; i < num7; i += 24)
							{
								if (TileMap.tileTypeAtPixel(i, Char.myCharz().cy + 3) == 8 || TileMap.tileTypeAtPixel(i, Char.myCharz().cy + 3) == 4)
								{
									flag3 = true;
									break;
								}
							}
							if (flag3)
							{
								Char.myCharz().cx = i - 24;
							}
							else
							{
								Char.myCharz().cx = num7;
							}
							Char.myCharz().cdir = -1;
						}
						else
						{
							int num8 = Char.myCharz().mobFocus.getX() - num5 - (flag ? 30 : 0);
							int num9 = Char.myCharz().mobFocus.getX();
							bool flag4 = false;
							while (num9 > num8)
							{
								if (TileMap.tileTypeAtPixel(num9, Char.myCharz().cy + 3) == 8 || TileMap.tileTypeAtPixel(num9, Char.myCharz().cy + 3) == 4)
								{
									flag4 = true;
									break;
								}
								num9 -= 24;
							}
							if (flag4)
							{
								Char.myCharz().cx = num9 + 24;
							}
							else
							{
								Char.myCharz().cx = num8;
							}
							Char.myCharz().cdir = 1;
						}
						Service.gI().charMove();
					}
					GameCanvas.clearKeyHold();
					GameCanvas.clearKeyPressed();
					return true;
				}
				bool flag5 = false;
				if (Char.myCharz().mobFocus is BigBoss || Char.myCharz().mobFocus is BigBoss2)
				{
					flag5 = true;
				}
				int num10 = (Char.myCharz().myskill.dx - ((!flag5) ? 20 : 50)) * ((Char.myCharz().cx > Char.myCharz().mobFocus.getX()) ? 1 : (-1));
				if (num3 <= Char.myCharz().myskill.dx)
				{
					num10 = 0;
				}
				Char.myCharz().currentMovePoint = new MovePoint(Char.myCharz().mobFocus.getX() + num10, Char.myCharz().mobFocus.getY());
				Char.myCharz().endMovePointCommand = new Command(null, null, 8002, null);
				GameCanvas.clearKeyHold();
				GameCanvas.clearKeyPressed();
				return false;
			}
			if (Char.myCharz().npcFocus != null)
			{
				if (Char.myCharz().npcFocus.isHide)
				{
					return false;
				}
				if (Char.myCharz().cx < Char.myCharz().npcFocus.cx)
				{
					Char.myCharz().cdir = 1;
				}
				else
				{
					Char.myCharz().cdir = -1;
				}
				if (Char.myCharz().cx < Char.myCharz().npcFocus.cx)
				{
					Char.myCharz().npcFocus.cdir = -1;
				}
				else
				{
					Char.myCharz().npcFocus.cdir = 1;
				}
				int num11 = Math.abs(Char.myCharz().cx - Char.myCharz().npcFocus.cx);
				int num12 = Math.abs(Char.myCharz().cy - Char.myCharz().npcFocus.cy);
				if (num12 > 40)
				{
					Char.myCharz().cy = Char.myCharz().npcFocus.cy - 40;
				}
				if (num11 < 60)
				{
					GameCanvas.clearKeyHold();
					GameCanvas.clearKeyPressed();
					if (tMenuDelay == 0)
					{
						if (Char.myCharz().taskMaint != null && Char.myCharz().taskMaint.taskId == 0)
						{
							if (Char.myCharz().taskMaint.index < 4 && Char.myCharz().npcFocus.template.npcTemplateId == 4)
							{
								return false;
							}
							if (Char.myCharz().taskMaint.index < 3 && Char.myCharz().npcFocus.template.npcTemplateId == 3)
							{
								return false;
							}
						}
						tMenuDelay = 50;
						InfoDlg.showWait();
						Service.gI().charMove();
						Service.gI().openMenu(Char.myCharz().npcFocus.template.npcTemplateId);
					}
				}
				else
				{
					int num13 = (20 + Res.r.nextInt(20)) * ((Char.myCharz().cx > Char.myCharz().npcFocus.cx) ? 1 : (-1));
					Char.myCharz().currentMovePoint = new MovePoint(Char.myCharz().npcFocus.cx + num13, Char.myCharz().cy);
					Char.myCharz().endMovePointCommand = new Command(null, null, 8002, null);
					GameCanvas.clearKeyHold();
					GameCanvas.clearKeyPressed();
				}
				return false;
			}
			if (Char.myCharz().charFocus != null)
			{
				if (mobCapcha != null)
				{
					return false;
				}
				if (Char.myCharz().cx < Char.myCharz().charFocus.cx)
				{
					Char.myCharz().cdir = 1;
				}
				else
				{
					Char.myCharz().cdir = -1;
				}
				int num14 = Math.abs(Char.myCharz().cx - Char.myCharz().charFocus.cx);
				int num15 = Math.abs(Char.myCharz().cy - Char.myCharz().charFocus.cy);
				if (Char.myCharz().isMeCanAttackOtherPlayer(Char.myCharz().charFocus) || Char.myCharz().isSelectingSkillBuffToPlayer())
				{
					if (Char.myCharz().myskill == null)
					{
						return false;
					}
					if (!checkSkillValid())
					{
						return false;
					}
					if (Char.myCharz().cx < Char.myCharz().charFocus.cx)
					{
						Char.myCharz().cdir = 1;
					}
					else
					{
						Char.myCharz().cdir = -1;
					}
					Char.myCharz().cvx = 0;
					if (num14 <= Char.myCharz().myskill.dx && num15 <= Char.myCharz().myskill.dy)
					{
						if (Char.myCharz().myskill.template.id == 20)
						{
							return true;
						}
						int num16 = 20;
						if (Char.myCharz().myskill.dx > 60)
						{
							num16 = 60;
							if (num14 < 20)
							{
								Char.myCharz().createShadow(Char.myCharz().cx, Char.myCharz().cy, 10);
							}
						}
						bool flag6 = false;
						if ((TileMap.tileTypeAtPixel(Char.myCharz().cx, Char.myCharz().cy + 3) & 2) == 2)
						{
							int num17 = ((Char.myCharz().cx > Char.myCharz().charFocus.cx) ? 1 : (-1));
							if ((TileMap.tileTypeAtPixel(Char.myCharz().charFocus.cx + num16 * num17, Char.myCharz().cy + 3) & 2) != 2)
							{
								flag6 = true;
							}
						}
						if (num14 <= num16 && !flag6)
						{
							if (Char.myCharz().cx > Char.myCharz().charFocus.cx)
							{
								Char.myCharz().cx = Char.myCharz().charFocus.cx + num16;
								Char.myCharz().cdir = -1;
							}
							else
							{
								Char.myCharz().cx = Char.myCharz().charFocus.cx - num16;
								Char.myCharz().cdir = 1;
							}
							Service.gI().charMove();
						}
						GameCanvas.clearKeyHold();
						GameCanvas.clearKeyPressed();
						return true;
					}
					int num18 = (Char.myCharz().myskill.dx - 20) * ((Char.myCharz().cx > Char.myCharz().charFocus.cx) ? 1 : (-1));
					if (num14 <= Char.myCharz().myskill.dx)
					{
						num18 = 0;
					}
					Char.myCharz().currentMovePoint = new MovePoint(Char.myCharz().charFocus.cx + num18, Char.myCharz().charFocus.cy);
					Char.myCharz().endMovePointCommand = new Command(null, null, 8002, null);
					GameCanvas.clearKeyHold();
					GameCanvas.clearKeyPressed();
					return false;
				}
				if (num14 < 60 && num15 < 40)
				{
					playerMenu(Char.myCharz().charFocus);
					if (!GameCanvas.isTouch && Char.myCharz().charFocus.charID >= 0 && TileMap.mapID != 51 && TileMap.mapID != 52 && popUpYesNo == null)
					{
						GameCanvas.panel.setTypePlayerMenu(Char.myCharz().charFocus);
						GameCanvas.panel.show();
						Service.gI().getPlayerMenu(Char.myCharz().charFocus.charID);
						Service.gI().messagePlayerMenu(Char.myCharz().charFocus.charID);
					}
				}
				else
				{
					int num19 = (20 + Res.r.nextInt(20)) * ((Char.myCharz().cx > Char.myCharz().charFocus.cx) ? 1 : (-1));
					Char.myCharz().currentMovePoint = new MovePoint(Char.myCharz().charFocus.cx + num19, Char.myCharz().charFocus.cy);
					Char.myCharz().endMovePointCommand = new Command(null, null, 8002, null);
					GameCanvas.clearKeyHold();
					GameCanvas.clearKeyPressed();
				}
				return false;
			}
			if (Char.myCharz().itemFocus != null)
			{
				pickItem();
				return false;
			}
			return true;
		}

	public bool isMeCanAttackMob(Mob m)
		{
			if (m == null)
			{
				return false;
			}
			if (Char.myCharz().cTypePk == 5)
			{
				return true;
			}
			if (Char.myCharz().isAttacPlayerStatus() && !m.isMobMe)
			{
				return false;
			}
			if (Char.myCharz().mobMe != null && m.Equals(Char.myCharz().mobMe))
			{
				return false;
			}
			Char @char = findCharInMap(m.mobId);
			if (@char == null)
			{
				return true;
			}
			if (@char.cTypePk == 5)
			{
				return true;
			}
			if (Char.myCharz().isMeCanAttackOtherPlayer(@char))
			{
				return true;
			}
			return false;
		}

	public void resetButton()
		{
			if (!ModMenu.modMenuOpen)
			{
				GameCanvas.menu.showMenu = false;
			}
			ChatTextField.gI().close();
			ChatTextField.gI().center = null;
			isLockKey = false;
			typeTrade = 0;
			indexMenu = 0;
			indexSelect = 0;
			indexItemUse = -1;
			indexRow = -1;
			indexRowMax = 0;
			indexTitle = 0;
			typeTrade = (typeTradeOrder = 0);
			mSystem.endKey();
			if (Char.myCharz().cHP <= 0 || Char.myCharz().statusMe == 14 || Char.myCharz().statusMe == 5)
			{
				if (Char.myCharz().meDead)
				{
					cmdDead = new Command(mResources.DIES[0], 11038);
					center = cmdDead;
					Char.myCharz().cHP = 0L;
				}
				isHaveSelectSkill = false;
			}
			else
			{
				isHaveSelectSkill = true;
			}
			scrMain.clear();
		}

	public bool isVsMap()
		{
			return true;
		}

	private void checkDrag()
		{
			if (isAnalog == 1 || gamePad.disableCheckDrag())
			{
				return;
			}
			Char.myCharz().cmtoChar = true;
			if (isUseTouch)
			{
				return;
			}
			if (GameCanvas.isPointerJustDown)
			{
				GameCanvas.isPointerJustDown = false;
				isPointerDowning = true;
				ptDownTime = 0;
				ptLastDownX = (ptFirstDownX = GameCanvas.px);
				ptLastDownY = (ptFirstDownY = GameCanvas.py);
			}
			if (isPointerDowning)
			{
				int num = GameCanvas.px - ptLastDownX;
				int num2 = GameCanvas.py - ptLastDownY;
				if (!isChangingCameraMode && (Res.abs(GameCanvas.px - ptFirstDownX) > 15 || Res.abs(GameCanvas.py - ptFirstDownY) > 15))
				{
					isChangingCameraMode = true;
				}
				ptLastDownX = GameCanvas.px;
				ptLastDownY = GameCanvas.py;
				ptDownTime++;
				if (isChangingCameraMode)
				{
					Char.myCharz().cmtoChar = false;
					cmx -= num;
					cmy -= num2;
					if (cmx < 24)
					{
						int num3 = (24 - cmx) / 3;
						if (num3 != 0)
						{
							cmx += num - num / num3;
						}
					}
					if (cmx < (isVsMap() ? 24 : 0))
					{
						cmx = (isVsMap() ? 24 : 0);
					}
					if (cmx > cmxLim)
					{
						int num4 = (cmx - cmxLim) / 3;
						if (num4 != 0)
						{
							cmx += num - num / num4;
						}
					}
					if (cmx > cmxLim + ((!isVsMap()) ? 24 : 0))
					{
						cmx = cmxLim + ((!isVsMap()) ? 24 : 0);
					}
					if (cmy < 0)
					{
						int num5 = -cmy / 3;
						if (num5 != 0)
						{
							cmy += num2 - num2 / num5;
						}
					}
					if (cmy < -((!isVsMap()) ? 24 : 0))
					{
						cmy = -((!isVsMap()) ? 24 : 0);
					}
					if (cmy > cmyLim)
					{
						cmy = cmyLim;
					}
					cmtoX = cmx;
					cmtoY = cmy;
				}
			}
			if (isPointerDowning && GameCanvas.isPointerJustRelease)
			{
				isPointerDowning = false;
				isChangingCameraMode = false;
				if (Res.abs(GameCanvas.px - ptFirstDownX) > 15 || Res.abs(GameCanvas.py - ptFirstDownY) > 15)
				{
					GameCanvas.isPointerJustRelease = false;
				}
			}
		}

	private bool inRectangle(int xClick, int yClick, int x, int y, int w, int h)
		{
			return xClick >= x && xClick <= x + w && yClick >= y && yClick <= y + h;
		}

	private void checkAuto()
		{
			long num = mSystem.currentTimeMillis();
			if (GameCanvas.keyPressed[(!Main.isPC) ? 2 : 21] || GameCanvas.keyPressed[(!Main.isPC) ? 4 : 23] || GameCanvas.keyPressed[(!Main.isPC) ? 6 : 24] || GameCanvas.keyPressed[1] || GameCanvas.keyPressed[3])
			{
				auto = 0;
				isAutoPlay = false;
			}
			if (GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25] && !isPaintPopup())
			{
				if (auto == 0)
				{
					if (num - lastFire < 800 && checkSkillValid2() && (Char.myCharz().mobFocus != null || (Char.myCharz().charFocus != null && Char.myCharz().isMeCanAttackOtherPlayer(Char.myCharz().charFocus))))
					{
						Res.outz("toi day");
						auto = 10;
						GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25] = false;
					}
				}
				else
				{
					auto = 0;
					GameCanvas.keyPressed[(!Main.isPC) ? 4 : 23] = (GameCanvas.keyPressed[(!Main.isPC) ? 6 : 24] = false);
				}
				lastFire = num;
			}
			if (GameCanvas.gameTick % 5 == 0 && auto > 0 && Char.myCharz().currentMovePoint == null)
			{
				if (Char.myCharz().myskill != null && (Char.myCharz().myskill.template.isUseAlone() || Char.myCharz().myskill.paintCanNotUseSkill))
				{
					return;
				}
				if ((Char.myCharz().mobFocus != null && Char.myCharz().mobFocus.status != 1 && Char.myCharz().mobFocus.status != 0 && Char.myCharz().charFocus == null) || (Char.myCharz().charFocus != null && Char.myCharz().isMeCanAttackOtherPlayer(Char.myCharz().charFocus)))
				{
					if (Char.myCharz().myskill.paintCanNotUseSkill)
					{
						return;
					}
					doFire(isFireByShortCut: false, skipWaypoint: true);
				}
			}
			if (auto > 1)
			{
				auto--;
			}
		}

	public void doUseHP()
		{
			if (Char.myCharz().stone || Char.myCharz().blindEff || Char.myCharz().holdEffID > 0)
			{
				return;
			}
			long num = mSystem.currentTimeMillis();
			if (num - lastUsePotion >= 10000)
			{
				if (!Char.myCharz().doUsePotion())
				{
					info1.addInfo(mResources.HP_EMPTY, 0);
					return;
				}
				ServerEffect.addServerEffect(11, Char.myCharz(), 5);
				ServerEffect.addServerEffect(104, Char.myCharz(), 4);
				lastUsePotion = num;
				SoundMn.gI().eatPeans();
			}
		}

	public void activeSuperPower(int x, int y)
		{
			if (!isSuperPower)
			{
				SoundMn.gI().bigeExlode();
				isSuperPower = true;
				tPower = 0;
				dxPower = 0;
				xPower = x - cmx;
				yPower = y - cmy;
			}
		}

	public void doiMauTroi()
		{
			isRongThanXuatHien = true;
			mautroi = mGraphics.blendColor(0.4f, 0, GameCanvas.colorTop[GameCanvas.colorTop.Length - 1]);
		}

	public void callRongThan(int x, int y)
		{
			Res.outz("VE RONG THAN O VI TRI x= " + x + " y=" + y);
			doiMauTroi();
			Effect me = new Effect((!isRongNamek) ? 17 : 25, x, y - 77, 2, -1, 1);
			EffecMn.addEff(me);
		}

	public void hideRongThan()
		{
			isRongThanXuatHien = false;
			EffecMn.removeEff(17);
			if (isRongNamek)
			{
				isRongNamek = false;
				EffecMn.removeEff(25);
			}
		}

	private void autoPlay()
		{
			if (timeSkill > 0)
			{
				timeSkill--;
			}
			if (!canAutoPlay || isChangeZone || Char.myCharz().statusMe == 14 || Char.myCharz().statusMe == 5 || Char.myCharz().isCharge || Char.myCharz().isFlyAndCharge || Char.myCharz().isUseChargeSkill())
			{
				return;
			}
			bool flag = false;
			for (int i = 0; i < vMob.size(); i++)
			{
				Mob mob = (Mob)vMob.elementAt(i);
				if (mob.status != 0 && mob.status != 1)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				return;
			}
			bool flag2 = false;
			for (int j = 0; j < Char.myCharz().arrItemBag.Length; j++)
			{
				Item item = Char.myCharz().arrItemBag[j];
				if (item != null && item.template.type == 6)
				{
					flag2 = true;
					break;
				}
			}
			if (!flag2 && GameCanvas.gameTick % 150 == 0)
			{
				Service.gI().requestPean();
			}
			if (Char.myCharz().cHP <= Char.myCharz().cHPFull * 20 / 100 || Char.myCharz().cMP <= Char.myCharz().cMPFull * 20 / 100)
			{
				doUseHP();
			}
			if (Char.myCharz().mobFocus == null || (Char.myCharz().mobFocus != null && Char.myCharz().mobFocus.isMobMe))
			{
				for (int k = 0; k < vMob.size(); k++)
				{
					Mob mob2 = (Mob)vMob.elementAt(k);
					if (mob2.status != 0 && mob2.status != 1 && mob2.hp > 0 && !mob2.isMobMe)
					{
						Char.myCharz().cx = mob2.x;
						Char.myCharz().cy = mob2.y;
						Char.myCharz().mobFocus = mob2;
						Service.gI().charMove();
						break;
					}
				}
			}
			else if (Char.myCharz().mobFocus.hp <= 0 || Char.myCharz().mobFocus.status == 1 || Char.myCharz().mobFocus.status == 0)
			{
				Char.myCharz().mobFocus = null;
			}
			if (Char.myCharz().mobFocus == null || timeSkill != 0 || (Char.myCharz().skillInfoPaint() != null && Char.myCharz().indexSkill < Char.myCharz().skillInfoPaint().Length && Char.myCharz().dart != null && Char.myCharz().arr != null))
			{
				return;
			}
			Skill skill = null;
			if (GameCanvas.isTouch)
			{
				for (int l = 0; l < onScreenSkill.Length; l++)
				{
					if (onScreenSkill[l] == null || onScreenSkill[l].paintCanNotUseSkill || onScreenSkill[l].template.id == 10 || onScreenSkill[l].template.id == 11 || onScreenSkill[l].template.id == 14 || onScreenSkill[l].template.id == 23 || onScreenSkill[l].template.id == 7 || Char.myCharz().skillInfoPaint() != null || onScreenSkill[l].template.isSkillSpec())
					{
						continue;
					}
					long num = 0L;
					num = ((onScreenSkill[l].template.manaUseType == 2) ? 1 : ((onScreenSkill[l].template.manaUseType == 1) ? (onScreenSkill[l].manaUse * Char.myCharz().cMPFull / 100) : onScreenSkill[l].manaUse));
					if (Char.myCharz().cMP >= num)
					{
						if (skill == null)
						{
							skill = onScreenSkill[l];
						}
						else if (skill.coolDown < onScreenSkill[l].coolDown)
						{
							skill = onScreenSkill[l];
						}
					}
				}
				if (skill != null)
				{
					doSelectSkill(skill, isShortcut: true);
					doDoubleClickToObj(Char.myCharz().mobFocus);
				}
				return;
			}
			for (int m = 0; m < keySkill.Length; m++)
			{
				if (keySkill[m] == null || keySkill[m].paintCanNotUseSkill || keySkill[m].template.id == 10 || keySkill[m].template.id == 11 || keySkill[m].template.id == 14 || keySkill[m].template.id == 23 || keySkill[m].template.id == 7 || Char.myCharz().skillInfoPaint() != null)
				{
					continue;
				}
				long num2 = 0L;
				num2 = ((keySkill[m].template.manaUseType == 2) ? 1 : ((keySkill[m].template.manaUseType == 1) ? (keySkill[m].manaUse * Char.myCharz().cMPFull / 100) : keySkill[m].manaUse));
				if (Char.myCharz().cMP >= num2)
				{
					if (skill == null)
					{
						skill = keySkill[m];
					}
					else if (skill.coolDown < keySkill[m].coolDown)
					{
						skill = keySkill[m];
					}
				}
			}
			if (skill != null)
			{
				doSelectSkill(skill, isShortcut: true);
				doDoubleClickToObj(Char.myCharz().mobFocus);
			}
		}

	private void askToPick()
		{
			Npc npc = new Npc(5, 0, -100, 100, 5, info1.charId[Char.myCharz().cgender][2]);
			string nhatvatpham = mResources.nhatvatpham;
			string[] menu = new string[2]
			{
				mResources.YES,
				mResources.NO
			};
			npc.idItem = 673;
			gI().createMenu(menu, npc);
			ChatPopup.addChatPopupWithIcon(nhatvatpham, 100000, npc, 5820);
		}

	private void pickItem()
		{
			if (Char.myCharz().itemFocus == null)
			{
				return;
			}
			if (Char.myCharz().cx < Char.myCharz().itemFocus.x)
			{
				Char.myCharz().cdir = 1;
			}
			else
			{
				Char.myCharz().cdir = -1;
			}
			int num = Math.abs(Char.myCharz().cx - Char.myCharz().itemFocus.x);
			int num2 = Math.abs(Char.myCharz().cy - Char.myCharz().itemFocus.y);
			if (num <= 40 && num2 < 40)
			{
				GameCanvas.clearKeyHold();
				GameCanvas.clearKeyPressed();
				if (Char.myCharz().itemFocus.template.id != 673)
				{
					Service.gI().pickItem(Char.myCharz().itemFocus.itemMapID);
				}
				else
				{
					askToPick();
				}
			}
			else
			{
				Char.myCharz().currentMovePoint = new MovePoint(Char.myCharz().itemFocus.x, Char.myCharz().itemFocus.y);
				Char.myCharz().endMovePointCommand = new Command(null, null, 8002, null);
				GameCanvas.clearKeyHold();
				GameCanvas.clearKeyPressed();
			}
		}

	public bool isCharging()
		{
			if (Char.myCharz().isFlyAndCharge || Char.myCharz().isUseSkillAfterCharge || Char.myCharz().isStandAndCharge || Char.myCharz().isWaitMonkey || isSuperPower || Char.myCharz().isFreez)
			{
				return true;
			}
			return false;
		}

	public void checkCharFocus()
		{
		}

	public static Npc findNPCInMap(short id)
		{
			for (int i = 0; i < vNpc.size(); i++)
			{
				Npc npc = (Npc)vNpc.elementAt(i);
				if (npc.template.npcTemplateId == id)
				{
					return npc;
				}
			}
			return null;
		}

	public static Char findCharInMap(int charId)
		{
			for (int i = 0; i < vCharInMap.size(); i++)
			{
				Char @char = (Char)vCharInMap.elementAt(i);
				if (@char.charID == charId)
				{
					return @char;
				}
			}
			return null;
		}

	public static Mob findMobInMap(sbyte mobIndex)
		{
			return (Mob)vMob.elementAt(mobIndex);
		}

	public static Mob findMobInMap(int mobId)
		{
			for (int i = 0; i < vMob.size(); i++)
			{
				Mob mob = (Mob)vMob.elementAt(i);
				if (mob.mobId == mobId)
				{
					return mob;
				}
			}
			return null;
		}

	public static Npc getNpcTask()
		{
			for (int i = 0; i < vNpc.size(); i++)
			{
				Npc npc = (Npc)vNpc.elementAt(i);
				if (npc.template.npcTemplateId == getTaskNpcId())
				{
					return npc;
				}
			}
			return null;
		}

	public void getInjure()
		{
		}

	public void starVS()
		{
			curr = (last = mSystem.currentTimeMillis());
			secondVS = 180;
		}

	private Char findCharVS1()
		{
			for (int i = 0; i < vCharInMap.size(); i++)
			{
				Char @char = (Char)vCharInMap.elementAt(i);
				if (@char.cTypePk != 0)
				{
					return @char;
				}
			}
			return null;
		}

	private Char findCharVS2()
		{
			for (int i = 0; i < vCharInMap.size(); i++)
			{
				Char @char = (Char)vCharInMap.elementAt(i);
				if (@char.cTypePk != 0 && @char != findCharVS1())
				{
					return @char;
				}
			}
			return null;
		}

	public bool isVS()
		{
			if (TileMap.isVoDaiMap() && (Char.myCharz().cTypePk != 0 || (TileMap.mapID == 130 && findCharVS1() != null && findCharVS2() != null)))
			{
				return true;
			}
			return false;
		}

	private void loadInforBar()
		{
			imgScrW = 84;
			hpBarW = 66L;
			mpBarW = 59;
			hpBarX = 52;
			hpBarY = 10;
			spBarW = 61;
			expBarW = gW - 61;
		}

	public bool isPaintUI()
		{
			if (isPaintStore || isPaintWeapon || isPaintNonNam || isPaintNonNu || isPaintAoNam || isPaintAoNu || isPaintGangTayNam || isPaintGangTayNu || isPaintQuanNam || isPaintQuanNu || isPaintGiayNam || isPaintGiayNu || isPaintLien || isPaintNhan || isPaintNgocBoi || isPaintPhu || isPaintStack || isPaintStackLock || isPaintGrocery || isPaintGroceryLock || isPaintUpGrade || isPaintConvert || isPaintSplit || isPaintUpPearl || isPaintBox || isPaintTrade)
			{
				return true;
			}
			return false;
		}

	public bool isOpenUI()
		{
			if (ModMenu.uiCustomOpen)
			{
				return true;
			}
			if (isPaintItemInfo || isPaintInfoMe || isPaintStore || isPaintNonNam || isPaintNonNu || isPaintAoNam || isPaintAoNu || isPaintGangTayNam || isPaintGangTayNu || isPaintQuanNam || isPaintQuanNu || isPaintGiayNam || isPaintGiayNu || isPaintLien || isPaintNhan || isPaintNgocBoi || isPaintPhu || isPaintWeapon || isPaintStack || isPaintStackLock || isPaintGrocery || isPaintGroceryLock || isPaintUpGrade || isPaintConvert || isPaintUpPearl || isPaintBox || isPaintSplit || isPaintTrade)
			{
				return true;
			}
			return false;
		}

	public static void loadImg()
		{
			TileMap.loadTileImage();
		}

	public static int getTaskMapId()
		{
			int num = 0;
			if (Char.myCharz().taskMaint == null)
			{
				return -1;
			}
			return mapTasks[Char.myCharz().taskMaint.index];
		}

	public static sbyte getTaskNpcId()
		{
			sbyte result = 0;
			if (Char.myCharz().taskMaint == null)
			{
				result = -1;
			}
			else if (Char.myCharz().taskMaint.index <= tasks.Length - 1)
			{
				result = (sbyte)tasks[Char.myCharz().taskMaint.index];
			}
			return result;
		}

	public void refreshTeam()
		{
		}

	public void openWeb(string strLeft, string strRight, string url, string title, string str)
		{
			isPaintAlert = true;
			isLockKey = true;
			indexRow = 0;
			setPopupSize(175, 200);
			textsTitle = title;
			texts = mFont.tahoma_7.splitFontVector(str, popupW - 30);
			center = null;
			left = new Command(strLeft, 11068, url);
			right = new Command(strRight, 11069);
		}

	public void sendSms(string strLeft, string strRight, short port, string syntax, string title, string str)
		{
			isPaintAlert = true;
			isLockKey = true;
			indexRow = 0;
			setPopupSize(175, 200);
			textsTitle = title;
			texts = mFont.tahoma_7.splitFontVector(str, popupW - 30);
			center = null;
			MyVector myVector = new MyVector();
			myVector.addElement(string.Empty + port);
			myVector.addElement(syntax);
			left = new Command(strLeft, 11074);
			right = new Command(strRight, 11075);
		}

	public void openUIZone(Message message)
		{
			InfoDlg.hide();
			try
			{
				zones = new int[message.reader().readByte()];
				pts = new int[zones.Length];
				numPlayer = new int[zones.Length];
				maxPlayer = new int[zones.Length];
				rank1 = new int[zones.Length];
				rankName1 = new string[zones.Length];
				rank2 = new int[zones.Length];
				rankName2 = new string[zones.Length];
				for (int i = 0; i < zones.Length; i++)
				{
					zones[i] = message.reader().readByte();
					pts[i] = message.reader().readByte();
					numPlayer[i] = message.reader().readByte();
					maxPlayer[i] = message.reader().readByte();
					sbyte b = message.reader().readByte();
					if (b == 1)
					{
						rankName1[i] = message.reader().readUTF();
						rank1[i] = message.reader().readInt();
						rankName2[i] = message.reader().readUTF();
						rank2[i] = message.reader().readInt();
					}
				}
			}
			catch (Exception ex)
			{
				Cout.LogError("Loi ham OPEN UIZONE " + ex.ToString());
			}
			GameCanvas.panel.setTypeZone();
			GameCanvas.panel.show();
		}

	public void showViewInfo()
		{
			indexMenu = 3;
			isPaintInfoMe = true;
			setPopupSize(175, 200);
		}

	private void actDead()
		{
			MyVector myVector = new MyVector();
			myVector.addElement(new Command(mResources.DIES[1], 110381));
			myVector.addElement(new Command(mResources.DIES[2], 110382));
			myVector.addElement(new Command(mResources.DIES[3], 110383));
			GameCanvas.menu.startAt(myVector, 3);
		}

	public void getFlagImage(int charID, sbyte cflag)
		{
			if (vFlag.size() == 0)
			{
				Service.gI().getFlag(2, cflag);
				Res.outz("getFlag1");
				return;
			}
			if (charID == Char.myCharz().charID)
			{
				Res.outz("my cflag: isme");
				if (Char.myCharz().isGetFlagImage(cflag))
				{
					Res.outz("my cflag: true");
					for (int i = 0; i < vFlag.size(); i++)
					{
						PKFlag pKFlag = (PKFlag)vFlag.elementAt(i);
						if (pKFlag != null && pKFlag.cflag == cflag)
						{
							Res.outz("my cflag: cflag==");
							Char.myCharz().flagImage = pKFlag.IDimageFlag;
						}
					}
				}
				else if (!Char.myCharz().isGetFlagImage(cflag))
				{
					Res.outz("my cflag: false");
					Service.gI().getFlag(2, cflag);
				}
				return;
			}
			Res.outz("my cflag: not me");
			if (findCharInMap(charID) == null)
			{
				return;
			}
			if (findCharInMap(charID).isGetFlagImage(cflag))
			{
				Res.outz("my cflag: true");
				for (int j = 0; j < vFlag.size(); j++)
				{
					PKFlag pKFlag2 = (PKFlag)vFlag.elementAt(j);
					if (pKFlag2 != null && pKFlag2.cflag == cflag)
					{
						Res.outz("my cflag: cflag==");
						findCharInMap(charID).flagImage = pKFlag2.IDimageFlag;
					}
				}
			}
			else if (!findCharInMap(charID).isGetFlagImage(cflag))
			{
				Res.outz("my cflag: false");
				Service.gI().getFlag(2, cflag);
			}
		}

	public void showWinNumber(string num, string finish)
		{
			winnumber = new int[num.Length];
			randomNumber = new int[num.Length];
			tMove = new int[num.Length];
			moveCount = new int[num.Length];
			delayMove = new int[num.Length];
			try
			{
				for (int i = 0; i < num.Length; i++)
				{
					winnumber[i] = short.Parse(num[i].ToString());
					randomNumber[i] = Res.random(0, 11);
					tMove[i] = 1;
					delayMove[i] = 0;
				}
			}
			catch (Exception)
			{
			}
			tShow = 100;
			moveIndex = 0;
			strFinish = finish;
			lastXS = (currXS = mSystem.currentTimeMillis());
		}

	public void showYourNumber(string strNum)
		{
			yourNumber = strNum;
			strPaint = mFont.tahoma_7.splitFontArray(yourNumber, 500);
		}

	public static void checkRemoveImage()
		{
			ImgByName.checkDelHash(ImgByName.hashImagePath, 10, isTrue: false);
		}

	public static bool ispaintPhubangBar()
		{
			if (TileMap.mapPhuBang() && phuban_Info.type_PB == 0)
			{
				return true;
			}
			return false;
		}

	public static bool setIsInScreen(int x, int y, int wOne, int hOne)
		{
			if (x < cmx - wOne || x > cmx + GameCanvas.w + wOne || y < cmy - hOne || y > cmy + GameCanvas.h + hOne * 3 / 2)
			{
				return false;
			}
			return true;
		}

	public static bool isSmallScr()
		{
			if (GameCanvas.w <= 320)
			{
				return true;
			}
			return false;
		}

}
