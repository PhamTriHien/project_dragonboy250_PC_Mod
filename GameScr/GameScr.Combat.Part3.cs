using System;
using Assets.src.g;
public partial class GameScr : mScreen, IChatable
{
	public static void updateFlyText()
		{
			for (int i = 0; i < 5; i++)
			{
				if (flyTextState[i] == -1)
				{
					continue;
				}
				if (flyTextState[i] > flyTextYTo[i])
				{
					flyTime[i]++;
					if (flyTime[i] == 25)
					{
						flyTime[i] = 0;
						flyTextState[i] = -1;
						flyTextYTo[i] = 0;
						flyTextDx[i] = 0;
						flyTextX[i] = 0;
					}
				}
				else
				{
					flyTextState[i] += Res.abs(flyTextDy[i]);
					flyTextX[i] += flyTextDx[i];
					flyTextY[i] += flyTextDy[i];
				}
			}
		}
	public static void loadSplash()
		{
			if (imgSplash == null)
			{
				imgSplash = new Image[3];
				for (int i = 0; i < 3; i++)
				{
					imgSplash[i] = GameCanvas.loadImage("/e/sp" + i + ".png");
				}
			}
			splashX = new int[2];
			splashY = new int[2];
			splashState = new int[2];
			splashF = new int[2];
			splashDir = new int[2];
			splashState[0] = (splashState[1] = -1);
		}
	public static bool startSplash(int x, int y, int dir)
		{
			int num = ((splashState[0] != -1) ? 1 : 0);
			if (splashState[num] != -1)
			{
				return false;
			}
			splashState[num] = 0;
			splashDir[num] = dir;
			splashX[num] = x;
			splashY[num] = y;
			return true;
		}
	public static void updateSplash()
		{
			for (int i = 0; i < 2; i++)
			{
				if (splashState[i] != -1)
				{
					splashState[i]++;
					splashX[i] += splashDir[i] << 2;
					splashY[i]--;
					if (splashState[i] >= 6)
					{
						splashState[i] = -1;
					}
					else
					{
						splashF[i] = (splashState[i] >> 1) % 3;
					}
				}
			}
		}
	public static void addEffectEnd(int type, int subtype, int typePaint, int x, int y, int levelPaint, int dir, short timeRemove, Point[] listObj)
		{
			Effect_End eff = new Effect_End(type, subtype, typePaint, x, y, levelPaint, dir, timeRemove, listObj);
			addEffect2Vector(eff);
		}
	public static void addEffectEnd_Target(int type, int subtype, int typePaint, Char charUse, Point target, int levelPaint, short timeRemove, short range)
		{
			Effect_End eff = new Effect_End(type, subtype, typePaint, charUse.clone(), target, levelPaint, timeRemove, range);
			addEffect2Vector(eff);
		}
	public static void addEffect2Vector(Effect_End eff)
		{
			if (eff.levelPaint == 0)
			{
				EffectManager.addHiEffect(eff);
			}
			else if (eff.levelPaint == 1)
			{
				EffectManager.addMidEffects(eff);
			}
			else if (eff.levelPaint == 2)
			{
				EffectManager.addMid_2Effects(eff);
			}
			else
			{
				EffectManager.addLowEffect(eff);
			}
		}

}
