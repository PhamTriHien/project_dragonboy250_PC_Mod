using System;
using Assets.src.g;
using UnityEngine;

public partial class Service
{
	private ISession session = Session_ME.gI();

	protected static Service instance;

	public static long curCheckController;

	public static long curCheckMap;

	public static long logController;

	public static long logMap;

	public int demGui;

	public static bool reciveFromMainSession;

	public static Service gI()
		{
			if (instance == null)
			{
				instance = new Service();
			}
			return instance;
		}

	public void charInfo(string day, string month, string year, string address, string cmnd, string dayCmnd, string noiCapCmnd, string sdt, string name)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)42);
				message.writer().writeUTF(day);
				message.writer().writeUTF(month);
				message.writer().writeUTF(year);
				message.writer().writeUTF(address);
				message.writer().writeUTF(cmnd);
				message.writer().writeUTF(dayCmnd);
				message.writer().writeUTF(noiCapCmnd);
				message.writer().writeUTF(sdt);
				message.writer().writeUTF(name);
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

	public void checkAd(sbyte status)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-44));
				message.writer().writeByte(status);
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

	public void test(short x, short y)
		{
			Res.outz("gui x= " + x + " y= " + y);
			Message message = null;
			try
			{
				message = new Message(0);
				message.writer().writeShort(x);
				message.writer().writeShort(y);
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

	public void test2()
		{
			Res.outz("gui test1");
			Message message = null;
			try
			{
				message = new Message(1);
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

	public void testJoint()
		{
		}

	public void mobCapcha(char ch)
		{
			Res.outz("cap char c= " + ch);
			Message message = null;
			try
			{
				message = new Message((sbyte)(-85));
				message.writer().writeChar(ch);
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

	public void getArchivemnt(int index)
		{
			Res.outz("get ngoc");
			Message message = null;
			try
			{
				message = new Message((sbyte)(-76));
				message.writer().writeByte(index);
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

	public void updateCaption(sbyte gender)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-41));
				message.writer().writeByte(gender);
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

	public Message messageNotMap(sbyte command)
		{
			Message message = new Message((sbyte)(-28));
			message.writer().writeByte(command);
			return message;
		}

	public static Message messageSubCommand(sbyte command)
		{
			Message message = new Message((sbyte)(-30));
			message.writer().writeByte(command);
			return message;
		}

	public void sendCheckController()
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-120));
				session.sendMessage(message);
			}
			catch (Exception)
			{
			}
			finally
			{
				curCheckController = mSystem.currentTimeMillis();
				message.cleanup();
			}
		}

	public void sendCheckMap()
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-121));
				session.sendMessage(message);
			}
			catch (Exception)
			{
			}
			finally
			{
				curCheckMap = mSystem.currentTimeMillis();
				message.cleanup();
			}
		}

	public static long lastCharMoveTime = 0;

	public void createChar(string name, int gender, int hair)
		{
			Message message = new Message((sbyte)(-28));
			try
			{
				message.writer().writeByte((sbyte)2);
				message.writer().writeUTF(name);
				message.writer().writeByte(gender);
				message.writer().writeByte(hair);
			}
			catch (Exception ex)
			{
				Cout.println(ex.Message + ex.StackTrace);
			}
			session.sendMessage(message);
		}

	public void getEffData(short id)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-66));
				message.writer().writeShort(id);
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

	public void confirmMenu(short npcID, sbyte select)
		{
			Res.outz("confirme menu" + select);
			Message message = null;
			try
			{
				message = new Message((sbyte)32);
				message.writer().writeShort(npcID);
				message.writer().writeByte(select);
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

	public void openMenu(int npcId)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)33);
				message.writer().writeShort(npcId);
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

	public void menu(int npcId, int menuId, int optionId)
		{
			Cout.println("menuid: " + menuId);
			Message message = null;
			try
			{
				message = new Message((sbyte)22);
				message.writer().writeByte(npcId);
				message.writer().writeByte(menuId);
				message.writer().writeByte(optionId);
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

	public void menuId(short menuId)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)27);
				message.writer().writeShort(menuId);
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

	public void textBoxId(short menuId, string str)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)88);
				message.writer().writeShort(menuId);
				message.writer().writeUTF(str);
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

	public void boxSort()
		{
			Message message = null;
			try
			{
				message = messageSubCommand(19);
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

	public void boxCoinOut(int coinOut)
		{
			Message message = null;
			try
			{
				message = messageSubCommand(21);
				message.writer().writeInt(coinOut);
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

	public void returnTownFromDead()
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-15));
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

	public void wakeUpFromDead()
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-16));
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

	public void updateData()
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-87));
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
				Cout.println(ex.Message + ex.StackTrace);
			}
			finally
			{
				message.cleanup();
			}
		}

	public void updateMap()
		{
			Message message = null;
			try
			{
				message = messageNotMap(6);
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
				Cout.println(ex.Message + ex.StackTrace);
			}
			finally
			{
				message.cleanup();
			}
		}

	public void testInvite(int charId)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)59);
				message.writer().writeInt(charId);
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

	public void addCuuSat(int charId)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)62);
				message.writer().writeInt(charId);
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

	public void finishUpdate()
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-38));
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

	public void finishUpdate(int playerID)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-38));
				message.writer().writeInt(playerID);
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

	public void getResource(sbyte action, MyVector vResourceIndex)
		{
			Res.outz("request resource action= " + action);
			Message message = null;
			try
			{
				message = new Message((sbyte)(-74));
				message.writer().writeByte(action);
				if (action == 2 && vResourceIndex != null)
				{
					message.writer().writeShort(vResourceIndex.size());
					for (int i = 0; i < vResourceIndex.size(); i++)
					{
						message.writer().writeShort(short.Parse((string)vResourceIndex.elementAt(i)));
					}
				}
				if (Session_ME2.gI().isConnected() && !Session_ME2.connecting)
				{
					session = Session_ME2.gI();
				}
				else
				{
					reciveFromMainSession = true;
					session = Session_ME.gI();
				}
				session.sendMessage(message);
				session = Session_ME.gI();
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

	public void imageSource(MyVector vID)
		{
			Message message = null;
			try
			{
				Res.outz("IMAGE SOURCE size= " + vID.size());
				message = new Message((sbyte)(-111));
				message.writer().writeShort(vID.size());
				if (vID.size() > 0)
				{
					for (int i = 0; i < vID.size(); i++)
					{
						Res.outz("gui len str " + ((ImageSource)vID.elementAt(i)).id);
						message.writer().writeUTF(((ImageSource)vID.elementAt(i)).id);
					}
				}
				if (Session_ME2.gI().isConnected() && !Session_ME2.connecting)
				{
					session = Session_ME2.gI();
				}
				else
				{
					session = Session_ME.gI();
					reciveFromMainSession = true;
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

	public void sendServerData(sbyte action, int id, sbyte[] data)
		{
			Message message = null;
			try
			{
				Res.outz("SERVER DATA");
				message = new Message((sbyte)(-110));
				message.writer().writeByte(action);
				if (action == 1)
				{
					message.writer().writeInt(id);
					if (data != null)
					{
						int num = data.Length;
						message.writer().writeShort(num);
						message.writer().write(ref data, 0, num);
					}
				}
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

	public void sendCmdExtra(sbyte sub, string user, string pass)
		{
			Message message = new Message((sbyte)24);
			try
			{
				message.writer().writeByte(sub);
				if (sub == sbyte.MaxValue)
				{
					message.writer().writeUTF(user);
					message.writer().writeUTF(pass);
					Controller.isEXTRA_LINK = false;
					Res.err(" =====> SEND EXTRA_LINK " + sub + " user:" + user + " pass:" + pass);
				}
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
