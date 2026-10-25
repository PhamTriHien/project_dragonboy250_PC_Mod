using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// CLASS ImgByName: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public class ImgByName
{
	public static MyHashTable hashImagePath = new MyHashTable();

	/// <summary>
	/// Thiết lập giá trị hoặc trạng thái cho SetImage.
	/// </summary>
	public static void SetImage(string name, Image img, sbyte nFrame)
	{
		hashImagePath.put(string.Empty + name, new MainImage(img, nFrame));
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của getImagePath.
	/// </summary>
	public static MainImage getImagePath(string nameImg, MyHashTable hash)
	{
		MainImage mainImage = (MainImage)hash.get(string.Empty + nameImg);
		if (mainImage == null)
		{
			mainImage = new MainImage();
			MainImage fromRms = getFromRms(nameImg);
			if (fromRms != null)
			{
				mainImage.img = fromRms.img;
				mainImage.nFrame = fromRms.nFrame;
			}
			hash.put(string.Empty + nameImg, mainImage);
		}
		mainImage.count = GameCanvas.timeNow / 1000;
		if (mainImage.img == null)
		{
			mainImage.timeImageNull--;
			if (mainImage.timeImageNull <= 0)
			{
				Service.gI().getImgByName(nameImg);
				mainImage.timeImageNull = 200;
			}
		}
		return mainImage;
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của getFromRms.
	/// </summary>
	public static MainImage getFromRms(string nameImg)
	{
		string text = mGraphics.zoomLevel + "ImgByName_" + nameImg;
		MainImage result = null;
		sbyte[] array = null;
		array = Rms.loadRMS(text);
		if (array == null)
		{
			return result;
		}
		try
		{
			result = new MainImage();
			result.nFrame = array[0];
			result.img = (mGraphics.zoomLevel > 1) ? Image.createServerZoomImage(array, 1, array.Length - 1) : Image.createImage(array, 1, array.Length - 1);
			if (mGraphics.zoomLevel > 1 && result.img != null && !result.img.isZoomScaled)
			{
				result.img.isZoomScaled = true;
			}
		}
		catch (Exception)
		{
			Debug.LogError(text + ">>>>>getFromRms: nulllllllllll 2222");
			return null;
		}
		return result;
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ saveRMS.
	/// </summary>
	public static void saveRMS(string nameImg, sbyte nFrame, sbyte[] data)
	{
		string text = mGraphics.zoomLevel + "ImgByName_" + nameImg;
		DataOutputStream dataOutputStream = new DataOutputStream(data.Length + 1);
		int i = 0;
		try
		{
			dataOutputStream.writeByte(nFrame);
			for (i = 0; i < data.Length; i++)
			{
				dataOutputStream.writeByte(data[i]);
			}
			Rms.saveRMS(text, dataOutputStream.toByteArray());
			dataOutputStream.close();
		}
		catch (Exception ex)
		{
			Debug.LogError(i + ">>Errr save rms: " + text + "  " + ex.ToString());
		}
	}

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của checkDelHash.
	/// </summary>
	public static void ReloadForGraphicsMode()
	{
		if (hashImagePath == null)
		{
			return;
		}
		IDictionaryEnumerator enumerator = hashImagePath.GetEnumerator();
		while (enumerator.MoveNext())
		{
			if (enumerator.Value is MainImage mi && mi.img != null && mi.img.rawServerPngBytes != null)
			{
				mi.img = Image.refreshServerZoomImage(mi.img);
			}
		}
	}

	public static void checkDelHash(MyHashTable hash, int minute, bool isTrue)
	{
		MyVector myVector = new MyVector("checkDelHash");
		if (isTrue)
		{
			hash.clear();
			return;
		}
		IDictionaryEnumerator enumerator = hash.GetEnumerator();
		while (enumerator.MoveNext())
		{
			MainImage mainImage = (MainImage)enumerator.Value;
			if (GameCanvas.timeNow / 1000 - mainImage.count > minute * 60)
			{
				string o = (string)enumerator.Key;
				myVector.addElement(o);
			}
		}
		for (int i = 0; i < myVector.size(); i++)
		{
			hash.remove((string)myVector.elementAt(i));
		}
	}
}
