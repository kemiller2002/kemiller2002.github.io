# Kevin Miller Site current state

## Repository status

The redesign is implemented on `feature/kmm-site-001-redesign` in PR #10.

Praxis 3.1.4 and Ordo 1.2.0 are installed and active. Forma 0.3.0 is pinned by
checksum and consumed through its released installation action.

## Implemented

- typed F# static publishing pipeline;
- Markdig isolated behind `MarkdownRenderer`;
- explicit parsed, validated, rendered, and publishable article states;
- legacy route preservation;
- deterministic static generation;
- Home, Writing, article, Work, Speaking, About, Contact, speaker bio, and 404;
- Atom feed and sitemap;
- per-page canonical and OpenGraph metadata;
- previous/next and explicit-metadata related writing;
- Forma-based presentation with constrained Kevin Miller identity tokens;
- production Pages workflow that verifies the generated artifact before deploy;
- responsive/accessibility browser suite;
- legacy Node generator and committed generated output removed.

## Content accounting

The current corpus contains 165 Markdown sources discovered by the publisher.
The migration baseline records 172 historically published HTML paths.

Explicit drafts remain drafts. Historical URLs that were accidentally exposed by
the old generator receive noindex compatibility pages without their draft body.

## Known upstream/deferred work

- Forma has no dedicated editorial callout primitive. Until forma#82 ships,
  semantic Markdown blockquotes are the supported callout-like construct.
- Broader Forma forced-colors application-theme defects found during the 0.3.0
  release are tracked upstream separately.
- Praxis 3.1.4 does not provide the newer `work reconcile` operation, so
  pre-Praxis migration commits are not retroactively fabricated.

## Active work

KMM-SITE-001 remains active while PR #10 is being proven and cut over.

The immediate gate is final green evidence after the renderer was split into
bounded modules for strict Ordo structural compliance.

## Completion condition

Merge only after the current head passes:

- Praxis validation;
- strict Ordo verification;
- F# build/tests/content checks;
- deterministic generation;
- generated-site verification;
- full browser/accessibility verification.

After merge, verify the GitHub Pages production deployment and kevinmmiller.us
before completing KMM-SITE-009 and the root migration item.
