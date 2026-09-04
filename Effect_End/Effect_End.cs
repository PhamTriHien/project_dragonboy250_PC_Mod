using System;
public partial class Effect_End
{
	public const sbyte Lvlpaint_All = -1;

	public const sbyte Lvlpaint_Front = 0;

	public const sbyte Lvlpaint_Mid = 1;

	public const sbyte Lvlpaint_Mid_2 = 2;

	public const sbyte Lvlpaint_Behind = 3;

	public const short End_String_Lose = 0;

	public const short End_String_Win = 1;

	public const short End_String_Draw = 2;

	public const short End_FireWork = 3;

	public const short End_line_in = 9;

	public const short End_e8_rock = 10;

	public const short End_e8_ice = 11;

	public const short End_SUB_MaFuBa = 16;

	public const short End_SUB_Destroy = 17;

	public const short End_POW_Kamex10 = 18;

	public const short End_POW_Destroy = 19;

	public const short End_POW_MaFuBa = 20;

	public const short End_GONG_Kamex10 = 21;

	public const short End_GONG_Destroy = 22;

	public const short End_GONG_MaFuBa = 23;

	public const short End_Skill_Kamex10 = 24;

	public const short End_Skill_Destroy = 25;

	public const short End_Skill_MaFuBa = 26;

	private MyVector VecEffEnd = new MyVector("EffectEnd VecEffEnd");

	public FrameImage fraImgEff;

	public byte[] nFrame = new byte[10];

	public byte[] nFrame_2 = new byte[10];

	public int typePaint;

	public int typeEffect;

	public int typeSub;

	public int range;

	public short idEndeff;

	public int fRemove;

	public int fMove;

	public int n_frame;

	public int x;

	public int y;

	public int w;

	public int h;

	public int dir;

	public int dir_nguoc;

	public int levelPaint;

	public int f;

	public int frame;

	public int fSpeed;

	public int vx;

	public int vy;

	public int x1000;

	public int y1000;

	public int vx1000;

	public int vy1000;

	public int dy_throw;

	public int vMax;

	public int toX;

	public int toY;

	public int stt;

	public int dx;

	public int dy;

	public short timeRemove;

	public long time;

	public bool isRemove;

	public bool isAddSub;

	public Char charUse;

	public Point[] listObj;

	public Point target;

	public static short[][] arrInfoEff = new short[29][]
			{
				new short[3] { 68, 264, 4 },
				new short[3] { 30, 120, 4 },
				new short[3] { 66, 280, 4 },
				new short[3] { 0, 0, 1 },
				new short[3] { 111, 68, 2 },
				new short[3] { 90, 68, 2 },
				new short[3] { 125, 68, 2 },
				new short[3] { 47, 282, 6 },
				new short[3] { 10, 40, 4 },
				new short[3] { 92, 525, 7 },
				new short[3] { 62, 372, 6 },
				new short[3] { 80, 352, 4 },
				new short[3] { 80, 352, 4 },
				new short[3] { 80, 352, 4 },
				new short[3] { 72, 240, 3 },
				new short[3] { 20, 42, 3 },
				new short[3] { 65, 160, 4 },
				new short[3] { 50, 300, 6 },
				new short[3] { 84, 168, 2 },
				new short[3] { 90, 540, 6 },
				new short[3] { 180, 900, 6 },
				new short[3] { 62, 186, 3 },
				new short[3] { 34, 80, 4 },
				new short[3] { 140, 560, 4 },
				new short[3] { 64, 600, 6 },
				new short[3] { 36, 200, 5 },
				new short[3] { 35, 200, 5 },
				new short[3] { 50, 250, 5 },
				new short[3] { 50, 240, 6 }
			};

	public int life;

	public int goc_Arc;

	public int va;

	public int gocT_Arc;

	public byte[] mpaintone_Arrow = new byte[24]
			{
				12, 11, 10, 9, 8, 7, 6, 5, 4, 3,
				2, 1, 0, 23, 22, 21, 20, 19, 18, 17,
				16, 15, 14, 13
			};

	public byte[] mImageArrow = new byte[24]
			{
				0, 0, 2, 1, 1, 2, 0, 0, 2, 1,
				1, 2, 0, 0, 2, 1, 1, 2, 0, 0,
				2, 1, 1, 2
			};

	public byte[] mXoayArrow = new byte[24]
			{
				2, 2, 3, 3, 3, 4, 5, 5, 5, 5,
				5, 1, 0, 0, 0, 0, 0, 7, 6, 6,
				6, 6, 6, 2
			};

	private int rS;

	private int angleS;

	private int angleO;

	private int iAngleS;

	private int iDotS;

	private int[] xArgS;

	private int[] yArgS;

	private int[] xDotS;

	private int[] yDotS;

	public static int[][] colorStar = new int[3][]
			{
				new int[3] { 16310304, 16298056, 16777215 },
				new int[3] { 7045120, 12643960, 16777215 },
				new int[3] { 2407423, 11987199, 16777215 }
			};

	private int[] colorpaint;

	private int indexColorStar;

	private int xline;

	private int yline;

	private FrameImage[] fra_skill;




























}
