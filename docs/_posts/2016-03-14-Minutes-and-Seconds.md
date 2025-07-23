---
layout: post
title: "Minutes and Seconds"
date: 2016-03-14 00:00:00 -0500
---
When dealing with dates in .NET, most applications use the <a href="https://msdn.microsoft.com/en-us/library/system.datetime(v=vs.110).aspx" target="_blank">System.DateTime</a> struct to store and manipulate dates.  It has a natural conversion from <a href="https://msdn.microsoft.com/en-us/library/ms187819.aspx" target="_blank">MSSQL Server's DateTime</a> datatype, and has little difficulty in translating into different data formats (JSON, XML, etc.)

```

DateTime modifiedDate = (DateTime)reader["modifiedDate"];

```


For a while now JSON has been a standard in communicating between applications, web clients and servers, to and from database and so on, and serializing a DateTime object looks like this converting it into the <a href="https://en.wikipedia.org/wiki/ISO_8601" target="_blank">ISO 8601</a> format: 

```

var dateTime = new DateTime(1923, 3, 20);
var jsonDateTime = 
  Newtonsoft.Json.JsonConvert.SerializeObject(dateTime);
...
"1923-03-20T00:00:00"

```


and converting it back works exactly as expected: 

```

var deserializedDateTime = 
(DateTime)Newtonsoft.Json.JsonConvert.
DeserializeObject(jsonDateTime);


Console.WriteLine(deserializedDateTime.ToString());
...
3/20/1923 12:00:00 AM

```


Serializing the date and parsing it with JavaScript on some browsers (such as Chrome for Mac and Windows, and Safari) yields different results. The browser has moved the date 4 hours into the past, because JavaScript has assumed the time it parsed was in <a href="https://en.wikipedia.org/wiki/Coordinated_Universal_Time" target="_blank">UTC</a>.


```

var d = new Date("1923-03-20T00:00:00");
console.log(d);
....
Mon Mar 19 1923 20:00:00 GMT-0400 (EDT)

```


Secondly, JavaScript based its interpretation off of an incorrect timezone assumption.  In 1923, the majority of the United States stayed on standard time the entire year, and those that moved their clocks forward did so on <a href="http://www.timeanddate.com/time/change/usa/new-york?year=1923" target="_blank">April 29</a>.  Daylight Savings Time was not in effect, and throughout history, the use of it, and when it was applied, has varied. Even if the passed in date was meant to be in UTC, the conversion should have made it **Mon March 19, 1923  19:00:00 GMT -0500 (EST)**.

This also means when serializing the date to JSON and sending it back to the server, it sends it as it's assumed UTC date.  


```

"1923-03-20T00:00:00.000Z"
...

var dateTime = DateTime.Parse("1923-03-20T00:00:00.000Z");
Console.WriteLine(dateTime);

...
3/19/1923 7:00:00 PM

```

At first glance, this looks correct, but changing the date to <strong>1923-05-20T00:00:00.000Z</strong> (adding the Z at the end, indicating the date is in UTC) yields <strong>3/19/1923 7:00:00 PM</strong> showing that the .NET <strong>DateTime</strong> object is trying to apply the Time Zone Offset.  Now the question is, "Why is .NET deserializing the date as 3/19/1923 7:00:00 PM and not 3/19/1923 8:00:00 PM like JavaScript does especially since the serialized date came from JavaScript after it interpreted the original date as UTC?"  The answer is because JavaScript only interpreted the date as UTC and displayed it as the timezone the machine is set in.  The date it had didn't change, so when it serialized it to the ISO 8601 format, JavaScript only appended the Z thinking the date was already in UTC.

Sending the <a href="https://en.wikipedia.org/wiki/List_of_UTC_time_offsets" target="_blank">Time Zone Offset</a> when serializing from .NET, fixes the inconsistencies in some instances, but not all of them.  


```

var dateTime = new DateTime(1983, 3,12);
var offset = new DateTimeOffset(dateTime);
var output = Newtonsoft.Json.JsonConvert.SerializeObject(offset);

```

Since the time zone wasn't specified, .NET assumes it is the local time of the server and produces <strong>2016-03-11T19:00:00-05:00</strong>. Loading that string into a JavaScript <a href="https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Date" target="_blank">Date</a> object produces the same results although in UTC: 

```

var d = new Date("2016-03-11T19:00:00-05:00")

console.log(d.toISOString())

...

2016-03-12T00:00:00.000Z

```


One of the issues with creating DateTimes in .NET and not explicitly setting the timezone is that it can make incorrect assumptions.  Setting the timezone to Indiana (East) and parsing the following date <strong>1989-05-12T00:00:00.000Z</strong> yields <strong>1989-05-11T20:00:00-04:00</strong> showing the UTC conversion to Eastern Daylight Time.  Indiana didn't follow Daylight Savings Time until 2006, meaning the conversion is incorrect.  (Even if it did do the conversion correctly, setting the **Set Timezone Automatically** flag in Windows 10 forces it to be **Eastern Time**)

Explicitly setting the timezone using the <a href="https://msdn.microsoft.com/en-us/library/system.datetimeoffset(v=vs.110).aspx" target="_blank">DateTimeOffset</a> corrects the problem in .NET, but it doesn't guarantee correctness across systems.  


```

var dateTime = DateTime.Parse("2000-05-12");
var offset = new DateTimeOffset(dateTime, new TimeSpan(-5,0,0));
var jObject = Newtonsoft.Json.JsonConvert.SerializeObject(offset);

Console.WriteLine(jObject);

...

2000-05-12T00:00:00-05:00

```


When the browser parses the date with the correct offset, it attempts to display the date with the offset it thinks it should be. 


```

var d = new Date("2000-05-12T00:00:00-05:00")
console.log(d);
...
Fri May 12 2000 01:00:00 GMT-0400 (Eastern Daylight Time)

```


The data in the Date object is still correct, and exporting it will not alter it in any way.  Its attempt to display the date in a local time is the issue.  In the United States, the only dates that are affected are prior to 2007, because that is when the last time a change to the timezones occurred. (The start of Daylight Savings Time was moved ahead to the second Sunday in March, and then end extended to the first Sunday in November.  If you load a date from the year 2004 into JavaScript in Chrome, the date will incorrectly show for all dates between March 14 and April 4.)  

The real solution to handling dates is to use libraries specifically designed to handle timezones and Daylight Savings Time (<a href="http://momentjs.com/" target="_blank">MomentJs</a> for JavaScript and <a href="http://nodatime.org/" target="_blank">NodaTime</a> for .NET). These significantly reduce the inconsistencies found in using the built in date and time conversions found in .NET and in Browsers, but even these can have mistakes in them.