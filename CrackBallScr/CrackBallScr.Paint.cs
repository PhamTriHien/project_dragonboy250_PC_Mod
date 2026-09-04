using System;
using System.Threading;

public partial class CrackBallScr
{
	public override void paint(mGraphics g)
		{
			try
			{
				GameScr.gI().paint(g);
				g.translate(-GameScr.cmx, -GameScr.cmy);
				g.translate(0, GameCanvas.transY);
				for (int i = 0; i < listBall.Length; i++)
				{
					if (listBall[i].isPaint && listBall[i].y > listBall[i].yTo - 20)
					{
						g.drawImage(TileMap.bong, listBall[i].x, listBall[i].yTo + 7, mGraphics.VCENTER | mGraphics.HCENTER);
					}
				}
				for (int j = 0; j < listBall.Length; j++)
				{
					if (listBall[j].isPaint)
					{
						SmallImage.drawSmallImage(g, listBall[j].idImg, listBall[j].x, listBall[j].y, 0, mGraphics.VCENTER | mGraphics.HCENTER);
					}
				}
				if (isKame)
				{
					if (fraImgKame != null)
					{
						int num = Char.myCharz().cx - fraImgKame.frameWidth - 28;
						for (int k = 0; k < GameCanvas.w / fraImgKame.frameWidth + 1; k++)
						{
							fraImgKame.drawFrame(frame, num - k * (fraImgKame.frameWidth - 1), Char.myCharz().cy - fraImgKame.frameHeight / 2 - 12 + 2, 0, 0, g);
						}
					}
					if (fraImgKame_1 != null)
					{
						int num2 = Char.myCharz().cx - fraImgKame_1.frameWidth - 10;
						fraImgKame_1.drawFrame(frame, num2 - 5, Char.myCharz().cy - fraImgKame_1.frameHeight / 2 - 12, 0, 0, g);
					}
				}
				GameScr.resetTranslate(g);
				int num3 = 240;
				int num4 = GameCanvas.w - num3;
				int num5 = 15;
				g.setColor(13524492);
				g.fillRect(num4, num5 - 15, num3, 15);
				g.drawImage(Panel.imgXu, num4 + 11, num5 - 7, 3);
				g.drawImage(Panel.imgLuong, num4 + 90, num5 - 8, 3);
				mFont.tahoma_7_yellow.drawString(g, Char.myCharz().xuStr + string.Empty, num4 + 24, num5 - 13, mFont.LEFT, mFont.tahoma_7_grey);
				mFont.tahoma_7_yellow.drawString(g, Char.myCharz().luongStr + string.Empty, num4 + 100, num5 - 13, mFont.LEFT, mFont.tahoma_7_grey);
				g.drawImage(Panel.imgLuongKhoa, num4 + 150, num5 - 7, 3);
				mFont.tahoma_7_yellow.drawString(g, Char.myCharz().luongKhoaStr + string.Empty, num4 + 160, num5 - 13, mFont.LEFT, mFont.tahoma_7_grey);
				g.drawImage(Panel.imgTicket, num4 + 200, num5 - 7, 3);
				mFont.tahoma_7_yellow.drawString(g, numTicket + string.Empty, num4 + 210, num5 - 13, mFont.LEFT, mFont.tahoma_7_grey);
				if (step < 4)
				{
					int num6 = num3 / 2 + 20;
					int num7 = GameCanvas.w - num6;
					g.setColor(11837316);
					g.fillRect(num7, num5, num6, 15);
					if (typePrice == 0)
					{
						g.drawImage(Panel.imgXu, num7 + 21, num5 + 8, 3);
					}
					else
					{
						g.drawImage(Panel.imgLuongKhoa, num7 + 21, num5 + 7, 3);
						g.drawImage(Panel.imgLuong, num7 + 18, num5 + 7, 3);
					}
					mFont.tahoma_7_red.drawString(g, " -" + cost, num7 + 30, num5 + 2, mFont.LEFT, mFont.tahoma_7_grey);
					g.drawImage(Panel.imgTicket, num7 + 80, num5 + 7, 3);
					mFont.tahoma_7_red.drawString(g, " -" + checkTicket(), num7 + 90, num5 + 2, mFont.LEFT, mFont.tahoma_7_grey);
				}
				g.drawImage(GameScr.imgSkill, xSkill[0], ySkill, 0);
				if (indexSkillSelect == 0)
				{
					g.drawImage(GameScr.imgSkill2, xSkill[0], ySkill, 0);
				}
				if (step < 3)
				{
					SmallImage.drawSmallImage(g, 540, xSkill[0] + 14, ySkill + 14, 0, StaticObj.VCENTER_HCENTER);
				}
				else
				{
					g.drawImage(imgReplay, xSkill[0] + 14 - 10, ySkill + 14 - 10, 0);
				}
				g.drawImage(GameScr.imgSkill, xSkill[1], ySkill, 0);
				if (indexSkillSelect == 1)
				{
					g.drawImage(GameScr.imgSkill2, xSkill[1], ySkill, 0);
				}
				g.drawImage(imgX, xSkill[1] + 14 - 10, ySkill + 14 - 10, 0);
				if (step > 3)
				{
					GameCanvas.paintz.paintFrameSimple(xP, yP, wP, hP, g);
					int num8 = GameCanvas.hw - idItem.Length * 30 / 2;
					for (int l = 0; l < idItem.Length; l++)
					{
						SmallImage.drawSmallImage(g, idItem[l], num8 + 5 + l * 30, yP + 10, 0, 0);
					}
				}
				if (isAutoCrackBall)
				{
					g.drawImage(GameScr.imgSkill2, (xSkill[0] + xSkill[1]) / 2, ySkill, 0);
				}
				else
				{
					g.drawImage(GameScr.imgSkill, (xSkill[0] + xSkill[1]) / 2, ySkill, 0);
				}
				SmallImage.drawSmallImage(g, 4387, (xSkill[0] + xSkill[1]) / 2 + 14, ySkill + 14, 0, StaticObj.VCENTER_HCENTER);
			}
			catch (Exception)
			{
			}
		}

}
