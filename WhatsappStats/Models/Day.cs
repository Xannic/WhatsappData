using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WhatsappStats.Models
{
    public class Day
    {
        public List<MessageObject> MessageObjects { get; set; }
    }

    public class MessageObject
    {
        public string Message { get; set; }
        public string Sender { get; set; }
        public TimeSpan Time { get; set; }
        public bool Media { get; set; }
    }

    public class ViewModel {
        public Dictionary<string, List<MessageObject>> Dict { get; set; }
        public string StartedTalkingDate { get; set; }
        public string MostActiveDay { get; set; }
        public int MessagesOnMostActiveDay { get; set; }
        public int WordCount { get; set; }
        public int MediaCount { get; set; }
        public double TotalMessages { get; set; }
        public double BoyfriendMessages { get; set; }
        public double GirlfriendMessages { get; set; }
        public int[] MessagesOnHours { get; set; }
        public string[] DayLabels { get; set; }
        public int[] DayMessagesCount { get; set; }
        public string[] MonthLabels { get; set; }
        public int[] MonthMessagesCount { get; set; }
        public int TalkingStreak { get; set; }
    }
}
