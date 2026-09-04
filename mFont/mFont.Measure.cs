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
			if (mGraphics.zoomLevel == 1)
			{
				int num = 0;
				for (int i = 0; i < s.Length; i++)
				{
					int num2 = strFont.IndexOf(s[i]);
					if (num2 == -1)
					{
						num2 = 0;
					}
					num += fImages[num2][2] + space;
				}
				return num;
			}
			return getWidthExactOf(s);
		}

	public int getWidthExactOf(string s)
		{
			try
			{
				GUIStyle gUIStyle = new GUIStyle();
				gUIStyle.font = myFont;
				return (int)gUIStyle.CalcSize(new GUIContent(s)).x / mGraphics.zoomLevel;
			}
			catch (Exception ex)
			{
				Cout.LogError("GET WIDTH OF " + s + " FAIL.\n" + ex.Message + "\n" + ex.StackTrace);
				return getWidthNotExactOf(s);
			}
		}

	public int getWidthNotExactOf(string s)
		{
			return s.Length * wO / mGraphics.zoomLevel;
		}

	public int getHeight()
		{
			if (mGraphics.zoomLevel == 1)
			{
				return height;
			}
			if (height > 0)
			{
				return height / mGraphics.zoomLevel;
			}
			GUIStyle gUIStyle = new GUIStyle();
			gUIStyle.font = myFont;
			try
			{
				height = (int)gUIStyle.CalcSize(new GUIContent("Adg")).y + 2;
			}
			catch (Exception ex)
			{
				Cout.LogError("FAIL GET HEIGHT " + ex.StackTrace);
				height = 20;
			}
			return height / mGraphics.zoomLevel;
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
