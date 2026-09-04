using System;

public partial class CreateCharScr : mScreen, IActionListener
{
	public static CreateCharScr instance;

	private PopUp p;

	public static bool isCreateChar = false;

	private Command cmdSelectSv;

	public static TField tAddName;

	public static int indexGender;

	public static int indexHair;

	public static int selected;

	public static int[][] hairID = new int[3][]
		{
			new int[3] { 64, 30, 31 },
			new int[3] { 9, 29, 32 },
			new int[3] { 6, 27, 28 }
		};

	public static int[] defaultLeg = new int[3] { 2, 13, 8 };

	public static int[] defaultBody = new int[3] { 1, 12, 7 };

	private int yButton;

	private int disY;

	private int[] bgID = new int[3] { 0, 4, 8 };

	public int yBegin;

	private int curIndex;

	private int cx = 168;

	private int cy = 350;

	private int dy = 45;

	private int cp1;

	private int cf;

	public CreateCharScr()
		{
			if (GameCanvas.loginScr == null)
			{
				GameCanvas.loginScr = new LoginScr();
			}
			try
			{
				if (!GameCanvas.lowGraphic)
				{
					loadMapFromResource(new sbyte[3] { 39, 40, 41 });
				}
				loadMapTableFromResource(new sbyte[3] { 39, 40, 41 });
			}
			catch (Exception ex)
			{
				Cout.LogError("Tao char loi " + ex.ToString());
			}
			if (GameCanvas.w <= 200)
			{
				GameScr.setPopupSize(128, 100);
				GameScr.popupX = (GameCanvas.w - 128) / 2;
				GameScr.popupY = 10;
				cy += 15;
				dy -= 15;
			}
			indexGender = 1;
			tAddName = new TField();
			tAddName.width = GameCanvas.loginScr.tfUser.width;
			if (GameCanvas.w < 200)
			{
				tAddName.width = 60;
			}
			tAddName.height = mScreen.ITEM_HEIGHT + 2;
			if (GameCanvas.w < 200)
			{
				tAddName.x = GameScr.popupX + 45;
				tAddName.y = GameScr.popupY + 12;
			}
			else
			{
				tAddName.x = GameCanvas.w / 2 - tAddName.width / 2;
				tAddName.y = 35;
			}
			if (!GameCanvas.isTouch)
			{
				tAddName.isFocus = true;
			}
			tAddName.setIputType(TField.INPUT_TYPE_ANY);
			tAddName.showSubTextField = false;
			tAddName.strInfo = mResources.char_name;
			if (tAddName.getText().Equals("@"))
			{
				tAddName.setText(GameCanvas.loginScr.tfUser.getText().Substring(0, GameCanvas.loginScr.tfUser.getText().IndexOf("@")));
			}
			tAddName.name = mResources.char_name;
			indexGender = 1;
			indexHair = 0;
			center = new Command(mResources.NEWCHAR, this, 8000, null);
			left = new Command(mResources.BACK, this, 8001, null);
			if (!GameCanvas.isTouch)
			{
				right = tAddName.cmdClear;
			}
			yBegin = tAddName.y;
		}

	public static CreateCharScr gI()
		{
			if (instance == null)
			{
				instance = new CreateCharScr();
			}
			return instance;
		}

	public static void init()
		{
		}

	public override void switchToMe()
		{
			LoginScr.isContinueToLogin = false;
			GameCanvas.menu.showMenu = false;
			GameCanvas.endDlg();
			base.switchToMe();
			indexGender = Res.random(0, 3);
			indexHair = Res.random(0, 3);
			doChangeMap();
			Char.isLoadingMap = false;
			tAddName.setFocusWithKb(isFocus: true);
			ServerListScreen.countDieConnect = 0;
			if (GameCanvas.isTouch)
			{
				string svName = (ServerListScreen.nameServer != null && ServerListScreen.ipSelect >= 0 && ServerListScreen.ipSelect < ServerListScreen.nameServer.Length) ? ServerListScreen.nameServer[ServerListScreen.ipSelect] : "Server";
				cmdSelectSv = new Command(svName, this, 10018, null);
				cmdSelectSv.x = 1;
				cmdSelectSv.y = 3;
			}
		}

}
