using System;
using Assets.src.e;
using Assets.src.g;
using UnityEngine;
public partial class GameCanvas : IActionListener
{
	public static Image loadImageRMS(string path)
		{
			path = Main.res + "/x" + mGraphics.zoomLevel + path;
			path = cutPng(path);
			Image result = null;
			try
			{
				result = Image.createImage(path);
			}
			catch (Exception ex)
			{
				try
				{
					string[] array = Res.split(path, "/", 0);
					string filename = "x" + mGraphics.zoomLevel + array[array.Length - 1];
					sbyte[] array2 = Rms.loadRMS(filename);
					if (array2 != null)
					{
						result = Image.createImage(array2, 0, array2.Length);
						array2 = null;
					}
				}
				catch (Exception)
				{
					Cout.LogError("Loi ham khong tim thay a: " + ex.ToString());
				}
			}
			return result;
		}
	public static Image loadImage(string path)
		{
			path = Main.res + "/x" + mGraphics.zoomLevel + path;
			path = cutPng(path);
			Image result = null;
			try
			{
				result = Image.createImage(path);
			}
			catch (Exception)
			{
			}
			return result;
		}
	public static Image loadCustomImage(string filename)
		{
			try
			{
				if (cachedCustomLogo != null)
				{
					return cachedCustomLogo;
				}
				string text = filename;
				if (!text.Contains("/") && !text.Contains("\\"))
				{
					text = "custom_logo.png";
				}
				if (System.IO.File.Exists(text))
				{
					byte[] array = System.IO.File.ReadAllBytes(text);
					cachedCustomLogo = Image.createImage(array);
					return cachedCustomLogo;
				}
			}
			catch (Exception)
			{
			}
			return null;
		}
	public static string cutPng(string str)
		{
			string result = str;
			if (str.Contains(".png"))
			{
				result = str.Replace(".png", string.Empty);
			}
			return result;
		}
	public static int random(int a, int b)
		{
			return a + r.nextInt(b - a);
		}
	public void loadWaterSplash()
		{
			if (!lowGraphic)
			{
				imgWS = new Image[3];
				for (int i = 0; i < 3; i++)
				{
					imgWS[i] = loadImage("/e/w" + i + ".png");
				}
				wsX = new int[2];
				wsY = new int[2];
				wsState = new int[2];
				wsF = new int[2];
				wsState[0] = (wsState[1] = -1);
			}
		}
	public bool startWaterSplash(int x, int y)
		{
			if (lowGraphic)
			{
				return false;
			}
			int num = ((wsState[0] != -1) ? 1 : 0);
			if (wsState[num] != -1)
			{
				return false;
			}
			wsState[num] = 0;
			wsX[num] = x;
			wsY[num] = y;
			return true;
		}
	public static bool isPaint(int x, int y)
		{
			if (x < GameScr.cmx)
			{
				return false;
			}
			if (x > GameScr.cmx + GameScr.gW)
			{
				return false;
			}
			if (y < GameScr.cmy)
			{
				return false;
			}
			if (y > GameScr.cmy + GameScr.gH + 30)
			{
				return false;
			}
			return true;
		}
	public void resetToLoginScrz()
		{
			resetToLoginScr = true;
		}

}
