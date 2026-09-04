using System;
using Assets.src.e;
using Assets.src.g;
using UnityEngine;

public partial class Char : IMapObject
{
	public void useItem(int indexUI)
			{
				Item item = arrItemBag[indexUI];
				if (!item.isTypeBody())
				{
					return;
				}
				item.isLock = true;
				item.typeUI = 5;
				Item item2 = arrItemBody[item.template.type];
				arrItemBag[indexUI] = null;
				if (item2 != null)
				{
					item2.typeUI = 3;
					arrItemBody[item.template.type] = null;
					item2.indexUI = indexUI;
					arrItemBag[indexUI] = item2;
				}
				item.indexUI = item.template.type;
				arrItemBody[item.indexUI] = item;
				for (int i = 0; i < arrItemBody.Length; i++)
				{
					Item item3 = arrItemBody[i];
					if (item3 != null)
					{
						if (item3.template.type == 0)
						{
							body = item3.template.part;
						}
						else if (item3.template.type == 1)
						{
							leg = item3.template.part;
						}
					}
				}
			}

	public void searchItem()
			{
				int[] array = new int[4] { -1, -1, -1, -1 };
				if (itemFocus != null)
				{
					return;
				}
				for (int i = 0; i < GameScr.vItemMap.size(); i++)
				{
					ItemMap itemMap = (ItemMap)GameScr.vItemMap.elementAt(i);
					int num = Math.abs(myCharz().cx - itemMap.x);
					int num2 = Math.abs(myCharz().cy - itemMap.y);
					int num3 = ((num <= num2) ? num2 : num);
					if (num > 48 || num2 > 48 || (itemFocus != null && num3 >= array[3]))
					{
						continue;
					}
					if (GameScr.gI().auto != 0 && GameScr.gI().isBagFull())
					{
						if (itemMap.template.type == 9)
						{
							itemFocus = itemMap;
							array[3] = num3;
						}
					}
					else
					{
						itemFocus = itemMap;
						array[3] = num3;
					}
				}
			}

	public Char clone()
			{
				Char @char = new Char();
				@char.charID = charID;
				@char.cx = cx;
				@char.cy = cy;
				@char.cdir = cdir;
				if (arrItemBody != null)
				{
					@char.arrItemBody = new Item[arrItemBody.Length];
					for (int i = 0; i < arrItemBody.Length; i++)
					{
						if (arrItemBody[i] == null)
						{
							@char.arrItemBody[i] = null;
						}
						else
						{
							@char.arrItemBody[i] = arrItemBody[i].clone();
						}
					}
				}
				return @char;
			}

}
