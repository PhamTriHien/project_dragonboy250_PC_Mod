using System;

public partial class NewBoss
{
	private void paintShadow(mGraphics g)
		{
			int num = TileMap.size;
			if ((TileMap.mapID < 114 || TileMap.mapID > 120) && TileMap.mapID != 127 && TileMap.mapID != 128)
			{
				if (TileMap.tileTypeAt(xSd + num / 2, ySd + 1, 4))
				{
					g.setClip(xSd / num * num, (ySd - 30) / num * num, num, 100);
				}
				else if (TileMap.tileTypeAt((xSd - num / 2) / num, (ySd + 1) / num) == 0)
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
			g.drawImage(shadowBig, xSd, ySd - 5, 3);
			g.setClip(GameScr.cmx, GameScr.cmy - GameCanvas.transY, GameScr.gW, GameScr.gH + 2 * GameCanvas.transY);
		}

	public new bool isPaint()
		{
			if (x < GameScr.cmx)
			{
				return false;
			}
			if (x > GameScr.cmx + GameScr.gW)
			{
				return false;
			}
			if (y < GameScr.cmy)
			{
				return false;
			}
			if (y > GameScr.cmy + GameScr.gH + 30)
			{
				return false;
			}
			if (status == 0)
			{
				return false;
			}
			return true;
		}

	public override void paint(mGraphics g)
		{
			if (Mob.arrMobTemplate[templateId].data == null || isHide)
			{
				return;
			}
			if (isMafuba)
			{
				if (!changBody)
				{
					Mob.arrMobTemplate[templateId].data.paintFrame(g, frame, xMFB, yMFB, (dir != 1) ? 1 : 0, 2);
				}
				else
				{
					SmallImage.drawSmallImage(g, smallBody, xMFB, yMFB, (dir != 1) ? 2 : 0, mGraphics.BOTTOM | mGraphics.HCENTER);
				}
				return;
			}
			if (isShadown)
			{
				paintShadow(g);
			}
			g.translate(0, GameCanvas.transY);
			if (!changBody)
			{
				int num = 33;
				if (yTemp == -1)
				{
					yTemp = y;
				}
				if (TileMap.tileTypeAt(x + num, y + fy, 4))
				{
					xTempLeft = TileMap.tileXofPixel(x + num) - num;
					xTempRight = TileMap.tileXofPixel(x + num);
					if (x > xTempLeft && x < xTempRight && xTempRight != -1)
					{
						x = xTempLeft;
					}
				}
				if (y < yTemp && yTemp != -1)
				{
					yTemp = y;
					x += num;
				}
				if (y > yTemp)
				{
					yTemp = y;
					x -= num;
				}
				Mob.arrMobTemplate[templateId].data.paintFrame(g, frame, x, y + fy, (dir != 1) ? 1 : 0, 2);
			}
			else
			{
				SmallImage.drawSmallImage(g, smallBody, x, y + fy - 9, (dir != 1) ? 2 : 0, mGraphics.BOTTOM | mGraphics.HCENTER);
			}
			g.translate(0, -GameCanvas.transY);
			if (hp <= 0)
			{
				return;
			}
			int imageWidth = mGraphics.getImageWidth(imgHPtem);
			int imageHeight = mGraphics.getImageHeight(imgHPtem);
			int num2 = imageWidth;
			int num3 = imageWidth;
			int num4 = x - imageWidth;
			int num5 = y - h - 5;
			int num6 = imageWidth * 2 * per / 100;
			int num7 = num6;
			if (per_tem >= per)
			{
				num7 = imageWidth * (per_tem -= ((GameCanvas.gameTick % 6 <= 3) ? offset : offset++)) / 100;
				if (per_tem <= 0)
				{
					per_tem = 0;
				}
				if (per_tem < per)
				{
					per_tem = per;
				}
				if (offset >= 3)
				{
					offset = 3;
				}
			}
			if (num6 > num2)
			{
				num3 = num6 - num2;
				if (num3 <= 0)
				{
					num3 = 0;
				}
			}
			else
			{
				num2 = num6;
				num3 = 0;
			}
			g.drawImage(GameScr.imgHP_tm_xam, num4, num5, mGraphics.TOP | mGraphics.LEFT);
			g.drawImage(GameScr.imgHP_tm_xam, num4 + imageWidth, num5, mGraphics.TOP | mGraphics.LEFT);
			g.setColor(16777215);
			g.fillRect(num4, num5, num7, 2);
			g.drawRegion(imgHPtem, 0, 0, num2, imageHeight, 0, num4, num5, mGraphics.TOP | mGraphics.LEFT);
			g.drawRegion(imgHPtem, 0, 0, num3, imageHeight, 0, num4 + imageWidth, num5, mGraphics.TOP | mGraphics.LEFT);
		}

}
