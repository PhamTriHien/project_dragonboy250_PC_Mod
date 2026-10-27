using System;

/// <summary>
/// Module Auto Bơm Đậu & Hồi Phục: Tự động thu hoạch và sử dụng đậu thần để hồi máu và mana khi xuống dưới ngưỡng.
/// </summary>
public static class ModAutoHeal
{
	public static bool autoPean = true;
	public static int autoPeanHpPercent = 30;
	public static int autoPeanMpPercent = 30;
	public static long autoPeanMinHp = 0;
	public static long autoPeanMinMp = 0;
	public static bool lockHPMP = false;

	public static bool autoHarvestPea = true;
	public static bool autoDonateClan = true;
	public static string donateTargetNamesRaw = "";
	public static bool autoRequestPea = false;
	public static bool autoFeedPetOnAsk = true;
	public static int petEatBeanHpPercent = 30;
	public static int petEatBeanMpPercent = 30;

	private static long lastPeanTime = 0;
	private static long lastHarvestTime = 0;
	private static long lastBoxStockTime = 0;
	private static long lastBoxOpenProbeTime = 0;
	private static long lastClanDonateTime = 0;
	private static long lastClanRequestTime = 0;
	private static long lastFeedPetTime = 0;
	private static bool s_ReopenUiAfterPctInput = false;

	private sealed class PercentInputListener : IActionListener
	{
		private readonly int targetMode;

		public PercentInputListener(int targetMode)
		{
			this.targetMode = targetMode;
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
						if (val < 0) val = 0;
						if (val > 100) val = 100;
						switch (targetMode)
						{
							case 0:
								autoPeanHpPercent = val;
								GameScr.info1?.addInfo("Ăn đậu khi HP Sư Phụ < " + val + "%", 0);
								break;
							case 1:
								autoPeanMpPercent = val;
								GameScr.info1?.addInfo("Ăn đậu khi KI Sư Phụ < " + val + "%", 0);
								break;
							case 2:
								petEatBeanHpPercent = val;
								GameScr.info1?.addInfo("Ăn đậu khi HP Đệ Tử <= " + val + "%", 0);
								break;
							case 3:
								petEatBeanMpPercent = val;
								GameScr.info1?.addInfo("Ăn đậu khi KI Đệ Tử <= " + val + "%", 0);
								break;
						}
						ModConfig.SaveConfig();
						SoundMn.gI().buttonClick();
					}
				}
				GameCanvas.endDlg();
				if (s_ReopenUiAfterPctInput)
				{
					s_ReopenUiAfterPctInput = false;
					ModUI.uiCustomOpen = true;
				}
			}
			catch
			{
				GameCanvas.endDlg();
			}
		}
	}

	private static void ShowPercentInputDialog(int mode, string title, int currentVal)
	{
		if (GameCanvas.inputDlg == null) return;
		s_ReopenUiAfterPctInput = ModUI.uiCustomOpen;
		ModUI.uiCustomOpen = false;
		ModMenu.modMenuOpen = false;
		GameCanvas.inputDlg.show(title, new Command("OK", new PercentInputListener(mode), 1, null), TField.INPUT_TYPE_NUMERIC);
		if (GameCanvas.inputDlg.tfInput != null)
		{
			GameCanvas.inputDlg.tfInput.setMaxTextLenght(3);
			GameCanvas.inputDlg.tfInput.setText(currentVal.ToString());
			if (!Main.isPC) GameCanvas.inputDlg.tfInput.setFocusWithKb(isFocus: true);
		}
	}

	public static void ShowInputHpPercent() => ShowPercentInputDialog(0, "Nhập % HP Sư Phụ ăn đậu (0 - 100):", autoPeanHpPercent);
	public static void ShowInputMpPercent() => ShowPercentInputDialog(1, "Nhập % KI Sư Phụ ăn đậu (0 - 100):", autoPeanMpPercent);
	public static void ShowInputPetHpPercent() => ShowPercentInputDialog(2, "Nhập % HP Đệ Tử ăn đậu (0 - 100):", petEatBeanHpPercent);
	public static void ShowInputPetMpPercent() => ShowPercentInputDialog(3, "Nhập % KI Đệ Tử ăn đậu (0 - 100):", petEatBeanMpPercent);

	public static bool MatchesDonateTarget(string playerName)
	{
		if (string.IsNullOrEmpty(donateTargetNamesRaw)) return true;
		if (string.IsNullOrEmpty(playerName)) return false;
		string[] parts = donateTargetNamesRaw.Split(new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
		if (parts.Length == 0) return true;
		for (int i = 0; i < parts.Length; i++)
		{
			string kw = parts[i].Trim();
			if (kw.Length > 0 && playerName.IndexOf(kw, StringComparison.OrdinalIgnoreCase) >= 0)
			{
				return true;
			}
		}
		return false;
	}

	private class DonateFilterChatListener : IChatable
	{
		public void onChatFromMe(string text, string to)
		{
			donateTargetNamesRaw = (text ?? string.Empty).Trim();
			ModConfig.SaveConfig();
			ChatTextField.gI().isShow = false;
			ChatTextField.gI().parentScreen = GameScr.gI();
			GameScr.info1.addInfo(string.IsNullOrEmpty(donateTargetNamesRaw) ? "Cho đậu bang: Tất cả thành viên" : ("Chỉ cho đậu: " + donateTargetNamesRaw), 0);
		}
		public void onCancelChat()
		{
			ChatTextField.gI().isShow = false;
			ChatTextField.gI().parentScreen = GameScr.gI();
		}
	}

	private static readonly DonateFilterChatListener donateChatListener = new DonateFilterChatListener();

	public static void ShowInputDonateTarget()
	{
		ChatTextField.gI().strChat = "Nhập tên TV được cho đậu (cách nhau dấu ;)";
		ChatTextField.gI().tfChat.name = "Tên thành viên (để trống = tất cả)";
		ChatTextField.gI().tfChat.setIputType(TField.INPUT_TYPE_ANY);
		ChatTextField.gI().startChat2(donateChatListener, string.Empty);
		ChatTextField.gI().tfChat.setText(donateTargetNamesRaw ?? string.Empty);
	}

	private class MinThresholdChatListener : IChatable
	{
		public bool isMp;

		public void onChatFromMe(string text, string to)
		{
			ChatTextField.gI().isShow = false;
			ChatTextField.gI().parentScreen = GameScr.gI();
			long val = 0;
			if (!string.IsNullOrEmpty(text))
			{
				long.TryParse(text.Trim(), out val);
				if (val < 0) val = 0;
			}
			if (isMp)
			{
				autoPeanMinMp = val;
				GameScr.info1.addInfo((val > 0) ? ("Ăn đậu khi KI < " + Res.formatNumber(val)) : "Đã tắt mức KI tuyệt đối", 0);
			}
			else
			{
				autoPeanMinHp = val;
				GameScr.info1.addInfo((val > 0) ? ("Ăn đậu khi HP < " + Res.formatNumber(val)) : "Đã tắt mức HP tuyệt đối", 0);
			}
			ModConfig.SaveConfig();
		}

		public void onCancelChat()
		{
			ChatTextField.gI().isShow = false;
			ChatTextField.gI().parentScreen = GameScr.gI();
		}
	}

	private static readonly MinThresholdChatListener s_MinThresholdListener = new MinThresholdChatListener();

	public static void ShowInputMinHp()
	{
		s_MinThresholdListener.isMp = false;
		ChatTextField.gI().strChat = "Nhập mức HP tuyệt đối ăn đậu (0 = tắt)";
		ChatTextField.gI().tfChat.name = "HP tối thiểu (0 = tắt)";
		ChatTextField.gI().tfChat.setIputType(TField.INPUT_TYPE_NUMERIC);
		ChatTextField.gI().startChat2(s_MinThresholdListener, string.Empty);
		ChatTextField.gI().tfChat.setText((autoPeanMinHp > 0) ? autoPeanMinHp.ToString() : "0");
	}

	public static void ShowInputMinMp()
	{
		s_MinThresholdListener.isMp = true;
		ChatTextField.gI().strChat = "Nhập mức KI tuyệt đối ăn đậu (0 = tắt)";
		ChatTextField.gI().tfChat.name = "KI tối thiểu (0 = tắt)";
		ChatTextField.gI().tfChat.setIputType(TField.INPUT_TYPE_NUMERIC);
		ChatTextField.gI().startChat2(s_MinThresholdListener, string.Empty);
		ChatTextField.gI().tfChat.setText((autoPeanMinMp > 0) ? autoPeanMinMp.ToString() : "0");
	}

	public static void CyclePetEatBeanPercent()
	{
		if (petEatBeanHpPercent <= 20) petEatBeanHpPercent = 30;
		else if (petEatBeanHpPercent == 30) petEatBeanHpPercent = 50;
		else if (petEatBeanHpPercent == 50) petEatBeanHpPercent = 70;
		else petEatBeanHpPercent = 20;
		petEatBeanMpPercent = petEatBeanHpPercent;
		ModConfig.SaveConfig();
		SoundMn.gI().buttonClick();
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của GetBeanCount.
	/// </summary>
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

	/// <summary>
	/// Truy xuất và trả về dữ liệu của GetBoxBeanCount.
	/// </summary>
	public static int GetBoxBeanCount()
	{
		Char me = Char.myCharz();
		if (me == null || me.arrItemBox == null)
		{
			return 0;
		}
		int count = 0;
		for (int i = 0; i < me.arrItemBox.Length; i++)
		{
			Item it = me.arrItemBox[i];
			if (it != null && it.template != null && it.template.type == 6)
			{
				count += it.quantity;
			}
		}
		return count;
	}

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của IsBeanFull.
	/// </summary>
	public static bool IsBeanFull()
	{
		Char me = Char.myCharz();
		if (me == null || me.arrItemBag == null)
		{
			return true;
		}

		MagicTree mt = (GameScr.gI() != null) ? GameScr.gI().magicTree : null;
		int maxCapacity = (mt != null && mt.maxPeas > 0) ? mt.maxPeas : 99;

		int currentBagBeans = GetBeanCount();
		if (currentBagBeans >= maxCapacity)
		{
			return true;
		}

		int emptyBagSlots = 0;
		bool hasBeanSlot = false;
		for (int i = 0; i < me.arrItemBag.Length; i++)
		{
			Item it = me.arrItemBag[i];
			if (it == null)
			{
				emptyBagSlots++;
			}
			else if (it.template != null && it.template.type == 6)
			{
				hasBeanSlot = true;
			}
		}

		if (!hasBeanSlot && emptyBagSlots == 0)
		{
			return true;
		}

		return false;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ HarvestMagicTreeNow.
	/// </summary>
	public static void HarvestMagicTreeNow()
	{
		if (IsBeanFull())
		{
			GameScr.info1.addInfo("Đậu thần trong hành trang/rương đã đầy!", 0);
			SoundMn.gI().buttonClick();
			return;
		}
		if (GameScr.gI() != null && GameScr.gI().magicTree != null && GameScr.gI().magicTree.currPeas <= 0)
		{
			GameScr.info1.addInfo("Cây đậu hiện chưa có hạt nào!", 0);
			SoundMn.gI().buttonClick();
			return;
		}
		Service.gI().magicTree(1);
		lastHarvestTime = mSystem.currentTimeMillis();
		GameScr.info1.addInfo("Đã gửi yêu cầu thu hoạch đậu thần!", 0);
		SoundMn.gI().buttonClick();
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ DonateClanNow.
	/// </summary>
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
				if (cm != null && cm.type == 1 && cm.playerId != myId && cm.recieve < cm.maxCap && MatchesDonateTarget(cm.playerName))
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

	/// <summary>
	/// Thực thi phương thức nghiệp vụ RequestClanPeaNow.
	/// </summary>
	public static void RequestClanPeaNow()
	{
		Char me = Char.myCharz();
		if (me == null || me.clanID == -1)
		{
			GameScr.info1.addInfo("Bạn hiện chưa gia nhập bang hội!", 0);
			SoundMn.gI().buttonClick();
			return;
		}
		Service.gI().clanMessage(1, string.Empty, -1);
		lastClanRequestTime = mSystem.currentTimeMillis();
		GameScr.info1.addInfo("Đã gửi yêu cầu xin đậu bang hội!", 0);
		SoundMn.gI().buttonClick();
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ FeedPetBean.
	/// </summary>
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

		Char petInfo = Char.myPetz();
		if (petInfo != null && petInfo.cHPFull > 0)
		{
			petInfo.cHP = petInfo.cHPFull;
			petInfo.cMP = petInfo.cMPFull;
		}
		Char mapPet = GameScr.findCharInMap(-me.charID);
		if (mapPet != null && mapPet.cHPFull > 0)
		{
			mapPet.cHP = mapPet.cHPFull;
		}

		SoundMn.gI().HP_MPup();
		GameScr.info1.addInfo("Đã ăn đậu cho đệ tử! (" + reason + ")", 0);
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ DoRealAutoHeal.
	/// </summary>
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
			ModGoBack.TryPickHomeChicken(me);

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

				if (curHpPercent < autoPeanHpPercent || curMpPercent < autoPeanMpPercent ||
				    (autoPeanMinHp > 0 && me.cHP < autoPeanMinHp) || (autoPeanMinMp > 0 && me.cMP < autoPeanMinMp))
				{
					needHeal = true;
				}
				else if (ModTeleport.IsColdPlanetAntiCheckActive() && !TileMap.tileTypeAt(me.cx, me.cy, TileMap.T_TOP) && curMpPercent < 10)
				{
					needHeal = true;
				}
			}

			if (needHeal && GetBeanCount() > 0)
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

			// 2. Tự động thu đậu thần từ cây đậu khi về map nhà và dự trữ tối thiểu 20 hạt vào Rương Nhà
			if (autoHarvestPea)
			{
				bool isHomeMap = (GameScr.gI() != null && GameScr.gI().magicTree != null)
					|| TileMap.mapID == 21 || TileMap.mapID == 22 || TileMap.mapID == 23;

				if (isHomeMap)
				{
					if (!IsBeanFull() && GameScr.gI() != null && GameScr.gI().magicTree != null)
					{
						MagicTree mt = GameScr.gI().magicTree;
						if (mt.currPeas > 0 && now - lastHarvestTime > 4000)
						{
							lastHarvestTime = now;
							Service.gI().magicTree(1);
						}
					}

					if (!ModSetActivator.isBusy && !ModNextMap.isNextMapActive && !ModGoBack.isReturning && GetBeanCount() > 5)
					{
						if (me.arrItemBox == null)
						{
							if (now - lastBoxOpenProbeTime > 8000 && ModSetActivator.FindHomeBoxNpc() != null)
							{
								lastBoxOpenProbeTime = now;
								Service.gI().openMenu(3);
							}
						}
						else if (GetBoxBeanCount() < 20 && ModSetActivator.GetFreeBoxSlots() != 0 && now - lastBoxStockTime > 1200 && me.arrItemBag != null)
						{
							for (int b = 0; b < me.arrItemBag.Length; b++)
							{
								Item bagIt = me.arrItemBag[b];
								if (bagIt != null && bagIt.template != null && bagIt.template.type == 6)
								{
									lastBoxStockTime = now;
									Service.gI().getItem(1, (sbyte)b);
									break;
								}
							}
						}
					}
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
						if (cm != null && cm.type == 1 && cm.playerId != me.charID && cm.recieve < cm.maxCap && MatchesDonateTarget(cm.playerName))
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

			// 4. Tự động xin đậu bang hội theo chu kỳ 5 phút
			if (autoRequestPea && me.clanID != -1 && !IsBeanFull() && now - lastClanRequestTime >= 301000L)
			{
				lastClanRequestTime = now;
				Service.gI().clanMessage(1, string.Empty, -1);
				GameScr.info1.addInfo("Tự động xin đậu bang hội!", 0);
			}

			// 5. Giám sát sinh lực đệ tử thực tế trong map, tự ăn đậu khi máu/KI đệ tử xuống thấp
			if (autoFeedPetOnAsk && me.havePet)
			{
				Char mapPet = GameScr.findCharInMap(-me.charID);
				if (mapPet != null && mapPet.cHPFull > 0 && mapPet.cHP > 0)
				{
					long petHpPercent = mapPet.cHP * 100L / mapPet.cHPFull;
					if (petHpPercent <= petEatBeanHpPercent)
					{
						FeedPetBean("HP đệ còn " + petHpPercent + "%");
					}
				}
				Char myPet = Char.myPetz();
				if (myPet != null && myPet.cMPFull > 0 && myPet.cMP > 0 && myPet.cHP > 0)
				{
					long petMpPercent = myPet.cMP * 100L / myPet.cMPFull;
					if (petMpPercent <= petEatBeanMpPercent)
					{
						FeedPetBean("KI đệ còn " + petMpPercent + "%");
					}
				}
			}
		}
		catch
		{
		}
	}
}
