using System;

public partial class ServerScr : mScreen, IActionListener
{
	private int mainSelect;

	private MyVector vecServer = new MyVector();

	private Command cmdCheck;

	public const int icmd = 100;

	private int wc;

	private int hc;

	private int w2c;

	private int numw;

	private int numh;

	private Command cmdGlobal;

	private Command cmdVietNam;

	private const string RMS_SELECT_AREA = "area_select";

	public bool isChooseArea;

	public bool isPaintNewUi;

	private ListNew list;

	public sbyte select_Area;

	public sbyte select_Lang;

	public sbyte select_typeSv;

	private Command cmdChooseArea;

	private bool isPaint_select_area;

	private bool isPaint_select_lang;

	private int x;

	private int y;

	private int w;

	private int h;

	private int xName;

	private int yName;

	private int xsub;

	private int ysub;

	private int wsub;

	private int hsub;

	private int xsubpaint;

	private int ysubpaint;

	private int xPop;

	private int yPop;

	private int wPop;

	private int hPop;

	private int xinfo;

	private int yinfo;

	private int winfo;

	private int hinfo;

	private int yBox;

	private int wBox;

	private int hBox;

	private int ntypeSv;

	private int wCheck;

	private int xPopUp_Area;

	private int yPopUp_Area;

	private int xPopUp_Lang;

	private int yPopUp_Lang;

	private int htext = 15;

	private string[] strLang = new string[3] { "Tiếng Việt", "English", "Indo" };

	private string[] strArea = new string[2] { "VIỆT NAM", "GLOBAL" };

	private string[] strTypeSV = new string[2] { "Máy chủ tiêu chuẩn", "Máy chủ Super" };

	private string[] strTypeSV_info = new string[2] { "Máy chủ tiêu chuẩn:\nTiến trình game bình thường.", "Máy chủ Super:\n -Không thể giao dịch vàng.\n x3 Sức mạnh\n x3 Tiềm năng\n x3 Vàng\n x3 Vật phẩm khác" };

	private string strShowAll = "Chỉ hiện thị máy chủ đã chơi.";

	public int cmy;

	public static Image[] iconHead;

	public static bool isShowSv_HaveChar;

	public ServerScr()
		{
			TileMap.bgID = (byte)(mSystem.currentTimeMillis() % 9);
			if (TileMap.bgID == 5 || TileMap.bgID == 6)
			{
				TileMap.bgID = 4;
			}
			GameScr.loadCamera(fullmScreen: true, -1, -1);
			GameScr.cmx = 100;
			GameScr.cmy = 200;
		}

	public override void switchToMe()
		{
			Res.outz("switchToMe >>>>ServerScr: " + Rms.loadRMSInt(ServerListScreen.RMS_svselect));
			SoundMn.gI().stopAll();
			base.switchToMe();
			loadIconHead();
			mainSelect = ServerListScreen.ipSelect;
			numw = 1;
			numh = 1;
			Load_NewUI();
			if (!isPaintNewUi && !isChooseArea)
			{
				cmdGlobal = new Command(strArea[0], this, 98, null);
				cmdGlobal.x = 0;
				cmdGlobal.y = 0;
				cmdVietNam = new Command(strArea[1], this, 97, null);
				cmdVietNam.x = 50;
				cmdVietNam.y = 0;
				vecServer = new MyVector();
				vecServer.addElement(cmdGlobal);
				vecServer.addElement(cmdVietNam);
				sort();
			}
		}

	private void sort()
		{
			mainSelect = ServerListScreen.ipSelect;
			w2c = 5;
			wc = 76;
			hc = mScreen.cmdH;
			numw = 2;
			if (vecServer.size() > 2)
			{
				numw = GameCanvas.w / (wc + w2c);
			}
			numh = vecServer.size() / numw + ((vecServer.size() % numw != 0) ? 1 : 0);
			for (int i = 0; i < vecServer.size(); i++)
			{
				Command command = (Command)vecServer.elementAt(i);
				if (command != null)
				{
					int num = GameCanvas.hw - numw * (wc + w2c) / 2;
					int num2 = num + i % numw * (wc + w2c);
					int num3 = GameCanvas.hh - numh * (hc + w2c) / 2;
					int num4 = num3 + i / numw * (hc + w2c);
					command.x = num2;
					command.y = num4;
					command.w = wc;
				}
			}
		}

	private void sort_newUI()
		{
			mainSelect = ServerListScreen.ipSelect;
			w2c = 5;
			wc = 76;
			hc = mScreen.cmdH;
			numw = 1;
			int num = xsub + wsub / 2 + 3;
			ysubpaint = ysub + 5;
			numw = wsub / (wc + w2c);
			numh = vecServer.size() / numw + ((vecServer.size() % numw != 0) ? 1 : 0);
			xsubpaint = num - numw * (wc + w2c) / 2;
			for (int i = 0; i < vecServer.size(); i++)
			{
				Command command = (Command)vecServer.elementAt(i);
				if (command != null)
				{
					int num2 = xsubpaint + i % numw * (wc + w2c);
					int num3 = ysubpaint + i / numw * (hc + w2c);
					command.x = num2;
					command.y = num3;
					command.w = wc;
				}
			}
			list = new ListNew(xsub, ysub, wsub, hsub, 0, 0, 0, isLim0: true);
			list.setMaxCamera(numh * (hc + w2c) - hsub);
			list.resetList();
		}

	private void GetVecTypeSv(sbyte area, sbyte typeSv)
		{
			vecServer.removeAllElements();
			ntypeSv = 1;
			select_Area = area;
			mResources.loadLanguague(area);
			for (int i = 0; i < ServerListScreen.nameServer.Length; i++)
			{
				if (area == 1)
				{
					if (ServerListScreen.language[i] != 0 && ServerListScreen.typeSv[i] == 1)
					{
						ntypeSv = 2;
					}
				}
				else if (ServerListScreen.typeSv[i] == 1)
				{
					ntypeSv = 2;
				}
			}
			if (typeSv > (sbyte)(ntypeSv - 1))
			{
				typeSv = (sbyte)(ntypeSv - 1);
			}
			select_typeSv = typeSv;
			for (int j = 0; j < ServerListScreen.nameServer.Length; j++)
			{
				if (area == 1)
				{
					if (ServerListScreen.language[j] == 0)
					{
						continue;
					}
					if (ServerListScreen.typeSv[j] == 1)
					{
						ntypeSv = 2;
					}
					if (ServerListScreen.typeSv[j] != typeSv)
					{
						continue;
					}
					int num = -1;
					if (ServerListScreen.typeClass != null && j < ServerListScreen.typeClass.Length)
					{
						num = ServerListScreen.typeClass[j];
					}
					if (!isShowSv_HaveChar || num != -1)
					{
						Command command = new Command(ServerListScreen.nameServer[j], this, 100 + j, null);
						command.isPaintNew = ServerListScreen.isNew[j] == 1;
						if (num > -1)
						{
							command.imgBtn = iconHead[num];
						}
						vecServer.addElement(command);
					}
					continue;
				}
				if (ServerListScreen.typeSv[j] == 1)
				{
					ntypeSv = 2;
				}
				if (ServerListScreen.language[j] != 0 || ServerListScreen.typeSv[j] != typeSv)
				{
					continue;
				}
				int num2 = -1;
				if (ServerListScreen.typeClass != null && j < ServerListScreen.typeClass.Length)
				{
					num2 = ServerListScreen.typeClass[j];
				}
				if (!isShowSv_HaveChar || num2 != -1)
				{
					Command command2 = new Command(ServerListScreen.nameServer[j], this, 100 + j, null);
					command2.isPaintNew = ServerListScreen.isNew[j] == 1;
					if (num2 > -1)
					{
						command2.imgBtn = iconHead[num2];
					}
					vecServer.addElement(command2);
				}
			}
			Sort_NewSv();
			sort_newUI();
		}

	private void UpdTouch_NewUI()
		{
			if (!isPaintNewUi)
			{
				return;
			}
			int num = 0;
			if (list != null)
			{
				list.moveCamera();
				if (GameCanvas.isPointer(xsub, 0, wsub, GameCanvas.h))
				{
					list.update_Pos_UP_DOWN();
				}
				num = list.cmx;
			}
			if (GameCanvas.isPointer(xsub, ysub, wsub, hsub))
			{
				int num2 = (GameCanvas.px - xsubpaint) / (wc + w2c) + (GameCanvas.py - ysubpaint + num) / (hc + w2c) * numw;
				int num3 = vecServer.size();
				if (num2 >= 0 && num2 < num3)
				{
					mainSelect = num2;
					for (int i = 0; i < vecServer.size(); i++)
					{
						Command command = (Command)vecServer.elementAt(i);
						if (command == null)
						{
							continue;
						}
						if (i == mainSelect)
						{
							if (command.isPointerPressInsideCamera(0, num))
							{
								command.performAction();
							}
						}
						else
						{
							command.isFocus = false;
						}
					}
				}
			}
			if (GameCanvas.isPointer(xinfo - 2, yinfo + hinfo, wCheck + 4, wCheck + 4) && GameCanvas.isPointerJustRelease)
			{
				isShowSv_HaveChar = !isShowSv_HaveChar;
				GetVecTypeSv(select_Area, select_typeSv);
			}
			if (ntypeSv == 1)
			{
				return;
			}
			for (sbyte b = 0; b < ntypeSv; b++)
			{
				int num4 = yPop + b * (hPop + 5);
				if (GameCanvas.isPointerHoldIn(xPop, num4, wPop, hPop) && GameCanvas.isPointerDown)
				{
					GetVecTypeSv(select_Area, b);
					break;
				}
			}
		}

	private void UpdTouch_NewUI_Popup()
		{
			if (GameCanvas.isPointer(xPopUp_Area, yBox, wBox, hBox) && GameCanvas.isPointerJustRelease)
			{
				isPaint_select_area = !isPaint_select_area;
				isPaint_select_lang = false;
				GameCanvas.isPointerJustRelease = false;
			}
			if (!isPaint_select_area)
			{
				return;
			}
			for (sbyte b = 0; b < strArea.Length; b++)
			{
				int num = yPopUp_Area + b * htext;
				if (GameCanvas.isPointerHoldIn(xPopUp_Area, num, wBox, htext) && GameCanvas.isPointerDown)
				{
					if (isChooseArea)
					{
						select_Area = b;
					}
					else
					{
						SetNewSelectMenu(b, select_typeSv);
					}
					isPaint_select_lang = (isPaint_select_area = false);
					break;
				}
			}
		}

	private void Load_NewUI()
		{
			if (GameCanvas.isTouch)
			{
				if (Rms.loadRMS("area_select") == null)
				{
					isChooseArea = true;
					cmdChooseArea = new Command(mResources.OK, this, 999, null);
					cmdChooseArea.x = GameCanvas.hw - 38;
					cmdChooseArea.y = GameCanvas.hh + 50;
					vecServer = new MyVector();
					vecServer.addElement(cmdChooseArea);
					yBox = GameCanvas.hh - 30;
					wBox = 70;
					hBox = 20;
				}
				else
				{
					isChooseArea = false;
					Load_RMS_Area();
					SetNewSelectMenu(select_Area, select_typeSv);
				}
			}
		}

	private void Save_RMS_Area()
		{
			Rms.saveRMS("area_select", new sbyte[2] { select_Area, select_Lang });
		}

	private void Load_RMS_Area()
		{
			sbyte[] array = Rms.loadRMS("area_select");
			try
			{
				select_Area = array[0];
				select_Lang = array[1];
			}
			catch (Exception)
			{
				select_Area = (select_Lang = 0);
			}
		}

	public void Sort_NewSv()
		{
			for (int i = 0; i < vecServer.size() - 1; i++)
			{
				Command command = (Command)vecServer.elementAt(i);
				for (int j = i + 1; j < vecServer.size(); j++)
				{
					Command command2 = (Command)vecServer.elementAt(j);
					if (command2.isPaintNew && !command.isPaintNew)
					{
						Command command3 = command2;
						command2 = command;
						command = command3;
						vecServer.setElementAt(command, i);
						vecServer.setElementAt(command2, j);
					}
				}
			}
		}

	public void loadIconHead()
		{
			if (iconHead == null)
			{
				iconHead = new Image[3];
				for (int i = 0; i < iconHead.Length; i++)
				{
					iconHead[i] = GameCanvas.loadImage("/iconHead_" + i + ".png");
				}
			}
		}

}
