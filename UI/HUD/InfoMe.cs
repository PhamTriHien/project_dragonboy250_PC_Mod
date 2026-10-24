public class InfoMe
{
	public static InfoMe me;

	public int[][] charId = new int[3][];

	public Info info = new Info();

	public int dir;

	public int f;

	public int tF;

	public int cmtoY;

	public int cmy;

	public int cmdy;

	public int cmvy;

	public int cmyLim;

	public int cmtoX;

	public int cmx;

	public int cmdx;

	public int cmvx;

	public int cmxLim;

	public bool isDone;

	public bool isUpdate = true;

	public int timeDelay;

	public int playerID;

	public int timeCount;

	public Command cmdChat;

	public bool isShow;

	public InfoMe()
	{
		for (int i = 0; i < charId.Length; i++)
		{
			charId[i] = new int[3];
		}
	}

	public static InfoMe gI()
	{
		if (me == null)
		{
			me = new InfoMe();
		}
		return me;
	}

	public void loadCharId()
	{
		for (int i = 0; i < charId.Length; i++)
		{
			charId[i] = new int[3];
		}
	}

	public void paint(mGraphics g)
	{
		if ((Equals(GameScr.info2) && GameScr.gI().isVS()) || (Equals(GameScr.info2) && GameScr.gI().popUpYesNo != null) || !GameScr.isPaint || (GameCanvas.currentScreen != GameScr.gI() && GameCanvas.currentScreen != CrackBallScr.gI()) || ChatPopup.serverChatPopUp != null || !isUpdate || Char.ischangingMap || (GameCanvas.panel.isShow && Equals(GameScr.info2)))
		{
			return;
		}
		if (info == null || info.infoWaitToShow == null || info.infoWaitToShow.size() == 0 || info.info == null)
		{
			return;
		}
		if (cmy <= -40)
		{
			return;
		}
		g.translate(-g.getTranslateX(), -g.getTranslateY());
		g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
		info.paint(g, cmx, cmy, dir);
		if (info.info != null && info.info.charInfo == null && charId != null)
		{
			SmallImage.drawSmallImage(g, charId[Char.myCharz().cgender][f], cmx, cmy + 3, (dir != 1) ? 2 : 0, StaticObj.VCENTER_HCENTER);
		}
		g.translate(-g.getTranslateX(), -g.getTranslateY());
	}

	public void hide()
	{
		info.hide();
		info.info = null;
		info.says = null;
		isDone = true;
		cmy = -50;
		cmtoY = -50;
	}

	public void moveCamera()
	{
		if (cmy != cmtoY)
		{
			int dy = cmtoY - cmy;
			int vy = dy / 4;
			if (vy == 0)
			{
				vy = (dy > 0) ? 1 : -1;
			}
			cmy += vy;
			if ((dy > 0 && cmy > cmtoY) || (dy < 0 && cmy < cmtoY))
			{
				cmy = cmtoY;
			}
		}
		if (cmx != cmtoX)
		{
			int dx = cmtoX - cmx;
			int vx = dx / 4;
			if (vx == 0)
			{
				vx = (dx > 0) ? 1 : -1;
			}
			cmx += vx;
			if ((dx > 0 && cmx > cmtoX) || (dx < 0 && cmx < cmtoX))
			{
				cmx = cmtoX;
			}
		}
		tF++;
		if (tF >= 5)
		{
			tF = 0;
			f = (f == 0) ? 1 : 0;
		}
	}

	public void doClick(int t)
	{
		timeDelay = t;
	}

	public void update()
	{
		if (info == null)
		{
			return;
		}
		if (info.infoWaitToShow == null || info.infoWaitToShow.size() == 0 || info.info == null)
		{
			if (cmy > -50)
			{
				cmtoY = -50;
				moveCamera();
				if (cmy <= -45)
				{
					cmy = -50;
					info.info = null;
					info.says = null;
					isDone = true;
				}
			}
			return;
		}
		if ((Equals(GameScr.info2) && GameScr.gI().popUpYesNo != null) || !isUpdate)
		{
			return;
		}
		moveCamera();
		if (!isDone)
		{
			if (timeDelay > 0)
			{
				timeDelay--;
				if (timeDelay == 0)
				{
					GameCanvas.panel.setTypeMessage();
					GameCanvas.panel.show();
					if (info != null && info.info != null && info.info.charInfo != null)
					{
						GameCanvas.panel.addLogMessage(info.info);
					}
				}
			}
			if (Char.myCharz() != null)
			{
				int targetX = Char.myCharz().cx - GameScr.cmx + ((Char.myCharz().cdir == 1) ? -20 : 20);
				if (targetX < 24 + info.sayWidth / 2)
				{
					targetX = 24 + info.sayWidth / 2;
				}
				if (targetX > GameCanvas.w - 24 - info.sayWidth / 2)
				{
					targetX = GameCanvas.w - 24 - info.sayWidth / 2;
				}
				cmtoX = targetX;

				int targetY = Char.myCharz().cy - 40 - GameScr.cmy;
				int minY = (info.says != null) ? ((info.says.Length + 1) * 12 + 10) : 25;
				if (targetY < minY)
				{
					targetY = minY;
				}
				cmtoY = targetY;

				int diffX = cmtoX - cmx;
				if (Res.abs(diffX) > 4)
				{
					dir = (diffX > 0) ? 1 : -1;
				}
			}
			if (info.info != null && info.info.charInfo != null)
			{
				cmtoX = GameCanvas.w - info.W - 36;
				if (cmtoX < 5)
				{
					cmtoX = 5;
				}
				cmtoY = 5;
			}
		}
		if (info.info == null)
		{
			return;
		}
		if (info.infoWaitToShow.size() > 1)
		{
			if (info.info.timeCount == 0)
			{
				info.time++;
				if (info.time >= info.info.speed)
				{
					info.time = 0;
					info.infoWaitToShow.removeElementAt(0);
					if (info.infoWaitToShow.size() > 0)
					{
						InfoItem infoItem = (InfoItem)info.infoWaitToShow.firstElement();
						info.info = infoItem;
						info.getInfo();
					}
				}
				return;
			}
			info.info.curr = mSystem.currentTimeMillis();
			if (info.info.curr - info.info.last >= 1000)
			{
				info.info.last = mSystem.currentTimeMillis();
				info.info.timeCount--;
			}
			if (info.info.timeCount == 0)
			{
				info.infoWaitToShow.removeElementAt(0);
				if (info.infoWaitToShow.size() != 0)
				{
					InfoItem infoItem2 = (InfoItem)info.infoWaitToShow.firstElement();
					info.info = infoItem2;
					info.getInfo();
				}
			}
		}
		else if (info.infoWaitToShow.size() == 1)
		{
			if (info.info.timeCount == 0)
			{
				info.time++;
				if (info.time >= info.info.speed)
				{
					isDone = true;
					cmtoY = -50;
				}
				if (info.time >= info.info.speed + 15)
				{
					info.time = 0;
					info.infoWaitToShow.removeAllElements();
					info.info = null;
					info.says = null;
					info.timeW = 200;
					cmy = -50;
					cmtoY = -50;
				}
			}
			else
			{
				info.info.curr = mSystem.currentTimeMillis();
				if (info.info.curr - info.info.last >= 1000)
				{
					info.info.last = mSystem.currentTimeMillis();
					info.info.timeCount--;
				}
				if (info.info.timeCount == 0)
				{
					isDone = true;
					cmtoY = -50;
					info.time = 0;
					info.infoWaitToShow.removeAllElements();
					info.info = null;
					info.says = null;
					cmdChat = null;
					cmy = -50;
				}
			}
		}
	}

	public void addInfoWithChar(string s, Char c, bool isChatServer)
	{
		playerID = c.charID;
		info.addInfo(s, 3, c, isChatServer);
		isDone = false;
		if (info.infoWaitToShow.size() == 1)
		{
			cmx = GameCanvas.w - info.W - 36;
			if (cmx < 5)
			{
				cmx = 5;
			}
			cmy = -25;
		}
		ModMenu.ProcessServerBossNotice(s);
	}

	public void addInfo(string s, int Type)
	{
		s = Res.changeString(s);
		if (!string.IsNullOrEmpty(s) && !s.StartsWith("[Mod]"))
		{
			ModTdltLogger.LogServerMessage("INFO_TOAST_" + Type, s);
		}
		ModMenu.ProcessServerBossNotice(s);
		if (info.infoWaitToShow.size() > 0 && s.Equals(((InfoItem)info.infoWaitToShow.lastElement()).s))
		{
			return;
		}
		if (info.infoWaitToShow.size() > 10)
		{
			for (int i = 0; i < 5; i++)
			{
				info.infoWaitToShow.removeElementAt(0);
			}
		}
		Char cInfo = null;
		info.addInfo(s, Type, cInfo, isChatServer: false);
		if (info.infoWaitToShow.size() == 1)
		{
			cmy = 0;
			cmx = Char.myCharz().cx - GameScr.cmx + ((Char.myCharz().cdir != 1) ? 20 : (-20));
		}
		isDone = false;
	}
}
