using System;
using UnityEngine;

public static class ModFps
{
	public static int targetFps = 144;
	public static bool isAutoFps = true;
	public static int pingMs = 25;

	private static GUIStyle pingFpsStyle;
	private static GUIStyle pingFpsStyleShadow;

	public static int GetDeviceMaxRefreshRate()
	{
		try
		{
			int rate = Screen.currentResolution.refreshRate;
			if (rate <= 0)
			{
				rate = 60;
			}
			return rate;
		}
		catch
		{
			return 60;
		}
	}

	public static int GetBestFpsForDevice()
	{
		int max = GetDeviceMaxRefreshRate();
		if (max >= 240) return 240;
		if (max >= 185) return 185;
		if (max >= 165) return 165;
		if (max >= 144) return 144;
		if (max >= 120) return 120;
		if (max >= 90) return 90;
		return 60;
	}

	public static void LoadFPS()
	{
		try
		{
			if (isAutoFps)
			{
				targetFps = GetBestFpsForDevice();
			}
			ApplyFPS();
		}
		catch
		{
			targetFps = 144;
			ApplyFPS();
		}
	}

	public static void ApplyFPS()
	{
		try
		{
			QualitySettings.vSyncCount = 0;
			Application.targetFrameRate = targetFps;
		}
		catch
		{
		}
	}

	public static void SetAutoFPS()
	{
		isAutoFps = true;
		targetFps = GetBestFpsForDevice();
		ApplyFPS();
		ModConfig.SaveConfig();
	}

	public static void SetFPS(int fps)
	{
		isAutoFps = false;
		targetFps = fps;
		ApplyFPS();
		ModConfig.SaveConfig();
	}

	public static void CycleFPS()
	{
		int[] steps = new int[8] { 30, 60, 90, 120, 144, 165, 185, 240 };
		int idx = Array.IndexOf(steps, targetFps);
		if (idx == -1)
		{
			targetFps = 144;
			isAutoFps = false;
		}
		else if (idx >= steps.Length - 1)
		{
			SetAutoFPS();
			return;
		}
		else
		{
			isAutoFps = false;
			targetFps = steps[idx + 1];
		}
		ApplyFPS();
		ModConfig.SaveConfig();
	}

	public static string GetFpsCaption()
	{
		if (isAutoFps)
		{
			return "FPS: " + targetFps + " (Auto " + GetDeviceMaxRefreshRate() + "Hz)";
		}
		return "FPS: " + targetFps;
	}

	public static void PaintFPS(mGraphics g)
	{
		try
		{
			if (g == null)
			{
				return;
			}

			// Tự động ẩn FPS khi mở Hành trang (Panel), Menu, Hộp thoại hoặc giao diện Mod
			if ((GameCanvas.panel != null && GameCanvas.panel.isShow) ||
			    (GameCanvas.panel2 != null && GameCanvas.panel2.isShow) ||
			    (GameCanvas.menu != null && GameCanvas.menu.showMenu) ||
			    GameCanvas.currentDialog != null ||
			    ModUI.uiCustomOpen)
			{
				return;
			}

			g.translate(-g.getTranslateX(), -g.getTranslateY());
			g.setClip(0, 0, GameCanvas.w, GameCanvas.h);

			int curFps = (int)(1f / Time.unscaledDeltaTime);
			if (curFps > 999) curFps = 999;
			if (curFps < 0) curFps = 0;

			int drawX;
			int drawY;
			if (ModMenu.IsInGame())
			{
				drawX = 84;
				drawY = (Char.myCharz() != null && Char.myCharz().secondPower > 0) ? 43 : 28;
			}
			else
			{
				drawX = 8;
				drawY = 6;
			}

			string text = curFps + "fps - " + pingMs + "ms";
			if (mFont.tahoma_7_grey != null)
			{
				mFont.tahoma_7_grey.drawString(g, text, drawX + 1, drawY + 1, mFont.LEFT);
			}
			if (mFont.tahoma_7_green != null)
			{
				mFont.tahoma_7_green.drawString(g, text, drawX, drawY, mFont.LEFT);
			}
			else if (mFont.tahoma_7_white != null)
			{
				mFont.tahoma_7_white.drawString(g, text, drawX, drawY, mFont.LEFT);
			}
		}
		catch
		{
		}
	}
}
