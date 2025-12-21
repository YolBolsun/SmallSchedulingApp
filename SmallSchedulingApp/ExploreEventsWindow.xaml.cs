using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using SmallSchedulingApp.Models;
using SmallSchedulingApp.Services;

namespace SmallSchedulingApp
{
    /// <summary>
    /// Window for exploring and adding events from CSV
    /// </summary>
    public sealed partial class ExploreEventsWindow : Window
    {
        private readonly CsvService _csvService;
        private readonly EventService _eventService;
        private List<ExploreEvent> _allEvents = new();
        private List<ExploreEvent> _filteredEvents = new();
        private string _currentFilter = "All";

        public ExploreEventsWindow(EventService eventService)
        {
            InitializeComponent();
            _csvService = new CsvService();
            _eventService = eventService;

            // Set window size
            var appWindow = this.AppWindow;
            appWindow.Resize(new Windows.Graphics.SizeInt32 { Width = 900, Height = 700 });

            // Load events
            LoadEventsAsync();
        }

        private async void LoadEventsAsync()
        {
            try
            {
                MessageText.Text = "Loading events...";

                var events = await _csvService.FetchExploreEventsAsync();
                _allEvents = _csvService.FilterByDateRange(events);

                if (_allEvents.Count == 0)
                {
                    MessageText.Text = "No events found. Using default CSV URL.";
                }
                else
                {
                    MessageText.Text = $"Found {_allEvents.Count} events";
                }

                ApplyFilter(_currentFilter);
            }
            catch (Exception ex)
            {
                MessageText.Text = $"Error loading events: {ex.Message}";
            }
        }

        public void SetCsvUrl(string url)
        {
            _csvService.SetCsvUrl(url);
        }

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton button)
            {
                // Uncheck all other filter buttons
                AllFilterButton.IsChecked = false;
                MovieFilterButton.IsChecked = false;
                ShowFilterButton.IsChecked = false;
                AnimeFilterButton.IsChecked = false;

                // Check the clicked button
                button.IsChecked = true;

                var tag = button.Tag?.ToString() ?? "All";
                _currentFilter = tag;
                ApplyFilter(tag);
            }
        }

        private void ApplyFilter(string tag)
        {
            _filteredEvents = _csvService.FilterByTag(_allEvents, tag);
            EventsItemsControl.ItemsSource = _filteredEvents;

            if (_filteredEvents.Count == 0 && _allEvents.Count > 0)
            {
                MessageText.Text = $"No events found for filter: {tag}";
            }
            else if (_filteredEvents.Count > 0)
            {
                MessageText.Text = $"Showing {_filteredEvents.Count} events";
            }
        }

        private void AddEventButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is ExploreEvent exploreEvent)
            {
                // Convert ExploreEvent to CalendarEvent
                var calendarEvent = new CalendarEvent
                {
                    EventName = exploreEvent.Name,
                    StartDate = exploreEvent.StartDate,
                    Frequency = exploreEvent.Frequency,
                    Occurrences = exploreEvent.Count
                };

                _eventService.AddEvent(calendarEvent);

                // Show confirmation
                ShowConfirmationDialog(exploreEvent.Name);
            }
        }

        private async void ShowConfirmationDialog(string eventName)
        {
            var dialog = new ContentDialog
            {
                Title = "Event Added",
                Content = $"'{eventName}' has been added to your calendar.",
                CloseButtonText = "OK",
                XamlRoot = this.Content.XamlRoot
            };

            await dialog.ShowAsync();
        }
    }
}
