using System;
using System.Collections;
using Assets.src.e;
using UnityEngine;

public partial class mGraphics
{
	public void drawRegion(Image arg0, int x0, int y0, int w0, int h0, int arg5, int x, int y, int arg8)
		{
			if (arg0 != null)
			{
				x *= zoomLevel;
				y *= zoomLevel;
				x0 *= zoomLevel;
				y0 *= zoomLevel;
				w0 *= zoomLevel;
				h0 *= zoomLevel;
				_drawRegion(arg0, x0, y0, w0, h0, arg5, x, y, arg8);
			}
		}

	public void drawRegion(Image arg0, int x0, int y0, int w0, int h0, int arg5, float x, float y, int arg8)
		{
			if (arg0 != null)
			{
				x *= (float)zoomLevel;
				y *= (float)zoomLevel;
				x0 *= zoomLevel;
				y0 *= zoomLevel;
				w0 *= zoomLevel;
				h0 *= zoomLevel;
				__drawRegion(arg0, x0, y0, w0, h0, arg5, x, y, arg8);
			}
		}

	public void drawRegion(Image arg0, int x0, int y0, int w0, int h0, int arg5, int x, int y, int arg8, bool isClip)
		{
			drawRegion(arg0, x0, y0, w0, h0, arg5, x, y, arg8);
		}

	public void __drawRegion(Image image, int x0, int y0, int w, int h, int transform, float x, float y, int anchor)
		{
			if (image == null)
			{
				return;
			}
			if (isTranslate)
			{
				x += (float)translateX;
				y += (float)translateY;
			}
			float num = w;
			float num2 = h;
			float num3 = 0f;
			float num4 = 0f;
			float num5 = 0f;
			float num6 = 0f;
			float num7 = 1f;
			float num8 = 0f;
			int num9 = 1;
			if ((anchor & HCENTER) == HCENTER)
			{
				num5 -= num / 2f;
			}
			if ((anchor & VCENTER) == VCENTER)
			{
				num6 -= num2 / 2f;
			}
			if ((anchor & RIGHT) == RIGHT)
			{
				num5 -= num;
			}
			if ((anchor & BOTTOM) == BOTTOM)
			{
				num6 -= num2;
			}
			x += num5;
			y += num6;
			int num10 = 0;
			int num11 = 0;
			int num12 = 0;
			int num13 = 0;
			if (isClip)
			{
				num10 = clipX;
				num11 = clipY;
				num12 = clipW;
				num13 = clipH;
				if (isTranslate)
				{
					num10 += clipTX;
					num11 += clipTY;
				}
				Rect r = new Rect(x, y, w, h);
				Rect rect = intersectRect(r2: new Rect(num10, num11, num12, num13), r1: r);
				if (rect.width <= 0f || rect.height <= 0f)
				{
					return;
				}
				num = rect.width;
				num2 = rect.height;
				num3 = rect.x - r.x;
				num4 = rect.y - r.y;
			}
			float num14 = 0f;
			float num15 = 0f;
			switch (transform)
			{
			case 2:
				num14 += num;
				num7 = -1f;
				if (isClip)
				{
					if ((float)num10 > x)
					{
						num8 = 0f - num3;
					}
					else if ((float)(num10 + num12) < x + (float)w)
					{
						num8 = 0f - ((float)(num10 + num12) - x - (float)w);
					}
				}
				break;
			case 1:
				num9 = -1;
				num15 += num2;
				break;
			case 3:
				num9 = -1;
				num15 += num2;
				num7 = -1f;
				num14 += num;
				break;
			}
			int num16 = 0;
			int num17 = 0;
			if (transform == 5 || transform == 6 || transform == 4 || transform == 7)
			{
				matrixBackup = GUI.matrix;
				size = new Vector2(w, h);
				relativePosition = new Vector2(x, y);
				UpdatePos(3);
				switch (transform)
				{
				case 6:
					UpdatePos(3);
					break;
				case 5:
					size = new Vector2(w, h);
					UpdatePos(3);
					break;
				}
				switch (transform)
				{
				case 5:
					GUIUtility.RotateAroundPivot(90f, pivot);
					break;
				case 6:
					GUIUtility.RotateAroundPivot(270f, pivot);
					break;
				case 4:
					GUIUtility.RotateAroundPivot(270f, pivot);
					num14 += num;
					num7 = -1f;
					if (isClip)
					{
						if ((float)num10 > x)
						{
							num8 = 0f - num3;
						}
						else if ((float)(num10 + num12) < x + (float)w)
						{
							num8 = 0f - ((float)(num10 + num12) - x - (float)w);
						}
					}
					break;
				case 7:
					GUIUtility.RotateAroundPivot(270f, pivot);
					num9 = -1;
					num15 += num2;
					break;
				}
			}
			Graphics.DrawTexture(new Rect(x + num3 + num14 + (float)num16, y + num4 + (float)num17 + num15, num * num7, num2 * (float)num9), image.texture, new Rect(((float)x0 + num3 + num8) / (float)image.texture.width, ((float)image.texture.height - num2 - ((float)y0 + num4)) / (float)image.texture.height, num / (float)image.texture.width, num2 / (float)image.texture.height), 0, 0, 0, 0);
			if (transform == 5 || transform == 6 || transform == 4 || transform == 7)
			{
				GUI.matrix = matrixBackup;
			}
		}

	public void _drawRegion(Image image, float x0, float y0, int w, int h, int transform, int x, int y, int anchor)
		{
			if (image == null)
			{
				return;
			}
			if (isTranslate)
			{
				x += translateX;
				y += translateY;
			}
			float num = w;
			float num2 = h;
			float num3 = 0f;
			float num4 = 0f;
			float num5 = 0f;
			float num6 = 0f;
			float num7 = 1f;
			float num8 = 0f;
			int num9 = 1;
			if ((anchor & HCENTER) == HCENTER)
			{
				num5 -= num / 2f;
			}
			if ((anchor & VCENTER) == VCENTER)
			{
				num6 -= num2 / 2f;
			}
			if ((anchor & RIGHT) == RIGHT)
			{
				num5 -= num;
			}
			if ((anchor & BOTTOM) == BOTTOM)
			{
				num6 -= num2;
			}
			x += (int)num5;
			y += (int)num6;
			int num10 = 0;
			int num11 = 0;
			int num12 = 0;
			int num13 = 0;
			if (isClip)
			{
				num10 = clipX;
				num11 = clipY;
				num12 = clipW;
				num13 = clipH;
				if (isTranslate)
				{
					num10 += clipTX;
					num11 += clipTY;
				}
				Rect r = new Rect(x, y, w, h);
				Rect rect = intersectRect(r2: new Rect(num10, num11, num12, num13), r1: r);
				if (rect.width <= 0f || rect.height <= 0f)
				{
					return;
				}
				num = rect.width;
				num2 = rect.height;
				num3 = rect.x - r.x;
				num4 = rect.y - r.y;
			}
			float num14 = 0f;
			float num15 = 0f;
			switch (transform)
			{
			case 2:
				num14 += num;
				num7 = -1f;
				if (isClip)
				{
					if (num10 > x)
					{
						num8 = 0f - num3;
					}
					else if (num10 + num12 < x + w)
					{
						num8 = -(num10 + num12 - x - w);
					}
				}
				break;
			case 1:
				num9 = -1;
				num15 += num2;
				break;
			case 3:
				num9 = -1;
				num15 += num2;
				num7 = -1f;
				num14 += num;
				break;
			}
			int num16 = 0;
			int num17 = 0;
			if (transform == 5 || transform == 6 || transform == 4 || transform == 7)
			{
				matrixBackup = GUI.matrix;
				size = new Vector2(w, h);
				relativePosition = new Vector2(x, y);
				UpdatePos(3);
				switch (transform)
				{
				case 6:
					UpdatePos(3);
					break;
				case 5:
					size = new Vector2(w, h);
					UpdatePos(3);
					break;
				}
				switch (transform)
				{
				case 5:
					GUIUtility.RotateAroundPivot(90f, pivot);
					break;
				case 6:
					GUIUtility.RotateAroundPivot(270f, pivot);
					break;
				case 4:
					GUIUtility.RotateAroundPivot(270f, pivot);
					num14 += num;
					num7 = -1f;
					if (isClip)
					{
						if (num10 > x)
						{
							num8 = 0f - num3;
						}
						else if (num10 + num12 < x + w)
						{
							num8 = -(num10 + num12 - x - w);
						}
					}
					break;
				case 7:
					GUIUtility.RotateAroundPivot(270f, pivot);
					num9 = -1;
					num15 += num2;
					break;
				}
			}
			Graphics.DrawTexture(new Rect((float)x + num3 + num14 + (float)num16, (float)y + num4 + (float)num17 + num15, num * num7, num2 * (float)num9), image.texture, new Rect((x0 + num3 + num8) / (float)image.texture.width, ((float)image.texture.height - num2 - (y0 + num4)) / (float)image.texture.height, num / (float)image.texture.width, num2 / (float)image.texture.height), 0, 0, 0, 0);
			if (transform == 5 || transform == 6 || transform == 4 || transform == 7)
			{
				GUI.matrix = matrixBackup;
			}
		}

	public void drawRegionGui(Image image, float x0, float y0, int w, int h, int transform, float x, float y, int anchor)
		{
			GUI.color = setColorMiniMap(807956);
			x *= (float)zoomLevel;
			y *= (float)zoomLevel;
			x0 *= (float)zoomLevel;
			y0 *= (float)zoomLevel;
			w *= zoomLevel;
			h *= zoomLevel;
		}

	public void drawRegion2(Image image, float x0, float y0, int w, int h, int transform, int x, int y, int anchor)
		{
			GUI.color = image.colorBlend;
			if (isTranslate)
			{
				x += translateX;
				y += translateY;
			}
			string key = "dg" + x0 + y0 + w + h + transform + image.GetHashCode();
			Texture2D texture2D = (Texture2D)cachedTextures[key];
			if (texture2D == null)
			{
				Image image2 = Image.createImage(image, (int)x0, (int)y0, w, h, transform);
				texture2D = image2.texture;
				cache(key, texture2D);
			}
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			float num5 = w;
			float num6 = h;
			float num7 = 0f;
			float num8 = 0f;
			if ((anchor & HCENTER) == HCENTER)
			{
				num7 -= num5 / 2f;
			}
			if ((anchor & VCENTER) == VCENTER)
			{
				num8 -= num6 / 2f;
			}
			if ((anchor & RIGHT) == RIGHT)
			{
				num7 -= num5;
			}
			if ((anchor & BOTTOM) == BOTTOM)
			{
				num8 -= num6;
			}
			x += (int)num7;
			y += (int)num8;
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
			GUI.DrawTexture(new Rect(x - num, y - num2, w, h), texture2D);
			if (isClip)
			{
				GUI.EndGroup();
			}
			GUI.color = new Color(1f, 1f, 1f, 1f);
		}

	public void drawImagaByDrawTexture(Image image, float x, float y)
		{
			x *= (float)zoomLevel;
			y *= (float)zoomLevel;
			GUI.DrawTexture(new Rect(x + (float)translateX, y + (float)translateY, image.getRealImageWidth(), image.getRealImageHeight()), image.texture);
		}

	public void drawImage(Image image, int x, int y, int anchor)
		{
			if (image != null)
			{
				drawRegion(image, 0, 0, getImageWidth(image), getImageHeight(image), 0, x, y, anchor);
			}
		}

	public void drawImageFog(Image image, int x, int y, int anchor)
		{
			if (image != null)
			{
				drawRegion(image, 0, 0, image.texture.width, image.texture.height, 0, x, y, anchor);
			}
		}

	public void drawImage(Image image, int x, int y)
		{
			if (image != null)
			{
				drawRegion(image, 0, 0, getImageWidth(image), getImageHeight(image), 0, x, y, TOP | LEFT);
			}
		}

	public void drawImage(Image image, float x, float y, int anchor)
		{
			if (image != null)
			{
				drawRegion(image, 0, 0, getImageWidth(image), getImageHeight(image), 0, x, y, anchor);
			}
		}

	public void drawImageScale(Image image, int x, int y, int w, int h, int tranform)
		{
			GUI.color = Color.red;
			x *= zoomLevel;
			y *= zoomLevel;
			w *= zoomLevel;
			h *= zoomLevel;
			if (image != null)
			{
				Graphics.DrawTexture(new Rect(x + translateX, y + translateY, (tranform != 0) ? (-w) : w, h), image.texture);
			}
		}

	public void drawImageSimple(Image image, int x, int y)
		{
			x *= zoomLevel;
			y *= zoomLevel;
			if (image != null)
			{
				Graphics.DrawTexture(new Rect(x, y, image.w, image.h), image.texture);
			}
		}

	public static int getImageWidth(Image image)
		{
			return image.getWidth();
		}

	public static int getImageHeight(Image image)
		{
			return image.getHeight();
		}

	public static Image blend(Image img0, float level, int rgb)
		{
			int num = rgb & 0xFF;
			int num2 = (rgb >> 8) & 0xFF;
			int num3 = (rgb >> 16) & 0xFF;
			float num4 = (float)num / 256f;
			float num5 = (float)num2 / 256f;
			float num6 = (float)num3 / 256f;
			Color color = new Color(num6, num5, num4);
			Color[] pixels = img0.texture.GetPixels();
			float num7 = color.r;
			float num8 = color.g;
			float num9 = color.b;
			for (int i = 0; i < pixels.Length; i++)
			{
				Color color2 = pixels[i];
				if (isNotTranColor(color2))
				{
					float num10 = (num7 - color2.r) * level + color2.r;
					float num11 = (num8 - color2.g) * level + color2.g;
					float num12 = (num9 - color2.b) * level + color2.b;
					if (num10 > 255f)
					{
						num10 = 255f;
					}
					if (num10 < 0f)
					{
						num10 = 0f;
					}
					if (num11 > 255f)
					{
						num11 = 255f;
					}
					if (num11 < 0f)
					{
						num11 = 0f;
					}
					if (num12 < 0f)
					{
						num12 = 0f;
					}
					if (num12 > 255f)
					{
						num12 = 255f;
					}
					pixels[i].r = num10;
					pixels[i].g = num11;
					pixels[i].b = num12;
				}
			}
			Image image = Image.createImage(img0.getRealImageWidth(), img0.getRealImageHeight());
			image.texture.SetPixels(pixels);
			Image.setTextureQuality(image.texture);
			image.texture.Apply();
			Cout.LogError2("BLEND ----------------------------------------------------");
			return image;
		}

	public static int blendColor(float level, int color, int colorBlend)
		{
			Color color2 = setColorObj(colorBlend);
			float num = color2.r * 255f;
			float num2 = color2.g * 255f;
			float num3 = color2.b * 255f;
			Color color3 = setColorObj(color);
			float num4 = (num + color3.r) * level + color3.r;
			float num5 = (num2 + color3.g) * level + color3.g;
			float num6 = (num3 + color3.b) * level + color3.b;
			if (num4 > 255f)
			{
				num4 = 255f;
			}
			if (num4 < 0f)
			{
				num4 = 0f;
			}
			if (num5 > 255f)
			{
				num5 = 255f;
			}
			if (num5 < 0f)
			{
				num5 = 0f;
			}
			if (num6 < 0f)
			{
				num6 = 0f;
			}
			if (num6 > 255f)
			{
				num6 = 255f;
			}
			return (int)num6 & (255 + ((int)num5 << 8)) & (255 + ((int)num4 << 16)) & 0xFF;
		}

	public static int getRealImageWidth(Image img)
		{
			return img.w;
		}

	public static int getRealImageHeight(Image img)
		{
			return img.h;
		}

	internal void drawRegion(Small img, int p1, int p2, int p3, int p4, int transform, int x, int y, int anchor)
		{
			throw new NotImplementedException();
		}

}
