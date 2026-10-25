using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Image
{
	private struct ServerTexPair
	{
		public Texture2D hd;
		public Texture2D java;
	}
	private static readonly Dictionary<string, Texture2D> s_JavaScaledTextures = new Dictionary<string, Texture2D>();
	private static readonly Dictionary<ulong, ServerTexPair> s_ServerZoomTextures = new Dictionary<ulong, ServerTexPair>();
	private const int INTERVAL = 5;

	private const int MAXTIME = 500;

	public Texture2D texture;

	public static Image imgTemp;

	public static string filenametemp;

	public static byte[] datatemp;

	public static Image imgSrcTemp;

	public static int xtemp;

	public static int ytemp;

	public static int wtemp;

	public static int htemp;

	public static int transformtemp;

	public int w;

	public int h;

	public static int status;

	public Color colorBlend = Color.black;

	public bool isZoomScaled;

	public byte[] rawServerPngBytes;

	private static ulong ComputePngHash(byte[] data)
	{
		ulong hash = 14695981039346656037UL;
		for (int i = 0; i < data.Length; i++)
		{
			hash ^= data[i];
			hash *= 1099511628211UL;
		}
		return hash;
	}

	private static Texture2D QuantizeTextureJava2x2(Texture2D src)
	{
		if (src == null || src.width < 2 || src.height < 2)
		{
			return src;
		}
		int w = src.width;
		int h = src.height;
		Color32[] pixels = src.GetPixels32();
		for (int y = 0; y + 1 < h; y += 2)
		{
			int row0 = (h - 1 - y) * w;
			int row1 = (h - 2 - y) * w;
			for (int x = 0; x + 1 < w; x += 2)
			{
				Color32 c0 = pixels[row0 + x];
				Color32 c1 = pixels[row0 + x + 1];
				Color32 c2 = pixels[row1 + x];
				Color32 c3 = pixels[row1 + x + 1];

				int sumA = c0.a + c1.a + c2.a + c3.a;
				Color32 outCol;
				if (sumA >= 256)
				{
					int ar = (c0.r * c0.a + c1.r * c1.a + c2.r * c2.a + c3.r * c3.a) / sumA;
					int ag = (c0.g * c0.a + c1.g * c1.a + c2.g * c2.a + c3.g * c3.a) / sumA;
					int ab = (c0.b * c0.a + c1.b * c1.a + c2.b * c2.a + c3.b * c3.a) / sumA;

					Color32 best = default;
					int bestDist = int.MaxValue;
					if (c0.a >= 128) { int d = System.Math.Abs(c0.r - ar) + System.Math.Abs(c0.g - ag) + System.Math.Abs(c0.b - ab); if (d < bestDist) { bestDist = d; best = c0; } }
					if (c1.a >= 128) { int d = System.Math.Abs(c1.r - ar) + System.Math.Abs(c1.g - ag) + System.Math.Abs(c1.b - ab); if (d < bestDist) { bestDist = d; best = c1; } }
					if (c2.a >= 128) { int d = System.Math.Abs(c2.r - ar) + System.Math.Abs(c2.g - ag) + System.Math.Abs(c2.b - ab); if (d < bestDist) { bestDist = d; best = c2; } }
					if (c3.a >= 128) { int d = System.Math.Abs(c3.r - ar) + System.Math.Abs(c3.g - ag) + System.Math.Abs(c3.b - ab); if (d < bestDist) { bestDist = d; best = c3; } }
					outCol = (bestDist == int.MaxValue)
						? new Color32((byte)ar, (byte)ag, (byte)ab, 255)
						: new Color32(best.r, best.g, best.b, 255);
				}
				else
				{
					outCol = new Color32(0, 0, 0, 0);
				}

				pixels[row0 + x] = outCol;
				pixels[row0 + x + 1] = outCol;
				pixels[row1 + x] = outCol;
				pixels[row1 + x + 1] = outCol;
			}
		}
		Texture2D dst = new Texture2D(w, h, TextureFormat.RGBA32, false);
		dst.SetPixels32(pixels);
		dst.Apply(false, false);
		setTextureQuality(dst);
		return dst;
	}

	public static Image createServerZoomImage(sbyte[] imageData, int offset, int length)
	{
		if (imageData == null || offset < 0 || length <= 0 || offset + length > imageData.Length)
		{
			return null;
		}
		byte[] array = new byte[length];
		for (int i = 0; i < length; i++)
		{
			array[i] = convertSbyteToByte(imageData[i + offset]);
		}
		return createServerZoomImage(array);
	}

	public static Image createServerZoomImage(byte[] pngBytes)
	{
		if (pngBytes == null || pngBytes.Length == 0)
		{
			return null;
		}
		ulong key = ComputePngHash(pngBytes) ^ ((ulong)pngBytes.Length * 1099511628211UL);
		ServerTexPair pair;
		if (!s_ServerZoomTextures.TryGetValue(key, out pair) || pair.hd == null)
		{
			Image decoded = createImage(pngBytes);
			if (decoded == null || decoded.texture == null)
			{
				return decoded;
			}
			pair = new ServerTexPair { hd = decoded.texture, java = null };
			s_ServerZoomTextures[key] = pair;
		}
		Texture2D chosen = pair.hd;
		if (mGraphics.zoomLevel == 2 && ModGraphics.IsJavaGraphics)
		{
			if (pair.java == null)
			{
				pair.java = QuantizeTextureJava2x2(pair.hd);
				s_ServerZoomTextures[key] = pair;
			}
			chosen = pair.java ?? pair.hd;
		}
		setTextureQuality(chosen);
		return new Image
		{
			texture = chosen,
			w = chosen.width,
			h = chosen.height,
			isZoomScaled = true,
			rawServerPngBytes = pngBytes
		};
	}

	public static Image refreshServerZoomImage(Image current)
	{
		if (current == null || current.rawServerPngBytes == null || current.rawServerPngBytes.Length == 0)
		{
			return current;
		}
		return createServerZoomImage(current.rawServerPngBytes);
	}

	public static Image createEmptyImage()
	{
		return __createEmptyImage();
	}

	public static Image createImage(string filename)
	{
		return __createImage(filename);
	}

	public static Image createImage(byte[] imageData)
	{
		return __createImage(imageData);
	}

	public static Image createImage(Image src, int x, int y, int w, int h, int transform)
	{
		return __createImage(src, x, y, w, h, transform);
	}

	public static Image createImage(int w, int h)
	{
		return __createImage(w, h);
	}

	public static Image createImage(Image img)
	{
		if (img == null || img.texture == null)
		{
			return null;
		}
		Image image = createImage(img.w, img.h);
		image.texture = img.texture;
		return image;
	}

	public static Image createImage(sbyte[] imageData, int offset, int lenght)
	{
		if (offset + lenght > imageData.Length)
		{
			return null;
		}
		byte[] array = new byte[lenght];
		for (int i = 0; i < lenght; i++)
		{
			array[i] = convertSbyteToByte(imageData[i + offset]);
		}
		return createImage(array);
	}

	public static byte convertSbyteToByte(sbyte var)
	{
		if (var > 0)
		{
			return (byte)var;
		}
		return (byte)(var + 256);
	}

	public static byte[] convertArrSbyteToArrByte(sbyte[] var)
	{
		byte[] array = new byte[var.Length];
		for (int i = 0; i < var.Length; i++)
		{
			if (var[i] > 0)
			{
				array[i] = (byte)var[i];
			}
			else
			{
				array[i] = (byte)(var[i] + 256);
			}
		}
		return array;
	}

	public static Image createRGBImage(int[] rbg, int w, int h, bool bl)
	{
		Image image = createImage(w, h);
		Color[] array = new Color[rbg.Length];
		for (int i = 0; i < array.Length; i++)
		{
			ref Color reference = ref array[i];
			reference = setColorFromRBG(rbg[i]);
		}
		image.texture.SetPixels(0, 0, w, h, array);
		image.texture.Apply();
		return image;
	}

	public static Color setColorFromRBG(int rgb)
	{
		int num = rgb & 0xFF;
		int num2 = (rgb >> 8) & 0xFF;
		int num3 = (rgb >> 16) & 0xFF;
		float b = (float)num / 256f;
		float g = (float)num2 / 256f;
		float r = (float)num3 / 256f;
		return new Color(r, g, b);
	}

	public static void update()
	{
		if (status == 2)
		{
			status = 1;
			imgTemp = __createEmptyImage();
			status = 0;
		}
		else if (status == 3)
		{
			status = 1;
			imgTemp = __createImage(filenametemp);
			status = 0;
		}
		else if (status == 4)
		{
			status = 1;
			imgTemp = __createImage(datatemp);
			status = 0;
		}
		else if (status == 5)
		{
			status = 1;
			imgTemp = __createImage(imgSrcTemp, xtemp, ytemp, wtemp, htemp, transformtemp);
			status = 0;
		}
		else if (status == 6)
		{
			status = 1;
			imgTemp = __createImage(wtemp, htemp);
			status = 0;
		}
	}

	private static Image _createEmptyImage()
	{
		return __createEmptyImage();
	}

	private static Image _createImage(string filename)
	{
		return __createImage(filename);
	}

	private static Image _createImage(byte[] imageData)
	{
		return __createImage(imageData);
	}

	private static Image _createImage(Image src, int x, int y, int w, int h, int transform)
	{
		return __createImage(src, x, y, w, h, transform);
	}

	private static Image _createImage(int w, int h)
	{
		return __createImage(w, h);
	}

	public static byte[] loadData(string filename)
	{
		Image image = new Image();
		TextAsset textAsset = (TextAsset)Resources.Load(filename, typeof(TextAsset));
		if (textAsset == null || textAsset.bytes == null || textAsset.bytes.Length == 0)
		{
			return null;
		}
		sbyte[] array = ArrayCast.cast(textAsset.bytes);
		Debug.LogError("CHIEU DAI MANG BYTE IMAGE CREAT = " + array.Length);
		return textAsset.bytes;
	}

	private static Image __createImage(string filename)
	{
		if (string.IsNullOrEmpty(filename))
		{
			return null;
		}
		string cleanName = filename.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
			? filename.Substring(0, filename.Length - 4)
			: filename;
		if (cleanName.StartsWith("/"))
		{
			cleanName = cleanName.Substring(1);
		}
		Texture2D texture2D = Resources.Load(cleanName) as Texture2D;
		if (texture2D == null)
		{
			return null;
		}
		bool isX1Asset = cleanName.StartsWith("x1/", StringComparison.OrdinalIgnoreCase) ||
			cleanName.IndexOf("/x1/", StringComparison.OrdinalIgnoreCase) >= 0;
		if (isX1Asset && mGraphics.zoomLevel == 2)
		{
			if (!s_JavaScaledTextures.TryGetValue(cleanName, out Texture2D scaledTex) || scaledTex == null)
			{
				scaledTex = ScaleTextureNearestNeighbor(texture2D, texture2D.width * 2, texture2D.height * 2);
				s_JavaScaledTextures[cleanName] = scaledTex;
			}
			if (scaledTex != null)
			{
				texture2D = scaledTex;
			}
		}
		Image image = new Image();
		image.texture = texture2D;
		image.w = image.texture.width;
		image.h = image.texture.height;
		setTextureQuality(image);
		return image;
	}

	private static Texture2D ScaleTextureNearestNeighbor(Texture2D src, int targetW, int targetH)
	{
		if (src == null || targetW <= 0 || targetH <= 0)
		{
			return src;
		}
		setTextureQuality(src);
		RenderTexture prevActive = RenderTexture.active;
		RenderTexture rt = RenderTexture.GetTemporary(targetW, targetH, 0, RenderTextureFormat.ARGB32);
		rt.filterMode = FilterMode.Point;
		Texture2D dst = new Texture2D(targetW, targetH, TextureFormat.RGBA32, false);
		setTextureQuality(dst);
		try
		{
			Graphics.Blit(src, rt);
			RenderTexture.active = rt;
			dst.ReadPixels(new Rect(0f, 0f, targetW, targetH), 0, 0, false);
			dst.Apply(false, false);
		}
		finally
		{
			RenderTexture.active = prevActive;
			RenderTexture.ReleaseTemporary(rt);
		}
		return dst;
	}

	private static Image __createImage(byte[] imageData)
	{
		if (imageData == null || imageData.Length == 0)
		{
			Cout.LogError("Create Image from byte array fail");
			return null;
		}
		Image image = new Image();
		try
		{
			image.texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
			image.texture.LoadImage(imageData);
			image.w = image.texture.width;
			image.h = image.texture.height;
			setTextureQuality(image);
		}
		catch (Exception)
		{
			Cout.LogError("CREAT IMAGE FROM ARRAY FAIL \n" + Environment.StackTrace);
		}
		return image;
	}

	private static Image __createImage(Image src, int x, int y, int w, int h, int transform)
	{
		if (src == null || src.texture == null)
		{
			return null;
		}
		Image image = new Image();
		image.texture = new Texture2D(w, h, TextureFormat.RGBA32, false);
		y = src.texture.height - y - h;
		for (int i = 0; i < w; i++)
		{
			for (int j = 0; j < h; j++)
			{
				int num = i;
				if (transform == 2)
				{
					num = w - i;
				}
				int num2 = j;
				image.texture.SetPixel(i, j, src.texture.GetPixel(x + num, y + num2));
			}
		}
		image.texture.Apply();
		image.w = image.texture.width;
		image.h = image.texture.height;
		setTextureQuality(image);
		return image;
	}

	private static Image __createEmptyImage()
	{
		return new Image();
	}

	public static Image __createImage(int w, int h)
	{
		Image image = new Image();
		image.texture = new Texture2D(w, h, TextureFormat.RGBA32, mipmap: false);
		setTextureQuality(image);
		image.w = w;
		image.h = h;
		image.texture.Apply();
		return image;
	}

	public static int getImageWidth(Image image)
	{
		return (image != null) ? image.getWidth() : 0;
	}

	public static int getImageHeight(Image image)
	{
		return (image != null) ? image.getHeight() : 0;
	}

	public int getWidth()
	{
		return (texture != null ? w : 0) / mGraphics.zoomLevel;
	}

	public int getHeight()
	{
		return (texture != null ? h : 0) / mGraphics.zoomLevel;
	}

	private static void setTextureQuality(Image img)
	{
		if (img != null && img.texture != null)
		{
			setTextureQuality(img.texture);
		}
	}

	public static void setTextureQuality(Texture2D texture)
	{
		if (texture == null) return;
		texture.anisoLevel = 0;
		texture.filterMode = FilterMode.Point;
		texture.mipMapBias = 0f;
		texture.wrapMode = TextureWrapMode.Clamp;
	}

	public Color[] getColor()
	{
		return (texture != null) ? texture.GetPixels() : new Color[0];
	}

	public int getRealImageWidth()
	{
		return (texture != null) ? w : 0;
	}

	public int getRealImageHeight()
	{
		return (texture != null) ? h : 0;
	}

	public void getRGB(ref int[] data, int x1, int x2, int x, int y, int w, int h)
	{
		if (texture == null) return;
		Color[] pixels = texture.GetPixels(x, this.h - 1 - y, w, h);
		for (int i = 0; i < pixels.Length; i++)
		{
			data[i] = mGraphics.getIntByColor(pixels[i]);
		}
	}
}
