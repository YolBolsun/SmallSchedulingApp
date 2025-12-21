using System;
using System.Collections.Generic;
using System.Linq;
using SmallSchedulingApp.Helpers;
using SmallSchedulingApp.Models;

namespace SmallSchedulingApp.Services
{
    public class EventService
    {
        private readonly DatabaseService _db;

        public EventService()
        {
            _db = DatabaseService.Instance;
        }

        public List<CalendarEvent> GetAllEvents()
        {
            return _db.Events.FindAll().ToList();
        }

        public void AddEvent(CalendarEvent evt)
        {
            _db.Events.Insert(evt);
        }

        public void DeleteEvent(int eventId)
        {
            _db.Events.Delete(eventId);
        }

        public void DeleteSingleOccurrence(int eventId, DateTime dateToDelete)
        {
            var evt = _db.Events.FindById(eventId);
            if (evt == null) return;

            var allDates = RecurrenceCalculator.CalculateOccurrences(evt);
            var index = allDates.FindIndex(d => d.Date == dateToDelete.Date);

            if (index == -1) return;

            if (index == 0)
            {
                // First occurrence - adjust start date
                if (allDates.Count > 1)
                {
                    evt.StartDate = allDates[1];
                    evt.Occurrences--;
                    _db.Events.Update(evt);
                }
                else
                {
                    // Only one occurrence, delete the whole event
                    _db.Events.Delete(eventId);
                }
            }
            else if (index == allDates.Count - 1)
            {
                // Last occurrence - reduce count
                evt.Occurrences--;
                _db.Events.Update(evt);
            }
            else
            {
                // Middle occurrence - split into two events
                var beforeEvent = new CalendarEvent
                {
                    EventName = evt.EventName,
                    StartDate = evt.StartDate,
                    Frequency = evt.Frequency,
                    Occurrences = index
                };

                var afterEvent = new CalendarEvent
                {
                    EventName = evt.EventName,
                    StartDate = allDates[index + 1],
                    Frequency = evt.Frequency,
                    Occurrences = evt.Occurrences - index - 1
                };

                _db.Events.Delete(eventId);
                _db.Events.Insert(beforeEvent);
                _db.Events.Insert(afterEvent);
            }
        }

        public List<CalendarEvent> GetEventsForDate(DateTime date)
        {
            var allEvents = GetAllEvents();
            var eventsOnDate = new List<CalendarEvent>();

            foreach (var evt in allEvents)
            {
                if (RecurrenceCalculator.EventOccursOnDate(evt, date))
                {
                    eventsOnDate.Add(evt);
                }
            }

            return eventsOnDate;
        }

        public Dictionary<DateTime, List<CalendarEvent>> GetEventsForMonth(int year, int month)
        {
            var firstDay = new DateTime(year, month, 1);
            var lastDay = firstDay.AddMonths(1).AddDays(-1);

            var allEvents = GetAllEvents();
            var eventsInMonth = new Dictionary<DateTime, List<CalendarEvent>>();

            foreach (var evt in allEvents)
            {
                var dates = RecurrenceCalculator.CalculateOccurrences(evt);
                foreach (var date in dates.Where(d => d >= firstDay && d <= lastDay))
                {
                    if (!eventsInMonth.ContainsKey(date.Date))
                        eventsInMonth[date.Date] = new List<CalendarEvent>();

                    eventsInMonth[date.Date].Add(evt);
                }
            }

            return eventsInMonth;
        }
    }
}
