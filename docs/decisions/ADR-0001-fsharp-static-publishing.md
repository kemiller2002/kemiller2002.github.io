# ADR-0001: Use a typed F# static publishing pipeline

Status: Accepted for migration implementation  
Date: 2026-09-29  
Work item: KMM-SITE-001  
Related: #1, #4, #5

## Context

The current site uses a small dependency-free Node script that performs source discovery, front-matter parsing, Markdown parsing, routing, templating, excerpt generation, and output writing in one file.

That implementation has been useful because it is transparent and has few dependencies. It now has three material limitations for the redesign:

1. the handwritten Markdown implementation is necessarily partial and historical content already contains edge cases and raw HTML;
2. content validity, publication policy, routing, and rendering are not represented as distinct legal states;
3. presentation is coupled to bespoke site HTML/CSS instead of the shared Forma contract.

The site is fundamentally static. It does not need a client application framework to publish articles.

## Decision

Build the replacement generator in F# as one small executable with modules separating pure publication rules from file-system effects.

Use Markdig behind a narrow adapter for Markdown-to-HTML conversion.

Use a bounded application-owned front-matter parser for the repository's flat metadata grammar rather than adopting a general YAML object model.

Render static semantic HTML using pinned Forma presentation assets.

Shape the publication state model according to Ordo principles and govern implementation/execution through Praxis once installed.

Do not introduce Limen into the publication pipeline. Limen may be installed later only for a concrete browser-side interactive capability.

## Consequences

### Positive

- Publication legality can be encoded in types and constructors.
- Route and metadata failures can stop publication before rendering/deployment.
- Markdown behavior comes from a maintained CommonMark-oriented parser rather than a partial local reimplementation.
- The build remains small: one application project, one Markdown package, and no runtime JavaScript requirement.
- Forma becomes the reusable presentation authority.
- The static output remains inexpensive and simple to host on GitHub Pages.

### Costs

- The build now requires the .NET SDK in development and CI.
- Markdig becomes an intentional external dependency that must be pinned and reviewed.
- Historical content inconsistencies that the current generator tolerated will become visible migration findings.
- The migration needs explicit compatibility evidence before the Node generator can be removed.

## Rejected alternatives

### Keep extending the handwritten Node Markdown parser

Rejected because every new Markdown edge case increases parser responsibility without adding meaningful site-specific value.

### Use Limen to process Markdown

Rejected because build-time content transformation is outside Limen's browser/application boundary.

### Introduce React, Angular, or another client framework

Rejected because the site does not have application-state needs that justify shipping a client framework for ordinary reading/navigation.

### Split the generator into many F# assemblies immediately

Rejected because the site is small. File/module boundaries are enough initially and reduce agent/navigation overhead. Assembly boundaries can be introduced when a genuine distribution or dependency boundary appears.

## Compatibility decision

For existing posts, filename-derived dated routes remain authoritative because that is what the current generator publishes. Front-matter date disagreement is reported but does not silently relocate an article.
