using System;
using Assets.src.g;
using UnityEngine;

public partial class Service
{
	public void giaodich(sbyte action, int playerID, sbyte index, int num)
		{
			Res.outz2("giao dich action = " + action);
			Message message = null;
			try
			{
				message = new Message((sbyte)(-86));
				message.writer().writeByte(action);
				if (action == 0 || action == 1)
				{
					Res.outz2(">>>> len playerID =" + playerID);
					message.writer().writeInt(playerID);
				}
				if (action == 2)
				{
					Res.outz2("gui len index =" + index + " num= " + num);
					message.writer().writeByte(index);
					message.writer().writeInt(num);
				}
				if (action == 4)
				{
					Res.outz2(">>>> len index =" + index);
					message.writer().writeByte(index);
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

	public void sendClientInput(TField[] t)
		{
			Message message = null;
			try
			{
				Res.outz(" gui input ");
				message = new Message((sbyte)(-125));
				Res.outz("byte lent = " + t.Length);
				message.writer().writeByte(t.Length);
				for (int i = 0; i < t.Length; i++)
				{
					message.writer().writeUTF(t[i].getText());
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

	public void friend(sbyte action, int playerId)
		{
			Res.outz("add friend");
			Message message = null;
			try
			{
				message = new Message((sbyte)(-80));
				message.writer().writeByte(action);
				if (playerId != -1)
				{
					message.writer().writeInt(playerId);
				}
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

	public void getPlayerMenu(int playerID)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-79));
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

	public void clanDonate(int id)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-54));
				message.writer().writeInt(id);
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

	public void clanMessage(int type, string text, int clanID)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-51));
				message.writer().writeByte(type);
				if (type == 0)
				{
					message.writer().writeUTF(text);
				}
				if (type == 2)
				{
					message.writer().writeInt(clanID);
				}
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

	public void joinClan(int id, sbyte action)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-49));
				message.writer().writeInt(id);
				message.writer().writeByte(action);
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

	public void clanMember(int id)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-50));
				message.writer().writeInt(id);
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

	public void searchClan(string text)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-47));
				message.writer().writeUTF(text);
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

	public void clanRemote(int id, sbyte role)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-56));
				message.writer().writeInt(id);
				message.writer().writeByte(role);
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

	public void leaveClan()
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-55));
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

	public void clanInvite(sbyte action, int playerID, int clanID, int code)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-57));
				message.writer().writeByte(action);
				if (action == 0)
				{
					message.writer().writeInt(playerID);
				}
				if (action == 1 || action == 2)
				{
					message.writer().writeInt(clanID);
					message.writer().writeInt(code);
				}
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

	public void getClan(sbyte action, int id, string text)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-46));
				message.writer().writeByte(action);
				if (action == 2 || action == 4)
				{
					message.writer().writeShort((short)id);
					message.writer().writeUTF(text);
				}
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

	public void selectCharToPlay(string charname)
		{
			Message message = new Message((sbyte)(-28));
			try
			{
				message.writer().writeByte((sbyte)1);
				message.writer().writeUTF(charname);
			}
			catch (Exception ex)
			{
				Cout.println(ex.Message + ex.StackTrace);
			}
			session.sendMessage(message);
		}

	public void acceptInviteTrade(int playerMapId)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)37);
				message.writer().writeInt(playerMapId);
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

	public void cancelInviteTrade()
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)50);
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

	public void tradeAccept()
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)39);
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

	public void tradeInvite(int charId)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)36);
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

	public void addFriend(string name)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)53);
				message.writer().writeUTF(name);
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

	public void addPartyAccept(int charId)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)76);
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

	public void addPartyCancel(int charId)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)77);
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

	public void addParty(string name)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)75);
				message.writer().writeUTF(name);
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

	public void player_vs_player(sbyte action, sbyte type, int playerId)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-59));
				message.writer().writeByte(action);
				message.writer().writeByte(type);
				message.writer().writeInt(playerId);
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

	public void outParty()
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)79);
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

	public void pleaseInputParty(string str)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)16);
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

	public void acceptPleaseParty(string str)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)17);
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

	public void sendCardInfo(string NAP, string PIN)
		{
			Message message = null;
			try
			{
				message = messageNotMap(16);
				message.writer().writeUTF(NAP);
				message.writer().writeUTF(PIN);
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

	public void saveRms(string key, sbyte[] data)
		{
			Message message = null;
			try
			{
				message = messageSubCommand(60);
				message.writer().writeUTF(key);
				message.writer().writeInt(data.Length);
				message.writer().write(data);
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

	public void loadRMS(string key)
		{
			Cout.println("REQUEST RMS");
			Message message = null;
			try
			{
				message = messageSubCommand(61);
				message.writer().writeUTF(key);
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

	public void changeName(string name, int id)
		{
			Message message = null;
			try
			{
				message = messageNotMap(18);
				message.writer().writeInt(id);
				message.writer().writeUTF(name);
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

	public void inviteClanDun(string name)
		{
			Message message = null;
			try
			{
				message = messageNotMap(34);
				message.writer().writeUTF(name);
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

	public void activeAccProtect(int pass)
		{
			Message message = null;
			try
			{
				message = messageNotMap(37);
				message.writer().writeInt(pass);
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

	public void clearAccProtect(int pass)
		{
			Message message = null;
			try
			{
				message = messageNotMap(41);
				message.writer().writeInt(pass);
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

	public void updateActive(int passOld, int passNew)
		{
			Message message = null;
			try
			{
				message = messageNotMap(38);
				message.writer().writeInt(passOld);
				message.writer().writeInt(passNew);
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

	public void openLockAccProtect(int pass2)
		{
			Message message = null;
			try
			{
				message = messageNotMap(39);
				message.writer().writeInt(pass2);
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

	public void sendTop(string topName, sbyte selected)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-96));
				message.writer().writeUTF(topName);
				message.writer().writeByte(selected);
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

	public void enemy(sbyte b, int charID)
		{
			Message message = null;
			Res.outz("add enemy");
			try
			{
				message = new Message((sbyte)(-99));
				message.writer().writeByte(b);
				if (b == 1 || b == 2)
				{
					message.writer().writeInt(charID);
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

	public void getFlag(sbyte action, sbyte flagType)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-103));
				message.writer().writeByte(action);
				Res.outz("------------service--  " + action + "   " + flagType);
				if (action != 0)
				{
					message.writer().writeByte(flagType);
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

	public void messagePlayerMenu(int charId)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-30));
				message.writer().writeByte((sbyte)63);
				message.writer().writeInt(charId);
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

	public void playerMenuAction(int charId, short select)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-30));
				message.writer().writeByte((sbyte)64);
				message.writer().writeInt(charId);
				message.writer().writeShort(select);
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

	public void sendDelAcc()
		{
			Message message = new Message((sbyte)69);
			try
			{
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
