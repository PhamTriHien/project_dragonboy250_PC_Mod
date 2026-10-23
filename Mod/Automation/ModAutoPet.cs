using System;

/// <summary>
/// Module Auto Đệ Tử: Quản lý trạng thái đệ tử (đi theo, bảo vệ, tấn công, về nhà), HUD chỉ số đệ tử, né Boss, tự bật cờ đen và tự trị thương Namếc.
/// </summary>
public static class ModAutoPet
{
	public static bool isEnabled;
	public static int status = 2;
	public static int intervalMs = 5000;
	public static bool showPetHUD = false;
	public static bool petAvoidBoss = false;
	public static bool petAutoFlag = false;
	public static bool autoNamekHeal = false;
	public static long exitOnPetPowerLimit = 0;

	private static long nextAt;
	private static long lastSilentPetInfoTime;
	private static long silentPetInfoExpireAt;
	private static long lastFlagTime;
	private static long lastHealTime;
	private static bool isEvadingBoss;

	private static readonly string[] petStatusNames = new string[] { "Đi Theo", "Bảo Vệ", "Tấn Công", "Về Nhà" };

	public static void CyclePetPowerLimit()
	{
		if (exitOnPetPowerLimit <= 0) exitOnPetPowerLimit = 149_900_000L;
		else if (exitOnPetPowerLimit == 149_900_000L) exitOnPetPowerLimit = 1_499_000_000L;
		else if (exitOnPetPowerLimit == 1_499_000_000L) exitOnPetPowerLimit = 17_990_000_000L;
		else exitOnPetPowerLimit = 0;
		ModConfig.SaveConfig();
		SoundMn.gI().buttonClick();
	}

	public static string GetPetPowerLimitLabel()
	{
		if (exitOnPetPowerLimit <= 0) return "Mốc SM: Tắt";
		if (exitOnPetPowerLimit == 149_900_000L) return "Mốc: 149.9Tr";
		if (exitOnPetPowerLimit == 1_499_000_000L) return "Mốc: 1.49T";
		if (exitOnPetPowerLimit == 17_990_000_000L) return "Mốc: 17.99T";
		return "Mốc: " + Res.formatNumber2(exitOnPetPowerLimit);
	}

	/// <summary>
	/// Kiểm tra và tiêu thụ cờ yêu cầu thông tin đệ tử ngầm (tránh mở bảng hành trang đệ tử).
	/// </summary>
	public static bool ConsumeSilentPetInfoRequest()
	{
		long now = mSystem.currentTimeMillis();
		if (silentPetInfoExpireAt > now)
		{
			silentPetInfoExpireAt = 0;
			return true;
		}
		return false;
	}

	/// <summary>
	/// Cấu hình tham số và thiết lập hoạt động cho module.
	/// </summary>
	public static void Configure(int value, int interval)
	{
		status = value < 0 ? 0 : (value > 3 ? 3 : value);
		intervalMs = interval < 1000 ? 1000 : (interval > 3600000 ? 3600000 : interval);
		isEnabled = true;
		nextAt = 0;
		ModConfig.SaveConfig();
	}

	/// <summary>
	/// Bật hoặc tắt trạng thái kích hoạt của Toggle.
	/// </summary>
	public static void Toggle()
	{
		if (status < 0 || status > 3) status = 2;
		isEnabled = !isEnabled;
		nextAt = 0;
		ModConfig.SaveConfig();
		GameScr.info1.addInfo("Auto Đệ Tử: " + (isEnabled ? "BẬT" : "TẮT"), 0);
	}

	/// <summary>
	/// Tìm thực thể Đệ Tử của người chơi trong bản đồ hiện tại.
	/// </summary>
	public static Char FindMyPetInMap(Char me)
	{
		if (me == null || !me.havePet || GameScr.vCharInMap == null)
		{
			return null;
		}
		Char myPet = Char.myPetz();
		for (int i = 0; i < GameScr.vCharInMap.size(); i++)
		{
			Char c = (Char)GameScr.vCharInMap.elementAt(i);
			if (c == null) continue;
			if (c.charID == -me.charID)
			{
				return c;
			}
			if (myPet != null && !string.IsNullOrEmpty(myPet.cName) && !string.IsNullOrEmpty(c.cName) && c.cName.EndsWith(myPet.cName))
			{
				return c;
			}
		}
		return null;
	}

	private static Skill FindNamekHealSkill(Char me)
	{
		if (me == null) return null;
		if (GameScr.keySkill != null)
		{
			for (int i = 0; i < GameScr.keySkill.Length; i++)
			{
				Skill s = GameScr.keySkill[i];
				if (s != null && s.template != null && s.template.id == 7) return s;
			}
		}
		if (GameScr.onScreenSkill != null)
		{
			for (int i = 0; i < GameScr.onScreenSkill.Length; i++)
			{
				Skill s = GameScr.onScreenSkill[i];
				if (s != null && s.template != null && s.template.id == 7) return s;
			}
		}
		if (me.vSkill != null)
		{
			for (int i = 0; i < me.vSkill.size(); i++)
			{
				Skill s = (Skill)me.vSkill.elementAt(i);
				if (s != null && s.template != null && s.template.id == 7) return s;
			}
		}
		return null;
	}

	/// <summary>
	/// Cập nhật trạng thái logic của đối tượng theo từng chu kỳ khung hình.
	/// </summary>
	public static void Update()
	{
		if (!ModMenu.IsInGame()) return;
		Char me = Char.myCharz();
		if (me == null || !me.havePet || Char.isLoadingMap || Char.ischangingMap) return;

		long now = mSystem.currentTimeMillis();

		Char petInMap = FindMyPetInMap(me);
		if (petInMap != null)
		{
			Char myPet = Char.myPetz();
			if (myPet != null)
			{
				myPet.cHP = petInMap.cHP;
				if (petInMap.cHPFull > 0)
				{
					myPet.cHPFull = petInMap.cHPFull;
				}
			}
		}

		if ((showPetHUD || exitOnPetPowerLimit > 0) && (GameCanvas.panel == null || !GameCanvas.panel.isShow) && GameCanvas.currentDialog == null && !ModUI.uiCustomOpen)
		{
			if (now - lastSilentPetInfoTime >= 12000)
			{
				lastSilentPetInfoTime = now;
				silentPetInfoExpireAt = now + 2500;
				Service.gI().petInfo();
			}
		}

		// 0. Kiểm tra mốc sức mạnh đệ tử để tự động thu đệ về nhà & dừng Tàn Sát (chống lố mốc mở skill)
		if (exitOnPetPowerLimit > 0)
		{
			Char myPetCheck = Char.myPetz();
			if (myPetCheck != null && myPetCheck.cPower >= exitOnPetPowerLimit)
			{
				Service.gI().petStatus(3);
				myPetCheck.petStatus = 3;
				status = 3;
				if (ModTanSat.autoTanSat)
				{
					ModTanSat.autoTanSat = false;
				}
				exitOnPetPowerLimit = 0;
				ModConfig.SaveConfig();
				GameScr.info1.addInfo("Đệ tử đã đạt mốc SM (" + Res.formatNumber2(myPetCheck.cPower) + ")! Đã cho đệ về nhà & dừng Tàn Sát!", 0);
				SoundMn.gI().buttonClick();
				return;
			}
		}

		// 1. Tự động cho Đệ về nhà né Boss khi phát hiện Boss trong khu
		if (petAvoidBoss)
		{
			Char liveBoss = ModBossNotice.FindLiveBossInMap();
			if (liveBoss != null)
			{
				if (!isEvadingBoss || now >= nextAt)
				{
					isEvadingBoss = true;
					Service.gI().petStatus(3);
					nextAt = now + 2500;
					if (Char.myPetz() != null) Char.myPetz().petStatus = 3;
				}
				if (petAutoFlag && me.cFlag != 0 && now - lastFlagTime >= 3000)
				{
					lastFlagTime = now;
					Service.gI().getFlag(1, 0);
				}
				return;
			}
			else if (isEvadingBoss)
			{
				isEvadingBoss = false;
				nextAt = 0;
			}
		}

		// 2. Tự động bật Cờ Đen (flag 8) khi úp đệ tử
		if (petAutoFlag && !me.isDie && me.cHP > 0 && me.cFlag != 8 && now - lastFlagTime >= 5000)
		{
			lastFlagTime = now;
			Service.gI().getFlag(1, 8);
		}

		// 3. Tự động dùng Trị Thương (Namếc - Skill ID 7) cứu/bơm máu cho Đệ Tử
		if (autoNamekHeal && me.cgender == 1 && !me.isDie && me.cHP > 0 && petInMap != null && now - lastHealTime >= 1200)
		{
			bool needHeal = petInMap.isDie || petInMap.cHP <= 0 || (petInMap.cHPFull > 0 && petInMap.cHP * 100L / petInMap.cHPFull <= 35);
			if (needHeal)
			{
				Skill healSkill = FindNamekHealSkill(me);
				if (healSkill != null && !healSkill.paintCanNotUseSkill && now - healSkill.lastTimeUseThisSkill >= healSkill.coolDown)
				{
					lastHealTime = now;
					healSkill.lastTimeUseThisSkill = now;
					Skill prevSkill = me.myskill;
					Service.gI().selectSkill(7);
					MyVector vMob = new MyVector();
					MyVector vChar = new MyVector();
					vChar.addElement(petInMap);
					Service.gI().sendPlayerAttack(vMob, vChar, 2);
					if (prevSkill != null && prevSkill.template != null && prevSkill.template.id != 7)
					{
						Service.gI().selectSkill(prevSkill.template.id);
					}
				}
			}
		}

		// 4. Duy trì trạng thái lệnh Đệ Tử
		if (!isEnabled) return;
		if (status < 0 || status > 3) status = 2;
		if (now < nextAt) return;
		Service.gI().petStatus((sbyte)status);
		if (Char.myPetz() != null) Char.myPetz().petStatus = (sbyte)status;
		nextAt = now + intervalMs;
	}

	/// <summary>
	/// Hiển thị bảng thông số Đệ Tử thời gian thực trên màn hình (HUD Đệ Tử).
	/// </summary>
	public static void PaintPetHUD(mGraphics g)
	{
		if (!showPetHUD || !ModMenu.IsInGame()) return;
		if ((GameCanvas.panel != null && GameCanvas.panel.isShow) ||
		    (GameCanvas.panel2 != null && GameCanvas.panel2.isShow) ||
		    (GameCanvas.menu != null && GameCanvas.menu.showMenu) ||
		    GameCanvas.currentDialog != null ||
		    ModUI.uiCustomOpen)
		{
			return;
		}

		Char me = Char.myCharz();
		if (me == null || !me.havePet) return;
		Char pet = Char.myPetz();
		if (pet == null) return;

		try
		{
			g.translate(-g.getTranslateX(), -g.getTranslateY());
			g.setClip(0, 0, GameCanvas.w, GameCanvas.h);

			int x = 6 + SafeArea.Left;
			int y = 76;

			string petName = string.IsNullOrEmpty(pet.cName) ? "Đệ Tử" : pet.cName;
			int stIdx = (pet.petStatus >= 0 && pet.petStatus < petStatusNames.Length) ? pet.petStatus : status;
			string stLabel = (stIdx >= 0 && stIdx < petStatusNames.Length) ? petStatusNames[stIdx] : "Tấn Công";
			if (isEvadingBoss) stLabel = "Né Boss";

			string line1 = "[Đệ] " + petName + " (" + stLabel + ")";
			string line2 = "HP: " + Res.formatNumber2(pet.cHP) + "/" + Res.formatNumber2(pet.cHPFull) + " | TL: " + pet.cStamina;
			string line3 = "SM: " + Res.formatNumber2(pet.cPower) + " | SĐ: " + Res.formatNumber2(pet.cDamFull);

			(mFont.tahoma_7b_yellow ?? mFont.tahoma_7_yellow).drawString(g, line1, x, y, mFont.LEFT);
			mFont.tahoma_7_green2.drawString(g, line2, x, y + 11, mFont.LEFT);
			mFont.tahoma_7_white.drawString(g, line3, x, y + 22, mFont.LEFT);
		}
		catch
		{
		}
	}
}
