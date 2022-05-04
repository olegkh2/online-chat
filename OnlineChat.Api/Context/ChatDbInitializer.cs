using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using OnlineChat.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OnlineChat.Api.Context
{
    public static class ChatDbInitializer
    {
        public static void AddData(IApplicationBuilder applicationBuilder)
        {
            using(var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ChatDbContext>();

                context.Database.EnsureCreated();

                //Users
                if (!context.Users.Any())
                {
                    context.Users.AddRange(new List<User>()
                    {
                        new User()
                        {
                            Name = "Jhon",
                            Login = "user1",
                            Password = "123456",
                        },
                        new User()
                        {
                            Name = "Tom",
                            Login = "user2",
                            Password = "123456",
                        },
                        new User()
                        {
                            Name = "Alice",
                            Login = "user3",
                            Password = "123456",
                        },
                        new User()
                        {
                            Name = "Jennifer",
                            Login = "user4",
                            Password = "123456",
                        },
                    });

                    context.SaveChanges();
                }


                //Chats
                if (!context.Chats.Any())
                {
                    context.Chats.AddRange(new List<Chat>()
                    {
                        new Chat()
                        {
                            Name = "General chat",
                            Type = ChatType.Group,
                        },
                        new Chat()
                        {
                            Name = "Chat with Tom",
                            Type = ChatType.Private,
                        },
                        new Chat()
                        {
                            Name = "Group chat",
                            Type = ChatType.Group,
                        },
                    });

                    context.SaveChanges();
                }

                //ChatUser
                if (!context.ChatUsers.Any())
                {
                    context.ChatUsers.AddRange(new List<ChatUser>()
                    {
                        new ChatUser()
                        {
                            UserId = 1,
                            ChatId = 1,
                        },
                        new ChatUser()
                        {
                            UserId = 2,
                            ChatId = 1,
                        },
                        new ChatUser()
                        {
                            UserId = 3,
                            ChatId = 1,
                        },
                        new ChatUser()
                        {
                            UserId = 4,
                            ChatId = 1,
                        },
                        new ChatUser()
                        {
                            UserId = 1,
                            ChatId = 2,
                        },
                        new ChatUser()
                        {
                            UserId = 2,
                            ChatId = 2,
                        },
                        new ChatUser()
                        {
                            UserId = 1,
                            ChatId = 3,
                        },
                        new ChatUser()
                        {
                            UserId = 3,
                            ChatId = 3,
                        },
                        new ChatUser()
                        {
                            UserId = 4,
                            ChatId = 3,
                        },
                    });

                    context.SaveChanges();
                }

                //Messages
                if (!context.Messages.Any())
                {
                    Random rnd = new Random();
                    int minute = 0;
                    List<Message> fistChatMes = new List<Message>();

                    for(int i = 0; i < 35; i++)
                    {
                        fistChatMes.Add(new Message()
                        {
                            Text = getRandomString(),
                            Timestamp = DateTime.Now.AddSeconds(60 * minute++),
                            ChatId = 1,
                            UserId = rnd.Next(1,5),
                        });
                    }
                    context.Messages.AddRange(fistChatMes);

                    List<Message> secondChatMes = new List<Message>();

                    for (int i = 0; i < 15; i++)
                    {
                        secondChatMes.Add(new Message()
                        {
                            Text = getRandomString(),
                            Timestamp = DateTime.Now.AddSeconds(60 * minute++),
                            ChatId = 2,
                            UserId = rnd.Next(1, 3),
                        });
                    }
                    context.Messages.AddRange(secondChatMes);

                    List<Message> thirdChatMes = new List<Message>();

                    for (int i = 0; i < 15; i++)
                    {
                        int userId = rnd.Next(1, 5);
                        if (userId == 2)
                            userId = 1;

                        thirdChatMes.Add(new Message()
                        {
                            Text = getRandomString(),
                            Timestamp = DateTime.Now.AddSeconds(60 * minute++),
                            ChatId = 3,
                            UserId = userId,
                        });
                    }
                    context.Messages.AddRange(thirdChatMes);

                    context.SaveChanges();
                }
            }
        }

        private static string getRandomString()
        {
            Random rnd = new Random();
            StringBuilder sb = new StringBuilder();
            string Alphabet = "qwertyuiopasdfghjklzxcvbnm012345679 ";

            for (int i = 0; i < rnd.Next(5, 50); i++)
            {
                sb.Append(Alphabet[rnd.Next(0, Alphabet.Length - 1)]);
            }
            return sb.ToString();
        }
    }
}
