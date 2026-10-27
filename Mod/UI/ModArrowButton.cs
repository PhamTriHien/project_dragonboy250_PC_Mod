using System;

/// <summary>
/// Quản lý nút mở Menu Mod và cụm nút tác vụ góc dưới phải xếp theo cung tròn hình quạt tay 2 tầng đồng tâm bằng 100% asset gốc NRO.
/// </summary>
public static class ModArrowButton
{
	public static readonly short[] PotaraTemplateIds = new short[] { 1884, 921, 454, 933, 1155, 1165, 1228, 1852 };

	public static bool showExtraQuickButtons = false;

	public const int FAN_PIVOT_MARGIN = 26;
	public const int FAN_INNER_RADIUS = 45;
	public const int FAN_OUTER_RADIUS = 80;
	public const int FAN_EXTRA_RADIUS = 115;
	public const int FAN_BTN_SIZE = 32;
	public const int FAN_BTN_RADIUS = 16;
	public const int FAN_FIRE_SIZE = 40;
	public const int FAN_FIRE_RADIUS = 21;

	public static void GetFanPivot(out int pivotX, out int pivotY)
	{
		pivotX = GameCanvas.w - SafeArea.Right - FAN_PIVOT_MARGIN;
		pivotY = GameCanvas.h - FAN_PIVOT_MARGIN;
	}

	public static void GetFanPoint(int radius, double angleDeg, out int cx, out int cy)
	{
		int px, py;
		GetFanPivot(out px, out py);
		double rad = angleDeg * System.Math.PI / 180.0;
		cx = px + (int)System.Math.Round(radius * System.Math.Cos(rad));
		cy = py - (int)System.Math.Round(radius * System.Math.Sin(rad));
	}

	private static void GetFanButtonBounds(int radius, double angleDeg, out int x, out int y, out int w, out int h)
	{
		int cx, cy;
		GetFanPoint(radius, angleDeg, out cx, out cy);
		w = FAN_BTN_SIZE;
		h = FAN_BTN_SIZE;
		x = cx - w / 2;
		y = cy - h / 2;
	}

	public static bool IsCursorInCircle(int cx, int cy, int radius)
	{
		int dx = GameCanvas.px - cx;
		int dy = GameCanvas.py - cy;
		return dx * dx + dy * dy <= radius * radius;
	}

	public static bool IsPointerHoldInCircle(int cx, int cy, int radius)
	{
		if (!GameCanvas.isPointerDown && !GameCanvas.isPointerJustRelease && !GameCanvas.isPointerClick)
		{
			return false;
		}
		int dx = GameCanvas.px - cx;
		int dy = GameCanvas.py - cy;
		return dx * dx + dy * dy <= radius * radius;
	}

	public static void GetAttackBounds(out int x, out int y, out int w, out int h)
	{
		int px, py;
		GetFanPivot(out px, out py);
		w = FAN_FIRE_SIZE;
		h = FAN_FIRE_SIZE;
		x = px - w / 2;
		y = py - h / 2;
	}

	public static void GetFocusBounds(out int x, out int y, out int w, out int h)
	{
		GetFanButtonBounds(FAN_INNER_RADIUS, 90.0, out x, out y, out w, out h);
	}

	public static void GetCapsuleBounds(out int x, out int y, out int w, out int h)
	{
		GetFanButtonBounds(FAN_INNER_RADIUS, 135.0, out x, out y, out w, out h);
	}

	public static void GetPeaBounds(out int x, out int y, out int w, out int h)
	{
		if (GameScr.isAnalog != 0)
		{
			GetFanButtonBounds(FAN_INNER_RADIUS, 180.0, out x, out y, out w, out h);
		}
		else
		{
			int px, py;
			GetFanPivot(out px, out py);
			w = FAN_FIRE_SIZE;
			h = FAN_FIRE_SIZE;
			x = px - w / 2;
			y = py - h / 2;
		}
	}

	public static void GetLockTargetBounds(out int x, out int y, out int w, out int h)
	{
		if (GameScr.isAnalog != 0)
		{
			GetFanButtonBounds(FAN_OUTER_RADIUS, 90.0, out x, out y, out w, out h);
		}
		else
		{
			GetFanButtonBounds(FAN_INNER_RADIUS, 90.0, out x, out y, out w, out h);
		}
	}

	public static void GetPotaraBounds(out int x, out int y, out int w, out int h)
	{
		GetFanButtonBounds(FAN_OUTER_RADIUS, 118.0, out x, out y, out w, out h);
	}

	public static void GetAutoTrainBounds(out int x, out int y, out int w, out int h)
	{
		GetFanButtonBounds(FAN_OUTER_RADIUS, 146.0, out x, out y, out w, out h);
	}

	public static void GetZoneBounds(out int x, out int y, out int w, out int h)
	{
		GetFanButtonBounds(FAN_EXTRA_RADIUS, 104.0, out x, out y, out w, out h);
	}

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
		if (!ModMenu.IsInGame() || ModGraphics.isAfkBlackScreen || !GameCanvas.isTouch)
		{
			return false;
		}
		Char me = Char.myCharz();
		if (me == null || me.statusMe == 14 || (me.taskMaint != null && me.taskMaint.taskId == 0))
		{
			return false;
		}
		if (GameScr.instance != null && (GameScr.instance.mobCapcha != null || GameScr.instance.isPaintPopup() || GameScr.instance.popUpYesNo != null))
		{
			return false;
		}
		if (InfoDlg.isShow || GameCanvas.currentScreen == MoneyCharge.instance)
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
		iconId = (!GameScr.isudungCapsun4 && GameScr.isudungCapsun3) ? 1087 : 1088;
		ItemTemplate tpl194 = ItemTemplates.get(194);
		if (tpl194 != null && tpl194.iconID > 0 && iconId == 1088) iconId = tpl194.iconID;

		Char me = Char.myCharz();
		if (me == null || me.arrItemBag == null) return false;

		short[] ids = new short[] { 194, 193 };
		for (int k = 0; k < ids.Length; k++)
		{
			for (int i = 0; i < me.arrItemBag.Length; i++)
			{
				Item it = me.arrItemBag[i];
				if (it != null && it.template != null && it.template.id == ids[k] && it.quantity > 0)
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
		if (GameScr.isPickNgocRong)
		{
			GameCanvas.keyPressed[14] = true;
			GameScr.isPickNgocRong = false;
			Service.gI().useItem(-1, -1, -1, -1);
			SoundMn.gI().buttonClick();
			return;
		}
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
			if (!ModMenu.IsInGame() || ModGraphics.isAfkBlackScreen)
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

				int capX, capY, capW, capH;
				GetCapsuleBounds(out capX, out capY, out capW, out capH);
				int capCx = capX + capW / 2;
				int capCy = capY + capH / 2;
				bool hoverCap = IsCursorInCircle(capCx, capCy, FAN_BTN_RADIUS) || mScreen.keyTouch == 14;
				if (GameScr.isPickNgocRong)
				{
					if (GameScr.imgNR3 == null) GameScr.imgNR3 = GameCanvas.loadImage("/mainImage/myTexture2dPea_2.png");
					if (GameScr.imgNR4 == null) GameScr.imgNR4 = GameCanvas.loadImage("/mainImage/myTexture2dPea_3.png");
					Image nrImg = hoverCap ? GameScr.imgNR4 : GameScr.imgNR3;
					if (nrImg != null)
					{
						g.drawImage(nrImg, capCx, capCy, mGraphics.HCENTER | mGraphics.VCENTER);
					}
				}
				else
				{
					Image capBg = hoverCap ? GameScr.imgNutF : GameScr.imgNut;
					if (capBg != null)
					{
						g.drawImage(capBg, capCx, capCy, mGraphics.HCENTER | mGraphics.VCENTER);
					}
					int capIdx, capIcon;
					FindCapsuleInBag(out capIdx, out capIcon);
					SmallImage.drawSmallImage(g, capIcon, capCx, capCy, 0, mGraphics.VCENTER | mGraphics.HCENTER);
				}

				int lockX, lockY, lockW, lockH;
				GetLockTargetBounds(out lockX, out lockY, out lockW, out lockH);
				int lockCenterX = lockX + lockW / 2;
				int lockCenterY = lockY + lockH / 2;
				bool isLocked = ModMapEntityHUD.isLockTarget;
				bool isHoverLock = IsCursorInCircle(lockCenterX, lockCenterY, FAN_BTN_RADIUS);

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
					if (mFont.tahoma_7b_yellow != null)
					{
						mFont.tahoma_7b_yellow.drawString(g, "KHÓA", lockCenterX, lockY + lockH - 10, mFont.CENTER);
					}
				}
				else
				{
					if (mFont.tahoma_7_yellow != null)
					{
						mFont.tahoma_7_yellow.drawString(g, "GIM", lockCenterX, lockY + lockH - 10, mFont.CENTER);
					}
				}

				int potX, potY, potW, potH;
				GetPotaraBounds(out potX, out potY, out potW, out potH);
				int potCenterX = potX + potW / 2;
				int potCenterY = potY + potH / 2;
				bool isFused = me != null && (me.isFusion || me.isNhapThe);
				bool isHoverPot = IsCursorInCircle(potCenterX, potCenterY, FAN_BTN_RADIUS);

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

				int autoX, autoY, autoW, autoH;
				GetAutoTrainBounds(out autoX, out autoY, out autoW, out autoH);
				int autoCx = autoX + autoW / 2;
				int autoCy = autoY + autoH / 2;
				bool isAutoTrainOn = AutoTrainMenu.isChecked != null && AutoTrainMenu.isChecked.Length > 0 && AutoTrainMenu.isChecked[0];
				bool hoverAuto = IsCursorInCircle(autoCx, autoCy, FAN_BTN_RADIUS) || mScreen.keyTouch == 16;

				Image autoBg = (isAutoTrainOn || hoverAuto) ? GameScr.imgNutF : GameScr.imgNut;
				if (autoBg != null)
				{
					g.drawImage(autoBg, autoCx, autoCy, mGraphics.HCENTER | mGraphics.VCENTER);
				}

				if (GameScr.fraAuto == null)
				{
					Image imgAuto = GameCanvas.loadImage("/mainImage/img_auto.png");
					if (imgAuto != null)
					{
						GameScr.fraAuto = new FrameImage(imgAuto, 1, 6, 0);
					}
				}
				if (GameScr.fraAuto != null)
				{
					int idxAuto = isAutoTrainOn ? (GameCanvas.gameTick / 4 % 5 + 1) : 0;
					GameScr.fraAuto.drawFrame(idxAuto, autoCx, autoCy, 0, mGraphics.HCENTER | mGraphics.VCENTER, g);
				}
				else if (mFont.tahoma_7b_yellow != null)
				{
					mFont.tahoma_7b_yellow.drawString(g, "AT", autoCx, autoCy - 5, mFont.CENTER);
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
					int znX, znY, znW, znH;
					GetZoneBounds(out znX, out znY, out znW, out znH);
					int znCx = znX + znW / 2;
					int znCy = znY + znH / 2;
					bool hoverZn = IsCursorInCircle(znCx, znCy, FAN_BTN_RADIUS);
					Image znBg = hoverZn ? GameScr.imgNutF : GameScr.imgNut;
					if (znBg != null) g.drawImage(znBg, znCx, znCy, mGraphics.HCENTER | mGraphics.VCENTER);
					mFont.tahoma_7b_yellow.drawString(g, "K." + TileMap.zoneID, znCx, znCy - 5, mFont.CENTER);

					int[] dirs = new int[] { 0, 2, 1 };
					string[] dirLabels = new string[] { "<", "^", ">" };
					for (int d = 0; d < dirs.Length; d++)
					{
						int dx, dy, dw, dh;
						GetMapDirBounds(dirs[d], out dx, out dy, out dw, out dh);
						bool hoverDir = GameCanvas.px >= dx && GameCanvas.px <= dx + dw && GameCanvas.py >= dy && GameCanvas.py <= dy + dh;
						Image dirBg = hoverDir ? GameScr.imgNutF : GameScr.imgNut;
						if (dirBg != null) g.drawImage(dirBg, dx + dw / 2, dy + dh / 2, mGraphics.HCENTER | mGraphics.VCENTER);
						mFont.tahoma_7b_white.drawString(g, dirLabels[d], dx + dw / 2, dy + dh / 2 - 5, mFont.CENTER);
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
			if (!ModMenu.IsInGame() || ModGraphics.isAfkBlackScreen)
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

				int capX, capY, capW, capH;
				GetCapsuleBounds(out capX, out capY, out capW, out capH);
				if (IsPointerHoldInCircle(capX + capW / 2, capY + capH / 2, FAN_BTN_RADIUS))
				{
					mScreen.keyTouch = 14;
					GameCanvas.isPointerJustDown = false;
					if (GameCanvas.isPointerClick || GameCanvas.isPointerJustRelease)
					{
						mScreen.keyTouch = -1;
						GameCanvas.clearAllPointerEvent();
						StopCharacterClickMove();
						UseCapsuleQuick();
						return true;
					}
					return true;
				}

				int lockX, lockY, lockW, lockH;
				GetLockTargetBounds(out lockX, out lockY, out lockW, out lockH);
				if (IsPointerHoldInCircle(lockX + lockW / 2, lockY + lockH / 2, FAN_BTN_RADIUS))
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

				int potX, potY, potW, potH;
				GetPotaraBounds(out potX, out potY, out potW, out potH);
				if (IsPointerHoldInCircle(potX + potW / 2, potY + potH / 2, FAN_BTN_RADIUS))
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

				int autoX, autoY, autoW, autoH;
				GetAutoTrainBounds(out autoX, out autoY, out autoW, out autoH);
				if (IsPointerHoldInCircle(autoX + autoW / 2, autoY + autoH / 2, FAN_BTN_RADIUS))
				{
					mScreen.keyTouch = 16;
					GameCanvas.isPointerJustDown = false;
					if (GameCanvas.isPointerClick || GameCanvas.isPointerJustRelease)
					{
						mScreen.keyTouch = -1;
						GameCanvas.clearAllPointerEvent();
						StopCharacterClickMove();
						AutoTrainMenu.gI().show();
						SoundMn.gI().buttonClick();
						return true;
					}
					return true;
				}

				if (showExtraQuickButtons)
				{
					int znX, znY, znW, znH;
					GetZoneBounds(out znX, out znY, out znW, out znH);
					if (IsPointerHoldInCircle(znX + znW / 2, znY + znH / 2, FAN_BTN_RADIUS))
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
