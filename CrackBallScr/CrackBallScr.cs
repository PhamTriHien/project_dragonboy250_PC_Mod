using System;
using System.Threading;

public partial class CrackBallScr : mScreen
{
	public static CrackBallScr instance;

	private BallInfo[] listBall;

	private byte step;

	private byte typePrice;

	private int rO;

	private int xO;

	private int yO;

	private int angle;

	private int iAngle;

	private int iDot;

	private int yTo;

	private int indexSelect;

	private int indexSkillSelect;

	private int numTicket;

	private int xP;

	private int yP;

	private int wP;

	private int hP;

	private int price;

	private int cost;

	private int countFr;

	private int countKame;

	private int frame;

	private int vp;

	private int[] xArg;

	private int[] yArg;

	private int[] xDot;

	private int[] yDot;

	private short[] idItem;

	private long timeStart;

	private long timeKame;

	private bool isKame;

	private bool isCanSkill;

	private bool isSendSv;

	private short idTicket;

	private static int ySkill;

	private static int[] xSkill;

	private static FrameImage fraImgKame;

	private static FrameImage fraImgKame_1;

	private static FrameImage fraImgKame_2;

	private static Image imgX;

	private static Image imgReplay;

	public static bool isAutoCrackBall;

	public static bool isCallStop;

	public static bool isContinue;

	private byte[] fr = new byte[21]
		{
			19, 19, 19, 19, 19, 19, 19, 19, 19, 19,
			19, 19, 19, 19, 19, 19, 19, 19, 19, 19,
			20
		};

	private byte[] nFrame = new byte[12]
		{
			0, 0, 0, 1, 1, 1, 2, 2, 2, 3,
			3, 3
		};

	public CrackBallScr()
		{
			xSkill = new int[3];
			xSkill[0] = 16;
			ySkill = GameCanvas.h - 41;
			xSkill[1] = GameCanvas.w - 40;
			xSkill[2] = (xSkill[0] + xSkill[1]) / 2;
			Image img = GameCanvas.loadImage("/e/e_1.png");
			fraImgKame = new FrameImage(img, 30, 30);
			Image img2 = GameCanvas.loadImage("/e/e_0.png");
			fraImgKame_1 = new FrameImage(img2, 68, 65);
			Image img3 = GameCanvas.loadImage("/e/e_2.png");
			fraImgKame_2 = new FrameImage(img3, 66, 70);
			imgReplay = GameCanvas.loadImage("/e/nut2.png");
			imgX = GameCanvas.loadImage("/e/nut3.png");
			wP = 230;
			xP = GameCanvas.hw - wP / 2;
			hP = 40;
			yP = -hP;
		}

	public static CrackBallScr gI()
		{
			if (instance == null)
			{
				instance = new CrackBallScr();
			}
			return instance;
		}

	public void SetCrackBallScr(short[] idImage, byte typePrice, int price, short idTicket)
		{
			if (idImage != null && idImage.Length > 0)
			{
				yTo = Char.myCharz().cy - 10;
				setAuraItem();
				listBall = new BallInfo[idImage.Length];
				for (int i = 0; i < listBall.Length; i++)
				{
					listBall[i] = new BallInfo();
					listBall[i].idImg = idImage[i];
					listBall[i].count = i * 25;
					listBall[i].yTo = -999;
					listBall[i].vx = Res.random(2, 5);
					listBall[i].dir = Res.random(-1, 2);
					listBall[i].SetChar();
				}
				isCanSkill = false;
				isKame = false;
				isSendSv = false;
				timeStart = GameCanvas.timeNow + Res.random(1000, 2000);
				step = 0;
				indexSelect = -1;
				indexSkillSelect = -1;
				this.typePrice = typePrice;
				this.price = price;
				cost = 0;
				Char.myCharz().moveTo(470, 408, 1);
				Char.myCharz().cdir = -1;
				Char.myCharz().statusMe = 1;
				countFr = 0;
				countKame = 0;
				frame = 0;
				vp = 0;
				yP = -hP;
				this.idTicket = idTicket;
				numTicket = 0;
				checkNumTicket();
				switchToMe();
				SoundMn.gI().hoisinh();
			}
		}

	private void setAuraItem()
		{
			rO = GameCanvas.hh / 3 + 10;
			if (rO > 50)
			{
				rO = 50;
			}
			xO = 360;
			GameScr.cmx = GameScr.cmxLim / 2;
			yO = GameScr.cmy + GameCanvas.hh / 3 + 30;
			iDot = 175;
			angle = 0;
			iAngle = 360 / iDot;
			xArg = new int[iDot];
			yArg = new int[iDot];
			xDot = new int[iDot];
			yDot = new int[iDot];
			setDotPosition();
		}

	private void setDotPosition()
		{
			if (GameCanvas.lowGraphic)
			{
				return;
			}
			for (int i = 0; i < yArg.Length; i++)
			{
				yArg[i] = Res.abs(rO * Res.sin(angle) / 1024);
				xArg[i] = Res.abs(rO * Res.cos(angle) / 1024);
				if (angle < 90)
				{
					xDot[i] = xO + xArg[i];
					yDot[i] = yO - yArg[i];
				}
				else if (angle >= 90 && angle < 180)
				{
					xDot[i] = xO - xArg[i];
					yDot[i] = yO - yArg[i];
				}
				else if (angle >= 180 && angle < 270)
				{
					xDot[i] = xO - xArg[i];
					yDot[i] = yO + yArg[i];
				}
				else
				{
					xDot[i] = xO + xArg[i];
					yDot[i] = yO + yArg[i];
				}
				angle += iAngle;
			}
		}

	public void DoneCrackBallScr(short[] idImage)
		{
			step = 3;
			idItem = idImage;
		}

	public override void switchToMe()
		{
			GameScr.isPaintOther = true;
			GameScr.gI().isRongThanXuatHien = true;
			base.switchToMe();
		}

	private byte checkTicket()
		{
			byte b = 0;
			for (int i = 0; i < listBall.Length; i++)
			{
				if (listBall[i].isDone)
				{
					b++;
				}
			}
			if (b > numTicket)
			{
				b = (byte)numTicket;
			}
			return b;
		}

	private byte checkNum()
		{
			byte b = 0;
			for (int i = 0; i < listBall.Length; i++)
			{
				if (listBall[i].isDone)
				{
					b++;
				}
			}
			b -= checkTicket();
			if (b <= 0)
			{
				b = 0;
			}
			return b;
		}

	private void checkNumTicket()
		{
			for (int i = 0; i < Char.myCharz().arrItemBag.Length; i++)
			{
				if (Char.myCharz().arrItemBag[i] != null && Char.myCharz().arrItemBag[i].template.id == idTicket)
				{
					numTicket = Char.myCharz().arrItemBag[i].quantity;
					break;
				}
			}
		}

	private void useSkillCrackBall()
		{
			if (step < 2)
			{
				if (checkTicket() + checkNum() > 0)
				{
					step = 2;
					SoundMn.gI().gong();
					Char.myCharz().setSkillPaint(GameScr.sks[13], 0);
					timeKame = GameCanvas.timeNow + Res.random(2000, 3000);
				}
			}
			else if (yP == GameCanvas.hh / 3)
			{
				Service.gI().SendCrackBall(typePrice, 0);
			}
		}

	public void startAutoCrackBall()
		{
			Thread thread = new Thread(AutoCrackBall);
			thread.Start();
		}

	public void stopAutoCrackBall()
		{
			isAutoCrackBall = false;
			indexSkillSelect = -1;
			isCallStop = false;
			isContinue = false;
		}

	public void AutoCrackBall()
		{
			int num = 0;
			bool flag = false;
			try
			{
				while (isAutoCrackBall && GameCanvas.currentScreen == instance)
				{
					indexSkillSelect = 2;
					while (num < 7 && step != 5)
					{
						doClickBall(num);
						num++;
						Thread.Sleep(300);
					}
					if (num == 7)
					{
						Thread.Sleep(800);
						gI().useSkillCrackBall();
						Thread.Sleep(4000);
						if (isCallStop)
						{
							stopAutoCrackBall();
							Thread.ResetAbort();
							break;
						}
						if (step == 5)
						{
							gI().useSkillCrackBall();
							num = 0;
						}
						Thread.Sleep(1000);
					}
					if (step == 5 && num == 0 && !flag)
					{
						flag = true;
						gI().useSkillCrackBall();
					}
				}
			}
			catch (Exception)
			{
			}
		}

}
