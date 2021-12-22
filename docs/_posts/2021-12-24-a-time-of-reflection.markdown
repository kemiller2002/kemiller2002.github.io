---
layout: post
title: "A time of reflection"
date: 2021-12-24 00:00:00 -0500
categories: C#
---

They say the holidays should be a time where one looks back at the past year, or period of time in their life and reflects upon decisions and events in their lifetime. One is supposed to their successes and failure in a hope to gleam some insight into the highs and lows of times past in an effort to either duplicate or avoid such events. Both from a personal and societal standpoint, the location of where we stand today sometimes feels but just a few feet from where we were but contrarily it maybe a great distance from where we started.

One of the most amazing things about C#, and all credit goes to all the languages designers and programmers who've worked on it, is it's amazing ability to change and adapt to current trends and needs of the community. C# is over 20, and Microsoft started working on it's design years before it was ever mentioned to the public. Many languages over the years have waxed and waned during C#'s lifetime and several even though heralded as the next big shift in computer programming never were never granted a seat at the table of major languages sustained by community support. C#, even though it was backed by one of the largest software companies in the world, was never guaranteed that spot, and some argue, it almost never made it.

Back in the 90's, Microsoft was in the midst of a legal battle with Sun over trademark issues of Java and J++, Delphi, based off of Turbo Pascal, was a major player in the Rapid Application Development space (RAD for short), and there was this quirky language that allowed interactivity in these new web browsers called JavaScript. Amidst all of this, in mid 2000, Microsoft announced the future release of a new programming language called "C#" which looks nothing like their flagship RAD tool, Visual Basic arguably the most loved by some (and reviled by others) language of all time. The VB syntax in all it's forms powered most of Microsoft's lightweight programming tooling. Everything from VBScript in their Classic ASP system, to Office automation, to their RAD Tool which probably created the majority of workplace applications across the world. Although not officially stated, Microsoft had signaled a change in direction pertaining to their approach to software development.

Understanding the background, the stage is now set to see the world where C# 1.0 debuted. C# entered a world where for years there was a debate on whether anyone really needed any more programming languages, and on top of that, it looked extremely similar to a relatively young language . . . . Java. It was so similar in fact that even some of Java's quirks which could have been corrected, such as [array covariance, "https://csharp.2000things.com/2014/07/28/1147-why-generics-dont-support-covariance/"], weren't.

At this time, the C# language had several quirks which would be frightful at today. C# only had arrays, and as common in many languages, aren't natively expandable. .NET had the ArraList which was, but only if you could accept that all items in it were of type Object. This lead to this problem of "boxing" which although still prevalent today, is much less talked about than it was back in 2001. The concept of the .ini file was gone and replaced with a configuration file in XML, which a fair number of developers had never seen or understood how to use. The list goes on.

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

Although ridiculed by some as limiting, it has saved countless hours of having to track down accidental statements where a `break` line was missed. Similarly C# made the necessity of the "dreaded Yoda statement" `if(42 == answer)` where the constant is the left hand of the comparison operator obsolete, because having an assignment clause in a conditional statement is simply not allowed `if(answer = 42)`.

Around 2005 Microsoft took an [ambitious step, "https://docs.microsoft.com/en-us/archive/blogs/dsyme/netc-generics-history-some-photos-from-feb-1999"] and introduced the concept of Generics. Something that almost didn't make the cut, or might have looked completely different than it was today. This clearly reshaped the language's landscape for future development. Now one can't think of C# without generics and it's feature successors. Java, C++, Python, Rust, and Swift all have support for generics and without it, C#'s future would have been questionable.

During the same year, the legendary C++ programmer, Herb Sutter, wrote his iconic article, [The Free Lunch is Over, "http://www.gotw.ca/publications/concurrency-ddj.htm"], forecasting the downfall of the simple single threaded application when it comes to speed and scalability.
