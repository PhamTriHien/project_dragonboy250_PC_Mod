using System;

public partial class ChatPopup
{
	public override void update()
		{
			if (scr != null)
			{
				GameScr.info1.isUpdate = false;
				scr.updatecm();
			}
			else
			{
				GameScr.info1.isUpdate = true;
			}
			if (GameCanvas.menu.showMenu)
			{
				strY = 0;
				cx = GameCanvas.w / 2 - sayWidth / 2 - 1;
				cy = GameCanvas.menu.menuY - ch;
			}
			else
			{
				strY = 0;
				if (GameScr.gI().right != null || GameScr.gI().left != null || GameScr.gI().center != null || cmdNextLine != null || cmdMsg1 != null)
				{
					strY = 5;
					cx = GameCanvas.w / 2 - sayWidth / 2 - 1;
					cy = GameCanvas.h - 20 - ch;
				}
				else
				{
					cx = GameCanvas.w / 2 - sayWidth / 2 - 1;
					cy = GameCanvas.h - 5 - ch;
				}
			}
			if (delay > 0)
			{
				delay--;
			}
			if (performDelay > 0)
			{
				performDelay--;
			}
			else
			{
				GameScr.info1.info.time = 0;
				for (int i = 0; i < GameScr.info1.info.infoWaitToShow.size(); i++)
				{
					if (((InfoItem)GameScr.info1.info.infoWaitToShow.elementAt(i)).speed != 70)
					{
						((InfoItem)GameScr.info1.info.infoWaitToShow.elementAt(i)).speed = 10;
					}
				}
			}
			if (sayRun > 1)
			{
				sayRun--;
			}
			if ((c != null && Char.chatPopup != null && Char.chatPopup != this) || (c != null && Char.chatPopup == null) || delay <= 0)
			{
				Effect2.vEffect2Outside.removeElement(this);
				Effect2.vEffect2.removeElement(this);
			}
		}

	private void doKeyText(int type)
		{
			cmyText += 12 * type;
			if (cmyText < 0)
			{
				cmyText = 0;
			}
			if (cmyText > lim)
			{
				cmyText = lim;
			}
		}

	public void updateKey()
		{
			if (isClip)
			{
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
				if (GameCanvas.isPointerHoldIn(cx, 0, sayWidth + 2, ch))
				{
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
						if (cmyText > lim)
						{
							cmyText = lim;
						}
					}
					else
					{
						pyy = 0;
						pyy = 0;
					}
				}
			}
			if (scr != null)
			{
				if (GameCanvas.isTouch)
				{
					scr.updateKey();
				}
				if (GameCanvas.keyHold[(!Main.isPC) ? 2 : 21])
				{
					scr.cmtoY -= 12;
					if (scr.cmtoY < 0)
					{
						scr.cmtoY = 0;
					}
				}
				if (GameCanvas.keyHold[(!Main.isPC) ? 8 : 22])
				{
					GameCanvas.keyPressed[(!Main.isPC) ? 8 : 22] = false;
					scr.cmtoY += 12;
					if (scr.cmtoY > scr.cmyLim)
					{
						scr.cmtoY = scr.cmyLim;
					}
				}
			}
			if (GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25] || mScreen.getCmdPointerLast(GameCanvas.currentScreen.center))
			{
				GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25] = false;
				mScreen.keyTouch = -1;
				if (cmdNextLine != null)
				{
					cmdNextLine.performAction();
				}
				else if (cmdMsg1 != null)
				{
					cmdMsg1.performAction();
				}
				else if (cmdMsg2 != null)
				{
					cmdMsg2.performAction();
				}
			}
			if (scr == null || !scr.pointerIsDowning)
			{
				if (cmdMsg1 != null && (GameCanvas.keyPressed[12] || GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25] || mScreen.getCmdPointerLast(cmdMsg1)))
				{
					GameCanvas.keyPressed[12] = false;
					GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25] = false;
					GameCanvas.isPointerClick = false;
					GameCanvas.isPointerJustRelease = false;
					cmdMsg1.performAction();
					mScreen.keyTouch = -1;
				}
				if (cmdMsg2 != null && (GameCanvas.keyPressed[13] || mScreen.getCmdPointerLast(cmdMsg2)))
				{
					GameCanvas.keyPressed[13] = false;
					GameCanvas.isPointerClick = false;
					GameCanvas.isPointerJustRelease = false;
					cmdMsg2.performAction();
					mScreen.keyTouch = -1;
				}
			}
		}

	public void perform(int idAction, object p)
		{
			if (idAction == 1000)
			{
				try
				{
					GameMidlet.instance.platformRequest((string)p);
				}
				catch (Exception)
				{
				}
				if (!Main.isPC)
				{
					GameMidlet.instance.notifyDestroyed();
				}
				else
				{
					idAction = 1001;
				}
				GameCanvas.endDlg();
			}
			if (idAction == 1001)
			{
				scr = null;
				Char.chatPopup = null;
				serverChatPopUp = null;
				GameScr.info1.isUpdate = true;
				Char.isLockKey = false;
				if (isHavePetNpc)
				{
					GameScr.info1.info.time = 0;
					GameScr.info1.info.info.speed = 10;
				}
			}
			if (idAction != 8000 || performDelay > 0)
			{
				return;
			}
			int num = currChatPopup.currentLine;
			num++;
			if (num >= currChatPopup.lines.Length)
			{
				Char.chatPopup = null;
				currChatPopup = null;
				GameScr.info1.isUpdate = true;
				Char.isLockKey = false;
				if (nextMultiChatPopUp != null)
				{
					num = 0;
					addChatPopupMultiLine(nextMultiChatPopUp, 100000, nextChar);
					nextMultiChatPopUp = null;
					nextChar = null;
				}
				else
				{
					if (!isHavePetNpc)
					{
						return;
					}
					GameScr.info1.info.time = 0;
					for (int i = 0; i < GameScr.info1.info.infoWaitToShow.size(); i++)
					{
						if (((InfoItem)GameScr.info1.info.infoWaitToShow.elementAt(i)).speed == 10000000)
						{
							((InfoItem)GameScr.info1.info.infoWaitToShow.elementAt(i)).speed = 10;
						}
					}
				}
			}
			else
			{
				ChatPopup chatPopup = addChatPopup(currChatPopup.lines[num], currChatPopup.delay, currChatPopup.c);
				chatPopup.currentLine = num;
				chatPopup.lines = currChatPopup.lines;
				chatPopup.cmdNextLine = currChatPopup.cmdNextLine;
				currChatPopup = chatPopup;
			}
		}

}
