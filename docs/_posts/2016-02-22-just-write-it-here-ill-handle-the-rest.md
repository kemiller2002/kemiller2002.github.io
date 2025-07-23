---
layout: post
title: "Just write it here, I'll handle the rest"
date: 2016-02-22
---

It's pretty common knowledge in a .NET console application using the following command will produce the following result. 

```
Console.WriteLine("I am a console message.");
```

<img src="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/NewConsoleOutput/Images/BasicConsoleOutput.png" alt="Console output" />

On the surface, this doesn't look like it's that helpful.  Console applications can immediately output a message, but most applications don't run in the console, and those that do, run in a process not visible to the user.  However, <strong>System.Console</strong> has a method which makes it ideal for logging: <a href="https://msdn.microsoft.com/en-us/library/system.console.setout.aspx" target="_blank">Console.SetOut</a>.  Passing in a <strong>TextWriter</strong> object changes the system's default output behavior and pushes the entries to a different location.  

The following example outputs the contents to "C:\temp\logfile.txt" instead of the default console itself.

```

using (StreamWriter writer = File.CreateText("C:\\temp\\logfile.txt"))
{
  Console.SetOut(writer);

  Console.WriteLine("This is a logged message to the file.");
}

```


<img src="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/NewConsoleOutput/Images/LogToTextFile.png" alt="Text output" />

This allows the application to change the destination based on the need, and the destination can be anywhere.  <img src="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/NewConsoleOutput/Images/overrride.png" alt="Override" align="right" style="padding-left:4px;border-color:#ffffff" />The output class must inherit the <a href="https://msdn.microsoft.com/en-us/library/system.io.textwriter(v=vs.110).aspx" target="_blank">TextWriter</a> class, but this doesn't mean that it has to write to a file, screen, or something which ultimately outputs to a stream.  Override the appropriate methods in the <strong>TextWriter</strong> and handle the data in any manner necessary.  (Here are two examples: 1. <a href="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/NewConsoleOutput/ConsoleOutputExample/XmlFileWriter.cs" target="_blank">XmlLogWriter</a>, 2. <a href="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/NewConsoleOutput/ConsoleOutputExample/DbWriter.cs" target="_blank">DbLogWriter</a>)
<br/>

The real advantage comes when writing larger applications where the solution consists of multiple projects requiring a way to communicate information about the system.  When using a standard logging framework, every project must have knowledge of the framework to use it.  This becomes cumbersome if core projects are used across multiple solutions, or if you want to change the logging framework at some point in time in the future.  <strong>System.Console</strong> is available to all .NET assemblies by default, and by using it (and <strong>Console.Error.Write(Line)</strong>), the system components do not need to reference anything additional.

Along with this, the <strong>Console</strong> allows <strong>SetOut</strong> to be called at any time in the application's lifetime, so it's possible to change the output stream while the program is executing.  This allows for the output to be changed on the fly should the need arise.

```

Console.WriteLine("I am a console message.");
//If you want to change back to the primary output at some point in time, 
//it's easier to save it for later.
var primaryOut = Console.Out;

using (var writer = File.CreateText("C:\\temp\\logfile.txt"))
{
    Console.SetOut(writer);

    Console.WriteLine("This is a logged message to the file.");

    Console.SetError(writer);
}

Console.SetOut(primaryOut);

Console.WriteLine("This is logged to the console again.");

Console.ReadLine();

```