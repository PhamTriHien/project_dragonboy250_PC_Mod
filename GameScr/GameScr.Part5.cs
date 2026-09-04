using System;
using Assets.src.g;
public partial class GameScr : mScreen, IChatable
{
	private void pickItem()
		{
			if (Char.myCharz().itemFocus == null)
			{
				return;
			}
			if (Char.myCharz().cx < Char.myCharz().itemFocus.x)
			{
				Char.myCharz().cdir = 1;
			}
			else
			{
				Char.myCharz().cdir = -1;
			}
			int num = Math.abs(Char.myCharz().cx - Char.myCharz().itemFocus.x);
			int num2 = Math.abs(Char.myCharz().cy - Char.myCharz().itemFocus.y);
			if (num <= 40 && num2 < 40)
			{
				GameCanvas.clearKeyHold();
				GameCanvas.clearKeyPressed();
				if (Char.myCharz().itemFocus.template.id != 673)
				{
					Service.gI().pickItem(Char.myCharz().itemFocus.itemMapID);
				}
				else
				{
					askToPick();
				}
			}
			else
			{
				Char.myCharz().currentMovePoint = new MovePoint(Char.myCharz().itemFocus.x, Char.myCharz().itemFocus.y);
				Char.myCharz().endMovePointCommand = new Command(null, null, 8002, null);
				GameCanvas.clearKeyHold();
				GameCanvas.clearKeyPressed();
			}
		}
	public bool isCharging()
		{
			if (Char.myCharz().isFlyAndCharge || Char.myCharz().isUseSkillAfterCharge || Char.myCharz().isStandAndCharge || Char.myCharz().isWaitMonkey || isSuperPower || Char.myCharz().isFreez)
			{
				return true;
			}
			return false;
		}
	public void checkCharFocus()
		{
		}
	public static Npc findNPCInMap(short id)
		{
			for (int i = 0; i < vNpc.size(); i++)
			{
				Npc npc = (Npc)vNpc.elementAt(i);
				if (npc.template.npcTemplateId == id)
				{
					return npc;
				}
			}
			return null;
		}
	public static Char findCharInMap(int charId)
		{
			for (int i = 0; i < vCharInMap.size(); i++)
			{
				Char @char = (Char)vCharInMap.elementAt(i);
				if (@char.charID == charId)
				{
					return @char;
				}
			}
			return null;
		}
	public static Mob findMobInMap(sbyte mobIndex)
		{
			return (Mob)vMob.elementAt(mobIndex);
		}
	public static Mob findMobInMap(int mobId)
		{
			for (int i = 0; i < vMob.size(); i++)
			{
				Mob mob = (Mob)vMob.elementAt(i);
				if (mob.mobId == mobId)
				{
					return mob;
				}
			}
			return null;
		}
	public static Npc getNpcTask()
		{
			for (int i = 0; i < vNpc.size(); i++)
			{
				Npc npc = (Npc)vNpc.elementAt(i);
				if (npc.template.npcTemplateId == getTaskNpcId())
				{
					return npc;
				}
			}
			return null;
		}
	public void getInjure()
		{
		}
	public void starVS()
		{
			curr = (last = mSystem.currentTimeMillis());
			secondVS = 180;
		}
	private Char findCharVS1()
		{
			for (int i = 0; i < vCharInMap.size(); i++)
			{
				Char @char = (Char)vCharInMap.elementAt(i);
				if (@char.cTypePk != 0)
				{
					return @char;
				}
			}
			return null;
		}
	private Char findCharVS2()
		{
			for (int i = 0; i < vCharInMap.size(); i++)
			{
				Char @char = (Char)vCharInMap.elementAt(i);
				if (@char.cTypePk != 0 && @char != findCharVS1())
				{
					return @char;
				}
			}
			return null;
		}
	public bool isVS()
		{
			if (TileMap.isVoDaiMap() && (Char.myCharz().cTypePk != 0 || (TileMap.mapID == 130 && findCharVS1() != null && findCharVS2() != null)))
			{
				return true;
			}
			return false;
		}
	private void loadInforBar()
		{
			imgScrW = 84;
			hpBarW = 66L;
			mpBarW = 59;
			hpBarX = 52;
			hpBarY = 10;
			spBarW = 61;
			expBarW = gW - 61;
		}
	public bool isPaintUI()
		{
			if (isPaintStore || isPaintWeapon || isPaintNonNam || isPaintNonNu || isPaintAoNam || isPaintAoNu || isPaintGangTayNam || isPaintGangTayNu || isPaintQuanNam || isPaintQuanNu || isPaintGiayNam || isPaintGiayNu || isPaintLien || isPaintNhan || isPaintNgocBoi || isPaintPhu || isPaintStack || isPaintStackLock || isPaintGrocery || isPaintGroceryLock || isPaintUpGrade || isPaintConvert || isPaintSplit || isPaintUpPearl || isPaintBox || isPaintTrade)
			{
				return true;
			}
			return false;
		}
	public bool isOpenUI()
		{
			if (ModMenu.uiCustomOpen)
			{
				return true;
			}
			if (isPaintItemInfo || isPaintInfoMe || isPaintStore || isPaintNonNam || isPaintNonNu || isPaintAoNam || isPaintAoNu || isPaintGangTayNam || isPaintGangTayNu || isPaintQuanNam || isPaintQuanNu || isPaintGiayNam || isPaintGiayNu || isPaintLien || isPaintNhan || isPaintNgocBoi || isPaintPhu || isPaintWeapon || isPaintStack || isPaintStackLock || isPaintGrocery || isPaintGroceryLock || isPaintUpGrade || isPaintConvert || isPaintUpPearl || isPaintBox || isPaintSplit || isPaintTrade)
			{
				return true;
			}
			return false;
		}
	public static void loadImg()
		{
			TileMap.loadTileImage();
		}
	public static int getTaskMapId()
		{
			int num = 0;
			if (Char.myCharz().taskMaint == null)
			{
				return -1;
			}
			return mapTasks[Char.myCharz().taskMaint.index];
		}
	public static sbyte getTaskNpcId()
		{
			sbyte result = 0;
			if (Char.myCharz().taskMaint == null)
			{
				result = -1;
			}
			else if (Char.myCharz().taskMaint.index <= tasks.Length - 1)
			{
				result = (sbyte)tasks[Char.myCharz().taskMaint.index];
			}
			return result;
		}
	public void refreshTeam()
		{
		}
	public void openWeb(string strLeft, string strRight, string url, string title, string str)
		{
			isPaintAlert = true;
			isLockKey = true;
			indexRow = 0;
			setPopupSize(175, 200);
			textsTitle = title;
			texts = mFont.tahoma_7.splitFontVector(str, popupW - 30);
			center = null;
			left = new Command(strLeft, 11068, url);
			right = new Command(strRight, 11069);
		}
	public void sendSms(string strLeft, string strRight, short port, string syntax, string title, string str)
		{
			isPaintAlert = true;
			isLockKey = true;
			indexRow = 0;
			setPopupSize(175, 200);
			textsTitle = title;
			texts = mFont.tahoma_7.splitFontVector(str, popupW - 30);
			center = null;
			MyVector myVector = new MyVector();
			myVector.addElement(string.Empty + port);
			myVector.addElement(syntax);
			left = new Command(strLeft, 11074);
			right = new Command(strRight, 11075);
		}
	public void openUIZone(Message message)
		{
			InfoDlg.hide();
			try
			{
				zones = new int[message.reader().readByte()];
				pts = new int[zones.Length];
				numPlayer = new int[zones.Length];
				maxPlayer = new int[zones.Length];
				rank1 = new int[zones.Length];
				rankName1 = new string[zones.Length];
				rank2 = new int[zones.Length];
				rankName2 = new string[zones.Length];
				for (int i = 0; i < zones.Length; i++)
				{
					zones[i] = message.reader().readByte();
					pts[i] = message.reader().readByte();
					numPlayer[i] = message.reader().readByte();
					maxPlayer[i] = message.reader().readByte();
					sbyte b = message.reader().readByte();
					if (b == 1)
					{
						rankName1[i] = message.reader().readUTF();
						rank1[i] = message.reader().readInt();
						rankName2[i] = message.reader().readUTF();
						rank2[i] = message.reader().readInt();
					}
				}
			}
			catch (Exception ex)
			{
				Cout.LogError("Loi ham OPEN UIZONE " + ex.ToString());
			}
			GameCanvas.panel.setTypeZone();
			GameCanvas.panel.show();
		}
	public void showViewInfo()
		{
			indexMenu = 3;
			isPaintInfoMe = true;
			setPopupSize(175, 200);
		}
	private void actDead()
		{
			MyVector myVector = new MyVector();
			myVector.addElement(new Command(mResources.DIES[1], 110381));
			myVector.addElement(new Command(mResources.DIES[2], 110382));
			myVector.addElement(new Command(mResources.DIES[3], 110383));
			GameCanvas.menu.startAt(myVector, 3);
		}
	public void getFlagImage(int charID, sbyte cflag)
		{
			if (vFlag.size() == 0)
			{
				Service.gI().getFlag(2, cflag);
				Res.outz("getFlag1");
				return;
			}
			if (charID == Char.myCharz().charID)
			{
				Res.outz("my cflag: isme");
				if (Char.myCharz().isGetFlagImage(cflag))
				{
					Res.outz("my cflag: true");
					for (int i = 0; i < vFlag.size(); i++)
					{
						PKFlag pKFlag = (PKFlag)vFlag.elementAt(i);
						if (pKFlag != null && pKFlag.cflag == cflag)
						{
							Res.outz("my cflag: cflag==");
							Char.myCharz().flagImage = pKFlag.IDimageFlag;
						}
					}
				}
				else if (!Char.myCharz().isGetFlagImage(cflag))
				{
					Res.outz("my cflag: false");
					Service.gI().getFlag(2, cflag);
				}
				return;
			}
			Res.outz("my cflag: not me");
			if (findCharInMap(charID) == null)
			{
				return;
			}
			if (findCharInMap(charID).isGetFlagImage(cflag))
			{
				Res.outz("my cflag: true");
				for (int j = 0; j < vFlag.size(); j++)
				{
					PKFlag pKFlag2 = (PKFlag)vFlag.elementAt(j);
					if (pKFlag2 != null && pKFlag2.cflag == cflag)
					{
						Res.outz("my cflag: cflag==");
						findCharInMap(charID).flagImage = pKFlag2.IDimageFlag;
					}
				}
			}
			else if (!findCharInMap(charID).isGetFlagImage(cflag))
			{
				Res.outz("my cflag: false");
				Service.gI().getFlag(2, cflag);
			}
		}
	public void showWinNumber(string num, string finish)
		{
			winnumber = new int[num.Length];
			randomNumber = new int[num.Length];
			tMove = new int[num.Length];
			moveCount = new int[num.Length];
			delayMove = new int[num.Length];
			try
			{
				for (int i = 0; i < num.Length; i++)
				{
					winnumber[i] = short.Parse(num[i].ToString());
					randomNumber[i] = Res.random(0, 11);
					tMove[i] = 1;
					delayMove[i] = 0;
				}
			}
			catch (Exception)
			{
			}
			tShow = 100;
			moveIndex = 0;
			strFinish = finish;
			lastXS = (currXS = mSystem.currentTimeMillis());
		}
	public void showYourNumber(string strNum)
		{
			yourNumber = strNum;
			strPaint = mFont.tahoma_7.splitFontArray(yourNumber, 500);
		}
	public static void checkRemoveImage()
		{
			ImgByName.checkDelHash(ImgByName.hashImagePath, 10, isTrue: false);
		}
	public static bool ispaintPhubangBar()
		{
			if (TileMap.mapPhuBang() && phuban_Info.type_PB == 0)
			{
				return true;
			}
			return false;
		}
	public static bool setIsInScreen(int x, int y, int wOne, int hOne)
		{
			if (x < cmx - wOne || x > cmx + GameCanvas.w + wOne || y < cmy - hOne || y > cmy + GameCanvas.h + hOne * 3 / 2)
			{
				return false;
			}
			return true;
		}
	public static bool isSmallScr()
		{
			if (GameCanvas.w <= 320)
			{
				return true;
			}
			return false;
		}

}
