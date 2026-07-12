---
layout: post
title: "When Disagreement Doesn't Mean Ambiguity"
date: 2026-07-12
---

One of the most common explanations I hear for software defects is:

> "The requirement was ambiguous."

The developer interpreted it one way.

QA interpreted it another.

Product meant something else.

Everyone nods, the story gets updated, and the team moves on.

The explanation feels reasonable, but it quietly assumes something that isn't necessarily true.

**If two competent people disagree, the requirement must have been ambiguous.**

I don't think that's true.

More importantly, I think believing it causes teams to stop asking the question that actually matters.

**What evidence supported each interpretation?**

## Disagreement Is Not the Diagnosis

When a developer builds one thing and QA expects another, people often jump straight to the conclusion that the story wasn't clear.

Maybe.

But that's only one explanation.

There are others.

Perhaps the story genuinely allowed multiple reasonable interpretations.

Perhaps the developer ignored existing system behavior.

Perhaps QA assumed a business rule that didn't apply.

Perhaps Product forgot to communicate a recent decision.

Perhaps the team never shared the same mental model of how the feature was supposed to work.

Those are completely different problems.

Calling all of them "ambiguity" hides the actual cause.

## Ambiguity Should Be Earned

I think we throw the word _ambiguous_ around far too easily.

People disagreeing is an observation.

Ambiguity is a diagnosis.

Those aren't the same thing.

For something to be genuinely ambiguous, multiple interpretations should be reasonably supported by the available evidence.

That last part matters.

**Reasonably supported by the available evidence.**

Software doesn't exist in a vacuum.

Every requirement sits inside a larger body of knowledge.

There is existing product behavior.

There are business rules.

There are architectural decisions.

There are regulatory constraints.

There are previous conversations.

There is domain knowledge.

All of those things are evidence.

If they all point toward one interpretation, then the requirement may be incomplete, but that doesn't automatically make it ambiguous.

It may simply mean someone reached a conclusion that wasn't well supported.

## Stories Don't Create Understanding

One sentence I've seen recently really resonated with me:

> "The story isn't the understanding. It's just the artifact."

I think that's exactly right.

The conversation creates understanding.

The story records it.

Unfortunately, many teams reverse those two ideas.

They believe the Jira ticket _creates_ shared understanding.

It doesn't.

It captures whatever understanding already exists.

Or, more accurately, whatever understanding the team _thinks_ exists.

When refinement doesn't actually build shared understanding, people often compensate by writing longer stories.

More acceptance criteria.

More bullet points.

More paragraphs.

That usually creates more documentation.

It doesn't necessarily create more clarity.

## Not Every Interpretation Is Equally Strong

Suppose a requirement says:

> The system shall deactivate inactive accounts after 90 days.

One developer assumes administrators are included.

QA assumes administrators are excluded.

Are both interpretations equally valid?

Maybe.

But maybe not.

What does the system already do?

What do previous security decisions say?

What does the compliance documentation require?

How have administrator accounts behaved everywhere else in the product?

If all of that evidence consistently points in one direction, then the stronger interpretation isn't simply a matter of opinion.

It is better supported.

That's an important distinction.

Clarity isn't about accepting every possible interpretation.

It's about determining which interpretations the evidence actually supports.

## Shift the Conversation

Instead of asking:

> Which interpretation do you think is correct?

Ask:

> What evidence supports your interpretation?

That changes the discussion almost immediately.

Now people stop arguing from intuition and start pointing to evidence.

Existing behavior.

Business rules.

Architecture decisions.

Customer workflows.

Regulatory requirements.

Previous design decisions.

Examples.

Now disagreement becomes something you can investigate instead of negotiate.

## AI Raises the Stakes

This becomes even more important as AI writes more of our software.

Humans often notice uncertainty.

We hesitate.

We ask questions.

We recognize when something doesn't quite fit.

AI usually doesn't.

It fills in missing information confidently.

If the evidence genuinely supports multiple interpretations, AI will faithfully amplify that ambiguity.

But if the evidence strongly supports one interpretation and the team simply failed to surface it, AI may confidently build the wrong thing while looking completely correct.

The problem wasn't that AI misunderstood.

The problem was that the evidence available to the AI didn't reflect the evidence the organization already possessed.

## A Better Question

When two people disagree about a requirement, don't immediately conclude that the requirement was ambiguous.

Instead ask:

- What evidence did each person use?
- Which assumptions did they make?
- What information existed before implementation?
- Would a knowledgeable third party reasonably arrive at both conclusions?

If the answer is yes, you've probably found genuine ambiguity.

If the answer is no, you've found something else.

A missing decision.

A knowledge gap.

An unsupported assumption.

A failure to communicate important context.

Those problems need different solutions.

That's one of the ideas we've been exploring with the Clarity Framework.

Clarity isn't about eliminating every possible interpretation.

It's about making sure the interpretation you choose is the one best supported by the available evidence.

Because disagreement is just a symptom.

The real question is whether the evidence ever justified more than one answer in the first place.
