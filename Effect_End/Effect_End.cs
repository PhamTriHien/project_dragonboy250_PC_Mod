using System;

public partial class Effect_End
{
	public const sbyte Lvlpaint_All = -1;

	public const sbyte Lvlpaint_Front = 0;

	public const sbyte Lvlpaint_Mid = 1;

	public const sbyte Lvlpaint_Mid_2 = 2;

	public const sbyte Lvlpaint_Behind = 3;

	public const short End_String_Lose = 0;

	public const short End_String_Win = 1;

	public const short End_String_Draw = 2;

	public const short End_FireWork = 3;

	public const short End_line_in = 9;

	public const short End_e8_rock = 10;

	public const short End_e8_ice = 11;

	public const short End_SUB_MaFuBa = 16;

	public const short End_SUB_Destroy = 17;

	public const short End_POW_Kamex10 = 18;

	public const short End_POW_Destroy = 19;

	public const short End_POW_MaFuBa = 20;

	public const short End_GONG_Kamex10 = 21;

	public const short End_GONG_Destroy = 22;

	public const short End_GONG_MaFuBa = 23;

	public const short End_Skill_Kamex10 = 24;

	public const short End_Skill_Destroy = 25;

	public const short End_Skill_MaFuBa = 26;

	private MyVector VecEffEnd = new MyVector("EffectEnd VecEffEnd");

	public FrameImage fraImgEff;

	public byte[] nFrame = new byte[10];

	public byte[] nFrame_2 = new byte[10];

	public int typePaint;

	public int typeEffect;

	public int typeSub;

	public int range;

	public short idEndeff;

	public int fRemove;

	public int fMove;

	public int n_frame;

	public int x;

	public int y;

	public int w;

	public int h;

	public int dir;

	public int dir_nguoc;

	public int levelPaint;

	public int f;

	public int frame;

	public int fSpeed;

	public int vx;

	public int vy;

	public int x1000;

	public int y1000;

	public int vx1000;

	public int vy1000;

	public int dy_throw;

	public int vMax;

	public int toX;

	public int toY;

	public int stt;

	public int dx;

	public int dy;

	public short timeRemove;

	public long time;

	public bool isRemove;

	public bool isAddSub;

	public Char charUse;

	public Point[] listObj;

	public Point target;

	public static short[][] arrInfoEff = new short[29][]
			{
				new short[3] { 68, 264, 4 },
				new short[3] { 30, 120, 4 },
				new short[3] { 66, 280, 4 },
				new short[3] { 0, 0, 1 },
				new short[3] { 111, 68, 2 },
				new short[3] { 90, 68, 2 },
				new short[3] { 125, 68, 2 },
				new short[3] { 47, 282, 6 },
				new short[3] { 10, 40, 4 },
				new short[3] { 92, 525, 7 },
				new short[3] { 62, 372, 6 },
				new short[3] { 80, 352, 4 },
				new short[3] { 80, 352, 4 },
				new short[3] { 80, 352, 4 },
				new short[3] { 72, 240, 3 },
				new short[3] { 20, 42, 3 },
				new short[3] { 65, 160, 4 },
				new short[3] { 50, 300, 6 },
				new short[3] { 84, 168, 2 },
				new short[3] { 90, 540, 6 },
				new short[3] { 180, 900, 6 },
				new short[3] { 62, 186, 3 },
				new short[3] { 34, 80, 4 },
				new short[3] { 140, 560, 4 },
				new short[3] { 64, 600, 6 },
				new short[3] { 36, 200, 5 },
				new short[3] { 35, 200, 5 },
				new short[3] { 50, 250, 5 },
				new short[3] { 50, 240, 6 }
			};

	public int life;

	public int goc_Arc;

	public int va;

	public int gocT_Arc;

	public byte[] mpaintone_Arrow = new byte[24]
			{
				12, 11, 10, 9, 8, 7, 6, 5, 4, 3,
				2, 1, 0, 23, 22, 21, 20, 19, 18, 17,
				16, 15, 14, 13
			};

	public byte[] mImageArrow = new byte[24]
			{
				0, 0, 2, 1, 1, 2, 0, 0, 2, 1,
				1, 2, 0, 0, 2, 1, 1, 2, 0, 0,
				2, 1, 1, 2
			};

	public byte[] mXoayArrow = new byte[24]
			{
				2, 2, 3, 3, 3, 4, 5, 5, 5, 5,
				5, 1, 0, 0, 0, 0, 0, 7, 6, 6,
				6, 6, 6, 2
			};

	private int rS;

	private int angleS;

	private int angleO;

	private int iAngleS;

	private int iDotS;

	private int[] xArgS;

	private int[] yArgS;

	private int[] xDotS;

	private int[] yDotS;

	public static int[][] colorStar = new int[3][]
			{
				new int[3] { 16310304, 16298056, 16777215 },
				new int[3] { 7045120, 12643960, 16777215 },
				new int[3] { 2407423, 11987199, 16777215 }
			};

	private int[] colorpaint;

	private int indexColorStar;

	private int xline;

	private int yline;

	private FrameImage[] fra_skill;

	public Effect_End(int type, int typeSub, int x, int y, int levelPaint, int dir, short timeRemove, Point[] listObj)
			{
				f = 0;
				stt = 0;
				typeEffect = type;
				this.typeSub = typeSub;
				this.x = x;
				this.y = y;
				this.levelPaint = levelPaint;
				this.dir = dir;
				dir_nguoc = ((dir == -1) ? 2 : 0);
				time = mSystem.currentTimeMillis();
				this.timeRemove = timeRemove;
				isRemove = (isAddSub = false);
				n_frame = 4;
				if (listObj != null)
				{
					this.listObj = new Point[listObj.Length];
					for (int i = 0; i < this.listObj.Length; i++)
					{
						this.listObj[i] = listObj[i];
					}
				}
				get_Img_Skill();
				create_Effect();
			}

	public Effect_End(int type, int typeSub, int typePaint, Char charUse, Point target, int levelPaint, short timeRemove, short range)
			{
				f = 0;
				stt = 0;
				typeEffect = type;
				this.typeSub = typeSub;
				this.typePaint = typePaint;
				this.charUse = charUse;
				if (charUse.containsCaiTrang(1265))
				{
					if (typeEffect == 21 || typeEffect == 22 || typeEffect == 23)
					{
						this.charUse.cx += 10 * this.charUse.cdir;
					}
					else if (typeEffect == 18 || typeEffect == 19 || typeEffect == 20)
					{
						this.charUse.cx += -15 * this.charUse.cdir;
					}
					else
					{
						this.charUse.cx += 15 * this.charUse.cdir;
					}
				}
				x = this.charUse.cx;
				y = this.charUse.cy;
				dir = this.charUse.cdir;
				dir_nguoc = ((dir == -1) ? 2 : 0);
				this.target = target;
				this.levelPaint = levelPaint;
				time = mSystem.currentTimeMillis();
				this.timeRemove = timeRemove;
				this.range = range;
				isRemove = (isAddSub = false);
				n_frame = 4;
				get_Img_Skill();
				create_Effect();
			}

	public Effect_End(int type, int typeSub, int typePaint, int x, int y, int levelPaint, int dir, short timeRemove, Point[] listObj)
			{
				f = 0;
				stt = 0;
				typeEffect = type;
				this.typeSub = typeSub;
				this.typePaint = typePaint;
				this.x = x;
				this.y = y;
				this.levelPaint = levelPaint;
				this.dir = dir;
				dir_nguoc = ((dir == -1) ? 2 : 0);
				time = mSystem.currentTimeMillis();
				this.timeRemove = timeRemove;
				isRemove = (isAddSub = false);
				n_frame = 4;
				if (listObj != null)
				{
					this.listObj = new Point[listObj.Length];
					for (int i = 0; i < this.listObj.Length; i++)
					{
						this.listObj[i] = listObj[i];
					}
				}
				get_Img_Skill();
				create_Effect();
			}

	public static Image getImage(int id)
			{
				if (id < 0)
				{
					return null;
				}
				string path = "/e/e_" + id + ".png";
				Image result = null;
				try
				{
					result = mSystem.loadImage(path);
				}
				catch (Exception)
				{
				}
				return result;
			}

	public void create_Effect()
			{
				try
				{
					setSoundSkill_END(x, y, typeEffect);
					switch (typeEffect)
					{
					case 0:
					case 1:
					case 2:
						set_End_String(typeEffect);
						break;
					case 3:
						set_FireWork();
						break;
					case 16:
					case 17:
						set_Sub();
						break;
					case 18:
					case 19:
					case 20:
						set_Pow();
						break;
					case 21:
					case 22:
					case 23:
						set_Gong();
						break;
					case 24:
						set_Skill_Kamex10();
						break;
					case 25:
						set_Skill_Destroy();
						break;
					case 26:
						set_Skill_MaFuba();
						break;
					case 9:
						set_LINE_IN();
						break;
					case 10:
					case 11:
						set_End_Rock();
						break;
					}
				}
				catch (Exception ex)
				{
					Res.err("ERR create_Effect: " + ex.ToString());
					removeEff();
				}
			}

	public void removeEff()
			{
				isRemove = true;
			}

	public void createDanFocus(bool isRandom, Char obj)
			{
				if (isRandom)
				{
					switch (Res.random(4))
					{
					case 0:
						gocT_Arc = 90;
						break;
					case 1:
						gocT_Arc = 270;
						break;
					case 2:
						gocT_Arc = 180;
						break;
					case 3:
						gocT_Arc = 0;
						break;
					}
				}
				else if (obj.cdir == 1)
				{
					gocT_Arc = 0;
				}
				else
				{
					gocT_Arc = 180;
				}
				va = (short)(256 * vMax);
				vx = 0;
				vy = 0;
				life = 0;
				vx1000 = va * Res.cos(gocT_Arc) >> 10;
				vy1000 = va * Res.sin(gocT_Arc) >> 10;
			}

	public void create_Arrow(int vMax, Point targetPoint)
			{
				this.vMax = vMax;
				int num = 0;
				int num2 = 0;
				if (targetPoint != null)
				{
					num = targetPoint.x - x;
					num2 = targetPoint.y - y;
					toX = targetPoint.x;
					toY = targetPoint.y;
				}
				else
				{
					num = toX - x;
					num2 = toY - y;
				}
				if (x > toX)
				{
					dir = 2;
					dir_nguoc = 0;
				}
				else
				{
					dir = 0;
					dir_nguoc = 2;
				}
				int frameAngle = Res.angle(num, num2);
				frame = setFrameAngle(frameAngle);
				fSpeed = frame;
				create_Speed(num, num2);
			}

	public void create_Speed(int dx, int dy)
			{
				int num = 0;
				int num2 = 0;
				int num3 = Res.getDistance(dx, dy) / vMax;
				if (num3 == 0)
				{
					num3 = 1;
				}
				num = dx / num3;
				num2 = dy / num3;
				if (num == 0 && dx < num3)
				{
					num = ((dx >= 0) ? 1 : (-1));
				}
				if (num2 == 0 && dy < num3)
				{
					num2 = ((dy >= 0) ? 1 : (-1));
				}
				if (Res.abs(num) > Res.abs(dx))
				{
					num = dx;
				}
				if (Res.abs(num2) > Res.abs(dy))
				{
					num2 = dy;
				}
				vx = num;
				vy = num2;
			}

	public void moveTo_xy(int toX, int toY, int fMove, int typeEff_End, int rangeEnd)
			{
				if (f < fMove)
				{
					frame = setFrameAngle((dir == -1) ? 180 : 0);
					return;
				}
				frame = fSpeed;
				if (Res.abs(x - toX) < Res.abs(vx))
				{
					x = toX;
					vx = 0;
				}
				else
				{
					x += vx;
				}
				if (Res.abs(y - toY) < Res.abs(vy))
				{
					y = toY;
					vy = 0;
				}
				else
				{
					y += vy;
				}
				if (Res.abs(x - toX) >= Res.abs(vMax) || Res.abs(y - toY) >= Res.abs(vMax) || typeEff_End < 0)
				{
					return;
				}
				if (target != null)
				{
					int num = target.x;
					int num2 = target.y;
					if (rangeEnd > 0)
					{
						num += Res.random_Am(0, rangeEnd);
						num2 += Res.random_Am(0, rangeEnd);
					}
					GameScr.addEffectEnd(typeEff_End, 0, 0, num, num2, 1, 0, -1, null);
					removeEff();
				}
				else if (isAddSub)
				{
					isAddSub = false;
					int num3 = x;
					int num4 = y;
					if (rangeEnd > 1)
					{
						num3 += Res.random_Am_0(rangeEnd);
						num4 += Res.random_Am_0(rangeEnd);
					}
					GameScr.addEffectEnd(typeEff_End, 0, 0, num3, num4, 1, 0, -1, null);
				}
			}

	private void changeAngleStar()
			{
				if (vMax < 40)
				{
					vMax += 2;
				}
				angleS = angleO;
				angleS -= vMax;
				if (angleS >= 360)
				{
					angleS -= 360;
				}
				if (angleS < 0)
				{
					angleS = 360 + angleS;
				}
				angleO = angleS;
			}

	private void Fill_Rect_Img(mGraphics g, FrameImage head, FrameImage body, FrameImage foot, int frame, int x, int y, int w)
			{
				int num = 0;
				int num2 = w;
				bool flag = false;
				if (head != null && foot != null)
				{
					flag = true;
					num2 = w - (head.frameWidth + foot.frameWidth);
				}
				if (num2 > 0)
				{
					num = num2 / body.frameWidth;
					if (num2 % body.frameWidth > 0)
					{
						num++;
					}
					if (dir == -1)
					{
						for (int i = 0; i < num; i++)
						{
							int num3 = 0;
							num3 = ((i != num - 1) ? ((!flag) ? (x + i * body.frameWidth) : (x + foot.frameWidth + body.frameWidth + i * body.frameWidth)) : ((!flag) ? (x + w - body.frameWidth) : (x + foot.frameWidth)));
							body.drawFrame(frame, num3, y - body.frameHeight / 2, 2, 0, g);
						}
					}
					else
					{
						for (int j = 0; j < num; j++)
						{
							int num4 = 0;
							num4 = ((j != num - 1) ? ((!flag) ? (x + j * body.frameWidth) : (x + j * body.frameWidth + head.frameWidth)) : ((!flag) ? (x + w - body.frameWidth) : (x + w - (body.frameWidth + foot.frameWidth))));
							body.drawFrame(frame, num4, y - body.frameHeight / 2, 0, 0, g);
						}
					}
				}
				if (dir == -1)
				{
					head?.drawFrame(frame, x + w - head.frameWidth, y - head.frameHeight / 2, 2, 0, g);
					foot?.drawFrame(frame, x, y - foot.frameHeight / 2, 2, 0, g);
				}
				else
				{
					head?.drawFrame(frame, x, y - head.frameHeight / 2, 0, 0, g);
					foot?.drawFrame(frame, x + w - foot.frameWidth - 1, y - foot.frameHeight / 2, 0, 0, g);
				}
			}

	private void pnt_LINE_IN(mGraphics g)
			{
				for (int i = 0; i < VecEffEnd.size(); i++)
				{
					Line line = (Line)VecEffEnd.elementAt(i);
					if (line != null)
					{
						int color = 0;
						if (i / 2 < colorpaint.Length)
						{
							color = colorpaint[i / 2];
						}
						g.setColor(color);
						g.drawLine(line.x0 / 1000, line.y0 / 1000, line.x1 / 1000, line.y1 / 1000);
						if (line.is2Line)
						{
							g.drawLine(line.x0 / 1000 + 1, line.y0 / 1000, line.x1 / 1000 + 1, line.y1 / 1000);
						}
					}
				}
			}

	private void upd_End_Rock()
			{
				for (int i = 0; i < VecEffEnd.size(); i++)
				{
					Point point = (Point)VecEffEnd.elementAt(i);
					point.update();
					if (point.y < toY)
					{
						VecEffEnd.removeElementAt(i);
						i--;
					}
				}
				if (f >= fRemove)
				{
					removeEff();
				}
			}

	private void pnt_End_Rock(mGraphics g)
			{
				for (int i = 0; i < VecEffEnd.size(); i++)
				{
					Point point = (Point)VecEffEnd.elementAt(i);
					if (fraImgEff != null)
					{
						fraImgEff.drawFrame(point.frame, point.x, point.y, 0, mGraphics.VCENTER | mGraphics.HCENTER, g);
					}
				}
			}

	private void updListObj_Mafuba(bool ismafuba)
			{
				if (listObj == null)
				{
					return;
				}
				for (int i = 0; i < listObj.Length; i++)
				{
					if (listObj[i] == null)
					{
						continue;
					}
					if (listObj[i].type == 0)
					{
						Mob mob = GameScr.findMobInMap(listObj[i].id);
						if (mob != null)
						{
							mob.isMafuba = ismafuba;
							mob.isHide = false;
							mob.xMFB = xDotS[i];
							mob.yMFB = yDotS[i];
						}
						continue;
					}
					Char @char = null;
					@char = ((Char.myCharz().charID != listObj[i].id) ? GameScr.findCharInMap(listObj[i].id) : Char.myCharz());
					if (@char != null)
					{
						@char.isMafuba = ismafuba;
						@char.isHide = false;
						@char.xMFB = xDotS[i];
						@char.yMFB = yDotS[i];
					}
				}
			}

	private void hideListObj_Mafuba(bool ishide)
			{
				if (listObj == null)
				{
					return;
				}
				for (int i = 0; i < listObj.Length; i++)
				{
					if (listObj[i] == null)
					{
						continue;
					}
					if (listObj[i].type == 0)
					{
						Mob mob = GameScr.findMobInMap(listObj[i].id);
						if (mob != null)
						{
							mob.isHide = ishide;
						}
						continue;
					}
					Char @char = null;
					@char = ((Char.myCharz().charID != listObj[i].id) ? GameScr.findCharInMap(listObj[i].id) : Char.myCharz());
					if (@char != null)
					{
						@char.isHide = ishide;
					}
				}
			}

	private void get_Img_Skill()
			{
				int num = 0;
				int[] array = null;
				int[] array2 = null;
				switch (typeEffect)
				{
				case 18:
					num = 24;
					array = new int[1];
					array2 = new int[1] { 9 };
					break;
				case 21:
					num = 24;
					array = new int[1] { 1 };
					array2 = new int[1] { 10 };
					break;
				case 24:
					num = 24;
					array = new int[3] { 2, 3, 4 };
					array2 = new int[3] { 11, 12, 13 };
					break;
				case 19:
					num = 25;
					array = new int[1];
					array2 = new int[1] { 14 };
					break;
				case 22:
					num = 25;
					array = new int[1] { 1 };
					array2 = new int[1] { 15 };
					break;
				case 17:
					num = 25;
					array = new int[1] { 2 };
					array2 = new int[1] { 16 };
					break;
				case 25:
					num = 25;
					array = new int[4] { 3, 4, 5, 6 };
					array2 = new int[4] { 17, 18, 19, 20 };
					break;
				case 20:
					num = 26;
					array = new int[1];
					array2 = new int[1] { 21 };
					break;
				case 23:
					num = 26;
					array = new int[1] { 1 };
					array2 = new int[1] { 22 };
					break;
				case 16:
					num = 26;
					if (typeSub == 0)
					{
						array = new int[1] { 7 };
						array2 = new int[1] { 28 };
					}
					if (typeSub == 1)
					{
						array = new int[1] { 2 };
						array2 = new int[1] { 23 };
					}
					break;
				case 26:
				{
					num = 26;
					int num2 = 0;
					int num3 = 0;
					if (typeSub == 0)
					{
						num2 = 4;
						num3 = 25;
					}
					else if (typeSub == 1)
					{
						num2 = 5;
						num3 = 26;
					}
					else if (typeSub == 2)
					{
						num2 = 6;
						num3 = 27;
					}
					array = new int[2] { num2, 3 };
					array2 = new int[2] { num3, 24 };
					break;
				}
				}
				if (array == null || array2 == null)
				{
					return;
				}
				fra_skill = new FrameImage[array.Length];
				for (int i = 0; i < array.Length; i++)
				{
					string nameImg = "Skills_" + num + "_" + typePaint + "_" + array[i];
					FrameImage frameImage = mSystem.getFraImage(nameImg);
					if (frameImage == null)
					{
						frameImage = new FrameImage(array2[i]);
					}
					if (frameImage != null)
					{
						fra_skill[i] = frameImage;
					}
				}
			}

	private void upd_Gong()
			{
				if (charUse != null)
				{
					if (typeEffect == 21)
					{
						x = charUse.cx - 3 * charUse.cdir;
						y = charUse.cy;
					}
					else if (typeEffect == 22)
					{
						x = charUse.cx + 20 * charUse.cdir;
						y = charUse.cy - 4;
					}
					else if (typeEffect == 23)
					{
						x = charUse.cx;
						y = charUse.cy - 50;
					}
					else
					{
						x = charUse.cx;
						y = charUse.cy;
					}
				}
				if (timeRemove > 0)
				{
					if (GameCanvas.timeNow - time >= timeRemove)
					{
						removeEff();
					}
				}
				else if (f >= fra_skill[0].nFrame * n_frame)
				{
					removeEff();
				}
			}

	private void pnt_Gong(mGraphics g, int anchor)
			{
				if (fra_skill[0] != null)
				{
					fra_skill[0].drawFrame(f / n_frame % fra_skill[0].nFrame, x, y, dir_nguoc, anchor, g);
				}
			}

	private void upd_Pow()
			{
				if (charUse != null)
				{
					x = charUse.cx;
					y = charUse.cy + 13;
				}
				if (timeRemove > 0)
				{
					if (GameCanvas.timeNow - time >= timeRemove)
					{
						removeEff();
					}
				}
				else if (nFrame != null)
				{
					if (f > nFrame.Length)
					{
						removeEff();
					}
				}
				else if (f >= fra_skill[0].nFrame * n_frame)
				{
					removeEff();
				}
			}

	private void pnt_Pow(mGraphics g, int anchor)
			{
				if (fra_skill[0] != null)
				{
					if (nFrame != null)
					{
						fra_skill[0].drawFrame(nFrame[f % nFrame.Length], x, y, dir_nguoc, anchor, g);
					}
					else
					{
						fra_skill[0].drawFrame(f / n_frame % fra_skill[0].nFrame, x, y, dir_nguoc, anchor, g);
					}
				}
			}

	private void upd_Sub()
			{
				if (timeRemove > 0)
				{
					if (GameCanvas.timeNow - time >= timeRemove)
					{
						removeEff();
					}
				}
				else if (f >= fra_skill[0].nFrame * n_frame)
				{
					removeEff();
				}
			}

	private void pnt_Sub(mGraphics g, int anchor)
			{
				fra_skill[0].drawFrame(f / n_frame % fra_skill[0].nFrame, x, y, dir, anchor, g);
			}

	private void upd_()
			{
			}

	private void pnt_(mGraphics g)
			{
			}

}
