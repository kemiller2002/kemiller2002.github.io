---
layout: post
title: "C# 7 Additions – Throw Expressions"
date: 2016-09-01 00:00:00 -0500

---

In previous versions, throwing exceptions had certain limitations where they could be used.  Although not hampering, at times it caused additional work to validate and throw an exception, and C# 7 has removed much of the developer overhead for validation and execution.  

<h4>Expressions</h4>
Previously to throw an exception in the middle of an expression there were really two options: 

```
var colorString = "green,red,blue".Split(',');
var colors = (colorString.Length > 0) ? colorString : null
if(colors == null){throw new Exception("There are no colors");}

```

or 
```
var colorArray = (colors.Length > 0) ? 
   colors : 
   new Func<string[]>(() => { throw new Exception(); })();
```


It is now possible to also throw an exception in the middle of an expression.  Instead of checking for null, it is possible to throw as the second condition in the <a href="https://msdn.microsoft.com/en-us/library/ms173224.aspx" target="_blank">Null Coalescing Operator</a>.  

```
var firstName = name ?? throw new ArgumentException ();
```

It is also possible in the <a href="https://msdn.microsoft.com/en-us/library/ty67wk28.aspx" target="_blank">Conditional Operator</a> as well. 

```
var arrayFirstValue = (array.Length > 0)? array[1] : 
  throw new Expection("array contains no elements");
```


<h4>Expression Bodied Members</h4> 
C# 6 added the ability to write a method with a single statement with a "fat arrow" (=>) and the statement.  What used to be 

```
public string GetPhoneNumber ()
{
  return "867-5309";
}

```

can now be condensed to: 

```
public string GetPhoneNumber () => "867-5309"
```

If you need a method stub, because you don't know how to complete the method, and it was appropriate to use an Expression Bodied Member, you were left with two possibilities as throwing an expression wasn't allowed by the compiler.

```
public string GetPhoneNumber () => return null; //or String.Empty
```

or

```
public string GetPhoneNumber () {throw NotImplementedException();}
```

The first is error prone, because if program calls the method, there is no indication that it isn't functioning properly.  (Is null an expected return or an indicator of an error?)  The second is better, but it is a little cumbersome that you must convert it to a standard function just to throw the exception.  C# 7 solves this inconvenience and is now possible to throw exceptions in the Expression Bodied Member. 

```
public string GetPhoneNumber () => throw new NotImplementedException();
```

