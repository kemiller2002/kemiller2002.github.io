---
layout: post
title: "Reclaiming Stored Procedure Code"
date: 2015-01-05 00:00:00 -0500
---
One of the largest problems people have with keeping stored procedures in source control is not a disagreement that they should be but the change to the process in which they are accustomed to.  Normally with code, you create a physical file which is then piped to the compiler to output an assembly.  With Microsoft's SQL Server Management (SSMS) Studio the process of creating a file and then running it has changed this, because the database stores the SQL code for procedures in the database negating the need for a physical file.  This is a convenient shortcut but makes it difficult to keep SQL source in sync, because inevitably people forget to create the file from Management Studio when they're done. SSMS has a menu item for generating a file, but the creation options are limiting depending on the need, and it adds additional code around the procedure which most of the time isn't necessary.  Not to mention generating one file is simple enough, but having to do it for hundreds of changed procedures is tedious and time consuming.  

<img src="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/GettingProcedures/ScriptStoredProcedure.png" alt="Script Procedure" />

Fortunately, there is a convenient fix for this.  Since MSSQL Server stores the code in the database, you can easily retrieve the code and save it yourself.  Microsoft created the <strong>INFORMATION_SCHEMA.ROUTINES</strong> view which returns information such as it's name, type, schema, and most importantly, the code.  Using this approach you can also format the code the way you want when saving it to a file.  I always make the scripts so they <a href="http://structuredsight.com/2014/03/12/non-failing-scripts/" title="Non Failing Scripts" target="_blank">don't fail</a> so I prepend the necessary header code and change the create to an alter command before saving it.  


```

param(
    [string] $connectionString
    ,[string] $outputFilesLocation
)


function ExecuteReader
{
    param($fnExecuteReaderConents)

    $commandText = "SELECT SPECIFIC_CATALOG, SPECIFIC_SCHEMA, 
SPECIFIC_NAME, ROUTINE_DEFINITION FROM INFORMATION_SCHEMA.ROUTINES
WHERE ROUTINE_NAME NOT LIKE 'sp_%' AND ROUTINE_NAME NOT LIKE 'fn_%'"
    
    $connection = New-Object System.Data.SqlClient.SqlConnection
    $connection.ConnectionString = $connectionString
    $connection.Open()
    
    $command = New-Object System.Data.SqlClient.SqlCommand
    $command.Connection = $connection
    $command.CommandText = $commandText

    $reader= $command.ExecuteReader()
    
    while($reader.Read()){
        $fnExecuteReaderConents.Invoke($reader)
    }
}

$makeFileContens = {
    param($reader)

    $catalog = $reader.GetString(0)
    $schema = $reader.GetString(1)
    $procedureName = $reader.GetString(2)
    $contents = $reader.GetString(3)
    
    $replacementExpression = $contents -Replace "(?i:create procedure)","ALTER PROCEDURE"

    $header = "IF NOT EXISTS ( 
       SELECT * FROM sys.procedures t JOIN sys.schemas s 
       ON t.schema_id = s.schema_id WHERE t.name = '$procedureName' AND
       s.name = '$schema')
    BEGIN 
        exec sp_sqlexec 'CREATE PROCEDURE $schema.$procedureName AS PRINT ''TEMPLATE'' '
    END

    GO
"
    @{
        FileName = "$schema.$procedureName.sql"
        Contents = $header + $replacementExpression
    }
}

ExecuteReader $makeFileContens | foreach{
    Set-Content "$outputFilesLocation\$($_.FileName)" $_.Contents
}

```


File on Github at: <a href="https://github.com/kemiller2002/StructuredSight/blob/master/GettingProcedures/GetProcedures.ps1" title="Pull Procedures" target="_blank">https://github.com/kemiller2002/StructuredSight/blob/master/GettingProcedures/GetProcedures.ps1</a>