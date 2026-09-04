using System;
using UnityEngine;

public partial class LoginScr : mScreen, IActionListener
{
	public TField tfUser;

	public TField tfPass;

	public static bool isContinueToLogin = false;

	private int focus;

	private int wC;

	private int yL;

	private int defYL;

	public bool isCheck;

	public bool isRes;

	public Command cmdLogin;

	public Command cmdCheck;

	public Command cmdFogetPass;

	public Command cmdRes;

	public Command cmdMenu;

	public Command cmdBackFromRegister;

	public Command cmdBack;

	public string listFAQ = string.Empty;

	public string titleFAQ;

	public string subtitleFAQ;

	private string numSupport = string.Empty;

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

	public static GameMidlet m;

	private int yy = GameCanvas.hh - mScreen.ITEM_HEIGHT - 5;

	private int freeAreaHeight;

	private int xP;

	private int yP;

	private int wP;

	private int hP;

	private int t = 20;

	private bool isRegistering;

	private string passRe = string.Empty;

	public bool isFAQ;

	private int tipid = -1;

	public bool isLogin2;

	private int v = 2;

	private int g;

	private int ylogo = -40;

	private int dir = 1;

	private Command cmdCallHotline;

	public static bool isLoggingIn;

	public LoginScr()
		{
			yLog = GameCanvas.hh - 30;
			TileMap.bgID = (sbyte)(mSystem.currentTimeMillis() % 9);
			if (TileMap.bgID == 5 || TileMap.bgID == 6)
			{
				TileMap.bgID = 4;
			}
			GameScr.loadCamera(fullmScreen: true, -1, -1);
			GameScr.cmx = 100;
			GameScr.cmy = 200;
			Main.closeKeyBoard();
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
			tfUser = new TField();
			tfUser.y = GameCanvas.hh - mScreen.ITEM_HEIGHT - 9;
			tfUser.width = wC;
			tfUser.height = mScreen.ITEM_HEIGHT + 2;
			tfUser.isFocus = true;
			tfUser.setIputType(TField.INPUT_TYPE_ANY);
			tfUser.name = ((mResources.language != 2) ? (mResources.phone + "/") : string.Empty) + mResources.email;
			tfPass = new TField();
			tfPass.y = GameCanvas.hh - 4;
			tfPass.setIputType(TField.INPUT_TYPE_PASSWORD);
			tfPass.width = wC;
			tfPass.height = mScreen.ITEM_HEIGHT + 2;
			yt += 35;
			isCheck = true;
			switch (Rms.loadRMSInt(Rms.RMS_check))
			{
			case 1:
				isCheck = true;
				break;
			case 2:
				isCheck = false;
				break;
			}
			tfUser.setText(Rms.loadRMSString(Rms.RMS_acc));
			tfPass.setText(Rms.loadRMSString(Rms.RMS_pass));
			if (cmdCallHotline == null)
			{
				cmdCallHotline = new Command("Gọi hotline", this, 13, null);
				cmdCallHotline.x = GameCanvas.w - 75;
				if (mSystem.clientType == 1 && !GameCanvas.isTouch)
				{
					cmdCallHotline.y = GameCanvas.h - 20;
				}
				else
				{
					int num = 2;
					cmdCallHotline.y = num + 6;
				}
			}
			focus = 0;
			cmdLogin = new Command((GameCanvas.w <= 200) ? mResources.login2 : mResources.login, GameCanvas.instance, 888393, null);
			cmdCheck = new Command(mResources.remember, this, 2001, null);
			cmdRes = new Command(mResources.register, this, 2002, null);
			cmdBackFromRegister = new Command(mResources.CANCEL, this, 10021, null);
			cmdBack = new Command(mResources.BACK, this, 101, null);
			left = (cmdMenu = new Command(mResources.MENU, this, 2003, null));
			freeAreaHeight = tfUser.y - 2 * tfUser.height;
			if (GameCanvas.isTouch)
			{
				cmdLogin.x = GameCanvas.w / 2 + 8;
				cmdMenu.x = GameCanvas.w / 2 - mScreen.cmdW - 8;
				if (GameCanvas.h >= 200)
				{
					cmdLogin.y = yLog + 110;
					cmdMenu.y = yLog + 110;
				}
				cmdBackFromRegister.x = GameCanvas.w / 2 + 3;
				cmdBackFromRegister.y = yLog + 110;
				cmdRes.x = GameCanvas.w / 2 - 84;
				cmdRes.y = cmdMenu.y;
				cmdBack.x = 2;
				cmdBack.y = GameCanvas.h - mScreen.cmdH;
			}
			wP = 170;
			hP = ((!isRes) ? 100 : 110);
			xP = GameCanvas.hw - wP / 2;
			yP = tfUser.y - 15;
			int num2 = 4;
			int num3 = num2 * 32 + 23 + 33;
			if (num3 >= GameCanvas.w)
			{
				num2--;
				num3 = num2 * 32 + 23 + 33;
			}
			xLog = GameCanvas.w / 2 - num3 / 2;
			yLog = GameCanvas.hh - 30;
			lY = ((GameCanvas.w < 200) ? (tfUser.y - 30) : (yLog - 30));
			tfUser.x = xLog + 10;
			tfUser.y = yLog + 20;
			cmdOK = new Command(mResources.OK, this, 2008, null);
			cmdOK.x = GameCanvas.w / 2 - 84;
			cmdOK.y = cmdLogin.y;
			cmdFogetPass = new Command(mResources.forgetPass, this, 1003, null);
			cmdFogetPass.x = GameCanvas.w / 2 + 3;
			cmdFogetPass.y = cmdLogin.y;
			center = cmdOK;
			left = cmdFogetPass;
		}

	public static void getServerLink()
		{
			try
			{
				if (isTryGetIPFromWap)
				{
					return;
				}
				Command command = new Command();
				ActionChat actionChat = delegate(string str)
				{
					try
					{
						if (str != null && !(str == string.Empty))
						{
							Rms.saveIP(str);
							if (str.Contains(":"))
							{
								int num = str.IndexOf(":");
								string text = str.Substring(0, num);
								string s = str.Substring(num + 1);
								GameMidlet.IP = text;
								GameMidlet.PORT = int.Parse(s);
								Session_ME.gI().connect(text, int.Parse(s));
								isTryGetIPFromWap = true;
							}
						}
					}
					catch (Exception)
					{
					}
				};
				command.actionChat = actionChat;
				Net.connectHTTP(ServerListScreen.linkGetHost, command);
			}
			catch (Exception)
			{
			}
		}

	public override void switchToMe()
		{
			isRegistering = false;
			SoundMn.gI().stopAll();
			tfUser.isFocus = true;
			tfPass.isFocus = false;
			if (GameCanvas.isTouch)
			{
				tfUser.isFocus = false;
			}
			GameCanvas.loadBG(0);
			left = new Command(mResources.BACK, this, 101, null);
			base.switchToMe();
		}

	public void setUserPass()
		{
			string text = Rms.loadRMSString(Rms.RMS_acc);
			if (text != null && !text.Equals(string.Empty))
			{
				tfUser.setText(text);
			}
			string text2 = Rms.loadRMSString(Rms.RMS_pass);
			if (text2 != null && !text2.Equals(string.Empty))
			{
				tfPass.setText(text2);
			}
		}

	protected void doMenu()
		{
			MyVector myVector = new MyVector();
			myVector.addElement(new Command(mResources.registerNewAcc, this, 2004, null));
			if (!isLogin2)
			{
				myVector.addElement(new Command(mResources.selectServer, this, 1004, null));
			}
			myVector.addElement(new Command(mResources.forgetPass, this, 1003, null));
			myVector.addElement(new Command(mResources.website, this, 1005, null));
			if (Main.isPC)
			{
				myVector.addElement(new Command(mResources.EXIT, GameCanvas.instance, 8885, null));
			}
			GameCanvas.menu.startAt(myVector, 0);
		}

	protected void doRegister()
		{
			if (tfUser.getText().Equals(string.Empty))
			{
				GameCanvas.startOKDlg(mResources.userBlank);
				return;
			}
			char[] array = tfUser.getText().ToCharArray();
			if (tfPass.getText().Equals(string.Empty))
			{
				GameCanvas.startOKDlg(mResources.passwordBlank);
				return;
			}
			if (tfUser.getText().Length < 5)
			{
				GameCanvas.startOKDlg(mResources.accTooShort);
				return;
			}
			int num = 0;
			string text = null;
			if (mResources.language == 2)
			{
				if (tfUser.getText().IndexOf("@") == -1 || tfUser.getText().IndexOf(".") == -1)
				{
					text = mResources.emailInvalid;
				}
				num = 0;
			}
			else
			{
				try
				{
					long num2 = long.Parse(tfUser.getText());
					if (tfUser.getText().Length < 8 || tfUser.getText().Length > 12 || (!tfUser.getText().StartsWith("0") && !tfUser.getText().StartsWith("84")))
					{
						text = mResources.phoneInvalid;
					}
					num = 1;
				}
				catch (Exception)
				{
					if (tfUser.getText().IndexOf("@") == -1 || tfUser.getText().IndexOf(".") == -1)
					{
						text = mResources.emailInvalid;
					}
					num = 0;
				}
			}
			if (text != null)
			{
				GameCanvas.startOKDlg(text);
			}
			else
			{
				GameCanvas.msgdlg.setInfo(mResources.plsCheckAcc + ((num != 1) ? (mResources.email + ": ") : (mResources.phone + ": ")) + tfUser.getText() + "\n" + mResources.password + ": " + tfPass.getText(), new Command(mResources.ACCEPT, this, 4000, null), null, new Command(mResources.NO, GameCanvas.instance, 8882, null));
			}
			GameCanvas.currentDialog = GameCanvas.msgdlg;
		}

	protected void doRegister(string user)
		{
			isFAQ = false;
			GameCanvas.startWaitDlg(mResources.CONNECTING);
			GameCanvas.connect();
			GameCanvas.startWaitDlg(mResources.REGISTERING);
			passRe = tfPass.getText();
			Service.gI().requestRegister(user, tfPass.getText(), Rms.loadRMSString(Rms.RMS_userAo + ServerListScreen.ipSelect), Rms.loadRMSString("passAo" + ServerListScreen.ipSelect), GameMidlet.VERSION);
			Rms.saveRMSString(Rms.RMS_acc, user);
			Rms.saveRMSString(Rms.RMS_pass, tfPass.getText());
			t = 20;
			isRegistering = true;
		}

	public void doViewFAQ()
		{
			if (!listFAQ.Equals(string.Empty) || !listFAQ.Equals(string.Empty))
			{
			}
			if (!Session_ME.connected)
			{
				isFAQ = true;
				GameCanvas.connect();
			}
			GameCanvas.startWaitDlg();
		}

	protected void doSelectServer()
		{
			MyVector myVector = new MyVector();
			if (isLocal)
			{
				myVector.addElement(new Command("Server LOCAL", this, 20004, null));
			}
			myVector.addElement(new Command("Server Bokken", this, 20001, null));
			myVector.addElement(new Command("Server Shuriken", this, 20002, null));
			myVector.addElement(new Command("Server Tessen (mới)", this, 20003, null));
			GameCanvas.menu.startAt(myVector, 0);
			if (loadIndexServer() != -1 && !GameCanvas.isTouch)
			{
				GameCanvas.menu.menuSelectedItem = loadIndexServer();
			}
		}

	protected void saveIndexServer(int index)
		{
			Rms.saveRMSInt("indServer", index);
		}

	protected int loadIndexServer()
		{
			return Rms.loadRMSInt("indServer");
		}

	public void doLogin()
		{
			string text = Rms.loadRMSString(Rms.RMS_acc);
			string text2 = Rms.loadRMSString(Rms.RMS_pass);
			if (text != null && !text.Equals(string.Empty))
			{
				isLogin2 = false;
			}
			else if (Rms.loadRMSString(Rms.RMS_userAo + ServerListScreen.ipSelect) != null && !Rms.loadRMSString(Rms.RMS_userAo + ServerListScreen.ipSelect).Equals(string.Empty))
			{
				isLogin2 = true;
			}
			else
			{
				isLogin2 = false;
			}
			if ((text == null || text.Equals(string.Empty)) && isLogin2)
			{
				text = Rms.loadRMSString(Rms.RMS_userAo + ServerListScreen.ipSelect);
				text2 = "a";
			}
			if (text == null || text2 == null || GameMidlet.VERSION == null || text.Equals(string.Empty))
			{
				return;
			}
			if (text2.Equals(string.Empty))
			{
				focus = 1;
				tfUser.isFocus = false;
				tfPass.isFocus = true;
				if (!GameCanvas.isTouch)
				{
					right = tfPass.cmdClear;
				}
				return;
			}
			if (!Session_ME.gI().isConnected())
			{
				GameCanvas.connect();
			}
			Service.gI().login(text, text2, GameMidlet.VERSION, (sbyte)(isLogin2 ? 1 : 0));
			Res.outz(Controller.isEXTRA_LINK + " = Controller.isEXTRA_LINK " + text + " " + text2 + " " + GameMidlet.VERSION + " " + (sbyte)(isLogin2 ? 1 : 0));
			Rms.saveRMSInt(ServerListScreen.RMS_svselect, ServerListScreen.ipSelect);
			if (Session_ME.connected)
			{
				GameCanvas.startWaitDlg();
			}
			else
			{
				GameCanvas.startOK(mResources.maychutathoacmatsong + " [0]", 8884, null);
			}
			focus = 0;
			if (!isLogin2)
			{
				actRegisterLeft();
			}
			GameCanvas.timeBreakLoading = mSystem.currentTimeMillis() + 30000;
		}

	public void savePass()
		{
			if (isCheck)
			{
				Rms.saveRMSInt(Rms.RMS_check, 1);
				Rms.saveRMSString(Rms.RMS_acc, tfUser.getText().ToLower().Trim());
				Rms.saveRMSString(Rms.RMS_pass, tfPass.getText().ToLower().Trim());
			}
			else
			{
				Rms.saveRMSInt(Rms.RMS_check, 2);
				Rms.saveRMSString(Rms.RMS_acc, string.Empty);
				Rms.saveRMSString(Rms.RMS_pass, string.Empty);
			}
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

	public void resetLogo()
		{
			yL = -50;
		}

	public void actRegisterLeft()
		{
			if (isLogin2)
			{
				doLogin();
				return;
			}
			isRes = false;
			tfPass.isFocus = false;
			tfUser.isFocus = true;
			left = cmdMenu;
		}

	public void actRegister()
		{
			GameCanvas.endDlg();
			isRes = true;
			tfPass.isFocus = false;
			tfUser.isFocus = true;
		}

	public void backToRegister()
		{
			GameCanvas.timeBreakLoading = mSystem.currentTimeMillis() + 30000;
			ServerListScreen.countDieConnect = 0;
			if (GameCanvas.loginScr.isLogin2)
			{
				GameCanvas.startYesNoDlg(mResources.note, new Command(mResources.YES, GameCanvas.panel, 10019, null), new Command(mResources.NO, GameCanvas.panel, 10020, null));
				return;
			}
			if (Main.isWindowsPhone)
			{
				GameMidlet.isBackWindowsPhone = true;
			}
			GameCanvas.instance.resetToLoginScr = false;
			ServerListScreen.isAutoLogin = false;
			ServerScr.isShowSv_HaveChar = false;
			GameCanvas.instance.doResetToLoginScr(GameCanvas.serverScreen);
		}

}
