using System;
using UnityEngine;
public partial class ServerListScreen : mScreen, IActionListener
{
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

}
