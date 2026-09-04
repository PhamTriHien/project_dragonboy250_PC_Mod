using System;
using Assets.src.g;
public partial class Mob
{
	public virtual void paint(mGraphics g)
			{
				if (isHide)
				{
					return;
				}
				if (isMafuba)
				{
					if (!changBody)
					{
						arrMobTemplate[templateId].data.paintFrame(g, frame, xMFB, yMFB, (dir != 1) ? 1 : 0, 2);
					}
					else
					{
						SmallImage.drawSmallImage(g, smallBody, xMFB, yMFB, (dir != 1) ? 2 : 0, mGraphics.BOTTOM | mGraphics.HCENTER);
					}
					return;
				}
				if (isShadown && status != 0)
				{
					paintShadow(g);
				}
				if (!isPaint() || (status == 1 && p3 > 0 && GameCanvas.gameTick % 3 == 0))
				{
					return;
				}
				g.translate(0, GameCanvas.transY);
				if (!changBody)
				{
					arrMobTemplate[templateId].data.paintFrame(g, frame, x, y + fy, (dir != 1) ? 1 : 0, 2);
				}
				else
				{
					SmallImage.drawSmallImage(g, smallBody, x, y + fy - 9, (dir != 1) ? 2 : 0, mGraphics.BOTTOM | mGraphics.HCENTER);
				}
				g.translate(0, -GameCanvas.transY);
				if (Char.myCharz().mobFocus == null || !Char.myCharz().mobFocus.Equals(this) || status == 1 || hp <= 0 || imgHPtem == null)
				{
					return;
				}
				int imageWidth = mGraphics.getImageWidth(imgHPtem);
				int imageHeight = mGraphics.getImageHeight(imgHPtem);
				int num = imageWidth * per / 100;
				int num2 = num;
				if (per_tem >= per)
				{
					num2 = imageWidth * (per_tem -= ((GameCanvas.gameTick % 6 <= 3) ? offset : offset++)) / 100;
					if (per_tem <= 0)
					{
						per_tem = 0;
					}
					if (per_tem < per)
					{
						per_tem = per;
					}
					if (offset >= 3)
					{
						offset = 3;
					}
				}
				g.drawImage(GameScr.imgHP_tm_xam, x - (imageWidth >> 1), y - h - 5, mGraphics.TOP | mGraphics.LEFT);
				g.setColor(16777215);
				g.fillRect(x - (imageWidth >> 1), y - h - 5, num2, 2);
				g.drawRegion(imgHPtem, 0, 0, num, imageHeight, 0, x - (imageWidth >> 1), y - h - 5, mGraphics.TOP | mGraphics.LEFT);
			}
	public int getX()
			{
				return x;
			}
	public int getY()
			{
				return y;
			}
	public int getH()
			{
				return h;
			}
	public int getW()
			{
				return w;
			}
	public void stopMoving()
			{
				if (status == 5)
				{
					status = 2;
					p1 = (p2 = (p3 = 0));
					forceWait = 50;
				}
			}
	public bool isInvisible()
			{
				return status == 0 || status == 1;
			}
	public void GetFrame()
			{
				if (isGetFr && isTypeNewMod() && arrMobTemplate[templateId].data != null)
				{
					frameArr = (int[][])Controller.frameHT_NEWBOSS.get(templateId + string.Empty);
					stand = frameArr[0];
					move = frameArr[1];
					moveFast = frameArr[2];
					attack1 = frameArr[3];
					attack2 = frameArr[4];
					hurt = frameArr[5];
					isGetFr = false;
				}
			}
	private bool isTypeNewMod()
			{
				if (arrMobTemplate[templateId].data != null && arrMobTemplate[templateId].data.typeData == 2)
				{
					return true;
				}
				return false;
			}

}
