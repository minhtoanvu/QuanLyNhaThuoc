using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using QLNhaThuocApp.Models;
using QLNhaThuoc.BUS;

namespace QLNhaThuocApp.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly HomeBUS _bus;

    public HomeController(ILogger<HomeController> logger, IConfiguration config)
    {
        _logger = logger;
        _bus = new HomeBUS(config.GetConnectionString("DefaultConnection"));
    }

    public IActionResult Index()
    {
        try
        {
            return View(_bus.GetDashboard());
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = ex.Message;
            return View(new QLNhaThuoc.DAL.HomeDAL.DashboardData());
        }
    }

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
