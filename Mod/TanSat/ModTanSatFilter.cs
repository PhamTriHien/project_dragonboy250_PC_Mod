using System;
using System.Collections.Generic;

/// <summary>
/// Bộ lọc mục tiêu Tàn Sát: Quản lý danh sách quái và kỹ năng được phép sử dụng trong chế độ Tàn Sát tự động.
/// </summary>
public static class ModTanSatFilter
{
	public static int selectedMobTemplateId = -1;
	public static bool selectAllMobs = true;
	public static List<int> tickedMobTemplateIds = new List<int>();

	public static bool selectAllSkills = false;
	public static int skillSelectionMode = 0; // 0: Chỉ định 1 chiêu (mặc định), 1: Kết hợp (Combo nhiều chiêu), 2: Tất cả chiêu đánh
	public static List<int> tickedSkillTemplateIds = new List<int>();

	// Nhan dien ky nang tan cong 100% dua tren thong tin metadata cua Game Engine goc
	public static bool IsAttackSkill(Skill s)
	{
		if (s == null || s.template == null) return false;
		if (s.template.isBuffToPlayer() || s.template.isUseAlone()) return false;
		sbyte id = s.template.id;
		if (id == 6 || id == 7 || id == 8 || id == 12 || id == 13 || id == 14 || id == 18 || id == 19 || id == 20 || id == 21 || id == 22 || id == 23)
		{
			return false;
		}
		return s.template.isAttackSkill() || s.template.isSkillSpec() || s.template.type == 1 || s.template.type == 4;
	}

	public static bool IsAutoAttackSkill(Skill s)
	{
		if (!IsAttackSkill(s)) return false;
		sbyte id = s.template.id;
		return id == 0 || id == 1 || id == 2 || id == 3 || id == 4 || id == 5 || id == 17;
	}

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của IsAttackSkill.
	/// </summary>
	public static bool IsAttackSkill(int templateId)
	{
		if (templateId < 0) return false;
		Skill s = FindSkillByTemplateId(templateId);
		if (s != null)
		{
			return IsAttackSkill(s);
		}
		return true;
	}

	public static bool IsAttackSkillOnly(int templateId) => IsAttackSkill(templateId);
	public static bool IsSupportSpec(int templateId) => !IsAttackSkill(templateId);

	/// <summary>
	/// Kiểm tra quái có nằm trong danh sách cho phép tấn công hay không.
	/// </summary>
	public static bool IsMobAllowed(int templateId)
	{
		if (selectAllMobs) return true;
		if (tickedMobTemplateIds.Count > 0) return tickedMobTemplateIds.Contains(templateId);
		if (selectedMobTemplateId >= 0) return templateId == selectedMobTemplateId;
		return false;
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của GetHotbarSkills.
	/// </summary>
	public static Skill[] GetHotbarSkills()
	{
		if (Main.isPC || !GameCanvas.isTouch)
		{
			return GameScr.keySkill;
		}
		return GameScr.onScreenSkill;
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của GetSkillHotbarSlot.
	/// </summary>
	public static int GetSkillHotbarSlot(int skillTemplateId)
	{
		if (skillTemplateId < 0) return -1;
		if (GameScr.keySkill != null)
		{
			for (int i = 0; i < GameScr.keySkill.Length; i++)
			{
				Skill s = GameScr.keySkill[i];
				if (s != null && s.template != null && s.template.id == skillTemplateId)
				{
					return i;
				}
			}
		}
		if (GameScr.onScreenSkill != null)
		{
			for (int j = 0; j < GameScr.onScreenSkill.Length; j++)
			{
				Skill s2 = GameScr.onScreenSkill[j];
				if (s2 != null && s2.template != null && s2.template.id == skillTemplateId)
				{
					return j;
				}
			}
		}
		return -1;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ FindSkillByTemplateId.
	/// </summary>
	public static Skill FindSkillByTemplateId(int templateId)
	{
		if (templateId < 0) return null;
		if (GameScr.keySkill != null)
		{
			for (int i = 0; i < GameScr.keySkill.Length; i++)
			{
				Skill s = GameScr.keySkill[i];
				if (s != null && s.template != null && s.template.id == templateId)
				{
					return s;
				}
			}
		}
		if (GameScr.onScreenSkill != null)
		{
			for (int j = 0; j < GameScr.onScreenSkill.Length; j++)
			{
				Skill s2 = GameScr.onScreenSkill[j];
				if (s2 != null && s2.template != null && s2.template.id == templateId)
				{
					return s2;
				}
			}
		}
		Char me = Char.myCharz();
		if (me != null)
		{
			if (me.myskill != null && me.myskill.template != null && me.myskill.template.id == templateId)
			{
				return me.myskill;
			}
			if (me.vSkillFight != null)
			{
				for (int k = 0; k < me.vSkillFight.size(); k++)
				{
					Skill s3 = (Skill)me.vSkillFight.elementAt(k);
					if (s3 != null && s3.template != null && s3.template.id == templateId)
					{
						return s3;
					}
				}
			}
			if (me.vSkill != null)
			{
				for (int m = 0; m < me.vSkill.size(); m++)
				{
					Skill s4 = (Skill)me.vSkill.elementAt(m);
					if (s4 != null && s4.template != null && s4.template.id == templateId)
					{
						return s4;
					}
				}
			}
		}
		return null;
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của GetSelectedMobName.
	/// </summary>
	public static string GetSelectedMobName()
	{
		if (selectAllMobs) return "Tất cả quái";
		if (tickedMobTemplateIds.Count > 0)
		{
			int firstId = tickedMobTemplateIds[0];
			for (int i = 0; i < GameScr.vMob.size(); i++)
			{
				Mob m = (Mob)GameScr.vMob.elementAt(i);
				if (m != null && !m.isMobMe && m.templateId == firstId && m.getTemplate() != null)
				{
					return (tickedMobTemplateIds.Count == 1) ? m.getTemplate().name : (m.getTemplate().name + " (+" + (tickedMobTemplateIds.Count - 1) + ")");
				}
			}
			return "Đã chọn " + tickedMobTemplateIds.Count + " loại quái";
		}
		if (selectedMobTemplateId == -1) return "Chưa chọn quái";
		for (int i = 0; i < GameScr.vMob.size(); i++)
		{
			Mob m = (Mob)GameScr.vMob.elementAt(i);
			if (m != null && !m.isMobMe && m.templateId == selectedMobTemplateId && m.getTemplate() != null)
			{
				return m.getTemplate().name;
			}
		}
		return "Quái #" + selectedMobTemplateId;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ CycleMobSelection.
	/// </summary>
	public static void CycleMobSelection(int dir)
	{
		List<int> available = new List<int>();
		for (int i = 0; i < GameScr.vMob.size(); i++)
		{
			Mob m = (Mob)GameScr.vMob.elementAt(i);
			if (m != null && !m.isMobMe && !available.Contains(m.templateId))
			{
				available.Add(m.templateId);
			}
		}

		if (available.Count == 0)
		{
			selectAllMobs = true;
			selectedMobTemplateId = -1;
			tickedMobTemplateIds.Clear();
			return;
		}

		if (selectAllMobs)
		{
			selectAllMobs = false;
			selectedMobTemplateId = (dir > 0) ? available[0] : available[available.Count - 1];
			tickedMobTemplateIds.Clear();
			tickedMobTemplateIds.Add(selectedMobTemplateId);
		}
		else
		{
			int curId = (selectedMobTemplateId >= 0) ? selectedMobTemplateId : ((tickedMobTemplateIds.Count == 1) ? tickedMobTemplateIds[0] : -1);
			int curIdx = available.IndexOf(curId);
			if (curIdx == -1)
			{
				selectAllMobs = true;
				selectedMobTemplateId = -1;
				tickedMobTemplateIds.Clear();
			}
			else
			{
				int nextIdx = curIdx + dir;
				if (nextIdx < 0 || nextIdx >= available.Count)
				{
					selectAllMobs = true;
					selectedMobTemplateId = -1;
					tickedMobTemplateIds.Clear();
				}
				else
				{
					selectedMobTemplateId = available[nextIdx];
					tickedMobTemplateIds.Clear();
					tickedMobTemplateIds.Add(selectedMobTemplateId);
				}
			}
		}
		ModConfig.SaveConfig();
	}

	public static bool IsMobTicked(int templateId) => tickedMobTemplateIds.Contains(templateId);

	/// <summary>
	/// Bật hoặc tắt trạng thái kích hoạt của ToggleMobTicked.
	/// </summary>
	public static void ToggleMobTicked(int templateId)
	{
		if (tickedMobTemplateIds.Contains(templateId))
		{
			tickedMobTemplateIds.Remove(templateId);
		}
		else
		{
			tickedMobTemplateIds.Add(templateId);
		}
		selectedMobTemplateId = -1;
		selectAllMobs = false;
		ModConfig.SaveConfig();
	}

	/// <summary>
	/// Bật hoặc tắt trạng thái kích hoạt của ToggleSelectAllMobs.
	/// </summary>
	public static void ToggleSelectAllMobs()
	{
		selectAllMobs = !selectAllMobs;
		tickedMobTemplateIds.Clear();
		selectedMobTemplateId = -1;
		ModConfig.SaveConfig();
	}

	/// <summary>
	/// Đảm bảo luôn có ít nhất 1 kỹ năng chỉ định mặc định (ưu tiên chiêu đang cầm hoặc đấm cơ bản) khi chưa chọn chiêu.
	/// </summary>
	public static void EnsureDesignatedSkill()
	{
		if (selectAllSkills) return;
		// Loai bo cac skill khong hop le neu co trong danh sach cu
		for (int i = tickedSkillTemplateIds.Count - 1; i >= 0; i--)
		{
			Skill existing = FindSkillByTemplateId(tickedSkillTemplateIds[i]);
			if (existing != null && !IsAttackSkill(existing))
			{
				tickedSkillTemplateIds.RemoveAt(i);
			}
		}
		if (ModTanSat.lockedSkillTemplateId >= 0)
		{
			Skill lk = FindSkillByTemplateId(ModTanSat.lockedSkillTemplateId);
			if (lk != null && !IsAttackSkill(lk))
			{
				ModTanSat.lockedSkillTemplateId = -1;
			}
		}
		if (tickedSkillTemplateIds.Count > 0)
		{
			if (skillSelectionMode == 0 && ModTanSat.lockedSkillTemplateId < 0)
			{
				ModTanSat.lockedSkillTemplateId = tickedSkillTemplateIds[0];
			}
			return;
		}
		if (ModTanSat.lockedSkillTemplateId >= 0)
		{
			tickedSkillTemplateIds.Add(ModTanSat.lockedSkillTemplateId);
			return;
		}

		Char me = Char.myCharz();
		if (me == null) return;

		Skill defaultSkill = null;
		if (me.myskill != null && IsAutoAttackSkill(me.myskill))
		{
			defaultSkill = me.myskill;
		}
		if (defaultSkill == null && GameScr.keySkill != null)
		{
			for (int i = 0; i < GameScr.keySkill.Length; i++)
			{
				if (GameScr.keySkill[i] != null && IsAutoAttackSkill(GameScr.keySkill[i]))
				{
					defaultSkill = GameScr.keySkill[i];
					break;
				}
			}
		}
		if (defaultSkill == null && GameScr.onScreenSkill != null)
		{
			for (int j = 0; j < GameScr.onScreenSkill.Length; j++)
			{
				if (GameScr.onScreenSkill[j] != null && IsAutoAttackSkill(GameScr.onScreenSkill[j]))
				{
					defaultSkill = GameScr.onScreenSkill[j];
					break;
				}
			}
		}
		if (defaultSkill == null && me.vSkillFight != null)
		{
			for (int k = 0; k < me.vSkillFight.size(); k++)
			{
				Skill s = (Skill)me.vSkillFight.elementAt(k);
				if (s != null && IsAutoAttackSkill(s))
				{
					defaultSkill = s;
					break;
				}
			}
		}
		if (defaultSkill == null && me.vSkill != null)
		{
			for (int m = 0; m < me.vSkill.size(); m++)
			{
				Skill s2 = (Skill)me.vSkill.elementAt(m);
				if (s2 != null && IsAutoAttackSkill(s2))
				{
					defaultSkill = s2;
					break;
				}
			}
		}

		if (defaultSkill != null && defaultSkill.template != null)
		{
			tickedSkillTemplateIds.Add(defaultSkill.template.id);
			if (skillSelectionMode == 0)
			{
				ModTanSat.lockedSkillTemplateId = defaultSkill.template.id;
			}
		}
	}

	/// <summary>
	/// Chỉ định duy nhất 1 kỹ năng cố định cho Tàn Sát (không tự động đổi sang chiêu khác).
	/// </summary>
	public static void DesignateSingleSkill(int templateId)
	{
		if (templateId < 0) return;
		skillSelectionMode = 0;
		selectAllSkills = false;
		ModTanSat.lockedSkillTemplateId = templateId;
		tickedSkillTemplateIds.Clear();
		tickedSkillTemplateIds.Add(templateId);
		ModConfig.SaveConfig();
	}

	/// <summary>
	/// Thiết lập chế độ chọn kỹ năng Tàn Sát: 0 = Chỉ 1 chiêu, 1 = Kết hợp (Combo), 2 = Tất cả.
	/// </summary>
	public static void SetSkillSelectionMode(int mode)
	{
		skillSelectionMode = mode;
		if (mode == 0)
		{
			selectAllSkills = false;
			EnsureDesignatedSkill();
			if (tickedSkillTemplateIds.Count > 0)
			{
				int keepId = (ModTanSat.lockedSkillTemplateId >= 0) ? ModTanSat.lockedSkillTemplateId : tickedSkillTemplateIds[0];
				ModTanSat.lockedSkillTemplateId = keepId;
				tickedSkillTemplateIds.Clear();
				tickedSkillTemplateIds.Add(keepId);
			}
		}
		else if (mode == 1)
		{
			selectAllSkills = false;
			if (ModTanSat.lockedSkillTemplateId >= 0 && !tickedSkillTemplateIds.Contains(ModTanSat.lockedSkillTemplateId))
			{
				tickedSkillTemplateIds.Add(ModTanSat.lockedSkillTemplateId);
			}
			ModTanSat.lockedSkillTemplateId = -1;
			EnsureDesignatedSkill();
		}
		else
		{
			skillSelectionMode = 2;
			selectAllSkills = true;
			ModTanSat.lockedSkillTemplateId = -1;
			tickedSkillTemplateIds.Clear();
		}
		ModConfig.SaveConfig();
	}

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của IsSkillAllowed.
	/// </summary>
	public static bool IsSkillAllowed(Skill s)
	{
		if (s == null || s.template == null) return false;
		EnsureDesignatedSkill();
		if (selectAllSkills) return IsAutoAttackSkill(s);
		if (skillSelectionMode == 0 && ModTanSat.lockedSkillTemplateId >= 0)
		{
			return s.template.id == ModTanSat.lockedSkillTemplateId;
		}
		return tickedSkillTemplateIds.Contains(s.template.id);
	}

	public static bool IsSkillAllowedBySetup(Skill s) => IsSkillAllowed(s);

	public static bool IsSkillTicked(int templateId)
	{
		EnsureDesignatedSkill();
		if (selectAllSkills) return false;
		if (skillSelectionMode == 0 && ModTanSat.lockedSkillTemplateId >= 0)
		{
			return templateId == ModTanSat.lockedSkillTemplateId;
		}
		return tickedSkillTemplateIds.Contains(templateId);
	}

	/// <summary>
	/// Bật hoặc tắt trạng thái kích hoạt của ToggleSkillTicked.
	/// </summary>
	public static void ToggleSkillTicked(int templateId)
	{
		if (skillSelectionMode == 0 || selectAllSkills)
		{
			DesignateSingleSkill(templateId);
			return;
		}

		// Chế độ 1: Kết hợp (Combo nhiều chiêu)
		selectAllSkills = false;
		ModTanSat.lockedSkillTemplateId = -1;
		if (tickedSkillTemplateIds.Contains(templateId))
		{
			if (tickedSkillTemplateIds.Count > 1)
			{
				tickedSkillTemplateIds.Remove(templateId);
			}
		}
		else
		{
			tickedSkillTemplateIds.Add(templateId);
		}
		ModConfig.SaveConfig();
	}

	/// <summary>
	/// Bật hoặc tắt trạng thái kích hoạt của ToggleSelectAllSkills.
	/// </summary>
	public static void ToggleSelectAllSkills()
	{
		if (selectAllSkills)
		{
			SetSkillSelectionMode(0);
		}
		else
		{
			SetSkillSelectionMode(2);
		}
	}

	public static int GetEffectiveSkillCooldown(Skill s)
	{
		if (s == null) return 300;
		int baseCd = (ModTanSat.freezeSkillCooldown || (s.template != null && ModTanSat.frozenSkillIds.Contains(s.template.id)))
			? ModTanSat.timeAttack
			: ((s.coolDown < ModTanSat.timeAttack) ? ModTanSat.timeAttack : s.coolDown);
		if (s.template != null && ModTanSat.customSkillDelays.TryGetValue(s.template.id, out int customDelay) && customDelay > 0)
		{
			if (customDelay > baseCd) baseCd = customDelay;
		}
		return baseCd;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ HasEnoughMp.
	/// </summary>
	public static bool HasEnoughMp(Char me, Skill s)
	{
		if (me == null || s == null || s.template == null) return false;
		if (s.template.manaUseType == 1) return me.cMP >= (long)me.cMPFull * (long)s.manaUse / 100L;
		if (s.template.manaUseType == 2) return me.cMP >= 1L;
		return me.cMP >= s.manaUse;
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của GetBestSkillToUse.
	/// </summary>
	public static Skill GetBestSkillToUse()
	{
		Char me = Char.myCharz();
		if (me == null) return null;

		EnsureDesignatedSkill();
		long now = mSystem.currentTimeMillis();

		// 1. CHẾ ĐỘ CHỈ ĐỊNH 1 CHIÊU CỐ ĐỊNH (skillSelectionMode == 0 hoặc lockedSkillTemplateId >= 0)
		if (!selectAllSkills && (skillSelectionMode == 0 || ModTanSat.lockedSkillTemplateId >= 0))
		{
			int targetTplId = (ModTanSat.lockedSkillTemplateId >= 0)
				? ModTanSat.lockedSkillTemplateId
				: ((tickedSkillTemplateIds.Count > 0) ? tickedSkillTemplateIds[0] : -1);
			if (targetTplId >= 0)
			{
				Skill designated = FindSkillByTemplateId(targetTplId);
				if (designated != null && designated.template != null && IsAttackSkill(designated))
				{
					return designated;
				}
			}
		}

		// 2. CHẾ ĐỘ KẾT HỢP (COMBO CÁC CHIÊU ĐÃ TICK TRONG DANH SÁCH)
		if (!selectAllSkills && tickedSkillTemplateIds.Count > 0)
		{
			for (int k = 0; k < tickedSkillTemplateIds.Count; k++)
			{
				int tplId = tickedSkillTemplateIds[k];
				Skill s = FindSkillByTemplateId(tplId);
				if (s != null && s.template != null && IsAttackSkill(s))
				{
					int effCd = GetEffectiveSkillCooldown(s);
					if (now - s.lastTimeUseThisSkill >= effCd && HasEnoughMp(me, s))
					{
						return s;
					}
				}
			}

			for (int k = 0; k < tickedSkillTemplateIds.Count; k++)
			{
				int tplId2 = tickedSkillTemplateIds[k];
				Skill s3 = FindSkillByTemplateId(tplId2);
				if (s3 != null && s3.template != null && IsAttackSkill(s3))
				{
					return s3;
				}
			}
		}

		// 3. CHẾ ĐỘ TẤT CẢ KỸ NĂNG ĐÁNH CƠ BẢN (selectAllSkills == true): Ưu tiên theo thứ tự ô phím thay vì chọn bừa theo cooldown cao nhất
		if (GameScr.keySkill != null)
		{
			for (int i = 0; i < GameScr.keySkill.Length; i++)
			{
				Skill s = GameScr.keySkill[i];
				if (s != null && IsAutoAttackSkill(s) && now - s.lastTimeUseThisSkill >= GetEffectiveSkillCooldown(s) && HasEnoughMp(me, s))
				{
					return s;
				}
			}
			for (int i = 0; i < GameScr.keySkill.Length; i++)
			{
				Skill s = GameScr.keySkill[i];
				if (s != null && IsAutoAttackSkill(s)) return s;
			}
		}

		if (GameScr.onScreenSkill != null)
		{
			for (int j = 0; j < GameScr.onScreenSkill.Length; j++)
			{
				Skill s = GameScr.onScreenSkill[j];
				if (s != null && IsAutoAttackSkill(s) && now - s.lastTimeUseThisSkill >= GetEffectiveSkillCooldown(s) && HasEnoughMp(me, s))
				{
					return s;
				}
			}
			for (int j = 0; j < GameScr.onScreenSkill.Length; j++)
			{
				Skill s = GameScr.onScreenSkill[j];
				if (s != null && IsAutoAttackSkill(s)) return s;
			}
		}

		if (me.vSkillFight != null)
		{
			for (int k = 0; k < me.vSkillFight.size(); k++)
			{
				Skill s = (Skill)me.vSkillFight.elementAt(k);
				if (s != null && IsAutoAttackSkill(s) && now - s.lastTimeUseThisSkill >= GetEffectiveSkillCooldown(s) && HasEnoughMp(me, s))
				{
					return s;
				}
			}
			for (int k = 0; k < me.vSkillFight.size(); k++)
			{
				Skill s = (Skill)me.vSkillFight.elementAt(k);
				if (s != null && IsAutoAttackSkill(s)) return s;
			}
		}

		if (me.vSkill != null)
		{
			for (int m = 0; m < me.vSkill.size(); m++)
			{
				Skill s2 = (Skill)me.vSkill.elementAt(m);
				if (s2 != null && IsAutoAttackSkill(s2) && now - s2.lastTimeUseThisSkill >= GetEffectiveSkillCooldown(s2) && HasEnoughMp(me, s2))
				{
					return s2;
				}
			}
		}

		if (me.myskill != null && IsAttackSkill(me.myskill))
		{
			return me.myskill;
		}

		return null;
	}
}
