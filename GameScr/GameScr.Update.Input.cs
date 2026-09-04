using System;
using Assets.src.g;

public partial class GameScr : mScreen, IChatable
{
	public override void keyPress(int keyCode)
			{
				base.keyPress(keyCode);
			}

	public override void updateKey()
			{
				if (Controller.isStopReadMessage || Char.myCharz().isTeleport || Char.myCharz().isPaintNewSkill || InfoDlg.isLock)
				{
					return;
				}
				if (GameCanvas.isTouch && !ChatTextField.gI().isShow && !GameCanvas.menu.showMenu)
				{
					updateKeyTouchControl();
				}
				checkAuto();
				GameCanvas.debug("F2", 0);
				if (ChatPopup.currChatPopup != null)
				{
					Command cmdNextLine = ChatPopup.currChatPopup.cmdNextLine;
					if ((GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25] || mScreen.getCmdPointerLast(cmdNextLine)) && cmdNextLine != null)
					{
						GameCanvas.isPointerJustRelease = false;
						GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25] = false;
						mScreen.keyTouch = -1;
						cmdNextLine?.performAction();
					}
				}
				else if (!ChatTextField.gI().isShow)
				{
					if ((GameCanvas.keyPressed[12] || mScreen.getCmdPointerLast(GameCanvas.currentScreen.left)) && left != null)
					{
						GameCanvas.isPointerJustRelease = false;
						GameCanvas.isPointerClick = false;
						GameCanvas.keyPressed[12] = false;
						mScreen.keyTouch = -1;
						if (left != null)
						{
							left.performAction();
						}
					}
					if ((GameCanvas.keyPressed[13] || mScreen.getCmdPointerLast(GameCanvas.currentScreen.right)) && right != null)
					{
						GameCanvas.isPointerJustRelease = false;
						GameCanvas.isPointerClick = false;
						GameCanvas.keyPressed[13] = false;
						mScreen.keyTouch = -1;
						if (right != null)
						{
							right.performAction();
						}
					}
					if ((GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25] || mScreen.getCmdPointerLast(GameCanvas.currentScreen.center)) && center != null)
					{
						GameCanvas.isPointerJustRelease = false;
						GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25] = false;
						mScreen.keyTouch = -1;
						if (center != null)
						{
							center.performAction();
						}
					}
				}
				else
				{
					if (ChatTextField.gI().left != null && (GameCanvas.keyPressed[12] || mScreen.getCmdPointerLast(ChatTextField.gI().left)) && ChatTextField.gI().left != null)
					{
						ChatTextField.gI().left.performAction();
					}
					if (ChatTextField.gI().right != null && (GameCanvas.keyPressed[13] || mScreen.getCmdPointerLast(ChatTextField.gI().right)) && ChatTextField.gI().right != null)
					{
						ChatTextField.gI().right.performAction();
					}
					if (ChatTextField.gI().center != null && (GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25] || mScreen.getCmdPointerLast(ChatTextField.gI().center)) && ChatTextField.gI().center != null)
					{
						ChatTextField.gI().center.performAction();
					}
				}
				GameCanvas.debug("F6", 0);
				updateKeyAlert();
				GameCanvas.debug("F7", 0);
				if (Char.myCharz().currentMovePoint != null)
				{
					for (int i = 0; i < GameCanvas.keyPressed.Length; i++)
					{
						if (GameCanvas.keyPressed[i])
						{
							Char.myCharz().currentMovePoint = null;
							break;
						}
					}
				}
				GameCanvas.debug("F8", 0);
				if (ChatTextField.gI().isShow && GameCanvas.keyAsciiPress != 0)
				{
					ChatTextField.gI().keyPressed(GameCanvas.keyAsciiPress);
					GameCanvas.keyAsciiPress = 0;
				}
				else if (isLockKey)
				{
					GameCanvas.clearKeyHold();
					GameCanvas.clearKeyPressed();
				}
				else
				{
					if (GameCanvas.menu.showMenu || isOpenUI() || Char.isLockKey || ModMenu.uiCustomOpen)
					{
						return;
					}
					if (GameCanvas.keyAsciiPress != 0 && (GameCanvas.keyAsciiPress == 109 || GameCanvas.keyAsciiPress == 77) && !ChatTextField.gI().isShow)
					{
						GameCanvas.keyAsciiPress = 0;
						GameCanvas.clearKeyPressed();
						if (GameCanvas.panel != null && GameCanvas.panel.isShow)
						{
							GameCanvas.panel.hide();
						}
						else
						{
							actMenu();
						}
					}
					if (GameCanvas.keyAsciiPress != 0 && (GameCanvas.keyAsciiPress == 107 || GameCanvas.keyAsciiPress == 75) && !ChatTextField.gI().isShow)
					{
						GameCanvas.keyAsciiPress = 0;
						GameCanvas.clearKeyPressed();
						ModHotkey.ToggleModMenu();
					}
					if (GameCanvas.keyPressed[10])
					{
						GameCanvas.keyPressed[10] = false;
						doUseHP();
						GameCanvas.clearKeyPressed();
					}
					if (GameCanvas.keyPressed[11] && mobCapcha == null)
					{
						if (popUpYesNo != null)
						{
							popUpYesNo.cmdYes.performAction();
						}
						else if (info2.info.info != null && info2.info.info.charInfo != null)
						{
							GameCanvas.panel.setTypeMessage();
							GameCanvas.panel.show();
						}
						GameCanvas.keyPressed[11] = false;
						GameCanvas.clearKeyPressed();
					}
					if (GameCanvas.keyAsciiPress != 0 && TField.isQwerty && GameCanvas.keyAsciiPress == 32)
					{
						doUseHP();
						GameCanvas.keyAsciiPress = 0;
						GameCanvas.clearKeyPressed();
					}
					if (GameCanvas.keyAsciiPress != 0 && mobCapcha == null && TField.isQwerty && GameCanvas.keyAsciiPress == 121)
					{
						if (popUpYesNo != null)
						{
							popUpYesNo.cmdYes.performAction();
							GameCanvas.keyAsciiPress = 0;
							GameCanvas.clearKeyPressed();
						}
						else if (info2.info.info != null && info2.info.info.charInfo != null)
						{
							GameCanvas.panel.setTypeMessage();
							GameCanvas.panel.show();
							GameCanvas.keyAsciiPress = 0;
							GameCanvas.clearKeyPressed();
						}
					}
					if (GameCanvas.keyPressed[10] && mobCapcha == null)
					{
						GameCanvas.keyPressed[10] = false;
						info2.doClick(10);
						GameCanvas.clearKeyPressed();
					}
					checkDrag();
					if (!Char.myCharz().isFlyAndCharge)
					{
						checkClick();
					}
					if (Char.myCharz().cmdMenu != null && Char.myCharz().cmdMenu.isPointerPressInside())
					{
						Char.myCharz().cmdMenu.performAction();
					}
					if (Char.myCharz().skillPaint != null)
					{
						return;
					}
					if (GameCanvas.keyAsciiPress != 0)
					{
						if (mobCapcha == null)
						{
							if (TField.isQwerty)
							{
								if (GameCanvas.keyPressed[1])
								{
									if (keySkill[0] != null)
									{
										doSelectSkill(keySkill[0], isShortcut: true);
									}
								}
								else if (GameCanvas.keyPressed[2])
								{
									if (keySkill[1] != null)
									{
										doSelectSkill(keySkill[1], isShortcut: true);
									}
								}
								else if (GameCanvas.keyPressed[3])
								{
									if (keySkill[2] != null)
									{
										doSelectSkill(keySkill[2], isShortcut: true);
									}
								}
								else if (GameCanvas.keyPressed[4])
								{
									if (keySkill[3] != null)
									{
										doSelectSkill(keySkill[3], isShortcut: true);
									}
								}
								else if (GameCanvas.keyPressed[5])
								{
									if (keySkill[4] != null)
									{
										doSelectSkill(keySkill[4], isShortcut: true);
									}
								}
								else if (GameCanvas.keyPressed[6])
								{
									if (keySkill[5] != null)
									{
										doSelectSkill(keySkill[5], isShortcut: true);
									}
								}
								else if (GameCanvas.keyPressed[7])
								{
									if (keySkill[6] != null)
									{
										doSelectSkill(keySkill[6], isShortcut: true);
									}
								}
								else if (GameCanvas.keyPressed[8])
								{
									if (keySkill[7] != null)
									{
										doSelectSkill(keySkill[7], isShortcut: true);
									}
								}
								else if (GameCanvas.keyPressed[9])
								{
									if (keySkill[8] != null)
									{
										doSelectSkill(keySkill[8], isShortcut: true);
									}
								}
								else if (GameCanvas.keyPressed[0])
								{
									if (keySkill[9] != null)
									{
										doSelectSkill(keySkill[9], isShortcut: true);
									}
								}
								else if (GameCanvas.keyAsciiPress == 114)
								{
									ChatTextField.gI().startChat(this, string.Empty);
								}
							}
							else if (!GameCanvas.isMoveNumberPad)
							{
								ChatTextField.gI().startChat(GameCanvas.keyAsciiPress, this, string.Empty);
							}
							else if (GameCanvas.keyAsciiPress == 55)
							{
								if (keySkill[0] != null)
								{
									doSelectSkill(keySkill[0], isShortcut: true);
								}
							}
							else if (GameCanvas.keyAsciiPress == 56)
							{
								if (keySkill[1] != null)
								{
									doSelectSkill(keySkill[1], isShortcut: true);
								}
							}
							else if (GameCanvas.keyAsciiPress == 57)
							{
								if (keySkill[(!Main.isPC) ? 2 : 21] != null)
								{
									doSelectSkill(keySkill[2], isShortcut: true);
								}
							}
							else if (GameCanvas.keyAsciiPress == 48)
							{
								ChatTextField.gI().startChat(this, string.Empty);
							}
						}
						else
						{
							char[] array = keyInput.ToCharArray();
							MyVector myVector = new MyVector();
							for (int j = 0; j < array.Length; j++)
							{
								myVector.addElement(array[j] + string.Empty);
							}
							myVector.removeElementAt(0);
							string text = (char)GameCanvas.keyAsciiPress + string.Empty;
							if (text.Equals(string.Empty) || text == null || text.Equals("\n"))
							{
								text = "-";
							}
							myVector.insertElementAt(text, myVector.size());
							keyInput = string.Empty;
							for (int k = 0; k < myVector.size(); k++)
							{
								keyInput += ((string)myVector.elementAt(k)).ToUpper();
							}
							Service.gI().mobCapcha((char)GameCanvas.keyAsciiPress);
						}
						GameCanvas.keyAsciiPress = 0;
					}
					if (Char.myCharz().statusMe == 1)
					{
						GameCanvas.debug("F10", 0);
						if (!doSeleckSkillFlag)
						{
							if (GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25])
							{
								GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25] = false;
								doFire(isFireByShortCut: false, skipWaypoint: false);
							}
							else if (GameCanvas.keyHold[(!Main.isPC) ? 2 : 21])
							{
								if (!Char.myCharz().isLockMove)
								{
									setCharJump(0);
								}
							}
							else if (GameCanvas.keyHold[1] && mobCapcha == null)
							{
								if (!Main.isPC)
								{
									Char.myCharz().cdir = -1;
									if (!Char.myCharz().isLockMove)
									{
										setCharJump(-4);
									}
								}
							}
							else if (GameCanvas.keyHold[(!Main.isPC) ? 5 : 25] && mobCapcha == null)
							{
								if (!Main.isPC)
								{
									Char.myCharz().cdir = 1;
									if (!Char.myCharz().isLockMove)
									{
										setCharJump(4);
									}
								}
							}
							else if (GameCanvas.keyHold[(!Main.isPC) ? 4 : 23])
							{
								isAutoPlay = false;
								Char.myCharz().isAttack = false;
								if (Char.myCharz().cdir == 1)
								{
									Char.myCharz().cdir = -1;
								}
								else if (!Char.myCharz().isLockMove)
								{
									if (Char.myCharz().cx - Char.myCharz().cxSend != 0)
									{
										Service.gI().charMove();
									}
									Char.myCharz().statusMe = 2;
									Char.myCharz().cvx = -Char.myCharz().cspeed;
								}
								Char.myCharz().holder = false;
							}
							else if (GameCanvas.keyHold[(!Main.isPC) ? 6 : 24])
							{
								isAutoPlay = false;
								Char.myCharz().isAttack = false;
								if (Char.myCharz().cdir == -1)
								{
									Char.myCharz().cdir = 1;
								}
								else if (!Char.myCharz().isLockMove)
								{
									if (Char.myCharz().cx - Char.myCharz().cxSend != 0)
									{
										Service.gI().charMove();
									}
									Char.myCharz().statusMe = 2;
									Char.myCharz().cvx = Char.myCharz().cspeed;
								}
								Char.myCharz().holder = false;
							}
						}
					}
					else if (Char.myCharz().statusMe == 2)
					{
						GameCanvas.debug("F11", 0);
						if (GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25])
						{
							GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25] = false;
							doFire(isFireByShortCut: false, skipWaypoint: true);
						}
						else if (GameCanvas.keyHold[(!Main.isPC) ? 2 : 21])
						{
							if (Char.myCharz().cx - Char.myCharz().cxSend != 0 || Char.myCharz().cy - Char.myCharz().cySend != 0)
							{
								Service.gI().charMove();
							}
							Char.myCharz().cvy = -10;
							Char.myCharz().statusMe = 3;
							Char.myCharz().cp1 = 0;
						}
						else if (GameCanvas.keyHold[1] && mobCapcha == null)
						{
							if (Main.isPC)
							{
								if (Char.myCharz().cx - Char.myCharz().cxSend != 0 || Char.myCharz().cy - Char.myCharz().cySend != 0)
								{
									Service.gI().charMove();
								}
								Char.myCharz().cdir = -1;
								Char.myCharz().cvy = -10;
								Char.myCharz().cvx = -4;
								Char.myCharz().statusMe = 3;
								Char.myCharz().cp1 = 0;
							}
						}
						else if (GameCanvas.keyHold[3] && mobCapcha == null)
						{
							if (!Main.isPC)
							{
								if (Char.myCharz().cx - Char.myCharz().cxSend != 0 || Char.myCharz().cy - Char.myCharz().cySend != 0)
								{
									Service.gI().charMove();
								}
								Char.myCharz().cdir = 1;
								Char.myCharz().cvy = -10;
								Char.myCharz().cvx = 4;
								Char.myCharz().statusMe = 3;
								Char.myCharz().cp1 = 0;
							}
						}
						else if (GameCanvas.keyHold[(!Main.isPC) ? 4 : 23])
						{
							isAutoPlay = false;
							if (Char.myCharz().cdir == 1)
							{
								Char.myCharz().cdir = -1;
							}
							else
							{
								Char.myCharz().cvx = -Char.myCharz().cspeed + Char.myCharz().cBonusSpeed;
							}
						}
						else if (GameCanvas.keyHold[(!Main.isPC) ? 6 : 24])
						{
							isAutoPlay = false;
							if (Char.myCharz().cdir == -1)
							{
								Char.myCharz().cdir = 1;
							}
							else
							{
								Char.myCharz().cvx = Char.myCharz().cspeed + Char.myCharz().cBonusSpeed;
							}
						}
					}
					else if (Char.myCharz().statusMe == 3)
					{
						isAutoPlay = false;
						GameCanvas.debug("F12", 0);
						if (GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25])
						{
							GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25] = false;
							doFire(isFireByShortCut: false, skipWaypoint: true);
						}
						if (GameCanvas.keyHold[(!Main.isPC) ? 4 : 23] || (GameCanvas.keyHold[1] && mobCapcha == null))
						{
							if (Char.myCharz().cdir == 1)
							{
								Char.myCharz().cdir = -1;
							}
							else
							{
								Char.myCharz().cvx = -Char.myCharz().cspeed;
							}
						}
						else if (GameCanvas.keyHold[(!Main.isPC) ? 6 : 24] || (GameCanvas.keyHold[3] && mobCapcha == null))
						{
							if (Char.myCharz().cdir == -1)
							{
								Char.myCharz().cdir = 1;
							}
							else
							{
								Char.myCharz().cvx = Char.myCharz().cspeed;
							}
						}
						if ((GameCanvas.keyHold[(!Main.isPC) ? 2 : 21] || ((GameCanvas.keyHold[1] || GameCanvas.keyHold[3]) && mobCapcha == null)) && Char.myCharz().canFly && Char.myCharz().cMP > 0 && Char.myCharz().cp1 < 8 && Char.myCharz().cvy > -4)
						{
							Char.myCharz().cp1++;
							Char.myCharz().cvy = -7;
						}
					}
					else if (Char.myCharz().statusMe == 4)
					{
						GameCanvas.debug("F13", 0);
						if (GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25])
						{
							GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25] = false;
							doFire(isFireByShortCut: false, skipWaypoint: true);
						}
						if (GameCanvas.keyHold[(!Main.isPC) ? 2 : 21] && Char.myCharz().cMP > 0 && Char.myCharz().canFly)
						{
							isAutoPlay = false;
							if ((Char.myCharz().cx - Char.myCharz().cxSend != 0 || Char.myCharz().cy - Char.myCharz().cySend != 0) && (Res.abs(Char.myCharz().cx - Char.myCharz().cxSend) > 96 || Res.abs(Char.myCharz().cy - Char.myCharz().cySend) > 24))
							{
								Service.gI().charMove();
							}
							Char.myCharz().cvy = -10;
							Char.myCharz().statusMe = 3;
							Char.myCharz().cp1 = 0;
						}
						if (GameCanvas.keyHold[(!Main.isPC) ? 4 : 23])
						{
							isAutoPlay = false;
							if (Char.myCharz().cdir == 1)
							{
								Char.myCharz().cdir = -1;
							}
							else
							{
								Char.myCharz().cp1++;
								Char.myCharz().cvx = -Char.myCharz().cspeed;
								if (Char.myCharz().cp1 > 5 && Char.myCharz().cvy > 6)
								{
									Char.myCharz().statusMe = 10;
									Char.myCharz().cp1 = 0;
									Char.myCharz().cvy = 0;
								}
							}
						}
						else if (GameCanvas.keyHold[(!Main.isPC) ? 6 : 24])
						{
							isAutoPlay = false;
							if (Char.myCharz().cdir == -1)
							{
								Char.myCharz().cdir = 1;
							}
							else
							{
								Char.myCharz().cp1++;
								Char.myCharz().cvx = Char.myCharz().cspeed;
								if (Char.myCharz().cp1 > 5 && Char.myCharz().cvy > 6)
								{
									Char.myCharz().statusMe = 10;
									Char.myCharz().cp1 = 0;
									Char.myCharz().cvy = 0;
								}
							}
						}
					}
					else if (Char.myCharz().statusMe == 10)
					{
						GameCanvas.debug("F14", 0);
						if (GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25])
						{
							GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25] = false;
							doFire(isFireByShortCut: false, skipWaypoint: true);
						}
						if (Char.myCharz().canFly && Char.myCharz().cMP > 0)
						{
							if (GameCanvas.keyHold[(!Main.isPC) ? 2 : 21])
							{
								isAutoPlay = false;
								if ((Char.myCharz().cx - Char.myCharz().cxSend != 0 || Char.myCharz().cy - Char.myCharz().cySend != 0) && (Res.abs(Char.myCharz().cx - Char.myCharz().cxSend) > 96 || Res.abs(Char.myCharz().cy - Char.myCharz().cySend) > 24))
								{
									Service.gI().charMove();
								}
								Char.myCharz().cvy = -10;
								Char.myCharz().statusMe = 3;
								Char.myCharz().cp1 = 0;
							}
							else if (GameCanvas.keyHold[(!Main.isPC) ? 4 : 23])
							{
								isAutoPlay = false;
								if (Char.myCharz().cdir == 1)
								{
									Char.myCharz().cdir = -1;
								}
								else
								{
									Char.myCharz().cvx = -(Char.myCharz().cspeed + 1);
								}
							}
							else if (GameCanvas.keyHold[(!Main.isPC) ? 6 : 24])
							{
								if (Char.myCharz().cdir == -1)
								{
									Char.myCharz().cdir = 1;
								}
								else
								{
									Char.myCharz().cvx = Char.myCharz().cspeed + 1;
								}
							}
						}
					}
					else if (Char.myCharz().statusMe == 7)
					{
						GameCanvas.debug("F15", 0);
						if (GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25])
						{
							GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25] = false;
						}
						if (GameCanvas.keyHold[(!Main.isPC) ? 4 : 23])
						{
							isAutoPlay = false;
							if (Char.myCharz().cdir == 1)
							{
								Char.myCharz().cdir = -1;
							}
							else
							{
								Char.myCharz().cvx = -Char.myCharz().cspeed + 2;
							}
						}
						else if (GameCanvas.keyHold[(!Main.isPC) ? 6 : 24])
						{
							isAutoPlay = false;
							if (Char.myCharz().cdir == -1)
							{
								Char.myCharz().cdir = 1;
							}
							else
							{
								Char.myCharz().cvx = Char.myCharz().cspeed - 2;
							}
						}
					}
					GameCanvas.debug("F17", 0);
					if (GameCanvas.keyPressed[(!Main.isPC) ? 8 : 22] && GameCanvas.keyAsciiPress != 56)
					{
						GameCanvas.keyPressed[(!Main.isPC) ? 8 : 22] = false;
						Char.myCharz().delayFall = 0;
					}
					if (GameCanvas.keyPressed[10])
					{
						GameCanvas.keyPressed[10] = false;
						doUseHP();
					}
					GameCanvas.debug("F20", 0);
					GameCanvas.clearKeyPressed();
					GameCanvas.debug("F23", 0);
					doSeleckSkillFlag = false;
				}
			}

	private void checkClick()
			{
				if (isCharging())
				{
					return;
				}
				if (ModArrowButton.CheckClick())
				{
					return;
				}
				if (ModMenu.uiCustomOpen)
				{
					return;
				}
				if (cmdMenu != null)
				{
					int menuW = (cmdMenu.w > 0) ? cmdMenu.w : 64;
					int menuH = (cmdMenu.h > 0) ? cmdMenu.h : 34;
					bool isClickMenuBtn = GameCanvas.isPointerHoldIn(cmdMenu.x, cmdMenu.y, menuW, menuH);
					if (!isClickMenuBtn && GameCanvas.isTouch && GameCanvas.isPointerHoldIn(0, 0, 60, 50))
					{
						isClickMenuBtn = true;
					}
					if (!isClickMenuBtn && !GameCanvas.isTouch && GameCanvas.isPointerHoldIn(0, GameCanvas.h - 35, 65, 35))
					{
						isClickMenuBtn = true;
					}
					if (isClickMenuBtn)
					{
						if (GameCanvas.isPointerClick || GameCanvas.isPointerJustRelease)
						{
							GameCanvas.clearAllPointerEvent();
							Char.myCharz().currentMovePoint = null;
							Char.myCharz().vMovePoints.removeAllElements();
							clickMoving = false;
							cmdMenu.performAction();
							return;
						}
						return;
					}
				}
				if (popUpYesNo != null && popUpYesNo.cmdYes != null && popUpYesNo.cmdYes.isPointerPressInside())
				{
					popUpYesNo.cmdYes.performAction();
				}
				else
				{
					if (checkClickToCapcha())
					{
						return;
					}
					long num = mSystem.currentTimeMillis();
					if (lastSingleClick != 0)
					{
						lastSingleClick = 0L;
						GameCanvas.isPointerJustDown = false;
						if (!disableSingleClick)
						{
							checkSingleClick();
							GameCanvas.isPointerJustRelease = false;
							isWaitingDoubleClick = true;
							timeStartDblClick = mSystem.currentTimeMillis();
						}
					}
					if (isWaitingDoubleClick)
					{
						timeEndDblClick = mSystem.currentTimeMillis();
						if (timeEndDblClick - timeStartDblClick < 300 && GameCanvas.isPointerJustRelease)
						{
							isWaitingDoubleClick = false;
							checkDoubleClick();
						}
					}
					if (GameCanvas.isPointerJustRelease)
					{
						disableSingleClick = checkSingleClickEarly();
						lastSingleClick = num;
						lastClickCMX = cmx;
						lastClickCMY = cmy;
						GameCanvas.isPointerJustRelease = false;
					}
				}
			}

	private IMapObject findClickToItem(int px, int py)
			{
				IMapObject mapObject = null;
				int num = 0;
				int num2 = 30;
				MyVector[] array = new MyVector[4] { vMob, vNpc, vItemMap, vCharInMap };
				for (int i = 0; i < array.Length; i++)
				{
					for (int j = 0; j < array[i].size(); j++)
					{
						IMapObject mapObject2 = (IMapObject)array[i].elementAt(j);
						if (mapObject2.isInvisible())
						{
							continue;
						}
						if (mapObject2 is Mob)
						{
							Mob mob = (Mob)mapObject2;
							if (mob.isMobMe && mob.Equals(Char.myCharz().mobMe))
							{
								continue;
							}
						}
						int x = mapObject2.getX();
						int y = mapObject2.getY();
						int w = mapObject2.getW();
						int h = mapObject2.getH();
						if (!inRectangle(px, py, x - w / 2 - num2, y - h - num2, w + num2 * 2, h + num2 * 2))
						{
							continue;
						}
						if (mapObject == null)
						{
							mapObject = mapObject2;
							num = Res.abs(px - x) + Res.abs(py - y);
							if (i == 1)
							{
								return mapObject;
							}
						}
						else
						{
							int num3 = Res.abs(px - x) + Res.abs(py - y);
							if (num3 < num)
							{
								mapObject = mapObject2;
								num = num3;
							}
						}
					}
				}
				return mapObject;
			}

	private Mob findClickToMOB(int px, int py)
			{
				int num = 30;
				Mob mob = null;
				int num2 = 0;
				for (int i = 0; i < vMob.size(); i++)
				{
					Mob mob2 = (Mob)vMob.elementAt(i);
					if (mob2.isInvisible())
					{
						continue;
					}
					if (mob2 != null)
					{
						Mob mob3 = mob2;
						if (mob3.isMobMe && mob3.Equals(Char.myCharz().mobMe))
						{
							continue;
						}
					}
					int x = mob2.getX();
					int y = mob2.getY();
					int w = mob2.getW();
					int h = mob2.getH();
					if (!inRectangle(px, py, x - w / 2 - num, y - h - num, w + num * 2, h + num * 2))
					{
						continue;
					}
					if (mob == null)
					{
						mob = mob2;
						num2 = Res.abs(px - x) + Res.abs(py - y);
						continue;
					}
					int num3 = Res.abs(px - x) + Res.abs(py - y);
					if (num3 < num2)
					{
						mob = mob2;
						num2 = num3;
					}
				}
				return mob;
			}

	private bool checkSingleClickEarly()
			{
				int num = GameCanvas.px + cmx;
				int num2 = GameCanvas.py + cmy;
				Char.myCharz().cancelAttack();
				IMapObject mapObject = findClickToItem(num, num2);
				if (mapObject != null)
				{
					if (Char.myCharz().isAttacPlayerStatus() && Char.myCharz().charFocus != null && !mapObject.Equals(Char.myCharz().charFocus) && !mapObject.Equals(Char.myCharz().charFocus.mobMe) && mapObject is Char)
					{
						Char @char = (Char)mapObject;
						if (@char.cTypePk != 5 && !@char.isAttacPlayerStatus())
						{
							checkClickMoveTo(num, num2, 2);
							return false;
						}
					}
					if (Char.myCharz().mobFocus == mapObject || Char.myCharz().itemFocus == mapObject)
					{
						doDoubleClickToObj(mapObject);
						return true;
					}
					if (TileMap.mapID == 51 && mapObject.Equals(Char.myCharz().npcFocus))
					{
						checkClickMoveTo(num, num2, 3);
						return false;
					}
					if (Char.myCharz().skillPaint != null || Char.myCharz().arr != null || Char.myCharz().dart != null || Char.myCharz().skillInfoPaint() != null)
					{
						return false;
					}
					Char.myCharz().focusManualTo(mapObject);
					mapObject.stopMoving();
					return false;
				}
				return false;
			}

	private void checkDoubleClick()
			{
				int num = GameCanvas.px + lastClickCMX;
				int num2 = GameCanvas.py + lastClickCMY;
				int cy = Char.myCharz().cy;
				if (isLockKey)
				{
					return;
				}
				IMapObject mapObject = findClickToItem(num, num2);
				if (mapObject != null)
				{
					if (mapObject is Mob && !isMeCanAttackMob((Mob)mapObject))
					{
						checkClickMoveTo(num, num2, 4);
					}
					else
					{
						if (checkClickToBotton(mapObject) || (!mapObject.Equals(Char.myCharz().npcFocus) && mobCapcha != null))
						{
							return;
						}
						if (Char.myCharz().isAttacPlayerStatus() && Char.myCharz().charFocus != null && !mapObject.Equals(Char.myCharz().charFocus) && !mapObject.Equals(Char.myCharz().charFocus.mobMe) && mapObject is Char)
						{
							Char @char = (Char)mapObject;
							if (@char.cTypePk != 5 && !@char.isAttacPlayerStatus())
							{
								checkClickMoveTo(num, num2, 5);
								return;
							}
						}
						if (TileMap.mapID == 51 && mapObject.Equals(Char.myCharz().npcFocus))
						{
							checkClickMoveTo(num, num2, 6);
						}
						else
						{
							doDoubleClickToObj(mapObject);
						}
					}
				}
				else if (!checkClickToPopup(num, num2) && !checkClipTopChatPopUp(num, num2) && !Main.isPC)
				{
					checkClickMoveTo(num, num2, 7);
				}
			}

	private bool checkClickToBotton(IMapObject Object)
			{
				if (Object == null)
				{
					return false;
				}
				int y = Object.getY();
				int num = Char.myCharz().cy;
				if (y < num)
				{
					while (y < num)
					{
						num -= 5;
						if (TileMap.tileTypeAt(Char.myCharz().cx, num, 8192))
						{
							auto = 0;
							Char.myCharz().cancelAttack();
							Char.myCharz().currentMovePoint = null;
							return true;
						}
					}
				}
				return false;
			}

	private void doDoubleClickToObj(IMapObject obj)
			{
				if ((obj.Equals(Char.myCharz().npcFocus) || mobCapcha == null) && !checkClickToBotton(obj))
				{
					checkEffToObj(obj, isnew: false);
					Char.myCharz().cancelAttack();
					Char.myCharz().currentMovePoint = null;
					Char.myCharz().cvx = (Char.myCharz().cvy = 0);
					obj.stopMoving();
					auto = 10;
					doFire(isFireByShortCut: false, skipWaypoint: true);
					clickToX = obj.getX();
					clickToY = obj.getY();
					clickOnTileTop = false;
					clickMoving = true;
					clickMovingRed = true;
					clickMovingTimeOut = 20;
					clickMovingP1 = 30;
				}
			}

	private void checkSingleClick()
			{
				int xClick = GameCanvas.px + lastClickCMX;
				int yClick = GameCanvas.py + lastClickCMY;
				if (!isLockKey && !checkClickToPopup(xClick, yClick) && !checkClipTopChatPopUp(xClick, yClick))
				{
					checkClickMoveTo(xClick, yClick, 0);
				}
			}

	private void checkClickMoveTo(int xClick, int yClick, int index)
			{
				if (gamePad.disableClickMove())
				{
					return;
				}
				Char.myCharz().cancelAttack();
				if (xClick < TileMap.pxw && xClick > TileMap.pxw - 32)
				{
					Char.myCharz().currentMovePoint = new MovePoint(TileMap.pxw, yClick);
					return;
				}
				if (xClick < 32 && xClick > 0)
				{
					Char.myCharz().currentMovePoint = new MovePoint(0, yClick);
					return;
				}
				if (xClick < TileMap.pxw && xClick > TileMap.pxw - 48)
				{
					Char.myCharz().currentMovePoint = new MovePoint(TileMap.pxw, yClick);
					return;
				}
				if (xClick < 48 && xClick > 0)
				{
					Char.myCharz().currentMovePoint = new MovePoint(0, yClick);
					return;
				}
				clickToX = xClick;
				clickToY = yClick;
				clickOnTileTop = false;
				Char.myCharz().delayFall = 0;
				int num = ((!Char.myCharz().canFly || Char.myCharz().cMP <= 0) ? 1000 : 0);
				if (clickToY > Char.myCharz().cy && Res.abs(clickToX - Char.myCharz().cx) < 12)
				{
					return;
				}
				for (int i = 0; i < 60 + num && clickToY + i < TileMap.pxh - 24; i += 24)
				{
					if (TileMap.tileTypeAt(clickToX, clickToY + i, 2))
					{
						clickToY = TileMap.tileYofPixel(clickToY + i);
						clickOnTileTop = true;
						break;
					}
				}
				for (int j = 0; j < 40 + num; j += 24)
				{
					if (TileMap.tileTypeAt(clickToX, clickToY - j, 2))
					{
						clickToY = TileMap.tileYofPixel(clickToY - j);
						clickOnTileTop = true;
						break;
					}
				}
				clickMoving = true;
				clickMovingRed = false;
				clickMovingP1 = ((!clickOnTileTop) ? 30 : ((yClick >= clickToY) ? clickToY : yClick));
				Char.myCharz().delayFall = 0;
				if (!clickOnTileTop && clickToY < Char.myCharz().cy - 50)
				{
					Char.myCharz().delayFall = 20;
				}
				clickMovingTimeOut = 30;
				auto = 0;
				if (Char.myCharz().holder)
				{
					Char.myCharz().removeHoleEff();
				}
				Char.myCharz().currentMovePoint = new MovePoint(clickToX, clickToY);
				Char.myCharz().cdir = ((Char.myCharz().cx - Char.myCharz().currentMovePoint.xEnd <= 0) ? 1 : (-1));
				Char.myCharz().endMovePointCommand = null;
				isAutoPlay = false;
			}

	public void updateKeyTouchCapcha()
			{
				if (isNotPaintTouchControl())
				{
					return;
				}
				for (int i = 0; i < strCapcha.Length; i++)
				{
					keyCapcha[i] = -1;
					if (!GameCanvas.isTouchControl)
					{
						continue;
					}
					int num = (GameCanvas.w - strCapcha.Length * disXC) / 2;
					int w = strCapcha.Length * disXC;
					int y = GameCanvas.h - 40;
					int h = disXC;
					if (!GameCanvas.isPointerHoldIn(num, y, w, h))
					{
						continue;
					}
					int num2 = (GameCanvas.px - num) / disXC;
					if (i == num2)
					{
						keyCapcha[i] = 1;
					}
					if (GameCanvas.isPointerClick && GameCanvas.isPointerJustRelease && i == num2)
					{
						char[] array = keyInput.ToCharArray();
						MyVector myVector = new MyVector();
						for (int j = 0; j < array.Length; j++)
						{
							myVector.addElement(array[j] + string.Empty);
						}
						myVector.removeElementAt(0);
						myVector.insertElementAt(strCapcha[i] + string.Empty, myVector.size());
						keyInput = string.Empty;
						for (int k = 0; k < myVector.size(); k++)
						{
							keyInput += ((string)myVector.elementAt(k)).ToUpper();
						}
						Service.gI().mobCapcha(strCapcha[i]);
					}
				}
			}

	public bool checkClickToCapcha()
			{
				if (mobCapcha == null)
				{
					return false;
				}
				int x = (GameCanvas.w - 5 * disXC) / 2;
				int w = 5 * disXC;
				int y = GameCanvas.h - 40;
				int h = disXC;
				if (GameCanvas.isPointerHoldIn(x, y, w, h))
				{
					return true;
				}
				return false;
			}

	private void updateKeyTouchControl()
			{
				if (isNotPaintTouchControl())
				{
					return;
				}
				mScreen.keyTouch = -1;
				if (GameCanvas.isTouchControl)
				{
					if (GameCanvas.isPointerHoldIn(0, 0, 60, 50) && (GameCanvas.isPointerClick || GameCanvas.isPointerJustRelease))
					{
						if (cmdMenu != null)
						{
							cmdMenu.performAction();
						}
						else if (Char.myCharz().cmdMenu != null)
						{
							Char.myCharz().cmdMenu.performAction();
						}
						Char.myCharz().currentMovePoint = null;
						Char.myCharz().vMovePoints.removeAllElements();
						clickMoving = false;
						GameCanvas.clearAllPointerEvent();
						flareFindFocus = true;
						flareTime = 5;
						return;
					}
					if (Main.isPC)
					{
						checkMouseChat();
					}
					if (!TileMap.isOfflineMap() && GameCanvas.isPointerHoldIn(xC, yC, 34, 34))
					{
						mScreen.keyTouch = 15;
						GameCanvas.isPointerJustDown = false;
						isPointerDowning = false;
						if (GameCanvas.isPointerClick && GameCanvas.isPointerJustRelease)
						{
							ChatTextField.gI().startChat(this, string.Empty);
							SoundMn.gI().buttonClick();
							Char.myCharz().currentMovePoint = null;
							GameCanvas.clearAllPointerEvent();
							return;
						}
					}
					if (Char.myCharz().cmdMenu != null && GameCanvas.isPointerHoldIn(Char.myCharz().cmdMenu.x - 17, Char.myCharz().cmdMenu.y - 17, 34, 34))
					{
						mScreen.keyTouch = 20;
						GameCanvas.isPointerJustDown = false;
						isPointerDowning = false;
						if (GameCanvas.isPointerClick && GameCanvas.isPointerJustRelease)
						{
							GameCanvas.clearAllPointerEvent();
							Char.myCharz().cmdMenu.performAction();
							return;
						}
					}
					updateGamePad();
					if (((isAnalog != 0) ? GameCanvas.isPointerHoldIn(xHP, yHP + 10, 34, 34) : GameCanvas.isPointerHoldIn(xHP, yHP + 10, 40, 40)) && Char.myCharz().statusMe != 14 && mobCapcha == null)
					{
						mScreen.keyTouch = 10;
						GameCanvas.isPointerJustDown = false;
						isPointerDowning = false;
						if (GameCanvas.isPointerClick && GameCanvas.isPointerJustRelease)
						{
							GameCanvas.keyPressed[10] = true;
							GameCanvas.isPointerClick = (GameCanvas.isPointerJustDown = (GameCanvas.isPointerJustRelease = false));
						}
					}
					if (((isAnalog != 0) ? GameCanvas.isPointerHoldIn(xHP + 5, yHP - 6 - 34 + 10, 34, 34) : GameCanvas.isPointerHoldIn(xHP + 5, yHP - 6 - 40 + 10, 40, 40)) && Char.myCharz().statusMe != 14 && mobCapcha == null)
					{
						if (isPickNgocRong)
						{
							mScreen.keyTouch = 14;
							GameCanvas.isPointerJustDown = false;
							isPointerDowning = false;
							if (GameCanvas.isPointerClick && GameCanvas.isPointerJustRelease)
							{
								GameCanvas.keyPressed[14] = true;
								GameCanvas.isPointerClick = (GameCanvas.isPointerJustDown = (GameCanvas.isPointerJustRelease = false));
								isPickNgocRong = false;
								Service.gI().useItem(-1, -1, -1, -1);
							}
						}
						else if (isudungCapsun4)
						{
							mScreen.keyTouch = 14;
							GameCanvas.isPointerJustDown = false;
							isPointerDowning = false;
							if (GameCanvas.isPointerClick && GameCanvas.isPointerJustRelease)
							{
								GameCanvas.keyPressed[14] = true;
								GameCanvas.isPointerClick = (GameCanvas.isPointerJustDown = (GameCanvas.isPointerJustRelease = false));
								for (int i = 0; i < Char.myCharz().arrItemBag.Length; i++)
								{
									Item item = Char.myCharz().arrItemBag[i];
									if (item == null)
									{
										continue;
									}
									Res.err("find " + item.template.id);
									if (item.template.id == 194)
									{
										isudungCapsun4 = item.quantity > 0;
										if (isudungCapsun4)
										{
											Service.gI().useItem(0, 1, (sbyte)i, -1);
											break;
										}
									}
								}
							}
						}
						else if (isudungCapsun3)
						{
							mScreen.keyTouch = 14;
							GameCanvas.isPointerJustDown = false;
							isPointerDowning = false;
							if (GameCanvas.isPointerClick && GameCanvas.isPointerJustRelease)
							{
								GameCanvas.keyPressed[14] = true;
								GameCanvas.isPointerClick = (GameCanvas.isPointerJustDown = (GameCanvas.isPointerJustRelease = false));
								for (int j = 0; j < Char.myCharz().arrItemBag.Length; j++)
								{
									Item item2 = Char.myCharz().arrItemBag[j];
									if (item2 != null && item2.template.id == 193)
									{
										isudungCapsun3 = item2.quantity > 0;
										if (isudungCapsun3)
										{
											Service.gI().useItem(0, 1, (sbyte)j, -1);
											break;
										}
									}
								}
							}
						}
					}
				}
				if (mobCapcha != null)
				{
					updateKeyTouchCapcha();
				}
				else if (isHaveSelectSkill)
				{
					if (isCharging())
					{
						return;
					}
					keyTouchSkill = -1;
					int totalSlots = (Main.isPC ? keySkill.Length : onScreenSkill.Length);
					int totalW = totalSlots * wSkill;
					if (GameCanvas.isPointerHoldIn(xSkill + xS[0] - 2, yS[0] - 2, totalW + 4, wSkill + 4))
					{
						GameCanvas.isPointerJustDown = false;
						isPointerDowning = false;
						int num = (GameCanvas.pxLast - (xSkill + xS[0])) / wSkill;
						if (num >= 0 && num < totalSlots)
						{
							keyTouchSkill = num;
							if (GameCanvas.isPointerClick && GameCanvas.isPointerJustRelease)
							{
								GameCanvas.isPointerClick = (GameCanvas.isPointerJustDown = (GameCanvas.isPointerJustRelease = false));
								selectedIndexSkill = num;
								if (indexSelect < 0)
								{
									indexSelect = 0;
								}
								Skill[] skillArr = (Main.isPC ? keySkill : onScreenSkill);
								if (selectedIndexSkill < skillArr.Length)
								{
									Skill skill = skillArr[selectedIndexSkill];
									if (skill != null)
									{
										doSelectSkill(skill, isShortcut: true);
									}
								}
							}
						}
					}
				}
				if (GameCanvas.isPointerJustRelease)
				{
					if (GameCanvas.keyHold[1] || GameCanvas.keyHold[(!Main.isPC) ? 2 : 21] || GameCanvas.keyHold[3] || GameCanvas.keyHold[(!Main.isPC) ? 4 : 23] || GameCanvas.keyHold[(!Main.isPC) ? 6 : 24])
					{
						GameCanvas.isPointerJustRelease = false;
					}
					GameCanvas.keyHold[1] = false;
					GameCanvas.keyHold[(!Main.isPC) ? 2 : 21] = false;
					GameCanvas.keyHold[3] = false;
					GameCanvas.keyHold[(!Main.isPC) ? 4 : 23] = false;
					GameCanvas.keyHold[(!Main.isPC) ? 6 : 24] = false;
				}
			}

	private void updateClickToArrow()
			{
				if (tDoubleDelay > 0)
				{
					tDoubleDelay--;
				}
				if (clickMoving)
				{
					clickMoving = false;
					IMapObject mapObject = findClickToItem(clickToX, clickToY);
					if (mapObject == null || (mapObject != null && mapObject.Equals(Char.myCharz().npcFocus) && TileMap.mapID == 51))
					{
						ServerEffect.addServerEffect(134, clickToX, clickToY + GameCanvas.transY / 2, 3);
					}
				}
			}

	public void updateKeyAlert()
			{
				if (!isPaintAlert || GameCanvas.currentDialog != null)
				{
					return;
				}
				bool flag = false;
				if (GameCanvas.keyPressed[Key.NUM8])
				{
					indexRow++;
					if (indexRow >= texts.size())
					{
						indexRow = 0;
					}
					flag = true;
				}
				else if (GameCanvas.keyPressed[Key.NUM2])
				{
					indexRow--;
					if (indexRow < 0)
					{
						indexRow = texts.size() - 1;
					}
					flag = true;
				}
				if (flag)
				{
					scrMain.moveTo(indexRow * scrMain.ITEM_SIZE);
					GameCanvas.clearKeyHold();
					GameCanvas.clearKeyPressed();
				}
				if (GameCanvas.isTouch)
				{
					ScrollResult scrollResult = scrMain.updateKey();
					if (scrollResult.isDowning || scrollResult.isFinish)
					{
						indexRow = scrollResult.selected;
						flag = true;
					}
				}
				if (!flag || indexRow < 0 || indexRow >= texts.size())
				{
					return;
				}
				string text = (string)texts.elementAt(indexRow);
				int num = -1;
				fnick = null;
				alertURL = null;
				center = null;
				ChatTextField.gI().center = null;
				if ((num = text.IndexOf("http://")) >= 0)
				{
					Cout.println("currentLine: " + text);
					alertURL = text.Substring(num);
					center = new Command(mResources.open_link, 12000);
					if (!GameCanvas.isTouch)
					{
						ChatTextField.gI().center = new Command(mResources.open_link, null, 12000, null);
					}
				}
				else
				{
					if ((num = text.IndexOf("@")) < 0)
					{
						return;
					}
					string text2 = text.Substring(2);
					text2 = text2.Trim();
					num = text2.IndexOf("@");
					string text3 = text2.Substring(num);
					int num2 = -1;
					num2 = text3.IndexOf(" ");
					num2 = ((num2 > 0) ? (num2 + num) : (num + text3.Length));
					fnick = text2.Substring(num + 1, num2);
					if (!fnick.Equals(string.Empty) && !fnick.Equals(Char.myCharz().cName))
					{
						center = new Command(mResources.SELECT, 12009, fnick);
						if (!GameCanvas.isTouch)
						{
							ChatTextField.gI().center = new Command(mResources.SELECT, null, 12009, fnick);
						}
					}
					else
					{
						fnick = null;
						center = null;
					}
				}
			}

	public bool isNotPaintTouchControl()
			{
				if (!GameCanvas.isTouchControl && GameCanvas.currentScreen == gI())
				{
					return true;
				}
				if (!GameCanvas.isTouch)
				{
					return true;
				}
				if (ChatTextField.gI().isShow)
				{
					return true;
				}
				if (InfoDlg.isShow)
				{
					return true;
				}
				if (GameCanvas.currentDialog != null || ChatPopup.currChatPopup != null || GameCanvas.menu.showMenu || GameCanvas.panel.isShow || isPaintPopup())
				{
					return true;
				}
				return false;
			}

	private static void setTouchBtn()
			{
				if (isAnalog != 0)
				{
					xTG = (xF = GameCanvas.w - 45);
					if (gamePad.isLargeGamePad)
					{
						xSkill = gamePad.wZone + 20;
						wSkill = 35;
						xHP = xF - 45;
					}
					else if (gamePad.isMediumGamePad)
					{
						xHP = xF - 45;
					}
					yF = GameCanvas.h - 45;
					yTG = yF - 45;
				}
			}

}
