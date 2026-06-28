---
layout: post
title: "C# 7 Additions – Out Variables"
date: 2016-08-30
---

C# 7 removes the need for out variables to be predeclared before passing them into a function.  


```

public static string GetPhoneNumber(string name)
{
  Dictionary<string, string> phoneBook = new Dictionary<string, string>();
  phoneBook.Add("jenny", "867-5309");
  
  if (phoneBook.TryGetValue(name, out var phoneNumber))
  {
      return phoneNumber;
  }
  
  throw new Exception("phone number not found for: {name}");
}

```


It also now allows the use of the <strong>var</strong> keyword to declare the variable type, because the compiler will infer the type based on the declared parameter type. This is not allowed when the compiler can't infer the type because of method overloading.  It would be nice if the compiler would attempt to infer it's type based on the use later on in the method similar to F#'s inferred types, but this isn't slated to be in the current release. 

<img src="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/C%23Seven/OutVariableConfused.png" alt="compiler confused because of method overloading. " />

In Visual Studio 15 Preview 4, the out variable isn't working exactly as it will in the final release.  Wild cards will hopefully be added so extraneous variables don't need to be declared. 


```

public void GetFirstNameAndLast(out string firstName, out string lastName)
{
  firstName = "Jenny";
  lastName = "?";
}
 . . . . 
GetFirstNameAndLast(out string firstName, *);
//Don't need the last name, so use a wildcard (*) instead;

```


The following code won't work until the scope restrictions on out variables is updated.  (They have said they intend on doing this before the release.)


```

public void GetFirstNameAndLast(out string firstName, out string lastName)
{
  firstName = "Jenny";
  lastName = "?";
}

public void PrintFirstName()
{
  GetFirstNameAndLast(out string firstName, out string lastName);
  Console.WriteLine(firstName);
}

```


In this example, the scope is limited to the method call where the strings are set.  To get it work currently, variable scope must be extended and can be like so: 


```


public bool GetFirstNameAndLast(out string firstName, out string lastName)
{
  firstName = "Jenny";
  lastName = "?";

  return true;
}

public void PrintFirstName()
{
if (GetFirstNameAndLast(out string firstName, out string lastName))
  {
   Console.WriteLine(firstName);
  }
}

```


The conditional statement wraps the variables and they can now be used in the <strong>Console.WriteLine</strong>.  This will be corrected in the final release and won't be necessary.