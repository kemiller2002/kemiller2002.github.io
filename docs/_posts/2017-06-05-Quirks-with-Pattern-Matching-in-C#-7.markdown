---
layout: post
title: "Quirks with Pattern Matching in C# 7"
date: 2017-06-05 00:00:00 -0500

---

With C# 7, Microsoft added the concept of pattern matching by enhancing the switch statement.  Compared to functional languages (both pure and impure), this seems to be somewhat lacking in a feature by feature comparison, however it is still nice in allowing a cleaner format of code.  With this, there are some interesting quirks, that you should be aware of before using.  Nothing they've added breaks existing rules of the language, and with a thorough understanding how the language behaves their choices make sense, but there are some gotchas that on the surface looks like they should function one way, but act in a completely different manner.  

Consider the following example.

```

static void BaseExampleIsString ()
{
  string jennysNumber = "867-5309";

  if(jennysNumber is string)
  {
    Console.WriteLine("The variable is a string");
  }

  switch (jennysNumber)
  {
    case string s:
      Console.WriteLine(
        "The switch statement recognizes the variable is a string: " 
        + s);
    break;
    default:
      Console.WriteLine("The default statement was triggered");
    break;
  }
}

```


<img src="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/PatternMatchingQuirks_Standard/Images/BaseExampleIsString.png" alt="Shows "IS" and pattern matching work on type string." />

C# 7 now allows the use of a switch statement to determine the type of a variable.  It as also expanded the use of **is** to include constants including null.


```

public static void ShowCanDetermineNull ()
{
  string jennysNumber = null;

  Console.WriteLine(jennysNumber is null);
}

```


<img src="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/PatternMatchingQuirks_Standard/Images/ShowIsCanShowNull.png" alt="is can show if something is null : shows true" />

With these two understandings, which line executes in the following code? 


```

static void BaseExampleNull ()
{
  string jennysNumber = null;
  switch (jennysNumber)
  {
    case string s when (s is null):
      Console.WriteLine("The variable is of type string and is null");
    break;
    case string s:
      Console.WriteLine("The variable is of type string");
    break;
    default:
      Console.WriteLine("This is the default statement.");
    break;
  }
}

```


<img src="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/PatternMatchingQuirks_Standard/Images/BaseExampleNull.png" alt="Shows default code executed." />

Based on the previous examples, its a reasonable conclusion that the one of the first two case statements would execute, but they don't.

<h3>The **is** operator</h3>
The **is** operator was introduced in C# 1.0, and its use has been expanded, but none of the existing functionality has changed.  Up until C# 7, **is** has been used to determine if an object is of a certain type like so.  


```

public static void ShowIsType ()
{
  string jennysNumber = "867-5309";

  Console.WriteLine(jennysNumber is string);
}

```


This outputs exactly as expected.  The console prints "True"  (Replacing **string** with **var** works the exactly the same.  Remember that the object is still typed. **var** only tells the compiler to figure out what type the variable should be instead of explicitly telling it.)

<img src="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/PatternMatchingQuirks_Standard/Images/IsOperatorShowString.png" alt="Is Operator String: True" />

What happens if the string is null?  The compiler thinks its a string.  It will prevent you from being able to pass it to methods requiring another reference type even though the value is explicitly null. 


```

public static void ShowIsTypeNull ()
{
  string jennysNumber = null;

  Console.WriteLine(jennysNumber is string);
}

```


<img src="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/PatternMatchingQuirks_Standard/Images/ShowTypeIsNull.png" alt="Type is null" />

The **is** operator is a run time check not a compile time one, and since it is null, the runtime doesn't know what type it is.  In this example, the compiler could give flags to the runtime saying what type it actually is even though it's null, but this would be difficult if not impossible for all scenarios, so for consistency, it still returns false.  Consistency is key. 

Printing out True and False is nice, but it's not really descriptive.  What about adding text to describe what is being evaluated. 


```

public static void ShowIsTypeWithQuestion ()
{
  string jennysNumber = "867-5309";

  Console.WriteLine("What is Jenny's Number? " + jennysNumber is string);
}

```


<img src="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/PatternMatchingQuirks_Standard/Images/ShowIsTypeWithQuestion.png" alt="Is Type With Question, Question doesn't appear" />

Why didn't the question appear?  It has to do with <a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/operators/index" target="_blank">operator precedence</a>.  The **+** has a higher operator precedence than **is** and is evaluated first.  What is actually happening is: 

```

public static void ShowIsTypeWithQuestion ()
{
  string jennysNumber = "867-5309";

  Console.WriteLine(
    ("What is Jenny's Number? " + jennysNumber) is string
  );
}

```


This becomes clear if the clause is flipped, because the compiler doesn't know how to evaluate **string** when using the **+** operator.

<img src="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/PatternMatchingQuirks_Standard/Images/QuestionFlipped.png" alt="Flipping clauses throws error." />

Adding parenthesis around the **jennysNumber is string** fixes the issue, because parenthesis have a higher operator precedence than the **+** operator. 


```

public static void ShowIsTypeWithQuestionFlipped()
{
  string jennysNumber = "867-5309";

  Console.WriteLine((jennysNumber is string) + " What is Jenny's Number? ");
}

```


<img src="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/PatternMatchingQuirks_Standard/Images/QuestionFlippedWithParenOutput.png" alt="output of is operator and + flipped with parenthesis (shows both question and value)" />


<h3>Pattern Matching with Switch Statements</h3>

<h4>Null and Dealing with Types</h4>
Null is an interesting case, because as shown during the runtime, it's difficult to determine what type an object is. 

<h5>Base Example</h5>


```

public static void NullExample ()
{
  string jennysNumber = null;
  switch (jennysNumber)
  {
    case string s:
      Console.WriteLine("It's a string and it's null");
    break;

    case null:
      Console.WriteLine("There is no Jenny's Number");
    break;

    default:
      Console.WriteLine("This is the default case.");
    break;
  }
}

```


This code works exactly as how you think it should.  Even though the type is **string**, the runtime can't define it as such, and so it skips the first case, and reaches the second.  

Adding a type object clause works exactly the same way

```

public static void NullWithObjectExample ()
{
  Object jennysNumber = null;

  switch (jennysNumber)
  {
    case string s:
      Console.WriteLine("This is a string");
    break;
    case Object o:
      Console.WriteLine("It's an object");
    break;
    case null:
      Console.WriteLine("This is the null case");
    break;
  }
}

```


<img src="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/PatternMatchingQuirks_Standard/Images/ObjectNullCase.png" alt="shows object case works same way" />

What about **var**.  Case statements now support **var** as a proposed type in the statement.

```

public static void VarCase()
{
  String jennysNumber = null;

  switch (jennysNumber)
  {
     case string s:
      Console.WriteLine("This is a string");
     break;
     case var o:
      Console.WriteLine("var option has been hit. Is it type string?");
      Console.WriteLine(o is string);
     break;
  }
}

```


If you mouse over either **var** or the variable name, the compiler will tell you what type it is.  
<img src="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/PatternMatchingQuirks_Standard/Images/MouseOverOKnowsItsTypeString.png" alt="show compiler knows what type it is." />

<img src="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/PatternMatchingQuirks_Standard/Images/VarTypeHitDoesntKnowItsString.png" alt="Shows var case statement doesn't know type" />

It knows what the type is, but don't let this fool you into thinking it works like the other typed statements though.  The **var** statement doesn't care that the runtime can't determine the type.  A case statement with the **var** type will always execute provided there is no condition forbidding null values **when (o != null)**.  Like before, it still can't determine the type inside the case statement statement. 

<h4>Why determine object type at compile time?</h4>
At any point in time (baring the use of dynamic), the compiler knows the immediate type of the variable.  It could use this to directly point the correct case concerning the type.  If that were true, it couldn't handle the following scenario, or any concerning inheritance of child types. 


```

public static void ObjectOrString ()
{
  Object jennysNumber = "867-5309";

  switch (jennysNumber)
  {
    case string s:
      Console.WriteLine("This is a string");
    break;
    case Object o:
      Console.WriteLine("This is an object");
    break;
   default:
      Console.WriteLine("This is the default case");
    break;
  }
}

```


<img src="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/PatternMatchingQuirks_Standard/Images/ObjectOrString.png" alt="shows is string" />

Personally, I would like to see either a warning or an error, that it's not possible for type cases to determine if the variable is null **case string s when (s is null)**, but as long as the code is tested and developers knows about this edge case, problems can be minimized. 

All the examples can be found on github: <a href="https://github.com/kemiller2002/StructuredSight/tree/master/PatternMatchingQuirks_Standard" target="_blank">https://github.com/kemiller2002/StructuredSight/tree/master/PatternMatchingQuirks_Standard</a>