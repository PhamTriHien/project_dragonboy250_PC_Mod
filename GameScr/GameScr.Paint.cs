using System;
using Assets.src.g;

public partial class GameScr : mScreen, IChatable
{
	public static void paintOngMauPercent(Image img0, Image img1, Image img2, float x, float y, int size, float pixelPercent, mGraphics g)
			{
				int clipX = g.getClipX();
				int clipY = g.getClipY();
				int clipWidth = g.getClipWidth();
				int clipHeight = g.getClipHeight();
				g.setClip((int)x, (int)y, (int)pixelPercent, 13);
				int num = size / 15 - 2;
				for (int i = 0; i < num; i++)
				{
					g.drawImage(img1, x + (float)((i + 1) * 15), y, 0);
				}
				g.drawImage(img0, x, y, 0);
				g.drawImage(img1, x + (float)size - 30f, y, 0);
				g.drawImage(img2, x + (float)size - 15f, y, 0);
				g.setClip(clipX, clipY, clipWidth, clipHeight);
			}

	public void paintEffect(mGraphics g)
			{
				for (int i = 0; i < Effect2.vEffect2.size(); i++)
				{
					Effect2 effect = (Effect2)Effect2.vEffect2.elementAt(i);
					if (effect != null && !(effect is ChatPopup))
					{
						effect.paint(g);
					}
				}
				if (!GameCanvas.lowGraphic)
				{
					for (int i = 0; i < Effect2.vAnimateEffect.size(); i++)
					{
						Effect2 effect2 = (Effect2)Effect2.vAnimateEffect.elementAt(i);
						effect2.paint(g);
					}
				}
				for (int i = 0; i < Effect2.vEffect2Outside.size(); i++)
				{
					Effect2 effect3 = (Effect2)Effect2.vEffect2Outside.elementAt(i);
					effect3.paint(g);
				}
			}

	public void paintBgItem(mGraphics g, int layer)
			{
				if (ModMenu.graphicsQuality == 3)
				{
					return;
				}
				for (int i = 0; i < TileMap.vCurrItem.size(); i++)
				{
					BgItem bgItem = (BgItem)TileMap.vCurrItem.elementAt(i);
					if (bgItem.idImage != -1 && bgItem.layer == layer)
					{
						bgItem.paint(g);
					}
				}
				if (TileMap.mapID == 48 && layer == 3 && GameCanvas.bgW != null && GameCanvas.bgW[0] != 0)
				{
					for (int j = 0; j < TileMap.pxw / GameCanvas.bgW[0] + 1; j++)
					{
						g.drawImage(GameCanvas.imgBG[0], j * GameCanvas.bgW[0], TileMap.pxh - GameCanvas.bgH[0] - 70, 0);
					}
				}
			}

	public void paintBlackSky(mGraphics g)
			{
				if (!GameCanvas.lowGraphic)
				{
					g.fillTrans(imgTrans, 0, 0, GameCanvas.w, GameCanvas.h);
				}
			}

	public override void paint(mGraphics g)
			{
				countEff = 0;
				if (!isPaint)
				{
					return;
				}
				GameCanvas.debug("PA1", 1);
				if (isFreez || (isUseFreez && ChatPopup.currChatPopup == null))
				{
					dem++;
					if ((dem < 30 && dem >= 0 && GameCanvas.gameTick % 4 == 0) || (dem >= 30 && dem <= 50 && GameCanvas.gameTick % 3 == 0) || dem > 50)
					{
						g.setColor(16777215);
						g.fillRect(0, 0, GameCanvas.w, GameCanvas.h);
						if (dem <= 50)
						{
							return;
						}
						if (isUseFreez)
						{
							isUseFreez = false;
							dem = 0;
							if (activeRongThan)
							{
								callRongThan(xR, yR);
							}
							else
							{
								hideRongThan();
							}
						}
						paintInfoBar(g);
						g.translate(-cmx, -cmy);
						g.translate(0, GameCanvas.transY);
						Char.myCharz().paint(g);
						mSystem.paintFlyText(g);
						resetTranslate(g);
						paintSelectedSkill(g);
						return;
					}
				}
				GameCanvas.debug("PA2", 1);
				GameCanvas.paintBGGameScr(g);
				if (isRongThanXuatHien && TileMap.bgID != 3)
				{
					paintBlackSky(g);
				}
				else if (ModMenu.graphicsQuality < 2)
				{
					paint_ios_bg(g);
					if (isFireWorks && TileMap.bgID != 3)
					{
						paintBlackSky(g);
					}
				}
				GameCanvas.debug("PA3", 1);
				if (shock_scr > 0)
				{
					g.translate(-cmx + shock_x[shock_scr % shock_x.Length], -cmy + shock_y[shock_scr % shock_y.Length]);
					shock_scr--;
				}
				else
				{
					g.translate(-cmx, -cmy);
				}
				if (isSuperPower)
				{
					int tx = ((GameCanvas.gameTick % 3 != 0) ? (-3) : 3);
					g.translate(tx, 0);
				}
				if (ModMenu.graphicsQuality == 0)
				{
					BackgroudEffect.paintBehindTileAll(g);
					EffecMn.paintLayer1(g);
				}
				TileMap.paintTilemap(g);
				TileMap.paintOutTilemap(g);
				for (int i = 0; i < vCharInMap.size(); i++)
				{
					Char @char = (Char)vCharInMap.elementAt(i);
					if (@char.isMabuHold && TileMap.mapID == 128)
					{
						@char.paintHeadWithXY(g, @char.cx, @char.cy, 0);
					}
				}
				if (Char.myCharz().isMabuHold && TileMap.mapID == 128)
				{
					Char.myCharz().paintHeadWithXY(g, Char.myCharz().cx, Char.myCharz().cy, 0);
				}
				if (ModMenu.graphicsQuality < 3)
				{
					paintBgItem(g, 2);
				}
				if (Char.myCharz().cmdMenu != null && GameCanvas.isTouch)
				{
					if (mScreen.keyTouch == 20)
					{
						g.drawImage(imgChat2, Char.myCharz().cmdMenu.x + cmx, Char.myCharz().cmdMenu.y + cmy, mGraphics.HCENTER | mGraphics.VCENTER);
					}
					else
					{
						g.drawImage(imgChat, Char.myCharz().cmdMenu.x + cmx, Char.myCharz().cmdMenu.y + cmy, mGraphics.HCENTER | mGraphics.VCENTER);
					}
				}
				GameCanvas.debug("PA4", 1);
				GameCanvas.debug("PA5", 1);
				if (ModMenu.graphicsQuality == 0)
				{
					BackgroudEffect.paintBackAll(g);
					EffectManager.lowEffects.paintAll(g);
					for (int i = 0; i < Effect2.vEffectFeet.size(); i++)
					{
						Effect2 effect = (Effect2)Effect2.vEffectFeet.elementAt(i);
						effect.paint(g);
					}
				}
				for (int i = 0; i < Teleport.vTeleport.size(); i++)
				{
					((Teleport)Teleport.vTeleport.elementAt(i)).paintHole(g);
				}
				for (int i = 0; i < vNpc.size(); i++)
				{
					Npc npc = (Npc)vNpc.elementAt(i);
					if (npc.cHP > 0)
					{
						npc.paintShadow(g);
					}
				}
				for (int i = 0; i < vNpc.size(); i++)
				{
					((Npc)vNpc.elementAt(i)).paint(g);
				}
				g.translate(0, GameCanvas.transY);
				GameCanvas.debug("PA7", 1);
				GameCanvas.debug("PA8", 1);
				for (int i = 0; i < vCharInMap.size(); i++)
				{
					Char char2 = null;
					try
					{
						char2 = (Char)vCharInMap.elementAt(i);
					}
					catch (Exception ex)
					{
						Cout.LogError("Loi ham paint char gamesc: " + ex.ToString());
					}
					if (char2 != null && (!GameCanvas.panel.isShow || !GameCanvas.panel.isTypeShop()) && char2.isShadown)
					{
						char2.paintShadow(g);
					}
				}
				Char.myCharz().paintShadow(g);
				if (ModMenu.graphicsQuality == 0)
				{
					EffecMn.paintLayer2(g);
				}
				for (int i = 0; i < vMob.size(); i++)
				{
					((Mob)vMob.elementAt(i)).paint(g);
				}
				for (int i = 0; i < Teleport.vTeleport.size(); i++)
				{
					((Teleport)Teleport.vTeleport.elementAt(i)).paint(g);
				}
				for (int i = 0; i < vCharInMap.size(); i++)
				{
					Char char3 = null;
					try
					{
						char3 = (Char)vCharInMap.elementAt(i);
					}
					catch (Exception)
					{
					}
					if (char3 != null && (!GameCanvas.panel.isShow || !GameCanvas.panel.isTypeShop()))
					{
						char3.paint(g);
					}
				}
				Char.myCharz().paint(g);
				if (Char.myCharz().skillPaint != null && Char.myCharz().skillInfoPaint() != null && Char.myCharz().indexSkill < Char.myCharz().skillInfoPaint().Length)
				{
					Char.myCharz().paintCharWithSkill(g);
					Char.myCharz().paintMount2(g);
				}
				for (int i = 0; i < vCharInMap.size(); i++)
				{
					Char char4 = null;
					try
					{
						char4 = (Char)vCharInMap.elementAt(i);
					}
					catch (Exception ex3)
					{
						Cout.LogError("Loi ham paint char gamescr: " + ex3.ToString());
					}
					if (char4 != null && (!GameCanvas.panel.isShow || !GameCanvas.panel.isTypeShop()) && char4.skillPaint != null && char4.skillInfoPaint() != null && char4.indexSkill < char4.skillInfoPaint().Length)
					{
						char4.paintCharWithSkill(g);
						char4.paintMount2(g);
					}
				}
				for (int i = 0; i < vItemMap.size(); i++)
				{
					((ItemMap)vItemMap.elementAt(i)).paint(g);
				}
				g.translate(0, -GameCanvas.transY);
				GameCanvas.debug("PA9", 1);
				if (ModMenu.graphicsQuality == 0)
				{
					paintSplash(g);
					paintEffect(g);
				}
				if (ModMenu.graphicsQuality < 3)
				{
					paintBgItem(g, 3);
				}
				for (int i = 0; i < vNpc.size(); i++)
				{
					Npc npc2 = (Npc)vNpc.elementAt(i);
					npc2.paintName(g);
				}
				if (ModMenu.graphicsQuality == 0)
				{
					EffecMn.paintLayer3(g);
				}
				for (int i = 0; i < vNpc.size(); i++)
				{
					Npc npc3 = (Npc)vNpc.elementAt(i);
					if (npc3.chatInfo != null)
					{
						npc3?.chatInfo.paint(g, npc3.cx, npc3.cy - npc3.ch - GameCanvas.transY, npc3.cdir);
					}
				}
				for (int i = 0; i < vCharInMap.size(); i++)
				{
					Char char5 = null;
					try
					{
						char5 = (Char)vCharInMap.elementAt(i);
					}
					catch (Exception)
					{
					}
					if (char5 != null && char5.chatInfo != null)
					{
						char5.chatInfo.paint(g, char5.cx, char5.cy - char5.ch, char5.cdir);
					}
				}
				if (Char.myCharz().chatInfo != null)
				{
					Char.myCharz().chatInfo.paint(g, Char.myCharz().cx, Char.myCharz().cy - Char.myCharz().ch, Char.myCharz().cdir);
				}
				if (ModMenu.graphicsQuality == 0)
				{
					EffectManager.mid_2Effects.paintAll(g);
					EffectManager.midEffects.paintAll(g);
					BackgroudEffect.paintFrontAll(g);
				}
				if (ModMenu.graphicsQuality < 3)
				{
					for (int j = 0; j < TileMap.vCurrItem.size(); j++)
					{
						BgItem bgItem = (BgItem)TileMap.vCurrItem.elementAt(j);
						if (bgItem.idImage != -1 && bgItem.layer > 3)
						{
							bgItem.paint(g);
						}
					}
				}
				PopUp.paintAll(g);
				if (TileMap.mapID == 120)
				{
					if (percentMabu != 100)
					{
						int w = percentMabu * mGraphics.getImageWidth(imgHPLost) / 100;
						int num = percentMabu;
						g.drawImage(imgHPLost, TileMap.pxw / 2 - mGraphics.getImageWidth(imgHPLost) / 2, 220, 0);
						g.setClip(TileMap.pxw / 2 - mGraphics.getImageWidth(imgHPLost) / 2, 220, w, 10);
						g.drawImage(imgHP, TileMap.pxw / 2 - mGraphics.getImageWidth(imgHPLost) / 2, 220, 0);
						g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
					}
					if (mabuEff)
					{
						tMabuEff++;
						if (GameCanvas.gameTick % 3 == 0)
						{
							Effect me = new Effect(19, Res.random(TileMap.pxw / 2 - 50, TileMap.pxw / 2 + 50), 340, 2, 1, -1);
							EffecMn.addEff(me);
						}
						if (GameCanvas.gameTick % 15 == 0)
						{
							Effect me2 = new Effect(18, Res.random(TileMap.pxw / 2 - 5, TileMap.pxw / 2 + 5), Res.random(300, 320), 2, 1, -1);
							EffecMn.addEff(me2);
						}
						if (tMabuEff == 100)
						{
							activeSuperPower(TileMap.pxw / 2, 300);
						}
						if (tMabuEff == 110)
						{
							tMabuEff = 0;
							mabuEff = false;
						}
					}
				}
				if (ModMenu.graphicsQuality == 0)
				{
					BackgroudEffect.paintFog(g);
				}
				bool flag = true;
				for (int i = 0; i < BackgroudEffect.vBgEffect.size(); i++)
				{
					BackgroudEffect backgroudEffect = (BackgroudEffect)BackgroudEffect.vBgEffect.elementAt(i);
					if (backgroudEffect.typeEff == 0)
					{
						flag = false;
						break;
					}
				}
				if (mGraphics.zoomLevel <= 1 || Main.isIpod || Main.isIphone4)
				{
					flag = false;
				}
				if (flag && !isRongThanXuatHien)
				{
					int num2 = TileMap.pxw / (mGraphics.getImageWidth(TileMap.imgLight) + 50);
					if (num2 <= 0)
					{
						num2 = 1;
					}
					if (TileMap.tileID != 28)
					{
						for (int i = 0; i < num2; i++)
						{
							int num3 = 100 + i * (mGraphics.getImageWidth(TileMap.imgLight) + 50) - cmx / 2;
							int num4 = -20;
							int imageWidth = mGraphics.getImageWidth(TileMap.imgLight);
							if (num3 + imageWidth >= cmx && num3 <= cmx + GameCanvas.w && num4 + mGraphics.getImageHeight(TileMap.imgLight) >= cmy && num4 <= cmy + GameCanvas.h)
							{
								g.drawImage(TileMap.imgLight, 100 + i * (mGraphics.getImageWidth(TileMap.imgLight) + 50) - cmx / 2, num4, 0);
							}
						}
					}
				}
				mSystem.paintFlyText(g);
				GameCanvas.debug("PA14", 1);
				GameCanvas.debug("PA15", 1);
				GameCanvas.debug("PA16", 1);
				paintArrowPointToNPC(g);
				GameCanvas.debug("PA17", 1);
				if (!isPaintOther && isPaintRada == 1 && !GameCanvas.panel.isShow)
				{
					paintInfoBar(g);
				}
				resetTranslate(g);
				paint_xp_bar(g);
				if (!isPaintOther)
				{
					if (GameCanvas.open3Hour && TileMap.mapID != 170)
					{
						if (GameCanvas.w > 250)
						{
							g.drawImage(GameCanvas.img18, 160, 6, 0);
							mFont.tahoma_7_white.drawString(g, "Chơi quá 180 phút một ngày ", 180, 2, 0);
							mFont.tahoma_7_white.drawString(g, "sẽ ảnh hưởng xấu đến sức khỏe.", 180, 12, 0);
						}
						else
						{
							g.drawImage(GameCanvas.img18, 5, GameCanvas.h - 67, 0);
							mFont.tahoma_7_white.drawString(g, "Chơi quá 180 phút một ngày sẽ ảnh hưởng xấu đến sức khỏe.", 25, GameCanvas.h - 70, 0);
						}
					}
					GameCanvas.debug("PA21", 1);
					GameCanvas.debug("PA18", 1);
					g.translate(-g.getTranslateX(), -g.getTranslateY());
					if ((TileMap.mapID == 128 || TileMap.mapID == 127) && mabuPercent != 0)
					{
						int num5 = 30;
						int num6 = 200;
						g.setColor(0);
						g.fillRect(num5 - 27, num6 - 112, 54, 8);
						g.setColor(16711680);
						g.setClip(num5 - 25, num6 - 110, mabuPercent, 4);
						g.fillRect(num5 - 25, num6 - 110, 50, 4);
						g.setClip(0, 0, 3000, 3000);
						mFont.tahoma_7b_white.drawString(g, "Mabu", num5, num6 - 112 + 10, 2, mFont.tahoma_7b_dark);
					}
					if (Char.myCharz().isFusion)
					{
						Char.myCharz().tFusion++;
						if (GameCanvas.gameTick % 3 == 0)
						{
							g.setColor(16777215);
							g.fillRect(0, 0, GameCanvas.w, GameCanvas.h);
						}
						if (Char.myCharz().tFusion >= 100)
						{
							Char.myCharz().fusionComplete();
						}
					}
					for (int i = 0; i < vCharInMap.size(); i++)
					{
						Char char6 = null;
						try
						{
							char6 = (Char)vCharInMap.elementAt(i);
						}
						catch (Exception)
						{
						}
						if (char6 != null && char6.isFusion && Char.isCharInScreen(char6))
						{
							char6.tFusion++;
							if (GameCanvas.gameTick % 3 == 0)
							{
								g.setColor(16777215);
								g.fillRect(0, 0, GameCanvas.w, GameCanvas.h);
							}
							if (char6.tFusion >= 100)
							{
								char6.fusionComplete();
							}
						}
					}
					GameCanvas.paintz.paintTabSoft(g);
					GameCanvas.debug("PA19", 1);
					GameCanvas.debug("PA20", 1);
					resetTranslate(g);
					paintSelectedSkill(g);
					GameCanvas.debug("PA22", 1);
					resetTranslate(g);
					if (GameCanvas.isTouch && GameCanvas.isTouchControl)
					{
						paintTouchControl(g);
					}
					resetTranslate(g);
					paintChatVip(g);
					if (!GameCanvas.panel.isShow && GameCanvas.currentDialog == null && ChatPopup.currChatPopup == null && ChatPopup.serverChatPopUp == null && GameCanvas.currentScreen.Equals(instance))
					{
						base.paint(g);
						if (cmdMenu != null && left != cmdMenu && !GameCanvas.menu.showMenu)
						{
							cmdMenu.paint(g);
						}
						if (mScreen.keyMouse == 1 && cmdMenu != null)
						{
							g.drawImage(ItemMap.imageFlare, cmdMenu.x + 7, cmdMenu.y + 15, 3);
						}
					}
					resetTranslate(g);
					int num7 = 100 + ((Char.vItemTime.size() != 0) ? (textTime.size() * 12) : 0);
					if (Char.myCharz().clan != null)
					{
						int num8 = 0;
						int num9 = 0;
						int num10 = (GameCanvas.h - 100 - 60) / 12;
						for (int i = 0; i < vCharInMap.size(); i++)
						{
							Char char7 = (Char)vCharInMap.elementAt(i);
							if (char7.clanID == -1 || char7.clanID != Char.myCharz().clan.ID)
							{
								continue;
							}
							if (char7.isOutX() && char7.cx < Char.myCharz().cx)
							{
								int num11 = num10;
								if (Char.vItemTime.size() != 0)
								{
									num11 -= textTime.size();
								}
								if (num8 <= num11)
								{
									mFont.tahoma_7_green.drawString(g, char7.cName, 20, num7 - 12 + num8 * 12, mFont.LEFT, mFont.tahoma_7_grey);
									char7.paintHp(g, 10, num7 + num8 * 12 - 5);
									num8++;
								}
							}
							else if (char7.isOutX() && char7.cx > Char.myCharz().cx && num9 <= num10)
							{
								mFont.tahoma_7_green.drawString(g, char7.cName, GameCanvas.w - 25, num7 - 12 + num9 * 12, mFont.RIGHT, mFont.tahoma_7_grey);
								char7.paintHp(g, GameCanvas.w - 15, num7 + num9 * 12 - 5);
								num9++;
							}
						}
					}
					ChatTextField.gI().paint(g);
					if (isNewClanMessage && !GameCanvas.panel.isShow && GameCanvas.gameTick % 4 == 0)
					{
						g.drawImage(ItemMap.imageFlare, cmdMenu.x + 15, cmdMenu.y + 30, mGraphics.BOTTOM | mGraphics.HCENTER);
					}
					if (isSuperPower)
					{
						dxPower += 5;
						if (tPower >= 0)
						{
							tPower += dxPower;
						}
						Res.outz("x power= " + xPower);
						if (tPower < 0)
						{
							tPower--;
							if (tPower == -20)
							{
								isSuperPower = false;
								tPower = 0;
								dxPower = 0;
							}
						}
						else if ((xPower - tPower > 0 || tPower < TileMap.pxw) && tPower > 0)
						{
							g.setColor(16777215);
							if (!GameCanvas.lowGraphic)
							{
								g.fillArg(0, 0, GameCanvas.w, GameCanvas.h, 0, 0);
							}
							else
							{
								g.fillRect(0, 0, GameCanvas.w, GameCanvas.h);
							}
						}
						else
						{
							tPower = -1;
						}
					}
					for (int i = 0; i < Char.vItemTime.size(); i++)
					{
						((ItemTime)Char.vItemTime.elementAt(i)).paint(g, cmdMenu.x + 32 + i * 24, 55);
					}
					for (int i = 0; i < textTime.size(); i++)
					{
						((ItemTime)textTime.elementAt(i)).paintText(g, cmdMenu.x + ((Char.vItemTime.size() == 0) ? 25 : 5), ((Char.vItemTime.size() == 0) ? 45 : 90) + i * 12);
					}
					paintXoSo(g);
					if (mResources.language == 1)
					{
						long second = mSystem.currentTimeMillis() - deltaTime;
						mFont.tahoma_7b_white.drawString(g, NinjaUtil.getDate2(second), 10, GameCanvas.h - 65, 0, mFont.tahoma_7b_dark);
					}
					if (!yourNumber.Equals(string.Empty))
					{
						for (int i = 0; i < strPaint.Length; i++)
						{
							mFont.tahoma_7b_white.drawString(g, strPaint[i], 5, 85 + i * 18, 0, mFont.tahoma_7b_dark);
						}
					}
				}
				int num12 = 0;
				int num13 = GameCanvas.hw;
				if (num13 > 200)
				{
					num13 = 200;
				}
				paintPhuBanBar(g, num12 + GameCanvas.w / 2, 0, num13);
				EffectManager.hiEffects.paintAll(g);
				if (nCT_timeBallte > mSystem.currentTimeMillis() && TileMap.mapID == 170 && isPaint_CT && nCT_nBoyBaller / 2 > 0)
				{
					try
					{
						paint_CT(g, num12 + GameCanvas.w / 2, 0, num13);
					}
					catch (Exception)
					{
					}
				}
				if (TileMap.mapID == 172)
				{
					string text = mResources.WAIT + "  " + nUSER_CT + "/" + nUSER_MAX_CT;
					mFont.tahoma_7b_dark.drawString(g, mResources.WAIT + "  " + nUSER_CT + "/" + nUSER_MAX_CT, GameCanvas.w - 10, 40, 1);
				}
			}

	private void paintXoSo(mGraphics g)
			{
				if (tShow != 0)
				{
					string text = string.Empty;
					for (int i = 0; i < winnumber.Length; i++)
					{
						text = text + randomNumber[i] + " ";
					}
					PopUp.paintPopUp(g, 20, 45, 95, 35, 16777215, isButton: false);
					mFont.tahoma_7b_dark.drawString(g, mResources.kquaVongQuay, 68, 50, 2);
					mFont.tahoma_7b_dark.drawString(g, text + string.Empty, 68, 65, 2);
				}
			}

	private void paintWaypointArrow(mGraphics g)
			{
				int num = 10;
				Task taskMaint = Char.myCharz().taskMaint;
				if (taskMaint != null && taskMaint.taskId == 0 && ((taskMaint.index != 1 && taskMaint.index < 6) || taskMaint.index == 0))
				{
					return;
				}
				for (int i = 0; i < TileMap.vGo.size(); i++)
				{
					Waypoint waypoint = (Waypoint)TileMap.vGo.elementAt(i);
					if (waypoint.minY == 0 || waypoint.maxY >= TileMap.pxh - 24)
					{
						if (waypoint.maxY <= TileMap.pxh / 2)
						{
							int x = waypoint.minX + (waypoint.maxX - waypoint.minX) / 2;
							int y = waypoint.minY + (waypoint.maxY - waypoint.minY) / 2 + runArrow;
							if (GameCanvas.isTouch)
							{
								y = waypoint.maxY + (waypoint.maxY - waypoint.minY) + runArrow + num;
							}
							g.drawRegion(arrow, 0, 0, 13, 16, 6, x, y, StaticObj.VCENTER_HCENTER);
						}
						else if (waypoint.minY >= TileMap.pxh / 2)
						{
							g.drawRegion(arrow, 0, 0, 13, 16, 4, waypoint.minX + (waypoint.maxX - waypoint.minX) / 2, waypoint.minY - 12 - runArrow, StaticObj.VCENTER_HCENTER);
						}
					}
					else if (waypoint.minX >= 0 && waypoint.minX < 24)
					{
						if (!GameCanvas.isTouch)
						{
							g.drawRegion(arrow, 0, 0, 13, 16, 2, waypoint.maxX + 12 + runArrow, waypoint.maxY - 12, StaticObj.VCENTER_HCENTER);
						}
						else
						{
							g.drawRegion(arrow, 0, 0, 13, 16, 2, waypoint.maxX + 12 + runArrow, waypoint.maxY - 32, StaticObj.VCENTER_HCENTER);
						}
					}
					else if (waypoint.minX <= TileMap.tmw * 24 && waypoint.minX >= TileMap.tmw * 24 - 48)
					{
						if (!GameCanvas.isTouch)
						{
							g.drawRegion(arrow, 0, 0, 13, 16, 0, waypoint.minX - 12 - runArrow, waypoint.maxY - 12, StaticObj.VCENTER_HCENTER);
						}
						else
						{
							g.drawRegion(arrow, 0, 0, 13, 16, 0, waypoint.minX - 12 - runArrow, waypoint.maxY - 32, StaticObj.VCENTER_HCENTER);
						}
					}
					else
					{
						g.drawRegion(arrow, 0, 0, 13, 16, 4, waypoint.minX + (waypoint.maxX - waypoint.minX) / 2, waypoint.maxY - 48 - runArrow, StaticObj.VCENTER_HCENTER);
					}
				}
			}

	private void paintArrowPointToNPC(mGraphics g)
			{
				try
				{
					if (ChatPopup.currChatPopup != null)
					{
						return;
					}
					int num = getTaskNpcId();
					if (num == -1)
					{
						return;
					}
					Npc npc = null;
					for (int i = 0; i < vNpc.size(); i++)
					{
						Npc npc2 = (Npc)vNpc.elementAt(i);
						if (npc2.template.npcTemplateId == num)
						{
							if (npc == null)
							{
								npc = npc2;
							}
							else if (Res.abs(npc2.cx - Char.myCharz().cx) < Res.abs(npc.cx - Char.myCharz().cx))
							{
								npc = npc2;
							}
						}
					}
					if (npc == null || npc.statusMe == 15 || (npc.cx > cmx && npc.cx < cmx + gW && npc.cy > cmy && npc.cy < cmy + gH) || GameCanvas.gameTick % 10 < 5)
					{
						return;
					}
					int num2 = npc.cx - Char.myCharz().cx;
					int num3 = npc.cy - Char.myCharz().cy;
					int x = 0;
					int y = 0;
					int arg = 0;
					if (num2 > 0 && num3 >= 0)
					{
						if (Res.abs(num2) >= Res.abs(num3))
						{
							x = gW - 10;
							y = gH / 2 + 30;
							if (GameCanvas.isTouch)
							{
								y = gH / 2 + 10;
							}
							arg = 0;
						}
						else
						{
							x = gW / 2;
							y = gH - 10;
							arg = 5;
						}
					}
					else if (num2 >= 0 && num3 < 0)
					{
						if (Res.abs(num2) >= Res.abs(num3))
						{
							x = gW - 10;
							y = gH / 2 + 30;
							if (GameCanvas.isTouch)
							{
								y = gH / 2 + 10;
							}
							arg = 0;
						}
						else
						{
							x = gW / 2;
							y = 10;
							arg = 6;
						}
					}
					if (num2 < 0 && num3 >= 0)
					{
						if (Res.abs(num2) >= Res.abs(num3))
						{
							x = 10;
							y = gH / 2 + 30;
							if (GameCanvas.isTouch)
							{
								y = gH / 2 + 10;
							}
							arg = 3;
						}
						else
						{
							x = gW / 2;
							y = gH - 10;
							arg = 5;
						}
					}
					else if (num2 <= 0 && num3 < 0)
					{
						if (Res.abs(num2) >= Res.abs(num3))
						{
							x = 10;
							y = gH / 2 + 30;
							if (GameCanvas.isTouch)
							{
								y = gH / 2 + 10;
							}
							arg = 3;
						}
						else
						{
							x = gW / 2;
							y = 10;
							arg = 6;
						}
					}
					resetTranslate(g);
					g.drawRegion(arrow, 0, 0, 13, 16, arg, x, y, StaticObj.VCENTER_HCENTER);
				}
				catch (Exception ex)
				{
					Cout.LogError("Loi ham arrow to npc: " + ex.ToString());
				}
			}

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
						if (gamePad.isSmallGamePad)
						{
							if (isAnalog != 1)
							{
								g.setColor(9670800);
								g.fillRect(xHP + 9, yHP + 10 + 10, 22, 20);
								g.setColor(16777215);
								g.fillRect(xHP + 9, yHP + 10 + ((num3 != 0) ? (20 - num3) : 0) + 10, 22, (num3 == 0) ? 20 : num3);
								g.drawImage((mScreen.keyTouch != 10) ? imgHP1 : imgHP2, xHP, yHP + 10, 0);
								mFont.tahoma_7_red.drawString(g, string.Empty + hpPotion, xHP + 20, yHP + 15 + 10, 2);
								if (isPickNgocRong)
								{
									g.drawImage((mScreen.keyTouch != 14) ? imgNR1 : imgNR2, xHP + 5, yHP - 6 - 40 + 10, 0);
								}
								else if (isudungCapsun4)
								{
									g.drawImage((mScreen.keyTouch != 14) ? imgNutF : imgNut, xHP + 5, yHP - 6 - 40 + 10, 0);
									SmallImage.drawSmallImage(g, 1088, xHP - 7 + 5, yHP - 6 - 40 - 7 + 10, 0, 0);
								}
								else if (isudungCapsun3)
								{
									g.drawImage((mScreen.keyTouch != 14) ? imgNutF : imgNut, xHP + 5, yHP - 6 - 40 + 10, 0);
									SmallImage.drawSmallImage(g, 1087, xHP - 7 + 5, yHP - 6 - 40 - 7 + 10, 0, 0);
								}
							}
							else if (isAnalog == 1)
							{
								int num4 = 10;
								g.drawImage((mScreen.keyTouch != 10) ? imgSkill : imgSkill2, xSkill + xHP - 1, yHP - 1 + num4, 0);
								SmallImage.drawSmallImage(g, 542, xSkill + xHP + 3, yHP + 3 + num4, 0, 0);
								mFont.number_gray.drawString(g, string.Empty + hpPotion, xSkill + xHP + 22, yHP + 13 + num4, 1);
								if (num2 < 10000)
								{
									g.setColor(2721889);
									num3 = (int)(num2 * 20 / 10000);
									g.fillRect(xSkill + xHP + 3, yHP + 3 + num3 + num4, 20, 20 - num3);
								}
								if (isPickNgocRong)
								{
									g.drawImage((mScreen.keyTouch != 14) ? imgNR3 : imgNR4, xHP + 20 + 5, yHP + 20 - 6 - 40 + 10, mGraphics.HCENTER | mGraphics.VCENTER);
								}
								else if (isudungCapsun4)
								{
									g.drawImage((mScreen.keyTouch != 14) ? imgNut : imgNutF, xHP + 20 + 5, yHP + 20 - 6 - 40 + 10, mGraphics.HCENTER | mGraphics.VCENTER);
									SmallImage.drawSmallImage(g, 1088, xHP + 20 - 7 + 5, yHP + 20 - 6 - 40 - 7 + 10, 0, 0);
								}
								else if (isudungCapsun3)
								{
									g.drawImage((mScreen.keyTouch != 14) ? imgNut : imgNutF, xHP + 20 + 5, yHP + 20 - 6 - 40 + 10, mGraphics.HCENTER | mGraphics.VCENTER);
									SmallImage.drawSmallImage(g, 1087, xHP + 20 - 7 + 5, yHP + 20 - 6 - 40 - 7 + 10, 0, 0);
								}
							}
						}
						else if (isAnalog != 1)
						{
							g.setColor(9670800);
							g.fillRect(xHP + 9, yHP + 10 - 6, 22, 20);
							g.setColor(16777215);
							g.fillRect(xHP + 9, yHP + 10 + ((num3 != 0) ? (20 - num3) : 0) - 6, 22, (num3 == 0) ? 20 : num3);
							g.drawImage((mScreen.keyTouch != 10) ? imgHP1 : imgHP2, xHP, yHP - 6, 0);
							mFont.tahoma_7_red.drawString(g, string.Empty + hpPotion, xHP + 20, yHP + 15 - 6, 2);
							if (isPickNgocRong)
							{
								g.drawImage((mScreen.keyTouch != 14) ? imgNR1 : imgNR2, xHP, yHP - 6 - 40, 0);
							}
							else if (isudungCapsun4)
							{
								g.drawImage((mScreen.keyTouch != 14) ? imgNut : imgNutF, xHP + 20, yHP + 20 - 6 - 40, mGraphics.HCENTER | mGraphics.VCENTER);
								SmallImage.drawSmallImage(g, 1088, xHP + 20 - 7, yHP + 20 - 6 - 40 - 7, 0, 0);
							}
							else if (isudungCapsun3)
							{
								g.drawImage((mScreen.keyTouch != 14) ? imgNut : imgNutF, xHP + 20, yHP + 20 - 6 - 40, mGraphics.HCENTER | mGraphics.VCENTER);
								SmallImage.drawSmallImage(g, 1087, xHP + 20 - 7, yHP + 20 - 6 - 40 - 7, 0, 0);
							}
						}
						else
						{
							g.setColor(9670800);
							g.fillRect(xHP + 10, yHP + 10 - 6 + 10, 20, 18);
							g.setColor(16777215);
							g.fillRect(xHP + 10, yHP + 10 + ((num3 != 0) ? (20 - num3) : 0) - 6 + 10, 20, (num3 == 0) ? 18 : num3);
							g.drawImage((mScreen.keyTouch != 10) ? imgHP3 : imgHP4, xHP + 20, yHP + 20 - 6 + 10, mGraphics.HCENTER | mGraphics.VCENTER);
							mFont.tahoma_7_red.drawString(g, string.Empty + hpPotion, xHP + 20, yHP + 15 - 6 + 10, 2);
							if (isPickNgocRong)
							{
								g.drawImage((mScreen.keyTouch != 14) ? imgNR3 : imgNR4, xHP + 20 + 5, yHP + 20 - 6 - 40 + 10, mGraphics.HCENTER | mGraphics.VCENTER);
							}
							else if (isudungCapsun4)
							{
								g.drawImage((mScreen.keyTouch != 14) ? imgNut : imgNutF, xHP + 20 + 5, yHP + 20 - 6 - 40 + 10, mGraphics.HCENTER | mGraphics.VCENTER);
								SmallImage.drawSmallImage(g, 1088, xHP + 20 - 7 + 5, yHP + 20 - 6 - 40 - 7 + 10, 0, 0);
							}
							else if (isudungCapsun3)
							{
								g.drawImage((mScreen.keyTouch != 14) ? imgNut : imgNutF, xHP + 20 + 5, yHP + 20 - 6 - 40 + 10, mGraphics.HCENTER | mGraphics.VCENTER);
								SmallImage.drawSmallImage(g, 1087, xHP + 20 - 7 + 5, yHP + 20 - 6 - 40 - 7 + 10, 0, 0);
							}
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

	public void paint_CT(mGraphics g, int x, int y, int w)
			{
				w = 194;
				w = 182;
				w = 170;
				int num = 66;
				int num2 = 11;
				if (x > GameCanvas.w - w / 2)
				{
					x = GameCanvas.w - w / 2;
				}
				if (x < mGraphics.getImageWidth(imgKhung) + w / 2 + 10)
				{
					x = mGraphics.getImageWidth(imgKhung) + w / 2 + 10;
				}
				int frameHeight = fra_PVE_Bar_0.frameHeight;
				int num3 = y + frameHeight + mGraphics.getImageHeight(imgBall) / 2 + 2;
				int frameWidth = fra_PVE_Bar_1.frameWidth;
				int num4 = w / 2 - frameWidth / 2;
				int num5 = x - w / 2 + 3;
				int num6 = x + frameWidth / 2;
				int num7 = y + 3;
				int num8 = num4 - fra_PVE_Bar_0.frameWidth;
				int num9 = num8 / fra_PVE_Bar_0.frameWidth;
				if (num8 % fra_PVE_Bar_0.frameWidth > 0)
				{
					num9++;
				}
				for (int i = 0; i < num9; i++)
				{
					if (i < num9 - 1)
					{
						g.drawRegion(img_ct_bar_0, 0, 15, mGraphics.getImageWidth(img_ct_bar_0), 15, 2, num5 + fra_PVE_Bar_0.frameWidth + i * fra_PVE_Bar_0.frameWidth, num7, mGraphics.TOP | mGraphics.LEFT, isClip: true);
					}
					else
					{
						g.drawRegion(img_ct_bar_0, 0, 15, mGraphics.getImageWidth(img_ct_bar_0), 15, 2, num5 + num8, num7, mGraphics.TOP | mGraphics.LEFT, isClip: true);
					}
					if (i < num9 - 1)
					{
						g.drawRegion(img_ct_bar_0, 0, 15, mGraphics.getImageWidth(img_ct_bar_0), 15, 2, num6 + i * fra_PVE_Bar_0.frameWidth, num7, mGraphics.TOP | mGraphics.LEFT, isClip: true);
					}
					else
					{
						g.drawRegion(img_ct_bar_0, 0, 15, mGraphics.getImageWidth(img_ct_bar_0), 15, 2, num6 + num8 - fra_PVE_Bar_0.frameWidth, num7, mGraphics.TOP | mGraphics.LEFT, isClip: true);
					}
				}
				fra_PVE_Bar_0.drawFrame(0, num5, num7, 2, 0, g);
				fra_PVE_Bar_0.drawFrame(0, num6 + num8, num7, 0, 0, g);
				int num10 = nCT_TeamA * 100 / (nCT_nBoyBaller / 2) * num / 100;
				if (num10 > 0)
				{
					if (num10 < 6)
					{
						num10 = 6;
					}
					g.setClip(num5, num7, num10, 15);
				}
				if (nCT_TeamA > 0)
				{
					for (int j = 0; j < num2; j++)
					{
						if (j == 0)
						{
							g.drawRegion(img_ct_bar_0, 0, 60, mGraphics.getImageWidth(img_ct_bar_0), 15, 2, num5, num7, mGraphics.TOP | mGraphics.LEFT, isClip: true);
						}
						else
						{
							g.drawRegion(img_ct_bar_0, 0, 75, mGraphics.getImageWidth(img_ct_bar_0), 15, 2, num5 + j * 6, num7, mGraphics.TOP | mGraphics.LEFT, isClip: true);
						}
					}
				}
				GameCanvas.resetTrans(g);
				int num11 = nCT_TeamB * 100 / (nCT_nBoyBaller / 2) * num / 100;
				if (num - (num - num11) > 0)
				{
					if (num11 < 6)
					{
						num11 = 6;
					}
					g.setClip(num6 + num - num11, num7, num - (num - num11), 15);
				}
				if (nCT_TeamB > 0)
				{
					for (int k = 0; k < num2; k++)
					{
						if (k == 0)
						{
							g.drawRegion(img_ct_bar_0, 0, 30, mGraphics.getImageWidth(img_ct_bar_0), 15, 0, num6 + num8, num7, mGraphics.TOP | mGraphics.LEFT, isClip: true);
						}
						else
						{
							g.drawRegion(img_ct_bar_0, 0, 45, mGraphics.getImageWidth(img_ct_bar_0), 15, 0, num6 + num8 - k * 6, num7, mGraphics.TOP | mGraphics.LEFT, isClip: true);
						}
					}
				}
				GameCanvas.resetTrans(g);
				fra_PVE_Bar_1.drawFrame(0, x - frameWidth / 2 + 1, y, 0, 0, g);
				string st = NinjaUtil.getTime((int)((nCT_timeBallte - mSystem.currentTimeMillis()) / 1000)) + string.Empty;
				mFont.tahoma_7b_yellow.drawString(g, st, num5 + w / 2 - 2, y + 5, 2);
				mFont.tahoma_7_grey.drawString(g, "Tầng " + nCT_floor, num5 + w / 2 - 3, y + fra_PVE_Bar_1.frameHeight, mFont.CENTER);
				int width = mFont.tahoma_7b_red.getWidth(nCT_TeamA + string.Empty);
				mFont.tahoma_7b_blue.drawString(g, nCT_TeamA + string.Empty, x - frameWidth / 2 - width, num7 + fra_PVE_Bar_1.frameHeight, 0);
				SmallImage.drawSmallImage(g, 2325, x - frameWidth / 2 - width - 15, num7 + fra_PVE_Bar_1.frameHeight, 2, mGraphics.TOP | mGraphics.LEFT);
				width = mFont.tahoma_7b_red.getWidth(nCT_TeamB + string.Empty);
				mFont.tahoma_7b_red.drawString(g, nCT_TeamB + string.Empty, x + frameWidth / 2, num7 + fra_PVE_Bar_1.frameHeight, 0);
				SmallImage.drawSmallImage(g, 2323, x + frameWidth / 2 + width + 3, num7 + fra_PVE_Bar_1.frameHeight, 0, mGraphics.TOP | mGraphics.LEFT);
				paint_board_CT(g, GameCanvas.w - mFont.tahoma_7b_dark.getWidth("#01 AAAAAAAAAA"), 40);
				GameCanvas.resetTrans(g);
			}

}
