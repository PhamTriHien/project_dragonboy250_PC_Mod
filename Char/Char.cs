using System;
using Assets.src.e;
using Assets.src.g;
using UnityEngine;
public partial class Char : IMapObject
{
	public string xuStr;

	public string luongStr;

	public string luongKhoaStr;

	public long lastUpdateTime;

	public bool meLive;

	public bool isMask;

	public bool isTeleport;

	public bool isUsePlane;

	public int shadowX;

	public int shadowY;

	public int shadowLife;

	public bool isNhapThe;

	public PetFollow petFollow;

	public int rank;

	public const sbyte A_STAND = 1;

	public const sbyte A_RUN = 2;

	public const sbyte A_JUMP = 3;

	public const sbyte A_FALL = 4;

	public const sbyte A_DEADFLY = 5;

	public const sbyte A_NOTHING = 6;

	public const sbyte A_ATTK = 7;

	public const sbyte A_INJURE = 8;

	public const sbyte A_AUTOJUMP = 9;

	public const sbyte A_FLY = 10;

	public const sbyte SKILL_STAND = 12;

	public const sbyte SKILL_FALL = 13;

	public const sbyte A_DEAD = 14;

	public const sbyte A_HIDE = 15;

	public const sbyte A_RESETPOINT = 16;

	public static ChatPopup chatPopup;

	public long cPower;

	public Info chatInfo;

	public sbyte petStatus;

	public int cx = 24;

	public int cy = 24;

	public int cvx;

	public int cvy;

	public int cp1;

	public int cp2;

	public int cp3;

	public int statusMe = 5;

	public int cdir = 1;

	public int charID;

	public int cgender;

	public int ctaskId;

	public int menuSelect;

	public int cBonusSpeed;

	public int cspeed = 4;

	public int cCriticalFull;

	public int cCritDameFull;

	public int clevel;

	public int xReload;

	public int yReload;

	public int cyStartFall;

	public int saveStatus;

	public int eff5BuffHp;

	public int eff5BuffMp;

	public int cdameDown;

	public int cStr;

	public long cMP;

	public long cHP;

	public long cHPNew;

	public long cHPShow;

	public long cHPFull;

	public long cMPFull;

	public long cDamFull;

	public long cDefull;

	public long cGiamST;

	public long cLevelPercent;

	public long cTiemNang;

	public long cNangdong;

	public long damHP;

	public long damMP;

	public bool isMob;

	public bool isCrit;

	public bool isDie;

	public int pointUydanh;

	public int pointNon;

	public int pointVukhi;

	public int pointAo;

	public int pointLien;

	public int pointGangtay;

	public int pointNhan;

	public int pointQuan;

	public int pointNgocboi;

	public int pointGiay;

	public int pointPhu;

	public int countFinishDay;

	public int countLoopBoos;

	public int limitTiemnangso;

	public int limitKynangso;

	public short[] potential = new short[4];

	public string cName = string.Empty;

	public int clanID;

	public sbyte ctypeClan;

	public Clan clan;

	public sbyte role;

	public int cw = 22;

	public int ch = 32;

	public int chw = 11;

	public int chh = 16;

	public Command cmdMenu;

	public bool canFly = true;

	public bool cmtoChar;

	public bool me;

	public bool cFinishedAttack;

	public bool cchistlast;

	public bool isAttack;

	public bool isAttFly;

	public int cwpt;

	public int cwplv;

	public int cf;

	public int tick;

	public static bool fallAttack;

	public bool isJump;

	public bool autoFall;

	public bool attack = true;

	public long xu;

	public int xuInBox;

	public int yen;

	public int gold_lock;

	public int luong;

	public int luongKhoa;

	public NClass nClass;

	public Command endMovePointCommand;

	public MyVector vSkill = new MyVector();

	public MyVector vSkillFight = new MyVector();

	public MyVector vEff = new MyVector();

	public Skill myskill;

	public Task taskMaint;

	public bool paintName = true;

	public Archivement[] arrArchive;

	public Item[] arrItemBag;

	public Item[] arrItemBox;

	public Item[] arrItemBody;

	public Skill[] arrPetSkill;

	public Item[][] arrItemShop;

	public string[][] infoSpeacialSkill;

	public short[][] imgSpeacialSkill;

	public short cResFire;

	public short cResIce;

	public short cResWind;

	public short cMiss;

	public short cExactly;

	public short cFatal;

	public sbyte cPk;

	public sbyte cTypePk;

	public short cReactDame;

	public short sysUp;

	public short sysDown;

	public int avatar;

	public int skillTemplateId;

	public Mob mobFocus;

	public Mob mobMe;

	public int tMobMeBorn;

	public Npc npcFocus;

	public Char charFocus;

	public ItemMap itemFocus;

	public MyVector focus = new MyVector();

	public Mob[] attMobs;

	public Char[] attChars;

	public short[] moveFast;

	public int testCharId = -9999;

	public int killCharId = -9999;

	public sbyte resultTest;

	public int countKill;

	public int countKillMax;

	public bool isInvisiblez;

	public bool isShadown = true;

	public const sbyte PK_NORMAL = 0;

	public const sbyte PK_PHE = 1;

	public const sbyte PK_BANG = 2;

	public const sbyte PK_THIDAU = 3;

	public const sbyte PK_LUYENTAP = 4;

	public const sbyte PK_TUDO = 5;

	public MyVector taskOrders = new MyVector();

	public int cStamina;

	public static short[] idHead;

	public static short[] idAvatar;

	public int exp;

	public string[] strLevel;

	public string currStrLevel;

	public static Image eyeTraiDat = GameCanvas.loadImage("/mainImage/myTexture2dmat-trai-dat.png");

	public static Image eyeNamek = GameCanvas.loadImage("/mainImage/myTexture2dmat-namek.png");

	public bool isFreez;

	public bool isCharge;

	public int seconds;

	public int freezSeconds;

	public long last;

	public long cur;

	public long lastFreez;

	public long currFreez;

	public bool isFlyUp;

	public static MyVector vItemTime = new MyVector();

	public static short ID_NEW_MOUNT = 30000;

	public short idMount;

	public bool isHaveMount;

	public bool isMountVip;

	public bool isEventMount;

	public bool isSpeacialMount;

	public static Image imgMount_TD = GameCanvas.loadImage("/mainImage/myTexture2dthucuoi10.png");

	public static Image imgMount_NM = GameCanvas.loadImage("/mainImage/myTexture2dthucuoi20.png");

	public static Image imgMount_NM_1 = GameCanvas.loadImage("/mainImage/myTexture2dthucuoi21.png");

	public static Image imgMount_XD = GameCanvas.loadImage("/mainImage/myTexture2dthucuoi30.png");

	public static Image imgMount_TD_VIP = GameCanvas.loadImage("/mainImage/myTexture2dthucuoi11.png");

	public static Image imgMount_NM_VIP = GameCanvas.loadImage("/mainImage/myTexture2dthucuoi22.png");

	public static Image imgMount_NM_1_VIP = GameCanvas.loadImage("/mainImage/myTexture2dthucuoi23.png");

	public static Image imgMount_XD_VIP = GameCanvas.loadImage("/mainImage/myTexture2dthucuoi31.png");

	public static Image imgEventMount = GameCanvas.loadImage("/mainImage/myTexture2drong.png");

	public static Image imgEventMountWing = GameCanvas.loadImage("/mainImage/myTexture2dcanhrong.png");

	public sbyte[] FrameMount = new sbyte[8] { 0, 0, 1, 1, 2, 2, 1, 1 };

	public int frameMount;

	public int frameNewMount;

	public int transMount;

	public int genderMount;

	public int idcharMount;

	public int xMount;

	public int yMount;

	public int dxMount;

	public int dyMount;

	public int xChar;

	public int xdis;

	public int speedMount;

	public bool isStartMount;

	public bool isMount;

	public bool isEndMount;

	public sbyte cFlag;

	public int flagImage;

	public short x_hint;

	public short y_hint;

	public short s_danhHieu1;

}
