---
layout: post
title: "Compile Time Stored Procedures"
date: 2015-04-25 00:00:00 -0500
---
<p>(code for this project can be found <a href="https://github.com/kemiller2002/StructuredSight/tree/master/CompileTimeStoredProcedures/CompileTimeStoredProcedures">here</a>)</p>

<p>One of the largest problems with interacting with databases is the lack of compile time assurance that the application code and the SQL code interact correctly.  Testing helps with finding the errors, but as a system grows in size and complexity it becomes both difficult and costly to find every mistake.  There are pre-made solutions such as <a href="https://msdn.microsoft.com/en-us/library/hh156509.aspx" title="F# Type Providers" target="_blank">F# Type Providers</a> or <a href="https://msdn.microsoft.com/en-us/data/ef.aspx" title="Entity Framework" target="_blank">Entity Framework</a>, but these present their own challenges such as not having the project in F# or avoiding the heavy handedness of Entity Framework.  </p>

<p>Fortunately SQL server provides enough information about Stored Procedures in the Metadata tables that makes it possible to create a from scratch compile time safe mapping between stored procedures and application code.  Combining this with 
<a href="https://msdn.microsoft.com/en-us/library/bb126445.aspx">T4 Text Templates</a> it's possible to automatically generate all the code necessary to handle stored procedure inputs and returns.

</p>
<p>The first part is to pull all the stored procedure data from the <strong>sys.procedures</strong> view.  </p>

```
SELECT p.Name, Object_Id, s.name FROM sys.procedures p
JOIN sys.schemas s ON p.schema_id = s.schema_id
WHERE p.Name NOT LIKE &#39;sp_%&#39;

```

<p>I remove all the procedures starting with **sp_** as this prefix is meant to designate that it is a system procedure for use by SQL Server.  The three important pieces from the table are the <strong>Procedure Name</strong> (p.name), <strong>Object Identifier</strong> (Object_Id), and the <strong>Schema Name</strong> (s.name).</p>

<p>
<img src="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/CompileTimeStoredProcedures/Images/sys.procedures.png" alt="Select Sys Procedures"></p>
<p>Armed with the <strong>object id</strong> it&#39;s now possible to pull all the data for a procedures parameters from the <strong>sys.parameters</strong> view.  </p>

```
SELECT parameter_id, object_id, p.Name, t.name as pType,
t.max_length, has_default_value FROM sys.Parameters p
JOIN sys.types t ON p.system_type_id = t.system_type_id
and Object_id =

```

<p>With this and the **procedure&#39;s name** it&#39;s possible to construct code to call SQL Server stored procedures which is compile time safe.  Simply generate a method which takes in the appropriate parameters to call the stored procedure.  The last step is to generate a lookup table which translates the SQL datatypes to ones which are valid C# syntax.  </p>

```
public interface IDataType
{
  string Type {get;set;}
  bool AllowsNull{get;set;}
}

public static string GetParameterType(IDataType data)
{
   switch (data.Type)
   {
     case &quot;bigint&quot;: {return &quot;long&quot; + (data.AllowsNull ? &quot;?&quot; : &quot;&quot;);}

     case &quot;varbinary&quot;:
     case &quot;image&quot;:
     case &quot;binary&quot;: {return &quot;byte[]&quot;;}

     case &quot;bit&quot;: {return &quot;bool&quot; + (data.AllowsNull ? &quot;?&quot; : &quot;&quot;);}

     ...
   }
}

```

<p><strong>Idenfitfing which parameters have a default value and be not passed to the procedure</strong></p>

<p>Since stored procedure parameters can have default values and therefore aren&#39;t necessary to populate, it would be nice to be able to know which one&#39;s are available but aren&#39;t required.  This is a little trickier.  There is a field **has_default_value** which looks like it would tell you if the parameter has a default value and therefore is optional, but there is a problem in the SQL Server metadata table, and it is only correct for CLR parameters, and not SQL ones.  In order to actually find out if parameter has a default value, there are two options.  </p>

<ol>
<li>Use Sql Management Objects (SMO)</li>
<li>Pull the stored procedure code using something like SELECT OBJECT_DEFINITION (PROCEDURE_OBJECT_ID)</li>
</ol>

<p>Using SMO to pull the definition is actually relatively simple assuming you have access to the appropriate dlls from Sql Server Management Studio.</p>

```
C:\Program Files (x86)\Microsoft SQL Server\100\SDK\Assemblies\

Microsoft.SqlServer.ConnectionInfo.dll
Microsoft.SqlServer.Smo.dll
Microsoft.SqlServer.SqlEnum.dll
Microsoft.SqlServer.Management.Sdk.Sfc.dll

```

<p>Code:</p>

```
Microsoft.SqlServer.Management.Common.ServerConnection serverConnection
 = new Microsoft.SqlServer.Management.Common.ServerConnection(sqlConnection);

var server = new Microsoft.SqlServer.Management.Smo.Server(serverConnection);
var database = server.Databases[sqlConnection.Database];
var parms = database.StoredProcedures[procedureName].Parameters;

foreach( Microsoft.SqlServer.Management.Smo.StoredProcedureParameter
  parm in parms)
{
  new Column
  {
    ObjectId = parm.ID,
    Name = parm.Name,
    Type = parm.DataType.ToString(),
    AllowsNull = !String.IsNullOrEmpty(parm.DefaultValue),
    IsOutput = parm.IsOutputParameter
  }
 }

```

<p>If you don't have access to the SMO dlls, you can still use something akin to a regular expression (or some other parsing method) to pull the parameter information out from the Object_Definition.  As with parsing anything, there are a number of gotchas to watch out for, but it is possible with a little bit of work.</p>
<h2 id="-user-defined-tables"> User Defined Tables</h2>
<p>Starting in SQL Server 2008 it is possible to pass tables as parameters into stored procedures.  There is view called **sys.table_types** which lists all of the user defined tables and has the necessary columns to link it back to which stored procedures use it as a parameter, and retrieve its column definition.  </p>

```
SELECT type_table_object_id, Name, user_type_id FROM sys.table_types

```

<h2 id="stored-procedure-output">Stored Procedure Output</h2>
<p>To automatically generate C# code for SQL Server output, you'll really need SQL Server 2012 and later for it to be effective.  In that edition they added, the **dm_exec_describe_first_result_set_for_object** procedure</p>

```
SELECT column_ordinal, dm.name, dm.is_nullable, dm.system_type_id, t.name, dm.max_length
FROM sys.dm_exec_describe_first_result_set_for_object
(
    STORED_PROCEDURE_OBJECT_ID,
    NULL
) dm
JOIN sys.types t ON t.system_type_id = dm.system_type_id
WHERE t.name <> 'sysname'

```

<p>This procedure takes a stored procedure **Object_Id** and returns the column information concerning the first result set it finds.  </p>
<p><img src="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/CompileTimeStoredProcedures/Images/resultset.png" alt="Result Set"></p>
<p>The great thing about this procedure is that not only can you generate classes which can automatically update their types and names based on a result set that changes during development, but the procedure will return nothing if the stored procedure is non functional because of a changes in the database.  By running this this and having it map all the result sets, the ones which come up blank which should return data indicate the procedure will not work with the current schema and needs to be fixed.  It quickly helps eliminate the difficulties of knowing which procedures will break during program execution.</p>
<h2 id="putting-it-all-together">Putting It All Together</h2>

<p>With these procedure, all that is really necessary is something that opens a connection to the database, pulls the necessary data and create the appropriate C# files.  </p>

<p>As an example and a quick starter I've created an <a href="https://github.com/kemiller2002/StructuredSight/tree/master/CompileTimeStoredProcedures/CompileTimeStoredProcedures">example solution</a>.  The SQL metadata retrieval is in the <a href="https://github.com/kemiller2002/StructuredSight/blob/master/CompileTimeStoredProcedures/CompileTimeStoredProcedures/Classes.tt">Classes.tt</a> file, and all the code C# generation is in the <a href="https://github.com/kemiller2002/StructuredSight/blob/master/CompileTimeStoredProcedures/CompileTimeStoredProcedures/StoredProcedures.tt">StoredProcedures.tt</a> file.  The database I used to generate the example code is found <a href="https://github.com/kemiller2002/StructuredSight/tree/master/CompileTimeStoredProcedures/Example%20Database">here</a>, and uses the <a href="https://msftdbprodsamples.codeplex.com/releases/view/125550" title="Adventure Works" target="_blank">Adventure Works Database</a>.