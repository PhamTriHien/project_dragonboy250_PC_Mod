using System;
using Assets.src.e;
using Assets.src.g;
using UnityEngine;

public partial class Char : IMapObject
{
	public void bagSort()
			{
				try
				{
					MyVector myVector = new MyVector();
					for (int i = 0; i < arrItemBag.Length; i++)
					{
						Item item = arrItemBag[i];
						if (item != null && item.template.isUpToUp && !item.isExpires)
						{
							myVector.addElement(item);
						}
					}
					for (int j = 0; j < myVector.size(); j++)
					{
						Item item2 = (Item)myVector.elementAt(j);
						if (item2 == null)
						{
							continue;
						}
						for (int k = j + 1; k < myVector.size(); k++)
						{
							Item item3 = (Item)myVector.elementAt(k);
							if (item3 != null && item2.template.Equals(item3.template) && item2.isLock == item3.isLock)
							{
								item2.quantity += item3.quantity;
								arrItemBag[item3.indexUI] = null;
								myVector.setElementAt(null, k);
							}
						}
					}
					for (int l = 0; l < arrItemBag.Length; l++)
					{
						if (arrItemBag[l] == null)
						{
							continue;
						}
						for (int m = 0; m <= l; m++)
						{
							if (arrItemBag[m] == null)
							{
								arrItemBag[m] = arrItemBag[l];
								arrItemBag[m].indexUI = m;
								arrItemBag[l] = null;
								break;
							}
						}
					}
				}
				catch (Exception)
				{
					Cout.println("Char.bagSort()");
				}
			}

	private bool isHead_Fly(int head2)
			{
				if (Arr_Head_FlyMove.Length > 0)
				{
					for (int i = 0; i < Arr_Head_FlyMove.Length; i++)
					{
						if (Arr_Head_FlyMove[i] == head2)
						{
							return true;
						}
					}
				}
				return false;
			}

	public void paintMount1(mGraphics g)
			{
				if (xMount <= GameScr.cmx || xMount >= GameScr.cmx + GameCanvas.w)
				{
					return;
				}
				if (me)
				{
					if (!isEndMount && !isStartMount && !isMount)
					{
						return;
					}
					if (idMount >= ID_NEW_MOUNT)
					{
						string nameImg = strMount + (idMount - ID_NEW_MOUNT) + "_0";
						FrameImage fraImage = mSystem.getFraImage(nameImg);
						fraImage?.drawFrame(frameNewMount / 2 % fraImage.nFrame, xMount, yMount + fy, transMount, 3, g);
					}
					else
					{
						if (isSpeacialMount)
						{
							return;
						}
						if (isEventMount)
						{
							g.drawRegion(imgEventMountWing, 0, FrameMount[frameMount] * 60, 60, 60, transMount, xMount + dxMount, yMount + dyMount + fy, 0);
						}
						else if (genderMount == 2)
						{
							if (!isMountVip)
							{
								g.drawRegion(imgMount_XD, 0, FrameMount[frameMount] * 40, 50, 40, transMount, xMount + dxMount, yMount + dyMount + fy, 0);
							}
							else
							{
								g.drawRegion(imgMount_XD_VIP, 0, FrameMount[frameMount] * 40, 50, 40, transMount, xMount + dxMount, yMount + dyMount + fy, 0);
							}
						}
						else if (genderMount == 1)
						{
							if (!isMountVip)
							{
								g.drawRegion(imgMount_NM, 0, FrameMount[frameMount] * 40, 50, 40, transMount, xMount + dxMount, yMount + dyMount + fy, 0);
							}
							else
							{
								g.drawRegion(imgMount_NM_VIP, 0, FrameMount[frameMount] * 40, 50, 40, transMount, xMount + dxMount, yMount + dyMount + fy, 0);
							}
						}
					}
				}
				else
				{
					if (me)
					{
						return;
					}
					if (idMount >= ID_NEW_MOUNT)
					{
						string nameImg2 = strMount + (idMount - ID_NEW_MOUNT) + "_0";
						FrameImage fraImage2 = mSystem.getFraImage(nameImg2);
						fraImage2?.drawFrame(frameNewMount / 2 % fraImage2.nFrame, xMount, yMount + fy, transMount, 3, g);
					}
					else
					{
						if (isSpeacialMount)
						{
							return;
						}
						if (isEventMount)
						{
							g.drawRegion(imgEventMountWing, 0, FrameMount[frameMount] * 60, 60, 60, transMount, xMount + dxMount, yMount + dyMount + fy, 0);
						}
						else
						{
							if (!isMount)
							{
								return;
							}
							if (genderMount == 2)
							{
								if (!isMountVip)
								{
									g.drawRegion(imgMount_XD, 0, FrameMount[frameMount] * 40, 50, 40, transMount, xMount + dxMount, yMount + dyMount + fy, 0);
								}
								else
								{
									g.drawRegion(imgMount_XD_VIP, 0, FrameMount[frameMount] * 40, 50, 40, transMount, xMount + dxMount, yMount + dyMount + fy, 0);
								}
							}
							else if (genderMount == 1)
							{
								if (!isMountVip)
								{
									g.drawRegion(imgMount_NM, 0, FrameMount[frameMount] * 40, 50, 40, transMount, xMount + dxMount, yMount + dyMount + fy, 0);
								}
								else
								{
									g.drawRegion(imgMount_NM_VIP, 0, FrameMount[frameMount] * 40, 50, 40, transMount, xMount + dxMount, yMount + dyMount + fy, 0);
								}
							}
						}
					}
				}
			}

	public void paintMount2(mGraphics g)
			{
				if (xMount <= GameScr.cmx || xMount >= GameScr.cmx + GameCanvas.w)
				{
					return;
				}
				if (me)
				{
					if (!isEndMount && !isStartMount && !isMount)
					{
						return;
					}
					if (idMount >= ID_NEW_MOUNT)
					{
						string nameImg = strMount + (idMount - ID_NEW_MOUNT) + "_1";
						FrameImage fraImage = mSystem.getFraImage(nameImg);
						fraImage?.drawFrame(frameNewMount / 2 % fraImage.nFrame, xMount, yMount + fy, transMount, 3, g);
					}
					else if (isSpeacialMount)
					{
						checkFrameTick(move);
						if (Mob.arrMobTemplate[50] != null && Mob.arrMobTemplate[50].data != null)
						{
							Mob.arrMobTemplate[50].data.paintFrame(g, fM, xMount + ((cdir != 1) ? 8 : (-8)), yMount + 35, (cdir != 1) ? 1 : 0, 0);
						}
						else
						{
							getMountData();
						}
					}
					else if (isEventMount)
					{
						g.drawRegion(imgEventMount, 0, FrameMount[frameMount] * 60, 60, 60, transMount, xMount + dxMount, yMount + dyMount + fy, 0);
					}
					else if (genderMount == 0)
					{
						if (!isMountVip)
						{
							g.drawRegion(imgMount_TD, 0, FrameMount[frameMount] * 40, 50, 40, transMount, xMount + dxMount, yMount + dyMount + fy, 0);
						}
						else
						{
							g.drawRegion(imgMount_TD_VIP, 0, FrameMount[frameMount] * 40, 50, 40, transMount, xMount + dxMount, yMount + dyMount + fy, 0);
						}
					}
					else if (genderMount == 1)
					{
						if (!isMountVip)
						{
							g.drawRegion(imgMount_NM_1, 0, FrameMount[frameMount] * 40, 50, 40, transMount, xMount + dxMount, yMount + dyMount + fy, 0);
						}
						else
						{
							g.drawRegion(imgMount_NM_1_VIP, 0, FrameMount[frameMount] * 40, 50, 40, transMount, xMount + dxMount, yMount + dyMount + fy, 0);
						}
					}
				}
				else
				{
					if (me)
					{
						return;
					}
					if (idMount >= ID_NEW_MOUNT)
					{
						string nameImg2 = strMount + (idMount - ID_NEW_MOUNT) + "_1";
						FrameImage fraImage2 = mSystem.getFraImage(nameImg2);
						fraImage2?.drawFrame(frameNewMount / 2 % fraImage2.nFrame, xMount, yMount + fy, transMount, 3, g);
						return;
					}
					if (isSpeacialMount)
					{
						checkFrameTick(move);
						if (Mob.arrMobTemplate[50] != null && Mob.arrMobTemplate[50].data != null)
						{
							Mob.arrMobTemplate[50].data.paintFrame(g, fM, xMount + ((cdir != 1) ? 8 : (-8)), yMount + 35, (cdir != 1) ? 1 : 0, 0);
						}
						else
						{
							getMountData();
						}
						return;
					}
					if (isEventMount)
					{
						g.drawRegion(imgEventMount, 0, FrameMount[frameMount] * 60, 60, 60, transMount, xMount + dxMount, yMount + dyMount + fy, 0);
					}
					if (!isMount)
					{
						return;
					}
					if (genderMount == 0)
					{
						if (!isMountVip)
						{
							g.drawRegion(imgMount_TD, 0, FrameMount[frameMount] * 40, 50, 40, transMount, xMount + dxMount, yMount + dyMount + fy, 0);
						}
						else
						{
							g.drawRegion(imgMount_TD_VIP, 0, FrameMount[frameMount] * 40, 50, 40, transMount, xMount + dxMount, yMount + dyMount + fy, 0);
						}
					}
					else if (genderMount == 1)
					{
						if (!isMountVip)
						{
							g.drawRegion(imgMount_NM_1, 0, FrameMount[frameMount] * 40, 50, 40, transMount, xMount + dxMount, yMount + dyMount + fy, 0);
						}
						else
						{
							g.drawRegion(imgMount_NM_1_VIP, 0, FrameMount[frameMount] * 40, 50, 40, transMount, xMount + dxMount, yMount + dyMount + fy, 0);
						}
					}
				}
			}

	public void setDefaultBody()
			{
				if (cgender == 0)
				{
					body = 57;
				}
				else if (cgender == 1)
				{
					body = 59;
				}
				else if (cgender == 2)
				{
					body = 57;
				}
			}

	public void setDefaultLeg()
			{
				if (cgender == 0)
				{
					leg = 58;
				}
				else if (cgender == 1)
				{
					leg = 60;
				}
				else if (cgender == 2)
				{
					leg = 58;
				}
			}

	private void paintCharName_HP_MP_Overhead(mGraphics g)
			{
				Part part = GameScr.parts[getFHead(head)];
				int num = CharInfo[cf][0][2] - part.pi[CharInfo[cf][0][0]].dy + 5;
				if ((isInvisiblez && !me) || (!me && TileMap.mapID == 113 && cy >= 360))
				{
					return;
				}
				if (me)
				{
					num += 5;
					paintHp(g, cx, cy - num + 3);
					if (fraDanhHieu != null)
					{
						int x = cx - fraDanhHieu.frameWidth / 2;
						int y = cy - num + 3 - mFont.tahoma_7.getHeight() - (fraDanhHieu.frameHeight + 5);
						if (GameCanvas.gameTick % 5 == 0)
						{
							danhHieuFramme++;
						}
						if (danhHieuFramme >= fraDanhHieu.nFrame)
						{
							danhHieuFramme = 0;
						}
						fraDanhHieu.drawFrame(danhHieuFramme, x, y, 0, mGraphics.TOP | mGraphics.LEFT, g);
					}
					return;
				}
				bool flag = myChar.clan != null && clanID == myChar.clan.ID;
				bool flag2 = cTypePk == 3 || cTypePk == 5;
				bool flag3 = cTypePk == 4;
				if (cName.StartsWith("$"))
				{
					cName = cName.Substring(1);
					isPet = true;
				}
				if (cName.StartsWith("#"))
				{
					cName = cName.Substring(1);
					isMiniPet = true;
				}
				if (myCharz().charFocus != null && myCharz().charFocus.Equals(this))
				{
					num += 5;
					paintHp(g, cx, cy - num + 3);
					if (fraDanhHieu != null)
					{
						int x2 = cx - fraDanhHieu.frameWidth / 2;
						int y2 = cy - num + 3 - mFont.tahoma_7.getHeight() - (fraDanhHieu.frameHeight + 5);
						if (GameCanvas.gameTick % 5 == 0)
						{
							danhHieuFramme++;
						}
						if (danhHieuFramme >= fraDanhHieu.nFrame)
						{
							danhHieuFramme = 0;
						}
						fraDanhHieu.drawFrame(danhHieuFramme, x2, y2, 0, mGraphics.TOP | mGraphics.LEFT, g);
					}
				}
				num += mFont.tahoma_7_white.getHeight();
				mFont mFont2 = mFont.tahoma_7_whiteSmall;
				if (isPet || isMiniPet)
				{
					mFont2 = mFont.tahoma_7_blue1Small;
				}
				else if (flag2)
				{
					mFont2 = mFont.nameFontRed;
				}
				else if (flag3)
				{
					mFont2 = mFont.nameFontYellow;
				}
				else if (flag)
				{
					mFont2 = mFont.nameFontGreen;
				}
				if (TileMap.mapID == 170)
				{
					if (flagImage == 2325)
					{
						mFont2 = mFont.tahoma_7_blue;
					}
					else if (flagImage == 2323)
					{
						mFont2 = mFont.tahoma_7_red;
					}
				}
				if ((paintName || flag2 || flag3) && !flag)
				{
					if (mSystem.clientType == 1)
					{
						mFont2.drawString(g, cName, cx, cy - num, mFont.CENTER, mFont.tahoma_7_greySmall);
					}
					else
					{
						mFont2.drawString(g, cName, cx, cy - num, mFont.CENTER);
					}
					num += mFont.tahoma_7.getHeight();
				}
				if (flag)
				{
					if (myCharz().charFocus != null && myCharz().charFocus.Equals(this))
					{
						mFont2.drawString(g, cName, cx, cy - num, mFont.CENTER, mFont.tahoma_7_greySmall);
					}
					else if (charFocus == null)
					{
						mFont2.drawString(g, cName, cx - 10, cy - num + 3, mFont.LEFT, mFont.tahoma_7_grey);
						paintHp(g, cx - 16, cy - num + 10);
					}
				}
			}

	public void paintBag(mGraphics g, short[] id, int x, int y, int dir, bool isPaintChar)
			{
				int num = 0;
				int num2 = 0;
				if (statusMe == 6)
				{
					num = 8;
					num2 = 17;
				}
				if (statusMe == 1)
				{
					if (cp1 % 15 < 5)
					{
						num = 8;
						num2 = 17;
					}
					else
					{
						num = 8;
						num2 = 18;
					}
				}
				if (statusMe == 2)
				{
					if (cf <= 3)
					{
						num = 7;
						num2 = 17;
					}
					else
					{
						num = 7;
						num2 = 18;
					}
				}
				if (statusMe == 3 || statusMe == 9)
				{
					num = 5;
					num2 = 20;
				}
				if (statusMe == 4)
				{
					if (cf == 8)
					{
						num = 5;
						num2 = 16;
					}
					else
					{
						num = 5;
						num2 = 20;
					}
				}
				if (statusMe == 10)
				{
					if (cf == 8)
					{
						num = 0;
						num2 = 23;
					}
					else
					{
						num = 5;
						num2 = 22;
					}
				}
				if (isInjure > 0)
				{
					num = 5;
					num2 = 18;
				}
				if (skillPaint != null && skillInfoPaint() != null && indexSkill < skillInfoPaint().Length)
				{
					num = -1;
					num2 = 17;
				}
				fBag++;
				if (fBag > 10000)
				{
					fBag = 0;
				}
				sbyte b = (sbyte)(fBag / 4 % id.Length);
				if (!isPaintChar)
				{
					if (id.Length == 2)
					{
						b = 1;
					}
					if (id.Length == 3)
					{
						if (id[2] >= 0)
						{
							b = 2;
							if (GameCanvas.gameTick % 10 > 5)
							{
								b = 1;
							}
						}
						else
						{
							b = 1;
						}
					}
				}
				else if (id.Length > 1 && (b == 0 || b == 1) && statusMe != 1 && statusMe != 6)
				{
					fBag = 0;
					b = 0;
					if (GameCanvas.gameTick % 10 > 5)
					{
						b = 1;
					}
				}
				SmallImage.drawSmallImage(g, id[b], x + ((dir != 1) ? num : (-num)), y - num2, (dir != 1) ? 2 : 0, StaticObj.VCENTER_HCENTER);
			}

	public bool isCharBodyImageID(int id)
			{
				Part part = GameScr.parts[head];
				Part part2 = GameScr.parts[leg];
				Part part3 = GameScr.parts[body];
				for (int i = 0; i < CharInfo.Length; i++)
				{
					if (id == part.pi[CharInfo[i][0][0]].id)
					{
						return true;
					}
					if (id == part2.pi[CharInfo[i][1][0]].id)
					{
						return true;
					}
					if (id == part3.pi[CharInfo[i][2][0]].id)
					{
						return true;
					}
				}
				return false;
			}

	public void paintHead(mGraphics g, int cx, int cy, int look)
			{
				Part part = GameScr.parts[head];
				SmallImage.drawSmallImage(g, part.pi[CharInfo[0][0][0]].id, cx, cy, (look != 0) ? 2 : 0, mGraphics.RIGHT | mGraphics.VCENTER);
			}

	public void paintHeadWithXY(mGraphics g, int x, int y, int look)
			{
				Part part = GameScr.parts[head];
				SmallImage.drawSmallImage(g, part.pi[CharInfo[0][0][0]].id, x + CharInfo[0][0][1] + part.pi[CharInfo[0][0][0]].dx - 3, y + 3, look, mGraphics.LEFT | mGraphics.BOTTOM);
			}

	public void paintCharBody(mGraphics g, int cx, int cy, int cdir, int cf, bool isPaintBag)
			{
				ph = GameScr.parts[head];
				pl = GameScr.parts[leg];
				pb = GameScr.parts[body];
				if (bag >= 0 && statusMe != 14)
				{
					if (!ClanImage.idImages.containsKey(bag + string.Empty))
					{
						ClanImage.idImages.put(bag + string.Empty, new ClanImage());
						Service.gI().requestBagImage(bag);
					}
					else
					{
						ClanImage clanImage = (ClanImage)ClanImage.idImages.get(bag + string.Empty);
						if (clanImage.idImage != null && isPaintBag)
						{
							paintBag(g, clanImage.idImage, cx, cy, cdir, isPaintChar: true);
						}
					}
				}
				int num = 2;
				int anchor = 24;
				int anchor2 = StaticObj.TOP_RIGHT;
				int num2 = -1;
				if (cdir == 1)
				{
					num = 0;
					anchor = 0;
					anchor2 = 0;
					num2 = 1;
				}
				if (statusMe == 14)
				{
					if (GameCanvas.gameTick % 4 > 0)
					{
						g.drawImage(ItemMap.imageFlare, cx, cy - ch - 11, mGraphics.HCENTER | mGraphics.VCENTER);
					}
					int num3 = 0;
					if (head == 89 || head == 457 || head == 460 || head == 461 || head == 462 || head == 463 || head == 464 || head == 465 || head == 466)
					{
						num3 = 15;
					}
					if (head == 1291)
					{
						num3 = 23;
					}
					SmallImage.drawSmallImage(g, 834, cx, cy - CharInfo[cf][2][2] + pb.pi[CharInfo[cf][2][0]].dy - 2 + num3, num, StaticObj.TOP_CENTER);
					SmallImage.drawSmallImage(g, 79, cx, cy - ch - 8, 0, mGraphics.HCENTER | mGraphics.BOTTOM);
					SmallImage.drawSmallImage(g, ph.pi[CharInfo[cf][0][0]].id, cx + (CharInfo[cf][0][1] + ph.pi[CharInfo[cf][0][0]].dx) * num2, cy - CharInfo[cf][0][2] + ph.pi[CharInfo[cf][0][0]].dy, num, anchor);
					paintHat_behind(g, cf, cy - CharInfo[cf][2][2] + pb.pi[CharInfo[cf][2][0]].dy);
					if (isHead_2Fr(head))
					{
						Part part = GameScr.parts[getFHead(head)];
						SmallImage.drawSmallImage(g, part.pi[CharInfo[cf][0][0]].id, cx + (CharInfo[cf][0][1] + part.pi[CharInfo[cf][0][0]].dx) * num2, cy - CharInfo[cf][0][2] + part.pi[CharInfo[cf][0][0]].dy, num, anchor);
					}
					else
					{
						SmallImage.drawSmallImage(g, ph.pi[CharInfo[cf][0][0]].id, cx + (CharInfo[cf][0][1] + ph.pi[CharInfo[cf][0][0]].dx) * num2, cy - CharInfo[cf][0][2] + ph.pi[CharInfo[cf][0][0]].dy, num, anchor);
					}
					paintHat_front(g, cf, cy - CharInfo[cf][2][2] + pb.pi[CharInfo[cf][2][0]].dy);
					paintRedEye(g, cx + (CharInfo[cf][0][1] + ph.pi[CharInfo[cf][0][0]].dx) * num2, cy - CharInfo[cf][0][2] + ph.pi[CharInfo[cf][0][0]].dy, num, anchor);
				}
				else
				{
					paintHat_behind(g, cf, cy - CharInfo[cf][2][2] + pb.pi[CharInfo[cf][2][0]].dy);
					try
					{
						if (isHead_2Fr(head))
						{
							Part part2 = GameScr.parts[getFHead(head)];
							SmallImage.drawSmallImage(g, part2.pi[CharInfo[cf][0][0]].id, cx + (CharInfo[cf][0][1] + part2.pi[CharInfo[cf][0][0]].dx) * num2, cy - CharInfo[cf][0][2] + part2.pi[CharInfo[cf][0][0]].dy, num, anchor);
						}
						else
						{
							SmallImage.drawSmallImage(g, ph.pi[CharInfo[cf][0][0]].id, cx + (CharInfo[cf][0][1] + ph.pi[CharInfo[cf][0][0]].dx) * num2, cy - CharInfo[cf][0][2] + ph.pi[CharInfo[cf][0][0]].dy, num, anchor);
						}
						SmallImage.drawSmallImage(g, pl.pi[CharInfo[cf][1][0]].id, cx + (CharInfo[cf][1][1] + pl.pi[CharInfo[cf][1][0]].dx) * num2, cy - CharInfo[cf][1][2] + pl.pi[CharInfo[cf][1][0]].dy, num, anchor);
						SmallImage.drawSmallImage(g, pb.pi[CharInfo[cf][2][0]].id, cx + (CharInfo[cf][2][1] + pb.pi[CharInfo[cf][2][0]].dx) * num2, cy - CharInfo[cf][2][2] + pb.pi[CharInfo[cf][2][0]].dy, num, anchor);
						paintRedEye(g, cx + (CharInfo[cf][0][1] + ph.pi[CharInfo[cf][0][0]].dx) * num2, cy - CharInfo[cf][0][2] + ph.pi[CharInfo[cf][0][0]].dy, num, anchor);
					}
					catch (Exception ex)
					{
						Debug.LogError(">>>>>>err: " + ex.ToString());
					}
				}
				ch = ((isMonkey != 1 && !isFusion) ? (CharInfo[0][0][2] + ph.pi[CharInfo[0][0][0]].dy + 10) : 60);
				int num4 = ((Res.abs(ph.pi[CharInfo[cf][0][0]].dy) < 22) ? ph.pi[CharInfo[cf][0][0]].dy : ((ph.pi[CharInfo[cf][0][0]].dy >= 0) ? (ph.pi[CharInfo[cf][0][0]].dy - 5) : (ph.pi[CharInfo[cf][0][0]].dy + 5)));
				cH_new = cy - CharInfo[cf][0][2] + num4;
				if (statusMe == 1 && charID > 0 && !isMask && !isUseChargeSkill() && !isWaitMonkey && skillPaint == null && cf != 23 && bag < 0 && ((GameCanvas.gameTick + charID) % 30 == 0 || isFreez))
				{
					g.drawImage((cgender != 1) ? eyeTraiDat : eyeNamek, cx + -((cgender != 1) ? 2 : 2) * num2, cy - 32 + ((cgender != 1) ? 11 : 10) - cf, anchor2);
				}
				if (eProtect != null)
				{
					eProtect.paint(g);
				}
				if (eDanhHieu != null)
				{
					eDanhHieu.paint(g);
				}
				paintPKFlag(g);
			}

	private void paintRedEye(mGraphics g, int xx, int yy, int trans, int anchor)
			{
				if (head != 934 || (statusMe != 1 && statusMe != 6))
				{
					return;
				}
				if (fraRedEye == null || fraRedEye.imgFrame == null)
				{
					Image img = mSystem.loadImage("/redeye.png");
					fraRedEye = new FrameImage(img, 14, 10);
				}
				else if (frEye[fChopmat] != -1)
				{
					int num = 8;
					int num2 = 15;
					if (trans == 2)
					{
						num = -8;
					}
					fraRedEye.drawFrame(frEye[fChopmat], xx + num, yy + num2, trans, anchor, g);
				}
			}

	public bool isHead_2Fr(int idHead)
			{
				for (int i = 0; i < Arr_Head_2Fr.Length; i++)
				{
					if (Arr_Head_2Fr[i][0] == idHead)
					{
						return true;
					}
				}
				return false;
			}

	private void updateFHead()
			{
				if (isHead_2Fr(head))
				{
					fHead++;
					if (fHead > 10000)
					{
						fHead = 0;
					}
				}
				else
				{
					fHead = 0;
				}
			}

	private int getFHead(int idHead)
			{
				for (int i = 0; i < Arr_Head_2Fr.Length; i++)
				{
					if (Arr_Head_2Fr[i][0] == idHead)
					{
						return Arr_Head_2Fr[i][fHead / 4 % Arr_Head_2Fr[i].Length];
					}
				}
				return idHead;
			}

}
