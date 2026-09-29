# kevinmmiller.us

This repository builds Kevin M. Miller's static site with a typed F# publishing pipeline.

## Architecture

- `site-src/posts/` contains Markdown posts and front matter.
- `site-src/assets/` contains site-owned static assets only.
- `src/KevinMiller.Site/` owns content discovery, front-matter parsing, validation, Markdown rendering, routes, metadata, feeds, and static HTML generation.
- Forma 0.3.0 owns the shared presentation system and is pinned by checksum in `forma.lock`.
- Ordo governs legal publishing states and repository engineering constraints.
- Praxis governs work, provenance, validation, evidence, and handoff.
- Limen is intentionally absent. This site has no browser-side application state that requires it.
- `migration/legacy-html-routes.txt` is the durable compatibility baseline for URLs published before the F# cutover.

Generated output is written to `dist-v2/` and deployed by GitHub Actions. Generated site output is not committed.

## Build

Install the pinned Forma presentation:

```bash
curl -fsSLo /tmp/forma-install.sh https://raw.githubusercontent.com/kemiller2002/forma/v0.3.0/actions/install-presentation/install.sh
bash /tmp/forma-install.sh
```

Then build, test, validate, and generate:

```bash
dotnet build KevinMiller.Site.slnx --configuration Release
dotnet test KevinMiller.Site.slnx --configuration Release --no-build
dotnet run --project src/KevinMiller.Site/KevinMiller.Site.fsproj --configuration Release --no-build -- validate --root .
dotnet run --project src/KevinMiller.Site/KevinMiller.Site.fsproj --configuration Release --no-build -- verify-build --root .
dotnet run --project src/KevinMiller.Site/KevinMiller.Site.fsproj --configuration Release --no-build -- build --root . --out dist-v2
dotnet run --project src/KevinMiller.Site/KevinMiller.Site.fsproj --configuration Release --no-build -- verify-site --root . --out dist-v2
```

To inspect the generated site locally:

```bash
python3 -m http.server 4001 --directory dist-v2
```

## Publishing rules

Published article routes remain in their historical dated form. Drafts never enter the archive, feed, or sitemap. If an explicit draft was accidentally published by the legacy generator, the F# build preserves that historical URL with a noindex compatibility page without republishing the draft body.

The build fails on invalid content, duplicate routes, missing historical routes, missing local links/assets, a missing or drifted Forma asset, or nondeterministic output.
