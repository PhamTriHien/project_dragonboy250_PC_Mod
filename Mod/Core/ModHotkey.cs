using System;
using UnityEngine;

public static class ModHotkey
{
	public static void UpdateHotkeys()
	{
		try
		{
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
}
