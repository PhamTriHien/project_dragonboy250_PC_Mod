using System;
using Assets.src.e;
using Assets.src.g;
using UnityEngine;
public partial class GameCanvas : IActionListener
{
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

}
