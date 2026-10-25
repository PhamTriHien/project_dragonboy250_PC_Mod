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

	private bool CanUseJavaBitmapFont()
		{
			return (mGraphics.zoomLevel == 1 || ModGraphics.IsJavaGraphics)
				&& fImages != null
				&& fImages.Length > 0
				&& !string.IsNullOrEmpty(strFont)
				&& EnsureJavaImageLoaded() != null;
		}

	private int DrawJavaGlyph(mGraphics g, char ch, int drawX, int drawY)
		{
			int idx = ResolveJavaGlyphIndex(ch);
			int gx = fImages[idx][0];
			int gy = fImages[idx][1];
			int gw = fImages[idx][2];
			int gh = fImages[idx][3];
			Image sheet = EnsureJavaImageLoaded();
			if (sheet != null && sheet.texture != null)
			{
				int sheetH = sheet.getHeight();
				int sheetW = sheet.getWidth();
				if (sheetH > 0 && gy + gh > sheetH)
				{
					gy -= sheetH;
					gx = sheetW / 2;
				}
				g.drawRegion(sheet, gx, gy, gw, gh, 0, drawX, drawY, 20);
			}
			return gw + space;
		}

	public void drawString(mGraphics g, string st, int x, int y, int align)
		{
			if (string.IsNullOrEmpty(st))
			{
				return;
			}
			if (CanUseJavaBitmapFont())
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
					if (char.IsLowSurrogate(st[i]))
					{
						continue;
					}
					num += DrawJavaGlyph(g, st[i], num, y);
				}
				return;
			}
			setTypePaint(g, st, x, y, align, 0);
		}

	public void drawStringBorder(mGraphics g, string st, int x, int y, int align)
		{
			if (CanUseJavaBitmapFont())
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
			if (CanUseJavaBitmapFont())
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
			sbyte b = (font != null) ? font.id : (sbyte)0;
			setTypePaint(g, st, x - 1, y - 1, align, b);
			setTypePaint(g, st, x - 1, y + 1, align, b);
			setTypePaint(g, st, x + 1, y - 1, align, b);
			setTypePaint(g, st, x + 1, y + 1, align, b);
			setTypePaint(g, st, x, y - 1, align, b);
			setTypePaint(g, st, x, y + 1, align, b);
			setTypePaint(g, st, x + 1, y, align, b);
			setTypePaint(g, st, x - 1, y, align, b);
			setTypePaint(g, st, x, y, align, 0);
		}

	public void drawString(mGraphics g, string st, int x, int y, int align, mFont font)
		{
			if (string.IsNullOrEmpty(st))
			{
				return;
			}
			if (CanUseJavaBitmapFont())
			{
				mFont shadow = font;
				if (shadow == bigNumber_black && strFont != null && strFont.Length <= 16 && number_gray != null)
				{
					shadow = number_gray;
				}
				bool useShadow = !GameCanvas.lowGraphic && shadow != null && shadow.CanUseJavaBitmapFont();
				int length = st.Length;
				int num = align switch
				{
					0 => x,
					1 => x - getWidth(st),
					_ => x - (getWidth(st) >> 1),
				};
				for (int i = 0; i < length; i++)
				{
					if (char.IsLowSurrogate(st[i]))
					{
						continue;
					}
					if (useShadow)
					{
						shadow.DrawJavaGlyph(g, st[i], num + 1, y);
						shadow.DrawJavaGlyph(g, st[i], num, y + 1);
					}
					num += DrawJavaGlyph(g, st[i], num, y);
				}
				return;
			}
			if (font != null)
			{
				setTypePaint(g, st, x, y + 1, align, font.id);
			}
			setTypePaint(g, st, x, y, align, 0);
		}

	private static readonly GUIStyle s_StyleLeft = new GUIStyle { alignment = TextAnchor.UpperLeft };
	private static readonly GUIStyle s_StyleRight = new GUIStyle { alignment = TextAnchor.UpperRight };
	private static readonly GUIStyle s_StyleCenter = new GUIStyle { alignment = TextAnchor.UpperCenter };

	public void _drawString(mGraphics g, string st, int x0, int y0, int align)
		{
			if (string.IsNullOrEmpty(st)) return;
			y0 += yAddFont;
			GUIStyle gUIStyle;
			float num = 0f;
			float num2 = y0;
			switch (align)
			{
			case 0:
				num = x0;
				gUIStyle = s_StyleLeft;
				break;
			case 1:
				num = x0 - GameCanvas.w;
				gUIStyle = s_StyleRight;
				break;
			default:
				num = x0 - GameCanvas.w / 2;
				gUIStyle = s_StyleCenter;
				break;
			}
			gUIStyle.font = myFont;
			gUIStyle.normal.textColor = color1;
			g.drawString(st, (int)num, (int)num2, gUIStyle);
		}

}
