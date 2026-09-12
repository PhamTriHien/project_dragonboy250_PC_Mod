using System;
using System.Security.Cryptography;
using System.Text;

namespace DragonBoy_Net8_Native.Src.Mod.Security
{
	public static class ModCredentialSecurity
	{
		private const string ENC_PREFIX = "ENC_V1:";
		private static string _cachedDeviceKey = null;

		// Login Watchdog State
		public static bool isLoggingIn = false;
		public static long loginStartTime = 0;
		public static long lastLoginAttemptTime = 0;
		private const long LOGIN_TIMEOUT_MS = 12000; // 12 seconds max wait for server login response

		// Zone Change Watchdog State
		public static bool isWaitingZoneChange = false;
		public static long zoneChangeStartTime = 0;
		public static int targetZone = -1;
		private const long ZONE_TIMEOUT_MS = 5000; // 5 seconds max wait for zone change response

		public static string GetDeviceKey()
		{
			if (!string.IsNullOrEmpty(_cachedDeviceKey))
			{
				return _cachedDeviceKey;
			}
			try
			{
				string devId = Rms.loadRMSString("sys_dev_id");
				if (string.IsNullOrEmpty(devId))
				{
					devId = Guid.NewGuid().ToString("N");
					Rms.saveRMSString("sys_dev_id", devId);
				}
				_cachedDeviceKey = "NRO_KEY_" + devId;
			}
			catch
			{
				_cachedDeviceKey = "NRO_KEY_DEFAULT_MOD_2026";
			}
			return _cachedDeviceKey;
		}

		public static string ObfuscatePassword(string plainPassword)
		{
			if (string.IsNullOrEmpty(plainPassword))
			{
				return string.Empty;
			}
			try
			{
				byte[] plainBytes = Encoding.UTF8.GetBytes(plainPassword);
				byte[] keyBytes;
				using (SHA256 sha = SHA256.Create())
				{
					keyBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(GetDeviceKey()));
				}

				byte[] cipherBytes = new byte[plainBytes.Length];
				for (int i = 0; i < plainBytes.Length; i++)
				{
					cipherBytes[i] = (byte)(plainBytes[i] ^ keyBytes[i % keyBytes.Length] ^ (0x5A + (i & 0x0F)));
				}
				return ENC_PREFIX + Convert.ToBase64String(cipherBytes);
			}
			catch
			{
				return plainPassword;
			}
		}

		public static string DeobfuscatePassword(string storedPassword)
		{
			if (string.IsNullOrEmpty(storedPassword))
			{
				return string.Empty;
			}
			// Nếu là mật khẩu thô từ phiên bản cũ (không có tiền tố ENC), giữ nguyên
			if (!storedPassword.StartsWith(ENC_PREFIX))
			{
				return storedPassword;
			}
			try
			{
				string b64 = storedPassword.Substring(ENC_PREFIX.Length);
				byte[] cipherBytes = Convert.FromBase64String(b64);
				byte[] keyBytes;
				using (SHA256 sha = SHA256.Create())
				{
					keyBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(GetDeviceKey()));
				}

				byte[] plainBytes = new byte[cipherBytes.Length];
				for (int i = 0; i < cipherBytes.Length; i++)
				{
					plainBytes[i] = (byte)(cipherBytes[i] ^ keyBytes[i % keyBytes.Length] ^ (0x5A + (i & 0x0F)));
				}
				return Encoding.UTF8.GetString(plainBytes);
			}
			catch
			{
				return storedPassword;
			}
		}

		public static void SaveCredentials(string username, string password, bool remember)
		{
			try
			{
				if (remember)
				{
					Rms.saveRMSInt(Rms.RMS_check, 1);
					Rms.saveRMSString(Rms.RMS_acc, username != null ? username.Trim() : string.Empty);
					Rms.saveRMSString(Rms.RMS_pass, !string.IsNullOrEmpty(password) ? ObfuscatePassword(password) : string.Empty);
				}
				else
				{
					Rms.saveRMSInt(Rms.RMS_check, 2);
					Rms.saveRMSString(Rms.RMS_acc, string.Empty);
					Rms.saveRMSString(Rms.RMS_pass, string.Empty);
				}
			}
			catch { }
		}

		public static string LoadSavedPassword()
		{
			try
			{
				string raw = Rms.loadRMSString(Rms.RMS_pass);
				return DeobfuscatePassword(raw);
			}
			catch
			{
				return string.Empty;
			}
		}

		public static bool CanAttemptLogin()
		{
			long now = mSystem.currentTimeMillis();
			if (isLoggingIn && now - loginStartTime < LOGIN_TIMEOUT_MS)
			{
				GameScr.info1?.addInfo("Đang đăng nhập, vui lòng chờ...", 0);
				return false;
			}
			if (now - lastLoginAttemptTime < 1500)
			{
				GameScr.info1?.addInfo("Thao tác quá nhanh, vui lòng chờ 1s...", 0);
				return false;
			}
			return true;
		}

		public static void OnLoginStarted()
		{
			isLoggingIn = true;
			loginStartTime = mSystem.currentTimeMillis();
			lastLoginAttemptTime = loginStartTime;
		}

		public static void OnLoginFinished()
		{
			isLoggingIn = false;
			loginStartTime = 0;
		}

		public static void UpdateLoginWatchdog()
		{
			if (isLoggingIn && loginStartTime > 0)
			{
				long now = mSystem.currentTimeMillis();
				if (now - loginStartTime > LOGIN_TIMEOUT_MS)
				{
					isLoggingIn = false;
					loginStartTime = 0;
					GameCanvas.endDlg();
					GameCanvas.startOK("Máy chủ không phản hồi (Timeout), vui lòng thử lại!", 8884, null);
				}
			}
		}

		public static void StartZoneChangeWatchdog(int zone)
		{
			isWaitingZoneChange = true;
			zoneChangeStartTime = mSystem.currentTimeMillis();
			targetZone = zone;
		}

		public static void OnZoneChangeSuccess()
		{
			isWaitingZoneChange = false;
			zoneChangeStartTime = 0;
			targetZone = -1;
		}

		public static void UpdateZoneWatchdog()
		{
			if (isWaitingZoneChange && zoneChangeStartTime > 0)
			{
				long now = mSystem.currentTimeMillis();
				if (now - zoneChangeStartTime > ZONE_TIMEOUT_MS)
				{
					isWaitingZoneChange = false;
					zoneChangeStartTime = 0;
					targetZone = -1;
					InfoDlg.hide();
					GameScr.info1?.addInfo("Khu vực đầy hoặc đổi khu thất bại!", 0);
				}
			}
		}

		public static void SwitchServerCleanly(int newIpSelect)
		{
			try
			{
				Session_ME.gI().close();
				ServerListScreen.SetIpSelect(newIpSelect, issave: true);

				if (ServerListScreen.address != null && newIpSelect >= 0 && newIpSelect < ServerListScreen.address.Length)
				{
					GameMidlet.IP = ServerListScreen.address[newIpSelect];
					GameMidlet.PORT = ServerListScreen.port[newIpSelect];
					if (ServerListScreen.language != null && newIpSelect < ServerListScreen.language.Length)
					{
						GameMidlet.LANGUAGE = ServerListScreen.language[newIpSelect];
						if (ServerListScreen.language[newIpSelect] != mResources.language)
						{
							mResources.loadLanguague(ServerListScreen.language[newIpSelect]);
						}
					}
					if (ServerListScreen.nameServer != null && newIpSelect < ServerListScreen.nameServer.Length)
					{
						LoginScr.serverName = ServerListScreen.nameServer[newIpSelect];
					}
				}

				if (GameCanvas.serverScreen == null)
				{
					GameCanvas.serverScreen = new ServerListScreen();
				}
				GameCanvas.serverScreen.switchToMe();
			}
			catch (Exception ex)
			{
				Cout.LogError("Loi SwitchServerCleanly: " + ex.Message);
			}
		}
	}
}
