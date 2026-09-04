using System;
using System.Collections.Generic;
using Assets.src.g;
using UnityEngine;

public partial class Panel : IActionListener, IChatable
{
	private void paintPetStatus(mGraphics g)
			{
				g.setClip(xScroll, yScroll, wScroll, hScroll);
				g.translate(0, -cmy);
				for (int i = 0; i < strStatus.Length; i++)
				{
					int x = xScroll;
					int num = yScroll + i * ITEM_HEIGHT;
					int num2 = wScroll - 1;
					int h = ITEM_HEIGHT - 1;
					if (num - cmy <= yScroll + hScroll && num - cmy >= yScroll - ITEM_HEIGHT)
					{
						g.setColor((i != selected) ? 15196114 : 16383818);
						g.fillRect(x, num, num2, h);
						mFont.tahoma_7b_dark.drawString(g, strStatus[i], xScroll + wScroll / 2, num + 6, mFont.CENTER);
					}
				}
				paintScrollArrow(g);
			}

	private void paintPetSkill()
			{
			}

	private void paintPetInventory(mGraphics g)
			{
				g.setColor(16711680);
				g.setClip(xScroll, yScroll, wScroll, hScroll);
				g.translate(0, -cmy);
				Item[] arrItemBody = Char.myPetz().arrItemBody;
				Skill[] arrPetSkill = Char.myPetz().arrPetSkill;
				for (int i = 0; i < arrItemBody.Length + arrPetSkill.Length; i++)
				{
					bool flag = i < arrItemBody.Length;
					int num = i;
					int num2 = i - arrItemBody.Length;
					int num3 = xScroll + 36;
					int num4 = yScroll + i * ITEM_HEIGHT;
					int num5 = wScroll - 36;
					int h = ITEM_HEIGHT - 1;
					int num6 = xScroll;
					int num7 = yScroll + i * ITEM_HEIGHT;
					int num8 = 34;
					int num9 = ITEM_HEIGHT - 1;
					if (num4 - cmy > yScroll + hScroll || num4 - cmy < yScroll - ITEM_HEIGHT)
					{
						continue;
					}
					Item item = ((!flag) ? null : arrItemBody[num]);
					g.setColor((i == selected) ? 16383818 : ((!flag) ? 15723751 : 15196114));
					g.fillRect(num3, num4, num5, h);
					g.setColor((i == selected) ? 9541120 : ((!flag) ? 11837316 : 9993045));
					if (item != null)
					{
						for (int j = 0; j < item.itemOption.Length; j++)
						{
							if (item.itemOption[j].optionTemplate.id == 72 && item.itemOption[j].param > 0)
							{
								sbyte color_Item_Upgrade = GetColor_Item_Upgrade(item.itemOption[j].param);
								int color_ItemBg = GetColor_ItemBg(color_Item_Upgrade);
								if (color_ItemBg != -1)
								{
									g.setColor((i != selected) ? GetColor_ItemBg(color_Item_Upgrade) : GetColor_ItemBg(color_Item_Upgrade));
								}
							}
						}
					}
					g.fillRect(num6, num7, num8, num9);
					if (item != null && item.isSelect && GameCanvas.panel.type == 12)
					{
						g.setColor((i != selected) ? 6047789 : 7040779);
						g.fillRect(num6, num7, num8, num9);
					}
					if (item != null)
					{
						string text = string.Empty;
						mFont mFont2 = mFont.tahoma_7_green2;
						if (item.itemOption != null)
						{
							for (int k = 0; k < item.itemOption.Length; k++)
							{
								if (item.itemOption[k].optionTemplate.id == 72)
								{
									text = " [+" + item.itemOption[k].param + "]";
								}
								if (item.itemOption[k].optionTemplate.id == 41)
								{
									if (item.itemOption[k].param == 1)
									{
										mFont2 = GetFont(0);
									}
									else if (item.itemOption[k].param == 2)
									{
										mFont2 = GetFont(2);
									}
									else if (item.itemOption[k].param == 3)
									{
										mFont2 = GetFont(8);
									}
									else if (item.itemOption[k].param == 4)
									{
										mFont2 = GetFont(7);
									}
								}
							}
						}
						mFont2.drawString(g, item.template.name + text, num3 + 5, num4 + 1, 0);
						string text2 = string.Empty;
						if (item.itemOption != null)
						{
							if (item.itemOption.Length > 0 && item.itemOption[0] != null && item.itemOption[0].optionTemplate.id != 102 && item.itemOption[0].optionTemplate.id != 107)
							{
								text2 += item.itemOption[0].getOptionString();
							}
							mFont mFont3 = mFont.tahoma_7_blue;
							if (item.compare < 0 && item.template.type != 5)
							{
								mFont3 = mFont.tahoma_7_red;
							}
							if (item.itemOption.Length > 1)
							{
								for (int l = 1; l < 2; l++)
								{
									if (item.itemOption[l] != null && item.itemOption[l].optionTemplate.id != 102 && item.itemOption[l].optionTemplate.id != 107)
									{
										text2 = text2 + "," + item.itemOption[l].getOptionString();
									}
								}
							}
							mFont3.drawString(g, text2, num3 + 5, num4 + 11, mFont.LEFT);
						}
						SmallImage.drawSmallImage(g, item.template.iconID, num6 + num8 / 2, num7 + num9 / 2, 0, 3);
						if (item.itemOption != null)
						{
							for (int m = 0; m < item.itemOption.Length; m++)
							{
								paintOptItem(g, item.itemOption[m].optionTemplate.id, item.itemOption[m].param, num6, num7, num8, num9);
							}
							for (int n = 0; n < item.itemOption.Length; n++)
							{
								paintOptSlotItem(g, item.itemOption[n].optionTemplate.id, item.itemOption[n].param, num6, num7, num8, num9);
							}
						}
						if (item.quantity > 1)
						{
							mFont.tahoma_7_yellow.drawString(g, string.Empty + item.quantity, num6 + num8, num7 + num9 - mFont.tahoma_7_yellow.getHeight(), 1);
						}
					}
					else if (!flag)
					{
						Skill skill = arrPetSkill[num2];
						g.drawImage(GameScr.imgSkill, num6 + num8 / 2, num7 + num9 / 2, 3);
						if (skill.template != null)
						{
							mFont.tahoma_7_blue.drawString(g, skill.template.name, num3 + 5, num4 + 1, 0);
							mFont.tahoma_7_green2.drawString(g, mResources.level + ": " + skill.point + string.Empty, num3 + 5, num4 + 11, 0);
							SmallImage.drawSmallImage(g, skill.template.iconId, num6 + num8 / 2, num7 + num9 / 2, 0, 3);
						}
						else
						{
							mFont.tahoma_7_green2.drawString(g, skill.moreInfo, num3 + 5, num4 + 5, 0);
							SmallImage.drawSmallImage(g, GameScr.efs[98].arrEfInfo[0].idImg, num6 + num8 / 2, num7 + num9 / 2, 0, 3);
						}
					}
				}
				paintScrollArrow(g);
			}

	private void paintMapTrans(mGraphics g)
			{
				g.setColor(16711680);
				g.setClip(xScroll, yScroll, wScroll, hScroll);
				g.translate(0, -cmy);
				for (int i = 0; i < mapNames.Length; i++)
				{
					int num = xScroll + 36;
					int num2 = yScroll + i * ITEM_HEIGHT;
					int num3 = wScroll - 36;
					int h = ITEM_HEIGHT - 1;
					int num4 = xScroll;
					int num5 = yScroll + i * ITEM_HEIGHT;
					int num6 = 34;
					int num7 = ITEM_HEIGHT - 1;
					if (num2 - cmy <= yScroll + hScroll && num2 - cmy >= yScroll - ITEM_HEIGHT)
					{
						g.setColor((i != selected) ? 15196114 : 16383818);
						g.fillRect(xScroll, num2, wScroll, h);
						mFont.tahoma_7b_blue.drawString(g, mapNames[i], 5, num2 + 1, 0);
						mFont.tahoma_7_grey.drawString(g, planetNames[i], 5, num2 + 11, 0);
					}
				}
				paintScrollArrow(g);
			}

	private void paintZone(mGraphics g)
			{
				g.setColor(16711680);
				g.setClip(xScroll, yScroll, wScroll, hScroll);
				g.translate(0, -cmy);
				int[] zones = GameScr.gI().zones;
				int[] pts = GameScr.gI().pts;
				for (int i = 0; i < pts.Length; i++)
				{
					int num = xScroll + 36;
					int num2 = yScroll + i * ITEM_HEIGHT;
					int num3 = wScroll - 36;
					int h = ITEM_HEIGHT - 1;
					int num4 = xScroll;
					int y = yScroll + i * ITEM_HEIGHT;
					int num5 = 34;
					int h2 = ITEM_HEIGHT - 1;
					if (num2 - cmy > yScroll + hScroll || num2 - cmy < yScroll - ITEM_HEIGHT)
					{
						continue;
					}
					g.setColor((i != selected) ? 15196114 : 16383818);
					g.fillRect(num, num2, num3, h);
					g.setColor(zoneColor[pts[i]]);
					g.fillRect(num4, y, num5, h2);
					if (zones[i] != -1)
					{
						if (pts[i] != 1)
						{
							mFont.tahoma_7_yellow.drawString(g, zones[i] + string.Empty, num4 + num5 / 2, num2 + 6, mFont.CENTER);
						}
						else
						{
							mFont.tahoma_7_grey.drawString(g, zones[i] + string.Empty, num4 + num5 / 2, num2 + 6, mFont.CENTER);
						}
						mFont.tahoma_7_green2.drawString(g, GameScr.gI().numPlayer[i] + "/" + GameScr.gI().maxPlayer[i], num + 5, num2 + 6, 0);
					}
					if (GameScr.gI().rankName1[i] != null)
					{
						mFont.tahoma_7_grey.drawString(g, GameScr.gI().rankName1[i] + "(Top " + GameScr.gI().rank1[i] + ")", num + num3 - 2, num2 + 1, mFont.RIGHT);
						mFont.tahoma_7_grey.drawString(g, GameScr.gI().rankName2[i] + "(Top " + GameScr.gI().rank2[i] + ")", num + num3 - 2, num2 + 11, mFont.RIGHT);
					}
				}
				paintScrollArrow(g);
			}

	private void paintClans(mGraphics g)
			{
				g.setClip(xScroll, yScroll, wScroll, hScroll);
				g.translate(-cmx, -cmy);
				g.setColor(0);
				int num = xScroll + wScroll / 2 - clansOption.Length * TAB_W / 2;
				if (currentListLength == 2)
				{
					mFont.tahoma_7_green2.drawString(g, clanReport, xScroll + wScroll / 2, yScroll + 24 + hScroll / 2 - mFont.tahoma_7.getHeight() / 2, 2);
					if (isMessage && myMember.size() == 1)
					{
						for (int i = 0; i < mResources.clanEmpty.Length; i++)
						{
							mFont.tahoma_7b_dark.drawString(g, mResources.clanEmpty[i], xScroll + wScroll / 2, yScroll + 24 + hScroll / 2 - mResources.clanEmpty.Length * 12 / 2 + i * 12, mFont.CENTER);
						}
					}
				}
				if (isMessage)
				{
					currentListLength = ClanMessage.vMessage.size() + 2;
				}
				for (int j = 0; j < currentListLength; j++)
				{
					int num2 = xScroll;
					int num3 = yScroll + j * ITEM_HEIGHT;
					int num4 = 24;
					int num5 = ITEM_HEIGHT - 1;
					int num6 = xScroll + num4;
					int num7 = yScroll + j * ITEM_HEIGHT;
					int num8 = wScroll - num4;
					int num9 = ITEM_HEIGHT - 1;
					if (num7 - cmy > yScroll + hScroll || num7 - cmy < yScroll - ITEM_HEIGHT)
					{
						continue;
					}
					switch (j)
					{
					case 0:
					{
						for (int k = 0; k < clansOption.Length; k++)
						{
							g.setColor((k != cSelected || j != selected) ? 15723751 : 16383818);
							g.fillRect(num + k * TAB_W, num7, TAB_W - 1, 23);
							for (int l = 0; l < clansOption[k].Length; l++)
							{
								mFont.tahoma_7_grey.drawString(g, clansOption[k][l], num + k * TAB_W + TAB_W / 2, yScroll + l * 11, mFont.CENTER);
							}
						}
						continue;
					}
					case 1:
						g.setColor((j != selected) ? 15196114 : 16383818);
						g.fillRect(xScroll, num7, wScroll, num9);
						if (clanInfo != null)
						{
							mFont.tahoma_7b_dark.drawString(g, clanInfo, xScroll + wScroll / 2, num7 + 6, mFont.CENTER);
						}
						continue;
					}
					if (isSearchClan)
					{
						if (clans == null || clans.Length == 0)
						{
							continue;
						}
						g.setColor((j != selected) ? 15196114 : 16383818);
						g.fillRect(num6, num7, num8, num9);
						g.setColor((j != selected) ? 9993045 : 9541120);
						g.fillRect(num2, num3, num4, num5);
						if (ClanImage.isExistClanImage(clans[j - 2].imgID))
						{
							if (ClanImage.getClanImage((short)clans[j - 2].imgID).idImage != null)
							{
								SmallImage.drawSmallImage(g, ClanImage.getClanImage((short)clans[j - 2].imgID).idImage[0], num2 + num4 / 2, num3 + num5 / 2, 0, StaticObj.VCENTER_HCENTER);
							}
						}
						else
						{
							ClanImage clanImage = new ClanImage();
							clanImage.ID = clans[j - 2].imgID;
							if (!ClanImage.isExistClanImage(clanImage.ID))
							{
								ClanImage.addClanImage(clanImage);
							}
						}
						string st = ((clans[j - 2].name.Length <= 23) ? clans[j - 2].name : (clans[j - 2].name.Substring(0, 23) + "..."));
						mFont.tahoma_7b_green2.drawString(g, st, num6 + 5, num7, 0);
						g.setClip(num6, num7, num8 - 10, num9);
						mFont.tahoma_7_blue.drawString(g, clans[j - 2].slogan, num6 + 5, num7 + 11, 0);
						g.setClip(xScroll, yScroll + cmy, wScroll, hScroll);
						mFont.tahoma_7_green2.drawString(g, clans[j - 2].currMember + "/" + clans[j - 2].maxMember, num6 + num8 - 5, num7, mFont.RIGHT);
						continue;
					}
					if (isViewMember)
					{
						g.setColor((j != selected) ? 15196114 : 16383818);
						g.fillRect(num6, num7, num8, num9);
						g.setColor((j != selected) ? 9993045 : 9541120);
						g.fillRect(num2, num3, num4, num5);
						Member member = ((this.member == null) ? ((Member)myMember.elementAt(j - 2)) : ((Member)this.member.elementAt(j - 2)));
						if (member.headICON != -1)
						{
							SmallImage.drawSmallImage(g, member.headICON, num2, num3, 0, 0);
						}
						else
						{
							Part part = GameScr.parts[member.head];
							SmallImage.drawSmallImage(g, part.pi[Char.CharInfo[0][0][0]].id, num2 + part.pi[Char.CharInfo[0][0][0]].dx, num3 + 3 + part.pi[Char.CharInfo[0][0][0]].dy, 0, 0);
						}
						g.setClip(xScroll, yScroll + cmy, wScroll, hScroll);
						mFont mFont2 = mFont.tahoma_7b_dark;
						if (member.role == 0)
						{
							mFont2 = mFont.tahoma_7b_red;
						}
						else if (member.role == 1)
						{
							mFont2 = mFont.tahoma_7b_green;
						}
						else if (member.role == 2)
						{
							mFont2 = mFont.tahoma_7b_green2;
						}
						mFont2.drawString(g, member.name, num6 + 5, num7, 0);
						mFont.tahoma_7_blue.drawString(g, mResources.power + ": " + member.powerPoint, num6 + 5, num7 + 11, 0);
						SmallImage.drawSmallImage(g, 7223, num6 + num8 - 7, num7 + 12, 0, 3);
						mFont.tahoma_7_blue.drawString(g, string.Empty + member.clanPoint, num6 + num8 - 15, num7 + 6, mFont.RIGHT);
						continue;
					}
					if (!isMessage || ClanMessage.vMessage.size() == 0)
					{
						continue;
					}
					ClanMessage clanMessage = (ClanMessage)ClanMessage.vMessage.elementAt(j - 2);
					g.setColor((j != selected || clanMessage.option != null) ? 15196114 : 16383818);
					g.fillRect(num2, num3, num8 + num4, num9);
					clanMessage.paint(g, num2, num3);
					if (clanMessage.option == null)
					{
						continue;
					}
					int num10 = xScroll + wScroll - 2 - clanMessage.option.Length * 40;
					for (int m = 0; m < clanMessage.option.Length; m++)
					{
						if (m == cSelected && j == selected)
						{
							g.drawImage(GameScr.imgLbtnFocus2, num10 + m * 40 + 20, num7 + num9 / 2, StaticObj.VCENTER_HCENTER);
							mFont.tahoma_7b_green2.drawString(g, clanMessage.option[m], num10 + m * 40 + 20, num7 + 6, mFont.CENTER);
						}
						else
						{
							g.drawImage(GameScr.imgLbtn2, num10 + m * 40 + 20, num7 + num9 / 2, StaticObj.VCENTER_HCENTER);
							mFont.tahoma_7b_dark.drawString(g, clanMessage.option[m], num10 + m * 40 + 20, num7 + 6, mFont.CENTER);
						}
					}
				}
				paintScrollArrow(g);
			}

	private void paintClanInfo(mGraphics g)
			{
				if (Char.myCharz().clan == null)
				{
					SmallImage.drawSmallImage(g, Char.myCharz().avatarz(), 25, 50, 0, 33);
					mFont.tahoma_7b_white.drawString(g, mResources.not_join_clan, (wScroll - 50) / 2 + 50, 20, mFont.CENTER);
				}
				else if (!isViewMember)
				{
					Clan clan = Char.myCharz().clan;
					if (clan != null)
					{
						SmallImage.drawSmallImage(g, Char.myCharz().avatarz(), 25, 50, 0, 33);
						mFont.tahoma_7b_white.drawString(g, clan.name, 60, 4, mFont.LEFT, mFont.tahoma_7b_dark);
						mFont.tahoma_7_yellow.drawString(g, mResources.achievement_point + ": " + clan.powerPoint, 60, 16, mFont.LEFT, mFont.tahoma_7_grey);
						mFont.tahoma_7_yellow.drawString(g, mResources.clan_point + ": " + clan.clanPoint, 60, 27, mFont.LEFT, mFont.tahoma_7_grey);
						mFont.tahoma_7_yellow.drawString(g, mResources.level + ": " + clan.level, 60, 38, mFont.LEFT, mFont.tahoma_7_grey);
						TextInfo.paint(g, clan.slogan, 60, 38, wScroll - 70, ITEM_HEIGHT, mFont.tahoma_7_yellow);
					}
				}
				else
				{
					Clan clan2 = ((currClan == null) ? Char.myCharz().clan : currClan);
					SmallImage.drawSmallImage(g, Char.myCharz().avatarz(), 25, 50, 0, 33);
					mFont.tahoma_7b_white.drawString(g, clan2.name, 60, 4, mFont.LEFT, mFont.tahoma_7b_dark);
					mFont.tahoma_7_yellow.drawString(g, mResources.member + ": " + clan2.currMember + "/" + clan2.maxMember, 60, 16, mFont.LEFT, mFont.tahoma_7_grey);
					mFont.tahoma_7_yellow.drawString(g, mResources.clan_leader + ": " + clan2.leaderName, 60, 27, mFont.LEFT, mFont.tahoma_7_grey);
					TextInfo.paint(g, clan2.slogan, 60, 38, wScroll - 70, ITEM_HEIGHT, mFont.tahoma_7_yellow);
				}
			}

	private void paintPetInfo(mGraphics g)
			{
				mFont.tahoma_7_yellow.drawString(g, mResources.power + ": " + NinjaUtil.getMoneys(Char.myPetz().cPower), X + 60, 4, mFont.LEFT, mFont.tahoma_7_grey);
				if (Char.myPetz().cPower > 0)
				{
					mFont.tahoma_7_yellow.drawString(g, (!Char.myPetz().me) ? Char.myPetz().currStrLevel : Char.myPetz().getStrLevel(), X + 60, 16, mFont.LEFT, mFont.tahoma_7_grey);
				}
				if (Char.myPetz().cDamFull > 0)
				{
					mFont.tahoma_7_yellow.drawString(g, mResources.hit_point + " :" + Char.myPetz().cDamFull, X + 60, 27, mFont.LEFT, mFont.tahoma_7_grey);
				}
				if (Char.myPetz().cMaxStamina > 0)
				{
					mFont.tahoma_7_yellow.drawString(g, mResources.vitality, X + 60, 38, mFont.LEFT, mFont.tahoma_7_grey);
					g.drawImage(GameScr.imgMPLost, X + 100, 41, 0);
					int num = Char.myPetz().cStamina * mGraphics.getImageWidth(GameScr.imgMP) / Char.myPetz().cMaxStamina;
					g.setClip(100, X + 41, num, 20);
					g.drawImage(GameScr.imgMP, X + 100, 41, 0);
				}
				g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
			}

	private void paintZoneInfo(mGraphics g)
			{
				mFont.tahoma_7b_white.drawString(g, mResources.zone + " " + TileMap.zoneID, 60, 4, mFont.LEFT, mFont.tahoma_7b_dark);
				mFont.tahoma_7_yellow.drawString(g, TileMap.mapName, 60, 16, mFont.LEFT, mFont.tahoma_7_grey);
				mFont.tahoma_7b_white.drawString(g, TileMap.zoneID + string.Empty, 25, 27, mFont.CENTER);
			}

	private void paintMapInfo(mGraphics g)
			{
				mFont.tahoma_7b_white.drawString(g, mResources.MENUGENDER[TileMap.planetID], 60, 4, mFont.LEFT);
				string text = string.Empty;
				if (TileMap.mapID >= 135 && TileMap.mapID <= 138)
				{
					text = " " + mResources.tang + TileMap.zoneID;
				}
				mFont.tahoma_7_yellow.drawString(g, TileMap.mapName + text, 60, 16, mFont.LEFT);
				mFont.tahoma_7b_white.drawString(g, mResources.quest_place + ": ", 60, 27, mFont.LEFT);
				if (GameScr.getTaskMapId() >= 0 && GameScr.getTaskMapId() <= TileMap.mapNames.Length - 1)
				{
					mFont.tahoma_7_yellow.drawString(g, TileMap.mapNames[GameScr.getTaskMapId()], 60, 38, mFont.LEFT);
				}
				else
				{
					mFont.tahoma_7_yellow.drawString(g, mResources.random, 60, 38, mFont.LEFT);
				}
			}

	private void paintPetStatusInfo(mGraphics g)
			{
				mFont.tahoma_7b_white.drawString(g, "HP: " + Char.myPetz().cHP + "/" + Char.myPetz().cHPFull, X + 60, 4, mFont.LEFT, mFont.tahoma_7b_dark);
				mFont.tahoma_7b_white.drawString(g, "MP: " + Char.myPetz().cMP + "/" + Char.myPetz().cMPFull, X + 60, 16, mFont.LEFT, mFont.tahoma_7b_dark);
				mFont.tahoma_7_yellow.drawString(g, mResources.critical + ": " + Char.myPetz().cCriticalFull + ", " + mResources.armor + ": " + Char.myPetz().cDefull, X + 60, 27, mFont.LEFT, mFont.tahoma_7_grey);
				mFont.tahoma_7_yellow.drawString(g, mResources.status + ": " + strStatus[Char.myPetz().petStatus], X + 60, 38, mFont.LEFT, mFont.tahoma_7_grey);
			}

	public void paintMap(mGraphics g)
			{
				g.setClip(xScroll, yScroll, wScroll, hScroll);
				g.translate(-cmxMap, -cmyMap);
				g.drawImage(imgMap, xScroll, yScroll, 0);
				int head = Char.myCharz().head;
				Part part = GameScr.parts[head];
				SmallImage.drawSmallImage(g, part.pi[Char.CharInfo[0][0][0]].id, xMap, yMap + 5, 0, 3);
				int align = mFont.CENTER;
				if (xMap <= 40)
				{
					align = mFont.LEFT;
				}
				if (xMap >= 220)
				{
					align = mFont.RIGHT;
				}
				mFont.tahoma_7b_yellow.drawString(g, TileMap.mapName, xMap, yMap - 12, align, mFont.tahoma_7_grey);
				int num = -1;
				if (GameScr.getTaskMapId() != -1)
				{
					for (int i = 0; i < mapId[TileMap.planetID].Length; i++)
					{
						if (mapId[TileMap.planetID][i] == GameScr.getTaskMapId())
						{
							num = i;
							break;
						}
						num = 4;
					}
					if (GameCanvas.gameTick % 4 > 0)
					{
						g.drawImage(ItemMap.imageFlare, xScroll + mapX[TileMap.planetID][num], yScroll + mapY[TileMap.planetID][num], 3);
					}
				}
				if (!GameCanvas.isTouch)
				{
					g.drawImage(imgBantay, xMove, yMove, StaticObj.TOP_RIGHT);
					for (int j = 0; j < mapX[TileMap.planetID].Length; j++)
					{
						int num2 = mapX[TileMap.planetID][j] + xScroll;
						int num3 = mapY[TileMap.planetID][j] + yScroll;
						if (Res.inRect(num2 - 15, num3 - 15, 30, 30, xMove, yMove))
						{
							align = mFont.CENTER;
							if (num2 <= 20)
							{
								align = mFont.LEFT;
							}
							if (num2 >= 220)
							{
								align = mFont.RIGHT;
							}
							mFont.tahoma_7b_yellow.drawString(g, TileMap.mapNames[mapId[TileMap.planetID][j]], num2, num3 - 12, align, mFont.tahoma_7_grey);
							break;
						}
					}
				}
				else if (!trans)
				{
					for (int k = 0; k < mapX[TileMap.planetID].Length; k++)
					{
						int num4 = mapX[TileMap.planetID][k] + xScroll;
						int num5 = mapY[TileMap.planetID][k] + yScroll;
						if (Res.inRect(num4 - 15, num5 - 15, 30, 30, pX, pY))
						{
							align = mFont.CENTER;
							if (num4 <= 30)
							{
								align = mFont.LEFT;
							}
							if (num4 >= 220)
							{
								align = mFont.RIGHT;
							}
							g.drawImage(imgBantay, num4, num5, StaticObj.TOP_RIGHT);
							mFont.tahoma_7b_yellow.drawString(g, TileMap.mapNames[mapId[TileMap.planetID][k]], num4, num5 - 12, align, mFont.tahoma_7_grey);
							break;
						}
					}
				}
				g.translate(-g.getTranslateX(), -g.getTranslateY());
				if (num != -1)
				{
					if (mapX[TileMap.planetID][num] + xScroll < cmxMap)
					{
						g.drawRegion(Mob.imgHP, 0, 0, 9, 6, 5, xScroll + 5, yScroll + hScroll / 2 - 4, 0);
					}
					if (cmxMap + wScroll < mapX[TileMap.planetID][num] + xScroll)
					{
						g.drawRegion(Mob.imgHP, 0, 0, 9, 6, 6, xScroll + wScroll - 5, yScroll + hScroll / 2 - 4, StaticObj.TOP_RIGHT);
					}
					if (mapY[TileMap.planetID][num] < cmyMap)
					{
						g.drawRegion(Mob.imgHP, 0, 0, 9, 6, 1, xScroll + wScroll / 2, yScroll + 5, StaticObj.TOP_CENTER);
					}
					if (mapY[TileMap.planetID][num] > cmyMap + hScroll)
					{
						g.drawRegion(Mob.imgHP, 0, 0, 9, 6, 0, xScroll + wScroll / 2, yScroll + hScroll - 5, StaticObj.BOTTOM_HCENTER);
					}
				}
			}

}
