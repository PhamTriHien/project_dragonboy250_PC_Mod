using System;
using Assets.src.e;
using Assets.src.f;
using Assets.src.g;
using UnityEngine;

public partial class Controller : IMessageHandler
{
	public void requestItemPlayer(Message msg)
		{
			try
			{
				int num = msg.reader().readUnsignedByte();
				Item item = GameScr.currentCharViewInfo.arrItemBody[num];
				item.saleCoinLock = msg.reader().readInt();
				item.sys = msg.reader().readByte();
				item.options = new MyVector();
				try
				{
					while (true)
					{
						ItemOption itemOption = readItemOption(msg);
						if (itemOption != null)
						{
							item.options.addElement(itemOption);
						}
					}
				}
				catch (Exception ex)
				{
					Cout.println("Loi tairequestItemPlayer 1" + ex.ToString());
				}
			}
			catch (Exception ex2)
			{
				Cout.println("Loi tairequestItemPlayer 2" + ex2.ToString());
			}
		}

	public void readClanMsg(Message msg, int index)
		{
			try
			{
				ClanMessage clanMessage = new ClanMessage();
				sbyte b = msg.reader().readByte();
				clanMessage.type = b;
				clanMessage.id = msg.reader().readInt();
				clanMessage.playerId = msg.reader().readInt();
				clanMessage.playerName = msg.reader().readUTF();
				clanMessage.role = msg.reader().readByte();
				clanMessage.time = msg.reader().readInt() + 1000000000;
				bool flag = false;
				GameScr.isNewClanMessage = false;
				if (b == 0)
				{
					string text = msg.reader().readUTF();
					GameScr.isNewClanMessage = true;
					if (mFont.tahoma_7.getWidth(text) > Panel.WIDTH_PANEL - 60)
					{
						clanMessage.chat = mFont.tahoma_7.splitFontArray(text, Panel.WIDTH_PANEL - 10);
					}
					else
					{
						clanMessage.chat = new string[1];
						clanMessage.chat[0] = text;
					}
					clanMessage.color = msg.reader().readByte();
				}
				else if (b == 1)
				{
					clanMessage.recieve = msg.reader().readByte();
					clanMessage.maxCap = msg.reader().readByte();
					flag = msg.reader().readByte() == 1;
					if (flag)
					{
						GameScr.isNewClanMessage = true;
					}
					if (clanMessage.playerId != Char.myCharz().charID)
					{
						if (clanMessage.recieve < clanMessage.maxCap)
						{
							clanMessage.option = new string[1] { mResources.donate };
						}
						else
						{
							clanMessage.option = null;
						}
					}
					if (GameCanvas.panel.cp != null)
					{
						GameCanvas.panel.updateRequest(clanMessage.recieve, clanMessage.maxCap);
					}
				}
				else if (b == 2 && Char.myCharz().role == 0)
				{
					GameScr.isNewClanMessage = true;
					clanMessage.option = new string[2]
					{
						mResources.CANCEL,
						mResources.receive
					};
				}
				if (GameCanvas.currentScreen != GameScr.instance)
				{
					GameScr.isNewClanMessage = false;
				}
				else if (GameCanvas.panel.isShow && GameCanvas.panel.type == 0 && GameCanvas.panel.currentTabIndex == 3)
				{
					GameScr.isNewClanMessage = false;
				}
				ClanMessage.addMessage(clanMessage, index, flag);
			}
			catch (Exception)
			{
				Cout.println("LOI TAI CMD -= " + msg.command);
			}
		}

	public void keyValueAction(string key, string value)
		{
			if (key.Equals("eff"))
			{
				if (Panel.graphics > 0)
				{
					return;
				}
				string[] array = Res.split(value, ".", 0);
				int id = int.Parse(array[0]);
				int layer = int.Parse(array[1]);
				int x = int.Parse(array[2]);
				int y = int.Parse(array[3]);
				int loop;
				int loopCount;
				if (array.Length <= 4)
				{
					loop = -1;
					loopCount = 1;
				}
				else
				{
					loop = int.Parse(array[4]);
					loopCount = int.Parse(array[5]);
				}
				Effect effect = new Effect(id, x, y, layer, loop, loopCount);
				if (array.Length > 6)
				{
					effect.typeEff = int.Parse(array[6]);
					if (array.Length > 7)
					{
						effect.indexFrom = int.Parse(array[7]);
						effect.indexTo = int.Parse(array[8]);
					}
				}
				EffecMn.addEff(effect);
			}
			else if (key.Equals("beff") && Panel.graphics <= 1)
			{
				BackgroudEffect.addEffect(int.Parse(value));
			}
		}

	public void messageNotLogin(Message msg)
		{
			try
			{
				sbyte b = msg.reader().readByte();
				Res.outz("---messageNotLogin : " + b);
				if (b == 2)
				{
					string linkDefault = msg.reader().readUTF();
					Res.outz(">>Get CLIENT_INFO");
					ServerListScreen.linkDefault = linkDefault;
					mSystem.AddIpTest();
					ServerListScreen.getServerList(ServerListScreen.linkDefault);
					try
					{
						sbyte b2 = msg.reader().readByte();
						Panel.CanNapTien = b2 == 1;
					}
					catch (Exception)
					{
					}
					isGet_CLIENT_INFO = true;
				}
			}
			catch (Exception)
			{
			}
			finally
			{
				msg?.cleanup();
			}
		}

	public bool readCharInfo(Char c, Message msg)
		{
			try
			{
				c.clevel = msg.reader().readByte();
				c.isInvisiblez = msg.reader().readBoolean();
				c.cTypePk = msg.reader().readByte();
				Res.outz("ADD TYPE PK= " + c.cTypePk + " to player " + c.charID + " @@ " + c.cName);
				c.nClass = GameScr.nClasss[msg.reader().readByte()];
				c.cgender = msg.reader().readByte();
				c.head = msg.reader().readShort();
				c.cName = msg.reader().readUTF();
				c.cHP = msg.reader().readLong();
				c.dHP = c.cHP;
				if (c.cHP == 0)
				{
					c.statusMe = 14;
				}
				c.cHPFull = msg.reader().readLong();
				if (c.cy >= TileMap.pxh - 100)
				{
					c.isFlyUp = true;
				}
				c.body = msg.reader().readShort();
				c.leg = msg.reader().readShort();
				c.bag = msg.reader().readShort();
				Res.outz(" body= " + c.body + " leg= " + c.leg + " bag=" + c.bag + "BAG ==" + c.bag + "*********************************");
				c.isShadown = true;
				sbyte b = msg.reader().readByte();
				if (c.wp == -1)
				{
					c.setDefaultWeapon();
				}
				if (c.body == -1)
				{
					c.setDefaultBody();
				}
				if (c.leg == -1)
				{
					c.setDefaultLeg();
				}
				c.cx = msg.reader().readShort();
				c.cy = msg.reader().readShort();
				c.xSd = c.cx;
				c.ySd = c.cy;
				c.eff5BuffHp = msg.reader().readShort();
				c.eff5BuffMp = msg.reader().readShort();
				int num = msg.reader().readByte();
				for (int i = 0; i < num; i++)
				{
					EffectChar effectChar = new EffectChar(msg.reader().readByte(), msg.reader().readInt(), msg.reader().readInt(), msg.reader().readShort());
					c.vEff.addElement(effectChar);
					if (effectChar.template.type == 12 || effectChar.template.type == 11)
					{
						c.isInvisiblez = true;
					}
				}
				return true;
			}
			catch (Exception ex)
			{
				ex.StackTrace.ToString();
			}
			return false;
		}

}
