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

        Assert.Contains(">Heading</h1>", html)
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
