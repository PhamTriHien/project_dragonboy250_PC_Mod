using System;
using Assets.src.g;
public partial class GameScr : mScreen, IChatable
{
	private void paintSelectedSkill(mGraphics g)
			{
				if (mobCapcha != null)
				{
					paintCapcha(g);
				}
				else
				{
					if (GameCanvas.currentDialog != null || ChatPopup.currChatPopup != null || GameCanvas.menu.showMenu || isPaintPopup() || GameCanvas.panel.isShow || Char.myCharz().taskMaint.taskId == 0 || ChatTextField.gI().isShow || GameCanvas.currentScreen == MoneyCharge.instance)
					{
						return;
					}
					long num = mSystem.currentTimeMillis();
					long num2 = num - lastUsePotion;
					int num3 = 0;
					if (num2 < 10000)
					{
						num3 = (int)(num2 * 20 / 10000);
					}
					if (!GameCanvas.isTouch)
					{
						g.drawImage((mScreen.keyTouch != 10) ? imgSkill : imgSkill2, xSkill + xHP - 1, yHP - 1, 0);
						SmallImage.drawSmallImage(g, 542, xSkill + xHP + 3, yHP + 3, 0, 0);
						mFont.number_gray.drawString(g, string.Empty + hpPotion, xSkill + xHP + 22, yHP + 15, 1);
						if (num2 < 10000)
						{
							g.setColor(2721889);
							num3 = (int)(num2 * 20 / 10000);
							g.fillRect(xSkill + xHP + 3, yHP + 3 + num3, 20, 20 - num3);
						}
					}
					else if (Char.myCharz().statusMe != 14)
					{
						if (isAnalog != 0)
						{
							int peaCx = xHP + 16;
							int peaCy = yHP + 16;
							int fillH = (num3 == 0) ? 18 : (num3 * 18 / 20);
							g.setColor(9670800);
							g.fillRect(peaCx - 10, peaCy - 10, 20, 18);
							g.setColor(16777215);
							g.fillRect(peaCx - 10, peaCy + 8 - fillH, 20, fillH);
							g.drawImage((mScreen.keyTouch != 10) ? imgHP3 : imgHP4, peaCx, peaCy, mGraphics.HCENTER | mGraphics.VCENTER);
							mFont.tahoma_7_red.drawString(g, string.Empty + hpPotion, peaCx, peaCy - 5, 2);
						}
						else
						{
							int peaCx = xHP + 20;
							int peaCy = yHP + 20;
							int fillH = (num3 == 0) ? 20 : num3;
							g.setColor(9670800);
							g.fillRect(peaCx - 11, peaCy - 10, 22, 20);
							g.setColor(16777215);
							g.fillRect(peaCx - 11, peaCy + 10 - fillH, 22, fillH);
							g.drawImage((mScreen.keyTouch != 10) ? imgHP1 : imgHP2, peaCx, peaCy, mGraphics.HCENTER | mGraphics.VCENTER);
							mFont.tahoma_7_red.drawString(g, string.Empty + hpPotion, peaCx, peaCy - 5, 2);
						}
					}
					if (isHaveSelectSkill)
					{
						Skill[] array = (Main.isPC ? keySkill : ((!GameCanvas.isTouch) ? keySkill : onScreenSkill));
						if (mScreen.keyTouch == 10)
						{
						}
						if (!GameCanvas.isTouch)
						{
							g.setColor(11152401);
							g.fillRect(xSkill + xHP + 2, yHP - 10 + 6, 20, 10);
							mFont.tahoma_7_white.drawString(g, "*", xSkill + xHP + 12, yHP - 8 + 6, mFont.CENTER);
						}
						int num5 = (Main.isPC ? array.Length : ((!GameCanvas.isTouch) ? array.Length : nSkill));
						for (int i = 0; i < num5; i++)
						{
							if (Main.isPC)
							{
								string[] array2 = (TField.isQwerty ? new string[10] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" } : new string[5] { "7", "8", "9", "10", "11" });
								int num6 = -13;
								mFont.tahoma_7b_dark.drawString(g, array2[i], xSkill + xS[i] + 14, yS[i] + num6, mFont.CENTER);
								mFont.tahoma_7b_white.drawString(g, array2[i], xSkill + xS[i] + 14, yS[i] + num6 + 1, mFont.CENTER);
							}
							else if (!GameCanvas.isTouch)
							{
								string[] array3 = (TField.isQwerty ? new string[5] { "Q", "W", "E", "R", "T" } : new string[5] { "7", "8", "9", "1", "3" });
								g.setColor(11152401);
								g.fillRect(xSkill + xS[i] + 2, yS[i] - 10 + 8, 20, 10);
								mFont.tahoma_7_white.drawString(g, array3[i], xSkill + xS[i] + 12, yS[i] - 10 + 6, mFont.CENTER);
							}
							Skill skill = array[i];
							if (skill != Char.myCharz().myskill)
							{
								g.drawImage(imgSkill, xSkill + xS[i] - 1, yS[i] - 1, 0);
							}
							if (skill == null)
							{
								continue;
							}
							if (skill == Char.myCharz().myskill)
							{
								g.drawImage(imgSkill2, xSkill + xS[i] - 1, yS[i] - 1, 0);
								if (GameCanvas.isTouch && !Main.isPC)
								{
									g.drawRegion(Mob.imgHP, 0, 12, 9, 6, 0, xSkill + xS[i] + 8, yS[i] - 7, 0);
								}
							}
							skill.paint(xSkill + xS[i] + 13, yS[i] + 13, g);
							if ((i == selectedIndexSkill && !isPaintUI() && GameCanvas.gameTick % 10 > 5) || i == keyTouchSkill)
							{
								g.drawImage(ItemMap.imageFlare, xSkill + xS[i] + 13, yS[i] + 14, 3);
							}
						}
					}
					paintGamePad(g);
				}
			}
	public void paintOpen(mGraphics g)
			{
				if (isstarOpen)
				{
					g.translate(-g.getTranslateX(), -g.getTranslateY());
					g.fillRect(0, 0, GameCanvas.w, moveUp);
					g.setColor(10275899);
					g.fillRect(0, moveUp - 1, GameCanvas.w, 1);
					g.fillRect(0, moveDow + 1, GameCanvas.w, 1);
				}
			}
	public static void paintSplash(mGraphics g)
			{
				for (int i = 0; i < 2; i++)
				{
					if (splashState[i] != -1)
					{
						if (splashDir[i] == 1)
						{
							g.drawImage(imgSplash[splashF[i]], splashX[i], splashY[i], 3);
						}
						else
						{
							g.drawRegion(imgSplash[splashF[i]], 0, 0, mGraphics.getImageWidth(imgSplash[splashF[i]]), mGraphics.getImageHeight(imgSplash[splashF[i]]), 2, splashX[i], splashY[i], 3);
						}
					}
				}
			}
	public void paintTitle(mGraphics g, string title, bool arrow)
			{
				int num = 0;
				num = gW / 2;
				g.setColor(Paint.COLORDARK);
				g.fillRoundRect(num - mFont.tahoma_8b.getWidth(title) / 2 - 12, popupY + 4, mFont.tahoma_8b.getWidth(title) + 22, 24, 6, 6);
				if ((indexTitle == 0 || GameCanvas.isTouch) && arrow)
				{
					SmallImage.drawSmallImage(g, 989, num - mFont.tahoma_8b.getWidth(title) / 2 - 15 - 7 - ((GameCanvas.gameTick % 8 <= 3) ? 2 : 0), popupY + 16, 2, StaticObj.VCENTER_HCENTER);
					SmallImage.drawSmallImage(g, 989, num + mFont.tahoma_8b.getWidth(title) / 2 + 15 + 5 + ((GameCanvas.gameTick % 8 <= 3) ? 2 : 0), popupY + 16, 0, StaticObj.VCENTER_HCENTER);
				}
				if (indexTitle == 0)
				{
					g.setColor(Paint.COLORFOCUS);
				}
				else
				{
					g.setColor(Paint.COLORBORDER);
				}
				g.drawRoundRect(num - mFont.tahoma_8b.getWidth(title) / 2 - 12, popupY + 4, mFont.tahoma_8b.getWidth(title) + 22, 24, 6, 6);
				mFont.tahoma_8b.drawString(g, title, num, popupY + 9, 2);
			}
	public void paintChatVip(mGraphics g)
			{
				if (vChatVip.size() != 0 && isPaintChatVip)
				{
					g.setClip(0, GameCanvas.h - 13, GameCanvas.w, 15);
					g.fillRect(0, GameCanvas.h - 13, GameCanvas.w, 15, 0, 90);
					string st = (string)vChatVip.elementAt(0);
					mFont.tahoma_7b_yellow.drawString(g, st, xChatVip, GameCanvas.h - 13, 0, mFont.tahoma_7b_dark);
				}
			}
	private void paint_ios_bg(mGraphics g)
			{
				if (mSystem.clientType == 5)
				{
					if (imgBgIOS != null)
					{
						g.setColor(16777215);
						g.fillRect(0, 0, GameCanvas.w, GameCanvas.h);
						g.drawImage(imgBgIOS, GameCanvas.w / 2, GameCanvas.h / 2, mGraphics.VCENTER | mGraphics.HCENTER);
					}
					else
					{
						int num = ((TileMap.bgID % 2 != 0) ? 1 : 2);
						imgBgIOS = GameCanvas.loadImage("/bg/bg_ios_" + num + ".png");
					}
				}
			}

}
