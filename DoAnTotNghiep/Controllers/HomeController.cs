using DoAnTotNghiep.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace DoAnTotNghiep.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult News()
        {
            return View();
        }

        public IActionResult ChiaSeBieuMau()
        {
            return View();
        }

        public IActionResult KetQuaHocTap()
        {
            return View();
        }

        public IActionResult ThoiKhoaBieu()
        {
            return View();
        }

        public IActionResult XemLichThi()
        {
            return View();
        }

        public IActionResult CaNhan()
        {
            return View();
        }

        public IActionResult DangKyHP()
        {
            return View();
        }
        
        public IActionResult DanhGiaRenLuyen()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}