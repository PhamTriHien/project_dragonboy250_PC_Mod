using System;
using UnityEngine;

public partial class Res
{
	public static int sin(int a)
		{
			a = fixangle(a);
			if (a >= 0 && a < 90)
			{
				return sinz[a];
			}
			if (a >= 90 && a < 180)
			{
				return sinz[180 - a];
			}
			if (a >= 180 && a < 270)
			{
				return -sinz[a - 180];
			}
			return -sinz[360 - a];
		}

	public static int cos(int a)
		{
			a = fixangle(a);
			if (a >= 0 && a < 90)
			{
				return cosz[a];
			}
			if (a >= 90 && a < 180)
			{
				return -cosz[180 - a];
			}
			if (a >= 180 && a < 270)
			{
				return -cosz[a - 180];
			}
			return cosz[360 - a];
		}

	public static int tan(int a)
		{
			a = fixangle(a);
			if (a >= 0 && a < 90)
			{
				return tanz[a];
			}
			if (a >= 90 && a < 180)
			{
				return -tanz[180 - a];
			}
			if (a >= 180 && a < 270)
			{
				return tanz[a - 180];
			}
			return -tanz[360 - a];
		}

	public static int atan(int a)
		{
			for (int i = 0; i <= 90; i++)
			{
				if (tanz[i] >= a)
				{
					return i;
				}
			}
			return 0;
		}

	public static int angle(int dx, int dy)
		{
			int num;
			if (dx != 0)
			{
				int a = Math.abs((dy << 10) / dx);
				num = atan(a);
				if (dy >= 0 && dx < 0)
				{
					num = 180 - num;
				}
				if (dy < 0 && dx < 0)
				{
					num = 180 + num;
				}
				if (dy < 0 && dx >= 0)
				{
					num = 360 - num;
				}
			}
			else
			{
				num = ((dy <= 0) ? 270 : 90);
			}
			return num;
		}

	public static int fixangle(int angle)
		{
			if (angle >= 360)
			{
				angle -= 360;
			}
			if (angle < 0)
			{
				angle += 360;
			}
			return angle;
		}

	public static int xetVX(int goc, int d)
		{
			return cos(fixangle(goc)) * d >> 10;
		}

	public static int xetVY(int goc, int d)
		{
			return sin(fixangle(goc)) * d >> 10;
		}

	public static int random(int a, int b)
		{
			if (a == b)
			{
				return a;
			}
			return a + r.nextInt(b - a);
		}

	public static int random(int a)
		{
			return r.nextInt(a);
		}

	public static int random_Am(int a, int b)
		{
			int num = a + r.nextInt(b - a);
			if (random(2) == 0)
			{
				num = -num;
			}
			return num;
		}

	public static int random_Am_0(int a)
		{
			int num;
			for (num = 0; num == 0; num = r.nextInt() % a)
			{
			}
			return num;
		}

	public static int s2tick(int currentTimeMillis)
		{
			int num = 0;
			num = currentTimeMillis * 16 / 1000;
			if (currentTimeMillis * 16 % 1000 >= 5)
			{
				num++;
			}
			return num;
		}

	public static int distance(int x1, int y1, int x2, int y2)
		{
			return sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
		}

	public static int getDistance(int x, int y)
		{
			return sqrt(x * x + y * y);
		}

	public static int sqrt(int a)
		{
			if (a <= 0)
			{
				return 0;
			}
			int num = (a + 1) / 2;
			int num2;
			do
			{
				num2 = num;
				num = num / 2 + a / (2 * num);
			}
			while (Math.abs(num2 - num) > 1);
			return num;
		}

	public static int rnd(int a)
		{
			return r.nextInt(a);
		}

	public static int abs(int i)
		{
			return (i <= 0) ? (-i) : i;
		}

	public static bool inRect(int x1, int y1, int width, int height, int x2, int y2)
		{
			return x2 >= x1 && x2 <= x1 + width && y2 >= y1 && y2 <= y1 + height;
		}

}
