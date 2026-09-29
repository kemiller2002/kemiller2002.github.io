namespace KevinMiller.Site

open System
open System.Globalization
open System.Net

module Rendering =
    let private siteUrl = "https://kevinmmiller.us"
    let private siteName = "Kevin M. Miller"

    let private encode (value: string) =
        WebUtility.HtmlEncode(value)

    let private routeUrl route =
        if route = "/" then siteUrl else siteUrl + route

    let private dateText (date: DateOnly) =
        date.ToString("MMMM d, yyyy", CultureInfo.InvariantCulture)

    let private navLink current key href label =
        let currentAttribute =
            if current = key then " aria-current=\"page\"" else ""

        $"<li><a class=\"ef-site-nav__link\" href=\"{href}\"{currentAttribute}>{encode label}</a></li>"

    let private shell current title description canonical content =
        let pageTitle =
            if title = siteName then siteName else $"{encode title} | {siteName}"

        let nav =
            [ navLink current "writing" "/blog/" "Writing"
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
  <link rel="canonical" href="{encode canonical}">
  <link rel="preconnect" href="https://fonts.googleapis.com">
  <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
  <link rel="stylesheet" href="https://fonts.googleapis.com/css2?family=IBM+Plex+Mono:wght@400;500&amp;family=Manrope:wght@400;500;600;700&amp;family=Newsreader:opsz,wght@6..72,500&amp;display=swap">
  <link rel="stylesheet" href="/assets/forma/forma-echelon-marketing.css">
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

    let private indexItem marker eyebrow title summary href =
        $"""<li class="ef-index__item">
  <p class="ef-index__marker">{encode marker}</p>
  <div class="ef-index__heading">
    <p class="ef-eyebrow">{encode eyebrow}</p>
    <h3 class="ef-index__title">{encode title}</h3>
  </div>
  <p class="ef-index__summary">{encode summary}</p>
  <a class="ef-index__link" href="{encode href}">Read</a>
</li>"""

    let home (posts: ValidatedPost list) =
        let recent =
            posts
            |> List.filter (fun post -> ValidatedPost.status post = PublicationStatus.Published)
            |> List.sortByDescending (ValidatedPost.identity >> SourceIdentity.legacyDate)
            |> List.truncate 6
            |> List.mapi (fun index post ->
                let identity = ValidatedPost.identity post
                let marker = sprintf "W / %02d" (index + 1)
                let eyebrow = dateText (SourceIdentity.legacyDate identity)
                let summary =
                    ValidatedPost.description post
                    |> Option.defaultValue "An essay on engineering, systems, leadership, or decision-making."

                indexItem marker eyebrow (ValidatedPost.title post) summary (ValidatedPost.route post))
            |> String.concat "\n"

        let workEntries =
            [ indexItem "W / 01" "Engineering practice" "Echelon Foundry" "Advisory, research, and engineering systems for consequential software decisions." "https://echelonfoundry.com"
              indexItem "W / 02" "State and legality" "Ordo" "State-Directed Engineering: explicit state, legal transitions, capabilities, obligations, and evidence." "https://github.com/kemiller2002/ordo"
              indexItem "W / 03" "Work and evidence" "Praxis" "Repository work, provenance, execution evidence, validation, and durable handoff." "https://github.com/kemiller2002/praxis"
              indexItem "W / 04" "Presentation and boundaries" "Forma + Limen" "Shared semantic presentation through Forma, with Limen reserved for explicit browser/application boundaries." "https://github.com/kemiller2002/forma" ]
            |> String.concat "\n"

        let experienceEntries =
            [ indexItem "2020–25" "Director of Engineering" "Ren" "Led nine engineering teams; release flow moved from six weeks to two days while operating cost and hotfix pressure fell." "/about/"
              indexItem "2018–20" "Senior Developer" "T2 Systems" "Stabilized large deployment footprints, built CLR diagnostics, and improved delivery practices across production environments." "/about/"
              indexItem "2015–18" "Development Manager" "TCC Software Solutions" "Led public-sector and enterprise engineering work with an emphasis on architecture, delivery discipline, and operational clarity." "/about/" ]
            |> String.concat "\n"

        let content =
            $"""<section class="ef-hero" aria-labelledby="home-title">
  <div class="ef-hero__content">
    <p class="ef-eyebrow">Engineering · Systems · Judgment</p>
    <h1 class="ef-hero__title" id="home-title">I work on software systems where the difficult problem is understanding what is actually true.</h1>
    <p class="ef-lead">I work across engineering leadership, system diagnostics, modernization, AI, security, and decision-making under constraint.</p>
    <div class="ef-actions">
      <a class="ef-button" data-ef-variant="primary" href="/blog/">Read the writing</a>
      <a class="ef-button" href="/echelon-systems/">See current work</a>
    </div>
  </div>
</section>

<section class="ef-section" aria-labelledby="recent-writing-title">
  <header class="ef-section-heading">
    <div class="ef-section-heading__text">
      <p class="ef-eyebrow">Recent writing</p>
      <h2 id="recent-writing-title">Notes on engineering, systems, and judgment</h2>
      <p>Long-form work about what happens when the easy explanation is incomplete.</p>
    </div>
    <div class="ef-section-heading__aside">
      <a class="ef-button" href="/blog/">View all writing</a>
    </div>
  </header>
  <ol class="ef-index" aria-label="Recent writing">
    {recent}
  </ol>
</section>

<section class="ef-section" aria-labelledby="current-work-title">
  <header class="ef-section-heading">
    <div class="ef-section-heading__text">
      <p class="ef-eyebrow">Current work</p>
      <h2 id="current-work-title">Engineering systems that make important claims inspectable</h2>
    </div>
  </header>
  <ol class="ef-index" aria-label="Current work">
    {workEntries}
  </ol>
</section>

<section class="ef-section" aria-labelledby="experience-title">
  <header class="ef-section-heading">
    <div class="ef-section-heading__text">
      <p class="ef-eyebrow">Selected experience</p>
      <h2 id="experience-title">Leadership and delivery work</h2>
    </div>
  </header>
  <ol class="ef-index" aria-label="Selected experience">
    {experienceEntries}
  </ol>
</section>

<section class="ef-section" aria-labelledby="more-title">
  <header class="ef-section-heading">
    <div class="ef-section-heading__text">
      <p class="ef-eyebrow">More</p>
      <h2 id="more-title">Speaking, background, and contact</h2>
    </div>
    <div class="ef-actions">
      <a class="ef-button" href="/talks.html">Speaking</a>
      <a class="ef-button" href="/about/">About</a>
      <a class="ef-button" href="/contact/">Contact</a>
    </div>
  </header>
</section>"""

        shell "" siteName "Engineering leadership, systems diagnostics, modernization, AI, security, and decision-making under constraint." siteUrl content

    let archive (posts: ValidatedPost list) =
        let published =
            posts
            |> List.filter (fun post -> ValidatedPost.status post = PublicationStatus.Published)
            |> List.sortByDescending (ValidatedPost.identity >> SourceIdentity.legacyDate)

        let yearSections =
            published
            |> List.groupBy (ValidatedPost.identity >> fun identity -> identity.Year)
            |> List.sortByDescending fst
            |> List.map (fun (year, yearPosts) ->
                let entries =
                    yearPosts
                    |> List.mapi (fun index post ->
                        let identity = ValidatedPost.identity post
                        let date = SourceIdentity.legacyDate identity
                        let marker = sprintf "%02d / %02d" date.Month date.Day
                        let topic =
                            match ValidatedPost.categories post with
                            | first :: _ -> first
                            | [] -> "Writing"

                        let summary =
                            ValidatedPost.description post
                            |> Option.defaultValue "Essay"

                        indexItem marker topic (ValidatedPost.title post) summary (ValidatedPost.route post))
                    |> String.concat "\n"

                $"""<section class="ef-section" aria-labelledby="writing-{year}">
  <header class="ef-section-heading">
    <div class="ef-section-heading__text">
      <p class="ef-eyebrow">Archive</p>
      <h2 id="writing-{year}">{year}</h2>
    </div>
  </header>
  <ol class="ef-index" aria-label="Writing from {year}">
    {entries}
  </ol>
</section>""")
            |> String.concat "\n"

        let content =
            $"""<section class="ef-hero" aria-labelledby="writing-title">
  <div class="ef-hero__content">
    <p class="ef-eyebrow">Writing</p>
    <h1 class="ef-hero__title" id="writing-title">Engineering, systems, leadership, and the evidence behind difficult decisions.</h1>
    <p class="ef-lead">A chronological archive of essays and technical notes.</p>
  </div>
</section>
{yearSections}"""

        shell "writing" "Writing" "Essays and technical notes by Kevin M. Miller." (routeUrl "/blog/") content

    let article post bodyHtml previousPost nextPost =
        let identity = ValidatedPost.identity post
        let publishedDate = SourceIdentity.legacyDate identity
        let fallbackDescription =
            sprintf "An article by Kevin M. Miller, published %s." (dateText publishedDate)

        let description =
            ValidatedPost.description post
            |> Option.defaultValue fallbackDescription

        let topics =
            (ValidatedPost.categories post @ ValidatedPost.tags post)
            |> List.distinct
            |> List.truncate 6
            |> List.map (fun topic -> $"<span>{encode topic}</span>")
            |> String.concat " · "

        let topicLine =
            if String.IsNullOrWhiteSpace(topics) then
                ""
            else
                $"<p class=\"ef-eyebrow\">{topics}</p>"

        let adjacent label candidate =
            match candidate with
            | None -> ""
            | Some other ->
                $"<a class=\"ef-button\" href=\"{encode (ValidatedPost.route other)}\">{encode label}: {encode (ValidatedPost.title other)}</a>"

        let content =
            $"""<article>
  <header class="ef-hero" aria-labelledby="article-title">
    <div class="ef-hero__content">
      <p class="ef-eyebrow">Writing · {encode (dateText publishedDate)}</p>
      <h1 class="ef-hero__title" id="article-title">{encode (ValidatedPost.title post)}</h1>
      <p class="ef-lead">{encode description}</p>
      {topicLine}
    </div>
  </header>

  <section class="ef-section" aria-label="Article">
    <article class="ef-prose">
      {bodyHtml}
    </article>
  </section>

  <section class="ef-section" aria-labelledby="continue-reading-title">
    <header class="ef-section-heading">
      <div class="ef-section-heading__text">
        <p class="ef-eyebrow">Continue reading</p>
        <h2 id="continue-reading-title">More writing</h2>
      </div>
    </header>
    <div class="ef-actions">
      {adjacent "Newer" nextPost}
      {adjacent "Older" previousPost}
      <a class="ef-button" href="/blog/">Writing archive</a>
    </div>
  </section>
</article>"""

        shell "writing" (ValidatedPost.title post) description (routeUrl (ValidatedPost.route post)) content
