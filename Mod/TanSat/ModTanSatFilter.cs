using System;
using System.Collections.Generic;

public static class ModTanSatFilter
{
	public static int selectedMobTemplateId = -1;
	public static bool selectAllMobs = true;
	public static List<int> tickedMobTemplateIds = new List<int>();

	public static bool selectAllSkills = true;
	public static List<int> tickedSkillTemplateIds = new List<int>();

	// Nhan dien ky nang tan cong 100% dua tren thong tin metadata cua Game Engine goc
	public static bool IsAttackSkill(Skill s)
	{
		if (s == null || s.template == null) return false;
		if (s.template.isBuffToPlayer()) return false;
		return s.template.isAttackSkill() || s.template.isSkillSpec() || s.template.type == 1 || s.template.type == 4;
	}

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

	public static Skill[] GetHotbarSkills()
	{
		if (Main.isPC || !GameCanvas.isTouch)
		{
			return GameScr.keySkill;
		}
		return GameScr.onScreenSkill;
	}

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

	public static string GetSelectedMobName()
	{
		if (selectAllMobs || selectedMobTemplateId == -1) return "Tất cả quái";
		for (int i = 0; i < GameScr.vMob.size(); i++)
		{
			Mob m = (Mob)GameScr.vMob.elementAt(i);
			if (m != null && m.templateId == selectedMobTemplateId)
			{
				return m.getTemplate().name;
			}
		}
		return "Quái #" + selectedMobTemplateId;
	}

	public static void CycleMobSelection(int dir)
	{
		List<int> available = new List<int>();
		for (int i = 0; i < GameScr.vMob.size(); i++)
		{
			Mob m = (Mob)GameScr.vMob.elementAt(i);
			if (m != null && !available.Contains(m.templateId))
			{
				available.Add(m.templateId);
			}
		}

		if (available.Count == 0)
		{
			selectAllMobs = true;
			selectedMobTemplateId = -1;
			return;
		}

		if (selectAllMobs)
		{
			selectAllMobs = false;
			selectedMobTemplateId = (dir > 0) ? available[0] : available[available.Count - 1];
		}
		else
		{
			int curIdx = available.IndexOf(selectedMobTemplateId);
			if (curIdx == -1)
			{
				selectAllMobs = true;
				selectedMobTemplateId = -1;
			}
			else
			{
				int nextIdx = curIdx + dir;
				if (nextIdx < 0 || nextIdx >= available.Count)
				{
					selectAllMobs = true;
					selectedMobTemplateId = -1;
				}
				else
				{
					selectedMobTemplateId = available[nextIdx];
				}
			}
		}
		ModConfig.SaveConfig();
	}

	public static bool IsMobTicked(int templateId) => tickedMobTemplateIds.Contains(templateId);

	public static void ToggleMobTicked(int templateId)
	{
		if (tickedMobTemplateIds.Contains(templateId)) tickedMobTemplateIds.Remove(templateId);
		else tickedMobTemplateIds.Add(templateId);
		selectAllMobs = (tickedMobTemplateIds.Count == 0);
		ModConfig.SaveConfig();
	}

	public static void ToggleSelectAllMobs()
	{
		selectAllMobs = !selectAllMobs;
		if (selectAllMobs)
		{
			tickedMobTemplateIds.Clear();
		}
		else
		{
			for (int i = 0; i < GameScr.vMob.size(); i++)
			{
				Mob m = (Mob)GameScr.vMob.elementAt(i);
				if (m != null && !tickedMobTemplateIds.Contains(m.templateId))
				{
					tickedMobTemplateIds.Add(m.templateId);
				}
			}
		}
		ModConfig.SaveConfig();
	}

	public static bool IsSkillAllowed(Skill s)
	{
		if (s == null || s.template == null) return false;
		if (selectAllSkills || tickedSkillTemplateIds.Count == 0) return true;
		return tickedSkillTemplateIds.Contains(s.template.id);
	}

	public static bool IsSkillAllowedBySetup(Skill s) => IsSkillAllowed(s);
	public static bool IsSkillTicked(int templateId) => tickedSkillTemplateIds.Contains(templateId);

	public static void ToggleSkillTicked(int templateId)
	{
		if (tickedSkillTemplateIds.Contains(templateId)) tickedSkillTemplateIds.Remove(templateId);
		else tickedSkillTemplateIds.Add(templateId);
		selectAllSkills = (tickedSkillTemplateIds.Count == 0);
		ModConfig.SaveConfig();
	}

	public static void ToggleSelectAllSkills()
	{
		selectAllSkills = !selectAllSkills;
		if (selectAllSkills)
		{
			tickedSkillTemplateIds.Clear();
		}
		else
		{
			List<Skill> skills = ModUI.GetPlayerAttackSkills();
			for (int i = 0; i < skills.Count; i++)
			{
				Skill s = skills[i];
				if (s != null && s.template != null && !tickedSkillTemplateIds.Contains(s.template.id))
				{
					tickedSkillTemplateIds.Add(s.template.id);
				}
			}
		}
		ModConfig.SaveConfig();
	}

	public static bool HasEnoughMp(Char me, Skill s)
	{
		if (me == null || s == null || s.template == null) return false;
		if (s.template.manaUseType == 1) return me.cMP >= (long)me.cMPFull * (long)s.manaUse / 100L;
		if (s.template.manaUseType == 2) return me.cMP >= 1L;
		return me.cMP >= s.manaUse;
	}

	public static Skill GetBestSkillToUse()
	{
		Char me = Char.myCharz();
		if (me == null) return null;

		long now = mSystem.currentTimeMillis();

		// 1. NEU NGUOI CHOI TICK CHON CAC O KY NANG CU THE (!selectAllSkills va tickedSkillTemplateIds.Count > 0)
		if (!selectAllSkills && tickedSkillTemplateIds.Count > 0)
		{
			// Uu tien chieu da het hoi chieu va du KI trong danh sach da tick
			for (int k = 0; k < tickedSkillTemplateIds.Count; k++)
			{
				int tplId = tickedSkillTemplateIds[k];
				Skill s = FindSkillByTemplateId(tplId);
				if (s != null && s.template != null)
				{
					int effCd = (s.coolDown < 300) ? 300 : s.coolDown;
					if (now - s.lastTimeUseThisSkill >= effCd && HasEnoughMp(me, s))
					{
						return s;
					}
				}
			}

			// Neu tat ca chieu da tick dang trong thoi gian hoi chieu, giu nguyen chieu dau tien da tick de cho hoi chieu
			for (int k = 0; k < tickedSkillTemplateIds.Count; k++)
			{
				int tplId2 = tickedSkillTemplateIds[k];
				Skill s3 = FindSkillByTemplateId(tplId2);
				if (s3 != null && s3.template != null)
				{
					return s3;
				}
			}
		}

		// 2. NEU NGUOI CHOI CHON "TAT CA KY NANG" HOAC FALLBACK DUNG KY NANG THUC TE CUA NHAN VAT
		if (GameScr.keySkill != null)
		{
			Skill bestSpecial = null;
			for (int i = 0; i < GameScr.keySkill.Length; i++)
			{
				Skill s = GameScr.keySkill[i];
				if (s != null && s.template != null && now - s.lastTimeUseThisSkill >= s.coolDown && HasEnoughMp(me, s))
				{
					if (bestSpecial == null || s.coolDown > bestSpecial.coolDown)
					{
						bestSpecial = s;
					}
				}
			}
			if (bestSpecial != null) return bestSpecial;

			for (int i = 0; i < GameScr.keySkill.Length; i++)
			{
				Skill s = GameScr.keySkill[i];
				if (s != null && s.template != null) return s;
			}
		}

		if (GameScr.onScreenSkill != null)
		{
			Skill bestSpecial2 = null;
			for (int j = 0; j < GameScr.onScreenSkill.Length; j++)
			{
				Skill s = GameScr.onScreenSkill[j];
				if (s != null && s.template != null && now - s.lastTimeUseThisSkill >= s.coolDown && HasEnoughMp(me, s))
				{
					if (bestSpecial2 == null || s.coolDown > bestSpecial2.coolDown)
					{
						bestSpecial2 = s;
					}
				}
			}
			if (bestSpecial2 != null) return bestSpecial2;

			for (int j = 0; j < GameScr.onScreenSkill.Length; j++)
			{
				Skill s = GameScr.onScreenSkill[j];
				if (s != null && s.template != null) return s;
			}
		}

		if (me.vSkillFight != null)
		{
			for (int k = 0; k < me.vSkillFight.size(); k++)
			{
				Skill s = (Skill)me.vSkillFight.elementAt(k);
				if (s != null && s.template != null && now - s.lastTimeUseThisSkill >= s.coolDown && HasEnoughMp(me, s))
				{
					return s;
				}
			}
			for (int k = 0; k < me.vSkillFight.size(); k++)
			{
				Skill s = (Skill)me.vSkillFight.elementAt(k);
				if (s != null && s.template != null) return s;
			}
		}

		if (me.vSkill != null)
		{
			for (int m = 0; m < me.vSkill.size(); m++)
			{
				Skill s2 = (Skill)me.vSkill.elementAt(m);
				if (s2 != null && s2.template != null && now - s2.lastTimeUseThisSkill >= s2.coolDown && HasEnoughMp(me, s2))
				{
					return s2;
				}
			}
		}

		if (me.myskill != null && me.myskill.template != null)
		{
			return me.myskill;
		}

		return null;
	}
}
