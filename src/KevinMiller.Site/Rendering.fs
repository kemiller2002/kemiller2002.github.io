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

    let private shell current openGraphType title description canonical content =
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
  <meta property="og:type" content="{encode openGraphType}">
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

    let private indexItemWithAction marker eyebrow title summary href action =
        $"""<li class="ef-index__item">
  <p class="ef-index__marker">{encode marker}</p>
  <div class="ef-index__heading">
    <p class="ef-eyebrow">{encode eyebrow}</p>
    <h3 class="ef-index__title">{encode title}</h3>
  </div>
  <p class="ef-index__summary">{encode summary}</p>
  <a class="ef-index__link" href="{encode href}">{encode action}</a>
</li>"""

    let private indexItem marker eyebrow title summary href =
        indexItemWithAction marker eyebrow title summary href "Read"

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
            [ indexItemWithAction "W / 01" "Engineering practice" "Echelon Foundry" "Advisory, research, and engineering systems for consequential software decisions." "https://echelonfoundry.com" "Explore"
              indexItemWithAction "W / 02" "State and legality" "Ordo" "State-Directed Engineering: explicit state, legal transitions, capabilities, obligations, and evidence." "https://github.com/kemiller2002/ordo" "Explore"
              indexItemWithAction "W / 03" "Work and evidence" "Praxis" "Repository work, provenance, execution evidence, validation, and durable handoff." "https://github.com/kemiller2002/praxis" "Explore"
              indexItemWithAction "W / 04" "Presentation and boundaries" "Forma + Limen" "Shared semantic presentation through Forma, with Limen reserved for explicit browser/application boundaries." "https://github.com/kemiller2002/forma" "Explore" ]
            |> String.concat "\n"

        let experienceEntries =
            [ indexItemWithAction "2020–25" "Director of Engineering" "Ren" "Led nine engineering teams; release flow moved from six weeks to two days while operating cost and hotfix pressure fell." "/about/" "About"
              indexItemWithAction "2018–20" "Senior Developer" "T2 Systems" "Stabilized large deployment footprints, built CLR diagnostics, and improved delivery practices across production environments." "/about/" "About"
              indexItemWithAction "2015–18" "Development Manager" "TCC Software Solutions" "Led public-sector and enterprise engineering work with an emphasis on architecture, delivery discipline, and operational clarity." "/about/" "About" ]
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

        shell "" "website" siteName "Engineering leadership, systems diagnostics, modernization, AI, security, and decision-making under constraint." siteUrl content

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

        shell "writing" "website" "Writing" "Essays and technical notes by Kevin M. Miller." (routeUrl "/blog/") content

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

        shell "writing" "article" (ValidatedPost.title post) description (routeUrl (ValidatedPost.route post)) content


    let about () =
        let content =
            """<section class="ef-hero" aria-labelledby="about-title">
  <div class="ef-hero__content">
    <p class="ef-eyebrow">About</p>
    <h1 class="ef-hero__title" id="about-title">I build and diagnose software systems where context matters.</h1>
    <p class="ef-lead">My work combines engineering leadership, architecture, diagnostics, modernization, AI, security, and evidence-based decision-making.</p>
  </div>
</section>

<section class="ef-section" aria-labelledby="about-work-title">
  <header class="ef-section-heading">
    <div class="ef-section-heading__text">
      <p class="ef-eyebrow">Practice</p>
      <h2 id="about-work-title">The recurring problem is not technology. It is understanding the system well enough to change it safely.</h2>
    </div>
  </header>
  <div class="ef-prose">
    <p>Over more than 25 years, I have led engineering organizations, modernized enterprise platforms, built cloud and AI systems, and worked through environments where legacy software, organizational incentives, security constraints, and incomplete evidence all interact.</p>
    <p>I approach difficult technical work diagnostically rather than ideologically. The first job is to establish what is actually happening, which assumptions are doing hidden work, what evidence supports the current explanation, and which decisions remain reversible.</p>
    <p>That approach now shows up through Echelon Foundry and the systems around it: Ordo for explicit state and legal transitions, Praxis for governed work and evidence, Forma for presentation, Limen for browser boundaries, and related assurance and diagnostic systems.</p>
    <p>I also write and speak about engineering judgment, failure patterns, modernization, AI adoption, and decision-making under constraint.</p>
  </div>
</section>

<section class="ef-section" aria-labelledby="about-pattern-title">
  <header class="ef-section-heading">
    <div class="ef-section-heading__text">
      <p class="ef-eyebrow">Operating pattern</p>
      <h2 id="about-pattern-title">Evidence first. Explicit constraints. Reversible moves when uncertainty is high.</h2>
    </div>
  </header>
  <ol class="ef-index" aria-label="How I work">
    <li class="ef-index__item"><p class="ef-index__marker">01</p><div class="ef-index__heading"><p class="ef-eyebrow">Diagnose</p><h3 class="ef-index__title">Separate symptoms from mechanisms</h3></div><p class="ef-index__summary">Collect enough evidence to explain why the system behaves as it does before prescribing a rewrite, reorg, or new tool.</p><a class="ef-index__link" href="/blog/">Writing</a></li>
    <li class="ef-index__item"><p class="ef-index__marker">02</p><div class="ef-index__heading"><p class="ef-eyebrow">Constrain</p><h3 class="ef-index__title">Make legal behavior explicit</h3></div><p class="ef-index__summary">Use state, types, boundaries, and governance to eliminate failure modes rather than relying on people to remember rules.</p><a class="ef-index__link" href="/echelon-systems/">Work</a></li>
    <li class="ef-index__item"><p class="ef-index__marker">03</p><div class="ef-index__heading"><p class="ef-eyebrow">Verify</p><h3 class="ef-index__title">Make important claims inspectable</h3></div><p class="ef-index__summary">Treat tests, provenance, diagnostics, and communication as evidence about the system, not ceremonial paperwork around it.</p><a class="ef-index__link" href="/contact/">Contact</a></li>
  </ol>
</section>"""

        shell "about" "profile" "About" "About Kevin M. Miller: engineering leadership, systems diagnostics, architecture, and evidence-based decision-making." (routeUrl "/about/") content

    let contact () =
        let content =
            """<section class="ef-hero" aria-labelledby="contact-title">
  <div class="ef-hero__content">
    <p class="ef-eyebrow">Contact</p>
    <h1 class="ef-hero__title" id="contact-title">Bring me the version of the problem people are avoiding.</h1>
    <p class="ef-lead">For consulting, technical collaboration, speaking, or a difficult system that needs a second set of eyes, use one of the direct paths below.</p>
  </div>
</section>

<section class="ef-section" aria-labelledby="contact-paths-title">
  <header class="ef-section-heading">
    <div class="ef-section-heading__text">
      <p class="ef-eyebrow">Contact paths</p>
      <h2 id="contact-paths-title">Start with the context that best fits the work.</h2>
    </div>
  </header>
  <ol class="ef-index" aria-label="Contact options">
    <li class="ef-index__item"><p class="ef-index__marker">01</p><div class="ef-index__heading"><p class="ef-eyebrow">Consulting</p><h3 class="ef-index__title">Echelon Foundry</h3></div><p class="ef-index__summary">Modernization, diagnostics, AI adoption, architecture, security, and engineering-system work.</p><a class="ef-index__link" href="https://echelonfoundry.com">Echelon Foundry</a></li>
    <li class="ef-index__item"><p class="ef-index__marker">02</p><div class="ef-index__heading"><p class="ef-eyebrow">Professional</p><h3 class="ef-index__title">LinkedIn</h3></div><p class="ef-index__summary">A direct path for professional introductions, leadership conversations, and speaking inquiries.</p><a class="ef-index__link" href="https://www.linkedin.com/in/kemiller2002" rel="me">LinkedIn</a></li>
    <li class="ef-index__item"><p class="ef-index__marker">03</p><div class="ef-index__heading"><p class="ef-eyebrow">Engineering</p><h3 class="ef-index__title">GitHub</h3></div><p class="ef-index__summary">Repositories, current engineering systems, open issues, and implementation work.</p><a class="ef-index__link" href="https://github.com/kemiller2002/" rel="me">GitHub</a></li>
  </ol>
</section>"""

        shell "contact" "profile" "Contact" "Contact Kevin M. Miller about engineering consulting, collaboration, or speaking." (routeUrl "/contact/") content

    let speaking () =
        let talks =
            [ indexItemWithAction "01" "Systems · Keynote" "The Diagnostic Mindset: How to See What Others Miss" "A cross-domain approach to separating symptoms from system behavior and finding the evidence that changes the diagnosis." "/contact/" "Discuss this talk"
              indexItemWithAction "02" "Failure · Keynote" "Failure Has a Pattern" "What software incidents, operational failures, and other complex systems teach about drift, coupling, incentives, and early warning signals." "/contact/" "Discuss this talk"
              indexItemWithAction "03" "Architecture" "Architecture as a System of Constraints" "Why architecture is less about diagrams than about making dangerous choices difficult and desirable behavior easy to preserve." "/contact/" "Discuss this talk"
              indexItemWithAction "04" "Engineering" "How to Read a Codebase Like a Detective" "A forensic approach to code, tests, names, structure, history, and the organizational decisions embedded in a system." "/contact/" "Discuss this talk"
              indexItemWithAction "05" "AI · Judgment" "AI Adoption That Preserves Judgment" "How to use AI to compress work and ambiguity without quietly outsourcing accountability, evidence, or domain authority." "/contact/" "Discuss this talk"
              indexItemWithAction "06" "Leadership" "Decision-Making Under Pressure" "A practical model for acting with incomplete information, distinguishing reversible from irreversible moves, and updating as evidence changes." "/contact/" "Discuss this talk" ]
            |> String.concat "
"

        let content =
            $"""<section class="ef-hero" aria-labelledby="speaking-title">
  <div class="ef-hero__content">
    <p class="ef-eyebrow">Speaking</p>
    <h1 class="ef-hero__title" id="speaking-title">Systems, diagnostics, and the judgment required when the easy story is incomplete.</h1>
    <p class="ef-lead">Talks for engineering conferences, leadership events, and technical teams, grounded in real work across software delivery, architecture, AI, diagnostics, and organizational systems.</p>
    <div class="ef-actions">
      <a class="ef-button" data-ef-variant="primary" href="/contact/">Invite Kevin to speak</a>
    </div>
  </div>
</section>

<section class="ef-section" aria-labelledby="talks-title">
  <header class="ef-section-heading">
    <div class="ef-section-heading__text">
      <p class="ef-eyebrow">Representative talks</p>
      <h2 id="talks-title">Sessions can be adapted to the audience and format.</h2>
      <p>The emphasis is practical: how people reason about complex systems, what evidence changes a decision, and how engineering structures can reduce avoidable failure.</p>
    </div>
  </header>
  <ol class="ef-index" aria-label="Representative talks">
    {talks}
  </ol>
</section>

<section class="ef-section" aria-labelledby="speaker-bio-title">
  <header class="ef-section-heading">
    <div class="ef-section-heading__text">
      <p class="ef-eyebrow">Speaker bio</p>
      <h2 id="speaker-bio-title">Engineering leadership informed by systems thinking and cross-domain diagnostics.</h2>
    </div>
  </header>
  <div class="ef-prose">
    <p>Kevin M. Miller is an engineering leader, systems strategist, and founder of Echelon Foundry. His work focuses on modernization, explicit system design, diagnostics, AI adoption, security, and technical decision-making under constraint.</p>
    <p>He has led engineering organizations and enterprise platform work for more than 25 years and writes about how systems fail, how teams reason under uncertainty, and how architecture can preserve judgment instead of hiding it.</p>
  </div>
</section>"""

        shell "speaking" "profile" "Speaking" "Talks by Kevin M. Miller on systems, diagnostics, architecture, AI, engineering leadership, and decision-making under uncertainty." (routeUrl "/talks.html") content

    let work () =
        let group eyebrow title entries =
            let rendered =
                entries
                |> List.mapi (fun index (name, summary, href) ->
                    indexItemWithAction (sprintf "%02d" (index + 1)) eyebrow name summary href "Explore")
                |> String.concat "
"

            $"""<section class="ef-section" aria-labelledby="{encode title}-title">
  <header class="ef-section-heading">
    <div class="ef-section-heading__text">
      <p class="ef-eyebrow">{encode eyebrow}</p>
      <h2 id="{encode title}-title">{encode title}</h2>
    </div>
  </header>
  <ol class="ef-index" aria-label="{encode title}">
    {rendered}
  </ol>
</section>"""

        let foundation =
            group
                "Engineering foundation"
                "Govern the work and the state"
                [ "Praxis", "Repository work protocol, provenance, execution evidence, validation, diagnostics, and durable handoff.", "https://github.com/kemiller2002/praxis"
                  "Ordo", "State-Directed Engineering: explicit state, legal transitions, capabilities, obligations, evidence, and unknown effects.", "https://github.com/kemiller2002/ordo"
                  "Visual Engineering", "Evidence-bounded UI research and decision criteria for hierarchy, typography, color, composition, and interaction.", "https://github.com/kemiller2002/visual-engineering"
                  "Communication Engineering", "Evidence-bounded communication practice organized around purpose, audience, proof obligations, context, and consequence.", "https://github.com/kemiller2002/communication-engineering" ]

        let assurance =
            group
                "Assurance"
                "Make important claims prove themselves"
                [ "Tutela", "Security engineering around assets, trust boundaries, threats, controls, unknowns, evidence, and explicit release posture.", "https://github.com/kemiller2002/tutela"
                  "Aegis", "Typed operational fault handling and recovery for .NET applications, including redaction, containment, diagnostics, and durable evidence.", "https://github.com/kemiller2002/aegis"
                  "Dokimos", "Evidence-based code quality over time with immutable observations, regressions, improvements, hotspots, and attribution.", "https://github.com/kemiller2002/dokimos"
                  "Percepta", "Governs what users must be able to perceive about application state, actions, constraints, and unresolved work.", "https://github.com/kemiller2002/percepta" ]

        let boundaries =
            group
                "Experience and boundaries"
                "Build the surface without surrendering authority"
                [ "Forma", "Zero-runtime semantic HTML and CSS design system with tokens, accessibility contracts, reusable patterns, and validated presentation.", "https://github.com/kemiller2002/forma"
                  "Folio", "Standards-first print components and layout primitives for professional HTML, paper, and PDF output.", "https://github.com/kemiller2002/folio"
                  "Limen", "A browser boundary that keeps browser capabilities separate from application authority and domain state.", "https://github.com/kemiller2002/limen"
                  "Conditor", "Declarative repository initialization that resolves, installs, verifies, locks, scaffolds, and hands off governed environments.", "https://github.com/kemiller2002/conditor" ]

        let specialized =
            group
                "Specialized systems"
                "Apply the same discipline to harder domains"
                [ "Strata", "Declarative PostgreSQL schema management that distinguishes mismatch from not-compared, not-modelled, and unverifiable.", "https://github.com/kemiller2002/strata"
                  "Echelon Diagnostic Framework", "A diagnostic grammar for evidence boundaries, hypotheses, confidence policy, controlled validation, and challengeable diagnoses.", "https://github.com/kemiller2002/echelon-diagnostic-framework"
                  "Clarity", "Makes beliefs, assumptions, evidence, confidence, ownership, monitoring, and reconsideration criteria explicit.", "https://github.com/kemiller2002/clarity-framework"
                  "HelixNote", "Ledger-first documentation and reasoning for cases where facts, timelines, contradictions, uncertainty, and provenance matter.", "https://github.com/kemiller2002/helix-note-application"
                  "Chrona", "Time-entry work designed to participate in a governed project-administration flow rather than exist as detached CRUD.", "https://github.com/kemiller2002/chrona"
                  "Summa", "Project-administration and financial-work foundation coordinating independently authoritative repositories.", "https://github.com/kemiller2002/summa"
                  "Signal", "A structured-assessment pilot built on the same repository operating discipline and evidence model.", "https://github.com/kemiller2002/signal" ]

        let content =
            $"""<section class="ef-hero" aria-labelledby="work-title">
  <div class="ef-hero__content">
    <p class="ef-eyebrow">Current work</p>
    <h1 class="ef-hero__title" id="work-title">A connected engineering ecosystem for making state, authority, evidence, quality, and presentation explicit.</h1>
    <p class="ef-lead">The point is not a larger toolchain. Each system has a bounded responsibility, and the systems reinforce one another without collapsing those responsibilities into one framework.</p>
  </div>
</section>
{foundation}
{assurance}
{boundaries}
{specialized}

<section class="ef-section" aria-labelledby="work-principle-title">
  <div class="ef-cta">
    <div class="ef-cta__text">
      <p class="ef-eyebrow">Echelon Foundry</p>
      <h2 id="work-principle-title">One ecosystem, not a bag of tools.</h2>
      <p>Ordo constrains meaning, Praxis preserves work and evidence, Forma shapes presentation, Limen protects browser boundaries, and the assurance systems test the claims around them.</p>
    </div>
    <div class="ef-actions">
      <a class="ef-button" data-ef-variant="primary" href="https://echelonfoundry.com">Explore Echelon Foundry</a>
    </div>
  </div>
</section>"""

        shell "work" "website" "Work" "The Echelon systems: engineering governance, explicit state, assurance, presentation, browser boundaries, diagnostics, and supporting tools." (routeUrl "/echelon-systems/") content
