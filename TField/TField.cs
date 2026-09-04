using System;
using System.Threading;

public partial class TField : IActionListener
{
	public bool isFocus;

	public int x;

	public int y;

	public int width;

	public int height;

	public bool lockArrow;

	public bool justReturnFromTextBox;

	public bool paintFocus = true;

	public const sbyte KEY_LEFT = 14;

	public const sbyte KEY_RIGHT = 15;

	public const sbyte KEY_CLEAR = 19;

	public static int typeXpeed = 2;

	private static readonly int[] MAX_TIME_TO_CONFIRM_KEY = new int[7] { 30, 14, 11, 9, 6, 4, 2 };

	private static int CARET_HEIGHT = 0;

	private static readonly int CARET_WIDTH = 1;

	private static readonly int CARET_SHOWING_TIME = 5;

	private static readonly int TEXT_GAP_X = 4;

	private static readonly int MAX_SHOW_CARET_COUNER = 10;

	public static readonly int INPUT_TYPE_ANY = 0;

	public static readonly int INPUT_TYPE_NUMERIC = 1;

	public static readonly int INPUT_TYPE_PASSWORD = 2;

	public static readonly int INPUT_ALPHA_NUMBER_ONLY = 3;

	private static string[] print = new string[12]
		{
			" 0", ".,@?!_1\"/$-():*+<=>;%&~#%^&*{}[];'/1", "abc2áàảãạâấầẩẫậăắằẳẵặ2", "def3đéèẻẽẹêếềểễệ3", "ghi4íìỉĩị4", "jkl5", "mno6óòỏõọôốồổỗộơớờởỡợ6", "pqrs7", "tuv8úùủũụưứừửữự8", "wxyz9ýỳỷỹỵ9",
			"*", "#"
		};

	private static string[] printA = new string[12]
		{
			"0", "1", "abc2", "def3", "ghi4", "jkl5", "mno6", "pqrs7", "tuv8", "wxyz9",
			"0", "0"
		};

	private static string[] printBB = new string[17]
		{
			" 0", "er1", "ty2", "ui3", "df4", "gh5", "jk6", "cv7", "bn8", "m9",
			"0", "0", "qw!", "as?", "zx", "op.", "l,"
		};

	private string text = string.Empty;

	private string passwordText = string.Empty;

	private string paintedText = string.Empty;

	private int caretPos;

	private int counter;

	private int maxTextLenght = 500;

	private int offsetX;

	private static int lastKey = -1984;

	private int keyInActiveState;

	private int indexOfActiveChar;

	private int showCaretCounter = MAX_SHOW_CARET_COUNER;

	private int inputType = INPUT_TYPE_ANY;

	public static bool isQwerty = true;

	public static int typingModeAreaWidth;

	public static int mode = 0;

	public static long timeChangeMode;

	public static readonly string[] modeNotify = new string[4] { "abc", "Abc", "ABC", "123" };

	public static readonly int NOKIA = 0;

	public static readonly int MOTO = 1;

	public static readonly int ORTHER = 2;

	public static readonly int BB = 3;

	public static int changeModeKey = 11;

	public static readonly sbyte abc = 0;

	public static readonly sbyte Abc = 1;

	public static readonly sbyte ABC = 2;

	public static readonly sbyte number123 = 3;

	public static TField currentTField;

	public bool isTfield;

	public bool isPaintMouse = true;

	public string name = string.Empty;

	public string title = string.Empty;

	public string strInfo;

	public Command cmdClear;

	public Command cmdDoneAction;

	private mScreen parentScr;

	private int timeDelayKyCode;

	private int holdCount;

	public static int changeDau;

	private int indexDau = -1;

	private int indexTemplate;

	private int indexCong;

	private long timeDau;

	private static string printDau = "aáàảãạâấầẩẫậăắằẳẵặeéèẻẽẹêếềểễệiíìỉĩịoóòỏõọôốồổỗộơớờởỡợuúùủũụưứừửữựyýỳỷỹỵ";

	public static Image imgTf;

	public int timePutKeyClearAll;

	public int timeClearFirt;

	public bool isPaintCarret;

	public bool showSubTextField = true;

	public static TouchScreenKeyboard kb;

	public static int[][] BBKEY = new int[17][]
		{
			new int[2] { 32, 48 },
			new int[2] { 49, 69 },
			new int[2] { 50, 84 },
			new int[2] { 51, 85 },
			new int[2] { 52, 68 },
			new int[2] { 53, 71 },
			new int[2] { 54, 74 },
			new int[2] { 55, 67 },
			new int[2] { 56, 66 },
			new int[2] { 57, 77 },
			new int[2] { 42, 128 },
			new int[2] { 35, 137 },
			new int[2] { 33, 113 },
			new int[2] { 63, 97 },
			new int[3] { 64, 121, 122 },
			new int[2] { 46, 111 },
			new int[2] { 44, 108 }
		};

	public TField(mScreen parentScr)
		{
			text = string.Empty;
			this.parentScr = parentScr;
			init();
		}

	public TField()
		{
			text = string.Empty;
			init();
		}

	public TField(int x, int y, int w, int h)
		{
			text = string.Empty;
			init();
			this.x = x;
			this.y = y;
			width = w;
			height = h;
		}

	public TField(string text, int maxLen, int inputType)
		{
			this.text = text;
			maxTextLenght = maxLen;
			this.inputType = inputType;
			init();
			isTfield = true;
		}

	public static bool setNormal(char ch)
		{
			if ((ch < '0' || ch > '9') && (ch < 'A' || ch > 'Z') && (ch < 'a' || ch > 'z'))
			{
				return false;
			}
			return true;
		}

	public void doChangeToTextBox()
		{
		}

	public void init()
		{
			CARET_HEIGHT = mScreen.ITEM_HEIGHT + 1;
			cmdClear = new Command(mResources.DELETE, this, 1000, null);
			if (Main.isPC)
			{
				typeXpeed = 0;
			}
			if (imgTf == null)
			{
				imgTf = GameCanvas.loadImage("/mainImage/myTexture2dtf.png");
			}
		}

	public void clearAllText()
		{
			text = string.Empty;
			if (kb != null)
			{
				kb.text = string.Empty;
			}
			caretPos = 0;
			setOffset(0);
			setPasswordTest();
		}

	public void clear()
		{
			if (caretPos > 0 && text.Length > 0)
			{
				text = text.Substring(0, caretPos - 1);
				caretPos--;
				setOffset(0);
				setPasswordTest();
				if (kb != null)
				{
					kb.text = text;
				}
			}
		}

	public void clearAll()
		{
			if (caretPos > 0 && text.Length > 0)
			{
				text = text.Substring(0, text.Length - 1);
				caretPos--;
				setOffset();
				setPasswordTest();
				setFocusWithKb(isFocus: true);
				if (kb != null)
				{
					kb.text = string.Empty;
				}
			}
		}

	public void setOffset()
		{
			if (paintedText != null && mFont.tahoma_8b != null)
			{
				if (inputType == INPUT_TYPE_PASSWORD)
				{
					paintedText = passwordText;
				}
				else
				{
					paintedText = text;
				}
				if (offsetX < 0 && mFont.tahoma_8b.getWidth(paintedText) + offsetX < width - TEXT_GAP_X - 13 - typingModeAreaWidth)
				{
					offsetX = width - 10 - typingModeAreaWidth - mFont.tahoma_8b.getWidth(paintedText);
				}
				if (offsetX + mFont.tahoma_8b.getWidth(paintedText.Substring(0, caretPos)) <= 0)
				{
					offsetX = -mFont.tahoma_8b.getWidth(paintedText.Substring(0, caretPos));
					offsetX += 40;
				}
				else if (offsetX + mFont.tahoma_8b.getWidth(paintedText.Substring(0, caretPos)) >= width - 12 - typingModeAreaWidth)
				{
					offsetX = width - 10 - typingModeAreaWidth - mFont.tahoma_8b.getWidth(paintedText.Substring(0, caretPos)) - 2 * TEXT_GAP_X;
				}
				if (offsetX > 0)
				{
					offsetX = 0;
				}
			}
		}

	public void setOffset(int index)
		{
			if (inputType == INPUT_TYPE_PASSWORD)
			{
				paintedText = passwordText;
			}
			else
			{
				paintedText = text;
			}
			int num = mFont.tahoma_8b.getWidth(paintedText.Substring(0, caretPos));
			switch (index)
			{
			case -1:
				if (num + offsetX < 15 && caretPos > 0 && caretPos < paintedText.Length)
				{
					offsetX += mFont.tahoma_8b.getWidth(paintedText.Substring(caretPos, 1));
				}
				break;
			case 1:
				if (num + offsetX > width - 25 && caretPos < paintedText.Length && caretPos > 0)
				{
					offsetX -= mFont.tahoma_8b.getWidth(paintedText.Substring(caretPos - 1, 1));
				}
				break;
			default:
				offsetX = -(num - (width - 12));
				break;
			}
			if (offsetX > 0)
			{
				offsetX = 0;
			}
			else if (offsetX < 0)
			{
				int num2 = mFont.tahoma_8b.getWidth(paintedText) - (width - 12);
				if (offsetX < -num2)
				{
					offsetX = -num2;
				}
			}
		}

	private bool isFocused()
		{
			return isFocus;
		}

	public string subString(string str, int index, int indexTo)
		{
			if (index >= 0 && indexTo > str.Length - 1)
			{
				return str.Substring(index);
			}
			if (index < 0 || index > str.Length - 1 || indexTo < 0 || indexTo > str.Length - 1)
			{
				return string.Empty;
			}
			string text = string.Empty;
			for (int i = index; i < indexTo; i++)
			{
				text += str[i];
			}
			return text;
		}

	private void setPasswordTest()
		{
			if (inputType == INPUT_TYPE_PASSWORD)
			{
				passwordText = string.Empty;
				for (int i = 0; i < text.Length; i++)
				{
					passwordText += "*";
				}
				if (keyInActiveState > 0 && caretPos > 0)
				{
					passwordText = passwordText.Substring(0, caretPos - 1) + text[caretPos - 1] + passwordText.Substring(caretPos, passwordText.Length);
				}
			}
		}

	public void setTextBox()
		{
			if (GameCanvas.isPointerHoldIn(x + width - 20, y, 40, height))
			{
				clearAllText();
				isFocus = true;
			}
			else if (GameCanvas.isPointerHoldIn(x, y, width - 20, height))
			{
				setFocusWithKb(isFocus: true);
			}
			else
			{
				setFocus(isFocus: false);
			}
		}

	public string getText()
		{
			return text;
		}

	public void clearKb()
		{
			if (kb != null)
			{
				kb.text = string.Empty;
			}
		}

	public void setText(string text)
		{
			if (text != null)
			{
				lastKey = -1984;
				keyInActiveState = 0;
				indexOfActiveChar = 0;
				this.text = text;
				paintedText = text;
				if (text == string.Empty)
				{
					TouchScreenKeyboard.Clear();
				}
				setPasswordTest();
				caretPos = text.Length;
				setOffset();
			}
		}

	public void insertText(string text)
		{
			this.text = this.text.Substring(0, caretPos) + text + this.text.Substring(caretPos);
			setPasswordTest();
			caretPos += text.Length;
			setOffset();
		}

	public int getMaxTextLenght()
		{
			return maxTextLenght;
		}

	public void setMaxTextLenght(int maxTextLenght)
		{
			this.maxTextLenght = maxTextLenght;
		}

	public int getIputType()
		{
			return inputType;
		}

	public void setIputType(int iputType)
		{
			inputType = iputType;
			setMaxTextLenght(500);
		}

}
