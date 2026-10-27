using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Mở rộng Hành Trang & Cửa Hàng: Quản lý Set Đồ 1 & 2, Mua Số Lượng Lớn (Mua SLL) và Auto Sử Dụng vật phẩm trong túi.
/// </summary>
public sealed class ModInventoryExt : IActionListener
{
	private static readonly ModInventoryExt s_Instance = new ModInventoryExt();

	public const int ACT_SET1_ADD = 29001;
	public const int ACT_SET1_REMOVE = 29002;
	public const int ACT_SET2_ADD = 29003;
	public const int ACT_SET2_REMOVE = 29004;
	public const int ACT_SHOP_BULK_BUY = 29005;
	public const int ACT_SHOP_BULK_STOP = 29006;
	public const int ACT_BAG_AUTO_USE = 29007;
	public const int ACT_BAG_AUTO_STOP = 29008;

	private const int DLG_BULK_BUY_QTY = 29021;
	private const int DLG_AUTO_USE_SEC = 29022;

	public static string equipSet1Raw = string.Empty;
	public static string equipSet2Raw = string.Empty;

	private static readonly List<string> s_EquipQueue = new List<string>();
	private static long s_LastEquipTick = 0;

	public static bool isBulkBuying = false;
	public static int bulkBuyTargetId = -1;
	public static string bulkBuyItemName = string.Empty;
	public static sbyte bulkBuyCurrencyType = 0;
	public static int bulkBuyTotalQty = 0;
	public static int bulkBuyDoneQty = 0;
	private static long s_LastBulkBuyTick = 0;
	private static Item s_PendingShopItem = null;
	private static int s_PendingShopSelected = -1;
	private static int s_PendingShopType = 0;

	public static int autoUseTemplateId = -1;
	public static string autoUseItemName = string.Empty;
	public static int autoUseIntervalSec = 10;
	public static readonly Dictionary<int, int> autoUseItemsSec = new Dictionary<int, int>();
	private static readonly Dictionary<int, long> s_LastMultiAutoUseTick = new Dictionary<int, long>();
	private static readonly List<int> s_AutoUseKeyBuffer = new List<int>();
	private static Item s_PendingBagItem = null;

	public static string ExportAutoUseItems()
	{
		if (autoUseItemsSec.Count == 0) return string.Empty;
		StringBuilder sb = new StringBuilder();
		foreach (KeyValuePair<int, int> kv in autoUseItemsSec)
		{
			if (kv.Key < 0 || kv.Value < 1) continue;
			if (sb.Length > 0) sb.Append(';');
			sb.Append(kv.Key).Append(':').Append(kv.Value);
		}
		return sb.ToString();
	}

	public static void ImportAutoUseItems(string raw)
	{
		autoUseItemsSec.Clear();
		s_LastMultiAutoUseTick.Clear();
		if (string.IsNullOrEmpty(raw)) return;
		string[] parts = raw.Split(new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
		for (int i = 0; i < parts.Length; i++)
		{
			string p = parts[i].Trim();
			int sep = p.IndexOf(':');
			if (sep > 0)
			{
				if (int.TryParse(p.Substring(0, sep).Trim(), out int tid) &&
				    int.TryParse(p.Substring(sep + 1).Trim(), out int sec) &&
				    tid >= 0 && sec >= 1)
				{
					autoUseItemsSec[tid] = sec;
				}
			}
		}
	}

	public static ModInventoryExt gI()
	{
		return s_Instance;
	}

	public static bool IsWearableItem(Item item)
	{
		if (item == null || item.template == null) return false;
		if (item.isTypeBody()) return true;
		int t = item.template.type;
		return (t >= 0 && t <= 5) || t == 11 || t == 23 || t == 32 || t == 35 || t == 72;
	}

	/// <summary>
	/// Sinh chuỗi định danh duy nhất cho trang bị dựa trên ID mẫu, cấp cường hóa và danh sách chỉ số.
	/// </summary>
	public static string BuildItemSignature(Item item)
	{
		if (item == null || item.template == null) return string.Empty;
		StringBuilder sb = new StringBuilder();
		sb.Append(item.template.id).Append('#');
		if (item.itemOption != null)
		{
			for (int i = 0; i < item.itemOption.Length; i++)
			{
				ItemOption opt = item.itemOption[i];
				if (opt == null || opt.optionTemplate == null) continue;
				int oid = opt.optionTemplate.id;
				if (oid == 93 || oid == 73) continue;
				sb.Append(oid).Append(':').Append(opt.param).Append(',');
			}
		}
		return sb.ToString();
	}

	private static List<string> ParseSetList(string raw)
	{
		List<string> list = new List<string>();
		if (string.IsNullOrEmpty(raw)) return list;
		string[] parts = raw.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
		for (int i = 0; i < parts.Length; i++)
		{
			string trimmed = parts[i].Trim();
			if (trimmed.Length > 0) list.Add(trimmed);
		}
		return list;
	}

	private static string JoinSetList(List<string> list)
	{
		if (list == null || list.Count == 0) return string.Empty;
		return string.Join("|", list.ToArray());
	}

	public static int GetSetCount(int setIndex)
	{
		return ParseSetList(setIndex == 1 ? equipSet1Raw : equipSet2Raw).Count;
	}

	public static bool IsInSet(int setIndex, Item item)
	{
		string sig = BuildItemSignature(item);
		if (sig.Length == 0) return false;
		List<string> list = ParseSetList(setIndex == 1 ? equipSet1Raw : equipSet2Raw);
		return list.Contains(sig);
	}

	public static void AddToSet(int setIndex, Item item)
	{
		if (item == null || item.template == null) return;
		string sig = BuildItemSignature(item);
		if (sig.Length == 0) return;

		List<string> list = ParseSetList(setIndex == 1 ? equipSet1Raw : equipSet2Raw);
		int sameTypeSlot = item.template.type;
		for (int i = list.Count - 1; i >= 0; i--)
		{
			int hashIdx = list[i].IndexOf('#');
			if (hashIdx > 0)
			{
				int tid;
				if (int.TryParse(list[i].Substring(0, hashIdx), out tid))
				{
					ItemTemplate tmpl = ItemTemplates.get((short)tid);
					if (tmpl != null && tmpl.type == sameTypeSlot)
					{
						list.RemoveAt(i);
					}
				}
			}
		}
		if (!list.Contains(sig))
		{
			list.Add(sig);
		}
		if (setIndex == 1) equipSet1Raw = JoinSetList(list);
		else equipSet2Raw = JoinSetList(list);
		ModConfig.SaveConfig();
		GameScr.info1.addInfo("Đã lưu [" + item.template.name + "] vào Set " + setIndex + " (" + list.Count + " món)", 0);
	}

	public static void RemoveFromSet(int setIndex, Item item)
	{
		string sig = BuildItemSignature(item);
		if (sig.Length == 0) return;
		List<string> list = ParseSetList(setIndex == 1 ? equipSet1Raw : equipSet2Raw);
		if (list.Remove(sig))
		{
			if (setIndex == 1) equipSet1Raw = JoinSetList(list);
			else equipSet2Raw = JoinSetList(list);
			ModConfig.SaveConfig();
			string name = (item != null && item.template != null) ? item.template.name : "Trang bị";
			GameScr.info1.addInfo("Đã xóa [" + name + "] khỏi Set " + setIndex, 0);
		}
	}

	public static void ClearSets()
	{
		equipSet1Raw = string.Empty;
		equipSet2Raw = string.Empty;
		s_EquipQueue.Clear();
		ModConfig.SaveConfig();
		GameScr.info1.addInfo("Đã xóa toàn bộ dữ liệu Set Đồ 1 & Set Đồ 2.", 0);
		SoundMn.gI().buttonClick();
	}

	public static void ShowEquipSetsInfo()
	{
		StringBuilder sb = new StringBuilder();
		AppendSingleSetSummary(sb, 1, equipSet1Raw);
		sb.Append('\n');
		AppendSingleSetSummary(sb, 2, equipSet2Raw);
		if (ModUI.uiCustomOpen)
		{
			ModUI.uiCustomOpen = false;
			ModMenu.modMenuOpen = false;
		}
		GameCanvas.startOKDlg(sb.ToString());
		SoundMn.gI().buttonClick();
	}

	private static void AppendSingleSetSummary(StringBuilder sb, int setIndex, string raw)
	{
		List<string> list = ParseSetList(raw);
		sb.Append("[Set Đồ ").Append(setIndex).Append(" - ").Append(list.Count).Append(" món]");
		if (list.Count == 0)
		{
			sb.Append("\n  (Chưa lưu món nào)");
			return;
		}
		Char me = Char.myCharz();
		for (int i = 0; i < list.Count; i++)
		{
			string sig = list[i];
			string itemName = sig;
			string extraTag = string.Empty;
			string statusTag = string.Empty;
			int hashIdx = sig.IndexOf('#');
			int tid = -1;
			if (hashIdx > 0 && int.TryParse(sig.Substring(0, hashIdx), out tid))
			{
				ItemTemplate tmpl = ItemTemplates.get((short)tid);
				itemName = (tmpl != null && !string.IsNullOrEmpty(tmpl.name)) ? tmpl.name : ("ID " + tid);
			}
			Item matched = FindLiveItemForSummary(me, sig, tid, out bool onBody);
			if (matched != null)
			{
				statusTag = onBody ? " [Đang mặc]" : " [Trong túi]";
				string tag = ModSetActivator.GetCompactOptionTag(matched);
				if (!string.IsNullOrEmpty(tag))
				{
					extraTag = " (" + tag + ")";
				}
			}
			else
			{
				statusTag = " [Chưa thấy]";
			}
			sb.Append("\n  ").Append(i + 1).Append(". ").Append(itemName).Append(extraTag).Append(statusTag);
		}
	}

	private static Item FindLiveItemForSummary(Char me, string sig, int tid, out bool onBody)
	{
		onBody = false;
		if (me == null) return null;
		if (me.arrItemBody != null)
		{
			for (int i = 0; i < me.arrItemBody.Length; i++)
			{
				Item it = me.arrItemBody[i];
				if (it != null && string.Equals(BuildItemSignature(it), sig, StringComparison.Ordinal))
				{
					onBody = true;
					return it;
				}
			}
		}
		if (me.arrItemBag != null)
		{
			int bagIdx = FindBagIndexBySignature(me.arrItemBag, sig);
			if (bagIdx >= 0) return me.arrItemBag[bagIdx];
		}
		if (tid >= 0 && me.arrItemBody != null)
		{
			for (int i = 0; i < me.arrItemBody.Length; i++)
			{
				Item it = me.arrItemBody[i];
				if (it != null && it.template != null && it.template.id == tid)
				{
					onBody = true;
					return it;
				}
			}
		}
		return null;
	}

	public static void EquipSet(int setIndex)
	{
		List<string> list = ParseSetList(setIndex == 1 ? equipSet1Raw : equipSet2Raw);
		if (list.Count == 0)
		{
			GameScr.info1.addInfo("Set Đồ " + setIndex + " đang trống! Mở Hành Trang bấm vào đồ để Thêm Vào Set.", 0);
			return;
		}
		s_EquipQueue.Clear();
		for (int i = 0; i < list.Count; i++)
		{
			s_EquipQueue.Add(list[i]);
		}
		GameScr.info1.addInfo("Đang mặc Set Đồ " + setIndex + " (" + list.Count + " món)...", 0);
		SoundMn.gI().buttonClick();
	}

	/// <summary>
	/// Nối các lệnh Set Đồ 1/2 và Auto Sử Dụng vào menu Hành Trang.
	/// </summary>
	public static void AppendInventoryCommands(MyVector menu, Panel panel, Item currItem, bool isBodySlot)
	{
		if (menu == null || panel == null || currItem == null || currItem.template == null) return;
		if (panel.type == 12 || panel.type == 13) return;

		if (IsWearableItem(currItem))
		{
			if (IsInSet(1, currItem))
				menu.addElement(new Command("Xóa Khỏi\nSet 1", panel, ACT_SET1_REMOVE, currItem));
			else
				menu.addElement(new Command("Thêm Vào\nSet 1", panel, ACT_SET1_ADD, currItem));

			if (IsInSet(2, currItem))
				menu.addElement(new Command("Xóa Khỏi\nSet 2", panel, ACT_SET2_REMOVE, currItem));
			else
				menu.addElement(new Command("Thêm Vào\nSet 2", panel, ACT_SET2_ADD, currItem));
		}
		if (!isBodySlot && currItem.template.id != 457)
		{
			if (autoUseItemsSec.TryGetValue(currItem.template.id, out int curSec))
				menu.addElement(new Command("Ngưng Auto\n(" + curSec + "s)", panel, ACT_BAG_AUTO_STOP, currItem));
			else
				menu.addElement(new Command("Auto\nSử Dụng", panel, ACT_BAG_AUTO_USE, currItem));
		}
	}

	/// <summary>
	/// Nối lệnh Mua SLL / Dừng Mua vào menu Cửa Hàng.
	/// </summary>
	public static void AppendShopCommands(MyVector menu, Panel panel, Item currItem, int selectedIdx, int typeShop)
	{
		if (menu == null || panel == null || currItem == null || currItem.template == null) return;
		if (panel.type == 17 || typeShop == 4 || typeShop == 0) return;

		bool canBuy = (currItem.isBuySpec && currItem.buySpec > 0) ||
		              currItem.buyCoin > 0 ||
		              currItem.buyGold > 0;
		if (!canBuy) return;

		if (isBulkBuying)
		{
			menu.addElement(new Command("Dừng Mua\nSLL", panel, ACT_SHOP_BULK_STOP, currItem));
		}
		else
		{
			s_PendingShopSelected = selectedIdx;
			s_PendingShopType = typeShop;
			menu.addElement(new Command("Mua SLL", panel, ACT_SHOP_BULK_BUY, currItem));
		}
	}

	/// <summary>
	/// Xử lý các idAction mở rộng từ Panel.perform.
	/// </summary>
	public static bool PerformPanelAction(int idAction, object p)
	{
		Item item = p as Item;
		switch (idAction)
		{
			case ACT_SET1_ADD:
				AddToSet(1, item);
				return true;
			case ACT_SET1_REMOVE:
				RemoveFromSet(1, item);
				return true;
			case ACT_SET2_ADD:
				AddToSet(2, item);
				return true;
			case ACT_SET2_REMOVE:
				RemoveFromSet(2, item);
				return true;
			case ACT_SHOP_BULK_BUY:
				if (item != null && item.template != null)
				{
					s_PendingShopItem = item;
					GameCanvas.inputDlg.show(
						"Nhập số lượng cần mua [" + item.template.name + "]:",
						new Command(mResources.OK, gI(), DLG_BULK_BUY_QTY, null),
						TField.INPUT_TYPE_NUMERIC);
					GameCanvas.inputDlg.tfInput.setText("10");
				}
				return true;
			case ACT_SHOP_BULK_STOP:
				StopBulkBuy("Đã dừng Mua SLL.");
				return true;
			case ACT_BAG_AUTO_USE:
				if (item != null && item.template != null)
				{
					s_PendingBagItem = item;
					int initSec = autoUseItemsSec.TryGetValue(item.template.id, out int existingSec) ? existingSec : autoUseIntervalSec;
					GameCanvas.inputDlg.show(
						"Nhập chu kỳ giây sử dụng [" + item.template.name + "]:",
						new Command(mResources.OK, gI(), DLG_AUTO_USE_SEC, null),
						TField.INPUT_TYPE_NUMERIC);
					GameCanvas.inputDlg.tfInput.setText(initSec.ToString());
				}
				return true;
			case ACT_BAG_AUTO_STOP:
				if (item != null && item.template != null)
				{
					autoUseItemsSec.Remove(item.template.id);
					s_LastMultiAutoUseTick.Remove(item.template.id);
					if (autoUseTemplateId == item.template.id)
					{
						autoUseTemplateId = -1;
						autoUseItemName = string.Empty;
					}
					ModConfig.SaveConfig();
					GameScr.info1.addInfo("Đã ngưng Auto Sử Dụng [" + item.template.name + "].", 0);
				}
				else
				{
					autoUseItemsSec.Clear();
					s_LastMultiAutoUseTick.Clear();
					autoUseTemplateId = -1;
					autoUseItemName = string.Empty;
					ModConfig.SaveConfig();
					GameScr.info1.addInfo("Đã ngưng toàn bộ Auto Sử Dụng vật phẩm.", 0);
				}
				return true;
		}
		return false;
	}

	public static void StopBulkBuy(string reason)
	{
		isBulkBuying = false;
		bulkBuyTargetId = -1;
		bulkBuyTotalQty = 0;
		bulkBuyDoneQty = 0;
		if (!string.IsNullOrEmpty(reason))
		{
			GameScr.info1.addInfo(reason, 0);
		}
	}

	public void perform(int idAction, object p)
	{
		if (idAction == DLG_BULK_BUY_QTY)
		{
			string txt = (GameCanvas.inputDlg.tfInput.getText() ?? string.Empty).Trim();
			GameCanvas.endDlg();
			int qty;
			if (!int.TryParse(txt, out qty) || qty <= 0 || s_PendingShopItem == null || s_PendingShopItem.template == null)
			{
				GameScr.info1.addInfo("Số lượng mua không hợp lệ!", 0);
				return;
			}
			if (s_PendingShopItem.isBuySpec && s_PendingShopItem.buySpec > 0)
			{
				bulkBuyCurrencyType = 3;
			}
			else if (s_PendingShopItem.buyCoin > 0)
			{
				bulkBuyCurrencyType = 0;
			}
			else
			{
				bulkBuyCurrencyType = 1;
			}
			bulkBuyTargetId = (s_PendingShopType == 8) ? s_PendingShopSelected : s_PendingShopItem.template.id;
			bulkBuyItemName = s_PendingShopItem.template.name;
			bulkBuyTotalQty = qty;
			bulkBuyDoneQty = 0;
			isBulkBuying = true;
			s_LastBulkBuyTick = 0;
			GameScr.info1.addInfo("Bắt đầu Mua SLL: " + qty + " x " + bulkBuyItemName, 0);
		}
		else if (idAction == DLG_AUTO_USE_SEC)
		{
			string txt = (GameCanvas.inputDlg.tfInput.getText() ?? string.Empty).Trim();
			GameCanvas.endDlg();
			int sec;
			if (!int.TryParse(txt, out sec) || sec < 1 || s_PendingBagItem == null || s_PendingBagItem.template == null)
			{
				GameScr.info1.addInfo("Chu kỳ giây phải >= 1!", 0);
				return;
			}
			autoUseIntervalSec = sec;
			int targetTplId = s_PendingBagItem.template.id;
			autoUseTemplateId = targetTplId;
			autoUseItemName = s_PendingBagItem.template.name;
			autoUseItemsSec[targetTplId] = sec;
			s_LastMultiAutoUseTick.Remove(targetTplId);
			ModConfig.SaveConfig();
			if (GameCanvas.panel != null && GameCanvas.panel.isShow)
			{
				GameCanvas.panel.hide();
			}
			GameScr.info1.addInfo("Auto Sử Dụng [" + autoUseItemName + "] mỗi " + sec + "s: BẬT (" + autoUseItemsSec.Count + " món)", 0);
		}
	}

	private static int FindBagIndexBySignature(Item[] bag, string sig)
	{
		if (bag == null || string.IsNullOrEmpty(sig)) return -1;
		for (int i = bag.Length - 1; i >= 0; i--)
		{
			if (bag[i] != null && string.Equals(BuildItemSignature(bag[i]), sig, StringComparison.Ordinal))
			{
				return i;
			}
		}
		int hashIdx = sig.IndexOf('#');
		if (hashIdx > 0)
		{
			int tid;
			if (int.TryParse(sig.Substring(0, hashIdx), out tid))
			{
				for (int i = bag.Length - 1; i >= 0; i--)
				{
					if (bag[i] != null && bag[i].template != null && bag[i].template.id == tid)
					{
						return i;
					}
				}
			}
		}
		return -1;
	}

	public static void Update()
	{
		if (!ModMenu.IsInGame()) return;
		Char me = Char.myCharz();
		if (me == null || me.isDie) return;

		long now = mSystem.currentTimeMillis();

		if (s_EquipQueue.Count > 0 && now - s_LastEquipTick >= 120)
		{
			s_LastEquipTick = now;
			string nextSig = s_EquipQueue[0];
			s_EquipQueue.RemoveAt(0);
			int bagIdx = FindBagIndexBySignature(me.arrItemBag, nextSig);
			if (bagIdx >= 0)
			{
				Service.gI().getItem(4, (sbyte)bagIdx);
			}
			if (s_EquipQueue.Count == 0)
			{
				GameScr.info1.addInfo("Đã hoàn tất mặc Set Đồ!", 0);
			}
		}

		if (isBulkBuying && now - s_LastBulkBuyTick >= 1500)
		{
			s_LastBulkBuyTick = now;
			if (bulkBuyDoneQty >= bulkBuyTotalQty)
			{
				StopBulkBuy("Đã mua đủ " + bulkBuyTotalQty + " [" + bulkBuyItemName + "]!");
			}
			else if (ModSetActivator.GetFreeBagSlots() <= 0)
			{
				StopBulkBuy("Hành trang đã đầy! Dừng Mua SLL (" + bulkBuyDoneQty + "/" + bulkBuyTotalQty + ").");
			}
			else
			{
				Service.gI().buyItem(bulkBuyCurrencyType, bulkBuyTargetId, 0);
				bulkBuyDoneQty++;
				if (bulkBuyDoneQty >= bulkBuyTotalQty)
				{
					StopBulkBuy("Đã mua xong " + bulkBuyDoneQty + " [" + bulkBuyItemName + "]!");
				}
			}
		}

		if (autoUseItemsSec.Count > 0 && GameCanvas.gameTick % 5 == 0 && me.arrItemBag != null)
		{
			s_AutoUseKeyBuffer.Clear();
			foreach (int k in autoUseItemsSec.Keys)
			{
				s_AutoUseKeyBuffer.Add(k);
			}
			for (int kIdx = 0; kIdx < s_AutoUseKeyBuffer.Count; kIdx++)
			{
				int tplId = s_AutoUseKeyBuffer[kIdx];
				if (!autoUseItemsSec.TryGetValue(tplId, out int intervalSec) || intervalSec < 1) continue;
				if (s_LastMultiAutoUseTick.TryGetValue(tplId, out long lastTick) && now - lastTick < intervalSec * 1000L)
				{
					continue;
				}
				int foundIdx = -1;
				for (int i = 0; i < me.arrItemBag.Length; i++)
				{
					Item it = me.arrItemBag[i];
					if (it != null && it.template != null && it.template.id == tplId)
					{
						foundIdx = i;
						break;
					}
				}
				if (foundIdx >= 0)
				{
					s_LastMultiAutoUseTick[tplId] = now;
					Service.gI().useItem(0, 1, (sbyte)foundIdx, -1);
					break;
				}
			}
		}
	}
}
