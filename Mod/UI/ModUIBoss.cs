using System;

public static class ModUIBoss
{
	public static void Paint(int uiX, int uiY, int uiW, int uiH, mGraphics g)
	{
		mFont.tahoma_7b_white.drawString(g, "Hiển thị HUD:", uiX + 20, uiY + 54, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 96, uiY + 48, 85, ModBossNotice.isShowBossNotice ? "BẬT" : "TẮT", ModBossNotice.isShowBossNotice, g);
		ModUI.PaintNativeButton(uiX + 190, uiY + 48, 120, "Xóa Danh Sách", false, g);

		mFont.tahoma_7b_yellow.drawString(g, "Boss vừa xuất hiện từ Server (Tối đa 6):", uiX + 20, uiY + 76, mFont.LEFT);

		int listY = uiY + 92;
		int listW = uiW - 36;
		g.setColor(0x111111);
		g.fillRect(uiX + 18, listY, listW, 122);
		g.setColor(0x444444);
		g.drawRect(uiX + 18, listY, listW, 122);

		lock (ModBossNotice.listBossNotices)
		{
			if (ModBossNotice.listBossNotices.Count == 0)
			{
				mFont.tahoma_7_grey.drawString(g, "(Chưa nhận được thông báo boss nào từ server)", uiX + uiW / 2, listY + 52, mFont.CENTER);
			}
			else
			{
				int displayCount = (ModBossNotice.listBossNotices.Count < ModBossNotice.MAX_BOSS_NOTICES) ? ModBossNotice.listBossNotices.Count : ModBossNotice.MAX_BOSS_NOTICES;
				for (int i = 0; i < displayCount; i++)
				{
					ModBossNotice.BossNoticeEntry entry = ModBossNotice.listBossNotices[i];
					if (entry == null) continue;
					int rowY = listY + 4 + i * 19;

					g.setColor(0xff3300);
					g.fillRect(uiX + 22, rowY + 3, 3, 11);

					int curX = uiX + 30;
					if (mFont.tahoma_7b_yellow != null)
					{
						mFont.tahoma_7b_yellow.drawString(g, entry.bossName, curX, rowY + 1, mFont.LEFT);
						curX += mFont.tahoma_7b_yellow.getWidth(entry.bossName);
					}

					if (mFont.tahoma_7_white != null)
					{
						mFont.tahoma_7_white.drawString(g, " - " + entry.mapName + " - ", curX, rowY + 1, mFont.LEFT);
						curX += mFont.tahoma_7_white.getWidth(" - " + entry.mapName + " - ");
					}

					if (mFont.tahoma_7_green2 != null)
					{
						mFont.tahoma_7_green2.drawString(g, entry.timeStr, curX, rowY + 1, mFont.LEFT);
					}
				}
			}
		}
	}

	public static bool HandleTap(int px, int py, int uiX, int uiY, int uiW, int uiH)
	{
		if (px >= uiX + 96 && px <= uiX + 181 && py >= uiY + 48 && py <= uiY + 70)
		{
			ModBossNotice.isShowBossNotice = !ModBossNotice.isShowBossNotice;
			ModConfig.SaveConfig();
			SoundMn.gI().buttonClick();
			return true;
		}

		if (px >= uiX + 190 && px <= uiX + 310 && py >= uiY + 48 && py <= uiY + 70)
		{
			lock (ModBossNotice.listBossNotices)
			{
				ModBossNotice.listBossNotices.Clear();
			}
			SoundMn.gI().buttonClick();
			return true;
		}

		return false;
	}
}
