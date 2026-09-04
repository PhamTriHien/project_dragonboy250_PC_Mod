using System;
using Assets.src.g;
public partial class GameScr : mScreen, IChatable
{
	public void paintPhuBanBar(mGraphics g, int x, int y, int w)
		{
			if (phuban_Info == null || isPaintOther || isPaintRada != 1 || GameCanvas.panel.isShow || !ispaintPhubangBar())
			{
				return;
			}
			if (w < fra_PVE_Bar_1.frameWidth + fra_PVE_Bar_0.frameWidth * 4)
			{
				w = fra_PVE_Bar_1.frameWidth + fra_PVE_Bar_0.frameWidth * 4;
			}
			if (x > GameCanvas.w - w / 2)
			{
				x = GameCanvas.w - w / 2;
			}
			if (x < mGraphics.getImageWidth(imgKhung) + w / 2 + 10)
			{
				x = mGraphics.getImageWidth(imgKhung) + w / 2 + 10;
			}
			int frameHeight = fra_PVE_Bar_0.frameHeight;
			int num = y + frameHeight + mGraphics.getImageHeight(imgBall) / 2 + 2;
			int frameWidth = fra_PVE_Bar_1.frameWidth;
			int num2 = w / 2 - frameWidth / 2;
			int num3 = x - w / 2;
			int num4 = x + frameWidth / 2;
			int y2 = y + 3;
			int num5 = num2 - fra_PVE_Bar_0.frameWidth;
			int num6 = num5 / fra_PVE_Bar_0.frameWidth;
			if (num5 % fra_PVE_Bar_0.frameWidth > 0)
			{
				num6++;
			}
			for (int i = 0; i < num6; i++)
			{
				if (i < num6 - 1)
				{
					fra_PVE_Bar_0.drawFrame(1, num3 + fra_PVE_Bar_0.frameWidth + i * fra_PVE_Bar_0.frameWidth, y2, 0, 0, g);
				}
				else
				{
					fra_PVE_Bar_0.drawFrame(1, num3 + num5, y2, 0, 0, g);
				}
				if (i < num6 - 1)
				{
					fra_PVE_Bar_0.drawFrame(1, num4 + i * fra_PVE_Bar_0.frameWidth, y2, 0, 0, g);
				}
				else
				{
					fra_PVE_Bar_0.drawFrame(1, num4 + num5 - fra_PVE_Bar_0.frameWidth, y2, 0, 0, g);
				}
			}
			fra_PVE_Bar_0.drawFrame(0, num3, y2, 2, 0, g);
			fra_PVE_Bar_0.drawFrame(0, num4 + num5, y2, 0, 0, g);
			if (phuban_Info.pointTeam1 > 0)
			{
				int idx = 2;
				int idx2 = 3;
				if (phuban_Info.color_1 == 4)
				{
					idx = 4;
					idx2 = 5;
				}
				int num7 = phuban_Info.pointTeam1 * num2 / phuban_Info.maxPoint;
				if (num7 < 0)
				{
					num7 = 0;
				}
				if (num7 > num2)
				{
					num7 = num2;
				}
				g.setClip(num3 + num2 - num7, y2, num7, frameHeight);
				for (int j = 0; j < num6; j++)
				{
					if (j < num6 - 1)
					{
						fra_PVE_Bar_0.drawFrame(idx2, num3 + fra_PVE_Bar_0.frameWidth + j * fra_PVE_Bar_0.frameWidth, y2, 0, 0, g);
					}
					else
					{
						fra_PVE_Bar_0.drawFrame(idx2, num3 + num5, y2, 0, 0, g);
					}
				}
				fra_PVE_Bar_0.drawFrame(idx, num3, y2, 2, 0, g);
				GameCanvas.resetTrans(g);
			}
			if (phuban_Info.pointTeam2 > 0)
			{
				int idx3 = 2;
				int idx4 = 3;
				if (phuban_Info.color_2 == 4)
				{
					idx3 = 4;
					idx4 = 5;
				}
				int num8 = phuban_Info.pointTeam2 * num2 / phuban_Info.maxPoint;
				if (num8 < 0)
				{
					num8 = 0;
				}
				if (num8 > num2)
				{
					num8 = num2;
				}
				g.setClip(num4, y2, num8, frameHeight);
				for (int k = 0; k < num6; k++)
				{
					if (k < num6 - 1)
					{
						fra_PVE_Bar_0.drawFrame(idx4, num4 + k * fra_PVE_Bar_0.frameWidth, y2, 0, 0, g);
					}
					else
					{
						fra_PVE_Bar_0.drawFrame(idx4, num4 + num5 - fra_PVE_Bar_0.frameWidth, y2, 0, 0, g);
					}
				}
				fra_PVE_Bar_0.drawFrame(idx3, num4 + num5, y2, 0, 0, g);
				GameCanvas.resetTrans(g);
			}
			fra_PVE_Bar_1.drawFrame(0, x - frameWidth / 2, y, 0, 0, g);
			string timeCountDown = mSystem.getTimeCountDown(phuban_Info.timeStart, phuban_Info.timeSecond, isOnlySecond: true, isShortText: false);
			mFont.tahoma_7b_yellow.drawString(g, timeCountDown, x + 1, y + fra_PVE_Bar_1.frameHeight / 2 - mFont.tahoma_7b_green2.getHeight() / 2, 2);
			Panel.setTextColor(phuban_Info.color_1, 1).drawString(g, phuban_Info.nameTeam1, x - 5, num + 5, 1);
			Panel.setTextColor(phuban_Info.color_2, 1).drawString(g, phuban_Info.nameTeam2, x + 5, num + 5, 0);
			if (phuban_Info.type_PB != 0)
			{
				int y3 = y + frameHeight / 2 - 2;
				mFont.bigNumber_While.drawString(g, string.Empty + phuban_Info.pointTeam1, num3 + num2 / 2, y3, 2);
				mFont.bigNumber_While.drawString(g, string.Empty + phuban_Info.pointTeam2, num4 + num2 / 2, y3, 2);
			}
			g.drawImage(imgVS, x, y + fra_PVE_Bar_1.frameHeight + 2, 3);
			if (phuban_Info.type_PB == 0)
			{
				paintChienTruong_Life(g, phuban_Info.maxLife, phuban_Info.color_1, phuban_Info.lifeTeam1, x - 13, phuban_Info.color_2, phuban_Info.lifeTeam2, x + 13, num);
			}
		}
	public static void paintChienTruong_Life(mGraphics g, int maxLife, int cl1, int lifeTeam1, int x1, int cl2, int lifeTeam2, int x2, int y)
		{
			if (imgBall == null)
			{
				return;
			}
			int num = mGraphics.getImageHeight(imgBall) / 2;
			for (int i = 0; i < maxLife; i++)
			{
				int num2 = 0;
				if (i < lifeTeam1)
				{
					num2 = 1;
				}
				g.drawRegion(imgBall, 0, num2 * num, mGraphics.getImageWidth(imgBall), num, 0, x1 - i * (num + 1), y, mGraphics.VCENTER | mGraphics.HCENTER);
			}
			for (int j = 0; j < maxLife; j++)
			{
				int num3 = 0;
				if (j < lifeTeam2)
				{
					num3 = 1;
				}
				g.drawRegion(imgBall, 0, num3 * num, mGraphics.getImageWidth(imgBall), num, 0, x2 + j * (num + 1), y, mGraphics.VCENTER | mGraphics.HCENTER);
			}
		}
	private void paint_board_CT(mGraphics g, int x, int y)
		{
			if (!is_Paint_boardCT_Expand)
			{
				string s = "#01 nnnnnnnnnnnn";
				int width = mFont.tahoma_7.getWidth(s);
				int num = GameCanvas.w - width - 20;
				for (int i = 0; i < nTop; i++)
				{
					mFont mFont2 = mFont.tahoma_7_white;
					switch (i)
					{
					case 0:
						mFont2 = mFont.tahoma_7_red;
						break;
					case 1:
						mFont2 = mFont.tahoma_7_yellow;
						break;
					case 2:
						mFont2 = mFont.tahoma_7_blue;
						break;
					}
					if (i == nTop - 1)
					{
						mFont2 = mFont.tahoma_7_green;
					}
					string[] array = Res.split((string)res_CT.elementAt(i), "|", 0);
					int[] array2 = new int[2] { 0, 18 };
					for (int j = 0; j < 2; j++)
					{
						mFont2.drawString(g, array[j], num + array2[j], y + i * mFont.tahoma_7.getHeight(), 0, mFont.tahoma_7);
					}
				}
				GameCanvas.resetTrans(g);
				xRect = num;
				yRect = y;
				wRect = width + 10;
				hRect = mFont.tahoma_7b_dark.getHeight() * 6;
			}
			else
			{
				string s2 = "#01 namec1000000 0001   00000";
				int[] array3 = new int[4] { 0, 18, 80, 101 };
				int width2 = mFont.tahoma_7.getWidth(s2);
				int num2 = GameCanvas.w - width2 - 20;
				int num3 = y;
				for (int k = 0; k < nTop; k++)
				{
					string[] array4 = Res.split((string)res_CT.elementAt(k), "|", 0);
					mFont mFont3 = mFont.tahoma_7_white;
					switch (k)
					{
					case 0:
						mFont3 = mFont.tahoma_7_red;
						break;
					case 1:
						mFont3 = mFont.tahoma_7_yellow;
						break;
					case 2:
						mFont3 = mFont.tahoma_7_blue;
						break;
					}
					if (k == nTop - 1)
					{
						mFont3 = mFont.tahoma_7_green;
					}
					num3 = k * mFont.tahoma_7_white.getHeight() + y;
					for (int l = 0; l < array3.Length; l++)
					{
						mFont3.drawString(g, array4[l], num2 + array3[l], num3, 0, mFont.tahoma_7);
					}
				}
				xRect = num2;
				yRect = y;
				wRect = width2 + 10;
				hRect = mFont.tahoma_7b_dark.getHeight() * 6;
			}
			GameCanvas.resetTrans(g);
		}
	private void paintHPCT(mGraphics g, int x, int y, Char c)
		{
			g.drawImage(imgKhung, x, y, 0);
			int x2 = x + 3;
			int num = y + 19;
			int num2 = 0;
			int num3 = 0;
			int width = imgHP_NEW.getWidth();
			int num4 = imgHP_NEW.getHeight() / 2;
			num2 = (int)(c.cHP * width / c.cHPFull);
			if (num2 <= 0)
			{
				num2 = 1;
			}
			else if (num2 > width)
			{
				num2 = width;
			}
			g.drawRegion(imgHP_NEW, 0, num4, 80, num4, 0, x2, num, 0);
			num3 = (int)(c.cMP * width / c.cMPFull);
			if (num3 <= 0)
			{
				num3 = 1;
			}
			else if (num3 > width)
			{
				num3 = width;
			}
			g.drawRegion(imgHP_NEW, 0, 0, 80, num4, 0, x2, num + 6, 0);
		}

}
