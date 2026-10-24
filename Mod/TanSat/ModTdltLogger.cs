using System;
using System.IO;
using System.Text;
using UnityEngine;

public static class ModTdltLogger
{
	private static readonly object _lock = new object();
	private static string _logFilePath = null;
	public static bool isEnabled = true;
	public static int totalEventsLogged = 0;
	public static long sessionStartTime = 0;

	public static string LogFilePath
	{
		get
		{
			if (_logFilePath == null)
			{
				try
				{
					_logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tdlt_activity.log");
				}
				catch
				{
					_logFilePath = "tdlt_activity.log";
				}
			}
			return _logFilePath;
		}
	}

	public static void InitSession()
	{
		lock (_lock)
		{
			try
			{
				sessionStartTime = mSystem.currentTimeMillis();
				string header = $"================================================================================\n" +
								$"[TDLT LOGGER SESSION START] Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}\n" +
								$"Operating System: {Environment.OSVersion}, Machine: {Environment.MachineName}\n" +
								$"================================================================================\n";
				File.AppendAllText(LogFilePath, header, Encoding.UTF8);
				Console.WriteLine("[TDLT] Logger session initialized: " + LogFilePath);
			}
			catch (Exception ex)
			{
				Console.WriteLine("[TDLT ERR] Failed to init log file: " + ex.Message);
			}
		}
	}

	public static void Log(string tag, string message)
	{
		if (!isEnabled) return;
		lock (_lock)
		{
			try
			{
				totalEventsLogged++;
				string timeStr = DateTime.Now.ToString("HH:mm:ss.fff");
				long elapsed = (sessionStartTime > 0) ? (mSystem.currentTimeMillis() - sessionStartTime) : 0;
				string line = $"[{timeStr} | +{elapsed}ms] [{tag}] {message}";
				Console.WriteLine("[TDLT] " + line);
				File.AppendAllText(LogFilePath, line + "\n", Encoding.UTF8);
			}
			catch
			{
			}
		}
	}

	public static void LogItemUse(sbyte type, sbyte where, sbyte index, short template)
	{
		string itemName = "Unknown";
		try
		{
			Char me = Char.myCharz();
			if (me != null && me.arrItemBag != null && index >= 0 && index < me.arrItemBag.Length && me.arrItemBag[index] != null && me.arrItemBag[index].template != null)
			{
				itemName = me.arrItemBag[index].template.name + $" (ID: {me.arrItemBag[index].template.id})";
			}
			else if (template >= 0)
			{
				ItemTemplate t = ItemTemplates.get(template);
				if (t != null) itemName = t.name + $" (ID: {t.id})";
			}
		}
		catch { }

		Log("USE_ITEM_SEND", $"Packet -43 sent -> type={type}, where={where}, index={index}, template={template}, Item='{itemName}'");
	}

	public static void LogCanAutoPlay(sbyte rawVal, bool canAutoPlay)
	{
		Log("AUTOPLAY_PACKET", $"Packet -116 received from Server -> rawByte={rawVal}, canAutoPlay={canAutoPlay}");
	}

	public static void LogItemResponse(sbyte action, sbyte where, sbyte index, string info)
	{
		Log("ITEM_RESPONSE", $"Packet -43 response from Server -> action={action}, where={where}, index={index}, info='{info}'");
	}

	public static void LogAutoPlayTick(int timeSkill, bool canAutoPlay, bool isAutoPlay, int mobCount)
	{
		Char me = Char.myCharz();
		int cx = me != null ? me.cx : 0;
		int cy = me != null ? me.cy : 0;
		int focusMobId = (me != null && me.mobFocus != null) ? me.mobFocus.mobId : -1;
		Log("AUTOPLAY_TICK", $"timeSkill={timeSkill}, canAutoPlay={canAutoPlay}, isAutoPlay={isAutoPlay}, myPos=({cx},{cy}), focusMobId={focusMobId}, mobCount={mobCount}");
	}

	public static void LogAutoPlayTargetSelected(Mob mob, int oldCx, int oldCy)
	{
		int mobTpl = mob != null ? mob.templateId : -1;
		string mobName = (Mob.arrMobTemplate != null && mobTpl >= 0 && mobTpl < Mob.arrMobTemplate.Length && Mob.arrMobTemplate[mobTpl] != null) ? Mob.arrMobTemplate[mobTpl].name : ("Mob#" + mobTpl);
		int mx = mob != null ? mob.x : 0;
		int my = mob != null ? mob.y : 0;
		long hp = mob != null ? mob.hp : 0;
		int dist = Res.distance(oldCx, oldCy, mx, my);

		Log("TARGET_SELECTED", $"Selected Mob: ID={mob?.mobId}, Template={mobTpl} ('{mobName}'), Pos=({mx},{my}), HP={hp}, DistFromPlayer={dist}px. Player Teleporting to ({mx},{my})");
	}

	public static void LogAutoPlayFire(Skill skill, Mob target)
	{
		Char me = Char.myCharz();
		int skId = skill != null ? skill.skillId : -1;
		int tplId = (skill != null && skill.template != null) ? skill.template.id : -1;
		string skName = (skill != null && skill.template != null) ? skill.template.name : "Unknown";
		int cd = skill != null ? skill.coolDown : 0;
		long lastUse = skill != null ? skill.lastTimeUseThisSkill : 0;
		long delta = mSystem.currentTimeMillis() - lastUse;

		Log("FIRE_ATTACK", $"Skill='{skName}' (tpl={tplId}, id={skId}, cd={cd}ms, deltaLastUse={delta}ms) -> Target Mob: ID={target?.mobId}, Pos=({target?.x},{target?.y}), MobHP={target?.hp}/{target?.maxHp}, PlayerPos=({me?.cx},{me?.cy}), PlayerHP={me?.cHP}/{me?.cHPFull}");
	}

	public static void LogCharMove(string method, int cx, int cy, int cxSend, int cySend)
	{
		Log("CHAR_MOVE", $"{method} -> cx={cx}, cy={cy}, cxSend={cxSend}, cySend={cySend}");
	}

	public static void LogServerMessage(string source, string message)
	{
		if (string.IsNullOrEmpty(message)) return;
		Char me = Char.myCharz();
		string cleanMsg = message.Replace("\r", " ").Replace("\n", " | ");
		Log("SERVER_MSG", $"[{source}] '{cleanMsg}' | Map='{TileMap.mapName}' (ID:{TileMap.mapID}, Zone:{TileMap.zoneID}), PlayerPos=({me?.cx},{me?.cy}), PlayerHP={me?.cHP}/{me?.cHPFull}");
	}

	public static void LogMapInfo(int mapId, string mapName, int zoneId, sbyte planetId, int cx, int cy, sbyte teleport)
	{
		Char me = Char.myCharz();
		Log("MAP_INFO", $"Packet -24 -> Map='{mapName}' (ID:{mapId}, Zone:{zoneId}, Planet:{planetId}), SpawnPos=({cx},{cy}), TeleportType={teleport}, PlayerHP={me?.cHP}/{me?.cHPFull}, statusMe={me?.statusMe}, meDead={me?.meDead}, isNhapThe={me?.isNhapThe}");
	}

	public static void LogCharDead(string source, int toX, int toY, sbyte cPk, long cPower)
	{
		Char me = Char.myCharz();
		Log("CHAR_DEAD", $"[{source}] Map='{TileMap.mapName}' (ID:{TileMap.mapID}, Zone:{TileMap.zoneID}), DiePos=({toX},{toY}), CurPos=({me?.cx},{me?.cy}), HP={me?.cHP}/{me?.cHPFull}, cPk={cPk}, Power={cPower}, isNhapThe={me?.isNhapThe}");
	}

	public static void LogMobAttackMe(Mob mob, long dameHp, long dameMp)
	{
		Char me = Char.myCharz();
		int mobTpl = mob != null ? mob.templateId : -1;
		string mobName = (Mob.arrMobTemplate != null && mobTpl >= 0 && mobTpl < Mob.arrMobTemplate.Length && Mob.arrMobTemplate[mobTpl] != null) ? Mob.arrMobTemplate[mobTpl].name : ("Mob#" + mobTpl);
		Log("MOB_ATTACK_ME", $"Packet -11 -> Mob: ID={mob?.mobId}, Template={mobTpl} ('{mobName}'), Pos=({mob?.x},{mob?.y}), DameHP={dameHp}, DameMP={dameMp}, PlayerHP_Before={me?.cHP}/{me?.cHPFull}, Map='{TileMap.mapName}' (ID:{TileMap.mapID})");
	}

	public static void LogPlayerHitMe(string source, int attackerId, long dameHp, long newHp, bool isDie, bool isCrit)
	{
		Char me = Char.myCharz();
		Char attacker = GameScr.findCharInMap(attackerId);
		string attackerName = attacker != null ? attacker.cName : ("Char#" + attackerId);
		Log("CHAR_HIT_ME", $"[{source}] Attacker='{attackerName}' (ID:{attackerId}), DameHP={dameHp}, NewHP={newHp}/{me?.cHPFull}, isDie={isDie}, isCrit={isCrit}, Map='{TileMap.mapName}' (ID:{TileMap.mapID}, Zone:{TileMap.zoneID}), PlayerPos=({me?.cx},{me?.cy})");
	}
}
