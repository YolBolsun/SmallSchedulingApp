using System;
using System.Collections.Generic;
using LiteDB;

namespace SmallSchedulingApp.Models
{
    public class CalendarEvent
    {
        [BsonId]
        public int Id { get; set; }

        public string EventName { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }

        public EventFrequency Frequency { get; set; } = EventFrequency.Daily;

        public int Occurrences { get; set; } = 1;

        // Auto-calculated field (not stored in database)
        [BsonIgnore]
        public List<DateTime> CalculatedDates { get; set; } = new();
    }
}
