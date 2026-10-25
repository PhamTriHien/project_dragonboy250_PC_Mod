using System;
using System.Collections.Generic;

/// <summary>
/// Module Auto Sử Dụng Vật Phẩm: Tự động dùng item buff theo thời gian (Cuồng nộ, Bổ huyết, Bổ khí, Giáp xên, Ẩn danh, Đồ ăn, Hải sản, Máy dò...) và tự động dùng item theo danh sách ID.
/// Kiểm tra chặt chẽ thời gian hiệu lực trong Char.vItemTime để chỉ tự dùng lại khi hết thời gian, tuyệt đối không dùng liên tục.
/// </summary>
public static class ModAutoItem
{
	public static bool isEnabled;
	public static bool autoOpenCskb = false;
	private static long lastCskbTime = 0;
	public static string itemIdsRaw = string.Empty;
	public static HashSet<int> itemIds = new HashSet<int>();
	private static readonly List<int> s_OrderedItemIds = new List<int>();
	public static int intervalMs = 3000;
	public static int conditionType;
	public static int thresholdPercent = 30;
	private static long nextAt;
	private static int lastBagIndex = -1;

	// Thời điểm cho phép kiểm tra lại sau khi đăng nhập / đổi map làm xóa tạm thời Char.vItemTime
	private static long s_WaitServerItemTimeUntil = 0;

	// Theo dõi thời điểm vừa gửi lệnh dùng từng item để khóa chờ gói tin cmd -106 từ Server (tránh dùng lặp liên tục)
	private static readonly Dictionary<int, long> s_LastUseTimeByItemId = new Dictionary<int, long>();
	private static readonly Dictionary<int, short> s_LearnedIconByItemId = new Dictionary<int, short>();
	private static int s_LastUsedItemId = -1;
	private static long s_LastUsedItemTimestamp = 0;

	// Danh sách 12 nút bật/tắt nhanh các nhóm item có thời gian sử dụng phổ biến
	public static readonly string[] PresetNames = new string[]
	{
		"Cuồng Nộ",
		"Bổ Huyết",
		"Bổ Khí",
		"Giáp Xên",
		"Ẩn Danh",
		"Đồ Ăn",
		"Hải Sản",
		"Máy Dò",
		"Khẩu Trang",
		"Bùa x2 Đệ",
		"Cỏ 4 Lá",
		"Combo Buff"
	};

	public static readonly int[][] PresetItemGroups = new int[][]
	{
		new int[] { 381, 1099 },
		new int[] { 382, 1100 },
		new int[] { 383, 1101 },
		new int[] { 384, 1102 },
		new int[] { 385, 1103 },
		new int[] { 663, 664, 665, 666, 667 },
		new int[] { 880, 881, 882 },
		new int[] { 379 },
		new int[] { 764 },
		new int[] { 1628 },
		new int[] { 859, 1635 },
		new int[] { 381, 1099, 382, 1100, 383, 1101, 384, 1102 }
	};

	// Các nhóm item dùng chung 1 ô hiệu ứng thời gian (chỉ dùng 1 item trong nhóm cho đến khi hết hiệu lực)
	private static readonly int[][] s_SharedSlotItemGroups = new int[][]
	{
		new int[] { 381, 1099 },
		new int[] { 382, 1100 },
		new int[] { 383, 1101 },
		new int[] { 384, 1102 },
		new int[] { 385, 1103 },
		new int[] { 663, 664, 665, 666, 667 },
		new int[] { 880, 881, 882 },
		new int[] { 859, 1635 }
	};

	// Bảng ánh xạ chính xác từ ID template vật phẩm sang danh sách idIcon trong Char.vItemTime (trích xuất từ m_0.java & dữ liệu gốc)
	private static readonly Dictionary<int, short[]> s_KnownItemIcons = new Dictionary<int, short[]>
	{
		{ 379, new short[] { 2758 } },
		{ 380, new short[] { 2759 } },
		{ 381, new short[] { 2754, 10716 } },
		{ 1099, new short[] { 10716, 2754 } },
		{ 382, new short[] { 2755, 10714 } },
		{ 1100, new short[] { 10714, 2755 } },
		{ 383, new short[] { 2756, 10715 } },
		{ 1101, new short[] { 10715, 2756 } },
		{ 384, new short[] { 2757, 10712 } },
		{ 1102, new short[] { 10712, 2757 } },
		{ 385, new short[] { 2760, 10717 } },
		{ 1103, new short[] { 10717, 2760 } },
		{ 663, new short[] { 6324, 6325, 6326, 6327, 6328 } },
		{ 664, new short[] { 6324, 6325, 6326, 6327, 6328 } },
		{ 665, new short[] { 6324, 6325, 6326, 6327, 6328 } },
		{ 666, new short[] { 6324, 6325, 6326, 6327, 6328 } },
		{ 667, new short[] { 6324, 6325, 6326, 6327, 6328 } },
		{ 880, new short[] { 8060, 8061, 8062 } },
		{ 881, new short[] { 8060, 8061, 8062 } },
		{ 882, new short[] { 8060, 8061, 8062 } },
		{ 764, new short[] { 7149 } },
		{ 859, new short[] { 7992, 13618 } },
		{ 1628, new short[] { 13540 } },
		{ 1635, new short[] { 13618, 7992 } }
	};

	/// <summary>
	/// Thông báo khi danh sách Char.vItemTime vừa bị xóa lúc đăng nhập để chờ Server đồng bộ lại các gói cmd -106.
	/// </summary>
	public static void NotifyItemTimeCleared()
	{
		s_WaitServerItemTimeUntil = mSystem.currentTimeMillis() + 2500;
		s_LastUseTimeByItemId.Clear();
		s_LastUsedItemId = -1;
	}

	/// <summary>
	/// Tiếp nhận gói tin cmd -106 (ITEM_TIME) từ Server để học idIcon động cho item bất kỳ và mở khóa ngay khi hết hạn (seconds <= 0).
	/// </summary>
	public static void OnServerItemTime(short idIcon, int seconds)
	{
		long now = mSystem.currentTimeMillis();
		if (seconds > 0)
		{
			if (s_LastUsedItemId >= 0 && now - s_LastUsedItemTimestamp <= 3500)
			{
				s_LearnedIconByItemId[s_LastUsedItemId] = idIcon;
			}
		}
		else
		{
			// Khi Server báo hiệu ứng kết thúc (seconds <= 0), xóa khóa chờ của các item có icon tương ứng để tự dùng lại ngay
			foreach (KeyValuePair<int, short[]> kv in s_KnownItemIcons)
			{
				short[] icons = kv.Value;
				for (int i = 0; i < icons.Length; i++)
				{
					if (icons[i] == idIcon)
					{
						s_LastUseTimeByItemId.Remove(kv.Key);
						break;
					}
				}
			}
			foreach (KeyValuePair<int, short> kv in s_LearnedIconByItemId)
			{
				if (kv.Value == idIcon)
				{
					s_LastUseTimeByItemId.Remove(kv.Key);
				}
			}
		}
	}

	/// <summary>
	/// Khởi tạo và nạp dữ liệu cấu hình cho LoadIds.
	/// </summary>
	public static void LoadIds(string raw)
	{
		itemIds.Clear();
		s_OrderedItemIds.Clear();
		string source = raw == null ? string.Empty : raw.Trim();
		string[] parts = source.Split(new char[] { ',', ';', ' ', '|' }, StringSplitOptions.RemoveEmptyEntries);
		for (int i = 0; i < parts.Length; i++)
		{
			if (int.TryParse(parts[i], out int id) && id >= 0 && itemIds.Add(id))
			{
				s_OrderedItemIds.Add(id);
			}
		}
		RebuildItemIdsRaw();
	}

	private static void RebuildItemIdsRaw()
	{
		if (s_OrderedItemIds.Count == 0)
		{
			itemIdsRaw = string.Empty;
			return;
		}
		string[] strs = new string[s_OrderedItemIds.Count];
		for (int i = 0; i < s_OrderedItemIds.Count; i++)
		{
			strs[i] = s_OrderedItemIds[i].ToString();
		}
		itemIdsRaw = string.Join(",", strs);
	}

	/// <summary>
	/// Kiểm tra một nút Preset có đang được bật trong danh sách ID hay không.
	/// </summary>
	public static bool IsPresetActive(int presetIndex)
	{
		if (presetIndex < 0 || presetIndex >= PresetItemGroups.Length) return false;
		int[] group = PresetItemGroups[presetIndex];
		if (presetIndex == 11)
		{
			// Combo Buff: chỉ sáng khi cả 4 nhóm buff chính đều có mặt
			return IsPresetActive(0) && IsPresetActive(1) && IsPresetActive(2) && IsPresetActive(3);
		}
		for (int i = 0; i < group.Length; i++)
		{
			if (itemIds.Contains(group[i])) return true;
		}
		return false;
	}

	/// <summary>
	/// Bật hoặc tắt nhanh một nhóm item thời gian theo Preset.
	/// </summary>
	public static void TogglePreset(int presetIndex)
	{
		if (presetIndex < 0 || presetIndex >= PresetItemGroups.Length) return;
		int[] group = PresetItemGroups[presetIndex];
		bool currentlyActive = IsPresetActive(presetIndex);

		if (currentlyActive)
		{
			for (int i = 0; i < group.Length; i++)
			{
				int id = group[i];
				if (itemIds.Remove(id))
				{
					s_OrderedItemIds.Remove(id);
				}
			}
			if (itemIds.Count == 0)
			{
				isEnabled = false;
			}
		}
		else
		{
			for (int i = 0; i < group.Length; i++)
			{
				int id = group[i];
				if (itemIds.Add(id))
				{
					s_OrderedItemIds.Add(id);
				}
			}
			isEnabled = true;
		}

		RebuildItemIdsRaw();
		nextAt = 0;
		ModConfig.SaveConfig();
		GameScr.info1?.addInfo("Auto " + PresetNames[presetIndex] + ": " + (!currentlyActive ? "BẬT" : "TẮT"), 0);
	}

	/// <summary>
	/// Xóa toàn bộ danh sách ID đang chọn.
	/// </summary>
	public static void ClearAllIds()
	{
		itemIds.Clear();
		s_OrderedItemIds.Clear();
		itemIdsRaw = string.Empty;
		isEnabled = false;
		s_LastUseTimeByItemId.Clear();
		ModConfig.SaveConfig();
		GameScr.info1?.addInfo("Đã xóa danh sách ID Auto Item.", 0);
	}

	/// <summary>
	/// Cấu hình tham số và thiết lập hoạt động cho module.
	/// </summary>
	public static void Configure(string ids, int interval)
	{
		LoadIds(ids);
		conditionType = 0;
		intervalMs = interval < 1000 ? 1000 : (interval > 3600000 ? 3600000 : interval);
		isEnabled = itemIds.Count > 0;
		nextAt = 0;
		ModConfig.SaveConfig();
	}

	/// <summary>
	/// Bật hoặc tắt trạng thái kích hoạt của Toggle.
	/// </summary>
	public static void Toggle()
	{
		if (itemIds.Count == 0)
		{
			GameScr.info1.addInfo("Auto Item: hãy chọn món bên dưới hoặc bấm Nhập ID", 0);
			return;
		}
		isEnabled = !isEnabled;
		nextAt = 0;
		ModConfig.SaveConfig();
		GameScr.info1.addInfo("Auto Item: " + (isEnabled ? "BẬT" : "TẮT"), 0);
	}

	private static bool reopenModUiAfterInput;

	/// <summary>
	/// Thực thi phương thức nghiệp vụ ShowInputItemIds.
	/// </summary>
	public static void ShowInputItemIds()
	{
		if (GameCanvas.inputDlg == null) return;
		reopenModUiAfterInput = ModUI.uiCustomOpen;
		ModUI.uiCustomOpen = false;
		ModMenu.modMenuOpen = false;
		GameCanvas.inputDlg.show("Nhập danh sách ID item cần tự dùng (cách nhau dấu phẩy):", new Command("OK", new AutoItemActionListener(), 1, null), TField.INPUT_TYPE_ANY);
		if (GameCanvas.inputDlg.tfInput != null)
		{
			GameCanvas.inputDlg.tfInput.setMaxTextLenght(150);
			GameCanvas.inputDlg.tfInput.setText(itemIdsRaw);
			if (!Main.isPC) GameCanvas.inputDlg.tfInput.setFocusWithKb(isFocus: true);
		}
	}

	/// <summary>
	/// CLASS AutoItemActionListener: Định nghĩa và điều phối logic trong hệ thống game.
	/// </summary>
	private class AutoItemActionListener : IActionListener
	{
		/// <summary>
		/// Thực thi hành động tương tác khi người dùng kích hoạt lệnh hoặc nút bấm.
		/// </summary>
		public void perform(int idAction, object p)
		{
			try
			{
				if (idAction == 1 && GameCanvas.inputDlg != null && GameCanvas.inputDlg.tfInput != null)
				{
					string text = GameCanvas.inputDlg.tfInput.getText();
					LoadIds(text);
					if (itemIds.Count > 0)
					{
						isEnabled = true;
					}
					nextAt = 0;
					ModConfig.SaveConfig();
					GameCanvas.endDlg();
					GameScr.info1?.addInfo("Đã lưu danh sách ID Auto Item (" + itemIds.Count + " ID).", 0);
					SoundMn.gI().buttonClick();
				}
				else
				{
					GameCanvas.endDlg();
				}
				if (reopenModUiAfterInput)
				{
					reopenModUiAfterInput = false;
					ModUI.uiCustomOpen = true;
				}
			}
			catch
			{
				GameCanvas.endDlg();
			}
		}
	}

	/// <summary>
	/// Kiểm tra xem một idIcon có đang còn thời gian hiệu lực thực tế trong Char.vItemTime hay không.
	/// </summary>
	public static bool HasActiveItemTime(int iconId)
	{
		if (iconId <= 0 || Char.vItemTime == null)
		{
			return false;
		}
		for (int i = 0; i < Char.vItemTime.size(); i++)
		{
			ItemTime it = (ItemTime)Char.vItemTime.elementAt(i);
			if (it != null && it.idIcon == iconId && (it.minute > 0 || it.second > 0))
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Kiểm tra toàn diện xem vật phẩm có thời gian sử dụng đã đang có hiệu lực (hoặc vừa kích hoạt chờ Server phản hồi) hay chưa.
	/// </summary>
	private static bool IsTimedItemCurrentlyActive(Item item, Char me, long now)
	{
		int tplId = item.template.id;

		// 1. Khóa chờ 6000ms sau khi vừa gửi gói useItem cho ID này (đợi Server trả về cmd -106, chống spam liên tục)
		if (s_LastUseTimeByItemId.TryGetValue(tplId, out long lastUsedAt) && now - lastUsedAt < 6000)
		{
			return true;
		}

		// 2. Kiểm tra trực tiếp iconID của ItemTemplate trong Char.vItemTime
		if (item.template.iconID > 0 && HasActiveItemTime(item.template.iconID))
		{
			return true;
		}

		// 3. Kiểm tra bảng ánh xạ idIcon chuẩn (bao gồm cả nhóm Đồ ăn 663..667, Hải sản 880..882, Buff cấp 1 & cấp 2)
		if (s_KnownItemIcons.TryGetValue(tplId, out short[] knownIcons))
		{
			for (int i = 0; i < knownIcons.Length; i++)
			{
				if (HasActiveItemTime(knownIcons[i]))
				{
					return true;
				}
			}
		}

		// 4. Kiểm tra idIcon đã học động từ gói tin cmd -106 của Server cho các item tùy chỉnh
		if (s_LearnedIconByItemId.TryGetValue(tplId, out short learnedIcon) && HasActiveItemTime(learnedIcon))
		{
			return true;
		}

		// 5. Nếu item thuộc nhóm dùng chung ô buff (ví dụ Đồ ăn, Hải sản, Cuồng nộ 1/2...), kiểm tra cả iconID của các item cùng nhóm trong hành trang
		for (int g = 0; g < s_SharedSlotItemGroups.Length; g++)
		{
			int[] group = s_SharedSlotItemGroups[g];
			bool inGroup = false;
			for (int k = 0; k < group.Length; k++)
			{
				if (group[k] == tplId)
				{
					inGroup = true;
					break;
				}
			}
			if (!inGroup) continue;

			for (int k = 0; k < group.Length; k++)
			{
				int peerId = group[k];
				if (s_LastUseTimeByItemId.TryGetValue(peerId, out long peerLastUsed) && now - peerLastUsed < 6000)
				{
					return true;
				}
				if (s_LearnedIconByItemId.TryGetValue(peerId, out short peerLearnedIcon) && HasActiveItemTime(peerLearnedIcon))
				{
					return true;
				}
			}

			if (me.arrItemBag != null)
			{
				for (int b = 0; b < me.arrItemBag.Length; b++)
				{
					Item bagItem = me.arrItemBag[b];
					if (bagItem == null || bagItem.template == null) continue;
					for (int k = 0; k < group.Length; k++)
					{
						if (bagItem.template.id == group[k] && bagItem.template.iconID > 0 && HasActiveItemTime(bagItem.template.iconID))
						{
							return true;
						}
					}
				}
			}
			break;
		}

		// 6. Trường hợp vật phẩm cải trang / khẩu trang (như Khẩu trang 764) hoặc trang bị mặc lên người: nếu đã mặc trong arrItemBody thì không dùng lặp lại
		if (tplId == 764 && me.arrItemBody != null)
		{
			for (int b = 0; b < me.arrItemBody.Length; b++)
			{
				Item bodyItem = me.arrItemBody[b];
				if (bodyItem != null && bodyItem.template != null && bodyItem.template.id == tplId)
				{
					return true;
				}
			}
		}

		return false;
	}

	private static void RecordItemUsed(int tplId, long now)
	{
		s_LastUseTimeByItemId[tplId] = now;
		s_LastUsedItemId = tplId;
		s_LastUsedItemTimestamp = now;

		for (int g = 0; g < s_SharedSlotItemGroups.Length; g++)
		{
			int[] group = s_SharedSlotItemGroups[g];
			bool inGroup = false;
			for (int k = 0; k < group.Length; k++)
			{
				if (group[k] == tplId)
				{
					inGroup = true;
					break;
				}
			}
			if (inGroup)
			{
				for (int k = 0; k < group.Length; k++)
				{
					s_LastUseTimeByItemId[group[k]] = now;
				}
				break;
			}
		}
	}

	/// <summary>
	/// Cập nhật trạng thái logic của đối tượng theo từng chu kỳ khung hình.
	/// </summary>
	public static void Update()
	{
		if (!ModMenu.IsInGame() || Char.myCharz() == null || GameScr.gI() == null) return;
		if (Char.isLoadingMap || Char.ischangingMap || Controller.isStopReadMessage) return;
		Char me = Char.myCharz();
		if (me.isDie || me.cHP <= 0 || me.statusMe == 14 || me.statusMe == 5) return;
		long now = mSystem.currentTimeMillis();

		if (autoOpenCskb && now - lastCskbTime >= 350 && me.arrItemBag != null)
		{
			for (int c = 0; c < me.arrItemBag.Length; c++)
			{
				Item cskb = me.arrItemBag[c];
				if (cskb != null && cskb.template != null && cskb.template.id == 380)
				{
					lastCskbTime = now;
					Service.gI().useItem(0, 1, (sbyte)cskb.indexUI, -1);
					break;
				}
			}
		}

		if (!isEnabled || itemIds.Count == 0) return;
		if (now < s_WaitServerItemTimeUntil || now < nextAt) return;
		Item[] bag = me.arrItemBag;
		if (bag == null || bag.Length == 0) return;
		if (conditionType == 1 && (me.cHPFull <= 0 || me.cHP * 100L > me.cHPFull * thresholdPercent)) return;
		if (conditionType == 2 && (me.cMPFull <= 0 || me.cMP * 100L > me.cMPFull * thresholdPercent)) return;

		int len = bag.Length;
		int startIdx = (lastBagIndex + 1) % len;
		if (startIdx < 0) startIdx = 0;

		for (int offset = 0; offset < len; offset++)
		{
			int i = (startIdx + offset) % len;
			Item item = bag[i];
			if (item == null || item.template == null || !itemIds.Contains(item.template.id)) continue;

			// Kiểm tra thời gian hiệu lực của vật phẩm: nếu đang còn thời gian trong Char.vItemTime thì tuyệt đối không dùng đè
			if (IsTimedItemCurrentlyActive(item, me, now))
			{
				continue;
			}

			lastBagIndex = i;
			RecordItemUsed(item.template.id, now);
			Service.gI().useItem(0, 1, (sbyte)item.indexUI, -1);
			// Khoảng nghỉ ngắn giữa 2 lần kích hoạt các item khác nhau để Server xử lý tuần tự và trả về gói cmd -106
			nextAt = now + (conditionType == 0 ? (intervalMs < 1000 ? intervalMs : 1000) : intervalMs);
			return;
		}
	}
}
