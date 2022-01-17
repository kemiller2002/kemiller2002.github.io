---
layout: post
title: "C# 7 Additions - Tuples"
date: 2016-08-27 00:00:00 -0500

---

In C# 7 Microsoft has introduced an updated Tuple type. It has a streamlined syntax compared to it's predecessor making it fall it look more like F#. Instead of declaring it like previous versions, the new Tuple type looks like: 

```
var myTuple = (1, "this is my string") //a tuple of type (int,string);

```

Likewise to declare it as a return type, the syntax is similar to declaring it:

```
public (int,string) GetWordCount() => (1, "test");

```

The first thing to note about the new type is that it is not included automatically in a new project. If you immediately use it, you'll see the following error.
<img src="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/C%23Seven/NativeHandlingTuplesFSharp.png" />

As of VS 15 preview 4 (not to be confused with VS 2015), you must include the <strong>System.ValueTuple</strong> Nuget package to take advantage of it.

```
Install-Package System.ValueTuple -Pre
```


This raises the question about how the new Tuple type and the previous one included since .NET 4 are related? They're not. They are treated as two different types and are not compatible with each other.  <a href="https://msdn.microsoft.com/en-us/library/system.tuple(v=vs.110).aspx">System.Tuple</a> is a reference type and System. ValueTuple is a value type.

<img src="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/C%23Seven/TuplesNotTheSame.png" />

So what are advantages over the previous version? The syntax simpler, and there are several other advantages.

<h4>Named Properties</h4>
In the System.Tuple version, properties of the return object were referenced as Item1, Item2 etc. This gets confusing when there are multiples of the same type in the Tuple as you have to know what position had which value type.

```
public string GetName()
{
  var tuple = GetNameAddressPhone();
  return tuple.Item1; //or was it item2?
}

public Tuple<string,string,string> GetNameAddressPhone ()
{
  return new Tuple<string, string, string>
    ("867-5309", "Jenny", "Ny Ny");
}

```

Now it's possible to explicitly name the item types to reduce confusion.

```
public (string Phone, string Name, string Address)
  GetNameAdressAndPhoneByNewTuple() => 
    ("867-5309", "Jenny", "NY Ny");

public string GetName()
{
  var tuple = GetNameAdressAndPhoneByNewTuple();
  return tuple.Name;
}

```

The Item properties (Item1, Item2, etc.) have also been included allowing methods to be updated to the new type without breaking code based on it's predecessor.

It's also possible to explicitly name the values when creating the object:

```
public (int count, string name) GetWordCount(string message)
{
  var messageArray = message.Split(',');
  return (count:messageArray.Length, name:messageArray[0]);
}

```


<h4>Deconstruction</h4>
It is now possible to name and assign variable values upon creating (or returning) a tuple. Although not necessary, it reduces the amount of code necessary to pull values out of the type.

```
public (string, string) GetNameAndPhoneNumber ()
{
  (var phone, var name, var address) = GetNameAdressAndPhoneByNewTuple();
  return (name, phone);
}

```

It's not certain if C# will get wildcards like F# to automatically discard values which aren't needed. If they are allowed then it's possible to only create a variable for the name like so:

```
public string GetName ()
{
  (*, var name, *) = GetNameAdressAndPhoneByNewTuple();
  return name;
}

```


<h4>Updating Values</h4>
System.Tuple is immutable.  Once created it's not possible to update any of the values.  This restriction has been removed in the new version.  From a purely functional perspective this could be considered a step backwards, but in C# many people find this approach more forgiving and beneficial.
<img src="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/C%23Seven/UpdateTuple.png" />

Like all value types, when it is passed into a method, a copy of the tuple is created, so modifying it in the in the method does not affect the original.

<img src="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/C%23Seven/TuplePassedThroughMethod.png" />

However if you compare two different tuples and they have the same values, the Equals method compares the values in each and if they are all equal, it considers them equal.

<img src="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/C%23Seven/DifferentTuplesAreEqual.png" />
<h3>Integrations with Other Languages</h3>
Unfortunately, C#'s new tuple type doesn't automatically allow it to translate tuples from F#.
<img src="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/C%23Seven/IntegrationWithFSharp.png" />

F# can't desconstruct the values like it can with it's native tuples, and to return it, you have to explicitly instantiate the object type and add the values.
<img src="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/C%23Seven/NativeHandlingTuplesFSharp.png" />

Either way, the translation to F# isn't horrible as it acts like any other object passed to it by C#.