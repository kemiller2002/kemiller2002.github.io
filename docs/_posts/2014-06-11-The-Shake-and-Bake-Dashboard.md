---
layout: post
title: "The Shake and Bake Dashboard"
date: 2014-06-11 00:00:00 -0500
---
A few weeks ago, someone asked me if it is possible to create a server dashboard (real time processor usage, available memory etc.) using web technologies.  Quickly thinking about it, I thought "sure" how hard could it be, but there are several pieces needed to make it functional and meaningful.  

<ol>
        <li>A way to send the data to clients.  (You could have each page poll a web server over and over, but that creates a lot of overhead for little benefit.)</li>
<li>A way to retrieve the server data on at regular intervals.</li>
        <li>A way to meaningfully present the data.</li>
</ol>

<h2>Transmitting Data to the Client</h2>
A short time ago, Microsoft released <a href="http://signalr.net/" title="Signal R" target="_blank">SignalR 2.0</a> and made it incredibly easy to install and use in projects.  It is a method of using <a href="http://en.wikipedia.org/wiki/WebSocket" title="WebSockets" target="_blank">WebSockets</a> to create a single open TCP connection to frequently transmit data between the browser and the server.  Browsers can send data without opening a new HTTP request, and servers can push new data to the client when needed. It's fast, efficient and removes the requirement for the client to continuously poll for updates. 

The <a href="http://www.asp.net/signalr" title="Documentation and Tutorials" target="_blank">documentation and tutorials</a> are easy to follow and walk you through the process of setting up a test project to teach the basics.  The only issue I found with installing it in my own project was making sure the JQuery bundle referenced in the <strong>_Layout.cshtml</strong> was moved to execute before the <strong>RenderBody</strong> method.  

<h2>Polling Data</h2>
ASP.NET code is typically an on demand only process.  A client must get or post data to the server in order for the server side code to execute.  Once the request ends, the objects in that process are disposed/readied to be garbage collected and makes having an in memory object which makes updates at regular intervals problematic.  There are ways around this, such as having a process ping the web server at regular intervals, but this made it more complicated than I wanted as I would have to create another application to communicate to the web server to trigger the update actions.  

While searching for a solution, I discovered <a href="http://msdn.microsoft.com/en-us/library/system.web.hosting.iregisteredobject.aspx" title="IRegisteredObject" target="_blank">IRegisteredObject</a> which allows you to use the <a href="http://msdn.microsoft.com/en-us/library/system.web.hosting.hostingenvironment.registerobject.aspx" title="Register Object" target="_blank">HostingEnvironment.RegisterObject</a> to keep an object in web application's memory. After that I created a <a href="http://msdn.microsoft.com/en-us/library/system.threading.timer(v=vs.110).aspx" title="Timer" target="_blank">Timer</a> object to gather information about the current process and send it through SignalR on regular intervals. 

```

public void Start()
{
  HostingEnvironment.RegisterObject(this);

  _timer = new Timer(OnTimerElapsed, null,
  TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
}

private void OnTimerElapsed(object sender)
{
  var number = ProcessorMonitor.GetProcessorMonitor().ProcessorUsage;
  ProcessorHub.SendProcessorUpdate(number);
}

```


<h2>Displaying Data</h2>
Displaying the chart was the easiest part of the process.  Google has an excellent <a href="https://developers.google.com/chart/" title="Google Charts" target="_blank">charting</a> module in JavaScript.  It's powerful, easy to use, and fast which I needed to quickly refresh the graph when new data came from the server. 

<img src="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/RealTimeDashboard/ProcessorSparkline.jpg" alt="Sparkline" />

Getting the module from Google is extremely easy.  Add the following tag to the page: 

```

&lt;script type="text/javascript" src="https://www.google.com/jsapi"&gt;&lt;/script&gt;

```


To create a basic <a href="http://en.wikipedia.org/wiki/Sparkline" title="Sparkline" target="_blank">sparkline</a> that refreshes from a SignalR update only took the following code to allow the graphing to start as soon as the paged finished loading: 

```

google.load("visualization", "1", { packages: ["imagesparkline"] });

$(function () {
  var dataItems = [['Processor Percentage']];
  
    var drawChart = function () {
  
      var data = google.visualization.arrayToDataTable(dataItems);

      var chart = new google.visualization.ImageSparkLine(
        document.getElementById('chart_div')
      );
  
      chart.draw(data, 
        { 
          width: 400, height: 200, showAxisLines: false, 
          showValueLabels: false, labelPosition: 'left' 
        }
    );
  }

  var processorNotification = $.connection.processorHub;
  $.connection.hub.logging = true;
  
  processorNotification.client.broadcastProcessorStats = function (percentage) {
    dataItems.push([percentage]);
    drawChart();
  };

  $.connection.hub.start();

    });

```


All the code for the project can be found <a href="https://github.com/kemiller2002/StructuredSight/tree/master/RealTimeDashboard/Dashboard" target="_blank">here</a>.