using System;
using Assets.src.e;
using Assets.src.g;
using UnityEngine;
public partial class Char : IMapObject
{
	private void updateSkillPaint()
			{
				if (statusMe == 14 || statusMe == 5)
				{
					return;
				}
				if (skillPaint != null && ((charFocus != null && isMeCanAttackOtherPlayer(charFocus) && charFocus.statusMe == 14) || (mobFocus != null && mobFocus.status == 0)))
				{
					if (!me)
					{
						if ((TileMap.tileTypeAtPixel(cx, cy) & 2) == 2)
						{
							statusMe = 1;
						}
						else
						{
							statusMe = 6;
						}
						cp3 = 0;
					}
					indexSkill = 0;
					skillPaint = null;
					skillPaintRandomPaint = null;
					eff0 = (eff1 = (eff2 = null));
					i0 = (i1 = (i2 = 0));
					mobFocus = null;
					charFocus = null;
					effPaints = null;
					currentMovePoint = null;
					arr = null;
					hasSendAttack = false;
					if ((TileMap.tileTypeAtPixel(cx, cy) & 2) != 2)
					{
						delayFall = 5;
					}
				}
				if (skillPaint != null && arr == null && skillInfoPaint() != null && indexSkill >= skillInfoPaint().Length)
				{
					if (!me)
					{
						if ((TileMap.tileTypeAtPixel(cx, cy) & 2) == 2)
						{
							statusMe = 1;
						}
						else
						{
							statusMe = 6;
						}
						cp3 = 0;
					}
					indexSkill = 0;
					Res.outz("remove 2");
					skillPaint = null;
					skillPaintRandomPaint = null;
					eff0 = (eff1 = (eff2 = null));
					i0 = (i1 = (i2 = 0));
					arr = null;
					hasSendAttack = false;
					if ((TileMap.tileTypeAtPixel(cx, cy) & 2) != 2)
					{
						delayFall = 5;
					}
				}
				SkillInfoPaint[] array = skillInfoPaint();
				if (array == null || indexSkill < 0 || indexSkill > array.Length - 1)
				{
					return;
				}
				if (array[indexSkill].effS0Id != 0)
				{
					eff0 = GameScr.efs[array[indexSkill].effS0Id - 1];
					i0 = (dx0 = (dy0 = 0));
				}
				if (array[indexSkill].effS1Id != 0)
				{
					eff1 = GameScr.efs[array[indexSkill].effS1Id - 1];
					i1 = (dx1 = (dy1 = 0));
				}
				if (array[indexSkill].effS2Id != 0)
				{
					eff2 = GameScr.efs[array[indexSkill].effS2Id - 1];
					i2 = (dx2 = (dy2 = 0));
				}
				SkillInfoPaint[] array2 = array;
				int num = indexSkill;
				if (array2 != null && array2[num] != null && num >= 0 && num <= array2.Length - 1 && array2[num].arrowId != 0)
				{
					int arrowId = array2[num].arrowId;
					if (arrowId >= 100)
					{
						object obj;
						if (mobFocus == null)
						{
							IMapObject mapObject = charFocus;
							obj = mapObject;
						}
						else
						{
							obj = mobFocus;
						}
						IMapObject mapObject2 = (IMapObject)obj;
						if (mapObject2 != null)
						{
							int num2 = 0;
							int num3 = Res.abs(mapObject2.getX() - cx);
							int num4 = Res.abs(mapObject2.getY() - cy);
							if (num3 > 4 * num4)
							{
								num2 = 0;
							}
							else
							{
								num2 = ((mapObject2.getY() >= cy) ? 3 : (-3));
								if (mapObject2 is BigBoss)
								{
									BigBoss bigBoss = (BigBoss)mapObject2;
									if (bigBoss.haftBody)
									{
										num2 = -20;
									}
								}
							}
							dart = new PlayerDart(this, arrowId - 100, skillPaintRandomPaint, cx + (array2[num].adx - 10) * cdir, cy + array2[num].ady + num2);
							if (myskill != null)
							{
								if (myskill.template.id == 1)
								{
									SoundMn.gI().traidatKame();
								}
								else if (myskill.template.id == 3)
								{
									SoundMn.gI().namekKame();
								}
								else if (myskill.template.id == 5)
								{
									SoundMn.gI().xaydaKame();
								}
								else if (myskill.template.id == 11)
								{
									SoundMn.gI().nameLazer();
								}
							}
						}
						else if (isFlyAndCharge || isUseSkillAfterCharge)
						{
							stopUseChargeSkill();
						}
					}
					else
					{
						Res.outz("g");
						arr = new Arrow(this, GameScr.arrs[arrowId - 1]);
						arr.life = 10;
						arr.ax = cx + array2[num].adx;
						arr.ay = cy + array2[num].ady;
					}
				}
				if ((mobFocus != null || (!me && charFocus != null) || (me && charFocus != null && (isMeCanAttackOtherPlayer(charFocus) || isSelectingSkillBuffToPlayer()) && arr == null && dart == null)) && indexSkill == array.Length - 1)
				{
					setAttack();
					if (me && myskill.template.isAttackSkill())
					{
						saveLoadPreviousSkill();
					}
				}
				if (me)
				{
					return;
				}
				IMapObject mapObject3 = null;
				if (mobFocus != null)
				{
					mapObject3 = mobFocus;
				}
				else if (charFocus != null)
				{
					mapObject3 = charFocus;
				}
				if (mapObject3 == null)
				{
					return;
				}
				if (Res.abs(mapObject3.getX() - cx) < 10)
				{
					if (mapObject3.getX() > cx)
					{
						cx -= 10;
					}
					else
					{
						cx += 10;
					}
				}
				if (mapObject3.getX() > cx)
				{
					cdir = 1;
				}
				else
				{
					cdir = -1;
				}
			}

}
