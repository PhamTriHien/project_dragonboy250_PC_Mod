using System;
using Assets.src.e;
using Assets.src.g;
using UnityEngine;

public partial class Char : IMapObject
{
	public void createShadow(int x, int y, int life)
			{
				shadowX = x;
				shadowY = y;
				shadowLife = life;
			}

}
