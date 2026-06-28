---
layout: post
title: "Pushing Data"
date: 2016-02-29 00:00:00 -0500
---
Consider the following two pieces of code: 

```

public IEnumerable<string> EnumerateWithStrings()
{
  using (var connection = new 
    SqlConnection (
    "Data Source=localhost;Initial Catalog=AdventureWorks2014;
        IntegratedSecurity=SSPI")
    )
  using (var command = connection.CreateCommand())
  {
    connection.Open();

    command.CommandText = "SELECT FirstName FROM Person.Person";

    var reader = command.ExecuteReader();

    while (reader.Read())
    {
      yield return reader["FirstName"].ToString();
    }
  }
}

```

and

```

public IEnumerable<string> InjectSqlConnection()
{   
  using (var connection = new SqlConnection())
  {
    connection.ConnectionString = 
      "Data Source=localhost;Initial Catalog=AdventureWorks2014;
        Integrated Security=SSPI";
    
    connection.Open();  

    return GetNamesWithConnection(connection);
  }
}
public IEnumerable<string> GetNamesWithConnection(SqlConnection connection)
{
   using (var command = connection.CreateCommand())
   {
      connection.Open();
      command.CommandText = "SELECT FirstName FROM Person.Person";

      var reader = command.ExecuteReader();

      while (reader.Read())
      {
        yield return reader["FirstName"].ToString();
      }
   }
}

```


Although they look like they are roughly the same, they produce two very different results.  The first returns an enumeration of <strong>string</strong> and the second throw's an <strong>InvalidOperationException</strong>.

The difference stems from how C# handles the <a href="https://msdn.microsoft.com/en-us/library/9k7k7cf0.aspx" target="_blank">yield</a> statement.  Both methods promise to return an object which implements <strong>IEnumerable&lt;string&gt;</strong>.  In .NET 1 and 1.1, there was no <strong>yield</strong> statement, and to return a custom object (not an array) which implemented it, you had to create an object which satisfied the <a href="https://msdn.microsoft.com/en-us/library/system.collections.ienumerable(v=vs.110).aspx" target="_blank">IEnumerable</a> requirements. (Remember it's not generic, because it was created before generics were added.)

When it was added in .NET 2.0, in order to satisfy the requirements of returning an object, the C# compiler turns the method which uses <strong>yield</strong> into an object with the <a href="https://msdn.microsoft.com/en-us/library/9eekhta0(v=vs.110).aspx">IEnumerable&lt;T&gt;</a> interface.  

```

[CompilerGenerated]
private sealed class <EnumerateWithStrings>d__2 : IEnumerable<string>, IEnumerable, IEnumerator<string>, IDisposable, IEnumerator
{
    // Fields
  private int <>1__state;
  private string <>2__current;
  private int <>l__initialThreadId;
  private SqlCommand<command>5__2;
  private SqlConnection<connection>5__3;
  private SqlDataReader<reader>5__1;

.....
public <EnumerateWithStrings>d__2(int <>1__state)
{
  this.<> 1__state = <> 1__state;
  this.<> l__initialThreadId = Environment.CurrentManagedThreadId;
}

private void <>m__Finally1()
{
  this.<> 1__state = -1;
  if (this.< connection > 5__3 != null)
  {
    this.< connection > 5__3.Dispose();
  }
}

```


(<a href="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/RegisterForDispose/IEnumerableConversion.cs" target="_blank">Entire generated class</a>).

The important thing to note is that the generated class implements <a href="https://msdn.microsoft.com/en-us/library/system.idisposable(v=vs.110).aspx" target="_blank">IDispoable</a>, and it is responsible for disposing of the <a href="https://msdn.microsoft.com/en-us/library/system.data.sqlclient.sqlconnection(v=vs.110).aspx" target="_blank">SqlConnection</a> and <a href="https://msdn.microsoft.com/en-us/library/system.data.sqlclient.sqlcommand(v=vs.110).aspx" target="_blank">SqlCommand</a>, not the original method.  This means that as the <strong>IEnumerable&lt;string&gt;</strong> object returns through the calling methods, nothing will dispose them until the enumeration object itself is disposed.  The other method does not do this, because the <strong>SqlConnection</strong> is injected and the using statement is outside of the <strong>GetNamesWithConnection's</strong> control (and hence it is not included in the generated class the <strong>GetNamesWithConnection</strong> converts to).  Once the enumerator returns from the method responsible for disposing the <strong>SqlConnection</strong> (in the above example <strong>InjectSqlConnection</strong>), the using statement's scope exits and the <strong>SqlConnection's</strong> <a href="https://msdn.microsoft.com/en-us/library/aa326260(v=vs.71).aspx">Dispose</a> method fires. 

<h2>How to fix the InjectSqlConnection method?</h2>
The easiest solution is to ensure that all operations performed on the <strong>IEnumerable<string></strong> object which it retrieves should occur before the using statement completes and <strong>SqlConnection.Dispose</strong> fires.  In many scenarios this isn't and issue, but in the following example this isn't a possibility.  

```
[Route("")]
[AcceptVerbs("Get")]
public IEnumerable<string> ReturnNamesForWeb()
{
  using (var connection = new SqlConnection())
    //var connection = new SqlConnection();
  {
    connection.ConnectionString = 
    "Data Source=localhost;Initial Catalog=AdventureWorks2014;
      Integrated Security=SSPI";
    connection.Open();  

    return GetNamesWithConnection(connection);
  }
}
```


This method is from a class which inherits the <a href="https://msdn.microsoft.com/en-us/library/system.web.http.apicontroller(v=vs.118).aspx" target="_blank">API Controller</a> where the method's job isn't to act upon the data, but return the it so the invoking method can serialize and send it out the response stream.  

In this scenario, the first option would be to take the the <strong>SqlConnection</strong> out of the using statement, and let the <a href="https://msdn.microsoft.com/en-us/library/0xy59wtx(v=vs.110).aspx" target="_blank">Garbage Collector</a> (eventually) dispose of it.  This is a bad idea, and really isn't a viable solution.  Not disposing of <strong>SqlConnections</strong> will eventually use up all the available ones in the connection pool.

The second option would be to manually enumerate through the returned enumeration. 
change this: 

```
return GetNamesWithConnection(connection);
```

to this:

```

foreach(var name in GetNamesWithConnection(connection))
{
  yield return name;
}

```

This is a possible solution, but it is a lot more work on the part of writing the code (since you would have to do this to each method returning data) and more work on the part of the system (keeping track multiple enumerators (both the original and the new), etc.).  It's a viable option, but it's not ideal.

A third option would be to convert the returned enumeration to an object which doesn't require a <strong>SqlConnection</strong> to operate such as an <a href="https://msdn.microsoft.com/en-us/library/9b9dty7d.aspx" target="_blank">Array</a> or a <a href="https://msdn.microsoft.com/en-us/library/6sh2ey19(v=vs.110).aspx" target="_blank">List<T></a>.  


```
return GetNamesWithConnection(connection).ToList();
```


This works, because the <a href="https://msdn.microsoft.com/library/bb342261(v=vs.100).aspx" target="_blank">ToList()</a> method creates a <strong>List</strong> object which implements <strong>IEnumerable</strong> and the <strong>ToList</strong> method loops through the contents of the enumeration adding each item to the list.  Lists (and Arrays) exist purely in memory and is why the <strong>SqlConnection</strong is unnecessary loading their contents.  With small amounts of data and few requests, this isn't problematic, but as the requests and size of the collection grows, the system requires more memory to accommodate the requests.  Let's say the method on average returns 100 rows, and that the total size of the rows is approximately 1 MB.  Most web servers can easily handle 1 MB of data in memory without issue, but as concurrent requests grow, each request requires another MB of memory to hold the data.  If there are 1000 concurrent requests, then the system now requires 1 GB of memory just to store the data necessary to serialize it and send it out. This differs from the other other method of returning the object created by the <strong>yield</strong> return statement which only loads the record it needs at that moment (this is referred to as <a href="https://en.wikipedia.org/wiki/Lazy_loading">Lazy Loading</a>).  

<strong>Request.RegiserForDispose</strong>

The <strong>ApiController</strong> object has a property named <strong>Request</strong> which returns an object of type <a href="https://msdn.microsoft.com/en-us/library/system.net.http.httprequestmessage(v=vs.118).aspx">HttpRequestMessage</a>.  This object has a method named <a href="https://msdn.microsoft.com/en-us/library/system.net.http.httprequestmessageextensions.registerfordispose(v=vs.118).aspx">RegisterForDispose</a> which returns void and takes in an <strong>IDisposable</strong> object.  Any object passed into this method will be disposed once the current request finishes which is after the contents have been added to the response stream.


```

public IEnumerable<string> InjectSqlConnection()
{   
    var connection = new SqlConnection(connection);
    Request.RegisterForDispose(connection);

    connection.ConnectionString = 
      "Data Source=localhost;Initial Catalog=AdventureWorks2014;
         Integrated Security=SSPI";
    connection.Open();  

    return GetNamesWithConnection(connection);
  }
}

```


Encasing the <strong>SqlConnection</strong> in a <a href="https://msdn.microsoft.com/en-us/library/yh598w02.aspx" target="_blank">using</a> statement no longer becomes necessary to ensure it gets disposed.  This allows the SqlConnection's creation and registration to dispose to be abstracted into it's own method.  As long as the system retrieves all connections from this method, it will be impossible to leave a connection undisposed.  
  

```
public SqlConnection CreateConnection () 
{
  var connection = new SqlConnection();
  connection.ConnectionString =
    "Data Source=localhost;Initial Catalog=AdventureWorks2014;
        Integrated Security=SSPI";
  connection.Open();  
  return connection;
}

  public IEnumerable&lt;string&gt; InjectSqlConnection() => 
    GetNamesWithConnection(CreateConnection());
}
```