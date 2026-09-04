using System;
using Assets.src.g;
public partial class GameScr : mScreen, IChatable
{
	public bool isMeCanAttackMob(Mob m)
		{
			if (m == null)
			{
				return false;
			}
			if (Char.myCharz().cTypePk == 5)
			{
				return true;
			}
			if (Char.myCharz().isAttacPlayerStatus() && !m.isMobMe)
			{
				return false;
			}
			if (Char.myCharz().mobMe != null && m.Equals(Char.myCharz().mobMe))
			{
				return false;
			}
			Char @char = findCharInMap(m.mobId);
			if (@char == null)
			{
				return true;
			}
			if (@char.cTypePk == 5)
			{
				return true;
			}
			if (Char.myCharz().isMeCanAttackOtherPlayer(@char))
			{
				return true;
			}
			return false;
		}
	public void resetButton()
		{
			if (!ModMenu.modMenuOpen)
			{
				GameCanvas.menu.showMenu = false;
			}
			ChatTextField.gI().close();
			ChatTextField.gI().center = null;
			isLockKey = false;
			typeTrade = 0;
			indexMenu = 0;
			indexSelect = 0;
			indexItemUse = -1;
			indexRow = -1;
			indexRowMax = 0;
			indexTitle = 0;
			typeTrade = (typeTradeOrder = 0);
			mSystem.endKey();
			if (Char.myCharz().cHP <= 0 || Char.myCharz().statusMe == 14 || Char.myCharz().statusMe == 5)
			{
				if (Char.myCharz().meDead)
				{
					cmdDead = new Command(mResources.DIES[0], 11038);
					center = cmdDead;
					Char.myCharz().cHP = 0L;
				}
				isHaveSelectSkill = false;
			}
			else
			{
				isHaveSelectSkill = true;
			}
			scrMain.clear();
		}
	public bool isVsMap()
		{
			return true;
		}
	private void checkDrag()
		{
			if (isAnalog == 1 || gamePad.disableCheckDrag())
			{
				return;
			}
			Char.myCharz().cmtoChar = true;
			if (isUseTouch)
			{
				return;
			}
			if (GameCanvas.isPointerJustDown)
			{
				GameCanvas.isPointerJustDown = false;
				isPointerDowning = true;
				ptDownTime = 0;
				ptLastDownX = (ptFirstDownX = GameCanvas.px);
				ptLastDownY = (ptFirstDownY = GameCanvas.py);
			}
			if (isPointerDowning)
			{
				int num = GameCanvas.px - ptLastDownX;
				int num2 = GameCanvas.py - ptLastDownY;
				if (!isChangingCameraMode && (Res.abs(GameCanvas.px - ptFirstDownX) > 15 || Res.abs(GameCanvas.py - ptFirstDownY) > 15))
				{
					isChangingCameraMode = true;
				}
				ptLastDownX = GameCanvas.px;
				ptLastDownY = GameCanvas.py;
				ptDownTime++;
				if (isChangingCameraMode)
				{
					Char.myCharz().cmtoChar = false;
					cmx -= num;
					cmy -= num2;
					if (cmx < 24)
					{
						int num3 = (24 - cmx) / 3;
						if (num3 != 0)
						{
							cmx += num - num / num3;
						}
					}
					if (cmx < (isVsMap() ? 24 : 0))
					{
						cmx = (isVsMap() ? 24 : 0);
					}
					if (cmx > cmxLim)
					{
						int num4 = (cmx - cmxLim) / 3;
						if (num4 != 0)
						{
							cmx += num - num / num4;
						}
					}
					if (cmx > cmxLim + ((!isVsMap()) ? 24 : 0))
					{
						cmx = cmxLim + ((!isVsMap()) ? 24 : 0);
					}
					if (cmy < 0)
					{
						int num5 = -cmy / 3;
						if (num5 != 0)
						{
							cmy += num2 - num2 / num5;
						}
					}
					if (cmy < -((!isVsMap()) ? 24 : 0))
					{
						cmy = -((!isVsMap()) ? 24 : 0);
					}
					if (cmy > cmyLim)
					{
						cmy = cmyLim;
					}
					cmtoX = cmx;
					cmtoY = cmy;
				}
			}
			if (isPointerDowning && GameCanvas.isPointerJustRelease)
			{
				isPointerDowning = false;
				isChangingCameraMode = false;
				if (Res.abs(GameCanvas.px - ptFirstDownX) > 15 || Res.abs(GameCanvas.py - ptFirstDownY) > 15)
				{
					GameCanvas.isPointerJustRelease = false;
				}
			}
		}
	private bool inRectangle(int xClick, int yClick, int x, int y, int w, int h)
		{
			return xClick >= x && xClick <= x + w && yClick >= y && yClick <= y + h;
		}
	private void checkAuto()
		{
			long num = mSystem.currentTimeMillis();
			if (GameCanvas.keyPressed[(!Main.isPC) ? 2 : 21] || GameCanvas.keyPressed[(!Main.isPC) ? 4 : 23] || GameCanvas.keyPressed[(!Main.isPC) ? 6 : 24] || GameCanvas.keyPressed[1] || GameCanvas.keyPressed[3])
			{
				auto = 0;
				isAutoPlay = false;
			}
			if (GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25] && !isPaintPopup())
			{
				if (auto == 0)
				{
					if (num - lastFire < 800 && checkSkillValid2() && (Char.myCharz().mobFocus != null || (Char.myCharz().charFocus != null && Char.myCharz().isMeCanAttackOtherPlayer(Char.myCharz().charFocus))))
					{
						Res.outz("toi day");
						auto = 10;
						GameCanvas.keyPressed[(!Main.isPC) ? 5 : 25] = false;
					}
				}
				else
				{
					auto = 0;
					GameCanvas.keyPressed[(!Main.isPC) ? 4 : 23] = (GameCanvas.keyPressed[(!Main.isPC) ? 6 : 24] = false);
				}
				lastFire = num;
			}
			if (GameCanvas.gameTick % 5 == 0 && auto > 0 && Char.myCharz().currentMovePoint == null)
			{
				if (Char.myCharz().myskill != null && (Char.myCharz().myskill.template.isUseAlone() || Char.myCharz().myskill.paintCanNotUseSkill))
				{
					return;
				}
				if ((Char.myCharz().mobFocus != null && Char.myCharz().mobFocus.status != 1 && Char.myCharz().mobFocus.status != 0 && Char.myCharz().charFocus == null) || (Char.myCharz().charFocus != null && Char.myCharz().isMeCanAttackOtherPlayer(Char.myCharz().charFocus)))
				{
					if (Char.myCharz().myskill.paintCanNotUseSkill)
					{
						return;
					}
					doFire(isFireByShortCut: false, skipWaypoint: true);
				}
			}
			if (auto > 1)
			{
				auto--;
			}
		}
	public void doUseHP()
		{
			if (Char.myCharz().stone || Char.myCharz().blindEff || Char.myCharz().holdEffID > 0)
			{
				return;
			}
			long num = mSystem.currentTimeMillis();
			if (num - lastUsePotion >= 10000)
			{
				if (!Char.myCharz().doUsePotion())
				{
					info1.addInfo(mResources.HP_EMPTY, 0);
					return;
				}
				ServerEffect.addServerEffect(11, Char.myCharz(), 5);
				ServerEffect.addServerEffect(104, Char.myCharz(), 4);
				lastUsePotion = num;
				SoundMn.gI().eatPeans();
			}
		}
	public void activeSuperPower(int x, int y)
		{
			if (!isSuperPower)
			{
				SoundMn.gI().bigeExlode();
				isSuperPower = true;
				tPower = 0;
				dxPower = 0;
				xPower = x - cmx;
				yPower = y - cmy;
			}
		}
	public void doiMauTroi()
		{
			isRongThanXuatHien = true;
			mautroi = mGraphics.blendColor(0.4f, 0, GameCanvas.colorTop[GameCanvas.colorTop.Length - 1]);
		}
	public void callRongThan(int x, int y)
		{
			Res.outz("VE RONG THAN O VI TRI x= " + x + " y=" + y);
			doiMauTroi();
			Effect me = new Effect((!isRongNamek) ? 17 : 25, x, y - 77, 2, -1, 1);
			EffecMn.addEff(me);
		}
	public void hideRongThan()
		{
			isRongThanXuatHien = false;
			EffecMn.removeEff(17);
			if (isRongNamek)
			{
				isRongNamek = false;
				EffecMn.removeEff(25);
			}
		}
	private void autoPlay()
		{
			if (timeSkill > 0)
			{
				timeSkill--;
			}
			if (!canAutoPlay || isChangeZone || Char.myCharz().statusMe == 14 || Char.myCharz().statusMe == 5 || Char.myCharz().isCharge || Char.myCharz().isFlyAndCharge || Char.myCharz().isUseChargeSkill())
			{
				return;
			}
			bool flag = false;
			for (int i = 0; i < vMob.size(); i++)
			{
				Mob mob = (Mob)vMob.elementAt(i);
				if (mob.status != 0 && mob.status != 1)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				return;
			}
			bool flag2 = false;
			for (int j = 0; j < Char.myCharz().arrItemBag.Length; j++)
			{
				Item item = Char.myCharz().arrItemBag[j];
				if (item != null && item.template.type == 6)
				{
					flag2 = true;
					break;
				}
			}
			if (!flag2 && GameCanvas.gameTick % 150 == 0)
			{
				Service.gI().requestPean();
			}
			if (Char.myCharz().cHP <= Char.myCharz().cHPFull * 20 / 100 || Char.myCharz().cMP <= Char.myCharz().cMPFull * 20 / 100)
			{
				doUseHP();
			}
			if (Char.myCharz().mobFocus == null || (Char.myCharz().mobFocus != null && Char.myCharz().mobFocus.isMobMe))
			{
				for (int k = 0; k < vMob.size(); k++)
				{
					Mob mob2 = (Mob)vMob.elementAt(k);
					if (mob2.status != 0 && mob2.status != 1 && mob2.hp > 0 && !mob2.isMobMe)
					{
						Char.myCharz().cx = mob2.x;
						Char.myCharz().cy = mob2.y;
						Char.myCharz().mobFocus = mob2;
						Service.gI().charMove();
						break;
					}
				}
			}
			else if (Char.myCharz().mobFocus.hp <= 0 || Char.myCharz().mobFocus.status == 1 || Char.myCharz().mobFocus.status == 0)
			{
				Char.myCharz().mobFocus = null;
			}
			if (Char.myCharz().mobFocus == null || timeSkill != 0 || (Char.myCharz().skillInfoPaint() != null && Char.myCharz().indexSkill < Char.myCharz().skillInfoPaint().Length && Char.myCharz().dart != null && Char.myCharz().arr != null))
			{
				return;
			}
			Skill skill = null;
			if (GameCanvas.isTouch)
			{
				for (int l = 0; l < onScreenSkill.Length; l++)
				{
					if (onScreenSkill[l] == null || onScreenSkill[l].paintCanNotUseSkill || onScreenSkill[l].template.id == 10 || onScreenSkill[l].template.id == 11 || onScreenSkill[l].template.id == 14 || onScreenSkill[l].template.id == 23 || onScreenSkill[l].template.id == 7 || Char.myCharz().skillInfoPaint() != null || onScreenSkill[l].template.isSkillSpec())
					{
						continue;
					}
					long num = 0L;
					num = ((onScreenSkill[l].template.manaUseType == 2) ? 1 : ((onScreenSkill[l].template.manaUseType == 1) ? (onScreenSkill[l].manaUse * Char.myCharz().cMPFull / 100) : onScreenSkill[l].manaUse));
					if (Char.myCharz().cMP >= num)
					{
						if (skill == null)
						{
							skill = onScreenSkill[l];
						}
						else if (skill.coolDown < onScreenSkill[l].coolDown)
						{
							skill = onScreenSkill[l];
						}
					}
				}
				if (skill != null)
				{
					doSelectSkill(skill, isShortcut: true);
					doDoubleClickToObj(Char.myCharz().mobFocus);
				}
				return;
			}
			for (int m = 0; m < keySkill.Length; m++)
			{
				if (keySkill[m] == null || keySkill[m].paintCanNotUseSkill || keySkill[m].template.id == 10 || keySkill[m].template.id == 11 || keySkill[m].template.id == 14 || keySkill[m].template.id == 23 || keySkill[m].template.id == 7 || Char.myCharz().skillInfoPaint() != null)
				{
					continue;
				}
				long num2 = 0L;
				num2 = ((keySkill[m].template.manaUseType == 2) ? 1 : ((keySkill[m].template.manaUseType == 1) ? (keySkill[m].manaUse * Char.myCharz().cMPFull / 100) : keySkill[m].manaUse));
				if (Char.myCharz().cMP >= num2)
				{
					if (skill == null)
					{
						skill = keySkill[m];
					}
					else if (skill.coolDown < keySkill[m].coolDown)
					{
						skill = keySkill[m];
					}
				}
			}
			if (skill != null)
			{
				doSelectSkill(skill, isShortcut: true);
				doDoubleClickToObj(Char.myCharz().mobFocus);
			}
		}
	private void askToPick()
		{
			Npc npc = new Npc(5, 0, -100, 100, 5, info1.charId[Char.myCharz().cgender][2]);
			string nhatvatpham = mResources.nhatvatpham;
			string[] menu = new string[2]
			{
				mResources.YES,
				mResources.NO
			};
			npc.idItem = 673;
			gI().createMenu(menu, npc);
			ChatPopup.addChatPopupWithIcon(nhatvatpham, 100000, npc, 5820);
		}

}
