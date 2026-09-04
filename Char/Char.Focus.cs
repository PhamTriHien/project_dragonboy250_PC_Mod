using System;
using Assets.src.e;
using Assets.src.g;
using UnityEngine;

public partial class Char : IMapObject
{
	public void searchFocus()
			{
				if (myCharz().skillPaint != null || myCharz().arr != null || myCharz().dart != null)
				{
					timeFocusToMob = 200;
					return;
				}
				if (timeFocusToMob > 0)
				{
					timeFocusToMob--;
					return;
				}
				if (isManualFocus && charFocus != null && (charFocus.statusMe == 15 || charFocus.isInvisiblez))
				{
					charFocus = null;
				}
				if (GameCanvas.gameTick % 2 == 0 || isMeCanAttackOtherPlayer(charFocus))
				{
					return;
				}
				int num = 0;
				if (nClass != null && (nClass.classId == 0 || nClass.classId == 1 || nClass.classId == 3 || nClass.classId == 5))
				{
					num = 40;
				}
				int[] array = new int[4] { -1, -1, -1, -1 };
				int num2 = GameScr.cmx - 10;
				int num3 = GameScr.cmx + GameCanvas.w + 10;
				int cmy = GameScr.cmy;
				int num4 = GameScr.cmy + GameCanvas.h - GameScr.cmdBarH + 10;
				if (isManualFocus)
				{
					if ((mobFocus != null && mobFocus.status != 1 && mobFocus.status != 0 && num2 <= mobFocus.x && mobFocus.x <= num3 && cmy <= mobFocus.y && mobFocus.y <= num4) || (npcFocus != null && num2 <= npcFocus.cx && npcFocus.cx <= num3 && cmy <= npcFocus.cy && npcFocus.cy <= num4) || (charFocus != null && num2 <= charFocus.cx && charFocus.cx <= num3 && cmy <= charFocus.cy && charFocus.cy <= num4) || (itemFocus != null && num2 <= itemFocus.x && itemFocus.x <= num3 && cmy <= itemFocus.y && itemFocus.y <= num4))
					{
						return;
					}
					isManualFocus = false;
				}
				num2 = myCharz().cx - 80;
				num3 = myCharz().cx + 80;
				cmy = myCharz().cy - 30;
				num4 = myCharz().cy + 30;
				if (npcFocus != null && npcFocus.template.npcTemplateId == 6)
				{
					num2 = myCharz().cx - 20;
					num3 = myCharz().cx + 20;
					cmy = myCharz().cy - 10;
					num4 = myCharz().cy + 10;
				}
				if (npcFocus == null)
				{
					for (int i = 0; i < GameScr.vNpc.size(); i++)
					{
						Npc npc = (Npc)GameScr.vNpc.elementAt(i);
						if (npc.statusMe != 15)
						{
							int num5 = Math.abs(myCharz().cx - npc.cx);
							int num6 = Math.abs(myCharz().cy - npc.cy);
							int num7 = ((num5 <= num6) ? num6 : num5);
							num2 = myCharz().cx - 80;
							num3 = myCharz().cx + 80;
							cmy = myCharz().cy - 30;
							num4 = myCharz().cy + 30;
							if (npc.template.npcTemplateId == 6)
							{
								num2 = myCharz().cx - 20;
								num3 = myCharz().cx + 20;
								cmy = myCharz().cy - 10;
								num4 = myCharz().cy + 10;
							}
							if (num2 <= npc.cx && npc.cx <= num3 && cmy <= npc.cy && npc.cy <= num4 && (npcFocus == null || num7 < array[1]))
							{
								npcFocus = npc;
								array[1] = num7;
							}
						}
					}
				}
				else
				{
					if (num2 <= npcFocus.cx && npcFocus.cx <= num3 && cmy <= npcFocus.cy && npcFocus.cy <= num4)
					{
						clearFocus(1);
						return;
					}
					deFocusNPC();
					for (int j = 0; j < GameScr.vNpc.size(); j++)
					{
						Npc npc2 = (Npc)GameScr.vNpc.elementAt(j);
						if (npc2.statusMe != 15)
						{
							int num8 = Math.abs(myCharz().cx - npc2.cx);
							int num9 = Math.abs(myCharz().cy - npc2.cy);
							int num10 = ((num8 <= num9) ? num9 : num8);
							num2 = myCharz().cx - 80;
							num3 = myCharz().cx + 80;
							cmy = myCharz().cy - 30;
							num4 = myCharz().cy + 30;
							if (npc2.template.npcTemplateId == 6)
							{
								num2 = myCharz().cx - 20;
								num3 = myCharz().cx + 20;
								cmy = myCharz().cy - 10;
								num4 = myCharz().cy + 10;
							}
							if (num2 <= npc2.cx && npc2.cx <= num3 && cmy <= npc2.cy && npc2.cy <= num4 && (npcFocus == null || num10 < array[1]))
							{
								npcFocus = npc2;
								array[1] = num10;
							}
						}
					}
				}
				if (itemFocus == null)
				{
					for (int k = 0; k < GameScr.vItemMap.size(); k++)
					{
						ItemMap itemMap = (ItemMap)GameScr.vItemMap.elementAt(k);
						int num11 = Math.abs(myCharz().cx - itemMap.x);
						int num12 = Math.abs(myCharz().cy - itemMap.y);
						int num13 = ((num11 <= num12) ? num12 : num11);
						if (num11 > 48 || num12 > 48 || (itemFocus != null && num13 >= array[3]))
						{
							continue;
						}
						if (GameScr.gI().auto != 0 && GameScr.gI().isBagFull())
						{
							if (itemMap.template.type == 9)
							{
								itemFocus = itemMap;
								array[3] = num13;
							}
						}
						else
						{
							itemFocus = itemMap;
							array[3] = num13;
						}
					}
				}
				else
				{
					if (num2 <= itemFocus.x && itemFocus.x <= num3 && cmy <= itemFocus.y && itemFocus.y <= num4)
					{
						clearFocus(3);
						return;
					}
					itemFocus = null;
					for (int l = 0; l < GameScr.vItemMap.size(); l++)
					{
						ItemMap itemMap2 = (ItemMap)GameScr.vItemMap.elementAt(l);
						int num14 = Math.abs(myCharz().cx - itemMap2.x);
						int num15 = Math.abs(myCharz().cy - itemMap2.y);
						int num16 = ((num14 <= num15) ? num15 : num14);
						if (num2 > itemMap2.x || itemMap2.x > num3 || cmy > itemMap2.y || itemMap2.y > num4 || (itemFocus != null && num16 >= array[3]))
						{
							continue;
						}
						if (GameScr.gI().auto != 0 && GameScr.gI().isBagFull())
						{
							if (itemMap2.template.type == 9)
							{
								itemFocus = itemMap2;
								array[3] = num16;
							}
						}
						else
						{
							itemFocus = itemMap2;
							array[3] = num16;
						}
					}
				}
				num2 = myCharz().cx - myCharz().getdxSkill() - 10;
				num3 = myCharz().cx + myCharz().getdxSkill() + 10;
				cmy = myCharz().cy - myCharz().getdySkill() - num - 20;
				num4 = myCharz().cy + myCharz().getdySkill() + 20;
				if (num4 > myCharz().cy + 30)
				{
					num4 = myCharz().cy + 30;
				}
				if (mobFocus == null)
				{
					for (int m = 0; m < GameScr.vMob.size(); m++)
					{
						Mob mob = (Mob)GameScr.vMob.elementAt(m);
						int num17 = Math.abs(myCharz().cx - mob.x);
						int num18 = Math.abs(myCharz().cy - mob.y);
						int num19 = ((num17 <= num18) ? num18 : num17);
						if (num2 <= mob.x && mob.x <= num3 && cmy <= mob.y && mob.y <= num4 && (mobFocus == null || num19 < array[0]))
						{
							mobFocus = mob;
							array[0] = num19;
						}
					}
				}
				else
				{
					if (mobFocus.status != 1 && mobFocus.status != 0 && num2 <= mobFocus.x && mobFocus.x <= num3 && cmy <= mobFocus.y && mobFocus.y <= num4)
					{
						clearFocus(0);
						return;
					}
					mobFocus = null;
					for (int n = 0; n < GameScr.vMob.size(); n++)
					{
						Mob mob2 = (Mob)GameScr.vMob.elementAt(n);
						int num20 = Math.abs(myCharz().cx - mob2.x);
						int num21 = Math.abs(myCharz().cy - mob2.y);
						int num22 = ((num20 <= num21) ? num21 : num20);
						if (num2 <= mob2.x && mob2.x <= num3 && cmy <= mob2.y && mob2.y <= num4 && (mobFocus == null || num22 < array[0]))
						{
							mobFocus = mob2;
							array[0] = num22;
						}
					}
				}
				if (charFocus == null)
				{
					for (int num23 = 0; num23 < GameScr.vCharInMap.size(); num23++)
					{
						Char @char = (Char)GameScr.vCharInMap.elementAt(num23);
						if (@char.statusMe != 15 && !@char.isInvisiblez && wdx == 0 && wdy == 0)
						{
							int num24 = Math.abs(myCharz().cx - @char.cx);
							int num25 = Math.abs(myCharz().cy - @char.cy);
							int num26 = ((num24 <= num25) ? num25 : num24);
							if (num2 <= @char.cx && @char.cx <= num3 && cmy <= @char.cy && @char.cy <= num4 && (charFocus == null || num26 < array[2]))
							{
								charFocus = @char;
								array[2] = num26;
							}
						}
					}
				}
				else
				{
					if (num2 <= charFocus.cx && charFocus.cx <= num3 && cmy <= charFocus.cy && charFocus.cy <= num4 && charFocus.statusMe != 15 && !charFocus.isInvisiblez)
					{
						clearFocus(2);
						return;
					}
					charFocus = null;
					for (int num27 = 0; num27 < GameScr.vCharInMap.size(); num27++)
					{
						Char char2 = (Char)GameScr.vCharInMap.elementAt(num27);
						if (char2.statusMe != 15 && !char2.isInvisiblez && wdx == 0 && wdy == 0)
						{
							int num28 = Math.abs(myCharz().cx - char2.cx);
							int num29 = Math.abs(myCharz().cy - char2.cy);
							int num30 = ((num28 <= num29) ? num29 : num28);
							if (num2 <= char2.cx && char2.cx <= num3 && cmy <= char2.cy && char2.cy <= num4 && (charFocus == null || num30 < array[2]))
							{
								charFocus = char2;
								array[2] = num30;
							}
						}
					}
				}
				int num31 = -1;
				for (int num32 = 0; num32 < array.Length; num32++)
				{
					if (num31 == -1)
					{
						if (array[num32] != -1)
						{
							num31 = num32;
						}
					}
					else if (array[num32] < array[num31] && array[num32] != -1)
					{
						num31 = num32;
					}
				}
				clearFocus(num31);
				if (me && isAttacPlayerStatus())
				{
					if (mobFocus != null && !mobFocus.isMobMe)
					{
						mobFocus = null;
					}
					npcFocus = null;
					itemFocus = null;
				}
			}

	public void clearFocus(int index)
			{
				switch (index)
				{
				case 0:
					deFocusNPC();
					charFocus = null;
					itemFocus = null;
					break;
				case 1:
					mobFocus = null;
					charFocus = null;
					itemFocus = null;
					break;
				case 2:
					mobFocus = null;
					deFocusNPC();
					itemFocus = null;
					break;
				case 3:
					mobFocus = null;
					deFocusNPC();
					charFocus = null;
					break;
				}
			}

	public void findNextFocusByKey()
			{
				Res.outz("focus size= " + focus.size());
				if ((myCharz().skillPaint != null || myCharz().arr != null || myCharz().dart != null || myCharz().skillInfoPaint() != null) && focus.size() == 0)
				{
					return;
				}
				focus.removeAllElements();
				int num = 0;
				int num2 = GameScr.cmx + 10;
				int num3 = GameScr.cmx + GameCanvas.w - 10;
				int num4 = GameScr.cmy + 10;
				int num5 = GameScr.cmy + GameScr.gH;
				for (int i = 0; i < GameScr.vCharInMap.size(); i++)
				{
					Char @char = (Char)GameScr.vCharInMap.elementAt(i);
					if (@char.statusMe != 15 && !@char.isInvisiblez && num2 <= @char.cx && @char.cx <= num3 && num4 <= @char.cy && @char.cy <= num5 && @char.charID != -114 && (TileMap.mapID != 129 || (TileMap.mapID == 129 && myCharz().cy > 264)))
					{
						focus.addElement(@char);
						if (charFocus != null && @char.Equals(charFocus))
						{
							num = focus.size();
						}
					}
				}
				if (me && isAttacPlayerStatus())
				{
					Res.outz("co the tan cong nguoi");
					for (int j = 0; j < GameScr.vMob.size(); j++)
					{
						Mob mob = (Mob)GameScr.vMob.elementAt(j);
						if (!GameScr.gI().isMeCanAttackMob(mob))
						{
							Res.outz("khong the tan cong quai");
							mobFocus = null;
							continue;
						}
						Res.outz("co the tan ong quai");
						focus.addElement(mob);
						if (mobFocus != null)
						{
							num = focus.size();
						}
					}
					npcFocus = null;
					itemFocus = null;
					if (focus.size() > 0)
					{
						if (num >= focus.size())
						{
							num = 0;
						}
						focusManualTo(focus.elementAt(num));
					}
					else
					{
						mobFocus = null;
						deFocusNPC();
						charFocus = null;
						itemFocus = null;
						isManualFocus = false;
					}
					return;
				}
				for (int k = 0; k < GameScr.vItemMap.size(); k++)
				{
					ItemMap itemMap = (ItemMap)GameScr.vItemMap.elementAt(k);
					if (num2 <= itemMap.x && itemMap.x <= num3 && num4 <= itemMap.y && itemMap.y <= num5)
					{
						focus.addElement(itemMap);
						if (itemFocus != null && itemMap.Equals(itemFocus))
						{
							num = focus.size();
						}
					}
				}
				for (int l = 0; l < GameScr.vMob.size(); l++)
				{
					Mob mob2 = (Mob)GameScr.vMob.elementAt(l);
					if (mob2.status != 1 && mob2.status != 0 && num2 <= mob2.x && mob2.x <= num3 && num4 <= mob2.y && mob2.y <= num5)
					{
						focus.addElement(mob2);
						if (mobFocus != null && mob2.Equals(mobFocus))
						{
							num = focus.size();
						}
					}
				}
				for (int m = 0; m < GameScr.vNpc.size(); m++)
				{
					Npc npc = (Npc)GameScr.vNpc.elementAt(m);
					if (npc.statusMe != 15 && num2 <= npc.cx && npc.cx <= num3 && num4 <= npc.cy && npc.cy <= num5)
					{
						focus.addElement(npc);
						if (npcFocus != null && npc.Equals(npcFocus))
						{
							num = focus.size();
						}
					}
				}
				if (focus.size() > 0)
				{
					if (num >= focus.size())
					{
						num = 0;
					}
					focusManualTo(focus.elementAt(num));
				}
				else
				{
					mobFocus = null;
					deFocusNPC();
					charFocus = null;
					itemFocus = null;
					isManualFocus = false;
				}
			}

	public void deFocusNPC()
			{
				if (me && npcFocus != null)
				{
					if (!GameCanvas.menu.showMenu)
					{
						chatPopup = null;
					}
					npcFocus = null;
				}
			}


	public void clearTask()
			{
				myCharz().taskMaint = null;
				for (int i = 0; i < myCharz().arrItemBag.Length; i++)
				{
					if (myCharz().arrItemBag[i] != null && myCharz().arrItemBag[i].template.type == 8)
					{
						myCharz().arrItemBag[i] = null;
					}
				}
				Npc.clearEffTask();
			}

	public void focusManualTo(object objectz)
			{
				if (objectz is Mob)
				{
					mobFocus = (Mob)objectz;
					deFocusNPC();
					charFocus = null;
					itemFocus = null;
				}
				else if (objectz is Npc)
				{
					myCharz().mobFocus = null;
					myCharz().deFocusNPC();
					myCharz().npcFocus = (Npc)objectz;
					myCharz().charFocus = null;
					myCharz().itemFocus = null;
				}
				else if (objectz is Char)
				{
					myCharz().mobFocus = null;
					myCharz().deFocusNPC();
					myCharz().charFocus = (Char)objectz;
					myCharz().itemFocus = null;
				}
				else if (objectz is ItemMap)
				{
					myCharz().mobFocus = null;
					myCharz().deFocusNPC();
					myCharz().charFocus = null;
					myCharz().itemFocus = (ItemMap)objectz;
				}
				isManualFocus = true;
			}


}
