using System;
using Assets.src.e;
using Assets.src.g;
using UnityEngine;

public partial class Char : IMapObject
{
	public static int[][][] CharInfo = new int[33][][]
			{
				new int[4][]
				{
					new int[3] { 0, -13, 34 },
					new int[3] { 1, -8, 10 },
					new int[3] { 1, -9, 16 },
					new int[3] { 1, -9, 45 }
				},
				new int[4][]
				{
					new int[3] { 0, -13, 35 },
					new int[3] { 1, -8, 10 },
					new int[3] { 1, -9, 17 },
					new int[3] { 1, -9, 46 }
				},
				new int[4][]
				{
					new int[3] { 1, -10, 33 },
					new int[3] { 2, -10, 11 },
					new int[3] { 2, -8, 16 },
					new int[3] { 1, -12, 49 }
				},
				new int[4][]
				{
					new int[3] { 1, -10, 32 },
					new int[3] { 3, -12, 10 },
					new int[3] { 3, -11, 15 },
					new int[3] { 1, -13, 47 }
				},
				new int[4][]
				{
					new int[3] { 1, -10, 34 },
					new int[3] { 4, -8, 11 },
					new int[3] { 4, -7, 17 },
					new int[3] { 1, -12, 47 }
				},
				new int[4][]
				{
					new int[3] { 1, -10, 34 },
					new int[3] { 5, -12, 11 },
					new int[3] { 5, -9, 17 },
					new int[3] { 1, -13, 49 }
				},
				new int[4][]
				{
					new int[3] { 1, -10, 33 },
					new int[3] { 6, -10, 10 },
					new int[3] { 6, -8, 16 },
					new int[3] { 1, -12, 47 }
				},
				new int[4][]
				{
					new int[3] { 0, -9, 36 },
					new int[3] { 7, -5, 17 },
					new int[3] { 7, -11, 25 },
					new int[3] { 1, -8, 49 }
				},
				new int[4][]
				{
					new int[3] { 0, -7, 35 },
					new int[3] { 0, -18, 22 },
					new int[3] { 7, -10, 25 },
					new int[3] { 1, -7, 48 }
				},
				new int[4][]
				{
					new int[3] { 1, -11, 35 },
					new int[3] { 10, -3, 25 },
					new int[3] { 12, -10, 26 },
					new int[3]
				},
				new int[4][]
				{
					new int[3] { 1, -11, 37 },
					new int[3] { 11, -3, 25 },
					new int[3] { 12, -11, 27 },
					new int[3]
				},
				new int[4][]
				{
					new int[3] { 0, -14, 34 },
					new int[3] { 12, -8, 21 },
					new int[3] { 9, -7, 31 },
					new int[3]
				},
				new int[4][]
				{
					new int[3] { 0, -12, 35 },
					new int[3] { 8, -5, 14 },
					new int[3] { 8, -15, 29 },
					new int[3] { 1, -9, 49 }
				},
				new int[4][]
				{
					new int[3] { 1, -9, 34 },
					new int[3] { 9, -12, 9 },
					new int[3] { 10, -7, 19 },
					new int[3]
				},
				new int[4][]
				{
					new int[3] { 1, -13, 34 },
					new int[3] { 9, -12, 9 },
					new int[3] { 11, -10, 19 },
					new int[3]
				},
				new int[4][]
				{
					new int[3] { 1, -8, 32 },
					new int[3] { 9, -12, 9 },
					new int[3] { 2, -6, 15 },
					new int[3]
				},
				new int[4][]
				{
					new int[3] { 1, -8, 32 },
					new int[3] { 9, -12, 9 },
					new int[3] { 13, -12, 16 },
					new int[3]
				},
				new int[4][]
				{
					new int[3] { 0, -10, 31 },
					new int[3] { 9, -12, 9 },
					new int[3] { 7, -13, 20 },
					new int[3]
				},
				new int[4][]
				{
					new int[3] { 0, -11, 32 },
					new int[3] { 9, -12, 9 },
					new int[3] { 8, -15, 26 },
					new int[3]
				},
				new int[4][]
				{
					new int[3] { 0, -9, 33 },
					new int[3] { 9, -12, 9 },
					new int[3] { 14, -8, 18 },
					new int[3]
				},
				new int[4][]
				{
					new int[3] { 0, -11, 33 },
					new int[3] { 9, -12, 9 },
					new int[3] { 15, -6, 19 },
					new int[3]
				},
				new int[4][]
				{
					new int[3] { 0, -16, 31 },
					new int[3] { 9, -12, 9 },
					new int[3] { 9, -8, 28 },
					new int[3]
				},
				new int[4][]
				{
					new int[3] { 0, -14, 34 },
					new int[3] { 1, -8, 10 },
					new int[3] { 8, -16, 28 },
					new int[3]
				},
				new int[4][]
				{
					new int[3] { 0, -8, 36 },
					new int[3] { 7, -5, 17 },
					new int[3] { 0, -5, 25 },
					new int[3]
				},
				new int[4][]
				{
					new int[3] { 0, -9, 31 },
					new int[3] { 9, -12, 9 },
					new int[3] { 0, -6, 20 },
					new int[3]
				},
				new int[4][]
				{
					new int[3] { 2, -9, 36 },
					new int[3] { 13, -5, 17 },
					new int[3] { 16, -11, 25 },
					new int[3]
				},
				new int[4][]
				{
					new int[3] { 1, -9, 34 },
					new int[3] { 8, -5, 13 },
					new int[3] { 10, -7, 19 },
					new int[3]
				},
				new int[4][]
				{
					new int[3] { 1, -13, 34 },
					new int[3] { 8, -5, 13 },
					new int[3] { 11, -10, 19 },
					new int[3]
				},
				new int[4][]
				{
					new int[3] { 1, -8, 32 },
					new int[3] { 8, -5, 13 },
					new int[3] { 2, -6, 15 },
					new int[3]
				},
				new int[4][]
				{
					new int[3] { 1, -8, 32 },
					new int[3] { 8, -5, 13 },
					new int[3] { 13, -12, 16 },
					new int[3]
				},
				new int[4][]
				{
					new int[3] { 0, -9, 33 },
					new int[3] { 8, -5, 13 },
					new int[3] { 14, -8, 18 },
					new int[3]
				},
				new int[4][]
				{
					new int[3] { 0, -11, 33 },
					new int[3] { 8, -5, 13 },
					new int[3] { 15, -6, 19 },
					new int[3]
				},
				new int[4][]
				{
					new int[3] { 0, -16, 32 },
					new int[3] { 8, -5, 13 },
					new int[3] { 9, -8, 29 },
					new int[3]
				}
			};

	public static int[] CHAR_WEAPONX = new int[11]
			{
				-2, -6, 22, 21, 19, 22, 10, -2, -2, 5,
				19
			};

	public static int[] CHAR_WEAPONY = new int[11]
			{
				9, 22, 25, 17, 26, 37, 36, 49, 50, 52,
				36
			};

	private static Char myChar;

	private static Char myPet;

	public static int[] listAttack;

	public static int[][] listIonC;

	public int cvyJump;

	private int indexUseSkill = -1;

	public int cxSend;

	public int cySend;

	public int cdirSend = 1;

	public int cxFocus;

	public int cyFocus;

	public int cactFirst = 5;

	public MyVector vMovePoints = new MyVector();

	public static string[][] inforClass = new string[2][]
			{
				new string[4] { "1", "1", "chiÃªu 1", "0" },
				new string[4] { "2", "2", "chiÃªu 2", "5" }
			};

	public static int[][] inforSkill = new int[10][]
			{
				new int[12]
				{
					1, 0, 1, 1000, 40, 1, 0, 20, 0, 0,
					0, 0
				},
				new int[12]
				{
					2, 1, 10, 1000, 100, 1, 0, 40, 0, 0,
					0, 0
				},
				new int[12]
				{
					2, 2, 11, 800, 100, 1, 0, 45, 0, 0,
					0, 0
				},
				new int[12]
				{
					2, 3, 12, 600, 100, 1, 0, 50, 0, 0,
					0, 0
				},
				new int[12]
				{
					2, 4, 13, 500, 100, 1, 0, 55, 0, 0,
					0, 0
				},
				new int[12]
				{
					3, 1, 14, 500, 100, 1, 0, 60, 0, 0,
					0, 0
				},
				new int[12]
				{
					3, 2, 14, 500, 100, 1, 0, 60, 0, 0,
					0, 0
				},
				new int[12]
				{
					3, 3, 14, 500, 100, 1, 0, 60, 0, 0,
					0, 0
				},
				new int[12]
				{
					3, 4, 14, 500, 100, 1, 0, 60, 0, 0,
					0, 0
				},
				new int[12]
				{
					3, 5, 14, 500, 100, 1, 0, 60, 0, 0,
					0, 0
				}
			};
}
