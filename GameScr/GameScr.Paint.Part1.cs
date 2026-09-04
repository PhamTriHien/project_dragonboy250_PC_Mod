using System;
using Assets.src.g;
public partial class GameScr : mScreen, IChatable
{
	public static void paintOngMauPercent(Image img0, Image img1, Image img2, float x, float y, int size, float pixelPercent, mGraphics g)
			{
				int clipX = g.getClipX();
				int clipY = g.getClipY();
				int clipWidth = g.getClipWidth();
				int clipHeight = g.getClipHeight();
				g.setClip((int)x, (int)y, (int)pixelPercent, 13);
				int num = size / 15 - 2;
				for (int i = 0; i < num; i++)
				{
					g.drawImage(img1, x + (float)((i + 1) * 15), y, 0);
				}
				g.drawImage(img0, x, y, 0);
				g.drawImage(img1, x + (float)size - 30f, y, 0);
				g.drawImage(img2, x + (float)size - 15f, y, 0);
				g.setClip(clipX, clipY, clipWidth, clipHeight);
			}
	public void paintEffect(mGraphics g)
			{
				for (int i = 0; i < Effect2.vEffect2.size(); i++)
				{
					Effect2 effect = (Effect2)Effect2.vEffect2.elementAt(i);
					if (effect != null && !(effect is ChatPopup))
					{
						effect.paint(g);
					}
				}
				if (!GameCanvas.lowGraphic)
				{
					for (int i = 0; i < Effect2.vAnimateEffect.size(); i++)
					{
						Effect2 effect2 = (Effect2)Effect2.vAnimateEffect.elementAt(i);
						effect2.paint(g);
					}
				}
				for (int i = 0; i < Effect2.vEffect2Outside.size(); i++)
				{
					Effect2 effect3 = (Effect2)Effect2.vEffect2Outside.elementAt(i);
					effect3.paint(g);
				}
			}
	public void paintBgItem(mGraphics g, int layer)
			{
				if (ModMenu.graphicsQuality == 3)
				{
					return;
				}
				for (int i = 0; i < TileMap.vCurrItem.size(); i++)
				{
					BgItem bgItem = (BgItem)TileMap.vCurrItem.elementAt(i);
					if (bgItem.idImage != -1 && bgItem.layer == layer)
					{
						bgItem.paint(g);
					}
				}
				if (TileMap.mapID == 48 && layer == 3 && GameCanvas.bgW != null && GameCanvas.bgW[0] != 0)
				{
					for (int j = 0; j < TileMap.pxw / GameCanvas.bgW[0] + 1; j++)
					{
						g.drawImage(GameCanvas.imgBG[0], j * GameCanvas.bgW[0], TileMap.pxh - GameCanvas.bgH[0] - 70, 0);
					}
				}
			}
	public void paintBlackSky(mGraphics g)
			{
				if (!GameCanvas.lowGraphic)
				{
					g.fillTrans(imgTrans, 0, 0, GameCanvas.w, GameCanvas.h);
				}
			}

}
