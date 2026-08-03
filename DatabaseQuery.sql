-- ============================================================
--  QLNhaThuoc - Database Script
--  Tương thích với codebase N-Layer (DAL / DTO)
--  Lưu file dưới dạng UTF-8 (không BOM) hoặc UTF-16
-- ============================================================

-- Tạo & chọn database
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'QLNhaThuoc')
    CREATE DATABASE QLNhaThuoc;
GO
USE QLNhaThuoc;
GO

-- ============================================================
-- 1. Bảng Khách Hàng
-- ============================================================
IF OBJECT_ID(N'KhachHang', 'U') IS NULL
CREATE TABLE KhachHang (
    MaKH         VARCHAR(20)    NOT NULL PRIMARY KEY,
    Ho           NVARCHAR(30)   NOT NULL,
    Ten          NVARCHAR(20)   NOT NULL,
    SoDT         VARCHAR(15)    NOT NULL,
    Email        VARCHAR(100)   NULL,
    DiaChi       NVARCHAR(MAX)  NULL,
    DiemTichLuy  INT            NOT NULL DEFAULT 0,
    HangThanhVien NVARCHAR(20)  NULL  -- VD: 'Bạc', 'Vàng', 'Kim cương'
);
GO

-- ============================================================
-- 2. Bảng Danh Mục
-- ============================================================
IF OBJECT_ID(N'DanhMuc', 'U') IS NULL
CREATE TABLE DanhMuc (
    MaDM  VARCHAR(20)    NOT NULL PRIMARY KEY,
    TenDM NVARCHAR(100)  NOT NULL,
    MoTa  NVARCHAR(MAX)  NULL
);
GO

-- ============================================================
-- 3. Bảng Sản Phẩm / Thuốc
--    Bổ sung cột CongDung (DAL đọc reader["CongDung"])
-- ============================================================
IF OBJECT_ID(N'SanPham', 'U') IS NULL
CREATE TABLE SanPham (
    MaSP            VARCHAR(20)    NOT NULL PRIMARY KEY,
    TenSP           NVARCHAR(100)  NOT NULL,
    MaDM            VARCHAR(20)    NULL REFERENCES DanhMuc(MaDM),
    DonViTinh       NVARCHAR(20)   NULL,
    ThanhPhan       NVARCHAR(MAX)  NULL,
    CongDung        NVARCHAR(MAX)  NULL,   -- << thêm mới, DAL dùng reader["CongDung"]
    GiaBan          DECIMAL(18,2)  NOT NULL DEFAULT 0,
    SoLuongTonKho   INT            NOT NULL DEFAULT 0,
    MoTa            NVARCHAR(MAX)  NULL
);
GO

-- ============================================================
-- 4. Bảng Nhân Viên
-- ============================================================
IF OBJECT_ID(N'NhanVien', 'U') IS NULL
CREATE TABLE NhanVien (
    MaNV         VARCHAR(20)   NOT NULL PRIMARY KEY,
    Ho           NVARCHAR(30)  NOT NULL,
    Ten          NVARCHAR(20)  NOT NULL,
    SoDT         VARCHAR(15)   NOT NULL,
    Email        VARCHAR(100)  NOT NULL,
    DiaChi       NVARCHAR(MAX) NULL,
    Luong        DECIMAL(18,2) NOT NULL DEFAULT 0,
    ChucVu       NVARCHAR(30)  NOT NULL,  -- 'Admin', 'Ban Hang', 'Kho', 'Ke Toan'
    TenDangNhap  VARCHAR(50)   NOT NULL UNIQUE,
    MatKhau      VARCHAR(255)  NOT NULL
);
GO

-- ============================================================
-- 5. Hóa Đơn & Chi Tiết Hóa Đơn
-- ============================================================
IF OBJECT_ID(N'HoaDon', 'U') IS NULL
CREATE TABLE HoaDon (
    MaHD                 VARCHAR(20)   NOT NULL PRIMARY KEY,
    NgayXuatHD           DATETIME      NOT NULL DEFAULT GETDATE(),
    MaNV                 VARCHAR(20)   NULL REFERENCES NhanVien(MaNV),
    MaKH                 VARCHAR(20)   NULL REFERENCES KhachHang(MaKH),
    TongTien             DECIMAL(18,2) NOT NULL DEFAULT 0,
    PhuongThucThanhToan  NVARCHAR(50)  NULL,
    DiemSuDung           INT           NOT NULL DEFAULT 0,
    TienGiamTru          DECIMAL(18,2) NOT NULL DEFAULT 0
);
GO

IF OBJECT_ID(N'ChiTietHoaDon', 'U') IS NULL
CREATE TABLE ChiTietHoaDon (
    MaHD      VARCHAR(20)   NOT NULL REFERENCES HoaDon(MaHD),
    MaSP      VARCHAR(20)   NOT NULL REFERENCES SanPham(MaSP),
    SoLuong   INT           NOT NULL,
    DonGia    DECIMAL(18,2) NOT NULL,
    ThanhTien DECIMAL(18,2) NOT NULL,
    PRIMARY KEY (MaHD, MaSP)
);
GO

-- ============================================================
-- 6. Kho, Nhà Cung Cấp, Phiếu Nhập / Xuất Kho
-- ============================================================
IF OBJECT_ID(N'Kho', 'U') IS NULL
CREATE TABLE Kho (
    MaKho    VARCHAR(20)    NOT NULL PRIMARY KEY,
    TenKho   NVARCHAR(100)  NOT NULL,
    DiaChi   NVARCHAR(MAX)  NULL,
    TrangThai NVARCHAR(50)  NULL  -- N'Hoat dong', N'Ngung hoat dong'
);
GO

IF OBJECT_ID(N'NhaCungCap', 'U') IS NULL
CREATE TABLE NhaCungCap (
    MaNCC   VARCHAR(20)    NOT NULL PRIMARY KEY,
    TenNCC  NVARCHAR(100)  NULL,
    SoDT    VARCHAR(15)    NULL,
    Email   VARCHAR(100)   NULL,
    DiaChi  NVARCHAR(MAX)  NULL
);
GO

IF OBJECT_ID(N'PhieuNhapKho', 'U') IS NULL
CREATE TABLE PhieuNhapKho (
    MaPNK        VARCHAR(20)   NOT NULL PRIMARY KEY,
    NgayNhap     DATETIME      NOT NULL DEFAULT GETDATE(),
    MaNV         VARCHAR(20)   NULL REFERENCES NhanVien(MaNV),
    MaKho        VARCHAR(20)   NULL REFERENCES Kho(MaKho),
    MaNCC        VARCHAR(20)   NULL REFERENCES NhaCungCap(MaNCC),
    TongTienNhap DECIMAL(18,2) NOT NULL DEFAULT 0,
    TrangThai    NVARCHAR(50)  NULL  -- N'Hoan Thanh', N'Dang nhap', N'Huy'
);
GO

IF OBJECT_ID(N'ChiTietNhapKho', 'U') IS NULL
CREATE TABLE ChiTietNhapKho (
    MaPNK       VARCHAR(20)   NOT NULL REFERENCES PhieuNhapKho(MaPNK),
    MaSP        VARCHAR(20)   NOT NULL REFERENCES SanPham(MaSP),
    SoLo        VARCHAR(50)   NULL,
    NgaySanXuat DATE          NULL,
    HanSuDung   DATE          NULL,
    SoLuongNhap INT           NOT NULL,
    GiaNhap     DECIMAL(18,2) NOT NULL,
    PRIMARY KEY (MaPNK, MaSP)
);
GO

-- ============================================================
--  PhieuXuatKho – cấu trúc mới phù hợp với KhoDAL.cs
--  DAL insert: (MaPXK, NgayXuat, GiaXuat, SoLuong, MaKho, MaNV, MaSP)
--  và query: SELECT MaPXK, NgayXuat, SoLuong, GiaXuat, TenSP, DonViTinh, MaSP
-- ============================================================
IF OBJECT_ID(N'PhieuXuatKho', 'U') IS NULL
CREATE TABLE PhieuXuatKho (
    MaPXK     VARCHAR(20)   NOT NULL PRIMARY KEY,
    NgayXuat  DATETIME      NOT NULL DEFAULT GETDATE(),
    MaNV      VARCHAR(20)   NULL REFERENCES NhanVien(MaNV),
    MaKho     VARCHAR(20)   NULL REFERENCES Kho(MaKho),
    MaSP      VARCHAR(20)   NULL REFERENCES SanPham(MaSP),   -- << mỗi dòng = 1 sản phẩm
    SoLuong   INT           NOT NULL DEFAULT 0,
    GiaXuat   DECIMAL(18,2) NOT NULL DEFAULT 0,
    TrangThai NVARCHAR(50)  NULL
);
GO

-- ChiTietXuatKho giữ nguyên (dùng cho tính năng mở rộng sau này)
IF OBJECT_ID(N'ChiTietXuatKho', 'U') IS NULL
CREATE TABLE ChiTietXuatKho (
    MaPXK   VARCHAR(20)   NOT NULL REFERENCES PhieuXuatKho(MaPXK),
    MaSP    VARCHAR(20)   NOT NULL REFERENCES SanPham(MaSP),
    SoLuong INT           NOT NULL,
    GiaXuat DECIMAL(18,2) NOT NULL,
    PRIMARY KEY (MaPXK, MaSP)
);
GO

-- ============================================================
-- 7. Báo Cáo Sự Cố & Xử Lý Sự Cố
-- ============================================================
IF OBJECT_ID(N'BaoCaoSuCo', 'U') IS NULL
CREATE TABLE BaoCaoSuCo (
    MaBCSC   VARCHAR(20)   NOT NULL PRIMARY KEY,
    MaKH     VARCHAR(20)   NULL REFERENCES KhachHang(MaKH),
    MaHD     VARCHAR(20)   NULL REFERENCES HoaDon(MaHD),
    NgayGui  DATETIME      NOT NULL DEFAULT GETDATE(),
    NoiDung  NVARCHAR(MAX) NULL,
    TrangThai NVARCHAR(50) NULL  -- N'Cho xu ly', N'Da xac nhan', N'Da huy'
);
GO

IF OBJECT_ID(N'XuLySuCo', 'U') IS NULL
CREATE TABLE XuLySuCo (
    MaBCSC  VARCHAR(20)   NOT NULL PRIMARY KEY REFERENCES BaoCaoSuCo(MaBCSC),
    NgayXuLy DATETIME     NOT NULL DEFAULT GETDATE(),
    KetQua  NVARCHAR(MAX) NOT NULL,
    MaNV    VARCHAR(20)   NULL REFERENCES NhanVien(MaNV)
);
GO

-- ============================================================
-- 8. Dữ liệu mẫu (INSERT)
--    Dùng N'' prefix cho tất cả chuỗi tiếng Việt
-- ============================================================

-- Danh Mục
IF NOT EXISTS (SELECT 1 FROM DanhMuc WHERE MaDM = 'DM001')
INSERT INTO DanhMuc (MaDM, TenDM, MoTa) VALUES
('DM001', N'Thuốc kháng sinh',     N'Nhóm thuốc kháng sinh điều trị nhiễm khuẩn'),
('DM002', N'Thuốc hạ sốt giảm đau', N'Paracetamol, Ibuprofen và các thuốc tương tự'),
('DM003', N'Vitamin & Thực phẩm bổ sung', N'Các loại vitamin, khoáng chất và TPCN'),
('DM004', N'Thuốc tiêu hóa',       N'Thuốc trị tiêu chảy, táo bón, đầy hơi'),
('DM005', N'Thuốc tim mạch',       N'Thuốc huyết áp, tim mạch'),
('DM006', N'Thuốc nhỏ mắt - tai', N'Dung dịch nhỏ mắt, nhỏ tai');
GO

-- Nhân Viên (mật khẩu hash ví dụ – thực tế dùng bcrypt/SHA256)
IF NOT EXISTS (SELECT 1 FROM NhanVien WHERE MaNV = 'NV001')
INSERT INTO NhanVien (MaNV, Ho, Ten, SoDT, Email, DiaChi, Luong, ChucVu, TenDangNhap, MatKhau) VALUES
('NV001', N'Nguyễn', N'Admin',    '0901000001', 'admin@nhathuoc.vn',   N'TP. Hồ Chí Minh', 15000000, N'Admin',    'admin',    'admin123'),
('NV002', N'Trần',   N'Hoa',      '0901000002', 'hoa@nhathuoc.vn',     N'TP. Hồ Chí Minh',  9000000, N'Ban Hang', 'banhang',  'banhang123'),
('NV003', N'Lê',     N'Kho',      '0901000003', 'kho@nhathuoc.vn',     N'TP. Hồ Chí Minh',  8000000, N'Kho',      'kho',      'kho123'),
('NV004', N'Phạm',   N'Kế Toán',  '0901000004', 'ketoan@nhathuoc.vn',  N'TP. Hồ Chí Minh', 10000000, N'Ke Toan',  'ketoan',   'ketoan123');
GO

-- Khách Hàng
IF NOT EXISTS (SELECT 1 FROM KhachHang WHERE MaKH = 'KH001')
INSERT INTO KhachHang (MaKH, Ho, Ten, SoDT, Email, DiaChi, DiemTichLuy, HangThanhVien) VALUES
('KH001', N'Nguyễn', N'Văn A',   '0912345671', 'vana@email.com',   N'123 Nguyễn Trãi, Q.1, TP.HCM', 150, N'Bạc'),
('KH002', N'Trần',   N'Thị B',   '0912345672', 'thib@email.com',   N'456 Lê Lợi, Q.3, TP.HCM',      320, N'Vàng'),
('KH003', N'Lê',     N'Văn C',   '0912345673', 'vanc@email.com',   N'789 CMT8, Q.10, TP.HCM',          0, N'Bạc'),
('KH004', N'Phạm',   N'Thị D',   '0912345674', 'thid@email.com',   N'321 Đinh Tiên Hoàng, Bình Thạnh', 500, N'Kim cương'),
('KH005', N'Hoàng',  N'Văn E',   '0912345675', 'vane@email.com',   N'654 Phan Xích Long, Phú Nhuận',    80, N'Bạc');
GO

-- Kho
IF NOT EXISTS (SELECT 1 FROM Kho WHERE MaKho = 'KHO001')
INSERT INTO Kho (MaKho, TenKho, DiaChi, TrangThai) VALUES
('KHO001', N'Kho Trung Tâm',  N'123 Nguyễn Trãi, Q.1, TP.HCM',      N'Hoat dong'),
('KHO002', N'Kho Chi Nhánh 1', N'456 Lê Lợi, Q.3, TP.HCM',           N'Hoat dong'),
('KHO003', N'Kho Dự Phòng',   N'789 CMT8, Q.10, TP.HCM',             N'Ngung hoat dong');
GO

-- Nhà Cung Cấp
IF NOT EXISTS (SELECT 1 FROM NhaCungCap WHERE MaNCC = 'NCC001')
INSERT INTO NhaCungCap (MaNCC, TenNCC, SoDT, Email, DiaChi) VALUES
('NCC001', N'Công ty Dược Hậu Giang',   '02923891400', 'info@dhg.com.vn',      N'288 Bis Nguyễn Văn Cừ, Cần Thơ'),
('NCC002', N'Công ty Imexpharm',         '02963825234', 'info@imexpharm.com',   N'04 Đường 30/4, Sa Đéc, Đồng Tháp'),
('NCC003', N'Công ty Domesco',           '02773836166', 'info@domesco.com',     N'66 Quốc lộ 30, Sa Đéc, Đồng Tháp'),
('NCC004', N'Công ty OPC Pharma',        '02839354741', 'info@opcpharma.com',   N'1000 Phạm Hùng, Bình Chánh, TP.HCM');
GO

-- Sản Phẩm
IF NOT EXISTS (SELECT 1 FROM SanPham WHERE MaSP = 'SP001')
INSERT INTO SanPham (MaSP, TenSP, MaDM, DonViTinh, ThanhPhan, CongDung, GiaBan, SoLuongTonKho, MoTa) VALUES
('SP001', N'Paracetamol 500mg',    'DM002', N'Viên',  N'Paracetamol 500mg',      N'Hạ sốt, giảm đau nhẹ đến vừa',                    3500,   200, N'Thuốc hạ sốt phổ biến'),
('SP002', N'Amoxicillin 500mg',    'DM001', N'Viên',  N'Amoxicillin 500mg',      N'Điều trị nhiễm khuẩn đường hô hấp, tiết niệu',    8500,   150, N'Kháng sinh nhóm Penicillin'),
('SP003', N'Vitamin C 1000mg',     'DM003', N'Viên',  N'Acid Ascorbic 1000mg',   N'Bổ sung vitamin C, tăng đề kháng',                 5000,   300, N'Vitamin tan trong nước'),
('SP004', N'Oresol gói',           'DM004', N'Gói',   N'NaCl, KCl, Glucose',     N'Bù nước điện giải khi tiêu chảy, mất nước',        2000,   500, N'Dung dịch bù điện giải'),
('SP005', N'Omeprazole 20mg',      'DM004', N'Viên',  N'Omeprazole 20mg',        N'Điều trị loét dạ dày, trào ngược dạ dày thực quản', 6000,  120, N'Ức chế bơm proton'),
('SP006', N'Amlodipine 5mg',       'DM005', N'Viên',  N'Amlodipine besylate 5mg',N'Điều trị tăng huyết áp, đau thắt ngực',           12000,   80, N'Thuốc chẹn kênh canxi'),
('SP007', N'Nhỏ mắt Tobramycin',   'DM006', N'Lọ',   N'Tobramycin 0.3%',        N'Điều trị nhiễm khuẩn mắt',                        35000,   60, N'Kháng sinh nhỏ mắt'),
('SP008', N'Ibuprofen 400mg',      'DM002', N'Viên',  N'Ibuprofen 400mg',        N'Hạ sốt, giảm đau, kháng viêm',                    5000,   180, N'Thuốc kháng viêm không steroid'),
('SP009', N'Vitamin B Complex',    'DM003', N'Viên',  N'B1, B2, B6, B12',        N'Bổ sung vitamin nhóm B, hỗ trợ thần kinh',         8000,   250, N'Tổng hợp vitamin B'),
('SP010', N'Cetirizine 10mg',      'DM002', N'Viên',  N'Cetirizine HCl 10mg',    N'Điều trị dị ứng, viêm mũi dị ứng, mề đay',        4500,   100, N'Thuốc kháng histamin');
GO

-- ============================================================
-- 9. Kiểm tra toàn vẹn dữ liệu (tuỳ chọn)
-- ============================================================
SELECT 'KhachHang'   AS Bang, COUNT(*) AS SoBanGhi FROM KhachHang   UNION ALL
SELECT 'NhanVien',              COUNT(*)             FROM NhanVien    UNION ALL
SELECT 'DanhMuc',               COUNT(*)             FROM DanhMuc     UNION ALL
SELECT 'SanPham',               COUNT(*)             FROM SanPham     UNION ALL
SELECT 'Kho',                   COUNT(*)             FROM Kho         UNION ALL
SELECT 'NhaCungCap',            COUNT(*)             FROM NhaCungCap;
GO
