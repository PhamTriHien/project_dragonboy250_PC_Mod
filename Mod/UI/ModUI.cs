using System;
using System.Collections.Generic;

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
		Char me = Char.myCharz();
		if (me != null && me.vSkill != null)
		{
			for (int i = 0; i < me.vSkill.size(); i++)
			{
				Skill s = (Skill)me.vSkill.elementAt(i);
				if (s != null && s.template != null)
				{
					int tId = s.template.id;
					if (tId != 7 && tId != 8 && tId != 9 && tId != 10 && tId != 14 && tId != 19 && tId != 21 && tId != 22 && tId != 23)
					{
						list.Add(s);
					}
				}
			}
		}
		return list;
	}

	public static void DrawCheckbox(int bx, int by, bool isChecked, mGraphics g)
	{
		g.setColor(0x333333);
		g.fillRect(bx, by, 14, 14);
		g.setColor(0x888888);
		g.drawRect(bx, by, 14, 14);
		if (isChecked)
		{
			g.setColor(0x00e676);
			g.fillRect(bx + 2, by + 2, 10, 10);
		}
	}

	public static void PaintNativeButton(int x, int y, int w, int h, string text, bool isFocus, mGraphics g)
	{
		try
		{
			if (isFocus)
			{
				g.setColor(0x13381b);
				g.fillRect(x + 1, y + 1, w - 2, h - 2);
				g.setColor(0x00e676);
				g.drawRect(x, y, w - 1, h - 1);
				g.setColor(0x00783e);
				g.drawRect(x + 1, y + 1, w - 3, h - 3);
			}
			else
			{
				g.setColor(0x242424);
				g.fillRect(x + 1, y + 1, w - 2, h - 2);
				g.setColor(0x555555);
				g.drawRect(x, y, w - 1, h - 1);
				g.setColor(0x181818);
				g.drawRect(x + 1, y + 1, w - 3, h - 3);
			}

			int textY = y + (h - 10) / 2;
			(isFocus ? mFont.tahoma_7b_green2 : mFont.tahoma_7b_white).drawString(g, text, x + w / 2, textY, mFont.CENTER);
		}
		catch
		{
		}
	}

	public static void PaintNativeButton(int x, int y, int w, string text, bool isFocus, mGraphics g)
	{
		PaintNativeButton(x, y, w, 20, text, isFocus, g);
	}

	public static void PaintTanSatUI(mGraphics g)
	{
		if (!uiCustomOpen)
		{
			return;
		}
		try
		{
			int uiW = 340;
			int uiH = 250;
			int uiX = (GameCanvas.w - uiW) / 2;
			int uiY = (GameCanvas.h - uiH) / 2;

			GameCanvas.paintz.paintFrame(uiX, uiY, uiW, uiH, g);

			string title = "CÀI ĐẶT TÀN SÁT";
			if (selectedTab == 1) title = "CÀI ĐẶT TỰ NHẶT";
			else if (selectedTab == 2) title = "CÀI ĐẶT TỐC ĐỘ CHẠY";
			else if (selectedTab == 3) title = "CÀI ĐẶT BƠM ĐẬU & HP";
			else if (selectedTab == 4) title = "CÀI ĐẶT ĐỒ HỌA & FPS";
			else if (selectedTab == 5) title = "CÀI ĐẶT THÔNG BÁO BOSS";
			else if (selectedTab == 6) title = "TỰ ĐỘNG QUA MAP (NEXT MAP)";

			mFont.tahoma_7b_yellow.drawString(g, title, uiX + uiW / 2, uiY + 10, mFont.CENTER);

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

			// 7 Tab Buttons Header (Nhỏ gọn, tinh tế)
			string[] tabNames = new string[7] { "Tàn Sát", "Tự Nhặt", "Tốc Độ", "Hồi Máu", "Đồ Họa", "Báo Boss", "Next Map" };
			int tabW = 42;
			int tabH = 19;
			int startTabX = uiX + 11;
			for (int t = 0; t < 7; t++)
			{
				int tx = startTabX + t * 45;
				PaintNativeButton(tx, uiY + 28, tabW, tabH, tabNames[t], selectedTab == t, g);
			}

			// Render Tab tương ứng
			switch (selectedTab)
			{
				case 0:
					ModUITanSat.Paint(uiX, uiY, uiW, uiH, g);
					break;
				case 1:
					ModUIAutoPick.Paint(uiX, uiY, uiW, uiH, g);
					break;
				case 2:
					ModUISpeed.Paint(uiX, uiY, uiW, uiH, g);
					break;
				case 3:
					ModUIAutoHeal.Paint(uiX, uiY, uiW, uiH, g);
					break;
				case 4:
					ModUIGraphics.Paint(uiX, uiY, uiW, uiH, g);
					break;
				case 5:
					ModUIBoss.Paint(uiX, uiY, uiW, uiH, g);
					break;
				case 6:
					ModUINextMap.Paint(uiX, uiY, uiW, uiH, g);
					break;
			}

			// Nút ĐÓNG nhỏ gọn ở đáy
			int closeBtnW = 75;
			int closeBtnH = 20;
			int closeBtnX = uiX + (uiW - closeBtnW) / 2;
			int closeBtnY = uiY + 222;
			PaintNativeButton(closeBtnX, closeBtnY, closeBtnW, closeBtnH, "ĐÓNG", false, g);
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

			int uiW = 340;
			int uiH = 250;
			int uiX = (GameCanvas.w - uiW) / 2;
			int uiY = (GameCanvas.h - uiH) / 2;

			if (uiCustomOpen)
			{
				int px = GameCanvas.px;
				int py = GameCanvas.py;
				bool isClick = GameCanvas.isPointerClick || GameCanvas.isPointerJustRelease;

				if (px >= uiX && px <= uiX + uiW && py >= uiY && py <= uiY + uiH)
				{
					GameCanvas.isPointerClick = (GameCanvas.isPointerJustDown = (GameCanvas.isPointerJustRelease = false));
					GameCanvas.isPointerDown = false;

					if (isClick)
					{
						// Nút [X] đóng ở góc phải
						if (px >= uiX + uiW - 32 && px <= uiX + uiW - 4 && py >= uiY + 4 && py <= uiY + 28)
						{
							uiCustomOpen = false;
							ModConfig.SaveConfig();
							SoundMn.gI().buttonClose();
							return;
						}

						// Chuyển 7 Tab chính
						if (py >= uiY + 26 && py <= uiY + 50)
						{
							int startTabX = uiX + 11;
							int tabW = 41;
							for (int t = 0; t < 7; t++)
							{
								int tx = startTabX + t * 45;
								if (px >= tx && px <= tx + tabW)
								{
									selectedTab = t;
									ModConfig.SaveConfig();
									SoundMn.gI().buttonClick();
									return;
								}
							}
						}

						// Xử lý click theo Tab hiện tại
						bool handled = false;
						switch (selectedTab)
						{
							case 0:
								handled = ModUITanSat.HandleTap(px, py, uiX, uiY, uiW, uiH);
								break;
							case 1:
								handled = ModUIAutoPick.HandleTap(px, py, uiX, uiY, uiW, uiH);
								break;
							case 2:
								handled = ModUISpeed.HandleTap(px, py, uiX, uiY, uiW, uiH);
								break;
							case 3:
								handled = ModUIAutoHeal.HandleTap(px, py, uiX, uiY, uiW, uiH);
								break;
							case 4:
								handled = ModUIGraphics.HandleTap(px, py, uiX, uiY, uiW, uiH);
								break;
							case 5:
								handled = ModUIBoss.HandleTap(px, py, uiX, uiY, uiW, uiH);
								break;
							case 6:
								handled = ModUINextMap.HandleTap(px, py, uiX, uiY, uiW, uiH);
								break;
						}
						if (handled)
						{
							return;
						}

						// Nút ĐÓNG ở đáy
						int closeBtnW = 75;
						int closeBtnX = uiX + (uiW - closeBtnW) / 2;
						int closeBtnY = uiY + 222;
						if (px >= closeBtnX && px <= closeBtnX + closeBtnW && py >= closeBtnY && py <= closeBtnY + 20)
						{
							uiCustomOpen = false;
							ModConfig.SaveConfig();
							SoundMn.gI().buttonClose();
							return;
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
