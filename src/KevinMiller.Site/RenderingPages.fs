namespace KevinMiller.Site

open System

module RenderingPages =
    open RenderingCore

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


    let speakerBio () =
        let content =
            """<section class="ef-hero" aria-labelledby="speaker-bio-title">
  <div class="ef-hero__content">
    <p class="ef-eyebrow">Speaker bio</p>
    <h1 class="ef-hero__title" id="speaker-bio-title">Kevin M. Miller</h1>
    <p class="ef-lead">Engineering leader, systems strategist, writer, and founder of Echelon Foundry.</p>
  </div>
</section>

<section class="ef-section" aria-labelledby="speaker-bio-copy-title">
  <header class="ef-section-heading">
    <div class="ef-section-heading__text">
      <p class="ef-eyebrow">Bio</p>
      <h2 id="speaker-bio-copy-title">Engineering judgment for systems under constraint</h2>
    </div>
  </header>
  <div class="ef-prose">
    <p>Kevin M. Miller has spent more than 25 years building, modernizing, diagnosing, and leading software systems. His work spans engineering leadership, architecture, delivery systems, AI adoption, security, diagnostics, and technical decision-making.</p>
    <p>He is the founder of Echelon Foundry, where he develops engineering systems that make state, evidence, authority, and important technical claims explicit enough to inspect. His talks focus on systems thinking, failure patterns, modernization, architecture, AI, and decision-making under uncertainty.</p>
  </div>
  <div class="ef-actions">
    <a class="ef-button" data-ef-variant="primary" href="/talks.html">View talks</a>
    <a class="ef-button" href="/contact/">Speaking inquiries</a>
  </div>
</section>"""

        shell "speaking" "profile" "Speaker Bio" "Speaker bio for Kevin M. Miller, engineering leader, systems strategist, writer, and founder of Echelon Foundry." (routeUrl "/speaker-bio.html") content

    let notFound () =
        let content =
            """<section class="ef-hero" aria-labelledby="not-found-title">
  <div class="ef-hero__content">
    <p class="ef-eyebrow">404</p>
    <h1 class="ef-hero__title" id="not-found-title">That page is not here.</h1>
    <p class="ef-lead">The address may be old, mistyped, or no longer part of the public site.</p>
    <div class="ef-actions">
      <a class="ef-button" data-ef-variant="primary" href="/">Go home</a>
      <a class="ef-button" href="/blog/">Browse writing</a>
    </div>
  </div>
</section>"""

        shell "" "website" "Not Found" "The requested page could not be found." (routeUrl "/404.html") content


    let unpublishedLegacy (post: ValidatedPost) =
        let identity = ValidatedPost.identity post
        let canonical = routeUrl (ValidatedPost.route post)

        let content =
            $"""<section class="ef-hero" aria-labelledby="unpublished-title">
  <div class="ef-hero__content">
    <p class="ef-eyebrow">Unpublished</p>
    <h1 class="ef-hero__title" id="unpublished-title">{encode (ValidatedPost.title post)}</h1>
    <p class="ef-lead">This article is not currently published. This compatibility page preserves a URL that existed on an earlier version of the site without republishing draft content.</p>
    <div class="ef-actions">
      <a class="ef-button" data-ef-variant="primary" href="/blog/">Browse current writing</a>
    </div>
  </div>
</section>"""

        shellWithRobots
            "writing"
            "article"
            (ValidatedPost.title post)
            "This article is not currently published."
            canonical
            (Some "noindex, nofollow")
            content

