namespace DoAnTotNghiep.Models
{
    public class InfoLogin
    {
        public string Username { get; set; }

        public string Password { get; set; }
    }
    public class ChangePass
    {
        public string oldpass { get; set; }
        public string newpass { get; set; }

        public string validpass { get; set; }
    }
    public class TokenModel
    {
        public string email { get; set; }

    }
    public class AccessTokenModel
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string scope { get; set; }
        public string token_type { get; set; }
        public string id_token { get; set; }

    }
}