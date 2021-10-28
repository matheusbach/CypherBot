using System;
using System.IO;
using System.Reflection;
using Telegram.Bot;

namespace CypherBot
{
    internal static class CypherBot
    {
        public static TelegramBotClient botClient = new TelegramBotClient("botTelegramToken");

        private static void Main(string[] args)
        {
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

            Console.WriteLine("Starting CypherBot");
            botClient.OnMessage += OnAction.BotClient_OnMessage;
            botClient.OnInlineQuery += OnAction.BotClient_OnInlineQuery;
            botClient.OnCallbackQuery += OnAction.BotClient_OnCallbackQuery;

            Data.ChannelPosts.LoadPostsData();
            Data.SorteioParticipantes.LoadParticipantesData();
            Data.MessagesToDelete.timerMessageToDeletesData.Elapsed += Data.MessagesToDelete.TimerMessageToDeletesData_Elapsed;
            Data.MessagesToDelete.LoadMessageToDeletesData();
            Data.MessagesToDelete.timerMessageToDeletesData.Start();

            botClient.StartReceiving();

            while (true)
            {
                Console.ReadKey();
            }
        }
    }
}