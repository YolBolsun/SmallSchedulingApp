using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using SmallSchedulingApp.Models;

namespace SmallSchedulingApp.Services
{
    /// <summary>
    /// Service for fetching and parsing CSV data for explore events
    /// </summary>
    public class CsvService
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private string _csvUrl = "https://raw.githubusercontent.com/YolBolsun/SmallSchedulingApp/main/defaultEvents.csv"; // Default URL

        public void SetCsvUrl(string url)
        {
            _csvUrl = url;
        }

        public async Task<List<ExploreEvent>> FetchExploreEventsAsync()
        {
            try
            {
                var csvContent = await _httpClient.GetStringAsync(_csvUrl);
                return ParseCsv(csvContent);
            }
            catch
            {
                // Return empty list if fetch fails
                return new List<ExploreEvent>();
            }
        }

        private List<ExploreEvent> ParseCsv(string csvContent)
        {
            var events = new List<ExploreEvent>();
            var lines = csvContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            // Skip header row
            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i];
                var values = ParseCsvLine(line);

                if (values.Count < 6) continue; // Need at least Name, Summary, Image, StartDate, Frequency, Count

                var exploreEvent = new ExploreEvent
                {
                    Name = values[0],
                    Summary = values[1],
                    ImageUrl = values[2],
                    Count = int.TryParse(values[5], out var count) ? count : 1
                };

                // Parse start date
                if (DateTime.TryParse(values[3], out var startDate))
                {
                    exploreEvent.StartDate = startDate;
                }
                else
                {
                    continue; // Skip events with invalid dates
                }

                // Parse frequency
                exploreEvent.Frequency = values[4].ToLower() switch
                {
                    "daily" => EventFrequency.Daily,
                    "weekly" => EventFrequency.Weekly,
                    "bi-weekly" or "biweekly" => EventFrequency.BiWeekly,
                    "monthly" => EventFrequency.Monthly,
                    _ => EventFrequency.Daily
                };

                // Parse tags (Tag1, Tag2, Tag3)
                for (int j = 6; j < values.Count && j < 9; j++)
                {
                    if (!string.IsNullOrWhiteSpace(values[j]))
                    {
                        exploreEvent.Tags.Add(values[j].Trim());
                    }
                }

                events.Add(exploreEvent);
            }

            return events;
        }

        private List<string> ParseCsvLine(string line)
        {
            var values = new List<string>();
            var currentValue = string.Empty;
            var insideQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"')
                {
                    insideQuotes = !insideQuotes;
                }
                else if (c == ',' && !insideQuotes)
                {
                    values.Add(currentValue.Trim());
                    currentValue = string.Empty;
                }
                else
                {
                    currentValue += c;
                }
            }

            // Add the last value
            values.Add(currentValue.Trim());

            return values;
        }

        /// <summary>
        /// Filter events to show only those from 2 weeks ago to future
        /// </summary>
        public List<ExploreEvent> FilterByDateRange(List<ExploreEvent> events)
        {
            var twoWeeksAgo = DateTime.Now.AddDays(-14);
            return events.Where(e => e.StartDate >= twoWeeksAgo).ToList();
        }

        /// <summary>
        /// Filter events by tag
        /// </summary>
        public List<ExploreEvent> FilterByTag(List<ExploreEvent> events, string tag)
        {
            if (string.IsNullOrWhiteSpace(tag) || tag.ToLower() == "all")
            {
                return events;
            }

            return events.Where(e => e.Tags.Any(t => t.Equals(tag, StringComparison.OrdinalIgnoreCase))).ToList();
        }
    }
}
