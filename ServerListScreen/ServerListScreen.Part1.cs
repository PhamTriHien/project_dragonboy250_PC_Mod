using System;
using UnityEngine;
public partial class ServerListScreen : mScreen, IActionListener
{
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

}
