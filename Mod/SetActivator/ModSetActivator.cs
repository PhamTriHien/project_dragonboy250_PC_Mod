using System;
using System.Collections.Generic;

/// <summary>
/// Module Úp Set Kích Hoạt (SKH): Quản lý nhận diện 13 bộ set kích hoạt, tự động lọc, bán đồ rác và cất trang bị SKH/Sao vào rương nhà.
/// </summary>
public static class ModSetActivator
{
	/// <summary>
	/// ENUM SetActivatorState: Định nghĩa và điều phối logic trong hệ thống game.
	/// </summary>
	public enum SetActivatorState
	{
		Idle,
		Farming,
		BagFull_SavingLocation,
		MovingToUron,
		InteractingUron,
		SellingJunk,
		MovingToHomeBox,
		InteractingBox,
		StoringToBox,
		MovingToSanta,
		BuyingTdltAtSanta,
		ReturningToFarm,
		SwitchingZone,
		TeleportingToSpot
	}

	public static bool isActive = false;
	public static bool autoSellJunk = true;
	public static bool autoStoreToBox = true;
	public static int minStarToKeep = 1; // 0: Bán cả đồ sao, 1: Giữ >= 1 sao, 2: Giữ >= 2 sao, 3: Giữ >= 3 sao
	public static bool showItemId = true;
	public static bool showSkhHud = false;
	public static bool sellGodItems = false;
	public static bool sellSkhItems = false;
	public static int sellMaxLevel = 12;
	public static int minEmptyBagSlots = 1;
	public static string keepItemIdsRaw = "521;1523;1524";
	public static string sellItemIdsRaw = "";
	public static bool autoUseTdlt = false;
	public static bool autoBuyTdlt = false;
	private static long lastTdltActionTime = 0;
	private static int s_SantaBuyStep = 0;
	private static int s_SantaRetryCount = 0;
	private static bool s_TripWasTanSat = false;
	private static bool s_TripWasGoBack = false;
	private static bool s_ReopenUiAfterNumericInput = false;

	private sealed class NumericSettingListener : IActionListener
	{
		private readonly int mode;

		public NumericSettingListener(int mode)
		{
			this.mode = mode;
		}

		public void perform(int idAction, object p)
		{
			try
			{
				if (idAction == 1 && GameCanvas.inputDlg != null && GameCanvas.inputDlg.tfInput != null)
				{
					string raw = (GameCanvas.inputDlg.tfInput.getText() ?? string.Empty).Trim();
					if (int.TryParse(raw, out int val))
					{
						if (mode == 0)
						{
							if (val < 1) val = 1;
							if (val > 20) val = 20;
							sellMaxLevel = val;
							GameScr.info1?.addInfo("Bán trang bị tối đa Cấp <= " + sellMaxLevel, 0);
						}
						else
						{
							if (val < 0) val = 0;
							if (val > 20) val = 20;
							minEmptyBagSlots = val;
							GameScr.info1?.addInfo("Dọn hành trang khi ô trống <= " + minEmptyBagSlots, 0);
						}
						ModConfig.SaveConfig();
						SoundMn.gI().buttonClick();
					}
				}
				GameCanvas.endDlg();
				if (s_ReopenUiAfterNumericInput)
				{
					s_ReopenUiAfterNumericInput = false;
					ModUI.uiCustomOpen = true;
				}
			}
			catch
			{
				GameCanvas.endDlg();
			}
		}
	}

	public static void ShowInputSellMaxLevel()
	{
		if (GameCanvas.inputDlg == null) return;
		s_ReopenUiAfterNumericInput = ModUI.uiCustomOpen;
		ModUI.uiCustomOpen = false;
		ModMenu.modMenuOpen = false;
		GameCanvas.inputDlg.show("Nhập cấp đồ tối đa cho phép bán rác (1 - 20):", new Command("OK", new NumericSettingListener(0), 1, null), TField.INPUT_TYPE_NUMERIC);
		if (GameCanvas.inputDlg.tfInput != null)
		{
			GameCanvas.inputDlg.tfInput.setMaxTextLenght(2);
			GameCanvas.inputDlg.tfInput.setText(sellMaxLevel.ToString());
			if (!Main.isPC) GameCanvas.inputDlg.tfInput.setFocusWithKb(isFocus: true);
		}
	}

	public static void ShowInputMinEmptyBagSlots()
	{
		if (GameCanvas.inputDlg == null) return;
		s_ReopenUiAfterNumericInput = ModUI.uiCustomOpen;
		ModUI.uiCustomOpen = false;
		ModMenu.modMenuOpen = false;
		GameCanvas.inputDlg.show("Nhập ngưỡng ô trống đi cất/bán đồ (0 - 20):", new Command("OK", new NumericSettingListener(1), 1, null), TField.INPUT_TYPE_NUMERIC);
		if (GameCanvas.inputDlg.tfInput != null)
		{
			GameCanvas.inputDlg.tfInput.setMaxTextLenght(2);
			GameCanvas.inputDlg.tfInput.setText(minEmptyBagSlots.ToString());
			if (!Main.isPC) GameCanvas.inputDlg.tfInput.setFocusWithKb(isFocus: true);
		}
	}

	private class IdListChatListener : IChatable
	{
		public bool isSellList;

		public void onChatFromMe(string text, string to)
		{
			ChatTextField.gI().isShow = false;
			ChatTextField.gI().parentScreen = GameScr.gI();
			string cleaned = (text ?? string.Empty).Trim();
			if (isSellList)
			{
				sellItemIdsRaw = cleaned;
				GameScr.info1.addInfo(string.IsNullOrEmpty(cleaned) ? "Đã xóa danh sách ID bán thêm" : ("ID bán thêm: " + cleaned), 0);
			}
			else
			{
				keepItemIdsRaw = cleaned;
				GameScr.info1.addInfo(string.IsNullOrEmpty(cleaned) ? "Đã xóa danh sách ID giữ lại" : ("ID giữ lại: " + cleaned), 0);
			}
			ModConfig.SaveConfig();
		}

		public void onCancelChat()
		{
			ChatTextField.gI().isShow = false;
			ChatTextField.gI().parentScreen = GameScr.gI();
		}
	}

	private static readonly IdListChatListener s_IdListChatListener = new IdListChatListener();

	public static void ShowInputKeepIds()
	{
		s_IdListChatListener.isSellList = false;
		ChatTextField.gI().strChat = "Nhập các ID vật phẩm giữ lại (cách nhau dấu ;)";
		ChatTextField.gI().tfChat.name = "ID giữ lại (VD: 521;1523;1524)";
		ChatTextField.gI().tfChat.setIputType(TField.INPUT_TYPE_ANY);
		ChatTextField.gI().startChat2(s_IdListChatListener, string.Empty);
		ChatTextField.gI().tfChat.setText(keepItemIdsRaw ?? string.Empty);
	}

	public static void ShowInputSellIds()
	{
		s_IdListChatListener.isSellList = true;
		ChatTextField.gI().strChat = "Nhập các ID vật phẩm ép bán (cách nhau dấu ;)";
		ChatTextField.gI().tfChat.name = "ID ép bán (để trống = tắt)";
		ChatTextField.gI().tfChat.setIputType(TField.INPUT_TYPE_ANY);
		ChatTextField.gI().startChat2(s_IdListChatListener, string.Empty);
		ChatTextField.gI().tfChat.setText(sellItemIdsRaw ?? string.Empty);
	}

	public static void CycleSellMaxLevel()
	{
		if (sellMaxLevel <= 5) sellMaxLevel = 8;
		else if (sellMaxLevel <= 8) sellMaxLevel = 10;
		else if (sellMaxLevel <= 10) sellMaxLevel = 12;
		else if (sellMaxLevel <= 12) sellMaxLevel = 13;
		else sellMaxLevel = 5;
		ModConfig.SaveConfig();
		GameScr.info1.addInfo("Bán trang bị tối đa Cấp <= " + sellMaxLevel, 0);
		SoundMn.gI().buttonClick();
	}

	public static SetActivatorState currentState = SetActivatorState.Idle;

	public static int savedFarmMapId = -1;
	public static int savedFarmZoneId = -1;
	public static int savedFarmX = -1;
	public static int savedFarmY = -1;

	public static bool isSellingJunk = false;
	private static int currentSellingBagIndex = -1;
	private static long lastSellActionTime = 0;
	private static int sellFailTimeoutCount = 0;
	private static int storeAttemptCount = 0;
	private static bool isBoxFullWarned = false;

	private static long stateTimer = 0;
	private static long lastZoneAttemptTime = 0;
	private static int zoneAttemptCount = 0;

	public static bool isBusy
	{
		get
		{
			return currentState != SetActivatorState.Idle && currentState != SetActivatorState.Farming;
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ SaveCurrentFarmPosition.
	/// </summary>
	public static void SaveCurrentFarmPosition()
	{
		Char me = Char.myCharz();
		if (me == null || !ModMenu.IsInGame())
		{
			return;
		}

		savedFarmMapId = TileMap.mapID;
		savedFarmZoneId = TileMap.zoneID;
		savedFarmX = me.cx;
		savedFarmY = me.cy;
		ModConfig.SaveConfig();

		GameScr.info1.addInfo("Úp Set KH: Đã lưu bãi " + ModNextMap.GetMapName(savedFarmMapId) + " [K." + savedFarmZoneId + "] (" + savedFarmX + ", " + savedFarmY + ")", 0);
		SoundMn.gI().buttonClick();
	}

	/// <summary>
	/// Dọn dẹp và đặt lại trạng thái ban đầu cho ResetPosition.
	/// </summary>
	public static void ResetPosition()
	{
		savedFarmMapId = -1;
		savedFarmZoneId = -1;
		savedFarmX = -1;
		savedFarmY = -1;
		currentState = SetActivatorState.Idle;
		isSellingJunk = false;
		isBoxFullWarned = false;
		ModConfig.SaveConfig();

		GameScr.info1.addInfo("Úp Set KH: Đã xóa bãi úp lưu!", 0);
		SoundMn.gI().buttonClick();
	}

	/// <summary>
	/// Bật hoặc tắt trạng thái kích hoạt của ToggleActive.
	/// </summary>
	public static void ToggleActive()
	{
		isActive = !isActive;
		if (isActive)
		{
			if (savedFarmMapId == -1)
			{
				SaveCurrentFarmPosition();
			}
			isBoxFullWarned = false;
			currentState = SetActivatorState.Farming;
			GameScr.info1.addInfo("Auto Úp Set Kích Hoạt: BẬT", 0);
		}
		else
		{
			currentState = SetActivatorState.Idle;
			isSellingJunk = false;
			if (ModNextMap.isNextMapActive)
			{
				ModNextMap.StopNextMap();
			}
			GameScr.info1.addInfo("Auto Úp Set Kích Hoạt: TẮT", 0);
		}
		ModConfig.SaveConfig();
		SoundMn.gI().buttonClick();
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ CycleMinStar.
	/// </summary>
	public static void CycleMinStar()
	{
		minStarToKeep++;
		if (minStarToKeep > 3)
		{
			minStarToKeep = 0;
		}
		ModConfig.SaveConfig();
		SoundMn.gI().buttonClick();
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của GetMinStarText.
	/// </summary>
	public static string GetMinStarText()
	{
		if (minStarToKeep == 0) return "Bán Hết Sao";
		return "Giữ >=" + minStarToKeep + " Sao";
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của GetFreeBagSlots.
	/// </summary>
	public static int GetFreeBagSlots()
	{
		try
		{
			Char me = Char.myCharz();
			if (me == null || me.arrItemBag == null) return 0;
			int free = 0;
			for (int i = 0; i < me.arrItemBag.Length; i++)
			{
				if (me.arrItemBag[i] == null)
				{
					free++;
				}
			}
			return free;
		}
		catch
		{
			return 0;
		}
	}

	/// <summary>
	/// Truy xuất số ô trống trong rương đồ tại nhà.
	/// </summary>
	public static int GetFreeBoxSlots()
	{
		try
		{
			Char me = Char.myCharz();
			if (me == null || me.arrItemBox == null) return -1;
			int free = 0;
			for (int i = 0; i < me.arrItemBox.Length; i++)
			{
				if (me.arrItemBox[i] == null)
				{
					free++;
				}
			}
			return free;
		}
		catch
		{
			return -1;
		}
	}

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của IsSetKichHoat.
	/// </summary>
	public static bool IsSetKichHoat(Item item)
	{
		if (item == null || item.itemOption == null) return false;
		for (int i = 0; i < item.itemOption.Length; i++)
		{
			ItemOption opt = item.itemOption[i];
			if (opt == null || opt.optionTemplate == null) continue;
			int optId = opt.optionTemplate.id;

			// Dải ID option của các bộ set kích hoạt chuẩn trong NRO (bao gồm cả 15 bộ SKH thế hệ mới)
			if ((optId >= 127 && optId <= 144) || (optId >= 210 && optId <= 225) ||
			    (optId >= 233 && optId <= 248) || (optId >= 254 && optId <= 257))
			{
				return true;
			}

			// So khớp tên option theo từ khóa set kích hoạt
			string optName = opt.optionTemplate.name;
			if (!string.IsNullOrEmpty(optName))
			{
				string lower = optName.ToLower();
				if (lower.Contains("kích hoạt") || lower.Contains("kich hoat") ||
					lower.Contains("kirin") || lower.Contains("songoku") ||
					lower.Contains("thiên xin hăng") || lower.Contains("thien xin hang") ||
					lower.Contains("cadic") || lower.Contains("nappa") ||
					lower.Contains("kakarot") || lower.Contains("pikkoro") ||
					lower.Contains("ốc tiêu") || lower.Contains("oc tieu") ||
					lower.Contains("dende") || lower.Contains("zelot") ||
					lower.Contains("songohan") || lower.StartsWith("set ") || lower.Contains("set:"))
				{
					return true;
				}
			}
		}
		return false;
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của GetItemStarCount.
	/// </summary>
	public static int GetItemStarCount(Item item)
	{
		if (item == null || item.itemOption == null) return 0;
		int stars = 0;
		for (int i = 0; i < item.itemOption.Length; i++)
		{
			ItemOption opt = item.itemOption[i];
			if (opt == null || opt.optionTemplate == null) continue;
			int optId = opt.optionTemplate.id;

			if ((optId == 102 || optId == 107) && opt.param > stars)
			{
				stars = opt.param;
			}
		}
		return stars;
	}

	public static bool HasRareSealOption(Item item)
	{
		if (item == null || item.itemOption == null) return false;
		for (int i = 0; i < item.itemOption.Length; i++)
		{
			ItemOption opt = item.itemOption[i];
			if (opt == null || opt.optionTemplate == null) continue;
			int optId = opt.optionTemplate.id;
			if (optId == 34 || optId == 35 || optId == 36)
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Chuỗi rút gọn chỉ số % nổi bật hiển thị trực tiếp trên ô trang bị theo chuẩn Mod Java.
	/// </summary>
	public static string GetCompactOptionTag(Item item)
	{
		if (item == null || item.itemOption == null) return string.Empty;
		string result = string.Empty;
		for (int i = 0; i < item.itemOption.Length; i++)
		{
			ItemOption opt = item.itemOption[i];
			if (opt == null || opt.optionTemplate == null) continue;
			string tag = null;
			switch (opt.optionTemplate.id)
			{
				case 9:
					if (item.template != null && item.template.type == 32)
					{
						tag = opt.param + "p";
					}
					break;
				case 50: tag = opt.param + "%SĐ"; break;
				case 77: tag = opt.param + "%HP"; break;
				case 80: tag = opt.param + "%HP/30s"; break;
				case 81: tag = opt.param + "%KI/30s"; break;
				case 94: tag = opt.param + "%AD"; break;
				case 95: tag = opt.param + "%HM"; break;
				case 96: tag = opt.param + "%HK"; break;
				case 97: tag = opt.param + "%PST"; break;
				case 100: tag = opt.param + "%Vàng"; break;
				case 101: tag = opt.param + "%TNSM"; break;
				case 103: tag = opt.param + "%KI"; break;
			}
			if (tag != null)
			{
				result = (result.Length == 0) ? tag : (result + " " + tag);
			}
		}
		if (IsSetKichHoat(item))
		{
			result = (result.Length == 0) ? "SKH" : ("SKH " + result);
		}
		return result;
	}

	/// <summary>
	/// Kiểm tra vật phẩm có nằm trong danh sách ID ngăn cách bởi dấu ; hoặc ,.
	/// </summary>
	public static bool IsInIdList(string rawList, int id)
	{
		if (string.IsNullOrEmpty(rawList)) return false;
		string[] parts = rawList.Split(new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
		for (int i = 0; i < parts.Length; i++)
		{
			int parsed;
			if (int.TryParse(parts[i].Trim(), out parsed) && parsed == id)
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Kiểm tra trang bị có phải là Đồ Thần Linh, Hủy Diệt hoặc Thiên Sứ (level >= 13) hay không.
	/// </summary>
	public static bool IsGodItem(Item item)
	{
		if (item == null || item.template == null) return false;
		if (item.template.level >= 13) return true;
		string name = item.template.name;
		if (!string.IsNullOrEmpty(name))
		{
			string lower = name.ToLower();
			if (lower.Contains("thần ") || lower.EndsWith(" thần") || lower.Contains("thần linh") || lower.Contains("than linh") ||
			    lower.Contains("hủy diệt") || lower.Contains("huy diet") ||
			    lower.Contains("thiên sứ") || lower.Contains("thien su"))
			{
				return true;
			}
		}
		return false;
	}

	public static void CycleMinEmptyBagSlots()
	{
		if (minEmptyBagSlots < 0) minEmptyBagSlots = 0;
		else if (minEmptyBagSlots == 0) minEmptyBagSlots = 1;
		else if (minEmptyBagSlots == 1) minEmptyBagSlots = 2;
		else if (minEmptyBagSlots == 2) minEmptyBagSlots = 3;
		else if (minEmptyBagSlots == 3) minEmptyBagSlots = 5;
		else minEmptyBagSlots = 0;
		ModConfig.SaveConfig();
		SoundMn.gI().buttonClick();
	}

	/// <summary>
	/// Kiểm tra xem trang bị có phải là đồ SKH, Thần/HĐ/TS hoặc đồ Sao cần cất vào rương hay không.
	/// </summary>
	public static bool IsValuableStoreItem(Item item)
	{
		if (item == null || item.template == null) return false;
		int type = item.template.type;
		if (type < 0 || type > 4) return false;
		if (IsInIdList(sellItemIdsRaw, item.template.id) && !item.isLock && item.upgrade <= 0) return false;
		if (IsInIdList(keepItemIdsRaw, item.template.id)) return true;
		if (!sellSkhItems && (IsSetKichHoat(item) || HasRareSealOption(item))) return true;
		if (!sellGodItems && (IsGodItem(item) || item.template.level > sellMaxLevel)) return true;
		if (minStarToKeep > 0 && GetItemStarCount(item) >= minStarToKeep) return true;
		return false;
	}

	/// <summary>
	/// Kiểm tra trong hành trang có trang bị SKH/Sao cần cất vào rương hay không.
	/// </summary>
	public static bool HasValuableStoreItemInBag(Char me)
	{
		if (me == null || me.arrItemBag == null) return false;
		for (int i = 0; i < me.arrItemBag.Length; i++)
		{
			if (me.arrItemBag[i] != null && IsValuableStoreItem(me.arrItemBag[i]))
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của IsImportantItem.
	/// </summary>
	public static bool IsImportantItem(Item item)
	{
		if (item == null || item.template == null) return true;
		if (item.upgrade > 0) return true;
		if (item.isLock) return true;

		int id = item.template.id;
		if (IsInIdList(keepItemIdsRaw, id)) return true;
		if (IsInIdList(sellItemIdsRaw, id)) return false;

		int type = item.template.type;
		// CHỈ cho phép bán nếu là trang bị thường (0: Áo, 1: Quần, 2: Găng, 3: Giày, 4: Rada)
		if (type < 0 || type > 4)
		{
			return true;
		}

		if (!sellGodItems && IsGodItem(item)) return true;
		if (!sellGodItems && item.template.level > sellMaxLevel) return true;

		if (id == 457) return true; // Thỏi vàng
		if (id >= 14 && id <= 20) return true; // Ngọc rồng
		if (id >= 13 && id <= 20) return true; // Đậu thần
		if (id >= 220 && id <= 226) return true; // Đá nâng cấp
		if (id >= 441 && id <= 447) return true; // Đá pha lê
		if (id == 193 || id == 194) return true; // Capsule
		if (id >= 380 && id <= 385) return true; // Potion buff
		if (id == 921 || id == 454 || id == 933) return true; // Bông tai Porata

		if (item.isHaveOption(30)) return true; // Không thể giao dịch
		if (item.isHaveOption(73) || item.isHaveOption(93)) return true; // Hạn sử dụng

		return false;
	}

	private static long lastFullNonJunkWarnTime = 0;

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của IsJunkItem.
	/// </summary>
	public static bool IsJunkItem(Item item)
	{
		if (item == null || item.template == null) return false;
		if (IsInIdList(sellItemIdsRaw, item.template.id) && !item.isLock && item.upgrade <= 0) return true;
		if (IsImportantItem(item)) return false;
		if (!sellSkhItems && (IsSetKichHoat(item) || HasRareSealOption(item))) return false;
		if (minStarToKeep > 0 && GetItemStarCount(item) >= minStarToKeep) return false;

		return true;
	}

	/// <summary>
	/// Kiểm tra xem trong hành trang có ít nhất một vật phẩm rác có thể bán được hay không.
	/// </summary>
	public static bool HasSellableJunkInBag(Char me)
	{
		if (me == null || me.arrItemBag == null) return false;
		for (int i = 0; i < me.arrItemBag.Length; i++)
		{
			if (me.arrItemBag[i] != null && IsJunkItem(me.arrItemBag[i]))
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của GetTargetSpaceshipMapId.
	/// </summary>
	public static int GetTargetSpaceshipMapId()
	{
		Char me = Char.myCharz();
		if (me == null) return 24;

		// Map 24: Trạm tàu Trái Đất, 25: Trạm tàu Namếc, 26: Trạm tàu Xayda
		int currentMap = TileMap.mapID;
		if (currentMap >= 8 && currentMap <= 13) return 25;
		if (currentMap >= 15 && currentMap <= 20) return 26;
		if (currentMap >= 1 && currentMap <= 6) return 24;

		if (me.cgender == 1) return 25;
		if (me.cgender == 2) return 26;
		return 24;
	}

	/// <summary>
	/// Truy xuất ID map Nhà theo hành tinh nhân vật (21: Trái Đất, 22: Namếc, 23: Xayda).
	/// </summary>
	public static int GetHomeMapId()
	{
		Char me = Char.myCharz();
		if (me == null) return 21;
		return 21 + me.cgender;
	}

	/// <summary>
	/// Tìm NPC Rương Đồ (templateId = 3) tại map Nhà.
	/// </summary>
	public static Npc FindHomeBoxNpc()
	{
		if (GameScr.vNpc == null) return null;
		for (int i = 0; i < GameScr.vNpc.size(); i++)
		{
			Npc npc = (Npc)GameScr.vNpc.elementAt(i);
			if (npc == null || npc.template == null) continue;
			if (npc.template.npcTemplateId == 3)
			{
				return npc;
			}
			string name = (npc.template.name ?? string.Empty).ToLower();
			if (name.Contains("rương") || name.Contains("ruong"))
			{
				return npc;
			}
		}
		return null;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ FindUronNpc.
	/// </summary>
	public static Npc FindUronNpc()
	{
		if (GameScr.vNpc == null) return null;
		for (int i = 0; i < GameScr.vNpc.size(); i++)
		{
			Npc npc = (Npc)GameScr.vNpc.elementAt(i);
			if (npc == null || npc.template == null) continue;
			int tid = npc.template.npcTemplateId;
			if (tid == 16 || tid == 7 || tid == 8 || tid == 9)
			{
				return npc;
			}
		}
		for (int i = 0; i < GameScr.vNpc.size(); i++)
		{
			Npc npc = (Npc)GameScr.vNpc.elementAt(i);
			if (npc == null || npc.template == null || npc.template.name == null) continue;
			string name = npc.template.name.ToLower();
			if (name.Contains("uron") || name.Contains("urôn") || name.Contains("u-rôn") || name.Contains("oolong"))
			{
				return npc;
			}
		}
		for (int i = 0; i < GameScr.vNpc.size(); i++)
		{
			Npc npc = (Npc)GameScr.vNpc.elementAt(i);
			if (npc == null || npc.template == null) continue;
			if (npc.template.npcTemplateId == 19 || npc.template.npcTemplateId == 18)
			{
				return npc;
			}
		}
		return null;
	}

	private static Npc FindSantaNpc()
	{
		if (GameScr.vNpc == null) return null;
		for (int i = 0; i < GameScr.vNpc.size(); i++)
		{
			Npc npc = (Npc)GameScr.vNpc.elementAt(i);
			if (npc == null || npc.template == null) continue;
			if (npc.template.npcTemplateId == 39)
			{
				return npc;
			}
			string name = (npc.template.name ?? string.Empty).ToLower();
			if (name.Contains("santa") || name.Contains("satan") || name.Contains("héc-quyn"))
			{
				return npc;
			}
		}
		return null;
	}

	private static bool IsNonFarmServiceMap(int mapId)
	{
		return mapId == 5 || (mapId >= 21 && mapId <= 26);
	}

	private static void CaptureCurrentFarmAndPauseAutos(Char me, bool preserveExistingSpot = false)
	{
		if (!isBusy)
		{
			s_TripWasTanSat = ModTanSat.autoTanSat;
			s_TripWasGoBack = ModGoBack.isGoBackActive;
			ModTanSat.autoTanSat = false;
			ModGoBack.isGoBackActive = false;
		}
		if (me != null && TileMap.mapID >= 0 && (!preserveExistingSpot || savedFarmMapId < 0) && !IsNonFarmServiceMap(TileMap.mapID))
		{
			savedFarmMapId = TileMap.mapID;
			savedFarmZoneId = TileMap.zoneID;
			savedFarmX = me.cx;
			savedFarmY = me.cy;
			ModConfig.SaveConfig();
		}
	}

	private static void RestorePausedAutosAfterTrip()
	{
		if (isActive || s_TripWasTanSat)
		{
			ModTanSat.autoTanSat = true;
		}
		if (s_TripWasGoBack)
		{
			ModGoBack.isGoBackActive = true;
		}
		s_TripWasTanSat = false;
		s_TripWasGoBack = false;
	}

	public static void StartBuyTdltNow()
	{
		Char me = Char.myCharz();
		if (me == null || !ModMenu.IsInGame() || me.isDie) return;
		if ((me.luong + me.luongKhoa) < 1)
		{
			GameScr.info1.addInfo("Không đủ Ngọc/Hồng Ngọc để mua TĐLT!", 0);
			return;
		}
		CaptureCurrentFarmAndPauseAutos(me);
		currentState = SetActivatorState.MovingToSanta;
		s_SantaBuyStep = 0;
		s_SantaRetryCount = 0;
		stateTimer = mSystem.currentTimeMillis();
		GameScr.info1.addInfo("Đang sang Đảo Kamê gặp Santa mua TĐLT...", 0);
		if (TileMap.mapID != 5)
		{
			ModNextMap.StartNextMap(5);
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ StartGoSellJunkNow.
	/// </summary>
	public static void StartGoSellJunkNow(bool preserveSavedSpot = false)
	{
		Char me = Char.myCharz();
		if (me == null || !ModMenu.IsInGame()) return;

		CaptureCurrentFarmAndPauseAutos(me, preserveExistingSpot: preserveSavedSpot);

		int targetMap = GetTargetSpaceshipMapId();
		currentState = SetActivatorState.MovingToUron;
		stateTimer = mSystem.currentTimeMillis();
		GameScr.info1.addInfo("Bắt đầu di chuyển đến Trạm Tàu Vũ Trụ gặp Urôn để bán đồ...", 0);
		if (TileMap.mapID != targetMap)
		{
			ModNextMap.StartNextMap(targetMap);
		}
	}

	/// <summary>
	/// Bắt đầu về Nhà để cất trang bị SKH / Đồ Sao vào Rương Đồ.
	/// </summary>
	public static void StartGoStoreBoxNow()
	{
		Char me = Char.myCharz();
		if (me == null || !ModMenu.IsInGame()) return;

		CaptureCurrentFarmAndPauseAutos(me);

		int homeMap = GetHomeMapId();
		currentState = SetActivatorState.MovingToHomeBox;
		stateTimer = mSystem.currentTimeMillis();
		GameScr.info1.addInfo("Bắt đầu về Nhà cất đồ SKH/Đồ Sao vào Rương...", 0);
		if (TileMap.mapID != homeMap)
		{
			ModNextMap.StartNextMap(homeMap);
		}
	}

	/// <summary>
	/// Bộ xử lý sự kiện tiếp nhận và phản hồi OnSaleRequestReceived.
	/// </summary>
	public static void OnSaleRequestReceived(sbyte type, short id)
	{
		if (!isSellingJunk) return;

		try
		{
			// Tự động gửi packet xác nhận bán đồ
			Service.gI().saleItem(1, type, id);
			lastSellActionTime = mSystem.currentTimeMillis();
			sellFailTimeoutCount = 0;
			GameCanvas.endDlg();
		}
		catch
		{
		}
	}

	/// <summary>
	/// Cập nhật trạng thái logic của đối tượng theo từng chu kỳ khung hình.
	/// </summary>
	public static void Update()
	{
		if (!ModMenu.IsInGame() || ModAutoBuyBua.isBusy) return;
		Char me = Char.myCharz();
		if (me == null || me.isDie) return;

		long now = mSystem.currentTimeMillis();
		UpdateAutoTdlt(me, now);

		switch (currentState)
		{
			case SetActivatorState.Idle:
				if (isActive)
				{
					currentState = SetActivatorState.Farming;
					break;
				}
				if ((autoStoreToBox || autoSellJunk) && !ModNextMap.isNextMapActive && !ModGoBack.isReturning && !Char.isLoadingMap && !Char.ischangingMap && !IsNonFarmServiceMap(TileMap.mapID))
				{
					CheckFullBagAndDispatch(me, now);
				}
				break;

			case SetActivatorState.Farming:
				if (!isActive)
				{
					currentState = SetActivatorState.Idle;
					break;
				}

				ModDropRate.ScanBagForSpecialItems(me);
				if (!ModNextMap.isNextMapActive && !ModGoBack.isReturning && !Char.isLoadingMap && !Char.ischangingMap && !IsNonFarmServiceMap(TileMap.mapID))
				{
					CheckFullBagAndDispatch(me, now);
				}
				break;

			case SetActivatorState.MovingToUron:
				int targetStation = GetTargetSpaceshipMapId();
				if (TileMap.mapID == targetStation && !Char.isLoadingMap && !Char.ischangingMap)
				{
					if (ModNextMap.isNextMapActive)
					{
						ModNextMap.StopNextMap();
					}
					currentState = SetActivatorState.InteractingUron;
					stateTimer = now;
				}
				else if (!ModNextMap.isNextMapActive && !Char.isLoadingMap && !Char.ischangingMap)
				{
					// Nếu chưa đến nơi mà nextmap dừng, kích hoạt lại
					if (now - stateTimer > 3000)
					{
						stateTimer = now;
						ModNextMap.StartNextMap(targetStation);
					}
				}
				break;

			case SetActivatorState.InteractingUron:
				if (Char.isLoadingMap || Char.ischangingMap) break;

				Npc uron = FindUronNpc();
				if (uron != null)
				{
					int dist = Res.distance(me.cx, me.cy, uron.cx, uron.cy);
					if (dist > 30)
					{
						ModTeleport.TeleportTo(uron.cx, uron.cy);
					}
					me.focusManualTo(uron);

					if (now - stateTimer > 900)
					{
						stateTimer = now;
						int npcTmplId = (uron.template != null) ? uron.template.npcTemplateId : uron.npcId;
						Service.gI().openMenu(npcTmplId);
						Service.gI().confirmMenu((short)npcTmplId, 0);
						if (GameCanvas.menu != null)
						{
							GameCanvas.menu.showMenu = false;
						}

						currentState = SetActivatorState.SellingJunk;
						isSellingJunk = true;
						lastSellActionTime = now + 350;
						currentSellingBagIndex = -1;
						sellFailTimeoutCount = 0;
					}
				}
				else
				{
					// Không tìm thấy Urôn trong map, chờ hoặc thử tìm lại
					if (now - stateTimer > 5000)
					{
						GameScr.info1.addInfo("Không tìm thấy NPC Urôn tại map này! Quay lại bãi úp...", 0);
						currentState = SetActivatorState.ReturningToFarm;
						stateTimer = now;
						ModNextMap.StartNextMap(savedFarmMapId);
					}
				}
				break;

			case SetActivatorState.SellingJunk:
				if (now - lastSellActionTime < 300)
				{
					break;
				}

				// Tìm món đồ rác tiếp theo trong hành trang
				int junkIndex = -1;
				Item junkItem = null;
				if (me.arrItemBag != null)
				{
					for (int i = 0; i < me.arrItemBag.Length; i++)
					{
						Item item = me.arrItemBag[i];
						if (item != null && IsJunkItem(item))
						{
							junkIndex = i;
							junkItem = item;
							break;
						}
					}
				}

				if (junkIndex != -1 && junkItem != null)
				{
					currentSellingBagIndex = junkIndex;
					lastSellActionTime = now;
					Service.gI().saleItem(0, 1, (short)junkIndex);
					sellFailTimeoutCount++;

					if (sellFailTimeoutCount > 15)
					{
						// Nếu kẹt bán quá nhiều lần, bỏ qua kết thúc
						GameScr.info1.addInfo("Bán đồ hoàn tất hoặc kết thúc giao dịch.", 0);
						FinishSellingAndReturn();
					}
				}
				else
				{
					// Đã bán hết đồ rác!
					GameScr.info1.addInfo("Đã bán hết đồ rác!", 0);
					FinishSellingAndReturn();
				}
				break;

			case SetActivatorState.MovingToHomeBox:
				int homeMap = GetHomeMapId();
				if (TileMap.mapID == homeMap && !Char.isLoadingMap && !Char.ischangingMap)
				{
					if (ModNextMap.isNextMapActive)
					{
						ModNextMap.StopNextMap();
					}
					currentState = SetActivatorState.InteractingBox;
					stateTimer = now;
				}
				else if (!ModNextMap.isNextMapActive && !Char.isLoadingMap && !Char.ischangingMap)
				{
					if (now - stateTimer > 3000)
					{
						stateTimer = now;
						ModNextMap.StartNextMap(homeMap);
					}
				}
				break;

			case SetActivatorState.InteractingBox:
				if (Char.isLoadingMap || Char.ischangingMap) break;

				Npc boxNpc = FindHomeBoxNpc();
				if (boxNpc != null)
				{
					int distBox = Res.distance(me.cx, me.cy, boxNpc.cx, boxNpc.cy);
					if (distBox > 30)
					{
						ModTeleport.TeleportTo(boxNpc.cx, boxNpc.cy);
					}
					me.focusManualTo(boxNpc);

					if (now - stateTimer > 900)
					{
						stateTimer = now;
						int boxTmplId = (boxNpc.template != null) ? boxNpc.template.npcTemplateId : 3;
						Service.gI().openMenu(boxTmplId);

						currentState = SetActivatorState.StoringToBox;
						lastSellActionTime = now;
						storeAttemptCount = 0;
					}
				}
				else
				{
					if (now - stateTimer > 5000)
					{
						GameScr.info1.addInfo("Không tìm thấy Rương Đồ tại nhà! Quay lại bãi úp...", 0);
						ReturnToSavedFarm();
					}
				}
				break;

			case SetActivatorState.StoringToBox:
				if (now - lastSellActionTime < 350)
				{
					break;
				}

				int freeBox = GetFreeBoxSlots();
				if (freeBox == 0)
				{
					isBoxFullWarned = true;
					GameScr.info1.addInfo("Rương đồ ở nhà đã đầy!", 0);
					FinishStoringAndNext(me);
					break;
				}

				int storeIdx = -1;
				if (me.arrItemBag != null)
				{
					for (int i = 0; i < me.arrItemBag.Length; i++)
					{
						Item item = me.arrItemBag[i];
						if (item != null && IsValuableStoreItem(item))
						{
							storeIdx = i;
							break;
						}
					}
				}

				if (storeIdx != -1 && storeAttemptCount < 25)
				{
					lastSellActionTime = now;
					storeAttemptCount++;
					Service.gI().getItem(1, (sbyte)storeIdx);
				}
				else
				{
					GameScr.info1.addInfo("Đã cất đồ giá trị vào rương!", 0);
					FinishStoringAndNext(me);
				}
				break;

			case SetActivatorState.MovingToSanta:
				if (TileMap.mapID == 5 && !Char.isLoadingMap && !Char.ischangingMap)
				{
					if (ModNextMap.isNextMapActive)
					{
						ModNextMap.StopNextMap();
					}
					currentState = SetActivatorState.BuyingTdltAtSanta;
					s_SantaBuyStep = 0;
					s_SantaRetryCount = 0;
					stateTimer = now;
				}
				else if (!ModNextMap.isNextMapActive && !Char.isLoadingMap && !Char.ischangingMap && now - stateTimer > 3000)
				{
					stateTimer = now;
					ModNextMap.StartNextMap(5);
				}
				break;

			case SetActivatorState.BuyingTdltAtSanta:
				if (Char.isLoadingMap || Char.ischangingMap) break;
				Npc santa = FindSantaNpc();
				if (santa == null)
				{
					if (now - stateTimer > 5000)
					{
						GameScr.info1.addInfo("Không tìm thấy NPC Santa tại Đảo Kamê!", 0);
						ReturnToSavedFarm();
					}
					break;
				}

				int santaY = santa.cy - 3;
				if (Res.distance(me.cx, me.cy, santa.cx, santaY) > 28)
				{
					ModTeleport.TeleportTo(santa.cx, santaY);
				}
				me.focusManualTo(santa);
				int santaId = (santa.template != null) ? santa.template.npcTemplateId : 39;

				if (s_SantaBuyStep == 0 && now - stateTimer >= 550)
				{
					Service.gI().openMenu(santaId);
					s_SantaBuyStep = 1;
					stateTimer = now;
				}
				else if (s_SantaBuyStep == 1 && now - stateTimer >= 550)
				{
					int shopIdx = FindSantaShopMenuIndex();
					Service.gI().confirmMenu((short)santaId, (sbyte)(shopIdx >= 0 ? shopIdx : 0));
					if (GameCanvas.menu != null)
					{
						GameCanvas.menu.showMenu = false;
					}
					s_SantaBuyStep = 2;
					stateTimer = now;
				}
				else if (s_SantaBuyStep == 2 && now - stateTimer >= 600)
				{
					if (GameCanvas.menu != null && GameCanvas.menu.showMenu)
					{
						int subIdx = FindSantaShopMenuIndex();
						Service.gI().confirmMenu((short)santaId, (sbyte)(subIdx >= 0 ? subIdx : 0));
						GameCanvas.menu.showMenu = false;
						stateTimer = now;
						break;
					}

					GetBagTdltInfo(me, out _, out bool hasBase521);
					int totalGems = me.luong + me.luongKhoa;
					int[] prefIds = hasBase521 ? new int[] { 1524, 1523, 521 } : new int[] { 521, 1524, 1523 };
					int tdltBuyId = hasBase521 ? ((totalGems >= 22) ? 1524 : ((totalGems >= 9) ? 1523 : 521)) : 521;
					sbyte tdltBuyType = 1;

					if (me.arrItemShop != null)
					{
						bool matchedInShop = false;
						for (int p = 0; p < prefIds.Length && !matchedInShop; p++)
						{
							for (int t = 0; t < me.arrItemShop.Length && !matchedInShop; t++)
							{
								if (me.arrItemShop[t] == null) continue;
								for (int k = 0; k < me.arrItemShop[t].Length; k++)
								{
									Item sIt = me.arrItemShop[t][k];
									if (sIt != null && sIt.template != null && sIt.template.id == prefIds[p])
									{
										if (sIt.isBuySpec && sIt.buySpec > 0) continue;
										if (sIt.buyGold > 0 && totalGems >= sIt.buyGold)
										{
											tdltBuyId = prefIds[p];
											tdltBuyType = 1;
											matchedInShop = true;
											break;
										}
										if (sIt.buyGold <= 0 && sIt.buyCoin > 0 && me.xu >= sIt.buyCoin)
										{
											tdltBuyId = prefIds[p];
											tdltBuyType = 0;
											matchedInShop = true;
											break;
										}
									}
								}
							}
						}
					}

					Service.gI().buyItem(tdltBuyType, tdltBuyId, 1);
					s_SantaRetryCount++;
					s_SantaBuyStep = 3;
					stateTimer = now;
				}
				else if (s_SantaBuyStep == 3 && now - stateTimer >= 750)
				{
					int bagMins = GetBagTdltInfo(me, out int usableIdx, out _);
					if (usableIdx < 0 && s_SantaRetryCount < 3 && (me.luong + me.luongKhoa) >= 1)
					{
						s_SantaBuyStep = 2;
						stateTimer = now;
						break;
					}
					if (usableIdx >= 0 && (autoUseTdlt || !ItemTime.isExistItem(4387)))
					{
						Service.gI().useItem(0, 1, (sbyte)usableIdx, -1);
					}
					GameScr.info1.addInfo((bagMins > 0 || usableIdx >= 0) ? "Đã mua & gia hạn TĐLT tại Santa!" : "Đã hoàn tất giao dịch Santa!", 0);
					ReturnToSavedFarm();
				}
				break;

			case SetActivatorState.ReturningToFarm:
				if (savedFarmMapId == -1)
				{
					RestorePausedAutosAfterTrip();
					currentState = isActive ? SetActivatorState.Farming : SetActivatorState.Idle;
					break;
				}

				if (TileMap.mapID == savedFarmMapId && !Char.isLoadingMap && !Char.ischangingMap)
				{
					if (ModNextMap.isNextMapActive)
					{
						ModNextMap.StopNextMap();
					}

					if (TileMap.zoneID != savedFarmZoneId && savedFarmZoneId != -1)
					{
						currentState = SetActivatorState.SwitchingZone;
						stateTimer = now;
						lastZoneAttemptTime = now;
						zoneAttemptCount = 0;
						Service.gI().requestChangeZone(savedFarmZoneId, -1);
						GameScr.info1.addInfo("Đã về map bãi úp. Đang đổi sang Khu " + savedFarmZoneId + "...", 0);
					}
					else
					{
						currentState = SetActivatorState.TeleportingToSpot;
						stateTimer = now;
					}
				}
				else if (!ModNextMap.isNextMapActive && !Char.isLoadingMap && !Char.ischangingMap)
				{
					if (now - stateTimer > 3000)
					{
						stateTimer = now;
						ModNextMap.StartNextMap(savedFarmMapId);
					}
				}
				break;

			case SetActivatorState.SwitchingZone:
				if (Char.isLoadingMap || Char.ischangingMap) break;

				if (TileMap.zoneID == savedFarmZoneId)
				{
					currentState = SetActivatorState.TeleportingToSpot;
					stateTimer = now;
					break;
				}

				if (now - lastZoneAttemptTime > 2500)
				{
					lastZoneAttemptTime = now;
					zoneAttemptCount++;
					if (zoneAttemptCount <= 3)
					{
						Service.gI().requestChangeZone(savedFarmZoneId, -1);
					}
					else
					{
						GameScr.info1.addInfo("Khu " + savedFarmZoneId + " có thể đã đầy! Tiếp tục ở khu hiện tại.", 0);
						currentState = SetActivatorState.TeleportingToSpot;
						stateTimer = now;
					}
				}
				break;

			case SetActivatorState.TeleportingToSpot:
				if (Char.isLoadingMap || Char.ischangingMap) break;

				if (savedFarmX != -1 && savedFarmY != -1)
				{
					ModTeleport.TeleportTo(savedFarmX, savedFarmY);
					GameScr.info1.addInfo("Đã về đúng vị trí bãi úp!", 0);
					SoundMn.gI().buttonClick();
				}

				RestorePausedAutosAfterTrip();
				currentState = isActive ? SetActivatorState.Farming : SetActivatorState.Idle;
				break;
		}
	}

	private static void CheckFullBagAndDispatch(Char me, long now)
	{
		if (GetFreeBagSlots() > minEmptyBagSlots) return;

		if (autoStoreToBox && !isBoxFullWarned && HasValuableStoreItemInBag(me))
		{
			GameScr.info1.addInfo("Hành trang đầy! Ưu tiên về Nhà cất đồ SKH/Sao/Thần vào Rương trước...", 0);
			StartGoStoreBoxNow();
		}
		else if (autoSellJunk && HasSellableJunkInBag(me))
		{
			GameScr.info1.addInfo("Hành trang đầy! Tự động đi bán đồ rác tại Urôn...", 0);
			StartGoSellJunkNow();
		}
		else if (now - lastFullNonJunkWarnTime > 30000)
		{
			lastFullNonJunkWarnTime = now;
			GameScr.info1.addInfo("Hành trang đã đầy vật phẩm giá trị (không còn đồ rác để bán)!", 0);
		}
	}

	private static void FinishStoringAndNext(Char me)
	{
		if (GameCanvas.panel != null && GameCanvas.panel.isShow)
		{
			GameCanvas.panel.hide();
		}
		if (autoSellJunk && me != null && HasSellableJunkInBag(me))
		{
			StartGoSellJunkNow(preserveSavedSpot: true);
			return;
		}
		ReturnToSavedFarm();
	}

	private static int FindSantaShopMenuIndex()
	{
		if (GameCanvas.menu == null || !GameCanvas.menu.showMenu || GameCanvas.menu.menuItems == null)
		{
			return 0;
		}
		for (int i = 0; i < GameCanvas.menu.menuItems.size(); i++)
		{
			Command cmd = (Command)GameCanvas.menu.menuItems.elementAt(i);
			if (cmd != null && !string.IsNullOrEmpty(cmd.caption))
			{
				string cap = ModNextMapData.RemoveAccents(cmd.caption).ToLower();
				if (cap.Contains("cua hang") || cap.Contains("shop") || cap.Contains("tiem"))
				{
					return i;
				}
			}
		}
		return 0;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ FinishSellingAndReturn.
	/// </summary>
	private static void FinishSellingAndReturn()
	{
		isSellingJunk = false;
		GameCanvas.endDlg();

		Char me = Char.myCharz();
		if (autoStoreToBox && !isBoxFullWarned && me != null && HasValuableStoreItemInBag(me))
		{
			StartGoStoreBoxNow();
			return;
		}

		ReturnToSavedFarm();
	}

	private static void ReturnToSavedFarm()
	{
		isSellingJunk = false;
		GameCanvas.endDlg();
		if (GameCanvas.panel != null && GameCanvas.panel.isShow)
		{
			GameCanvas.panel.hide();
		}
		currentState = SetActivatorState.ReturningToFarm;
		stateTimer = mSystem.currentTimeMillis();
		if (savedFarmMapId >= 0)
		{
			ModNextMap.StartNextMap(savedFarmMapId);
		}
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của GetStatusText.
	/// </summary>
	public static string GetStatusText()
	{
		if (!isActive && !isBusy) return "Đang TẮT";
		switch (currentState)
		{
			case SetActivatorState.Farming:
				return "Đang Úp Quái (Trống: " + GetFreeBagSlots() + " ô)";
			case SetActivatorState.BagFull_SavingLocation:
				return "Full Hành Trang - Đang lưu vị trí";
			case SetActivatorState.MovingToUron:
				return "Đang đến Trạm Tàu gặp Urôn";
			case SetActivatorState.InteractingUron:
				return "Đang mở cửa hàng Urôn";
			case SetActivatorState.SellingJunk:
				return "Đang bán đồ rác tại Urôn";
			case SetActivatorState.MovingToHomeBox:
				return "Đang về Nhà mở Rương Đồ";
			case SetActivatorState.InteractingBox:
				return "Đang mở Rương Đồ tại nhà";
			case SetActivatorState.StoringToBox:
				return "Đang cất đồ SKH/Sao vào Rương";
			case SetActivatorState.MovingToSanta:
				return "Đang sang Đảo Kamê gặp Santa";
			case SetActivatorState.BuyingTdltAtSanta:
				return "Đang mua TĐLT tại Santa";
			case SetActivatorState.ReturningToFarm:
				return "Đang quay lại bản đồ bãi úp";
			case SetActivatorState.SwitchingZone:
				return "Đang đổi lại Khu " + savedFarmZoneId;
			case SetActivatorState.TeleportingToSpot:
				return "Đang teleport về đúng bãi úp";
			default:
				return "Đang chờ";
		}
	}

	public static int GetBagTdltInfo(Char me, out int usableBagIdx, out bool hasBase521)
	{
		usableBagIdx = -1;
		hasBase521 = false;
		if (me == null || me.arrItemBag == null) return 0;

		int maxMins = 0;
		for (int i = 0; i < me.arrItemBag.Length; i++)
		{
			Item item = me.arrItemBag[i];
			if (item == null || item.template == null) continue;
			int tid = item.template.id;
			if (tid == 521) hasBase521 = true;
			if (tid != 521 && tid != 1523 && tid != 1524) continue;

			bool expired = false;
			int itemMins = (tid == 1524) ? 1440 : 60;
			if (item.itemOption != null)
			{
				for (int j = 0; j < item.itemOption.Length; j++)
				{
					ItemOption opt = item.itemOption[j];
					if (opt == null || opt.optionTemplate == null) continue;
					int oid = opt.optionTemplate.id;
					if (oid == 9)
					{
						itemMins = opt.param;
						if (opt.param <= 0)
						{
							expired = true;
							break;
						}
					}
					else if ((oid == 1 || oid == 63 || oid == 64 || oid == 65 || oid == 73 || oid == 93) && opt.param <= 0)
					{
						expired = true;
						break;
					}
					else if (!string.IsNullOrEmpty(opt.optionTemplate.name))
					{
						string oname = ModNextMapData.RemoveAccents(opt.optionTemplate.name).ToLower();
						if (oname.Contains("phut") && opt.param <= 0)
						{
							expired = true;
							break;
						}
						if (oname.Contains("phut") && opt.param > 0)
						{
							itemMins = opt.param;
						}
					}
				}
			}

			if (!expired)
			{
				if (usableBagIdx < 0 || tid == 1524 || tid == 1523)
				{
					usableBagIdx = i;
				}
				if (itemMins > maxMins)
				{
					maxMins = itemMins;
				}
			}
		}
		return maxMins;
	}

	public static bool TryUseUnexpiredTdltInBag(Char me)
	{
		GetBagTdltInfo(me, out int usableIdx, out _);
		if (usableIdx >= 0)
		{
			Service.gI().useItem(0, 1, (sbyte)usableIdx, -1);
			return true;
		}
		return false;
	}

	private static void UpdateAutoTdlt(Char me, long now)
	{
		if ((!autoUseTdlt && !autoBuyTdlt) || me == null || me.isDie) return;
		if (isBusy || ModNextMap.isNextMapActive || ModGoBack.isReturning || Char.isLoadingMap || Char.ischangingMap) return;
		if (now - lastTdltActionTime < 3500) return;

		ItemTime activeTdlt = ItemTime.getItemById(4387);
		bool isTdltActive = activeTdlt != null;
		int activeMins = isTdltActive ? activeTdlt.minute : 0;
		int bagMins = GetBagTdltInfo(me, out int usableBagIdx, out _);

		if (!isTdltActive && autoUseTdlt && usableBagIdx >= 0)
		{
			lastTdltActionTime = now;
			Service.gI().useItem(0, 1, (sbyte)usableBagIdx, -1);
			return;
		}

		if (autoBuyTdlt && (me.luong + me.luongKhoa) >= 1)
		{
			bool needBuy = (!isTdltActive && (usableBagIdx < 0 || (!autoUseTdlt && bagMins < 10))) ||
			               (isTdltActive && activeMins >= 0 && activeMins < 10 && bagMins < 10);
			if (needBuy)
			{
				lastTdltActionTime = now;
				StartBuyTdltNow();
			}
		}
	}

	private static readonly string[] s_SkhSetNames = new string[]
	{
		"Songoku",
		"Thiên Xin Hăng",
		"Krillin",
		"Kaioshin",
		"Gohan",
		"Piccolo",
		"Ốc Tiêu",
		"Nail",
		"Daimao",
		"Nail M",
		"Champa",
		"Kakarot",
		"Cađíc",
		"Nappa",
		"Cađíc M",
		"SKH Khác"
	};

	private static int ClassifySkhSetIndex(Item item)
	{
		if (item == null || item.itemOption == null) return -1;
		for (int i = 0; i < item.itemOption.Length; i++)
		{
			ItemOption opt = item.itemOption[i];
			if (opt == null || opt.optionTemplate == null || string.IsNullOrEmpty(opt.optionTemplate.name)) continue;
			string low = opt.optionTemplate.name.ToLower();
			if (low.Contains("ốc tiêu") || low.Contains("oc tieu")) return 6;
		}

		for (int i = 0; i < item.itemOption.Length; i++)
		{
			ItemOption opt = item.itemOption[i];
			if (opt == null || opt.optionTemplate == null) continue;
			switch (opt.optionTemplate.id)
			{
				case 129: case 141: return 0; // Songoku
				case 127: case 139: return 1; // Thiên Xin Hăng
				case 128: case 140: return 2; // Krillin
				case 245: case 246: case 247: case 248: return 3; // Kaioshin
				case 233: case 234: return 4; // Gohan
				case 130: case 142: return 5; // Piccolo
				case 131: case 143: return 7; // Nail
				case 132: case 144: return 8; // Daimao
				case 237: case 238: case 239: case 240: return 9; // Nail M
				case 254: case 255: case 256: case 257: return 10; // Champa
				case 133: case 136: return 11; // Kakarot
				case 134: case 137: return 12; // Cađíc
				case 135: case 138: return 13; // Nappa
				case 241: case 242: case 243: case 244: return 14; // Cađíc M
			}
		}

		for (int i = 0; i < item.itemOption.Length; i++)
		{
			ItemOption opt = item.itemOption[i];
			if (opt == null || opt.optionTemplate == null || string.IsNullOrEmpty(opt.optionTemplate.name)) continue;
			string low = opt.optionTemplate.name.ToLower();
			if (low.Contains("songoku") || low.Contains("kamejoko")) return 0;
			if (low.Contains("tienshinhan") || low.Contains("thiên xin") || low.Contains("thien xin")) return 1;
			if (low.Contains("krillin") || low.Contains("kirin")) return 2;
			if (low.Contains("kaioshin") || low.Contains("kaio-ken") || low.Contains("kaioken") || low.Contains("thần vũ trụ")) return 3;
			if (low.Contains("gohan")) return 4;
			if (low.Contains("piccolo daimao") || low.Contains("daimao")) return 8;
			if (low.Contains("ốc tiêu") || low.Contains("oc tieu")) return 6;
			if (low.Contains("piccolo") || low.Contains("pikkoro")) return 5;
			if (low.Contains("nail namek")) return 9;
			if (low.Contains("nail")) return 7;
			if (low.Contains("champa")) return 10;
			if (low.Contains("kakarot") || low.Contains("galick")) return 11;
			if (low.Contains("vegeta m") || low.Contains("cađíc m") || low.Contains("cadic m")) return 14;
			if (low.Contains("cađíc") || low.Contains("cadic") || low.Contains("vegeta")) return 12;
			if (low.Contains("nappa")) return 13;
		}
		return IsSetKichHoat(item) ? 15 : -1;
	}

	private static void ScanArrayForSkh(Item[] arr, int[,] planetCounts, int[,] setCounts)
	{
		if (arr == null) return;
		for (int i = 0; i < arr.Length; i++)
		{
			Item item = arr[i];
			if (item == null || item.template == null || !IsSetKichHoat(item)) continue;
			int type = item.template.type;
			if (type < 0 || type > 4) continue;
			int gender = item.template.gender;
			if (type == 4 || gender < 0 || gender > 2)
			{
				planetCounts[0, 4]++;
				planetCounts[1, 4]++;
				planetCounts[2, 4]++;
			}
			else
			{
				planetCounts[gender, type]++;
			}

			int setIdx = ClassifySkhSetIndex(item);
			if (setIdx >= 0 && setIdx < s_SkhSetNames.Length)
			{
				setCounts[setIdx, type]++;
			}
		}
	}

	public static void PaintSkhHUD(mGraphics g)
	{
		if (!showSkhHud || !ModMenu.IsInGame() || GameCanvas.panel.isShow || ModUI.uiCustomOpen) return;
		try
		{
			Char me = Char.myCharz();
			if (me == null) return;

			int[,] planetCounts = new int[3, 5];
			int[,] setCounts = new int[s_SkhSetNames.Length, 5];
			ScanArrayForSkh(me.arrItemBody, planetCounts, setCounts);
			ScanArrayForSkh(me.arrItemBag, planetCounts, setCounts);
			ScanArrayForSkh(me.arrItemBox, planetCounts, setCounts);

			g.translate(-g.getTranslateX(), -g.getTranslateY());
			g.setClip(0, 0, GameCanvas.w, GameCanvas.h);

			int x = 6 + SafeArea.Left;
			int y = 92;
			mFont.tahoma_7b_yellow.drawString(g, "-- THỐNG KÊ 15 BỘ SKH [Áo,Quần,Găng,Giày,Rada] --", x, y, mFont.LEFT);
			int row = 1;
			string[] pNames = new string[] { "TĐ", "NM", "XD" };
			for (int p = 0; p < 3; p++)
			{
				string line = pNames[p] + ": [" + planetCounts[p, 0] + "," + planetCounts[p, 1] + "," + planetCounts[p, 2] + "," + planetCounts[p, 3] + "," + planetCounts[p, 4] + "]";
				mFont font = (me.cgender == p) ? mFont.tahoma_7_yellow : mFont.tahoma_7_white;
				font.drawString(g, line, x, y + 11 * row, mFont.LEFT);
				row++;
			}

			for (int s = 0; s < s_SkhSetNames.Length; s++)
			{
				int total = 0;
				int slotsFilled = 0;
				for (int t = 0; t < 5; t++)
				{
					total += setCounts[s, t];
					if (setCounts[s, t] > 0) slotsFilled++;
				}
				if (total <= 0) continue;

				string sLine = "• " + s_SkhSetNames[s] + ": [" + setCounts[s, 0] + "," + setCounts[s, 1] + "," + setCounts[s, 2] + "," + setCounts[s, 3] + "," + setCounts[s, 4] + "] (" + slotsFilled + "/5)";
				mFont sFont = (slotsFilled >= 5) ? mFont.tahoma_7b_white : mFont.tahoma_7_yellow;
				sFont.drawString(g, sLine, x, y + 11 * row, mFont.LEFT);
				row++;
			}
		}
		catch
		{
		}
	}
}
