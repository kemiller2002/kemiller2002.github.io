namespace KevinMiller.Site

open System
open System.Globalization
open System.IO
open System.Text.RegularExpressions

[<RequireQualifiedAccess>]
type FindingSeverity =
    | Warning
    | Error

type PublicationFinding =
    { Code: string
      Severity: FindingSeverity
      SourcePath: string
      Message: string }

[<RequireQualifiedAccess>]
type PublicationStatus =
    | Published
    | Draft

type SourceIdentity =
    { SourcePath: string
      FileName: string
      Year: int
      Month: int
      Day: int
      Slug: string }

module SourceIdentity =
    let private filePattern =
        Regex(
            "^(?<year>\\d{4})-(?<month>\\d{1,2})-(?<day>\\d{1,2})-(?<slug>.+)\\.md$",
            RegexOptions.Compiled
        )

    let tryCreate (sourcePath: string) =
        match Path.GetFileName(sourcePath) with
        | null ->
            Error
                { Code = "POST-FILENAME"
                  Severity = FindingSeverity.Error
                  SourcePath = sourcePath
                  Message = "Source path has no filename." }
        | fileName ->
            let matched = filePattern.Match(fileName)

            if not matched.Success then
                Error
                    { Code = "POST-FILENAME"
                      Severity = FindingSeverity.Error
                      SourcePath = sourcePath
                      Message = $"Unsupported post filename format: {fileName}" }
            else
                let year = Int32.Parse(matched.Groups["year"].Value, CultureInfo.InvariantCulture)
                let month = Int32.Parse(matched.Groups["month"].Value, CultureInfo.InvariantCulture)
                let day = Int32.Parse(matched.Groups["day"].Value, CultureInfo.InvariantCulture)
                let slug = matched.Groups["slug"].Value

                try
                    DateOnly(year, month, day) |> ignore

                    Ok
                        { SourcePath = sourcePath
                          FileName = fileName
                          Year = year
                          Month = month
                          Day = day
                          Slug = slug }
                with :? ArgumentOutOfRangeException ->
                    Error
                        { Code = "POST-FILENAME-DATE"
                          Severity = FindingSeverity.Error
                          SourcePath = sourcePath
                          Message = $"Filename contains an invalid calendar date: {fileName}" }

    let legacyDate identity = DateOnly(identity.Year, identity.Month, identity.Day)

    let urlPath identity =
        let encodedSlug = Uri.EscapeDataString(identity.Slug)
        sprintf "/%04d/%02d/%02d/%s.html" identity.Year identity.Month identity.Day encodedSlug

    let outputPath identity =
        Path.Combine(
            string identity.Year,
            sprintf "%02d" identity.Month,
            sprintf "%02d" identity.Day,
            $"{identity.Slug}.html"
        )

type FrontMatter =
    { Layout: string option
      Title: string option
      Date: string option
      Description: string option
      Author: string option
      Published: bool option
      Categories: string list
      Tags: string list
      Extensions: Map<string, string list> }

module FrontMatter =
    let empty =
        { Layout = None
          Title = None
          Date = None
          Description = None
          Author = None
          Published = None
          Categories = []
          Tags = []
          Extensions = Map.empty }

type ParsedDocument =
    { Metadata: FrontMatter
      Body: string
      Findings: PublicationFinding list }

type ValidatedPost =
    private
        { Identity: SourceIdentity
          Title: string
          Description: string option
          Author: string option
          Status: PublicationStatus
          Categories: string list
          Tags: string list
          Body: string
          DeclaredDate: DateOnly }

module ValidatedPost =
    let identity post = post.Identity
    let title post = post.Title
    let description post = post.Description
    let author post = post.Author
    let status post = post.Status
    let categories post = post.Categories
    let tags post = post.Tags
    let body post = post.Body
    let declaredDate post = post.DeclaredDate
    let route post = SourceIdentity.urlPath post.Identity

module Publication =
    let private tryDateOnly (raw: string) =
        let value =
            if String.IsNullOrWhiteSpace(raw) then
                ""
            else
                raw.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries)
                |> Array.tryHead
                |> Option.defaultValue ""

        let formats =
            [| "yyyy-MM-dd"
               "yyyy-M-d"
               "yyyy-M-dd"
               "yyyy-MM-d" |]

        match DateOnly.TryParseExact(value, formats, CultureInfo.InvariantCulture, DateTimeStyles.None) with
        | true, parsed -> Some parsed
        | false, _ -> None

    let validate (identity: SourceIdentity) (document: ParsedDocument) =
        let errors = ResizeArray<PublicationFinding>()
        let warnings = ResizeArray<PublicationFinding>()

        document.Findings
        |> List.iter (fun finding ->
            match finding.Severity with
            | FindingSeverity.Warning -> warnings.Add(finding)
            | FindingSeverity.Error -> errors.Add(finding))

        let title =
            match document.Metadata.Title with
            | Some value when not (String.IsNullOrWhiteSpace(value)) -> Some(value.Trim())
            | _ ->
                errors.Add
                    { Code = "POST-TITLE"
                      Severity = FindingSeverity.Error
                      SourcePath = identity.SourcePath
                      Message = "Published source requires a non-empty title in front matter." }

                None

        let declaredDate =
            match document.Metadata.Date with
            | Some raw ->
                match tryDateOnly raw with
                | Some date -> Some date
                | None ->
                    errors.Add
                        { Code = "POST-DATE"
                          Severity = FindingSeverity.Error
                          SourcePath = identity.SourcePath
                          Message = $"Front-matter date is not supported: {raw}" }

                    None
            | None ->
                errors.Add
                    { Code = "POST-DATE"
                      Severity = FindingSeverity.Error
                      SourcePath = identity.SourcePath
                      Message = "Published source requires a date in front matter." }

                None

        if identity.Slug.IndexOfAny([| '#'; '?'; '%' |]) >= 0 then
            warnings.Add
                { Code = "POST-SLUG-RESERVED"
                  Severity = FindingSeverity.Warning
                  SourcePath = identity.SourcePath
                  Message =
                    $"Legacy slug '{identity.Slug}' contains a URL-reserved character. The physical output filename is preserved while the public URL path segment is percent-encoded." }

        match declaredDate with
        | Some date when date <> SourceIdentity.legacyDate identity ->
            let declaredText = date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)

            let legacyText =
                (SourceIdentity.legacyDate identity).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)

            warnings.Add
                { Code = "POST-DATE-DISAGREEMENT"
                  Severity = FindingSeverity.Warning
                  SourcePath = identity.SourcePath
                  Message =
                    $"Front-matter date {declaredText} disagrees with filename date {legacyText}. The filename remains the legacy route authority." }
        | _ -> ()

        if errors.Count > 0 then
            Error(List.ofSeq errors @ List.ofSeq warnings)
        else
            let status =
                match document.Metadata.Published with
                | Some false -> PublicationStatus.Draft
                | _ -> PublicationStatus.Published

            let requiredTitle = title |> Option.get
            let requiredDate = declaredDate |> Option.get

            Ok(
                { Identity = identity
                  Title = requiredTitle
                  Description = document.Metadata.Description
                  Author = document.Metadata.Author
                  Status = status
                  Categories = document.Metadata.Categories
                  Tags = document.Metadata.Tags
                  Body = document.Body
                  DeclaredDate = requiredDate },
                List.ofSeq warnings
            )


    let uniqueRouteFindings (posts: ValidatedPost seq) =
        posts
        |> Seq.groupBy ValidatedPost.route
        |> Seq.choose (fun (route, matchingPosts) ->
            let sources =
                matchingPosts
                |> Seq.map (ValidatedPost.identity >> fun identity -> identity.SourcePath)
                |> Seq.toList

            if sources.Length <= 1 then
                None
            else
                Some
                    { Code = "POST-ROUTE-COLLISION"
                      Severity = FindingSeverity.Error
                      SourcePath = String.Join(", ", sources)
                      Message = $"Multiple sources generate the same route '{route}'." })
        |> Seq.toList
