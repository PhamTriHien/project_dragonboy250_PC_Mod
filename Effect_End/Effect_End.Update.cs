using System;

public partial class Effect_End : Effect2
{
	public static void setSoundSkill_END(int x, int y, int typeEffect)
		{
			try
			{
				int num = -1;
				int num2 = Res.random(3);
				if (num >= 0)
				{
					SoundMn.playSound(x, y, num, SoundMn.volume);
				}
			}
			catch (Exception ex)
			{
				Res.err("ERR setSoundSkill_END: " + ex.ToString());
			}
		}

	public void update()
		{
			try
			{
				f++;
				switch (typeEffect)
				{
				case 0:
				case 1:
				case 2:
					upd_End_String();
					break;
				case 3:
					upd_FireWork();
					break;
				case 16:
				case 17:
					upd_Sub();
					break;
				case 18:
				case 19:
				case 20:
					upd_Pow();
					break;
				case 21:
				case 22:
				case 23:
					upd_Gong();
					break;
				case 24:
					upd_Skill_Kamex10();
					break;
				case 25:
					upd_Skill_Destroy();
					break;
				case 26:
					upd_Skill_MaFuba();
					break;
				case 9:
					upd_LINE_IN();
					break;
				case 10:
				case 11:
					upd_End_Rock();
					break;
				}
			}
			catch (Exception ex)
			{
				Res.err("ERR update: " + ex.ToString());
				removeEff();
			}
		}

	public void updateAngleXP(int fmove)
		{
			if (f < fmove)
			{
				return;
			}
			if (charUse == null || target == null || f >= fRemove)
			{
				f = fRemove;
				return;
			}
			int num = target.x - charUse.cx;
			int num2 = target.y - charUse.cy;
			life++;
			if ((Res.abs(num) < 10 && Res.abs(num2) < 10) || life > fRemove)
			{
				f = fRemove;
				return;
			}
			int num3 = Res.angle(num, num2);
			if (Res.abs(num3 - gocT_Arc) < 90 || num * num + num2 * num2 > 4096)
			{
				if (Res.abs(num3 - gocT_Arc) < 15)
				{
					gocT_Arc = num3;
				}
				else if ((num3 - gocT_Arc >= 0 && num3 - gocT_Arc < 180) || num3 - gocT_Arc < -180)
				{
					gocT_Arc = Res.fixangle(gocT_Arc + 15);
				}
				else
				{
					gocT_Arc = Res.fixangle(gocT_Arc - 15);
				}
			}
			if (f > fRemove * 2 / 3 && va < 8192)
			{
				va += 3096;
			}
			vx1000 = va * Res.cos(gocT_Arc) >> 10;
			vy1000 = va * Res.sin(gocT_Arc) >> 10;
			num += vx1000;
			int num4 = num >> 10;
			x += num4;
			num &= 0x3FF;
			num2 += vy1000;
			int num5 = num2 >> 10;
			y += num5;
			num2 &= 0x3FF;
		}

	public int setFrameAngle(int goc)
		{
			if (goc <= 15 || goc > 345)
			{
				return 12;
			}
			int num = (goc - 15) / 15 + 1;
			if (num > 24)
			{
				num = 24;
			}
			return mpaintone_Arrow[num];
		}

	private void set_End_String(int typeEffect)
		{
			switch (typeEffect)
			{
			case 0:
				fraImgEff = new FrameImage(4);
				break;
			case 1:
				fraImgEff = new FrameImage(5);
				break;
			case 2:
				fraImgEff = new FrameImage(6);
				break;
			}
			fRemove = 100;
			dy_throw = GameCanvas.h / 3 + 10;
			vy = 10;
			y1000 = 0;
			isAddSub = false;
		}

	private void set_FireWork()
		{
			int num = 0;
			num = Res.random(3, 5);
			fRemove = 90;
			for (int i = 0; i < num; i++)
			{
				Point point = new Point();
				point.x = x + Res.random_Am_0(4);
				point.y = y + Res.random_Am_0(5);
				if (typeSub == 0)
				{
					point.fRe = Res.random(10);
					int num2 = 1;
					if (i % 2 == 0)
					{
						num2 = -1;
					}
					point.x = x + Res.random(arrInfoEff[5][0] / 2) * num2;
					point.y = y - Res.random(arrInfoEff[5][1] / 2);
					point.fraImgEff = new FrameImage(7);
				}
				VecEffEnd.addElement(point);
			}
		}

	private void set_Skill_Kamex10()
		{
			w = fra_skill[0].frameWidth;
			h = fra_skill[0].frameHeight;
			vMax = Res.abs(x - target.x);
			nFrame = new byte[6] { 0, 0, 0, 1, 1, 1 };
			isAddSub = false;
			SoundMn.playSound(x, y, SoundMn.KAMEX10_1, SoundMn.volume);
		}

	private void set_Skill_Destroy()
		{
			x = charUse.cx + 20 * charUse.cdir;
			int num = 15;
			fMove = timeRemove / num;
			if (target != null)
			{
				for (int i = 0; i < num; i++)
				{
					Point point = new Point();
					point.fraImgEff = fra_skill[0];
					point.fraImgEff_2 = fra_skill[2];
					point.x = x;
					point.y = y;
					if (target != null)
					{
						point.toX = target.x;
						point.toY = target.y;
						if (range > 0)
						{
							point.toX += Res.random_Am(0, range);
							point.toY += Res.random_Am(0, range);
						}
					}
					vMax = Res.random(9, 12);
					if (i == num - 1)
					{
						point.fraImgEff = fra_skill[1];
						point.fraImgEff_2 = fra_skill[3];
						point.toX = target.x;
						point.toY = target.y;
						vMax = 9;
					}
					point.isPaint = false;
					point.isChange = false;
					point.isRemove = false;
					point.create_Arrow(vMax);
					VecEffEnd.addElement(point);
				}
			}
			else
			{
				removeEff();
			}
		}

	private void set_Skill_MaFuba()
		{
			nFrame = new byte[9] { 0, 0, 0, 1, 1, 1, 2, 2, 2 };
			isAddSub = false;
			fMove = 10;
			x1000 = x;
			y1000 = y + 12;
			dy = 25;
			dy_throw = 19;
			if (typeSub == 1)
			{
				dy_throw = 21;
			}
			else if (typeSub == 2)
			{
				dy_throw = 31;
			}
			h = fra_skill[1].frameHeight + 50 - dy_throw;
			vy = 1;
			vy1000 = 1;
			y = y1000 - h;
			rS = 90;
			vMax = 1;
			angleS = (angleO = 25);
			iDotS = 1;
			if (listObj != null && listObj.Length > 0)
			{
				iDotS = listObj.Length;
			}
			iAngleS = 360 / iDotS;
			xArgS = new int[iDotS];
			yArgS = new int[iDotS];
			xDotS = new int[iDotS];
			yDotS = new int[iDotS];
			GameScr.addEffectEnd(16, 0, typePaint, x1000, y1000, 1, 0, -1, null);
			SoundMn.playSound(x, y, SoundMn.MAFUBA_0, SoundMn.volume);
		}

	private void setDotStar()
		{
			for (int i = 0; i < yArgS.Length; i++)
			{
				if (angleS >= 360)
				{
					angleS -= 360;
				}
				if (angleS < 0)
				{
					angleS = 360 + angleS;
				}
				yArgS[i] = Res.abs(rS * Res.sin(angleS) / 1024);
				xArgS[i] = Res.abs(rS * Res.cos(angleS) / 1024);
				if (angleS < 90)
				{
					xDotS[i] = x + xArgS[i];
					yDotS[i] = y - yArgS[i];
				}
				else if (angleS >= 90 && angleS < 180)
				{
					xDotS[i] = x - xArgS[i];
					yDotS[i] = y - yArgS[i];
				}
				else if (angleS >= 180 && angleS < 270)
				{
					xDotS[i] = x - xArgS[i];
					yDotS[i] = y + yArgS[i];
				}
				else
				{
					xDotS[i] = x + xArgS[i];
					yDotS[i] = y + yArgS[i];
				}
				angleS -= iAngleS;
			}
		}

	private void set_LINE_IN()
		{
			indexColorStar = typeSub;
			x1000 = x * 1000;
			y1000 = y * 1000;
			fRemove = Res.random(4, 6);
			vMax = 5;
			xline = 10;
			yline = 20;
			create_Star_Line_In(vMax, xline, yline, 0);
		}

	private void set_End_Rock()
		{
			fraImgEff = new FrameImage(8);
			fRemove = Res.random(23, 27);
			int num = Res.random(1, 3);
			toY = y - 40;
			for (int i = 0; i < num; i++)
			{
				Point point = new Point();
				point.x = x + Res.random_Am(0, 20);
				point.y = y + Res.random_Am_0(7);
				if (typeEffect == 10)
				{
					point.frame = Res.random(0, fraImgEff.nFrame - 2);
				}
				else if (typeEffect == 11)
				{
					point.frame = Res.random(2, fraImgEff.nFrame);
				}
				else
				{
					point.frame = Res.random(0, fraImgEff.nFrame);
				}
				point.dis = Res.random(2);
				point.vy = -Res.random(1, 4);
				VecEffEnd.addElement(point);
			}
		}

	private void set_Gong()
		{
			if (charUse != null)
			{
				if (typeEffect == 21)
				{
					x = charUse.cx - 3 * charUse.cdir;
					y = charUse.cy;
					SoundMn.playSound(x, y, SoundMn.KAMEX10_0, SoundMn.volume);
				}
				else if (typeEffect == 22)
				{
					x = charUse.cx + 20 * charUse.cdir;
					y = charUse.cy - 4;
					SoundMn.playSound(x, y, SoundMn.DESTROY_2, SoundMn.volume);
				}
				else if (typeEffect == 23)
				{
					x = charUse.cx;
					y = charUse.cy - 50;
					SoundMn.playSound(x, y, SoundMn.MAFUBA_2, SoundMn.volume);
				}
				else
				{
					x = charUse.cx;
					y = charUse.cy;
				}
			}
		}

	private void set_Pow()
		{
			nFrame = null;
			n_frame = 3;
			if (typeEffect == 18)
			{
				if (typeSub == 0)
				{
					nFrame = new byte[9] { 0, 0, 0, 1, 1, 1, 2, 2, 2 };
				}
				else
				{
					nFrame = new byte[12]
					{
						3, 3, 3, 4, 4, 4, 5, 5, 5, 6,
						6, 6
					};
				}
			}
		}

	private void set_Sub()
		{
			if (typeEffect == 17)
			{
				x += ((dir != 0) ? (-fra_skill[0].frameWidth) : 0);
			}
		}

	private void set_()
		{
		}

}
