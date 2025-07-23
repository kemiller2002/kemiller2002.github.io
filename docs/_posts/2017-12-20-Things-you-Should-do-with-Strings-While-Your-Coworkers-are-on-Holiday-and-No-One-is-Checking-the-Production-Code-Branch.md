---
layout: post
title: "Things you Should do with Strings While Your Coworkers are on Holiday and No One is Checking the Production Code Branch"
date: 2017-12-20 00:00:00 -0500
---
Hi everyone!  This is part of the really cool new <a href="https://crosscuttingconcerns.com/The-First-C-Advent-Calendar" rel="noopener noreferrer" target="_blank">CS Advent Calendar</a> run by Matthew Groves!  Go check out all the really great articles by everyone!

In a non infrequent basis, interviewers ask the question, "What is a string?" and they are looking for a quick answer similar to, "It is an immutable reference type."  This normally, sparks follow up questions such as,  "explain what immutable means in this scenario," or "so are there any examples where you can change a string?"  The most common answers is, "No," and with good reason.  Adding two Strings together creates a new third String.  Calling methods like **ToUpper()** doesn't modify the one being operated on. It creates a new string, and although strings can be treated like an array of characters, the compiler prevents the modification of those characters in their specific positions.

```
public class ImmutableStringsExample
{
  public readonly string SeasonsGreetings = "Bah Humbug!!!!!";
  public unsafe void MutateSeasonsGreetingsString ()
  {
    var happyHolidays = "Happy Holidays!";

    fixed (char* seasonsGreetingsLocation = SeasonsGreetings)
    {
      for(var ii = 0; ii &lt; happyHolidays.Length; ii++)
      {
        seasonsGreetingsLocation[ii] = happyHolidays[ii];
      }
    }
  }
}

```