using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
#if NET8_0_OR_GREATER
using System.Net.Http;
#else
using System.Net;
#endif
using UnityEngine;

public static class ModAutoUpdate
{
	public const string CurrentVersion = "2.5.0";
	public const string ManifestUrl = "https://raw.githubusercontent.com/PhamTriHien/project_dragonboy250_PC_Mod/main/version.json";

	public static bool isChecking = false;
	public static bool hasChecked = false;
	public static bool hasNewVersion = false;
	public static bool hasPrompted = false;

	public static string remoteVersion = string.Empty;
	public static string downloadUrl = string.Empty;
	public static string changelog = string.Empty;
	public static string buildDate = string.Empty;
	public static string manualStatusMsg = null;

	public static void StartCheckAsync(bool force = false, bool isManual = false)
	{
		if (isChecking) return;
		if (!force && hasChecked) return;

		isChecking = true;
		if (isManual)
		{
			manualStatusMsg = "Đang kiểm tra cập nhật...";
		}

		Thread thread = new Thread((ThreadStart)delegate
		{
			try
			{
				PerformCheck(isManual);
			}
			catch (Exception ex)
			{
				if (isManual)
				{
					manualStatusMsg = "Kiểm tra thất bại: " + ex.Message;
				}
			}
			finally
			{
				isChecking = false;
				hasChecked = true;
			}
		});
		thread.IsBackground = true;
		thread.Start();
	}

	private static void PerformCheck(bool isManual)
	{
		string json = null;
		try
		{
#if NET8_0_OR_GREATER
			using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3)))
			using (var client = new HttpClient())
			{
				client.Timeout = TimeSpan.FromSeconds(3);
				client.DefaultRequestHeaders.Add("User-Agent", "DragonBoy-AutoUpdater/" + CurrentVersion);
				var resp = client.GetAsync(ManifestUrl, cts.Token).GetAwaiter().GetResult();
				if (resp.IsSuccessStatusCode)
				{
					json = resp.Content.ReadAsStringAsync(cts.Token).GetAwaiter().GetResult();
				}
			}
#else
			using (var wc = new WebClient())
			{
				wc.Headers.Add("User-Agent", "DragonBoy-AutoUpdater/" + CurrentVersion);
				json = wc.DownloadString(ManifestUrl);
			}
#endif
		}
		catch
		{
			if (isManual)
			{
				manualStatusMsg = "Không thể kết nối máy chủ cập nhật (Timeout/Mất mạng).";
			}
			return;
		}

		if (string.IsNullOrEmpty(json)) return;

		remoteVersion = ExtractJsonField(json, "version");
		changelog = ExtractJsonField(json, "changelog");
		buildDate = ExtractJsonField(json, "buildDate");
		downloadUrl = SelectDownloadUrl(json);

		if (!string.IsNullOrEmpty(remoteVersion) && IsNewerVersion(remoteVersion, CurrentVersion))
		{
			hasNewVersion = true;
			if (isManual)
			{
				manualStatusMsg = "Phát hiện bản mới v" + remoteVersion + "!";
			}
		}
		else
		{
			hasNewVersion = false;
			if (isManual)
			{
				manualStatusMsg = "Bạn đang dùng bản mới nhất (v" + CurrentVersion + ").";
			}
		}
	}

	public static string SelectDownloadUrl(string json)
	{
		bool isAndroid = (Application.platform == RuntimePlatform.Android) || (mSystem.clientType == 2);
		bool isIOS = (Application.platform == RuntimePlatform.IPhonePlayer) || (mSystem.clientType == 3 || mSystem.clientType == 5 || mSystem.clientType == 7);
		bool isWindows = (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor) || (mSystem.clientType == 4 || mSystem.clientType == 1);

		if (isAndroid)
		{
			string url = ExtractJsonField(json, "downloadUrl_android");
			if (!string.IsNullOrEmpty(url)) return url;
		}
		else if (isIOS)
		{
			string url = ExtractJsonField(json, "downloadUrl_ios");
			if (!string.IsNullOrEmpty(url)) return url;
		}
		else if (isWindows)
		{
			string url = ExtractJsonField(json, "downloadUrl_win");
			if (!string.IsNullOrEmpty(url)) return url;
		}

		return ExtractJsonField(json, "downloadUrl");
	}

	public static void UpdateTick()
	{
		if (hasNewVersion && !hasPrompted)
		{
			if (GameCanvas.currentDialog == null && ServerListScreen.loadScreen)
			{
				hasPrompted = true;
				ShowUpdateDialog();
			}
		}

		if (!string.IsNullOrEmpty(manualStatusMsg))
		{
			string msg = manualStatusMsg;
			manualStatusMsg = null;
			if (hasNewVersion)
			{
				ShowUpdateDialog();
			}
			else
			{
				GameCanvas.startOK(msg, 8882, null);
			}
		}
	}

	public static void ShowUpdateDialog()
	{
		string info = "Phát hiện bản cập nhật mới v" + remoteVersion + "!\n"
			+ (!string.IsNullOrEmpty(buildDate) ? ("Ngày phát hành: " + buildDate + "\n") : string.Empty)
			+ (!string.IsNullOrEmpty(changelog) ? ("Nội dung: " + changelog + "\n") : string.Empty)
			+ "Bạn có muốn tải bản mới ngay bây giờ không?";

		Command cmdYes = new Command("Tải ngay", new AutoUpdateActionListener(downloadUrl), 1, null);
		Command cmdNo = new Command("Để sau", new AutoUpdateActionListener(downloadUrl), 2, null);
		GameCanvas.startYesNoDlg(info, cmdYes, cmdNo);
	}

	public static void CheckManual()
	{
		if (isChecking)
		{
			GameCanvas.startOK("Đang kiểm tra cập nhật, vui lòng chờ...", 8882, null);
			return;
		}
		if (hasChecked)
		{
			if (hasNewVersion)
			{
				ShowUpdateDialog();
			}
			else
			{
				GameCanvas.startOK("Bạn đang sử dụng phiên bản mới nhất (v" + CurrentVersion + ")!", 8882, null);
			}
		}
		else
		{
			GameCanvas.startOK("Đang kết nối GitHub kiểm tra cập nhật...", 8882, null);
			StartCheckAsync(force: true, isManual: true);
		}
	}

	public static bool IsNewerVersion(string remote, string local)
	{
		try
		{
			var rParts = remote.TrimStart('v', 'V').Split('.');
			var lParts = local.TrimStart('v', 'V').Split('.');

			int max = System.Math.Max(rParts.Length, lParts.Length);
			for (int i = 0; i < max; i++)
			{
				int rNum = (i < rParts.Length && int.TryParse(rParts[i], out int r)) ? r : 0;
				int lNum = (i < lParts.Length && int.TryParse(lParts[i], out int l)) ? l : 0;

				if (rNum > lNum) return true;
				if (rNum < lNum) return false;
			}
		}
		catch { }
		return false;
	}

	public static string ExtractJsonField(string json, string fieldName)
	{
		try
		{
			string pattern = "\"" + fieldName + "\"";
			int idx = json.IndexOf(pattern, StringComparison.OrdinalIgnoreCase);
			if (idx == -1) return null;

			int colonIdx = json.IndexOf(':', idx + pattern.Length);
			if (colonIdx == -1) return null;

			int valStart = colonIdx + 1;
			while (valStart < json.Length && (json[valStart] == ' ' || json[valStart] == '\t' || json[valStart] == '\r' || json[valStart] == '\n'))
			{
				valStart++;
			}

			if (valStart >= json.Length) return null;

			if (json[valStart] == '"')
			{
				valStart++;
				int valEnd = json.IndexOf('"', valStart);
				if (valEnd == -1) return null;
				return json.Substring(valStart, valEnd - valStart);
			}
			else
			{
				int valEnd = valStart;
				while (valEnd < json.Length && json[valEnd] != ',' && json[valEnd] != '}' && json[valEnd] != '\r' && json[valEnd] != '\n')
				{
					valEnd++;
				}
				return json.Substring(valStart, valEnd - valStart).Trim();
			}
		}
		catch
		{
			return null;
		}
	}
}

public class AutoUpdateActionListener : IActionListener
{
	private string url;

	public AutoUpdateActionListener(string targetUrl)
	{
		this.url = targetUrl;
	}

	public void perform(int idAction, object p)
	{
		GameCanvas.endDlg();
		if (idAction == 1)
		{
			try
			{
				if (!string.IsNullOrEmpty(url))
				{
					Application.OpenURL(url);
				}
				else
				{
					GameScr.info1?.addInfo("Không tìm thấy liên kết tải bản cập nhật!", 0);
				}
			}
			catch (Exception ex)
			{
				GameScr.info1?.addInfo("Lỗi mở liên kết: " + ex.Message, 0);
			}
		}
		else if (idAction == 2)
		{
			ModAutoUpdate.hasPrompted = true;
		}
	}
}
