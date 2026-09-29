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
    member _.MissingOpeningFrontMatterFenceIsAnExplicitError() =
        let source =
            """layout: post
title: "Why Small Steps Beat Big Goals Every Time"
date: 2025-02-19
---

Everyone loves big goals.
"""

        match FrontMatterParser.parse "site-src/posts/2025-2-19-never-start.md" source with
        | Ok _ -> failwith "Expected malformed front matter to fail."
        | Error findings ->
            Assert.True(findings |> List.exists (fun finding -> finding.Code = "FRONT-MATTER-OPEN"))

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
