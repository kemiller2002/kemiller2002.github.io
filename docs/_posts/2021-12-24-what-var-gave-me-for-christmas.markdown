---
layout: post
title: "What var Gave me for Christmas"
date: 2021-12-24 00:00:00 -0500
categories: C#
---

Way back in the day when Microsoft released the update including `var`, the language designers advised to only use when creating an anonymous type. It was a gift only to be used in the sparingness of circumstances and all other variable declarations should explicitly state what type they are. Since that time, that belief has been reversed, as it is heralded by many as an efficient way of not cluttering code. This isn't post about `var` being good or evil. Really that's all a matter of situation and perspective which has been debated since it's inception. It does however lend itself to a demonstration into how exactly the C# type system works and little unknown differences into how people think something should work rather than how it does.

`var present = new CandyCane()`

This is pretty straight forward. Based on what's assigned the variable `present` is a candy cane, and who doesn't love candy cane's for the holidays?

Expand upon this, let's say our candy cane class looks something like this:

`
class CandyCane : IHolidayGift
{

}

`
