using System;

/// <summary>
/// Module Auto Reconnect & Re-login: Tự động kết nối lại máy chủ và đăng nhập tài khoản khi mất kết nối mạng.
/// </summary>
public static class ModAutoLogin
{
	public static bool isEnabled = true;
	public static bool keepSavedZone = false;
	public static int savedKeepZoneId = -1;
	private static long lastKeepZoneCheckTime = 0;
	public static bool isReconnecting = false;
	public static int retryCount = 0;
	public static long lastRetryTime = 0;
	public static int reconnectDelayMs = 8000;
	public static string statusMessage = "";

	public static void ToggleKeepZone()
	{
		keepSavedZone = !keepSavedZone;
		if (keepSavedZone && ModMenu.IsInGame())
		{
			savedKeepZoneId = TileMap.zoneID;
			GameScr.info1.addInfo("Giữ Khu Cố Định: BẬT (Khu " + savedKeepZoneId + ")", 0);
		}
		else
		{
			GameScr.info1.addInfo("Giữ Khu Cố Định: TẮT", 0);
		}
		ModConfig.SaveConfig();
		SoundMn.gI().buttonClick();
	}

	public static bool wasTanSatActive = false;
	public static bool wasAutoPickActive = false;
	public static bool wasAutoHealActive = false;
	public static bool wasGoBackActive = false;
	public static bool wasSetKHActive = false;
	public static bool wasAutoBuaActive = false;
	public static bool wasSpeedHackActive = false;

	public static int lastFarmMapId = -1;
	public static int lastFarmZoneId = -1;
	public static int lastFarmX = -1;
	public static int lastFarmY = -1;

	public static bool hasEnteredGameOnce = false;

	/// <summary>
	/// Dọn dẹp và đặt lại trạng thái ban đầu cho ResetSession.
	/// </summary>
	public static void ResetSession()
	{
		isReconnecting = false;
		hasEnteredGameOnce = false;
		retryCount = 0;
		lastRetryTime = 0;
		statusMessage = "";
		wasTanSatActive = false;
		wasAutoPickActive = false;
		wasAutoHealActive = false;
		wasGoBackActive = false;
		wasSetKHActive = false;
		wasAutoBuaActive = false;
		wasSpeedHackActive = false;
		lastFarmMapId = -1;
		lastFarmZoneId = -1;
		lastFarmX = -1;
		lastFarmY = -1;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ SnapshotAutoState.
	/// </summary>
	public static void SnapshotAutoState()
	{
		if (ModMenu.IsInGame())
		{
			hasEnteredGameOnce = true;
			wasTanSatActive = ModTanSat.autoTanSat;
			wasAutoPickActive = ModAutoPick.autoPick;
			wasAutoHealActive = ModAutoHeal.autoPean;
			wasGoBackActive = ModGoBack.isGoBackActive;
			wasSetKHActive = ModSetActivator.isActive;
			wasAutoBuaActive = ModAutoBuyBua.isAutoRebuy;
			wasSpeedHackActive = ModSpeed.speedHack;

			if (ModGoBack.isGoBackActive && ModGoBack.savedMapId >= 0)
			{
				lastFarmMapId = ModGoBack.savedMapId;
				lastFarmZoneId = ModGoBack.savedZoneId;
				lastFarmX = ModGoBack.savedX;
				lastFarmY = ModGoBack.savedY;
			}
			else if (ModSetActivator.isActive && ModSetActivator.savedFarmMapId >= 0)
			{
				lastFarmMapId = ModSetActivator.savedFarmMapId;
				lastFarmZoneId = ModSetActivator.savedFarmZoneId;
				lastFarmX = ModSetActivator.savedFarmX;
				lastFarmY = ModSetActivator.savedFarmY;
			}
			else if (TileMap.mapID >= 0 && Char.myCharz() != null)
			{
				lastFarmMapId = TileMap.mapID;
				lastFarmZoneId = TileMap.zoneID;
				lastFarmX = Char.myCharz().cx;
				lastFarmY = Char.myCharz().cy;
			}
		}
	}

	public static void TriggerScheduledReconnect()
	{
		if (!isEnabled) return;
		hasEnteredGameOnce = true;
		isReconnecting = true;
		retryCount = 0;
		lastRetryTime = 0;
		statusMessage = "Đến giờ hẹn lịch! Đang tự động đăng nhập lại...";
	}

	/// <summary>
	/// Bộ xử lý sự kiện tiếp nhận và phản hồi OnDisconnected.
	/// </summary>
	public static void OnDisconnected()
	{
		if (ModSchedule.isEnabled && ModSchedule.disconnectOnOffWindow && ModSchedule.isOffWindowDisconnected)
		{
			isReconnecting = false;
			return;
		}
		if (!isEnabled || !hasEnteredGameOnce || isReconnecting) return;
		if (GameCanvas.currentScreen == GameCanvas.loginScr || GameCanvas.currentScreen == GameCanvas.serverScreen)
		{
			return;
		}

		SnapshotAutoState();
		isReconnecting = true;
		retryCount = 0;
		lastRetryTime = mSystem.currentTimeMillis();
		statusMessage = "Mất kết nối. Đang chuẩn bị tự động đăng nhập lại...";
		Res.outz("[ModAutoLogin] Disconnected detected. Auto-reconnect triggered.");
	}

	/// <summary>
	/// Cập nhật trạng thái logic của đối tượng theo từng chu kỳ khung hình.
	/// </summary>
	public static void Update()
	{
		if (keepSavedZone && savedKeepZoneId >= 0 && ModMenu.IsInGame() && !Char.isLoadingMap && !Char.ischangingMap)
		{
			if (!ModNextMap.isNextMapActive && !ModBossNotice.isAutoScanBossZone && !ModGoBack.isReturning && !ModSetActivator.isBusy && !ModAutoBuyBua.isBusy)
			{
				long tNow = mSystem.currentTimeMillis();
				if (TileMap.zoneID != savedKeepZoneId && tNow - lastKeepZoneCheckTime >= 3500)
				{
					lastKeepZoneCheckTime = tNow;
					Service.gI().requestChangeZone(savedKeepZoneId, -1);
				}
			}
		}

		if (!isEnabled || !isReconnecting) return;
		if (ModSchedule.isEnabled && ModSchedule.disconnectOnOffWindow && ModSchedule.isOffWindowDisconnected)
		{
			isReconnecting = false;
			return;
		}

		// Khi da vao lai map on dinh
		if (ModMenu.IsInGame() && !Char.isLoadingMap)
		{
			RestoreAutoState();
			isReconnecting = false;
			retryCount = 0;
			statusMessage = "";
			return;
		}

		long now = mSystem.currentTimeMillis();
		int currentBackoff = reconnectDelayMs * (retryCount + 1);
		if (now - lastRetryTime < currentBackoff)
		{
			return;
		}

		lastRetryTime = now;
		if (retryCount >= 3)
		{
			Res.outz("[ModAutoLogin] Max reconnect attempts reached (3). Pausing auto-reconnect to prevent server rate limiting.");
			isReconnecting = false;
			statusMessage = "";
			return;
		}
		retryCount++;
		statusMessage = "Tự động kết nối lại lần " + retryCount + "/3...";
		Res.outz("[ModAutoLogin] " + statusMessage);

		PerformLoginSequence();
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ PerformLoginSequence.
	/// </summary>
	public static void PerformLoginSequence()
	{
		// 1. Dong cac popup thong bao loi (MsgDlg / Dialog OK)
		try
		{
			if (GameCanvas.currentDialog != null)
			{
				GameCanvas.endDlg();
			}
		}
		catch { }

		// 2. Man hinh chon nhan vat (SelectCharScr)
		try
		{
			if (GameCanvas.currentScreen == GameCanvas._SelectCharScr)
			{
				SelectCharScr.gI().perform(100, null);
				return;
			}
		}
		catch { }

		// 3. Man hinh chon server (ServerListScreen)
		try
		{
			if (GameCanvas.currentScreen == GameCanvas.serverScreen)
			{
				ServerListScreen.isAutoLogin = true;
				GameCanvas.serverScreen.Login_New();
				return;
			}
		}
		catch { }

		// 4. Man hinh dang nhap (LoginScr)
		try
		{
			if (GameCanvas.currentScreen == GameCanvas.loginScr)
			{
				GameCanvas.loginScr.doLogin();
				return;
			}
		}
		catch { }

		// 5. Bat ky man hinh cho nao khac
		try
		{
			if (GameCanvas.serverScreen != null)
			{
				ServerListScreen.isAutoLogin = true;
				GameCanvas.serverScreen.Login_New();
			}
		}
		catch { }
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ RestoreAutoState.
	/// </summary>
	public static void RestoreAutoState()
	{
		try
		{
			Res.outz("[ModAutoLogin] Restoring active auto features...");

			// 1. Tan Sat
			if (wasTanSatActive)
			{
				ModTanSat.autoTanSat = true;
			}

			// 2. Tu Nhat
			if (wasAutoPickActive)
			{
				ModAutoPick.autoPick = true;
			}

			// 3. Hoi Mau & Dau
			if (wasAutoHealActive)
			{
				ModAutoHeal.autoPean = true;
			}

			// 4. GoBack ve lai bai farm cu
			if (wasGoBackActive)
			{
				ModGoBack.isGoBackActive = true;
				if (lastFarmMapId >= 0 && (TileMap.mapID != lastFarmMapId || (lastFarmZoneId >= 0 && TileMap.zoneID != lastFarmZoneId)))
				{
					ModGoBack.savedMapId = lastFarmMapId;
					ModGoBack.savedZoneId = lastFarmZoneId;
					ModGoBack.savedX = lastFarmX;
					ModGoBack.savedY = lastFarmY;
					ModGoBack.StartGoBackNow();
				}
			}

			// 5. Up Set KH
			if (wasSetKHActive)
			{
				ModSetActivator.isActive = true;
				if (!wasGoBackActive && lastFarmMapId >= 0 && (TileMap.mapID != lastFarmMapId || (lastFarmZoneId >= 0 && TileMap.zoneID != lastFarmZoneId)))
				{
					ModSetActivator.savedFarmMapId = lastFarmMapId;
					ModSetActivator.savedFarmZoneId = lastFarmZoneId;
					ModSetActivator.savedFarmX = lastFarmX;
					ModSetActivator.savedFarmY = lastFarmY;
					ModSetActivator.currentState = ModSetActivator.SetActivatorState.ReturningToFarm;
					ModNextMap.StartNextMap(lastFarmMapId);
				}
			}

			// 6. Tu Mua Bua
			if (wasAutoBuaActive)
			{
				ModAutoBuyBua.isAutoRebuy = true;
			}

			// 7. Toc Do
			if (wasSpeedHackActive)
			{
				ModSpeed.speedHack = true;
			}

			GameScr.info1.addInfo("Tự đăng nhập lại thành công! Đã khôi phục toàn bộ Auto.", 0);
		}
		catch (Exception ex)
		{
			Res.outz("[ModAutoLogin] Error restoring state: " + ex.Message);
		}
	}
}
