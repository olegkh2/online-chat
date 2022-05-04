using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineChat.Api.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime Timestamp { get; set; }
        public int ChatId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }

    }
}
