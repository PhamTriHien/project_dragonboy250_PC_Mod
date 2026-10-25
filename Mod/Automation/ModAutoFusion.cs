/// <summary>
/// Module Auto Hợp Thể: Tự động hợp thể đệ tử bằng Bông Tai Porata (ưu tiên C3 1884 -> C2 921 -> C1 454 trong hành trang) và chế độ Spam Bông Tai hồi 100% HP/KI chu kỳ 11s.
/// </summary>
public static class ModAutoFusion
{
	public static bool isEnabled;
	public static bool spamPotaraMode = false;
	public static int fusionType = 4;
	public static int intervalMs = 10000;
	private static long nextAt;
	private static long fusedSinceTime;
	private static long unfusedWaitUntil;

	private static readonly int[] PotaraItemPriority = new int[] { 1884, 921, 454 };

	public static void Configure(int type, int interval)
	{
		fusionType = type < 4 ? 4 : (type > 6 ? 6 : type);
		intervalMs = interval < 3000 ? 3000 : (interval > 3600000 ? 3600000 : interval);
		isEnabled = true;
		nextAt = 0;
		ModConfig.SaveConfig();
	}

	public static void Toggle()
	{
		isEnabled = !isEnabled;
		nextAt = 0;
		fusedSinceTime = 0;
		unfusedWaitUntil = 0;
		ModConfig.SaveConfig();
		GameScr.info1?.addInfo("Auto Hợp Thể: " + (isEnabled ? "BẬT" : "TẮT"), 0);
	}

	public static void ToggleSpamPotara()
	{
		spamPotaraMode = !spamPotaraMode;
		if (spamPotaraMode)
		{
			fusionType = 4;
			isEnabled = true;
		}
		fusedSinceTime = 0;
		unfusedWaitUntil = 0;
		nextAt = 0;
		ModConfig.SaveConfig();
		GameScr.info1?.addInfo("Spam Bông Tai (11s hồi HP/KI): " + (spamPotaraMode ? "BẬT" : "TẮT"), 0);
	}

	public static int FindBestPotaraBagIndex(Char me)
	{
		if (me == null || me.arrItemBag == null) return -1;
		for (int p = 0; p < PotaraItemPriority.Length; p++)
		{
			int targetId = PotaraItemPriority[p];
			for (int i = 0; i < me.arrItemBag.Length; i++)
			{
				Item it = me.arrItemBag[i];
				if (it != null && it.template != null && it.template.id == targetId)
				{
					return it.indexUI;
				}
			}
		}
		return -1;
	}

	public static bool UsePotaraOrFusionPacket(Char me)
	{
		if (me == null) return false;
		if (fusionType == 4)
		{
			int bagIdx = FindBestPotaraBagIndex(me);
			if (bagIdx >= 0)
			{
				Service.gI().useItem(0, 1, (sbyte)bagIdx, -1);
				return true;
			}
		}
		Service.gI().funsion((sbyte)fusionType);
		return true;
	}

	public static void Update()
	{
		Char me = Char.myCharz();
		if (!isEnabled || !ModMenu.IsInGame() || me == null || !me.havePet) return;
		if (Char.isLoadingMap || Char.ischangingMap || me.isDie || me.cHP <= 0 || me.statusMe == 14 || me.statusMe == 5) return;

		long now = mSystem.currentTimeMillis();

		// Chế độ Spam Bông Tai: duy trì hợp thể 11000ms -> tách ra -> chờ 2600ms -> hợp thể lại để hồi đầy HP/KI liên tục
		if (spamPotaraMode && fusionType == 4)
		{
			int potaraIdx = FindBestPotaraBagIndex(me);
			if (potaraIdx < 0) return;

			if (me.isFusion)
			{
				if (fusedSinceTime <= 0)
				{
					fusedSinceTime = now;
				}
				else if (now - fusedSinceTime >= 11000 && now >= nextAt)
				{
					Service.gI().useItem(0, 1, (sbyte)potaraIdx, -1);
					fusedSinceTime = 0;
					unfusedWaitUntil = now + 2600;
					nextAt = now + 1500;
				}
			}
			else
			{
				fusedSinceTime = 0;
				if (now >= unfusedWaitUntil && now >= nextAt)
				{
					Service.gI().useItem(0, 1, (sbyte)potaraIdx, -1);
					fusedSinceTime = now;
					nextAt = now + 2600;
				}
			}
			return;
		}

		if (me.isFusion) return;
		if (now < nextAt) return;
		UsePotaraOrFusionPacket(me);
		nextAt = now + intervalMs;
	}
}
