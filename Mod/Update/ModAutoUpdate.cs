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

/// <summary>
/// Module Tá»± Äá»™ng Cáº­p Nháº­t: Kiá»ƒm tra phiÃªn báº£n má»›i tá»« mÃ¡y chá»§ GitHub Release vÃ  táº£i vá» báº£n vÃ¡ tá»± Ä‘á»™ng.
/// </summary>
public static class ModAutoUpdate
{
	public static readonly string CurrentVersion = "2.5.57";
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

	// State cho NÃºt Sáº£nh & Báº£ng ThÃ´ng Tin Cáº­p Nháº­t
	public static bool isShowUpdateBoard = false;
	public static int scrollY = 0;
	public static int maxScrollY = 0;
	private static int lastPointerY = 0;
	private static bool isDraggingScroll = false;

	public static int GetBtnW() => 72;
	public static int GetBtnH() => 22;
	public static int GetBtnX() => GameCanvas.w - GetBtnW() - 6;
	public static int GetBtnY() => 4;

	/// <summary>
	/// Dá»n dáº¹p vÃ  Ä‘áº·t láº¡i tráº¡ng thÃ¡i ban Ä‘áº§u cho ResetAndCheckOnLaunch.
	/// </summary>
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

	/// <summary>
	/// Thá»±c thi phÆ°Æ¡ng thá»©c nghiá»‡p vá»¥ StartCheckAsync.
	/// </summary>
	public static void StartCheckAsync(bool force = false, bool isManual = false)
	{
#if NET8_0_OR_GREATER
		if (OperatingSystem.IsBrowser())
		{
			hasChecked = true;
			isChecking = false;
			if (isManual)
			{
				manualStatusMsg = "PhiÃªn báº£n Web tá»± Ä‘á»™ng cáº­p nháº­t trá»±c tiáº¿p tá»« mÃ¡y chá»§!";
			}
			return;
		}
#endif
		if (isChecking) return;
		if (!force && hasChecked) return;

		isChecking = true;
		if (isManual)
		{
			manualStatusMsg = "Äang kiá»ƒm tra cáº­p nháº­t...";
		}

		/// <summary>
		/// Thá»±c thi phÆ°Æ¡ng thá»©c nghiá»‡p vá»¥ Thread.
		/// </summary>
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
					manualStatusMsg = "Kiá»ƒm tra tháº¥t báº¡i: " + ex.Message;
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

	/// <summary>
	/// Thá»±c thi phÆ°Æ¡ng thá»©c nghiá»‡p vá»¥ PerformCheck.
	/// </summary>
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
				manualStatusMsg = "KhÃ´ng thá»ƒ káº¿t ná»‘i mÃ¡y chá»§ cáº­p nháº­t (Timeout/Máº¥t máº¡ng).";
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
				manualStatusMsg = "PhÃ¡t hiá»‡n báº£n má»›i v" + remoteVersion + "!";
			}
		}
		else
		{
			hasNewVersion = false;
			if (isManual)
			{
				manualStatusMsg = "Báº¡n Ä‘ang dÃ¹ng báº£n má»›i nháº¥t (v" + CurrentVersion + ").";
			}
		}
	}

	/// <summary>
	/// Thá»±c thi phÆ°Æ¡ng thá»©c nghiá»‡p vá»¥ SelectDownloadUrl.
	/// </summary>
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

	/// <summary>
	/// Cáº­p nháº­t tráº¡ng thÃ¡i logic cá»§a UpdateTick theo chu ká»³ khung hÃ¬nh.
	/// </summary>
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

	/// <summary>
	/// Thá»±c thi phÆ°Æ¡ng thá»©c nghiá»‡p vá»¥ ShowUpdateDialog.
	/// </summary>
	public static void ShowUpdateDialog()
	{
		scrollY = 0;
		isShowUpdateBoard = true;
	}

	/// <summary>
	/// Kiá»ƒm tra Ä‘iá»u kiá»‡n há»£p lá»‡ hoáº·c tráº¡ng thÃ¡i cá»§a CheckManual.
	/// </summary>
	public static void CheckManual()
	{
		if (isChecking)
		{
			GameCanvas.startOK("Äang kiá»ƒm tra cáº­p nháº­t, vui lÃ²ng chá»...", 8882, null);
			return;
		}
		if (hasChecked)
		{
			ShowUpdateDialog();
		}
		else
		{
			GameCanvas.startOK("Äang káº¿t ná»‘i GitHub kiá»ƒm tra cáº­p nháº­t...", 8882, null);
			StartCheckAsync(force: true, isManual: true);
		}
	}

	/// <summary>
	/// Thá»±c thi phÆ°Æ¡ng thá»©c nghiá»‡p vá»¥ StartInGameDownload.
	/// </summary>
	public static void StartInGameDownload()
	{
		if (isDownloading) return;
		if (string.IsNullOrEmpty(downloadUrl))
		{
			GameScr.info1?.addInfo("KhÃ´ng tÃ¬m tháº¥y Ä‘Æ°á»ng dáº«n táº£i báº£n cáº­p nháº­t!", 0);
			return;
		}

		isDownloading = true;
		isDownloadCanceled = false;
		downloadPercent = 0;
		downloadedBytes = 0;
		totalBytes = 0;
		downloadSpeedStr = "0 KB/s";

		/// <summary>
		/// Thá»±c thi phÆ°Æ¡ng thá»©c nghiá»‡p vá»¥ Thread.
		/// </summary>
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
				GameScr.info1?.addInfo("Táº£i tháº¥t báº¡i: " + ex.Message, 0);
			}
		});
		thread.IsBackground = true;
		thread.Start();
	}

	/// <summary>
	/// Thá»±c thi phÆ°Æ¡ng thá»©c nghiá»‡p vá»¥ ReadStreamToTarget.
	/// </summary>
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

	/// <summary>
	/// Truy xuáº¥t vÃ  tráº£ vá» dá»¯ liá»‡u cá»§a GetTargetLocalPath.
	/// </summary>
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

	/// <summary>
	/// Bá»™ xá»­ lÃ½ sá»± kiá»‡n tiáº¿p nháº­n vÃ  pháº£n há»“i OnDownloadCompleted.
	/// </summary>
	private static void OnDownloadCompleted(string localFile)
	{
		bool isAndroid = (Application.platform == RuntimePlatform.Android) || (mSystem.clientType == 2);
		bool isIOS = (Application.platform == RuntimePlatform.IPhonePlayer) || (mSystem.clientType == 3 || mSystem.clientType == 5 || mSystem.clientType == 7);

		if (isAndroid)
		{
			GameScr.info1?.addInfo("Táº£i xong APK! Äang má»Ÿ trÃ¬nh cÃ i Ä‘áº·t...", 0);
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
			GameScr.info1?.addInfo("Táº£i xong IPA! Äang má»Ÿ TrollStore...", 0);
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
			GameScr.info1?.addInfo("Táº£i hoÃ n táº¥t! Tá»± Ä‘á»™ng khá»Ÿi Ä‘á»™ng láº¡i game...", 0);
			ApplyUpdateAndRestart();
		}
	}

	/// <summary>
	/// Kiá»ƒm tra Ä‘iá»u kiá»‡n há»£p lá»‡ hoáº·c tráº¡ng thÃ¡i cá»§a CancelDownload.
	/// </summary>
	public static void CancelDownload()
	{
		isDownloadCanceled = true;
		isDownloading = false;
		isDownloadCompleted = false;
		isShowCompletedBoard = false;
		downloadPercent = 0;
		GameScr.info1?.addInfo("ÄÃ£ há»§y táº£i báº£n cáº­p nháº­t!", 0);
	}

	/// <summary>
	/// Thá»±c hiá»‡n váº½ vÃ  hiá»ƒn thá»‹ thÃ nh pháº§n PaintDownloadProgress lÃªn canvas Ä‘á»“ há»a.
	/// </summary>
	public static void PaintDownloadProgress(mGraphics g)
	{
		if (!isDownloading && !isShowCompletedBoard) return;

		// Phá»§ má» ná»n Ä‘en nháº¹
		g.setColor(0, 0.6f);
		g.fillRect(0, 0, GameCanvas.w, GameCanvas.h);

		// Khung há»™p thoáº¡i chÃ­nh giá»¯a
		int boxW = 260;
		int boxH = isShowCompletedBoard ? 145 : 135;
		if (boxW > GameCanvas.w - 20) boxW = GameCanvas.w - 20;
		int boxX = (GameCanvas.w - boxW) / 2;
		int boxY = (GameCanvas.h - boxH) / 2;

		// Khung ná»n giáº¥y da be viá»n vÃ ng hoÃ ng kim NRO
		PopUp.paintPopUp(g, boxX, boxY, boxW, boxH, 0, isButton: true);

		if (isShowCompletedBoard)
		{
			// TIÃŠU Äá»€ HOÃ€N Táº¤T
			int titleY = boxY + 12;
			mFont.tahoma_7b_red.drawString(g, "Cáº¬P NHáº¬T HOÃ€N Táº¤T", boxX + boxW / 2, titleY, mFont.CENTER);

			int infoY1 = titleY + 18;
			mFont.tahoma_7b_dark.drawString(g, "ÄÃ£ táº£i xong báº£n v" + (string.IsNullOrEmpty(remoteVersion) ? CurrentVersion : remoteVersion) + " (100%)", boxX + boxW / 2, infoY1, mFont.CENTER);

			int infoY2 = infoY1 + 16;
			bool isAndroid = (Application.platform == RuntimePlatform.Android) || (mSystem.clientType == 2);
			bool isIOS = (Application.platform == RuntimePlatform.IPhonePlayer) || (mSystem.clientType == 3 || mSystem.clientType == 5 || mSystem.clientType == 7);

			if (isAndroid)
			{
				mFont.tahoma_7_green.drawString(g, "Äang má»Ÿ trÃ¬nh cÃ i Ä‘áº·t APK...", boxX + boxW / 2, infoY2, mFont.CENTER);
				mFont.tahoma_7b_dark.drawString(g, "Náº¿u chÆ°a hiá»‡n, báº¥m 'CÃ€I Äáº¶T' bÃªn dÆ°á»›i.", boxX + boxW / 2, infoY2 + 14, mFont.CENTER);
			}
			else if (isIOS)
			{
				mFont.tahoma_7_green.drawString(g, "ÄÃ£ táº£i xong file IPA!", boxX + boxW / 2, infoY2, mFont.CENTER);
				mFont.tahoma_7b_dark.drawString(g, "Báº¥m 'Má»ž CÃ€I Äáº¶T' Ä‘á»ƒ cÃ i qua TrollStore.", boxX + boxW / 2, infoY2 + 14, mFont.CENTER);
			}
			else
			{
				mFont.tahoma_7_green.drawString(g, "Äang khá»Ÿi Ä‘á»™ng láº¡i game...", boxX + boxW / 2, infoY2, mFont.CENTER);
				mFont.tahoma_7b_dark.drawString(g, "Náº¿u chÆ°a tá»± táº¯t, báº¥m 'KHá»žI Äá»˜NG Láº I'.", boxX + boxW / 2, infoY2 + 14, mFont.CENTER);
			}

			// 2 NÃºt báº¥m: [ CÃ€I Äáº¶T / KHá»žI Äá»˜NG Láº I ] vÃ  [ ÄÃ“NG ]
			int btnActionW = 96;
			int btnCloseW = 60;
			int totalBtnW = btnActionW + btnCloseW + 8;
			int btnActionX = boxX + (boxW - totalBtnW) / 2;
			int btnCloseX = btnActionX + btnActionW + 8;
			int btnY = boxY + boxH - 30;
			int btnH = 22;

			string actionLabel = isAndroid ? "CÃ€I Äáº¶T" : (isIOS ? "Má»ž CÃ€I Äáº¶T" : "KHá»žI Äá»˜NG Láº I");
			PopUp.paintPopUp(g, btnActionX, btnY, btnActionW, btnH, 1, isButton: true);
			mFont.tahoma_7b_dark.drawString(g, actionLabel, btnActionX + btnActionW / 2, btnY + 4, mFont.CENTER);

			PopUp.paintPopUp(g, btnCloseX, btnY, btnCloseW, btnH, 0, isButton: true);
			mFont.tahoma_7b_dark.drawString(g, "ÄÃ“NG", btnCloseX + btnCloseW / 2, btnY + 4, mFont.CENTER);
		}
		else
		{
			// ÄANG Táº¢I TRá»°C TIáº¾P
			int titleY = boxY + 12;
			mFont.tahoma_7b_red.drawString(g, "Cáº¬P NHáº¬T TRá»°C TIáº¾P", boxX + boxW / 2, titleY, mFont.CENTER);

			int infoY1 = titleY + 18;
			string verStr = "Äang táº£i báº£n v" + remoteVersion;
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
				// Ná»n rÃ£nh thanh náº¡p (vÃ ng/cam NRO): hiá»ƒn thá»‹ trá»n váº¹n 100% chiá»u dÃ i barW (pixelPercent = barW)
				GameScr.paintOngMauPercent(GameScr.frBarPow20, GameScr.frBarPow21, GameScr.frBarPow22, barX, barY, barW, barW, g);
				if (downloadPercent > 0)
				{
					// Thanh tiáº¿n Ä‘á»™ náº¡p (xanh lá»¥c NRO): hiá»ƒn thá»‹ chÃ­nh xÃ¡c theo tá»‰ lá»‡ pixel thá»±c táº¿
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

			// NÃºt Há»¦Y Bá»Ž
			int btnW = 80;
			int btnH = 22;
			int btnX = (GameCanvas.w - btnW) / 2;
			int btnY = barY + 22;

			PopUp.paintPopUp(g, btnX, btnY, btnW, btnH, 0, isButton: true);
			mFont.tahoma_7b_dark.drawString(g, "Há»¦Y Bá»Ž", btnX + btnW / 2, btnY + 4, mFont.CENTER);
		}
	}

	/// <summary>
	/// Cáº­p nháº­t tráº¡ng thÃ¡i logic cá»§a UpdateDownloadInput theo chu ká»³ khung hÃ¬nh.
	/// </summary>
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

				// Click nÃºt Action (CÃ i Ä‘áº·t / Khá»Ÿi Ä‘á»™ng láº¡i)
				if (px >= btnActionX && px <= btnActionX + btnActionW && py >= btnY && py <= btnY + btnH)
				{
					GameCanvas.isPointerClick = false;
					SoundMn.gI()?.buttonClick();
					string file = !string.IsNullOrEmpty(downloadedFilePath) ? downloadedFilePath : GetTargetLocalPath();
					OnDownloadCompleted(file);
					return;
				}

				// Click nÃºt ÄÃ³ng
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

		// Khi Ä‘ang táº£i:
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

	/// <summary>
	/// Thá»±c hiá»‡n váº½ vÃ  hiá»ƒn thá»‹ thÃ nh pháº§n PaintLobbyUI lÃªn canvas Ä‘á»“ há»a.
	/// </summary>
	public static void PaintLobbyUI(mGraphics g)
	{
		PaintLobbyUpdateButton(g);
		PaintUpdateInfoBoard(g);
	}

	/// <summary>
	/// Thá»±c hiá»‡n váº½ vÃ  hiá»ƒn thá»‹ thÃ nh pháº§n PaintLobbyUpdateButton lÃªn canvas Ä‘á»“ há»a.
	/// </summary>
	public static void PaintLobbyUpdateButton(mGraphics g)
	{
		if (isDownloading || isShowCompletedBoard || isShowUpdateBoard) return;
		if (!isDownloadCompleted && !hasNewVersion) return;

		int btnW = GetBtnW();
		int btnH = GetBtnH();
		int btnX = GetBtnX();
		int btnY = GetBtnY();

		if (isDownloadCompleted)
		{
			// NÃºt CÃ€I Äáº¶T mÃ u vÃ ng rá»±c rá»¡ vá»›i cháº¥m Ä‘á» nháº¥p nhÃ¡y
			PopUp.paintPopUp(g, btnX, btnY, btnW, btnH, 1, isButton: true);
			mFont.tahoma_7b_dark.drawString(g, "CÃ€I Äáº¶T", btnX + btnW / 2, btnY + 4, mFont.CENTER);

			// Cháº¥m SÃ¡ng Äá» nháº¥p nhÃ¡y
			PaintRedNotificationDot(g, btnX + btnW - 3, btnY - 2);
		}
		else if (hasNewVersion)
		{
			// NÃºt ná»•i viá»n vÃ ng rá»±c NRO (Style 1: active button)
			PopUp.paintPopUp(g, btnX, btnY, btnW, btnH, 1, isButton: true);
			mFont.tahoma_7b_dark.drawString(g, "Cáº¬P NHáº¬T", btnX + btnW / 2, btnY + 4, mFont.CENTER);

			// Cháº¥m SÃ¡ng Äá» nháº¥p nhÃ¡y thu hÃºt sá»± chÃº Ã½
			PaintRedNotificationDot(g, btnX + btnW - 3, btnY - 2);
		}
		else
		{
			// NÃºt phiÃªn báº£n chuáº©n NRO (Style 0: normal button)
			PopUp.paintPopUp(g, btnX, btnY, btnW, btnH, 0, isButton: true);
			mFont.tahoma_7b_dark.drawString(g, "v" + CurrentVersion, btnX + btnW / 2, btnY + 4, mFont.CENTER);
		}
	}

	/// <summary>
	/// Thá»±c hiá»‡n váº½ vÃ  hiá»ƒn thá»‹ thÃ nh pháº§n PaintRedNotificationDot lÃªn canvas Ä‘á»“ há»a.
	/// </summary>
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

	/// <summary>
	/// Thá»±c hiá»‡n váº½ vÃ  hiá»ƒn thá»‹ thÃ nh pháº§n PaintUpdateInfoBoard lÃªn canvas Ä‘á»“ há»a.
	/// </summary>
	public static void PaintUpdateInfoBoard(mGraphics g)
	{
		if (!isShowUpdateBoard || isDownloading || isShowCompletedBoard || !hasNewVersion) return;

		// Phá»§ ná»n má» tá»‘i toÃ n mÃ n hÃ¬nh
		g.setColor(0, 0.7f);
		g.fillRect(0, 0, GameCanvas.w, GameCanvas.h);

		// KÃ­ch thÆ°á»›c báº£ng
		int boxW = 300;
		int boxH = 195;
		if (boxW > GameCanvas.w - 16) boxW = GameCanvas.w - 16;
		if (boxH > GameCanvas.h - 16) boxH = GameCanvas.h - 16;
		int boxX = (GameCanvas.w - boxW) / 2;
		int boxY = (GameCanvas.h - boxH) / 2;

		// Khung há»™p thoáº¡i ná»n giáº¥y da be viá»n vÃ ng hoÃ ng kim NRO
		PopUp.paintPopUp(g, boxX, boxY, boxW, boxH, 0, isButton: true);

		// 1. TiÃªu Ä‘á»
		mFont.tahoma_7b_red.drawString(g, "THÃ”NG TIN Cáº¬P NHáº¬T", boxX + boxW / 2, boxY + 10, mFont.CENTER);

		// NÃºt [X] Ä‘Ã³ng nhanh gÃ³c trÃªn bÃªn pháº£i
		int closeX = boxX + boxW - 22;
		int closeY = boxY + 6;
		PopUp.paintPopUp(g, closeX, closeY, 16, 16, 0, isButton: true);
		mFont.tahoma_7b_dark.drawString(g, "X", closeX + 8, closeY + 2, mFont.CENTER);

		// 2. DÃ²ng thÃ´ng tin phiÃªn báº£n
		int infoY = boxY + 28;
		if (hasNewVersion)
		{
			mFont.tahoma_7b_dark.drawString(g, "Báº£n má»›i: v" + remoteVersion, boxX + 14, infoY, mFont.LEFT);
			mFont.tahoma_7_grey.drawString(g, "Hiá»‡n táº¡i: v" + CurrentVersion, boxX + boxW - 14, infoY, mFont.RIGHT);
			if (!string.IsNullOrEmpty(buildDate))
			{
				mFont.tahoma_7b_dark.drawString(g, "NgÃ y phÃ¡t hÃ nh: " + buildDate, boxX + 14, infoY + 14, mFont.LEFT);
			}
		}
		else
		{
			mFont.tahoma_7b_dark.drawString(g, "PhiÃªn báº£n hiá»‡n táº¡i: v" + CurrentVersion, boxX + 14, infoY, mFont.LEFT);
			mFont.tahoma_7_green.drawString(g, "Báº¡n Ä‘ang sá»­ dá»¥ng phiÃªn báº£n má»›i nháº¥t!", boxX + 14, infoY + 14, mFont.LEFT);
		}

		// 3. Khung ná»™i dung chi tiáº¿t (Changelog Box)
		int contentY = infoY + 30;
		int contentW = boxW - 24;
		int contentH = boxH - (contentY - boxY) - 42;
		if (contentH < 40) contentH = 40;

		// Ná»n tá»‘i sáº¯c nÃ©t cho khung vÄƒn báº£n (gá»— sáº«m viá»n nÃ¢u da)
		g.setColor(0x24150c);
		g.fillRect(boxX + 12, contentY, contentW, contentH);
		g.setColor(0x8d6e63);
		g.drawRect(boxX + 12, contentY, contentW, contentH);

		// Clip vÃ  váº½ cÃ¡c dÃ²ng vÄƒn báº£n
		g.setClip(boxX + 14, contentY + 3, contentW - 4, contentH - 6);

		string textToDisplay = changelog;
		if (string.IsNullOrEmpty(textToDisplay))
		{
			textToDisplay = hasNewVersion 
				? ("PhÃ¡t hiá»‡n báº£n cáº­p nháº­t má»›i v" + remoteVersion + "! HÃ£y báº¥m 'Cáº­p nháº­t' Ä‘á»ƒ táº£i trá»±c tiáº¿p báº£n má»›i nháº¥t.")
				: ("Báº¡n Ä‘ang sá»­ dá»¥ng phiÃªn báº£n má»›i nháº¥t cá»§a Mod DragonBoy. KhÃ´ng cÃ³ báº£n cáº­p nháº­t nÃ o má»›i hÆ¡n.");
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

		// 4. CÃ¡c nÃºt thao tÃ¡c bÃªn dÆ°á»›i
		int btnY = boxY + boxH - 32;
		int btnW = 86;
		int btnH = 24;

		if (hasNewVersion)
		{
			int btnUpdateX = boxX + boxW / 2 - btnW - 8;
			int btnCloseX = boxX + boxW / 2 + 8;

			// NÃºt Cáº¬P NHáº¬T
			PopUp.paintPopUp(g, btnUpdateX, btnY, btnW, btnH, 1, isButton: true);
			mFont.tahoma_7b_dark.drawString(g, "Cáº¬P NHáº¬T", btnUpdateX + btnW / 2, btnY + 5, mFont.CENTER);

			// NÃºt ÄÃ“NG
			PopUp.paintPopUp(g, btnCloseX, btnY, btnW, btnH, 0, isButton: true);
			mFont.tahoma_7b_dark.drawString(g, "ÄÃ“NG", btnCloseX + btnW / 2, btnY + 5, mFont.CENTER);
		}
		else
		{
			// NÃºt ÄÃ“NG chÃ­nh giá»¯a
			int btnCloseX = (GameCanvas.w - btnW) / 2;
			PopUp.paintPopUp(g, btnCloseX, btnY, btnW, btnH, 0, isButton: true);
			mFont.tahoma_7b_dark.drawString(g, "ÄÃ“NG", btnCloseX + btnW / 2, btnY + 5, mFont.CENTER);
		}
	}

	/// <summary>
	/// Cáº­p nháº­t tráº¡ng thÃ¡i logic cá»§a UpdateLobbyInput theo chu ká»³ khung hÃ¬nh.
	/// </summary>
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

		// 1. Khi Ä‘ang má»Ÿ Báº£ng ThÃ´ng Tin Cáº­p Nháº­t
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

			// Xá»­ lÃ½ vuá»‘t cuá»™n cáº£m á»©ng hoáº·c kÃ©o chuá»™t
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

				// Click nÃºt [X] Ä‘Ã³ng
				if (px >= closeX && px <= closeX + 16 && py >= closeY && py <= closeY + 16)
				{
					isShowUpdateBoard = false;
					SoundMn.gI()?.buttonClick();
					GameCanvas.isPointerClick = false;
					return;
				}

				// Click nÃºt [ ÄÃ“NG ]
				if (px >= btnCloseX && px <= btnCloseX + btnActionW && py >= btnActionY && py <= btnActionY + btnActionH)
				{
					isShowUpdateBoard = false;
					SoundMn.gI()?.buttonClick();
					GameCanvas.isPointerClick = false;
					return;
				}

				// Click nÃºt [ Cáº¬P NHáº¬T ]
				if (hasNewVersion && px >= btnUpdateX && px <= btnUpdateX + btnActionW && py >= btnActionY && py <= btnActionY + btnActionH)
				{
					isShowUpdateBoard = false;
					SoundMn.gI()?.buttonClick();
					StartInGameDownload();
					GameCanvas.isPointerClick = false;
					return;
				}

				// Click ra ngoÃ i vÃ¹ng báº£ng -> ÄÃ³ng báº£ng
				if (px < boxX || px > boxX + boxW || py < boxY || py > boxY + boxH)
				{
					isShowUpdateBoard = false;
					GameCanvas.isPointerClick = false;
					return;
				}
			}

			// PhÃ­m Back / ESC / ÄÃ³ng
			if (GameCanvas.keyPressed[13] || GameCanvas.keyPressed[5])
			{
				GameCanvas.clearKeyPressed();
				isShowUpdateBoard = false;
			}
			return;
		}

		// 2. Khi á»Ÿ sáº£nh game (chÆ°a má»Ÿ báº£ng), xá»­ lÃ½ click nÃºt á»Ÿ gÃ³c trÃªn bÃªn pháº£i
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
					GameCanvas.startOK("Äang káº¿t ná»‘i GitHub kiá»ƒm tra cáº­p nháº­t...", 8882, null);
				}
				else if (hasChecked)
				{
					GameCanvas.startOK("Báº¡n Ä‘ang á»Ÿ phiÃªn báº£n má»›i nháº¥t (v" + CurrentVersion + ").\nÄÃ£ Ä‘á»“ng bá»™ dá»¯ liá»‡u GitHub!", 8882, null);
				}
				else
				{
					CheckManual();
				}
			}
		}
	}

	/// <summary>
	/// Ãp dá»¥ng cÃ¡c thay Ä‘á»•i cáº¥u hÃ¬nh hoáº·c hiá»‡u á»©ng cá»§a ApplyUpdateAndRestart.
	/// </summary>
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
			GameScr.info1?.addInfo("Lá»—i khá»Ÿi Ä‘á»™ng updater: " + ex.Message, 0);
		}
	}

	/// <summary>
	/// Kiá»ƒm tra Ä‘iá»u kiá»‡n há»£p lá»‡ hoáº·c tráº¡ng thÃ¡i cá»§a IsNewerVersion.
	/// </summary>
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

	/// <summary>
	/// Thá»±c thi phÆ°Æ¡ng thá»©c nghiá»‡p vá»¥ ExtractJsonField.
	/// </summary>
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

/// <summary>
/// CLASS AutoUpdateActionListener: Äá»‹nh nghÄ©a vÃ  Ä‘iá»u phá»‘i logic trong há»‡ thá»‘ng game.
/// </summary>
public class AutoUpdateActionListener : IActionListener
{
	private string url;

	/// <summary>
	/// Thá»±c thi phÆ°Æ¡ng thá»©c nghiá»‡p vá»¥ AutoUpdateActionListener.
	/// </summary>
	public AutoUpdateActionListener(string targetUrl)
	{
		this.url = targetUrl;
	}

	/// <summary>
	/// Thá»±c thi hÃ nh Ä‘á»™ng tÆ°Æ¡ng tÃ¡c khi ngÆ°á»i dÃ¹ng kÃ­ch hoáº¡t lá»‡nh hoáº·c nÃºt báº¥m.
	/// </summary>
	public void perform(int idAction, object p)
	{
		GameCanvas.endDlg();
		if (idAction == 1) // Cáº­p nháº­t ngay -> Táº£i trá»±c tiáº¿p trong game
		{
			ModAutoUpdate.StartInGameDownload();
		}
		else if (idAction == 2) // Äá»ƒ sau
		{
			ModAutoUpdate.hasPrompted = true;
		}
	}
}
