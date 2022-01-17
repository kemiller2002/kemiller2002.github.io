---
layout: post
title: "Generating SQL"
date: 2014-01-15 00:00:00 -0500

---

Generally, I don’t like writing code more than once if I can avoid it.  Most people apply the DRY principle to application code, but I also find it a good idea to follow this in writing SQL administration code.   Most applications today store a significant amount of administration level data in the database so the support team can quickly change the behavior of a system without having to redeploy the application.  Although beneficial, it does require someone to write SQL to code insert and update values in the database, and depending on the amount necessary can be time consuming, error prone, and tedious.

To handle tasks which require writing several (In my case hundreds) of SQL Insert statements at once, I created the following <a title="Sql Generator fsx file" href="https://github.com/kemiller2002/StructuredSight/blob/master/SqlGeneration/Generator.fsx" target="_blank">script</a>.

Use it by creating an accompanying file like so:

```

#load "Generator.fsx"

open Generator


let tableName = "Users"
let schemaName = "dbo"

let fields = seq {
            //This is the Entry record.  Defined in the other fsx file.
    yield {FieldName="UserId"; FieldValue = "1"; IsNumber=true; 
              IsPrimaryKey=true} 
    yield {FieldName="Login"; FieldValue = "kmiller"; IsNumber=false; 
              IsPrimaryKey = false}
    yield {FieldName="Password"; FieldValue = "password"; IsNumber=false; 
              IsPrimaryKey = false} 
}


let insertStatments = GenerateInsertStatement schemaName tableName fields

//prints to console or fsi. 
printfn "%s" insertStatments

//This line writes it to a text file.
//System.IO.File.WriteAllText(@"C:\temp\MyInsertScript.sql", insertStatments)

```


The crux behind the script is the <code>GenerateInsertStatement</code>.  It takes the table name as a string and then a <a href="http://msdn.microsoft.com/en-us/library/dd233209.aspx">sequence</a> of Entry.

To handle multiple inserts all you need do is create a sequence of sequences: 


```

﻿#load "Generator.fsx"

open Generator

let tableName = "Users"
let schemaName = "dbo"

let multipleInserts = seq {

    yield seq {
            //This is the Entry record.  Defined in the other fsx file.
         yield {FieldName="UserId"; FieldValue = "1"; IsNumber=true;
                         IsPrimaryKey=true} 
         yield {FieldName="Login"; FieldValue = "kmiller"; IsNumber=false; 
                         IsPrimaryKey = false}
         yield {FieldName="Password"; FieldValue = "password"; IsNumber=false; 
                         IsPrimaryKey = false} 
    }

    yield seq {
            //This is the Entry record.  Defined in the other fsx file.
         yield {FieldName="UserId"; FieldValue = "2"; IsNumber=true; 
                         IsPrimaryKey=true} 
         yield {FieldName="Login"; FieldValue = "Lucy"; IsNumber=false; 
                         IsPrimaryKey = false}
         yield {FieldName="Password"; FieldValue = "woof"; IsNumber=false; 
                         IsPrimaryKey = false} 
    }


}


let insertStatments = multipleInserts 
                    |> Seq.map(fun(x) -> GenerateInsertStatement schemaName tableName x) 
                    |> (fun(x) -> System.String.Join(@"
                    GO
                    ", x))

printfn "%s" insertStatments
```


Sometimes you need to insert data into a table with an identity column.  This requires wrapping the statement in using the <a href="http://msdn.microsoft.com/en-us/library/ms188059.aspx" title="Identity Insert" target="_blank">Identity Insert</a> options. The generation script has a function which will wrap the sql code in an identity insert for the table.  Use the <strong>AddIdentityInsertWrapper</strong> like so:


```

let insertStatments = GenerateInsertStatement schemaName tableName fields

let fullSql = AddIdentityInsertWrapper schemaName tableName insertStatments

```
 

