using System;
using System.Collections.Generic;

/// <summary>
/// Module Auto Sử Dụng Vật Phẩm: Tự động dùng máy dò, capsule kì bí, bổ huyết, cuồng nộ theo chu kỳ hoặc điều kiện.
/// </summary>
public static class ModAutoItem
{
	public static bool isEnabled;
	public static bool autoOpenCskb = false;
	private static long lastCskbTime = 0;
	public static string itemIdsRaw = string.Empty;
	public static HashSet<int> itemIds = new HashSet<int>();
	public static int intervalMs = 3000;
	public static int conditionType;
	public static int thresholdPercent = 30;
	private static long nextAt;
	private static int lastBagIndex = -1;

	/// <summary>
	/// Khởi tạo và nạp dữ liệu cấu hình cho LoadIds.
	/// </summary>
	public static void LoadIds(string raw)
	{
		itemIdsRaw = raw == null ? string.Empty : raw.Trim();
		itemIds.Clear();
		string[] parts = itemIdsRaw.Split(new char[] { ',', ';', ' ', '|' }, StringSplitOptions.RemoveEmptyEntries);
		for (int i = 0; i < parts.Length; i++)
		{
			if (int.TryParse(parts[i], out int id) && id >= 0) itemIds.Add(id);
		}
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
			GameScr.info1.addInfo("Auto Item: hãy bấm Nhập ID để thêm item", 0);
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
					ModConfig.SaveConfig();
					GameCanvas.endDlg();
					GameScr.info1?.addInfo("Đã lưu danh sách ID Auto Item.", 0);
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
	/// Cập nhật trạng thái logic của đối tượng theo từng chu kỳ khung hình.
	/// </summary>
	public static void Update()
	{
		if (!ModMenu.IsInGame() || Char.myCharz() == null || GameScr.gI() == null) return;
		if (Char.isLoadingMap || Char.ischangingMap) return;
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
		if (now < nextAt) return;
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
			if (conditionType == 0 && item.template.iconID > 0 && ItemTime.isExistItem(item.template.iconID))
			{
				continue;
			}
			lastBagIndex = i;
			Service.gI().useItem(0, 1, (sbyte)item.indexUI, -1);
			nextAt = now + intervalMs;
			return;
		}
	}
}
