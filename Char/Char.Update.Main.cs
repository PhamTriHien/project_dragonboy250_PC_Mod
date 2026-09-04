using System;
using Assets.src.e;
using Assets.src.g;
using UnityEngine;

public partial class Char : IMapObject
{
	public virtual void update()
			{
				if (isMafuba)
				{
					cf = 23;
					countMafuba++;
					if (countMafuba > 150)
					{
						isMafuba = false;
					}
					return;
				}
				countMafuba = 0;
				if (isHide || isMabuHold)
				{
					return;
				}
				if ((!isCopy && clevel < 14) || statusMe == 1 || statusMe == 6)
				{
				}
				if (petFollow != null)
				{
					if (GameCanvas.gameTick % 3 == 0)
					{
						if (myCharz().cdir == 1)
						{
							petFollow.cmtoX = cx - 20;
						}
						if (myCharz().cdir == -1)
						{
							petFollow.cmtoX = cx + 20;
						}
						petFollow.cmtoY = cy - 40;
						if (petFollow.cmx > cx)
						{
							petFollow.dir = -1;
						}
						else
						{
							petFollow.dir = 1;
						}
						if (petFollow.cmtoX < 100)
						{
							petFollow.cmtoX = 100;
						}
						if (petFollow.cmtoX > TileMap.pxw - 100)
						{
							petFollow.cmtoX = TileMap.pxw - 100;
						}
					}
					petFollow.update();
				}
				if (!me && cHP <= 0 && clanID != -100 && statusMe != 14 && statusMe != 5)
				{
					startDie((short)cx, (short)cy);
				}
				if (isInjureHp)
				{
					twHp++;
					if (twHp == 20)
					{
						twHp = 0;
						isInjureHp = false;
					}
				}
				else if (dHP > cHP)
				{
					long num = dHP - cHP >> 1;
					if (num < 1)
					{
						num = 1L;
					}
					dHP -= num;
				}
				else
				{
					dHP = cHP;
				}
				if (secondPower != 0)
				{
					currS = mSystem.currentTimeMillis();
					if (currS - lastS >= 1000)
					{
						lastS = mSystem.currentTimeMillis();
						secondPower--;
					}
				}
				if (isPaintNewSkill)
				{
					if (GameCanvas.timeNow > timeReset_newSkill || statusMe == 14 || statusMe == 5)
					{
						timeReset_newSkill = 0L;
						isPaintNewSkill = false;
					}
					UpdSkillPaint_NEW();
					if (isShadown)
					{
						updateShadown();
					}
				}
				else
				{
					if (!me && GameScr.notPaint)
					{
						return;
					}
					if (sleepEff && GameCanvas.gameTick % 10 == 0)
					{
						EffecMn.addEff(new Effect(41, cx, cy, 3, 1, 1));
					}
					if (huytSao)
					{
						huytSao = false;
						EffecMn.addEff(new Effect(39, cx, cy, 3, 3, 1));
					}
					if (blindEff && GameCanvas.gameTick % 5 == 0)
					{
						ServerEffect.addServerEffect(113, this, 1);
					}
					if (protectEff)
					{
						int y = cH_new + 73;
						if (GameCanvas.gameTick % 5 == 0)
						{
							eProtect = new Effect(33, cx, y, 3, 3, 1);
						}
						if (eProtect != null)
						{
							eProtect.update();
							eProtect.x = cx;
							eProtect.y = y;
						}
					}
					if (danhHieuEff)
					{
						if (eDanhHieu == null)
						{
							string text = (string)GameCanvas.danhHieu.get(charID + string.Empty);
							if (text != null)
							{
								string[] array = Res.split(text.Trim(), ",", 0);
								short id = short.Parse(array[0]);
								short num2 = short.Parse(array[1]);
								eDanhHieu = new Effect(id, cx, cH_new + 73, 1, -1, -1);
								eDanhHieu.timeExist = num2 * 1000 + mSystem.currentTimeMillis();
							}
						}
						if (eDanhHieu != null)
						{
							eDanhHieu.update();
							eDanhHieu.x = cx;
							eDanhHieu.y = cH_new;
							if (eDanhHieu.timeExist <= mSystem.currentTimeMillis())
							{
								eDanhHieu = null;
								GameCanvas.danhHieu.remove(charID + string.Empty);
							}
						}
					}
					if (charFocus != null && charFocus.cy < 0)
					{
						charFocus = null;
					}
					if (isFusion)
					{
						tFusion++;
					}
					if (isNhapThe)
					{
						int num3 = 0;
						if (GameCanvas.gameTick % 25 == 0)
						{
							num3 = 114;
							ServerEffect.addServerEffect(num3, this, 1);
						}
					}
					if (isSetPos)
					{
						tpos++;
						if (tpos != 1)
						{
							return;
						}
						tpos = 0;
						isSetPos = false;
						cx = xPos;
						cy = yPos;
						cp1 = (cp2 = (cp3 = 0));
						if (typePos == 1)
						{
							if (me)
							{
								cxSend = cx;
								cySend = cy;
							}
							currentMovePoint = null;
							telePortSkill = false;
							ServerEffect.addServerEffect(173, cx, cy, 1);
						}
						else
						{
							ServerEffect.addServerEffect(60, cx, cy, 1);
						}
						if ((TileMap.tileTypeAtPixel(cx, cy) & 2) == 2)
						{
							statusMe = 1;
						}
						else
						{
							statusMe = 4;
						}
						return;
					}
					soundUpdate();
					if (stone)
					{
						return;
					}
					if (isFreez)
					{
						if (GameCanvas.gameTick % 5 == 0)
						{
							ServerEffect.addServerEffect(113, cx, cy, 1);
						}
						cf = 23;
						long num4 = mSystem.currentTimeMillis();
						if (num4 - lastFreez >= 1000)
						{
							freezSeconds--;
							lastFreez = num4;
							if (freezSeconds < 0)
							{
								isFreez = false;
								seconds = 0;
								if (me)
								{
									myCharz().isLockMove = false;
									GameScr.gI().dem = 0;
									GameScr.gI().isFreez = false;
								}
							}
						}
						if (TileMap.tileTypeAt(cx / TileMap.size, cy / TileMap.size) == 0)
						{
							ty++;
							wt++;
							fy += ((!wy) ? 1 : (-1));
							if (wt == 10)
							{
								wt = 0;
								wy = !wy;
							}
						}
						return;
					}
					if (isWaitMonkey)
					{
						isLockMove = true;
						cf = 17;
						if (GameCanvas.gameTick % 5 == 0)
						{
							ServerEffect.addServerEffect(154, cx, cy - 10, 2);
						}
						if (GameCanvas.gameTick % 5 == 0)
						{
							ServerEffect.addServerEffect(1, cx, cy + 10, 1);
						}
						chargeCount++;
						if (chargeCount == 500)
						{
							isWaitMonkey = false;
							isLockMove = false;
						}
						return;
					}
					if (isStandAndCharge)
					{
						chargeCount++;
						bool flag = !TileMap.tileTypeAt(myCharz().cx, myCharz().cy, 2);
						updateEffect();
						updateSkillPaint();
						moveFast = null;
						currentMovePoint = null;
						cf = 17;
						if (flag && cgender != 2)
						{
							cf = 12;
						}
						if (cgender == 2)
						{
							if (TileMap.mapID == 170)
							{
								if (GameCanvas.gameTick % 4 == 0)
								{
								}
								if (GameCanvas.gameTick % 2 == 0)
								{
									if (cdir == 1)
									{
										ServerEffect.addServerEffect(70, cx - 18, cy - ch / 2 + 8, 1);
										ServerEffect.addServerEffect(70, cx + 23, cy - ch / 2 + 15, 1);
									}
									else
									{
										ServerEffect.addServerEffect(70, cx + 18, cy - ch / 2 + 8, 1);
										ServerEffect.addServerEffect(70, cx - 23, cy - ch / 2 + 15, 1);
									}
								}
							}
							else
							{
								if (GameCanvas.gameTick % 3 == 0)
								{
									ServerEffect.addServerEffect(154, cx, cy - ch / 2 + 10, 1);
								}
								if (GameCanvas.gameTick % 5 == 0)
								{
									ServerEffect.addServerEffect(114, cx + Res.random(-20, 20), cy + Res.random(-20, 20), 1);
								}
							}
						}
						if (cgender == 1)
						{
							if (GameCanvas.gameTick % 4 == 0)
							{
							}
							if (GameCanvas.gameTick % 2 == 0)
							{
								if (cdir == 1)
								{
									ServerEffect.addServerEffect(70, cx - 18, cy - ch / 2 + 8, 1);
									ServerEffect.addServerEffect(70, cx + 23, cy - ch / 2 + 15, 1);
								}
								else
								{
									ServerEffect.addServerEffect(70, cx + 18, cy - ch / 2 + 8, 1);
									ServerEffect.addServerEffect(70, cx - 23, cy - ch / 2 + 15, 1);
								}
							}
						}
						if (cgender == 0 && GameCanvas.gameTick % 2 == 0)
						{
							if (cdir == 1)
							{
								ServerEffect.addServerEffect(70, cx - 18, cy - ch / 2 + 8, 1);
								ServerEffect.addServerEffect(70, cx + 23, cy - ch / 2 + 15, 1);
							}
							else
							{
								ServerEffect.addServerEffect(70, cx + 18, cy - ch / 2 + 8, 1);
								ServerEffect.addServerEffect(70, cx - 23, cy - ch / 2 + 15, 1);
							}
						}
						cur = mSystem.currentTimeMillis();
						Res.outz("  7.5 gong namekLazer " + cName + "_" + cgender);
						if (cur - last > seconds || cur - last > 10000)
						{
							Res.outz("<*> 8  namekLazer gong xong " + cName);
							stopUseChargeSkill();
							isStandAndCharge = false;
							int skillId = myskill.skillId;
							if (me)
							{
								if (cgender == 2)
								{
									Res.outz("<*> 9 [me] xay da xong  " + myCharz().myskill.skillId);
									myCharz().setSkillPaint(GameScr.sks[myCharz().myskill.skillId], flag ? 1 : 0);
								}
								if (cgender == 1)
								{
									Res.outz("<*> 9 [me] namec xong " + myCharz().myskill.skillId);
									isCreateDark = true;
									myCharz().setSkillPaint(GameScr.sks[myCharz().myskill.skillId], flag ? 1 : 0);
								}
								if (cgender == 0)
								{
									Res.outz("<*> 9 [me] namec xong " + myCharz().myskill.skillId);
									myCharz().setSkillPaint(GameScr.sks[myCharz().myskill.skillId], flag ? 1 : 0);
								}
								if (myCharz().myskill.skillId >= 77 && myCharz().myskill.skillId <= 83)
								{
									Service.gI().skill_not_focus(4);
								}
								skillId = myCharz().myskill.skillId;
							}
							else
							{
								if (cgender == 2)
								{
									setSkillPaint(GameScr.sks[skillTemplateId], flag ? 1 : 0);
									Res.outz("<*> 10 xay da xong 111   " + skillTemplateId);
								}
								if (cgender == 1)
								{
									setSkillPaint(GameScr.sks[skillTemplateId], flag ? 1 : 0);
									Res.outz("<*> 10 C_NAMEC xong 222   " + skillTemplateId);
								}
								if (cgender == 0)
								{
									setSkillPaint(GameScr.sks[skillTemplateId], flag ? 1 : 0);
									Res.outz("<*> 10  C_TRAIDAT xong 333   " + skillTemplateId);
								}
								skillId = skillTemplateId;
							}
							if (cgender == 2 && statusMe != 14 && statusMe != 5 && (skillId < 77 || skillId > 83))
							{
								GameScr.gI().activeSuperPower(cx, cy);
							}
							Res.outz("<*> 11 Hoàn thành skill not focus -  STAND");
						}
						chargeCount++;
						if (chargeCount == 500)
						{
							stopUseChargeSkill();
						}
						return;
					}
					if (isFlyAndCharge)
					{
						updateEffect();
						updateSkillPaint();
						moveFast = null;
						currentMovePoint = null;
						posDisY++;
						if (TileMap.tileTypeAt(cx, cy - ch, 8192))
						{
							stopUseChargeSkill();
							return;
						}
						if (posDisY == 20)
						{
							last = mSystem.currentTimeMillis();
						}
						if (posDisY > 20)
						{
							cur = mSystem.currentTimeMillis();
							if (cur - last > seconds || cur - last > 10000)
							{
								Res.outz("<*> 12 kết thúc skill  qua cau kinh khi \tFLY " + cName);
								isFlyAndCharge = false;
								if (me)
								{
									isCreateDark = true;
									bool flag2 = TileMap.tileTypeAt(myCharz().cx, myCharz().cy, 2);
									isUseSkillAfterCharge = true;
									setSkillPaint(GameScr.sks[myCharz().myskill.skillId], (!flag2) ? 1 : 0);
								}
								else if (TileMap.mapID == 170)
								{
									isCreateDark = true;
									isUseSkillAfterCharge = true;
									bool flag3 = TileMap.tileTypeAt(cx, cy, 2);
									setSkillPaint(GameScr.sks[skillTemplateId], (!flag3) ? 1 : 0);
								}
							}
							else
							{
								cf = 32;
								if (cgender == 0 && GameCanvas.gameTick % 3 == 0)
								{
									ServerEffect.addServerEffect(153, cx, cy - ch, 2);
								}
								if (TileMap.mapID == 170 && (cgender == 2 || cgender == 1) && GameCanvas.gameTick % 3 == 0)
								{
									ServerEffect.addServerEffect(153, cx, cy - ch, 2);
								}
								chargeCount++;
								if (chargeCount == 500)
								{
									stopUseChargeSkill();
								}
							}
						}
						else
						{
							if (statusMe != 14)
							{
								statusMe = 3;
							}
							cvy = -3;
							cy += cvy;
							cf = 7;
						}
						return;
					}
					if (me && GameCanvas.isTouch)
					{
						if (charFocus != null && charFocus.charID >= 0 && charFocus.cx > 100 && charFocus.cx < TileMap.pxw - 100 && isInEnterOnlinePoint() == null && isInEnterOfflinePoint() == null && !isAttacPlayerStatus() && TileMap.mapID != 51 && TileMap.mapID != 52 && GameCanvas.panel.vPlayerMenu.size() > 0 && GameScr.gI().popUpYesNo == null)
						{
							int num5 = Math.abs(cx - charFocus.cx);
							int num6 = Math.abs(cy - charFocus.cy);
							if (num5 < 60 && num6 < 40)
							{
								if (cmdMenu == null)
								{
									cmdMenu = new Command(mResources.MENU, 11111);
									cmdMenu.isPlaySoundButton = false;
								}
								cmdMenu.x = charFocus.cx - GameScr.cmx;
								cmdMenu.y = charFocus.cy - charFocus.ch - 30 - GameScr.cmy;
							}
							else
							{
								cmdMenu = null;
							}
						}
						else
						{
							cmdMenu = null;
						}
					}
					if (isShadown)
					{
						updateShadown();
					}
					if (isTeleport)
					{
						return;
					}
					if (chatInfo != null)
					{
						chatInfo.update();
					}
					if (shadowLife > 0)
					{
						shadowLife--;
					}
					if (resultTest > 0 && GameCanvas.gameTick % 2 == 0)
					{
						resultTest--;
						if (resultTest == 30 || resultTest == 60)
						{
							resultTest = 0;
						}
					}
					updateSkillPaint();
					if (mobMe != null)
					{
						updateMobMe();
					}
					if (arr != null)
					{
						arr.update();
					}
					if (dart != null)
					{
						dart.update();
					}
					updateEffect();
					if (holdEffID != 0)
					{
						if (GameCanvas.gameTick % 5 == 0)
						{
							EffecMn.addEff(new Effect(32, cx, cy + 24, 3, 5, 1));
						}
					}
					else
					{
						if (blindEff || sleepEff)
						{
							return;
						}
						if (holder)
						{
							if (charHold != null && (charHold.statusMe == 14 || charHold.statusMe == 5))
							{
								removeHoleEff();
							}
							if (mobHold != null && mobHold.status == 1)
							{
								removeHoleEff();
							}
							if (me && statusMe == 2 && currentMovePoint != null)
							{
								holder = false;
								charHold = null;
								mobHold = null;
							}
							if (TileMap.tileTypeAt(cx, cy, 2))
							{
								cf = 16;
							}
							else
							{
								cf = 31;
							}
							return;
						}
						if (cHP > 0)
						{
							for (int i = 0; i < vEff.size(); i++)
							{
								EffectChar effectChar = (EffectChar)vEff.elementAt(i);
								if (effectChar.template.type == 0 || effectChar.template.type == 12)
								{
									if (GameCanvas.isEff1)
									{
										cHP += effectChar.param;
										cMP += effectChar.param;
									}
								}
								else if (effectChar.template.type == 4 || effectChar.template.type == 17)
								{
									if (GameCanvas.isEff1)
									{
										cHP += effectChar.param;
									}
								}
								else if (effectChar.template.type == 13 && GameCanvas.isEff1)
								{
									cHP -= cHPFull * 3 / 100;
									if (cHP < 1)
									{
										cHP = 1L;
									}
								}
							}
							if (eff5BuffHp > 0 && GameCanvas.isEff2)
							{
								cHP += eff5BuffHp;
							}
							if (eff5BuffMp > 0 && GameCanvas.isEff2)
							{
								cMP += eff5BuffMp;
							}
							if (cHP > cHPFull)
							{
								cHP = cHPFull;
							}
							if (cMP > cMPFull)
							{
								cMP = cMPFull;
							}
						}
						if (cmtoChar)
						{
							GameScr.cmtoX = cx - GameScr.gW2;
							GameScr.cmtoY = cy - GameScr.gH23;
							if (!GameCanvas.isTouchControl)
							{
								GameScr.cmtoX += GameScr.gW6 * cdir;
							}
						}
						tick = (tick + 1) % 100;
						if (me)
						{
							if (charFocus != null && !GameScr.vCharInMap.contains(charFocus))
							{
								charFocus = null;
							}
							if (cx < 10)
							{
								cvx = 0;
								cx = 10;
							}
							else if (cx > TileMap.pxw - 10)
							{
								cx = TileMap.pxw - 10;
								cvx = 0;
							}
							if (!ischangingMap && isInWaypoint())
							{
								Service.gI().charMove();
								if (TileMap.isTrainingMap())
								{
									Service.gI().getMapOffline();
									ischangingMap = true;
								}
								else
								{
									Service.gI().requestChangeMap();
								}
								isLockKey = true;
								ischangingMap = true;
								GameCanvas.clearKeyHold();
								GameCanvas.clearKeyPressed();
								InfoDlg.showWait();
								return;
							}
							if (currentMovePoint == null && statusMe != 4 && Res.abs(cx - cxSend) + Res.abs(cy - cySend) >= 200 && cy - cySend <= 0 && me)
							{
								Service.gI().charMove();
							}
							if (isLockMove)
							{
								currentMovePoint = null;
							}
							if (currentMovePoint != null)
							{
								if (abs(cx - currentMovePoint.xEnd) <= 16 && abs(cy - currentMovePoint.yEnd) <= 16)
								{
									cx = (currentMovePoint.xEnd + cx) / 2;
									cy = currentMovePoint.yEnd;
									currentMovePoint = null;
									GameScr.instance.clickMoving = false;
									checkPerformEndMovePointAction();
									cvx = (cvy = 0);
									if ((TileMap.tileTypeAtPixel(cx, cy) & 2) == 2)
									{
										statusMe = 1;
									}
									else
									{
										setCharFallFromJump();
									}
									Service.gI().charMove();
								}
								else
								{
									cdir = ((currentMovePoint.xEnd > cx) ? 1 : (-1));
									if (TileMap.tileTypeAt(cx, cy, 2))
									{
										statusMe = 2;
										if (currentMovePoint != null)
										{
											cvx = cspeed * cdir;
											cvy = 0;
										}
										if (abs(cx - currentMovePoint.xEnd) <= 10)
										{
											if (currentMovePoint.yEnd > cy)
											{
												bool flag4 = false;
												sbyte b = 1;
												b = (sbyte)((cdir == 1) ? 1 : (-1));
												for (int j = 0; j < 2; j++)
												{
													if (TileMap.tileTypeAt(currentMovePoint.xEnd + chw * b, cy + chh * j, 2))
													{
														flag4 = true;
														break;
													}
												}
												if (flag4)
												{
													currentMovePoint = null;
													GameScr.instance.clickMoving = false;
													statusMe = 1;
													cvx = (cvy = 0);
													checkPerformEndMovePointAction();
												}
												else
												{
													SoundMn.gI().charJump();
													cx = currentMovePoint.xEnd;
													statusMe = 10;
													cvy = -5;
													cvx = 0;
													Res.outz("Jum lun");
												}
											}
											else
											{
												SoundMn.gI().charJump();
												cx = currentMovePoint.xEnd;
												statusMe = 10;
												cvy = -5;
												cvx = 0;
											}
										}
										if (cdir == 1)
										{
											if (TileMap.tileTypeAt(cx + chw, cy - chh, 4))
											{
												cvx = cspeed * cdir;
												statusMe = 10;
												cvy = -5;
											}
										}
										else if (TileMap.tileTypeAt(cx - chw - 1, cy - chh, 8))
										{
											cvx = cspeed * cdir;
											statusMe = 10;
											cvy = -5;
										}
									}
									else
									{
										if (currentMovePoint.yEnd < cy + 10)
										{
											statusMe = 10;
											cvy = -5;
											if (abs(cy - currentMovePoint.yEnd) <= 10)
											{
												cy = currentMovePoint.yEnd;
												cvy = 0;
											}
											if (abs(cx - currentMovePoint.xEnd) <= 10)
											{
												cvx = 0;
											}
											else
											{
												cvx = cspeed * cdir;
											}
										}
										else if (TileMap.tileTypeAt(cx, cy, 2))
										{
											currentMovePoint = null;
											GameScr.instance.clickMoving = false;
											statusMe = 1;
											cvx = (cvy = 0);
											checkPerformEndMovePointAction();
										}
										else
										{
											if (statusMe == 10 || statusMe == 2)
											{
												cvy = 0;
											}
											statusMe = 4;
										}
										if (currentMovePoint.yEnd > cy)
										{
											if (cdir == 1)
											{
												if (TileMap.tileTypeAt(cx + chw, cy - chh, 4))
												{
													cvx = (cvy = 0);
													statusMe = 4;
													currentMovePoint = null;
													GameScr.instance.clickMoving = false;
													checkPerformEndMovePointAction();
												}
											}
											else if (TileMap.tileTypeAt(cx - chw - 1, cy - chh, 8))
											{
												cvx = (cvy = 0);
												statusMe = 4;
												currentMovePoint = null;
												GameScr.instance.clickMoving = false;
												checkPerformEndMovePointAction();
											}
										}
									}
								}
							}
							searchFocus();
						}
						else
						{
							checkHideCharName();
							if (statusMe == 1 || statusMe == 6)
							{
								bool flag5 = false;
								if (currentMovePoint != null)
								{
									if (abs(currentMovePoint.xEnd - cx) < 17 && abs(currentMovePoint.yEnd - cy) < 25)
									{
										cx = currentMovePoint.xEnd;
										cy = currentMovePoint.yEnd;
										currentMovePoint = null;
										if ((TileMap.tileTypeAtPixel(cx, cy) & 2) == 2)
										{
											statusMe = 1;
											cp3 = 0;
											GameCanvas.gI().startDust(-1, cx - -8, cy);
											GameCanvas.gI().startDust(1, cx - 8, cy);
										}
										else
										{
											statusMe = 4;
											cvy = 0;
											cp1 = 0;
										}
										flag5 = true;
									}
									else if ((statusBeforeNothing == 10 || cf == 8) && vMovePoints.size() > 0)
									{
										flag5 = true;
									}
									else if (cy == currentMovePoint.yEnd)
									{
										if (cx != currentMovePoint.xEnd)
										{
											cx = (cx + currentMovePoint.xEnd) / 2;
											cf = GameCanvas.gameTick % 5 + 2;
										}
									}
									else if (cy < currentMovePoint.yEnd)
									{
										cf = 12;
										cx = (cx + currentMovePoint.xEnd) / 2;
										if (cvy < 0)
										{
											cvy = 0;
										}
										cy += cvy;
										if ((TileMap.tileTypeAtPixel(cx, cy) & 2) == 2)
										{
											GameCanvas.gI().startDust(-1, cx - -8, cy);
											GameCanvas.gI().startDust(1, cx - 8, cy);
										}
										cvy++;
										if (cvy > 16)
										{
											cy = (cy + currentMovePoint.yEnd) / 2;
										}
									}
									else
									{
										cf = 7;
										cx = (cx + currentMovePoint.xEnd) / 2;
										cy = (cy + currentMovePoint.yEnd) / 2;
									}
								}
								else
								{
									flag5 = true;
								}
								if (flag5 && vMovePoints.size() > 0)
								{
									currentMovePoint = (MovePoint)vMovePoints.firstElement();
									vMovePoints.removeElementAt(0);
									if (currentMovePoint.status == 2)
									{
										if ((TileMap.tileTypeAtPixel(cx, cy + 12) & 2) != 2)
										{
											statusMe = 10;
											cp1 = 0;
											cp2 = 0;
											cvx = -(cx - currentMovePoint.xEnd) / 10;
											cvy = -(cy - currentMovePoint.yEnd) / 10;
											if (cx - currentMovePoint.xEnd > 0)
											{
												cdir = -1;
											}
											else if (cx - currentMovePoint.xEnd < 0)
											{
												cdir = 1;
											}
										}
										else
										{
											statusMe = 2;
											if (cx - currentMovePoint.xEnd > 0)
											{
												cdir = -1;
											}
											else if (cx - currentMovePoint.xEnd < 0)
											{
												cdir = 1;
											}
											cvx = cspeed * cdir;
											cvy = 0;
										}
									}
									else if (currentMovePoint.status == 3)
									{
										if ((TileMap.tileTypeAtPixel(cx, cy + 23) & 2) != 2)
										{
											statusMe = 10;
											cp1 = 0;
											cp2 = 0;
											cvx = -(cx - currentMovePoint.xEnd) / 10;
											cvy = -(cy - currentMovePoint.yEnd) / 10;
											if (cx - currentMovePoint.xEnd > 0)
											{
												cdir = -1;
											}
											else if (cx - currentMovePoint.xEnd < 0)
											{
												cdir = 1;
											}
										}
										else
										{
											statusMe = 3;
											GameCanvas.gI().startDust(-1, cx - -8, cy);
											GameCanvas.gI().startDust(1, cx - 8, cy);
											if (cx - currentMovePoint.xEnd > 0)
											{
												cdir = -1;
											}
											else if (cx - currentMovePoint.xEnd < 0)
											{
												cdir = 1;
											}
											cvx = abs(cx - currentMovePoint.xEnd) / 10 * cdir;
											cvy = -10;
										}
									}
									else if (currentMovePoint.status == 4)
									{
										statusMe = 4;
										if (cx - currentMovePoint.xEnd > 0)
										{
											cdir = -1;
										}
										else if (cx - currentMovePoint.xEnd < 0)
										{
											cdir = 1;
										}
										cvx = abs(cx - currentMovePoint.xEnd) / 9 * cdir;
										cvy = 0;
									}
									else
									{
										cx = currentMovePoint.xEnd;
										cy = currentMovePoint.yEnd;
										currentMovePoint = null;
									}
								}
								else if (flag5 && me && (cx != cxSend || cy != cySend))
								{
									Service.gI().charMove();
								}
							}
						}
						switch (statusMe)
						{
						case 1:
							updateCharStand();
							break;
						case 2:
							updateCharRun();
							break;
						case 3:
							updateCharJump();
							break;
						case 4:
							updateCharFall();
							break;
						case 5:
							updateCharDeadFly();
							break;
						case 16:
							updateResetPoint();
							break;
						case 9:
							updateCharAutoJump();
							break;
						case 10:
							updateCharFly();
							break;
						case 12:
							updateSkillStand();
							break;
						case 13:
							updateSkillFall();
							break;
						case 14:
							cp1++;
							if (cp1 > 30)
							{
								cp1 = 0;
							}
							if (cp1 % 15 < 5)
							{
								cf = 0;
							}
							else
							{
								cf = 1;
							}
							break;
						case 6:
							if (isInjure <= 0)
							{
								cf = 0;
							}
							else if (statusBeforeNothing == 10)
							{
								cx += cvx;
							}
							else if (cf <= 1)
							{
								cp1++;
								if (cp1 > 6)
								{
									cf = 0;
								}
								else
								{
									cf = 1;
								}
								if (cp1 > 10)
								{
									cp1 = 0;
								}
							}
							if (cf != 7 && cf != 12 && (TileMap.tileTypeAtPixel(cx, cy + 1) & 2) != 2)
							{
								cvx = 0;
								cvy = 0;
								statusMe = 4;
								cf = 7;
							}
							if (me)
							{
								break;
							}
							cp3++;
							if (cp3 > 10)
							{
								if ((TileMap.tileTypeAtPixel(cx, cy + 1) & 2) != 2)
								{
									cy += 5;
								}
								else
								{
									cf = 0;
								}
							}
							if (cp3 > 50)
							{
								cp3 = 0;
								currentMovePoint = null;
							}
							break;
						}
						if (isInjure > 0)
						{
							cf = 23;
							isInjure--;
						}
						if (wdx != 0 || wdy != 0)
						{
							startDie(wdx, wdy);
							wdx = 0;
							wdy = 0;
						}
						if (moveFast != null)
						{
							if (moveFast[0] == 0)
							{
								moveFast[0]++;
								ServerEffect.addServerEffect(60, this, 1);
							}
							else if (moveFast[0] < 10)
							{
								moveFast[0]++;
							}
							else
							{
								cx = moveFast[1];
								cy = moveFast[2];
								moveFast = null;
								ServerEffect.addServerEffect(60, this, 1);
								if (me)
								{
									if ((TileMap.tileTypeAtPixel(cx, cy) & 2) != 2)
									{
										statusMe = 4;
										myCharz().setAutoSkillPaint(GameScr.sks[38], 1);
									}
									else
									{
										Service.gI().charMove();
										myCharz().setAutoSkillPaint(GameScr.sks[38], 0);
									}
								}
							}
						}
						if (statusMe != 10)
						{
							fy = 0;
						}
						if (isCharge)
						{
							cf = 17;
							if (GameCanvas.gameTick % 4 == 0)
							{
								ServerEffect.addServerEffect(1, cx, cy + GameCanvas.transY, 1);
							}
							if (me)
							{
								long num7 = mSystem.currentTimeMillis();
								if (num7 - last >= 1000)
								{
									Res.outz("%= " + myskill.damage);
									last = num7;
									cHP += cHPFull * myskill.damage / 100;
									cMP += cMPFull * myskill.damage / 100;
									if (cHP < cHPFull)
									{
										GameScr.startFlyText("+" + cHPFull * myskill.damage / 100 + " " + mResources.HP, cx, cy - ch - 20, 0, -1, mFont.HP);
									}
									if (cMP < cMPFull)
									{
										GameScr.startFlyText("+" + cMPFull * myskill.damage / 100 + " " + mResources.KI, cx, cy - ch - 20, 0, -2, mFont.MP);
									}
									Service.gI().skill_not_focus(2);
								}
							}
						}
						if (isFlyUp)
						{
							if (me)
							{
								isLockKey = true;
								statusMe = 3;
								cvy = -8;
								if (cy <= TileMap.pxh - 240)
								{
									isFlyUp = false;
									isLockKey = false;
									statusMe = 4;
								}
							}
							else
							{
								statusMe = 3;
								cvy = -8;
								if (cy <= TileMap.pxh - 240)
								{
									cvy = 0;
									isFlyUp = false;
									cvy = 0;
									statusMe = 1;
								}
							}
						}
						updateMount();
						updEffChar();
						updateEye();
						updateFHead();
					}
				}
			}


}
