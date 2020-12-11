//using System.Collections.Generic;
//using System.Threading.Tasks;
//using Telegram.Bot;
//using TelegramBot.Models.Commands;

//namespace TelegramBot.Models
//{
//    public static class Bot
//    {
//        private static TelegramBotClient client;
//        private static List<Command> commandsList;
//        public static IReadOnlyList<Command> Commands { get => commandsList.AsReadOnly(); }

//        public static async Task<TelegramBotClient> Get()
//        {
//            if (client != null)
//            {
//                return client;
//            }

//            commandsList = new List<Command>();
//            commandsList.Add(new HelloCommand());
//            commandsList.Add(new ApplicantNameCommand());
//            commandsList.Add(new ApplicantNumberCommand());
//            commandsList.Add(new ApplicantPositionCommand());
//            commandsList.Add(new ClientNameCommand());
//            commandsList.Add(new ProblemCommand());
//            commandsList.Add(new DateTimeCommand());
//            commandsList.Add(new ProblemPhotoCommand());

//            //TODO: Add more commands
 
//            client = new TelegramBotClient(ConfigurationManager.AppSetting.GetSection("TelegramBotSettings:Key").Value);
//            string hook = ConfigurationManager.AppSetting.GetSection("TelegramBotSettings:Url").Value + "api/message/update";
//            await client.SetWebhookAsync(hook);
//            return client;
//        }
//    }
//}
