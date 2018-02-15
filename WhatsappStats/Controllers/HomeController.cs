using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WhatsappStats.Models;

namespace WhatsappStats.Controllers
{
    public class HomeController : Controller
    {
        int wordcount = 0;
        int MediaCount = 0;
        double TotalMessages = 0;
        double BoyfriendMessages = 0;
        double GirlfriendMessages = 0;
        int[] MessagesOnHours = new int[24];
        int streak = 0;

        Dictionary<string, List<MessageObject>> MessagesByMonth = new Dictionary<string, List<MessageObject>>();
        Dictionary<string, List<MessageObject>> MessagesByWeekDay = new Dictionary<string, List<MessageObject>>();
        public IActionResult Index()
        {
            var vm = new ViewModel();
            vm.Dict = ParseWhatsappDataToDayList();
            vm.StartedTalkingDate = DateTime.ParseExact(vm.Dict.Keys.First(),"dd-MM-yy",CultureInfo.InvariantCulture).ToLongDateString();
            var mostActiveRecord = vm.Dict.OrderByDescending(x => x.Value.Count()).First();
            vm.MostActiveDay = DateTime.ParseExact(mostActiveRecord.Key, "dd-MM-yy", CultureInfo.InvariantCulture).ToLongDateString();
            vm.MessagesOnMostActiveDay = mostActiveRecord.Value.Count();
            vm.WordCount = wordcount;
            vm.MediaCount = MediaCount;
            vm.TotalMessages = TotalMessages;
            vm.GirlfriendMessages = GirlfriendMessages;
            vm.BoyfriendMessages = BoyfriendMessages;
            vm.MessagesOnHours = MessagesOnHours;
            vm.DayLabels = vm.Dict.Keys.ToArray();
            vm.DayMessagesCount = vm.Dict.Select(x => x.Value.Count()).ToArray();

            var date = DateTime.ParseExact(vm.Dict.Keys.First(), "dd-MM-yy", CultureInfo.InvariantCulture);
            foreach (var entry in vm.Dict)
            {
                if (entry.Key == date.ToString("dd-MM-yy")) continue;
                var nextDay = date.AddDays(1);
                var testable = nextDay.ToString("dd-MM-yy");
                if (entry.Key == testable)
                {
                    streak++;
                    date = nextDay;
                }
                else {
                    streak = 0;
                }
                date = DateTime.ParseExact(entry.Key, "dd-MM-yy", CultureInfo.InvariantCulture);
            }

            vm.TalkingStreak = streak;
            vm.MonthLabels = MessagesByMonth.Keys.ToArray();
            vm.MonthMessagesCount = MessagesByMonth.Select(x => x.Value.Count()).ToArray();

            vm.WeekDayLabels = MessagesByMonth.Keys.ToArray();
            vm.WeekDayMessagesCount = MessagesByMonth.Select(x => x.Value.Count()).ToArray();
            return View(vm);
        }

        private Dictionary<string, List<MessageObject>> ParseWhatsappDataToDayList()
        {
            var dict = new Dictionary<string, List<MessageObject>>();
            string pathToFile = "C:\\";
            string fileName = "WhatsApp_Jolijn.txt";
            string fullPath = Path.Combine(pathToFile, fileName);

            using (StreamReader sr = new StreamReader(fullPath))
            {
                string currentLine;
                while ((currentLine = sr.ReadLine()) != null)
                {
                    
                    if (string.IsNullOrWhiteSpace(currentLine))
                        continue;

                    var index = currentLine.IndexOf(',');

                    if (index == -1)
                    {
                        wordcount += currentLine.Split(' ').Count();
                        continue;
                    }
                     

                    var date = currentLine.Substring(0, index).Trim();
                    DateTime dateTime;
                    if (!DateTime.TryParseExact(date, "dd-MM-yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime)) {
                        wordcount += currentLine.Split(' ').Count();
                        continue;
                    }

                    currentLine = currentLine.Substring(index+1).Trim();
                    index = currentLine.IndexOf('-');
                    var time = TimeSpan.Parse(currentLine.Substring(0, index).Trim());
                    currentLine = currentLine.Substring(index + 1).Trim();
                    index = currentLine.IndexOf(':');
                    var sender = currentLine.Substring(0, index).Trim();
                    currentLine = currentLine.Substring(index + 1).Trim();
                    var media = currentLine.Contains("<Media weggelaten>");
                    if (!dict.ContainsKey(date)) {
                        dict.Add(date, new List<MessageObject>());
                    }

                    MessagesOnHours[time.Hours]++;

                    if (sender == "Jolijn")
                    {
                        GirlfriendMessages++;
                    }
                    else if(sender == "Xander Den Hartog") {
                        BoyfriendMessages++;
                    }

                    if (!media)
                    {
                        wordcount += currentLine.Split(' ').Count();
                    }
                    else {
                        MediaCount++;
                    }
                    TotalMessages++;

                    var monthkey = dateTime.ToString("MMM-yy");
                    if (!MessagesByMonth.ContainsKey(monthkey))
                        MessagesByMonth.Add(monthkey, new List<MessageObject>());

                    var WeekDaykey = dateTime.ToString("MMM-yy");
                    if (!MessagesByWeekDay.ContainsKey(WeekDaykey))
                        MessagesByWeekDay.Add(WeekDaykey, new List<MessageObject>());

                    MessagesByWeekDay[WeekDaykey].Add(new MessageObject { Message = currentLine, Sender = sender, Time = time, Media = media });
                    MessagesByMonth[monthkey].Add(new MessageObject { Message = currentLine, Sender = sender, Time = time, Media = media });
                    dict[date].Add(new MessageObject { Message = currentLine, Sender = sender, Time = time, Media = media });
                }
            }

            return dict;
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
