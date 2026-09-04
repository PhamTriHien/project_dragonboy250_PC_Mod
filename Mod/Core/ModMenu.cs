using System;
using System.Collections.Generic;
using UnityEngine;

public static class ModMenu
{
	#region Backward Compatibility Aliases

	// Tàn Sát
	public static bool autoTanSat
	{
		get { return ModTanSat.autoTanSat; }
		set { ModTanSat.autoTanSat = value; }
	}
	public static bool autoAttack
	{
		get { return ModTanSat.autoAttack; }
		set { ModTanSat.autoAttack = value; }
	}
	public static bool useTeleport
	{
		get { return ModTanSat.useTeleport; }
		set { ModTanSat.useTeleport = value; }
	}
	public static Mob currentFarmTarget
	{
		get { return ModTanSat.currentFarmTarget; }
		set { ModTanSat.currentFarmTarget = value; }
	}
	public static int selectedMobTemplateId
	{
		get { return ModTanSat.selectedMobTemplateId; }
		set { ModTanSat.selectedMobTemplateId = value; }
	}
	public static bool selectAllMobs
	{
		get { return ModTanSat.selectAllMobs; }
		set { ModTanSat.selectAllMobs = value; }
	}
	public static List<int> tickedMobTemplateIds
	{
		get { return ModTanSat.tickedMobTemplateIds; }
	}
	public static int selectedSkillTemplateId
	{
		get { return ModTanSat.selectedSkillTemplateId; }
		set { ModTanSat.selectedSkillTemplateId = value; }
	}
	public static bool selectAllSkills
	{
		get { return ModTanSat.selectAllSkills; }
		set { ModTanSat.selectAllSkills = value; }
	}
	public static List<int> tickedSkillTemplateIds
	{
		get { return ModTanSat.tickedSkillTemplateIds; }
	}

	// Tự Nhặt
	public static bool autoPick
	{
		get { return ModAutoPick.autoPick; }
		set { ModAutoPick.autoPick = value; }
	}
	public static bool pickAll
	{
		get { return ModAutoPick.pickAll; }
		set { ModAutoPick.pickAll = value; }
	}
	public static bool pickGold
	{
		get { return ModAutoPick.pickGold; }
		set { ModAutoPick.pickGold = value; }
	}
	public static bool pickEquip
	{
		get { return ModAutoPick.pickEquip; }
		set { ModAutoPick.pickEquip = value; }
	}
	public static bool pickGem
	{
		get { return ModAutoPick.pickGem; }
		set { ModAutoPick.pickGem = value; }
	}

	// Tốc Chạy
	public static bool speedHack
	{
		get { return ModSpeed.speedHack; }
		set { ModSpeed.speedHack = value; }
	}
	public static float speedMult
	{
		get { return ModSpeed.speedMult; }
		set { ModSpeed.speedMult = value; }
	}

	// Bơm Đậu & Khóa HP
	public static bool autoPean
	{
		get { return ModAutoHeal.autoPean; }
		set { ModAutoHeal.autoPean = value; }
	}
	public static int autoPeanHpPercent
	{
		get { return ModAutoHeal.autoPeanHpPercent; }
		set { ModAutoHeal.autoPeanHpPercent = value; }
	}
	public static bool lockHPMP
	{
		get { return ModAutoHeal.lockHPMP; }
		set { ModAutoHeal.lockHPMP = value; }
	}

	// Đồ Họa & FPS
	public static int graphicsQuality
	{
		get { return ModGraphics.graphicsQuality; }
		set { ModGraphics.graphicsQuality = value; }
	}
	public static string[] graphicsNames
	{
		get { return ModGraphics.graphicsNames; }
	}
	public static int targetFps
	{
		get { return ModFps.targetFps; }
		set { ModFps.targetFps = value; }
	}
	public static bool isAutoFps
	{
		get { return ModFps.isAutoFps; }
		set { ModFps.isAutoFps = value; }
	}

	public static int pingMs
	{
		get { return ModFps.pingMs; }
		set { ModFps.pingMs = value; }
	}

	// Thông Báo Boss
	public static bool isShowBossNotice
	{
		get { return ModBossNotice.isShowBossNotice; }
		set { ModBossNotice.isShowBossNotice = value; }
	}
	public static List<ModBossNotice.BossNoticeEntry> listBossNotices
	{
		get { return ModBossNotice.listBossNotices; }
	}
	public static bool IsBossName(string name)
	{
		return ModBossNotice.IsBossName(name);
	}
	public static void AddBossNotice(string bossName, string mapName, string timeStr)
	{
		ModBossNotice.AddBossNotice(bossName, mapName, timeStr);
	}
	public static void ProcessServerBossNotice(string raw)
	{
		ModBossNotice.ProcessServerBossNotice(raw);
	}

	// Next Map
	public static bool isNextMapActive
	{
		get { return ModNextMap.isNextMapActive; }
		set { ModNextMap.isNextMapActive = value; }
	}
	public static int nextMapTargetId
	{
		get { return ModNextMap.nextMapTargetId; }
		set { ModNextMap.nextMapTargetId = value; }
	}
	public static string GetMapName(int id)
	{
		return ModNextMap.GetMapName(id);
	}
	public static void StartNextMap(int targetId)
	{
		ModNextMap.StartNextMap(targetId);
	}
	public static void StopNextMap()
	{
		ModNextMap.StopNextMap();
	}

	// UI
	public static bool uiCustomOpen
	{
		get { return ModUI.uiCustomOpen; }
		set { ModUI.uiCustomOpen = value; }
	}
	public static bool uiTanSatOpen
	{
		get { return ModUI.uiTanSatOpen; }
		set { ModUI.uiTanSatOpen = value; }
	}
	public static int selectedTab
	{
		get { return ModUI.selectedTab; }
		set { ModUI.selectedTab = value; }
	}
	public static int tanSatTab
	{
		get { return ModUI.tanSatTab; }
		set { ModUI.tanSatTab = value; }
	}

	// Config
	public static void SaveConfig()
	{
		ModConfig.SaveConfig();
	}
	public static void LoadConfig()
	{
		ModConfig.LoadConfig();
	}

	#endregion

	#region Core State & Lifecycle

	private static bool inited = false;
	private static long mapChangeWatchdogTime = 0;
	public static bool modMenuOpen = false;
	private static Command[] cmds;
	private static readonly ModAction act = new ModAction();

	private class ModAction : IActionListener
	{
		public void perform(int idAction, object p)
		{
			ModMenu.OnAction(idAction);
		}
	}

	public static bool IsInGame()
	{
		try
		{
			if (GameCanvas.currentScreen == null || GameScr.instance == null)
			{
				return false;
			}
			if (GameCanvas.currentScreen != GameScr.instance)
			{
				return false;
			}
			if (Char.myCharz() == null)
			{
				return false;
			}
			return Session_ME.gI().isConnected();
		}
		catch
		{
			return false;
		}
	}

	public static void OpenMenu()
	{
		try
		{
			ModUI.uiCustomOpen = true;
			modMenuOpen = true;
			SoundMn.gI().buttonClick();
		}
		catch
		{
		}
	}

	public static void CloseMenu()
	{
		try
		{
			ModUI.uiCustomOpen = false;
			modMenuOpen = false;
			ModConfig.SaveConfig();
			SoundMn.gI().buttonClose();
		}
		catch
		{
		}
	}

	public static void OnAction(int id)
	{
		try
		{
			if (id >= 1 && id <= 7)
			{
				ModUI.selectedTab = id - 1;
				ModUI.uiCustomOpen = true;
				SoundMn.gI().buttonClick();
			}
		}
		catch
		{
		}
	}

	public static void Update()
	{
		try
		{
			if (!inited)
			{
				inited = true;
				ModConfig.LoadConfig();
				ModFps.LoadFPS();
			}

			// Tự động điều chỉnh FPS theo tần số quét màn hình
			if (ModFps.isAutoFps)
			{
				int best = ModFps.GetBestFpsForDevice();
				if (best != ModFps.targetFps)
				{
					ModFps.targetFps = best;
					ModFps.ApplyFPS();
				}
			}

			// Phím tắt bàn phím PC
			ModHotkey.UpdateHotkeys();

			// Tương tác giao diện Modal
			ModUI.HandleTap();

			if (!IsInGame())
			{
				GameScr.isAutoPlay = false;
				GameScr.canAutoPlay = false;
				ModUI.uiCustomOpen = false;
				modMenuOpen = false;
				return;
			}

			// Keep-Alive Heartbeat: Giữ kết nối socket liên tục
			long now = mSystem.currentTimeMillis();
			if (now - Session_ME.lastSendTime > 15000)
			{
				Session_ME.lastSendTime = now;
				Service.gI().clientOk();
			}

			Char me = Char.myCharz();
			if (me == null)
			{
				return;
			}

			// Universal Map-Change Watchdog: Tự động chống kẹt map / kẹt khóa phím khi chuyển map
			if (Char.ischangingMap || Char.isLockKey)
			{
				if (mapChangeWatchdogTime == 0)
				{
					mapChangeWatchdogTime = now;
				}
				else if (now - mapChangeWatchdogTime > 1800)
				{
					Char.ischangingMap = false;
					Char.isLockKey = false;
					me.isLockAttack = false;
					me.isLockMove = false;
					InfoDlg.hide();
					GameCanvas.endDlg();
					GameCanvas.clearKeyHold();
					GameCanvas.clearKeyPressed();
					mapChangeWatchdogTime = 0;
				}
			}
			else
			{
				mapChangeWatchdogTime = 0;
			}

			if (me.charID != ModSpeed.lastCharId)
			{
				ModSpeed.lastCharId = me.charID;
				ModSpeed.originalSpeed = -1;
			}

			// Xử lý tốc chạy
			if (ModSpeed.speedHack)
			{
				if (ModSpeed.originalSpeed == -1)
				{
					ModSpeed.originalSpeed = me.cspeed;
				}
				me.cspeed = (int)(ModSpeed.originalSpeed * ModSpeed.speedMult);
			}
			else if (ModSpeed.originalSpeed != -1)
			{
				me.cspeed = ModSpeed.originalSpeed;
				ModSpeed.originalSpeed = -1;
			}

			// Tự động bơm đậu & khóa HP/MP
			ModAutoHeal.DoRealAutoHeal();

			// Tự động chuyển map
			ModNextMap.UpdateNextMap();

			// Tự động nhặt đồ
			ModAutoPick.RunRealAutoPick();

			// Tự động Tàn Sát
			ModTanSat.RunTanSat();
		}
		catch
		{
		}
	}

	public static void Paint(mGraphics g)
	{
		if (!IsInGame())
		{
			return;
		}
		try
		{
			// 1. Nút mũi tên gốc của game bên góc phải (chỉ hiển thị trong game khi log server)
			ModArrowButton.Paint(g);

			// 2. HUD Thông báo Boss (chỉ khi đã vào game)
			ModBossNotice.PaintBossNotice(g);

			// 3. Giao diện Modal Cài Đặt (7 Tab) (chỉ trong game)
			ModUI.PaintTanSatUI(g);

			// 4. Hiển thị FPS & Ping (chỉ trong game)
			ModFps.PaintFPS(g);
		}
		catch
		{
		}
	}

	public static void PaintFPS(mGraphics g)
	{
		ModFps.PaintFPS(g);
	}

	public static void PaintBossNotice(mGraphics g)
	{
		ModBossNotice.PaintBossNotice(g);
	}

	public static void PaintTanSatUI(mGraphics g)
	{
		ModUI.PaintTanSatUI(g);
	}

	#endregion
}
