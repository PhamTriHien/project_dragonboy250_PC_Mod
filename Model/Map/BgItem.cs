/// <summary>
/// CLASS BgItem: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public class BgItem
{
	public int id;

	public int trans;

	public short idImage;

	public int x;

	public int y;

	public int dx;

	public int dy;

	public sbyte layer;

	public int nTilenotMove;

	public int[] tileX;

	public int[] tileY;

	public static MyHashTable imgNew = new MyHashTable();

	public static MyVector vKeysNew = new MyVector();

	public static MyVector vKeysLast = new MyVector();

	private bool isBlur;

	public int transX;

	public int transY;

	public static int[] idNotBlend = new int[61]
	{
		79, 80, 81, 82, 83, 84, 85, 86, 87, 88,
		89, 90, 91, 92, 95, 144, 99, 100, 101, 102,
		103, 104, 105, 106, 107, 108, 109, 110, 111, 112,
		113, 114, 115, 117, 118, 119, 120, 121, 122, 123,
		124, 125, 126, 127, 132, 133, 134, 139, 140, 141,
		142, 143, 144, 145, 146, 147, 171, 121, 122, 229,
		218
	};

	public static int[] isMiniBgz = new int[18]
	{
		79, 80, 81, 85, 86, 90, 91, 92, 99, 100,
		101, 102, 103, 104, 105, 106, 107, 108
	};

	public static sbyte[] newSmallVersion;

	/// <summary>
	/// Dọn dẹp và đặt lại trạng thái ban đầu cho clearHashTable.
	/// </summary>
	public static void clearHashTable()
	{
		imgNew.clear();
		vKeysNew.removeAllElements();
	}

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của isExistKeyNews.
	/// </summary>
	public static bool isExistKeyNews(string keyNew)
	{
		for (int i = 0; i < vKeysNew.size(); i++)
		{
			string text = (string)vKeysNew.elementAt(i);
			if (text.Equals(keyNew))
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của isExistKeyLast.
	/// </summary>
	public static bool isExistKeyLast(string keyLast)
	{
		for (int i = 0; i < vKeysLast.size(); i++)
		{
			string text = (string)vKeysLast.elementAt(i);
			if (text.Equals(keyLast))
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của isNotBlend.
	/// </summary>
	public bool isNotBlend()
	{
		if (mGraphics.zoomLevel == 1 || ModGraphics.IsJavaGraphics)
		{
			return true;
		}
		if (TileMap.isInAirMap())
		{
			return true;
		}
		for (int i = 0; i < idNotBlend.Length; i++)
		{
			if (idImage == idNotBlend[i])
			{
				return true;
			}
		}
		return false;
	}

	public static Image LoadMapBgItemImage(short idImage)
	{
		Image image = null;
		if (mGraphics.zoomLevel == 1 || ModGraphics.IsJavaGraphics)
		{
			image = GameCanvas.loadImage("/mapBackGround/" + idImage + ".png");
			if (image != null)
			{
				return image;
			}
		}
		bool needRequest = false;
		sbyte[] array = Rms.loadRMS(mGraphics.zoomLevel + "bgItem" + idImage);
		if (array != null)
		{
			if (newSmallVersion != null && idImage >= 0 && idImage < newSmallVersion.Length && array.Length % 127 != newSmallVersion[idImage])
			{
				needRequest = true;
			}
			if (!needRequest)
			{
				image = (mGraphics.zoomLevel > 1) ? Image.createServerZoomImage(array, 0, array.Length) : Image.createImage(array, 0, array.Length);
				if (image != null)
				{
					return image;
				}
				needRequest = true;
			}
		}
		else
		{
			needRequest = true;
		}
		image = GameCanvas.loadImage("/mapBackGround/" + idImage + ".png");
		if (image == null)
		{
			image = Image.createRGBImage(new int[1], 1, 1, bl: true);
			if (needRequest)
			{
				Service.gI().getBgTemplate(idImage);
			}
		}
		return image;
	}

	public static void ReloadCurrMapItems()
	{
		clearHashTable();
		vKeysLast.removeAllElements();
		if (TileMap.vCurrItem == null)
		{
			return;
		}
		for (int i = 0; i < TileMap.vCurrItem.size(); i++)
		{
			if (TileMap.vCurrItem.elementAt(i) is BgItem item)
			{
				string key = item.idImage + string.Empty;
				if (!imgNew.containsKey(key))
				{
					Image img = LoadMapBgItemImage(item.idImage);
					if (img != null)
					{
						imgNew.put(key, img);
					}
					vKeysLast.addElement(key);
				}
				if (!isExistKeyNews(key))
				{
					vKeysNew.addElement(key);
				}
				item.changeColor();
			}
		}
	}

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của isMiniBg.
	/// </summary>
	public bool isMiniBg()
	{
		for (int i = 0; i < isMiniBgz.Length; i++)
		{
			if (idImage == isMiniBgz[i])
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ changeColor.
	/// </summary>
	public void changeColor()
	{
		string blendKey = idImage + "blend" + layer + "_t" + TileMap.tileID;
		if (isNotBlend() || layer == 2 || layer == 4 || imgNew.containsKey(blendKey))
		{
			return;
		}
		Image image = (Image)imgNew.get(idImage + string.Empty);
		if (image != null && image.getRealImageWidth() > 4)
		{
			sbyte[] array = Rms.loadRMS("x" + mGraphics.zoomLevel + "blend" + idImage + "layer" + layer + "_t" + TileMap.tileID);
			if (array == null || array.Length == 0)
			{
				imgNew.put(blendKey, BgItemMn.blendImage(image, layer, idImage));
				return;
			}
			Image v = Image.createImage(array, 0, array.Length);
			if (mGraphics.zoomLevel > 1 && v != null && !v.isZoomScaled)
			{
				v.isZoomScaled = true;
			}
			imgNew.put(blendKey, v);
		}
	}

	/// <summary>
	/// Thực hiện vẽ toàn bộ đối tượng và các lớp hiển thị lên canvas mGraphics.
	/// </summary>
	public void paint(mGraphics g)
	{
		if (Char.isLoadingMap || (idImage == 279 && GameScr.gI().tMabuEff >= 110))
		{
			return;
		}
		int cmx = GameScr.cmx;
		int cmy = GameScr.cmy;
		Image image = null;
		string blendKey = idImage + "blend" + layer + "_t" + TileMap.tileID;
		image = ((layer == 2 || layer == 4) ? ((Image)imgNew.get(idImage + string.Empty)) : (isNotBlend() ? ((Image)imgNew.get(idImage + string.Empty)) : ((Image)imgNew.get(blendKey))));
		if (image == null)
		{
			image = (Image)imgNew.get(idImage + string.Empty);
		}
		if (image == null || idImage == 96)
		{
			return;
		}
		if (layer == 4)
		{
			transX = -cmx / 2 + 100;
		}
		if (idImage == 28 && layer == 3)
		{
			transX = -cmx / 3 + 200;
		}
		if ((idImage == 67 || idImage == 68 || idImage == 69 || idImage == 70) && layer == 3)
		{
			transX = -cmx / 3 + 200;
		}
		if (isMiniBg() && layer < 4)
		{
			transX = -(cmx >> 4) + 50;
			transY = (cmy >> 5) - 15;
		}
		int num = x + dx + transX;
		int num2 = y + dy + transY;
		int imgW = mGraphics.getImageWidth(image);
		int imgH = mGraphics.getImageHeight(image);
		if (imgW <= 0 || imgH <= 0)
		{
			return;
		}

		if (x + dx + imgW + transX >= cmx && x + dx + transX <= cmx + GameCanvas.w && y + dy + transY + imgH >= cmy && y + dy + transY <= cmy + GameCanvas.h)
		{
			g.drawRegion(image, 0, 0, imgW, imgH, trans, x + dx + transX, y + dy + transY, 0);
			if (idImage == 11 && TileMap.mapID != 122)
			{
				g.setClip(num, num2 + 24, 48, 14);
				for (int i = 0; i < 2; i++)
				{
					g.drawRegion(TileMap.imgWaterflow, 0, (GameCanvas.gameTick % 8 >> 2) * 24, 24, 24, 0, num + i * 24, num2 + 24, 0);
				}
				g.setClip(GameScr.cmx, GameScr.cmy, GameScr.gW, GameScr.gH);
			}
		}
		if (TileMap.isDoubleMap() && idImage > 137 && idImage != 156 && idImage != 159 && idImage != 157 && idImage != 165 && idImage != 167 && idImage != 168 && idImage != 169 && idImage != 170 && idImage != 238 && TileMap.pxw - (x + dx + transX) >= cmx && TileMap.pxw - (x + dx + transX + imgW) <= cmx + GameCanvas.w && y + dy + transY + imgH >= cmy && y + dy + transY <= cmy + GameCanvas.h && (idImage < 241 || idImage >= 266))
		{
			g.drawRegion(image, 0, 0, imgW, imgH, 2, TileMap.pxw - (x + dx + transX), y + dy + transY, StaticObj.TOP_RIGHT);
		}
	}
}
