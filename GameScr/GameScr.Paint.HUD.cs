using System;
using Assets.src.g;

public partial class GameScr : mScreen, IChatable
{
	public void paintCapcha(mGraphics g)
			{
				MobCapcha.paint(g, Char.myCharz().cx, Char.myCharz().cy);
				g.translate(-g.getTranslateX(), -g.getTranslateY());
				if (GameCanvas.menu.showMenu || GameCanvas.panel.isShow || ChatPopup.currChatPopup != null || !GameCanvas.isTouch)
				{
					return;
				}
				for (int i = 0; i < strCapcha.Length; i++)
				{
					int x = (GameCanvas.w - strCapcha.Length * disXC) / 2 + i * disXC + disXC / 2;
					if (keyCapcha[i] == -1)
					{
						g.drawImage(imgNut, x, GameCanvas.h - 25, 3);
						mFont.tahoma_7b_dark.drawString(g, strCapcha[i] + string.Empty, x, GameCanvas.h - 30, 2);
					}
					else
					{
						g.drawImage(imgNutF, x, GameCanvas.h - 25, 3);
						mFont.tahoma_7b_green2.drawString(g, strCapcha[i] + string.Empty, x, GameCanvas.h - 30, 2);
					}
				}
			}

	private void paintTouchControl(mGraphics g)
			{
				if (isNotPaintTouchControl())
				{
					return;
				}
				resetTranslate(g);
				if (!TileMap.isOfflineMap() && !isVS())
				{
					if (mScreen.keyTouch == 15 || mScreen.keyMouse == 15)
					{
						g.drawImage((!Main.isPC) ? imgChat2 : imgChatsPC2, xC + 17, yC + 17 + mGraphics.addYWhenOpenKeyBoard, mGraphics.HCENTER | mGraphics.VCENTER);
					}
					else
					{
						g.drawImage((!Main.isPC) ? imgChat : imgChatPC, xC + 17, yC + 17 + mGraphics.addYWhenOpenKeyBoard, mGraphics.HCENTER | mGraphics.VCENTER);
					}
				}
				if (isUseTouch)
				{
				}
			}

	public void paintImageBarRight(mGraphics g, Char c)
			{
				int num = (int)(c.cHP * hpBarW / c.cHPFull);
				int num2 = (int)(c.cMP * mpBarW / c.cMPFull);
				int num3 = (int)(dHP * hpBarW / c.cHPFull);
				int num4 = (int)(dMP * mpBarW / c.cMPFull);
				g.setClip(GameCanvas.w / 2 + 58 - mGraphics.getImageWidth(imgPanel), 0, 95, 100);
				g.drawRegion(imgPanel, 0, 0, mGraphics.getImageWidth(imgPanel), mGraphics.getImageHeight(imgPanel), 2, GameCanvas.w / 2 + 60, 0, mGraphics.RIGHT | mGraphics.TOP);
				g.setClip((int)(GameCanvas.w / 2 + 60 - 83 - hpBarW + hpBarW - num3), 5, num3, 10);
				g.drawImage(imgHPLost, GameCanvas.w / 2 + 60 - 83, 5, mGraphics.RIGHT | mGraphics.TOP);
				g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
				g.setClip((int)(GameCanvas.w / 2 + 60 - 83 - hpBarW + hpBarW - num), 5, num, 10);
				g.drawImage(imgHP, GameCanvas.w / 2 + 60 - 83, 5, mGraphics.RIGHT | mGraphics.TOP);
				g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
				g.setClip((int)(GameCanvas.w / 2 + 60 - 83 - mpBarW + hpBarW - num4), 20, num4, 6);
				g.drawImage(imgMPLost, GameCanvas.w / 2 + 60 - 83, 20, mGraphics.RIGHT | mGraphics.TOP);
				g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
				g.setClip((int)(GameCanvas.w / 2 + 60 - 83 - mpBarW + hpBarW - num2), 20, num2, 6);
				g.drawImage(imgMP, GameCanvas.w / 2 + 60 - 83, 20, mGraphics.RIGHT | mGraphics.TOP);
				g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
			}

	private void paintImageBar(mGraphics g, bool isLeft, Char c)
			{
				if (c != null)
				{
					int num = 0;
					int num2 = 0;
					int num3 = 0;
					int num4 = 0;
					if (c.charID == Char.myCharz().charID)
					{
						num = (int)(dHP * hpBarW / c.cHPFull);
						num2 = (int)(dMP * mpBarW / c.cMPFull);
						num3 = (int)(c.cHP * hpBarW / c.cHPFull);
						num4 = (int)(c.cMP * mpBarW / c.cMPFull);
					}
					else
					{
						num = (int)(c.dHP * hpBarW / c.cHPFull);
						num2 = c.perCentMp * mpBarW / 100;
						num3 = (int)(c.cHP * hpBarW / c.cHPFull);
						num4 = c.perCentMp * mpBarW / 100;
					}
					if (Char.myCharz().secondPower > 0)
					{
						int w = Char.myCharz().powerPoint * spBarW / Char.myCharz().maxPowerPoint;
						g.drawImage(imgPanel2, 58, 29, 0);
						g.setClip(83, 31, w, 10);
						g.drawImage(imgSP, 83, 31, 0);
						g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
						mFont.tahoma_7_white.drawString(g, Char.myCharz().strInfo + ":" + Char.myCharz().powerPoint + "/" + Char.myCharz().maxPowerPoint, 115, 29, 2);
					}
					if (c.charID != Char.myCharz().charID)
					{
						g.setClip(mGraphics.getImageWidth(imgPanel) - 95, 0, 95, 100);
					}
					g.drawImage(imgPanel, 0, 0, 0);
					if (isLeft)
					{
						g.setClip(83, 5, num, 10);
					}
					else
					{
						g.setClip((int)(83 + hpBarW - num), 5, num, 10);
					}
					g.drawImage(imgHPLost, 83, 5, 0);
					g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
					if (isLeft)
					{
						g.setClip(83, 5, num3, 10);
					}
					else
					{
						g.setClip((int)(83 + hpBarW - num3), 5, num3, 10);
					}
					g.drawImage(imgHP, 83, 5, 0);
					g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
					if (isLeft)
					{
						g.setClip(83, 20, num2, 6);
					}
					else
					{
						g.setClip(83 + mpBarW - num2, 20, num2, 6);
					}
					g.drawImage(imgMPLost, 83, 20, 0);
					g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
					if (isLeft)
					{
						g.setClip(83, 20, num2, 6);
					}
					else
					{
						g.setClip(83 + mpBarW - num4, 20, num4, 6);
					}
					g.drawImage(imgMP, 83, 20, 0);
					g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
					if (Char.myCharz().cMP == 0 && GameCanvas.gameTick % 10 > 5)
					{
						g.setClip(83, 20, 2, 6);
						g.drawImage(imgMPLost, 83, 20, 0);
						g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
					}
				}
			}

	private void paintInfoBar(mGraphics g)
			{
				resetTranslate(g);
				if (TileMap.mapID == 130 && findCharVS1() != null && findCharVS2() != null)
				{
					g.translate(GameCanvas.w / 2 - 62, 0);
					paintImageBar(g, isLeft: true, findCharVS1());
					g.translate(-(GameCanvas.w / 2 - 65), 0);
					paintImageBarRight(g, findCharVS2());
					findCharVS1().paintHeadWithXY(g, 137, 25, 0);
					findCharVS2().paintHeadWithXY(g, GameCanvas.w - 15 - 122, 25, 2);
				}
				else if (isVS() && Char.myCharz().charFocus != null)
				{
					g.translate(GameCanvas.w / 2 - 62, 0);
					paintImageBar(g, isLeft: true, Char.myCharz().charFocus);
					g.translate(-(GameCanvas.w / 2 - 65), 0);
					paintImageBarRight(g, Char.myCharz());
					Char.myCharz().paintHeadWithXY(g, 137, 25, 0);
					Char.myCharz().charFocus.paintHeadWithXY(g, GameCanvas.w - 15 - 122, 25, 2);
				}
				else if (ispaintPhubangBar() && isSmallScr())
				{
					paintHPBar_NEW(g, 1, 1, Char.myCharz());
				}
				else
				{
					paintImageBar(g, isLeft: true, Char.myCharz());
					if (Char.myCharz().isInEnterOfflinePoint() != null || Char.myCharz().isInEnterOnlinePoint() != null)
					{
						mFont.tahoma_7_green2.drawString(g, mResources.enter, imgScrW / 2, 8 + mGraphics.addYWhenOpenKeyBoard, mFont.CENTER);
					}
					else if (Char.myCharz().mobFocus != null)
					{
						if (Char.myCharz().mobFocus.getTemplate() != null)
						{
							mFont.tahoma_7b_green2.drawString(g, Char.myCharz().mobFocus.getTemplate().name, imgScrW / 2, 9 + mGraphics.addYWhenOpenKeyBoard, mFont.CENTER);
						}
						if (Char.myCharz().mobFocus.templateId != 0)
						{
							mFont.tahoma_7b_green2.drawString(g, NinjaUtil.getMoneys(Char.myCharz().mobFocus.hp) + string.Empty, imgScrW / 2, 22 + mGraphics.addYWhenOpenKeyBoard, mFont.CENTER);
						}
					}
					else if (Char.myCharz().npcFocus != null)
					{
						mFont.tahoma_7b_green2.drawString(g, Char.myCharz().npcFocus.template.name, imgScrW / 2, 9 + mGraphics.addYWhenOpenKeyBoard, mFont.CENTER);
						if (Char.myCharz().npcFocus.template.npcTemplateId == 4)
						{
							mFont.tahoma_7b_green2.drawString(g, gI().magicTree.currPeas + "/" + gI().magicTree.maxPeas, imgScrW / 2, 22 + mGraphics.addYWhenOpenKeyBoard, mFont.CENTER);
						}
					}
					else if (Char.myCharz().charFocus != null)
					{
						mFont.tahoma_7b_green2.drawString(g, Char.myCharz().charFocus.cName, imgScrW / 2, 9 + mGraphics.addYWhenOpenKeyBoard, mFont.CENTER);
						mFont.tahoma_7b_green2.drawString(g, NinjaUtil.getMoneys(Char.myCharz().charFocus.cHP) + string.Empty, imgScrW / 2, 22 + mGraphics.addYWhenOpenKeyBoard, mFont.CENTER);
					}
					else
					{
						mFont.tahoma_7b_green2.drawString(g, Char.myCharz().cName, imgScrW / 2, 9 + mGraphics.addYWhenOpenKeyBoard, mFont.CENTER);
						mFont.tahoma_7b_green2.drawString(g, NinjaUtil.getMoneys(Char.myCharz().cPower) + string.Empty, imgScrW / 2, 22 + mGraphics.addYWhenOpenKeyBoard, mFont.CENTER);
					}
				}
				g.translate(-g.getTranslateX(), -g.getTranslateY());
				if (isVS() && secondVS > 0)
				{
					curr = mSystem.currentTimeMillis();
					if (curr - last >= 1000)
					{
						last = mSystem.currentTimeMillis();
						secondVS--;
					}
					mFont.tahoma_7b_white.drawString(g, secondVS + string.Empty, GameCanvas.w / 2, 13, 2, mFont.tahoma_7b_dark);
				}
				if (flareFindFocus)
				{
					g.drawImage(ItemMap.imageFlare, 40, 35, mGraphics.BOTTOM | mGraphics.HCENTER);
					flareTime--;
					if (flareTime < 0)
					{
						flareTime = 0;
						flareFindFocus = false;
					}
				}
			}

	private void paintGamePad(mGraphics g)
			{
				if (isAnalog != 0 && Char.myCharz().statusMe != 14)
				{
					g.drawImage((mScreen.keyTouch != 5 && mScreen.keyMouse != 5) ? imgFire0 : imgFire1, xF + 20, yF + 20, mGraphics.HCENTER | mGraphics.VCENTER);
					gamePad.paint(g);
					g.drawImage((mScreen.keyTouch != 13) ? imgFocus : imgFocus2, xTG + 20, yTG + 20, mGraphics.HCENTER | mGraphics.VCENTER);
				}
			}

	public static void paintHPBar_NEW(mGraphics g, int x, int y, Char c)
			{
				g.drawImage(imgKhung, x, y, 0);
				int x2 = x + 3;
				int num = y + 19;
				int num2 = 0;
				int num3 = 0;
				int width = imgHP_NEW.getWidth();
				int num4 = imgHP_NEW.getHeight() / 2;
				num2 = (int)(c.cHP * width / c.cHPFull);
				if (num2 <= 0)
				{
					num2 = 1;
				}
				else if (num2 > width)
				{
					num2 = width;
				}
				g.drawRegion(imgHP_NEW, 0, num4, num2, num4, 0, x2, num, 0);
				num3 = (int)(c.cMP * width / c.cMPFull);
				if (num3 <= 0)
				{
					num3 = 1;
				}
				else if (num3 > width)
				{
					num3 = width;
				}
				g.drawRegion(imgHP_NEW, 0, 0, num3, num4, 0, x2, num + 6, 0);
				int x3 = x + imgKhung.getWidth() / 2 + 1;
				int y2 = num + 13;
				mFont.tahoma_7_green2.drawString(g, c.cName, x3, y + 4, 2);
				if (c.mobFocus != null)
				{
					if (c.mobFocus.getTemplate() != null)
					{
						mFont.tahoma_7_green2.drawString(g, c.mobFocus.getTemplate().name, x3, y2, 2);
					}
				}
				else if (c.npcFocus != null)
				{
					mFont.tahoma_7_green2.drawString(g, c.npcFocus.template.name, x3, y2, 2);
				}
				else if (c.charFocus != null)
				{
					mFont.tahoma_7_green2.drawString(g, c.charFocus.cName, x3, y2, 2);
				}
			}

	private void paint_xp_bar(mGraphics g)
			{
				g.setColor(8421504);
				g.fillRect(0, GameCanvas.h - 2, GameCanvas.w, 2);
				int w = (int)(Char.myCharz().cLevelPercent * GameCanvas.w / 10000);
				g.setColor(16777215);
				g.fillRect(0, GameCanvas.h - 2, w, 2);
				g.setColor(0);
				w = GameCanvas.w / 10;
				for (int i = 1; i < 10; i++)
				{
					g.fillRect(i * w, GameCanvas.h - 2, 1, 2);
				}
			}

}
