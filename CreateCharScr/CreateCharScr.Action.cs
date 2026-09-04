using System;

public partial class CreateCharScr
{
	public static void loadMapFromResource(sbyte[] mapID)
		{
			Res.outz("newwwwwwwwww =============");
			DataInputStream dataInputStream = null;
			for (int i = 0; i < mapID.Length; i++)
			{
				dataInputStream = MyStream.readFile("/mymap/" + mapID[i]);
				MapTemplate.tmw[i] = (ushort)dataInputStream.read();
				MapTemplate.tmh[i] = (ushort)dataInputStream.read();
				Cout.LogError("Thong TIn : " + MapTemplate.tmw[i] + "::" + MapTemplate.tmh[i]);
				MapTemplate.maps[i] = new int[dataInputStream.available()];
				Cout.LogError("lent= " + MapTemplate.maps[i].Length);
				for (int j = 0; j < MapTemplate.tmw[i] * MapTemplate.tmh[i]; j++)
				{
					MapTemplate.maps[i][j] = dataInputStream.read();
				}
				MapTemplate.types[i] = new int[MapTemplate.maps[i].Length];
			}
		}

	public void loadMapTableFromResource(sbyte[] mapID)
		{
			if (GameCanvas.lowGraphic)
			{
				return;
			}
			DataInputStream dataInputStream = null;
			try
			{
				for (int i = 0; i < mapID.Length; i++)
				{
					dataInputStream = MyStream.readFile("/mymap/mapTable" + mapID[i]);
					Cout.LogError("mapTable : " + mapID[i]);
					short num = dataInputStream.readShort();
					MapTemplate.vCurrItem[i] = new MyVector();
					Res.outz("nItem= " + num);
					for (int j = 0; j < num; j++)
					{
						short id = dataInputStream.readShort();
						short num2 = dataInputStream.readShort();
						short num3 = dataInputStream.readShort();
						if (TileMap.getBIById(id) != null)
						{
							BgItem bIById = TileMap.getBIById(id);
							BgItem bgItem = new BgItem();
							bgItem.id = id;
							bgItem.idImage = bIById.idImage;
							bgItem.dx = bIById.dx;
							bgItem.dy = bIById.dy;
							bgItem.x = num2 * TileMap.size;
							bgItem.y = num3 * TileMap.size;
							bgItem.layer = bIById.layer;
							MapTemplate.vCurrItem[i].addElement(bgItem);
							if (!BgItem.imgNew.containsKey(bgItem.idImage + string.Empty))
							{
								try
								{
									Image image = GameCanvas.loadImage("/mapBackGround/" + bgItem.idImage + ".png");
									if (image == null)
									{
										BgItem.imgNew.put(bgItem.idImage + string.Empty, Image.createRGBImage(new int[1], 1, 1, bl: true));
										Service.gI().getBgTemplate(bgItem.idImage);
									}
									else
									{
										BgItem.imgNew.put(bgItem.idImage + string.Empty, image);
									}
								}
								catch (Exception)
								{
									Image image2 = GameCanvas.loadImage("/mapBackGround/" + bgItem.idImage + ".png");
									if (image2 == null)
									{
										image2 = Image.createRGBImage(new int[1], 1, 1, bl: true);
										Service.gI().getBgTemplate(bgItem.idImage);
									}
									BgItem.imgNew.put(bgItem.idImage + string.Empty, image2);
								}
								BgItem.vKeysLast.addElement(bgItem.idImage + string.Empty);
							}
							if (!BgItem.isExistKeyNews(bgItem.idImage + string.Empty))
							{
								BgItem.vKeysNew.addElement(bgItem.idImage + string.Empty);
							}
							bgItem.changeColor();
						}
						else
						{
							Res.outz("item null");
						}
					}
				}
			}
			catch (Exception ex2)
			{
				Cout.println("LOI TAI loadMapTableFromResource" + ex2.ToString());
			}
		}

	public void doChangeMap()
		{
			TileMap.maps = new int[MapTemplate.maps[indexGender].Length];
			for (int i = 0; i < MapTemplate.maps[indexGender].Length; i++)
			{
				TileMap.maps[i] = MapTemplate.maps[indexGender][i];
			}
			TileMap.types = MapTemplate.types[indexGender];
			TileMap.pxh = MapTemplate.pxh[indexGender];
			TileMap.pxw = MapTemplate.pxw[indexGender];
			TileMap.tileID = MapTemplate.pxw[indexGender];
			TileMap.tmw = MapTemplate.tmw[indexGender];
			TileMap.tmh = MapTemplate.tmh[indexGender];
			TileMap.tileID = bgID[indexGender] + 1;
			TileMap.loadMainTile();
			TileMap.loadTileCreatChar();
			GameCanvas.loadBG(bgID[indexGender]);
			GameScr.loadCamera(fullmScreen: false, cx, cy);
		}

	public override void keyPress(int keyCode)
		{
			tAddName.keyPressed(keyCode);
		}

	public override void update()
		{
			cp1++;
			if (cp1 > 30)
			{
				cp1 = 0;
			}
			if (cp1 % 15 < 5)
			{
				cf = 0;
			}
			else
			{
				cf = 1;
			}
			tAddName.update();
			if (cmdSelectSv != null && cmdSelectSv.isPointerPressInside())
			{
				cmdSelectSv.performAction();
			}
			if (selected != 0)
			{
				tAddName.isFocus = false;
			}
		}

	public override void updateKey()
		{
			if (GameCanvas.keyPressed[(!Main.isPC) ? 2 : 21])
			{
				selected--;
				if (selected < 0)
				{
					selected = mResources.MENUNEWCHAR.Length - 1;
				}
			}
			if (GameCanvas.keyPressed[(!Main.isPC) ? 8 : 22])
			{
				selected++;
				if (selected >= mResources.MENUNEWCHAR.Length)
				{
					selected = 0;
				}
			}
			if (selected == 0)
			{
				if (!GameCanvas.isTouch)
				{
					right = tAddName.cmdClear;
				}
				tAddName.update();
			}
			if (selected == 1)
			{
				if (GameCanvas.keyPressed[(!Main.isPC) ? 4 : 23])
				{
					indexGender--;
					if (indexGender < 0)
					{
						indexGender = mResources.MENUGENDER.Length - 1;
					}
					doChangeMap();
				}
				if (GameCanvas.keyPressed[(!Main.isPC) ? 6 : 24])
				{
					indexGender++;
					if (indexGender > mResources.MENUGENDER.Length - 1)
					{
						indexGender = 0;
					}
					doChangeMap();
				}
				right = null;
			}
			if (selected == 2)
			{
				if (GameCanvas.keyPressed[(!Main.isPC) ? 4 : 23])
				{
					indexHair--;
					if (indexHair < 0)
					{
						indexHair = mResources.hairStyleName[0].Length - 1;
					}
				}
				if (GameCanvas.keyPressed[(!Main.isPC) ? 6 : 24])
				{
					indexHair++;
					if (indexHair > mResources.hairStyleName[0].Length - 1)
					{
						indexHair = 0;
					}
				}
				right = null;
			}
			if (GameCanvas.isPointerJustRelease)
			{
				int num = 110;
				int num2 = 60;
				int num3 = 78;
				if (GameCanvas.w > GameCanvas.h)
				{
					num = 100;
					num2 = 40;
				}
				if (GameCanvas.isPointerHoldIn(GameCanvas.w / 2 - 3 * num3 / 2, 15, num3 * 3, 80))
				{
					selected = 0;
					tAddName.isFocus = true;
				}
				if (GameCanvas.isPointerHoldIn(GameCanvas.w / 2 - 3 * num3 / 2, num - 30, num3 * 3, num2 + 5))
				{
					selected = 1;
					int num4 = indexGender;
					indexGender = (GameCanvas.px - (GameCanvas.w / 2 - 3 * num3 / 2)) / num3;
					if (indexGender < 0)
					{
						indexGender = 0;
					}
					if (indexGender > mResources.MENUGENDER.Length - 1)
					{
						indexGender = mResources.MENUGENDER.Length - 1;
					}
					if (num4 != indexGender)
					{
						doChangeMap();
					}
				}
				if (GameCanvas.isPointerHoldIn(GameCanvas.w / 2 - 3 * num3 / 2, num - 30 + num2 + 5, num3 * 3, 65))
				{
					selected = 2;
					int num5 = indexHair;
					indexHair = (GameCanvas.px - (GameCanvas.w / 2 - 3 * num3 / 2)) / num3;
					if (indexHair < 0)
					{
						indexHair = 0;
					}
					if (indexHair > mResources.hairStyleName[0].Length - 1)
					{
						indexHair = mResources.hairStyleName[0].Length - 1;
					}
					if (num5 != selected)
					{
						doChangeMap();
					}
				}
			}
			if (!TouchScreenKeyboard.visible)
			{
				base.updateKey();
			}
			GameCanvas.clearKeyHold();
			GameCanvas.clearKeyPressed();
		}

	public void perform(int idAction, object p)
		{
			switch (idAction)
			{
			case 8000:
				if (tAddName.getText().Equals(string.Empty))
				{
					GameCanvas.startOKDlg(mResources.char_name_blank);
					break;
				}
				if (tAddName.getText().Length < 5)
				{
					GameCanvas.startOKDlg(mResources.char_name_short);
					break;
				}
				if (tAddName.getText().Length > 15)
				{
					GameCanvas.startOKDlg(mResources.char_name_long);
					break;
				}
				InfoDlg.showWait();
				Service.gI().createChar(tAddName.getText(), indexGender, hairID[indexGender][indexHair]);
				break;
			case 8001:
				if (GameCanvas.loginScr.isLogin2)
				{
					GameCanvas.startYesNoDlg(mResources.note, new Command(mResources.YES, this, 10019, null), new Command(mResources.NO, this, 10020, null));
					break;
				}
				Session_ME.gI().close();
				ServerListScreen.isAutoLogin = false;
				GameCanvas.serverScreen.switchToMe();
				break;
			case 10020:
				GameCanvas.endDlg();
				break;
			case 10019:
				Session_ME.gI().close();
				GameCanvas.endDlg();
				ServerListScreen.isAutoLogin = false;
				GameCanvas.serverScreen.switchToMe();
				break;
			case 10018:
				ServerListScreen.SetIpSelect(-1, issave: true);
				ServerScr.isShowSv_HaveChar = false;
				Controller.isEXTRA_LINK = false;
				GameCanvas.serverScr.switchToMe();
				break;
			}
		}

}
