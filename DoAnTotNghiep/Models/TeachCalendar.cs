namespace DoAnTotNghiep.Models
{
    public class TeachCalendar
    {
        public string teacherphone { get; set; }
        public string teachername { get; set; }
        public string MHP { get; set; }
        public string ModulesName { get; set; }
        public decimal Credits { get; set; }
        public DateTime minDay { get; set; }
        public DateTime maxDay { get; set; }
        public int IndependentClassID { get; set; }
    }
    public class TeachCalendarDetail
    {
        public DateTime Day { get; set; }
        public string Contents { get; set; }
        public string Description { get; set; }
        public int HaveTest { get; set; }

    }
}
