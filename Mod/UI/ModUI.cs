using System;
using System.Collections.Generic;
using UnityEngine;

public static class ModUI
{
	public static bool uiCustomOpen = false;
	public static bool uiTanSatOpen
	{
		get { return uiCustomOpen; }
		set { uiCustomOpen = value; }
	}
	public static int selectedTab = 0;
	public static int tanSatTab = 0;

	public static readonly string[] tabNames = new string[11]
	{
		"Tàn Sát",
		"Tự Nhặt",
		"Tốc Độ",
		"Hồi Máu",
		"Tự Động",
		"Đồ Họa",
		"Báo Boss",
		"Qua Map",
		"GoBack",
		"Úp Set KH",
		"Lệnh & Phím"
	};

	public static int colScrollY = 0;
	private static bool isColDragging = false;
	private static bool hasColDragged = false;
	private static int startColDragY = 0;
	private static int lastColDragY = 0;

	public static int detailScrollY = 0;
	private static bool isDetailDragging = false;
	private static bool hasDetailDragged = false;
	private static int startDetailDragY = 0;
	private static int lastDetailDragY = 0;

	private static Image imgBtX;

	public static void ReloadForGraphicsMode()
	{
		imgBtX = null;
	}

	public static List<int> GetUniqueMobTemplateIds()
	{
		List<int> list = new List<int>();
		if (GameScr.vMob != null)
		{
			for (int i = 0; i < GameScr.vMob.size(); i++)
			{
				Mob m = (Mob)GameScr.vMob.elementAt(i);
				if (m != null && !m.isMobMe && !list.Contains(m.templateId))
				{
					list.Add(m.templateId);
				}
			}
		}
		return list;
	}

	public static List<Skill> GetPlayerAttackSkills()
	{
		List<Skill> list = new List<Skill>();
		List<int> addedIds = new List<int>();
		Char me = Char.myCharz();

		if (GameScr.keySkill != null)
		{
			for (int i = 0; i < GameScr.keySkill.Length; i++)
			{
				Skill s = GameScr.keySkill[i];
				if (s != null && s.template != null && !addedIds.Contains(s.template.id))
				{
					list.Add(s);
					addedIds.Add(s.template.id);
				}
			}
		}

		if (GameScr.onScreenSkill != null)
		{
			for (int j = 0; j < GameScr.onScreenSkill.Length; j++)
			{
				Skill s2 = GameScr.onScreenSkill[j];
				if (s2 != null && s2.template != null && !addedIds.Contains(s2.template.id))
				{
					list.Add(s2);
					addedIds.Add(s2.template.id);
				}
			}
		}

		if (me != null && me.myskill != null && me.myskill.template != null && !addedIds.Contains(me.myskill.template.id))
		{
			list.Add(me.myskill);
			addedIds.Add(me.myskill.template.id);
		}

		if (me != null && me.vSkillFight != null)
		{
			for (int k = 0; k < me.vSkillFight.size(); k++)
			{
				Skill s3 = (Skill)me.vSkillFight.elementAt(k);
				if (s3 != null && s3.template != null && !addedIds.Contains(s3.template.id))
				{
					list.Add(s3);
					addedIds.Add(s3.template.id);
				}
			}
		}

		return list;
	}

	public static string GetSkillShortName(Skill s)
	{
		if (s == null || s.template == null)
		{
			return string.Empty;
		}
		string n = s.template.name;
		if (string.IsNullOrEmpty(n))
		{
			return "Chiêu " + s.template.id;
		}
		if (n.Length > 12)
		{
			return n.Substring(0, 10) + "..";
		}
		return n;
	}

	public static void DrawCheckbox(int bx, int by, bool isChecked, mGraphics g)
	{
		if (Paint.imgCheck == null)
		{
			try { Paint.imgCheck = GameCanvas.loadImage("/mainImage/myTexture2dcheck.png"); } catch { }
		}
		if (Paint.imgCheck != null)
		{
			g.drawRegion(Paint.imgCheck, 0, (isChecked ? 2 : 0) * 18, 20, 18, 0, bx, by, 0);
		}
		else
		{
			GameCanvas.paintz.paintCheckPass(g, bx, by, isChecked, false);
		}
	}

	public static void PaintNativeButton(int x, int y, int w, int h, string text, bool isFocus, mGraphics g)
	{
		try
		{
			if (Command.btn0left == null)
			{
				try
				{
					Command.btn0left = GameCanvas.loadImage("/mainImage/btn0left.png");
					Command.btn0mid = GameCanvas.loadImage("/mainImage/btn0mid.png");
					Command.btn0right = GameCanvas.loadImage("/mainImage/btn0right.png");
					Command.btn1left = GameCanvas.loadImage("/mainImage/btn1left.png");
					Command.btn1mid = GameCanvas.loadImage("/mainImage/btn1mid.png");
					Command.btn1right = GameCanvas.loadImage("/mainImage/btn1right.png");
				}
				catch { }
			}

			if (Command.btn0left != null && Command.btn0mid != null && Command.btn0right != null)
			{
				Image bLeft = isFocus ? Command.btn1left : Command.btn0left;
				Image bMid = isFocus ? Command.btn1mid : Command.btn0mid;
				Image bRight = isFocus ? Command.btn1right : Command.btn0right;

				if (h >= 24 && w >= 20)
				{
					Command.paintOngMau(bLeft, bMid, bRight, x, y, w, g);
				}
				else
				{
					int topH = h / 2;
					int botH = h - topH;
					int srcBotY = 24 - botH;

					if (w >= 20)
					{
						g.drawRegion(bLeft, 0, 0, 10, topH, 0, x, y, 0);
						for (int i = 10; i <= w - 20; i += 10)
						{
							g.drawRegion(bMid, 0, 0, 10, topH, 0, x + i, y, 0);
						}
						int rem = w % 10;
						if (rem > 0)
						{
							g.drawRegion(bMid, 0, 0, rem, topH, 0, x + w - 10 - rem, y, 0);
						}
						g.drawRegion(bRight, 0, 0, 10, topH, 0, x + w - 10, y, 0);

						g.drawRegion(bLeft, 0, srcBotY, 10, botH, 0, x, y + topH, 0);
						for (int i = 10; i <= w - 20; i += 10)
						{
							g.drawRegion(bMid, 0, srcBotY, 10, botH, 0, x + i, y + topH, 0);
						}
						if (rem > 0)
						{
							g.drawRegion(bMid, 0, srcBotY, rem, botH, 0, x + w - 10 - rem, y + topH, 0);
						}
						g.drawRegion(bRight, 0, srcBotY, 10, botH, 0, x + w - 10, y + topH, 0);
					}
					else
					{
						int halfW = w / 2;
						int remW = w - halfW;
						g.drawRegion(bLeft, 0, 0, halfW, topH, 0, x, y, 0);
						g.drawRegion(bRight, 10 - remW, 0, remW, topH, 0, x + halfW, y, 0);
						g.drawRegion(bLeft, 0, srcBotY, halfW, botH, 0, x, y + topH, 0);
						g.drawRegion(bRight, 10 - remW, srcBotY, remW, botH, 0, x + halfW, y + topH, 0);
					}
				}
			}
			else
			{
				g.setColor(isFocus ? 16383818 : 14338484);
				g.fillRect(x + 1, y + 1, w - 2, h - 2);
				g.setColor(6702080);
				g.drawRect(x, y, w - 1, h - 1);
			}

			int textY = y + (h - 10) / 2;
			(isFocus ? (mFont.tahoma_7_green2 ?? mFont.tahoma_7b_green2) : (mFont.tahoma_7_grey ?? mFont.tahoma_7b_dark)).drawString(g, text, x + w / 2, textY, mFont.CENTER);
		}
		catch
		{
		}
	}

	public static void PaintNativeButton(int x, int y, int w, string text, bool isFocus, mGraphics g)
	{
		PaintNativeButton(x, y, w, 18, text, isFocus, g);
	}

	public static void PaintArrowButton(int x, int y, int w, int h, bool isUp, bool isFocus, mGraphics g)
	{
		PaintNativeButton(x, y, w, h, string.Empty, isFocus, g);
		if (Mob.imgHP != null)
		{
			int arrowX = x + (w - 9) / 2;
			int arrowY = y + (h - 6) / 2;
			g.drawRegion(Mob.imgHP, 0, 0, 9, 6, isUp ? 1 : 0, arrowX, arrowY, 0);
		}
		else
		{
			int cx = x + w / 2;
			int cy = y + h / 2;
			g.setColor(isFocus ? 0x00783e : 0x3c1400);
			if (isUp)
			{
				for (int r = 0; r < 4; r++)
				{
					g.fillRect(cx - r, cy - 2 + r, r * 2 + 1, 1);
				}
			}
			else
			{
				for (int r = 0; r < 4; r++)
				{
					g.fillRect(cx - (3 - r), cy - 1 + r, (3 - r) * 2 + 1, 1);
				}
			}
		}
	}

	public static void PaintTanSatUI(mGraphics g)
	{
		if (!uiCustomOpen)
		{
			return;
		}
		try
		{
			int maxW = GameCanvas.w - 12;
			int maxH = GameCanvas.h - 12;
			int uiW = (maxW > 450) ? 450 : maxW;
			int uiH = (maxH > 260) ? 260 : maxH;
			if (uiW < 280) uiW = GameCanvas.w;
			if (uiH < 200) uiH = GameCanvas.h;
			int uiX = (GameCanvas.w - uiW) / 2;
			int uiY = (GameCanvas.h - uiH) / 2;
			if (uiX < 0) uiX = 0;
			if (uiY < 0) uiY = 0;

			GameCanvas.paintz.paintFrame(uiX, uiY, uiW, uiH, g);
			GameCanvas.paintz.paintFrameInside(uiX + 6, uiY + 6, uiW - 12, uiH - 12, g);

			string currentTabName = (selectedTab >= 0 && selectedTab < tabNames.Length) ? tabNames[selectedTab] : "CÀI ĐẶT";
			string title = "MENU MOD - " + currentTabName.ToUpper();
			mFont.tahoma_7b_dark.drawString(g, title, uiX + uiW / 2, uiY + 8, mFont.CENTER);

			if (imgBtX == null)
			{
				try
				{
					imgBtX = GameCanvas.loadImage("/mainImage/myTexture2dbtX.png");
				}
				catch
				{
				}
			}
			if (imgBtX != null)
			{
				g.drawImage(imgBtX, uiX + uiW - 25, uiY + 6, 0);
			}
			else
			{
				g.setColor(0x8B0000);
				g.fillRect(uiX + uiW - 25, uiY + 7, 18, 18);
				g.setColor(0xFFFFFF);
				g.drawRect(uiX + uiW - 25, uiY + 7, 18, 18);
				mFont.tahoma_7b_white.drawString(g, "X", uiX + uiW - 16, uiY + 9, mFont.CENTER);
			}

			int colW = (uiW > 380) ? 76 : 68;
			int colX = uiX + 6;
			int colY = uiY + 24;
			int colH = uiH - 30;

			int closeBtnH = 17;
			int closeBtnY = colY + colH - closeBtnH - 2;
			int tabListH = closeBtnY - colY - 3;
			int itemH = 17;
			int itemStep = 19;
			int colContentH = tabNames.Length * itemStep + 2;
			int maxColScroll = (colContentH > tabListH) ? (colContentH - tabListH) : 0;
			if (colScrollY > maxColScroll) colScrollY = maxColScroll;
			if (colScrollY < 0) colScrollY = 0;

			GameCanvas.paintz.paintFrameSimple(colX, colY, colW, colH, g);
			g.setColor(15196114);
			g.fillRect(colX + 2, colY + 2, colW - 4, colH - 4);

			g.setClip(colX + 2, colY + 2, colW - 4, tabListH);
			for (int t = 0; t < tabNames.Length; t++)
			{
				int btnY = colY + 2 + t * itemStep - colScrollY;
				if (btnY + itemH < colY || btnY > colY + tabListH) continue;
				bool isSel = (selectedTab == t);
				PaintNativeButton(colX + 2, btnY, colW - 4, itemH, tabNames[t], isSel, g);
			}
			g.setClip(0, 0, GameCanvas.w, GameCanvas.h);

			PaintNativeButton(colX + 2, closeBtnY, colW - 4, closeBtnH, "ĐÓNG", false, g);

			int detailX = colX + colW + 5;
			int detailY = uiY + 24;
			int detailW = uiW - (colW + 17);
			int detailH = uiH - 30;

			GameCanvas.paintz.paintFrameSimple(detailX, detailY, detailW, detailH, g);
			g.setColor(15787715);
			g.fillRect(detailX + 2, detailY + 2, detailW - 4, detailH - 4);

			g.setClip(detailX + 2, detailY + 2, detailW - 4, detailH - 4);

			bool useDetailScroll = (selectedTab == 5 && !ModUIBackground.isOpen);
			if (useDetailScroll)
			{
				int maxPaintDetailScroll = (ModUIGraphics.ContentHeight - (detailH - 4) > 0) ? (ModUIGraphics.ContentHeight - (detailH - 4)) : 0;
				if (detailScrollY > maxPaintDetailScroll) detailScrollY = maxPaintDetailScroll;
				if (detailScrollY < 0) detailScrollY = 0;
				g.translate(0, -detailScrollY);
			}

			switch (selectedTab)
			{
				case 0:
					ModUITanSat.Paint(detailX, detailY, detailW, detailH, g);
					break;
				case 1:
					ModUIAutoPick.Paint(detailX, detailY, detailW, detailH, g);
					break;
				case 2:
					ModUISpeed.Paint(detailX, detailY, detailW, detailH, g);
					break;
				case 3:
					ModUIAutoHeal.Paint(detailX, detailY, detailW, detailH, g);
					break;
				case 4:
					ModUIAutomation.Paint(detailX, detailY, detailW, detailH, g);
					break;
				case 5:
					if (ModUIBackground.isOpen)
					{
						ModUIBackground.Paint(detailX, detailY, detailW, detailH, g);
					}
					else
					{
						ModUIGraphics.Paint(detailX, detailY, detailW, detailH, g);
					}
					break;
				case 6:
					ModUIBoss.Paint(detailX, detailY, detailW, detailH, g);
					break;
				case 7:
					ModUINextMap.Paint(detailX, detailY, detailW, detailH, g);
					break;
				case 8:
					ModUIGoBack.Paint(detailX, detailY, detailW, detailH, g);
					break;
				case 9:
					ModUISetActivator.Paint(detailX, detailY, detailW, detailH, g);
					break;
				case 10:
					ModUIHelp.Paint(detailX, detailY, detailW, detailH, g);
					break;
			}

			if (useDetailScroll)
			{
				g.translate(0, detailScrollY);
			}

			g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
		}
		catch
		{
		}
	}

	public static void HandleTap()
	{
		if (!ModMenu.IsInGame() && !uiCustomOpen)
		{
			return;
		}
		try
		{
			if (ModArrowButton.CheckClick())
			{
				return;
			}

			int maxW = GameCanvas.w - 12;
			int maxH = GameCanvas.h - 12;
			int uiW = (maxW > 450) ? 450 : maxW;
			int uiH = (maxH > 260) ? 260 : maxH;
			if (uiW < 280) uiW = GameCanvas.w;
			if (uiH < 200) uiH = GameCanvas.h;
			int uiX = (GameCanvas.w - uiW) / 2;
			int uiY = (GameCanvas.h - uiH) / 2;
			if (uiX < 0) uiX = 0;
			if (uiY < 0) uiY = 0;

			if (uiCustomOpen)
			{
				int px = GameCanvas.px;
				int py = GameCanvas.py;

				int colW = (uiW > 380) ? 76 : 68;
				int colX = uiX + 6;
				int colY = uiY + 24;
				int colH = uiH - 30;

				int closeBtnH = 17;
				int closeBtnY = colY + colH - closeBtnH - 2;
				int tabListH = closeBtnY - colY - 3;
				int itemStep = 19;
				int colContentH = tabNames.Length * itemStep + 2;
				int maxColScroll = (colContentH > tabListH) ? (colContentH - tabListH) : 0;

				int detailX = colX + colW + 5;
				int detailY = uiY + 24;
				int detailW = uiW - (colW + 17);
				int detailH = uiH - 30;
				int maxDetailScroll = (ModUIGraphics.ContentHeight - (detailH - 4) > 0) ? (ModUIGraphics.ContentHeight - (detailH - 4)) : 0;

				bool isDown = Input.GetMouseButton(0) || GameCanvas.isPointerDown;
				bool isJustRelease = Input.GetMouseButtonUp(0) || GameCanvas.isPointerJustRelease;

				float wheel = Input.GetAxis("Mouse ScrollWheel");
				if (wheel == 0 && GameCanvas.pXYScrollMouse != 0)
				{
					wheel = GameCanvas.pXYScrollMouse;
					GameCanvas.pXYScrollMouse = 0;
				}
				if (wheel != 0)
				{
					if (px >= colX && px <= colX + colW && py >= colY && py <= colY + tabListH)
					{
						int step = 28;
						if (wheel > 0) colScrollY -= step;
						else if (wheel < 0) colScrollY += step;
						if (colScrollY < 0) colScrollY = 0;
						if (colScrollY > maxColScroll) colScrollY = maxColScroll;
					}
					else if (px >= detailX && px <= detailX + detailW && py >= detailY && py <= detailY + detailH)
					{
						if (selectedTab == 5 && !ModUIBackground.isOpen)
						{
							int step = 28;
							if (wheel > 0) detailScrollY -= step;
							else if (wheel < 0) detailScrollY += step;
							if (detailScrollY < 0) detailScrollY = 0;
							if (detailScrollY > maxDetailScroll) detailScrollY = maxDetailScroll;
						}
						else if (selectedTab == 5 && ModUIBackground.isOpen)
						{
							ModUIBackground.OnMouseScroll(wheel);
						}
						else if (selectedTab == 4)
						{
							ModUIAutomation.OnMouseScroll(wheel);
						}
						else if (selectedTab == 9)
						{
							ModUISetActivator.OnMouseScroll(wheel);
						}
						else if (selectedTab == 10)
						{
							ModUIHelp.OnMouseScroll(wheel);
						}
						else if (selectedTab == 0)
						{
							ModUITanSat.OnMouseScroll(wheel);
						}
						else if (selectedTab == 7)
						{
							ModUINextMap.OnMouseScroll(wheel);
						}
					}
				}

				if (isDown)
				{
					if (!isColDragging && !isDetailDragging)
					{
						if (px >= colX && px <= colX + colW && py >= colY && py <= colY + tabListH)
						{
							isColDragging = true;
							hasColDragged = false;
							startColDragY = py;
							lastColDragY = py;
						}
						else if (px >= detailX && px <= detailX + detailW && py >= detailY && py <= detailY + detailH)
						{
							if (selectedTab == 5 && !ModUIBackground.isOpen)
							{
								isDetailDragging = true;
								hasDetailDragged = false;
								startDetailDragY = py;
								lastDetailDragY = py;
							}
						}
					}

					if (isColDragging)
					{
						int moveY = py - lastColDragY;
						lastColDragY = py;
						if (Res.abs(py - startColDragY) > 5)
						{
							hasColDragged = true;
						}
						if (hasColDragged && maxColScroll > 0)
						{
							colScrollY -= moveY;
							if (colScrollY < 0) colScrollY = 0;
							if (colScrollY > maxColScroll) colScrollY = maxColScroll;
						}
					}
					else if (isDetailDragging)
					{
						int moveY = py - lastDetailDragY;
						lastDetailDragY = py;
						if (Res.abs(py - startDetailDragY) > 5)
						{
							hasDetailDragged = true;
						}
						if (hasDetailDragged && maxDetailScroll > 0)
						{
							detailScrollY -= moveY;
							if (detailScrollY < 0) detailScrollY = 0;
							if (detailScrollY > maxDetailScroll) detailScrollY = maxDetailScroll;
						}
					}
				}

				if (selectedTab == 10)
				{
					ModUIHelp.UpdateDragScroll(px, py, detailX, detailY, detailW, detailH);
				}
				else if (selectedTab == 4)
				{
					ModUIAutomation.UpdateDragScroll(px, py, detailX, detailY, detailW, detailH);
				}
				else if (selectedTab == 9)
				{
					ModUISetActivator.UpdateDragScroll(px, py, detailX, detailY, detailW, detailH);
				}
				else if (selectedTab == 0)
				{
					ModUITanSat.UpdateDragScroll(px, py, detailX, detailY, detailW, detailH);
				}
				else if (selectedTab == 7)
				{
					ModUINextMap.UpdateDragScroll(px, py, detailX, detailY, detailW, detailH);
				}
				else if (selectedTab == 5 && ModUIBackground.isOpen)
				{
					ModUIBackground.UpdateDragScroll(px, py, detailX, detailY, detailW, detailH);
				}

				if (isJustRelease)
				{
					if (isColDragging || (px >= colX && px <= colX + colW && py >= colY && py <= colY + tabListH))
					{
						isColDragging = false;
						if (!hasColDragged && px >= colX + 2 && px <= colX + colW - 2 && py >= colY + 2 && py <= colY + tabListH)
						{
							int relY = py - (colY + 2) + colScrollY;
							if (relY >= 0)
							{
								int clickedIdx = relY / itemStep;
								if (clickedIdx >= 0 && clickedIdx < tabNames.Length)
								{
									selectedTab = clickedIdx;
									detailScrollY = 0;
									ModUIBackground.isOpen = false;
									ModConfig.SaveConfig();
									SoundMn.gI().buttonClick();
									GameCanvas.isPointerJustRelease = GameCanvas.isPointerClick = false;
									return;
								}
							}
						}
						hasColDragged = false;
					}

					if (px >= colX && px <= colX + colW && py >= closeBtnY - 2 && py <= closeBtnY + closeBtnH + 2)
					{
						uiCustomOpen = false;
						ModUIBackground.isOpen = false;
						ModConfig.SaveConfig();
						SoundMn.gI().buttonClose();
						GameCanvas.isPointerJustRelease = GameCanvas.isPointerClick = false;
						return;
					}

					if (isDetailDragging || (px >= detailX && px <= detailX + detailW && py >= detailY && py <= detailY + detailH))
					{
						if (selectedTab == 5 && !ModUIBackground.isOpen && !hasDetailDragged)
						{
							if (ModUIGraphics.HandleTap(px, py + detailScrollY, detailX, detailY, detailW, detailH))
							{
								isDetailDragging = false;
								hasDetailDragged = false;
								GameCanvas.isPointerJustRelease = GameCanvas.isPointerClick = false;
								return;
							}
						}
						isDetailDragging = false;
						hasDetailDragged = false;
					}

					if (px >= uiX + uiW - 32 && px <= uiX + uiW - 4 && py >= uiY + 4 && py <= uiY + 28)
					{
						uiCustomOpen = false;
						ModUIBackground.isOpen = false;
						ModConfig.SaveConfig();
						SoundMn.gI().buttonClose();
						GameCanvas.isPointerJustRelease = GameCanvas.isPointerClick = false;
						return;
					}

					if (px >= detailX && px <= detailX + detailW && py >= detailY && py <= detailY + detailH)
					{
						bool handled = false;
						switch (selectedTab)
						{
							case 0:
								handled = ModUITanSat.HandleTap(px, py, detailX, detailY, detailW, detailH);
								break;
							case 1:
								handled = ModUIAutoPick.HandleTap(px, py, detailX, detailY, detailW, detailH);
								break;
							case 2:
								handled = ModUISpeed.HandleTap(px, py, detailX, detailY, detailW, detailH);
								break;
							case 3:
								handled = ModUIAutoHeal.HandleTap(px, py, detailX, detailY, detailW, detailH);
								break;
							case 4:
								handled = ModUIAutomation.HandleTap(px, py, detailX, detailY, detailW, detailH);
								break;
							case 5:
								if (ModUIBackground.isOpen)
								{
									handled = ModUIBackground.HandleTap(px, py, detailX, detailY, detailW, detailH);
								}
								break;
							case 6:
								handled = ModUIBoss.HandleTap(px, py, detailX, detailY, detailW, detailH);
								break;
							case 7:
								handled = ModUINextMap.HandleTap(px, py, detailX, detailY, detailW, detailH);
								break;
							case 8:
								handled = ModUIGoBack.HandleTap(px, py, detailX, detailY, detailW, detailH);
								break;
							case 9:
								handled = ModUISetActivator.HandleTap(px, py, detailX, detailY, detailW, detailH);
								break;
							case 10:
								handled = ModUIHelp.HandleTap(px, py, detailX, detailY, detailW, detailH);
								break;
						}
						if (handled)
						{
							GameCanvas.isPointerJustRelease = GameCanvas.isPointerClick = false;
							return;
						}
					}

					if (px >= uiX && px <= uiX + uiW && py >= uiY && py <= uiY + uiH)
					{
						GameCanvas.isPointerJustRelease = GameCanvas.isPointerClick = false;
					}
				}

				if (!isDown)
				{
					isColDragging = false;
					hasColDragged = false;
					isDetailDragging = false;
					hasDetailDragged = false;
				}
			}
		}
		catch
		{
		}
	}
}
