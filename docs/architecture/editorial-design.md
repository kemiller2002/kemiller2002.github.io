# Editorial Design Contract

Status: target presentation for KMM-SITE-003 and KMM-SITE-006  
Parent: KMM-SITE-001 / #1  
Forma dependency gap: kemiller2002/forma#51

## Design intent

This is a personal technical journal first and a professional profile second.

It should feel related to Echelon Foundry because it uses the same presentation system, but it must not read like another product microsite. The dominant visual idea is editorial: strong type, readable measure, deliberate spacing, restrained surfaces, and index-like navigation.

The design should communicate:

- technical depth without terminal cosplay;
- personal authorship without a resume-template aesthetic;
- rigor without looking institutional;
- warmth without becoming soft or decorative;
- strong long-form reading as the primary experience.

## Forma composition

Use documented Forma contracts, not locally invented equivalents.

Primary patterns:

- `marketing-shell`: global shell, skip link, header, navigation, main, footer;
- `hero`: identity/thesis opening;
- `section-heading`: section framing;
- `entry-index`: writing, current work, and compact experience listings;
- `prose`: article body and long-form page content;
- `facts`: small factual summaries only where a definition list is genuinely useful;
- `cta`: closing contact/speaking action when warranted.

Do not turn every content group into a card grid. Cards are the exception, not the default structure.

## Identity

Header identity:

```text
KM   Kevin M. Miller
     Engineering · Systems · Judgment
```

The `KM` mark is a text mark, not a logo dependency. The tagline may collapse according to the Forma shell contract; the name never disappears.

Primary navigation:

```text
Writing
Work
Speaking
About
Contact
```

The full Echelon system catalog is reachable through Work, not promoted as a global product-navigation item.

## Typography and tone

Use the Forma marketing typography contract. Prefer its editorial display face for major headings and body/mono roles for their intended purposes.

The personal brand should remain light-first and reading-oriented. Do not copy the Echelon Foundry product palette wholesale. When Forma's brand-manifest release path is available, define a Kevin Miller brand through validated role tokens rather than local component CSS.

Desired perceptual direction:

- page surface: warm/quiet rather than stark;
- text: very dark ink rather than pure decorative black;
- accent: restrained, mature, and secondary to text;
- dividers/borders: functional and sparse;
- no gradients for decoration;
- no glass effects;
- no giant floating cards;
- no fake terminal chrome.

These are intent constraints. Literal colors belong in a Forma brand manifest, not this repository's component CSS.

## Homepage

### 1. Hero

Use the canonical `ef-hero`.

Eyebrow:

```text
Engineering · Systems · Judgment
```

Primary statement:

```text
I work on software systems where the difficult problem is understanding what is actually true.
```

Supporting lead should explain, in compact form, that Kevin works across engineering leadership, system diagnostics, modernization, AI, security, and decision-making under constraint.

Primary action: `Read the writing`  
Secondary action: `See current work`

The hero should not include a wall of achievements or a four-card service grid.

### 2. Featured / recent writing

Use `ef-section-heading` plus `ef-index`.

The newest or explicitly featured essay gets a slightly stronger first position through content hierarchy, not a separate bespoke card component.

Each entry includes:

- publication date;
- topic/category when available;
- title;
- one-sentence description/excerpt;
- article link.

Show a limited recent set on the homepage. The full archive belongs at `/blog/`.

### 3. Current work

Use `ef-index`, not product cards.

Initial entries:

- Echelon Foundry: umbrella engineering/research practice;
- Ordo: state-directed engineering;
- Praxis: repository work/evidence/provenance system;
- Forma + Limen: presentation and browser-boundary work, described together at the personal-site level unless separate emphasis is justified.

The complete ecosystem remains on `/echelon-systems/`.

### 4. Selected experience

Use an index/timeline-like presentation.

Keep only the experience needed to establish scope and pattern:

- Ren;
- T2 Systems;
- TCC Software Solutions.

Emphasize the kind of change and measurable result rather than exhaustive job-description bullets.

### 5. Closing paths

Compact links to:

- speaking;
- about;
- contact.

A heavy sales CTA is inappropriate for the default personal homepage.

## Writing archive

The archive should feel like a journal index, not a grid of cards.

Use `ef-index` grouped by year.

Entry anatomy:

```text
2026 / 09 / 29
TOPIC
Article title
Short description
Read
```

Requirements:

- newest first;
- year anchors/deep links;
- drafts never appear;
- optional topic filtering is static for the first release;
- if client-side filtering is later justified, that is a separate Limen work item rather than ad-hoc JavaScript.

## Article page

The article page is the highest-priority visual surface.

Use the shell plus an article-specific header followed by `article.ef-prose`.

Header includes:

- `Writing` or topic eyebrow;
- one H1;
- optional description/deck;
- publication date;
- tags/categories when useful, not every piece of metadata.

Article body requirements:

- narrow readable measure;
- predictable heading rhythm;
- strong code/pre treatment from Forma;
- visible links;
- blockquote support;
- images constrained to available width;
- historical raw HTML does not escape the article layout;
- no sticky social/share rail;
- no auto-playing or attention-seeking motion.

After the article:

- previous and next writing;
- explicit related writing when metadata supports it;
- return to writing archive.

## Work / Echelon systems page

The existing Echelon systems page has useful content but too many repeated card surfaces.

Retain the conceptual groups:

- engineering foundation;
- assurance;
- experience and boundaries;
- specialized systems.

Render each group as an `ef-section` with an `ef-index`. Each system entry has name, concise role, and destination link.

This produces a scannable technical catalog without making the personal homepage carry the entire ecosystem.

## Speaking

Use a simple editorial page:

- speaking thesis;
- current/representative talks;
- speaker bio;
- contact path.

Avoid conference-speaker marketing tropes such as oversized quote carousels unless real evidence/content justifies them.

## About

Long-form `ef-prose`, with a short factual sidebar/facts region only if it improves scanning.

The page should connect engineering leadership, diagnostic work, writing, and cross-domain practice without repeating the homepage.

## Contact

Keep contact static and low-friction.

The first release does not require an application form. Prefer explicit email/social/contact routes. If a form is later required, define its state/effects and decide whether native HTML is sufficient before introducing Limen.

## Responsive contract

Follow Forma's required widths:

- 1280px;
- 768px;
- 390px;
- 320px.

Source order must remain meaningful when the hero/index reflows. No horizontal page overflow. Navigation may wrap/recompose under the Forma shell rules.

## Accessibility

Inherited/shared behavior must remain inside Forma. Site content must provide:

- one H1 per page;
- non-skipped heading hierarchy;
- useful link text;
- image alt text;
- code language metadata where known;
- explicit current navigation;
- no color-only meaning;
- no hover-only disclosure;
- no interaction required merely to read content.

## JavaScript posture

First release: zero client-side JavaScript required by the personal site.

Limen is reserved for a future capability that has actual application state or browser effects. Static filtering by separate topic/year pages is preferred until interaction provides enough value to justify another execution boundary.
