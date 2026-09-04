using System;
using System.Collections.Generic;
using Assets.src.g;
using UnityEngine;
public partial class Panel : IActionListener, IChatable
{
	private void paintCharInfo(mGraphics g, Char c)
			{
				mFont.tahoma_7b_white.drawString(g, ((GameScr.isNewMember == 1) ? "       " : string.Empty) + c.cName, X + 60, 4, mFont.LEFT, mFont.tahoma_7b_dark);
				if (GameScr.isNewMember == 1)
				{
					SmallImage.drawSmallImage(g, 5427, X + 55, 4, 0, 0);
				}
				if (c.cMaxStamina > 0)
				{
					mFont.tahoma_7_yellow.drawString(g, mResources.vitality, X + 60, 16, mFont.LEFT, mFont.tahoma_7_grey);
					g.drawImage(GameScr.imgMPLost, X + 95, 19, 0);
					int num = c.cStamina * mGraphics.getImageWidth(GameScr.imgMP) / c.cMaxStamina;
					g.setClip(95, X + 19, num, 20);
					g.drawImage(GameScr.imgMP, X + 95, 19, 0);
				}
				g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
				if (c.cPower > 0)
				{
					mFont.tahoma_7_yellow.drawString(g, (!c.me) ? c.currStrLevel : c.getStrLevel(), X + 60, 27, mFont.LEFT, mFont.tahoma_7_grey);
				}
				mFont.tahoma_7_yellow.drawString(g, mResources.power + ": " + NinjaUtil.getMoneys(c.cPower), X + 60, 38, mFont.LEFT, mFont.tahoma_7_grey);
			}
	private void paintCharInfo(mGraphics g, Char c, int x, int y)
			{
				mFont.tahoma_7b_white.drawString(g, ((GameScr.isNewMember == 1) ? "       " : string.Empty) + c.cName, x + 60, y + 4, mFont.LEFT, mFont.tahoma_7b_dark);
				if (GameScr.isNewMember == 1)
				{
					SmallImage.drawSmallImage(g, 5427, x + 55, y + 4, 0, 0);
				}
				if (c.cMaxStamina > 0)
				{
					mFont.tahoma_7_yellow.drawString(g, mResources.vitality, x + 60, y + 16, mFont.LEFT, mFont.tahoma_7_grey);
					g.drawImage(GameScr.imgMPLost, x + 95, y + 19, 0);
					int num = c.cStamina * mGraphics.getImageWidth(GameScr.imgMP) / c.cMaxStamina;
					g.drawImage(GameScr.imgMP, x + 95, y + 19, 0);
				}
				if (c.cPower > 0)
				{
					mFont.tahoma_7_yellow.drawString(g, (!c.me) ? c.currStrLevel : c.getStrLevel(), x + 60, y + 27, mFont.LEFT, mFont.tahoma_7_grey);
				}
				mFont.tahoma_7_yellow.drawString(g, mResources.power + ": " + NinjaUtil.getMoneys(c.cPower), x + 60, y + 38, mFont.LEFT, mFont.tahoma_7_grey);
			}
	private void paintSkillInfo(mGraphics g)
			{
				mFont.tahoma_7_white.drawString(g, "Top " + Char.myCharz().rank, X + 45 + (W - 50) / 2, 2, mFont.CENTER);
				mFont.tahoma_7_yellow.drawString(g, mResources.potential_point, X + 45 + (W - 50) / 2, 14, mFont.CENTER);
				mFont.tahoma_7_white.drawString(g, string.Empty + NinjaUtil.getMoneys(Char.myCharz().cTiemNang), X + ((GameCanvas.gameTick % 20 > 10) ? (GameCanvas.gameTick % 4 / 2) : 0) + 45 + (W - 50) / 2, 26, mFont.CENTER);
				mFont.tahoma_7_yellow.drawString(g, mResources.active_point + ": " + NinjaUtil.getMoneys(Char.myCharz().cNangdong), X + ((GameCanvas.gameTick % 20 > 10) ? (GameCanvas.gameTick % 4 / 2) : 0) + 45 + (W - 50) / 2, 38, mFont.CENTER);
			}
	private void paintTopInfo(mGraphics g)
			{
				g.setClip(X + 1, Y, W - 2, yScroll - 2);
				g.setColor(9993045);
				g.fillRect(X, Y, W - 2, 50);
				switch (type)
				{
				case 13:
					if (currentTabIndex == 0 || currentTabIndex == 1)
					{
						if (Equals(GameCanvas.panel))
						{
							SmallImage.drawSmallImage(g, Char.myCharz().avatarz(), X + 25, 50, 0, 33);
							paintGiaoDichInfo(g);
						}
						if (Equals(GameCanvas.panel2) && charMenu != null)
						{
							SmallImage.drawSmallImage(g, charMenu.avatarz(), X + 25, 50, 0, 33);
							paintCharInfo(g, charMenu);
						}
					}
					if (currentTabIndex == 2 && charMenu != null)
					{
						SmallImage.drawSmallImage(g, charMenu.avatarz(), X + 25, 50, 0, 33);
						paintCharInfo(g, charMenu);
					}
					break;
				case 12:
					if (currentTabIndex == 0)
					{
						int id = 1410;
						for (int i = 0; i < GameScr.vNpc.size(); i++)
						{
							Npc npc = (Npc)GameScr.vNpc.elementAt(i);
							if (npc.template.npcTemplateId == idNPC)
							{
								id = npc.avatar;
							}
						}
						SmallImage.drawSmallImage(g, id, X + 25, 50, 0, 33);
						paintCombineInfo(g);
					}
					if (currentTabIndex == 1)
					{
						SmallImage.drawSmallImage(g, Char.myCharz().avatarz(), X + 25, 50, 0, 33);
						paintMyInfo(g);
					}
					break;
				case 11:
				case 16:
				case 23:
				case 24:
					SmallImage.drawSmallImage(g, Char.myCharz().avatarz(), X + 25, 50, 0, 33);
					paintMyInfo(g);
					break;
				case 15:
					SmallImage.drawSmallImage(g, Char.myCharz().avatarz(), X + 25, 50, 0, 33);
					paintMyInfo(g);
					break;
				case 9:
					SmallImage.drawSmallImage(g, Char.myCharz().avatarz(), X + 25, 50, 0, 33);
					paintMyInfo(g);
					break;
				case 21:
					if (currentTabIndex == 0)
					{
						Debug.LogWarning(">>>head:" + Char.myPetz().avatarz());
						SmallImage.drawSmallImage(g, Char.myPetz().avatarz(), X + 25, 50, 0, 33);
						paintPetInfo(g);
					}
					if (currentTabIndex == 1)
					{
						SmallImage.drawSmallImage(g, Char.myPetz().avatarz(), X + 25, 50, 0, 33);
						paintPetStatusInfo(g);
					}
					if (currentTabIndex == 2)
					{
						SmallImage.drawSmallImage(g, Char.myCharz().avatarz(), X + 25, 50, 0, 33);
						paintItemBodyBagInfo(g);
					}
					break;
				case 0:
					if (currentTabIndex == 0)
					{
						SmallImage.drawSmallImage(g, Char.myCharz().avatarz(), X + 25, 50, 0, 33);
						paintMyInfo(g);
					}
					if (currentTabIndex == 1)
					{
						SmallImage.drawSmallImage(g, Char.myCharz().avatarz(), X + 25, 50, 0, 33);
						if (isnewInventory)
						{
							paintCharInfo(g, Char.myCharz());
						}
						else
						{
							paintItemBodyBagInfo(g);
						}
					}
					if (currentTabIndex == 2)
					{
						SmallImage.drawSmallImage(g, Char.myCharz().avatarz(), X + 25, 50, 0, 33);
						paintSkillInfo(g);
					}
					if (currentTabIndex == 3)
					{
						if (mainTabName.Length == 5)
						{
							paintClanInfo(g);
						}
						else
						{
							SmallImage.drawSmallImage(g, Char.myCharz().avatarz(), X + 25, 50, 0, 33);
							paintToolInfo(g);
						}
					}
					if (currentTabIndex == 4)
					{
						SmallImage.drawSmallImage(g, Char.myCharz().avatarz(), X + 25, 50, 0, 33);
						paintToolInfo(g);
					}
					break;
				case 25:
					SmallImage.drawSmallImage(g, Char.myCharz().avatarz(), X + 25, 50, 0, 33);
					paintMyInfo(g);
					break;
				case 2:
					if (currentTabIndex == 0)
					{
						SmallImage.drawSmallImage(g, 526, X + 25, 50, 0, 33);
						paintItemBoxInfo(g);
					}
					if (currentTabIndex == 1)
					{
						SmallImage.drawSmallImage(g, Char.myCharz().avatarz(), X + 25, 50, 0, 33);
						paintItemBodyBagInfo(g);
					}
					break;
				case 3:
					SmallImage.drawSmallImage(g, 561, X + 25, 50, 0, 33);
					paintZoneInfo(g);
					break;
				case 1:
					if (currentTabIndex == currentTabName.Length - 1 && GameCanvas.panel2 == null)
					{
						SmallImage.drawSmallImage(g, Char.myCharz().avatarz(), X + 25, 50, 0, 33);
					}
					else if (Char.myCharz().npcFocus != null)
					{
						SmallImage.drawSmallImage(g, Char.myCharz().npcFocus.avatar, X + 25, 50, 0, 33);
					}
					paintShopInfo(g);
					break;
				case 4:
					SmallImage.drawSmallImage(g, Char.myCharz().avatarz(), X + 25, 50, 0, 33);
					paintMapInfo(g);
					break;
				case 7:
				case 17:
					SmallImage.drawSmallImage(g, Char.myCharz().avatarz(), X + 25, 50, 0, 33);
					paintMyInfo(g);
					break;
				case 8:
					SmallImage.drawSmallImage(g, Char.myCharz().avatarz(), X + 25, 50, 0, 33);
					paintMyInfo(g);
					break;
				case 10:
					if (charMenu != null)
					{
						SmallImage.drawSmallImage(g, charMenu.avatarz(), X + 25, 50, 0, 33);
						paintCharInfo(g, charMenu);
					}
					break;
				case 14:
					SmallImage.drawSmallImage(g, Char.myCharz().avatarz(), X + 25, 50, 0, 33);
					paintMapInfo(g);
					break;
				case 18:
					SmallImage.drawSmallImage(g, Char.myCharz().avatarz(), X + 25, 50, 0, 33);
					paintMyInfo(g);
					break;
				case 19:
					SmallImage.drawSmallImage(g, Char.myCharz().avatarz(), X + 25, 50, 0, 33);
					paintToolInfo(g);
					break;
				case 20:
					SmallImage.drawSmallImage(g, Char.myCharz().avatarz(), X + 25, 50, 0, 33);
					paintToolInfo(g);
					break;
				case 22:
					SmallImage.drawSmallImage(g, Char.myCharz().avatarz(), X + 25, 50, 0, 33);
					paintToolInfo(g);
					break;
				case 5:
				case 6:
					break;
				}
			}
	private void paintChatManager(mGraphics g)
			{
			}
	private void paintChatPlayer(mGraphics g)
			{
			}
	private void paintInfomation(mGraphics g)
			{
			}

}
