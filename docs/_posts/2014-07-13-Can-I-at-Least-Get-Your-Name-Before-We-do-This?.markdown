---
layout: post
title: "Can I at Least Get Your Name Before We do This?"
date: 2014-07-13 00:00:00 -0500

---

As applications grow in size and age, they can become difficult to navigate and unruly to manage.  Most organizations give little thought to spending extra time to maintain existing code to a certain level of quality, and even less thought to spend money to rewrite or even refactor code to keep it maintainable.  Fortunately, in .NET 4.5 Microsoft added a significant piece of functionality to at least help gather data on application flow.  In the <strong>System.Runtime.CompilerServices</strong> namespace, 3 new attributes were added to help log calling function information.  
<ul>
     <li>CallerFilePathAttribute</li>
     <li>CallerMemberNameAttribute</li>
     <li>CallerLineNumberAttribute</li>
</ul>

Add the attributes to the appropriate parameters in a method to use them.  


```

public static void Log(string message, 
                [CallerMemberNameAttribute]string methodName = "No Name Given", 
                [CallerFilePathAttribute]string filePath = "No File Path Given", 
                [CallerLineNumberAttribute]int lineNumber = -1)
{
    Console.WriteLine("Message: " + message);
    Console.WriteLine("\tFile Path: " + filePath);
    Console.WriteLine("\tLine Number: " + lineNumber);
    Console.WriteLine("\tMethod Name: " + methodName);
}

```


<img src="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/CanIAtLeastGetYourName/Images/Console.jpg" alt="Console Output" />


The great part about this is that it will populate the information without modification to the calling methods' source code.  You can add parameters with these attributes to existing methods, and they will automatically populate.  There are a couple of quirks though.  

<h3>Optional Parameters </h3>  In order for the application to compile, you must mark these as <a href="http://msdn.microsoft.com/en-us/library/dd264739.aspx" title="Optional Parameters" target="_blank">optional parameters</a>.  After looking at how the information is resolved at compile time, this makes sense.  If you notice in ILSpy, the attribute is added to the **called** method parameter, but the compiler generates the parameter values in the **calling** method.  This means the compiler accessing the called method must be aware the attribute has been added to the parameter (meaning this feature will require a recompile of the assemblies calling these methods), and it must understand it needs to add the caller method information when invoking.  An optional parameter is required to signify to the compiler that the source file is not missing parameter information (which would result in an error), and that the calling method expects the compiler to add the correct default values. 

In addition to this, it's a smart idea anyway, because not all .NET languages understand these attributes.  For example, calling the C# method from F# doesn't give the expected values.  It inserts the default ones.  F# doesn't understand that when invoking this C# method, it should replace the optional parameter values with the calling ones.  

<img src="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/CanIAtLeastGetYourName/Images/FsharpConsole.jpg" alt="F# Console Output" />

<h3>Namespaces</h3>
When using <strong>CallerMemberNameAttribute</strong> namespaces aren't provided.  This means that if you have methods with the same name in different classes or namespaces, there is no differentiation between them in the information passed to the called method.  You can get around this by using the other two method parameters, which will give you the file of the calling method, and even the line where the called function was invoked, but without them there is a level of uncertainty. 

<h3>The parameters can be overridden</h3>
If you explicitly set the parameter's value, the calling method's information won't be sent. 

```
StructuredSight.CallerExample.Logging.Logger.Log("Application Start", 
     "I AM GOING TO OVERRIDE THIS NAME");
```


Depending on the scenario, this can be both an advantage and a disadvantage.  If you need to override it and pass in different information you can, but this also means you can't add this attribute to an existing parameter and have it override what the calling method is already passing.

<h3>Reflection</h3>
Reflection, doesn't work at all with this feature.  Calling parameters are a compile time feature, and reflection is a runtime one.  If you call the method with these attributes directly, the compiler won't know to add them, so you're on your own to supply the information.  (Reflection with optional parameters will work by sending <a href="http://msdn.microsoft.com/en-us/library/system.type.missing.aspx" title="Type.Missing" target="_blank">Type.Missing</a>, but it won't get you the caller values). 

<h3>Speed</h3>
Actually, there is no slow down when using this, because it's all resolved at compile time.  Using ILSpy reveals there is nothing like reflection or other runtime resolutions involved. (This actually makes a lot of sense, because reflection doesn't have information like the code file path or line numbers of the calling function, so it has to be resolved at compile time.):

```

namespace StructuredSight.CallerExample
{
    internal class MoreBusinessLogic
    {
        public void DoApplicationThings()
        {
            Logger.Log(
            "More Business Logic", 
            "DoApplicationThings",
            "c:\\Users\\Kevin\\Documents\\GitHub\\StructuredSight\\CanIAtLeastGetYourName\\CallingName\\MoreBusinessLogic.cs", 
            13);
        }
    }
}

```


The new feature is great, but it has it's limitations.  Considering that it's possible to add the attributes to existing methods, and as long as the invoking methods are recompiled and the compiler is aware of how to insert the information, a wealth of information is available with very little work.  All around, it's a significant win for helping maintain applications.

<a href="https://github.com/kemiller2002/StructuredSight/tree/master/CanIAtLeastGetYourName" title="Project Contents" target="_blank">You can all the project contents here. </a>
