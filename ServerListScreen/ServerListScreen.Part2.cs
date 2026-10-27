using System;
using UnityEngine;
public partial class ServerListScreen : mScreen, IActionListener
{
	public static void ResetSecondaryConnection()
	{
		Session_ME2.gI().clearSendingMessage();
		Session_ME2.gI().close();
		Controller.clearClientTypeConnect(isMainSession: false);
		Service.resetClientTypeSendState(isMainSession: false);
		GameMidlet.IP2 = null;
		GameMidlet.PORT2 = 0;
		GameMidlet.isConnect2 = false;
	}

	public static void ApplySecondaryEndpointFromHandshake(string requestedHost, int requestedPort, bool serverRequestedSecondary, bool isMainSession)
	{
		if (isMainSession)
		{
			ResetSecondaryConnection();
			Debug.Log(string.Format("[SECONDARY] serverIndex={0} main={1}:{2} handshakeEndpoint={3}:{4} serverFlag={5} secondaryEnabled=false reason=forced_disabled", ipSelect, GameMidlet.IP, GameMidlet.PORT, requestedHost, requestedPort, serverRequestedSecondary));
		}
		else
		{
			Debug.Log(string.Format("[SECONDARY] serverIndex={0} main={1}:{2} handshakeEndpoint={3}:{4} serverFlag={5} secondaryEnabled=false reason=non_main_handshake_ignored", ipSelect, GameMidlet.IP, GameMidlet.PORT, requestedHost, requestedPort, serverRequestedSecondary));
		}
	}

	public static void clearPendingLoginActionAfterImageSource()
	{
		pendingLoginActionAfterImageSource = 0;
		loginActionPending = 0;
	}

	public static bool performPendingLoginActionAfterImageSource(bool allowLoginAction = true)
	{
		if (pendingLoginActionAfterImageSource == 0 || !Service.isLoginReadyForCurrentConnection())
		{
			return false;
		}
		int action = pendingLoginActionAfterImageSource;
		clearPendingLoginActionAfterImageSource();
		if (allowLoginAction)
		{
			if (GameCanvas.serverScreen == null)
			{
				GameCanvas.serverScreen = new ServerListScreen();
			}
			GameCanvas.serverScreen.performLoginAction(action);
		}
		return true;
	}

	private void connectBeforeLogin(int action)
	{
		if (address == null || address.Length == 0 || ipSelect < 0 || ipSelect >= address.Length)
		{
			int idx = (serverPriority >= 0 && address != null && serverPriority < address.Length) ? serverPriority : 0;
			SetIpSelect(idx, issave: true);
		}
		loginActionPending = 0;
		if (Service.isLoginReadyForCurrentConnection())
		{
			clearPendingLoginActionAfterImageSource();
			performLoginAction(action);
			return;
		}
		pendingLoginActionAfterImageSource = action;
		loginActionPending = action;
		if (Session_ME.connecting || Session_ME.gI().isConnected())
		{
			GameCanvas.startWaitDlg(mResources.CONNECTING);
			return;
		}
		flagServer = 1;
		countConnectServer = 1;
		timeNextConnectServer = mSystem.currentTimeMillis() + 1000L;
		GameCanvas.startWaitDlg(mResources.CONNECTING);
		Session_ME.gI().close();
		testConnect = 0;
		ConnectIP();
	}

	private void performLoginAction(int action)
	{
		if (action == 3)
		{
			Login_New();
		}
		else if (action == 10100)
		{
			actPlayNew();
		}
	}

	private void actPlayNew()
	{
		GameScr.resetItemUpdateFlags();
		try
		{
			Rms.deleteRecord(Rms.RMS_userAo + ipSelect);
			Rms.deleteRecord("passAo" + ipSelect);
			Rms.saveRMSString(Rms.RMS_userAo + ipSelect, string.Empty);
			Rms.saveRMSString("passAo" + ipSelect, string.Empty);
		}
		catch (Exception)
		{
		}
		if (GameCanvas.loginScr == null)
		{
			GameCanvas.loginScr = new LoginScr();
		}
		GameCanvas.loginScr.isLogin2 = true;
		GameCanvas.loginScr.switchToMe();
		DragonBoy_Net8_Native.Src.Mod.Security.ModCredentialSecurity.OnLoginStarted();
		Service.gI().login2(string.Empty);
		Res.outz("tao user ao");
		GameCanvas.startWaitDlg(mResources.PLEASEWAIT);
		if (nameServer != null && ipSelect >= 0 && ipSelect < nameServer.Length)
		{
			LoginScr.serverName = nameServer[ipSelect];
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
			Res.outz(">>>>switchToMe  ServerListScreen:\n" + Environment.StackTrace);
			loginActionPending = 0;
			GameCanvas.clearTransientPopupState();
			GameCanvas.clearAllPointerEvent();
			GameCanvas.clearKeyPressed();
			GameCanvas.endDlg();
			EffectManager.remove();
			GameScr.cmy = 0;
			GameScr.cmx = 0;
			int savedSv = Rms.loadRMSInt(RMS_svselect);
			if (savedSv >= 0 && nameServer != null && savedSv < nameServer.Length)
			{
				ipSelect = savedSv;
			}
			initCommand();
			isWait = false;
			GameCanvas.loginScr = null;
			loadScreen = true;
			GameCanvas.loadBG(0);
			bigOk = true;
			selected = 0;
			string sName = (nameServer != null && ipSelect >= 0 && ipSelect < nameServer.Length) ? nameServer[ipSelect] : string.Empty;
			if (cmd != null && cmd.Length > 2 + nCmdPlay && cmd[2 + nCmdPlay] != null)
			{
				cmd[2 + nCmdPlay].caption = mResources.server + ": " + sName;
			}
			center = new Command(string.Empty, this, cmd[selected].idAction, null);
			if (cmd != null && cmd.Length > 1 + nCmdPlay && cmd[1 + nCmdPlay] != null)
			{
				cmd[1 + nCmdPlay].caption = mResources.change_account;
			}
			if (cmd != null && cmd.Length == 4 + nCmdPlay && cmd[3 + nCmdPlay] != null)
			{
				cmd[3 + nCmdPlay].caption = mResources.option;
			}
			Char.isLoadingMap = false;
			mSystem.resetCurInapp();
			count_reConnect = 0;
			DragonBoy_Net8_Native.Src.Mod.Security.ModCredentialSecurity.OnLoginFinished();
			if (!Session_ME.gI().isConnected() && !Session_ME.connecting)
			{
				ConnectIP();
			}
			base.switchToMe();
		}
	public void switchToMe2()
		{
			loginActionPending = 0;
			GameCanvas.endDlg();
			GameScr.cmy = 0;
			GameScr.cmx = 0;
			int savedSv = Rms.loadRMSInt(RMS_svselect);
			if (savedSv >= 0 && nameServer != null && savedSv < nameServer.Length)
			{
				ipSelect = savedSv;
			}
			initCommand();
			isWait = false;
			GameCanvas.loginScr = null;
			loadScreen = true;
			GameCanvas.loadBG(0);
			bigOk = true;
			string sName = (nameServer != null && ipSelect >= 0 && ipSelect < nameServer.Length) ? nameServer[ipSelect] : string.Empty;
			if (cmd != null && cmd.Length > 2 + nCmdPlay && cmd[2 + nCmdPlay] != null)
			{
				cmd[2 + nCmdPlay].caption = mResources.server + ": " + sName;
			}
			center = new Command(string.Empty, this, cmd[selected].idAction, null);
			if (cmd != null && cmd.Length > 1 + nCmdPlay && cmd[1 + nCmdPlay] != null)
			{
				cmd[1 + nCmdPlay].caption = mResources.change_account;
			}
			if (cmd != null && cmd.Length == 4 + nCmdPlay && cmd[3 + nCmdPlay] != null)
			{
				cmd[3 + nCmdPlay].caption = mResources.option;
			}
			mSystem.resetCurInapp();
			DragonBoy_Net8_Native.Src.Mod.Security.ModCredentialSecurity.OnLoginFinished();
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
			isGetData = false;
			loadScreen = true;
			bigOk = true;
			GameCanvas.serverScreen.show2();
		}
	public void init()
		{
			if (cmd != null)
			{
				int numY = GameCanvas.hh - 15 * cmd.Length + 28;
				for (int i = 0; i < cmd.Length; i++)
				{
					if (cmd[i] != null)
					{
						cmd[i].x = (GameCanvas.w - cmd[i].w) / 2;
						cmd[i].y = numY + i * 30;
					}
				}
			}
			if (cmd_New_Ui != null)
			{
				int num = GameCanvas.hh - 15 * cmd_New_Ui.Length + 28;
				for (int i = 0; i < cmd_New_Ui.Length; i++)
				{
					if (cmd_New_Ui[i] != null)
					{
						cmd_New_Ui[i].x = (GameCanvas.w - cmd_New_Ui[i].w) / 2;
						cmd_New_Ui[i].y = num;
						num += 30;
					}
				}
			}
			if (cmdCallHotline != null)
			{
				cmdCallHotline.x = GameCanvas.w - 75;
				if (mSystem.clientType == 1 && !GameCanvas.isTouch)
				{
					cmdCallHotline.y = GameCanvas.h - 20;
				}
				else
				{
					cmdCallHotline.y = 8;
				}
			}
			if (cmdDeleteRMS != null)
			{
				cmdDeleteRMS.x = GameCanvas.w - 78;
				cmdDeleteRMS.y = GameCanvas.h - 26;
			}
			if (!loadScreen)
			{
				if (isGetData)
				{
					cmdDownload = new Command(mResources.huy, this, 4, null);
					cmdDownload.x = GameCanvas.w / 2 - mScreen.cmdW / 2;
					cmdDownload.y = GameCanvas.hh + 56;
					if (!GameCanvas.isTouch)
					{
						cmdDownload.y = GameCanvas.h - mScreen.cmdH - 1;
					}
					else if (cmdDownload.y > GameCanvas.h - mScreen.cmdH - 2)
					{
						cmdDownload.y = GameCanvas.h - mScreen.cmdH - 2;
					}
				}
				else
				{
					cmdDownload = new Command(mResources.taidulieu, this, 2, null);
					cmdDownload.isFocus = true;
					cmdDownload.x = GameCanvas.w / 2 - mScreen.cmdW / 2;
					cmdDownload.y = GameCanvas.hh + 45;
					if (cmdDownload.y > GameCanvas.h - mScreen.cmdH - 2)
					{
						cmdDownload.y = GameCanvas.h - mScreen.cmdH - 2;
					}
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
			loginActionPending = 0;
			flagServer = 0;
			countConnectServer = 0;
			timeNextConnectServer = 0L;
			GameCanvas.endDlg();
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
			ModProxyServer.ResolveSelectedServerEndpoint(ref GameMidlet.IP, ref GameMidlet.PORT);
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
			ModNextMap.StopNextMap();
			GameScr.resetItemUpdateFlags();
			bool flag = false;
			bool flag2 = false;
			string text = Rms.loadRMSString(Rms.RMS_userAo + ipSelect);
			try
			{
				string acc = Rms.loadRMSString(Rms.RMS_acc);
				string pass = DragonBoy_Net8_Native.Src.Mod.Security.ModCredentialSecurity.LoadSavedPassword();
				if (string.IsNullOrEmpty(pass))
				{
					pass = Rms.loadRMSString(Rms.RMS_pass);
				}
				if (!string.IsNullOrEmpty(acc) && !string.IsNullOrEmpty(pass))
				{
					flag = true;
				}
				if (!string.IsNullOrEmpty(text))
				{
					flag2 = true;
				}
			}
			catch (Exception)
			{
			}
			if (!flag && !flag2)
			{
				if (GameCanvas.loginScr == null)
				{
					GameCanvas.loginScr = new LoginScr();
				}
				GameCanvas.loginScr.switchToMe();
				DragonBoy_Net8_Native.Src.Mod.Security.ModCredentialSecurity.OnLoginStarted();
				GameCanvas.connect();
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
				GameCanvas.startWaitDlg(mResources.PLEASEWAIT);
			}
			else if (flag)
			{
				if (GameCanvas.loginScr == null)
				{
					GameCanvas.loginScr = new LoginScr();
				}
				GameCanvas.loginScr.doLogin();
			}
			else if (flag2)
			{
				if (GameCanvas.loginScr == null)
				{
					GameCanvas.loginScr = new LoginScr();
				}
				GameCanvas.loginScr.switchToMe();
				DragonBoy_Net8_Native.Src.Mod.Security.ModCredentialSecurity.OnLoginStarted();
				GameCanvas.connect();
				GameCanvas.loginScr.isLogin2 = true;
				Service.gI().login(text, string.Empty, GameMidlet.VERSION, 1);
				Rms.saveRMSInt(RMS_svselect, ipSelect);
				GameCanvas.startWaitDlg(mResources.PLEASEWAIT);
			}
			if (nameServer != null && ipSelect >= 0 && ipSelect < nameServer.Length)
			{
				LoginScr.serverName = nameServer[ipSelect];
			}
		}
}
