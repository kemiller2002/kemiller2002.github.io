# Personal Site Redesign and Publishing Architecture

Status: implementation target for KMM-SITE-001  
Branch: `feature/kmm-site-001-redesign`  
GitHub issue: #1

## Purpose

Redesign `kevinmmiller.us` / `kemiller2002.github.io` as an editorial technical journal and personal engineering site, while replacing the current dependency-free Node generator with a typed F# static publishing pipeline.

The site remains static-first. JavaScript is not required to read, navigate, or discover content.

## Ownership boundaries

| Concern | Owner |
|---|---|
| Content source | Markdown and page source in this repository |
| Publication legality and invariants | F# domain shaped by Ordo |
| Build orchestration, evidence, provenance, handoff | Praxis |
| Shared semantic HTML, typography, layout, responsive behavior, accessibility presentation | Forma |
| Markdown parsing | Markdig behind an application-owned F# adapter |
| Site-specific content projection and metadata | F# site generator |
| Browser-side interaction beyond native HTML | Limen, only when a concrete interactive requirement exists |
| Hosting | GitHub Pages |

Limen is intentionally absent from the initial publishing path. Markdown-to-HTML conversion is a build-time transformation, not a browser/application authority boundary.

## Build pipeline

```text
site-src/posts/*.md
site-src/pages/*
        |
        v
Discover source
        |
        v
Parse front matter + body
        |
        v
Validate source identity and metadata
        |
        v
Validated content model
        |
        +--> Markdown adapter --> content HTML
        |
        v
Project page/article view model
        |
        v
Render Forma semantic markup
        |
        v
Verify routes, links, assets, metadata and deterministic output
        |
        v
Publishable site
        |
        v
GitHub Pages artifact
```

Effects such as file discovery, reading, and writing stay outside the publication domain. The core model receives values and returns values or explicit findings.

## Publication state

The implementation must distinguish at least these states:

```text
Source
  -> Parsed
  -> Validated
  -> Rendered
  -> Verified
  -> Publishable
```

A later state may only be constructed from the immediately required validated predecessor. A parse failure is not a draft. A validation failure is not a rendered post. A rendered file is not publishable merely because HTML exists.

Draft/publication status is separate from validity. A valid draft is valid content that policy excludes from the production artifact.

## Historical URL compatibility

The current Node generator derives dated post routes from the filename, not from the front-matter `date` value:

```text
YYYY-M-D-slug.md -> /YYYY/MM/DD/slug.html
```

The F# migration therefore treats the filename as the legacy route authority for existing posts. Month and day are zero-padded in generated routes.

A front-matter date that disagrees with the filename date is a migration finding. It must not silently change the route.

Known examples include posts whose filename date and declared date differ. The migration phase must inventory all such cases before cutover.

Case, punctuation, and historical slug spelling are compatibility data. They are not normalized away during migration.

## Front-matter compatibility contract

Existing content demonstrates these fields:

- `layout`
- `title`
- `date`
- `description`
- `author`
- `published`
- `categories` as a scalar, inline list, or indented list
- `tags` as an inline list or indented list

The site does not need a general YAML implementation. The application-owned parser should support the deliberately bounded flat metadata grammar used by the repository and reject unsupported nested structures explicitly.

Required behavior:

1. Preserve quoted scalar content after the first key separator.
2. Support booleans for `published`.
3. Support scalar, bracketed, and indented string lists for tags/categories.
4. Preserve unknown keys as diagnostics-compatible extension metadata rather than silently discarding them.
5. Require a complete opening and closing front-matter fence for metadata to be interpreted as metadata.
6. Report malformed front matter with source path and actionable detail.

At least one historical source appears to contain front-matter-like lines without the opening `---`. The new pipeline must report that source rather than render the metadata lines as ordinary prose without explanation.

## Markdown boundary

Use Markdig as the one deliberate content parsing dependency, pinned to an exact version. As of this design, Markdig 1.4.0 is the current release.

Markdig is an adapter, not the domain model:

```text
Validated Markdown source
        |
        v
MarkdownPort
        |
        v
Markdig adapter
        |
        v
Rendered content HTML
```

No Markdig type crosses into the domain or rendering contracts.

Historical posts contain raw HTML. Raw HTML support must be explicit and regression-tested. The migration should not rewrite historical prose merely to simplify parsing.

## Initial F# structure

Keep the implementation small enough for agents to reason about. Start with one application project and one test project rather than a project per layer.

```text
src/
  KevinMiller.Site/
    Domain.fs
    FrontMatter.fs
    Markdown.fs
    Discovery.fs
    Projection.fs
    Rendering.fs
    Verification.fs
    Build.fs
    Program.fs

tests/
  KevinMiller.Site.Tests/
```

Split into additional assemblies only when an actual independent distribution or dependency boundary appears.

## Rendering and Forma

Forma owns shared presentation. The site generator emits canonical Forma structure and classes and supplies site content.

The site must pin a concrete Forma release through `forma.lock`. Shared presentation must not be copied into local CSS.

Local CSS is limited to the Forma marketing-site contract: personal identity imagery, content-specific figures/diagrams, allowlisted identity retargets, and temporary documented Forma capability gaps.

## Visual direction

The personal site should be related to Echelon visually without presenting as another Echelon product.

The primary design model is an editorial technical journal:

- text hierarchy and rhythm before decorative containers;
- fewer card grids;
- stronger article and archive surfaces;
- a compact personal masthead;
- current work presented as a concise index rather than a product catalog on the homepage;
- speaking, experience, about, and contact remain easy to locate;
- article pages use a comfortable measure, strong code presentation, clear metadata, and restrained chrome;
- the site remains useful and complete with client-side JavaScript disabled.

### Homepage information hierarchy

1. Identity and concise thesis.
2. Featured/recent writing.
3. Current work.
4. Selected experience.
5. Speaking/about/contact.

The complete Echelon ecosystem remains available on its own page rather than dominating the personal homepage.

### Article information hierarchy

1. Article type/topic context.
2. Title.
3. Publication date and optional description/tags.
4. Long-form content.
5. Previous/next navigation.
6. Related writing when explicit metadata supports it.
7. Return to writing archive.

## Metadata and discovery

The F# generator ultimately owns:

- a correct canonical URL per page;
- page-specific descriptions;
- OpenGraph/social metadata;
- feed generation;
- sitemap generation;
- topic/tag archive projections;
- draft exclusion;
- previous/next navigation;
- related-content projection from explicit metadata/rules;
- internal-link and asset validation.

The current site-wide canonical URL behavior must not survive cutover.

## Verification gates

Before deployment, the migration target requires:

- F# build succeeds;
- unit tests succeed;
- all source content is accounted for;
- no duplicate generated route exists;
- historical URL inventory matches the compatibility contract;
- output is deterministic across two builds;
- internal links and referenced assets resolve or are explicitly external;
- Forma lock/install verification succeeds;
- Forma local-CSS policy succeeds;
- required responsive widths include 1280, 768, 390, and 320 CSS px;
- keyboard focus and skip navigation work;
- WCAG 2.2 A/AA automated checks are clean;
- reduced motion, forced colors, text spacing, and high zoom checks run;
- Praxis and Ordo verification succeed;
- GitHub Pages deploys only the verified artifact.

## Migration rule

The legacy Node build remains the production path until the new pipeline proves compatibility. New F# work is built side-by-side. Cutover is a separate work item and must remove the old generator only after verification evidence exists.

## Open implementation observations

- Praxis 3.5.0, Ordo 1.4.0, and Forma 0.3.0 are the pinned migration baselines.
- The current execution environment can mutate GitHub but cannot run .NET locally.
- Repository Actions did not surface a workflow run for either branch or default-branch bootstrap attempts during the initial migration session.
- Until governed execution is available, implementation commits on the migration branch must be treated as pre-governance work and reconciled to their real KMM-SITE work items after Praxis installation. Do not manufacture attribution by touching files.
