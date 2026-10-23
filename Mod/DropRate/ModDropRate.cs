using System;
using System.Collections.Generic;

/// <summary>
/// Module Tối Ưu Tỉ Lệ Rơi Đồ: Tự động nhặt vật phẩm tức thì (Instant Pick), hồi sinh đánh quái và khóa mục tiêu kết liễu.
/// </summary>
public static class ModDropRate
{
	// Các đòn bẩy tối ưu hóa tỉ lệ và sản lượng rơi đồ thực chiến
	public static bool isInstantPick = true;          // Hút đồ tức thì ngay tick 0 khi server gửi packet
	public static bool isInstantRespawnAttack = true; // Khóa và đánh quái ngay tick đầu tiên quái vừa hồi sinh
	public static bool isLastHitLock = true;          // Khóa đòn kết liễu đảm bảo quyền sở hữu item rơi 100%

	// Bộ đếm thống kê thực nghiệm thời gian thực (Real-Time Drop Rate Tracker)
	public static int totalMobsKilled = 0;
	public static int totalItemsDropped = 0;
	public static int totalSetKHCount = 0;
	public static int totalStarCount = 0;
	public static long sessionStartTime = 0;

	private static bool bagBaselineInitialized = false;
	private static readonly HashSet<Item> countedBagItems = new HashSet<Item>();

	/// <summary>
	/// Khởi tạo và nạp dữ liệu cấu hình cho InitSession.
	/// </summary>
	public static void InitSession()
	{
		if (sessionStartTime == 0)
		{
			sessionStartTime = mSystem.currentTimeMillis();
		}
	}

	/// <summary>
	/// Dọn dẹp và đặt lại trạng thái ban đầu cho ResetStats.
	/// </summary>
	public static void ResetStats()
	{
		totalMobsKilled = 0;
		totalItemsDropped = 0;
		totalSetKHCount = 0;
		totalStarCount = 0;
		sessionStartTime = mSystem.currentTimeMillis();
		bagBaselineInitialized = false;
		countedBagItems.Clear();
		GameScr.info1.addInfo("Đã đặt lại thống kê rơi đồ!", 0);
		SoundMn.gI().buttonClick();
	}

	/// <summary>
	/// Quét hành trang nhân vật để ghi nhận các trang bị Đồ Kích Hoạt và Đồ Sao mới nhặt được.
	/// </summary>
	public static void ScanBagForSpecialItems(Char me)
	{
		if (me == null || me.arrItemBag == null) return;
		if (!bagBaselineInitialized)
		{
			bagBaselineInitialized = true;
			for (int i = 0; i < me.arrItemBag.Length; i++)
			{
				if (me.arrItemBag[i] != null) countedBagItems.Add(me.arrItemBag[i]);
			}
			return;
		}
		for (int i = 0; i < me.arrItemBag.Length; i++)
		{
			Item item = me.arrItemBag[i];
			if (item == null || item.template == null || !countedBagItems.Add(item)) continue;
			if (item.template.type >= 0 && item.template.type <= 4)
			{
				if (ModSetActivator.IsSetKichHoat(item))
				{
					totalSetKHCount++;
					GameScr.info1?.addInfo("NHẶT ĐỒ KÍCH HOẠT: [" + item.template.id + "] " + item.template.name + "!", 0);
				}
				else
				{
					int stars = ModSetActivator.GetItemStarCount(item);
					if (stars > 0)
					{
						totalStarCount++;
						GameScr.info1?.addInfo("NHẶT ĐỒ SAO: [" + item.template.id + "] " + item.template.name + " (" + stars + " Sao)!", 0);
					}
				}
			}
		}
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của GetDropRatePercent.
	/// </summary>
	public static double GetDropRatePercent()
	{
		if (totalMobsKilled <= 0) return 0.0;
		return System.Math.Round(((double)totalItemsDropped / totalMobsKilled) * 100.0, 2);
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của GetMobsPerMinute.
	/// </summary>
	public static double GetMobsPerMinute()
	{
		if (sessionStartTime <= 0) return 0.0;
		long elapsedMs = mSystem.currentTimeMillis() - sessionStartTime;
		double minutes = elapsedMs / 60000.0;
		if (minutes <= 0.05) return 0.0;
		return System.Math.Round(totalMobsKilled / minutes, 1);
	}

	/// <summary>
	/// Bộ xử lý sự kiện tiếp nhận và phản hồi OnMobDied.
	/// </summary>
	public static void OnMobDied(Mob m)
	{
		if (m == null) return;
		InitSession();
		totalMobsKilled++;
	}

	/// <summary>
	/// Bộ xử lý sự kiện tiếp nhận và phản hồi OnItemSpawned.
	/// </summary>
	public static void OnItemSpawned(ItemMap item)
	{
		if (item == null) return;
		InitSession();

		Char me = Char.myCharz();
		int myId = (me != null) ? me.charID : -1;

		bool isMyItem = (item.playerId == myId || item.playerId == -1 || item.playerId == -2 || item.playerId == 0);
		if (!isMyItem)
		{
			return;
		}

		totalItemsDropped++;

		if (isInstantPick && ModAutoPick.autoPick && me != null)
		{
			if (ModAutoPick.ShouldPickItem(item) && Res.distance(me.cx, me.cy, item.x, item.y) <= 60)
			{
				me.itemFocus = item;
				Service.gI().pickItem(item.itemMapID);
			}
		}
	}

	/// <summary>
	/// Cập nhật trạng thái logic của đối tượng theo từng chu kỳ khung hình.
	/// </summary>
	public static void Update()
	{
		ScanBagForSpecialItems(Char.myCharz());
	}
}
