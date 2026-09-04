using System;
using Assets.src.e;
using Assets.src.f;
using Assets.src.g;
using UnityEngine;

public partial class Controller : IMessageHandler
{
	private void readGetImgByName(Message msg)
		{
			try
			{
				string name = msg.reader().readUTF();
				sbyte nFrame = msg.reader().readByte();
				sbyte[] array = null;
				array = NinjaUtil.readByteArray(msg);
				Image img = createImage(array);
				ImgByName.SetImage(name, img, nFrame);
				if (array == null)
				{
				}
			}
			catch (Exception)
			{
			}
		}

	private void readFrameBoss(Message msg, int mobTemplateId)
		{
			try
			{
				int num = msg.reader().readByte();
				int[][] array = new int[num][];
				for (int i = 0; i < num; i++)
				{
					int num2 = msg.reader().readByte();
					array[i] = new int[num2];
					for (int j = 0; j < num2; j++)
					{
						array[i][j] = msg.reader().readByte();
					}
				}
				frameHT_NEWBOSS.put(mobTemplateId + string.Empty, array);
			}
			catch (Exception)
			{
			}
		}

	public void phuban_Info(Message msg)
		{
			try
			{
				sbyte b = msg.reader().readByte();
				if (b == 0)
				{
					readPhuBan_CHIENTRUONGNAMEK(msg, b);
				}
			}
			catch (Exception)
			{
			}
		}

	private void readPhuBan_CHIENTRUONGNAMEK(Message msg, int type_PB)
		{
			try
			{
				sbyte b = msg.reader().readByte();
				if (b == 0)
				{
					short idmapPaint = msg.reader().readShort();
					string nameTeam = msg.reader().readUTF();
					string nameTeam2 = msg.reader().readUTF();
					int maxPoint = msg.reader().readInt();
					short timeSecond = msg.reader().readShort();
					int maxLife = msg.reader().readByte();
					GameScr.phuban_Info = new InfoPhuBan(type_PB, idmapPaint, nameTeam, nameTeam2, maxPoint, timeSecond);
					GameScr.phuban_Info.maxLife = maxLife;
					GameScr.phuban_Info.updateLife(type_PB, 0, 0);
				}
				else if (b == 1)
				{
					int pointTeam = msg.reader().readInt();
					int pointTeam2 = msg.reader().readInt();
					if (GameScr.phuban_Info != null)
					{
						GameScr.phuban_Info.updatePoint(type_PB, pointTeam, pointTeam2);
					}
				}
				else if (b == 2)
				{
					sbyte b2 = msg.reader().readByte();
					short type = 0;
					short num = -1;
					if (b2 == 1)
					{
						type = 1;
						num = 3;
					}
					else if (b2 == 2)
					{
						type = 2;
					}
					num = -1;
					GameScr.phuban_Info = null;
					GameScr.addEffectEnd(type, num, 0, GameCanvas.hw, GameCanvas.hh, 0, 0, -1, null);
				}
				else if (b == 5)
				{
					short timeSecond2 = msg.reader().readShort();
					if (GameScr.phuban_Info != null)
					{
						GameScr.phuban_Info.updateTime(type_PB, timeSecond2);
					}
				}
				else if (b == 4)
				{
					int lifeTeam = msg.reader().readByte();
					int lifeTeam2 = msg.reader().readByte();
					if (GameScr.phuban_Info != null)
					{
						GameScr.phuban_Info.updateLife(type_PB, lifeTeam, lifeTeam2);
					}
				}
			}
			catch (Exception)
			{
			}
		}

}
