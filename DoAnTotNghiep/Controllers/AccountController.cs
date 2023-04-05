using DoAnTotNghiep.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

namespace DoAnTotNghiep.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient sharedClient;
        public AccountController(IConfiguration configuration)
        {
            _configuration = configuration;
            sharedClient = new()
            {
                BaseAddress = new Uri(_configuration["UrlApi"]),
            };
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(InfoLogin model)
        {
            using HttpResponseMessage response = sharedClient.PostAsJsonAsync("/api/v1/signin", model).Result;
            var todo = response.Content.ReadAsStringAsync().Result;
            if(todo.IndexOf("\"expiresIn\":1") > 0){
                return RedirectToAction("News","Home");
            }
            else
            {
                return View();
            }
        }
    }
}
