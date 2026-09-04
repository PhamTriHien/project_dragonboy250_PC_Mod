using System;
using Assets.src.g;
using UnityEngine;

public partial class Service
{
	public void combine(sbyte action, MyVector id)
		{
			Res.outz("combine");
			Message message = null;
			try
			{
				message = new Message((sbyte)(-81));
				message.writer().writeByte(action);
				if (action == 1)
				{
					message.writer().writeByte(id.size());
					for (int i = 0; i < id.size(); i++)
					{
						message.writer().writeByte(((Item)id.elementAt(i)).indexUI);
						Res.outz("gui id " + ((Item)id.elementAt(i)).indexUI);
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

	public void clanImage(sbyte id)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-62));
				message.writer().writeByte(id);
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

	public void useItem(sbyte type, sbyte where, sbyte index, short template)
		{
			Cout.println("USE ITEM! " + type);
			if (Char.myCharz().statusMe == 14)
			{
				return;
			}
			Message message = null;
			try
			{
				message = new Message((sbyte)(-43));
				message.writer().writeByte(type);
				message.writer().writeByte(where);
				message.writer().writeByte(index);
				if (index == -1)
				{
					message.writer().writeShort(template);
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

	public void getItem(sbyte type, sbyte id)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-40));
				message.writer().writeByte(type);
				message.writer().writeByte(id);
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

	public void requestItemInfo(int typeUI, int indexUI)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)35);
				message.writer().writeByte(typeUI);
				message.writer().writeByte(indexUI);
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

	public void requestItemPlayer(int charId, int indexUI)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)90);
				message.writer().writeInt(charId);
				message.writer().writeByte(indexUI);
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

	public void saleItem(sbyte action, sbyte type, short id)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)7);
				message.writer().writeByte(action);
				message.writer().writeByte(type);
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

	public void buyItem(sbyte type, int id, int quantity)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)6);
				message.writer().writeByte(type);
				message.writer().writeShort(id);
				if (quantity > 1)
				{
					message.writer().writeShort(quantity);
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

	public void requestItem(int typeUI)
		{
			Message message = null;
			try
			{
				message = messageSubCommand(22);
				message.writer().writeByte(typeUI);
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

	public void upgradeItem(Item item, Item[] items, bool isGold)
		{
			GameCanvas.msgdlg.pleasewait();
			Message message = null;
			try
			{
				message = new Message((sbyte)14);
				message.writer().writeBoolean(isGold);
				message.writer().writeByte(item.indexUI);
				for (int i = 0; i < items.Length; i++)
				{
					if (items[i] != null)
					{
						message.writer().writeByte(items[i].indexUI);
					}
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

	public void crystalCollectLock(Item[] items)
		{
			GameCanvas.msgdlg.pleasewait();
			Message message = null;
			try
			{
				message = new Message((sbyte)13);
				for (int i = 0; i < items.Length; i++)
				{
					if (items[i] != null)
					{
						message.writer().writeByte(items[i].indexUI);
					}
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

	public void tradeItemLock(int coin, Item[] items)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)38);
				message.writer().writeInt(coin);
				int num = 0;
				for (int i = 0; i < items.Length; i++)
				{
					if (items[i] != null)
					{
						num++;
					}
				}
				message.writer().writeByte(num);
				for (int j = 0; j < items.Length; j++)
				{
					if (items[j] != null)
					{
						message.writer().writeByte(items[j].indexUI);
					}
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

	public void pickItem(int itemMapId)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-20));
				message.writer().writeShort(itemMapId);
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

	public void throwItem(int index)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-18));
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

	public void chat(string text)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)44);
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

	public void updateItem()
		{
			Message message = null;
			try
			{
				message = messageNotMap(8);
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

	public void chatPlayer(string text, int id)
		{
			Res.outz("chat player text = " + text);
			Message message = null;
			try
			{
				message = new Message((sbyte)(-72));
				message.writer().writeInt(id);
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

	public void chatGlobal(string text)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-71));
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

	public void chatPrivate(string to, string text)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)91);
				message.writer().writeUTF(to);
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

	public void requestIcon(int id)
		{
			GameCanvas.connect();
			Message message = null;
			try
			{
				message = new Message((sbyte)(-67));
				message.writer().writeInt(id);
				if (Session_ME2.gI().isConnected() && !Session_ME2.connecting)
				{
					session = Session_ME2.gI();
				}
				else
				{
					session = Session_ME.gI();
				}
				session.sendMessage(message);
				Res.outz(">>>>>>>>>>>>>REQUEST ICON " + id + "  isConnected:" + Controller.isGet_CLIENT_INFO);
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

	public void doConvertUpgrade(int index1, int index2, int index3)
		{
			Message message = null;
			try
			{
				message = messageNotMap(33);
				message.writer().writeByte(index1);
				message.writer().writeByte(index2);
				message.writer().writeByte(index3);
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

	public void inputNumSplit(int indexItem, int numSplit)
		{
			Message message = null;
			try
			{
				message = messageNotMap(40);
				message.writer().writeByte(indexItem);
				message.writer().writeInt(numSplit);
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

	public void getBgTemplate(short id)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-32));
				message.writer().writeShort(id);
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

	public void getChest(sbyte action)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-35));
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

	public void requestBagImage(int ID)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-63));
				message.writer().writeShort(ID);
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

	public void getBag(sbyte action)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-36));
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

	public void getBody(sbyte action)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-37));
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

	public void kigui(sbyte action, int itemId, sbyte moneyType, int money, int quaintly)
		{
			Message message = null;
			try
			{
				Res.outz("ki gui action= " + action);
				message = new Message((sbyte)(-100));
				message.writer().writeByte(action);
				if (action == 0)
				{
					message.writer().writeShort(itemId);
					message.writer().writeByte(moneyType);
					message.writer().writeInt(money);
					message.writer().writeInt(quaintly);
				}
				if (action == 1 || action == 2)
				{
					message.writer().writeShort(itemId);
				}
				if (action == 3)
				{
					message.writer().writeShort(itemId);
					message.writer().writeByte(moneyType);
					message.writer().writeInt(money);
				}
				if (action == 4)
				{
					message.writer().writeByte(moneyType);
					message.writer().writeByte(money);
					Res.outz("currTab= " + moneyType + " page= " + money);
				}
				if (action == 5)
				{
					message.writer().writeShort(itemId);
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

	public void setLockInventory(int pass)
		{
			Message message = null;
			try
			{
				Res.outz("------------setLockInventory:     " + pass);
				message = new Message((sbyte)(-104));
				message.writer().writeInt(pass);
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

	public void getQuayso()
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-126));
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

	public void getImgByName(string nameImg)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)66);
				message.writer().writeUTF(nameImg);
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

	public void SendCrackBall(byte type, byte soluong)
		{
			Message message = new Message((sbyte)(-127));
			try
			{
				message.writer().writeByte(type);
				if (soluong > 0)
				{
					message.writer().writeByte(soluong);
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

	public void SendRada(int i, int id)
		{
			Message message = new Message(sbyte.MaxValue);
			try
			{
				message.writer().writeByte(i);
				if (id != -1)
				{
					message.writer().writeShort(id);
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

	public void sendOptHat(sbyte action)
		{
			Message message = new Message((sbyte)24);
			try
			{
				if (action == 1)
				{
					sbyte[] array = Res.TakeSnapShot();
					message.writer().writeByte(1);
					message.writer().writeShort(array.Length);
					message.writer().write(array);
				}
				else
				{
					message.writer().writeByte((Char.myCharz().idHat != -1) ? (-1) : 0);
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
