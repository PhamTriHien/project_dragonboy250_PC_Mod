using System;
using Assets.src.g;
public partial class Mob
{
	public Mob()
			{
			}
	public Mob(int mobId, bool isDisable, bool isDontMove, bool isFire, bool isIce, bool isWind, int templateId, int sys, long hp, sbyte level, long maxp, short pointx, short pointy, sbyte status, sbyte levelBoss)
			{
				this.isDisable = isDisable;
				this.isDontMove = isDontMove;
				this.isFire = isFire;
				this.isIce = isIce;
				this.isWind = isWind;
				this.sys = sys;
				this.mobId = mobId;
				this.templateId = templateId;
				this.hp = hp;
				this.level = level;
				xFirst = (x = (this.pointx = pointx));
				yFirst = (y = (this.pointy = pointy));
				this.status = status;
				if (templateId != 70)
				{
					checkData();
					getData();
				}
				if (!isExistNewMob(templateId + string.Empty))
				{
					newMob.addElement(templateId + string.Empty);
				}
				maxHp = maxp;
				this.levelBoss = levelBoss;
				updateHp_bar();
				isDie = false;
				xSd = pointx;
				ySd = pointy;
				if (isNewModStand())
				{
					stand = new int[17]
					{
						0, 0, 0, 0, 0, 1, 1, 1, 1, 1,
						2, 2, 2, 2, 2, 2, 2
					};
					move = new int[17]
					{
						0, 0, 0, 0, 0, 1, 1, 1, 1, 1,
						2, 2, 2, 2, 2, 2, 2
					};
					moveFast = new int[17]
					{
						0, 0, 0, 0, 0, 1, 1, 1, 1, 1,
						2, 2, 2, 2, 2, 2, 2
					};
					attack1 = new int[12]
					{
						3, 3, 3, 3, 4, 4, 4, 4, 5, 5,
						5, 5
					};
					attack2 = new int[12]
					{
						3, 3, 3, 3, 4, 4, 4, 4, 5, 5,
						5, 5
					};
				}
				else if (isNewMod())
				{
					stand = new int[12]
					{
						0, 0, 0, 0, 0, 0, 0, 0, 1, 1,
						1, 1
					};
					move = new int[16]
					{
						1, 1, 1, 1, 2, 2, 2, 2, 1, 1,
						1, 1, 3, 3, 3, 3
					};
					moveFast = new int[8] { 1, 1, 2, 2, 1, 1, 3, 3 };
					attack1 = new int[11]
					{
						4, 4, 4, 5, 5, 5, 6, 6, 6, 6,
						6
					};
					attack2 = new int[11]
					{
						7, 7, 7, 8, 8, 8, 9, 9, 9, 9,
						9
					};
				}
				else if (isSpecial())
				{
					stand = new int[12]
					{
						0, 0, 0, 0, 0, 0, 0, 0, 1, 1,
						1, 1
					};
					move = new int[16]
					{
						2, 2, 3, 3, 2, 2, 4, 4, 2, 2,
						3, 3, 2, 2, 4, 4
					};
					moveFast = new int[8] { 2, 2, 3, 3, 2, 2, 4, 4 };
					attack1 = new int[8] { 5, 6, 7, 8, 9, 10, 11, 12 };
					attack2 = new int[4] { 5, 12, 13, 14 };
				}
				else
				{
					stand = new int[12]
					{
						0, 0, 0, 0, 0, 0, 0, 0, 1, 1,
						1, 1
					};
					move = new int[15]
					{
						1, 1, 1, 1, 2, 2, 2, 2, 3, 3,
						3, 3, 2, 2, 2
					};
					moveFast = new int[7] { 1, 1, 2, 2, 3, 3, 2 };
					attack1 = new int[3] { 4, 5, 6 };
					attack2 = new int[3] { 7, 8, 9 };
				}
			}
	public void getData()
			{
				if (arrMobTemplate[templateId].data == null)
				{
					arrMobTemplate[templateId].data = new EffectData();
					string text = "/Mob/" + templateId;
					DataInputStream dataInputStream = MyStream.readFile(text);
					if (dataInputStream != null)
					{
						arrMobTemplate[templateId].data.readData(text + "/data");
						arrMobTemplate[templateId].data.img = GameCanvas.loadImage(text + "/img.png");
					}
					else
					{
						Service.gI().requestModTemplate(templateId);
					}
					if (lastMob.size() > 15)
					{
						arrMobTemplate[int.Parse((string)lastMob.elementAt(0))].data = null;
						lastMob.removeElementAt(0);
					}
					lastMob.addElement(templateId + string.Empty);
				}
				else
				{
					w = arrMobTemplate[templateId].data.width;
					h = arrMobTemplate[templateId].data.height;
				}
			}
	public virtual void setBody(short id)
			{
				changBody = true;
				smallBody = id;
			}
	public virtual void clearBody()
			{
				changBody = false;
			}
	public static bool isExistNewMob(string id)
			{
				for (int i = 0; i < newMob.size(); i++)
				{
					string text = (string)newMob.elementAt(i);
					if (text.Equals(id))
					{
						return true;
					}
				}
				return false;
			}
	public void checkData()
			{
				int num = 0;
				for (int i = 0; i < arrMobTemplate.Length; i++)
				{
					if (arrMobTemplate[i].data != null)
					{
						num++;
					}
				}
				if (num < 10)
				{
					return;
				}
				for (int j = 0; j < arrMobTemplate.Length; j++)
				{
					if (arrMobTemplate[j].data != null && num > 5)
					{
						arrMobTemplate[j].data = null;
					}
				}
			}
	public void checkFrameTick(int[] array)
			{
				if (tick > array.Length - 1)
				{
					tick = 0;
				}
				frame = array[tick];
				tick++;
			}
	private void updateShadown()
			{
				int num = TileMap.size;
				xSd = x;
				wCount = 0;
				if (ySd <= 0 || TileMap.tileTypeAt(xSd, ySd, 2))
				{
					return;
				}
				if (TileMap.tileTypeAt(xSd / num, ySd / num) == 0)
				{
					isOutMap = true;
				}
				else if (TileMap.tileTypeAt(xSd / num, ySd / num) != 0 && !TileMap.tileTypeAt(xSd, ySd, 2))
				{
					xSd = x;
					ySd = y;
					isOutMap = false;
				}
				while (isOutMap && wCount < 10)
				{
					wCount++;
					ySd += 24;
					if (TileMap.tileTypeAt(xSd, ySd, 2))
					{
						if (ySd % 24 != 0)
						{
							ySd -= ySd % 24;
						}
						break;
					}
				}
			}
	private void paintShadow(mGraphics g)
			{
				int num = TileMap.size;
				if (TileMap.tileTypeAt(xSd + num / 2, ySd + 1, 4))
				{
					g.setClip(xSd / num * num, (ySd - 30) / num * num, num, 100);
				}
				else if (TileMap.tileTypeAt((xSd - num / 2) / num, (ySd + 1) / num) == 0)
				{
					g.setClip(xSd / num * num, (ySd - 30) / num * num, 100, 100);
				}
				else if (TileMap.tileTypeAt((xSd + num / 2) / num, (ySd + 1) / num) == 0)
				{
					g.setClip(xSd / num * num, (ySd - 30) / num * num, num, 100);
				}
				else if (TileMap.tileTypeAt(xSd - num / 2, ySd + 1, 8))
				{
					g.setClip(xSd / 24 * num, (ySd - 30) / num * num, num, 100);
				}
				g.drawImage(TileMap.bong, xSd, ySd, 3);
				g.setClip(GameScr.cmx, GameScr.cmy - GameCanvas.transY, GameScr.gW, GameScr.gH + 2 * GameCanvas.transY);
			}

}
