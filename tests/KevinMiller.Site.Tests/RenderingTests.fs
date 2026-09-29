namespace KevinMiller.Site.Tests

open KevinMiller.Site
open Xunit

type RenderingTests() =
    let assertFormaShell (html: string) =
        Assert.Contains("class=\"ef-site\"", html)
        Assert.Contains("class=\"ef-site-header\"", html)
        Assert.Contains("class=\"ef-site-footer\"", html)
        Assert.Contains("/assets/forma/forma-echelon-marketing.css", html)

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
