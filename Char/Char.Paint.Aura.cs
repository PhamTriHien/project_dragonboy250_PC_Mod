using System;
using Assets.src.e;
using Assets.src.g;
using UnityEngine;

public partial class Char : IMapObject
{
	private void paintEff_Pet(mGraphics g)
			{
				for (int i = 0; i < vEffChar.size(); i++)
				{
					Effect effect = (Effect)vEffChar.elementAt(i);
					if (effect.effId >= 201)
					{
						effect.paint(g);
					}
				}
			}

	private void paintSuperEffBehind(mGraphics g)
			{
				if ((me && !isPaintAura2) || idAuraEff > -1 || (statusMe != 1 && statusMe != 6) || mSystem.currentTimeMillis() - timeBlue <= 0 || isCopy || clevel < 16)
				{
					return;
				}
				int num = 7598;
				int num2 = 4;
				if (clevel >= 19)
				{
					num = 7676;
				}
				if (clevel >= 22)
				{
					num = 7677;
				}
				if (clevel >= 25)
				{
					num = 7678;
				}
				if (num != -1)
				{
					Small small = SmallImage.imgNew[num];
					if (small == null)
					{
						SmallImage.createImage(num);
						return;
					}
					int y = GameCanvas.gameTick / 4 % num2 * (mGraphics.getImageHeight(small.img) / num2);
					g.drawRegion(small.img, 0, y, mGraphics.getImageWidth(small.img), mGraphics.getImageHeight(small.img) / num2, 0, cx, cy + 2, mGraphics.BOTTOM | mGraphics.HCENTER);
				}
			}

	private void paintSuperEffFront(mGraphics g)
			{
				if (!isPaintAura2)
				{
					return;
				}
				if (statusMe == 1 || statusMe == 6)
				{
					if (mSystem.currentTimeMillis() - timeBlue <= 0)
					{
						return;
					}
					if (isCopy)
					{
						if (GameCanvas.gameTick % 2 == 0)
						{
							tBlue++;
						}
						if (tBlue > 6)
						{
							tBlue = 0;
						}
						g.drawImage(GameCanvas.imgViolet[tBlue], cx, cy + 9, mGraphics.BOTTOM | mGraphics.HCENTER);
						return;
					}
					if (clevel >= 14 && !GameCanvas.lowGraphic)
					{
						bool flag = false;
						if (mSystem.currentTimeMillis() - timeBlue > -1000 && IsAddDust1)
						{
							flag = true;
							IsAddDust1 = false;
						}
						if (mSystem.currentTimeMillis() - timeBlue > -500 && IsAddDust2)
						{
							flag = true;
							IsAddDust2 = false;
						}
						if (flag)
						{
							GameCanvas.gI().startDust(-1, cx - -8, cy);
							GameCanvas.gI().startDust(1, cx - 8, cy);
							addDustEff(1);
						}
					}
					if (clevel == 14)
					{
						if (GameCanvas.gameTick % 2 == 0)
						{
							tBlue++;
						}
						if (tBlue > 6)
						{
							tBlue = 0;
						}
						g.drawImage(GameCanvas.imgBlue[tBlue], cx, cy + 9, mGraphics.BOTTOM | mGraphics.HCENTER);
					}
					else if (clevel == 15)
					{
						if (GameCanvas.gameTick % 2 == 0)
						{
							tBlue++;
						}
						if (tBlue > 6)
						{
							tBlue = 0;
						}
						g.drawImage(GameCanvas.imgViolet[tBlue], cx, cy + 9, mGraphics.BOTTOM | mGraphics.HCENTER);
					}
					else
					{
						if (clevel < 16)
						{
							return;
						}
						int num = -1;
						int num2 = 4;
						if (clevel >= 16 && clevel < 22)
						{
							num = 7599;
							num2 = 4;
						}
						if (num != -1)
						{
							Small small = SmallImage.imgNew[num];
							if (small == null)
							{
								SmallImage.createImage(num);
								return;
							}
							int y = GameCanvas.gameTick / 4 % num2 * (mGraphics.getImageHeight(small.img) / num2);
							g.drawRegion(small.img, 0, y, mGraphics.getImageWidth(small.img), mGraphics.getImageHeight(small.img) / num2, 0, cx, cy + 2, mGraphics.BOTTOM | mGraphics.HCENTER);
						}
					}
				}
				else
				{
					timeBlue = mSystem.currentTimeMillis() + 1500;
					IsAddDust1 = true;
					IsAddDust2 = true;
				}
			}

	private void paintEffect(mGraphics g)
			{
				if (effPaints != null)
				{
					for (int i = 0; i < effPaints.Length; i++)
					{
						if (effPaints[i] == null)
						{
							continue;
						}
						if (effPaints[i].eMob != null)
						{
							int y = effPaints[i].eMob.y;
							if (effPaints[i].eMob is BigBoss)
							{
								y = effPaints[i].eMob.y - 60;
							}
							if (effPaints[i].eMob is BigBoss2)
							{
								y = effPaints[i].eMob.y - 50;
							}
							if (effPaints[i].eMob is BachTuoc)
							{
								y = effPaints[i].eMob.y - 40;
							}
							SmallImage.drawSmallImage(g, effPaints[i].getImgId(), effPaints[i].eMob.x, y, 0, mGraphics.BOTTOM | mGraphics.HCENTER);
						}
						else if (effPaints[i].eChar != null)
						{
							SmallImage.drawSmallImage(g, effPaints[i].getImgId(), effPaints[i].eChar.cx, effPaints[i].eChar.cy, 0, mGraphics.BOTTOM | mGraphics.HCENTER);
						}
					}
				}
				if (indexEff >= 0 && eff != null)
				{
					SmallImage.drawSmallImage(g, eff.arrEfInfo[indexEff].idImg, cx + eff.arrEfInfo[indexEff].dx, cy + eff.arrEfInfo[indexEff].dy, 0, mGraphics.VCENTER | mGraphics.HCENTER);
				}
				if (indexEffTask >= 0 && effTask != null)
				{
					SmallImage.drawSmallImage(g, effTask.arrEfInfo[indexEffTask].idImg, cx + effTask.arrEfInfo[indexEffTask].dx, cy + effTask.arrEfInfo[indexEffTask].dy, 0, mGraphics.VCENTER | mGraphics.HCENTER);
				}
			}

	public void paintShadow(mGraphics g)
			{
				if (isMabuHold || head == 377 || leg == 471 || isTeleport || isFlyUp)
				{
					return;
				}
				int num = TileMap.size;
				if ((TileMap.mapID < 114 || TileMap.mapID > 120) && TileMap.mapID != 127 && TileMap.mapID != 128 && !TileMap.tileTypeAt(xSd + num / 2, ySd + 1, 4))
				{
					if (TileMap.tileTypeAt((xSd - num / 2) / num, (ySd + 1) / num) == 0)
					{
						g.setClip(xSd / num * num, (ySd - 30) / num * num, 100, 100);
					}
					else if (TileMap.tileTypeAt((xSd + num / 2) / num, (ySd + 1) / num) == 0)
					{
						g.setClip(xSd / num * num, (ySd - 30) / num * num, num, 100);
					}
					else if (TileMap.tileTypeAt(xSd - num / 2, ySd + 1, 8))
					{
						g.setClip(xSd / 24 * num, (ySd - 30) / num * num, num, 100);
					}
				}
				g.drawImage(TileMap.bong, xSd, ySd, 3);
				g.setClip(GameScr.cmx, GameScr.cmy - GameCanvas.transY, GameScr.gW, GameScr.gH + 2 * GameCanvas.transY);
			}

	public void updateShadown()
			{
				int num = 0;
				xSd = cx;
				if (TileMap.tileTypeAt(cx, cy, 2))
				{
					ySd = cy;
					return;
				}
				ySd = cy;
				while (num < 30)
				{
					num++;
					ySd += 24;
					if (TileMap.tileTypeAt(xSd, ySd, 2))
					{
						if (ySd % 24 != 0)
						{
							ySd -= ySd % 24;
						}
						break;
					}
				}
			}

	public void paintEffBehind(mGraphics g)
			{
				for (int i = 0; i < vEffChar.size(); i++)
				{
					Effect effect = (Effect)vEffChar.elementAt(i);
					if (effect.layer == 0)
					{
						bool flag = true;
						if (effect.isStand == 0)
						{
							flag = ((statusMe == 1 || statusMe == 6) ? true : false);
						}
						if (flag)
						{
							effect.paint(g);
						}
					}
				}
			}

	public void paintEffFront(mGraphics g)
			{
				for (int i = 0; i < vEffChar.size(); i++)
				{
					Effect effect = (Effect)vEffChar.elementAt(i);
					if (effect.layer == 1)
					{
						bool flag = true;
						if (effect.isStand == 0)
						{
							flag = ((statusMe == 1 || statusMe == 6) ? true : false);
						}
						if (flag)
						{
							effect.paint(g);
						}
					}
				}
			}

	public void paintAuraBehind(mGraphics g)
			{
				if ((!me || isPaintAura) && idAuraEff > -1 && (statusMe == 1 || statusMe == 6) && !GameCanvas.panel.isShow && mSystem.currentTimeMillis() - timeBlue > 0)
				{
					string nameImg = strEffAura + idAuraEff + "_0";
					FrameImage fraImage = mSystem.getFraImage(nameImg);
					fraImage?.drawFrame(GameCanvas.gameTick / 4 % fraImage.nFrame, cx, cy, (cdir != 1) ? 2 : 0, mGraphics.BOTTOM | mGraphics.HCENTER, g);
				}
			}

	public void paintAuraFront(mGraphics g)
			{
				if ((me && !isPaintAura) || idAuraEff <= -1)
				{
					return;
				}
				if (statusMe == 1 || statusMe == 6)
				{
					if (!GameCanvas.panel.isShow && !GameCanvas.lowGraphic)
					{
						bool flag = false;
						if (mSystem.currentTimeMillis() - timeBlue > -1000 && IsAddDust1)
						{
							flag = true;
							IsAddDust1 = false;
						}
						if (mSystem.currentTimeMillis() - timeBlue > -500 && IsAddDust2)
						{
							flag = true;
							IsAddDust2 = false;
						}
						if (flag)
						{
							GameCanvas.gI().startDust(-1, cx - -8, cy);
							GameCanvas.gI().startDust(1, cx - 8, cy);
							addDustEff(1);
						}
						if (mSystem.currentTimeMillis() - timeBlue > 0)
						{
							string nameImg = strEffAura + idAuraEff + "_1";
							FrameImage fraImage = mSystem.getFraImage(nameImg);
							fraImage?.drawFrame(GameCanvas.gameTick / 4 % fraImage.nFrame, cx, cy + 2, (cdir != 1) ? 2 : 0, mGraphics.BOTTOM | mGraphics.HCENTER, g);
						}
					}
				}
				else
				{
					timeBlue = mSystem.currentTimeMillis() + 1500;
					IsAddDust1 = true;
					IsAddDust2 = true;
				}
			}

	public void paintEff_Lvup_behind(mGraphics g)
			{
				if (idEff_Set_Item != -1)
				{
					if (fraEff != null)
					{
						fraEff.drawFrame(GameCanvas.gameTick / 4 % fraEff.nFrame, cx, cy + 3, (cdir != 1) ? 2 : 0, mGraphics.BOTTOM | mGraphics.HCENTER, g);
					}
					else
					{
						fraEff = mSystem.getFraImage(strEff_Set_Item + idEff_Set_Item + "_0");
					}
				}
			}

	public void paintEff_Lvup_front(mGraphics g)
			{
				if (idEff_Set_Item != -1)
				{
					if (fraEffSub != null)
					{
						fraEffSub.drawFrame(GameCanvas.gameTick / 4 % fraEffSub.nFrame, cx, cy + 8, (cdir != 1) ? 2 : 0, mGraphics.BOTTOM | mGraphics.HCENTER, g);
					}
					else
					{
						fraEffSub = mSystem.getFraImage(strEff_Set_Item + idEff_Set_Item + "_1");
					}
				}
			}

}
