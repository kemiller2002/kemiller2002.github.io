---
layout: post
title: "Are you finally going to do something about this?"
date: 2014-12-01 00:00:00 -0500
---
There is often a lot of questions about the execution of finally statements in C#.  It is a common assumption that finally blocks reliably execute, but even in the <a href="http://msdn.microsoft.com/en-us/library/zwc8s4fz.aspx" title="C# Exception" target="_blank">C# Reference concerning the finally</a> block it explicitly notes this is incorrect.  

<div class="blockQuote">Within a handled exception, the associated finally block is guaranteed to be run. However, if the exception is unhandled, execution of the finally block is dependent on how the exception unwind operation is triggered. 
</div>

So what does this mean?  In the event of an exception not getting caught, the finally block executing isn't guaranteed.  How does the following piece of code react?

<pre class="prettyprint linenums:1">
static void Main(string[] args)
{
  try
  {
    throw new Exception("This is a normal exception.");
  }
  finally
  {
    Console.WriteLine("Written");
  }
}

```


The finally block executes, because the exception process is still being handled by the CLR and the exception information is passed to the underlying operating system.....in most scenarios. The instances where this finally example doesn't execute is not the fault of the application, but the operating system and the user.  

<video controls>
  <source src="https://github.com/kemiller2002/StructuredSight/blob/master/FinallyGoingToGetThis/ShowFinallyNotExecute.mp4?raw=true" type="video/mp4">
</video>

When the debugger isn't attached, and the system allows the program to continue its execution path until completion, the finally block executes. With the prompt from the operating system to attach the debugger and the user's intervention, interaction subverts the application's ability to run it.  Furthermore, any abnormally terminated program has the same issue (.NET applications don't terminate by the operating system issuing a shutdown command and allowing the application to finish cleanup tasks).  Any application killed in Task Manager, power failure, etc. won't execute the finally block.  

Outside of this, there are two other scenarios where a finally batch won't execute (there could be other scenarios where it is prevented also, but these two absolutely won't allow it).  The first is during a <a href="http://en.wikipedia.org/wiki/Stack_overflow" title="Stack Overflow" target="_blank">Stack Overflow Exception</a>.    The program's stack gets corrupted and the application, or thread, crashes.  

```

static void MakeStackOverflow()
{
  MakeStackOverflow();
}

```


There is nothing that will prevent an overflow from corrupting the stack, and the most you can do is have windows generate a dump file on crash and go through it to find the issue. (Set the following registry key to produce a dump file: <a href="http://msdn.microsoft.com/en-us/library/windows/desktop/bb787181%28v=vs.85%29.aspx" title="Local Dump Registry Key" target="_blank">HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\Windows Error Reporting\LocalDumps</a>)

The second is calling <a href="http://msdn.microsoft.com/en-us/library/ms131100%28v=vs.110%29.aspx" title="Fail Fast" target="_blank">Environment.FailFast</a>.  This creates a dump of the application and terminates the process.  The concerning part about this is that there is no restriction on its use.  If a third part assembly accidentally invokes it (or does it on purpose for stop a security breach), the host application will close with no hope of recovery.  The only upside to this is it creates a dump file and logs information to the Windows Event Log, so it can be tracked down if that appears to be the case.

Outside of the examples where the finally block doesn't execute, a great way of catching most unhandled exceptions in code is by adding an event listener to the <strong>UnhandledExceptionEvent</strong>.  This will at least allow one last chance at logging if for some reason an exception wasn't caught.


```

AppDomain.CurrentDomain.UnhandledException += 
  new UnhandledExceptionEventHandler((x, y) =>
    {
      Console.WriteLine("Unhandled: " + ((Exception)y.ExceptionObject).Message);
    });

```


Assuming the finally statement will run is a bad practice, but depending on what executes in the finally block, it not may not be an issue.  If the application crashes before executing the code to clean up resources or log an event, this probably isn't a problem.  It however can be one if the finally block is used to manually reset state (like an account status of paying an invoice) used by other applications.