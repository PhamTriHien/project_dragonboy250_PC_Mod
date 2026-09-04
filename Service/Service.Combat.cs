using System;
using Assets.src.g;
using UnityEngine;

public partial class Service
{
	public void speacialSkill(sbyte index)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)112);
				message.writer().writeByte(index);
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

	public void skill_not_focus(sbyte status)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-45));
				message.writer().writeByte(status);
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

	public void magicTree(sbyte type)
		{
			Message message = new Message((sbyte)(-34));
			try
			{
				message.writer().writeByte(type);
				session.sendMessage(message);
				message.cleanup();
			}
			catch (Exception)
			{
			}
		}

	public void requestSkill(int skillId)
		{
			Message message = null;
			try
			{
				message = messageNotMap(9);
				message.writer().writeShort(skillId);
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

	public void upSkill(int skillTemplateId, int point)
		{
			Message message = null;
			try
			{
				message = messageSubCommand(17);
				message.writer().writeShort(skillTemplateId);
				message.writer().writeByte(point);
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

	public void selectSkill(int skillTemplateId)
		{
			Cout.println(Char.myCharz().cName + " SELECT SKILL " + skillTemplateId);
			Message message = null;
			try
			{
				message = new Message((sbyte)34);
				message.writer().writeShort(skillTemplateId);
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

	public void sendPlayerAttack(MyVector vMob, MyVector vChar, int type)
		{
			try
			{
				Res.outz(">>SEND ATTACT  vMob=" + vMob.size() + "  vChar=" + vChar.size());
				Message message = null;
				if (type == 0)
				{
					return;
				}
				if (vMob.size() > 0 && vChar.size() > 0)
				{
					switch (type)
					{
					case 1:
						message = new Message((sbyte)(-4));
						break;
					case 2:
						message = new Message((sbyte)67);
						break;
					}
					message.writer().writeByte(vMob.size());
					for (int i = 0; i < vMob.size(); i++)
					{
						Mob mob = (Mob)vMob.elementAt(i);
						message.writer().writeByte(mob.mobId);
					}
					for (int j = 0; j < vChar.size(); j++)
					{
						Char @char = (Char)vChar.elementAt(j);
						if (@char != null)
						{
							message.writer().writeInt(@char.charID);
						}
						else
						{
							message.writer().writeInt(-1);
						}
					}
				}
				else if (vMob.size() > 0)
				{
					message = new Message((sbyte)54);
					for (int k = 0; k < vMob.size(); k++)
					{
						Mob mob2 = (Mob)vMob.elementAt(k);
						if (!mob2.isMobMe)
						{
							message.writer().writeByte(mob2.mobId);
							continue;
						}
						message.writer().writeByte((sbyte)(-1));
						message.writer().writeInt(mob2.mobId);
					}
				}
				else if (vChar.size() > 0)
				{
					message = new Message((sbyte)(-60));
					for (int l = 0; l < vChar.size(); l++)
					{
						Char char2 = (Char)vChar.elementAt(l);
						message.writer().writeInt(char2.charID);
					}
				}
				message.writer().writeSByte((sbyte)Char.myCharz().cdir);
				if (message != null)
				{
					session.sendMessage(message);
				}
			}
			catch (Exception)
			{
				Res.err(">>err ATTACT  vMob=" + vMob.size() + "  vChar=" + vChar.size());
			}
		}

	public void updateSkill()
		{
			Message message = null;
			try
			{
				message = messageNotMap(7);
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

	public void getMagicTree(sbyte action)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-34));
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

	public void upPotential(int typePotential, int num)
		{
			Message message = null;
			try
			{
				message = messageSubCommand(16);
				message.writer().writeByte(typePotential);
				message.writer().writeShort(num);
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

	public void petInfo()
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-107));
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

	public void petStatus(sbyte status)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-108));
				message.writer().writeByte(status);
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

	public void funsion(sbyte type)
		{
			Message message = null;
			try
			{
				Res.outz("FUNSION");
				message = new Message((sbyte)125);
				message.writer().writeByte(type);
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

	public void changeOnKeyScr(sbyte[] skill)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-113));
				for (int i = 0; i < GameScr.onScreenSkill.Length; i++)
				{
					message.writer().writeByte(skill[i]);
				}
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

	public void requestPean()
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-114));
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

	public void sendThachDau(int id)
		{
			Res.outz("GUI THACH DAU");
			Message message = null;
			try
			{
				message = new Message((sbyte)(-118));
				message.writer().writeInt(id);
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

	public void new_skill_not_focus(sbyte idTemplateSkill, sbyte dir, short x, short y)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-45));
				message.writer().writeSByte(20);
				message.writer().writeSByte(idTemplateSkill);
				message.writer().writeShort(Char.myCharz().cx);
				message.writer().writeShort(Char.myCharz().cy);
				message.writer().writeSByte(dir);
				message.writer().writeShort(x);
				message.writer().writeShort(y);
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

	public void sendCT_ready(sbyte sub, sbyte sub_sub)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)24);
				message.writer().writeByte(sub);
				message.writer().writeByte(sub_sub);
				Res.err(" =====> SEND OPTION_HAT " + sub + "_" + sub_sub);
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
