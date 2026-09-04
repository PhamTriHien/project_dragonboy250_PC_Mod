using System;
using UnityEngine;

public partial class RadarScr : mScreen, IActionListener
{
	public const sbyte SUBCMD_ALL = 0;

	public const sbyte SUBCMD_USE = 1;

	public const sbyte SUBCMD_LEVEL = 2;

	public const sbyte SUBCMD_AMOUNT = 3;

	public const sbyte SUBCMD_AURA = 4;

	public static RadarScr instance;

	public static bool TYPE_UI;

	public static FrameImage fraImgFocus;

	public static FrameImage fraImgFocusNone;

	public static FrameImage fraEff;

	private static Image imgUI;

	private static Image imgUIText;

	private static Image imgArrow_Left;

	private static Image imgArrow_Right;

	private static Image imgArrow_Down;

	private static Image imgLock;

	private static Image imgUse_0;

	private static Image imgUse;

	private static Image imgBack;

	private static Image imgChange;

	private static Image imgBar_0;

	private static Image imgBar_1;

	private static Image imgPro_0;

	private static Image imgPro_1;

	private static Image[] imgRank;

	public static int xUi;

	public static int yUi;

	public static int wUi;

	public static int hUi;

	public static int xMon;

	public static int yMon;

	public static int xText;

	public static int yText;

	public static int wText;

	public static int cmyText;

	public static int hText;

	public static int yCmd;

	public static int[] xCmd = new int[0];

	public static int[] dxCmd = new int[0];

	private static int[][] xyArrow;

	private static int[][] xyItem;

	private static int[] index = new int[5] { -2, -1, 0, 1, 2 };

	private int dyArrow;

	private int[] dxArrow;

	private int page;

	private int maxpage;

	private int indexFocus;

	public static MyVector list;

	public static MyVector listUse;

	private static int num;

	private static int numMax;

	private Info_RadaScr focus_card;

	private int pxx;

	private int pyy;

	private int xClip;

	private int wClip;

	private int yClip;

	private int hClip;

	public RadarScr()
		{
			TYPE_UI = true;
			Image img = mSystem.loadImage("/radar/17.png");
			Image img2 = mSystem.loadImage("/radar/3.png");
			Image img3 = mSystem.loadImage("/radar/23.png");
			fraImgFocus = new FrameImage(img, 28, 28);
			fraImgFocusNone = new FrameImage(img2, 30, 30);
			fraEff = new FrameImage(img3, 11, 11);
			imgUI = mSystem.loadImage("/radar/0.png");
			imgArrow_Left = mSystem.loadImage("/radar/1.png");
			imgArrow_Right = mSystem.loadImage("/radar/2.png");
			imgUIText = mSystem.loadImage("/radar/17.png");
			imgArrow_Down = mSystem.loadImage("/radar/4.png");
			imgLock = mSystem.loadImage("/radar/5.png");
			imgUse_0 = mSystem.loadImage("/radar/6.png");
			imgRank = new Image[7];
			for (int i = 0; i < 7; i++)
			{
				imgRank[i] = mSystem.loadImage("/radar/" + (i + 7) + ".png");
			}
			imgUse = mSystem.loadImage("/radar/14.png");
			imgBack = mSystem.loadImage("/radar/15.png");
			imgChange = mSystem.loadImage("/radar/16.png");
			imgUIText = mSystem.loadImage("/radar/18.png");
			imgBar_1 = mSystem.loadImage("/radar/19.png");
			imgPro_0 = mSystem.loadImage("/radar/20.png");
			imgPro_1 = mSystem.loadImage("/radar/21.png");
			imgBar_0 = mSystem.loadImage("/radar/22.png");
			wUi = 200;
			hUi = 219;
			xUi = GameCanvas.hw - (wUi + 40) / 2;
			yUi = GameCanvas.hh - hUi / 2;
			xText = xUi + wUi - 81;
			yText = yUi + 29;
			wText = 120;
			hText = 80;
			xyArrow = new int[3][]
			{
				new int[2]
				{
					xUi + 34,
					yUi + hUi - 42
				},
				new int[2]
				{
					xUi + wUi / 2 - imgArrow_Down.getWidth() / 2,
					yUi + hUi / 2 + 33
				},
				new int[2]
				{
					xUi + wUi - 41,
					yUi + hUi - 42
				}
			};
			xyItem = new int[5][]
			{
				new int[2]
				{
					xUi + 25,
					yUi + hUi - 82
				},
				new int[2]
				{
					xUi + 57,
					yUi + hUi - 62
				},
				new int[2]
				{
					xUi + wUi / 2 - 14,
					yUi + hUi - 102
				},
				new int[2]
				{
					xUi + wUi - 57 - 28,
					yUi + hUi - 62
				},
				new int[2]
				{
					xUi + wUi - 25 - 28,
					yUi + hUi - 82
				}
			};
			dxArrow = new int[2];
			dyArrow = 0;
			xMon = xUi + 73;
			yMon = yUi + hUi / 2 + 5;
			yCmd = yUi + hUi - 22;
			xCmd = new int[3]
			{
				xUi + wUi / 2 - 8 - 80,
				xUi + wUi / 2 - 8,
				xUi + wUi / 2 - 8 + 80
			};
			dxCmd = new int[3];
			yClip = yText + 10 + 70;
			hClip = 0;
			list = new MyVector();
			listUse = new MyVector();
			page = 1;
			maxpage = 2;
		}

	public static RadarScr gI()
		{
			if (instance == null)
			{
				instance = new RadarScr();
			}
			return instance;
		}

	public void SetRadarScr(MyVector list, int num, int numMax)
		{
			RadarScr.list = list;
			SetNum(num, numMax);
			page = 1;
			indexFocus = 2;
			listIndex();
			TYPE_UI = true;
			SetListUse();
			if (TYPE_UI)
			{
				maxpage = list.size() / 5 + ((list.size() % 5 > 0) ? 1 : 0);
			}
			else
			{
				maxpage = listUse.size() / 5 + ((listUse.size() % 5 > 0) ? 1 : 0);
			}
		}

	public static void SetNum(int num, int numMax)
		{
			RadarScr.num = num;
			RadarScr.numMax = numMax;
		}

	public static void SetListUse()
		{
			listUse = new MyVector(string.Empty);
			for (int i = 0; i < list.size(); i++)
			{
				Info_RadaScr info_RadaScr = (Info_RadaScr)list.elementAt(i);
				if (info_RadaScr != null && info_RadaScr.isUse == 1)
				{
					listUse.addElement(info_RadaScr);
				}
			}
		}

	public void listIndex()
		{
			MyVector myVector = listUse;
			if (TYPE_UI)
			{
				myVector = list;
			}
			int num = (page - 1) * 5;
			int num2 = num + 5;
			for (int i = num; i < num2; i++)
			{
				if (i >= myVector.size())
				{
					index[i - num] = -1;
					continue;
				}
				Info_RadaScr info_RadaScr = (Info_RadaScr)myVector.elementAt(i);
				if (info_RadaScr != null)
				{
					index[i - num] = info_RadaScr.id;
				}
			}
			cmyText = 0;
			hText = 0;
			SoundMn.gI().radarItem();
		}

	public override void switchToMe()
		{
			GameScr.isPaintOther = true;
			base.switchToMe();
		}

}
