using System;
using UnityEngine;

public partial class Res
{
	public static string changeString(string str)
		{
			if (string.IsNullOrEmpty(str))
			{
				return str;
			}
			try
			{
				for (int i = 0; i < translations.Length; i++)
				{
					if (str.IndexOf(translations[i][0], StringComparison.OrdinalIgnoreCase) >= 0)
					{
						str = ReplaceIgnoreCase(str, translations[i][0], translations[i][1]);
					}
				}
			}
			catch (Exception)
			{
			}
			return str;
		}

	private static string ReplaceIgnoreCase(string input, string pattern, string replacement)
		{
			if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(pattern))
			{
				return input;
			}
			int pos = 0;
			while (true)
			{
				int idx = input.IndexOf(pattern, pos, StringComparison.OrdinalIgnoreCase);
				if (idx < 0)
				{
					break;
				}
				input = input.Substring(0, idx) + replacement + input.Substring(idx + pattern.Length);
				pos = idx + replacement.Length;
			}
			return input;
		}

	public static string GetVietnameseOptionTemplate(int id, string defaultName)
		{
			switch (id)
			{
			case 0: return "Tấn công +#";
			case 1: return "Máu +#";
			case 2: return "KI +#";
			case 3: return "Chí mạng +#%";
			case 4: return "Giáp +#";
			case 5: return "Biến #% sát thương thành KI";
			case 6: return "HP +#k";
			case 7: return "KI +#k";
			case 8: return "Hút #% HP từ sát thương";
			case 9: return "Tăng #% tốc độ chạy";
			case 10: return "Hồi #% HP khi đánh quái";
			case 14: return "Chí mạng +#%";
			case 16: return "Tấn công quái +#";
			case 19: return "Tấn công +#%";
			case 22: return "HP +#k";
			case 23: return "KI +#k";
			case 27: return "Hồi #% HP và KI mỗi 30s";
			case 28: return "Hồi #% HP và KI khi đánh quái";
			case 30: return "Không thể giao dịch";
			case 33: return "Dịch chuyển tức thời";
			case 34: return "Nâng cấp sao pha lê";
			case 35: return "Sao pha lê cấp 2";
			case 36: return "Sao pha lê cấp 3";
			case 47: return "Giáp +#";
			case 48: return "HP/KI +#";
			case 50: return "Đánh chí mạng +#%";
			case 73: return "Hạn sử dụng # ngày";
			case 77: return "Cộng #% tiềm năng và sức mạnh";
			case 80: return "HP +#%";
			case 81: return "KI +#%";
			case 86: return "Tăng #% vàng rơi từ quái";
			case 87: return "Tăng #% exp khi đánh quái";
			case 93: return "Hạn sử dụng # ngày";
			case 94: return "Giáp +#%";
			case 95: return "Biến #% sát thương thành HP";
			case 96: return "Biến #% sát thương thành KI";
			case 97: return "Phản #% sát thương";
			case 98: return "Xuyên giáp chưởng #%";
			case 99: return "Xuyên giáp cận chiến #%";
			case 100: return "Kháng biến #%";
			case 101: return "Tăng #% HP, KI và Sức đánh";
			case 102: return "Lỗ sao pha lê";
			case 103: return "Đã mở khóa # lỗ sao";
			case 105: return "Vô hình khi không đánh quái";
			case 106: return "Bất tử khi HP < 10%";
			case 107: return "Tối đa # lỗ sao";
			case 108: return "Né đòn +#%";
			case 117: return "Đẹp trai";
			default: return changeString(defaultName);
			}
		}

	public static string replace(string _text, string _searchStr, string _replacementStr)
		{
			return _text.Replace(_searchStr, _replacementStr);
		}

	public static string[] split(string original, string separator, int count)
		{
			int num = original.IndexOf(separator);
			string[] array;
			if (num >= 0)
			{
				array = split(original.Substring(num + separator.Length), separator, count + 1);
			}
			else
			{
				array = new string[count + 1];
				num = original.Length;
			}
			array[count] = original.Substring(0, num);
			return array;
		}

	public static string formatNumber(long number)
		{
			string empty = string.Empty;
			string empty2 = string.Empty;
			empty = string.Empty;
			if (number >= 1000000000)
			{
				empty2 = mResources.billion;
				long num = number % 1000000000 / 100000000;
				number /= 1000000000;
				empty = number + string.Empty;
				if (num > 0)
				{
					string text = empty;
					return text + "," + num + empty2;
				}
				return empty + empty2;
			}
			if (number >= 1000000)
			{
				empty2 = mResources.million;
				long num2 = number % 1000000 / 100000;
				number /= 1000000;
				empty = number + string.Empty;
				if (num2 > 0)
				{
					string text = empty;
					return text + "," + num2 + empty2;
				}
				return empty + empty2;
			}
			return number + string.Empty;
		}

	public static string formatNumber2(long number)
		{
			string empty = string.Empty;
			string empty2 = string.Empty;
			empty = string.Empty;
			if (number >= 1000000000)
			{
				empty2 = mResources.billion;
				long num = number % 1000000000 / 10000000;
				number /= 1000000000;
				empty = number + string.Empty;
				if (num >= 10)
				{
					if (num % 10 == 0)
					{
						num /= 10;
					}
					string text = empty;
					return text + "," + num + empty2;
				}
				if (num > 0)
				{
					string text = empty;
					return text + ",0" + num + empty2;
				}
				return empty + empty2;
			}
			if (number >= 1000000)
			{
				empty2 = mResources.million;
				long num2 = number % 1000000 / 10000;
				number /= 1000000;
				empty = number + string.Empty;
				if (num2 >= 10)
				{
					if (num2 % 10 == 0)
					{
						num2 /= 10;
					}
					string text = empty;
					return text + "," + num2 + empty2;
				}
				if (num2 > 0)
				{
					string text = empty;
					return text + ",0" + num2 + empty2;
				}
				return empty + empty2;
			}
			if (number >= 10000)
			{
				empty2 = "k";
				long num3 = number % 1000 / 10;
				number /= 1000;
				empty = number + string.Empty;
				if (num3 >= 10)
				{
					if (num3 % 10 == 0)
					{
						num3 /= 10;
					}
					string text = empty;
					return text + "," + num3 + empty2;
				}
				if (num3 > 0)
				{
					string text = empty;
					return text + ",0" + num3 + empty2;
				}
				return empty + empty2;
			}
			return number + string.Empty;
		}

}
