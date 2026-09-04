using System;
using System.Collections;
using Assets.src.e;
using UnityEngine;

public partial class mGraphics
{
	public static int HCENTER = 1;

	public static int VCENTER = 2;

	public static int LEFT = 4;

	public static int RIGHT = 8;

	public static int TOP = 16;

	public static int BOTTOM = 32;

	private float r;

	private float g;

	private float b;

	private float a;

	public int clipX;

	public int clipY;

	public int clipW;

	public int clipH;

	private bool isClip;

	private bool isTranslate = true;

	private int translateX;

	private int translateY;

	private float translateXf;

	private float translateYf;

	public static int zoomLevel = 1;

	public const int BASELINE = 64;

	public const int SOLID = 0;

	public const int DOTTED = 1;

	public const int TRANS_MIRROR = 2;

	public const int TRANS_MIRROR_ROT180 = 1;

	public const int TRANS_MIRROR_ROT270 = 4;

	public const int TRANS_MIRROR_ROT90 = 7;

	public const int TRANS_NONE = 0;

	public const int TRANS_ROT180 = 3;

	public const int TRANS_ROT270 = 6;

	public const int TRANS_ROT90 = 5;

	public static Hashtable cachedTextures = new Hashtable();

	public static int addYWhenOpenKeyBoard;

	private int clipTX;

	private int clipTY;

	private int currentBGColor;

	private Vector2 pos = new Vector2(0f, 0f);

	private Rect rect;

	private Matrix4x4 matrixBackup;

	private Vector2 pivot;

	public Vector2 size = new Vector2(128f, 128f);

	public Vector2 relativePosition = new Vector2(0f, 0f);

	public Color clTrans;

	public static Color transParentColor = new Color(1f, 1f, 1f, 0f);

	private Material lineMaterial;

	private void cache(string key, Texture value)
		{
			if (cachedTextures.Count > 400)
			{
				cachedTextures.Clear();
			}
			if (value.width * value.height < GameCanvas.w * GameCanvas.h)
			{
				cachedTextures.Add(key, value);
			}
		}

	public void translate(int tx, int ty)
		{
			tx *= zoomLevel;
			ty *= zoomLevel;
			translateX += tx;
			translateY += ty;
			isTranslate = true;
			if (translateX == 0 && translateY == 0)
			{
				isTranslate = false;
			}
		}

	public void translate(float x, float y)
		{
			translateXf += x;
			translateYf += y;
			isTranslate = true;
			if (translateXf == 0f && translateYf == 0f)
			{
				isTranslate = false;
			}
		}

	public int getTranslateX()
		{
			return translateX / zoomLevel;
		}

	public int getTranslateY()
		{
			return translateY / zoomLevel + addYWhenOpenKeyBoard;
		}

	public void setClip(int x, int y, int w, int h)
		{
			x *= zoomLevel;
			y *= zoomLevel;
			w *= zoomLevel;
			h *= zoomLevel;
			clipTX = translateX;
			clipTY = translateY;
			clipX = x;
			clipY = y;
			clipW = w;
			clipH = h;
			isClip = true;
		}

	public int getClipX()
		{
			return GameScr.cmx;
		}

	public int getClipY()
		{
			return GameScr.cmy;
		}

	public int getClipWidth()
		{
			return GameScr.gW;
		}

	public int getClipHeight()
		{
			return GameScr.gH;
		}

	private void UpdatePos(int anchor)
		{
			Vector2 vector = new Vector2(0f, 0f);
			switch (anchor)
			{
			case 3:
				vector = new Vector2(size.x / 2f, size.y / 2f);
				break;
			case 20:
				vector = new Vector2(0f, 0f);
				break;
			case 17:
				vector = new Vector2(Screen.width / 2, 0f);
				break;
			case 24:
				vector = new Vector2(Screen.width, 0f);
				break;
			case 6:
				vector = new Vector2(0f, Screen.height / 2);
				break;
			case 10:
				vector = new Vector2(Screen.width, Screen.height / 2);
				break;
			case 36:
				vector = new Vector2(0f, Screen.height);
				break;
			case 33:
				vector = new Vector2(Screen.width / 2, Screen.height);
				break;
			case 40:
				vector = new Vector2(Screen.width, Screen.height);
				break;
			}
			pos = vector + relativePosition;
			rect = new Rect(pos.x - size.x * 0.5f, pos.y - size.y * 0.5f, size.x, size.y);
			pivot = new Vector2(rect.xMin + rect.width * 0.5f, rect.yMin + rect.height * 0.5f);
		}

	public void reset()
		{
			isClip = false;
			isTranslate = false;
			translateX = 0;
			translateY = 0;
		}

}
