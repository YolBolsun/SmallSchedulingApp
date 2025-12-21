Start Initial Design
This project is a calendar that will live in the taskbar on windows beside the default calendar.
When selected, the calendar view will pop up which shows the current month with buttons to move to the next and previous month. It should have a similar look and feel to the default windows calendar.
In the calendar, we will show events.
	- The dates with events will be circled
	- hovering over a date with an event will expand the day to show a list of events where each should have a small red x to be able to delete that event which will ask the user if they want to delete just that occurrence or all occurrences 
There will also be an add event button which will open an add event view. 
Beside the add event button will be an explore event button which will be nonfunctional for now.

The add event view allows the user to specify 
- event name
- event start date
- frequency (default daily)
	- Daily 
	- Weekly 
	- Bi-weekly
	- Monthly
- event occurences (default 1) (The number of times to repeat the event, with the default being a one time event
)

End initial design

Initial Message:

The core concept of this project is a calendar that sits in the taskbar for the user, on opening the calendar it will look like a normal calendar from showing 2 weeks ago up until 1 month ahead for a total of 6 weeks at a time by default. at the top of the calendar there will be a small button to "Add Event", when that button is clicked the user is given a simple interface. This interface will have fields for the user to fill out as follows: Event Name (Text), Event Start Date (Date), Frequency (This is a drop down for Single, Daily, Weekly, Bi-weekly, and Monthly), and Count (This is an integer when combined with the frequency and event start date should set up events accordingly). An example is as follows: Event Name: South Park, Event Start Date: 12/3/2025, Frequency: Weekly, Count: 4. After clicking Add Event the calendar will be updated to reflect that 12/3/2025, 12/10/2025, 12/17/2025, 12/24/2025 will have their dates circled and if the user hovers a circled date it will display the Event Name text for all events on that day.

Additionally there should be a button next to Add Event called "Explore Events" when clicked it will open a new window, this window is meant for a future version which will take information that is originally compiled through various API calls by the owner of the application and not the user, uploaded to a github repo likely in a CSV which this application will point to for filling in information on the explore page. On the explore page, there should be simple filters at the top (Movie, Show, Anime, and potentially more to be added), in the explore window there should be a small title card for the Name, image, and summary, it should be compact and if the summary is long it should be able to scroll down to not let one event take over the whole window. When a user clicks on the event it is enlarged and a button at the bottom offers to "Add Event" which will fill their calendar with relevant information from the CSV from that event. 
The title card should display an image if available and limit the size up to 200px by 200px that way the Explore Events section can be uniform on a grid as users scroll through events that occurred in the with a Event start date of 2 weeks ago to any future events. 

In the event that a user would like to remove an event from their calendar, there should be a button at the top of the calendar called "Remove Event", when clicked a small window should pop up displaying a list of names for events added to the user's calendar. The user should be able to click one or many names in that list, and on the bottom of that window is a button "Remove Event(s)" this will remove all their occurrences from the calendar, and remove them from that list. The list should only show events from three months prior and any future planned events in the user's calendar.

This application should also look to a specific github project repository for updates for the application.
This application should be in dark mode.

For testing the Explore Events the CSV content would be Name, Summary, Image, Start Date, Frequency, Count, Tag1, Tag2, Tag3 if the tags are available. An example as as followed and should be filled in for testing Explore Events: 

The Mighty Nein, "The Mighty Nein follows a group of misfits with troubled pasts and secrets who find themselves drawn together by circumstance. They become entangled in a larger conflict and "must work together to save the realm" after a "powerful arcane relic known as The Beacon falls into dangerous hands".", https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcRb4dZJrjCFzcLCR7tEOD49QKmQIkL8XwPVa3PhX5z-YN2tNw4OFSC30iohzUHLkl8nbKK3&s=10, 11/19/2025, Weekly, 7, Show, , 
