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
                var result = ParseCsv(csvContent);
                return result;
            }
            catch (Exception ex)
            {
                // Log the error for debugging
                System.Diagnostics.Debug.WriteLine($"Error fetching CSV: {ex.Message}");
                throw; // Re-throw to let caller handle
            }
        }

        private List<ExploreEvent> ParseCsv(string csvContent)
        {
            var events = new List<ExploreEvent>();
            var lines = csvContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            System.Diagnostics.Debug.WriteLine($"Total lines in CSV: {lines.Length}");

            // Skip header row
            for (int i = 1; i < lines.Length; i++)
            {
                try
                {
                    var line = lines[i];
                    System.Diagnostics.Debug.WriteLine($"Parsing line {i}: {line}");

                    var values = ParseCsvLine(line);
                    System.Diagnostics.Debug.WriteLine($"Parsed {values.Count} values");

                    if (values.Count < 6)
                    {
                        System.Diagnostics.Debug.WriteLine($"Skipping line {i}: Not enough values ({values.Count})");
                        continue;
                    }

                    var exploreEvent = new ExploreEvent
                    {
                        Name = values[0].Trim(),
                        Summary = values[1].Trim(),
                        ImageUrl = values[2].Trim(),
                        Count = int.TryParse(values[5], out var count) ? count : 1
                    };

                    // Parse start date
                    if (DateTime.TryParse(values[3], out var startDate))
                    {
                        exploreEvent.StartDate = startDate;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"Skipping line {i}: Invalid date '{values[3]}'");
                        continue;
                    }

                    // Parse frequency
                    exploreEvent.Frequency = values[4].Trim().ToLower() switch
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

                    System.Diagnostics.Debug.WriteLine($"Successfully parsed event: {exploreEvent.Name}");
                    events.Add(exploreEvent);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error parsing line {i}: {ex.Message}");
                }
            }

            System.Diagnostics.Debug.WriteLine($"Total events parsed: {events.Count}");
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
                    // Toggle quote state, but don't add the quote to the value
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

            System.Diagnostics.Debug.WriteLine($"ParseCsvLine result: {string.Join(" | ", values)}");
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
            System.Diagnostics.Debug.WriteLine($"FilterByTag called with tag: '{tag}', event count: {events.Count}");

            if (string.IsNullOrWhiteSpace(tag) || tag.ToLower() == "all")
            {
                System.Diagnostics.Debug.WriteLine($"Returning all {events.Count} events (no filter)");
                return events;
            }

            foreach (var evt in events)
            {
                System.Diagnostics.Debug.WriteLine($"Event '{evt.Name}' has tags: {string.Join(", ", evt.Tags)}");
            }

            var filtered = events.Where(e => e.Tags.Any(t => t.Equals(tag, StringComparison.OrdinalIgnoreCase))).ToList();
            System.Diagnostics.Debug.WriteLine($"FilterByTag returning {filtered.Count} events");
            return filtered;
        }
    }
}
