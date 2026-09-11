using System;
using Assets.src.e;

public class SmallImage
{
	public static int[][] smallImg;

	public static SmallImage instance;

	public static Image[] imgbig;

	public static Small[] imgNew;

	public static MyVector vKeys = new MyVector();

	public static Image imgEmpty = null;

	public static sbyte[] newSmallVersion;

	public static MyVector vt_images_watingDowload = new MyVector();

	public static int smallCount;

	public static short maxSmall;

	public SmallImage()
	{
		readImage();
	}

	public static void loadBigRMS()
	{
		if (imgbig == null || imgbig[0] == null)
		{
			imgbig = new Image[5];
			for (int i = 0; i < 5; i++)
			{
				try
				{
					imgbig[i] = GameCanvas.loadImageRMS("/img/Big" + i + ".png");
					if (imgbig[i] == null)
					{
						imgbig[i] = GameCanvas.loadImage("/img/Big" + i + ".png");
					}
					if (imgbig[i] == null)
					{
						imgbig[i] = GameCanvas.loadImage("/x1/img/Big" + i + ".png");
					}
					if (imgbig[i] == null)
					{
						imgbig[i] = Image.createImage(Main.res + "/x1/img/Big" + i + ".png");
					}
				}
				catch (Exception ex)
				{
					Cout.LogError("Loi loadBigRMS " + i + ": " + ex.Message);
				}
			}
		}
	}

	public static void freeBig()
	{
		imgbig = null;
		mSystem.gcc();
	}

	public static void loadBigImage()
	{
		imgEmpty = Image.createRGBImage(new int[1], 1, 1, bl: true);
	}

	public static void init()
	{
		instance = null;
		instance = new SmallImage();
	}

	public void readData(byte[] data)
	{
	}

	public void readImage()
	{
		int num = 0;
		try
		{
			DataInputStream dataInputStream = new DataInputStream(Rms.loadRMS("NR_image"));
			short num2 = dataInputStream.readShort();
			smallImg = new int[num2][];
			for (int i = 0; i < smallImg.Length; i++)
			{
				smallImg[i] = new int[5];
			}
			for (int j = 0; j < num2; j++)
			{
				num++;
				smallImg[j][0] = dataInputStream.readUnsignedByte();
				smallImg[j][1] = dataInputStream.readShort();
				smallImg[j][2] = dataInputStream.readShort();
				smallImg[j][3] = dataInputStream.readShort();
				smallImg[j][4] = dataInputStream.readShort();
			}
		}
		catch (Exception ex)
		{
			Cout.LogError3("Loi readImage: " + ex.ToString() + "i= " + num);
		}
	}

	public static void clearHastable()
	{
	}

	public static void ensureImgNew(int id)
	{
		if (imgNew == null)
		{
			imgNew = new Small[System.Math.Max(10000, id + 500)];
		}
		else if (id >= imgNew.Length)
		{
			Array.Resize(ref imgNew, System.Math.Max(imgNew.Length * 2, id + 500));
		}
	}

	public static Small getSmall(int id)
	{
		if (id < 0) return null;
		ensureImgNew(id);
		return imgNew[id];
	}

	public static void setSmall(int id, Small s)
	{
		if (id < 0) return;
		ensureImgNew(id);
		imgNew[id] = s;
	}

	public static void createImage(int id)
	{
		if (id < 0) return;
		Res.outz("is request =" + id + " zoom=" + mGraphics.zoomLevel);
		ensureImgNew(id);
		if (mGraphics.zoomLevel == 1)
		{
			Image image = GameCanvas.loadImage("/SmallImage/Small" + id + ".png");
			if (image != null)
			{
				setSmall(id, new Small(image, id));
				return;
			}
			setSmall(id, new Small(imgEmpty, id));
			if (GameCanvas.currentScreen == GameCanvas._SelectCharScr)
			{
				Service.gI().requestIcon(id);
			}
			else
			{
				vt_images_watingDowload.addElement(getSmall(id));
			}
			return;
		}
		Image image2 = GameCanvas.loadImage("/SmallImage/Small" + id + ".png");
		if (image2 != null)
		{
			setSmall(id, new Small(image2, id));
			return;
		}
		bool flag = false;
		sbyte[] array = Rms.loadRMS(mGraphics.zoomLevel + "Small" + id);
		if (array != null)
		{
			if (newSmallVersion != null && id < newSmallVersion.Length && array.Length % 127 != newSmallVersion[id])
			{
				flag = true;
			}
			if (!flag)
			{
				Image image3 = Image.createImage(array, 0, array.Length);
				if (image3 != null)
				{
					setSmall(id, new Small(image3, id));
				}
				else
				{
					flag = true;
				}
			}
		}
		else
		{
			flag = true;
		}
		if (flag)
		{
			setSmall(id, new Small(imgEmpty, id));
			if (GameCanvas.currentScreen == GameCanvas._SelectCharScr)
			{
				Service.gI().requestIcon(id);
			}
			else
			{
				vt_images_watingDowload.addElement(getSmall(id));
			}
		}
	}

	public static void drawSmallImage(mGraphics g, int id, int x, int y, int transform, int anchor)
	{
		if (id < 0) return;
		if (smallImg == null)
		{
			if (instance == null) instance = new SmallImage();
			else instance.readImage();
		}
		if (imgbig == null || imgbig[0] == null)
		{
			loadBigRMS();
		}

		if (imgbig == null || imgbig[0] == null)
		{
			Small small = getSmall(id);
			if (small == null)
			{
				createImage(id);
			}
			else
			{
				g.drawRegion(small, 0, 0, mGraphics.getImageWidth(small.img), mGraphics.getImageHeight(small.img), transform, x, y, anchor);
			}
		}
		else if (smallImg != null)
		{
			if (id >= smallImg.Length || smallImg[id] == null || smallImg[id][1] >= 256 || smallImg[id][3] >= 256 || smallImg[id][2] >= 256 || smallImg[id][4] >= 256)
			{
				Small small2 = getSmall(id);
				if (small2 == null)
				{
					createImage(id);
				}
				else
				{
					small2.paint(g, transform, x, y, anchor);
				}
			}
			else if (smallImg[id][0] < imgbig.Length && imgbig[smallImg[id][0]] != null)
			{
				g.drawRegion(imgbig[smallImg[id][0]], smallImg[id][1], smallImg[id][2], smallImg[id][3], smallImg[id][4], transform, x, y, anchor);
			}
		}
		else if (GameCanvas.currentScreen != GameScr.gI())
		{
			Small small3 = getSmall(id);
			if (small3 == null)
			{
				createImage(id);
			}
			else
			{
				small3.paint(g, transform, x, y, anchor);
			}
		}
	}

	public static void drawSmallImage(mGraphics g, int id, int f, int x, int y, int w, int h, int transform, int anchor)
	{
		if (id < 0) return;
		if (smallImg == null)
		{
			if (instance == null) instance = new SmallImage();
			else instance.readImage();
		}
		if (imgbig == null || imgbig[0] == null)
		{
			loadBigRMS();
		}

		if (imgbig == null || imgbig[0] == null)
		{
			Small small = getSmall(id);
			if (small == null)
			{
				createImage(id);
			}
			else
			{
				g.drawRegion(small.img, 0, f * w, w, h, transform, x, y, anchor);
			}
		}
		else if (smallImg != null)
		{
			if (id >= smallImg.Length || smallImg[id] == null || smallImg[id][1] >= 256 || smallImg[id][3] >= 256 || smallImg[id][2] >= 256 || smallImg[id][4] >= 256)
			{
				Small small2 = getSmall(id);
				if (small2 == null)
				{
					createImage(id);
				}
				else
				{
					small2.paint(g, transform, f, x, y, w, h, anchor);
				}
			}
			else if (smallImg[id][0] != 4 && smallImg[id][0] < imgbig.Length && imgbig[smallImg[id][0]] != null)
			{
				g.drawRegion(imgbig[smallImg[id][0]], 0, f * w, w, h, transform, x, y, anchor);
			}
			else
			{
				Small small3 = getSmall(id);
				if (small3 == null)
				{
					createImage(id);
				}
				else
				{
					small3.paint(g, transform, f, x, y, w, h, anchor);
				}
			}
		}
		else if (GameCanvas.currentScreen != GameScr.gI())
		{
			Small small4 = getSmall(id);
			if (small4 == null)
			{
				createImage(id);
			}
			else
			{
				small4.paint(g, transform, f, x, y, w, h, anchor);
			}
		}
	}

	public static void update()
	{
		int num = 0;
		if (GameCanvas.gameTick % 1000 != 0 || imgNew == null)
		{
			return;
		}
		for (int i = 0; i < imgNew.Length; i++)
		{
			if (imgNew[i] != null)
			{
				num++;
				imgNew[i].update();
				smallCount++;
			}
		}
		if (num > 200 && GameCanvas.lowGraphic)
		{
			imgNew = new Small[maxSmall];
		}
	}
}
