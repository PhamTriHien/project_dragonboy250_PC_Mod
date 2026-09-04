using System;
using System.Collections.Generic;
using Assets.src.g;
using UnityEngine;
public partial class Panel : IActionListener, IChatable
{
	public void paintTask(mGraphics g)
			{
				int num = ((GameCanvas.h <= 300) ? 15 : 20);
				if (isPaintMap && !GameScr.gI().isMapDocNhan() && !GameScr.gI().isMapFize())
				{
					g.drawImage((keyTouchMapButton != 1) ? GameScr.imgLbtn : GameScr.imgLbtnFocus, xScroll + wScroll / 2, yScroll + hScroll - num, 3);
					mFont.tahoma_7b_dark.drawString(g, mResources.map, xScroll + wScroll / 2, yScroll + hScroll - (num + 5), mFont.CENTER);
				}
				xstart = xScroll + 5;
				ystart = yScroll + 14;
				yPaint = ystart;
				g.setClip(xScroll, yScroll, wScroll, hScroll - 35);
				if (scroll != null)
				{
					if (scroll.cmy > 0)
					{
						g.drawRegion(Mob.imgHP, 0, 0, 9, 6, 1, xScroll + wScroll - 12, yScroll + 3, 0);
					}
					if (scroll.cmy < scroll.cmyLim)
					{
						g.drawRegion(Mob.imgHP, 0, 0, 9, 6, 0, xScroll + wScroll - 12, yScroll + hScroll - 45, 0);
					}
					g.translate(0, -scroll.cmy);
				}
				indexRowMax = 0;
				if (indexMenu == 0)
				{
					bool flag = false;
					if (Char.myCharz().taskMaint != null)
					{
						for (int i = 0; i < Char.myCharz().taskMaint.names.Length; i++)
						{
							mFont.tahoma_7_grey.drawString(g, Char.myCharz().taskMaint.names[i], xScroll + wScroll / 2, yPaint - 5 + i * 12, mFont.CENTER);
							indexRowMax++;
						}
						yPaint += (Char.myCharz().taskMaint.names.Length - 1) * 12;
						int num2 = 0;
						string empty = string.Empty;
						for (int j = 0; j < Char.myCharz().taskMaint.subNames.Length; j++)
						{
							if (Char.myCharz().taskMaint.subNames[j] != null)
							{
								num2 = j;
								empty = "- " + Char.myCharz().taskMaint.subNames[j];
								if (Char.myCharz().taskMaint.counts[j] != -1)
								{
									if (Char.myCharz().taskMaint.index == j)
									{
										if (Char.myCharz().taskMaint.counts[j] != 1)
										{
											string text = empty;
											empty = text + " (" + Char.myCharz().taskMaint.count + "/" + Char.myCharz().taskMaint.counts[j] + ")";
										}
										if (Char.myCharz().taskMaint.count == Char.myCharz().taskMaint.counts[j])
										{
											mFont.tahoma_7.drawString(g, empty, xstart + 5, yPaint += 12, 0);
										}
										else
										{
											mFont tahoma_7_grey = mFont.tahoma_7_grey;
											if (!flag)
											{
												flag = true;
												tahoma_7_grey = mFont.tahoma_7_blue;
												tahoma_7_grey.drawString(g, empty, xstart + 5 + ((tahoma_7_grey == mFont.tahoma_7_blue && GameCanvas.gameTick % 20 > 10) ? (GameCanvas.gameTick % 4 / 2) : 0), yPaint += 12, 0);
											}
											else
											{
												tahoma_7_grey.drawString(g, "- ...", xstart + 5 + ((tahoma_7_grey == mFont.tahoma_7_blue && GameCanvas.gameTick % 20 > 10) ? (GameCanvas.gameTick % 4 / 2) : 0), yPaint += 12, 0);
											}
										}
									}
									else if (Char.myCharz().taskMaint.index > j)
									{
										if (Char.myCharz().taskMaint.counts[j] != 1)
										{
											string text = empty;
											empty = text + " (" + Char.myCharz().taskMaint.counts[j] + "/" + Char.myCharz().taskMaint.counts[j] + ")";
										}
										mFont.tahoma_7_white.drawString(g, empty, xstart + 5, yPaint += 12, 0);
									}
									else
									{
										if (Char.myCharz().taskMaint.counts[j] != 1)
										{
											empty = empty + " 0/" + Char.myCharz().taskMaint.counts[j];
										}
										mFont tahoma_7_grey2 = mFont.tahoma_7_grey;
										if (!flag)
										{
											flag = true;
											tahoma_7_grey2 = mFont.tahoma_7_blue;
											tahoma_7_grey2.drawString(g, empty, xstart + 5 + ((tahoma_7_grey2 == mFont.tahoma_7_blue && GameCanvas.gameTick % 20 > 10) ? (GameCanvas.gameTick % 4 / 2) : 0), yPaint += 12, 0);
										}
										else
										{
											tahoma_7_grey2.drawString(g, "- ...", xstart + 5 + ((tahoma_7_grey2 == mFont.tahoma_7_blue && GameCanvas.gameTick % 20 > 10) ? (GameCanvas.gameTick % 4 / 2) : 0), yPaint += 12, 0);
										}
									}
								}
								else if (Char.myCharz().taskMaint.index > j)
								{
									mFont.tahoma_7_white.drawString(g, empty, xstart + 5, yPaint += 12, 0);
								}
								else
								{
									mFont tahoma_7_grey3 = mFont.tahoma_7_grey;
									if (!flag)
									{
										flag = true;
										tahoma_7_grey3 = mFont.tahoma_7_blue;
										tahoma_7_grey3.drawString(g, empty, xstart + 5 + ((tahoma_7_grey3 == mFont.tahoma_7_blue && GameCanvas.gameTick % 20 > 10) ? (GameCanvas.gameTick % 4 / 2) : 0), yPaint += 12, 0);
									}
									else
									{
										tahoma_7_grey3.drawString(g, "- ...", xstart + 5 + ((tahoma_7_grey3 == mFont.tahoma_7_blue && GameCanvas.gameTick % 20 > 10) ? (GameCanvas.gameTick % 4 / 2) : 0), yPaint += 12, 0);
									}
								}
								indexRowMax++;
							}
							else if (Char.myCharz().taskMaint.index <= j)
							{
								empty = "- " + Char.myCharz().taskMaint.subNames[num2];
								mFont mFont2 = mFont.tahoma_7_grey;
								if (!flag)
								{
									flag = true;
									mFont2 = mFont.tahoma_7_blue;
								}
								mFont2.drawString(g, empty, xstart + 5 + ((mFont2 == mFont.tahoma_7_blue && GameCanvas.gameTick % 20 > 10) ? (GameCanvas.gameTick % 4 / 2) : 0), yPaint += 12, 0);
							}
						}
						yPaint += 5;
						for (int k = 0; k < Char.myCharz().taskMaint.details.Length; k++)
						{
							mFont.tahoma_7_green2.drawString(g, Char.myCharz().taskMaint.details[k], xstart + 5, yPaint += 12, 0);
							indexRowMax++;
						}
					}
					else
					{
						int taskMapId = GameScr.getTaskMapId();
						sbyte taskNpcId = GameScr.getTaskNpcId();
						string empty2 = string.Empty;
						if (taskMapId == -3 || taskNpcId == -3)
						{
							empty2 = mResources.DES_TASK[3];
						}
						else if (Char.myCharz().taskMaint == null && Char.myCharz().ctaskId == 9 && Char.myCharz().nClass.classId == 0)
						{
							empty2 = mResources.TASK_INPUT_CLASS;
						}
						else
						{
							if (taskNpcId < 0 || taskMapId < 0)
							{
								return;
							}
							empty2 = mResources.DES_TASK[0] + Npc.arrNpcTemplate[taskNpcId].name + mResources.DES_TASK[1] + TileMap.mapNames[taskMapId] + mResources.DES_TASK[2];
						}
						string[] array = mFont.tahoma_7_white.splitFontArray(empty2, 150);
						for (int l = 0; l < array.Length; l++)
						{
							if (l == 0)
							{
								mFont.tahoma_7_white.drawString(g, array[l], xstart + 5, yPaint = ystart, 0);
							}
							else
							{
								mFont.tahoma_7_white.drawString(g, array[l], xstart + 5, yPaint += 12, 0);
							}
						}
					}
				}
				else if (indexMenu == 1)
				{
					yPaint = ystart - 12;
					for (int m = 0; m < Char.myCharz().taskOrders.size(); m++)
					{
						TaskOrder taskOrder = (TaskOrder)Char.myCharz().taskOrders.elementAt(m);
						mFont.tahoma_7_white.drawString(g, taskOrder.name, xstart + 5, yPaint += 12, 0);
						if (taskOrder.count == taskOrder.maxCount)
						{
							mFont.tahoma_7_white.drawString(g, ((taskOrder.taskId != 0) ? mResources.KILLBOSS : mResources.KILL) + " " + Mob.arrMobTemplate[taskOrder.killId].name + " (" + taskOrder.count + "/" + taskOrder.maxCount + ")", xstart + 5, yPaint += 12, 0);
						}
						else
						{
							mFont.tahoma_7_blue.drawString(g, ((taskOrder.taskId != 0) ? mResources.KILLBOSS : mResources.KILL) + " " + Mob.arrMobTemplate[taskOrder.killId].name + " (" + taskOrder.count + "/" + taskOrder.maxCount + ")", xstart + 5, yPaint += 12, 0);
						}
						indexRowMax += 3;
						inforW = popupW - 25;
						paintMultiLine(g, mFont.tahoma_7_grey, taskOrder.description, xstart + 5, yPaint += 12, 0);
						yPaint += 12;
					}
				}
				if (scroll == null)
				{
					scroll = new Scroll();
					scroll.setStyle(indexRowMax, 12, xScroll, yScroll, wScroll, hScroll - num - 40, styleUPDOWN: true, 1);
				}
			}
	public void paintMultiLine(mGraphics g, mFont f, string[] arr, string str, int x, int y, int align)
			{
				for (int i = 0; i < arr.Length; i++)
				{
					string text = arr[i];
					if (text.StartsWith("c"))
					{
						if (text.StartsWith("c0"))
						{
							text = text.Substring(2);
							f = mFont.tahoma_7b_dark;
						}
						else if (text.StartsWith("c1"))
						{
							text = text.Substring(2);
							f = mFont.tahoma_7b_yellow;
						}
						else if (text.StartsWith("c2"))
						{
							text = text.Substring(2);
							f = mFont.tahoma_7b_green;
						}
					}
					if (i == 0)
					{
						f.drawString(g, text, x, y, align);
						continue;
					}
					if (i < indexRow + 30 && i > indexRow - 30)
					{
						f.drawString(g, text, x, y += 12, align);
					}
					else
					{
						y += 12;
					}
					yPaint += 12;
					indexRowMax++;
				}
			}
	public void paintMultiLine(mGraphics g, mFont f, string str, int x, int y, int align)
			{
				int num = ((!GameCanvas.isTouch || GameCanvas.w < 320) ? 10 : 20);
				string[] array = f.splitFontArray(str, inforW - num);
				for (int i = 0; i < array.Length; i++)
				{
					if (i == 0)
					{
						f.drawString(g, array[i], x, y, align);
						continue;
					}
					if (i < indexRow + 15 && i > indexRow - 15)
					{
						f.drawString(g, array[i], x, y += 12, align);
					}
					else
					{
						y += 12;
					}
					yPaint += 12;
					indexRowMax++;
				}
			}
	private void paintAccount(mGraphics g)
			{
				g.setClip(xScroll, yScroll, wScroll, hScroll);
				g.translate(0, -cmy);
				for (int i = 0; i < strAccount.Length; i++)
				{
					int x = xScroll;
					int num = yScroll + i * ITEM_HEIGHT;
					int num2 = wScroll - 1;
					int h = ITEM_HEIGHT - 1;
					if (num - cmy <= yScroll + hScroll && num - cmy >= yScroll - ITEM_HEIGHT)
					{
						g.setColor((i != selected) ? 15196114 : 16383818);
						g.fillRect(x, num, num2, h);
						mFont.tahoma_7b_dark.drawString(g, strAccount[i], xScroll + wScroll / 2, num + 6, mFont.CENTER);
					}
				}
				paintScrollArrow(g);
			}

}
