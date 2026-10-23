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

	public static bool isReturning
	{
		get { return isGoBackActive && currentState != GoBackState.Idle; }
		set { if (!value) currentState = GoBackState.Idle; }
	}

	private static int lastAliveMapId = -1;
	private static int lastAliveZoneId = -1;
	private static int lastAliveX = -1;
	private static int lastAliveY = -1;

	private static long stateTimer = 0;
	private static long lastZoneAttemptTime = 0;
	private static int zoneAttemptCount = 0;

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
			if (ModNextMap.isNextMapActive)
			{
				ModNextMap.StopNextMap();
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

	/// <summary>
	/// Truy xuất và trả về dữ liệu của GetStatusText.
	/// </summary>
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
			case GoBackState.MovingHomeForMp:
				return "Đang về nhà hồi KI...";
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

	/// <summary>
	/// Cập nhật trạng thái logic của đối tượng theo từng chu kỳ khung hình.
	/// </summary>
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

		// 1. Cập nhật vị trí sống cuối cùng chỉ khi đang ở trạng thái bình thường (Idle, không đang NextMap hay bán đồ/mua bùa)
		if (!isDead && !Char.isLoadingMap && !Char.ischangingMap && currentState == GoBackState.Idle && !ModNextMap.isNextMapActive && !ModSetActivator.isBusy && !ModAutoBuyBua.isBusy)
		{
			if (TileMap.mapID != 21 && TileMap.mapID != 22 && TileMap.mapID != 23)
			{
				lastAliveMapId = TileMap.mapID;
				lastAliveZoneId = TileMap.zoneID;
				lastAliveX = me.cx;
				lastAliveY = me.cy;

				if (gobackOnLowMp && me.cMPFull > 0 && (me.cMP * 100L / me.cMPFull) < lowMpPercent)
				{
					if (isAutoRecordOnDeath || savedMapId < 0)
					{
						savedMapId = TileMap.mapID;
						savedZoneId = TileMap.zoneID;
						savedX = me.cx;
						savedY = me.cy;
						ModConfig.SaveConfig();
					}
					int homeMap = 21 + me.cgender;
					currentState = GoBackState.MovingHomeForMp;
					stateTimer = now;
					ModNextMap.StartNextMap(homeMap);
					GameScr.info1.addInfo("GoBack: KI < " + lowMpPercent + "%! Về nhà hồi phục...", 0);
					return;
				}
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
					int targetMap = (lastAliveMapId >= 0) ? lastAliveMapId : TileMap.mapID;
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

				if (savedMapId < 0)
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
				// Nghỉ tối thiểu 800ms tại nhà để nạp xong tài nguyên và chỉ số
				if (now - stateTimer < 800)
				{
					return;
				}

				if ((TileMap.mapID == 21 || TileMap.mapID == 22 || TileMap.mapID == 23) && GameScr.vItemMap != null)
				{
					for (int i = 0; i < GameScr.vItemMap.size(); i++)
					{
						ItemMap itm = (ItemMap)GameScr.vItemMap.elementAt(i);
						if (itm != null && itm.template != null && itm.template.id == 73)
						{
							Service.gI().pickItem(itm.itemMapID);
							break;
						}
					}
				}

				// Đảm bảo hồi phục HP/KI trước khi rời khỏi nhà (chờ tối đa 3000ms nếu đang hồi đậu/thu hoạch cây đậu)
				bool needRecover = (me.cHPFull > 0 && me.cHP * 100L < me.cHPFull * 20L) ||
				                   (gobackOnLowMp && me.cMPFull > 0 && me.cMP * 100L < me.cMPFull * 50L);
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

				if (savedMapId < 0)
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
