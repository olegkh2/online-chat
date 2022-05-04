using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineChat.Api.Models
{
    public class Chat
    {
        public Chat()
        {
            Messages = new List<Message>();
            Users = new List<ChatUser>();
        }
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Message> Messages { get; set; }
        public ICollection<ChatUser> Users { get; set; }
        public ChatType Type { get; set; }
    }
}
