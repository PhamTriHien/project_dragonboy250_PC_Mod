using System;
using System.Collections.Generic;
using Assets.src.g;
using UnityEngine;

/// <summary>
/// Module HUD Thực Thể Bản Đồ: Hiển thị thanh máu, thông tin người chơi và Boss xuất hiện trong cùng khu vực.
/// </summary>
public static class ModMapEntityHUD
{
	/// <summary>
	/// CLASS MapEntityEntry: Định nghĩa và điều phối logic trong hệ thống game.
	/// </summary>
	public class MapEntityEntry
	{
		public int entityId;
		public bool isBoss;
		public bool canAttack;
		public string name;
		public long hp;
		public long maxHp;
		public int x;
		public int y;
		public Char charRef;
		public Mob mobRef;
		public int clickX;
		public int clickY;
		public int clickW;
		public int clickH;
	}

	public static bool isShowMapEntityHUD = true;
	public static bool isShowTargetStatus = true;
	public static bool isLockTarget = false;
	public static int lockedCharId = 0;
	public static int lockedMobId = -1;
	public static int maxDisplayEntries = 15;

	private static readonly List<MapEntityEntry> currentEntries = new List<MapEntityEntry>();

	/// <summary>
	/// Thực thi phương thức nghiệp vụ FormatHp.
	/// </summary>
	public static string FormatHp(long hp)
	{
		if (hp <= 0)
		{
			return "0";
		}
		if (hp >= 1_000_000_000L)
		{
			return (hp / 1_000_000_000.0).ToString("0.##") + "T";
		}
		if (hp >= 1_000_000L)
		{
			return (hp / 1_000_000.0).ToString("0.##") + "Tr";
		}
		if (hp >= 1_000L)
		{
			return (hp / 1_000.0).ToString("0.#") + "k";
		}
		return hp.ToString();
	}

	/// <summary>
	/// Bật hoặc tắt khóa cứng mục tiêu đang chọn (Gim Mục Tiêu).
	/// </summary>
	public static void ToggleLockTarget()
	{
		isLockTarget = !isLockTarget;
		Char me = Char.myCharz();
		if (isLockTarget && me != null)
		{
			if (me.charFocus != null)
			{
				lockedCharId = me.charFocus.charID;
				lockedMobId = -1;
			}
			else if (me.mobFocus != null)
			{
				lockedMobId = me.mobFocus.mobId;
				lockedCharId = 0;
			}
		}
		else
		{
			lockedCharId = 0;
			lockedMobId = -1;
		}
		ModConfig.SaveConfig();
		GameScr.info1.addInfo("Gim mục tiêu: " + (isLockTarget ? "BẬT" : "TẮT"), 0);
		SoundMn.gI().buttonClick();
	}

	/// <summary>
	/// Duy trì mục tiêu đã khóa trên bản đồ.
	/// </summary>
	public static void UpdateLockTarget()
	{
		if (!isLockTarget || !ModMenu.IsInGame()) return;
		Char me = Char.myCharz();
		if (me == null) return;

		if (me.charFocus != null && me.charFocus.cHP > 0 && me.charFocus.statusMe != 14 && me.charFocus.statusMe != 5)
		{
			lockedCharId = me.charFocus.charID;
			lockedMobId = -1;
			return;
		}
		if (me.mobFocus != null && me.mobFocus.hp > 0 && me.mobFocus.status != 0 && me.mobFocus.status != 1)
		{
			lockedMobId = me.mobFocus.mobId;
			lockedCharId = 0;
			return;
		}

		if (lockedCharId != 0 && GameScr.vCharInMap != null)
		{
			for (int i = 0; i < GameScr.vCharInMap.size(); i++)
			{
				Char c = (Char)GameScr.vCharInMap.elementAt(i);
				if (c != null && c.charID == lockedCharId && c.cHP > 0 && c.statusMe != 14 && c.statusMe != 5)
				{
					me.charFocus = c;
					me.mobFocus = null;
					return;
				}
			}
		}
		if (lockedMobId >= 0 && GameScr.vMob != null)
		{
			for (int j = 0; j < GameScr.vMob.size(); j++)
			{
				Mob m = (Mob)GameScr.vMob.elementAt(j);
				if (m != null && m.mobId == lockedMobId && m.hp > 0 && m.status != 0 && m.status != 1)
				{
					me.mobFocus = m;
					me.charFocus = null;
					return;
				}
			}
		}
	}

	/// <summary>
	/// Vẽ đường tia đỏ nối tới mục tiêu đang khóa và hiển thị trạng thái chiến đấu của đối thủ.
	/// </summary>
	public static void PaintTargetStatusHUD(mGraphics g)
	{
		if (!ModMenu.IsInGame() || g == null) return;
		if ((GameCanvas.panel != null && GameCanvas.panel.isShow) ||
		    (GameCanvas.panel2 != null && GameCanvas.panel2.isShow) ||
		    (GameCanvas.menu != null && GameCanvas.menu.showMenu) ||
		    GameCanvas.currentDialog != null ||
		    ModUI.uiCustomOpen)
		{
			return;
		}

		try
		{
			Char me = Char.myCharz();
			if (me == null) return;

			g.translate(-g.getTranslateX(), -g.getTranslateY());
			g.setClip(0, 0, GameCanvas.w, GameCanvas.h);

			if (isLockTarget)
			{
				int tx = -1;
				int ty = -1;
				if (me.charFocus != null && me.charFocus.cHP > 0)
				{
					tx = me.charFocus.cx;
					ty = me.charFocus.cy - 12;
				}
				else if (me.mobFocus != null && me.mobFocus.hp > 0)
				{
					tx = me.mobFocus.x;
					ty = me.mobFocus.y - 12;
				}
				if (tx >= 0 && ty >= 0)
				{
					g.setColor(0xFF0000);
					g.drawLine(me.cx - GameScr.cmx, me.cy - 12 - GameScr.cmy, tx - GameScr.cmx, ty - GameScr.cmy);
				}
			}

			if (isShowTargetStatus && me.charFocus != null && me.charFocus.cHP > 0)
			{
				Char target = me.charFocus;
				int cx = GameCanvas.w / 2;
				int cy = 38;

				string hpLine = target.cName + " [" + FormatHp(target.cHP) + "/" + FormatHp(target.cHPFull > 0 ? target.cHPFull : target.cHP) + "]";
				mFont.tahoma_7b_red.drawString(g, hpLine, cx, cy, mFont.CENTER, mFont.tahoma_7_grey);
				cy += 10;

				string effStr = string.Empty;
				if (target.holdEffID != 0) effStr += "[Bị trói] ";
				if (target.sleepEff) effStr += "[Thôi miên] ";
				if (target.blindEff) effStr += "[TDHS] ";
				if (target.isFreez) effStr += "[Choáng/Băng] ";
				if (target.stone) effStr += "[Hóa đá] ";
				if (target.isMonkey == 1) effStr += "[Biến khỉ] ";
				if (target.isCharge) effStr += "[Tái tạo] ";

				if (!string.IsNullOrEmpty(effStr))
				{
					mFont.tahoma_7_yellow.drawString(g, effStr.Trim(), cx, cy, mFont.CENTER, mFont.tahoma_7_grey);
				}
			}
		}
		catch
		{
		}
	}

	/// <summary>
	/// Cập nhật trạng thái logic của UpdateEntries theo chu kỳ khung hình.
	/// </summary>
	public static void UpdateEntries()
	{
		lock (currentEntries)
		{
			currentEntries.Clear();

			Char me = Char.myCharz();
			if (me == null || !ModMenu.IsInGame())
			{
				return;
			}

			// 1. Quét nhân vật (Char) toàn map: Hiển thị chung cả Boss trong map và Player khác (bỏ qua Đệ tử, MiniPet, Nhân bản, Trọng tài)
			if (GameScr.vCharInMap != null)
			{
				for (int i = 0; i < GameScr.vCharInMap.size(); i++)
				{
					Char c = (Char)GameScr.vCharInMap.elementAt(i);
					if (c == null || c == me || string.IsNullOrEmpty(c.cName) || c.statusMe == 14 || c.statusMe == 5 || c.cHP <= 0)
					{
						continue;
					}

					if (c.cName.StartsWith("$"))
					{
						c.cName = c.cName.Substring(1).TrimStart();
						c.isPet = true;
					}
					else if (c.cName.StartsWith("#"))
					{
						c.cName = c.cName.Substring(1).TrimStart();
						c.isMiniPet = true;
					}

					string cleanName = c.cName.Trim();
					if (string.IsNullOrEmpty(cleanName) || c.isPet || c.isMiniPet || c.isCopy || c.clanID == -2 || c.charID == -114 ||
					    cleanName.StartsWith("Đệ tử", StringComparison.OrdinalIgnoreCase) ||
					    cleanName.StartsWith("Trọng tài", StringComparison.OrdinalIgnoreCase))
					{
						continue;
					}

					bool isAttackable = (c.cTypePk == 5 || c.cTypePk == 3 || me.isMeCanAttackOtherPlayer(c));
					bool isBossChar = (c.charID < 0 || c.cTypePk == 5);

					if (isBossChar)
					{
						currentEntries.Add(new MapEntityEntry
						{
							entityId = c.charID,
							isBoss = true,
							canAttack = isAttackable,
							name = cleanName,
							hp = c.cHP,
							maxHp = (c.cHPFull > 0) ? c.cHPFull : c.cHP,
							x = c.cx,
							y = c.cy,
							charRef = c,
							mobRef = null
						});
					}
					else if (c.charID > 0)
					{
						currentEntries.Add(new MapEntityEntry
						{
							entityId = c.charID,
							isBoss = false,
							canAttack = isAttackable,
							name = cleanName,
							hp = c.cHP,
							maxHp = (c.cHPFull > 0) ? c.cHPFull : c.cHP,
							x = c.cx,
							y = c.cy,
							charRef = c,
							mobRef = null
						});
					}
				}
			}

			// 2. Chỉ quét các thực thể Boss đặc biệt dạng Mob (Hirudegarn, Bạch Tuộc, NewBoss), tuyệt đối không lấy quái thường / Siêu Quái
			if (GameScr.vMob != null)
			{
				for (int j = 0; j < GameScr.vMob.size(); j++)
				{
					Mob m = (Mob)GameScr.vMob.elementAt(j);
					if (m == null || m.isMobMe || m.status == 0 || m.status == 1 || m.hp <= 0)
					{
						continue;
					}

					bool isRealBossMob = (m.isBoss || m is NewBoss || m is BigBoss || m is BigBoss2 || m is BachTuoc);
					if (isRealBossMob)
					{
						string templateName = (m.getTemplate() != null && !string.IsNullOrEmpty(m.getTemplate().name))
							? m.getTemplate().name.Trim()
							: "Boss";
						currentEntries.Add(new MapEntityEntry
						{
							entityId = 1_000_000 + m.mobId,
							isBoss = true,
							canAttack = true,
							name = templateName,
							hp = m.hp,
							maxHp = (m.maxHp > 0) ? m.maxHp : m.hp,
							x = m.x,
							y = m.y,
							charRef = null,
							mobRef = m
						});
					}
				}
			}

			// Sắp xếp cố định: Boss lên đầu (Boss tấn công được ưu tiên trước), sau đó đến Player, cùng nhóm xếp A-Z
			currentEntries.Sort((a, b) =>
			{
				if (a.isBoss != b.isBoss)
				{
					return b.isBoss.CompareTo(a.isBoss);
				}
				if (a.isBoss && a.canAttack != b.canAttack)
				{
					return b.canAttack.CompareTo(a.canAttack);
				}
				int nameCmp = string.Compare(a.name, b.name, StringComparison.OrdinalIgnoreCase);
				if (nameCmp != 0)
				{
					return nameCmp;
				}
				return a.entityId.CompareTo(b.entityId);
			});

			if (currentEntries.Count > maxDisplayEntries)
			{
				currentEntries.RemoveRange(maxDisplayEntries, currentEntries.Count - maxDisplayEntries);
			}
		}
	}

	/// <summary>
	/// Thực hiện vẽ toàn bộ đối tượng và các lớp hiển thị lên canvas mGraphics.
	/// </summary>
	public static void Paint(mGraphics g)
	{
		PaintTargetStatusHUD(g);
		if (!isShowMapEntityHUD || !ModMenu.IsInGame() || g == null)
		{
			return;
		}

		// Tự động ẩn khi đang mở Panel, Menu, Dialog hoặc Mod UI
		if ((GameCanvas.panel != null && GameCanvas.panel.isShow) ||
		    (GameCanvas.panel2 != null && GameCanvas.panel2.isShow) ||
		    (GameCanvas.menu != null && GameCanvas.menu.showMenu) ||
		    GameCanvas.currentDialog != null ||
		    ModUI.uiCustomOpen)
		{
			return;
		}

		UpdateEntries();

		lock (currentEntries)
		{
			if (currentEntries.Count == 0)
			{
				return;
			}

			try
			{
				g.translate(-g.getTranslateX(), -g.getTranslateY());
				g.setClip(0, 0, GameCanvas.w, GameCanvas.h);

				Char me = Char.myCharz();
				int startY = ModBossNotice.GetBottomY() + 4;
				int lineH = 12;
				int drawY = startY;

				for (int i = 0; i < currentEntries.Count; i++)
				{
					MapEntityEntry entry = currentEntries[i];
					if (entry == null)
					{
						continue;
					}

					bool isFocused = (me != null) && (
						(entry.charRef != null && me.charFocus == entry.charRef) ||
						(entry.mobRef != null && me.mobFocus == entry.mobRef)
					);

					string namePart = isFocused ? ("> " + entry.name) : entry.name;
					string hpPart = " - " + FormatHp(entry.hp) + "/" + FormatHp(entry.maxHp);

					mFont nameFont;
					if (entry.isBoss)
					{
						if (entry.canAttack)
						{
							// Boss trạng thái có thể tấn công được -> Chữ màu đỏ
							nameFont = isFocused
								? (mFont.tahoma_7b_red ?? mFont.tahoma_7_red ?? mFont.tahoma_7_white)
								: (mFont.tahoma_7_red ?? mFont.tahoma_7b_red ?? mFont.tahoma_7_white);
						}
						else
						{
							// Boss chưa thể tấn công (đang chờ / cTypePk == 0) -> Chữ màu vàng
							nameFont = isFocused
								? (mFont.tahoma_7b_yellow ?? mFont.tahoma_7_yellow ?? mFont.tahoma_7_white)
								: (mFont.tahoma_7_yellow ?? mFont.tahoma_7_white);
						}
					}
					else if (entry.canAttack)
					{
						nameFont = isFocused
							? (mFont.tahoma_7b_red ?? mFont.tahoma_7_red ?? mFont.tahoma_7_white)
							: (mFont.tahoma_7_red ?? mFont.tahoma_7_white);
					}
					else
					{
						nameFont = isFocused
							? (mFont.tahoma_7b_yellow ?? mFont.tahoma_7_yellow ?? mFont.tahoma_7_white)
							: (mFont.tahoma_7_green2 ?? mFont.tahoma_7_white);
					}

					mFont hpFont = entry.isBoss
						? (mFont.tahoma_7_yellow ?? mFont.tahoma_7_white)
						: mFont.tahoma_7_white;

					if (nameFont == null || hpFont == null)
					{
						break;
					}

					int nameW = nameFont.getWidth(namePart);
					int hpW = hpFont.getWidth(hpPart);
					int totalW = nameW + hpW;

					int lineX = GameCanvas.w - totalW - 2 - SafeArea.Right;
					if (lineX < 2)
					{
						lineX = 2;
					}

					nameFont.drawString(g, namePart, lineX, drawY, mFont.LEFT);
					hpFont.drawString(g, hpPart, lineX + nameW, drawY, mFont.LEFT);

					entry.clickX = lineX;
					entry.clickY = drawY - 1;
					entry.clickW = totalW + 2;
					entry.clickH = lineH;

					drawY += lineH + 2;
				}
			}
			catch
			{
			}
		}
	}

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của CheckClick.
	/// </summary>
	public static bool CheckClick(int px, int py)
	{
		if (!isShowMapEntityHUD || !ModMenu.IsInGame())
		{
			return false;
		}

		if ((GameCanvas.panel != null && GameCanvas.panel.isShow) ||
		    (GameCanvas.panel2 != null && GameCanvas.panel2.isShow) ||
		    (GameCanvas.menu != null && GameCanvas.menu.showMenu) ||
		    GameCanvas.currentDialog != null ||
		    ModUI.uiCustomOpen)
		{
			return false;
		}

		lock (currentEntries)
		{
			if (currentEntries.Count == 0)
			{
				return false;
			}

			for (int i = 0; i < currentEntries.Count; i++)
			{
				MapEntityEntry entry = currentEntries[i];
				if (entry == null)
				{
					continue;
				}

				if (px >= entry.clickX && px <= entry.clickX + entry.clickW &&
				    py >= entry.clickY && py <= entry.clickY + entry.clickH)
				{
					if (GameCanvas.isPointerClick || GameCanvas.isPointerJustRelease)
					{
						GameCanvas.clearAllPointerEvent();

						Char me = Char.myCharz();
						if (me != null)
						{
							me.currentMovePoint = null;
							me.vMovePoints.removeAllElements();

							// Khóa mục tiêu
							if (entry.charRef != null)
							{
								me.charFocus = entry.charRef;
								me.mobFocus = null;
								me.npcFocus = null;
								lockedCharId = entry.charRef.charID;
								lockedMobId = -1;
							}
							else if (entry.mobRef != null)
							{
								me.mobFocus = entry.mobRef;
								me.charFocus = null;
								me.npcFocus = null;
								lockedMobId = entry.mobRef.mobId;
								lockedCharId = 0;
							}

							// Dịch chuyển tức thời đến tọa độ thực thể
							ModTeleport.TeleportTo(entry.x, entry.y);
						}

						if (GameScr.gI() != null)
						{
							GameScr.gI().clickMoving = false;
						}

						SoundMn.gI().buttonClick();
						GameScr.info1.addInfo("Đến: " + entry.name, 0);
						return true;
					}
					return true;
				}
			}
		}

		return false;
	}
}
