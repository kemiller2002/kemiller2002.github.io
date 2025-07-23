---
layout: post
title: "A Lesson in Blind Trust"
date: 2025-04-30 00:00:00 -0500
categories: General
published: true
---
Years ago, I worked for a company with several sister organizations that sold products to both consumers and businesses. One of these organizations operated much like a startup—it was relatively new and trying to establish itself as a major online retailer in a competitive space already dominated by well-established players with loyal customer bases.

To gain an edge, someone proposed a seemingly smart and straightforward strategy: undercut the competition. They decided to sell overlapping products at a small discount compared to rival sites. The challenge, of course, was determining competitors’ pricing in real time. The solution? Build a tool to periodically scrape pricing data from competitor websites and automatically adjust prices slightly lower. The dev team got it working, ran a few tests, and flipped the switch.

At first, everything seemed to be working fine. But then something changed—either by coincidence or as a strategic move, one of the competitor sites began displaying post-discounted prices, including promotional offers and subsidies. Instead of listing an item at $50, it now showed $10. Our system dutifully scraped that new price, applied the standard discount, and began selling that item for $6—despite its $45 overhead.

With no monitoring or oversight of the automation logic, the company bled money fast. What started as a clever pricing tactic nearly bankrupted the organization within weeks.

Automation is powerful. Trust in your systems is necessary. But blind trust—especially in critical systems—can be catastrophic.