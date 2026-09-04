using System;
using System.Threading;

public partial class TField
{
	public void paintInputTf(mGraphics g, bool iss, int x, int y, int w, int h, int xText, int yText, string text, string info)
		{
			g.setColor(0);
			if (iss)
			{
				g.drawRegion(imgTf, 0, 81, 29, 27, 0, x, y, 0);
				g.drawRegion(imgTf, 0, 135, 29, 27, 0, x + w - 29, y, 0);
				g.drawRegion(imgTf, 0, 108, 29, 27, 0, x + w - 58, y, 0);
				for (int i = 0; i < (w - 58) / 29; i++)
				{
					g.drawRegion(imgTf, 0, 108, 29, 27, 0, x + 29 + i * 29, y, 0);
				}
			}
			else
			{
				g.drawRegion(imgTf, 0, 0, 29, 27, 0, x, y, 0);
				g.drawRegion(imgTf, 0, 54, 29, 27, 0, x + w - 29, y, 0);
				g.drawRegion(imgTf, 0, 27, 29, 27, 0, x + w - 58, y, 0);
				for (int j = 0; j < (w - 58) / 29; j++)
				{
					g.drawRegion(imgTf, 0, 27, 29, 27, 0, x + 29 + j * 29, y, 0);
				}
			}
			g.setClip(x + 3, y + 1, w - 4, h);
			if (text != null && !text.Equals(string.Empty))
			{
				mFont.tahoma_8b.drawString(g, text, xText, yText, 0);
			}
			else if (info != null)
			{
				if (iss)
				{
					mFont.tahoma_7b_focus.drawString(g, info, xText, yText, 0);
				}
				else
				{
					mFont.tahoma_7b_unfocus.drawString(g, info, xText, yText, 0);
				}
			}
		}

	public void paint(mGraphics g)
		{
			g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
			bool flag = isFocused();
			if (inputType == INPUT_TYPE_PASSWORD)
			{
				paintedText = passwordText;
			}
			else
			{
				paintedText = text;
			}
			paintInputTf(g, flag, x, y - 1, width, height + 5, TEXT_GAP_X + offsetX + x + 1, y + (height - mFont.tahoma_8b.getHeight()) / 2 + 2, paintedText, name);
			g.setClip(x + 3, y + 1, width - 4, height - 2);
			g.setColor(0);
			if (flag && isPaintMouse && isPaintCarret)
			{
				if (keyInActiveState == 0 && (showCaretCounter > 0 || counter / CARET_SHOWING_TIME % 4 == 0))
				{
					g.setColor(7999781);
					g.fillRect(TEXT_GAP_X + 1 + offsetX + x + mFont.tahoma_8b.getWidth(paintedText.Substring(0, caretPos) + "a") - CARET_WIDTH - mFont.tahoma_8b.getWidth("a"), y + (height - CARET_HEIGHT) / 2 + 5, CARET_WIDTH, CARET_HEIGHT);
				}
				GameCanvas.resetTrans(g);
				if (text != null && text.Length > 0 && GameCanvas.isTouch)
				{
					g.drawImage(GameCanvas.imgClear, x + width - 13, y + height / 2 + 3, mGraphics.VCENTER | mGraphics.HCENTER);
				}
			}
		}

}
