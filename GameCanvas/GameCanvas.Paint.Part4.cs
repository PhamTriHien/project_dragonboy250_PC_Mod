using System;
using Assets.src.e;
using Assets.src.g;
using UnityEngine;
public partial class GameCanvas : IActionListener
{
	public void paintChangeMap(mGraphics g)
		{
			string empty = string.Empty;
			resetTrans(g);
			g.setColor(0);
			g.fillRect(0, 0, w, h);
			g.drawImage(LoginScr.imgTitle, w / 2, h / 2 - 24, StaticObj.BOTTOM_HCENTER);
			paintShukiren(hw, h / 2 + 24, g);
			mFont.tahoma_7b_white.drawString(g, mResources.PLEASEWAIT + ((LoginScr.timeLogin <= 0) ? empty : (" " + LoginScr.timeLogin + "s")), w / 2, h / 2, 2);
		}
	public void paint(mGraphics gx)
		{
			try
			{
				debugPaint.removeAllElements();
				debug("PA", 1);
				if (currentScreen != null)
				{
					currentScreen.paint(g);
				}
				debug("PB", 1);
				g.translate(-g.getTranslateX(), -g.getTranslateY());
				g.setClip(0, 0, w, h);
				if (panel.isShow)
				{
					panel.paint(g);
					if (panel2 != null && panel2.isShow)
					{
						panel2.paint(g);
					}
					if (panel.chatTField != null && panel.chatTField.isShow)
					{
						panel.chatTField.paint(g);
					}
					if (panel2 != null && panel2.chatTField != null && panel2.chatTField.isShow)
					{
						panel2.chatTField.paint(g);
					}
				}
				Res.paintOnScreenDebug(g);
				InfoDlg.paint(g);
				if (currentDialog != null)
				{
					debug("PC", 1);
					currentDialog.paint(g);
				}
				else if (menu.showMenu)
				{
					debug("PD", 1);
					resetTrans(g);
					menu.paintMenu(g);
				}
				GameScr.info1.paint(g);
				GameScr.info2.paint(g);
				if (GameScr.gI().popUpYesNo != null)
				{
					GameScr.gI().popUpYesNo.paint(g);
				}
				if (ChatPopup.currChatPopup != null)
				{
					ChatPopup.currChatPopup.paint(g);
				}
				Hint.paint(g);
				if (ChatPopup.serverChatPopUp != null)
				{
					ChatPopup.serverChatPopUp.paint(g);
				}
				for (int i = 0; i < Effect2.vEffect2.size(); i++)
				{
					Effect2 effect = (Effect2)Effect2.vEffect2.elementAt(i);
					if (effect is ChatPopup && !effect.Equals(ChatPopup.currChatPopup) && !effect.Equals(ChatPopup.serverChatPopUp))
					{
						effect.paint(g);
					}
				}
				if (currentDialog != null)
				{
					currentDialog.paint(g);
				}
				if (isWait())
				{
					paintChangeMap(g);
					if (timeLoading > 0 && LoginScr.timeLogin <= 0 && mSystem.currentTimeMillis() - TIMEOUT >= 1000)
					{
						timeLoading--;
						if (timeLoading == 0)
						{
							timeLoading = 15;
						}
						TIMEOUT = mSystem.currentTimeMillis();
					}
				}
				debug("PE", 1);
				resetTrans(g);
				EffecMn.paintLayer4(g);
				if (open3Hour && !isLoading)
				{
					if (currentScreen == loginScr || currentScreen == serverScreen || currentScreen == serverScr)
					{
						g.drawImage(img18, 5, 5, 0);
					}
					if (currentScreen == CreateCharScr.instance)
					{
						g.drawImage(img18, hw, 5, 0);
					}
				}
				resetTrans(g);
				int num = h / 4;
				if (currentScreen != null && currentScreen is GameScr && thongBaoTest != null)
				{
					g.setClip(60, num, w - 120, mFont.tahoma_7_white.getHeight() + 2);
					mFont.tahoma_7_grey.drawString(g, thongBaoTest, xThongBaoTranslate, num + 1, 0);
					mFont.tahoma_7_yellow.drawString(g, thongBaoTest, xThongBaoTranslate, num, 0);
					g.setClip(0, 0, w, h);
				}
				resetTrans(g);
				ModMenu.Paint(g);
			}
			catch (Exception)
			{
			}
		}
	public void paintDust(mGraphics g)
		{
			if (lowGraphic || ModMenu.graphicsQuality >= 1)
			{
				return;
			}
			for (int i = 0; i < 2; i++)
			{
				if (dustState[i] != -1 && isPaint(dustX[i], dustY[i]))
				{
					g.drawImage(imgDust[i][dustState[i]], dustX[i], dustY[i], 3);
				}
			}
		}
	public static void paintShukiren(int x, int y, mGraphics g)
		{
			g.drawRegion(imgShuriken, 0, Main.f * 16, 16, 16, 0, x, y, mGraphics.HCENTER | mGraphics.VCENTER);
		}

}
