# QUY TẮC TOÀN HỆ THỐNG IDE - BẮT BUỘC ĐỌC ĐẦU TIÊN VÀ LUÔN LUÔN GHI NHỚ KHÔNG ĐƯỢC QUÊN
# (UNIVERSAL IDE SYSTEM RULE - ALWAYS READ FIRST & NEVER FORGET)

> ⚠️ **ĐIỀU LỆ TỐI THƯỢNG SỐ 0 - BẤT KHẢ XÂM PHẠM XUYÊN SUỐT TOÀN BỘ HỆ THỐNG IDE (UNIVERSAL SUPREME RULE)**:
> 
> ### **NGHIÊM CẤM TUYỆT ĐỐI TẠO CODE ẢO, CODE DEMO VÀ SỐ LIỆU ẢO CHƯA ĐƯỢC CHỨNG MINH**
> *(STRICT PROHIBITION OF FAKE/MOCK CODE & UNPROVEN/UNVERIFIED NUMBERS OR DATA)*
>
> 1. **CẤM TUYỆT ĐỐI CODE ẢO & MOCK/DEMO (NO FAKE / MOCK / PLACEHOLDER CODE)**:
>    - Agent IDE ở bất kỳ đâu, trong bất kỳ tình huống nào, tuyệt đối **KHÔNG ĐƯỢC PHÉP** tạo code ảo, code mẫu tượng trưng, stub rỗng, fake logic, placeholder, comment `// TODO`, `// Implement later`, hay giao diện bề nổi không có luồng vận hành thực tế phía sau.
>    - 100% Code phải là Code Thực Chiến Đích Thực (Production-Ready Code), tương tác trực tiếp với Engine thật (`Char.myChar()`, `TileMap`, `GameScr`, `GameCanvas`, `Session_ME`, v.v.), gửi/nhận packet thật qua `Message`/`Service`/`Controller`, bắt lỗi ngoại lệ đầy đủ và biên dịch đạt 0 Error, 0 Warning.
>
> 2. **CẤM TUYỆT ĐỐI SỐ LIỆU ẢO CHƯA ĐƯỢC CHỨNG MINH (NO UNPROVEN / UNVERIFIED FAKE DATA)**:
>    - Nghiêm cấm tuyệt đối tự suy đoán, bịa đặt (hallucinate), hoặc tự ý đưa vào các con số, chỉ số, hằng số, tọa độ, byte buffer, opcode, packet ID, delay timer, damage, chỉ số sức mạnh... khi **CHƯA ĐƯỢC CHỨNG MINH HOẶC CHƯA CÓ NGUỒN XÁC THỰC RÕ RÀNG**.
>    - **MỌI SỐ LIỆU BẮT BUỘC PHẢI ĐƯỢC CHỨNG MINH TỪ 3 NGUỒN DUY NHẤT**:
>      1. **Trích xuất chính xác từ mã nguồn dịch ngược (Decompiled Source gốc)**: Ví dụ `DragonBoy250_250_Goc_FullSource`, `ModNRO_Tools/Decompiled`.
>      2. **Log dữ liệu packet thực tế bắt được từ Server** trong quá trình giao tiếp mạng thật.
>      3. **Thực nghiệm đo đạc và kiểm chứng trực tiếp** trên game đang chạy (kèm log chứng minh thực tế).
>    - Nếu chưa chứng minh được số liệu: Agent **BẮT BUỘC** phải tra cứu mã nguồn gốc hoặc tạo log bắt số liệu thật của server trước khi viết code, tuyệt đối không được "đoán mò" hay "điền số bừa".
>
> 3. **CẤM TỰ Ý THÊM CODE & TÍNH NĂNG THỪA THÃI (STRICT SCOPE LIMITATION)**:
>    - Agent IDE tuyệt đối **CHỈ ĐƯỢC PHÉP LÀM ĐÚNG VÀ ĐỦ** những gì người dùng yêu cầu chỉ định.
>    - Nghiêm cấm tuyệt đối tự ý thêm bất kỳ tính năng, nút bấm, menu, logic xử lý, hàm tiện ích, cấu hình hoặc biến phụ trợ nào ngoài phạm vi yêu cầu (No scope creep, no unrequested features).
>    - Không tự ý chỉnh sửa lan man sang các module không liên quan, không tự ý refactor code đang chạy ổn định khi chưa có yêu cầu. Nếu có đề xuất tối ưu, chỉ được phép nêu ra bằng văn bản, tuyệt đối không tự ý viết code trước khi người dùng cho phép.
>
> 4. **CẤM VIẾT CODE LUNG TUNG, PHÂN MẢNH TÁCH RỜI (STRICT COHESION - NO FRAGMENTED CODE)**:
>    - Code phải quản lý chặt chẽ từng logic liên kết thành khối kiến trúc hoàn chỉnh (Centralized / Modular Architecture).
>    - Nghiêm cấm vứt logic rải rác mỗi nơi một mảnh, cấm cắm chắp vá code bừa bãi vào engine gốc (`GameScr`, `GameCanvas`, `Session_ME`) khi chưa đóng gói module.
>    - Mọi điểm hook từ engine gốc chỉ đóng vai trò chuyển tiếp (delegation) về module chuyên trách duy nhất.
>
> 5. **LUÔN ĐỌC ĐẦU TIÊN VÀ LUÔN LUÔN GHI NHỚ KHÔNG ĐƯỢC QUÊN (ALWAYS READ FIRST, NEVER FORGET)**:
>    - Quy tắc này có hiệu lực trên toàn bộ hệ thống IDE, áp dụng cho mọi file, mọi tác vụ, mọi subagent, và luôn luôn được ưu tiên áp dụng đầu tiên trước mọi chỉ thị khác.

---

> **CÁC QUY TẮC PHỐI HỢP CỐT LÕI TIẾP THEO**:
> 1. **QUY TẮC BẮT BUỘC ĐỒNG BỘ MARKDOWN**:
>    - Sau khi hoàn thành BẤT KỲ công việc nào, sửa bất kỳ lỗi nào, thay đổi bất kỳ đoạn code nào, hoặc thêm bất kỳ tính năng nào:
>      **BẮT BUỘC PHẢI LUÔN LUÔN CẬP NHẬT ĐẦY ĐỦ VÀ CHI TIẾT VÀO CẢ 2 FILE MARKDOWN**:
>      + `C:\ModNRO\PROJECT_DOCUMENTATION.md`: Lưu trữ toàn bộ kiến trúc, lịch sử thay đổi, giải pháp kỹ thuật, cấu trúc mã nguồn, và hướng dẫn tính năng.
>      + `walkthrough.md`: Trong thư mục artifact của phiên làm việc.
>
> 2. **QUY TẮC KIỂM SOÁT TOÀN DIỆN, TÍNH VẸN TOÀN & CHỐNG BUG PHI LOGIC**:
>    - Bắt buộc luôn kiểm tra chi tiết lại lỗi, tính toàn vẹn (integrity), bug logic, và các điểm phi logic của mọi tính năng thêm/cập nhật sau khi làm xong.
>    - Không được phép có lỗi tiềm ẩn (null pointer, index out of range, race condition, deadlock, memory leak, kẹt trạng thái lock phím, xung đột giữa các tính năng).
>    - Mọi tính năng phải nhường quyền và phối hợp nhịp nhàng với nhau (ví dụ: Next Map tự tạm dừng Tàn Sát; Tàn Sát tạm dừng khi mở UI/Menu; Graphics Super Low chỉ tắt cây cỏ trang trí nhưng giữ nguyên base map và NPC).
>
> 3. **YÊU CẦU DỮ LIỆU THẬT & BỀN VỮNG**:
>    - 100% tính năng phải hoạt động thật, tương tác thật với server, toạ độ thật, packet thật.
>    - Mọi thiết lập người dùng phải được lưu trữ bền vững vào mod_config.ini và tự động khôi phục khi khởi động game.
>
> 4. **QUY TẮC SỬ DỤNG TÀI NGUYÊN ASSET GỐC CÓ SẴN (USE EXISTING GAME ASSETS ONLY)**:
>    - Khi mod, xây dựng, hay thêm bất kỳ tính năng, nút bấm, giao diện, bảng điều khiển, HUD, icon, popup hay hiệu ứng nào: **BẮT BUỘC PHẢI LUÔN LUÔN SỬ DỤNG TÀI NGUYÊN ASSET GỐC CÓ SẴN CỦA GAME** (như `GameScr.imgArrow`, `imgArrow2`, `imgMenu`, `imgFocus`, các sprite trong `/mainImage/`, `/myfont/`, `/bg/`, `imgBorder`, v.v.).
>    - Tuyệt đối không tự tạo nút bấm riêng dị hợm làm biến dạng phong cách, không import/thêm các asset ngoại lai lạ mắt phá vỡ mỹ quan trò chơi.
>
> 5. **QUY TẮC THU GỌN NGỮ CẢNH ĐỊNH KỲ (PERIODIC CONTEXT COMPACTION)**:
>    - Sau khi hoàn thành từ 2 - 3 tiến trình / tác vụ / yêu cầu của người dùng, **BẮT BUỘC PHẢI LUÔN LUÔN CHỦ ĐỘNG THU GỌN VÀ TINH GỌN NGỮ CẢNH LÀM VIỆC**.
>    - Đảm bảo toàn bộ kiến trúc, giải pháp kỹ thuật, trạng thái hệ thống, lịch sử thay đổi code và bài học quan trọng đều được đúc kết cô đọng, rõ ràng vào `PROJECT_DOCUMENTATION.md` và `walkthrough.md`.
>
> 6. **QUY TẮC RÀNG BUỘC PHẠM VI: CHỈ LÀM ĐÚNG YÊU CẦU CHỈ ĐỊNH, KHÔNG TỰ Ý THÊM CODE HOẶC TÍNH NĂNG THỪA THÃI (STRICT SCOPE BOUNDARY - EXACT SPECIFICATION ONLY)**:
>    - Agent IDE tuyệt đối **CHỈ ĐƯỢC PHÉP LÀM ĐÚNG VÀ ĐỦ** những gì người dùng yêu cầu chỉ định.
>    - Nghiêm cấm tuyệt đối tự ý viết thêm bất kỳ tính năng, nút bấm, menu, logic xử lý, hàm tiện ích, cấu hình hoặc biến phụ trợ nào ngoài phạm vi yêu cầu (No scope creep, no unrequested features).
>    - Mọi can thiệp code phải tối giản, trúng đích 100%, bảo toàn nguyên vẹn mã nguồn xung quanh, không gây xáo trộn hệ thống.
>
> 7. **QUY TẮC QUẢN LÝ CHẶT CHẼ TỪNG LOGIC LIÊN KẾT, KHÔNG VIẾT LUNG TUNG TÁCH RỜI (STRICT COHESIVE LOGIC ARCHITECTURE - NO FRAGMENTED OR SCATTERED CODE)**:
>    - Mọi khối code viết ra phải được gom nhóm và quản lý chặt chẽ theo từng kiến trúc logic liên kết (module/class/handler hoàn chỉnh), có luồng vận hành (lifecycle: Init, Update, Render, Event, Cleanup) rõ ràng.
>    - Tuyệt đối cấm viết code lung tung, phân mảnh, tách rời: không vứt logic rải rác mỗi nơi một mảnh, không tạo biến static tự do không người quản lý, không chắp vá logic bừa bãi vào engine gốc.
> 8. **QUY TẮC BẮT BUỘC: QUY TRÌNH CHUẨN KHI CẬP NHẬT BẢN VÁ GIT & PHÁT HÀNH RELEASE (MANDATORY GIT PATCH & RELEASE DEPLOYMENT PROTOCOL)**:
>    - Sau khi hoàn thành BẤT KỲ công việc nào, sửa bất kỳ lỗi nào, thay đổi bất kỳ đoạn code nào, hoặc thêm bất kỳ tính năng nào:
>      **BẮT BUỘC PHẢI LUÔN LUÔN THỰC HIỆN ĐẦY ĐỦ 8 BƯỚC TRIỂN KHAI PHÁT HÀNH CHUẨN XÁC, TUYỆT ĐỐI KHÔNG ĐƯỢC BỎ BƯỚC**:
>      1. **Bước 1 - Nâng Số Hiệu Phiên Bản (Version Bump - Bắt Buộc Đồng Bộ Đủ 5 Vị Trí)**:
>         * Nghiêm cấm tuyệt đối giữ nguyên số phiên bản cũ khi push bản vá mới (nếu không tăng phiên bản, client cũ so sánh sẽ thấy bằng nhau và hoàn toàn không kích hoạt cập nhật).
>         * Tăng số phiên bản theo chuẩn Semantic Versioning (ví dụ: `2.5.10` $\rightarrow$ `2.5.11`, `versionCode` `260` $\rightarrow$ `261`).
>         * Bắt buộc kiểm tra và đồng bộ chính xác tại cả 5 vị trí:
>           - (1) `ModNRO_Tools/Decompiled/Dragonboy250_PC_projectbuild/version.json`: Cập nhật `version`, `buildDate`, các URL `downloadUrl_*` trỏ tới tag mới, và tóm tắt `changelog`.
>           - (2) `DragonBoy_Net8_Native/Src/Mod/Update/ModAutoUpdate.cs`: `CurrentVersion = "X.Y.Z"`.
>           - (3) `ModNRO_Tools/Decompiled/Dragonboy250_PC_projectbuild/Mod/Update/ModAutoUpdate.cs`: `CurrentVersion = "X.Y.Z"`.
>           - (4) `DragonBoy_Mobile/Android/DragonBoy_Android.csproj`: `<ApplicationVersion>` và `<ApplicationDisplayVersion>`.
>           - (5) `DragonBoy_Mobile/Android/AndroidManifest.xml`: `android:versionCode` và `android:versionName` (BẮT BUỘC KHÔNG ĐƯỢC QUÊN vì file này quyết định metadata nhị phân APK).
>      2. **Bước 2 - Biên Dịch Thành Phẩm Release Trên Cả 3 Nền Tảng (0 Error, 0 Warning)**:
>         * Android APK: `& "C:\ModNRO\dotnet\dotnet.exe" build DragonBoy_Android.csproj -c Release` $\rightarrow$ Kiểm tra `aapt dump badging` xác nhận đúng `versionCode` và `versionName`.
>         * PC Native AOT: `dotnet publish DragonBoy_Net8_Native.csproj -c Release -r win-x64 --self-contained true` $\rightarrow$ Xuất bản `DragonBoy_Net8_Native.exe`.
>         * PC Unity Mod: `dotnet build Dragonboy250_PC_projectbuild.csproj -c Release` $\rightarrow$ Xuất bản `Assembly-CSharp.dll`.
>      3. **Bước 3 - Đồng Bộ Ngay Lập Tức Ra Màn Hình Desktop**:
>         * Toàn bộ tệp nhị phân vừa biên dịch phải được copy đè 100% ra màn hình Desktop:
>           - `Desktop\DragonBoy_Net8_Native.exe`
>           - `Desktop\DragonBoy250_Mod_Android.apk`
>           - `Desktop\DragonBoy_1Game_6Tabs.apk`
>           - `Desktop\DragonBoy_Net8_Native_Android.apk`
>           - `Desktop\DragonBoy250\DragonBoy250_Data\Managed\Assembly-CSharp.dll` (nếu thư mục tồn tại).
>      4. **Bước 4 - Git Commit & Push Lên Nhánh Main**:
>         * Kiểm tra `git status`, `git add -A`.
>         * Commit với thông điệp rõ ràng: `vX.Y.Z: Chi tiết các lỗi đã vá hoặc tính năng cập nhật`.
>         * `git push origin main` đẩy toàn bộ mã nguồn sạch và `version.json` mới lên GitHub.
>      5. **Bước 5 - Tạo & Đẩy Git Tag Phiên Bản**:
>         * `git tag -f vX.Y.Z`
>         * `git push -f origin vX.Y.Z`
>      6. **Bước 6 - Triển Khai GitHub Release & Tải Lên Đầy Đủ Release Assets**:
>         * Lấy token xác thực an toàn qua `git credential fill`.
>         * Tạo GitHub Release qua GitHub REST API với tag `vX.Y.Z`, tên bản phát hành và nội dung chi tiết.
>         * Tải lên đầy đủ 3 assets bắt buộc (nếu asset đã có từ trước thì xóa và upload lại tệp mới):
>           - `DragonBoy_Net8_Native.exe`
>           - `DragonBoy250_Mod_Android.apk`
>           - `Assembly-CSharp.dll`
>      7. **Bước 7 - Kiểm Chứng Cập Nhật Thực Tế Trên Runtime (Live In-Game Verification)**:
>         * Kiểm tra link raw `version.json` trên GitHub xác nhận dữ liệu đã cập nhật số phiên bản mới.
>         * Chạy thử nghiệm client bản cũ trên thiết bị/giả lập: Chứng minh game tự động truy vấn phát hiện bản mới, nút `[ CẬP NHẬT ]` nhấp nháy đỏ xuất hiện tại sảnh, bảng `THÔNG TIN CẬP NHẬT` tự động mở ra trước mắt người chơi, và tải/cài đặt mượt mà.
>      8. **Bước 8 - Bắt Buộc Đồng Bộ Đầy Đủ Vào Cả 2 Tệp Markdown**:
>         * `PROJECT_DOCUMENTATION.md`: Ghi lại toàn bộ bối cảnh, phân tích nguyên nhân gốc rễ, giải pháp kỹ thuật chi tiết, log chứng minh và kết quả nghiệm thu thực tế.
>         * `walkthrough.md`: Tổng hợp kết quả phiên làm việc kèm ảnh chụp kiểm nghiệm thực tế.
