/// <summary>
/// Module Auto Hồi Sinh: Tự động hồi sinh nhân vật tại chỗ hoặc về nhà khi bị tiêu diệt.
/// </summary>
public static class ModAutoRevive
{
	public static bool isEnabled;
	public static int intervalMs = 10000;
	private static long nextAt;
	private static long deathDetectTime;

	/// <summary>
	/// Bật hoặc tắt trạng thái kích hoạt của Toggle.
	/// </summary>
	public static void Toggle()
	{
		isEnabled = !isEnabled;
		nextAt = 0;
		deathDetectTime = 0;
		ModConfig.SaveConfig();
		GameScr.info1.addInfo("Auto hồi sinh: " + (isEnabled ? "BẬT" : "TẮT"), 0);
	}

	/// <summary>
	/// Cập nhật trạng thái logic của đối tượng theo từng chu kỳ khung hình.
	/// </summary>
	public static void Update()
	{
		if (!isEnabled || !ModMenu.IsInGame() || Char.myCharz() == null) return;
		if (ModGoBack.isReturning || ModGoBack.isForcedHomeReturnActive ||
		    (ModGoBack.isGoBackActive && (ModGoBack.savedMapId >= 0 || ModGoBack.isAutoRecordOnDeath)) ||
		    (ModTanSat.autoTanSat && ModTanSat.antiScanMode))
		{
			return;
		}
		Char me = Char.myCharz();
		bool isDead = me.cHP <= 0 || me.statusMe == 14 || me.meDead;
		if (!isDead)
		{
			deathDetectTime = 0;
			return;
		}
		long now = mSystem.currentTimeMillis();
		if (deathDetectTime == 0)
		{
			deathDetectTime = now;
			return;
		}
		if (now - deathDetectTime < 600 || now < nextAt) return;
		if ((me.luong + me.luongKhoa) <= 0) return;
		Service.gI().wakeUpFromDead();
		nextAt = now + intervalMs;
	}
}
