using System;

namespace Assets.src.g;

public partial class BigBoss
{
	private void paintShadow(mGraphics g)
		{
			g.drawImage(shadowBig, xSd, yFirst, 3);
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
			if (data == null || isHide)
			{
				return;
			}
			if (isMafuba)
			{
				if (!changBody)
				{
					data.paintFrame(g, frame, xMFB, yMFB, (dir != 1) ? 1 : 0, 2);
				}
				else
				{
					SmallImage.drawSmallImage(g, smallBody, xMFB, yMFB, (dir != 1) ? 2 : 0, mGraphics.BOTTOM | mGraphics.HCENTER);
				}
				return;
			}
			if (isShadown && status != 0)
			{
				paintShadow(g);
			}
			g.translate(0, GameCanvas.transY);
			if (!changBody)
			{
				data.paintFrame(g, frame, x, y + fy, (dir != 1) ? 1 : 0, 2);
			}
			else
			{
				SmallImage.drawSmallImage(g, smallBody, x, y + fy - 9, (dir != 1) ? 2 : 0, mGraphics.BOTTOM | mGraphics.HCENTER);
			}
			g.translate(0, -GameCanvas.transY);
			int imageWidth = mGraphics.getImageWidth(imgHPtem);
			int imageHeight = mGraphics.getImageHeight(imgHPtem);
			int num = imageWidth;
			int num2 = imageWidth;
			int num3 = x - imageWidth;
			int num4 = y - h - 5;
			int num5 = imageWidth * 2 * per / 100;
			if (num5 > num)
			{
				num2 = num5 - num;
				if (num2 <= 0)
				{
					num2 = 0;
				}
			}
			else
			{
				num = num5;
				num2 = 0;
			}
			g.drawImage(GameScr.imgHP_tm_xam, num3, num4, mGraphics.TOP | mGraphics.LEFT);
			g.drawImage(GameScr.imgHP_tm_xam, num3 + imageWidth, num4, mGraphics.TOP | mGraphics.LEFT);
			g.drawRegion(imgHPtem, 0, 0, num, imageHeight, 0, num3, num4, mGraphics.TOP | mGraphics.LEFT);
			g.drawRegion(imgHPtem, 0, 0, num2, imageHeight, 0, num3 + imageWidth, num4, mGraphics.TOP | mGraphics.LEFT);
			if (shock)
			{
				Res.outz("type= " + type);
				tShock++;
				Effect me = new Effect((type != 2) ? 22 : 19, x + tShock * 50, y + 25, 2, 1, -1);
				EffecMn.addEff(me);
				Effect me2 = new Effect((type != 2) ? 22 : 19, x - tShock * 50, y + 25, 2, 1, -1);
				EffecMn.addEff(me2);
				if (tShock == 50)
				{
					tShock = 0;
					shock = false;
				}
			}
		}

}
