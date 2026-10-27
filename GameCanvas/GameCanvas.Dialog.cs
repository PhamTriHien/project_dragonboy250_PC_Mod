using System;
using Assets.src.e;
using Assets.src.g;
using UnityEngine;

public partial class GameCanvas : IActionListener
{
	public static void showErrorForm(int type, string moreInfo)
		{
		}

	public static void endDlg()
		{
			if (inputDlg != null)
			{
				inputDlg.tfInput.setMaxTextLenght(500);
			}
			currentDialog = null;
			InfoDlg.hide();
			DragonBoy_Net8_Native.Src.Mod.Security.ModCredentialSecurity.OnLoginFinished();
		}

	public static void clearTransientPopupState()
		{
			currentDialog = null;
			InfoDlg.hide();
			if (menu != null)
			{
				menu.showMenu = false;
			}
			if (panel != null)
			{
				panel.isShow = false;
			}
			if (panel2 != null)
			{
				panel2.isShow = false;
			}
			if (msgdlg != null)
			{
				msgdlg.isWait = false;
			}
			if (GameScr.info1 != null)
			{
				GameScr.info1.info = null;
			}
			ChatPopup.clearTransientPopups();
			Hint.isShow = false;
			Hint.isPaint = false;
			if (PopUp.vPopups != null)
			{
				PopUp.vPopups.removeAllElements();
			}
			if (flyTexts != null)
			{
				flyTexts.removeAllElements();
			}
		}

	public static void startYesNoDlg(string info, int iYes, object pYes, int iNo, object pNo)
		{
			info = Res.changeString(info);
			closeKeyBoard();
			msgdlg.setInfo(info, new Command(mResources.YES, instance, iYes, pYes), new Command(string.Empty, instance, iYes, pYes), new Command(mResources.NO, instance, iNo, pNo));
			msgdlg.show();
		}

	public static void startYesNoDlg(string info, Command cmdYes, Command cmdNo)
		{
			info = Res.changeString(info);
			closeKeyBoard();
			msgdlg.setInfo(info, cmdYes, null, cmdNo);
			msgdlg.show();
		}
}
