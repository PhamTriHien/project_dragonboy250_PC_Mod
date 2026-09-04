
public partial class Menu
{
	public void paintMenu(mGraphics g)
		{
			if (GameScr.gI().activeRongThan && GameScr.gI().isUseFreez)
			{
				return;
			}
			g.translate(-g.getTranslateX(), -g.getTranslateY());
			g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
			g.translate(-cmx, 0);
			for (int i = 0; i < menuItems.size(); i++)
			{
				if (i == menuSelectedItem)
				{
					g.drawImage(imgMenu2, menuX + i * menuW + 1, menuTemY[i], 0);
				}
				else
				{
					g.drawImage(imgMenu1, menuX + i * menuW + 1, menuTemY[i], 0);
				}
				Command command = (Command)menuItems.elementAt(i);
				string[] array = command.subCaption;
				if (array == null)
				{
					array = new string[1] { ((Command)menuItems.elementAt(i)).caption };
				}
				int num = menuTemY[i] + (menuH - array.Length * 14) / 2 + 1;
				for (int j = 0; j < array.Length; j++)
				{
					if (i == menuSelectedItem)
					{
						mFont.tahoma_7b_green2.drawString(g, array[j], menuX + i * menuW + menuW / 2, num + j * 14, 2);
					}
					else if (command.isDisplay)
					{
						mFont.tahoma_7b_red.drawString(g, array[j], menuX + i * menuW + menuW / 2, num + j * 14, 2);
					}
					else
					{
						mFont.tahoma_7b_dark.drawString(g, array[j], menuX + i * menuW + menuW / 2, num + j * 14, 2);
					}
				}
			}
			g.translate(-g.getTranslateX(), -g.getTranslateY());
		}

}
