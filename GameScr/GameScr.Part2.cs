using System;
using Assets.src.g;
public partial class GameScr : mScreen, IChatable
{
	public bool testAct()
		{
			for (sbyte b = 2; b < 9; b += 2)
			{
				if (GameCanvas.keyHold[b])
				{
					return false;
				}
			}
			return true;
		}
	public void clanInvite(string strInvite, int clanID, int code)
		{
			ClanObject clanObject = new ClanObject();
			clanObject.code = code;
			clanObject.clanID = clanID;
			startYesNoPopUp(strInvite, new Command(mResources.YES, 12002, clanObject), new Command(mResources.NO, 12003, clanObject));
		}

}
