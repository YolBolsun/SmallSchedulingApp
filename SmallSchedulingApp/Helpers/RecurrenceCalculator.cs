using System;
using System.Collections.Generic;
using System.Linq;
using SmallSchedulingApp.Models;

namespace SmallSchedulingApp.Helpers
{
    public static class RecurrenceCalculator
    {
        public static List<DateTime> CalculateOccurrences(CalendarEvent evt)
        {
            var dates = new List<DateTime>();
            var currentDate = evt.StartDate.Date;

            for (int i = 0; i < evt.Occurrences; i++)
            {
                dates.Add(currentDate);

                currentDate = evt.Frequency switch
                {
                    EventFrequency.Daily => currentDate.AddDays(1),
                    EventFrequency.Weekly => currentDate.AddDays(7),
                    EventFrequency.BiWeekly => currentDate.AddDays(14),
                    EventFrequency.Monthly => currentDate.AddMonths(1),
                    _ => currentDate
                };
            }

            return dates;
        }

        public static bool EventOccursOnDate(CalendarEvent evt, DateTime date)
        {
            var occurrences = CalculateOccurrences(evt);
            return occurrences.Any(d => d.Date == date.Date);
        }
    }
}
