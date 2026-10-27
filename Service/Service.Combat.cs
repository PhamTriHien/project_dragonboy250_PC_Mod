using System;
using Assets.src.g;
using UnityEngine;

/// <summary>
/// Thành phần phân mảnh (partial) của lớp Service: Quản lý kỹ năng chiến đấu, tính toán sát thương, va chạm và kích hoạt chiêu thức ().
/// </summary>
public partial class Service
{
	private static long lastTimeSendPlayerAttack = 0L;

	/// <summary>
	/// Thực thi phương thức nghiệp vụ speacialSkill.
	/// </summary>
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

	/// <summary>
	/// Thực thi phương thức nghiệp vụ skill_not_focus.
	/// </summary>
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

	/// <summary>
	/// Thực thi phương thức nghiệp vụ magicTree.
	/// </summary>
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

	/// <summary>
	/// Thực thi phương thức nghiệp vụ requestSkill.
	/// </summary>
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

	/// <summary>
	/// Thực thi phương thức nghiệp vụ upSkill.
	/// </summary>
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

	/// <summary>
	/// Thực thi phương thức nghiệp vụ selectSkill.
	/// </summary>
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

	/// <summary>
	/// Đóng gói và gửi thông điệp mạng sendPlayerAttack lên máy chủ.
	/// </summary>
	public void sendPlayerAttack(MyVector vMob, MyVector vChar, int type)
		{
			try
			{
				long now = mSystem.currentTimeMillis();
				long elapsed = (lastTimeSendPlayerAttack != 0L) ? (now - lastTimeSendPlayerAttack) : 0L;
				if (lastTimeSendPlayerAttack != 0L && elapsed < 500L)
				{
					return;
				}
				lastTimeSendPlayerAttack = now;
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

	/// <summary>
	/// Cập nhật trạng thái logic của updateSkill theo chu kỳ khung hình.
	/// </summary>
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

	/// <summary>
	/// Truy xuất và trả về dữ liệu của getMagicTree.
	/// </summary>
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

	/// <summary>
	/// Thực thi phương thức nghiệp vụ upPotential.
	/// </summary>
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

	/// <summary>
	/// Thực thi phương thức nghiệp vụ petInfo.
	/// </summary>
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

	/// <summary>
	/// Thực thi phương thức nghiệp vụ petStatus.
	/// </summary>
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

	/// <summary>
	/// Thực thi phương thức nghiệp vụ funsion.
	/// </summary>
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

	/// <summary>
	/// Thực thi phương thức nghiệp vụ changeOnKeyScr.
	/// </summary>
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

	/// <summary>
	/// Thực thi phương thức nghiệp vụ requestPean.
	/// </summary>
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

	/// <summary>
	/// Đóng gói và gửi thông điệp mạng sendThachDau lên máy chủ.
	/// </summary>
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

	/// <summary>
	/// Thực thi phương thức nghiệp vụ new_skill_not_focus.
	/// </summary>
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

	/// <summary>
	/// Đóng gói và gửi thông điệp mạng sendCT_ready lên máy chủ.
	/// </summary>
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
