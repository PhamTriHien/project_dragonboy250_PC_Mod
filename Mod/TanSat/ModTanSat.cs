using System;
using System.Collections.Generic;

/// <summary>
/// Module Tàn Sát Pro: Tự động tìm kiếm, khóa mục tiêu, dịch chuyển theo nhịp mặt đất chuẩn NextMap (chống kẹt map / RESET_POINT) và tiêu diệt quái/boss.
/// </summary>
public static class ModTanSat
{
	private static bool _autoTanSat = false;
	public static bool autoTanSat
	{
		get { return _autoTanSat; }
		set
		{
			bool wasActive = _autoTanSat;
			_autoTanSat = value;
			if (value && !wasActive)
			{
				ModAutoPick.autoPick = true;
			}
			else if (!value)
			{
				currentFarmTarget = null;
				lastTeleportTargetMobId = -1;
				resetPenaltyCount = 0;
				lastResetPenaltyTime = 0;
				consecutiveConfirmedHits = 0;
				currentTargetResetCount = 0;
				currentTargetUnchangedAttackCount = 0;
				pendingAttackPacket = false;
				pointMoveStuckSince = 0;
				chargeStuckSince = 0;
				ModTeleport.ClearEchoes();
				ignoredMobUntil.Clear();
			}
		}
	}
	public static bool autoAttack
	{
		get { return autoTanSat; }
		set { autoTanSat = value; }
	}

	public static bool useTeleport = true;
	public static bool antiScanMode = false;
	public static bool avoidSuperMob = false;
	public static bool autoCheckStamina = true;
	public static bool autoChangeZone = false;
	public static bool autoSupportSkills = false;
	public static bool freezeSkillCooldown = false;
	public static bool checkLagMob = true;
	public static bool autoResetZone = false;
	public static int resetZoneMinutes = 15;
	public static bool showGoldTrain = false;
	public static bool trainPets = false;
	public static bool autoChangeFocus = false;

	public static void SetAntiScanMode(bool enabled)
	{
		antiScanMode = enabled;
		if (enabled && !useTeleport)
		{
			useTeleport = true;
		}
		lastTeleportTargetMobId = -1;
		currentTargetUnchangedAttackCount = 0;
		lastResyncAttackCount = 0;
		ModTeleport.ClearEchoes();
		ModTeleport.ResetColdStepTracking();
	}

	public static long goldTrainTotal = 0;
	public static long goldTrainStartTime = 0;
	private static long lastTrackedXu = -1;
	private static int lastTrackedCharId = -1;
	private static long lastResetZoneTime = 0;
	public static readonly HashSet<int> frozenSkillIds = new HashSet<int>();
	public static readonly Dictionary<int, int> customSkillDelays = new Dictionary<int, int>();
	private static int editingSkillDelayId = -1;
	private static string editingSkillDelayName = string.Empty;
	private static bool reopenUiAfterSkillDelayInput = false;

	public static void ToggleSkillFrozen(int skillTplId)
	{
		if (skillTplId < 0) return;
		bool nowFrozen;
		if (frozenSkillIds.Contains(skillTplId))
		{
			frozenSkillIds.Remove(skillTplId);
			nowFrozen = false;
		}
		else
		{
			frozenSkillIds.Add(skillTplId);
			nowFrozen = true;
		}
		ModConfig.SaveConfig();
		Skill sk = ModTanSatFilter.FindSkillByTemplateId(skillTplId);
		string name = (sk != null && sk.template != null && !string.IsNullOrEmpty(sk.template.name)) ? sk.template.name : ("Skill #" + skillTplId);
		GameScr.info1?.addInfo("Đóng băng chiêu [" + name + "]: " + (nowFrozen ? "BẬT" : "TẮT"), 0);
	}

	public static void ShowInputCustomSkillDelay(int skillTplId, string skillName)
	{
		if (skillTplId < 0 || GameCanvas.inputDlg == null) return;
		editingSkillDelayId = skillTplId;
		editingSkillDelayName = string.IsNullOrEmpty(skillName) ? ("Skill #" + skillTplId) : skillName;
		reopenUiAfterSkillDelayInput = ModUI.uiCustomOpen;
		ModUI.uiCustomOpen = false;
		ModMenu.modMenuOpen = false;
		int curVal = 0;
		customSkillDelays.TryGetValue(skillTplId, out curVal);
		GameCanvas.inputDlg.show("Độ trễ riêng [" + editingSkillDelayName + "] (ms, 0 = Mặc định):", new Command("OK", new SkillDelayActionListener(), 1, null), TField.INPUT_TYPE_NUMERIC);
		if (GameCanvas.inputDlg.tfInput != null)
		{
			GameCanvas.inputDlg.tfInput.setMaxTextLenght(7);
			GameCanvas.inputDlg.tfInput.setText(curVal > 0 ? curVal.ToString() : "0");
			if (!Main.isPC) GameCanvas.inputDlg.tfInput.setFocusWithKb(isFocus: true);
		}
	}

	private class SkillDelayActionListener : IActionListener
	{
		public void perform(int idAction, object p)
		{
			try
			{
				if (idAction == 1 && GameCanvas.inputDlg != null && GameCanvas.inputDlg.tfInput != null && editingSkillDelayId >= 0)
				{
					string raw = (GameCanvas.inputDlg.tfInput.getText() ?? string.Empty).Trim();
					if (int.TryParse(raw, out int ms) && ms > 0)
					{
						if (ms < 50) ms = 50;
						if (ms > 600000) ms = 600000;
						customSkillDelays[editingSkillDelayId] = ms;
						GameScr.info1?.addInfo("Độ trễ [" + editingSkillDelayName + "]: " + ms + "ms", 0);
					}
					else
					{
						customSkillDelays.Remove(editingSkillDelayId);
						GameScr.info1?.addInfo("Độ trễ [" + editingSkillDelayName + "]: Mặc định (" + timeAttack + "ms)", 0);
					}
					ModConfig.SaveConfig();
					SoundMn.gI().buttonClick();
				}
				GameCanvas.endDlg();
				if (reopenUiAfterSkillDelayInput)
				{
					reopenUiAfterSkillDelayInput = false;
					ModUI.uiCustomOpen = true;
				}
			}
			catch
			{
				GameCanvas.endDlg();
			}
		}
	}

	public static Mob currentFarmTarget = null;
	public static long targetLockTime = 0;
	public static long targetLastHp = 0;
	public static int lastSentSkillTemplateId = -1;
	public static long lastManualMoveTime = 0;
	public static long lastAttackTime = 0;
	public static long lastConfirmedAttackPacketTime = 0;
	private static long prevConfirmedAttackStartTime = 0;
	private static bool pendingAttackPacket = false;
	public static int timeAttack = 300;
	public static int lockedSkillTemplateId = -1;
	public static int lastTeleportTargetMobId = -1;
	public static long lastTeleportTime = 0;

	public static int lastMapId = -1;
	public static int resetPenaltyCount = 0;
	public static long lastResetPenaltyTime = 0;
	private static int consecutiveConfirmedHits = 0;
	public static int currentTargetResetCount = 0;
	public static int currentTargetUnchangedAttackCount = 0;
	private static int lastResyncAttackCount = 0;
	public static long waitUntilTime = 0;
	public static long lastStepMoveTime = 0;
	private static long changingMapStuckSince = 0;
	private static int lastPointMoveX = -1;
	private static int lastPointMoveY = -1;
	private static long pointMoveStuckSince = 0;
	private static long chargeStuckSince = 0;
	private static long lastStaminaUseTime = 0;
	private static long lastAutoZoneChangeTime = 0;
	private static long ksDetectedSince = 0;
	private static long lastSupportSkillTime = 0;
	private static readonly Dictionary<int, long> ignoredMobUntil = new Dictionary<int, long>();

	/// <summary>
	/// Phản hồi khi máy chủ gửi gói RESET_POINT (cmd 46) trong lúc đang Tàn Sát:
	/// Giảm bước nhảy xuống mức an toàn (MoveSteppedTo) và giữ nguyên mục tiêu trừ khi bị kẹt địa hình quá 6 lần liên tiếp.
	/// </summary>
	public static void OnServerResetPoint()
	{
		long now = mSystem.currentTimeMillis();
		resetPenaltyCount++;
		lastResetPenaltyTime = now;
		consecutiveConfirmedHits = 0;
		currentTargetResetCount++;
		lastTeleportTargetMobId = -1;
		waitUntilTime = now + 180;
		lastStepMoveTime = now;
		ModTeleport.lastStepTime = now;
		ModTeleport.lastStepDistance = 0;
		ModTeleport.isWaitingSettle = false;
		ModTeleport.isCurrentPosSynced = false;
		targetLockTime = now;

		ModTdltLogger.Log("RESET_POINT", $"Server RESET_POINT received -> pos=({Char.myCharz()?.cx},{Char.myCharz()?.cy}), penalty={resetPenaltyCount}, targetReset={currentTargetResetCount}");

		if (currentTargetResetCount >= 6 && currentFarmTarget != null)
		{
			ignoredMobUntil[currentFarmTarget.mobId] = now + 5000L;
			currentFarmTarget = null;
			currentTargetResetCount = 0;
			currentTargetUnchangedAttackCount = 0;
		}
	}

	/// <summary>
	/// Phản hồi khi máy chủ xác nhận thành công tọa độ dịch chuyển (RESET_POINT_CONFIRM):
	/// Làm mới bộ đếm thời gian khóa mục tiêu để bắt đầu chu kỳ tấn công chính xác sau đồng bộ.
	/// </summary>
	public static void OnTeleportSyncConfirmed()
	{
		long now = mSystem.currentTimeMillis();
		targetLockTime = now;
		currentTargetUnchangedAttackCount = 0;
		lastResyncAttackCount = 0;
	}

	public static void OnAttackPacketSent()
	{
		long now = mSystem.currentTimeMillis();
		pendingAttackPacket = false;
		lastConfirmedAttackPacketTime = now;
		prevConfirmedAttackStartTime = (lastAttackTime > 0) ? lastAttackTime : now;
		if (autoTanSat)
		{
			currentTargetUnchangedAttackCount++;
		}
	}

	public static void OnPendingAttackAborted()
	{
		if (!pendingAttackPacket)
		{
			return;
		}
		pendingAttackPacket = false;
		Char me = Char.myCharz();
		if (me != null && (me.hasSendAttack || me.dart != null || me.arr != null))
		{
			return;
		}
		long restoreTime = System.Math.Max(prevConfirmedAttackStartTime, lastConfirmedAttackPacketTime - 180);
		if (restoreTime < 0)
		{
			restoreTime = 0;
		}
		if (lastAttackTime > restoreTime)
		{
			lastAttackTime = restoreTime;
		}
		if (me != null && me.myskill != null && me.myskill.lastTimeUseThisSkill > restoreTime)
		{
			me.myskill.lastTimeUseThisSkill = restoreTime;
		}
		Skill bestSkill = ModTanSatFilter.GetBestSkillToUse();
		if (bestSkill != null && bestSkill.lastTimeUseThisSkill > restoreTime)
		{
			bestSkill.lastTimeUseThisSkill = restoreTime;
		}
	}

	private static bool HasHeldMoveKeys()
	{
		if (GameCanvas.keyHold == null)
		{
			return false;
		}
		return GameCanvas.keyHold[21] || GameCanvas.keyHold[22] || GameCanvas.keyHold[23] || GameCanvas.keyHold[24] ||
		       GameCanvas.keyHold[2] || GameCanvas.keyHold[8] || GameCanvas.keyHold[4] || GameCanvas.keyHold[6] ||
		       GameCanvas.keyHold[1] || GameCanvas.keyHold[3];
	}

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của IsManualMoving.
	/// </summary>
	public static bool IsManualMoving()
	{
		Char me = Char.myCharz();
		if (me == null) return false;

		bool keyMove = HasHeldMoveKeys();
		bool pointMove = (me.currentMovePoint != null || (me.vMovePoints != null && me.vMovePoints.size() > 0));

		return keyMove || pointMove;
	}

	// Backward-compatible delegates to ModTanSatFilter
	public static int selectedMobTemplateId
	{
		get { return ModTanSatFilter.selectedMobTemplateId; }
		set { ModTanSatFilter.selectedMobTemplateId = value; }
	}
	public static bool selectAllMobs
	{
		get { return ModTanSatFilter.selectAllMobs; }
		set { ModTanSatFilter.selectAllMobs = value; }
	}
	public static List<int> tickedMobTemplateIds => ModTanSatFilter.tickedMobTemplateIds;

	public static bool selectAllSkills
	{
		get { return ModTanSatFilter.selectAllSkills; }
		set { ModTanSatFilter.selectAllSkills = value; }
	}
	public static List<int> tickedSkillTemplateIds => ModTanSatFilter.tickedSkillTemplateIds;

	public static string GetSelectedMobName() => ModTanSatFilter.GetSelectedMobName();
	public static void CycleMobSelection(int dir) => ModTanSatFilter.CycleMobSelection(dir);
	public static bool IsMobTicked(int templateId) => ModTanSatFilter.IsMobTicked(templateId);
	public static void ToggleMobTicked(int templateId) => ModTanSatFilter.ToggleMobTicked(templateId);
	public static void ToggleSelectAllMobs() => ModTanSatFilter.ToggleSelectAllMobs();
	public static bool IsSkillTicked(int templateId) => ModTanSatFilter.IsSkillTicked(templateId);
	public static void ToggleSkillTicked(int templateId) => ModTanSatFilter.ToggleSkillTicked(templateId);
	public static void ToggleSelectAllSkills() => ModTanSatFilter.ToggleSelectAllSkills();

	/// <summary>
	/// Truy xuất và trả về dữ liệu của GetBestSkillToUse.
	/// </summary>
	public static Skill GetBestSkillToUse()
	{
		return ModTanSatFilter.GetBestSkillToUse();
	}
	public static Skill[] GetHotbarSkills() => ModTanSatFilter.GetHotbarSkills();
	public static int GetSkillHotbarSlot(int skillTemplateId) => ModTanSatFilter.GetSkillHotbarSlot(skillTemplateId);
	public static Skill FindSkillByTemplateId(int templateId) => ModTanSatFilter.FindSkillByTemplateId(templateId);

	public static bool IsTileBlocked(int px, int py) => ModTanSatTargeting.IsTileBlocked(px, py);

	/// <summary>
	/// Truy xuất và trả về dữ liệu của GetSafeAttackPosition.
	/// </summary>
	public static void GetSafeAttackPosition(Mob target, bool isRanged, out int outX, out int outY)
	{
		ModTanSatTargeting.GetSafeAttackPosition(target, isRanged, out outX, out outY);
	}

	private static Mob SelectBestMob(Char me, MyVector mobs, long now, bool ignoreBlacklist)
	{
		Mob bestMob = null;
		int minDistance = int.MaxValue;
		bool coldBfs = ModTeleport.IsColdPlanetAntiCheckActive();

		for (int i = 0; i < mobs.size(); i++)
		{
			Mob m = (Mob)mobs.elementAt(i);
			if (m == null || m.isMobMe || m.status == 0 || m.status == 1 || m.hp <= 0) continue;
			if (avoidSuperMob && (m.levelBoss > 0 || m.isBoss)) continue;
			if (!GameScr.gI().isMeCanAttackMob(m)) continue;
			if (!ModTanSatFilter.IsMobAllowed(m.templateId)) continue;

			if (!ignoreBlacklist && ignoredMobUntil.TryGetValue(m.mobId, out long ignoreUntil))
			{
				if (now < ignoreUntil) continue;
				ignoredMobUntil.Remove(m.mobId);
			}

			bool isFlyingMob = (Mob.arrMobTemplate != null &&
			                    m.templateId >= 0 &&
			                    m.templateId < Mob.arrMobTemplate.Length &&
			                    Mob.arrMobTemplate[m.templateId] != null &&
			                    (Mob.arrMobTemplate[m.templateId].type == 4 || Mob.arrMobTemplate[m.templateId].type == 5));
			bool sameColdBase = coldBfs && !isFlyingMob && ModTeleport.IsSameColdGroundBase(me.cx, me.cy, m.x, m.y);
			int euclidDist = Res.distance(me.cx, me.cy, m.x, m.y);
			int dist;
			if (sameColdBase)
			{
				dist = Res.abs(m.x - me.cx) - 20000;
				if ((m.x - me.cx) * me.cdir >= 0)
				{
					dist -= 24;
				}
			}
			else if (coldBfs)
			{
				bool directAboveCurrentBase = isFlyingMob &&
				                              TileMap.tileTypeAt(me.cx, me.cy, 2) &&
				                              Res.abs(m.x - me.cx) <= 168 &&
				                              m.y < me.cy &&
				                              (me.cy - m.y) <= 220 &&
				                              ModTeleport.IsColdDirectAirReachable(me.cx, me.cy, m.x, m.y);
				if (directAboveCurrentBase)
				{
					dist = euclidDist - 4000;
				}
				else
				{
					int tileCost = ModTeleport.GetColdTravelTileCost(me.cx, me.cy, m.x, m.y);
					dist = (euclidDist / 2) + (tileCost * 20);
				}
			}
			else
			{
				dist = euclidDist;
			}

			if (ModDropRate.isLastHitLock && m.hp < (m.maxHp / 3) && euclidDist < 150)
			{
				dist -= 50;
			}
			if (ModDropRate.isInstantRespawnAttack && m.hp >= m.maxHp && euclidDist < 120)
			{
				dist -= 30;
			}

			if (dist < minDistance)
			{
				minDistance = dist;
				bestMob = m;
			}
		}

		return bestMob;
	}

	private static void CheckAndRestoreStamina(Char me, long now)
	{
		if (!autoCheckStamina || me == null || me.cStamina > 5 || now - lastStaminaUseTime < 2000)
		{
			return;
		}
		if (me.arrItemBag != null)
		{
			for (int i = 0; i < me.arrItemBag.Length; i++)
			{
				Item it = me.arrItemBag[i];
				if (it != null && it.template != null && (it.template.id == 212 || it.template.id == 211))
				{
					lastStaminaUseTime = now;
					Service.gI().useItem(0, 1, (sbyte)i, -1);
					GameScr.info1?.addInfo("Tự dùng " + it.template.name + " hồi thể lực!", 0);
					return;
				}
			}
		}
	}

	private static void CheckAutoChangeZone(Char me, long now)
	{
		if (!autoChangeZone || me == null || now - lastAutoZoneChangeTime < 12000)
		{
			return;
		}
		if (TileMap.isOfflineMap() || TileMap.mapID == 21 || TileMap.mapID == 22 || TileMap.mapID == 23)
		{
			return;
		}

		bool hasOtherPlayerNear = false;
		if (GameScr.vCharInMap != null)
		{
			for (int i = 0; i < GameScr.vCharInMap.size(); i++)
			{
				Char c = (Char)GameScr.vCharInMap.elementAt(i);
				if (c != null && !c.isPet && !c.isMiniPet && !c.isCopy && c.charID > 0 && c.charID != me.charID && c.charID != -me.charID && c.cHP > 0)
				{
					if (!string.IsNullOrEmpty(c.cName) && (c.cName.StartsWith("#") || c.cName.StartsWith("$")))
					{
						continue;
					}
					if (Res.distance(me.cx, me.cy, c.cx, c.cy) <= 180)
					{
						hasOtherPlayerNear = true;
						break;
					}
				}
			}
		}

		if (hasOtherPlayerNear)
		{
			if (ksDetectedSince == 0)
			{
				ksDetectedSince = now;
			}
			else if (now - ksDetectedSince >= 8000)
			{
				ksDetectedSince = 0;
				lastAutoZoneChangeTime = now;
				int maxZ = (GameScr.gI() != null && GameScr.gI().zones != null && GameScr.gI().zones.Length > 0) ? GameScr.gI().zones.Length : 15;
				int nextZone = (TileMap.zoneID + 1) % maxZ;
				if (GameScr.gI() != null && GameScr.gI().numPlayer != null && GameScr.gI().maxPlayer != null)
				{
					int minPlayers = int.MaxValue;
					for (int z = 0; z < GameScr.gI().numPlayer.Length; z++)
					{
						if (z == TileMap.zoneID) continue;
						int np = GameScr.gI().numPlayer[z];
						int mp = GameScr.gI().maxPlayer[z];
						if (np < mp - 2 && np < minPlayers)
						{
							minPlayers = np;
							nextZone = z;
						}
					}
				}
				Service.gI().requestChangeZone(nextZone, -1);
				GameScr.info1?.addInfo("Tự đổi sang Khu " + nextZone + " tránh KS!", 0);
			}
		}
		else
		{
			ksDetectedSince = 0;
		}
	}

	public static void UpdateGoldTrain(Char me, long now)
	{
		if (me == null) return;
		if (me.charID != lastTrackedCharId || lastTrackedXu < 0)
		{
			lastTrackedCharId = me.charID;
			lastTrackedXu = me.xu;
			if (goldTrainStartTime <= 0)
			{
				goldTrainStartTime = now;
			}
		}
		else if (me.xu != lastTrackedXu)
		{
			if (me.xu > lastTrackedXu)
			{
				goldTrainTotal += (me.xu - lastTrackedXu);
			}
			lastTrackedXu = me.xu;
		}
	}

	public static long GetGoldPerHour()
	{
		if (goldTrainStartTime <= 0 || goldTrainTotal <= 0) return 0;
		long elapsed = mSystem.currentTimeMillis() - goldTrainStartTime;
		if (elapsed < 1000) return 0;
		return goldTrainTotal * 3600000L / elapsed;
	}

	public static void ResetGoldTrain()
	{
		goldTrainTotal = 0;
		goldTrainStartTime = mSystem.currentTimeMillis();
		lastTrackedXu = (Char.myCharz() != null) ? Char.myCharz().xu : -1;
	}

	public static void PaintGoldTrainHUD(mGraphics g)
	{
		if (!showGoldTrain || !ModMenu.IsInGame() || g == null) return;
		if ((GameCanvas.panel != null && GameCanvas.panel.isShow) ||
		    (GameCanvas.panel2 != null && GameCanvas.panel2.isShow) ||
		    (GameCanvas.menu != null && GameCanvas.menu.showMenu) ||
		    GameCanvas.currentDialog != null ||
		    ModUI.uiCustomOpen)
		{
			return;
		}
		try
		{
			g.translate(-g.getTranslateX(), -g.getTranslateY());
			g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
			int x = 6 + SafeArea.Left;
			int y = 64;
			string text = "Vàng úp: " + Res.formatNumber2(goldTrainTotal) + " (" + Res.formatNumber2(GetGoldPerHour()) + "/h)";
			(mFont.tahoma_7b_yellow ?? mFont.tahoma_7_yellow).drawString(g, text, x, y, mFont.LEFT);
		}
		catch
		{
		}
	}

	private static void CheckAutoResetZone(Char me, long now)
	{
		if (!autoResetZone || me == null || resetZoneMinutes <= 0)
		{
			lastResetZoneTime = 0;
			return;
		}
		if (TileMap.isOfflineMap() || TileMap.mapID == 21 || TileMap.mapID == 22 || TileMap.mapID == 23)
		{
			return;
		}
		if (lastResetZoneTime == 0)
		{
			lastResetZoneTime = now;
			return;
		}
		long intervalMs = resetZoneMinutes * 60000L;
		if (now - lastResetZoneTime >= intervalMs)
		{
			lastResetZoneTime = now;
			Service.gI().requestChangeZone(TileMap.zoneID, -1);
			GameScr.info1?.addInfo("Tự làm mới Khu " + TileMap.zoneID + " (" + resetZoneMinutes + "p)!", 0);
		}
	}

	public static bool UpdateSupportAndFreezeSkills(Char me, long now)
	{
		if (me == null) return false;

		if (freezeSkillCooldown || frozenSkillIds.Count > 0)
		{
			Skill[] hotbar = ModTanSatFilter.GetHotbarSkills();
			if (hotbar != null)
			{
				for (int i = 0; i < hotbar.Length; i++)
				{
					Skill s = hotbar[i];
					if (s != null && s.template != null && (freezeSkillCooldown || frozenSkillIds.Contains(s.template.id)))
					{
						s.coolDown = 0;
						s.lastTimeUseThisSkill = 0;
						s.paintCanNotUseSkill = false;
					}
				}
			}
			if (me.myskill != null && me.myskill.template != null && (freezeSkillCooldown || frozenSkillIds.Contains(me.myskill.template.id)))
			{
				me.myskill.coolDown = 0;
				me.myskill.lastTimeUseThisSkill = 0;
				me.myskill.paintCanNotUseSkill = false;
			}
		}

		if (!autoSupportSkills || now - lastSupportSkillTime < 1000)
		{
			return false;
		}

		Skill[] skills = ModTanSatFilter.GetHotbarSkills();
		if (skills == null) return false;

		for (int i = 0; i < skills.Length; i++)
		{
			Skill s = skills[i];
			if (s == null || s.template == null || ModTanSatFilter.IsAttackSkill(s)) continue;
			int tplId = s.template.id;
			// Chỉ tự bật các skill buff/hỗ trợ: Thái dương hạ san (6), Trị thương (7), Đẻ trứng (12), Biến khỉ (13), Khiên năng lượng (19), Huýt sáo (21)
			if (tplId != 6 && tplId != 7 && tplId != 12 && tplId != 13 && tplId != 19 && tplId != 21) continue;
			if (tplId == 7 && me.cHPFull > 0 && (me.cHP * 100L / me.cHPFull) > 65) continue;
			if (tplId == 13 && me.isMonkey == 1) continue;
			if (tplId == 19 && me.protectEff) continue;

			int cd = (s.coolDown > 0) ? s.coolDown : 1500;
			if (now - s.lastTimeUseThisSkill >= cd && ModTanSatFilter.HasEnoughMp(me, s))
			{
				lastSupportSkillTime = now;
				me.myskill = s;
				Service.gI().selectSkill(tplId);
				GameScr.gI().doSelectSkill(s, isShortcut: true);
				s.lastTimeUseThisSkill = now;
				lastSentSkillTemplateId = tplId;
				waitUntilTime = now + 400;
				return true;
			}
		}

		return false;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ RunTanSat.
	/// </summary>
	public static void RunTanSat()
	{
		try
		{
			if (!autoTanSat) return;

			if (ModNextMap.isNextMapActive || ModGoBack.isReturning || ModSetActivator.isBusy || ModAutoBuyBua.isBusy || Char.isLoadingMap || ModAutoCaptcha.IsCaptchaActive()) return;
			if (ModAutoPick.isBusy)
			{
				if (currentFarmTarget != null && (currentFarmTarget.status == 0 || currentFarmTarget.status == 1 || currentFarmTarget.hp <= 0))
				{
					OnPendingAttackAborted();
					currentFarmTarget = null;
					currentTargetResetCount = 0;
					currentTargetUnchangedAttackCount = 0;
					lastResyncAttackCount = 0;
				}
				targetLockTime = mSystem.currentTimeMillis();
				return;
			}

			Char me = Char.myCharz();
			if (me == null || me.cHP <= 0 || me.statusMe == 14 || me.statusMe == 5) return;

			// Tắt hoàn toàn AutoPlay (TĐLT) gốc của game khi đang bật Tàn Sát để không tự đổi chiêu và tự nhảy mục tiêu
			GameScr.isAutoPlay = false;
			if (GameScr.gI() != null)
			{
				GameScr.gI().auto = 0;
			}

			long now = mSystem.currentTimeMillis();

			// Nhận diện khi đổi sang map mới: Đặt lại toàn bộ trạng thái giống ModNextMap
			if (TileMap.mapID != lastMapId)
			{
				lastMapId = TileMap.mapID;
				currentFarmTarget = null;
				lastTeleportTargetMobId = -1;
				resetPenaltyCount = 0;
				lastResetPenaltyTime = 0;
				consecutiveConfirmedHits = 0;
				currentTargetResetCount = 0;
				currentTargetUnchangedAttackCount = 0;
				pendingAttackPacket = false;
				changingMapStuckSince = 0;
				pointMoveStuckSince = 0;
				chargeStuckSince = 0;
				ModTeleport.ClearEchoes();
				ignoredMobUntil.Clear();
				waitUntilTime = now + 250;
				return;
			}

			// Watchdog chống kẹt trạng thái Char.ischangingMap (tương tự ModNextMap)
			if (Char.ischangingMap)
			{
				if (changingMapStuckSince == 0)
				{
					changingMapStuckSince = now;
				}
				else if (now - changingMapStuckSince > 2000)
				{
					Char.ischangingMap = false;
					Char.isLockKey = false;
					me.isLockAttack = false;
					me.isLockMove = false;
					InfoDlg.hide();
					GameCanvas.endDlg();
					changingMapStuckSince = 0;
				}
				return;
			}
			changingMapStuckSince = 0;

			if (me.isTeleport || me.statusMe == 16)
			{
				waitUntilTime = now + 250;
				return;
			}

			if (me.isCharge || me.isFreez)
			{
				chargeStuckSince = 0;
				return;
			}
			if (me.isFlyAndCharge || me.isStandAndCharge || me.isUseSkillAfterCharge || me.isWaitMonkey || me.isCreateDark)
			{
				if (chargeStuckSince == 0)
				{
					chargeStuckSince = now;
					return;
				}
				if (now - chargeStuckSince <= 10000)
				{
					return;
				}
				me.stopUseChargeSkill();
				me.isWaitMonkey = false;
				chargeStuckSince = 0;
			}
			else
			{
				chargeStuckSince = 0;
			}

			if (HasHeldMoveKeys())
			{
				pointMoveStuckSince = 0;
				lastManualMoveTime = now;
				return;
			}
			if (me.currentMovePoint != null || (me.vMovePoints != null && me.vMovePoints.size() > 0))
			{
				if (pointMoveStuckSince == 0 || Res.abs(me.cx - lastPointMoveX) >= 8 || Res.abs(me.cy - lastPointMoveY) >= 8)
				{
					lastPointMoveX = me.cx;
					lastPointMoveY = me.cy;
					pointMoveStuckSince = now;
					lastManualMoveTime = now;
					return;
				}
				if (now - pointMoveStuckSince <= 1000)
				{
					lastManualMoveTime = now;
					return;
				}
				me.currentMovePoint = null;
				if (me.vMovePoints != null)
				{
					me.vMovePoints.removeAllElements();
				}
				if (GameScr.instance != null)
				{
					GameScr.instance.clickMoving = false;
				}
				pointMoveStuckSince = 0;
			}
			else
			{
				pointMoveStuckSince = 0;
			}
			if (now - lastManualMoveTime < 350 || now < waitUntilTime)
			{
				return;
			}
			if (ModTeleport.TryEscapeColdUnderground(me, now))
			{
				targetLockTime = now;
				return;
			}

			CheckAndRestoreStamina(me, now);
			CheckAutoChangeZone(me, now);
			CheckAutoResetZone(me, now);
			if (trainPets && me.havePet && Char.myPetz() != null && Char.myPetz().petStatus != 2 && Char.myPetz().petStatus != 1 && now - lastSupportSkillTime > 3000)
			{
				lastSupportSkillTime = now;
				Service.gI().petStatus(2);
				Char.myPetz().petStatus = 2;
			}
			if (UpdateSupportAndFreezeSkills(me, now))
			{
				return;
			}

			// 1. Xác định kỹ năng tấn công
			Skill skillToUse = GetBestSkillToUse();
			if (skillToUse == null) return;

			if (me.myskill != skillToUse || lastSentSkillTemplateId != skillToUse.template.id || GameScr.lastSkill != skillToUse)
			{
				me.myskill = skillToUse;
				GameScr.lastSkill = skillToUse;
				Service.gI().selectSkill(skillToUse.template.id);
				lastSentSkillTemplateId = skillToUse.template.id;
			}

			// 2. Giữ khóa mục tiêu hiện tại cho đến khi quái chết hẳn (hoặc bỏ qua quái lag 15s >=90% HP / đổi mục tiêu khi bật trainPets / autoChangeFocus)
			bool targetDiedOrInvalid = false;
			if (currentFarmTarget != null)
			{
				bool isTrainingDummy = (currentFarmTarget.templateId == 0 && (TileMap.isTrainingMap() || currentFarmTarget.maxHp >= 100000000L));
				if (currentFarmTarget.isMobMe || currentFarmTarget.status == 0 || currentFarmTarget.status == 1 || currentFarmTarget.hp <= 0 || (avoidSuperMob && (currentFarmTarget.levelBoss > 0 || currentFarmTarget.isBoss)) || !ModTanSatFilter.IsMobAllowed(currentFarmTarget.templateId) || !GameScr.gI().isMeCanAttackMob(currentFarmTarget))
				{
					targetDiedOrInvalid = true;
				}
				else if (checkLagMob && !isTrainingDummy && currentFarmTarget.maxHp > 0 && currentFarmTarget.hp * 100L >= currentFarmTarget.maxHp * 90L && now - targetLockTime >= 15000)
				{
					ignoredMobUntil[currentFarmTarget.mobId] = now + 30000L;
					targetDiedOrInvalid = true;
				}
				else if (trainPets && me.havePet && currentFarmTarget.maxHp > 0 && currentFarmTarget.hp * 100L < currentFarmTarget.maxHp * 80L && currentTargetUnchangedAttackCount >= 1)
				{
					ignoredMobUntil[currentFarmTarget.mobId] = now + 4000L;
					targetDiedOrInvalid = true;
				}
				else if (autoChangeFocus && currentTargetUnchangedAttackCount >= 1)
				{
					ignoredMobUntil[currentFarmTarget.mobId] = now + 1200L;
					targetDiedOrInvalid = true;
				}
				else if (currentFarmTarget.hp < targetLastHp)
				{
					long hpDrop = targetLastHp - currentFarmTarget.hp;
					targetLastHp = currentFarmTarget.hp;
					bool hasCompanion = me.havePet || me.petFollow != null || me.mobMe != null;
					bool noRecentSelfAttack = lastConfirmedAttackPacketTime <= 0 || (now - lastConfirmedAttackPacketTime > 1600);
					bool isChipDamage = me.cDamFull > 200 && hpDrop < System.Math.Max(1L, me.cDamFull / 3);
					bool isExternalOrPetHit = noRecentSelfAttack || (hasCompanion && isChipDamage);
					if (!isExternalOrPetHit)
					{
						int confirmMaxX = System.Math.Max(72, skillToUse.dx);
						int confirmMaxY = System.Math.Max(72, skillToUse.dy);
						if (Res.abs(me.cx - currentFarmTarget.x) <= confirmMaxX && Res.abs(me.cy - currentFarmTarget.y) <= confirmMaxY)
						{
							ModTeleport.isWaitingServerSync = false;
							ModTeleport.isCurrentPosSynced = true;
						}
						targetLockTime = now;
						currentTargetResetCount = 0;
						currentTargetUnchangedAttackCount = 0;
						lastResyncAttackCount = 0;
						consecutiveConfirmedHits++;
						if (consecutiveConfirmedHits >= 2 && lastResetPenaltyTime > 0 && now - lastResetPenaltyTime >= 900)
						{
							lastResetPenaltyTime = 0;
							if (resetPenaltyCount > 0)
							{
								resetPenaltyCount--;
							}
						}
						else if (resetPenaltyCount > 0 && lastResetPenaltyTime == 0)
						{
							resetPenaltyCount = 0;
						}
					}
				}
				else
				{
					if (currentFarmTarget.hp > targetLastHp)
					{
						targetLastHp = currentFarmTarget.hp;
					}
					if (ModTeleport.IsWaitingServerSync())
					{
						targetLockTime = now;
						currentTargetUnchangedAttackCount = 0;
					}
					else
					{
						bool coldAntiCheck = ModTeleport.IsColdPlanetAntiCheckActive();
						int resyncMinElapsed = coldAntiCheck ? 280 : 350;
						long refAttackTime = (lastConfirmedAttackPacketTime > 0) ? lastConfirmedAttackPacketTime : lastAttackTime;
						if (!isTrainingDummy && currentFarmTarget.hp > 1 && currentTargetUnchangedAttackCount > 0 && now - refAttackTime >= resyncMinElapsed && lastResyncAttackCount != currentTargetUnchangedAttackCount)
						{
							bool shouldResync = (coldAntiCheck && currentTargetUnchangedAttackCount == 2) ||
							                    currentTargetUnchangedAttackCount == 3 ||
							                    currentTargetUnchangedAttackCount == 6;
							if (shouldResync)
							{
								lastResyncAttackCount = currentTargetUnchangedAttackCount;
								if (!coldAntiCheck && !ModTeleport.isCurrentPosSynced && currentTargetUnchangedAttackCount == 3 && lastTeleportTargetMobId != currentFarmTarget.mobId && TileMap.tileTypeAt(me.cx, me.cy, 2) && !ModTeleport.HasPendingEchoNear(me.cx, me.cy))
								{
									ModTeleport.TeleportTo(me.cx, me.cy);
								}
								else
								{
									ModTeleport.SendSingleCharMove(me.cx, me.cy);
								}
								lastTeleportTargetMobId = currentFarmTarget.mobId;
								lastTeleportTime = now;
							}
						}
					}
					if (checkLagMob && !isTrainingDummy && currentTargetUnchangedAttackCount >= 8 && now - targetLockTime > 4500)
					{
						ignoredMobUntil[currentFarmTarget.mobId] = now + 6000L;
						targetDiedOrInvalid = true;
					}
					else if (!isTrainingDummy && currentTargetUnchangedAttackCount >= 12 && now - targetLockTime > 20000)
					{
						ignoredMobUntil[currentFarmTarget.mobId] = now + 5000L;
						targetDiedOrInvalid = true;
					}
				}
			}

			if (targetDiedOrInvalid)
			{
				if (me.skillPaint != null && !me.hasSendAttack && me.dart == null && me.arr == null)
				{
					OnPendingAttackAborted();
				}
				else if (pendingAttackPacket && me.dart == null && me.arr == null && !me.hasSendAttack)
				{
					OnPendingAttackAborted();
				}
				pendingAttackPacket = false;
				currentFarmTarget = null;
				lastTeleportTargetMobId = -1;
				currentTargetResetCount = 0;
				currentTargetUnchangedAttackCount = 0;
				lastResyncAttackCount = 0;
				ModTeleport.isWaitingSettle = false;
				if (me.mobFocus == null || me.mobFocus.status == 0 || me.mobFocus.status == 1 || me.mobFocus.hp <= 0)
				{
					me.skillPaint = null;
					me.skillPaintRandomPaint = null;
					me.dart = null;
					me.arr = null;
					me.indexSkill = 0;
					me.effPaints = null;
					me.mobFocus = null;
				}
				if (!ModTeleport.IsColdPlanetAntiCheckActive())
				{
					waitUntilTime = System.Math.Max(waitUntilTime, now + 120);
					return;
				}
			}

			// 3. Tìm quái mục tiêu mới nếu chưa có (nhường AutoPick nhặt vật phẩm rơi trước khi khóa mục tiêu mới)
			if (currentFarmTarget == null)
			{
				if (ModAutoPick.HasPendingPickableItem())
				{
					return;
				}

				MyVector mobs = GameScr.vMob;
				if (mobs != null && mobs.size() > 0)
				{
					Mob bestMob = SelectBestMob(me, mobs, now, ignoreBlacklist: false);
					if (bestMob == null && ignoredMobUntil.Count > 0)
					{
						bestMob = SelectBestMob(me, mobs, now, ignoreBlacklist: true);
					}

					if (bestMob != null)
					{
						currentFarmTarget = bestMob;
						targetLockTime = now;
						targetLastHp = bestMob.hp;
						currentTargetResetCount = 0;
						currentTargetUnchangedAttackCount = 0;
						lastResyncAttackCount = 0;
					}
				}
			}

			if (currentFarmTarget == null)
			{
				if (ModTeleport.IsColdPlanetAntiCheckActive() && !TileMap.tileTypeAt(me.cx, me.cy, 2))
				{
					int groundY = ModTanSatTargeting.FindGroundY(me.cx, me.cy, 360);
					if (groundY != -1)
					{
						ModTeleport.MoveSteppedTo(me.cx, groundY);
					}
				}
				return;
			}

			me.mobFocus = currentFarmTarget;
			me.charFocus = null;
			me.cdir = (currentFarmTarget.x >= me.cx) ? 1 : -1;

			// 4. Kiểm tra cự ly thực chiến với quái mục tiêu và dịch chuyển theo chuẩn NextMap
			int anchorX = currentFarmTarget.x;
			int anchorY = currentFarmTarget.y;
			int deltaX = Res.abs(me.cx - anchorX);
			int deltaY = Res.abs(me.cy - anchorY);

			int tplId = (skillToUse.template != null) ? skillToUse.template.id : -1;
			bool isMeleeTemplate = (tplId == 0 || tplId == 2 || tplId == 4 || tplId == 9 || tplId == 17);
			bool isRanged = !isMeleeTemplate && (skillToUse.dx > 75);
			bool isColdAntiCheckNow = ModTeleport.IsColdPlanetAntiCheckActive();
			bool isMobFlying = (Mob.arrMobTemplate != null &&
			                    currentFarmTarget.templateId >= 0 &&
			                    currentFarmTarget.templateId < Mob.arrMobTemplate.Length &&
			                    Mob.arrMobTemplate[currentFarmTarget.templateId] != null &&
			                    (Mob.arrMobTemplate[currentFarmTarget.templateId].type == 4 || Mob.arrMobTemplate[currentFarmTarget.templateId].type == 5));

			bool isNewTarget = (lastTeleportTargetMobId != currentFarmTarget.mobId);
			bool syncedColdPos = isColdAntiCheckNow && ModTeleport.isCurrentPosSynced;
			int coldEngagedMeleeRangeX = System.Math.Min(System.Math.Max(28, skillToUse.dx - 10), 38);
			int maxRangeX = isRanged ? (skillToUse.dx - 8) : ((syncedColdPos || (!isNewTarget && isColdAntiCheckNow)) ? coldEngagedMeleeRangeX : 24);
			int maxRangeY = isRanged ? (skillToUse.dy - 6) : 24;

			bool isOutOfRange = (deltaX > maxRangeX || deltaY > maxRangeY);
			if (!isNewTarget && isColdAntiCheckNow && isOutOfRange && !isRanged && me.skillPaint != null && !me.hasSendAttack && deltaX <= skillToUse.dx && deltaY <= skillToUse.dy)
			{
				isOutOfRange = false;
			}
			bool onGroundNow = TileMap.tileTypeAt(me.cx, me.cy, 2);
			bool canAdoptInPlace = !isColdAntiCheckNow || onGroundNow || !TileMap.tileTypeAt(anchorX, anchorY, 2);
			int adoptMaxX = 16;
			int adoptMaxY = 16;
			bool canStopMidWalkToFire = false;
			if (isColdAntiCheckNow)
			{
				if (isRanged)
				{
					adoptMaxX = syncedColdPos ? System.Math.Max(36, skillToUse.dx - 20) : 24;
					int rangedSafeY = System.Math.Max(20, System.Math.Min(skillToUse.dy - 12, 44));
					if (isMobFlying && deltaY > deltaX)
					{
						rangedSafeY = System.Math.Min(rangedSafeY, 28);
					}
					adoptMaxY = syncedColdPos ? rangedSafeY : 16;
					canStopMidWalkToFire = syncedColdPos && (onGroundNow || isMobFlying);
				}
				else
				{
					adoptMaxX = syncedColdPos ? System.Math.Min(coldEngagedMeleeRangeX, 34) : 20;
					adoptMaxY = syncedColdPos ? 20 : 16;
				}
			}
			bool adoptLineClear = !isColdAntiCheckNow || !isRanged || deltaX <= 36 ||
			                      ModTeleport.IsSameColdGroundBase(me.cx, me.cy, anchorX, anchorY) ||
			                      ModTeleport.IsColdDirectAirReachable(me.cx, me.cy, anchorX, anchorY);

			if (isNewTarget && canAdoptInPlace && adoptLineClear && lastResyncAttackCount == 0 && !isOutOfRange && (!ModTeleport.isWaitingSettle || canStopMidWalkToFire) && deltaX <= adoptMaxX && deltaY <= adoptMaxY && !ModTanSatTargeting.IsInsideWaypoint(me.cx, me.cy))
			{
				ModTeleport.isWaitingSettle = false;
				ModTeleport.ResetColdStepTracking();
				lastTeleportTargetMobId = currentFarmTarget.mobId;
				lastTeleportTime = now;
				isNewTarget = false;
			}

			if (isNewTarget || isOutOfRange)
			{
				if (useTeleport)
				{
					// Bắt buộc đợi máy chủ đồng bộ xong gói dịch chuyển trước đó mới dịch chuyển tiếp để tránh dồn hàng đợi RESET_POINT gây lag
					if (ModTeleport.IsWaitingServerSync())
					{
						targetLockTime = now;
						return;
					}

					int chaseMinInterval = ModTeleport.IsColdPlanetAntiCheckActive() ? 180 : 650;
					if (!isNewTarget && now - lastTeleportTime < chaseMinInterval)
					{
						// Đã đứng cạnh mục tiêu hiện tại, tránh giật hình liên tục trên cùng 1 quái
					}
					else
					{
						if (isRanged && (me.dart != null || me.arr != null))
						{
							return;
						}

						int safeX, safeY;
						GetSafeAttackPosition(currentFarmTarget, isRanged, out safeX, out safeY);

						lastStepMoveTime = now;
						bool arrived = ModTeleport.MoveSteppedTo(safeX, safeY, isMobFlying, allowColdLongTeleport: isNewTarget);

						me.cdir = (anchorX >= me.cx) ? 1 : -1;
						me.cvx = 0;
						me.cvy = 0;

						if (!arrived)
						{
							if (ModTeleport.IsColdStepTimedOut())
							{
								ignoredMobUntil[currentFarmTarget.mobId] = now + 8000L;
								currentFarmTarget = null;
								currentTargetResetCount = 0;
								currentTargetUnchangedAttackCount = 0;
								ModTeleport.ResetColdStepTracking();
								return;
							}
							// Đang nhảy từng bước mặt đất đến quái hoặc đang chờ đồng bộ vị trí: Gia hạn targetLockTime
							targetLockTime = now;
							return;
						}

						lastTeleportTargetMobId = currentFarmTarget.mobId;
						lastTeleportTime = now;
						me.statusMe = 1;
						me.delayFall = 0;
						me.cvx = 0;
						me.cvy = 0;
					}
				}
				else if (isOutOfRange)
				{
					int safeX, safeY;
					GetSafeAttackPosition(currentFarmTarget, isRanged, out safeX, out safeY);
					me.moveTo(safeX, safeY, 0);
					return;
				}
			}

			// 5. Đã ở trong tầm đánh quái: Khóa hướng mặt và mục tiêu (không gọi charMove mỗi frame để tránh spam 50 gói/s khi lơ lửng đánh quái bay)
			me.cdir = (anchorX >= me.cx) ? 1 : -1;
			me.mobFocus = currentFarmTarget;
			me.charFocus = null;
			if (!TileMap.tileTypeAt(me.cx, me.cy, 2))
			{
				me.statusMe = 1;
				me.delayFall = 0;
				me.cvx = 0;
				me.cvy = 0;
			}

			// Đợi máy chủ xác nhận tọa độ dịch chuyển (RESET_POINT_CONFIRM) trước khi xuất đòn đầu tiên để tránh đấm hụt lúc đang đồng bộ
			if (ModTeleport.IsWaitingServerSyncForAttack())
			{
				targetLockTime = now;
				return;
			}

			// 6. Kiểm tra Cooldown thực tế & KI của game cho kỹ năng này
			int effectiveCooldown = (skillToUse.coolDown < timeAttack) ? timeAttack : skillToUse.coolDown;
			if (skillToUse.template != null && customSkillDelays.TryGetValue(skillToUse.template.id, out int customDelay) && customDelay > 0)
			{
				effectiveCooldown = System.Math.Max(effectiveCooldown, customDelay);
			}

			if (now - skillToUse.lastTimeUseThisSkill < effectiveCooldown || now - lastAttackTime < timeAttack)
			{
				return;
			}
			if (!ModTanSatFilter.HasEnoughMp(me, skillToUse))
			{
				return;
			}

			// 7. Bảo toàn hoạt ảnh đánh trước đó nếu đang thực thi (kèm watchdog chống kẹt vĩnh viễn khi ngoại lệ / mất gói)
			if (me.skillPaint != null || (me.skillInfoPaint() != null && me.indexSkill < me.skillInfoPaint().Length) || me.dart != null || me.arr != null)
			{
				if (lastAttackTime > 0 && now - lastAttackTime > 2400)
				{
					me.skillPaint = null;
					me.skillPaintRandomPaint = null;
					me.dart = null;
					me.arr = null;
					me.indexSkill = 0;
					me.effPaints = null;
					OnPendingAttackAborted();
				}
				else
				{
					return;
				}
			}

			// 8. Thực thi xuất chiêu trực tiếp qua Game Engine chuẩn
			// LƯU Ý QUAN TRỌNG: Tuyệt đối KHÔNG gán skillToUse.lastTimeUseThisSkill = now TRƯỚC khi gọi setSkillPaint / doSelectSkill / sendUseChargeSkill,
			// vì bên trong setSkillPaint() có kiểm tra: if (num - myskill.lastTimeUseThisSkill < myskill.coolDown) return;
			lastAttackTime = now;
			skillToUse.paintCanNotUseSkill = false;
			if (me.myskill != null)
			{
				me.myskill.paintCanNotUseSkill = false;
			}

			if (me.isUseChargeSkill())
			{
				me.currentFireByShortcut = true;
				me.sendUseChargeSkill();
				OnAttackPacketSent();
			}
			else if (me.isUseSkillSpec())
			{
				GameScr.gI().doSelectSkill(skillToUse, isShortcut: true);
				if (me.skillPaint != null && !me.hasSendAttack && me.dart == null && me.arr == null)
				{
					pendingAttackPacket = true;
				}
				else
				{
					OnAttackPacketSent();
				}
			}
			else if (skillToUse.skillId >= 0 && skillToUse.skillId < GameScr.sks.Length && GameScr.sks[skillToUse.skillId] != null)
			{
				bool isGroundedNow = TileMap.tileTypeAt(me.cx, me.cy, 2);
				me.currentFireByShortcut = true;
				me.setSkillPaint(GameScr.sks[skillToUse.skillId], (!isGroundedNow) ? 1 : 0);
				me.delayFall = 0;
				me.cvy = 0;
				if (me.skillPaint != null && !me.hasSendAttack && me.dart == null && me.arr == null)
				{
					pendingAttackPacket = true;
				}
				else if (me.hasSendAttack || me.dart != null || me.arr != null)
				{
					OnAttackPacketSent();
				}
			}
			else
			{
				GameScr.gI().doSelectSkill(skillToUse, isShortcut: true);
				if (me.skillPaint != null && !me.hasSendAttack && me.dart == null && me.arr == null)
				{
					pendingAttackPacket = true;
				}
				else
				{
					OnAttackPacketSent();
				}
			}
			skillToUse.lastTimeUseThisSkill = now;
			if (me.myskill != null)
			{
				me.myskill.lastTimeUseThisSkill = now;
			}
			ModTdltLogger.LogAutoPlayFire(skillToUse, currentFarmTarget);
		}
		catch
		{
		}
	}
}
