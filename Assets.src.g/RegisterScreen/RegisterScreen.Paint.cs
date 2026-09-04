using System;

namespace Assets.src.g;

public partial class RegisterScreen
{
	public override void paint(mGraphics g)
		{
			GameCanvas.debug("PLG1", 1);
			GameCanvas.paintBGGameScr(g);
			GameCanvas.debug("PLG2", 2);
			int num = tfUser.y - 50;
			if (GameCanvas.h <= 220)
			{
				num += 5;
			}
			if (ChatPopup.currChatPopup != null || ChatPopup.serverChatPopUp != null)
			{
				return;
			}
			if (GameCanvas.currentDialog == null)
			{
				int num2 = ((GameCanvas.w < 200) ? 160 : 180);
				xLog = GameCanvas.hw - 120;
				int num3 = 110;
				yLog = (GameCanvas.h - num3) / 2;
				PopUp.paintPopUp(g, xLog, yLog, 240, num3, -1, isButton: true);
				if (GameCanvas.h > 160 && imgTitle != null)
				{
					g.drawImage(imgTitle, GameCanvas.hw, num, 3);
				}
				GameCanvas.debug("PLG4", 1);
				int num4 = 4;
				int num5 = num4 * 32 + 23 + 33;
				if (num5 >= GameCanvas.w)
				{
					num4--;
					num5 = num4 * 32 + 23 + 33;
				}
				tfUser.x = xLog + 10;
				tfUser.y = yLog + 15;
				tfSodt.x = tfUser.x;
				tfSodt.y = tfUser.y + 30;
				tfNgay.x = xLog + 10;
				tfNgay.y = tfSodt.y + 30;
				tfThang.x = tfNgay.x + 75;
				tfThang.y = tfNgay.y;
				tfNam.x = tfThang.x + 75;
				tfNam.y = tfThang.y;
				mFont.tahoma_7b_focus.drawString(g, "Cập nhật thông tin", GameCanvas.hw, yLog + 2, 2);
				tfUser.paint(g);
				tfSodt.paint(g);
				tfNgay.paint(g);
				tfThang.paint(g);
				tfNam.paint(g);
			}
			GameCanvas.resetTrans(g);
			string vERSION = GameMidlet.VERSION;
			g.setColor(GameCanvas.skyColor);
			g.fillRect(GameCanvas.w - 40, 4, 36, 11);
			mFont.tahoma_7_grey.drawString(g, vERSION, GameCanvas.w - 22, 4, mFont.CENTER);
			g.drawImage(GameCanvas.img18, 10, 10, 0);
			base.paint(g);
		}

}
