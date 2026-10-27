using System;

public class AutoTrainMenu : mScreen, IActionListener
{
	public static bool isNhat = false;

	public static AutoTrainMenu instance;

	public static Paint paintUI = new Paint();

	public static bool[] isChecked = new bool[3] { false, true, false };

	public TField tfHpOn;

	public TField tfHpDuoi;

	public static long hpQuaiOn = 0L;

	public static long hpQuaiDuoi = 10000000001L;

	public static bool isCheckAttackByHp = false;

	public static bool isCheckAutoPean = false;

	public static int peanHpPercent = 50;

	public static int peanMpPercent = 50;

	public TField tfHpPean;

	public TField tfMpPean;

	public static long lastTimeUsePean = 0L;

	public static MyVector listMobNames = new MyVector();

	public static bool[] isCheckedMobName = new bool[100];

	public static bool isCheckAllMob = true;

	private static int mobNameSnapshotMapId = -1;

	private static bool mobNameSnapshotReady;

	private static bool mobSelectionPreparedForCurrentMap;

	private static bool mobSelectionLoadedFromMapRms;

	private static bool mobSelectionCurrentMapWasReloaded;

	public static MyVector listSkills = new MyVector();

	public static bool[] isCheckedSkill = new bool[20];

	public static int currentTab = 0;

	public static bool[] isCheckItem = new bool[50];

	public static bool[] isCheckPick = new bool[4];

	public TField tfIdNhat;

	public TField tfIdLoaiTru;

	public TField tfIdVutBo;

	public static string strIdNhat = "";

	public static string strIdLoaiTru = "";

	public static string strIdVutBo = "";

	public TField[] tfBuaKills = new TField[4];

	public static int[] muaBuaKills = new int[4];

	public static bool[] isCheckBua = new bool[4];

	public int x;

	public int y;

	public int w;

	public int h;

	public int xScroll;

	public int yScroll;

	public int wScroll;

	public int hScroll;

	public Scroll[] scrolls = new Scroll[3];

	public int selectedIndex = -1;

	public new Command cmdClose;

	public static long lastTimeAttack = 0L;

	public static int[] ITEM_TEMPLATE_IDS = new int[25]
	{
		521, 1628, 381, 1150, 384, 1153, 382, 1152, 383, 1151,
		1517, 1518, 663, 664, 665, 666, 667, 880, 881, 882,
		385, 1154, 211, 212, 2021
	};

	private static readonly long[] lastTimeUseItemByType = new long[ITEM_TEMPLATE_IDS.Length];

	private const long ITEM_USE_COOLDOWN_MILLIS = 60000L;

	public static int[] BUA_TEMPLATE_IDS = new int[4] { 213, 216, 222, 219 };

	public static int[] BUA_ICON_IDS = new int[4] { 2754, 2756, 2758, 2755 };

	private static long lastTimeUseItem;

	private static long lastTimePick;

	private static long lastTimeDrop;

	private static long lastTimeBuyBua;

	private static long lastTimeChangeZone;

	private static long timeMapEmpty;

	public static long autoTrainNextZoneAllowedTime;

	public static int unreachableMobId = -1;

	public static long unreachableMobTime;

	public static int lastTargetMobId = -1;

	public static long targetMobStartTime;

	public static bool needRescanMobs = false;

	public static int lastMapIdBeforeChange = -1;

	public static string[] OPTION_NAMES => new string[3]
	{
		mResources.tuDongDanh,
		mResources.boQuaSieuQuai,
		mResources.doiKhuKhiHetQuai
	};

	public static string[] TAB_NAMES => mResources.autoTrain_TabNames;

	public static string[] TAB_NAMES_2 => mResources.autoTrain_TabNames_2;

	public static string[] PICK_NAMES => mResources.autoPick_Names;

	public static string[] BUA_NAMES => mResources.autoBua_Names;

	public static string[] ITEM_NAMES
	{
		get
		{
			string[] array = new string[ITEM_TEMPLATE_IDS.Length];
			for (int i = 0; i < ITEM_TEMPLATE_IDS.Length; i++)
			{
				array[i] = getItemName(i);
			}
			return array;
		}
	}

	public Scroll scroll
	{
		get
		{
			if (currentTab >= 0 && currentTab < scrolls.Length && scrolls[currentTab] != null)
			{
				return scrolls[currentTab];
			}
			return scrolls[0];
		}
	}

	static AutoTrainMenu()
	{
		isCheckedSkill[0] = true;
		isCheckedSkill[1] = true;
		isCheckPick[0] = true;
		for (int i = 0; i < isCheckedMobName.Length; i++)
		{
			isCheckedMobName[i] = true;
		}
	}

	public static string getItemName(int index)
	{
		if (index < 0 || index >= ITEM_TEMPLATE_IDS.Length)
		{
			return "";
		}
		ItemTemplate itemTemplate = ItemTemplates.get((short)ITEM_TEMPLATE_IDS[index]);
		if (itemTemplate != null && !string.IsNullOrEmpty(itemTemplate.name))
		{
			return itemTemplate.name;
		}
		return "";
	}

	public static AutoTrainMenu gI()
	{
		if (instance == null)
		{
			instance = new AutoTrainMenu();
		}
		return instance;
	}

	public AutoTrainMenu()
	{
		tfBuaKills = new TField[4];
		scrolls = new Scroll[3];
		selectedIndex = -1;
		cmdClose = new Command(mResources.CLOSE, this, 1, null);
		if (!GameCanvas.isTouch)
		{
			right = cmdClose;
		}
		tfHpOn = new TField
		{
			name = mResources.hpTren,
			width = 46,
			height = 20
		};
		tfHpOn.setIputType(TField.INPUT_TYPE_NUMERIC);
		tfHpOn.setMaxTextLenght(12);
		tfHpOn.setText("0");
		tfHpDuoi = new TField
		{
			name = mResources.hpDuoi,
			width = 58,
			height = 20
		};
		tfHpDuoi.setIputType(TField.INPUT_TYPE_NUMERIC);
		tfHpDuoi.setMaxTextLenght(20);
		tfHpDuoi.setText(mResources.khongGioiHan);
		tfHpPean = new TField
		{
			name = mResources.percentHpDungDau,
			width = 22,
			height = 20
		};
		tfHpPean.setIputType(TField.INPUT_TYPE_NUMERIC);
		tfHpPean.setMaxTextLenght(3);
		tfHpPean.setText("50");
		tfMpPean = new TField
		{
			name = mResources.percentMpDungDau,
			width = 22,
			height = 20
		};
		tfMpPean.setIputType(TField.INPUT_TYPE_NUMERIC);
		tfMpPean.setMaxTextLenght(3);
		tfMpPean.setText("50");
		for (int i = 0; i < 4; i++)
		{
			tfBuaKills[i] = new TField();
			tfBuaKills[i].name = ((mResources.autoBua_Names != null && i < mResources.autoBua_Names.Length) ? mResources.autoBua_Names[i] : "");
			tfBuaKills[i].setIputType(TField.INPUT_TYPE_NUMERIC);
			tfBuaKills[i].setMaxTextLenght(5);
			tfBuaKills[i].setText("0");
			tfBuaKills[i].width = 28;
			tfBuaKills[i].height = 20;
		}
		tfIdNhat = new TField
		{
			name = mResources.dsItemIdNhat,
			width = 180,
			height = 24
		};
		tfIdLoaiTru = new TField
		{
			name = mResources.dsItemIdLoaiTru,
			width = 180,
			height = 24
		};
		tfIdVutBo = new TField
		{
			name = mResources.dsItemIdVutBo,
			width = 180,
			height = 24
		};
		for (int j = 0; j < 3; j++)
		{
			scrolls[j] = new Scroll
			{
				ITEM_SIZE = 26,
				ITEM_PER_LINE = 1
			};
		}
	}

	public static void scanSkills()
	{
		if (listSkills == null)
		{
			listSkills = new MyVector();
		}
		listSkills.removeAllElements();
		Skill[] array = ((TField.isQwerty || !GameCanvas.isTouch) ? GameScr.keySkill : GameScr.onScreenSkill);
		if (array != null)
		{
			for (int i = 0; i < array.Length; i++)
			{
				listSkills.addElement(array[i]);
			}
		}
		else if (Char.myCharz() != null && Char.myCharz().vSkill != null)
		{
			for (int j = 0; j < Char.myCharz().vSkill.size(); j++)
			{
				listSkills.addElement(Char.myCharz().vSkill.elementAt(j));
			}
		}
	}

	private static int getBuffIconId(int templateId)
	{
		switch (templateId)
		{
		case 213:
		case 381:
		case 1150:
			return 2754;
		case 216:
		case 383:
		case 1151:
			return 2756;
		case 219:
		case 382:
		case 1152:
			return 2755;
		case 222:
			return 2758;
		case 384:
		case 1153:
			return 2757;
		case 385:
		case 1154:
			return 2760;
		default:
			return -1;
		}
	}

	private static bool isItemUseOnCooldown(int itemIndex, long now)
	{
		return itemIndex > 0 && itemIndex < lastTimeUseItemByType.Length && lastTimeUseItemByType[itemIndex] > 0 && now - lastTimeUseItemByType[itemIndex] < ITEM_USE_COOLDOWN_MILLIS;
	}

	private static void markItemUseSent(int itemIndex, long now)
	{
		if (itemIndex > 0 && itemIndex < lastTimeUseItemByType.Length)
		{
			lastTimeUseItemByType[itemIndex] = now;
		}
	}

	private static int getActiveBuffKey(int index)
	{
		if (index < 0 || index >= ITEM_TEMPLATE_IDS.Length)
		{
			return -1;
		}
		int num = ITEM_TEMPLATE_IDS[index];
		ItemTemplate itemTemplate = ItemTemplates.get((short)num);
		int num2 = ((itemTemplate != null) ? itemTemplate.iconID : num);
		int buffIconId = getBuffIconId(num);
		if (buffIconId != -1 && (ItemTime.isExistItem(buffIconId) || ItemTime.isExistMessage(buffIconId)))
		{
			return buffIconId;
		}
		if (ItemTime.isExistItem(num2) || ItemTime.isExistMessage(num2))
		{
			return (buffIconId != -1) ? buffIconId : num2;
		}
		if (ItemTime.isExistItem(num) || ItemTime.isExistMessage(num))
		{
			return (buffIconId != -1) ? buffIconId : num;
		}
		if (num == 521)
		{
			return GameScr.canAutoPlay ? 521 : (-1);
		}
		return -1;
	}

	private static bool isItemBuffActive(int index)
	{
		return getActiveBuffKey(index) != -1;
	}

	private static void updateAutoItem()
	{
		Char @char = Char.myCharz();
		if (@char == null || @char.statusMe == 14 || @char.arrItemBag == null)
		{
			return;
		}
		long num = mSystem.currentTimeMillis();
		if (num - lastTimeUseItem < 1000L)
		{
			return;
		}
		if ((!isCheckItem[0] && GameScr.canAutoPlay) || (isCheckItem[0] && !GameScr.canAutoPlay))
		{
			for (int i = 0; i < @char.arrItemBag.Length; i++)
			{
				Item item = @char.arrItemBag[i];
				if (item != null && item.template != null && item.template.id == 521)
				{
					Service.gI().useItem(0, 1, (sbyte)item.indexUI, -1);
					lastTimeUseItem = num;
					return;
				}
			}
		}
		if (isCheckItem.Length > 1 && isCheckItem[1] && !isItemBuffActive(1) && !isItemUseOnCooldown(1, num))
		{
			int num2 = ITEM_TEMPLATE_IDS[1];
			for (int j = 0; j < @char.arrItemBag.Length; j++)
			{
				Item item2 = @char.arrItemBag[j];
				if (item2 != null && item2.template != null && item2.template.id == num2)
				{
					Service.gI().useItem(0, 1, (sbyte)item2.indexUI, -1);
					long num3 = mSystem.currentTimeMillis();
					lastTimeUseItem = num3;
					markItemUseSent(1, num3);
					Res.outz("[AutoItem] >> DÙNG BÙA X2 ĐỆ TỬ: " + item2.template.name + " (ID: " + num2 + ")");
					return;
				}
			}
		}
		if (isCheckItem.Length > 24 && isCheckItem[24] && !isItemUseOnCooldown(24, num) && TileMap.mapID != 155 && TileMap.mapID != 166 && !Char.isLoadingMap && !GameCanvas.isLoading)
		{
			int num4 = ITEM_TEMPLATE_IDS[24];
			for (int k = 0; k < @char.arrItemBag.Length; k++)
			{
				Item item3 = @char.arrItemBag[k];
				if (item3 != null && item3.template != null && item3.template.id == num4)
				{
					Service.gI().useItem(0, 1, (sbyte)item3.indexUI, -1);
					long num5 = mSystem.currentTimeMillis();
					lastTimeUseItem = num5;
					markItemUseSent(24, num5);
					return;
				}
			}
		}
		int num6 = System.Math.Min(24, ITEM_TEMPLATE_IDS.Length);
		for (int l = 2; l < num6; l++)
		{
			if (!isCheckItem[l] || isItemUseOnCooldown(l, num) || isItemBuffActive(l))
			{
				continue;
			}
			int num7 = ITEM_TEMPLATE_IDS[l];
			for (int m = 0; m < @char.arrItemBag.Length; m++)
			{
				Item item4 = @char.arrItemBag[m];
				if (item4 != null && item4.template != null && item4.template.id == num7)
				{
					Service.gI().useItem(0, 1, (sbyte)item4.indexUI, -1);
					long num8 = mSystem.currentTimeMillis();
					lastTimeUseItem = num8;
					markItemUseSent(l, num8);
					Res.outz("[AutoItem] >> Tiến hành dùng: " + item4.template.name + " (ID: " + num7 + ", Ô túi: " + item4.indexUI + ")");
					return;
				}
			}
		}
	}

	private static bool isIdInList(int id, string idListStr)
	{
		if (string.IsNullOrEmpty(idListStr) || idListStr.Trim().Length == 0)
		{
			return false;
		}
		string[] array = idListStr.Split(new char[3] { ',', ' ', ';' }, StringSplitOptions.RemoveEmptyEntries);
		for (int i = 0; i < array.Length; i++)
		{
			if (int.TryParse(array[i].Trim(), out var result) && result == id)
			{
				return true;
			}
		}
		return false;
	}

	private static void updateAutoPick()
	{
		Char @char = Char.myCharz();
		if (@char == null || @char.statusMe == 14)
		{
			return;
		}
		long num = mSystem.currentTimeMillis();
		if (isCheckPick[3] && !string.IsNullOrEmpty(strIdVutBo) && num - lastTimeDrop >= 1000L && @char.arrItemBag != null)
		{
			for (int i = 0; i < @char.arrItemBag.Length; i++)
			{
				Item item = @char.arrItemBag[i];
				if (item != null && item.template != null && isIdInList(item.template.id, strIdVutBo))
				{
					Service.gI().useItem(1, 1, (sbyte)item.indexUI, -1);
					lastTimeDrop = num;
					break;
				}
			}
		}
		if (!isCheckPick[0] || num - lastTimePick < 350L || GameScr.vItemMap == null || GameScr.vItemMap.size() <= 0)
		{
			return;
		}
		bool flag = isCheckPick[1];
		bool flag2 = isCheckPick[2];
		ItemMap itemMap = null;
		int num2 = 999999;
		for (int j = 0; j < GameScr.vItemMap.size(); j++)
		{
			ItemMap itemMap2 = (ItemMap)GameScr.vItemMap.elementAt(j);
			if (itemMap2 == null || itemMap2.template == null)
			{
				continue;
			}
			if (!flag && itemMap2.playerId != @char.charID && itemMap2.playerId != -1)
			{
				continue;
			}
			if (!string.IsNullOrEmpty(strIdLoaiTru) && isIdInList(itemMap2.template.id, strIdLoaiTru))
			{
				continue;
			}
			if (!string.IsNullOrEmpty(strIdNhat) && !isIdInList(itemMap2.template.id, strIdNhat))
			{
				continue;
			}
			int num3 = Res.distance(@char.cx, @char.cy, itemMap2.x, itemMap2.y);
			if ((flag2 || num3 <= 200) && num3 < num2)
			{
				num2 = num3;
				itemMap = itemMap2;
			}
		}
		if (itemMap == null)
		{
			return;
		}
		if (num2 > 30)
		{
			if (flag2 || GameScr.canAutoPlay)
			{
				@char.cx = itemMap.x;
				@char.cy = itemMap.y;
				Service.gI().charMove();
			}
			else if (@char.currentMovePoint == null || Res.distance(@char.currentMovePoint.xEnd, @char.currentMovePoint.yEnd, itemMap.x, itemMap.y) > 20)
			{
				@char.currentMovePoint = new MovePoint(itemMap.x, itemMap.y);
			}
		}
		Service.gI().pickItem(itemMap.itemMapID);
		lastTimePick = num;
	}

	private static void updateAutoBua()
	{
		long num = mSystem.currentTimeMillis();
		if (num - lastTimeBuyBua < 3000L)
		{
			return;
		}
		for (int i = 0; i < 4; i++)
		{
			if (isCheckBua[i] && !ItemTime.isExistItem(BUA_ICON_IDS[i]) && !ItemTime.isExistItem(BUA_TEMPLATE_IDS[i]))
			{
				int quantity = ((muaBuaKills[i] > 0) ? muaBuaKills[i] : 1);
				Service.gI().buyItem(0, BUA_TEMPLATE_IDS[i], quantity);
				lastTimeBuyBua = num;
				return;
			}
		}
	}

	private static void updateAutoPean()
	{
		if (!isCheckAutoPean)
		{
			return;
		}
		Char @char = Char.myCharz();
		if (@char == null || @char.statusMe == 14 || GameScr.gI().mobCapcha != null)
		{
			return;
		}
		long num = mSystem.currentTimeMillis();
		if (num - lastTimeUsePean < 5000L || @char.cHPFull <= 0 || @char.cMPFull <= 0)
		{
			return;
		}
		long num2 = @char.cHP * 100L / @char.cHPFull;
		long num3 = @char.cMP * 100L / @char.cMPFull;
		if ((num2 < peanHpPercent || num3 < peanMpPercent) && @char.doUsePotion())
		{
			lastTimeUsePean = num;
			ServerEffect.addServerEffect(11, @char, 5);
			ServerEffect.addServerEffect(104, @char, 4);
			SoundMn.gI().eatPeans();
		}
	}

	public static void updateAutoTrain()
	{
		if (ModTanSat.autoTanSat || ModNextMap.isNextMapActive || ModMenu.modMenuOpen || ModMenu.uiCustomOpen)
		{
			return;
		}
		Char @char = Char.myCharz();
		if (@char == null || @char.statusMe == 14 || @char.statusMe == 5 || @char.cHP <= 0 || @char.meDead)
		{
			if (isChecked[0])
			{
				setAutoAttack(enable: false, "Nhân vật đã tử vong");
			}
			return;
		}
		if (GameScr.gI().mobCapcha != null)
		{
			return;
		}
		updateAutoPean();
		if (isChecked[0] && GameCanvas.currentScreen == GameScr.instance && (ChatTextField.gI() == null || !ChatTextField.gI().isShow))
		{
			int[] array = new int[12] { 1, 2, 3, 4, 6, 7, 8, 9, 21, 22, 23, 24 };
			bool flag = false;
			for (int i = 0; i < array.Length; i++)
			{
				int num = array[i];
				if ((num < GameCanvas.keyHold.Length && GameCanvas.keyHold[num]) || (num < GameCanvas.keyPressed.Length && GameCanvas.keyPressed[num]))
				{
					flag = true;
					break;
				}
			}
			if (!flag && GameScr.isAnalog != 0 && GameScr.gamePad != null && GameScr.gamePad.disableCheckDrag())
			{
				flag = true;
			}
			if (flag)
			{
				setAutoAttack(enable: false, "Bấm phím di chuyển hoặc Joystick");
				@char.currentMovePoint = null;
				return;
			}
		}
		if (!isChecked[0])
		{
			return;
		}
		updateAutoPick();
		updateAutoItem();
		updateAutoBua();
		if (needRescanMobs)
		{
			needRescanMobs = false;
			scanMobsInMap();
			unreachableMobId = -1;
			unreachableMobTime = 0L;
			lastTargetMobId = -1;
			targetMobStartTime = 0L;
		}
		long num2 = mSystem.currentTimeMillis();
		Mob mob = @char.mobFocus;
		if (!isMobValid(mob))
		{
			if (mob != null && mob.mobId == lastTargetMobId)
			{
				lastTargetMobId = -1;
				targetMobStartTime = 0L;
			}
			@char.mobFocus = null;
			mob = findMobToAttack();
			if (mob == null && !isCheckAllMob && listMobNames != null && listMobNames.size() > 0)
			{
				scanMobsInMap();
				mob = findMobToAttack();
			}
		}
		if (mob == null)
		{
			lastTargetMobId = -1;
			targetMobStartTime = 0L;
			if (isChecked[2] && !Char.isLoadingMap && !GameCanvas.isLoading)
			{
				if (timeMapEmpty == 0L)
				{
					timeMapEmpty = num2;
				}
				else if (num2 - timeMapEmpty >= 3000L && num2 - lastTimeChangeZone >= 3000L && num2 > autoTrainNextZoneAllowedTime)
				{
					lastTimeChangeZone = num2;
					timeMapEmpty = 0L;
					lastMapIdBeforeChange = TileMap.mapID;
					int num3 = (TileMap.zoneID + 1) % 15;
					Res.outz("[AutoTrain] >> Map hết quái liên tục sau 3s tại khu " + TileMap.zoneID + " -> Tự động chuyển sang khu " + num3);
					Service.gI().requestChangeZone(num3, -1);
				}
			}
			else
			{
				timeMapEmpty = 0L;
			}
			return;
		}
		timeMapEmpty = 0L;
		@char.mobFocus = mob;
		if (mob.mobId != lastTargetMobId)
		{
			lastTargetMobId = mob.mobId;
			targetMobStartTime = num2;
		}
		Skill nextSkillToUse = getNextSkillToUse(num2);
		int num4 = System.Math.Max(40, (nextSkillToUse != null) ? nextSkillToUse.dx : 40);
		int num5 = System.Math.Max(30, (nextSkillToUse != null) ? nextSkillToUse.dy : 30);
		MobTemplate mobTemplate = ((Mob.arrMobTemplate != null && mob.templateId >= 0 && mob.templateId < Mob.arrMobTemplate.Length) ? Mob.arrMobTemplate[mob.templateId] : null);
		bool flag2 = mobTemplate != null && (mobTemplate.type == 4 || mobTemplate.type == 5);
		int num6 = Res.abs(@char.cx - mob.x);
		int num7 = Res.abs(@char.cy - mob.y);
		bool flag3 = num6 <= num4 && (flag2 || num7 <= num5);
		if (!flag3)
		{
			if (targetMobStartTime > 0 && num2 - targetMobStartTime > 4000L)
			{
				unreachableMobId = mob.mobId;
				unreachableMobTime = num2;
				lastTargetMobId = -1;
				targetMobStartTime = 0L;
				@char.mobFocus = null;
				@char.currentMovePoint = null;
				return;
			}
			if (GameScr.canAutoPlay)
			{
				@char.cx = mob.x;
				@char.cy = mob.y;
				Service.gI().charMove();
			}
			else
			{
				int yEnd = (flag2 ? @char.cy : mob.y);
				if (@char.currentMovePoint == null || Res.distance(@char.currentMovePoint.xEnd, @char.currentMovePoint.yEnd, mob.x, yEnd) > 20)
				{
					@char.currentMovePoint = new MovePoint(mob.x, yEnd);
				}
				@char.cdir = ((mob.x >= @char.cx) ? 1 : (-1));
				return;
			}
		}
		else if (!GameScr.canAutoPlay && @char.endMovePointCommand == null)
		{
			@char.currentMovePoint = null;
		}
		@char.cdir = ((mob.x >= @char.cx) ? 1 : (-1));
		if (!isCharacterBusyAttacking(@char) && @char.endMovePointCommand == null && nextSkillToUse != null && num2 - lastTimeAttack >= 500L)
		{
			lastTimeAttack = num2;
			GameScr.gI().doUseSkillForAutoTrain(nextSkillToUse);
			if (@char.skillPaint != null || @char.dart != null || (@char.myskill != null && @char.myskill.template != null && @char.myskill.template.isUseAlone()))
			{
				nextSkillToUse.lastTimeUseThisSkill = num2;
			}
		}
	}

	private static bool isCharacterBusyAttacking(Char myChar)
	{
		if (myChar == null)
		{
			return false;
		}
		return myChar.skillPaint != null || myChar.dart != null || myChar.isFreez || myChar.isWaitMonkey || myChar.isCharge || myChar.isFlyAndCharge || myChar.isStandAndCharge || myChar.statusMe == 4 || myChar.statusMe == 10 || myChar.holdEffID != 0 || myChar.blindEff || myChar.sleepEff;
	}

	private static Skill getNextSkillToUse(long now)
	{
		if (listSkills == null || listSkills.size() == 0)
		{
			scanSkills();
		}
		Char @char = Char.myCharz();
		if (@char == null || listSkills == null || listSkills.size() == 0)
		{
			return null;
		}
		Skill skill = null;
		for (int i = 1; i < listSkills.size(); i++)
		{
			if (i < isCheckedSkill.Length && isCheckedSkill[i])
			{
				Skill skill2 = listSkills.elementAt(i) as Skill;
				if (skill2 != null && isSkillReady(skill2, @char, now) && (skill == null || skill2.coolDown > skill.coolDown))
				{
					skill = skill2;
				}
			}
		}
		if (skill != null)
		{
			return skill;
		}
		if (isCheckedSkill.Length > 0 && isCheckedSkill[0] && listSkills.size() > 0)
		{
			Skill skill3 = listSkills.elementAt(0) as Skill;
			if (skill3 != null && isSkillReady(skill3, @char, now))
			{
				return skill3;
			}
		}
		return null;
	}

	private static bool isSkillReady(Skill skill, Char myChar, long now)
	{
		if (skill == null || skill.template == null || myChar == null)
		{
			return false;
		}
		int num = ((skill.coolDown > 500) ? skill.coolDown : 500);
		if (now - skill.lastTimeUseThisSkill < num)
		{
			return false;
		}
		long num2 = ((skill.template.manaUseType == 2) ? 1L : ((skill.template.manaUseType == 1) ? ((long)skill.manaUse * myChar.cMPFull / 100L) : skill.manaUse));
		if (myChar.cMP >= num2)
		{
			return true;
		}
		if (GameCanvas.gameTick % 60 == 0)
		{
			Res.outz("[AutoTrain] Skill [" + skill.template.name + "] KHÔNG ĐỦ KI (Cần: " + num2 + ", Hiện có: " + myChar.cMP + ")");
		}
		return false;
	}

	private static bool isMobValid(Mob mob)
	{
		if (mob == null || mob.isMobMe || mob.hp <= 0 || mob.isDie || mob.status == 1 || mob.status == 0 || (mob.x == 0 && mob.y == 0))
		{
			return false;
		}
		if (isChecked[1] && (mob.checkIsBoss() || mob.levelBoss != 0 || mob.isBoss))
		{
			return false;
		}
		long num = mSystem.currentTimeMillis();
		if (mob.mobId == unreachableMobId && num - unreachableMobTime < 3000L)
		{
			return false;
		}
		if (isCheckAttackByHp)
		{
			if (hpQuaiOn > 0 && mob.hp < hpQuaiOn)
			{
				return false;
			}
			if (hpQuaiDuoi > 0 && mob.hp > hpQuaiDuoi)
			{
				return false;
			}
		}
		ensureMobSelectionHasAnySelected();
		if (isCheckAllMob)
		{
			return true;
		}
		string text = mob.mobName;
		if (string.IsNullOrEmpty(text) && Mob.arrMobTemplate != null && mob.templateId >= 0 && mob.templateId < Mob.arrMobTemplate.Length && Mob.arrMobTemplate[mob.templateId] != null)
		{
			text = Mob.arrMobTemplate[mob.templateId].name;
		}
		for (int i = 0; i < listMobNames.size(); i++)
		{
			if (i < isCheckedMobName.Length && isCheckedMobName[i] && text != null && text.Equals(listMobNames.elementAt(i) as string))
			{
				return true;
			}
		}
		return listMobNames.size() <= 0;
	}

	private static Mob findMobToAttack()
	{
		Char @char = Char.myCharz();
		if (@char == null || GameScr.vMob == null)
		{
			return null;
		}
		Mob result = null;
		int num = 999999;
		for (int i = 0; i < GameScr.vMob.size(); i++)
		{
			Mob mob = GameScr.vMob.elementAt(i) as Mob;
			if (isMobValid(mob))
			{
				int num2 = Res.distance(@char.cx, @char.cy, mob.x, mob.y);
				if (num2 < num)
				{
					num = num2;
					result = mob;
				}
			}
		}
		return result;
	}

	public static void resetMobNameSnapshot()
	{
		bool flag = mobNameSnapshotReady && mobNameSnapshotMapId >= 0;
		mobNameSnapshotMapId = -1;
		mobNameSnapshotReady = false;
		mobSelectionPreparedForCurrentMap = false;
		mobSelectionLoadedFromMapRms = false;
		mobSelectionCurrentMapWasReloaded = mobSelectionCurrentMapWasReloaded || flag;
		if (listMobNames != null)
		{
			listMobNames.removeAllElements();
		}
	}

	public static void captureMobNameSnapshot()
	{
		if (GameScr.vMob == null)
		{
			return;
		}
		if (listMobNames == null)
		{
			listMobNames = new MyVector();
		}
		listMobNames.removeAllElements();
		for (int i = 0; i < GameScr.vMob.size(); i++)
		{
			Mob mob = GameScr.vMob.elementAt(i) as Mob;
			if (mob == null || mob.isMobMe || mob.templateId < 0)
			{
				continue;
			}
			string text = mob.mobName;
			if (string.IsNullOrEmpty(text) && Mob.arrMobTemplate != null && mob.templateId < Mob.arrMobTemplate.Length && Mob.arrMobTemplate[mob.templateId] != null)
			{
				text = Mob.arrMobTemplate[mob.templateId].name;
			}
			if (string.IsNullOrEmpty(text))
			{
				text = mResources.quai + mob.mobId;
			}
			if (!listMobNames.contains(text))
			{
				listMobNames.addElement(text);
			}
		}
		mobNameSnapshotMapId = TileMap.mapID;
		mobNameSnapshotReady = true;
		prepareMobSelectionForCurrentMap();
	}

	public static void resetMobSelectionToAll()
	{
		isCheckAllMob = true;
		if (isCheckedMobName == null)
		{
			return;
		}
		for (int i = 0; i < isCheckedMobName.Length; i++)
		{
			isCheckedMobName[i] = false;
		}
		int num = ((listMobNames != null) ? System.Math.Min(listMobNames.size(), isCheckedMobName.Length) : 0);
		for (int j = 0; j < num; j++)
		{
			isCheckedMobName[j] = true;
		}
	}

	public static void ensureMobSelectionHasAnySelected()
	{
		if (isCheckAllMob || listMobNames == null || isCheckedMobName == null)
		{
			return;
		}
		int num = System.Math.Min(listMobNames.size(), isCheckedMobName.Length);
		if (num <= 0)
		{
			resetMobSelectionToAll();
			return;
		}
		for (int i = 0; i < num; i++)
		{
			if (isCheckedMobName[i])
			{
				return;
			}
		}
		resetMobSelectionToAll();
	}

	private static string getMobSelectionRmsKey()
	{
		Char @char = Char.myCharz();
		if (@char == null || TileMap.mapID < 0)
		{
			return null;
		}
		return "AT_Mob_v1_" + @char.charID + "_" + TileMap.mapID;
	}

	public static void saveMobSelectionForCurrentMap()
	{
		string mobSelectionRmsKey = getMobSelectionRmsKey();
		if (string.IsNullOrEmpty(mobSelectionRmsKey))
		{
			return;
		}
		try
		{
			myWriter myWriter2 = new myWriter();
			myWriter2.writeBoolean(isCheckAllMob);
			int num = ((listMobNames != null && isCheckedMobName != null) ? System.Math.Min(listMobNames.size(), isCheckedMobName.Length) : 0);
			myWriter2.writeSByte((sbyte)num);
			for (int i = 0; i < num; i++)
			{
				string str = (listMobNames.elementAt(i) as string) ?? "";
				myWriter2.writeUTF(str);
				myWriter2.writeBoolean(isCheckedMobName[i]);
			}
			Rms.saveRMS(mobSelectionRmsKey, myWriter2.getData());
		}
		catch
		{
		}
	}

	public static bool loadMobSelectionForCurrentMap()
	{
		string mobSelectionRmsKey = getMobSelectionRmsKey();
		if (string.IsNullOrEmpty(mobSelectionRmsKey))
		{
			return false;
		}
		sbyte[] array = Rms.loadRMS(mobSelectionRmsKey);
		if (array == null)
		{
			return false;
		}
		try
		{
			myReader myReader2 = new myReader(array);
			bool flag = myReader2.readBool();
			int num = myReader2.readByte();
			if (num < 0)
			{
				num = 0;
			}
			for (int i = 0; i < isCheckedMobName.Length; i++)
			{
				isCheckedMobName[i] = false;
			}
			for (int j = 0; j < num; j++)
			{
				string text = myReader2.readUTF();
				bool flag2 = myReader2.readBool();
				if (!flag2 || string.IsNullOrEmpty(text) || listMobNames == null)
				{
					continue;
				}
				for (int k = 0; k < listMobNames.size(); k++)
				{
					if (k < isCheckedMobName.Length && text.Equals(listMobNames.elementAt(k) as string))
					{
						isCheckedMobName[k] = true;
					}
				}
			}
			isCheckAllMob = flag;
			if (flag)
			{
				int num2 = ((listMobNames != null) ? System.Math.Min(listMobNames.size(), isCheckedMobName.Length) : 0);
				for (int l = 0; l < num2; l++)
				{
					isCheckedMobName[l] = true;
				}
			}
			else
			{
				ensureMobSelectionHasAnySelected();
			}
			return true;
		}
		catch
		{
			return false;
		}
	}

	private static void prepareMobSelectionForCurrentMap()
	{
		bool flag = loadMobSelectionForCurrentMap();
		if (!flag)
		{
			resetMobSelectionToAll();
		}
		mobSelectionPreparedForCurrentMap = true;
		mobSelectionLoadedFromMapRms = flag;
	}

	public static void scanMobsInMap()
	{
		if (mobNameSnapshotReady && mobNameSnapshotMapId == TileMap.mapID)
		{
			return;
		}
		if (GameCanvas.isLoading || Char.isLoadingMap)
		{
			return;
		}
		captureMobNameSnapshot();
	}

	public void clearAllFocus()
	{
		TField[] array = new TField[11]
		{
			tfHpOn,
			tfHpDuoi,
			tfHpPean,
			tfMpPean,
			(tfBuaKills != null && tfBuaKills.Length > 0) ? tfBuaKills[0] : null,
			(tfBuaKills != null && tfBuaKills.Length > 1) ? tfBuaKills[1] : null,
			(tfBuaKills != null && tfBuaKills.Length > 2) ? tfBuaKills[2] : null,
			(tfBuaKills != null && tfBuaKills.Length > 3) ? tfBuaKills[3] : null,
			tfIdNhat,
			tfIdLoaiTru,
			tfIdVutBo
		};
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != null)
			{
				array[i].isFocus = false;
				array[i].x = -9999;
				array[i].y = -9999;
			}
		}
	}

	public void show()
	{
		selectedIndex = -1;
		clearAllFocus();
		scanMobsInMap();
		scanSkills();
		loadAutoTrainConfig();
		w = System.Math.Min(340, GameCanvas.w - 10);
		h = System.Math.Min(240, GameCanvas.h - 10);
		x = (GameCanvas.w - w) / 2;
		y = (GameCanvas.h - h) / 2;
		xScroll = x + 8;
		yScroll = y + 49;
		wScroll = w - 16;
		hScroll = h - 57;
		if (!isNhat && currentTab >= 2)
		{
			currentTab = 0;
		}
		for (int i = 0; i < 3; i++)
		{
			if (scrolls[i] == null)
			{
				scrolls[i] = new Scroll();
			}
			scrolls[i].xPos = xScroll;
			scrolls[i].yPos = yScroll;
			scrolls[i].width = wScroll;
			scrolls[i].height = hScroll;
			scrolls[i].cmy = 0;
			scrolls[i].cmtoY = 0;
			scrolls[i].cmvy = 0;
			scrolls[i].selectedItem = -1;
			scrolls[i].ITEM_SIZE = 26;
			scrolls[i].ITEM_PER_LINE = 1;
		}
		updateScrollHeight();
		resetScrollTouchState();
		clearAllFocus();
		GameCanvas.currentScreen = this;
	}

	public void updateScrollHeight()
	{
		int num = ((listMobNames != null && listMobNames.size() > 0) ? ((listMobNames.size() + 1) / 2) : 0);
		if (scrolls[0] != null)
		{
			scrolls[0].cmyLim = System.Math.Max(0, num * 26 + 272 - hScroll);
		}
		if (scrolls[1] != null)
		{
			scrolls[1].cmyLim = System.Math.Max(0, (ITEM_TEMPLATE_IDS.Length + 1) / 2 * 26 + 6 - hScroll);
		}
		if (scrolls[2] != null && PICK_NAMES != null)
		{
			scrolls[2].cmyLim = System.Math.Max(0, PICK_NAMES.Length * 26 + 80 - hScroll);
		}
	}

	public override void paint(mGraphics g)
	{
		GameScr.gI().paint(g);
		if (GameCanvas.paintz != null)
		{
			GameCanvas.paintz.paintFrameSimple(x, y, w, h, g);
		}
		if (Panel.imgX != null)
		{
			g.drawImage(Panel.imgX, x + w - 15, y + 15, mGraphics.VCENTER | mGraphics.HCENTER);
		}
		g.setColor(0xCF6C32);
		g.fillRoundRect(x + 8, y + 6, 45, 17, 4, 4);
		mFont.tahoma_7_white.drawString(g, "Reset", x + 30, y + 8, mFont.CENTER);
		GameCanvas.resetTrans(g);
		mFont.tahoma_7b_dark.drawString(g, mResources.autoTrain, x + w / 2, y + 6, mFont.CENTER);
		int num = (isNhat ? 3 : 2);
		string[] array = (isNhat ? TAB_NAMES : TAB_NAMES_2);
		int num2 = (w - 70) / num;
		int num3 = x + 35;
		for (int i = 0; i < num; i++)
		{
			int num4 = num3 + i * num2;
			PopUp.paintPopUp(g, num4, y + 26, num2 - 2, 20, (currentTab == i) ? 1 : 0, isButton: true);
			string st = ((array != null && i < array.Length) ? array[i] : "");
			((currentTab == i) ? mFont.tahoma_7_green2 : mFont.tahoma_7_grey).drawString(g, st, num4 + (num2 - 2) / 2, y + 29, mFont.CENTER);
		}
		g.setClip(xScroll, yScroll, wScroll, hScroll);
		g.translate(0, -scroll.cmy);
		int curY = yScroll + 5;
		if (currentTab == 0)
		{
			paintTabTrain(g, ref curY);
		}
		else if (currentTab == 1)
		{
			paintTabItem(g, ref curY);
		}
		else if (currentTab == 2 && isNhat)
		{
			paintTabPick(g, ref curY);
		}
		g.translate(0, scroll.cmy);
		GameCanvas.resetTrans(g);
		g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
		if (Mob.imgHP != null && scroll != null)
		{
			if (scroll.cmy > 0)
			{
				g.drawRegion(Mob.imgHP, 0, 0, 9, 6, 1, xScroll + wScroll - 12, yScroll + 3, 0);
			}
			if (scroll.cmy < scroll.cmyLim)
			{
				g.drawRegion(Mob.imgHP, 0, 0, 9, 6, 0, xScroll + wScroll - 12, yScroll + hScroll - 9, 0);
			}
		}
		base.paint(g);
	}

	private void paintTabTrain(mGraphics g, ref int curY)
	{
		int num = xScroll + wScroll / 2;
		paintCheckboxItem(g, 0, xScroll + 5, curY, isChecked[0], selectedIndex == 0);
		paintCheckboxItem(g, 1, num + 5, curY, isChecked[1], selectedIndex == 1);
		curY += 26;
		paintCheckboxItem(g, 2, xScroll + 5, curY, isChecked[2], selectedIndex == 2);
		paintCheckboxItemCustom(g, mResources.danhTheoHp, num + 5, curY, isCheckAttackByHp, focus: false);
		curY += 26;
		string name = ((PICK_NAMES != null && PICK_NAMES.Length > 0) ? PICK_NAMES[0] : "");
		paintCheckboxItemCustom(g, name, xScroll + 5, curY, isCheckPick[0], focus: false);
		if (isCheckAttackByHp)
		{
			tfHpOn.width = 46;
			tfHpDuoi.width = 58;
			int itemY = curY + 2;
			mFont.tahoma_7b_dark.drawString(g, mResources.HP, num + 5, itemY, 0);
			int num2 = num + mFont.tahoma_7b_dark.getWidth(mResources.HP + " ");
			paintTFieldLine(g, tfHpOn, num2 + 5, itemY);
			num2 += tfHpOn.width;
			mFont.tahoma_7b_dark.drawString(g, "-", num2 + 8, itemY, 0);
			paintTFieldLine(g, tfHpDuoi, num2 + 15, itemY);
		}
		curY += 26;
		paintCheckboxItemCustom(g, mResources.tuDungDau, xScroll + 5, curY, isCheckAutoPean, focus: false);
		tfHpPean.width = 22;
		tfMpPean.width = 22;
		int itemY2 = curY + 2;
		mFont.tahoma_7b_dark.drawString(g, "HP <", xScroll + 95, itemY2, 0);
		int num3 = xScroll + mFont.tahoma_7b_dark.getWidth("HP < ");
		paintTFieldLine(g, tfHpPean, num3 + 95, itemY2);
		num3 += tfHpPean.width;
		mFont.tahoma_7b_dark.drawString(g, "%  MP <", num3 + 97, itemY2, 0);
		num3 += mFont.tahoma_7b_dark.getWidth("%  MP < ");
		paintTFieldLine(g, tfMpPean, num3 + 97, itemY2);
		mFont.tahoma_7b_dark.drawString(g, "%", num3 + tfMpPean.width + 99, itemY2, 0);
		curY += 26;
		int width = mFont.tahoma_7b_dark.getWidth(mResources.chonQuaiTheoTen);
		mFont.tahoma_7b_dark.drawString(g, mResources.chonQuaiTheoTen, xScroll + 5, curY + 4, 0);
		if (listMobNames != null && listMobNames.size() > 0)
		{
			paintCheckboxItemCustom(g, mResources.tatCa, xScroll + 15 + width, curY + 2, isCheckAllMob, focus: false);
			curY += 20;
			for (int i = 0; i < listMobNames.size(); i++)
			{
				int itemX = ((i % 2 == 0) ? (xScroll + 20) : (num + 10));
				int itemY3 = curY + i / 2 * 26;
				string name2 = (listMobNames.elementAt(i) as string) ?? "";
				bool isCheckedItem = i < isCheckedMobName.Length && isCheckedMobName[i];
				paintCheckboxItemCustom(g, name2, itemX, itemY3, isCheckedItem, focus: false);
			}
			curY += (listMobNames.size() + 1) / 2 * 26;
		}
		else
		{
			mFont.tahoma_7_white.drawString(g, mResources.khongCoMucTieu, xScroll + 10 + width, curY + 4, 0);
			curY += 20;
		}
		curY += 10;
		mFont.tahoma_7b_dark.drawString(g, mResources.danhSachSkillTuDanh, xScroll + 5, curY, 0);
		curY += 22;
		paintSkillsCenteredWithLeftCB(g, curY);
		curY += 105;
	}

	private void paintTabItem(mGraphics g, ref int curY)
	{
		int num = xScroll;
		int num2 = xScroll + wScroll / 2;
		for (int i = 0; i < ITEM_TEMPLATE_IDS.Length; i++)
		{
			int num3 = ((i % 2 == 0) ? num : num2);
			int num4 = curY + i / 2 * 26;
			paintUI.paintCheckPass(g, num3 + 5, num4, i < isCheckItem.Length && isCheckItem[i], focus: false);
			ItemTemplate itemTemplate = ItemTemplates.get((short)ITEM_TEMPLATE_IDS[i]);
			string st;
			if (itemTemplate != null)
			{
				SmallImage.drawSmallImage(g, itemTemplate.iconID, num3 + 37, num4 + 8, 0, mGraphics.VCENTER | mGraphics.HCENTER);
				st = ((!string.IsNullOrEmpty(itemTemplate.name)) ? itemTemplate.name : getItemName(i));
			}
			else
			{
				st = getItemName(i);
			}
			mFont.tahoma_7b_dark.drawString(g, st, num3 + 51, num4 + 2, 0);
		}
		curY += (ITEM_TEMPLATE_IDS.Length + 1) / 2 * 26;
	}

	private void paintTabPick(mGraphics g, ref int curY)
	{
		if (PICK_NAMES == null)
		{
			return;
		}
		int num = curY;
		for (int i = 1; i < PICK_NAMES.Length; i++)
		{
			paintCheckboxItemCustom(g, PICK_NAMES[i], xScroll + 5, num + (i - 1) * 26, i < isCheckPick.Length && isCheckPick[i], focus: false);
		}
		curY = num + PICK_NAMES.Length * 26 - 16;
		mFont.tahoma_7b_dark.drawString(g, mResources.dsIdNhat, xScroll + 5, curY + 7, 0);
		paintTFieldCustom(g, tfIdNhat, xScroll + 120, curY);
		curY += 30;
		mFont.tahoma_7b_dark.drawString(g, mResources.dsIdLoaiTru, xScroll + 5, curY + 7, 0);
		paintTFieldCustom(g, tfIdLoaiTru, xScroll + 120, curY);
		curY += 30;
		mFont.tahoma_7b_dark.drawString(g, mResources.dsIdVutBo, xScroll + 5, curY + 7, 0);
		paintTFieldCustom(g, tfIdVutBo, xScroll + 120, curY);
		curY += 30;
	}

	private void paintSkillsCenteredWithLeftCB(mGraphics g, int startY)
	{
		string[] array = ((Main.isPC && TField.isQwerty) ? new string[10] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" } : new string[10] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" });
		int num = xScroll + (wScroll - 280) / 2;
		for (int i = 0; i < 10; i++)
		{
			bool check = i < isCheckedSkill.Length && isCheckedSkill[i];
			int num2 = i % 5;
			int num3 = i / 5;
			int num4 = num + num2 * 56;
			int num5 = startY + num3 * 48;
			paintUI.paintCheckPass(g, num4, num5 + 19, check, focus: false);
			if (Main.isPC && !GameCanvas.isTouch)
			{
				string st = ((i < array.Length) ? array[i] : "");
				mFont.tahoma_7b_dark.drawString(g, st, num4 + 38, num5 + 3, mFont.CENTER);
				mFont.tahoma_7b_white.drawString(g, st, num4 + 38, num5 + 4, mFont.CENTER);
			}
			Skill skill = ((listSkills != null && i < listSkills.size()) ? (listSkills.elementAt(i) as Skill) : null);
			if (GameScr.imgSkill != null)
			{
				Image image = ((skill != null && Char.myCharz() != null && skill == Char.myCharz().myskill && GameScr.imgSkill2 != null) ? GameScr.imgSkill2 : GameScr.imgSkill);
				g.drawImage(image, num4 + 23, num5 + 15, 0);
			}
			if (skill != null && skill.template != null)
			{
				skill.paint(num4 + 37, num5 + 29, g);
			}
		}
	}

	private void paintTFieldCustom(mGraphics g, TField tf, int itemX, int itemY)
	{
		if (tf == null)
		{
			return;
		}
		tf.x = itemX;
		tf.y = itemY;
		if (g == null)
		{
			return;
		}
		g.setColor(tf.isFocus ? 0xA7F2AC : 0xEEC385);
		g.fillRoundRect(itemX, itemY, tf.width, tf.height, 6, 6);
		string text = tf.getText();
		if (!string.IsNullOrEmpty(text))
		{
			try
			{
				text = NinjaUtil.getMoneys(long.Parse(text.Replace(".", "")));
			}
			catch
			{
			}
		}
		if (!string.IsNullOrEmpty(text))
		{
			mFont.tahoma_7b_dark.drawString(g, text, itemX + 6, itemY + 5, 0);
		}
		if (tf.isFocus && GameCanvas.gameTick % 20 < 10)
		{
			int num = ((!string.IsNullOrEmpty(text)) ? mFont.tahoma_7b_dark.getWidth(text) : 0);
			g.setColor(0);
			g.fillRect(itemX + 6 + num, itemY + 4, 1, 14);
		}
	}

	private void paintTFieldLine(mGraphics g, TField tf, int itemX, int itemY)
	{
		if (tf == null || g == null)
		{
			return;
		}
		string text = tf.getText() ?? "";
		tf.x = itemX;
		tf.y = itemY;
		if (text.Length > 0)
		{
			mFont.tahoma_7b_dark.drawString(g, text, itemX + 4, itemY, 0);
		}
		g.setColor(tf.isFocus ? 0xA7F2AC : 0x482405);
		g.fillRect(itemX, itemY + 15, tf.width, 1);
		if (tf.isFocus && GameCanvas.gameTick % 20 < 10)
		{
			int num = ((text.Length > 0) ? mFont.tahoma_7b_dark.getWidth(text) : 0);
			g.setColor(0);
			g.fillRect(itemX + 4 + num, itemY + 1, 1, 13);
		}
	}

	private void paintCheckboxItem(mGraphics g, int index, int itemX, int itemY, bool isCheckedItem, bool focus)
	{
		bool flag = focus && !GameCanvas.isTouch;
		paintUI.paintCheckPass(g, itemX, itemY, isCheckedItem, focus);
		string[] oPTION_NAMES = OPTION_NAMES;
		if (oPTION_NAMES != null && index >= 0 && index < oPTION_NAMES.Length)
		{
			(flag ? mFont.tahoma_7b_red : mFont.tahoma_7b_dark).drawString(g, oPTION_NAMES[index], itemX + 24, itemY + 2, 0);
		}
	}

	private void paintCheckboxItemCustom(mGraphics g, string name, int itemX, int itemY, bool isCheckedItem, bool focus)
	{
		bool flag = focus && !GameCanvas.isTouch;
		paintUI.paintCheckPass(g, itemX, itemY, isCheckedItem, focus);
		(flag ? mFont.tahoma_7b_red : mFont.tahoma_7b_dark).drawString(g, name, itemX + 24, itemY + 2, 0);
	}

	public override void update()
	{
		GameScr.gI().update();
		if (scroll != null)
		{
			scroll.updatecm();
		}
		if (tfHpOn != null)
		{
			if (!tfHpOn.isFocus && (string.IsNullOrEmpty(tfHpOn.getText()) || tfHpOn.getText().Trim().Length == 0))
			{
				tfHpOn.setText("0");
			}
			updateTField(tfHpOn, ref hpQuaiOn, 0L);
		}
		if (tfHpDuoi != null)
		{
			if (!tfHpDuoi.isFocus && (string.IsNullOrEmpty(tfHpDuoi.getText()) || tfHpDuoi.getText().Trim().Length == 0))
			{
				tfHpDuoi.setText("Không giới hạn");
			}
			updateTField(tfHpDuoi, ref hpQuaiDuoi, 10000000001L);
		}
		if (tfHpDuoi != null && tfHpOn != null && !tfHpDuoi.isFocus && !tfHpOn.isFocus && hpQuaiDuoi < hpQuaiOn)
		{
			hpQuaiDuoi = hpQuaiOn + 1;
			tfHpDuoi.setText(NinjaUtil.getMoneys(hpQuaiDuoi));
		}
		if (tfHpPean != null)
		{
			if (!tfHpPean.isFocus && (string.IsNullOrEmpty(tfHpPean.getText()) || tfHpPean.getText().Trim().Length == 0))
			{
				tfHpPean.setText("50");
			}
			updateTFieldInt(tfHpPean, ref peanHpPercent);
		}
		if (tfMpPean != null)
		{
			if (!tfMpPean.isFocus && (string.IsNullOrEmpty(tfMpPean.getText()) || tfMpPean.getText().Trim().Length == 0))
			{
				tfMpPean.setText("50");
			}
			updateTFieldInt(tfMpPean, ref peanMpPercent);
		}
		if (tfBuaKills != null)
		{
			for (int i = 0; i < 4 && i < tfBuaKills.Length; i++)
			{
				updateTFieldInt(tfBuaKills[i], ref muaBuaKills[i]);
			}
		}
		if (tfIdNhat != null)
		{
			if (tfIdNhat.isFocus)
			{
				tfIdNhat.update();
			}
			strIdNhat = tfIdNhat.getText() ?? "";
		}
		if (tfIdLoaiTru != null)
		{
			if (tfIdLoaiTru.isFocus)
			{
				tfIdLoaiTru.update();
			}
			strIdLoaiTru = tfIdLoaiTru.getText() ?? "";
		}
		if (tfIdVutBo != null)
		{
			if (tfIdVutBo.isFocus)
			{
				tfIdVutBo.update();
			}
			strIdVutBo = tfIdVutBo.getText() ?? "";
		}
	}

	private void updateTField(TField tf, ref long value, long defaultVal)
	{
		if (tf == null)
		{
			return;
		}
		if (tf.isFocus)
		{
			tf.update();
		}
		try
		{
			string text = tf.getText();
			if (!string.IsNullOrEmpty(text) && text != "Không giới hạn" && text != mResources.khongGioiHan)
			{
				string text2 = text.Replace(".", "").Trim();
				value = ((text2.Length > 0) ? long.Parse(text2) : defaultVal);
			}
			else
			{
				value = defaultVal;
			}
		}
		catch
		{
			value = defaultVal;
		}
	}

	private void updateTFieldInt(TField tf, ref int value)
	{
		if (tf == null)
		{
			return;
		}
		if (tf.isFocus)
		{
			tf.update();
		}
		try
		{
			string text = tf.getText();
			value = ((!string.IsNullOrEmpty(text)) ? int.Parse(text.Trim()) : 0);
		}
		catch
		{
			value = 0;
		}
	}

	public static void resetToDefault()
	{
		for (int i = 0; i < isChecked.Length; i++)
		{
			isChecked[i] = false;
		}
		if (isChecked.Length > 1)
		{
			isChecked[1] = true;
		}
		isCheckAttackByHp = false;
		hpQuaiOn = 0L;
		hpQuaiDuoi = 10000000001L;
		isCheckAutoPean = false;
		peanHpPercent = 50;
		peanMpPercent = 50;
		isCheckAllMob = true;
		for (int j = 0; j < isCheckedMobName.Length; j++)
		{
			isCheckedMobName[j] = true;
		}
		for (int k = 0; k < isCheckedSkill.Length; k++)
		{
			isCheckedSkill[k] = false;
		}
		if (isCheckedSkill.Length > 1)
		{
			isCheckedSkill[0] = true;
			isCheckedSkill[1] = true;
		}
		for (int l = 0; l < isCheckItem.Length; l++)
		{
			isCheckItem[l] = false;
		}
		for (int m = 0; m < isCheckPick.Length; m++)
		{
			isCheckPick[m] = false;
		}
		if (isCheckPick.Length > 0)
		{
			isCheckPick[0] = true;
		}
		strIdNhat = "";
		strIdLoaiTru = "";
		strIdVutBo = "";
		if (instance != null)
		{
			instance.tfHpOn?.setText("0");
			instance.tfHpDuoi?.setText("Không giới hạn");
			instance.tfHpPean?.setText("50");
			instance.tfMpPean?.setText("50");
			instance.tfIdNhat?.setText("");
			instance.tfIdLoaiTru?.setText("");
			instance.tfIdVutBo?.setText("");
		}
	}

	public static void saveAutoTrainConfig()
	{
		Char @char = Char.myCharz();
		if (@char == null)
		{
			return;
		}
		try
		{
			myWriter myWriter2 = new myWriter();
			myWriter2.writeBoolean(isChecked[1]);
			myWriter2.writeBoolean(isChecked[2]);
			myWriter2.writeBoolean(isCheckAttackByHp);
			AutoTrainMenu autoTrainMenu = gI();
			if (hpQuaiDuoi < hpQuaiOn)
			{
				hpQuaiDuoi = hpQuaiOn + 1;
				autoTrainMenu?.tfHpDuoi?.setText(NinjaUtil.getMoneys(hpQuaiDuoi));
			}
			string str = ((autoTrainMenu?.tfHpOn != null && !string.IsNullOrEmpty(autoTrainMenu.tfHpOn.getText())) ? autoTrainMenu.tfHpOn.getText() : "0");
			string str2 = ((autoTrainMenu?.tfHpDuoi != null && !string.IsNullOrEmpty(autoTrainMenu.tfHpDuoi.getText())) ? autoTrainMenu.tfHpDuoi.getText() : "Không giới hạn");
			myWriter2.writeUTF(str);
			myWriter2.writeUTF(str2);
			myWriter2.writeBoolean(isCheckAutoPean);
			string str3 = ((autoTrainMenu?.tfHpPean != null && !string.IsNullOrEmpty(autoTrainMenu.tfHpPean.getText())) ? autoTrainMenu.tfHpPean.getText() : "50");
			string str4 = ((autoTrainMenu?.tfMpPean != null && !string.IsNullOrEmpty(autoTrainMenu.tfMpPean.getText())) ? autoTrainMenu.tfMpPean.getText() : "50");
			myWriter2.writeUTF(str3);
			myWriter2.writeUTF(str4);
			myWriter2.writeBoolean(isCheckAllMob);
			int num = ((listMobNames != null && isCheckedMobName != null) ? System.Math.Min(listMobNames.size(), isCheckedMobName.Length) : 0);
			myWriter2.writeSByte((sbyte)num);
			for (int i = 0; i < num; i++)
			{
				string str5 = (listMobNames.elementAt(i) as string) ?? "";
				myWriter2.writeUTF(str5);
				myWriter2.writeBoolean(isCheckedMobName[i]);
			}
			myWriter2.writeSByte((sbyte)isCheckedSkill.Length);
			for (int j = 0; j < isCheckedSkill.Length; j++)
			{
				myWriter2.writeBoolean(isCheckedSkill[j]);
			}
			string[] iTEM_NAMES = ITEM_NAMES;
			myWriter2.writeSByte((sbyte)iTEM_NAMES.Length);
			for (int k = 0; k < iTEM_NAMES.Length; k++)
			{
				myWriter2.writeBoolean(k < isCheckItem.Length && isCheckItem[k]);
			}
			string[] pICK_NAMES = PICK_NAMES;
			int num2 = ((pICK_NAMES != null) ? pICK_NAMES.Length : 0);
			myWriter2.writeSByte((sbyte)num2);
			for (int l = 0; l < num2; l++)
			{
				myWriter2.writeBoolean(l < isCheckPick.Length && isCheckPick[l]);
			}
			string str6 = ((autoTrainMenu?.tfIdNhat != null) ? autoTrainMenu.tfIdNhat.getText() : strIdNhat) ?? "";
			string str7 = ((autoTrainMenu?.tfIdLoaiTru != null) ? autoTrainMenu.tfIdLoaiTru.getText() : strIdLoaiTru) ?? "";
			string str8 = ((autoTrainMenu?.tfIdVutBo != null) ? autoTrainMenu.tfIdVutBo.getText() : strIdVutBo) ?? "";
			myWriter2.writeUTF(str6);
			myWriter2.writeUTF(str7);
			myWriter2.writeUTF(str8);
			Rms.saveRMS("AT_v6_" + @char.charID, myWriter2.getData());
			saveMobSelectionForCurrentMap();
		}
		catch
		{
		}
	}

	public static void loadAutoTrainConfig()
	{
		Char @char = Char.myCharz();
		if (@char == null)
		{
			return;
		}
		bool flag = isChecked[0];
		sbyte[] array = Rms.loadRMS("AT_v6_" + @char.charID);
		bool flag2 = array != null;
		if (array == null)
		{
			array = Rms.loadRMS("AT_v5_" + @char.charID);
		}
		if (array == null)
		{
			resetToDefault();
			isChecked[0] = flag;
			if (!mobSelectionPreparedForCurrentMap)
			{
				resetMobSelectionToAll();
			}
			else if (!loadMobSelectionForCurrentMap())
			{
				resetMobSelectionToAll();
			}
			ensureMobSelectionHasAnySelected();
			return;
		}
		try
		{
			myReader myReader2 = new myReader(array);
			isChecked[0] = flag;
			isChecked[1] = myReader2.readBool();
			isChecked[2] = myReader2.readBool();
			isCheckAttackByHp = myReader2.readBool();
			string text = myReader2.readUTF();
			string text2 = myReader2.readUTF();
			try
			{
				hpQuaiOn = ((!string.IsNullOrEmpty(text)) ? long.Parse(text.Replace(".", "").Trim()) : 0L);
			}
			catch
			{
				hpQuaiOn = 0L;
			}
			string text3;
			if (string.IsNullOrEmpty(text2) || text2.Equals("Không giới hạn") || text2.Equals(mResources.khongGioiHan))
			{
				hpQuaiDuoi = 10000000001L;
				text3 = "Không giới hạn";
			}
			else
			{
				try
				{
					hpQuaiDuoi = long.Parse(text2.Replace(".", "").Trim());
				}
				catch
				{
					hpQuaiDuoi = 10000000001L;
				}
				if (hpQuaiDuoi < hpQuaiOn)
				{
					hpQuaiDuoi = hpQuaiOn + 1;
				}
				text3 = NinjaUtil.getMoneys(hpQuaiDuoi);
			}
			string text4 = NinjaUtil.getMoneys(hpQuaiOn);
			try
			{
				isCheckAutoPean = myReader2.readBool();
				string s = myReader2.readUTF();
				string s2 = myReader2.readUTF();
				try
				{
					peanHpPercent = int.Parse(s);
				}
				catch
				{
					peanHpPercent = 50;
				}
				try
				{
					peanMpPercent = int.Parse(s2);
				}
				catch
				{
					peanMpPercent = 50;
				}
			}
			catch
			{
				isCheckAutoPean = false;
				peanHpPercent = 50;
				peanMpPercent = 50;
			}
			isCheckAllMob = myReader2.readBool();
			for (int i = 0; i < isCheckedMobName.Length; i++)
			{
				isCheckedMobName[i] = isCheckAllMob;
			}
			if (flag2)
			{
				int num = myReader2.readByte();
				if (num < 0)
				{
					num = 0;
				}
				for (int j = 0; j < num; j++)
				{
					string text5 = myReader2.readUTF();
					bool flag3 = myReader2.readBool();
					if (!flag3 || string.IsNullOrEmpty(text5) || listMobNames == null)
					{
						continue;
					}
					for (int k = 0; k < listMobNames.size(); k++)
					{
						if (k < isCheckedMobName.Length && text5.Equals(listMobNames.elementAt(k) as string))
						{
							isCheckedMobName[k] = true;
							break;
						}
					}
				}
			}
			ensureMobSelectionHasAnySelected();
			int num2 = myReader2.readByte();
			for (int l = 0; l < num2; l++)
			{
				bool flag4 = myReader2.readBool();
				if (l < isCheckedSkill.Length)
				{
					isCheckedSkill[l] = flag4;
				}
			}
			int num3 = myReader2.readByte();
			for (int m = 0; m < num3; m++)
			{
				bool flag5 = myReader2.readBool();
				if (m < isCheckItem.Length)
				{
					isCheckItem[m] = flag5;
				}
			}
			int num4 = myReader2.readByte();
			for (int n = 0; n < num4; n++)
			{
				bool flag6 = myReader2.readBool();
				if (n < isCheckPick.Length)
				{
					isCheckPick[n] = flag6;
				}
			}
			strIdNhat = myReader2.readUTF() ?? "";
			strIdLoaiTru = myReader2.readUTF() ?? "";
			strIdVutBo = myReader2.readUTF() ?? "";
			AutoTrainMenu autoTrainMenu = gI();
			if (autoTrainMenu != null)
			{
				autoTrainMenu.tfHpOn?.setText(text4);
				autoTrainMenu.tfHpDuoi?.setText(text3);
				autoTrainMenu.tfHpPean?.setText(peanHpPercent.ToString());
				autoTrainMenu.tfMpPean?.setText(peanMpPercent.ToString());
				autoTrainMenu.tfIdNhat?.setText(strIdNhat);
				autoTrainMenu.tfIdLoaiTru?.setText(strIdLoaiTru);
				autoTrainMenu.tfIdVutBo?.setText(strIdVutBo);
			}
			if (!mobSelectionPreparedForCurrentMap)
			{
				resetMobSelectionToAll();
			}
			else if (!loadMobSelectionForCurrentMap())
			{
				resetMobSelectionToAll();
			}
			ensureMobSelectionHasAnySelected();
		}
		catch
		{
			resetToDefault();
			isChecked[0] = flag;
		}
	}

	public override void updateKey()
	{
		base.updateKey();
		if (GameCanvas.isPointerJustRelease)
		{
			int px = GameCanvas.px;
			int py = GameCanvas.py;
			if (px >= x + 5 && px <= x + 58 && py >= y + 4 && py <= y + 25)
			{
				clearTouchState();
				resetToDefault();
				return;
			}
			if (px >= x + w - 30 && px <= x + w && py >= y && py <= y + 25)
			{
				clearTouchState();
				perform(1, null);
				return;
			}
			if (px >= xScroll + wScroll - 20 && px <= xScroll + wScroll + 5 && py >= yScroll - 10 && py <= yScroll + 20)
			{
				if (scroll != null)
				{
					scroll.cmtoY -= 60;
					if (scroll.cmtoY < 0)
					{
						scroll.cmtoY = 0;
					}
				}
				clearTouchState();
				return;
			}
			if (px >= xScroll + wScroll - 20 && px <= xScroll + wScroll + 5 && py >= yScroll + hScroll - 20 && py <= yScroll + hScroll + 10)
			{
				if (scroll != null)
				{
					scroll.cmtoY += 60;
					if (scroll.cmtoY > scroll.cmyLim)
					{
						scroll.cmtoY = scroll.cmyLim;
					}
				}
				clearTouchState();
				return;
			}
			int num = (isNhat ? 3 : 2);
			int num2 = (w - 70) / num;
			int num3 = x + 35;
			for (int i = 0; i < num; i++)
			{
				if (px >= num3 + i * num2 && px <= num3 + (i + 1) * num2 && py >= y + 24 && py <= y + 48)
				{
					if (currentTab != i)
					{
						currentTab = i;
						clearAllFocus();
					}
					clearTouchState();
					return;
				}
			}
			if (GameCanvas.isPointerHoldIn(xScroll, yScroll, wScroll, hScroll) && scroll != null)
			{
				int screenY = yScroll - scroll.cmy + 5;
				if (currentTab == 0)
				{
					updateKeyTabTrain(screenY);
				}
				else if (currentTab == 1)
				{
					updateKeyTabItem(screenY);
				}
				else if (currentTab == 2 && isNhat)
				{
					updateKeyTabPick(screenY);
				}
			}
		}
		if (GameCanvas.pXYScrollMouse != 0 && scroll != null)
		{
			scroll.cmtoY -= GameCanvas.pXYScrollMouse * 35;
			if (scroll.cmtoY < 0)
			{
				scroll.cmtoY = 0;
			}
			if (scroll.cmtoY > scroll.cmyLim)
			{
				scroll.cmtoY = scroll.cmyLim;
			}
			GameCanvas.pXYScrollMouse = 0;
		}
		if (scroll != null)
		{
			int cmtoY = scroll.cmtoY;
			scroll.selectedItem = -1;
			scroll.updateKey();
			if (GameCanvas.isPointerClick)
			{
				scroll.cmtoY = cmtoY;
			}
			if (scroll.cmy < 0)
			{
				scroll.cmy = 0;
				scroll.cmtoY = 0;
			}
		}
		if (!GameCanvas.isTouch)
		{
			if (GameCanvas.keyPressed[2])
			{
				GameCanvas.keyPressed[2] = false;
				selectedIndex--;
				if (selectedIndex < 0)
				{
					selectedIndex = 10;
				}
				updateFocusState();
			}
			else if (GameCanvas.keyPressed[8])
			{
				GameCanvas.keyPressed[8] = false;
				selectedIndex++;
				if (selectedIndex > 10)
				{
					selectedIndex = 0;
				}
				updateFocusState();
			}
			else if (GameCanvas.keyPressed[5])
			{
				GameCanvas.keyPressed[5] = false;
				if (selectedIndex >= 0 && selectedIndex < OPTION_NAMES.Length)
				{
					toggleOption(selectedIndex);
				}
				else if (selectedIndex == 8)
				{
					focusTField(tfHpOn);
				}
				else if (selectedIndex == 9)
				{
					focusTField(tfHpDuoi);
				}
			}
		}
	}

	private void clearTouchState()
	{
		GameCanvas.clearAllPointerEvent();
		GameCanvas.isPointerClick = false;
		GameCanvas.isPointerJustDown = false;
		GameCanvas.isPointerJustRelease = false;
		resetScrollTouchState();
	}

	private void resetScrollTouchState()
	{
		if (scrolls == null)
		{
			return;
		}
		for (int i = 0; i < scrolls.Length; i++)
		{
			if (scrolls[i] != null)
			{
				scrolls[i].pointerIsDowning = false;
				scrolls[i].isDownWhenRunning = false;
				scrolls[i].selectedItem = -1;
			}
		}
	}

	private bool checkTouchTField(TField tf, int screenY)
	{
		if (tf != null && GameCanvas.py >= screenY - 5 && GameCanvas.py <= screenY + 28 && GameCanvas.px >= tf.x - 4 && GameCanvas.px <= tf.x + tf.width + 6)
		{
			focusTField(tf);
			clearTouchState();
			return true;
		}
		return false;
	}

	private bool isTouchCheckbox(int xCB, int yCB)
	{
		return GameCanvas.px >= xCB - 2 && GameCanvas.px <= xCB + 22 && GameCanvas.py >= yCB - 2 && GameCanvas.py <= yCB + 20;
	}

	private void updateKeyTabTrain(int screenY)
	{
		int num = xScroll + wScroll / 2;
		if (isTouchCheckbox(xScroll + 5, screenY))
		{
			toggleOption(0);
			clearTouchState();
			return;
		}
		if (isTouchCheckbox(num + 5, screenY))
		{
			toggleOption(1);
			clearTouchState();
			return;
		}
		if (isTouchCheckbox(xScroll + 5, screenY + 26))
		{
			toggleOption(2);
			clearTouchState();
			return;
		}
		if (isTouchCheckbox(num + 5, screenY + 26))
		{
			isCheckAttackByHp = !isCheckAttackByHp;
			clearAllFocus();
			clearTouchState();
			return;
		}
		if (isTouchCheckbox(xScroll + 5, screenY + 52))
		{
			isCheckPick[0] = !isCheckPick[0];
			clearAllFocus();
			clearTouchState();
			return;
		}
		if (isCheckAttackByHp)
		{
			if (checkTouchTField(tfHpOn, screenY + 52))
			{
				return;
			}
			if (checkTouchTField(tfHpDuoi, screenY + 52))
			{
				return;
			}
		}
		if (isTouchCheckbox(xScroll + 5, screenY + 78))
		{
			isCheckAutoPean = !isCheckAutoPean;
			clearAllFocus();
			clearTouchState();
			return;
		}
		if (checkTouchTField(tfHpPean, screenY + 78) || checkTouchTField(tfMpPean, screenY + 78))
		{
			return;
		}
		int num2 = ((listMobNames != null) ? listMobNames.size() : 0);
		if (num2 > 0)
		{
			int width = mFont.tahoma_7b_dark.getWidth(mResources.chonQuaiTheoTen);
			if (isTouchCheckbox(xScroll + 15 + width, screenY + 106))
			{
				isCheckAllMob = !isCheckAllMob;
				for (int i = 0; i < num2 && i < isCheckedMobName.Length; i++)
				{
					isCheckedMobName[i] = isCheckAllMob;
				}
				saveMobSelectionForCurrentMap();
				clearTouchState();
				return;
			}
			int num3 = screenY + 124;
			for (int j = 0; j < num2; j++)
			{
				int xCB = ((j % 2 == 0) ? (xScroll + 20) : (num + 10));
				int yCB = num3 + j / 2 * 26;
				if (isTouchCheckbox(xCB, yCB))
				{
					if (j < isCheckedMobName.Length)
					{
						isCheckedMobName[j] = !isCheckedMobName[j];
					}
					bool flag = true;
					for (int k = 0; k < num2 && k < isCheckedMobName.Length; k++)
					{
						if (!isCheckedMobName[k])
						{
							flag = false;
							break;
						}
					}
					isCheckAllMob = flag;
					saveMobSelectionForCurrentMap();
					clearTouchState();
					return;
				}
			}
		}
		int num4 = screenY + 124 + ((num2 > 0) ? ((num2 + 1) / 2 * 26) : 0) + 51;
		int num5 = xScroll + (wScroll - 280) / 2;
		for (int l = 0; l < 10; l++)
		{
			int xCB2 = num5 + l % 5 * 56;
			int yCB2 = num4 + l / 5 * 48 + 19;
			if (isTouchCheckbox(xCB2, yCB2))
			{
				if (l < isCheckedSkill.Length)
				{
					isCheckedSkill[l] = !isCheckedSkill[l];
				}
				clearAllFocus();
				clearTouchState();
				return;
			}
		}
	}

	private void updateKeyTabItem(int screenY)
	{
		int num = xScroll + wScroll / 2;
		for (int i = 0; i < ITEM_TEMPLATE_IDS.Length; i++)
		{
			int xCB = ((i % 2 == 0) ? (xScroll + 5) : (num + 5));
			int yCB = screenY + i / 2 * 26;
			if (isTouchCheckbox(xCB, yCB))
			{
				if (i < isCheckItem.Length)
				{
					isCheckItem[i] = !isCheckItem[i];
				}
				clearAllFocus();
				clearTouchState();
				return;
			}
		}
	}

	private void updateKeyTabPick(int screenY)
	{
		if (PICK_NAMES == null)
		{
			return;
		}
		for (int i = 1; i < PICK_NAMES.Length; i++)
		{
			if (isTouchCheckbox(xScroll + 5, screenY + (i - 1) * 26))
			{
				if (i < isCheckPick.Length)
				{
					isCheckPick[i] = !isCheckPick[i];
				}
				clearAllFocus();
				clearTouchState();
				return;
			}
		}
		int num = screenY + PICK_NAMES.Length * 26;
		if (!checkTouchTField(tfIdNhat, num - 16) && !checkTouchTField(tfIdLoaiTru, num + 14))
		{
			checkTouchTField(tfIdVutBo, num + 44);
		}
	}

	public override void keyPress(int keyCode)
	{
		if (tfHpOn != null && tfHpOn.isFocus)
		{
			handleHpKeyPress(tfHpOn, keyCode);
			return;
		}
		if (tfHpDuoi != null && tfHpDuoi.isFocus)
		{
			handleHpKeyPress(tfHpDuoi, keyCode);
			return;
		}
		if (tfHpPean != null && tfHpPean.isFocus)
		{
			handlePercentKeyPress(tfHpPean, keyCode);
			return;
		}
		if (tfMpPean != null && tfMpPean.isFocus)
		{
			handlePercentKeyPress(tfMpPean, keyCode);
			return;
		}
		if (tfBuaKills != null)
		{
			for (int i = 0; i < tfBuaKills.Length; i++)
			{
				if (tfBuaKills[i] != null && tfBuaKills[i].isFocus)
				{
					tfBuaKills[i].keyPressed(keyCode);
					return;
				}
			}
		}
		if (tfIdNhat != null && tfIdNhat.isFocus)
		{
			tfIdNhat.keyPressed(keyCode);
		}
		else if (tfIdLoaiTru != null && tfIdLoaiTru.isFocus)
		{
			tfIdLoaiTru.keyPressed(keyCode);
		}
		else if (tfIdVutBo != null && tfIdVutBo.isFocus)
		{
			tfIdVutBo.keyPressed(keyCode);
		}
		else
		{
			base.keyPress(keyCode);
		}
	}

	private void handlePercentKeyPress(TField tf, int keyCode)
	{
		if (tf == null)
		{
			return;
		}
		string text = tf.getText() ?? "";
		if (keyCode == 8 || keyCode == -8 || keyCode == 204 || keyCode == 19)
		{
			if (text.Length > 0)
			{
				tf.setText(text.Substring(0, text.Length - 1));
			}
		}
		else if (keyCode >= 48 && keyCode <= 57 && text.Length < 3)
		{
			char c = (char)keyCode;
			string s = ((text == "0" || text.Length == 0) ? c.ToString() : (text + c));
			if (int.TryParse(s, out var result))
			{
				if (result > 100)
				{
					result = 100;
				}
				tf.setText(result.ToString());
			}
		}
	}

	private void handleHpKeyPress(TField tf, int keyCode)
	{
		if (tf == null)
		{
			return;
		}
		string text = tf.getText() ?? "";
		if (keyCode == 8 || keyCode == -8 || keyCode == 204 || keyCode == 19)
		{
			if (text == "Không giới hạn" || text == mResources.khongGioiHan)
			{
				tf.setText("");
				return;
			}
			string text2 = text.Replace(".", "");
			if (text2.Length <= 1)
			{
				tf.setText("");
				return;
			}
			string s = text2.Substring(0, text2.Length - 1);
			if (long.TryParse(s, out var result))
			{
				tf.setText(NinjaUtil.getMoneys(result));
			}
		}
		else if (keyCode >= 48 && keyCode <= 57)
		{
			string text3 = ((text == "Không giới hạn" || text == mResources.khongGioiHan) ? "" : text.Replace(".", ""));
			int num = ((tf == tfHpOn) ? 8 : 14);
			if (text3.Length < num)
			{
				char c = (char)keyCode;
				string s2 = ((text3 == "0" || text3.Length == 0) ? c.ToString() : (text3 + c));
				if (long.TryParse(s2, out var result2))
				{
					tf.setText(NinjaUtil.getMoneys(result2));
				}
			}
		}
	}

	private bool isTextContainsLetters(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return false;
		}
		for (int i = 0; i < text.Length; i++)
		{
			if (char.IsLetter(text[i]))
			{
				return true;
			}
		}
		return false;
	}

	private void focusTField(TField tf)
	{
		TField[] array = new TField[11]
		{
			tfHpOn,
			tfHpDuoi,
			tfHpPean,
			tfMpPean,
			(tfBuaKills != null && tfBuaKills.Length > 0) ? tfBuaKills[0] : null,
			(tfBuaKills != null && tfBuaKills.Length > 1) ? tfBuaKills[1] : null,
			(tfBuaKills != null && tfBuaKills.Length > 2) ? tfBuaKills[2] : null,
			(tfBuaKills != null && tfBuaKills.Length > 3) ? tfBuaKills[3] : null,
			tfIdNhat,
			tfIdLoaiTru,
			tfIdVutBo
		};
		for (int i = 0; i < array.Length; i++)
		{
			TField tField = array[i];
			if (tField != null)
			{
				tField.isFocus = tField == tf;
				if (tField.isFocus && (tField == tfHpOn || tField == tfHpDuoi || tField == tfHpPean || tField == tfMpPean) && isTextContainsLetters(tField.getText()))
				{
					tField.setText("");
				}
			}
		}
	}

	private void updateFocusState()
	{
		if (selectedIndex == 8)
		{
			focusTField(tfHpOn);
		}
		else if (selectedIndex == 9)
		{
			focusTField(tfHpDuoi);
		}
		else
		{
			clearAllFocus();
		}
	}

	private static void ensureSkillSelectionForAutoAttack()
	{
		int num = System.Math.Min(10, isCheckedSkill.Length);
		for (int i = 0; i < num; i++)
		{
			if (isCheckedSkill[i])
			{
				return;
			}
		}
		for (int j = 0; j < num; j++)
		{
			isCheckedSkill[j] = true;
		}
		saveAutoTrainConfig();
		Res.outz("[AutoTrain] Chưa chọn skill tự đánh, đã tự chọn tất cả 10 ô skill và lưu cấu hình RMS.");
	}

	public static void setAutoAttack(bool enable, string reason)
	{
		if (enable)
		{
			ensureSkillSelectionForAutoAttack();
		}
		isChecked[0] = enable;
		GameScr.isAutoPlay = enable;
		Res.outz("[AutoTrain] >> [CHECKBOX TỰ ĐỘNG ĐÁNH] = " + (enable ? "BẬT (TRUE)" : "TẮT (FALSE)") + " | Nguồn kích hoạt: " + reason);
		if (isCheckItem == null || isCheckItem.Length <= 0 || !isCheckItem[0])
		{
			return;
		}
		Char @char = Char.myCharz();
		if (@char == null || @char.arrItemBag == null)
		{
			return;
		}
		if ((enable && !GameScr.canAutoPlay) || (!enable && GameScr.canAutoPlay))
		{
			for (int i = 0; i < @char.arrItemBag.Length; i++)
			{
				Item item = @char.arrItemBag[i];
				if (item != null && item.template != null && item.template.id == 521)
				{
					Service.gI().useItem(0, 1, (sbyte)item.indexUI, -1);
					lastTimeUseItem = mSystem.currentTimeMillis();
					break;
				}
			}
		}
	}

	private void toggleOption(int index)
	{
		selectedIndex = -1;
		if (index == 0)
		{
			setAutoAttack(!isChecked[0], "Click chuột vào ô Checkbox trên Menu Auto");
		}
		else if (index >= 0 && index < isChecked.Length)
		{
			isChecked[index] = !isChecked[index];
		}
		clearAllFocus();
	}

	public void perform(int idAction, object p)
	{
		if (idAction == 1)
		{
			saveAutoTrainConfig();
			clearAllFocus();
			clearTouchState();
			GameCanvas.currentScreen = GameScr.gI();
		}
	}
}
