using System;
using System.Collections.Generic;
using Assets.src.g;
using UnityEngine;
public partial class Panel : IActionListener, IChatable
{
	private void doFireOption()
			{
				if (selected < 0)
				{
					return;
				}
				switch (selected)
				{
				case 0:
					SoundMn.gI().AuraToolOption();
					break;
				case 1:
					SoundMn.gI().AuraToolOption2();
					break;
				case 2:
					SoundMn.gI().soundToolOption();
					break;
				case 3:
					if (Main.isPC)
					{
						GameCanvas.startYesNoDlg(mResources.changeSizeScreen, new Command(mResources.YES, this, 170391, null), new Command(mResources.NO, this, 4005, null));
					}
					else
					{
						SoundMn.gI().CaseSizeScr();
					}
					break;
				case 4:
					if (Main.isPC)
					{
						GameCanvas.startYesNoDlg(mResources.changeSizeScreen, new Command(mResources.YES, this, 170391, null), new Command(mResources.NO, this, 4005, null));
					}
					else
					{
						SoundMn.gI().CaseAnalog();
					}
					break;
				case 5:
					SoundMn.gI().CaseAnalog();
					break;
				}
			}
	private void doFireAccount()
			{
				if (selected < 0)
				{
					return;
				}
				switch (selected)
				{
				case 0:
					GameCanvas.endDlg();
					if (chatTField == null)
					{
						chatTField = new ChatTextField();
						chatTField.tfChat.y = GameCanvas.h - 35 - ChatTextField.gI().tfChat.height;
						chatTField.initChatTextField();
						chatTField.parentScreen = GameCanvas.panel;
					}
					chatTField.tfChat.setText(string.Empty);
					chatTField.strChat = mResources.input_Inventory_Pass;
					chatTField.tfChat.name = mResources.input_Inventory_Pass;
					chatTField.to = string.Empty;
					chatTField.isShow = true;
					chatTField.tfChat.isFocus = true;
					chatTField.tfChat.setIputType(TField.INPUT_TYPE_NUMERIC);
					if (GameCanvas.isTouch)
					{
						chatTField.tfChat.doChangeToTextBox();
					}
					if (!Main.isPC)
					{
						chatTField.startChat2(this, string.Empty);
					}
					if (Main.isWindowsPhone)
					{
						chatTField.tfChat.strInfo = chatTField.strChat;
					}
					break;
				case 1:
					Service.gI().friend(0, -1);
					InfoDlg.showWait();
					break;
				case 2:
					Service.gI().enemy(0, -1);
					InfoDlg.showWait();
					break;
				case 3:
					setTypeMessage();
					if (chatTField == null)
					{
						chatTField = new ChatTextField();
						chatTField.tfChat.y = GameCanvas.h - 35 - ChatTextField.gI().tfChat.height;
						chatTField.initChatTextField();
						chatTField.parentScreen = GameCanvas.panel;
					}
					break;
				case 4:
					if (mResources.language == 2)
					{
						string url = "http://dragonball.indonaga.com/coda/?username=" + GameCanvas.loginScr.tfUser.getText();
						hideNow();
						try
						{
							GameMidlet.instance.platformRequest(url);
							break;
						}
						catch (Exception ex)
						{
							ex.StackTrace.ToString();
							break;
						}
					}
					hideNow();
					if (Char.myCharz().taskMaint.taskId <= 10)
					{
						GameCanvas.startOKDlg(mResources.finishBomong);
					}
					else
					{
						MoneyCharge.gI().switchToMe();
					}
					break;
				case 5:
					setTypeAuto();
					break;
				}
			}
	private bool GetInventorySelect_isbody(int select, int subSelect, Item[] arrItem)
			{
				int num = select - 1 + subSelect * 20;
				return subSelect == 0 && num < arrItem.Length;
			}
	private int GetInventorySelect_body(int select, int subSelect)
			{
				return select - 1 + subSelect * 20;
			}
	private int GetInventorySelect_bag(int select, int subSelect, Item[] arrItem)
			{
				int num = select - 1 + subSelect * 20;
				return num - arrItem.Length;
			}
	private void setNewSelected(int arrLength, bool resetSelect, bool isTabBox)
			{
				int num = arrLength / 20 + ((arrLength % 20 > 0) ? 1 : 0);
				int num2 = xScroll;
				newSelected = (GameCanvas.px - num2) / TAB_W_NEW;
				if (newSelected > num - 1)
				{
					newSelected = num - 1;
				}
				if (GameCanvas.px < num2)
				{
					newSelected = 0;
				}
				if (!isTabBox)
				{
					setTabInventory(resetSelect);
				}
				else
				{
					setTabBox();
				}
			}

}
