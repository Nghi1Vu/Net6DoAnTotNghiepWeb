using DoAnTotNghiep.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
            try
            {
                using HttpResponseMessage response = sharedClient.PostAsJsonAsync("/api/v1/GetStudentInfo", model).Result;
                var todo2 = response.Content.ReadAsStringAsync().Result;
                StudentInfo studentInfo = JsonConvert.DeserializeObject<StudentInfo>(todo2);
                HttpContext.Session.SetObjectAsJson("StudentInfo", studentInfo);
                if (studentInfo.UserId != 0)
                {
                    return RedirectToAction("News", "Home");
                }
                else
                {
                    return RedirectToAction("Index", "Account");
                }

            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Account");
            }
        }
    }
}
