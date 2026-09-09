using System;
using System.Collections.Generic;

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
	public static int lastTeleportTargetMobId = -1;
	public static long lastTeleportTime = 0;

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
	public static Skill GetBestSkillToUse() => ModTanSatFilter.GetBestSkillToUse();
	public static Skill[] GetHotbarSkills() => ModTanSatFilter.GetHotbarSkills();
	public static int GetSkillHotbarSlot(int skillTemplateId) => ModTanSatFilter.GetSkillHotbarSlot(skillTemplateId);
	public static Skill FindSkillByTemplateId(int templateId) => ModTanSatFilter.FindSkillByTemplateId(templateId);

	public static bool IsTileBlocked(int px, int py) => ModTanSatTargeting.IsTileBlocked(px, py);
	public static void GetSafeAttackPosition(Mob target, bool isRanged, out int outX, out int outY)
	{
		ModTanSatTargeting.GetSafeAttackPosition(target, isRanged, out outX, out outY);
	}

	public static void RunTanSat()
	{
		try
		{
			if (!autoTanSat) return;

			// Tam dung khi dang Next Map, GoBack, nhat do hoac chuyen map
			if (ModNextMap.isNextMapActive || ModGoBack.isReturning || ModSetActivator.isBusy || ModAutoBuyBua.isBusy || ModAutoPick.isBusy || Char.isLoadingMap || Char.ischangingMap) return;

			Char me = Char.myCharz();
			if (me == null || me.cHP <= 0 || me.statusMe == 14 || me.statusMe == 5) return;

			long now = mSystem.currentTimeMillis();

			// 0. Uu tien tuyet doi cho thao tac di chuyen thu cong cua nguoi choi (Phim mui ten / WASD / click chuot)
			if (IsManualMoving())
			{
				lastManualMoveTime = now;
				return;
			}
			if (now - lastManualMoveTime < 350)
			{
				return;
			}

			// 1. Xac dinh ky nang tan cong (CHI DUNG DUY NHAT KY NANG CHI DINH / TICK CHON)
			Skill skillToUse = GetBestSkillToUse();
			if (skillToUse == null) return;

			// Dong bo myskill va gui selectSkill ve server khi thay doi chieu
			if (me.myskill != skillToUse || lastSentSkillTemplateId != skillToUse.template.id || GameScr.lastSkill != skillToUse)
			{
				me.myskill = skillToUse;
				GameScr.lastSkill = skillToUse;
				Service.gI().selectSkill(skillToUse.template.id);
				lastSentSkillTemplateId = skillToUse.template.id;
			}

			// 2. Watchdog chong ket quai ma / quai da chet / khong giam HP
			bool targetDiedOrInvalid = false;
			if (currentFarmTarget != null)
			{
				if (currentFarmTarget.status == 0 || currentFarmTarget.status == 1 || currentFarmTarget.hp <= 0)
				{
					targetDiedOrInvalid = true;
				}
				else if (currentFarmTarget.hp < targetLastHp)
				{
					// Quai dang thuc su mat mau: Cap nhat moc va gia han thoi gian
					targetLastHp = currentFarmTarget.hp;
					targetLockTime = now;
				}
				else if (currentFarmTarget.templateId != 0 && now - targetLockTime > 4000)
				{
					// Qua 4 giay khong the gay sat thuong: Chuyen muc tieu khac tranh ket
					targetDiedOrInvalid = true;
				}
			}

			if (targetDiedOrInvalid)
			{
				currentFarmTarget = null;
				lastTeleportTargetMobId = -1;
				// Giai phong ngay lap tuc tat ca cac khoa hoat anh / phi tieu cua quai cu de dich chuyen tuc thi
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
			}

			// 3. Tim quai muc tieu moi neu chua co
			if (currentFarmTarget == null)
			{
				MyVector mobs = GameScr.vMob;
				if (mobs == null || mobs.size() == 0) return;

				Mob bestMob = null;
				int minDistance = int.MaxValue;

				for (int i = 0; i < mobs.size(); i++)
				{
					Mob m = (Mob)mobs.elementAt(i);
					if (m == null || m.status == 0 || m.status == 1 || m.hp <= 0) continue;
					if (!GameScr.gI().isMeCanAttackMob(m)) continue;

					// Loc theo cau hinh quai
					if (!selectAllMobs && tickedMobTemplateIds.Count > 0 && !tickedMobTemplateIds.Contains(m.templateId)) continue;

					int dist = Res.distance(me.cx, me.cy, m.x, m.y);

					// Trick Tăng Tỉ Lệ Rơi Đồ: Ưu tiên quái sắp chết (Last-Hit Lock) để chiếm quyền rớt item
					if (ModDropRate.isLastHitLock && m.hp < (m.maxHp / 3) && dist < 150)
					{
						dist -= 50;
					}

					// Trick Instant Respawn Attack: Ưu tiên quái vừa xuất hiện để triệt tiêu quái nhanh nhất
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

				if (bestMob != null)
				{
					currentFarmTarget = bestMob;
					targetLockTime = now;
					targetLastHp = bestMob.hp;
				}
			}

			if (currentFarmTarget == null) return;

			// Luon khoa muc tieu len quai ngay khi xac dinh duoc quai
			me.mobFocus = currentFarmTarget;
			me.charFocus = null;
			me.cdir = (currentFarmTarget.x >= me.cx) ? 1 : -1;

			// 4. Kiem tra cu ly thuc chien voi quai muc tieu
			int anchorX = currentFarmTarget.x;
			int anchorY = currentFarmTarget.y;
			int deltaX = Res.abs(me.cx - anchorX);
			int deltaY = Res.abs(me.cy - anchorY);
			bool isRanged = (skillToUse.dx > 40);

			int maxRangeX = (skillToUse.dx > 40) ? (skillToUse.dx + 20) : 55;
			int maxRangeY = (skillToUse.dy > 40) ? (skillToUse.dy + 20) : 75;

			bool isNewTarget = (lastTeleportTargetMobId != currentFarmTarget.mobId);
			bool isOutOfRange = (deltaX > maxRangeX || deltaY > maxRangeY);

			if (isNewTarget || isOutOfRange)
			{
				if (useTeleport)
				{
					// Neu la cung mot muc tieu dang danh ma chi bi out-of-range, gioi han toi thieu 600ms moi dich chuyen tiep tranh loop giat hinh
					if (!isNewTarget && now - lastTeleportTime < 600)
					{
						// Tam thoi khong dich chuyen lai ngay tren cung 1 quai
					}
					else
					{
						// Voi chieu ban xa, doi phi tieu cu bay xong truoc khi dich chuyen
						if (isRanged && (me.dart != null || me.arr != null))
						{
							return;
						}

						int safeX, safeY;
						GetSafeAttackPosition(currentFarmTarget, isRanged, out safeX, out safeY);

						ModTeleport.TeleportTo(safeX, safeY);
						lastTeleportTargetMobId = currentFarmTarget.mobId;
						lastTeleportTime = now;

						me.cdir = (anchorX >= me.cx) ? 1 : -1;
						me.cvx = 0;
						me.cvy = 0;

						bool isMobFlying = (Mob.arrMobTemplate != null && currentFarmTarget.templateId >= 0 && currentFarmTarget.templateId < Mob.arrMobTemplate.Length && Mob.arrMobTemplate[currentFarmTarget.templateId] != null && (Mob.arrMobTemplate[currentFarmTarget.templateId].type == 4 || Mob.arrMobTemplate[currentFarmTarget.templateId].type == 5));
						if (isMobFlying)
						{
							me.statusMe = 10;
							me.delayFall = 30;
						}
						else
						{
							me.statusMe = 1;
							me.delayFall = 0;
						}
						return;
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

			// 5. Da o trong tam danh quai: Khoa huong mat va muc tieu
			me.cdir = (anchorX >= me.cx) ? 1 : -1;
			me.mobFocus = currentFarmTarget;
			me.charFocus = null;

			// Dong bo toa do neu can
			if (me.cx != me.cxSend || me.cy != me.cySend)
			{
				Service.gI().charMove();
			}

			// 6. Kiem tra Cooldown thuc te & KI cua game cho ky nang nay (BAT BUOC PHAI CO COOLDOWN TRANH LOI)
			int effectiveCooldown = (skillToUse.coolDown < timeAttack) ? timeAttack : skillToUse.coolDown;

			if (now - skillToUse.lastTimeUseThisSkill < effectiveCooldown || now - lastAttackTime < timeAttack)
			{
				return;
			}
			if (!ModTanSatFilter.HasEnoughMp(me, skillToUse))
			{
				return;
			}

			// 7. Neu hoat anh danh truoc do dang thuc thi (skillPaint / dart / arr), bao toan hoat anh va cho ket thuc
			if (me.skillPaint != null || (me.skillInfoPaint() != null && me.indexSkill < me.skillInfoPaint().Length))
			{
				return;
			}
			if (me.dart != null || me.arr != null)
			{
				return;
			}

			// 8. Thuc thi xuat chieu truc tiep qua Game Engine chuan (100% theo dac ta cooldown thuc cua game)
			lastAttackTime = now;
			skillToUse.lastTimeUseThisSkill = now;
			me.myskill.lastTimeUseThisSkill = now;

			if (me.isUseChargeSkill())
			{
				me.currentFireByShortcut = true;
				me.sendUseChargeSkill();
			}
			else if (skillToUse.skillId >= 0 && skillToUse.skillId < GameScr.sks.Length && GameScr.sks[skillToUse.skillId] != null)
			{
				bool isGroundedNow = TileMap.tileTypeAt(me.cx, me.cy, 2);
				me.currentFireByShortcut = true;
				me.setSkillPaint(GameScr.sks[skillToUse.skillId], (!isGroundedNow) ? 1 : 0);
				if (isGroundedNow)
				{
					me.delayFall = 20;
				}
			}
			else
			{
				GameScr.gI().doSelectSkill(skillToUse, isShortcut: true);
			}
		}
		catch
		{
		}
	}
}
