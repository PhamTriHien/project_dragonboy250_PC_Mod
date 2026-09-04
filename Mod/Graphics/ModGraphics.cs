using System;

public static class ModGraphics
{
	// 0: Ultra (mặc định), 1: Medium (xóa hiệu ứng động), 2: Low (xóa bg, phông trắng xanh nhạt), 3: Super Low (xóa cây cỏ trang trí, chỉ chừa base & NPC)
	public static int graphicsQuality = 0;
	public static readonly string[] graphicsNames = new string[4] { "Ultra", "Medium", "Low", "Super Low" };

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
