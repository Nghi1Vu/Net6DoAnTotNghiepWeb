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
            ViewBag.URLWeb = _configuration["URLWeb"];
            return View();
        }
        [HttpPost]
        public IActionResult Index(InfoLogin model)
        {
            try
            {
                using HttpResponseMessage response = sharedClient.PostAsJsonAsync("/api/v1/GetStudentInfo", model).Result;
                var todo2 = response.Content.ReadAsStringAsync().Result;
                KeyResponse studentInfo = JsonConvert.DeserializeObject<KeyResponse>(todo2);
                HttpContext.Session.SetObjectAsJson("StudentInfo", studentInfo.rsInfo);
                HttpContext.Session.SetString("Key", studentInfo.Key);
                if (studentInfo.rsInfo.UserId != 0)
                {
                    return Content("/Home/News");
                }
                else
                {
                    return Content(todo2);
                }

            }
            catch (Exception ex)
            {
                return Content(ex.InnerException.Message);
            }
        }
    }
}
