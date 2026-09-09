using System;

public static class ModGoBack
{
	public enum GoBackState
	{
		Idle,
		WaitingRevive,
		DelayAtHome,
		NavigatingToMap,
		ChangingZone,
		TeleportingToSpot
	}

	public static bool isGoBackActive = false;
	public static bool isAutoRecordOnDeath = true;

	public static int savedMapId = -1;
	public static int savedZoneId = -1;
	public static int savedX = -1;
	public static int savedY = -1;

	public static GoBackState currentState = GoBackState.Idle;

	public static bool isReturning
	{
		get { return isGoBackActive && currentState != GoBackState.Idle; }
	}

	private static int lastAliveMapId = -1;
	private static int lastAliveZoneId = -1;
	private static int lastAliveX = -1;
	private static int lastAliveY = -1;

	private static long stateTimer = 0;
	private static long lastZoneAttemptTime = 0;
	private static int zoneAttemptCount = 0;

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

	public static void ResetPosition()
	{
		savedMapId = -1;
		savedZoneId = -1;
		savedX = -1;
		savedY = -1;
		currentState = GoBackState.Idle;
		ModConfig.SaveConfig();

		GameScr.info1.addInfo("GoBack: Đã xóa vị trí lưu!", 0);
		SoundMn.gI().buttonClick();
	}

	public static void ToggleGoBack()
	{
		isGoBackActive = !isGoBackActive;
		if (!isGoBackActive)
		{
			currentState = GoBackState.Idle;
			if (ModNextMap.isNextMapActive)
			{
				ModNextMap.StopNextMap();
			}
		}
		ModConfig.SaveConfig();

		GameScr.info1.addInfo("GoBack: " + (isGoBackActive ? "BẬT" : "TẮT"), 0);
		SoundMn.gI().buttonClick();
	}

	public static void StartGoBackNow()
	{
		if (savedMapId <= 0)
		{
			GameScr.info1.addInfo("Chưa có vị trí GoBack được lưu!", 0);
			return;
		}

		isGoBackActive = true;
		if (TileMap.mapID == savedMapId)
		{
			currentState = GoBackState.ChangingZone;
			lastZoneAttemptTime = 0;
			zoneAttemptCount = 0;
		}
		else
		{
			currentState = GoBackState.NavigatingToMap;
			stateTimer = mSystem.currentTimeMillis();
			ModNextMap.StartNextMap(savedMapId);
		}

		GameScr.info1.addInfo("GoBack: Bắt đầu di chuyển về " + ModNextMap.GetMapName(savedMapId) + "...", 0);
		SoundMn.gI().buttonClick();
	}

	public static string GetStatusText()
	{
		if (!isGoBackActive)
		{
			return "Đang TẮT";
		}
		switch (currentState)
		{
			case GoBackState.Idle:
				return "Đang theo dõi (Chờ kiệt sức)";
			case GoBackState.WaitingRevive:
				return "Đang gửi lệnh về nhà hồi sinh...";
			case GoBackState.DelayAtHome:
				return "Đã về nhà, đang hồi phục...";
			case GoBackState.NavigatingToMap:
				return "Đang Next Map sang: " + ModNextMap.GetMapName(savedMapId);
			case GoBackState.ChangingZone:
				return "Đang đổi sang Khu " + savedZoneId + "...";
			case GoBackState.TeleportingToSpot:
				return "Đang dịch chuyển về toạ độ cũ...";
			default:
				return "Sẵn sàng";
		}
	}

	public static void Update()
	{
		if (!isGoBackActive || !ModMenu.IsInGame() || ModAutoBuyBua.isBusy)
		{
			return;
		}

		Char me = Char.myCharz();
		if (me == null)
		{
			return;
		}

		long now = mSystem.currentTimeMillis();
		bool isDead = (me.cHP <= 0 || me.statusMe == 14 || me.statusMe == 5 || me.meDead);

		// 1. Cập nhật vị trí sống cuối cùng khi đang bình thường (không ở nhà, không loading)
		if (!isDead && !Char.isLoadingMap && !Char.ischangingMap)
		{
			if (TileMap.mapID != 21 && TileMap.mapID != 22 && TileMap.mapID != 23)
			{
				lastAliveMapId = TileMap.mapID;
				lastAliveZoneId = TileMap.zoneID;
				lastAliveX = me.cx;
				lastAliveY = me.cy;
			}
		}

		// 2. Xử lý khi nhân vật KIỆT SỨC (CHẾT)
		if (isDead)
		{
			if (currentState == GoBackState.Idle)
			{
				// Lưu tọa độ chết nếu bật tự định vị trí khi chết
				if (isAutoRecordOnDeath)
				{
					int targetMap = (lastAliveMapId > 0) ? lastAliveMapId : TileMap.mapID;
					// Tránh lưu nhầm map nhà nếu chết ở nhà
					if (targetMap != 21 && targetMap != 22 && targetMap != 23)
					{
						savedMapId = targetMap;
						savedZoneId = (lastAliveZoneId >= 0) ? lastAliveZoneId : TileMap.zoneID;
						savedX = (lastAliveX > 0) ? lastAliveX : me.cx;
						savedY = (lastAliveY > 0) ? lastAliveY : me.cy;
						ModConfig.SaveConfig();
					}
				}

				if (savedMapId <= 0)
				{
					return;
				}

				// Đóng các bảng popup/menu chết gốc của game
				GameCanvas.endDlg();
				GameCanvas.menu.showMenu = false;
				InfoDlg.hide();

				// Gửi gói tin Service(-15) về nhà hồi sinh
				Service.gI().returnTownFromDead();
				currentState = GoBackState.WaitingRevive;
				stateTimer = now;
				GameScr.info1.addInfo("GoBack: Kiệt sức! Đang về nhà hồi sinh...", 0);
				return;
			}
			else if (currentState != GoBackState.WaitingRevive)
			{
				// Chết lại trong lúc đang trên đường quay về: Giữ nguyên đích đến, gửi lại lệnh về nhà
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
		switch (currentState)
		{
			case GoBackState.WaitingRevive:
			{
				// Kiểm tra nếu nhân vật đã sống lại thành công
				bool isRevived = (!me.meDead && me.cHP > 0 && me.statusMe != 14 && me.statusMe != 5 && !Char.isLoadingMap && !Char.ischangingMap);
				if (isRevived)
				{
					currentState = GoBackState.DelayAtHome;
					stateTimer = now;
					return;
				}

				// Watchdog: Nếu sau 4 giây vẫn chưa hồi sinh, gửi lại lệnh về nhà
				if (now - stateTimer > 4000)
				{
					Service.gI().returnTownFromDead();
					stateTimer = now;
				}
				break;
			}

			case GoBackState.DelayAtHome:
			{
				// Nghỉ 800ms tại nhà để nạp xong tài nguyên và chỉ số
				if (now - stateTimer < 800)
				{
					return;
				}

				if (savedMapId <= 0)
				{
					currentState = GoBackState.Idle;
					return;
				}

				// Nếu map đích chính là map hiện tại
				if (TileMap.mapID == savedMapId)
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
					ModNextMap.StartNextMap(savedMapId);
					GameScr.info1.addInfo("GoBack: Đang đi về " + ModNextMap.GetMapName(savedMapId) + "...", 0);
				}
				break;
			}

			case GoBackState.NavigatingToMap:
			{
				// Đã đến map đích thành công
				if (TileMap.mapID == savedMapId && !Char.isLoadingMap && !Char.ischangingMap)
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

				// Watchdog: Nếu NextMap bị dừng đột ngột trước khi đến nơi
				if (!ModNextMap.isNextMapActive && TileMap.mapID != savedMapId && !Char.isLoadingMap && !Char.ischangingMap)
				{
					if (now - stateTimer > 2000)
					{
						ModNextMap.StartNextMap(savedMapId);
						stateTimer = now;
					}
				}
				break;
			}

			case GoBackState.ChangingZone:
			{
				if (Char.isLoadingMap || Char.ischangingMap)
				{
					return;
				}

				// Đã ở đúng khu vực đích hoặc không có yêu cầu khu
				if (savedZoneId < 0 || TileMap.zoneID == savedZoneId)
				{
					currentState = GoBackState.TeleportingToSpot;
					stateTimer = now;
					return;
				}

				// Gửi yêu cầu đổi khu vực
				if (now - lastZoneAttemptTime > 2500)
				{
					lastZoneAttemptTime = now;
					zoneAttemptCount++;
					Service.gI().requestChangeZone(savedZoneId, -1);
					GameScr.info1.addInfo("GoBack: Đang vào Khu " + savedZoneId + "...", 0);
				}

				// Watchdog: Nếu quá 3 lần thử (~7.5s) mà không vào được khu (khu đầy), bỏ qua để teleport tại khu hiện tại
				if (zoneAttemptCount >= 3)
				{
					GameScr.info1.addInfo("GoBack: Khu " + savedZoneId + " có thể đã đầy, giữ khu hiện tại!", 0);
					currentState = GoBackState.TeleportingToSpot;
					stateTimer = now;
				}
				break;
			}

			case GoBackState.TeleportingToSpot:
			{
				if (Char.isLoadingMap || Char.ischangingMap)
				{
					return;
				}

				// Teleport đến toạ độ chính xác đã lưu
				if (savedX > 0 && savedY > 0)
				{
					ModTeleport.TeleportTo(savedX, savedY);
				}

				SoundMn.gI().buttonClose();
				GameScr.info1.addInfo("GoBack: Đã về lại vị trí cũ! (Khu " + TileMap.zoneID + ")", 0);
				currentState = GoBackState.Idle;
				stateTimer = 0;
				break;
			}
		}
	}
}
