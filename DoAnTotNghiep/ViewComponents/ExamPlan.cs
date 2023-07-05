using DoAnTotNghiep.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace DoAnTotNghiep.ViewComponents
{
    [ViewComponent(Name = "GetExamPlan")]
    public class ExamPlan : ViewComponent
    {
        private readonly HttpClient sharedClient;
        private readonly IConfiguration _configuration;

        public ExamPlan(IConfiguration configuration)
        {
            _configuration= configuration;
            sharedClient = new()
            {
                BaseAddress = new Uri(_configuration["UrlApi"]),
            };
        }
        public async Task<IViewComponentResult> InvokeAsync(string ClassCode)
        {
            string key = HttpContext.Session.GetString("Key");
            sharedClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", key);
            var user = HttpContext.Session.GetObjectFromJson<StudentInfo>("StudentInfo");
            using HttpResponseMessage response = sharedClient.GetAsync("api/v1/GetExamCalendar?UserID=" + user.UserId).Result;
            var todo = response.Content.ReadAsStringAsync().Result;
            response.EnsureSuccessStatusCode();
            var studenClasses = JsonConvert.DeserializeObject<List<ExamCalendar>>(todo).Where(x=>x.ClassCode==ClassCode).ToList();
            ViewBag.StudentInfo = user;
            sharedClient.Dispose();
            return View(studenClasses);
        }
    }
}
