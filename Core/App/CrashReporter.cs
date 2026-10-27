using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public static class CrashReporter
{
	private const uint MB_OK = 0u;

	private const uint MB_ICONERROR = 16u;

	private const uint MB_SETFOREGROUND = 65536u;

	private const uint MB_TOPMOST = 262144u;

	private static bool hasShownPopup;

	private static readonly object lockObj = new object();

	[DllImport("user32.dll", CharSet = CharSet.Unicode)]
	private static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	public static void Initialize()
	{
		try
		{
			Application.logMessageReceivedThreaded += HandleThreadedLog;
			AppDomain.CurrentDomain.UnhandledException += HandleUnhandledException;
		}
		catch (Exception ex)
		{
			Debug.LogError("Failed to initialize CrashReporter: " + ex);
		}
	}

	private static void HandleThreadedLog(string logString, string stackTrace, LogType type)
	{
		if (type == LogType.Exception || type == LogType.Assert)
		{
			string errorType = "C# Exception (" + type.ToString() + ")";
			string details = logString + "\n\nStackTrace:\n" + stackTrace;
			ShowCrashPopup(errorType, details);
		}
	}

	private static void HandleUnhandledException(object sender, UnhandledExceptionEventArgs args)
	{
		object exObj = (args != null) ? args.ExceptionObject : null;
		string details = (exObj != null) ? exObj.ToString() : "Unknown exception";
		ShowCrashPopup("Unhandled Thread Exception", details);
	}

	private static void ShowCrashPopup(string errorType, string details)
	{
		lock (lockObj)
		{
			if (hasShownPopup)
			{
				return;
			}
			hasShownPopup = true;
		}
		string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
		string report = string.Concat(new string[]
		{
			"========================================\n[Dragon Boy Crash Report - ",
			timestamp,
			"]\nLoại lỗi: ",
			errorType,
			"\n\nChi tiết:\n",
			details,
			"\n========================================\n"
		});
		try
		{
			string path = Path.Combine(Application.persistentDataPath, "crash_log.txt");
			File.WriteAllText(path, report, Encoding.UTF8);
		}
		catch
		{
			try
			{
				File.WriteAllText("crash_log.txt", report, Encoding.UTF8);
			}
			catch
			{
			}
		}
		if (Environment.OSVersion.Platform != PlatformID.Win32NT)
		{
			return;
		}
		try
		{
			string popupText = "Game gặp sự cố và bắt buộc phải dừng lại!\n\nVui lòng chụp màn hình thông báo này gửi cho Admin:\n--------------------------------------------------\n" + details;
			if (popupText.Length > 1500)
			{
				popupText = popupText.Substring(0, 1500) + "\n...\n(Toàn bộ chi tiết lỗi đã được lưu tại crash_log.txt)";
			}
			MessageBox(IntPtr.Zero, popupText, "[Dragon Boy] - LỖI TRÒ CHƠI", MB_OK | MB_ICONERROR | MB_SETFOREGROUND | MB_TOPMOST);
		}
		catch (Exception ex)
		{
			Debug.LogError("Error showing crash MessageBox: " + ex);
		}
	}
}
