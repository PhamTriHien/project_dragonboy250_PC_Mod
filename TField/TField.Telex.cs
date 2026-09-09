using System;

public partial class TField
{
	public bool suspendTelex = false;

	private static readonly string[] TELEX_VOW = new string[]
	{
		"aáàảãạ", "ăắằẳẵặ", "âấầẩẫậ",
		"eéèẻẽẹ", "êếềểễệ", "iíìỉĩị",
		"oóòỏõọ", "ôốồổỗộ", "ơớờởỡợ",
		"uúùủũụ", "ưứừửữự", "yýỳỷỹỵ",
		"AÁÀẢÃẠ", "ĂẮẰẲẴẶ", "ÂẤẦẨẪẬ",
		"EÉÈẺẼẸ", "ÊẾỀỂỄỆ", "IÍÌỈĨỊ",
		"OÓÒỎÕỌ", "ÔỐỒỔỖỘ", "ƠỚỜỞỠỢ",
		"UÚÙỦŨỤ", "ƯỨỪỬỮỰ", "YÝỲỶỸỴ"
	};

	private static int TelexToneIndex(char lower)
	{
		switch (lower)
		{
		case 's': return 1;
		case 'f': return 2;
		case 'r': return 3;
		case 'x': return 4;
		case 'j': return 5;
		default: return -1;
		}
	}

	private static bool TelexFindVowel(char ch, out int table, out int idx)
	{
		for (int t = 0; t < TELEX_VOW.Length; t++)
		{
			int i = TELEX_VOW[t].IndexOf(ch);
			if (i >= 0)
			{
				table = t;
				idx = i;
				return true;
			}
		}
		table = -1;
		idx = -1;
		return false;
	}

	private void TelexFinish()
	{
		setPasswordTest();
		setOffset();
		if (kb != null)
		{
			kb.text = text;
		}
	}

	private bool tryTelexCompose(int keyCode)
	{
		try
		{
			if (!Main.isPC || inputType != INPUT_TYPE_ANY || suspendTelex)
			{
				return false;
			}
			if (keyCode < 32 || keyCode == 127 || keyCode > 0xFFFF)
			{
				return false;
			}
			char c = (char)keyCode;
			if (char.IsSurrogate(c) || char.IsControl(c))
			{
				return false;
			}
			if (caretPos < 0 || caretPos > text.Length)
			{
				caretPos = text.Length;
			}
			char lower = char.ToLowerInvariant(c);
			bool isUpper = char.IsUpper(c);
			if (caretPos > 0)
			{
				char prev = text[caretPos - 1];
				// 1. dd -> đ
				if (lower == 'd' && (prev == 'd' || prev == 'D'))
				{
					text = text.Substring(0, caretPos - 1) + ((prev == 'D') ? 'Đ' : 'đ') + text.Substring(caretPos);
					TelexFinish();
					return true;
				}
				// 2. Nguyên âm đôi: aa -> â, ee -> ê, oo -> ô
				if ((lower == 'a' && (prev == 'a' || prev == 'A'))
					|| (lower == 'e' && (prev == 'e' || prev == 'E'))
					|| (lower == 'o' && (prev == 'o' || prev == 'O')))
				{
					char rep = (lower == 'a') ? 'â' : ((lower == 'e') ? 'ê' : 'ô');
					if (prev == char.ToUpperInvariant(prev) && isUpper && char.IsLetter(prev))
					{
						rep = char.ToUpperInvariant(rep);
					}
					text = text.Substring(0, caretPos - 1) + rep + text.Substring(caretPos);
					TelexFinish();
					return true;
				}
				// 3. Phím w: a -> ă, o -> ơ, u -> ư (và ngược lại)
				if (lower == 'w')
				{
					string repW = null;
					switch (prev)
					{
					case 'a': repW = "ă"; break;
					case 'A': repW = "Ă"; break;
					case 'o': repW = "ơ"; break;
					case 'O': repW = "Ơ"; break;
					case 'u': repW = "ư"; break;
					case 'U': repW = "Ư"; break;
					case 'ă': repW = isUpper ? "AW" : "aw"; break;
					case 'Ă': repW = "AW"; break;
					case 'ơ': repW = isUpper ? "OW" : "ow"; break;
					case 'Ơ': repW = "OW"; break;
					case 'ư': repW = isUpper ? "UW" : "uw"; break;
					case 'Ư': repW = "UW"; break;
					}
					if (repW != null)
					{
						if (text.Length - 1 + repW.Length > maxTextLenght)
						{
							return false;
						}
						text = text.Substring(0, caretPos - 1) + repW + text.Substring(caretPos);
						caretPos += repW.Length - 1;
						TelexFinish();
						return true;
					}
					return false;
				}
			}
			// 4. Phím dấu: s f r x j bỏ dấu vào nguyên âm cuối của từ
			int tone = TelexToneIndex(lower);
			if (tone >= 0)
			{
				int start = caretPos - 1;
				while (start >= 0 && char.IsLetter(text[start]))
				{
					start--;
				}
				start++;
				int target = -1;
				int tTable = -1;
				for (int i = start; i < caretPos; i++)
				{
					int tt, ii;
					if (TelexFindVowel(text[i], out tt, out ii))
					{
						target = i;
						tTable = tt;
					}
				}
				if (target >= 0)
				{
					text = text.Substring(0, target) + TELEX_VOW[tTable][tone] + text.Substring(target + 1);
					TelexFinish();
					return true;
				}
				return false;
			}
			// 5. Phím z: xóa dấu nguyên âm cuối (đã trơn thì gõ z thường)
			if (lower == 'z')
			{
				int start2 = caretPos - 1;
				while (start2 >= 0 && char.IsLetter(text[start2]))
				{
					start2--;
				}
				start2++;
				int target2 = -1;
				int tTable2 = -1;
				int tIdx2 = -1;
				for (int i2 = start2; i2 < caretPos; i2++)
				{
					int tt2, ii2;
					if (TelexFindVowel(text[i2], out tt2, out ii2))
					{
						target2 = i2;
						tTable2 = tt2;
						tIdx2 = ii2;
					}
				}
				if (target2 >= 0 && tIdx2 > 0)
				{
					text = text.Substring(0, target2) + TELEX_VOW[tTable2][0] + text.Substring(target2 + 1);
					TelexFinish();
					return true;
				}
				return false;
			}
			return false;
		}
		catch
		{
			return false;
		}
	}
}
