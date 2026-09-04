using System;
using UnityEngine;

public partial class ServerListScreen : mScreen, IActionListener
{
	public static string[] nameServer;

	public static string[] address;

	public static sbyte serverPriority;

	public static bool[] hasConnected;

	public static short[] port;

	public static int selected;

	public static bool isWait;

	public static Command cmdUpdateServer;

	public static sbyte[] language;

	public static sbyte[] typeSv;

	public static sbyte[] isNew;

	public static sbyte[] typeClass;

	public static Char[] listChar;

	public static bool isHaveChar;

	private Command[] cmd;

	private Command cmdCallHotline;

	private int nCmdPlay;

	public static Command cmdDeleteRMS;

	private int lY;

	public static string smartPhoneVN = "Vũ trụ 1:dragon1.teamobi.com:14445:0:0:0,Vũ trụ 2:dragon2.teamobi.com:14445:0:0:0,Vũ trụ 3:dragon3.teamobi.com:14445:0:0:0,Vũ trụ 4:dragon4.teamobi.com:14445:0:0:0,Vũ trụ 5:dragon5.teamobi.com:14445:0:0:0,Vũ trụ 6:dragon6.teamobi.com:14445:0:0:0,Vũ trụ 7:dragon7.teamobi.com:14445:0:0:0,Vũ trụ 8:dragon10.teamobi.com:14446:0:0:0,Vũ trụ 9:dragon10.teamobi.com:14447:0:0:0,Vũ trụ 10:dragon10.teamobi.com:14445:0:0:0,Vũ trụ 11:dragon11.teamobi.com:14445:0:0:0,Võ đài liên vũ trụ:dragonwar.teamobi.com:20000:0:0:0,Universe 1:dragon.indonaga.com:14445:1:0:0,Naga:dragon.indonaga.com:14446:2:0:0,0,0";

	public static string javaVN = "Vũ trụ 1:112.213.94.23:14445:0:0:0,Vũ trụ 2:210.211.109.199:14445:0:0:0,Vũ trụ 3:112.213.85.88:14445:0:0:0,Vũ trụ 4:27.0.12.164:14445:0:0:0,Vũ trụ 5:27.0.12.16:14445:0:0:0,Vũ trụ 6:27.0.12.173:14445:0:0:0,Vũ trụ 7:112.213.94.223:14445:0:0:0,Vũ trụ 8:27.0.14.66:14446:0:0:0,Vũ trụ 9:27.0.14.66:14447:0:0:0,Vũ trụ 10:27.0.14.66:14445:0:0:0,Vũ trụ 11:112.213.85.35:14445:0:0:0,Võ đài liên vũ trụ:27.0.12.173:20000:0:0:0,Universe 1:52.74.230.22:14445:1:0:0,Naga:52.74.230.22:14446:2:0:0,0,0";

	public static string smartPhoneIn = "Naga:dragon.indonaga.com:14446:2:0:0,2,0";

	public static string javaIn = "Naga:52.74.230.22:14446:2:0:0,2,0";

	public static string smartPhoneE = "Universe 1:dragon.indonaga.com:14445:1:0:0,1,0";

	public static string javaE = "Universe 1:52.74.230.22:14445:1:0:0,1,0";

	public static string linkGetHost = "http://112.213.94.23/mod/server_extra.php";

	public static string linkDefault = javaVN;

	public const sbyte languageVersion = 2;

	public new int keyTouch = -1;

	private int tam;

	public static bool stopDownload;

	public static string linkweb = "http://ngocrongonline.com";

	public static int countDieConnect;

	public static bool waitToLogin;

	public static int tWaitToLogin;

	public static long count_reConnect;

	public static string RMS_NRlink = "NRlink3";

	public static int ipSelect;

	public static int flagServer;

	public static bool bigOk;

	public static int percent;

	public static string strWait;

	public static int nBig;

	public static int nBg;

	public static int demPercent;

	public static int maxBg;

	public static bool isGetData = false;

	public static Command cmdDownload;

	private Command cmdStart;

	public string dataSize;

	public static int p;

	public static int testConnect = -1;

	public static bool loadScreen;

	public static bool isAutoConect = true;

	public static string RMS_svselect = "svselect";

	public static string RMS_NR_Extralink = "NRlink_extra";

	private Command[] cmd_New_Ui;

	public static bool isNewUI;

	public static bool isAutoLogin = true;

	public ServerListScreen()
		{
			int num = 4;
			int num2 = num * 32 + 23 + 33;
			if (num2 >= GameCanvas.w)
			{
				num--;
				num2 = num * 32 + 23 + 33;
			}
			initCommand();
			if (!GameCanvas.isTouch)
			{
				selected = 0;
				processInput();
			}
			GameScr.loadCamera(fullmScreen: true, -1, -1);
			GameScr.cmx = 100;
			GameScr.cmy = 200;
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
					int num3 = 2;
					cmdCallHotline.y = num3 + 6;
				}
			}
			cmdUpdateServer = new Command();
			cmdUpdateServer.actionChat = delegate(string str)
			{
				string text = str;
				string text2 = str;
				if (text == null)
				{
					text = linkDefault;
				}
				else
				{
					if (text == null && text2 != null)
					{
						if (text2.Equals(string.Empty) || text2.Length < 20)
						{
							text2 = linkDefault;
						}
						getServerList(text2);
					}
					if (text != null && text2 == null)
					{
						if (text.Equals(string.Empty) || text.Length < 20)
						{
							text = linkDefault;
						}
						getServerList(text);
					}
					if (text != null && text2 != null)
					{
						if (text.Length > text2.Length)
						{
							getServerList(text);
						}
						else
						{
							getServerList(text2);
						}
					}
				}
			};
			setLinkDefault(mSystem.LANGUAGE);
		}

	public static void createDeleteRMS()
		{
			if (cmdDeleteRMS == null)
			{
				if (GameCanvas.serverScreen == null)
				{
					GameCanvas.serverScreen = new ServerListScreen();
				}
				cmdDeleteRMS = new Command(string.Empty, GameCanvas.serverScreen, 14, null);
				cmdDeleteRMS.x = GameCanvas.w - 78;
				cmdDeleteRMS.y = GameCanvas.h - 26;
			}
		}

	private void initCommand()
		{
			nCmdPlay = 0;
			string text = Rms.loadRMSString(Rms.RMS_acc);
			if (text == null)
			{
				if (Rms.loadRMS(Rms.RMS_userAo + ipSelect) != null)
				{
					nCmdPlay = 1;
				}
			}
			else if (text.Equals(string.Empty))
			{
				if (Rms.loadRMS(Rms.RMS_userAo + ipSelect) != null)
				{
					nCmdPlay = 1;
				}
			}
			else
			{
				nCmdPlay = 1;
			}
			cmd = new Command[(mGraphics.zoomLevel <= 1) ? (4 + nCmdPlay) : (3 + nCmdPlay)];
			int num = GameCanvas.hh - 15 * cmd.Length + 28;
			for (int i = 0; i < cmd.Length; i++)
			{
				switch (i)
				{
				case 0:
					cmd[0] = new Command(string.Empty, this, 3, null);
					if (text == null)
					{
						cmd[0].caption = mResources.playNew;
						if (Rms.loadRMS(Rms.RMS_userAo + ipSelect) != null)
						{
							cmd[0].caption = mResources.choitiep;
						}
						break;
					}
					if (text.Equals(string.Empty))
					{
						cmd[0].caption = mResources.playNew;
						if (Rms.loadRMS(Rms.RMS_userAo + ipSelect) != null)
						{
							cmd[0].caption = mResources.choitiep;
						}
						break;
					}
					cmd[0].caption = mResources.playAcc + ": " + text;
					if (cmd[0].caption.Length > 23)
					{
						cmd[0].caption = cmd[0].caption.Substring(0, 23);
						cmd[0].caption += "...";
					}
					break;
				case 1:
					if (nCmdPlay == 1)
					{
						cmd[1] = new Command(string.Empty, this, 10100, null);
						cmd[1].caption = mResources.playNew;
					}
					else
					{
						cmd[1] = new Command(mResources.change_account, this, 7, null);
					}
					break;
				case 2:
					if (nCmdPlay == 1)
					{
						cmd[2] = new Command(mResources.change_account, this, 7, null);
					}
					else
					{
						cmd[2] = new Command(string.Empty, this, 17, null);
					}
					break;
				case 3:
					if (nCmdPlay == 1)
					{
						cmd[3] = new Command(string.Empty, this, 17, null);
					}
					else
					{
						cmd[3] = new Command(mResources.option, this, 8, null);
					}
					break;
				case 4:
					cmd[4] = new Command(mResources.option, this, 8, null);
					break;
				}
				cmd[i].y = num;
				cmd[i].setType();
				cmd[i].x = (GameCanvas.w - cmd[i].w) / 2;
				num += 30;
			}
		}

	public static void doUpdateServer()
		{
			if (cmdUpdateServer == null && GameCanvas.serverScreen == null)
			{
				GameCanvas.serverScreen = new ServerListScreen();
			}
			Net.connectHTTP2(linkDefault, cmdUpdateServer);
		}

	public static void getServerList(string str)
		{
			string[] array = Res.split(str.Trim(), ",", 0);
			Res.outz(">>> getServerList= " + str);
			mResources.loadLanguague(sbyte.Parse(array[array.Length - 2]));
			nameServer = new string[array.Length - 2];
			address = new string[array.Length - 2];
			port = new short[array.Length - 2];
			language = new sbyte[array.Length - 2];
			typeSv = new sbyte[array.Length - 2];
			isNew = new sbyte[array.Length - 2];
			hasConnected = new bool[2];
			for (int i = 0; i < array.Length - 2; i++)
			{
				string[] array2 = Res.split(array[i].Trim(), ":", 0);
				nameServer[i] = array2[0];
				address[i] = array2[1];
				port[i] = short.Parse(array2[2]);
				language[i] = sbyte.Parse(array2[3].Trim());
				try
				{
					typeSv[i] = sbyte.Parse(array2[4].Trim());
				}
				catch (Exception)
				{
					typeSv[i] = 0;
				}
				try
				{
					isNew[i] = sbyte.Parse(array2[5].Trim());
				}
				catch (Exception)
				{
					isNew[i] = 0;
				}
			}
			serverPriority = sbyte.Parse(array[array.Length - 1]);
			Res.outz(">>> getServerList= serverPriority: " + serverPriority);
			if (ipSelect < 0 || ipSelect >= nameServer.Length)
			{
				SetIpSelect((serverPriority >= 0 && serverPriority < nameServer.Length) ? serverPriority : 0, issave: true);
			}
			saveIP();
		}

	public override void update()
		{
			if (waitToLogin)
			{
				tWaitToLogin++;
				if (tWaitToLogin >= 1)
				{
					GameCanvas.serverScreen.selectServer();
					if (GameCanvas.loginScr == null)
					{
						GameCanvas.loginScr = new LoginScr();
					}
					GameCanvas.loginScr.switchToMe();
					waitToLogin = false;
				}
			}
			if (flagServer > 0)
			{
				flagServer--;
				if (flagServer == 0)
				{
					GameCanvas.endDlg();
				}
			}
			for (int i = 0; i < cmd.Length; i++)
			{
				if (i == selected)
				{
					cmd[i].isFocus = true;
				}
				else
				{
					cmd[i].isFocus = false;
				}
			}
			GameScr.cmx++;
			if (!loadScreen && (bigOk || percent == 100))
			{
				cmdDownload = null;
			}
			base.update();
			if (Char.isLoadingMap || !loadScreen || !isAutoConect || GameCanvas.currentScreen != this)
			{
				return;
			}
			if (!Session_ME.gI().isConnected() && !Session_ME.connecting)
			{
				if (mSystem.currentTimeMillis() > count_reConnect)
				{
					SetIpSelect(ipSelect, issave: true);
					Session_ME.gI().close();
					ConnectIP();
					count_reConnect = mSystem.currentTimeMillis() + 5000;
				}
			}
			else
			{
				count_reConnect = mSystem.currentTimeMillis() + 5000;
			}
		}

	private void processInput()
		{
			if (loadScreen)
			{
				center = new Command(string.Empty, this, cmd[selected].idAction, null);
			}
			else
			{
				center = cmdDownload;
			}
		}

	public static void updateDeleteData()
		{
			if (cmdDeleteRMS != null && cmdDeleteRMS.isPointerPressInside())
			{
				cmdDeleteRMS.performAction();
			}
		}

	public static void saveIP()
		{
			DataOutputStream dataOutputStream = new DataOutputStream();
			try
			{
				dataOutputStream.writeByte(mResources.language);
				dataOutputStream.writeByte((sbyte)nameServer.Length);
				for (int i = 0; i < nameServer.Length; i++)
				{
					dataOutputStream.writeUTF(nameServer[i]);
					dataOutputStream.writeUTF(address[i]);
					dataOutputStream.writeShort(port[i]);
					dataOutputStream.writeByte(language[i]);
					try
					{
						dataOutputStream.writeByte(typeSv[i]);
					}
					catch (Exception)
					{
						dataOutputStream.writeByte(0);
					}
					try
					{
						dataOutputStream.writeByte(isNew[i]);
					}
					catch (Exception)
					{
						dataOutputStream.writeByte(0);
					}
				}
				dataOutputStream.writeByte(serverPriority);
				Rms.saveRMS(RMS_NRlink, dataOutputStream.toByteArray());
				dataOutputStream.close();
				SplashScr.loadIP();
			}
			catch (Exception)
			{
			}
		}

	public static bool allServerConnected()
		{
			for (int i = 0; i < 2; i++)
			{
				if (!hasConnected[i])
				{
					return false;
				}
			}
			return true;
		}

	public static void loadIP()
		{
			sbyte[] array = Rms.loadRMS(RMS_NRlink);
			if (array == null)
			{
				getServerList(linkDefault);
				return;
			}
			DataInputStream dataInputStream = new DataInputStream(array);
			if (dataInputStream == null)
			{
				return;
			}
			try
			{
				mResources.loadLanguague(dataInputStream.readByte());
				sbyte b = dataInputStream.readByte();
				nameServer = new string[b];
				address = new string[b];
				port = new short[b];
				language = new sbyte[b];
				typeSv = new sbyte[b];
				isNew = new sbyte[b];
				for (int i = 0; i < b; i++)
				{
					nameServer[i] = dataInputStream.readUTF();
					address[i] = dataInputStream.readUTF();
					port[i] = dataInputStream.readShort();
					language[i] = dataInputStream.readByte();
					try
					{
						typeSv[i] = dataInputStream.readByte();
					}
					catch (Exception)
					{
						typeSv[i] = 0;
					}
					try
					{
						isNew[i] = dataInputStream.readByte();
					}
					catch (Exception)
					{
						isNew[i] = 0;
					}
				}
				serverPriority = dataInputStream.readByte();
				dataInputStream.close();
				if (ipSelect < 0 || (nameServer != null && ipSelect >= nameServer.Length))
				{
					SetIpSelect((serverPriority >= 0 && nameServer != null && serverPriority < nameServer.Length) ? serverPriority : 0, issave: true);
				}
				SplashScr.loadIP();
			}
			catch (Exception)
			{
			}
		}

	public override void switchToMe()
		{
			Res.outz(">>>>switchToMe  ServerListScreen: ");
			EffectManager.remove();
			GameScr.cmy = 0;
			GameScr.cmx = 0;
			initCommand();
			isWait = false;
			GameCanvas.loginScr = null;
			loadScreen = true;
			GameCanvas.loadBG(0);
			bigOk = true;
			string sName = (nameServer != null && ipSelect >= 0 && ipSelect < nameServer.Length) ? nameServer[ipSelect] : string.Empty;
			cmd[2 + nCmdPlay].caption = mResources.server + ": " + sName;
			center = new Command(string.Empty, this, cmd[selected].idAction, null);
			cmd[1 + nCmdPlay].caption = mResources.change_account;
			if (cmd.Length == 4 + nCmdPlay)
			{
				cmd[3 + nCmdPlay].caption = mResources.option;
			}
			Char.isLoadingMap = false;
			mSystem.resetCurInapp();
			count_reConnect = 0;
			if (!Session_ME.gI().isConnected() && !Session_ME.connecting)
			{
				ConnectIP();
			}
			base.switchToMe();
		}

	public void switchToMe2()
		{
			GameScr.cmy = 0;
			GameScr.cmx = 0;
			initCommand();
			isWait = false;
			GameCanvas.loginScr = null;
			loadScreen = true;
			GameCanvas.loadBG(0);
			bigOk = true;
			string sName = (nameServer != null && ipSelect >= 0 && ipSelect < nameServer.Length) ? nameServer[ipSelect] : string.Empty;
			cmd[2 + nCmdPlay].caption = mResources.server + ": " + sName;
			center = new Command(string.Empty, this, cmd[selected].idAction, null);
			cmd[1 + nCmdPlay].caption = mResources.change_account;
			if (cmd.Length == 4 + nCmdPlay)
			{
				cmd[3 + nCmdPlay].caption = mResources.option;
			}
			mSystem.resetCurInapp();
			base.switchToMe();
		}

	public void connectOk()
		{
		}

	public void cancel()
		{
			if (GameCanvas.serverScreen == null)
			{
				GameCanvas.serverScreen = new ServerListScreen();
			}
			demPercent = 0;
			percent = 0;
			stopDownload = true;
			GameCanvas.serverScreen.show2();
			isGetData = false;
			cmdDownload.isFocus = true;
			center = new Command(string.Empty, this, 2, null);
		}

	public void init()
		{
			if (!loadScreen)
			{
				cmdDownload = new Command(mResources.taidulieu, this, 2, null);
				cmdDownload.isFocus = true;
				cmdDownload.x = GameCanvas.w / 2 - mScreen.cmdW / 2;
				cmdDownload.y = GameCanvas.hh + 45;
				if (cmdDownload.y > GameCanvas.h - 26)
				{
					cmdDownload.y = GameCanvas.h - 26;
				}
			}
			if (!GameCanvas.isTouch)
			{
				selected = 0;
				processInput();
			}
		}

	public void show2()
		{
			Debug.LogError(">>>>ServerListScreen show2: ");
			GameScr.cmx = 0;
			GameScr.cmy = 0;
			initCommand();
			loadScreen = false;
			percent = 0;
			bigOk = false;
			isGetData = false;
			p = 0;
			demPercent = 0;
			strWait = mResources.PLEASEWAIT;
			Char.isLoadingMap = false;
			init();
			base.switchToMe();
		}

	public void setLinkDefault(sbyte language)
		{
			if (language == 2)
			{
				if (mSystem.clientType == 1)
				{
					linkDefault = javaIn;
				}
				else
				{
					linkDefault = smartPhoneIn;
				}
			}
			else if (language == 1)
			{
				linkDefault = javaE;
				if (mSystem.clientType == 1)
				{
					linkDefault = javaE;
				}
				else
				{
					linkDefault = smartPhoneE;
				}
			}
			else
			{
				linkDefault = javaVN;
				if (mSystem.clientType == 1)
				{
					linkDefault = javaVN;
				}
				else
				{
					linkDefault = smartPhoneVN;
				}
			}
			mSystem.AddIpTest();
		}

	public static void ConnectIP()
		{
			if (address == null || address.Length == 0)
			{
				return;
			}
			if (ipSelect < 0 || ipSelect >= address.Length)
			{
				ipSelect = (serverPriority >= 0 && serverPriority < address.Length) ? serverPriority : 0;
			}
			GameMidlet.IP = address[ipSelect];
			GameMidlet.PORT = port[ipSelect];
			if (language != null && ipSelect < language.Length)
			{
				mResources.loadLanguague(language[ipSelect]);
			}
			if (nameServer != null && ipSelect < nameServer.Length)
			{
				LoginScr.serverName = nameServer[ipSelect];
			}
			GameCanvas.connect();
		}

	public void Login_New()
		{
			if (GameCanvas.loginScr == null)
			{
				GameCanvas.loginScr = new LoginScr();
			}
			GameCanvas.loginScr.switchToMe();
			bool flag = false;
			bool flag2 = false;
			string text = Rms.loadRMSString(Rms.RMS_userAo + ipSelect);
			try
			{
				if (!Rms.loadRMSString(Rms.RMS_acc).Equals(string.Empty))
				{
					flag = true;
				}
				if (!text.Equals(string.Empty))
				{
					flag2 = true;
				}
			}
			catch (Exception)
			{
			}
			if (!Session_ME.gI().isConnected())
			{
				GameCanvas.connect();
			}
			Service.gI().setClientType();
			if (!flag && !flag2)
			{
				if (text == null || text.Equals(string.Empty))
				{
					Debug.LogError(">>>>Login_New: login2: ");
					Service.gI().login2(string.Empty);
				}
				else
				{
					GameCanvas.loginScr.isLogin2 = true;
					Service.gI().login(text, string.Empty, GameMidlet.VERSION, 1);
				}
				Rms.saveRMSInt(RMS_svselect, ipSelect);
				if (Session_ME.connected)
				{
					GameCanvas.startWaitDlg();
				}
				else
				{
					GameCanvas.startOK(mResources.maychutathoacmatsong + " [3]", 8884, null);
				}
			}
			else
			{
				GameCanvas.loginScr.doLogin();
			}
			LoginScr.serverName = nameServer[ipSelect];
		}

	public static void LoadRMS_ExtraLink()
		{
			sbyte[] array = Rms.loadRMS(RMS_NR_Extralink);
			if (array == null)
			{
				Controller.isEXTRA_LINK = false;
				return;
			}
			DataInputStream dataInputStream = new DataInputStream(array);
			if (dataInputStream == null)
			{
				return;
			}
			try
			{
				sbyte b = dataInputStream.readByte();
				typeClass = new sbyte[b];
				listChar = new Char[b];
				for (int i = 0; i < b; i++)
				{
					typeClass[i] = dataInputStream.readByte();
					if (typeClass[i] > -1)
					{
						isHaveChar = true;
						listChar[i] = new Char();
						listChar[i].cgender = typeClass[i];
						listChar[i].head = dataInputStream.readShort();
						listChar[i].body = dataInputStream.readShort();
						listChar[i].leg = dataInputStream.readShort();
						listChar[i].bag = dataInputStream.readShort();
						listChar[i].cName = dataInputStream.readUTF();
					}
				}
				dataInputStream.close();
				Controller.isEXTRA_LINK = true;
			}
			catch (Exception)
			{
			}
		}

	public static void saveRMS_ExtraLink()
		{
			if (typeClass == null)
			{
				return;
			}
			DataOutputStream dataOutputStream = new DataOutputStream();
			try
			{
				dataOutputStream.writeByte((sbyte)typeClass.Length);
				for (int i = 0; i < typeClass.Length; i++)
				{
					dataOutputStream.writeByte(typeClass[i]);
					if (typeClass[i] > -1 && listChar != null && listChar[i] != null)
					{
						dataOutputStream.writeShort((short)listChar[i].head);
						dataOutputStream.writeShort((short)listChar[i].body);
						dataOutputStream.writeShort((short)listChar[i].leg);
						dataOutputStream.writeShort((short)listChar[i].bag);
						dataOutputStream.writeUTF(listChar[i].cName);
					}
				}
				Rms.saveRMS(RMS_NR_Extralink, dataOutputStream.toByteArray());
				dataOutputStream.close();
				SplashScr.loadIP();
			}
			catch (Exception)
			{
			}
		}

	public void Set_UI_New()
		{
			if (!GameCanvas.isTouch)
			{
				return;
			}
			isNewUI = true;
			cmd_New_Ui = new Command[2];
			int num = GameCanvas.hh - 15 * cmd_New_Ui.Length + 28;
			for (int i = 0; i < cmd_New_Ui.Length; i++)
			{
				switch (i)
				{
				case 0:
					cmd_New_Ui[0] = new Command(string.Empty, this, 3, null);
					cmd_New_Ui[0].caption = mResources.playNew;
					if (Rms.loadRMS(Rms.RMS_userAo + ipSelect) != null)
					{
						cmd_New_Ui[0].caption = mResources.choitiep;
					}
					break;
				case 1:
					cmd_New_Ui[1] = new Command(mResources.change_account, this, 7, null);
					break;
				}
				cmd_New_Ui[i].y = num;
				cmd_New_Ui[i].setType();
				cmd_New_Ui[i].x = (GameCanvas.w - cmd_New_Ui[i].w) / 2;
				num += 30;
			}
		}

	public static void CheckBack_ServerListScreen()
		{
			if (GameCanvas.serverScreen == null)
			{
				GameCanvas.serverScreen = new ServerListScreen();
			}
			bool flag = false;
			bool flag2 = false;
			try
			{
				if (!Rms.loadRMSString(Rms.RMS_acc).Equals(string.Empty))
				{
					flag = true;
				}
				if (!Rms.loadRMSString(Rms.RMS_userAo + ipSelect).Equals(string.Empty))
				{
					flag2 = true;
				}
			}
			catch (Exception)
			{
			}
			Debug.LogError(">>>>CheckBack_ServerListScreen: " + ipSelect + "  auto login:" + isAutoLogin);
			if (ipSelect == -1 || !isAutoLogin)
			{
				GameCanvas.serverScreen.switchToMe();
				return;
			}
			if (!flag && !flag2)
			{
				GameCanvas.serverScreen.switchToMe();
				return;
			}
			Controller.isEXTRA_LINK = false;
			GameCanvas.serverScreen.switchToMe();
			GameCanvas.serverScreen.Login_New();
		}

}
