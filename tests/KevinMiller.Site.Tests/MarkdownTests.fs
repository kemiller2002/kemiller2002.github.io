namespace KevinMiller.Site.Tests

open KevinMiller.Site
open Xunit

type MarkdownTests() =
    [<Fact>]
    member _.RendersTheSupportedMarkdownBoundary() =
        let markdown =
            """# Heading

> Quoted evidence.

- one
- two

1. first
2. second

**strong** and *emphasis* with [a link](https://example.com).

![Diagram](https://example.com/diagram.png)

```fsharp
let answer = 42
```
"""

        let html = MarkdownRenderer.render markdown

        // The page shell owns the only h1; body headings move one level down.
        Assert.Contains(">Heading</h2>", html)
        Assert.DoesNotContain("<h1", html)
        Assert.Contains("<blockquote>", html)
        Assert.Contains("<ul>", html)
        Assert.Contains("<ol>", html)
        Assert.Contains("<strong>strong</strong>", html)
        Assert.Contains("<em>emphasis</em>", html)
        Assert.Contains("href=\"https://example.com\"", html)
        Assert.Contains("src=\"https://example.com/diagram.png\"", html)
        Assert.Contains("language-fsharp", html)
        Assert.Contains("let answer = 42", html)

    [<Fact>]
    member _.PreservesExplicitHistoricalRawHtml() =
        let markdown =
            """Before.

<figure class="legacy-figure">
  <img src="https://example.com/legacy.png" alt="Legacy image">
  <figcaption>Historical caption</figcaption>
</figure>

After.
"""

        let html = MarkdownRenderer.render markdown

        Assert.Contains("<figure class=\"legacy-figure\">", html)
        Assert.Contains("<figcaption>Historical caption</figcaption>", html)
        Assert.Contains("<p>Before.</p>", html)
        Assert.Contains("<p>After.</p>", html)

    [<Fact>]
    member _.DemotesBodyHeadingsOnlyWhenTheBodyUsesH1() =
        let withH1 = MarkdownRenderer.render "# Title\n\n## Section\n\n<h1>Raw</h1>\n"
        Assert.DoesNotContain("<h1", withH1)
        Assert.Contains(">Title</h2>", withH1)
        Assert.Contains(">Section</h3>", withH1)
        Assert.Contains("<h2>Raw</h2>", withH1)

        let withoutH1 = MarkdownRenderer.render "## Section\n\n### Detail\n"
        Assert.Contains(">Section</h2>", withoutH1)
        Assert.Contains(">Detail</h3>", withoutH1)

    [<Fact>]
    member _.ContainsCodeBlocksInALabeledScroller() =
        let html = MarkdownRenderer.render "```\nlet aVeryLongLine = 1\n```\n\nText.\n"
        Assert.Contains("""<div class="ef-bounded-overflow" role="group" aria-label="Code sample" tabindex="0"><pre>""", html)
        Assert.Contains("</pre></div>", html)
        Assert.Contains("<p>Text.</p>", html)
