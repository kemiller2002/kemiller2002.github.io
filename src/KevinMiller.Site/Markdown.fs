namespace KevinMiller.Site

open Markdig

module MarkdownRenderer =
    let private pipeline = MarkdownPipelineBuilder().UseAdvancedExtensions().Build()

    let render (markdown: string) =
        Markdown.ToHtml(markdown, pipeline)
