namespace KevinMiller.Site

open System

module RenderingEditorial =
    open RenderingCore

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

        shell "home" "website" siteName "Engineering leadership, systems diagnostics, modernization, AI, security, and decision-making under constraint." siteUrl content

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

    let article post bodyHtml previousPost nextPost relatedPosts =
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

        let relatedSection =
            if List.isEmpty relatedPosts then
                ""
            else
                let entries =
                    relatedPosts
                    |> List.mapi (fun index related ->
                        let identity = ValidatedPost.identity related
                        let marker = sprintf "R / %02d" (index + 1)
                        let eyebrow =
                            match ValidatedPost.categories related with
                            | first :: _ -> first
                            | [] -> dateText (SourceIdentity.legacyDate identity)

                        let summary =
                            ValidatedPost.description related
                            |> Option.defaultValue "Related writing"

                        indexItem marker eyebrow (ValidatedPost.title related) summary (ValidatedPost.route related))
                    |> String.concat "\n"

                $"""<section class="ef-section" aria-labelledby="related-writing-title">
  <header class="ef-section-heading">
    <div class="ef-section-heading__text">
      <p class="ef-eyebrow">Related writing</p>
      <h2 id="related-writing-title">On the same explicit topics</h2>
      <p>Related by shared tags and categories from article metadata.</p>
    </div>
  </header>
  <ol class="ef-index" aria-label="Related writing">
    {entries}
  </ol>
</section>"""

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

  {relatedSection}

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



