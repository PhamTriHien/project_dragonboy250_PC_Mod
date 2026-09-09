using System;
using System.IO;
using System.Text;
using UnityEngine;

public static class ModConfig
{
	public static bool isTranslate = true;

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

			// Tàn Sát: Luôn lưu false để khi thoát game/khởi động lại Tàn Sát luôn TẮT
			sb.AppendLine("autoTanSat=False");
			sb.AppendLine("useTeleport=" + ModTanSat.useTeleport);
			sb.AppendLine("selectAllMobs=" + ModTanSat.selectAllMobs);
			sb.AppendLine("tickedMobTemplateIds=" + string.Join(",", ModTanSat.tickedMobTemplateIds.ConvertAll(i => i.ToString()).ToArray()));
			sb.AppendLine("selectAllSkills=" + ModTanSat.selectAllSkills);
			sb.AppendLine("tickedSkillTemplateIds=" + string.Join(",", ModTanSat.tickedSkillTemplateIds.ConvertAll(i => i.ToString()).ToArray()));
			sb.AppendLine("timeAttack=" + ModTanSat.timeAttack);

			// Tự Nhặt
			sb.AppendLine("autoPick=" + ModAutoPick.autoPick);
			sb.AppendLine("pickAll=" + ModAutoPick.pickAll);
			sb.AppendLine("pickGold=" + ModAutoPick.pickGold);
			sb.AppendLine("pickEquip=" + ModAutoPick.pickEquip);
			sb.AppendLine("pickGem=" + ModAutoPick.pickGem);
			sb.AppendLine("filterById=" + ModAutoPick.filterById);
			sb.AppendLine("filterIds=" + ModAutoPick.filterIdsRaw);
			sb.AppendLine("filterByName=" + ModAutoPick.filterByName);
			sb.AppendLine("filterName=" + ModAutoPick.filterNameRaw);

			// Tốc Chạy
			sb.AppendLine("speedHack=" + ModSpeed.speedHack);
			sb.AppendLine("speedMult=" + ModSpeed.speedMult);

			// Bơm Đậu & HP
			sb.AppendLine("autoPean=" + ModAutoHeal.autoPean);
			sb.AppendLine("autoPeanHpPercent=" + ModAutoHeal.autoPeanHpPercent);
			sb.AppendLine("lockHPMP=" + ModAutoHeal.lockHPMP);
			sb.AppendLine("autoHarvestPea=" + ModAutoHeal.autoHarvestPea);
			sb.AppendLine("autoDonateClan=" + ModAutoHeal.autoDonateClan);
			sb.AppendLine("autoFeedPetOnAsk=" + ModAutoHeal.autoFeedPetOnAsk);

			// Đồ Họa & FPS
			sb.AppendLine("graphicsQuality=" + ModGraphics.graphicsQuality);
			sb.AppendLine("resolutionIndex=" + ModGraphics.resolutionIndex);
			sb.AppendLine("isFullscreen=" + ModGraphics.isFullscreen);
			sb.AppendLine("targetFps=" + ModFps.targetFps);
			sb.AppendLine("isAutoFps=" + ModFps.isAutoFps);
			sb.AppendLine("isAnalog=" + (GameScr.isAnalog == 1));

			// Thông Báo Boss
			sb.AppendLine("isShowBossNotice=" + ModBossNotice.isShowBossNotice);

			// Logo TriHienKun
			sb.AppendLine("isShowLogoInGame=" + ModLogo.isShowLogoInGame);
			sb.AppendLine("logoPosition=" + ModLogo.logoPosition);

			// HUD Player & Boss trong map
			sb.AppendLine("isShowMapEntityHUD=" + ModMapEntityHUD.isShowMapEntityHUD);

			// GoBack Map
			sb.AppendLine("isGoBackActive=" + ModGoBack.isGoBackActive);
			sb.AppendLine("isAutoRecordOnDeath=" + ModGoBack.isAutoRecordOnDeath);
			sb.AppendLine("savedMapId=" + ModGoBack.savedMapId);
			sb.AppendLine("savedZoneId=" + ModGoBack.savedZoneId);
			sb.AppendLine("savedX=" + ModGoBack.savedX);
			sb.AppendLine("savedY=" + ModGoBack.savedY);

			// Úp Set Kích Hoạt & Lọc Đồ
			sb.AppendLine("autoSetKHActive=" + ModSetActivator.isActive);
			sb.AppendLine("autoSellJunkFullBag=" + ModSetActivator.autoSellJunk);
			sb.AppendLine("minStarToKeep=" + ModSetActivator.minStarToKeep);
			sb.AppendLine("showItemId=" + ModSetActivator.showItemId);
			sb.AppendLine("savedFarmMapId=" + ModSetActivator.savedFarmMapId);
			sb.AppendLine("savedFarmZoneId=" + ModSetActivator.savedFarmZoneId);
			sb.AppendLine("savedFarmX=" + ModSetActivator.savedFarmX);
			sb.AppendLine("savedFarmY=" + ModSetActivator.savedFarmY);

			// Né Broly & Khinh Công
			sb.AppendLine("isAutoKiteBroly=" + ModKiteBroly.isAutoKite);
			sb.AppendLine("isKhinhCongBroly=" + ModKiteBroly.isKhinhCong);
			sb.AppendLine("safeDistanceBroly=" + ModKiteBroly.safeDistance);
			sb.AppendLine("autoAttackBroly=" + ModKiteBroly.autoAttackBroly);

			// Tối ưu Tỉ Lệ Rơi Đồ (Trick Drop Rate)
			sb.AppendLine("isInstantPick=" + ModDropRate.isInstantPick);
			sb.AppendLine("isInstantRespawnAttack=" + ModDropRate.isInstantRespawnAttack);
			sb.AppendLine("isLastHitLock=" + ModDropRate.isLastHitLock);

			// Auto Mua Bùa Bà Hạt Mít
			sb.AppendLine("isAutoRebuyBua=" + ModAutoBuyBua.isAutoRebuy);
			sb.AppendLine("selectedBuaType=" + ModAutoBuyBua.selectedBuaType);
			sb.AppendLine("selectedBuaPackage=" + ModAutoBuyBua.selectedPackage);

			// Việt Hoá Data Server
			sb.AppendLine("isTranslate=" + isTranslate);

			// Hình Nền Phong Cảnh Git
			sb.AppendLine("isCustomBGActive=" + ModBackground.isCustomBGActive);
			sb.AppendLine("selectedBgId=" + ModBackground.selectedBgId);

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
						ModTanSat.autoTanSat = false;
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
					case "timeAttack":
						if (int.TryParse(val, out int ta))
						{
							if (ta < 50) ta = 50;
							if (ta > 3000) ta = 3000;
							ModTanSat.timeAttack = ta;
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
					case "filterById":
						bool.TryParse(val, out ModAutoPick.filterById);
						break;
					case "filterIds":
						ModAutoPick.LoadFilterIds(val);
						break;
					case "filterByName":
						bool.TryParse(val, out ModAutoPick.filterByName);
						break;
					case "filterName":
						ModAutoPick.LoadFilterNames(val);
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
					case "autoHarvestPea":
						bool.TryParse(val, out ModAutoHeal.autoHarvestPea);
						break;
					case "autoDonateClan":
						bool.TryParse(val, out ModAutoHeal.autoDonateClan);
						break;
					case "autoFeedPetOnAsk":
						bool.TryParse(val, out ModAutoHeal.autoFeedPetOnAsk);
						break;
					case "graphicsQuality":
						int.TryParse(val, out ModGraphics.graphicsQuality);
						break;
					case "resolutionIndex":
						int.TryParse(val, out ModGraphics.resolutionIndex);
						break;
					case "isFullscreen":
						bool.TryParse(val, out ModGraphics.isFullscreen);
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
					case "isShowLogoInGame":
						bool.TryParse(val, out ModLogo.isShowLogoInGame);
						break;
					case "logoPosition":
						int.TryParse(val, out ModLogo.logoPosition);
						break;
					case "isShowMapEntityHUD":
						bool.TryParse(val, out ModMapEntityHUD.isShowMapEntityHUD);
						break;
					case "isGoBackActive":
						bool.TryParse(val, out ModGoBack.isGoBackActive);
						break;
					case "isAutoRecordOnDeath":
						bool.TryParse(val, out ModGoBack.isAutoRecordOnDeath);
						break;
					case "savedMapId":
						int.TryParse(val, out ModGoBack.savedMapId);
						break;
					case "savedZoneId":
						int.TryParse(val, out ModGoBack.savedZoneId);
						break;
					case "savedX":
						int.TryParse(val, out ModGoBack.savedX);
						break;
					case "savedY":
						int.TryParse(val, out ModGoBack.savedY);
						break;
					case "autoSetKHActive":
						bool.TryParse(val, out ModSetActivator.isActive);
						break;
					case "autoSellJunkFullBag":
						bool.TryParse(val, out ModSetActivator.autoSellJunk);
						break;
					case "minStarToKeep":
						int.TryParse(val, out ModSetActivator.minStarToKeep);
						break;
					case "showItemId":
						bool.TryParse(val, out ModSetActivator.showItemId);
						break;
					case "savedFarmMapId":
						int.TryParse(val, out ModSetActivator.savedFarmMapId);
						break;
					case "savedFarmZoneId":
						int.TryParse(val, out ModSetActivator.savedFarmZoneId);
						break;
					case "savedFarmX":
						int.TryParse(val, out ModSetActivator.savedFarmX);
						break;
					case "savedFarmY":
						int.TryParse(val, out ModSetActivator.savedFarmY);
						break;
					case "isAutoKiteBroly":
						bool.TryParse(val, out ModKiteBroly.isAutoKite);
						break;
					case "isKhinhCongBroly":
						bool.TryParse(val, out ModKiteBroly.isKhinhCong);
						break;
					case "safeDistanceBroly":
						int.TryParse(val, out ModKiteBroly.safeDistance);
						break;
					case "autoAttackBroly":
						bool.TryParse(val, out ModKiteBroly.autoAttackBroly);
						break;
					case "isInstantPick":
						bool.TryParse(val, out ModDropRate.isInstantPick);
						break;
					case "isInstantRespawnAttack":
						bool.TryParse(val, out ModDropRate.isInstantRespawnAttack);
						break;
					case "isLastHitLock":
						bool.TryParse(val, out ModDropRate.isLastHitLock);
						break;
					case "isAutoRebuyBua":
						bool.TryParse(val, out ModAutoBuyBua.isAutoRebuy);
						break;
					case "selectedBuaType":
						int.TryParse(val, out ModAutoBuyBua.selectedBuaType);
						break;
					case "selectedBuaPackage":
						int.TryParse(val, out ModAutoBuyBua.selectedPackage);
						break;
					case "isTranslate":
						bool.TryParse(val, out isTranslate);
						break;
					case "isAnalog":
						if (bool.TryParse(val, out bool analogBool))
						{
							GameScr.isAnalog = analogBool ? 1 : 0;
						}
						break;
					case "isCustomBGActive":
						bool.TryParse(val, out ModBackground.isCustomBGActive);
						break;
					case "selectedBgId":
						ModBackground.selectedBgId = val;
						break;
				}
			}
		}
		catch
		{
		}
	}
}
