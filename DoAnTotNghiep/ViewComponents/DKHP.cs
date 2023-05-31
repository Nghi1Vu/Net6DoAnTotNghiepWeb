using DoAnTotNghiep.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DoAnTotNghiep.ViewComponents
{
    [ViewComponent(Name = "GetIC")]
    public class DKHP : ViewComponent
    {
        private readonly HttpClient sharedClient;
        private readonly IConfiguration _configuration;

        public DKHP(IConfiguration configuration)
        {
            _configuration= configuration;
            sharedClient = new()
            {
                BaseAddress = new Uri(_configuration["UrlApi"]),
            };
        }
        public async Task<IViewComponentResult> InvokeAsync(int ModulesID, int TimesInDay, int DayStudy)
        {
            var user = HttpContext.Session.GetObjectFromJson<StudentInfo>("StudentInfo");
            var result = new List<IndependentClass>();
            if (ModulesID != 0)
            {
                using HttpResponseMessage response = sharedClient.GetAsync("api/v1/GetIC?ModulesID=" + ModulesID).Result;
                var todo = response.Content.ReadAsStringAsync().Result;
                response.EnsureSuccessStatusCode();
                result = JsonConvert.DeserializeObject<List<IndependentClass>>(todo);
            }
            else
            {
                using HttpResponseMessage response = sharedClient.GetAsync("api/v1/GetICByTKB?TimesInDay=" + TimesInDay + "&DayStudy=" + DayStudy).Result;
                var todo = response.Content.ReadAsStringAsync().Result;
                response.EnsureSuccessStatusCode();
                result = JsonConvert.DeserializeObject<List<IndependentClass>>(todo);
            }
            ViewBag.StudentInfo = user;
            sharedClient.Dispose();
            return View(result);
        }
    }
    [ViewComponent(Name = "GetProfile")]
    public class GetProfile : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = HttpContext.Session.GetObjectFromJson<StudentInfo>("StudentInfo");
            return Content(user.Fullname);
        }
    }
}
