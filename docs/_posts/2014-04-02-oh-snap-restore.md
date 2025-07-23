---
layout: post
title: "Oh Snap! (Restore)"
date: 2014-04-02
---

One of the most difficult aspects of the software development process is developing for and testing database updates.  Unlike updating web sites, and executables which can be either updated by an X-Copy or re-installing an MSI, databases always maintain state.  Once it’s been altered, the only way to go back to the previous incarnation is a database restore.  

In 2008, Microsoft introduced Snapshot Restore (Snap Restore).  It creates a secondary database on the server which tracks the deltas between the state when it was created and the changes made afterward.  Furthermore, the live database can be refreshed back to the state of the snapshot, making it great for development and testing.  You can write your scripts, test the updates, and if something is amiss, run the restore and try again.  It's also great for production deployments where getting it right the first time is paramount.  As long as the deployment occurs when no other updates are happening, all the updates can be validated and quickly backed out if need be. 


```

CREATE DATABASE SNAP_Example ON --the name of the snapshot database to create
(
     Name = 'Example' 
     -- The database file, each file in a database must have an entry.
     ,FileName = 'C:\Temp\SnapExample.ss' 
     --Path To Place Data For Snapshot file.

) AS SNAPSHOT OF Example --The database to copy

```



This can all be accomplished with backing up and restoring a copy of the same database.  The problem with this approach is that the secondary will be the same size as the original.  With small databases, this isn't an issue, but it becomes cumbersome as it gets larger.  

<img src="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/SnapRestore/Images/SnapBefore.jpg" alt="DB Snapshot creation" />

Notice the size on disk is only 128k.  As updates are made on the real database, this size grows, but it is still far smaller than making a full copy of it. 

When comparing the live database and the snap shot they have the same data after the creation: 

<img src="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/SnapRestore/Images/ComparingDBs.jpg" alt="Database comparison" />

Now after deleting the data from the live database: 

<img src="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/SnapRestore/Images/QueryAfterDelete.jpg" alt="After delete" />

To get the database back to the original state, just execute the restore command: 


```

USE Master --Any db but the one you are restoring will do.  
--Being in the same db will lock it and prevent the restore.
RESTORE DATABASE [Example] FROM Database_Snapshot = 'snap_example'

```


<img src="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/SnapRestore/Images/DataAfterRestore.jpg" alt="Data After Restore" />

Unfortunately, this only works on the Development and Enterprise versions of SQL Server, and it doesn't work on any database with Full Text Indexing.  With all this being said, MSDN licenses cover the use of the Development Edition server, making it available in development and test environments, and can help refine the update scripts before running them in production.  

All the scripts for this can be found <a href="https://github.com/kemiller2002/StructuredSight/tree/master/SnapRestore" title="Data Scripts" target="_blank">here</a>.

<a href="http://technet.microsoft.com/en-us/library/ms175158.aspx" title="MSDN" target="_blank">MSDN Database Snapshot Documentation</a>