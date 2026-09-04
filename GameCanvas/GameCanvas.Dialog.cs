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
