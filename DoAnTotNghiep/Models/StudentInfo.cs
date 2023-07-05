namespace DoAnTotNghiep.Models
{
    public class KeyResponse
    {
        public StudentInfo rsInfo { get; set; }
        public string Key { get; set; }
    }
    public class StudentInfo
    {
        public int UserId { get; set; }
        public int CourseIndustryID { get; set; }
        public int CourseID { get; set; }
        public decimal Credits { get; set; }
        public string Fullname { get; set; }
        public string CourseName { get; set; }
        public string CourseCode { get; set; }
        public string TeacherName { get; set; }
        public string TranningLevelName { get; set; }
        public string IndustryName { get; set; }
        public string IndustryCode { get; set; }
        public string DepartmentName { get; set; }
        public string Usercode { get; set; }
        public string Classname { get; set; }
        public string XH { get; set; }
        public int ClassID { get; set; }
        public decimal TBCTL { get; set; }
        public decimal TotalCredits { get; set; }
        public decimal Amount { get; set; }
        public string Email { get; set; }
        public int? Phone { get; set; }
        public string Address { get; set; }
        public string Images { get; set; } = string.Empty;
    }
}
