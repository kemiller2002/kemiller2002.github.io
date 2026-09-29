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

    let private printFinding finding =
        let severity =
            match finding.Severity with
            | FindingSeverity.Warning -> "WARNING"
            | FindingSeverity.Error -> "ERROR"

        printfn "%s %s %s: %s" severity finding.Code finding.SourcePath finding.Message

    let private printAnalysis analysis =
        analysis.Findings
        |> List.sortBy (fun finding -> finding.SourcePath, finding.Code)
        |> List.iter printFinding

        let published =
            analysis.Posts
            |> List.filter (fun post -> ValidatedPost.status post = PublicationStatus.Published)
            |> List.length

        let drafts =
            analysis.Posts
            |> List.filter (fun post -> ValidatedPost.status post = PublicationStatus.Draft)
            |> List.length

        let errorCount =
            analysis.Findings
            |> List.filter (fun finding -> finding.Severity = FindingSeverity.Error)
            |> List.length

        let warningCount =
            analysis.Findings
            |> List.filter (fun finding -> finding.Severity = FindingSeverity.Warning)
            |> List.length

        printfn
            "Analyzed %d source files: %d publishable, %d drafts, %d errors, %d warnings."
            analysis.SourceCount
            published
            drafts
            errorCount
            warningCount

        errorCount

    let private resolveOutput root rawOutput =
        if Path.IsPathRooted(rawOutput) then
            Path.GetFullPath(rawOutput)
        else
            Path.GetFullPath(Path.Combine(root, rawOutput))

    [<EntryPoint>]
    let main arguments =
        let command =
            arguments
            |> Array.tryHead
            |> Option.filter (fun value -> not (value.StartsWith("--", StringComparison.Ordinal)))
            |> Option.defaultValue "validate"

        let root =
            optionValue "--root" arguments
            |> Option.defaultValue (Directory.GetCurrentDirectory())
            |> Path.GetFullPath

        match command with
        | "validate" ->
            let analysis = SiteBuild.analyze root
            let errors = printAnalysis analysis
            if errors = 0 then 0 else 1

        | "build" ->
            let outputRoot =
                optionValue "--out" arguments
                |> Option.defaultValue "dist-v2"
                |> resolveOutput root

            match SiteBuild.build root outputRoot with
            | Error analysis ->
                printAnalysis analysis |> ignore
                eprintfn "ERROR BUILD: site output was not written because validation failed."
                1
            | Ok analysis ->
                let errors = printAnalysis analysis

                if errors = 0 then
                    printfn "Built static site at %s" outputRoot
                    0
                else
                    1

        | other ->
            eprintfn "ERROR CLI: unsupported command '%s'." other
            eprintfn "Usage: validate [--root PATH] | build [--root PATH] [--out PATH]"
            2
