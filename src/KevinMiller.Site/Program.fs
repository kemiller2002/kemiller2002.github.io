namespace KevinMiller.Site

open System
open System.IO

module Program =
    let private optionValue name (arguments: string array) =
        arguments
        |> Array.tryFindIndex ((=) name)
        |> Option.bind (fun index ->
            if index + 1 < arguments.Length then
                Some arguments[index + 1]
            else
                None)

    let private normalizeRelativePath root path =
        Path.GetRelativePath(root, path).Replace('\\', '/')

    let private printFinding finding =
        let severity =
            match finding.Severity with
            | FindingSeverity.Warning -> "WARNING"
            | FindingSeverity.Error -> "ERROR"

        printfn "%s %s %s: %s" severity finding.Code finding.SourcePath finding.Message

    let private validate root =
        let postsDirectory = Path.Combine(root, "site-src", "posts")

        if not (Directory.Exists(postsDirectory)) then
            eprintfn "ERROR SITE-SOURCE %s: post source directory does not exist." postsDirectory
            2
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

            findings
            |> Seq.sortBy (fun finding -> finding.SourcePath, finding.Code)
            |> Seq.iter printFinding

            let published =
                posts
                |> Seq.filter (fun post -> ValidatedPost.status post = PublicationStatus.Published)
                |> Seq.length

            let drafts =
                posts
                |> Seq.filter (fun post -> ValidatedPost.status post = PublicationStatus.Draft)
                |> Seq.length

            let errorCount =
                findings
                |> Seq.filter (fun finding -> finding.Severity = FindingSeverity.Error)
                |> Seq.length

            let warningCount =
                findings
                |> Seq.filter (fun finding -> finding.Severity = FindingSeverity.Warning)
                |> Seq.length

            printfn
                "Validated %d source files: %d publishable, %d drafts, %d errors, %d warnings."
                files.Length
                published
                drafts
                errorCount
                warningCount

            if errorCount = 0 then 0 else 1

    [<EntryPoint>]
    let main arguments =
        let root =
            optionValue "--root" arguments
            |> Option.defaultValue (Directory.GetCurrentDirectory())
            |> Path.GetFullPath

        let command =
            arguments
            |> Array.tryFind (fun value -> not (value.StartsWith("--", StringComparison.Ordinal)) && value <> root)
            |> Option.defaultValue "validate"

        match command with
        | "validate" -> validate root
        | other ->
            eprintfn "ERROR CLI: unsupported command '%s'. Use: validate [--root PATH]" other
            2
