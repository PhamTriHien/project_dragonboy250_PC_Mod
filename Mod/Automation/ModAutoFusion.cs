/// <summary>
/// Module Auto Hợp Thể: Tự động hợp thể đệ tử bằng bông tai Porata khi hết thời gian hợp thể.
/// </summary>
public static class ModAutoFusion
{
	public static bool isEnabled;
	public static int fusionType = 4;
	public static int intervalMs = 10000;
	private static long nextAt;

	/// <summary>
	/// Cấu hình tham số và thiết lập hoạt động cho module.
	/// </summary>
	public static void Configure(int type, int interval)
	{
		fusionType = type < 4 ? 4 : (type > 6 ? 6 : type);
		intervalMs = interval < 3000 ? 3000 : (interval > 3600000 ? 3600000 : interval);
		isEnabled = true; nextAt = 0; ModConfig.SaveConfig();
	}
	/// <summary>
	/// Bật hoặc tắt trạng thái kích hoạt của Toggle.
	/// </summary>
	public static void Toggle()
	{
		isEnabled = !isEnabled; nextAt = 0; ModConfig.SaveConfig();
		GameScr.info1.addInfo("Auto Fusion: " + (isEnabled ? "BẬT" : "TẮT"), 0);
	}
	/// <summary>
	/// Cập nhật trạng thái logic của đối tượng theo từng chu kỳ khung hình.
	/// </summary>
	public static void Update()
	{
		Char me = Char.myCharz();
		if (!isEnabled || !ModMenu.IsInGame() || me == null || me.isFusion || !me.havePet) return;
		if (Char.isLoadingMap || Char.ischangingMap || me.isDie || me.cHP <= 0 || me.statusMe == 14 || me.statusMe == 5) return;
		long now = mSystem.currentTimeMillis();
		if (now < nextAt) return;
		Service.gI().funsion((sbyte)fusionType);
		nextAt = now + intervalMs;
	}
}
