using System;

/// <summary>
/// Module GoBack: Tự động ghi nhớ vị trí và quay trở lại đúng toạ độ, khu vực và bản đồ sau khi nhân vật hồi sinh.
/// </summary>
public static class ModGoBack
{
	/// <summary>
	/// ENUM GoBackState: Định nghĩa và điều phối logic trong hệ thống game.
	/// </summary>
	public enum GoBackState
	{
		Idle,
		WaitingRevive,
		MovingHomeForMp,
		DelayAtHome,
		NavigatingToMap,
		ChangingZone,
		TeleportingToSpot
	}

	public static bool isGoBackActive = false;
	public static bool isAutoRecordOnDeath = true;
	public static bool gobackOnLowMp = false;
	public static int lowMpPercent = 10;
	public static bool eatChickenAtHome = true;

	private static long s_LastPickChickenTime = 0;

	private class LowMpInputListener : IActionListener
	{
		public void perform(int idAction, object p)
		{
			try
			{
				if (GameCanvas.inputDlg == null || GameCanvas.inputDlg.tfInput == null)
				{
					GameCanvas.endDlg();
					return;
				}
				string text = (GameCanvas.inputDlg.tfInput.getText() ?? string.Empty).Trim();
				GameCanvas.endDlg();
				if (int.TryParse(text, out int val))
				{
					lowMpPercent = System.Math.Max(1, System.Math.Min(99, val));
					ModConfig.SaveConfig();
					GameScr.info1.addInfo("Mức KI về nhà hồi phục: < " + lowMpPercent + "%", 0);
					SoundMn.gI().buttonClick();
				}
			}
			catch
			{
				GameCanvas.endDlg();
			}
		}
	}

	private static readonly LowMpInputListener s_LowMpListener = new LowMpInputListener();

	public static void ShowInputLowMpPercent()
	{
		if (GameCanvas.inputDlg == null) return;
		ModUI.uiCustomOpen = false;
		ModMenu.modMenuOpen = false;
		GameCanvas.inputDlg.show("Nhập mức % KI tự về nhà hồi phục (1 - 99%):", new Command("OK", s_LowMpListener, 1, null), TField.INPUT_TYPE_NUMERIC);
		if (GameCanvas.inputDlg.tfInput != null)
		{
			GameCanvas.inputDlg.tfInput.setMaxTextLenght(3);
			GameCanvas.inputDlg.tfInput.setText(lowMpPercent.ToString());
			if (!Main.isPC) GameCanvas.inputDlg.tfInput.setFocusWithKb(isFocus: true);
		}
	}

	public static bool HasHomeChickenOnFloor(Char me)
	{
		if (!eatChickenAtHome || me == null || GameScr.vItemMap == null)
		{
			return false;
		}
		if (TileMap.mapID != 21 && TileMap.mapID != 22 && TileMap.mapID != 23)
		{
			return false;
		}
		for (int i = 0; i < GameScr.vItemMap.size(); i++)
		{
			ItemMap itm = (ItemMap)GameScr.vItemMap.elementAt(i);
			if (itm != null && itm.template != null &&
			    (itm.template.id == 73 || itm.template.id == 74 || itm.template.id == 75) &&
			    (itm.playerId == me.charID || itm.playerId == -1))
			{
				return true;
			}
		}
		return false;
	}

	public static void CycleLowMpPercent()
	{
		if (lowMpPercent <= 10) lowMpPercent = 20;
		else if (lowMpPercent == 20) lowMpPercent = 30;
		else if (lowMpPercent == 30) lowMpPercent = 50;
		else lowMpPercent = 10;
		ModConfig.SaveConfig();
		GameScr.info1.addInfo("Mức KI về nhà hồi phục: < " + lowMpPercent + "%", 0);
		SoundMn.gI().buttonClick();
	}

	public static void TryPickHomeChicken(Char me)
	{
		if (!eatChickenAtHome || me == null || me.isDie || me.cHP <= 0 || me.statusMe == 14 || me.statusMe == 5)
		{
			return;
		}
		if (TileMap.mapID != 21 && TileMap.mapID != 22 && TileMap.mapID != 23)
		{
			return;
		}
		if (me.cHP >= me.cHPFull && me.cMP >= me.cMPFull)
		{
			return;
		}
		long now = mSystem.currentTimeMillis();
		if (now - s_LastPickChickenTime < 600 || GameScr.vItemMap == null)
		{
			return;
		}
		for (int i = 0; i < GameScr.vItemMap.size(); i++)
		{
			ItemMap itm = (ItemMap)GameScr.vItemMap.elementAt(i);
			if (itm != null && itm.template != null &&
			    (itm.template.id == 73 || itm.template.id == 74 || itm.template.id == 75) &&
			    (itm.playerId == me.charID || itm.playerId == -1))
			{
				s_LastPickChickenTime = now;
				Service.gI().pickItem(itm.itemMapID);
				break;
			}
		}
	}

	public static int savedMapId = -1;
	public static int savedZoneId = -1;
	public static int savedX = -1;
	public static int savedY = -1;

	private class CustomGoBackChatListener : IChatable
	{
		public void onChatFromMe(string text, string to)
		{
			ChatTextField.gI().isShow = false;
			ChatTextField.gI().parentScreen = GameScr.gI();
			if (string.IsNullOrEmpty(text)) return;
			string[] parts = text.Split(new char[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);
			if (parts.Length >= 1 && int.TryParse(parts[0].Trim(), out int mId))
			{
				savedMapId = mId;
				if (parts.Length >= 2 && int.TryParse(parts[1].Trim(), out int zId)) savedZoneId = zId;
				if (parts.Length >= 4 && int.TryParse(parts[2].Trim(), out int px) && int.TryParse(parts[3].Trim(), out int py))
				{
					savedX = px;
					savedY = py;
				}
				ModConfig.SaveConfig();
				GameScr.info1.addInfo("GoBack tùy chỉnh: Map " + savedMapId + " [K." + savedZoneId + "] (" + savedX + "," + savedY + ")", 0);
			}
		}
		public void onCancelChat()
		{
			ChatTextField.gI().isShow = false;
			ChatTextField.gI().parentScreen = GameScr.gI();
		}
	}

	private static readonly CustomGoBackChatListener customGoBackListener = new CustomGoBackChatListener();

	public static void ShowInputCustomGoBack()
	{
		ChatTextField.gI().strChat = "Nhập: MapID,Khu,X,Y (VD: 5,2,350,240)";
		ChatTextField.gI().tfChat.name = "MapID,Khu,X,Y";
		ChatTextField.gI().tfChat.setIputType(TField.INPUT_TYPE_ANY);
		ChatTextField.gI().startChat2(customGoBackListener, string.Empty);
		string cur = (savedMapId >= 0) ? (savedMapId + "," + System.Math.Max(0, savedZoneId) + "," + System.Math.Max(0, savedX) + "," + System.Math.Max(0, savedY)) : string.Empty;
		ChatTextField.gI().tfChat.setText(cur);
	}

	public static GoBackState currentState = GoBackState.Idle;
	public static bool isForcedHomeReturnActive = false;

	public static bool isReturning
	{
		get { return (isGoBackActive || isForcedHomeReturnActive) && currentState != GoBackState.Idle; }
		set
		{
			if (!value)
			{
				currentState = GoBackState.Idle;
				isForcedHomeReturnActive = false;
			}
		}
	}

	private static int lastAliveMapId = -1;
	private static int lastAliveZoneId = -1;
	private static int lastAliveX = -1;
	private static int lastAliveY = -1;

	private static int activeReturnMapId = -1;
	private static int activeReturnZoneId = -1;
	private static int activeReturnX = -1;
	private static int activeReturnY = -1;
	private static long lastColdKickSignalTime = 0;
	private static int forcedColdHomeReturnCount = 0;
	private static long lastForcedColdReturnTime = 0;
	private static long requiredHomeCooldownMs = 1200;

	public static void ResetColdReturnSafety()
	{
		forcedColdHomeReturnCount = 0;
		lastForcedColdReturnTime = 0;
		requiredHomeCooldownMs = 1200;
	}

	private static long stateTimer = 0;
	private static long lastZoneAttemptTime = 0;
	private static int zoneAttemptCount = 0;
	private static bool s_HasCheckedSessionEntry = false;
	private static long s_SessionEnterTime = 0;
	private static long s_LastAutoPersistTime = 0;
	private static long s_LastSpotStepTime = 0;
	private static long s_SpotArrivedTime = 0;

	private static int GetTargetMapId() => (currentState != GoBackState.Idle && activeReturnMapId >= 0) ? activeReturnMapId : savedMapId;
	private static int GetTargetZoneId() => (currentState != GoBackState.Idle && activeReturnMapId >= 0) ? activeReturnZoneId : savedZoneId;
	private static int GetTargetX() => (currentState != GoBackState.Idle && activeReturnMapId >= 0) ? activeReturnX : savedX;
	private static int GetTargetY() => (currentState != GoBackState.Idle && activeReturnMapId >= 0) ? activeReturnY : savedY;

	public static void MarkSessionEntryChecked()
	{
		s_HasCheckedSessionEntry = true;
	}

	public static void OnOutOfGame()
	{
		s_HasCheckedSessionEntry = false;
		s_SessionEnterTime = 0;
		lastAliveMapId = -1;
		lastAliveZoneId = -1;
		lastAliveX = -1;
		lastAliveY = -1;
		s_LastSpotStepTime = 0;
		s_SpotArrivedTime = 0;
		if (currentState != GoBackState.Idle || isForcedHomeReturnActive)
		{
			currentState = GoBackState.Idle;
			isForcedHomeReturnActive = false;
			if (ModNextMap.isNextMapActive)
			{
				ModNextMap.StopNextMap();
			}
		}
	}

	public static void BeginReturn(int mapId, int zoneId, int x, int y, bool forcedHomeReturn = false)
	{
		if (mapId < 0)
		{
			return;
		}
		long now = mSystem.currentTimeMillis();
		activeReturnMapId = mapId;
		activeReturnZoneId = zoneId;
		activeReturnX = x;
		activeReturnY = y;
		isForcedHomeReturnActive = forcedHomeReturn;
		if (!forcedHomeReturn)
		{
			isGoBackActive = true;
		}

		ModTanSat.currentFarmTarget = null;
		ModAutoPick.isBusy = false;
		ModNextMap.resetPenaltyCount = 0;
		s_LastSpotStepTime = 0;
		s_SpotArrivedTime = 0;

		Char me = Char.myCharz();
		bool isDead = me != null && ((me.cHPFull > 0 && me.cHP <= 0) || me.statusMe == 14 || me.statusMe == 5 || me.meDead);
		if (isDead)
		{
			if (ModNextMap.isNextMapActive)
			{
				ModNextMap.StopNextMap();
			}
			GameCanvas.endDlg();
			GameCanvas.menu.showMenu = false;
			InfoDlg.hide();
			Service.gI().returnTownFromDead();
			currentState = GoBackState.WaitingRevive;
			stateTimer = now;
			return;
		}

		if ((TileMap.mapID == 21 || TileMap.mapID == 22 || TileMap.mapID == 23) && TileMap.mapID != mapId)
		{
			if (ModNextMap.isNextMapActive)
			{
				ModNextMap.StopNextMap();
			}
			requiredHomeCooldownMs = 1200;
			currentState = GoBackState.DelayAtHome;
			stateTimer = now;
			return;
		}

		if (TileMap.mapID != mapId)
		{
			currentState = GoBackState.NavigatingToMap;
			stateTimer = now;
			ModNextMap.StartNextMap(mapId);
		}
		else
		{
			if (ModNextMap.isNextMapActive)
			{
				ModNextMap.StopNextMap();
			}
			currentState = GoBackState.ChangingZone;
			lastZoneAttemptTime = 0;
			zoneAttemptCount = 0;
			stateTimer = now;
		}
	}

	/// <summary>
	/// Nhận diện thông báo Server đẩy nhân vật khỏi hành tinh Cold ('Meninggalkan Cold Planet') để tự động quay lại map úp.
	/// </summary>
	public static void CheckServerKickMessage(string msg)
	{
		if (string.IsNullOrEmpty(msg))
		{
			return;
		}
		string lower = msg.ToLowerInvariant();
		if (lower.Contains("meninggalkan cold") || lower.Contains("leaving cold") || lower.Contains("rời khỏi hành tinh cold"))
		{
			lastColdKickSignalTime = mSystem.currentTimeMillis();
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ SaveCurrentPosition.
	/// </summary>
	public static void SaveCurrentPosition()
	{
		Char me = Char.myCharz();
		if (me == null || !ModMenu.IsInGame())
		{
			return;
		}

		savedMapId = TileMap.mapID;
		savedZoneId = TileMap.zoneID;
		savedX = me.cx;
		savedY = me.cy;
		ModConfig.SaveConfig();

		GameScr.info1.addInfo("GoBack: Đã lưu vị trí " + ModNextMap.GetMapName(savedMapId) + " [K." + savedZoneId + "] (" + savedX + ", " + savedY + ")", 0);
		SoundMn.gI().buttonClick();
	}

	/// <summary>
	/// Dọn dẹp và đặt lại trạng thái ban đầu cho ResetPosition.
	/// </summary>
	public static void ResetPosition()
	{
		savedMapId = -1;
		savedZoneId = -1;
		savedX = -1;
		savedY = -1;
		activeReturnMapId = -1;
		activeReturnZoneId = -1;
		activeReturnX = -1;
		activeReturnY = -1;
		isForcedHomeReturnActive = false;
		currentState = GoBackState.Idle;
		ModConfig.SaveConfig();

		GameScr.info1.addInfo("GoBack: Đã xóa vị trí lưu!", 0);
		SoundMn.gI().buttonClick();
	}

	/// <summary>
	/// Bật hoặc tắt trạng thái kích hoạt của ToggleGoBack.
	/// </summary>
	public static void ToggleGoBack()
	{
		isGoBackActive = !isGoBackActive;
		if (!isGoBackActive)
		{
			currentState = GoBackState.Idle;
			isForcedHomeReturnActive = false;
			if (ModNextMap.isNextMapActive)
			{
				ModNextMap.StopNextMap();
			}
		}
		else
		{
			s_HasCheckedSessionEntry = true;
			Char me = Char.myCharz();
			if (ModMenu.IsInGame() && me != null && (savedMapId < 0 || isAutoRecordOnDeath) &&
			    TileMap.mapID >= 0 && TileMap.mapID != 21 && TileMap.mapID != 22 && TileMap.mapID != 23)
			{
				savedMapId = TileMap.mapID;
				savedZoneId = TileMap.zoneID;
				savedX = me.cx;
				savedY = me.cy;
			}
		}
		ModConfig.SaveConfig();

		GameScr.info1.addInfo("GoBack: " + (isGoBackActive ? "BẬT" : "TẮT"), 0);
		SoundMn.gI().buttonClick();
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ StartGoBackNow.
	/// </summary>
	public static void StartGoBackNow()
	{
		if (savedMapId < 0)
		{
			GameScr.info1.addInfo("Chưa có vị trí GoBack được lưu!", 0);
			return;
		}

		s_HasCheckedSessionEntry = true;
		BeginReturn(savedMapId, savedZoneId, savedX, savedY, forcedHomeReturn: false);
		GameScr.info1.addInfo("GoBack: Bắt đầu di chuyển về " + ModNextMap.GetMapName(savedMapId) + "...", 0);
		SoundMn.gI().buttonClick();
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của GetStatusText.
	/// </summary>
	public static string GetStatusText()
	{
		if (!isGoBackActive && !isForcedHomeReturnActive)
		{
			return "Đang TẮT";
		}
		switch (currentState)
		{
			case GoBackState.Idle:
				return "Đang theo dõi (Chờ kiệt sức)";
			case GoBackState.WaitingRevive:
				return "Đang gửi lệnh về nhà hồi sinh...";
			case GoBackState.MovingHomeForMp:
				return "Đang về nhà hồi KI...";
			case GoBackState.DelayAtHome:
				return "Đã về nhà, đang hồi phục...";
			case GoBackState.NavigatingToMap:
				return "Đang Next Map sang: " + ModNextMap.GetMapName(GetTargetMapId());
			case GoBackState.ChangingZone:
				return "Đang đổi sang Khu " + GetTargetZoneId() + "...";
			case GoBackState.TeleportingToSpot:
				return "Đang dịch chuyển về toạ độ cũ...";
			default:
				return "Sẵn sàng";
		}
	}

	/// <summary>
	/// Cập nhật trạng thái logic của đối tượng theo từng chu kỳ khung hình.
	/// </summary>
	public static void Update()
	{
		if (!ModMenu.IsInGame() || ModAutoBuyBua.isBusy || ModSetActivator.isBusy)
		{
			return;
		}

		Char me = Char.myCharz();
		if (me == null || me.cHPFull <= 0)
		{
			return;
		}

		long now = mSystem.currentTimeMillis();
		bool isDead = (me.cHP <= 0 || me.statusMe == 14 || me.statusMe == 5 || me.meDead);

		if (!s_HasCheckedSessionEntry)
		{
			if (Char.isLoadingMap || Char.ischangingMap || me.isTeleport || me.statusMe == 16)
			{
				return;
			}
			if (s_SessionEnterTime == 0)
			{
				s_SessionEnterTime = now;
				return;
			}
			if (now - s_SessionEnterTime < 450)
			{
				return;
			}
			s_HasCheckedSessionEntry = true;

			if (isGoBackActive && savedMapId >= 0 && currentState == GoBackState.Idle)
			{
				bool needMap = TileMap.mapID != savedMapId;
				bool needZone = !needMap && savedZoneId >= 0 && TileMap.zoneID != savedZoneId;
				bool needSpot = !needMap && !needZone && savedX > 0 && savedY > 0 &&
				                (Res.abs(me.cx - savedX) > 48 || Res.abs(me.cy - savedY) > 48);
				if (isDead || needMap || needZone || needSpot)
				{
					BeginReturn(savedMapId, savedZoneId, savedX, savedY, forcedHomeReturn: false);
					GameScr.info1.addInfo("GoBack: Tự động quay lại " + ModNextMap.GetMapName(savedMapId) + "...", 0);
					return;
				}
			}
		}

		if (!isGoBackActive && !ModTanSat.autoTanSat && !isForcedHomeReturnActive)
		{
			return;
		}

		// 1. Cập nhật vị trí sống cuối cùng và phát hiện trường hợp bị Server đẩy về nhà (ví dụ hết thời gian lưu trú trên hành tinh Cold)
		if (!isDead && !Char.isLoadingMap && !Char.ischangingMap && currentState == GoBackState.Idle && !ModNextMap.isNextMapActive)
		{
			if (TileMap.mapID != 21 && TileMap.mapID != 22 && TileMap.mapID != 23)
			{
				bool strictCoordGuard = ModTeleport.IsAntiScanModeActive() || ModTeleport.IsColdPlanetMap();
				bool canSaveCoords = !strictCoordGuard || (!me.isTeleport && me.cy > 36 && !ModTeleport.IsColdPositionBlocked(me.cx, me.cy));
				if (canSaveCoords)
				{
					lastAliveMapId = TileMap.mapID;
					lastAliveZoneId = TileMap.zoneID;
					lastAliveX = me.cx;
					lastAliveY = me.cy;
					if (isGoBackActive && isAutoRecordOnDeath && ModTanSat.autoTanSat &&
					    (savedMapId != lastAliveMapId || savedZoneId != lastAliveZoneId || now - s_LastAutoPersistTime >= 10000))
					{
						savedMapId = lastAliveMapId;
						savedZoneId = lastAliveZoneId;
						savedX = lastAliveX;
						savedY = lastAliveY;
						s_LastAutoPersistTime = now;
						ModConfig.SaveConfig();
					}
				}
				else if (lastAliveMapId != TileMap.mapID)
				{
					lastAliveMapId = TileMap.mapID;
					lastAliveZoneId = TileMap.zoneID;
				}

				if (isGoBackActive && gobackOnLowMp && me.cMPFull > 0 && (me.cMP * 100L / me.cMPFull) < lowMpPercent)
				{
					if (isAutoRecordOnDeath || savedMapId < 0)
					{
						savedMapId = TileMap.mapID;
						savedZoneId = TileMap.zoneID;
						savedX = me.cx;
						savedY = me.cy;
						ModConfig.SaveConfig();
					}
					activeReturnMapId = savedMapId;
					activeReturnZoneId = savedZoneId;
					activeReturnX = savedX;
					activeReturnY = savedY;
					int homeMap = 21 + me.cgender;
					currentState = GoBackState.MovingHomeForMp;
					stateTimer = now;
					ModNextMap.StartNextMap(homeMap);
					GameScr.info1.addInfo("GoBack: KI < " + lowMpPercent + "%! Về nhà hồi phục...", 0);
					return;
				}
			}
			else if (lastAliveMapId >= 0 && lastAliveMapId != 21 && lastAliveMapId != 22 && lastAliveMapId != 23)
			{
				bool wasAntiScanKick = ModTanSat.antiScanMode &&
				                       ((lastAliveMapId != 0 && lastAliveMapId != 7 && lastAliveMapId != 14) || (now - lastColdKickSignalTime <= 15000));
				bool wasForcedFromFarMap = isGoBackActive && ModTanSat.autoTanSat && lastAliveMapId != 0 && lastAliveMapId != 7 && lastAliveMapId != 14;
				if ((ModTanSat.autoTanSat || isGoBackActive) && (wasAntiScanKick || wasForcedFromFarMap))
				{
					bool useLastAlive = !isGoBackActive || isAutoRecordOnDeath || savedMapId < 0;
					activeReturnMapId = useLastAlive ? lastAliveMapId : savedMapId;
					activeReturnZoneId = useLastAlive ? lastAliveZoneId : savedZoneId;
					activeReturnX = useLastAlive ? lastAliveX : savedX;
					activeReturnY = useLastAlive ? lastAliveY : savedY;

					if (isGoBackActive && (isAutoRecordOnDeath || savedMapId < 0))
					{
						savedMapId = activeReturnMapId;
						savedZoneId = activeReturnZoneId;
						savedX = activeReturnX;
						savedY = activeReturnY;
						ModConfig.SaveConfig();
					}

					if (ModTanSat.antiScanMode || (lastAliveMapId >= 105 && lastAliveMapId <= 110) || (now - lastColdKickSignalTime <= 15000))
					{
						if (lastForcedColdReturnTime > 0 && now - lastForcedColdReturnTime > 1200000)
						{
							forcedColdHomeReturnCount = 0;
						}
						forcedColdHomeReturnCount++;
						lastForcedColdReturnTime = now;
						ModTeleport.coldPlanetResetPoints = 0;
						requiredHomeCooldownMs = (forcedColdHomeReturnCount >= 2 || ModTeleport.totalSessionResetPoints >= 220) ? 45000 : 3500;
					}
					else
					{
						requiredHomeCooldownMs = 1200;
					}

					lastAliveMapId = -1;
					lastColdKickSignalTime = 0;
					isForcedHomeReturnActive = true;
					currentState = GoBackState.DelayAtHome;
					stateTimer = now;
					ModTdltLogger.Log("GOBACK_AUTO_RETURN", $"Server forced return to Home (map={TileMap.mapID}, coldCount={forcedColdHomeReturnCount}, waitMs={requiredHomeCooldownMs}) -> Returning to map={activeReturnMapId} [K.{activeReturnZoneId}] ({activeReturnX},{activeReturnY})");
					GameScr.info1.addInfo("Tự quay lại " + ModNextMap.GetMapName(activeReturnMapId) + " [K." + activeReturnZoneId + "]...", 0);
					return;
				}
				lastAliveMapId = -1;
			}
		}

		// 2. Xử lý khi nhân vật KIỆT SỨC (CHẾT)
		if (isDead)
		{
			if (!isGoBackActive && !isForcedHomeReturnActive)
			{
				int candidateMap = (lastAliveMapId >= 0 && lastAliveMapId != 21 && lastAliveMapId != 22 && lastAliveMapId != 23) ? lastAliveMapId : TileMap.mapID;
				bool diedWhileAntiScanTanSat = ModTanSat.autoTanSat && ModTanSat.antiScanMode &&
				                               candidateMap >= 0 && candidateMap != 21 && candidateMap != 22 && candidateMap != 23;
				if (!diedWhileAntiScanTanSat)
				{
					return;
				}
				activeReturnMapId = candidateMap;
				activeReturnZoneId = (candidateMap == lastAliveMapId && lastAliveZoneId >= 0) ? lastAliveZoneId : TileMap.zoneID;
				activeReturnX = (lastAliveX > 0) ? lastAliveX : me.cx;
				activeReturnY = (lastAliveY > 36) ? lastAliveY : -1;
				isForcedHomeReturnActive = true;
			}

			ModTanSat.currentFarmTarget = null;
			ModAutoPick.isBusy = false;
			if (ModNextMap.isNextMapActive)
			{
				ModNextMap.StopNextMap();
			}

			if (currentState == GoBackState.Idle)
			{
				if (isGoBackActive && isAutoRecordOnDeath)
				{
					if (lastAliveMapId >= 0 && lastAliveMapId != 21 && lastAliveMapId != 22 && lastAliveMapId != 23)
					{
						savedMapId = lastAliveMapId;
						savedZoneId = (lastAliveZoneId >= 0) ? lastAliveZoneId : TileMap.zoneID;
						savedX = (lastAliveX > 0) ? lastAliveX : me.cx;
						savedY = (lastAliveY > 0) ? lastAliveY : me.cy;
						ModConfig.SaveConfig();
					}
					else if (savedMapId < 0 && TileMap.mapID >= 0 && TileMap.mapID != 21 && TileMap.mapID != 22 && TileMap.mapID != 23)
					{
						savedMapId = TileMap.mapID;
						savedZoneId = TileMap.zoneID;
						savedX = me.cx;
						savedY = me.cy;
						ModConfig.SaveConfig();
					}
				}

				if (isGoBackActive && !isForcedHomeReturnActive)
				{
					activeReturnMapId = savedMapId;
					activeReturnZoneId = savedZoneId;
					activeReturnX = savedX;
					activeReturnY = savedY;
				}

				if ((activeReturnMapId < 0 ? savedMapId : activeReturnMapId) < 0)
				{
					return;
				}

				GameCanvas.endDlg();
				GameCanvas.menu.showMenu = false;
				InfoDlg.hide();

				Service.gI().returnTownFromDead();
				currentState = GoBackState.WaitingRevive;
				stateTimer = now;
				GameScr.info1.addInfo("GoBack: Kiệt sức! Đang về nhà hồi sinh...", 0);
				return;
			}
			else if (currentState != GoBackState.WaitingRevive)
			{
				GameCanvas.endDlg();
				GameCanvas.menu.showMenu = false;
				InfoDlg.hide();
				Service.gI().returnTownFromDead();
				currentState = GoBackState.WaitingRevive;
				stateTimer = now;
				return;
			}
		}

		// 3. Máy trạng thái luân chuyển (State Machine)
		int targetMapId = GetTargetMapId();
		int targetZoneId = GetTargetZoneId();
		int targetX = GetTargetX();
		int targetY = GetTargetY();

		switch (currentState)
		{
			case GoBackState.WaitingRevive:
			{
				bool isRevived = (!me.meDead && me.cHP > 0 && me.statusMe != 14 && me.statusMe != 5 && !Char.isLoadingMap && !Char.ischangingMap);
				bool isAtHome = (TileMap.mapID == 21 || TileMap.mapID == 22 || TileMap.mapID == 23);
				if (isRevived && (isAtHome || now - stateTimer >= 2200))
				{
					currentState = GoBackState.DelayAtHome;
					stateTimer = now;
					return;
				}

				if (isDead && !Char.isLoadingMap && !Char.ischangingMap && now - stateTimer > 4000)
				{
					Service.gI().returnTownFromDead();
					stateTimer = now;
				}
				break;
			}

			case GoBackState.MovingHomeForMp:
			{
				int homeMap = 21 + me.cgender;
				if (TileMap.mapID == homeMap && !Char.isLoadingMap && !Char.ischangingMap)
				{
					if (ModNextMap.isNextMapActive)
					{
						ModNextMap.StopNextMap();
					}
					currentState = GoBackState.DelayAtHome;
					stateTimer = now;
					return;
				}
				if (!ModNextMap.isNextMapActive && !Char.isLoadingMap && !Char.ischangingMap && now - stateTimer > 2500)
				{
					stateTimer = now;
					ModNextMap.StartNextMap(homeMap);
				}
				break;
			}

			case GoBackState.DelayAtHome:
			{
				if (Char.isLoadingMap || Char.ischangingMap || me.isTeleport || me.statusMe == 16)
				{
					return;
				}

				long minHomeWait = isForcedHomeReturnActive ? requiredHomeCooldownMs : 1200;
				if (now - stateTimer < minHomeWait)
				{
					if ((TileMap.mapID == 21 || TileMap.mapID == 22 || TileMap.mapID == 23) && !TileMap.tileTypeAt(me.cx, me.cy, 2) && me.statusMe == 1 && me.cy <= 48)
					{
						me.statusMe = 4;
					}
					return;
				}
				if ((TileMap.mapID == 21 || TileMap.mapID == 22 || TileMap.mapID == 23) && !TileMap.tileTypeAt(me.cx, me.cy, 2) && now - stateTimer < 2500)
				{
					if (me.statusMe == 1 && me.cy <= 48)
					{
						me.statusMe = 4;
					}
					return;
				}

				TryPickHomeChicken(me);

				bool needRecover = (me.cHPFull > 0 && me.cHP * 100L < me.cHPFull * 20L) ||
				                   (isGoBackActive && gobackOnLowMp && me.cMPFull > 0 && me.cMP * 100L < me.cMPFull * 50L);
				if (needRecover && now - stateTimer < 3000)
				{
					if (GameScr.hpPotion > 0)
					{
						GameScr.gI().doUseHP();
					}
					else if (TileMap.mapID == 21 || TileMap.mapID == 22 || TileMap.mapID == 23)
					{
						MagicTree tree = GameScr.gI().magicTree;
						if (tree != null && tree.currPeas > 0 && !tree.isUpdate)
						{
							Service.gI().openMenu(4);
							Service.gI().confirmMenu(4, 0);
						}
					}
					return;
				}

				if (targetMapId < 0)
				{
					currentState = GoBackState.Idle;
					isForcedHomeReturnActive = false;
					return;
				}

				if (TileMap.mapID == targetMapId)
				{
					currentState = GoBackState.ChangingZone;
					lastZoneAttemptTime = 0;
					zoneAttemptCount = 0;
					stateTimer = now;
				}
				else
				{
					currentState = GoBackState.NavigatingToMap;
					stateTimer = now;
					ModNextMap.StartNextMap(targetMapId);
					GameScr.info1.addInfo("GoBack: Đang đi về " + ModNextMap.GetMapName(targetMapId) + "...", 0);
				}
				break;
			}

			case GoBackState.NavigatingToMap:
			{
				if (TileMap.mapID == targetMapId && !Char.isLoadingMap && !Char.ischangingMap)
				{
					if (ModNextMap.isNextMapActive)
					{
						ModNextMap.StopNextMap();
					}
					currentState = GoBackState.ChangingZone;
					lastZoneAttemptTime = 0;
					zoneAttemptCount = 0;
					stateTimer = now;
					return;
				}

				if (!ModNextMap.isNextMapActive && TileMap.mapID != targetMapId && !Char.isLoadingMap && !Char.ischangingMap)
				{
					if (now - stateTimer > 2000)
					{
						ModNextMap.StartNextMap(targetMapId);
						stateTimer = now;
					}
				}
				break;
			}

			case GoBackState.ChangingZone:
			{
				if (Char.isLoadingMap || Char.ischangingMap || me.isTeleport || me.statusMe == 16)
				{
					return;
				}
				if (targetMapId >= 0 && TileMap.mapID != targetMapId)
				{
					currentState = GoBackState.NavigatingToMap;
					stateTimer = now;
					ModNextMap.StartNextMap(targetMapId);
					return;
				}

				if (targetZoneId < 0 || TileMap.zoneID == targetZoneId)
				{
					currentState = GoBackState.TeleportingToSpot;
					stateTimer = now;
					s_LastSpotStepTime = 0;
					s_SpotArrivedTime = 0;
					ModNextMap.resetPenaltyCount = 0;
					return;
				}

				if (now - stateTimer < 600)
				{
					return;
				}

				if (now - lastZoneAttemptTime > 2500)
				{
					lastZoneAttemptTime = now;
					zoneAttemptCount++;
					Service.gI().requestChangeZone(targetZoneId, -1);
					GameScr.info1.addInfo("GoBack: Đang vào Khu " + targetZoneId + "...", 0);
				}

				if (zoneAttemptCount >= 3)
				{
					GameScr.info1.addInfo("GoBack: Khu " + targetZoneId + " có thể đã đầy, giữ khu hiện tại!", 0);
					currentState = GoBackState.TeleportingToSpot;
					stateTimer = now;
					s_LastSpotStepTime = 0;
					s_SpotArrivedTime = 0;
					ModNextMap.resetPenaltyCount = 0;
				}
				break;
			}

			case GoBackState.TeleportingToSpot:
			{
				if (Char.isLoadingMap || Char.ischangingMap || me.isTeleport || me.statusMe == 16)
				{
					return;
				}
				if (targetMapId >= 0 && TileMap.mapID != targetMapId)
				{
					currentState = GoBackState.NavigatingToMap;
					stateTimer = now;
					ModNextMap.StartNextMap(targetMapId);
					return;
				}
				if (now - stateTimer < 350)
				{
					return;
				}

				if (targetX > 0 && targetY > 0)
				{
					int spotX = targetX;
					int spotY = targetY;
					if (TileMap.pxw > 160 && ModTanSatTargeting.IsInsideWaypoint(spotX, spotY))
					{
						if (spotX < 64) spotX = 64;
						else if (spotX > TileMap.pxw - 64) spotX = TileMap.pxw - 64;
					}
					if (!TileMap.isInAirMap() && !TileMap.tileTypeAt(spotX, spotY, 2))
					{
						int fixedY = ModTanSatTargeting.FindGroundY(spotX, spotY, 120);
						if (fixedY != -1)
						{
							spotY = fixedY;
						}
					}

					int dx = Res.abs(me.cx - spotX);
					int dy = Res.abs(me.cy - spotY);
					if (dx <= 28 && dy <= 36)
					{
						if (s_SpotArrivedTime == 0)
						{
							s_SpotArrivedTime = now;
							return;
						}
						if (now - s_SpotArrivedTime < 220)
						{
							return;
						}
					}
					else if (now - stateTimer < 8000)
					{
						s_SpotArrivedTime = 0;
						if (now - s_LastSpotStepTime < 200)
						{
							return;
						}
						s_LastSpotStepTime = now;
						ModWaypoint.MoveSteppedTo(spotX, spotY);
						return;
					}
				}

				ModNextMap.resetPenaltyCount = 0;
				ModWaypoint.ResetWaypointState();
				ModTeleport.ClearEchoes();
				ModTeleport.isWaitingServerSync = false;
				ModTeleport.isCurrentPosSynced = true;
				SoundMn.gI().buttonClose();
				GameScr.info1.addInfo("GoBack: Đã về lại vị trí cũ! (Khu " + TileMap.zoneID + ")", 0);
				currentState = GoBackState.Idle;
				isForcedHomeReturnActive = false;
				stateTimer = 0;
				break;
			}
		}
	}
}
