using System;
using Assets.src.g;
public partial class Mob
{
	public virtual void update()
			{
				if (isMafuba)
				{
					return;
				}
				GetFrame();
				if (blindEff && GameCanvas.gameTick % 5 == 0)
				{
					ServerEffect.addServerEffect(113, x, y, 1);
				}
				if (sleepEff && GameCanvas.gameTick % 10 == 0)
				{
					EffecMn.addEff(new Effect(41, x, y, 3, 1, 1));
				}
				if (!GameCanvas.lowGraphic && status != 1 && status != 0 && !GameCanvas.lowGraphic && GameCanvas.gameTick % (15 + mobId * 2) == 0)
				{
					for (int i = 0; i < GameScr.vCharInMap.size(); i++)
					{
						Char @char = (Char)GameScr.vCharInMap.elementAt(i);
						if (@char != null && @char.isFlyAndCharge && @char.cf == 32)
						{
							Char char2 = new Char();
							char2.cx = @char.cx;
							char2.cy = @char.cy - @char.ch;
							if (@char.cgender == 0)
							{
								MonsterDart.addMonsterDart(x + dir * w, y, checkIsBoss(), -100L, -100L, char2, 25);
							}
						}
					}
					if (Char.myCharz().isFlyAndCharge && Char.myCharz().cf == 32)
					{
						Char char3 = new Char();
						char3.cx = Char.myCharz().cx;
						char3.cy = Char.myCharz().cy - Char.myCharz().ch;
						if (Char.myCharz().cgender == 0)
						{
							MonsterDart.addMonsterDart(x + dir * w, y, checkIsBoss(), -100L, -100L, char3, 25);
						}
					}
				}
				if (holdEffID != 0 && GameCanvas.gameTick % 5 == 0)
				{
					EffecMn.addEff(new Effect(holdEffID, x, y + 24, 3, 5, 1));
				}
				if (isFreez)
				{
					if (GameCanvas.gameTick % 5 == 0)
					{
						ServerEffect.addServerEffect(113, x, y, 1);
					}
					long num = mSystem.currentTimeMillis();
					if (num - last >= 1000)
					{
						seconds--;
						last = num;
						if (seconds < 0)
						{
							isFreez = false;
							seconds = 0;
						}
					}
					if (isTypeNewMod())
					{
						frame = hurt[GameCanvas.gameTick % hurt.Length];
					}
					else if (isNewModStand())
					{
						frame = attack1[GameCanvas.gameTick % attack1.Length];
					}
					else if (isNewMod())
					{
						if (GameCanvas.gameTick % 20 > 5)
						{
							frame = 11;
						}
						else
						{
							frame = 10;
						}
					}
					else if (isSpecial())
					{
						if (GameCanvas.gameTick % 20 > 5)
						{
							frame = 1;
						}
						else
						{
							frame = 15;
						}
					}
					else if (GameCanvas.gameTick % 20 > 5)
					{
						frame = 11;
					}
					else
					{
						frame = 10;
					}
				}
				if (!isUpdate())
				{
					return;
				}
				if (isShadown)
				{
					updateShadown();
				}
				if (vMobMove == null && arrMobTemplate[templateId].rangeMove != 0)
				{
					return;
				}
				if (status != 3 && isBusyAttackSomeOne)
				{
					if (cFocus != null)
					{
						cFocus.doInjure(dame, dameMp, isCrit: false, isMob: true);
					}
					else if (mobToAttack != null)
					{
						mobToAttack.setInjure();
					}
					isBusyAttackSomeOne = false;
				}
				if (levelBoss > 0)
				{
					updateSuperEff();
				}
				if (status != 0 && status != 1 && !isMobMe)
				{
					if (hp <= 0)
					{
						status = 0;
					}
					else if (arrMobTemplate != null && templateId < arrMobTemplate.Length && arrMobTemplate[templateId] != null)
					{
						int maxRange = arrMobTemplate[templateId].rangeMove;
						if (maxRange > 0 && Res.abs(x - xFirst) > maxRange + 15)
						{
							x = xFirst;
						}
						if (arrMobTemplate[templateId].type != 4 && arrMobTemplate[templateId].type != 5)
						{
							if (yFirst > 0 && Res.abs(y - yFirst) > 25)
							{
								y = yFirst;
							}
						}
					}
				}
				switch (status)
				{
				case 1:
					isDisable = false;
					isDontMove = false;
					isFire = false;
					isIce = false;
					isWind = false;
					y += p1;
					if (GameCanvas.gameTick % 2 == 0)
					{
						if (p2 > 1)
						{
							p2--;
						}
						else if (p2 < -1)
						{
							p2++;
						}
					}
					x += p2;
					if (isTypeNewMod())
					{
						frame = hurt[GameCanvas.gameTick % hurt.Length];
					}
					else if (isNewModStand())
					{
						frame = attack1[GameCanvas.gameTick % attack1.Length];
					}
					else if (isNewMod())
					{
						frame = 11;
					}
					else if (isSpecial())
					{
						frame = 15;
					}
					else
					{
						frame = 11;
					}
					if (isDie)
					{
						isDie = false;
						if (isMobMe)
						{
							for (int j = 0; j < GameScr.vMob.size(); j++)
							{
								if (((Mob)GameScr.vMob.elementAt(j)).mobId == mobId)
								{
									GameScr.vMob.removeElementAt(j);
								}
							}
						}
						p1 = 0;
						p2 = 0;
						x = (y = 0);
						hp = getTemplate().hp;
						status = 0;
						timeStatus = 0;
						break;
					}
					if ((TileMap.tileTypeAtPixel(x, y) & 2) == 2)
					{
						p1 = ((p1 <= 4) ? (-p1) : (-4));
						if (p3 == 0)
						{
							p3 = 16;
						}
					}
					else
					{
						p1++;
					}
					if (p3 > 0)
					{
						p3--;
						if (p3 == 0)
						{
							isDie = true;
						}
					}
					break;
				case 2:
					if (holdEffID == 0 && !isFreez && !blindEff && !sleepEff)
					{
						timeStatus = 0;
						updateMobStandWait();
					}
					break;
				case 4:
					if (holdEffID == 0 && !blindEff && !sleepEff && !isFreez)
					{
						timeStatus = 0;
						p1++;
						if (p1 > 40 + mobId % 5)
						{
							y -= 2;
							status = 5;
							p1 = 0;
						}
					}
					break;
				case 3:
					if (holdEffID == 0 && !blindEff && !sleepEff && !isFreez)
					{
						updateMobAttack();
					}
					break;
				case 5:
					if (holdEffID != 0 || blindEff || sleepEff)
					{
						break;
					}
					if (isFreez)
					{
						if (arrMobTemplate[templateId].type == 4)
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
					}
					else
					{
						timeStatus = 0;
						updateMobWalk();
					}
					break;
				case 6:
					timeStatus = 0;
					p1++;
					y += p1;
					if (y >= yFirst)
					{
						y = yFirst;
						p1 = 0;
						status = 5;
					}
					break;
				case 7:
					updateInjure();
					break;
				}
			}
	public MobTemplate getTemplate()
			{
				return arrMobTemplate[templateId];
			}
	public bool checkIsBoss()
			{
				if (isBoss || levelBoss > 0)
				{
					return true;
				}
				return false;
			}

}
