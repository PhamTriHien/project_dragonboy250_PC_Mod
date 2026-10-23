using System;
using System.Collections.Generic;

/// <summary>
/// Module Tàn Sát Pro: Tự động tìm kiếm, khóa mục tiêu, dịch chuyển theo nhịp mặt đất chuẩn NextMap (chống kẹt map / RESET_POINT) và tiêu diệt quái/boss.
/// </summary>
public static class ModTanSat
{
	public static bool autoTanSat = false;
	public static bool autoAttack
	{
		get { return autoTanSat; }
		set
		{
			autoTanSat = value;
			if (!value)
			{
				currentFarmTarget = null;
				lastTeleportTargetMobId = -1;
				resetPenaltyCount = 0;
				currentTargetResetCount = 0;
				currentTargetUnchangedAttackCount = 0;
				ModTeleport.ClearEchoes();
				ignoredMobUntil.Clear();
			}
		}
	}

	public static bool useTeleport = true;
	public static Mob currentFarmTarget = null;
	public static long targetLockTime = 0;
	public static long targetLastHp = 0;
	public static int lastSentSkillTemplateId = -1;
	public static long lastManualMoveTime = 0;
	public static long lastAttackTime = 0;
	public static int timeAttack = 300;
	public static int lockedSkillTemplateId = -1;
	public static int lastTeleportTargetMobId = -1;
	public static long lastTeleportTime = 0;

	public static int lastMapId = -1;
	public static int resetPenaltyCount = 0;
	public static long lastResetPenaltyTime = 0;
	public static int currentTargetResetCount = 0;
	public static int currentTargetUnchangedAttackCount = 0;
	public static long waitUntilTime = 0;
	public static long lastStepMoveTime = 0;
	private static long changingMapStuckSince = 0;
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
		currentTargetResetCount++;
		lastTeleportTargetMobId = -1;
		waitUntilTime = now + 380;
		lastStepMoveTime = now;
		ModTeleport.lastStepTime = now;
		ModTeleport.lastStepDistance = 0;
		ModTeleport.isWaitingSettle = false;
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
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của IsManualMoving.
	/// </summary>
	public static bool IsManualMoving()
	{
		Char me = Char.myCharz();
		if (me == null) return false;

		bool keyMove = false;
		if (GameCanvas.keyHold != null)
		{
			keyMove = GameCanvas.keyHold[21] || GameCanvas.keyHold[22] || GameCanvas.keyHold[23] || GameCanvas.keyHold[24] ||
			          GameCanvas.keyHold[2] || GameCanvas.keyHold[8] || GameCanvas.keyHold[4] || GameCanvas.keyHold[6] ||
			          GameCanvas.keyHold[1] || GameCanvas.keyHold[3];
		}

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

		for (int i = 0; i < mobs.size(); i++)
		{
			Mob m = (Mob)mobs.elementAt(i);
			if (m == null || m.status == 0 || m.status == 1 || m.hp <= 0) continue;
			if (!GameScr.gI().isMeCanAttackMob(m)) continue;
			if (!ModTanSatFilter.IsMobAllowed(m.templateId)) continue;

			if (!ignoreBlacklist && ignoredMobUntil.TryGetValue(m.mobId, out long ignoreUntil))
			{
				if (now < ignoreUntil) continue;
				ignoredMobUntil.Remove(m.mobId);
			}

			int dist = Res.distance(me.cx, me.cy, m.x, m.y);

			if (ModDropRate.isLastHitLock && m.hp < (m.maxHp / 3) && dist < 150)
			{
				dist -= 50;
			}
			if (ModDropRate.isInstantRespawnAttack && m.hp >= m.maxHp && dist < 120)
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

	/// <summary>
	/// Thực thi phương thức nghiệp vụ RunTanSat.
	/// </summary>
	public static void RunTanSat()
	{
		try
		{
			if (!autoTanSat) return;

			if (ModNextMap.isNextMapActive || ModGoBack.isReturning || ModSetActivator.isBusy || ModAutoBuyBua.isBusy || ModAutoPick.isBusy || Char.isLoadingMap) return;

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
				currentTargetResetCount = 0;
				currentTargetUnchangedAttackCount = 0;
				changingMapStuckSince = 0;
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

			if (me.isCharge || me.isFlyAndCharge || me.isStandAndCharge || me.isUseSkillAfterCharge || me.isWaitMonkey || me.isFreez || me.isCreateDark) return;

			if (IsManualMoving())
			{
				lastManualMoveTime = now;
				return;
			}
			if (now - lastManualMoveTime < 350 || now < waitUntilTime)
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

			// 2. Giữ khóa mục tiêu hiện tại cho đến khi quái chết hẳn (chỉ bỏ qua nếu là quái ma bị lỗi sau >= 12 đòn đánh liên tiếp không mất máu)
			bool targetDiedOrInvalid = false;
			if (currentFarmTarget != null)
			{
				if (currentFarmTarget.status == 0 || currentFarmTarget.status == 1 || currentFarmTarget.hp <= 0 || !ModTanSatFilter.IsMobAllowed(currentFarmTarget.templateId) || !GameScr.gI().isMeCanAttackMob(currentFarmTarget))
				{
					targetDiedOrInvalid = true;
				}
				else if (currentFarmTarget.hp < targetLastHp)
				{
					targetLastHp = currentFarmTarget.hp;
					targetLockTime = now;
					currentTargetResetCount = 0;
					currentTargetUnchangedAttackCount = 0;
					if (resetPenaltyCount > 0 && now - lastResetPenaltyTime > 12000)
					{
						resetPenaltyCount--;
						lastResetPenaltyTime = now;
					}
				}
				else if (currentFarmTarget.templateId != 0 && currentTargetUnchangedAttackCount >= 12 && now - targetLockTime > 20000)
				{
					ignoredMobUntil[currentFarmTarget.mobId] = now + 5000L;
					targetDiedOrInvalid = true;
				}
			}

			if (targetDiedOrInvalid)
			{
				currentFarmTarget = null;
				lastTeleportTargetMobId = -1;
				currentTargetResetCount = 0;
				currentTargetUnchangedAttackCount = 0;
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
				waitUntilTime = System.Math.Max(waitUntilTime, now + 120);
				return;
			}

			// 3. Tìm quái mục tiêu mới nếu chưa có (nhường AutoPick nhặt vật phẩm rơi trước khi khóa mục tiêu mới)
			if (currentFarmTarget == null)
			{
				if (ModAutoPick.HasPendingPickableItem())
				{
					return;
				}

				MyVector mobs = GameScr.vMob;
				if (mobs == null || mobs.size() == 0) return;

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
				}
			}

			if (currentFarmTarget == null) return;

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

			int maxRangeX = isRanged ? (skillToUse.dx - 8) : System.Math.Max(38, skillToUse.dx - 2);
			int maxRangeY = isRanged ? (skillToUse.dy - 6) : System.Math.Max(32, skillToUse.dy - 2);

			bool isNewTarget = (lastTeleportTargetMobId != currentFarmTarget.mobId);
			bool isOutOfRange = (deltaX > maxRangeX || deltaY > maxRangeY);

			if (isNewTarget && !isOutOfRange && !ModTeleport.isWaitingSettle && deltaX <= 24 && deltaY <= 24 && !ModTanSatTargeting.IsInsideWaypoint(me.cx, me.cy))
			{
				lastTeleportTargetMobId = currentFarmTarget.mobId;
				lastTeleportTime = now;
				isNewTarget = false;
			}

			if (isNewTarget || isOutOfRange)
			{
				if (useTeleport)
				{
					if (!isNewTarget && now - lastTeleportTime < 650)
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

						bool isMobFlying = (Mob.arrMobTemplate != null &&
						                    currentFarmTarget.templateId >= 0 &&
						                    currentFarmTarget.templateId < Mob.arrMobTemplate.Length &&
						                    Mob.arrMobTemplate[currentFarmTarget.templateId] != null &&
						                    (Mob.arrMobTemplate[currentFarmTarget.templateId].type == 4 || Mob.arrMobTemplate[currentFarmTarget.templateId].type == 5));

						lastStepMoveTime = now;
						bool arrived = ModTeleport.MoveSteppedTo(safeX, safeY, isMobFlying);

						me.cdir = (anchorX >= me.cx) ? 1 : -1;
						me.cvx = 0;
						me.cvy = 0;

						if (!arrived)
						{
							// Đang nhảy từng bước mặt đất đến quái hoặc đang chờ đồng bộ vị trí: Gia hạn targetLockTime
							targetLockTime = now;
							return;
						}

						lastTeleportTargetMobId = currentFarmTarget.mobId;
						lastTeleportTime = now;

						if (isMobFlying && !TileMap.tileTypeAt(me.cx, me.cy, 2))
						{
							me.statusMe = 4;
							me.delayFall = 40;
							me.cvy = 0;
						}
						else
						{
							me.statusMe = 1;
							me.delayFall = 0;
						}
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
				me.delayFall = 40;
				me.cvy = 0;
			}

			// 6. Kiểm tra Cooldown thực tế & KI của game cho kỹ năng này
			int effectiveCooldown = (skillToUse.coolDown < timeAttack) ? timeAttack : skillToUse.coolDown;

			if (now - skillToUse.lastTimeUseThisSkill < effectiveCooldown || now - lastAttackTime < timeAttack)
			{
				return;
			}
			if (!ModTanSatFilter.HasEnoughMp(me, skillToUse))
			{
				return;
			}

			// 7. Bảo toàn hoạt ảnh đánh trước đó nếu đang thực thi
			if (me.skillPaint != null || (me.skillInfoPaint() != null && me.indexSkill < me.skillInfoPaint().Length))
			{
				return;
			}
			if (me.dart != null || me.arr != null)
			{
				return;
			}

			// 8. Thực thi xuất chiêu trực tiếp qua Game Engine chuẩn
			// LƯU Ý QUAN TRỌNG: Tuyệt đối KHÔNG gán skillToUse.lastTimeUseThisSkill = now TRƯỚC khi gọi setSkillPaint / doSelectSkill / sendUseChargeSkill,
			// vì bên trong setSkillPaint() có kiểm tra: if (num - myskill.lastTimeUseThisSkill < myskill.coolDown) return;
			lastAttackTime = now;
			currentTargetUnchangedAttackCount++;
			skillToUse.paintCanNotUseSkill = false;
			if (me.myskill != null)
			{
				me.myskill.paintCanNotUseSkill = false;
			}

			if (me.isUseChargeSkill())
			{
				me.currentFireByShortcut = true;
				me.sendUseChargeSkill();
			}
			else if (me.isUseSkillSpec())
			{
				GameScr.gI().doSelectSkill(skillToUse, isShortcut: true);
			}
			else if (skillToUse.skillId >= 0 && skillToUse.skillId < GameScr.sks.Length && GameScr.sks[skillToUse.skillId] != null)
			{
				bool isGroundedNow = TileMap.tileTypeAt(me.cx, me.cy, 2);
				me.currentFireByShortcut = true;
				me.setSkillPaint(GameScr.sks[skillToUse.skillId], (!isGroundedNow) ? 1 : 0);
				if (!isGroundedNow)
				{
					me.delayFall = 40;
					me.cvy = 0;
				}
				else
				{
					me.delayFall = 0;
				}
			}
			else
			{
				GameScr.gI().doSelectSkill(skillToUse, isShortcut: true);
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
