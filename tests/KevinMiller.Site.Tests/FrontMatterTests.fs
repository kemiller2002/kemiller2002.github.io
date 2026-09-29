namespace KevinMiller.Site.Tests

open System
open KevinMiller.Site
open Xunit

type FrontMatterTests() =
    [<Fact>]
    member _.ParsesInlineTagsAndCategories() =
        let source =
            """---
title: "Where the Magic Metaphor Breaks"
date: 2025-12-28
categories: [negotiation, leadership, ethics]
tags: [negotiation, consent, power, decision-making]
---
Body.
"""

        match FrontMatterParser.parse "post.md" source with
        | Error findings ->
            failwithf "Expected metadata to parse, got %A" findings
        | Ok parsed ->
            Assert.Equal<string list>([ "negotiation"; "leadership"; "ethics" ], parsed.Metadata.Categories)

            Assert.Equal<string list>(
                [ "negotiation"; "consent"; "power"; "decision-making" ],
                parsed.Metadata.Tags
            )

    [<Fact>]
    member _.ParsesIndentedStringLists() =
        let source =
            """---
title: "Types"
date: 2023-10-04
categories:
  - types
  - JavaScript
tags:
  - language
  - design
---
Body.
"""

        match FrontMatterParser.parse "post.md" source with
        | Error findings ->
            failwithf "Expected metadata to parse, got %A" findings
        | Ok parsed ->
            Assert.Equal<string list>([ "types"; "JavaScript" ], parsed.Metadata.Categories)
            Assert.Equal<string list>([ "language"; "design" ], parsed.Metadata.Tags)

    [<Fact>]
    member _.ParsesFoldedSummaryAsDescription() =
        let source =
            """---
title: "When Validated Frameworks Stop Being True"
date: 2026-01-04
summary: >
  Why frameworks that perform well in test environments often fail when expanded,
  and how internal validity quietly gets mistaken for truth.
---
Body.
"""

        match FrontMatterParser.parse "post.md" source with
        | Error findings ->
            failwithf "Expected folded metadata to parse, got %A" findings
        | Ok parsed ->
            Assert.Equal(
                Some "Why frameworks that perform well in test environments often fail when expanded, and how internal validity quietly gets mistaken for truth.",
                parsed.Metadata.Description
            )

    [<Fact>]
    member _.PublishedFalseCreatesAValidDraft() =
        let source =
            """---
title: "Draft"
date: 2025-03-31
published: false
---
Body.
"""

        let identity =
            match SourceIdentity.tryCreate "site-src/posts/2025-03-31-draft.md" with
            | Ok value -> value
            | Error finding -> failwith finding.Message

        let document =
            match FrontMatterParser.parse identity.SourcePath source with
            | Ok value -> value
            | Error findings -> failwithf "Expected metadata to parse, got %A" findings

        match Publication.validate identity document with
        | Error findings -> failwithf "Expected draft to validate, got %A" findings
        | Ok(post, _) -> Assert.Equal(PublicationStatus.Draft, ValidatedPost.status post)

    [<Fact>]
    member _.MissingOpeningFrontMatterFenceIsAcceptedWithMigrationWarning() =
        let source =
            """layout: post
title: "Why Small Steps Beat Big Goals Every Time"
date: 2025-02-19
---

Everyone loves big goals.
"""

        let identity =
            match SourceIdentity.tryCreate "site-src/posts/2025-2-19-never-start.md" with
            | Ok value -> value
            | Error finding -> failwith finding.Message

        let document =
            match FrontMatterParser.parse identity.SourcePath source with
            | Error findings -> failwithf "Expected legacy metadata to parse, got %A" findings
            | Ok value -> value

        match Publication.validate identity document with
        | Error findings -> failwithf "Expected legacy source to validate with warning, got %A" findings
        | Ok(_, warnings) ->
            Assert.True(
                warnings
                |> List.exists (fun finding ->
                    finding.Code = "FRONT-MATTER-OPEN"
                    && finding.Severity = FindingSeverity.Warning)
            )

    [<Fact>]
    member _.OneDigitFrontMatterDateIsAcceptedForLegacyContent() =
        let source =
            """---
title: "Concrete Forensics"
date: 2025-12-5
---
Body.
"""

        let identity =
            match SourceIdentity.tryCreate "site-src/posts/2025-12-5-didnt-expect-concrete.md" with
            | Ok value -> value
            | Error finding -> failwith finding.Message

        let document =
            match FrontMatterParser.parse identity.SourcePath source with
            | Ok value -> value
            | Error findings -> failwithf "Expected metadata to parse, got %A" findings

        match Publication.validate identity document with
        | Error findings -> failwithf "Expected one-digit date to validate, got %A" findings
        | Ok(post, _) ->
            Assert.Equal(DateOnly(2025, 12, 5), ValidatedPost.declaredDate post)

    [<Fact>]
    member _.FilenameRemainsRouteAuthorityWhenFrontMatterDateDisagrees() =
        let source =
            """---
title: "When AI Fails, It’s Usually a Management Problem"
date: 2026-01-31
---
Body.
"""

        let identity =
            match SourceIdentity.tryCreate "site-src/posts/2026-2-1-ai-cant-do.md" with
            | Ok value -> value
            | Error finding -> failwith finding.Message

        let document =
            match FrontMatterParser.parse identity.SourcePath source with
            | Ok value -> value
            | Error findings -> failwithf "Expected metadata to parse, got %A" findings

        match Publication.validate identity document with
        | Error findings -> failwithf "Expected source to validate with warning, got %A" findings
        | Ok(post, warnings) ->
            Assert.Equal("/2026/02/01/ai-cant-do.html", ValidatedPost.route post)
            Assert.Equal(DateOnly(2026, 1, 31), ValidatedPost.declaredDate post)

            Assert.True(
                warnings
                |> List.exists (fun finding -> finding.Code = "POST-DATE-DISAGREEMENT")
            )

    [<Fact>]
    member _.ReservedSlugCharactersAreReported() =
        let source =
            """---
title: "The Book Of F#"
date: 2014-03-10
---
Body.
"""

        let identity =
            match SourceIdentity.tryCreate "site-src/posts/2014-03-10-The-Book-Of-F#.md" with
            | Ok value -> value
            | Error finding -> failwith finding.Message

        let document =
            match FrontMatterParser.parse identity.SourcePath source with
            | Ok value -> value
            | Error findings -> failwithf "Expected metadata to parse, got %A" findings

        match Publication.validate identity document with
        | Error findings -> failwithf "Expected source to validate with warning, got %A" findings
        | Ok(_, warnings) ->
            Assert.True(
                warnings
                |> List.exists (fun finding -> finding.Code = "POST-SLUG-RESERVED")
            )

    [<Fact>]
    member _.ReservedSlugCharactersArePercentEncodedInPublicRoutes() =
        let hashIdentity =
            match SourceIdentity.tryCreate "site-src/posts/2014-03-10-The-Book-Of-F#.md" with
            | Ok value -> value
            | Error finding -> failwith finding.Message

        let questionIdentity =
            match SourceIdentity.tryCreate "site-src/posts/2014-10-26-I-m-out-of-Range?-You-re-out-of-Range!.md" with
            | Ok value -> value
            | Error finding -> failwith finding.Message

        Assert.Equal("/2014/03/10/The-Book-Of-F%23.html", SourceIdentity.urlPath hashIdentity)
        Assert.Equal(
            "/2014/10/26/I-m-out-of-Range%3F-You-re-out-of-Range%21.html",
            SourceIdentity.urlPath questionIdentity
        )

        Assert.EndsWith("The-Book-Of-F#.html", SourceIdentity.outputPath hashIdentity)
        Assert.EndsWith(
            "I-m-out-of-Range?-You-re-out-of-Range!.html",
            SourceIdentity.outputPath questionIdentity
        )

    [<Fact>]
    member _.RelatedPostsUseOnlyExplicitSharedMetadataAndDeterministicOrder() =
        let post path title date categories tags =
            let source =
                "---\n"
                + "title: \"" + title + "\"\n"
                + "date: " + date + "\n"
                + "categories: [" + System.String.Join(", ", categories) + "]\n"
                + "tags: [" + System.String.Join(", ", tags) + "]\n"
                + "---\nBody.\n"

            let identity =
                match SourceIdentity.tryCreate path with
                | Ok value -> value
                | Error finding -> failwith finding.Message

            let document =
                match FrontMatterParser.parse path source with
                | Ok value -> value
                | Error findings -> failwithf "Expected post to parse, got %A" findings

            match Publication.validate identity document with
            | Ok(value, _) -> value
            | Error findings -> failwithf "Expected post to validate, got %A" findings

        let current =
            post "site-src/posts/2026-01-10-current.md" "Current" "2026-01-10" [ "Engineering" ] [ "AI"; "Evidence" ]

        let strong =
            post "site-src/posts/2026-01-09-strong.md" "Strong" "2026-01-09" [ "Engineering" ] [ "AI"; "Evidence" ]

        let tagOnly =
            post "site-src/posts/2026-01-08-tag.md" "Tag" "2026-01-08" [ "Leadership" ] [ "AI" ]

        let categoryOnly =
            post "site-src/posts/2026-01-07-category.md" "Category" "2026-01-07" [ "Engineering" ] [ "Other" ]

        let unrelated =
            post "site-src/posts/2026-01-06-unrelated.md" "Unrelated" "2026-01-06" [ "Cooking" ] [ "Chocolate" ]

        let related =
            Publication.relatedPosts 3 current [ unrelated; categoryOnly; tagOnly; strong; current ]

        Assert.Equal<string list>(
            [ "Strong"; "Tag"; "Category" ],
            related |> List.map ValidatedPost.title
        )

