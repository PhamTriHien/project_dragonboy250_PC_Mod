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

	public static int colScrollY = 0;
	private static bool isColDragging = false;
	private static bool hasColDragged = false;
	private static int startColDragY = 0;
	private static int startColScrollY = 0;

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

		// 1. Quet toan bo ky nang dang trang bi tren phim tat PC (GameScr.keySkill)
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

		// 2. Quet tiep toan bo ky nang dang trang bi tren o man hinh / cam ung (GameScr.onScreenSkill)
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

		// 3. Quet ky nang dang duoc chon hien tai (Char.myCharz().myskill)
		if (me != null && me.myskill != null && me.myskill.template != null && !addedIds.Contains(me.myskill.template.id))
		{
			list.Add(me.myskill);
			addedIds.Add(me.myskill.template.id);
		}

		// 4. Quet danh sach ky nang chien dau cua nhan vat (Char.myCharz().vSkillFight)
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

		// 5. Quet toan bo danh sach ky nang da hoc (Char.myCharz().vSkill)
		if (me != null && me.vSkill != null)
		{
			for (int m = 0; m < me.vSkill.size(); m++)
			{
				Skill s4 = (Skill)me.vSkill.elementAt(m);
				if (s4 != null && s4.template != null && (s4.template.maxPoint == 0 || s4.point > 0) && !addedIds.Contains(s4.template.id))
				{
					list.Add(s4);
					addedIds.Add(s4.template.id);
				}
			}
		}

		return list;
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

				bool needClip = (h < 24);
				if (needClip)
				{
					g.setClip(x, y, w, h);
				}

				if (w >= 20)
				{
					Command.paintOngMau(bLeft, bMid, bRight, x, y, w, g);
				}
				else
				{
					g.drawRegion(bLeft, 0, 0, w / 2, 24, 0, x, y, 0);
					g.drawRegion(bRight, 10 - (w - w / 2), 0, w - w / 2, 24, 0, x + w / 2, y, 0);
				}

				if (needClip)
				{
					g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
				}
			}
			else
			{
				// Fallback dùng màu be/nâu truyền thống NRO
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
			int uiW = 440;
			int uiH = 260;
			int uiX = (GameCanvas.w - uiW) / 2;
			int uiY = (GameCanvas.h - uiH) / 2;

			// 1. Khung Dialog chính: Dùng asset viền chuẩn NRO + nền giấy ngà truyền thống
			GameCanvas.paintz.paintFrame(uiX, uiY, uiW, uiH, g);
			GameCanvas.paintz.paintFrameInside(uiX + 6, uiY + 6, uiW - 12, uiH - 12, g);

			// Tiêu đề
			string currentTabName = (selectedTab >= 0 && selectedTab < tabNames.Length) ? tabNames[selectedTab] : "CÀI ĐẶT";
			string title = "MENU MOD - " + currentTabName.ToUpper();
			mFont.tahoma_7b_dark.drawString(g, title, uiX + uiW / 2, uiY + 9, mFont.CENTER);

			// Nút [X] đóng ở góc phải
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

			// 2. Cột Danh Mục Bên Trái (Master Navigation Sidebar - Cuộn Dọc Không Giới Hạn)
			int colX = uiX + 8;
			int colY = uiY + 28;
			int colW = 98;
			int colH = 196;

			GameCanvas.paintz.paintFrameSimple(colX, colY, colW, colH, g);
			g.setColor(15196114);
			g.fillRect(colX + 2, colY + 2, colW - 4, colH - 4);

			int itemH = 22;
			int itemStep = 24;
			int contentH = tabNames.Length * itemStep + 4;
			int maxScroll = (contentH > colH - 4) ? (contentH - (colH - 4)) : 0;
			if (colScrollY > maxScroll) colScrollY = maxScroll;
			if (colScrollY < 0) colScrollY = 0;

			g.setClip(colX + 2, colY + 2, colW - 4, colH - 4);
			for (int t = 0; t < tabNames.Length; t++)
			{
				int btnY = colY + 3 + t * itemStep - colScrollY;
				if (btnY + itemH < colY || btnY > colY + colH) continue;
				bool isSel = (selectedTab == t);
				PaintNativeButton(colX + 4, btnY, colW - 8, itemH, tabNames[t], isSel, g);
			}
			g.setClip(0, 0, GameCanvas.w, GameCanvas.h);

			// Thanh cuộn NRO thanh mảnh ở mép phải Cột Danh Mục
			if (maxScroll > 0)
			{
				int barH = colH * (colH - 4) / contentH;
				if (barH < 14) barH = 14;
				int barY = colY + 2 + (colH - 4 - barH) * colScrollY / maxScroll;
				g.setColor(3847752);
				g.fillRect(colX + colW - 4, barY, 2, barH);
			}

			// Nút ĐÓNG ống màu cố định ở đáy Cột Trái
			int closeBtnY = uiY + 228;
			PaintNativeButton(colX, closeBtnY, colW, 22, "ĐÓNG", false, g);

			// 3. Vùng Nội Dung Chi Tiết Bên Phải (Detail Panel)
			int detailX = uiX + 112;
			int detailY = uiY + 28;
			int detailW = 320;
			int detailH = 222;

			GameCanvas.paintz.paintFrameSimple(detailX, detailY, detailW, detailH, g);
			g.setColor(15787715);
			g.fillRect(detailX + 2, detailY + 2, detailW - 4, detailH - 4);

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
		}
		catch
		{
		}
	}

	public static void HandleTap()
	{
		if (!ModMenu.IsInGame())
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

			int uiW = 440;
			int uiH = 260;
			int uiX = (GameCanvas.w - uiW) / 2;
			int uiY = (GameCanvas.h - uiH) / 2;

			if (uiCustomOpen)
			{
				int px = GameCanvas.px;
				int py = GameCanvas.py;

				int colX = uiX + 8;
				int colY = uiY + 28;
				int colW = 98;
				int colH = 196;

				int detailX = uiX + 112;
				int detailY = uiY + 28;
				int detailW = 320;
				int detailH = 222;

				int itemStep = 24;
				int contentH = tabNames.Length * itemStep + 4;
				int maxScroll = (contentH > colH - 4) ? (contentH - (colH - 4)) : 0;

				// 1. Xử lý con lăn chuột (Mouse ScrollWheel)
				float wheel = Input.GetAxis("Mouse ScrollWheel");
				if (wheel == 0 && GameCanvas.pXYScrollMouse != 0)
				{
					wheel = GameCanvas.pXYScrollMouse;
					GameCanvas.pXYScrollMouse = 0;
				}
				if (wheel != 0)
				{
					// Nếu con trỏ chuột ở Cột Danh Mục bên trái
					if (px >= colX && px <= colX + colW && py >= colY && py <= colY + colH)
					{
						int step = 28;
						if (wheel > 0)
						{
							colScrollY -= step;
							if (colScrollY < 0) colScrollY = 0;
						}
						else if (wheel < 0)
						{
							colScrollY += step;
							if (colScrollY > maxScroll) colScrollY = maxScroll;
						}
					}
					// Nếu con trỏ chuột ở Vùng Chi Tiết bên phải
					else if (px >= detailX && px <= detailX + detailW && py >= detailY && py <= detailY + detailH)
					{
						if (selectedTab == 9)
						{
							ModUIHelp.OnMouseScroll(wheel);
						}
						else if (selectedTab == 0)
						{
							ModUITanSat.OnMouseScroll(wheel);
						}
						else if (selectedTab == 4 && ModUIBackground.isOpen)
						{
							ModUIBackground.OnMouseScroll(wheel);
						}
					}
				}

				// 2. Xử lý kéo thả cuộn Cột Danh Mục (Drag Scroll Left Sidebar)
				if (GameCanvas.isPointerJustDown)
				{
					if (px >= colX && px <= colX + colW && py >= colY && py <= colY + colH)
					{
						isColDragging = true;
						hasColDragged = false;
						startColDragY = py;
						startColScrollY = colScrollY;
					}
				}
				else if (GameCanvas.isPointerDown && isColDragging)
				{
					int deltaY = py - startColDragY;
					if (Res.abs(deltaY) > 4)
					{
						hasColDragged = true;
					}
					if (hasColDragged && maxScroll > 0)
					{
						colScrollY = startColScrollY - deltaY;
						if (colScrollY < 0) colScrollY = 0;
						if (colScrollY > maxScroll) colScrollY = maxScroll;
					}
				}
				else if (GameCanvas.isPointerJustRelease)
				{
					isColDragging = false;
				}

				// Xử lý kéo thả cuộn trong Sub-Panel
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

				bool isClick = GameCanvas.isPointerClick || GameCanvas.isPointerJustRelease;

				if (px >= uiX && px <= uiX + uiW && py >= uiY && py <= uiY + uiH)
				{
					GameCanvas.isPointerClick = (GameCanvas.isPointerJustDown = (GameCanvas.isPointerJustRelease = false));
					GameCanvas.isPointerDown = false;

					if (isClick)
					{
						// Nút [X] đóng ở góc phải trên cùng
						if (px >= uiX + uiW - 32 && px <= uiX + uiW - 4 && py >= uiY + 4 && py <= uiY + 28)
						{
							uiCustomOpen = false;
							ModUIBackground.isOpen = false;
							ModConfig.SaveConfig();
							SoundMn.gI().buttonClose();
							return;
						}

						// Nút [ĐÓNG] ở đáy Cột Trái
						if (px >= colX && px <= colX + colW && py >= uiY + 228 && py <= uiY + 252)
						{
							uiCustomOpen = false;
							ModUIBackground.isOpen = false;
							ModConfig.SaveConfig();
							SoundMn.gI().buttonClose();
							return;
						}

						// Bấm chọn danh mục ở Cột Trái
						if (!hasColDragged && px >= colX + 4 && px <= colX + colW - 4 && py >= colY + 2 && py <= colY + colH - 2)
						{
							int clickedIdx = (py - (colY + 3) + colScrollY) / itemStep;
							if (clickedIdx >= 0 && clickedIdx < tabNames.Length)
							{
								selectedTab = clickedIdx;
								ModUIBackground.isOpen = false;
								ModConfig.SaveConfig();
								SoundMn.gI().buttonClick();
								return;
							}
						}
						if (hasColDragged)
						{
							hasColDragged = false;
						}

						// Xử lý click trong Vùng Chi Tiết bên phải
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
									else
									{
										handled = ModUIGraphics.HandleTap(px, py, detailX, detailY, detailW, detailH);
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
								return;
							}
						}
					}
				}
				else if (isClick)
				{
					GameCanvas.clearAllPointerEvent();
				}
			}
		}
		catch
		{
		}
	}
}
