# Kevin Miller Site handoff

## Objective

Complete and deploy the governed redesign of kevinmmiller.us from PR #10.

## Branch and work

- branch: `feature/kmm-site-001-redesign`
- root work item: `KMM-SITE-001`
- pull request: #10
- child work items: KMM-SITE-002 through KMM-SITE-009

## Architecture

The application/site generator is F#.

Forma owns presentation. Ordo governs engineering structure. Praxis governs work
and evidence. Limen is intentionally not used because there is no browser-side
application state.

See `context/ARCHITECTURE.md` for the full boundary map.

## Important migration facts

- Do not restore `build.mjs`, the old local design system, or committed generated
  `docs/` output.
- Do not copy Forma component CSS into the site.
- `forma.lock` is authoritative for the presentation asset.
- `migration/legacy-html-routes.txt` is the historical-route compatibility
  baseline.
- Drafts must never enter publishable artifacts, the archive, feed, sitemap, or
  related-writing results.
- Historical draft URLs may only emit their noindex compatibility placeholder.
- Reserved URL characters in legacy slugs are encoded in public links/canonicals
  while physical output compatibility remains intact.
- Related writing is deterministic and uses only explicit tags/categories.

## Validation

For repository governance:

```bash
./ros registry check
./ros validate
npx --yes @echelon-foundry/sde@1.2.0 verify --strict
```

For the publishing pipeline, follow the commands in `README.md`. PR CI also
runs the generated site through the browser/accessibility suite.

## Current architectural repair

Strict Ordo identified the former monolithic `Rendering.fs` as oversized at
656 physical lines. It has been split into:

- `RenderingCore.fs` for shell/shared primitives;
- `RenderingEditorial.fs` for Home/archive/article;
- `RenderingPages.fs` for static pages;
- `Rendering.fs` as a small compatibility facade.

Do not recombine these modules.

## Next action

1. Confirm the current PR head is green in the site, Praxis, and strict-Ordo
   workflows.
2. Record completion evidence on KMM-SITE-003 through KMM-SITE-008.
3. Merge PR #10 only after all release gates are green.
4. Verify the production Pages deployment and public site.
5. Complete KMM-SITE-009 and KMM-SITE-001 with the production evidence.
