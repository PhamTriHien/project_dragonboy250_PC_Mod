using System;
using Assets.src.g;
using UnityEngine;

/// <summary>
/// Thành phần phân mảnh (partial) của lớp Service: Đảm nhiệm phân hệ chuyên trách Movement ().
/// </summary>
public partial class Service
{
	/// <summary>
	/// Thực thi phương thức nghiệp vụ gotoPlayer.
	/// </summary>
	public void gotoPlayer(int id)
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)18);
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
	/// Thực thi phương thức nghiệp vụ requestChangeMap.
	/// </summary>
	public void requestChangeMap()
		{
			Message message = new Message((sbyte)(-23));
			session.sendMessage(message);
			message.cleanup();
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ requestChangeZone.
	/// </summary>
	public void requestChangeZone(int zoneId, int indexUI)
		{
			Message message = new Message((sbyte)21);
			try
			{
				message.writer().writeByte(zoneId);
				session.sendMessage(message);
				message.cleanup();
			}
			catch (Exception)
			{
			}
		}

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của checkMMove.
	/// </summary>
	public void checkMMove(int second)
		{
			Message message = new Message((sbyte)(-78));
			try
			{
				message.writer().writeInt(second);
				session.sendMessage(message);
				message.cleanup();
			}
			catch (Exception)
			{
			}
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ charMove.
	/// </summary>
	public void charMove()
		{
			int num = Char.myCharz().cx - Char.myCharz().cxSend;
			int num2 = Char.myCharz().cy - Char.myCharz().cySend;
			if (Char.ischangingMap || (num == 0 && num2 == 0) || Controller.isStopReadMessage || Char.myCharz().isTeleport || Char.myCharz().cy <= 0 || Char.myCharz().telePortSkill)
			{
				return;
			}
			lastCharMoveTime = mSystem.currentTimeMillis();
			if (GameScr.canAutoPlay || ModTanSat.autoTanSat || ModNextMap.isNextMapActive || ModGoBack.isReturning)
			{
				ModTdltLogger.LogCharMove("charMove", Char.myCharz().cx, Char.myCharz().cy, Char.myCharz().cxSend, Char.myCharz().cySend);
			}
			try
			{
				Message message = new Message((sbyte)(-7));
				Char.myCharz().cxSend = Char.myCharz().cx;
				Char.myCharz().cySend = Char.myCharz().cy;
				Char.myCharz().cdirSend = Char.myCharz().cdir;
				Char.myCharz().cactFirst = Char.myCharz().statusMe;
				if (TileMap.tileTypeAt(Char.myCharz().cx / TileMap.size, Char.myCharz().cy / TileMap.size) == 0)
				{
					message.writer().writeByte((sbyte)1);
				}
				else
				{
					message.writer().writeByte((sbyte)0);
				}
				message.writer().writeShort(Char.myCharz().cx);
				if (num2 != 0)
				{
					message.writer().writeShort(Char.myCharz().cy);
				}
				session.sendMessage(message);
				GameScr.tickMove++;
				message.cleanup();
			}
			catch (Exception ex)
			{
				Cout.LogError("LOI CHAR MOVE " + ex.ToString());
			}
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ charMoveTo.
	/// </summary>
	public void charMoveTo(int x, int y)
		{
			Char me = Char.myCharz();
			if (me == null || Controller.isStopReadMessage)
			{
				return;
			}
			me.cx = x;
			me.cy = y;
			me.cvx = 0;
			me.cvy = 0;
			try
			{
				Message message = new Message((sbyte)(-7));
				me.cxSend = me.cx;
				me.cySend = me.cy;
				me.cdirSend = me.cdir;
				me.cactFirst = me.statusMe;
				if (TileMap.tileTypeAt(me.cx / TileMap.size, me.cy / TileMap.size) == 0)
				{
					message.writer().writeByte((sbyte)1);
				}
				else
				{
					message.writer().writeByte((sbyte)0);
				}
				message.writer().writeShort(me.cx);
				message.writer().writeShort(me.cy);
				session.sendMessage(message);
				if (GameScr.canAutoPlay || ModTanSat.autoTanSat || ModNextMap.isNextMapActive || ModGoBack.isReturning)
				{
					ModTdltLogger.LogCharMove($"charMoveTo({x},{y})", me.cx, me.cy, me.cxSend, me.cySend);
				}
				GameScr.tickMove++;
				message.cleanup();
				lastCharMoveTime = mSystem.currentTimeMillis();
			}
			catch (Exception ex)
			{
				Cout.LogError("LOI CHAR MOVE TO " + ex.ToString());
			}
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ selectZone.
	/// </summary>
	public void selectZone(sbyte sub, int value)
		{
		}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ openUIZone.
	/// </summary>
	public void openUIZone()
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)29);
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
	/// Thực thi phương thức nghiệp vụ requestMaptemplate.
	/// </summary>
	public void requestMaptemplate(int maptemplateId)
		{
			Message message = null;
			try
			{
				message = messageNotMap(10);
				message.writer().writeByte(maptemplateId);
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
	/// Truy xuất và trả về dữ liệu của getMapOffline.
	/// </summary>
	public void getMapOffline()
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-33));
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
	/// Thực thi phương thức nghiệp vụ finishLoadMap.
	/// </summary>
	public void finishLoadMap()
		{
			Message message = null;
			try
			{
				message = new Message((sbyte)(-39));
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
	/// Thực thi phương thức nghiệp vụ transportNow.
	/// </summary>
	public void transportNow()
		{
			Message message = null;
			try
			{
				Res.outz("------------transportNow  ");
				message = new Message((sbyte)(-105));
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
