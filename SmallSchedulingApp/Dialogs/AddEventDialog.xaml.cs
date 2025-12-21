using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SmallSchedulingApp.Models;

namespace SmallSchedulingApp.Dialogs
{
    public sealed partial class AddEventDialog : ContentDialog
    {
        public CalendarEvent? NewEvent { get; private set; }
        public DateTimeOffset DefaultDate { get; set; }

        public AddEventDialog()
        {
            this.InitializeComponent();
            DefaultDate = DateTimeOffset.Now;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(EventNameBox.Text))
            {
                args.Cancel = true;
                return;
            }

            if (!StartDatePicker.Date.HasValue)
            {
                args.Cancel = true;
                return;
            }

            // Parse frequency
            var frequency = FrequencyCombo.SelectedIndex switch
            {
                0 => EventFrequency.Daily,
                1 => EventFrequency.Weekly,
                2 => EventFrequency.BiWeekly,
                3 => EventFrequency.Monthly,
                _ => EventFrequency.Daily
            };

            // Create new event
            NewEvent = new CalendarEvent
            {
                EventName = EventNameBox.Text.Trim(),
                StartDate = StartDatePicker.Date.Value.DateTime,
                Frequency = frequency,
                Occurrences = (int)OccurrencesBox.Value
            };
        }
    }
}
