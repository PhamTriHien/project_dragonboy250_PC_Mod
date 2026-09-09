using System;
using UnityEngine;

public static class ModUIBackground
{
	public static bool isOpen = false;
	public static int scrollY = 0;

	private static bool isDragging = false;
	private static bool hasDragged = false;
	private static int startDragY = 0;
	private static int startScrollY = 0;

	public static void OnMouseScroll(float wheel)
	{
		int listH = 194;
		int cardStep = 52;
		int contentH = ModBackground.bgList.Count * cardStep + 6;
		int maxScroll = (contentH > listH - 6) ? (contentH - (listH - 6)) : 0;
		if (maxScroll <= 0) return;

		int step = 28;
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
		int listY = detailY + 24;
		int listW = detailW - 8;
		int listH = detailH - 28;

		int cardStep = 52;
		int contentH = ModBackground.bgList.Count * cardStep + 6;
		int maxScroll = (contentH > listH - 6) ? (contentH - (listH - 6)) : 0;

		bool isDown = Input.GetMouseButton(0) || GameCanvas.isPointerDown;
		bool isJustDown = Input.GetMouseButtonDown(0) || GameCanvas.isPointerJustDown;
		bool isJustRelease = Input.GetMouseButtonUp(0) || GameCanvas.isPointerJustRelease;

		if (isJustDown)
		{
			if (px >= listX && px <= listX + listW && py >= listY && py <= listY + listH)
			{
				isDragging = true;
				hasDragged = false;
				startDragY = py;
				startScrollY = scrollY;
			}
		}
		else if (isDown && isDragging)
		{
			int deltaY = py - startDragY;
			if (Res.abs(deltaY) > 4)
			{
				hasDragged = true;
			}
			if (hasDragged && maxScroll > 0)
			{
				scrollY = startScrollY - deltaY;
				if (scrollY < 0) scrollY = 0;
				if (scrollY > maxScroll) scrollY = maxScroll;
			}
		}
		else if (isJustRelease)
		{
			isDragging = false;
		}
	}

	public static void Paint(int detailX, int detailY, int detailW, int detailH, mGraphics g)
	{
		// 1. Header: Tiêu đề + Nút Mặc Định + Nút Quay Lại
		mFont.tahoma_7b_dark.drawString(g, "PHONG CẢNH NỀN GAME (GIT)", detailX + 6, detailY + 6, mFont.LEFT);

		int defW = 68;
		int defX = detailX + detailW - defW - 68;
		ModUI.PaintNativeButton(defX, detailY + 3, defW, 18, "MẶC ĐỊNH", !ModBackground.isCustomBGActive, g);

		int backW = 62;
		int backX = detailX + detailW - backW - 4;
		ModUI.PaintNativeButton(backX, detailY + 3, backW, 18, "<< TRỞ VỀ", false, g);

		// 2. Vùng danh sách các hình nền
		int listX = detailX + 4;
		int listY = detailY + 24;
		int listW = detailW - 8;
		int listH = detailH - 28;

		GameCanvas.paintz.paintFrameSimple(listX, listY, listW, listH, g);
		g.setColor(15196114);
		g.fillRect(listX + 2, listY + 2, listW - 4, listH - 4);

		int cardH = 46;
		int cardStep = 52;
		int contentH = ModBackground.bgList.Count * cardStep + 6;
		int maxScroll = (contentH > listH - 6) ? (contentH - (listH - 6)) : 0;
		if (scrollY > maxScroll) scrollY = maxScroll;
		if (scrollY < 0) scrollY = 0;

		g.setClip(listX + 2, listY + 2, listW - 4, listH - 4);

		for (int i = 0; i < ModBackground.bgList.Count; i++)
		{
			BackgroundItem item = ModBackground.bgList[i];
			int cardX = listX + 4;
			int cardY = listY + 4 + i * cardStep - scrollY;
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
			g.fillRect(cardX + 4, cardY + 4, 12, cardH - 8);
			g.setColor(0x333333);
			g.drawRect(cardX + 4, cardY + 4, 12, cardH - 8);

			// Thông tin văn bản
			int textX = cardX + 22;
			mFont.tahoma_7b_dark.drawString(g, item.name, textX, cardY + 4, mFont.LEFT);
			int nameW = mFont.tahoma_7b_dark.getWidth(item.name);
			mFont.tahoma_7_grey.drawString(g, "(" + item.size + ")", textX + nameW + 5, cardY + 4, mFont.LEFT);

			// Dòng trạng thái
			if (item.isDownloading)
			{
				mFont.tahoma_7b_blue.drawString(g, "Đang tải từ Git: " + item.downloadPercent + "%", textX, cardY + 17, mFont.LEFT);
			}
			else if (isCurrentActive)
			{
				mFont.tahoma_7b_green2.drawString(g, "● Đang kích hoạt làm nền game", textX, cardY + 17, mFont.LEFT);
			}
			else if (item.isDownloaded)
			{
				mFont.tahoma_7_grey.drawString(g, "Đã tải về máy - sẵn sàng dùng", textX, cardY + 17, mFont.LEFT);
			}
			else
			{
				mFont.tahoma_7_grey.drawString(g, "Lưu trữ trên GitHub (Chưa tải)", textX, cardY + 17, mFont.LEFT);
			}

			// Mô tả chủ đề
			mFont.tahoma_7_grey.drawString(g, item.desc, textX, cardY + 30, mFont.LEFT);

			// Nút thao tác bên phải thẻ
			int btnW = 62;
			int btnX = cardX + cardW - btnW - 4;

			if (item.isDownloading)
			{
				ModUI.PaintNativeButton(btnX, cardY + 14, btnW, 18, item.downloadPercent + "%", false, g);
			}
			else if (!item.isDownloaded)
			{
				ModUI.PaintNativeButton(btnX, cardY + 14, btnW, 18, "TẢI VỀ", false, g);
			}
			else
			{
				if (isCurrentActive)
				{
					ModUI.PaintNativeButton(btnX, cardY + 4, btnW, 18, "ĐANG DÙNG", true, g);
				}
				else
				{
					ModUI.PaintNativeButton(btnX, cardY + 4, btnW, 18, "ÁP DỤNG", false, g);
				}
				ModUI.PaintNativeButton(btnX, cardY + 24, btnW, 18, "XÓA TỆP", false, g);
			}
		}

		g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
	}

	public static bool HandleTap(int px, int py, int detailX, int detailY, int detailW, int detailH)
	{
		// Nếu vừa thực hiện thao tác kéo trượt thì bỏ qua click
		if (hasDragged)
		{
			hasDragged = false;
			return false;
		}

		// 1. Nút << TRỞ VỀ
		int backW = 62;
		int backX = detailX + detailW - backW - 4;
		if (px >= backX && px <= backX + backW && py >= detailY + 3 && py <= detailY + 21)
		{
			isOpen = false;
			ModUI.detailScrollY = 0;
			SoundMn.gI().buttonClick();
			return true;
		}

		// 2. Nút MẶC ĐỊNH
		int defW = 68;
		int defX = detailX + detailW - defW - 68;
		if (px >= defX && px <= defX + defW && py >= detailY + 3 && py <= detailY + 21)
		{
			ModBackground.ResetToDefault(saveConfig: true);
			SoundMn.gI().buttonClick();
			return true;
		}

		// 3. Tương tác trên thẻ danh sách
		int listX = detailX + 4;
		int listY = detailY + 24;
		int listW = detailW - 8;
		int listH = detailH - 28;

		if (px < listX || px > listX + listW || py < listY || py > listY + listH)
		{
			return false;
		}

		int cardH = 46;
		int cardStep = 52;

		for (int i = 0; i < ModBackground.bgList.Count; i++)
		{
			BackgroundItem item = ModBackground.bgList[i];
			int cardX = listX + 4;
			int cardY = listY + 4 + i * cardStep - scrollY;
			int cardW = listW - 8;

			if (cardY + cardH < listY || cardY > listY + listH)
			{
				continue;
			}

			int btnW = 62;
			int btnX = cardX + cardW - btnW - 4;

			if (item.isDownloading)
			{
				continue;
			}

			if (!item.isDownloaded)
			{
				// Nút TẢI VỀ
				if (px >= btnX && px <= btnX + btnW && py >= cardY + 14 && py <= cardY + 32)
				{
					ModBackground.DownloadBackgroundAsync(item);
					SoundMn.gI().buttonClick();
					return true;
				}
			}
			else
			{
				// Nút ÁP DỤNG
				if (px >= btnX && px <= btnX + btnW && py >= cardY + 4 && py <= cardY + 22)
				{
					ModBackground.ApplyBackground(item.id, saveConfig: true);
					SoundMn.gI().buttonClick();
					return true;
				}

				// Nút XÓA TỆP
				if (px >= btnX && px <= btnX + btnW && py >= cardY + 24 && py <= cardY + 42)
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
