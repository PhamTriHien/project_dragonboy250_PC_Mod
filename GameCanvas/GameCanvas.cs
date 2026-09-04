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

	public GameCanvas()
		{
			switch (Rms.loadRMSInt("languageVersion"))
			{
			case -1:
				Rms.saveRMSInt("languageVersion", 2);
				break;
			default:
				Main.main.doClearRMS();
				Rms.saveRMSInt("languageVersion", 2);
				break;
			case 2:
				break;
			}
			clearOldData = Rms.loadRMSInt(GameMidlet.VERSION);
			if (clearOldData != 1)
			{
				Main.main.doClearRMS();
				Rms.saveRMSInt(GameMidlet.VERSION, 1);
			}
			initGame();
		}

	public static string getPlatformName()
		{
			string text = Rms.loadRMSString("sys_dev_id");
			if (string.IsNullOrEmpty(text) || text.Equals("Pc platform xxx") || text.Equals("n/a"))
			{
				try
				{
					text = SystemInfo.deviceUniqueIdentifier;
				}
				catch
				{
					text = Guid.NewGuid().ToString("N");
				}
				if (string.IsNullOrEmpty(text) || text.Equals("n/a") || text.Equals("Pc platform xxx"))
				{
					text = Guid.NewGuid().ToString("N");
				}
				Rms.saveRMSString("sys_dev_id", text);
			}
			return "PC_" + (text.Length > 16 ? text.Substring(0, 16) : text);
		}

	public void initGame()
		{
			try
			{
				MotherCanvas.instance.setChildCanvas(this);
				w = MotherCanvas.instance.getWidthz();
				h = MotherCanvas.instance.getHeightz();
				hw = w / 2;
				hh = h / 2;
				isTouch = true;
				if (w >= 240)
				{
					isTouchControl = true;
				}
				if (w < 320)
				{
					isTouchControlSmallScreen = true;
				}
				if (w >= 320)
				{
					isTouchControlLargeScreen = true;
				}
				msgdlg = new MsgDlg();
				if (h <= 160)
				{
					Paint.hTab = 15;
					mScreen.cmdH = 17;
				}
				GameScr.d = ((w <= h) ? h : w) + 20;
				instance = this;
				mFont.init();
				mScreen.ITEM_HEIGHT = mFont.tahoma_8b.getHeight() + 8;
				initPaint();
				loadDust();
				loadWaterSplash();
				panel = new Panel();
				imgShuriken = loadImage("/mainImage/myTexture2df.png");
				int num = Rms.loadRMSInt(Rms.RMS_clienttype);
				if (num != -1)
				{
					mSystem.clientType = num;
				}
				if (mSystem.clientType == 7 && (Rms.loadRMSString("fake") == null || Rms.loadRMSString("fake") == string.Empty))
				{
					imgShuriken = loadImage("/mainImage/wait.png");
				}
				imgClear = loadImage("/mainImage/myTexture2der.png");
				img18 = loadImage("/mainImage/18+.png");
				debugUpdate = new MyVector();
				debugPaint = new MyVector();
				debugSession = new MyVector();
				for (int i = 0; i < 3; i++)
				{
					imgBorder[i] = loadImage("/mainImage/myTexture2dbd" + i + ".png");
				}
				borderConnerW = mGraphics.getImageWidth(imgBorder[0]);
				borderConnerH = mGraphics.getImageHeight(imgBorder[0]);
				borderCenterW = mGraphics.getImageWidth(imgBorder[1]);
				borderCenterH = mGraphics.getImageHeight(imgBorder[1]);
				Panel.graphics = Rms.loadRMSInt("lowGraphic");
				lowGraphic = Rms.loadRMSInt("lowGraphic") == 1;
				GameScr.isPaintChatVip = Rms.loadRMSInt("serverchat") != 1;
				Char.isPaintAura = Rms.loadRMSInt("isPaintAura") == 1;
				Char.isPaintAura2 = Rms.loadRMSInt("isPaintAura2") == 1;
				Res.init();
				SmallImage.loadBigImage();
				Panel.WIDTH_PANEL = 176;
				if (Panel.WIDTH_PANEL > w)
				{
					Panel.WIDTH_PANEL = w;
				}
				InfoMe.gI().loadCharId();
				Command.btn0left = loadImage("/mainImage/btn0left.png");
				Command.btn0mid = loadImage("/mainImage/btn0mid.png");
				Command.btn0right = loadImage("/mainImage/btn0right.png");
				Command.btn1left = loadImage("/mainImage/btn1left.png");
				Command.btn1mid = loadImage("/mainImage/btn1mid.png");
				Command.btn1right = loadImage("/mainImage/btn1right.png");
				serverScreen = new ServerListScreen();
				img18 = loadImage("/mainImage/18+.png");
				for (int j = 0; j < 7; j++)
				{
					imgBlue[j] = loadImage("/effectdata/blue/" + j + ".png");
					imgViolet[j] = loadImage("/effectdata/violet/" + j + ".png");
				}
				ServerListScreen.createDeleteRMS();
				serverScr = new ServerScr();
				loginScr = new LoginScr();
				_SelectCharScr = new SelectCharScr();
			}
			catch (Exception)
			{
				Debug.LogError("----------------->>>>>>>>>>errr");
			}
		}

	public static GameCanvas gI()
		{
			return instance;
		}

	public void initPaint()
		{
			paintz = new Paint();
		}

	public void onDisconnected()
		{
			if (Controller.isConnectionFail)
			{
				Controller.isConnectionFail = false;
			}
			isResume = true;
			Session_ME.gI().clearSendingMessage();
			Session_ME2.gI().clearSendingMessage();
			Session_ME.gI().close();
			Session_ME2.gI().close();
			if (Controller.isLoadingData)
			{
				startOK(mResources.pls_restart_game_error, 8885, null);
				Controller.isDisconnected = false;
				return;
			}
			Debug.LogError(">>>>onDisconnected");
			if (currentScreen != serverScreen)
			{
				serverScreen.switchToMe();
				startOK(mResources.maychutathoacmatsong + " [4]", 8884, null);
				ServerListScreen.waitToLogin = true;
				ServerListScreen.tWaitToLogin = 0;
			}
			else
			{
				endDlg();
			}
			Char.isLoadingMap = false;
			if (Controller.isMain)
			{
				ServerListScreen.testConnect = 0;
			}
			mSystem.endKey();
		}

	public void onConnectionFail()
		{
			if (currentScreen.Equals(SplashScr.instance))
			{
				startOK(mResources.maychutathoacmatsong + " [1]", 8884, null);
				return;
			}
			Session_ME.gI().clearSendingMessage();
			Session_ME2.gI().clearSendingMessage();
			ServerListScreen.isWait = false;
			if (Controller.isLoadingData)
			{
				startOK(mResources.maychutathoacmatsong + " [2]", 8884, null);
				Controller.isConnectionFail = false;
				return;
			}
			isResume = true;
			LoginScr.isContinueToLogin = false;
			if (ServerListScreen.nameServer != null && ServerListScreen.ipSelect >= 0 && ServerListScreen.ipSelect < ServerListScreen.nameServer.Length)
			{
				LoginScr.serverName = ServerListScreen.nameServer[ServerListScreen.ipSelect];
			}
			if (currentScreen != serverScreen)
			{
				ServerListScreen.countDieConnect = 0;
			}
			else
			{
				endDlg();
				ServerListScreen.loadScreen = true;
				serverScreen.switchToMe();
			}
			Char.isLoadingMap = false;
			if (Controller.isMain)
			{
				ServerListScreen.testConnect = 0;
			}
			mSystem.endKey();
		}

	public static bool isWaiting()
		{
			if (InfoDlg.isShow || (msgdlg != null && msgdlg.info.Equals(mResources.PLEASEWAIT)) || Char.isLoadingMap || LoginScr.isContinueToLogin)
			{
				return true;
			}
			return false;
		}

	public static void connect()
		{
			if (!Session_ME.gI().isConnected())
			{
				Session_ME.gI().connect(GameMidlet.IP, GameMidlet.PORT);
			}
		}

	public static void connect2()
		{
			if (!Session_ME2.gI().isConnected())
			{
				Res.outz("IP2= " + GameMidlet.IP2 + " PORT 2= " + GameMidlet.PORT2);
				Session_ME2.gI().connect(GameMidlet.IP2, GameMidlet.PORT2);
			}
		}

	public static void resetTrans(mGraphics g)
		{
			g.translate(-g.getTranslateX(), -g.getTranslateY());
			g.setClip(0, 0, w, h);
		}

	public static void resetTransGameScr(mGraphics g)
		{
			g.translate(-g.getTranslateX(), -g.getTranslateY());
			g.translate(0, 0);
			g.setClip(0, 0, w, h);
			g.translate(-GameScr.cmx, -GameScr.cmy);
		}

	public void initGameCanvas()
		{
			debug("SP2i1", 0);
			w = MotherCanvas.instance.getWidthz();
			h = MotherCanvas.instance.getHeightz();
			debug("SP2i2", 0);
			hw = w / 2;
			hh = h / 2;
			wd3 = w / 3;
			hd3 = h / 3;
			w2d3 = 2 * w / 3;
			h2d3 = 2 * h / 3;
			w3d4 = 3 * w / 4;
			h3d4 = 3 * h / 4;
			wd6 = w / 6;
			hd6 = h / 6;
			debug("SP2i3", 0);
			mScreen.initPos();
			debug("SP2i4", 0);
			debug("SP2i5", 0);
			inputDlg = new InputDlg();
			debug("SP2i6", 0);
			listPoint = new MyVector();
			debug("SP2i7", 0);
		}

	public void start()
		{
		}

	public int getWidth()
		{
			return (int)ScaleGUI.WIDTH;
		}

	public int getHeight()
		{
			return (int)ScaleGUI.HEIGHT;
		}

	public static void debug(string s, int type)
		{
		}

	public void doResetToLoginScr(mScreen screen)
		{
			try
			{
				SoundMn.gI().stopAll();
				LoginScr.isContinueToLogin = false;
				TileMap.lastType = (TileMap.bgType = 0);
				Char.clearMyChar();
				GameScr.clearGameScr();
				GameScr.resetAllvector();
				InfoDlg.hide();
				GameScr.info1.hide();
				GameScr.info2.hide();
				GameScr.info2.cmdChat = null;
				Hint.isShow = false;
				ChatPopup.currChatPopup = null;
				Controller.isStopReadMessage = false;
				GameScr.loadCamera(fullmScreen: true, -1, -1);
				GameScr.cmx = 100;
				panel.currentTabIndex = 0;
				panel.selected = (isTouch ? (-1) : 0);
				panel.init();
				panel2 = null;
				GameScr.isPaint = true;
				ClanMessage.vMessage.removeAllElements();
				GameScr.textTime.removeAllElements();
				GameScr.vClan.removeAllElements();
				GameScr.vFriend.removeAllElements();
				GameScr.vEnemies.removeAllElements();
				TileMap.vCurrItem.removeAllElements();
				BackgroudEffect.vBgEffect.removeAllElements();
				EffecMn.vEff.removeAllElements();
				Effect.newEff.removeAllElements();
				menu.showMenu = false;
				panel.vItemCombine.removeAllElements();
				panel.isShow = false;
				if (panel.tabIcon != null)
				{
					panel.tabIcon.isShow = false;
				}
				if (mGraphics.zoomLevel == 1)
				{
					SmallImage.clearHastable();
				}
				Session_ME.gI().close();
				Session_ME2.gI().close();
			}
			catch (Exception ex)
			{
				Cout.println("Loi tai doResetToLoginScr " + ex.ToString());
			}
			ServerListScreen.isAutoConect = true;
			ServerListScreen.countDieConnect = 0;
			ServerListScreen.testConnect = -1;
			ServerListScreen.loadScreen = true;
			ServerListScreen.count_reConnect = 0;
			ServerListScreen.waitToLogin = false;
			ServerListScreen.tWaitToLogin = 0;
			ServerListScreen.isWait = false;
			LoginScr.timeLogin = 0;
			if (ServerListScreen.ipSelect == -1)
			{
				serverScr.switchToMe();
				return;
			}
			if (serverScreen == null)
			{
				serverScreen = new ServerListScreen();
			}
			serverScreen.switchToMe();
		}

	public static void fillRect(mGraphics g, int color, int x, int y, int w, int h, int detalY)
		{
			g.setColor(color);
			int cmy = GameScr.cmy;
			if (cmy > GameCanvas.h)
			{
				cmy = GameCanvas.h;
			}
			g.fillRect(x, y - ((detalY != 0) ? (cmy >> detalY) : 0), w, h + ((detalY != 0) ? (cmy >> detalY) : 0));
		}

	public static bool isHDVersion()
		{
			if (mGraphics.zoomLevel > 1)
			{
				return true;
			}
			return false;
		}

	public static void getYBackground(int typeBg)
		{
			try
			{
				int gH = GameScr.gH23;
				switch (typeBg)
				{
				case 0:
					yb[0] = gH - bgH[0] + 70;
					yb[1] = yb[0] - bgH[1] + 20;
					yb[2] = yb[1] - bgH[2] + 30;
					yb[3] = yb[2] - bgH[3] + 50;
					break;
				case 1:
					yb[0] = gH - bgH[0] + 120;
					yb[1] = yb[0] - bgH[1] + 40;
					yb[2] = yb[1] - 90;
					yb[3] = yb[2] - 25;
					break;
				case 2:
					yb[0] = gH - bgH[0] + 150;
					yb[1] = yb[0] - bgH[1] - 60;
					yb[2] = yb[1] - bgH[2] - 40;
					yb[3] = yb[2] - bgH[3] - 10;
					yb[4] = yb[3] - bgH[4];
					break;
				case 3:
					yb[0] = gH - bgH[0] + 10;
					yb[1] = yb[0] + 80;
					yb[2] = yb[1] - bgH[2] - 10;
					break;
				case 4:
					yb[0] = gH - bgH[0] + 130;
					yb[1] = yb[0] - bgH[1];
					yb[2] = yb[1] - bgH[2] - 20;
					yb[3] = yb[1] - bgH[2] - 80;
					break;
				case 5:
					yb[0] = gH - bgH[0] + 40;
					yb[1] = yb[0] - bgH[1] + 10;
					yb[2] = yb[1] - bgH[2] + 15;
					yb[3] = yb[2] - bgH[3] + 50;
					break;
				case 6:
					yb[0] = gH - bgH[0] + 100;
					yb[1] = yb[0] - bgH[1] - 30;
					yb[2] = yb[1] - bgH[2] + 10;
					yb[3] = yb[2] - bgH[3] + 15;
					yb[4] = yb[3] - bgH[4] + 15;
					break;
				case 7:
					yb[0] = gH - bgH[0] + 20;
					yb[1] = yb[0] - bgH[1] + 15;
					yb[2] = yb[1] - bgH[2] + 20;
					yb[3] = yb[1] - bgH[2] - 10;
					break;
				case 8:
					yb[0] = gH - 103 + 150;
					if (TileMap.mapID == 103)
					{
						yb[0] -= 100;
					}
					yb[1] = yb[0] - bgH[1] - 10;
					yb[2] = yb[1] - bgH[2] + 40;
					yb[3] = yb[2] - bgH[3] + 10;
					break;
				case 9:
					yb[0] = gH - bgH[0] + 100;
					yb[1] = yb[0] - bgH[1] + 22;
					yb[2] = yb[1] - bgH[2] + 50;
					yb[3] = yb[2] - bgH[3];
					break;
				case 10:
					yb[0] = gH - bgH[0] - 45;
					yb[1] = yb[0] - bgH[1] - 10;
					break;
				case 11:
					yb[0] = gH - bgH[0] + 60;
					yb[1] = yb[0] - bgH[1] + 5;
					yb[2] = yb[1] - bgH[2] - 15;
					break;
				case 12:
					yb[0] = gH + 40;
					yb[1] = yb[0] - 40;
					yb[2] = yb[1] - 40;
					break;
				case 13:
					yb[0] = gH - 80;
					yb[1] = yb[0];
					break;
				case 15:
					yb[0] = gH - 20;
					yb[1] = yb[0] - 80;
					break;
				case 16:
					yb[0] = gH - bgH[0] + 75;
					yb[1] = yb[0] - bgH[1] + 50;
					yb[2] = yb[1] - bgH[2] + 50;
					yb[3] = yb[2] - bgH[3] + 90;
					break;
				case 19:
					yb[0] = gH - bgH[0] + 150;
					yb[1] = yb[0] - bgH[1] - 60;
					yb[2] = yb[1] - bgH[2] - 40;
					yb[3] = yb[2] - bgH[3] - 10;
					yb[4] = yb[3] - bgH[4];
					break;
				default:
					yb[0] = gH - bgH[0] + 75;
					yb[1] = yb[0] - bgH[1] + 50;
					yb[2] = yb[1] - bgH[2] + 50;
					yb[3] = yb[2] - bgH[3] + 90;
					break;
				}
			}
			catch (Exception)
			{
				int gH2 = GameScr.gH23;
				for (int i = 0; i < yb.Length; i++)
				{
					yb[i] = 1;
				}
			}
		}

	private static void randomRaintEff(int typeBG)
		{
			for (int i = 0; i < bgRain.Length; i++)
			{
				if (typeBG == bgRain[i] && Res.random(0, 2) == 0)
				{
					BackgroudEffect.addEffect(0);
					break;
				}
			}
		}

	public static bool isPointSelect(int x, int y, int w, int h)
		{
			if (!isPointerSelect)
			{
				return false;
			}
			if (px >= x && px <= x + w && py >= y && py <= y + h)
			{
				return true;
			}
			return false;
		}

	public static void checkBackButton()
		{
			if (ChatPopup.serverChatPopUp == null && ChatPopup.currChatPopup == null)
			{
				startYesNoDlg(mResources.DOYOUWANTEXIT, new Command(mResources.YES, instance, 8885, null), new Command(mResources.NO, instance, 8882, null));
			}
		}

	public static void startOKDlg(string info)
		{
			info = Res.changeString(info);
			closeKeyBoard();
			msgdlg.setInfo(info, null, new Command(mResources.OK, instance, 8882, null), null);
			currentDialog = msgdlg;
		}

	public static void startWaitDlg(string info)
		{
			info = Res.changeString(info);
			closeKeyBoard();
			msgdlg.setInfo(info, null, new Command(mResources.CANCEL, instance, 8882, null), null);
			currentDialog = msgdlg;
			msgdlg.isWait = true;
		}

	public static void startOKDlg(string info, bool isError)
		{
			info = Res.changeString(info);
			closeKeyBoard();
			msgdlg.setInfo(info, null, new Command(mResources.CANCEL, instance, 8882, null), null);
			currentDialog = msgdlg;
			msgdlg.isWait = true;
		}

	public static void startWaitDlg()
		{
			closeKeyBoard();
			Char.isLoadingMap = true;
		}

	public void openWeb(string strLeft, string strRight, string url, string str)
		{
			str = Res.changeString(str);
			msgdlg.setInfo(str, new Command(strLeft, this, 8881, url), null, new Command(strRight, this, 8882, null));
			currentDialog = msgdlg;
		}

	public static void startOK(string info, int actionID, object p)
		{
			info = Res.changeString(info);
			closeKeyBoard();
			msgdlg.setInfo(info, null, new Command(mResources.OK, instance, actionID, p), null);
			msgdlg.show();
		}

	public static void startserverThongBao(string msgSv)
		{
			msgSv = Res.changeString(msgSv);
			thongBaoTest = msgSv;
			xThongBaoTranslate = w - 60;
			dir_ = -1;
		}

	public static string getMoneys(int m)
		{
			string text = string.Empty;
			int num = m / 1000 + 1;
			for (int i = 0; i < num; i++)
			{
				if (m >= 1000)
				{
					int num2 = m % 1000;
					text = ((num2 != 0) ? ((num2 >= 10) ? ((num2 >= 100) ? ("." + num2 + text) : (".0" + num2 + text)) : (".00" + num2 + text)) : (".000" + text));
					m /= 1000;
					continue;
				}
				text = m + text;
				break;
			}
			return text;
		}

	public static int getX(int start, int w)
		{
			return (px - start) / w;
		}

	public static int getY(int start, int w)
		{
			return (py - start) / w;
		}

	protected void sizeChanged(int w, int h)
		{
		}

	public static bool isGetResourceFromServer()
		{
			return true;
		}

	public static Image loadImageRMS(string path)
		{
			path = Main.res + "/x" + mGraphics.zoomLevel + path;
			path = cutPng(path);
			Image result = null;
			try
			{
				result = Image.createImage(path);
			}
			catch (Exception ex)
			{
				try
				{
					string[] array = Res.split(path, "/", 0);
					string filename = "x" + mGraphics.zoomLevel + array[array.Length - 1];
					sbyte[] array2 = Rms.loadRMS(filename);
					if (array2 != null)
					{
						result = Image.createImage(array2, 0, array2.Length);
						array2 = null;
					}
				}
				catch (Exception)
				{
					Cout.LogError("Loi ham khong tim thay a: " + ex.ToString());
				}
			}
			return result;
		}

	public static Image loadImage(string path)
		{
			path = Main.res + "/x" + mGraphics.zoomLevel + path;
			path = cutPng(path);
			Image result = null;
			try
			{
				result = Image.createImage(path);
			}
			catch (Exception)
			{
			}
			return result;
		}

	private static Image cachedCustomLogo = null;

	public static Image loadCustomImage(string filename)
		{
			try
			{
				if (cachedCustomLogo != null)
				{
					return cachedCustomLogo;
				}
				string text = filename;
				if (!text.Contains("/") && !text.Contains("\\"))
				{
					text = "custom_logo.png";
				}
				if (System.IO.File.Exists(text))
				{
					byte[] array = System.IO.File.ReadAllBytes(text);
					cachedCustomLogo = Image.createImage(array);
					return cachedCustomLogo;
				}
			}
			catch (Exception)
			{
			}
			return null;
		}

	public static string cutPng(string str)
		{
			string result = str;
			if (str.Contains(".png"))
			{
				result = str.Replace(".png", string.Empty);
			}
			return result;
		}

	public static int random(int a, int b)
		{
			return a + r.nextInt(b - a);
		}

	public void loadWaterSplash()
		{
			if (!lowGraphic)
			{
				imgWS = new Image[3];
				for (int i = 0; i < 3; i++)
				{
					imgWS[i] = loadImage("/e/w" + i + ".png");
				}
				wsX = new int[2];
				wsY = new int[2];
				wsState = new int[2];
				wsF = new int[2];
				wsState[0] = (wsState[1] = -1);
			}
		}

	public bool startWaterSplash(int x, int y)
		{
			if (lowGraphic)
			{
				return false;
			}
			int num = ((wsState[0] != -1) ? 1 : 0);
			if (wsState[num] != -1)
			{
				return false;
			}
			wsState[num] = 0;
			wsX[num] = x;
			wsY[num] = y;
			return true;
		}

	public static bool isPaint(int x, int y)
		{
			if (x < GameScr.cmx)
			{
				return false;
			}
			if (x > GameScr.cmx + GameScr.gW)
			{
				return false;
			}
			if (y < GameScr.cmy)
			{
				return false;
			}
			if (y > GameScr.cmy + GameScr.gH + 30)
			{
				return false;
			}
			return true;
		}

	public void resetToLoginScrz()
		{
			resetToLoginScr = true;
		}

	public void perform(int idAction, object p)
		{
			switch (idAction)
			{
			case 9000:
				endDlg();
				SplashScr.imgLogo = null;
				SmallImage.loadBigRMS();
				mSystem.gcc();
				ServerListScreen.bigOk = true;
				ServerListScreen.loadScreen = true;
				GameScr.gI().loadGameScr();
				if (currentScreen != loginScr)
				{
					serverScreen.switchToMe2();
				}
				break;
			case 999:
				mSystem.closeBanner();
				endDlg();
				break;
			case 888396:
				endDlg();
				break;
			case 888397:
			{
				string text4 = (string)p;
				break;
			}
			case 9999:
				endDlg();
				if (loginScr == null)
				{
					loginScr = new LoginScr();
				}
				loginScr.doLogin();
				break;
			case 8881:
			{
				string url = (string)p;
				try
				{
					GameMidlet.instance.platformRequest(url);
				}
				catch (Exception)
				{
				}
				currentDialog = null;
				break;
			}
			case 8882:
				InfoDlg.hide();
				currentDialog = null;
				ServerListScreen.isAutoConect = false;
				ServerListScreen.countDieConnect = 0;
				break;
			case 8884:
				endDlg();
				if (serverScr == null)
				{
					serverScr = new ServerScr();
				}
				serverScr.switchToMe();
				break;
			case 8885:
				GameMidlet.instance.exit();
				break;
			case 8886:
			{
				endDlg();
				string name = (string)p;
				Service.gI().addFriend(name);
				break;
			}
			case 8887:
			{
				endDlg();
				int charId = (int)p;
				Service.gI().addPartyAccept(charId);
				break;
			}
			case 8888:
			{
				int charId2 = (int)p;
				Service.gI().addPartyCancel(charId2);
				endDlg();
				break;
			}
			case 8889:
			{
				string str = (string)p;
				endDlg();
				Service.gI().acceptPleaseParty(str);
				break;
			}
			case 88810:
			{
				int playerMapId = (int)p;
				endDlg();
				Service.gI().acceptInviteTrade(playerMapId);
				break;
			}
			case 88811:
				endDlg();
				Service.gI().cancelInviteTrade();
				break;
			case 88814:
			{
				Item[] items = (Item[])p;
				endDlg();
				Service.gI().crystalCollectLock(items);
				break;
			}
			case 88817:
				ChatPopup.addChatPopup(string.Empty, 1, Char.myCharz().npcFocus);
				Service.gI().menu(Char.myCharz().npcFocus.template.npcTemplateId, menu.menuSelectedItem, 0);
				break;
			case 88818:
			{
				short menuId2 = (short)p;
				Service.gI().textBoxId(menuId2, inputDlg.tfInput.getText());
				endDlg();
				break;
			}
			case 88819:
			{
				short menuId = (short)p;
				Service.gI().menuId(menuId);
				break;
			}
			case 88820:
			{
				string[] array = (string[])p;
				if (Char.myCharz().npcFocus == null)
				{
					break;
				}
				int menuSelectedItem = menu.menuSelectedItem;
				if (array.Length > 1)
				{
					MyVector myVector = new MyVector();
					for (int i = 0; i < array.Length - 1; i++)
					{
						myVector.addElement(new Command(array[i + 1], instance, 88821, menuSelectedItem));
					}
					menu.startAt(myVector, 3);
				}
				else
				{
					ChatPopup.addChatPopup(string.Empty, 1, Char.myCharz().npcFocus);
					Service.gI().menu(Char.myCharz().npcFocus.template.npcTemplateId, menuSelectedItem, 0);
				}
				break;
			}
			case 88821:
			{
				int menuId3 = (int)p;
				ChatPopup.addChatPopup(string.Empty, 1, Char.myCharz().npcFocus);
				Service.gI().menu(Char.myCharz().npcFocus.template.npcTemplateId, menuId3, menu.menuSelectedItem);
				break;
			}
			case 88822:
				ChatPopup.addChatPopup(string.Empty, 1, Char.myCharz().npcFocus);
				Service.gI().menu(Char.myCharz().npcFocus.template.npcTemplateId, menu.menuSelectedItem, 0);
				break;
			case 88823:
				startOKDlg(mResources.SENTMSG);
				break;
			case 88824:
				startOKDlg(mResources.NOSENDMSG);
				break;
			case 88825:
				startOKDlg(mResources.sendMsgSuccess, isError: false);
				break;
			case 88826:
				startOKDlg(mResources.cannotSendMsg, isError: false);
				break;
			case 88827:
				startOKDlg(mResources.sendGuessMsgSuccess);
				break;
			case 88828:
				startOKDlg(mResources.sendMsgFail);
				break;
			case 88829:
			{
				string text5 = inputDlg.tfInput.getText();
				if (!text5.Equals(string.Empty))
				{
					Service.gI().changeName(text5, (int)p);
					InfoDlg.showWait();
				}
				break;
			}
			case 88836:
				inputDlg.tfInput.setMaxTextLenght(6);
				inputDlg.show(mResources.INPUT_PRIVATE_PASS, new Command(mResources.ACCEPT, instance, 888361, null), TField.INPUT_TYPE_NUMERIC);
				break;
			case 888361:
			{
				string text3 = inputDlg.tfInput.getText();
				endDlg();
				if (text3.Length < 6 || text3.Equals(string.Empty))
				{
					startOKDlg(mResources.ALERT_PRIVATE_PASS_1);
					break;
				}
				try
				{
					Service.gI().activeAccProtect(int.Parse(text3));
					break;
				}
				catch (Exception ex3)
				{
					startOKDlg(mResources.ALERT_PRIVATE_PASS_2);
					Cout.println("Loi tai 888361 Gamescavas " + ex3.ToString());
					break;
				}
			}
			case 88837:
			{
				string text2 = inputDlg.tfInput.getText();
				endDlg();
				try
				{
					Service.gI().openLockAccProtect(int.Parse(text2.Trim()));
					break;
				}
				catch (Exception ex2)
				{
					Cout.println("Loi tai 88837 " + ex2.ToString());
					break;
				}
			}
			case 88839:
			{
				string text = inputDlg.tfInput.getText();
				endDlg();
				if (text.Length < 6 || text.Equals(string.Empty))
				{
					startOKDlg(mResources.ALERT_PRIVATE_PASS_1);
					break;
				}
				try
				{
					startYesNoDlg(mResources.cancelAccountProtection, 888391, text, 8882, null);
					break;
				}
				catch (Exception)
				{
					startOKDlg(mResources.ALERT_PRIVATE_PASS_2);
					break;
				}
			}
			case 888391:
			{
				string s = (string)p;
				endDlg();
				Service.gI().clearAccProtect(int.Parse(s));
				break;
			}
			case 888392:
				Service.gI().menu(4, menu.menuSelectedItem, 0);
				break;
			case 888393:
				if (loginScr == null)
				{
					loginScr = new LoginScr();
				}
				loginScr.doLogin();
				Main.closeKeyBoard();
				break;
			case 888394:
				endDlg();
				break;
			case 888395:
				endDlg();
				break;
			case 101023:
				Main.numberQuit = 0;
				break;
			case 101024:
				Res.outz("output 101024");
				endDlg();
				break;
			case 101025:
				endDlg();
				if (ServerListScreen.loadScreen)
				{
					serverScreen.switchToMe();
				}
				else
				{
					serverScreen.show2();
				}
				break;
			case 101026:
				mSystem.onDisconnected();
				break;
			case 100001:
				Service.gI().getFlag(0, -1);
				InfoDlg.showWait();
				break;
			case 100002:
				if (loginScr == null)
				{
					loginScr = new LoginScr();
				}
				loginScr.backToRegister();
				break;
			case 100005:
				if (Char.myCharz().statusMe == 14)
				{
					startOKDlg(mResources.can_not_do_when_die);
				}
				else
				{
					Service.gI().openUIZone();
				}
				break;
			case 100006:
				mSystem.onDisconnected();
				break;
			case 100016:
				ServerListScreen.SetIpSelect(17, issave: false);
				instance.doResetToLoginScr(serverScreen);
				ServerListScreen.waitToLogin = true;
				endDlg();
				break;
			}
		}

	public static bool isWait()
		{
			return Char.isLoadingMap || LoginScr.isContinueToLogin || ServerListScreen.waitToLogin || ServerListScreen.isWait || SelectCharScr.isWait;
		}

}
