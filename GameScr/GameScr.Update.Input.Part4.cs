using System;
using Assets.src.g;
/// <summary>
/// Thành phần phân mảnh (partial) của lớp GameScr: Đảm nhiệm vòng lặp cập nhật trạng thái logic theo từng khung hình (Input).
/// </summary>
public partial class GameScr : mScreen, IChatable
{
	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của checkSingleClick.
	/// </summary>
	private void checkSingleClick()
			{
				int xClick = GameCanvas.px + lastClickCMX;
				int yClick = GameCanvas.py + lastClickCMY;
				if (!isLockKey && !checkClickToPopup(xClick, yClick) && !checkClipTopChatPopUp(xClick, yClick))
				{
					IMapObject mapObject = findClickToItem(xClick, yClick);
					if (mapObject != null)
					{
						Char.myCharz().focusManualTo(mapObject);
					}
				}
			}
	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của checkClickMoveTo.
	/// </summary>
	private void checkClickMoveTo(int xClick, int yClick, int index)
			{
				// Bỏ hoàn toàn việc nhân vật tự động di chuyển theo vị trí click chuột bên ngoài
				return;
			}
	/// <summary>
	/// Cập nhật trạng thái logic của updateKeyTouchCapcha theo chu kỳ khung hình.
	/// </summary>
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
	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của checkClickToCapcha.
	/// </summary>
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
	/// <summary>
	/// Cập nhật trạng thái logic của updateKeyTouchControl theo chu kỳ khung hình.
	/// </summary>
	private void updateKeyTouchControl()
			{
				if (isNotPaintTouchControl())
				{
					return;
				}
				if (ModArrowButton.CheckClick())
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
					int peaR = (isAnalog != 0) ? ModArrowButton.FAN_BTN_RADIUS : 20;
					if (ModArrowButton.IsPointerHoldInCircle(xHP + peaR, yHP + peaR, peaR) && Char.myCharz().statusMe != 14 && mobCapcha == null)
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
	/// <summary>
	/// Cập nhật trạng thái logic của updateClickToArrow theo chu kỳ khung hình.
	/// </summary>
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

}
