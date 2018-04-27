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
		static string[] Scopes = { CalendarService.Scope.CalendarReadonly };
		static string ApplicationName = "Google Calendar API .NET Quickstart";
		static UserCredential credential;
        static Boolean calendar_retrieved;
        static Events events;
        static CalendarService service;

		static void Main(string[] args)
        {
			using (var stream =
				new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
			{
				string credPath = System.Environment.GetFolderPath(
					System.Environment.SpecialFolder.Personal);
				credPath = Path.Combine(credPath, ".credentials/calendar-dotnet-quickstart.json");

				credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
					GoogleClientSecrets.Load(stream).Secrets,
					Scopes,
					"user",
					CancellationToken.None,
					new FileDataStore(credPath, true)).Result;
				Console.WriteLine("Credential file saved to: " + credPath);
			}

			bot.OnMessage += bot_OnMessage;
            bot.StartReceiving();
            Console.ReadLine();
            bot.StopReceiving();
        }

        private static void get_calendar(){
			service = new CalendarService(new BaseClientService.Initializer()
			{
				HttpClientInitializer = credential,
				ApplicationName = ApplicationName,
			});

			
		}

        private static void bot_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            if  (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.TextMessage)
            {
                if(!calendar_retrieved){
                    get_calendar();
                }

				// List events.
				EventsResource.ListRequest request = service.Events.List("primary");
				request.TimeMin = DateTime.Now;
				request.ShowDeleted = false;
				request.SingleEvents = true;
				request.MaxResults = 10;
				request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;
				calendar_retrieved = true;
				events = request.Execute();
                bot.SendTextMessageAsync(e.Message.Chat.Id, "Upcoming events:");
                if (events.Items != null && events.Items.Count > 0)
                {
                    foreach (var eventItem in events.Items)
                    {
                        string when = eventItem.Start.DateTime.ToString();
                        if (String.IsNullOrEmpty(when))
                        {
                            when = eventItem.Start.Date;
                        }
                        //Console.WriteLine("{0} ({1})", eventItem.Summary, when);
                        bot.SendTextMessageAsync(e.Message.Chat.Id, eventItem.Summary + when);
                    }
                }
                else
                {
                    bot.SendTextMessageAsync(e.Message.Chat.Id, "No upcoming events found.");
                }
                Console.Read();
            }
        }
	}
}