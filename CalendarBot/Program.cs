using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;

namespace CalendarQuickstart
{
	class Program
	{
        private static readonly TelegramBotClient bot = new TelegramBotClient("560256367:AAHJ2SGUS82VlqzaR53o9Oc7I8a1k1l3jtQ");
		
        static void Main(string[] args)
        {
            bot.OnMessage += bot_OnMessage;
            bot.StartReceiving();
            Console.ReadLine();
            bot.StopReceiving();
        }	

        private static void bot_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.TextMessage)
                bot.SendTextMessageAsync(e.Message.Chat.Id, "Gotcha");
        }
	}
}