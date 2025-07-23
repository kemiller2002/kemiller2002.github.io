---
layout: post
title: "JSIooa nvrtai Sncgtpi -> Sorting In JavaScript"
date: 2014-04-06 00:00:00 -0500
---
JavaScript has built in functionality to sort arrays, but it doesn't always work the way you think it would.  

Take the following array: 
```
5,9,7,4,2,3,15,8,7,2,undefined,undefined
```


Using the Array.Sort function (which all arrays have because of inheritance)


```

var arr = [5,9,7,4,2,3,15,8,7,2,undefined,undefined];
arr.sort();

```


Orders the array in the following order: 


```

15,2,2,3,4,5,7,7,8,9,undefined,undefined

```


<a href="http://jsfiddle.net/6N7JQ/4/" title="Running Example" target="_blank">Default Sort Running Example</a>

Even though the array is numerical, the default Array.Sort places the 15 first in the series.  The default comparison function sorts values by their Unicode characters and not numerical ones.  In short, it treats them like strings. 

There is an easy fix, the Array.Sort method takes a function as a parameter which allows you to define how the comparison elements are treated: 


```

function compare(a,b){
    if(a > b){return 1;}
    if(b > a){return -1;}
    return 0;
}

//the short hand function for the sort is this: function (a,b){return a-b}
//but this example makes it a little easier to see what's going on.

arr.sort(compare);

```


Now the sort order comes out as: 


```
2,2,3,4,5,7,7,8,9,15,undefined,undefined
```

<a href="http://jsfiddle.net/6N7JQ/5/" title="Custom Sort Running Example" target="_blank">Custom Sort Running Example</a>


Great!  Now the numbers are sorting by their numerical value.  Now what if you want the undefined values to appear first in the array?  This **should** be an easy change:


```

function compare(a,b){
    if(a > b || a === undefined){return 1;}
    if(b > a || b === undefined){return -1;}
    return 0;
}

```


The sort order?  It comes out exactly as before: 

```
2,2,3,4,5,7,7,8,9,15,undefined,undefined
```


<a href="http://jsfiddle.net/6N7JQ/7/" title="Put Undefined First Example" target="_blank">Put Undefined First Example</a>

So what's going on?

<a href="http://www.ecma-international.org/ecma-262/5.1/#sec-15.4.4.11" title="JavaScript Sort Order Spec" target="_blank">JavaScript Sort Order Specification</a>

If you look at steps 11 and 12 in the sort order steps in the JavaScript (ECMA Script) specification:

<blockquote>
If x is undefined, return 1.
If y is undefined, return −1.
</blockquote>

JavaScript automatically handles undefined values when comparing elements in an array.  There is no way to override this with a custom comparison function, because the undefined check happens before the comparison is called.   

There is a way around this predicament, although it's not ideal.  The current comparison function sorts the numbers in ascending order, and automatically forces undefined values to be last.  By changing the way the comparison to put the values in descending order, it forces the elements in the array to be in the correct position in regards to one another but in reverse.  To fix the reverse order problem, simply call <strong>Array.Reverse()</strong> and the order comes out the following way: 


```

function compare(a,b){
    if(a < b){return 1;}
    if(b < a){return -1;}
    return 0;
}

arr.sort(compare);
arr.reverse();

```



```
undefined,undefined,2,2,3,4,5,7,7,8,9,15
```


This solution causes extra work, and with large datasets the amount of extra time could become noticable, but it does work in a pinch. The other solution would be to completely ignore the Array.sort method and write a custom one.  



<a href="http://jsfiddle.net/6N7JQ/9/" title="Reverse Order" target="_blank">Sort In Reverse</a>