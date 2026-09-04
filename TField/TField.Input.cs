using System;
using System.Threading;

public partial class TField
{
	public static void setVendorTypeMode(int mode)
		{
			if (mode == MOTO)
			{
				print[0] = "0";
				print[10] = " *";
				print[11] = "#";
				changeModeKey = 35;
			}
			else if (mode == NOKIA)
			{
				print[0] = " 0";
				print[10] = "*";
				print[11] = "#";
				changeModeKey = 35;
			}
			else if (mode == ORTHER)
			{
				print[0] = "0";
				print[10] = "*";
				print[11] = " #";
				changeModeKey = 42;
			}
		}

	public void clearKeyWhenPutText(int keyCode)
		{
			if (keyCode == -8 && timeDelayKyCode <= 0)
			{
				if (timeDelayKyCode <= 0)
				{
					timeDelayKyCode = 1;
				}
				clear();
			}
		}

	private void keyPressedAny(int keyCode)
		{
			string[] array = ((inputType != INPUT_TYPE_PASSWORD && inputType != INPUT_ALPHA_NUMBER_ONLY) ? print : printA);
			if (keyCode == lastKey)
			{
				indexOfActiveChar = (indexOfActiveChar + 1) % array[keyCode - 48].Length;
				char c = array[keyCode - 48][indexOfActiveChar];
				string text = string.Concat(arg1: (mode == 0) ? char.ToLower(c) : ((mode == 1) ? char.ToUpper(c) : ((mode != 2) ? array[keyCode - 48][array[keyCode - 48].Length - 1] : char.ToUpper(c))), arg0: this.text.Substring(0, caretPos - 1));
				if (caretPos < this.text.Length)
				{
					text += this.text.Substring(caretPos, this.text.Length);
				}
				this.text = text;
				keyInActiveState = MAX_TIME_TO_CONFIRM_KEY[typeXpeed];
				setPasswordTest();
			}
			else if (this.text.Length < maxTextLenght)
			{
				if (mode == 1 && lastKey != -1984)
				{
					mode = 0;
				}
				indexOfActiveChar = 0;
				char c2 = array[keyCode - 48][indexOfActiveChar];
				string text2 = string.Concat(arg1: (mode == 0) ? char.ToLower(c2) : ((mode == 1) ? char.ToUpper(c2) : ((mode != 2) ? array[keyCode - 48][array[keyCode - 48].Length - 1] : char.ToUpper(c2))), arg0: this.text.Substring(0, caretPos));
				if (caretPos < this.text.Length)
				{
					text2 += this.text.Substring(caretPos, this.text.Length);
				}
				this.text = text2;
				keyInActiveState = MAX_TIME_TO_CONFIRM_KEY[typeXpeed];
				caretPos++;
				setPasswordTest();
				setOffset();
			}
			lastKey = keyCode;
		}

	private void keyPressedAscii(int keyCode)
		{
			if ((inputType == INPUT_TYPE_PASSWORD || inputType == INPUT_ALPHA_NUMBER_ONLY) && (keyCode < 48 || keyCode > 57) && (keyCode < 65 || keyCode > 90) && (keyCode < 97 || keyCode > 122))
			{
				return;
			}
			if (this.text.Length < maxTextLenght)
			{
				string text = this.text.Substring(0, caretPos) + (char)keyCode;
				if (caretPos < this.text.Length)
				{
					text += this.text.Substring(caretPos, this.text.Length - caretPos);
				}
				this.text = text;
				caretPos++;
				setPasswordTest();
				setOffset(0);
			}
			if (kb != null)
			{
				kb.text = this.text;
			}
		}

	public static void setMode()
		{
			mode++;
			if (mode > 3)
			{
				mode = 0;
			}
			lastKey = changeModeKey;
			timeChangeMode = Environment.TickCount / 1000;
		}

	private void setDau()
		{
			timeDau = Environment.TickCount / 100;
			if (indexDau == -1)
			{
				for (int num = caretPos; num > 0; num--)
				{
					char c = this.text[num - 1];
					for (int i = 0; i < printDau.Length; i++)
					{
						char c2 = printDau[i];
						if (c == c2)
						{
							indexTemplate = i;
							indexCong = 0;
							indexDau = num - 1;
							return;
						}
					}
				}
				indexDau = -1;
			}
			else
			{
				indexCong++;
				if (indexCong >= 6)
				{
					indexCong = 0;
				}
				string text = this.text.Substring(0, indexDau);
				string text2 = this.text.Substring(indexDau + 1);
				string text3 = printDau.Substring(indexTemplate + indexCong, 1);
				this.text = text + text3 + text2;
			}
		}

	public bool keyPressed(int keyCode)
		{
			if (Main.isPC && keyCode == -8)
			{
				clearKeyWhenPutText(-8);
				return true;
			}
			if (keyCode == 8 || keyCode == -8 || keyCode == 204)
			{
				clear();
				return true;
			}
			if (isQwerty && keyCode >= 32)
			{
				keyPressedAscii(keyCode);
				return false;
			}
			if (keyCode == changeDau && inputType == INPUT_TYPE_ANY)
			{
				setDau();
				return false;
			}
			if (keyCode == 42)
			{
				keyCode = 58;
			}
			if (keyCode == 35)
			{
				keyCode = 59;
			}
			if (keyCode >= 48 && keyCode <= 59)
			{
				if (inputType == INPUT_TYPE_ANY || inputType == INPUT_TYPE_PASSWORD || inputType == INPUT_ALPHA_NUMBER_ONLY)
				{
					keyPressedAny(keyCode);
				}
				else if (inputType == INPUT_TYPE_NUMERIC)
				{
					keyPressedAscii(keyCode);
					keyInActiveState = 1;
				}
			}
			else
			{
				indexOfActiveChar = 0;
				lastKey = -1984;
				if (keyCode == 14 && !lockArrow)
				{
					if (caretPos > 0)
					{
						caretPos--;
						setOffset(0);
						showCaretCounter = MAX_SHOW_CARET_COUNER;
						return false;
					}
				}
				else if (keyCode == 15 && !lockArrow)
				{
					if (caretPos < text.Length)
					{
						caretPos++;
						setOffset(0);
						showCaretCounter = MAX_SHOW_CARET_COUNER;
						return false;
					}
				}
				else
				{
					if (keyCode == 19)
					{
						clear();
						return false;
					}
					lastKey = keyCode;
				}
			}
			return true;
		}

	public void update()
		{
			isPaintCarret = true;
			if (Main.isPC)
			{
				if (timeDelayKyCode > 0)
				{
					timeDelayKyCode--;
				}
				if (timeDelayKyCode <= 0)
				{
					timeDelayKyCode = 0;
				}
			}
			if (kb != null && currentTField == this)
			{
				if (kb.text.Length < 40 && isFocus)
				{
					setText(kb.text);
				}
				if (kb.done && cmdDoneAction != null)
				{
					cmdDoneAction.performAction();
				}
			}
			counter++;
			if (keyInActiveState > 0)
			{
				keyInActiveState--;
				if (keyInActiveState == 0)
				{
					indexOfActiveChar = 0;
					if (mode == 1 && lastKey != changeModeKey && isFocus)
					{
						mode = 0;
					}
					lastKey = -1984;
					setPasswordTest();
				}
			}
			if (showCaretCounter > 0)
			{
				showCaretCounter--;
			}
			if (GameCanvas.isPointerJustRelease)
			{
				setTextBox();
			}
			if (indexDau != -1 && Environment.TickCount / 100 - timeDau > 5)
			{
				indexDau = -1;
			}
		}

	public void setFocus(bool isFocus)
		{
			if (this.isFocus != isFocus)
			{
				mode = 0;
			}
			lastKey = -1984;
			timeChangeMode = (int)(DateTime.Now.Ticks / 1000);
			this.isFocus = isFocus;
			if (isFocus)
			{
				currentTField = this;
				if (kb != null)
				{
					kb.text = currentTField.text;
				}
			}
		}

	public void setFocusWithKb(bool isFocus)
		{
			if (this.isFocus != isFocus)
			{
				mode = 0;
			}
			lastKey = -1984;
			timeChangeMode = (int)(DateTime.Now.Ticks / 1000);
			this.isFocus = isFocus;
			if (isFocus)
			{
				currentTField = this;
			}
			else if (currentTField == this)
			{
				currentTField = null;
			}
			if (Thread.CurrentThread.Name == Main.mainThreadName && currentTField != null)
			{
				isFocus = true;
				TouchScreenKeyboard.hideInput = !currentTField.showSubTextField;
				TouchScreenKeyboardType t = TouchScreenKeyboardType.ASCIICapable;
				if (inputType == INPUT_TYPE_NUMERIC)
				{
					t = TouchScreenKeyboardType.NumberPad;
				}
				bool type = false;
				if (inputType == INPUT_TYPE_PASSWORD)
				{
					type = true;
				}
				kb = TouchScreenKeyboard.Open(currentTField.text, t, b1: false, b2: false, type, b3: false, currentTField.name);
				if (kb != null)
				{
					kb.text = currentTField.text;
				}
				Cout.LogWarning("SHOW KEYBOARD FOR " + currentTField.text);
			}
		}

	public void perform(int idAction, object p)
		{
			if (idAction == 1000)
			{
				clear();
			}
		}

}
