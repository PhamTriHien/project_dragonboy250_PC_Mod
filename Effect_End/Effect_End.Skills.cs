using System;

public partial class Effect_End
{
	private void upd_End_String()
			{
				x = GameCanvas.hw;
				y = y1000;
				if (f > fRemove)
				{
					removeEff();
				}
				vy++;
				if (vy > 15)
				{
					vy = 15;
				}
				if (y1000 + vy < dy_throw)
				{
					y1000 += vy;
					return;
				}
				y1000 = dy_throw;
				if (!isAddSub)
				{
					isAddSub = true;
					if (typeSub != -1)
					{
						GameScr.addEffectEnd(typeSub, 0, 0, x, y, levelPaint, 0, -1, null);
					}
				}
			}

	private void pnt_End_String(mGraphics g)
			{
				if (fraImgEff != null)
				{
					fraImgEff.drawFrame(f / 5 % fraImgEff.nFrame, x, y, 0, 33, g);
				}
			}

	private void upd_FireWork()
			{
				for (int i = 0; i < VecEffEnd.size(); i++)
				{
					Point point = (Point)VecEffEnd.elementAt(i);
					point.update();
					if (point.f == point.fRe)
					{
						SoundMn.playSound(point.x, point.y, SoundMn.FIREWORK, SoundMn.volume);
					}
					if (point.f - point.fRe <= point.fraImgEff.nFrame * 3 - 1)
					{
						continue;
					}
					point.f = 0;
					if (typeSub == 0)
					{
						point.fRe = Res.random(10);
						int num = 1;
						if (i % 2 == 0)
						{
							num = -1;
						}
						point.x = x + Res.random(arrInfoEff[5][0] / 2) * num;
						point.y = y - Res.random(arrInfoEff[5][1] / 2);
					}
				}
				if (f >= fRemove)
				{
					removeEff();
				}
			}

	private void pnt_FireWork(mGraphics g)
			{
				for (int i = 0; i < VecEffEnd.size(); i++)
				{
					Point point = (Point)VecEffEnd.elementAt(i);
					if (point.f - point.fRe > -1 && point.fraImgEff != null)
					{
						point.fraImgEff.drawFrame((point.f - point.fRe) / 3 % point.fraImgEff.nFrame, point.x, point.y, 0, 3, g);
					}
				}
			}

	private void upd_Skill_Kamex10()
			{
				fSpeed++;
				w += 20;
				if (w > vMax)
				{
					w = vMax;
				}
				x = charUse.cx + 10;
				y = charUse.cy - 3;
				if (dir == -1)
				{
					x = charUse.cx - w - 10;
				}
				if (!isAddSub && GameCanvas.timeNow - time >= timeRemove)
				{
					f = 0;
					nFrame = new byte[6] { 2, 2, 2, 3, 3, 3 };
					isAddSub = true;
				}
				if (f > nFrame.Length - 1)
				{
					if (isAddSub)
					{
						removeEff();
					}
					else
					{
						f = 0;
					}
				}
			}

	private void pnt_Skill_Kamex10(mGraphics g)
			{
				if (fra_skill != null)
				{
					g.setClip(x, y - h / 2, w, h);
					Fill_Rect_Img(g, fra_skill[0], fra_skill[1], fra_skill[2], nFrame[f], x, y, vMax);
					GameCanvas.resetTransGameScr(g);
					if (dir == -1 && fra_skill[0] != null)
					{
						fra_skill[0].drawFrame(nFrame[f], x + w - fra_skill[0].frameWidth, y - fra_skill[0].frameHeight / 2 - 1, 2, 0, g);
					}
				}
			}

	private void upd_Skill_Destroy()
			{
				int num = 0;
				for (int i = 0; i < VecEffEnd.size(); i++)
				{
					Point point = (Point)VecEffEnd.elementAt(i);
					if (!point.isPaint && GameCanvas.timeNow - time >= i * fMove)
					{
						point.isPaint = true;
						GameScr.addEffectEnd(17, 0, typePaint, charUse.cx, charUse.cy - 3, 2, dir_nguoc, -1, null);
						if (i == VecEffEnd.size() - 1)
						{
							SoundMn.playSound(point.x, point.y, SoundMn.DESTROY_1, SoundMn.volume);
						}
						else
						{
							SoundMn.playSound(point.x, point.y, SoundMn.DESTROY_0, SoundMn.volume);
						}
					}
					if (point.isPaint && !point.isRemove)
					{
						point.f++;
						if (!point.isChange)
						{
							if (point.f < 10 && i == VecEffEnd.size() - 1 && charUse != null && !TileMap.tileTypeAt(charUse.cx - (charUse.chw + 1) * charUse.cdir, charUse.cy, (charUse.cdir != 1) ? 4 : 8))
							{
								charUse.cx -= charUse.cdir;
							}
							point.moveTo_xy(point.toX, point.toY);
							if (point.x == point.toX)
							{
								point.isChange = true;
								point.f = 0;
							}
						}
						if (point.isChange && point.f >= n_frame * point.fraImgEff_2.nFrame)
						{
							point.isRemove = true;
						}
					}
					if (point.isRemove)
					{
						num++;
					}
				}
				if (num == VecEffEnd.size())
				{
					removeEff();
				}
			}

	private void pnt_Skill_Destroy(mGraphics g)
			{
				for (int i = 0; i < VecEffEnd.size(); i++)
				{
					Point point = (Point)VecEffEnd.elementAt(i);
					if (point.isPaint && !point.isRemove)
					{
						if (!point.isChange)
						{
							point.paint_Arrow(g, point.fraImgEff, mGraphics.VCENTER | mGraphics.HCENTER, isCountFr: false);
						}
						if (point.isChange)
						{
							point.fraImgEff_2.drawFrame(point.f / n_frame % point.fraImgEff_2.nFrame, point.x, point.y, dir_nguoc, mGraphics.VCENTER | mGraphics.HCENTER, g);
						}
					}
				}
			}

	private void upd_Skill_MaFuba()
			{
				if (stt == 0)
				{
					if (f == 3)
					{
						SoundMn.playSound(x, y, SoundMn.MAFUBA_1, SoundMn.volume);
					}
					frame++;
					if (frame > nFrame.Length - 1)
					{
						frame = nFrame.Length - 1;
					}
					if (f == fMove + 4)
					{
						GameScr.addEffectEnd(16, 1, typePaint, x, y, 3, 0, 2945, null);
					}
					if (f > fMove + 4)
					{
						rS--;
						if (rS < 0)
						{
							rS = 0;
							f = 0;
							fSpeed = 0;
							nFrame_2 = new byte[22]
							{
								1, 1, 0, 0, 0, 0, 1, 1, 1, 1,
								0, 0, 0, 1, 1, 1, 0, 0, 1, 1,
								1, 2
							};
							hideListObj_Mafuba(ishide: true);
							stt = 1;
						}
						else
						{
							changeAngleStar();
							setDotStar();
							updListObj_Mafuba(ismafuba: true);
						}
					}
				}
				else if (stt == 1)
				{
					fSpeed++;
					if (fSpeed > nFrame_2.Length - 1)
					{
						fSpeed = nFrame_2.Length - 1;
						if (GameCanvas.gameTick % 2 == 0)
						{
							vy1000++;
						}
						vy += vy1000;
						if (vy >= h - fra_skill[0].frameHeight - dy + dy_throw)
						{
							vy = h - fra_skill[0].frameHeight - dy + dy_throw;
							f = 0;
							fSpeed = 0;
							stt = 2;
							nFrame_2 = new byte[11]
							{
								3, 3, 3, 3, 3, 4, 4, 4, 5, 5,
								5
							};
						}
					}
				}
				else if (stt == 2)
				{
					fSpeed++;
					if (fSpeed > nFrame_2.Length - 1)
					{
						stt = 3;
						frame = 0;
						nFrame = new byte[17]
						{
							2, 2, 1, 1, 0, 0, 3, 3, 3, 0,
							0, 0, 4, 4, 4, 0, 0
						};
					}
				}
				else if (stt == 3)
				{
					frame++;
					if (frame == 3)
					{
						SoundMn.playSound(x, y, SoundMn.MAFUBA_1, SoundMn.volume);
					}
					if (frame > nFrame.Length - 1)
					{
						frame = 0;
						stt = 4;
						nFrame = new byte[51]
						{
							0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
							0, 0, 0, 0, 0, 0, 0, 3, 3, 3,
							0, 0, 0, 4, 4, 4, 0, 0, 0, 0,
							0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
							0, 0, 0, 0, 0, 3, 3, 0, 0, 4,
							4
						};
					}
				}
				else
				{
					frame++;
					if (frame > nFrame.Length - 1)
					{
						frame = 0;
					}
					if (GameCanvas.timeNow - time >= timeRemove)
					{
						GameScr.addEffectEnd(16, 0, typePaint, x1000, y1000, 1, 0, -1, null);
						updListObj_Mafuba(ismafuba: false);
						removeEff();
					}
				}
			}

	private void pnt_Skill_MaFuba(mGraphics g)
			{
				if (fra_skill == null)
				{
					return;
				}
				if (nFrame != null)
				{
					fra_skill[0].drawFrame(nFrame[frame], x1000, y1000, 0, mGraphics.BOTTOM | mGraphics.HCENTER, g);
				}
				if (stt == 1 || stt == 2)
				{
					int anchor = mGraphics.BOTTOM | mGraphics.HCENTER;
					int num = dy;
					if (nFrame_2[fSpeed] == 0 || nFrame_2[fSpeed] == 1)
					{
						anchor = mGraphics.VCENTER | mGraphics.HCENTER;
						num = 0;
					}
					fra_skill[1].drawFrame(nFrame_2[fSpeed], x, y + num + vy, 0, anchor, g);
				}
			}

	private void upd_LINE_IN()
			{
				for (int i = 0; i < VecEffEnd.size(); i++)
				{
					Line line = (Line)VecEffEnd.elementAt(i);
					line.update();
					if (f >= fRemove)
					{
						VecEffEnd.removeElement(line);
						i--;
					}
				}
				if (f >= fRemove)
				{
					if (GameCanvas.timeNow - time >= timeRemove)
					{
						VecEffEnd.removeAllElements();
						removeEff();
					}
					else
					{
						fRemove = Res.random(4, 6);
						f = 0;
						create_Star_Line_In(vMax, xline, yline, 0);
					}
				}
			}

	private void create_Star_Line_In(int vline, int minline, int maxline, int numpoint)
			{
				if (f == -1)
				{
					VecEffEnd.removeAllElements();
				}
				int num = 4;
				colorpaint = new int[num];
				if (maxline <= minline)
				{
					maxline = minline + 1;
				}
				for (int i = 0; i < num; i++)
				{
					if (Res.random(2) == 0)
					{
						colorpaint[i] = colorStar[indexColorStar][Res.random(3)];
					}
					else
					{
						colorpaint[i] = colorStar[indexColorStar][2];
					}
				}
				for (int j = 0; j < num; j++)
				{
					Line line = new Line();
					int num2 = 5 + 180 / num * j;
					int num3 = 180 / num + 180 / num * j - 5;
					if (num3 <= num2)
					{
						num3 = num2 + 1;
					}
					int num4 = Res.random(minline, maxline);
					int num5 = Res.random(vline, vline + 3);
					int num6 = Res.random(num2, num3);
					int num7 = Res.random(13, 23);
					bool is2Line = Res.random(4) == 0;
					num6 = Res.fixangle(num6 % 360);
					line.setLine(x1000 - Res.sin(num6) * (num4 + num7), y1000 - Res.cos(num6) * (num4 + num7), x1000 - Res.sin(num6) * num7, y1000 - Res.cos(num6) * num7, Res.sin(num6) * num5, Res.cos(num6) * num5, is2Line);
					if (numpoint > 0)
					{
						line.type = Res.random(numpoint);
					}
					VecEffEnd.addElement(line);
					line = new Line();
					num6 += 180 + Res.random_Am(2, 5);
					num6 = Res.fixangle(num6 % 360);
					line.setLine(x1000 - Res.sin(num6) * (num4 + num7), y1000 - Res.cos(num6) * (num4 + num7), x1000 - Res.sin(num6) * num7, y1000 - Res.cos(num6) * num7, Res.sin(num6) * num5, Res.cos(num6) * num5, is2Line);
					if (numpoint > 0)
					{
						line.type = Res.random(numpoint);
					}
					VecEffEnd.addElement(line);
				}
			}

}
