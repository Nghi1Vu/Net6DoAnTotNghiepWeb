namespace DoAnTotNghiep.Models
{
    public class ExamCalendar
    {
        public string DepartmentName { get; set; }
        public string ClassName { get; set; }
        public string ClassCode { get; set; }
        public string Campus { get; set; }
        public string Room { get; set; }
        public long RegisterID { get; set; }
        public string ModulesName { get; set; }
        public string ExamTimeName { get; set; }
        public DateTime DateExam { get; set; }
        public DateTime CreatedTime { get; set; }
        public int ID { get; set; }
    }
}
