using System;
using UnityEngine;

public partial class RadarScr
{
	public override void paint(mGraphics g)
		{
			try
			{
				GameScr.gI().paint(g);
				g.translate(-GameScr.cmx, -GameScr.cmy);
				g.translate(0, GameCanvas.transY);
				GameScr.resetTranslate(g);
				g.drawImage(imgUI, xUi, yUi, 0);
				g.drawImage(imgPro_0, xUi + wUi / 2 - imgPro_0.getWidth() / 2, yUi - imgPro_0.getHeight() / 2 - 2, 0);
				g.setClip(xUi + wUi / 2 - imgPro_0.getWidth() / 2 + 13, yUi - imgPro_0.getHeight() / 2 + 3, wClip, imgPro_0.getHeight());
				g.drawImage(imgPro_1, xUi + wUi / 2 - imgPro_0.getWidth() / 2 + 13, yUi - imgPro_0.getHeight() / 2 + 3, 0);
				GameScr.resetTranslate(g);
				g.drawImage(imgChange, xCmd[0], yCmd + dxCmd[0], 0);
				g.drawImage(imgUse_0, xCmd[1], yCmd + dxCmd[1], 0);
				g.drawImage(imgBack, xCmd[2], yCmd + dxCmd[2], 0);
				if (TYPE_UI)
				{
					g.drawRegion(imgUse, 0, 0, 17, 17, 0, xCmd[1], yCmd + dxCmd[1], 0);
				}
				else
				{
					g.drawRegion(imgUse, 0, 0, 17, 17, 1, xCmd[1], yCmd + dxCmd[1], 0);
				}
				if (focus_card != null)
				{
					g.setClip(xUi + 30, yUi + 13, wUi - 60, hUi / 2);
					focus_card.paintInfo(g, xMon, yMon);
					GameScr.resetTranslate(g);
					mFont.tahoma_7b_yellow.drawString(g, ((focus_card.level <= 0) ? " " : ("Lv." + focus_card.level + " ")) + focus_card.name, xUi + wUi / 2, yUi + 15, 2);
					mFont.tahoma_7_white.drawString(g, "no." + focus_card.no, xUi + 30, yText - 2, 0);
					g.drawImage(imgBar_0, xUi + 36, yText + 10, 0);
					g.setClip(xUi + 36, yClip - hClip, 7, hClip);
					g.drawImage(imgBar_1, xUi + 36, yText + 10, 0);
					GameScr.resetTranslate(g);
					g.drawImage(imgRank[focus_card.rank], xUi + 39 - 5 + 14, yText + 12, 0);
				}
				g.setClip(xText, yText, wText + 5, hText + 8);
				if (focus_card != null)
				{
					g.drawImage(imgUIText, xText, yText, 0);
				}
				GameScr.resetTranslate(g);
				g.setClip(xText, yText + 1, wText, hText + 5);
				if (focus_card != null && focus_card.cp != null)
				{
					if (focus_card.cp.says == null)
					{
						return;
					}
					focus_card.cp.paintRada(g, cmyText);
				}
				GameScr.resetTranslate(g);
				if ((!TYPE_UI && listUse.size() > 5) || TYPE_UI)
				{
					if (page > 1)
					{
						g.drawImage(imgArrow_Left, xyArrow[0][0], xyArrow[0][1] + dxArrow[0], 0);
					}
					if (page < maxpage)
					{
						g.drawImage(imgArrow_Right, xyArrow[2][0], xyArrow[2][1] + dxArrow[1], 0);
					}
				}
				for (int i = 0; i < index.Length; i++)
				{
					int num = 0;
					int num2 = 0;
					int idx = 0;
					if (i == indexFocus)
					{
						num = dyArrow;
						num2 = -10;
						idx = 1;
						g.drawImage(imgArrow_Down, xyItem[i][0] + 10, xyItem[i][1] + dyArrow + 29 + num2, 0);
					}
					Info_RadaScr info = Info_RadaScr.GetInfo(listUse, index[i]);
					if (TYPE_UI)
					{
						info = Info_RadaScr.GetInfo(list, index[i]);
					}
					if (info != null)
					{
						fraImgFocus.drawFrame(info.rank, xyItem[i][0], xyItem[i][1] + num + num2, 0, 0, g);
						SmallImage.drawSmallImage(g, info.idIcon, xyItem[i][0] + 14, xyItem[i][1] + 14 + num + num2, 0, StaticObj.VCENTER_HCENTER);
						info.paintEff(g, xyItem[i][0], xyItem[i][1] + num + num2);
						if (info.level == 0)
						{
							g.drawImage(imgLock, xyItem[i][0], xyItem[i][1] + num + num2, 0);
						}
						if (i == indexFocus)
						{
							fraImgFocus.drawFrame(7, xyItem[i][0], xyItem[i][1] + num + num2, 0, 0, g);
						}
						if (info.isUse == 1)
						{
							fraImgFocus.drawFrame(8, xyItem[i][0], xyItem[i][1] + num + num2, 0, 0, g);
						}
					}
					else
					{
						fraImgFocusNone.drawFrame(idx, xyItem[i][0] - 1, xyItem[i][1] - 1 + num + num2, 0, 0, g);
					}
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("-pnt-radaScr-null: " + ex.ToString());
			}
		}

}
