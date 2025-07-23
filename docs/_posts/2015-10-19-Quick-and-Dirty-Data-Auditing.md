---
layout: post
title: "Quick and Dirty Data Auditing"
date: 2015-10-19 00:00:00 -0500
---
A lot of times a project, especially as it progresses, lacks proper requirements, time, or the resources, and tracking changes to data is often an afterthought. Rapid changes to the the database schema and code occur on a frequent basis, and with a moving trend to create software with fewer checks and balances (less quality assurance staff, less emphasis from management to allow proper time for code development), errors in saved data becomes more and more prevalent.

With all of this in mind, the ability to track changes in data is a powerful tool for both development and production issue remediation. However, one of the biggest issues faced is maintaining auditing tables when the database schema changes, and trying to ensure they don't cause a system failure on their own.

Take for example the following table:

```
  CREATE TABLE Account
  (
     AccountId INT NOT NULL PRIMARY IDENTITY(1,1)
    ,FirstName VARCHAR(10) NOT NULL
    ,LastName VARCHAR(10) NOT NULL
  )

```

and with this a common scenario is to create an auditing table like so:

```
  CREATE TABLE Account_Audit
  (
    AccountId INT NOT NULL
    ,FirstName VARCHAR(10) NOT NULL
    ,LastName VARCHAR(10) NOT NULL
  )

```

and a trigger which fires on updates:

```
  CREATE TRIGGER ...
    INSERT INTO Account_Audit SELECT * FROM inserted
    ...

```

With this scenario there are some problems. The most major being that if the live table gets updated and not the audit table, the trigger fails and stops the insert. (This is easily fixed in development, not so much in production.) Hopefully if this is the case, the calling application is verifying the <a href="http://structuredsight.com/2014/05/27/return-from-the-void/" target="_blank">database update succeeds</a>.

This problem can be solved by specifying the columns specifically,

```

INSERT INTO Account_Audit (AccountId, FirstName, LastName)
  SELECT AccountId, FirstName, LastName FROM inserted

```

but this leaves the problem that if columns are added to the live table, and either the audit table or the trigger aren't also updated, new fields may slip through the cracks and won't be noticed until someone actively checks the table for information. Unfortunately, this is a commonly occurring worse case scenario, because most of the time you only look in a audit table when you need to track down something that might be wrong.

There is actually a fairly simple solution to this problem. Store the updated data as a single text entry.  Furthermore it's just as easy to store it as an XML entry which most runtime environments have a convenient way of parsing. To accomplish this, create an auditing table with the appropriate fields:

```

CREATE TABLE Auditing
  (
    TableId INT NOT NULL
    ,EntryDateTime DATETIME NOT NULL DEFAULT (GETUTCDATE())
    ,Action VARCHAR(10) NOT NULL
    ,AuditEntry NVARCHAR(MAX) NOT NULL /* Keeping the type to be nvarchar so that if the format changes later the table doesn't have to be altered. */
    ,TransactionEntryId UNIQUEIDENTIFIER NOT NULL
 )

 CREATE CLUSTERED INDEX IX_Auditint_Clustered_TableId_TransactionEntryId ON Auditing(TableId,TransactionEntryId)
 --The index isn't necessary for auditing, but it will come in handy when retrieving data later.

```

The **TableId** is the **sys.tables.object_id** so that when a trigger enters data into the table, it is easily discernible which data came from where.  The **action** field says whether it was an insert or a delete. (Updates to a table do a delete of the previous data and then an insert.)  The **TransactionEntryId** links multiple entries into the audit table together to show which insert and deletes are the result of an **update**.

Next, create the trigger and apply it to the appropriate table or tables.

```

CREATE TRIGGER KeyDeployment.Audit_KeyDeployment_Environments
ON KeyDeployment.Environments
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
  DECLARE @TransactionId UNIQUEIDENTIFIER = NEWID()

  SELECT @ParentTableId = parent_id FROM sys.triggers WHERE object_id = @@PROCID

  IF EXISTS (SELECT 1 FROM inserted)
  BEGIN
    INSERT INTO dbo.Auditing
        (TableId, Action, TransactionEntryId, AuditEntry)
    SELECT
    @ParentTableId,
    'Insert',
    @TransactionId,
    (SELECT * FROM inserted FOR XML RAW)
  END

  IF EXISTS(SELECT 1 FROM deleted)
  BEGIN
    INSERT INTO dbo.Auditing (TableId, Action, AuditEntry, TransactionEntryId)
    SELECT
    'Deleted'
    ,(SELECT * FROM deleted FOR XML RAW)
    ,@TransactionId
    END
END
/*
  Yes normally SELECT * is bad, but in this scenario it allows us to make changes to the live tables without modifying the triggers.
*/

```

In truth, this doesn't work for all scenarios.  Tables with a high transaction volume, or ones with large amounts of data for each record may suffer problems, but it works well enough in many scenarios to be a viable solution even if it isn't one for the long term. (Like any solution, before jumping to conclusions about performance, the change should be tested.)  Also, by keeping the trigger contents generic, this same code can be applied to multiple places without needing to change it alleviating the possibility of accidentally making a mistake in the setup process.

Depending on the scenario, it can beneficial to be able to enable auditing at will. In an ideal scenario, it would be as easy as applying the trigger to the table when necessary, but in many organizations, this isn't possibly without a formal deployment process, and when looking into a problem normally auditing needs to be added quickly. By adding another table which tells triggers when to record data, auditing can be turned on by adding an entry.

```

CREATE TABLE AuditTables
(
  TableId INT NOT NULL PRIMARY KEY
)

```

and edit the trigger template

```

CREATE TRIGGER KeyDeployment.Audit_KeyDeployment_Environments
ON KeyDeployment.Environments
AFTER INSERT, UPDATE, DELETE
AS
BEGIN

  DECLARE @TransactionId UNIQUEIDENTIFIER = NEWID()
  DECLARE @ParentTableId INT

  SELECT @ParentTableId = parent_id FROM sys.triggers WHERE object_id = @@PROCID

  IF EXISTS (SELECT 1 FROM AuditTables WHERE TableId = @ParentTableId)
BEGIN
    IF EXISTS (SELECT 1 FROM inserted)
    BEGIN
      INSERT INTO
      dbo.Auditing (TableId, Action, TransactionEntryId, AuditEntry)
      SELECT
      @ParentTableId,
      'Insert',
      @TransactionId,
      (SELECT * FROM inserted FOR XML RAW)
    END

    IF EXISTS(SELECT 1 FROM deleted)
    BEGIN
    INSERT INTO
    dbo.Auditing (TableId, Action, AuditEntry, TransactionEntryId)
    SELECT
    @ParentTableId,
    'Deleted',
    (SELECT * FROM deleted FOR XML RAW)
    ,@TransactionId
    END
  END
END

```

The drawback to this approach is that the triggers will still fire on each data change in the table, but it does alleviate some of the overhead of the second insert statement and the audit tables growing.