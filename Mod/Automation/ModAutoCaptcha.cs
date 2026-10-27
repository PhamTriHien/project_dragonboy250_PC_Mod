using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
#if NET8_0_OR_GREATER
using System.Net.Http;
#endif
using UnityEngine;

/// <summary>
/// Tự động nhận diện và giải Captcha (MobCapcha) qua API HTTP/HTTPS, gõ phím tuần tự và ngắt kết nối an toàn khi sai quá ngưỡng.
/// </summary>
public sealed class ModAutoCaptcha : IActionListener
{
	private static readonly ModAutoCaptcha s_Instance = new ModAutoCaptcha();

	private const int ACT_SAVE_API_URL = 29041;

	public static bool isEnabled = false;
	public static bool disconnectOnFail = true;
	public static int maxFailCount = 5;
	public static string captchaApiUrl = string.Empty;

	private enum CaptchaState
	{
		Idle,
		WaitBeforeSolve,
		SolvingHttp,
		TypingChars,
		WaitVerifyResult
	}

	private static CaptchaState s_State = CaptchaState.Idle;
	private static byte[] s_CurrentPngBytes = null;
	private static string s_SolvedCode = string.Empty;
	private static int s_TypingIndex = 0;
	private static bool s_SentEnterConfirm = false;
	private static int s_NextCharDelayMs = 260;
	private static long s_StateTimer = 0;
	private static int s_FailCount = 0;
	private static volatile bool s_HttpDone = false;
	private static volatile string s_HttpResponse = null;

#if NET8_0_OR_GREATER
	private static readonly HttpClient s_HttpClient = CreateHttpClient();

	private static HttpClient CreateHttpClient()
	{
		HttpClient client = new HttpClient();
		client.Timeout = TimeSpan.FromSeconds(25);
		return client;
	}
#else
	private static WWW s_ActiveWww = null;
#endif

	public static ModAutoCaptcha gI()
	{
		return s_Instance;
	}

	public static bool IsCaptchaActive()
	{
		return GameScr.gI() != null && GameScr.gI().mobCapcha != null && !MobCapcha.explode;
	}

	private static bool s_ReopenModUiAfterInput = false;

	public static void PromptApiUrl()
	{
		if (GameCanvas.inputDlg == null) return;
		s_ReopenModUiAfterInput = ModUI.uiCustomOpen;
		ModUI.uiCustomOpen = false;
		ModMenu.modMenuOpen = false;
		GameCanvas.inputDlg.show(
			"Nhập URL API giải Captcha (POST image=base64):",
			new Command(mResources.OK, gI(), ACT_SAVE_API_URL, null),
			TField.INPUT_TYPE_ANY);
		if (GameCanvas.inputDlg.tfInput != null)
		{
			GameCanvas.inputDlg.tfInput.setMaxTextLenght(250);
			GameCanvas.inputDlg.tfInput.setText(captchaApiUrl ?? string.Empty);
			if (!Main.isPC) GameCanvas.inputDlg.tfInput.setFocusWithKb(isFocus: true);
		}
	}

	public void perform(int idAction, object p)
	{
		if (idAction == ACT_SAVE_API_URL)
		{
			captchaApiUrl = (GameCanvas.inputDlg.tfInput.getText() ?? string.Empty).Trim();
			GameCanvas.endDlg();
			ModConfig.SaveConfig();
			GameScr.info1.addInfo(
				string.IsNullOrEmpty(captchaApiUrl)
					? "Đã xóa URL API giải Captcha."
					: "Đã lưu API Captcha: " + captchaApiUrl,
				0);
			if (s_ReopenModUiAfterInput)
			{
				s_ReopenModUiAfterInput = false;
				ModUI.uiCustomOpen = true;
			}
		}
	}

	/// <summary>
	/// Tiếp nhận mảng byte ảnh PNG Captcha nguyên bản từ gói tin máy chủ (cmd = -85, b = 0).
	/// </summary>
	public static void OnCaptchaReceived(byte[] rawPngBytes)
	{
		if (rawPngBytes == null || rawPngBytes.Length == 0) return;
		s_CurrentPngBytes = rawPngBytes;
		s_SolvedCode = string.Empty;
		s_TypingIndex = 0;
		s_SentEnterConfirm = false;
		s_NextCharDelayMs = 200 + Res.random(0, 201);
		s_HttpDone = false;
		s_HttpResponse = null;
		s_FailCount = 0;
		if (isEnabled && !string.IsNullOrEmpty(captchaApiUrl))
		{
			s_State = CaptchaState.WaitBeforeSolve;
			s_StateTimer = mSystem.currentTimeMillis();
		}
		else
		{
			s_State = CaptchaState.Idle;
		}
	}

	public static void Reset()
	{
		s_State = CaptchaState.Idle;
		s_SolvedCode = string.Empty;
		s_TypingIndex = 0;
		s_SentEnterConfirm = false;
		s_NextCharDelayMs = 200 + Res.random(0, 201);
		s_HttpDone = false;
		s_HttpResponse = null;
#if !NET8_0_OR_GREATER
		if (s_ActiveWww != null)
		{
			try { s_ActiveWww.Dispose(); } catch { }
			s_ActiveWww = null;
		}
#endif
	}

	public static void Update()
	{
		if (!ModMenu.IsInGame())
		{
			if (s_State != CaptchaState.Idle) Reset();
			return;
		}

		if (!IsCaptchaActive())
		{
			if (s_State != CaptchaState.Idle)
			{
				GameScr.info1.addInfo("Auto Captcha: Đã vượt Captcha thành công!", 0);
				Reset();
			}
			return;
		}

		if (!isEnabled || string.IsNullOrEmpty(captchaApiUrl))
		{
			return;
		}

		long now = mSystem.currentTimeMillis();

		switch (s_State)
		{
			case CaptchaState.Idle:
				if (s_CurrentPngBytes != null && s_CurrentPngBytes.Length > 0)
				{
					s_State = CaptchaState.WaitBeforeSolve;
					s_StateTimer = now;
				}
				break;

			case CaptchaState.WaitBeforeSolve:
				if (now - s_StateTimer >= 1000)
				{
					StartSolveRequest();
				}
				break;

			case CaptchaState.SolvingHttp:
#if !NET8_0_OR_GREATER
				if (s_ActiveWww != null && s_ActiveWww.isDone)
				{
					s_HttpResponse = string.IsNullOrEmpty(s_ActiveWww.error) ? s_ActiveWww.text : null;
					try { s_ActiveWww.Dispose(); } catch { }
					s_ActiveWww = null;
					s_HttpDone = true;
				}
#endif
				if (s_HttpDone)
				{
					string parsed = ExtractCaptchaAnswer(s_HttpResponse);
					if (!string.IsNullOrEmpty(parsed) && parsed.Length >= 4 && parsed.Length <= 7)
					{
						s_SolvedCode = parsed;
						s_TypingIndex = 0;
						s_SentEnterConfirm = false;
						s_NextCharDelayMs = 200 + Res.random(0, 201);
						s_State = CaptchaState.TypingChars;
						s_StateTimer = now;
						GameScr.info1.addInfo("Auto Captcha: Nhận mã [" + s_SolvedCode + "], đang nhập...", 0);
					}
					else
					{
						HandleFailedAttempt(now, "API không trả về mã hợp lệ");
					}
				}
				else if (now - s_StateTimer > 30000)
				{
					HandleFailedAttempt(now, "Quá hạn 30s chờ API Captcha");
				}
				break;

			case CaptchaState.TypingChars:
				if (now - s_StateTimer >= s_NextCharDelayMs)
				{
					s_StateTimer = now;
					s_NextCharDelayMs = 200 + Res.random(0, 201);
					if (s_TypingIndex < s_SolvedCode.Length)
					{
						char rawCh = s_SolvedCode[s_TypingIndex++];
						SendSingleCaptchaChar(rawCh);
					}
					else if (!s_SentEnterConfirm)
					{
						s_SentEnterConfirm = true;
						Service.gI().mobCapcha('\r');
						s_State = CaptchaState.WaitVerifyResult;
					}
					else
					{
						s_State = CaptchaState.WaitVerifyResult;
					}
				}
				break;

			case CaptchaState.WaitVerifyResult:
				if (now - s_StateTimer >= 3000)
				{
					if (IsCaptchaActive())
					{
						HandleFailedAttempt(now, "Mã Captcha bị từ chối");
					}
					else
					{
						Reset();
					}
				}
				break;
		}
	}

	private static void HandleFailedAttempt(long now, string reason)
	{
		s_FailCount++;
		if (s_FailCount > maxFailCount)
		{
			Reset();
			if (disconnectOnFail)
			{
				GameScr.info1.addInfo("Auto Captcha sai quá " + maxFailCount + " lần! Tự ngắt kết nối bảo vệ nick.", 0);
				ModAutoLogin.NotifyIntentionalDisconnect();
				Session_ME.gI().close();
			}
			else
			{
				isEnabled = false;
				GameScr.info1.addInfo("Auto Captcha sai quá " + maxFailCount + " lần! Đã tạm tắt.", 0);
			}
			return;
		}
		GameScr.info1.addInfo("Auto Captcha (" + s_FailCount + "/" + maxFailCount + "): " + reason + ", thử lại sau 3s...", 0);
		s_State = CaptchaState.WaitBeforeSolve;
		s_StateTimer = now + 2000;
	}

	private static void SendSingleCaptchaChar(char ch)
	{
		GameScr scr = GameScr.gI();
		if (scr == null) return;

		char exactChar = ch;
		if (!string.IsNullOrEmpty(scr.strCapcha))
		{
			for (int i = 0; i < scr.strCapcha.Length; i++)
			{
				if (char.ToUpperInvariant(scr.strCapcha[i]) == char.ToUpperInvariant(ch))
				{
					exactChar = scr.strCapcha[i];
					break;
				}
			}
		}

		string curInput = scr.keyInput ?? "-----";
		if (curInput.Length > 0)
		{
			scr.keyInput = curInput.Substring(1) + char.ToUpperInvariant(exactChar);
		}
		Service.gI().mobCapcha(exactChar);
	}

	private static void StartSolveRequest()
	{
		if (s_CurrentPngBytes == null || s_CurrentPngBytes.Length == 0 || string.IsNullOrEmpty(captchaApiUrl))
		{
			s_State = CaptchaState.Idle;
			return;
		}

		s_HttpDone = false;
		s_HttpResponse = null;
		s_State = CaptchaState.SolvingHttp;
		s_StateTimer = mSystem.currentTimeMillis();
		string b64 = Convert.ToBase64String(s_CurrentPngBytes);
		string url = captchaApiUrl.Trim();

#if BROWSER
		if (OperatingSystem.IsBrowser())
		{
			_ = SolveBrowserAsync(url, b64);
			return;
		}
#endif
#if NET8_0_OR_GREATER
		Thread worker = new Thread((ThreadStart)delegate
		{
			try
			{
				var form = new FormUrlEncodedContent(new KeyValuePair<string, string>[]
				{
					new KeyValuePair<string, string>("image", b64)
				});
				using (var resp = s_HttpClient.PostAsync(url, form).GetAwaiter().GetResult())
				{
					if (resp.IsSuccessStatusCode)
					{
						s_HttpResponse = resp.Content.ReadAsStringAsync().GetAwaiter().GetResult();
					}
				}
			}
			catch
			{
				s_HttpResponse = null;
			}
			finally
			{
				s_HttpDone = true;
			}
		});
		worker.IsBackground = true;
		worker.Start();
#else
		try
		{
			string body = "image=" + Uri.EscapeDataString(b64);
			byte[] postBytes = Encoding.ASCII.GetBytes(body);
			Dictionary<string, string> headers = new Dictionary<string, string>();
			headers["Content-Type"] = "application/x-www-form-urlencoded";
			s_ActiveWww = new WWW(url, postBytes, headers);
		}
		catch
		{
			s_HttpResponse = null;
			s_HttpDone = true;
		}
#endif
	}

#if BROWSER
	private static async System.Threading.Tasks.Task SolveBrowserAsync(string url, string b64)
	{
		try
		{
			var pairs = new KeyValuePair<string, string>[]
			{
				new KeyValuePair<string, string>("image", b64)
			};
			string baseUrl = DragonBoy_Web_Host.WebInterop.GetHttpBaseUrl().TrimEnd('/');
			string relayUrl = baseUrl + "/api/proxy?url=" + Uri.EscapeDataString(url);
			using (var relayForm = new FormUrlEncodedContent(pairs))
			using (var resp = await s_HttpClient.PostAsync(relayUrl, relayForm))
			{
				if (resp.IsSuccessStatusCode)
				{
					s_HttpResponse = await resp.Content.ReadAsStringAsync();
					return;
				}
				int code = (int)resp.StatusCode;
				if (code != 404 && code != 405)
				{
					return;
				}
			}
			using (var directForm = new FormUrlEncodedContent(pairs))
			using (var directResp = await s_HttpClient.PostAsync(url, directForm))
			{
				if (directResp.IsSuccessStatusCode)
				{
					s_HttpResponse = await directResp.Content.ReadAsStringAsync();
				}
			}
		}
		catch
		{
			s_HttpResponse = null;
		}
		finally
		{
			s_HttpDone = true;
		}
	}
#endif

	private static string ExtractCaptchaAnswer(string raw)
	{
		if (string.IsNullOrEmpty(raw)) return null;
		string trimmed = raw.Trim();
		if (trimmed.Length == 0) return null;

		if (trimmed.StartsWith("{"))
		{
			string status = ModAutoUpdate.ExtractJsonField(trimmed, "status");
			if (!string.IsNullOrEmpty(status) && status != "0" &&
			    !string.Equals(status, "ok", StringComparison.OrdinalIgnoreCase) &&
			    !string.Equals(status, "true", StringComparison.OrdinalIgnoreCase) &&
			    !string.Equals(status, "success", StringComparison.OrdinalIgnoreCase))
			{
				return null;
			}

			string[] keys = new string[] { "captcha", "code", "result", "text", "data" };
			for (int i = 0; i < keys.Length; i++)
			{
				string val = ModAutoUpdate.ExtractJsonField(trimmed, keys[i]);
				if (!string.IsNullOrEmpty(val))
				{
					return FilterAllowedChars(val.Trim());
				}
			}
			return null;
		}

		return FilterAllowedChars(trimmed);
	}

	private static string FilterAllowedChars(string candidate)
	{
		if (string.IsNullOrEmpty(candidate)) return string.Empty;
		GameScr scr = GameScr.gI();
		string keypad = (scr != null) ? scr.strCapcha : null;
		StringBuilder sb = new StringBuilder();
		for (int i = 0; i < candidate.Length; i++)
		{
			char c = candidate[i];
			if (char.IsWhiteSpace(c)) continue;
			if (!string.IsNullOrEmpty(keypad))
			{
				for (int k = 0; k < keypad.Length; k++)
				{
					if (char.ToUpperInvariant(keypad[k]) == char.ToUpperInvariant(c))
					{
						sb.Append(keypad[k]);
						break;
					}
				}
			}
			else if (char.IsLetterOrDigit(c))
			{
				sb.Append(c);
			}
		}
		return sb.ToString();
	}
}
