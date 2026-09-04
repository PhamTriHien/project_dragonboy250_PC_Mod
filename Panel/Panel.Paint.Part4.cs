using System;
using System.Collections.Generic;
using Assets.src.g;
using UnityEngine;
public partial class Panel : IActionListener, IChatable
{
	private void paintFriend(mGraphics g)
			{
				g.setClip(xScroll, yScroll, wScroll, hScroll);
				g.translate(0, -cmy);
				g.setColor(0);
				if (currentListLength == 0)
				{
					mFont.tahoma_7_green2.drawString(g, mResources.no_friend, xScroll + wScroll / 2, yScroll + hScroll / 2 - mFont.tahoma_7.getHeight() / 2, 2);
					return;
				}
				for (int i = 0; i < currentListLength; i++)
				{
					int num = xScroll;
					int num2 = yScroll + i * ITEM_HEIGHT;
					int num3 = 24;
					int h = ITEM_HEIGHT - 1;
					int num4 = xScroll + num3;
					int num5 = yScroll + i * ITEM_HEIGHT;
					int num6 = wScroll - num3;
					int h2 = ITEM_HEIGHT - 1;
					g.setColor((i != selected) ? 15196114 : 16383818);
					g.fillRect(num4, num5, num6, h2);
					g.setColor((i != selected) ? 9993045 : 9541120);
					g.fillRect(num, num2, num3, h);
					InfoItem infoItem = (InfoItem)vFriend.elementAt(i);
					if (infoItem.charInfo.headICON != -1)
					{
						SmallImage.drawSmallImage(g, infoItem.charInfo.headICON, num, num2, 0, 0);
					}
					else
					{
						Part part = GameScr.parts[infoItem.charInfo.head];
						SmallImage.drawSmallImage(g, part.pi[Char.CharInfo[0][0][0]].id, num + part.pi[Char.CharInfo[0][0][0]].dx, num2 + 3 + part.pi[Char.CharInfo[0][0][0]].dy, 0, 0);
					}
					g.setClip(xScroll, yScroll + cmy, wScroll, hScroll);
					if (infoItem.isOnline)
					{
						mFont.tahoma_7b_green.drawString(g, infoItem.charInfo.cName, num4 + 5, num5, 0);
						mFont.tahoma_7_blue.drawString(g, infoItem.s, num4 + 5, num5 + 11, 0);
					}
					else
					{
						mFont.tahoma_7_grey.drawString(g, infoItem.charInfo.cName, num4 + 5, num5, 0);
						mFont.tahoma_7_grey.drawString(g, infoItem.s, num4 + 5, num5 + 11, 0);
					}
				}
				paintScrollArrow(g);
			}
	public void paintPlayerMenu(mGraphics g)
			{
				g.setClip(xScroll, yScroll, wScroll, hScroll);
				g.translate(0, -cmy);
				for (int i = 0; i < vPlayerMenu.size(); i++)
				{
					int x = xScroll;
					int num = yScroll + i * ITEM_HEIGHT;
					int num2 = wScroll - 1;
					int h = ITEM_HEIGHT - 1;
					if (num - cmy <= yScroll + hScroll && num - cmy >= yScroll - ITEM_HEIGHT)
					{
						Command command = (Command)vPlayerMenu.elementAt(i);
						g.setColor((i != selected) ? 15196114 : 16383818);
						g.fillRect(x, num, num2, h);
						if (command.caption2.Equals(string.Empty))
						{
							mFont.tahoma_7b_dark.drawString(g, command.caption, xScroll + wScroll / 2, num + 6, mFont.CENTER);
							continue;
						}
						mFont.tahoma_7b_dark.drawString(g, command.caption, xScroll + wScroll / 2, num + 1, mFont.CENTER);
						mFont.tahoma_7b_dark.drawString(g, command.caption2, xScroll + wScroll / 2, num + 11, mFont.CENTER);
					}
				}
				paintScrollArrow(g);
			}
	private void paintArchivement(mGraphics g)
			{
				g.setClip(xScroll, yScroll, wScroll, hScroll);
				g.translate(0, -cmy);
				g.setColor(0);
				if (currentListLength == 0)
				{
					mFont.tahoma_7_green2.drawString(g, mResources.no_mission, xScroll + wScroll / 2, yScroll + hScroll / 2 - mFont.tahoma_7.getHeight() / 2, 2);
				}
				else
				{
					if (Char.myCharz().arrArchive == null || Char.myCharz().arrArchive.Length != currentListLength)
					{
						return;
					}
					for (int i = 0; i < currentListLength; i++)
					{
						int num = xScroll;
						int num2 = yScroll + i * ITEM_HEIGHT;
						int num3 = wScroll;
						int num4 = ITEM_HEIGHT - 1;
						Archivement archivement = Char.myCharz().arrArchive[i];
						g.setColor((i != selected || ((archivement.isRecieve || archivement.isFinish) && (!archivement.isRecieve || !archivement.isFinish))) ? 15196114 : 16383818);
						g.fillRect(num, num2, num3, num4);
						if (archivement == null)
						{
							continue;
						}
						if (!archivement.isFinish)
						{
							mFont.tahoma_7.drawString(g, archivement.info1, num + 5, num2, 0);
							mFont.tahoma_7_green.drawString(g, archivement.money + " " + mResources.RUBY, num + num3 - 5, num2, mFont.RIGHT);
							mFont.tahoma_7_red.drawString(g, archivement.info2, num + 5, num2 + 11, 0);
						}
						else if (archivement.isFinish && !archivement.isRecieve)
						{
							mFont.tahoma_7.drawString(g, archivement.info1, num + 5, num2, 0);
							mFont.tahoma_7_blue.drawString(g, mResources.reward_mission + archivement.money + " " + mResources.RUBY, num + 5, num2 + 11, 0);
							if (i == selected)
							{
								mFont.tahoma_7b_green2.drawString(g, mResources.receive_upper, num + num3 - 20, num2 + 6, mFont.CENTER);
								mFont.tahoma_7b_dark.drawString(g, mResources.receive_upper, num + num3 - 20, num2 + 6, mFont.CENTER);
							}
							else
							{
								g.drawImage(GameScr.imgLbtn2, num + num3 - 20, num2 + num4 / 2, StaticObj.VCENTER_HCENTER);
								mFont.tahoma_7b_dark.drawString(g, mResources.receive_upper, num + num3 - 20, num2 + 6, mFont.CENTER);
							}
						}
						else if (archivement.isFinish && archivement.isRecieve)
						{
							mFont.tahoma_7_green.drawString(g, archivement.info1, num + 5, num2, 0);
							mFont.tahoma_7_green.drawString(g, archivement.info2, num + 5, num2 + 11, 0);
						}
					}
					paintScrollArrow(g);
				}
			}
	private void paintTab(mGraphics g)
			{
				if (type == 23 || type == 24)
				{
					g.setColor(13524492);
					g.fillRect(X + 1, 78, W - 2, 1);
					mFont.tahoma_7b_dark.drawString(g, mResources.gameInfo, xScroll + wScroll / 2, 59, mFont.CENTER);
					return;
				}
				if (type == 20)
				{
					g.setColor(13524492);
					g.fillRect(X + 1, 78, W - 2, 1);
					mFont.tahoma_7b_dark.drawString(g, mResources.account, xScroll + wScroll / 2, 59, mFont.CENTER);
					return;
				}
				if (type == 22)
				{
					g.setColor(13524492);
					g.fillRect(X + 1, 78, W - 2, 1);
					mFont.tahoma_7b_dark.drawString(g, mResources.autoFunction, xScroll + wScroll / 2, 59, mFont.CENTER);
					return;
				}
				if (type == 19)
				{
					g.setColor(13524492);
					g.fillRect(X + 1, 78, W - 2, 1);
					mFont.tahoma_7b_dark.drawString(g, mResources.option, xScroll + wScroll / 2, 59, mFont.CENTER);
					return;
				}
				if (type == 18)
				{
					g.setColor(13524492);
					g.fillRect(X + 1, 78, W - 2, 1);
					mFont.tahoma_7b_dark.drawString(g, mResources.change_flag, xScroll + wScroll / 2, 59, mFont.CENTER);
					return;
				}
				if (type == 13 && Equals(GameCanvas.panel2))
				{
					g.setColor(13524492);
					g.fillRect(X + 1, 78, W - 2, 1);
					mFont.tahoma_7b_dark.drawString(g, mResources.item_receive2, xScroll + wScroll / 2, 59, mFont.CENTER);
					return;
				}
				if (type == 12 && GameCanvas.panel2 != null)
				{
					g.setColor(13524492);
					g.fillRect(X + 1, 78, W - 2, 1);
					mFont.tahoma_7b_dark.drawString(g, mResources.UPGRADE, xScroll + wScroll / 2, 59, mFont.CENTER);
					return;
				}
				if (type == 11)
				{
					g.setColor(13524492);
					g.fillRect(X + 1, 78, W - 2, 1);
					mFont.tahoma_7b_dark.drawString(g, mResources.friend, xScroll + wScroll / 2, 59, mFont.CENTER);
					return;
				}
				if (type == 16)
				{
					g.setColor(13524492);
					g.fillRect(X + 1, 78, W - 2, 1);
					mFont.tahoma_7b_dark.drawString(g, mResources.enemy, xScroll + wScroll / 2, 59, mFont.CENTER);
					return;
				}
				if (type == 15)
				{
					g.setColor(13524492);
					g.fillRect(X + 1, 78, W - 2, 1);
					mFont.tahoma_7b_dark.drawString(g, topName, xScroll + wScroll / 2, 59, mFont.CENTER);
					return;
				}
				if (type == 2 && GameCanvas.panel2 != null)
				{
					g.setColor(13524492);
					g.fillRect(X + 1, 78, W - 2, 1);
					mFont.tahoma_7b_dark.drawString(g, mResources.chest, xScroll + wScroll / 2, 59, mFont.CENTER);
					return;
				}
				if (type == 9)
				{
					g.setColor(13524492);
					g.fillRect(X + 1, 78, W - 2, 1);
					mFont.tahoma_7b_dark.drawString(g, mResources.achievement_mission, xScroll + wScroll / 2, 59, mFont.CENTER);
					return;
				}
				if (type == 3)
				{
					g.setColor(13524492);
					g.fillRect(X + 1, 78, W - 2, 1);
					mFont.tahoma_7b_dark.drawString(g, mResources.select_zone, startTabPos + TAB_W / 2, 59, mFont.CENTER);
					return;
				}
				if (type == 14)
				{
					g.setColor(13524492);
					g.fillRect(X + 1, 78, W - 2, 1);
					mFont.tahoma_7b_dark.drawString(g, mResources.select_map, startTabPos + TAB_W / 2, 59, mFont.CENTER);
					return;
				}
				if (type == 4)
				{
					mFont.tahoma_7b_dark.drawString(g, mResources.map, startTabPos + TAB_W / 2, 59, mFont.CENTER);
					g.setColor(13524492);
					g.fillRect(X + 1, 78, W - 2, 1);
					return;
				}
				if (type == 7)
				{
					mFont.tahoma_7b_dark.drawString(g, mResources.trangbi, startTabPos + TAB_W / 2, 59, mFont.CENTER);
					g.setColor(13524492);
					g.fillRect(X + 1, 78, W - 2, 1);
					return;
				}
				if (type == 17)
				{
					mFont.tahoma_7b_dark.drawString(g, mResources.kigui, startTabPos + TAB_W / 2, 59, mFont.CENTER);
					g.setColor(13524492);
					g.fillRect(X + 1, 78, W - 2, 1);
					return;
				}
				if (type == 8)
				{
					mFont.tahoma_7b_dark.drawString(g, mResources.msg, startTabPos + TAB_W / 2, 59, mFont.CENTER);
					g.setColor(13524492);
					g.fillRect(X + 1, 78, W - 2, 1);
					return;
				}
				if (type == 10)
				{
					mFont.tahoma_7b_dark.drawString(g, mResources.wat_do_u_want, startTabPos + TAB_W / 2, 59, mFont.CENTER);
					g.setColor(13524492);
					g.fillRect(X + 1, 78, W - 2, 1);
					return;
				}
				if (currentTabIndex == 3 && mainTabName.Length != 4)
				{
					g.translate(-cmx, 0);
				}
				for (int i = 0; i < currentTabName.Length; i++)
				{
					g.setColor((i != currentTabIndex) ? 16773296 : 6805896);
					PopUp.paintPopUp(g, startTabPos + i * TAB_W, 52, TAB_W - 1, 25, (i == currentTabIndex) ? 1 : 0, isButton: true);
					if (i == keyTouchTab)
					{
						g.drawImage(ItemMap.imageFlare, startTabPos + i * TAB_W + TAB_W / 2, 62, 3);
					}
					mFont mFont2 = ((i != currentTabIndex) ? mFont.tahoma_7_grey : mFont.tahoma_7_green2);
					if (!currentTabName[i][1].Equals(string.Empty))
					{
						mFont2.drawString(g, currentTabName[i][0], startTabPos + i * TAB_W + TAB_W / 2, 53, mFont.CENTER);
						mFont2.drawString(g, currentTabName[i][1], startTabPos + i * TAB_W + TAB_W / 2, 64, mFont.CENTER);
					}
					else
					{
						mFont2.drawString(g, currentTabName[i][0], startTabPos + i * TAB_W + TAB_W / 2, 59, mFont.CENTER);
					}
					if (type == 0 && currentTabName.Length == 5 && GameScr.isNewClanMessage && GameCanvas.gameTick % 4 == 0)
					{
						g.drawImage(ItemMap.imageFlare, startTabPos + 3 * TAB_W + TAB_W / 2, 77, mGraphics.BOTTOM | mGraphics.HCENTER);
					}
				}
				g.setColor(13524492);
				g.fillRect(1, 78, W - 2, 1);
			}
	private void paintBottomMoneyInfo(mGraphics g)
			{
				if (type != 13 || (currentTabIndex != 2 && !Equals(GameCanvas.panel2)))
				{
					g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
					g.setColor(11837316);
					g.fillRect(X + 1, H - 15, W - 2, 14);
					g.setColor(13524492);
					g.fillRect(X + 1, H - 15, W - 2, 1);
					g.drawImage(imgXu, X + 11, H - 7, 3);
					g.drawImage(imgLuong, X + 75, H - 8, 3);
					mFont.tahoma_7_yellow.drawString(g, Char.myCharz().xuStr + string.Empty, X + 24, H - 13, mFont.LEFT, mFont.tahoma_7_grey);
					mFont.tahoma_7_yellow.drawString(g, Char.myCharz().luongStr + string.Empty, X + 85, H - 13, mFont.LEFT, mFont.tahoma_7_grey);
					g.drawImage(imgLuongKhoa, X + 130, H - 8, 3);
					mFont.tahoma_7_yellow.drawString(g, Char.myCharz().luongKhoaStr + string.Empty, X + 140, H - 13, mFont.LEFT, mFont.tahoma_7_grey);
				}
			}
	private void paintToolInfo(mGraphics g)
			{
				mFont.tahoma_7b_white.drawString(g, mResources.dragon_ball + " " + GameMidlet.VERSION, 60, 4, mFont.LEFT, mFont.tahoma_7b_dark);
				mFont.tahoma_7_yellow.drawString(g, mResources.character + ": " + Char.myCharz().cName, 60, 16, mFont.LEFT, mFont.tahoma_7_grey);
				string text = ((!GameCanvas.loginScr.tfUser.getText().Equals(string.Empty)) ? GameCanvas.loginScr.tfUser.getText() : mResources.not_register_yet);
				string svName = (ServerListScreen.nameServer != null && ServerListScreen.ipSelect >= 0 && ServerListScreen.ipSelect < ServerListScreen.nameServer.Length) ? ServerListScreen.nameServer[ServerListScreen.ipSelect] : string.Empty;
				mFont.tahoma_7_yellow.drawString(g, mResources.account_server + " " + svName + ": " + text, 60, 27, mFont.LEFT, mFont.tahoma_7_grey);
			}
	private void paintGiaoDichInfo(mGraphics g)
			{
				mFont.tahoma_7_yellow.drawString(g, mResources.select_item, 60, 4, mFont.LEFT, mFont.tahoma_7_grey);
				mFont.tahoma_7_yellow.drawString(g, mResources.lock_trade, 60, 16, mFont.LEFT, mFont.tahoma_7_grey);
				mFont.tahoma_7_yellow.drawString(g, mResources.wait_opp_lock_trade, 60, 27, mFont.LEFT, mFont.tahoma_7_grey);
				mFont.tahoma_7_yellow.drawString(g, mResources.press_done, 60, 38, mFont.LEFT, mFont.tahoma_7_grey);
			}
	private void paintMyInfo(mGraphics g)
			{
				paintCharInfo(g, Char.myCharz());
			}

}
