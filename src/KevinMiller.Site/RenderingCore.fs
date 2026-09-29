namespace KevinMiller.Site

open System
open System.Globalization
open System.Net

module RenderingCore =
    let siteUrl = "https://kevinmmiller.us"
    let siteName = "Kevin M. Miller"

    let encode (value: string) =
        WebUtility.HtmlEncode(value)

    let routeUrl route =
        if route = "/" then siteUrl else siteUrl + route

    let dateText (date: DateOnly) =
        date.ToString("MMMM d, yyyy", CultureInfo.InvariantCulture)

    let navLink current key href label =
        let currentAttribute =
            if current = key then " aria-current=\"page\"" else ""

        $"<li><a class=\"ef-site-nav__link\" href=\"{href}\"{currentAttribute}>{encode label}</a></li>"

    let shellWithRobots current openGraphType title description canonical robots content =
        let pageTitle =
            if title = siteName then siteName else $"{encode title} | {siteName}"

        let robotsMeta =
            robots
            |> Option.map (fun value -> $"  <meta name=\"robots\" content=\"{encode value}\">\n")
            |> Option.defaultValue ""

        let nav =
            [ navLink current "home" "/" "Home"
              navLink current "writing" "/blog/" "Writing"
              navLink current "work" "/echelon-systems/" "Work"
              navLink current "speaking" "/talks.html" "Speaking"
              navLink current "about" "/about/" "About"
              navLink current "contact" "/contact/" "Contact" ]
            |> String.concat "\n"

        $"""<!doctype html>
<html lang="en" data-ef-theme="light">
<head>
  <meta charset="utf-8">
  <meta name="viewport" content="width=device-width, initial-scale=1">
  <title>{pageTitle}</title>
  <meta name="description" content="{encode description}">
{robotsMeta}  <meta property="og:type" content="{encode openGraphType}">
  <meta property="og:title" content="{encode title}">
  <meta property="og:description" content="{encode description}">
  <meta property="og:url" content="{encode canonical}">
  <meta name="twitter:card" content="summary">
  <link rel="canonical" href="{encode canonical}">
  <link rel="alternate" type="application/atom+xml" title="Kevin M. Miller — Writing" href="/feed.xml">
  <link rel="preconnect" href="https://fonts.googleapis.com">
  <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
  <link rel="stylesheet" href="https://fonts.googleapis.com/css2?family=IBM+Plex+Mono:wght@400;500&amp;family=Manrope:wght@400;500;600;700&amp;family=Newsreader:opsz,wght@6..72,500&amp;display=swap">
  <link rel="stylesheet" href="/assets/forma/forma-echelon-marketing.css">
  <link rel="stylesheet" href="/site.css">
</head>
<body class="ef-site" data-ef-layout="marketing">
  <a class="ef-skip-link" href="#main-content">Skip to main content</a>
  <header class="ef-site-header">
    <div class="ef-site-header__inner">
      <div class="ef-site-header__identity">
        <a class="ef-site-header__brand" href="/">
          <span class="ef-site-header__mark" aria-hidden="true">KM</span>
          <span class="ef-site-header__name">Kevin M. Miller</span>
        </a>
        <p class="ef-site-header__tagline">Engineering · Systems · Judgment</p>
      </div>
      <nav class="ef-site-nav" aria-label="Primary">
        <ul class="ef-site-nav__list">
          {nav}
        </ul>
      </nav>
    </div>
  </header>
  <main class="ef-site__main" id="main-content" tabindex="-1">
    {content}
  </main>
  <footer class="ef-site-footer" data-ef-tone="inverse">
    <div class="ef-site-footer__inner">
      <div class="ef-site-footer__main">
        <div>
          <p class="ef-eyebrow">Kevin M. Miller</p>
          <p>Engineering leadership, systems diagnostics, and writing about judgment under constraint.</p>
        </div>
      </div>
      <nav class="ef-site-footer__nav" aria-label="Footer">
        <ul>
          <li><a href="/blog/">Writing</a></li>
          <li><a href="/talks.html">Speaking</a></li>
          <li><a href="/about/">About</a></li>
          <li><a href="/contact/">Contact</a></li>
        </ul>
      </nav>
      <p class="ef-site-footer__note">
        <span>&copy; 2026 Kevin M. Miller</span>
        <span>Built as static HTML with Forma</span>
      </p>
    </div>
  </footer>
</body>
</html>"""

    let shell current openGraphType title description canonical content =
        shellWithRobots current openGraphType title description canonical None content

    let indexItemWithAction marker eyebrow title summary href action =
        $"""<li class="ef-index__item">
  <p class="ef-index__marker">{encode marker}</p>
  <div class="ef-index__heading">
    <p class="ef-eyebrow">{encode eyebrow}</p>
    <h3 class="ef-index__title">{encode title}</h3>
  </div>
  <p class="ef-index__summary">{encode summary}</p>
  <a class="ef-index__link" href="{encode href}">{encode action}</a>
</li>"""

    let indexItem marker eyebrow title summary href =
        indexItemWithAction marker eyebrow title summary href "Read"

