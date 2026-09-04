public class SplashScr : mScreen
{
	public static int splashScrStat;

	private bool isCheckConnect;

	private bool isSwitchToLogin;

	public static int nData = -1;

	public static int maxData = -1;

	public static SplashScr instance;

	public static Image imgLogo;

	private int timeLoading = 10;

	public long TIMEOUT;

	public SplashScr()
	{
		instance = this;
	}

	public static void loadSplashScr()
	{
		splashScrStat = 0;
	}

	public override void update()
	{
		splashScrStat++;
		if (splashScrStat == 10 && !isCheckConnect)
		{
			isCheckConnect = true;
			if (Rms.loadRMSInt("serverchat") != -1)
			{
				GameScr.isPaintChatVip = Rms.loadRMSInt("serverchat") == 0;
			}
			if (Rms.loadRMSInt("isPlaySound") != -1)
			{
				GameCanvas.isPlaySound = Rms.loadRMSInt("isPlaySound") == 1;
			}
			if (GameCanvas.isPlaySound)
			{
				SoundMn.gI().loadSound(TileMap.mapID);
			}
			SoundMn.gI().getStrOption();
			ServerListScreen.loadIP();
		}
		if (splashScrStat >= 25 && !isSwitchToLogin)
		{
			isSwitchToLogin = true;
			if (GameCanvas.serverScreen == null)
			{
				GameCanvas.serverScreen = new ServerListScreen();
			}
			ServerListScreen.loadScreen = true;
			if (Session_ME.gI().isConnected())
			{
				GameCanvas.serverScreen.switchToMe();
			}
			else
			{
				mSystem.onDisconnected();
				GameCanvas.serverScreen.switchToMe();
			}
		}
		ServerListScreen.updateDeleteData();
	}

	public static void loadIP()
	{
		int sv = Rms.loadRMSInt(ServerListScreen.RMS_svselect);
		Res.err(">>>>>loadIP:  svselect == " + sv + "  clientType:" + mSystem.clientType);
		if (sv == -1)
		{
			ServerListScreen.SetIpSelect((ServerListScreen.serverPriority >= 0) ? ServerListScreen.serverPriority : 0, issave: true);
		}
		else
		{
			ServerListScreen.SetIpSelect(sv, issave: false);
		}
		ServerListScreen.ConnectIP();
	}

	public override void paint(mGraphics g)
	{
		if (imgLogo != null)
		{
			g.setColor(16777215);
			g.fillRect(0, 0, GameCanvas.w, GameCanvas.h);
			g.drawImage(imgLogo, GameCanvas.w / 2, GameCanvas.h / 2, 3);
		}
		else
		{
			g.setColor(0);
			g.fillRect(0, 0, GameCanvas.w, GameCanvas.h);
			GameCanvas.paintShukiren(GameCanvas.hw, GameCanvas.hh, g);
			ServerListScreen.paintDeleteData(g);
		}
	}

	public static void loadImg()
	{
		if (imgLogo == null)
		{
			Image customLogo = GameCanvas.loadCustomImage("custom_logo.png");
			if (customLogo != null)
			{
				imgLogo = customLogo;
			}
			else
			{
				imgLogo = GameCanvas.loadImage("/gamelogo.png");
			}
		}
	}
}
