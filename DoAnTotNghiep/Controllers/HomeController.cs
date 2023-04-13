using DoAnTotNghiep.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NuGet.Common;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;

namespace DoAnTotNghiep.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient sharedClient;
        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
            sharedClient = new()
            {
                BaseAddress = new Uri(_configuration["UrlApi"]),
            };
        }
        public IActionResult Index(string state, string code, string scope)
        {
            HttpClient httpClient = new()
            {
                BaseAddress = new Uri("https://accounts.google.com"),
            };
            HttpClient httpClient1 = new()
            {
                BaseAddress = new Uri("https://www.googleapis.com"),
            };
            try
            {
                var creds = $"code={code}&client_id=340866818101-3vn7u84kjevf296o5s2r7uofdqrm8k9r.apps.googleusercontent.com&client_secret=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&redirect_uri=https://localhost:7044/Home/Index&grant_type=authorization_code";
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                var content = new StringContent(creds, Encoding.UTF8, "application/x-www-form-urlencoded");
                using var response = httpClient.PostAsync("/o/oauth2/token", content).Result;
                //using HttpResponseMessage response = httpClient.po("/o/oauth2/token?code="+ code + "&client_id=340866818101-3vn7u84kjevf296o5s2r7uofdqrm8k9r.apps.googleusercontent.com&client_secret=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&redirect_uri=" + "https://localhost:7044/Home/Index" + "&grant_type=authorization_code", "").Result;
                var todo = response.Content.ReadAsStringAsync().Result;
                AccessTokenModel token = JsonConvert.DeserializeObject<AccessTokenModel>(todo);
                using HttpResponseMessage response1 = httpClient1.GetAsync("/oauth2/v3/tokeninfo?access_token=" + token.access_token).Result;
                var todo1 = response1.Content.ReadAsStringAsync().Result;
                TokenModel token1 = JsonConvert.DeserializeObject<TokenModel>(todo1);
                using HttpResponseMessage response2 = sharedClient.PostAsJsonAsync("/api/v1/LoginWithEmail", token1).Result;
                var todo2 = response2.Content.ReadAsStringAsync().Result;
                if (todo2.IndexOf("\"expiresIn\":1") > 0)
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
                throw ex;
            }
        }

        public IActionResult News()
        {
            try
            {
                using HttpResponseMessage response = sharedClient.GetAsync("api/v1/GetNews").Result;
                var todo = response.Content.ReadAsStringAsync().Result;
                response.EnsureSuccessStatusCode();
                var news = JsonConvert.DeserializeObject<List<News>>(todo);
                sharedClient.Dispose();
                return View(news);
            }
            catch
            {        
                return RedirectToAction("Index", "Account");
            }        
  
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