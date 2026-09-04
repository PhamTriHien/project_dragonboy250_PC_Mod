using System;
using Assets.src.g;

namespace Assets.src.f;

public partial class Controller2
{
	public static void readMessage(Message msg)
		{
			try
			{
				if (readMessage_Part1(msg.command, msg)) return;
				if (readMessage_Part2(msg.command, msg)) return;
			}
			catch (Exception ex4)
			{
				Res.outz("=====> Controller2 " + ex4.StackTrace);
			}
		}

}
