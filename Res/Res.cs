using System;
using UnityEngine;

public partial class Res
{
	private static short[] sinz = new short[91]
		{
			0, 18, 36, 54, 71, 89, 107, 125, 143, 160,
			178, 195, 213, 230, 248, 265, 282, 299, 316, 333,
			350, 367, 384, 400, 416, 433, 449, 465, 481, 496,
			512, 527, 543, 558, 573, 587, 602, 616, 630, 644,
			658, 672, 685, 698, 711, 724, 737, 749, 761, 773,
			784, 796, 807, 818, 828, 839, 849, 859, 868, 878,
			887, 896, 904, 912, 920, 928, 935, 943, 949, 956,
			962, 968, 974, 979, 984, 989, 994, 998, 1002, 1005,
			1008, 1011, 1014, 1016, 1018, 1020, 1022, 1023, 1023, 1024,
			1024
		};

	private static short[] cosz;

	private static int[] tanz;

	public static string[] LOG_CAT = new string[5]
		{
			"<color=#ff0000ff>[  LOG_CAT  ]</color>",
			"<color=#ff0000ff>[LOG_SESSION]</color>",
			"<color=#ffff00ff>[LOG_SESSION]</color>",
			"<color=#ff0000ff>[LOG_MOBILE ]</color>",
			string.Empty
		};

	public static int count;

	public static bool isIcon;

	public static bool isBig;

	public static MyVector debug = new MyVector();

	public static MyRandom r = new MyRandom();

	public static void init()
		{
			cosz = new short[91];
			tanz = new int[91];
			for (int i = 0; i <= 90; i++)
			{
				cosz[i] = sinz[90 - i];
				if (cosz[i] == 0)
				{
					tanz[i] = int.MaxValue;
				}
				else
				{
					tanz[i] = (sinz[i] << 10) / cosz[i];
				}
			}
		}

	public static sbyte[] TakeSnapShot()
		{
			return null;
		}

	public static void outz(string s)
		{
			if (mSystem.isTest)
			{
				Debug.Log(s);
			}
		}

	public static void outz(string s, int logIndex)
		{
			if (mSystem.isTest)
			{
				Debug.Log(LOG_CAT[logIndex] + s);
			}
		}

	public static void err(string s)
		{
			if (mSystem.isTest)
			{
				Debug.LogError(s);
			}
		}

	public static void outz2(string s)
		{
		}

	public static void onScreenDebug(string s)
		{
		}

	public static void paintOnScreenDebug(mGraphics g)
		{
		}

	public static void updateOnScreenDebug()
		{
		}

	private static readonly string[][] translations = new string[][]
		{
			new string[] { "Anda masuk ke terlalu banyak akun di perangkat yang sama.[1]", "Bạn đã đăng nhập quá nhiều tài khoản trên cùng một thiết bị. [1]" },
			new string[] { "Anda masuk ke terlalu banyak akun di perangkat yang sama", "Bạn đã đăng nhập quá nhiều tài khoản trên cùng một thiết bị" },
			new string[] { "terlalu banyak akun di perangkat yang sama", "quá nhiều tài khoản trên cùng một thiết bị" },
			new string[] { "terlalu banyak akun", "quá nhiều tài khoản" },
			new string[] { "perangkat yang sama", "cùng một thiết bị" },
			new string[] { "Too many accounts logged in on the same device", "Bạn đã đăng nhập quá nhiều tài khoản trên cùng một thiết bị" },
			new string[] { "Error, harap coba lagi.[500]", "Lỗi kết nối máy chủ, vui lòng thử lại sau. [500]" },
			new string[] { "Error, harap coba lagi", "Lỗi kết nối máy chủ, vui lòng thử lại sau" },
			new string[] { "harap coba lagi", "vui lòng thử lại" },
			new string[] { "harap coba kembali", "vui lòng thử lại" },
			new string[] { "silakan coba lagi", "vui lòng thử lại" },
			new string[] { "silahkan coba lại", "vui lòng thử lại" },
			new string[] { "coba lagi", "thử lại" },
			new string[] { "Anda telah menerima", "Bạn đã nhận được" },
			new string[] { "Anda menerima", "Bạn nhận được" },
			new string[] { "Anda mendapatkan", "Bạn nhận được" },
			new string[] { "Tidak cukup emas", "Không đủ vàng" },
			new string[] { "Tidak đủ permata", "Không đủ ngọc" },
			new string[] { "Không đủ ngọc", "Không đủ ngọc" },
			new string[] { "Tidak cukup permata", "Không đủ ngọc" },
			new string[] { "Tidak cukup", "Không đủ" },
			new string[] { "Kekuatan không đủ", "Sức mạnh không đủ" },
			new string[] { "Kekuatan tidak cukup", "Sức mạnh không đủ" },
			new string[] { "Level tidak đủ", "Cấp độ không đủ" },
			new string[] { "Level không đủ", "Cấp độ không đủ" },
			new string[] { "Level tidak cukup", "Cấp độ không đủ" },
			new string[] { "Tingkat tidak cukup", "Cấp độ không đủ" },
			new string[] { "Karakter sedang sibuk", "Nhân vật đang bận" },
			new string[] { "Koneksi terputus", "Mất kết nối máy chủ" },
			new string[] { "Koneksi gagal", "Kết nối thất bại" },
			new string[] { "Gagal terhubung", "Không thể kết nối máy chủ" },
			new string[] { "Tidak dapat terhubung", "Không thể kết nối máy chủ" },
			new string[] { "Gagal masuk", "Đăng nhập thất bại" },
			new string[] { "Kata sandi salah", "Sai mật khẩu" },
			new string[] { "Password salah", "Sai mật khẩu" },
			new string[] { "Akun tidak ada", "Tài khoản không tồn tại" },
			new string[] { "Akun tidak terdaftar", "Tài khoản chưa đăng ký" },
			new string[] { "Akun sedang login", "Tài khoản đang đăng nhập" },
			new string[] { "Akun sedang digunakan", "Tài khoản đang đăng nhập" },
			new string[] { "Server sedang pemeliharaan", "Máy chủ đang bảo trì" },
			new string[] { "Server đang bảo trì", "Máy chủ đang bảo trì" },
			new string[] { "Sedang maintenance", "Máy chủ đang bảo trì" },
			new string[] { "Sedang pemeliharaan", "Máy chủ đang bảo trì" },
			new string[] { "Server đang đầy", "Máy chủ đã đầy" },
			new string[] { "Server sedang penuh", "Máy chủ đã đầy" },
			new string[] { "Server penuh", "Máy chủ đã đầy" },
			new string[] { "Tas penuh", "Hành trang đã đầy" },
			new string[] { "Ransel penuh", "Hành trang đã đầy" },
			new string[] { "Kotak penuh", "Rương đồ đã đầy" },
			new string[] { "Rương đầy", "Rương đồ đã đầy" },
			new string[] { "Pembelian berhasil", "Mua thành công" },
			new string[] { "Membeli berhasil", "Mua thành công" },
			new string[] { "Penjualan berhasil", "Bán thành công" },
			new string[] { "Menjual berhasil", "Bán thành công" },
			new string[] { "Waktu tunggu", "Thời gian chờ" },
			new string[] { "Anda chưa thể dùng vật phẩm này", "Bạn chưa thể dùng vật phẩm này" },
			new string[] { "Anda belum bisa memakai item ini", "Bạn chưa thể dùng vật phẩm này" },
			new string[] { "Tidak bisa memakai item ini", "Không thể dùng vật phẩm này" },
			new string[] { "Tidak bisa diperdagangkan", "Không thể giao dịch" },
			new string[] { "Tidak dapat ditukar", "Không thể giao dịch" },
			new string[] { "Masa berlaku", "Hạn sử dụng" },
			new string[] { "Kedaluwarsa", "Hạn sử dụng" },
			new string[] { "Terkunci", "Đã khóa" },
			new string[] { "Permintaan kekuatan", "Yêu cầu sức mạnh" },
			new string[] { "Kekuatan dibutuhkan", "Yêu cầu sức mạnh" },
			new string[] { "Kekuatan Anda", "Sức mạnh của bạn" },
			new string[] { "Dibutuhkan", "Yêu cầu" },
			new string[] { "Sarung Tangan", "Găng tay" },
			new string[] { "Sarung", "Găng tay" },
			new string[] { "Celana", "Quần" },
			new string[] { "Sepatu", "Giày" },
			new string[] { "Radar", "Rada" },
			new string[] { "Kacang Ajaib", "Đậu thần" },
			new string[] { "Kacang", "Đậu thần" },
			new string[] { "Bola Naga", "Ngọc Rồng" },
			new string[] { "Kapsul", "Capsule" },
			new string[] { "Jubah", "Áo choàng" },
			new string[] { "Pohon Ajaib", "Cây Đậu Thần" },
			new string[] { "Permata", "Ngọc" },
			new string[] { "Batu", "Đá" },
			new string[] { "Baju", "Áo" },
			new string[] { "Topi", "Nón" },
			new string[] { "Rambut", "Tóc" },
			new string[] { "Emas", "Vàng" },
			new string[] { "Koin", "Xu" },
			new string[] { "Gagal", "Thất bại" },
			new string[] { "Berhasil", "Thành công" },
			new string[] { "Kunci", "Khóa" },
			new string[] { "Serangan", "Sức đánh" },
			new string[] { "Pertahanan", "Giáp" },
			new string[] { "Kritikal", "Chí mạng" },
			new string[] { "Kecepatan", "Tốc độ" },
			new string[] { "Menghindar", "Né đòn" },
			new string[] { "Memantulkan", "Phản" },
			new string[] { "Menyerap", "Hút" },
			new string[] { "Meningkatkan", "Tăng" },
			new string[] { "Mengurangi", "Giảm" },
			new string[] { "Kerusakan", "Sát thương" },
			new string[] { "Pulihkan", "Hồi phục" },
			new string[] { "Darah", "HP" },
			new string[] { "Tenaga", "KI" },
			new string[] { "setiap", "mỗi" },
			new string[] { "detik", "giây" },
			new string[] { "hari", "ngày" },

			new string[] { "You have received", "Bạn đã nhận được" },
			new string[] { "You received", "Bạn nhận được" },
			new string[] { "Not enough gold", "Không đủ vàng" },
			new string[] { "Not enough gems", "Không đủ ngọc" },
			new string[] { "Not enough gem", "Không đủ ngọc" },
			new string[] { "Not enough", "Không đủ" },
			new string[] { "Power is not enough", "Sức mạnh không đủ" },
			new string[] { "Level is not enough", "Cấp độ không đủ" },
			new string[] { "Character is busy", "Nhân vật đang bận" },
			new string[] { "Connection lost", "Mất kết nối máy chủ" },
			new string[] { "Connection failed", "Kết nối thất bại" },
			new string[] { "Login failed", "Đăng nhập thất bại" },
			new string[] { "Wrong password", "Sai mật khẩu" },
			new string[] { "Account does not exist", "Tài khoản không tồn tại" },
			new string[] { "Account is logged in", "Tài khoản đang đăng nhập" },
			new string[] { "Server maintenance", "Máy chủ đang bảo trì" },
			new string[] { "Server is full", "Máy chủ đã đầy" },
			new string[] { "Inventory is full", "Hành trang đã đầy" },
			new string[] { "Bag is full", "Hành trang đã đầy" },
			new string[] { "Chest is full", "Rương đồ đã đầy" },
			new string[] { "Box is full", "Rương đồ đã đầy" },
			new string[] { "Buy successfully", "Mua thành công" },
			new string[] { "Buy success", "Mua thành công" },
			new string[] { "Sell successfully", "Bán thành công" },
			new string[] { "Sell success", "Bán thành công" },
			new string[] { "Wait time", "Thời gian chờ" },
			new string[] { "You cannot use this item", "Bạn chưa thể dùng vật phẩm này" },
			new string[] { "Cannot use this item", "Không thể dùng vật phẩm này" },
			new string[] { "Untradable", "Không thể giao dịch" },
			new string[] { "Cannot trade", "Không thể giao dịch" },
			new string[] { "Expires in", "Hạn sử dụng" },
			new string[] { "Expired", "Hết hạn" },
			new string[] { "Locked", "Đã khóa" },
			new string[] { "Power requirement", "Yêu cầu sức mạnh" },
			new string[] { "Require power", "Yêu cầu sức mạnh" },
			new string[] { "Your power", "Sức mạnh của bạn" },
			new string[] { "Requirement", "Yêu cầu" },
			new string[] { "Senzu Bean", "Đậu thần" },
			new string[] { "Magic Bean", "Đậu thần" },
			new string[] { "Dragon Ball", "Ngọc Rồng" },
			new string[] { "Magic Tree", "Cây Đậu Thần" },
			new string[] { "Trousers", "Quần" },
			new string[] { "Gloves", "Găng tay" },
			new string[] { "Glove", "Găng tay" },
			new string[] { "Shirt", "Áo" },
			new string[] { "Armor", "Áo" },
			new string[] { "Pants", "Quần" },
			new string[] { "Shoes", "Giày" },
			new string[] { "Boots", "Giày" },
			new string[] { "Cloak", "Áo choàng" },
			new string[] { "Diamond", "Ngọc" },
			new string[] { "Ruby", "Hồng ngọc" },
			new string[] { "Stone", "Đá" },
			new string[] { "Gold", "Vàng" },
			new string[] { "Coin", "Xu" },
			new string[] { "Hat", "Nón" },
			new string[] { "Hair", "Tóc" },
			new string[] { "Lock", "Khóa" },
			new string[] { "Failed", "Thất bại" },
			new string[] { "Success", "Thành công" },
			new string[] { "Critical", "Chí mạng" },
			new string[] { "Defense", "Giáp" },
			new string[] { "Damage", "Sát thương" },
			new string[] { "Attack", "Sức đánh" },
			new string[] { "Speed", "Tốc độ" },
			new string[] { "Dodge", "Né đòn" },
			new string[] { "Reflect", "Phản" },
			new string[] { "Absorb", "Hút" },
			new string[] { "Increase", "Tăng" },
			new string[] { "Reduce", "Giảm" },
			new string[] { "Recover", "Hồi phục" },
			new string[] { "every", "mỗi" },
			new string[] { "seconds", "giây" },
			new string[] { "second", "giây" },
			new string[] { "days", "ngày" },
			new string[] { "day", "ngày" }
		};

}
