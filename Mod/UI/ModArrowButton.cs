using System;

/// <summary>
/// CLASS ModArrowButton: Định nghĩa và điều phối nút mở Mod Menu và cụm nút tác vụ nhanh bên phải màn hình.
/// </summary>
public static class ModArrowButton
{
	private static readonly short[] PotaraTemplateIds = new short[] { 921, 454, 933, 1155, 1165, 1228, 1852 };

	// Vị trí và kích thước vùng bấm nút tam giác / mũi tên menu sát mép phải màn hình
	public static void GetBounds(out int x, out int y, out int w, out int h)
	{
		Image img = (GameScr.imgMenu != null) ? GameScr.imgMenu : GameScr.imgArrow;
		int imgW = (img != null) ? mGraphics.getImageWidth(img) : 14;
		int imgH = (img != null) ? mGraphics.getImageHeight(img) : 30;

		w = imgW + 12;
		h = imgH + 12;

		// Nằm sát mép phải màn hình có tính toán Safe Area
		x = GameCanvas.w - w - SafeArea.Right;
		// Căn giữa chiều cao bên phải màn hình, tránh đè lên cụm nút nhanh phía dưới trên màn hình thấp
		y = GameCanvas.h / 2 - h / 2;
		int maxMenuY = GameCanvas.h - 150 - h;
		if (y > maxMenuY && maxMenuY > 40)
		{
			y = maxMenuY;
		}
	}

	// Vị trí nút Khóa Mục Tiêu (Gim) bên phải màn hình (nằm ngay phía trên nút Đổi Mục Tiêu gốc)
	public static void GetLockTargetBounds(out int x, out int y, out int w, out int h)
	{
		w = 32;
		h = 32;
		x = GameCanvas.w - 40 - SafeArea.Right;
		y = GameCanvas.h - 146;
	}

	// Vị trí nút Hợp Thể Bông Tai Porata bên phải màn hình (nằm kế bên trái nút Khóa Mục Tiêu)
	public static void GetPotaraBounds(out int x, out int y, out int w, out int h)
	{
		w = 32;
		h = 32;
		x = GameCanvas.w - 76 - SafeArea.Right;
		y = GameCanvas.h - 146;
	}

	public static bool IsQuickButtonsVisible()
	{
		if (!ModMenu.IsInGame())
		{
			return false;
		}
		Char me = Char.myCharz();
		if (me == null || me.statusMe == 14)
		{
			return false;
		}
		if ((GameCanvas.panel != null && GameCanvas.panel.isShow) ||
		    (GameCanvas.panel2 != null && GameCanvas.panel2.isShow) ||
		    (GameCanvas.menu != null && GameCanvas.menu.showMenu) ||
		    GameCanvas.currentDialog != null ||
		    (ChatTextField.gI() != null && ChatTextField.gI().isShow) ||
		    ChatPopup.currChatPopup != null ||
		    ModUI.uiCustomOpen)
		{
			return false;
		}
		return true;
	}

	private static bool FindPotaraInBag(out int bagIndex, out int iconId, out string itemName)
	{
		bagIndex = -1;
		iconId = 3893;
		itemName = "Bông tai Porata";

		ItemTemplate tpl454 = ItemTemplates.get(454);
		if (tpl454 != null && tpl454.iconID > 0)
		{
			iconId = tpl454.iconID;
		}

		Char me = Char.myCharz();
		if (me == null || me.arrItemBag == null)
		{
			return false;
		}

		for (int p = 0; p < PotaraTemplateIds.Length; p++)
		{
			short targetId = PotaraTemplateIds[p];
			for (int i = 0; i < me.arrItemBag.Length; i++)
			{
				Item it = me.arrItemBag[i];
				if (it != null && it.template != null && it.template.id == targetId)
				{
					bagIndex = i;
					if (it.template.iconID > 0)
					{
						iconId = it.template.iconID;
					}
					if (!string.IsNullOrEmpty(it.template.name))
					{
						itemName = it.template.name;
					}
					return true;
				}
			}
		}

		for (int i = 0; i < me.arrItemBag.Length; i++)
		{
			Item it = me.arrItemBag[i];
			if (it == null || it.template == null || string.IsNullOrEmpty(it.template.name))
			{
				continue;
			}
			string lower = it.template.name.ToLowerInvariant();
			if (lower.Contains("bông tai") || lower.Contains("bong tai") || lower.Contains("porata") || lower.Contains("potara"))
			{
				bagIndex = i;
				if (it.template.iconID > 0)
				{
					iconId = it.template.iconID;
				}
				itemName = it.template.name;
				return true;
			}
		}

		return false;
	}

	public static void UsePotaraFusion()
	{
		Char me = Char.myCharz();
		if (me == null || Char.isLoadingMap || Char.ischangingMap || me.isDie || me.cHP <= 0 || me.statusMe == 14)
		{
			return;
		}

		int bagIndex;
		int iconId;
		string itemName;
		if (FindPotaraInBag(out bagIndex, out iconId, out itemName) && bagIndex >= 0)
		{
			Service.gI().useItem(0, 1, (sbyte)bagIndex, -1);
			SoundMn.gI().buttonClick();
		}
		else
		{
			Service.gI().funsion((sbyte)ModAutoFusion.fusionType);
			SoundMn.gI().buttonClick();
			GameScr.info1.addInfo("Không tìm thấy Bông tai Porata trong hành trang!", 0);
		}
	}

	/// <summary>
	/// Thực hiện vẽ toàn bộ đối tượng và các lớp hiển thị lên canvas mGraphics.
	/// </summary>
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
			if (GameScr.imgNut == null)
			{
				GameScr.imgNut = GameCanvas.loadImage("/mainImage/myTexture2dnut.png");
			}
			if (GameScr.imgNutF == null)
			{
				GameScr.imgNutF = GameCanvas.loadImage("/mainImage/myTexture2dnutF.png");
			}
			if (GameScr.imgFocus == null)
			{
				GameScr.imgFocus = GameCanvas.loadImage("/mainImage/myTexture2dfocus.png");
			}
			if (GameScr.imgFocus2 == null)
			{
				GameScr.imgFocus2 = GameCanvas.loadImage("/mainImage/myTexture2dfocus2.png");
			}

			g.translate(-g.getTranslateX(), -g.getTranslateY());
			g.setClip(0, 0, GameCanvas.w, GameCanvas.h);

			// Trạng thái Mod Menu đang mở hay đóng
			bool isOpen = (ModMenu.modMenuOpen && GameCanvas.menu != null && GameCanvas.menu.showMenu) || ModUI.uiCustomOpen;

			int hitX, hitY, hitW, hitH;
			GetBounds(out hitX, out hitY, out hitW, out hitH);

			Image menuImg = GameScr.imgMenu;
			if (menuImg != null)
			{
				int imgW = mGraphics.getImageWidth(menuImg);
				int imgH = mGraphics.getImageHeight(menuImg);
				int drawX = GameCanvas.w - imgW - SafeArea.Right;
				int drawY = hitY + (hitH - imgH) / 2;

				// transform = 2 (TRANS_MIRROR): Nút tam giác quay vào trong (<) khi menu đang đóng
				// transform = 0 (Normal): Nút tam giác quay ra ngoài (>) khi menu đang mở
				int transform = isOpen ? 0 : 2;
				g.drawRegion(menuImg, 0, 0, imgW, imgH, transform, drawX, drawY, 0);

				// Hiệu ứng rê chuột (hover flare gốc của game)
				if (GameCanvas.px >= hitX && GameCanvas.px <= GameCanvas.w - SafeArea.Right && GameCanvas.py >= hitY && GameCanvas.py <= hitY + hitH)
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
					int drawX = GameCanvas.w - aw - 2 - SafeArea.Right;
					int drawY = hitY + (hitH - ah) / 2;
					g.drawImage(arrowImg, drawX, drawY, 0);

					if (GameCanvas.px >= hitX && GameCanvas.px <= GameCanvas.w - SafeArea.Right && GameCanvas.py >= hitY && GameCanvas.py <= hitY + hitH)
					{
						if (ItemMap.imageFlare != null)
						{
							g.drawImage(ItemMap.imageFlare, drawX + aw / 2, drawY + ah / 2, 3);
						}
					}
				}
			}

			// Vẽ cụm 2 nút tác vụ nhanh bên phải: Hợp Thể Bông Tai & Khóa Mục Tiêu
			if (IsQuickButtonsVisible())
			{
				Char me = Char.myCharz();

				// 1. Nút Hợp Thể Bông Tai Porata (chỉ hiển thị nút tròn + icon bông tai, không hiển thị chữ)
				int potX, potY, potW, potH;
				GetPotaraBounds(out potX, out potY, out potW, out potH);
				int potCenterX = potX + potW / 2;
				int potCenterY = potY + potH / 2;
				bool isFused = me != null && (me.isFusion || me.isNhapThe);
				bool isHoverPot = GameCanvas.px >= potX && GameCanvas.px <= potX + potW && GameCanvas.py >= potY && GameCanvas.py <= potY + potH;

				Image potBg = (isFused || isHoverPot) ? GameScr.imgNutF : GameScr.imgNut;
				if (potBg != null)
				{
					g.drawImage(potBg, potCenterX, potCenterY, mGraphics.HCENTER | mGraphics.VCENTER);
				}

				int bagIdx;
				int potIconId;
				string potName;
				FindPotaraInBag(out bagIdx, out potIconId, out potName);
				SmallImage.drawSmallImage(g, potIconId, potCenterX, potCenterY, 0, mGraphics.VCENTER | mGraphics.HCENTER);

				// 2. Nút Khóa Mục Tiêu (Gim Mục Tiêu)
				int lockX, lockY, lockW, lockH;
				GetLockTargetBounds(out lockX, out lockY, out lockW, out lockH);
				int lockCenterX = lockX + lockW / 2;
				int lockCenterY = lockY + lockH / 2;
				bool isLocked = ModMapEntityHUD.isLockTarget;
				bool isHoverLock = GameCanvas.px >= lockX && GameCanvas.px <= lockX + lockW && GameCanvas.py >= lockY && GameCanvas.py <= lockY + lockH;

				Image lockBg = (isLocked || isHoverLock) ? GameScr.imgNutF : GameScr.imgNut;
				if (lockBg != null)
				{
					g.drawImage(lockBg, lockCenterX, lockCenterY, mGraphics.HCENTER | mGraphics.VCENTER);
				}

				Image focusIcon = isLocked ? GameScr.imgFocus2 : GameScr.imgFocus;
				if (focusIcon != null)
				{
					g.drawImage(focusIcon, lockCenterX, lockCenterY, mGraphics.HCENTER | mGraphics.VCENTER);
				}

				if (isLocked)
				{
					if (mFont.tahoma_7b_red != null)
					{
						mFont.tahoma_7b_red.drawString(g, "KHÓA", lockCenterX, lockY + lockH - 10, mFont.CENTER, mFont.tahoma_7_grey);
					}
				}
				else
				{
					if (mFont.tahoma_7_yellow != null)
					{
						mFont.tahoma_7_yellow.drawString(g, "GIM", lockCenterX, lockY + lockH - 10, mFont.CENTER, mFont.tahoma_7_grey);
					}
				}
			}

			g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
		}
		catch
		{
		}
	}

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của CheckClick.
	/// </summary>
	public static bool CheckClick()
	{
		try
		{
			if (!ModMenu.IsInGame())
			{
				return false;
			}

			// Kiểm tra click cụm nút tác vụ nhanh bên phải (Hợp Thể Bông Tai & Khóa Mục Tiêu)
			if (IsQuickButtonsVisible())
			{
				int potX, potY, potW, potH;
				GetPotaraBounds(out potX, out potY, out potW, out potH);
				if (GameCanvas.isPointerHoldIn(potX, potY, potW, potH))
				{
					mScreen.keyTouch = -1;
					GameCanvas.isPointerJustDown = false;
					if (GameCanvas.isPointerClick || GameCanvas.isPointerJustRelease)
					{
						GameCanvas.clearAllPointerEvent();
						StopCharacterClickMove();
						UsePotaraFusion();
						return true;
					}
					return true;
				}

				int lockX, lockY, lockW, lockH;
				GetLockTargetBounds(out lockX, out lockY, out lockW, out lockH);
				if (GameCanvas.isPointerHoldIn(lockX, lockY, lockW, lockH))
				{
					mScreen.keyTouch = -1;
					GameCanvas.isPointerJustDown = false;
					if (GameCanvas.isPointerClick || GameCanvas.isPointerJustRelease)
					{
						GameCanvas.clearAllPointerEvent();
						StopCharacterClickMove();
						ModMapEntityHUD.ToggleLockTarget();
						return true;
					}
					return true;
				}
			}

			int x, y, w, h;
			GetBounds(out x, out y, out w, out h);

			if (GameCanvas.isPointerHoldIn(x, y, w, h))
			{
				if (GameCanvas.isPointerClick || GameCanvas.isPointerJustRelease)
				{
					GameCanvas.clearAllPointerEvent();
					StopCharacterClickMove();
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

	private static void StopCharacterClickMove()
	{
		Char me = Char.myCharz();
		if (me != null)
		{
			me.vMovePoints.removeAllElements();
			me.currentMovePoint = null;
		}
		if (GameScr.instance != null)
		{
			GameScr.instance.clickMoving = false;
			GameScr.instance.isPointerDowning = false;
		}
	}
}
