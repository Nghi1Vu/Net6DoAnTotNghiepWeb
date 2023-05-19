namespace DoAnTotNghiep.Models
{
    public class Message
    {
        public string Content { get; set; }
        public string fromuser { get; set; }
        public int CommentID { get; set; }
        public int ParentID { get; set; }
        public int OwnerID { get; set; }
        public string touser { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}
