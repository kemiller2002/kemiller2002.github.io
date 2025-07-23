---
layout: post
title: "Looking back at C#"
date: 2021-12-23
---

#### Happy Holidays! This is my post for the [C# advent calendar](https://www.csadvent.christmas/)!

They say the holidays should be a time where one looks back at the past year, or period of time in their life and reflects upon decisions and events in their lifetime. One is supposed ponder their successes and failures in a hope to gleam some insight into the highs and lows of times past in an effort to either duplicate or avoid such events. Both from a personal and societal standpoint, the location of where we stand today sometimes feels but just a few feet from the beginning, but in reality it maybe a great distance from where we started.

One of the most amazing things about C#, is the hard work the designers put into changing and adapting to current trends and community difficulties. C# was officially released over 20 years ago, and Microsoft started working on its design long before it was ever mentioned to the public. Many languages waxed and waned during C#'s lifetime and several were even heralded as the next big shift in computer programming only to fall short and become footnotes in the annals of history. C#, even though it was backed by one of the largest software companies in the world, was never guaranteed to be successful, and at times looked like it would become a yet another passing development fad.

The 90's was really a different era of programming, both technologically and culturally. The internet had just taken off and started to become mainstream. The mouse wheel was virtually unheard of and trackballs were yet another item to clean and maintain. Microsoft was in the midst of a legal battle with Sun over trademark issues concerning Java and J++. Delphi, based off of Turbo Pascal, was a significant leader in the Rapid Application Development (RAD) space. A quirky programming language that allowed interactivity in web browsers called JavaScript was new to the scene. COBOL programmers and others working on the year 2000 crisis, along with HTML developers were paid like rock stars, and [Microsoft saved Apple from financial ruin](https://appleinsider.com/articles/18/08/06/august-6-1997----the-day-apple-and-microsoft-made-peace).

In middle of 2000, Microsoft announced the future release of a new programming language called "C#" resembling nothing like Visual Basic, their flagship RAD tool, which was arguably the most loved (and reviled) language at that time. The VB syntax in all its forms powered most of Microsoft's lightweight programming tooling. Everything from VBScript in their Classic ASP system, to Office automation, to their RAD Tool which probably created the majority of workplace applications across the world. Although not officially stated, Microsoft signaled a change in direction pertaining to their approach to software development with the release of C# and was fine leaving the `BEGIN/END` and `ON ERROR GOTO` syntax behind much to the chagrin of a lot of developers.[[1](#1)]

After glimpsing the history of the 90's, the stage is now set to see the world where C# 1.0 debuted. C# entered a world where an ongoing debate raged on whether the world really needed any more commercial programming languages, and on top of that, it looked extremely similar to a rival company's programming language . . . . Java. It was so similar even some of Java's shortcomings were knowingly duplicated, such as [array covariance](https://csharp.2000things.com/2014/07/28/1147-why-generics-dont-support-covariance/).[[2](#2)]

Along with this, the C# language had quirks which considered daunting at the time and some considered unusable today.

- C# only had arrays (meaning not lists), and in many languages, aren't natively extendable. There were helper functions at the time in the .NET Framework, but it wasn't as easy as doing something like `List<T>.Add`. (The built in functions worked on copying data to a new array, so if you were counting on references to the original array still working, you were out of luck. You also had to know the trade offs between time and space and their optimal approach.) .NET had the ArrayList which was natively extendable, but only if you could accept that all items in it were of type Object.

- [Boxing](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/types/boxing-and-unboxing) which although still exists today, is much less an issue than it was back in 2001.

- Nullable values of [Value Types](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/value-types) (integers for instance) didn't exist. This left developers with the thorny problem determining how to handle default values. Did you assume that when an integer value was `0` it was the default value? This works unless `0` is _actually_ within the range of acceptable values and not just a default. This led a lot of developers manually assign some number like `-1` as a default value each time, and it was a workable approach until someone forgets about setting the default value and you spend hours tracking down how a variable pops up as 0 in an incorrect moment.

- Although technically not strictly C#, the concept of the .ini file was replaced with a configuration file in XML, which a hordes of developers had never seen or understood how to use. Although it sounds rather silly now, this confusion as the simple `key = value` was replaced with a much more verbose syntax.

On the other hand, Microsoft took several bold steps not seen in languages at the time to aid programmers in their day to day work. Although C# may not have invented some or any of these concepts, it evangelized several of them that pushed it into popularity simply by gently directing developers into become more productive by removing common pitfalls. Fall through in case statements is one such example:

```
    switch number:
    {
        case 0:
          DoSomething();
          //No break statement. NOT ALLOWED.
        case 1
          DoSomethingElse();
          break;
        ...
    }
```

Although ridiculed by some as limiting, it saves countless hours of having to track down accidental statements where a `break` line was omitted. Similarly C# made the necessity of the ["Yoda syntax"](https://en.wikipedia.org/wiki/Yoda_conditions) `if(42 == answer)` where the constant is the left hand of the comparison operator obsolete because having an assignment clause in a conditional statement is not allowed `if(answer = 42)`. (They also decided not to allow something like `if(42)` and require `if(42 > 0)`, because it is a little confusing to people who don't understand why in other languages like JavaScript `if(-1)` evaluates to true.)

Around 2005 Microsoft took an [ambitious step](https://docs.microsoft.com/en-us/archive/blogs/dsyme/netc-generics-history-some-photos-from-feb-1999) and introduced the concept of Generics. Generics almost didn't make it into the language, and was in danger of looking different than it was today. C#'s implementation solidly reshaped the language's and the Common Language Runtime's landscape for future development giving a stronger feature set over Java's which came out the year before. C++, Python, Rust, Swift, and now Go all have support for generics now and without it, C#'s future would have definitely been questionable. Now one can't think of C# without generics and it's feature successors and was also seen as a significant step in bridging the gap between purely academic languages and ones used in a commercial setting.

During the same year, the legendary C++ programmer, Herb Sutter, wrote his iconic article, [The Free Lunch is Over](http://www.gotw.ca/publications/concurrency-ddj.htm), forecasting the downfall of systems relying on strictly the belief that processor speed would increase allowing software to run faster without additional optimizations. Microsoft realized that any type of non-synchronous (multithreading, asynchronous, and concurrent) programming was difficult for (pretty much all) developers. Borrowing the concept from F# (which was ultimately influenced by earlier works in Haskell), C# gained the async/await model in 2011 in the Community Technology Preview and officially in 2012. [[3](#3)] It's difficult to say where other languages drew their inspiration from, but several languages followed C# and added this pattern:

- Python in 2015
- Typescript in 2015
- JavaScript in 2017
- Rust in 2019
- C++ in 2020
- Swift in 2021

Amongst all of this happening, over the years Microsoft worked on reducing language terseness to increase readability and overall productivity from a strictly writing standpoint. It was always possible to pass functions as arguments in C#, but the [syntax](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/delegates/using-delegates) in C# 1.0 was cumbersome and prevented its wide spread adoption. The introduction of Lambda functions in 2007 (C# 3.0) solved this problem opening the way for widespread adoption of [LINQ's dot notation (or method syntax)](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/linq/query-syntax-and-method-syntax-in-linq). Getters and setters used to require a developer to manually create a backing variable and write the instructions to update an retrieve the value. On top of all of this, Microsoft rewrote the compiler to allow hooks into the build process for analysis and output. This just names a few along with recent additions of [Expression Bodied Members](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/statements-expressions-operators/expression-bodied-members) and several others.

Looking back over 20 years later, the C# does show significant roots from where it began but several aspects are entirely different as well. As opinions about programming theory evolved, it did too. Back in the 1990's Functional Programming was considered a largely academic pursuit but is very mainstream today. In it's inception, C# listed itself as an Object Oriented language but over the years changed to reflect a more Object Oriented first language with additions to allow a Functional nature of development. Some question if it is headed the same way of PERL, where it appears chaotic because of the sheer number of ways to accomplish the same task, but at he same time without adaptation, it's doomed to be shelved like so many predecessors before it.

<a name="1"></a>[1] So yes, technically speaking, Microsoft did not officially abandon Visual Basic. They created VB.NET with some syntax changes, and agreed to "coevolve" it with C#. Back at the time though, they didn't officially say it, but the amount of time spent advertising and promoting it was far less. It was a common belief among many developers at the time, Microsoft had every intention of moving developers to C#.

<a name="2"></a>[2] Microsoft did solve the array covariance problem with Generics. With their initial release, it was explicitly forbidden to do something like

```
public void AddEmployee(List<Employee> employees){}
...
public void AddManager () {
  List<Manager> managers = new List<Manager>();

  //Originally can't do this even though a Manager is of type Employee.
  AddEmployee(managers);
}
```

In certain instances, type conversion for Generics, Delegates, etc. were relaxed based on how the type was used. Microsoft did then go back and update a significant portion of the .NET framework to allow these relaxed implicit reference conversion rules seamlessly.

<a name="3"></a>[3] Microsoft did add the Task Parallel Library in 2010 to solve similar but different programming problems.