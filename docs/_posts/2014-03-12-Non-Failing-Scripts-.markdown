---
layout: post
title: "Non Failing Scripts "
date: 2014-03-12 00:00:00 -0500

---

One of the most frustrating parts of database development is double applying scripts.  If you are working directly on the production server, this problem is alleviated by making alterations to the SQL commands along the way.   With doing development in a test environment first this becomes much more difficult.  If you are creating a new stored procedure, the first time that it’s run you must use the **CREATE** command, and subsequently the **ALTER** command every time after.  This makes handing the scripts off to a database team to run on the production servers difficult, because you have to account for the environment you are developing becoming out of sync with the environment your scripts will ultimately be run in.  

Ideally, you could refresh your database environment to mirror production frequently to keep synchronicity, but in reality this is time consuming and many times not possible.  The key becomes to write scripts so they can be applied to any environment either once or multiple times without causing errors.  In my previous post I talked about how to use generators to make data scripts re-runnable without accidentally double inserting records.  Using SQL Server’s system tables it’s relatively easy to do the same for tables, columns, stored procedures etc.  

For stored procedures, most places just prepend a **DROP PROCEDURE** command and then create the stored procedure each time.  The problem with this is that it removes permissions, and it’s easy to miss recreating them.  I prefer to create a dummy procedure immediately before, and then using the alter command to change it to what it should be.  If the procedure is already in the environment, it skips the step and immediately moves to the alter portion of the script.


```

IF NOT EXISTS(SELECT * FROM sys.procedures WHERE name = 'my procedure name')
BEGIN
     exec sp_sqlexec N'CREATE PROCEDURE [my procedure name] AS BEGIN PRINT ''Place holder'' END'
END
GO

ALTER PROCEDURE [my procedure name]
AS
BEGIN
     SELECT * FROM Accounts
END

```


The same thing can be done with adding or dropping columns

```

IF NOT EXISTS(SELECT * FROM sys.tables t 
                   JOIN sys.columns c ON t.object_id = c.object_id
                   WHERE
                   t.Name = 'MyTable' AND 
                   c.Name = 'NewColumn')
BEGIN
     ALTER TABLE MyTable
ADD [NewColumn] VARCHAR(50)
END

```


Virtually everything in MSSQL Server can be queried by the system tables:
<ul> 
<li>**views : sys.Views**</li>
<li>**constraints: sys.SysConstraints**</li>
<li>**schemas: sys.Schemas<br/>(you can link to this in your queries if your tables etc. aren't under the default)**</li>
</ul>
These are just a few of the system tables for querying data.  If you need additional information about a database object, you can always link back to sys.objects.  For Example, sys.SysConstraints doesn't contain the constraint name, so to get it, all you need do is create a join.

```

SELECT o.Name FROM  sys.sysconstraints c 
       JOIN sys.objects o ON c.constid = o.object_id 
       --c.id is object id of what the constraint is linked to, like a table

```


<style type="text/css">
pre {
    background-color: #f0f0f0;
    padding-left: 10px;
    padding-right: 10px;
    font-size:8pt;
}


</style>