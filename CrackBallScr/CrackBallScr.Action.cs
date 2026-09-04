using System;
using System.Threading;

public partial class CrackBallScr
{
	public void perform(int idAction, object p)
		{
		}

	public override void update()
		{
			try
			{
				cost = price * checkNum();
				checkNumTicket();
				GameScr.gI().update();
				if (timeStart - GameCanvas.timeNow > 0)
				{
					for (int i = 0; i < listBall.Length; i++)
					{
						listBall[i].count += 2;
						if (listBall[i].count >= iDot)
						{
							listBall[i].count = 0;
						}
						listBall[i].x = xDot[listBall[i].count];
						listBall[i].y = yDot[listBall[i].count];
					}
					return;
				}
				if (step == 0)
				{
					step = 1;
				}
				if (step == 1)
				{
					for (int j = 0; j < listBall.Length; j++)
					{
						if (listBall[j].yTo == -999 || listBall[j].isDone)
						{
							continue;
						}
						if (listBall[j].y < listBall[j].yTo)
						{
							if (listBall[j].vy < 0)
							{
								listBall[j].vy = 0;
							}
							if (listBall[j].y + listBall[j].vy > listBall[j].yTo)
							{
								listBall[j].y = listBall[j].yTo;
							}
							else
							{
								listBall[j].y += listBall[j].vy;
							}
							listBall[j].vy++;
						}
						else
						{
							if (listBall[j].vy > 0)
							{
								listBall[j].vy = 0;
							}
							listBall[j].y += listBall[j].vy;
							listBall[j].vy--;
						}
						if (listBall[j].y == listBall[j].yTo)
						{
							Effect me = new Effect(19, listBall[j].x - 5, listBall[j].y + 25, 2, 1, -1);
							EffecMn.addEff(me);
							SoundMn.gI().charFall();
							listBall[j].isDone = true;
							if (!isCanSkill)
							{
								isCanSkill = true;
							}
						}
					}
				}
				if (step == 2)
				{
					for (int k = 0; k < listBall.Length; k++)
					{
						if (listBall[k].isDone)
						{
							continue;
						}
						if (listBall[k].y > -10)
						{
							if (listBall[k].vy > 0)
							{
								listBall[k].vy = 0;
							}
							listBall[k].y += listBall[k].vy;
							listBall[k].vy--;
							listBall[k].x += listBall[k].vx * listBall[k].dir;
							listBall[k].vx -= 3;
						}
						if (listBall[k].y == -10)
						{
							listBall[k].isPaint = false;
						}
					}
					countFr++;
					if (countFr > fr.Length - 1)
					{
						countFr = fr.Length - 1;
						isKame = true;
						SoundMn.gI().newKame();
						if (!isSendSv && timeKame - GameCanvas.timeNow < 0)
						{
							Service.gI().SendCrackBall(2, (byte)(checkTicket() + checkNum()));
							isSendSv = true;
						}
					}
					Char.myCharz().cf = fr[countFr];
					countKame++;
					if (countKame > 5)
					{
						countKame = 0;
					}
					frame = nFrame[countKame];
				}
				if (step == 3)
				{
					if (countKame <= 5)
					{
						countKame = 5;
					}
					countKame++;
					if (countKame > nFrame.Length - 1)
					{
						countKame = nFrame.Length - 1;
						step = 4;
						isKame = false;
						int num = 0;
						for (int l = 0; l < listBall.Length; l++)
						{
							if (listBall[l].isDone && !listBall[l].isSetImg)
							{
								listBall[l].idImg = idItem[num];
								listBall[l].isSetImg = true;
								num++;
							}
						}
					}
					frame = nFrame[countKame];
				}
				if (step == 4)
				{
					for (int m = 0; m < listBall.Length; m++)
					{
						if (listBall[m].isPaint)
						{
							listBall[m].xTo = Char.myCharz().cx;
						}
					}
					step = 5;
				}
				if (step != 5)
				{
					return;
				}
				vp++;
				if (yP < GameCanvas.hh / 3)
				{
					if (yP + vp > GameCanvas.hh / 3)
					{
						yP = GameCanvas.hh / 3;
					}
					else
					{
						yP += vp;
					}
				}
				for (int n = 0; n < listBall.Length; n++)
				{
					if (!listBall[n].isPaint)
					{
						continue;
					}
					if (listBall[n].x < listBall[n].xTo)
					{
						if (listBall[n].vx < 0)
						{
							listBall[n].vx = 0;
						}
						if (listBall[n].x + listBall[n].vx > listBall[n].xTo)
						{
							listBall[n].x = listBall[n].xTo;
						}
						else
						{
							listBall[n].x += listBall[n].vx;
						}
						listBall[n].vx++;
					}
					else
					{
						if (listBall[n].vx > 0)
						{
							listBall[n].vx = 0;
						}
						listBall[n].x += listBall[n].vx;
						listBall[n].vx--;
					}
					if (listBall[n].x == listBall[n].xTo)
					{
						listBall[n].isPaint = false;
					}
				}
			}
			catch (Exception)
			{
			}
		}

	public override void updateKey()
		{
			if (InfoDlg.isLock)
			{
				return;
			}
			if (GameCanvas.isTouch && !ChatTextField.gI().isShow && !GameCanvas.menu.showMenu)
			{
				updateKeyTouchControl();
			}
			if (isAutoCrackBall && !GameCanvas.keyPressed[0])
			{
				GameCanvas.clearKeyHold();
				GameCanvas.clearKeyPressed();
			}
			if (GameCanvas.keyPressed[0])
			{
				doClickSkill(2);
			}
			for (int i = 1; i < 8; i++)
			{
				if (GameCanvas.keyPressed[i])
				{
					GameCanvas.keyPressed[i] = false;
					doClickBall(i - 1);
				}
			}
			if (GameCanvas.keyPressed[12])
			{
				GameCanvas.keyPressed[12] = false;
				doClickSkill(0);
			}
			if (GameCanvas.keyPressed[13])
			{
				GameCanvas.keyPressed[13] = false;
				doClickSkill(1);
			}
			GameCanvas.clearKeyPressed();
		}

	private void updateKeyTouchControl()
		{
			if (step == 1 && GameCanvas.isPointerClick)
			{
				for (int i = 0; i < listBall.Length; i++)
				{
					if (GameCanvas.isPointerHoldIn(listBall[i].x - 20 - GameScr.cmx, listBall[i].y - 10 - GameScr.cmy, 30, 30) && GameCanvas.isPointerClick && GameCanvas.isPointerJustRelease)
					{
						doClickBall(i);
					}
				}
			}
			if (!GameCanvas.isPointerClick)
			{
				return;
			}
			for (int j = 0; j < xSkill.Length; j++)
			{
				if (GameCanvas.isPointerHoldIn(xSkill[j], ySkill, 36, 36) && GameCanvas.isPointerClick && GameCanvas.isPointerJustRelease)
				{
					if (isAutoCrackBall && j != 2)
					{
						break;
					}
					doClickSkill(j);
				}
			}
		}

	private void doClickBall(int index)
		{
			if (!listBall[index].isDone)
			{
				SoundMn.gI().getItem();
				long num = ((typePrice != 0) ? Char.myCharz().checkLuong() : Char.myCharz().xu);
				if (checkTicket() >= numTicket && num < cost + price)
				{
					string s = mResources.not_enough_money_1 + " " + ((typePrice != 0) ? mResources.LUONG : mResources.XU);
					GameScr.info1.addInfo(s, 0);
				}
				else
				{
					indexSelect = index;
					listBall[indexSelect].yTo = yTo + Res.random(-3, 3);
				}
			}
		}

	private void doClickSkill(int index)
		{
			indexSkillSelect = index;
			if (indexSkillSelect == 2)
			{
				isAutoCrackBall = !isAutoCrackBall;
				if (isAutoCrackBall)
				{
					startAutoCrackBall();
				}
				else
				{
					isCallStop = true;
				}
			}
			else if (index == 0)
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
			else
			{
				if (isAutoCrackBall)
				{
					stopAutoCrackBall();
				}
				GameScr.gI().isRongThanXuatHien = false;
				GameScr.gI().switchToMe();
			}
		}

}
