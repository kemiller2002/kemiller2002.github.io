---
layout: post
title: "C# 7 Additions – Deconstructors"
date: 2016-08-29
---

C# has a new type of method, the Deconstructor.  When a type implements this method type with the name of <strong>Deconstruct</strong>, multiple variables maybe directly assigned as a return type would.


```

class FirstAndLastName
{
 string _firstName;
 string _lastName;

 public FirstAndLastName(string firstName, string lastName)
 {
   _firstName = firstName;
   _lastName = lastName;
 }

 public void Deconstruct(out string firstName, out string lastName)
 {
   firstName = _firstName;
   lastName = _lastName;
 }

 public void Deconstruct(out string firstName) 
 {
   firstName = _firstName;
 }

}
......
void ShowExample ()
{
  var firstAndLastName = new FirstAndLastName("Jenny", "?");
  var (firstName, lastName) = firstAndLastName; //calls method Deconstruct.
}

```


The method must be named <strong>Deconstruct</strong> and have a return type of void.  The parameters to be assigned all must be <strong>out</strong> parameters, and because they are out parameters with a return type of void, C# allows the Deconstruct method to be overloaded solely based on these parameters.  This is how the new System.ValueTuple allows it's properties to be assigned to separate variables without assigning each one individually.  


```

public (string, string, string) GetNameAdressAndPhoneByNewTuple() 
  => ("867-5309", "Jenny", "NY Ny"); //new tuple declaration.

public (string, string) GetNameAndPhoneNumber ()
{
  (var phone, var name, var address) = GetNameAdressAndPhoneByNewTuple();
  //the tuple's properties are split into three variables. 

  return (name, phone);
}

```


Deconstruct also does not need to be directly attached to the class.  C# allows the method to be implemented as an extension method as well. 


```

static class FirstAndLastNameNoDeconstructorExtensions
{
  public static void Deconstruct
    (this FirstAndLastNameNoDeconstructor obj, out string first, out string last)
  {
    first = obj.First;
    last = obj.Last;
  }
    
}


class FirstAndLastNameNoDeconstructor
{
 public string First;
 public string Last;
 public FirstAndLastNameNoDeconstructor(string first, string last)
 {
   First = first;
   Last = last;
 }
}

....

public void ShowNoDeconstrcutor()
{
  var noDeconstructor = new FirstAndLastNameNoDeconstructor
    ("jenny", "no last name given");
  
  var (first, last) = noDeconstructor;
}

```



At the moment it is uncertain if wildcards will be added allowing unneeded variables to be omitted from being assigned.  This addition would allow the insertion of the <strong>*</strong> to indicate a parameter is not needed (similar to _ in F#)

```

(var phone, var name, *) = GetNameAdressAndPhoneByNewTuple();

```