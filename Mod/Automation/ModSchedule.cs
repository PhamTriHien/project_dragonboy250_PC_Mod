using System;
using System.Collections.Generic;

/// <summary>
/// Module Lập Lịch Treo Game: Quản lý thời gian tự động On/Off (ngắt kết nối / kết nối lại) và lịch hẹn giờ di chuyển 6 khung giờ (Map + Khu + Quái Tàn Sát).
/// </summary>
public static class ModSchedule
{
	public static bool isEnabled;
	public static int startMinute = 0;
	public static int endMinute = 0;
	public static bool disconnectOnOffWindow = false;
	public static bool isOffWindowDisconnected = false;
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
				hasEvaluatedWindow = false;
				nextCheck = 0;
				ModConfig.SaveConfig();
				GameScr.info1?.addInfo("Đã lưu Lịch Slot #" + (editingSlotIndex + 1) + ": " + (string.IsNullOrEmpty(trainSlotsRaw[editingSlotIndex]) ? "(Trống)" : trainSlotsRaw[editingSlotIndex]), 0);
			}
		}
		public void onCancelChat()
		{
			ChatTextField.gI().isShow = false;
			ChatTextField.gI().parentScreen = GameScr.gI();
		}
	}

	private class GlobalWindowChatListener : IChatable
	{
		public void onChatFromMe(string text, string to)
		{
			ChatTextField.gI().isShow = false;
			ChatTextField.gI().parentScreen = GameScr.gI();
			string trimmed = (text ?? string.Empty).Trim();
			if (Configure(trimmed))
			{
				GameScr.info1?.addInfo("Đã đặt khung giờ On/Off: " + GetWindowSummaryLabel(), 0);
			}
			else
			{
				GameScr.info1?.addInfo("Sai định dạng! Hãy nhập HH:mm-HH:mm (VD: 06:30-23:00)", 0);
			}
		}
		public void onCancelChat()
		{
			ChatTextField.gI().isShow = false;
			ChatTextField.gI().parentScreen = GameScr.gI();
		}
	}

	private static readonly TrainSlotChatListener slotChatListener = new TrainSlotChatListener();
	private static readonly GlobalWindowChatListener globalWindowListener = new GlobalWindowChatListener();

	public static string GetWindowSummaryLabel()
	{
		if (startMinute == endMinute) return "Cả ngày";
		int sh = startMinute / 60;
		int sm = startMinute % 60;
		int eh = endMinute / 60;
		int em = endMinute % 60;
		return sh.ToString("D2") + ":" + sm.ToString("D2") + "-" + eh.ToString("D2") + ":" + em.ToString("D2");
	}

	public static void ShowInputGlobalWindow()
	{
		ModUI.uiCustomOpen = false;
		ModMenu.modMenuOpen = false;
		ChatTextField.gI().strChat = "Nhập khung giờ On/Off (HH:mm-HH:mm)";
		ChatTextField.gI().tfChat.name = "VD: 06:30-23:00 (00:00-00:00 = Cả ngày)";
		ChatTextField.gI().tfChat.setIputType(TField.INPUT_TYPE_ANY);
		ChatTextField.gI().startChat2(globalWindowListener, string.Empty);
		string cur = (startMinute == endMinute) ? "00:00-00:00" : GetWindowSummaryLabel();
		ChatTextField.gI().tfChat.setText(cur);
	}

	public static void ShowInputTrainSlot(int slotIndex)
	{
		if (slotIndex < 0 || slotIndex >= trainSlotsRaw.Length) slotIndex = 0;
		editingSlotIndex = slotIndex;
		ModUI.uiCustomOpen = false;
		ModMenu.modMenuOpen = false;
		ChatTextField.gI().strChat = "Slot #" + (slotIndex + 1) + " (HH:mm-HH:mm|MapID|Khu|MobIDs)";
		ChatTextField.gI().tfChat.name = "VD: 08:00-12:00|29|2|13,14";
		ChatTextField.gI().tfChat.setIputType(TField.INPUT_TYPE_ANY);
		ChatTextField.gI().startChat2(slotChatListener, string.Empty);
		ChatTextField.gI().tfChat.setText(trainSlotsRaw[slotIndex] ?? string.Empty);
	}

	public static bool Configure(string value)
	{
		string[] parts = (value ?? string.Empty).Split('-');
		int start, end;
		if (parts.Length != 2 || !TryMinute(parts[0], out start) || !TryMinute(parts[1], out end)) return false;
		ApplyPreset(start, end);
		return true;
	}

	public static void ApplyPreset(int start, int end)
	{
		startMinute = start;
		endMinute = end;
		isEnabled = true;
		hasEvaluatedWindow = false;
		isOffWindowDisconnected = false;
		nextCheck = 0;
		ModConfig.SaveConfig();
	}

	private static bool TryMinute(string text, out int minute)
	{
		minute = 0;
		string[] p = (text ?? string.Empty).Trim().Split(':');
		int hour, min;
		if (p.Length != 2 || !int.TryParse(p[0], out hour) || !int.TryParse(p[1], out min) || hour < 0 || hour > 23 || min < 0 || min > 59) return false;
		minute = hour * 60 + min;
		return true;
	}

	private static bool TryParseTrainSlot(string raw, out int startMin, out int endMin, out int mapId, out int zoneId, out List<int> mobIds)
	{
		startMin = 0;
		endMin = 0;
		mapId = -1;
		zoneId = -1;
		mobIds = null;
		if (string.IsNullOrEmpty(raw)) return false;
		string[] segs = raw.Split('|');
		if (segs.Length < 2) return false;
		string[] times = segs[0].Split('-');
		if (times.Length != 2 || !TryMinute(times[0], out startMin) || !TryMinute(times[1], out endMin)) return false;
		if (!int.TryParse(segs[1].Trim(), out mapId) || mapId < 0) return false;
		if (segs.Length >= 3 && int.TryParse(segs[2].Trim(), out int z)) zoneId = z;
		if (segs.Length >= 4 && !string.IsNullOrEmpty(segs[3].Trim()))
		{
			string[] mp = segs[3].Split(new char[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < mp.Length; i++)
			{
				if (int.TryParse(mp[i].Trim(), out int mid) && mid >= 0)
				{
					if (mobIds == null) mobIds = new List<int>();
					if (!mobIds.Contains(mid)) mobIds.Add(mid);
				}
			}
		}
		return true;
	}

	public static void Toggle()
	{
		isEnabled = !isEnabled;
		hasEvaluatedWindow = false;
		isOffWindowDisconnected = false;
		activeSlotIndex = -1;
		nextCheck = 0;
		ModConfig.SaveConfig();
		GameScr.info1?.addInfo("Lịch Auto: " + (isEnabled ? "BẬT" : "TẮT"), 0);
	}

	private static void HandleOffWindowDisconnect()
	{
		if (!disconnectOnOffWindow || isOffWindowDisconnected) return;
		isOffWindowDisconnected = true;
		if (ModMenu.IsInGame())
		{
			ModAutoLogin.SnapshotAutoState();
			ModAutoLogin.wasTanSatActive = true;
			ModTanSat.autoTanSat = false;
			ModAutoLogin.isReconnecting = false;
			GameScr.info1?.addInfo("Hết giờ treo máy: Ngắt kết nối nghỉ theo lịch!", 0);
			Session_ME.gI().close();
		}
	}

	private static void HandleOnWindowReconnect()
	{
		if (!isOffWindowDisconnected) return;
		isOffWindowDisconnected = false;
		if (!ModMenu.IsInGame() && ModAutoLogin.isEnabled)
		{
			ModAutoLogin.TriggerScheduledReconnect();
		}
	}

	public static void Update()
	{
		if (!isEnabled)
		{
			isOffWindowDisconnected = false;
			return;
		}
		long now = mSystem.currentTimeMillis();
		if (now < nextCheck) return;
		nextCheck = now + 1000;
		int current = DateTime.Now.Hour * 60 + DateTime.Now.Minute;

		// 1. Kiểm tra 6 khung giờ hẹn map/khu/quái chuyên biệt (Tàn Sát Pro)
		bool anyCustomSlotConfigured = false;
		int matchedSlot = -1;
		int targetMap = -1;
		int targetZone = -1;
		List<int> targetMobIds = null;

		for (int i = 0; i < trainSlotsRaw.Length; i++)
		{
			if (TryParseTrainSlot(trainSlotsRaw[i], out int sMin, out int eMin, out int mId, out int zId, out List<int> mList))
			{
				anyCustomSlotConfigured = true;
				bool inSlot = (sMin == eMin) ? true : (sMin < eMin ? (current >= sMin && current < eMin) : (current >= sMin || current < eMin));
				if (inSlot)
				{
					matchedSlot = i;
					targetMap = mId;
					targetZone = zId;
					targetMobIds = mList;
					break;
				}
			}
		}

		if (anyCustomSlotConfigured)
		{
			if (matchedSlot >= 0)
			{
				HandleOnWindowReconnect();
				if (ModMenu.IsInGame() && !Char.isLoadingMap && !Char.ischangingMap)
				{
					if (activeSlotIndex != matchedSlot)
					{
						if (targetMobIds != null && targetMobIds.Count > 0)
						{
							ModTanSatFilter.selectAllMobs = false;
							ModTanSatFilter.selectedMobTemplateId = -1;
							ModTanSatFilter.tickedMobTemplateIds.Clear();
							ModTanSatFilter.tickedMobTemplateIds.AddRange(targetMobIds);
						}
						else
						{
							ModTanSatFilter.selectAllMobs = true;
							ModTanSatFilter.tickedMobTemplateIds.Clear();
						}
					}
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
			}
			else
			{
				if (activeSlotIndex != -1)
				{
					activeSlotIndex = -1;
					ModTanSat.autoTanSat = false;
					GameScr.info1?.addInfo("Đã hết khung giờ hẹn lịch Train! Tạm dừng Tàn Sát.", 0);
				}
				HandleOffWindowDisconnect();
			}
			return;
		}

		// 2. Chế độ khung giờ tổng quát (Cả ngày / 08h-18h / 22h-06h / Tùy chỉnh)
		bool inWindow = startMinute == endMinute ? true : (startMinute < endMinute ? current >= startMinute && current < endMinute : current >= startMinute || current < endMinute);
		if (!inWindow)
		{
			if (!hasEvaluatedWindow || lastWindow)
			{
				hasEvaluatedWindow = true;
				lastWindow = false;
				ModTanSat.autoTanSat = false;
				ModAutoPick.autoPick = false;
				ModAutoItem.isEnabled = false;
				GameScr.info1?.addInfo("Lịch Auto: ĐÃ TẮT (Ngoài khung giờ)", 0);
			}
			HandleOffWindowDisconnect();
			return;
		}

		HandleOnWindowReconnect();
		if (hasEvaluatedWindow && lastWindow) return;
		hasEvaluatedWindow = true;
		lastWindow = true;
		if (ModMenu.IsInGame())
		{
			ModTanSat.autoTanSat = true;
			ModAutoPick.autoPick = true;
			ModAutoItem.isEnabled = ModAutoItem.itemIds.Count > 0;
			GameScr.info1?.addInfo("Lịch Auto: ĐÃ BẬT", 0);
		}
	}
}
