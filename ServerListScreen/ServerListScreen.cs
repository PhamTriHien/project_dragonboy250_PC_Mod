using System;
using UnityEngine;
public partial class ServerListScreen : mScreen, IActionListener
{
	public static string[] nameServer;

	public static string[] address;

	public static sbyte serverPriority;

	public static bool[] hasConnected;

	public static short[] port;

	public static int selected;

	public static bool isWait;

	public static Command cmdUpdateServer;

	public static sbyte[] language;

	public static sbyte[] typeSv;

	public static sbyte[] isNew;

	public static sbyte[] typeClass;

	public static Char[] listChar;

	public static bool isHaveChar;

	private Command[] cmd;

	private Command cmdCallHotline;

	private int nCmdPlay;

	public static Command cmdDeleteRMS;

	private int lY;

	public static string smartPhoneVN = "Vũ trụ 1:dragon1.teamobi.com:14445:0:0:0,Vũ trụ 2:dragon2.teamobi.com:14445:0:0:0,Vũ trụ 3:dragon3.teamobi.com:14445:0:0:0,Vũ trụ 4:dragon4.teamobi.com:14445:0:0:0,Vũ trụ 5:dragon5.teamobi.com:14445:0:0:0,Vũ trụ 6:dragon6.teamobi.com:14445:0:0:0,Vũ trụ 7:dragon7.teamobi.com:14445:0:0:0,Vũ trụ 8:dragon10.teamobi.com:14446:0:0:0,Vũ trụ 9:dragon10.teamobi.com:14447:0:0:0,Vũ trụ 10:dragon10.teamobi.com:14445:0:0:0,Vũ trụ 11:dragon11.teamobi.com:14445:0:0:0,Võ đài liên vũ trụ:dragonwar.teamobi.com:20000:0:0:0,Universe 1:dragon.indonaga.com:14445:1:0:0,Naga:dragon.indonaga.com:14446:2:0:0,0,0";

	public static string javaVN = "Vũ trụ 1:112.213.94.23:14445:0:0:0,Vũ trụ 2:210.211.109.199:14445:0:0:0,Vũ trụ 3:112.213.85.88:14445:0:0:0,Vũ trụ 4:27.0.12.164:14445:0:0:0,Vũ trụ 5:27.0.12.16:14445:0:0:0,Vũ trụ 6:27.0.12.173:14445:0:0:0,Vũ trụ 7:112.213.94.223:14445:0:0:0,Vũ trụ 8:27.0.14.66:14446:0:0:0,Vũ trụ 9:27.0.14.66:14447:0:0:0,Vũ trụ 10:27.0.14.66:14445:0:0:0,Vũ trụ 11:112.213.85.35:14445:0:0:0,Võ đài liên vũ trụ:27.0.12.173:20000:0:0:0,Universe 1:52.74.230.22:14445:1:0:0,Naga:52.74.230.22:14446:2:0:0,0,0";

	public static string smartPhoneIn = "Naga:dragon.indonaga.com:14446:2:0:0,2,0";

	public static string javaIn = "Naga:52.74.230.22:14446:2:0:0,2,0";

	public static string smartPhoneE = "Universe 1:dragon.indonaga.com:14445:1:0:0,1,0";

	public static string javaE = "Universe 1:52.74.230.22:14445:1:0:0,1,0";

	public static string linkGetHost = "http://112.213.94.23/mod/server_extra.php";

	public static string linkDefault = javaVN;

	public const sbyte languageVersion = 2;

	public new int keyTouch = -1;

	private int tam;

	public static bool stopDownload;

	public static string linkweb = "http://ngocrongonline.com";

	public static int countDieConnect;

	public static bool waitToLogin;

	public static int tWaitToLogin;

	public static long count_reConnect;

	public static string RMS_NRlink = "NRlink3";

	public static int ipSelect;

	public static int flagServer;

	public static bool bigOk;

	public static int percent;

	public static string strWait;

	public static int nBig;

	public static int nBg;

	public static int demPercent;

	public static int maxBg;

	public static bool isGetData = false;

	public static Command cmdDownload;

	private Command cmdStart;

	public string dataSize;

	public static int p;

	public static int testConnect = -1;

	public static bool loadScreen;

	public static bool isAutoConect = true;

	public static string RMS_svselect = "svselect";

	public static string RMS_NR_Extralink = "NRlink_extra";

	private Command[] cmd_New_Ui;

	public static bool isNewUI;

	public static bool isAutoLogin = true;


























}
