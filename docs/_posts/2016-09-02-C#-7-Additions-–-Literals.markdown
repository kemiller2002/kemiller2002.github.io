---
layout: post
title: "C# 7 Additions – Literals"
date: 2016-09-02 00:00:00 -0500

---

A small, but nice chance in C# 7 is increased flexibility in literals.  Previously, large numeric constants had no separator, and it was difficult to easily read a large number.  For example, if you needed a constant for the number of stars in the observable universe (1,000,000,000,000,000,000,000), you'd have to do the following: 

```
ulong numberOfStarsInTheUniverse = 10000000000000000000;
```

If you hadn't caught the error, the constant is too short, and it's difficult to tell looking at the numbers without a separator.  In C# 7, it's now possible to use the underscore (_) in between the numbers. So the previous example now becomes much easier to read, and it is easily recognizable the number is off.

```
ulong numberOfStarsInTheUniverse = 10_000_000_000_000_000_000; 
  //This is too short!
```


The new version adds binary constants too.  Instead of writing a constant in hex, or decimal, a constant can now be written like so: 

```
int defaultAdminFlag = 0b1000_0000_0000_0000_0000;
```
