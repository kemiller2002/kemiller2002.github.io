---
layout: post
title: "C# 7 Additions – Local Functions"
date: 2016-08-28
---

In C# 7 it is now possible to create a function within a function termed a Local Function.  This is for instances where a second function is helpful, but it's not really needed in the rest of the class.  It's created just like regular functions except in the middle of another function.


```

public static int AddFactorialNumbers (int number)
{
  int Multiply (int innerNumber)
  {
    return (innerNumber == 1) ? 1 : innerNumber * Multiply(innerNumber - 1);
  }
  return Multiply(number);
}

```


Just like normal functions, you can create expression bodied members as well

```

int Multiply (int innerNumber) => 
  (innerNumber == 1) ? 1 : innerNumber * Multiply(innerNumber - 1);

```


Local variables in the outer functions are accessible, and it's possible to embed local functions inside other local functions:

```

public static string MakeFirstCharacterUpperCase(string name)
{
 string MakeUpperCase()
 {
   string JoinString(IEnumerable<char> array) => String.Join("", array);
   return name[0].ToString().ToUpper() + JoinString(name.Skip(1));
 }

 return MakeUpperCase();
}

```


So how does it work?  Looking at the IL code, the compiler has converted the internal function into a private static one inside the class. 

<img src="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/C%23Seven/ILShowLocalFunction.png" alt="IL Code showing private static function" />

The name is generated at compile time, so it is not accessible to other methods, but it is still possible to access it through reflection with the <strong>private</strong> and <strong>static</strong> binding flags. 

<img src="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/C%23Seven/ReflectionOnLocalFunction.png" alt="reflection shows local function. " />

Someone I know asked what would be a good use case of Local Functions vs. Lambdas.  Lambdas can't contain enumerators, and by encasing an enumerations in a local function it allows others parts of the outer method to be eagerly evaluated.  For example, if you have a method which takes a parameter and returns an enumeration, the evaluation of the parameter won't occur until program starts to enumerate the collection. Encapsulating the enumeration in a local function allows the other parts of the outer function to be eagerly evaluated.  You can find an example of the difference between using one and not using one <a href="https://gist.github.com/kemiller2002/6c9c92dcaec057f756baf4a9df9757d4" target="_blank">here</a>.