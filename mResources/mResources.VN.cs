
public partial class mResources
{
	public static void loadLanguague()
		{
			loadLanguague(0);
		}

	public static void loadLanguague(sbyte newLanguage)
		{
			language = 0;
			if (LoginScr.imgTitle == null)
			{
				Image customLogo = GameCanvas.loadCustomImage("custom_logo.png");
				if (customLogo != null)
				{
					LoginScr.imgTitle = customLogo;
				}
				else
				{
					LoginScr.imgTitle = GameCanvas.loadImage("/mainImage/logo1.png");
				}
			}
			T1.load();
			ServerListScreen.linkweb = "http://ngocrongonline.com";
		}

}
