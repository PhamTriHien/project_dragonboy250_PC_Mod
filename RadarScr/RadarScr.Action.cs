using System;
using UnityEngine;

public partial class RadarScr
{
	public override void update()
		{
			try
			{
				if (hText < 80)
				{
					hText += 4;
					if (hText > 80)
					{
						hText = 80;
					}
				}
				focus_card = Info_RadaScr.GetInfo(listUse, index[indexFocus]);
				if (TYPE_UI)
				{
					focus_card = Info_RadaScr.GetInfo(list, index[indexFocus]);
				}
				GameScr.gI().update();
				if (GameCanvas.gameTick % 10 < 6)
				{
					if (GameCanvas.gameTick % 2 == 0)
					{
						dyArrow--;
					}
				}
				else
				{
					dyArrow = 0;
				}
				if (focus_card != null)
				{
					int num = focus_card.amount * 100 / focus_card.max_amount;
					hClip = num * imgBar_1.getHeight() / 100;
					int num2 = RadarScr.num * 100 / list.size();
					wClip = num2 * imgPro_1.getWidth() / 100;
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("-upd-radaScr-null: " + ex.ToString());
			}
		}

	public override void updateKey()
		{
			if (!InfoDlg.isLock)
			{
				if (GameCanvas.isTouch && !ChatTextField.gI().isShow && !GameCanvas.menu.showMenu)
				{
					updateKeyTouchControl();
				}
				if (GameCanvas.keyPressed[(!Main.isPC) ? 8 : 22])
				{
					GameCanvas.keyPressed[(!Main.isPC) ? 8 : 22] = false;
					doKeyText(1);
				}
				if (GameCanvas.keyPressed[(!Main.isPC) ? 2 : 21])
				{
					GameCanvas.keyPressed[(!Main.isPC) ? 2 : 21] = false;
					doKeyText(-1);
				}
				if (GameCanvas.keyPressed[(!Main.isPC) ? 4 : 23])
				{
					GameCanvas.keyPressed[(!Main.isPC) ? 4 : 23] = false;
					doKeyItem(1);
				}
				if (GameCanvas.keyPressed[(!Main.isPC) ? 6 : 24])
				{
					GameCanvas.keyPressed[(!Main.isPC) ? 6 : 24] = false;
					doKeyItem(0);
				}
				if (GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25])
				{
					GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25] = false;
					doClickUse(1);
				}
				if (GameCanvas.keyPressed[13])
				{
					doClickUse(2);
				}
				if (GameCanvas.keyPressed[12])
				{
					GameCanvas.keyPressed[12] = false;
					doClickUse(0);
				}
				GameCanvas.clearKeyPressed();
			}
		}

	private void doChangeUI()
		{
			TYPE_UI = !TYPE_UI;
			page = 1;
			indexFocus = 0;
			if (TYPE_UI)
			{
				maxpage = list.size() / 5 + ((list.size() % 5 > 0) ? 1 : 0);
			}
			else
			{
				maxpage = listUse.size() / 5 + ((listUse.size() % 5 > 0) ? 1 : 0);
			}
			listIndex();
			hText = 0;
		}

	private void updateKeyTouchControl()
		{
			if (GameCanvas.isPointerClick)
			{
				for (int i = 0; i < 5; i++)
				{
					if (GameCanvas.isPointerHoldIn(xyItem[i][0], xyItem[i][1], 30, 30) && GameCanvas.isPointerClick && GameCanvas.isPointerJustRelease && i != indexFocus)
					{
						doClickItem(i);
					}
				}
				if (GameCanvas.isPointerHoldIn(xyArrow[0][0] - 5, xyArrow[0][1] - 5, 20, 20))
				{
					if (GameCanvas.isPointerDown)
					{
						dxArrow[0] = 1;
					}
					if (GameCanvas.isPointerClick && GameCanvas.isPointerJustRelease)
					{
						doClickArrow(0);
						dxArrow[0] = 0;
					}
				}
				if (GameCanvas.isPointerHoldIn(xyArrow[2][0] - 5, xyArrow[2][1] - 5, 20, 20))
				{
					if (GameCanvas.isPointerDown)
					{
						dxArrow[1] = 1;
					}
					if (GameCanvas.isPointerClick && GameCanvas.isPointerJustRelease)
					{
						doClickArrow(1);
						dxArrow[1] = 0;
					}
				}
				for (int j = 0; j < xCmd.Length; j++)
				{
					if (GameCanvas.isPointerHoldIn(xCmd[j] - 5, yCmd - 5, 20, 20))
					{
						if (GameCanvas.isPointerDown)
						{
							dxCmd[j] = 1;
						}
						if (GameCanvas.isPointerClick && GameCanvas.isPointerJustRelease)
						{
							doClickUse(j);
							dxCmd[j] = 0;
						}
					}
				}
			}
			else
			{
				dxCmd[0] = 0;
				dxCmd[1] = 0;
				dxCmd[2] = 0;
				dxArrow[0] = 0;
				dxArrow[1] = 0;
			}
			if (!GameCanvas.isPointerHoldIn(xText, 0, wText, yText + hText))
			{
				return;
			}
			if (GameCanvas.isPointerMove)
			{
				if (pyy == 0)
				{
					pyy = GameCanvas.py;
				}
				pxx = pyy - GameCanvas.py;
				if (pxx != 0)
				{
					cmyText += pxx;
					pyy = GameCanvas.py;
				}
				if (cmyText < 0)
				{
					cmyText = 0;
				}
				if (cmyText > focus_card.cp.lim)
				{
					cmyText = focus_card.cp.lim;
				}
			}
			else
			{
				pyy = 0;
				pyy = 0;
			}
		}

	private void doClickUse(int i)
		{
			switch (i)
			{
			case 0:
				doChangeUI();
				break;
			case 1:
				if (focus_card != null)
				{
					Service.gI().SendRada(1, focus_card.id);
				}
				break;
			case 2:
				GameScr.gI().switchToMe();
				break;
			}
			SoundMn.gI().radarClick();
		}

	private void doClickArrow(int dir)
		{
			if (TYPE_UI)
			{
				maxpage = list.size() / 5 + ((list.size() % 5 > 0) ? 1 : 0);
			}
			else
			{
				maxpage = listUse.size() / 5 + ((listUse.size() % 5 > 0) ? 1 : 0);
			}
			int num = page;
			if (dir == 0)
			{
				if (page == 1)
				{
					return;
				}
				num--;
				if (num < 1)
				{
					num = 1;
				}
			}
			else
			{
				if (page == maxpage)
				{
					return;
				}
				num++;
				if (num > maxpage)
				{
					num = maxpage;
				}
			}
			if (num != page)
			{
				page = num;
				listIndex();
			}
		}

	private void doClickItem(int focus)
		{
			indexFocus = focus;
			listIndex();
		}

	private void doKeyText(int type)
		{
			cmyText += 12 * type;
			if (cmyText < 0)
			{
				cmyText = 0;
			}
			if (cmyText > focus_card.cp.lim)
			{
				cmyText = focus_card.cp.lim;
			}
		}

	private void doKeyItem(int type)
		{
			int num = indexFocus;
			int num2 = page;
			num = ((type != 0) ? (num - 1) : (num + 1));
			if (num >= index.Length)
			{
				if (page < maxpage)
				{
					num = 0;
					num2++;
				}
				else
				{
					num = index.Length - 1;
				}
			}
			if (num < 0)
			{
				if (page > 1)
				{
					num = index.Length - 1;
					num2--;
				}
				else
				{
					num = 0;
				}
			}
			if (num != indexFocus)
			{
				indexFocus = num;
				cmyText = 0;
				hText = 0;
			}
			if (num2 != page)
			{
				page = num2;
				listIndex();
			}
		}

	public void perform(int idAction, object p)
		{
		}
}
