using System;

public static class ModKiteBroly
{
	public static bool isAutoKite = false;
	public static bool isKhinhCong = false;
	public static int safeDistance = 150;     // Khoảng cách an toàn duy trì (120, 150, 180, 200)
	public static int dangerDistance = 85;    // Ngưỡng áp sát kích hoạt né đấm
	public static bool autoAttackBroly = false; // Tự động đánh chưởng khi an toàn

	public static int khinhCongY = -1;
	public static string currentBrolyName = string.Empty;
	public static int currentBrolyDist = -1;
	public static string brolyStatusText = "Đang chờ...";

	private static long lastDodgeTime = 0;
	private static long lastAttackTime = 0;

	public static void ToggleAutoKite()
	{
		isAutoKite = !isAutoKite;
		if (isAutoKite)
		{
			isKhinhCong = true;
			Char me = Char.myCharz();
			if (me != null)
			{
				khinhCongY = me.cy;
			}
			GameScr.info1.addInfo("Auto Né Broly: BẬT (Khinh Công)", 0);
		}
		else
		{
			brolyStatusText = "Đang TẮT";
			GameScr.info1.addInfo("Auto Né Broly: TẮT", 0);
		}
		ModConfig.SaveConfig();
		SoundMn.gI().buttonClick();
	}

	public static void ToggleKhinhCong()
	{
		isKhinhCong = !isKhinhCong;
		if (isKhinhCong)
		{
			Char me = Char.myCharz();
			if (me != null)
			{
				khinhCongY = me.cy;
			}
			GameScr.info1.addInfo("Đứng Khinh Công: BẬT", 0);
		}
		else
		{
			khinhCongY = -1;
			GameScr.info1.addInfo("Đứng Khinh Công: TẮT", 0);
		}
		ModConfig.SaveConfig();
		SoundMn.gI().buttonClick();
	}

	public static void CycleSafeDistance()
	{
		if (safeDistance == 120) safeDistance = 150;
		else if (safeDistance == 150) safeDistance = 180;
		else if (safeDistance == 180) safeDistance = 200;
		else safeDistance = 120;

		ModConfig.SaveConfig();
		SoundMn.gI().buttonClick();
	}

	public static bool FindBroly(out int brolyX, out int brolyY, out object targetRef, out string name)
	{
		brolyX = -1;
		brolyY = -1;
		targetRef = null;
		name = string.Empty;

		// 1. Quét nhân vật Boss trong map
		if (GameScr.vCharInMap != null)
		{
			for (int i = 0; i < GameScr.vCharInMap.size(); i++)
			{
				Char c = (Char)GameScr.vCharInMap.elementAt(i);
				if (c == null || c.cHP <= 0 || c.statusMe == 14 || c.statusMe == 5) continue;
				string cname = c.cName;
				if (string.IsNullOrEmpty(cname)) continue;
				string lower = cname.ToLower();
				if (lower.Contains("broly") || lower.Contains("super broly"))
				{
					brolyX = c.cx;
					brolyY = c.cy;
					targetRef = c;
					name = cname;
					return true;
				}
			}
		}

		// 2. Quét quái Boss trong map
		if (GameScr.vMob != null)
		{
			for (int j = 0; j < GameScr.vMob.size(); j++)
			{
				Mob m = (Mob)GameScr.vMob.elementAt(j);
				if (m == null || m.hp <= 0 || m.status == 0 || m.status == 1) continue;
				string mname = (m.getTemplate() != null) ? m.getTemplate().name : string.Empty;
				if (string.IsNullOrEmpty(mname)) continue;
				string lower = mname.ToLower();
				if (lower.Contains("broly") || lower.Contains("super broly"))
				{
					brolyX = m.x;
					brolyY = m.y;
					targetRef = m;
					name = mname;
					return true;
				}
			}
		}

		return false;
	}

	public static void DoKhinhCong()
	{
		Char me = Char.myCharz();
		if (me == null || me.isDie) return;

		if (khinhCongY == -1)
		{
			khinhCongY = me.cy;
		}

		me.cy = khinhCongY;
		me.cvy = 0;
		me.delayFall = 15;
	}

	public static void AttackBroly(object target)
	{
		try
		{
			Char me = Char.myCharz();
			if (me == null || me.myskill == null) return;

			MyVector vMob = new MyVector();
			MyVector vChar = new MyVector();

			if (target is Char c)
			{
				vChar.addElement(c);
				Service.gI().sendPlayerAttack(vMob, vChar, 2);
			}
			else if (target is Mob m)
			{
				vMob.addElement(m);
				Service.gI().sendPlayerAttack(vMob, vChar, 1);
			}
		}
		catch
		{
		}
	}

	public static void Update()
	{
		if (!ModMenu.IsInGame()) return;
		Char me = Char.myCharz();
		if (me == null || me.isDie) return;

		long now = mSystem.currentTimeMillis();

		// Nếu bật đứng khinh công độc lập
		if (isKhinhCong && !isAutoKite)
		{
			DoKhinhCong();
		}

		if (!isAutoKite) return;

		int brolyX, brolyY;
		object targetRef;
		string brolyName;

		bool hasBroly = FindBroly(out brolyX, out brolyY, out targetRef, out brolyName);
		if (!hasBroly)
		{
			currentBrolyName = string.Empty;
			currentBrolyDist = -1;
			brolyStatusText = "Không có Broly trong map";
			if (isKhinhCong)
			{
				DoKhinhCong();
			}
			return;
		}

		int distX = Res.abs(me.cx - brolyX);
		currentBrolyDist = distX;
		currentBrolyName = brolyName;

		// Tự động focus vào Broly
		if (targetRef is Char targetChar)
		{
			me.charFocus = targetChar;
		}
		else if (targetRef is Mob targetMob)
		{
			me.mobFocus = targetMob;
		}

		// KIỂM TRA BROLY ÁP SÁT VÀO VÙNG NGUY HIỂM (TẦM ĐẤM CẬN CHIẾN)
		if (distX <= dangerDistance && now - lastDodgeTime > 350)
		{
			lastDodgeTime = now;
			int targetX;

			// Nếu nhân vật đang ở bên phải Broly: Lướt sang bên trái
			if (me.cx >= brolyX)
			{
				targetX = brolyX - safeDistance;
				// Kiểm tra biên map trái
				if (targetX < 30)
				{
					targetX = brolyX + safeDistance + 30;
				}
			}
			// Nếu nhân vật đang ở bên trái Broly: Lướt sang bên phải
			else
			{
				targetX = brolyX + safeDistance;
				// Kiểm tra biên map phải
				if (targetX > TileMap.pxw - 30)
				{
					targetX = brolyX - safeDistance - 30;
				}
			}

			// Đứng khinh công ở độ cao ngang hoặc cao hơn Broly một chút
			int targetY = brolyY - 20;
			if (targetY < 50) targetY = 50;
			if (targetY > TileMap.pxh - 50) targetY = TileMap.pxh - 50;
			khinhCongY = targetY;

			// Thực hiện dịch chuyển sang vị trí an toàn
			ModTeleport.TeleportTo(targetX, targetY);
			me.cdir = (targetX > brolyX) ? -1 : 1; // Luôn quay mặt về phía Broly
			me.cvy = 0;
			me.delayFall = 20;

			brolyStatusText = "Né đấm! Dịch sang " + ((targetX > brolyX) ? "Phải" : "Trái") + " (KC: " + Res.abs(targetX - brolyX) + "px)";
			SoundMn.gI().buttonClick();
		}
		else if (distX > dangerDistance)
		{
			// Ở cự ly an toàn: Broly chỉ có thể chưởng từ xa
			brolyStatusText = "An toàn (KC: " + distX + "px) - Broly chỉ chưởng từ xa";
			DoKhinhCong();
			me.cdir = (me.cx > brolyX) ? -1 : 1;

			// Tự động đánh chưởng nếu được bật
			if (autoAttackBroly && now - lastAttackTime > 450)
			{
				lastAttackTime = now;
				AttackBroly(targetRef);
			}
		}
	}
}
