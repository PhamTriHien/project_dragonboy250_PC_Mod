public static partial class ModTranslate
{
	// Bảng ánh xạ tiếng Việt chuẩn xác cho toàn bộ 187 Maps (ID 0 đến 186)
	public static readonly string[] MapNames = new string[]
	{
		"Làng Aru", // 0
		"Đồi hoa cúc", // 1
		"Thung lũng tre", // 2
		"Rừng nấm", // 3
		"Rừng xương", // 4
		"Đảo Kamê", // 5
		"Đông Karin", // 6
		"Làng Mori", // 7
		"Đồi nấm tím", // 8
		"Thị trấn Moori", // 9
		"Thung lũng Namếc", // 10
		"Thung lũng Maima", // 11
		"Vực Maima", // 12
		"Đảo Guru", // 13
		"Làng Kakarot", // 14
		"Đồi hoang", // 15
		"Làng Plant", // 16
		"Rừng nguyên sinh", // 17
		"Rừng thông Xayda", // 18
		"Thành phố Vegeta", // 19
		"Vách núi đen", // 20
		"Nhà Gôhan", // 21
		"Nhà Moori", // 22
		"Nhà Broly", // 23
		"Trạm tàu vũ trụ Trái Đất", // 24
		"Trạm tàu vũ trụ Namếc", // 25
		"Trạm tàu vũ trụ Xayda", // 26
		"Rừng trúc", // 27
		"Rừng dương xỉ", // 28
		"Nam Kamê", // 29
		"Đảo Bulông", // 30
		"Núi hoa vàng", // 31
		"Núi hoa tím", // 32
		"Nam Guru", // 33
		"Đông Nam Guru", // 34
		"Rừng cọ", // 35
		"Rừng đá", // 36
		"Thung lũng đen", // 37
		"Bờ vực đen", // 38
		"Vách núi Aru", // 39
		"Vách núi Moori", // 40
		"Vực cấm", // 41
		"Vách núi Aru", // 42
		"Vách núi Moori", // 43
		"Vách núi Kakarot", // 44
		"Thần điện", // 45
		"Tháp Karin", // 46
		"Rừng Karin", // 47
		"Hành tinh Kaio", // 48
		"Phòng tập thời gian", // 49
		"Thánh địa Kaio", // 50
		"Đấu trường", // 51
		"Đại hội võ thuật", // 52
		"Tường thành 1", // 53
		"Tầng 3", // 54
		"Tầng 1", // 55
		"Tầng 2", // 56
		"Tầng 4", // 57
		"Tường thành 2", // 58
		"Tường thành 3", // 59
		"Trại Độc Nhãn 1", // 60
		"Trại Độc Nhãn 2", // 61
		"Trại Độc Nhãn 3", // 62
		"Trại lính Fide", // 63
		"Núi dây leo", // 64
		"Núi cây quỷ", // 65
		"Trại quỷ già", // 66
		"Vực chết", // 67
		"Thung lũng Nappa", // 68
		"Vực tử thần", // 69
		"Núi Appule", // 70
		"Căn cứ Raspberry", // 71
		"Thung lũng Raspberry", // 72
		"Thung lũng chết", // 73
		"Đồi cây Fide", // 74
		"Vách núi chết", // 75
		"Núi đá", // 76
		"Rừng đá", // 77
		"Lãnh địa Fide", // 78
		"Núi khỉ đỏ", // 79
		"Núi khỉ vàng", // 80
		"Hang quỷ chim", // 81
		"Núi khỉ đen", // 82
		"Hang khỉ đen", // 83
		"Siêu thị", // 84
		"Hành tinh M-2", // 85
		"Hành tinh Polaris", // 86
		"Hành tinh Cretaceous", // 87
		"Hành tinh Monmaasu", // 88
		"Hành tinh Rudeeze", // 89
		"Hành tinh Gelbo", // 90
		"Hành tinh Tigere", // 91
		"Thành phố phía đông", // 92
		"Thành phố phía nam", // 93
		"Quần đảo Sasebo", // 94
		"Thành phố Vegeta", // 95
		"Cao nguyên", // 96
		"Thành phố phía bắc", // 97
		"Núi phía bắc", // 98
		"Thung lũng phía bắc", // 99
		"Thị trấn Ginder", // 100
		"Võ đài liên vũ trụ", // 101
		"Nhà Bunma", // 102
		"Võ đài Xên Bọ Hung (Cell)", // 103
		"Sân sau Siêu thị", // 104
		"Cánh đồng tuyết", // 105
		"Rừng tuyết", // 106
		"Núi tuyết", // 107
		"Dòng sông băng", // 108
		"Rừng băng", // 109
		"Hang băng", // 110
		"Đông Nam Karin", // 111
		"Võ đài Bà Hạt Mít", // 112
		"Đại hội võ thuật", // 113
		"Cửa phi thuyền", // 114
		"Phòng chờ", // 115
		"Vùng đất bí mật Kaio", // 116
		"Phòng 1", // 117
		"Phòng 2", // 118
		"Phòng 3", // 119
		"Phòng chỉ huy", // 120
		"Đấu trường", // 121
		"Hỏa Diệm Sơn 1", // 122
		"Hỏa Diệm Sơn 2", // 123
		"Hỏa Diệm Sơn 3", // 124
		"Đại hội bang hội", // 125
		"Thành phố Santa", // 126
		"Cửa phi thuyền", // 127
		"Bụng Mabư", // 128
		"Đại hội võ thuật", // 129
		"Đại hội võ thuật liên vũ trụ", // 130
		"Hành tinh Yardrat 1", // 131
		"Hành tinh Yardrat 2", // 132
		"Hành tinh Yardrat 3", // 133
		"Đại hội võ thuật Vũ trụ 6 - 7", // 134
		"Hang hải tặc", // 135
		"Hang bạch tuộc", // 136
		"Hang kho báu", // 137
		"Cảng hải tặc", // 138
		"Hành tinh Potaufeu", // 139
		"Hang Potaufeu", // 140
		"Con đường rắn độc 1", // 141
		"Con đường rắn độc 2", // 142
		"Con đường rắn độc 3", // 143
		"Vùng hoang vu", // 144
		"Đại hội siêu cấp", // 145
		"Tây Karin", // 146
		"Sa mạc", // 147
		"Lâu đài quỷ", // 148
		"Thành phố Santa", // 149
		"Võ đài offline", // 150
		"Hành tinh bóng tối", // 151
		"Vùng đất băng giá", // 152
		"Khu vực bang hội", // 153
		"Hành tinh Bill", // 154
		"Hành tinh ngục tù 1", // 155
		"Thánh địa Tây Kaio", // 156
		"Thánh địa Đông Kaio", // 157
		"Thánh địa Bắc Kaio", // 158
		"Thánh địa Nam Kaio", // 159
		"Khu vực hang động", // 160
		"Bìa rừng nguyên sinh", // 161
		"Rừng nguyên sinh", // 162
		"Làng Plant cổ đại", // 163
		"Thung lũng Maima", // 164
		"Lãnh địa Boss", // 165
		"Hành tinh ngục tù 2", // 166
		"Hồ Máu 1", // 167
		"Hồ Máu 2", // 168
		"Hồ Máu 3", // 169
		"Phòng tập thời gian", // 170
		"Hồ Máu 4", // 171
		"Phòng chờ thời gian", // 172
		"Cánh đồng tuyết 2", // 173
		"Rừng tuyết 2", // 174
		"Hang băng 2", // 175
		"Nam Kamê mới", // 176
		"Rừng nguyên sinh", // 177
		"Núi phía bắc", // 178
		"Phòng thí nghiệm Myuu", // 179
		"Mù căng chải", // 180
		"Sa mạc hoang vu", // 181
		"Hành tinh Bill", // 182
		"Hành tinh Bill", // 183
		"Tường thành cổ 1", // 184
		"Tường thành cổ 2", // 185
		"Đấu trường thành cổ", // 186
	};
}
