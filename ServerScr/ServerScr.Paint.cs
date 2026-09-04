using System;

public partial class ServerScr
{
	public override void paint(mGraphics g)
		{
			GameCanvas.paintBGGameScr(g);
			if (isChooseArea)
			{
				paintChooseArea(g);
			}
			else if (isPaintNewUi)
			{
				paintNewSelectMenu(g);
				if (ServerListScreen.cmdDeleteRMS != null)
				{
					mFont.tahoma_7_white.drawString(g, mResources.xoadulieu, GameCanvas.w - 2, GameCanvas.h - 15, 1, mFont.tahoma_7_grey);
				}
			}
			else
			{
				for (int i = 0; i < vecServer.size(); i++)
				{
					if (vecServer.elementAt(i) != null)
					{
						((Command)vecServer.elementAt(i)).paint(g);
					}
				}
			}
			base.paint(g);
		}

	private void paintChooseArea(mGraphics g)
		{
			if (isChooseArea)
			{
				paint_Area(g, GameCanvas.hw - wBox / 2, yBox);
				paint_Lang(g, GameCanvas.hw + 20, yBox);
				cmdChooseArea.paint(g);
			}
		}

	private void paintNewSelectMenu(mGraphics g)
		{
			if (!isPaintNewUi)
			{
				return;
			}
			g.setColor(14601141);
			g.fillRect(x, y, w, h);
			PopUp.paintPopUp(g, xName - 50, yName, 100, 20, 0, isButton: true);
			mFont.tahoma_7b_dark.drawString(g, mResources.selectServer2, xName, yName + 5, 2);
			for (int i = 0; i < ntypeSv; i++)
			{
				int num = yPop + i * (hPop + 5);
				PopUp.paintPopUp(g, xPop, num, wPop, hPop, (select_typeSv == i) ? 1 : 0, isButton: true);
				mFont.tahoma_7b_dark.drawString(g, strTypeSV[i], xPop + wPop / 2, num + 5, 2);
			}
			g.setColor(10254674);
			g.fillRect(xinfo, yinfo, winfo, hinfo);
			string[] array = mFont.tahoma_7.splitFontArray(strTypeSV_info[select_typeSv], winfo - 10);
			for (int j = 0; j < array.Length; j++)
			{
				mFont.tahoma_7_white.drawString(g, array[j], xinfo + 5, yinfo + 5 + j * 11, 0);
			}
			paintShowAllCheck(g);
			paint_Area(g, 10, yBox);
			paint_Lang(g, GameCanvas.w - wBox - 10, yBox);
			g.setColor(10254674);
			g.fillRect(xsub, ysub, wsub, hsub);
			g.setClip(xsub, ysub, wsub, hsub);
			g.translate(0, -list.cmx);
			for (int k = 0; k < vecServer.size(); k++)
			{
				Command command = (Command)vecServer.elementAt(k);
				if (command != null)
				{
					command.paint(g);
					if (command.isPaintNew && GameCanvas.gameTick % 10 > 1)
					{
						g.drawImage(Panel.imgNew, command.x + 60, command.y, 0);
					}
				}
			}
			GameCanvas.resetTrans(g);
		}

	private void paint_Area(mGraphics g, int x, int y)
		{
			x -= 5;
			xPopUp_Area = x;
			PopUp.paintPopUp(g, x, y, wBox, hBox, 0, isButton: true);
			mFont.tahoma_7b_dark.drawString(g, strArea[select_Area], x + (wBox - 10) / 2, y + 5, 2);
			g.drawRegion(Mob.imgHP, 0, 30, 9, 6, 0, x + wBox - 10, y + 14, mGraphics.BOTTOM | mGraphics.HCENTER);
			if (!isPaint_select_area)
			{
				return;
			}
			yPopUp_Area = y + hBox + 5;
			g.setColor(10254674);
			g.fillRect(x, yPopUp_Area, wBox, strArea.Length * htext + 1);
			for (int i = 0; i < strArea.Length; i++)
			{
				mFont.tahoma_7_white.drawString(g, strArea[i], x + wBox / 2, yPopUp_Area + i * htext + 2, 2);
				if (select_Area == i)
				{
					g.setColor(15591444);
					g.drawRect(x + 2, yPopUp_Area + i * htext + 1, wBox - 4, htext - 2);
				}
			}
		}

	private void paint_Lang(mGraphics g, int x, int y)
		{
		}

	public void paintShowAllCheck(mGraphics g)
		{
			int num = xinfo;
			int num2 = yinfo + hinfo + 2;
			g.setColor(16777215);
			g.fillRect(num, num2, wCheck, wCheck);
			if (isShowSv_HaveChar)
			{
				g.setColor(3329330);
				g.fillRect(num + 1, num2 + 1, wCheck - 2, wCheck - 2);
			}
			mFont.tahoma_7b_dark.drawString(g, strShowAll, num + wCheck + 2, num2, 0);
		}

}
