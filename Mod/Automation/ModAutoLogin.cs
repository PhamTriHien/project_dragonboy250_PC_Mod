using System;

public static class ModAutoLogin
{
	public static bool isEnabled = true;
	public static bool isReconnecting = false;
	public static int retryCount = 0;
	public static long lastRetryTime = 0;
	public static int reconnectDelayMs = 3000;
	public static string statusMessage = "";

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

	public static void SnapshotAutoState()
	{
		if (ModMenu.IsInGame())
		{
			wasTanSatActive = ModTanSat.autoTanSat;
			wasAutoPickActive = ModAutoPick.autoPick;
			wasAutoHealActive = ModAutoHeal.autoPean;
			wasGoBackActive = ModGoBack.isGoBackActive;
			wasSetKHActive = ModSetActivator.isActive;
			wasAutoBuaActive = ModAutoBuyBua.isAutoRebuy;
			wasSpeedHackActive = ModSpeed.speedHack;

			if (ModGoBack.savedMapId > 0)
			{
				lastFarmMapId = ModGoBack.savedMapId;
				lastFarmZoneId = ModGoBack.savedZoneId;
				lastFarmX = ModGoBack.savedX;
				lastFarmY = ModGoBack.savedY;
			}
			else if (TileMap.mapID > 0 && Char.myCharz() != null)
			{
				lastFarmMapId = TileMap.mapID;
				lastFarmZoneId = TileMap.zoneID;
				lastFarmX = Char.myCharz().cx;
				lastFarmY = Char.myCharz().cy;
			}
		}
	}

	public static void OnDisconnected()
	{
		if (!isEnabled) return;

		SnapshotAutoState();
		isReconnecting = true;
		retryCount = 0;
		lastRetryTime = mSystem.currentTimeMillis();
		statusMessage = "Mất kết nối. Đang chuẩn bị tự động đăng nhập lại...";
		Res.outz("[ModAutoLogin] Disconnected detected. Auto-reconnect triggered.");
	}

	public static void Update()
	{
		if (!isEnabled || !isReconnecting) return;

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
		if (now - lastRetryTime < reconnectDelayMs)
		{
			return;
		}

		lastRetryTime = now;
		retryCount++;
		statusMessage = "Tự động kết nối lại lần " + retryCount + "...";
		Res.outz("[ModAutoLogin] " + statusMessage);

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
				if (lastFarmMapId > 0 && (TileMap.mapID != lastFarmMapId || (lastFarmZoneId >= 0 && TileMap.zoneID != lastFarmZoneId)))
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
