using System;
using Assets.src.g;
public partial class GameScr : mScreen, IChatable
{
	public bool isAttack()
		{
			if (checkClickToBotton(Char.myCharz().charFocus))
			{
				return false;
			}
			if (checkClickToBotton(Char.myCharz().mobFocus))
			{
				return false;
			}
			if (checkClickToBotton(Char.myCharz().npcFocus))
			{
				return false;
			}
			if (ChatTextField.gI().isShow)
			{
				return false;
			}
			if (InfoDlg.isLock || Char.myCharz().isLockAttack || Char.isLockKey)
			{
				return false;
			}
			if (Char.myCharz().myskill != null && Char.myCharz().myskill.template.id == 6 && Char.myCharz().itemFocus != null)
			{
				pickItem();
				return false;
			}
			if (Char.myCharz().myskill != null && Char.myCharz().myskill.template.type == 2 && Char.myCharz().npcFocus == null && Char.myCharz().myskill.template.id != 6)
			{
				if (!checkSkillValid())
				{
					return false;
				}
				return true;
			}
			if (Char.myCharz().skillPaint != null || (Char.myCharz().mobFocus == null && Char.myCharz().npcFocus == null && Char.myCharz().charFocus == null && Char.myCharz().itemFocus == null))
			{
				return false;
			}
			if (Char.myCharz().mobFocus != null)
			{
				if (Char.myCharz().mobFocus.isBigBoss() && Char.myCharz().mobFocus.status == 4)
				{
					Char.myCharz().mobFocus = null;
					Char.myCharz().currentMovePoint = null;
				}
				isAutoPlay = true;
				if (!isMeCanAttackMob(Char.myCharz().mobFocus))
				{
					Res.outz("can not attack");
					return false;
				}
				if (mobCapcha != null)
				{
					return false;
				}
				if (Char.myCharz().myskill == null)
				{
					return false;
				}
				if (Char.myCharz().isSelectingSkillUseAlone())
				{
					return false;
				}
				int num = -1;
				int num2 = Res.abs(Char.myCharz().cx - cmx) * mGraphics.zoomLevel;
				if (Char.myCharz().charFocus != null)
				{
					num = Res.abs(Char.myCharz().cx - Char.myCharz().charFocus.cx) * mGraphics.zoomLevel;
				}
				else if (Char.myCharz().mobFocus != null)
				{
					num = Res.abs(Char.myCharz().cx - Char.myCharz().mobFocus.x) * mGraphics.zoomLevel;
				}
				if ((Char.myCharz().mobFocus.status == 1 || Char.myCharz().mobFocus.status == 0 || Char.myCharz().myskill.template.type == 4 || num == -1 || num > num2) && Char.myCharz().myskill.template.type == 4)
				{
					if (Char.myCharz().mobFocus.x < Char.myCharz().cx)
					{
						Char.myCharz().cdir = -1;
					}
					else
					{
						Char.myCharz().cdir = 1;
					}
					doSelectSkill(Char.myCharz().myskill, isShortcut: true);
				}
				if (!checkSkillValid())
				{
					return false;
				}
				if (Char.myCharz().cx < Char.myCharz().mobFocus.getX())
				{
					Char.myCharz().cdir = 1;
				}
				else
				{
					Char.myCharz().cdir = -1;
				}
				int num3 = Math.abs(Char.myCharz().cx - Char.myCharz().mobFocus.getX());
				int num4 = Math.abs(Char.myCharz().cy - Char.myCharz().mobFocus.getY());
				Char.myCharz().cvx = 0;
				if (num3 <= Char.myCharz().myskill.dx && num4 <= Char.myCharz().myskill.dy)
				{
					if (Char.myCharz().myskill.template.id == 20)
					{
						return true;
					}
					if (num4 > num3 && Res.abs(Char.myCharz().cy - Char.myCharz().mobFocus.getY()) > 30 && Char.myCharz().mobFocus.getTemplate().type == 4)
					{
						Char.myCharz().currentMovePoint = new MovePoint(Char.myCharz().cx + Char.myCharz().cdir, Char.myCharz().mobFocus.getY());
						Char.myCharz().endMovePointCommand = new Command(null, null, 8002, null);
						GameCanvas.clearKeyHold();
						GameCanvas.clearKeyPressed();
						return false;
					}
					int num5 = 20;
					bool flag = false;
					if (Char.myCharz().mobFocus is BigBoss || Char.myCharz().mobFocus is BigBoss2)
					{
						flag = true;
					}
					if (Char.myCharz().myskill.dx > 100)
					{
						num5 = 60;
						if (num3 < 20)
						{
							Char.myCharz().createShadow(Char.myCharz().cx, Char.myCharz().cy, 10);
						}
					}
					bool flag2 = false;
					if ((TileMap.tileTypeAtPixel(Char.myCharz().cx, Char.myCharz().cy + 3) & 2) == 2)
					{
						int num6 = ((Char.myCharz().cx > Char.myCharz().mobFocus.getX()) ? 1 : (-1));
						if ((TileMap.tileTypeAtPixel(Char.myCharz().mobFocus.getX() + num5 * num6, Char.myCharz().cy + 3) & 2) != 2)
						{
							flag2 = true;
						}
					}
					if (num3 <= num5 && !flag2)
					{
						if (Char.myCharz().cx > Char.myCharz().mobFocus.getX())
						{
							int num7 = Char.myCharz().mobFocus.getX() + num5 + (flag ? 30 : 0);
							int i = Char.myCharz().mobFocus.getX();
							bool flag3 = false;
							for (; i < num7; i += 24)
							{
								if (TileMap.tileTypeAtPixel(i, Char.myCharz().cy + 3) == 8 || TileMap.tileTypeAtPixel(i, Char.myCharz().cy + 3) == 4)
								{
									flag3 = true;
									break;
								}
							}
							if (flag3)
							{
								Char.myCharz().cx = i - 24;
							}
							else
							{
								Char.myCharz().cx = num7;
							}
							Char.myCharz().cdir = -1;
						}
						else
						{
							int num8 = Char.myCharz().mobFocus.getX() - num5 - (flag ? 30 : 0);
							int num9 = Char.myCharz().mobFocus.getX();
							bool flag4 = false;
							while (num9 > num8)
							{
								if (TileMap.tileTypeAtPixel(num9, Char.myCharz().cy + 3) == 8 || TileMap.tileTypeAtPixel(num9, Char.myCharz().cy + 3) == 4)
								{
									flag4 = true;
									break;
								}
								num9 -= 24;
							}
							if (flag4)
							{
								Char.myCharz().cx = num9 + 24;
							}
							else
							{
								Char.myCharz().cx = num8;
							}
							Char.myCharz().cdir = 1;
						}
						Service.gI().charMove();
					}
					GameCanvas.clearKeyHold();
					GameCanvas.clearKeyPressed();
					return true;
				}
				bool flag5 = false;
				if (Char.myCharz().mobFocus is BigBoss || Char.myCharz().mobFocus is BigBoss2)
				{
					flag5 = true;
				}
				int num10 = (Char.myCharz().myskill.dx - ((!flag5) ? 20 : 50)) * ((Char.myCharz().cx > Char.myCharz().mobFocus.getX()) ? 1 : (-1));
				if (num3 <= Char.myCharz().myskill.dx)
				{
					num10 = 0;
				}
				Char.myCharz().currentMovePoint = new MovePoint(Char.myCharz().mobFocus.getX() + num10, Char.myCharz().mobFocus.getY());
				Char.myCharz().endMovePointCommand = new Command(null, null, 8002, null);
				GameCanvas.clearKeyHold();
				GameCanvas.clearKeyPressed();
				return false;
			}
			if (Char.myCharz().npcFocus != null)
			{
				if (Char.myCharz().npcFocus.isHide)
				{
					return false;
				}
				if (Char.myCharz().cx < Char.myCharz().npcFocus.cx)
				{
					Char.myCharz().cdir = 1;
				}
				else
				{
					Char.myCharz().cdir = -1;
				}
				if (Char.myCharz().cx < Char.myCharz().npcFocus.cx)
				{
					Char.myCharz().npcFocus.cdir = -1;
				}
				else
				{
					Char.myCharz().npcFocus.cdir = 1;
				}
				int num11 = Math.abs(Char.myCharz().cx - Char.myCharz().npcFocus.cx);
				int num12 = Math.abs(Char.myCharz().cy - Char.myCharz().npcFocus.cy);
				if (num12 > 40)
				{
					Char.myCharz().cy = Char.myCharz().npcFocus.cy - 40;
				}
				if (num11 < 60)
				{
					GameCanvas.clearKeyHold();
					GameCanvas.clearKeyPressed();
					if (tMenuDelay == 0)
					{
						if (Char.myCharz().taskMaint != null && Char.myCharz().taskMaint.taskId == 0)
						{
							if (Char.myCharz().taskMaint.index < 4 && Char.myCharz().npcFocus.template.npcTemplateId == 4)
							{
								return false;
							}
							if (Char.myCharz().taskMaint.index < 3 && Char.myCharz().npcFocus.template.npcTemplateId == 3)
							{
								return false;
							}
						}
						tMenuDelay = 50;
						InfoDlg.showWait();
						Service.gI().charMove();
						Service.gI().openMenu(Char.myCharz().npcFocus.template.npcTemplateId);
					}
				}
				else
				{
					int num13 = (20 + Res.r.nextInt(20)) * ((Char.myCharz().cx > Char.myCharz().npcFocus.cx) ? 1 : (-1));
					Char.myCharz().currentMovePoint = new MovePoint(Char.myCharz().npcFocus.cx + num13, Char.myCharz().cy);
					Char.myCharz().endMovePointCommand = new Command(null, null, 8002, null);
					GameCanvas.clearKeyHold();
					GameCanvas.clearKeyPressed();
				}
				return false;
			}
			if (Char.myCharz().charFocus != null)
			{
				if (mobCapcha != null)
				{
					return false;
				}
				if (Char.myCharz().cx < Char.myCharz().charFocus.cx)
				{
					Char.myCharz().cdir = 1;
				}
				else
				{
					Char.myCharz().cdir = -1;
				}
				int num14 = Math.abs(Char.myCharz().cx - Char.myCharz().charFocus.cx);
				int num15 = Math.abs(Char.myCharz().cy - Char.myCharz().charFocus.cy);
				if (Char.myCharz().isMeCanAttackOtherPlayer(Char.myCharz().charFocus) || Char.myCharz().isSelectingSkillBuffToPlayer())
				{
					if (Char.myCharz().myskill == null)
					{
						return false;
					}
					if (!checkSkillValid())
					{
						return false;
					}
					if (Char.myCharz().cx < Char.myCharz().charFocus.cx)
					{
						Char.myCharz().cdir = 1;
					}
					else
					{
						Char.myCharz().cdir = -1;
					}
					Char.myCharz().cvx = 0;
					if (num14 <= Char.myCharz().myskill.dx && num15 <= Char.myCharz().myskill.dy)
					{
						if (Char.myCharz().myskill.template.id == 20)
						{
							return true;
						}
						int num16 = 20;
						if (Char.myCharz().myskill.dx > 60)
						{
							num16 = 60;
							if (num14 < 20)
							{
								Char.myCharz().createShadow(Char.myCharz().cx, Char.myCharz().cy, 10);
							}
						}
						bool flag6 = false;
						if ((TileMap.tileTypeAtPixel(Char.myCharz().cx, Char.myCharz().cy + 3) & 2) == 2)
						{
							int num17 = ((Char.myCharz().cx > Char.myCharz().charFocus.cx) ? 1 : (-1));
							if ((TileMap.tileTypeAtPixel(Char.myCharz().charFocus.cx + num16 * num17, Char.myCharz().cy + 3) & 2) != 2)
							{
								flag6 = true;
							}
						}
						if (num14 <= num16 && !flag6)
						{
							if (Char.myCharz().cx > Char.myCharz().charFocus.cx)
							{
								Char.myCharz().cx = Char.myCharz().charFocus.cx + num16;
								Char.myCharz().cdir = -1;
							}
							else
							{
								Char.myCharz().cx = Char.myCharz().charFocus.cx - num16;
								Char.myCharz().cdir = 1;
							}
							Service.gI().charMove();
						}
						GameCanvas.clearKeyHold();
						GameCanvas.clearKeyPressed();
						return true;
					}
					int num18 = (Char.myCharz().myskill.dx - 20) * ((Char.myCharz().cx > Char.myCharz().charFocus.cx) ? 1 : (-1));
					if (num14 <= Char.myCharz().myskill.dx)
					{
						num18 = 0;
					}
					Char.myCharz().currentMovePoint = new MovePoint(Char.myCharz().charFocus.cx + num18, Char.myCharz().charFocus.cy);
					Char.myCharz().endMovePointCommand = new Command(null, null, 8002, null);
					GameCanvas.clearKeyHold();
					GameCanvas.clearKeyPressed();
					return false;
				}
				if (num14 < 60 && num15 < 40)
				{
					playerMenu(Char.myCharz().charFocus);
					if (!GameCanvas.isTouch && Char.myCharz().charFocus.charID >= 0 && TileMap.mapID != 51 && TileMap.mapID != 52 && popUpYesNo == null)
					{
						GameCanvas.panel.setTypePlayerMenu(Char.myCharz().charFocus);
						GameCanvas.panel.show();
						Service.gI().getPlayerMenu(Char.myCharz().charFocus.charID);
						Service.gI().messagePlayerMenu(Char.myCharz().charFocus.charID);
					}
				}
				else
				{
					int num19 = (20 + Res.r.nextInt(20)) * ((Char.myCharz().cx > Char.myCharz().charFocus.cx) ? 1 : (-1));
					Char.myCharz().currentMovePoint = new MovePoint(Char.myCharz().charFocus.cx + num19, Char.myCharz().charFocus.cy);
					Char.myCharz().endMovePointCommand = new Command(null, null, 8002, null);
					GameCanvas.clearKeyHold();
					GameCanvas.clearKeyPressed();
				}
				return false;
			}
			if (Char.myCharz().itemFocus != null)
			{
				pickItem();
				return false;
			}
			return true;
		}

}
