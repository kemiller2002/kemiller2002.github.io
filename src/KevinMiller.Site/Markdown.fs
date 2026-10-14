namespace KevinMiller.Site

open System.Text.RegularExpressions
open Markdig

module MarkdownRenderer =
    let private pipeline = MarkdownPipelineBuilder().UseAdvancedExtensions().Build()

    let private h1Pattern = Regex("""<h1\b""", RegexOptions.IgnoreCase ||| RegexOptions.Compiled)

    let private headingTagPattern =
        Regex("""<(?<close>/?)h(?<level>[1-6])\b""", RegexOptions.IgnoreCase ||| RegexOptions.Compiled)

    let private preBlockPattern =
        Regex("""<pre\b.*?</pre>""", RegexOptions.IgnoreCase ||| RegexOptions.Singleline ||| RegexOptions.Compiled)

    /// The page shell owns the single level-one heading. A body that uses h1
    /// (Markdown "#" or historical raw HTML) is shifted one level down so the
    /// document keeps one h1 and its relative outline.
    let demoteHeadings (html: string) =
        if h1Pattern.IsMatch(html) then
            headingTagPattern.Replace(
                html,
                MatchEvaluator(fun matched ->
                    let level = min 6 (int matched.Groups["level"].Value + 1)
                    let close = matched.Groups["close"].Value
                    $"<{close}h{level}")
            )
        else
            html

    /// Code keeps its line structure, so a long line scrolls inside a labeled,
    /// keyboard-focusable Forma bounded-overflow container instead of widening
    /// the page (visual-engineering ULI-L05: contained, labeled scroller).
    let containCodeBlocks (html: string) =
        preBlockPattern.Replace(
            html,
            MatchEvaluator(fun matched ->
                $"""<div class="ef-bounded-overflow" role="group" aria-label="Code sample" tabindex="0">{matched.Value}</div>""")
        )

    let render (markdown: string) =
        Markdown.ToHtml(markdown, pipeline) |> demoteHeadings |> containCodeBlocks
