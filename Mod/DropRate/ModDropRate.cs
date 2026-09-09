using System;

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

	public static void InitSession()
	{
		if (sessionStartTime == 0)
		{
			sessionStartTime = mSystem.currentTimeMillis();
		}
	}

	public static void ResetStats()
	{
		totalMobsKilled = 0;
		totalItemsDropped = 0;
		totalSetKHCount = 0;
		totalStarCount = 0;
		sessionStartTime = mSystem.currentTimeMillis();
		GameScr.info1.addInfo("Đã đặt lại thống kê rơi đồ!", 0);
		SoundMn.gI().buttonClick();
	}

	public static double GetDropRatePercent()
	{
		if (totalMobsKilled <= 0) return 0.0;
		return System.Math.Round(((double)totalItemsDropped / totalMobsKilled) * 100.0, 2);
	}

	public static double GetMobsPerMinute()
	{
		if (sessionStartTime <= 0) return 0.0;
		long elapsedMs = mSystem.currentTimeMillis() - sessionStartTime;
		double minutes = elapsedMs / 60000.0;
		if (minutes <= 0.05) return 0.0;
		return System.Math.Round(totalMobsKilled / minutes, 1);
	}

	public static void OnMobDied(Mob m)
	{
		if (m == null) return;
		InitSession();
		totalMobsKilled++;
	}

	public static void OnItemSpawned(ItemMap item)
	{
		if (item == null) return;
		InitSession();

		Char me = Char.myCharz();
		int myId = (me != null) ? me.charID : -1;

		// Kiểm tra quyền sở hữu hợp lệ:
		// - Thuộc về chính nhân vật (playerId == myId)
		// - Đồ rơi tự do / của chung (playerId == -1 || playerId == 0)
		// - Đồ hào quang đặc biệt rơi từ quái như Ngọc Rồng (playerId == -2)
		bool isMyItem = (item.playerId == myId || item.playerId == -1 || item.playerId == -2 || item.playerId == 0);
		if (!isMyItem)
		{
			return;
		}

		totalItemsDropped++;

		// Phân loại vật phẩm trang bị / kích hoạt / sao
		if (item.template != null && item.template.type >= 0 && item.template.type <= 4)
		{
			// Kiểm tra nếu là trang bị
			Item fakeItem = new Item();
			fakeItem.template = item.template;

			// Kiểm tra đồ kích hoạt
			if (ModSetActivator.IsSetKichHoat(fakeItem))
			{
				totalSetKHCount++;
				GameScr.info1.addInfo("RƠI ĐỒ KÍCH HOẠT: [" + item.template.id + "] " + item.template.name + "!", 0);
			}
			else if (ModSetActivator.GetItemStarCount(fakeItem) > 0)
			{
				totalStarCount++;
				GameScr.info1.addInfo("RƠI ĐỒ SAO: [" + item.template.id + "] " + item.template.name + " (" + ModSetActivator.GetItemStarCount(fakeItem) + " Sao)!", 0);
			}
		}

		// TRICK ZERO-LATENCY INSTANT PICK: Hút đồ ngay tức thì tại tick 0
		if (isInstantPick)
		{
			if (ModAutoPick.ShouldPickItem(item))
			{
				if (me != null)
				{
					me.itemFocus = item;
				}
				Service.gI().pickItem(item.itemMapID);
			}
		}
	}

	public static void Update()
	{
		// ModDropRate update
	}
}
