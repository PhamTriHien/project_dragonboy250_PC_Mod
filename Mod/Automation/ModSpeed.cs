using System;

public static class ModSpeed
{
	public static bool speedHack = false;
	public static float speedMult = 2f;
	public static int originalSpeed = -1;
	public static int lastCharId = -1;

	public static void SetSpeedHack(bool enabled)
	{
		speedHack = enabled;
		ModConfig.SaveConfig();
	}

	public static void SetSpeedMult(float mult)
	{
		speedMult = mult;
		ModConfig.SaveConfig();
	}
}
