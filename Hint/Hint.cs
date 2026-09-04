using System;

public partial class Hint
{
	public static int x;

	public static int y;

	public static int type;

	public static int t;

	public static int xF;

	public static int yF;

	public static bool isShow;

	public static bool activeClick;

	public static bool isViewMap;

	public static bool isCloseMap;

	public static bool isViewPotential;

	public static bool isPaint;

	public static bool isCamera;

	public static int trans;

	public static bool paintFlare;

	public static bool isPaintArrow;

	private int s = 2;

	public static bool isOnTask(int tastId, int index)
		{
			if (Char.myCharz().taskMaint != null && Char.myCharz().taskMaint.taskId == tastId && Char.myCharz().taskMaint.index == index)
			{
				return true;
			}
			return false;
		}

	public static bool isHaveItem()
		{
			if (GameCanvas.panel.isShow)
			{
				isPaint = false;
			}
			for (int i = 0; i < GameScr.vItemMap.size(); i++)
			{
				ItemMap itemMap = (ItemMap)GameScr.vItemMap.elementAt(i);
				if (itemMap.playerId == Char.myCharz().charID && itemMap.template.id == 73)
				{
					type = 1;
					x = itemMap.x;
					y = itemMap.y + 5;
					isCamera = true;
					return true;
				}
			}
			return false;
		}

}
