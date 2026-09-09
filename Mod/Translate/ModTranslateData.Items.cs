using System.Collections.Generic;

public static partial class ModTranslate
{
	// Bảng ánh xạ tiếng Việt chuẩn xác cho 265 Option Templates (ID 0 đến 264)
	public static readonly string[] OptionTemplates = new string[]
	{
		"Tấn công: +#", // 0
				"Thời gian sử dụng còn # phút", // 1
				"HP, KI +#000", // 2
				"Hút #% sát thương Ki (PvP)", // 3
				"Hồi #% KI mỗi đòn đánh", // 4
				"+#% Sát thương chí mạng", // 5
				"HP +#", // 6
				"KI +#", // 7
				"Hút #% HP, KI xung quanh mỗi 5 giây", // 8
				"Hiệu lực trong # phút", // 9
				"Sát thương chuẩn #%", // 10
				"Điểm thưởng +#", // 11
				"Lượt dùng còn +#", // 12
				"Diệt quái +#", // 13
				"Chí mạng +#%", // 14
				"Phản đòn cận chiến +#", // 15
				"Tốc độ di chuyển +#%", // 16
				"Né đòn: +#", // 17
				"Chính xác: +#%", // 18
				"Tấn công quái +#%", // 19
				"Mã PIN #", // 20
				"Yêu cầu sức mạnh # tỷ", // 21
				"HP +#k", // 22
				"KI +#k", // 23
				"Trọng lực cao, làm chậm khu vực xung quanh", // 24
				"Áo choàng mỗi 5 giây", // 25
				"Hóa đá xung quanh mỗi 30 giây", // 26
				"+# HP/30s", // 27
				"+# KI/30s", // 28
				"Biến xung quanh thành socola mỗi 30 giây", // 29
				"Không thể giao dịch", // 30
				"Số lượng #", // 31
				"Không thể nâng cấp", // 32
				"Dịch chuyển tức thời", // 33
				"Phong ấn Ngôi sao (Seal of Star)", // 34
				"Phong ấn Mặt trăng (Seal of Moon)", // 35
				"Phong ấn Mặt trời (Seal of Sun)", // 36
				"Số lần gửi #", // 37
				"Chỉ khi hợp thể", // 38
				"<Yêu cầu cập nhật phiên bản mới>", // 39
				"Siêu cải trang # đá ngũ sắc", // 40
				"Chỉ số phụ +#:", // 41
				"Tấn công +#% quái bay", // 42
				"Tấn công +#% quái khỉ", // 43
				"Tấn công +#% quái đất", // 44
				"Tấn công +#% ở Namek", // 45
				"Tấn công +#% người Trái Đất", // 46
				"Giáp: +#", // 47
				"HP, KI +#", // 48
				"Sức đánh +#%", // 49
				"Sát thương tăng +#%", // 50
				"Songoku ss#", // 51
				"Vegeta ss#", // 52
				"Broly", // 53
				"Mr. Santa", // 54
				"Broly ss#", // 55
				"Trunks", // 56
				"Tenshinhan", // 57
				"Android 17", // 58
				"Super Namek #", // 59
				"Songoten", // 60
				"Songoku", // 61
				"Hồi phục HP #%", // 62
				"Còn lại # ngày", // 63
				"Còn lại # giờ", // 64
				"Còn lại # phút", // 65
				"Trống", // 66
				"Dùng để nâng cấp Găng tay", // 67
				"Dùng để nâng cấp Áo", // 68
				"Dùng để nâng cấp Quần", // 69
				"Dùng để nâng cấp Giày", // 70
				"Dùng để nâng cấp Rada", // 71
				"Cấp #", // 72
				"", // 73
				"Dùng để nâng cấp", // 74
				"Dùng để nâng cấp phép", // 75
				"VIP", // 76
				"HP +#%", // 77
				"Phá hủy +#%", // 78
				"Đệ tử nhận #% sức đánh", // 79
				"Hồi #% HP mỗi 30s", // 80
				"Hồi #% KI mỗi 30s", // 81
				"Quái không chủ động tấn công và giảm 20% sát thương nhận vào", // 82
				"Tăng 20% Sức mạnh và Tiềm năng nhận được khi đánh quái", // 83
				"Bay không tốn KI", // 84
				"Bay hồi phục KI", // 85
				"Ký gửi (Vàng)", // 86
				"Ký gửi (Ngọc)", // 87
				"Tăng #% exp khi luyện quái", // 88
				"Bay hồi phục HP và KI", // 89
				"Quyên góp và thử vận may", // 90
				"Mở và thử vận may", // 91
				"Chúc mừng năm mới bang hội và mọi người bằng pháo hoa", // 92
				"Hạn dùng: # ngày", // 93
				"Giảm #% sát thương", // 94
				"Hút #% sát thương thành HP", // 95
				"Hút #% sát thương thành KI", // 96
				"Phản #% sát thương nhận vào", // 97
				"Xuyên #% giáp bằng chưởng KI", // 98
				"Xuyên #% giáp cận chiến", // 99
				"+#% Vàng rơi từ quái", // 100
				"+#% Sức mạnh và Tiềm năng nhận từ quái", // 101
				"# Sao Pha Lê", // 102
				"KI +#%", // 103
				"Hút #% sát thương đánh quái thành HP", // 104
				"Vô hình khi không tấn công quái hoặc Boss", // 105
				"Kháng lạnh (Miễn dịch đóng băng)", // 106
				"# Sao Pha Lê", // 107
				"Né đòn +#%", // 108
				"Mùi hôi gây sát thương xung quanh", // 109
				"Có cơ hội nhặt được Pha lê từ quái", // 110
				"Quấy rối người xung quanh", // 111
				"Giảm giá #% khi mua Cải trang hoặc Avatar", // 112
				"Ném vào Sói Hercules", // 113
				"+#% Tốc độ di chuyển", // 114
				"Biến người chạm phải thành củ cà rốt", // 115
				"Không bị ảnh hưởng Thái Dương Hạ San", // 116
				"Đẹp trai tăng +#% sát thương xung quanh", // 117
				"Làm mù mục tiêu và gây choáng # mili giây", // 118
				"Làm mù xung quanh trong # giây", // 119
				"Thời gian hồi chiêu # giây", // 120
				"Thôi miên ngủ # giây", // 121
				"Khiên bảo hộ trong # giây", // 122
				"Trói mục tiêu trong # giây", // 123
				"Tỉnh dậy trong suy yếu -#% sức đánh trong 10 giây", // 124
				"Tăng và hồi #% HP cho bạn và đồng đội xung quanh trong 30 giây", // 125
				"Biến mục tiêu thành socola và giảm #% sức đánh trong 30 giây", // 126
				"Set Tienshinhan", // 127
				"Set Krillin", // 128
				"Set Songoku", // 129
				"Set Piccolo", // 130
				"Set Nail", // 131
				"Set Piccolo Daimao", // 132
				"Set Kakarot", // 133
				"Set Cađíc (Vegeta)", // 134
				"Set Nappa", // 135
				"$(5 món: +100% sát thương đấm Galick)", // 136
				"$(5 món: x5 thời gian biến Khỉ)", // 137
				"$(5 món: +80% HP)", // 138
				"$(5 món: x2 Thái Dương Hạ San)", // 139
				"$(5 món: +100% sát thương Quả Cầu Kênh Khi)", // 140
				"$(5 món: +100% sát thương Kamejoko)", // 141
				"$(5 món: +100% sát thương Masenko +50% KI)", // 142
				"$(5 món: Bất tử khi đánh quái)", // 143
				"$(5 món: +150% ST, +20% Chí mạng và bất tử cho Đệ tử)", // 144
				"$(Đứng gần người chơi mặc cải trang Dr. Slump: +20% sức đánh, +66% tốc độ)", // 145
				"$(Đứng gần 2 người chơi mặc cải trang Dr. Slump: +30% sức đánh, +100% tốc độ)", // 146
				"+#% Sức đánh", // 147
				"+#% Tốc độ di chuyển", // 148
				"Tăng khi đứng gần Cải trang OP khác", // 149
				"Tăng khi đứng gần Cải trang Sát thủ Android", // 150
				"$(Đứng gần người chơi nhóm Pilaf: +20% sức đánh, +66% tốc độ)", // 151
				"$(Đứng gần 2 người chơi nhóm Pilaf: +30% sức đánh, +100% tốc độ)", // 152
				"#% Tự phát nổ sau khi chết", // 153
				"Không thể bán", // 154
				"Giảm 50% Sức đánh, HP, KI và +#% Sức mạnh, Tiềm năng, Vàng rơi", // 155
				"Tích lũy 1% Sức đánh khi chỉ dùng đòn đấm, tối đa #%", // 156
				"Giảm #% toàn bộ sát thương khi KI dưới 20%", // 157
				"Có thể nhặt vật phẩm sự kiện ngẫu nhiên khi không mặc đồ và không cải trang.", // 158
				"x# Sát thương cơ bản Chưởng Ki mỗi phút", // 159
				"+#% Tiềm năng, Sức mạnh Đệ tử khi Sư phụ dùng Cải trang này", // 160
				"Nhận # ngọc miễn phí mỗi ngày khi đánh bại quái.", // 161
				"Hiệu ứng vui nhộn hồi #% KI/giây cho bạn và đồng đội xung quanh", // 162
				"Biến người thành quả bí ngô", // 163
				"Đổi # Điểm Sự kiện", // 164
				"Tỉ lệ +#% sát thương Lửa khi cận chiến đánh quái", // 165
				"Tăng +#% sức đánh cực ngầu khi bay với Cải trang Tàu Pảy Pảy", // 166
				"Tạo luồng khí lạnh", // 167
				"Hấp thụ sát thương để tăng sức đánh", // 168
				"Có tỉ lệ đầu độc kẻ địch", // 169
				"Tăng +#% sức đánh khi bay với Cải trang Gia đình Fide", // 170
				"Số lượng #k", // 171
				"#", // 172
				"Hồi #% HP và KI", // 173
				"Sự kiện năm #", // 174
				"Giảm #% thời gian bị mù", // 175
				"$(Đứng gần 4 đồng đội: +20% Sức đánh, +50% Tốc độ chạy)", // 176
				"$(Lên đến +2% tất cả chỉ số nếu gần Mabư Mập)", // 177
				"Hồi #% KI mỗi 10s", // 178
				"+2% sức đánh, lên đến 10% khi ở gần Cải trang Demons Frost", // 179
				"+#% sức đánh khi ở gần Cải trang Black Gohan Rosé", // 180
				"Đòn đánh sau dịch chuyển +#% sát thương", // 181
				"+#% sát thương cho Đệ tử triệu hồi", // 182
				"Giảm #% thời gian hồi chiêu Khiên năng lượng", // 183
				"+#% sức đánh, lên đến 10% khi ở gần Gohan Xanh, Android 18 Đỏ, Arale", // 184
				"+# giờ", // 185
				"Quay kẹo trong 30 giây", // 186
				"Giảm thời gian bị mù # giây", // 187
				"Đệ tử Kamejoko +#% sát thương", // 188
				"Đệ tử Thái Dương Hạ San +#% sát thương", // 189
				"Đệ tử Masenko +#% sát thương", // 190
				"Né đòn chí mạng +#%", // 191
				"Chí mạng +#%", // 192
				"+#% sức đánh, lên đến 11% khi gần Cải trang Cầu thủ bóng đá", // 193
				"+#% sức đánh, lên đến 10% khi gần Cải trang Mùa hè khác", // 194
				"+#% sức đánh, lên đến 10% khi gần Cải trang Siêu anh hùng", // 195
				"Đẹp trai +#% sức đánh, lên đến 18% khi gần Cải trang Thỏ khác", // 196
				"Tấn công +#% lên Xayda", // 197
				"Giảm #% sát thương từ Trái Đất", // 198
				"Giảm #% sát thương từ Namek", // 199
				"Giảm #% sát thương từ Xayda", // 200
				"Tấn công +#% khi gần 2 thành viên bang", // 201
				"HP +#% khi gần 2 thành viên bang", // 202
				"KI +#% khi gần 2 thành viên bang", // 203
				"Tấn công +#% lên Boss", // 204
				"+#% sức đánh, lên đến 18% khi gần Cải trang Diệt Quỷ", // 205
				"Vật phẩm hiếm rơi từ quái (+#%)", // 206
				"Vật phẩm hiếm rơi từ Boss (+#%)", // 207
				"Trang bị đã tinh chế / chuyển hóa", // 208
				"Hạ cấp # lần", // 209
				"# Chỉ số ẩn", // 210
				"Chưa giám định #/5", // 211
				"Độ bền #/1000", // 212
				"Giá bán: # triệu vàng", // 213
				"Phạm vi Tuyệt kỹ +#%", // 214
				"Số mục tiêu +#", // 215
				"Tuyệt kỹ +#% sát thương", // 216
				"- Chưa xác thực", // 217
				"---------", // 218
				"Lần tinh luyện #", // 219
				"Hoàn thành #%", // 220
				"Hồi #% HP và KI mỗi 30 giây cho bản thân và đồng đội", // 221
				"Mỗi 120s tăng #% sức đánh trong 30s", // 222
				"Mỗi 120s hồi #% HP trong 30s", // 223
				"Giảm thời gian hồi tất cả kỹ năng xuống 0.1s", // 224
				"Giảm # giây hiệu ứng Ma Phong Ba (Mafuba)", // 225
				"Tăng +#% sát thương vui nhộn cho mọi người xung quanh", // 226
				"Giảm #% hiệu ứng khống chế khi dùng khiên năng lượng", // 227
				"Đã khảm đến lỗ sao pha lê #", // 228
				"Khí lạnh làm giảm #% HP và KI mỗi 30s của người xung quanh", // 229
				"+# triệu Tiềm năng, Sức mạnh", // 230
				"Hạn dùng hoặc Vĩnh viễn", // 231
				"Giao dịch còn lại: #", // 232
				"Set Gohan", // 233
				"$(5 món: +300% may mắn, +50% vàng rơi từ quái)", // 234
				"Lượt ký gửi còn lại: #", // 235
				"+#% May mắn", // 236
				"Set Nail Namek", // 237
				"$[2 món] Tăng chí mạng", // 238
				"$[4 món] Tăng vừa sát thương Masenko", // 239
				"$[5 món] Tăng mạnh sát thương Masenko", // 240
				"Set Cađíc (Vegeta M)", // 241
				"$[2 món] Tăng HP và phạm vi nổ", // 242
				"$[4 món] Tăng sát thương vụ nổ", // 243
				"$[5 món] Tăng cực lớn sát thương vụ nổ", // 244
				"Set Kaioshin", // 245
				"$[2 món] Tăng chí mạng", // 246
				"$[4 món] Giảm hao hụt HP và KI khi dùng Kaio-ken", // 247
				"$[5 món] Tăng cực nhiều sát thương và giảm hao hụt HP, KI khi dùng Kaio-ken", // 248
				"Hắc hóa: +#% HP cho người chơi xung quanh", // 249
				"Ngầu: +#% sức đánh của bạn và đồng đội", // 250
				"Hồi #% HP mỗi 10s cho bạn và đồng đội", // 251
				"Hạn dùng: # giờ", // 252
				"# Kilis", // 253
				"Set Thần Hủy Diệt Champa", // 254
				"$[2 món] Tăng nhẹ sát thương lên Boss", // 255
				"$[4 món] Tăng đáng kể sát thương lên Boss", // 256
				"$[5 món] Tăng cực đại sát thương Boss và tăng mạnh sát thương chí mạng", // 257
				"Có thể dùng cho Đệ tử 2", // 258
				"Ngầu: giảm #% sát thương cho người xung quanh", // 259
				"#% cơ hội nhận Kháng Lạnh", // 260
				"Sát thương chuẩn (5–10%)", // 261
				"Wow: Tăng #% Chí mạng cho đồng minh xung quanh", // 262
				"Sát thương cuối +#%", // 263
				"Hạn dùng: #", // 264
	};

	// Bảng tra cứu tên vật phẩm dịch chuẩn xác theo ID
	public static readonly Dictionary<int, string> SpecificItemNames = new Dictionary<int, string>()
	{
		{ 0, "Áo thun" },
		{ 1, "Áo len Namếc" },
		{ 2, "Áo vải thô Xayda" },
		{ 3, "Áo thun dày" },
		{ 4, "Áo khoác Piccolo" },
		{ 5, "Áo sắt Xayda" },
		{ 6, "Quần đen" },
		{ 7, "Quần len Namếc" },
		{ 8, "Quần thô Xayda" },
		{ 9, "Quần dày" },
		{ 10, "Quần bạt Piccolo" },
		{ 11, "Quần sắt Xayda" },
		{ 12, "Rada cấp 1" },
		{ 13, "Đậu thần cấp 1" },
		{ 14, "Ngọc Rồng 1 sao" },
		{ 15, "Ngọc Rồng 2 sao" },
		{ 16, "Ngọc Rồng 3 sao" },
		{ 17, "Ngọc Rồng 4 sao" },
		{ 18, "Ngọc Rồng 5 sao" },
		{ 19, "Ngọc Rồng 6 sao" },
		{ 20, "Ngọc Rồng 7 sao" },
		{ 21, "Găng đen" },
		{ 22, "Găng len Namếc" },
		{ 23, "Găng thô Xayda" },
		{ 24, "Găng thun đen" },
		{ 25, "Găng len Piccolo" },
		{ 26, "Găng sắt Xayda" },
		{ 27, "Giày nhựa" },
		{ 28, "Giày len Namếc" },
		{ 29, "Giày tròn Xayda" },
		{ 30, "Giày cao su" },
		{ 31, "Giày nhựa Piccolo" },
		{ 32, "Giày sắt Xayda" },
		{ 33, "Áo thun" },
		{ 34, "Áo thun dày" },
		{ 35, "Quần đen" },
		{ 36, "Quần dày" },
		{ 37, "Găng vải thô" },
		{ 38, "Găng thun dày" },
		{ 39, "Giày nhựa dày" },
		{ 40, "Giày cao su dày" },
		{ 41, "Áo gai Namếc" },
		{ 42, "Áo Piccolo" },
		{ 43, "Quần gai Namếc" },
		{ 44, "Quần thun Piccolo" },
		{ 45, "Găng thun Piccolo" },
		{ 46, "Găng gai Namếc" },
		{ 47, "Giày gai Namếc" },
		{ 48, "Giày cao su Piccolo" },
		{ 49, "Áo thô Xayda" },
		{ 50, "Áo đồng Xayda" },
		{ 51, "Quần thun thô" },
		{ 52, "Quần đồng Xayda" },
		{ 53, "Găng thô Xayda" },
		{ 54, "Găng đồng Xayda" },
		{ 55, "Giày cao su thô" },
		{ 56, "Giày đồng Xayda" },
		{ 57, "Rada cấp 2" },
		{ 58, "Rada cấp 3" },
		{ 59, "Rada cấp 4" },
		{ 60, "Đậu thần cấp 2" },
		{ 61, "Đậu thần cấp 3" },
		{ 62, "Đậu thần cấp 4" },
		{ 63, "Đậu thần cấp 5" },
		{ 64, "Đậu thần cấp 6" },
		{ 65, "Đậu thần cấp 7" },
		{ 66, "Sách Dragon cấp 1" },
		{ 67, "Sách Dragon cấp 2" },
		{ 68, "Sách Dragon cấp 3" },
		{ 69, "Sách Dragon cấp 4" },
		{ 70, "Sách Dragon cấp 5" },
		{ 71, "Sách Dragon cấp 6" },
		{ 72, "Sách Dragon cấp 7" },
		{ 73, "Đùi gà" },
		{ 74, "Đùi gà nướng" },
		{ 75, "Đùi heo Xayda" },
		{ 76, "Vàng" },
		{ 77, "Ngọc" },
		{ 78, "Thần Đăng" },
		{ 79, "Sách Demon cấp 1" },
		{ 80, "Sách Demon cấp 2" },
		{ 81, "Sách Demon cấp 3" },
		{ 82, "Sách Demon cấp 4" },
		{ 83, "Sách Demon cấp 5" },
		{ 84, "Sách Demon cấp 6" },
		{ 85, "Sách Demon cấp 7" },
		{ 87, "Sách Galick cấp 1" },
		{ 88, "Sách Galick cấp 2" },
		{ 89, "Sách Galick cấp 3" },
		{ 90, "Sách Galick cấp 4" },
		{ 91, "Sách Galick cấp 5" },
		{ 92, "Sách Galick cấp 6" },
		{ 93, "Sách Galick cấp 7" },
		{ 94, "Sách Kamejoko cấp 1" },
		{ 95, "Sách Kamejoko cấp 2" },
		{ 96, "Sách Kamejoko cấp 3" },
		{ 97, "Sách Kamejoko cấp 4" },
		{ 98, "Sách Kamejoko cấp 5" },
		{ 99, "Sách Kamejoko cấp 6" },
		{ 100, "Sách Kamejoko cấp 7" },
		{ 101, "Sách Masenko cấp 1" },
		{ 102, "Sách Masenko cấp 2" },
		{ 103, "Sách Masenko cấp 3" },
		{ 104, "Sách Masenko cấp 4" },
		{ 105, "Sách Masenko cấp 5" },
		{ 106, "Sách Masenko cấp 6" },
		{ 107, "Sách Masenko cấp 7" },
		{ 108, "Sách Antomic cấp 1" },
		{ 109, "Sách Antomic cấp 2" },
		{ 110, "Sách Antomic cấp 3" },
		{ 111, "Sách Antomic cấp 4" },
		{ 112, "Sách Antomic cấp 5" },
		{ 113, "Sách Antomic cấp 6" },
		{ 114, "Sách Antomic cấp 7" },
		{ 115, "Sách Thái Dương Hạ San cấp 1" },
		{ 116, "Sách Thái Dương Hạ San cấp 2" },
		{ 117, "Sách Thái Dương Hạ San cấp 3" },
		{ 118, "Sách Thái Dương Hạ San cấp 4" },
		{ 119, "Sách Thái Dương Hạ San cấp 5" },
		{ 120, "Sách Thái Dương Hạ San cấp 6" },
		{ 121, "Sách Thái Dương Hạ San cấp 7" },
		{ 122, "Sách Trị Thương cấp 1" },
		{ 123, "Sách Trị Thương cấp 2" },
		{ 124, "Sách Trị Thương cấp 3" },
		{ 125, "Sách Trị Thương cấp 4" },
		{ 126, "Sách Trị Thương cấp 5" },
		{ 127, "Sách Trị Thương cấp 6" },
		{ 128, "Sách Trị Thương cấp 7" },
		{ 129, "Sách Khiên Năng Lượng cấp 1" },
		{ 130, "Sách Khiên Năng Lượng cấp 2" },
		{ 131, "Sách Khiên Năng Lượng cấp 3" },
		{ 132, "Sách Khiên Năng Lượng cấp 4" },
		{ 133, "Sách Khiên Năng Lượng cấp 5" },
		{ 134, "Sách Khiên Năng Lượng cấp 6" },
		{ 135, "Sách Khiên Năng Lượng cấp 7" },
		{ 136, "Áo Kame" },
		{ 137, "Áo thun Kame" },
		{ 138, "Áo võ sĩ Kame" },
		{ 139, "Áo võ sĩ Goku" },
		{ 140, "Quần Kame" },
		{ 141, "Quần siêu Kame" },
		{ 142, "Quần võ sĩ Kame" },
		{ 143, "Quần võ sĩ Goku" },
		{ 144, "Găng vải Kame" },
		{ 145, "Găng siêu Kame" },
		{ 146, "Găng võ sĩ Kame" },
		{ 147, "Găng võ sĩ Goku" },
		{ 148, "Giày nhựa Kame" },
		{ 149, "Giày cao su Kame" },
		{ 150, "Giày võ sĩ Kame" },
		{ 151, "Giày võ sĩ Goku" },
		{ 152, "Áo choàng len" },
		{ 153, "Áo choàng thun" },
		{ 154, "Áo thun Piccolo" },
		{ 155, "Áo khoác Piccolo" },
		{ 156, "Quần len cứng" },
		{ 157, "Quần thun cứng" },
		{ 158, "Quần cứng Piccolo" },
		{ 159, "Quần mềm Piccolo" },
		{ 160, "Găng len cứng" },
		{ 161, "Găng thun cứng" },
		{ 162, "Găng Piccolo" },
		{ 163, "Găng da Piccolo" },
		{ 164, "Giày nhựa cứng" },
		{ 165, "Giày cao su cứng" },
		{ 166, "Giày da Piccolo" },
		{ 167, "Giày sắt Piccolo" },
		{ 168, "Áo giáp bạc" },
		{ 169, "Áo giáp vàng" },
		{ 170, "Áo lông Xayda" },
		{ 171, "Áo khoác Xayda" },
		{ 172, "Quần bạc Xayda" },
		{ 173, "Quần vàng Xayda" },
		{ 174, "Quần lông Xayda" },
		{ 175, "Quần da Xayda" },
		{ 176, "Găng bạc Xayda" },
		{ 177, "Găng vàng Xayda" },
		{ 178, "Găng lông Xayda" },
		{ 179, "Găng da Xayda" },
		{ 180, "Giày bạc Xayda" },
		{ 181, "Giày vàng Xayda" },
		{ 182, "Giày lông Xayda" },
		{ 183, "Giày da Xayda" },
		{ 184, "Rada cấp 5" },
		{ 185, "Rada cấp 6" },
		{ 186, "Rada cấp 7" },
		{ 187, "Rada cấp 8" },
		{ 188, "Vàng" },
		{ 189, "Vàng" },
		{ 190, "Vàng" },
		{ 191, "Cà chua" },
		{ 192, "Cà rốt" },
		{ 193, "Túi 10 Capsule" },
		{ 194, "Capsule đặc biệt" },
		{ 195, "Thuốc tẩy tiềm năng" },
		{ 211, "Nho Tím" },
		{ 212, "Nho Xanh" },
		{ 213, "Bùa Trí Tuệ" },
		{ 214, "Bùa Mạnh Mẽ" },
		{ 215, "Bùa Da Trâu" },
		{ 216, "Bùa Oai Hùm" },
		{ 217, "Bùa Bất Tử" },
		{ 218, "Bùa Dẻo Dai" },
		{ 219, "Bùa Thu Hút" },
		{ 220, "Đá Lục Bảo" },
		{ 221, "Đá Lam Ngọc" },
		{ 222, "Đá Hồng Ngọc" },
		{ 223, "Đá Titan" },
		{ 224, "Đá Thạch Anh Tím" },
		{ 225, "Mảnh vụn" },
		{ 226, "Bình nước phép" },
		{ 230, "Áo bạc Goku" },
		{ 231, "Áo vàng Goku" },
		{ 232, "Áo da Trunks" },
		{ 233, "Áo jean Trunks" },
		{ 234, "Áo sắt Tròn" },
		{ 235, "Áo đồng Tròn" },
		{ 236, "Áo bạc Zealot" },
		{ 237, "Áo vàng Zealot" },
		{ 238, "Áo da đỏ Xayda" },
		{ 239, "Áo Siêu Xayda" },
		{ 240, "Áo Kaio Xayda" },
		{ 241, "Áo Rồng Xayda" },
		{ 242, "Quần bạc Goku" },
		{ 243, "Quần vàng Goku" },
		{ 244, "Quần da Trunks" },
		{ 245, "Quần jean Trunks" },
		{ 246, "Quần sắt Tròn" },
		{ 247, "Quần đồng Tròn" },
		{ 248, "Quần bạc Zealot" },
		{ 249, "Quần vàng Zealot" },
		{ 250, "Quần da đỏ Xayda" },
		{ 251, "Quần Siêu Xayda" },
		{ 252, "Quần Kaio Xayda" },
		{ 253, "Quần Rồng Xayda" },
		{ 254, "Găng bạc Goku" },
		{ 255, "Găng vàng Goku" },
		{ 256, "Găng da Trunks" },
		{ 257, "Găng jean Trunks" },
		{ 258, "Găng sắt Tròn" },
		{ 259, "Găng đồng Tròn" },
		{ 260, "Găng bạc Zealot" },
		{ 261, "Găng vàng Zealot" },
		{ 262, "Găng lông đỏ" },
		{ 263, "Găng Siêu Xayda" },
		{ 264, "Găng Kaio Xayda" },
		{ 265, "Găng Rồng Xayda" },
		{ 266, "Giày bạc Goku" },
		{ 267, "Giày vàng Goku" },
		{ 268, "Giày da Trunks" },
		{ 269, "Giày jean Trunks" },
		{ 270, "Giày sắt Tròn" },
		{ 271, "Giày đồng Tròn" },
		{ 272, "Giày bạc Zealot" },
		{ 273, "Giày vàng Zealot" },
		{ 274, "Giày lông đỏ" },
		{ 275, "Giày Siêu Xayda" },
		{ 276, "Giày Kaio Xayda" },
		{ 277, "Giày Rồng Xayda" },
		{ 278, "Rada cấp 9" },
		{ 279, "Rada cấp 10" },
		{ 280, "Rada cấp 11" },
		{ 281, "Rada cấp 12" },
		{ 282, "Cải trang Kaio" },
		{ 283, "Cải trang Yajirôbê" },
		{ 284, "Cải trang Tàu Pảy Pảy" },
		{ 285, "Cải trang Thần Mèo Karin" },
		{ 286, "Cải trang Thượng Đế" },
		{ 287, "Cải trang Mr. Pôpô" },
		{ 288, "Cải trang Kuku" },
		{ 289, "Cải trang Zarbon" },
		{ 290, "Cải trang Jeice" },
		{ 291, "Cải trang Ninja Áo Tím" },
		{ 292, "Cải trang Recoome" },
		{ 293, "1 gói 30 Hạt Đậu Thần" },
		{ 294, "2 gói 30 Hạt Đậu Thần" },
		{ 295, "3 gói 30 Hạt Đậu Thần" },
		{ 296, "4 gói 30 Hạt Đậu Thần" },
		{ 297, "5 gói 30 Hạt Đậu Thần" },
		{ 298, "6 gói 30 Hạt Đậu Thần" },
		{ 299, "7 gói 30 Hạt Đậu Thần" },
		{ 300, "Sách Kaio-ken cấp 1" },
		{ 301, "Sách Kaio-ken cấp 2" },
		{ 302, "Sách Kaio-ken cấp 3" },
		{ 303, "Sách Kaio-ken cấp 4" },
		{ 304, "Sách Kaio-ken cấp 5" },
		{ 305, "Sách Kaio-ken cấp 6" },
		{ 306, "Sách Kaio-ken cấp 7" },
		{ 307, "Sách Quả Cầu Kênh Khi cấp 1" },
		{ 308, "Sách Quả Cầu Kênh Khi cấp 2" },
		{ 309, "Sách Quả Cầu Kênh Khi cấp 3" },
		{ 310, "Sách Quả Cầu Kênh Khi cấp 4" },
		{ 311, "Sách Quả Cầu Kênh Khi cấp 5" },
		{ 312, "Sách Quả Cầu Kênh Khi cấp 6" },
		{ 313, "Sách Quả Cầu Kênh Khi cấp 7" },
		{ 314, "Sách Biến Khỉ cấp 1" },
		{ 315, "Sách Biến Khỉ cấp 2" },
		{ 316, "Sách Biến Khỉ cấp 3" },
		{ 317, "Sách Biến Khỉ cấp 4" },
		{ 318, "Sách Biến Khỉ cấp 5" },
		{ 319, "Sách Biến Khỉ cấp 6" },
		{ 320, "Sách Biến Khỉ cấp 7" },
		{ 321, "Sách Tự Sát cấp 1" },
		{ 322, "Sách Tự Sát cấp 2" },
		{ 323, "Sách Tự Sát cấp 3" },
		{ 324, "Sách Tự Sát cấp 4" },
		{ 325, "Sách Tự Sát cấp 5" },
		{ 326, "Sách Tự Sát cấp 6" },
		{ 327, "Sách Tự Sát cấp 7" },
		{ 328, "Sách Makankosappo cấp 1" },
		{ 329, "Sách Makankosappo cấp 2" },
		{ 330, "Sách Makankosappo cấp 3" },
		{ 331, "Sách Makankosappo cấp 4" },
		{ 332, "Sách Makankosappo cấp 5" },
		{ 333, "Sách Makankosappo cấp 6" },
		{ 334, "Sách Makankosappo cấp 7" },
		{ 335, "Sách Đẻ Trứng cấp 1" },
		{ 336, "Sách Đẻ Trứng cấp 2" },
		{ 337, "Sách Đẻ Trứng cấp 3" },
		{ 338, "Sách Đẻ Trứng cấp 4" },
		{ 339, "Sách Đẻ Trứng cấp 5" },
		{ 340, "Sách Đẻ Trứng cấp 6" },
		{ 341, "Sách Đẻ Trứng cấp 7" },
		{ 342, "Vệ tinh KI" },
		{ 343, "Vệ tinh Trí Tuệ" },
		{ 344, "Vệ tinh Phòng Thủ" },
		{ 345, "Vệ tinh HP" },
		{ 346, "Cân Đẩu Vân" },
		{ 347, "Rồng Bay" },
		{ 348, "Ván Bay JetBoard" },
		{ 349, "Cân Đẩu Vân VIP" },
		{ 350, "Rồng Bay VIP" },
		{ 351, "Ván Bay VIP" },
		{ 352, "Đậu thần cấp 8" },
		{ 353, "Ngọc Rồng Namếc 1 sao" },
		{ 354, "Ngọc Rồng Namếc 2 sao" },
		{ 355, "Ngọc Rồng Namếc 3 sao" },
		{ 356, "Ngọc Rồng Namếc 4 sao" },
		{ 357, "Ngọc Rồng Namếc 5 sao" },
		{ 358, "Ngọc Rồng Namếc 6 sao" },
		{ 359, "Ngọc Rồng Namếc 7 sao" },
		{ 360, "Ngọc Rồng Namếc" },
		{ 361, "Gói 10 Rada Dò Namếc" },
		{ 362, "Hóa thạch Ngọc Rồng Namếc" },
		{ 363, "Tháo cờ" },
		{ 364, "Cờ Xanh Dương" },
		{ 365, "Cờ Đỏ" },
		{ 366, "Cờ Tím" },
		{ 367, "Cờ Vàng" },
		{ 368, "Cờ Xanh Lá" },
		{ 369, "Cờ Hồng" },
		{ 370, "Cờ Cam" },
		{ 371, "Cờ Xám" },
		{ 372, "Ngọc Rồng Sao Đen 1 sao" },
		{ 373, "Ngọc Rồng Sao Đen 2 sao" },
		{ 374, "Ngọc Rồng Sao Đen 3 sao" },
		{ 375, "Ngọc Rồng Sao Đen 4 sao" },
		{ 376, "Ngọc Rồng Sao Đen 5 sao" },
		{ 377, "Ngọc Rồng Sao Đen 6 sao" },
		{ 378, "Ngọc Rồng Sao Đen 7 sao" },
		{ 379, "Rada Dò Capsule Kì Bí" },
		{ 380, "Capsule Kì Bí" },
		{ 381, "Cuồng Nộ" },
		{ 382, "Bổ Huyết" },
		{ 383, "Bổ Khí" },
		{ 384, "Giáp Xên" },
		{ 385, "Ẩn Danh" },
		{ 400, "Đổi tên đệ tử" },
		{ 401, "Đổi đệ tử mới" },
		{ 402, "Nâng chiêu 1 đệ tử" },
		{ 403, "Nâng chiêu 2 đệ tử" },
		{ 404, "Nâng chiêu 3 đệ tử" },
		{ 405, "Cải trang Fide cấp 1" },
		{ 406, "Cải trang Fide cấp 2" },
		{ 407, "Cải trang Fide cấp 3" },
		{ 408, "Cải trang Chi-Chi" },
		{ 409, "Cải trang Bunma" },
		{ 410, "Cải trang Launch" },
		{ 411, "Cải trang Android 18" },
		{ 421, "Cải trang Sơn Tinh" },
		{ 422, "Cải trang Thủy Tinh" },
		{ 423, "Cải trang Bujin" },
		{ 424, "Cải trang Kogu" },
		{ 425, "Cải trang Zangya" },
		{ 426, "Cải trang Bido" },
		{ 427, "Cải trang Bojack" },
		{ 428, "Cải trang Siêu Bojack" },
		{ 429, "Cải trang Guldo" },
		{ 430, "Cải trang Recoome" },
		{ 431, "Cải trang Jeice" },
		{ 432, "Cải trang Burter" },
		{ 433, "Cải trang Đội Trưởng Ginyu" },
		{ 434, "Sách Khiên Năng Lượng cấp 1" },
		{ 435, "Sách Khiên Năng Lượng cấp 2" },
		{ 436, "Sách Khiên Năng Lượng cấp 3" },
		{ 437, "Sách Khiên Năng Lượng cấp 4" },
		{ 438, "Sách Khiên Năng Lượng cấp 5" },
		{ 439, "Sách Khiên Năng Lượng cấp 6" },
		{ 440, "Sách Khiên Năng Lượng cấp 7" },
		{ 523, "Đậu thần cấp 9" },
		{ 555, "Áo Thần Trái Đất" },
		{ 556, "Quần Thần Trái Đất" },
		{ 557, "Áo Thần Namếc" },
		{ 558, "Quần Thần Namếc" },
		{ 559, "Áo Thần Xayda" },
		{ 560, "Quần Thần Xayda" },
		{ 561, "Nhẫn Thần Linh" },
		{ 562, "Găng Thần Trái Đất" },
		{ 563, "Giày Thần Trái Đất" },
		{ 564, "Găng Thần Namếc" },
		{ 565, "Giày Thần Namếc" },
		{ 566, "Găng Thần Xayda" },
		{ 567, "Giày Thần Xayda" },
		{ 595, "Đậu thần cấp 10" },
		{ 650, "Áo Hủy Diệt" },
		{ 651, "Quần Hủy Diệt" },
		{ 652, "Áo Hủy Diệt Namếc" },
		{ 653, "Quần Hủy Diệt Namếc" },
		{ 654, "Áo Hủy Diệt Xayda" },
		{ 655, "Quần Hủy Diệt Xayda" },
		{ 656, "Nhẫn Hủy Diệt" },
		{ 657, "Găng Hủy Diệt" },
		{ 658, "Giày Hủy Diệt" },
		{ 659, "Găng Hủy Diệt Namếc" },
		{ 660, "Giày Hủy Diệt Namếc" },
		{ 661, "Găng Hủy Diệt Xayda" },
		{ 662, "Giày Hủy Diệt Xayda" },
		{ 702, "Bí ngô 1 sao" },
		{ 703, "Bí ngô 2 sao" },
		{ 704, "Bí ngô 3 sao" },
		{ 705, "Bí ngô 4 sao" },
		{ 706, "Bí ngô 5 sao" },
		{ 707, "Bí ngô 6 sao" },
		{ 708, "Bí ngô 7 sao" },
		{ 807, "Ngọc Rồng Đen 1 sao" },
		{ 808, "Ngọc Rồng Đen 2 sao" },
		{ 809, "Ngọc Rồng Đen 3 sao" },
		{ 810, "Ngọc Rồng Đen 4 sao" },
		{ 811, "Ngọc Rồng Đen 5 sao" },
		{ 812, "Ngọc Rồng Đen 6 sao" },
		{ 813, "Ngọc Rồng Đen 7 sao" },
		{ 925, "Ngọc Rồng Băng 1 sao" },
		{ 926, "Ngọc Rồng Băng 2 sao" },
		{ 927, "Ngọc Rồng Băng 3 sao" },
		{ 928, "Ngọc Rồng Băng 4 sao" },
		{ 929, "Ngọc Rồng Băng 5 sao" },
		{ 930, "Ngọc Rồng Băng 6 sao" },
		{ 931, "Ngọc Rồng Băng 7 sao" },
		{ 1048, "Áo Thiên Sứ Trái Đất" },
		{ 1049, "Áo Thiên Sứ Namếc" },
		{ 1050, "Áo Thiên Sứ Xayda" },
		{ 1051, "Quần Thiên Sứ Trái Đất" },
		{ 1052, "Quần Thiên Sứ Namếc" },
		{ 1053, "Quần Thiên Sứ Xayda" },
		{ 1054, "Găng Thiên Sứ Trái Đất" },
		{ 1055, "Găng Thiên Sứ Namếc" },
		{ 1056, "Găng Thiên Sứ Xayda" },
		{ 1057, "Giày Thiên Sứ Trái Đất" },
		{ 1058, "Giày Thiên Sứ Namếc" },
		{ 1059, "Giày Thiên Sứ Xayda" },
		{ 1060, "Nhẫn Thiên Sứ Trái Đất" },
		{ 1061, "Nhẫn Thiên Sứ Namếc" },
		{ 1062, "Nhẫn Thiên Sứ Xayda" },
		{ 1715, "Đậu thần cấp 11" },
		{ 2000, "Đá Linh Hồn" },
		{ 2001, "Đá Thực Tại" },
		{ 2002, "Đá Sức Mạnh" },
		{ 2003, "Đá Không Gian" },
		{ 2004, "Đá Thời Gian" },
	};

	// Bảng ánh xạ mô tả vật phẩm chuẩn xác
	public static readonly Dictionary<string, string> ItemDescriptionMap = new Dictionary<string, string>()
	{
		{ "Item peristiwa", "Vật phẩm sự kiện" },
		{ "Mengubah wajah", "Cải trang biến hình" },
		{ "Mengurangi damage yang diterima", "Giảm sát thương nhận vào" },
		{ "Meningkatkan HP", "Tăng HP" },
		{ "Meningkatkan serangan", "Tăng sức đánh" },
		{ "Meningkatkan MP", "Tăng KI" },
		{ "Item pendukung", "Vật phẩm hỗ trợ" },
		{ "Dimasukkan ke dalam barang bintang kristal untuk meningkatkan pilihan", "Dùng đục lỗ trang bị pha lê để thêm thuộc tính" },
		{ "Judul", "Danh hiệu" },
		{ "Mengisi HP dan KI", "Hồi phục HP và KI" },
		{ "Mengisi penuh HP dan MP", "Hồi phục đầy đủ HP và KI" },
		{ "Meningkatkan Critical", "Tăng chí mạng" },
		{ "Meningkatkan critical", "Tăng chí mạng" },
		{ "Terbang tanpa menggunakan KI", "Bay không tốn KI" },
		{ "Item acara, menghilang saat acara berakhir", "Vật phẩm sự kiện, biến mất khi sự kiện kết thúc" },
		{ "Mempunyai Item Item Berharga", "Chứa nhiều vật phẩm có giá trị" },
		{ "One Piece Menyamar", "Cải trang One Piece" },
		{ "Item balap terbaik", "Vật phẩm đua top" },
		{ "Kumpulkan 7 Dragon Ball untuk mengabulkan keinginan", "Thu thập đủ 7 Viên Ngọc Rồng để gọi Rồng Thần" },
		{ "Pakai PK ke bendera warna lain", "Bật cờ PK với người khác cờ" },
		{ "Menyimpan permintaan ke Shen-Long", "Lưu giữ điều ước Rồng Thần" },
		{ "Temui Urinai Obaba untuk menggunakan batu ini", "Gặp Bà Hạt Mít để sử dụng đá này" },
		{ "Cari Urinai Obaba untuk sử dụng batu này", "Tìm Bà Hạt Mít để sử dụng đá này" },
		{ "Kumpul kan untuk memanggil Naga Tengkorak", "Thu thập để gọi Rồng Xương" },
		{ "Kumpul kan untuk memanggil Naga Tengkorak (otomatis dihapus setelah event)", "Thu thập để gọi Rồng Xương (tự động xóa sau sự kiện)" },
		{ "Kumpulkan untuk berharap Naga Es", "Thu thập để gọi Rồng Băng ước" },
		{ "Disguise when Fusion", "Cải trang khi Hợp Thể" },
		{ "Task Item", "Vật phẩm nhiệm vụ" },
		{ "Pesawat transport", "Tàu vũ trụ di chuyển" },
		{ "Pesawat transport VIP", "Tàu vũ trụ di chuyển VIP" },
		{ "Hapus semua Potential", "Tẩy tất cả điểm tiềm năng" },
		{ "Mengisi HP 100%", "Hồi phục 100% HP" },
		{ "Mengisi HP 20%", "Hồi phục 20% HP" },
		{ "Matikan PK", "Tắt cờ PK" },
		{ "Gunaan PK", "Bật cờ PK" },
		{ "Berisi item-item fantastis", "Chứa nhiều vật phẩm kì bí" },
		{ "Attack damage base +100% selama maksimal 10 menit", "Sức đánh gốc +100% trong tối đa 10 phút" },
		{ "HP +100% selama 10 menit", "HP +100% trong 10 phút" },
		{ "KI +100% selama 10 menit", "KI +100% trong 10 phút" },
		{ "Mengurangi damage yang diterima 50% selama 10 menit", "Giảm 50% sát thương nhận vào trong 10 phút" },
		{ "Semua serangan anggota clan +15%", "Tăng 15% sức đánh cho tất cả thành viên bang" },
		{ "HP dan KI semua anggota clan +20%", "Tăng 20% HP và KI cho tất cả thành viên bang" },
		{ "Flying mengisi KI", "Bay hồi phục KI" },
		{ "Flying mengisi HP, KI", "Bay hồi phục HP, KI" },
		{ "Digunakan untuk melacak Dragon Ball Namek", "Dùng để dò tìm Ngọc Rồng Namếc" }
	};
}
