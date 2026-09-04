using System;
using Assets.src.e;
using Assets.src.g;
using UnityEngine;
public partial class GameCanvas : IActionListener
{
	public static long timeNow = 0L;

	public static bool open3Hour;

	public static bool lowGraphic = false;

	public static bool serverchat = false;

	public static bool isMoveNumberPad = true;

	public static bool isLoading;

	public static bool isTouch = false;

	public static bool isTouchControl;

	public static bool isTouchControlSmallScreen;

	public static bool isTouchControlLargeScreen;

	public static bool isConnectFail;

	public static GameCanvas instance;

	public static bool bRun;

	public static bool[] keyPressed = new bool[30];

	public static bool[] keyReleased = new bool[30];

	public static bool[] keyHold = new bool[30];

	public static bool isPointerDown;

	public static bool isPointerClick;

	public static bool isPointerJustRelease;

	public static bool isPointerSelect;

	public static bool isPointerMove;

	public static int px;

	public static int py;

	public static int pxFirst;

	public static int pyFirst;

	public static int pxLast;

	public static int pyLast;

	public static int pxMouse;

	public static int pyMouse;

	public static Position[] arrPos = new Position[4];

	public static int gameTick;

	public static int taskTick;

	public static bool isEff1;

	public static bool isEff2;

	public static long timeTickEff1;

	public static long timeTickEff2;

	public static int w;

	public static int h;

	public static int hw;

	public static int hh;

	public static int wd3;

	public static int hd3;

	public static int w2d3;

	public static int h2d3;

	public static int w3d4;

	public static int h3d4;

	public static int wd6;

	public static int hd6;

	public static mScreen currentScreen;

	public static Menu menu = new Menu();

	public static Panel panel;

	public static Panel panel2;

	public static ChooseCharScr chooseCharScr;

	public static LoginScr loginScr;

	public static RegisterScreen registerScr;

	public static Dialog currentDialog;

	public static MsgDlg msgdlg;

	public static InputDlg inputDlg;

	public static MyVector currentPopup = new MyVector();

	public static int requestLoseCount;

	public static MyVector listPoint;

	public static Paint paintz;

	public static bool isGetResFromServer;

	public static Image[] imgBG;

	public static int skyColor;

	public static int curPos = 0;

	public static int[] bgW;

	public static int[] bgH;

	public static int planet = 0;

	private mGraphics g = new mGraphics();

	public static Image img18;

	public static Image[] imgBlue = new Image[7];

	public static Image[] imgViolet = new Image[7];

	public static MyHashTable danhHieu = new MyHashTable();

	public static MyVector messageServer = new MyVector(string.Empty);

	public static bool isPlaySound = true;

	private static int clearOldData;

	public static int timeOpenKeyBoard;

	public static bool isFocusPanel2;

	public static int fps = 0;

	public static int max;

	public static int up;

	public static int upmax;

	private long timefps = mSystem.currentTimeMillis() + 1000;

	private long timeup = mSystem.currentTimeMillis() + 1000;

	public static int isRequestMapID = -1;

	public static long waitingTimeChangeMap;

	private static int dir_ = -1;

	private int tickWaitThongBao;

	public bool isPaintCarret;

	public static MyVector debugUpdate;

	public static MyVector debugPaint;

	public static MyVector debugSession;

	private static bool isShowErrorForm = false;

	public static bool paintBG;

	public static int gsskyHeight;

	public static int gsgreenField1Y;

	public static int gsgreenField2Y;

	public static int gshouseY;

	public static int gsmountainY;

	public static int bgLayer0y;

	public static int bgLayer1y;

	public static Image imgCloud;

	public static Image imgSun;

	public static Image imgSun2;

	public static Image imgClear;

	public static Image[] imgBorder = new Image[3];

	public static Image[] imgSunSpec = new Image[3];

	public static int borderConnerW;

	public static int borderConnerH;

	public static int borderCenterW;

	public static int borderCenterH;

	public static int[] cloudX;

	public static int[] cloudY;

	public static int sunX;

	public static int sunY;

	public static int sunX2;

	public static int sunY2;

	public static int[] layerSpeed;

	public static int[] moveX;

	public static int[] moveXSpeed;

	public static bool isBoltEff;

	public static bool boltActive;

	public static int tBolt;

	public static Image imgBgIOS;

	public static int typeBg = -1;

	public static int transY;

	public static int[] yb = new int[5];

	public static int[] colorTop;

	public static int[] colorBotton;

	public static int yb1;

	public static int yb2;

	public static int yb3;

	public static int nBg = 0;

	public static int lastBg = -1;

	public static int[] bgRain = new int[3] { 1, 4, 11 };

	public static int[] bgRainFont = new int[1] { -1 };

	public static Image imgCaycot;

	public static Image tam;

	public static int typeBackGround = -1;

	public static int saveIDBg = -10;

	public static bool isLoadBGok;

	private static long lastTimePress = 0L;

	public static int keyAsciiPress;

	public static int pXYScrollMouse;

	private static Image imgSignal;

	public static MyVector flyTexts = new MyVector();

	public int longTime;

	public static long timeBreakLoading;

	private static string thongBaoTest;

	public static int xThongBaoTranslate = w - 60;

	public static bool isPointerJustDown = false;

	private int count = 1;

	public static bool csWait;

	public static MyRandom r = new MyRandom();

	public static bool isBlackScreen;

	public static int[] bgSpeed;

	public static int cmdBarX;

	public static int cmdBarY;

	public static int cmdBarW;

	public static int cmdBarH;

	public static int cmdBarLeftW;

	public static int cmdBarRightW;

	public static int cmdBarCenterW;

	public static int hpBarX;

	public static int hpBarY;

	public static int hpBarW;

	public static int expBarW;

	public static int lvPosX;

	public static int moneyPosX;

	public static int hpBarH;

	public static int girlHPBarY;

	public int timeOut;

	public int[] dustX;

	public int[] dustY;

	public int[] dustState;

	public static int[] wsX;

	public static int[] wsY;

	public static int[] wsState;

	public static int[] wsF;

	public static Image[] imgWS;

	public static Image imgShuriken;

	public static Image[][] imgDust;

	public static bool isResume;

	public static ServerListScreen serverScreen;

	public static ServerScr serverScr;

	public static SelectCharScr _SelectCharScr;

	public bool resetToLoginScr;

	public static long TIMEOUT;

	public static int timeLoading = 15;







































	private static Image cachedCustomLogo = null;











}
