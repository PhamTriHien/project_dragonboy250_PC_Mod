using System;
using System.Collections.Generic;
using Assets.src.g;
using UnityEngine;
public partial class Panel : IActionListener, IChatable
{
	private void doFireSkill()
			{
				if (selected < 0)
				{
					return;
				}
				if (Char.myCharz().statusMe == 14)
				{
					GameCanvas.startOKDlg(mResources.can_not_do_when_die);
					return;
				}
				if (selected == 0 || selected == 1 || selected == 2 || selected == 3 || selected == 4 || selected == 5)
				{
					long cTiemNang = Char.myCharz().cTiemNang;
					int cHPGoc = Char.myCharz().cHPGoc;
					int cMPGoc = Char.myCharz().cMPGoc;
					int cDamGoc = Char.myCharz().cDamGoc;
					int cDefGoc = Char.myCharz().cDefGoc;
					int cCriticalGoc = Char.myCharz().cCriticalGoc;
					int num = 0;
					int num2 = 1000;
					if (selected == 0)
					{
						if (cTiemNang < Char.myCharz().cHPGoc + num2)
						{
							GameCanvas.startOKDlg(mResources.not_enough_potential_point1 + Char.myCharz().cTiemNang + mResources.not_enough_potential_point2 + (Char.myCharz().cHPGoc + num2), isError: false);
							return;
						}
						if (cTiemNang > cHPGoc && cTiemNang < 10 * (2 * (cHPGoc + num2) + 180) / 2)
						{
							GameCanvas.startYesNoDlg(mResources.use_potential_point_for1 + (cHPGoc + num2) + mResources.use_potential_point_for2 + Char.myCharz().hpFrom1000TiemNang + mResources.for_HP, new Command(mResources.increase_upper, this, 9000, null), new Command(mResources.CANCEL, this, 4007, null));
							return;
						}
						if (cTiemNang >= 10 * (2 * (cHPGoc + num2) + 180) / 2 && cTiemNang < 100 * (2 * (cHPGoc + num2) + 1980) / 2)
						{
							MyVector myVector = new MyVector(string.Empty);
							myVector.addElement(new Command(mResources.increase_upper + "\n" + Char.myCharz().hpFrom1000TiemNang + mResources.HP + "\n-" + Res.formatNumber2(cHPGoc + num2), this, 9000, null));
							myVector.addElement(new Command(mResources.increase_upper + "\n" + 10 * Char.myCharz().hpFrom1000TiemNang + mResources.HP + "\n-" + Res.formatNumber2(10 * (2 * (cHPGoc + num2) + 180) / 2), this, 9006, null));
							GameCanvas.menu.startAt(myVector, X, (selected + 1) * ITEM_HEIGHT - cmy + yScroll);
							addSkillDetail2(selected);
						}
						if (cTiemNang >= 100 * (2 * (cHPGoc + num2) + 1980) / 2)
						{
							MyVector myVector2 = new MyVector(string.Empty);
							myVector2.addElement(new Command(mResources.increase_upper + "\n" + Char.myCharz().hpFrom1000TiemNang + mResources.HP + "\n-" + Res.formatNumber2(cHPGoc + num2), this, 9000, null));
							myVector2.addElement(new Command(mResources.increase_upper + "\n" + 10 * Char.myCharz().hpFrom1000TiemNang + mResources.HP + "\n-" + Res.formatNumber2(10 * (2 * (cHPGoc + num2) + 180) / 2), this, 9006, null));
							myVector2.addElement(new Command(mResources.increase_upper + "\n" + 100 * Char.myCharz().hpFrom1000TiemNang + mResources.HP + "\n-" + Res.formatNumber2(100 * (2 * (cHPGoc + num2) + 1980) / 2), this, 9007, null));
							GameCanvas.menu.startAt(myVector2, X, (selected + 1) * ITEM_HEIGHT - cmy + yScroll);
							addSkillDetail2(selected);
						}
					}
					if (selected == 1)
					{
						if (Char.myCharz().cTiemNang < Char.myCharz().cMPGoc + num2)
						{
							GameCanvas.startOKDlg(mResources.not_enough_potential_point1 + Char.myCharz().cTiemNang + mResources.not_enough_potential_point2 + (Char.myCharz().cMPGoc + num2));
							return;
						}
						if (cTiemNang > cMPGoc && cTiemNang < 10 * (2 * (cMPGoc + num2) + 180) / 2)
						{
							GameCanvas.startYesNoDlg(mResources.use_potential_point_for1 + (cMPGoc + num2) + mResources.use_potential_point_for2 + Char.myCharz().mpFrom1000TiemNang + mResources.for_KI, new Command(mResources.increase_upper, this, 9000, null), new Command(mResources.CANCEL, this, 4007, null));
							return;
						}
						if (cTiemNang >= 10 * (2 * (cMPGoc + num2) + 180) / 2 && cTiemNang < 100 * (2 * (cMPGoc + num2) + 1980) / 2)
						{
							MyVector myVector3 = new MyVector(string.Empty);
							myVector3.addElement(new Command(mResources.increase_upper + "\n" + Char.myCharz().mpFrom1000TiemNang + mResources.KI + "\n-" + Res.formatNumber2(cHPGoc + num2), this, 9000, null));
							myVector3.addElement(new Command(mResources.increase_upper + "\n" + 10 * Char.myCharz().mpFrom1000TiemNang + mResources.KI + "\n-" + Res.formatNumber2(10 * (2 * (cHPGoc + num2) + 180) / 2), this, 9006, null));
							GameCanvas.menu.startAt(myVector3, X, (selected + 1) * ITEM_HEIGHT - cmy + yScroll);
							addSkillDetail2(selected);
						}
						if (cTiemNang >= 100 * (2 * (cMPGoc + num2) + 1980) / 2)
						{
							MyVector myVector4 = new MyVector(string.Empty);
							myVector4.addElement(new Command(mResources.increase_upper + "\n" + Char.myCharz().mpFrom1000TiemNang + mResources.KI + "\n-" + Res.formatNumber2(cMPGoc + num2), this, 9000, null));
							myVector4.addElement(new Command(mResources.increase_upper + "\n" + 10 * Char.myCharz().mpFrom1000TiemNang + mResources.KI + "\n-" + Res.formatNumber2(10 * (2 * (cMPGoc + num2) + 180) / 2), this, 9006, null));
							myVector4.addElement(new Command(mResources.increase_upper + "\n" + 100 * Char.myCharz().mpFrom1000TiemNang + mResources.KI + "\n-" + Res.formatNumber2(100 * (2 * (cMPGoc + num2) + 1980) / 2), this, 9007, null));
							GameCanvas.menu.startAt(myVector4, X, (selected + 1) * ITEM_HEIGHT - cmy + yScroll);
							addSkillDetail2(selected);
						}
					}
					if (selected == 2)
					{
						if (Char.myCharz().cTiemNang < Char.myCharz().cDamGoc * Char.myCharz().expForOneAdd)
						{
							GameCanvas.startOKDlg(mResources.not_enough_potential_point1 + Char.myCharz().cTiemNang + mResources.not_enough_potential_point2 + cDamGoc * 100);
							return;
						}
						if (cTiemNang > cDamGoc && cTiemNang < 10 * (2 * cDamGoc + 9) / 2 * Char.myCharz().expForOneAdd)
						{
							GameCanvas.startYesNoDlg(mResources.use_potential_point_for1 + cDamGoc * 100 + mResources.use_potential_point_for2 + Char.myCharz().damFrom1000TiemNang + mResources.for_hit_point, new Command(mResources.increase_upper, this, 9000, null), new Command(mResources.CANCEL, this, 4007, null));
							return;
						}
						if (cTiemNang >= 10 * (2 * cDamGoc + 9) / 2 * Char.myCharz().expForOneAdd && cTiemNang < 100 * (2 * cDamGoc + 99) / 2 * Char.myCharz().expForOneAdd)
						{
							MyVector myVector5 = new MyVector(string.Empty);
							myVector5.addElement(new Command(mResources.increase_upper + "\n" + Char.myCharz().damFrom1000TiemNang + "\n" + mResources.hit_point + "\n-" + Res.formatNumber2(cDamGoc * 100), this, 9000, null));
							myVector5.addElement(new Command(mResources.increase_upper + "\n" + 10 * Char.myCharz().damFrom1000TiemNang + "\n" + mResources.hit_point + "\n-" + Res.formatNumber2(10 * (2 * cDamGoc + 9) / 2 * Char.myCharz().expForOneAdd), this, 9006, null));
							GameCanvas.menu.startAt(myVector5, X, (selected + 1) * ITEM_HEIGHT - cmy + yScroll);
							addSkillDetail2(selected);
						}
						if (cTiemNang >= 100 * (2 * cDamGoc + 99) / 2 * Char.myCharz().expForOneAdd)
						{
							MyVector myVector6 = new MyVector(string.Empty);
							myVector6.addElement(new Command(mResources.increase_upper + "\n" + Char.myCharz().damFrom1000TiemNang + "\n" + mResources.hit_point + "\n-" + Res.formatNumber2(cDamGoc * 100), this, 9000, null));
							myVector6.addElement(new Command(mResources.increase_upper + "\n" + 10 * Char.myCharz().damFrom1000TiemNang + "\n" + mResources.hit_point + "\n-" + Res.formatNumber2(10 * (2 * cDamGoc + 9) / 2 * Char.myCharz().expForOneAdd), this, 9006, null));
							myVector6.addElement(new Command(mResources.increase_upper + "\n" + 100 * Char.myCharz().damFrom1000TiemNang + "\n" + mResources.hit_point + "\n-" + Res.formatNumber2(100 * (2 * cDamGoc + 99) / 2 * Char.myCharz().expForOneAdd), this, 9007, null));
							GameCanvas.menu.startAt(myVector6, X, (selected + 1) * ITEM_HEIGHT - cmy + yScroll);
							addSkillDetail2(selected);
						}
					}
					if (selected == 3)
					{
						if (Char.myCharz().cTiemNang < 50000 + Char.myCharz().cDefGoc * 1000)
						{
							GameCanvas.startOKDlg(mResources.not_enough_potential_point1 + NinjaUtil.getMoneys(Char.myCharz().cTiemNang) + mResources.not_enough_potential_point2 + NinjaUtil.getMoneys(50000 + Char.myCharz().cDefGoc * 1000));
							return;
						}
						long number = (long)(2 * (cDefGoc + 5)) / 2L * 100000;
						long number2 = 10L * (long)(2 * (cDefGoc + 5) + 9) / 2 * 100000;
						long number3 = 100L * (long)(2 * (cDefGoc + 5) + 99) / 2 * 100000;
						mResources.use_potential_point_for1 = mResources.increase_upper;
						MyVector myVector7 = new MyVector(string.Empty);
						myVector7.addElement(new Command(mResources.use_potential_point_for1 + "\n1 " + mResources.armor + "\n" + Res.formatNumber2(number), this, 9000, null));
						myVector7.addElement(new Command(mResources.use_potential_point_for1 + "\n10 " + mResources.armor + "\n" + Res.formatNumber2(number2), this, 9006, null));
						myVector7.addElement(new Command(mResources.use_potential_point_for1 + "\n100 " + mResources.armor + "\n" + Res.formatNumber2(number3), this, 9007, null));
						GameCanvas.menu.startAt(myVector7, X, (selected + 1) * ITEM_HEIGHT - cmy + yScroll);
						addSkillDetail2(selected);
					}
					else if (selected == 4)
					{
						long num3 = 50000000L;
						int num4 = Char.myCharz().cCriticalGoc;
						if (num4 > t_tiemnang.Length - 1)
						{
							num4 = t_tiemnang.Length - 1;
						}
						num3 = t_tiemnang[num4];
						if (Char.myCharz().cTiemNang < num3)
						{
							GameCanvas.startOKDlg(mResources.not_enough_potential_point1 + Res.formatNumber2(Char.myCharz().cTiemNang) + mResources.not_enough_potential_point2 + Res.formatNumber2(num3));
							return;
						}
						GameCanvas.startYesNoDlg(mResources.use_potential_point_for1 + Res.formatNumber(num3) + mResources.use_potential_point_for2 + Char.myCharz().criticalFrom1000Tiemnang + mResources.for_crit, new Command(mResources.increase_upper, this, 9000, null), new Command(mResources.CANCEL, this, 4007, null));
					}
					else if (selected == 5)
					{
						Service.gI().speacialSkill(0);
					}
					return;
				}
				int num5 = selected - 6;
				SkillTemplate skillTemplate = Char.myCharz().nClass.skillTemplates[num5];
				Skill skill = Char.myCharz().getSkill(skillTemplate);
				Skill skill2 = null;
				MyVector myVector8 = new MyVector(string.Empty);
				if (skill != null)
				{
					if (skill.point == skillTemplate.maxPoint)
					{
						myVector8.addElement(new Command(mResources.make_shortcut, this, 9003, skill.template));
						myVector8.addElement(new Command(mResources.CLOSE, 2));
					}
					else
					{
						skill2 = skillTemplate.skills[skill.point];
						myVector8.addElement(new Command(mResources.UPGRADE, this, 9002, skill2));
						myVector8.addElement(new Command(mResources.make_shortcut, this, 9003, skill.template));
					}
				}
				else
				{
					skill2 = skillTemplate.skills[0];
					myVector8.addElement(new Command(mResources.learn, this, 9004, skill2));
				}
				GameCanvas.menu.startAt(myVector8, X, (selected + 1) * ITEM_HEIGHT - cmy + yScroll);
				addSkillDetail(skillTemplate, skill, skill2);
			}
	private void doFireBox()
			{
				if (selected < 0)
				{
					return;
				}
				currItem = null;
				MyVector myVector = new MyVector();
				if (currentTabIndex == 0 && !Equals(GameCanvas.panel2))
				{
					if (selected == 0)
					{
						setNewSelected(Char.myCharz().arrItemBox.Length, resetSelect: false, isTabBox: true);
					}
					else
					{
						sbyte b = (sbyte)GetInventorySelect_body(selected, newSelected);
						Item item = Char.myCharz().arrItemBox[b];
						if (item != null)
						{
							if (isBoxClan)
							{
								myVector.addElement(new Command(mResources.GETOUT, this, 1000, item));
								myVector.addElement(new Command(mResources.USE, this, 2010, item));
							}
							else if (item.isTypeBody())
							{
								myVector.addElement(new Command(mResources.GETOUT, this, 1000, item));
							}
							else
							{
								myVector.addElement(new Command(mResources.GETOUT, this, 1000, item));
							}
							currItem = item;
						}
					}
				}
				if (currentTabIndex == 1 || Equals(GameCanvas.panel2))
				{
					if (selected == 0)
					{
						setNewSelected(Char.myCharz().arrItemBody.Length + Char.myCharz().arrItemBag.Length, resetSelect: true, isTabBox: false);
					}
					else
					{
						Item[] arrItemBody = Char.myCharz().arrItemBody;
						if (!GetInventorySelect_isbody(selected, newSelected, arrItemBody))
						{
							sbyte b2 = (sbyte)GetInventorySelect_bag(selected, newSelected, arrItemBody);
							Item item2 = Char.myCharz().arrItemBag[b2];
							if (item2 != null)
							{
								myVector.addElement(new Command(mResources.move_to_chest, this, 1001, item2));
								if (item2.isTypeBody())
								{
									myVector.addElement(new Command(mResources.USE, this, 2000, item2));
								}
								else
								{
									myVector.addElement(new Command(mResources.USE, this, 2001, item2));
								}
								currItem = item2;
							}
						}
						else
						{
							Item item3 = Char.myCharz().arrItemBody[GetInventorySelect_body(selected, newSelected)];
							if (item3 != null)
							{
								myVector.addElement(new Command(mResources.move_to_chest2, this, 1002, item3));
								currItem = item3;
							}
						}
					}
				}
				if (currItem != null)
				{
					Char.myCharz().setPartTemp(currItem.headTemp, currItem.bodyTemp, currItem.legTemp, currItem.bagTemp);
					if (isBoxClan)
					{
						myVector.addElement(new Command(mResources.MOVEOUT, this, 2011, currItem));
					}
					GameCanvas.menu.startAt(myVector, X, (selected + 1) * ITEM_HEIGHT - cmy + yScroll);
					addItemDetail(currItem);
				}
				else
				{
					cp = null;
				}
				cmyLim = currentListLength * ITEM_HEIGHT - hScroll;
			}

}
