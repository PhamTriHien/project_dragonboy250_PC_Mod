using System;
using System.Collections;
using UnityEngine;

public partial class mFont
{
	public void setHeight(int height)
		{
			this.height = height;
		}

	public MyVector splitFontVector(string src, int lineWidth)
		{
			MyVector myVector = new MyVector();
			if (string.IsNullOrEmpty(src))
			{
				return myVector;
			}
			if (src.IndexOf('\n') < 0 && src.IndexOf('\b') < 0 && getWidth(src) <= lineWidth)
			{
				myVector.addElement(src);
				return myVector;
			}
			string text = string.Empty;
			for (int i = 0; i < src.Length; i++)
			{
				if (src[i] == '\n' || src[i] == '\b')
				{
					myVector.addElement(text);
					text = string.Empty;
					continue;
				}
				text += src[i];
				if (getWidth(text) > lineWidth)
				{
					int num = 0;
					num = text.Length - 1;
					while (num >= 0 && text[num] != ' ')
					{
						num--;
					}
					if (num < 0)
					{
						num = text.Length - 1;
					}
					myVector.addElement(text.Substring(0, num));
					i = i - (text.Length - num) + 1;
					text = string.Empty;
				}
				if (i == src.Length - 1 && !text.Trim().Equals(string.Empty))
				{
					myVector.addElement(text);
				}
			}
			return myVector;
		}

	public string splitFirst(string str)
		{
			string text = string.Empty;
			bool flag = false;
			for (int i = 0; i < str.Length; i++)
			{
				if (!flag)
				{
					string text2 = str.Substring(i);
					text = ((!compare(text2, " ")) ? (text + text2) : (text + str[i] + "-"));
					flag = true;
				}
				else if (str[i] == ' ')
				{
					flag = false;
				}
			}
			return text;
		}

	public string[] splitStrInLine(string src, int lineWidth)
		{
			ArrayList arrayList = splitStrInLineA(src, lineWidth);
			string[] array = new string[arrayList.Count];
			for (int i = 0; i < arrayList.Count; i++)
			{
				array[i] = (string)arrayList[i];
			}
			return array;
		}

	public ArrayList splitStrInLineA(string src, int lineWidth)
		{
			ArrayList arrayList = new ArrayList();
			int i = 0;
			int num = 0;
			int length = src.Length;
			if (length < 5)
			{
				arrayList.Add(src);
				return arrayList;
			}
			string text = string.Empty;
			try
			{
				while (true)
				{
					if (getWidthNotExactOf(text) < lineWidth)
					{
						text += src[num];
						num++;
						if (src[num] != '\n')
						{
							if (num < length - 1)
							{
								continue;
							}
							num = length - 1;
						}
					}
					if (num != length - 1 && src[num + 1] != ' ')
					{
						int num2 = num;
						while (src[num + 1] != '\n' && (src[num + 1] != ' ' || src[num] == ' ') && num != i)
						{
							num--;
						}
						if (num == i)
						{
							num = num2;
						}
					}
					string text2 = src.Substring(i, num + 1 - i);
					if (text2[0] == '\n')
					{
						text2 = text2.Substring(1, text2.Length - 1);
					}
					if (text2[text2.Length - 1] == '\n')
					{
						text2 = text2.Substring(0, text2.Length - 1);
					}
					arrayList.Add(text2);
					if (num == length - 1)
					{
						break;
					}
					for (i = num + 1; i != length - 1 && src[i] == ' '; i++)
					{
					}
					if (i == length - 1)
					{
						break;
					}
					num = i;
					text = string.Empty;
				}
			}
			catch (Exception ex)
			{
				Cout.LogWarning("EXCEPTION WHEN REAL SPLIT " + src + "\nend=" + num + "\n" + ex.Message + "\n" + ex.StackTrace);
				arrayList.Add(src);
			}
			return arrayList;
		}

	public string[] splitFontArray(string src, int lineWidth)
		{
			MyVector myVector = splitFontVector(src, lineWidth);
			string[] array = new string[myVector.size()];
			for (int i = 0; i < myVector.size(); i++)
			{
				array[i] = (string)myVector.elementAt(i);
			}
			return array;
		}

	public bool compare(string strSource, string str)
		{
			for (int i = 0; i < strSource.Length; i++)
			{
				if ((string.Empty + strSource[i]).Equals(str))
				{
					return true;
				}
			}
			return false;
		}

	public int getWidth(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return 0;
			}
			if ((mGraphics.zoomLevel == 1 || ModGraphics.IsJavaGraphics) && fImages != null && fImages.Length > 0 && !string.IsNullOrEmpty(strFont))
			{
				int num = 0;
				for (int i = 0; i < s.Length; i++)
				{
					if (char.IsLowSurrogate(s[i]))
					{
						continue;
					}
					int num2 = ResolveJavaGlyphIndex(s[i]);
					num += fImages[num2][2] + space;
				}
				return num;
			}
			if (ModGraphics.IsIosGraphics && IosBitmapFont.HasMetrics(id))
			{
				int iosW = IosBitmapFont.MeasureWidth(id, s);
				if (iosW >= 0)
				{
					return iosW;
				}
			}
			return getWidthExactOf(s);
		}

	private static readonly System.Collections.Generic.List<mFont> s_AllInstances = new System.Collections.Generic.List<mFont>(40);
	private readonly System.Collections.Generic.Dictionary<string, int> widthCache = new System.Collections.Generic.Dictionary<string, int>(128);
	private static readonly GUIStyle s_CachedMeasureStyle = new GUIStyle();
	private static readonly GUIContent s_CachedMeasureContent = new GUIContent(string.Empty);

	public int getWidthExactOf(string s)
		{
			try
			{
				if (string.IsNullOrEmpty(s)) return 0;
				int cachedWidth;
				if (widthCache.TryGetValue(s, out cachedWidth))
				{
					return cachedWidth;
				}
				if (ModGraphics.IsIosGraphics && IosBitmapFont.HasMetrics(id))
				{
					int iosW = IosBitmapFont.MeasureWidth(id, s);
					if (iosW >= 0)
					{
						if (widthCache.Count < 512)
						{
							widthCache[s] = iosW;
						}
						return iosW;
					}
				}
				s_CachedMeasureStyle.font = myFont;
				s_CachedMeasureContent.text = s;
				int zoom = (mGraphics.zoomLevel > 0) ? mGraphics.zoomLevel : 1;
				float scale = ModGraphics.GetFontSizeScale();
				float rawW;
				if (myFont != null && myFont.dynamic && Mathf.Abs(scale - 1f) > 0.001f)
				{
					int refSize = (myFont.fontSize > 0) ? myFont.fontSize : (14 * zoom);
					s_CachedMeasureStyle.fontSize = Mathf.Max(7, Mathf.RoundToInt(refSize * scale));
					rawW = s_CachedMeasureStyle.CalcSize(s_CachedMeasureContent).x;
					s_CachedMeasureStyle.fontSize = 0;
				}
				else
				{
					s_CachedMeasureStyle.fontSize = 0;
					rawW = s_CachedMeasureStyle.CalcSize(s_CachedMeasureContent).x * scale;
				}
				int calculated = (int)(rawW / (float)zoom + 0.5f);
				if (widthCache.Count < 512)
				{
					widthCache[s] = calculated;
				}
				return calculated;
			}
			catch (Exception ex)
			{
				s_CachedMeasureStyle.fontSize = 0;
				Cout.LogError("GET WIDTH OF " + s + " FAIL.\n" + ex.Message + "\n" + ex.StackTrace);
				return getWidthNotExactOf(s);
			}
		}

	public int getWidthNotExactOf(string s)
		{
			if (string.IsNullOrEmpty(s))
			{
				return 0;
			}
			if ((mGraphics.zoomLevel == 1 || ModGraphics.IsJavaGraphics) && fImages != null && fImages.Length > 0 && !string.IsNullOrEmpty(strFont))
			{
				return getWidth(s);
			}
			if (ModGraphics.IsIosGraphics && IosBitmapFont.HasMetrics(id))
			{
				int iosW = IosBitmapFont.MeasureWidth(id, s);
				if (iosW >= 0)
				{
					return iosW;
				}
			}
			return s.Length * wO / mGraphics.zoomLevel;
		}

	public int getHeight()
		{
			if (mGraphics.zoomLevel == 1)
			{
				return (height > 0) ? height : javaHeight;
			}
			if (ModGraphics.IsJavaGraphics && javaHeight > 0)
			{
				return javaHeight;
			}
			if (ModGraphics.IsIosGraphics && IosBitmapFont.HasMetrics(id))
			{
				return IosBitmapFont.GetLineHeight(id);
			}
			if (height > 0)
			{
				return System.Math.Max(9, height / mGraphics.zoomLevel);
			}
			GUIStyle gUIStyle = new GUIStyle();
			gUIStyle.font = myFont;
			try
			{
				int zoom = (mGraphics.zoomLevel > 0) ? mGraphics.zoomLevel : 2;
				float scale = ModGraphics.GetFontSizeScale();
				if (myFont != null && myFont.dynamic && Mathf.Abs(scale - 1f) > 0.001f)
				{
					int refSize = (myFont.fontSize > 0) ? myFont.fontSize : (14 * zoom);
					gUIStyle.fontSize = Mathf.Max(7, Mathf.RoundToInt(refSize * scale));
					height = (int)gUIStyle.CalcSize(new GUIContent("Adg")).y + 4;
				}
				else
				{
					height = (int)(gUIStyle.CalcSize(new GUIContent("Adg")).y * scale) + 4;
				}
			}
			catch (Exception ex)
			{
				Cout.LogError("FAIL GET HEIGHT " + ex.StackTrace);
				height = 20;
			}
			return System.Math.Max(9, height / mGraphics.zoomLevel);
		}

	public static void InvalidateFontMetrics()
		{
			for (int i = 0; i < s_AllInstances.Count; i++)
			{
				mFont f = s_AllInstances[i];
				if (f != null)
				{
					f.widthCache.Clear();
					f.height = 0;
					f.wO = f.getWidthExactOf("o");
				}
			}
		}

	public static string[] splitStringSv(string _text, string _searchStr)
		{
			int num = 0;
			int startIndex = 0;
			int length = _searchStr.Length;
			int num2 = _text.IndexOf(_searchStr, startIndex);
			while (num2 != -1)
			{
				startIndex = num2 + length;
				num2 = _text.IndexOf(_searchStr, startIndex);
				num++;
			}
			string[] array = new string[num + 1];
			int num3 = _text.IndexOf(_searchStr);
			int num4 = 0;
			int num5 = 0;
			while (num3 != -1)
			{
				array[num5] = _text.Substring(num4, num3 - num4);
				num4 = num3 + length;
				num3 = _text.IndexOf(_searchStr, num4);
				num5++;
			}
			array[num5] = _text.Substring(num4, _text.Length - num4);
			return array;
		}

}
