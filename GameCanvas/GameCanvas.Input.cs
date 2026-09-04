using System;
using Assets.src.e;
using Assets.src.g;
using UnityEngine;

public partial class GameCanvas : IActionListener
{
	public static void closeKeyBoard()
		{
			mGraphics.addYWhenOpenKeyBoard = 0;
			timeOpenKeyBoard = 0;
			Main.closeKeyBoard();
		}

	public void keyPressedz(int keyCode)
		{
			lastTimePress = mSystem.currentTimeMillis();
			if ((keyCode >= 48 && keyCode <= 57) || (keyCode >= 65 && keyCode <= 122) || keyCode == 10 || keyCode == 8 || keyCode == 13 || keyCode == 32 || keyCode == 31)
			{
				keyAsciiPress = keyCode;
			}
			mapKeyPress(keyCode);
		}

	public void mapKeyPress(int keyCode)
		{
			if (currentDialog != null)
			{
				currentDialog.keyPress(keyCode);
				keyAsciiPress = 0;
				return;
			}
			currentScreen.keyPress(keyCode);
			switch (keyCode)
			{
			case 119:
			case 87:
				if (ChatTextField.gI().isShow || currentScreen is LoginScr || currentScreen is CreateCharScr)
				{
					break;
				}
				keyAsciiPress = 0;
				if ((currentScreen is GameScr || currentScreen is CrackBallScr) && Char.myCharz().isAttack)
				{
					clearKeyHold();
					clearKeyPressed();
				}
				else
				{
					keyHold[21] = true;
					keyPressed[21] = true;
				}
				break;
			case 115:
			case 83:
				if (ChatTextField.gI().isShow || currentScreen is LoginScr || currentScreen is CreateCharScr)
				{
					break;
				}
				keyAsciiPress = 0;
				if ((currentScreen is GameScr || currentScreen is CrackBallScr) && Char.myCharz().isAttack)
				{
					clearKeyHold();
					clearKeyPressed();
				}
				else
				{
					keyHold[22] = true;
					keyPressed[22] = true;
				}
				break;
			case 97:
			case 65:
				if (ChatTextField.gI().isShow || currentScreen is LoginScr || currentScreen is CreateCharScr)
				{
					break;
				}
				keyAsciiPress = 0;
				if ((currentScreen is GameScr || currentScreen is CrackBallScr) && Char.myCharz().isAttack)
				{
					clearKeyHold();
					clearKeyPressed();
				}
				else
				{
					keyHold[23] = true;
					keyPressed[23] = true;
				}
				break;
			case 100:
			case 68:
				if (ChatTextField.gI().isShow || currentScreen is LoginScr || currentScreen is CreateCharScr)
				{
					break;
				}
				keyAsciiPress = 0;
				if ((currentScreen is GameScr || currentScreen is CrackBallScr) && Char.myCharz().isAttack)
				{
					clearKeyHold();
					clearKeyPressed();
				}
				else
				{
					keyHold[24] = true;
					keyPressed[24] = true;
				}
				break;
			case -38:
			case -1:
				if ((currentScreen is GameScr || currentScreen is CrackBallScr) && Char.myCharz().isAttack)
				{
					clearKeyHold();
					clearKeyPressed();
				}
				else
				{
					keyHold[21] = true;
					keyPressed[21] = true;
				}
				break;
			case -39:
			case -2:
				if ((currentScreen is GameScr || currentScreen is CrackBallScr) && Char.myCharz().isAttack)
				{
					clearKeyHold();
					clearKeyPressed();
				}
				else
				{
					keyHold[22] = true;
					keyPressed[22] = true;
				}
				break;
			case -3:
				if ((currentScreen is GameScr || currentScreen is CrackBallScr) && Char.myCharz().isAttack)
				{
					clearKeyHold();
					clearKeyPressed();
				}
				else
				{
					keyHold[23] = true;
					keyPressed[23] = true;
				}
				break;
			case -4:
				if ((currentScreen is GameScr || currentScreen is CrackBallScr) && Char.myCharz().isAttack)
				{
					clearKeyHold();
					clearKeyPressed();
				}
				else
				{
					keyHold[24] = true;
					keyPressed[24] = true;
				}
				break;
			case -5:
			case 10:
				if ((currentScreen is GameScr || currentScreen is CrackBallScr) && Char.myCharz().isAttack)
				{
					clearKeyHold();
					clearKeyPressed();
					break;
				}
				keyHold[25] = true;
				keyPressed[25] = true;
				keyHold[15] = true;
				keyPressed[15] = true;
				break;
			case 48:
				keyHold[0] = true;
				keyPressed[0] = true;
				break;
			case 49:
				if (currentScreen == CrackBallScr.instance || (currentScreen == GameScr.instance && isMoveNumberPad && !ChatTextField.gI().isShow))
				{
					keyHold[1] = true;
					keyPressed[1] = true;
				}
				break;
			case 51:
				if (currentScreen == CrackBallScr.instance || (currentScreen == GameScr.instance && isMoveNumberPad && !ChatTextField.gI().isShow))
				{
					keyHold[3] = true;
					keyPressed[3] = true;
				}
				break;
			case 55:
				keyHold[7] = true;
				keyPressed[7] = true;
				break;
			case 57:
				keyHold[9] = true;
				keyPressed[9] = true;
				break;
			case 42:
				keyHold[10] = true;
				keyPressed[10] = true;
				break;
			case 35:
				keyHold[11] = true;
				keyPressed[11] = true;
				break;
			case -21:
			case -6:
				keyHold[12] = true;
				keyPressed[12] = true;
				break;
			case -22:
			case -7:
				keyHold[13] = true;
				keyPressed[13] = true;
				break;
			case 50:
				if (currentScreen == CrackBallScr.instance || (currentScreen == GameScr.instance && isMoveNumberPad && !ChatTextField.gI().isShow))
				{
					keyHold[2] = true;
					keyPressed[2] = true;
				}
				break;
			case 52:
				if (currentScreen == CrackBallScr.instance || (currentScreen == GameScr.instance && isMoveNumberPad && !ChatTextField.gI().isShow))
				{
					keyHold[4] = true;
					keyPressed[4] = true;
				}
				break;
			case 54:
				if (currentScreen == CrackBallScr.instance || (currentScreen == GameScr.instance && isMoveNumberPad && !ChatTextField.gI().isShow))
				{
					keyHold[6] = true;
					keyPressed[6] = true;
				}
				break;
			case 56:
				if (currentScreen == CrackBallScr.instance || (currentScreen == GameScr.instance && isMoveNumberPad && !ChatTextField.gI().isShow))
				{
					keyHold[8] = true;
					keyPressed[8] = true;
				}
				break;
			case 53:
				if (currentScreen == CrackBallScr.instance || (currentScreen == GameScr.instance && isMoveNumberPad && !ChatTextField.gI().isShow))
				{
					keyHold[5] = true;
					keyPressed[5] = true;
				}
				break;
			case -8:
				keyHold[14] = true;
				keyPressed[14] = true;
				break;
			case -26:
				keyHold[16] = true;
				keyPressed[16] = true;
				break;
			case 113:
				keyHold[17] = true;
				keyPressed[17] = true;
				break;
			}
		}

	public void keyReleasedz(int keyCode)
		{
			keyAsciiPress = 0;
			mapKeyRelease(keyCode);
		}

	public void mapKeyRelease(int keyCode)
		{
			switch (keyCode)
			{
			case 119:
			case 87:
			case -38:
			case -1:
				keyHold[21] = false;
				break;
			case 115:
			case 83:
			case -39:
			case -2:
				keyHold[22] = false;
				break;
			case 97:
			case 65:
			case -3:
				keyHold[23] = false;
				break;
			case 100:
			case 68:
			case -4:
				keyHold[24] = false;
				break;
			case -5:
			case 10:
				keyHold[25] = false;
				keyReleased[25] = true;
				keyHold[15] = true;
				keyPressed[15] = true;
				break;
			case 48:
				keyHold[0] = false;
				keyReleased[0] = true;
				break;
			case 49:
				if (currentScreen == CrackBallScr.instance || (currentScreen == GameScr.instance && isMoveNumberPad && !ChatTextField.gI().isShow))
				{
					keyHold[1] = false;
					keyReleased[1] = true;
				}
				break;
			case 51:
				if (currentScreen == CrackBallScr.instance || (currentScreen == GameScr.instance && isMoveNumberPad && !ChatTextField.gI().isShow))
				{
					keyHold[3] = false;
					keyReleased[3] = true;
				}
				break;
			case 55:
				keyHold[7] = false;
				keyReleased[7] = true;
				break;
			case 57:
				keyHold[9] = false;
				keyReleased[9] = true;
				break;
			case 42:
				keyHold[10] = false;
				keyReleased[10] = true;
				break;
			case 35:
				keyHold[11] = false;
				keyReleased[11] = true;
				break;
			case -21:
			case -6:
				keyHold[12] = false;
				keyReleased[12] = true;
				break;
			case -22:
			case -7:
				keyHold[13] = false;
				keyReleased[13] = true;
				break;
			case 50:
				if (currentScreen == CrackBallScr.instance || (currentScreen == GameScr.instance && isMoveNumberPad && !ChatTextField.gI().isShow))
				{
					keyHold[2] = false;
					keyReleased[2] = true;
				}
				break;
			case 52:
				if (currentScreen == CrackBallScr.instance || (currentScreen == GameScr.instance && isMoveNumberPad && !ChatTextField.gI().isShow))
				{
					keyHold[4] = false;
					keyReleased[4] = true;
				}
				break;
			case 54:
				if (currentScreen == CrackBallScr.instance || (currentScreen == GameScr.instance && isMoveNumberPad && !ChatTextField.gI().isShow))
				{
					keyHold[6] = false;
					keyReleased[6] = true;
				}
				break;
			case 56:
				if (currentScreen == CrackBallScr.instance || (currentScreen == GameScr.instance && isMoveNumberPad && !ChatTextField.gI().isShow))
				{
					keyHold[8] = false;
					keyReleased[8] = true;
				}
				break;
			case 53:
				if (currentScreen == CrackBallScr.instance || (currentScreen == GameScr.instance && isMoveNumberPad && !ChatTextField.gI().isShow))
				{
					keyHold[5] = false;
					keyReleased[5] = true;
				}
				break;
			case -8:
				keyHold[14] = false;
				break;
			case -26:
				keyHold[16] = false;
				break;
			case 113:
				keyHold[17] = false;
				keyReleased[17] = true;
				break;
			}
		}

	public void pointerMouse(int x, int y)
		{
			pxMouse = x;
			pyMouse = y;
		}

	public void scrollMouse(int a)
		{
			pXYScrollMouse = a;
			if (panel != null && panel.isShow)
			{
				panel.updateScroolMouse(a);
			}
		}

	public void pointerDragged(int x, int y)
		{
			isPointerSelect = false;
			if (Res.abs(x - pxLast) >= 10 || Res.abs(y - pyLast) >= 10)
			{
				isPointerClick = false;
				isPointerDown = true;
				isPointerMove = true;
			}
			px = x;
			py = y;
			curPos++;
			if (curPos > 3)
			{
				curPos = 0;
			}
			arrPos[curPos] = new Position(x, y);
		}

	public static bool isHoldPress()
		{
			if (mSystem.currentTimeMillis() - lastTimePress >= 800)
			{
				return true;
			}
			return false;
		}

	public void pointerPressed(int x, int y)
		{
			isPointerSelect = false;
			isPointerJustRelease = false;
			isPointerJustDown = true;
			isPointerDown = true;
			isPointerClick = false;
			isPointerMove = false;
			lastTimePress = mSystem.currentTimeMillis();
			pxFirst = x;
			pyFirst = y;
			pxLast = x;
			pyLast = y;
			px = x;
			py = y;
		}

	public void pointerReleased(int x, int y)
		{
			if (!isPointerMove)
			{
				isPointerSelect = true;
			}
			isPointerDown = false;
			isPointerMove = false;
			isPointerJustRelease = true;
			isPointerClick = true;
			mScreen.keyTouch = -1;
			px = x;
			py = y;
		}

	public static bool isPointerHoldIn(int x, int y, int w, int h)
		{
			if (!isPointerDown && !isPointerJustRelease)
			{
				return false;
			}
			if (px >= x && px <= x + w && py >= y && py <= y + h)
			{
				return true;
			}
			return false;
		}

	public static bool isMouseFocus(int x, int y, int w, int h)
		{
			if (pxMouse >= x && pxMouse <= x + w && pyMouse >= y && pyMouse <= y + h)
			{
				return true;
			}
			return false;
		}

	public static void clearKeyPressed()
		{
			for (int i = 0; i < keyPressed.Length; i++)
			{
				keyPressed[i] = false;
			}
			isPointerJustRelease = false;
		}

	public static void clearKeyHold()
		{
			for (int i = 0; i < keyHold.Length; i++)
			{
				keyHold[i] = false;
			}
		}

	public static bool isPointer(int x, int y, int w, int h)
		{
			if (!isPointerDown && !isPointerJustRelease)
			{
				return false;
			}
			if (px >= x && px <= x + w && py >= y && py <= y + h)
			{
				return true;
			}
			return false;
		}

	public static void clearAllPointerEvent()
		{
			isPointerClick = false;
			isPointerDown = false;
			isPointerJustDown = false;
			isPointerJustRelease = false;
			isPointerSelect = false;
			GameScr.gI().lastSingleClick = 0L;
			GameScr.gI().isPointerDowning = false;
		}

}
