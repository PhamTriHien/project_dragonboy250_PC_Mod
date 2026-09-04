using System;
using System.Collections.Generic;

public static class ModTanSatFilter
{
	public static int selectedMobTemplateId = -1;
	public static bool selectAllMobs = true;
	public static List<int> tickedMobTemplateIds = new List<int>();

	public static int selectedSkillTemplateId = -1;
	public static bool selectAllSkills = true;
	public static List<int> tickedSkillTemplateIds = new List<int>();

	public static string GetSelectedMobName()
	{
		if (selectAllMobs || selectedMobTemplateId == -1)
		{
			return "Tất cả quái";
		}
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

	public static string GetSelectedSkillName()
	{
		if (selectAllSkills || selectedSkillTemplateId == -1)
		{
			return "Tất cả chiêu";
		}
		Char me = Char.myCharz();
		if (me != null && me.vSkill != null)
		{
			for (int i = 0; i < me.vSkill.size(); i++)
			{
				Skill s = (Skill)me.vSkill.elementAt(i);
				if (s != null && s.template != null && s.template.id == selectedSkillTemplateId)
				{
					return s.template.name;
				}
			}
		}
		return "Skill #" + selectedSkillTemplateId;
	}

	public static void CycleSkillSelection(int dir)
	{
		Char me = Char.myCharz();
		if (me == null || me.vSkill == null || me.vSkill.size() == 0)
		{
			selectAllSkills = true;
			selectedSkillTemplateId = -1;
			return;
		}

		List<int> available = new List<int>();
		for (int i = 0; i < me.vSkill.size(); i++)
		{
			Skill s = (Skill)me.vSkill.elementAt(i);
			if (s != null && s.template != null && !available.Contains(s.template.id))
			{
				available.Add(s.template.id);
			}
		}

		if (available.Count == 0)
		{
			selectAllSkills = true;
			selectedSkillTemplateId = -1;
			return;
		}

		if (selectAllSkills)
		{
			selectAllSkills = false;
			selectedSkillTemplateId = (dir > 0) ? available[0] : available[available.Count - 1];
		}
		else
		{
			int curIdx = available.IndexOf(selectedSkillTemplateId);
			if (curIdx == -1)
			{
				selectAllSkills = true;
				selectedSkillTemplateId = -1;
			}
			else
			{
				int nextIdx = curIdx + dir;
				if (nextIdx < 0 || nextIdx >= available.Count)
				{
					selectAllSkills = true;
					selectedSkillTemplateId = -1;
				}
				else
				{
					selectedSkillTemplateId = available[nextIdx];
				}
			}
		}
		ModConfig.SaveConfig();
	}

	public static bool IsMobTicked(int templateId)
	{
		return tickedMobTemplateIds.Contains(templateId);
	}

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

	public static bool IsSkillTicked(int templateId)
	{
		return tickedSkillTemplateIds.Contains(templateId);
	}

	public static void ToggleSkillTicked(int templateId)
	{
		if (tickedSkillTemplateIds.Contains(templateId))
		{
			tickedSkillTemplateIds.Remove(templateId);
		}
		else
		{
			tickedSkillTemplateIds.Add(templateId);
		}
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
			Char me = Char.myCharz();
			if (me != null && me.vSkill != null)
			{
				for (int i = 0; i < me.vSkill.size(); i++)
				{
					Skill s = (Skill)me.vSkill.elementAt(i);
					if (s != null && s.template != null && !tickedSkillTemplateIds.Contains(s.template.id))
					{
						tickedSkillTemplateIds.Add(s.template.id);
					}
				}
			}
		}
		ModConfig.SaveConfig();
	}

	public static Skill GetBestSkillToUse()
	{
		Char me = Char.myCharz();
		if (me == null || me.vSkill == null || me.vSkill.size() == 0)
		{
			return null;
		}

		long now = mSystem.currentTimeMillis();

		// 1. Tìm skill đặc biệt thỏa mãn điều kiện và đã hết thời gian hồi chiêu
		for (int i = 0; i < me.vSkill.size(); i++)
		{
			Skill s = (Skill)me.vSkill.elementAt(i);
			if (s == null || s.template == null)
			{
				continue;
			}

			// Bỏ qua các kỹ năng buff hỗ trợ, tự sát, biến khỉ
			int tId = s.template.id;
			if (tId == 7 || tId == 8 || tId == 9 || tId == 10 || tId == 14 || tId == 19 || tId == 21 || tId == 22 || tId == 23)
			{
				continue;
			}

			if (!selectAllSkills && tickedSkillTemplateIds.Count > 0 && !tickedSkillTemplateIds.Contains(tId))
			{
				continue;
			}

			if (me.cMP >= s.manaUse && now >= s.lastTimeUseThisSkill + s.coolDown)
			{
				return s;
			}
		}

		// 2. Fallback về kỹ năng cơ bản (skill 0 - đấm thường) khi đã hết thời gian hồi
		if (me.vSkill.size() > 0)
		{
			Skill basicSkill = (Skill)me.vSkill.elementAt(0);
			if (basicSkill != null && me.cMP >= basicSkill.manaUse && now >= basicSkill.lastTimeUseThisSkill + basicSkill.coolDown)
			{
				return basicSkill;
			}
		}

		return null;
	}
}
