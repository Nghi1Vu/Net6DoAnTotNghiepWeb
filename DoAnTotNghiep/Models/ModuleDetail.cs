namespace DoAnTotNghiep.Models
{
    public class ModuleDetail
    {
        public string DepartmentName { get; set; }
        public string HPModules { get; set; }
        public string ModulesName { get; set; }
        public string ModulesNameSort { get; set; }
        public string ModulesCode { get; set; }
        public string Descriptions { get; set; }
        public string Purposely { get; set; }
        public string PurposelyKN { get; set; }
        public string PurposelyYT { get; set; }
        public string ChapterName { get; set; }
        public string ContentChapter { get; set; }
        public int TimesST { get; set; }
        public int TimesLT { get; set; }
        public int TimesBT { get; set; }
        public int TimesTL { get; set; }
        public int TimesTH { get; set; }
        public int TimesK { get; set; }
        public int TimesTest { get; set; }
        public string References { get; set; }
        public string TranningLevelName { get; set; }
        public string ExamTypeName { get; set; }
        public int ExamRateTL { get; set; }
        public int ExamRateTN { get; set; }
        public int ExamRateK { get; set; }
        public int NumberStPerClass { get; set; }
        public Decimal Credits { get; set; }
    }
}
