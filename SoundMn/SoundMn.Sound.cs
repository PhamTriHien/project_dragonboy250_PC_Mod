using System;

public partial class SoundMn
{
	public void loadSound(int mapID)
		{
			Sound.init(new int[3] { AIR_SHIP, RAIN, TAITAONANGLUONG }, new int[39]
			{
				GET_ITEM, MOVE, LOW_PUNCH, LOW_KICK, FLY, JUMP, PANEL_OPEN, BUTTON_CLOSE, BUTTON_CLICK, MEDIUM_PUNCH,
				MEDIUM_KICK, PANEL_OPEN, EAT_PEAN, OPEN_DIALOG, NORMAL_KAME, NAMEK_KAME, XAYDA_KAME, EXPLODE_1, EXPLODE_2, TRAIDAT_KAME,
				HP_UP, THAIDUONGHASAN, HOISINH, GONG, KHICHAY, BIG_EXPLODE, NAMEK_LAZER, NAMEK_CHARGE, RADAR_CLICK, RADAR_ITEM,
				FIREWORK, KAMEX10_0, KAMEX10_1, DESTROY_0, DESTROY_1, MAFUBA_0, MAFUBA_1, MAFUBA_2, DESTROY_2
			});
		}

	public void getSoundOption()
		{
			if (GameCanvas.loginScr.isLogin2 && Char.myCharz().taskMaint != null && Char.myCharz().taskMaint.taskId >= 2)
			{
				Panel.strTool = new string[10]
				{
					mResources.radaCard,
					mResources.quayso,
					mResources.gameInfo,
					mResources.change_flag,
					mResources.change_zone,
					mResources.chat_world,
					mResources.account,
					mResources.option,
					mResources.change_account,
					mResources.REGISTOPROTECT
				};
				if (Char.myCharz().havePet)
				{
					Panel.strTool = new string[11]
					{
						mResources.radaCard,
						mResources.quayso,
						mResources.gameInfo,
						mResources.pet,
						mResources.change_flag,
						mResources.change_zone,
						mResources.chat_world,
						mResources.account,
						mResources.option,
						mResources.change_account,
						mResources.REGISTOPROTECT
					};
				}
			}
			else
			{
				Panel.strTool = new string[9]
				{
					mResources.radaCard,
					mResources.quayso,
					mResources.gameInfo,
					mResources.change_flag,
					mResources.change_zone,
					mResources.chat_world,
					mResources.account,
					mResources.option,
					mResources.change_account
				};
				if (Char.myCharz().havePet)
				{
					Panel.strTool = new string[10]
					{
						mResources.radaCard,
						mResources.quayso,
						mResources.gameInfo,
						mResources.pet,
						mResources.change_flag,
						mResources.change_zone,
						mResources.chat_world,
						mResources.account,
						mResources.option,
						mResources.change_account
					};
				}
			}
			if (IsDelAcc)
			{
				string[] array = new string[Panel.strTool.Length + 1];
				for (int i = 0; i < Panel.strTool.Length; i++)
				{
					array[i] = Panel.strTool[i];
				}
				array[Panel.strTool.Length] = mResources.delacc;
				Panel.strTool = array;
			}
		}

	public void soundToolOption()
		{
			GameCanvas.isPlaySound = !GameCanvas.isPlaySound;
			if (GameCanvas.isPlaySound)
			{
				gI().loadSound(TileMap.mapID);
				Rms.saveRMSInt("isPlaySound", 1);
			}
			else
			{
				gI().closeSound();
				Rms.saveRMSInt("isPlaySound", 0);
			}
			getStrOption();
		}

	public void closeSound()
		{
			Sound.stopAll = true;
			stopAll();
		}

	public void openSound()
		{
			if (Sound.music == null)
			{
				loadSound(0);
			}
			Sound.stopAll = false;
		}

	public void charFall()
		{
			Sound.playSound(MOVE, 0.1f);
			poolCount++;
		}

	public void charJump()
		{
			Sound.playSound(MOVE, 0.2f);
			poolCount++;
		}

	public void panelOpen()
		{
			Sound.playSound(PANEL_OPEN, 0.5f);
			poolCount++;
		}

	public void buttonClose()
		{
			Sound.playSound(BUTTON_CLOSE, 0.5f);
			poolCount++;
		}

	public void buttonClick()
		{
			Sound.playSound(BUTTON_CLICK, 0.5f);
			poolCount++;
		}

	public void charFly()
		{
			Sound.playSound(FLY, 0.2f);
			poolCount++;
		}

	public void stopFly()
		{
		}

	public void openMenu()
		{
			Sound.playSound(BUTTON_CLOSE, 0.5f);
			poolCount++;
		}

	public void panelClick()
		{
			Sound.playSound(PANEL_CLICK, 0.5f);
			poolCount++;
		}

	public void eatPeans()
		{
			Sound.playSound(EAT_PEAN, 0.5f);
			poolCount++;
		}

	public void openDialog()
		{
			Sound.playSound(OPEN_DIALOG, 0.5f);
		}

	public void radarClick()
		{
			Sound.playSound(RADAR_CLICK, 0.5f);
		}

	public static void playSound(int x, int y, int id, float volume)
		{
			Sound.playSound(id, volume);
		}

}
