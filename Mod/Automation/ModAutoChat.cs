using System;

/// <summary>
/// Module Auto Chat: Tự động gửi tin nhắn chat theo chu kỳ thời gian cấu hình (hỗ trợ xoay vòng nhiều câu cách nhau bằng dấu ;).
/// </summary>
public static class ModAutoChat
{
	public static bool isEnabled;
	public static string message = string.Empty;
	public static int intervalMs = 30000;
	private static long nextSendAt;
	private static int currentPhraseIndex = 0;

	/// <summary>
	/// Cấu hình tham số và thiết lập hoạt động cho module.
	/// </summary>
	public static void Configure(string text, int interval)
	{
		message = (text ?? string.Empty).Trim();
		intervalMs = interval < 5000 ? 5000 : (interval > 3600000 ? 3600000 : interval);
		nextSendAt = 0;
		currentPhraseIndex = 0;
		isEnabled = !string.IsNullOrEmpty(message);
		ModConfig.SaveConfig();
	}

	/// <summary>
	/// Bật hoặc tắt trạng thái kích hoạt của Toggle.
	/// </summary>
	public static void Toggle()
	{
		if (!isEnabled && string.IsNullOrEmpty(message == null ? null : message.Trim()))
		{
			GameScr.info1?.addInfo("Auto Chat: hãy đặt nội dung trước.", 0);
			return;
		}
		isEnabled = !isEnabled;
		nextSendAt = 0;
		currentPhraseIndex = 0;
		ModConfig.SaveConfig();
		GameScr.info1?.addInfo("Auto Chat: " + (isEnabled ? "BẬT" : "TẮT"), 0);
	}

	/// <summary>
	/// Dừng tiến trình hoặc luồng xử lý nền.
	/// </summary>
	public static void Stop()
	{
		isEnabled = false;
		nextSendAt = 0;
	}

	private static bool reopenModUiAfterInput;

	/// <summary>
	/// Thực thi phương thức nghiệp vụ ShowInputChatMessage.
	/// </summary>
	public static void ShowInputChatMessage()
	{
		if (GameCanvas.inputDlg == null) return;
		reopenModUiAfterInput = ModUI.uiCustomOpen;
		ModUI.uiCustomOpen = false;
		ModMenu.modMenuOpen = false;
		GameCanvas.inputDlg.show("Nhập nội dung Auto Chat (nhiều câu cách nhau dấu ;):", new Command("OK", new AutoChatActionListener(), 1, null), TField.INPUT_TYPE_ANY);
		if (GameCanvas.inputDlg.tfInput != null)
		{
			GameCanvas.inputDlg.tfInput.setMaxTextLenght(200);
			GameCanvas.inputDlg.tfInput.setText(message ?? string.Empty);
			if (!Main.isPC) GameCanvas.inputDlg.tfInput.setFocusWithKb(isFocus: true);
		}
	}

	public static void ShowInputIntervalSeconds()
	{
		if (GameCanvas.inputDlg == null) return;
		reopenModUiAfterInput = ModUI.uiCustomOpen;
		ModUI.uiCustomOpen = false;
		ModMenu.modMenuOpen = false;
		int curSec = System.Math.Max(5, intervalMs / 1000);
		GameCanvas.inputDlg.show("Nhập chu kỳ Auto Chat (giây, tối thiểu 5s):", new Command("OK", new AutoChatIntervalListener(), 1, null), TField.INPUT_TYPE_NUMERIC);
		if (GameCanvas.inputDlg.tfInput != null)
		{
			GameCanvas.inputDlg.tfInput.setMaxTextLenght(5);
			GameCanvas.inputDlg.tfInput.setText(curSec.ToString());
			if (!Main.isPC) GameCanvas.inputDlg.tfInput.setFocusWithKb(isFocus: true);
		}
	}

	private class AutoChatIntervalListener : IActionListener
	{
		public void perform(int idAction, object p)
		{
			try
			{
				if (idAction == 1 && GameCanvas.inputDlg != null && GameCanvas.inputDlg.tfInput != null)
				{
					string raw = (GameCanvas.inputDlg.tfInput.getText() ?? string.Empty).Trim();
					if (int.TryParse(raw, out int sec) && sec >= 5)
					{
						if (sec > 3600) sec = 3600;
						intervalMs = sec * 1000;
						nextSendAt = 0;
						ModConfig.SaveConfig();
						GameScr.info1?.addInfo("Chu kỳ Auto Chat: " + sec + " giây.", 0);
						SoundMn.gI().buttonClick();
					}
				}
				GameCanvas.endDlg();
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
	/// CLASS AutoChatActionListener: Định nghĩa và điều phối logic trong hệ thống game.
	/// </summary>
	private class AutoChatActionListener : IActionListener
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
					Configure(text, intervalMs);
					GameCanvas.endDlg();
					GameScr.info1?.addInfo("Đã lưu nội dung Auto Chat.", 0);
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
		if (!isEnabled || string.IsNullOrEmpty(message == null ? null : message.Trim()) || !ModMenu.IsInGame()) return;
		if (!Session_ME.gI().isConnected()) return;
		long now = mSystem.currentTimeMillis();
		if (nextSendAt != 0 && now < nextSendAt) return;
		try
		{
			string[] phrases = message.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
			string toSend = message;
			if (phrases != null && phrases.Length > 0)
			{
				if (currentPhraseIndex < 0 || currentPhraseIndex >= phrases.Length)
				{
					currentPhraseIndex = 0;
				}
				toSend = phrases[currentPhraseIndex].Trim();
				currentPhraseIndex = (currentPhraseIndex + 1) % phrases.Length;
			}
			if (!string.IsNullOrEmpty(toSend))
			{
				Service.gI().chat(toSend);
			}
			int clamped = intervalMs < 5000 ? 5000 : (intervalMs > 3600000 ? 3600000 : intervalMs);
			nextSendAt = now + clamped;
		}
		catch
		{
			nextSendAt = now + 10000;
		}
	}
}
