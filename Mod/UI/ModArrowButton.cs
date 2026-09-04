using System;

public static class ModArrowButton
{
	// Vị trí và kích thước vùng bấm nút tam giác / mũi tên menu sát mép phải màn hình
	public static void GetBounds(out int x, out int y, out int w, out int h)
	{
		Image img = (GameScr.imgMenu != null) ? GameScr.imgMenu : GameScr.imgArrow;
		int imgW = (img != null) ? mGraphics.getImageWidth(img) : 14;
		int imgH = (img != null) ? mGraphics.getImageHeight(img) : 30;

		w = imgW + 12;
		h = imgH + 12;

		// Nằm sát mép phải màn hình
		x = GameCanvas.w - w;
		// Căn giữa chiều cao bên phải màn hình
		y = GameCanvas.h / 2 - h / 2;
	}

	public static void Paint(mGraphics g)
	{
		try
		{
			if (!ModMenu.IsInGame())
			{
				return;
			}

			// Nạp hình ảnh asset gốc của game nếu chưa có
			if (GameScr.imgMenu == null)
			{
				GameScr.imgMenu = GameCanvas.loadImage("/mainImage/myTexture2dmenu.png");
			}
			if (GameScr.imgArrow == null)
			{
				GameScr.imgArrow = GameCanvas.loadImage("/mainImage/myTexture2darrow.png");
			}
			if (GameScr.imgArrow2 == null)
			{
				GameScr.imgArrow2 = GameCanvas.loadImage("/mainImage/myTexture2darrow2.png");
			}

			g.translate(-g.getTranslateX(), -g.getTranslateY());
			g.setClip(0, 0, GameCanvas.w, GameCanvas.h);

			// Trạng thái Mod Menu đang mở hay đóng
			bool isOpen = (ModMenu.modMenuOpen && GameCanvas.menu != null && GameCanvas.menu.showMenu) || ModUI.uiCustomOpen;

			Image menuImg = GameScr.imgMenu;
			if (menuImg != null)
			{
				int imgW = mGraphics.getImageWidth(menuImg);
				int imgH = mGraphics.getImageHeight(menuImg);
				int drawX = GameCanvas.w - imgW;
				int drawY = GameCanvas.h / 2 - imgH / 2;

				// transform = 2 (TRANS_MIRROR): Nút tam giác quay vào trong (<) khi menu đang đóng
				// transform = 0 (Normal): Nút tam giác quay ra ngoài (>) khi menu đang mở
				int transform = isOpen ? 0 : 2;
				g.drawRegion(menuImg, 0, 0, imgW, imgH, transform, drawX, drawY, 0);

				// Hiệu ứng rê chuột (hover flare gốc của game)
				int hitX, hitY, hitW, hitH;
				GetBounds(out hitX, out hitY, out hitW, out hitH);
				if (GameCanvas.px >= hitX && GameCanvas.px <= GameCanvas.w && GameCanvas.py >= hitY && GameCanvas.py <= hitY + hitH)
				{
					if (ItemMap.imageFlare != null)
					{
						g.drawImage(ItemMap.imageFlare, drawX + imgW / 2, drawY + imgH / 2, 3);
					}
				}
			}
			else
			{
				// Fallback dùng asset mũi tên tam giác imgArrow / imgArrow2 nếu thiếu imgMenu
				Image arrowImg = isOpen ? GameScr.imgArrow : GameScr.imgArrow2;
				if (arrowImg != null)
				{
					int aw = mGraphics.getImageWidth(arrowImg);
					int ah = mGraphics.getImageHeight(arrowImg);
					int drawX = GameCanvas.w - aw - 2;
					int drawY = GameCanvas.h / 2 - ah / 2;
					g.drawImage(arrowImg, drawX, drawY, 0);

					int hitX, hitY, hitW, hitH;
					GetBounds(out hitX, out hitY, out hitW, out hitH);
					if (GameCanvas.px >= hitX && GameCanvas.px <= GameCanvas.w && GameCanvas.py >= hitY && GameCanvas.py <= hitY + hitH)
					{
						if (ItemMap.imageFlare != null)
						{
							g.drawImage(ItemMap.imageFlare, drawX + aw / 2, drawY + ah / 2, 3);
						}
					}
				}
			}

			g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
		}
		catch
		{
		}
	}

	public static bool CheckClick()
	{
		try
		{
			if (!ModMenu.IsInGame())
			{
				return false;
			}

			int x, y, w, h;
			GetBounds(out x, out y, out w, out h);

			if (GameCanvas.isPointerHoldIn(x, y, w, h))
			{
				if (GameCanvas.isPointerClick || GameCanvas.isPointerJustRelease)
				{
					GameCanvas.clearAllPointerEvent();
					Char me = Char.myCharz();
					if (me != null)
					{
						me.vMovePoints.removeAllElements();
						me.currentMovePoint = null;
					}
					if (GameScr.instance != null)
					{
						GameScr.instance.clickMoving = false;
					}
					ModHotkey.ToggleModMenu();
					return true;
				}
				return true; // Giữ chuột trên nút -> chặn click xuyên qua
			}
		}
		catch
		{
		}
		return false;
	}
}
