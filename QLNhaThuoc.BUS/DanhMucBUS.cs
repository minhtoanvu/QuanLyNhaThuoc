using QLNhaThuoc.DTO;
using QLNhaThuoc.DAL;

namespace QLNhaThuoc.BUS
{
    public class DanhMucBUS
    {
        private readonly DanhMucDAL _dal;

        public DanhMucBUS(string connectionString)
        {
            _dal = new DanhMucDAL(connectionString);
        }

        public List<DanhMuc> GetAll() => _dal.GetAll();

        public DanhMuc GetById(string maDM) => _dal.GetById(maDM);

        public bool Insert(DanhMuc dm)
        {
            if (string.IsNullOrEmpty(dm.MaDM))
                dm.MaDM = "DM" + DateTime.Now.ToString("yyMMddHHmmss");
            return _dal.Insert(dm);
        }

        public bool Update(DanhMuc dm) => _dal.Update(dm);

        public bool Delete(string maDM) => _dal.Delete(maDM);
    }
}
