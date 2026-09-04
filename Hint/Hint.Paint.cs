using System;

public partial class Hint
{
	public static bool isPaintz()
		{
			if (isOnTask(0, 3) && GameCanvas.panel.currentTabIndex == 0 && (GameCanvas.panel.cmy < 0 || GameCanvas.panel.cmy > 30))
			{
				return false;
			}
			if (isOnTask(2, 0) && GameCanvas.panel.isShow && GameCanvas.panel.currentTabIndex != 0)
			{
				return false;
			}
			return true;
		}

	public static void paintArrowPointToHint(mGraphics g)
		{
			try
			{
				if (!isPaintArrow || (x > GameScr.cmx && x < GameScr.cmx + GameScr.gW && y > GameScr.cmy && y < GameScr.cmy + GameScr.gH) || GameCanvas.gameTick % 10 < 5 || ChatPopup.currChatPopup != null || ChatPopup.serverChatPopUp != null || GameCanvas.panel.isShow || !isCamera)
				{
					return;
				}
				int num = x - Char.myCharz().cx;
				int num2 = y - Char.myCharz().cy;
				int num3 = 0;
				int num4 = 0;
				int arg = 0;
				if (num > 0 && num2 >= 0)
				{
					if (Res.abs(num) >= Res.abs(num2))
					{
						num3 = GameScr.gW - 10;
						num4 = GameScr.gH / 2 + 30;
						if (GameCanvas.isTouch)
						{
							num4 = GameScr.gH / 2 + 10;
						}
						arg = 0;
					}
					else
					{
						num3 = GameScr.gW / 2;
						num4 = GameScr.gH - 10;
						arg = 5;
					}
				}
				else if (num >= 0 && num2 < 0)
				{
					if (Res.abs(num) >= Res.abs(num2))
					{
						num3 = GameScr.gW - 10;
						num4 = GameScr.gH / 2 + 30;
						if (GameCanvas.isTouch)
						{
							num4 = GameScr.gH / 2 + 10;
						}
						arg = 0;
					}
					else
					{
						num3 = GameScr.gW / 2;
						num4 = 10;
						arg = 6;
					}
				}
				if (num < 0 && num2 >= 0)
				{
					if (Res.abs(num) >= Res.abs(num2))
					{
						num3 = 10;
						num4 = GameScr.gH / 2 + 30;
						if (GameCanvas.isTouch)
						{
							num4 = GameScr.gH / 2 + 10;
						}
						arg = 3;
					}
					else
					{
						num3 = GameScr.gW / 2;
						num4 = GameScr.gH - 10;
						arg = 5;
					}
				}
				else if (num <= 0 && num2 < 0)
				{
					if (Res.abs(num) >= Res.abs(num2))
					{
						num3 = 10;
						num4 = GameScr.gH / 2 + 30;
						if (GameCanvas.isTouch)
						{
							num4 = GameScr.gH / 2 + 10;
						}
						arg = 3;
					}
					else
					{
						num3 = GameScr.gW / 2;
						num4 = 10;
						arg = 6;
					}
				}
				GameScr.resetTranslate(g);
				g.drawRegion(GameScr.arrow, 0, 0, 13, 16, arg, num3, num4, StaticObj.VCENTER_HCENTER);
			}
			catch (Exception)
			{
			}
		}

	public static void paint(mGraphics g)
		{
			if (ChatPopup.serverChatPopUp != null || Char.myCharz().isUsePlane || Char.myCharz().isTeleport)
			{
				return;
			}
			paintArrowPointToHint(g);
			if (GameCanvas.menu.tDelay == 0 && isPaint && ChatPopup.scr == null && !Char.ischangingMap && GameCanvas.currentScreen == GameScr.gI() && (!GameCanvas.panel.isShow || GameCanvas.panel.cmx == 0))
			{
				if (isCamera)
				{
					g.translate(-GameScr.cmx, -GameScr.cmy);
				}
				if (trans == 0)
				{
					g.drawImage(Panel.imgBantay, x - 15, y, 0);
				}
				if (trans == 1)
				{
					g.drawRegion(Panel.imgBantay, 0, 0, 14, 16, 2, x + 15, y, StaticObj.TOP_RIGHT);
				}
				if (paintFlare)
				{
					g.drawImage(ItemMap.imageFlare, x, y, 3);
				}
			}
		}

}
