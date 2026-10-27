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
		public readonly object syncRoot = new object();
		public string name;
		public int lineHeight;
		public int baseLine;
		public int scaleW;
		public int scaleH;
		public int yBias;
		public readonly Glyph[] ascii = new Glyph[128];
		public readonly Dictionary<int, Glyph> unicode = new Dictionary<int, Glyph>(256);
		public Texture2D texture;
		public volatile bool metricsLoaded;
		public volatile bool metricsAttempted;
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

	private static void EnsureAllMetricsLoaded()
	{
		for (int i = 0; i < s_Faces.Length; i++)
		{
			EnsureMetricsLoaded(s_Faces[i]);
		}
	}

	private static bool EnsureMetricsLoaded(FontFace face)
	{
		if (face.metricsLoaded)
		{
			return true;
		}
		if (face.metricsAttempted)
		{
			return face.metricsLoaded;
		}
		lock (face.syncRoot)
		{
			if (face.metricsLoaded)
			{
				return true;
			}
			if (!face.metricsAttempted)
			{
				LoadMetrics(face);
				face.metricsAttempted = true;
			}
			return face.metricsLoaded;
		}
	}

	private static void NormalizeWhiteAlphaMask(Texture2D tex)
	{
		if (tex == null)
		{
			return;
		}
		try
		{
			Color32[] pixels = tex.GetPixels32();
			if (pixels != null && pixels.Length > 0)
			{
				for (int i = 0; i < pixels.Length; i++)
				{
					pixels[i] = new Color32(255, 255, 255, pixels[i].a);
				}
				tex.SetPixels32(pixels);
				tex.Apply(true);
			}
		}
		catch
		{
		}
	}

	private static bool EnsureTextureReady(FontFace face)
	{
		if (face.texture != null)
		{
			return true;
		}
		try
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
						NormalizeWhiteAlphaMask(tex);
						tex.wrapMode = TextureWrapMode.Clamp;
						tex.filterMode = FilterMode.Trilinear;
						tex.anisoLevel = 2;
						loaded = tex;
					}
				}
			}
			if (loaded != null)
			{
				face.texture = loaded;
				return true;
			}
		}
		catch
		{
		}
		face.texture = null;
		return false;
	}

	public static bool HasMetrics(sbyte fontId)
	{
		if (!ModGraphics.IsIosGraphics)
		{
			return false;
		}
		EnsureAllMetricsLoaded();
		return s_Faces[ResolveFaceIndex(fontId)].metricsLoaded;
	}

	public static bool CanRender(sbyte fontId)
	{
		if (!ModGraphics.IsIosGraphics)
		{
			return false;
		}
		EnsureAllMetricsLoaded();
		FontFace face = s_Faces[ResolveFaceIndex(fontId)];
		return face.metricsLoaded && EnsureTextureReady(face);
	}

	public static bool ContainsAllGlyphs(sbyte fontId, string text)
	{
		return HasMetrics(fontId);
	}

	public static int MeasureWidth(sbyte fontId, string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return 0;
		}
		EnsureAllMetricsLoaded();
		FontFace face = s_Faces[ResolveFaceIndex(fontId)];
		if (!face.metricsLoaded)
		{
			return -1;
		}

		string norm = NormalizeText(text);
		float sumAdvance = 0f;
		Glyph sp;
		FontFace unusedFace;
		Glyph g;
		float faceScale;
		for (int i = 0; i < norm.Length; i++)
		{
			char ch = norm[i];
			if (ch == '\r' || ch == '\n')
			{
				continue;
			}
			if (ch == '\t')
			{
				if (TryGetGlyph(face, ' ', out sp))
				{
					sumAdvance += sp.xadvance * 4f;
				}
				continue;
			}
			int cp = ch;
			if (char.IsHighSurrogate(ch) && i + 1 < norm.Length && char.IsLowSurrogate(norm[i + 1]))
			{
				cp = char.ConvertToUtf32(ch, norm[i + 1]);
				i++;
			}
			else if (char.IsSurrogate(ch))
			{
				continue;
			}

			TryResolveGlyph(face, cp, false, out unusedFace, out g, out faceScale);
			sumAdvance += g.xadvance * faceScale;
		}

		float w = sumAdvance * ResolveLogicalScale(fontId);
		return (int)(w + 0.5f);
	}

	public static int GetLineHeight(sbyte fontId)
	{
		EnsureAllMetricsLoaded();
		FontFace face = s_Faces[ResolveFaceIndex(fontId)];
		int h = (int)(face.lineHeight * ResolveLogicalScale(fontId) + 0.5f);
		return System.Math.Max(9, h);
	}

	public static bool DrawString(mGraphics g, sbyte fontId, Color tint, string text, int x, int y, int align)
	{
		if (string.IsNullOrEmpty(text))
		{
			return true;
		}
		EnsureAllMetricsLoaded();
		FontFace face = s_Faces[ResolveFaceIndex(fontId)];
		if (!face.metricsLoaded || !EnsureTextureReady(face))
		{
			return false;
		}

		string norm = NormalizeText(text);
		if (norm.Length == 0)
		{
			return true;
		}

		int zoom = (mGraphics.zoomLevel > 0) ? mGraphics.zoomLevel : 2;
		float logicalScale = ResolveLogicalScale(fontId);
		float screenScale = logicalScale * zoom;

		float sumAdvance = 0f;
		Glyph sp;
		FontFace unusedFace;
		Glyph mg;
		float mScale;
		for (int i = 0; i < norm.Length; i++)
		{
			char ch = norm[i];
			if (ch == '\r' || ch == '\n')
			{
				continue;
			}
			if (ch == '\t')
			{
				if (TryGetGlyph(face, ' ', out sp))
				{
					sumAdvance += sp.xadvance * 4f;
				}
				continue;
			}
			int cp = ch;
			if (char.IsHighSurrogate(ch) && i + 1 < norm.Length && char.IsLowSurrogate(norm[i + 1]))
			{
				cp = char.ConvertToUtf32(ch, norm[i + 1]);
				i++;
			}
			else if (char.IsSurrogate(ch))
			{
				continue;
			}

			TryResolveGlyph(face, cp, false, out unusedFace, out mg, out mScale);
			sumAdvance += mg.xadvance * mScale;
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
			if (cw <= 0f || chClip <= 0f)
			{
				return true;
			}
		}

		Color prevColor = GUI.color;
		GUI.color = tint;
		float cursorX = startX;
		FontFace glyphFace;
		Glyph gl;
		float faceScale;

		for (int i = 0; i < norm.Length; i++)
		{
			char c = norm[i];
			if (c == '\r' || c == '\n')
			{
				continue;
			}
			if (c == '\t')
			{
				if (TryGetGlyph(face, ' ', out sp))
				{
					cursorX += sp.xadvance * 4f * screenScale;
				}
				continue;
			}

			int cp = c;
			if (char.IsHighSurrogate(c) && i + 1 < norm.Length && char.IsLowSurrogate(norm[i + 1]))
			{
				cp = char.ConvertToUtf32(c, norm[i + 1]);
				i++;
			}
			else if (char.IsSurrogate(c))
			{
				continue;
			}

			TryResolveGlyph(face, cp, true, out glyphFace, out gl, out faceScale);
			float gScale = screenScale * faceScale;
			Texture2D atlas = (glyphFace != null && glyphFace.texture != null) ? glyphFace.texture : face.texture;

			if (gl.valid && gl.width > 0 && gl.height > 0 && atlas != null)
			{
				int yBias = (glyphFace != null) ? glyphFace.yBias : face.yBias;
				float dx = cursorX + gl.xoffset * gScale;
				float dy = startY + (gl.yoffset + yBias) * gScale;
				float dw = gl.width * gScale;
				float dh = gl.height * gScale;

				float u = gl.u;
				float v = gl.v;
				float uw = gl.uw;
				float vh = gl.vh;

				bool clippedOut = false;
				if (hasClip)
				{
					float rx1 = (dx > cx) ? dx : cx;
					float ry1 = (dy > cy) ? dy : cy;
					float rx2 = ((dx + dw) < (cx + cw)) ? (dx + dw) : (cx + cw);
					float ry2 = ((dy + dh) < (cy + chClip)) ? (dy + dh) : (cy + chClip);
					if (rx2 <= rx1 || ry2 <= ry1)
					{
						clippedOut = true;
					}
					else if (rx1 > dx || ry1 > dy || rx2 < dx + dw || ry2 < dy + dh)
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

				if (!clippedOut)
				{
					GUI.DrawTextureWithTexCoords(new Rect(dx, dy, dw, dh), atlas, new Rect(u, v, uw, vh), true);
				}
			}

			cursorX += gl.xadvance * gScale;
		}

		GUI.color = prevColor;
		return true;
	}

	private static bool NeedsTextNormalization(string text)
	{
		for (int i = 0; i < text.Length; i++)
		{
			char c = text[i];
			if (c < 0x20)
			{
				if (c != '\t' && c != '\r' && c != '\n')
				{
					return true;
				}
				continue;
			}
			if (c < 0x7F)
			{
				continue;
			}
			if (c <= 0xA0 || c == 0x00AD)
			{
				return true;
			}
			if ((c >= 0x0300 && c <= 0x036F) || (c >= 0x1DC0 && c <= 0x1DFF) || (c >= 0xFE20 && c <= 0xFE2F))
			{
				return true;
			}
			if (c >= 0x1680)
			{
				if (c >= 0x1EA0 && c <= 0x1EF9)
				{
					continue;
				}
				if (c == 0x1680 ||
					(c >= 0x2000 && c <= 0x200F) ||
					(c >= 0x2028 && c <= 0x202F) ||
					(c >= 0x205F && c <= 0x206F) ||
					c == 0x3000 ||
					(c >= 0xFE00 && c <= 0xFE0F) ||
					c == 0xFEFF ||
					c == '\u2026' || c == '\u22EF' ||
					c == '\u2192' || c == '\u2190' || c == '\u2194' ||
					c == '\u21D2' || c == '\u21D0' ||
					c == '\u2794' || c == '\u279C' || c == '\u279D' || c == '\u279E' ||
					c == '\u27F5' || c == '\u27F6' || c == '\u27F7' || c == '\u27F9' ||
					c == '\u2264' || c == '\u2A7D' || c == '\u2265' || c == '\u2A7E' ||
					c == '\u2260' || c == '\u2122' || c == '\u00A9' || c == '\u00AE')
				{
					return true;
				}
			}
			if (c == '\u00A9' || c == '\u00AE')
			{
				return true;
			}
		}
		return false;
	}

	private static string NormalizeText(string text)
	{
		if (string.IsNullOrEmpty(text) || !NeedsTextNormalization(text))
		{
			return text;
		}

		bool hasCombining = false;
		for (int i = 0; i < text.Length; i++)
		{
			char c = text[i];
			if ((c >= 0x0300 && c <= 0x036F) || (c >= 0x1DC0 && c <= 0x1DFF) || (c >= 0xFE20 && c <= 0xFE2F))
			{
				hasCombining = true;
				break;
			}
		}
		if (hasCombining)
		{
			try
			{
				text = text.Normalize(NormalizationForm.FormC);
			}
			catch
			{
			}
		}

		StringBuilder sb = new StringBuilder(text.Length + 8);
		for (int i = 0; i < text.Length; i++)
		{
			char c = text[i];
			if ((c < 0x20 && c != '\t' && c != '\r' && c != '\n') ||
				(c >= 0x7F && c <= 0x9F) ||
				c == 0x00AD ||
				(c >= 0x200B && c <= 0x200F) ||
				(c >= 0x202A && c <= 0x202E) ||
				(c >= 0x2060 && c <= 0x206F) ||
				(c >= 0xFE00 && c <= 0xFE0F) ||
				c == 0xFEFF)
			{
				continue;
			}
			if (c == 0x00A0 || c == 0x1680 || (c >= 0x2000 && c <= 0x200A) || c == 0x202F || c == 0x205F || c == 0x3000)
			{
				sb.Append(' ');
				continue;
			}
			if (c == 0x2028 || c == 0x2029)
			{
				sb.Append('\n');
				continue;
			}
			switch (c)
			{
				case '\u2026':
				case '\u22EF':
					sb.Append("...");
					break;
				case '\u2192':
				case '\u2794':
				case '\u279C':
				case '\u279D':
				case '\u279E':
				case '\u27F6':
					sb.Append("->");
					break;
				case '\u2190':
				case '\u27F5':
					sb.Append("<-");
					break;
				case '\u2194':
				case '\u27F7':
					sb.Append("<->");
					break;
				case '\u21D2':
				case '\u27F9':
					sb.Append("=>");
					break;
				case '\u21D0':
				case '\u2264':
				case '\u2A7D':
					sb.Append("<=");
					break;
				case '\u2265':
				case '\u2A7E':
					sb.Append(">=");
					break;
				case '\u2260':
					sb.Append("!=");
					break;
				case '\u2122':
					sb.Append("TM");
					break;
				case '\u00A9':
					sb.Append("(c)");
					break;
				case '\u00AE':
					sb.Append("(R)");
					break;
				default:
					sb.Append(c);
					break;
			}
		}
		return sb.ToString();
	}

	private static int MapSingleCharEquivalent(int cp)
	{
		if (cp >= 0xFF01 && cp <= 0xFF5E)
		{
			return cp - 0xFEE0;
		}
		switch (cp)
		{
			case '\u25CF':
			case '\u25CB':
			case '\u25C9':
			case '\u25CE':
			case '\u25AA':
			case '\u25AB':
			case '\u25A0':
			case '\u25A1':
			case '\u25C6':
			case '\u25C7':
			case '\u00B7':
			case '\u2219':
			case '\u22C5':
			case '\u2043':
				return '\u2022';
			case '\u2605':
			case '\u2606':
			case '\u2726':
			case '\u2727':
			case '\u2729':
			case '\u272A':
			case '\u272B':
			case '\u2B50':
			case '\u2736':
				return '*';
			case '\u2010':
			case '\u2011':
			case '\u2012':
			case '\u2015':
			case '\u2212':
			case '\uFE63':
				return '\u2013';
			case '\u2018':
			case '\u2019':
			case '\u201A':
			case '\u201B':
			case '\u2032':
			case '\u00B4':
			case '\u02BC':
				return '\'';
			case '\u201C':
			case '\u201D':
			case '\u201E':
			case '\u201F':
			case '\u2033':
				return '"';
			case '\u2039':
			case '\u25C0':
			case '\u25C4':
			case '\u25C2':
			case '\u25C3':
			case '\u276E':
				return '<';
			case '\u203A':
			case '\u25B6':
			case '\u25BA':
			case '\u25B8':
			case '\u25B9':
			case '\u276F':
				return '>';
			case '\u25B2':
			case '\u25B4':
			case '\u2191':
				return '^';
			case '\u25BC':
			case '\u25BE':
			case '\u2193':
			case '\u2713':
			case '\u2714':
			case '\u2705':
				return 'v';
			case '\u00D7':
			case '\u2715':
			case '\u2716':
			case '\u2717':
			case '\u2718':
			case '\u274C':
				return 'x';
			case '\u00F7':
				return '/';
			case '\u2248':
			case '\u223C':
			case '\u301C':
				return '~';
			case '\u00B1':
				return '+';
			case '\u00B0':
			case '\u00BA':
				return 'o';
			case '\u26A1':
				return '!';
			case '\u2070':
			case '\u2080':
				return '0';
			case '\u2081':
				return '1';
			case '\u2082':
				return '2';
			case '\u2083':
				return '3';
			case '\u2074':
			case '\u2084':
				return '4';
			case '\u2075':
			case '\u2085':
				return '5';
			case '\u2076':
			case '\u2086':
				return '6';
			case '\u2077':
			case '\u2087':
				return '7';
			case '\u2078':
			case '\u2088':
				return '8';
			case '\u2079':
			case '\u2089':
				return '9';
			default:
				return cp;
		}
	}

	private static int MapAsciiFallback(int cp)
	{
		switch (cp)
		{
			case '\u2022':
				return '*';
			case '\u2013':
			case '\u2014':
				return '-';
			case '\u00AB':
				return '<';
			case '\u00BB':
				return '>';
			case '\u00B9':
				return '1';
			case '\u00B2':
				return '2';
			case '\u00B3':
				return '3';
			case '\u20AB':
				return 'd';
			default:
				return 0;
		}
	}

	private static bool TryResolveGlyph(FontFace primaryFace, int codePoint, bool requireTexture, out FontFace resolvedFace, out Glyph glyph, out float faceScale)
	{
		if (TryGetGlyph(primaryFace, codePoint, out glyph))
		{
			resolvedFace = primaryFace;
			faceScale = 1f;
			return true;
		}

		int mapped = MapSingleCharEquivalent(codePoint);
		if (mapped != codePoint && TryGetGlyph(primaryFace, mapped, out glyph))
		{
			resolvedFace = primaryFace;
			faceScale = 1f;
			return true;
		}

		for (int i = 0; i < s_Faces.Length; i++)
		{
			FontFace alt = s_Faces[i];
			if (alt == primaryFace || !alt.metricsLoaded)
			{
				continue;
			}
			if (TryGetGlyph(alt, codePoint, out glyph) || (mapped != codePoint && TryGetGlyph(alt, mapped, out glyph)))
			{
				if (!requireTexture || EnsureTextureReady(alt))
				{
					resolvedFace = alt;
					faceScale = (alt.lineHeight > 0) ? (primaryFace.lineHeight / (float)alt.lineHeight) : 1f;
					return true;
				}
			}
		}

		int asciiFallback = MapAsciiFallback(mapped);
		if (asciiFallback != 0 && TryGetGlyph(primaryFace, asciiFallback, out glyph))
		{
			resolvedFace = primaryFace;
			faceScale = 1f;
			return true;
		}

		if (TryGetGlyph(primaryFace, '?', out glyph) || TryGetGlyph(primaryFace, 'o', out glyph))
		{
			resolvedFace = primaryFace;
			faceScale = 1f;
			return false;
		}

		glyph = new Glyph { valid = false, xadvance = 22 };
		resolvedFace = primaryFace;
		faceScale = 1f;
		return false;
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

	private static void LoadMetrics(FontFace face)
	{
		try
		{
			byte[] rawBytes = ReadEmbeddedBytes("ios_fonts/x4/" + face.name + ".fnt");
			if (rawBytes == null || rawBytes.Length == 0)
			{
				try
				{
					TextAsset ta = Resources.Load("ios_fonts/x4/" + face.name + ".fnt", typeof(TextAsset)) as TextAsset;
					if (ta != null && ta.bytes != null && ta.bytes.Length > 0)
					{
						rawBytes = ta.bytes;
					}
				}
				catch
				{
				}
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
				if (id < 0)
				{
					continue;
				}
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
			face.yBias = 9 - face.ascii['A'].yoffset;
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
