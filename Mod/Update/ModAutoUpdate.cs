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
	public const string CurrentVersion = "2.5.13";
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
	public static bool isDownloadCompleted = false;
	public static bool isShowCompletedBoard = false;
	public static string downloadedFilePath = string.Empty;
	public static int downloadPercent = 0;
	public static long downloadedBytes = 0;
	public static long totalBytes = 0;
	public static string downloadSpeedStr = string.Empty;

	// State cho Nút Sảnh & Bảng Thông Tin Cập Nhật
	public static bool isShowUpdateBoard = false;
	public static int scrollY = 0;
	public static int maxScrollY = 0;
	private static int lastPointerY = 0;
	private static bool isDraggingScroll = false;

	public static int GetBtnW() => 72;
	public static int GetBtnH() => 22;
	public static int GetBtnX() => GameCanvas.w - GetBtnW() - 6;
	public static int GetBtnY() => 4;

	public static void ResetAndCheckOnLaunch()
	{
		isChecking = false;
		hasChecked = false;
		hasNewVersion = false;
		hasPrompted = false;
		isShowUpdateBoard = false;
		scrollY = 0;
		maxScrollY = 0;
		isDownloading = false;
		isDownloadCanceled = false;
		isDownloadCompleted = false;
		isShowCompletedBoard = false;
		downloadedFilePath = string.Empty;
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
				string manifestUrlWithCacheBust = ManifestUrl + "?t=" + mSystem.currentTimeMillis();
#if NET8_0_OR_GREATER
			using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10)))
			using (var client = new HttpClient())
			{
				client.Timeout = TimeSpan.FromSeconds(10);
				client.DefaultRequestHeaders.Add("User-Agent", "DragonBoy-AutoUpdater/" + CurrentVersion);
				var resp = client.GetAsync(manifestUrlWithCacheBust, cts.Token).GetAwaiter().GetResult();
				if (resp.IsSuccessStatusCode)
				{
					json = resp.Content.ReadAsStringAsync(cts.Token).GetAwaiter().GetResult();
				}
			}
#else
			using (var wc = new WebClient())
			{
				wc.Headers.Add("User-Agent", "DragonBoy-AutoUpdater/" + CurrentVersion);
				json = wc.DownloadString(manifestUrlWithCacheBust);
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
			hasPrompted = true;
			ShowUpdateDialog();
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
		scrollY = 0;
		isShowUpdateBoard = true;
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
			ShowUpdateDialog();
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
				isDownloadCompleted = true;
				isShowCompletedBoard = true;
				downloadPercent = 100;
				downloadedFilePath = targetFile;

				OnDownloadCompleted(targetFile);
			}
			catch (Exception ex)
			{
				isDownloading = false;
				isDownloadCompleted = false;
				isShowCompletedBoard = false;
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
			string currentExePath = null;
			try
			{
				currentExePath = Process.GetCurrentProcess().MainModule?.FileName;
			}
			catch { }
			string exeName = !string.IsNullOrEmpty(currentExePath) ? Path.GetFileName(currentExePath) : "DragonBoy_Net8_Native.exe";
			string appDir = !string.IsNullOrEmpty(currentExePath) ? Path.GetDirectoryName(currentExePath) : AppDomain.CurrentDomain.BaseDirectory;
			return Path.Combine(appDir, exeName + ".new");
		}
	}

	private static void OnDownloadCompleted(string localFile)
	{
		bool isAndroid = (Application.platform == RuntimePlatform.Android) || (mSystem.clientType == 2);
		bool isIOS = (Application.platform == RuntimePlatform.IPhonePlayer) || (mSystem.clientType == 3 || mSystem.clientType == 5 || mSystem.clientType == 7);

		if (isAndroid)
		{
			GameScr.info1?.addInfo("Tải xong APK! Đang mở trình cài đặt...", 0);
#if NET8_0_OR_GREATER
			if (Application.InstallApkHandler != null)
			{
				try
				{
					Application.InstallApkHandler(localFile);
					return;
				}
				catch { }
			}
#endif
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
		isDownloadCompleted = false;
		isShowCompletedBoard = false;
		downloadPercent = 0;
		GameScr.info1?.addInfo("Đã hủy tải bản cập nhật!", 0);
	}

	public static void PaintDownloadProgress(mGraphics g)
	{
		if (!isDownloading && !isShowCompletedBoard) return;

		// Phủ mờ nền đen nhẹ
		g.setColor(0, 0.6f);
		g.fillRect(0, 0, GameCanvas.w, GameCanvas.h);

		// Khung hộp thoại chính giữa
		int boxW = 260;
		int boxH = isShowCompletedBoard ? 145 : 135;
		if (boxW > GameCanvas.w - 20) boxW = GameCanvas.w - 20;
		int boxX = (GameCanvas.w - boxW) / 2;
		int boxY = (GameCanvas.h - boxH) / 2;

		// Khung nền giấy da be viền vàng hoàng kim NRO
		PopUp.paintPopUp(g, boxX, boxY, boxW, boxH, 0, isButton: true);

		if (isShowCompletedBoard)
		{
			// TIÊU ĐỀ HOÀN TẤT
			int titleY = boxY + 12;
			mFont.tahoma_7b_red.drawString(g, "CẬP NHẬT HOÀN TẤT", boxX + boxW / 2, titleY, mFont.CENTER);

			int infoY1 = titleY + 18;
			mFont.tahoma_7b_dark.drawString(g, "Đã tải xong bản v" + (string.IsNullOrEmpty(remoteVersion) ? CurrentVersion : remoteVersion) + " (100%)", boxX + boxW / 2, infoY1, mFont.CENTER);

			int infoY2 = infoY1 + 16;
			bool isAndroid = (Application.platform == RuntimePlatform.Android) || (mSystem.clientType == 2);
			bool isIOS = (Application.platform == RuntimePlatform.IPhonePlayer) || (mSystem.clientType == 3 || mSystem.clientType == 5 || mSystem.clientType == 7);

			if (isAndroid)
			{
				mFont.tahoma_7_green.drawString(g, "Đang mở trình cài đặt APK...", boxX + boxW / 2, infoY2, mFont.CENTER);
				mFont.tahoma_7b_dark.drawString(g, "Nếu chưa hiện, bấm 'CÀI ĐẶT' bên dưới.", boxX + boxW / 2, infoY2 + 14, mFont.CENTER);
			}
			else if (isIOS)
			{
				mFont.tahoma_7_green.drawString(g, "Đã tải xong file IPA!", boxX + boxW / 2, infoY2, mFont.CENTER);
				mFont.tahoma_7b_dark.drawString(g, "Bấm 'MỞ CÀI ĐẶT' để cài qua TrollStore.", boxX + boxW / 2, infoY2 + 14, mFont.CENTER);
			}
			else
			{
				mFont.tahoma_7_green.drawString(g, "Đang khởi động lại game...", boxX + boxW / 2, infoY2, mFont.CENTER);
				mFont.tahoma_7b_dark.drawString(g, "Nếu chưa tự tắt, bấm 'KHỞI ĐỘNG LẠI'.", boxX + boxW / 2, infoY2 + 14, mFont.CENTER);
			}

			// 2 Nút bấm: [ CÀI ĐẶT / KHỞI ĐỘNG LẠI ] và [ ĐÓNG ]
			int btnActionW = 96;
			int btnCloseW = 60;
			int totalBtnW = btnActionW + btnCloseW + 8;
			int btnActionX = boxX + (boxW - totalBtnW) / 2;
			int btnCloseX = btnActionX + btnActionW + 8;
			int btnY = boxY + boxH - 30;
			int btnH = 22;

			string actionLabel = isAndroid ? "CÀI ĐẶT" : (isIOS ? "MỞ CÀI ĐẶT" : "KHỞI ĐỘNG LẠI");
			PopUp.paintPopUp(g, btnActionX, btnY, btnActionW, btnH, 1, isButton: true);
			mFont.tahoma_7b_dark.drawString(g, actionLabel, btnActionX + btnActionW / 2, btnY + 4, mFont.CENTER);

			PopUp.paintPopUp(g, btnCloseX, btnY, btnCloseW, btnH, 0, isButton: true);
			mFont.tahoma_7b_dark.drawString(g, "ĐÓNG", btnCloseX + btnCloseW / 2, btnY + 4, mFont.CENTER);
		}
		else
		{
			// ĐANG TẢI TRỰC TIẾP
			int titleY = boxY + 12;
			mFont.tahoma_7b_red.drawString(g, "CẬP NHẬT TRỰC TIẾP", boxX + boxW / 2, titleY, mFont.CENTER);

			int infoY1 = titleY + 18;
			string verStr = "Đang tải bản v" + remoteVersion;
			if (!string.IsNullOrEmpty(downloadSpeedStr))
			{
				verStr += " (" + downloadSpeedStr + ")";
			}
			mFont.tahoma_7b_dark.drawString(g, verStr, boxX + boxW / 2, infoY1, mFont.CENTER);

			int infoY2 = infoY1 + 16;
			float downMB = downloadedBytes / (1024f * 1024f);
			float totMB = totalBytes / (1024f * 1024f);
			string sizeStr = downloadPercent + "% (" + downMB.ToString("0.0") + " MB / " + totMB.ToString("0.0") + " MB)";
			mFont.tahoma_7b_dark.drawString(g, sizeStr, boxX + boxW / 2, infoY2, mFont.CENTER);

			int barW = boxW - 40;
			if (barW > 200) barW = 200;
			int barH = 14;
			int barX = (GameCanvas.w - barW) / 2;
			int barY = infoY2 + 18;

			if (GameScr.frBarPow20 != null && GameScr.frBarPow21 != null && GameScr.frBarPow22 != null &&
			    GameScr.frBarPow0 != null && GameScr.frBarPow1 != null && GameScr.frBarPow2 != null)
			{
				// Nền rãnh thanh nạp (vàng/cam NRO): hiển thị trọn vẹn 100% chiều dài barW (pixelPercent = barW)
				GameScr.paintOngMauPercent(GameScr.frBarPow20, GameScr.frBarPow21, GameScr.frBarPow22, barX, barY, barW, barW, g);
				if (downloadPercent > 0)
				{
					// Thanh tiến độ nạp (xanh lục NRO): hiển thị chính xác theo tỉ lệ pixel thực tế
					float fillW = (float)downloadPercent * (float)barW / 100f;
					if (fillW > barW) fillW = barW;
					GameScr.paintOngMauPercent(GameScr.frBarPow0, GameScr.frBarPow1, GameScr.frBarPow2, barX, barY, barW, fillW, g);
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
			int btnH = 22;
			int btnX = (GameCanvas.w - btnW) / 2;
			int btnY = barY + 22;

			PopUp.paintPopUp(g, btnX, btnY, btnW, btnH, 0, isButton: true);
			mFont.tahoma_7b_dark.drawString(g, "HỦY BỎ", btnX + btnW / 2, btnY + 4, mFont.CENTER);
		}
	}

	public static void UpdateDownloadInput()
	{
		if (!isDownloading && !isShowCompletedBoard) return;

		int boxW = 260;
		int boxH = isShowCompletedBoard ? 145 : 135;
		if (boxW > GameCanvas.w - 20) boxW = GameCanvas.w - 20;
		int boxX = (GameCanvas.w - boxW) / 2;
		int boxY = (GameCanvas.h - boxH) / 2;

		if (isShowCompletedBoard)
		{
			int btnActionW = 96;
			int btnCloseW = 60;
			int totalBtnW = btnActionW + btnCloseW + 8;
			int btnActionX = boxX + (boxW - totalBtnW) / 2;
			int btnCloseX = btnActionX + btnActionW + 8;
			int btnY = boxY + boxH - 30;
			int btnH = 22;

			if (GameCanvas.isPointerJustDown || GameCanvas.isPointerClick)
			{
				int px = GameCanvas.px;
				int py = GameCanvas.py;

				// Click nút Action (Cài đặt / Khởi động lại)
				if (px >= btnActionX && px <= btnActionX + btnActionW && py >= btnY && py <= btnY + btnH)
				{
					GameCanvas.isPointerClick = false;
					SoundMn.gI()?.buttonClick();
					string file = !string.IsNullOrEmpty(downloadedFilePath) ? downloadedFilePath : GetTargetLocalPath();
					OnDownloadCompleted(file);
					return;
				}

				// Click nút Đóng
				if (px >= btnCloseX && px <= btnCloseX + btnCloseW && py >= btnY && py <= btnY + btnH)
				{
					GameCanvas.isPointerClick = false;
					SoundMn.gI()?.buttonClick();
					isShowCompletedBoard = false;
					return;
				}
			}

			if (GameCanvas.keyPressed[13] || GameCanvas.keyPressed[5])
			{
				GameCanvas.clearKeyPressed();
				isShowCompletedBoard = false;
			}
			return;
		}

		// Khi đang tải:
		int barW = boxW - 40;
		if (barW > 200) barW = 200;
		int barY = boxY + 68;

		int btnCancelW = 80;
		int btnCancelH = 22;
		int btnCancelX = (GameCanvas.w - btnCancelW) / 2;
		int btnCancelY = barY + 22;

		if (GameCanvas.isPointerJustDown || GameCanvas.isPointerClick)
		{
			int px = GameCanvas.px;
			int py = GameCanvas.py;
			if (px >= btnCancelX && px <= btnCancelX + btnCancelW && py >= btnCancelY && py <= btnCancelY + btnCancelH)
			{
				GameCanvas.isPointerClick = false;
				CancelDownload();
				SoundMn.gI()?.buttonClick();
				return;
			}
		}

		if (GameCanvas.keyPressed[13] || GameCanvas.keyPressed[5])
		{
			GameCanvas.clearKeyPressed();
			CancelDownload();
		}
	}

	public static void PaintLobbyUI(mGraphics g)
	{
		PaintLobbyUpdateButton(g);
		PaintUpdateInfoBoard(g);
	}

	public static void PaintLobbyUpdateButton(mGraphics g)
	{
		if (isDownloading || isShowCompletedBoard || isShowUpdateBoard) return;

		int btnW = GetBtnW();
		int btnH = GetBtnH();
		int btnX = GetBtnX();
		int btnY = GetBtnY();

		if (isDownloadCompleted)
		{
			// Nút CÀI ĐẶT màu vàng rực rỡ với chấm đỏ nhấp nháy
			PopUp.paintPopUp(g, btnX, btnY, btnW, btnH, 1, isButton: true);
			mFont.tahoma_7b_dark.drawString(g, "CÀI ĐẶT", btnX + btnW / 2, btnY + 4, mFont.CENTER);

			// Chấm Sáng Đỏ nhấp nháy
			PaintRedNotificationDot(g, btnX + btnW - 3, btnY - 2);
		}
		else if (hasNewVersion)
		{
			// Nút nổi viền vàng rực NRO (Style 1: active button)
			PopUp.paintPopUp(g, btnX, btnY, btnW, btnH, 1, isButton: true);
			mFont.tahoma_7b_dark.drawString(g, "CẬP NHẬT", btnX + btnW / 2, btnY + 4, mFont.CENTER);

			// Chấm Sáng Đỏ nhấp nháy thu hút sự chú ý
			PaintRedNotificationDot(g, btnX + btnW - 3, btnY - 2);
		}
		else
		{
			// Nút phiên bản chuẩn NRO (Style 0: normal button)
			PopUp.paintPopUp(g, btnX, btnY, btnW, btnH, 0, isButton: true);
			mFont.tahoma_7b_dark.drawString(g, "v" + CurrentVersion, btnX + btnW / 2, btnY + 4, mFont.CENTER);
		}
	}

	private static void PaintRedNotificationDot(mGraphics g, int dotX, int dotY)
	{
		int pulse = (int)(System.Math.Sin(GameCanvas.gameTick * 0.25) * 2);
		int haloRadius = 7 + pulse;
		if (haloRadius < 5) haloRadius = 5;

		g.setColor(0xff1744, 0.45f);
		g.fillRoundRect(dotX - haloRadius / 2, dotY - haloRadius / 2, haloRadius, haloRadius, haloRadius, haloRadius);

		g.setColor(0xd50000);
		g.fillRoundRect(dotX - 3, dotY - 3, 6, 6, 6, 6);

		g.setColor(0xffffff);
		g.fillRect(dotX - 1, dotY - 2, 2, 1);
	}

	public static void PaintUpdateInfoBoard(mGraphics g)
	{
		if (!isShowUpdateBoard || isDownloading || isShowCompletedBoard || !hasNewVersion) return;

		// Phủ nền mờ tối toàn màn hình
		g.setColor(0, 0.7f);
		g.fillRect(0, 0, GameCanvas.w, GameCanvas.h);

		// Kích thước bảng
		int boxW = 300;
		int boxH = 195;
		if (boxW > GameCanvas.w - 16) boxW = GameCanvas.w - 16;
		if (boxH > GameCanvas.h - 16) boxH = GameCanvas.h - 16;
		int boxX = (GameCanvas.w - boxW) / 2;
		int boxY = (GameCanvas.h - boxH) / 2;

		// Khung hộp thoại nền giấy da be viền vàng hoàng kim NRO
		PopUp.paintPopUp(g, boxX, boxY, boxW, boxH, 0, isButton: true);

		// 1. Tiêu đề
		mFont.tahoma_7b_red.drawString(g, "THÔNG TIN CẬP NHẬT", boxX + boxW / 2, boxY + 10, mFont.CENTER);

		// Nút [X] đóng nhanh góc trên bên phải
		int closeX = boxX + boxW - 22;
		int closeY = boxY + 6;
		PopUp.paintPopUp(g, closeX, closeY, 16, 16, 0, isButton: true);
		mFont.tahoma_7b_dark.drawString(g, "X", closeX + 8, closeY + 2, mFont.CENTER);

		// 2. Dòng thông tin phiên bản
		int infoY = boxY + 28;
		if (hasNewVersion)
		{
			mFont.tahoma_7b_dark.drawString(g, "Bản mới: v" + remoteVersion, boxX + 14, infoY, mFont.LEFT);
			mFont.tahoma_7_grey.drawString(g, "Hiện tại: v" + CurrentVersion, boxX + boxW - 14, infoY, mFont.RIGHT);
			if (!string.IsNullOrEmpty(buildDate))
			{
				mFont.tahoma_7b_dark.drawString(g, "Ngày phát hành: " + buildDate, boxX + 14, infoY + 14, mFont.LEFT);
			}
		}
		else
		{
			mFont.tahoma_7b_dark.drawString(g, "Phiên bản hiện tại: v" + CurrentVersion, boxX + 14, infoY, mFont.LEFT);
			mFont.tahoma_7_green.drawString(g, "Bạn đang sử dụng phiên bản mới nhất!", boxX + 14, infoY + 14, mFont.LEFT);
		}

		// 3. Khung nội dung chi tiết (Changelog Box)
		int contentY = infoY + 30;
		int contentW = boxW - 24;
		int contentH = boxH - (contentY - boxY) - 42;
		if (contentH < 40) contentH = 40;

		// Nền tối sắc nét cho khung văn bản (gỗ sẫm viền nâu da)
		g.setColor(0x24150c);
		g.fillRect(boxX + 12, contentY, contentW, contentH);
		g.setColor(0x8d6e63);
		g.drawRect(boxX + 12, contentY, contentW, contentH);

		// Clip và vẽ các dòng văn bản
		g.setClip(boxX + 14, contentY + 3, contentW - 4, contentH - 6);

		string textToDisplay = changelog;
		if (string.IsNullOrEmpty(textToDisplay))
		{
			textToDisplay = hasNewVersion 
				? ("Phát hiện bản cập nhật mới v" + remoteVersion + "! Hãy bấm 'Cập nhật' để tải trực tiếp bản mới nhất.")
				: ("Bạn đang sử dụng phiên bản mới nhất của Mod DragonBoy. Không có bản cập nhật nào mới hơn.");
		}

		string[] lines = mFont.tahoma_7_white.splitFontArray(textToDisplay, contentW - 12);
		int lineHeight = mFont.tahoma_7_white.getHeight() + 2;
		int totalTextHeight = lines.Length * lineHeight;
		maxScrollY = System.Math.Max(0, totalTextHeight - (contentH - 6));
		if (scrollY > maxScrollY) scrollY = maxScrollY;
		if (scrollY < 0) scrollY = 0;

		int textY = contentY + 4 - scrollY;
		for (int i = 0; i < lines.Length; i++)
		{
			if (textY + lineHeight >= contentY && textY <= contentY + contentH)
			{
				mFont.tahoma_7_white.drawString(g, lines[i], boxX + 16, textY, mFont.LEFT);
			}
			textY += lineHeight;
		}
		g.setClip(0, 0, GameCanvas.w, GameCanvas.h);

		// 4. Các nút thao tác bên dưới
		int btnY = boxY + boxH - 32;
		int btnW = 86;
		int btnH = 24;

		if (hasNewVersion)
		{
			int btnUpdateX = boxX + boxW / 2 - btnW - 8;
			int btnCloseX = boxX + boxW / 2 + 8;

			// Nút CẬP NHẬT
			PopUp.paintPopUp(g, btnUpdateX, btnY, btnW, btnH, 1, isButton: true);
			mFont.tahoma_7b_dark.drawString(g, "CẬP NHẬT", btnUpdateX + btnW / 2, btnY + 5, mFont.CENTER);

			// Nút ĐÓNG
			PopUp.paintPopUp(g, btnCloseX, btnY, btnW, btnH, 0, isButton: true);
			mFont.tahoma_7b_dark.drawString(g, "ĐÓNG", btnCloseX + btnW / 2, btnY + 5, mFont.CENTER);
		}
		else
		{
			// Nút ĐÓNG chính giữa
			int btnCloseX = (GameCanvas.w - btnW) / 2;
			PopUp.paintPopUp(g, btnCloseX, btnY, btnW, btnH, 0, isButton: true);
			mFont.tahoma_7b_dark.drawString(g, "ĐÓNG", btnCloseX + btnW / 2, btnY + 5, mFont.CENTER);
		}
	}

	public static void UpdateLobbyInput()
	{
		if (isDownloading || isShowCompletedBoard)
		{
			UpdateDownloadInput();
			return;
		}

		int btnW = GetBtnW();
		int btnH = GetBtnH();
		int btnX = GetBtnX();
		int btnY = GetBtnY();

		// 1. Khi đang mở Bảng Thông Tin Cập Nhật
		if (isShowUpdateBoard)
		{
			int boxW = 300;
			int boxH = 195;
			if (boxW > GameCanvas.w - 16) boxW = GameCanvas.w - 16;
			if (boxH > GameCanvas.h - 16) boxH = GameCanvas.h - 16;
			int boxX = (GameCanvas.w - boxW) / 2;
			int boxY = (GameCanvas.h - boxH) / 2;

			int closeX = boxX + boxW - 22;
			int closeY = boxY + 6;

			int btnActionY = boxY + boxH - 32;
			int btnActionW = 86;
			int btnActionH = 24;

			int btnUpdateX = boxX + boxW / 2 - btnActionW - 8;
			int btnCloseX = hasNewVersion ? (boxX + boxW / 2 + 8) : ((GameCanvas.w - btnActionW) / 2);

			int contentY = boxY + 58;
			int contentW = boxW - 24;
			int contentH = boxH - (contentY - boxY) - 42;

			// Xử lý vuốt cuộn cảm ứng hoặc kéo chuột
			if (GameCanvas.isPointerJustDown)
			{
				lastPointerY = GameCanvas.py;
				if (GameCanvas.px >= boxX + 12 && GameCanvas.px <= boxX + 12 + contentW &&
					GameCanvas.py >= contentY && GameCanvas.py <= contentY + contentH)
				{
					isDraggingScroll = true;
				}
			}
			else if (GameCanvas.isPointerMove && isDraggingScroll)
			{
				int deltaY = GameCanvas.py - lastPointerY;
				scrollY -= deltaY;
				if (scrollY < 0) scrollY = 0;
				if (scrollY > maxScrollY) scrollY = maxScrollY;
				lastPointerY = GameCanvas.py;
			}

			if (GameCanvas.isPointerClick)
			{
				isDraggingScroll = false;
				int px = GameCanvas.px;
				int py = GameCanvas.py;

				// Click nút [X] đóng
				if (px >= closeX && px <= closeX + 16 && py >= closeY && py <= closeY + 16)
				{
					isShowUpdateBoard = false;
					SoundMn.gI()?.buttonClick();
					GameCanvas.isPointerClick = false;
					return;
				}

				// Click nút [ ĐÓNG ]
				if (px >= btnCloseX && px <= btnCloseX + btnActionW && py >= btnActionY && py <= btnActionY + btnActionH)
				{
					isShowUpdateBoard = false;
					SoundMn.gI()?.buttonClick();
					GameCanvas.isPointerClick = false;
					return;
				}

				// Click nút [ CẬP NHẬT ]
				if (hasNewVersion && px >= btnUpdateX && px <= btnUpdateX + btnActionW && py >= btnActionY && py <= btnActionY + btnActionH)
				{
					isShowUpdateBoard = false;
					SoundMn.gI()?.buttonClick();
					StartInGameDownload();
					GameCanvas.isPointerClick = false;
					return;
				}

				// Click ra ngoài vùng bảng -> Đóng bảng
				if (px < boxX || px > boxX + boxW || py < boxY || py > boxY + boxH)
				{
					isShowUpdateBoard = false;
					GameCanvas.isPointerClick = false;
					return;
				}
			}

			// Phím Back / ESC / Đóng
			if (GameCanvas.keyPressed[13] || GameCanvas.keyPressed[5])
			{
				GameCanvas.clearKeyPressed();
				isShowUpdateBoard = false;
			}
			return;
		}

		// 2. Khi ở sảnh game (chưa mở bảng), xử lý click nút ở góc trên bên phải
		if (GameCanvas.isPointerClick)
		{
			int px = GameCanvas.px;
			int py = GameCanvas.py;
			if (px >= btnX - 4 && px <= btnX + btnW + 6 && py >= btnY - 4 && py <= btnY + btnH + 6)
			{
				GameCanvas.isPointerClick = false;
				SoundMn.gI()?.buttonClick();
				if (isDownloadCompleted)
				{
					isShowCompletedBoard = true;
				}
				else if (hasNewVersion)
				{
					scrollY = 0;
					isShowUpdateBoard = true;
				}
				else if (isChecking)
				{
					GameCanvas.startOK("Đang kết nối GitHub kiểm tra cập nhật...", 8882, null);
				}
				else if (hasChecked)
				{
					GameCanvas.startOK("Bạn đang ở phiên bản mới nhất (v" + CurrentVersion + ").\nĐã đồng bộ dữ liệu GitHub!", 8882, null);
				}
				else
				{
					CheckManual();
				}
			}
		}
	}

	public static void ApplyUpdateAndRestart()
	{
		string currentExePath = null;
		try
		{
			currentExePath = Process.GetCurrentProcess().MainModule?.FileName;
		}
		catch { }

		string exeName = !string.IsNullOrEmpty(currentExePath) ? Path.GetFileName(currentExePath) : "DragonBoy_Net8_Native.exe";
		string appDir = !string.IsNullOrEmpty(currentExePath) ? Path.GetDirectoryName(currentExePath) : AppDomain.CurrentDomain.BaseDirectory;
		int pid = Process.GetCurrentProcess().Id;

		string newExePath = Path.Combine(appDir, exeName + ".new");
		if (!File.Exists(newExePath))
		{
			string altPath = Path.Combine(appDir, "DragonBoy_Net8_Native.exe.new");
			if (File.Exists(altPath))
			{
				newExePath = altPath;
			}
		}

		string targetExePath = Path.Combine(appDir, exeName);
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
move /y """ + newExePath + @""" """ + targetExePath + @""" > nul
echo [UPDATER] Khoi dong lai game...
start """" """ + targetExePath + @"""
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
