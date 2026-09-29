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

    let private tryBoolean sourcePath key value =
        match value.Trim().ToLowerInvariant() with
        | "true" -> Ok true
        | "false" -> Ok false
        | _ ->
            Error
                { Code = "FRONT-MATTER-BOOLEAN"
                  Severity = FindingSeverity.Error
                  SourcePath = sourcePath
                  Message = $"Front-matter key '{key}' must be true or false, not '{value}'." }

    let private looksLikeBrokenFrontMatter (lines: string array) =
        let closingFence =
            lines
            |> Array.tryFindIndex (fun line -> line.Trim() = "---")

        match closingFence with
        | Some index when index > 0 && index <= 12 ->
            lines[0 .. index - 1]
            |> Array.exists (fun line ->
                let trimmed = line.TrimStart()
                trimmed.StartsWith("title:", StringComparison.OrdinalIgnoreCase)
                || trimmed.StartsWith("date:", StringComparison.OrdinalIgnoreCase)
                || trimmed.StartsWith("layout:", StringComparison.OrdinalIgnoreCase))
        | _ -> false

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

            { Layout = firstValue "layout" values
              Title = firstValue "title" values
              Date = firstValue "date" values
              Description = firstValue "description" values
              Author = firstValue "author" values
              Published = publishedValue
              Categories = allValues "categories" values
              Tags = allValues "tags" values
              Extensions = extensions })

    let parse sourcePath (source: string) =
        let normalized = source.Replace("\r\n", "\n")
        let lines = normalized.Split('\n')

        if lines.Length = 0 then
            Ok { Metadata = FrontMatter.empty; Body = "" }
        elif lines[0].Trim() <> "---" then
            if looksLikeBrokenFrontMatter lines then
                Error
                    [ { Code = "FRONT-MATTER-OPEN"
                        Severity = FindingSeverity.Error
                        SourcePath = sourcePath
                        Message =
                          "Front-matter-like metadata is followed by a closing fence but the opening '---' fence is missing." } ]
            else
                Ok
                    { Metadata = FrontMatter.empty
                      Body = normalized }
        else
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
                let values = Dictionary<string, ResizeArray<string>>(StringComparer.OrdinalIgnoreCase)
                let errors = ResizeArray<PublicationFinding>()
                let mutable currentListKey: string option = None

                for index in 1 .. closingIndex - 1 do
                    let line = lines[index]
                    let trimmed = line.Trim()

                    if String.IsNullOrWhiteSpace(trimmed) then
                        ()
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

                            if not (String.IsNullOrWhiteSpace(value)) then
                                addValues values key (parseInlineList value)

                if errors.Count > 0 then
                    Error(List.ofSeq errors)
                else
                    match buildMetadata sourcePath values with
                    | Error finding -> Error [ finding ]
                    | Ok metadata ->
                        let body =
                            if closingIndex + 1 >= lines.Length then
                                ""
                            else
                                String.Join("\n", lines[(closingIndex + 1) ..]).TrimStart()

                        Ok
                            { Metadata = metadata
                              Body = body }
