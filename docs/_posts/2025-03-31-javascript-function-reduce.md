---
layout: post
title: "Reduce over functions or Data, Data Everywhere"
date: 2025-03-31 00:00:00 -0500
categories: JavaScript
published: false
---
Treating functions as data lets you assemble logic the same way you combine values. Code is just a series of steps, so you can put those steps in an array and manipulate them with tools like `reduce`. To warm up, here is a clunky way to add numbers one at a time:

```javascript
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

Grouping the numbers into an array is cleaner and lets the program decide how many times to run:

```javascript
function addNumbersSequentially(numbersArray) {
    let result = 0;
    for (let ii = 0; ii < numbersArray.length; ii++) {
        result += numbersArray[ii];
    }

    return result;
}
```

`Array.reduce` already does this work for us, so we can rely on it instead of reimplementing the pattern:

```javascript
function reduceEquivalent(array, accumulator, initialValue) {
    let accumulatedValue = accumulator(initialValue);

    for (let ii = 0; ii < array.length; ii++) {
        accumulatedValue = accumulator(accumulatedValue, array[ii]);
    }

    return accumulatedValue;
}

function addNumbersAccumulator(currentTotal, number) {
    return currentTotal + number;
}

const result = reduceEquivalent([1, 2, 3, 4, 5, 6, 7], addNumbersAccumulator, 0);
console.log(result); // 28
```

And using the built-in `reduce` is even shorter:

```javascript
function addNumbersSequentially(numbersArray) {
    return numbersArray.reduce(addNumbersAccumulator, 0);
}
```

The same idea works for functions themselves. Here is a verbose, multi-step calculation:

```javascript
function processNumberDetailed(inputNumber) {
    const doubled = inputNumber * 2;
    const squared = doubled ** 2;
    const subtracted = squared - 15;
    const divided = subtracted / 3;
    const absolute = Math.abs(divided);
    const rounded = Math.round(absolute);

    return `The processed result is ${rounded}`;
}

// Example usage:
const output = processNumberDetailed(4);
console.log(output); // "The processed result is 9"
```

Breaking each step into its own function makes the intent clearer:

```javascript
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
```

With those helpers in place, we can treat the functions themselves as data and reduce over them:

```javascript
const processingSteps = [
    doubleNumber,
    squareNumber,
    subtractFifteen,
    divideByThree,
    convertToAbsolute,
    roundToNearestWhole,
    formatResult,
];

function processNumberWithReduce(inputNumber) {
    return processingSteps.reduce((current, step) => step(current), inputNumber);
}

console.log(processNumberWithReduce(4)); // "The processed result is 9"
```

`reduce` keeps the accumulated value for you—whether that value is a running total or the output of the previous function. Treating functions as data unlocks simple, composable pipelines without the clutter of intermediate variables.
