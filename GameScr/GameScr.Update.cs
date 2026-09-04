using System;
using Assets.src.g;

public partial class GameScr : mScreen, IChatable
{
	public void updateOpen()
			{
				if (isstarOpen)
				{
					if (moveUp > -3)
					{
						moveUp -= 4;
					}
					else
					{
						moveUp = -2;
					}
					if (moveDow < GameCanvas.h + 3)
					{
						moveDow += 4;
					}
					else
					{
						moveDow = GameCanvas.h + 2;
					}
					if (moveUp <= -2 && moveDow >= GameCanvas.h + 2)
					{
						isstarOpen = false;
					}
				}
			}

	public override void update()
			{
				if (GameCanvas.keyPressed[16])
				{
					GameCanvas.keyPressed[16] = false;
					Char.myCharz().findNextFocusByKey();
				}
				if (GameCanvas.keyPressed[13] && !GameCanvas.panel.isShow)
				{
					GameCanvas.keyPressed[13] = false;
					Char.myCharz().findNextFocusByKey();
				}
				if (GameCanvas.keyPressed[17])
				{
					GameCanvas.keyPressed[17] = false;
					Char.myCharz().searchItem();
					if (Char.myCharz().itemFocus != null)
					{
						pickItem();
					}
				}
				if (GameCanvas.gameTick % 100 == 0 && TileMap.mapID == 137)
				{
					shock_scr = 30;
				}
				if (isAutoPlay && GameCanvas.gameTick % 20 == 0)
				{
					autoPlay();
				}
				updateXoSo();
				mSystem.checkAdComlete();
				SmallImage.update();
				try
				{
					if (LoginScr.isContinueToLogin)
					{
						LoginScr.isContinueToLogin = false;
					}
					if (tickMove == 1)
					{
						lastTick = mSystem.currentTimeMillis();
					}
					if (tickMove == 100)
					{
						tickMove = 0;
						currTick = mSystem.currentTimeMillis();
						int second = (int)(currTick - lastTick) / 1000;
						Service.gI().checkMMove(second);
					}
					if (lockTick > 0)
					{
						lockTick--;
						if (lockTick == 0)
						{
							Controller.isStopReadMessage = false;
						}
					}
					checkCharFocus();
					GameCanvas.debug("E1", 0);
					updateCamera();
					GameCanvas.debug("E2", 0);
					ChatTextField.gI().update();
					GameCanvas.debug("E3", 0);
					for (int i = 0; i < vCharInMap.size(); i++)
					{
						((Char)vCharInMap.elementAt(i)).update();
					}
					for (int i = 0; i < Teleport.vTeleport.size(); i++)
					{
						((Teleport)Teleport.vTeleport.elementAt(i)).update();
					}
					Char.myCharz().update();
					if (Char.myCharz().statusMe == 1)
					{
					}
					if (popUpYesNo != null)
					{
						popUpYesNo.update();
					}
					EffecMn.update();
					GameCanvas.debug("E5x", 0);
					for (int i = 0; i < vMob.size(); i++)
					{
						((Mob)vMob.elementAt(i)).update();
					}
					GameCanvas.debug("E6", 0);
					for (int i = 0; i < vNpc.size(); i++)
					{
						((Npc)vNpc.elementAt(i)).update();
					}
					nSkill = onScreenSkill.Length;
					for (int i = onScreenSkill.Length - 1; i >= 0; i--)
					{
						Skill skill = onScreenSkill[i];
						if (skill != null)
						{
							nSkill = i + 1;
							break;
						}
						nSkill--;
					}
					setSkillBarPosition();
					GameCanvas.debug("E7", 0);
					GameCanvas.gI().updateDust();
					GameCanvas.debug("E8", 0);
					updateFlyText();
					PopUp.updateAll();
					updateSplash();
					updateSS();
					GameCanvas.updateBG();
					GameCanvas.debug("E9", 0);
					updateClickToArrow();
					GameCanvas.debug("E10", 0);
					for (int i = 0; i < vItemMap.size(); i++)
					{
						((ItemMap)vItemMap.elementAt(i)).update();
					}
					GameCanvas.debug("E11", 0);
					GameCanvas.debug("E13", 0);
					for (int i = Effect2.vRemoveEffect2.size() - 1; i >= 0; i--)
					{
						Effect2.vEffect2.removeElement(Effect2.vRemoveEffect2.elementAt(i));
						Effect2.vRemoveEffect2.removeElementAt(i);
					}
					for (int i = 0; i < Effect2.vEffect2.size(); i++)
					{
						Effect2 effect = (Effect2)Effect2.vEffect2.elementAt(i);
						effect.update();
					}
					for (int i = 0; i < Effect2.vEffect2Outside.size(); i++)
					{
						Effect2 effect2 = (Effect2)Effect2.vEffect2Outside.elementAt(i);
						effect2.update();
					}
					for (int i = 0; i < Effect2.vAnimateEffect.size(); i++)
					{
						Effect2 effect3 = (Effect2)Effect2.vAnimateEffect.elementAt(i);
						effect3.update();
					}
					for (int i = 0; i < Effect2.vEffectFeet.size(); i++)
					{
						Effect2 effect4 = (Effect2)Effect2.vEffectFeet.elementAt(i);
						effect4.update();
					}
					for (int i = 0; i < Effect2.vEffect3.size(); i++)
					{
						Effect2 effect5 = (Effect2)Effect2.vEffect3.elementAt(i);
						effect5.update();
					}
					BackgroudEffect.updateEff();
					info1.update();
					info2.update();
					GameCanvas.debug("E15", 0);
					if (currentCharViewInfo != null && !currentCharViewInfo.Equals(Char.myCharz()))
					{
						currentCharViewInfo.update();
					}
					runArrow++;
					if (runArrow > 3)
					{
						runArrow = 0;
					}
					if (isInjureHp)
					{
						twHp++;
						if (twHp == 20)
						{
							twHp = 0L;
							isInjureHp = false;
						}
					}
					else if (dHP > Char.myCharz().cHP)
					{
						long num = dHP - Char.myCharz().cHP >> 1;
						if (num < 1)
						{
							num = 1L;
						}
						dHP -= num;
					}
					else
					{
						dHP = Char.myCharz().cHP;
					}
					if (isInjureMp)
					{
						twMp++;
						if (twMp == 20)
						{
							twMp = 0L;
							isInjureMp = false;
						}
					}
					else if (dMP > Char.myCharz().cMP)
					{
						long num2 = dMP - Char.myCharz().cMP >> 1;
						if (num2 < 1)
						{
							num2 = 1L;
						}
						dMP -= num2;
					}
					else
					{
						dMP = Char.myCharz().cMP;
					}
					if (tMenuDelay > 0)
					{
						tMenuDelay--;
					}
					if (isRongThanMenu())
					{
						int num3 = 100;
						while (yR - num3 < cmy)
						{
							cmy--;
						}
					}
					for (int i = 0; i < Char.vItemTime.size(); i++)
					{
						((ItemTime)Char.vItemTime.elementAt(i)).update();
					}
					for (int i = 0; i < textTime.size(); i++)
					{
						((ItemTime)textTime.elementAt(i)).update();
					}
					updateChatVip();
				}
				catch (Exception)
				{
				}
				int num4 = GameCanvas.gameTick % 4000;
				if (num4 == 1000)
				{
					checkRemoveImage();
				}
				EffectManager.update();
			}

	public void updateSS()
			{
				if (indexMenu != -1)
				{
					if (cmySK != cmtoYSK)
					{
						cmvySK = cmtoYSK - cmySK << 2;
						cmdySK += cmvySK;
						cmySK += cmdySK >> 4;
						cmdySK &= 15;
					}
					if (Math.abs(cmtoYSK - cmySK) < 15 && cmySK < 0)
					{
						cmtoYSK = 0;
					}
					if (Math.abs(cmtoYSK - cmySK) < 15 && cmySK > cmyLimSK)
					{
						cmtoYSK = cmyLimSK;
					}
				}
			}

	private void updateGamePad()
			{
				if (isAnalog == 0 || Char.myCharz().statusMe == 14)
				{
					return;
				}
				if (GameCanvas.isPointerHoldIn(xF, yF, 40, 40))
				{
					mScreen.keyTouch = 5;
					if (GameCanvas.isPointerJustRelease)
					{
						GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25] = true;
						GameCanvas.isPointerClick = (GameCanvas.isPointerJustDown = (GameCanvas.isPointerJustRelease = false));
					}
				}
				gamePad.update();
				if (GameCanvas.isPointerHoldIn(xTG, yTG, 34, 34))
				{
					mScreen.keyTouch = 13;
					GameCanvas.isPointerJustDown = false;
					isPointerDowning = false;
					if (GameCanvas.isPointerClick && GameCanvas.isPointerJustRelease)
					{
						Char.myCharz().findNextFocusByKey();
						GameCanvas.isPointerClick = (GameCanvas.isPointerJustDown = (GameCanvas.isPointerJustRelease = false));
					}
				}
			}

}
