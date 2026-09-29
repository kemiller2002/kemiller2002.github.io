namespace KevinMiller.Site

open System
open System.IO
open System.Security.Cryptography
open System.Text.RegularExpressions

type DeterminismResult =
    | Deterministic
    | Different of string list
    | BuildFailed of PublicationFinding list

module Verification =
    let private fileDigest (path: string) =
        use stream = File.OpenRead(path)
        use sha = SHA256.Create()
        sha.ComputeHash(stream)
        |> Convert.ToHexString

    let private snapshot (root: string) =
        Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories)
        |> Seq.map (fun path ->
            let relative = Path.GetRelativePath(root, path).Replace('\\', '/')
            relative, fileDigest path)
        |> Map.ofSeq

    let private differences (left: Map<string, string>) (right: Map<string, string>) =
        let keys map =
            map
            |> Map.toSeq
            |> Seq.map fst
            |> Set.ofSeq

        let paths =
            Set.union (keys left) (keys right)

        paths
        |> Seq.choose (fun path ->
            match Map.tryFind path left, Map.tryFind path right with
            | Some a, Some b when a = b -> None
            | Some a, Some b -> Some $"{path}: {a} != {b}"
            | Some _, None -> Some $"{path}: missing from second build"
            | None, Some _ -> Some $"{path}: missing from first build"
            | None, None -> None)
        |> Seq.toList

    let verifyDeterministicBuild root =
        let tempRoot =
            Path.Combine(Path.GetTempPath(), "kevinmiller-site-determinism-" + Guid.NewGuid().ToString("N"))

        let first = Path.Combine(tempRoot, "first")
        let second = Path.Combine(tempRoot, "second")

        try
            match SiteBuild.build root first with
            | Error analysis -> BuildFailed analysis.Findings
            | Ok _ ->
                match SiteBuild.build root second with
                | Error analysis -> BuildFailed analysis.Findings
                | Ok _ ->
                    let delta = differences (snapshot first) (snapshot second)
                    if List.isEmpty delta then Deterministic else Different delta
        finally
            if Directory.Exists(tempRoot) then
                Directory.Delete(tempRoot, true)


    let private normalizeRelativePath (path: string) =
        path.Replace('\\', '/')

    let private requiredStaticOutputs =
        [ "index.html"
          "blog/index.html"
          "about/index.html"
          "contact/index.html"
          "echelon-systems/index.html"
          "talks.html"
          "speaker-bio.html"
          "404.html"
          "feed.xml"
          "sitemap.xml"
          "CNAME" ]

    let private rootedReferencePattern =
        Regex("""(?i)(?:href|src)\s*=\s*["'](?<url>/[^"']*)["']""", RegexOptions.Compiled)

    let private referencePath (url: string) =
        if url.StartsWith("//", StringComparison.Ordinal) then
            None
        else
            let separator = url.IndexOfAny([| '?'; '#' |])
            let rawPath = if separator >= 0 then url.Substring(0, separator) else url

            if String.IsNullOrWhiteSpace(rawPath) then
                None
            else
                Some rawPath

    let private outputRelativeForPublicPath (publicPath: string) =
        let decoded = Uri.UnescapeDataString(publicPath)
        let trimmed = decoded.TrimStart('/')

        if publicPath = "/" then
            "index.html"
        elif publicPath.EndsWith("/", StringComparison.Ordinal) then
            Path.Combine(trimmed, "index.html") |> normalizeRelativePath
        else
            trimmed |> normalizeRelativePath

    let verifyGeneratedSite root outputRoot =
        let findings = ResizeArray<PublicationFinding>()

        if not (Directory.Exists(outputRoot)) then
            [ { Code = "SITE-OUTPUT"
                Severity = FindingSeverity.Error
                SourcePath = outputRoot
                Message = "Generated site output directory does not exist." } ]
        else
            let analysis = SiteBuild.analyze root

            let expectedPostOutputs =
                analysis.Posts
                |> List.filter (fun post -> ValidatedPost.status post = PublicationStatus.Published)
                |> List.map (ValidatedPost.identity >> SourceIdentity.outputPath >> normalizeRelativePath)

            let legacyRouteManifest =
                Path.Combine(root, "migration", "legacy-html-routes.txt")

            let legacyHtmlOutputs =
                if File.Exists(legacyRouteManifest) then
                    File.ReadAllLines(legacyRouteManifest)
                    |> Array.map (fun line -> line.Trim())
                    |> Array.filter (fun line ->
                        not (String.IsNullOrWhiteSpace(line))
                        && not (line.StartsWith("#", StringComparison.Ordinal)))
                    |> Array.toList
                else
                    []

            let expectedOutputs =
                requiredStaticOutputs @ expectedPostOutputs @ legacyHtmlOutputs
                |> Set.ofList

            for relative in expectedOutputs do
                let path = Path.Combine(outputRoot, relative)

                if not (File.Exists(path)) then
                    findings.Add
                        { Code = "SITE-OUTPUT-MISSING"
                          Severity = FindingSeverity.Error
                          SourcePath = relative
                          Message =
                            if List.contains relative legacyHtmlOutputs then
                                "Legacy published HTML route is missing from the F# replacement output."
                            else
                                "Expected generated public artifact is missing." }

            let formaLockPath = Path.Combine(root, "forma.lock")
            let formaLocked = File.Exists(formaLockPath)

            if formaLocked then
                let pinnedAsset =
                    File.ReadAllLines(formaLockPath)
                    |> Array.tryPick (fun line ->
                        let prefix = "asset forma-echelon-marketing.css sha256:"

                        if line.StartsWith(prefix, StringComparison.Ordinal) then
                            Some(line.Substring(prefix.Length).Trim().ToLowerInvariant())
                        else
                            None)

                match pinnedAsset with
                | None ->
                    findings.Add
                        { Code = "SITE-FORMA-LOCK"
                          Severity = FindingSeverity.Error
                          SourcePath = "forma.lock"
                          Message = "Forma lock does not contain a checksum for forma-echelon-marketing.css." }
                | Some expectedHash ->
                    let installedPath =
                        Path.Combine(root, "assets", "forma", "forma-echelon-marketing.css")

                    if not (File.Exists(installedPath)) then
                        findings.Add
                            { Code = "SITE-FORMA-MISSING"
                              Severity = FindingSeverity.Error
                              SourcePath = "assets/forma/forma-echelon-marketing.css"
                              Message = "Pinned Forma marketing CSS was not installed before site verification." }
                    else
                        let actualHash = fileDigest installedPath |> fun value -> value.ToLowerInvariant()

                        if actualHash <> expectedHash then
                            findings.Add
                                { Code = "SITE-FORMA-CHECKSUM"
                                  Severity = FindingSeverity.Error
                                  SourcePath = "assets/forma/forma-echelon-marketing.css"
                                  Message =
                                    $"Installed Forma marketing CSS checksum {actualHash} does not match forma.lock {expectedHash}." }

            let checkedReferences = Collections.Generic.HashSet<string>(StringComparer.Ordinal)

            for htmlPath in Directory.EnumerateFiles(outputRoot, "*.html", SearchOption.AllDirectories) do
                let html = File.ReadAllText(htmlPath)
                let sourcePath = Path.GetRelativePath(outputRoot, htmlPath) |> normalizeRelativePath

                for retired in [ "/custom.css"; "/contact.js" ] do
                    if html.Contains(retired, StringComparison.OrdinalIgnoreCase) then
                        findings.Add
                            { Code = "SITE-LEGACY-ASSET"
                              Severity = FindingSeverity.Error
                              SourcePath = sourcePath
                              Message = $"Generated HTML still references retired asset '{retired}'." }

                for matched in rootedReferencePattern.Matches(html) do
                    let url = matched.Groups["url"].Value

                    match referencePath url with
                    | None -> ()
                    | Some publicPath when checkedReferences.Add(publicPath) ->
                        try
                            let relative = outputRelativeForPublicPath publicPath
                            let target = Path.Combine(outputRoot, relative)

                            if not (File.Exists(target)) then
                                let isPendingForma =
                                    publicPath = "/assets/forma/forma-echelon-marketing.css"
                                    && not formaLocked

                                findings.Add
                                    { Code =
                                        if isPendingForma then
                                            "SITE-FORMA-PENDING"
                                        else
                                            "SITE-LINK-MISSING"
                                      Severity =
                                        if isPendingForma then
                                            FindingSeverity.Warning
                                        else
                                            FindingSeverity.Error
                                      SourcePath = sourcePath
                                      Message =
                                        if isPendingForma then
                                            "Forma marketing CSS is not installed yet because forma.lock is not present."
                                        else
                                            $"Root-local reference '{url}' resolves to missing generated artifact '{relative}'." }
                        with error ->
                            findings.Add
                                { Code = "SITE-LINK-INVALID"
                                  Severity = FindingSeverity.Error
                                  SourcePath = sourcePath
                                  Message = $"Could not resolve root-local reference '{url}': {error.Message}" }
                    | Some _ -> ()

            List.ofSeq findings
