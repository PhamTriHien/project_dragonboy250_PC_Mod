using System;
using System.IO;
using System.Text;
using UnityEngine;

/// <summary>
/// CLASS ModTdltLogger: Ghi nhật ký chẩn đoán hoạt động Tàn Sát / Auto (tự tắt mặc định trên trình duyệt WebAssembly để bảo vệ RAM MEMFS và tự cắt vòng dung lượng trên PC/Mobile).
/// </summary>
public static class ModTdltLogger
{
	private static readonly object _lock = new object();
	private static string _logFilePath = null;
#if NET8_0_OR_GREATER
	public static bool isEnabled = !OperatingSystem.IsBrowser();
#else
	public static bool isEnabled = true;
#endif
	public static int totalEventsLogged = 0;
	public static long sessionStartTime = 0;
	private static long _approxBytesSinceCheck = 0;

	private static long GetMaxLogBytes()
	{
#if NET8_0_OR_GREATER
		if (OperatingSystem.IsBrowser())
		{
			return 256 * 1024L;
		}
#endif
		return 8 * 1024 * 1024L;
	}

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

	private static void TrimLogFileIfNeeded()
	{
		try
		{
			string path = LogFilePath;
			if (!File.Exists(path))
			{
				return;
			}
			long maxBytes = GetMaxLogBytes();
			FileInfo fi = new FileInfo(path);
			if (fi.Length <= maxBytes)
			{
				return;
			}
			int keepBytes = (int)(maxBytes / 2);
			byte[] tail = new byte[keepBytes];
			using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				fs.Seek(-keepBytes, SeekOrigin.End);
				int read = 0;
				while (read < keepBytes)
				{
					int n = fs.Read(tail, read, keepBytes - read);
					if (n <= 0) break;
					read += n;
				}
			}
			int firstNewline = Array.IndexOf(tail, (byte)'\n');
			int startOffset = (firstNewline >= 0 && firstNewline + 1 < tail.Length) ? (firstNewline + 1) : 0;
			using (FileStream outFs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read))
			{
				outFs.Write(tail, startOffset, tail.Length - startOffset);
			}
		}
		catch
		{
		}
	}

	/// <summary>
	/// Khởi tạo và nạp dữ liệu cấu hình cho InitSession.
	/// </summary>
	public static void InitSession()
	{
		sessionStartTime = mSystem.currentTimeMillis();
		if (!isEnabled) return;
		lock (_lock)
		{
			try
			{
				TrimLogFileIfNeeded();
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

	/// <summary>
	/// Thực thi phương thức nghiệp vụ Log.
	/// </summary>
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
#if NET8_0_OR_GREATER
				if (!OperatingSystem.IsBrowser())
				{
					Console.WriteLine("[TDLT] " + line);
				}
#else
				Console.WriteLine("[TDLT] " + line);
#endif
				File.AppendAllText(LogFilePath, line + "\n", Encoding.UTF8);
				_approxBytesSinceCheck += line.Length + 2;
				if (_approxBytesSinceCheck >= 65536)
				{
					_approxBytesSinceCheck = 0;
					TrimLogFileIfNeeded();
				}
			}
			catch
			{
			}
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ LogItemUse.
	/// </summary>
	public static void LogItemUse(sbyte type, sbyte where, sbyte index, short template)
	{
		if (!isEnabled) return;
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

	/// <summary>
	/// Thực thi phương thức nghiệp vụ LogCanAutoPlay.
	/// </summary>
	public static void LogCanAutoPlay(sbyte rawVal, bool canAutoPlay)
	{
		if (!isEnabled) return;
		Log("AUTOPLAY_PACKET", $"Packet -116 received from Server -> rawByte={rawVal}, canAutoPlay={canAutoPlay}");
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ LogItemResponse.
	/// </summary>
	public static void LogItemResponse(sbyte action, sbyte where, sbyte index, string info)
	{
		if (!isEnabled) return;
		Log("ITEM_RESPONSE", $"Packet -43 response from Server -> action={action}, where={where}, index={index}, info='{info}'");
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ LogAutoPlayTick.
	/// </summary>
	public static void LogAutoPlayTick(int timeSkill, bool canAutoPlay, bool isAutoPlay, int mobCount)
	{
		if (!isEnabled) return;
		Char me = Char.myCharz();
		int cx = me != null ? me.cx : 0;
		int cy = me != null ? me.cy : 0;
		int focusMobId = (me != null && me.mobFocus != null) ? me.mobFocus.mobId : -1;
		Log("AUTOPLAY_TICK", $"timeSkill={timeSkill}, canAutoPlay={canAutoPlay}, isAutoPlay={isAutoPlay}, myPos=({cx},{cy}), focusMobId={focusMobId}, mobCount={mobCount}");
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ LogAutoPlayTargetSelected.
	/// </summary>
	public static void LogAutoPlayTargetSelected(Mob mob, int oldCx, int oldCy)
	{
		if (!isEnabled) return;
		int mobTpl = mob != null ? mob.templateId : -1;
		string mobName = (Mob.arrMobTemplate != null && mobTpl >= 0 && mobTpl < Mob.arrMobTemplate.Length && Mob.arrMobTemplate[mobTpl] != null) ? Mob.arrMobTemplate[mobTpl].name : ("Mob#" + mobTpl);
		int mx = mob != null ? mob.x : 0;
		int my = mob != null ? mob.y : 0;
		long hp = mob != null ? mob.hp : 0;
		int dist = Res.distance(oldCx, oldCy, mx, my);

		Log("TARGET_SELECTED", $"Selected Mob: ID={mob?.mobId}, Template={mobTpl} ('{mobName}'), Pos=({mx},{my}), HP={hp}, DistFromPlayer={dist}px. Player Teleporting to ({mx},{my})");
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ LogAutoPlayFire.
	/// </summary>
	public static void LogAutoPlayFire(Skill skill, Mob target)
	{
		if (!isEnabled) return;
		Char me = Char.myCharz();
		int skId = skill != null ? skill.skillId : -1;
		int tplId = (skill != null && skill.template != null) ? skill.template.id : -1;
		string skName = (skill != null && skill.template != null) ? skill.template.name : "Unknown";
		int cd = skill != null ? skill.coolDown : 0;
		long lastUse = skill != null ? skill.lastTimeUseThisSkill : 0;
		long delta = mSystem.currentTimeMillis() - lastUse;

		Log("FIRE_ATTACK", $"Skill='{skName}' (tpl={tplId}, id={skId}, cd={cd}ms, deltaLastUse={delta}ms) -> Target Mob: ID={target?.mobId}, Pos=({target?.x},{target?.y}), MobHP={target?.hp}/{target?.maxHp}, PlayerPos=({me?.cx},{me?.cy}), PlayerHP={me?.cHP}/{me?.cHPFull}");
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ LogCharMove.
	/// </summary>
	public static void LogCharMove(string method, int cx, int cy, int cxSend, int cySend)
	{
		if (!isEnabled) return;
		Log("CHAR_MOVE", $"{method} -> cx={cx}, cy={cy}, cxSend={cxSend}, cySend={cySend}");
	}

	/// <summary>
	/// Ghi nhận thông báo từ Server hoặc Toast hiển thị trên màn hình.
	/// </summary>
	public static void LogServerMessage(string source, string message)
	{
		if (!isEnabled || string.IsNullOrEmpty(message)) return;
		Char me = Char.myCharz();
		string cleanMsg = message.Replace("\r", " ").Replace("\n", " | ");
		Log("SERVER_MSG", $"[{source}] '{cleanMsg}' | Map='{TileMap.mapName}' (ID:{TileMap.mapID}, Zone:{TileMap.zoneID}), PlayerPos=({me?.cx},{me?.cy}), PlayerHP={me?.cHP}/{me?.cHPFull}");
	}

	/// <summary>
	/// Ghi nhận gói tin chuyển map MAP_INFO (-24) từ Server.
	/// </summary>
	public static void LogMapInfo(int mapId, string mapName, int zoneId, sbyte planetId, int cx, int cy, sbyte teleport)
	{
		if (!isEnabled) return;
		Char me = Char.myCharz();
		Log("MAP_INFO", $"Packet -24 -> Map='{mapName}' (ID:{mapId}, Zone:{zoneId}, Planet:{planetId}), SpawnPos=({cx},{cy}), TeleportType={teleport}, PlayerHP={me?.cHP}/{me?.cHPFull}, statusMe={me?.statusMe}, meDead={me?.meDead}, isNhapThe={me?.isNhapThe}");
	}

	/// <summary>
	/// Ghi nhận trạng thái nhân vật bị hạ gục (CHAR_DEAD).
	/// </summary>
	public static void LogCharDead(string source, int toX, int toY, sbyte cPk, long cPower)
	{
		if (!isEnabled) return;
		Char me = Char.myCharz();
		Log("CHAR_DEAD", $"[{source}] Map='{TileMap.mapName}' (ID:{TileMap.mapID}, Zone:{TileMap.zoneID}), DiePos=({toX},{toY}), CurPos=({me?.cx},{me?.cy}), HP={me?.cHP}/{me?.cHPFull}, cPk={cPk}, Power={cPower}, isNhapThe={me?.isNhapThe}");
	}

	/// <summary>
	/// Ghi nhận gói tin quái/Boss tấn công nhân vật (-11).
	/// </summary>
	public static void LogMobAttackMe(Mob mob, long dameHp, long dameMp)
	{
		if (!isEnabled) return;
		Char me = Char.myCharz();
		int mobTpl = mob != null ? mob.templateId : -1;
		string mobName = (Mob.arrMobTemplate != null && mobTpl >= 0 && mobTpl < Mob.arrMobTemplate.Length && Mob.arrMobTemplate[mobTpl] != null) ? Mob.arrMobTemplate[mobTpl].name : ("Mob#" + mobTpl);
		Log("MOB_ATTACK_ME", $"Packet -11 -> Mob: ID={mob?.mobId}, Template={mobTpl} ('{mobName}'), Pos=({mob?.x},{mob?.y}), DameHP={dameHp}, DameMP={dameMp}, PlayerHP_Before={me?.cHP}/{me?.cHPFull}, Map='{TileMap.mapName}' (ID:{TileMap.mapID})");
	}

	/// <summary>
	/// Ghi nhận gói tin nhân vật/Boss Char gây sát thương lên người chơi.
	/// </summary>
	public static void LogPlayerHitMe(string source, int attackerId, long dameHp, long newHp, bool isDie, bool isCrit)
	{
		if (!isEnabled) return;
		Char me = Char.myCharz();
		Char attacker = GameScr.findCharInMap(attackerId);
		string attackerName = attacker != null ? attacker.cName : ("Char#" + attackerId);
		Log("CHAR_HIT_ME", $"[{source}] Attacker='{attackerName}' (ID:{attackerId}), DameHP={dameHp}, NewHP={newHp}/{me?.cHPFull}, isDie={isDie}, isCrit={isCrit}, Map='{TileMap.mapName}' (ID:{TileMap.mapID}, Zone:{TileMap.zoneID}), PlayerPos=({me?.cx},{me?.cy})");
	}
}
