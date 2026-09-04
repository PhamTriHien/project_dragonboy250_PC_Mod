using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public partial class Session_ME
{
	public void sendMessage(Message message)
		{
			count++;
			Res.outz("SEND MSG: " + message.command);
			lastSendTime = mSystem.currentTimeMillis();
			sender.AddMessage(message);
		}

	private static void doSendMessage(Message m)
		{
			sbyte[] data = m.getData();
			try
			{
				if (getKeyComplete)
				{
					sbyte value = writeKey(m.command);
					dos.Write(value);
				}
				else
				{
					dos.Write(m.command);
				}
				if (data != null)
				{
					int num = data.Length;
					if (getKeyComplete)
					{
						int num2 = writeKey((sbyte)(num >> 8));
						dos.Write((sbyte)num2);
						int num3 = writeKey((sbyte)(num & 0xFF));
						dos.Write((sbyte)num3);
					}
					else
					{
						dos.Write((ushort)num);
					}
					if (getKeyComplete)
					{
						for (int i = 0; i < data.Length; i++)
						{
							sbyte value2 = writeKey(data[i]);
							dos.Write(value2);
						}
					}
					sendByteCount += 5 + data.Length;
				}
				else
				{
					if (getKeyComplete)
					{
						int num4 = 0;
						int num5 = writeKey((sbyte)(num4 >> 8));
						dos.Write((sbyte)num5);
						int num6 = writeKey((sbyte)(num4 & 0xFF));
						dos.Write((sbyte)num6);
					}
					else
					{
						dos.Write((ushort)0);
					}
					sendByteCount += 5;
				}
				dos.Flush();
			}
			catch (Exception ex)
			{
				Debug.Log(ex.StackTrace);
				dos.Flush();
			}
		}

	public static sbyte readKey(sbyte b)
		{
			sbyte[] array = key;
			sbyte num = curR;
			curR = (sbyte)(num + 1);
			sbyte result = (sbyte)((array[num] & 0xFF) ^ (b & 0xFF));
			if (curR >= key.Length)
			{
				curR %= (sbyte)key.Length;
			}
			return result;
		}

	public static sbyte writeKey(sbyte b)
		{
			sbyte[] array = key;
			sbyte num = curW;
			curW = (sbyte)(num + 1);
			sbyte result = (sbyte)((array[num] & 0xFF) ^ (b & 0xFF));
			if (curW >= key.Length)
			{
				curW %= (sbyte)key.Length;
			}
			return result;
		}

	public static void onRecieveMsg(Message msg)
		{
			if (Thread.CurrentThread.Name == Main.mainThreadName)
			{
				messageHandler.onMessage(msg);
			}
			else
			{
				lock (recieveMsg)
				{
					recieveMsg.addElement(msg);
				}
			}
		}

	public static void update()
		{
			while (true)
			{
				Message message = null;
				lock (recieveMsg)
				{
					if (recieveMsg.size() > 0)
					{
						message = (Message)recieveMsg.elementAt(0);
						recieveMsg.removeElementAt(0);
					}
				}
				if (message == null || Controller.isStopReadMessage)
				{
					break;
				}
				if (lastSendTime > 0)
				{
					long rtt = mSystem.currentTimeMillis() - lastSendTime;
					if (rtt >= 1 && rtt <= 800)
					{
						ModMenu.pingMs = (int)rtt;
					}
					lastSendTime = 0;
				}
				messageHandler.onMessage(message);
			}
		}

	public void close()
		{
			cleanNetwork();
		}

	private static void cleanNetwork()
		{
			key = null;
			curR = 0;
			curW = 0;
			Debug.LogError(">>>cleanNetwork ...!");
			try
			{
				connected = false;
				connecting = false;
				if (sc != null)
				{
					sc.Close();
					sc = null;
				}
				if (dataStream != null)
				{
					dataStream.Close();
					dataStream = null;
				}
				if (dos != null)
				{
					dos.Close();
					dos = null;
				}
				if (dis != null)
				{
					dis.Close();
					dis = null;
				}
				if (Thread.CurrentThread.Name == Main.mainThreadName)
				{
					if (sendThread != null)
					{
						sendThread.Abort();
					}
					sendThread = null;
					if (initThread != null)
					{
						initThread.Abort();
					}
					initThread = null;
					if (collectorThread != null)
					{
						collectorThread.Abort();
					}
					collectorThread = null;
				}
				else
				{
					sendThread = null;
					initThread = null;
					collectorThread = null;
				}
				if (isMainSession)
				{
					ServerListScreen.testConnect = 0;
				}
				instance.timeWaitConnect = 0;
				Controller.isGet_CLIENT_INFO = false;
			}
			catch (Exception)
			{
			}
		}

	public static byte convertSbyteToByte(sbyte var)
		{
			if (var > 0)
			{
				return (byte)var;
			}
			return (byte)(var + 256);
		}

	public static byte[] convertSbyteToByte(sbyte[] var)
		{
			byte[] array = new byte[var.Length];
			for (int i = 0; i < var.Length; i++)
			{
				if (var[i] > 0)
				{
					array[i] = (byte)var[i];
				}
				else
				{
					array[i] = (byte)(var[i] + 256);
				}
			}
			return array;
		}

	public bool isCompareIPConnect()
		{
			return true;
		}

}
