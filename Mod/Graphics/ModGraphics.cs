using System;
using UnityEngine;

public static class ModGraphics
{
	// 0: Ultra (mặc định), 1: Medium (xóa hiệu ứng động), 2: Low (xóa bg), 3: Super Low
	public static int graphicsQuality = 0;
	public static readonly string[] graphicsNames = new string[4] { "Ultra", "Medium", "Low", "Super Low" };

	// 0: 1024x600 (Gốc), 1: 1280x720 (HD 16:9), 2: 1600x900 (HD+), 3: 1920x1080 (Full HD)
	public static int resolutionIndex = 0;
	public static readonly int[,] resolutionList = new int[4, 2]
	{
		{ 1024, 600 },
		{ 1280, 720 },
		{ 1600, 900 },
		{ 1920, 1080 }
	};
	public static readonly string[] resolutionNames = new string[4] { "1024x600", "1280x720", "1600x900", "1920x1080" };
	public static bool isFullscreen = false;

	private static int lastScreenWidth = 0;
	private static int lastScreenHeight = 0;
	private static bool lastFullscreen = false;

	public static void InitGraphics()
	{
		try
		{
			// Kích hoạt bộ lọc nét đồ họa phần cứng GPU trong Unity, chống nhòe mờ
			QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
			QualitySettings.antiAliasing = 4;
			QualitySettings.masterTextureLimit = 0;
			QualitySettings.vSyncCount = 0;

			// Mặc định game luôn mở ở dạng cửa sổ nhỏ hiển thị giữa màn hình desktop
			isFullscreen = false;
			if (resolutionIndex < 0 || resolutionIndex >= resolutionNames.Length)
			{
				resolutionIndex = 0;
			}
			ApplyResolution(resolutionIndex);
		}
		catch
		{
		}
	}

	public static void ToggleFullscreen()
	{
		try
		{
			isFullscreen = !isFullscreen;
			if (isFullscreen)
			{
				Resolution maxRes = Screen.currentResolution;
				Screen.SetResolution(maxRes.width, maxRes.height, true);
			}
			else
			{
				int w = resolutionList[resolutionIndex, 0];
				int h = resolutionList[resolutionIndex, 1];
				Screen.SetResolution(w, h, false);
			}
			ModConfig.SaveConfig();
		}
		catch
		{
		}
	}

	public static void ApplyResolution(int index)
	{
		try
		{
			if (index < 0 || index >= resolutionNames.Length) index = 0;
			resolutionIndex = index;
			int w = resolutionList[index, 0];
			int h = resolutionList[index, 1];
			Screen.SetResolution(w, h, isFullscreen);
			ModConfig.SaveConfig();
		}
		catch
		{
		}
	}

	public static void UpdateResolutionWatcher()
	{
		try
		{
			if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight || Screen.fullScreen != lastFullscreen)
			{
				lastScreenWidth = Screen.width;
				lastScreenHeight = Screen.height;
				lastFullscreen = Screen.fullScreen;
				isFullscreen = Screen.fullScreen;

				ScaleGUI.WIDTH = Screen.width;
				ScaleGUI.HEIGHT = Screen.height;
				if (MotherCanvas.instance != null)
				{
					MotherCanvas.instance.checkZoomLevel(Screen.width, Screen.height);
				}
				if (GameMidlet.gameCanvas != null)
				{
					GameMidlet.gameCanvas.initGameCanvas();
				}
			}
		}
		catch
		{
		}
	}

	public static string GetCurrentQualityName()
	{
		if (graphicsQuality >= 0 && graphicsQuality < graphicsNames.Length)
		{
			return graphicsNames[graphicsQuality];
		}
		return graphicsNames[0];
	}

	public static void SetQuality(int quality)
	{
		if (quality >= 0 && quality < graphicsNames.Length)
		{
			graphicsQuality = quality;
			ModConfig.SaveConfig();
		}
	}

	public static void CycleQuality()
	{
		graphicsQuality = (graphicsQuality + 1) % graphicsNames.Length;
		ModConfig.SaveConfig();
	}
}
