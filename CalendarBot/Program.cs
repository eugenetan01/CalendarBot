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

namespace CalendarBot
{
    class Program
    {
        private static readonly TelegramBotClient bot = new TelegramBotClient("Enter code here");
        static string[] Scopes = { CalendarService.Scope.CalendarReadonly };
        static string ApplicationName = "Google Calendar API .NET Quickstart";
        static UserCredential credential;
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
            }

            bot.OnMessage += bot_OnMessage;
            bot.StartReceiving();
            Console.ReadLine();
            bot.StopReceiving();
        }

        private static void get_calendar()
        {
            service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });


        }

        private static void bot_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            string text = e.Message.Text;
            Console.WriteLine(text);
            if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.TextMessage && e.Message.Text.Equals("/getCal"))
            {
				get_calendar();
				EventsResource.ListRequest request = service.Events.List("primary");
				request.TimeMin = DateTime.Now;
				request.TimeMax = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
				request.ShowDeleted = false;
				request.SingleEvents = true;
				request.MaxResults = 10;
				request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;
				events = request.Execute();

				if (events.Items != null && events.Items.Count > 0)
				{
                    for (int i = 0; i <= events.Items.Count()-1; i++)
					{
                        var eventItem = events.Items[i];
						string when = eventItem.Start.DateTime.ToString();
						if (String.IsNullOrEmpty(when))
						{
							when = eventItem.Start.Date;
						}
                        bot.SendTextMessageAsync(e.Message.Chat.Id, eventItem.Summary + " " + when + " " + eventItem.Location);
					}
				}
				else
					bot.SendTextMessageAsync(e.Message.Chat.Id, "No upcoming events found.");
            }
            else
                bot.SendTextMessageAsync(e.Message.Chat.Id, "Please enter the correct command.");
        
        }
    }
}
