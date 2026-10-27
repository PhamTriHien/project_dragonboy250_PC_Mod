using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

/// <summary>
/// Bộ quản lý cấu hình Mod tập trung: Lưu trữ, đọc và ghi toàn bộ cài đặt tùy biến của người chơi vào file mod_config.ini.
/// </summary>
public static class ModConfig
{
	public static bool isTranslate = true;

	private static string ConfigPath
	{
		get
		{
			try
			{
#if NET8_0_OR_GREATER
				if (OperatingSystem.IsBrowser() || OperatingSystem.IsAndroid() || OperatingSystem.IsIOS() || Application.CurrentTabId > 1)
				{
					string dir = Application.persistentDataPath;
					if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
					{
						Directory.CreateDirectory(dir);
					}
					return Path.Combine(dir, "mod_config.ini");
				}
#endif
				string legacyDesktop = Path.Combine(Application.dataPath, "../mod_config.ini");
				if (File.Exists(legacyDesktop))
				{
					return legacyDesktop;
				}
				string pDir = Application.persistentDataPath;
				if (!string.IsNullOrEmpty(pDir) && !Directory.Exists(pDir))
				{
					Directory.CreateDirectory(pDir);
				}
				return Path.Combine(pDir, "mod_config.ini");
			}
			catch
			{
				return "mod_config.ini";
			}
		}
	}

	/// <summary>
	/// Lưu trữ toàn bộ giá trị cài đặt cấu hình hiện tại vào tệp mod_config.ini.
	/// </summary>
	public static void SaveConfig()
	{
		try
		{
			StringBuilder sb = new StringBuilder();

			// Tàn Sát: Luôn lưu false để khi thoát game/khởi động lại Tàn Sát luôn TẮT
			sb.AppendLine("autoTanSat=False");
			sb.AppendLine("useTeleport=" + ModTanSat.useTeleport);
			sb.AppendLine("tanSatAntiScanMode=" + ModTanSat.antiScanMode);
			sb.AppendLine("avoidSuperMob=" + ModTanSat.avoidSuperMob);
			sb.AppendLine("autoCheckStamina=" + ModTanSat.autoCheckStamina);
			sb.AppendLine("autoChangeZone=" + ModTanSat.autoChangeZone);
			sb.AppendLine("autoSupportSkills=" + ModTanSat.autoSupportSkills);
			sb.AppendLine("freezeSkillCooldown=" + ModTanSat.freezeSkillCooldown);
			sb.AppendLine("selectAllMobs=" + ModTanSat.selectAllMobs);
			sb.AppendLine("tickedMobTemplateIds=" + string.Join(",", ModTanSat.tickedMobTemplateIds.ConvertAll(i => i.ToString()).ToArray()));
			sb.AppendLine("tanSatUseAllSkills=" + ModTanSat.selectAllSkills);
			sb.AppendLine("skillSelectionMode=" + ModTanSatFilter.skillSelectionMode);
			sb.AppendLine("tickedSkillTemplateIds=" + string.Join(",", ModTanSat.tickedSkillTemplateIds.ConvertAll(i => i.ToString()).ToArray()));
			sb.AppendLine("timeAttack=" + ModTanSat.timeAttack);
			sb.AppendLine("lockedSkillTemplateId=" + ModTanSat.lockedSkillTemplateId);
			sb.AppendLine("checkLagMob=" + ModTanSat.checkLagMob);
			sb.AppendLine("autoResetZone=" + ModTanSat.autoResetZone);
			sb.AppendLine("resetZoneMinutes=" + ModTanSat.resetZoneMinutes);
			sb.AppendLine("showGoldTrain=" + ModTanSat.showGoldTrain);
			sb.AppendLine("trainPets=" + ModTanSat.trainPets);
			sb.AppendLine("autoChangeFocus=" + ModTanSat.autoChangeFocus);
			List<string> frozenIds = new List<string>();
			foreach (int fid in ModTanSat.frozenSkillIds)
			{
				frozenIds.Add(fid.ToString());
			}
			sb.AppendLine("frozenSkillIds=" + string.Join(",", frozenIds.ToArray()));
			List<string> delayPairs = new List<string>();
			foreach (KeyValuePair<int, int> kv in ModTanSat.customSkillDelays)
			{
				delayPairs.Add(kv.Key + ":" + kv.Value);
			}
			sb.AppendLine("customSkillDelays=" + string.Join(",", delayPairs.ToArray()));
			sb.AppendLine("aStarStepSize=" + ModAStarPathFinder.stepSize);
			sb.AppendLine("aStarStepDelayMs=" + ModAStarPathFinder.stepDelayMs);
			sb.AppendLine("autoSkillSlots=" + ModAutoSkill.ExportConfig());

			// Tự Nhặt & Vứt Đồ
			sb.AppendLine("autoPick=" + ModAutoPick.autoPick);
			sb.AppendLine("pickAll=" + ModAutoPick.pickAll);
			sb.AppendLine("pickGold=" + ModAutoPick.pickGold);
			sb.AppendLine("pickEquip=" + ModAutoPick.pickEquip);
			sb.AppendLine("pickGem=" + ModAutoPick.pickGem);
			sb.AppendLine("filterById=" + ModAutoPick.filterById);
			sb.AppendLine("filterIds=" + ModAutoPick.filterIdsRaw);
			sb.AppendLine("filterByName=" + ModAutoPick.filterByName);
			sb.AppendLine("filterName=" + ModAutoPick.filterNameRaw);
			sb.AppendLine("maxPickDistance=" + ModAutoPick.maxPickDistance);
			sb.AppendLine("autoDropItem=" + ModAutoPick.autoDropItem);
			sb.AppendLine("dropIds=" + ModAutoPick.dropIdsRaw);
			sb.AppendLine("onlyMyItems=" + ModAutoPick.onlyMyItems);
			sb.AppendLine("showPickRadius=" + ModAutoPick.showPickRadius);
			sb.AppendLine("blacklistIds=" + ModAutoPick.blacklistIdsRaw);

			// Tốc Chạy
			sb.AppendLine("speedHack=" + ModSpeed.speedHack);
			sb.AppendLine("speedMult=" + ModSpeed.speedMult.ToString(System.Globalization.CultureInfo.InvariantCulture));

			// Bơm Đậu & HP
			sb.AppendLine("autoPean=" + ModAutoHeal.autoPean);
			sb.AppendLine("autoPeanHpPercent=" + ModAutoHeal.autoPeanHpPercent);
			sb.AppendLine("autoPeanMpPercent=" + ModAutoHeal.autoPeanMpPercent);
			sb.AppendLine("lockHPMP=" + ModAutoHeal.lockHPMP);
			sb.AppendLine("autoHarvestPea=" + ModAutoHeal.autoHarvestPea);
			sb.AppendLine("autoDonateClan=" + ModAutoHeal.autoDonateClan);
			sb.AppendLine("autoRequestPea=" + ModAutoHeal.autoRequestPea);
			sb.AppendLine("autoFeedPetOnAsk=" + ModAutoHeal.autoFeedPetOnAsk);
			sb.AppendLine("donateTargetNames=" + ModAutoHeal.donateTargetNamesRaw);
			sb.AppendLine("petEatBeanHpPercent=" + ModAutoHeal.petEatBeanHpPercent);
			sb.AppendLine("petEatBeanMpPercent=" + ModAutoHeal.petEatBeanMpPercent);
			sb.AppendLine("autoPeanMinHp=" + ModAutoHeal.autoPeanMinHp);
			sb.AppendLine("autoPeanMinMp=" + ModAutoHeal.autoPeanMinMp);

			// Đồ Họa & FPS
			sb.AppendLine("graphicsQuality=" + ModGraphics.graphicsQuality);
			sb.AppendLine("renderMode=" + ModGraphics.renderMode);
			sb.AppendLine("renderBitDepth=" + ModGraphics.renderBitDepth);
			sb.AppendLine("resolutionIndex=" + ModGraphics.resolutionIndex);
			sb.AppendLine("isFullscreen=" + ModGraphics.isFullscreen);
#if NET8_0_OR_GREATER
			sb.AppendLine("fontTheme=" + ModGraphics.fontTheme);
#endif
			sb.AppendLine("fontSizePreset=" + ModGraphics.fontSizeIndex);
			sb.AppendLine("targetFps=" + ModFps.targetFps);
			sb.AppendLine("isAutoFps=" + ModFps.isAutoFps);
			sb.AppendLine("animSpeedIndex=" + ModGraphics.animSpeedIndex);
			sb.AppendLine("isAfkLowCpu=" + ModGraphics.isAfkLowCpu);
			sb.AppendLine("isHideOtherPlayers=" + ModGraphics.hideOtherPlayers);
			sb.AppendLine("enableQuickPcHotkeys=" + ModHotkey.enableQuickPcHotkeys);
			sb.AppendLine("showExtraQuickButtons=" + ModArrowButton.showExtraQuickButtons);
			sb.AppendLine("isAnalog=" + (GameScr.isAnalog == 1));

			// Thông Báo Boss & Săn Boss
			sb.AppendLine("isShowBossNotice=" + ModBossNotice.isShowBossNotice);
			sb.AppendLine("isAutoScanBossZone=" + ModBossNotice.isAutoScanBossZone);
			sb.AppendLine("isAutoAttackBoss=" + ModBossNotice.isAutoAttackBoss);
			sb.AppendLine("isAutoFocusBoss=" + ModBossNotice.isAutoFocusBoss);
			sb.AppendLine("isAutoFollowBoss=" + ModBossNotice.isAutoFollowBoss);
			sb.AppendLine("isKsSuperBroly1Hp=" + ModBossNotice.isKsSuperBroly1Hp);
			sb.AppendLine("filterBossNames=" + ModBossNotice.filterBossNamesRaw);

			// Logo TriHienKun
			sb.AppendLine("isShowLogoInGame=" + ModLogo.isShowLogoInGame);
			sb.AppendLine("logoPosition=" + ModLogo.logoPosition);

			// HUD Player & Boss trong map
			sb.AppendLine("isShowMapEntityHUD=" + ModMapEntityHUD.isShowMapEntityHUD);
			sb.AppendLine("isShowTargetStatus=" + ModMapEntityHUD.isShowTargetStatus);
			sb.AppendLine("isShowMasterHUD=" + ModMapEntityHUD.isShowMasterHUD);
			sb.AppendLine("isLockTarget=" + ModMapEntityHUD.isLockTarget);

#if NET8_0_OR_GREATER
			// Menu HUB Nổi Trên Màn Hình (Floating HUD)
			sb.AppendLine("isShowFloatingHUD=" + ModFloatingHUD.isShowFloatingHUD);
			sb.AppendLine("floatingBubbleX=" + ModFloatingHUD.bubbleX);
			sb.AppendLine("floatingBubbleY=" + ModFloatingHUD.bubbleY);
#endif

			// GoBack Map & Xmap Capsule
			sb.AppendLine("isGoBackActive=" + ModGoBack.isGoBackActive);
			sb.AppendLine("isAutoRecordOnDeath=" + ModGoBack.isAutoRecordOnDeath);
			sb.AppendLine("gobackOnLowMp=" + ModGoBack.gobackOnLowMp);
			sb.AppendLine("lowMpPercent=" + ModGoBack.lowMpPercent);
			sb.AppendLine("eatChickenAtHome=" + ModGoBack.eatChickenAtHome);
			sb.AppendLine("savedMapId=" + ModGoBack.savedMapId);
			sb.AppendLine("savedZoneId=" + ModGoBack.savedZoneId);
			sb.AppendLine("savedX=" + ModGoBack.savedX);
			sb.AppendLine("savedY=" + ModGoBack.savedY);
			sb.AppendLine("useCapsuleVip=" + ModNextMap.useCapsuleVip);
			sb.AppendLine("useNormalCapsule=" + ModNextMap.useNormalCapsule);
			sb.AppendLine("speedBoostTransport=" + ModNextMap.speedBoostTransport);
			sb.AppendLine("useSafeMovement=" + ModNextMap.useSafeMovement);
			sb.AppendLine("instantSpaceshipSkip=" + ModNextMap.instantSpaceshipSkip);
			sb.AppendLine("delayXmapMs=" + ModNextMap.delayXmapMs);
			sb.AppendLine("delayUseCapsuleMs=" + ModNextMap.delayUseCapsuleMs);

			// Úp Set Kích Hoạt & Lọc Đồ
			sb.AppendLine("autoSetKHActive=" + ModSetActivator.isActive);
			sb.AppendLine("autoSellJunkFullBag=" + ModSetActivator.autoSellJunk);
			sb.AppendLine("autoStoreToBox=" + ModSetActivator.autoStoreToBox);
			sb.AppendLine("minStarToKeep=" + ModSetActivator.minStarToKeep);
			sb.AppendLine("showItemId=" + ModSetActivator.showItemId);
			sb.AppendLine("showSkhHud=" + ModSetActivator.showSkhHud);
			sb.AppendLine("sellGodItems=" + ModSetActivator.sellGodItems);
			sb.AppendLine("sellSkhItems=" + ModSetActivator.sellSkhItems);
			sb.AppendLine("sellMaxLevel=" + ModSetActivator.sellMaxLevel);
			sb.AppendLine("minEmptyBagSlots=" + ModSetActivator.minEmptyBagSlots);
			sb.AppendLine("keepItemIds=" + ModSetActivator.keepItemIdsRaw);
			sb.AppendLine("sellItemIds=" + ModSetActivator.sellItemIdsRaw);
			sb.AppendLine("autoUseTdlt=" + ModSetActivator.autoUseTdlt);
			sb.AppendLine("autoBuyTdlt=" + ModSetActivator.autoBuyTdlt);
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
			sb.AppendLine("buaEnabledMask=" + ModAutoBuyBua.buaEnabledMask);
			sb.AppendLine("buaExpireTimestamps=" + ModAutoBuyBua.ExportExpireTimestamps());
			sb.AppendLine("selectedBuaPackage=" + ModAutoBuyBua.selectedPackage);
			sb.AppendLine("showCharmHud=" + ModAutoBuyBua.showCharmHud);

			// Việt Hoá Data Server
			sb.AppendLine("isTranslate=" + isTranslate);

			// Hình Nền Phong Cảnh Git
			sb.AppendLine("isCustomBGActive=" + ModBackground.isCustomBGActive);
			sb.AppendLine("selectedBgId=" + ModBackground.selectedBgId);

			// Auto Reconnect & Login khi mất mạng
			sb.AppendLine("autoReconnect=" + ModAutoLogin.isEnabled);
			sb.AppendLine("keepSavedZone=" + ModAutoLogin.keepSavedZone);
			sb.AppendLine("savedKeepZoneId=" + ModAutoLogin.savedKeepZoneId);
			sb.AppendLine("savedKeepZoneMapId=" + ModAutoLogin.savedKeepZoneMapId);
			sb.AppendLine("autoNewAccountEnabled=" + ModAutoNewAccount.isEnabled);
			sb.AppendLine("autoChat=" + ModAutoChat.isEnabled);
			sb.AppendLine("autoChatMessage=" + Convert.ToBase64String(Encoding.UTF8.GetBytes(ModAutoChat.message ?? string.Empty)));
			sb.AppendLine("autoChatIntervalMs=" + ModAutoChat.intervalMs);
			sb.AppendLine("autoBoxSort=" + ModAutoBoxSort.isEnabled);
			sb.AppendLine("autoBoxSortIntervalMs=" + ModAutoBoxSort.intervalMs);
			sb.AppendLine("autoPoint=" + ModAutoPoint.isEnabled);
			sb.AppendLine("autoPointType=" + ModAutoPoint.targetType);
			sb.AppendLine("autoPointAmount=" + ModAutoPoint.pointsPerTick);
			sb.AppendLine("autoPointIntervalMs=" + ModAutoPoint.intervalMs);
			sb.AppendLine("autoPointMaxStat=" + ModAutoPoint.maxTargetStat);
			sb.AppendLine("autoPointMultiTargets=" + ModAutoPoint.ExportMultiStatTargets());
			sb.AppendLine("autoItem=" + ModAutoItem.isEnabled);
			sb.AppendLine("autoItemIds=" + ModAutoItem.itemIdsRaw);
			sb.AppendLine("autoItemIntervalMs=" + ModAutoItem.intervalMs);
			sb.AppendLine("autoItemCondition=" + ModAutoItem.conditionType);
			sb.AppendLine("autoItemThreshold=" + ModAutoItem.thresholdPercent);
			sb.AppendLine("autoOpenCskb=" + ModAutoItem.autoOpenCskb);
			sb.AppendLine("schedule=" + ModSchedule.isEnabled);
			sb.AppendLine("scheduleDisconnectOff=" + ModSchedule.disconnectOnOffWindow);
			sb.AppendLine("scheduleStartMinute=" + ModSchedule.startMinute);
			sb.AppendLine("scheduleEndMinute=" + ModSchedule.endMinute);
			for (int s = 0; s < 6; s++)
			{
				sb.AppendLine("trainSlot" + s + "=" + (ModSchedule.trainSlotsRaw[s] ?? ""));
				sb.AppendLine("onOffSlot" + s + "=" + (ModSchedule.onOffSlotsRaw[s] ?? ""));
			}
			sb.AppendLine("autoRevive=" + ModAutoRevive.isEnabled);
			sb.AppendLine("autoPet=" + ModAutoPet.isEnabled);
			sb.AppendLine("autoPetStatus=" + ModAutoPet.status);
			sb.AppendLine("autoPetIntervalMs=" + ModAutoPet.intervalMs);
			sb.AppendLine("showPetHUD=" + ModAutoPet.showPetHUD);
			sb.AppendLine("petAvoidBoss=" + ModAutoPet.petAvoidBoss);
			sb.AppendLine("petAutoFlag=" + ModAutoPet.petAutoFlag);
			sb.AppendLine("autoNamekHeal=" + ModAutoPet.autoNamekHeal);
			sb.AppendLine("namekHealThresholdPercent=" + ModAutoPet.namekHealThresholdPercent);
			sb.AppendLine("petAutoAttackOnBark=" + ModAutoPet.petAutoAttackOnBark);
			sb.AppendLine("petAutoKok=" + ModAutoPet.petAutoKok);
			sb.AppendLine("disconnectOnPetPowerLimit=" + ModAutoPet.disconnectOnPetPowerLimit);
			sb.AppendLine("exitOnPetPowerLimit=" + ModAutoPet.exitOnPetPowerLimit);
			sb.AppendLine("autoFusion=" + ModAutoFusion.isEnabled);
			sb.AppendLine("spamPotaraMode=" + ModAutoFusion.spamPotaraMode);
			sb.AppendLine("autoFusionType=" + ModAutoFusion.fusionType);
			sb.AppendLine("autoFusionIntervalMs=" + ModAutoFusion.intervalMs);

			// Proxy, Custom Server, Set Đồ Nhanh & Auto Captcha
			sb.AppendLine("proxyRaw=" + (ModProxyServer.proxyRaw ?? string.Empty));
			sb.AppendLine("customSvName=" + (ModProxyServer.customSvName ?? string.Empty));
			sb.AppendLine("customSvIp=" + (ModProxyServer.customSvIp ?? string.Empty));
			sb.AppendLine("customSvPort=" + ModProxyServer.customSvPort);
			sb.AppendLine("lobbyBtnX=" + ModProxyServer.lobbyBtnX);
			sb.AppendLine("lobbyBtnY=" + ModProxyServer.lobbyBtnY);
			sb.AppendLine("equipSet1Raw=" + (ModInventoryExt.equipSet1Raw ?? string.Empty));
			sb.AppendLine("equipSet2Raw=" + (ModInventoryExt.equipSet2Raw ?? string.Empty));
			sb.AppendLine("autoUseItemsMulti=" + ModInventoryExt.ExportAutoUseItems());
			sb.AppendLine("savedAccountsList=" + ModAccountManager.ExportConfig());
			sb.AppendLine("autoCaptchaEnabled=" + ModAutoCaptcha.isEnabled);
			sb.AppendLine("captchaDisconnectOnFail=" + ModAutoCaptcha.disconnectOnFail);
			sb.AppendLine("captchaMaxFailCount=" + ModAutoCaptcha.maxFailCount);
			sb.AppendLine("captchaApiUrl=" + (ModAutoCaptcha.captchaApiUrl ?? string.Empty));

			string content = sb.ToString();
			File.WriteAllText(ConfigPath, content);
#if BROWSER
			if (OperatingSystem.IsBrowser()) DragonBoy_Web_Host.WebStorageSync.OnTextFileSaved(ConfigPath, content);
#endif
		}
		catch
		{
		}
	}

	/// <summary>
	/// Đọc và áp dụng toàn bộ giá trị cài đặt cấu hình từ tệp mod_config.ini.
	/// </summary>
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
					case "autoPoint":
						bool.TryParse(val, out ModAutoPoint.isEnabled);
						break;
					case "autoPointType":
						if (int.TryParse(val, out int apt)) ModAutoPoint.Configure(apt, ModAutoPoint.pointsPerTick, ModAutoPoint.intervalMs);
						break;
					case "autoPointAmount":
						if (int.TryParse(val, out int apa)) ModAutoPoint.Configure(ModAutoPoint.targetType, apa, ModAutoPoint.intervalMs);
						break;
					case "autoPointIntervalMs":
						if (int.TryParse(val, out int api)) ModAutoPoint.Configure(ModAutoPoint.targetType, ModAutoPoint.pointsPerTick, api);
						break;
					case "autoPointMaxStat":
						if (long.TryParse(val, out long apm) && apm >= 0) ModAutoPoint.maxTargetStat = apm;
						break;
					case "autoItem":
						bool.TryParse(val, out ModAutoItem.isEnabled);
						break;
					case "autoItemIds":
						ModAutoItem.LoadIds(val);
						break;
					case "autoItemIntervalMs":
						if (int.TryParse(val, out int aii)) ModAutoItem.intervalMs = aii < 1000 ? 1000 : (aii > 3600000 ? 3600000 : aii);
						break;
					case "autoItemCondition":
						int.TryParse(val, out ModAutoItem.conditionType);
						break;
					case "autoItemThreshold":
						if (int.TryParse(val, out int ait)) ModAutoItem.thresholdPercent = ait < 1 ? 1 : (ait > 99 ? 99 : ait);
						break;
					case "schedule":
						bool.TryParse(val, out ModSchedule.isEnabled);
						break;
					case "scheduleStartMinute":
						int.TryParse(val, out ModSchedule.startMinute);
						break;
					case "scheduleEndMinute":
						int.TryParse(val, out ModSchedule.endMinute);
						break;
					case "autoRevive":
						bool.TryParse(val, out ModAutoRevive.isEnabled);
						break;
					case "autoPet":
						bool.TryParse(val, out ModAutoPet.isEnabled);
						break;
					case "autoPetStatus":
						if (int.TryParse(val, out int aps)) ModAutoPet.status = (aps >= 0 && aps <= 3) ? aps : 2;
						break;
					case "autoPetIntervalMs":
						if (int.TryParse(val, out int apiPet)) ModAutoPet.intervalMs = apiPet < 1000 ? 1000 : (apiPet > 3600000 ? 3600000 : apiPet);
						break;
					case "showPetHUD":
						bool.TryParse(val, out ModAutoPet.showPetHUD);
						break;
					case "petAvoidBoss":
						bool.TryParse(val, out ModAutoPet.petAvoidBoss);
						break;
					case "petAutoFlag":
						bool.TryParse(val, out ModAutoPet.petAutoFlag);
						break;
					case "autoNamekHeal":
						bool.TryParse(val, out ModAutoPet.autoNamekHeal);
						break;
					case "autoFusion":
						bool.TryParse(val, out ModAutoFusion.isEnabled);
						break;
					case "autoFusionType":
						int.TryParse(val, out ModAutoFusion.fusionType);
						break;
					case "autoFusionIntervalMs":
						if (int.TryParse(val, out int afi)) ModAutoFusion.intervalMs = afi < 3000 ? 3000 : (afi > 3600000 ? 3600000 : afi);
						break;
					case "autoTanSat":
						ModTanSat.autoTanSat = false;
						break;
					case "useTeleport":
						bool.TryParse(val, out ModTanSat.useTeleport);
						break;
					case "tanSatAntiScanMode":
						bool.TryParse(val, out ModTanSat.antiScanMode);
						break;
					case "avoidSuperMob":
						bool.TryParse(val, out ModTanSat.avoidSuperMob);
						break;
					case "autoCheckStamina":
						bool.TryParse(val, out ModTanSat.autoCheckStamina);
						break;
					case "autoChangeZone":
						bool.TryParse(val, out ModTanSat.autoChangeZone);
						break;
					case "autoSupportSkills":
						bool.TryParse(val, out ModTanSat.autoSupportSkills);
						break;
					case "freezeSkillCooldown":
						bool.TryParse(val, out ModTanSat.freezeSkillCooldown);
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
					case "tanSatUseAllSkills":
						bool.TryParse(val, out ModTanSatFilter.selectAllSkills);
						break;
					case "skillSelectionMode":
						if (int.TryParse(val, out int ssm) && ssm >= 0 && ssm <= 2) ModTanSatFilter.skillSelectionMode = ssm;
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
					case "lockedSkillTemplateId":
						int.TryParse(val, out ModTanSat.lockedSkillTemplateId);
						break;
					case "checkLagMob":
						bool.TryParse(val, out ModTanSat.checkLagMob);
						break;
					case "autoResetZone":
						bool.TryParse(val, out ModTanSat.autoResetZone);
						break;
					case "resetZoneMinutes":
						if (int.TryParse(val, out int rzm) && rzm >= 1) ModTanSat.resetZoneMinutes = rzm;
						break;
					case "showGoldTrain":
						bool.TryParse(val, out ModTanSat.showGoldTrain);
						break;
					case "trainPets":
						bool.TryParse(val, out ModTanSat.trainPets);
						break;
					case "autoChangeFocus":
						bool.TryParse(val, out ModTanSat.autoChangeFocus);
						break;
					case "frozenSkillIds":
						ModTanSat.frozenSkillIds.Clear();
						if (!string.IsNullOrEmpty(val))
						{
							string[] fParts = val.Split(',');
							for (int fi = 0; fi < fParts.Length; fi++)
							{
								if (int.TryParse(fParts[fi].Trim(), out int fid) && !ModTanSat.frozenSkillIds.Contains(fid))
								{
									ModTanSat.frozenSkillIds.Add(fid);
								}
							}
						}
						break;
					case "customSkillDelays":
						ModTanSat.customSkillDelays.Clear();
						if (!string.IsNullOrEmpty(val))
						{
							string[] dParts = val.Split(',');
							for (int di = 0; di < dParts.Length; di++)
							{
								string[] kv = dParts[di].Split(':');
								if (kv.Length == 2 && int.TryParse(kv[0].Trim(), out int sid) && int.TryParse(kv[1].Trim(), out int sdelay) && sdelay >= 0)
								{
									ModTanSat.customSkillDelays[sid] = sdelay;
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
					case "maxPickDistance":
						if (int.TryParse(val, out int mpd) && mpd >= 0) ModAutoPick.maxPickDistance = mpd;
						break;
					case "autoDropItem":
						bool.TryParse(val, out ModAutoPick.autoDropItem);
						break;
					case "dropIds":
						ModAutoPick.LoadDropIds(val);
						break;
					case "onlyMyItems":
						bool.TryParse(val, out ModAutoPick.onlyMyItems);
						break;
					case "showPickRadius":
						bool.TryParse(val, out ModAutoPick.showPickRadius);
						break;
					case "blacklistIds":
						ModAutoPick.LoadBlacklistIds(val);
						break;
					case "speedHack":
						bool.TryParse(val, out ModSpeed.speedHack);
						break;
					case "speedMult":
						if (float.TryParse(val.Replace(',', '.'), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float parsedSpeedMult)) { ModSpeed.speedMult = parsedSpeedMult < 1.0f ? 1.0f : (parsedSpeedMult > 5.0f ? 5.0f : parsedSpeedMult); }
						break;
					case "autoPean":
						bool.TryParse(val, out ModAutoHeal.autoPean);
						break;
					case "autoPeanHpPercent":
						int.TryParse(val, out ModAutoHeal.autoPeanHpPercent);
						break;
					case "autoPeanMpPercent":
						int.TryParse(val, out ModAutoHeal.autoPeanMpPercent);
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
					case "autoRequestPea":
						bool.TryParse(val, out ModAutoHeal.autoRequestPea);
						break;
					case "autoFeedPetOnAsk":
						bool.TryParse(val, out ModAutoHeal.autoFeedPetOnAsk);
						break;
					case "donateTargetNames":
						ModAutoHeal.donateTargetNamesRaw = val ?? "";
						break;
					case "petEatBeanHpPercent":
						if (int.TryParse(val, out int pebhp) && pebhp >= 0 && pebhp <= 100)
						{
							ModAutoHeal.petEatBeanHpPercent = pebhp;
						}
						break;
					case "petEatBeanMpPercent":
						if (int.TryParse(val, out int pebmp) && pebmp >= 0 && pebmp <= 100)
						{
							ModAutoHeal.petEatBeanMpPercent = pebmp;
						}
						break;
					case "graphicsQuality":
						if (int.TryParse(val, out int gq) && gq >= 0 && gq < ModGraphics.graphicsNames.Length)
						{
							ModGraphics.graphicsQuality = gq;
						}
						break;
					case "renderMode":
					case "renderBitDepth":
						if (int.TryParse(val, out int bd))
						{
							ModGraphics.SetRenderMode(bd, false);
						}
						break;
					case "resolutionIndex":
						int.TryParse(val, out ModGraphics.resolutionIndex);
						break;
					case "isFullscreen":
						bool.TryParse(val, out ModGraphics.isFullscreen);
						break;
#if NET8_0_OR_GREATER
					case "fontTheme":
						if (int.TryParse(val, out int ft) && ft >= 0 && ft < ModGraphics.fontThemeNames.Length)
						{
							ModGraphics.fontTheme = ft;
							mFont.InvalidateFontMetrics();
						}
						break;
#endif
					case "fontSizePreset":
						if (int.TryParse(val, out int fsi) && fsi >= 0 && fsi < ModGraphics.fontSizeNames.Length)
						{
							ModGraphics.fontSizeIndex = fsi;
#if NET8_0_OR_GREATER
							mFont.InvalidateFontMetrics();
#endif
						}
						break;
					case "targetFps":
						int.TryParse(val, out ModFps.targetFps);
						break;
					case "isAutoFps":
						bool.TryParse(val, out ModFps.isAutoFps);
						break;
					case "animSpeedIndex":
						if (int.TryParse(val, out int asi))
						{
							ModGraphics.SetAnimSpeed(asi, false);
						}
						break;
					case "isAfkLowCpu":
						bool.TryParse(val, out ModGraphics.isAfkLowCpu);
						break;
					case "isShowBossNotice":
						bool.TryParse(val, out ModBossNotice.isShowBossNotice);
						break;
					case "isAutoScanBossZone":
						bool.TryParse(val, out ModBossNotice.isAutoScanBossZone);
						break;
					case "isAutoAttackBoss":
						bool.TryParse(val, out ModBossNotice.isAutoAttackBoss);
						break;
					case "isAutoFocusBoss":
						bool.TryParse(val, out ModBossNotice.isAutoFocusBoss);
						break;
					case "isKsSuperBroly1Hp":
						bool.TryParse(val, out ModBossNotice.isKsSuperBroly1Hp);
						break;
					case "filterBossNames":
						ModBossNotice.filterBossNamesRaw = val ?? "";
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
					case "isShowTargetStatus":
						bool.TryParse(val, out ModMapEntityHUD.isShowTargetStatus);
						break;
					case "isLockTarget":
						bool.TryParse(val, out ModMapEntityHUD.isLockTarget);
						break;
#if NET8_0_OR_GREATER
					case "isShowFloatingHUD":
						bool.TryParse(val, out ModFloatingHUD.isShowFloatingHUD);
						break;
					case "floatingBubbleX":
						int.TryParse(val, out ModFloatingHUD.bubbleX);
						break;
					case "floatingBubbleY":
						int.TryParse(val, out ModFloatingHUD.bubbleY);
						break;
#endif
					case "isGoBackActive":
						bool.TryParse(val, out ModGoBack.isGoBackActive);
						break;
					case "isAutoRecordOnDeath":
						bool.TryParse(val, out ModGoBack.isAutoRecordOnDeath);
						break;
					case "gobackOnLowMp":
						bool.TryParse(val, out ModGoBack.gobackOnLowMp);
						break;
					case "lowMpPercent":
						if (int.TryParse(val, out int lmp) && lmp >= 1 && lmp <= 99) ModGoBack.lowMpPercent = lmp;
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
					case "autoStoreToBox":
						bool.TryParse(val, out ModSetActivator.autoStoreToBox);
						break;
					case "minStarToKeep":
						int.TryParse(val, out ModSetActivator.minStarToKeep);
						break;
					case "showItemId":
						bool.TryParse(val, out ModSetActivator.showItemId);
						break;
					case "showSkhHud":
						bool.TryParse(val, out ModSetActivator.showSkhHud);
						break;
					case "sellGodItems":
						bool.TryParse(val, out ModSetActivator.sellGodItems);
						break;
					case "sellSkhItems":
						bool.TryParse(val, out ModSetActivator.sellSkhItems);
						break;
					case "sellMaxLevel":
						if (int.TryParse(val, out int sml) && sml >= 1 && sml <= 99) ModSetActivator.sellMaxLevel = sml;
						break;
					case "minEmptyBagSlots":
						if (int.TryParse(val, out int mebs) && mebs >= 1 && mebs <= 50) ModSetActivator.minEmptyBagSlots = mebs;
						break;
					case "keepItemIds":
						ModSetActivator.keepItemIdsRaw = val ?? "";
						break;
					case "sellItemIds":
						ModSetActivator.sellItemIdsRaw = val ?? "";
						break;
					case "autoUseTdlt":
						bool.TryParse(val, out ModSetActivator.autoUseTdlt);
						break;
					case "autoBuyTdlt":
						bool.TryParse(val, out ModSetActivator.autoBuyTdlt);
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
						if (int.TryParse(val, out int sbt) && sbt >= 0 && sbt < ModAutoBuyBua.buaTypeNames.Length) ModAutoBuyBua.selectedBuaType = sbt;
						break;
					case "selectedBuaPackage":
						int.TryParse(val, out ModAutoBuyBua.selectedPackage);
						break;
					case "showCharmHud":
						bool.TryParse(val, out ModAutoBuyBua.showCharmHud);
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
					case "autoReconnect":
						bool.TryParse(val, out ModAutoLogin.isEnabled);
						break;
					case "keepSavedZone":
						bool.TryParse(val, out ModAutoLogin.keepSavedZone);
						break;
					case "savedKeepZoneId":
						int.TryParse(val, out ModAutoLogin.savedKeepZoneId);
						break;
					case "autoChat":
						bool.TryParse(val, out ModAutoChat.isEnabled);
						break;
					case "autoChatMessage":
						try { ModAutoChat.message = Encoding.UTF8.GetString(Convert.FromBase64String(val)); } catch { ModAutoChat.message = string.Empty; }
						break;
					case "autoChatIntervalMs":
						if (int.TryParse(val, out int autoChatInterval)) ModAutoChat.intervalMs = autoChatInterval < 5000 ? 5000 : (autoChatInterval > 3600000 ? 3600000 : autoChatInterval);
						break;
					case "autoBoxSort":
						bool.TryParse(val, out ModAutoBoxSort.isEnabled);
						break;
					case "autoBoxSortIntervalMs":
						if (int.TryParse(val, out int boxSortInterval)) ModAutoBoxSort.intervalMs = boxSortInterval < 15000 ? 15000 : (boxSortInterval > 3600000 ? 3600000 : boxSortInterval);
						break;
					case "autoOpenCskb":
						bool.TryParse(val, out ModAutoItem.autoOpenCskb);
						break;
					case "exitOnPetPowerLimit":
						if (long.TryParse(val, out long eppl) && eppl >= 0) ModAutoPet.exitOnPetPowerLimit = eppl;
						break;
					case "autoPeanMinHp":
						if (long.TryParse(val, out long apmHp) && apmHp >= 0) ModAutoHeal.autoPeanMinHp = apmHp;
						break;
					case "autoPeanMinMp":
						if (long.TryParse(val, out long apmMp) && apmMp >= 0) ModAutoHeal.autoPeanMinMp = apmMp;
						break;
					case "isHideOtherPlayers":
						bool.TryParse(val, out ModGraphics.hideOtherPlayers);
						break;
					case "enableQuickPcHotkeys":
						bool.TryParse(val, out ModHotkey.enableQuickPcHotkeys);
						break;
					case "showExtraQuickButtons":
						bool.TryParse(val, out ModArrowButton.showExtraQuickButtons);
						break;
					case "eatChickenAtHome":
						bool.TryParse(val, out ModGoBack.eatChickenAtHome);
						break;
					case "useCapsuleVip":
						bool.TryParse(val, out ModNextMap.useCapsuleVip);
						break;
					case "useNormalCapsule":
						bool.TryParse(val, out ModNextMap.useNormalCapsule);
						break;
					case "speedBoostTransport":
						bool.TryParse(val, out ModNextMap.speedBoostTransport);
						break;
					case "buaEnabledMask":
						if (int.TryParse(val, out int bem) && bem >= 0) ModAutoBuyBua.buaEnabledMask = bem;
						break;
					case "buaExpireTimestamps":
						ModAutoBuyBua.ImportExpireTimestamps(val);
						break;
					case "autoPointMultiTargets":
						ModAutoPoint.ImportMultiStatTargets(val);
						break;
					case "scheduleDisconnectOff":
						bool.TryParse(val, out ModSchedule.disconnectOnOffWindow);
						break;
					case "petAutoAttackOnBark":
						bool.TryParse(val, out ModAutoPet.petAutoAttackOnBark);
						break;
					case "petAutoKok":
						bool.TryParse(val, out ModAutoPet.petAutoKok);
						break;
					case "disconnectOnPetPowerLimit":
						bool.TryParse(val, out ModAutoPet.disconnectOnPetPowerLimit);
						break;
					case "spamPotaraMode":
						bool.TryParse(val, out ModAutoFusion.spamPotaraMode);
						break;
					case "proxyRaw":
						ModProxyServer.proxyRaw = val ?? string.Empty;
						break;
					case "customSvName":
						ModProxyServer.customSvName = val ?? string.Empty;
						break;
					case "customSvIp":
						ModProxyServer.customSvIp = val ?? string.Empty;
						break;
					case "customSvPort":
						if (int.TryParse(val, out int csp) && csp >= 0 && csp <= 65535) ModProxyServer.customSvPort = csp;
						break;
					case "equipSet1Raw":
						ModInventoryExt.equipSet1Raw = val ?? string.Empty;
						break;
					case "equipSet2Raw":
						ModInventoryExt.equipSet2Raw = val ?? string.Empty;
						break;
					case "autoCaptchaEnabled":
						bool.TryParse(val, out ModAutoCaptcha.isEnabled);
						break;
					case "captchaDisconnectOnFail":
						bool.TryParse(val, out ModAutoCaptcha.disconnectOnFail);
						break;
					case "captchaMaxFailCount":
						if (int.TryParse(val, out int cmfc) && cmfc >= 1 && cmfc <= 20) ModAutoCaptcha.maxFailCount = cmfc;
						break;
					case "captchaApiUrl":
						ModAutoCaptcha.captchaApiUrl = val ?? string.Empty;
						break;
					case "trainSlot0":
					case "trainSlot1":
					case "trainSlot2":
					case "trainSlot3":
					case "trainSlot4":
					case "trainSlot5":
						int slotIdx = key[key.Length - 1] - '0';
						if (slotIdx >= 0 && slotIdx < 6) ModSchedule.trainSlotsRaw[slotIdx] = val ?? "";
						break;
					case "onOffSlot0":
					case "onOffSlot1":
					case "onOffSlot2":
					case "onOffSlot3":
					case "onOffSlot4":
					case "onOffSlot5":
						int offIdx = key[key.Length - 1] - '0';
						if (offIdx >= 0 && offIdx < 6) ModSchedule.onOffSlotsRaw[offIdx] = val ?? "";
						break;
					case "aStarStepSize":
						if (int.TryParse(val, out int ass) && ass >= 1 && ass <= 3) ModAStarPathFinder.stepSize = ass;
						break;
					case "aStarStepDelayMs":
						if (int.TryParse(val, out int asd) && asd >= 25 && asd <= 100) ModAStarPathFinder.stepDelayMs = asd;
						break;
					case "autoSkillSlots":
						ModAutoSkill.ImportConfig(val);
						break;
					case "isShowMasterHUD":
						bool.TryParse(val, out ModMapEntityHUD.isShowMasterHUD);
						break;
					case "isAutoFollowBoss":
						bool.TryParse(val, out ModBossNotice.isAutoFollowBoss);
						break;
					case "useSafeMovement":
						bool.TryParse(val, out ModNextMap.useSafeMovement);
						break;
					case "instantSpaceshipSkip":
						bool.TryParse(val, out ModNextMap.instantSpaceshipSkip);
						break;
					case "delayXmapMs":
						if (int.TryParse(val, out int dxm) && dxm >= 100 && dxm <= 3000) ModNextMap.delayXmapMs = dxm;
						break;
					case "delayUseCapsuleMs":
						if (int.TryParse(val, out int duc) && duc >= 200 && duc <= 5000) ModNextMap.delayUseCapsuleMs = duc;
						break;
					case "savedKeepZoneMapId":
						int.TryParse(val, out ModAutoLogin.savedKeepZoneMapId);
						break;
					case "autoNewAccountEnabled":
						bool.TryParse(val, out ModAutoNewAccount.isEnabled);
						break;
					case "namekHealThresholdPercent":
						if (int.TryParse(val, out int nht) && nht >= 5 && nht <= 95) ModAutoPet.namekHealThresholdPercent = nht;
						break;
					case "lobbyBtnX":
						if (int.TryParse(val, out int lbx)) ModProxyServer.lobbyBtnX = lbx;
						break;
					case "lobbyBtnY":
						if (int.TryParse(val, out int lby)) ModProxyServer.lobbyBtnY = lby;
						break;
					case "autoUseItemsMulti":
						ModInventoryExt.ImportAutoUseItems(val);
						break;
					case "savedAccountsList":
						ModAccountManager.ImportConfig(val);
						break;
				}
			}
			if (ModProxyServer.HasCustomServer() && ServerListScreen.nameServer != null && ServerListScreen.nameServer.Length > 0)
			{
				ModProxyServer.InjectCustomServer();
			}
		}
		catch
		{
		}
	}
}
