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

	// In-Game Direct Download States
	public static bool isDownloading = false;
	public static bool isDownloadCanceled = false;
	public static int downloadPercent = 0;
	public static long downloadedBytes = 0;
	public static long totalBytes = 0;
	public static string downloadSpeedStr = string.Empty;

	public static void ResetAndCheckOnLaunch()
	{
		isChecking = false;
		hasChecked = false;
		hasNewVersion = false;
		hasPrompted = false;
		isDownloading = false;
		isDownloadCanceled = false;
		downloadPercent = 0;
		downloadedBytes = 0;
		totalBytes = 0;
		downloadSpeedStr = string.Empty;
		StartCheckAsync(force: true, isManual: false);
	}

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
		if (hasNewVersion && !hasPrompted && !isDownloading)
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
			+ "Bạn có muốn cập nhật trực tiếp ngay không?";

		Command cmdYes = new Command("Cập nhật", new AutoUpdateActionListener(downloadUrl), 1, null);
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

	public static void StartInGameDownload()
	{
		if (isDownloading) return;
		if (string.IsNullOrEmpty(downloadUrl))
		{
			GameScr.info1?.addInfo("Không tìm thấy đường dẫn tải bản cập nhật!", 0);
			return;
		}

		isDownloading = true;
		isDownloadCanceled = false;
		downloadPercent = 0;
		downloadedBytes = 0;
		totalBytes = 0;
		downloadSpeedStr = "0 KB/s";

		Thread thread = new Thread((ThreadStart)delegate
		{
			string targetFile = GetTargetLocalPath();
			string tempFile = targetFile + ".tmp";

			try
			{
				if (File.Exists(tempFile))
				{
					File.Delete(tempFile);
				}

				string dir = Path.GetDirectoryName(targetFile);
				if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
				{
					Directory.CreateDirectory(dir);
				}

#if NET8_0_OR_GREATER
				using (var client = new HttpClient())
				{
					client.Timeout = TimeSpan.FromMinutes(10);
					client.DefaultRequestHeaders.Add("User-Agent", "DragonBoy-AutoUpdater/" + CurrentVersion);

					using (var response = client.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead).GetAwaiter().GetResult())
					{
						response.EnsureSuccessStatusCode();
						totalBytes = response.Content.Headers.ContentLength ?? 0;

						using (var stream = response.Content.ReadAsStreamAsync().GetAwaiter().GetResult())
						using (var fs = new FileStream(tempFile, FileMode.Create, FileAccess.Write, FileShare.None, 65536, false))
						{
							ReadStreamToTarget(stream, fs);
						}
					}
				}
#else
				var req = (HttpWebRequest)WebRequest.Create(downloadUrl);
				req.Timeout = 600000;
				req.UserAgent = "DragonBoy-AutoUpdater/" + CurrentVersion;

				using (var resp = (HttpWebResponse)req.GetResponse())
				{
					totalBytes = resp.ContentLength;
					using (var stream = resp.GetResponseStream())
					using (var fs = new FileStream(tempFile, FileMode.Create, FileAccess.Write, FileShare.None, 65536, false))
					{
						ReadStreamToTarget(stream, fs);
					}
				}
#endif

				if (isDownloadCanceled)
				{
					try { if (File.Exists(tempFile)) File.Delete(tempFile); } catch { }
					return;
				}

				if (File.Exists(targetFile))
				{
					File.Delete(targetFile);
				}
				File.Move(tempFile, targetFile);

				isDownloading = false;
				downloadPercent = 100;

				OnDownloadCompleted(targetFile);
			}
			catch (Exception ex)
			{
				isDownloading = false;
				downloadPercent = 0;
				try { if (File.Exists(tempFile)) File.Delete(tempFile); } catch { }
				GameScr.info1?.addInfo("Tải thất bại: " + ex.Message, 0);
			}
		});
		thread.IsBackground = true;
		thread.Start();
	}

	private static void ReadStreamToTarget(Stream stream, Stream fs)
	{
		byte[] buffer = new byte[65536];
		int read;
		long lastCalcTime = mSystem.currentTimeMillis();
		long bytesSinceLastCalc = 0;

		while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
		{
			if (isDownloadCanceled) break;

			fs.Write(buffer, 0, read);
			downloadedBytes += read;
			bytesSinceLastCalc += read;

			if (totalBytes > 0)
			{
				downloadPercent = (int)((downloadedBytes * 100) / totalBytes);
				if (downloadPercent > 100) downloadPercent = 100;
			}

			long now = mSystem.currentTimeMillis();
			if (now - lastCalcTime >= 1000)
			{
				float speedKB = (bytesSinceLastCalc / 1024f) / ((now - lastCalcTime) / 1000f);
				if (speedKB >= 1024)
				{
					downloadSpeedStr = (speedKB / 1024f).ToString("0.0") + " MB/s";
				}
				else
				{
					downloadSpeedStr = speedKB.ToString("0") + " KB/s";
				}
				lastCalcTime = now;
				bytesSinceLastCalc = 0;
			}
		}
	}

	private static string GetTargetLocalPath()
	{
		bool isAndroid = (Application.platform == RuntimePlatform.Android) || (mSystem.clientType == 2);
		bool isIOS = (Application.platform == RuntimePlatform.IPhonePlayer) || (mSystem.clientType == 3 || mSystem.clientType == 5 || mSystem.clientType == 7);

		if (isAndroid)
		{
			return Path.Combine(Application.persistentDataPath, "DragonBoy250_Mod_Android.apk");
		}
		else if (isIOS)
		{
			return Path.Combine(Application.persistentDataPath, "DragonBoy_Mod_iOS.ipa");
		}
		else
		{
			string appDir = AppDomain.CurrentDomain.BaseDirectory;
			return Path.Combine(appDir, "DragonBoy_Net8_Native.exe.new");
		}
	}

	private static void OnDownloadCompleted(string localFile)
	{
		bool isAndroid = (Application.platform == RuntimePlatform.Android) || (mSystem.clientType == 2);
		bool isIOS = (Application.platform == RuntimePlatform.IPhonePlayer) || (mSystem.clientType == 3 || mSystem.clientType == 5 || mSystem.clientType == 7);

		if (isAndroid)
		{
			GameScr.info1?.addInfo("Tải xong APK! Đang mở trình cài đặt...", 0);
			try
			{
				Application.OpenURL("file://" + localFile);
			}
			catch
			{
				Application.OpenURL(downloadUrl);
			}
		}
		else if (isIOS)
		{
			GameScr.info1?.addInfo("Tải xong IPA! Đang mở TrollStore...", 0);
			try
			{
				Application.OpenURL("trollstore://install?url=" + localFile);
			}
			catch
			{
				Application.OpenURL(downloadUrl);
			}
		}
		else
		{
			GameScr.info1?.addInfo("Tải hoàn tất! Tự động khởi động lại game...", 0);
			ApplyUpdateAndRestart();
		}
	}

	public static void CancelDownload()
	{
		isDownloadCanceled = true;
		isDownloading = false;
		downloadPercent = 0;
		GameScr.info1?.addInfo("Đã hủy tải bản cập nhật!", 0);
	}

	public static void PaintDownloadProgress(mGraphics g)
	{
		if (!isDownloading) return;

		// Phủ mờ nền đen mờ
		g.setColor(0, 0.65f);
		g.fillRect(0, 0, GameCanvas.w, GameCanvas.h);

		// Khung hộp thoại chính giữa
		int boxW = 260;
		int boxH = 135;
		if (boxW > GameCanvas.w - 20) boxW = GameCanvas.w - 20;
		int boxX = (GameCanvas.w - boxW) / 2;
		int boxY = (GameCanvas.h - boxH) / 2;

		PopUp.paintPopUp(g, boxX, boxY, boxW, boxH, -1, isButton: true);

		// Tiêu đề
		int titleY = boxY + 12;
		mFont.tahoma_7b_yellow.drawString(g, "CẬP NHẬT TRỰC TIẾP", boxX + boxW / 2, titleY, mFont.CENTER);

		// Tên phiên bản & Tốc độ tải
		int infoY1 = titleY + 18;
		string verStr = "Đang tải bản v" + remoteVersion;
		if (!string.IsNullOrEmpty(downloadSpeedStr))
		{
			verStr += " (" + downloadSpeedStr + ")";
		}
		mFont.tahoma_7_white.drawString(g, verStr, boxX + boxW / 2, infoY1, mFont.CENTER);

		// Thông số dung lượng
		int infoY2 = infoY1 + 16;
		float downMB = downloadedBytes / (1024f * 1024f);
		float totMB = totalBytes / (1024f * 1024f);
		string sizeStr = downloadPercent + "% (" + downMB.ToString("0.0") + " MB / " + totMB.ToString("0.0") + " MB)";
		mFont.tahoma_7b_white.drawString(g, sizeStr, boxX + boxW / 2, infoY2, mFont.CENTER);

		// Thanh tiến trình phong cách NRO
		int barW = boxW - 40;
		if (barW > 200) barW = 200;
		int barH = 14;
		int barX = (GameCanvas.w - barW) / 2;
		int barY = infoY2 + 18;

		if (GameScr.frBarPow20 != null && GameScr.frBarPow21 != null && GameScr.frBarPow22 != null)
		{
			GameScr.paintOngMauPercent(GameScr.frBarPow20, GameScr.frBarPow21, GameScr.frBarPow22, barX, barY, barW, 100f, g);
			if (downloadPercent > 0)
			{
				GameScr.paintOngMauPercent(GameScr.frBarPow0, GameScr.frBarPow1, GameScr.frBarPow2, barX, barY, barW, downloadPercent, g);
			}
		}
		else
		{
			g.setColor(0x333333);
			g.fillRect(barX, barY, barW, barH);
			g.setColor(0x555555);
			g.drawRect(barX, barY, barW, barH);

			int fillW = (barW - 2) * downloadPercent / 100;
			if (fillW > 0)
			{
				g.setColor(0x00e676);
				g.fillRect(barX + 1, barY + 1, fillW, barH - 2);
			}
		}

		// Nút HỦY BỎ
		int btnW = 80;
		int btnH = 24;
		int btnX = (GameCanvas.w - btnW) / 2;
		int btnY = barY + 22;

		PopUp.paintPopUp(g, btnX, btnY, btnW, btnH, 0, isButton: true);
		mFont.tahoma_7b_white.drawString(g, "HỦY BỎ", btnX + btnW / 2, btnY + 5, mFont.CENTER);
	}

	public static void UpdateDownloadInput()
	{
		if (!isDownloading) return;

		int boxW = 260;
		int boxH = 135;
		if (boxW > GameCanvas.w - 20) boxW = GameCanvas.w - 20;
		int boxX = (GameCanvas.w - boxW) / 2;
		int boxY = (GameCanvas.h - boxH) / 2;

		int barW = boxW - 40;
		if (barW > 200) barW = 200;
		int barY = boxY + 68;

		int btnW = 80;
		int btnH = 24;
		int btnX = (GameCanvas.w - btnW) / 2;
		int btnY = barY + 22;

		if (GameCanvas.isPointerJustDown || GameCanvas.isPointerClick)
		{
			int px = GameCanvas.px;
			int py = GameCanvas.py;
			if (px >= btnX && px <= btnX + btnW && py >= btnY && py <= btnY + btnH)
			{
				CancelDownload();
				SoundMn.gI()?.buttonClick();
			}
		}

		if (GameCanvas.keyPressed[13] || GameCanvas.keyPressed[5])
		{
			GameCanvas.clearKeyPressed();
			CancelDownload();
		}
	}

	private static void ApplyUpdateAndRestart()
	{
		string appDir = AppDomain.CurrentDomain.BaseDirectory;
		string exeName = "DragonBoy_Net8_Native.exe";
		int pid = Process.GetCurrentProcess().Id;

		string batPath = Path.Combine(appDir, "apply_update.bat");
		string batContent = @"@echo off
chcp 65001 > nul
echo [UPDATER] Dang cho tien trinh game thoat...
timeout /t 1 /nobreak > nul
:wait_loop
tasklist /fi ""PID eq " + pid + @""" 2>nul | findstr /i """ + pid + @""" > nul
if not errorlevel 1 (
    timeout /t 1 /nobreak > nul
    goto wait_loop
)
echo [UPDATER] Tien trinh da thoat. Dang ghi de ban cap nhat moi...
move /y """ + Path.Combine(appDir, exeName + ".new") + @""" """ + Path.Combine(appDir, exeName) + @""" > nul
echo [UPDATER] Khoi dong lai DragonBoy Native AOT...
start """" """ + Path.Combine(appDir, exeName) + @"""
del ""%~f0""
";

		try
		{
			File.WriteAllText(batPath, batContent);
			Process.Start(new ProcessStartInfo
			{
				FileName = "cmd.exe",
				Arguments = "/c \"" + batPath + "\"",
				WorkingDirectory = appDir,
				UseShellExecute = true,
				CreateNoWindow = true
			});
			Environment.Exit(0);
		}
		catch (Exception ex)
		{
			GameScr.info1?.addInfo("Lỗi khởi động updater: " + ex.Message, 0);
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
		if (idAction == 1) // Cập nhật ngay -> Tải trực tiếp trong game
		{
			ModAutoUpdate.StartInGameDownload();
		}
		else if (idAction == 2) // Để sau
		{
			ModAutoUpdate.hasPrompted = true;
		}
	}
}
