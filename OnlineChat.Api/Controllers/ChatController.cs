using OnlineChat.Api.Models;
using Microsoft.AspNetCore.Mvc;
using OnlineChat.Api.Context;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using OnlineChat.Api.Hubs;
using System;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;

namespace OnlineChat.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private ChatDbContext _context;

        public ChatController(ChatDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets a chat with all messages
        /// </summary>
        /// <param name="chatId"></param>
        /// <returns>Chat</returns>
        [HttpGet("{chatId}")]
        public IActionResult Get(int chatId)
        {
            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var chat = _context.Chats
                .Include(x => x.Messages)
                .Where(x => x.Id == chatId 
                    && x.Users.Contains(new ChatUser { UserId = userId, ChatId = chatId }))
                .Select(x => new
                {
                    Id = x.Id,
                    Name = x.Name,
                    Messages = x.Messages,
                })
                .FirstOrDefault();

            if (chat == null)
                return BadRequest();

            return Ok(chat);
        }

        /// <summary>
        /// Gets a list of chats to which the user is connected
        /// </summary>
        /// <returns>List of chats</returns>
        [HttpGet]
        public IActionResult GetAll()
        {
            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var chats = _context.Chats
                .Where(x => x.Users.Any(x => x.UserId == userId))
                .Select(x => new
                {
                    Id = x.Id,
                    Name = x.Name,
                })
                .ToList();

            if (chats == null)
                return BadRequest();

            return Ok(chats);
        }
    }
}
