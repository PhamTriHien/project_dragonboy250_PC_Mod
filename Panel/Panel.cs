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

	public Panel()
		{
			init();
			cmdClose = new Command(string.Empty, this, 1003, null);
			cmdClose.img = GameCanvas.loadImage("/mainImage/myTexture2dbtX.png");
			cmdClose.cmdClosePanel = true;
			currItem = null;
		}

	public static void loadBg()
		{
			imgMap = GameCanvas.loadImage("/img/map" + TileMap.planetID + ".png");
			imgBantay = GameCanvas.loadImage("/mainImage/myTexture2dbantay.png");
			imgX = GameCanvas.loadImage("/mainImage/myTexture2dbtX.png");
			imgXu = GameCanvas.loadImage("/mainImage/myTexture2dimgMoney.png");
			imgLuong = GameCanvas.loadImage("/mainImage/myTexture2dimgDiamond.png");
			imgLuongKhoa = GameCanvas.loadImage("/mainImage/luongkhoa.png");
			imgUp = GameCanvas.loadImage("/mainImage/myTexture2dup.png");
			imgDown = GameCanvas.loadImage("/mainImage/myTexture2ddown.png");
			imgStar = GameCanvas.loadImage("/mainImage/star.png");
			imgMaxStar = GameCanvas.loadImage("/mainImage/starE.png");
			imgStar8 = GameCanvas.loadImage("/mainImage/star8.png");
			imgStar9 = mSystem.loadImage("/mainImage/star9.png");
			imgStarCuongHoa = mSystem.loadImage("/mainImage/starCH.png");
			imgNew = GameCanvas.loadImage("/mainImage/new.png");
			imgTicket = GameCanvas.loadImage("/mainImage/ticket12.png");
		}

	public void init()
		{
			pX = GameCanvas.pxLast + cmxMap;
			pY = GameCanvas.pyLast + cmyMap;
			lastTabIndex = new int[tabName.Length];
			for (int i = 0; i < lastTabIndex.Length; i++)
			{
				lastTabIndex[i] = -1;
			}
		}


	public void show()
		{
			if (GameCanvas.isTouch)
			{
				cmdClose.x = 156;
				cmdClose.y = 3;
			}
			else
			{
				cmdClose.x = GameCanvas.w - 19;
				cmdClose.y = GameCanvas.h - 19;
			}
			cmdClose.isPlaySoundButton = false;
			ChatPopup.currChatPopup = null;
			InfoDlg.hide();
			timeShow = 20;
			isShow = true;
			isClose = false;
			SoundMn.gI().panelOpen();
			if (isTypeShop())
			{
				Char.myCharz().setPartOld();
			}
		}


	public void moveCamera()
		{
			if (timeShow > 0)
			{
				timeShow--;
			}
			if (justRelease && Equals(GameCanvas.panel) && typeShop == 2 && maxPageShop[currentTabIndex] > 1)
			{
				if (cmy < -50)
				{
					InfoDlg.showWait();
					justRelease = false;
					if (currPageShop[currentTabIndex] <= 0)
					{
						Service.gI().kigui(4, -1, (sbyte)currentTabIndex, maxPageShop[currentTabIndex] - 1, -1);
					}
					else
					{
						Service.gI().kigui(4, -1, (sbyte)currentTabIndex, currPageShop[currentTabIndex] - 1, -1);
					}
				}
				else if (cmy > cmyLim + 50)
				{
					justRelease = false;
					InfoDlg.showWait();
					if (currPageShop[currentTabIndex] >= maxPageShop[currentTabIndex] - 1)
					{
						Service.gI().kigui(4, -1, (sbyte)currentTabIndex, 0, -1);
					}
					else
					{
						Service.gI().kigui(4, -1, (sbyte)currentTabIndex, currPageShop[currentTabIndex] + 1, -1);
					}
				}
			}
			if (cmx != cmtoX && !pointerIsDowning)
			{
				cmvx = cmtoX - cmx << 2;
				cmdx += cmvx;
				cmx += cmdx >> 3;
				cmdx &= 15;
			}
			if (Math.abs(cmtoX - cmx) < 10)
			{
				cmx = cmtoX;
			}
			if (isClose)
			{
				isClose = false;
				cmtoX = wScroll;
			}
			if (cmtoX >= wScroll - 10 && cmx >= wScroll - 10 && position == 0)
			{
				isShow = false;
				cleanCombine();
				if (isChangeZone)
				{
					isChangeZone = false;
					if (Char.myCharz().cHP > 0 && Char.myCharz().statusMe != 14)
					{
						InfoDlg.showWait();
						if (type == 3)
						{
							Service.gI().requestChangeZone(selected, -1);
						}
						else if (type == 14)
						{
							Service.gI().requestMapSelect(selected);
						}
					}
				}
				if (isSelectPlayerMenu)
				{
					isSelectPlayerMenu = false;
					int num = vPlayerMenu.size() - vPlayerMenu_id.size();
					if (Char.myCharz().charFocus != null)
					{
						if (selected - num < 0)
						{
							Char.myCharz().charFocus.menuSelect = selected;
						}
						else
						{
							Char.myCharz().charFocus.menuSelect = short.Parse((string)vPlayerMenu_id.elementAt(selected - num));
						}
					}
					Command command = (Command)vPlayerMenu.elementAt(selected);
					command.performAction();
				}
				vPlayerMenu.removeAllElements();
				vPlayerMenu_id.removeAllElements();
				charMenu = null;
			}
			if (cmRun != 0 && !pointerIsDowning)
			{
				cmtoY += cmRun / 100;
				if (cmtoY < 0)
				{
					cmtoY = 0;
				}
				else if (cmtoY > cmyLim)
				{
					cmtoY = cmyLim;
				}
				else
				{
					cmy = cmtoY;
				}
				cmRun = cmRun * 9 / 10;
				if (cmRun < 100 && cmRun > -100)
				{
					cmRun = 0;
				}
			}
			if (cmy != cmtoY && !pointerIsDowning)
			{
				cmvy = cmtoY - cmy << 2;
				cmdy += cmvy;
				cmy += cmdy >> 4;
				cmdy &= 15;
			}
			cmyLast[currentTabIndex] = cmy;
		}

	public Member getCurrMember()
		{
			if (selected < 2)
			{
				return null;
			}
			if (selected > ((member == null) ? myMember.size() : member.size()) + 1)
			{
				return null;
			}
			return (member == null) ? ((Member)myMember.elementAt(selected - 2)) : ((Member)member.elementAt(selected - 2));
		}

	public ClanMessage getCurrMessage()
		{
			if (selected < 2)
			{
				return null;
			}
			if (selected > ClanMessage.vMessage.size() + 1)
			{
				return null;
			}
			return (ClanMessage)ClanMessage.vMessage.elementAt(selected - 2);
		}

	public Clan getCurrClan()
		{
			if (selected < 2)
			{
				return null;
			}
			if (selected > clans.Length + 1)
			{
				return null;
			}
			return clans[selected - 2];
		}

	public int getCompare(Item item)
		{
			if (item == null)
			{
				return -1;
			}
			if (item.isTypeBody())
			{
				if (item.itemOption == null)
				{
					return -1;
				}
				ItemOption itemOption = item.itemOption[0];
				if (itemOption.optionTemplate.id == 22)
				{
					itemOption.optionTemplate = GameScr.gI().iOptionTemplates[6];
					itemOption.param *= 1000;
				}
				if (itemOption.optionTemplate.id == 23)
				{
					itemOption.optionTemplate = GameScr.gI().iOptionTemplates[7];
					itemOption.param *= 1000;
				}
				Item item2 = null;
				for (int i = 0; i < Char.myCharz().arrItemBody.Length; i++)
				{
					Item item3 = Char.myCharz().arrItemBody[i];
					if (itemOption.optionTemplate.id == 22)
					{
						itemOption.optionTemplate = GameScr.gI().iOptionTemplates[6];
						itemOption.param *= 1000;
					}
					if (itemOption.optionTemplate.id == 23)
					{
						itemOption.optionTemplate = GameScr.gI().iOptionTemplates[7];
						itemOption.param *= 1000;
					}
					if (item3 != null && item3.itemOption != null && item3.template.type == item.template.type)
					{
						item2 = item3;
						break;
					}
				}
				if (item2 == null)
				{
					isUp = true;
					return itemOption.param;
				}
				int num = 0;
				num = ((item2 == null || item2.itemOption == null) ? itemOption.param : (itemOption.param - item2.itemOption[0].param));
				if (num < 0)
				{
					isUp = false;
				}
				else
				{
					isUp = true;
				}
				return num;
			}
			return 0;
		}

	private string getStatus(int status)
		{
			return status switch
			{
				0 => mResources.follow, 
				1 => mResources.defend, 
				2 => mResources.attack, 
				3 => mResources.gohome, 
				_ => "aaa", 
			};
		}

	public void hideNow()
		{
			if (timeShow > 0)
			{
				isClose = false;
				return;
			}
			cp = null;
			if (isTypeShop() || TileMap.mapID == 45)
			{
				Char.myCharz().resetPartTemp();
			}
			if (chatTField != null && type == 13 && chatTField.isShow)
			{
				chatTField = null;
			}
			if (type == 13 && !isAccept)
			{
				Service.gI().giaodich(3, -1, -1, -1);
			}
			Res.outz("HIDE PANELLLLLLLLLLLLLLLLLLLLLL");
			SoundMn.gI().buttonClose();
			GameScr.isPaint = true;
			TileMap.lastPlanetId = -1;
			imgMap = null;
			mSystem.gcc();
			isClanOption = false;
			isClose = true;
			cleanCombine();
			Hint.clickNpc();
			GameCanvas.panel2 = null;
			GameCanvas.clearAllPointerEvent();
			GameCanvas.clearKeyPressed();
			pointerDownTime = (pointerDownFirstX = 0);
			pointerIsDowning = false;
			isShow = false;
			if ((Char.myCharz().cHP <= 0 || Char.myCharz().statusMe == 14 || Char.myCharz().statusMe == 5) && Char.myCharz().meDead)
			{
				Command center = new Command(mResources.DIES[0], 11038, GameScr.gI());
				GameScr.gI().center = center;
				Char.myCharz().cHP = 0L;
			}
		}

	public void hide()
		{
			if (timeShow > 0)
			{
				isClose = false;
				return;
			}
			cp = null;
			if (isTypeShop() || TileMap.mapID == 45)
			{
				Char.myCharz().resetPartTemp();
			}
			if (chatTField != null && type == 13 && chatTField.isShow)
			{
				chatTField = null;
			}
			if (type == 13 && !isAccept)
			{
				Service.gI().giaodich(3, -1, -1, -1);
			}
			if (type == 15)
			{
				Service.gI().sendThachDau(-1);
			}
			SoundMn.gI().buttonClose();
			GameScr.isPaint = true;
			TileMap.lastPlanetId = -1;
			if (imgMap != null)
			{
				imgMap.texture = null;
				imgMap = null;
			}
			mSystem.gcc();
			isClanOption = false;
			if (type != 4)
			{
				if (type == 24)
				{
					setTypeGameInfo();
				}
				else if (type == 23)
				{
					setTypeMain();
				}
				else if (type == 3 || type == 14)
				{
					if (isChangeZone)
					{
						isClose = true;
					}
					else
					{
						setTypeMain();
						cmx = (cmtoX = 0);
					}
				}
				else if (type == 18 || type == 19 || type == 20 || type == 21)
				{
					setTypeMain();
					cmx = (cmtoX = 0);
				}
				else if (type == 8 || type == 11 || type == 16)
				{
					setTypeAccount();
					cmx = (cmtoX = 0);
				}
				else
				{
					isClose = true;
				}
			}
			else
			{
				setTypeMain();
				cmx = (cmtoX = 0);
			}
			Hint.clickNpc();
			GameCanvas.panel2 = null;
			GameCanvas.clearAllPointerEvent();
			GameCanvas.clearKeyPressed();
			GameCanvas.isFocusPanel2 = false;
			pointerDownTime = (pointerDownFirstX = 0);
			pointerIsDowning = false;
			if ((Char.myCharz().cHP <= 0 || Char.myCharz().statusMe == 14 || Char.myCharz().statusMe == 5) && Char.myCharz().meDead)
			{
				Command center = new Command(mResources.DIES[0], 11038, GameScr.gI());
				GameScr.gI().center = center;
				Char.myCharz().cHP = 0L;
			}
		}

	private void doSpeacialSkill()
		{
		}

	private void doRada()
		{
			hide();
			if (RadarScr.list == null || RadarScr.list.size() == 0)
			{
				Service.gI().SendRada(0, -1);
				RadarScr.gI().switchToMe();
			}
			else
			{
				RadarScr.gI().switchToMe();
			}
		}


	public void putMoney()
		{
			if (chatTField == null)
			{
				chatTField = new ChatTextField();
				chatTField.tfChat.y = GameCanvas.h - 35 - ChatTextField.gI().tfChat.height;
				chatTField.initChatTextField();
				chatTField.parentScreen = GameCanvas.panel;
			}
			chatTField.strChat = mResources.input_money_to_trade;
			chatTField.tfChat.name = mResources.input_money;
			chatTField.to = string.Empty;
			chatTField.isShow = true;
			chatTField.tfChat.setIputType(TField.INPUT_TYPE_NUMERIC);
			chatTField.tfChat.setMaxTextLenght(10);
			if (GameCanvas.isTouch)
			{
				chatTField.tfChat.doChangeToTextBox();
			}
			if (Main.isWindowsPhone)
			{
				chatTField.tfChat.strInfo = chatTField.strChat;
			}
			if (!Main.isPC)
			{
				chatTField.startChat2(this, string.Empty);
			}
		}

	public void putQuantily()
		{
			if (chatTField == null)
			{
				chatTField = new ChatTextField();
				chatTField.tfChat.y = GameCanvas.h - 35 - ChatTextField.gI().tfChat.height;
				chatTField.initChatTextField();
				chatTField.parentScreen = GameCanvas.panel;
			}
			chatTField.strChat = mResources.input_quantity_to_trade;
			chatTField.tfChat.name = mResources.input_quantity;
			chatTField.to = string.Empty;
			chatTField.isShow = true;
			chatTField.tfChat.setIputType(TField.INPUT_TYPE_NUMERIC);
			if (GameCanvas.isTouch)
			{
				chatTField.tfChat.doChangeToTextBox();
			}
			if (Main.isWindowsPhone)
			{
				chatTField.tfChat.strInfo = chatTField.strChat;
			}
			if (!Main.isPC)
			{
				chatTField.startChat2(this, string.Empty);
			}
		}

	public void chagenSlogan()
		{
			chatTField.strChat = mResources.input_clan_slogan;
			chatTField.tfChat.name = mResources.input_clan_slogan;
			chatTField.to = string.Empty;
			chatTField.isShow = true;
			chatTField.tfChat.isFocus = true;
			chatTField.tfChat.setIputType(TField.INPUT_TYPE_ANY);
			if (Main.isWindowsPhone)
			{
				chatTField.tfChat.strInfo = chatTField.strChat;
			}
			if (!Main.isPC)
			{
				chatTField.startChat2(this, string.Empty);
			}
		}

	public void changeIcon()
		{
			if (tabIcon == null)
			{
				tabIcon = new TabClanIcon();
			}
			tabIcon.text = chatTField.tfChat.getText();
			tabIcon.show(isGetName: false);
			chatTField.isShow = false;
		}

	private void addFriend(InfoItem info)
		{
			string text = "|0|1|" + info.charInfo.cName;
			text += "\n";
			text = ((!info.isOnline) ? (text + "|3|1|" + mResources.is_offline) : (text + "|4|1|" + mResources.is_online));
			text += "\n--";
			string text2 = text;
			text = text2 + "\n|5|" + mResources.power + ": " + info.s;
			cp = new ChatPopup();
			popUpDetailInit(cp, text);
			charInfo = info.charInfo;
			currItem = null;
		}

	private void addLogMessage(InfoItem info)
		{
			string text = "|0|1|" + info.charInfo.cName;
			text += "\n";
			text += "\n--";
			text = text + "\n|5|" + Res.split(info.s, "|", 0)[2];
			cp = new ChatPopup();
			popUpDetailInit(cp, text);
			charInfo = info.charInfo;
			currItem = null;
		}

	private void addSkillDetail2(int type)
		{
			string empty = string.Empty;
			int num = 0;
			if (selected == 0)
			{
				num = Char.myCharz().cHPGoc + 1000;
			}
			if (selected == 1)
			{
				num = Char.myCharz().cMPGoc + 1000;
			}
			if (selected == 2)
			{
				num = Char.myCharz().cDamGoc * Char.myCharz().expForOneAdd;
			}
			if (selected == 3)
			{
				num = 500000 + Char.myCharz().cDefGoc * 100000;
			}
			string text = empty;
			empty = text + "|5|2|" + mResources.USE + " " + num + " " + mResources.potential;
			if (type == 0)
			{
				empty = empty + "\n|5|2|" + mResources.to_gain_20hp;
			}
			if (type == 1)
			{
				empty = empty + "\n|5|2|" + mResources.to_gain_20mp;
			}
			if (type == 2)
			{
				empty = empty + "\n|5|2|" + mResources.to_gain_1pow;
			}
			if (type == 3)
			{
				empty = empty + "\n|5|2|" + mResources.to_gain_1pow;
			}
			currItem = null;
			partID = null;
			charInfo = null;
			idIcon = -1;
			cp = new ChatPopup();
			popUpDetailInit(cp, empty);
		}

	public void itemRequest(sbyte itemAction, string info, sbyte where, sbyte index)
		{
			GameCanvas.endDlg();
			ItemObject itemObject = new ItemObject();
			itemObject.type = itemAction;
			itemObject.id = index;
			itemObject.where = where;
			GameCanvas.startYesNoDlg(info, new Command(mResources.YES, this, 2004, itemObject), new Command(mResources.NO, this, 4005, null));
		}

	public void saleRequest(sbyte type, string info, short id)
		{
			ItemObject itemObject = new ItemObject();
			itemObject.type = type;
			itemObject.id = id;
			GameCanvas.startYesNoDlg(info, new Command(mResources.YES, this, 3003, itemObject), new Command(mResources.NO, this, 4005, null));
		}

	private void setDotStar()
		{
			for (int i = 0; i < yArgS.Length; i++)
			{
				if (angleS >= 360)
				{
					angleS -= 360;
				}
				if (angleS < 0)
				{
					angleS = 360 + angleS;
				}
				yArgS[i] = Res.abs(rS * Res.sin(angleS) / 1024);
				xArgS[i] = Res.abs(rS * Res.cos(angleS) / 1024);
				if (angleS < 90)
				{
					xDotS[i] = xS + xArgS[i];
					yDotS[i] = yS - yArgS[i];
				}
				else if (angleS >= 90 && angleS < 180)
				{
					xDotS[i] = xS - xArgS[i];
					yDotS[i] = yS - yArgS[i];
				}
				else if (angleS >= 180 && angleS < 270)
				{
					xDotS[i] = xS - xArgS[i];
					yDotS[i] = yS + yArgS[i];
				}
				else
				{
					xDotS[i] = xS + xArgS[i];
					yDotS[i] = yS + yArgS[i];
				}
				angleS -= iAngleS;
			}
		}


	private void doNotiRuby(int type)
		{
			try
			{
				currItem.buyRuby = int.Parse(chatTField.tfChat.getText());
			}
			catch (Exception)
			{
				GameCanvas.startOKDlg(mResources.input_money_wrong);
				chatTField.isShow = false;
				return;
			}
			Command cmdYes = new Command(mResources.YES, this, (type != 0) ? 11001 : 11000, null);
			Command cmdNo = new Command(mResources.NO, this, 11002, null);
			GameCanvas.startYesNoDlg(mResources.notiRuby, cmdYes, cmdNo);
		}


	private bool isTabInven()
		{
			if ((type == 0 && currentTabIndex == 1) || (type == 7 && currentTabIndex == 0))
			{
				return true;
			}
			return false;
		}

	private bool IsTabOption()
		{
			if (size_tab > 0)
			{
				if (currentTabName.Length > 1)
				{
					if (selected == 0)
					{
						return true;
					}
				}
				else if (selected >= 0)
				{
					return true;
				}
			}
			return false;
		}

	private int checkCurrentListLength(int arrLength)
		{
			int num = 20;
			int num2 = arrLength / 20 + ((arrLength % 20 > 0) ? 1 : 0);
			size_tab = (sbyte)num2;
			if (newSelected > num2 - 1)
			{
				newSelected = num2 - 1;
			}
			if (arrLength % 20 > 0 && newSelected == num2 - 1)
			{
				num = arrLength % 20;
			}
			return num + 1;
		}

}
