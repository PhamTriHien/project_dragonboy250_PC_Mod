using System;
using System.Collections.Generic;
using Assets.src.g;
using UnityEngine;

/// <summary>
/// Thành phần phân mảnh (partial) của lớp Service: Quản lý xác thực tài khoản, phiên kết nối và đăng nhập máy chủ.
/// </summary>
public partial class Service
{
	public static volatile bool isSendClientType = false;

	public static volatile bool isSendClientType2 = false;

	private static readonly object clientTypeSendLock = new object();

	private static readonly Queue<Message> pendingLoginMessages = new Queue<Message>();

	private static long lastClientTypeConnectionGeneration = long.MinValue;

	private static long lastClientType2ConnectionGeneration = long.MinValue;

	private static long imageSourceReadyConnectionGeneration = long.MinValue;

	/// <summary>
	/// Thực thi phương thức nghiệp vụ androidPack.
	/// </summary>
	public void androidPack()
	{
		if (mSystem.android_pack == null)
		{
			return;
		}
		Message message = null;
		try
		{
			message = new Message((sbyte)126);
			message.writer().writeUTF(mSystem.android_pack);
			session.sendMessage(message);
		}
		catch (Exception ex)
		{
			ex.StackTrace.ToString();
		}
		finally
		{
			if (message != null)
			{
				message.cleanup();
			}
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ androidPack2.
	/// </summary>
	public void androidPack2()
	{
		if (mSystem.android_pack == null)
		{
			return;
		}
		Message message = null;
		try
		{
			message = new Message((sbyte)126);
			message.writer().writeUTF(mSystem.android_pack);
			if (Session_ME2.gI().isConnected() && !Session_ME2.connecting)
			{
				session = Session_ME2.gI();
			}
			else
			{
				session = Session_ME.gI();
			}
			session.sendMessage(message);
			session = Session_ME.gI();
		}
		catch (Exception ex)
		{
			ex.StackTrace.ToString();
		}
		finally
		{
			if (message != null)
			{
				message.cleanup();
			}
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ messageNotLogin.
	/// </summary>
	public Message messageNotLogin(sbyte command)
	{
		Message message = new Message((sbyte)(-29));
		message.writer().writeByte(command);
		return message;
	}

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setClientType.
	/// </summary>
	public bool setClientType(bool useSecondarySession = false)
	{
		lock (clientTypeSendLock)
		{
			ISession targetSession = useSecondarySession ? (ISession)Session_ME2.gI() : (ISession)Session_ME.gI();
			if (targetSession == null || !targetSession.isConnected())
			{
				return false;
			}
			long generation = useSecondarySession ? Session_ME2.ConnectionGeneration : Session_ME.ConnectionGeneration;
			bool alreadySent = useSecondarySession
				? (isSendClientType2 || lastClientType2ConnectionGeneration == generation)
				: (isSendClientType || lastClientTypeConnectionGeneration == generation);
			if (alreadySent)
			{
				if (useSecondarySession)
				{
					isSendClientType2 = true;
					return true;
				}
				isSendClientType = true;
				return !isLoginSendReadyLocked(targetSession) || flushPendingLoginMessagesLocked(targetSession);
			}
			if (Rms.loadRMSInt(Rms.RMS_clienttype) != -1)
			{
				mSystem.clientType = Rms.loadRMSInt(Rms.RMS_clienttype);
			}
			try
			{
				string svName = (ServerListScreen.nameServer != null
					&& ServerListScreen.ipSelect >= 0
					&& ServerListScreen.ipSelect < ServerListScreen.nameServer.Length)
					? ServerListScreen.nameServer[ServerListScreen.ipSelect]
					: string.Empty;
				Debug.LogError((useSecondarySession ? ">>send ClientType2" : ">>send ClientType1")
					+ "  >>" + ServerListScreen.ipSelect + "  sv:" + svName);
				sbyte wireClientType = (sbyte)((mSystem.clientType == 2) ? 4 : mSystem.clientType);
				Message message = messageNotLogin(2);
				message.writer().writeByte(wireClientType);
				message.writer().writeByte(mGraphics.zoomLevel);
				message.writer().writeBoolean(value: false);
				message.writer().writeInt(GameCanvas.w);
				message.writer().writeInt(GameCanvas.h);
				message.writer().writeBoolean(TField.isQwerty);
				message.writer().writeBoolean(GameCanvas.isTouch);
				message.writer().writeUTF(GameCanvas.getPlatformName() + "|" + GameMidlet.VERSION);
				DataInputStream dataInputStream = MyStream.readFile("/info");
				if (dataInputStream != null)
				{
					sbyte[] data = new sbyte[dataInputStream.r.buffer.Length];
					dataInputStream.read(ref data);
					if (data != null)
					{
						message.writer().writeShort(data.Length);
						message.writer().write(data);
					}
				}
				targetSession.sendMessage(message);
				message.cleanup();
				if (useSecondarySession)
				{
					isSendClientType2 = true;
					lastClientType2ConnectionGeneration = generation;
					return true;
				}
				isSendClientType = true;
				lastClientTypeConnectionGeneration = generation;
				return !isLoginSendReadyLocked(targetSession) || flushPendingLoginMessagesLocked(targetSession);
			}
			catch (Exception ex)
			{
				Cout.println(ex.Message + ex.StackTrace);
				return false;
			}
		}
	}

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho setClientType2.
	/// </summary>
	public void setClientType2()
	{
		setClientType(useSecondarySession: true);
	}

	public static void resetClientTypeSendState(bool isMainSession)
	{
		lock (clientTypeSendLock)
		{
			if (isMainSession)
			{
				isSendClientType = false;
				lastClientTypeConnectionGeneration = long.MinValue;
				imageSourceReadyConnectionGeneration = long.MinValue;
			}
			else
			{
				isSendClientType2 = false;
				lastClientType2ConnectionGeneration = long.MinValue;
			}
		}
	}

	public static void resetClientTypeSendState()
	{
		lock (clientTypeSendLock)
		{
			isSendClientType = false;
			isSendClientType2 = false;
			lastClientTypeConnectionGeneration = long.MinValue;
			lastClientType2ConnectionGeneration = long.MinValue;
			imageSourceReadyConnectionGeneration = long.MinValue;
			while (pendingLoginMessages.Count > 0)
			{
				Message msg = pendingLoginMessages.Dequeue();
				if (msg != null)
				{
					msg.cleanup();
				}
			}
		}
	}

	public static bool markImageSourceReadyAndFlushPendingLogin()
	{
		lock (clientTypeSendLock)
		{
			ISession targetSession = Session_ME.gI();
			long generation = Session_ME.ConnectionGeneration;
			if (targetSession == null || !targetSession.isConnected() || lastClientTypeConnectionGeneration != generation)
			{
				return false;
			}
			imageSourceReadyConnectionGeneration = generation;
			int pendingCount = pendingLoginMessages.Count;
			if (pendingCount > 0)
			{
				flushPendingLoginMessagesLocked(targetSession);
			}
			return pendingCount > 0;
		}
	}

	public static bool isLoginReadyForCurrentConnection()
	{
		lock (clientTypeSendLock)
		{
			return isLoginSendReadyLocked(Session_ME.gI());
		}
	}

	private static bool isLoginSendReadyLocked(ISession targetSession)
	{
		if (targetSession == null || !targetSession.isConnected())
		{
			return false;
		}
		long generation = Session_ME.ConnectionGeneration;
		return lastClientTypeConnectionGeneration == generation
			&& imageSourceReadyConnectionGeneration == generation;
	}

	private static bool flushPendingLoginMessagesLocked(ISession targetSession)
	{
		try
		{
			while (pendingLoginMessages.Count > 0)
			{
				Message message = pendingLoginMessages.Peek();
				targetSession.sendMessage(message);
				pendingLoginMessages.Dequeue();
				if (message != null)
				{
					message.cleanup();
				}
			}
			return true;
		}
		catch (Exception ex)
		{
			Cout.println(ex.Message + ex.StackTrace);
			return false;
		}
	}

	private void sendLoginMessage(Message message)
	{
		if (message == null)
		{
			return;
		}
		lock (clientTypeSendLock)
		{
			if (isLoginSendReadyLocked(session) && flushPendingLoginMessagesLocked(session))
			{
				session.sendMessage(message);
				message.cleanup();
			}
			else
			{
				pendingLoginMessages.Enqueue(message);
			}
		}
	}

	/// <summary>
	/// Thực hiện gửi yêu cầu đăng nhập tài khoản lên máy chủ.
	/// </summary>
	public void login(string username, string pass, string version, sbyte type)
	{
		Res.outz("Login " + username + " " + pass + " " + version);
		string loginCode = LoginIdConfig.getLoginCode();
		if (string.IsNullOrEmpty(loginCode))
		{
			Debug.LogError("[LOGIN] Cannot send login packet; login_codes_generated.txt is missing or invalid.");
			return;
		}
		Message message = null;
		try
		{
			ModTeleport.ResetSessionColdCounters();
			message = messageNotLogin(0);
			message.writer().writeUTF(loginCode);
			message.writer().writeUTF(username);
			message.writer().writeUTF(pass);
			message.writer().writeUTF(version);
			message.writer().writeSByte(type);
			sendLoginMessage(message);
			message = null;
		}
		catch (Exception ex)
		{
			Cout.println(ex.Message + ex.StackTrace);
		}
		finally
		{
			if (message != null)
			{
				message.cleanup();
			}
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ clientOk.
	/// </summary>
	public void clientOk()
	{
		Message message = null;
		try
		{
			message = messageNotMap(13);
			session.sendMessage(message);
		}
		catch (Exception ex)
		{
			Cout.println(ex.Message + ex.StackTrace);
		}
		finally
		{
			if (message != null)
			{
				message.cleanup();
			}
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ login2.
	/// </summary>
	public void login2(string user)
	{
		Res.outz("Login 2:  " + user);
		Message message = null;
		try
		{
			message = new Message((sbyte)(-101));
			message.writer().writeUTF(user);
			message.writer().writeByte(1);
			sendLoginMessage(message);
			message = null;
		}
		catch (Exception)
		{
		}
		finally
		{
			if (message != null)
			{
				message.cleanup();
			}
		}
	}
}
