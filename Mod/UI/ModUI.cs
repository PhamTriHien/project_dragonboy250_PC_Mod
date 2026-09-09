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

	public static readonly string[] tabNames = new string[10]
	{
		"Tàn Sát",
		"Tự Nhặt",
		"Tốc Độ",
		"Hồi Máu",
		"Đồ Họa",
		"Báo Boss",
		"Qua Map",
		"GoBack",
		"Úp Set KH",
		"Lệnh & Phím"
	};

	// Cột Danh Mục Trái (Left Sidebar) - Chạm kéo trực tiếp, không thanh cuộn
	public static int colScrollY = 0;
	private static bool isColDragging = false;
	private static bool hasColDragged = false;
	private static int startColDragY = 0;
	private static int lastColDragY = 0;

	// Khung Nội Dung Phải (Right Detail Panel) - Chạm kéo trực tiếp, không thanh cuộn
	public static int detailScrollY = 0;
	private static bool isDetailDragging = false;
	private static bool hasDetailDragged = false;
	private static int startDetailDragY = 0;
	private static int lastDetailDragY = 0;

	private static Image imgBtX;

	public static List<int> GetUniqueMobTemplateIds()
	{
		List<int> list = new List<int>();
		if (GameScr.vMob != null)
		{
			for (int i = 0; i < GameScr.vMob.size(); i++)
			{
				Mob m = (Mob)GameScr.vMob.elementAt(i);
				if (m != null && !list.Contains(m.templateId))
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

				if (w >= 20)
				{
					Command.paintOngMau(bLeft, bMid, bRight, x, y, w, g);
				}
				else
				{
					g.drawRegion(bLeft, 0, 0, w / 2, 24, 0, x, y, 0);
					g.drawRegion(bRight, 10 - (w - w / 2), 0, w - w / 2, 24, 0, x + w / 2, y, 0);
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
			(isFocus ? mFont.tahoma_7b_green2 : mFont.tahoma_7b_dark).drawString(g, text, x + w / 2, textY, mFont.CENTER);
		}
		catch
		{
		}
	}

	public static void PaintNativeButton(int x, int y, int w, string text, bool isFocus, mGraphics g)
	{
		PaintNativeButton(x, y, w, 20, text, isFocus, g);
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
			// Tự động scale tương thích mọi tỷ lệ và độ phân giải màn hình
			int maxW = GameCanvas.w - 12;
			int maxH = GameCanvas.h - 12;
			int uiW = (maxW < 440) ? maxW : 440;
			int uiH = (maxH < 260) ? maxH : 260;
			if (uiW < 320) uiW = GameCanvas.w;
			if (uiH < 220) uiH = GameCanvas.h;
			int uiX = (GameCanvas.w - uiW) / 2;
			int uiY = (GameCanvas.h - uiH) / 2;
			if (uiX < 0) uiX = 0;
			if (uiY < 0) uiY = 0;

			// 1. Khung Dialog chính: Asset chuẩn NRO
			GameCanvas.paintz.paintFrame(uiX, uiY, uiW, uiH, g);
			GameCanvas.paintz.paintFrameInside(uiX + 6, uiY + 6, uiW - 12, uiH - 12, g);

			// Tiêu đề
			string currentTabName = (selectedTab >= 0 && selectedTab < tabNames.Length) ? tabNames[selectedTab] : "CÀI ĐẶT";
			string title = "MENU MOD - " + currentTabName.ToUpper();
			mFont.tahoma_7b_dark.drawString(g, title, uiX + uiW / 2, uiY + 9, mFont.CENTER);

			// Nút [X] đóng góc phải
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

			// 2. Cột Danh Mục Bên Trái (Chạm kéo trực tiếp - Không thanh cuộn)
			int colW = (uiW > 380) ? 96 : 82;
			int colH = uiH - 62;
			int colX = uiX + 8;
			int colY = uiY + 28;
			int closeBtnY = uiY + uiH - 30;

			GameCanvas.paintz.paintFrameSimple(colX, colY, colW, colH, g);
			g.setColor(15196114);
			g.fillRect(colX + 2, colY + 2, colW - 4, colH - 4);

			int itemH = 22;
			int itemStep = 24;
			int colContentH = tabNames.Length * itemStep + 6;
			int maxColScroll = (colContentH > colH - 4) ? (colContentH - (colH - 4)) : 0;
			if (colScrollY > maxColScroll) colScrollY = maxColScroll;
			if (colScrollY < 0) colScrollY = 0;

			g.setClip(colX + 2, colY + 2, colW - 4, colH - 4);
			for (int t = 0; t < tabNames.Length; t++)
			{
				int btnY = colY + 3 + t * itemStep - colScrollY;
				if (btnY + itemH < colY || btnY > colY + colH) continue;
				bool isSel = (selectedTab == t);
				PaintNativeButton(colX + 3, btnY, colW - 6, itemH, tabNames[t], isSel, g);
			}
			g.setClip(0, 0, GameCanvas.w, GameCanvas.h);

			// Nút ĐÓNG cố định ở đáy Cột Trái
			PaintNativeButton(colX, closeBtnY, colW, 22, "ĐÓNG", false, g);

			// 3. Vùng Nội Dung Chi Tiết Bên Phải (Chạm kéo trực tiếp - Không thanh cuộn)
			int detailX = colX + colW + 6;
			int detailY = uiY + 28;
			int detailW = uiW - (colW + 20);
			int detailH = uiH - 36;

			GameCanvas.paintz.paintFrameSimple(detailX, detailY, detailW, detailH, g);
			g.setColor(15787715);
			g.fillRect(detailX + 2, detailY + 2, detailW - 4, detailH - 4);

			g.setClip(detailX + 2, detailY + 2, detailW - 4, detailH - 4);

			// Áp dụng trượt ảo cho các tab có nội dung dài
			bool useDetailScroll = (selectedTab == 4 && !ModUIBackground.isOpen);
			if (useDetailScroll)
			{
				g.translate(0, -detailScrollY);
			}

			// Render Sub-Panel tương ứng
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
					if (ModUIBackground.isOpen)
					{
						ModUIBackground.Paint(detailX, detailY, detailW, detailH, g);
					}
					else
					{
						ModUIGraphics.Paint(detailX, detailY, detailW, detailH, g);
					}
					break;
				case 5:
					ModUIBoss.Paint(detailX, detailY, detailW, detailH, g);
					break;
				case 6:
					ModUINextMap.Paint(detailX, detailY, detailW, detailH, g);
					break;
				case 7:
					ModUIGoBack.Paint(detailX, detailY, detailW, detailH, g);
					break;
				case 8:
					ModUISetActivator.Paint(detailX, detailY, detailW, detailH, g);
					break;
				case 9:
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
			// Kiểm tra click vào nút mũi tên Mod Menu
			if (ModArrowButton.CheckClick())
			{
				return;
			}

			int maxW = GameCanvas.w - 12;
			int maxH = GameCanvas.h - 12;
			int uiW = (maxW < 440) ? maxW : 440;
			int uiH = (maxH < 260) ? maxH : 260;
			if (uiW < 320) uiW = GameCanvas.w;
			if (uiH < 220) uiH = GameCanvas.h;
			int uiX = (GameCanvas.w - uiW) / 2;
			int uiY = (GameCanvas.h - uiH) / 2;
			if (uiX < 0) uiX = 0;
			if (uiY < 0) uiY = 0;

			if (uiCustomOpen)
			{
				int px = GameCanvas.px;
				int py = GameCanvas.py;

				int colW = (uiW > 380) ? 96 : 82;
				int colH = uiH - 62;
				int colX = uiX + 8;
				int colY = uiY + 28;
				int closeBtnY = uiY + uiH - 30;

				int detailX = colX + colW + 6;
				int detailY = uiY + 28;
				int detailW = uiW - (colW + 20);
				int detailH = uiH - 36;

				int itemStep = 24;
				int colContentH = tabNames.Length * itemStep + 6;
				int maxColScroll = (colContentH > colH - 4) ? (colContentH - (colH - 4)) : 0;
				int maxDetailScroll = (275 - (detailH - 4) > 0) ? (275 - (detailH - 4)) : 0;

				// Đọc trạng thái chuột & cảm ứng đáng tin cậy
				bool isDown = Input.GetMouseButton(0) || GameCanvas.isPointerDown;
				bool isJustRelease = Input.GetMouseButtonUp(0) || GameCanvas.isPointerJustRelease;

				// 1. Xử lý con lăn chuột (Mouse ScrollWheel)
				float wheel = Input.GetAxis("Mouse ScrollWheel");
				if (wheel == 0 && GameCanvas.pXYScrollMouse != 0)
				{
					wheel = GameCanvas.pXYScrollMouse;
					GameCanvas.pXYScrollMouse = 0;
				}
				if (wheel != 0)
				{
					// Con lăn chuột trên Cột Trái
					if (px >= colX && px <= colX + colW && py >= colY && py <= colY + colH)
					{
						int step = 28;
						if (wheel > 0) colScrollY -= step;
						else if (wheel < 0) colScrollY += step;
						if (colScrollY < 0) colScrollY = 0;
						if (colScrollY > maxColScroll) colScrollY = maxColScroll;
					}
					// Con lăn chuột trên Khung Phải
					else if (px >= detailX && px <= detailX + detailW && py >= detailY && py <= detailY + detailH)
					{
						if (selectedTab == 4 && !ModUIBackground.isOpen)
						{
							int step = 28;
							if (wheel > 0) detailScrollY -= step;
							else if (wheel < 0) detailScrollY += step;
							if (detailScrollY < 0) detailScrollY = 0;
							if (detailScrollY > maxDetailScroll) detailScrollY = maxDetailScroll;
						}
						else if (selectedTab == 4 && ModUIBackground.isOpen)
						{
							ModUIBackground.OnMouseScroll(wheel);
						}
						else if (selectedTab == 9)
						{
							ModUIHelp.OnMouseScroll(wheel);
						}
						else if (selectedTab == 0)
						{
							ModUITanSat.OnMouseScroll(wheel);
						}
					}
				}

				// 2. Chạm kéo trượt trực tiếp (Direct Touch/Drag)
				if (isDown)
				{
					if (!isColDragging && !isDetailDragging)
					{
						if (px >= colX && px <= colX + colW && py >= colY && py <= colY + colH)
						{
							isColDragging = true;
							hasColDragged = false;
							startColDragY = py;
							lastColDragY = py;
						}
						else if (px >= detailX && px <= detailX + detailW && py >= detailY && py <= detailY + detailH)
						{
							if (selectedTab == 4 && !ModUIBackground.isOpen)
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

				// Xử lý kéo thả cho các Sub-Panel chuyên dụng
				if (selectedTab == 9)
				{
					ModUIHelp.UpdateDragScroll(px, py, detailX, detailY, detailW, detailH);
				}
				else if (selectedTab == 0)
				{
					ModUITanSat.UpdateDragScroll(px, py, detailX, detailY, detailW, detailH);
				}
				else if (selectedTab == 4 && ModUIBackground.isOpen)
				{
					ModUIBackground.UpdateDragScroll(px, py, detailX, detailY, detailW, detailH);
				}

				// 3. Xử lý click/chạm khi nhấc tay (Release / Click)
				if (isJustRelease)
				{
					// Nếu đang kéo cột trái
					if (isColDragging)
					{
						isColDragging = false;
						if (!hasColDragged && px >= colX + 2 && px <= colX + colW - 2 && py >= colY + 2 && py <= colY + colH - 2)
						{
							int relY = py - (colY + 3) + colScrollY;
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

					// Nếu đang kéo khung phải (Tab Đồ Họa)
					if (isDetailDragging)
					{
						isDetailDragging = false;
						if (!hasDetailDragged && px >= detailX && px <= detailX + detailW && py >= detailY && py <= detailY + detailH)
						{
							if (ModUIGraphics.HandleTap(px, py + detailScrollY, detailX, detailY, detailW, detailH))
							{
								GameCanvas.isPointerJustRelease = GameCanvas.isPointerClick = false;
								return;
							}
						}
						hasDetailDragged = false;
					}

					// Nút [X] đóng góc phải
					if (px >= uiX + uiW - 32 && px <= uiX + uiW - 4 && py >= uiY + 4 && py <= uiY + 28)
					{
						uiCustomOpen = false;
						ModUIBackground.isOpen = false;
						ModConfig.SaveConfig();
						SoundMn.gI().buttonClose();
						GameCanvas.isPointerJustRelease = GameCanvas.isPointerClick = false;
						return;
					}

					// Nút [ĐÓNG] ở đáy Cột Trái
					if (px >= colX && px <= colX + colW && py >= closeBtnY && py <= closeBtnY + 26)
					{
						uiCustomOpen = false;
						ModUIBackground.isOpen = false;
						ModConfig.SaveConfig();
						SoundMn.gI().buttonClose();
						GameCanvas.isPointerJustRelease = GameCanvas.isPointerClick = false;
						return;
					}

					// Xử lý click trong Vùng Chi Tiết bên phải cho các Tab khác
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
								if (ModUIBackground.isOpen)
								{
									handled = ModUIBackground.HandleTap(px, py, detailX, detailY, detailW, detailH);
								}
								break;
							case 5:
								handled = ModUIBoss.HandleTap(px, py, detailX, detailY, detailW, detailH);
								break;
							case 6:
								handled = ModUINextMap.HandleTap(px, py, detailX, detailY, detailW, detailH);
								break;
							case 7:
								handled = ModUIGoBack.HandleTap(px, py, detailX, detailY, detailW, detailH);
								break;
							case 8:
								handled = ModUISetActivator.HandleTap(px, py, detailX, detailY, detailW, detailH);
								break;
							case 9:
								handled = ModUIHelp.HandleTap(px, py, detailX, detailY, detailW, detailH);
								break;
						}
						if (handled)
						{
							GameCanvas.isPointerJustRelease = GameCanvas.isPointerClick = false;
							return;
						}
					}

					// Chặn sự kiện click xuyên xuống thế giới game
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
