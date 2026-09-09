using System;

public static class ModAutoHeal
{
	public static bool autoPean = true;
	public static int autoPeanHpPercent = 30;
	public static bool lockHPMP = false;

	public static bool autoHarvestPea = true;
	public static bool autoDonateClan = true;
	public static bool autoFeedPetOnAsk = true;

	private static long lastPeanTime = 0;
	private static long lastHarvestTime = 0;
	private static long lastClanDonateTime = 0;
	private static long lastFeedPetTime = 0;

	public static int GetBeanCount()
	{
		Char me = Char.myCharz();
		if (me == null || me.arrItemBag == null)
		{
			return 0;
		}
		int count = 0;
		for (int i = 0; i < me.arrItemBag.Length; i++)
		{
			Item it = me.arrItemBag[i];
			if (it != null && it.template != null && it.template.type == 6)
			{
				count += it.quantity;
			}
		}
		return count;
	}

	public static void HarvestMagicTreeNow()
	{
		Service.gI().magicTree(1);
		lastHarvestTime = mSystem.currentTimeMillis();
		GameScr.info1.addInfo("Đã gửi yêu cầu thu hoạch đậu thần!", 0);
		SoundMn.gI().buttonClick();
	}

	public static void DonateClanNow()
	{
		if (GetBeanCount() <= 0)
		{
			GameScr.info1.addInfo("Không còn đậu trong hành trang!", 0);
			return;
		}

		if (ClanMessage.vMessage != null && ClanMessage.vMessage.size() > 0)
		{
			Char me = Char.myCharz();
			int myId = (me != null) ? me.charID : -1;
			for (int i = 0; i < ClanMessage.vMessage.size(); i++)
			{
				ClanMessage cm = (ClanMessage)ClanMessage.vMessage.elementAt(i);
				if (cm != null && cm.type == 1 && cm.playerId != myId && cm.recieve < cm.maxCap)
				{
					Service.gI().clanDonate(cm.id);
					cm.recieve++;
					lastClanDonateTime = mSystem.currentTimeMillis();
					GameScr.info1.addInfo("Đã cho đậu: " + cm.playerName, 0);
					SoundMn.gI().buttonClick();
					return;
				}
			}
		}

		GameScr.info1.addInfo("Hiện không có ai xin đậu trong bang!", 0);
	}

	public static void FeedPetBean(string reason)
	{
		long now = mSystem.currentTimeMillis();
		if (now - lastFeedPetTime < 2000)
		{
			return;
		}

		Char me = Char.myCharz();
		if (me == null || me.cHP <= 0 || me.statusMe == 14 || me.statusMe == 5)
		{
			return;
		}

		if (GetBeanCount() <= 0)
		{
			GameScr.info1.addInfo("Hết đậu thần, không thể cho đệ tử!", 0);
			return;
		}

		lastFeedPetTime = now;
		lastPeanTime = now;

		if (!me.doUsePotion())
		{
			GameScr.gI().doUseHP();
		}

		SoundMn.gI().HP_MPup();
		GameScr.info1.addInfo("Đã ăn đậu cho đệ tử! (" + reason + ")", 0);
	}

	public static void DoRealAutoHeal()
	{
		try
		{
			Char me = Char.myCharz();
			if (me == null || me.cHP <= 0 || me.statusMe == 14 || me.statusMe == 5)
			{
				return;
			}

			long now = mSystem.currentTimeMillis();

			// 1. Tự ăn đậu thần cho bản thân khi HP/MP xuống thấp
			bool needHeal = false;
			if (lockHPMP)
			{
				if (me.cHP < me.cHPFull || me.cMP < me.cMPFull)
				{
					needHeal = true;
				}
			}
			else if (autoPean)
			{
				long curHpPercent = (me.cHPFull > 0) ? (me.cHP * 100L / me.cHPFull) : 100;
				long curMpPercent = (me.cMPFull > 0) ? (me.cMP * 100L / me.cMPFull) : 100;

				if (curHpPercent < autoPeanHpPercent || curMpPercent < autoPeanHpPercent)
				{
					needHeal = true;
				}
			}

			if (needHeal)
			{
				if (now - lastPeanTime >= 1500)
				{
					lastPeanTime = now;
					if (!me.doUsePotion())
					{
						GameScr.gI().doUseHP();
					}
				}
			}

			// 2. Tự động thu đậu thần từ cây đậu
			if (autoHarvestPea)
			{
				if (GameScr.gI() != null && GameScr.gI().magicTree != null)
				{
					MagicTree mt = GameScr.gI().magicTree;
					if (mt.currPeas > 0 && now - lastHarvestTime > 5000)
					{
						lastHarvestTime = now;
						Service.gI().magicTree(1);
					}
				}
				else if (now - lastHarvestTime > 45000)
				{
					lastHarvestTime = now;
					Service.gI().magicTree(1);
				}
			}

			// 3. Tự động cho đậu bang hội khi có thành viên xin
			if (autoDonateClan && now - lastClanDonateTime >= 1500)
			{
				if (ClanMessage.vMessage != null && ClanMessage.vMessage.size() > 0 && GetBeanCount() > 0)
				{
					for (int i = 0; i < ClanMessage.vMessage.size(); i++)
					{
						ClanMessage cm = (ClanMessage)ClanMessage.vMessage.elementAt(i);
						if (cm != null && cm.type == 1 && cm.playerId != me.charID && cm.recieve < cm.maxCap)
						{
							Service.gI().clanDonate(cm.id);
							cm.recieve++;
							lastClanDonateTime = now;
							GameScr.info1.addInfo("Tự động cho đậu: " + cm.playerName, 0);
							break;
						}
					}
				}
			}

			// 4. Giám sát sinh lực đệ tử, tự ăn đậu khi máu đệ tử nguy kịch
			if (autoFeedPetOnAsk && me.havePet && Char.myPetz() != null)
			{
				Char pet = Char.myPetz();
				if (pet.cHPFull > 0 && pet.cHP > 0)
				{
					long petHpPercent = pet.cHP * 100L / pet.cHPFull;
					if (petHpPercent <= 25)
					{
						FeedPetBean("HP đệ còn " + petHpPercent + "%");
					}
				}
			}
		}
		catch
		{
		}
	}
}
