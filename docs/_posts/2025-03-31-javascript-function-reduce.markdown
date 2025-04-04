---
layout: post
title: "Reduce over functions or Data, Data Everywhere"
date: 2025-03-22 00:00:00 -0500
categories: General
published: false
---

Walking through the concept of functions as data and applying it can lead to applying and manipluating different code. Assuming that code is a series of steps, then code can be applied as an array and then manipulated through techniquies like reduce. First, working up to understand reduce, look at the following code.

```
function addNumbersSequentially(n1, n2, n3, n4, n5, n6, n7) {
    const step1 = n1 + n2;
    const step2 = step1 + n3;
    const step3 = step2 + n4;
    const step4 = step3 + n5;
    const step5 = step4 + n6;
    const step6 = step5 + n7;

    return step6;
}

// Example usage:
const sumOutput = addNumbersSequentially(1, 2, 3, 4, 5, 6, 7);
```

Obviously this is inefficient, and it has multiple shortcomings, but it does show the easiest way to add numbers together one step at a time.

Putting the numbers together and sending them as an array, groups them together and shows they are part of a group. It also allows the program to determine the number of time rquired to add them all together. With this, we are taking our first step towards writing the program in a declarative style instead of listing out each individual step (imperative).

```
function addNumbersSequentially(numbersArray) {
    let result = 0;
    for(let ii = 0; ii < numbersArray.length; ii++){
        result += numbersArray[ii]
    }

    return result;
}

```

This is effective, but we are duplicating a process that is already included to JavaScript's functions. Reduce will do the same thing as above and once undesrstood it's application and pattern is often clearer.

Conceptually Array.Reduce is roughly:

```
function reduceEquivalent (array, accumulator, initialValue){
    let accumulatedValue = accumulator(initialValue);

    for(let ii = 0 ; ii < array.length; ii++){
        accumulatedValue = accumulator(accumulatedValue, array[ii])
    }

    return accumulatedValue;
}
```

So if we wanted to add a list of numbers in an array together with the above function it owuld look like:

```
function addNumbersAccumulator (currentTotal, number){
    return currentTotal + number
}
// or

const addNumbersAccumulator = (currentTotal,number) => currentTotal + number

```

to call the function it would be:

```

const result = reduceEquivalent([1, 2, 3, 4, 5, 6, 7], addNumbersAccumulator, 0);

console.log(result) //28

```

```
function addNumbersAccumulator (currentTotal, number){
    return currentTotal + number
}

function addNumbersSequentially(numbersArray) {
    return numbersArray.reduce(addNumbersAccumulator, 0);
}
```

```
// Example of a JavaScript function with several sequential steps
function processNumberDetailed(inputNumber) {
    // Step 1: Double the input number
    const doubled = inputNumber * 2;

    // Step 2: Square the result from step 1
    const squared = doubled ** 2;

    // Step 3: Subtract 15 from the squared value
    const subtracted = squared - 15;

    // Step 4: Divide by 3
    const divided = subtracted / 3;

    // Step 5: Convert to absolute value (positive number)
    const absolute = Math.abs(divided);

    // Step 6: Round to nearest whole number
    const rounded = Math.round(absolute);

    // Step 7: Format as a readable string
    const formattedResult = `The processed result is ${rounded}`;

    // Return final formatted result
    return formattedResult;
}

// Example usage:
const output = processNumberDetailed(4);
console.log(output); // "The processed result is 9"

```

Taking this example and making it more descript of each step removing the need for the comments, describing what each step does. Now function names tell what the next step in the process is.

```
// Descriptive step functions
function doubleNumber(num) {
    return num * 2;
}

function squareNumber(num) {
    return num ** 2;
}

function subtractFifteen(num) {
    return num - 15;
}

function divideByThree(num) {
    return num / 3;
}

function convertToAbsolute(num) {
    return Math.abs(num);
}

function roundToNearestWhole(num) {
    return Math.round(num);
}

function formatResult(num) {
    return `The processed result is ${num}`;
}

// Main function using descriptive steps
function processNumberDetailed(inputNumber) {
    const doubled = doubleNumber(inputNumber);
    const squared = squareNumber(doubled);
    const subtracted = subtractFifteen(squared);
    const divided = divideByThree(subtracted);
    const absolute = convertToAbsolute(divided);
    const rounded = roundToNearestWhole(absolute);

    return formatResult(rounded);
}

// Example usage:
const output = processNumberDetailed(4);
console.log(output); // "The processed result is 9"

```

If we look at what the funtion does, it really just atkes the output from one function and places it into the next one. To us, we don't really care what te output of one function is, but with this method we have to stoare it in a temporary variable so we can use it in the next one. we have several steps in a function and are storing the accumpuated state in a different varaible at each step.

Now lets take an aside and look at a similar process with an array adding each number together and calculating the result. The following function is trite, and is only for an exmpale, to get us to the next part of the process

```

```
