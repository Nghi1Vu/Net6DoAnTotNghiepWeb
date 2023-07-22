using DoAnTotNghiep.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            string key = HttpContext.Session.GetString("Key");
            sharedClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", key);
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
                using HttpResponseMessage response2 = sharedClient.PostAsJsonAsync("/api/v1/GetStudentInfoByEmail", token1).Result;
                var todo2 = response2.Content.ReadAsStringAsync().Result;
                KeyResponse studentInfo = JsonConvert.DeserializeObject<KeyResponse>(todo2);
                HttpContext.Session.SetObjectAsJson("StudentInfo", studentInfo.rsInfo);
                HttpContext.Session.SetString("Key", studentInfo.Key);
                if (studentInfo.rsInfo.UserId != 0)
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
        public IActionResult Logout()
        {
            HttpContext.Session.SetObjectAsJson("StudentInfo",null);
            HttpContext.Session.SetString("Key", "");
            return RedirectToAction("Index", "Account");
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
        public IActionResult TrangCaNhan()
        {
            var userid = HttpContext.Session.GetObjectFromJson<StudentInfo>("StudentInfo");
            return View(userid);
        }

        public IActionResult ThongTinGiaDinh()
        {
            try
            {
                var user = HttpContext.Session.GetObjectFromJson<StudentInfo>("StudentInfo");
                using HttpResponseMessage response = sharedClient.GetAsync("api/v1/GetFamilyDetail?UserId=" + user.UserId).Result;
                var todo = response.Content.ReadAsStringAsync().Result;
                response.EnsureSuccessStatusCode();
                var result = JsonConvert.DeserializeObject<FamilyDetail>(todo);
                sharedClient.Dispose();
                return View(result);
            }
            catch
            {
                return RedirectToAction("Index", "Account");
            }
        }

        public IActionResult ThongBaoTraoDoiTrongLop()
        {
            try
            {
                var user = HttpContext.Session.GetObjectFromJson<StudentInfo>("StudentInfo");
                using HttpResponseMessage response = sharedClient.GetAsync("api/v1/GetMessage?ClassID=" + user.ClassID).Result;
                var todo = response.Content.ReadAsStringAsync().Result;
                response.EnsureSuccessStatusCode();
                var result = JsonConvert.DeserializeObject<List<Message>>(todo);
                sharedClient.Dispose();
                return View(result);
            }
            catch
            {
                return RedirectToAction("Index", "Account");
            }
        }
        public IActionResult PostMessage(string content)
        {
            try
            {
                var user = HttpContext.Session.GetObjectFromJson<StudentInfo>("StudentInfo");
                using HttpResponseMessage response = sharedClient.GetAsync("api/v1/PostMessage?UserId=" + user.UserId + "&content=" + content).Result;
                var todo = response.Content.ReadAsStringAsync().Result;
                response.EnsureSuccessStatusCode();
                var result = JsonConvert.DeserializeObject<string>(todo);
                sharedClient.Dispose();
                return RedirectToAction("ThongBaoTraoDoiTrongLop");
            }
            catch
            {
                return RedirectToAction("Index", "Account");
            }
        }
        public JsonResult HandleDKHP(int id, int mdid)
        {
            var user = HttpContext.Session.GetObjectFromJson<StudentInfo>("StudentInfo");
            var obj = new
            {
                id = id,
                UserID = user.UserId,
                amount = user.Amount,
                mdid = mdid,
                CourseID = user.CourseID,
                CourseIndustryID = user.CourseIndustryID
            };
            using HttpResponseMessage response = sharedClient.PostAsJsonAsync("api/v1/HandleDKHP", obj).Result;
            var todo = response.Content.ReadAsStringAsync().Result;
            response.EnsureSuccessStatusCode();
            var result = JsonConvert.DeserializeObject<string>(todo);
            sharedClient.Dispose();
            return Json(result);
        }
        public IActionResult DoiMatKhau()
        {
            return View();
        }
        [HttpPost]
        public JsonResult DoiMatKhau(ChangePass change)
        {
            try
            {
                if (change.newpass != change.validpass)
                {
                    return Json("K");
                }
                var user = HttpContext.Session.GetObjectFromJson<StudentInfo>("StudentInfo");
                var obj = new
                {
                    username = user.Usercode,
                    oldpass = change.oldpass,
                    newpass = change.newpass
                };
                using HttpResponseMessage response = sharedClient.PostAsJsonAsync("api/v1/ChangePassword", obj).Result;
                var todo = response.Content.ReadAsStringAsync().Result;
                response.EnsureSuccessStatusCode();
                var result = JsonConvert.DeserializeObject<int>(todo);
                sharedClient.Dispose();
                if (result == 1)
                {
                    return Json("Y");
                }
                return Json("N");
            }
            catch
            {
                return Json("N");
            }
        }

        public IActionResult DSHoSo()
        {
            try
            {
                var user = HttpContext.Session.GetObjectFromJson<StudentInfo>("StudentInfo");
                using HttpResponseMessage response = sharedClient.GetAsync("api/v1/GetDsGtHs?UserID=" + user.UserId).Result;
                var todo = response.Content.ReadAsStringAsync().Result;
                response.EnsureSuccessStatusCode();
                var news = JsonConvert.DeserializeObject<List<DsGtHs>>(todo);
                sharedClient.Dispose();
                return View(news);
            }
            catch
            {
                return RedirectToAction("Index", "Account");
            }
        }

        public IActionResult DetailNews(int id)
        {
            try
            {
                using HttpResponseMessage response = sharedClient.GetAsync("api/v1/GetNewsDetail?NewsId=" + id).Result;
                var todo = response.Content.ReadAsStringAsync().Result;
                response.EnsureSuccessStatusCode();
                var news = JsonConvert.DeserializeObject<News>(todo);
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
            try
            {
                var user = HttpContext.Session.GetObjectFromJson<StudentInfo>("StudentInfo");
                using HttpResponseMessage response = sharedClient.GetAsync("api/v1/GetKQHTByUser?UserId=" + user.UserId).Result;
                var todo = response.Content.ReadAsStringAsync().Result;
                response.EnsureSuccessStatusCode();
                var result = JsonConvert.DeserializeObject<List<KQHT>>(todo);
                sharedClient.Dispose();
                ViewBag.StudentInfo = user;
                return View(result);
            }
            catch
            {
                return RedirectToAction("Index", "Account");
            }
        }

        public IActionResult KetQuaHocTapTrenLop(int IndependentClassID)
        {
            try
            {
                var user = HttpContext.Session.GetObjectFromJson<StudentInfo>("StudentInfo");
                using HttpResponseMessage response = sharedClient.GetAsync("api/v1/GetKQHTByClass?IndependentClassID=" + IndependentClassID).Result;
                var todo = response.Content.ReadAsStringAsync().Result;
                response.EnsureSuccessStatusCode();
                var result = JsonConvert.DeserializeObject<List<KQHT>>(todo);
                sharedClient.Dispose();
                ViewBag.StudentInfo = user;
                return View(result);
            }
            catch
            {
                return RedirectToAction("Index", "Account");
            }
        }

        public IActionResult XemKetQuaHocTapCacMon(int UserID)
        {
            try
            {
                using HttpResponseMessage response = sharedClient.GetAsync("api/v1/GetKQHTByUser?UserId=" + UserID).Result;
                var todo = response.Content.ReadAsStringAsync().Result;
                response.EnsureSuccessStatusCode();
                var result = JsonConvert.DeserializeObject<List<KQHT>>(todo);
                sharedClient.Dispose();
                return View(result);
            }
            catch
            {
                return RedirectToAction("Index", "Account");
            }
        }

        public IActionResult KetQuaThi()
        {
            try
            {
                var user = HttpContext.Session.GetObjectFromJson<StudentInfo>("StudentInfo");
                using HttpResponseMessage response = sharedClient.GetAsync("api/v1/GetExamResult?UserID=" + user.UserId).Result;
                var todo = response.Content.ReadAsStringAsync().Result;
                response.EnsureSuccessStatusCode();
                var studenClasses = JsonConvert.DeserializeObject<List<ExamResult>>(todo);
                ViewBag.StudentInfo = user;
                sharedClient.Dispose();
                return View(studenClasses);
            }
            catch
            {
                return RedirectToAction("Index", "Account");
            }
        }

        public IActionResult KetQuaThiTrenLop(int IndependentClassID)
        {
            try
            {
                var user = HttpContext.Session.GetObjectFromJson<StudentInfo>("StudentInfo");
                using HttpResponseMessage response = sharedClient.GetAsync("api/v1/GetExamByClass?IndependentClassID=" + IndependentClassID).Result;
                var todo = response.Content.ReadAsStringAsync().Result;
                response.EnsureSuccessStatusCode();
                var studenClasses = JsonConvert.DeserializeObject<List<ExamByClass>>(todo);
                ViewBag.StudentInfo = user;
                sharedClient.Dispose();
                return View(studenClasses);
            }
            catch
            {
                return RedirectToAction("Index", "Account");
            }
        }

        public IActionResult XemKetQuaThiCacMon(int UserId)
        {
            try
            {
                var user = HttpContext.Session.GetObjectFromJson<StudentInfo>("StudentInfo");
                using HttpResponseMessage response = sharedClient.GetAsync("api/v1/GetExamResult?UserID=" + UserId).Result;
                var todo = response.Content.ReadAsStringAsync().Result;
                response.EnsureSuccessStatusCode();
                var studenClasses = JsonConvert.DeserializeObject<List<ExamResult>>(todo);
                ViewBag.StudentInfo = user;
                sharedClient.Dispose();
                return View(studenClasses);
            }
            catch
            {
                return RedirectToAction("Index", "Account");
            }
        }


        public IActionResult TBChungTichLuy()
        {
            try
            {
                var user = HttpContext.Session.GetObjectFromJson<StudentInfo>("StudentInfo");
                using HttpResponseMessage response = sharedClient.GetAsync("api/v1/GetProgramSemester?CourseIndustryID=" + user.CourseIndustryID + "&CourseID=" + user.CourseID + "&UserID=" + user.UserId).Result;
                var todo = response.Content.ReadAsStringAsync().Result;
                response.EnsureSuccessStatusCode();
                var studenClasses = JsonConvert.DeserializeObject<List<ProgramSemester>>(todo);
                ViewBag.StudentInfo = user;
                sharedClient.Dispose();
                return View(studenClasses);
            }
            catch
            {
                return RedirectToAction("Index", "Account");
            }
        }

        public IActionResult TBChungHocKy()
        {
            try
            {
                var user = HttpContext.Session.GetObjectFromJson<StudentInfo>("StudentInfo");
                using HttpResponseMessage response = sharedClient.GetAsync("api/v1/GetTBCHK?UserID=" + user.UserId).Result;
                var todo = response.Content.ReadAsStringAsync().Result;
                response.EnsureSuccessStatusCode();
                var studenClasses = JsonConvert.DeserializeObject<List<TBCHKModel>>(todo);
                ViewBag.StudentInfo = user;
                sharedClient.Dispose();
                return View(studenClasses);
            }
            catch
            {
                return RedirectToAction("Index", "Account");
            }
        }

        public IActionResult XetThuTotNghiep()
        {
            try
            {
                var user = HttpContext.Session.GetObjectFromJson<StudentInfo>("StudentInfo");
                using HttpResponseMessage response = sharedClient.GetAsync("api/v1/GetCertificateByUser?UserID=" + user.UserId + "&CourseIndustryID=" + user.CourseIndustryID).Result;
                var todo = response.Content.ReadAsStringAsync().Result;
                response.EnsureSuccessStatusCode();
                var studenClasses = JsonConvert.DeserializeObject<List<Certificate>>(todo);
                using HttpResponseMessage response2 = sharedClient.GetAsync("api/v1/GetExamResult?UserID=" + user.UserId).Result;
                var todo2 = response2.Content.ReadAsStringAsync().Result;
                response2.EnsureSuccessStatusCode();
                var F = JsonConvert.DeserializeObject<List<ExamResult>>(todo2).Where(x => x.XH == "F").Count();
                using HttpResponseMessage response3 = sharedClient.GetAsync("api/v1/GetTTCN?UserID=" + user.UserId).Result;
                var todo3 = response3.Content.ReadAsStringAsync().Result;
                response3.EnsureSuccessStatusCode();
                var ttcn = JsonConvert.DeserializeObject<List<TTCN>>(todo3).Sum(x => x.Costs);
                ViewBag.StudentInfo = user;
                ViewBag.F = F;
                ViewBag.ttcn = ttcn;
                sharedClient.Dispose();
                return View(studenClasses);
            }
            catch
            {
                return RedirectToAction("Index", "Account");
            }
        }

        public IActionResult ThoiKhoaBieu()
        {
            ViewBag.aDate = DateTime.Now.AddDays(-7).ToString("dd/MM/yyyy");
            ViewBag.eDate = DateTime.Now.AddDays(8).ToString("dd/MM/yyyy");
            return View();
        }
        public IActionResult GetTKB(string aDate, string eDate)
        {
            try
            {
                var user = HttpContext.Session.GetObjectFromJson<StudentInfo>("StudentInfo");
                using HttpResponseMessage response = sharedClient.GetAsync("api/v1/GetTKB?UserID=" + user.UserId + "&aDate=" + DateTime.Parse(aDate).ToString("yyyy-MM-dd") + "&eDate=" + DateTime.Parse(eDate).ToString("yyyy-MM-dd")).Result;
                var todo = response.Content.ReadAsStringAsync().Result;
                response.EnsureSuccessStatusCode();
                var studenClasses = JsonConvert.DeserializeObject<List<TKB>>(todo);
                ViewBag.StudentInfo = user;
                sharedClient.Dispose();
                ViewBag.aDate = DateTime.Parse(aDate);
                ViewBag.eDate = DateTime.Parse(eDate);
                return View(studenClasses);
            }
            catch
            {
                return RedirectToAction("Index", "Account");
            }
        }
        public IActionResult LichGiangDay()
        {
            try
            {
                var user = HttpContext.Session.GetObjectFromJson<StudentInfo>("StudentInfo");
                using HttpResponseMessage response = sharedClient.GetAsync("api/v1/GetTeachCalendar?UserID=" + user.UserId).Result;
                var todo = response.Content.ReadAsStringAsync().Result;
                response.EnsureSuccessStatusCode();
                var studenClasses = JsonConvert.DeserializeObject<List<TeachCalendar>>(todo);
                ViewBag.StudentInfo = user;
                sharedClient.Dispose();
                return View(studenClasses);
            }
            catch
            {
                return RedirectToAction("Index", "Account");
            }
        }

        public IActionResult XemLichGiangDay(int IndependentClassID)
        {
            try
            {
                var user = HttpContext.Session.GetObjectFromJson<StudentInfo>("StudentInfo");
                using HttpResponseMessage response = sharedClient.GetAsync("api/v1/GetTeachCalendarDetail?IndependentClassID=" + IndependentClassID + "&UserID=" + user.UserId).Result;
                var todo = response.Content.ReadAsStringAsync().Result;
                response.EnsureSuccessStatusCode();
                var studenClasses = JsonConvert.DeserializeObject<List<TeachCalendarDetail>>(todo);
                ViewBag.StudentInfo = user;
                sharedClient.Dispose();
                return View(studenClasses);
            }
            catch
            {
                return RedirectToAction("Index", "Account");
            }
        }

        public IActionResult XemLichThi()
        {
            try
            {
                var user = HttpContext.Session.GetObjectFromJson<StudentInfo>("StudentInfo");
                using HttpResponseMessage response = sharedClient.GetAsync("api/v1/GetExamCalendar?UserID=" + user.UserId).Result;
                var todo = response.Content.ReadAsStringAsync().Result;
                response.EnsureSuccessStatusCode();
                var studenClasses = JsonConvert.DeserializeObject<List<ExamCalendar>>(todo);
                ViewBag.StudentInfo = user;
                sharedClient.Dispose();
                return View(studenClasses);
            }
            catch
            {
                return RedirectToAction("Index", "Account");
            }
        }

        public IActionResult KeHoachThi()
        {
            try
            {
                var user = HttpContext.Session.GetObjectFromJson<StudentInfo>("StudentInfo");
                using HttpResponseMessage response = sharedClient.GetAsync("api/v1/GetExamCalendar?UserID=" + user.UserId).Result;
                var todo = response.Content.ReadAsStringAsync().Result;
                response.EnsureSuccessStatusCode();
                var studenClasses = JsonConvert.DeserializeObject<List<ExamCalendar>>(todo);
                ViewBag.StudentInfo = user;
                sharedClient.Dispose();
                return View(studenClasses);
            }
            catch
            {
                return RedirectToAction("Index", "Account");
            }
        }
        public IActionResult KeHoachThiChiTiet(string ClassCode)
        {

            return ViewComponent("GetExamPlan", ClassCode);

        }
        public IActionResult Taikhoan(string aDate, string eDate)
        {
            try
            {
                if (aDate == null && eDate == null)
                {
                    aDate = DateTime.Now.AddYears(-1).ToString("dd/MM/yyyy");
                    eDate = DateTime.Now.AddDays(0).ToString("dd/MM/yyyy");
                }
                var user = HttpContext.Session.GetObjectFromJson<StudentInfo>("StudentInfo");
                using HttpResponseMessage response = sharedClient.GetAsync("api/v1/GetTradeHistory?UserId=" + user.UserId).Result;
                var todo = response.Content.ReadAsStringAsync().Result;
                response.EnsureSuccessStatusCode();
                var result = JsonConvert.DeserializeObject<List<TradeHistory>>(todo);
                sharedClient.Dispose();
                ViewBag.StudentInfo = user;
                var fiter = result.Where(x => x.CreatedTime.Date >= DateTime.Parse(aDate).Date && x.CreatedTime.Date <= DateTime.Parse(eDate).Date).ToList();
                return View(fiter);
            }
            catch
            {
                return RedirectToAction("Index", "Account");
            }
        }

        public IActionResult ThanhToanCongNo()
        {
            try
            {
                var user = HttpContext.Session.GetObjectFromJson<StudentInfo>("StudentInfo");
                using HttpResponseMessage response = sharedClient.GetAsync("api/v1/GetTTCN?UserID=" + user.UserId).Result;
                var todo = response.Content.ReadAsStringAsync().Result;
                response.EnsureSuccessStatusCode();
                var studenClasses = JsonConvert.DeserializeObject<List<TTCN>>(todo);
                using HttpResponseMessage response2 = sharedClient.GetAsync("api/v1/GetTTCNDone?UserID=" + user.UserId).Result;
                var todo2 = response2.Content.ReadAsStringAsync().Result;
                response2.EnsureSuccessStatusCode();
                var studenClasses2 = JsonConvert.DeserializeObject<List<TTCN>>(todo2);
                ViewBag.StudentInfo = user;
                ViewBag.StudentAmount = studenClasses2;
                sharedClient.Dispose();
                return View(studenClasses);
            }
            catch
            {
                return RedirectToAction("Index", "Account");
            }
        }
        [HttpPost]
        public IActionResult PostTTCN(string id)
        {
            try
            {
                string ttcnid = id.Trim().Substring(0, id.Length - 1);
                var user = HttpContext.Session.GetObjectFromJson<StudentInfo>("StudentInfo");
                var obj = new
                {
                    UserId = user.UserId,
                    ttcnid = ttcnid,
                    amount = user.Amount
                };
                using HttpResponseMessage response = sharedClient.PostAsJsonAsync("api/v1/PostTTCN", obj).Result;
                var todo = response.Content.ReadAsStringAsync().Result;
                response.EnsureSuccessStatusCode();
                var result = JsonConvert.DeserializeObject<string>(todo);
                if (result == "Y")
                {
                    using HttpResponseMessage response2 = sharedClient.PostAsJsonAsync("/api/v1/GetStudentInfoByEmail", new { email = user.Email != null ? user.Email : user.Usercode }).Result;
                    var todo2 = response2.Content.ReadAsStringAsync().Result;
                    StudentInfo studentInfo = JsonConvert.DeserializeObject<StudentInfo>(todo2);
                    HttpContext.Session.SetObjectAsJson("StudentInfo", studentInfo);
                }
                sharedClient.Dispose();
                return Content(result);
            }
            catch
            {
                return RedirectToAction("Index", "Account");
            }
        }
        [HttpPost]
        public IActionResult PostOneDoor(string id)
        {
            try
            {
                string odid = id.Trim().Substring(0, id.Length - 1);
                var user = HttpContext.Session.GetObjectFromJson<StudentInfo>("StudentInfo");
                var obj = new
                {
                    UserId = user.UserId,
                    odid = odid,
                    amount = user.Amount
                };
                using HttpResponseMessage response = sharedClient.PostAsJsonAsync("api/v1/PostOneDoor", obj).Result;
                var todo = response.Content.ReadAsStringAsync().Result;
                response.EnsureSuccessStatusCode();
                var result = JsonConvert.DeserializeObject<string>(todo);
                if (result == "Y")
                {
                    using HttpResponseMessage response2 = sharedClient.PostAsJsonAsync("/api/v1/GetStudentInfoByEmail", new { email = user.Email != null ? user.Email : user.Usercode }).Result;
                    var todo2 = response2.Content.ReadAsStringAsync().Result;
                    StudentInfo studentInfo = JsonConvert.DeserializeObject<StudentInfo>(todo2);
                    HttpContext.Session.SetObjectAsJson("StudentInfo", studentInfo);
                }
                sharedClient.Dispose();
                return Content(result);
            }
            catch
            {
                return RedirectToAction("Index", "Account");
            }
        }
        public IActionResult DichVuMotCua()
        {
            try
            {
                var user = HttpContext.Session.GetObjectFromJson<StudentInfo>("StudentInfo");
                using HttpResponseMessage response = sharedClient.GetAsync("api/v1/GetChannelAmount?ClassID=" + user.ClassID).Result;
                var todo = response.Content.ReadAsStringAsync().Result;
                response.EnsureSuccessStatusCode();
                var studenClasses = JsonConvert.DeserializeObject<List<ChannelAmount>>(todo);
                using HttpResponseMessage response2 = sharedClient.GetAsync("api/v1/GetStudentAmount?UserID=" + user.UserId).Result;
                var todo2 = response2.Content.ReadAsStringAsync().Result;
                response2.EnsureSuccessStatusCode();
                var studenClasses2 = JsonConvert.DeserializeObject<List<StudentAmount>>(todo2);
                ViewBag.StudentInfo = user;
                ViewBag.StudentAmount = studenClasses2;
                sharedClient.Dispose();
                return View(studenClasses);
            }
            catch
            {
                return RedirectToAction("Index", "Account");
            }
        }

        public IActionResult LopHoc()
        {
            try
            {
                var user = HttpContext.Session.GetObjectFromJson<StudentInfo>("StudentInfo");
                using HttpResponseMessage response = sharedClient.GetAsync("api/v1/GetStudentClass?ClassID=" + user.ClassID).Result;
                var todo = response.Content.ReadAsStringAsync().Result;
                response.EnsureSuccessStatusCode();
                var studenClasses = JsonConvert.DeserializeObject<List<StudenClass>>(todo);
                sharedClient.Dispose();
                ViewBag.StudentInfo = user;
                return View(studenClasses);
            }
            catch
            {
                return RedirectToAction("Index", "Account");
            }
        }

        public IActionResult CaNhan()
        {
            try
            {
                var user = HttpContext.Session.GetObjectFromJson<StudentInfo>("StudentInfo");
                using HttpResponseMessage response = sharedClient.GetAsync("api/v1/GetStudentDetail?UserId=" + user.UserId).Result;
                var todo = response.Content.ReadAsStringAsync().Result;
                response.EnsureSuccessStatusCode();
                var result = JsonConvert.DeserializeObject<StudentDetail>(todo);
                sharedClient.Dispose();
                ViewBag.StudentInfo = user;
                return View(result);
            }
            catch
            {
                return RedirectToAction("Index", "Account");
            }
        }

        public IActionResult DangKyHP()
        {
            try
            {
                var user = HttpContext.Session.GetObjectFromJson<StudentInfo>("StudentInfo");
                using HttpResponseMessage response = sharedClient.GetAsync("api/v1/GetProgramSemester?CourseIndustryID=" + user.CourseIndustryID + "&CourseID=" + user.CourseID + "&UserID=" + user.UserId).Result;
                var todo = response.Content.ReadAsStringAsync().Result;
                response.EnsureSuccessStatusCode();
                var result = JsonConvert.DeserializeObject<List<ProgramSemester>>(todo);
                using HttpResponseMessage response2 = sharedClient.GetAsync("api/v1/GetDKHPByTKB?UserID=" + user.UserId).Result;
                var todo2 = response2.Content.ReadAsStringAsync().Result;
                response2.EnsureSuccessStatusCode();
                ViewBag.DKHPByTKB = JsonConvert.DeserializeObject<List<DKHPByTKB>>(todo2).ToList();//.Where(x => result != null && result.Where(y => y.ModulesID == x.ModulesId && y.kqht==1).Count() <= 0).ToList();
                ViewBag.StudentInfo = user;
                sharedClient.Dispose();
                return View(result);
            }
            catch
            {
                return RedirectToAction("Index", "Account");
            }
        }

        public IActionResult TinhHinhDangKyHP()
        {
            try
            {
                var user = HttpContext.Session.GetObjectFromJson<StudentInfo>("StudentInfo");
                using HttpResponseMessage response = sharedClient.GetAsync("api/v1/GetLogDKHP?UserID=" + user.UserId).Result;
                var todo = response.Content.ReadAsStringAsync().Result;
                response.EnsureSuccessStatusCode();
                var studenClasses = JsonConvert.DeserializeObject<List<LogDKHP>>(todo);
                ViewBag.StudentInfo = user;
                sharedClient.Dispose();
                return View(studenClasses);
            }
            catch
            {
                return RedirectToAction("Index", "Account");
            }
        }

        public IActionResult KhungChuongTrinh()
        {
            try
            {
                var user = HttpContext.Session.GetObjectFromJson<StudentInfo>("StudentInfo");
                using HttpResponseMessage response = sharedClient.GetAsync("api/v1/GetProgramSemester?CourseIndustryID=" + user.CourseIndustryID + "&CourseID=" + user.CourseID + "&UserID=" + user.UserId).Result;
                var todo = response.Content.ReadAsStringAsync().Result;
                response.EnsureSuccessStatusCode();
                var result = JsonConvert.DeserializeObject<List<ProgramSemester>>(todo);
                ViewBag.StudentInfo = user;
                sharedClient.Dispose();
                return View(result);
            }
            catch
            {
                return RedirectToAction("Index", "Account");
            }
        }
        //[ResponseCache(Duration =3600,Location =ResponseCacheLocation.Client)]
        public IActionResult KhungChuongTrinhKy()
        {
            try
            {
                var user = HttpContext.Session.GetObjectFromJson<StudentInfo>("StudentInfo");
                using HttpResponseMessage response = sharedClient.GetAsync("api/v1/GetProgramSemester?CourseIndustryID=" + user.CourseIndustryID + "&CourseID=" + user.CourseID + "&UserID=" + user.UserId).Result;
                var todo = response.Content.ReadAsStringAsync().Result;
                response.EnsureSuccessStatusCode();
                var result = JsonConvert.DeserializeObject<List<ProgramSemester>>(todo);
                ViewBag.StudentInfo = user;
                sharedClient.Dispose();
                return View(result);
            }
            catch
            {
                return RedirectToAction("Index", "Account");
            }
        }

        public IActionResult ChiTietChuongTrinh(int ModulesID)
        {
            try
            {
                var user = HttpContext.Session.GetObjectFromJson<StudentInfo>("StudentInfo");
                using HttpResponseMessage response = sharedClient.GetAsync("api/v1/GetModuleDetail?ModulesID=" + ModulesID).Result;
                var todo = response.Content.ReadAsStringAsync().Result;
                response.EnsureSuccessStatusCode();
                var result = JsonConvert.DeserializeObject<List<ModuleDetail>>(todo);
                ViewBag.StudentInfo = user;
                sharedClient.Dispose();
                return View(result);
            }
            catch
            {
                return RedirectToAction("Index", "Account");
            }
        }

        public IActionResult DanhGiaRenLuyen()
        {
            try
            {
                var user = HttpContext.Session.GetObjectFromJson<StudentInfo>("StudentInfo");
                using HttpResponseMessage response = sharedClient.GetAsync("api/v1/GetRLSemester?UserId=" + user.UserId).Result;
                var todo = response.Content.ReadAsStringAsync().Result;
                response.EnsureSuccessStatusCode();
                var result = JsonConvert.DeserializeObject<List<RLSemester>>(todo);
                sharedClient.Dispose();
                return View(result);
            }
            catch
            {
                return RedirectToAction("Index", "Account");
            }
        }

        public IActionResult BieuMauDanhGiaRenLuyen(int Id)
        {
            try
            {
                using HttpResponseMessage response = sharedClient.GetAsync("api/v1/GetRLForm").Result;
                var todo = response.Content.ReadAsStringAsync().Result;
                response.EnsureSuccessStatusCode();
                var result = JsonConvert.DeserializeObject<List<RLForm>>(todo);
                sharedClient.Dispose();
                ViewBag.SemesterID = Id;
                return View(result);
            }
            catch
            {
                return RedirectToAction("Index", "Account");
            }
        }
        [HttpPost]
        public IActionResult SendRLForm(PostRLForm model)
        {
            try
            {
                var user = HttpContext.Session.GetObjectFromJson<StudentInfo>("StudentInfo");
                var obj = new
                {
                    UserId = user.UserId,
                    SemesterID = model.SemesterID,
                    detail = model.detail
                };
                using HttpResponseMessage response = sharedClient.PostAsJsonAsync("api/v1/PostRLForm", obj).Result;
                var todo = response.Content.ReadAsStringAsync().Result;
                response.EnsureSuccessStatusCode();
                var result = JsonConvert.DeserializeObject<int>(todo);
                sharedClient.Dispose();
                return RedirectToAction("DanhGiaRenLuyen");
            }
            catch
            {
                return RedirectToAction("Index", "Account");
            }
        }
        [HttpPost]
        public JsonResult DeleteDKHP(int id)
        {
            var user = HttpContext.Session.GetObjectFromJson<StudentInfo>("StudentInfo");
            var obj = new
            {
                UserID = user.UserId,
                IndependentClassID = id,
            };
            using HttpResponseMessage response = sharedClient.PostAsJsonAsync("api/v1/DeleteDKHP", obj).Result;
            var todo = response.Content.ReadAsStringAsync().Result;
            response.EnsureSuccessStatusCode();
            var result = JsonConvert.DeserializeObject<string>(todo);
            sharedClient.Dispose();
            return Json(result);
        }
        public IActionResult GetIC(int ModulesID, int TimesInDay, int DayStudy)
        {
            return ViewComponent("GetIC", new { ModulesID, TimesInDay, DayStudy });
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