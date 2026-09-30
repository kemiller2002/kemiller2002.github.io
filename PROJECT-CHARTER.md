---
id: PROJECT-CHARTER-kevin-miller-site
title: Kevin Miller Site Project Charter
status: accepted
version: 1.0.0
created: 2026-09-29
updated: 2026-09-29
---

# Kevin Miller Site project charter

## Purpose

Publish Kevin M. Miller's writing, engineering work, speaking material, and
professional background through a small, durable, accessible static website.

The site should make long-form writing the strongest surface while connecting it
to current engineering work without becoming another JavaScript application
framework.

## Intended users

- readers of Kevin's technical and systems writing;
- engineering leaders and practitioners evaluating his ideas or work;
- prospective consulting and collaboration contacts;
- event organizers evaluating speaking topics;
- future maintainers and agents publishing or changing the site.

## Bounded outcome

Replace the legacy Node/custom-CSS generator with a governed F# static publishing
pipeline that preserves historical public URLs, consumes Forma as the shared
presentation system, and deploys only verified static artifacts to GitHub Pages.

## Included

- historical Markdown publication;
- typed metadata, route, status, rendered, and publishable states;
- editorial Home/archive/article experience;
- Work, Speaking, About, Contact, speaker bio, and 404 surfaces;
- Atom feed and sitemap;
- deterministic generation;
- historical-route compatibility;
- responsive and accessibility validation;
- GitHub Pages deployment;
- Praxis and Ordo governance.

## Excluded

- client-side application state without a demonstrated need;
- a JavaScript framework;
- a local fork of Forma components;
- opaque AI/embedding-based related-content inference;
- republishing draft bodies solely because the legacy generator accidentally
  exposed them;
- dynamic server infrastructure.

## Success criteria

- every Markdown source is accounted for;
- legacy dated public routes remain valid or have explicit compatibility
  behavior;
- drafts cannot transition to publishable artifacts;
- generated output is deterministic;
- Forma remains pinned and site-local CSS remains within its allowed policy;
- Praxis and strict Ordo verification pass;
- browser validation passes at Forma contract widths and WCAG 2.2 A/AA;
- Pages deploys only the verified artifact;
- a successor can continue from repository records without chat history.

## Constraints

- F# owns the publishing application.
- External dependencies require deliberate justification.
- Markdig is the bounded Markdown implementation dependency.
- Forma owns reusable presentation behavior.
- Limen is introduced only for a real browser/application boundary.
- Historical URLs are compatibility contracts.
- Accessibility failures block release rather than becoming post-release cleanup.

## Decision authority

Kevin M. Miller is the product owner. Repository governance, tests, and release
gates define the machine-enforced conditions for a releasable change.
