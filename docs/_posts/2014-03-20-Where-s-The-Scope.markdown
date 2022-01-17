---
layout: post
title: "Where's The Scope"
date: 2014-03-20 00:00:00 -0500

---

When I started programming, it was customary to declare the variables at the top of the method.  Many people stated that it was clearer to declare everything at the beginning and in the order it was used (or alphabetically for easier searching), and some languages, such as Delphi (Turbo Pascal) even required it.  As the years passed over the past fifteen years, this has been a debated topic.  

From my experience, the shift has been towards only declaring a variable just before needing it.  In truth this makes a lot of sense, it allows the compiler to enforce scope and allows programmers the luxury of having it not compile when a variable is used when it shouldn't be.  This comes with the drawback that variable declarations become more difficult to find when searching through.  If it’s declared at the top, then it’s always in the same place, although in truth modern IDEs alleviate this problem as it’s normally a single key stroke to find its declaration.  

So, what is the correct answer between the two styles?  As a blanket rule, the answer is: they’re both wrong.  Where to declare variables is significantly different than what to name them.  Most languages don’t care about what a variable is named.  As long as they follow certain rules (don’t start with a number in some, or must start with a $ in others), the compiler/interpreter doesn't really do much else with it.  Where they are declared though can cause significant changes to how the system functions, and most developers forget that between languages this can make a huge difference.  
Take C# and JavaScript for example.  They look somewhat the same.  Are they?  No, they aren't really even close.  In C# variables are created at the branch level:

```

public static void Main()
{
     string executeCode;
     if(true)
     {
          executeCode = "This code runs.";
     }

     Console.WriteLine(executeCode);
}

```



<a href="http://dotnetfiddle.net/1QW6Wa" title="Running C# Code" target="_blank">Running Example</a>



```

public static void Main()
{
     if(true)
     {
          string example;

     }

     example = "This code does not";
}

```


<a href="http://dotnetfiddle.net/mQDY2Z" title="Non Running example" target="_blank">Non-running example</a>

In JavaScript they are declared at the function level:

```

myVariable = "I'm Global";

function showGlobal() {
     alert(myVariable);
}

showGlobal();

function localOverride (){
     alert(myVariable);
     
     if (true){
          var myVariable;
     }
} 

localOverride();



```


<a href="http://jsfiddle.net/pSHh2/1/" title="JSFiddle Example" target="_blank">Running Example</a>  

The second alert shows "undefined" instead of "I'm Global" even though it's at the top of the function, because JavaScript moves all variable declaration to be at the top of the method regardless of where they are declared. (Instantiating the variable when it's declared in the branch doesn't initialize it until the branch either.  Only the declaration is moved to the top of the function). 

Believing that all languages work the same, because one can write similar syntax with error can cause major problems.   The language that trips a lot of people up and how they should be declared is SQL.  Take the following example:


```

DECLARE @Counter INT

SET @Counter = 10

WHILE @Counter > 0
BEGIN
DECLARE @LocalVariable INT

IF(@LocalVariable IS NULL)
BEGIN
SET @LocalVariable = @Counter
END

PRINT @LocalVariable
SET @Counter = @Counter - 1
END

```

Here's the output: 

```

10
10
10
10
10
10
10
10
10
10

```


Notice how it only prints out 10 even though the counter is being updated each loop?  If the **@LocalVariable** was actually initialized each time the loop executed, it would have counted down to 1 because it would be set back to **NULL** each time.  

What you have to remember, is that even though you wrote it a certain way doesn't mean that the compiler/interpreter will output the exact same instruction set you thought it would.  Assuming a variable will behave a certain way without testing or explicitly setting it, can be the difference between working code, and spending several hours tracking down a problem. 

<style type="text/css">
pre {
    background-color: #f0f0f0;
    padding-left: 10px;
    padding-right: 10px;
    font-size:8pt;
}
</style>