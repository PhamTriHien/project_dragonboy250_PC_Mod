using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

public static class IosBitmapFont
{
	private struct Glyph
	{
		public bool valid;
		public short width;
		public short height;
		public short xoffset;
		public short yoffset;
		public short xadvance;
		public float u;
		public float v;
		public float uw;
		public float vh;
	}

	private sealed class FontFace
	{
		public string name;
		public int lineHeight;
		public int baseLine;
		public int scaleW;
		public int scaleH;
		public int yBias;
		public readonly Glyph[] ascii = new Glyph[128];
		public readonly Dictionary<int, Glyph> unicode = new Dictionary<int, Glyph>(256);
		public Texture2D texture;
		public bool metricsLoaded;
	}

	private const int FaceBarmeno = 0;
	private const int FaceChelthm = 1;
	private const int FaceStaccato = 2;

	private static readonly FontFace[] s_Faces = new FontFace[3]
	{
		new FontFace { name = "barmeneb", lineHeight = 48, baseLine = 36, scaleW = 512, scaleH = 512 },
		new FontFace { name = "chelthm7", lineHeight = 46, baseLine = 39, scaleW = 512, scaleH = 512 },
		new FontFace { name = "staccato", lineHeight = 45, baseLine = 33, scaleW = 512, scaleH = 512 }
	};

	private static int ResolveFaceIndex(sbyte fontId)
	{
		if ((fontId >= 1 && fontId <= 9) || fontId == 19)
		{
			return FaceBarmeno;
		}
		if (fontId >= 25)
		{
			return FaceStaccato;
		}
		return FaceChelthm;
	}

	private static float ResolveBaseScale(sbyte fontId)
	{
		if (fontId == 19)
		{
			return 1.05f;
		}
		if (fontId >= 1 && fontId <= 9)
		{
			return 0.90f;
		}
		if (fontId >= 20 && fontId <= 24)
		{
			return 0.88f;
		}
		if (fontId >= 25)
		{
			return 1.00f;
		}
		return 0.95f;
	}

	private static float ResolveLogicalScale(sbyte fontId)
	{
		float userRatio = ModGraphics.GetFontSizeScale() / 0.75f;
		return (ResolveBaseScale(fontId) * 0.25f) * userRatio;
	}

	public static void ReloadTextures()
	{
		for (int i = 0; i < s_Faces.Length; i++)
		{
			s_Faces[i].texture = null;
		}
	}

	private static byte[] ReadEmbeddedBytes(string resourceName)
	{
		try
		{
			using (Stream stream = typeof(IosBitmapFont).Assembly.GetManifestResourceStream(resourceName))
			{
				if (stream == null)
				{
					return null;
				}
				byte[] buffer = new byte[stream.Length];
				int offset = 0;
				while (offset < buffer.Length)
				{
					int read = stream.Read(buffer, offset, buffer.Length - offset);
					if (read <= 0)
					{
						break;
					}
					offset += read;
				}
				return buffer;
			}
		}
		catch
		{
			return null;
		}
	}

	private static FontFace EnsureFaceLoaded(sbyte fontId)
	{
		FontFace face = s_Faces[ResolveFaceIndex(fontId)];
		if (!face.metricsLoaded)
		{
			LoadMetrics(face);
		}
		if (face.texture == null)
		{
			Texture2D loaded = Resources.Load("ios_fonts/x4/" + face.name, typeof(Texture2D)) as Texture2D;
			if (loaded == null)
			{
				byte[] pngBytes = ReadEmbeddedBytes("ios_fonts/x4/" + face.name + ".png");
				if (pngBytes != null && pngBytes.Length > 0)
				{
					Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, true);
					if (tex.LoadImage(pngBytes))
					{
						tex.wrapMode = TextureWrapMode.Clamp;
						tex.filterMode = FilterMode.Trilinear;
						tex.anisoLevel = 2;
						loaded = tex;
					}
				}
			}
			face.texture = loaded;
		}
		return (face.metricsLoaded && face.texture != null) ? face : null;
	}

	public static bool CanRender(sbyte fontId)
	{
		if (!ModGraphics.IsIosGraphics)
		{
			return false;
		}
		return EnsureFaceLoaded(fontId) != null;
	}

	public static bool ContainsAllGlyphs(sbyte fontId, string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return true;
		}
		FontFace face = EnsureFaceLoaded(fontId);
		if (face == null)
		{
			return false;
		}
		Glyph unused;
		for (int i = 0; i < text.Length; i++)
		{
			char ch = text[i];
			if (ch == '\r' || ch == '\n' || ch == '\t')
			{
				continue;
			}
			if (char.IsHighSurrogate(ch) && i + 1 < text.Length && char.IsLowSurrogate(text[i + 1]))
			{
				return false;
			}
			if (!TryGetGlyph(face, ch, out unused))
			{
				return false;
			}
		}
		return true;
	}

	public static int MeasureWidth(sbyte fontId, string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return 0;
		}
		FontFace face = s_Faces[ResolveFaceIndex(fontId)];
		if (!face.metricsLoaded)
		{
			LoadMetrics(face);
			if (!face.metricsLoaded)
			{
				return -1;
			}
		}
		int sumAdvance = 0;
		Glyph sp;
		Glyph g;
		Glyph fallback;
		for (int i = 0; i < text.Length; i++)
		{
			char ch = text[i];
			if (char.IsLowSurrogate(ch) || ch == '\r' || ch == '\n')
			{
				continue;
			}
			if (ch == '\t')
			{
				if (TryGetGlyph(face, ' ', out sp))
				{
					sumAdvance += sp.xadvance * 4;
				}
				continue;
			}
			if (TryGetGlyph(face, ch, out g))
			{
				sumAdvance += g.xadvance;
			}
			else if (TryGetGlyph(face, 'o', out fallback))
			{
				sumAdvance += fallback.xadvance;
			}
			else
			{
				sumAdvance += 22;
			}
		}
		float w = sumAdvance * ResolveLogicalScale(fontId);
		return (int)(w + 0.5f);
	}

	public static int GetLineHeight(sbyte fontId)
	{
		FontFace face = s_Faces[ResolveFaceIndex(fontId)];
		if (!face.metricsLoaded)
		{
			LoadMetrics(face);
		}
		int h = (int)(face.lineHeight * ResolveLogicalScale(fontId) + 0.5f);
		return System.Math.Max(9, h);
	}

	public static bool DrawString(mGraphics g, sbyte fontId, Color tint, string text, int x, int y, int align)
	{
		if (string.IsNullOrEmpty(text))
		{
			return true;
		}
		FontFace face = EnsureFaceLoaded(fontId);
		if (face == null || !ContainsAllGlyphs(fontId, text))
		{
			return false;
		}

		int zoom = (mGraphics.zoomLevel > 0) ? mGraphics.zoomLevel : 2;
		float logicalScale = ResolveLogicalScale(fontId);
		float screenScale = logicalScale * zoom;

		int sumAdvance = 0;
		Glyph sp;
		Glyph mg;
		for (int i = 0; i < text.Length; i++)
		{
			char ch = text[i];
			if (ch == '\r' || ch == '\n') continue;
			if (ch == '\t')
			{
				if (TryGetGlyph(face, ' ', out sp)) sumAdvance += sp.xadvance * 4;
				continue;
			}
			if (TryGetGlyph(face, ch, out mg))
			{
				sumAdvance += mg.xadvance;
			}
		}

		float totalScreenW = sumAdvance * screenScale;
		float startX = x * (float)zoom;
		float startY = y * (float)zoom;
		if (g.isTranslate)
		{
			startX += g.translateX;
			startY += g.translateY;
		}

		if (align == 1)
		{
			startX -= totalScreenW;
		}
		else if (align != 0)
		{
			startX -= totalScreenW * 0.5f;
		}

		bool hasClip = g.isClip;
		float cx = 0f, cy = 0f, cw = 0f, chClip = 0f;
		if (hasClip)
		{
			cx = g.clipX;
			cy = g.clipY;
			cw = g.clipW;
			chClip = g.clipH;
			if (g.isTranslate)
			{
				cx += g.clipTX;
				cy += g.clipTY;
			}
			if (cw <= 0f || chClip <= 0f)
			{
				return true;
			}
		}

		Color prevColor = GUI.color;
		GUI.color = tint;
		Texture2D atlas = face.texture;
		float cursorX = startX;
		Glyph gl;

		for (int i = 0; i < text.Length; i++)
		{
			char c = text[i];
			if (c == '\r' || c == '\n')
			{
				continue;
			}
			if (c == '\t')
			{
				if (TryGetGlyph(face, ' ', out sp))
				{
					cursorX += sp.xadvance * 4 * screenScale;
				}
				continue;
			}
			if (!TryGetGlyph(face, c, out gl))
			{
				continue;
			}

			if (gl.width > 0 && gl.height > 0)
			{
				float dx = cursorX + gl.xoffset * screenScale;
				float dy = startY + (gl.yoffset + face.yBias) * screenScale;
				float dw = gl.width * screenScale;
				float dh = gl.height * screenScale;

				float u = gl.u;
				float v = gl.v;
				float uw = gl.uw;
				float vh = gl.vh;

				if (hasClip)
				{
					float rx1 = (dx > cx) ? dx : cx;
					float ry1 = (dy > cy) ? dy : cy;
					float rx2 = ((dx + dw) < (cx + cw)) ? (dx + dw) : (cx + cw);
					float ry2 = ((dy + dh) < (cy + chClip)) ? (dy + dh) : (cy + chClip);
					if (rx2 <= rx1 || ry2 <= ry1)
					{
						cursorX += gl.xadvance * screenScale;
						continue;
					}
					if (rx1 > dx || ry1 > dy || rx2 < dx + dw || ry2 < dy + dh)
					{
						float leftCut = (rx1 - dx) / dw;
						float rightCut = (dx + dw - rx2) / dw;
						float topCut = (ry1 - dy) / dh;
						float bottomCut = (dy + dh - ry2) / dh;

						u += uw * leftCut;
						uw *= (1f - leftCut - rightCut);
						v += vh * bottomCut;
						vh *= (1f - topCut - bottomCut);

						dx = rx1;
						dy = ry1;
						dw = rx2 - rx1;
						dh = ry2 - ry1;
					}
				}

				GUI.DrawTextureWithTexCoords(new Rect(dx, dy, dw, dh), atlas, new Rect(u, v, uw, vh), true);
			}

			cursorX += gl.xadvance * screenScale;
		}

		GUI.color = prevColor;
		return true;
	}

	private static bool TryGetGlyph(FontFace face, int codePoint, out Glyph glyph)
	{
		if ((uint)codePoint < 128u)
		{
			glyph = face.ascii[codePoint];
			return glyph.valid;
		}
		return face.unicode.TryGetValue(codePoint, out glyph);
	}

	private static void LoadMetrics(FontFace face)
	{
		try
		{
			byte[] rawBytes = null;
			TextAsset ta = Resources.Load("ios_fonts/x4/" + face.name + ".fnt", typeof(TextAsset)) as TextAsset;
			if (ta != null && ta.bytes != null && ta.bytes.Length > 0)
			{
				rawBytes = ta.bytes;
			}
			else
			{
				rawBytes = ReadEmbeddedBytes("ios_fonts/x4/" + face.name + ".fnt");
			}
			if (rawBytes == null || rawBytes.Length == 0)
			{
				return;
			}
			string content = Encoding.UTF8.GetString(rawBytes);
			ParseFntContent(face, content);
		}
		catch
		{
		}
	}

	private static void ParseFntContent(FontFace face, string content)
	{
		if (string.IsNullOrEmpty(content))
		{
			return;
		}
		Array.Clear(face.ascii, 0, face.ascii.Length);
		face.unicode.Clear();

		string[] lines = content.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
		for (int i = 0; i < lines.Length; i++)
		{
			string line = lines[i].Trim();
			if (line.StartsWith("common ", StringComparison.Ordinal))
			{
				face.lineHeight = ReadAttr(line, "lineHeight=", face.lineHeight);
				face.baseLine = ReadAttr(line, "base=", face.baseLine);
				face.scaleW = System.Math.Max(1, ReadAttr(line, "scaleW=", face.scaleW));
				face.scaleH = System.Math.Max(1, ReadAttr(line, "scaleH=", face.scaleH));
			}
			else if (line.StartsWith("char id=", StringComparison.Ordinal))
			{
				int id = ReadAttr(line, "id=", -1);
				if (id < 0) continue;
				int gx = ReadAttr(line, "x=", 0);
				int gy = ReadAttr(line, "y=", 0);
				int gw = ReadAttr(line, "width=", 0);
				int gh = ReadAttr(line, "height=", 0);
				int xo = ReadAttr(line, "xoffset=", 0);
				int yo = ReadAttr(line, "yoffset=", 0);
				int xa = ReadAttr(line, "xadvance=", 0);

				Glyph g = new Glyph
				{
					valid = true,
					width = (short)gw,
					height = (short)gh,
					xoffset = (short)xo,
					yoffset = (short)yo,
					xadvance = (short)xa,
					u = gx / (float)face.scaleW,
					v = 1f - (gy + gh) / (float)face.scaleH,
					uw = gw / (float)face.scaleW,
					vh = gh / (float)face.scaleH
				};

				if ((uint)id < 128u)
				{
					face.ascii[id] = g;
				}
				else
				{
					face.unicode[id] = g;
				}
			}
		}

		if (face.ascii['A'].valid)
		{
			face.yBias = 8 - face.ascii['A'].yoffset;
		}
		else
		{
			face.yBias = 0;
		}
		face.metricsLoaded = face.unicode.Count > 0 || face.ascii['A'].valid;
	}

	private static int ReadAttr(string line, string key, int defaultValue)
	{
		int idx = line.IndexOf(key, StringComparison.Ordinal);
		if (idx < 0)
		{
			return defaultValue;
		}
		int start = idx + key.Length;
		while (start < line.Length && line[start] == ' ')
		{
			start++;
		}
		int end = start;
		if (end < line.Length && (line[end] == '-' || line[end] == '+'))
		{
			end++;
		}
		while (end < line.Length && line[end] >= '0' && line[end] <= '9')
		{
			end++;
		}
		if (end <= start)
		{
			return defaultValue;
		}
		int val;
		return int.TryParse(line.Substring(start, end - start), NumberStyles.Integer, CultureInfo.InvariantCulture, out val)
			? val
			: defaultValue;
	}
}
