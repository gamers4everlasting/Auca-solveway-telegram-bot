//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using Telegram.Bot;
//using Telegram.Bot.Args;
//using Telegram.Bot.Types;
//using Telegram.Bot.Types.Enums;

//namespace TelegramBot.Models
//{
//    public static  class BotExample
//    {
//        public static async Task StarGetUserInformationsAsync(Message message, ITelegramBotClient telegramBotClient)
//        {
//            var step = 1;
//            var userInfos = new List<string>();

//            var mre = new ManualResetEvent(false);

//            EventHandler<MessageEventArgs> mHandler = (sender, e) =>
//            {
//                if (message.From.Id != e.Message.From.Id) return;
//                if (message.Chat.Id != e.Message.Chat.Id) return;

//                if (step == 1)
//                {
//                    userInfos.Add(e.Message.Text);
//                    step++;
//                    telegramBotClient.SendTextMessageAsync(message.Chat.Id, $"Nice! How old are you, {e.Message.Text}?");
//                }
//                else if (step == 2)
//                {
//                    //if (e.Message.Text.IsInt())
//                    //{
//                    userInfos.Add(e.Message.Text);
//                    step++;
//                    telegramBotClient.SendTextMessageAsync(message.Chat.Id, "Where do you live?");
//                    //}
//                    //else
//                    //{
//                    //    _telegramBotClient.SendTextMessageAsync(message.Chat.Id, "Only numbers are allowed!");
//                    //    return;
//                    //}
//                }
//                else if (step == 3)
//                {
//                    userInfos.Add(e.Message.Text);
//                    step++;
//                    telegramBotClient.SendTextMessageAsync(message.Chat.Id, "Is this example helpful?");
//                }

//                else if (step == 4)
//                {
//                    userInfos.Add(e.Message.Text);
//                    telegramBotClient.SendTextMessageAsync(message.Chat.Id, $"Okay! Your entered informations:\n\nYour Name: *{userInfos.ElementAt(0)}*\nYour Age: *{userInfos.ElementAt(1)}*\nYou live in: *{userInfos.ElementAt(2)}*\nHelpful example? *{userInfos.ElementAt(3)}*", ParseMode.Markdown);
//                }
//            };

//            await telegramBotClient.SendTextMessageAsync(message.Chat.Id, $"Hello!\nLet me ask you a few questions\n\nWhat is your name?", ParseMode.Markdown);

//            telegramBotClient.OnMessage += mHandler;
//            mre.WaitOne();
//            telegramBotClient.OnMessage -= mHandler;
//        }
//    }
//}
