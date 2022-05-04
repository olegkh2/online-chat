using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using OnlineChat.Api.Context;
using OnlineChat.Api.Hubs;
using OnlineChat.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OnlineChat.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController : Controller
    {
        private ChatDbContext _context;

        public MessageController(ChatDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets 20 messages
        /// </summary>
        /// <param name="chatId">Chat Id</param>
        /// <param name="page">Desired iteration</param>
        /// <returns>List of 20 messages</returns>
        [HttpGet("{chatId}/{page?}")]
        public IActionResult Get(int chatId, int page = 0)
        {
            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if(!_context.ChatUsers.Contains(new ChatUser { UserId = userId, ChatId = chatId }))
            {
                return BadRequest();
            }

            var messages = _context.Messages
                .Include(x => x.User)
                .Where(x => x.ChatId == chatId)
                .OrderByDescending(x => x.Timestamp)
                .Skip(page * 20).Take(20)
                .Select(x => new
                {
                    Id = x.Id,
                    Text = x.Text,
                    Time = x.Timestamp.ToString("dd/MM/yyyy hh:mm:ss"),
                    UserId = x.User.Id,
                    UserName = x.User.Name,
                })
                .ToList();

            return Ok(messages);
        }

        /// <summary>
        /// Adds a message to the database and sends it to all users of the group
        /// </summary>
        /// <param name="roomId">Room Id</param>
        /// <param name="message">Message text</param>
        /// <param name="chat">Param from services</param>
        /// <returns>200 Ok status code</returns>
        [HttpPost("{roomId}/{message}")]
        public async Task<IActionResult> SendMessage(
            int roomId,
            string message,
            [FromServices] IHubContext<ChatHub> chat)
        {

            Message newMessage = new Message()
            {
                Text = message,
                Timestamp = DateTime.Now,
                ChatId = roomId,
                UserId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)),
            };

            await _context.Messages.AddAsync(newMessage);
            await _context.SaveChangesAsync();

            await chat.Clients.Groups(roomId.ToString()).SendAsync("RecieveMessage", new
            {
                Id = newMessage.Id,
                Text = newMessage.Text,
                Time = newMessage.Timestamp.ToString("dd/MM/yyyy hh:mm:ss"),
                UserId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier)),
                UserName = User.FindFirstValue(ClaimTypes.Name),
            });

            return Ok();
        }

        /// <summary>
        /// Updates existing message
        /// </summary>
        /// <param name="id">Message id</param>
        /// <param name="text">New message text</param>
        /// <returns>200 Ok status code</returns>
        [HttpPut]
        public async Task<IActionResult> Put(int id, string text)
        {

            var message = await _context.Messages.FirstOrDefaultAsync(x => x.Id == id);
            message.Text = text;
            await _context.SaveChangesAsync();

            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (message == null || message.UserId != userId)
                return BadRequest();

            return Ok();
        }

        /// <summary>
        /// Deletes existing message
        /// </summary>
        /// <param name="id">Message id</param>
        /// <returns>200 Ok status code</returns>
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {

            var message = await _context.Messages.FirstOrDefaultAsync(x => x.Id == id);

            var userId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (message == null || message.UserId != userId)
                return BadRequest();

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
