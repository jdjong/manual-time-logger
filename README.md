# Time Logger App

## Purpose

Providing an extremely easy and accessible way to keep track of your spend time (windows only).

## How

* Input form is always visible at the bottom right corner of the screen; you are automatically reminded to administrate some spend time
* Use of intuitive markers to denote the purpose of the text entered after the marker for quick administration
* Input is stored in a simple .csv file (C:\temp\timelogs) which allows for easy reporting and manual manipulations

## Use

### Run

Run the program by building it in Visual Studio for example.

### Configure

* Base path for the time logs
* Use auto fill for label and activity yes/no
* The label and activity presets
* Your name which will be included in the time log's file name
* The possible account/customer you are logging for

### Markers

* (required) start of the input needs to be one of the configured accounts/customers (when input is empty, use 1 to 5 as hotkeys to enter the first five configured values)
* (required) "\*" followed by the number of hours. Decimal separator can be either "." or ",". You can also enter a time span in the format "hh:mm"
* (required) "$" followed by the description of what you did
* (optional) "\#" followed by issue number
* (optional) "@" followed by label
* (optional) "!" followed by activity

### Examples

* testaccount @newfunctionality !development $csv file repository \#123 \*2
* customer1 \*2 $csv file repository \#123 @newfunctionality !development
* customer1 $Preference screen testing \*1,5 @uximprovements
* testaccount2 $writing read me\*.5

# Report Builder App

## Purpose

Automatically create reports for spend time aggregated by:

* engineer, issue, day
* engineer, label, day
* engineer, activity, day
* issue, day
* label, day
* activity, day

## How

Reading all the time logs from a folder and producing the desired report files (.csv).

## Use

### Run

Run the program by building it in Visual Studio for example. The following parameters can be passed in:

* -w yyyyMMdd (generates the reports for 1 week starting at monday of the week for the passed in date)
* -m yyyyMMdd (generates the reports for 1 month starting at the first day of the month for the passed in date)

### Configure

* Base path for the time logs
* Base path for the reports
