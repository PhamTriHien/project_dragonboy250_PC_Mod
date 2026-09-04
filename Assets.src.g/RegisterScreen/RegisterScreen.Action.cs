using System;

namespace Assets.src.g;

public partial class RegisterScreen
{
	protected void doRegister()
		{
			if (tfUser.getText().Equals(string.Empty))
			{
				GameCanvas.startOKDlg(mResources.userBlank);
				return;
			}
			char[] array = tfUser.getText().ToCharArray();
			if (tfNgay.getText().Equals(string.Empty))
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
				GameCanvas.msgdlg.setInfo(mResources.plsCheckAcc + ((num != 1) ? (mResources.email + ": ") : (mResources.phone + ": ")) + tfUser.getText() + "\n" + mResources.password + ": " + tfNgay.getText(), new Command(mResources.ACCEPT, this, 4000, null), null, new Command(mResources.NO, GameCanvas.instance, 8882, null));
			}
			GameCanvas.currentDialog = GameCanvas.msgdlg;
		}

	protected void doRegister(string user)
		{
		}

	public override void update()
		{
			tfUser.update();
			tfSodt.update();
			tfNgay.update();
			tfThang.update();
			tfNam.update();
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
			else if (tfNgay.isFocus)
			{
				tfNgay.keyPressed(keyCode);
			}
			else if (tfThang.isFocus)
			{
				tfThang.keyPressed(keyCode);
			}
			else if (tfNam.isFocus)
			{
				tfNam.keyPressed(keyCode);
			}
			else if (tfDiachi.isFocus)
			{
				tfDiachi.keyPressed(keyCode);
			}
			else if (tfCMND.isFocus)
			{
				tfCMND.keyPressed(keyCode);
			}
			else if (tfNoiCap.isFocus)
			{
				tfNoiCap.keyPressed(keyCode);
			}
			else if (tfSodt.isFocus)
			{
				tfSodt.keyPressed(keyCode);
			}
			else if (tfNgayCap.isFocus)
			{
				tfNgayCap.keyPressed(keyCode);
			}
			base.keyPress(keyCode);
		}

	public override void updateKey()
		{
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
				else if (tfNgay.isFocus)
				{
					right = tfNgay.cmdClear;
				}
				else if (tfThang.isFocus)
				{
					right = tfThang.cmdClear;
				}
				else if (tfNam.isFocus)
				{
					right = tfNam.cmdClear;
				}
				else if (tfDiachi.isFocus)
				{
					right = tfDiachi.cmdClear;
				}
				else if (tfCMND.isFocus)
				{
					right = tfCMND.cmdClear;
				}
				else if (tfNgayCap.isFocus)
				{
					right = tfNgayCap.cmdClear;
				}
				else if (tfNoiCap.isFocus)
				{
					right = tfNoiCap.cmdClear;
				}
				else if (tfSodt.isFocus)
				{
					right = tfSodt.cmdClear;
				}
			}
			if (GameCanvas.keyPressed[21])
			{
				focus--;
				if (focus < 0)
				{
					focus = 8;
				}
				processFocus();
			}
			else if (GameCanvas.keyPressed[22])
			{
				focus++;
				if (focus > 8)
				{
					focus = 0;
				}
				processFocus();
			}
			if (GameCanvas.keyPressed[21] || GameCanvas.keyPressed[22])
			{
				GameCanvas.clearKeyPressed();
				if (!isLogin2 || isRes)
				{
					if (focus == 1)
					{
						tfUser.isFocus = false;
						tfNgay.isFocus = true;
					}
					else if (focus == 0)
					{
						tfUser.isFocus = true;
						tfNgay.isFocus = false;
					}
					else
					{
						tfUser.isFocus = false;
						tfNgay.isFocus = false;
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
			if (GameCanvas.isPointerJustRelease)
			{
				if (GameCanvas.isPointerHoldIn(tfUser.x, tfUser.y, tfUser.width, tfUser.height))
				{
					focus = 0;
					processFocus();
				}
				else if (GameCanvas.isPointerHoldIn(tfNgay.x, tfNgay.y, tfNgay.width, tfNgay.height))
				{
					focus = 1;
					processFocus();
				}
				else if (GameCanvas.isPointerHoldIn(tfThang.x, tfThang.y, tfThang.width, tfThang.height))
				{
					focus = 2;
					processFocus();
				}
				else if (GameCanvas.isPointerHoldIn(tfNam.x, tfNam.y, tfNam.width, tfNam.height))
				{
					focus = 3;
					processFocus();
				}
				else if (GameCanvas.isPointerHoldIn(tfDiachi.x, tfDiachi.y, tfDiachi.width, tfDiachi.height))
				{
					focus = 4;
					processFocus();
				}
				else if (GameCanvas.isPointerHoldIn(tfCMND.x, tfCMND.y, tfCMND.width, tfCMND.height))
				{
					focus = 5;
					processFocus();
				}
				else if (GameCanvas.isPointerHoldIn(tfNgayCap.x, tfNgayCap.y, tfNgayCap.width, tfNgayCap.height))
				{
					focus = 6;
					processFocus();
				}
				else if (GameCanvas.isPointerHoldIn(tfNoiCap.x, tfNoiCap.y, tfNoiCap.width, tfNoiCap.height))
				{
					focus = 7;
					processFocus();
				}
				else if (GameCanvas.isPointerHoldIn(tfSodt.x, tfSodt.y, tfSodt.width, tfSodt.height))
				{
					focus = 8;
					processFocus();
				}
			}
			base.updateKey();
			GameCanvas.clearKeyPressed();
		}

	public void perform(int idAction, object p)
		{
			switch (idAction)
			{
			case 1000:
				try
				{
					GameMidlet.instance.platformRequest((string)p);
				}
				catch (Exception ex)
				{
					ex.StackTrace.ToString();
				}
				GameCanvas.endDlg();
				break;
			case 1001:
				GameCanvas.endDlg();
				isRes = false;
				break;
			case 1004:
				ServerListScreen.doUpdateServer();
				GameCanvas.serverScreen.switchToMe();
				break;
			case 10021:
				actRegisterLeft();
				break;
			case 1003:
				Session_ME.gI().close();
				GameCanvas.serverScreen.switchToMe();
				break;
			case 1005:
				try
				{
					GameMidlet.instance.platformRequest("http://ngocrongonline.com");
					break;
				}
				catch (Exception ex2)
				{
					ex2.StackTrace.ToString();
					break;
				}
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
				if (tfNgay.getText().Equals(string.Empty) || tfThang.getText().Equals(string.Empty) || tfNam.getText().Equals(string.Empty) || tfSodt.getText().Equals(string.Empty) || tfUser.getText().Equals(string.Empty))
				{
					GameCanvas.startOKDlg("Vui lòng điền đầy đủ thông tin");
					break;
				}
				GameCanvas.startOKDlg(mResources.PLEASEWAIT);
				Service.gI().charInfo(tfNgay.getText(), tfThang.getText(), tfNam.getText(), string.Empty, string.Empty, string.Empty, string.Empty, tfSodt.getText(), tfUser.getText());
				break;
			case 4000:
				doRegister(tfUser.getText());
				break;
			}
		}

	public void actRegisterLeft()
		{
			if (isLogin2)
			{
				doLogin();
				return;
			}
			isRes = false;
			tfNgay.isFocus = false;
			tfUser.isFocus = true;
			left = cmdMenu;
		}

	public void actRegister()
		{
			GameCanvas.endDlg();
			GameCanvas.startOKDlg(mResources.regNote);
			isRes = true;
			tfNgay.isFocus = false;
			tfUser.isFocus = true;
		}

}
