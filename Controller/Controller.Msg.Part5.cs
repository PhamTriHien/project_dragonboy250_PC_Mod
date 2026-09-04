using System;
using Assets.src.e;
using Assets.src.f;
using Assets.src.g;
using UnityEngine;

public partial class Controller : IMessageHandler
{
	public bool onMessage_Part5(Message msg)
	{
		Char @char = null;
		Mob mob = null;
		MyVector myVector = new MyVector();
		int num = 0;
		switch (msg.command)
		{
				case -29:
					messageNotLogin(msg);
					break;
				case -28:
					messageNotMap(msg);
					break;
				case -30:
					messageSubCommand(msg);
					break;
				case 62:
					GameCanvas.debug("SZ3", 2);
					@char = GameScr.findCharInMap(msg.reader().readInt());
					if (@char != null)
					{
						@char.killCharId = Char.myCharz().charID;
						Char.myCharz().npcFocus = null;
						Char.myCharz().mobFocus = null;
						Char.myCharz().itemFocus = null;
						Char.myCharz().charFocus = @char;
						Char.isManualFocus = true;
						GameScr.info1.addInfo(@char.cName + mResources.CUU_SAT, 0);
					}
					break;
				case 63:
					GameCanvas.debug("SZ4", 2);
					Char.myCharz().killCharId = msg.reader().readInt();
					Char.myCharz().npcFocus = null;
					Char.myCharz().mobFocus = null;
					Char.myCharz().itemFocus = null;
					Char.myCharz().charFocus = GameScr.findCharInMap(Char.myCharz().killCharId);
					Char.isManualFocus = true;
					break;
				case 64:
					GameCanvas.debug("SZ5", 2);
					@char = Char.myCharz();
					try
					{
						@char = GameScr.findCharInMap(msg.reader().readInt());
					}
					catch (Exception ex2)
					{
						Cout.println("Loi CLEAR_CUU_SAT " + ex2.ToString());
					}
					@char.killCharId = -9999;
					break;
				case 39:
					GameCanvas.debug("SA49", 2);
					GameScr.gI().typeTradeOrder = 2;
					if (GameScr.gI().typeTrade >= 2 && GameScr.gI().typeTradeOrder >= 2)
					{
						InfoDlg.showWait();
					}
					break;
				case 57:
				{
					GameCanvas.debug("SZ6", 2);
					MyVector myVector2 = new MyVector();
					myVector2.addElement(new Command(msg.reader().readUTF(), GameCanvas.instance, 88817, null));
					GameCanvas.menu.startAt(myVector2, 3);
					break;
				}
				case 58:
				{
					GameCanvas.debug("SZ7", 2);
					int num21 = msg.reader().readInt();
					Char char11 = ((num21 != Char.myCharz().charID) ? GameScr.findCharInMap(num21) : Char.myCharz());
					char11.moveFast = new short[3];
					char11.moveFast[0] = 0;
					short num167 = msg.reader().readShort();
					short num168 = msg.reader().readShort();
					char11.moveFast[1] = num167;
					char11.moveFast[2] = num168;
					try
					{
						num21 = msg.reader().readInt();
						Char char12 = ((num21 != Char.myCharz().charID) ? GameScr.findCharInMap(num21) : Char.myCharz());
						char12.cx = num167;
						char12.cy = num168;
					}
					catch (Exception ex26)
					{
						Cout.println("Loi MOVE_FAST " + ex26.ToString());
					}
					break;
				}
				case 88:
				{
					string info4 = msg.reader().readUTF();
					short num166 = msg.reader().readShort();
					GameCanvas.inputDlg.show(info4, new Command(mResources.ACCEPT, GameCanvas.instance, 88818, num166), TField.INPUT_TYPE_ANY);
					break;
				}
				case 27:
				{
					myVector = new MyVector();
					string text8 = msg.reader().readUTF();
					int num157 = msg.reader().readByte();
					for (int num158 = 0; num158 < num157; num158++)
					{
						string caption4 = msg.reader().readUTF();
						short num159 = msg.reader().readShort();
						myVector.addElement(new Command(caption4, GameCanvas.instance, 88819, num159));
					}
					GameCanvas.menu.startWithoutCloseButton(myVector, 3);
					break;
				}
				case 33:
				{
					GameCanvas.debug("SA51", 2);
					InfoDlg.hide();
					GameCanvas.clearKeyHold();
					GameCanvas.clearKeyPressed();
					myVector = new MyVector();
					try
					{
						while (true)
						{
							string caption3 = msg.reader().readUTF();
							myVector.addElement(new Command(caption3, GameCanvas.instance, 88822, null));
						}
					}
					catch (Exception ex23)
					{
						Cout.println("Loi OPEN_UI_MENU " + ex23.ToString());
					}
					if (Char.myCharz().npcFocus == null)
					{
						return true;
					}
					for (int num153 = 0; num153 < Char.myCharz().npcFocus.template.menu.Length; num153++)
					{
						string[] array16 = Char.myCharz().npcFocus.template.menu[num153];
						myVector.addElement(new Command(array16[0], GameCanvas.instance, 88820, array16));
					}
					GameCanvas.menu.startAt(myVector, 3);
					break;
				}
				case 40:
				{
					GameCanvas.debug("SA52", 2);
					GameCanvas.taskTick = 150;
					short taskId = msg.reader().readShort();
					sbyte index2 = msg.reader().readByte();
					string str3 = msg.reader().readUTF();
					str3 = Res.changeString(str3);
					string str4 = msg.reader().readUTF();
					str4 = Res.changeString(str4);
					string[] array12 = new string[msg.reader().readByte()];
					string[] array13 = new string[array12.Length];
					GameScr.tasks = new int[array12.Length];
					GameScr.mapTasks = new int[array12.Length];
					short[] array14 = new short[array12.Length];
					short num141 = -1;
					for (int num142 = 0; num142 < array12.Length; num142++)
					{
						string str5 = msg.reader().readUTF();
						str5 = Res.changeString(str5);
						GameScr.tasks[num142] = msg.reader().readByte();
						GameScr.mapTasks[num142] = msg.reader().readShort();
						string str6 = msg.reader().readUTF();
						str6 = Res.changeString(str6);
						array14[num142] = -1;
						array12[num142] = str5;
						if (!str6.Equals(string.Empty))
						{
							array13[num142] = str6;
						}
					}
					try
					{
						num141 = msg.reader().readShort();
						Cout.println(" TASK_GET count:" + num141);
						for (int num143 = 0; num143 < array12.Length; num143++)
						{
							array14[num143] = msg.reader().readShort();
							Cout.println(num143 + " i TASK_GET   counts[i]:" + array14[num143]);
						}
					}
					catch (Exception ex20)
					{
						Cout.println("Loi TASK_GET " + ex20.ToString());
					}
					Char.myCharz().taskMaint = new Task(taskId, index2, str3, str4, array12, array14, num141, array13);
					if (Char.myCharz().npcFocus != null)
					{
						Npc.clearEffTask();
					}
					Char.taskAction(isNextStep: true);
					break;
				}
				case 41:
					GameCanvas.debug("SA53", 2);
					GameCanvas.taskTick = 100;
					Res.outz("TASK NEXT");
					Char.myCharz().taskMaint.index++;
					Char.myCharz().taskMaint.count = 0;
					Npc.clearEffTask();
					Char.taskAction(isNextStep: true);
					break;
				case 50:
				{
					sbyte b57 = msg.reader().readByte();
					Panel.vGameInfo.removeAllElements();
					for (int num138 = 0; num138 < b57; num138++)
					{
						GameInfo gameInfo = new GameInfo();
						gameInfo.id = msg.reader().readShort();
						gameInfo.main = msg.reader().readUTF();
						gameInfo.content = msg.reader().readUTF();
						Panel.vGameInfo.addElement(gameInfo);
						bool hasRead = Rms.loadRMSInt(gameInfo.id + string.Empty) != -1;
						gameInfo.hasRead = hasRead;
					}
					break;
				}
				case 43:
					GameCanvas.taskTick = 50;
					GameCanvas.debug("SA55", 2);
					Char.myCharz().taskMaint.count = msg.reader().readShort();
					if (Char.myCharz().npcFocus != null)
					{
						Npc.clearEffTask();
					}
					try
					{
						short x_hint = msg.reader().readShort();
						short y_hint = msg.reader().readShort();
						Char.myCharz().x_hint = x_hint;
						Char.myCharz().y_hint = y_hint;
					}
					catch (Exception)
					{
					}
					break;
				case 90:
					GameCanvas.debug("SA577", 2);
					requestItemPlayer(msg);
					break;
				case 29:
					GameCanvas.debug("SA58", 2);
					GameScr.gI().openUIZone(msg);
					break;
				case -21:
				{
					GameCanvas.debug("SA60", 2);
					short itemMapID = msg.reader().readShort();
					for (int num133 = 0; num133 < GameScr.vItemMap.size(); num133++)
					{
						if (((ItemMap)GameScr.vItemMap.elementAt(num133)).itemMapID == itemMapID)
						{
							GameScr.vItemMap.removeElementAt(num133);
							break;
						}
					}
					break;
				}
			default:
				return false;
		}
		return true;
	}

}
