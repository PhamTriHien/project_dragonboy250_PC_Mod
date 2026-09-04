using System;
using UnityEngine;
public partial class ServerListScreen : mScreen, IActionListener
{
	public static void LoadRMS_ExtraLink()
		{
			sbyte[] array = Rms.loadRMS(RMS_NR_Extralink);
			if (array == null)
			{
				Controller.isEXTRA_LINK = false;
				return;
			}
			DataInputStream dataInputStream = new DataInputStream(array);
			if (dataInputStream == null)
			{
				return;
			}
			try
			{
				sbyte b = dataInputStream.readByte();
				typeClass = new sbyte[b];
				listChar = new Char[b];
				for (int i = 0; i < b; i++)
				{
					typeClass[i] = dataInputStream.readByte();
					if (typeClass[i] > -1)
					{
						isHaveChar = true;
						listChar[i] = new Char();
						listChar[i].cgender = typeClass[i];
						listChar[i].head = dataInputStream.readShort();
						listChar[i].body = dataInputStream.readShort();
						listChar[i].leg = dataInputStream.readShort();
						listChar[i].bag = dataInputStream.readShort();
						listChar[i].cName = dataInputStream.readUTF();
					}
				}
				dataInputStream.close();
				Controller.isEXTRA_LINK = true;
			}
			catch (Exception)
			{
			}
		}
	public static void saveRMS_ExtraLink()
		{
			if (typeClass == null)
			{
				return;
			}
			DataOutputStream dataOutputStream = new DataOutputStream();
			try
			{
				dataOutputStream.writeByte((sbyte)typeClass.Length);
				for (int i = 0; i < typeClass.Length; i++)
				{
					dataOutputStream.writeByte(typeClass[i]);
					if (typeClass[i] > -1 && listChar != null && listChar[i] != null)
					{
						dataOutputStream.writeShort((short)listChar[i].head);
						dataOutputStream.writeShort((short)listChar[i].body);
						dataOutputStream.writeShort((short)listChar[i].leg);
						dataOutputStream.writeShort((short)listChar[i].bag);
						dataOutputStream.writeUTF(listChar[i].cName);
					}
				}
				Rms.saveRMS(RMS_NR_Extralink, dataOutputStream.toByteArray());
				dataOutputStream.close();
				SplashScr.loadIP();
			}
			catch (Exception)
			{
			}
		}
	public void Set_UI_New()
		{
			if (!GameCanvas.isTouch)
			{
				return;
			}
			isNewUI = true;
			cmd_New_Ui = new Command[2];
			int num = GameCanvas.hh - 15 * cmd_New_Ui.Length + 28;
			for (int i = 0; i < cmd_New_Ui.Length; i++)
			{
				switch (i)
				{
				case 0:
					cmd_New_Ui[0] = new Command(string.Empty, this, 3, null);
					cmd_New_Ui[0].caption = mResources.playNew;
					if (Rms.loadRMS(Rms.RMS_userAo + ipSelect) != null)
					{
						cmd_New_Ui[0].caption = mResources.choitiep;
					}
					break;
				case 1:
					cmd_New_Ui[1] = new Command(mResources.change_account, this, 7, null);
					break;
				}
				cmd_New_Ui[i].y = num;
				cmd_New_Ui[i].setType();
				cmd_New_Ui[i].x = (GameCanvas.w - cmd_New_Ui[i].w) / 2;
				num += 30;
			}
		}
	public static void CheckBack_ServerListScreen()
		{
			if (GameCanvas.serverScreen == null)
			{
				GameCanvas.serverScreen = new ServerListScreen();
			}
			bool flag = false;
			bool flag2 = false;
			try
			{
				if (!Rms.loadRMSString(Rms.RMS_acc).Equals(string.Empty))
				{
					flag = true;
				}
				if (!Rms.loadRMSString(Rms.RMS_userAo + ipSelect).Equals(string.Empty))
				{
					flag2 = true;
				}
			}
			catch (Exception)
			{
			}
			Debug.LogError(">>>>CheckBack_ServerListScreen: " + ipSelect + "  auto login:" + isAutoLogin);
			if (ipSelect == -1 || !isAutoLogin)
			{
				GameCanvas.serverScreen.switchToMe();
				return;
			}
			if (!flag && !flag2)
			{
				GameCanvas.serverScreen.switchToMe();
				return;
			}
			Controller.isEXTRA_LINK = false;
			GameCanvas.serverScreen.switchToMe();
			GameCanvas.serverScreen.Login_New();
		}

}
