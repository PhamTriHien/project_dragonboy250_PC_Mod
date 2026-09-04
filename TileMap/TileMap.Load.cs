using System;

public partial class TileMap
{
	public static Image imgLight = GameCanvas.loadImage("/bg/light.png");

	public static void loadBg()
		{
			bong = GameCanvas.loadImage("/mainImage/myTexture2dbong.png");
			if (mGraphics.zoomLevel != 1 && !Main.isIpod && !Main.isIphone4)
			{
				imgLight = GameCanvas.loadImage("/bg/light.png");
			}
		}

	public static void loadTileCreatChar()
		{
		}

	public static void loadTileImage()
		{
			if (imgWaterfall == null)
			{
				imgWaterfall = GameCanvas.loadImageRMS("/tWater/wtf.png");
			}
			if (imgTopWaterfall == null)
			{
				imgTopWaterfall = GameCanvas.loadImageRMS("/tWater/twtf.png");
			}
			if (imgWaterflow == null)
			{
				imgWaterflow = GameCanvas.loadImageRMS("/tWater/wts.png");
			}
			if (imgWaterlowN == null)
			{
				imgWaterlowN = GameCanvas.loadImageRMS("/tWater/wtsN.png");
			}
			if (imgWaterlowN2 == null)
			{
				imgWaterlowN2 = GameCanvas.loadImageRMS("/tWater/wtsN2.png");
			}
			mSystem.gcc();
		}

	public static void loadMap(int tileId)
		{
			pxh = tmh * size;
			pxw = tmw * size;
			Res.outz("load tile ID= " + tileID);
			int num = tileId - 1;
			try
			{
				for (int i = 0; i < tmw * tmh; i++)
				{
					for (int j = 0; j < tileType[num].Length; j++)
					{
						setTile(i, tileIndex[num][j], tileType[num][j]);
					}
				}
			}
			catch (Exception)
			{
				Cout.println("Error Load Map");
				GameMidlet.instance.exit();
			}
		}

	public static void loadMapFromResource(int mapID)
		{
			DataInputStream dataInputStream = null;
			dataInputStream = MyStream.readFile("/mymap/" + mapID);
			tmw = (ushort)dataInputStream.read();
			tmh = (ushort)dataInputStream.read();
			maps = new int[dataInputStream.available()];
			for (int i = 0; i < tmw * tmh; i++)
			{
				maps[i] = (ushort)dataInputStream.read();
			}
			types = new int[maps.Length];
		}

	public static void loadMainTile()
		{
			if (lastTileID != tileID)
			{
				getTile();
				lastTileID = tileID;
			}
		}

}
