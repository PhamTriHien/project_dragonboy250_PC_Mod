using System;
using Assets.src.g;
public partial class GameScr : mScreen, IChatable
{
	public GameScr()
		{
			if (GameCanvas.w == 128 || GameCanvas.h <= 208)
			{
				indexSize = 20;
			}
			cmdback = new Command(string.Empty, 11021);
			cmdMenu = new Command("menu", 11000);
			cmdFocus = new Command(string.Empty, 11001);
			cmdMenu.img = imgMenu;
			int mImgW = (imgMenu != null) ? mGraphics.getImageWidth(imgMenu) : 0;
			int mImgH = (imgMenu != null) ? mGraphics.getImageHeight(imgMenu) : 0;
			cmdMenu.w = (mImgW > 0) ? (mImgW + 20) : 60;
			cmdMenu.h = (mImgH > 0) ? (mImgH + 12) : 32;
			cmdMenu.isPlaySoundButton = false;
			cmdFocus.img = imgFocus;
			if (GameCanvas.isTouch)
			{
				cmdMenu.x = 0;
				cmdMenu.y = 50;
				cmdFocus = null;
			}
			else
			{
				cmdMenu.x = 0;
				cmdMenu.y = gH - 30;
				cmdFocus.x = gW - 32;
				cmdFocus.y = gH - 32;
			}
			left = cmdMenu;
			right = cmdFocus;
			isPaintRada = 1;
			if (GameCanvas.isTouch)
			{
				isHaveSelectSkill = true;
			}
			cmdDoiCo = new Command("Đổi cờ", GameCanvas.gI(), 100001, null);
			cmdLogOut = new Command("Logout", GameCanvas.gI(), 100002, null);
			cmdChatTheGioi = new Command("chat world", GameCanvas.gI(), 100003, null);
			cmdshowInfo = new Command("InfoLog", GameCanvas.gI(), 100004, null);
			cmdDoiCo.setType();
			cmdLogOut.setType();
			cmdChatTheGioi.setType();
			cmdshowInfo.setType();
			cmdChatTheGioi.x = GameCanvas.w - cmdChatTheGioi.w;
			cmdshowInfo.x = GameCanvas.w - cmdshowInfo.w;
			cmdLogOut.x = GameCanvas.w - cmdLogOut.w;
			cmdDoiCo.x = GameCanvas.w - cmdDoiCo.w;
			cmdChatTheGioi.y = cmdChatTheGioi.h + mFont.tahoma_7_white.getHeight();
			cmdshowInfo.y = cmdChatTheGioi.h * 2 + mFont.tahoma_7_white.getHeight();
			cmdLogOut.y = cmdChatTheGioi.h * 3 + mFont.tahoma_7_white.getHeight();
			cmdDoiCo.y = cmdChatTheGioi.h * 4 + mFont.tahoma_7_white.getHeight();
		}
	public static void loadBg()
		{
			fra_PVE_Bar_0 = new FrameImage(mSystem.loadImage("/mainImage/i_pve_bar_0.png"), 6, 15);
			fra_PVE_Bar_1 = new FrameImage(mSystem.loadImage("/mainImage/i_pve_bar_1.png"), 38, 21);
			imgVS = mSystem.loadImage("/mainImage/i_vs.png");
			imgBall = mSystem.loadImage("/mainImage/i_charlife.png");
			imgHP_NEW = mSystem.loadImage("/mainImage/i_hp.png");
			imgKhung = mSystem.loadImage("/mainImage/i_khung.png");
			imgLbtn = GameCanvas.loadImage("/mainImage/myTexture2dbtnl.png");
			imgLbtnFocus = GameCanvas.loadImage("/mainImage/myTexture2dbtnlf.png");
			imgLbtn2 = GameCanvas.loadImage("/mainImage/myTexture2dbtnl2.png");
			imgLbtnFocus2 = GameCanvas.loadImage("/mainImage/myTexture2dbtnlf2.png");
			imgPanel = GameCanvas.loadImage("/mainImage/myTexture2dpanel.png");
			imgPanel2 = GameCanvas.loadImage("/mainImage/panel2.png");
			imgHP = GameCanvas.loadImage("/mainImage/myTexture2dHP.png");
			imgSP = GameCanvas.loadImage("/mainImage/SP.png");
			imgHPLost = GameCanvas.loadImage("/mainImage/myTexture2dhpLost.png");
			imgMPLost = GameCanvas.loadImage("/mainImage/myTexture2dmpLost.png");
			imgMP = GameCanvas.loadImage("/mainImage/myTexture2dMP.png");
			imgSkill = GameCanvas.loadImage("/mainImage/myTexture2dskill.png");
			imgSkill2 = GameCanvas.loadImage("/mainImage/myTexture2dskill2.png");
			imgMenu = GameCanvas.loadImage("/mainImage/myTexture2dmenu.png");
			if (imgMenu != null && instance != null && instance.cmdMenu != null)
			{
				instance.cmdMenu.img = imgMenu;
				instance.cmdMenu.w = mGraphics.getImageWidth(imgMenu) + 20;
				instance.cmdMenu.h = mGraphics.getImageHeight(imgMenu) + 12;
			}
			imgFocus = GameCanvas.loadImage("/mainImage/myTexture2dfocus.png");
			imgHP_tm_do = GameCanvas.loadImage("/mainImage/tm-do.png");
			imgHP_tm_vang = GameCanvas.loadImage("/mainImage/tm-vang.png");
			imgHP_tm_xam = GameCanvas.loadImage("/mainImage/tm-xam.png");
			imgHP_tm_xanh = GameCanvas.loadImage("/mainImage/tm-xanh.png");
			imgChatPC = GameCanvas.loadImage("/pc/chat.png");
			imgChatsPC2 = GameCanvas.loadImage("/pc/chat2.png");
			imgArrow = GameCanvas.loadImage("/mainImage/myTexture2darrow.png");
			imgArrow2 = GameCanvas.loadImage("/mainImage/myTexture2darrow2.png");
			if (GameCanvas.isTouch)
			{
				imgChat = GameCanvas.loadImage("/mainImage/myTexture2dchat.png");
				imgChat2 = GameCanvas.loadImage("/mainImage/myTexture2dchat2.png");
				imgFocus2 = GameCanvas.loadImage("/mainImage/myTexture2dfocus2.png");
				imgHP1 = GameCanvas.loadImage("/mainImage/myTexture2dPea0.png");
				imgHP2 = GameCanvas.loadImage("/mainImage/myTexture2dPea1.png");
				imgAnalog1 = GameCanvas.loadImage("/mainImage/myTexture2danalog1.png");
				imgAnalog2 = GameCanvas.loadImage("/mainImage/myTexture2danalog2.png");
				imgHP3 = GameCanvas.loadImage("/mainImage/myTexture2dPea2.png");
				imgHP4 = GameCanvas.loadImage("/mainImage/myTexture2dPea3.png");
				imgFire0 = GameCanvas.loadImage("/mainImage/myTexture2dfirebtn0.png");
				imgFire1 = GameCanvas.loadImage("/mainImage/myTexture2dfirebtn1.png");
			}
			imgNR1 = GameCanvas.loadImage("/mainImage/myTexture2dPea_0.png");
			imgNR2 = GameCanvas.loadImage("/mainImage/myTexture2dPea_1.png");
			imgNR3 = GameCanvas.loadImage("/mainImage/myTexture2dPea_2.png");
			imgNR4 = GameCanvas.loadImage("/mainImage/myTexture2dPea_3.png");
			flyTextX = new int[5];
			flyTextY = new int[5];
			flyTextDx = new int[5];
			flyTextDy = new int[5];
			flyTextState = new int[5];
			flyTextString = new string[5];
			flyTextYTo = new int[5];
			flyTime = new int[5];
			flyTextColor = new int[8];
			for (int i = 0; i < 5; i++)
			{
				flyTextState[i] = -1;
			}
			sbyte[] array = Rms.loadRMS("NRdataVersion");
			sbyte[] array2 = Rms.loadRMS("NRmapVersion");
			sbyte[] array3 = Rms.loadRMS("NRskillVersion");
			sbyte[] array4 = Rms.loadRMS("NRitemVersion");
			if (array != null)
			{
				vcData = array[0];
			}
			if (array2 != null)
			{
				vcMap = array2[0];
			}
			if (array3 != null)
			{
				vcSkill = array3[0];
			}
			if (array4 != null)
			{
				vcItem = array4[0];
			}
			imgNut = GameCanvas.loadImage("/mainImage/myTexture2dnut.png");
			imgNutF = GameCanvas.loadImage("/mainImage/myTexture2dnutF.png");
			MobCapcha.init();
			isAnalog = ((Rms.loadRMSInt("analog") == 1) ? 1 : 0);
			gamePad = new GamePad();
			arrow = GameCanvas.loadImage("/mainImage/myTexture2darrow3.png");
			imgTrans = GameCanvas.loadImage("/bg/trans.png");
			imgRoomStat = GameCanvas.loadImage("/mainImage/myTexture2dstat.png");
			frBarPow0 = GameCanvas.loadImage("/mainImage/myTexture2dlineColor20.png");
			frBarPow1 = GameCanvas.loadImage("/mainImage/myTexture2dlineColor21.png");
			frBarPow2 = GameCanvas.loadImage("/mainImage/myTexture2dlineColor22.png");
			frBarPow20 = GameCanvas.loadImage("/mainImage/myTexture2dlineColor00.png");
			frBarPow21 = GameCanvas.loadImage("/mainImage/myTexture2dlineColor01.png");
			frBarPow22 = GameCanvas.loadImage("/mainImage/myTexture2dlineColor02.png");
		}
	public void initSelectChar()
		{
			readPart();
			SmallImage.init();
		}
	public void initTraining()
		{
			if (CreateCharScr.isCreateChar)
			{
				CreateCharScr.isCreateChar = false;
				right = null;
			}
		}
	public bool isMapDocNhan()
		{
			if (TileMap.mapID >= 53 && TileMap.mapID <= 62)
			{
				return true;
			}
			return false;
		}
	public bool isMapFize()
		{
			if (TileMap.mapID >= 63)
			{
				return true;
			}
			return false;
		}
	public override void switchToMe()
		{
			vChatVip.removeAllElements();
			ServerListScreen.isWait = false;
			if (BackgroudEffect.isHaveRain())
			{
				SoundMn.gI().rain();
			}
			LoginScr.isContinueToLogin = false;
			Char.isLoadingMap = false;
			if (!isPaintOther)
			{
				Service.gI().finishLoadMap();
			}
			if (TileMap.isTrainingMap())
			{
				initTraining();
			}
			info1.isUpdate = true;
			info2.isUpdate = true;
			resetButton();
			isLoadAllData = true;
			isPaintOther = false;
			base.switchToMe();
		}
	public static int getMaxExp(int level)
		{
			int num = 0;
			for (int i = 0; i <= level; i++)
			{
				num += (int)exps[i];
			}
			return num;
		}
	public static void resetAllvector()
		{
			vCharInMap.removeAllElements();
			Teleport.vTeleport.removeAllElements();
			vItemMap.removeAllElements();
			Effect2.vEffect2.removeAllElements();
			Effect2.vAnimateEffect.removeAllElements();
			Effect2.vEffect2Outside.removeAllElements();
			Effect2.vEffectFeet.removeAllElements();
			Effect2.vEffect3.removeAllElements();
			vMobAttack.removeAllElements();
			vMob.removeAllElements();
			vNpc.removeAllElements();
			Char.myCharz().vMovePoints.removeAllElements();
		}
	public bool isBagFull()
		{
			for (int num = Char.myCharz().arrItemBag.Length - 1; num >= 0; num--)
			{
				if (Char.myCharz().arrItemBag[num] == null)
				{
					return false;
				}
			}
			return true;
		}
	public void createConfirm(string[] menu, Npc npc)
		{
			resetButton();
			isLockKey = true;
			left = new Command(menu[0], 130011, npc);
			right = new Command(menu[1], 130012, npc);
		}
	public void readPart()
		{
			DataInputStream dataInputStream = null;
			try
			{
				dataInputStream = new DataInputStream(Rms.loadRMS("NR_part"));
				int num = dataInputStream.readShort();
				parts = new Part[num];
				for (int i = 0; i < num; i++)
				{
					int type = dataInputStream.readByte();
					parts[i] = new Part(type);
					for (int j = 0; j < parts[i].pi.Length; j++)
					{
						parts[i].pi[j] = new PartImage();
						parts[i].pi[j].id = dataInputStream.readShort();
						parts[i].pi[j].dx = dataInputStream.readByte();
						parts[i].pi[j].dy = dataInputStream.readByte();
					}
				}
			}
			catch (Exception ex)
			{
				Cout.LogError("LOI TAI readPart " + ex.ToString());
			}
			finally
			{
				try
				{
					dataInputStream.close();
				}
				catch (Exception ex2)
				{
					Res.outz2("LOI TAI readPart 2" + ex2.StackTrace);
				}
			}
		}
	public void readEfect()
		{
			DataInputStream dataInputStream = null;
			try
			{
				dataInputStream = new DataInputStream(Rms.loadRMS("NR_effect"));
				int num = dataInputStream.readShort();
				efs = new EffectCharPaint[num];
				for (int i = 0; i < num; i++)
				{
					efs[i] = new EffectCharPaint();
					efs[i].idEf = dataInputStream.readShort();
					efs[i].arrEfInfo = new EffectInfoPaint[dataInputStream.readByte()];
					for (int j = 0; j < efs[i].arrEfInfo.Length; j++)
					{
						efs[i].arrEfInfo[j] = new EffectInfoPaint();
						efs[i].arrEfInfo[j].idImg = dataInputStream.readShort();
						efs[i].arrEfInfo[j].dx = dataInputStream.readByte();
						efs[i].arrEfInfo[j].dy = dataInputStream.readByte();
					}
				}
			}
			catch (Exception)
			{
			}
			finally
			{
				try
				{
					dataInputStream.close();
				}
				catch (Exception ex2)
				{
					Cout.LogError("Loi ham Eff: " + ex2.ToString());
				}
			}
		}
	public void readArrow()
		{
			DataInputStream dataInputStream = null;
			try
			{
				dataInputStream = new DataInputStream(Rms.loadRMS("NR_arrow"));
				int num = dataInputStream.readShort();
				arrs = new Arrowpaint[num];
				for (int i = 0; i < num; i++)
				{
					arrs[i] = new Arrowpaint();
					arrs[i].id = dataInputStream.readShort();
					arrs[i].imgId[0] = dataInputStream.readShort();
					arrs[i].imgId[1] = dataInputStream.readShort();
					arrs[i].imgId[2] = dataInputStream.readShort();
				}
			}
			catch (Exception)
			{
			}
			finally
			{
				try
				{
					dataInputStream.close();
				}
				catch (Exception ex2)
				{
					Cout.LogError("Loi ham readArrow: " + ex2.ToString());
				}
			}
		}
	public void readOk()
		{
			try
			{
				Res.outz("<readOk><vsData<" + vsData + "==" + vcData);
				Res.outz("<readOk><vsMap<" + vsMap + "==" + vcMap);
				Res.outz("<readOk><vsSkill<" + vsSkill + "==" + vcSkill);
				Res.outz("<readOk><vsItem<" + vsItem + "==" + vcItem);
				if (vsData == vcData && vsMap == vcMap && vsSkill == vcSkill && vsItem == vcItem)
				{
					Res.outz(vsData + "," + vsMap + "," + vsSkill + "," + vsItem);
					gI().readDart();
					gI().readEfect();
					gI().readArrow();
					gI().readSkill();
					Service.gI().clientOk();
				}
			}
			catch (Exception ex)
			{
				Cout.LogError("Loi ham readOk: " + ex.ToString());
			}
		}
	public static GameScr gI()
		{
			if (instance == null)
			{
				instance = new GameScr();
			}
			return instance;
		}
	public static void clearGameScr()
		{
			instance = null;
		}
	public void loadGameScr()
		{
			loadSplash();
			Res.init();
			loadInforBar();
		}

}
