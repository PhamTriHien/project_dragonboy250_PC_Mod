using System;
using Assets.src.g;
public partial class GameScr : mScreen, IChatable
{
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

}
