namespace DoAnTotNghiep.Models
{
    public class PostRLForm
    {
        public int SemesterID { get; set; }
        public List<RLFormDetail> detail { get; set; }


    }
    public class RLFormDetail
    {
        public int RLAnswerID { get; set; }
        public int Score { get; set; }
    }
}
