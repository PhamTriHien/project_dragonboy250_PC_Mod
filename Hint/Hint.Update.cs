using System;

public partial class Hint
{
	public static void clickNpc()
		{
			if (GameCanvas.panel.isShow)
			{
				isPaint = false;
			}
			if (GameScr.getNpcTask() != null)
			{
				x = GameScr.getNpcTask().cx;
				y = GameScr.getNpcTask().cy;
				trans = 0;
				isCamera = true;
				type = (GameCanvas.isTouch ? 1 : 0);
			}
		}

	public static void nextMap(int index)
		{
			if (!GameCanvas.panel.isShow && PopUp.vPopups.size() - 1 >= index)
			{
				PopUp popUp = (PopUp)PopUp.vPopups.elementAt(index);
				x = popUp.cx + popUp.sayWidth / 2;
				y = popUp.cy + 30;
				if (popUp.isHide || !popUp.isPaint)
				{
					isPaint = false;
				}
				else
				{
					isPaint = true;
				}
				type = 0;
				isCamera = true;
				trans = 0;
				if (!GameCanvas.isTouch)
				{
					isPaint = false;
				}
			}
		}

	public static void clickMob()
		{
			type = 1;
			if (GameCanvas.panel.isShow)
			{
				isPaint = false;
			}
			bool flag = false;
			for (int i = 0; i < GameScr.vMob.size(); i++)
			{
				Mob mob = (Mob)GameScr.vMob.elementAt(i);
				if (mob.isHintFocus)
				{
					flag = true;
					break;
				}
			}
			for (int j = 0; j < GameScr.vMob.size(); j++)
			{
				Mob mob2 = (Mob)GameScr.vMob.elementAt(j);
				if (mob2.isHintFocus)
				{
					x = mob2.x;
					y = mob2.y + 5;
					isCamera = true;
					if (mob2.status == 0)
					{
						mob2.isHintFocus = false;
					}
					break;
				}
				if (!flag)
				{
					if (mob2.status != 0)
					{
						mob2.isHintFocus = true;
						break;
					}
					mob2.isHintFocus = false;
				}
			}
		}

	public static void hint()
		{
			if (Char.myCharz().taskMaint != null && GameCanvas.currentScreen == GameScr.instance)
			{
				int taskId = Char.myCharz().taskMaint.taskId;
				int index = Char.myCharz().taskMaint.index;
				isCamera = false;
				trans = 0;
				type = 0;
				isPaint = true;
				isPaintArrow = true;
				if (GameCanvas.menu.showMenu && taskId > 0)
				{
					isPaint = false;
				}
				switch (taskId)
				{
				case 0:
					if (ChatPopup.currChatPopup != null || Char.myCharz().statusMe == 14)
					{
						x = GameCanvas.w / 2;
						y = GameCanvas.h - 15;
						return;
					}
					if (index == 0 && TileMap.vGo.size() != 0)
					{
						x = ((Waypoint)TileMap.vGo.elementAt(0)).minX - 100;
						y = ((Waypoint)TileMap.vGo.elementAt(0)).minY + 40;
						isCamera = true;
					}
					if (index == 1)
					{
						nextMap(0);
					}
					if (index == 2)
					{
						clickNpc();
					}
					if (index == 3)
					{
						if (!GameCanvas.panel.isShow)
						{
							clickNpc();
						}
						else if (GameCanvas.panel.currentTabIndex == 0)
						{
							if (GameCanvas.panel.cp == null)
							{
								x = GameCanvas.panel.xScroll + GameCanvas.panel.wScroll / 2;
								y = GameCanvas.panel.yScroll + 20;
							}
							else if (GameCanvas.menu.tDelay != 0)
							{
								x = GameCanvas.panel.xScroll + 25;
								y = GameCanvas.panel.yScroll + 60;
							}
						}
						else if (GameCanvas.panel.currentTabIndex == 1)
						{
							x = GameCanvas.panel.startTabPos + 10;
							y = 65;
						}
					}
					if (index == 4)
					{
						if (GameCanvas.panel.isShow)
						{
							x = GameCanvas.panel.cmdClose.x + 5;
							y = GameCanvas.panel.cmdClose.y + 5;
						}
						else if (GameCanvas.menu.showMenu)
						{
							x = GameCanvas.w / 2;
							y = GameCanvas.h - 20;
						}
						else
						{
							clickNpc();
						}
					}
					if (index == 5)
					{
						clickNpc();
					}
					return;
				case 1:
					if (ChatPopup.currChatPopup != null || Char.myCharz().statusMe == 14)
					{
						x = GameCanvas.w / 2;
						y = GameCanvas.h - 15;
						return;
					}
					if (index == 0)
					{
						if (TileMap.isOfflineMap())
						{
							nextMap(0);
						}
						else
						{
							clickMob();
						}
					}
					if (index == 1)
					{
						if (!TileMap.isOfflineMap())
						{
							nextMap(1);
						}
						else
						{
							clickNpc();
						}
					}
					return;
				case 2:
					if (ChatPopup.currChatPopup != null || Char.myCharz().statusMe == 14)
					{
						x = GameCanvas.w / 2;
						y = GameCanvas.h - 15;
						return;
					}
					if (index == 0)
					{
						if (!TileMap.isOfflineMap())
						{
							isViewMap = true;
						}
						if (!GameCanvas.panel.isShow)
						{
							if (!isViewMap)
							{
								x = GameScr.gI().cmdMenu.x;
								y = GameScr.gI().cmdMenu.y + 13;
								trans = 1;
							}
							else
							{
								if (GameScr.getTaskMapId() == TileMap.mapID)
								{
									if (!isHaveItem())
									{
										clickMob();
									}
								}
								else
								{
									nextMap(0);
								}
								if (isViewMap)
								{
									isCloseMap = true;
								}
							}
						}
						else if (!isViewMap)
						{
							if (GameCanvas.panel.currentTabIndex == 0)
							{
								int num = ((GameCanvas.h <= 300) ? 10 : 15);
								x = GameCanvas.panel.xScroll + GameCanvas.panel.wScroll / 2;
								y = GameCanvas.panel.yScroll + GameCanvas.panel.hScroll - num;
							}
							else
							{
								x = GameCanvas.panel.startTabPos + 10;
								y = 65;
							}
						}
						else if (!isCloseMap)
						{
							x = GameCanvas.panel.cmdClose.x + 5;
							y = GameCanvas.panel.cmdClose.y + 5;
						}
						else
						{
							isPaint = false;
						}
						if (Char.myCharz().cMP <= 0)
						{
							x = GameScr.xHP + 5;
							y = GameScr.yHP + 13;
							isCamera = false;
						}
					}
					if (index == 1)
					{
						isPaint = false;
						isPaintArrow = false;
					}
					return;
				case 3:
					if (ChatPopup.currChatPopup != null || Char.myCharz().statusMe == 14)
					{
						x = GameCanvas.w / 2;
						y = GameCanvas.h - 15;
					}
					else if (index == 0)
					{
						if (!GameCanvas.panel.isShow)
						{
							if (!isViewPotential)
							{
								x = GameScr.gI().cmdMenu.x;
								y = GameScr.gI().cmdMenu.y + 13;
								trans = 1;
							}
							else
							{
								if (GameScr.getTaskMapId() == TileMap.mapID)
								{
									if (!isHaveItem())
									{
										clickMob();
									}
								}
								else
								{
									nextMap(0);
								}
								if (isViewMap)
								{
									isCloseMap = true;
								}
							}
						}
						else if (!isViewPotential)
						{
							int num2 = ((GameCanvas.h <= 300) ? 10 : 15);
							x = GameCanvas.panel.xScroll + 10 + 108 - 18;
							y = 65;
						}
						else if (!isCloseMap)
						{
							x = GameCanvas.panel.cmdClose.x + 5;
							y = GameCanvas.panel.cmdClose.y + 5;
						}
						else
						{
							isPaint = false;
						}
						if (Char.myCharz().cMP <= 0)
						{
							x = GameScr.xHP + 5;
							y = GameScr.yHP + 13;
							isCamera = false;
						}
					}
					else
					{
						isPaint = false;
						isPaintArrow = false;
					}
					return;
				}
				if (Char.myCharz().taskMaint.taskId == 9 && Char.myCharz().taskMaint.index == 2)
				{
					for (int i = 0; i < PopUp.vPopups.size(); i++)
					{
						PopUp popUp = (PopUp)PopUp.vPopups.elementAt(i);
						if (popUp.cy <= 24)
						{
							x = popUp.cx + popUp.sayWidth / 2;
							y = popUp.cy + 30;
							isCamera = true;
							isPaint = false;
							isPaintArrow = true;
							return;
						}
					}
				}
				isPaint = false;
				isPaintArrow = false;
			}
			else
			{
				isPaint = false;
				isPaintArrow = false;
			}
		}

	public static void update()
		{
			hint();
			int num = ((trans != 0) ? (-2) : 2);
			if (!activeClick)
			{
				paintFlare = false;
				t++;
				if (t == 50)
				{
					t = 0;
					activeClick = true;
				}
				return;
			}
			t++;
			if (type == 0)
			{
				if (t == 2)
				{
					x += 2 * num;
					y -= 4;
					paintFlare = true;
				}
				if (t == 4)
				{
					x -= 2 * num;
					y += 4;
					activeClick = false;
					paintFlare = false;
					t = 0;
				}
				if (t > 4)
				{
					activeClick = false;
				}
			}
			if (type != 1)
			{
				return;
			}
			if (t == 2)
			{
				if (GameCanvas.isTouch)
				{
					GameScr.startFlyText(mResources.press_twice, x, y + 10, 0, 20, mFont.MISS_ME);
				}
				paintFlare = true;
				x += 2 * num;
				y -= 4;
			}
			if (t == 4)
			{
				paintFlare = false;
				x -= num;
				y += 2;
			}
			if (t == 6)
			{
				paintFlare = true;
				x += num;
				y -= 2;
			}
			if (t == 8)
			{
				paintFlare = false;
				x -= num;
				y += 2;
			}
			if (t == 10)
			{
				x -= num;
				y += 2;
				activeClick = false;
				t = 0;
			}
		}

}
