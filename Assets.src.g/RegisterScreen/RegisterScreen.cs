using System;

namespace Assets.src.g;

public partial class RegisterScreen : mScreen, IActionListener
{
	public TField tfUser;

	public TField tfNgay;

	public TField tfThang;

	public TField tfNam;

	public TField tfDiachi;

	public TField tfCMND;

	public TField tfNgayCap;

	public TField tfNoiCap;

	public TField tfSodt;

	public static bool isContinueToLogin = false;

	private int focus;

	private int wC;

	private int yL;

	private int defYL;

	public bool isCheck;

	public bool isRes;

	private Command cmdLogin;

	private Command cmdCheck;

	private Command cmdFogetPass;

	private Command cmdRes;

	private Command cmdMenu;

	private Command cmdBackFromRegister;

	public string listFAQ = string.Empty;

	public string titleFAQ;

	public string subtitleFAQ;

	private string numSupport = string.Empty;

	private string strUser;

	private string strPass;

	public static bool isLocal = false;

	public static bool isUpdateAll;

	public static bool isUpdateData;

	public static bool isUpdateMap;

	public static bool isUpdateSkill;

	public static bool isUpdateItem;

	public static string serverName;

	public static Image imgTitle;

	public int plX;

	public int plY;

	public int lY;

	public int lX;

	public int logoDes;

	public int lineX;

	public int lineY;

	public static int[] bgId = new int[5] { 0, 8, 2, 6, 9 };

	public static bool isTryGetIPFromWap;

	public static short timeLogin;

	public static long lastTimeLogin;

	public static long currTimeLogin;

	private int yt;

	private Command cmdSelect;

	private Command cmdOK;

	private int xLog;

	private int yLog;

	private int xP;

	private int yP;

	private int wP;

	private int hP;

	private int tipid = -1;

	public bool isLogin2;

	private int v = 2;

	private int g;

	private int ylogo = -40;

	private int dir = 1;

	public static bool isLoggingIn;

	public RegisterScreen(sbyte haveName)
		{
			yLog = 130;
			TileMap.bgID = (sbyte)(mSystem.currentTimeMillis() % 9);
			if (TileMap.bgID == 5 || TileMap.bgID == 6)
			{
				TileMap.bgID = 4;
			}
			GameScr.loadCamera(fullmScreen: true, -1, -1);
			GameScr.cmx = 100;
			GameScr.cmy = 200;
			if (GameCanvas.h > 200)
			{
				defYL = GameCanvas.hh - 80;
			}
			else
			{
				defYL = GameCanvas.hh - 65;
			}
			resetLogo();
			wC = ((GameCanvas.w < 200) ? 140 : 160);
			yt = GameCanvas.hh - mScreen.ITEM_HEIGHT - 5;
			if (GameCanvas.h <= 160)
			{
				yt = 20;
			}
			tfSodt = new TField();
			tfSodt.setIputType(TField.INPUT_TYPE_NUMERIC);
			tfSodt.width = 220;
			tfSodt.height = mScreen.ITEM_HEIGHT + 2;
			tfSodt.name = "Số điện thoại";
			if (haveName == 1)
			{
				tfSodt.setText("01234567890");
			}
			tfUser = new TField();
			tfUser.width = 220;
			tfUser.height = mScreen.ITEM_HEIGHT + 2;
			tfUser.isFocus = true;
			tfUser.name = "Họ và tên";
			if (haveName == 1)
			{
				tfUser.setText("Nguyễn Văn A");
			}
			tfUser.setIputType(TField.INPUT_TYPE_ANY);
			tfNgay = new TField();
			tfNgay.setIputType(TField.INPUT_TYPE_NUMERIC);
			tfNgay.width = 70;
			tfNgay.height = mScreen.ITEM_HEIGHT + 2;
			tfNgay.name = "Ngày sinh";
			if (haveName == 1)
			{
				tfNgay.setText("01");
			}
			tfThang = new TField();
			tfThang.setIputType(TField.INPUT_TYPE_NUMERIC);
			tfThang.width = 70;
			tfThang.height = mScreen.ITEM_HEIGHT + 2;
			tfThang.name = "Tháng sinh";
			if (haveName == 1)
			{
				tfThang.setText("01");
			}
			tfNam = new TField();
			tfNam.setIputType(TField.INPUT_TYPE_NUMERIC);
			tfNam.width = 70;
			tfNam.height = mScreen.ITEM_HEIGHT + 2;
			tfNam.name = "Năm sinh";
			if (haveName == 1)
			{
				tfNam.setText("1990");
			}
			tfDiachi = new TField();
			tfDiachi.setIputType(TField.INPUT_TYPE_ANY);
			tfDiachi.width = 220;
			tfDiachi.height = mScreen.ITEM_HEIGHT + 2;
			tfDiachi.name = "Địa chỉ đăng ký thường trú";
			if (haveName == 1)
			{
				tfDiachi.setText("123 đường số 1, Quận 1, TP.HCM");
			}
			tfCMND = new TField();
			tfCMND.setIputType(TField.INPUT_TYPE_NUMERIC);
			tfCMND.width = 220;
			tfCMND.height = mScreen.ITEM_HEIGHT + 2;
			tfCMND.name = "Số Chứng minh nhân dân hoặc số hộ chiếu";
			if (haveName == 1)
			{
				tfCMND.setText("123456789");
			}
			tfNgayCap = new TField();
			tfNgayCap.setIputType(TField.INPUT_TYPE_ANY);
			tfNgayCap.width = 220;
			tfNgayCap.height = mScreen.ITEM_HEIGHT + 2;
			tfNgayCap.name = "Ngày cấp";
			if (haveName == 1)
			{
				tfNgayCap.setText("01/01/2005");
			}
			tfNoiCap = new TField();
			tfNoiCap.setIputType(TField.INPUT_TYPE_ANY);
			tfNoiCap.width = 220;
			tfNoiCap.height = mScreen.ITEM_HEIGHT + 2;
			tfNoiCap.name = "Nơi cấp";
			if (haveName == 1)
			{
				tfNoiCap.setText("TP.HCM");
			}
			yt += 35;
			isCheck = true;
			focus = 0;
			cmdLogin = new Command((GameCanvas.w <= 200) ? mResources.login2 : mResources.login, GameCanvas.instance, 888393, null);
			cmdCheck = new Command(mResources.remember, this, 2001, null);
			cmdRes = new Command(mResources.register, this, 2002, null);
			cmdBackFromRegister = new Command(mResources.CANCEL, this, 10021, null);
			left = (cmdMenu = new Command(mResources.MENU, this, 2003, null));
			if (GameCanvas.isTouch)
			{
				cmdLogin.x = GameCanvas.w / 2 - 100;
				cmdMenu.x = GameCanvas.w / 2 - mScreen.cmdW - 8;
				if (GameCanvas.h >= 200)
				{
					cmdLogin.y = GameCanvas.h / 2 - 40;
					cmdMenu.y = yLog + 110;
				}
				cmdBackFromRegister.x = GameCanvas.w / 2 + 3;
				cmdBackFromRegister.y = yLog + 110;
				cmdRes.x = GameCanvas.w / 2 - 84;
				cmdRes.y = cmdMenu.y;
			}
			wP = 170;
			hP = ((!isRes) ? 100 : 110);
			xP = GameCanvas.hw - wP / 2;
			yP = tfUser.y - 15;
			int num = 4;
			int num2 = num * 32 + 23 + 33;
			if (num2 >= GameCanvas.w)
			{
				num--;
				num2 = num * 32 + 23 + 33;
			}
			xLog = GameCanvas.w / 2 - num2 / 2;
			yLog = 5;
			lY = ((GameCanvas.w < 200) ? (tfUser.y - 30) : (yLog - 30));
			tfUser.x = xLog + 10;
			tfUser.y = yLog + 20;
			cmdOK = new Command(mResources.OK, this, 2008, null);
			cmdOK.x = 260;
			cmdOK.y = GameCanvas.h - 60;
			cmdFogetPass = new Command("Thoát", this, 1003, null);
			cmdFogetPass.x = 260;
			cmdFogetPass.y = GameCanvas.h - 30;
			cmdOK.x = GameCanvas.w / 2 - 80;
			cmdFogetPass.x = GameCanvas.w / 2 + 10;
			cmdFogetPass.y = (cmdOK.y = GameCanvas.h - 25);
			center = cmdOK;
			left = cmdFogetPass;
		}

	public new void switchToMe()
		{
			Res.outz("Res switch");
			SoundMn.gI().stopAll();
			focus = 0;
			tfUser.isFocus = true;
			tfNgay.isFocus = false;
			if (GameCanvas.isTouch)
			{
				tfUser.isFocus = false;
				focus = -1;
			}
			base.switchToMe();
		}

	protected void doMenu()
		{
			MyVector myVector = new MyVector("vMenu Login");
			myVector.addElement(new Command(mResources.registerNewAcc, this, 2004, null));
			if (!isLogin2)
			{
				myVector.addElement(new Command(mResources.selectServer, this, 1004, null));
			}
			myVector.addElement(new Command(mResources.forgetPass, this, 1003, null));
			myVector.addElement(new Command(mResources.website, this, 1005, null));
			int num = Rms.loadRMSInt("lowGraphic");
			if (num == 1)
			{
				myVector.addElement(new Command(mResources.increase_vga, this, 10041, null));
			}
			else
			{
				myVector.addElement(new Command(mResources.decrease_vga, this, 10042, null));
			}
			myVector.addElement(new Command(mResources.EXIT, GameCanvas.instance, 8885, null));
			GameCanvas.menu.startAt(myVector, 0);
		}

	protected int loadIndexServer()
		{
			return Rms.loadRMSInt("indServer");
		}

	public void doLogin()
		{
		}

	public void savePass()
		{
		}

	private void doChangeTip()
		{
			tipid++;
			if (tipid >= mResources.tips.Length)
			{
				tipid = 0;
			}
			if (GameCanvas.currentDialog == GameCanvas.msgdlg && GameCanvas.msgdlg.isWait)
			{
				GameCanvas.msgdlg.setInfo(mResources.tips[tipid]);
			}
		}

	public override void unLoad()
		{
			base.unLoad();
		}

	private void turnOffFocus()
		{
			tfUser.isFocus = false;
			tfNgay.isFocus = false;
			tfThang.isFocus = false;
			tfNam.isFocus = false;
			tfDiachi.isFocus = false;
			tfCMND.isFocus = false;
			tfNgayCap.isFocus = false;
			tfNoiCap.isFocus = false;
			tfSodt.isFocus = false;
		}

	private void processFocus()
		{
			turnOffFocus();
			switch (focus)
			{
			case 0:
				tfUser.isFocus = true;
				break;
			case 1:
				tfNgay.isFocus = true;
				break;
			case 2:
				tfThang.isFocus = true;
				break;
			case 3:
				tfNam.isFocus = true;
				break;
			case 4:
				tfDiachi.isFocus = true;
				break;
			case 5:
				tfCMND.isFocus = true;
				break;
			case 6:
				tfNgayCap.isFocus = true;
				break;
			case 7:
				tfNoiCap.isFocus = true;
				break;
			case 8:
				tfSodt.isFocus = true;
				break;
			}
		}

	public void resetLogo()
		{
			yL = -50;
		}

	public void backToRegister()
		{
			if (GameCanvas.loginScr.isLogin2)
			{
				GameCanvas.startYesNoDlg(mResources.note, new Command(mResources.YES, GameCanvas.panel, 10019, null), new Command(mResources.NO, GameCanvas.panel, 10020, null));
				return;
			}
			GameCanvas.instance.doResetToLoginScr(GameCanvas.loginScr);
			Session_ME.gI().close();
		}

}
