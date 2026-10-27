using System;
using System.Collections.Generic;
using System.Text;
using DragonBoy_Net8_Native.Src.Mod.Security;

/// <summary>
/// Quản lý danh sách nhiều tài khoản đã lưu (mã hóa mật khẩu thiết bị), chuyển đổi nhanh 1 chạm tại màn hình đăng nhập/sảnh và đếm ngược 26s chống kẹt tài khoản khi đổi phiên.
/// </summary>
public sealed class ModAccountManager : IActionListener
{
	public sealed class SavedAccount
	{
		public string Username = string.Empty;
		public string EncryptedPass = string.Empty;
		public int ServerIdx = 0;
		public string ServerName = string.Empty;
	}

	private static readonly ModAccountManager s_Instance = new ModAccountManager();
	public static readonly List<SavedAccount> accounts = new List<SavedAccount>();

	public const long ReloginCooldownMs = 26000L;
	public static long lastLeftGameMs = 0L;

	private const int ACT_SELECT_BASE = 29400;
	private const int ACT_DELETE_BASE = 29500;
	private const int ACT_SAVE_CURRENT = 29601;
	private const int ACT_ADD_NEW_USER = 29602;
	private const int ACT_SAVE_NEW_USER = 29603;
	private const int ACT_SAVE_NEW_PASS = 29604;
	private const int ACT_OPEN_DELETE_MENU = 29605;
	private const int ACT_CLEAR_ALL_CONFIRM = 29606;

	private static string s_PendingUsername = string.Empty;

	public static ModAccountManager gI()
	{
		return s_Instance;
	}

	public static void NotifyLeftGame()
	{
		lastLeftGameMs = mSystem.currentTimeMillis();
	}

	public static int GetReloginRemainingSeconds()
	{
		if (lastLeftGameMs <= 0L)
		{
			return 0;
		}
		long elapsed = mSystem.currentTimeMillis() - lastLeftGameMs;
		if (elapsed < 0L || elapsed >= ReloginCooldownMs)
		{
			return 0;
		}
		return (int)((ReloginCooldownMs - elapsed + 999L) / 1000L);
	}

	public static void UpsertAccount(string username, string plainPassword, int serverIdx)
	{
		if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(plainPassword))
		{
			return;
		}
		string cleanUser = username.Trim();
		if (cleanUser.Length == 0)
		{
			return;
		}

		string encPass = ModCredentialSecurity.ObfuscatePassword(plainPassword);
		string svName = GetServerNameSafe(serverIdx);

		for (int i = 0; i < accounts.Count; i++)
		{
			if (string.Equals(accounts[i].Username, cleanUser, StringComparison.OrdinalIgnoreCase))
			{
				accounts[i].Username = cleanUser;
				accounts[i].EncryptedPass = encPass;
				if (serverIdx >= 0)
				{
					accounts[i].ServerIdx = serverIdx;
				}
				if (!string.IsNullOrEmpty(svName))
				{
					accounts[i].ServerName = svName;
				}
				ModConfig.SaveConfig();
				return;
			}
		}

		if (accounts.Count >= 30)
		{
			accounts.RemoveAt(0);
		}

		SavedAccount item = new SavedAccount();
		item.Username = cleanUser;
		item.EncryptedPass = encPass;
		item.ServerIdx = serverIdx >= 0 ? serverIdx : 0;
		item.ServerName = svName;
		accounts.Add(item);
		ModConfig.SaveConfig();
	}

	private static string GetServerNameSafe(int serverIdx)
	{
		if (ServerListScreen.nameServer != null && serverIdx >= 0 && serverIdx < ServerListScreen.nameServer.Length)
		{
			return ServerListScreen.nameServer[serverIdx] ?? string.Empty;
		}
		return LoginScr.serverName ?? string.Empty;
	}

	public static string ExportConfig()
	{
		if (accounts.Count == 0)
		{
			return string.Empty;
		}
		StringBuilder sb = new StringBuilder();
		for (int i = 0; i < accounts.Count; i++)
		{
			SavedAccount a = accounts[i];
			if (a == null || string.IsNullOrEmpty(a.Username))
			{
				continue;
			}
			if (sb.Length > 0)
			{
				sb.Append(';');
			}
			string u64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(a.Username));
			string pEnc = Convert.ToBase64String(Encoding.UTF8.GetBytes(a.EncryptedPass ?? string.Empty));
			string s64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(a.ServerName ?? string.Empty));
			sb.Append(u64).Append('|').Append(pEnc).Append('|').Append(a.ServerIdx).Append('|').Append(s64);
		}
		return sb.ToString();
	}

	public static void ImportConfig(string raw)
	{
		accounts.Clear();
		if (string.IsNullOrEmpty(raw))
		{
			return;
		}
		string[] entries = raw.Split(';');
		for (int i = 0; i < entries.Length; i++)
		{
			string entry = entries[i].Trim();
			if (entry.Length == 0)
			{
				continue;
			}
			string[] parts = entry.Split('|');
			if (parts.Length < 2)
			{
				continue;
			}
			try
			{
				string user = Encoding.UTF8.GetString(Convert.FromBase64String(parts[0]));
				string encPass = Encoding.UTF8.GetString(Convert.FromBase64String(parts[1]));
				int svIdx = 0;
				if (parts.Length >= 3)
				{
					int.TryParse(parts[2], out svIdx);
				}
				string svName = string.Empty;
				if (parts.Length >= 4 && parts[3].Length > 0)
				{
					svName = Encoding.UTF8.GetString(Convert.FromBase64String(parts[3]));
				}
				if (!string.IsNullOrEmpty(user))
				{
					SavedAccount acc = new SavedAccount();
					acc.Username = user;
					acc.EncryptedPass = encPass;
					acc.ServerIdx = svIdx >= 0 ? svIdx : 0;
					acc.ServerName = svName;
					accounts.Add(acc);
				}
			}
			catch
			{
			}
		}
	}

	public static void PaintLoginButton(mGraphics g, int xLog, int yLog, int boxW)
	{
		int btnW = 62;
		int btnH = 18;
		int btnX = xLog + boxW - btnW - 8;
		int btnY = yLog + 72;
		string label = "TK (" + accounts.Count + ")";
		ModUI.PaintNativeButton(btnX, btnY, btnW, btnH, label, accounts.Count > 0, g);
	}

	public static bool CheckLoginButtonTap(LoginScr scr, int xLog, int yLog, int boxW)
	{
		if (scr == null || GameCanvas.currentDialog != null || (GameCanvas.menu != null && GameCanvas.menu.showMenu))
		{
			return false;
		}
		if (!GameCanvas.isPointerJustRelease && !GameCanvas.isPointerClick)
		{
			return false;
		}
		int btnW = 62;
		int btnH = 18;
		int btnX = xLog + boxW - btnW - 8;
		int btnY = yLog + 72;
		if (GameCanvas.px >= btnX && GameCanvas.px <= btnX + btnW && GameCanvas.py >= btnY && GameCanvas.py <= btnY + btnH)
		{
			GameCanvas.clearAllPointerEvent();
			SoundMn.gI().buttonClick();
			OpenAccountMenu();
			return true;
		}
		return false;
	}

	public static void PaintReloginCountdown(mGraphics g)
	{
		int waitSec = GetReloginRemainingSeconds();
		if (waitSec <= 0 || g == null)
		{
			return;
		}
		if (ModAutoUpdate.isDownloading || ModAutoUpdate.isShowCompletedBoard || ModAutoUpdate.isShowUpdateBoard)
		{
			return;
		}
		mFont font = mFont.tahoma_7b_yellow ?? mFont.tahoma_7_yellow ?? mFont.tahoma_7_white;
		if (font != null)
		{
			font.drawString(g, "Chờ đăng nhập lại: " + waitSec + "s", GameCanvas.w / 2, 6, mFont.CENTER);
		}
	}

	public static void OpenAccountMenu()
	{
		MyVector items = new MyVector();
		string curUser = Rms.loadRMSString(Rms.RMS_acc) ?? string.Empty;

		for (int i = 0; i < accounts.Count && i < 30; i++)
		{
			SavedAccount a = accounts[i];
			bool isCurrent = string.Equals(a.Username, curUser, StringComparison.OrdinalIgnoreCase);
			string svTag = !string.IsNullOrEmpty(a.ServerName) ? a.ServerName : ("SV " + (a.ServerIdx + 1));
			string caption = (isCurrent ? "[*] " : "") + ShortenUser(a.Username, 16) + "\n(" + ShortenUser(svTag, 16) + ")";
			items.addElement(new Command(caption, gI(), ACT_SELECT_BASE + i, null));
		}

		if (GameCanvas.loginScr != null && GameCanvas.currentScreen == GameCanvas.loginScr &&
		    GameCanvas.loginScr.tfUser != null && !string.IsNullOrEmpty(GameCanvas.loginScr.tfUser.getText()))
		{
			items.addElement(new Command("[+] Lưu TK\nđang nhập", gI(), ACT_SAVE_CURRENT, null));
		}

		items.addElement(new Command("[+] Thêm TK\nmới", gI(), ACT_ADD_NEW_USER, null));

		if (accounts.Count > 0)
		{
			items.addElement(new Command("[x] Xóa TK\nđã lưu", gI(), ACT_OPEN_DELETE_MENU, null));
		}

		GameCanvas.menu.startAt(items, 0);
	}

	private static string ShortenUser(string text, int maxLen)
	{
		if (string.IsNullOrEmpty(text))
		{
			return string.Empty;
		}
		return text.Length <= maxLen ? text : (text.Substring(0, maxLen) + "..");
	}

	private static void ApplyAndLoginAccount(int index)
	{
		if (index < 0 || index >= accounts.Count)
		{
			return;
		}
		SavedAccount acc = accounts[index];
		string plainPass = ModCredentialSecurity.DeobfuscatePassword(acc.EncryptedPass);

		ModCredentialSecurity.SaveCredentials(acc.Username, plainPass, true);

		if (ServerListScreen.address != null && acc.ServerIdx >= 0 && acc.ServerIdx < ServerListScreen.address.Length)
		{
			ServerListScreen.SetIpSelect(acc.ServerIdx, issave: true);
		}

		if (GameCanvas.loginScr != null)
		{
			GameCanvas.loginScr.isCheck = true;
			GameCanvas.loginScr.isLogin2 = false;
			if (GameCanvas.loginScr.tfUser != null) GameCanvas.loginScr.tfUser.setText(acc.Username);
			if (GameCanvas.loginScr.tfPass != null) GameCanvas.loginScr.tfPass.setText(plainPass);
		}

		Session_ME.gI().close();

		int waitSec = GetReloginRemainingSeconds();
		if (GameCanvas.serverScreen == null)
		{
			GameCanvas.serverScreen = new ServerListScreen();
		}
		GameCanvas.serverScreen.switchToMe();

		if (waitSec > 0)
		{
			GameCanvas.startOKDlg("Đã chọn TK: " + acc.Username + ".\nVui lòng chờ thêm " + waitSec + "s rồi bấm Chơi Tiếp để tránh kẹt tài khoản!");
			return;
		}

		GameCanvas.serverScreen.Login_New();
	}

	public void perform(int idAction, object p)
	{
		if (idAction >= ACT_SELECT_BASE && idAction < ACT_SELECT_BASE + 50)
		{
			ApplyAndLoginAccount(idAction - ACT_SELECT_BASE);
			return;
		}

		if (idAction >= ACT_DELETE_BASE && idAction < ACT_DELETE_BASE + 50)
		{
			int delIdx = idAction - ACT_DELETE_BASE;
			if (delIdx >= 0 && delIdx < accounts.Count)
			{
				string removedUser = accounts[delIdx].Username;
				accounts.RemoveAt(delIdx);
				ModConfig.SaveConfig();
				GameCanvas.startOKDlg("Đã xóa tài khoản: " + removedUser);
			}
			return;
		}

		switch (idAction)
		{
			case ACT_SAVE_CURRENT:
			{
				if (GameCanvas.loginScr == null || GameCanvas.loginScr.tfUser == null || GameCanvas.loginScr.tfPass == null)
				{
					break;
				}
				string u = (GameCanvas.loginScr.tfUser.getText() ?? string.Empty).Trim();
				string pw = GameCanvas.loginScr.tfPass.getText() ?? string.Empty;
				if (u.Length == 0 || pw.Length == 0)
				{
					GameCanvas.startOKDlg("Vui lòng nhập đầy đủ Tài khoản và Mật khẩu trước khi lưu!");
					break;
				}
				UpsertAccount(u, pw, ServerListScreen.ipSelect);
				ModCredentialSecurity.SaveCredentials(u, pw, true);
				GameCanvas.startOKDlg("Đã lưu tài khoản: " + u);
				break;
			}

			case ACT_ADD_NEW_USER:
				s_PendingUsername = string.Empty;
				GameCanvas.inputDlg.show(
					"Nhập Tài khoản (Email / SĐT):",
					new Command(mResources.OK, this, ACT_SAVE_NEW_USER, null),
					TField.INPUT_TYPE_ANY);
				break;

			case ACT_SAVE_NEW_USER:
			{
				string u = (GameCanvas.inputDlg.tfInput.getText() ?? string.Empty).Trim();
				GameCanvas.endDlg();
				if (u.Length == 0)
				{
					break;
				}
				s_PendingUsername = u;
				GameCanvas.inputDlg.show(
					"Nhập Mật khẩu cho " + ShortenUser(u, 18) + ":",
					new Command(mResources.OK, this, ACT_SAVE_NEW_PASS, null),
					TField.INPUT_TYPE_PASSWORD);
				break;
			}

			case ACT_SAVE_NEW_PASS:
			{
				string pw = GameCanvas.inputDlg.tfInput.getText() ?? string.Empty;
				GameCanvas.endDlg();
				if (string.IsNullOrEmpty(s_PendingUsername) || pw.Length == 0)
				{
					s_PendingUsername = string.Empty;
					break;
				}
				string savedUser = s_PendingUsername;
				s_PendingUsername = string.Empty;
				UpsertAccount(savedUser, pw, ServerListScreen.ipSelect);
				if (GameCanvas.loginScr != null)
				{
					if (GameCanvas.loginScr.tfUser != null) GameCanvas.loginScr.tfUser.setText(savedUser);
					if (GameCanvas.loginScr.tfPass != null) GameCanvas.loginScr.tfPass.setText(pw);
				}
				ModCredentialSecurity.SaveCredentials(savedUser, pw, true);
				GameCanvas.startOKDlg("Đã thêm tài khoản: " + savedUser);
				break;
			}

			case ACT_OPEN_DELETE_MENU:
			{
				MyVector delMenu = new MyVector();
				for (int i = 0; i < accounts.Count && i < 30; i++)
				{
					delMenu.addElement(new Command("Xóa: " + ShortenUser(accounts[i].Username, 16), this, ACT_DELETE_BASE + i, null));
				}
				if (accounts.Count > 1)
				{
					delMenu.addElement(new Command("Xóa TẤT CẢ\n(" + accounts.Count + " TK)", this, ACT_CLEAR_ALL_CONFIRM, null));
				}
				GameCanvas.menu.startAt(delMenu, 0);
				break;
			}

			case ACT_CLEAR_ALL_CONFIRM:
				accounts.Clear();
				ModConfig.SaveConfig();
				GameCanvas.startOKDlg("Đã xóa toàn bộ danh sách tài khoản đã lưu.");
				break;
		}
	}
}
