using System;
public partial class Effect_End
{
	private void pnt_Pow(mGraphics g, int anchor)
			{
				if (fra_skill[0] != null)
				{
					if (nFrame != null)
					{
						fra_skill[0].drawFrame(nFrame[f % nFrame.Length], x, y, dir_nguoc, anchor, g);
					}
					else
					{
						fra_skill[0].drawFrame(f / n_frame % fra_skill[0].nFrame, x, y, dir_nguoc, anchor, g);
					}
				}
			}
	private void upd_Sub()
			{
				if (timeRemove > 0)
				{
					if (GameCanvas.timeNow - time >= timeRemove)
					{
						removeEff();
					}
				}
				else if (f >= fra_skill[0].nFrame * n_frame)
				{
					removeEff();
				}
			}
	private void pnt_Sub(mGraphics g, int anchor)
			{
				fra_skill[0].drawFrame(f / n_frame % fra_skill[0].nFrame, x, y, dir, anchor, g);
			}
	private void upd_()
			{
			}
	private void pnt_(mGraphics g)
			{
			}

}
