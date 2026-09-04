using System;
using Assets.src.e;
using Assets.src.g;
using UnityEngine;
public partial class Char : IMapObject
{
	public void paintHp(mGraphics g, int x, int y)
			{
				int num = (int)((int)cHP * 100 / cHPFull) / 10 - 1;
				if (num < 0)
				{
					num = 0;
				}
				if (num > 9)
				{
					num = 9;
				}
				if (!me)
				{
					g.drawRegion(Mob.imgHP, 0, 6 * (9 - num), 9, 6, 0, x, y - mFont.tahoma_7.getHeight() - 6, 3);
				}
				if (cTypePk == 0 && (myCharz().cFlag == 0 || cFlag == 0 || (cFlag != 8 && myCharz().cFlag != 8 && cFlag == myCharz().cFlag)))
				{
					return;
				}
				len = (int)(cHP * 100 / cHPFull * w_hp_bar) / 100;
				num = (int)(cHP * 100 / cHPFull);
				if (num < 30)
				{
					imgHPtem = GameScr.imgHP_tm_do;
				}
				else if (num < 60)
				{
					imgHPtem = GameScr.imgHP_tm_vang;
				}
				else
				{
					imgHPtem = GameScr.imgHP_tm_xanh;
				}
				int imageWidth = mGraphics.getImageWidth(GameScr.imgHP_tm_xam);
				int imageHeight = mGraphics.getImageHeight(GameScr.imgHP_tm_xam);
				int w = imageWidth * num / 100;
				g.drawImage(GameScr.imgHP_tm_xam, x - (imageWidth >> 1), y - 1, mGraphics.TOP | mGraphics.LEFT);
				if (len < 5)
				{
					if (GameCanvas.gameTick % 6 < 3)
					{
						g.drawRegion(imgHPtem, 0, 0, w, imageHeight, 0, x - (imageWidth >> 1), y - 1, mGraphics.TOP | mGraphics.LEFT);
					}
				}
				else
				{
					g.drawRegion(imgHPtem, 0, 0, w, imageHeight, 0, x - (imageWidth >> 1), y - 1, mGraphics.TOP | mGraphics.LEFT);
				}
			}
	public void paintNameInSameParty(mGraphics g)
			{
				if (cTypePk != 3 && cTypePk != 5 && isPaint())
				{
					if (myCharz().charFocus == null || !myCharz().charFocus.Equals(this))
					{
						mFont.tahoma_7_yellow.drawString(g, cName, cx, cy - ch - mFont.tahoma_7_green.getHeight() - 5, mFont.CENTER, mFont.tahoma_7_grey);
					}
					else if (myCharz().charFocus != null && myCharz().charFocus.Equals(this))
					{
						mFont.tahoma_7_yellow.drawString(g, cName, cx, cy - ch - mFont.tahoma_7_green.getHeight() - 10, mFont.CENTER, mFont.tahoma_7_grey);
					}
				}
			}
	private void paintCharWithoutSkill(mGraphics g)
			{
				try
				{
					if (isMafuba)
					{
						paintCharBody(g, xMFB, yMFB, cdir, cf, isPaintBag: false);
						return;
					}
					if (isInvisiblez)
					{
						if (me)
						{
							if (GameCanvas.gameTick % 50 == 48 || GameCanvas.gameTick % 50 == 90)
							{
								SmallImage.drawSmallImage(g, 1196, cx, cy - 18, 0, mGraphics.VCENTER | mGraphics.HCENTER);
							}
							else
							{
								SmallImage.drawSmallImage(g, 1195, cx, cy - 18, 0, mGraphics.VCENTER | mGraphics.HCENTER);
							}
						}
					}
					else
					{
						paintCharBody(g, cx, cy + fy, cdir, cf, isPaintBag: true);
					}
					if (isLockAttack)
					{
						SmallImage.drawSmallImage(g, 290, cx, cy, 0, mGraphics.BOTTOM | mGraphics.HCENTER);
					}
				}
				catch (Exception ex)
				{
					Cout.LogError("Loi paint char without skill: " + ex.ToString());
				}
			}
	public void paintCharWithSkill(mGraphics g)
			{
				ty = 0;
				SkillInfoPaint[] array = skillInfoPaint();
				cf = array[indexSkill].status;
				paintCharWithoutSkill(g);
				if (cdir == 1)
				{
					if (eff0 != null)
					{
						if (dx0 == 0)
						{
							dx0 = array[indexSkill].e0dx;
						}
						if (dy0 == 0)
						{
							dy0 = array[indexSkill].e0dy;
						}
						SmallImage.drawSmallImage(g, eff0.arrEfInfo[i0].idImg, cx + dx0 + eff0.arrEfInfo[i0].dx, cy + dy0 + eff0.arrEfInfo[i0].dy, 0, mGraphics.VCENTER | mGraphics.HCENTER);
						i0++;
						if (i0 >= eff0.arrEfInfo.Length)
						{
							eff0 = null;
							i0 = (dx0 = (dy0 = 0));
						}
					}
					if (eff1 != null)
					{
						if (dx1 == 0)
						{
							dx1 = array[indexSkill].e1dx;
						}
						if (dy1 == 0)
						{
							dy1 = array[indexSkill].e1dy;
						}
						SmallImage.drawSmallImage(g, eff1.arrEfInfo[i1].idImg, cx + dx1 + eff1.arrEfInfo[i1].dx, cy + dy1 + eff1.arrEfInfo[i1].dy, 0, mGraphics.VCENTER | mGraphics.HCENTER);
						i1++;
						if (i1 >= eff1.arrEfInfo.Length)
						{
							eff1 = null;
							i1 = (dx1 = (dy1 = 0));
						}
					}
					if (eff2 != null)
					{
						if (dx2 == 0)
						{
							dx2 = array[indexSkill].e2dx;
						}
						if (dy2 == 0)
						{
							dy2 = array[indexSkill].e2dy;
						}
						SmallImage.drawSmallImage(g, eff2.arrEfInfo[i2].idImg, cx + dx2 + eff2.arrEfInfo[i2].dx, cy + dy2 + eff2.arrEfInfo[i2].dy, 0, mGraphics.VCENTER | mGraphics.HCENTER);
						i2++;
						if (i2 >= eff2.arrEfInfo.Length)
						{
							eff2 = null;
							i2 = (dx2 = (dy2 = 0));
						}
					}
				}
				else
				{
					if (eff0 != null)
					{
						if (dx0 == 0)
						{
							dx0 = array[indexSkill].e0dx;
						}
						if (dy0 == 0)
						{
							dy0 = array[indexSkill].e0dy;
						}
						SmallImage.drawSmallImage(g, eff0.arrEfInfo[i0].idImg, cx - dx0 - eff0.arrEfInfo[i0].dx, cy + dy0 + eff0.arrEfInfo[i0].dy, 2, mGraphics.VCENTER | mGraphics.HCENTER);
						i0++;
						if (i0 >= eff0.arrEfInfo.Length)
						{
							eff0 = null;
							i0 = 0;
							dx0 = 0;
							dy0 = 0;
						}
					}
					if (eff1 != null)
					{
						if (dx1 == 0)
						{
							dx1 = array[indexSkill].e1dx;
						}
						if (dy1 == 0)
						{
							dy1 = array[indexSkill].e1dy;
						}
						SmallImage.drawSmallImage(g, eff1.arrEfInfo[i1].idImg, cx - dx1 - eff1.arrEfInfo[i1].dx, cy + dy1 + eff1.arrEfInfo[i1].dy, 2, mGraphics.VCENTER | mGraphics.HCENTER);
						i1++;
						if (i1 >= eff1.arrEfInfo.Length)
						{
							eff1 = null;
							i1 = 0;
							dx1 = 0;
							dy1 = 0;
						}
					}
					if (eff2 != null)
					{
						if (dx2 == 0)
						{
							dx2 = array[indexSkill].e2dx;
						}
						if (dy2 == 0)
						{
							dy2 = array[indexSkill].e2dy;
						}
						SmallImage.drawSmallImage(g, eff2.arrEfInfo[i2].idImg, cx - dx2 - eff2.arrEfInfo[i2].dx, cy + dy2 + eff2.arrEfInfo[i2].dy, 2, mGraphics.VCENTER | mGraphics.HCENTER);
						i2++;
						if (i2 >= eff2.arrEfInfo.Length)
						{
							eff2 = null;
							i2 = 0;
							dx2 = 0;
							dy2 = 0;
						}
					}
				}
				indexSkill++;
			}
	private void paintPKFlag(mGraphics g)
			{
				if (cdir == 1)
				{
					if (cFlag != 0 && cFlag != -1)
					{
						SmallImage.drawSmallImage(g, flagImage, cx - 10, cy - ch - ((!me) ? 30 : 30) + ((GameCanvas.gameTick % 20 > 10) ? (GameCanvas.gameTick % 4 / 2) : 0), 2, 0);
					}
				}
				else if (cFlag != 0 && cFlag != -1)
				{
					SmallImage.drawSmallImage(g, flagImage, cx, cy - ch - ((!me) ? 30 : 30) + ((GameCanvas.gameTick % 20 > 10) ? (GameCanvas.gameTick % 4 / 2) : 0), 0, 0);
				}
			}
	public void paintHat_behind(mGraphics g, int cf, int yh)
			{
				try
				{
					if (idHat == -1)
					{
						return;
					}
					if (isFrNgang(cf))
					{
						if (fraHat_behind_2 != null)
						{
							fraHat_behind_2.drawFrame(GameCanvas.gameTick / 4 % fraHat_behind_2.nFrame, cx + hatInfo[cf][0] * ((cdir == 1) ? 1 : (-1)), yh + hatInfo[cf][1], (cdir != 1) ? 2 : 0, mGraphics.BOTTOM | mGraphics.HCENTER, g);
						}
						else
						{
							fraHat_behind_2 = mSystem.getFraImage(strHat_behind + strNgang + idHat);
						}
					}
					else if (fraHat_behind != null)
					{
						fraHat_behind.drawFrame(GameCanvas.gameTick / 4 % fraHat_behind.nFrame, cx + hatInfo[cf][0] * ((cdir == 1) ? 1 : (-1)), yh + hatInfo[cf][1], (cdir != 1) ? 2 : 0, mGraphics.BOTTOM | mGraphics.HCENTER, g);
					}
					else
					{
						fraHat_behind = mSystem.getFraImage(strHat_behind + idHat);
					}
				}
				catch (Exception)
				{
				}
			}
	public void paintHat_front(mGraphics g, int cf, int yh)
			{
				try
				{
					if (idHat == -1)
					{
						return;
					}
					if (isFrNgang(cf))
					{
						if (fraHat_font_2 != null)
						{
							fraHat_font_2.drawFrame(GameCanvas.gameTick / 4 % fraHat_font_2.nFrame, cx + hatInfo[cf][0] * ((cdir == 1) ? 1 : (-1)), yh + hatInfo[cf][1], (cdir != 1) ? 2 : 0, mGraphics.BOTTOM | mGraphics.HCENTER, g);
						}
						else
						{
							fraHat_font_2 = mSystem.getFraImage(strHat_font + strNgang + idHat);
						}
					}
					else if (fraHat_font != null)
					{
						fraHat_font.drawFrame(GameCanvas.gameTick / 4 % fraHat_font.nFrame, cx + hatInfo[cf][0] * ((cdir == 1) ? 1 : (-1)), yh + hatInfo[cf][1], (cdir != 1) ? 2 : 0, mGraphics.BOTTOM | mGraphics.HCENTER, g);
					}
					else
					{
						fraHat_font = mSystem.getFraImage(strHat_font + idHat);
					}
				}
				catch (Exception)
				{
				}
			}

}
