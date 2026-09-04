using System;
using Assets.src.g;
public partial class GameScr : mScreen, IChatable
{
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

}
