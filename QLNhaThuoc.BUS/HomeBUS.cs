using QLNhaThuoc.DAL;

namespace QLNhaThuoc.BUS
{
    public class HomeBUS
    {
        private readonly HomeDAL _dal;

        public HomeBUS(string connectionString)
        {
            _dal = new HomeDAL(connectionString);
        }

        public HomeDAL.DashboardData GetDashboard() => _dal.GetDashboard();
    }
}
