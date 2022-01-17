---
layout: post
title: "Find a Guid In a Haystack"
date: 2014-03-16 00:00:00 -0500

---

A little while ago we had a problem when an unidentified Guid came up in an error email.  Whoever wrote the original message, knew exactly where to look to find the associated record, but by now this person was long gone, and we were left with a problem on our hands.  There was an error with an account, and we didn't know where to look.  Obviously, if there was only one table with a Guid as a field, it would be easy to find, but the problem becomes a lot larger when every table has one or multiple fields with it as a type.  You're left blankly staring at it, and ultimately facing the realization that you have to go through each table one at a time field by field.  

We all agreed this was a waste, and we needed to speed up the process.  One, we can't stop development so everyone can look for it, and two, if it happens again, we're back in the same spot.  To handle this, I created the following script.  It uses the system tables to look up each column type and build a select statement to search the database one field at a time. 


```

DECLARE @SearchID VARCHAR(50)
DECLARE @TableName VARCHAR(50)
DECLARE @ColumnName VARCHAR(50)
DECLARE @SqlCode NVARCHAR(500)

SET @SearchId = 'CD839615-AE28-4B8B-99D0-11F82FADB187' 



DECLARE search_cursor CURSOR FOR
        SELECT t.Name, c.Name FROM sys.tables t 
                JOIN sys.columns c ON t.object_id = c.object_id
                JOIN sys.types tp ON c.system_type_id = tp.system_type_id
                WHERE tp.name = 'uniqueidentifier' 


OPEN search_cursor

FETCH NEXT FROM search_cursor 
INTO @TableName, @ColumnName

WHILE @@FETCH_STATUS = 0
BEGIN
        SET @SqlCode = 'SELECT * FROM ' + @TableName 
                     + ' WHERE ' 
                     + @ColumnName + ' = ''' + @SearchId + ''''
        
        SELECT @SqlCode

        exec sp_sqlexec @SqlCode

        FETCH NEXT FROM search_cursor 
        INTO @TableName, @ColumnName
END

CLOSE search_cursor
DEALLOCATE search_cursor

```


Did it take a little time? Yep.  Did it get the job done? Absolutely.  It didn't give us the table name, but it gave us all the associated fields to the record, and that was more than enough to find it.  The real win was that none of us had to spend time doing something that a computer can do for us.  Its time spent searching the database is nothing compared to how much money the company would spend if we had to look for it. 

You can modify it to look for anything in the database by changing the type the cursor statement looks for ** tp.name = 'uniqueidentifier'**.  You can also change this from **=** to **like** if you want to search in parts of character fields ** @ColumnName + ' like ''%' + @SearchId + '%'''**.  This is what I did when I had to modify the script to search for password information a system was saving to the database when it had to be scrubbed out.

<style type="text/css">
pre {
    background-color: #f0f0f0;
    padding-left: 10px;
    padding-right: 10px;
    font-size:8pt;
}
</style>