using System;
using Assets.src.e;
using Assets.src.g;
using UnityEngine;

public partial class Char : IMapObject
{
	public void applyCharLevelPercent()
			{
				try
				{
					long num = 1L;
					long num2 = 0L;
					int num3 = 0;
					for (int num4 = GameScr.exps.Length - 1; num4 >= 0; num4--)
					{
						if (cPower >= GameScr.exps[num4])
						{
							num = ((num4 != GameScr.exps.Length - 1) ? (GameScr.exps[num4 + 1] - GameScr.exps[num4]) : 1);
							num2 = cPower - GameScr.exps[num4];
							num3 = num4;
							break;
						}
					}
					clevel = num3;
					cLevelPercent = (int)(num2 * 10000 / num);
				}
				catch (Exception ex)
				{
					Cout.LogError("Loi char level percent: " + ex.ToString());
				}
			}

	public string getStrLevel()
			{
				string text = strLevel[clevel] + "+" + cLevelPercent / 100 + "." + cLevelPercent % 100 + "%";
				if (text.Length > 23 && text.IndexOf("cấp ") >= 0)
				{
					text = Res.replace(text, "cấp ", "c");
				}
				return text;
			}

	public void setPowerInfo(string info, short p, short maxP, short sc)
			{
				powerPoint = p;
				strInfo = info;
				maxPowerPoint = maxP;
				secondPower = sc;
				lastS = (currS = mSystem.currentTimeMillis());
			}

}
