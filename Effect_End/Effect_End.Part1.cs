using System;
public partial class Effect_End
{
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

}
