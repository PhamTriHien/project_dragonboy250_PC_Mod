using System;

/// <summary>
/// Module Auto Mua Bùa: Tự động mua và gia hạn đồng thời nhiều loại bùa hỗ trợ trong 10 loại bùa tại NPC Bà Hạt Mít.
/// </summary>
public static class ModAutoBuyBua
{
	public enum AutoBuyBuaState
	{
		Idle,
		GoingToVachNui,
		ApproachingBaHatMit,
		WaitingMenu,
		WaitingShop,
		ReturningToFarm,
		SwitchingZone
	}

	private sealed class CharmMenuListener : IActionListener
	{
		public void perform(int idAction, object p)
		{
			if (idAction >= 100 && idAction < 110)
			{
				int idx = idAction - 100;
				ToggleBuaIndex(idx);
				ShowCharmChecklistMenu();
			}
			else if (idAction == 120)
			{
				buaEnabledMask = (1 << buaTypeNames.Length) - 1;
				ModConfig.SaveConfig();
				GameScr.info1.addInfo("Đã chọn mua tất cả 10 loại Bùa!", 0);
				SoundMn.gI().buttonClick();
			}
			else if (idAction == 121)
			{
				buaEnabledMask = 1 << selectedBuaType;
				ModConfig.SaveConfig();
				GameScr.info1.addInfo("Chỉ giữ Bùa: " + buaTypeNames[selectedBuaType], 0);
				SoundMn.gI().buttonClick();
			}
		}
	}

	private static readonly CharmMenuListener s_MenuListener = new CharmMenuListener();

	public static bool isAutoRebuy = false;
	public static int selectedBuaType = 0;
	public static int buaEnabledMask = 1;
	public static int selectedPackage = 0;
	public static bool showCharmHud = false;
	public static readonly long[] charmExpireTimestamps = new long[10];

	public static string[] buaTypeNames = new string[]
	{
		"Thu Hút",
		"Trí Tuệ",
		"Oai Hùng",
		"Mạnh Mẽ",
		"Da Trâu",
		"Dẻo Dai",
		"Bất Tử",
		"Đệ Tử",
		"Trí Tuệ x3",
		"Trí Tuệ x4"
	};

	public static string[] buaKeywords = new string[]
	{
		"thu hut",
		"tri tue",
		"oai hung",
		"manh me",
		"da trau",
		"deo dai",
		"bat tu",
		"de tu",
		"tri tue x3",
		"tri tue x4"
	};

	public static int[] buaTemplateIds = new int[]
	{
		219,
		213,
		216,
		214,
		215,
		218,
		217,
		522,
		671,
		672
	};

	public static string[] packageNames = new string[]
	{
		"1 Giờ",
		"8 Giờ",
		"1 Tháng"
	};

	public static AutoBuyBuaState currentState = AutoBuyBuaState.Idle;
	public static bool isBusy => currentState != AutoBuyBuaState.Idle;

	public static int savedFarmMapId = -1;
	public static int savedFarmZoneId = -1;
	public static int savedFarmX = -1;
	public static int savedFarmY = -1;

	private static long stateTimer = 0;
	private static long lastAutoCheckTime = 0;
	private static long lastShopBuyStepTime = 0;
	private static int s_BoughtSessionMask = 0;
	private static int retryOpenMenuCount = 0;
	private static int retryShopCount = 0;
	private static long lastZoneAttemptTime = 0;
	private static int zoneAttemptCount = 0;

	public static bool IsBuaSelected(int index)
	{
		if (index < 0 || index >= buaTypeNames.Length) return false;
		if (buaEnabledMask == 0)
		{
			return index == selectedBuaType;
		}
		return ((buaEnabledMask >> index) & 1) != 0;
	}

	public static int GetSelectedCount()
	{
		int count = 0;
		for (int i = 0; i < buaTypeNames.Length; i++)
		{
			if (IsBuaSelected(i)) count++;
		}
		return count;
	}

	public static string GetSelectedButtonLabel()
	{
		int count = 0;
		int singleIdx = -1;
		for (int i = 0; i < buaTypeNames.Length; i++)
		{
			if (IsBuaSelected(i))
			{
				count++;
				singleIdx = i;
			}
		}
		if (count <= 0) return "Chọn Bùa";
		if (count == 1) return buaTypeNames[singleIdx];
		return "Bùa (" + count + "/10)";
	}

	public static void ToggleBuaIndex(int index)
	{
		if (index < 0 || index >= buaTypeNames.Length) return;
		if (buaEnabledMask == 0)
		{
			buaEnabledMask = 1 << selectedBuaType;
		}
		buaEnabledMask ^= (1 << index);
		if (buaEnabledMask == 0)
		{
			buaEnabledMask = 1 << index;
		}
		selectedBuaType = index;
		ModConfig.SaveConfig();
		SoundMn.gI().buttonClick();
	}

	public static void ShowCharmChecklistMenu()
	{
		MyVector vec = new MyVector();
		for (int i = 0; i < buaTypeNames.Length; i++)
		{
			bool on = IsBuaSelected(i);
			string prefix = on ? "[x] " : "[ ] ";
			vec.addElement(new Command(prefix + buaTypeNames[i], s_MenuListener, 100 + i, null));
		}
		vec.addElement(new Command("Chọn Tất Cả (10)", s_MenuListener, 120, null));
		vec.addElement(new Command("Chỉ Giữ 1 Bùa", s_MenuListener, 121, null));
		GameCanvas.menu.startAt(vec, 3);
	}

	public static string ExportExpireTimestamps()
	{
		string res = string.Empty;
		for (int i = 0; i < charmExpireTimestamps.Length; i++)
		{
			if (i > 0) res += ";";
			res += charmExpireTimestamps[i].ToString();
		}
		return res;
	}

	public static void ImportExpireTimestamps(string raw)
	{
		if (string.IsNullOrEmpty(raw)) return;
		string[] parts = raw.Split(new char[] { ';', ',' });
		for (int i = 0; i < parts.Length && i < charmExpireTimestamps.Length; i++)
		{
			if (long.TryParse(parts[i].Trim(), out long ts) && ts >= 0)
			{
				charmExpireTimestamps[i] = ts;
			}
		}
	}

	public static int GetHomeVachNuiMapId()
	{
		Char me = Char.myCharz();
		if (me == null) return 42;
		if (me.cgender == 1) return 43;
		if (me.cgender == 2) return 44;
		return 42;
	}

	public static Npc FindBaHatMit()
	{
		if (GameScr.vNpc == null) return null;
		for (int i = 0; i < GameScr.vNpc.size(); i++)
		{
			Npc npc = (Npc)GameScr.vNpc.elementAt(i);
			if (npc != null && npc.template != null)
			{
				if (npc.template.npcTemplateId == 21)
				{
					return npc;
				}
				string name = ModNextMapData.RemoveAccents(npc.template.name ?? string.Empty).ToLower();
				if (name.Contains("hat mit") || name.Contains("ba hat mit"))
				{
					return npc;
				}
			}
		}
		if (GameScr.vNpc.size() > 0)
		{
			return (Npc)GameScr.vNpc.elementAt(0);
		}
		return null;
	}

	public static void StartBuyBuaNow()
	{
		Char me = Char.myCharz();
		if (me == null || me.cHP <= 0) return;

		savedFarmMapId = TileMap.mapID;
		savedFarmZoneId = TileMap.zoneID;
		savedFarmX = me.cx;
		savedFarmY = me.cy;

		int targetMap = GetHomeVachNuiMapId();
		currentState = AutoBuyBuaState.GoingToVachNui;
		stateTimer = mSystem.currentTimeMillis();
		s_BoughtSessionMask = 0;
		lastShopBuyStepTime = 0;
		retryOpenMenuCount = 0;
		retryShopCount = 0;

		GameScr.info1.addInfo("Bắt đầu đi mua " + GetSelectedCount() + " loại bùa tại Vách Núi...", 0);
		SoundMn.gI().buttonClick();

		if (TileMap.mapID != targetMap)
		{
			ModNextMap.StartNextMap(targetMap);
		}
	}

	public static void StopAutoBuyBua()
	{
		currentState = AutoBuyBuaState.Idle;
		if (GameCanvas.panel != null && GameCanvas.panel.isShow)
		{
			GameCanvas.panel.hideNow();
		}
		if (GameCanvas.menu != null)
		{
			GameCanvas.menu.showMenu = false;
		}
	}

	public static void ToggleAutoRebuy()
	{
		isAutoRebuy = !isAutoRebuy;
		ModConfig.SaveConfig();
		GameScr.info1.addInfo("Tự động mua lại bùa: " + (isAutoRebuy ? "BẬT" : "TẮT"), 0);
		SoundMn.gI().buttonClick();
	}

	public static void CycleBuaType()
	{
		selectedBuaType = (selectedBuaType + 1) % buaTypeNames.Length;
		buaEnabledMask = 1 << selectedBuaType;
		ModConfig.SaveConfig();
		GameScr.info1.addInfo("Chọn mua: " + buaTypeNames[selectedBuaType], 0);
		SoundMn.gI().buttonClick();
	}

	public static void CyclePackage()
	{
		selectedPackage = (selectedPackage + 1) % packageNames.Length;
		ModConfig.SaveConfig();
		GameScr.info1.addInfo("Gói bùa: " + packageNames[selectedPackage], 0);
		SoundMn.gI().buttonClick();
	}

	public static string GetStatusText()
	{
		switch (currentState)
		{
			case AutoBuyBuaState.GoingToVachNui:
				return "Đang đến Vách núi " + ModNextMap.GetMapName(GetHomeVachNuiMapId());
			case AutoBuyBuaState.ApproachingBaHatMit:
				return "Đang tiếp cận Bà Hạt Mít...";
			case AutoBuyBuaState.WaitingMenu:
				return "Đang mở menu bùa...";
			case AutoBuyBuaState.WaitingShop:
				return "Đang mua bùa (" + GetSelectedCount() + " loại)...";
			case AutoBuyBuaState.ReturningToFarm:
			case AutoBuyBuaState.SwitchingZone:
				return "Đang quay lại bãi farm...";
			default:
				return isAutoRebuy ? "Sẵn sàng (Tự gia hạn)" : "Sẵn sàng";
		}
	}

	private static long GetShopItemRemainMinutes(Item item)
	{
		if (item == null || item.itemOption == null) return 0;
		long totalMins = 0;
		for (int i = 0; i < item.itemOption.Length; i++)
		{
			ItemOption opt = item.itemOption[i];
			if (opt == null || opt.optionTemplate == null) continue;
			int oid = opt.optionTemplate.id;
			if (oid == 63 && opt.param > 0)
			{
				totalMins += (long)opt.param * 1440L;
			}
			else if (oid == 64 && opt.param > 0)
			{
				totalMins += (long)opt.param * 60L;
			}
			else if ((oid == 65 || oid == 1) && opt.param > 0)
			{
				totalMins += opt.param;
			}
		}
		return totalMins;
	}

	private static bool AnySelectedCharmNeedsRebuy(long now)
	{
		for (int i = 0; i < buaTypeNames.Length; i++)
		{
			if (!IsBuaSelected(i)) continue;
			long exp = charmExpireTimestamps[i];
			if (exp <= 0 || exp - now <= 120000L)
			{
				return true;
			}
		}
		return false;
	}

	public static void Update()
	{
		if (!ModMenu.IsInGame()) return;

		Char me = Char.myCharz();
		if (me == null || me.cHP <= 0) return;

		long now = mSystem.currentTimeMillis();

		if (isAutoRebuy && currentState == AutoBuyBuaState.Idle && !ModNextMap.isNextMapActive && !ModGoBack.isReturning && !ModSetActivator.isBusy)
		{
			if (now - lastAutoCheckTime > 30000)
			{
				lastAutoCheckTime = now;
				if ((me.luong + me.luongKhoa) >= 1 && AnySelectedCharmNeedsRebuy(now))
				{
					StartBuyBuaNow();
					return;
				}
			}
		}

		if (currentState == AutoBuyBuaState.Idle) return;

		if (now - stateTimer > 65000)
		{
			GameScr.info1.addInfo("Quá thời gian mua bùa! Quay lại bãi farm...", 0);
			ReturnToFarmSpot();
			return;
		}

		int vachMapId = GetHomeVachNuiMapId();

		switch (currentState)
		{
			case AutoBuyBuaState.GoingToVachNui:
				if (TileMap.mapID == vachMapId && !Char.isLoadingMap && !Char.ischangingMap)
				{
					if (ModNextMap.isNextMapActive)
					{
						ModNextMap.StopNextMap();
					}
					currentState = AutoBuyBuaState.ApproachingBaHatMit;
					stateTimer = now;
				}
				else if (!ModNextMap.isNextMapActive && !Char.isLoadingMap && !Char.ischangingMap)
				{
					ModNextMap.StartNextMap(vachMapId);
				}
				break;

			case AutoBuyBuaState.ApproachingBaHatMit:
				if (Char.isLoadingMap || Char.ischangingMap) break;

				Npc baHatMit = FindBaHatMit();
				if (baHatMit != null)
				{
					int dist = Res.distance(me.cx, me.cy, baHatMit.cx, baHatMit.cy);
					if (dist > 30)
					{
						ModTeleport.TeleportTo(baHatMit.cx, baHatMit.cy);
					}
					me.focusManualTo(baHatMit);

					if (now - stateTimer > 800)
					{
						stateTimer = now;
						int tmplId = (baHatMit.template != null) ? baHatMit.template.npcTemplateId : baHatMit.npcId;
						Service.gI().openMenu(tmplId);
						currentState = AutoBuyBuaState.WaitingMenu;
						retryOpenMenuCount = 0;
					}
				}
				else if (now - stateTimer > 5000)
				{
					GameScr.info1.addInfo("Không tìm thấy Bà Hạt Mít! Quay lại bãi farm...", 0);
					ReturnToFarmSpot();
				}
				break;

			case AutoBuyBuaState.WaitingMenu:
				if (GameCanvas.menu.showMenu && GameCanvas.menu.menuItems != null && GameCanvas.menu.menuItems.size() > 0)
				{
					Npc npc = FindBaHatMit();
					int tmplId = (npc != null && npc.template != null) ? npc.template.npcTemplateId : 21;

					int targetIndex = -1;
					string searchPkg = (selectedPackage == 0) ? "1 gio" : ((selectedPackage == 1) ? "8 gio" : "1 thang");

					for (int i = 0; i < GameCanvas.menu.menuItems.size(); i++)
					{
						Command cmd = (Command)GameCanvas.menu.menuItems.elementAt(i);
						if (cmd != null && !string.IsNullOrEmpty(cmd.caption))
						{
							string cap = ModNextMapData.RemoveAccents(cmd.caption).ToLower();
							if (cap.Contains(searchPkg) || (selectedPackage == 0 && cap.Contains("1h")) || (selectedPackage == 1 && cap.Contains("8h")))
							{
								targetIndex = i;
								break;
							}
						}
					}

					if (targetIndex == -1)
					{
						int shopMenuIdx = -1;
						for (int j = 0; j < GameCanvas.menu.menuItems.size(); j++)
						{
							Command cmd2 = (Command)GameCanvas.menu.menuItems.elementAt(j);
							if (cmd2 != null && !string.IsNullOrEmpty(cmd2.caption))
							{
								string cap2 = ModNextMapData.RemoveAccents(cmd2.caption).ToLower();
								if (cap2.Contains("cua hang bua") || cap2.Contains("bua"))
								{
									shopMenuIdx = j;
									break;
								}
							}
						}
						if (shopMenuIdx != -1 && retryOpenMenuCount == 0)
						{
							retryOpenMenuCount = 1;
							Service.gI().confirmMenu((short)tmplId, (sbyte)shopMenuIdx);
							GameCanvas.menu.showMenu = false;
							stateTimer = now;
							break;
						}
						targetIndex = (shopMenuIdx != -1) ? shopMenuIdx : 0;
					}

					Service.gI().confirmMenu((short)tmplId, (sbyte)targetIndex);
					GameCanvas.menu.showMenu = false;
					currentState = AutoBuyBuaState.WaitingShop;
					stateTimer = now;
					lastShopBuyStepTime = now;
					s_BoughtSessionMask = 0;
					retryShopCount = 0;
				}
				else if (now - stateTimer > 2500)
				{
					retryOpenMenuCount++;
					if (retryOpenMenuCount >= 3)
					{
						GameScr.info1.addInfo("Không mở được menu Bà Hạt Mít! Quay lại bãi...", 0);
						ReturnToFarmSpot();
					}
					else
					{
						Npc npc2 = FindBaHatMit();
						if (npc2 != null)
						{
							int tmplId2 = (npc2.template != null) ? npc2.template.npcTemplateId : npc2.npcId;
							Service.gI().openMenu(tmplId2);
						}
						stateTimer = now;
					}
				}
				break;

			case AutoBuyBuaState.WaitingShop:
				if (now - stateTimer < 650 || now - lastShopBuyStepTime < 500) break;

				if (!HasAnyCharmInShop(me))
				{
					if (now - stateTimer > 4000)
					{
						retryShopCount++;
						if (retryShopCount >= 2)
						{
							GameScr.info1.addInfo("Cửa hàng bùa chưa tải xong! Quay lại bãi...", 0);
							ReturnToFarmSpot();
						}
						else
						{
							stateTimer = now;
						}
					}
					break;
				}

				SyncAllShopCharmTimers(me, now);

				int nextCharmIdx = -1;
				Item nextCharmItem = null;
				for (int c = 0; c < buaTypeNames.Length; c++)
				{
					if (!IsBuaSelected(c)) continue;
					if ((s_BoughtSessionMask & (1 << c)) != 0) continue;

					Item shopItem = FindBuaInShopByIndex(me, c);
					if (shopItem == null || shopItem.template == null)
					{
						s_BoughtSessionMask |= (1 << c);
						continue;
					}

					long remainMins = GetShopItemRemainMinutes(shopItem);
					if (remainMins > 2)
					{
						s_BoughtSessionMask |= (1 << c);
						continue;
					}

					nextCharmIdx = c;
					nextCharmItem = shopItem;
					break;
				}

				if (nextCharmIdx >= 0 && nextCharmItem != null)
				{
					s_BoughtSessionMask |= (1 << nextCharmIdx);
					lastShopBuyStepTime = now;
					stateTimer = now;
					long pkgAddMs = (selectedPackage == 0) ? 3600000L : ((selectedPackage == 1) ? 28800000L : 2592000000L);
					long baseTs = (charmExpireTimestamps[nextCharmIdx] > now) ? charmExpireTimestamps[nextCharmIdx] : now;
					charmExpireTimestamps[nextCharmIdx] = baseTs + pkgAddMs;
					Service.gI().buyItem(1, nextCharmItem.template.id, 0);
					GameScr.info1.addInfo("ĐÃ MUA: " + buaTypeNames[nextCharmIdx] + " (" + packageNames[selectedPackage] + ")", 0);
				}
				else
				{
					ModConfig.SaveConfig();
					if (GameCanvas.panel != null && GameCanvas.panel.isShow)
					{
						GameCanvas.panel.hideNow();
					}
					ReturnToFarmSpot();
				}
				break;

			case AutoBuyBuaState.ReturningToFarm:
				if (savedFarmMapId >= 0)
				{
					if (TileMap.mapID == savedFarmMapId && !Char.isLoadingMap && !Char.ischangingMap)
					{
						if (ModNextMap.isNextMapActive)
						{
							ModNextMap.StopNextMap();
						}
						if (savedFarmZoneId >= 0 && TileMap.zoneID != savedFarmZoneId)
						{
							currentState = AutoBuyBuaState.SwitchingZone;
							stateTimer = now;
							lastZoneAttemptTime = now;
							zoneAttemptCount = 1;
							Service.gI().requestChangeZone(savedFarmZoneId, -1);
						}
						else
						{
							if (savedFarmX > 0 && savedFarmY > 0)
							{
								ModTeleport.TeleportTo(savedFarmX, savedFarmY);
							}
							currentState = AutoBuyBuaState.Idle;
							GameScr.info1.addInfo("Đã quay về bãi farm an toàn!", 0);
						}
					}
					else if (!ModNextMap.isNextMapActive && !Char.isLoadingMap && !Char.ischangingMap)
					{
						ModNextMap.StartNextMap(savedFarmMapId);
					}
				}
				else
				{
					currentState = AutoBuyBuaState.Idle;
				}
				break;

			case AutoBuyBuaState.SwitchingZone:
				if (Char.isLoadingMap || Char.ischangingMap) break;

				if (savedFarmZoneId < 0 || TileMap.zoneID == savedFarmZoneId || zoneAttemptCount >= 3)
				{
					if (savedFarmX > 0 && savedFarmY > 0)
					{
						ModTeleport.TeleportTo(savedFarmX, savedFarmY);
					}
					currentState = AutoBuyBuaState.Idle;
					GameScr.info1.addInfo("Đã quay về bãi farm an toàn!", 0);
					break;
				}

				if (now - lastZoneAttemptTime > 2500)
				{
					lastZoneAttemptTime = now;
					zoneAttemptCount++;
					Service.gI().requestChangeZone(savedFarmZoneId, -1);
				}
				break;
		}
	}

	private static bool HasAnyCharmInShop(Char me)
	{
		if (me == null || me.arrItemShop == null) return false;
		for (int c = 0; c < buaTemplateIds.Length; c++)
		{
			if (FindBuaInShopByIndex(me, c) != null) return true;
		}
		return false;
	}

	private static void SyncAllShopCharmTimers(Char me, long now)
	{
		for (int c = 0; c < buaTypeNames.Length; c++)
		{
			Item it = FindBuaInShopByIndex(me, c);
			if (it != null)
			{
				long mins = GetShopItemRemainMinutes(it);
				if ((s_BoughtSessionMask & (1 << c)) == 0)
				{
					charmExpireTimestamps[c] = (mins > 0) ? (now + mins * 60000L) : 0L;
				}
			}
		}
	}

	private static Item FindBuaInShopByIndex(Char me, int buaIdx)
	{
		if (me == null || me.arrItemShop == null || buaIdx < 0 || buaIdx >= buaTemplateIds.Length) return null;

		int targetId = buaTemplateIds[buaIdx];
		string kw = buaKeywords[buaIdx];

		for (int tab = 0; tab < me.arrItemShop.Length; tab++)
		{
			if (me.arrItemShop[tab] == null) continue;
			for (int i = 0; i < me.arrItemShop[tab].Length; i++)
			{
				Item it = me.arrItemShop[tab][i];
				if (it != null && it.template != null && it.template.id == targetId)
				{
					return it;
				}
			}
		}

		for (int tab = 0; tab < me.arrItemShop.Length; tab++)
		{
			if (me.arrItemShop[tab] == null) continue;
			for (int i = 0; i < me.arrItemShop[tab].Length; i++)
			{
				Item it = me.arrItemShop[tab][i];
				if (it != null && it.template != null && !string.IsNullOrEmpty(it.template.name))
				{
					string name = ModNextMapData.RemoveAccents(it.template.name).ToLower();
					if (!string.IsNullOrEmpty(kw) && name.Contains(kw))
					{
						return it;
					}
				}
			}
		}
		return null;
	}

	public static void PaintCharmHUD(mGraphics g)
	{
		if (!showCharmHud || !ModMenu.IsInGame() || GameCanvas.panel.isShow || ModUI.uiCustomOpen) return;
		try
		{
			g.translate(-g.getTranslateX(), -g.getTranslateY());
			g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
			long now = mSystem.currentTimeMillis();
			int x = 6 + SafeArea.Left;
			int y = 138;
			int row = 0;
			for (int i = 0; i < buaTypeNames.Length; i++)
			{
				long remainMs = charmExpireTimestamps[i] - now;
				if (remainMs <= 0 && !IsBuaSelected(i)) continue;
				string timeTxt;
				if (remainMs <= 0)
				{
					timeTxt = "Hết hạn";
				}
				else
				{
					long totalMins = remainMs / 60000L;
					timeTxt = (totalMins >= 60) ? ((totalMins / 60) + "h" + (totalMins % 60) + "p") : (totalMins + "p");
				}
				string text = "Bùa " + buaTypeNames[i] + ": " + timeTxt;
				mFont f = (remainMs > 120000L) ? mFont.tahoma_7_yellow : mFont.tahoma_7_white;
				f.drawString(g, text, x, y + row * 11, mFont.LEFT);
				row++;
			}
		}
		catch
		{
		}
	}

	private static void ReturnToFarmSpot()
	{
		if (GameCanvas.panel != null && GameCanvas.panel.isShow)
		{
			GameCanvas.panel.hideNow();
		}
		if (GameCanvas.menu != null)
		{
			GameCanvas.menu.showMenu = false;
		}

		if (savedFarmMapId >= 0)
		{
			currentState = AutoBuyBuaState.ReturningToFarm;
			stateTimer = mSystem.currentTimeMillis();
			ModNextMap.StartNextMap(savedFarmMapId);
		}
		else
		{
			currentState = AutoBuyBuaState.Idle;
		}
	}
}
