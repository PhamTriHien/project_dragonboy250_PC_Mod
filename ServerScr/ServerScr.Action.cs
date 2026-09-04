using System;

public partial class ServerScr
{
	public override void update()
		{
			GameScr.cmx++;
			if (GameScr.cmx > GameCanvas.w * 3 + 100)
			{
				GameScr.cmx = 100;
			}
			if (!isPaintNewUi)
			{
				for (int i = 0; i < vecServer.size(); i++)
				{
					Command command = (Command)vecServer.elementAt(i);
					if (!GameCanvas.isTouch)
					{
						if (i == mainSelect)
						{
							if (GameCanvas.gameTick % 10 < 4)
							{
								command.isFocus = true;
							}
							else
							{
								command.isFocus = false;
							}
							cmdCheck = new Command(mResources.SELECT, this, command.idAction, null);
							center = cmdCheck;
						}
						else
						{
							command.isFocus = false;
						}
					}
					else if (command != null && command.isPointerPressInside())
					{
						command.performAction();
					}
				}
			}
			UpdTouch_NewUI();
			UpdTouch_NewUI_Popup();
			ServerListScreen.updateDeleteData();
		}

	public override void updateKey()
		{
			base.updateKey();
			int num = mainSelect % numw;
			int num2 = mainSelect / numw;
			if (GameCanvas.keyPressed[4])
			{
				if (num > 0)
				{
					mainSelect--;
				}
				GameCanvas.keyPressed[4] = false;
			}
			else if (GameCanvas.keyPressed[6])
			{
				if (num < numw - 1)
				{
					mainSelect++;
				}
				GameCanvas.keyPressed[6] = false;
			}
			else if (GameCanvas.keyPressed[2])
			{
				if (num2 > 0)
				{
					mainSelect -= numw;
				}
				GameCanvas.keyPressed[2] = false;
			}
			else if (GameCanvas.keyPressed[8])
			{
				if (num2 < numh - 1)
				{
					mainSelect += numw;
				}
				GameCanvas.keyPressed[8] = false;
			}
			if (mainSelect < 0)
			{
				mainSelect = 0;
			}
			if (mainSelect >= vecServer.size())
			{
				mainSelect = vecServer.size() - 1;
			}
			if (GameCanvas.keyPressed[5])
			{
				((Command)vecServer.elementAt(num)).performAction();
				GameCanvas.keyPressed[5] = false;
			}
			GameCanvas.clearKeyPressed();
		}

	public void perform(int idAction, object p)
		{
			Res.outz("idAction >>>>   " + idAction);
			switch (idAction)
			{
			case 999:
				Save_RMS_Area();
				SetNewSelectMenu(select_Area, 0);
				break;
			case 97:
			{
				if (isPaintNewUi)
				{
					break;
				}
				vecServer.removeAllElements();
				for (int i = 0; i < ServerListScreen.nameServer.Length; i++)
				{
					if (ServerListScreen.language[i] != 0)
					{
						vecServer.addElement(new Command(ServerListScreen.nameServer[i], this, 100 + i, null));
					}
				}
				sort();
				break;
			}
			case 98:
			{
				if (isPaintNewUi)
				{
					break;
				}
				vecServer.removeAllElements();
				for (int j = 0; j < ServerListScreen.nameServer.Length; j++)
				{
					if (ServerListScreen.language[j] == 0)
					{
						vecServer.addElement(new Command(ServerListScreen.nameServer[j], this, 100 + j, null));
					}
				}
				sort();
				break;
			}
			case 99:
				Session_ME.gI().clearSendingMessage();
				ServerListScreen.SetIpSelect(mainSelect, issave: false);
				GameCanvas.serverScreen.selectServer();
				GameCanvas.serverScreen.switchToMe();
				break;
			default:
				Session_ME.gI().close();
				ServerListScreen.SetIpSelect(idAction - 100, issave: true);
				ServerListScreen.ConnectIP();
				if (GameCanvas.serverScreen == null)
				{
					GameCanvas.serverScreen = new ServerListScreen();
				}
				GameCanvas.serverScreen.selectServer();
				GameCanvas.serverScreen.switchToMe();
				break;
			}
		}

	public void SetNewSelectMenu(int area, int typeSv)
		{
			isChooseArea = false;
			if (mSystem.clientType != 1)
			{
				isPaintNewUi = true;
			}
			wCheck = 10;
			w = GameCanvas.w / 3 * 2;
			h = GameCanvas.h / 3 * 2;
			x = (GameCanvas.w - w) / 2;
			y = (GameCanvas.h - h) / 2 + 20;
			xName = GameCanvas.w / 2;
			yName = y - 30;
			wsub = w / 3 * 2;
			wPop = w - wsub - 15;
			if (wPop < 80)
			{
				wPop = 80;
				wsub = w - wPop - 15;
			}
			hsub = h - 10 - wCheck;
			xsub = x + w - wsub - 5;
			ysub = y + 5;
			xPop = x + 5;
			yPop = y + 5;
			hPop = 20;
			xinfo = x + 5;
			yinfo = y + strTypeSV.Length * (hPop + 5) + 5;
			winfo = wPop;
			hinfo = h - (5 + strTypeSV.Length * (hPop + 5) + 5) - wCheck;
			yBox = 10;
			wBox = 70;
			hBox = 20;
			GetVecTypeSv((sbyte)area, (sbyte)typeSv);
		}

}
