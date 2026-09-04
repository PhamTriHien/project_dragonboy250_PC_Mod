using System;
using Assets.src.g;

public partial class GameScr : mScreen, IChatable
{
	public static void loadCamera(bool fullmScreen, int cx, int cy)
		{
			gW = GameCanvas.w;
			cmdBarH = 39;
			gH = GameCanvas.h;
			cmdBarW = gW;
			cmdBarX = 0;
			cmdBarY = GameCanvas.h - Paint.hTab - cmdBarH;
			girlHPBarY = 0;
			csPadMaxH = GameCanvas.h / 6;
			if (csPadMaxH < 48)
			{
				csPadMaxH = 48;
			}
			gW2 = gW >> 1;
			gH2 = gH >> 1;
			gW3 = gW / 3;
			gH3 = gH / 3;
			gW23 = gH - 120;
			gH23 = gH * 2 / 3;
			gW34 = 3 * gW / 4;
			gH34 = 3 * gH / 4;
			gW6 = gW / 6;
			gH6 = gH / 6;
			gssw = gW / TileMap.size + 2;
			gssh = gH / TileMap.size + 2;
			if (gW % 24 != 0)
			{
				gssw++;
			}
			cmxLim = (TileMap.tmw - 1) * TileMap.size - gW;
			cmyLim = (TileMap.tmh - 1) * TileMap.size - gH;
			if (cx == -1 && cy == -1)
			{
				cmx = (cmtoX = Char.myCharz().cx - gW2 + gW6 * Char.myCharz().cdir);
				cmy = (cmtoY = Char.myCharz().cy - gH23);
			}
			else
			{
				cmx = (cmtoX = cx - gW23 + gW6 * Char.myCharz().cdir);
				cmy = (cmtoY = cy - gH23);
			}
			firstY = cmy;
			if (cmx < 24)
			{
				cmx = (cmtoX = 24);
			}
			if (cmx > cmxLim)
			{
				cmx = (cmtoX = cmxLim);
			}
			if (cmy < 0)
			{
				cmy = (cmtoY = 0);
			}
			if (cmy > cmyLim)
			{
				cmy = (cmtoY = cmyLim);
			}
			gssx = cmx / TileMap.size - 1;
			if (gssx < 0)
			{
				gssx = 0;
			}
			gssy = cmy / TileMap.size;
			gssxe = gssx + gssw;
			gssye = gssy + gssh;
			if (gssy < 0)
			{
				gssy = 0;
			}
			if (gssye > TileMap.tmh - 1)
			{
				gssye = TileMap.tmh - 1;
			}
			TileMap.countx = (gssxe - gssx) * 4;
			if (TileMap.countx > TileMap.tmw)
			{
				TileMap.countx = TileMap.tmw;
			}
			TileMap.county = (gssye - gssy) * 4;
			if (TileMap.county > TileMap.tmh)
			{
				TileMap.county = TileMap.tmh;
			}
			TileMap.gssx = (Char.myCharz().cx - 2 * gW) / TileMap.size;
			if (TileMap.gssx < 0)
			{
				TileMap.gssx = 0;
			}
			TileMap.gssxe = TileMap.gssx + TileMap.countx;
			if (TileMap.gssxe > TileMap.tmw)
			{
				TileMap.gssxe = TileMap.tmw;
			}
			TileMap.gssy = (Char.myCharz().cy - 2 * gH) / TileMap.size;
			if (TileMap.gssy < 0)
			{
				TileMap.gssy = 0;
			}
			TileMap.gssye = TileMap.gssy + TileMap.county;
			if (TileMap.gssye > TileMap.tmh)
			{
				TileMap.gssye = TileMap.tmh;
			}
			ChatTextField.gI().parentScreen = instance;
			ChatTextField.gI().tfChat.y = GameCanvas.h - 35 - ChatTextField.gI().tfChat.height;
			ChatTextField.gI().initChatTextField();
			if (GameCanvas.isTouch)
			{
				yTouchBar = gH - 88;
				xC = gW - 40;
				yC = 2;
				if (GameCanvas.w <= 240)
				{
					xC = gW - 35;
					yC = 5;
				}
				xF = gW - 55;
				yF = yTouchBar + 35;
				xTG = gW - 37;
				yTG = yTouchBar - 1;
				if (GameCanvas.w >= 450)
				{
					yTG -= 12;
					yHP -= 7;
					xF -= 10;
					yF -= 5;
					xTG -= 10;
				}
			}
			setSkillBarPosition();
			disXC = ((GameCanvas.w <= 200) ? 30 : 40);
			if (Rms.loadRMSInt("viewchat") == -1)
			{
				GameCanvas.panel.isViewChatServer = true;
			}
			else
			{
				GameCanvas.panel.isViewChatServer = Rms.loadRMSInt("viewchat") == 1;
			}
		}

	public static void updateCamera()
		{
			if (isPaintOther)
			{
				return;
			}
			if (cmx != cmtoX || cmy != cmtoY)
			{
				cmvx = cmtoX - cmx << 2;
				cmvy = cmtoY - cmy << 2;
				cmdx += cmvx;
				cmx += cmdx >> 4;
				cmdx &= 15;
				cmdy += cmvy;
				cmy += cmdy >> 4;
				cmdy &= 15;
				if (cmx < 24)
				{
					cmx = 24;
				}
				if (cmx > cmxLim)
				{
					cmx = cmxLim;
				}
				if (cmy < 0)
				{
					cmy = 0;
				}
				if (cmy > cmyLim)
				{
					cmy = cmyLim;
				}
			}
			gssx = cmx / TileMap.size - 1;
			if (gssx < 0)
			{
				gssx = 0;
			}
			gssy = cmy / TileMap.size;
			gssxe = gssx + gssw;
			gssye = gssy + gssh;
			if (gssy < 0)
			{
				gssy = 0;
			}
			if (gssye > TileMap.tmh - 1)
			{
				gssye = TileMap.tmh - 1;
			}
			TileMap.gssx = (Char.myCharz().cx - 2 * gW) / TileMap.size;
			if (TileMap.gssx < 0)
			{
				TileMap.gssx = 0;
			}
			TileMap.gssxe = TileMap.gssx + TileMap.countx;
			if (TileMap.gssxe > TileMap.tmw)
			{
				TileMap.gssxe = TileMap.tmw;
				TileMap.gssx = TileMap.gssxe - TileMap.countx;
			}
			TileMap.gssy = (Char.myCharz().cy - 2 * gH) / TileMap.size;
			if (TileMap.gssy < 0)
			{
				TileMap.gssy = 0;
			}
			TileMap.gssye = TileMap.gssy + TileMap.county;
			if (TileMap.gssye > TileMap.tmh)
			{
				TileMap.gssye = TileMap.tmh;
				TileMap.gssy = TileMap.gssye - TileMap.county;
			}
			scrMain.updatecm();
			scrInfo.updatecm();
		}

	public static void resetTranslate(mGraphics g)
		{
			g.translate(-g.getTranslateX(), -g.getTranslateY());
			g.setClip(0, -200, GameCanvas.w, 200 + GameCanvas.h);
		}

}
