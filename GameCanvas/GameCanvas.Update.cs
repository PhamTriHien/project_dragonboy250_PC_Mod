using System;
using Assets.src.e;
using Assets.src.g;
using UnityEngine;

public partial class GameCanvas : IActionListener
{
	public void update()
		{
			if (currentScreen == _SelectCharScr)
			{
				if (gameTick % 2 == 0 && SmallImage.vt_images_watingDowload.size() > 0)
				{
					Small small = (Small)SmallImage.vt_images_watingDowload.elementAt(0);
					Service.gI().requestIcon(small.id);
					SmallImage.vt_images_watingDowload.removeElementAt(0);
				}
			}
			else if (isRequestMapID == 2 && waitingTimeChangeMap < mSystem.currentTimeMillis() && gameTick % 2 == 0 && currentScreen != null)
			{
				if (currentScreen == GameScr.gI())
				{
					if (Char.isLoadingMap)
					{
						Char.isLoadingMap = false;
					}
					if (ServerListScreen.waitToLogin)
					{
						ServerListScreen.waitToLogin = false;
					}
				}
				if (SmallImage.vt_images_watingDowload.size() > 0)
				{
					Small small2 = (Small)SmallImage.vt_images_watingDowload.elementAt(0);
					Service.gI().requestIcon(small2.id);
					SmallImage.vt_images_watingDowload.removeElementAt(0);
				}
				if (Effect.dowloadEff.size() <= 0)
				{
				}
			}
			if (mSystem.currentTimeMillis() > timefps)
			{
				timefps += 1000L;
				max = fps;
				fps = 0;
			}
			fps++;
			if (messageServer.size() > 0 && thongBaoTest == null)
			{
				startserverThongBao((string)messageServer.elementAt(0));
				messageServer.removeElementAt(0);
			}
			if (gameTick % 5 == 0)
			{
				timeNow = mSystem.currentTimeMillis();
			}
			Res.updateOnScreenDebug();
			try
			{
				if (TouchScreenKeyboard.visible)
				{
					timeOpenKeyBoard++;
					if (timeOpenKeyBoard > ((!Main.isWindowsPhone) ? 10 : 5))
					{
						mGraphics.addYWhenOpenKeyBoard = 94;
					}
				}
				else
				{
					mGraphics.addYWhenOpenKeyBoard = 0;
					timeOpenKeyBoard = 0;
				}
				debugUpdate.removeAllElements();
				long num = mSystem.currentTimeMillis();
				if (num - timeTickEff1 >= 780 && !isEff1)
				{
					timeTickEff1 = num;
					isEff1 = true;
				}
				else
				{
					isEff1 = false;
				}
				if (num - timeTickEff2 >= 7800 && !isEff2)
				{
					timeTickEff2 = num;
					isEff2 = true;
				}
				else
				{
					isEff2 = false;
				}
				if (taskTick > 0)
				{
					taskTick--;
				}
				gameTick++;
				if (gameTick > 10000)
				{
					if (mSystem.currentTimeMillis() - lastTimePress > 20000 && currentScreen == loginScr)
					{
						GameMidlet.instance.exit();
					}
					gameTick = 0;
				}
				if (currentScreen != null)
				{
					if (ChatPopup.serverChatPopUp != null)
					{
						ChatPopup.serverChatPopUp.update();
						ChatPopup.serverChatPopUp.updateKey();
					}
					else if (ChatPopup.currChatPopup != null)
					{
						ChatPopup.currChatPopup.update();
						ChatPopup.currChatPopup.updateKey();
					}
					else if (currentDialog != null)
					{
						debug("B", 0);
						currentDialog.update();
					}
					else if (menu.showMenu)
					{
						debug("C", 0);
						menu.updateMenu();
						debug("D", 0);
						menu.updateMenuKey();
					}
					else if (panel.isShow)
					{
						panel.update();
						if (isPointer(panel.X, panel.Y, panel.W, panel.H))
						{
							isFocusPanel2 = false;
						}
						if (panel2 != null && panel2.isShow)
						{
							panel2.update();
							if (isPointer(panel2.X, panel2.Y, panel2.W, panel2.H))
							{
								isFocusPanel2 = true;
							}
						}
						if (panel2 != null)
						{
							if (isFocusPanel2)
							{
								panel2.updateKey();
							}
							else
							{
								panel.updateKey();
							}
						}
						else
						{
							panel.updateKey();
						}
						if (panel.chatTField != null && panel.chatTField.isShow)
						{
							panel.chatTFUpdateKey();
						}
						else if (panel2 != null && panel2.chatTField != null && panel2.chatTField.isShow)
						{
							panel2.chatTFUpdateKey();
						}
						else if ((isPointer(panel.X, panel.Y, panel.W, panel.H) && panel2 != null) || panel2 == null)
						{
							panel.updateKey();
						}
						else if (panel2 != null && panel2.isShow && isPointer(panel2.X, panel2.Y, panel2.W, panel2.H))
						{
							panel2.updateKey();
						}
						if (isPointer(panel.X + panel.W, panel.Y, w - panel.W * 2, panel.H) && isPointerJustRelease && panel.isDoneCombine)
						{
							panel.hide();
						}
					}
					debug("E", 0);
					if (!isLoading)
					{
						currentScreen.update();
					}
					debug("F", 0);
					if (!panel.isShow && ChatPopup.serverChatPopUp == null)
					{
						currentScreen.updateKey();
					}
					Hint.update();
					SoundMn.gI().update();
				}
				debug("Ix", 0);
				Timer.update();
				debug("Hx", 0);
				InfoDlg.update();
				debug("G", 0);
				if (resetToLoginScr)
				{
					resetToLoginScr = false;
					doResetToLoginScr(loginScr);
				}
				debug("Zzz", 0);
				if ((currentScreen != serverScr || !serverScr.isPaintNewUi) && Controller.isConnectOK)
				{
					if (Controller.isMain)
					{
						ServerListScreen.testConnect = 2;
						Service.gI().setClientType();
						Service.gI().androidPack();
					}
					else
					{
						Service.gI().setClientType2();
						Service.gI().androidPack2();
					}
					Controller.isConnectOK = false;
				}
				if (Controller.isDisconnected)
				{
					if (!Controller.isMain)
					{
						if (currentScreen == serverScreen && !Service.reciveFromMainSession)
						{
							serverScreen.cancel();
						}
						if (currentScreen == loginScr && !Service.reciveFromMainSession)
						{
							onDisconnected();
						}
					}
					else
					{
						onDisconnected();
					}
					Controller.isDisconnected = false;
				}
				if (Controller.isConnectionFail)
				{
					if (!Controller.isMain)
					{
						if (currentScreen == serverScreen && ServerListScreen.isGetData && !Service.reciveFromMainSession)
						{
							ServerListScreen.testConnect = 0;
							serverScreen.cancel();
							Debug.Log("connect fail 1");
						}
						if (currentScreen == loginScr && !Service.reciveFromMainSession)
						{
							onConnectionFail();
							Debug.Log("connect fail 2");
						}
					}
					else
					{
						if (Session_ME.gI().isCompareIPConnect())
						{
							onConnectionFail();
						}
						Debug.Log("connect fail 3");
					}
					Controller.isConnectionFail = false;
				}
				if (Main.isResume)
				{
					Main.isResume = false;
					if (currentDialog != null && currentDialog.left != null && currentDialog.left.actionListener != null)
					{
						currentDialog.left.performAction();
					}
				}
				if (currentScreen != null && currentScreen is GameScr)
				{
					xThongBaoTranslate += dir_ * 2;
					if (xThongBaoTranslate - Panel.imgNew.getWidth() <= 60)
					{
						dir_ = 0;
						tickWaitThongBao++;
						if (tickWaitThongBao > 150)
						{
							tickWaitThongBao = 0;
							thongBaoTest = null;
						}
					}
				}
				if (currentScreen != null && currentScreen.Equals(GameScr.gI()))
				{
					if (GameScr.info1 != null)
					{
						GameScr.info1.update();
					}
					if (GameScr.info2 != null)
					{
						GameScr.info2.update();
					}
				}
				isPointerSelect = false;
			}
			catch (Exception)
			{
			}
		}

	public bool startDust(int dir, int x, int y)
		{
			if (lowGraphic)
			{
				return false;
			}
			int num = ((dir != 1) ? 1 : 0);
			if (dustState[num] != -1)
			{
				return false;
			}
			dustState[num] = 0;
			dustX[num] = x;
			dustY[num] = y;
			return true;
		}

	public void updateWaterSplash()
		{
			if (lowGraphic)
			{
				return;
			}
			for (int i = 0; i < 2; i++)
			{
				if (wsState[i] == -1)
				{
					continue;
				}
				wsY[i]--;
				if (gameTick % 2 == 0)
				{
					wsState[i]++;
					if (wsState[i] > 2)
					{
						wsState[i] = -1;
					}
					else
					{
						wsF[i] = wsState[i];
					}
				}
			}
		}

	public void updateDust()
		{
			if (lowGraphic || ModMenu.graphicsQuality >= 1)
			{
				return;
			}
			for (int i = 0; i < 2; i++)
			{
				if (dustState[i] != -1)
				{
					dustState[i]++;
					if (dustState[i] >= 5)
					{
						dustState[i] = -1;
					}
					if (i == 0)
					{
						dustX[i]--;
					}
					else
					{
						dustX[i]++;
					}
					dustY[i]--;
				}
			}
		}

	public void loadDust()
		{
			if (lowGraphic)
			{
				return;
			}
			if (imgDust == null)
			{
				imgDust = new Image[2][];
				for (int i = 0; i < imgDust.Length; i++)
				{
					imgDust[i] = new Image[5];
				}
				for (int j = 0; j < 2; j++)
				{
					for (int k = 0; k < 5; k++)
					{
						imgDust[j][k] = loadImage("/e/d" + j + k + ".png");
					}
				}
			}
			dustX = new int[2];
			dustY = new int[2];
			dustState = new int[2];
			dustState[0] = (dustState[1] = -1);
		}

}
