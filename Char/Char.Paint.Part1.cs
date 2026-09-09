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
				if (skillPaint != null && ((charFocus != null && isMeCanAttackOtherPlayer(charFocus) && (charFocus.statusMe == 14 || charFocus.statusMe == 5 || charFocus.cHP <= 0)) || (mobFocus != null && (mobFocus.status == 0 || mobFocus.status == 1 || mobFocus.hp <= 0))))
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
					if ((TileMap.tileTypeAtPixel(cx, cy) & 2) != 2)
					{
						delayFall = 5;
					}
				}
				if (skillPaint != null && arr == null && dart == null && (skillInfoPaint() == null || indexSkill >= skillInfoPaint().Length || indexSkill >= 30))
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
					if ((TileMap.tileTypeAtPixel(cx, cy) & 2) != 2)
					{
						delayFall = 5;
					}
					return;
				}
				SkillInfoPaint[] array = skillInfoPaint();
				if (array == null || array.Length == 0)
				{
					indexSkill = 0;
					skillPaint = null;
					skillPaintRandomPaint = null;
					return;
				}
				if (indexSkill < 0)
				{
					indexSkill = 0;
				}
				int num = (indexSkill < array.Length) ? indexSkill : (array.Length - 1);
				if (array[num].effS0Id != 0 && array[num].effS0Id - 1 >= 0 && array[num].effS0Id - 1 < GameScr.efs.Length)
				{
					eff0 = GameScr.efs[array[num].effS0Id - 1];
					i0 = (dx0 = (dy0 = 0));
				}
				if (array[num].effS1Id != 0 && array[num].effS1Id - 1 >= 0 && array[num].effS1Id - 1 < GameScr.efs.Length)
				{
					eff1 = GameScr.efs[array[num].effS1Id - 1];
					i1 = (dx1 = (dy1 = 0));
				}
				if (array[num].effS2Id != 0 && array[num].effS2Id - 1 >= 0 && array[num].effS2Id - 1 < GameScr.efs.Length)
				{
					eff2 = GameScr.efs[array[num].effS2Id - 1];
					i2 = (dx2 = (dy2 = 0));
				}
				if (array != null && array[num] != null && array[num].arrowId != 0 && dart == null && arr == null)
				{
					int arrowId = array[num].arrowId;
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
							if (arrowId - 100 >= 0 && arrowId - 100 < GameScr.darts.Length && GameScr.darts[arrowId - 100] != null)
							{
								dart = new PlayerDart(this, arrowId - 100, skillPaintRandomPaint, cx + (array[num].adx - 10) * cdir, cy + array[num].ady + num2);
							}
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
					else if (arrowId - 1 >= 0 && arrowId - 1 < GameScr.arrs.Length && GameScr.arrs[arrowId - 1] != null)
					{
						Res.outz("g");
						arr = new Arrow(this, GameScr.arrs[arrowId - 1]);
						arr.life = 10;
						arr.ax = cx + array[num].adx;
						arr.ay = cy + array[num].ady;
					}
				}
				if ((mobFocus != null || (!me && charFocus != null) || (me && charFocus != null && (isMeCanAttackOtherPlayer(charFocus) || isSelectingSkillBuffToPlayer()))) && arr == null && dart == null && num >= array.Length - 1 && !hasSendAttack)
				{
					setAttack();
					if (me && myskill != null && myskill.template != null && myskill.template.isAttackSkill())
					{
						saveLoadPreviousSkill();
					}
				}
				indexSkill++;
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
