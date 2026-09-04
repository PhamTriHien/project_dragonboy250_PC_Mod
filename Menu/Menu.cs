
public partial class Menu
{
	public bool showMenu;

	public MyVector menuItems;

	public int menuSelectedItem;

	public int menuX;

	public int menuY;

	public int menuW;

	public int menuH;

	public static int[] menuTemY;

	public static int cmtoX;

	public static int cmx;

	public static int cmdy;

	public static int cmvy;

	public static int cmxLim;

	public static int xc;

	private Command left = new Command(mResources.SELECT, 0);

	private Command right = new Command(mResources.CLOSE, 0, GameCanvas.w - 71, GameCanvas.h - mScreen.cmdH + 1);

	private Command center;

	public static Image imgMenu1;

	public static Image imgMenu2;

	private bool disableClose;

	public int tDelay;

	public int w;

	private int pa;

	private bool trans;

	private int pointerDownTime;

	private int pointerDownFirstX;

	private int[] pointerDownLastX = new int[3];

	private bool pointerIsDowning;

	private bool isDownWhenRunning;

	private bool wantUpdateList;

	private int waitToPerform;

	private int cmRun;

	private bool touch;

	private bool close;

	private int cmvx;

	private int cmdx;

	private bool isClose;

	public bool[] isNotClose;

	public static void loadBg()
		{
			imgMenu1 = GameCanvas.loadImage("/mainImage/myTexture2dbtMenu1.png");
			imgMenu2 = GameCanvas.loadImage("/mainImage/myTexture2dbtMenu2.png");
		}

	public bool isScrolling()
		{
			if ((!isClose && menuTemY[menuTemY.Length - 1] > menuY) || (isClose && menuTemY[menuTemY.Length - 1] < GameCanvas.h))
			{
				return true;
			}
			return false;
		}

	public void moveCamera()
		{
			if (cmRun != 0 && !pointerIsDowning)
			{
				cmtoX += cmRun / 100;
				if (cmtoX < 0)
				{
					cmtoX = 0;
				}
				else if (cmtoX > cmxLim)
				{
					cmtoX = cmxLim;
				}
				else
				{
					cmx = cmtoX;
				}
				cmRun = cmRun * 9 / 10;
				if (cmRun < 100 && cmRun > -100)
				{
					cmRun = 0;
				}
			}
			if (cmx != cmtoX && !pointerIsDowning)
			{
				cmvx = cmtoX - cmx << 2;
				cmdx += cmvx;
				cmx += cmdx >> 4;
				cmdx &= 15;
			}
		}

	public void doCloseMenu()
		{
			Res.outz("CLOSE MENU");
			isClose = false;
			showMenu = false;
			InfoDlg.hide();
			if (close)
			{
				GameCanvas.panel.cp = null;
				Char.chatPopup = null;
				if (GameCanvas.panel2 != null && GameCanvas.panel2.cp != null)
				{
					GameCanvas.panel2.cp = null;
				}
			}
			else
			{
				if (!touch)
				{
					return;
				}
				GameCanvas.panel.cp = null;
				if (GameCanvas.panel2 != null && GameCanvas.panel2.cp != null)
				{
					GameCanvas.panel2.cp = null;
				}
				if (menuSelectedItem >= 0)
				{
					Command command = (Command)menuItems.elementAt(menuSelectedItem);
					if (command != null)
					{
						SoundMn.gI().buttonClose();
						command.performAction();
					}
				}
			}
		}

}
