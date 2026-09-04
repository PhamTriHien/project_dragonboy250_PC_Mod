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

	public void setSkillPaint(SkillPaint skillPaint, int sType)
			{
				hasSendAttack = false;
				if (stone || (me && myskill.template.id == 9 && cHP <= cHPFull / 10))
				{
					return;
				}
				if (me)
				{
					if (mobFocus == null && charFocus == null)
					{
						stopUseChargeSkill();
					}
					if (mobFocus != null && (mobFocus.status == 1 || mobFocus.status == 0))
					{
						stopUseChargeSkill();
					}
					if (charFocus != null && (charFocus.statusMe == 14 || charFocus.statusMe == 5))
					{
						stopUseChargeSkill();
					}
					if ((myskill.template.id == 23 && ((charFocus != null && charFocus.holdEffID != 0) || (mobFocus != null && mobFocus.holdEffID != 0) || holdEffID != 0)) || sleepEff || blindEff)
					{
						return;
					}
				}
				Res.outz("skill id= " + skillPaint.id);
				if ((me && dart != null) || TileMap.isOfflineMap())
				{
					return;
				}
				long num = mSystem.currentTimeMillis();
				if (me)
				{
					if (isSelectingSkillBuffToPlayer() && charFocus == null)
					{
						return;
					}
					if (num - myskill.lastTimeUseThisSkill < myskill.coolDown)
					{
						myskill.paintCanNotUseSkill = true;
						return;
					}
					myskill.lastTimeUseThisSkill = num;
					if (myskill.template.manaUseType == 2)
					{
						cMP = 1L;
					}
					else if (myskill.template.manaUseType != 1)
					{
						cMP -= myskill.manaUse;
					}
					else
					{
						cMP -= myskill.manaUse * cMPFull / 100;
					}
					myCharz().cStamina--;
					GameScr.gI().isInjureMp = true;
					GameScr.gI().twMp = 0L;
					if (cMP < 0)
					{
						cMP = 0L;
					}
				}
				if (me)
				{
					if (myskill.template.id == 10)
					{
						Service.gI().skill_not_focus(4);
					}
					if (myskill.template.id == 11)
					{
						Service.gI().skill_not_focus(4);
					}
					if (myskill.template.id == 7)
					{
						SoundMn.gI().hoisinh();
					}
					if (myskill.template.id == 6)
					{
						Service.gI().skill_not_focus(0);
						GameScr.gI().isUseFreez = true;
						SoundMn.gI().thaiduonghasan();
					}
					if (myskill.template.id == 8)
					{
						if (!isCharge)
						{
							SoundMn.gI().taitaoPause();
							Service.gI().skill_not_focus(1);
							isCharge = true;
							last = (cur = mSystem.currentTimeMillis());
						}
						else
						{
							Service.gI().skill_not_focus(3);
							isCharge = false;
							SoundMn.gI().taitaoPause();
						}
					}
					if (myskill.template.id == 13)
					{
						if (isMonkey != 0)
						{
							GameScr.gI().auto = 0;
						}
						else if (!isCreateDark)
						{
							SoundMn.gI().gong();
							Service.gI().skill_not_focus(6);
							chargeCount = 0;
							isWaitMonkey = true;
						}
						return;
					}
					if (myskill.template.id == 14)
					{
						SoundMn.gI().gong();
						Service.gI().skill_not_focus(7);
						useChargeSkill(isGround: true);
					}
					if (myskill.template.id == 21)
					{
						Service.gI().skill_not_focus(10);
						return;
					}
					if (myskill.template.id == 12)
					{
						Service.gI().skill_not_focus(8);
					}
					if (myskill.template.id == 19)
					{
						Service.gI().skill_not_focus(9);
						return;
					}
				}
				if (isMonkey == 1 && skillPaint.id >= 35 && skillPaint.id <= 41)
				{
					skillPaint = GameScr.sks[106];
				}
				if (skillPaint.id >= 128 && skillPaint.id <= 134)
				{
					skillPaint = GameScr.sks[skillPaint.id - 65];
					if (charFocus != null)
					{
						cx = charFocus.cx;
						cy = charFocus.cy;
						currentMovePoint = null;
					}
					if (mobFocus != null)
					{
						cx = mobFocus.x;
						cy = mobFocus.y;
						currentMovePoint = null;
					}
					ServerEffect.addServerEffect(60, cx, cy, 1);
					telePortSkill = true;
				}
				if (skillPaint.id >= 107 && skillPaint.id <= 113)
				{
					skillPaint = GameScr.sks[skillPaint.id - 44];
					EffecMn.addEff(new Effect(23, cx, cy + ch / 2, 3, 2, 1));
				}
				setAutoSkillPaint(skillPaint, sType);
			}

	public void setAutoSkillPaint(SkillPaint skillPaint, int sType)
			{
				this.skillPaint = skillPaint;
				Res.outz("set auto skill " + ((skillPaint == null) ? "null" : "ko null"));
				if (skillPaint.id >= 0 && skillPaint.id <= 6)
				{
					int num = Res.random(0, skillPaint.id + 4) - 1;
					if (num < 0)
					{
						num = 0;
					}
					if (num > 6)
					{
						num = 6;
					}
					skillPaintRandomPaint = GameScr.sks[num];
				}
				else if (skillPaint.id >= 14 && skillPaint.id <= 20)
				{
					int num2 = Res.random(0, skillPaint.id - 14 + 4) - 1;
					if (num2 < 0)
					{
						num2 = 0;
					}
					if (num2 > 6)
					{
						num2 = 6;
					}
					skillPaintRandomPaint = GameScr.sks[num2 + 14];
				}
				else if (skillPaint.id >= 28 && skillPaint.id <= 34)
				{
					int num3 = Res.random(0, ((isMonkey != 1) ? skillPaint.id : 105) - ((isMonkey != 1) ? 28 : 105) + 4) - 1;
					if (num3 < 0)
					{
						num3 = 0;
					}
					if (num3 > 6)
					{
						num3 = 6;
					}
					if (isMonkey == 1)
					{
						num3 = 0;
					}
					skillPaintRandomPaint = GameScr.sks[num3 + ((isMonkey != 1) ? 28 : 105)];
				}
				else if (skillPaint.id >= 63 && skillPaint.id <= 69)
				{
					int num4 = Res.random(0, skillPaint.id - 63 + 4) - 1;
					if (num4 < 0)
					{
						num4 = 0;
					}
					if (num4 > 6)
					{
						num4 = 6;
					}
					skillPaintRandomPaint = GameScr.sks[num4 + 63];
				}
				else if (skillPaint.id >= 107 && skillPaint.id <= 109)
				{
					int num5 = Res.random(0, skillPaint.id - 107 + 4) - 1;
					if (num5 < 0)
					{
						num5 = 0;
					}
					if (num5 > 6)
					{
						num5 = 6;
					}
					skillPaintRandomPaint = GameScr.sks[num5 + 107];
				}
				else
				{
					skillPaintRandomPaint = skillPaint;
				}
				this.sType = sType;
				indexSkill = 0;
				i0 = (i1 = (i2 = (dx0 = (dx1 = (dx2 = (dy0 = (dy1 = (dy2 = 0))))))));
				eff0 = null;
				eff1 = null;
				eff2 = null;
				cvy = 0;
			}

	public bool isPaint()
			{
				if (cy < GameScr.cmy)
				{
					return false;
				}
				if (cy > GameScr.cmy + GameScr.gH + 30)
				{
					return false;
				}
				if (isOutX())
				{
					return false;
				}
				if (isSetPos)
				{
					return false;
				}
				if (isFusion)
				{
					return false;
				}
				return true;
			}

	private void paint_map_line(mGraphics g)
			{
				if (isPaintNewSkill || x_hint == 0 || y_hint == 0 || statusMe == 14)
				{
					return;
				}
				int arg = 0;
				int x = cx - 30;
				int y = cy - 15;
				int num = -30;
				int num2 = 5;
				if (Res.abs(cy - y_hint) > 150)
				{
					if (cy > y_hint)
					{
						arg = 7;
						x = cx;
						y = cy - 15 - 60;
					}
					else
					{
						arg = 5;
						x = cx;
						y = cy - 15 + 60;
					}
				}
				else if (cx > x_hint)
				{
					arg = 2;
				}
				else if (cx <= x_hint)
				{
					x = cx + 30;
				}
				if (GameCanvas.gameTick % 10 >= 5)
				{
					if (Res.abs(cx - x_hint) > 100)
					{
						g.drawRegion(GameScr.arrow, 0, 0, 13, 16, arg, x, y, StaticObj.VCENTER_HCENTER);
					}
					else if (Res.abs(cx - x_hint) < 50)
					{
						g.drawImage(Panel.imgBantay, x_hint + num, y_hint - 60 + num2, 0);
					}
				}
			}

	private void paintArrowAttack(mGraphics g)
			{
			}

	public void paintHp(mGraphics g, int x, int y)
			{
				int num = (int)((int)cHP * 100 / cHPFull) / 10 - 1;
				if (num < 0)
				{
					num = 0;
				}
				if (num > 9)
				{
					num = 9;
				}
				if (!me)
				{
					g.drawRegion(Mob.imgHP, 0, 6 * (9 - num), 9, 6, 0, x, y - mFont.tahoma_7.getHeight() - 6, 3);
				}
				if (cTypePk == 0 && (myCharz().cFlag == 0 || cFlag == 0 || (cFlag != 8 && myCharz().cFlag != 8 && cFlag == myCharz().cFlag)))
				{
					return;
				}
				len = (int)(cHP * 100 / cHPFull * w_hp_bar) / 100;
				num = (int)(cHP * 100 / cHPFull);
				if (num < 30)
				{
					imgHPtem = GameScr.imgHP_tm_do;
				}
				else if (num < 60)
				{
					imgHPtem = GameScr.imgHP_tm_vang;
				}
				else
				{
					imgHPtem = GameScr.imgHP_tm_xanh;
				}
				int imageWidth = mGraphics.getImageWidth(GameScr.imgHP_tm_xam);
				int imageHeight = mGraphics.getImageHeight(GameScr.imgHP_tm_xam);
				int w = imageWidth * num / 100;
				g.drawImage(GameScr.imgHP_tm_xam, x - (imageWidth >> 1), y - 1, mGraphics.TOP | mGraphics.LEFT);
				if (len < 5)
				{
					if (GameCanvas.gameTick % 6 < 3)
					{
						g.drawRegion(imgHPtem, 0, 0, w, imageHeight, 0, x - (imageWidth >> 1), y - 1, mGraphics.TOP | mGraphics.LEFT);
					}
				}
				else
				{
					g.drawRegion(imgHPtem, 0, 0, w, imageHeight, 0, x - (imageWidth >> 1), y - 1, mGraphics.TOP | mGraphics.LEFT);
				}
			}

	public void paintNameInSameParty(mGraphics g)
			{
				if (cTypePk != 3 && cTypePk != 5 && isPaint())
				{
					if (myCharz().charFocus == null || !myCharz().charFocus.Equals(this))
					{
						mFont.tahoma_7_yellow.drawString(g, cName, cx, cy - ch - mFont.tahoma_7_green.getHeight() - 5, mFont.CENTER, mFont.tahoma_7_grey);
					}
					else if (myCharz().charFocus != null && myCharz().charFocus.Equals(this))
					{
						mFont.tahoma_7_yellow.drawString(g, cName, cx, cy - ch - mFont.tahoma_7_green.getHeight() - 10, mFont.CENTER, mFont.tahoma_7_grey);
					}
				}
			}

	private void paintCharWithoutSkill(mGraphics g)
			{
				try
				{
					if (isMafuba)
					{
						paintCharBody(g, xMFB, yMFB, cdir, cf, isPaintBag: false);
						return;
					}
					if (isInvisiblez)
					{
						if (me)
						{
							if (GameCanvas.gameTick % 50 == 48 || GameCanvas.gameTick % 50 == 90)
							{
								SmallImage.drawSmallImage(g, 1196, cx, cy - 18, 0, mGraphics.VCENTER | mGraphics.HCENTER);
							}
							else
							{
								SmallImage.drawSmallImage(g, 1195, cx, cy - 18, 0, mGraphics.VCENTER | mGraphics.HCENTER);
							}
						}
					}
					else
					{
						paintCharBody(g, cx, cy + fy, cdir, cf, isPaintBag: true);
					}
					if (isLockAttack)
					{
						SmallImage.drawSmallImage(g, 290, cx, cy, 0, mGraphics.BOTTOM | mGraphics.HCENTER);
					}
				}
				catch (Exception ex)
				{
					Cout.LogError("Loi paint char without skill: " + ex.ToString());
				}
			}

	public void paintCharWithSkill(mGraphics g)
			{
				ty = 0;
				SkillInfoPaint[] array = skillInfoPaint();
				cf = array[indexSkill].status;
				paintCharWithoutSkill(g);
				if (cdir == 1)
				{
					if (eff0 != null)
					{
						if (dx0 == 0)
						{
							dx0 = array[indexSkill].e0dx;
						}
						if (dy0 == 0)
						{
							dy0 = array[indexSkill].e0dy;
						}
						SmallImage.drawSmallImage(g, eff0.arrEfInfo[i0].idImg, cx + dx0 + eff0.arrEfInfo[i0].dx, cy + dy0 + eff0.arrEfInfo[i0].dy, 0, mGraphics.VCENTER | mGraphics.HCENTER);
						i0++;
						if (i0 >= eff0.arrEfInfo.Length)
						{
							eff0 = null;
							i0 = (dx0 = (dy0 = 0));
						}
					}
					if (eff1 != null)
					{
						if (dx1 == 0)
						{
							dx1 = array[indexSkill].e1dx;
						}
						if (dy1 == 0)
						{
							dy1 = array[indexSkill].e1dy;
						}
						SmallImage.drawSmallImage(g, eff1.arrEfInfo[i1].idImg, cx + dx1 + eff1.arrEfInfo[i1].dx, cy + dy1 + eff1.arrEfInfo[i1].dy, 0, mGraphics.VCENTER | mGraphics.HCENTER);
						i1++;
						if (i1 >= eff1.arrEfInfo.Length)
						{
							eff1 = null;
							i1 = (dx1 = (dy1 = 0));
						}
					}
					if (eff2 != null)
					{
						if (dx2 == 0)
						{
							dx2 = array[indexSkill].e2dx;
						}
						if (dy2 == 0)
						{
							dy2 = array[indexSkill].e2dy;
						}
						SmallImage.drawSmallImage(g, eff2.arrEfInfo[i2].idImg, cx + dx2 + eff2.arrEfInfo[i2].dx, cy + dy2 + eff2.arrEfInfo[i2].dy, 0, mGraphics.VCENTER | mGraphics.HCENTER);
						i2++;
						if (i2 >= eff2.arrEfInfo.Length)
						{
							eff2 = null;
							i2 = (dx2 = (dy2 = 0));
						}
					}
				}
				else
				{
					if (eff0 != null)
					{
						if (dx0 == 0)
						{
							dx0 = array[indexSkill].e0dx;
						}
						if (dy0 == 0)
						{
							dy0 = array[indexSkill].e0dy;
						}
						SmallImage.drawSmallImage(g, eff0.arrEfInfo[i0].idImg, cx - dx0 - eff0.arrEfInfo[i0].dx, cy + dy0 + eff0.arrEfInfo[i0].dy, 2, mGraphics.VCENTER | mGraphics.HCENTER);
						i0++;
						if (i0 >= eff0.arrEfInfo.Length)
						{
							eff0 = null;
							i0 = 0;
							dx0 = 0;
							dy0 = 0;
						}
					}
					if (eff1 != null)
					{
						if (dx1 == 0)
						{
							dx1 = array[indexSkill].e1dx;
						}
						if (dy1 == 0)
						{
							dy1 = array[indexSkill].e1dy;
						}
						SmallImage.drawSmallImage(g, eff1.arrEfInfo[i1].idImg, cx - dx1 - eff1.arrEfInfo[i1].dx, cy + dy1 + eff1.arrEfInfo[i1].dy, 2, mGraphics.VCENTER | mGraphics.HCENTER);
						i1++;
						if (i1 >= eff1.arrEfInfo.Length)
						{
							eff1 = null;
							i1 = 0;
							dx1 = 0;
							dy1 = 0;
						}
					}
					if (eff2 != null)
					{
						if (dx2 == 0)
						{
							dx2 = array[indexSkill].e2dx;
						}
						if (dy2 == 0)
						{
							dy2 = array[indexSkill].e2dy;
						}
						SmallImage.drawSmallImage(g, eff2.arrEfInfo[i2].idImg, cx - dx2 - eff2.arrEfInfo[i2].dx, cy + dy2 + eff2.arrEfInfo[i2].dy, 2, mGraphics.VCENTER | mGraphics.HCENTER);
						i2++;
						if (i2 >= eff2.arrEfInfo.Length)
						{
							eff2 = null;
							i2 = 0;
							dx2 = 0;
							dy2 = 0;
						}
					}
				}
				indexSkill++;
			}

	private void paintPKFlag(mGraphics g)
			{
				if (cdir == 1)
				{
					if (cFlag != 0 && cFlag != -1)
					{
						SmallImage.drawSmallImage(g, flagImage, cx - 10, cy - ch - ((!me) ? 30 : 30) + ((GameCanvas.gameTick % 20 > 10) ? (GameCanvas.gameTick % 4 / 2) : 0), 2, 0);
					}
				}
				else if (cFlag != 0 && cFlag != -1)
				{
					SmallImage.drawSmallImage(g, flagImage, cx, cy - ch - ((!me) ? 30 : 30) + ((GameCanvas.gameTick % 20 > 10) ? (GameCanvas.gameTick % 4 / 2) : 0), 0, 0);
				}
			}

	public void paintHat_behind(mGraphics g, int cf, int yh)
			{
				try
				{
					if (idHat == -1)
					{
						return;
					}
					if (isFrNgang(cf))
					{
						if (fraHat_behind_2 != null)
						{
							fraHat_behind_2.drawFrame(GameCanvas.gameTick / 4 % fraHat_behind_2.nFrame, cx + hatInfo[cf][0] * ((cdir == 1) ? 1 : (-1)), yh + hatInfo[cf][1], (cdir != 1) ? 2 : 0, mGraphics.BOTTOM | mGraphics.HCENTER, g);
						}
						else
						{
							fraHat_behind_2 = mSystem.getFraImage(strHat_behind + strNgang + idHat);
						}
					}
					else if (fraHat_behind != null)
					{
						fraHat_behind.drawFrame(GameCanvas.gameTick / 4 % fraHat_behind.nFrame, cx + hatInfo[cf][0] * ((cdir == 1) ? 1 : (-1)), yh + hatInfo[cf][1], (cdir != 1) ? 2 : 0, mGraphics.BOTTOM | mGraphics.HCENTER, g);
					}
					else
					{
						fraHat_behind = mSystem.getFraImage(strHat_behind + idHat);
					}
				}
				catch (Exception)
				{
				}
			}

	public void paintHat_front(mGraphics g, int cf, int yh)
			{
				try
				{
					if (idHat == -1)
					{
						return;
					}
					if (isFrNgang(cf))
					{
						if (fraHat_font_2 != null)
						{
							fraHat_font_2.drawFrame(GameCanvas.gameTick / 4 % fraHat_font_2.nFrame, cx + hatInfo[cf][0] * ((cdir == 1) ? 1 : (-1)), yh + hatInfo[cf][1], (cdir != 1) ? 2 : 0, mGraphics.BOTTOM | mGraphics.HCENTER, g);
						}
						else
						{
							fraHat_font_2 = mSystem.getFraImage(strHat_font + strNgang + idHat);
						}
					}
					else if (fraHat_font != null)
					{
						fraHat_font.drawFrame(GameCanvas.gameTick / 4 % fraHat_font.nFrame, cx + hatInfo[cf][0] * ((cdir == 1) ? 1 : (-1)), yh + hatInfo[cf][1], (cdir != 1) ? 2 : 0, mGraphics.BOTTOM | mGraphics.HCENTER, g);
					}
					else
					{
						fraHat_font = mSystem.getFraImage(strHat_font + idHat);
					}
				}
				catch (Exception)
				{
				}
			}

	public void SetSkillPaint_NEW(short idskillPaint, bool isFly, sbyte typeFrame, sbyte typePaint, sbyte dir, short timeGong, sbyte typeItem)
			{
				isPaintNewSkill = true;
				timeReset_newSkill = GameCanvas.timeNow + 10000;
				this.idskillPaint = idskillPaint;
				this.isFly = isFly;
				this.typeFrame = typeFrame;
				this.typePaint = typePaint;
				this.typeItem = typeItem;
				cdir = dir;
				count_NEW = 0;
				stt = 0;
				long lastTimeUseThisSkill = mSystem.currentTimeMillis();
				if (me)
				{
					saveLoadPreviousSkill();
					myskill.lastTimeUseThisSkill = lastTimeUseThisSkill;
					if (myskill.template.manaUseType == 2)
					{
						cMP = 1L;
					}
					else if (myskill.template.manaUseType != 1)
					{
						cMP -= myskill.manaUse;
					}
					else
					{
						cMP -= myskill.manaUse * cMPFull / 100;
					}
					myCharz().cStamina--;
					GameScr.gI().isInjureMp = true;
					GameScr.gI().twMp = 0L;
					if (cMP < 0)
					{
						cMP = 0L;
					}
				}
				switch (idskillPaint)
				{
				case 24:
					GameScr.addEffectEnd_Target(18, 0, typePaint, clone(), null, 3, timeGong, 0);
					GameScr.addEffectEnd_Target(21, 0, typePaint, clone(), null, 1, timeGong, 0);
					break;
				case 25:
					GameScr.addEffectEnd_Target(19, 0, typePaint, clone(), null, 3, timeGong, 0);
					GameScr.addEffectEnd_Target(22, 0, typePaint, clone(), null, 1, timeGong, 0);
					break;
				case 26:
					GameScr.addEffectEnd_Target(20, 0, typePaint, clone(), null, 3, timeGong, 0);
					GameScr.addEffectEnd_Target(23, 0, typePaint, clone(), null, 1, timeGong, 0);
					break;
				}
				if (this.typeFrame == 1)
				{
					if (!this.isFly)
					{
						fr_start = new byte[7] { 20, 20, 20, 20, 20, 20, 19 };
						fr_atk = new byte[1] { 20 };
						fr_end = new byte[1];
					}
					else
					{
						fr_start = new byte[7] { 31, 31, 31, 31, 31, 31, 30 };
						fr_atk = new byte[1] { 31 };
						fr_end = new byte[1] { 12 };
					}
				}
				if (this.typeFrame == 2)
				{
					if (!this.isFly)
					{
						fr_start = new byte[1] { 20 };
						fr_atk = new byte[6] { 13, 13, 13, 14, 14, 14 };
						fr_end = new byte[1];
					}
					else
					{
						fr_start = new byte[1] { 31 };
						fr_atk = new byte[6] { 26, 26, 26, 27, 27, 27 };
						fr_end = new byte[1] { 12 };
					}
				}
				if (this.typeFrame == 4)
				{
					if (!this.isFly)
					{
						fr_start = new byte[6] { 17, 17, 17, 18, 18, 18 };
						fr_atk = new byte[1] { 18 };
						fr_end = new byte[1];
					}
					else
					{
						fr_start = new byte[7] { 7, 7, 7, 12, 12, 12, 12 };
						fr_atk = new byte[1] { 12 };
						fr_end = new byte[1] { 12 };
					}
				}
				if (this.typeFrame == 3)
				{
					if (!this.isFly)
					{
						fr_start = new byte[9] { 24, 24, 24, 17, 17, 17, 18, 18, 18 };
						fr_atk = new byte[1] { 20 };
						fr_end = new byte[1];
					}
					else
					{
						fr_start = new byte[10] { 23, 23, 23, 7, 7, 7, 12, 12, 12, 12 };
						fr_atk = new byte[1] { 31 };
						fr_end = new byte[1] { 12 };
					}
				}
			}

	public void SetSkillPaint_STT(int stt, short idskillPaint, Point targetDame, short timeDame, short rangeDame, sbyte typePaint, Point[] listObj, sbyte typeItem)
			{
				this.stt = stt;
				this.idskillPaint = idskillPaint;
				count_NEW = 0;
				this.targetDame = targetDame;
				this.typePaint = typePaint;
				this.timeDame = mSystem.currentTimeMillis() + timeDame;
				this.rangeDame = rangeDame;
				this.typeItem = typeItem;
				if (this.stt == 1)
				{
					if (this.idskillPaint == 24)
					{
						GameScr.addEffectEnd_Target(18, 1, typePaint, this, null, 3, timeDame, 0);
						GameScr.addEffectEnd_Target(24, 0, typePaint, this, this.targetDame, 1, timeDame, rangeDame);
					}
					if (this.idskillPaint == 25)
					{
						GameScr.addEffectEnd_Target(19, 0, typePaint, this, null, 3, timeDame, 0);
						GameScr.addEffectEnd_Target(25, 0, typePaint, this, this.targetDame, 1, timeDame, rangeDame);
					}
					if (this.idskillPaint == 26)
					{
						GameScr.addEffectEnd_Target(20, 0, typePaint, this, null, 3, timeDame, 0);
						GameScr.addEffectEnd(26, typeItem, typePaint, targetDame.x, targetDame.y, 1, 0, timeDame, listObj);
					}
				}
			}

	public void UpdSkillPaint_NEW()
			{
				if (stt == 0)
				{
					if (isFly && count_NEW < 20)
					{
						cvy = -3;
						cy += cvy;
					}
					if (fr_start.Length == 1)
					{
						cf = fr_start[0];
					}
					else if (count_NEW > fr_start.Length - 1)
					{
						cf = fr_start[fr_start.Length - 1];
					}
					else
					{
						cf = fr_start[count_NEW];
					}
				}
				else if (stt == 1)
				{
					cf = fr_atk[count_NEW % fr_atk.Length];
					if (mSystem.currentTimeMillis() - timeDame > 0)
					{
						SetSkillPaint_STT(2, 0, null, 0, 0, 0, null, 0);
					}
					if (count_NEW % 5 == 0)
					{
						GameScr.shock_scr = 5;
					}
					if (typeFrame == 1 && count_NEW < 10 && !TileMap.tileTypeAt(cx - (chw + 1) * cdir, cy, (cdir != 1) ? 4 : 8))
					{
						cx -= cdir;
					}
					if (typeFrame != 2)
					{
					}
				}
				else if (stt == 2)
				{
					if (fr_end.Length == 1)
					{
						cf = fr_end[0];
					}
					else if (count_NEW > fr_end.Length - 1)
					{
						cf = fr_end[fr_end.Length - 1];
					}
					else
					{
						cf = fr_end[count_NEW];
					}
					if (isFly)
					{
						cvx = (cvy = 0);
						statusMe = 4;
					}
					isPaintNewSkill = false;
				}
				count_NEW++;
			}

}
