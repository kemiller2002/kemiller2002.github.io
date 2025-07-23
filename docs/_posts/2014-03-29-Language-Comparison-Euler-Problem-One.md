---
layout: post
title: "Language Comparison Euler Problem One"
date: 2014-03-29 00:00:00 -0500
---
Question: 

If we list all the natural numbers below 10 that are multiples of 3 or 5, we get 3, 5, 6 and 9. The sum of these multiples is 23.

Find the sum of all the multiples of 3 or 5 below 1000.

F# 

```

let answer = [1..999] |> Seq.where(fun x -> x % 5 = 0 || x % 3 = 0) |> Seq.sum
printfn "%i" answer 

```


<a href="http://dotnetfiddle.net/XzDhZi" title="F# Example One" target="_blank">F# Example One</a>

or


```

let value = ref 0 
for i in [1..999] do 
    if (i % 3 = 0 || i % 5 = 0) then 
        value := value.Value + i 


printfn "%i" value.Value

```

<a href="http://dotnetfiddle.net/CRJDvk" title="F# Example 2" target="_blank">F# Example Two</a>

C# 


```

var value = Enumerable.Range(1,999).Where(x=>x % 3 == 0 || x % 5 == 0).Sum();
Console.WriteLine(value);


```

<a href="http://dotnetfiddle.net/hArqJg" title="C# Method One">C# Example One</a>

or

```

var value = 0; 

for(var ii = 1; ii < 1000; ii++)
{
     if(ii % 3 == 0 || ii % 5 == 0)
     {
          value += ii; 
     }
}
Console.WriteLine(value);

```

<a href="http://dotnetfiddle.net/n4x1Jq" title="C# Example 2" target="_blank">C# Example Two</a>

JavaScript

```

   var ii, val = 0; 
    
    for(ii = 0; ii < 1000; ii++){
        if(ii % 3 === 0 || ii % 5 === 0){
            val += ii
        }
    }
    
    alert (val);

```


<a href="http://jsfiddle.net/3Mk5u/" title="JS Fiddle Example" target="_blank">JavaScript Example One</a>

SQL 


```

DECLARE @i INT 
DECLARE @total INT 

SET @i = 0
SET @total = 0

WHILE @i < 1000 
BEGIN
     IF @i % 3 = 0 OR @i % 5 = 0
     BEGIN
          SET @total = @total + @i
     END

     SET @i = @i + 1

END

PRINT @Total 

```
 

or


```

IF EXISTS(SELECT * FROM sys.objects WHERE NAME = 'MakeRangeTable')
BEGIN
     DROP FUNCTION MakeRangeTable
END

GO

CREATE FUNCTION MakeRangeTable (@maxNumber INT)
     RETURNS  @Numbers TABLE (
          Number INT NOT NULL
     ) 
AS 
BEGIN
  
     DECLARE @i INT 
     SET @i = 1
     
     WHILE @i < 1000 
     BEGIN
          INSERT INTO @Numbers(Number) VALUES(@i)
          SET @i = @i + 1
     END
     RETURN
END

GO

SELECT SUM(Number) FROM MakeRangeTable(999) WHERE Number % 3 = 0 OR Number % 5 = 0

```


PowerShell 


```
1..999 | Where {($_ % 3 -eq 0) -or ($_ % 5 -eq 0)} | Measure -sum
```


or


```

$value = 0

foreach ($i in 1..999) {
     if($i % 3 -eq 0 -or $i % 5 -eq 0){
          $value += $i
     }
}

Write-Output $value

```



<style type="text/css">
pre {
    background-color: #f0f0f0;
    padding-left: 10px;
    padding-right: 10px;
    font-size:8pt;
}
</style>