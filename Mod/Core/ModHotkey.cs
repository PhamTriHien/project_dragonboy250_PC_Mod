using System;
using UnityEngine;

public static class ModHotkey
{
	private static int _lastHotkeyFrame = -1;

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
			// Phím F11 hoặc Alt + Enter: Bật / Tắt Toàn Màn Hình (Fullscreen) ở mọi nơi
			if (Input.GetKeyDown(KeyCode.F11) || ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && Input.GetKeyDown(KeyCode.Return)))
			{
				ModGraphics.ToggleFullscreen();
			}

			// Phím F10: Chuyển đổi chế độ Đồ Hoạ (0: Đồ Hoạ Java / 1: Đồ Hoạ HD) ở mọi màn hình
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

			// Kiểm tra phím tắt PC khi không trong khung chat
			if (ChatTextField.gI() != null && ChatTextField.gI().isShow)
			{
				return;
			}

			// Phím ~ (BackQuote) hoặc F2: Bật/Tắt Mod Menu
			if (Input.GetKeyDown(KeyCode.BackQuote) || Input.GetKeyDown(KeyCode.F2))
			{
				ToggleModMenu();
			}

			// Phím Home: Giải kẹt khẩn cấp nhân vật
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

				// Đẩy nhân vật ra khỏi cổng nếu đang chạm cổng
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
