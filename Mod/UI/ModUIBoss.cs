using System;

public static class ModUIBoss
{
	public static void Paint(int uiX, int uiY, int uiW, int uiH, mGraphics g)
	{
		mFont.tahoma_7b_white.drawString(g, "Hiển thị HUD:", uiX + 20, uiY + 52, mFont.LEFT);
		ModUI.PaintNativeButton(uiX + 96, uiY + 48, 52, 18, ModBossNotice.isShowBossNotice ? "BẬT" : "TẮT", ModBossNotice.isShowBossNotice, g);
		ModUI.PaintNativeButton(uiX + 158, uiY + 48, 95, 18, "Xóa Danh Sách", false, g);

		mFont.tahoma_7b_yellow.drawString(g, "Boss vừa xuất hiện từ Server (Tối đa 6):", uiX + 20, uiY + 74, mFont.LEFT);

		int listY = uiY + 90;
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

					g.setColor(entry.isDefeated ? 0x888888 : 0xff3300);
					g.fillRect(uiX + 22, rowY + 3, 3, 11);

					int curX = uiX + 30;
					if (mFont.tahoma_7b_yellow != null)
					{
						(entry.isDefeated ? mFont.tahoma_7_grey : mFont.tahoma_7b_yellow).drawString(g, entry.bossName, curX, rowY + 1, mFont.LEFT);
						curX += (entry.isDefeated ? mFont.tahoma_7_grey : mFont.tahoma_7b_yellow).getWidth(entry.bossName);
					}

					if (mFont.tahoma_7_white != null)
					{
						(entry.isDefeated ? mFont.tahoma_7_grey : mFont.tahoma_7_white).drawString(g, " - " + entry.mapName + " - ", curX, rowY + 1, mFont.LEFT);
						curX += (entry.isDefeated ? mFont.tahoma_7_grey : mFont.tahoma_7_white).getWidth(" - " + entry.mapName + " - ");
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
		if (px >= uiX + 96 && px <= uiX + 148 && py >= uiY + 48 && py <= uiY + 66)
		{
			ModBossNotice.isShowBossNotice = !ModBossNotice.isShowBossNotice;
			ModConfig.SaveConfig();
			SoundMn.gI().buttonClick();
			return true;
		}

		if (px >= uiX + 158 && px <= uiX + 253 && py >= uiY + 48 && py <= uiY + 66)
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
