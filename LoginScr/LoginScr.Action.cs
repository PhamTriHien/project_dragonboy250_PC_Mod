using System;
using UnityEngine;

public partial class LoginScr : mScreen, IActionListener
{
	public void updateTfWhenOpenKb()
		{
		}

	public override void update()
		{
			if (timeLogin > 0)
			{
				timeLogin = 0;
				GameCanvas.loginScr.doLogin();
			}
			if (isLogin2 && !isRes)
			{
				tfUser.name = ((mResources.language != 2) ? (mResources.phone + "/") : string.Empty) + mResources.email;
				tfPass.name = mResources.password;
				tfUser.isPaintCarret = false;
				tfPass.isPaintCarret = false;
				tfUser.update();
				tfPass.update();
			}
			else
			{
				tfUser.name = ((mResources.language != 2) ? (mResources.phone + "/") : string.Empty) + mResources.email;
				tfPass.name = mResources.password;
				tfUser.update();
				tfPass.update();
			}
			if (TouchScreenKeyboard.visible)
			{
				mGraphics.addYWhenOpenKeyBoard = 50;
			}
			for (int i = 0; i < Effect2.vEffect2.size(); i++)
			{
				Effect2 effect = (Effect2)Effect2.vEffect2.elementAt(i);
				effect.update();
			}
			if (isUpdateAll && !isUpdateData && !isUpdateItem && !isUpdateMap && !isUpdateSkill)
			{
				isUpdateAll = false;
				mSystem.gcc();
				Service.gI().finishUpdate();
			}
			GameScr.cmx++;
			if (GameScr.cmx > GameCanvas.w * 3 + 100)
			{
				GameScr.cmx = 100;
			}
			if (ChatPopup.currChatPopup != null)
			{
				return;
			}
			GameCanvas.debug("LGU1", 0);
			GameCanvas.debug("LGU2", 0);
			GameCanvas.debug("LGU3", 0);
			updateLogo();
			GameCanvas.debug("LGU4", 0);
			GameCanvas.debug("LGU5", 0);
			if (g >= 0)
			{
				ylogo += dir * g;
				g += dir * v;
				if (g <= 0)
				{
					dir *= -1;
				}
				if (ylogo > 0)
				{
					dir *= -1;
					g -= 2 * v;
				}
			}
			GameCanvas.debug("LGU6", 0);
			if (tipid >= 0 && GameCanvas.gameTick % 100 == 0)
			{
				doChangeTip();
			}
			if (isLogin2 && !isRes)
			{
				tfUser.isPaintCarret = false;
				tfPass.isPaintCarret = false;
				tfUser.update();
				tfPass.update();
			}
			else
			{
				tfUser.name = ((mResources.language != 2) ? (mResources.phone + "/") : string.Empty) + mResources.email;
				tfPass.name = mResources.password;
				tfUser.update();
				tfPass.update();
			}
			if (GameCanvas.isTouch)
			{
				if (isRes)
				{
					center = cmdRes;
					left = cmdBackFromRegister;
				}
				else
				{
					center = cmdOK;
					left = cmdFogetPass;
				}
				if (cmdBack != null && cmdBack.isPointerPressInside())
				{
					cmdBack.performAction();
				}
			}
			else if (isRes)
			{
				center = cmdRes;
				left = cmdBackFromRegister;
			}
			else
			{
				center = cmdOK;
				left = cmdFogetPass;
			}
			if (!Main.isPC && !TouchScreenKeyboard.visible && !Main.isMiniApp && !Main.isWindowsPhone)
			{
				string text = tfUser.getText().ToLower().Trim();
				string text2 = tfPass.getText().ToLower().Trim();
				if (!text.Equals(string.Empty) && !text2.Equals(string.Empty))
				{
					doLogin();
				}
				Main.isMiniApp = true;
			}
			updateTfWhenOpenKb();
		}

	public void updateLogo()
		{
			if (defYL != yL)
			{
				yL += defYL - yL >> 1;
			}
		}

	public override void keyPress(int keyCode)
		{
			if (tfUser.isFocus)
			{
				tfUser.keyPressed(keyCode);
			}
			else if (tfPass.isFocus)
			{
				tfPass.keyPressed(keyCode);
			}
			base.keyPress(keyCode);
		}

	public override void updateKey()
		{
			if (GameCanvas.isTouch)
			{
				if (cmdCallHotline != null && cmdCallHotline.isPointerPressInside())
				{
					cmdCallHotline.performAction();
				}
			}
			else if (mSystem.clientType == 1 && GameCanvas.keyPressed[13])
			{
				GameCanvas.keyPressed[13] = false;
				cmdCallHotline.performAction();
			}
			if (isContinueToLogin)
			{
				return;
			}
			if (!GameCanvas.isTouch)
			{
				if (tfUser.isFocus)
				{
					right = tfUser.cmdClear;
				}
				else
				{
					right = tfPass.cmdClear;
				}
			}
			if (GameCanvas.keyPressed[(!Main.isPC) ? 2 : 21])
			{
				focus--;
				if (focus < 0)
				{
					focus = 1;
				}
			}
			else if (GameCanvas.keyPressed[(!Main.isPC) ? 8 : 22] || GameCanvas.keyPressed[16])
			{
				focus++;
				if (focus > 1)
				{
					focus = 0;
				}
			}
			if (GameCanvas.keyPressed[(!Main.isPC) ? 2 : 21] || GameCanvas.keyPressed[(!Main.isPC) ? 8 : 22] || GameCanvas.keyPressed[16])
			{
				GameCanvas.clearKeyPressed();
				if (!isLogin2 || isRes)
				{
					if (focus == 1)
					{
						tfUser.isFocus = false;
						tfPass.isFocus = true;
					}
					else if (focus == 0)
					{
						tfUser.isFocus = true;
						tfPass.isFocus = false;
					}
					else
					{
						tfUser.isFocus = false;
						tfPass.isFocus = false;
					}
				}
			}
			if (GameCanvas.isTouch)
			{
				if (isRes)
				{
					center = cmdRes;
					left = cmdBackFromRegister;
				}
				else
				{
					center = cmdOK;
					left = cmdFogetPass;
				}
			}
			else if (isRes)
			{
				center = cmdRes;
				left = cmdBackFromRegister;
			}
			else
			{
				center = cmdOK;
				left = cmdFogetPass;
			}
			if (GameCanvas.isPointerJustRelease && (!isLogin2 || isRes))
			{
				if (GameCanvas.isPointerHoldIn(tfUser.x, tfUser.y, tfUser.width, tfUser.height))
				{
					focus = 0;
				}
				else if (GameCanvas.isPointerHoldIn(tfPass.x, tfPass.y, tfPass.width, tfPass.height))
				{
					focus = 1;
				}
			}
			if (Main.isPC && GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25] && right != null)
			{
				right.performAction();
			}
			base.updateKey();
			GameCanvas.clearKeyPressed();
		}

	public void perform(int idAction, object p)
		{
			Debug.LogError(">>>>Loginscr perform: " + idAction);
			switch (idAction)
			{
			case 101:
				GameCanvas.serverScreen.switchToMe();
				break;
			case 13:
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
				case 2:
					break;
				}
				break;
			case 1000:
				try
				{
					GameMidlet.instance.platformRequest((string)p);
				}
				catch (Exception)
				{
				}
				GameCanvas.endDlg();
				break;
			case 1001:
				GameCanvas.endDlg();
				isRes = false;
				break;
			case 1002:
			{
				GameCanvas.startWaitDlg();
				string text = Rms.loadRMSString(Rms.RMS_userAo + ServerListScreen.ipSelect);
				if (text == null || text.Equals(string.Empty))
				{
					Service.gI().login2(string.Empty);
					break;
				}
				GameCanvas.loginScr.isLogin2 = true;
				GameCanvas.connect();
				Service.gI().setClientType();
				Service.gI().login(text, string.Empty, GameMidlet.VERSION, 1);
				break;
			}
			case 1004:
				ServerListScreen.doUpdateServer();
				GameCanvas.serverScreen.switchToMe();
				break;
			case 10021:
				actRegisterLeft();
				break;
			case 1003:
				GameCanvas.startOKDlg(mResources.goToWebForPassword);
				break;
			case 1005:
				try
				{
					GameMidlet.instance.platformRequest("http://ngocrongonline.com");
					break;
				}
				catch (Exception)
				{
					break;
				}
			case 10041:
				Rms.saveRMSInt("lowGraphic", 0);
				GameCanvas.startOK(mResources.plsRestartGame, 8885, null);
				break;
			case 10042:
				Rms.saveRMSInt("lowGraphic", 1);
				GameCanvas.startOK(mResources.plsRestartGame, 8885, null);
				break;
			case 2001:
				if (isCheck)
				{
					isCheck = false;
				}
				else
				{
					isCheck = true;
				}
				break;
			case 2002:
				doRegister();
				break;
			case 2003:
				doMenu();
				break;
			case 2004:
				actRegister();
				break;
			case 2008:
				Rms.saveRMSString(Rms.RMS_acc, tfUser.getText().Trim());
				Rms.saveRMSString(Rms.RMS_pass, tfPass.getText().Trim());
				if (ServerListScreen.isNewUI)
				{
					Controller.isEXTRA_LINK = false;
					GameCanvas.serverScreen.Login_New();
				}
				else if (ServerListScreen.loadScreen)
				{
					GameCanvas.serverScreen.switchToMe();
				}
				else
				{
					GameCanvas.serverScreen.show2();
				}
				break;
			case 4000:
				doRegister(tfUser.getText());
				break;
			}
		}

}
