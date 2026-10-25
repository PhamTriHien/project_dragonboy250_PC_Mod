using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

/// <summary>
/// Quản lý kết nối qua SOCKS5 / HTTP CONNECT Proxy và cấu hình máy chủ tùy chỉnh IP:Port.
/// </summary>
public sealed class ModProxyServer : IActionListener
{
	private static readonly ModProxyServer s_Instance = new ModProxyServer();

	public static string proxyRaw = string.Empty;
	public static string customSvName = string.Empty;
	public static string customSvIp = string.Empty;
	public static int customSvPort = 14445;

	private const int ACT_OPEN_PROXY_INPUT = 29101;
	private const int ACT_SAVE_PROXY_INPUT = 29102;
	private const int ACT_CLEAR_PROXY = 29103;
	private const int ACT_OPEN_SV_INPUT = 29104;
	private const int ACT_SAVE_SV_INPUT = 29105;
	private const int ACT_CLEAR_SV = 29106;

	public static ModProxyServer gI()
	{
		return s_Instance;
	}

	public static bool HasProxy()
	{
		return !string.IsNullOrEmpty(proxyRaw) && proxyRaw.Trim().Length > 0;
	}

	public static bool HasCustomServer()
	{
		return !string.IsNullOrEmpty(customSvIp) && customSvIp.Trim().Length > 0 && customSvPort > 0 && customSvPort <= 65535;
	}

	public static int GetBtnX() => 6;
	public static int GetBtnY() => 4;
	public static int GetBtnW() => 68;
	public static int GetBtnH() => 22;

	private static IPAddress ResolveSingleIp(string host)
	{
		IPAddress ipAddr;
		if (!IPAddress.TryParse(host, out ipAddr))
		{
			IPAddress[] ips = Dns.GetHostAddresses(host);
			ipAddr = null;
			for (int i = 0; i < ips.Length; i++)
			{
				if (ips[i].AddressFamily == AddressFamily.InterNetwork)
				{
					ipAddr = ips[i];
					break;
				}
			}
			if (ipAddr == null && ips.Length > 0)
			{
				ipAddr = ips[0];
			}
		}
		if (ipAddr == null)
		{
			throw new SocketException((int)SocketError.HostNotFound);
		}
		return ipAddr;
	}

	private static void ConfigureSocketOptions(TcpClient sc)
	{
		sc.NoDelay = true;
		sc.SendBufferSize = 65536;
		sc.ReceiveBufferSize = 131072;
		try
		{
			sc.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
			sc.Client.LingerState = new LingerOption(false, 0);
		}
		catch
		{
		}
	}

	/// <summary>
	/// Mở kết nối TCP tới máy chủ đích trực tiếp hoặc xuyên qua SOCKS5 / HTTP CONNECT Proxy.
	/// </summary>
	public static TcpClient ConnectTcpClient(string targetHost, int targetPort)
	{
#if BROWSER
		if (OperatingSystem.IsBrowser())
		{
			IPAddress directBrowserIp = ResolveSingleIp(targetHost);
			TcpClient browserClient = new TcpClient(directBrowserIp.AddressFamily);
			ConfigureSocketOptions(browserClient);
			browserClient.Connect(directBrowserIp, targetPort);
			return browserClient;
		}
#endif
		if (!HasProxy())
		{
			IPAddress ipAddr = ResolveSingleIp(targetHost);
			TcpClient directClient = new TcpClient(ipAddr.AddressFamily);
			ConfigureSocketOptions(directClient);
			directClient.Connect(ipAddr, targetPort);
			return directClient;
		}

		bool isHttpProxy;
		string proxyHost;
		int proxyPort;
		string proxyUser;
		string proxyPass;
		if (!TryParseProxy(proxyRaw, out isHttpProxy, out proxyHost, out proxyPort, out proxyUser, out proxyPass))
		{
			throw new IOException("Định dạng Proxy không hợp lệ: " + proxyRaw);
		}

		IPAddress proxyIp = ResolveSingleIp(proxyHost);
		TcpClient sc = new TcpClient(proxyIp.AddressFamily);
		ConfigureSocketOptions(sc);
		sc.SendTimeout = 10000;
		sc.ReceiveTimeout = 10000;

		try
		{
			sc.Connect(proxyIp, proxyPort);
			NetworkStream stream = sc.GetStream();

			if (isHttpProxy)
			{
				HandshakeHttpConnect(stream, targetHost, targetPort, proxyUser, proxyPass);
			}
			else
			{
				HandshakeSocks5(stream, targetHost, targetPort, proxyUser, proxyPass);
			}

			sc.SendTimeout = 0;
			sc.ReceiveTimeout = 0;
			return sc;
		}
		catch
		{
			try { sc.Close(); } catch { }
			throw;
		}
	}

	public static bool TryParseProxy(string raw, out bool isHttp, out string host, out int port, out string user, out string pass)
	{
		isHttp = false;
		host = string.Empty;
		port = 0;
		user = string.Empty;
		pass = string.Empty;

		if (string.IsNullOrEmpty(raw)) return false;
		string s = raw.Trim();
		if (s.Length == 0) return false;

		if (s.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
		{
			isHttp = true;
			s = s.Substring(7);
		}
		else if (s.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
		{
			isHttp = true;
			s = s.Substring(8);
		}
		else if (s.StartsWith("socks5://", StringComparison.OrdinalIgnoreCase))
		{
			s = s.Substring(9);
		}
		else if (s.StartsWith("socks://", StringComparison.OrdinalIgnoreCase))
		{
			s = s.Substring(8);
		}

		if (s.Contains("@"))
		{
			int atIdx = s.LastIndexOf('@');
			string authPart = s.Substring(0, atIdx);
			string hostPart = s.Substring(atIdx + 1);
			int colonAuth = authPart.IndexOf(':');
			if (colonAuth >= 0)
			{
				user = authPart.Substring(0, colonAuth);
				pass = authPart.Substring(colonAuth + 1);
			}
			else
			{
				user = authPart;
			}
			int colonHost = hostPart.LastIndexOf(':');
			if (colonHost <= 0) return false;
			host = hostPart.Substring(0, colonHost).Trim();
			if (!int.TryParse(hostPart.Substring(colonHost + 1).Trim(), out port) || port <= 0 || port > 65535) return false;
			return host.Length > 0;
		}

		string[] parts = s.Split(':');
		if (parts.Length < 2) return false;
		host = parts[0].Trim();
		if (host.Length == 0) return false;
		if (!int.TryParse(parts[1].Trim(), out port) || port <= 0 || port > 65535) return false;

		if (parts.Length >= 4)
		{
			user = parts[2];
			pass = string.Join(":", parts, 3, parts.Length - 3);
		}
		else if (parts.Length == 3)
		{
			user = parts[2];
		}
		return true;
	}

	private static void ReadExact(Stream stream, byte[] buf, int offset, int count)
	{
		int total = 0;
		while (total < count)
		{
			int n = stream.Read(buf, offset + total, count - total);
			if (n <= 0)
			{
				throw new EndOfStreamException("Proxy đóng kết nối đột ngột khi đang bắt tay.");
			}
			total += n;
		}
	}

	private static void HandshakeHttpConnect(NetworkStream stream, string targetHost, int targetPort, string user, string pass)
	{
		StringBuilder sb = new StringBuilder();
		sb.Append("CONNECT ").Append(targetHost).Append(':').Append(targetPort).Append(" HTTP/1.1\r\n");
		sb.Append("Host: ").Append(targetHost).Append(':').Append(targetPort).Append("\r\n");
		if (!string.IsNullOrEmpty(user))
		{
			string token = Convert.ToBase64String(Encoding.UTF8.GetBytes(user + ":" + (pass ?? string.Empty)));
			sb.Append("Proxy-Authorization: Basic ").Append(token).Append("\r\n");
		}
		sb.Append("Proxy-Connection: Keep-Alive\r\n\r\n");

		byte[] reqBytes = Encoding.ASCII.GetBytes(sb.ToString());
		stream.Write(reqBytes, 0, reqBytes.Length);
		stream.Flush();

		StringBuilder headerSb = new StringBuilder();
		int matched = 0;
		int readCount = 0;
		while (readCount < 8192)
		{
			int b = stream.ReadByte();
			if (b < 0)
			{
				throw new EndOfStreamException("HTTP Proxy đóng kết nối trước khi trả phản hồi.");
			}
			readCount++;
			char ch = (char)b;
			headerSb.Append(ch);
			if ((matched == 0 && ch == '\r') ||
			    (matched == 1 && ch == '\n') ||
			    (matched == 2 && ch == '\r') ||
			    (matched == 3 && ch == '\n'))
			{
				matched++;
				if (matched == 4) break;
			}
			else
			{
				matched = (ch == '\r') ? 1 : 0;
			}
		}

		string resp = headerSb.ToString();
		int firstLineEnd = resp.IndexOf('\r');
		string statusLine = (firstLineEnd > 0) ? resp.Substring(0, firstLineEnd) : resp;
		if (!statusLine.Contains(" 200"))
		{
			throw new IOException("HTTP Proxy từ chối kết nối: " + statusLine);
		}
	}

	private static void HandshakeSocks5(NetworkStream stream, string targetHost, int targetPort, string user, string pass)
	{
		bool hasAuth = !string.IsNullOrEmpty(user);
		byte[] greeting = hasAuth
			? new byte[] { 0x05, 0x02, 0x00, 0x02 }
			: new byte[] { 0x05, 0x01, 0x00 };
		stream.Write(greeting, 0, greeting.Length);
		stream.Flush();

		byte[] methodResp = new byte[2];
		ReadExact(stream, methodResp, 0, 2);
		if (methodResp[0] != 0x05 || methodResp[1] == 0xFF)
		{
			throw new IOException("SOCKS5 Proxy không chấp nhận phương thức xác thực.");
		}

		if (methodResp[1] == 0x02)
		{
			byte[] uBytes = Encoding.UTF8.GetBytes(user ?? string.Empty);
			byte[] pBytes = Encoding.UTF8.GetBytes(pass ?? string.Empty);
			if (uBytes.Length > 255 || pBytes.Length > 255)
			{
				throw new IOException("Tài khoản hoặc mật khẩu SOCKS5 vượt quá 255 byte.");
			}
			byte[] authPacket = new byte[3 + uBytes.Length + pBytes.Length];
			authPacket[0] = 0x01;
			authPacket[1] = (byte)uBytes.Length;
			Buffer.BlockCopy(uBytes, 0, authPacket, 2, uBytes.Length);
			authPacket[2 + uBytes.Length] = (byte)pBytes.Length;
			Buffer.BlockCopy(pBytes, 0, authPacket, 3 + uBytes.Length, pBytes.Length);
			stream.Write(authPacket, 0, authPacket.Length);
			stream.Flush();

			byte[] authResp = new byte[2];
			ReadExact(stream, authResp, 0, 2);
			if (authResp[1] != 0x00)
			{
				throw new IOException("Xác thực SOCKS5 thất bại (sai user/pass).");
			}
		}
		else if (methodResp[1] != 0x00)
		{
			throw new IOException("SOCKS5 yêu cầu phương thức xác thực không hỗ trợ: " + methodResp[1]);
		}

		IPAddress parsedTargetIp;
		byte[] connectReq;
		if (IPAddress.TryParse(targetHost, out parsedTargetIp))
		{
			byte[] addrBytes = parsedTargetIp.GetAddressBytes();
			byte atyp = (parsedTargetIp.AddressFamily == AddressFamily.InterNetworkV6) ? (byte)0x04 : (byte)0x01;
			connectReq = new byte[4 + addrBytes.Length + 2];
			connectReq[0] = 0x05;
			connectReq[1] = 0x01;
			connectReq[2] = 0x00;
			connectReq[3] = atyp;
			Buffer.BlockCopy(addrBytes, 0, connectReq, 4, addrBytes.Length);
			connectReq[4 + addrBytes.Length] = (byte)((targetPort >> 8) & 0xFF);
			connectReq[5 + addrBytes.Length] = (byte)(targetPort & 0xFF);
		}
		else
		{
			byte[] domainBytes = Encoding.ASCII.GetBytes(targetHost);
			if (domainBytes.Length > 255)
			{
				throw new IOException("Tên miền máy chủ quá dài cho SOCKS5.");
			}
			connectReq = new byte[5 + domainBytes.Length + 2];
			connectReq[0] = 0x05;
			connectReq[1] = 0x01;
			connectReq[2] = 0x00;
			connectReq[3] = 0x03;
			connectReq[4] = (byte)domainBytes.Length;
			Buffer.BlockCopy(domainBytes, 0, connectReq, 5, domainBytes.Length);
			connectReq[5 + domainBytes.Length] = (byte)((targetPort >> 8) & 0xFF);
			connectReq[6 + domainBytes.Length] = (byte)(targetPort & 0xFF);
		}

		stream.Write(connectReq, 0, connectReq.Length);
		stream.Flush();

		byte[] repHead = new byte[4];
		ReadExact(stream, repHead, 0, 4);
		if (repHead[0] != 0x05 || repHead[1] != 0x00)
		{
			throw new IOException("SOCKS5 không thể kết nối tới máy chủ đích (mã lỗi " + repHead[1] + ").");
		}

		int remBytes;
		if (repHead[3] == 0x01)
		{
			remBytes = 6;
		}
		else if (repHead[3] == 0x04)
		{
			remBytes = 18;
		}
		else if (repHead[3] == 0x03)
		{
			byte[] dLen = new byte[1];
			ReadExact(stream, dLen, 0, 1);
			remBytes = dLen[0] + 2;
		}
		else
		{
			remBytes = 6;
		}

		byte[] discard = new byte[remBytes];
		ReadExact(stream, discard, 0, remBytes);
	}

	/// <summary>
	/// Chèn hoặc cập nhật máy chủ tùy chỉnh vào danh sách máy chủ của ServerListScreen.
	/// </summary>
	public static void InjectCustomServer()
	{
		if (!HasCustomServer()) return;
		if (ServerListScreen.nameServer == null || ServerListScreen.address == null || ServerListScreen.port == null) return;

		string dispName = string.IsNullOrEmpty(customSvName) ? ("SV Riêng (" + customSvIp + ")") : customSvName;
		short shortPort = (short)customSvPort;

		for (int i = 0; i < ServerListScreen.address.Length; i++)
		{
			if (string.Equals(ServerListScreen.nameServer[i], dispName, StringComparison.OrdinalIgnoreCase) ||
			    (string.Equals(ServerListScreen.address[i], customSvIp, StringComparison.OrdinalIgnoreCase) && ServerListScreen.port[i] == shortPort))
			{
				ServerListScreen.nameServer[i] = dispName;
				ServerListScreen.address[i] = customSvIp;
				ServerListScreen.port[i] = shortPort;
				return;
			}
		}

		int oldLen = ServerListScreen.nameServer.Length;
		string[] newNames = new string[oldLen + 1];
		string[] newAddrs = new string[oldLen + 1];
		short[] newPorts = new short[oldLen + 1];
		sbyte[] newLangs = new sbyte[oldLen + 1];
		sbyte[] newTypes = new sbyte[oldLen + 1];
		sbyte[] newIsNew = new sbyte[oldLen + 1];

		Array.Copy(ServerListScreen.nameServer, newNames, oldLen);
		Array.Copy(ServerListScreen.address, newAddrs, oldLen);
		Array.Copy(ServerListScreen.port, newPorts, oldLen);
		if (ServerListScreen.language != null) Array.Copy(ServerListScreen.language, newLangs, System.Math.Min(oldLen, ServerListScreen.language.Length));
		if (ServerListScreen.typeSv != null) Array.Copy(ServerListScreen.typeSv, newTypes, System.Math.Min(oldLen, ServerListScreen.typeSv.Length));
		if (ServerListScreen.isNew != null) Array.Copy(ServerListScreen.isNew, newIsNew, System.Math.Min(oldLen, ServerListScreen.isNew.Length));

		newNames[oldLen] = dispName;
		newAddrs[oldLen] = customSvIp;
		newPorts[oldLen] = shortPort;
		newLangs[oldLen] = mResources.language;
		newTypes[oldLen] = 0;
		newIsNew[oldLen] = 1;

		ServerListScreen.nameServer = newNames;
		ServerListScreen.address = newAddrs;
		ServerListScreen.port = newPorts;
		ServerListScreen.language = newLangs;
		ServerListScreen.typeSv = newTypes;
		ServerListScreen.isNew = newIsNew;
	}

	/// <summary>
	/// Khôi phục chính xác cổng nguyên 1..65535 khi chọn máy chủ tùy chỉnh hoặc cổng > 32767.
	/// </summary>
	public static void ResolveSelectedServerEndpoint(ref string ip, ref int port)
	{
		if (HasCustomServer() && string.Equals(ip, customSvIp, StringComparison.OrdinalIgnoreCase))
		{
			string dispName = string.IsNullOrEmpty(customSvName) ? ("SV Riêng (" + customSvIp + ")") : customSvName;
			if (ServerListScreen.nameServer != null &&
			    ServerListScreen.ipSelect >= 0 &&
			    ServerListScreen.ipSelect < ServerListScreen.nameServer.Length &&
			    string.Equals(ServerListScreen.nameServer[ServerListScreen.ipSelect], dispName, StringComparison.OrdinalIgnoreCase))
			{
				port = customSvPort;
				return;
			}
		}
		if (port < 0)
		{
			port = (ushort)(short)port;
		}
	}

	public static void PaintLobbyUI(mGraphics g)
	{
		if (ModAutoUpdate.isDownloading || ModAutoUpdate.isShowCompletedBoard || ModAutoUpdate.isShowUpdateBoard) return;
		if (GameCanvas.currentDialog != null || (GameCanvas.menu != null && GameCanvas.menu.showMenu)) return;

		int x = GetBtnX();
		int y = GetBtnY();
		int w = GetBtnW();
		int h = GetBtnH();
		bool active = HasProxy() || HasCustomServer();
		PopUp.paintPopUp(g, x, y, w, h, active ? 1 : 0, isButton: true);
		string label = HasProxy() ? "Proxy: BẬT" : (HasCustomServer() ? "SV: RIÊNG" : "Proxy / SV");
		mFont.tahoma_7b_dark.drawString(g, label, x + w / 2, y + 4, mFont.CENTER);
	}

	public static bool UpdateLobbyInput()
	{
		if (ModAutoUpdate.isDownloading || ModAutoUpdate.isShowCompletedBoard || ModAutoUpdate.isShowUpdateBoard) return false;
		if (GameCanvas.currentDialog != null || (GameCanvas.menu != null && GameCanvas.menu.showMenu)) return false;

		if (GameCanvas.isPointerJustRelease && GameCanvas.isPointerClick)
		{
			int x = GetBtnX();
			int y = GetBtnY();
			int w = GetBtnW();
			int h = GetBtnH();
			if (GameCanvas.px >= x && GameCanvas.px <= x + w && GameCanvas.py >= y && GameCanvas.py <= y + h)
			{
				GameCanvas.clearAllPointerEvent();
				SoundMn.gI().buttonClick();
				OpenConfigMenu();
				return true;
			}
		}
		return false;
	}

	public static void OpenConfigMenu()
	{
		MyVector items = new MyVector();
		items.addElement(new Command(HasProxy() ? "Đổi Proxy\n(" + ShortenText(proxyRaw, 14) + ")" : "Cài Proxy\n(SOCKS5/HTTP)", gI(), ACT_OPEN_PROXY_INPUT, null));
		if (HasProxy())
		{
			items.addElement(new Command("Xóa Proxy", gI(), ACT_CLEAR_PROXY, null));
		}
		items.addElement(new Command(HasCustomServer() ? ("Đổi SV Riêng\n(" + customSvIp + ":" + customSvPort + ")") : "Thêm Server\n(Tên:IP:Port)", gI(), ACT_OPEN_SV_INPUT, null));
		if (HasCustomServer())
		{
			items.addElement(new Command("Xóa SV Riêng", gI(), ACT_CLEAR_SV, null));
		}
		GameCanvas.menu.startAt(items, 0);
	}

	private static string ShortenText(string text, int maxLen)
	{
		if (string.IsNullOrEmpty(text)) return string.Empty;
		return text.Length <= maxLen ? text : (text.Substring(0, maxLen) + "..");
	}

	public void perform(int idAction, object p)
	{
		switch (idAction)
		{
			case ACT_OPEN_PROXY_INPUT:
				GameCanvas.inputDlg.show(
					"Nhập Proxy (host:port[:user:pass] hoặc http://...)",
					new Command(mResources.OK, this, ACT_SAVE_PROXY_INPUT, null),
					TField.INPUT_TYPE_ANY);
				GameCanvas.inputDlg.tfInput.setText(proxyRaw ?? string.Empty);
				break;

			case ACT_SAVE_PROXY_INPUT:
			{
				string raw = (GameCanvas.inputDlg.tfInput.getText() ?? string.Empty).Trim();
				GameCanvas.endDlg();
				if (raw.Length == 0)
				{
					proxyRaw = string.Empty;
					ModConfig.SaveConfig();
					Session_ME.gI().close();
					GameCanvas.startOKDlg("Đã tắt Proxy.");
					break;
				}
				bool isHttp;
				string host;
				int port;
				string u;
				string pw;
				if (!TryParseProxy(raw, out isHttp, out host, out port, out u, out pw))
				{
					GameCanvas.startOKDlg("Sai định dạng! Ví dụ: 127.0.0.1:1080 hoặc http://ip:port:user:pass");
					break;
				}
				proxyRaw = raw;
				ModConfig.SaveConfig();
				Session_ME.gI().close();
				GameCanvas.startOKDlg("Đã lưu " + (isHttp ? "HTTP" : "SOCKS5") + " Proxy: " + host + ":" + port);
				break;
			}

			case ACT_CLEAR_PROXY:
				proxyRaw = string.Empty;
				ModConfig.SaveConfig();
				Session_ME.gI().close();
				GameCanvas.startOKDlg("Đã xóa cấu hình Proxy.");
				break;

			case ACT_OPEN_SV_INPUT:
			{
				string cur = HasCustomServer()
					? ((string.IsNullOrEmpty(customSvName) ? "SV Riêng" : customSvName) + ":" + customSvIp + ":" + customSvPort)
					: string.Empty;
				GameCanvas.inputDlg.show(
					"Nhập Server (Tên:IP:Port hoặc IP:Port)",
					new Command(mResources.OK, this, ACT_SAVE_SV_INPUT, null),
					TField.INPUT_TYPE_ANY);
				GameCanvas.inputDlg.tfInput.setText(cur);
				break;
			}

			case ACT_SAVE_SV_INPUT:
			{
				string rawSv = (GameCanvas.inputDlg.tfInput.getText() ?? string.Empty).Trim();
				GameCanvas.endDlg();
				if (rawSv.Length == 0)
				{
					break;
				}
				string[] parts = rawSv.Split(':');
				string svName;
				string svIp;
				int svPort;
				if (parts.Length == 2)
				{
					svIp = parts[0].Trim();
					if (!int.TryParse(parts[1].Trim(), out svPort) || svPort <= 0 || svPort > 65535 || svIp.Length == 0)
					{
						GameCanvas.startOKDlg("Sai định dạng! Ví dụ: SV Test:127.0.0.1:14445");
						break;
					}
					svName = "SV Riêng (" + svIp + ")";
				}
				else if (parts.Length >= 3)
				{
					svName = parts[0].Trim();
					svIp = parts[1].Trim();
					if (!int.TryParse(parts[2].Trim(), out svPort) || svPort <= 0 || svPort > 65535 || svIp.Length == 0)
					{
						GameCanvas.startOKDlg("Sai định dạng! Ví dụ: SV Test:127.0.0.1:14445");
						break;
					}
					if (svName.Length == 0) svName = "SV Riêng (" + svIp + ")";
				}
				else
				{
					GameCanvas.startOKDlg("Sai định dạng! Ví dụ: SV Test:127.0.0.1:14445");
					break;
				}

				customSvName = svName;
				customSvIp = svIp;
				customSvPort = svPort;
				ModConfig.SaveConfig();
				InjectCustomServer();
				if (ServerListScreen.nameServer != null)
				{
					for (int i = 0; i < ServerListScreen.nameServer.Length; i++)
					{
						if (string.Equals(ServerListScreen.nameServer[i], customSvName, StringComparison.OrdinalIgnoreCase))
						{
							ServerListScreen.ipSelect = i;
							Rms.saveRMSInt(ServerListScreen.RMS_svselect, i);
							break;
						}
					}
				}
				Session_ME.gI().close();
				if (GameCanvas.serverScreen != null)
				{
					GameCanvas.serverScreen.selectServer();
				}
				GameCanvas.startOKDlg("Đã chọn máy chủ: " + customSvName + " (" + customSvIp + ":" + customSvPort + ")");
				break;
			}

			case ACT_CLEAR_SV:
				customSvName = string.Empty;
				customSvIp = string.Empty;
				customSvPort = 14445;
				ModConfig.SaveConfig();
				Session_ME.gI().close();
				ServerListScreen.getServerList(ServerListScreen.linkDefault);
				if (GameCanvas.serverScreen != null)
				{
					GameCanvas.serverScreen.switchToMe();
				}
				GameCanvas.startOKDlg("Đã xóa Server riêng và khôi phục danh sách gốc.");
				break;
		}
	}
}
