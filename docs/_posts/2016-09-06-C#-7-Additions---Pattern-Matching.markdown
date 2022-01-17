---
layout: post
title: "C# 7 Additions - Pattern Matching"
date: 2016-09-06 00:00:00 -0500

---

C# 7 has started to introduce Pattern Matching.  This is a concept found in functional programming, and although it isn't fully implemented compared to F#, it is a step in that direction.  Microsoft has announced they intend on expanding it in future releases. 

<h4>Constant Patterns</h4> 
The **is** keyword has been expanded to allow all constants on the right side of the operator instead of just a type.  Previously, C#'s only valid syntax was similar to: 

```
public bool IsInt32 (object obj)
{
  return (obj is Int32);
}
```


Now it is possible to compare a variable to anything which is a constant: null, a value, etc. 

```
public bool IsEqualToFive (object obj)
{
  return (obj is 5);
}
//or 
public bool IsNull (object obj)
{
  return (obj is null);
}

```

Behind the scenes, the **is** statement is converted to calling the **Equals** function in IL code.  The following two functions produce roughly the same code (they call different overloads of the Equals function). 


```

public static bool CheckIsNull(object i)
{
  return (i is null);
}

public static bool CheckEqualsNull(object i)
{
  return (i.Equals(null));
}

```


CheckIsNull

```

.method public hidebysig static bool  CheckIsNull(object i) cil managed
{
  // Code size       13 (0xd)
  .maxstack  2
  .locals init ([0] bool V_0)
  IL_0000:  nop
  IL_0001:  ldarg.0
  IL_0002:  ldnull
  IL_0003:  call       bool [mscorlib]System.Object::Equals(object,
                                                            object)
  IL_0008:  stloc.0
  IL_0009:  br.s       IL_000b
  IL_000b:  ldloc.0
  IL_000c:  ret
} // end of method PatternMatching::CheckIsNull

```


CheckEqualsNull

```

.method public hidebysig static bool  CheckEqualsNull(object i) cil managed
{
  // Code size       13 (0xd)
  .maxstack  2
  .locals init ([0] bool V_0)
  IL_0000:  nop
  IL_0001:  ldarg.0
  IL_0002:  ldnull
  IL_0003:  callvirt   instance bool [mscorlib]System.Object::Equals(object)
  IL_0008:  stloc.0
  IL_0009:  br.s       IL_000b
  IL_000b:  ldloc.0
  IL_000c:  ret
} // end of method PatternMatching::CheckEqualsNull

```


This can also be combined with other features allowing variable assignment through the **is** operator.


```

public static void WritePlusFiveIfInt(object possibleNumber)
{
  if(possibleNumber is int i)
  {
    Console.WriteLine(i + 5);
  }
  else
  {
    Console.WriteLine("possibleNumber is not an int");
  }
}

```


In Visual Studio Preview 4, the scoping rules surrounding variables assigned in this manner are more restrictive than in the final version.  Right now, they can only be used within the scope of the conditional statement. 

<h4>Switch Statements</h4>
The new pattern matching extensions have also extended and changed the use of **case** statements.  Patterns can now be used in switch statements.  


```

public static void Check(object i)
{
  switch (i)
  {
    default:
      Console.WriteLine("unknown type.");
    break;

    case int n:
      Console.WriteLine("I am an integer");
    break;
  }
}

```


Like in previous versions, the default statement will always be evaluated last, but the location of the other case statements now matter.  

```

public static void Check(object i)
{

  switch (i)
  {
     case object x:
       Console.WriteLine("unknown type.");
     break;

     case int n:
      Console.WriteLine("I am an integer");
      break;
 }

}

```

In this example, **case int n** will never evaluate, because the statement above it will always be true.  Fortunately, the C# compiler will evaluate this, determine that it can't be reached and raise a compiler error.  

```

public static void WriteIntRange (int i)
{
    switch (i)
    {
      case int n when (n >= 100):
        Console.WriteLine($"I am above 100: {n}");
      break;

      case int n when (n < 100 && n >= 50 ):
        Console.WriteLine($"I am between 100 and 50: {n}");
      break;

      case int n when (n < 50):
        Console.WriteLine($"I am between 100 and 50: {n}");
      break;
    }
}

```


The variables declared in patterns behave differently than others.  Each variable in a pattern can have the same name without running into a collision with other statements.  Just as before, in order to declare a variable of the same name inside the case statement, you must still explicitly enforce scope by adding braces ({}).  

```

case int n when (n >= 100):
{
  var statement = $"I am above 100: {n}";
  Console.WriteLine(statement);
  break;
}

case int n when (n < 100 && n >= 50 ):
{
  var statement = $"I am between 100 and 50: {n}";
  Console.WriteLine(statement);
  break;
}

```


Pattern matching has a ways to go when compared to its functional language equivalent, but it is still a nice addition and will become more complete as the language evolves. 