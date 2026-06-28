---
layout: post
title: "You Get an Update, and You Get an Update, and You Get an Update"
date: 2014-03-28
---

What's the difference between 

```

  UPDATE RecordsToProcess SET ReadyToProcess = 1 

```

and 

```

  UPDATE RecordsToProcess SET ReadyToProcess = 1 WHERE ReadyToProcess <> 1

```


When they've completed, both ensure all the records in the table have the ReadyToProcess value set to 1.  When looking strictly at the records in the database, there is no difference.  In this example, the end result isn't what's important, but it's how the system there.  In the first one it updates all records regardless of what the end value was, and the second only makes an update if the value is different than what is being set.  The first update statement isn't telling SQL Server, "Hey I want the value to be this."  What it's really telling SQL Server is, "I want you to update the field in this record to this.  It may look the same as the previous value, but I still want you to update it."

Depending on the number of records in the table, and what is affected by the table updating, it might not be much of a problem.  The first is less efficient than the second, but it still accomplishes the task.  It really starts to be an issue when updating a large number of records, or when something like a trigger is associated with the table.  Every time SQL Server updates a record, the associated trigger fires, and this is true even if the new value and the old value are the same.  An update is an update no matter what the end value is.  If the system logs audit data for updates etc. the non-qualified statement is forcing the system to do a lot more work for absolutely no gain.


<style type="text/css">
pre {
    background-color: #f0f0f0;
    padding-left: 10px;
    padding-right: 10px;
    font-size:8pt;
}
</style>