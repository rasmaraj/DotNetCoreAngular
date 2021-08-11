namespace API.Entities
{
    public class AppUser
    {
       
        public int id { get; set; }
        public string Username { get; set; }
        public byte[] PasswordHash{get;set;}
        public byte[] PasswordSalt{get;set;}
    }
}