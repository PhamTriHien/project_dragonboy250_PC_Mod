using System;

/// <summary>
/// Khung bong bóng hội thoại: Hiển thị lời thoại của nhân vật, NPC và thông điệp chat nổi trên màn hình.
/// </summary>
public partial class ChatPopup : Effect2, IActionListener
{
	public int sayWidth = 100;

	public int delay;

	public int sayRun;

	public string[] says;

	public int cx;

	public int cy;

	public int ch;

	public int cmx;

	public int cmy;

	public int lim;

	public Npc c;

	private bool outSide;

	public static long curr;

	public static long last;

	private int currentLine;

	private string[] lines;

	public Command cmdNextLine;

	public Command cmdMsg1;

	public Command cmdMsg2;

	public static ChatPopup currChatPopup;

	public static ChatPopup serverChatPopUp;

	public static string nextMultiChatPopUp;

	public static Npc nextChar;

	public bool isShopDetail;

	public sbyte starSlot;

	public sbyte maxStarSlot;

	public static Scroll scr;

	public static bool isHavePetNpc;

	public int mH;

	public static int performDelay;

	public int dx;

	public int dy;

	public int second;

	private Point[] saoPoint = new Point[14];

	private int indexStar;

	private int indexStar2;

	public bool[] starCuongHoa = new bool[20];

	public static int numSlot = 7;

	private int nMaxslot_duoi;

	private int nMaxslot_tren;

	private int nslot_duoi;

	private Image imgStar;

	public int strY;

	private int iconID;

	public bool isClip;

	public static int cmyText;

	private int pxx;

	private int pyy;

	public static void clearTransientPopups()
	{
		currChatPopup = null;
		serverChatPopUp = null;
		Char.chatPopup = null;
		Char.isLockKey = false;
		nextMultiChatPopUp = null;
		nextChar = null;
		scr = null;
		isHavePetNpc = false;
		if (GameScr.info1 != null)
		{
			GameScr.info1.isUpdate = true;
		}
		removeFromEffectList(Effect2.vEffect2);
		removeFromEffectList(Effect2.vEffect2Outside);
	}

	private static void removeFromEffectList(MyVector effects)
	{
		if (effects == null)
		{
			return;
		}
		for (int i = effects.size() - 1; i >= 0; i--)
		{
			if (i < effects.size() && effects.elementAt(i) is ChatPopup)
			{
				effects.removeElementAt(i);
			}
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ addNextPopUpMultiLine.
	/// </summary>
	public static void addNextPopUpMultiLine(string strNext, Npc next)
		{
			nextMultiChatPopUp = strNext;
			nextChar = next;
			if (currChatPopup == null)
			{
				addChatPopupMultiLine(nextMultiChatPopUp, 100000, nextChar);
				nextMultiChatPopUp = null;
				nextChar = null;
			}
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ addBigMessage.
	/// </summary>
	public static void addBigMessage(string chat, int howLong, Npc c)
		{
			chat = Res.changeString(chat);
			string[] array = new string[1] { chat };
			if (c.charID != 5 && GameScr.info1.isDone)
			{
				GameScr.info1.isUpdate = false;
			}
			Char.isLockKey = true;
			serverChatPopUp = addChatPopup(array[0], howLong, c);
			serverChatPopUp.strY = 5;
			serverChatPopUp.cx = GameCanvas.w / 2 - serverChatPopUp.sayWidth / 2 - 1;
			serverChatPopUp.cy = GameCanvas.h - 20 - serverChatPopUp.ch;
			serverChatPopUp.currentLine = 0;
			serverChatPopUp.lines = array;
			scr = new Scroll();
			int nItem = serverChatPopUp.says.Length;
			scr.setStyle(nItem, 12, serverChatPopUp.cx, serverChatPopUp.cy - serverChatPopUp.strY + 12, serverChatPopUp.sayWidth + 2, serverChatPopUp.ch - 25, styleUPDOWN: true, 1);
			SoundMn.gI().openDialog();
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ addChatPopupMultiLine.
	/// </summary>
	public static void addChatPopupMultiLine(string chat, int howLong, Npc c)
		{
			chat = Res.changeString(chat);
			string[] array = Res.split(chat, "\n", 0);
			Char.isLockKey = true;
			currChatPopup = addChatPopup(array[0], howLong, c);
			currChatPopup.currentLine = 0;
			currChatPopup.lines = array;
			string caption = mResources.CONTINUE;
			if (array.Length == 1)
			{
				caption = mResources.CLOSE;
			}
			currChatPopup.cmdNextLine = new Command(caption, currChatPopup, 8000, null);
			currChatPopup.cmdNextLine.x = GameCanvas.w / 2 - 35;
			currChatPopup.cmdNextLine.y = GameCanvas.h - 35;
			SoundMn.gI().openDialog();
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ addChatPopupWithIcon.
	/// </summary>
	public static ChatPopup addChatPopupWithIcon(string chat, int howLong, Npc c, int idIcon)
		{
			chat = Res.changeString(chat);
			performDelay = 10;
			ChatPopup chatPopup = new ChatPopup();
			chatPopup.sayWidth = GameCanvas.w - 30 - (GameCanvas.menu.showMenu ? GameCanvas.menu.menuX : 0);
			if (chatPopup.sayWidth > 320)
			{
				chatPopup.sayWidth = 320;
			}
			if (chat.Length < 10)
			{
				chatPopup.sayWidth = 64;
			}
			if (GameCanvas.w == 128)
			{
				chatPopup.sayWidth = 128;
			}
			chatPopup.says = mFont.tahoma_7_red.splitFontArray(chat, chatPopup.sayWidth - 10);
			chatPopup.delay = howLong;
			chatPopup.c = c;
			chatPopup.iconID = idIcon;
			Char.chatPopup = chatPopup;
			chatPopup.ch = 15 - chatPopup.sayRun + chatPopup.says.Length * 12 + 10;
			if (chatPopup.ch > GameCanvas.h - 80)
			{
				chatPopup.ch = GameCanvas.h - 80;
			}
			chatPopup.mH = 10;
			if (GameCanvas.menu.showMenu)
			{
				chatPopup.mH = 0;
			}
			Effect2.vEffect2.addElement(chatPopup);
			isHavePetNpc = false;
			if (c != null && c.charID == 5)
			{
				isHavePetNpc = true;
				GameScr.info1.addInfo(string.Empty, 1);
			}
			curr = (last = mSystem.currentTimeMillis());
			chatPopup.ch += 15;
			return chatPopup;
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ addChatPopup.
	/// </summary>
	public static ChatPopup addChatPopup(string chat, int howLong, Npc c)
		{
			chat = Res.changeString(chat);
			ModBossNotice.ProcessServerBossNotice(chat);
			ModAutoPet.OnCharChatMap(c, chat);
			performDelay = 10;
			ChatPopup chatPopup = new ChatPopup();
			chatPopup.sayWidth = GameCanvas.w - 30 - (GameCanvas.menu.showMenu ? GameCanvas.menu.menuX : 0);
			if (chatPopup.sayWidth > 320)
			{
				chatPopup.sayWidth = 320;
			}
			if (chat.Length < 10)
			{
				chatPopup.sayWidth = 64;
			}
			if (GameCanvas.w == 128)
			{
				chatPopup.sayWidth = 128;
			}
			chatPopup.says = mFont.tahoma_7_red.splitFontArray(chat, chatPopup.sayWidth - 10);
			chatPopup.delay = howLong;
			chatPopup.c = c;
			Char.chatPopup = chatPopup;
			chatPopup.ch = 15 - chatPopup.sayRun + chatPopup.says.Length * 12 + 10;
			if (chatPopup.ch > GameCanvas.h - 80)
			{
				chatPopup.ch = GameCanvas.h - 80;
			}
			chatPopup.mH = 10;
			if (GameCanvas.menu.showMenu)
			{
				chatPopup.mH = 0;
			}
			Effect2.vEffect2.addElement(chatPopup);
			isHavePetNpc = false;
			if (c != null && c.charID == 5)
			{
				isHavePetNpc = true;
				GameScr.info1.addInfo(string.Empty, 1);
			}
			curr = (last = mSystem.currentTimeMillis());
			return chatPopup;
		}

}
