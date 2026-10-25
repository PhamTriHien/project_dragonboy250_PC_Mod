using System;
using System.Collections.Generic;

/// <summary>
/// CLASS AutoPickActionListener: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public class AutoPickActionListener : IActionListener
{
	public const int ACTION_SET_FILTER_IDS = 99101;
	public const int ACTION_SET_FILTER_NAMES = 99102;
	public const int ACTION_SET_DROP_IDS = 99103;
	public const int ACTION_SET_BLACKLIST_IDS = 99104;

	/// <summary>
	/// Thực thi hành động tương tác khi người dùng kích hoạt lệnh hoặc nút bấm.
	/// </summary>
	public void perform(int idAction, object p)
	{
		try
		{
			if (GameCanvas.inputDlg == null || GameCanvas.inputDlg.tfInput == null)
			{
				GameCanvas.endDlg();
				return;
			}
			string text = GameCanvas.inputDlg.tfInput.getText();
			GameCanvas.endDlg();

			if (idAction == ACTION_SET_FILTER_IDS)
			{
				ModAutoPick.SetFilterIds(text);
				ModConfig.SaveConfig();
				SoundMn.gI().buttonClick();
			}
			else if (idAction == ACTION_SET_FILTER_NAMES)
			{
				ModAutoPick.SetFilterNames(text);
				ModConfig.SaveConfig();
				SoundMn.gI().buttonClick();
			}
			else if (idAction == ACTION_SET_DROP_IDS)
			{
				ModAutoPick.SetDropIds(text);
				ModConfig.SaveConfig();
				SoundMn.gI().buttonClick();
			}
			else if (idAction == ACTION_SET_BLACKLIST_IDS)
			{
				ModAutoPick.SetBlacklistIds(text);
				ModConfig.SaveConfig();
				SoundMn.gI().buttonClick();
			}
			if (ModAutoPick.reopenModUiAfterInput)
			{
				ModAutoPick.reopenModUiAfterInput = false;
				ModUI.uiCustomOpen = true;
			}
		}
		catch
		{
			GameCanvas.endDlg();
		}
	}
}

/// <summary>
/// CLASS ModAutoPick: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public static class ModAutoPick
{
	public static bool autoPick = true;
	public static bool pickAll = true;
	public static bool pickGold = true;
	public static bool pickEquip = true;
	public static bool pickGem = true;
	public static int maxPickDistance = 0; // 0: Toàn Map, 50, 100, 150, 200, 300, 500
	public static bool onlyMyItems = false;
	public static bool showPickRadius = false;

	// Danh sách ID không nhặt (Blacklist)
	public static string blacklistIdsRaw = "";
	public static HashSet<int> blacklistIds = new HashSet<int>();

	// Lọc theo ID Item
	public static bool filterById = false;
	public static string filterIdsRaw = "";
	public static HashSet<int> filterIds = new HashSet<int>();

	// Lọc theo Tên Item
	public static bool filterByName = false;
	public static string filterNameRaw = "";
	public static List<string> filterNameKeywords = new List<string>();

	// Tự động Vứt Đồ theo danh sách ID
	public static bool autoDropItem = false;
	public static string dropIdsRaw = "";
	public static HashSet<int> dropIds = new HashSet<int>();
	private static long lastDropTime = 0;

	public static bool isBusy = false;
	private static long lastPickTime = 0;
	private static int lastPickMapId = -1;
	private static int lastPickedItemMapId = -1;
	private static int currentItemRetryCount = 0;
	private static readonly Dictionary<int, long> ignoredItemUntil = new Dictionary<int, long>();
	private static readonly AutoPickActionListener listener = new AutoPickActionListener();

	/// <summary>
	/// Khởi tạo và nạp dữ liệu cấu hình cho LoadFilterIds.
	/// </summary>
	public static void LoadFilterIds(string raw)
	{
		filterIdsRaw = raw != null ? raw.Trim() : "";
		filterIds.Clear();
		if (!string.IsNullOrEmpty(filterIdsRaw))
		{
			string[] parts = filterIdsRaw.Split(new char[] { ',', ' ', ';', '|' }, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < parts.Length; i++)
			{
				if (int.TryParse(parts[i].Trim(), out int id))
				{
					filterIds.Add(id);
				}
			}
		}
	}

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho SetFilterIds.
	/// </summary>
	public static void SetFilterIds(string raw)
	{
		LoadFilterIds(raw);
		if (filterIds.Count > 0)
		{
			filterById = true;
			pickAll = false;
			GameScr.info1.addInfo("Đã đặt lọc ID nhặt (" + filterIds.Count + " ID): [" + filterIdsRaw + "]", 0);
		}
		else
		{
			filterById = false;
			GameScr.info1.addInfo("Đã xóa bộ lọc ID nhặt", 0);
		}
	}

	/// <summary>
	/// Khởi tạo và nạp dữ liệu cấu hình cho LoadFilterNames.
	/// </summary>
	public static void LoadFilterNames(string raw)
	{
		filterNameRaw = raw != null ? raw.Trim() : "";
		filterNameKeywords.Clear();
		if (!string.IsNullOrEmpty(filterNameRaw))
		{
			string[] parts = filterNameRaw.Split(new char[] { ',', ';', '|' }, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < parts.Length; i++)
			{
				string kw = parts[i].Trim().ToLower();
				if (!string.IsNullOrEmpty(kw))
				{
					filterNameKeywords.Add(kw);
				}
			}
		}
	}

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho SetFilterNames.
	/// </summary>
	public static void SetFilterNames(string raw)
	{
		LoadFilterNames(raw);
		if (filterNameKeywords.Count > 0)
		{
			filterByName = true;
			pickAll = false;
			GameScr.info1.addInfo("Đã đặt lọc Tên nhặt (" + filterNameKeywords.Count + " từ khóa): [" + filterNameRaw + "]", 0);
		}
		else
		{
			filterByName = false;
			GameScr.info1.addInfo("Đã xóa bộ lọc Tên nhặt", 0);
		}
	}

	/// <summary>
	/// Dọn dẹp và đặt lại trạng thái ban đầu cho ClearFilters.
	/// </summary>
	public static void ClearFilters()
	{
		filterIdsRaw = "";
		filterIds.Clear();
		filterById = false;

		filterNameRaw = "";
		filterNameKeywords.Clear();
		filterByName = false;

		dropIdsRaw = "";
		dropIds.Clear();
		autoDropItem = false;

		blacklistIdsRaw = "";
		blacklistIds.Clear();

		GameScr.info1.addInfo("Đã xóa toàn bộ lọc nhặt và danh sách vứt đồ", 0);
	}

	public static void LoadBlacklistIds(string raw)
	{
		blacklistIdsRaw = raw != null ? raw.Trim() : "";
		blacklistIds.Clear();
		if (!string.IsNullOrEmpty(blacklistIdsRaw))
		{
			string[] parts = blacklistIdsRaw.Split(new char[] { ',', ' ', ';', '|' }, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < parts.Length; i++)
			{
				if (int.TryParse(parts[i].Trim(), out int id))
				{
					blacklistIds.Add(id);
				}
			}
		}
	}

	public static void SetBlacklistIds(string raw)
	{
		LoadBlacklistIds(raw);
		if (blacklistIds.Count > 0)
		{
			GameScr.info1.addInfo("Đã đặt DS Không Nhặt (" + blacklistIds.Count + " ID): [" + blacklistIdsRaw + "]", 0);
		}
		else
		{
			GameScr.info1.addInfo("Đã xóa DS Không Nhặt", 0);
		}
	}

	public static void ShowInputBlacklistId()
	{
		if (GameCanvas.inputDlg == null) return;
		reopenModUiAfterInput = ModUI.uiCustomOpen;
		ModUI.uiCustomOpen = false;
		ModMenu.modMenuOpen = false;
		GameCanvas.inputDlg.show("Nhập danh sách ID KHÔNG nhặt (cách nhau dấu ; hoặc ,):", new Command("OK", listener, AutoPickActionListener.ACTION_SET_BLACKLIST_IDS, null), TField.INPUT_TYPE_ANY);
		if (GameCanvas.inputDlg.tfInput != null)
		{
			GameCanvas.inputDlg.tfInput.setMaxTextLenght(150);
			GameCanvas.inputDlg.tfInput.setText(blacklistIdsRaw);
			if (!Main.isPC) GameCanvas.inputDlg.tfInput.setFocusWithKb(isFocus: true);
		}
	}

	public static void CycleMaxPickDistance()
	{
		if (maxPickDistance <= 0) maxPickDistance = 50;
		else if (maxPickDistance == 50) maxPickDistance = 100;
		else if (maxPickDistance == 100) maxPickDistance = 150;
		else if (maxPickDistance == 150) maxPickDistance = 200;
		else if (maxPickDistance == 200) maxPickDistance = 300;
		else if (maxPickDistance == 300) maxPickDistance = 500;
		else maxPickDistance = 0;
	}

	public static void PaintPickRadius(mGraphics g)
	{
		if (!showPickRadius || maxPickDistance <= 0 || !ModMenu.IsInGame() || g == null) return;
		if ((GameCanvas.panel != null && GameCanvas.panel.isShow) ||
		    (GameCanvas.panel2 != null && GameCanvas.panel2.isShow) ||
		    (GameCanvas.menu != null && GameCanvas.menu.showMenu) ||
		    GameCanvas.currentDialog != null ||
		    ModUI.uiCustomOpen)
		{
			return;
		}
		try
		{
			Char me = Char.myCharz();
			if (me == null) return;
			g.translate(-g.getTranslateX(), -g.getTranslateY());
			g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
			g.setColor(0x00FF00);

			int cx = me.cx - GameScr.cmx;
			int cy = me.cy - 10 - GameScr.cmy;
			int r = maxPickDistance;
			int steps = 24;
			for (int i = 0; i < steps; i++)
			{
				int a1 = i * 360 / steps;
				int a2 = (i + 1) * 360 / steps;
				int x1 = cx + r * Res.cos(a1) / 1024;
				int y1 = cy + r * Res.sin(a1) / 1024;
				int x2 = cx + r * Res.cos(a2) / 1024;
				int y2 = cy + r * Res.sin(a2) / 1024;
				g.drawLine(x1, y1, x2, y2);
			}
		}
		catch
		{
		}
	}

	public static string GetPickDistanceLabel()
	{
		return (maxPickDistance <= 0) ? "Toàn Map" : (maxPickDistance + "px");
	}

	public static void LoadDropIds(string raw)
	{
		dropIdsRaw = raw != null ? raw.Trim() : "";
		dropIds.Clear();
		if (!string.IsNullOrEmpty(dropIdsRaw))
		{
			string[] parts = dropIdsRaw.Split(new char[] { ',', ' ', ';', '|' }, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < parts.Length; i++)
			{
				if (int.TryParse(parts[i].Trim(), out int id))
				{
					dropIds.Add(id);
				}
			}
		}
	}

	public static void SetDropIds(string raw)
	{
		LoadDropIds(raw);
		if (dropIds.Count > 0)
		{
			autoDropItem = true;
			GameScr.info1.addInfo("Đã đặt tự vứt (" + dropIds.Count + " ID): [" + dropIdsRaw + "]", 0);
		}
		else
		{
			autoDropItem = false;
			GameScr.info1.addInfo("Đã xóa danh sách ID tự vứt", 0);
		}
	}

	public static void ShowInputDropId()
	{
		if (GameCanvas.inputDlg == null) return;
		reopenModUiAfterInput = ModUI.uiCustomOpen;
		ModUI.uiCustomOpen = false;
		ModMenu.modMenuOpen = false;
		GameCanvas.inputDlg.show("Nhập danh sách ID item cần tự vứt bỏ (cách nhau dấu phẩy):", new Command("OK", listener, AutoPickActionListener.ACTION_SET_DROP_IDS, null), TField.INPUT_TYPE_ANY);
		if (GameCanvas.inputDlg.tfInput != null)
		{
			GameCanvas.inputDlg.tfInput.setMaxTextLenght(150);
			GameCanvas.inputDlg.tfInput.setText(dropIdsRaw);
			if (!Main.isPC) GameCanvas.inputDlg.tfInput.setFocusWithKb(isFocus: true);
		}
	}

	private static void UpdateAutoDrop(Char me, long now)
	{
		if (!autoDropItem || dropIds.Count == 0 || me == null || me.arrItemBag == null || now - lastDropTime < 450)
		{
			return;
		}
		for (int i = 0; i < me.arrItemBag.Length; i++)
		{
			Item it = me.arrItemBag[i];
			if (it != null && it.template != null && dropIds.Contains(it.template.id))
			{
				lastDropTime = now;
				Service.gI().useItem(2, 1, (sbyte)i, -1);
				return;
			}
		}
	}

	public static bool reopenModUiAfterInput = false;

	/// <summary>
	/// Thực thi phương thức nghiệp vụ ShowInputFilterId.
	/// </summary>
	public static void ShowInputFilterId()
	{
		if (GameCanvas.inputDlg == null) return;
		reopenModUiAfterInput = ModUI.uiCustomOpen;
		ModUI.uiCustomOpen = false;
		ModMenu.modMenuOpen = false;
		GameCanvas.inputDlg.show("Nhập danh sách ID cần nhặt (cách nhau dấu phẩy hoặc khoảng trắng):", new Command("OK", listener, AutoPickActionListener.ACTION_SET_FILTER_IDS, null), TField.INPUT_TYPE_ANY);
		if (GameCanvas.inputDlg.tfInput != null)
		{
			GameCanvas.inputDlg.tfInput.setMaxTextLenght(150);
			GameCanvas.inputDlg.tfInput.setText(filterIdsRaw);
			if (!Main.isPC) GameCanvas.inputDlg.tfInput.setFocusWithKb(isFocus: true);
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ ShowInputFilterName.
	/// </summary>
	public static void ShowInputFilterName()
	{
		if (GameCanvas.inputDlg == null) return;
		reopenModUiAfterInput = ModUI.uiCustomOpen;
		ModUI.uiCustomOpen = false;
		ModMenu.modMenuOpen = false;
		GameCanvas.inputDlg.show("Nhập từ khóa tên item cần nhặt (cách nhau dấu phẩy):", new Command("OK", listener, AutoPickActionListener.ACTION_SET_FILTER_NAMES, null), TField.INPUT_TYPE_ANY);
		if (GameCanvas.inputDlg.tfInput != null)
		{
			GameCanvas.inputDlg.tfInput.setMaxTextLenght(150);
			GameCanvas.inputDlg.tfInput.setText(filterNameRaw);
			if (!Main.isPC) GameCanvas.inputDlg.tfInput.setFocusWithKb(isFocus: true);
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ ShouldPickItem.
	/// </summary>
	public static bool ShouldPickItem(ItemMap it)
	{
		if (it == null || it.status == 2) return false;
		Char me = Char.myCharz();
		int myId = (me != null) ? me.charID : -1;
		if (onlyMyItems)
		{
			if (it.playerId != myId) return false;
		}
		else
		{
			bool isMyOrPublicItem = (it.playerId == myId || it.playerId == -1 || it.playerId == -2 || it.playerId == 0);
			if (!isMyOrPublicItem) return false;
		}

		if (it.template == null) return pickAll;
		if (it.template.type >= 0 && it.template.type <= 5 && GameScr.gI() != null && me != null && me.arrItemBag != null && GameScr.gI().isBagFull())
		{
			return false;
		}
		int tId = it.template.id;
		string name = it.template.name;

		if (blacklistIds.Count > 0 && blacklistIds.Contains(tId))
		{
			return false;
		}

		// Vật phẩm 673 (cờ/vật phẩm cần xác nhận askToPick) chỉ nhặt khi người chơi chỉ định đích danh trong lọc ID
		if (tId == 673 && !(filterById && filterIds.Contains(673)))
		{
			return false;
		}

		if (pickAll) return true;

		// 1. Lọc theo danh sách ID
		if (filterById && filterIds.Contains(tId))
		{
			return true;
		}

		// 2. Lọc theo từ khóa Tên
		if (filterByName && !string.IsNullOrEmpty(name))
		{
			string lowerName = name.ToLower();
			for (int i = 0; i < filterNameKeywords.Count; i++)
			{
				if (lowerName.Contains(filterNameKeywords[i]))
				{
					return true;
				}
			}
		}

		// 3. Vàng (id 190, 76...)
		if (pickGold && (tId == 190 || tId == 76 || (name != null && name.ToLower().Contains("vàng"))))
		{
			return true;
		}

		// 4. Trang bị (type 0..5: áo, quần, găng, giày, rada)
		if (pickEquip && it.template.type >= 0 && it.template.type <= 5)
		{
			return true;
		}

		// 5. Ngọc rồng & ngọc (type 12 hoặc id 14..20)
		if (pickGem && (it.template.type == 12 || (tId >= 14 && tId <= 20) || (name != null && name.ToLower().Contains("ngọc"))))
		{
			return true;
		}

		return false;
	}

	/// <summary>
	/// Kiểm tra xem trên bản đồ có vật phẩm rơi hợp lệ đang chờ nhặt hay không.
	/// </summary>
	public static bool HasPendingPickableItem()
	{
		if (!autoPick || ModNextMap.isNextMapActive || ModGoBack.isReturning || ModSetActivator.isBusy || ModAutoBuyBua.isBusy)
		{
			return false;
		}
		Char me = Char.myCharz();
		MyVector items = GameScr.vItemMap;
		if (me == null || items == null || items.size() == 0) return false;
		long now = mSystem.currentTimeMillis();
		for (int i = 0; i < items.size(); i++)
		{
			ItemMap it = (ItemMap)items.elementAt(i);
			if (it == null) continue;
			if (ignoredItemUntil.TryGetValue(it.itemMapID, out long ignoreUntil) && now < ignoreUntil) continue;
			int ix = (it.xEnd != 0) ? it.xEnd : it.x;
			int iy = (it.yEnd != 0) ? it.yEnd : it.y;
			if (maxPickDistance > 0 && Res.distance(me.cx, me.cy, ix, iy) > maxPickDistance) continue;
			if (ShouldPickItem(it)) return true;
		}
		return false;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ RunRealAutoPick.
	/// </summary>
	public static void RunRealAutoPick()
	{
		try
		{
			Char me = Char.myCharz();
			long now = mSystem.currentTimeMillis();
			if (me != null && me.cHP > 0 && !Char.isLoadingMap && !Char.ischangingMap)
			{
				UpdateAutoDrop(me, now);
			}

			if (!autoPick || ModNextMap.isNextMapActive || ModGoBack.isReturning || ModSetActivator.isBusy || ModAutoBuyBua.isBusy)
			{
				isBusy = false;
				return;
			}
			if (TileMap.mapID != lastPickMapId)
			{
				lastPickMapId = TileMap.mapID;
				lastPickedItemMapId = -1;
				currentItemRetryCount = 0;
				ignoredItemUntil.Clear();
			}
			if (me == null || me.cHP <= 0 || me.statusMe == 14 || me.statusMe == 5 || Char.isLoadingMap || Char.ischangingMap)
			{
				isBusy = false;
				return;
			}

			MyVector items = GameScr.vItemMap;
			if (items == null || items.size() == 0)
			{
				if (lastPickedItemMapId != -1)
				{
					ModTdltLogger.Log("ITEM_PICKED_OK", $"Server confirmed pick (cleared) -> itemMapId={lastPickedItemMapId}");
				}
				lastPickedItemMapId = -1;
				currentItemRetryCount = 0;
				isBusy = false;
				return;
			}

			ItemMap closest = null;
			int minDistance = int.MaxValue;

			for (int i = 0; i < items.size(); i++)
			{
				ItemMap it = (ItemMap)items.elementAt(i);
				if (it == null)
				{
					continue;
				}
				if (it.itemMapID == lastPickedItemMapId && it.status == 2)
				{
					ModTdltLogger.Log("ITEM_PICKED_OK", $"Server confirmed pick -> itemMapId={it.itemMapID}, tplId={it.template?.id ?? -1} ({it.template?.name})");
					lastPickedItemMapId = -1;
					currentItemRetryCount = 0;
				}
				if (ignoredItemUntil.TryGetValue(it.itemMapID, out long ignoreUntil))
				{
					if (now < ignoreUntil) continue;
					ignoredItemUntil.Remove(it.itemMapID);
				}

				if (ShouldPickItem(it))
				{
					int ix = (it.xEnd != 0) ? it.xEnd : it.x;
					int iy = (it.yEnd != 0) ? it.yEnd : it.y;
					int dist = Res.distance(me.cx, me.cy, ix, iy);
					if (maxPickDistance > 0 && dist > maxPickDistance)
					{
						continue;
					}
					if (dist < minDistance)
					{
						minDistance = dist;
						closest = it;
					}
				}
			}

			if (closest != null)
			{
				bool isFightingTarget = ModTanSat.autoTanSat &&
				                        ModTanSat.currentFarmTarget != null &&
				                        ModTanSat.currentFarmTarget.status != 0 &&
				                        ModTanSat.currentFarmTarget.status != 1 &&
				                        ModTanSat.currentFarmTarget.hp > 0;

				int pickX = (closest.xEnd != 0) ? closest.xEnd : closest.x;
				int pickY = (closest.yEnd != 0) ? closest.yEnd : closest.y;
				if (ModTanSatTargeting.IsInsideWaypoint(pickX, pickY) && TileMap.pxw > 160)
				{
					if (pickX < 72) pickX = 72;
					else if (pickX > TileMap.pxw - 72) pickX = TileMap.pxw - 72;
				}
				if (!TileMap.isInAirMap())
				{
					int groundY = ModTanSatTargeting.FindGroundY(pickX, pickY, 48);
					if (groundY != -1)
					{
						pickY = groundY;
					}
				}

				int dx = Res.abs(me.cx - pickX);
				int dy = Res.abs(me.cy - pickY);
				int groundDist = Res.distance(me.cx, me.cy, pickX, pickY);

				// Khi đang đánh dở quái Tàn Sát: Chỉ nhặt nếu item rơi gần (<= 72px), không dịch chuyển bỏ dở quái đang đánh
				if (isFightingTarget && groundDist > 72)
				{
					isBusy = false;
					return;
				}

				isBusy = true;
				bool coldPick = ModTeleport.IsColdPlanetAntiCheckActive();
				if (coldPick && ModTeleport.isWaitingSettle && !TileMap.tileTypeAt(me.cx, me.cy, 2))
				{
					me.statusMe = 1;
					me.delayFall = 0;
					me.cvx = 0;
					me.cvy = 0;
				}
				bool isNewItem = (closest.itemMapID != lastPickedItemMapId);
				int pickInterval = (coldPick && ModTeleport.isWaitingSettle)
					? ModTeleport.ColdStepIntervalMs
					: (isNewItem ? 45 : ((currentItemRetryCount >= 1) ? 260 : 160));
				if (now - lastPickTime < pickInterval)
				{
					return;
				}

				bool posSyncedForInPlace = !coldPick || ModTeleport.isCurrentPosSynced;
				int inPlaceMaxX = posSyncedForInPlace ? 22 : 6;
				int inPlaceTolY = posSyncedForInPlace ? (coldPick ? 12 : 16) : 2;
				bool canPickInPlace = dx <= inPlaceMaxX && dy <= inPlaceTolY && (isNewItem || currentItemRetryCount < 1);
				if (canPickInPlace)
				{
					if (ModTeleport.IsWaitingServerSyncForAttack())
					{
						return;
					}
				}
				else
				{
					if (ModTeleport.IsWaitingServerSync())
					{
						return;
					}
				}

				if (closest.itemMapID == lastPickedItemMapId)
				{
					currentItemRetryCount++;
					if (currentItemRetryCount > 4)
					{
						ignoredItemUntil[closest.itemMapID] = now + 10000L;
						lastPickedItemMapId = -1;
						currentItemRetryCount = 0;
						isBusy = false;
						return;
					}
				}
				else
				{
					lastPickedItemMapId = closest.itemMapID;
					currentItemRetryCount = 1;
				}

				lastPickTime = now;
				me.itemFocus = closest;
				if (canPickInPlace)
				{
					ModTeleport.isWaitingSettle = false;
					ModTdltLogger.Log("ITEM_PICK", $"Pick in-place -> itemMapId={closest.itemMapID}, tplId={closest.template?.id ?? -1} ({closest.template?.name}), pos=({pickX},{pickY}), me=({me.cx},{me.cy}), retry={currentItemRetryCount}");
					Service.gI().pickItem(closest.itemMapID);
				}
				else
				{
					bool wasAlreadyThere = Res.abs(me.cx - pickX) <= 12 && Res.abs(me.cy - pickY) <= (coldPick ? 2 : 16);
					if (ModTeleport.MoveSteppedTo(pickX, pickY, isTargetFlying: false, allowColdLongTeleport: true))
					{
						if (wasAlreadyThere && currentItemRetryCount >= 2 && (me.cx != pickX || me.cy != pickY || !coldPick || currentItemRetryCount == 3))
						{
							ModTeleport.SendSingleCharMove(pickX, pickY);
						}
						ModTanSat.lastTeleportTime = 0;
						if (!ModTeleport.IsWaitingServerSyncForAttack())
						{
							ModTdltLogger.Log("ITEM_PICK", $"Pick after step -> itemMapId={closest.itemMapID}, tplId={closest.template?.id ?? -1} ({closest.template?.name}), pos=({pickX},{pickY}), me=({me.cx},{me.cy}), retry={currentItemRetryCount}");
							Service.gI().pickItem(closest.itemMapID);
						}
						else
						{
							currentItemRetryCount--;
						}
					}
					else
					{
						ModTanSat.lastTeleportTime = 0;
						currentItemRetryCount--;
						if (ModTeleport.IsColdStepTimedOut())
						{
							ignoredItemUntil[closest.itemMapID] = now + 10000L;
							lastPickedItemMapId = -1;
							currentItemRetryCount = 0;
							ModTeleport.ResetColdStepTracking();
							isBusy = false;
							return;
						}
					}
				}
			}
			else
			{
				lastPickedItemMapId = -1;
				currentItemRetryCount = 0;
				isBusy = false;
			}
		}
		catch
		{
			isBusy = false;
		}
	}

	/// <summary>
	/// Xác nhận máy chủ đã xử lý nhặt vật phẩm (cmd -20 hoặc cmd -19) để giải phóng trạng thái bận tức thời.
	/// </summary>
	public static void OnServerConfirmPick(int itemMapId, bool isMe)
	{
		if (isMe)
		{
			ModTdltLogger.Log("ITEM_PICKED_OK", $"itemMapId={itemMapId}, retry={currentItemRetryCount}");
		}
		if (lastPickedItemMapId == itemMapId)
		{
			lastPickedItemMapId = -1;
			currentItemRetryCount = 0;
			isBusy = false;
		}
		ignoredItemUntil.Remove(itemMapId);
	}
}
