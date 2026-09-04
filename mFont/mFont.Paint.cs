using System;
using System.Collections;
using UnityEngine;

public partial class mFont
{
	public void setTypePaint(mGraphics g, string st, int x, int y, int align, sbyte idFont)
		{
			sbyte b = id;
			if (idFont > 0)
			{
				b = idFont;
			}
			x--;
			if (id > 24)
			{
				Color[] array = new Color[6]
				{
					setColor(6029312),
					setColor(7169025),
					setColor(7680),
					setColor(0),
					setColor(9264),
					setColor(6029312)
				};
				color1 = array[id - 25];
				color2 = array[id - 25];
				_drawString(g, st, x + 1, y, align);
				_drawString(g, st, x - 1, y, align);
				_drawString(g, st, x, y - 1, align);
				_drawString(g, st, x, y + 1, align);
				_drawString(g, st, x + 1, y + 1, align);
				_drawString(g, st, x + 1, y - 1, align);
				_drawString(g, st, x - 1, y - 1, align);
				_drawString(g, st, x - 1, y + 1, align);
				color1 = bigColor(id);
				color2 = bigColor(id);
			}
			else
			{
				setColorByID(b);
			}
			_drawString(g, st, x, y - yAdd, align);
		}

	public void drawString(mGraphics g, string st, int x, int y, int align)
		{
			if (mGraphics.zoomLevel == 1)
			{
				int length = st.Length;
				int num = align switch
				{
					0 => x, 
					1 => x - getWidth(st), 
					_ => x - (getWidth(st) >> 1), 
				};
				for (int i = 0; i < length; i++)
				{
					int num2 = strFont.IndexOf(st[i] + string.Empty);
					if (num2 == -1)
					{
						num2 = 0;
					}
					if (num2 > -1)
					{
						int x2 = fImages[num2][0];
						int num3 = fImages[num2][1];
						int w = fImages[num2][2];
						int num4 = fImages[num2][3];
						if (num3 + num4 > imgFont.texture.height)
						{
							num3 -= imgFont.texture.height;
							x2 = imgFont.texture.width / 2;
						}
						g.drawRegion(imgFont, x2, num3, w, num4, 0, num, y, 20);
					}
					num += fImages[num2][2] + space;
				}
			}
			else
			{
				setTypePaint(g, st, x, y, align, 0);
			}
		}

	public void drawStringBorder(mGraphics g, string st, int x, int y, int align)
		{
			if (mGraphics.zoomLevel == 1)
			{
				drawString(g, st, x, y, align);
			}
			else
			{
				setTypePaint(g, st, x, y, align, 0);
			}
		}

	public void drawStringBorder(mGraphics g, string st, int x, int y, int align, mFont font2)
		{
			if (mGraphics.zoomLevel == 1)
			{
				drawString(g, st, x, y, align, font2);
			}
			else
			{
				drawStringBd(g, st, x, y, align, font2);
			}
		}

	public void drawStringBd(mGraphics g, string st, int x, int y, int align, mFont font)
		{
			setTypePaint(g, st, x - 1, y - 1, align, 0);
			setTypePaint(g, st, x - 1, y + 1, align, 0);
			setTypePaint(g, st, x + 1, y - 1, align, 0);
			setTypePaint(g, st, x + 1, y + 1, align, 0);
			setTypePaint(g, st, x, y - 1, align, 0);
			setTypePaint(g, st, x, y + 1, align, 0);
			setTypePaint(g, st, x + 1, y, align, 0);
			setTypePaint(g, st, x - 1, y, align, 0);
			setTypePaint(g, st, x, y, align, 0);
		}

	public void drawString(mGraphics g, string st, int x, int y, int align, mFont font)
		{
			if (mGraphics.zoomLevel == 1)
			{
				int length = st.Length;
				int num = align switch
				{
					0 => x, 
					1 => x - getWidth(st), 
					_ => x - (getWidth(st) >> 1), 
				};
				for (int i = 0; i < length; i++)
				{
					int num2 = strFont.IndexOf(st[i]);
					if (num2 == -1)
					{
						num2 = 0;
					}
					if (num2 > -1)
					{
						int x2 = fImages[num2][0];
						int num3 = fImages[num2][1];
						int w = fImages[num2][2];
						int num4 = fImages[num2][3];
						if (num3 + num4 > imgFont.texture.height)
						{
							num3 -= imgFont.texture.height;
							x2 = imgFont.texture.width / 2;
						}
						if (!GameCanvas.lowGraphic && font != null)
						{
							g.drawRegion(font.imgFont, x2, num3, w, num4, 0, num + 1, y, 20);
							g.drawRegion(font.imgFont, x2, num3, w, num4, 0, num, y + 1, 20);
						}
						g.drawRegion(imgFont, x2, num3, w, num4, 0, num, y, 20);
					}
					num += fImages[num2][2] + space;
				}
			}
			else
			{
				setTypePaint(g, st, x, y + 1, align, font.id);
				setTypePaint(g, st, x, y, align, 0);
			}
		}

	public void _drawString(mGraphics g, string st, int x0, int y0, int align)
		{
			y0 += yAddFont;
			GUIStyle gUIStyle = new GUIStyle(GUI.skin.label);
			gUIStyle.font = myFont;
			float num = 0f;
			float num2 = 0f;
			switch (align)
			{
			case 0:
				num = x0;
				num2 = y0;
				gUIStyle.alignment = TextAnchor.UpperLeft;
				break;
			case 1:
				num = x0 - GameCanvas.w;
				num2 = y0;
				gUIStyle.alignment = TextAnchor.UpperRight;
				break;
			case 2:
			case 3:
				num = x0 - GameCanvas.w / 2;
				num2 = y0;
				gUIStyle.alignment = TextAnchor.UpperCenter;
				break;
			}
			gUIStyle.normal.textColor = color1;
			g.drawString(st, (int)num, (int)num2, gUIStyle);
		}

}
