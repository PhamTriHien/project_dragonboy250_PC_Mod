using System;

public partial class Panel : IActionListener, IChatable
{
	private bool invSplitIsBody = true;

	private int invSplitBodySel = 0;

	private int invSplitBagSel = 0;

	private const int INV_SPLIT_HEADER_H = 18;

	private const int INV_SPLIT_DIV = 4;

	private bool IsInventorySplit()
	{
		try
		{
			if (type != 0 || currentTabIndex != 1)
			{
				return false;
			}
			Char me = Char.myCharz();
			if (me == null || me.arrItemBody == null || me.arrItemBag == null)
			{
				return false;
			}
			return true;
		}
		catch
		{
			return false;
		}
	}

	private void GetInventorySplitLayout(out int leftX, out int leftW, out int rightX, out int rightW, out int listY, out int listH)
	{
		int totalW = wScroll;
		leftW = totalW * 46 / 100;
		if (leftW < 96)
		{
			leftW = 96;
		}
		if (leftW > 130)
		{
			leftW = 130;
		}
		if (totalW <= 200)
		{
			leftW = totalW / 2 - 2;
		}
		leftX = xScroll;
		rightX = leftX + leftW + INV_SPLIT_DIV;
		rightW = xScroll + totalW - rightX;
		if (rightW < 60)
		{
			rightW = 60;
		}
		listY = yScroll + INV_SPLIT_HEADER_H + 2;
		listH = hScroll - INV_SPLIT_HEADER_H - 2;
		if (listH < 24)
		{
			listH = 24;
		}
	}

	private void ClampInventorySplit()
	{
		try
		{
			Char me = Char.myCharz();
			if (me == null || me.arrItemBody == null || me.arrItemBag == null)
			{
				return;
			}
			int bodyLen = me.arrItemBody.Length;
			int bagLen = me.arrItemBag.Length;
			if (bodyLen <= 0)
			{
				invSplitBodySel = 0;
			}
			else
			{
				if (invSplitBodySel < 0)
				{
					invSplitBodySel = 0;
				}
				if (invSplitBodySel >= bodyLen)
				{
					invSplitBodySel = bodyLen - 1;
				}
			}
			if (bagLen <= 0)
			{
				invSplitBagSel = 0;
			}
			else
			{
				if (invSplitBagSel < 0)
				{
					invSplitBagSel = 0;
				}
				if (invSplitBagSel >= bagLen)
				{
					invSplitBagSel = bagLen - 1;
				}
			}
			if (bodyLen > 0 && bagLen == 0)
			{
				invSplitIsBody = true;
			}
			else if (bodyLen == 0 && bagLen > 0)
			{
				invSplitIsBody = false;
			}
			int lx, lw, rx, rw, ly, lh;
			GetInventorySplitLayout(out lx, out lw, out rx, out rw, out ly, out lh);
			int bagH = bagLen * ITEM_HEIGHT;
			cmyLim = bagH - lh;
			if (cmyLim < 0)
			{
				cmyLim = 0;
			}
			if (cmtoY < 0)
			{
				cmtoY = 0;
			}
			if (cmtoY > cmyLim)
			{
				cmtoY = cmyLim;
			}
			if (cmy < 0)
			{
				cmy = 0;
			}
			if (cmy > cmyLim)
			{
				cmy = cmyLim;
			}
			size_tab = 0;
			newSelected = 0;
			int bodyLen2 = bodyLen;
			if (!invSplitIsBody)
			{
				selected = bodyLen2 + invSplitBagSel + 1;
			}
			else
			{
				selected = invSplitBodySel + 1;
			}
			if (GameCanvas.isTouch && selected < 1)
			{
				selected = -1;
			}
			currentListLength = bodyLen + bagLen + 1;
			if (currentListLength < 1)
			{
				currentListLength = 1;
			}
		}
		catch
		{
		}
	}

	private void InitInventorySplit(bool resetSelect)
	{
		try
		{
			ITEM_HEIGHT = 24;
			if (resetSelect)
			{
				if (GameCanvas.isTouch)
				{
					selected = -1;
				}
				invSplitIsBody = true;
				invSplitBodySel = 0;
				invSplitBagSel = 0;
				cmy = (cmtoY = 0);
				cmRun = 0;
				currItem = null;
				cp = null;
			}
			ClampInventorySplit();
			if (!GameCanvas.isTouch && selected < 1)
			{
				invSplitIsBody = true;
				invSplitBodySel = 0;
				ClampInventorySplit();
			}
		}
		catch
		{
		}
	}

	private Item GetInventorySplitCurrItem()
	{
		try
		{
			Char me = Char.myCharz();
			if (me == null)
			{
				return null;
			}
			if (invSplitIsBody)
			{
				if (me.arrItemBody == null || invSplitBodySel < 0 || invSplitBodySel >= me.arrItemBody.Length)
				{
					return null;
				}
				return me.arrItemBody[invSplitBodySel];
			}
			if (me.arrItemBag == null || invSplitBagSel < 0 || invSplitBagSel >= me.arrItemBag.Length)
			{
				return null;
			}
			return me.arrItemBag[invSplitBagSel];
		}
		catch
		{
			return null;
		}
	}

	private void SyncInventorySplitSelected()
	{
		try
		{
			Char me = Char.myCharz();
			if (me == null)
			{
				return;
			}
			int bodyLen = (me.arrItemBody != null) ? me.arrItemBody.Length : 0;
			if (invSplitIsBody)
			{
				selected = invSplitBodySel + 1;
			}
			else
			{
				selected = bodyLen + invSplitBagSel + 1;
			}
			newSelected = 0;
			lastSelect[currentTabIndex] = selected;
		}
		catch
		{
		}
	}

	private void EnsureInventorySplitBagVisible()
	{
		try
		{
			int lx, lw, rx, rw, ly, lh;
			GetInventorySplitLayout(out lx, out lw, out rx, out rw, out ly, out lh);
			int rowY = ly + invSplitBagSel * ITEM_HEIGHT;
			if (rowY < ly + cmy)
			{
				cmtoY = rowY - ly;
			}
			else if (rowY + ITEM_HEIGHT > ly + cmy + lh)
			{
				cmtoY = rowY + ITEM_HEIGHT - ly - lh;
			}
			if (cmtoY < 0)
			{
				cmtoY = 0;
			}
			if (cmtoY > cmyLim)
			{
				cmtoY = cmyLim;
			}
			cmy = cmtoY;
		}
		catch
		{
		}
	}

	private void DrawInventorySplitRow(mGraphics g, Item item, int x, int y, int w, int h, bool isBody, bool isSelected)
	{
		try
		{
			int iconW = 30;
			if (w < 80)
			{
				iconW = 26;
			}
			int textX = x + iconW;
			int textW = x + w - textX;
			if (textW < 20)
			{
				textW = 20;
			}
			int baseBg = isBody ? 15196114 : 15723751;
			int baseIconBd = isBody ? 9993045 : 11837316;
			g.setColor(isSelected ? 16383818 : baseBg);
			g.fillRect(textX, y, textW, h);
			int iconBg = isSelected ? 9541120 : baseIconBd;
			if (item != null && item.itemOption != null)
			{
				for (int i = 0; i < item.itemOption.Length; i++)
				{
					if (item.itemOption[i] != null && item.itemOption[i].optionTemplate.id == 72 && item.itemOption[i].param > 0)
					{
						sbyte upId = GetColor_Item_Upgrade(item.itemOption[i].param);
						int upBg = GetColor_ItemBg(upId);
						if (upBg != -1)
						{
							iconBg = upBg;
						}
					}
				}
			}
			g.setColor(iconBg);
			g.fillRect(x, y, iconW, h);
			if (item == null)
			{
				return;
			}
			string plus = string.Empty;
			mFont nameFont = mFont.tahoma_7_green2;
			if (item.itemOption != null)
			{
				for (int k = 0; k < item.itemOption.Length; k++)
				{
					if (item.itemOption[k] == null)
					{
						continue;
					}
					if (item.itemOption[k].optionTemplate.id == 72)
					{
						plus = " [+" + item.itemOption[k].param + "]";
					}
					if (item.itemOption[k].optionTemplate.id == 41)
					{
						if (item.itemOption[k].param == 1)
						{
							nameFont = GetFont(0);
						}
						else if (item.itemOption[k].param == 2)
						{
							nameFont = GetFont(2);
						}
						else if (item.itemOption[k].param == 3)
						{
							nameFont = GetFont(8);
						}
						else if (item.itemOption[k].param == 4)
						{
							nameFont = GetFont(7);
						}
					}
				}
			}
			string fullName = item.template.name + plus;
			try
			{
				if (nameFont.getWidth(fullName) > textW - 6)
				{
					string[] sp = nameFont.splitFontArray(fullName, textW - 6);
					if (sp != null && sp.Length > 0)
					{
						fullName = sp[0];
					}
				}
			}
			catch
			{
			}
			nameFont.drawString(g, fullName, textX + 3, y + 1, 0);
			string optStr = string.Empty;
			if (item.itemOption != null)
			{
				if (item.itemOption.Length > 0 && item.itemOption[0] != null && item.itemOption[0].optionTemplate.id != 102 && item.itemOption[0].optionTemplate.id != 107)
				{
					optStr += item.itemOption[0].getOptionString();
				}
				mFont optFont = mFont.tahoma_7_blue;
				if (item.compare < 0 && item.template.type != 5)
				{
					optFont = mFont.tahoma_7_red;
				}
				if (item.itemOption.Length > 1)
				{
					for (int n = 1; n < 2; n++)
					{
						if (n < item.itemOption.Length && item.itemOption[n] != null && item.itemOption[n].optionTemplate.id != 102 && item.itemOption[n].optionTemplate.id != 107)
						{
							optStr = optStr + "," + item.itemOption[n].getOptionString();
						}
					}
				}
				try
				{
					if (optFont.getWidth(optStr) > textW - 6)
					{
						string[] sp2 = optFont.splitFontArray(optStr, textW - 6);
						if (sp2 != null && sp2.Length > 0)
						{
							optStr = sp2[0];
						}
					}
				}
				catch
				{
				}
				optFont.drawString(g, optStr, textX + 3, y + 11, mFont.LEFT);
			}
			SmallImage.drawSmallImage(g, item.template.iconID, x + iconW / 2, y + h / 2, 0, 3);
			if (item.itemOption != null)
			{
				for (int a = 0; a < item.itemOption.Length; a++)
				{
					if (item.itemOption[a] != null)
					{
						paintOptItem(g, item.itemOption[a].optionTemplate.id, item.itemOption[a].param, x, y, iconW, h);
					}
				}
				for (int b = 0; b < item.itemOption.Length; b++)
				{
					if (item.itemOption[b] != null)
					{
						paintOptSlotItem(g, item.itemOption[b].optionTemplate.id, item.itemOption[b].param, x, y, iconW, h);
					}
				}
			}
			if (item.quantity > 1)
			{
				mFont.tahoma_7_yellow.drawString(g, string.Empty + item.quantity, x + iconW, y + h - mFont.tahoma_7_yellow.getHeight(), 1);
			}
		}
		catch
		{
		}
	}

	private void paintInventorySplit(mGraphics g)
	{
		try
		{
			Char me = Char.myCharz();
			if (me == null || me.arrItemBody == null || me.arrItemBag == null)
			{
				return;
			}
			ClampInventorySplit();
			int leftX, leftW, rightX, rightW, listY, listH;
			GetInventorySplitLayout(out leftX, out leftW, out rightX, out rightW, out listY, out listH);
			Item[] arrBody = me.arrItemBody;
			Item[] arrBag = me.arrItemBag;
			g.setColor(13524492);
			g.fillRect(xScroll, yScroll, wScroll, 1);
			int hlY = yScroll + 1;
			bool leftFocus = invSplitIsBody;
			g.setColor(leftFocus ? 16383818 : 15723751);
			g.fillRect(leftX, hlY, leftW, INV_SPLIT_HEADER_H - 2);
			g.setColor(!leftFocus ? 16383818 : 15723751);
			g.fillRect(rightX, hlY, rightW, INV_SPLIT_HEADER_H - 2);
			g.setColor(9993045);
			g.drawRect(leftX, hlY, leftW, INV_SPLIT_HEADER_H - 2);
			g.drawRect(rightX, hlY, rightW, INV_SPLIT_HEADER_H - 2);
			string leftTitle = mResources.trangbi + " (" + arrBody.Length + ")";
			string rightTitle = mResources.inventory[0] + mResources.inventory[1] + " (" + arrBag.Length + ")";
			try
			{
				if (mFont.tahoma_7b_dark.getWidth(leftTitle) > leftW - 4)
				{
					leftTitle = mResources.trangbi;
				}
				if (mFont.tahoma_7b_dark.getWidth(rightTitle) > rightW - 4)
				{
					rightTitle = mResources.inventory[0] + mResources.inventory[1];
				}
			}
			catch
			{
			}
			mFont.tahoma_7b_dark.drawString(g, leftTitle, leftX + leftW / 2, hlY + 3, mFont.CENTER);
			mFont.tahoma_7b_dark.drawString(g, rightTitle, rightX + rightW / 2, hlY + 3, mFont.CENTER);
			g.setColor(13524492);
			g.fillRect(leftX + leftW, hlY, INV_SPLIT_DIV, INV_SPLIT_HEADER_H - 2 + listH + 2);
			g.setClip(leftX, listY, leftW, listH);
			for (int i = 0; i < arrBody.Length; i++)
			{
				int ry = listY + i * ITEM_HEIGHT;
				if (ry > listY + listH || ry < listY - ITEM_HEIGHT)
				{
					continue;
				}
				bool sel = leftFocus && i == invSplitBodySel && (selected != -1 || !GameCanvas.isTouch);
				if (GameCanvas.isTouch && selected == -1)
				{
					sel = leftFocus && i == invSplitBodySel;
				}
				DrawInventorySplitRow(g, arrBody[i], leftX, ry, leftW, ITEM_HEIGHT - 1, true, sel);
			}
			if (arrBody.Length == 0)
			{
				mFont.tahoma_7_grey.drawString(g, "Trong", leftX + leftW / 2, listY + listH / 2 - 4, mFont.CENTER);
			}
			g.setClip(rightX, listY, rightW, listH);
			g.translate(0, -cmy);
			for (int j = 0; j < arrBag.Length; j++)
			{
				int ry2 = listY + j * ITEM_HEIGHT;
				if (ry2 - cmy > listY + listH || ry2 - cmy < listY - ITEM_HEIGHT)
				{
					continue;
				}
				bool sel2 = !leftFocus && j == invSplitBagSel && (selected != -1 || !GameCanvas.isTouch);
				if (GameCanvas.isTouch && selected == -1)
				{
					sel2 = !leftFocus && j == invSplitBagSel;
				}
				DrawInventorySplitRow(g, arrBag[j], rightX, ry2, rightW, ITEM_HEIGHT - 1, false, sel2);
			}
			g.translate(0, cmy);
			if (arrBag.Length == 0)
			{
				mFont.tahoma_7_grey.drawString(g, "Trong", rightX + rightW / 2, listY + listH / 2 - 4, mFont.CENTER);
			}
			g.setClip(xScroll, yScroll, wScroll, hScroll);
			if (cmy > 0)
			{
				g.drawRegion(Mob.imgHP, 0, 0, 9, 6, 1, rightX + rightW - 12, listY + 3, 0);
			}
			if (cmy < cmyLim)
			{
				g.drawRegion(Mob.imgHP, 0, 0, 9, 6, 0, rightX + rightW - 12, listY + listH - 8, 0);
			}
		}
		catch (Exception)
		{
		}
	}

	private void UpdateInventorySplit()
	{
		try
		{
			Char me = Char.myCharz();
			if (me == null || me.arrItemBody == null || me.arrItemBag == null)
			{
				return;
			}
			ClampInventorySplit();
			int leftX, leftW, rightX, rightW, listY, listH;
			GetInventorySplitLayout(out leftX, out leftW, out rightX, out rightW, out listY, out listH);
			int bodyLen = me.arrItemBody.Length;
			int bagLen = me.arrItemBag.Length;
			bool handledKey = false;
			if (GameCanvas.keyPressed[23] || GameCanvas.keyPressed[4])
			{
				invSplitIsBody = true;
				handledKey = true;
				SoundMn.gI().panelClick();
				SyncInventorySplitSelected();
				currItem = GetInventorySplitCurrItem();
				if (currItem != null)
				{
					currItem.compare = getCompare(currItem);
				}
			}
			else if (GameCanvas.keyPressed[24] || GameCanvas.keyPressed[6])
			{
				invSplitIsBody = false;
				handledKey = true;
				SoundMn.gI().panelClick();
				SyncInventorySplitSelected();
				currItem = GetInventorySplitCurrItem();
				if (currItem != null)
				{
					currItem.compare = getCompare(currItem);
				}
				EnsureInventorySplitBagVisible();
			}
			if (GameCanvas.keyPressed[21] || GameCanvas.keyPressed[2])
			{
				handledKey = true;
				if (invSplitIsBody)
				{
					invSplitBodySel--;
					if (invSplitBodySel < 0)
					{
						invSplitBodySel = 0;
					}
				}
				else
				{
					invSplitBagSel--;
					if (invSplitBagSel < 0)
					{
						invSplitBagSel = 0;
					}
					EnsureInventorySplitBagVisible();
				}
				SyncInventorySplitSelected();
				currItem = GetInventorySplitCurrItem();
				if (currItem != null)
				{
					currItem.compare = getCompare(currItem);
				}
			}
			else if (GameCanvas.keyPressed[22] || GameCanvas.keyPressed[8])
			{
				handledKey = true;
				if (invSplitIsBody)
				{
					invSplitBodySel++;
					if (invSplitBodySel >= bodyLen)
					{
						invSplitBodySel = bodyLen - 1;
					}
					if (invSplitBodySel < 0)
					{
						invSplitBodySel = 0;
					}
				}
				else
				{
					invSplitBagSel++;
					if (invSplitBagSel >= bagLen)
					{
						invSplitBagSel = bagLen - 1;
					}
					if (invSplitBagSel < 0)
					{
						invSplitBagSel = 0;
					}
					EnsureInventorySplitBagVisible();
				}
				SyncInventorySplitSelected();
				currItem = GetInventorySplitCurrItem();
				if (currItem != null)
				{
					currItem.compare = getCompare(currItem);
				}
			}
		if ((GameCanvas.keyPressed[5] || GameCanvas.keyPressed[25] || GameCanvas.keyPressed[12]) && GetInventorySplitCurrItem() != null)
		{
			currItem = GetInventorySplitCurrItem();
			pointerDownTime = 0;
			waitToPerform = 2;
		}
		if (pointerIsDowning && !GameCanvas.isPointerDown && !GameCanvas.isPointerJustRelease)
		{
			pointerIsDowning = false;
			pointerDownTime = 0;
			cmtoY = cmy;
		}
		if (GameCanvas.isPointerDown)
			{
				justRelease = false;
				bool inRight = GameCanvas.isPointer(rightX, listY, rightW, listH);
				bool inLeft = GameCanvas.isPointer(leftX, listY, leftW, listH);
				if (!pointerIsDowning && (inRight || inLeft))
				{
					for (int i = 0; i < pointerDownLastX.Length; i++)
					{
						pointerDownLastX[0] = GameCanvas.py;
					}
					pointerDownFirstX = GameCanvas.py;
					pointerIsDowning = true;
					isDownWhenRunning = cmRun != 0;
					cmRun = 0;
				}
				else if (pointerIsDowning)
				{
					pointerDownTime++;
					int dy = GameCanvas.py - pointerDownLastX[0];
					if (!invSplitIsBody || GameCanvas.isPointer(rightX, listY, rightW, listH))
					{
						if (dy != 0)
						{
							cmtoY -= dy;
							if (cmtoY < 0)
							{
								cmtoY = 0;
							}
							if (cmtoY > cmyLim)
							{
								cmtoY = cmyLim;
							}
							if (cmy < 0 || cmy > cmyLim)
							{
								dy /= 2;
							}
							cmy -= dy;
							if (cmy < 0)
							{
								cmy = 0;
							}
							if (cmy > cmyLim)
							{
								cmy = cmyLim;
							}
						}
					}
					for (int n2 = pointerDownLastX.Length - 1; n2 > 0; n2--)
					{
						pointerDownLastX[n2] = pointerDownLastX[n2 - 1];
					}
				pointerDownLastX[0] = GameCanvas.py;
				if (dy != 0 && selected >= 0)
				{
					selected = -1;
				}
				}
			}
			if (GameCanvas.isPointerJustRelease && pointerIsDowning)
			{
				justRelease = true;
				int dy2 = GameCanvas.py - pointerDownLastX[0];
				GameCanvas.isPointerJustRelease = false;
				bool isTap = Res.abs(dy2) < 20 && Res.abs(GameCanvas.py - pointerDownFirstX) < 20 && !isDownWhenRunning;
				if (isTap)
				{
					cmRun = 0;
					cmtoY = cmy;
					pointerDownFirstX = -1000;
					int px = GameCanvas.px;
					int py = GameCanvas.py;
					bool hit = false;
					if (px >= leftX && px < leftX + leftW && py >= listY && py < listY + listH)
					{
						int idx = (py - listY) / ITEM_HEIGHT;
						if (idx >= 0 && idx < bodyLen)
						{
							invSplitIsBody = true;
							invSplitBodySel = idx;
							SyncInventorySplitSelected();
							currItem = GetInventorySplitCurrItem();
							if (currItem != null)
							{
								currItem.compare = getCompare(currItem);
							}
							hit = true;
						}
					}
					else if (px >= rightX && px < rightX + rightW && py >= listY && py < listY + listH)
					{
						int idx2 = (cmy + py - listY) / ITEM_HEIGHT;
						if (idx2 >= 0 && idx2 < bagLen)
						{
							invSplitIsBody = false;
							invSplitBagSel = idx2;
							SyncInventorySplitSelected();
							EnsureInventorySplitBagVisible();
							currItem = GetInventorySplitCurrItem();
							if (currItem != null)
							{
								currItem.compare = getCompare(currItem);
							}
							hit = true;
						}
					}
					if (hit)
					{
						pointerDownTime = 0;
						waitToPerform = 10;
						SoundMn.gI().panelClick();
					}
				}
				else
				{
					if (cmy < 0)
					{
						cmtoY = 0;
					}
					else if (cmy > cmyLim)
					{
						cmtoY = cmyLim;
					}
					else
					{
						int fl = GameCanvas.py - pointerDownLastX[0] + (pointerDownLastX[0] - pointerDownLastX[1]) + (pointerDownLastX[1] - pointerDownLastX[2]);
						fl = ((fl > 10) ? 10 : ((fl < -10) ? (-10) : 0));
						cmRun = -fl * 100;
					}
				}
				pointerIsDowning = false;
				pointerDownTime = 0;
				GameCanvas.isPointerJustRelease = false;
			}
			if (handledKey)
			{
				cmtoY = cmy;
				if (cmtoY > cmyLim)
				{
					cmtoY = cmyLim;
				}
				if (cmtoY < 0)
				{
					cmtoY = 0;
				}
			}
		}
		catch
		{
		}
	}

	private int GetInventorySplitMenuY()
	{
		try
		{
			int leftX, leftW, rightX, rightW, listY, listH;
			GetInventorySplitLayout(out leftX, out leftW, out rightX, out rightW, out listY, out listH);
			if (invSplitIsBody)
			{
				return listY + invSplitBodySel * ITEM_HEIGHT + ITEM_HEIGHT / 2;
			}
			return listY + invSplitBagSel * ITEM_HEIGHT - cmy + ITEM_HEIGHT / 2;
		}
		catch
		{
			return yScroll;
		}
	}

	private void DoFireInventorySplit()
	{
		try
		{
			if (GameCanvas.menu != null && GameCanvas.menu.showMenu)
			{
				return;
			}if (Char.myCharz().statusMe == 14)
			{
				GameCanvas.startOKDlg(mResources.can_not_do_when_die);
				return;
			}
			ClampInventorySplit();
			Item ci = GetInventorySplitCurrItem();
			if (ci == null)
			{
				cp = null;
				currItem = null;
				return;
			}
			currItem = ci;
			SyncInventorySplitSelected();
			MyVector menu = new MyVector();
			if (invSplitIsBody)
			{
				menu.addElement(new Command(mResources.GETOUT, this, 2002, currItem));
			}
			else
			{
				if (GameCanvas.panel.type == 12)
				{
					menu.addElement(new Command(mResources.use_for_combine, this, 6000, currItem));
				}
				else if (GameCanvas.panel.type == 13)
				{
					menu.addElement(new Command(mResources.use_for_trade, this, 7000, currItem));
				}
				else if (currItem.isTypeBody())
				{
					menu.addElement(new Command(mResources.USE, this, 2000, currItem));
					if (Char.myCharz().havePet)
					{
						menu.addElement(new Command(mResources.MOVEFORPET, this, 2005, currItem));
					}
				}
				else
				{
					menu.addElement(new Command(mResources.USE, this, 2001, currItem));
				}
			}
			Char.myCharz().setPartTemp(currItem.headTemp, currItem.bodyTemp, currItem.legTemp, currItem.bagTemp);
			if (GameCanvas.panel.type != 12 && GameCanvas.panel.type != 13)
			{
				if (position == 0)
				{
					menu.addElement(new Command(mResources.MOVEOUT, this, 2003, currItem));
				}
				if (position == 1)
				{
					menu.addElement(new Command(mResources.SALE, this, 3002, currItem));
				}
			}
			int menuY = GetInventorySplitMenuY();
			if (menuY < yScroll)
			{
				menuY = yScroll;
			}
			if (menuY > yScroll + hScroll - 20)
			{
				menuY = yScroll + hScroll - 20;
			}
			GameCanvas.menu.startAt(menu, X, menuY);
			addItemDetail(currItem);
		}
		catch (Exception ex)
		{
			Res.outz("SplitFire ex " + ex.StackTrace);
		}
	}
}
