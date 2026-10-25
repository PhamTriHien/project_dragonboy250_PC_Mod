using System;

/// <summary>
/// Quản lý nút mở Menu Mod và cụm nút tác vụ nhanh (Bông Tai, Khóa Mục Tiêu, Capsule, Khu, Qua Map Nhanh, Dừng Xmap) bằng 100% asset gốc NRO.
/// </summary>
public static class ModArrowButton
{
	public static readonly short[] PotaraTemplateIds = new short[] { 1884, 921, 454, 933, 1155, 1165, 1228, 1852 };

	public static bool showExtraQuickButtons = false;

	public static void GetBounds(out int x, out int y, out int w, out int h)
	{
		Image img = (GameScr.imgMenu != null) ? GameScr.imgMenu : GameScr.imgArrow;
		int imgW = (img != null) ? mGraphics.getImageWidth(img) : 14;
		int imgH = (img != null) ? mGraphics.getImageHeight(img) : 30;

		w = imgW + 12;
		h = imgH + 12;
		x = GameCanvas.w - w - SafeArea.Right;
		y = GameCanvas.h / 2 - h / 2;
		int maxMenuY = GameCanvas.h - 150 - h;
		if (y > maxMenuY && maxMenuY > 40)
		{
			y = maxMenuY;
		}
	}

	public static void GetLockTargetBounds(out int x, out int y, out int w, out int h)
	{
		w = 32;
		h = 32;
		x = GameCanvas.w - 40 - SafeArea.Right;
		y = GameCanvas.h - 146;
	}

	public static void GetPotaraBounds(out int x, out int y, out int w, out int h)
	{
		w = 32;
		h = 32;
		x = GameCanvas.w - 76 - SafeArea.Right;
		y = GameCanvas.h - 146;
	}

	public static void GetCapsuleBounds(out int x, out int y, out int w, out int h)
	{
		w = 32;
		h = 32;
		x = GameCanvas.w - 112 - SafeArea.Right;
		y = GameCanvas.h - 146;
	}

	public static void GetZoneBounds(out int x, out int y, out int w, out int h)
	{
		w = 32;
		h = 32;
		x = GameCanvas.w - 148 - SafeArea.Right;
		y = GameCanvas.h - 146;
	}

	public static void GetMapDirBounds(int dir, out int x, out int y, out int w, out int h)
	{
		w = 28;
		h = 28;
		int cx = GameCanvas.w / 2;
		y = GameCanvas.h - 86;
		if (dir == 0) x = cx - 46;
		else if (dir == 2) x = cx - 14;
		else x = cx + 18;
	}

	public static void GetStopActionBounds(out int x, out int y, out int w, out int h)
	{
		w = 72;
		h = 20;
		x = GameCanvas.w / 2 - w / 2;
		y = 60;
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

	public static bool FindPotaraInBag(out int bagIndex, out int iconId, out string itemName)
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

	public static bool FindCapsuleInBag(out int bagIndex, out int iconId)
	{
		bagIndex = -1;
		iconId = 1088;
		ItemTemplate tpl194 = ItemTemplates.get(194);
		if (tpl194 != null && tpl194.iconID > 0) iconId = tpl194.iconID;

		Char me = Char.myCharz();
		if (me == null || me.arrItemBag == null) return false;

		short[] ids = new short[] { 194, 193 };
		for (int k = 0; k < ids.Length; k++)
		{
			for (int i = 0; i < me.arrItemBag.Length; i++)
			{
				Item it = me.arrItemBag[i];
				if (it != null && it.template != null && it.template.id == ids[k])
				{
					bagIndex = i;
					if (it.template.iconID > 0) iconId = it.template.iconID;
					return true;
				}
			}
		}
		return false;
	}

	public static void UseCapsuleQuick()
	{
		Char me = Char.myCharz();
		if (me == null || Char.isLoadingMap || Char.ischangingMap || me.isDie || me.statusMe == 14) return;
		int idx;
		int iconId;
		if (FindCapsuleInBag(out idx, out iconId) && idx >= 0)
		{
			Service.gI().useItem(0, 1, (sbyte)idx, -1);
			SoundMn.gI().buttonClick();
		}
		else
		{
			GameScr.info1.addInfo("Không tìm thấy Capsule (194/193) trong hành trang!", 0);
		}
	}

	public static void UseTdltQuick()
	{
		Char me = Char.myCharz();
		if (me == null || Char.isLoadingMap || Char.ischangingMap || me.isDie || me.statusMe == 14) return;
		if (me.arrItemBag != null)
		{
			for (int i = 0; i < me.arrItemBag.Length; i++)
			{
				Item it = me.arrItemBag[i];
				if (it != null && it.template != null && it.template.id == 521)
				{
					Service.gI().useItem(0, 1, (sbyte)i, -1);
					SoundMn.gI().buttonClick();
					return;
				}
			}
		}
		GameScr.info1.addInfo("Không tìm thấy Tự Động Luyện Tập (521) trong hành trang!", 0);
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

	private static bool HasStoppableTask()
	{
		return ModNextMap.isNextMapActive || ModBossNotice.isAutoScanBossZone;
	}

	public static void Paint(mGraphics g)
	{
		try
		{
			if (!ModMenu.IsInGame())
			{
				return;
			}

			if (GameScr.imgMenu == null) GameScr.imgMenu = GameCanvas.loadImage("/mainImage/myTexture2dmenu.png");
			if (GameScr.imgArrow == null) GameScr.imgArrow = GameCanvas.loadImage("/mainImage/myTexture2darrow.png");
			if (GameScr.imgArrow2 == null) GameScr.imgArrow2 = GameCanvas.loadImage("/mainImage/myTexture2darrow2.png");
			if (GameScr.imgNut == null) GameScr.imgNut = GameCanvas.loadImage("/mainImage/myTexture2dnut.png");
			if (GameScr.imgNutF == null) GameScr.imgNutF = GameCanvas.loadImage("/mainImage/myTexture2dnutF.png");
			if (GameScr.imgFocus == null) GameScr.imgFocus = GameCanvas.loadImage("/mainImage/myTexture2dfocus.png");
			if (GameScr.imgFocus2 == null) GameScr.imgFocus2 = GameCanvas.loadImage("/mainImage/myTexture2dfocus2.png");

			g.translate(-g.getTranslateX(), -g.getTranslateY());
			g.setClip(0, 0, GameCanvas.w, GameCanvas.h);

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
				int transform = isOpen ? 0 : 2;
				g.drawRegion(menuImg, 0, 0, imgW, imgH, transform, drawX, drawY, 0);

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
				Image arrowImg = isOpen ? GameScr.imgArrow : GameScr.imgArrow2;
				if (arrowImg != null)
				{
					int aw = mGraphics.getImageWidth(arrowImg);
					int ah = mGraphics.getImageHeight(arrowImg);
					int drawX = GameCanvas.w - aw - 2 - SafeArea.Right;
					int drawY = hitY + (hitH - ah) / 2;
					g.drawImage(arrowImg, drawX, drawY, 0);
				}
			}

			if (IsQuickButtonsVisible())
			{
				Char me = Char.myCharz();

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

				if (HasStoppableTask())
				{
					int sx, sy, sw, sh;
					GetStopActionBounds(out sx, out sy, out sw, out sh);
					PopUp.paintPopUp(g, sx, sy, sw, sh, 1, isButton: true);
					mFont.tahoma_7b_red.drawString(g, "DỪNG CHẠY", sx + sw / 2, sy + 3, mFont.CENTER);
				}

				if (showExtraQuickButtons)
				{
					int capX, capY, capW, capH;
					GetCapsuleBounds(out capX, out capY, out capW, out capH);
					bool hoverCap = GameCanvas.px >= capX && GameCanvas.px <= capX + capW && GameCanvas.py >= capY && GameCanvas.py <= capY + capH;
					Image capBg = hoverCap ? GameScr.imgNutF : GameScr.imgNut;
					if (capBg != null) g.drawImage(capBg, capX + capW / 2, capY + capH / 2, mGraphics.HCENTER | mGraphics.VCENTER);
					int capIdx, capIcon;
					FindCapsuleInBag(out capIdx, out capIcon);
					SmallImage.drawSmallImage(g, capIcon, capX + capW / 2, capY + capH / 2, 0, mGraphics.VCENTER | mGraphics.HCENTER);

					int znX, znY, znW, znH;
					GetZoneBounds(out znX, out znY, out znW, out znH);
					bool hoverZn = GameCanvas.px >= znX && GameCanvas.px <= znX + znW && GameCanvas.py >= znY && GameCanvas.py <= znY + znH;
					Image znBg = hoverZn ? GameScr.imgNutF : GameScr.imgNut;
					if (znBg != null) g.drawImage(znBg, znX + znW / 2, znY + znH / 2, mGraphics.HCENTER | mGraphics.VCENTER);
					mFont.tahoma_7b_yellow.drawString(g, "K." + TileMap.zoneID, znX + znW / 2, znY + znH / 2 - 5, mFont.CENTER, mFont.tahoma_7_grey);

					int[] dirs = new int[] { 0, 2, 1 };
					string[] dirLabels = new string[] { "<", "^", ">" };
					for (int d = 0; d < dirs.Length; d++)
					{
						int dx, dy, dw, dh;
						GetMapDirBounds(dirs[d], out dx, out dy, out dw, out dh);
						bool hoverDir = GameCanvas.px >= dx && GameCanvas.px <= dx + dw && GameCanvas.py >= dy && GameCanvas.py <= dy + dh;
						Image dirBg = hoverDir ? GameScr.imgNutF : GameScr.imgNut;
						if (dirBg != null) g.drawImage(dirBg, dx + dw / 2, dy + dh / 2, mGraphics.HCENTER | mGraphics.VCENTER);
						mFont.tahoma_7b_white.drawString(g, dirLabels[d], dx + dw / 2, dy + dh / 2 - 5, mFont.CENTER, mFont.tahoma_7_grey);
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

			if (IsQuickButtonsVisible())
			{
				if (HasStoppableTask())
				{
					int sx, sy, sw, sh;
					GetStopActionBounds(out sx, out sy, out sw, out sh);
					if (GameCanvas.isPointerHoldIn(sx, sy, sw, sh))
					{
						mScreen.keyTouch = -1;
						GameCanvas.isPointerJustDown = false;
						if (GameCanvas.isPointerClick || GameCanvas.isPointerJustRelease)
						{
							GameCanvas.clearAllPointerEvent();
							StopCharacterClickMove();
							if (ModNextMap.isNextMapActive) ModNextMap.StopNextMap();
							if (ModBossNotice.isAutoScanBossZone) ModBossNotice.StopAutoScanZone();
							SoundMn.gI().buttonClick();
							return true;
						}
						return true;
					}
				}

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

				if (showExtraQuickButtons)
				{
					int capX, capY, capW, capH;
					GetCapsuleBounds(out capX, out capY, out capW, out capH);
					if (GameCanvas.isPointerHoldIn(capX, capY, capW, capH))
					{
						mScreen.keyTouch = -1;
						GameCanvas.isPointerJustDown = false;
						if (GameCanvas.isPointerClick || GameCanvas.isPointerJustRelease)
						{
							GameCanvas.clearAllPointerEvent();
							StopCharacterClickMove();
							UseCapsuleQuick();
							return true;
						}
						return true;
					}

					int znX, znY, znW, znH;
					GetZoneBounds(out znX, out znY, out znW, out znH);
					if (GameCanvas.isPointerHoldIn(znX, znY, znW, znH))
					{
						mScreen.keyTouch = -1;
						GameCanvas.isPointerJustDown = false;
						if (GameCanvas.isPointerClick || GameCanvas.isPointerJustRelease)
						{
							GameCanvas.clearAllPointerEvent();
							StopCharacterClickMove();
							Service.gI().openUIZone();
							SoundMn.gI().buttonClick();
							return true;
						}
						return true;
					}

					int[] dirs = new int[] { 0, 2, 1 };
					for (int d = 0; d < dirs.Length; d++)
					{
						int dx, dy, dw, dh;
						GetMapDirBounds(dirs[d], out dx, out dy, out dw, out dh);
						if (GameCanvas.isPointerHoldIn(dx, dy, dw, dh))
						{
							mScreen.keyTouch = -1;
							GameCanvas.isPointerJustDown = false;
							if (GameCanvas.isPointerClick || GameCanvas.isPointerJustRelease)
							{
								GameCanvas.clearAllPointerEvent();
								StopCharacterClickMove();
								ModNextMap.LoadMapDirection(dirs[d]);
								SoundMn.gI().buttonClick();
								return true;
							}
							return true;
						}
					}
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
				return true;
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
