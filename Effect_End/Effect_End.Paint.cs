using System;

public partial class Effect_End : Effect2
{
	public void paint(mGraphics g)
		{
			try
			{
				if (isRemove || f < 0)
				{
					return;
				}
				switch (typeEffect)
				{
				case 0:
				case 1:
				case 2:
					pnt_End_String(g);
					break;
				case 3:
					pnt_FireWork(g);
					break;
				case 17:
					pnt_Sub(g, mGraphics.VCENTER);
					break;
				case 16:
					if (typeSub == 0)
					{
						pnt_Sub(g, mGraphics.BOTTOM | mGraphics.HCENTER);
					}
					else
					{
						pnt_Sub(g, mGraphics.VCENTER | mGraphics.HCENTER);
					}
					break;
				case 18:
				case 19:
				case 20:
					pnt_Pow(g, mGraphics.BOTTOM | mGraphics.HCENTER);
					break;
				case 21:
				case 22:
				case 23:
					pnt_Gong(g, mGraphics.VCENTER | mGraphics.HCENTER);
					break;
				case 24:
					pnt_Skill_Kamex10(g);
					break;
				case 25:
					pnt_Skill_Destroy(g);
					break;
				case 26:
					pnt_Skill_MaFuba(g);
					break;
				case 9:
					pnt_LINE_IN(g);
					break;
				case 10:
				case 11:
					pnt_End_Rock(g);
					break;
				}
			}
			catch (Exception ex)
			{
				Res.err(ex.ToString());
				removeEff();
			}
		}

	public void paint_Arrow(mGraphics g, FrameImage frm, int index, int x, int y, int anchor, bool isCountFr)
		{
			if (frm != null)
			{
				int num = frm.nFrame / 3;
				if (num < 1)
				{
					num = 1;
				}
				int num2 = 0;
				int num3 = 3;
				if (frm.nFrame <= 6)
				{
					num2 = ((frm.nFrame <= 3) ? (f % num) : ((f / num3 % 2 != 0) ? 3 : 0));
				}
				else
				{
					num = 1;
					num2 = ((f / num3 - fMove > 8) ? 6 : ((f / num3 - fMove > 4) ? 3 : 0));
				}
				int idx = num * mImageArrow[index] + num2;
				if (frm.nFrame < 3)
				{
					idx = f / num3 % frm.nFrame;
				}
				if (isCountFr)
				{
					idx = f / num3 % frm.nFrame;
				}
				frm.drawFrame(idx, x, y, mXoayArrow[index], anchor, g);
			}
		}

}
