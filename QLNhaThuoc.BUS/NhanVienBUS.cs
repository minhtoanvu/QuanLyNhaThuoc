using QLNhaThuoc.DTO;
using QLNhaThuoc.DAL;

namespace QLNhaThuoc.BUS
{
    public class NhanVienBUS
    {
        private readonly NhanVienDAL _dal;

        public NhanVienBUS(string connectionString)
        {
            _dal = new NhanVienDAL(connectionString);
        }

        public List<NhanVien> GetAll() => _dal.GetAll();

        public NhanVien GetById(string maNV) => _dal.GetById(maNV);

        /// <summary>
        /// Dùng cho AccountController — đăng nhập
        /// </summary>
        public NhanVien DangNhap(string tenDangNhap, string matKhau)
            => _dal.DangNhap(tenDangNhap, matKhau);

        public bool Insert(NhanVien nv, out string error)
        {
            error = null;
            if (string.IsNullOrEmpty(nv.MaNV))
                nv.MaNV = "NV" + DateTime.Now.ToString("yyMMddHHmmss");

            // Kiểm tra tên đăng nhập đã tồn tại
            if (_dal.KiemTraTenDangNhap(nv.TenDangNhap))
            {
                error = "Tên đăng nhập đã tồn tại.";
                return false;
            }
            return _dal.Insert(nv);
        }

        public bool Update(NhanVien nv, string matKhauMoi = null)
            => _dal.Update(nv, matKhauMoi);

        public bool Delete(string maNV) => _dal.Delete(maNV);
    }
}
