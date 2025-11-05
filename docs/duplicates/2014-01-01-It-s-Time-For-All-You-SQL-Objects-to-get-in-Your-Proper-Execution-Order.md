---
layout: post
title: "It's Time For All You SQL Objects to get in Your Proper Execution Order"
date: 2014-01-01 00:00:00 -0500
---
One of the biggest difficulties with moving database objects from one environment to another is getting the execution order correct.  It's possible to script all the objects to a single file from the database, but this makes it difficult to store individual procedures, tables, etc. to source control. The ideal approach is to store all the objects their own file and then have a script which can look up and order the files for their appropriate execution order. 

Fortunately, MSSQL Server has all of this information in it's meta tables.  By looking at which tables link to other tables through foreign keys shows which tables need to run before others, and checking the **sys.sysreferences** table it's possible to see which procedures and views reference objects.

For Tables 

```

SELECT cs.name AS ChildSchemaName, co.name AS ChildObjectName, co.Type 
  FROM sys.sysreferences r

  JOIN sys.objects co ON r.rkeyid = co.object_id --Child Object
  JOIN sys.schemas cs ON co.schema_id = cs.schema_id --Child Schema

  JOIN sys.objects po ON r.fkeyid = po.object_id --Parent Object
  JOIN sys.schemas ps ON po.schema_id = ps.schema_id -- Parent Schema
WHERE 
  po.Name = 'PARENT_TABLE_NAME' AND 
  ps.Name = 'PARENT_SCHEMA_NAME'

```


<img src="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/SqlObjectOrderDependency/TableDependencies.png" alt="Table Dependencies" />

The following query shows all the tables which reference it through a foreign key.  Running this for all tables in the database shows a list of all tables and those that reference them.  The tables which have children and no parents should be executed first followed those which have parents and children and finally only those which have only have parents.  

The <a href="https://msdn.microsoft.com/en-us/library/bb677185.aspx" title="sys.dm_sql_referenced_entities " target="_blank">sys.dm_sql_referenced_entities</a> shows the referencing entities for most objects in the system.  


```

SELECT DISTINCT referenced_schema_name, referenced_entity_name 
  FROM sys.dm_sql_referenced_entities ('SCHEMA_NAME.OBJECT_NAME','OBJECT') x 
  JOIN Sys.objects o ON x.referenced_id  = o.object_id
  AND o.type IN ('OBJECT_TYPE')

```


To get the execution order for all of the different object types understanding which types can access other types is key.  Tables can't access views, so table scripts should always be executed before, and views can't access stored procedures, so view scripts should execute before stored procedures. 

The type execution order should be: 

<ol>

 <li>Schemas</li>
 <li>Tables</li>
 <li>Service Queues</li>
 <li>Views</li>
 <li>Functions</li>
 <li>Table Valued Functions</li>
 <li>Stored Procedures</li>
 <li>Triggers</li>
 <li>Constraints (these can call functions, although simple ones I place with the table.)</li>
</ol>

With all of this in mind, writing a script to pull the reference information for all the objects, grouping them by type, and sorting the execution order is really pretty simple.  Once all the object information has been placed in the correct order, the last step is to pull them from source control and add them to the migration file.  I like to name my objects in source control with the following convention <strong>Schema.DatabaseObjectName.Sql</strong> and place it in a folder named the object type (Table scripts go in the Table folder).  <img src="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/SqlObjectOrderDependency/SourceControlTables.png" alt="Source Control order." />

The real benefit from this is that while bundling the scripts, it's also possible to add logging information (which script ran, the time, etc.).  If the source control system has an API, it's also nice to log the last person to check in the script in case it fails. In addition to this, I have pre and post migration scripts which allow people to add SQL needed which may not cleanly fit into one of the object categories (migrating setup data for an application for instance).  


```

**********************************************************************************
* Starting Execution Database: AdventureWorks2012
* Starting Migration of: dbo.Accounts.sql
* Start Run Time: Apr 14 2015 11:14AM
**********************************************************************************

```


Now when the migration team or the automated deployment system runs the deployment file, it's guaranteed to be in the correct order, and there is a log of everything that is ran and when. 

The following files located at: <a href="https://github.com/kemiller2002/scm" title="SCM Files" target="_blank">https://github.com/kemiller2002/scm</a>.  All need to be located in the same directory.

<strong>ShowOrderDependency.ps1</strong>

```

$scriptpath = $MyInvocation.MyCommand.Path
$dir = Split-Path $scriptpath

. "$dir\GetItemOrderDependency.ps1"

$objectTypes = "U,SQ,V,FN,TF,P,TR".Split(",")

GetItemListing "Server=localhost\sqlexpress;Database=AdventureWorks2012;Trusted_Connection=True;" "AdventureWorks2012" $objectTypes | 
  select Values -Unique

```


By replacing the database connection string and target database, this file will print out all the object dependencies in order of run first to run last. 

<strong>BuildSqlFile.ps1</strong>
This file creates the output from files in source control.