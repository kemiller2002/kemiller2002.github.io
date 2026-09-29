namespace KevinMiller.Site

open System
open System.IO

type SiteAnalysis =
    { SourceCount: int
      Posts: ValidatedPost list
      Findings: PublicationFinding list }

module SiteBuild =
    let private normalizeRelativePath root path =
        Path.GetRelativePath(root, path).Replace('\\', '/')

    let private copyDirectory source destination exclusions =
        if Directory.Exists(source) then
            Directory.CreateDirectory(destination) |> ignore

            for file in Directory.EnumerateFiles(source, "*", SearchOption.AllDirectories) do
                let relative = Path.GetRelativePath(source, file).Replace('\\', '/')

                if not (Set.contains relative exclusions) then
                    let target = Path.Combine(destination, relative)
                    match Path.GetDirectoryName(target) with
                    | null -> ()
                    | targetDirectory -> Directory.CreateDirectory(targetDirectory) |> ignore

                    File.Copy(file, target, true)

    let analyze root =
        let postsDirectory = Path.Combine(root, "site-src", "posts")

        if not (Directory.Exists(postsDirectory)) then
            { SourceCount = 0
              Posts = []
              Findings =
                [ { Code = "SITE-SOURCE"
                    Severity = FindingSeverity.Error
                    SourcePath = postsDirectory
                    Message = "Post source directory does not exist." } ] }
        else
            let findings = ResizeArray<PublicationFinding>()
            let posts = ResizeArray<ValidatedPost>()

            let files =
                Directory.EnumerateFiles(postsDirectory, "*.md", SearchOption.TopDirectoryOnly)
                |> Seq.sort
                |> Seq.toList

            for file in files do
                let sourcePath = normalizeRelativePath root file

                match SourceIdentity.tryCreate sourcePath with
                | Error finding -> findings.Add(finding)
                | Ok identity ->
                    let raw = File.ReadAllText(file)

                    match FrontMatterParser.parse sourcePath raw with
                    | Error parseFindings -> parseFindings |> List.iter findings.Add
                    | Ok document ->
                        match Publication.validate identity document with
                        | Error validationFindings -> validationFindings |> List.iter findings.Add
                        | Ok(post, validationFindings) ->
                            validationFindings |> List.iter findings.Add

                            try
                                MarkdownRenderer.render (ValidatedPost.body post) |> ignore
                                posts.Add(post)
                            with error ->
                                findings.Add
                                    { Code = "MARKDOWN-RENDER"
                                      Severity = FindingSeverity.Error
                                      SourcePath = sourcePath
                                      Message = error.Message }

            Publication.uniqueRouteFindings posts
            |> List.iter findings.Add

            { SourceCount = files.Length
              Posts = List.ofSeq posts
              Findings = List.ofSeq findings }

    let private writeText (outputRoot: string) (relativePath: string) (content: string) =
        let target = Path.Combine(outputRoot, relativePath)

        match Path.GetDirectoryName(target) with
        | null -> ()
        | directory -> Directory.CreateDirectory(directory) |> ignore

        File.WriteAllText(target, content)

    let private renderPublishedPosts outputRoot posts =
        let published =
            posts
            |> List.filter (fun post -> ValidatedPost.status post = PublicationStatus.Published)
            |> List.sortByDescending (ValidatedPost.identity >> SourceIdentity.legacyDate)

        published
        |> List.iteri (fun index post ->
            let newer =
                if index = 0 then None else Some published[index - 1]

            let older =
                if index + 1 >= published.Length then None else Some published[index + 1]

            let bodyHtml = MarkdownRenderer.render (ValidatedPost.body post)

            let html =
                Rendering.article post bodyHtml older newer

            let outputPath =
                post
                |> ValidatedPost.identity
                |> SourceIdentity.outputPath

            writeText outputRoot outputPath html)

    let build root outputRoot =
        let analysis = analyze root

        let hasErrors =
            analysis.Findings
            |> List.exists (fun finding -> finding.Severity = FindingSeverity.Error)

        if hasErrors then
            Error analysis
        else
            if Directory.Exists(outputRoot) then
                Directory.Delete(outputRoot, true)

            Directory.CreateDirectory(outputRoot) |> ignore

            let published =
                analysis.Posts
                |> List.filter (fun post -> ValidatedPost.status post = PublicationStatus.Published)

            writeText outputRoot "index.html" (Rendering.home published)
            writeText outputRoot (Path.Combine("blog", "index.html")) (Rendering.archive published)
            renderPublishedPosts outputRoot analysis.Posts
            writeText outputRoot "feed.xml" (Syndication.atomFeed published)
            writeText outputRoot "sitemap.xml" (Syndication.sitemap published)
            writeText outputRoot "CNAME" "kevinmmiller.us\n"

            let legacyAssets = Path.Combine(root, "site-src", "assets")
            let contentAssetExclusions = set [ "custom.css"; "contact.js" ]
            copyDirectory legacyAssets outputRoot contentAssetExclusions

            let formaAssets = Path.Combine(root, "assets", "forma")
            let formaDestination = Path.Combine(outputRoot, "assets", "forma")
            copyDirectory formaAssets formaDestination Set.empty

            Ok analysis
