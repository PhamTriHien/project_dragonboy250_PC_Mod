# Project DragonBoy 2.5.0 PC Mod (C# / Unity)

Dự án mã nguồn hoàn chỉnh Mod Ngọc Rồng Online (Dragon Boy 2.5.0 PC), được tái cấu trúc dạng cây mô-đun (Tree Modular Architecture) và tối ưu hóa chuyên sâu.

## 🚀 Tính Năng Chính
- **Kiến trúc nhánh cây (Tree Modular Architecture)**: Phân chia mã nguồn thành các module độc lập, gọn gàng, 0 tệp lẻ ở thư mục gốc.
- **Tàn Sát Đỉnh Cao (Teleport Farming)**: Thuật toán tiếp cận quái Euclid gần nhất, đánh tối ưu khoảng cách, watchdog chống quái ma, tự chọn chiêu thức hồi nhanh nhất.
- **Next Map BFS 3 Hành Tinh**: Tìm đường ngắn nhất giữa 44 bản đồ, chuyển trạm tàu vũ trụ và waypoint tự động.
- **Tự Động Hóa (Automation)**:
  - Tự động nhặt item thật theo toạ độ server (All / Vàng / Trang bị / Ngọc).
  - Tự động dùng đậu thần thật \%$, khóa HP/MP.
  - Tùy chỉnh tốc độ di chuyển Speed Hack (.0\times - 3.0\times$).
- **Đồ Họa & Hiệu Năng**:
  - 4 cấp đồ họa: Ultra, Medium, Low (phông nền xanh nhạt), Super Low (chỉ giữ Base Map & NPC).
  - Tự động đồng bộ tần số quét màn hình (60Hz, 120Hz, 144Hz, 165Hz, 240Hz).
  - Hiển thị HUD FPS & Ping thời gian thực.
  - Zero-Allocation Texture rendering (triệt tiêu lỗi DirectX 11 0x887A0005 và màn hình đen).
- **HUD Thông Báo Boss**: Bắt gói tin server, xếp chồng 6 thông báo Boss gần nhất.
- **Giao Diện Mod 7 Tab**: Nút mũi tên kinh điển bên góc trái (imgArrow / imgArrow2), popup modal 7 tab tiện lợi.
- **Phím Tắt PC**:
  - ~ (BackQuote) hoặc F2: Bật/Tắt Mod Menu.
  - M: Bật/Tắt Menu Game gốc.
  - K: Bật/Tắt Mod Menu.
- **Lưu Trữ Bền Vững**: Tự động lưu/khôi phục toàn bộ cấu hình vào mod_config.ini.

## 🛠 Hướng Dẫn Biên Dịch (Build)

### Yêu Cầu:
- .NET SDK (hỗ trợ target framework 
et35)
- UnityEngine.dll (tại DragonBoy250_pc/DragonBoy250_Data/Managed/UnityEngine.dll)

### Lệnh Biên Dịch:
`ash
dotnet build Dragonboy250_PC_projectbuild.csproj -c Release
`
File output Assembly-CSharp.dll sẽ được tạo tại in/Release/net35/Assembly-CSharp.dll.

## 📂 Cấu Trúc Cây Thư Mục
`
Dragonboy250_PC_projectbuild/
├── Core/               # App, Collections, IO, Network, Input, Interfaces
├── Graphics/           # Image, Paint
├── Audio/              # Sound, AudioClip
├── Model/              # Item, Skill, Task, Clan, Player, Npc, Map, Darts
├── UI/                 # Screens, Dialogs, Controls, HUD
├── Effects/            # Hiệu ứng chiêu thức, pháo hoa, server effects
├── Mob/                # Quái vật, captcha
├── Mod/                # Hệ thống Mod (Core, TanSat, NextMap, Automation, Graphics, Boss, UI)
├── Char/               # Phân rã mô-đun lớp Char
├── Panel/              # Phân rã mô-đun lớp Panel
├── GameScr/            # Phân rã mô-đun GameScr
├── GameCanvas/         # Phân rã mô-đun GameCanvas
├── ServerListScreen/   # Sảnh chọn máy chủ
├── LoginScr/           # Màn hình đăng nhập
├── BachTuoc/           # Boss Bạch Tuộc
├── Controller/         # Message handlers
├── Service/            # Senders, Protocols
├── Session_ME/         # Socket TCP, Trao đổi khóa
└── Res/                # Tài nguyên, từ điển dịch thuật
`

## 📖 Tài Liệu Chi Tiết
Xem toàn bộ kiến trúc, giải pháp kỹ thuật và lịch sử thay đổi tại [PROJECT_DOCUMENTATION.md](PROJECT_DOCUMENTATION.md).
