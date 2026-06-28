---
layout: post
title: "C# 7 Additions – ref Variables"
date: 2016-08-31
---

C# 7 expands the use of the ref keyword. Along with its previous use, it can now be used in return statements, and local variables can store a reference to the object as well. At first glance, the question is "What is the real difference between returning a ref variable, and setting it through an out parameter?" Previously you could set a variable passed into a function with ref (or out) to a different value. In C# 7, you can return the reference of a field, variable etc. and store that in a local variable for later use.

The following is an examples showing its expanded use.

```
class Program
{
  class PersonInformation
  {
    public String Name {get;set;}
    public String PhoneNumber {get;set;}

  }

  static void Main(string[] args)
  {
    var person = new PersonInformation 
      { Name = "jenny", PhoneNumber = "867-5309" };
    ref var name = ref GetName(person);
    MakeNameCapitalized(ref name);
    Console.WriteLine(person.Name); //Writes "Jenny"
  }

  static ref string GetName (PersonInformation person)
  {
    return ref person.Name;
  }

  public static void MakeNameCapitalized (ref string name)
  {
    name = name[0].ToString().ToUpper() + String.Join("", name.Skip(1));
  }
}

```

As expected, the PersonInformation object is passed into the **GetName** function which returns a reference to the string field **Name**. This is then passed into the **MakeCapitalized** function which capitalizes the name "jenny" (making it "Jenny") in the original **PersonInformation** object. Compare this to the example <a href="https://gist.github.com/kemiller2002/ae80551a36c8073403b03e720e30613b">here</a> showing how the previous version of C# would not allow the modification of the original field in the same scenario. 

<h4>Classes vs Structs</h4>
If the PersonInformation is changed to be a struct (value type) instead of a class (reference type), the following code won't work without a slight modification, but it is still completely possible.

```
class Program
{
  struct PersonInformation
  {
    public String Name;
    public String PhoneNumber;
  }

  static void Main(string[] args)
  {
    var person = new PersonInformation { Name = "jenny", PhoneNumber = "867-5309" };
    ref var name = ref GetName(ref person); //this needs to be a ref parameter then.
    MakeNameCapitalized(ref name);
    Console.WriteLine(person.Name);
  }

  static ref string GetName (ref PersonInformation person) //this must also allow ref parameters.
  {
    return ref person.Name;
  }

  public static void MakeNameCapitalized (ref string name)
  {
    name = name[0].ToString().ToUpper() + String.Join("", name.Skip(1));
  }
}

```

Structs are passed by value meaning that passing a struct into a method creates a copy of it. Returning a reference to the struct's field would return a reference to the copied struct and would go out of scope as soon as the method completes. There would be no point, and it would cause errors pointing to fields to objects which didn't exist.

<h4>Caveats</h4>
With these new features there are some restrictions to it. Consider this. A string can be treated as an array of characters. With the new functionality, it should be possible to pass back a reference to a character location in that string and update it, because you have the reference to the character location in the string.

```
public static ref char GetLocationOfFirstChar (string name) => ref name[0];

```

Fortunately, this isn't allowed. The compiler prevents from it being a valid option, because if this were possible, it would break the string's immutability and cause havoc with C#'s ability to <a href="https://msdn.microsoft.com/en-us/library/system.string.intern(v=vs.110).aspx">intern</a> strings.
<img src="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/C%23Seven/RefStringNotAllowed.png" alt="ref string not allowed." />

The compiler is also smart enough to not allow references to variables which fall out of scope. The following is also not allowed:

```
public static ref int ReturnRefOfNumber()
{
  var someNumber = 123;
  return ref someNumber;
}

```

After the method exits **someNumber** no longer exists, and when another part of the application tries to access it, it won't be available. (You could say this might not be the case if it were a reference type like a string, but it still wouldn't matter, because all the reference has is a location to where the object is, not the actual object itself.  This causes 2 problems: One, currently there is no way to get the value from the reference.  Two, the object isn't rooted, so it could still be garbage collected at any point in time.)  

The compiler is also smart enough to trace the variable use through the calling methods. This is also not allowed:

```
public static ref int ReturnRefOfNumber()
{
  var someNumber = 123;
  return ref PassSomeNumber(ref someNumber);
}

public static ref int PassSomeNumber(ref int number) => ref number;

```