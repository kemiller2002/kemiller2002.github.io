namespace KevinMiller.Site

open System
open System.Globalization
open System.Xml.Linq

module Syndication =
    let private siteUrl = "https://kevinmmiller.us"
    let private atom = XNamespace.Get("http://www.w3.org/2005/Atom")
    let private sitemapNs = XNamespace.Get("http://www.sitemaps.org/schemas/sitemap/0.9")

    let private fullUrl route =
        if route = "/" then siteUrl else siteUrl + route

    let private isoDate (date: DateOnly) =
        date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)

    let private atomTimestamp (date: DateOnly) =
        isoDate date + "T00:00:00Z"

    let private published posts =
        posts
        |> List.filter (fun post -> ValidatedPost.status post = PublicationStatus.Published)
        |> List.sortByDescending (ValidatedPost.identity >> SourceIdentity.legacyDate)

    let sitemap posts =
        let articleUrls =
            published posts
            |> List.map (fun post ->
                let identity = ValidatedPost.identity post
                XElement(
                    sitemapNs + "url",
                    XElement(sitemapNs + "loc", fullUrl (ValidatedPost.route post)),
                    XElement(sitemapNs + "lastmod", isoDate (SourceIdentity.legacyDate identity))
                ))

        let roots =
            [ XElement(sitemapNs + "url", XElement(sitemapNs + "loc", siteUrl))
              XElement(sitemapNs + "url", XElement(sitemapNs + "loc", fullUrl "/blog/")) ]

        let root = XElement(sitemapNs + "urlset", roots @ articleUrls)
        let document = XDocument(root)
        document.Declaration <- XDeclaration("1.0", "utf-8", null)
        document.ToString(SaveOptions.DisableFormatting)

    let atomFeed posts =
        let entries =
            published posts
            |> List.truncate 20

        let updated =
            match entries with
            | latest :: _ ->
                latest
                |> ValidatedPost.identity
                |> SourceIdentity.legacyDate
                |> atomTimestamp
            | [] -> "1970-01-01T00:00:00Z"

        let entryElements =
            entries
            |> List.map (fun post ->
                let identity = ValidatedPost.identity post
                let route = ValidatedPost.route post
                let url = fullUrl route
                let summary =
                    ValidatedPost.description post
                    |> Option.defaultValue "Writing by Kevin M. Miller."

                XElement(
                    atom + "entry",
                    XElement(atom + "title", ValidatedPost.title post),
                    XElement(atom + "id", url),
                    XElement(atom + "link", XAttribute(XName.Get("href"), url)),
                    XElement(atom + "published", atomTimestamp (SourceIdentity.legacyDate identity)),
                    XElement(atom + "updated", atomTimestamp (SourceIdentity.legacyDate identity)),
                    XElement(atom + "summary", summary)
                ))

        let root =
            XElement(
                atom + "feed",
                XElement(atom + "title", "Kevin M. Miller — Writing"),
                XElement(atom + "id", fullUrl "/blog/"),
                XElement(atom + "link", XAttribute(XName.Get("href"), fullUrl "/feed.xml"), XAttribute(XName.Get("rel"), "self")),
                XElement(atom + "link", XAttribute(XName.Get("href"), fullUrl "/blog/")),
                XElement(atom + "updated", updated),
                XElement(atom + "author", XElement(atom + "name", "Kevin M. Miller")),
                entryElements
            )

        let document = XDocument(root)
        document.Declaration <- XDeclaration("1.0", "utf-8", null)
        document.ToString(SaveOptions.DisableFormatting)
