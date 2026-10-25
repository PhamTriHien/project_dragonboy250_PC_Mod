using System;
using System.Collections;
using Assets.src.e;
using UnityEngine;

public partial class mGraphics
{
	public Color setColorMiniMap(int rgb)
		{
			int num = rgb & 0xFF;
			int num2 = (rgb >> 8) & 0xFF;
			int num3 = (rgb >> 16) & 0xFF;
			float num4 = (float)num / 256f;
			float num5 = (float)num2 / 256f;
			float num6 = (float)num3 / 256f;
			return new Color(num6, num5, num4);
		}

	public float[] getRGB(Color cl)
		{
			float num = 256f * cl.r;
			float num2 = 256f * cl.g;
			float num3 = 256f * cl.b;
			return new float[3] { num, num2, num3 };
		}

	public void setColor(int rgb)
		{
			int num = rgb & 0xFF;
			int num2 = (rgb >> 8) & 0xFF;
			int num3 = (rgb >> 16) & 0xFF;
			b = (float)num / 256f;
			g = (float)num2 / 256f;
			r = (float)num3 / 256f;
			a = 1f;
		}

	public void setColor(Color color)
		{
			b = color.b;
			g = color.g;
			r = color.r;
			a = (color.a > 1f) ? (color.a / 255f) : color.a;
		}

	public void setBgColor(int rgb)
		{
			if (rgb != currentBGColor)
			{
				currentBGColor = rgb;
				int num = rgb & 0xFF;
				int num2 = (rgb >> 8) & 0xFF;
				int num3 = (rgb >> 16) & 0xFF;
				b = (float)num / 256f;
				g = (float)num2 / 256f;
				r = (float)num3 / 256f;
				Main.main.GetComponent<Camera>().backgroundColor = new Color(r, g, b);
			}
		}

	private static void DrawScaledLabel(Rect rect, string s, GUIStyle style)
		{
			if (style == null || style.fontSize > 0)
			{
				GUI.Label(rect, s, style);
				return;
			}
			float scale = ModGraphics.GetFontSizeScale();
			if (Mathf.Abs(scale - 1f) <= 0.001f)
			{
				GUI.Label(rect, s, style);
				return;
			}
			if (style.font != null && style.font.dynamic)
			{
				int refSize = (style.font.fontSize > 0) ? style.font.fontSize : (14 * zoomLevel);
				int targetSize = Mathf.Max(7, Mathf.RoundToInt(refSize * scale));
				int origSize = style.fontSize;
				style.fontSize = targetSize;
				Rect adjusted = new Rect(rect.x, rect.y + (refSize - targetSize) / 2, rect.width, rect.height);
				GUI.Label(adjusted, s, style);
				style.fontSize = origSize;
				return;
			}
			float anchorX = rect.x;
			if (style.alignment == TextAnchor.UpperCenter || style.alignment == TextAnchor.MiddleCenter || style.alignment == TextAnchor.LowerCenter)
			{
				anchorX = rect.x + rect.width * 0.5f;
			}
			else if (style.alignment == TextAnchor.UpperRight || style.alignment == TextAnchor.MiddleRight || style.alignment == TextAnchor.LowerRight)
			{
				anchorX = rect.x + rect.width;
			}
			float anchorY = rect.y + (14f * zoomLevel) * 0.5f;
			Matrix4x4 prevMatrix = GUI.matrix;
			GUIUtility.ScaleAroundPivot(new Vector2(scale, scale), new Vector2(anchorX, anchorY));
			GUI.Label(rect, s, style);
			GUI.matrix = prevMatrix;
		}

	public void drawString(string s, int x, int y, GUIStyle style)
		{
			x *= zoomLevel;
			y *= zoomLevel;
			if (isTranslate)
			{
				x += translateX;
				y += translateY;
			}
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			if (isClip)
			{
				num = clipX;
				num2 = clipY;
				num3 = clipW;
				num4 = clipH;
				if (isTranslate)
				{
					num += clipTX;
					num2 += clipTY;
				}
			}
			if (isClip)
			{
				GUI.BeginGroup(new Rect(num, num2, num3, num4));
			}
			DrawScaledLabel(new Rect(x - num, y - num2, ScaleGUI.WIDTH, 100f), s, style);
			if (isClip)
			{
				GUI.EndGroup();
			}
		}

	public void setColor(int rgb, float alpha)
		{
			int num = rgb & 0xFF;
			int num2 = (rgb >> 8) & 0xFF;
			int num3 = (rgb >> 16) & 0xFF;
			b = (float)num / 256f;
			g = (float)num2 / 256f;
			r = (float)num3 / 256f;
			a = (alpha > 1f) ? (alpha / 255f) : alpha;
		}

	public void drawString(string s, int x, int y, GUIStyle style, int w)
		{
			x *= zoomLevel;
			y *= zoomLevel;
			if (isTranslate)
			{
				x += translateX;
				y += translateY;
			}
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			if (isClip)
			{
				num = clipX;
				num2 = clipY;
				num3 = clipW;
				num4 = clipH;
				if (isTranslate)
				{
					num += clipTX;
					num2 += clipTY;
				}
			}
			if (isClip)
			{
				GUI.BeginGroup(new Rect(num, num2, num3, num4));
			}
			DrawScaledLabel(new Rect(x - num, y - num2 - 4, w, 100f), s, style);
			if (isClip)
			{
				GUI.EndGroup();
			}
		}

	public static bool isNotTranColor(Color color)
		{
			if (color == Color.clear || color == transParentColor)
			{
				return false;
			}
			return true;
		}

	public static Color setColorObj(int rgb)
		{
			int num = rgb & 0xFF;
			int num2 = (rgb >> 8) & 0xFF;
			int num3 = (rgb >> 16) & 0xFF;
			float num4 = (float)num / 256f;
			float num5 = (float)num2 / 256f;
			float num6 = (float)num3 / 256f;
			return new Color(num6, num5, num4);
		}

	public void fillTrans(Image imgTrans, int x, int y, int w, int h)
		{
			setColor(0, 0.5f);
			fillRect(x * zoomLevel, y * zoomLevel, w * zoomLevel, h * zoomLevel);
		}

	public static int getIntByColor(Color cl)
		{
			float num = cl.r * 255f;
			float num2 = cl.b * 255f;
			float num3 = cl.g * 255f;
			return (((int)num & 0xFF) << 16) | (((int)num3 & 0xFF) << 8) | ((int)num2 & 0xFF);
		}

	public void fillArg(int i, int j, int k, int l, int m, int n)
		{
			fillRect(i * zoomLevel, j * zoomLevel, k * zoomLevel, l * zoomLevel);
		}

}
