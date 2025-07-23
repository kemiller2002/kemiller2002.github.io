---
layout: post
title: "Wait Wait Don't Tell Me.....On Second Thought"
date: 2014-11-24
---

<style>
.pre{}<br />
</style>Consider the following:

```
DECLARE @Counter INT  --Counter 
SET @Counter = 100  --Keep track of numbers to insert

WHILE @Counter &gt; 0 --Loop down and add number to table until 0
BEGIN
  INSERT INTO NumberTable (Number)
  (@Counter)
END

```

Ideally, the example would start at 100 and loop until the counter hits zero, but the statement to decrement the counter is missing.  It won't ever stop running, and there is no visual cue to alert there is an issue.  The most logical solution to this would be to add the <a href="http://msdn.microsoft.com/en-us/library/ms176047.aspx" title="PRINT" target="_blank" rel="noopener noreferrer">PRINT</a> statement.

```
PRINT @Counter
```

Sadly, this does not solve the issue at hand as the <strong>PRINT</strong> statement only displays output sent to it after the script finishes executing.  It's great for knowing what occurred after it's finished but provides no use for telling what is currently happening. Conveniently, there is a solution using the <a href="http://msdn.microsoft.com/en-us/library/ms178592.aspx" title="RAISERROR" target="_blank" rel="noopener noreferrer">RAISERROR</a> function.

```
DECLARE @Counter INT  --Counter 
SET @Counter = 100  --Keep track of numbers to insert

/*<a href="/2014/03/21/wheres-the-scope/" style="font-weight:600pt" title="Where's the scope" target="_blank" rel="noopener noreferrer">Declare variable outside of loop because of scope declaration</a>*/

DECLARE @Output INT  

WHILE @Counter &gt; 0 --Loop down and add number to table until 0
BEGIN
  INSERT INTO NumberTable (Number)
  (@Counter)

  SET @Output = CAST(@Counter AS NVARCHAR)

  RAISERROR(@output,0,1) WITH NOWAIT
END

```

The first parameter is the message to display, and the second and third are the **error level** and the **system state**.  RAISERROR treats anything with an **error level** from 0-10 as a message and does not affect code execution.  The **system state** ranges from -1 to 255 and anything outside of that results in an exception (and for the purposes of printing messages, this value is inconsequential).  Using <strong>RAISERROR</strong> and adding the <strong>WITH NOWAIT</strong> option allows the immediate message output describing how the script is executing as it happens.

```
open System
open System.Data.SqlClient

try 
    let connectionString = 
      "Server=localhost;Database=Scratch;Integrated Security=sspi"
    let procedure = 
      "RAISERROR ('I''m a little teapot.', 10, 0) WITH NOWAIT"

    use connection = new SqlConnection(connectionString)
    connection.InfoMessage.Add
      (fun x-&gt;printfn "This is an informational message: %s" x.Message)
    
    connection.Open()
    let command = new SqlCommand (procedure, connection)
    command.ExecuteNonQuery() |&gt; ignore
with  
     | ex  -&gt; printfn "This is an exception message: %s \n Exception Type: %s" 
                      ex.Message (ex.GetType().ToString())

```

<img src="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/WaitWaitDontTellMe/InformationalMessage.png" alt="informational message">

When the error level is raised above 10, Sql Server raises an exception:

```
 let procedure = 
      "RAISERROR ('I''m a little teapot.', 11, 0) WITH NOWAIT"

```

<img src="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/WaitWaitDontTellMe/Exception.png" alt="Error">

This works really well when executing a command from within a .NET application, but an issue arises when executing code from an external process like <a href="http://msdn.microsoft.com/en-us/library/ms162773.aspx" title="Sql Command" target="_blank" rel="noopener noreferrer">SqlCmd</a>.  When using the RAISERROR function and supplying a severity above 10, it doesn't always trigger the process's <a href="http://msdn.microsoft.com/en-us/library/system.environment.exitcode%28v=vs.110%29.aspx" title="Exit Code" target="_blank" rel="noopener noreferrer">exit code</a> (<a href="http://msdn.microsoft.com/en-us/library/system.diagnostics.process.exitcode" title="Process Exit Code">Process.ExitCode</a>) to change to something other than 0 which alerts the calling application about an issue.

```
  let proc= new System.Diagnostics.Process()
  proc.StartInfo.FileName  &lt;- 
    @"C:\Program Files\Microsoft SQL Server\110\Tools\Binn\SqlCmd.exe";
  proc.StartInfo.RedirectStandardError  &lt;- true;
  proc.StartInfo.RedirectStandardOutput &lt;- true;
  proc.StartInfo.UseShellExecute  &lt;- false;
  proc.StartInfo.Arguments  &lt;- 
    @"-S ""localhost"" -E -Q ""RAISERROR('this is a message', 11, 0) WITH LOG"""
  proc.Start() |&gt; ignore;

  let errorMessage = proc.StandardError.ReadToEnd();
  proc.WaitForExit();
  
  let outputMessage = proc.StandardOutput.ReadToEnd();
  proc.WaitForExit();

  let exitCode = proc.ExitCode

  printfn "ERROR MESSAGE: %s" errorMessage
  printfn "OUTPUT MESSAGE: %s" outputMessage
  printfn "EXIT CODE: %i" exitCode

```

<img src="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/WaitWaitDontTellMe/ExitCodeZero.png" alt="Exit code 0">

Why use SQLCmd when .NET has a built in way of executing code with <a href="http://msdn.microsoft.com/en-us/library/system.data.sqlclient.sqlcommand%28v=vs.110%29.aspx" title="Sql Command">SqlCommand</a>?  The SqlCommand object does not allow the use of the batch seperator: <a href="http://msdn.microsoft.com/en-us/library/ms188037.aspx" title="GO" target="_blank" rel="noopener noreferrer">GO</a>.  For applications doing Create/Read/Update/Delete (CRUD) operations, not using the <strong>GO</strong> statement isn't a problem, but it is important to applications which perform maintenance on databases.  Certain SQL execution statementslike <a href="http://msdn.microsoft.com/en-us/library/ms187926.aspx" title="Create Procedure" target="_blank" rel="noopener noreferrer">CREATE PROCEDURE</a>, must be the first item in a batch meaning chaining multiple statements together in one execution is impossible without using <strong>GO</strong>.

Fortunately, there is an easy solution to this problem.  Change the error level to 20 - 25 or greater and add the <strong>WITH LOG</strong> option (values greater than 25 are interpreted as 25).  This will close the connection to Sql Server and force applications like SqlCmd to return an ExitCode that is something other than 0.  Unfortunately to set a severity level between 19-25, the account running the script must either be a member of the <a href="http://msdn.microsoft.com/en-us/library/ms188659.aspx" title="Sql Server Roles" target="_blank" rel="noopener noreferrer">sysadmin</a> role or have the <a href="http://msdn.microsoft.com/en-us/library/cc293611.aspx" title="Alter trace" target="_blank" rel="noopener noreferrer">Alter Trace</a> permission.

<img src="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/WaitWaitDontTellMe/ExitCodeNotZero.png" alt="Exit Code Not Zero">

It is possible to look at the output and error messages and attempt to parse them and determine if an error occurred instead, but this is error prone and not the standard method for checking for system exceptions.  It may work for applications specifically written for this scenario, but applications written to interact with others in the Windows ecosystem look at the exit code to determine process success or failure making setting it to the correct value vital when interacting with different systems.