namespace OnlineChat.Api.Models
{
    public class ChatUser
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public int ChatId { get; set; }
        public UserRole Role { get; set; }
    }
}
