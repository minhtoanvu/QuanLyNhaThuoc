using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLNhaThuoc.BUS;

namespace QLNhaThuocApp.Controllers
{
    [Authorize]
    public class TroLyController : Controller
    {
        private readonly TroLyBUS _bus;

        public TroLyController(IConfiguration config)
        {
            _bus = new TroLyBUS(
                config.GetConnectionString("DefaultConnection"),
                config.GetValue<string>("GeminiApiKey")
            );
        }

        public class TroLyRequest
        {
            public string CauHoi { get; set; }
        }

        [HttpPost]
        public JsonResult HoiDap([FromBody] TroLyRequest request)
        {
            var response = _bus.XuLyCauHoi(request?.CauHoi);
            return Json(response);
        }
    }
}
