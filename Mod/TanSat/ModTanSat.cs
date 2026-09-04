using System;
using System.Collections.Generic;

public static class ModTanSat
{
	public static bool autoTanSat = false;
	public static bool autoAttack
	{
		get { return autoTanSat; }
		set { autoTanSat = value; }
	}

	public static bool useTeleport = true;
	public static Mob currentFarmTarget = null;
	public static long targetLockTime = 0;
	public static long targetLastHp = 0;

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
	public static List<int> tickedMobTemplateIds
	{
		get { return ModTanSatFilter.tickedMobTemplateIds; }
	}

	public static int selectedSkillTemplateId
	{
		get { return ModTanSatFilter.selectedSkillTemplateId; }
		set { ModTanSatFilter.selectedSkillTemplateId = value; }
	}
	public static bool selectAllSkills
	{
		get { return ModTanSatFilter.selectAllSkills; }
		set { ModTanSatFilter.selectAllSkills = value; }
	}
	public static List<int> tickedSkillTemplateIds
	{
		get { return ModTanSatFilter.tickedSkillTemplateIds; }
	}

	public static string GetSelectedMobName() => ModTanSatFilter.GetSelectedMobName();
	public static void CycleMobSelection(int dir) => ModTanSatFilter.CycleMobSelection(dir);
	public static string GetSelectedSkillName() => ModTanSatFilter.GetSelectedSkillName();
	public static void CycleSkillSelection(int dir) => ModTanSatFilter.CycleSkillSelection(dir);
	public static bool IsMobTicked(int templateId) => ModTanSatFilter.IsMobTicked(templateId);
	public static void ToggleMobTicked(int templateId) => ModTanSatFilter.ToggleMobTicked(templateId);
	public static void ToggleSelectAllMobs() => ModTanSatFilter.ToggleSelectAllMobs();
	public static bool IsSkillTicked(int templateId) => ModTanSatFilter.IsSkillTicked(templateId);
	public static void ToggleSkillTicked(int templateId) => ModTanSatFilter.ToggleSkillTicked(templateId);
	public static void ToggleSelectAllSkills() => ModTanSatFilter.ToggleSelectAllSkills();
	public static Skill GetBestSkillToUse() => ModTanSatFilter.GetBestSkillToUse();

	public static bool IsTileBlocked(int px, int py) => ModTanSatTargeting.IsTileBlocked(px, py);
	public static void GetSafeAttackPosition(Mob target, bool isRanged, out int outX, out int outY)
	{
		ModTanSatTargeting.GetSafeAttackPosition(target, isRanged, out outX, out outY);
	}

	public static void RunTanSat()
	{
		try
		{
			if (!autoTanSat)
			{
				return;
			}

			// Chỉ tạm dừng khi đang Next Map hoặc khi đang chuyển map
			if (ModNextMap.isNextMapActive || Char.isLoadingMap || Char.ischangingMap)
			{
				return;
			}

			Char me = Char.myCharz();
			if (me == null || me.cHP <= 0 || me.statusMe == 14 || me.statusMe == 5)
			{
				return;
			}

			long now = mSystem.currentTimeMillis();

			// Watchdog chống kẹt quái ma
			if (currentFarmTarget != null)
			{
				if (currentFarmTarget.status == 0 || currentFarmTarget.status == 1 || currentFarmTarget.hp <= 0)
				{
					currentFarmTarget = null;
				}
				else if (now - targetLockTime > 4000 && currentFarmTarget.hp >= targetLastHp)
				{
					// Quái ma quá 4s không mất máu: Bỏ qua và đổi mục tiêu
					currentFarmTarget = null;
				}
			}

			// Tìm quái mục tiêu mới nếu chưa có
			if (currentFarmTarget == null)
			{
				MyVector mobs = GameScr.vMob;
				if (mobs == null || mobs.size() == 0)
				{
					return;
				}

				Mob bestMob = null;
				int minDistance = int.MaxValue;

				for (int i = 0; i < mobs.size(); i++)
				{
					Mob m = (Mob)mobs.elementAt(i);
					if (m == null || m.status == 0 || m.status == 1 || m.hp <= 0)
					{
						continue;
					}

					// Lọc theo cấu hình quái
					if (!selectAllMobs && tickedMobTemplateIds.Count > 0 && !tickedMobTemplateIds.Contains(m.templateId))
					{
						continue;
					}

					int dist = Res.distance(me.cx, me.cy, m.x, m.y);
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

			if (currentFarmTarget == null)
			{
				return;
			}

			// Chọn kỹ năng tối ưu (đã kiểm tra hết cooldown)
			Skill skillToUse = GetBestSkillToUse();
			if (skillToUse == null)
			{
				return;
			}

			bool isRanged = (skillToUse.dx > 40);
			int safeX, safeY;
			GetSafeAttackPosition(currentFarmTarget, isRanged, out safeX, out safeY);

			int distToTarget = Res.distance(me.cx, me.cy, safeX, safeY);
			int maxAttackDist = isRanged ? 60 : 30;

			// Tiếp cận quái
			if (distToTarget > maxAttackDist)
			{
				if (useTeleport)
				{
					ModTeleport.TeleportTo(safeX, safeY);
					me.cx = safeX;
					me.cy = safeY;
					me.statusMe = 1;
					me.cvx = 0;
					me.cvy = 0;
					me.delayFall = 30; // Chống rơi tự do khi đánh quái bay
				}
				else
				{
					me.moveTo(safeX, safeY, 0);
					return;
				}
			}

			// Đã ở vị trí áp sát quái: Ổn định trạng thái và hướng mặt về phía quái
			me.cvx = 0;
			me.cvy = 0;
			me.statusMe = 1;
			me.delayFall = 30; // Giữ thăng bằng trên không
			me.cdir = (currentFarmTarget.x >= me.cx) ? 1 : -1;
			me.mobFocus = currentFarmTarget;
			me.charFocus = null;

			// Đồng bộ kỹ năng với Server nếu đổi chiêu
			if (me.myskill != skillToUse)
			{
				me.myskill = skillToUse;
				Service.gI().selectSkill(skillToUse.template.id);
				GameScr.lastSkill = skillToUse;
			}

			// Tập hợp danh sách quái tấn công (gói tin 54 chuẩn server NRO)
			MyVector vMobAttack = new MyVector();
			vMobAttack.addElement(currentFarmTarget);

			// Cập nhật mốc thời gian hồi chiêu
			skillToUse.lastTimeUseThisSkill = now;

			// Gửi gói tin tấn công thật lên server NGAY LẬP TỨC (Frame 0 - Zero Delay Damage)
			Service.gI().sendPlayerAttack(vMobAttack, new MyVector(), 1);

			// Kích hoạt hiệu ứng đánh và animation visual nguyên bản mà không bị gửi lặp gói tin
			me.hasSendAttack = true;
			bool isGrounded = TileMap.tileTypeAt(me.cx, me.cy, 2);
			me.setSkillPaint(GameScr.sks[skillToUse.skillId], (!isGrounded) ? 1 : 0);
		}
		catch
		{
		}
	}
}
