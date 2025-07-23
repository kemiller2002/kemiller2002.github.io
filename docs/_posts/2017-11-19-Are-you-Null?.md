---
layout: post
title: "Are you Null?"
date: 2017-11-19 00:00:00 -0500
---
Within the last couple of days Microsoft released a proposed update for the next major release of C# version 8.  Over the past several years, there has been a large debate on the existence and use of **null** in software development.  Allowing **null** has been heralded as the billion dollar mistake by the null reference inventor, <a href="https://en.wikipedia.org/wiki/Tony_Hoare" rel="noopener" target="_blank">Sir Tony Hare</a>.  With this, Microsoft has decided to help the C# community by adding functionality to the C# compiler to help point out where a null reference might occur.

With the release of C# 8, anything referencing an object (string, etc.) must explicitly declare itself as possibly being null, and if that variable isn't explicitly checked before being used, the compiler generates a warning that a possible null reference might occur.  So how does this work?  By using the **?** at the end of a reference type, it signifies the developer acknowledges null might occur.  


```

  String  NotNullVariable = "I'm not null";
  String? NullVariable = "I might be null";

```



```

  String? maybeNull = "I could be null"; 

  Console.WriteLine(maybeNull.Length); //Warning!
  
  if (maybeNull != null) 
  {
    Console.WriteLine("Null averted, proceed"); 
    Console.WriteLine(maybeNull.Length); //No warning here.
  }

```



This looks like it would be a breaking change, and all code written in a previous version will suddenly stop compiling.  This would be true except for two things. 

<ol>
  <li>You must use a compiler flag to enforce the rule.</li>
  <li>The flag will only generate warnings not errors.</li>
</ol>

So legacy code is safe in the upgrade process if it's too difficult to convert.

With this, they are still working out a number of scenarios that prove tricky to handle. These are things like default array initialization (new string[2]).  Their comments about all of these can be found on their <a href="https://blogs.msdn.microsoft.com/dotnet/2017/11/15/nullable-reference-types-in-csharp/" rel="noopener" target="_blank">blog on MSDN</a>

I've added their code examples below of edge cases they are still working on:


```

void M(Person p)
{
    p.FirstName = null;          // 1 WARNING: it's null
    p.LastName = p.MiddleName;   // 2 WARNING: may be null
    string s = default(string);  // 3 WARNING: it's null
    string[] a = new string[10]; // 4 ok: too common
}

struct PersonHandle
{
    public Person person;        // 5 ok: too common
}

class Person
{
    public string FirstName;     // 6 WARNING: uninitialized
    public string? MiddleName; 
    public string LastName;      // 6 WARNING: uninitialized
}

```


Personally, I hoped the compiler would enforce these rules a little stronger.  Some languages like F# strictly enforce variable immutability unless explicitly allowed, and other functional languages do not allow it at all.  

It is possible to turn on "Warnings as errors" and have the compiler stop if it encounters a possible null exception, but this assumes the rest of the code has no other warnings that won't stop compilation.  Ideally, no warning flags should ever appear in code without being fixed, but that is a very difficult standard follow when dealing with legacy code from years past where no one followed that rule before you. Either way, the C# team was in a tight situation, and they did the best they could.  They needed to make strides towards making null references easier to track, but they couldn't break all of the legacy code using previous versions of C#.