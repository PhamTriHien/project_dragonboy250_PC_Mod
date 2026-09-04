using System;
using Assets.src.g;
using UnityEngine;

public partial class Service
{
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
				message.cleanup();
			}
		}

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
				message.cleanup();
			}
		}

	public Message messageNotLogin(sbyte command)
		{
			Message message = new Message((sbyte)(-29));
			message.writer().writeByte(command);
			return message;
		}

	public void setClientType()
		{
			if (Rms.loadRMSInt(Rms.RMS_clienttype) != -1)
			{
				mSystem.clientType = Rms.loadRMSInt(Rms.RMS_clienttype);
			}
			try
			{
				Res.outz(">>send ClientType1");
				Message message = messageNotLogin(2);
				message.writer().writeByte(mSystem.clientType);
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
				session.sendMessage(message);
				message.cleanup();
			}
			catch (Exception ex)
			{
				Cout.println(ex.Message + ex.StackTrace);
			}
		}

	public void setClientType2()
		{
			Res.outz("SET CLIENT TYPE");
			if (Rms.loadRMSInt(Rms.RMS_clienttype) != -1)
			{
				mSystem.clientType = Rms.loadRMSInt(Rms.RMS_clienttype);
			}
			try
			{
				Res.outz(">>send ClientType2");
				Message message = messageNotLogin(2);
				message.writer().writeByte(mSystem.clientType);
				message.writer().writeByte(mGraphics.zoomLevel);
				Res.outz("gui zoomlevel = " + mGraphics.zoomLevel);
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
				session = Session_ME2.gI();
				session.sendMessage(message);
				session = Session_ME.gI();
				message.cleanup();
			}
			catch (Exception ex)
			{
				ex.StackTrace.ToString();
			}
		}

	public void login(string username, string pass, string version, sbyte type)
		{
			Res.outz("Login " + username + " " + pass + " " + version);
			Debug.LogError("Login " + username + " " + pass + " " + version);
			try
			{
				Message message = messageNotLogin(0);
				message.writer().writeUTF(username);
				message.writer().writeUTF(pass);
				message.writer().writeUTF(version);
				message.writer().writeByte(type);
				session.sendMessage(message);
				message.cleanup();
			}
			catch (Exception ex)
			{
				Cout.println(ex.Message + ex.StackTrace);
			}
		}

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
				message.cleanup();
			}
		}

	public void login2(string user)
		{
			Res.outz("Login 2:  " + user);
			Message message = null;
			try
			{
				message = new Message((sbyte)(-101));
				message.writer().writeUTF(user);
				message.writer().writeByte(1);
				session.sendMessage(message);
			}
			catch (Exception)
			{
			}
			finally
			{
				message.cleanup();
			}
		}

}
