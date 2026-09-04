using System;
using System.Collections.Generic;
using Assets.src.g;
using UnityEngine;
public partial class Panel : IActionListener, IChatable
{
	public Panel()
		{
			init();
			cmdClose = new Command(string.Empty, this, 1003, null);
			cmdClose.img = GameCanvas.loadImage("/mainImage/myTexture2dbtX.png");
			cmdClose.cmdClosePanel = true;
			currItem = null;
		}
	public static void loadBg()
		{
			imgMap = GameCanvas.loadImage("/img/map" + TileMap.planetID + ".png");
			imgBantay = GameCanvas.loadImage("/mainImage/myTexture2dbantay.png");
			imgX = GameCanvas.loadImage("/mainImage/myTexture2dbtX.png");
			imgXu = GameCanvas.loadImage("/mainImage/myTexture2dimgMoney.png");
			imgLuong = GameCanvas.loadImage("/mainImage/myTexture2dimgDiamond.png");
			imgLuongKhoa = GameCanvas.loadImage("/mainImage/luongkhoa.png");
			imgUp = GameCanvas.loadImage("/mainImage/myTexture2dup.png");
			imgDown = GameCanvas.loadImage("/mainImage/myTexture2ddown.png");
			imgStar = GameCanvas.loadImage("/mainImage/star.png");
			imgMaxStar = GameCanvas.loadImage("/mainImage/starE.png");
			imgStar8 = GameCanvas.loadImage("/mainImage/star8.png");
			imgStar9 = mSystem.loadImage("/mainImage/star9.png");
			imgStarCuongHoa = mSystem.loadImage("/mainImage/starCH.png");
			imgNew = GameCanvas.loadImage("/mainImage/new.png");
			imgTicket = GameCanvas.loadImage("/mainImage/ticket12.png");
		}
	public void init()
		{
			pX = GameCanvas.pxLast + cmxMap;
			pY = GameCanvas.pyLast + cmyMap;
			lastTabIndex = new int[tabName.Length];
			for (int i = 0; i < lastTabIndex.Length; i++)
			{
				lastTabIndex[i] = -1;
			}
		}
	public void show()
		{
			if (GameCanvas.isTouch)
			{
				cmdClose.x = 156;
				cmdClose.y = 3;
			}
			else
			{
				cmdClose.x = GameCanvas.w - 19;
				cmdClose.y = GameCanvas.h - 19;
			}
			cmdClose.isPlaySoundButton = false;
			ChatPopup.currChatPopup = null;
			InfoDlg.hide();
			timeShow = 20;
			isShow = true;
			isClose = false;
			SoundMn.gI().panelOpen();
			if (isTypeShop())
			{
				Char.myCharz().setPartOld();
			}
		}
	public void moveCamera()
		{
			if (timeShow > 0)
			{
				timeShow--;
			}
			if (justRelease && Equals(GameCanvas.panel) && typeShop == 2 && maxPageShop[currentTabIndex] > 1)
			{
				if (cmy < -50)
				{
					InfoDlg.showWait();
					justRelease = false;
					if (currPageShop[currentTabIndex] <= 0)
					{
						Service.gI().kigui(4, -1, (sbyte)currentTabIndex, maxPageShop[currentTabIndex] - 1, -1);
					}
					else
					{
						Service.gI().kigui(4, -1, (sbyte)currentTabIndex, currPageShop[currentTabIndex] - 1, -1);
					}
				}
				else if (cmy > cmyLim + 50)
				{
					justRelease = false;
					InfoDlg.showWait();
					if (currPageShop[currentTabIndex] >= maxPageShop[currentTabIndex] - 1)
					{
						Service.gI().kigui(4, -1, (sbyte)currentTabIndex, 0, -1);
					}
					else
					{
						Service.gI().kigui(4, -1, (sbyte)currentTabIndex, currPageShop[currentTabIndex] + 1, -1);
					}
				}
			}
			if (cmx != cmtoX && !pointerIsDowning)
			{
				cmvx = cmtoX - cmx << 2;
				cmdx += cmvx;
				cmx += cmdx >> 3;
				cmdx &= 15;
			}
			if (Math.abs(cmtoX - cmx) < 10)
			{
				cmx = cmtoX;
			}
			if (isClose)
			{
				isClose = false;
				cmtoX = wScroll;
			}
			if (cmtoX >= wScroll - 10 && cmx >= wScroll - 10 && position == 0)
			{
				isShow = false;
				cleanCombine();
				if (isChangeZone)
				{
					isChangeZone = false;
					if (Char.myCharz().cHP > 0 && Char.myCharz().statusMe != 14)
					{
						InfoDlg.showWait();
						if (type == 3)
						{
							Service.gI().requestChangeZone(selected, -1);
						}
						else if (type == 14)
						{
							Service.gI().requestMapSelect(selected);
						}
					}
				}
				if (isSelectPlayerMenu)
				{
					isSelectPlayerMenu = false;
					int num = vPlayerMenu.size() - vPlayerMenu_id.size();
					if (Char.myCharz().charFocus != null)
					{
						if (selected - num < 0)
						{
							Char.myCharz().charFocus.menuSelect = selected;
						}
						else
						{
							Char.myCharz().charFocus.menuSelect = short.Parse((string)vPlayerMenu_id.elementAt(selected - num));
						}
					}
					Command command = (Command)vPlayerMenu.elementAt(selected);
					command.performAction();
				}
				vPlayerMenu.removeAllElements();
				vPlayerMenu_id.removeAllElements();
				charMenu = null;
			}
			if (cmRun != 0 && !pointerIsDowning)
			{
				cmtoY += cmRun / 100;
				if (cmtoY < 0)
				{
					cmtoY = 0;
				}
				else if (cmtoY > cmyLim)
				{
					cmtoY = cmyLim;
				}
				else
				{
					cmy = cmtoY;
				}
				cmRun = cmRun * 9 / 10;
				if (cmRun < 100 && cmRun > -100)
				{
					cmRun = 0;
				}
			}
			if (cmy != cmtoY && !pointerIsDowning)
			{
				cmvy = cmtoY - cmy << 2;
				cmdy += cmvy;
				cmy += cmdy >> 4;
				cmdy &= 15;
			}
			cmyLast[currentTabIndex] = cmy;
		}
	public Member getCurrMember()
		{
			if (selected < 2)
			{
				return null;
			}
			if (selected > ((member == null) ? myMember.size() : member.size()) + 1)
			{
				return null;
			}
			return (member == null) ? ((Member)myMember.elementAt(selected - 2)) : ((Member)member.elementAt(selected - 2));
		}
	public ClanMessage getCurrMessage()
		{
			if (selected < 2)
			{
				return null;
			}
			if (selected > ClanMessage.vMessage.size() + 1)
			{
				return null;
			}
			return (ClanMessage)ClanMessage.vMessage.elementAt(selected - 2);
		}
	public Clan getCurrClan()
		{
			if (selected < 2)
			{
				return null;
			}
			if (selected > clans.Length + 1)
			{
				return null;
			}
			return clans[selected - 2];
		}
	public int getCompare(Item item)
		{
			if (item == null)
			{
				return -1;
			}
			if (item.isTypeBody())
			{
				if (item.itemOption == null)
				{
					return -1;
				}
				ItemOption itemOption = item.itemOption[0];
				if (itemOption.optionTemplate.id == 22)
				{
					itemOption.optionTemplate = GameScr.gI().iOptionTemplates[6];
					itemOption.param *= 1000;
				}
				if (itemOption.optionTemplate.id == 23)
				{
					itemOption.optionTemplate = GameScr.gI().iOptionTemplates[7];
					itemOption.param *= 1000;
				}
				Item item2 = null;
				for (int i = 0; i < Char.myCharz().arrItemBody.Length; i++)
				{
					Item item3 = Char.myCharz().arrItemBody[i];
					if (itemOption.optionTemplate.id == 22)
					{
						itemOption.optionTemplate = GameScr.gI().iOptionTemplates[6];
						itemOption.param *= 1000;
					}
					if (itemOption.optionTemplate.id == 23)
					{
						itemOption.optionTemplate = GameScr.gI().iOptionTemplates[7];
						itemOption.param *= 1000;
					}
					if (item3 != null && item3.itemOption != null && item3.template.type == item.template.type)
					{
						item2 = item3;
						break;
					}
				}
				if (item2 == null)
				{
					isUp = true;
					return itemOption.param;
				}
				int num = 0;
				num = ((item2 == null || item2.itemOption == null) ? itemOption.param : (itemOption.param - item2.itemOption[0].param));
				if (num < 0)
				{
					isUp = false;
				}
				else
				{
					isUp = true;
				}
				return num;
			}
			return 0;
		}
	private string getStatus(int status)
		{
			return status switch
			{
				0 => mResources.follow, 
				1 => mResources.defend, 
				2 => mResources.attack, 
				3 => mResources.gohome, 
				_ => "aaa", 
			};
		}
	public void hideNow()
		{
			if (timeShow > 0)
			{
				isClose = false;
				return;
			}
			cp = null;
			if (isTypeShop() || TileMap.mapID == 45)
			{
				Char.myCharz().resetPartTemp();
			}
			if (chatTField != null && type == 13 && chatTField.isShow)
			{
				chatTField = null;
			}
			if (type == 13 && !isAccept)
			{
				Service.gI().giaodich(3, -1, -1, -1);
			}
			Res.outz("HIDE PANELLLLLLLLLLLLLLLLLLLLLL");
			SoundMn.gI().buttonClose();
			GameScr.isPaint = true;
			TileMap.lastPlanetId = -1;
			imgMap = null;
			mSystem.gcc();
			isClanOption = false;
			isClose = true;
			cleanCombine();
			Hint.clickNpc();
			GameCanvas.panel2 = null;
			GameCanvas.clearAllPointerEvent();
			GameCanvas.clearKeyPressed();
			pointerDownTime = (pointerDownFirstX = 0);
			pointerIsDowning = false;
			isShow = false;
			if ((Char.myCharz().cHP <= 0 || Char.myCharz().statusMe == 14 || Char.myCharz().statusMe == 5) && Char.myCharz().meDead)
			{
				Command center = new Command(mResources.DIES[0], 11038, GameScr.gI());
				GameScr.gI().center = center;
				Char.myCharz().cHP = 0L;
			}
		}

}
