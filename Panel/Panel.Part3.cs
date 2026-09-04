using System;
using System.Collections.Generic;
using Assets.src.g;
using UnityEngine;
public partial class Panel : IActionListener, IChatable
{
	private bool IsTabOption()
		{
			if (size_tab > 0)
			{
				if (currentTabName.Length > 1)
				{
					if (selected == 0)
					{
						return true;
					}
				}
				else if (selected >= 0)
				{
					return true;
				}
			}
			return false;
		}
	private int checkCurrentListLength(int arrLength)
		{
			int num = 20;
			int num2 = arrLength / 20 + ((arrLength % 20 > 0) ? 1 : 0);
			size_tab = (sbyte)num2;
			if (newSelected > num2 - 1)
			{
				newSelected = num2 - 1;
			}
			if (arrLength % 20 > 0 && newSelected == num2 - 1)
			{
				num = arrLength % 20;
			}
			return num + 1;
		}

}
