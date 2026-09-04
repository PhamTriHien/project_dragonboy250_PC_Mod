using System;
public partial class Effect_End
{
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

}
