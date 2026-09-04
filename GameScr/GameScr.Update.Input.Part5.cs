using System;
using Assets.src.g;
public partial class GameScr : mScreen, IChatable
{
	public void updateKeyAlert()
			{
				if (!isPaintAlert || GameCanvas.currentDialog != null)
				{
					return;
				}
				bool flag = false;
				if (GameCanvas.keyPressed[Key.NUM8])
				{
					indexRow++;
					if (indexRow >= texts.size())
					{
						indexRow = 0;
					}
					flag = true;
				}
				else if (GameCanvas.keyPressed[Key.NUM2])
				{
					indexRow--;
					if (indexRow < 0)
					{
						indexRow = texts.size() - 1;
					}
					flag = true;
				}
				if (flag)
				{
					scrMain.moveTo(indexRow * scrMain.ITEM_SIZE);
					GameCanvas.clearKeyHold();
					GameCanvas.clearKeyPressed();
				}
				if (GameCanvas.isTouch)
				{
					ScrollResult scrollResult = scrMain.updateKey();
					if (scrollResult.isDowning || scrollResult.isFinish)
					{
						indexRow = scrollResult.selected;
						flag = true;
					}
				}
				if (!flag || indexRow < 0 || indexRow >= texts.size())
				{
					return;
				}
				string text = (string)texts.elementAt(indexRow);
				int num = -1;
				fnick = null;
				alertURL = null;
				center = null;
				ChatTextField.gI().center = null;
				if ((num = text.IndexOf("http://")) >= 0)
				{
					Cout.println("currentLine: " + text);
					alertURL = text.Substring(num);
					center = new Command(mResources.open_link, 12000);
					if (!GameCanvas.isTouch)
					{
						ChatTextField.gI().center = new Command(mResources.open_link, null, 12000, null);
					}
				}
				else
				{
					if ((num = text.IndexOf("@")) < 0)
					{
						return;
					}
					string text2 = text.Substring(2);
					text2 = text2.Trim();
					num = text2.IndexOf("@");
					string text3 = text2.Substring(num);
					int num2 = -1;
					num2 = text3.IndexOf(" ");
					num2 = ((num2 > 0) ? (num2 + num) : (num + text3.Length));
					fnick = text2.Substring(num + 1, num2);
					if (!fnick.Equals(string.Empty) && !fnick.Equals(Char.myCharz().cName))
					{
						center = new Command(mResources.SELECT, 12009, fnick);
						if (!GameCanvas.isTouch)
						{
							ChatTextField.gI().center = new Command(mResources.SELECT, null, 12009, fnick);
						}
					}
					else
					{
						fnick = null;
						center = null;
					}
				}
			}
	public bool isNotPaintTouchControl()
			{
				if (!GameCanvas.isTouchControl && GameCanvas.currentScreen == gI())
				{
					return true;
				}
				if (!GameCanvas.isTouch)
				{
					return true;
				}
				if (ChatTextField.gI().isShow)
				{
					return true;
				}
				if (InfoDlg.isShow)
				{
					return true;
				}
				if (GameCanvas.currentDialog != null || ChatPopup.currChatPopup != null || GameCanvas.menu.showMenu || GameCanvas.panel.isShow || isPaintPopup())
				{
					return true;
				}
				return false;
			}
	private static void setTouchBtn()
			{
				if (isAnalog != 0)
				{
					xTG = (xF = GameCanvas.w - 45);
					if (gamePad.isLargeGamePad)
					{
						xSkill = gamePad.wZone + 20;
						wSkill = 35;
						xHP = xF - 45;
					}
					else if (gamePad.isMediumGamePad)
					{
						xHP = xF - 45;
					}
					yF = GameCanvas.h - 45;
					yTG = yF - 45;
				}
			}

}
