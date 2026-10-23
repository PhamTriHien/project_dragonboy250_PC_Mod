using System;

/// <summary>
/// Module Lập Lịch Treo Game: Quản lý thời gian tự động bật/tắt Auto và lịch hẹn giờ di chuyển 6 khung giờ (Map + Khu + Tàn Sát).
/// </summary>
public static class ModSchedule
{
	public static bool isEnabled;
	public static int startMinute = 0;
	public static int endMinute = 0;
	public static readonly string[] trainSlotsRaw = new string[6] { "", "", "", "", "", "" };

	private static long nextCheck;
	private static bool lastWindow;
	private static bool hasEvaluatedWindow;
	private static int activeSlotIndex = -1;
	private static int editingSlotIndex = 0;
	private static long lastSlotZoneRequestTime = 0;

	private class TrainSlotChatListener : IChatable
	{
		public void onChatFromMe(string text, string to)
		{
			ChatTextField.gI().isShow = false;
			ChatTextField.gI().parentScreen = GameScr.gI();
			if (editingSlotIndex >= 0 && editingSlotIndex < trainSlotsRaw.Length)
			{
				trainSlotsRaw[editingSlotIndex] = (text ?? string.Empty).Trim();
				activeSlotIndex = -1;
				ModConfig.SaveConfig();
				GameScr.info1.addInfo("Đã lưu Lịch Hẹn Giờ #" + (editingSlotIndex + 1) + ": " + (string.IsNullOrEmpty(trainSlotsRaw[editingSlotIndex]) ? "(Trống)" : trainSlotsRaw[editingSlotIndex]), 0);
			}
		}
		public void onCancelChat()
		{
			ChatTextField.gI().isShow = false;
			ChatTextField.gI().parentScreen = GameScr.gI();
		}
	}

	private static readonly TrainSlotChatListener slotChatListener = new TrainSlotChatListener();

	public static void ShowInputTrainSlot(int slotIndex)
	{
		if (slotIndex < 0 || slotIndex >= trainSlotsRaw.Length) slotIndex = 0;
		editingSlotIndex = slotIndex;
		ChatTextField.gI().strChat = "Lịch #" + (slotIndex + 1) + " (HH:mm-HH:mm|MapID|Khu)";
		ChatTextField.gI().tfChat.name = "VD: 08:00-12:00|29|2";
		ChatTextField.gI().tfChat.setIputType(TField.INPUT_TYPE_ANY);
		ChatTextField.gI().startChat2(slotChatListener, string.Empty);
		ChatTextField.gI().tfChat.setText(trainSlotsRaw[slotIndex] ?? string.Empty);
	}

	/// <summary>
	/// Cấu hình tham số và thiết lập hoạt động cho module.
	/// </summary>
	public static bool Configure(string value)
	{
		string[] parts = (value ?? string.Empty).Split('-');
		int start, end;
		if (parts.Length != 2 || !TryMinute(parts[0], out start) || !TryMinute(parts[1], out end)) return false;
		ApplyPreset(start, end);
		return true;
	}

	/// <summary>
	/// Thiết lập mốc phút bắt đầu và kết thúc cho lịch tự động và buộc kiểm tra lại ngay lập tức.
	/// </summary>
	public static void ApplyPreset(int start, int end)
	{
		startMinute = start;
		endMinute = end;
		isEnabled = true;
		hasEvaluatedWindow = false;
		nextCheck = 0;
		ModConfig.SaveConfig();
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ TryMinute.
	/// </summary>
	private static bool TryMinute(string text, out int minute)
	{
		minute = 0;
		string[] p = (text ?? string.Empty).Trim().Split(':');
		int hour, min;
		if (p.Length != 2 || !int.TryParse(p[0], out hour) || !int.TryParse(p[1], out min) || hour < 0 || hour > 23 || min < 0 || min > 59) return false;
		minute = hour * 60 + min;
		return true;
	}

	private static bool TryParseTrainSlot(string raw, out int startMin, out int endMin, out int mapId, out int zoneId)
	{
		startMin = 0;
		endMin = 0;
		mapId = -1;
		zoneId = -1;
		if (string.IsNullOrEmpty(raw)) return false;
		string[] segs = raw.Split('|');
		if (segs.Length < 2) return false;
		string[] times = segs[0].Split('-');
		if (times.Length != 2 || !TryMinute(times[0], out startMin) || !TryMinute(times[1], out endMin)) return false;
		if (!int.TryParse(segs[1].Trim(), out mapId) || mapId < 0) return false;
		if (segs.Length >= 3 && int.TryParse(segs[2].Trim(), out int z)) zoneId = z;
		return true;
	}

	/// <summary>
	/// Bật hoặc tắt trạng thái kích hoạt của Toggle.
	/// </summary>
	public static void Toggle()
	{
		isEnabled = !isEnabled;
		hasEvaluatedWindow = false;
		activeSlotIndex = -1;
		nextCheck = 0;
		ModConfig.SaveConfig();
		GameScr.info1.addInfo("Lịch Auto: " + (isEnabled ? "BẬT" : "TẮT"), 0);
	}

	/// <summary>
	/// Cập nhật trạng thái logic của đối tượng theo từng chu kỳ khung hình.
	/// </summary>
	public static void Update()
	{
		if (!isEnabled) return;
		long now = mSystem.currentTimeMillis();
		if (now < nextCheck) return;
		nextCheck = now + 1000;
		int current = DateTime.Now.Hour * 60 + DateTime.Now.Minute;

		// 1. Kiểm tra 6 khung giờ hẹn map/khu chuyên biệt (nếu có cấu hình)
		bool anyCustomSlotConfigured = false;
		int matchedSlot = -1;
		int targetMap = -1;
		int targetZone = -1;

		for (int i = 0; i < trainSlotsRaw.Length; i++)
		{
			if (TryParseTrainSlot(trainSlotsRaw[i], out int sMin, out int eMin, out int mId, out int zId))
			{
				anyCustomSlotConfigured = true;
				bool inSlot = (sMin == eMin) ? true : (sMin < eMin ? (current >= sMin && current < eMin) : (current >= sMin || current < eMin));
				if (inSlot)
				{
					matchedSlot = i;
					targetMap = mId;
					targetZone = zId;
					break;
				}
			}
		}

		if (anyCustomSlotConfigured)
		{
			if (matchedSlot >= 0 && ModMenu.IsInGame() && !Char.isLoadingMap && !Char.ischangingMap)
			{
				if (TileMap.mapID != targetMap && !ModNextMap.isNextMapActive)
				{
					ModNextMap.StartNextMap(targetMap);
				}
				else if (TileMap.mapID == targetMap)
				{
					if (targetZone >= 0 && TileMap.zoneID != targetZone && now - lastSlotZoneRequestTime > 3000)
					{
						lastSlotZoneRequestTime = now;
						Service.gI().requestChangeZone(targetZone, -1);
					}
					if (!ModTanSat.autoTanSat)
					{
						ModTanSat.autoTanSat = true;
						ModAutoPick.autoPick = true;
					}
				}
				activeSlotIndex = matchedSlot;
			}
			else if (matchedSlot == -1 && activeSlotIndex != -1)
			{
				activeSlotIndex = -1;
				ModTanSat.autoTanSat = false;
				GameScr.info1.addInfo("Đã hết khung giờ hẹn lịch Train! Tạm dừng Tàn Sát.", 0);
			}
			return;
		}

		// 2. Chế độ khung giờ tổng quát (Cả ngày / 08h-18h / 22h-06h)
		bool inWindow = startMinute == endMinute ? true : (startMinute < endMinute ? current >= startMinute && current < endMinute : current >= startMinute || current < endMinute);
		if (hasEvaluatedWindow && inWindow == lastWindow) return;
		hasEvaluatedWindow = true;
		lastWindow = inWindow;
		ModTanSat.autoTanSat = inWindow;
		ModAutoPick.autoPick = inWindow;
		ModAutoItem.isEnabled = inWindow && ModAutoItem.itemIds.Count > 0;
		GameScr.info1.addInfo("Lịch Auto: " + (inWindow ? "ĐÃ BẬT" : "ĐÃ TẮT"), 0);
	}
}
