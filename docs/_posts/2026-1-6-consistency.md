---
layout: post
title: "Consistency Is Underrated"
date: 2026-01-06
tags:
  - consistency
  - product
  - ux
  - systems
  - business
  - change-management
---

<!doctype html>
<html lang="en">
<head>
  <meta charset="utf-8" />
  <meta name="viewport" content="width=device-width, initial-scale=1" />
  <title>Consistency Is Underrated</title>
  <meta name="description" content="Why consistency, predictability, and known behavior often beat improvement that resets trust." />
  <style>
    :root { --text:#111; --muted:#444; --rule:#e6e6e6; --bg:#fff; }
    body { margin:0; background:var(--bg); color:var(--text); font: 18px/1.6 system-ui, -apple-system, Segoe UI, Roboto, Helvetica, Arial, sans-serif; }
    main { max-width: 780px; margin: 0 auto; padding: 48px 20px; }
    h1 { font-size: 2.2rem; line-height: 1.2; margin: 0 0 10px; }
    .meta { color: var(--muted); font-size: 0.95rem; margin-bottom: 28px; }
    p { margin: 0 0 18px; }
    hr { border: 0; border-top: 1px solid var(--rule); margin: 34px 0; }
    blockquote { margin: 22px 0; padding: 8px 18px; border-left: 4px solid var(--rule); color: var(--muted); }
    .callout { background: #fafafa; border: 1px solid var(--rule); border-radius: 10px; padding: 16px 16px; margin: 22px 0; }
    .callout strong { display:block; margin-bottom: 6px; }
    footer { margin-top: 44px; color: var(--muted); font-size: 0.95rem; }
  </style>
</head>

<body>
  <main>
    <header>
      <h1>Consistency Is Underrated</h1>
      <div class="meta">January 6, 2026</div>
    </header>

    <p>We were making fudge.</p>

    <p>If you have ever made it in volume, you learn pretty quickly that it is not perfectly consistent. Some batches set wrong. Some come out with the wrong texture. Sometimes you throw the whole thing away. If you are lucky, you only throw part of it away.</p>

    <p>We were talking about standards. About whether you change the process. About whether you try to make it better.</p>

    <p>That is when my dad said it, casually, like it was not advice at all.</p>

    <blockquote>
      <p>People want consistency.</p>
    </blockquote>

    <p>Not as a correction. Not as an argument. Just as a statement of how manufacturing works, and how customers behave when they find something they like. Once something works, you keep doing it, because you already have customers who chose <em>this</em> thing. Changing it means you are starting over in ways you do not always see.</p>

    <p>At the time, I did not fully buy it. I believed what most people believe early on. Better quality should always win. Improve something and customers will naturally gravitate toward it.</p>

    <p>That belief feels obvious when you are standing inside it.</p>

    <p>What I did not recognize was the assumption inside the belief.</p>

    <p>It assumes that customers are optimizing for the same things you are.</p>

    <p>They are not.</p>

    <p>Often, customers are optimizing for predictability. Familiarity. The thing they already learned how to use. The thing they already decided to trust. The thing they can fit into their routines without thinking about it.</p>

    <hr />

    <p>Years later, that lesson showed up again, this time in a place that surprised me.</p>

    <p>I worked at a financial company where a loan loss calculation was wrong for a specific class of loans. Not wildly wrong. But wrong. When we finally understood it, the reaction from the team was immediate. We should fix it.</p>

    <p>The engineering manager said no.</p>

    <p>At the time, it sounded insane. Why would you knowingly leave something incorrect in production?</p>

    <p>His reasoning was uncomfortable but clear. We were wrong, but we were consistently wrong. Every downstream system already knew how to accommodate that behavior. Reports, reconciliations, controls, and workflows were built around it. If we fixed it, we would not just correct a formula. We would break everything that depended on it and then spend months rediscovering all the places that had quietly adapted.</p>

    <div class="callout">
      <strong>The uncomfortable truth</strong>
      <p>A known wrongness can be safer than unknown correctness.</p>
    </div>

    <p>It was not a proud decision. It was not elegant. But it was operationally honest. Knowing exactly how something behaves, even when it is imperfect, is often safer than changing it and hoping nothing else breaks.</p>

    <hr />

    <p>I think about this a lot now, especially when building software and user interfaces.</p>

    <p>I once consulted on a product where a new product owner wanted to redesign an aging system. And to be fair, it showed its age. The interface was not modern. The experience was not beautiful. But it worked, and more importantly, users knew how it worked.</p>

    <p>This was not an application people lived in every day. Some users logged in once a month. Some logged in once every six months. Some logged in once a year to do a specific task and then disappeared again. They did not love the product, but they could get through it. It was familiar.</p>

    <p>The redesign aimed to make things better by putting tighter bounds on what users could enter. The theory was that correctness should be enforced up front, instead of letting a back office team review submissions and fix issues later.</p>

    <p>On paper, that sounds like an improvement.</p>

    <p>In practice, it broke the value proposition.</p>

    <p>Those users were not paying for perfect validation. They were paying to not have to think about edge cases. They wanted to enter data and move on. If something needed to be corrected, they expected the system and the people behind it to handle it. That was the service. A white glove posture that the organization had operated on for years.</p>

    <p>When the software changed, users got frustrated. Then angry. Internally, the reaction was confusion. Why are they being difficult?</p>

    <p>They were not being difficult.</p>

    <p>We changed what we were offering them.</p>

    <hr />

    <p>That is the part that is easy to miss. People do not always leave because you made something worse. Sometimes they leave because you changed the deal they thought they had with you.</p>

    <p>Consistency is not glamorous. It does not make for a dramatic roadmap. It rarely wins internal applause. But it is what people build their lives, habits, and systems around.</p>

    <p>Being consistently off can be better than fixing something and breaking everything around it. Not because correctness does not matter, but because predictability compounds. Familiarity compounds. Trust compounds.</p>

    <p>I still think about that fudge. About throwing away bad batches, but not reinventing the recipe every week. About how much effort it takes to earn a repeat customer, and how casually that trust can be spent.</p>

    <p>Every time someone says, “We should change this,” I pause a little longer now.</p>

    <p>Sometimes the right move is to improve.</p>

    <p>But sometimes the right move is to keep the promise you are already keeping.</p>

    <footer>
      <p>Originally drafted from a lesson learned in manufacturing, and re-learned in systems, finance, and product work.</p>
    </footer>
  </main>
</body>
</html>