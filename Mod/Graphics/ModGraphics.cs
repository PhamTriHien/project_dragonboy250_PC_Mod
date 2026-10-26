using System;
using UnityEngine;

public static class ModGraphics
{
	// 0: Ultra (mặc định), 1: Medium (xóa hiệu ứng động), 2: Low (xóa bg), 3: Super Low
	public static int graphicsQuality = 0;
	public static readonly string[] graphicsNames = new string[4] { "Ultra", "Medium", "Low", "Super Low" };

	// 2 chế độ đồ hoạ: 0: Đồ Hoạ Java (tài nguyên x1 + font bitmap từ src Java gốc), 1: Đồ Hoạ HD (tài nguyên x2 sắc nét)
	public static int renderMode = 0;
	public static bool IsJavaGraphics => renderMode == 0;
	public static readonly string[] renderModeNames = new string[2] { "Đồ Hoạ Java", "Đồ Hoạ HD" };

	public static int renderBitDepth
	{
		get => renderMode;
		set => renderMode = (value >= 1) ? 1 : 0;
	}
	public static string[] bitDepthNames => renderModeNames;

	// 0: 1024x600 (Gốc), 1: 1280x720 (HD 16:9), 2: 1600x900 (HD+), 3: 1920x1080 (Full HD)
	public static int resolutionIndex = 0;
	public static readonly int[,] resolutionList = new int[4, 2]
	{
		{ 1024, 600 },
		{ 1280, 720 },
		{ 1600, 900 },
		{ 1920, 1080 }
	};
	public static readonly string[] resolutionNames = new string[4] { "1024x600", "1280x720", "1600x900", "1920x1080" };
	public static bool isFullscreen = false;
	public static bool isAfkLowCpu = false;
	public static bool hideOtherPlayers = false;

	public static void ToggleHideOtherPlayers()
	{
		hideOtherPlayers = !hideOtherPlayers;
		ModConfig.SaveConfig();
		if (GameScr.info1 != null)
		{
			GameScr.info1.addInfo("Xóa người chơi khác: " + (hideOtherPlayers ? "BẬT" : "TẮT"), 0);
		}
		SoundMn.gI().buttonClick();
	}

	public static bool ShouldHideChar(Char c)
	{
		if (!hideOtherPlayers || c == null) return false;
		Char me = Char.myCharz();
		if (c == me || c.me) return false;
		if (me != null && (c.charID == -me.charID || me.charFocus == c)) return false;
		if (c.head == 31) return false;
		if (c.cTypePk == 5 || c.cTypePk == 3) return false;
		if (c.charID < 0 && !c.isPet && !c.isMiniPet) return false;
		if (!string.IsNullOrEmpty(c.cName) && ModBossNotice.IsBossName(c.cName)) return false;
		return true;
	}

	public static int fontSizeIndex = 1;
	public static readonly string[] fontSizeNames = new string[4] { "Nhỏ 65%", "Chuẩn 75%", "Lớn 90%", "Gốc 100%" };
	public static readonly float[] fontSizeScales = new float[4] { 0.65f, 0.75f, 0.90f, 1.0f };

	public static int animSpeedIndex = 0;
	public static readonly float[] animSpeedValues = new float[8] { 1.0f, 1.25f, 1.5f, 2.0f, 2.5f, 3.0f, 4.0f, 5.0f };
	public static readonly string[] animSpeedNames = new string[8] { "x1.0 Gốc", "x1.25", "x1.5", "x2.0", "x2.5", "x3.0", "x4.0", "x5.0" };

	public static float GetAnimSpeed()
	{
		if (animSpeedIndex >= 0 && animSpeedIndex < animSpeedValues.Length)
		{
			return animSpeedValues[animSpeedIndex];
		}
		return 1.0f;
	}

	public static void ApplyAnimSpeed()
	{
		try
		{
			float speed = GetAnimSpeed();
			Time.timeScale = speed;
			Time.fixedDeltaTime = 0.02f;
			Time.maximumDeltaTime = (speed > 1.0f) ? (0.1f * speed) : 0.1f;
		}
		catch
		{
		}
	}

	public static void SetAnimSpeed(int index, bool save = true)
	{
		if (index < 0 || index >= animSpeedValues.Length)
		{
			index = 0;
		}
		animSpeedIndex = index;
		ApplyAnimSpeed();
		if (save)
		{
			ModConfig.SaveConfig();
		}
	}

	public static void ToggleAfkLowCpu()
	{
		isAfkLowCpu = !isAfkLowCpu;
		ModFps.ApplyFPS();
		ModConfig.SaveConfig();
		if (GameScr.info1 != null)
		{
			GameScr.info1.addInfo("Treo Giảm CPU (15 FPS + Ẩn Nền): " + (isAfkLowCpu ? "BẬT" : "TẮT"), 0);
		}
	}

	private static int lastScreenWidth = 0;
	private static int lastScreenHeight = 0;
	private static bool lastFullscreen = false;

	public static void InitGraphics()
	{
		try
		{
			// Kích hoạt bộ lọc nét đồ họa phần cứng GPU trong Unity, chống nhòe mờ
			QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
			QualitySettings.antiAliasing = 4;
			QualitySettings.masterTextureLimit = 0;
			QualitySettings.vSyncCount = 0;

			// Mặc định game luôn mở ở dạng cửa sổ nhỏ hiển thị giữa màn hình desktop
			isFullscreen = false;
			if (resolutionIndex < 0 || resolutionIndex >= resolutionNames.Length)
			{
				resolutionIndex = 0;
			}
			ApplyResolution(resolutionIndex);
			ApplyAnimSpeed();
		}
		catch
		{
		}
	}

	public static void ToggleFullscreen()
	{
		try
		{
			isFullscreen = !isFullscreen;
			if (isFullscreen)
			{
				Resolution maxRes = Screen.currentResolution;
				Screen.SetResolution(maxRes.width, maxRes.height, true);
			}
			else
			{
				int w = resolutionList[resolutionIndex, 0];
				int h = resolutionList[resolutionIndex, 1];
				Screen.SetResolution(w, h, false);
			}
			ModConfig.SaveConfig();
		}
		catch
		{
		}
	}

	public static void ApplyResolution(int index)
	{
		try
		{
			if (index < 0 || index >= resolutionNames.Length) index = 0;
			resolutionIndex = index;
			int w = resolutionList[index, 0];
			int h = resolutionList[index, 1];
			Screen.SetResolution(w, h, isFullscreen);
			ModConfig.SaveConfig();
		}
		catch
		{
		}
	}

	public static void UpdateResolutionWatcher()
	{
		try
		{
			if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight || Screen.fullScreen != lastFullscreen)
			{
				lastScreenWidth = Screen.width;
				lastScreenHeight = Screen.height;
				lastFullscreen = Screen.fullScreen;
				isFullscreen = Screen.fullScreen;

				ScaleGUI.WIDTH = Screen.width;
				ScaleGUI.HEIGHT = Screen.height;
				if (MotherCanvas.instance != null)
				{
					MotherCanvas.instance.checkZoomLevel(Screen.width, Screen.height);
				}
				if (GameMidlet.gameCanvas != null)
				{
					GameMidlet.gameCanvas.initGameCanvas();
				}
				if (TileMap.tileID > 0)
				{
					TileMap.loadTileImage();
					TileMap.loadMainTile();
				}
				if (GameScr.instance != null && GameCanvas.currentScreen == GameScr.instance && TileMap.tmw > 0 && TileMap.tmh > 0)
				{
					GameScr.loadCamera(true, -1, -1);
				}
				GameCanvas.getYBackground(GameCanvas.typeBg);
				if (GameScr.gamePad != null)
				{
					GameScr.gamePad.updateZone();
				}
				GameScr.setSkillBarPosition();
			}
		}
		catch
		{
		}
	}

	public static string GetCurrentQualityName()
	{
		if (graphicsQuality >= 0 && graphicsQuality < graphicsNames.Length)
		{
			return graphicsNames[graphicsQuality];
		}
		return graphicsNames[0];
	}

	public static void SetQuality(int quality)
	{
		if (quality >= 0 && quality < graphicsNames.Length)
		{
			graphicsQuality = quality;
			ModConfig.SaveConfig();
		}
	}

	public static void CycleQuality()
	{
		graphicsQuality = (graphicsQuality + 1) % graphicsNames.Length;
		ModConfig.SaveConfig();
	}

	public static void SetRenderMode(int mode, bool refreshAssets = true)
	{
		int normalized = (mode >= 1) ? 1 : 0;
		bool changed = (renderMode != normalized);
		renderMode = normalized;
		ApplyRenderMode();
		if (refreshAssets)
		{
			ModConfig.SaveConfig();
			if (changed && GameMidlet.gameCanvas != null)
			{
				RefreshGraphicsAssets();
			}
		}
	}

	public static void SetBitDepth(int depth) => SetRenderMode(depth);

	public static void RefreshGraphicsAssets()
	{
		try
		{
			if (GameMidlet.gameCanvas == null)
			{
				return;
			}
			if (mGraphics.cachedTextures != null)
			{
				mGraphics.cachedTextures.Clear();
			}
			mSystem.ClearImageCache();
			mFont.ReloadJavaFontImages();
			mFont.InvalidateFontMetrics();
			Paint.loadbg();
			Paint.imgCheck = GameCanvas.loadImage("/mainImage/myTexture2dcheck.png");
			for (int i = 0; i < 3; i++)
			{
				GameCanvas.imgBorder[i] = GameCanvas.loadImage("/mainImage/myTexture2dbd" + i + ".png");
			}
			GameCanvas.borderConnerW = mGraphics.getImageWidth(GameCanvas.imgBorder[0]);
			GameCanvas.borderConnerH = mGraphics.getImageHeight(GameCanvas.imgBorder[0]);
			GameCanvas.borderCenterW = mGraphics.getImageWidth(GameCanvas.imgBorder[1]);
			GameCanvas.borderCenterH = mGraphics.getImageHeight(GameCanvas.imgBorder[1]);
			GameCanvas.imgShuriken = GameCanvas.loadImage("/mainImage/myTexture2df.png");
			GameCanvas.imgClear = GameCanvas.loadImage("/mainImage/myTexture2der.png");
			Command.btn0left = GameCanvas.loadImage("/mainImage/btn0left.png");
			Command.btn0mid = GameCanvas.loadImage("/mainImage/btn0mid.png");
			Command.btn0right = GameCanvas.loadImage("/mainImage/btn0right.png");
			Command.btn1left = GameCanvas.loadImage("/mainImage/btn1left.png");
			Command.btn1mid = GameCanvas.loadImage("/mainImage/btn1mid.png");
			Command.btn1right = GameCanvas.loadImage("/mainImage/btn1right.png");
			if (GameCanvas.instance != null)
			{
				GameCanvas.instance.loadDust();
				GameCanvas.instance.loadWaterSplash();
			}
			for (int j = 0; j < 7; j++)
			{
				GameCanvas.imgBlue[j] = GameCanvas.loadImage("/effectdata/blue/" + j + ".png");
				GameCanvas.imgViolet[j] = GameCanvas.loadImage("/effectdata/violet/" + j + ".png");
			}
			GameScr.ReloadHudImages();
			Panel.loadBg();
			Menu.loadBg();
			PopUp.goc = null;
			PopUp.imgPopUp = null;
			PopUp.imgPopUp2 = null;
			PopUp.loadBg();
			Info.gocnhon = GameCanvas.loadImage("/mainImage/myTexture2dgocnhon.png");
			TField.imgTf = GameCanvas.loadImage("/mainImage/myTexture2dtf.png");
			ModUI.ReloadForGraphicsMode();
			SmallImage.ReloadForGraphicsMode();
			Mob.ReloadForGraphicsMode();
			Effect.ReloadForGraphicsMode();
			ImgByName.ReloadForGraphicsMode();
			Char.ReloadForGraphicsMode();
			ItemMap.imageFlare = GameCanvas.loadImage("/mainImage/myTexture2dflare.png");
			ItemMap.imageAuraItem1 = GameCanvas.loadImage("/mainImage/myTexture2ditemaura1.png");
			ItemMap.imageAuraItem2 = GameCanvas.loadImage("/mainImage/myTexture2ditemaura2.png");
			ItemMap.imageAuraItem3 = GameCanvas.loadImage("/mainImage/myTexture2ditemaura3.png");
			MagicTree.pea = GameCanvas.loadImage("/mainImage/myTexture2dhatdau.png");
			EffectFeet.imgFeet1 = GameCanvas.loadImage("/mainImage/myTexture2dmove-1.png");
			EffectFeet.imgFeet3 = GameCanvas.loadImage("/mainImage/myTexture2dmove-3.png");
			if (Teleport.maybay != null)
			{
				for (int m = 0; m < 3 && m < Teleport.maybay.Length; m++)
				{
					if (Teleport.maybay[m] != null)
					{
						Teleport.maybay[m] = GameCanvas.loadImage("/mainImage/myTexture2dmaybay" + (m + 1) + ".png");
					}
				}
				if (Teleport.maybay.Length > 3 && Teleport.maybay[3] != null)
				{
					Teleport.maybay[3] = GameCanvas.loadImage("/mainImage/myTexture2dmaybay4a.png");
				}
				if (Teleport.maybay.Length > 4 && Teleport.maybay[4] != null)
				{
					Teleport.maybay[4] = GameCanvas.loadImage("/mainImage/myTexture2dmaybay4b.png");
				}
			}
			if (Teleport.hole != null)
			{
				Teleport.hole = GameCanvas.loadImage("/mainImage/hole.png");
			}
			if (TransportScr.ship != null)
			{
				TransportScr.ship = GameCanvas.loadImage("/mainImage/myTexture2dfutherShip.png");
			}
			if (TransportScr.taungam != null)
			{
				TransportScr.taungam = GameCanvas.loadImage("/mainImage/taungam.png");
			}
			RadarScr.ReloadForGraphicsMode();
			CrackBallScr.ReloadForGraphicsMode();
			TileMap.loadBg();
			if (TileMap.tileID > 0)
			{
				TileMap.loadTileImage();
				TileMap.loadMainTile();
			}
			BgItem.ReloadCurrMapItems();
			BackgroudEffect.ReloadForGraphicsMode();
			if (GameCanvas.typeBg >= 0)
			{
				int curBg = GameCanvas.typeBg;
				TileMap.lastBgID = -1;
				GameCanvas.loadBG(curBg);
			}
			mScreen.ITEM_HEIGHT = mFont.tahoma_8b.getHeight() + 8;
		}
		catch
		{
		}
	}

	public static float GetFontSizeScale()
	{
		if (fontSizeIndex >= 0 && fontSizeIndex < fontSizeScales.Length)
		{
			return fontSizeScales[fontSizeIndex];
		}
		return 0.75f;
	}

	public static void SetFontSize(int index)
	{
		if (index < 0 || index >= fontSizeNames.Length) index = 1;
		fontSizeIndex = index;
		mFont.InvalidateFontMetrics();
		ModConfig.SaveConfig();
	}

	public static void ApplyRenderMode()
	{
		try
		{
			Type gameViewType = Type.GetType("DragonBoy_Android_Host.GameView, DragonBoy_Android");
			if (gameViewType != null)
			{
				var method = gameViewType.GetMethod("ApplyHardwareBitDepth", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
				method?.Invoke(null, new object[] { renderMode });
			}
		}
		catch { }
	}

	public static void ApplyBitDepth() => ApplyRenderMode();
}
