---
layout: post
title: "Looking back at C#"
date: 2021-12-23 00:00:00 -0500
categories: c-sharp
permalink: /c-sharp/2021/12/24/reflection.html
---

They say the holidays should be a time where one looks back at the past year, or period of time in their life and reflects upon decisions and events in their lifetime. One is supposed ponder upon their successes and failure in a hope to gleam some insight into the highs and lows of times past in an effort to either duplicate or avoid such events. Both from a personal and societal standpoint, the location of where we stand today sometimes feels but just a few feet from where we were but contrarily it maybe a great distance from where we started.

One of the most amazing things about C#, is the hard work the designers put into changing and adapting to current trends and community requests. C# was officially released over 20 ago, and Microsoft started working on its design years before it was ever mentioned to the public. Many languages waxed and waned during C#'s lifetime and several were even heralded as the next big shift in computer programming. C#, even though it was backed by one of the largest software companies in the world, was never guaranteed that spot, and some argue, it almost never made it.

The 90's was really a different era of programming, both technology and culture wise. The internet had just taken off and started to become mainstream. The mouse wheel and infrared mouses were virtually unheard of. Microsoft was in the midst of a legal battle with Sun over trademark issues of Java and J++. Delphi, based off of Turbo Pascal, was a significant leader in the Rapid Application Development (RAD) space. There was this quirky language that allowed interactivity in these web browsers called JavaScript, and COBOL programmers and other people working on the year 2000 crisis, along with HTML developers were paid like rock stars. Along with all of this, [Microsoft saved Apple in 1997](https://appleinsider.com/articles/18/08/06/august-6-1997----the-day-apple-and-microsoft-made-peace).

In mid 2000, Microsoft announced the future release of a new programming language called "C#" and looked nothing like their flagship RAD tool, Visual Basic which was arguably the most loved (and reviled) language at that time. The VB syntax in all it's forms powered most of Microsoft's lightweight programming tooling. Everything from VBScript in their Classic ASP system, to Office automation, to their RAD Tool which probably created the majority of workplace applications across the world. Although not officially stated, Microsoft signaled a change in direction pertaining to their approach to software development with the release of C# and was fine leaving the "BEGIN/END" and "ON ERROR GOTO" syntax behind.[2]

Understanding the background, the stage is now set to see the world where C# 1.0 debuted. C# entered a world where for years there was a debate on whether anyone really needed any more programming languages, and on top of that, it looked extremely similar to a programming language of a rival company . . . . Java. It was so similar in fact even some of Java's shortcomings which could have been corrected, such as [array covariance](https://csharp.2000things.com/2014/07/28/1147-why-generics-dont-support-covariance/), weren't.[1]

Along with this, the C# language had several quirks which would be considered daunting at the time and would still be considered the same today.

- C# only had arrays, and as common in many languages, aren't natively extendable. There were helper functions at the time in the .NET framework, but it wasn't as easy as doing something like `List<T>.Add`. (The built in functions also created a new array, so if you were counting on references to the original array still working, you were out of luck.) .NET had the ArrayList which was, but only if you could accept that all items in it were of type Object.

- [Boxing](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/types/boxing-and-unboxing) which although still prevalent today, is much less talked about than it was back in 2001.

- Nullable values of [Value Types](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/value-types) (integers for instance) didn't exist. This left developers with thorny problem determining how to handle default values. Did you assume that when it was `0` that was just the default value, which works unless `0` is _actually_ within the range of acceptable values and not just a default. This led a lot of developers implementing the idea that another number, such as `-1`, which works until someone forgets to manually set the default value and you spend hours tracking down how an incorrect value popped up in the system.

- The concept of the .ini file was gone and replaced with a configuration file in XML, which a fair number of developers had never seen or understood how to use. Although it sounds rather silly now, this caused a fair number of developers a lot of confusion as the simple `key = value` was replaced with a much more verbose syntax.

With these quirks, the language team took several bold steps not seen in languages at the time to aid programmers in their day to day works. Although C# may not have invented some of these concepts, it evangelized several concepts that pushed it into popularity. Fall through in case statements is one such example:

```
    switch number:
    {
        case 0:
          DoSomething();
          //No break. NOT ALLOWED.
        case 1
          DoSomethingElse();
          break;
        ...
    }
```

Although ridiculed by some as limiting, it has saved countless hours of having to track down accidental statements where a `break` line was missed. Similarly C# made the necessity of the ["Yoda syntax"](https://en.wikipedia.org/wiki/Yoda_conditions) `if(42 == answer)` where the constant is the left hand of the comparison operator obsolete, because having an assignment clause in a conditional statement is simply not allowed `if(answer = 42)`. (They also decided not to allow something like `if(42)` and require `if(42 > 0)`, because it is a little confusing to people who don't understand that in other languages like JavaScript `if(-1)` evaluate to true because `-1` is not `0`.)

Around 2005 Microsoft took an [ambitious step](https://docs.microsoft.com/en-us/archive/blogs/dsyme/netc-generics-history-some-photos-from-feb-1999) and introduced the concept of Generics. Something that almost didn't make it, and might have looked completely different than it was today. This put it inline with other languages, Java being the main example which released a generics implementation the year before, and solidly reshaped the language's landscape for future development. Now one can't think of C# without generics and it's feature successors. C++, Python, Rust, Swift, and now Go all have support for generics and without it, C#'s future would have definitely been questionable. This was also seen as a significant in bridging the gap between purely academic languages and ones used in a professional setting.

During the same year, the legendary C++ programmer, Herb Sutter, wrote his iconic article, [The Free Lunch is Over](http://www.gotw.ca/publications/concurrency-ddj.htm), forecasting the downfall of applications relying on strictly the speed of the processor to solve computational issues. Microsoft realized that any type of parallel programming was difficult for (pretty much all) developers. Borrowing the concept from F# (which was ultimately influenced by earlier works in Haskell), C# gained the async/await model in 2011 (CTP) and officially in 2012. It's difficult to say where other languages drew their inspiration from, but several languages followed C# in adding this model:

- Python in 2015
- Typescript in 2015
- JavaScript in 2017
- Rust in 2019
- C++ in 2020
- Swift in 2021

Amongst all of this happening, over the years Microsoft also worked on reducing language terseness. It was always possible to pass functions as arguments in C#, but the syntax was cumbersome and prevented its wide spread adoption. The introduction of Lambda functions solved this problem. Getters and setters used to require a developer to manually create a variable and write the instructions to update an retrieve the value. On top of all of this, Microsoft rewrote the compiler to allow hooks into the build process for analysis and output.

[1] Microsoft did solve the array covariance problem with Generics. With the initial release of Generics, it was explicitly forbidden to do something like

```
public void AddEmployee(List<Employee> employees){}
...
public void AddManager () {
  List<Manager> managers = new List<Manager>();

  //Originally can't do this even though a Manager is of type Employee.
  AddEmployee(managers);
}
```

In certain instances type conversion for Generics, Delegates, etc. were relaxed under certain conditions based on how the type was used. Microsoft did then go back and update a significant portion of the .NET framework to allow these relaxed implicit reference conversion rules seamlessly.

[2] So yes, technically speaking, Microsoft did not officially abandon Visual Basic. They created VB.NET with some syntax changes, and agreed to "coevolve" it with C#. Back at the time though, they didn't officially say it, but the amount of time spent advertising and promoting it was far less. It was a common belief among many developers at the time, Microsoft had every intention of moving developers to C#.
