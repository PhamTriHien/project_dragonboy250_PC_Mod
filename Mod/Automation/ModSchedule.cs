using System;
using System.Collections.Generic;

/// <summary>
/// Module Lập Lịch Treo Game: Quản lý khung giờ tổng quát, 6 khung giờ nghỉ On/Off độc lập và 6 khung giờ hẹn lịch Train (Map + Khu + Quái Tàn Sát).
/// </summary>
public static class ModSchedule
{
	public static bool isEnabled;
	public static int startMinute = 0;
	public static int endMinute = 0;
	public static bool disconnectOnOffWindow = false;
	public static bool isOffWindowDisconnected = false;
	public static int scheduleSlotEditTab = 0;
	public static readonly string[] trainSlotsRaw = new string[6] { "", "", "", "", "", "" };
	public static readonly string[] onOffSlotsRaw = new string[6] { "", "", "", "", "", "" };

	private static long nextCheck;
	private static bool lastWindow;
	private static bool hasEvaluatedWindow;
	private static int activeSlotIndex = -1;
	private static int editingSlotIndex = 0;
	private static int editingOnOffSlotIndex = 0;
	private static long lastSlotZoneRequestTime = 0;

	private static bool s_HasPreSlotBackup = false;
	private static bool s_BackupGoBackActive = false;
	private static int s_BackupGoBackMapId = -1;
	private static int s_BackupGoBackZoneId = -1;
	private static int s_BackupGoBackX = -1;
	private static int s_BackupGoBackY = -1;
	private static bool s_BackupSelectAllMobs = true;
	private static int s_BackupSelectedMobId = -1;
	private static readonly List<int> s_BackupTickedMobIds = new List<int>();
	private static bool s_BackupTanSatActive = false;

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
				GameScr.info1?.addInfo("Đã lưu Lịch Train #" + (editingSlotIndex + 1) + ": " + (string.IsNullOrEmpty(trainSlotsRaw[editingSlotIndex]) ? "(Trống)" : trainSlotsRaw[editingSlotIndex]), 0);
			}
		}
		public void onCancelChat()
		{
			ChatTextField.gI().isShow = false;
			ChatTextField.gI().parentScreen = GameScr.gI();
		}
	}

	private class OnOffSlotChatListener : IChatable
	{
		public void onChatFromMe(string text, string to)
		{
			ChatTextField.gI().isShow = false;
			ChatTextField.gI().parentScreen = GameScr.gI();
			if (editingOnOffSlotIndex >= 0 && editingOnOffSlotIndex < onOffSlotsRaw.Length)
			{
				string trimmed = (text ?? string.Empty).Trim();
				if (string.IsNullOrEmpty(trimmed))
				{
					onOffSlotsRaw[editingOnOffSlotIndex] = string.Empty;
					hasEvaluatedWindow = false;
					nextCheck = 0;
					ModConfig.SaveConfig();
					GameScr.info1?.addInfo("Đã xóa Slot Nghỉ #" + (editingOnOffSlotIndex + 1), 0);
				}
				else if (TryParseWindow(trimmed, out int sMin, out int eMin) && sMin != eMin)
				{
					onOffSlotsRaw[editingOnOffSlotIndex] = FormatWindow(sMin, eMin);
					isEnabled = true;
					hasEvaluatedWindow = false;
					nextCheck = 0;
					ModConfig.SaveConfig();
					GameScr.info1?.addInfo("Đã lưu Slot Nghỉ #" + (editingOnOffSlotIndex + 1) + " (Tắt-Bật): " + onOffSlotsRaw[editingOnOffSlotIndex], 0);
				}
				else
				{
					GameScr.info1?.addInfo("Sai định dạng! Nhập Giờ Tắt-Giờ Bật HH:mm-HH:mm (VD: 23:30-05:30)", 0);
				}
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
	private static readonly OnOffSlotChatListener onOffSlotChatListener = new OnOffSlotChatListener();
	private static readonly GlobalWindowChatListener globalWindowListener = new GlobalWindowChatListener();

	private static string FormatWindow(int start, int end)
	{
		int sh = start / 60;
		int sm = start % 60;
		int eh = end / 60;
		int em = end % 60;
		return sh.ToString("D2") + ":" + sm.ToString("D2") + "-" + eh.ToString("D2") + ":" + em.ToString("D2");
	}

	public static string GetWindowSummaryLabel()
	{
		if (startMinute == endMinute) return "Cả ngày";
		return FormatWindow(startMinute, endMinute);
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
		ChatTextField.gI().strChat = "Slot Train #" + (slotIndex + 1) + " (HH:mm-HH:mm|MapID|Khu|MobIDs)";
		ChatTextField.gI().tfChat.name = "VD: 08:00-12:00|29|2|13,14 (trống = xóa)";
		ChatTextField.gI().tfChat.setIputType(TField.INPUT_TYPE_ANY);
		ChatTextField.gI().startChat2(slotChatListener, string.Empty);
		ChatTextField.gI().tfChat.setText(trainSlotsRaw[slotIndex] ?? string.Empty);
	}

	public static void ShowInputOnOffSlot(int slotIndex)
	{
		if (slotIndex < 0 || slotIndex >= onOffSlotsRaw.Length) slotIndex = 0;
		editingOnOffSlotIndex = slotIndex;
		ModUI.uiCustomOpen = false;
		ModMenu.modMenuOpen = false;
		ChatTextField.gI().strChat = "Slot Nghỉ #" + (slotIndex + 1) + " (Giờ Tắt-Giờ Bật HH:mm-HH:mm)";
		ChatTextField.gI().tfChat.name = "VD: 23:30-05:30 (trống = xóa)";
		ChatTextField.gI().tfChat.setIputType(TField.INPUT_TYPE_ANY);
		ChatTextField.gI().startChat2(onOffSlotChatListener, string.Empty);
		ChatTextField.gI().tfChat.setText(onOffSlotsRaw[slotIndex] ?? string.Empty);
	}

	public static bool Configure(string value)
	{
		if (!TryParseWindow(value, out int start, out int end)) return false;
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

	private static bool TryParseWindow(string raw, out int startMin, out int endMin)
	{
		startMin = 0;
		endMin = 0;
		if (string.IsNullOrEmpty(raw)) return false;
		string[] parts = raw.Split('-');
		return parts.Length == 2 && TryMinute(parts[0], out startMin) && TryMinute(parts[1], out endMin);
	}

	private static bool IsMinuteInRange(int current, int startMin, int endMin)
	{
		if (startMin == endMin) return true;
		return (startMin < endMin) ? (current >= startMin && current < endMin) : (current >= startMin || current < endMin);
	}

	private static bool IsCurrentMinuteInOffPeriod(int current)
	{
		if (startMinute != endMinute && !IsMinuteInRange(current, startMinute, endMinute))
		{
			return true;
		}
		for (int i = 0; i < onOffSlotsRaw.Length; i++)
		{
			if (!string.IsNullOrEmpty(onOffSlotsRaw[i]) &&
			    TryParseWindow(onOffSlotsRaw[i], out int offMin, out int onMin) &&
			    offMin != onMin &&
			    IsMinuteInRange(current, offMin, onMin))
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsInScheduledOffPeriod()
	{
		if (!isEnabled) return false;
		int current = DateTime.Now.Hour * 60 + DateTime.Now.Minute;
		return IsCurrentMinuteInOffPeriod(current);
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
		if (!TryParseWindow(segs[0], out startMin, out endMin)) return false;
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

	private static void CapturePreSlotBackup()
	{
		if (s_HasPreSlotBackup) return;
		s_HasPreSlotBackup = true;
		s_BackupGoBackActive = ModGoBack.isGoBackActive;
		s_BackupGoBackMapId = ModGoBack.savedMapId;
		s_BackupGoBackZoneId = ModGoBack.savedZoneId;
		s_BackupGoBackX = ModGoBack.savedX;
		s_BackupGoBackY = ModGoBack.savedY;
		s_BackupSelectAllMobs = ModTanSatFilter.selectAllMobs;
		s_BackupSelectedMobId = ModTanSatFilter.selectedMobTemplateId;
		s_BackupTickedMobIds.Clear();
		s_BackupTickedMobIds.AddRange(ModTanSatFilter.tickedMobTemplateIds);
		s_BackupTanSatActive = ModTanSat.autoTanSat;
	}

	private static void RestorePreSlotBackup(bool allowReturnToOldSpot)
	{
		if (!s_HasPreSlotBackup) return;
		s_HasPreSlotBackup = false;
		ModGoBack.isGoBackActive = s_BackupGoBackActive;
		ModGoBack.savedMapId = s_BackupGoBackMapId;
		ModGoBack.savedZoneId = s_BackupGoBackZoneId;
		ModGoBack.savedX = s_BackupGoBackX;
		ModGoBack.savedY = s_BackupGoBackY;
		ModTanSatFilter.selectAllMobs = s_BackupSelectAllMobs;
		ModTanSatFilter.selectedMobTemplateId = s_BackupSelectedMobId;
		ModTanSatFilter.tickedMobTemplateIds.Clear();
		ModTanSatFilter.tickedMobTemplateIds.AddRange(s_BackupTickedMobIds);
		ModTanSat.autoTanSat = s_BackupTanSatActive;

		if (allowReturnToOldSpot && ModMenu.IsInGame() && s_BackupGoBackMapId >= 0 &&
		    (s_BackupGoBackActive || s_BackupTanSatActive) && TileMap.mapID != s_BackupGoBackMapId)
		{
			ModGoBack.BeginReturn(s_BackupGoBackMapId, s_BackupGoBackZoneId, s_BackupGoBackX, s_BackupGoBackY, forcedHomeReturn: false);
		}
	}

	public static void Toggle()
	{
		isEnabled = !isEnabled;
		if (!isEnabled && activeSlotIndex != -1)
		{
			activeSlotIndex = -1;
			RestorePreSlotBackup(allowReturnToOldSpot: false);
		}
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
			GameScr.info1?.addInfo("Đến giờ nghỉ theo lịch: Ngắt kết nối!", 0);
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

		// 1. Ưu tiên cao nhất: Kiểm tra khung giờ tổng quát và 6 khung giờ Nghỉ On/Off độc lập
		if (IsCurrentMinuteInOffPeriod(current))
		{
			if (activeSlotIndex != -1)
			{
				activeSlotIndex = -1;
				RestorePreSlotBackup(allowReturnToOldSpot: false);
			}
			if (!hasEvaluatedWindow || lastWindow)
			{
				hasEvaluatedWindow = true;
				lastWindow = false;
				ModTanSat.autoTanSat = false;
				ModAutoPick.autoPick = false;
				ModAutoItem.isEnabled = false;
				GameScr.info1?.addInfo("Lịch Auto: TẠM NGHỈ (Ngoài khung giờ / Giờ nghỉ)", 0);
			}
			HandleOffWindowDisconnect();
			return;
		}

		HandleOnWindowReconnect();

		// 2. Kiểm tra 6 khung giờ hẹn lịch Train chuyên biệt (Map + Khu + Quái Tàn Sát)
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
				if (IsMinuteInRange(current, sMin, eMin))
				{
					matchedSlot = i;
					targetMap = mId;
					targetZone = zId;
					targetMobIds = mList;
					break;
				}
			}
		}

		if (matchedSlot >= 0)
		{
			hasEvaluatedWindow = true;
			lastWindow = true;
			if (ModMenu.IsInGame() && !Char.isLoadingMap && !Char.ischangingMap)
			{
				if (activeSlotIndex != matchedSlot)
				{
					if (activeSlotIndex == -1)
					{
						CapturePreSlotBackup();
					}
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
					ModGoBack.isGoBackActive = true;
					ModGoBack.savedMapId = targetMap;
					ModGoBack.savedZoneId = targetZone;
					ModGoBack.savedX = -1;
					ModGoBack.savedY = -1;
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
			return;
		}

		if (activeSlotIndex != -1)
		{
			activeSlotIndex = -1;
			RestorePreSlotBackup(allowReturnToOldSpot: true);
			if (!ModTanSat.autoTanSat)
			{
				GameScr.info1?.addInfo("Đã hết khung giờ hẹn lịch Train! Khôi phục trạng thái trước đó.", 0);
			}
			else
			{
				GameScr.info1?.addInfo("Đã hết lịch Train! Đang quay về vị trí cày ban đầu...", 0);
			}
			return;
		}

		if (anyCustomSlotConfigured && startMinute == endMinute)
		{
			return;
		}

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
