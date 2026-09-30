namespace KevinMiller.Site

open System
open System.Collections.Generic

module FrontMatterParser =
    let private unquote (value: string) =
        let trimmed = value.Trim()

        if trimmed.Length >= 2 then
            let first = trimmed[0]
            let last = trimmed[trimmed.Length - 1]

            if (first = '"' && last = '"') || (first = '\'' && last = '\'') then
                trimmed.Substring(1, trimmed.Length - 2)
            else
                trimmed
        else
            trimmed

    let private parseInlineList (value: string) =
        let trimmed = value.Trim()

        if trimmed.StartsWith("[", StringComparison.Ordinal)
           && trimmed.EndsWith("]", StringComparison.Ordinal) then
            trimmed.Substring(1, trimmed.Length - 2).Split(',', StringSplitOptions.RemoveEmptyEntries)
            |> Array.map unquote
            |> Array.map (fun item -> item.Trim())
            |> Array.filter (String.IsNullOrWhiteSpace >> not)
            |> Array.toList
        else
            [ unquote trimmed ]

    let private addValues
        (values: Dictionary<string, ResizeArray<string>>)
        (key: string)
        (items: string list)
        =
        let normalizedKey = key.Trim().ToLowerInvariant()

        let bucket =
            match values.TryGetValue(normalizedKey) with
            | true, existing -> existing
            | false, _ ->
                let created = ResizeArray<string>()
                values[normalizedKey] <- created
                created

        items |> List.iter bucket.Add

    let private firstValue key (values: Dictionary<string, ResizeArray<string>>) =
        match values.TryGetValue(key) with
        | true, bucket when bucket.Count > 0 -> Some bucket[0]
        | _ -> None

    let private allValues key (values: Dictionary<string, ResizeArray<string>>) =
        match values.TryGetValue(key) with
        | true, bucket -> List.ofSeq bucket
        | _ -> []

    let private tryBoolean (sourcePath: string) (key: string) (value: string) =
        match value.Trim().ToLowerInvariant() with
        | "true" -> Ok true
        | "false" -> Ok false
        | _ ->
            Error
                { Code = "FRONT-MATTER-BOOLEAN"
                  Severity = FindingSeverity.Error
                  SourcePath = sourcePath
                  Message = $"Front-matter key '{key}' must be true or false, not '{value}'." }

    let private legacyClosingFence (lines: string array) =
        let closingFence =
            lines
            |> Array.tryFindIndex (fun line -> line.Trim() = "---")

        match closingFence with
        | Some index when index > 0 && index <= 12 ->
            let looksLikeMetadata =
                lines[0 .. index - 1]
                |> Array.exists (fun line ->
                    let trimmed = line.TrimStart()
                    trimmed.StartsWith("title:", StringComparison.OrdinalIgnoreCase)
                    || trimmed.StartsWith("date:", StringComparison.OrdinalIgnoreCase)
                    || trimmed.StartsWith("layout:", StringComparison.OrdinalIgnoreCase))

            if looksLikeMetadata then Some index else None
        | _ -> None

    let private buildMetadata
        sourcePath
        (values: Dictionary<string, ResizeArray<string>>)
        =
        let published =
            match firstValue "published" values with
            | None -> Ok None
            | Some raw ->
                tryBoolean sourcePath "published" raw
                |> Result.map Some

        published
        |> Result.map (fun publishedValue ->
            let known =
                set
                    [ "layout"
                      "title"
                      "date"
                      "description"
                      "summary"
                      "author"
                      "published"
                      "categories"
                      "tags" ]

            let extensions =
                values
                |> Seq.choose (fun pair ->
                    if known.Contains(pair.Key) then
                        None
                    else
                        Some(pair.Key, List.ofSeq pair.Value))
                |> Map.ofSeq

            let description =
                match firstValue "description" values with
                | Some value -> Some value
                | None -> firstValue "summary" values

            { Layout = firstValue "layout" values
              Title = firstValue "title" values
              Date = firstValue "date" values
              Description = description
              Author = firstValue "author" values
              Published = publishedValue
              Categories = allValues "categories" values
              Tags = allValues "tags" values
              Extensions = extensions })

    let private parseMetadata
        sourcePath
        (lines: string array)
        startIndex
        closingIndex
        =
        let values = Dictionary<string, ResizeArray<string>>(StringComparer.OrdinalIgnoreCase)
        let errors = ResizeArray<PublicationFinding>()
        let mutable currentListKey: string option = None
        let mutable blockKey: string option = None
        let mutable blockStyle = ">"
        let blockLines = ResizeArray<string>()

        let commitBlock () =
            match blockKey with
            | None -> ()
            | Some key ->
                let content =
                    if blockStyle = "|" then
                        String.Join("\n", blockLines).Trim()
                    else
                        blockLines
                        |> Seq.map (fun line -> line.Trim())
                        |> Seq.filter (String.IsNullOrWhiteSpace >> not)
                        |> String.concat " "
                        |> fun value -> value.Trim()

                if not (String.IsNullOrWhiteSpace(content)) then
                    addValues values key [ content ]

                blockLines.Clear()
                blockKey <- None
                blockStyle <- ">"
                currentListKey <- None

        let mutable index = startIndex

        while index < closingIndex do
            let line = lines[index]
            let trimmed = line.Trim()

            match blockKey with
            | Some _ when String.IsNullOrWhiteSpace(line)
                          || (line.Length > 0 && Char.IsWhiteSpace(line[0])) ->
                blockLines.Add(if String.IsNullOrWhiteSpace(line) then "" else trimmed)
                index <- index + 1
            | Some _ ->
                commitBlock ()
            | None ->
                if String.IsNullOrWhiteSpace(trimmed) then
                    index <- index + 1
                elif trimmed.StartsWith("-", StringComparison.Ordinal)
                     && line.Length > 0
                     && Char.IsWhiteSpace(line[0]) then
                    match currentListKey with
                    | Some key ->
                        let item = trimmed.Substring(1).Trim() |> unquote

                        if not (String.IsNullOrWhiteSpace(item)) then
                            addValues values key [ item ]
                    | None ->
                        errors.Add
                            { Code = "FRONT-MATTER-LIST"
                              Severity = FindingSeverity.Error
                              SourcePath = sourcePath
                              Message = $"List item on metadata line {index + 1} has no owning key." }

                    index <- index + 1
                else
                    let separator = line.IndexOf(':')

                    if separator <= 0 then
                        errors.Add
                            { Code = "FRONT-MATTER-LINE"
                              Severity = FindingSeverity.Error
                              SourcePath = sourcePath
                              Message = $"Unsupported front-matter syntax on metadata line {index + 1}: {line}" }
                    else
                        let key = line.Substring(0, separator).Trim()
                        let value = line.Substring(separator + 1).Trim()
                        currentListKey <- Some key

                        if value = ">" || value = "|" then
                            blockKey <- Some key
                            blockStyle <- value
                            currentListKey <- None
                        elif not (String.IsNullOrWhiteSpace(value)) then
                            addValues values key (parseInlineList value)

                    index <- index + 1

        commitBlock ()

        if errors.Count > 0 then
            Error(List.ofSeq errors)
        else
            match buildMetadata sourcePath values with
            | Error finding -> Error [ finding ]
            | Ok metadata -> Ok metadata

    let private documentFromRange
        sourcePath
        (lines: string array)
        startIndex
        closingIndex
        findings
        =
        match parseMetadata sourcePath lines startIndex closingIndex with
        | Error errors -> Error errors
        | Ok metadata ->
            let body =
                if closingIndex + 1 >= lines.Length then
                    ""
                else
                    String.Join("\n", lines[(closingIndex + 1) ..]).TrimStart()

            Ok
                { Metadata = metadata
                  Body = body
                  Findings = findings }

    let parse sourcePath (source: string) =
        let normalized = source.Replace("\r\n", "\n")
        let lines = normalized.Split('\n')

        if lines.Length = 0 then
            Ok
                { Metadata = FrontMatter.empty
                  Body = ""
                  Findings = [] }
        elif lines[0].Trim() = "---" then
            let closingFence =
                lines
                |> Array.skip 1
                |> Array.tryFindIndex (fun line -> line.Trim() = "---")
                |> Option.map ((+) 1)

            match closingFence with
            | None ->
                Error
                    [ { Code = "FRONT-MATTER-CLOSE"
                        Severity = FindingSeverity.Error
                        SourcePath = sourcePath
                        Message = "Opening front-matter fence has no closing '---' fence." } ]
            | Some closingIndex ->
                documentFromRange sourcePath lines 1 closingIndex []
        else
            match legacyClosingFence lines with
            | Some closingIndex ->
                documentFromRange
                    sourcePath
                    lines
                    0
                    closingIndex
                    [ { Code = "FRONT-MATTER-OPEN"
                        Severity = FindingSeverity.Warning
                        SourcePath = sourcePath
                        Message =
                          "Legacy front matter is missing its opening '---' fence. It is accepted for migration compatibility and should not be used for new posts." } ]
            | None ->
                Ok
                    { Metadata = FrontMatter.empty
                      Body = normalized
                      Findings = [] }
