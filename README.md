# 💊 Hệ Thống Quản Lý Nhà Thuốc — QLNhaThuoc (N-Layer + DTO)

Hệ thống quản lý nhà thuốc chuyên nghiệp, được phát triển bằng **C# .NET 9.0 (ASP.NET Core MVC)** theo chuẩn kiến trúc **N-Layer** với phân tách tầng rõ ràng qua DTO.

---

## 🏗️ Cấu Trúc Kiến Trúc N-Layer

```
QLNhaThuoc_NLayer/
├── QLNhaThuoc.DTO/          # Lớp dữ liệu dùng chung (Data Transfer Objects)
├── QLNhaThuoc.DAL/          # Truy xuất CSDL qua ADO.NET (SqlConnection/SqlCommand)
├── QLNhaThuoc.BUS/          # Xử lý nghiệp vụ (Business Logic)
├── QLNhaThuocApp/           # Giao diện Web (ASP.NET Core MVC)
│   └── appsettings.json     # ← Cấu hình connection string tại đây
└── DatabaseQuery.sql        # Script tạo CSDL + dữ liệu mẫu đầy đủ
```

**Luồng dữ liệu:** `Controller (Web) → BUS → DAL → SQL Server`

---

## 💾 Bước 1: Cài Đặt Cơ Sở Dữ Liệu

File `DatabaseQuery.sql` ở thư mục gốc sẽ tự động tạo toàn bộ CSDL, bảng và **40 sản phẩm thuốc mẫu** kèm hóa đơn, nhân viên, khách hàng.

### ▶ Cách 1: Dùng dòng lệnh (Nhanh nhất)

Mở **PowerShell** hoặc **Command Prompt**, chạy lệnh sau:

```powershell
sqlcmd -S "(localdb)\MSSQLLocalDB" -i "đường_dẫn_đến\QLNhaThuoc_NLayer\DatabaseQuery.sql"
```

**Ví dụ cụ thể** (thay đường dẫn đúng với máy của bạn):
```powershell
sqlcmd -S "(localdb)\MSSQLLocalDB" -i "D:\QLNhaThuoc_NLayer\DatabaseQuery.sql"
```

> Nếu máy chưa có `sqlcmd`, xem phần **"Yêu Cầu Phần Mềm"** bên dưới.

### ▶ Cách 2: Dùng SQL Server Management Studio (SSMS)

1. Mở **SSMS** → Kết nối tới server: `(localdb)\MSSQLLocalDB`
2. Vào **File → Open → File...** → chọn file `DatabaseQuery.sql`
3. Nhấn **F5** hoặc nút **Execute** để chạy

### ▶ Cách 3: Dùng Visual Studio

1. Mở Solution `QLNhaThuoc.sln`
2. Vào **View → SQL Server Object Explorer**
3. Kết nối tới `(localdb)\MSSQLLocalDB`
4. Nhấp chuột phải → **New Query** → dán nội dung file `DatabaseQuery.sql` → **Execute**

---

## 🔌 Bước 2: Cấu Hình Connection String

File cần sửa: `QLNhaThuocApp/appsettings.json`

### 🟢 Trường hợp dùng LocalDB (mặc định — dành cho máy có Visual Studio)

Đây là cấu hình mặc định trong file, **không cần sửa gì**:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=QLNhaThuoc;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
}
```

> **LocalDB** là bản SQL Server nhẹ đi kèm Visual Studio, dùng cho phát triển. Tên server luôn là `(localdb)\MSSQLLocalDB`.

---

### 🟡 Trường hợp dùng SQL Server Express (cài riêng, không có Visual Studio)

Nếu bạn cài **SQL Server Express** (tải miễn phí tại microsoft.com), tên server thường là `.\SQLEXPRESS`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.\\SQLEXPRESS;Database=QLNhaThuoc;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
}
```

Cũng cần chạy script theo địa chỉ server tương ứng:
```powershell
sqlcmd -S ".\SQLEXPRESS" -i "D:\QLNhaThuoc_NLayer\DatabaseQuery.sql"
```

---

### 🟠 Trường hợp dùng SQL Server đầy đủ (Full SQL Server)

Nếu có SQL Server cài trên máy (không phải LocalDB hay Express):

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=TEN_MAY_TINH\\TEN_INSTANCE;Database=QLNhaThuoc;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
}
```

**Ví dụ:** Nếu tên máy là `LAPTOP-ABC` và instance mặc định (MSSQLSERVER):
```json
"Server=LAPTOP-ABC;Database=QLNhaThuoc;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
```

---

### 🔴 Trường hợp dùng SQL Server với tài khoản SQL (username/password)

Nếu SQL Server không dùng Windows Authentication mà dùng SQL Authentication:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=QLNhaThuoc;User Id=sa;Password=mat_khau_cua_ban;TrustServerCertificate=True;MultipleActiveResultSets=true"
}
```

---

### 🔍 Cách tìm tên server SQL đang chạy trên máy

Mở **PowerShell** và chạy:
```powershell
# Liệt kê tất cả SQL Server instances đang chạy
Get-Service | Where-Object {$_.DisplayName -like "SQL Server*" -and $_.Status -eq "Running"}

# Hoặc dùng sqlcmd để kiểm tra LocalDB
sqllocaldb info
sqllocaldb info MSSQLLocalDB
```

Nếu `sqllocaldb info` ra kết quả có `MSSQLLocalDB` → dùng connection string LocalDB là được.

---

## 🚀 Bước 3: Khởi Chạy Ứng Dụng

Sau khi đã tạo CSDL và cấu hình connection string, mở **PowerShell** tại thư mục dự án:

```powershell
dotnet run --project QLNhaThuocApp
```

Ứng dụng khởi động tại: 🔗 **http://localhost:5264**

### Tài khoản đăng nhập mẫu:

| Tên đăng nhập | Mật khẩu | Chức vụ     | Quyền                          |
|---------------|----------|-------------|--------------------------------|
| `admin`       | `123456` | Admin       | Toàn quyền                     |
| `banhang`     | `123456` | Bán Hàng    | Bán hàng, lịch sử, sự cố      |
| `banhang2`    | `123456` | Bán Hàng    | Bán hàng, lịch sử, sự cố      |
| `thukho`      | `123456` | Thủ Kho     | Quản lý kho, nhập/xuất kho     |
| `quanly`      | `123456` | Quản Lý     | Chỉ xem báo cáo/dashboard      |

---

## 🖥️ Yêu Cầu Phần Mềm

| Phần mềm | Phiên bản | Ghi chú |
|----------|-----------|---------|
| **.NET SDK** | 9.0 trở lên | [Tải tại dotnet.microsoft.com](https://dotnet.microsoft.com/download) |
| **SQL Server** | LocalDB / Express / Full | LocalDB đi kèm Visual Studio |
| **Visual Studio** (tùy chọn) | 2022+ | Có thể chạy bằng `dotnet run` mà không cần VS |

### Kiểm tra .NET đã cài chưa:
```powershell
dotnet --version
# Phải hiện 9.x.x trở lên
```

### Cài LocalDB nếu chưa có (không có Visual Studio):

Tải **SQL Server Express LocalDB** miễn phí:  
👉 https://www.microsoft.com/en-us/sql-server/sql-server-downloads  
(Chọn bản **Express**, trong lúc cài chọn thêm **LocalDB**)

---

## ❓ Xử Lý Lỗi Thường Gặp

### Lỗi: `A network-related or instance-specific error...`
**Nguyên nhân:** Connection string sai tên server, hoặc SQL Server chưa chạy.  
**Khắc phục:**
```powershell
# Kiểm tra LocalDB có đang chạy không
sqllocaldb info MSSQLLocalDB
# Nếu State = "Stopped", khởi động lại:
sqllocaldb start MSSQLLocalDB
```

### Lỗi: `Cannot open database "QLNhaThuoc" requested by the login`
**Nguyên nhân:** Chưa chạy file `DatabaseQuery.sql`.  
**Khắc phục:** Thực hiện lại **Bước 1** ở trên.

### Lỗi: `SSL connection error` hoặc lỗi certificate
**Nguyên nhân:** Thiếu `TrustServerCertificate=True` trong connection string.  
**Khắc phục:** Đảm bảo connection string có đủ tham số như mẫu ở trên.

### Lỗi port `5264` bị chiếm
**Khắc phục:** Sửa port trong `QLNhaThuocApp/Properties/launchSettings.json`:
```json
"applicationUrl": "http://localhost:5300"
```

---

## 📦 Dữ Liệu Mẫu Bao Gồm

Script `DatabaseQuery.sql` tạo sẵn:
- ✅ **40 sản phẩm thuốc** (kháng sinh, hạ sốt, tiêu hóa, vitamin, tim mạch, hô hấp, ngoài da, dị ứng, tiểu đường, TPCN)
- ✅ **10 danh mục** thuốc
- ✅ **5 nhân viên** với đủ các chức vụ
- ✅ **8 khách hàng** có điểm tích lũy
- ✅ **5 nhà cung cấp** (DHG, Pymepharco, Sanofi, Traphaco, Imexpharm)
- ✅ **3 kho** (Chính, Phụ, Lạnh)
- ✅ **10 hóa đơn** + chi tiết bán hàng
- ✅ **5 phiếu nhập kho** + chi tiết
- ✅ **2 báo cáo sự cố** mẫu
