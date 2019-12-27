# manual-time-logger

## Purpose

Providing an extremely easy and accessible way to keep track of your spend time (windows only).

## How

* Input form is always visible at the bottom right corner of the screen; you are automatically reminded to administrate some spend time.
* Use of intuitive markers to denote the purpose of the text entered after the marker for quick administration.
* Input is stored in a simple .csv file (C:\temp\timelogs) which allows for easy reporting and manual manipulations.

## Use

### Markers

* (required) "\*" followed by the number of hours. Decimal separator can be either "." or ","
* (required) "$" followed by the description of what you did
* (optional) "\#" followed by issue number
* (optional) "@" followed by label

### Examples

* @development 2.0 $csv file repository \#123 \*2
* $UX testing \*1,5 @test
* \*.5 $writing read me
