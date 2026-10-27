using System;
using UnityEngine;

/// <summary>
/// Quản lý hệ thống phím tắt PC toàn cục và phím tắt tác vụ nhanh trong màn hình chơi game.
/// </summary>
public static class ModHotkey
{
	public static bool enableQuickPcHotkeys = true;
	private static int _lastHotkeyFrame = -1;

	public static bool IsTypingOrUiBusy()
	{
		if (!ModMenu.IsInGame()) return true;
		if (ChatTextField.gI() != null && ChatTextField.gI().isShow) return true;
		if (GameCanvas.currentDialog != null) return true;
		if (GameCanvas.panel != null && GameCanvas.panel.isShow) return true;
		if (GameCanvas.panel2 != null && GameCanvas.panel2.isShow) return true;
		if (GameCanvas.menu != null && GameCanvas.menu.showMenu) return true;
		if (ChatPopup.currChatPopup != null) return true;
		if (ModAutoCaptcha.IsCaptchaActive()) return true;
		if (TouchScreenKeyboard.visible) return true;
		if (ModUI.uiCustomOpen) return true;
		if (Input.GetKey((KeyCode)306) || Input.GetKey((KeyCode)305) ||
		    Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
		{
			return true;
		}
		return false;
	}

	/// <summary>
	/// Xử lý phím tắt nhanh PC trước khi chuyển vào mapKeyPress của game (bảo toàn WASD di chuyển).
	/// </summary>
	public static bool HandleKeyPressed(int keyCode)
	{
		try
		{
			if (!enableQuickPcHotkeys || IsTypingOrUiBusy())
			{
				return false;
			}

			Char me = Char.myCharz();
			if (me == null) return false;

			char ch = (keyCode >= 65 && keyCode <= 90) ? (char)(keyCode + 32) : (char)keyCode;
			switch (ch)
			{
				case 'e':
					ModTanSat.autoTanSat = !ModTanSat.autoTanSat;
					ModConfig.SaveConfig();
					GameScr.info1.addInfo("Tự đánh (Tàn Sát): " + (ModTanSat.autoTanSat ? "BẬT" : "TẮT"), 0);
					SoundMn.gI().buttonClick();
					ConsumeKey();
					return true;

				case 'b':
					Service.gI().friend(0, -1);
					SoundMn.gI().buttonClick();
					ConsumeKey();
					return true;

				case 'c':
					ModArrowButton.UseCapsuleQuick();
					ConsumeKey();
					return true;

				case 'f':
					ModArrowButton.UsePotaraFusion();
					ConsumeKey();
					return true;

				case 'g':
					if (me.charFocus != null && me.charFocus.charID > 0)
					{
						Service.gI().giaodich(0, me.charFocus.charID, -1, -1);
						GameScr.info1.addInfo("Gửi lời mời giao dịch tới " + me.charFocus.cName, 0);
						SoundMn.gI().buttonClick();
					}
					else
					{
						GameScr.info1.addInfo("Hãy chọn một người chơi để giao dịch!", 0);
					}
					ConsumeKey();
					return true;

				case 'i':
					ModMapEntityHUD.ToggleLockTarget();
					ConsumeKey();
					return true;

				case 'j':
					ModNextMap.LoadMapDirection(0);
					ConsumeKey();
					return true;

				case 'k':
					ModNextMap.LoadMapDirection(2);
					ConsumeKey();
					return true;

				case 'l':
					ModNextMap.LoadMapDirection(1);
					ConsumeKey();
					return true;

				case 'm':
					if (me.statusMe != 14)
					{
						Service.gI().openUIZone();
						SoundMn.gI().buttonClick();
					}
					ConsumeKey();
					return true;

				case 'n':
					ModAutoPick.autoPick = !ModAutoPick.autoPick;
					ModConfig.SaveConfig();
					GameScr.info1.addInfo("Auto Nhặt: " + (ModAutoPick.autoPick ? "BẬT" : "TẮT"), 0);
					SoundMn.gI().buttonClick();
					ConsumeKey();
					return true;

				case 'o':
					ToggleGameMenu();
					ConsumeKey();
					return true;

				case 'q':
					ModAutoSkill.ToggleAutoAttackInPlace();
					ConsumeKey();
					return true;

				case 't':
					ModArrowButton.UseTdltQuick();
					ConsumeKey();
					return true;

				case 'x':
					ToggleModMenu();
					ConsumeKey();
					return true;

				case 'z':
				{
					sbyte nextFlag = (sbyte)((me.cFlag == 0) ? 8 : 0);
					Service.gI().getFlag(1, nextFlag);
					GameScr.info1.addInfo(nextFlag == 8 ? "Đã bật Cờ Đen" : "Đã tắt Cờ", 0);
					SoundMn.gI().buttonClick();
					ConsumeKey();
					return true;
				}
			}
		}
		catch
		{
		}
		return false;
	}

	private static void ConsumeKey()
	{
		GameCanvas.keyAsciiPress = 0;
		GameCanvas.clearKeyPressed();
	}

	public static void UpdateHotkeys()
	{
		try
		{
			int curFrame = Time.frameCount;
			if (curFrame > 0)
			{
				if (curFrame == _lastHotkeyFrame)
				{
					return;
				}
				_lastHotkeyFrame = curFrame;
			}

			if (Input.GetKeyDown(KeyCode.F11) || ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && Input.GetKeyDown(KeyCode.Return)))
			{
				ModGraphics.ToggleFullscreen();
			}

			if (Input.GetKeyDown(KeyCode.F10))
			{
				ModGraphics.SetRenderMode((ModGraphics.renderMode == 0) ? 1 : 0);
				string modeStr = (ModGraphics.renderMode == 0)
					? "Chế độ đồ hoạ: Đồ Hoạ Java (Src Java Gốc)"
					: "Chế độ đồ hoạ: Đồ Hoạ HD";
				if (GameScr.info1 != null && ModMenu.IsInGame())
				{
					GameScr.info1.addInfo(modeStr, 0);
				}
			}

			if (!ModMenu.IsInGame())
			{
				return;
			}

			if (ChatTextField.gI() != null && ChatTextField.gI().isShow)
			{
				return;
			}

			if (Input.GetKeyDown(KeyCode.F9))
			{
				ModGraphics.ToggleAfkBlackScreen();
			}

			if (Input.GetKeyDown(KeyCode.BackQuote) || Input.GetKeyDown(KeyCode.F2))
			{
				ToggleModMenu();
			}

			if (Input.GetKeyDown(KeyCode.Home))
			{
				EmergencyUnstuck();
			}
		}
		catch
		{
		}
	}

	public static void ToggleModMenu()
	{
		if (ModUI.uiCustomOpen)
		{
			ModMenu.CloseMenu();
		}
		else
		{
			ModMenu.OpenMenu();
		}
	}

	public static void ToggleGameMenu()
	{
		if (GameCanvas.panel != null && GameCanvas.panel.isShow)
		{
			GameCanvas.panel.hide();
		}
		else if (GameScr.instance != null)
		{
			GameScr.instance.actMenu();
		}
	}

	public static void EmergencyUnstuck()
	{
		try
		{
			Char me = Char.myCharz();
			Char.ischangingMap = false;
			Char.isLockKey = false;
			if (me != null)
			{
				me.isLockAttack = false;
				me.isLockMove = false;
				me.currentMovePoint = null;
				me.vMovePoints.removeAllElements();

				if (TileMap.vGo != null)
				{
					for (int w = 0; w < TileMap.vGo.size(); w++)
					{
						Waypoint wp = (Waypoint)TileMap.vGo.elementAt(w);
						if (wp != null && !wp.isEnter && me.cx >= wp.minX && me.cx <= wp.maxX && me.cy >= wp.minY && me.cy <= wp.maxY)
						{
							if (wp.minX <= 24) me.cx = wp.maxX + 20;
							else if (wp.maxX >= TileMap.pxw - 24) me.cx = wp.minX - 20;
							else me.cx += 25;
							break;
						}
					}
				}

				me.cy = TileMap.tileYofPixel(me.cy);
				me.cxSend = me.cx;
				me.cySend = me.cy;
				me.cxFocus = me.cx;
				me.cyFocus = me.cy;
				Service.gI().charMove();
			}

			InfoDlg.hide();
			GameCanvas.endDlg();
			GameCanvas.clearKeyHold();
			GameCanvas.clearKeyPressed();
			GameScr.info1.addInfo("Đã giải kẹt nhân vật!", 0);
		}
		catch
		{
		}
	}
}
