using System;

public partial class CreateCharScr
{
	public override void paint(mGraphics g)
		{
			if (Char.isLoadingMap)
			{
				return;
			}
			g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
			GameCanvas.paintBGGameScr(g);
			g.translate(-GameScr.cmx, -GameScr.cmy);
			if (!GameCanvas.lowGraphic)
			{
				for (int i = 0; i < MapTemplate.vCurrItem[indexGender].size(); i++)
				{
					BgItem bgItem = (BgItem)MapTemplate.vCurrItem[indexGender].elementAt(i);
					if (bgItem.idImage != -1 && bgItem.layer == 1)
					{
						bgItem.paint(g);
					}
				}
			}
			TileMap.paintTilemap(g);
			int num = 30;
			if (GameCanvas.w == 128)
			{
				num = 20;
			}
			int headId = hairID[indexGender][indexHair];
			int legId = defaultLeg[indexGender];
			int bodyId = defaultBody[indexGender];
			if (GameScr.parts == null || GameScr.parts.Length == 0)
			{
				GameScr.gI().initSelectChar();
			}
			Part partHead = (GameScr.parts != null && headId >= 0 && headId < GameScr.parts.Length) ? GameScr.parts[headId] : null;
			Part partLeg = (GameScr.parts != null && legId >= 0 && legId < GameScr.parts.Length) ? GameScr.parts[legId] : null;
			Part partBody = (GameScr.parts != null && bodyId >= 0 && bodyId < GameScr.parts.Length) ? GameScr.parts[bodyId] : null;
			try
			{
				g.drawImage(TileMap.bong, cx, cy + dy, 3);
				// Thứ tự vẽ: Chân (Layer dưới) -> Thân (Layer giữa) -> Đầu (Layer trên cùng)
				if (partLeg != null && partLeg.pi != null && partLeg.pi.Length > Char.CharInfo[cf][1][0])
				{
					SmallImage.drawSmallImage(g, partLeg.pi[Char.CharInfo[cf][1][0]].id, cx + Char.CharInfo[cf][1][1] + partLeg.pi[Char.CharInfo[cf][1][0]].dx, cy - Char.CharInfo[cf][1][2] + partLeg.pi[Char.CharInfo[cf][1][0]].dy + dy, 0, 0);
				}
				if (partBody != null && partBody.pi != null && partBody.pi.Length > Char.CharInfo[cf][2][0])
				{
					SmallImage.drawSmallImage(g, partBody.pi[Char.CharInfo[cf][2][0]].id, cx + Char.CharInfo[cf][2][1] + partBody.pi[Char.CharInfo[cf][2][0]].dx, cy - Char.CharInfo[cf][2][2] + partBody.pi[Char.CharInfo[cf][2][0]].dy + dy, 0, 0);
				}
				if (partHead != null && partHead.pi != null && partHead.pi.Length > Char.CharInfo[cf][0][0])
				{
					SmallImage.drawSmallImage(g, partHead.pi[Char.CharInfo[cf][0][0]].id, cx + Char.CharInfo[cf][0][1] + partHead.pi[Char.CharInfo[cf][0][0]].dx, cy - Char.CharInfo[cf][0][2] + partHead.pi[Char.CharInfo[cf][0][0]].dy + dy, 0, 0);
				}
			}
			catch (Exception ex)
			{
				Res.outz("Error drawing character in CreateCharScr: " + ex.Message);
			}
			if (!GameCanvas.lowGraphic)
			{
				for (int j = 0; j < MapTemplate.vCurrItem[indexGender].size(); j++)
				{
					BgItem bgItem2 = (BgItem)MapTemplate.vCurrItem[indexGender].elementAt(j);
					if (bgItem2.idImage != -1 && bgItem2.layer == 3)
					{
						bgItem2.paint(g);
					}
				}
			}
			g.translate(-g.getTranslateX(), -g.getTranslateY());
			if (GameCanvas.w < 200)
			{
				GameCanvas.paintz.paintFrame(GameScr.popupX, GameScr.popupY, GameScr.popupW, GameScr.popupH, g);
				// Thứ tự vẽ: Chân -> Thân -> Đầu
				if (partLeg != null && partLeg.pi != null && partLeg.pi.Length > Char.CharInfo[0][1][0])
				{
					SmallImage.drawSmallImage(g, partLeg.pi[Char.CharInfo[0][1][0]].id, GameCanvas.w / 2 + Char.CharInfo[0][1][1] + partLeg.pi[Char.CharInfo[0][1][0]].dx, GameScr.popupY + 30 + 3 * num - Char.CharInfo[0][1][2] + partLeg.pi[Char.CharInfo[0][1][0]].dy + dy, 0, 0);
				}
				if (partBody != null && partBody.pi != null && partBody.pi.Length > Char.CharInfo[0][2][0])
				{
					SmallImage.drawSmallImage(g, partBody.pi[Char.CharInfo[0][2][0]].id, GameCanvas.w / 2 + Char.CharInfo[0][2][1] + partBody.pi[Char.CharInfo[0][2][0]].dx, GameScr.popupY + 30 + 3 * num - Char.CharInfo[0][2][2] + partBody.pi[Char.CharInfo[0][2][0]].dy + dy, 0, 0);
				}
				if (partHead != null && partHead.pi != null && partHead.pi.Length > Char.CharInfo[0][0][0])
				{
					SmallImage.drawSmallImage(g, partHead.pi[Char.CharInfo[0][0][0]].id, GameCanvas.w / 2 + Char.CharInfo[0][0][1] + partHead.pi[Char.CharInfo[0][0][0]].dx, GameScr.popupY + 30 + 3 * num - Char.CharInfo[0][0][2] + partHead.pi[Char.CharInfo[0][0][0]].dy + dy, 0, 0);
				}
				for (int k = 0; k < mResources.MENUNEWCHAR.Length; k++)
				{
					if (selected == k)
					{
						g.drawRegion(GameScr.arrow, 0, 0, 13, 16, 2, GameScr.popupX + 10 + ((GameCanvas.gameTick % 7 > 3) ? 1 : 0), GameScr.popupY + 35 + k * num, StaticObj.VCENTER_HCENTER);
						g.drawRegion(GameScr.arrow, 0, 0, 13, 16, 0, GameScr.popupX + GameScr.popupW - 10 - ((GameCanvas.gameTick % 7 > 3) ? 1 : 0), GameScr.popupY + 35 + k * num, StaticObj.VCENTER_HCENTER);
					}
					mFont.tahoma_7b_dark.drawString(g, mResources.MENUNEWCHAR[k], GameScr.popupX + 20, GameScr.popupY + 30 + k * num, 0);
				}
				mFont.tahoma_7b_dark.drawString(g, mResources.MENUGENDER[indexGender], GameScr.popupX + 70, GameScr.popupY + 30 + num, mFont.LEFT);
				mFont.tahoma_7b_dark.drawString(g, mResources.hairStyleName[indexGender][indexHair], GameScr.popupX + 55, GameScr.popupY + 30 + 2 * num, mFont.LEFT);
				tAddName.paint(g);
			}
			else
			{
				if (!Main.isPC)
				{
					if (mGraphics.addYWhenOpenKeyBoard != 0)
					{
						yButton = 110;
						disY = 60;
						if (GameCanvas.w > GameCanvas.h)
						{
							int dy3 = (partBody != null && partBody.pi != null && partBody.pi.Length > Char.CharInfo[0][2][0]) ? partBody.pi[Char.CharInfo[0][2][0]].dy : 0;
							yButton = GameScr.popupY + 30 + 3 * num + dy3 + dy - 15;
							disY = 35;
						}
					}
					else
					{
						yButton = 110;
						disY = 60;
						if (GameCanvas.w > GameCanvas.h)
						{
							yButton = 100;
							disY = 45;
						}
					}
					tAddName.y = yButton - tAddName.height - disY + 5;
				}
				else
				{
					yButton = 110;
					disY = 60;
					if (GameCanvas.w > GameCanvas.h)
					{
						yButton = 100;
						disY = 45;
					}
					tAddName.y = yBegin;
				}
				for (int l = 0; l < 3; l++)
				{
					int num5 = 78;
					if (l != indexGender)
					{
						g.drawImage(GameScr.imgLbtn, GameCanvas.w / 2 - num5 + l * num5, yButton, 3);
					}
					else
					{
						if (selected == 1)
						{
							g.drawRegion(GameScr.arrow, 0, 0, 13, 16, 4, GameCanvas.w / 2 - num5 + l * num5, yButton - 20 + ((GameCanvas.gameTick % 7 > 3) ? 1 : 0), StaticObj.VCENTER_HCENTER);
						}
						g.drawImage(GameScr.imgLbtnFocus, GameCanvas.w / 2 - num5 + l * num5, yButton, 3);
					}
					mFont.tahoma_7b_dark.drawString(g, mResources.MENUGENDER[l], GameCanvas.w / 2 - num5 + l * num5, yButton - 5, mFont.CENTER);
				}
				for (int m = 0; m < 3; m++)
				{
					int num6 = 78;
					if (m != indexHair)
					{
						g.drawImage(GameScr.imgLbtn, GameCanvas.w / 2 - num6 + m * num6, yButton + disY, 3);
					}
					else
					{
						if (selected == 2)
						{
							g.drawRegion(GameScr.arrow, 0, 0, 13, 16, 4, GameCanvas.w / 2 - num6 + m * num6, yButton + disY - 20 + ((GameCanvas.gameTick % 7 > 3) ? 1 : 0), StaticObj.VCENTER_HCENTER);
						}
						g.drawImage(GameScr.imgLbtnFocus, GameCanvas.w / 2 - num6 + m * num6, yButton + disY, 3);
					}
					mFont.tahoma_7b_dark.drawString(g, mResources.hairStyleName[indexGender][m], GameCanvas.w / 2 - num6 + m * num6, yButton + disY - 5, mFont.CENTER);
				}
				tAddName.paint(g);
				g.setClip(0, 0, GameCanvas.w, GameCanvas.h);

				// Vẽ nhân vật trực tiếp ở giữa màn hình bên dưới các nút chọn để người chơi thấy rõ ràng
				int charPreviewX = GameCanvas.w / 2;
				int charPreviewY = yButton + disY + 60;
				try
				{
					if (TileMap.bong != null)
					{
						g.drawImage(TileMap.bong, charPreviewX, charPreviewY, 3);
					}
					// Thứ tự vẽ: Chân (Layer dưới) -> Thân (Layer giữa) -> Đầu (Layer trên cùng)
					if (partLeg != null && partLeg.pi != null && partLeg.pi.Length > Char.CharInfo[cf][1][0])
					{
						SmallImage.drawSmallImage(g, partLeg.pi[Char.CharInfo[cf][1][0]].id, charPreviewX + Char.CharInfo[cf][1][1] + partLeg.pi[Char.CharInfo[cf][1][0]].dx, charPreviewY - Char.CharInfo[cf][1][2] + partLeg.pi[Char.CharInfo[cf][1][0]].dy, 0, 0);
					}
					if (partBody != null && partBody.pi != null && partBody.pi.Length > Char.CharInfo[cf][2][0])
					{
						SmallImage.drawSmallImage(g, partBody.pi[Char.CharInfo[cf][2][0]].id, charPreviewX + Char.CharInfo[cf][2][1] + partBody.pi[Char.CharInfo[cf][2][0]].dx, charPreviewY - Char.CharInfo[cf][2][2] + partBody.pi[Char.CharInfo[cf][2][0]].dy, 0, 0);
					}
					if (partHead != null && partHead.pi != null && partHead.pi.Length > Char.CharInfo[cf][0][0])
					{
						SmallImage.drawSmallImage(g, partHead.pi[Char.CharInfo[cf][0][0]].id, charPreviewX + Char.CharInfo[cf][0][1] + partHead.pi[Char.CharInfo[cf][0][0]].dx, charPreviewY - Char.CharInfo[cf][0][2] + partHead.pi[Char.CharInfo[cf][0][0]].dy, 0, 0);
					}
				}
				catch (Exception ex)
				{
					Res.outz("Error drawing character preview in CreateCharScr: " + ex.Message);
				}
			}
			g.setClip(0, 0, GameCanvas.w, GameCanvas.h);
			if (cmdSelectSv != null)
			{
				cmdSelectSv.paint(g);
			}
			if (!TouchScreenKeyboard.visible)
			{
				base.paint(g);
			}
		}

}
