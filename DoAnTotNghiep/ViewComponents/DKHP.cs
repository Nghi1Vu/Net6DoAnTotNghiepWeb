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
        public async Task<IViewComponentResult> InvokeAsync(int ModulesID)
        {
            var user = HttpContext.Session.GetObjectFromJson<StudentInfo>("StudentInfo");
            using HttpResponseMessage response = sharedClient.GetAsync("api/v1/GetIC?ModulesID=" + ModulesID).Result;
            var todo = response.Content.ReadAsStringAsync().Result;
            response.EnsureSuccessStatusCode();
            var result = JsonConvert.DeserializeObject<List<IndependentClass>>(todo);
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
