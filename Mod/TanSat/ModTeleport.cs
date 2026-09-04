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

			// Xóa các điểm di chuyển trung gian
			me.vMovePoints.removeAllElements();
			me.currentMovePoint = null;
			me.cvx = 0;
			me.cvy = 0;

			// Gửi gói tin điểm đến nguyên tử duy nhất lên server
			Service.gI().charMoveTo(targetX, targetY);

			// Tạo hiệu ứng dịch chuyển visual
			ServerEffect.addServerEffect(1, targetX, targetY, 1);
		}
		catch
		{
		}
	}
}
