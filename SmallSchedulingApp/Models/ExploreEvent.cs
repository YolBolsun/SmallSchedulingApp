using System;
using System.Collections.Generic;

namespace SmallSchedulingApp.Models
{
    /// <summary>
    /// Represents an event from the CSV that can be explored and added to the user's calendar
    /// </summary>
    public class ExploreEvent
    {
        public string Name { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public EventFrequency Frequency { get; set; }
        public int Count { get; set; }
        public List<string> Tags { get; set; } = new();
    }
}
