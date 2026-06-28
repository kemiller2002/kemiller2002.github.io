---
layout: post
title: "I thought (var) Gozer was a man.  It's whatever it wants to be."
date: 2023-10-04 00:00:00 -0500
categories:
  - types
  - JavaScript
published: false
---
Tom Hacohen wrote a great [post](https://www.svix.com/blog/strong-typing-hill-to-die-on/) about his view of types and their place in development. At the end, he asked for different views, and so I thought I would oblige and participate.

Admittedly, my view of types and their necessity has changed over the past couple of years. JavaScript has been the weakly typed/untyped language I've used the most. On the other side, I've used C#, F#, and TypeScript quite extensively (others too but not worth mentioning). I've left the "what is right/wrong" camps to really what works for the situation. I think that strong typing is great and it definitely has a place, but I will say that personally at this time I program a lot in JavaScript, and I have dispensed with needing them to be effective.

No decision comes without a price, and the ultimate question is it worth paying. Types will help in catching certain errors. They do allow developers to cognitively offload the need to validate if the function is going to have any chance behaving correctly when passing it a parameter. The drawback to this is that in order to make functions more generic, objects passed to the function require a binding commonality: interface, inheritance, etc.

```csharp
    function FormattedPhoneNumber FormatNameAndPhoneNumber (INameAndPhoneNumber nameAndNumber){
        return  $"{nameAndNumber.Name}, {nameAndNumber.phoneNumber}"
    }

```

In this example it's possible to avoid the use of a complex type and separate the parts into two parameters.

```csharp
    function string FormatNameAndPhoneNumber (string name, string phoneNumber){
        return  $"{name}, {phoneNumber}"
    }
```

<!--

Let's take the following (slightly outrageous example). The system has a function that formats an employee's name to put it on a badge for a party.

```csharp
    public class Employee {
        Name Name {get; set;}
    }

    public string FormatNameForWelcomeBadge (Employee registrant){
        return `Hi! My name is: ${registrant.Name}.  Nice to meet you.`
    }
```

At some point in time, the business decides they would also like to include contractors and format their names in the same manner.

```csharp
    public class Contractor {
        Name Name {get; set;}
    }
```

With this example, there are a few obvious solutions.

1.  Add an interface. Create an interface with the property Name as a requirement.
2.  Potentially have both classes inherit from a base class named Person.
3.  Alter the method signature to pass in the string version of the name and not worry about calling the object.

This is assuming you can modify the classes and they aren't imported from an external library.

-->
