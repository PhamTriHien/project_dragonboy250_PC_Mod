using System;
using UnityEngine;

public partial class LoginScr : mScreen, IActionListener
{
	public override void paint(mGraphics g)
		{
			GameCanvas.debug("PLG1", 1);
			GameCanvas.paintBGGameScr(g);
			GameCanvas.debug("PLG2", 2);
			int num = tfUser.y - 50;
			if (GameCanvas.h <= 220)
			{
				num += 5;
			}
			mFont.tahoma_7_white.drawString(g, "v" + GameMidlet.VERSION, GameCanvas.w - 2, 17, 1, mFont.tahoma_7_grey);
			if (mSystem.clientType == 1 && !GameCanvas.isTouch)
			{
				mFont.tahoma_7_white.drawString(g, ServerListScreen.linkweb, GameCanvas.w - 2, GameCanvas.h - 15, 1, mFont.tahoma_7_grey);
			}
			else
			{
				mFont.tahoma_7_white.drawString(g, ServerListScreen.linkweb, GameCanvas.w - 2, 2, 1, mFont.tahoma_7_grey);
			}
			if (GameCanvas.currentDialog == null)
			{
				int h = 105;
				int w = ((GameCanvas.w < 200) ? 160 : 180);
				PopUp.paintPopUp(g, xLog, yLog - 10, w, h, -1, isButton: true);
				if (GameCanvas.h > 160 && imgTitle != null)
				{
					g.drawImage(imgTitle, GameCanvas.hw, num, 3);
				}
				GameCanvas.debug("PLG4", 1);
				int num2 = 4;
				int num3 = num2 * 32 + 23 + 33;
				if (num3 >= GameCanvas.w)
				{
					num2--;
					num3 = num2 * 32 + 23 + 33;
				}
				xLog = GameCanvas.w / 2 - num3 / 2;
				tfUser.x = xLog + 10;
				tfUser.y = yLog + 20;
				tfPass.x = xLog + 10;
				tfPass.y = yLog + 55;
				tfUser.paint(g);
				tfPass.paint(g);
				int num4 = 0;
				if (GameCanvas.w >= 176)
				{
					num4 = 50;
				}
				else
				{
					mFont.tahoma_7b_green2.drawString(g, mResources.acc + ":", tfUser.x - 35, tfUser.y + 7, 0);
					mFont.tahoma_7b_green2.drawString(g, mResources.pwd + ":", tfPass.x - 35, tfPass.y + 7, 0);
					mFont.tahoma_7b_green2.drawString(g, mResources.server + ":" + serverName, GameCanvas.w / 2, tfPass.y + 32, 2);
					num4 = 0;
				}
			}
			base.paint(g);
			cmdBack.paint(g);
		}

}
