using System;
using System.Collections;
using Assets.src.e;
using UnityEngine;

public partial class mGraphics
{
	public void fillRect(int x, int y, int w, int h, int color, int alpha)
		{
			float alpha2 = 0.5f;
			setColor(color, alpha2);
			fillRect(x, y, w, h);
		}

	public void drawLine(int x1, int y1, int x2, int y2)
		{
			x1 *= zoomLevel;
			y1 *= zoomLevel;
			x2 *= zoomLevel;
			y2 *= zoomLevel;
			if (y1 == y2)
			{
				if (x1 > x2)
				{
					int num = x2;
					x2 = x1;
					x1 = num;
				}
				fillRect(x1, y1, x2 - x1, 1);
				return;
			}
			if (x1 == x2)
			{
				if (y1 > y2)
				{
					int num2 = y2;
					y2 = y1;
					y1 = num2;
				}
				fillRect(x1, y1, 1, y2 - y1);
				return;
			}
			if (isTranslate)
			{
				x1 += translateX;
				y1 += translateY;
				x2 += translateX;
				y2 += translateY;
			}
			Vector2 vector = new Vector2(x1, y1);
			Vector2 vector2 = new Vector2(x2, y2);
			Vector2 vector3 = vector2 - vector;
			float num3 = 57.29578f * Mathf.Atan(vector3.y / vector3.x);
			if (vector3.x < 0f)
			{
				num3 += 180f;
			}
			int num4 = (int)Mathf.Ceil(0f);
			GUIUtility.RotateAroundPivot(num3, vector);
			int num5 = 0;
			int num6 = 0;
			int num7 = 0;
			int num8 = 0;
			if (isClip)
			{
				num5 = clipX;
				num6 = clipY;
				num7 = clipW;
				num8 = clipH;
				if (isTranslate)
				{
					num5 += clipTX;
					num6 += clipTY;
				}
			}
			Color oldColor = GUI.color;
			float alphaVal = (a > 1f) ? (a / 255f) : a;
			GUI.color = new Color(r, g, b, alphaVal);
			if (isClip)
			{
				GUI.BeginGroup(new Rect(num5, num6, num7, num8));
			}
			GUI.DrawTexture(new Rect(vector.x - (float)num5, vector.y - (float)num4 - (float)num6, vector3.magnitude, 1f), Texture2D.whiteTexture);
			if (isClip)
			{
				GUI.EndGroup();
			}
			GUI.color = oldColor;
			GUIUtility.RotateAroundPivot(0f - num3, vector);
		}

	public void drawRect(int x, int y, int w, int h)
		{
			int num = 1;
			fillRect(x, y, w, num);
			fillRect(x, y, num, h);
			fillRect(x + w, y, num, h + 1);
			fillRect(x, y + h, w + 1, num);
		}

	public void fillRect(int x, int y, int w, int h)
		{
			x *= zoomLevel;
			y *= zoomLevel;
			w *= zoomLevel;
			h *= zoomLevel;
			if (w <= 0 || h <= 0)
			{
				return;
			}
			if (isTranslate)
			{
				x += translateX;
				y += translateY;
			}
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			if (isClip)
			{
				num3 = clipX;
				num4 = clipY;
				num5 = clipW;
				num6 = clipH;
				if (isTranslate)
				{
					num3 += clipTX;
					num4 += clipTY;
				}
			}
			Color oldColor = GUI.color;
			float alphaVal = (a > 1f) ? (a / 255f) : a;
			GUI.color = new Color(r, g, b, alphaVal);
			if (isClip)
			{
				GUI.BeginGroup(new Rect(num3, num4, num5, num6));
			}
			GUI.DrawTexture(new Rect(x - num3, y - num4, w, h), Texture2D.whiteTexture);
			if (isClip)
			{
				GUI.EndGroup();
			}
			GUI.color = oldColor;
		}

	public void drawRoundRect(int x, int y, int w, int h, int arcWidth, int arcHeight)
		{
			drawRect(x, y, w, h);
		}

	public void fillRoundRect(int x, int y, int width, int height, int arcWidth, int arcHeight)
		{
			fillRect(x, y, width, height);
		}

	public Rect intersectRect(Rect r1, Rect r2)
		{
			float num = r1.x;
			float num2 = r1.y;
			float x = r2.x;
			float y = r2.y;
			float num3 = num;
			num3 += r1.width;
			float num4 = num2;
			num4 += r1.height;
			float num5 = x;
			num5 += r2.width;
			float num6 = y;
			num6 += r2.height;
			if (num < x)
			{
				num = x;
			}
			if (num2 < y)
			{
				num2 = y;
			}
			if (num3 > num5)
			{
				num3 = num5;
			}
			if (num4 > num6)
			{
				num4 = num6;
			}
			num3 -= num;
			num4 -= num2;
			if (num3 < -30000f)
			{
				num3 = -30000f;
			}
			if (num4 < -30000f)
			{
				num4 = -30000f;
			}
			return new Rect(num, num2, (int)num3, (int)num4);
		}

	public void CreateLineMaterial()
		{
			if (!lineMaterial)
			{
				try
				{
					Shader shader = Shader.Find("Hidden/Internal-Colored");
					if (shader == null)
					{
						shader = Shader.Find("GUI/Text Shader");
					}
					if (shader == null)
					{
						shader = Shader.Find("UI/Default");
					}
					if (shader != null)
					{
						lineMaterial = new Material(shader);
						lineMaterial.hideFlags = HideFlags.HideAndDontSave;
					}
				}
				catch (Exception)
				{
				}
			}
		}

	public void drawlineGL(MyVector totalLine)
		{
			if (lineMaterial == null)
			{
				CreateLineMaterial();
			}
			if (lineMaterial == null)
			{
				return;
			}
			try
			{
				lineMaterial.SetPass(0);
				GL.PushMatrix();
				GL.Begin(1);
				for (int i = 0; i < totalLine.size(); i++)
				{
					mLine mLine2 = (mLine)totalLine.elementAt(i);
					GL.Color(new Color(mLine2.r, mLine2.g, mLine2.b, mLine2.a));
					int num = mLine2.x1 * zoomLevel;
					int num2 = mLine2.y1 * zoomLevel;
					int num3 = mLine2.x2 * zoomLevel;
					int num4 = mLine2.y2 * zoomLevel;
					if (isTranslate)
					{
						num += translateX;
						num2 += translateY;
						num3 += translateX;
						num4 += translateY;
					}
					for (int j = 0; j < zoomLevel; j++)
					{
						GL.Vertex(new Vector2(num + j, num2 + j));
						GL.Vertex(new Vector2(num3 + j, num4 + j));
						if (j > 0)
						{
							GL.Vertex(new Vector2(num + j, num2));
							GL.Vertex(new Vector2(num3 + j, num4));
							GL.Vertex(new Vector2(num, num2 + j));
							GL.Vertex(new Vector2(num3, num4 + j));
						}
					}
				}
				GL.End();
				GL.PopMatrix();
				totalLine.removeAllElements();
			}
			catch (Exception)
			{
			}
		}

	public void drawLine(mGraphics g, int x, int y, int xTo, int yTo, int nLine, int color)
		{
			MyVector myVector = new MyVector();
			for (int i = 0; i < nLine; i++)
			{
				myVector.addElement(new mLine(x, y, xTo + i, yTo + i, color));
			}
			g.drawlineGL(myVector);
		}

}
