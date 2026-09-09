using System;

public static class ModTeleport
{
	public static void TeleportTo(int targetX, int targetY)
	{
		try
		{
			Char me = Char.myCharz();
			if (me == null)
			{
				return;
			}

			// Neu nhan vat da o rat gan diem den (<= 5px), khong can dich chuyen lai tranh spam packet
			if (Res.abs(me.cx - targetX) <= 5 && Res.abs(me.cy - targetY) <= 5)
			{
				return;
			}

			// Xoa cac diem di chuyen trung gian
			me.vMovePoints.removeAllElements();
			me.currentMovePoint = null;
			me.cvx = 0;
			me.cvy = 0;

			// Dong bo toa do client va moc send de tranh bi keo giat
			me.cx = targetX;
			me.cy = targetY;
			me.cxSend = targetX;
			me.cySend = targetY;

			// Gui goi tin diem den nguyen tu duy nhat len server
			Service.gI().charMoveTo(targetX, targetY);

			// Tao hieu ung dich chuyen visual
			ServerEffect.addServerEffect(1, targetX, targetY, 1);
		}
		catch
		{
		}
	}
}
