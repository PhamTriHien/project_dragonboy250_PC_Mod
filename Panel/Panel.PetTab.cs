using System;

public partial class Panel : IActionListener, IChatable
{
	private bool HasPetTabButton()
	{
		try
		{
			if (type != 0)
			{
				return false;
			}
			Char me = Char.myCharz();
			if (me == null || !me.havePet)
			{
				return false;
			}
			if (currentTabName == null || currentTabName.Length < 4)
			{
				return false;
			}
			int len = currentTabName.Length;
			int tabW0 = WIDTH_PANEL / 5 - 1;
			if (len < 5)
			{
				tabW0 += 5;
			}
			int wNeed = (len + 1) * tabW0 + 5;
			if (GameCanvas.w < wNeed + 2)
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

	private void GetPetTabLabel(out string line1, out string line2)
	{
		line1 = "De";
		line2 = "Tu";
		try
		{
			if (mResources.petMainTab != null && mResources.petMainTab.Length > 0 && mResources.petMainTab[0] != null && mResources.petMainTab[0].Length > 1)
			{
				line1 = mResources.petMainTab[0][0];
				line2 = mResources.petMainTab[0][1];
				if (string.IsNullOrEmpty(line1) && !string.IsNullOrEmpty(mResources.pet))
				{
					line1 = mResources.pet;
					line2 = string.Empty;
				}
			}
			else if (!string.IsNullOrEmpty(mResources.pet))
			{
				line1 = mResources.pet;
				line2 = string.Empty;
			}
		}
		catch
		{
		}
	}

	private void EnsurePetTabLayout()
	{
		try
		{
			if (currentTabName == null)
			{
				return;
			}
			if (type != 0)
			{
				return;
			}
			int len = currentTabName.Length;
			int tabW0 = WIDTH_PANEL / 5 - 1;
			if (len < 5)
			{
				tabW0 += 5;
			}
			if (!HasPetTabButton())
			{
				if (W != WIDTH_PANEL)
				{
					int oldS = wScroll;
					W = WIDTH_PANEL;
					wScroll = W - 4;
					TAB_W = tabW0;
					if (position == 1)
					{
						xScroll = GameCanvas.w - wScroll;
						X = xScroll - 2;
					}
					else
					{
						xScroll = 2;
						X = 0;
					}
					startTabPos = xScroll + wScroll / 2 - len * TAB_W / 2;
					if (cmtoX == 0 && cmx == oldS)
					{
						cmx = wScroll;
					}
				}
				return;
			}
			int total = len + 1;
			TAB_W = tabW0;
			int oldScroll = wScroll;
			W = total * TAB_W + 5;
			wScroll = W - 4;
			if (position == 1)
			{
				xScroll = GameCanvas.w - wScroll;
				X = xScroll - 2;
			}
			else
			{
				xScroll = 2;
				X = 0;
			}
			startTabPos = xScroll + wScroll / 2 - total * TAB_W / 2;
			if (cmtoX == 0 && cmx == oldScroll)
			{
				cmx = wScroll;
			}
		}
		catch
		{
		}
	}

	private void GetPetTabRect(out int px, out int py, out int pw, out int ph)
	{
		px = 0;
		py = 52;
		pw = 0;
		ph = 25;
		try
		{
			if (!HasPetTabButton() || currentTabName == null)
			{
				return;
			}
			px = startTabPos + currentTabName.Length * TAB_W;
			py = 52;
			pw = TAB_W - 1;
			ph = 25;
		}
		catch
		{
		}
	}

	private void PaintPetTabButton(mGraphics g)
	{
		try
		{
			if (!HasPetTabButton() || currentTabName == null)
			{
				return;
			}
			int px, py, pw, ph;
			GetPetTabRect(out px, out py, out pw, out ph);
			if (pw <= 0)
			{
				return;
			}
			g.setColor(16773296);
			PopUp.paintPopUp(g, px, py, pw, ph, 0, true);
			if (keyTouchTab == currentTabName.Length)
			{
				g.drawImage(ItemMap.imageFlare, px + (pw + 1) / 2, py + 10, 3);
			}
			string l1, l2;
			GetPetTabLabel(out l1, out l2);
			mFont f = mFont.tahoma_7_grey;
			if (string.IsNullOrEmpty(l2))
			{
				f.drawString(g, l1, px + (pw + 1) / 2, py + 7, mFont.CENTER);
			}
			else
			{
				f.drawString(g, l1, px + (pw + 1) / 2, py + 1, mFont.CENTER);
				f.drawString(g, l2, px + (pw + 1) / 2, py + 12, mFont.CENTER);
			}
		}
		catch
		{
		}
	}

	private bool HandlePetTabClick()
	{
		try
		{
			if (!isShow || isClose || timeShow > 0 || cmx != cmtoX)
			{
				return false;
			}
			if (!HasPetTabButton() || currentTabName == null)
			{
				return false;
			}
			int px, py, pw, ph;
			GetPetTabRect(out px, out py, out pw, out ph);
			if (pw <= 0)
			{
				return false;
			}
			if (GameCanvas.isPointer(px, py, pw, ph))
			{
				keyTouchTab = currentTabName.Length;
				if (GameCanvas.isPointerJustRelease)
				{
					GameCanvas.isPointerJustRelease = false;
					GameCanvas.isPointerClick = false;
					GameCanvas.isPointerJustDown = false;
					GameCanvas.isPointerDown = false;
					pointerIsDowning = false;
					pointerDownTime = 0;
					SoundMn.gI().panelClick();
					OpenPetPanel();
					return true;
				}
				return false;
			}
			return false;
		}
		catch
		{
			return false;
		}
	}

	private void OpenPetPanel()
	{
		try
		{
			if (InfoDlg.isShow)
			{
				return;
			}
			Char me = Char.myCharz();
			if (me == null || !me.havePet)
			{
				return;
			}
			if (me.statusMe == 14)
			{
				GameCanvas.startOKDlg(mResources.can_not_do_when_die);
				return;
			}
			doFirePet();
		}
		catch
		{
		}
	}
}
