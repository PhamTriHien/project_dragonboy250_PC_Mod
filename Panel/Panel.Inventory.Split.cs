using System;

/// <summary>
/// Thành phần phân mảnh (partial) của lớp Panel: Đảm nhiệm phân hệ chuyên trách Inventory (Split).
/// </summary>
public partial class Panel : IActionListener, IChatable
{
	private bool invSplitIsBody = true;

	private int invSplitBodySel = 0;

	private int invSplitBagSel = 0;

	private int cmyBody = 0;

	private int cmtoYBody = 0;

	private int cmyLimBody = 0;

	private const int INV_SPLIT_HEADER_H = 18;

	private const int INV_SPLIT_DIV = 2;

	private const int BODY_SLOT_SIZE = 32;

	private const int BODY_SLOT_GAP = 5;

	private static readonly string[] bodySlotGhostNames = new string[]
	{
		"Áo",
		"Quần",
		"Găng",
		"Giày",
		"Rada",
		"C.Trang",
		"Giáp",
		"Sách",
		"P.Kiện",
		"T.Cưỡi",
		"Pet",
		"D.Hiệu",
		"C.Mệnh",
		"B.Tai",
		"Đai"
	};

	/// <summary>
	/// Truy xuất nhãn tên ô trang bị theo chỉ số slotIdx.
	/// </summary>
	internal static string GetBodySlotGhostName(int slotIdx)
	{
		if (slotIdx >= 0 && slotIdx < bodySlotGhostNames.Length)
		{
			return bodySlotGhostNames[slotIdx];
		}
		return "Ô " + (slotIdx + 1);
	}

	/// <summary>
	/// Tính toán dòng và cột hiển thị của ô trang bị trên khung nhân vật (Rows 0..2 hai bên nhân vật, Rows 3+ lưới 4 cột phía dưới).
	/// </summary>
	private static void GetBodySlotGridPos(int slotIdx, out int row, out int col)
	{
		switch (slotIdx)
		{
		case 0:
			row = 0;
			col = 0;
			return;
		case 1:
			row = 1;
			col = 0;
			return;
		case 2:
			row = 0;
			col = 3;
			return;
		case 3:
			row = 2;
			col = 0;
			return;
		case 4:
			row = 1;
			col = 3;
			return;
		case 5:
			row = 2;
			col = 3;
			return;
		default:
		{
			int extra = slotIdx - 6;
			if (extra < 0)
			{
				extra = 0;
			}
			row = 3 + extra / 4;
			col = extra % 4;
			return;
		}
		}
	}

	/// <summary>
	/// Tính toán kích thước và toạ độ lưới Paperdoll bên trái đảm bảo hiển thị đầy đủ 100% số ô trong arrItemBody.
	/// </summary>
	private static void GetPaperdollMetrics(int leftX, int leftW, int listY, int listH, int bodyLen, out int slotSize, out int rowGap, out int startY, out int totalRows, out int totalBodyH)
	{
		int extraCount = (bodyLen > 6) ? (bodyLen - 6) : 0;
		int extraRows = (extraCount + 3) / 4;
		totalRows = 3 + extraRows;
		rowGap = (totalRows >= 5) ? 3 : 4;
		int maxSlotByW = (leftW - 8 - 6) / 4;
		if (maxSlotByW > 32)
		{
			maxSlotByW = 32;
		}
		int fitSlotByH = (listH - 6 - (totalRows - 1) * rowGap) / totalRows;
		int targetSlot = (totalRows >= 5) ? 28 : 30;
		if (fitSlotByH >= 32 && totalRows <= 4)
		{
			targetSlot = 32;
		}
		else if (fitSlotByH >= 30)
		{
			targetSlot = 30;
		}
		else if (fitSlotByH >= 26)
		{
			targetSlot = fitSlotByH;
		}
		else
		{
			targetSlot = 26;
		}
		slotSize = (targetSlot < maxSlotByW) ? targetSlot : maxSlotByW;
		if (slotSize < 24)
		{
			slotSize = 24;
		}
		int slotsH = totalRows * slotSize + (totalRows - 1) * rowGap;
		totalBodyH = slotsH + 6;
		if (totalBodyH <= listH)
		{
			startY = listY + (listH - slotsH) / 2;
		}
		else
		{
			startY = listY + 3;
		}
		if (startY < listY + 3)
		{
			startY = listY + 3;
		}
	}

	/// <summary>
	/// Tính toán toạ độ vẽ (sx, sy) của ô trang bị slotIdx trên khung bên trái.
	/// </summary>
	private static void GetBodySlotRect(int slotIdx, int leftX, int leftW, int startY, int slotSize, int rowGap, out int sx, out int sy)
	{
		int row, col;
		GetBodySlotGridPos(slotIdx, out row, out col);
		int sidePad = 5;
		if (leftW - sidePad * 2 < 4 * slotSize + 6)
		{
			sidePad = (leftW - (4 * slotSize + 6)) / 2;
			if (sidePad < 2)
			{
				sidePad = 2;
			}
		}
		int availSpan = leftW - sidePad * 2 - slotSize;
		if (availSpan < 0)
		{
			availSpan = 0;
		}
		sx = leftX + sidePad + (availSpan * col) / 3;
		sy = startY + row * (slotSize + rowGap);
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của GetBagGridLayout.
	/// </summary>
	private static void GetBagGridLayout(int rightW, out int cols, out int slotW, out int slotH, out int gap, out int startPad)
	{
		cols = (rightW >= 190) ? 5 : 4;
		gap = 2;
		int pad = 2;
		int availW = rightW - pad * 2 - (cols - 1) * gap;
		slotW = availW / cols;
		int remainder = availW % cols;
		startPad = pad + remainder / 2;
		slotH = slotW;
	}

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của IsInventorySplit.
	/// </summary>
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

	/// <summary>
	/// Truy xuất và trả về dữ liệu của GetInventorySplitLayout.
	/// </summary>
	private void GetInventorySplitLayout(out int leftX, out int leftW, out int rightX, out int rightW, out int listY, out int listH)
	{
		int totalW = wScroll;
		leftW = totalW * 44 / 100;
		if (leftW < 136)
		{
			leftW = 136;
		}
		if (leftW > 185)
		{
			leftW = 185;
		}
		if (totalW <= 220)
		{
			leftW = totalW / 2 - 2;
		}
		leftX = xScroll;
		rightX = leftX + leftW + INV_SPLIT_DIV;
		rightW = xScroll + totalW - rightX;
		if (rightW < 80)
		{
			rightW = 80;
		}
		listY = yScroll + INV_SPLIT_HEADER_H + 2;
		listH = hScroll - INV_SPLIT_HEADER_H - 2;
		if (listH < 24)
		{
			listH = 24;
		}
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của GetItemUpgradeLevel.
	/// </summary>
	private static int GetItemUpgradeLevel(Item item)
	{
		if (item == null || item.itemOption == null)
		{
			return 0;
		}
		for (int i = 0; i < item.itemOption.Length; i++)
		{
			if (item.itemOption[i] != null && item.itemOption[i].optionTemplate.id == 72)
			{
				return item.itemOption[i].param;
			}
		}
		return 0;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ ClampInventorySplit.
	/// </summary>
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
			int slotSize, rowGap, startY, totalRows, totalBodyH;
			GetPaperdollMetrics(lx, lw, ly, lh, bodyLen, out slotSize, out rowGap, out startY, out totalRows, out totalBodyH);
			cmyLimBody = totalBodyH - lh;
			if (cmyLimBody < 0)
			{
				cmyLimBody = 0;
			}
			if (cmtoYBody < 0)
			{
				cmtoYBody = 0;
			}
			if (cmtoYBody > cmyLimBody)
			{
				cmtoYBody = cmyLimBody;
			}
			if (cmyBody < 0)
			{
				cmyBody = 0;
			}
			if (cmyBody > cmyLimBody)
			{
				cmyBody = cmyLimBody;
			}
			int cols, slotW, slotH, gap, startPad;
			GetBagGridLayout(rw, out cols, out slotW, out slotH, out gap, out startPad);
			int bagRows = (bagLen + cols - 1) / cols;
			int totalBagH = bagRows * (slotH + gap) + 6;
			cmyLim = totalBagH - lh;
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
			if (!invSplitIsBody)
			{
				selected = bodyLen + invSplitBagSel + 1;
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

	/// <summary>
	/// Khởi tạo và nạp dữ liệu cấu hình cho InitInventorySplit.
	/// </summary>
	private void InitInventorySplit(bool resetSelect)
	{
		try
		{
			ITEM_HEIGHT = 34;
			if (resetSelect)
			{
				if (GameCanvas.isTouch)
				{
					selected = -1;
				}
				invSplitIsBody = false;
				invSplitBodySel = 0;
				invSplitBagSel = 0;
				cmy = (cmtoY = 0);
				cmyBody = (cmtoYBody = 0);
				cmRun = 0;
				currItem = null;
				cp = null;
			}
			ClampInventorySplit();
			if (!GameCanvas.isTouch && selected < 1)
			{
				invSplitIsBody = false;
				invSplitBagSel = 0;
				ClampInventorySplit();
			}
		}
		catch
		{
		}
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của GetInventorySplitCurrItem.
	/// </summary>
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

	/// <summary>
	/// Thực thi phương thức nghiệp vụ SyncInventorySplitSelected.
	/// </summary>
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

	/// <summary>
	/// Đảm bảo ô trang bị đang chọn trên khung trái luôn nằm trong vùng nhìn thấy.
	/// </summary>
	private void EnsureInventorySplitBodyVisible()
	{
		try
		{
			Char me = Char.myCharz();
			if (me == null || me.arrItemBody == null)
			{
				return;
			}
			int lx, lw, rx, rw, ly, lh;
			GetInventorySplitLayout(out lx, out lw, out rx, out rw, out ly, out lh);
			int slotSize, rowGap, startY, totalRows, totalBodyH;
			GetPaperdollMetrics(lx, lw, ly, lh, me.arrItemBody.Length, out slotSize, out rowGap, out startY, out totalRows, out totalBodyH);
			int sx, sy;
			GetBodySlotRect(invSplitBodySel, lx, lw, startY, slotSize, rowGap, out sx, out sy);
			if (sy < ly + cmyBody)
			{
				cmtoYBody = sy - ly - 2;
			}
			else if (sy + slotSize > ly + cmyBody + lh)
			{
				cmtoYBody = (sy + slotSize) - (ly + lh) + 2;
			}
			if (cmtoYBody < 0)
			{
				cmtoYBody = 0;
			}
			if (cmtoYBody > cmyLimBody)
			{
				cmtoYBody = cmyLimBody;
			}
			cmyBody = cmtoYBody;
		}
		catch
		{
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ EnsureInventorySplitBagVisible.
	/// </summary>
	private void EnsureInventorySplitBagVisible()
	{
		try
		{
			int lx, lw, rx, rw, ly, lh;
			GetInventorySplitLayout(out lx, out lw, out rx, out rw, out ly, out lh);
			int cols, slotW, slotH, gap, startPad;
			GetBagGridLayout(rw, out cols, out slotW, out slotH, out gap, out startPad);
			int row = invSplitBagSel / cols;
			int rowY = ly + 2 + row * (slotH + gap);
			if (rowY < ly + cmy)
			{
				cmtoY = row * (slotH + gap);
			}
			else if (rowY + slotH > ly + cmy + lh)
			{
				cmtoY = (row + 1) * (slotH + gap) - lh + 2;
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

	/// <summary>
	/// Thực hiện vẽ và hiển thị thành phần DrawEquipmentPaperdollSlot lên canvas đồ họa.
	/// </summary>
	private void DrawEquipmentPaperdollSlot(mGraphics g, Item item, int slotIdx, string ghostName, int x, int y, int size, bool isSelected)
	{
		try
		{
			int baseBg = isSelected ? 16383818 : ((item != null) ? 15723751 : 15196114);
			int upLv = GetItemUpgradeLevel(item);
			if (item != null && !isSelected && upLv > 0)
			{
				sbyte upId = GetColor_Item_Upgrade(upLv);
				int upBg = GetColor_ItemBg(upId);
				if (upBg != -1)
				{
					baseBg = upBg;
				}
			}
			g.setColor(baseBg);
			g.fillRect(x, y, size, size);
			g.setColor(isSelected ? 13524492 : 11837316);
			g.drawRect(x + 1, y + 1, size - 3, size - 3);
			g.setColor(isSelected ? 13524492 : 9993045);
			g.drawRect(x, y, size - 1, size - 1);
			if (isSelected)
			{
				g.setColor(16383818);
				g.drawRect(x + 1, y + 1, size - 3, size - 3);
			}
			if (item == null)
			{
				mFont.tahoma_7_grey.drawString(g, ghostName, x + size / 2, y + size / 2 - 4, mFont.CENTER);
				return;
			}
			SmallImage.drawSmallImage(g, item.template.iconID, x + size / 2, y + size / 2, 0, 3);
			if (item.itemOption != null)
			{
				for (int a = 0; a < item.itemOption.Length; a++)
				{
					if (item.itemOption[a] != null)
					{
						paintOptItem(g, item.itemOption[a].optionTemplate.id, item.itemOption[a].param, x, y, size, size);
						paintOptSlotItem(g, item.itemOption[a].optionTemplate.id, item.itemOption[a].param, x, y, size, size);
					}
				}
			}
			if (upLv > 0)
			{
				mFont.tahoma_7b_green.drawString(g, "+" + upLv, x + size - 2, y + 1, mFont.RIGHT);
			}
			if (item.quantity > 1)
			{
				string qStr = (item.quantity > 999) ? (item.quantity / 1000 + "k") : (string.Empty + item.quantity);
				mFont.tahoma_7_yellow.drawString(g, qStr, x + size - 2, y + size - 8, mFont.RIGHT);
			}
		}
		catch
		{
		}
	}

	/// <summary>
	/// Thực hiện vẽ và hiển thị thành phần DrawGridBagSlot lên canvas đồ họa.
	/// </summary>
	private void DrawGridBagSlot(mGraphics g, Item item, int x, int y, int w, int h, bool isSelected)
	{
		try
		{
			int baseBg = isSelected ? 16383818 : ((item != null) ? 15723751 : 15196114);
			int upLv = GetItemUpgradeLevel(item);
			if (item != null && !isSelected && upLv > 0)
			{
				sbyte upId = GetColor_Item_Upgrade(upLv);
				int upBg = GetColor_ItemBg(upId);
				if (upBg != -1)
				{
					baseBg = upBg;
				}
			}
			g.setColor(baseBg);
			g.fillRect(x, y, w, h);
			g.setColor(isSelected ? 13524492 : 11837316);
			g.drawRect(x + 1, y + 1, w - 3, h - 3);
			g.setColor(isSelected ? 13524492 : 9993045);
			g.drawRect(x, y, w - 1, h - 1);
			if (isSelected)
			{
				g.setColor(16383818);
				g.drawRect(x + 1, y + 1, w - 3, h - 3);
			}
			if (item == null)
			{
				return;
			}
			SmallImage.drawSmallImage(g, item.template.iconID, x + w / 2, y + h / 2, 0, 3);
			if (item.itemOption != null)
			{
				for (int a = 0; a < item.itemOption.Length; a++)
				{
					if (item.itemOption[a] != null)
					{
						paintOptItem(g, item.itemOption[a].optionTemplate.id, item.itemOption[a].param, x, y, w, h);
						paintOptSlotItem(g, item.itemOption[a].optionTemplate.id, item.itemOption[a].param, x, y, w, h);
					}
				}
			}
			if (upLv > 0)
			{
				mFont.tahoma_7b_green.drawString(g, "+" + upLv, x + w - 2, y + 1, mFont.RIGHT);
			}
			if (item.quantity > 1)
			{
				string qStr = (item.quantity > 999) ? (item.quantity / 1000 + "k") : (string.Empty + item.quantity);
				mFont.tahoma_7_yellow.drawString(g, qStr, x + w - 2, y + h - 8, mFont.RIGHT);
			}
			if (item.newItem && GameCanvas.gameTick % 10 > 4)
			{
				if (imgNew != null)
				{
					g.drawImage(imgNew, x + 2, y + 2, 0);
				}
				else
				{
					g.setColor(16711680);
					g.fillRect(x + 2, y + 2, 4, 4);
				}
			}
		}
		catch
		{
		}
	}

	/// <summary>
	/// Thực hiện vẽ và hiển thị thành phần paintInventorySplit lên canvas đồ họa.
	/// </summary>
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
			int bodyLen = arrBody.Length;
			int hlY = yScroll + 1;
			bool leftFocus = invSplitIsBody;

			g.setColor(13524492);
			g.fillRect(xScroll, yScroll, wScroll, 1);

			g.setColor(leftFocus ? 16383818 : 15723751);
			g.fillRect(leftX, hlY, leftW, INV_SPLIT_HEADER_H - 2);
			g.setColor(9993045);
			g.drawRect(leftX, hlY, leftW, INV_SPLIT_HEADER_H - 2);

			g.setColor(!leftFocus ? 16383818 : 15723751);
			g.fillRect(rightX, hlY, rightW, INV_SPLIT_HEADER_H - 2);
			g.setColor(9993045);
			g.drawRect(rightX, hlY, rightW, INV_SPLIT_HEADER_H - 2);

			string heroTitle = (me.cName != null && me.cName.Length > 0) ? me.cName : mResources.trangbi;
			if (mFont.tahoma_7b_dark.getWidth(heroTitle) > leftW - 6)
			{
				heroTitle = mResources.trangbi;
			}
			mFont.tahoma_7b_dark.drawString(g, heroTitle, leftX + leftW / 2, hlY + 3, mFont.CENTER);

			int usedCount = 0;
			for (int b = 0; b < arrBag.Length; b++)
			{
				if (arrBag[b] != null)
				{
					usedCount++;
				}
			}
			mFont.tahoma_7b_dark.drawString(g, "HÀNH TRANG", rightX + 8, hlY + 3, mFont.LEFT);
			mFont.tahoma_7_grey.drawString(g, usedCount + "/" + arrBag.Length, rightX + rightW - 8, hlY + 3, mFont.RIGHT);

			g.setColor(13524492);
			g.fillRect(leftX + leftW, hlY, INV_SPLIT_DIV, INV_SPLIT_HEADER_H - 2 + listH + 2);

			g.setColor(15196114);
			g.fillRect(leftX, listY, leftW, listH);
			g.setColor(9993045);
			g.drawRect(leftX, listY, leftW, listH);

			int slotSize, rowGap, startY, totalRows, totalBodyH;
			GetPaperdollMetrics(leftX, leftW, listY, listH, bodyLen, out slotSize, out rowGap, out startY, out totalRows, out totalBodyH);
			int avatarX = leftX + leftW / 2;
			int avatarFeetY = startY + 2 * (slotSize + rowGap) + slotSize - 2;

			g.setClip(leftX + 1, listY + 1, leftW - 2, listH - 2);
			g.translate(0, -cmyBody);

			g.setColor(11837316);
			g.fillRoundRect(avatarX - 22, avatarFeetY - 3, 44, 7, 4, 4);
			g.setColor(9993045);
			g.drawRoundRect(avatarX - 22, avatarFeetY - 3, 44, 7, 4, 4);

			try
			{
				me.paintCharBody(g, avatarX, avatarFeetY, 1, 0, isPaintBag: true);
			}
			catch
			{
			}

			for (int s = 0; s < bodyLen; s++)
			{
				int sx, sy;
				GetBodySlotRect(s, leftX, leftW, startY, slotSize, rowGap, out sx, out sy);
				if (sy - cmyBody > listY + listH || sy - cmyBody < listY - slotSize)
				{
					continue;
				}
				bool isSel = leftFocus && (s == invSplitBodySel);
				Item bItem = arrBody[s];
				string ghostName = GetBodySlotGhostName(s);
				DrawEquipmentPaperdollSlot(g, bItem, s, ghostName, sx, sy, slotSize, isSel);
			}

			g.translate(0, cmyBody);
			g.setClip(xScroll, yScroll, wScroll, hScroll);

			if (cmyLimBody > 0)
			{
				int trackHBody = listH - 8;
				int thumbHBody = trackHBody * listH / totalBodyH;
				if (thumbHBody < 14)
				{
					thumbHBody = 14;
				}
				int thumbYBody = listY + 4 + (cmyBody * (trackHBody - thumbHBody) / cmyLimBody);
				g.setColor(15196114);
				g.fillRect(leftX + leftW - 3, listY + 4, 2, trackHBody);
				g.setColor(13524492);
				g.fillRect(leftX + leftW - 3, thumbYBody, 2, thumbHBody);
			}

			g.setColor(15196114);
			g.fillRect(rightX, listY, rightW, listH);
			g.setColor(9993045);
			g.drawRect(rightX, listY, rightW, listH);

			int cols, slotW, slotH, gap, startPad;
			GetBagGridLayout(rightW, out cols, out slotW, out slotH, out gap, out startPad);
			int gridStartX = rightX + startPad;

			g.setClip(rightX + 1, listY + 1, rightW - 2, listH - 2);
			g.translate(0, -cmy);

			int bagLen = arrBag.Length;
			for (int j = 0; j < bagLen; j++)
			{
				int row = j / cols;
				int col = j % cols;
				int sx2 = gridStartX + col * (slotW + gap);
				int sy2 = listY + 2 + row * (slotH + gap);
				if (sy2 - cmy > listY + listH || sy2 - cmy < listY - slotH)
				{
					continue;
				}
				bool sel2 = !leftFocus && (j == invSplitBagSel);
				DrawGridBagSlot(g, arrBag[j], sx2, sy2, slotW, slotH, sel2);
			}

			g.translate(0, cmy);
			g.setClip(xScroll, yScroll, wScroll, hScroll);

			if (cmyLim > 0)
			{
				int trackH = listH - 8;
				int bagRows = (bagLen + cols - 1) / cols;
				int totalBagH = bagRows * (slotH + gap) + 6;
				int thumbH = trackH * listH / totalBagH;
				if (thumbH < 14)
				{
					thumbH = 14;
				}
				int thumbY = listY + 4 + (cmy * (trackH - thumbH) / cmyLim);
				g.setColor(15196114);
				g.fillRect(rightX + rightW - 4, listY + 4, 3, trackH);
				g.setColor(13524492);
				g.fillRect(rightX + rightW - 4, thumbY, 3, thumbH);
			}
		}
		catch (Exception)
		{
		}
	}

	/// <summary>
	/// Cập nhật trạng thái logic của UpdateInventorySplit theo chu kỳ khung hình.
	/// </summary>
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
			int cols, slotW, slotH, gap, startPad;
			GetBagGridLayout(rightW, out cols, out slotW, out slotH, out gap, out startPad);
			int gridStartX = rightX + startPad;
			int slotSize, rowGap, startY, totalRows, totalBodyH;
			GetPaperdollMetrics(leftX, leftW, listY, listH, bodyLen, out slotSize, out rowGap, out startY, out totalRows, out totalBodyH);
			bool handledKey = false;

			if (GameCanvas.keyPressed[23] || GameCanvas.keyPressed[4])
			{
				handledKey = true;
				if (!invSplitIsBody)
				{
					int col = invSplitBagSel % cols;
					if (col > 0)
					{
						invSplitBagSel--;
					}
					else if (bodyLen > 0)
					{
						invSplitIsBody = true;
						int r = invSplitBagSel / cols;
						if (r == 0)
						{
							invSplitBodySel = (bodyLen > 2) ? 2 : 0;
						}
						else if (r == 1)
						{
							invSplitBodySel = (bodyLen > 4) ? 4 : ((bodyLen > 1) ? 1 : 0);
						}
						else if (r == 2)
						{
							invSplitBodySel = (bodyLen > 5) ? 5 : ((bodyLen > 3) ? 3 : 0);
						}
						else
						{
							int targetIdx = 6 + (r - 3) * 4 + 3;
							if (targetIdx >= bodyLen)
							{
								targetIdx = bodyLen - 1;
							}
							if (targetIdx < 0)
							{
								targetIdx = 0;
							}
							invSplitBodySel = targetIdx;
						}
					}
				}
				else
				{
					if (invSplitBodySel == 2)
					{
						invSplitBodySel = 0;
					}
					else if (invSplitBodySel == 4)
					{
						invSplitBodySel = (bodyLen > 1) ? 1 : 0;
					}
					else if (invSplitBodySel == 5)
					{
						invSplitBodySel = (bodyLen > 3) ? 3 : ((bodyLen > 1) ? 1 : 0);
					}
					else if (invSplitBodySel >= 6)
					{
						int extraCol = (invSplitBodySel - 6) % 4;
						if (extraCol > 0)
						{
							invSplitBodySel--;
						}
					}
				}
				SoundMn.gI().panelClick();
				SyncInventorySplitSelected();
				if (invSplitIsBody)
				{
					EnsureInventorySplitBodyVisible();
				}
				else
				{
					EnsureInventorySplitBagVisible();
				}
				currItem = GetInventorySplitCurrItem();
				if (currItem != null)
				{
					currItem.compare = getCompare(currItem);
					addItemDetail(currItem);
				}
			}
			else if (GameCanvas.keyPressed[24] || GameCanvas.keyPressed[6])
			{
				handledKey = true;
				if (invSplitIsBody)
				{
					if (invSplitBodySel == 0 && bodyLen > 2)
					{
						invSplitBodySel = 2;
					}
					else if (invSplitBodySel == 1 && bodyLen > 4)
					{
						invSplitBodySel = 4;
					}
					else if (invSplitBodySel == 3 && bodyLen > 5)
					{
						invSplitBodySel = 5;
					}
					else if (invSplitBodySel >= 6 && ((invSplitBodySel - 6) % 4) < 3 && invSplitBodySel + 1 < bodyLen)
					{
						invSplitBodySel++;
					}
					else if (bagLen > 0)
					{
						int bRow, bCol;
						GetBodySlotGridPos(invSplitBodySel, out bRow, out bCol);
						invSplitIsBody = false;
						invSplitBagSel = bRow * cols;
						if (invSplitBagSel >= bagLen)
						{
							invSplitBagSel = bagLen - 1;
						}
					}
				}
				else
				{
					if (invSplitBagSel < bagLen - 1)
					{
						invSplitBagSel++;
					}
				}
				SoundMn.gI().panelClick();
				SyncInventorySplitSelected();
				if (invSplitIsBody)
				{
					EnsureInventorySplitBodyVisible();
				}
				else
				{
					EnsureInventorySplitBagVisible();
				}
				currItem = GetInventorySplitCurrItem();
				if (currItem != null)
				{
					currItem.compare = getCompare(currItem);
					addItemDetail(currItem);
				}
			}

			if (GameCanvas.keyPressed[21] || GameCanvas.keyPressed[2])
			{
				handledKey = true;
				if (invSplitIsBody)
				{
					if (invSplitBodySel == 1)
					{
						invSplitBodySel = 0;
					}
					else if (invSplitBodySel == 3)
					{
						invSplitBodySel = 1;
					}
					else if (invSplitBodySel == 4)
					{
						invSplitBodySel = (bodyLen > 2) ? 2 : 0;
					}
					else if (invSplitBodySel == 5)
					{
						invSplitBodySel = (bodyLen > 4) ? 4 : ((bodyLen > 2) ? 2 : 0);
					}
					else if (invSplitBodySel >= 6 && invSplitBodySel < 10)
					{
						int c = invSplitBodySel - 6;
						if (c <= 1)
						{
							invSplitBodySel = (bodyLen > 3) ? 3 : ((bodyLen > 1) ? 1 : 0);
						}
						else
						{
							invSplitBodySel = (bodyLen > 5) ? 5 : ((bodyLen > 3) ? 3 : 0);
						}
					}
					else if (invSplitBodySel >= 10)
					{
						invSplitBodySel -= 4;
					}
					EnsureInventorySplitBodyVisible();
				}
				else
				{
					if (invSplitBagSel - cols >= 0)
					{
						invSplitBagSel -= cols;
					}
					EnsureInventorySplitBagVisible();
				}
				SoundMn.gI().panelClick();
				SyncInventorySplitSelected();
				currItem = GetInventorySplitCurrItem();
				if (currItem != null)
				{
					currItem.compare = getCompare(currItem);
					addItemDetail(currItem);
				}
			}
			else if (GameCanvas.keyPressed[22] || GameCanvas.keyPressed[8])
			{
				handledKey = true;
				if (invSplitIsBody)
				{
					if (invSplitBodySel == 0 && bodyLen > 1)
					{
						invSplitBodySel = 1;
					}
					else if (invSplitBodySel == 1 && bodyLen > 3)
					{
						invSplitBodySel = 3;
					}
					else if (invSplitBodySel == 2 && bodyLen > 4)
					{
						invSplitBodySel = 4;
					}
					else if (invSplitBodySel == 4 && bodyLen > 5)
					{
						invSplitBodySel = 5;
					}
					else if (invSplitBodySel == 3 && bodyLen > 6)
					{
						invSplitBodySel = 6;
					}
					else if (invSplitBodySel == 5 && bodyLen > 6)
					{
						invSplitBodySel = (bodyLen > 9) ? 9 : (bodyLen - 1);
					}
					else if (invSplitBodySel >= 6)
					{
						int nextRowStart = 6 + ((invSplitBodySel - 6) / 4 + 1) * 4;
						if (nextRowStart < bodyLen)
						{
							int nextIdx = invSplitBodySel + 4;
							invSplitBodySel = (nextIdx < bodyLen) ? nextIdx : (bodyLen - 1);
						}
					}
					EnsureInventorySplitBodyVisible();
				}
				else
				{
					if (invSplitBagSel + cols < bagLen)
					{
						invSplitBagSel += cols;
					}
					EnsureInventorySplitBagVisible();
				}
				SoundMn.gI().panelClick();
				SyncInventorySplitSelected();
				currItem = GetInventorySplitCurrItem();
				if (currItem != null)
				{
					currItem.compare = getCompare(currItem);
					addItemDetail(currItem);
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
				cmtoYBody = cmyBody;
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
						pointerDownLastX[i] = GameCanvas.py;
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
					if (GameCanvas.isPointer(rightX, listY, rightW, listH))
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
					else if (GameCanvas.isPointer(leftX, listY, leftW, listH) && cmyLimBody > 0)
					{
						if (dy != 0)
						{
							cmtoYBody -= dy;
							if (cmtoYBody < 0)
							{
								cmtoYBody = 0;
							}
							if (cmtoYBody > cmyLimBody)
							{
								cmtoYBody = cmyLimBody;
							}
							cmyBody -= dy;
							if (cmyBody < 0)
							{
								cmyBody = 0;
							}
							if (cmyBody > cmyLimBody)
							{
								cmyBody = cmyLimBody;
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
					cmtoYBody = cmyBody;
					pointerDownFirstX = -1000;
					int px = GameCanvas.px;
					int py = GameCanvas.py;
					bool hit = false;

					if (px >= leftX && px < leftX + leftW && py >= listY && py < listY + listH)
					{
						int pyBody = py + cmyBody;
						for (int s2 = 0; s2 < bodyLen; s2++)
						{
							int sx3, sy3;
							GetBodySlotRect(s2, leftX, leftW, startY, slotSize, rowGap, out sx3, out sy3);
							if (px >= sx3 && px < sx3 + slotSize && pyBody >= sy3 && pyBody < sy3 + slotSize)
							{
								invSplitIsBody = true;
								invSplitBodySel = s2;
								SyncInventorySplitSelected();
								EnsureInventorySplitBodyVisible();
								currItem = GetInventorySplitCurrItem();
								if (currItem != null)
								{
									currItem.compare = getCompare(currItem);
									addItemDetail(currItem);
								}
								hit = true;
								break;
							}
						}
					}
					else if (px >= rightX && px < rightX + rightW && py >= listY && py < listY + listH)
					{
						int relX = px - gridStartX;
						int relY = py - (listY + 2) + cmy;
						if (relX >= 0 && relY >= 0)
						{
							int col2 = relX / (slotW + gap);
							int row2 = relY / (slotH + gap);
							int inX = relX % (slotW + gap);
							int inY = relY % (slotH + gap);
							if (col2 >= 0 && col2 < cols && inX < slotW && inY < slotH)
							{
								int idx2 = row2 * cols + col2;
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
										addItemDetail(currItem);
									}
									hit = true;
								}
							}
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

	/// <summary>
	/// Truy xuất và trả về dữ liệu của GetInventorySplitMenuY.
	/// </summary>
	private int GetInventorySplitMenuY()
	{
		try
		{
			int leftX, leftW, rightX, rightW, listY, listH;
			GetInventorySplitLayout(out leftX, out leftW, out rightX, out rightW, out listY, out listH);
			if (invSplitIsBody)
			{
				Char me = Char.myCharz();
				int bodyLen = (me != null && me.arrItemBody != null) ? me.arrItemBody.Length : 8;
				int slotSize, rowGap, startY, totalRows, totalBodyH;
				GetPaperdollMetrics(leftX, leftW, listY, listH, bodyLen, out slotSize, out rowGap, out startY, out totalRows, out totalBodyH);
				int sx, sy;
				GetBodySlotRect(invSplitBodySel, leftX, leftW, startY, slotSize, rowGap, out sx, out sy);
				return sy - cmyBody + slotSize / 2;
			}
			int cols, slotW, slotH, gap, startPad;
			GetBagGridLayout(rightW, out cols, out slotW, out slotH, out gap, out startPad);
			int row = invSplitBagSel / cols;
			int sy2 = listY + 2 + row * (slotH + gap) - cmy;
			return sy2 + slotH / 2;
		}
		catch
		{
			return yScroll;
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ DoFireInventorySplit.
	/// </summary>
	private void DoFireInventorySplit()
	{
		try
		{
			if (GameCanvas.menu != null && GameCanvas.menu.showMenu)
			{
				return;
			}
			if (Char.myCharz().statusMe == 14)
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
