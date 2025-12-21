using System;
using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SmallSchedulingApp.Helpers;
using SmallSchedulingApp.Services;

namespace SmallSchedulingApp
{
    /// <summary>
    /// The main calendar popup window
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private SystemTrayHelper? _systemTrayHelper;
        private readonly EventService _eventService;
        private DateTime _currentMonth;
        private bool _isVisible;

        public MainWindow()
        {
            InitializeComponent();

            _eventService = new EventService();
            _currentMonth = DateTime.Now;
            _isVisible = false;

            // Set window size
            var appWindow = this.AppWindow;
            appWindow.Resize(new Windows.Graphics.SizeInt32 { Width = 450, Height = 550 });

            // Window should be topmost
            appWindow.IsShownInSwitchers = false;

            // Start window off-screen (hidden)
            appWindow.Move(new Windows.Graphics.PointInt32 { X = -10000, Y = -10000 });

            // Subscribe to window events
            this.Activated += MainWindow_Activated;
            this.Closed += MainWindow_Closed;

            // Initialize calendar display
            UpdateCalendarDisplay();
        }

        public bool IsVisible => _isVisible;

        public void Show()
        {
            _isVisible = true;
            this.Activate();
        }

        public void Hide()
        {
            _isVisible = false;
            // Move window off-screen to "hide" it
            var appWindow = this.AppWindow;
            appWindow.Move(new Windows.Graphics.PointInt32 { X = -10000, Y = -10000 });
        }

        private void UpdateCalendarDisplay()
        {
            // Update month/year text
            MonthYearText.Text = _currentMonth.ToString("MMMM yyyy");

            // Generate calendar grid
            GenerateCalendarGrid();
        }

        private void GenerateCalendarGrid()
        {
            CalendarGrid.Children.Clear();
            CalendarGrid.RowDefinitions.Clear();
            CalendarGrid.ColumnDefinitions.Clear();

            // Create 7 columns (days of week)
            for (int i = 0; i < 7; i++)
            {
                CalendarGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            // Create 6 rows (weeks)
            for (int i = 0; i < 6; i++)
            {
                CalendarGrid.RowDefinitions.Add(new RowDefinition());
            }

            // Get events for the current month
            var eventsForMonth = _eventService.GetEventsForMonth(_currentMonth.Year, _currentMonth.Month);

            // Get the first day of the month
            var firstDayOfMonth = new DateTime(_currentMonth.Year, _currentMonth.Month, 1);
            var startDayOfWeek = (int)firstDayOfMonth.DayOfWeek; // 0 = Sunday

            // Get days in current month
            var daysInMonth = DateTime.DaysInMonth(_currentMonth.Year, _currentMonth.Month);

            // Get days in previous month
            var previousMonth = _currentMonth.AddMonths(-1);
            var daysInPreviousMonth = DateTime.DaysInMonth(previousMonth.Year, previousMonth.Month);

            int dayCounter = 1;
            int nextMonthDay = 1;

            // Generate 6 weeks of days (42 day cells)
            for (int row = 0; row < 6; row++)
            {
                for (int col = 0; col < 7; col++)
                {
                    int cellIndex = row * 7 + col;
                    DateTime cellDate;
                    bool isCurrentMonth;

                    if (cellIndex < startDayOfWeek)
                    {
                        // Previous month days
                        int day = daysInPreviousMonth - startDayOfWeek + cellIndex + 1;
                        cellDate = new DateTime(previousMonth.Year, previousMonth.Month, day);
                        isCurrentMonth = false;
                    }
                    else if (dayCounter <= daysInMonth)
                    {
                        // Current month days
                        cellDate = new DateTime(_currentMonth.Year, _currentMonth.Month, dayCounter);
                        isCurrentMonth = true;
                        dayCounter++;
                    }
                    else
                    {
                        // Next month days
                        var nextMonth = _currentMonth.AddMonths(1);
                        cellDate = new DateTime(nextMonth.Year, nextMonth.Month, nextMonthDay);
                        isCurrentMonth = false;
                        nextMonthDay++;
                    }

                    // Check if this date has events
                    var hasEvents = eventsForMonth.ContainsKey(cellDate.Date);
                    var eventsOnDate = hasEvents ? eventsForMonth[cellDate.Date] : null;

                    // Create day cell
                    var dayCell = CreateDayCell(cellDate, isCurrentMonth, eventsOnDate);
                    Grid.SetRow(dayCell, row);
                    Grid.SetColumn(dayCell, col);
                    CalendarGrid.Children.Add(dayCell);
                }
            }
        }

        private Border CreateDayCell(DateTime date, bool isCurrentMonth, List<Models.CalendarEvent>? eventsOnDate)
        {
            var border = new Border
            {
                BorderBrush = new Microsoft.UI.Xaml.Media.SolidColorBrush(
                    Microsoft.UI.Colors.Gray),
                BorderThickness = new Thickness(0.5),
                Padding = new Thickness(6),
                MinHeight = 60
            };

            var grid = new Grid();

            // Day number
            var dayText = new TextBlock
            {
                Text = date.Day.ToString(),
                FontSize = 16,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = isCurrentMonth
                    ? new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White)
                    : new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Gray)
            };

            // Highlight today's date
            if (date.Date == DateTime.Now.Date)
            {
                dayText.FontWeight = Microsoft.UI.Text.FontWeights.Bold;
                border.Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(
                    Microsoft.UI.ColorHelper.FromArgb(40, 255, 255, 255));
            }

            grid.Children.Add(dayText);

            // Add event indicator circle if there are events on this date
            if (eventsOnDate != null && eventsOnDate.Count > 0)
            {
                var eventCircle = new Microsoft.UI.Xaml.Shapes.Ellipse
                {
                    Width = 40,
                    Height = 40,
                    Stroke = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.DeepSkyBlue),
                    StrokeThickness = 2,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                grid.Children.Add(eventCircle);

                // Create flyout with event list (supports interactive content)
                var flyoutContent = new StackPanel
                {
                    Spacing = 10,
                    MinWidth = 240,
                    Padding = new Thickness(8)
                };

                foreach (var evt in eventsOnDate)
                {
                    var eventRow = new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Spacing = 8
                    };

                    var eventName = new TextBlock
                    {
                        Text = evt.EventName,
                        FontSize = 14,
                        VerticalAlignment = VerticalAlignment.Center,
                        TextWrapping = TextWrapping.Wrap,
                        MaxWidth = 180
                    };

                    var deleteButton = new Button
                    {
                        Content = "✕",
                        FontSize = 14,
                        Width = 28,
                        Height = 28,
                        Padding = new Thickness(0),
                        Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Red),
                        Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White),
                        Tag = new Tuple<int, DateTime>(evt.Id, date),
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    deleteButton.Click += DeleteEvent_Click;

                    eventRow.Children.Add(eventName);
                    eventRow.Children.Add(deleteButton);
                    flyoutContent.Children.Add(eventRow);
                }

                var flyout = new Flyout
                {
                    Content = flyoutContent
                };

                // Show flyout on pointer entered (hover)
                border.PointerEntered += (s, e) =>
                {
                    flyout.ShowAt(border);
                };

                // Also allow click to show
                border.Tapped += (s, e) =>
                {
                    flyout.ShowAt(border);
                };
            }

            border.Child = grid;
            border.Tag = date; // Store date for later use

            return border;
        }

        private async void DeleteEvent_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var data = (Tuple<int, DateTime>)button.Tag;
            int eventId = data.Item1;
            DateTime date = data.Item2;

            // For now, show a simple dialog asking to delete single or all
            var dialog = new ContentDialog
            {
                Title = "Delete Event",
                Content = "Delete only this occurrence or all occurrences?",
                PrimaryButtonText = "Delete This",
                SecondaryButtonText = "Delete All",
                CloseButtonText = "Cancel",
                XamlRoot = this.Content.XamlRoot
            };

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                // Delete single occurrence
                _eventService.DeleteSingleOccurrence(eventId, date);
                UpdateCalendarDisplay();
            }
            else if (result == ContentDialogResult.Secondary)
            {
                // Delete all occurrences
                _eventService.DeleteEvent(eventId);
                UpdateCalendarDisplay();
            }
        }

        private void PreviousMonth_Click(object sender, RoutedEventArgs e)
        {
            _currentMonth = _currentMonth.AddMonths(-1);
            UpdateCalendarDisplay();
        }

        private void NextMonth_Click(object sender, RoutedEventArgs e)
        {
            _currentMonth = _currentMonth.AddMonths(1);
            UpdateCalendarDisplay();
        }

        private async void AddEvent_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Dialogs.AddEventDialog
            {
                XamlRoot = this.Content.XamlRoot
            };

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary && dialog.NewEvent != null)
            {
                _eventService.AddEvent(dialog.NewEvent);
                UpdateCalendarDisplay();
            }
        }

        public void SetSystemTrayHelper(SystemTrayHelper helper)
        {
            _systemTrayHelper = helper;
        }

        private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
        {
            if (args.WindowActivationState == WindowActivationState.Deactivated && _isVisible)
            {
                // Hide window when it loses focus
                Hide();
            }
        }

        private void MainWindow_Closed(object sender, WindowEventArgs args)
        {
            // Prevent window from closing, just hide it instead
            args.Handled = true;
            Hide();
        }
    }
}
