using System;

public partial class ChatPopup
{
	public override void paint(mGraphics g)
		{
			if (GameScr.gI().activeRongThan && GameScr.gI().isUseFreez)
			{
				return;
			}
			GameCanvas.resetTrans(g);
			int num = cx;
			int num2 = cy;
			int num3 = sayWidth + 2;
			int num4 = ch;
			if ((num <= 0 || num2 <= 0) && !GameCanvas.panel.isShow)
			{
				return;
			}
			int num5 = 0;
			if (c != null)
			{
				num5 = ((GameCanvas.gameTick % 10 <= 2) ? 1 : 0);
				SmallImage.drawSmallImage(g, c.avatar, cx + 14, cy + num5, 0, StaticObj.BOTTOM_LEFT);
			}
			if (iconID != 0)
			{
				num5 = ((GameCanvas.gameTick % 10 <= 2) ? 1 : 0);
				SmallImage.drawSmallImage(g, iconID, cx + num3 / 2, cy + ch - 15 + num5, 0, StaticObj.VCENTER_HCENTER);
			}
			PopUp.paintPopUp(g, num, num2, num3, num4, 16777215, isButton: false);
			if (scr != null)
			{
				g.setClip(num, num2, num3, num4 - 16);
				g.translate(0, -scr.cmy);
			}
			int tx = 0;
			int ty = 0;
			if (isClip)
			{
				tx = g.getTranslateX();
				ty = g.getTranslateY();
				g.setClip(num, num2 + 1, num3, num4 - 17);
				g.translate(0, -cmyText);
			}
			int num6 = -1;
			for (int i = 0; i < says.Length; i++)
			{
				if (says[i].StartsWith("--"))
				{
					g.setColor(0);
					g.fillRect(num + 10, cy + sayRun + i * 12 + 6, num3 - 20, 1);
					continue;
				}
				mFont mFont2 = mFont.tahoma_7;
				int num7 = 2;
				string st = says[i];
				int num8 = 0;
				if (says[i].StartsWith("|"))
				{
					string[] array = Res.split(says[i], "|", 0);
					if (array.Length == 3)
					{
						st = array[2];
					}
					if (array.Length == 4)
					{
						st = array[3];
						num7 = int.Parse(array[2]);
					}
					num8 = int.Parse(array[1]);
					num6 = num8;
				}
				else
				{
					num8 = num6;
				}
				switch (num8)
				{
				case -1:
					mFont2 = mFont.tahoma_7;
					break;
				case 0:
					mFont2 = mFont.tahoma_7b_dark;
					break;
				case 1:
					mFont2 = mFont.tahoma_7b_green;
					break;
				case 2:
					mFont2 = mFont.tahoma_7b_blue;
					break;
				case 3:
					mFont2 = mFont.tahoma_7_red;
					break;
				case 4:
					mFont2 = mFont.tahoma_7_green;
					break;
				case 5:
					mFont2 = mFont.tahoma_7_blue;
					break;
				case 7:
					mFont2 = mFont.tahoma_7b_red;
					break;
				case 8:
					mFont2 = mFont.tahoma_7b_yellow;
					break;
				}
				if (says[i].StartsWith("<"))
				{
					string[] array2 = Res.split(says[i], "<", 0);
					string[] array3 = Res.split(array2[1], ">", 1);
					if (second == 0)
					{
						second = int.Parse(array3[1]);
					}
					else
					{
						curr = mSystem.currentTimeMillis();
						if (curr - last >= 1000)
						{
							last = curr;
							second--;
						}
					}
					st = second + " " + array3[2];
					mFont2.drawString(g, st, cx + sayWidth / 2, cy + sayRun + i * 12 - strY + 12, num7);
				}
				else
				{
					if (num7 == 2)
					{
						mFont2.drawString(g, st, cx + sayWidth / 2, cy + sayRun + i * 12 - strY + 12, num7);
					}
					if (num7 == 1)
					{
						mFont2.drawString(g, st, cx + sayWidth - 5, cy + sayRun + i * 12 - strY + 12, num7);
					}
				}
			}
			if (isClip)
			{
				GameCanvas.resetTrans(g);
				g.translate(tx, ty);
			}
			if (maxStarSlot > 4)
			{
				nMaxslot_tren = (maxStarSlot + 1) / 2;
				nMaxslot_duoi = maxStarSlot - nMaxslot_tren;
				int[] array4 = new int[maxStarSlot];
				int[] array5 = new int[maxStarSlot];
				for (int j = 0; j < nMaxslot_tren; j++)
				{
					g.drawImage(Panel.imgMaxStar, num + num3 / 2 - nMaxslot_tren * 20 / 2 + j * 20 + mGraphics.getImageWidth(Panel.imgMaxStar), num2 + num4 - 17, 3);
					array4[j] = num + num3 / 2 - nMaxslot_tren * 20 / 2 + j * 20 + mGraphics.getImageWidth(Panel.imgMaxStar);
					array5[j] = num2 + num4 - 17;
				}
				for (int k = 0; k < nMaxslot_duoi; k++)
				{
					g.drawImage(Panel.imgMaxStar, num + num3 / 2 - nMaxslot_duoi * 20 / 2 + k * 20 + mGraphics.getImageWidth(Panel.imgMaxStar), num2 + num4 - 8, 3);
					array4[nMaxslot_tren + k] = num + num3 / 2 - nMaxslot_duoi * 20 / 2 + k * 20 + mGraphics.getImageWidth(Panel.imgMaxStar);
					array5[nMaxslot_tren + k] = num2 + num4 - 8;
				}
				if (maxStarSlot >= 7)
				{
					int num9 = 7;
					for (int l = 7; l < maxStarSlot; l++)
					{
						if (starCuongHoa[l])
						{
							g.drawImage(Panel.imgStarCuongHoa, array4[l], array5[l], 3);
						}
					}
				}
				if (starSlot > 0)
				{
					imgStar = Panel.imgStar;
					if (starSlot >= nMaxslot_tren)
					{
						nslot_duoi = starSlot - nMaxslot_tren;
						for (int m = 0; m < nMaxslot_tren; m++)
						{
							g.drawImage(imgStar, num + num3 / 2 - nMaxslot_tren * 20 / 2 + m * 20 + mGraphics.getImageWidth(imgStar), num2 + num4 - 17, 3);
						}
						for (int n = 0; n < nslot_duoi; n++)
						{
							if (n + nMaxslot_tren >= numSlot)
							{
								imgStar = Panel.imgStar8;
							}
							g.drawImage(imgStar, num + num3 / 2 - nMaxslot_duoi * 20 / 2 + n * 20 + mGraphics.getImageWidth(imgStar), num2 + num4 - 8, 3);
						}
					}
					else
					{
						for (int num10 = 0; num10 < starSlot; num10++)
						{
							g.drawImage(imgStar, num + num3 / 2 - nMaxslot_tren * 20 / 2 + num10 * 20 + mGraphics.getImageWidth(imgStar), num2 + num4 - 17, 3);
						}
					}
				}
			}
			else
			{
				for (int num11 = 0; num11 < maxStarSlot; num11++)
				{
					g.drawImage(Panel.imgMaxStar, num + num3 / 2 - maxStarSlot * 20 / 2 + num11 * 20 + mGraphics.getImageWidth(Panel.imgMaxStar), num2 + num4 - 13, 3);
				}
				if (starSlot > 0)
				{
					for (int num12 = 0; num12 < starSlot; num12++)
					{
						g.drawImage(Panel.imgStar, num + num3 / 2 - maxStarSlot * 20 / 2 + num12 * 20 + mGraphics.getImageWidth(Panel.imgStar), num2 + num4 - 13, 3);
					}
				}
			}
			paintCmd(g);
		}

	public void paintRada(mGraphics g, int cmyText)
		{
			int num = cx;
			int num2 = cy;
			int num3 = sayWidth;
			int num4 = ch;
			int num5 = 0;
			int num6 = 0;
			num5 = g.getTranslateX();
			num6 = g.getTranslateY();
			g.translate(0, -cmyText);
			if ((num <= 0 || num2 <= 0) && !GameCanvas.panel.isShow)
			{
				return;
			}
			int num7 = -1;
			for (int i = 0; i < says.Length; i++)
			{
				if (says[i].StartsWith("--"))
				{
					g.setColor(16777215);
					g.fillRect(num + 10, cy + sayRun + i * 12 - 6, num3 - 20, 1);
					continue;
				}
				mFont mFont2 = mFont.tahoma_7_white;
				int num8 = 2;
				string st = says[i];
				int num9 = 0;
				if (says[i].StartsWith("|"))
				{
					string[] array = Res.split(says[i], "|", 0);
					if (array.Length == 3)
					{
						st = array[2];
					}
					if (array.Length == 4)
					{
						st = array[3];
						num8 = int.Parse(array[2]);
					}
					num9 = int.Parse(array[1]);
					num7 = num9;
				}
				else
				{
					num9 = num7;
				}
				switch (num9)
				{
				case -1:
					mFont2 = mFont.tahoma_7_white;
					break;
				case 0:
					mFont2 = mFont.tahoma_7b_white;
					break;
				case 1:
					mFont2 = mFont.tahoma_7b_green;
					break;
				case 2:
					mFont2 = mFont.tahoma_7b_red;
					break;
				}
				if (says[i].StartsWith("<"))
				{
					string[] array2 = Res.split(says[i], "<", 0);
					string[] array3 = Res.split(array2[1], ">", 1);
					if (second == 0)
					{
						second = int.Parse(array3[1]);
					}
					else
					{
						curr = mSystem.currentTimeMillis();
						if (curr - last >= 1000)
						{
							last = curr;
							second--;
						}
					}
					st = second + " " + array3[2];
					mFont2.drawString(g, st, cx + sayWidth / 2, cy + sayRun + i * 12 - strY, num8);
				}
				else
				{
					if (num8 == 2)
					{
						mFont2.drawString(g, st, cx + sayWidth / 2, cy + sayRun + i * 12 - strY, num8);
					}
					if (num8 == 1)
					{
						mFont2.drawString(g, st, cx + sayWidth - 5, cy + sayRun + i * 12 - strY, num8);
					}
				}
			}
			GameCanvas.resetTrans(g);
			g.translate(num5, num6);
		}

	public void paintCmd(mGraphics g)
		{
			g.translate(-g.getTranslateX(), -g.getTranslateY());
			g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
			GameCanvas.paintz.paintTabSoft(g);
			if (cmdNextLine != null)
			{
				GameCanvas.paintz.paintCmdBar(g, null, cmdNextLine, null);
			}
			if (cmdMsg1 != null)
			{
				GameCanvas.paintz.paintCmdBar(g, cmdMsg1, null, cmdMsg2);
			}
		}

}
