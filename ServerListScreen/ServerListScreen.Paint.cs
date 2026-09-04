using System;
using UnityEngine;

public partial class ServerListScreen : mScreen, IActionListener
{
	public override void paint(mGraphics g)
		{
			if (!loadScreen)
			{
				g.setColor(0);
				g.fillRect(0, 0, GameCanvas.w, GameCanvas.h);
			}
			else
			{
				GameCanvas.paintBGGameScr(g);
			}
			int num = 2;
			mFont.tahoma_7_white.drawString(g, "v" + GameMidlet.VERSION + "(" + mGraphics.zoomLevel + ")", GameCanvas.w - 2, num + 15, 1, mFont.tahoma_7_grey);
			try
			{
				string empty = string.Empty;
				string sName = (nameServer != null && ipSelect >= 0 && ipSelect < nameServer.Length) ? nameServer[ipSelect] : ("Server " + ipSelect);
				empty = ((testConnect != 0) ? (empty + sName + " connected") : (empty + sName + " disconnect"));
				if (mSystem.isTest)
				{
					mFont.tahoma_7_white.drawString(g, empty, GameCanvas.w - 2, num + 15 + 15, 1, mFont.tahoma_7_grey);
				}
			}
			catch (Exception)
			{
			}
			if (!isGetData || loadScreen)
			{
				if (mSystem.clientType == 1 && !GameCanvas.isTouch)
				{
					mFont.tahoma_7_white.drawString(g, linkweb, GameCanvas.w - 2, GameCanvas.h - 15, 1, mFont.tahoma_7_grey);
				}
				else
				{
					mFont.tahoma_7_white.drawString(g, linkweb, GameCanvas.w - 2, num, 1, mFont.tahoma_7_grey);
				}
			}
			else
			{
				mFont.tahoma_7_white.drawString(g, linkweb, GameCanvas.w - 2, num, 1, mFont.tahoma_7_grey);
			}
			int num2 = ((GameCanvas.w < 200) ? 160 : 180);
			paintDeleteData(g);
			if (!loadScreen)
			{
				if (!bigOk)
				{
					g.drawImage(LoginScr.imgTitle, GameCanvas.hw, GameCanvas.hh - 32, 3);
					if (!isGetData)
					{
						mFont.tahoma_7b_white.drawString(g, mResources.taidulieudechoi, GameCanvas.hw, GameCanvas.hh + 24, 2);
						if (cmdDownload != null)
						{
							cmdDownload.paint(g);
						}
					}
					else
					{
						if (cmdDownload != null)
						{
							cmdDownload.paint(g);
						}
						mFont.tahoma_7b_white.drawString(g, mResources.downloading_data + percent + "%", GameCanvas.w / 2, GameCanvas.hh + 24, 2);
						GameScr.paintOngMauPercent(GameScr.frBarPow20, GameScr.frBarPow21, GameScr.frBarPow22, GameCanvas.w / 2 - 50, GameCanvas.hh + 45, 100, 100f, g);
						GameScr.paintOngMauPercent(GameScr.frBarPow0, GameScr.frBarPow1, GameScr.frBarPow2, GameCanvas.w / 2 - 50, GameCanvas.hh + 45, 100, percent, g);
					}
				}
			}
			else
			{
				int num3 = GameCanvas.hh - 15 * cmd.Length - 15;
				if (num3 < 25)
				{
					num3 = 25;
				}
				if (LoginScr.imgTitle != null)
				{
					g.drawImage(LoginScr.imgTitle, GameCanvas.hw, num3, 3);
				}
				if (isNewUI)
				{
					paint_UI_New(g);
				}
				else
				{
					int num4 = cmd.Length;
					if (mGraphics.zoomLevel > 1)
					{
					}
					for (int i = 0; i < num4; i++)
					{
						cmd[i].paint(g);
					}
					g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
					if (mGraphics.zoomLevel == 1)
					{
						if (testConnect == -1)
						{
							if (GameCanvas.gameTick % 20 > 10)
							{
								g.drawRegion(GameScr.imgRoomStat, 0, 14, 7, 7, 0, (GameCanvas.w - mFont.tahoma_7b_dark.getWidth(cmd[2 + nCmdPlay].caption) >> 1) - 10, cmd[2 + nCmdPlay].y + 10, 0);
							}
						}
						else
						{
							g.drawRegion(GameScr.imgRoomStat, 0, testConnect * 7, 7, 7, 0, (GameCanvas.w - mFont.tahoma_7b_dark.getWidth(cmd[2 + nCmdPlay].caption) >> 1) - 10, cmd[2 + nCmdPlay].y + 9, 0);
						}
					}
				}
			}
			base.paint(g);
		}

	public static void paintDeleteData(mGraphics g)
		{
			if (cmdDeleteRMS != null)
			{
				mFont.tahoma_7_white.drawString(g, mResources.xoadulieu, GameCanvas.w - 2, GameCanvas.h - 15, 1, mFont.tahoma_7_grey);
			}
		}

	public void paint_UI_New(mGraphics g)
		{
			if (isNewUI)
			{
				for (int i = 0; i < cmd_New_Ui.Length; i++)
				{
					cmd_New_Ui[i].paint(g);
				}
			}
		}

}
