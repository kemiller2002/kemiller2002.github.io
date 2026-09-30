# Kevin Miller Site architecture

## Purpose

kevinmmiller.us is a static personal publishing site for Kevin M. Miller. The
repository owns source content, publication rules, static generation, validation,
and GitHub Pages delivery.

## System boundary

The site is intentionally static. It has no browser-side application state and
therefore does not use Limen today.

```
Markdown + front matter
        |
        v
F# source discovery and parsing
        |
        v
ValidatedPost
        |
        v
Markdown rendering + editorial projection
        |
        v
RenderedDocument
        |
        v
PublishableArtifact
        |
        v
verified static files
        |
        v
GitHub Pages
```

## Responsibility map

### F# publishing system

`src/KevinMiller.Site/` owns:

- source discovery;
- legacy filename identity;
- front-matter parsing;
- metadata and publication validation;
- route generation and collision detection;
- Markdown rendering through the application-owned Markdig boundary;
- related-writing projection from explicit tags/categories;
- Home, archive, article, and static-page projection;
- Atom feed and sitemap generation;
- historical-route compatibility;
- deterministic-output verification;
- generated-route/link/asset verification.

### Forma

Forma 0.3.0 owns shared presentation semantics, layout, responsive behavior,
accessibility behavior, and the marketing/content shell. The exact marketing
asset is pinned by checksum in `forma.lock`.

Site-owned CSS may retarget only approved identity tokens. Forma's
`check-site-css` action enforces that boundary.

### Ordo

Ordo 1.2.0 supplies repository engineering constraints and structural
verification. Strict verification is expected to pass. Structural failures are
fixed by changing architecture, not by permanently weakening the gate.

### Praxis

Praxis 3.1.4 owns work tracking, provenance, validation, telemetry, and durable
handoff. KMM-SITE-001 is the umbrella migration work item.

### Limen

Limen is deliberately absent. Introduce it only if a concrete browser capability
requires application/browser interaction that native HTML cannot satisfy.

## Publication states

The legal article pipeline is:

`ParsedDocument -> ValidatedPost -> RenderedDocument -> PublishableArtifact`

`ValidatedPost` is private to the domain constructor. Invalid metadata cannot
construct one. Drafts may become rendered compatibility pages but cannot
transition to `PublishableArtifact`.

## Effects boundary

Domain validation and projections are pure. File discovery, reading, writing,
asset copying, and output-directory replacement live in `SiteBuild`.

## Public compatibility

Historical article routes are derived from dated source filenames. The durable
pre-cutover HTML route baseline is `migration/legacy-html-routes.txt`.

Legacy slugs containing reserved URL characters keep their physical output
identity while public URLs encode the path segment correctly.

Three historical drafts were accidentally emitted by the legacy Node generator.
Their old routes are preserved with noindex compatibility pages, but draft body
content is not republished and drafts never enter the archive, feed, sitemap, or
related-writing projection.

## Verification boundary

A releasable artifact must pass:

- Forma checksum installation;
- Forma local-CSS policy;
- F# build and unit tests;
- content and route validation;
- deterministic-output verification;
- historical-route parity;
- generated local-link and asset verification;
- shell-structure checks;
- Praxis validation;
- strict Ordo verification;
- browser checks at 1280, 768, 390, and 320 px;
- axe WCAG 2.2 A/AA;
- keyboard, skip-link, touch-target, 200% text, text-spacing, reduced-motion,
  forced-colors, print, and JavaScript-disabled checks.

GitHub Pages receives only the artifact that passes these gates.
