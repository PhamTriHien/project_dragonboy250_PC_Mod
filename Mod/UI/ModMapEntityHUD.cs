using System;
using System.Collections.Generic;
using UnityEngine;

public static class ModMapEntityHUD
{
	public class MapEntityEntry
	{
		public int entityId;
		public bool isBoss;
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
	public static int maxDisplayEntries = 8;

	private static readonly List<MapEntityEntry> currentEntries = new List<MapEntityEntry>();

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

			// 1. Quét nhân vật (Char) trong map: Boss và Player khác
			if (GameScr.vCharInMap != null)
			{
				for (int i = 0; i < GameScr.vCharInMap.size(); i++)
				{
					Char c = (Char)GameScr.vCharInMap.elementAt(i);
					if (c == null || string.IsNullOrEmpty(c.cName) || c.statusMe == 14 || c.statusMe == 5)
					{
						continue;
					}

					bool isBossChar = (c.charID < 0 || c.cTypePk == 5 || ModBossNotice.IsBossName(c.cName));
					if (isBossChar)
					{
						currentEntries.Add(new MapEntityEntry
						{
							entityId = c.charID,
							isBoss = true,
							name = c.cName,
							hp = c.cHP,
							maxHp = (c.cHPFull > 0) ? c.cHPFull : c.cHP,
							x = c.cx,
							y = c.cy,
							charRef = c,
							mobRef = null
						});
					}
					else if (c.charID > 0 && !c.isPet && !c.isMiniPet && !c.cName.StartsWith("["))
					{
						currentEntries.Add(new MapEntityEntry
						{
							entityId = c.charID,
							isBoss = false,
							name = c.cName,
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

			// 2. Quét quái vật (Mob) trong map: Tìm Boss dạng Mob
			if (GameScr.vMob != null)
			{
				for (int j = 0; j < GameScr.vMob.size(); j++)
				{
					Mob m = (Mob)GameScr.vMob.elementAt(j);
					if (m == null || m.status == 0 || m.status == 1 || m.hp <= 0)
					{
						continue;
					}

					string templateName = (m.getTemplate() != null) ? m.getTemplate().name : string.Empty;
					bool isBossMob = (m.isBoss || m.levelBoss > 0 || (!string.IsNullOrEmpty(templateName) && ModBossNotice.IsBossName(templateName)));

					if (isBossMob)
					{
						currentEntries.Add(new MapEntityEntry
						{
							entityId = 1_000_000 + m.mobId,
							isBoss = true,
							name = !string.IsNullOrEmpty(templateName) ? templateName : "Boss",
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

			// Sắp xếp cố định: Boss lên đầu, trong cùng nhóm sắp xếp theo tên A-Z (nếu trùng tên theo entityId)
			// Giữ danh sách ổn định tuyệt đối, không bị nhảy/hoán đổi vị trí liên tục khi nhân vật di chuyển
			currentEntries.Sort((a, b) =>
			{
				if (a.isBoss != b.isBoss)
				{
					return b.isBoss.CompareTo(a.isBoss);
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

	public static void Paint(mGraphics g)
	{
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

					// Bỏ tiền tố [BOSS], hiển thị trực tiếp tên thực thể
					string namePart = entry.name;
					string hpPart = " - " + FormatHp(entry.hp) + "/" + FormatHp(entry.maxHp);

					mFont nameFont;
					if (isFocused)
					{
						nameFont = mFont.tahoma_7b_yellow ?? mFont.tahoma_7_yellow;
					}
					else if (entry.isBoss)
					{
						nameFont = mFont.tahoma_7_red ?? mFont.tahoma_7_white;
					}
					else
					{
						nameFont = mFont.tahoma_7_green2 ?? mFont.tahoma_7_white;
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

					int lineX = GameCanvas.w - totalW - 2;
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
							}
							else if (entry.mobRef != null)
							{
								me.mobFocus = entry.mobRef;
								me.charFocus = null;
								me.npcFocus = null;
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
