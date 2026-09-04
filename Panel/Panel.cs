using System;
using System.Collections.Generic;
using Assets.src.g;
using UnityEngine;
public partial class Panel : IActionListener, IChatable
{
	public class PlayerChat
		{
			public string name;

			public int charID;

			public bool isNewMessage;

			public List<InfoItem> chats = new List<InfoItem>();

			public PlayerChat(string name, int charId)
			{
				this.name = name;
				charID = charId;
				isNewMessage = true;
			}
		}

	public bool isShow;

	public int X;

	public int Y;

	public int W;

	public int H;

	public int ITEM_HEIGHT;

	public int TAB_W;

	public int TAB_W_NEW;

	public int cmtoY;

	public int cmy;

	public int cmdy;

	public int cmvy;

	public int cmyLim;

	public int xc;

	public int[] cmyLast;

	public int cmtoX;

	public int cmx;

	public int cmxLim;

	public int cmxMap;

	public int cmyMap;

	public int cmxMapLim;

	public int cmyMapLim;

	public int cmyQuest;

	public static Image imgBantay;

	public static Image imgX;

	public static Image imgMap;

	public TabClanIcon tabIcon;

	public MyVector vItemCombine = new MyVector();

	public int moneyGD;

	public int friendMoneyGD;

	public bool isLock;

	public bool isFriendLock;

	public bool isAccept;

	public bool isFriendAccep;

	public string topName;

	public ChatTextField chatTField;

	public static string specialInfo;

	public static short spearcialImage;

	public static Image imgStar;

	public static Image imgMaxStar;

	public static Image imgStar8;

	public static Image imgStar9;

	public static Image imgStarCuongHoa;

	public static Image imgNew;

	public static Image imgXu;

	public static Image imgTicket;

	public static Image imgLuong;

	public static Image imgLuongKhoa;

	private static Image imgUp;

	private static Image imgDown;

	private int pa1;

	private int pa2;

	private bool trans;

	private int pX;

	private int pY;

	private Command left = new Command(mResources.SELECT, 0);

	public int type;

	public int currentTabIndex;

	public int startTabPos;

	public int[] lastTabIndex;

	public string[][] currentTabName;

	private int[] currClanOption;

	public int mainTabPos = 4;

	public int shopTabPos = 50;

	public int boxTabPos = 50;

	public string[][] mainTabName;

	public string[] mapNames;

	public string[] planetNames;

	public static string[] strTool = new string[7]
		{
			mResources.gameInfo,
			mResources.change_flag,
			mResources.change_zone,
			mResources.chat_world,
			mResources.account,
			mResources.option,
			mResources.change_account
		};

	public static string[] strCauhinh = new string[4]
		{
			(!GameCanvas.isPlaySound) ? mResources.turnOnSound : mResources.turnOffSound,
			mResources.increase_vga,
			mResources.analog,
			(mGraphics.zoomLevel <= 1) ? mResources.x2Screen : mResources.x1Screen
		};

	public static string[] strAccount = new string[5]
		{
			mResources.inventory_Pass,
			mResources.friend,
			mResources.enemy,
			mResources.msg,
			mResources.charger
		};

	public static string[] strAuto = new string[1] { mResources.useGem };

	public static int graphics = 0;

	public string[][] shopTabName;

	public int[] maxPageShop;

	public int[] currPageShop;

	private static string[][] boxTabName = new string[2][]
		{
			mResources.chestt,
			mResources.inventory
		};

	private static string[][] boxCombine = new string[2][]
		{
			mResources.combine,
			mResources.inventory
		};

	private static string[][] boxZone = new string[1][] { mResources.zonee };

	private static string[][] boxMap = new string[1][] { mResources.mapp };

	private static string[][] boxGD = new string[3][]
		{
			mResources.inventory,
			mResources.item_give,
			mResources.item_receive
		};

	private static string[][] boxPet = mResources.petMainTab;

	public string[][][] tabName = new string[27][][]
		{
			null,
			null,
			boxTabName,
			boxZone,
			boxMap,
			null,
			null,
			new string[1][] { new string[1] { string.Empty } },
			new string[1][] { new string[1] { string.Empty } },
			new string[1][] { new string[1] { string.Empty } },
			new string[1][] { new string[1] { string.Empty } },
			new string[1][] { new string[1] { string.Empty } },
			boxCombine,
			boxGD,
			new string[1][] { new string[1] { string.Empty } },
			new string[1][] { new string[1] { string.Empty } },
			new string[1][] { new string[1] { string.Empty } },
			new string[1][] { new string[1] { string.Empty } },
			new string[1][] { new string[1] { string.Empty } },
			new string[1][] { new string[1] { string.Empty } },
			new string[1][] { new string[1] { string.Empty } },
			boxPet,
			new string[1][] { new string[1] { string.Empty } },
			new string[1][] { new string[1] { string.Empty } },
			new string[1][] { new string[1] { string.Empty } },
			new string[1][] { new string[1] { string.Empty } },
			new string[1][] { new string[1] { string.Empty } }
		};

	private static sbyte BOX_BAG = 0;

	private static sbyte BAG_BOX = 1;

	private static sbyte BOX_BODY = 2;

	private static sbyte BODY_BOX = 3;

	private static sbyte BAG_BODY = 4;

	private static sbyte BODY_BAG = 5;

	private static sbyte BAG_PET = 6;

	private static sbyte PET_BAG = 7;

	public int hasUse;

	public int hasUseBag;

	public int currentListLength;

	private int[] lastSelect;

	public static int[] mapIdTraidat = new int[16]
		{
			21, 0, 1, 2, 24, 3, 4, 5, 6, 27,
			28, 29, 30, 42, 47, 46
		};

	public static int[] mapXTraidat = new int[16]
		{
			39, 42, 105, 93, 61, 93, 142, 165, 210, 100,
			165, 220, 233, 10, 125, 125
		};

	public static int[] mapYTraidat = new int[16]
		{
			28, 60, 48, 96, 88, 131, 136, 95, 32, 200,
			189, 167, 120, 110, 20, 20
		};

	public static int[] mapIdNamek = new int[14]
		{
			22, 7, 8, 9, 25, 11, 12, 13, 10, 31,
			32, 33, 34, 43
		};

	public static int[] mapXNamek = new int[14]
		{
			55, 30, 93, 80, 24, 149, 219, 220, 233, 170,
			148, 195, 148, 10
		};

	public static int[] mapYNamek = new int[14]
		{
			136, 84, 69, 34, 25, 42, 32, 110, 192, 70,
			106, 156, 210, 57
		};

	public static int[] mapIdSaya = new int[14]
		{
			23, 14, 15, 16, 26, 17, 18, 20, 19, 35,
			36, 37, 38, 44
		};

	public static int[] mapXSaya = new int[14]
		{
			90, 95, 144, 234, 231, 122, 176, 158, 205, 54,
			105, 159, 231, 27
		};

	public static int[] mapYSaya = new int[14]
		{
			10, 43, 20, 36, 69, 87, 112, 167, 160, 151,
			173, 207, 194, 29
		};

	public static int[][] mapId = new int[3][] { mapIdTraidat, mapIdNamek, mapIdSaya };

	public static int[][] mapX = new int[3][] { mapXTraidat, mapXNamek, mapXSaya };

	public static int[][] mapY = new int[3][] { mapYTraidat, mapYNamek, mapYSaya };

	public Item currItem;

	public Clan currClan;

	public ClanMessage currMess;

	public Member currMem;

	public Clan[] clans;

	public MyVector member;

	public MyVector myMember;

	public MyVector logChat = new MyVector();

	public MyVector vPlayerMenu = new MyVector();

	public MyVector vFriend = new MyVector();

	public MyVector vMyGD = new MyVector();

	public MyVector vFriendGD = new MyVector();

	public MyVector vTop = new MyVector();

	public MyVector vEnemy = new MyVector();

	public MyVector vFlag = new MyVector();

	public MyVector vPlayerMenu_id = new MyVector();

	public Command cmdClose;

	public static bool CanNapTien = false;

	public static int WIDTH_PANEL = 240;

	private int position;

	public string playerChat;

	public Dictionary<string, PlayerChat> chats = new Dictionary<string, PlayerChat>();

	public Char charMenu;

	private bool isThachDau;

	public int typeShop = -1;

	public int xScroll;

	public int yScroll;

	public int wScroll;

	public int hScroll;

	public ChatPopup cp;

	public int idIcon;

	public int[] partID;

	private int timeShow;

	public bool isBoxClan;

	public int w;

	private int pa;

	public int selected;

	private int cSelected;

	private int newSelected;

	private bool isClanOption;

	public bool isSearchClan;

	public bool isMessage;

	public bool isViewMember;

	public const int TYPE_MAIN = 0;

	public const int TYPE_SHOP = 1;

	public const int TYPE_BOX = 2;

	public const int TYPE_ZONE = 3;

	public const int TYPE_MAP = 4;

	public const int TYPE_CLANS = 5;

	public const int TYPE_INFOMATION = 6;

	public const int TYPE_BODY = 7;

	public const int TYPE_MESS = 8;

	public const int TYPE_ARCHIVEMENT = 9;

	public const int PLAYER_MENU = 10;

	public const int TYPE_FRIEND = 11;

	public const int TYPE_COMBINE = 12;

	public const int TYPE_GIAODICH = 13;

	public const int TYPE_MAPTRANS = 14;

	public const int TYPE_TOP = 15;

	public const int TYPE_ENEMY = 16;

	public const int TYPE_KIGUI = 17;

	public const int TYPE_FLAG = 18;

	public const int TYPE_OPTION = 19;

	public const int TYPE_ACCOUNT = 20;

	public const int TYPE_PET_MAIN = 21;

	public const int TYPE_AUTO = 22;

	public const int TYPE_GAMEINFO = 23;

	public const int TYPE_GAMEINFOSUB = 24;

	public const int TYPE_SPEACIALSKILL = 25;

	private int pointerDownTime;

	private int pointerDownFirstX;

	private int[] pointerDownLastX = new int[3];

	private bool pointerIsDowning;

	private bool isDownWhenRunning;

	private bool wantUpdateList;

	private int waitToPerform;

	private int cmRun;

	private int keyTouchLock = -1;

	private int keyToundGD = -1;

	private int keyTouchCombine = -1;

	private int keyTouchMapButton = -1;

	public int indexMouse = -1;

	private bool justRelease;

	private int keyTouchTab = -1;

	private int nTableItem;

	public string[][] clansOption = new string[2][]
		{
			mResources.findClan,
			mResources.createClan
		};

	public string clanInfo = string.Empty;

	public string clanReport = string.Empty;

	private bool isHaveClan;

	private Scroll scroll;

	private int cmvx;

	private int cmdx;

	private bool isSelectPlayerMenu;

	private string[] strStatus = new string[6]
		{
			mResources.follow,
			mResources.defend,
			mResources.attack,
			mResources.gohome,
			mResources.fusion,
			mResources.fusionForever
		};

	private static string log;

	private int tt;

	private int currentButtonPress;

	public static long[] t_tiemnang = new long[14]
		{
			50000000L, 250000000L, 1250000000L, 5000000000L, 15000000000L, 30000000000L, 45000000000L, 60000000000L, 75000000000L, 90000000000L,
			110000000000L, 130000000000L, 150000000000L, 170000000000L
		};

	private int[] zoneColor = new int[3] { 43520, 14743570, 14155776 };

	public string[] combineInfo;

	public string[] combineTopInfo;

	public static int[] color1 = new int[3] { 2327248, 8982199, 16713222 };

	public static int[] color2 = new int[3] { 4583423, 16719103, 16714764 };

	private int sellectInventory;

	private Item itemInvenNew;

	private Effect eBanner;

	private static FrameImage screenTab6;

	private bool isUp;

	private int compare;

	public static string strWantToBuy = string.Empty;

	public int xstart;

	public int ystart;

	public int popupW = 140;

	public int popupH = 160;

	public int cmySK;

	public int cmtoYSK;

	public int cmdySK;

	public int cmvySK;

	public int cmyLimSK;

	public int popupY;

	public int popupX;

	public int isborderIndex;

	public int isselectedRow;

	public int indexSize = 28;

	public int indexTitle;

	public int indexSelect;

	public int indexRow = -1;

	public int indexRowMax;

	public int indexMenu;

	public int columns = 6;

	public int rows;

	public int inforX;

	public int inforY;

	public int inforW;

	public int inforH;

	private int yPaint;

	private int xMap;

	private int yMap;

	private int xMapTask;

	private int yMapTask;

	private int xMove;

	private int yMove;

	public static bool isPaintMap = true;

	public bool isClose;

	private int infoSelect;

	public static MyVector vGameInfo = new MyVector(string.Empty);

	public static string[] contenInfo;

	public bool isViewChatServer;

	private int currInfoItem;

	public Char charInfo;

	private bool isChangeZone;

	private bool isKiguiXu;

	private bool isKiguiLuong;

	private int delayKigui;

	public sbyte combineSuccess = -1;

	public int idNPC;

	public int xS;

	public int yS;

	private int rS;

	private int angleS;

	private int angleO;

	private int iAngleS;

	private int iDotS;

	private int speed;

	private int[] xArgS;

	private int[] yArgS;

	private int[] xDotS;

	private int[] yDotS;

	private int time;

	private int typeCombine;

	private int countUpdate;

	private int countR;

	private int countWait;

	private bool isSpeedCombine;

	private bool isCompleteEffCombine = true;

	private bool isPaintCombine;

	public bool isDoneCombine = true;

	public short iconID1;

	public short iconID2;

	public short iconID3;

	public short[] iconID;

	public string[][] speacialTabName;

	public static int[] sizeUpgradeEff = new int[3] { 2, 1, 1 };

	public static int nsize = 1;

	public const sbyte COLOR_WHITE = 0;

	public const sbyte COLOR_GREEN = 1;

	public const sbyte COLOR_PURPLE = 2;

	public const sbyte COLOR_ORANGE = 3;

	public const sbyte COLOR_BLUE = 4;

	public const sbyte COLOR_YELLOW = 5;

	public const sbyte COLOR_RED = 6;

	public const sbyte COLOR_BLACK = 7;

	public static int[][] colorUpgradeEffect = new int[7][]
		{
			new int[6] { 16777215, 15000805, 13487823, 11711155, 9671828, 7895160 },
			new int[6] { 61952, 58624, 52224, 45824, 39168, 32768 },
			new int[6] { 13500671, 12058853, 10682572, 9371827, 7995545, 6684800 },
			new int[6] { 16744192, 15037184, 13395456, 11753728, 10046464, 8404992 },
			new int[6] { 37119, 33509, 28108, 24499, 21145, 17536 },
			new int[6] { 16776192, 15063040, 12635136, 11776256, 10063872, 8290304 },
			new int[6] { 16711680, 15007744, 13369344, 11730944, 10027008, 8388608 }
		};

	public const int color_item_white = 15987701;

	public const int color_item_green = 2786816;

	public const int color_item_purple = 7078041;

	public const int color_item_orange = 12537346;

	public const int color_item_blue = 1269146;

	public const int color_item_yellow = 13279744;

	public const int color_item_red = 11599872;

	public const int color_item_black = 2039326;

	private Image imgo_0;

	private Image imgo_1;

	private Image imgo_2;

	private Image imgo_3;

	public const int numItem = 20;

	public const sbyte INVENTORY_TAB = 1;

	public sbyte size_tab;

	private bool isnewInventory;



































}
