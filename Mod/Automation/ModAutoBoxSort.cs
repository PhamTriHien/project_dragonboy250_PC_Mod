using System;

/// <summary>
/// Module Tự Động Sắp Xếp Rương Đồ: Sắp xếp gọn gàng vật phẩm trong rương và hành trang theo chủng loại.
/// </summary>
public static class ModAutoBoxSort
{
	public static bool isEnabled;
	public static int intervalMs = 60000;
	private static long nextRunAt;

	/// <summary>
	/// Thực thi phương thức nghiệp vụ SortNow.
	/// </summary>
	public static void SortNow()
	{
		if (!ModMenu.IsInGame() || !Session_ME.gI().isConnected()) return;
		Service.gI().boxSort();
		int clamped = intervalMs < 15000 ? 15000 : (intervalMs > 3600000 ? 3600000 : intervalMs);
		nextRunAt = mSystem.currentTimeMillis() + clamped;
	}

	/// <summary>
	/// Bật hoặc tắt trạng thái kích hoạt của Toggle.
	/// </summary>
	public static void Toggle()
	{
		isEnabled = !isEnabled;
		nextRunAt = 0;
		ModConfig.SaveConfig();
		GameScr.info1?.addInfo("Auto Sắp Xếp Rương: " + (isEnabled ? "BẬT" : "TẮT"), 0);
	}

	/// <summary>
	/// Cập nhật trạng thái logic của đối tượng theo từng chu kỳ khung hình.
	/// </summary>
	public static void Update()
	{
		if (!isEnabled) return;
		long now = mSystem.currentTimeMillis();
		if (nextRunAt != 0 && now < nextRunAt) return;
		SortNow();
	}
}
