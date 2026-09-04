using System;
using Assets.src.g;
public partial class GameScr : mScreen, IChatable
{
	public void paint_CT(mGraphics g, int x, int y, int w)
			{
				w = 194;
				w = 182;
				w = 170;
				int num = 66;
				int num2 = 11;
				if (x > GameCanvas.w - w / 2)
				{
					x = GameCanvas.w - w / 2;
				}
				if (x < mGraphics.getImageWidth(imgKhung) + w / 2 + 10)
				{
					x = mGraphics.getImageWidth(imgKhung) + w / 2 + 10;
				}
				int frameHeight = fra_PVE_Bar_0.frameHeight;
				int num3 = y + frameHeight + mGraphics.getImageHeight(imgBall) / 2 + 2;
				int frameWidth = fra_PVE_Bar_1.frameWidth;
				int num4 = w / 2 - frameWidth / 2;
				int num5 = x - w / 2 + 3;
				int num6 = x + frameWidth / 2;
				int num7 = y + 3;
				int num8 = num4 - fra_PVE_Bar_0.frameWidth;
				int num9 = num8 / fra_PVE_Bar_0.frameWidth;
				if (num8 % fra_PVE_Bar_0.frameWidth > 0)
				{
					num9++;
				}
				for (int i = 0; i < num9; i++)
				{
					if (i < num9 - 1)
					{
						g.drawRegion(img_ct_bar_0, 0, 15, mGraphics.getImageWidth(img_ct_bar_0), 15, 2, num5 + fra_PVE_Bar_0.frameWidth + i * fra_PVE_Bar_0.frameWidth, num7, mGraphics.TOP | mGraphics.LEFT, isClip: true);
					}
					else
					{
						g.drawRegion(img_ct_bar_0, 0, 15, mGraphics.getImageWidth(img_ct_bar_0), 15, 2, num5 + num8, num7, mGraphics.TOP | mGraphics.LEFT, isClip: true);
					}
					if (i < num9 - 1)
					{
						g.drawRegion(img_ct_bar_0, 0, 15, mGraphics.getImageWidth(img_ct_bar_0), 15, 2, num6 + i * fra_PVE_Bar_0.frameWidth, num7, mGraphics.TOP | mGraphics.LEFT, isClip: true);
					}
					else
					{
						g.drawRegion(img_ct_bar_0, 0, 15, mGraphics.getImageWidth(img_ct_bar_0), 15, 2, num6 + num8 - fra_PVE_Bar_0.frameWidth, num7, mGraphics.TOP | mGraphics.LEFT, isClip: true);
					}
				}
				fra_PVE_Bar_0.drawFrame(0, num5, num7, 2, 0, g);
				fra_PVE_Bar_0.drawFrame(0, num6 + num8, num7, 0, 0, g);
				int num10 = nCT_TeamA * 100 / (nCT_nBoyBaller / 2) * num / 100;
				if (num10 > 0)
				{
					if (num10 < 6)
					{
						num10 = 6;
					}
					g.setClip(num5, num7, num10, 15);
				}
				if (nCT_TeamA > 0)
				{
					for (int j = 0; j < num2; j++)
					{
						if (j == 0)
						{
							g.drawRegion(img_ct_bar_0, 0, 60, mGraphics.getImageWidth(img_ct_bar_0), 15, 2, num5, num7, mGraphics.TOP | mGraphics.LEFT, isClip: true);
						}
						else
						{
							g.drawRegion(img_ct_bar_0, 0, 75, mGraphics.getImageWidth(img_ct_bar_0), 15, 2, num5 + j * 6, num7, mGraphics.TOP | mGraphics.LEFT, isClip: true);
						}
					}
				}
				GameCanvas.resetTrans(g);
				int num11 = nCT_TeamB * 100 / (nCT_nBoyBaller / 2) * num / 100;
				if (num - (num - num11) > 0)
				{
					if (num11 < 6)
					{
						num11 = 6;
					}
					g.setClip(num6 + num - num11, num7, num - (num - num11), 15);
				}
				if (nCT_TeamB > 0)
				{
					for (int k = 0; k < num2; k++)
					{
						if (k == 0)
						{
							g.drawRegion(img_ct_bar_0, 0, 30, mGraphics.getImageWidth(img_ct_bar_0), 15, 0, num6 + num8, num7, mGraphics.TOP | mGraphics.LEFT, isClip: true);
						}
						else
						{
							g.drawRegion(img_ct_bar_0, 0, 45, mGraphics.getImageWidth(img_ct_bar_0), 15, 0, num6 + num8 - k * 6, num7, mGraphics.TOP | mGraphics.LEFT, isClip: true);
						}
					}
				}
				GameCanvas.resetTrans(g);
				fra_PVE_Bar_1.drawFrame(0, x - frameWidth / 2 + 1, y, 0, 0, g);
				string st = NinjaUtil.getTime((int)((nCT_timeBallte - mSystem.currentTimeMillis()) / 1000)) + string.Empty;
				mFont.tahoma_7b_yellow.drawString(g, st, num5 + w / 2 - 2, y + 5, 2);
				mFont.tahoma_7_grey.drawString(g, "Tầng " + nCT_floor, num5 + w / 2 - 3, y + fra_PVE_Bar_1.frameHeight, mFont.CENTER);
				int width = mFont.tahoma_7b_red.getWidth(nCT_TeamA + string.Empty);
				mFont.tahoma_7b_blue.drawString(g, nCT_TeamA + string.Empty, x - frameWidth / 2 - width, num7 + fra_PVE_Bar_1.frameHeight, 0);
				SmallImage.drawSmallImage(g, 2325, x - frameWidth / 2 - width - 15, num7 + fra_PVE_Bar_1.frameHeight, 2, mGraphics.TOP | mGraphics.LEFT);
				width = mFont.tahoma_7b_red.getWidth(nCT_TeamB + string.Empty);
				mFont.tahoma_7b_red.drawString(g, nCT_TeamB + string.Empty, x + frameWidth / 2, num7 + fra_PVE_Bar_1.frameHeight, 0);
				SmallImage.drawSmallImage(g, 2323, x + frameWidth / 2 + width + 3, num7 + fra_PVE_Bar_1.frameHeight, 0, mGraphics.TOP | mGraphics.LEFT);
				paint_board_CT(g, GameCanvas.w - mFont.tahoma_7b_dark.getWidth("#01 AAAAAAAAAA"), 40);
				GameCanvas.resetTrans(g);
			}

}
