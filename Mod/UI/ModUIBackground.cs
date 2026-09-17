using System;
using UnityEngine;

public static class ModUIBackground
{
	public static bool isOpen = false;
	public static int scrollY = 0;

	private static bool isDragging = false;
	private static bool hasDragged = false;
	private static int startDragY = 0;
	private static int lastDragY = 0;

	public static void OnMouseScroll(float wheel)
	{
		int listH = 194;
		int cardStep = 48;
		int contentH = ModBackground.bgList.Count * cardStep + 4;
		int maxScroll = (contentH > listH - 4) ? (contentH - (listH - 4)) : 0;
		if (maxScroll <= 0) return;

		int step = 26;
		if (wheel > 0)
		{
			scrollY -= step;
			if (scrollY < 0) scrollY = 0;
		}
		else if (wheel < 0)
		{
			scrollY += step;
			if (scrollY > maxScroll) scrollY = maxScroll;
		}
	}

	public static void UpdateDragScroll(int px, int py, int detailX, int detailY, int detailW, int detailH)
	{
		int listX = detailX + 4;
		int listY = detailY + 22;
		int listW = detailW - 8;
		int listH = detailH - 26;

		int cardStep = 48;
		int contentH = ModBackground.bgList.Count * cardStep + 4;
		int maxScroll = (contentH > listH - 4) ? (contentH - (listH - 4)) : 0;

		bool isDown = Input.GetMouseButton(0) || GameCanvas.isPointerDown;

		if (isDown)
		{
			if (!isDragging)
			{
				if (px >= listX && px <= listX + listW && py >= listY && py <= listY + listH)
				{
					isDragging = true;
					hasDragged = false;
					startDragY = py;
					lastDragY = py;
				}
			}
			else
			{
				int moveY = py - lastDragY;
				lastDragY = py;
				if (Res.abs(py - startDragY) > 5)
				{
					hasDragged = true;
				}
				if (hasDragged && maxScroll > 0)
				{
					scrollY -= moveY;
					if (scrollY < 0) scrollY = 0;
					if (scrollY > maxScroll) scrollY = maxScroll;
				}
			}
		}
		else
		{
			isDragging = false;
		}
	}

	public static void Paint(int detailX, int detailY, int detailW, int detailH, mGraphics g)
	{
		// 1. Header: Tiêu đề + Nút Mặc Định + Nút Quay Lại
		mFont.tahoma_7_green2.drawString(g, "PHONG CẢNH NỀN GAME (GIT)", detailX + 5, detailY + 5, mFont.LEFT);

		int defW = 62;
		int defX = detailX + detailW - defW - 60;
		ModUI.PaintNativeButton(defX, detailY + 2, defW, 16, "MẶC ĐỊNH", !ModBackground.isCustomBGActive, g);

		int backW = 56;
		int backX = detailX + detailW - backW - 4;
		ModUI.PaintNativeButton(backX, detailY + 2, backW, 16, "<< TRỞ VỀ", false, g);

		// 2. Vùng danh sách các hình nền
		int listX = detailX + 4;
		int listY = detailY + 22;
		int listW = detailW - 8;
		int listH = detailH - 26;

		GameCanvas.paintz.paintFrameSimple(listX, listY, listW, listH, g);
		g.setColor(15196114);
		g.fillRect(listX + 2, listY + 2, listW - 4, listH - 4);

		int cardH = 42;
		int cardStep = 48;
		int contentH = ModBackground.bgList.Count * cardStep + 4;
		int maxScroll = (contentH > listH - 4) ? (contentH - (listH - 4)) : 0;
		if (scrollY > maxScroll) scrollY = maxScroll;
		if (scrollY < 0) scrollY = 0;

		g.setClip(listX + 2, listY + 2, listW - 4, listH - 4);

		for (int i = 0; i < ModBackground.bgList.Count; i++)
		{
			BackgroundItem item = ModBackground.bgList[i];
			int cardX = listX + 4;
			int cardY = listY + 3 + i * cardStep - scrollY;
			int cardW = listW - 8;

			if (cardY + cardH < listY || cardY > listY + listH)
			{
				continue;
			}

			bool isCurrentActive = ModBackground.isCustomBGActive && ModBackground.selectedBgId.Equals(item.id, StringComparison.OrdinalIgnoreCase);

			// Khung thẻ
			g.setColor(isCurrentActive ? 0xD4EDFF : 0xEEEEEE);
			g.fillRect(cardX, cardY, cardW, cardH);
			g.setColor(isCurrentActive ? 0x00783e : 0xBBBBBB);
			g.drawRect(cardX, cardY, cardW, cardH);

			// Dải màu trời đặc trưng của theme
			g.setColor(item.topColor);
			g.fillRect(cardX + 3, cardY + 3, 10, cardH - 6);
			g.setColor(0x333333);
			g.drawRect(cardX + 3, cardY + 3, 10, cardH - 6);

			// Thông tin văn bản
			int textX = cardX + 18;
			(isCurrentActive ? mFont.tahoma_7_green2 : mFont.tahoma_7_grey).drawString(g, item.name, textX, cardY + 3, mFont.LEFT);
			int nameW = mFont.tahoma_7_grey.getWidth(item.name);
			mFont.tahoma_7_grey.drawString(g, "(" + item.size + ")", textX + nameW + 4, cardY + 3, mFont.LEFT);

			// Dòng trạng thái
			if (item.isDownloading)
			{
				(mFont.tahoma_7_blue ?? mFont.tahoma_7_white).drawString(g, "Đang tải: " + item.downloadPercent + "%", textX, cardY + 15, mFont.LEFT);
			}
			else if (isCurrentActive)
			{
				mFont.tahoma_7_green2.drawString(g, "● Đang kích hoạt làm nền game", textX, cardY + 15, mFont.LEFT);
			}
			else if (item.isDownloaded)
			{
				mFont.tahoma_7_grey.drawString(g, "Đã tải về máy - sẵn sàng dùng", textX, cardY + 15, mFont.LEFT);
			}
			else
			{
				mFont.tahoma_7_grey.drawString(g, "Lưu trữ trên GitHub (Chưa tải)", textX, cardY + 15, mFont.LEFT);
			}

			// Mô tả chủ đề
			mFont.tahoma_7_grey.drawString(g, item.desc, textX, cardY + 27, mFont.LEFT);

			// Nút thao tác bên phải thẻ
			int btnW = 56;
			int btnX = cardX + cardW - btnW - 4;

			if (item.isDownloading)
			{
				ModUI.PaintNativeButton(btnX, cardY + 13, btnW, 16, item.downloadPercent + "%", false, g);
			}
			else if (!item.isDownloaded)
			{
				ModUI.PaintNativeButton(btnX, cardY + 13, btnW, 16, "TẢI VỀ", false, g);
			}
			else
			{
				if (isCurrentActive)
				{
					ModUI.PaintNativeButton(btnX, cardY + 3, btnW, 16, "ĐANG DÙNG", true, g);
				}
				else
				{
					ModUI.PaintNativeButton(btnX, cardY + 3, btnW, 16, "ÁP DỤNG", false, g);
				}
				ModUI.PaintNativeButton(btnX, cardY + 22, btnW, 16, "XÓA", false, g);
			}
		}

		g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
	}

	public static bool HandleTap(int px, int py, int detailX, int detailY, int detailW, int detailH)
	{
		if (hasDragged)
		{
			hasDragged = false;
			return false;
		}

		// 1. Nút << TRỞ VỀ
		int backW = 56;
		int backX = detailX + detailW - backW - 4;
		if (px >= backX && px <= backX + backW && py >= detailY + 2 && py <= detailY + 20)
		{
			isOpen = false;
			ModUI.detailScrollY = 0;
			SoundMn.gI().buttonClick();
			return true;
		}

		// 2. Nút MẶC ĐỊNH
		int defW = 62;
		int defX = detailX + detailW - defW - 60;
		if (px >= defX && px <= defX + defW && py >= detailY + 2 && py <= detailY + 20)
		{
			ModBackground.ResetToDefault(saveConfig: true);
			SoundMn.gI().buttonClick();
			return true;
		}

		// 3. Tương tác trên thẻ danh sách
		int listX = detailX + 4;
		int listY = detailY + 22;
		int listW = detailW - 8;
		int listH = detailH - 26;

		if (px < listX || px > listX + listW || py < listY || py > listY + listH)
		{
			return false;
		}

		int cardH = 42;
		int cardStep = 48;

		for (int i = 0; i < ModBackground.bgList.Count; i++)
		{
			BackgroundItem item = ModBackground.bgList[i];
			int cardX = listX + 4;
			int cardY = listY + 3 + i * cardStep - scrollY;
			int cardW = listW - 8;

			if (cardY + cardH < listY || cardY > listY + listH)
			{
				continue;
			}

			int btnW = 56;
			int btnX = cardX + cardW - btnW - 4;

			if (item.isDownloading)
			{
				continue;
			}

			if (!item.isDownloaded)
			{
				// Nút TẢI VỀ
				if (px >= btnX && px <= btnX + btnW && py >= cardY + 12 && py <= cardY + 30)
				{
					ModBackground.DownloadBackgroundAsync(item);
					SoundMn.gI().buttonClick();
					return true;
				}
			}
			else
			{
				// Nút ÁP DỤNG
				if (px >= btnX && px <= btnX + btnW && py >= cardY + 2 && py <= cardY + 20)
				{
					ModBackground.ApplyBackground(item.id, saveConfig: true);
					SoundMn.gI().buttonClick();
					return true;
				}

				// Nút XÓA
				if (px >= btnX && px <= btnX + btnW && py >= cardY + 21 && py <= cardY + 39)
				{
					ModBackground.DeleteBackground(item.id);
					SoundMn.gI().buttonClick();
					return true;
				}
			}
		}

		return false;
	}
}
