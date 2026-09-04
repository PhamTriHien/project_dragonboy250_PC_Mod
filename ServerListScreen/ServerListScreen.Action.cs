using System;
using UnityEngine;

public partial class ServerListScreen : mScreen, IActionListener
{
	public void selectServer()
		{
			if (address == null || address.Length == 0)
			{
				return;
			}
			if (ipSelect < 0 || ipSelect >= address.Length)
			{
				ipSelect = (serverPriority >= 0 && serverPriority < address.Length) ? serverPriority : 0;
			}
			flagServer = 30;
			GameCanvas.startWaitDlg(mResources.PLEASEWAIT);
			Session_ME.gI().close();
			GameMidlet.IP = address[ipSelect];
			GameMidlet.PORT = port[ipSelect];
			if (language != null && ipSelect < language.Length)
			{
				GameMidlet.LANGUAGE = language[ipSelect];
				if (language[ipSelect] != mResources.language)
				{
					mResources.loadLanguague(language[ipSelect]);
				}
			}
			Rms.saveRMSInt(RMS_svselect, ipSelect);
			Res.err("1>>>saveRMSInt:  RMS_svselect == " + ipSelect);
			if (nameServer != null && ipSelect < nameServer.Length)
			{
				LoginScr.serverName = nameServer[ipSelect];
			}
			initCommand();
			loadScreen = true;
			countDieConnect = 0;
			Controller.isConnectOK = false;
			testConnect = -1;
			isAutoConect = true;
		}

	public override void updateKey()
		{
			if (GameCanvas.isTouch)
			{
				updateDeleteData();
				if (cmdCallHotline != null && cmdCallHotline.isPointerPressInside())
				{
					cmdCallHotline.performAction();
				}
				if (!loadScreen)
				{
					if (cmdDownload != null && cmdDownload.isPointerPressInside())
					{
						cmdDownload.performAction();
					}
					base.updateKey();
					return;
				}
				if (isNewUI)
				{
					for (int i = 0; i < cmd_New_Ui.Length; i++)
					{
						if (cmd_New_Ui[i] != null && cmd_New_Ui[i].isPointerPressInside())
						{
							cmd_New_Ui[i].performAction();
						}
					}
				}
				else
				{
					int num = cmd.Length;
					if (mGraphics.zoomLevel > 1)
					{
					}
					for (int j = 0; j < num; j++)
					{
						if (cmd[j] != null && cmd[j].isPointerPressInside())
						{
							cmd[j].performAction();
						}
					}
				}
			}
			else if (loadScreen)
			{
				if (GameCanvas.keyPressed[8])
				{
					int num2 = ((mGraphics.zoomLevel <= 1) ? 4 : 2);
					GameCanvas.keyPressed[8] = false;
					selected++;
					if (selected > num2)
					{
						selected = 0;
					}
					processInput();
				}
				if (GameCanvas.keyPressed[2])
				{
					int num3 = ((mGraphics.zoomLevel <= 1) ? 4 : 2);
					GameCanvas.keyPressed[2] = false;
					selected--;
					if (selected < 0)
					{
						selected = num3;
					}
					processInput();
				}
			}
			if (!isWait)
			{
				base.updateKey();
			}
		}

	public void perform(int idAction, object p)
		{
			Res.outz("perform " + idAction);
			if (idAction == 1000)
			{
				GameCanvas.connect();
			}
			if (idAction == 1 || idAction == 4)
			{
				Session_ME.gI().close();
				isAutoConect = false;
				countDieConnect = 0;
				loadScreen = true;
				testConnect = 0;
				isGetData = false;
				mSystem.println(">>>>>isGetData: " + isGetData);
				Rms.clearAll();
				switchToMe();
			}
			if (idAction == 2)
			{
				stopDownload = false;
				cmdDownload = new Command(mResources.huy, this, 4, null);
				cmdDownload.x = GameCanvas.w / 2 - mScreen.cmdW / 2;
				cmdDownload.y = GameCanvas.hh + 65;
				right = null;
				if (!GameCanvas.isTouch)
				{
					cmdDownload.x = GameCanvas.w / 2 - mScreen.cmdW / 2;
					cmdDownload.y = GameCanvas.h - mScreen.cmdH - 1;
				}
				center = new Command(string.Empty, this, 4, null);
				if (!isGetData)
				{
					Service.gI().getResource(1, null);
					if (!GameCanvas.isTouch)
					{
						cmdDownload.isFocus = true;
						center = new Command(string.Empty, this, 4, null);
						mSystem.println(">>>>>isGetData: " + isGetData);
					}
					isGetData = true;
				}
			}
			if (idAction == 3)
			{
				Res.outz("toi day");
				Login_New();
			}
			if (idAction == 10100)
			{
				if (GameCanvas.loginScr == null)
				{
					GameCanvas.loginScr = new LoginScr();
				}
				GameCanvas.loginScr.switchToMe();
				if (!Session_ME.gI().isConnected() || !Session_ME.isKeyComplete())
				{
					GameCanvas.connect();
					int waitAttempts = 0;
					while ((!Session_ME.gI().isConnected() || !Session_ME.isKeyComplete()) && waitAttempts < 40)
					{
						System.Threading.Thread.Sleep(30);
						waitAttempts++;
					}
				}
				Service.gI().setClientType();
				Service.gI().login2(string.Empty);
				Res.outz("tao user ao");
				GameCanvas.startWaitDlg();
				LoginScr.serverName = nameServer[ipSelect];
			}
			if (idAction == 5)
			{
				doUpdateServer();
				if (nameServer.Length == 1)
				{
					return;
				}
				MyVector myVector = new MyVector(string.Empty);
				for (int i = 0; i < nameServer.Length; i++)
				{
					myVector.addElement(new Command(nameServer[i], this, 6, null));
				}
				GameCanvas.menu.startAt(myVector, 0);
				if (!GameCanvas.isTouch)
				{
					GameCanvas.menu.menuSelectedItem = ipSelect;
				}
			}
			if (idAction == 6)
			{
				SetIpSelect(GameCanvas.menu.menuSelectedItem, issave: false);
				selectServer();
			}
			if (idAction == 7)
			{
				if (GameCanvas.loginScr == null)
				{
					GameCanvas.loginScr = new LoginScr();
				}
				GameCanvas.loginScr.switchToMe();
			}
			if (idAction == 8)
			{
				bool flag = Rms.loadRMSInt("lowGraphic") == 1;
				MyVector myVector2 = new MyVector("cau hinh");
				myVector2.addElement(new Command(mResources.cauhinhthap, this, 9, null));
				myVector2.addElement(new Command(mResources.cauhinhcao, this, 10, null));
				GameCanvas.menu.startAt(myVector2, 0);
				if (flag)
				{
					GameCanvas.menu.menuSelectedItem = 0;
				}
				else
				{
					GameCanvas.menu.menuSelectedItem = 1;
				}
			}
			if (idAction == 9)
			{
				Rms.saveRMSInt("lowGraphic", 1);
				GameCanvas.startOK(mResources.plsRestartGame, 8885, null);
			}
			if (idAction == 10)
			{
				Rms.saveRMSInt("lowGraphic", 0);
				GameCanvas.startOK(mResources.plsRestartGame, 8885, null);
			}
			if (idAction == 11)
			{
				if (GameCanvas.loginScr == null)
				{
					GameCanvas.loginScr = new LoginScr();
				}
				GameCanvas.loginScr.switchToMe();
				string text = Rms.loadRMSString(Rms.RMS_userAo + ipSelect);
				if (!Session_ME.gI().isConnected() || !Session_ME.isKeyComplete())
				{
					GameCanvas.connect();
					int waitAttempts = 0;
					while ((!Session_ME.gI().isConnected() || !Session_ME.isKeyComplete()) && waitAttempts < 40)
					{
						System.Threading.Thread.Sleep(30);
						waitAttempts++;
					}
				}
				Service.gI().setClientType();
				if (text == null || text.Equals(string.Empty))
				{
					Service.gI().login2(string.Empty);
				}
				else
				{
					GameCanvas.loginScr.isLogin2 = true;
					Service.gI().login(text, string.Empty, GameMidlet.VERSION, 1);
				}
				GameCanvas.startWaitDlg(mResources.PLEASEWAIT);
				Res.outz("tao user ao");
			}
			if (idAction == 12)
			{
				GameMidlet.instance.exit();
			}
			if (idAction == 13 && (!isGetData || loadScreen))
			{
				switch (mSystem.clientType)
				{
				case 1:
					mSystem.callHotlineJava();
					break;
				case 3:
				case 5:
					mSystem.callHotlineIphone();
					break;
				case 6:
					mSystem.callHotlineWindowsPhone();
					break;
				case 4:
					mSystem.callHotlinePC();
					break;
				}
			}
			if (idAction == 14)
			{
				Command cmdYes = new Command(mResources.YES, GameCanvas.serverScreen, 15, null);
				Command cmdNo = new Command(mResources.NO, GameCanvas.serverScreen, 16, null);
				GameCanvas.startYesNoDlg(mResources.deletaDataNote, cmdYes, cmdNo);
			}
			if (idAction == 15)
			{
				Rms.clearAll();
				Rms.saveRMSString("sys_dev_id", Guid.NewGuid().ToString("N"));
				GameCanvas.startOK(mResources.plsRestartGame, 8885, null);
			}
			if (idAction == 16)
			{
				InfoDlg.hide();
				GameCanvas.currentDialog = null;
			}
			if (idAction == 17)
			{
				if (GameCanvas.serverScr == null)
				{
					GameCanvas.serverScr = new ServerScr();
				}
				GameCanvas.serverScr.switchToMe();
			}
			if (idAction == 18)
			{
				GameCanvas.endDlg();
				InfoDlg.hide();
				if (GameCanvas.serverScr == null)
				{
					GameCanvas.serverScr = new ServerScr();
				}
				GameCanvas.serverScr.switchToMe();
			}
			if (idAction == 19)
			{
				if (mSystem.clientType == 1)
				{
					InfoDlg.hide();
					GameCanvas.currentDialog = null;
				}
				else
				{
					countDieConnect = 0;
					testConnect = 0;
					isAutoConect = true;
				}
			}
		}

	public static void SetIpSelect(int index, bool issave)
		{
			if (nameServer != null && nameServer.Length > 0)
			{
				if (index < 0 || index >= nameServer.Length)
				{
					index = (serverPriority >= 0 && serverPriority < nameServer.Length) ? serverPriority : 0;
				}
			}
			else if (index < 0)
			{
				index = 0;
			}
			Debug.LogError(">>>>SetIpSelect: " + index + "  save:" + issave);
			ipSelect = index;
			if (issave)
			{
				Rms.saveRMSInt(RMS_svselect, ipSelect);
				Res.err("2>>>saveRMSInt:  RMS_svselect == " + ipSelect);
			}
		}

}
