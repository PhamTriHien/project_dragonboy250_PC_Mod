using System;
using UnityEngine;

/// <summary>
/// CLASS Effect: Định nghĩa và điều phối logic trong hệ thống game.
/// </summary>
public class Effect
{
	public int effId;

	public int typeEff;

	public int indexFrom;

	public int indexTo;

	public bool isNearPlayer;

	public const int NEAR_PLAYER = 0;

	public const int LOOP_NORMAL = 1;

	public const int LOOP_TRANS = 2;

	public const int BACKGROUND = 3;

	public const int CHAR = 4;

	public const int CHAR_PET_EFF = 5;

	public const int FIRE_TD = 0;

	public const int BIRD = 1;

	public const int FIRE_NAMEK = 2;

	public const int FIRE_SAYAI = 3;

	public const int FROG = 5;

	public const int CA = 4;

	public const int ECH = 6;

	public const int TACKE = 7;

	public const int RAN = 8;

	public const int KHI = 9;

	public const int GACON = 10;

	public const int DANONG = 11;

	public const int DANBUOM = 12;

	public const int QUA = 13;

	public const int THIENTHACH = 14;

	public const int CAVOI = 15;

	public const int NAM = 16;

	public const int RONGTHAN = 17;

	public const int BUOMBAY = 26;

	public const int KHUCGO = 27;

	public const int DOIBAY = 28;

	public const int CONMEO = 29;

	public const int LUATAT = 30;

	public const int ONGCONG = 31;

	public const int KHANGIA1 = 42;

	public const int KHANGIA2 = 43;

	public const int KHANGIA3 = 44;

	public const int KHANGIA4 = 45;

	public const int KHANGIA5 = 46;

	public Char c;

	public int t;

	public int currFrame;

	public int x;

	public int y;

	public int loop;

	public int tLoop;

	public int tLoopCount;

	private bool isPaint = true;

	public int layer;

	public int isStand;

	public static MyVector vEffData = new MyVector();

	public int trans;

	public long timeExist;

	public static MyVector lastEff = new MyVector();

	public static MyVector newEff = new MyVector();

	public static MyVector dowloadEff = new MyVector();

	private int[] khangia1 = new int[10] { 0, 0, 0, 0, 0, 1, 1, 1, 1, 1 };

	private int[] khangia2 = new int[10] { 2, 2, 2, 2, 2, 3, 3, 3, 3, 3 };

	private int[] khangia3 = new int[10] { 4, 4, 4, 4, 4, 5, 5, 5, 5, 5 };

	private int[] khangia4 = new int[10] { 6, 6, 6, 6, 6, 7, 7, 7, 7, 7 };

	private int[] khangia5 = new int[10] { 8, 8, 8, 8, 8, 9, 9, 9, 9, 9 };

	private bool isGetTime;

	private short[] data;

	public int cLastStatusMe;

	public long cur_time_cLastStatusMe;

	/// <summary>
	/// Thực thi phương thức nghiệp vụ Effect.
	/// </summary>
	public Effect()
	{
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ Effect.
	/// </summary>
	public Effect(int id, Char c, int layer, int loop, int loopCount, sbyte isStand)
	{
		this.c = c;
		effId = id;
		this.layer = layer;
		this.loop = loop;
		tLoop = loopCount;
		this.isStand = isStand;
		trans = -1;
		initCommon(id);
		if (id == 78)
		{
			typeEff = 5;
		}
		else
		{
			typeEff = 4;
		}
		if (id >= 201 && id < 2000 && layer == 2)
		{
			typeEff = 5;
		}
		Debug.Log(id + ">>>>>>Add Effect: char:" + c.cName + "  layer:" + layer + " loop:" + loop + " loopCount:" + loopCount + " isStand:" + isStand);
		if (!isExistNewEff(effId + string.Empty))
		{
			newEff.addElement(effId + string.Empty);
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ Effect.
	/// </summary>
	public Effect(int id, int x, int y, int layer, int loop, int loopCount)
	{
		this.x = x;
		this.y = y;
		effId = id;
		this.layer = layer;
		this.loop = loop;
		tLoop = loopCount;
		initCommon(id);
		if (id == 78)
		{
			typeEff = 5;
		}
		else
		{
			typeEff = 1;
		}
		Debug.Log(id + ">>>>>>Add Effect: layer:" + layer + " loop:" + loop + " loopCount:" + loopCount);
		if (!isExistNewEff(effId + string.Empty))
		{
			newEff.addElement(effId + string.Empty);
		}
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của GetCharEff.
	/// </summary>
	public static void GetCharEff(Char c)
	{
		for (int i = 0; i < 1799; i++)
		{
			c.removeEffChar(0, 201 + i);
		}
		if (c.bag >= 201 && c.bag < 2000)
		{
			Effect e = new Effect(c.bag, c, 2, -1, 10, 1);
			c.addEffChar(e);
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ removeEffData.
	/// </summary>
	public static void removeEffData(int id)
	{
		for (int i = 0; i < vEffData.size(); i++)
		{
			EffectData effectData = (EffectData)vEffData.elementAt(i);
			if (effectData.ID == id)
			{
				vEffData.removeElement(effectData);
				break;
			}
		}
	}

	/// <summary>
	/// Thực thi phương thức nghiệp vụ addEffData.
	/// </summary>
	public static void addEffData(EffectData eff)
	{
		vEffData.addElement(eff);
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của getEffDataById.
	/// </summary>
	public static void ReloadForGraphicsMode()
	{
		if (vEffData == null)
		{
			return;
		}
		for (int i = 0; i < vEffData.size(); i++)
		{
			if (vEffData.elementAt(i) is EffectData ed)
			{
				if (ed.img != null && ed.img.rawServerPngBytes != null)
				{
					ed.img = Image.refreshServerZoomImage(ed.img);
				}
				else
				{
					int resolvedId = (ed.ID >= 42 && ed.ID <= 46) ? 106 : ed.ID;
					Image effImg = GameCanvas.loadImage("/effectdata/" + resolvedId + "/img.png");
					if (effImg != null)
					{
						ed.img = effImg;
					}
				}
			}
		}
	}

	public static EffectData getEffDataById(int id)
	{
		for (int i = 0; i < vEffData.size(); i++)
		{
			EffectData effectData = (EffectData)vEffData.elementAt(i);
			if (effectData.ID == id)
			{
				return effectData;
			}
		}
		return null;
	}

	/// <summary>
	/// Khởi tạo và nạp dữ liệu cấu hình cho initCommon.
	/// </summary>
	private void initCommon(int id)
	{
		indexFrom = -1;
		indexTo = -1;
		if (getEffDataById(id) != null)
		{
			return;
		}
		EffectData effectData = new EffectData();
		effectData.ID = id;
		if (id >= 42 && id <= 46)
		{
			id = 106;
		}
		string text = "/x" + mGraphics.zoomLevel + "/effectdata/" + id + "/data";
		DataInputStream dataInputStream = MyStream.readFile(text);
		if (dataInputStream != null)
		{
			if (id > 100 && id < 200)
			{
				effectData.readData2(text);
			}
			else
			{
				effectData.readData(text);
			}
			effectData.img = GameCanvas.loadImage("/effectdata/" + id + "/img.png");
		}
		else
		{
			Service.gI().getEffData((short)id);
		}
		addEffData(effectData);
		if (lastEff.size() > 20)
		{
			removeEffData(int.Parse((string)lastEff.elementAt(0)));
			lastEff.removeElementAt(0);
		}
		lastEff.addElement(effId + string.Empty);
	}

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của isExistNewEff.
	/// </summary>
	public static bool isExistNewEff(string id)
	{
		for (int i = 0; i < newEff.size(); i++)
		{
			string text = (string)newEff.elementAt(i);
			if (text.Equals(id))
			{
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Kiểm tra điều kiện hợp lệ hoặc trạng thái của isPaintz.
	/// </summary>
	public bool isPaintz()
	{
		if (!isPaint)
		{
			return false;
		}
		return true;
	}

	/// <summary>
	/// Thực hiện vẽ và hiển thị thành phần paintUnderBackground lên canvas đồ họa.
	/// </summary>
	public void paintUnderBackground(mGraphics g, int xLayer, int yLayer)
	{
		if (isPaintz() && getEffDataById(effId).img != null)
		{
			getEffDataById(effId).paintFrame(g, currFrame, x + xLayer, y + yLayer, trans, layer);
		}
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của getFrameKhangia.
	/// </summary>
	public void getFrameKhangia()
	{
		if (effId == 42)
		{
			currFrame = khangia1[t];
		}
		if (effId == 43)
		{
			currFrame = khangia2[t];
		}
		if (effId == 44)
		{
			currFrame = khangia3[t];
		}
		if (effId == 45)
		{
			currFrame = khangia4[t];
		}
		if (effId == 46)
		{
			currFrame = khangia5[t];
		}
		t++;
		if (t > khangia1.Length - 1)
		{
			t = 0;
		}
	}

	/// <summary>
	/// Thực hiện vẽ toàn bộ đối tượng và các lớp hiển thị lên canvas mGraphics.
	/// </summary>
	public void paint(mGraphics g)
	{
		if (!isPaint || getEffDataById(effId) == null || getEffDataById(effId).img == null)
		{
			return;
		}
		try
		{
			getEffDataById(effId).paintFrame(g, currFrame, x, y, trans, layer);
		}
		catch (Exception)
		{
		}
	}

	/// <summary>
	/// Cập nhật trạng thái logic của đối tượng theo từng chu kỳ khung hình.
	/// </summary>
	public void update()
	{
		try
		{
			if (effId >= 42 && effId <= 46)
			{
				getFrameKhangia();
			}
			else
			{
				if (getEffDataById(effId) == null || getEffDataById(effId).img == null)
				{
					return;
				}
				if (c != null && effId >= 201 && effId < 2000)
				{
					typeEff = ((getEffDataById(effId).hasAnimData || layer == 2) ? 5 : 4);
				}
				if (typeEff == 5 && getEffDataById(effId).hasAnimData)
				{
					data = getEffDataById(effId).get(c.statusMe);
				}
				else
				{
					data = getEffDataById(effId).get();
				}
				if (data != null)
				{
					if (!isGetTime)
					{
						isGetTime = true;
						int num = data.Length - 1;
						if (num > 0 && typeEff != 1)
						{
							t = Res.random(0, num);
						}
						if (typeEff == 0)
						{
							t = Res.random(indexFrom, indexTo);
						}
					}
					switch (typeEff)
					{
					case 4:
						x = c.cx;
						y = c.cy;
						if (t < data.Length)
						{
							t++;
						}
						break;
					case 5:
						trans = ((c.cdir != 1) ? 1 : 0);
						if (c.cdir == 1)
						{
							x = c.cx - 15;
						}
						else
						{
							x = c.cx + 15;
						}
						if (c.isMonkey == 0)
						{
							y = c.cy - 25;
						}
						else
						{
							y = c.cy - 35;
						}
						if (t < data.Length)
						{
							t++;
						}
						break;
					case 1:
					case 3:
						if (t < data.Length)
						{
							t++;
						}
						break;
					case 0:
						if (Res.inRect(x - 50, y - 50, 100, 100, Char.myCharz().cx, Char.myCharz().cy) && t > indexFrom && t < indexTo)
						{
							if (t < indexTo)
							{
								t = indexTo;
							}
							isNearPlayer = true;
						}
						if (!isNearPlayer)
						{
							t++;
							if (t == indexTo)
							{
								t = indexFrom;
							}
						}
						else if (t < data.Length)
						{
							t++;
						}
						break;
					case 2:
						if (t < data.Length)
						{
							t++;
						}
						tLoopCount++;
						if (tLoopCount == tLoop)
						{
							tLoopCount = 0;
							trans = Res.random(0, 2);
						}
						break;
					}
					if (t == data.Length / 2 && (effId == 62 || effId == 63 || effId == 64 || effId == 65))
					{
						SoundMn.playSound(x, y, SoundMn.FIREWORK, SoundMn.volume);
					}
					if (t <= data.Length - 1)
					{
						currFrame = data[t];
					}
				}
				if (t >= data.Length - 1)
				{
					if (typeEff == 0 || typeEff == 3)
					{
						isPaint = false;
					}
					if (tLoop == -1)
					{
						EffecMn.vEff.removeElement(this);
					}
					if (typeEff == 2)
					{
						t = 0;
						return;
					}
					if (typeEff == 1 && loop == 1)
					{
						isPaint = false;
					}
					if (typeEff == 4 || typeEff == 5)
					{
						if (loop == -1)
						{
							t = 0;
							return;
						}
						tLoopCount++;
						if (tLoopCount == tLoop)
						{
							tLoopCount = 0;
							loop--;
							t = 0;
							if (loop == 0)
							{
								c.removeEffChar(0, effId);
							}
						}
						return;
					}
					isNearPlayer = false;
					if (loop == -1)
					{
						tLoopCount++;
						t = 0;
						if (tLoopCount == tLoop)
						{
							tLoopCount = 0;
							if (tLoop > 1)
							{
								trans = Res.random(0, 2);
							}
						}
						return;
					}
					tLoopCount++;
					t = 0;
					if (tLoopCount == tLoop)
					{
						tLoopCount = 0;
						loop--;
						if (loop == 0)
						{
							EffecMn.vEff.removeElement(this);
						}
					}
				}
				else
				{
					isPaint = true;
				}
			}
		}
		catch (Exception)
		{
			EffecMn.vEff.removeElement(this);
		}
	}

	/// <summary>
	/// Truy xuất và trả về dữ liệu của getnFrame.
	/// </summary>
	public int getnFrame()
	{
		return data.Length;
	}
}
