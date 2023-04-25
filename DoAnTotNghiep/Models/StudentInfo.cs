namespace DoAnTotNghiep.Models
{
    public class StudentInfo
    {
        public int UserId { get; set; }
        public string Fullname { get; set; }
        public string TeacherName { get; set; }
        public string IndustryName { get; set; }
        public string DepartmentName { get; set; }
        public string Usercode { get; set; }
        public string Classname { get; set; }
        public decimal TBCTL { get; set; }
        public string Email { get; set; }
        public int? Phone { get; set; }
        public string Address { get; set; }
        public string Images { get; set; } =string.Empty;
    }
}
