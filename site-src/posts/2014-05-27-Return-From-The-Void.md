---
layout: post
title: "Return From The Void"
date: 2014-05-27 00:00:00 -0500
---
In a recent project, we were tasked with making a service layer more manageable and efficient, and with a tight deadline each minute needed to count.  My teammate and I quickly moved methods around, created new classes where necessary, and implemented the <a href="https://code.google.com/p/dapper-dot-net/" title="Dapper Link" target="_blank">Dapper</a> object mapper.  About half way into the final testing, my coworker looked at me and said, "The end date for a workflow isn't updating.  Do you think it's a caching problem?"  After looking through the code, I quickly found the issue.  The SQL update procedure inside the update function wasn't called.  Fortunately, it was a simple fix, but it occurred to me we used this pattern dozens of times in the service tier.  How many other places was this exact problem occurring that we haven't found?  

Testing caught the errors, but, the time spent tracking down this type of issue should have been avoided.  I designed the update functions to return void instead of boolean or integer and relied on the implicit success of a method which solely performs a side effect (which functions that return void are) to indicate it executed correctly.  I forgot when a method doesn't throw an exception, it doesn't mean the intended action was successful but only that it didn't explicitly fail.  

Take for example, the following two functions: 


```

public void UpdateDate(DateTime endDate)
{
     //Update Record returns an int, but since the return type 
     //is void we're going to ignore it.
     UpdateRecord(endDate); 
}

```


and


```

public void UpdateDate(DateTime endDate)
{
   //We need to do something here. 
}

```


To the calling function, there is no difference.  They both take the same parameter and return the same type (or not at all in this case), but to the overall system, there is a huge difference.  One correctly updates the value, and the other doesn't do anything and indicates nothing is wrong.  

Applying the functional principal of always having an output from a function easily helps this issue: 


```

public int UpdateDate(DateTime endDate)
{
     //Update Record returns an int.
     return UpdateRecord(endDate); 
}

```


and


```

public int UpdateDate(DateTime endDate)
{
     //We need to do something here
     //and this won't compile until we do!
}

```


Now the application won't compile until the function is written to perform an action.  The compiler helps enforce correct code.  

In truth, there are ways to get around the compiler error and still have a non-functioning program.  This is a common pattern seen over and over: 


```

public int UpdateDate(DateTime endDate)
{
     var numberOfRecordsUpdated = 0;

     //Update Record returns an int.
     numberOfRecordsUpdated = UpdateRecord(endDate); 

     return numberOfRecordsUpdated;
}

```


Comment out the change to <strong>numberOfRecordsUpdated</strong>, and the compiler states the code is valid.  By applying another functional pattern, this problem code is also precluded.  Enforcing the rule that variables should be immutable, removes this problem pattern since <strong>numberOfRecordsUpdated</strong> couldn't be changed once it's instantiated which means that it can't have a default initialization of zero.