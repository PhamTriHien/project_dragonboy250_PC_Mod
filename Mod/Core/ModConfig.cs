using System;
using System.IO;
using System.Text;
using UnityEngine;

public static class ModConfig
{
	private static string ConfigPath
	{
		get
		{
			try
			{
				return Path.Combine(Application.dataPath, "../mod_config.ini");
			}
			catch
			{
				return "mod_config.ini";
			}
		}
	}

	public static void SaveConfig()
	{
		try
		{
			StringBuilder sb = new StringBuilder();

			// Tàn Sát
			sb.AppendLine("autoTanSat=" + ModTanSat.autoTanSat);
			sb.AppendLine("useTeleport=" + ModTanSat.useTeleport);
			sb.AppendLine("selectAllMobs=" + ModTanSat.selectAllMobs);
			sb.AppendLine("tickedMobTemplateIds=" + string.Join(",", ModTanSat.tickedMobTemplateIds.ConvertAll(i => i.ToString()).ToArray()));
			sb.AppendLine("selectAllSkills=" + ModTanSat.selectAllSkills);
			sb.AppendLine("tickedSkillTemplateIds=" + string.Join(",", ModTanSat.tickedSkillTemplateIds.ConvertAll(i => i.ToString()).ToArray()));

			// Tự Nhặt
			sb.AppendLine("autoPick=" + ModAutoPick.autoPick);
			sb.AppendLine("pickAll=" + ModAutoPick.pickAll);
			sb.AppendLine("pickGold=" + ModAutoPick.pickGold);
			sb.AppendLine("pickEquip=" + ModAutoPick.pickEquip);
			sb.AppendLine("pickGem=" + ModAutoPick.pickGem);

			// Tốc Chạy
			sb.AppendLine("speedHack=" + ModSpeed.speedHack);
			sb.AppendLine("speedMult=" + ModSpeed.speedMult);

			// Bơm Đậu & HP
			sb.AppendLine("autoPean=" + ModAutoHeal.autoPean);
			sb.AppendLine("autoPeanHpPercent=" + ModAutoHeal.autoPeanHpPercent);
			sb.AppendLine("lockHPMP=" + ModAutoHeal.lockHPMP);

			// Đồ Họa & FPS
			sb.AppendLine("graphicsQuality=" + ModGraphics.graphicsQuality);
			sb.AppendLine("targetFps=" + ModFps.targetFps);
			sb.AppendLine("isAutoFps=" + ModFps.isAutoFps);

			// Thông Báo Boss
			sb.AppendLine("isShowBossNotice=" + ModBossNotice.isShowBossNotice);

			File.WriteAllText(ConfigPath, sb.ToString());
		}
		catch
		{
		}
	}

	public static void LoadConfig()
	{
		try
		{
			if (!File.Exists(ConfigPath))
			{
				SaveConfig();
				return;
			}

			string[] lines = File.ReadAllLines(ConfigPath);
			for (int i = 0; i < lines.Length; i++)
			{
				string line = lines[i].Trim();
				if (string.IsNullOrEmpty(line) || line.StartsWith("#"))
				{
					continue;
				}

				int eq = line.IndexOf('=');
				if (eq <= 0)
				{
					continue;
				}

				string key = line.Substring(0, eq).Trim();
				string val = line.Substring(eq + 1).Trim();

				switch (key)
				{
					case "autoTanSat":
						bool.TryParse(val, out ModTanSat.autoTanSat);
						break;
					case "useTeleport":
						bool.TryParse(val, out ModTanSat.useTeleport);
						break;
					case "selectAllMobs":
						bool.TryParse(val, out ModTanSatFilter.selectAllMobs);
						break;
					case "tickedMobTemplateIds":
						ModTanSat.tickedMobTemplateIds.Clear();
						if (!string.IsNullOrEmpty(val))
						{
							string[] p = val.Split(',');
							for (int j = 0; j < p.Length; j++)
							{
								int id;
								if (int.TryParse(p[j].Trim(), out id))
								{
									ModTanSat.tickedMobTemplateIds.Add(id);
								}
							}
						}
						break;
					case "selectAllSkills":
						bool.TryParse(val, out ModTanSatFilter.selectAllSkills);
						break;
					case "tickedSkillTemplateIds":
						ModTanSat.tickedSkillTemplateIds.Clear();
						if (!string.IsNullOrEmpty(val))
						{
							string[] p2 = val.Split(',');
							for (int k = 0; k < p2.Length; k++)
							{
								int id2;
								if (int.TryParse(p2[k].Trim(), out id2))
								{
									ModTanSat.tickedSkillTemplateIds.Add(id2);
								}
							}
						}
						break;
					case "autoPick":
						bool.TryParse(val, out ModAutoPick.autoPick);
						break;
					case "pickAll":
						bool.TryParse(val, out ModAutoPick.pickAll);
						break;
					case "pickGold":
						bool.TryParse(val, out ModAutoPick.pickGold);
						break;
					case "pickEquip":
						bool.TryParse(val, out ModAutoPick.pickEquip);
						break;
					case "pickGem":
						bool.TryParse(val, out ModAutoPick.pickGem);
						break;
					case "speedHack":
						bool.TryParse(val, out ModSpeed.speedHack);
						break;
					case "speedMult":
						float.TryParse(val, out ModSpeed.speedMult);
						break;
					case "autoPean":
						bool.TryParse(val, out ModAutoHeal.autoPean);
						break;
					case "autoPeanHpPercent":
						int.TryParse(val, out ModAutoHeal.autoPeanHpPercent);
						break;
					case "lockHPMP":
						bool.TryParse(val, out ModAutoHeal.lockHPMP);
						break;
					case "graphicsQuality":
						int.TryParse(val, out ModGraphics.graphicsQuality);
						break;
					case "targetFps":
						int.TryParse(val, out ModFps.targetFps);
						break;
					case "isAutoFps":
						bool.TryParse(val, out ModFps.isAutoFps);
						break;
					case "isShowBossNotice":
						bool.TryParse(val, out ModBossNotice.isShowBossNotice);
						break;
				}
			}
		}
		catch
		{
		}
	}
}
