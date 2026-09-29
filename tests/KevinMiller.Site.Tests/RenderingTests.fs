namespace KevinMiller.Site.Tests

open KevinMiller.Site
open Xunit

type RenderingTests() =
    let assertFormaShell (html: string) =
        Assert.Contains("class=\"ef-site\"", html)
        Assert.Contains("class=\"ef-site-header\"", html)
        Assert.Contains("class=\"ef-site-footer\"", html)
        Assert.Contains("/assets/forma/forma-echelon-marketing.css", html)
        Assert.Contains("/site.css", html)

    [<Fact>]
    member _.AboutUsesEditorialFormaSurface() =
        let html = Rendering.about ()
        assertFormaShell html
        Assert.Contains("<link rel=\"canonical\" href=\"https://kevinmmiller.us/about/\">", html)
        Assert.Contains("class=\"ef-prose\"", html)
        Assert.DoesNotContain("grid-two", html)
        Assert.DoesNotContain("card-title", html)

    [<Fact>]
    member _.ContactIsStaticAndRequiresNoJavaScript() =
        let html = Rendering.contact ()
        assertFormaShell html
        Assert.Contains("<link rel=\"canonical\" href=\"https://kevinmmiller.us/contact/\">", html)
        Assert.DoesNotContain("<form", html)
        Assert.DoesNotContain("<script", html)
        Assert.DoesNotContain("contact.js", html)
        Assert.Contains("https://echelonfoundry.com", html)
        Assert.Contains("https://www.linkedin.com/in/kemiller2002", html)
        Assert.Contains("https://github.com/kemiller2002/", html)

    [<Fact>]
    member _.SpeakingUsesRepresentativeEditorialIndex() =
        let html = Rendering.speaking ()
        assertFormaShell html
        Assert.Contains("<link rel=\"canonical\" href=\"https://kevinmmiller.us/talks.html\">", html)
        Assert.Contains("Representative talks", html)
        Assert.Contains("The Diagnostic Mindset", html)
        Assert.Contains("AI Adoption That Preserves Judgment", html)
        Assert.DoesNotContain("talk-card", html)
        Assert.DoesNotContain("speaking-layout", html)

    [<Fact>]
    member _.WorkUsesFormaIndexesInsteadOfLegacyCardGrid() =
        let html = Rendering.work ()
        assertFormaShell html
        Assert.Contains("<link rel=\"canonical\" href=\"https://kevinmmiller.us/echelon-systems/\">", html)
        Assert.Contains("Praxis", html)
        Assert.Contains("Ordo", html)
        Assert.Contains("Forma", html)
        Assert.Contains("Limen", html)
        Assert.DoesNotContain("grid-two", html)
        Assert.DoesNotContain("card-title", html)

    [<Fact>]
    member _.SpeakerBioPreservesLegacyPublicPage() =
        let html = Rendering.speakerBio ()
        assertFormaShell html
        Assert.Contains("<link rel=\"canonical\" href=\"https://kevinmmiller.us/speaker-bio.html\">", html)
        Assert.Contains("aria-current=\"page\"", html)
        Assert.Contains("View talks", html)

    [<Fact>]
    member _.NotFoundPageUsesTheStaticFormaShell() =
        let html = Rendering.notFound ()
        assertFormaShell html
        Assert.Contains("<link rel=\"canonical\" href=\"https://kevinmmiller.us/404.html\">", html)
        Assert.Contains("That page is not here.", html)

    [<Fact>]
    member _.HistoricallyPublishedDraftCompatibilityPageIsNoIndex() =
        let source =
            """---
title: "Private Draft"
date: 2025-03-31
published: false
---
Draft content that must not be republished.
"""

        let identity =
            match SourceIdentity.tryCreate "site-src/posts/2025-03-31-private-draft.md" with
            | Ok value -> value
            | Error finding -> failwith finding.Message

        let document =
            match FrontMatterParser.parse identity.SourcePath source with
            | Ok value -> value
            | Error findings -> failwithf "Expected draft metadata to parse, got %A" findings

        let post =
            match Publication.validate identity document with
            | Ok(value, _) -> value
            | Error findings -> failwithf "Expected draft to validate, got %A" findings

        let html = Rendering.unpublishedLegacy post

        Assert.Contains("<meta name=\"robots\" content=\"noindex, nofollow\">", html)
        Assert.Contains("This article is not currently published.", html)
        Assert.DoesNotContain("Draft content that must not be republished.", html)

