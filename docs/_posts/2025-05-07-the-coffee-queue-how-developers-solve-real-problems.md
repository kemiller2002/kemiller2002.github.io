---
layout: post
title: "The Coffee Queue: How Developers Solve Real Problems"
date: 2025-05-07
---

At a company I once worked for, we had a very specific rule: **only one carafe of coffee** in the office at a time.

Not for health reasons.  
Not for logistics.  
But because someone decided that a **second carafe—at 65 cents per refill—was too expensive**. For reference, that’s about **three cents a cup**.

Now, the coffee was awful. But it was all they offered. And in the morning, **everyone wanted it**.

---

## The Real Problem

Here’s what happened:

- People showed up to the breakroom in waves.
- The first few got their coffee.
- Then someone hit an empty carafe—and had to brew a new pot. That took 3–5 minutes.
- Others would arrive, unknowingly walk into the same dead-end, and suddenly…  
  **A line formed.**
- While they waited, people **got irritated**, started **complaining**, and by the time they returned to their desks, they were **annoyed—not at the coffee, but at whoever decided the coffee was worth more than their time**.

This was a **real workplace friction point**, every single day. But no one had formally written it up.

Eventually, the VP—tired of hearing people grumble—half-jokingly said:

> “If you write up a reason to have two carafes, I’ll pass it along.”

So I did.

---

## Enter Queuing Theory

I wrote a breakdown using **queuing theory**:

- With one carafe, we had a **G/G/1 queue** with **uncertain service availability** and **catastrophic wait times** during peak hours (i.e., mornings).
- People don’t know if the carafe has coffee until they try. If it’s empty, someone starts brewing, but anyone arriving during that time is stuck in line.
- This creates a hidden queue that **builds frustration**, **burns time**, and **kills morale**.

Adding a second carafe transforms the system into a **G/G/2 queue**:

- It introduces **redundancy**, **smooths out the peak demand**, and reduces the probability of service failure.
- Most importantly, it **preserves flow**. People get in, get coffee, and get back to work—without standing around venting about mismanagement over a three-cent decision.

---

## What Happened Next Was… Hilarious

Shortly after I sent the email, someone was **screen-sharing during a meeting** with several **VPs and managers**—and my write-up was still open on their screen.

They paused the meeting.

Then they read the email out loud.

And instead of brushing it off, the room **fully invested in the problem**. They started laughing—but the logic landed.  
It was too real to ignore.

That same **afternoon**, the company **publicly announced a new two-carafe policy**. Just like that, the coffee bottleneck was fixed.

---

## Why This Matters

This story isn’t about coffee.

It’s about how developers—and engineers in general—solve real problems:

- We recognize patterns.
- We model load, latency, and friction.
- We identify failure points no one else sees.
- And we make systems **work better for the people using them**—whether that’s a distributed microservice or a miserable morning coffee queue.

Real engineering isn’t always glamorous.  
Sometimes it smells like burnt coffee.  
But it makes things better.

And yes—sometimes it even gets your policy changed before lunch.