# kevinmmiller.us

This repository now uses a small dependency-free Node build script instead of Jekyll.

## Structure

- `site-src/`
  Source files for the site.
- `site-src/posts/`
  Markdown blog posts with front matter.
- `site-src/pages/`
  Page source for the homepage, archive, contact page, talks page, and speaker bio.
- `site-src/assets/`
  Static assets copied directly into `docs/`.
- `docs/`
  Generated output published by GitHub Pages.
- `build.mjs`
  Static site generator.

## Commands

```bash
npm run build
npm run serve
```

`npm run build` regenerates `docs/`.

`npm run serve` starts a simple local server at `http://localhost:4000` against the generated site.

## Notes

- Post URLs are generated in the same dated format used previously, so existing article links continue to work.
- The build uses no external packages.
