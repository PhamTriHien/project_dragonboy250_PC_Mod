using System;
using UnityEngine;

public static class ModLogo
{
	public static bool isShowLogoInGame = true;
	public static int logoPosition = 1; // 0: Góc phải trên (Top-Right), 1: Giữa trên sát mép (Top-Center)

	private static Image inGameLogo = null;
	private static bool isLoaded = false;

	public static void LoadLogo()
	{
		try
		{
			if (inGameLogo != null)
			{
				return;
			}
			inGameLogo = GameCanvas.loadCustomImage("custom_logo.png");
			isLoaded = (inGameLogo != null);
		}
		catch
		{
		}
	}

	public static Image GetLogo()
	{
		if (inGameLogo == null && !isLoaded)
		{
			LoadLogo();
		}
		return inGameLogo;
	}

	public static void GetBounds(out int x, out int y, out int w, out int h)
	{
		Image img = GetLogo();
		w = (img != null) ? mGraphics.getImageWidth(img) : 140;
		h = (img != null) ? mGraphics.getImageHeight(img) : 76;

		if (logoPosition == 0)
		{
			// Góc phải trên (Top-Right)
			x = GameCanvas.w - w - 8;
			y = 0;
		}
		else
		{
			// Giữa game, sát phía trên cùng màn hình (Top-Center)
			x = GameCanvas.hw - w / 2;
			y = 0;
		}
	}

	public static bool IsPointerInsideLogo(int px, int py)
	{
		if (!isShowLogoInGame || !ModMenu.IsInGame())
		{
			return false;
		}
		GetBounds(out int lx, out int ly, out int lw, out int lh);
		return (px >= lx && px <= lx + lw && py >= ly && py <= ly + lh);
	}

	public static void Paint(mGraphics g)
	{
		try
		{
			if (!isShowLogoInGame || !ModMenu.IsInGame() || g == null)
			{
				return;
			}

			// Tự động ẩn khi đang mở Panel, Menu, Hộp thoại hoặc Giao diện Mod
			if ((GameCanvas.panel != null && GameCanvas.panel.isShow) ||
			    (GameCanvas.panel2 != null && GameCanvas.panel2.isShow) ||
			    (GameCanvas.menu != null && GameCanvas.menu.showMenu) ||
			    GameCanvas.currentDialog != null ||
			    ModUI.uiCustomOpen)
			{
				return;
			}

			Image img = GetLogo();
			if (img == null)
			{
				return;
			}

			g.translate(-g.getTranslateX(), -g.getTranslateY());
			g.setClip(0, 0, GameCanvas.w, GameCanvas.h);

			GetBounds(out int drawX, out int drawY, out int drawW, out int drawH);
			g.drawImage(img, drawX, drawY, mGraphics.TOP | mGraphics.LEFT);
		}
		catch
		{
		}
	}
}
